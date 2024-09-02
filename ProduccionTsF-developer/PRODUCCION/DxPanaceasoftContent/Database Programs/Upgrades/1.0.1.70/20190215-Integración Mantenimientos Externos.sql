/*
 Script de Integración de Mantenimientos de EXTERNOS
 Fecha: 2019-02-15 --> Campos adicionales.
 */

If ColumnProperty(OBJECT_ID('dbo.InventoryReason'), 'requiereSystemLotNubmber', 'ColumnId') Is Null
Begin
	Alter Table dbo.InventoryReason Add requiereSystemLotNubmber Bit Null
End
Go
If ColumnProperty(OBJECT_ID('dbo.InventoryReason'), 'requiereUserLotNubmber', 'ColumnId') Is Null
Begin
	Alter Table dbo.InventoryReason Add requiereUserLotNubmber Bit Null
End
Go
If Not Exists (Select * From sys.indexes where name = 'IX_ProductionLot_12' And object_id = OBJECT_ID('dbo.ProductionLot'))
	Create Index IX_ProductionLot_12 On dbo.ProductionLot(internalNumber)
Go
