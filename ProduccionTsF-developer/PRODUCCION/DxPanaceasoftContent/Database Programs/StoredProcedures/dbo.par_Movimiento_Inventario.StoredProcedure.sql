If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_Movimiento_Inventario'
	)
Begin
	Drop Procedure dbo.par_Movimiento_Inventario
End
Go
Create Procedure dbo.par_Movimiento_Inventario
(
	@id int
)
As 
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


select 
	im.[id]
	, case	when (RTRIM(LTRIM(apd.valueCode)) = 'I') then 'INGRESO'
			when (RTRIM(LTRIM(apd.valueCode)) = 'E') THEN 'EGRESO' END AS TituloReporte
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
	, apd.[valueCode] as [CodigoNaturaleza]
	, apd.[valueName] as [NombreNaturaleza]
	, isnull(@sequentialRemissionGuide,'') as [SecuenciaGuiaRemision]
	, isnull(@sequentialRequisition,'') as [SecuenciaRequisicion]
	, isnull(@sequentialLiquidationMatSup,'') as [SecuenciaLiquidacionMateriales]
	, ep.[id_company] as [IdCompania], ISNULL(doc.[description],'') as [Descripcion] 
from [dbo].[InventoryMove] im
inner join [dbo].[Document] doc on im.[id] = doc.[id]
inner join [dbo].[EmissionPoint] ep on doc.[id_emissionPoint] = ep.[id]
inner join [dbo].[InventoryMoveDetail] imd on im.[id] = imd.[id_inventoryMove]
inner join [dbo].[WarehouseLocation] wl on imd.[id_warehouseLocation] = wl.[id]
inner join [dbo].[InventoryReason] ir on im.[id_inventoryReason] = ir.[id]
inner join [dbo].[Item] it on imd.[id_item] = it.[id]
inner join [dbo].[ItemInventory] ii on it.[id] = ii.[id_item]
inner join [dbo].[Warehouse] wa on imd.[id_warehouse] = wa.[id]
inner join [dbo].[MetricUnit] mu on ii.[id_metricUnitInventory] = mu.[id]
inner join [dbo].[AdvanceParametersDetail] apd on im.[idNatureMove] = apd.[id]
left outer join [dbo].[CostCenter] cc on cc.[id] = imd.[id_costCenter]
left outer join [dbo].[CostCenter] scc on scc.[id] = imd.[id_subCostCenter]
where 1 = 1 
and im.[id] = @id



-- [dbo].[par_Movimiento_Inventario] 171137



--select b.[name], a.* from InventoryMove a join InventoryReason b on a.id_inventoryReason = b.id 
Go
