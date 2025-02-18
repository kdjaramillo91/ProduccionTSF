/****** Object:  StoredProcedure [dbo].[spPar_ResumenComprasPorPeriodoGlaseo]    Script Date: 09/06/2023 01:41:02 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Query / SP Original: SP_ResumenComprasPeriodosG
CREATE OR ALTER     PROC [dbo].[spPar_ResumenComprasPorPeriodoGlaseo]
(
--@id varchar(50)='',
@proveedor int = 0,
@fi varchar(10) = '',
@ff varchar(10) = ''

)
as 
Set Nocount on
if @fi = '' set @fi = null
if @ff = '' set @ff = null

declare @fiDt date
declare @ffDt date


set @fiDt = convert(date,isnull(@fi,'1900-01-01'))
set @ffDt = convert(date,isnull(@ff,'1900-01-01'))



select 
'RESUMEN DE COMPRA POR PERIODO - GLASEO' as Titulo
,cia.businessName as Compania
,'GUAYAQUIL' AS Ciudad
,pl.receptionDate as FechaDesde
,pl.expirationDate as FechaHasta
,itg.id_trademark as Ag1Proveedor
,prov.fullname_businessName as Proveedor
,item.description as ProductoGlaseo
--,plp.quantity as Rendimiento
,plp.totalToPay Valor
,plp.price as PrecioPromedio
-----------------------
,item.description as Producto
,mu.id mu
 --,case when mu.id = 1 and itty.name ='entero' then plp.quantity else plp.totalPounds end as Rendimiento
  ,case when mu.id = 1
 --and itty.name ='entero' 
 then plp.quantity * 2.2046 else plp.totalPounds end as Rendimiento
,plp.quantity as Rendimiento2
,plp.totalToPay Valor
,plp.price as PrecioPromedio
,itty.name as TipoProducto
,itm.name as Marca
,case when @proveedor = 0 then 'Todos los Proveedores' else prov.fullname_businessName end as FiltroProv
,itg.id_size as Agr2Size
,size.name as Talla,
cia.businessName as Business,
cia.ruc as ruc,
cia.address as Direccion,
CASE WHEN YEAR(ISNULL(@fiDt, '')) = 1900 THEN '' ELSE CONVERT(VARCHAR(10), @fiDt, 103) END AS Fi,
CASE WHEN YEAR(ISNULL(@ffDt, '')) = 1900 THEN '' ELSE CONVERT(VARCHAR(10), @ffDt, 103) END AS Ff

FROM productionlotpayment plp
inner join productionlot pl on pl.id = plp.id_productionLot
inner join person prov on prov.id = pl.id_provider
inner join company cia on cia.id = pl.id_company
inner join ItemEquivalence ie on ie.id_item = plp.id_item
left join item item on item.id = ie.id_itemEquivalence
inner join ItemType itty on itty.id = item.id_itemType
left join ItemGeneral itg on itg.id_item = item.id
left join ItemTrademark itm on itm.id = itg.id_trademark
inner join Presentation pres on pres.id = item.id_presentation
inner join metricunit mu on mu.id = pres.id_metricUnit
left join ItemSize size on size.id = itg.id_size

where 
--plp.id_productionLot = @id and 
pl.id_provider= case when isnull(@proveedor,'') = '' then pl.id_provider  else @proveedor end and
convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.receptionDate) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date, pl.receptionDate) else @ffDt end
and pl.id_ProductionLotState in(9,10)
order by itty.name Desc,itg.id_size asc

/*
	EXEC spPar_ResumenComprasPorPeriodoGlaseo @proveedor=0,@fi=N'2022/02/01',@ff=N'2022/02/28'
	exec spPar_ResumenComprasPorPeriodoGlaseo @proveedor=0,@fi=N'2022/11/30',@ff=N'2022/11/30'
*/
GO