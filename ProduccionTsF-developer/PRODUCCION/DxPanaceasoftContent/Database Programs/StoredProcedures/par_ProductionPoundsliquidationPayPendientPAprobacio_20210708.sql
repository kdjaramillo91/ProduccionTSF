-- USE [PanaceaProduccionProcamaronex]
GO
/****** Object:  StoredProcedure [dbo].[par_ProductionPoundsliquidationPayPendientPAprobacion]    Script Date: 08/07/2021 15:37:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




ALTER Procedure [dbo].[par_ProductionPoundsliquidationPayPendientPAprobacion]
@id varchar(50) = '',
@fi varchar(10) = '',
@ff varchar(10) = '',
@proveedor int = 0
as 
set nocount on

if @fi = '' set @fi = null
if @ff = '' set @ff = null

declare @hasAdvance varchar(10)
declare @fiDt datetime
declare @ffDt datetime

set @id = isnull(@id,'')
set @fiDt = convert(date,isnull(@fi,'1900-01-01'))
set @ffDt = convert(date,isnull(@ff,'1900-01-01'))

set @hasAdvance = (select top 1 valueInteger from AdvanceParameters where code = 'TANT')
if(isnull(@hasAdvance,'') = '') 
 begin 
    RAISERROR('No se ha especificado Parámetro de Sistema con código TANT',1,1); 
 end


CREATE TABLE #AdvanceProviderAprovedTemp(
	id_advanceProvider INT
)

if(@hasAdvance = '1')
Begin
	INSERT INTO #AdvanceProviderAprovedTemp
	select ap.id
	from document d
	inner join AdvanceProvider ap
	on d.id = ap.id
	inner join documentState ds
	on ds.id = d.id_documentState
	where ds.code <> '05'
End

if(@hasAdvance = '1')
	Begin
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
				Company.logo2 as foto2,
				proceso = PT.[name],
				replace(replace(replace(replace(replace(left(Pricel.name,4),'C',' '),'.',' '),'O',' '),'R',' '),'E',' ') as PriceList,
				pl.sequentialLiquidation,
				pup.name as camaronera,
				ap.valueAdvanceTotalRounded as Anticipo,
				g.value as gramage,
				pls.name as estado,
				CONVERT(VARCHAR(10),processPlant.processPlant) as ProcesoPlanta
		  from   [dbo].ProductionLot  pl
		  inner join Person processPlant
			on processPlant.id = pl.id_personProcessPlant
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
		  INNER join [dbo].AdvanceProvider ap
			   on ap.id_lot=pl.id
		  INNER join [dbo].Grammage g
		       on g.id=ap.id_grammage
		  INNER JOIN [dbo].ProductionLotState pls
		       on pls.id=pl.id_ProductionLotState
		   INNER JOIN [dbo].#AdvanceProviderAprovedTemp advanceTemp
			   on advanceTemp.id_advanceProvider = ap.id
		where pl.internalNumber like case when @id = '' then pl.internalNumber
											else '%' + @id + '%' end
		and convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.[receptionDate]) else @fiDt end
		and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date, pl.[receptionDate]) else @ffDt end
		and pl.[id_provider] = case when @proveedor = 0 then pl.[id_provider] else @proveedor end
		and pls.Code = '07'
		order by pl.sequentialLiquidation asc
	End
Else
	Begin
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
				Company.logo2 as foto2,
				proceso = PT.[name],
				replace(replace(replace(replace(replace(left(Pricel.name,4),'C',' '),'.',' '),'O',' '),'R',' '),'E',' ') as PriceList,
				pl.sequentialLiquidation,
				pup.name as camaronera,
				--- ap.valueAdvanceTotalRounded as Anticipo,
				--g.value as gramage,
				0 as Anticipo,
				0 as gramage,
				pls.name as estado,
				CONVERT(VARCHAR(10),processPlant.processPlant) as ProcesoPlanta
		  from   [dbo].ProductionLot  pl
		  inner join Person processPlant
			on processPlant.id = pl.id_personProcessPlant
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
		  -- INNER join [dbo].AdvanceProvider ap
		  --	   on ap.id_lot=pl.id
		  -- INNER join [dbo].Grammage g
		  --      on g.id=ap.id_grammage
		  INNER JOIN [dbo].ProductionLotState pls
		       on pls.id=pl.id_ProductionLotState
		  -- INNER JOIN [dbo].#AdvanceProviderAprovedTemp advanceTemp
		  --	   on advanceTemp.id_advanceProvider = ap.id
		where pl.internalNumber like case when @id = '' then pl.internalNumber
											else '%' + @id + '%' end
		and convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.[receptionDate]) else @fiDt end
		and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date, pl.[receptionDate]) else @ffDt end
		and pl.[id_provider] = case when @proveedor = 0 then pl.[id_provider] else @proveedor end
		and pls.Code = '07'
		order by pl.sequentialLiquidation asc
	End



--select * from ProductionLotState
-- execute par_ProductionPoundsliquidationPayPendientPAprobacion '','','',0




