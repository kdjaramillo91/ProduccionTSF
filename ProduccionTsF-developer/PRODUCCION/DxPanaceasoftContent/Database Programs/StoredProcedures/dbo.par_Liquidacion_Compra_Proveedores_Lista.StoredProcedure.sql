If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_Liquidacion_Compra_Proveedores_Lista'
	)
Begin
	Drop Procedure dbo.par_Liquidacion_Compra_Proveedores_Lista
End
Go
Create Procedure dbo.par_Liquidacion_Compra_Proveedores_Lista
(
@id_provider int,
@f_ini varchar(10),
@f_fin varchar(10)
)
As 
set @f_ini = convert(date,isnull(@f_ini,'1900-01-01'))
set @f_fin = convert(date,isnull(@f_fin,'2200-01-01'))
select 
[PL].[id] as [PL_id],
[PL].[number] as [PL_number],
[PL].[internalNumber] as [PL_internalNumber],
[PL].[id_provider] as [PL_id_provider],
[PL].[id_priceList] as [PL_id_priceList],
[PL].[totalQuantityOrdered] as [PL_totalQuantityOrdered],
[PL].[totalQuantityRemitted] as [PL_totalQuantityRemitted],
[PL].[totalQuantityRecived] as [PL_totalQuantityRecived],
[PL].[totalQuantityLiquidation] as [PL_totalQuantityLiquidation],
[PL].[totalQuantityTrash] as [PL_totalQuantityTrash],
[PL].[totalQuantityLiquidationAdjust] as [PL_totalQuantityLiquidationAdjust],
[PL].[liquidationDate] as [PL_liquidationDate],
[PL].[closeDate] as [PL_closeDate],
[PL].[liquidationPaymentDate] as [PL_liquidationPaymentDate],
[PL].[id_company] as [PL_id_company],
[PL].[id_productionUnitProviderPool] as [PL_id_productionUnitProviderPool],
[PL].[id_productionUnitProvider] as [PL_id_productionUnitProvider],
[PL].[wholeSubtotal] as [PL_wholeSubtotal],
[PL].[subtotalTail] as [PLsubtotalTail],
[PL].[wholeGarbagePounds] as [PL_wholeGarbagePounds],
[PL].[poundsGarbageTail] as [PL_poundsGarbageTail],
[PL].[wholeLeftover] as [PL_wholeLeftover],
[PL].[totalAdjustmentPounds] as [PL_totalAdjustmentPounds],
[PL].[totalAdjustmentWholePounds] as [PL_totalAdjustmentWholePounds],
[PL].[totalAdjustmentTailPounds] as [PL_totalAdjustmentTailPounds],
[PL].[wholeSubtotalAdjust] as [PL_wholeSubtotalAdjust],
[PL].[subtotalTailAdjust] as [PL_subtotalTailAdjust],
[PL].[wholeSubtotalAdjustProcess] as [PL_wholeSubtotalAdjustProcess],
[PL].[totalToPay] as [PL_totalToPay],
[PL].[wholeTotalToPay] as [PL_wholeTotalToPay],
[PL].[tailTotalToPay] as [PL_tailTotalToPay],
[PL].[INPnumberPL] as [PL_INPnumberPL],
[PL].[ministerialAgreementPL] as [PL_ministerialAgreementPL],
[PL].[tramitNumberPL] as [PL_tramitNumberPL],
[QC].QuantityPoundsReceived as [ _QuantityPoundsReceived],
[ProductionUnitProviderPool].name as [NamePool],
[Person].fullname_businessName as [Nameproveedor],
dbo.FUN_GetRemissionGuide([PL].id) as RemissionGuide,
dbo.FUN_GetPurchaseOrder([PL].id) as PurchaseOrder,
[Person].identification_number as identity_prov,
Company.businessName as name_cia,
Company.ruc as ruc_cia,
company.address as adreess_cia,
Company.phoneNumber as telephone_cia,
Company.logo2,
PriceL.name as PriceList,
AP.valueAdvanceTotal	,
grammage = (select SUM(grammagereference)/COUNT(grammagereference) from QualityControl qcc where qcc.id_lot = PL.id),
Document.emissionDate as femision,
case QC.id_processType when 1 then 'Entero' else 'Cola' end as proceso
from  [dbo].[ProductionLot] [PL]
left join AdvanceProvider AP
	on AP.[id_lot] = [PL].[id]
left join QualityControl [QC]
	on QC.[id_lot] = [PL].[id]
left join Pricelist PriceL
	on PriceL.[id] = [PL].[id_priceList]
inner join Person Person
	on Person.[id] = [PL].id_provider
inner join ProductionUnitProviderPool [ProductionUnitProviderPool]
	on ProductionUnitProviderPool.id = [PL].id_productionUnitProviderPool
inner join Document
	on Document.id = [PL].id
INNER join [dbo].[EmissionPoint] EmissionPoint
	on EmissionPoint.[id] = Document.[id_emissionPoint]
INNER join [dbo].[Company] Company
	on Company.[id] = EmissionPoint.[id_company]
where
 document.[emissionDate]>= isnull(@f_ini,document.[emissionDate]) and  document.[emissionDate] <=isnull(@f_fin,document.[emissionDate])
and  pl.id_provider = isnull(@id_provider,pl.id_provider)

--execute par_Liquidacion_Compra_Proveedores_Lista null,null,null --'2018-03-01','2018-05-31'
--execute par_Liquidacion_Compra_Proveedores_Lista null,'',''



Go
