IF OBJECT_ID('inv_Exportar_Transf_Placa_Tunel') IS NULL
EXEC('CREATE PROCEDURE inv_Exportar_Transf_Placa_Tunel AS')

GO
ALTER   Procedure [dbo].[inv_Exportar_Transf_Placa_Tunel]
(
@startEmissionDate			DateTime,
@endEmissionDate			DateTime,
@id_warehouse				int,
@id_warehouseLocation		int
)
AS BEGIN

Set NoCount On

SELECT   J.name warehouseLocation,
PL.id id_Lot ,
IMT.dateTimeEntry ,
PL.internalNumber numberLot, 
PC.name car,
IMTD.boxesToReceive quantity,
MU.code metricUnit,
I.masterCode, 
I.name nameItem
FROM Document D
INNER JOIN InventoryMove IV WITH(NOLOCK) ON D.id = IV.id
INNER JOIN InventoryMovePlantTransfer IMT WITH(NOLOCK) ON IV.id = IMT.id_inventoryMoveEntry 
INNER JOIN InventoryMovePlantTransferDetail IMTD WITH(NOLOCK) ON IMT.id = IMTD.id_inventoryMovePlantTransfer
INNER JOIN LiquidationCartOnCart LCC WITH(NOLOCK) ON IMTD.id_liquidationCartOnCart = LCC.id
INNER JOIN LiquidationCartOnCartDetail LCCD WITH(NOLOCK) ON LCC.id = LCCD.id_LiquidationCartOnCart AND IMTD.id_liquidationCartOnCartDetail = LCCD.id
INNER JOIN ProductionCart PC  WITH(NOLOCK) ON LCCD.id_ProductionCart = PC.id
INNER JOIN ProductionLot PL WITH(NOLOCK) ON LCC.id_ProductionLot = PL.id
INNER JOIN dbo.MachineProdOpeningDetail F WITH (NOLOCK) ON (F.id = IMT.id_machineProdOpeningDetail)
INNER JOIN dbo.MachineForProd G WITH (NOLOCK) ON (G.id = F.id_MachineForProd)
INNER JOIN dbo.MachineProdOpening MPO WITH (NOLOCK) ON (MPO.id = F.id_MachineProdOpening)
INNER JOIN Warehouse W WITH(NOLOCK) ON G.id_materialWarehouse = W.id
INNER JOIN dbo.WarehouseLocation J WITH (NOLOCK) ON (J.id = G.id_materialWarehouseLocation)
INNER JOIN Item I WITH(NOLOCK) ON LCCD.id_ItemToWarehouse = I.id
INNER JOIN ItemInventory MI WITH(NOLOCK) ON I.id= MI.id_item 
INNER JOIN MetricUnit MU WITH(NOLOCK) ON MI.id_metricUnitInventory = MU.id 
WHERE D.emissionDate >= @startEmissionDate AND  D.emissionDate <= @endEmissionDate  
AND G.id_materialWarehouse = @id_warehouse
AND G.id_materialWarehouseLocation = @id_warehouseLocation
AND LCCD.id_ItemToWarehouse NOT IN (SELECT id_item FROM OpeningClosingPlateLyingDetail 
						WHERE id_warehouse = @id_warehouse AND id_warehouseLocation = @id_warehouseLocation)
ORDER BY I.name DESC




END