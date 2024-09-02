/****** Object:  StoredProcedure [dbo].[par_ProductionLiquidationEntryCR]    Script Date: 29/12/2022 10:42:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROC [dbo].[spPar_Recepcion]
(
	@id varchar(50) = '',
	@fi varchar(10) = '',
	@ff varchar(10) = '',
	@proveedor int = 0
)
As 

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
	   pl.receptionDate  as  PL_recepctionDate ,
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
	   converT(varchar(250),prs.fullname_businessName)  as  Nameproveedor ,
	   converT(varchar(250),pup.[name]) as ProductionUnitProviderName,
	   dbo.FUN_GetRemissionGuide(pl.id) as RemissionGuide,
	   dbo.FUN_GetPurchaseOrder(pl.id) as PurchaseOrder,
		prs.identification_number as identity_prov,
		Company.businessName as name_cia,
		Company.ruc as ruc_cia,
		company.address as adreess_cia,
		Company.phoneNumber as telephone_cia,
		CONVERT(varchar, @fiDt, 103) as Fi,
		CONVERT(VARCHAR, @ffDt, 103)as Ff,
		PriceL.name as PriceList,
		grammage = (select SUM(grammagereference)/COUNT(grammagereference) from QualityControl qcc , ProductionLotDetailQualityControl pldqc , ProductionLotDetail pld
																					  where pldqc.id_productionLotDetail = pld.id and
																					  qcc.id_lot= pld.id_productionLot 
																					  and qcc.id_lot = pl.id),
		rendimiento = (select SUM(wholePerformance)/COUNT(wholePerformance) from QualityControl qcc , ProductionLotDetailQualityControl pldqc , ProductionLotDetail pld
																					  where pldqc.id_productionLotDetail = pld.id and
																					  qcc.id_lot= pld.id_productionLot 
																					  and qcc.id_lot = pl.id),
		lbsrecibidas = (select SUM(QuantityPoundsReceived) from QualityControl qcc where qcc.id_lot = pl.id),
		proceso = Pr.[name],

		lbsrecibent = case PT.name when 'ENTERO' then  (select SUM(QuantityPoundsReceived) from QualityControl qcc where qcc.id_lot = pl.id) else  0 end,
		lbsrecibcola = case PT.name when 'COLA' then  (select SUM(QuantityPoundsReceived) from QualityControl qcc where qcc.id_lot = pl.id) else  0 end,
		
		gramagentero = case PT.name when 'ENTERO' then (select SUM(grammagereference)/COUNT(grammagereference) 
						from ProductionLotDetailQualityControl a 
						join ProductionLotDetail b on a.id_productionLotDetail = b.id
						join QualityControl c on a.id_qualityControl = c.id 
						where b.id_productionLot = pl.id) else 0 end,
		gramagcola = case PT.name when 'COLA' then (select SUM(grammagereference)/COUNT(grammagereference) 
						from ProductionLotDetailQualityControl a 
						join ProductionLotDetail b on a.id_productionLotDetail = b.id
						join QualityControl c on a.id_qualityControl = c.id 
						where b.id_productionLot = pl.id) else 0 end,

		rentero = case PT.name when 'ENTERO' then (select SUM(wholePerformance)/COUNT(wholePerformance) 
						from ProductionLotDetailQualityControl a 
						join ProductionLotDetail b on a.id_productionLotDetail = b.id
						join QualityControl c on a.id_qualityControl = c.id 
						where b.id_productionLot = pl.id) else 0 end,
		rentcola = case PT.name when 'COLA' then (select SUM(wholePerformance)/COUNT(wholePerformance) 
						from ProductionLotDetailQualityControl a 
						join ProductionLotDetail b on a.id_productionLotDetail = b.id
						join QualityControl c on a.id_qualityControl = c.id 
						where b.id_productionLot = pl.id) else 0 end,
		Inp = INPnumber, AcuerdoTramite = Case when ministerialAgreement <> '' Then isnull(ministerialAgreement,'') Else Isnull(tramitNumber,'') End,
		CONVERT(VARCHAR(10),p.processPlant) as ProcesoPlanta
		,CASE WHEN CONVERT(VARCHAR,year(@fiDt)) = '1900' then 'TODOS' ELSE CONVERT(VARCHAR,@fiDt,103) END as Fi,
CASE WHEN CONVERT(VARCHAR,year(@ffDt)) = '1900' then 'TODOS' ELSE CONVERT(VARCHAR,@ffDt,103) END as Ff



  from   [dbo].ProductionLot  pl
	inner join Person p
	on p.id = pl.id_personProcessPlant
   left join Pricelist  PriceL 
   on ( PriceL .id  = pl.id_priceList )
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
  Inner Join [dbo].Lot lo
       On lo.id = pl.id	  
  Left Outer Join (Select distinct id_lot,id_processType from QualityControl) qu
       On qu.id_lot = lo.id	
  Left Outer join ProcessType Pr
	   on Pr.id = qu.id_processtype      
where pl.internalNumber like case when @id = '' then pl.internalNumber
									else '%' + @id + '%' end
and convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.[receptionDate]) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date, pl.[receptionDate]) else @ffDt end
and pl.[id_provider] = case when @proveedor = 0 then pl.[id_provider] else @proveedor end
and pl.id_ProductionLotState not in (11,1) 
order by Nameproveedor, ProductionUnitProviderName

/*
	EXEC spPar_Recepcion '','2022/01/01','2022/01/31',''
*/

GO