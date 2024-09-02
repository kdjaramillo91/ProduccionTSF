
/****** Object:  StoredProcedure [dbo].[spPar_ComprobanteUnicoPagoEquivalente]431883    Script Date: 6/6/2023 09:23:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create or alter procedure [dbo].[spPar_ComprobanteUnicoPagoEquivalente]
@id int
as 
Set Nocount on

select 
cia.businessName as Compania
,pl.sequentialLiquidation as Nliquidacion
,'REPORTE DE LIQUIDACION DE INVENTARIO LOTE' as Titulo
,prov.fullname_businessName as Proveedor
,pl.receptionDate as FechaRecepcion
,prov.identification_number as IdentificacionProveedor
,pupo.name as Piscina
,cplt.name as Aguaje
,fz.name as Sector
,SUBSTRING(pl.internalNumber,7,100)+'-'+SUBSTRING(replace(pl.internalNumber,'-',' '),1,6) as Lote
,pl.INPnumberPL as INP
,case when pl.id_certification IS NULL then 'X' else ''
 END  as NoASC,
 case when pl.id_certification IS not NULL then 'X' else ''
 END  as SIASC
,case when ity.name = 'entero' Then pl.wholesubtotal+pl.wholeleftover+pl.wholeGarbagePounds
else pl.tailLeftOver End as LibrasRecibidas
,LibrasRemitidas =(select SUM(quantityRecived) from ProductionLotDetail where id_productionLot= @id)  
,PL.wholeLeftover + pl.totalQuantityTrash as Sobrante
,PL.wholeGarbagePounds AS BasuraEntero
,PL.poundsGarbageTail AS BasuraDescabezado
,pl.totalQuantityLiquidation as LbsProcesadas
,Case when (PL.wholeSubtotal+PL.wholeLeftover) > 0 Then round((PL.wholeSubtotal/(PL.wholeSubtotal+PL.wholeLeftover))*100,2) else 0 End as RendimientoEntero
,case when pl.wholeSubtotal = 0 then
case when (pl.tailLeftOver-pl.poundsGarbageTail) = 0 then 0 else
round((pl.subtotalTail/(pl.tailLeftOver-pl.poundsGarbageTail))*100,2) End
else
case when (pl.wholeLeftover-pl.poundsGarbageTail) = 0 then 0 else
round((pl.subtotalTail/(pl.wholeLeftover-pl.poundsGarbageTail))*100,2) End End as RendimientoCOla
--//---DETALLE--//----------------------------------------------------------
,item.id_itemType as AGR1EnteroCola
,case when ity.name = 'entero' then 'HOSO BLOCK' 
else  'HLSO BLOCK'
end as TipoProducto
,item.id_itemTypeCategory as AGR2Clase
,itc.name as Categoria
,plp.id_item as AGR3Producto

,plp.id_item as normal
,plpd.id_item as distribuido
,itemE.masterCode as CodigoProducto
,convert (varchar(250),itemE.description2) 
as Producto


,itg.id_size as AGR4Talla
,its.name as Talla
, Round(pldet.quantityPoundsITW,2)
as CantidadLibrasEntero
,case when item.id_itemType = 34 then
Round(totalProcessMetricUnitEq * 2.2046,2) end--else
as CantidadLibrasEntero2
,case when item.id_itemType = 35 then
plp.totalPounds else 0 end
 as CantidadLibrasCola
,plp.totalPounds 
as CantidadLibras
,case when item.id_itemType = 34 then
case when plp.id_metricUnit = 1  then PLP.price
when plp.id_metricUnit = 1 then PLP.price/2.2046
else PLP.price end 
else 0 end
as PrecioKilos
,case when item.id_itemType = 34 then
 plp.price / 2.2046 else plp.price end
  as  Preciolibras
 ,plp.id_metricUnit
,plp.distributedd
,PL.wholeSubtotal +PL.subtotalTail AS TotalCabCol
,plp.totalToPayEq as Totaltopay
--select  distributedd,*  
from ProductionLotpayment  plp
inner join productionlot pl on pl.id = plp.id_productionLot
inner join company cia on cia.id = pl.id_company
inner join ProductionUnitProviderPool pupo on pupo.id = pl.id_productionUnitProviderPool
left join item item on item.id = plp.id_item
left join ItemEquivalence ite on ite.id_item = item.id
left join item itemE on itemE.id = ite.id_itemEquivalence
left join itemtype ity on ity.id = item.id_itemtype
left join ItemTypeCategory itc on itc.id = item.id_itemTypeCategory
left join ItemGeneral itg
on itg.id_item = Item.id
left join ItemSize its
on its.id = itg.id_size
left outer join [dbo].[ProductionUnitProvider] PUP on PL.id_productionUnitProvider = PUP.id  
LEFT join Certification certi2 on certi2.id = pl.id_certification
left join FishingZone fz on fz.id = pup.id_FishingZone
left join Pricelist PriceL on [PriceL].[id] = [PL].[id_priceList]
left join CalendarPriceList cplis on  cplis.id = pricel.id_calendarPriceList
left join CalendarPriceListType cplt on cplt.id = cplis.id_calendarPriceListType
left join person prov on prov.id = pl.id_provider 
left join ProductionLotPaymentDistributed plpd on plpd.id_productionLotPayment = plp.id and plpd.isActive = 1
left join item itemd on itemd.id = plpd.id_item
left join itemtype ityd on ityd.id = itemd.id_itemtype
left join ItemTypeCategory itcd on itcd.id = itemd.id_itemTypeCategory
left join ItemGeneral itgd
on itgd.id_item = Itemd.id
left join ItemSize itsd
on itsd.id = itgd.id_size
left join ProductionLotLiquidationTotal  pldet on pldet.id_productionLot = plp.id_productionLot and pldet.id_ItemLiquidation = plp.id_item



where plp.id_productionLot = @id


