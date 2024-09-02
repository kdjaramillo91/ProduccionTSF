--exec spPar_MasterizadoPers @id=646234
GO
/****** Object:  StoredProcedure [dbo].[par_Movimiento_InventarioMasterizado]646234    Script Date: 22/01/2023 14:53:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


Create procedure [dbo].[spPar_MasterizadoPers] 

	@id int
as 
set nocount on 

select
Mastered.id as ID
,document.emissionDate as FECHAEMISION
,document.sequential as secuencia
,Mastered.dateTimeStartMastered as FECHAINICIO
,Mastered.dateTimeEndMastered as FECHAFIN
,document.number as NDOCUMENTO
,Document.description as Descripcion
,DocumentState.name as ESTADO
,Responsable.fullname_businessName as RESPONSABLE
,Warehouse.name as BODEGA_CABECERA
,WarehouseLocation.name as UBICACION_CABECERA
,turn.name as TURNO
,lot.number AS NLOTE
,lot.internalNumber as INLOTE 
,item1.masterCode as  CODIGO1
,itemtype1.name as TIPOITEM1
,itemtype2.name as TIPOITEM2
,item2.masterCode as  CODIGO2
,item1.name as NPRODUCTO1
,metricunit1a.code as UN
,itemsize1.name as TALLA1
,item2.name as NPRODUCTO2
,itemsize2.name as TALLA2
,Presentation1.name as PRESENTACION
,Presentation2.name as Presentacion1
,itemtrademark1.name as MARCA1
,itemtrademark2.name as MARCA2
,warehouseexitboxed.name BODEGA1
,WarehouseLocation1.name UBODEGA1
,costcenter1.name CENTROCOSTO1
,subcostcenter1.NAME SUBCENTROCOSTO1
,MasteredDetail.quantityMP Cantidad1
,warwhousemastered.name AS BODEGA2
,WarehouseLocation2.name UBODEGA2
,costcenter2.name CENTROCOSTO2
,subcostcenter2.NAME SUBCENTROCOSTO2

,costcenter3.name CENTROCOSTO3
,subcostcenter3.NAME SUBCENTROCOSTO3
,MasteredDetail.quantityPT as cantidad2
,warehouseentryboxed.name AS BODEGA3
,WarehouseLocation3.name UBODEGA3

,MasteredDetail.quantityBoxes as Cantidad3
--,InventoryMovew1.natureSequential as NEGRESO1
--,InventoryReason1.name as Razonegreso
--,InventoryReason3.name as Razoningreso
--,InventoryMovew2.natureSequential as NINGRESO2
--,InventoryMovew3.natureSequential as NINGRESO3
,isnull(documentop.number,'') as OP
,ISNULL(cust2.fullname_businessName,'') as CLIENTE
,case when ISNULL(cust2.fullname_businessName,'')= '' then 'S/C' else cust2.fullname_businessName end  as CLIENTEPers
,Presentation1.maximum as MAXIMO
,Presentation1.minimum as MINIMO
,Presentation2.maximum as MAXIMO2
,Presentation2.minimum as MINIMO2
,Presentation1.name as PRESENTACION1
,Presentation2.name as PRESENTACION2
,isnull(case when  MetricUnit1.code ='Kg' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityMP),2)
else 0 end,0) as KILOS1
,isnull(case when  MetricUnit1.code ='Kg' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityMP)*2.2046,2)
else 0 end,0) as LIBRAS1
,isnull(case when  MetricUnit2.code ='Kg' then Round((Presentation2.maximum*Presentation2.minimum*MasteredDetail.quantityPT),2)
else 0 end,0) as KILOS2
,isnull(case when  MetricUnit2.code ='Kg' then Round((Presentation2.maximum*Presentation2.minimum*MasteredDetail.quantityPT)*2.2046,2)
else 0 end,0) as LIBRAS2
,isnull(case when  MetricUnit1.code ='Kg' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityBoxes),2)
else 0 end,0) as KILOS3
,isnull(case when  MetricUnit1.code ='Kg' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityBoxes)*2.2046,2)
else 0 end,0) as LIBRAS3

,isnull(case when  MetricUnit1.code ='lbs' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityMP),2)
else  Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityMP)*2.2046,2) end,0) as LBSOKG1

,isnull(case when  MetricUnit2.code ='lbs' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityPT),2)
else  Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityPT)*2.2046,2) end,0) as LBSOKG2

,isnull(case when  MetricUnit1.code ='lbs' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityBoxes),2)
else Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityBoxes)*2.2046,2) end,0) as LBSOKG3

,isnull(case when  MetricUnit1.code ='lbs' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityMP)/2.2046,2)
else  Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityMP),2) end,0) as kgolb1

,isnull(case when  MetricUnit2.code ='lbs' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityPT)/2.2046,2)
else  Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityPT),2) end,0) as kgolb2

,isnull(case when  MetricUnit1.code ='lbs' then Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityBoxes)/2.2046,2)
else Round((Presentation1.maximum*Presentation1.minimum*MasteredDetail.quantityBoxes),2) end,0) as kgolb3

,SecMaster =(select d2.number from documentsource ds
inner join document d on d.id = ds.id_documentOrigin

left join document d2 on d2.id = ds.id_document
inner join inventorymove im on im.id = ds.id_document
--and im.idNatureMove = 5 and d.id_documentType = 1166
inner join InventoryReason ir on im.id_inventoryReason = ir.id and ir.code = 'IPAMM'
where id_documentOrigin = @id
And d2.id_documentState <> 5)


,RazonMaster =(select ir.name from documentsource ds
inner join document d on d.id = ds.id_documentOrigin
left join document d2 on d2.id = ds.id_document
inner join inventorymove im on im.id = ds.id_document
--and im.idNatureMove = 5 and d.id_documentType = 1166
inner join InventoryReason ir on im.id_inventoryReason = ir.id and ir.code = 'IPAMM'
where id_documentOrigin = @id
And d2.id_documentState <> 5)


,SeCCajasS =(select d2.number from documentsource ds
inner join document d on d.id = ds.id_documentOrigin
left join document d2 on d2.id = ds.id_document
inner join inventorymove im on im.id = ds.id_document
--and im.idNatureMove = 5 and d.id_documentType = 1166
inner join InventoryReason ir on im.id_inventoryReason = ir.id and ir.code = 'IPACS'
where id_documentOrigin = @id
And d2.id_documentState <> 5)

,RazonCajasS =(select ir.name from documentsource ds
inner join document d on d.id = ds.id_documentOrigin
left join document d2 on d2.id = ds.id_document
inner join inventorymove im on im.id = ds.id_document
--and im.idNatureMove = 5 and d.id_documentType = 1166
inner join InventoryReason ir on im.id_inventoryReason = ir.id and ir.code = 'IPACS'
where id_documentOrigin = @id
And d2.id_documentState <> 5)

,RazonEgreso =(select ir.name from documentsource ds
inner join document d on d.id = ds.id_documentOrigin
left join document d2 on d2.id = ds.id_document
inner join inventorymove im on im.id = ds.id_document
--and im.idNatureMove = 5 and d.id_documentType = 1166
inner join InventoryReason ir on im.id_inventoryReason = ir.id and ir.code = 'EPAM'
where id_documentOrigin = @id
And d2.id_documentState <> 5)

,SecEgreso =(select d2.number from documentsource ds
inner join document d on d.id = ds.id_documentOrigin
left join document d2 on d2.id = ds.id_document
inner join inventorymove im on im.id = ds.id_document
--and im.idNatureMove = 5 and d.id_documentType = 1166
inner join InventoryReason ir on im.id_inventoryReason = ir.id and ir.code = 'EPAM'
where id_documentOrigin = @id
And d2.id_documentState <> 5)

,lotepeRs = case when MasteredDetail.quantityMP < MasteredDetail.quantityBoxes Then 
lot.internalNumber else lot.internalNumber end
,co.logo as Logo
, co.logo2 as Logo2,
pro.internalNumber as Lote1
from Mastered Mastered
inner join Document Document on Document.id = Mastered.id --DOCUMENTO
inner join EmissionPoint emi on emi.id = Document.id_emissionPoint
inner join Company co on emi.id_company= co.id
inner join DocumentState DocumentState on DocumentState.id = Document.id_documentState--ESTADO DOCUMENTO
inner join Person Responsable on Responsable.id = Mastered.id_responsable -- RESPONSABLE
INNER join Warehouse Warehouse on Warehouse.id = Mastered.id_warehouseBoxes-- BODEGA
INNER join WarehouseLocation WarehouseLocation on WarehouseLocation.id = Mastered.id_warehouseLocationBoxes--UBICACION BODEGA
inner join MasteredDetail MasteredDetail on MasteredDetail.id_mastered = mastered.id --DETALLE
inner join lot lot on lot.id = MasteredDetail.id_lotMP --LOTE
inner join ProductionLot pro on pro.id=lot.id
inner join turn turn on turn.id = mastered.id_turn
--PRODUCTOS EN PROCESO--CAJAS SUELTAS
inner join item item1 on item1.id = MasteredDetail.id_productMP
inner join ItemGeneral itemgeneral1 on itemgeneral1.id_item = item1.id
inner join itemtype itemtype1 on itemtype1.id = item1.id_itemType
inner join ItemSize itemsize1 on itemsize1.id = itemgeneral1.id_size
inner join Presentation Presentation1 on Presentation1.id = item1.id_presentation
inner join MetricUnit MetricUnit1 on MetricUnit1.id = Presentation1.id_metricUnit
inner join ItemTrademark itemtrademark1 on itemtrademark1.id = itemgeneral1.id_trademark
left join metricunit metricunit1a on metricunit1a.id = item1.id_metricType
--PRODUCTOS TERMINADOS MASTERS--
inner join item item2 on item2.id = MasteredDetail.id_productPT
inner join ItemGeneral itemgeneral2 on itemgeneral2.id_item = item2.id
inner join itemtype itemtype2 on itemtype2.id = item2.id_itemType
inner join ItemSize itemsize2 on itemsize2.id = itemgeneral2.id_size
inner join Presentation Presentation2 on Presentation2.id = item2.id_presentation
inner join MetricUnit MetricUnit2 on MetricUnit2.id = Presentation2.id_metricUnit
inner join ItemTrademark itemtrademark2 on itemtrademark2.id = itemgeneral2.id_trademark
left join metricunit metricunit2a on metricunit2a.id = item2.id_metricType
left join warehouse warehouseexitboxed on warehouseexitboxed.id = mastereddetail.id_boxedWarehouse
left join warehouse warwhousemastered on warwhousemastered.id = mastereddetail.id_masteredWarehouse
inner join costcenter costcenter1 on costcenter1.id = MasteredDetail.id_costCenterExitBoxed
inner join costcenter subcostcenter1 on subcostcenter1.id = MasteredDetail.id_subCostCenterExitBoxed
inner join costcenter costcenter2 on costcenter2.id = MasteredDetail.id_costCenterEntryMastered
inner join costcenter subcostcenter2 on subcostcenter2.id = MasteredDetail.id_subCostCenterEntryMastered
inner join costcenter costcenter3 on costcenter3.id = MasteredDetail.id_costCenterEntryBoxes
inner join costcenter subcostcenter3 on subcostcenter3.id = MasteredDetail.id_subCostCenterEntryBoxes
left join warehouse warehouseentryboxed on warehouseentryboxed.id = MasteredDetail.id_warehouseBoxes
inner join WarehouseLocation WarehouseLocation1 on WarehouseLocation1.id = MasteredDetail.id_boxedWarehouseLocation-----
inner join WarehouseLocation WarehouseLocation2 on WarehouseLocation2.id = MasteredDetail.id_masteredWarehouseLocation 
inner join  WarehouseLocation WarehouseLocation3 on WarehouseLocation3.id = MasteredDetail.id_warehouseLocationBoxes

--inner join InventoryMove InventoryMovew1 on InventoryMovew1.idWarehouse = MasteredDetail.id_boxedWarehouse
--and InventoryMovew1.id_inventoryReason = 1149 and InventoryMovew1.sequential = Document.sequential
--inner join InventoryReason  InventoryReason1 on InventoryReason1.id = InventoryMovew1.id_inventoryReason

--inner join InventoryMove InventoryMovew2 on InventoryMovew2.idWarehouse = MasteredDetail.id_masteredWarehouse
--and InventoryMovew2.id_inventoryReason = 1150 and InventoryMovew2.sequential = Document.sequential

--inner join InventoryMove InventoryMovew3 on InventoryMovew3.idWarehouse = MasteredDetail.id_warehouseBoxes
--and InventoryMovew3.id_inventoryReason = 1151 and InventoryMovew3.sequential = Document.sequential

--inner join InventoryReason  InventoryReason3 on InventoryReason3.id = InventoryMovew3.id_inventoryReason
left join document documentop on documentop.id = MasteredDetail.id_sales 
left join salesorder salesorder on salesorder.id = documentop.id
left join person person on person.id = salesorder.id_customer
left join customer cust on cust.id = MasteredDetail.id_customer
left join person cust2 on cust2.id = cust.id



where mastered.id = @id 

--
