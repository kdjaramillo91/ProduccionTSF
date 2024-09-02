GO
/****** Object:  StoredProcedure [dbo].[par_Movimiento_InventarioIngreso]    Script Date: 01/03/2023 10:34:06 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[spPar_Movimiento_InventarioIngreso]
	@id int
as 
set nocount on 


declare @idRemissionGuide int
declare @idLiquidationMatSup int
declare @sequentialRemissionGuide varchar(20)
declare @sequentialLiquidationMatSup varchar(20)
declare @idWarehouse int
declare @sequentialRequisition varchar(20)

select TOP 1 @idRemissionGuide = b.id, @sequentialRemissionGuide = CONVERT(varchar(20), c.[sequential]) 
from [dbo].[DocumentSource] a
join [dbo].[RemissionGuide] b on a.[id_documentOrigin] = b.[id]
join [dbo].[Document] c on c.[id] = b.[id]
where [id_document] = @id

select top 1 @idWarehouse = [idWarehouse] 
from [dbo].[InventoryMove] 
where [id] = @id

select top 1 @sequentialRequisition = convert(varchar(20), dms.sequential)
from [dbo].[DispatchMaterialSequential] dms
where [id_RemissionGuide] = @idRemissionGuide and [id_Warehouse] = @idWarehouse

declare @docSource table
(
	[idDocument]		int
)
declare @invMove table
(
	[idInventoryMove]	int,
	[idWarehouse]		int
)


if (isnull(@sequentialRequisition, '') = '')
begin
	insert into @docSource
	select a.[id_document]
	from [dbo].[DocumentSource] a
	where [id_documentOrigin] = @idRemissionGuide

	insert into @invMove
	select a.[id], a.[idWarehouse] 
	from [dbo].[InventoryMove] a
	join [dbo].[Document] b on a.[id] = b.[id]
	join [dbo].[InventoryReason] d on a.[id_inventoryReason] = d.[id] and d.[code] = 'EPTAMDL'
	join [dbo].[DocumentState] c on b.[id_documentState] = c.[id] and c.[code] = '03'

	select top 1 @idWarehouse = b.[idWarehouse]
	from @docSource a 
	join @invMove b on a.idDocument = b.idInventoryMove

	select top 1 @sequentialRequisition = convert(varchar(20), dms.sequential)
	from [dbo].[DispatchMaterialSequential] dms
	where [id_RemissionGuide] = @idRemissionGuide and [id_Warehouse] = @idWarehouse
end

-- Liquidation
select TOP 1 @idLiquidationMatSup = b.id, @sequentialLiquidationMatSup = CONVERT(varchar(20), c.[sequential]) 
from [dbo].[DocumentSource] a
join [dbo].[LiquidationMaterialSupplies] b on a.[id_documentOrigin] = b.[id]
join [dbo].[Document] c on c.[id] = b.[id]
where [id_document] = @id


select distinct
	im.[id]
	, case	when (RTRIM(LTRIM(apd.valueCode)) = 'I') then 'INGRESO DE INVENTARIO'
			when (RTRIM(LTRIM(apd.valueCode)) = 'E') THEN 'EGRESO DE INVENTARIO' END AS TituloReporte
	, wa.[name] as [Bodega] 
	, ir.[name] as [Motivo]
	, doc.[emissionDate] as [FechaEmision]
	, it.[masterCode] as [CodigoProducto]
	, it.[name] as [DescripcionProducto]
	, mu.[name] as [UnidadMedida]
	, mu.[code] as [CodigoUnidadMedida]
	, imd.[amountMove] as [Cantidad]
	, convert(varchar(20), im.[sequential]) as [NumeroSecuencia]
	, wl.[id] as [IdUbicacion]
	, wl.[name] as [NombreUbicacion]
	, cc.[name] as [CentroCosto]
	, scc.[name] as [SubCentroCosto]
	, apd.[valueCode] as [CodigoNaturaleza]---------------------------
	, apd.[valueName] as [NombreNaturaleza]
	, isnull(@sequentialRemissionGuide,'') as [SecuenciaGuiaRemision]
	, isnull(@sequentialRequisition,'') as [SecuenciaRequisicion]
	, isnull(@sequentialLiquidationMatSup,'') as [SecuenciaLiquidacionMateriales]
	, ep.[id_company] as [IdCompania], ISNULL(doc.[description],'') as [Descripcion]
	, Pl.[number] as [SecTransac]
	, Convert(varchar(100),ds.[name]) as [EstadoDocumento]
	, isnull(@sequentialRemissionGuide,'') as [SecuenciaGuiaRemision]
	, itt.[name] as [tipoItem]
	, its.[name] as [itemTalla]
	, case  
		when (RTRIM(LTRIM(apd.valueCode)) = 'E') then ISNULL(l.internalNumber,'')
		when (RTRIM(LTRIM(apd.valueCode)) = 'I') then ISNULL(Pl.internalNumber,'') 
		else ''
		END as loteegreso
	--,l.internalNumber as loteegreso-------------------------
	,met2.code as KGOLB
	,pres.maximum as Maximum02
	,pres.minimum as Minimum03
	,CC2.name as CC02
	,cc3.name as SCC02
	,pl.internalNumber as LoteOrigenIngreso--------
	,imd.balance as balance
	,imd.unitPrice as preciounitario
	,mu.code 
	,pres.code


	,isnull (case
	when met2.code ='Lbs' then Round((pres.maximum*pres.minimum*imd.amountMove),2) 
	when  met2.code ='Kg' then rOUND(((pres.maximum*pres.minimum*imd.amountMove)/2.2046),2 )
	else 0 end,0 ) lbskg --SIEMPRE LIBRAS
	,isnull (case
	when met2.code ='Lbs' then Round((pres.maximum*pres.minimum*imd.amountMove*2.2046),2) 
	when  met2.code ='Kg' then Round((pres.maximum*pres.minimum*imd.amountMove),2)
	else 0 end,0 ) kg --SIEMPRE KILOS


	,isnull (case
	when met2.code ='Lbs' then Round((pres.maximum*pres.minimum*imd.amountMove),2 )
	when  met2.code ='Kg' then Round(((pres.maximum*pres.minimum*imd.amountMove)*2.2046),2 )
	else 0 end,0 ) lbskg2 --SIEMPRE LIBRAS
	,isnull (case
	when met2.code ='Lbs' then Round((pres.maximum*pres.minimum*imd.amountMove/2.2046) ,2)
	when  met2.code ='Kg' then Round((pres.maximum*pres.minimum*imd.amountMove ),2 )
	else 0 end,0 ) kg2 --SIEMPRE KILOS,
	 

	
from [dbo].[InventoryMove] im
inner join [dbo].[Document] doc on im.[id] = doc.[id]
inner join [dbo].[DocumentState] ds on ds.[id] = doc.[id_documentState]
inner join [dbo].[EmissionPoint] ep on doc.[id_emissionPoint] = ep.[id]
inner join [dbo].[InventoryMoveDetail] imd on im.[id] = imd.[id_inventoryMove]
inner join [dbo].[WarehouseLocation] wl on imd.[id_warehouseLocation] = wl.[id]
inner join [dbo].[InventoryReason] ir on im.[id_inventoryReason] = ir.[id]
inner join [dbo].[Item] it on imd.[id_item] = it.[id]
inner join [dbo].[ItemInventory] ii on it.[id] = ii.[id_item]
inner join [dbo].[Warehouse] wa on imd.[id_warehouse] = wa.[id]
inner join [dbo].[MetricUnit] mu on it.id_metricType = mu.[id]
inner join [dbo].[AdvanceParametersDetail] apd on im.[idNatureMove] = apd.[id]
left outer join [dbo].[CostCenter] cc on cc.[id] = imd.[id_costCenter]
left outer join [dbo].[CostCenter] scc on scc.[id] = imd.[id_subCostCenter]
inner join [dbo].[ItemType] itt on it.[id_itemType] = itt.[id]

--left join ProductionLot pl1 on im.id_productionLot = pl1.id
--inner join [dbo].[Lot] l on l.[id] = pl1.[id]
left join [dbo].[ItemGeneral] itg on itg.id_item = it.id
left join [dbo].[ItemSize] its on its.[id] = itg.[id_size]
inner join Presentation Pres on Pres.id = it.id_presentation
inner join MetricUnit met2 on met2.id = pres.id_metricUnit
----------------------------------------------------------------
 inner join ProductionLot Pl on Pl.id = im.id_productionLot
--inner join lot lt2 on lt2.id = im.id_productionLot
--inner join ProductionLotDetail pld 
--		   on pld.id_productionLot =  Pl.id
--		     -- and pld.id_item = imd.id_item
inner join ProductionProcess Pp on Pp.id = Pl.id_productionProcess
left join CostCenter CC2 on CC2.id  = Pp.id_CostCenter
left join CostCenter Cc3 on CC3.id = PP.id_SubCostCenter

left join 
(select a1.id ,
		a1.number,
		c1.id_originlot,
		c1.id_item,
		case 
	    when isnull(a1.internalNumber,'') != '' then  a1.internalNumber
		when isnull(b1.internalNumber,'') != '' then b1.internalNumber
		else ''
		end as internalNumber,
		'E' natureMove
 from [dbo].[Lot] a1 
 left join ProductionLot b1 on a1.id = b1.id
 inner join ProductionLotDetail c1 on c1.id_originLot = b1.id
) l on
 l.natureMove = RTRIM(LTRIM(apd.valueCode))
and l.id_originLot = imd.id_lot
and l.id_item = imd.id_item

-- pld.id_originLot= l.id


where 1 = 1 
and im.[id] = @id

