/****** Object:  StoredProcedure [dbo].[spPar_Ingreso]    Script Date: 12/04/2023 13:06:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER procedure [dbo].[spPar_Ingreso]
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

		 
declare @inventoryReasonFactor table
(
	inventoryReasonCode		varchar(20)
)
declare @docSource table
(
	[idDocument]		int
)
declare @invMove table
(
	[idInventoryMove]	int,
	[idWarehouse]		int
)

insert into @inventoryReasonFactor
select	b.value
from	setting a 
inner join SettingDetail b on 
			b.id_setting = a.id
where		a.code = 'PRIEF'


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
	,case 
	  when f.inventoryReasonCode is not null then
			case 
			  when fact.code ='Kg' then
				   (fact.minimum * fact.maximum) * imd.[amountMove] *  d.conversionToPounds   
			  when fact.code ='Lbs' then
					(fact.minimum * fact.maximum) * imd.[amountMove] * 1
			  end 
	 else 
	    round(case 
				when fact.code ='Kg' then
					(fact.minimum * fact.maximum) * imd.[amountMove] *  2.20462  
				when fact.code ='Lbs' then
					(fact.minimum * fact.maximum) * imd.[amountMove] * 1
				end,2)
	 end as enLibras
	 ,case 
		when  f.inventoryReasonCode is not null then
			case 
				when fact.code ='Kg' then
					(fact.minimum * fact.maximum) * imd.[amountMove] * 1
				when fact.code ='Lbs' then
					(fact.minimum * fact.maximum) * imd.[amountMove] * d.conversionToKilos 
			end
		else
			round(case 
					when fact.code ='Kg' then
						(fact.minimum * fact.maximum) * imd.[amountMove] * 1
					when fact.code ='Lbs' then
						(fact.minimum * fact.maximum) * imd.[amountMove] *  0.453592
				  end ,2)
	 end as enKilos
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
	, l.internalNumber [numLote]
	, l.[number] as [SecTransac]
	, Convert(varchar(100),ds.[name]) as [EstadoDocumento]
	, its.[name] as [itemTalla]
	, itt.[name] as [tipoItem]
	, @nomUser as [nombreUsuario]
	, waE.[name] as [bodegaIngreso]
	, waI.[name] as [bodegaEgreso]
	, wlI.[name] as [UbicacionEgreso]
	, Convert(varchar(30),dminv.[number]) as [numeroEgreso]
	,PERUSER.fullname_businessName AS USERCREATE
	
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
	else 0 end,0 ) kg2, --SIEMPRE KILOS,
	isnull(IMD.ordenProduccion,'') AS OP,
	isnull(pcus.fullname_businessName,'') as Cliente,
	isnull(sell.fullname_businessName,'') as Vendedor,
	isnull(im.noFactura,'') as Factura,
	isnull(im.contenedor,'') as contenedor,
	cia.businessName as Compania,
	pl.internalNumber as NumLoteP
	 

	--into #tee
from [dbo].[InventoryMove] im
inner join [dbo].[Document] doc on im.[id] = doc.[id]
inner join [dbo].[DocumentState] ds on ds.[id] = doc.[id_documentState]
inner join [dbo].[EmissionPoint] ep on doc.[id_emissionPoint] = ep.[id]
inner join [dbo].[InventoryMoveDetail] imd on im.[id] = imd.[id_inventoryMove]
left join [dbo].[Lot] l on l.[id] = imd.[id_lot]
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
left outer join [dbo].[CostCenter] cc on cc.[id] = imd.[id_costCenter]
left outer join [dbo].[CostCenter] scc on scc.[id] = imd.[id_subCostCenter]
left join [dbo].[InventoryMoveDetailTransfer] imdt on imd.[id] = imdt.[id_inventoryMoveDetailEntry]
left join [dbo].[Warehouse] waI on imdt.[id_warehouseExit] = waI.[id]
left join [dbo].[WarehouseLocation] wlI on wlI.[id] = imdt.[id_warehouseLocationExit]
left join [dbo].[Document] dminv on dminv.[id] = imdt.[id_inventoryMoveExit]
inner join [user] USC ON doc.id_userCreate = USC.id
inner join person PERUSER ON PERUSER.id = USC.id_employee
inner join Company cia on cia.id = wa.id_company -------------------------------
left join productionlot pl on pl.id = imd.id_lot
------------------- Conversion
left outer join Presentation PRESEN on
PRESEN.id = it.id
left outer join( 
		select --(a.minimum * a.maximum) * 
				(case when b.code ='Kg' then 2.20462
					  when b.code ='Lbs' then 1
					  end) factorLibras, 

				--(a.minimum * a.maximum) * 
				(case when b.code ='Kg' then 1
					  when b.code ='Lbs' then 0.453592
					  end) factorKilos,
				a.id,
				a.minimum,
				a.maximum,
				b.code
		from Presentation a 
		inner join MetricUnit b on a.id_metricUnit = b.id
		
		
) as fact on  fact.id = it.id_presentation
-- ====================================
left join ItemWeightConversionFreezen d on 
d.id_Item = it.id 
-- ====================================
left join @inventoryReasonFactor f on 
f.inventoryReasonCode = ir.code

inner join Presentation Pres on Pres.id = it.id_presentation
inner join MetricUnit met2 on met2.id = pres.id_metricUnit
left join person pcus on pcus.id = im.id_customer
left join person sell on sell.id = im.id_seller

where 1 = 1 
and im.[id] = @id


-- [dbo].[par_Movimiento_InventarioMotivoConversion] 78052,1
/*
	exec spPar_Ingreso 985371,0
*/


--select * from InventoryMoveDetail where id_inventoryMove = 985371


--select *from productionlot where id = 982911