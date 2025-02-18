
-- ==============================================================================================================
-- Creado por:		Wilber Leonard Benavides
-- Fecha Creación:	22 de Mayo del 2019
-- Descripción:		Procedimiento almacenado para consultar el Kardex y Saldo de Inventario.
/*
-- Pruebas:			
					EXEC [dbo].[inv_Consultar_Kardex_Saldo.StoredProcedure] NULL, NULL
*/

CREATE PROCEDURE [dbo].[inv_Consultar_Kardex_Saldo_StoredProcedure]
	@ParametrosBusquedaKardexSaldo		NVARCHAR(MAX)
	--@Resultado							NVARCHAR(MAX) OUTPUT
AS
BEGIN
	
	--==================================================================================================
	-- MAPEO DE PARÁMETROS
	--==================================================================================================
	DECLARE @id_documentType			INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_documentType')
	DECLARE @number						VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.number')
	DECLARE @reference					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.reference')
	DECLARE @startEmissionDate			DATETIME	    = JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.startEmissionDate')
	DECLARE @endEmissionDate			DATETIME	    = JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.endEmissionDate')
	DECLARE @idNatureMove				INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.idNatureMove')
	DECLARE @id_inventoryReason			INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_inventoryReason')
	DECLARE @id_warehouseExit			INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseExit')
	DECLARE @id_warehouseLocationExit	INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseLocationExit')
	DECLARE @id_dispatcher				INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_dispatcher')
	DECLARE @id_warehouseEntry			INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseEntry')
	DECLARE @id_warehouseLocationEntry	INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseLocationEntry')
	DECLARE @id_receiver				INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_receiver')
	DECLARE @numberLot					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.numberLot')
	DECLARE @internalNumberLot			VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.internalNumberLot')
	DECLARE @items						VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.items')
	DECLARE @id_user					INT 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_user')
	DECLARE @codeReport					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.codeReport')

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
	CREATE TABLE #TmpIdentificadoresInventoryMoveDetail
	(
		id	int,
		emissionDate datetime,
		sequential int,
		id_item int,
		id_warehouse int,
		id_warehouseLocation int,
		id_lot int,
		entryAmount decimal(14,6),
		exitAmount decimal(14,6),
		id_inventoryMove int,
		previousBalance decimal(14,6),
		balance decimal(14,6),
		balanceCutting decimal(14,6),
		numberRemissionGuide varchar(100),
		codeDocumentState varchar(20),
		dateCreate datetime,
		id_metricUnit int
	)

	INSERT INTO #TmpIdentificadoresInventoryMoveDetail
	SELECT A.id, C.emissionDate, B.sequential, A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot, A.entryAmount, A.exitAmount, A.id_inventoryMove, 0, 0, 0, '', D.code, A.dateCreate, A.id_metricUnit
	FROM dbo.InventoryMoveDetail A WITH (NOLOCK)
	INNER JOIN dbo.InventoryMove B WITH (NOLOCK) ON (B.id = A.id_inventoryMove)
	INNER JOIN dbo.Document C WITH (NOLOCK) ON (C.id = B.id)
	INNER JOIN dbo.DocumentState D WITH (NOLOCK) ON (D.id = C.id_documentState)
	LEFT JOIN dbo.InventoryExitMove E WITH (NOLOCK) ON (E.id = A.id_inventoryMove)
	LEFT JOIN dbo.InventoryEntryMove F WITH (NOLOCK) ON (F.id = A.id_inventoryMove)
	LEFT JOIN dbo.Lot G WITH (NOLOCK) ON (G.id = A.id_lot)
	LEFT JOIN dbo.ProductionLot H WITH (NOLOCK) ON (H.id = G.id)
	WHERE --(D.code = IIF((@codeReport = 'IMIPV1'), (D.code), ('03')))--'03' APROBADA
		  (C.id_documentType = ISNULL(@id_documentType, C.id_documentType) OR @id_documentType = 0)
		  AND (CAST(C.number AS VARCHAR(20)) LIKE '%' + @number + '%' OR @number = 'Todos')
		  AND (CAST(C.reference AS VARCHAR(20)) LIKE '%' + @reference + '%' OR @reference = 'Todas')
		  --AND (((C.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (C.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
		  AND (B.idNatureMove = ISNULL(@idNatureMove, B.idNatureMove) OR @idNatureMove = 0)
		  AND (B.id_inventoryReason = ISNULL(@id_inventoryReason, B.id_inventoryReason) OR @id_inventoryReason = 0)
		  AND (A.id_warehouse = ISNULL(@id_warehouseExit, A.id_warehouse) OR @id_warehouseExit = 0)
		  AND (A.id_warehouseLocation = ISNULL(@id_warehouseLocationExit, A.id_warehouseLocation) OR @id_warehouseLocationExit = 0)
		  AND (E.id_dispatcher = ISNULL(@id_dispatcher, E.id_dispatcher) OR @id_dispatcher = 0)
		  AND (A.id_warehouse = ISNULL(@id_warehouseEntry, A.id_warehouse) OR @id_warehouseEntry = 0)
		  AND (A.id_warehouseLocation = ISNULL(@id_warehouseLocationEntry, A.id_warehouseLocation) OR @id_warehouseLocationEntry = 0)
		  AND (F.id_receiver = ISNULL(@id_receiver, F.id_receiver) OR @id_receiver = 0)
		  AND (CAST(G.number AS VARCHAR(20)) LIKE '%' + @numberLot + '%' OR @numberLot = 'Todos')
		  AND (CAST(H.internalNumber AS VARCHAR(20)) LIKE '%' + @internalNumberLot + '%' OR @internalNumberLot = 'Todos')
		  AND (A.id_item IN (SELECT CAST(VALUE AS INT) FROM STRING_SPLIT(IIF(@items = 'Todos', '', @items), ',')) OR ISNULL(@items, 'Todos') = 'Todos')
		  AND (@ConfiguradoWAH = 0 OR (A.id_warehouse IN (
									SELECT AA.id_entityValue
									FROM #TmpUserEntityDetail AA WITH (NOLOCK)
								)))--Entidad de Bodega del Sitema

	--select *
	--from #TmpIdentificadoresInventoryMoveDetail
	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL
	--==================================================================================================
	CREATE TABLE #TmpIdentificadoresInventoryMoveDetailReduced
	(
		id	int,
		emissionDate datetime,
		sequential int,
		id_item int,
		id_warehouse int,
		id_warehouseLocation int,
		id_lot int,
		entryAmount decimal(14,6),
		exitAmount decimal(14,6),
		id_inventoryMove int,
		previousBalance decimal(14,6),
		balance decimal(14,6),
		balanceCutting decimal(14,6),
		numberRemissionGuide varchar(100),
		codeDocumentState varchar(20),
		dateCreate datetime,
		id_metricUnit int
	)

	INSERT INTO #TmpIdentificadoresInventoryMoveDetailReduced
	SELECT A.*
	FROM #TmpIdentificadoresInventoryMoveDetail A
	WHERE (((A.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (A.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
	
	--select *
	--from #TmpIdentificadoresInventoryMoveDetailReduced
	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL #TmpInventoryMoveDetailWithPreviousBalance
	--==================================================================================================
	CREATE TABLE #TmpInventoryMoveDetailWithPreviousBalance
	(
		id	int,
		previousBalance decimal(14,6),
		previousBalanceCutting decimal(14,6),
		id_item int,
		id_warehouse int,
		id_warehouseLocation int,
		id_lot int,
		emissionDate datetime,
		dateCreate datetime
	)

	INSERT INTO #TmpInventoryMoveDetailWithPreviousBalance
	SELECT A.id, SUM(ISNULL(AA.entryAmount, 0) - ISNULL(AA.exitAmount, 0)) as previoBalance, NULL, NULL, NULL, NULL, NULL, NULL, NULL
	FROM #TmpIdentificadoresInventoryMoveDetailReduced A
	INNER JOIN #TmpIdentificadoresInventoryMoveDetail AA WITH (NOLOCK) ON (AA.codeDocumentState = '03') 
	WHERE (A.codeDocumentState = '03')--'03' APROBADA 
	AND (AA.id_item = A.id_item AND AA.id_warehouse = A.id_warehouse AND AA.id_warehouseLocation = A.id_warehouseLocation AND (ISNULL(AA.id_lot, 0) = ISNULL(A.id_lot, 0))) 
	AND (AA.emissionDate < A.emissionDate OR (AA.emissionDate = A.emissionDate AND (AA.dateCreate < A.dateCreate)))--(ISNULL(AA.sequential, 0) < ISNULL(A.sequential, 0))))
	--AND (((A.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (A.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
	GROUP BY A.id;

	IF(@startEmissionDate IS NOT NULL)
	BEGIN
		UPDATE A
		SET A.id_item = AA.id_item,
			A.id_warehouse = AA.id_warehouse,
			A.id_warehouseLocation = AA.id_warehouseLocation,
			A.id_lot = AA.id_lot,
			A.emissionDate = AA.emissionDate,
			A.dateCreate = AA.dateCreate,
			A.previousBalanceCutting = A.previousBalance
		FROM #TmpInventoryMoveDetailWithPreviousBalance AS A
		INNER JOIN #TmpIdentificadoresInventoryMoveDetailReduced AA WITH (NOLOCK) ON (AA.id = A.id)

		UPDATE A
		SET A.previousBalanceCutting = 0
		FROM #TmpInventoryMoveDetailWithPreviousBalance AS A
		INNER JOIN #TmpIdentificadoresInventoryMoveDetailReduced AA WITH (NOLOCK) ON (AA.codeDocumentState = '03')
		WHERE (AA.id_item = A.id_item AND AA.id_warehouse = A.id_warehouse AND AA.id_warehouseLocation = A.id_warehouseLocation AND (ISNULL(AA.id_lot, 0) = ISNULL(A.id_lot, 0))) 
		AND (AA.emissionDate < A.emissionDate OR (AA.emissionDate = A.emissionDate AND (AA.dateCreate < A.dateCreate)))--(ISNULL(AA.sequential, 0) < ISNULL(A.sequential, 0))))
		--select *
		--from #TmpInventoryMoveDetailWithPreviousBalance
		--where ISNULL(id_lot, 0) != 0 
		--order by id_warehouseLocation, id_lot

		--==================================================================================================
		-- CREACIÓN DE TABLA TEMPORAL #TmpInventoryMoveDetailToNoShow
		--==================================================================================================
		CREATE TABLE #TmpInventoryMoveDetailToNoShow
		(
			id	int,
			id_item int,
			id_warehouse int,
			id_warehouseLocation int,
			id_lot int,
			entryAmount decimal(14,6),
			exitAmount decimal(14,6)
		)

		INSERT INTO #TmpInventoryMoveDetailToNoShow
		SELECT A.id, A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot, A.entryAmount, A.exitAmount 
		FROM #TmpIdentificadoresInventoryMoveDetail A
		WHERE (A.codeDocumentState = '03') AND (A.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)

		DELETE A
		FROM #TmpInventoryMoveDetailToNoShow A WITH (NOLOCK)
		INNER JOIN #TmpIdentificadoresInventoryMoveDetailReduced AA WITH (NOLOCK) ON (AA.codeDocumentState = '03') 
		WHERE (AA.id_item = A.id_item AND AA.id_warehouse = A.id_warehouse AND AA.id_warehouseLocation = A.id_warehouseLocation AND (ISNULL(AA.id_lot, 0) = ISNULL(A.id_lot, 0)))
		
		--select *
		--from #TmpInventoryMoveDetailToNoShow
		--==================================================================================================
		-- CREACIÓN DE TABLA TEMPORAL #TmpInventoryMoveDetailNoShow
		--==================================================================================================
		CREATE TABLE #TmpInventoryMoveDetailNoShow
		(
			id	int,
			previousBalance decimal(14,6),
			id_item int,
			id_warehouse int,
			id_warehouseLocation int,
			id_lot int
		)

		INSERT INTO #TmpInventoryMoveDetailNoShow
		SELECT Max(A.id), SUM(ISNULL(A.entryAmount, 0) - ISNULL(A.exitAmount, 0)) as previoBalance, A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot
		FROM #TmpInventoryMoveDetailToNoShow A
		GROUP BY A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot;

		--select *
		--from #TmpInventoryMoveDetailNoShow

	END

	--Actualizar previousBalance y balanceCutting
	UPDATE A
	SET A.previousBalance = ISNULL(AA.previousBalance, 0),
		A.balanceCutting = ISNULL(AA.previousBalanceCutting, 0) + (A.entryAmount - A.exitAmount),
		A.balance = ISNULL(AA.previousBalance, 0) + (A.entryAmount - A.exitAmount)
	FROM #TmpIdentificadoresInventoryMoveDetailReduced AS A
	LEFT JOIN #TmpInventoryMoveDetailWithPreviousBalance AA WITH (NOLOCK) ON (AA.id = A.id)
	
	--select 
	--SUM(AA.balanceCutting) as balanceCutting
	----SUM(AA.entryAmount - AA.exitAmount) as previoBalance2
	--from #TmpIdentificadoresInventoryMoveDetailReduced AA
	--where (AA.codeDocumentState = '03') AND ISNULL(AA.id_lot, 0) = 0 AND AA.id_item = 1349 AND AA.id_warehouse = 12 AND AA.id_warehouseLocation = 14
	--select *
	--from #TmpIdentificadoresInventoryMoveDetail

	--Actualizar numberRemissionGuide
	UPDATE A
	SET A.numberRemissionGuide = CAST(AA.sequential AS varchar(100)) + ' - '  + convert(varchar(10),AA.emissionDate,103)
	FROM #TmpIdentificadoresInventoryMoveDetailReduced AS A
	INNER JOIN dbo.DocumentSource AB WITH (NOLOCK) ON (AB.id_document = A.id_inventoryMove)
	INNER JOIN dbo.Document AA WITH (NOLOCK) ON (AB.id_documentOrigin = AA.id)
	INNER JOIN dbo.DocumentType AC WITH (NOLOCK) ON (AC.id = AA.id_documentType) AND (AC.code = '08')--Código de tipo de documento de Guía de Remisión

	--select *
	--from #TmpInventoryMoveDetailWithPreviousBalance

	--select *
	--from #TmpIdentificadoresInventoryMoveDetailReduced B 
	--Where (B.codeDocumentState = IIF((@codeReport = 'IMIPV1'), (B.codeDocumentState), ('03')))--'03' APROBADA

	

	IF(@startEmissionDate IS NOT NULL)
	BEGIN
	--==================================================================================================
		-- CREACIÓN DE TABLA TEMPORAL #TmpInventoryMoveDetailNoShow
		--==================================================================================================
		CREATE TABLE #TmpInventoryMoveDetailRespuesta
		(
			id	int,
			id_document	int,
			document	varchar(100),
			nameDocumentState	varchar(100),
			id_documentType	int,
			documentType	varchar(100),
			id_inventoryReason	int,
			inventoryReason	varchar(100),
			emissionDate datetime,
			id_item int,
			code_item	varchar(100),
			id_metricUnit int,
			metricUnit	varchar(100),
			id_lot int,
			number varchar(100),
			internalNumber varchar(100),
			id_warehouse int,
			warehouse varchar(100),
			id_warehouseLocation int,
			warehouseLocation varchar(100),
			id_warehouseExit int,
			warehouseExit varchar(100),
			id_warehouseLocationExit int,
			warehouseLocationExit varchar(100),
			id_warehouseEntry int,
			warehouseEntry varchar(100),
			id_warehouseLocationEntry int,
			warehouseLocationEntry varchar(100),
			previousBalance decimal(14,6),
			entryAmount decimal(14,6),
			exitAmount decimal(14,6),
			balance decimal(14,6),
			balanceCutting decimal(14,6),
			numberRemissionGuide varchar(100),
			idCompany int,
			nameBranchOffice varchar(100),
			nameDivision varchar(100),
			nameCompany varchar(100),
			dateCreate datetime,
			Provider_name varchar(100),
			isCopacking bit,
			nameProviderShrimp varchar(100),
			productionUnitProviderPool varchar(100),
			itemSize varchar(100),
			itemType varchar(100),
			ItemMetricUnit varchar(100),
			ItemPresentationValue decimal(14,6)
		)

		INSERT INTO #TmpInventoryMoveDetailRespuesta
		SELECT B.id
					,B.id_inventoryMove id_document
					,CAST(ISNULL(C.sequential, 0) AS varchar(100)) document
					,P.[name] nameDocumentState
					,D.id_documentType
					,E.[name] documentType
					,C.id_inventoryReason
					,F.[name] inventoryReason
					,D.emissionDate
					,B.id_item
					,(G.masterCode + ' - ' + G.[name]) code_item
					,B.id_metricUnit
					,H.code metricUnit
					,B.id_lot
					,(IIF((ISNULL(I.number, '') = ''), ISNULL(I.internalNumber, ''), (I.number + IIF(ISNULL(Pl.internalNumber, '') = '', '', ' - ' + Pl.internalNumber)))) number
					,Isnull(Pl.internalNumber,I.internalNumber) internalNumber
					,B.id_warehouse
					,J.[name] warehouse
					,B.id_warehouseLocation
					,K.[name] warehouseLocation
					,1 id_warehouseExit
					,'' warehouseExit
					,2 id_warehouseLocationExit
					,'' warehouseLocationExit
					,2 id_warehouseEntry
					,'' warehouseEntry
					,(IIF((B.entryAmount > 0), B.id_warehouseLocation, NULL)) id_warehouseLocationEntry
					,(IIF((B.entryAmount > 0), (ISNULL(K.[name], '')), '')) warehouseLocationEntry
					,B.previousBalance
					,B.entryAmount [entry]
					,B.exitAmount [exit]
					,B.balance
					,B.balanceCutting
					,B.numberRemissionGuide
					,L.id_company idCompany
					,M.[name] nameBranchOffice
					,N.[name] nameDivision
					,O.businessName nameCompany
					,B.dateCreate
					,PProveedor.fullname_businessName Provider_name
					,isnull(PProveedor.isCopacking,0) isCopacking
					,PLPUP.name nameProviderShrimp
					,PLPUPP.name productionUnitProviderPool
					,[itemSize].[name] itemSize
					,[itemType].[name] itemType
					,[presentationMetricUnit].[code] ItemMetricUnit
				    ,[presentation].[minimum] * [presentation].[maximum] ItemPresentationValue
			FROM --[dbo].[InventoryMoveDetail] A WITH (NOLOCK) INNER JOIN
			 #TmpIdentificadoresInventoryMoveDetailReduced B --ON (B.id = A.id) AND (B.codeDocumentState = IIF((@codeReport = 'IMIPV1'), (B.codeDocumentState), ('03')))--'03' APROBADA
			--AND (((B.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (B.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
			INNER JOIN dbo.InventoryMove C WITH (NOLOCK) ON (C.id = B.id_inventoryMove) AND (B.codeDocumentState = IIF((@codeReport = 'IMIPV1'), (B.codeDocumentState), ('03')))--'03' APROBADA
			INNER JOIN dbo.Document D WITH (NOLOCK) ON (D.id = C.id)
			INNER JOIN dbo.DocumentState P WITH (NOLOCK) ON (P.id = D.id_documentState)
			INNER JOIN dbo.DocumentType E WITH (NOLOCK) ON (E.id = D.id_documentType)
			LEFT JOIN dbo.InventoryReason F WITH (NOLOCK) ON (F.id = C.id_inventoryReason)
			INNER JOIN dbo.Item G WITH (NOLOCK) ON (G.id = B.id_item)
			INNER JOIN dbo.MetricUnit H WITH (NOLOCK) ON (H.id = B.id_metricUnit)
			LEFT JOIN dbo.Lot I WITH (NOLOCK) ON (I.id = B.id_lot)
			INNER JOIN dbo.Warehouse J WITH (NOLOCK) ON (J.id = B.id_warehouse)
			LEFT JOIN dbo.WarehouseLocation K WITH (NOLOCK) ON (K.id = B.id_warehouseLocation)
			INNER JOIN dbo.EmissionPoint L WITH (NOLOCK) ON (L.id = D.id_emissionPoint)
			INNER JOIN dbo.BranchOffice M WITH (NOLOCK) ON (M.id = L.id_branchOffice)
			INNER JOIN dbo.Division N WITH (NOLOCK) ON (N.id = L.id_division)
			INNER JOIN dbo.Company O WITH (NOLOCK) ON (O.id = L.id_company)
			LEFT JOIN dbo.ProductionLot Pl WITH (NOLOCK) ON (Pl.id = I.id or pl.internalNumber = I.internalNumber)
			LEFT JOIN dbo.Person PProveedor WITH (NOLOCK) ON (PProveedor.id = pl.id_provider) 
			LEFT join dbo.ProductionUnitProvider PLPUP WITH (NOLOCK) on (PLPUP.id = Pl.id_productionUnitProvider)
			LEFT join dbo.ProductionUnitProviderPool PLPUPP  WITH (NOLOCK) on (PLPUPP.id = pL.id_productionUnitProviderPool)
			LEFT JOIN dbo.ItemGeneral itemGeneral WITH (NOLOCK) ON (itemGeneral.id_Item = G.id)
			LEFT OUTER JOIN dbo.ItemGroup itemGroup	WITH (NOLOCK) ON  (itemGroup.id = itemGeneral.id_GROUP)
			LEFT OUTER JOIN dbo.ItemSize itemSize WITH (NOLOCK) ON  (itemSize.id = itemGeneral.id_size)
			INNER JOIN dbo.ItemType itemType WITH (NOLOCK) ON (g.id_itemType = itemType.id)
			LEFT JOIN dbo.presentation presentation WITH (NOLOCK) ON (g.id_presentation = presentation.id)
			LEFT JOIN dbo.MetricUnit presentationMetricUnit WITH (NOLOCK) ON (presentation.id_metricUnit = presentationMetricUnit.id)
			--ORDER BY D.emissionDate DESC, A.dateCreate DESC
			--UNION
			INSERT INTO #TmpInventoryMoveDetailRespuesta
			SELECT AA.id
					,0 id_document--A.id_inventoryMove id_document
					,'' document--CAST(ISNULL(C.sequential, 0) AS varchar(100)) document
					,AP.[name] nameDocumentState
					,0 id_documentType--D.id_documentType
					,'' documentType--E.[name] documentType
					,null id_inventoryReason--C.id_inventoryReason
					,'' inventoryReason--F.[name] inventoryReason
					,@startEmissionDate emissionDate--D.emissionDate
					,AA.id_item
					,(AG.masterCode + ' - ' + AG.[name]) code_item
					,AA.id_metricUnit
					,AH.code metricUnit
					,AA.id_lot
					,(IIF((ISNULL(AI.number, '') = ''), ISNULL(AI.internalNumber, ''), (AI.number + IIF(ISNULL(Pl.internalNumber, '') = '', '', ' - ' + Pl.internalNumber)))) number
					,Isnull(Pl.internalNumber,AI.internalNumber) internalNumber
					,AA.id_warehouse
					,AJ.[name] warehouse
					,AA.id_warehouseLocation
					,AK.[name] warehouseLocation
					,1 id_warehouseExit
					,'' warehouseExit
					,2 id_warehouseLocationExit
					,'' warehouseLocationExit
					,2 id_warehouseEntry
					,'' warehouseEntry
					,null id_warehouseLocationEntry--(IIF((A.entryAmount > 0), A.id_warehouseLocation, NULL)) id_warehouseLocationEntry
					,'' warehouseLocationEntry--(IIF((A.entryAmount > 0), (ISNULL(K.[name], '')), '')) warehouseLocationEntry
					,AB.previousBalance
					,0 [entry]--AA.entryAmount [entry]
					,0 [exit]--AA.exitAmount [exit]
					,AB.previousBalance balance
					,AB.previousBalance balanceCutting--B.balanceCutting
					,'' numberRemissionGuide--B.numberRemissionGuide
					,AL.id_company idCompany
					,AM.[name] nameBranchOffice
					,AN.[name] nameDivision
					,AO.businessName nameCompany
					,@startEmissionDate dateCreate--A.dateCreate
					,PProveedor.fullname_businessName Provider_name
					,isnull(PProveedor.isCopacking,0) isCopacking
					,PLPUP.name nameProviderShrimp
					,PLPUPP.name productionUnitProviderPool
					,[itemSize].[name] itemSize
					,[itemType].[name] itemType
					,[presentationMetricUnit].[code] ItemMetricUnit
					,[presentation].[minimum] * [presentation].[maximum] ItemPresentationValue
			FROM [dbo].[InventoryMoveDetail] AA WITH (NOLOCK)
			INNER JOIN #TmpInventoryMoveDetailNoShow AB ON (AB.id = AA.id)
			INNER JOIN dbo.InventoryMove AC WITH (NOLOCK) ON (AC.id = AA.id_inventoryMove)
			INNER JOIN dbo.Document AD WITH (NOLOCK) ON (AD.id = AC.id)
			INNER JOIN dbo.DocumentState AP WITH (NOLOCK) ON (AP.id = AD.id_documentState)
			--INNER JOIN dbo.DocumentType E WITH (NOLOCK) ON (E.id = D.id_documentType)
			--LEFT JOIN dbo.InventoryReason F WITH (NOLOCK) ON (F.id = C.id_inventoryReason)
			INNER JOIN dbo.Item AG WITH (NOLOCK) ON (AG.id = AA.id_item)
			INNER JOIN dbo.MetricUnit AH WITH (NOLOCK) ON (AH.id = AA.id_metricUnit)
			LEFT JOIN dbo.Lot AI WITH (NOLOCK) ON (AI.id = AA.id_lot)
			INNER JOIN dbo.Warehouse AJ WITH (NOLOCK) ON (AJ.id = AA.id_warehouse)
			LEFT JOIN dbo.WarehouseLocation AK WITH (NOLOCK) ON (AK.id = AA.id_warehouseLocation)
			INNER JOIN dbo.EmissionPoint AL WITH (NOLOCK) ON (AL.id = AD.id_emissionPoint)
			INNER JOIN dbo.BranchOffice AM WITH (NOLOCK) ON (AM.id = AL.id_branchOffice)
			INNER JOIN dbo.Division AN WITH (NOLOCK) ON (AN.id = AL.id_division)
			INNER JOIN dbo.Company AO WITH (NOLOCK) ON (AO.id = AL.id_company)
			LEFT JOIN dbo.ProductionLot Pl WITH (NOLOCK) ON (Pl.id = AI.id or pl.internalNumber = AI.internalNumber)
			LEFT JOIN dbo.Person PProveedor WITH (NOLOCK) ON (PProveedor.id = pl.id_provider) 
			LEFT join dbo.ProductionUnitProvider PLPUP WITH (NOLOCK) on (PLPUP.id = Pl.id_productionUnitProvider)
			LEFT join dbo.ProductionUnitProviderPool PLPUPP  WITH (NOLOCK) on (PLPUPP.id = pL.id_productionUnitProviderPool)
			LEFT JOIN dbo.ItemGeneral itemGeneral WITH (NOLOCK) ON (itemGeneral.id_Item = AG.id)
			LEFT OUTER JOIN dbo.ItemGroup itemGroup	WITH (NOLOCK) ON  (itemGroup.id = itemGeneral.id_GROUP)
			LEFT OUTER JOIN dbo.ItemSize itemSize WITH (NOLOCK) ON  (itemSize.id = itemGeneral.id_size)
			INNER JOIN dbo.ItemType itemType WITH (NOLOCK) ON (AG.id_itemType = itemType.id)
			LEFT JOIN dbo.presentation presentation WITH (NOLOCK) ON (AG.id_presentation = presentation.id)
			LEFT JOIN dbo.MetricUnit presentationMetricUnit WITH (NOLOCK) ON (presentation.id_metricUnit = presentationMetricUnit.id)

			SELECT * FROM #TmpInventoryMoveDetailRespuesta
		END
		ELSE
		BEGIN
			--INSERT INTO #TmpInventoryMoveDetailRespuesta
			SELECT B.id
				  ,B.id_inventoryMove id_document
				  ,CAST(ISNULL(C.sequential, 0) AS varchar(100)) document
				  ,P.[name] nameDocumentState
				  ,D.id_documentType
				  ,E.[name] documentType
				  ,C.id_inventoryReason
				  ,F.[name] inventoryReason
				  ,D.emissionDate
				  ,B.id_item
				  ,(G.masterCode + ' - ' + G.[name]) code_item
				  ,B.id_metricUnit
				  ,H.code metricUnit
				  ,B.id_lot
				  ,(IIF((ISNULL(I.number, '') = ''), ISNULL(I.internalNumber, ''), (I.number + IIF(ISNULL(Pl.internalNumber, ISNULL(I.internalNumber, '')) = '', '', ' - ' + ISNULL(Pl.internalNumber, I.internalNumber))))) number
				  ,Isnull(Pl.internalNumber,I.internalNumber) internalNumber
				  ,B.id_warehouse
				  ,J.[name] warehouse
				  ,B.id_warehouseLocation
				  ,K.[name] warehouseLocation
				  ,1 id_warehouseExit
				  ,'' warehouseExit
				  ,2 id_warehouseLocationExit
				  ,'' warehouseLocationExit
				  ,2 id_warehouseEntry
				  ,'' warehouseEntry
				  ,(IIF((B.entryAmount > 0), B.id_warehouseLocation, NULL)) id_warehouseLocationEntry
				  ,(IIF((B.entryAmount > 0), (ISNULL(K.[name], '')), '')) warehouseLocationEntry
				  ,B.previousBalance
				  ,B.entryAmount [entry]
				  ,B.exitAmount [exit]
				  ,B.balance
				  ,B.balanceCutting
				  ,B.numberRemissionGuide
				  ,L.id_company idCompany
				  ,M.[name] nameBranchOffice
				  ,N.[name] nameDivision
				  ,O.businessName nameCompany
				  ,B.dateCreate
				  ,PProveedor.fullname_businessName Provider_name
				  ,isnull(PProveedor.isCopacking,0) isCopacking
				  ,PLPUP.name nameProviderShrimp
				  ,PLPUPP.name productionUnitProviderPool
				  ,[itemSize].[name] itemSize
				  ,[itemType].[name] itemType
				  ,[presentationMetricUnit].[code] ItemMetricUnit
				  ,[presentation].[minimum] * [presentation].[maximum] ItemPresentationValue
			FROM #TmpIdentificadoresInventoryMoveDetailReduced B
			--INNER JOIN [dbo].[InventoryMoveDetail] A WITH (NOLOCK) ON (A.id = B.id) AND 
			--AND (((B.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (B.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
			INNER JOIN dbo.InventoryMove C WITH (NOLOCK) ON (C.id = B.id_inventoryMove) AND (B.codeDocumentState = IIF((@codeReport = 'IMIPV1'), (B.codeDocumentState), ('03')))--'03' APROBADA
			INNER JOIN dbo.Document D WITH (NOLOCK) ON (D.id = C.id)
			INNER JOIN dbo.DocumentState P WITH (NOLOCK) ON (P.id = D.id_documentState)
			INNER JOIN dbo.DocumentType E WITH (NOLOCK) ON (E.id = D.id_documentType)
			LEFT JOIN dbo.InventoryReason F WITH (NOLOCK) ON (F.id = C.id_inventoryReason)
			INNER JOIN dbo.Item G WITH (NOLOCK) ON (G.id = B.id_item)
			INNER JOIN dbo.MetricUnit H WITH (NOLOCK) ON (H.id = B.id_metricUnit)
			LEFT JOIN dbo.Lot I WITH (NOLOCK) ON (I.id = B.id_lot)
			INNER JOIN dbo.Warehouse J WITH (NOLOCK) ON (J.id = B.id_warehouse)
			LEFT JOIN dbo.WarehouseLocation K WITH (NOLOCK) ON (K.id = B.id_warehouseLocation)
			INNER JOIN dbo.EmissionPoint L WITH (NOLOCK) ON (L.id = D.id_emissionPoint)
			INNER JOIN dbo.BranchOffice M WITH (NOLOCK) ON (M.id = L.id_branchOffice)
			INNER JOIN dbo.Division N WITH (NOLOCK) ON (N.id = L.id_division)
			INNER JOIN dbo.Company O WITH (NOLOCK) ON (O.id = L.id_company)
			LEFT JOIN dbo.ProductionLot Pl WITH (NOLOCK) ON (Pl.id = I.id or pl.internalNumber = I.internalNumber)
			LEFT JOIN dbo.Person PProveedor WITH (NOLOCK) ON (PProveedor.id = pl.id_provider) 
			LEFT join dbo.ProductionUnitProvider PLPUP WITH (NOLOCK) on (PLPUP.id = Pl.id_productionUnitProvider)
			LEFT join dbo.ProductionUnitProviderPool PLPUPP  WITH (NOLOCK) on (PLPUPP.id = pL.id_productionUnitProviderPool)
			LEFT JOIN dbo.ItemGeneral itemGeneral WITH (NOLOCK) ON (itemGeneral.id_Item = G.id)
			LEFT OUTER JOIN dbo.ItemGroup itemGroup	WITH (NOLOCK) ON  (itemGroup.id = itemGeneral.id_GROUP)
			LEFT OUTER JOIN dbo.ItemSize itemSize WITH (NOLOCK) ON  (itemSize.id = itemGeneral.id_size)
			INNER JOIN dbo.ItemType itemType WITH (NOLOCK) ON (g.id_itemType = itemType.id)
			LEFT JOIN dbo.presentation presentation WITH (NOLOCK) ON (g.id_presentation = presentation.id)
			LEFT JOIN dbo.MetricUnit presentationMetricUnit WITH (NOLOCK) ON (presentation.id_metricUnit = presentationMetricUnit.id)
			--WHERE (B.codeDocumentState = IIF((@codeReport = 'IMIPV1'), (B.codeDocumentState), ('03')))--'03' APROBADA
		END
END
