/****** Object:  StoredProcedure [dbo].[spPar_LibrasLiquidadasSequencial]    Script Date: 15/06/2023 04:12:54 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER     PROC [dbo].[spPar_LibrasLiquidadasSequencial]
@id varchar(50) = '',
@fi varchar(10) = '',
@ff varchar(10) = '',
@proveedor int = 0
as 
set nocount on

if @fi = '' set @fi = null
if @ff = '' set @ff = null

declare @fiDt datetime
declare @ffDt datetime

set @id = isnull(@id,'')
set @fiDt = convert(date,isnull(@fi,'1900-01-01'))
set @ffDt = convert(date,isnull(@ff,'1900-01-01'))
select 
       pl.id  as  PL_id ,
       pl.number  as  PL_number ,
       pl.internalNumber  as  PL_internalNumber ,
       pl.barCode  as  PL_barCode ,
       pl.id_ProductionLotState  as  PL_id_ProductionLotState ,
       pl.id_productionUnit  as  PL_id_productionUnit ,
       pl.id_productionProcess  as  PL_id_productionProcess ,
       pl.receptionDate  as  PL_receptionDate ,
       pl.id_provider  as  PL_id_provider ,
       pl.liquidationDate  as  PL_liquidationDate ,
       pl.closeDate  as  PL_closeDate ,
	   pl.totalQuantityLiquidationAdjust  as  lbsprocesadas,
	   pl.totalQuantityRemitted  as  lbsremitidas ,
       pl.liquidationPaymentDate  as  PL_liquidationPaymentDate ,
       pl.id_company  as  PL_id_company ,
       pl.id_userCreate  as  PL_id_userCreate ,
       pl.dateCreate  as  PL_dateCreate ,
       pl.id_userUpdate  as  PL_id_userUpdate ,
       pl.dateUpdate  as  PL_dateUpdate ,
       pl.id_productionUnitProviderPool  as  PL_id_productionUnitProviderPool ,
       pl.id_productionUnitProvider  as  PL_id_productionUnitProvider ,
       pl.id_providerapparent  as  PL_id_providerapparent ,
       pl.id_processtype  as  PL_id_processtype ,
       pl.wholeSubtotal  as  lbsentero ,
       pl.subtotalTail  as  lbscola ,
       pl.wholeGarbagePounds  as  basuraentero ,
       pl.poundsGarbageTail  as  basuracola ,
       pl.wholeLeftover  as  PL_wholeLeftover ,
       pl.totalAdjustmentPounds  as  PL_totalAdjustmentPounds ,
       pl.totalAdjustmentWholePounds  as  PL_totalAdjustmentWholePounds ,
       pl.totalAdjustmentTailPounds  as  PL_totalAdjustmentTailPounds ,
       pl.wholeSubtotalAdjust  as  PL_wholeSubtotalAdjust ,
       pl.subtotalTailAdjust  as  PL_subtotalTailAdjust ,
       pl.wholeSubtotalAdjustProcess  as  PL_wholeSubtotalAdjustProcess ,
       pl.totalToPay  as  PL_totalToPay ,
       pl.wholeTotalToPay  as  PL_wholeTotalToPay ,
       pl.tailTotalToPay  as  PL_tailTotalToPay ,
       QC_QuantityPoundsReceived = isnull((select sum(isnull(quantitydrained,0))
					from [dbo].[ProductionLotDetail] pldTmp3 
					where pldTmp3.[id_productionLot] = pl.[id]),0),
	   pupp.name as  NamePool ,
	   convert(varchar(250),prs.fullname_businessName) as  Nameproveedor ,
	   converT(varchar(250),pup.[name]) as ProductionUnitProviderName,
	   lbsrecibidas = (select SUM(QuantityPoundsReceived) from QualityControl qcc where qcc.id_lot = pl.id),
	   prs.identification_number as identity_prov,
	   Company.businessName as name_cia,
	   Company.ruc as ruc_cia,
	   company.address as adreess_cia,
	   Company.phoneNumber as telephone_cia,
		proceso = PT.[name],
		PriceL.name as PriceList,
		pl.sequentialLiquidation,
		pup.name
		, CASE WHEN CONVERT(VARCHAR,year(@fiDt)) = '1900' then 'TODOS' ELSE CONVERT(VARCHAR(10), @fiDt, 103) END AS Fi
		, CASE WHEN CONVERT(VARCHAR,year(@ffDt)) = '1900' then 'TODOS' ELSE CONVERT(VARCHAR(10), @ffDt, 103) END AS Ff
		,CONVERT(VARCHAR(250),plant.processPlant) as ProcessPlant
  from   [dbo].ProductionLot  pl
  INNER join [Person] plant
	on plant.id = pl.id_personProcessPlant
   left join Pricelist  PriceL 
   on ( PriceL.id  = pl.id_priceList )
   inner join ProcessType PT
    on PT.id = pl.id_processtype
  inner join Person prs
       on prs.id  = pl.id_provider
  inner join ProductionUnitProvider pup
   on pup.id = pl.id_productionUnitProvider
  inner join ProductionUnitProviderPool pupp
		on pupp.id = pl.id_productionUnitProviderPool
  inner join Document
       on Document.id = pl.id
  INNER join [dbo].EmissionPoint   EmissionPoint 
       on  EmissionPoint .id  =  Document .id_emissionPoint 
  INNER join [dbo].Company   Company 
       on  Company .id  =  EmissionPoint .id_company 
  INNER join [dbo].ProductionLotState pls
       on pls.id=pl.id_ProductionLotState
where pl.internalNumber like case when @id = '' then pl.internalNumber
									else '%' + @id + '%' end
and convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.[receptionDate]) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date, pl.[receptionDate]) else @ffDt end
and pl.[id_provider] = case when @proveedor = 0 then pl.[id_provider] else @proveedor end
and pls.code in ('06','07','08')
order by pl.sequentialLiquidation asc

/*
	EXEC spPar_LibrasLiquidadasSequencial @id=N'',@fi=N'2022/02/01',@ff=N'2022/03/31',@proveedor=0
*/
GO


