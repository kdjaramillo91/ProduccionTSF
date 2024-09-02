/****** Object:  StoredProcedure [dbo].[spPar_FleteTerrestre]    Script Date: 05/04/2023 05:30:38 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER procedure [dbo].[spPar_FleteTerrestre]
@id int
as
set nocount on 

select  
lfd.[price] as flete,
lfd.[pricesavance] as anticipo,
lfd.[priceadjustment] as ajuste,
lfd.[pricedays] as valordias,
lfd.[priceextension] as extension,
lfd.[pricetotal] as total,
vh.[carRegistration],
ps.[fullname_businessName] as proveedor,
D.[emissionDate] as fechaemision,
convert(varchar(50),drg.[sequential]) as guiaremision,
drg.[emissionDate] as [FechaGuiaRemision],
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
c.[description] as nombrecia,
lfd.[PriceCancelled] as [FleteCancelado],
lf.[PriceCancelledTotal] as [FleteCanceladoTotal],
lfd.[descriptionRG] as [DescripcionRG],
rgt.[driverName] as [Chofer],
d.[description] as [DescripcionGeneral],
c.logo as logo,
c.logo2 as logo2,
lf.[invoicenumber] as [NumeroFactura]
from LiquidationFreight lf
inner join LiquidationFreightDetail lfd on lf.id = lfd.id_LiquidationFreight
inner join RemissionGuide rg on rg.id = lfd.id_remisionGuide
inner join RemissionGuideTransportation rgt on rgt.id_remionGuide = rg.id
inner join Vehicle vh on vh.id = rgt.id_vehicle
inner join Person ps on ps.id = rg.id_providerRemisionGuide
inner join Document d on lf.id = d.id
inner join [dbo].[Document] drg on drg.[id] = rg.[id]
inner join Person ps1 on vh.id_personOwner = ps1.id
inner join Person ps2 on ps2.id = lf.id_providertransport
left outer join EmissionPoint ep on ep.id = d.id_emissionPoint
left outer join Company c on c.id = ep.id_company
INNER JOIN .DocumentType dt on dt.id = d.id_documentType
inner join DocumentState ds on ds.id = d.id_documentState
where lf.id = @id

/*
	EXEC [dbo].[par_Liquidacion_Flete_terrestre] 140295
*/

GO