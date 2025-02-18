-- ==============================================================================================================
-- Creado por:		Wilber Leonard Benavides
-- Fecha Creación:	08 de Septiembre del 2021
-- Descripción:		Procedimiento almacenado para consultar el Saldo de Inventario en la opción de Tumbado de Placa(OpeningClosingPlateLying).
/*
-- Pruebas:			
					EXEC	@return_value = [dbo].[inv_Consultar_OpeningClosingPlateLying_StoredProcedure]
		@ParametrosBusquedaOpeningClosingPlateLying = '{"id_warehouse":21,"id_openingClosingPlateLyingDto":0,"id_warehouseType":4,"for_lot":0}'
*/

CREATE PROCEDURE [dbo].[inv_Consultar_OpeningClosingPlateLying_StoredProcedure]
	@ParametrosBusquedaOpeningClosingPlateLying		NVARCHAR(MAX)
	--@Resultado							NVARCHAR(MAX) OUTPUT
AS
BEGIN
	
	--==================================================================================================
	-- MAPEO DE PARÁMETROS
	--==================================================================================================
	DECLARE @id_warehouse			INT 			= JSON_VALUE(@ParametrosBusquedaOpeningClosingPlateLying, '$.id_warehouse')
	DECLARE @id_openingClosingPlateLyingDto	INT		= JSON_VALUE(@ParametrosBusquedaOpeningClosingPlateLying, '$.id_openingClosingPlateLyingDto')
	DECLARE @id_warehouseType	INT					= JSON_VALUE(@ParametrosBusquedaOpeningClosingPlateLying, '$.id_warehouseType')
	DECLARE @for_lot SMALLINT 						= JSON_VALUE(@ParametrosBusquedaOpeningClosingPlateLying, '$.for_lot')
	
	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL
	--==================================================================================================
	CREATE TABLE #TmpIdentificadoresInventoryMoveDetail
	(
		id_lot int,
		id_item int,
		id_warehouse int,
		id_warehouseLocation int,
		id_productionCart int,
		amount decimal(14,6)
	)

	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL
	--==================================================================================================
	CREATE TABLE #TmpOpeningClosingPlateLyingDetail
	(
		id_lot int,
		id_item int,
		id_warehouse int,
		id_warehouseLocation int,
		id_productionCart int,
	)
	IF (@for_lot = 1)
	BEGIN
	INSERT INTO #TmpIdentificadoresInventoryMoveDetail
	SELECT A.id_lot, A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_productionCart, 0.00 as amount
	FROM dbo.InventoryMoveDetail A WITH (NOLOCK)
	INNER JOIN dbo.InventoryMove B WITH (NOLOCK) ON (B.id = A.id_inventoryMove)
	INNER JOIN dbo.Document C WITH (NOLOCK) ON (C.id = B.id)
	INNER JOIN dbo.DocumentState D WITH (NOLOCK) ON (D.id = C.id_documentState AND D.code = '03')--'03' APROBADA
	INNER JOIN dbo.Warehouse W WITH (NOLOCK) ON (W.id = A.id_warehouse)
	WHERE (W.id_warehouseType = ISNULL(@id_warehouseType, W.id_warehouseType) OR @id_warehouseType = 0)
		  AND (A.id_warehouse = ISNULL(@id_warehouse, A.id_warehouse) OR @id_warehouse = 0)
		  GROUP BY  A.id_item, A.id_warehouseLocation, A.id_lot, A.id_productionCart, A.id_warehouse
		  having (sum(A.entryAmount) - sum(A.exitAmount)) > 0


	INSERT INTO #TmpOpeningClosingPlateLyingDetail
	SELECT OCPLD.id_lot, OCPLD.id_item, OCPLD.id_warehouse, OCPLD.id_warehouseLocation, OCPLD.id_productionCart
	FROM OpeningClosingPlateLyingDetail OCPLD
	INNER JOIN dbo.OpeningClosingPlateLying OCPL WITH (NOLOCK) ON (OCPL.id = OCPLD.id_openingClosingPlateLying)
	INNER JOIN dbo.Document DOCPLD WITH (NOLOCK) ON (DOCPLD.id = OCPL.id)
	INNER JOIN dbo.DocumentState DSOCPLD WITH (NOLOCK) ON (DSOCPLD.id = DOCPLD.id_documentState AND (/*DSOCPLD.code = '03' OR*/ DSOCPLD.code = '01'))--'03' APROBADA --'01' PENDIENTE
	WHERE OCPLD.id_openingClosingPlateLying != @id_openingClosingPlateLyingDto
	
	DELETE FROM #TmpIdentificadoresInventoryMoveDetail  
	FROM #TmpIdentificadoresInventoryMoveDetail A WITH (NOLOCK) 
	INNER JOIN #TmpOpeningClosingPlateLyingDetail DOCPLD WITH (NOLOCK) 
	ON (DOCPLD.id_lot = A.id_lot and DOCPLD.id_item = A.id_item and DOCPLD.id_warehouse = A.id_warehouse and DOCPLD.id_warehouseLocation = A.id_warehouseLocation and
	    DOCPLD.id_productionCart = A.id_productionCart)

	SELECT 0 as id, A.id_lot, PL.number as noSecTransLote, PL.internalNumber as noLote, 0 as id_item, '' as name_item,
	   0 as id_warehouse, '' as warehouse, 0 as id_warehouseLocation, '' as warehouseLocation, NULL as id_costCenterExit,
	   NULL as id_subCostCenterExit, NULL as id_productionCart, '' as productionCart, 0.00 as amount,
	   0 as id_metricUnit, '' as metricUnit, NULL as id_boxedWarehouse, '' as boxedWarehouse,
	   NULL as id_boxedWarehouseLocation, '' as boxedWarehouseLocation, NULL as id_costCenter, NULL as id_subCostCenter, 
	   '' as numberInventoryExit, '' as numberInventoryEntry
	FROM #TmpIdentificadoresInventoryMoveDetail A WITH (NOLOCK)
	LEFT JOIN dbo.ProductionLot PL WITH (NOLOCK) ON (PL.id = A.id_lot)
	GROUP BY  A.id_lot, PL.number, PL.internalNumber

	END
	ELSE
	BEGIN
	

	INSERT INTO #TmpIdentificadoresInventoryMoveDetail
	SELECT A.id_lot, A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_productionCart, (sum(A.entryAmount) - sum(A.exitAmount)) as amount	   
	FROM dbo.InventoryMoveDetail A WITH (NOLOCK)
	INNER JOIN dbo.InventoryMove B WITH (NOLOCK) ON (B.id = A.id_inventoryMove)
	INNER JOIN dbo.Document C WITH (NOLOCK) ON (C.id = B.id)
	INNER JOIN dbo.DocumentState D WITH (NOLOCK) ON (D.id = C.id_documentState AND D.code = '03')--'03' APROBADA
	INNER JOIN dbo.Warehouse W WITH (NOLOCK) ON (W.id = A.id_warehouse)
	WHERE (W.id_warehouseType = ISNULL(@id_warehouseType, W.id_warehouseType) OR @id_warehouseType = 0)
		  AND (A.id_warehouse = ISNULL(@id_warehouse, A.id_warehouse) OR @id_warehouse = 0)
		  GROUP BY  A.id_item, A.id_warehouseLocation, A.id_lot, A.id_productionCart, A.id_warehouse
		  having (sum(A.entryAmount) - sum(A.exitAmount)) > 0
	
	INSERT INTO #TmpOpeningClosingPlateLyingDetail
	SELECT OCPLD.id_lot, OCPLD.id_item, OCPLD.id_warehouse, OCPLD.id_warehouseLocation, OCPLD.id_productionCart
	FROM OpeningClosingPlateLyingDetail OCPLD
	INNER JOIN dbo.OpeningClosingPlateLying OCPL WITH (NOLOCK) ON (OCPL.id = OCPLD.id_openingClosingPlateLying)
	INNER JOIN dbo.Document DOCPLD WITH (NOLOCK) ON (DOCPLD.id = OCPL.id)
	INNER JOIN dbo.DocumentState DSOCPLD WITH (NOLOCK) ON (DSOCPLD.id = DOCPLD.id_documentState AND (/*DSOCPLD.code = '03' OR*/ DSOCPLD.code = '01'))--'03' APROBADA --'01' PENDIENTE
	WHERE OCPLD.id_openingClosingPlateLying != @id_openingClosingPlateLyingDto
	
	DELETE FROM #TmpIdentificadoresInventoryMoveDetail  
	FROM #TmpIdentificadoresInventoryMoveDetail A WITH (NOLOCK) 
	INNER JOIN #TmpOpeningClosingPlateLyingDetail DOCPLD WITH (NOLOCK) 
	ON (DOCPLD.id_lot = A.id_lot and DOCPLD.id_item = A.id_item and DOCPLD.id_warehouse = A.id_warehouse and DOCPLD.id_warehouseLocation = A.id_warehouseLocation and
	    DOCPLD.id_productionCart = A.id_productionCart)

	SELECT 0 as id, A.id_lot, PL.number as noSecTransLote, PL.internalNumber as noLote, A.id_item, E.[name] as name_item,
	   A.id_warehouse, W.[name] as warehouse, A.id_warehouseLocation, WL.[name] as warehouseLocation, NULL as id_costCenterExit,
	   NULL as id_subCostCenterExit, A.id_productionCart, PC.[name] as productionCart, A.amount,
	   F.id_metricUnitInventory as id_metricUnit, MU.code as metricUnit, NULL as id_boxedWarehouse, '' as boxedWarehouse,
	   NULL as id_boxedWarehouseLocation, '' as boxedWarehouseLocation, NULL as id_costCenter, NULL as id_subCostCenter, 
	   '' as numberInventoryExit, '' as numberInventoryEntry
	FROM #TmpIdentificadoresInventoryMoveDetail A WITH (NOLOCK)
	INNER JOIN dbo.Item E WITH (NOLOCK) ON (E.id = A.id_item)
	INNER JOIN dbo.ItemInventory F WITH (NOLOCK) ON (F.id_item = A.id_item)
	INNER JOIN dbo.MetricUnit MU WITH (NOLOCK) ON (MU.id = F.id_metricUnitInventory)
	INNER JOIN dbo.Warehouse W WITH (NOLOCK) ON (W.id = A.id_warehouse)
	INNER JOIN dbo.WarehouseLocation WL WITH (NOLOCK) ON (WL.id = A.id_warehouseLocation)
	LEFT JOIN dbo.ProductionCart PC WITH (NOLOCK) ON (PC.id = A.id_productionCart)
	LEFT JOIN dbo.ProductionLot PL WITH (NOLOCK) ON (PL.id = A.id_lot)
	END
END