
GO
/****** Object:  StoredProcedure [dbo].[SP_ResumenTallaCompras]    Script Date: 18/01/2023 20:03:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SP_ResumenTallaCompras   '4734','','',''
Create procedure [dbo].[SP_MargenPorTallas]

(
--@id varchar(50)='',
@proveedor int = 0,
@fi varchar(10) = '',
@ff varchar(10) = '',
@seqliq int ='',
@estado int = 0
)
as 
Set Nocount on
if @fi = '' set @fi = null
if @ff = '' set @ff = null

declare @fiDt date
declare @ffDt date



set @fiDt = convert(date,isnull(@fi,'1900-01-01'))
set @ffDt = convert(date,isnull(@ff,'1900-01-01'))



SELECT 
cia.businessName as Compania
,'GUAYAQUIL' as Ciudad
--,'RESUMEN DE MARGEN POR TALLAS' as Titulo
,pl.receptionDate as FechaDesde
,pl.expirationDate as FechaHasta
,PL.sequentialLiquidation AS nliqu
,convert(varchar,pl.sequentialLiquidation,25) as Liquidaciones
,pl.internalNumber as NloteInterno
,doc.emissionDate as FechaEmision
,'Todos' As Comisionista
,pli.startDate as FechaListadoPrecioReferenciales
,prov.id ids,prov.fullname_businessName as Proveedores
,cast(pls.id as decimal(17,2))as idestado
,pls.name as Estado
--detalle--------------------------------
,item.name as Producto
,itty.id as AgruparTipoProducto
,upper(itty.name) as TipoProducto
,itc.id as AgruparCategoriaProducto
,upper(itc.name) as CategoriaProducto
,size.id as AgruparTallas
,size.name as Talla
, plp.totalPounds as Rendimiento
,plp.totalPounds totallibras
,@fi as Fi
,@ff as Ff
,plp.totalToPay as Valor
,plp.price as PrecioPromedio
,plp.totalPriceDis as ValorRef
,plcs.price as PrecioRef
,plp.price-plcs.price as Margen
--,case when pls.id = 9 then 'PRELIMINAR' ELSE ''  as FILTROLIQ 
,case when CONVERT(VARCHAR,@proveedor) = '0' and CONVERT(VARCHAR,@seqliq)  = 0 then 'TODOS'  
else CONVERT(VARCHAR,PROV.fullname_businessName) end  as FILTROPROV 
,pls.name 
,@estado
,cia.logo as Logo
,cia.logo2 as Logo2
from ProductionLotPayment  plp
inner join productionlot pl on pl.id = plp.id_productionLot

--inner join ProcessType prt on prt.id = pl.id_processtype
inner join document doc on doc.id = plp.id_productionLot
inner join Company cia on cia.id = pl.id_company
left join pricelist pli on pli.id = pl.id_priceList
inner join item item on item.id = plp.id_item
inner join itemtype itty on itty.id = item.id_itemType
inner join ProductionLotState pls on pls.id = pl.id_ProductionLotState
left join ItemTypeCategory itc on itc.id = item.id_itemTypeCategory
inner join person prov on prov.id = pl.id_provider
left join itemgeneral itge on itge.id_item = plp.id_item
left join ItemSize size on size.id = itge.id_size
left join ItemTypeCategoryClassRelation itcr on itcr.id_ItemTypeCategory = item.id_itemTypeCategory
left join class class on class.id = itcr.id_Class
left join ItemSizeClass itsc on itsc.id_ItemSize = size.id and itsc.id_Class = class.id 
left join PriceListItemSizeDetail plcs on plcs.id_priceList = pl.id_priceList and plcs.id_class = class.id and plcs.Id_Itemsize = size.id 


--inner join ItemSizeProcessPLOrder itso on
--itso.id_ItemSize = size.id and itso.id_class = plcs.id_class 
--and itpt.idProcessType = itso.id_ProcessType
--and plcs.Id_Itemsize = size.id





where
--pl.id = @ID AND 
--pl.internalNumber like case when @id = '' then pl.internalNumber
--else '%' + @id + '%' end and

convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.receptionDate) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date, pl.receptionDate) else @ffDt end
and pl.id_provider = case when @proveedor = 0 then pl.id_provider  else @proveedor end

and convert(varchar,pl.sequentialLiquidation) = case when isnull(@seqliq,'') = '' then convert(varchar,pl.sequentialLiquidation) else @seqliq end
and pl.id_ProductionLotState = case when isnull(@estado,'') = 0 then pl.id_ProductionLotState else  @estado end 

and pls.name in('PENDIENTE DE APROBACIÓN','APROBADO')





--select *from person where fullname_businessName like '%NOSTRAMAR S.A.%'


--select * from ProductionLot where id = 858463

--select *from presentation where id =824614

