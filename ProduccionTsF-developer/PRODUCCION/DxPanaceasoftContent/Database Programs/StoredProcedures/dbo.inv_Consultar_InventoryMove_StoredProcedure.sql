
-- ==============================================================================================================
-- Creado por:		Wilber Leonard Benavides
-- Fecha Creación:	21 de Junio del 2019
-- Descripción:		Procedimiento almacenado para consultar los Movimientos de Inventario.
/*
-- Pruebas:			
					EXEC [dbo].[inv_Consultar_InventoryMove_StoredProcedure] NULL, NULL
*/

CREATE PROCEDURE [dbo].[inv_Consultar_InventoryMove_StoredProcedure]
	@ParametrosBusquedaInventoryMove		NVARCHAR(MAX)
AS
BEGIN
	
	--==================================================================================================
	-- MAPEO DE PARÁMETROS
	--==================================================================================================
	DECLARE @id_documentType			INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_documentType')
	DECLARE @id_documentState			INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_documentState')
	DECLARE @codeInventoryReasonIPXM    VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.codeInventoryReasonIPXM')
	DECLARE @number						VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.number')
	DECLARE @reference					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.reference')
	DECLARE @startEmissionDate			DATETIME	    = JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.startEmissionDate')
	DECLARE @endEmissionDate			DATETIME	    = JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.endEmissionDate')
	DECLARE @startAuthorizationDate		DATETIME	    = JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.startAuthorizationDate')
	DECLARE @endAuthorizationDate		DATETIME	    = JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.endAuthorizationDate')
	DECLARE @accessKey					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.accessKey')
	DECLARE @authorizationNumber		VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.authorizationNumber')
	DECLARE @id_receiver				INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_receiver')
	DECLARE @id_dispatcher				INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_dispatcher')
	DECLARE @id_inventoryReason			INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_inventoryReason')
	DECLARE @id_warehouseEntry			INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_warehouseEntry')
	DECLARE @id_warehouseLocationEntry	INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_warehouseLocationEntry')
	DECLARE @id_warehouseExit			INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_warehouseExit')
	DECLARE @id_warehouseLocationExit	INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_warehouseLocationExit')
	--DECLARE @idNatureMove				INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.idNatureMove')
	DECLARE @id_user					INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMove, '$.id_user')

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
	CREATE TABLE #TmpIdentificadoresInventoryMove
	(
		id	int
	)

	INSERT INTO #TmpIdentificadoresInventoryMove
	SELECT A.id_inventoryMove As id
	FROM dbo.InventoryMoveDetail A WITH (NOLOCK)
	INNER JOIN dbo.InventoryMove B WITH (NOLOCK) ON (B.id = A.id_inventoryMove)
	INNER JOIN dbo.Document C WITH (NOLOCK) ON (C.id = B.id)
	INNER JOIN dbo.DocumentState D WITH (NOLOCK) ON (D.id = C.id_documentState)
	LEFT JOIN dbo.InventoryExitMove E WITH (NOLOCK) ON (E.id = A.id_inventoryMove)
	LEFT JOIN dbo.InventoryEntryMove F WITH (NOLOCK) ON (F.id = A.id_inventoryMove)
	INNER JOIN dbo.InventoryReason G WITH (NOLOCK) ON (G.id = B.id_inventoryReason)
	WHERE (G.code = IIF((@codeInventoryReasonIPXM = 'Todos'), (G.code), (@codeInventoryReasonIPXM)))
		  AND (C.id_documentType = ISNULL(@id_documentType, C.id_documentType) OR @id_documentType = 0)
		  AND (C.id_documentState = ISNULL(@id_documentState, C.id_documentState) OR @id_documentState = 0)
		  AND (CAST(C.number AS VARCHAR(20)) LIKE '%' + @number + '%' OR @number = 'Todos')
		  AND (CAST(C.reference AS VARCHAR(20)) LIKE '%' + @reference + '%' OR @reference = 'Todas')
		  AND (((C.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (C.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
		  AND (((C.authorizationDate >= @startAuthorizationDate OR @startAuthorizationDate IS NULL) AND (C.authorizationDate < DATEADD(day, 1, @endAuthorizationDate) OR @endAuthorizationDate IS NULL)))
		  AND (CAST(C.accessKey AS VARCHAR(20)) LIKE '%' + @accessKey + '%' OR @accessKey = 'Todos')
		  AND (CAST(C.authorizationNumber AS VARCHAR(20)) LIKE '%' + @authorizationNumber + '%' OR @authorizationNumber = 'Todos')
		  AND (F.id_receiver = ISNULL(@id_receiver, F.id_receiver) OR @id_receiver = 0)
		  AND (E.id_dispatcher = ISNULL(@id_dispatcher, E.id_dispatcher) OR @id_dispatcher = 0)
		  AND (B.id_inventoryReason = ISNULL(@id_inventoryReason, B.id_inventoryReason) OR @id_inventoryReason = 0)
		  AND (B.idWarehouse = ISNULL(@id_warehouseExit, B.idWarehouse) OR @id_warehouseExit = 0)
		  AND (A.id_warehouseLocation = ISNULL(@id_warehouseLocationExit, A.id_warehouseLocation) OR @id_warehouseLocationExit = 0)
		  AND (B.idWarehouse = ISNULL(@id_warehouseEntry, B.idWarehouse) OR @id_warehouseEntry = 0)
		  AND (A.id_warehouseLocation = ISNULL(@id_warehouseLocationEntry, A.id_warehouseLocation) OR @id_warehouseLocationEntry = 0)
		  AND (@ConfiguradoWAH = 0 OR (B.idWarehouse IN (
									SELECT AA.id_entityValue
									FROM #TmpUserEntityDetail AA WITH (NOLOCK)
								)))--Entidad de Bodega del Sitema
		GROUP BY A.id_inventoryMove;

		--select * from #TmpIdentificadoresInventoryMove;

	SELECT A.id
		  ,A.natureSequential
		  ,A.idWarehouse
		  ,F.[name] nameWarehouse
		  ,C.emissionDate
		  ,A.id_inventoryReason
		  ,E.[name] nameInventoryReason
		  ,I.id_receiver
		  ,J.fullname_businessName nameReceiver
		  ,G.id_dispatcher
		  ,H.fullname_businessName nameDispatcher
		  ,C.id_documentState
		  ,D.[name] nameDocumentState
		FROM [dbo].[InventoryMove] A WITH (NOLOCK)
		INNER JOIN #TmpIdentificadoresInventoryMove B ON (B.id = A.id)
		INNER JOIN dbo.Document C WITH (NOLOCK) ON (C.id = B.id)
		INNER JOIN dbo.DocumentState D WITH (NOLOCK) ON (D.id = C.id_documentState)
		LEFT JOIN dbo.InventoryReason E WITH (NOLOCK) ON (E.id = A.id_inventoryReason)
		INNER JOIN dbo.Warehouse F WITH (NOLOCK) ON (F.id = A.idWarehouse)
		LEFT JOIN dbo.InventoryExitMove G WITH (NOLOCK) ON (G.id = A.id)
		LEFT JOIN dbo.Person H WITH (NOLOCK) ON (H.id = G.id_dispatcher)
		LEFT JOIN dbo.InventoryEntryMove I WITH (NOLOCK) ON (I.id = A.id)
		LEFT JOIN dbo.Person J WITH (NOLOCK) ON (J.id = I.id_receiver)
		ORDER BY C.emissionDate, A.id DESC

END