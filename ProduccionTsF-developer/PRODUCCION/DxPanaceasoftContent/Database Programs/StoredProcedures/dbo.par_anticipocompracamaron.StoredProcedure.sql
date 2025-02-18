If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_anticipocompracamaron'
	)
Begin
	Drop Procedure dbo.par_anticipocompracamaron
End
Go
Create Procedure dbo.par_anticipocompracamaron
(
	@id int
)
As
set nocount on
select 
ap.[id] as IdAnticipo,
 ps.[fullname_businessName] as proveedor,
 ps.[identification_number] as ruc,
 pgd.[cellPhoneNumber] as telefono,
 pl.internalNumber as lote,
 d.[number] as n,
 d.[emissionDate] as fecha,
 ps1.[fullname_businessName] as comprador,
 pup.[address] as sitio,
 ap.[QuantityPoundReceived] as libras_recibidas,
 pl.[totalQuantityRecived] as libras_despachadas, --[totalQuantityRemitted]
  gramage_promedio = ap.grammagelot,
  --(select SUM(QC.grammageReference)/COUNT(QC.grammageReference) from QualityControl QC where QC.id_lot = PL.id) ,
 pl.receptionDate as fecha_recepcion,
 pl.[closeDate] as fecha_procesamiento,
 pup.[INPnumber] as inp_camar,
 Pup.[ministerialAgreement] as acu_camar,
 pup.[tramitNumber] as tra_camar
 , inp_ampar = (select top 1 pup1.INPnumber 
				from [dbo].[ProductionLotDetailPurchaseDetail] pldpd 
				inner join [dbo].[ProductionLotDetail] pld1 on pld1.[id] = pldpd.[id_productionLotDetail]
				inner join [dbo].[PurchaseOrderDetail] pod1 on pldpd.[id_purchaseOrderDetail] = pod1.[id]
				inner join [dbo].[PurchaseOrder] po1 on pod1.[id_purchaseOrder] = po1.[id]
				inner join [dbo].[ProductionUnitProvider] pup1 on pup1.[id] = po1.[id_productionUnitProviderProtective]
				where pld1.[id_productionLot] = ap.[id_Lot])
 , acu_ampar = (select top 1 pup1.ministerialAgreement 
				from [dbo].[ProductionLotDetailPurchaseDetail] pldpd 
				inner join [dbo].[ProductionLotDetail] pld1 on pld1.[id] = pldpd.[id_productionLotDetail]
				inner join [dbo].[PurchaseOrderDetail] pod1 on pldpd.[id_purchaseOrderDetail] = pod1.[id]
				inner join [dbo].[PurchaseOrder] po1 on pod1.[id_purchaseOrder] = po1.[id]
				inner join [dbo].[ProductionUnitProvider] pup1 on pup1.[id] = po1.[id_productionUnitProviderProtective]
				where pld1.[id_productionLot] = ap.[id_Lot])
 , tra_ampar = (select top 1 pup1.tramitNumber 
				from [dbo].[ProductionLotDetailPurchaseDetail] pldpd 
				inner join [dbo].[ProductionLotDetail] pld1 on pld1.[id] = pldpd.[id_productionLotDetail]
				inner join [dbo].[PurchaseOrderDetail] pod1 on pldpd.[id_purchaseOrderDetail] = pod1.[id]
				inner join [dbo].[PurchaseOrder] po1 on pod1.[id_purchaseOrder] = po1.[id]
				inner join [dbo].[ProductionUnitProvider] pup1 on pup1.[id] = po1.[id_productionUnitProviderProtective]
				where pld1.[id_productionLot] = ap.[id_Lot])
,comp.logo2 as logo
 --case pup1.[INPnumber] when pup.[INPnumber] then ' ' else pup1.[INPnumber] end as inp_ampar,
 --case pup1.[ministerialAgreement] when pup.[ministerialAgreement] then ' ' else pup1.[ministerialAgreement] end as acu_ampar,
 --case pup1.[tramitNumber] when pup.[tramitNumber] then ' ' else pup1.[tramitNumber] end as tra_ampar,
 ,pls.[name] + ' Fecha Inicio: '+ format(DAY(pls.[startDate]),'0#')+'/'+format(month(pls.[startDate]),'0#')+'/'+format(year(pls.[startDate]),'####')+ ' Fecha Fin: ' + format(DAY(pls.[endDate]),'0#')+'/'+format(month(pls.[endDate]),'0#')+'/'+format(year(pls.[endDate]),'####') as lista,
ap.[AdvanceValuePercentageUsed] as porcentajeanticipo,
ap.[valueAdvanceTotalRounded] as valor_anticipo,
pty.[name] as proceso,
convert(varchar(100),c.[description]) as clase,
its.[name] as talla,
apd.[poundsDetail] AS libras,
apd.[valuePrice] as precio,
apd.valueTotal as total,
d.[description] as Observacion
,us.[username] as NombreUsuario
, pupp.[name] as Piscina
, ap.[valueAverage] as PrecioPromedio
, ap.valueAdvance as ValorAproximado
, PTL.[name] AS ProcesoLote
from [dbo].[AdvanceProvider] ap
inner join [Document] d on d.[id] = ap.[id]
inner join [dbo].[EmissionPoint] empo on d.[id_emissionPoint] = empo.[id]
inner join [dbo].[Company] comp on empo.[id_company] = comp.[id]
inner join [dbo].[ProductionLot] pl on pl.[id] = ap.[id_Lot]
INNER JOIN [dbo].[ProcessType] PTL ON pl.[id_processtype] = PTL.[id]
inner join [dbo].[ProductionUnitProviderPool] pupp on pl.[id_productionUnitProviderPool] = pupp.[id]
inner join [Lot] l on l.[id] = pl.[id]
inner join [dbo].[AdvanceProviderDetail] apd on apd.[id_AdvanceProvider] = ap.[id]
inner join [dbo].[Person] ps on ps.[id] = ap.[id_provider]
inner join [dbo].[Person] ps1 on ps1.[id] = pl.[id_buyer]
inner join [dbo].[ProviderGeneralData] pgd on pgd.[id_provider] = ps.[id]
inner join [dbo].[ProductionUnitProvider] pup on pup.[id] = ap.[id_productionUnitProvider]
inner join [dbo].[Person] ps2 on ps2.[id] = ap.[id_protectiveProvider]
inner join [dbo].[PriceList] pls on pls.[id] = ap.[id_priceList]
inner join [dbo].[Class] c on c.[id] = apd.[id_class]
inner join [dbo].[ItemSize] its on its.[id] = apd.[id_itemsize]
inner join [dbo].[ItemSizeProcessPLOrder] ispl on its.[id] = ispl.[id_ItemSize]
inner join [dbo].[ProcessType] pty on pty.[id] = ispl.[id_ProcessType]
inner join [dbo].[User] us on d.[id_userCreate] = us.[id]

where @id = d.id
order by ispl.id_ProcessType, c.id
-- execute par_anticipocompracamaron 138803

--select * from AdvanceProvider


-- select * from purchaseorderdetail where id =28

--select * from productionlotdetailpurchasedetail where id_productionlotdetail = 2


--select * from purchaseorder where id = 126563
--select * from productionlotdetail where id_productionlot = 126711

Go
