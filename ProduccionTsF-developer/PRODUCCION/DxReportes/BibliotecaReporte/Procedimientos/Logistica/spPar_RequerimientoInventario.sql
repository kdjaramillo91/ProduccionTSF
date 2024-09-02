/****** Object:  StoredProcedure [dbo].[spPar_RequerimientoInventario]    Script Date: 22/06/2023 10:57:28 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER   PROC [dbo].[spPar_RequerimientoInventario]
@id		INT
AS

SET NOCOUNT ON

SELECT req.id AS IdRequerimiento, 
ISNULL(doc.sequential, '') AS NumeroRequerimiento, 
ISNULL(doc.emissionDate, '') AS FechaEmision,
ISNULL(docst.name, '') AS Estado,
ISNULL(per.tradeName, '') AS PersonaRequiere,
ISNULL(ite.masterCode, '') AS CodigoProducto,
ISNULL(ite.name, '') AS NombreProducto,
ISNULL(docg.sequential, '') AS NumeroGuia, 
ISNULL(docg.emissionDate, '') AS FechaGuia,
ISNULL(seqR.sequential, 0) AS NumeroRequisicion,
ISNULL(met.code, '') AS Medida,
ISNULL(reqD.quantityRequest, 0) AS CantidadRequerida,
ISNULL(reqD.quantityUpdate, 0) AS CantidadEntregada,
ISNULL(apd.valueName, '') AS NaturalezaMovimiento,
ISNULL(Virt.nameWarehouse, '') AS NombreBodega, 
ISNULL(Virt.nameLocation, '') AS Ubicacion
FROM RequestInventoryMove req
INNER JOIN RequestInventoryMoveDetail reqD
 ON reqD.id_RequestInventoryMove = req.id
INNER JOIN Document doc
ON doc.id = req.id
LEFT OUTER JOIN DocumentSource docs
 ON docs.id_document = doc.id
LEFT OUTER JOIN DocumentState docst
 ON docst.id = doc.id_documentState 
 -- Guia de Remisión y Documento de Guia
LEFT OUTER JOIN RemissionGuide rem
 ON rem.id = docs.id_documentOrigin
LEFT OUTER JOIN Document docg
 ON docg.id = rem.id
-- Bodega y Ubicación
LEFT OUTER JOIN (SELECT ware.id AS IdWarehouse, ware.name AS nameWarehouse, 
				loc.id AS IdLocation, loc.name AS nameLocation
				FROM Warehouse ware
				  INNER JOIN WarehouseLocation loc
				   ON loc.id_warehouse = ware.id) AS Virt 
 ON Virt.IdWarehouse = req.id_Warehouse
 AND Virt.IdLocation = reqD.idWarehouseLocation
-- Número de Requisición
LEFT OUTER JOIN DispatchMaterialSequential seqR
 ON seqR.id_RemissionGuide = rem.id
 AND seqR.id_Warehouse = req.id_Warehouse
LEFT OUTER JOIN AdvanceParametersDetail apd
 ON apd.id = req.id_NatureMove
LEFT OUTER JOIN Item ite
 ON ite.id = reqD.id_item
 -- Unidad de MEdida 
LEFT OUTER JOIN ItemInventory iteI
 ON iteI.id_item = ite.id
 AND iteI.id_item = reqD.id_item
LEFT OUTER JOIN MetricUnit met
 ON met.id = iteI.id_metricUnitInventory
 -- Persona Requerida
LEFT OUTER JOIN Person per
 ON per.id = req.id_PersonRequest
WHERE req.id = @id

/*
	EXEC spPar_RequerimientoInventario 986345
	exec spPar_RequerimientoInventario @id=147005
	exec spPar_RequerimientoInventario @id=147010
	exec spPar_RequerimientoInventario @id=1126112
*/

GO