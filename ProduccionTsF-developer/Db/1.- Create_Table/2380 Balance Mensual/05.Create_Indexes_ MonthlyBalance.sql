

--- INDICES OPTMIZAICON CONSULTA SALDOS

CREATE NONCLUSTERED INDEX IX_InventoryMoveDetail_Saldo
ON [dbo].[InventoryMoveDetail] ([id_item])
INCLUDE ([id_lot],[id_inventoryMove],[entryAmount],[exitAmount],[id_metricUnit],[id_warehouse],[id_warehouseLocation])

CREATE NONCLUSTERED INDEX IX_MonthlyBalance_Saldo
ON [dbo].[MonthlyBalance] ([id_company],[Anio],[Periodo],[id_warehouse])
INCLUDE ([id_item],[id_warehouseLocation],[id_productionLot],[number_productionLot],[sequencial_productionLot],[id_metric_unit],[SaldoActual])