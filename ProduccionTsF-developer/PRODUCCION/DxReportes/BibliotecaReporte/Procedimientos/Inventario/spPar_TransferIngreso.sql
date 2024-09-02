--USE [PanaceaProduccionPC0808_2022]

--exec "PanaceaProduccionPC0808_2022"."dbo"."par_Movimiento_InventarioMotivo";1 889078, 1
GO
/****** Object:  StoredProcedure [dbo].[par_Movimiento_InventarioMotivo]    Script Date: 28/12/2022 23:23:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--select *from MetricUnit


Create procedure [dbo].[spPar_TransferIngreso]

	@id int,
	@id_user int
as 
set nocount on 

declare @idRemissionGuide int
declare @idLiquidationMatSup int
declare @sequentialRemissionGuide varchar(20)
declare @sequentialLiquidationMatSup varchar(20)
declare @idWarehouse int
declare @sequentialRequisition varchar(20)
declare @nomUser varchar(250)

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

set @nomUser = (select top 1 fullName_businessName from [user] u
				inner join person e
				on e.id = u.id_employee
				where u.id = @id_user)

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
	, convert(varchar(50),it.[name]) as [DescripcionProducto]
	, mu.[name] as [UnidadMedida]
	, mu.[code] as [CodigoUnidadMedida]
	, imd.[amountMove] as [Cantidad]
	, convert(varchar(20), doc.[number]) as [NumeroSecuencia]
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
	, convert(varchar(50),Substring(it.description,14,6)) as medidas
	, [numLote] = case when isnull(plo.internalNumber,'') ='' then l.internalNumber else plo.internalNumber  end-------------------------------
	, l.[number] as [SecTransac]
	, Convert(varchar(100),ds.[name]) as [EstadoDocumento]
	, its.[name] as [itemTalla]
	, itt.[name] as [tipoItem]
	, @nomUser as [nombreUsuario]
	, waE.[name] as [bodegaIngreso]
	, waI.[name] as [bodegaEgreso]
	, wlI.[name] as [UbicacionEgreso]
	, Convert(varchar(30),dminv.[number]) as [numeroEgreso]
	

	, case when mu2.code = 'Lbs' then 
	(imd.amountMove * (pres.minimum * pres.maximum))  
	when mu2.code = 'Kg' then (imd.amountMove * (pres.minimum * pres.maximum) * 2.2046 )
	 --(imd.amountMove * (pres.minimum * pres.maximum)* 2.2046  )  
	 else 0
	 -- (imd.amountMove * (pres.minimum * pres.maximum)) *2.2046 
	 end   as Libras


	,case when mu.code = 'Kg' then  (imd.amountMove * (pres.minimum * pres.maximum))
	  when mu2.code = 'Lbs' then  (imd.amountMove * (pres.minimum * pres.maximum) / 2.2046) else 
	 (imd.amountMove * (pres.minimum * pres.maximum)) end  as Kilos

from [dbo].[InventoryMove] im
inner join [dbo].[Document] doc on im.[id] = doc.[id]
inner join [dbo].[DocumentState] ds on ds.[id] = doc.[id_documentState]
inner join [dbo].[EmissionPoint] ep on doc.[id_emissionPoint] = ep.[id]
inner join [dbo].[InventoryMoveDetail] imd on im.[id] = imd.[id_inventoryMove]
left join [dbo].[Lot] l on l.[id] = imd.[id_lot]
left join productionlot plo on plo.id = l.id
inner join [dbo].[WarehouseLocation] wl on imd.[id_warehouseLocation] = wl.[id]
inner join [dbo].[InventoryReason] ir on im.[id_inventoryReason] = ir.[id]
inner join [dbo].[Item] it on imd.[id_item] = it.[id]
inner join [dbo].[ItemInventory] ii on it.[id] = ii.[id_item]
left join [dbo].[ItemGeneral] itg on itg.id_item = it.id
left join [dbo].[ItemSize] its on its.[id] = itg.[id_size]
inner join [dbo].[ItemType] itt on it.[id_itemType] = itt.[id]
inner join [dbo].[Warehouse] wa on imd.[id_warehouse] = wa.[id]
left join [dbo].[Warehouse] waE on im.[idWarehouseEntry] = waE.[id]
inner join [dbo].[MetricUnit] mu on ii.[id_metricUnitInventory] = mu.[id]
inner join [dbo].[AdvanceParametersDetail] apd on im.[idNatureMove] = apd.[id]
inner join Presentation pres on pres.id = it.id_presentation
left outer join [dbo].[CostCenter] cc on cc.[id] = imd.[id_costCenter]
left outer join [dbo].[CostCenter] scc on scc.[id] = imd.[id_subCostCenter]
left join [dbo].[InventoryMoveDetailTransfer] imdt on imd.[id] = imdt.[id_inventoryMoveDetailEntry]
left join [dbo].[Warehouse] waI on imdt.[id_warehouseExit] = waI.[id]
left join [dbo].[WarehouseLocation] wlI on wlI.[id] = imdt.[id_warehouseLocationExit]
left join [dbo].[Document] dminv on dminv.[id] = imdt.[id_inventoryMoveExit]
left join MetricUnit mu2 on mu2.id = pres.id_metricUnit
where 1 = 1 
and im.[id] = @id

--insert into #temp select 241228,'INGRESO','Bodega Virtual de Proveedores','Liquidación Por Pagar','2019-04-24 00:00:00.000','MI000095','Sacos Vacíos','Unidades','Un',8.000000,7655,24,'Ubicación Proveedor: CAMARONERA SAN JOSE S.A. SAN JOCAMAR','LOGISTICA','LOGISTICA GENERAL','I','INGRESO','54592','6206','429',2,' '
--select 241227,'salida','Bodega Virtual de Proveedores','Liquidación Por Pagar','2019-04-24 00:00:00.000','MI000095','Sacos Vacíos','Unidades','Un',8.000000,7655,24,'Ubicación Proveedor: CAMARONERA SAN JOSE S.A. SAN JOCAMAR','LOGISTICA','LOGISTICA GENERAL','I','INGRESO','54592','6206','429',2,' '

--select c.number,b.internalNumber,c.internalNumber from inventorymovedetail a
--inner join productionlot b on b.id = a.id_lot
--inner join lot c on c.id = b.id

-- where a.id_inventoryMove = 763382
--select b.[name], a.* from InventoryMove a join InventoryReason b on a.id_inventoryReason = b.id 




