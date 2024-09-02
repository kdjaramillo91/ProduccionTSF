
GO
/****** Object:  StoredProcedure [dbo].[par_Liquidacion_Flete_Fluvial]    Script Date: 01/02/2023 09:31:12 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




Create procedure [dbo].[spPar_FleteFluvial]
@id int
as

select  
	lfd.[price] as flete,
	lfd.[pricesavance] as anticipo,
	lfd.[priceadjustment] as ajuste,
	lfd.[pricedays] as valordias,
	lfd.[priceextension] as extension,
	lfd.[pricetotal] as total,
	vh.[carRegistration],
	ps.[fullname_businessName] as proveedor,
	drg.[emissionDate] as fechaemision,
	convert(varchar(20),drg.[sequential]) as guiaremision,
	convert(varchar(250),ps1.[fullname_businessName]) as duenotransporte,
	ps2.[fullname_businessName] as ciafactura,
	c.[businessName] as nombrecia,
	dt.[name] as opcion,
	c.[ruc] as ruc,
	d.[number] as numdoc,
	c.[address] as direccion,
	ds.[name] as estadodocumento,
	c.[phoneNumber] as telefonocia,
	c.[email] as correocia,
	c.[logo2] as logo,
	c.[logo] as logo2,
	c.[description] as nombrecia,
	lfd.[descriptionRGR] as DescripcionGuia,
	isnull(lfd.PriceCancelled,0) as FleteCanceladoFluvial,
	lf.[invoicenumber] as [NumeroFactura],
	d.[description] as DescripcionLiquidacion
from [dbo].[LiquidationFreightRiver] lf
inner join [dbo].[Document] d on lf.[id] = d.[id]
inner join [dbo].[LiquidationFreightRiverDetail] lfd on lf.id = lfd.id_LiquidationFreightRiver
inner join RemissionGuideRiver rg on rg.id = lfd.id_remisionGuideRiver
inner join RemissionGuideRiverTransportation rgt on rgt.id_remissionGuideRiver = rg.id
inner join [dbo].[Document] drg on rg.id = drg.id
inner join Vehicle vh on vh.id = rgt.id_vehicle
inner join Person ps on ps.id = rg.id_providerRemisionGuideRiver
inner join Person ps1 on vh.id_personOwner = ps1.id
inner join Person ps2 on ps2.id = lf.id_providertransport
inner join EmissionPoint ep on ep.id = d.id_emissionPoint
inner join Company c on c.id = ep.id_company
inner join DocumentType dt on dt.id = d.id_documentType
inner join DocumentState ds on ds.id = d.id_documentState
where lf.id = @id
 --execute  par_Liquidacion_Flete_Fluvial 145333
