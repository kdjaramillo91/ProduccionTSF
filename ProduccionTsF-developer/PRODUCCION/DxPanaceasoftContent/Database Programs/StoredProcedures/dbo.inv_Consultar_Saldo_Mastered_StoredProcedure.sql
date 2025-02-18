
-- ==============================================================================================================
-- Creado por:		Wilber Leonard Benavides
-- Fecha Creación:	03 de Julio del 2021
-- Descripción:		Procedimiento almacenado para consultar el Saldo de Inventario en la opción de Mastered.
/*
-- Pruebas:			
					EXEC [dbo].[inv_Consultar_Saldo_Mastered_StoredProcedure] NULL
*/

CREATE PROCEDURE [dbo].[inv_Consultar_Saldo_Mastered_StoredProcedure]
	@ParametrosBusquedaSaldoMastered		NVARCHAR(MAX)
	--@Resultado							NVARCHAR(MAX) OUTPUT
AS
BEGIN
	
	--==================================================================================================
	-- MAPEO DE PARÁMETROS
	--==================================================================================================
	DECLARE @id_warehouse			INT 			= JSON_VALUE(@ParametrosBusquedaSaldoMastered, '$.id_warehouse')
	DECLARE @id_warehouseLocation	INT 			= JSON_VALUE(@ParametrosBusquedaSaldoMastered, '$.id_warehouseLocation')
	DECLARE @id_warehouseType	INT 				= JSON_VALUE(@ParametrosBusquedaSaldoMastered, '$.id_warehouseType')
	DECLARE @for_lot SMALLINT 						= JSON_VALUE(@ParametrosBusquedaSaldoMastered, '$.for_lot')

	
	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL
	--==================================================================================================
	--CREATE TABLE #TmpIdentificadoresInventoryMoveDetail
	--(
	--	id	int,
	--	id_item int,
	--	id_warehouse int,
	--	id_warehouseLocation int,
	--	id_lot int,
	--	entryAmount decimal(14,6),
	--	exitAmount decimal(14,6),
	--)
	--id = id_productLotMP,
 --                   code = aItem.masterCode,
 --                   aItem.name,
 --                   noLote = aInternalNumber,
 --                   saldoStr = saldo.ToString("0.00"),
 --                   id_item = aItem.id,
 --                   id_lote = aIdLote ?? 0,
 --                   saldo

	IF (@for_lot = 1)
	BEGIN
	--INSERT INTO #TmpIdentificadoresInventoryMoveDetail
	--SELECT A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot, (sum(A.entryAmount) - sum(A.exitAmount)) as saldo
	SELECT '' as id, '' as code, '' as [name], (ISNULL(PL.internalNumber, ISNULL(L.internalNumber, 'Sin Lote'))) as noLote, 
		   '0.00' as saldoStr, 0 as id_item, null as id_size, 0 as id_itemType, null as id_trademark, null as id_presentationMP,
		   ISNULL(A.id_lot, 0) as id_lote, 0.00 as saldo
	FROM dbo.InventoryMoveDetail A WITH (NOLOCK)
	INNER JOIN dbo.InventoryMove B WITH (NOLOCK) ON (B.id = A.id_inventoryMove)
	INNER JOIN dbo.Document C WITH (NOLOCK) ON (C.id = B.id)
	INNER JOIN dbo.DocumentState D WITH (NOLOCK) ON (D.id = C.id_documentState AND D.code = '03')--'03' APROBADA
	INNER JOIN dbo.Item E WITH (NOLOCK) ON (E.id = A.id_item)
	INNER JOIN dbo.InventoryLine F WITH (NOLOCK) ON (F.id = E.id_inventoryLine)
	INNER JOIN dbo.Warehouse W WITH (NOLOCK) ON (W.id = A.id_warehouse)
	LEFT JOIN dbo.Lot L WITH (NOLOCK) ON (L.id = A.id_lot)
	LEFT JOIN dbo.ProductionLot PL WITH (NOLOCK) ON (PL.id = A.id_lot)
	WHERE 
		  (W.id_warehouseType = ISNULL(@id_warehouseType, W.id_warehouseType) OR @id_warehouseType = 0)
		  AND (A.id_warehouse = ISNULL(@id_warehouse, A.id_warehouse) OR @id_warehouse = 0)
		  AND (A.id_warehouseLocation = ISNULL(@id_warehouseLocation, A.id_warehouseLocation) OR @id_warehouseLocation = 0)
		  AND(F.code = 'PP' OR F.code = 'PT')--'PP' Producto en Proceso y 'PT' Producto Terminado
		  group by  A.id_lot, PL.internalNumber, L.internalNumber
		  having (sum(A.entryAmount) - sum(A.exitAmount)) > 0
	END
	ELSE
	BEGIN
	--INSERT INTO #TmpIdentificadoresInventoryMoveDetail
	--SELECT A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot, (sum(A.entryAmount) - sum(A.exitAmount)) as saldo
	SELECT (CAST(A.id_item AS VARCHAR) + ':' + ISNULL(CAST(A.id_lot AS VARCHAR), '')) as id, E.masterCode as code, E.[name] as [name],
		   (ISNULL(PL.internalNumber, ISNULL(L.internalNumber, 'Sin Lote'))) as noLote, 
		   CAST(CAST((sum(A.entryAmount) - sum(A.exitAmount)) AS DECIMAL(10, 2)) AS VARCHAR) as saldoStr, 
		   A.id_item as id_item, IG.id_size as id_size, E.id_itemType as id_itemType, IG.id_trademark as id_trademark,
		   E.id_presentation as id_presentationMP, ISNULL(A.id_lot, 0) as id_lote, (sum(A.entryAmount) - sum(A.exitAmount)) as saldo
	FROM dbo.InventoryMoveDetail A WITH (NOLOCK)
	INNER JOIN dbo.InventoryMove B WITH (NOLOCK) ON (B.id = A.id_inventoryMove)
	INNER JOIN dbo.Document C WITH (NOLOCK) ON (C.id = B.id)
	INNER JOIN dbo.DocumentState D WITH (NOLOCK) ON (D.id = C.id_documentState AND D.code = '03')--'03' APROBADA
	INNER JOIN dbo.Item E WITH (NOLOCK) ON (E.id = A.id_item)
	INNER JOIN dbo.ItemGeneral IG WITH (NOLOCK) ON (IG.id_item = A.id_item)
	INNER JOIN dbo.InventoryLine F WITH (NOLOCK) ON (F.id = E.id_inventoryLine)
	INNER JOIN dbo.Warehouse W WITH (NOLOCK) ON (W.id = A.id_warehouse)
	LEFT JOIN dbo.Lot L WITH (NOLOCK) ON (L.id = A.id_lot)
	LEFT JOIN dbo.ProductionLot PL WITH (NOLOCK) ON (PL.id = A.id_lot)
	WHERE 
		  (W.id_warehouseType = ISNULL(@id_warehouseType, W.id_warehouseType) OR @id_warehouseType = 0)
		  AND (A.id_warehouse = ISNULL(@id_warehouse, A.id_warehouse) OR @id_warehouse = 0)
		  AND (A.id_warehouseLocation = ISNULL(@id_warehouseLocation, A.id_warehouseLocation) OR @id_warehouseLocation = 0)
		  AND(F.code = 'PP' OR F.code = 'PT')--'PP' Producto en Proceso y 'PT' Producto Terminado
		  group by A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot, PL.internalNumber, L.internalNumber, E.masterCode,
				   E.[name], IG.id_size, E.id_itemType, IG.id_trademark, E.id_presentation
		  having (sum(A.entryAmount) - sum(A.exitAmount)) > 0
	END
	
	
END