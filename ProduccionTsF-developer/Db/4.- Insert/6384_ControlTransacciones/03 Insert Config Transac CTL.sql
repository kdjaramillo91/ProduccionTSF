
-------------------------------------
--------- Insercion de Datos --------
-------------------------------------
 

insert into TransCtlDocumentTypeConfig
(
	Id, 
	DtName,
	Peso,
	EstTimePerform,
	Controller,
	Method,
	CodeStateOK,
	CodeStateError
)values 
(59,'Tumbada Placa', 1, 0,'DXPANACEASOFT.Controllers.OpeningClosingPlateLyingController','ApproveOpeningClosingPlateLying','03','01'),
(1166,'Masterizado', 1, 0,'DXPANACEASOFT.Controllers.MasteredController','ApproveMastered','03','01'), -- Mastered
(1151,'Transferencia a Túneles', 1, 0,'DXPANACEASOFT.Controllers.InventoryMovePlantTransferController','ApproveInventoryMovePlantTransfer','03','01'); -- InventoryMovePlantTransfer -- 135


insert into TransCtlDocumentTypeConfigDetail
(	
	TransCtlConfigId,
	TableName,
	OrderExec,
	Stage
) values 
-- TUMBADA DE PLACAS
(59,'Document',1,''),
(59,'OpeningClosingPlateLying',2,''), 
(59,'DocumentSource',3,''), 
(59,'InventoryMove',4,'') ,
(59,'InventoryExitMove',5,''),
(59,'InventoryMoveDetail',6,''),
(59,'DocumentSource',7,''),
(59,'InventoryEntryMove',8,''),
(59,'InventoryMoveDetail',9,''),
(59,'InventoryMoveDetailTransfer',10,''),
-- MASTERIZADO
(1166,'Document',1,''),
(1166,'DocumentSource',2,''),
(1166,'InventoryExitMove',3,''),
(1166,'InventoryMoveDetail',4,''),
(1166,'DocumentSource',5,''),  -- Cajas sueltas sobrante
(1166,'InventoryEntryMove',6,''),
(1166,'InventoryMoveDetail',7,''),
(1166,'InventoryEntryMove',8,''),
(1166,'InventoryMoveDetail',9,''),
-- TRANSFERENCIA A TUNELES
(1151,'DocumentSource',1,''), -- => MovePlantTransferEntry
(1151,'Document',2,''), 
(1151,'InventoryMove',3,'') ,
(1151,'InventoryEntryMove',4,''),
(1151,'InventoryMoveDetail',5,''),
(1151,'InventoryMoveDetailTransfer',6,''), -- =>exit
(1151,'InventoryMovePlantTransfer',7,'')
  
