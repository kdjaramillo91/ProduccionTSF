/****** Object:  StoredProcedure [dbo].[spPar_MateriaPrimaPendiente]    Script Date: 09/05/2023 11:13:11 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROC [dbo].[spPar_MateriaPrimaPendiente]
@id varchar(50) = '',
@fi varchar(10) = '',
@ff varchar(10) = '',
@proveedor int
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
       pl.totalQuantityOrdered  as  PL_totalQuantityOrdered ,
       pl.totalQuantityRemitted  as  lbsremitidas ,
       pl.totalQuantityRecived  as  PL_totalQuantityRecived ,
       pl.totalQuantityLiquidation  as  PL_totalQuantityLiquidation ,
       pl.totalQuantityTrash  as  PL_totalQuantityTrash ,
       pl.totalQuantityLiquidationAdjust  as  PL_totalQuantityLiquidationAdjust ,
       pl.withPrice  as  PL_withPrice ,
       pl.pricePerLbs  as  PL_pricePerLbs ,
       pl.liquidationDate  as  PL_liquidationDate ,
       pl.closeDate  as  PL_closeDate ,
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
       pl.wholeSubtotal  as  PL_wholeSubtotal ,
       pl.subtotalTail  as  PLsubtotalTail ,
       pl.wholeGarbagePounds  as  PL_wholeGarbagePounds ,
       pl.poundsGarbageTail  as  PL_poundsGarbageTail ,
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
	   prs.fullname_businessName as  Nameproveedor ,
	   dbo.FUN_GetRemissionGuide(pl.id) as RemissionGuide,
	   dbo.FUN_GetPurchaseOrder(pl.id) as PurchaseOrder,
	    prs.identification_number as identity_prov,
		Company.businessName as name_cia,
		Company.ruc as ruc_cia,
		company.address as adreess_cia,
		Company.phoneNumber as telephone_cia,
		CASE WHEN CONVERT(VARCHAR,year(@fiDt)) = '1900' then 'TODOS' ELSE CONVERT(VARCHAR, @fiDt, 103) END AS Fi,
		CASE WHEN CONVERT(VARCHAR,year(@ffDt)) = '1900' then 'TODOS' ELSE CONVERT(VARCHAR, @ffDt, 103) END AS Ff,
		PriceL.name as PriceList,
		rendimiento = (select SUM(wholePerformance)/COUNT(wholePerformance) from QualityControl qcc , ProductionLotDetailQualityControl pldqc , ProductionLotDetail pld
		                                                                              where pldqc.id_productionLotDetail = pld.id and
																					  qcc.id_lot= pld.id_productionLot 
																					  and qcc.id_lot = pl.id),
		lbsrecibidas = (select SUM(QuantityPoundsReceived) from QualityControl qcc where qcc.id_lot = pl.id),
		gramagentero =  (select SUM(grammagereference)/COUNT(grammagereference) from QualityControl qcc , ProductionLotDetailQualityControl pldqc , ProductionLotDetail pld
		                                                                              where pldqc.id_productionLotDetail = pld.id and
																					  qcc.id_lot= pld.id_productionLot 
																					  and qcc.id_lot = pl.id 
																					  ),
         rentero =   (select SUM(wholePerformance)/COUNT(wholePerformance) from QualityControl qcc , ProductionLotDetailQualityControl pldqc , ProductionLotDetail pld
		                                                                              where pldqc.id_productionLotDetail = pld.id and
																					  qcc.id_lot= pld.id_productionLot 
																					  and qcc.id_lot = pl.id
																					  ) ,
		pup.name as camaronera,
		producto = (select top 1 it.name
					from [dbo].[ProductionLotDetail] pldTmp2 
					inner join [dbo].[Item] it on pldTmp2.[id_item] =it.[id]
					where pldTmp2.[id_productionLot] = pl.[id]),
		proceso = PT.[name],
		pls.name as estadolote,
		CONVERT(VARCHAR(10),processPlant.processPlant) as ProcesoPlanta
from   [dbo].ProductionLot  pl
 inner join Person processPlant
	on processPlant.id = pl.id_personProcessPlant
left outer join Pricelist  PriceL 
	on ( PriceL .id  = pl.id_priceList )
left join ProcessType PT
	on PT.id = pl.id_processtype
inner join Person prs
    on prs.id  = pl.id_provider
inner join ProductionUnitProvider pup
	on pup.id_provider = pl.id_provider
   and pup.id = pl.id_productionUnitProvider
inner join ProductionUnitProviderPool pupp
	on pupp.id = pl.id_productionUnitProviderPool
inner join Document
    on Document.id = pl.id
INNER join [dbo].EmissionPoint   EmissionPoint 
    on  EmissionPoint .id  =  Document .id_emissionPoint 
INNER join [dbo].Company   Company 
    on  Company .id  =  EmissionPoint .id_company
inner join ProductionLotState pls
	on pls.id = pl.id_ProductionLotState 
where pl.internalNumber like case when @id = '' then pl.internalNumber
									else '%' + @id + '%' end
and convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.[receptionDate]) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date, pl.[receptionDate]) else @ffDt end
and pl.[id_provider] = case when @proveedor = 0 then pl.[id_provider] else @proveedor end
and pl.id_ProductionLotState  in (11,1)  --no anulado ni pendiente de recepción
order by PL_number


/*
	EXEC spPar_MateriaPrimaPendiente @id=N'',@fi=N'2022/02/01',@ff=N'2022/03/31',@proveedor=0
*/
GO