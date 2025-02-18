
-- ==============================================================================================================
-- Creado por:		Wilber Leonard Benavides
-- Fecha Creación:	23 de Noviembre del 2021
-- Descripción:		Procedimiento almacenado para consultar el Internal Process y Saldo de Inventario.
/*
-- Pruebas:			
					EXEC [dbo].[inv_Consultar_Internal_Process_Saldo_StoredProcedure] NULL, NULL
*/

CREATE PROCEDURE [dbo].[inv_Consultar_Internal_Process_Saldo_StoredProcedure]
	@ParametrosBusquedaInternalProcessSaldo		NVARCHAR(MAX)
	--@Resultado							NVARCHAR(MAX) OUTPUT
AS
BEGIN
	
	--==================================================================================================
	-- MAPEO DE PARÁMETROS
	--==================================================================================================
	--DECLARE @id_documentType			INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_documentType')
	--DECLARE @number						VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.number')
	--DECLARE @reference					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.reference')
	DECLARE @startEmissionDate			DATETIME	    = JSON_VALUE(@ParametrosBusquedaInternalProcessSaldo, '$.startEmissionDate')
	DECLARE @endEmissionDate			DATETIME	    = JSON_VALUE(@ParametrosBusquedaInternalProcessSaldo, '$.endEmissionDate')
	DECLARE @id_warehouse				INT 			= JSON_VALUE(@ParametrosBusquedaInternalProcessSaldo, '$.id_warehouse')
	DECLARE @id_warehouseLocation		INT 			= JSON_VALUE(@ParametrosBusquedaInternalProcessSaldo, '$.id_warehouseLocation')
	--DECLARE @idNatureMove				INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.idNatureMove')
	--DECLARE @id_inventoryReason			INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_inventoryReason')
	--DECLARE @id_warehouseExit			INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseExit')
	--DECLARE @id_warehouseLocationExit	INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseLocationExit')
	--DECLARE @id_dispatcher				INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_dispatcher')
	--DECLARE @id_warehouseEntry			INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseEntry')
	--DECLARE @id_warehouseLocationEntry	INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseLocationEntry')
	--DECLARE @id_receiver				INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_receiver')
	--DECLARE @numberLot					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.numberLot')
	--DECLARE @internalNumberLot			VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.internalNumberLot')
	--DECLARE @items						VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.items')
	DECLARE @id_user					INT 			= JSON_VALUE(@ParametrosBusquedaInternalProcessSaldo, '$.id_user')
	--DECLARE @codeReport					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.codeReport')

	DECLARE @IdMetricUnitLbs int;
	SELECT @IdMetricUnitLbs = A.id FROM dbo.MetricUnit A WITH (NOLOCK)
	WHERE A.code = 'Lbs'

	DECLARE @ConfiguradoWAH int;
	SELECT @ConfiguradoWAH = COUNT(1) FROM dbo.UserEntityDetail A WITH (NOLOCK)
	INNER JOIN dbo.UserEntity B WITH (NOLOCK) ON (B.id = A.id_userEntity) AND (B.id_user = @id_user)
	INNER JOIN dbo.Entity C WITH (NOLOCK) ON (C.id = B.id_entity) AND (C.code = 'WAH')--Entidad de Bodega del Sitema;

	CREATE TABLE #TmpUserEntityDetail
	(
		id_entityValue	INT
	)
	INSERT INTO #TmpUserEntityDetail
	SELECT A.id_entityValue
	FROM dbo.UserEntityDetail A WITH (NOLOCK)
	INNER JOIN dbo.UserEntity B WITH (NOLOCK) ON (B.id = A.id_userEntity) AND (B.id_user = @id_user)
	INNER JOIN dbo.Entity C WITH (NOLOCK) ON (C.id = B.id_entity) AND (C.code = 'WAH')--Entidad de Bodega del Sitema
	INNER JOIN dbo.UserEntityDetailPermission D WITH (NOLOCK) ON (D.id_userEntityDetail = A.id)
	INNER JOIN dbo.Permission E WITH (NOLOCK) ON (E.id = D.id_permission) AND (E.[name] = 'Visualizar')--Permiso para visualizar

	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL
	--==================================================================================================
	--CREATE TABLE #TmpIdentificadoresInventoryMoveDetail
	--(
	--	id	int,
	--	emissionDate datetime,
	--	sequential int,
	--	id_item int,
	--	id_warehouse int,
	--	id_warehouseLocation int,
	--	id_lot int,
	--	entryAmount decimal(14,6),
	--	exitAmount decimal(14,6),
	--	id_inventoryMove int,
	--	previousBalance decimal(14,6),
	--	balance decimal(14,6),
	--	balanceCutting decimal(14,6),
	--	numberRemissionGuide varchar(100),
	--	codeDocumentState varchar(20),
	--	dateCreate datetime
	--)

	--INSERT INTO #TmpIdentificadoresInventoryMoveDetail
	SELECT A.id_warehouse, E.[name] as Bodega, (CAST(FORMAT(C.emissionDate, 'yyyyMM', 'en-US') AS bigint)) as IdMes,(FORMAT( C.emissionDate, 'yyyy-MM', 'en-US')) as Mes, IIF(G.code = '34', '1. Origen', IIF(G.code = '25' OR G.code = '28', '2. Libras Procesadas', IIF(G.code = '23' OR G.code = '26', '3. Libras Terminadas', IIF(G.code = '32', '4. Destino', IIF(H.idNatureMove = 5, '1.5 Otros Ingresos', '4.5 Otros Egresos'))))) as Tipo,
		   SUM((ISNULL(A.entryAmount, 0) - ISNULL(A.exitAmount, 0))*(ISNULL(J.minimum, 1))*(ISNULL(K.factor, 1))) as CantidadLbs
	--A.id, B.sequential, A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot, A.entryAmount, A.exitAmount, A.id_inventoryMove, 0, 0, 0, '', D.code, A.dateCreate
	FROM dbo.InventoryMoveDetail A WITH (NOLOCK)
	INNER JOIN dbo.InventoryMove B WITH (NOLOCK) ON (B.id = A.id_inventoryMove)
	INNER JOIN dbo.Document C WITH (NOLOCK) ON (C.id = B.id)
	INNER JOIN dbo.DocumentState D WITH (NOLOCK) ON (D.id = C.id_documentState)
	INNER JOIN dbo.Warehouse E WITH (NOLOCK) ON (E.id = A.id_warehouse)
	INNER JOIN dbo.WarehouseType F WITH (NOLOCK) ON (F.id = E.id_warehouseType)
	INNER JOIN dbo.DocumentType G WITH (NOLOCK) ON (G.id = C.id_documentType)
	INNER JOIN dbo.InventoryReason H WITH (NOLOCK) ON (H.id = B.id_inventoryReason)
	INNER JOIN dbo.Item I WITH (NOLOCK) ON (I.id = A.id_item)
	LEFT JOIN dbo.Presentation J WITH (NOLOCK) ON (J.id = I.id_presentation)
	LEFT JOIN dbo.MetricUnitConversion K WITH (NOLOCK) ON (K.id_metricOrigin = ISNULL(J.id_metricUnit, A.id_metricUnit) AND 
														   --K.id_metricDestiny = 4)
														   K.id_metricDestiny = @IdMetricUnitLbs)
	--LEFT JOIN dbo.InventoryExitMove E WITH (NOLOCK) ON (E.id = A.id_inventoryMove)
	--LEFT JOIN dbo.InventoryEntryMove F WITH (NOLOCK) ON (F.id = A.id_inventoryMove)
	--LEFT JOIN dbo.Lot G WITH (NOLOCK) ON (G.id = A.id_lot)
	--LEFT JOIN dbo.ProductionLot H WITH (NOLOCK) ON (H.id = G.id)
	WHERE (D.code = ('03'))--'03' APROBADA
		   AND F.code = 'BPIN' 
		   AND (((C.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (C.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
		   AND (A.id_warehouse = ISNULL(@id_warehouse, A.id_warehouse) OR @id_warehouse = 0)
		   AND (A.id_warehouseLocation = ISNULL(@id_warehouseLocation, A.id_warehouseLocation) OR @id_warehouseLocation = 0)
		   AND (@ConfiguradoWAH = 0 OR (A.id_warehouse IN (
									SELECT AA.id_entityValue
									FROM #TmpUserEntityDetail AA WITH (NOLOCK)
								)))--Entidad de Bodega del Sitema
		   GROUP BY A.id_warehouse, E.[name], IIF(G.code = '34', '1. Origen', IIF(G.code = '25' OR G.code = '28', '2. Libras Procesadas', IIF(G.code = '23' OR G.code = '26', '3. Libras Terminadas', IIF(G.code = '32', '4. Destino', IIF(H.idNatureMove = 5, '1.5 Otros Ingresos', '4.5 Otros Egresos'))))), (FORMAT( C.emissionDate, 'yyyy-MM', 'en-US')),
		            CAST(FORMAT(C.emissionDate, 'yyyyMM', 'en-US') AS BIGINT)
		  --(C.id_documentType = ISNULL(@id_documentType, C.id_documentType) OR @id_documentType = 0)
		  --AND (CAST(C.number AS VARCHAR(20)) LIKE '%' + @number + '%' OR @number = 'Todos')
		  --AND (CAST(C.reference AS VARCHAR(20)) LIKE '%' + @reference + '%' OR @reference = 'Todas')

		  --AND (((C.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (C.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))

		  --AND (B.idNatureMove = ISNULL(@idNatureMove, B.idNatureMove) OR @idNatureMove = 0)
		  --AND (B.id_inventoryReason = ISNULL(@id_inventoryReason, B.id_inventoryReason) OR @id_inventoryReason = 0)

		  --AND (A.id_warehouse = ISNULL(@id_warehouse, A.id_warehouse) OR @id_warehouse = 0)
		  --AND (A.id_warehouseLocation = ISNULL(@id_warehouseLocation, A.id_warehouseLocation) OR @id_warehouseLocation = 0)

		  --AND (E.id_dispatcher = ISNULL(@id_dispatcher, E.id_dispatcher) OR @id_dispatcher = 0)
		  --AND (A.id_warehouse = ISNULL(@id_warehouseEntry, A.id_warehouse) OR @id_warehouseEntry = 0)
		  --AND (A.id_warehouseLocation = ISNULL(@id_warehouseLocationEntry, A.id_warehouseLocation) OR @id_warehouseLocationEntry = 0)
		  --AND (F.id_receiver = ISNULL(@id_receiver, F.id_receiver) OR @id_receiver = 0)
		  --AND (CAST(G.number AS VARCHAR(20)) LIKE '%' + @numberLot + '%' OR @numberLot = 'Todos')
		  --AND (CAST(H.internalNumber AS VARCHAR(20)) LIKE '%' + @internalNumberLot + '%' OR @internalNumberLot = 'Todos')
		  --AND (A.id_item IN (SELECT CAST(VALUE AS INT) FROM STRING_SPLIT(IIF(@items = 'Todos', '', @items), ',')) OR ISNULL(@items, 'Todos') = 'Todos')
		  
		  

				--select * from MetricUnit
				--select * from presentation
				--select * from InventoryReason
--select * from DocumentType
--where [name] = 'Egreso Por Transferencia'
--select * from WarehouseType
--select * from Warehouse
--where id_warehouseType = 16

	
END