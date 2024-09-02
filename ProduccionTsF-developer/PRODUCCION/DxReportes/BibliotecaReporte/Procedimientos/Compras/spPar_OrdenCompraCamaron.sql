GO
/****** Object:  StoredProcedure [dbo].[par_OrdendecompracamaronCR]    Script Date: 04/01/2023 9:08:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[spPar_OrdenCompraCamaron]--986508
@num_purchaseorder decimal
as
declare @valortotal decimal(18,6)
select @valortotal = (select SUM(total) from PurchaseOrderDetailByGrammage where id_purchaseOrder =(convert(decimal,@num_purchaseorder)))

select 
it.[masterCode] AS codigo,
it.[name] as descripcion,
gg.[code] as codigogramage,
mu.[code] as unidad,
podg.[quantityOrdered] as cantidad,
podg.[price] as precio,
podg.[total] as valor,
dc.[emissionDate] as fecha,
dc.[number] as N,
ps.[fullname_businessName] as proveedor,
pup.[name]  as unidad_de_produccion,
post.[name] as via_de_embarque,
pt.[description] as plazo_pago,
pm.[description] as forma_pago,
ps1.[fullname_businessName] as comprador,
dc.[description] as observacion,
@valortotal as suma_total,
Company.logo as logo,
company.logo2 as logo2,
dbo.FUN_CantidadConLetraCastellano(convert(Int, @valortotal))+ ' ' + SUBSTRING(RTRIM(LTRIM(PARSENAME( @valortotal,1))),1,2) + '/100 DOLLARS' as cantidadletras,
--dbo.FUN_CantidadConLetraCastellano( @valortotal)+ ' ' + SUBSTRING(RTRIM(LTRIM(PARSENAME(CONVERT(Varchar, @valortotal),1))),1,2) + '/100 DOLLARS' as cantidadletras,
Convert(varchar(250),plant.processPlant) as 'ProcessPlant'
 from PurchaseOrderDetailByGrammage podg
 inner join [PurchaseOrder] po
 on po.id = podg.id_purchaseOrder
 inner join [Document] dc
 on dc.id = po.id
 left JOIN Grammage gg
 on GG.id  = podg.id_Grammage
 inner join Item it
 on it.id  = podg.id_item
 inner join [ItemInventory] ii
 on ii.id_item = it.id
 inner join [MetricUnit] mu
 on mu.id = ii.id_metricUnitInventory
 inner join [Provider] pv
 on pv.id = po.id_provider
 inner join [Person] ps
 on ps.id = pv.id
 inner join ProductionUnitProvider pup
 on pup.id = po.id_productionUnitProvider
 inner join [PaymentTerm] pt
 on pt.id = po.id_paymentTerm
 inner join PurchaseOrderShippingType post
 on post.id = po.id_shippingType
  inner join [PaymentMethod] pm
  on pm.id = po.id_paymentMethod
  inner join [Person] ps1
  on ps1.id = po.id_buyer
  inner join Document
       on Document.id = po.id
  INNER join [dbo].EmissionPoint   EmissionPoint 
       on  EmissionPoint .id  =  Document .id_emissionPoint 
  Inner join [dbo].Company   Company 
       on  Company .id  =  EmissionPoint .id_company 
  INNER join [Person] plant
	on plant.id = po.id_personProcessPlant
  where po.id = @num_purchaseorder

