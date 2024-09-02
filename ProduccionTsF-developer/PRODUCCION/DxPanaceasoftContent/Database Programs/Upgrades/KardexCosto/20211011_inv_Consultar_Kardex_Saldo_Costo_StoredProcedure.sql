
ALter PROCEDURE [dbo].[inv_Consultar_Kardex_Saldo_Costo_StoredProcedure]
	@ParametrosBusquedaKardexSaldo		NVARCHAR(MAX)
	--@Resultado							NVARCHAR(MAX) OUTPUT
AS
BEGIN
	SET NOCOUNT ON 
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

	DECLARE @UnidadMedidaLbsId int = (select top 1 Id from  MetricUnit where code = 'Lbs')

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
		entryAmount decimal(14,6), -- AMOUNT
		exitAmount decimal(14,6),  -- AMOUNT
		id_inventoryMove int,
		previousBalance decimal(14,6),
		balance decimal(14,6),
		balanceCutting decimal(14,6),
		numberRemissionGuide varchar(100),
		codeDocumentState varchar(20),
		dateCreate datetime,

		-- Cost
		amount decimal(14,6),  -- AMOUNT
		unitPriceMove decimal(14,6),-- Cost Unitario Cantidad --AmountCotsUnit
		unitCostTotal decimal(14,6), -- Costo totalCantidad - AmountCostTotal
		
	)

	INSERT INTO #TmpIdentificadoresInventoryMoveDetail
	SELECT	A.id, 
			C.emissionDate, 
			B.sequential, 
			A.id_item, 
			A.id_warehouse, 
			A.id_warehouseLocation, 
			A.id_lot, 
			A.entryAmount, 
			A.exitAmount, 
			A.id_inventoryMove, 
			0, 
			0, 
			0, 
			'', 
			D.code, 
			A.dateCreate,
			(A.entryAmount- A.exitAmount),
			A.unitPriceMove,
			(ISNULL(A.entryAmount,0)- ISNULL(A.exitAmount,0)) * ISNULL(A.unitPriceMove,0)
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
		-- Cost
		amount decimal(14,6),  -- AMOUNT
		unitPriceMove decimal(14,6),-- Cost Unitario Cantidad --AmountCotsUnit
		unitCostTotal decimal(14,6), -- Costo totalCantidad - AmountCostTotal
	)

	INSERT INTO #TmpIdentificadoresInventoryMoveDetailReduced
	SELECT A.*
	FROM #TmpIdentificadoresInventoryMoveDetail A
	WHERE (((A.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (A.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL #TmpInventoryMoveDetailWithPreviousBalance
	--==================================================================================================
	CREATE TABLE #TmpInventoryMoveDetailWithPreviousBalance
	(
		id	int,
		previousBalance decimal(14,6), -- AMOUNT
		previousBalanceCutting decimal(14,6),
		id_item int,
		id_warehouse int,
		id_warehouseLocation int,
		id_lot int,
		emissionDate datetime,
		dateCreate datetime,
		-- Cost
		previoUnitCostTotal decimal(18,6), -- Costo totalCantidad - AmountCostTotal
	)

	INSERT INTO #TmpInventoryMoveDetailWithPreviousBalance
	SELECT	A.id, 
			SUM(ISNULL(AA.entryAmount, 0) - ISNULL(AA.exitAmount, 0)) as previoBalance, 
			NULL, 
			NULL, 
			NULL, 
			NULL, 
			NULL, 
			NULL, 
			NULL,
			--0
			SUM( (ISNULL(AA.entryAmount, 0) - ISNULL(AA.exitAmount, 0)) * ISNULL(AA.unitCostTotal, 0)   ) as previoUnitCostTotal 
			
	FROM #TmpIdentificadoresInventoryMoveDetailReduced A
	INNER JOIN #TmpIdentificadoresInventoryMoveDetail AA WITH (NOLOCK) 
	ON (AA.codeDocumentState = '03') 
	WHERE (A.codeDocumentState = '03') --'03' APROBADA 
	AND (	AA.id_item = A.id_item 
			AND AA.id_warehouse = A.id_warehouse 
			/*AND AA.id_warehouseLocation = A.id_warehouseLocation AND (ISNULL(AA.id_lot, 0) = ISNULL(A.id_lot, 0))*/ ) 
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
		WHERE (	AA.id_item = A.id_item 
				AND AA.id_warehouse = A.id_warehouse 
				/*AND AA.id_warehouseLocation = A.id_warehouseLocation AND (ISNULL(AA.id_lot, 0) = ISNULL(A.id_lot, 0))*/ ) 
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
			exitAmount decimal(14,6),
			unitPriceMove decimal(14,6),
			unitCostTotal decimal(14,6)

		)

		INSERT INTO #TmpInventoryMoveDetailToNoShow
		SELECT	A.id, 
				A.id_item, 
				A.id_warehouse, 
				A.id_warehouseLocation, 
				A.id_lot, 
				A.entryAmount, A.exitAmount,
				A.unitPriceMove,
				(ISNULL(A.entryAmount,0)- ISNULL(A.exitAmount,0)) * ISNULL(A.unitPriceMove,0) AS unitCostTotal
		FROM #TmpIdentificadoresInventoryMoveDetail A
		WHERE (A.codeDocumentState = '03') AND (A.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)

		DELETE A
		FROM #TmpInventoryMoveDetailToNoShow A WITH (NOLOCK)
		INNER JOIN #TmpIdentificadoresInventoryMoveDetailReduced AA WITH (NOLOCK) ON (AA.codeDocumentState = '03') 
		WHERE (	AA.id_item = A.id_item 
				AND AA.id_warehouse = A.id_warehouse 
				/*AND AA.id_warehouseLocation = A.id_warehouseLocation AND (ISNULL(AA.id_lot, 0) = ISNULL(A.id_lot, 0))*/ )
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
			id_lot int,
			-- Cost
			previounitPriceMove decimal(14,6),
			previounitCostTotal decimal(14,6)
		)

		INSERT INTO #TmpInventoryMoveDetailNoShow
		SELECT	Max(A.id), SUM(ISNULL(A.entryAmount, 0) - ISNULL(A.exitAmount, 0)) as previoBalance, 
				A.id_item, 
				A.id_warehouse, 
				A.id_warehouseLocation, 
				A.id_lot,
				SUM(ISNULL(A.unitPriceMove, 0)),
				SUM( (ISNULL(A.entryAmount, 0) - ISNULL(A.exitAmount, 0)) * ISNULL(A.unitPriceMove, 0)   ) as previoUnitCostTotal 
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

	
	
	--SELECT COUNT(1) AS TotalRowCount_InventoryMoveDetail FROM InventoryMoveDetail
	IF(@startEmissionDate IS NOT NULL)
	BEGIN
	SELECT 
			x.id [idDetalleInventario]
			,x.id_document [idCabeceraInventario]
			,x.document [numeroDocumentoInventario]
			,x.nameDocumentState [nombreEstado]
			,x.id_documentType
			,x.documentType
			,ISNULL(x.id_inventoryReason,0) [idMotivoInventario]
			,ISNULL(NULLIF(x.inventoryReason, ''), 'Saldo Inicial') [nombreMotivoInventario]
			,x.emissionDate [fechaEmison]
			,x.id_item [idProducto]
			,x.code_item [nombreProducto]
			,x.id_metricUnit [idUnidadMedida]
			,x.metricUnit [nombreUnidadMedida]
			,x.id_lot
			,x.number
			,x.internalNumber [numberLot]
			,x.id_warehouse [idBodega]
			,x.warehouse [nombreBodega]
			,x.id_warehouseLocation [idUbicacion]
			,x.warehouseLocation [nombreUbicacion]
			,x.id_warehouseExit
			,x.warehouseExit
			,x.id_warehouseLocationExit
			,x.warehouseLocationExit
			,x.id_warehouseEntry
			,x.warehouseEntry
			,x.id_warehouseLocationEntry
			,x.warehouseLocationEntry
			,x.previousBalance
			,x.[entry] [montoEntrada]	-- Cantidad Ingreso
			,x.[exit] [montoSalida]		-- Cantidad Egreso
			,x.balance	-- Cantridad Previa
			,x.balanceCutting
			,x.numberRemissionGuide
			,x.idCompany [idCompania]
			,x.nameBranchOffice
			,x.nameDivision
			,x.nameCompany [nameCompania]
			,x.dateCreate
			,x.Provider_name
			,x.isCopacking
			,x.nameProviderShrimp
			,x.productionUnitProviderPool
			,x.itemSize
			,x.itemType
			,x.ItemMetricUnit
			,x.ItemPresentationValue,
			x.amount, -- Cantidad
			x.amountCostUnit, -- CostoUnitario
			x.amountCostTotal,
			x.MetricUnitId,
			--------------- CALCULADOS
			-------------------
			--- Saldo Inicial
			CASE 
			  WHEN ISNULL(x.id_inventoryReason,0) = 0 then x.previousPound
			  ELSE 0
			  END AS [previousPound],
			CASE 
				WHEN ISNULL(x.id_inventoryReason,0) = 0 
						then (CASE 
								WHEN x.previousTotalCostPound = 0 THEN 0
								ELSE (x.previousTotalCostPound/x.previousPound)
							END)
				ELSE 0
			END AS [previousCostPound],
			CASE 
				WHEN ISNULL(x.id_inventoryReason,0) = 0  THEN x.previousTotalCostPound
				ELSE 0
			END AS [previousTotalCostPound],
			-------------------
			--- Ingresos
			x.entradaPound,
			CASE 
				WHEN x.entradaTotalCostPound = 0 THEN 0
				ELSE (x.entradaTotalCostPound/x.entradaPound)
			END [entradaCostPound],
			x.entradaTotalCostPound,
			-------------------
			--- Egresos
			x.salidaPound,
			CASE 
				WHEN x.salidaTotalCostPound = 0 THEN 0
				ELSE (x.salidaTotalCostPound/x.salidaPound)
			END [salidaCostPound],
			x.salidaTotalCostPound,
			-------------------
			--- Saldo FInal
			x.finalPound,
			CASE 
				WHEN x.finalTotalCostPound = 0 THEN 0
				ELSE (x.finalTotalCostPound/x.finalPound)
			END [finalCostPound],
			x.finalTotalCostPound,
			-----------
			-- Forzados
			@startEmissionDate	 [fechaInicio],
			@endEmissionDate	 [fechaFin] ,
			0 [idEstado],
			'' [nombreEstado],
			-----------
			-- CABECERA
			Substring(x.itemPresentationDescrip,1,100) [itemPresentationDescrip],
			x.oneItemPound
	FROM
	(
		SELECT 
		--------------- CAMPO ORIGINALES
			x.id
			,x.id_document
			,x.document
			,x.nameDocumentState
			,x.id_documentType
			,x.documentType
			,x.id_inventoryReason
			,x.inventoryReason
			,x.emissionDate
			,x.id_item
			,x.code_item
			,x.id_metricUnit
			,x.metricUnit
			,x.id_lot
			,x.number
			,x.internalNumber
			,x.id_warehouse
			,x.warehouse
			,x.id_warehouseLocation
			,x.warehouseLocation
			,x.id_warehouseExit
			,x.warehouseExit
			,x.id_warehouseLocationExit
			,x.warehouseLocationExit
			,x.id_warehouseEntry
			,x.warehouseEntry
			,x.id_warehouseLocationEntry
			,x.warehouseLocationEntry
			,x.previousBalance
			,x.[entry] -- Cantidad Ingreso
			,x.[exit]   -- Cantidad Egreso
			,x.balance			   -- Cantridad Previa
			,x.balanceCutting
			,x.numberRemissionGuide
			,x.idCompany
			,x.nameBranchOffice
			,x.nameDivision
			,x.nameCompany
			,x.dateCreate
			,x.Provider_name
			,x.isCopacking
			,x.nameProviderShrimp
			,x.productionUnitProviderPool
			,x.itemSize
			,x.itemType
			,x.ItemMetricUnit
			,x.ItemPresentationValue,
			x.amount, -- Cantidad
			x.amountCostUnit, -- CostoUnitario
			x.amountCostTotal,
			x.MetricUnitId,
			--------------- CALCULADOS
			-------------------
			--- Saldo Inicial
			CASE 
			  WHEN Y.factor IS NULL THEN ( x.previousBalance * x.ItemPresentationValue ) 
			  ELSE ( x.previousBalance * x.ItemPresentationValue * Y.factor) 
			END AS [previousPound],
			(x.previousBalance * x.amountCostUnit) [previousTotalCostPound],
			-------------------
			--- Ingresos
			CASE 
			  WHEN Y.factor IS NULL THEN ( x.[entry] * x.ItemPresentationValue ) 
			  ELSE ( x.[entry] * x.ItemPresentationValue * Y.factor) 
			END AS [entradaPound],
			(x.[entry] * x.amountCostUnit) [entradaTotalCostPound],
			-------------------
			--- Egresos
			CASE 
			  WHEN Y.factor IS NULL THEN ( x.[exit] * x.ItemPresentationValue ) 
			  ELSE ( x.[exit] * x.ItemPresentationValue * Y.factor) 
			END AS [salidaPound],
			(x.[exit] * x.amountCostUnit) [salidaTotalCostPound],
			-------------------
			--- Saldo Final
			CASE 
			  WHEN Y.factor IS NULL THEN ( x.balance * x.ItemPresentationValue ) 
			  ELSE ( x.balance * x.ItemPresentationValue * Y.factor) 
			END AS [finalPound],
			(x.balance * x.amountCostUnit) [finalTotalCostPound],
			x.itemPresentationDescrip,
			-------------------
			--- Peso Unidad
			CASE 
			  WHEN Y.factor IS NULL THEN (x.ItemPresentationValue ) 
			  ELSE (  x.ItemPresentationValue * Y.factor) 
			END AS [oneItemPound]
		FROM  
		(
			SELECT  A.id
				   ,A.id_inventoryMove id_document
					,CAST(ISNULL(C.sequential, 0) AS varchar(100)) document
					,P.[name] nameDocumentState
					,D.id_documentType
					,E.[name] documentType
					,C.id_inventoryReason
					,F.[name] inventoryReason
					,D.emissionDate
					,A.id_item
					,(G.masterCode + ' - ' + G.[name]) code_item
					,A.id_metricUnit
					,H.code metricUnit
					,A.id_lot
					,(IIF((ISNULL(I.number, '') = ''), ISNULL(I.internalNumber, ''), (I.number + IIF(ISNULL(Pl.internalNumber, '') = '', '', ' - ' + Pl.internalNumber)))) number
					,Isnull(Pl.internalNumber,I.internalNumber) internalNumber
					,A.id_warehouse
					,J.[name] warehouse
					,A.id_warehouseLocation
					,K.[name] warehouseLocation
					,1 id_warehouseExit
					,'' warehouseExit
					,2 id_warehouseLocationExit
					,'' warehouseLocationExit
					,2 id_warehouseEntry
					,'' warehouseEntry
					,(IIF((A.entryAmount > 0), A.id_warehouseLocation, NULL)) id_warehouseLocationEntry
					,(IIF((A.entryAmount > 0), (ISNULL(K.[name], '')), '')) warehouseLocationEntry
					,B.previousBalance
					,A.entryAmount [entry] -- Cantidad Ingreso
					,A.exitAmount [exit]   -- Cantidad Egreso
					,B.balance			   -- Cantridad Previa
					,B.balanceCutting
					,B.numberRemissionGuide
					,L.id_company idCompany
					,M.[name] nameBranchOffice
					,N.[name] nameDivision
					,O.businessName nameCompany
					,A.dateCreate
					,PProveedor.fullname_businessName Provider_name
					,isnull(PProveedor.isCopacking,0) isCopacking
					,PLPUP.name nameProviderShrimp
					,PLPUPP.name productionUnitProviderPool
					,[itemSize].[name] itemSize
					,[itemType].[name] itemType
					,[presentationMetricUnit].[code] ItemMetricUnit
					,[presentation].[minimum] * [presentation].[maximum] ItemPresentationValue,
					(ISNULL(A.entryAmount,0)-ISNULL(A.exitAmount,0) ) [amount], -- Cantidad
					ISNULL(A.unitPriceMove,0) [amountCostUnit], -- CostoUnitario
					((ISNULL(A.entryAmount,0)-ISNULL(A.exitAmount,0) ) *  ISNULL(A.unitPriceMove,0)) [amountCostTotal],
					presentationMetricUnit.id [MetricUnitId],
					presentation.description [itemPresentationDescrip]
				--- MOvimientos En Periodo
				FROM [dbo].[InventoryMoveDetail] A WITH (NOLOCK)
				INNER JOIN #TmpIdentificadoresInventoryMoveDetailReduced B ON (B.id = A.id) AND (B.codeDocumentState = IIF((@codeReport = 'IMIPV1'), (B.codeDocumentState), ('03')))--'03' APROBADA
				--AND (((B.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (B.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
				INNER JOIN dbo.InventoryMove C WITH (NOLOCK) ON (C.id = A.id_inventoryMove)
				INNER JOIN dbo.Document D WITH (NOLOCK) ON (D.id = C.id)
				INNER JOIN dbo.DocumentState P WITH (NOLOCK) ON (P.id = D.id_documentState)
				INNER JOIN dbo.DocumentType E WITH (NOLOCK) ON (E.id = D.id_documentType)
				LEFT JOIN dbo.InventoryReason F WITH (NOLOCK) ON (F.id = C.id_inventoryReason)
				INNER JOIN dbo.Item G WITH (NOLOCK) ON (G.id = A.id_item)
				INNER JOIN dbo.MetricUnit H WITH (NOLOCK) ON (H.id = A.id_metricUnit)
				LEFT JOIN dbo.Lot I WITH (NOLOCK) ON (I.id = A.id_lot)
				INNER JOIN dbo.Warehouse J WITH (NOLOCK) ON (J.id = A.id_warehouse)
				LEFT JOIN dbo.WarehouseLocation K WITH (NOLOCK) ON (K.id = A.id_warehouseLocation)
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
				UNION
				-- MOVIMIENTOS  PREVIOS AL PERIODO
				(SELECT AA.id
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
						,AB.previousBalance balance  -- Cantidad Pevia
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
						,[presentation].[minimum] * [presentation].[maximum] ItemPresentationValue,
						AB.previousBalance [amount],
						(ISNULL(AB.previounitCostTotal,0) /  ISNULL(AB.previousBalance,0))  [amountCostUnit],
						AB.previounitCostTotal [amountCostTotal],
						presentationMetricUnit.id [MetricUnitId],
						presentation.description [itemPresentationDescrip]
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
				LEFT JOIN dbo.MetricUnit presentationMetricUnit WITH (NOLOCK) ON (presentation.id_metricUnit = presentationMetricUnit.id))
			) X LEFT JOIN MetricUnitConversion Y
			 ON X.MetricUnitId = Y.id_metricOrigin
			 WHERE Y.id_metricDestiny = @UnidadMedidaLbsId
		) X 
		--where X.code_item LIKE '%PT000026%'
		--order by X.[id_warehouse],
		--			 X.[id_warehouseLocation],
		--			 X.[emissionDate],
		--			 X.[id_document],
		--			 X.id
		END
		ELSE
		BEGIN
		SELECT 
			x.id [idDetalleInventario]
			,x.id_document [idCabeceraInventario]
			,x.document [numeroDocumentoInventario]
			,x.nameDocumentState [nombreEstado]
			,x.id_documentType
			,x.documentType
			,ISNULL(x.id_inventoryReason,0) [idMotivoInventario]
			,ISNULL(NULLIF(x.inventoryReason, ''), 'Saldo Inicial') [nombreMotivoInventario]
			,x.emissionDate [fechaEmison]
			,x.id_item [idProducto]
			,x.code_item [nombreProducto]
			,x.id_metricUnit [idUnidadMedida]
			,x.metricUnit [nombreUnidadMedida]
			,x.id_lot
			,x.number
			,x.internalNumber [numberLot]
			,x.id_warehouse [idBodega]
			,x.warehouse [nombreBodega]
			,x.id_warehouseLocation [idUbicacion]
			,x.warehouseLocation [nombreUbicacion]
			,x.id_warehouseExit
			,x.warehouseExit
			,x.id_warehouseLocationExit
			,x.warehouseLocationExit
			,x.id_warehouseEntry
			,x.warehouseEntry
			,x.id_warehouseLocationEntry
			,x.warehouseLocationEntry
			,x.previousBalance
			,x.[entry] [montoEntrada]	-- Cantidad Ingreso
			,x.[exit] [montoSalida]		-- Cantidad Egreso
			,x.balance	-- Cantridad Previa
			,x.balanceCutting
			,x.numberRemissionGuide
			,x.idCompany [idCompania]
			,x.nameBranchOffice
			,x.nameDivision
			,x.nameCompany [nameCompania]
			,x.dateCreate
			,x.Provider_name
			,x.isCopacking
			,x.nameProviderShrimp
			,x.productionUnitProviderPool
			,x.itemSize
			,x.itemType
			,x.ItemMetricUnit
			,x.ItemPresentationValue,
			x.amount, -- Cantidad
			x.amountCostUnit, -- CostoUnitario
			x.amountCostTotal,
			x.MetricUnitId,
			--------------- CALCULADOS
			-------------------
			--- Saldo Inicial
			--x.previousPound,
			CASE 
			  WHEN ISNULL(x.id_inventoryReason,0) = 0 then x.previousPound
			  ELSE 0
			  END AS [previousPound],
			CASE 
				WHEN ISNULL(x.id_inventoryReason,0) = 0 
						then (CASE 
								WHEN x.previousTotalCostPound = 0 THEN 0
								ELSE (x.previousTotalCostPound/x.previousPound)
							END)
				ELSE 0
			END AS [previousCostPound],
			--CASE 
			--	WHEN x.previousTotalCostPound = 0 THEN 0
			--	ELSE (x.previousTotalCostPound/x.previousPound)
			--END [previousCostPound],
			CASE 
				WHEN ISNULL(x.id_inventoryReason,0) = 0  THEN x.previousTotalCostPound
				ELSE 0
			END AS [previousTotalCostPound],
			--x.previousTotalCostPound,
			-------------------
			--- Ingresos
			x.entradaPound,
			CASE 
				WHEN x.entradaTotalCostPound = 0 THEN 0
				ELSE (x.entradaTotalCostPound/x.entradaPound)
			END [entradaCostPound],
			x.entradaTotalCostPound,
			-------------------
			--- Egresos
			x.salidaPound,
			CASE 
				WHEN x.salidaTotalCostPound = 0 THEN 0
				ELSE (x.salidaTotalCostPound/x.salidaPound)
			END [salidaCostPound],
			x.salidaTotalCostPound,
			-------------------
			--- Saldo FInal
			x.finalPound,
			CASE 
				WHEN x.finalTotalCostPound = 0 THEN 0
				ELSE (x.finalTotalCostPound/x.finalPound)
			END [finalCostPound],
			x.finalTotalCostPound,
			-----------
			-- Forzados
			@startEmissionDate	 [fechaInicio],
			@endEmissionDate	 [fechaFin] ,
			0 [idEstado],
			'' [nombreEstado],
			--- Cabcera
			Substring(x.itemPresentationDescrip,1,100) [itemPresentationDescrip],
			x.oneItemPound
	FROM
	(
		SELECT 
		--------------- CAMPO ORIGINALES
			x.id
			,x.id_document
			,x.document
			,x.nameDocumentState
			,x.id_documentType
			,x.documentType
			,x.id_inventoryReason
			,x.inventoryReason
			,x.emissionDate
			,x.id_item
			,x.code_item
			,x.id_metricUnit
			,x.metricUnit
			,x.id_lot
			,x.number
			,x.internalNumber
			,x.id_warehouse
			,x.warehouse
			,x.id_warehouseLocation
			,x.warehouseLocation
			,x.id_warehouseExit
			,x.warehouseExit
			,x.id_warehouseLocationExit
			,x.warehouseLocationExit
			,x.id_warehouseEntry
			,x.warehouseEntry
			,x.id_warehouseLocationEntry
			,x.warehouseLocationEntry
			,x.previousBalance
			,x.[entry] -- Cantidad Ingreso
			,x.[exit]   -- Cantidad Egreso
			,x.balance			   -- Cantridad Previa
			,x.balanceCutting
			,x.numberRemissionGuide
			,x.idCompany
			,x.nameBranchOffice
			,x.nameDivision
			,x.nameCompany
			,x.dateCreate
			,x.Provider_name
			,x.isCopacking
			,x.nameProviderShrimp
			,x.productionUnitProviderPool
			,x.itemSize
			,x.itemType
			,x.ItemMetricUnit
			,x.ItemPresentationValue,
			x.amount, -- Cantidad
			x.amountCostUnit, -- CostoUnitario
			x.amountCostTotal,
			x.MetricUnitId,
			--------------- CALCULADOS
			-------------------
			--- Saldo Inicial
			CASE 
			  WHEN Y.factor IS NULL THEN ( x.previousBalance * x.ItemPresentationValue ) 
			  ELSE ( x.previousBalance * x.ItemPresentationValue * Y.factor) 
			END AS [previousPound],
			(x.previousBalance * x.amountCostUnit) [previousTotalCostPound],
			-------------------
			--- Ingresos
			CASE 
			  WHEN Y.factor IS NULL THEN ( x.entry * x.ItemPresentationValue ) 
			  ELSE ( x.entry * x.ItemPresentationValue * Y.factor) 
			END AS [entradaPound],
			(x.entry * x.amountCostUnit) [entradaTotalCostPound],
			-------------------
			--- Egresos
			CASE 
			  WHEN Y.factor IS NULL THEN ( x.[exit] * x.ItemPresentationValue ) 
			  ELSE ( x.[exit] * x.ItemPresentationValue * Y.factor) 
			END AS [salidaPound],
			(x.[exit] * x.amountCostUnit) [salidaTotalCostPound],
			-------------------
			--- Saldo FInal
			CASE 
			  WHEN Y.factor IS NULL THEN ( x.balance * x.ItemPresentationValue ) 
			  ELSE ( x.balance * x.ItemPresentationValue * Y.factor) 
			END AS [finalPound],
			(x.balance * x.amountCostUnit) [finalTotalCostPound],
			x.itemPresentationDescrip,
			-------------------
			--- Peso Unidad
			CASE 
			  WHEN Y.factor IS NULL THEN (x.ItemPresentationValue ) 
			  ELSE (  x.ItemPresentationValue * Y.factor) 
			END AS [oneItemPound]
		FROM
		(
			SELECT A.id
				  ,A.id_inventoryMove id_document
				  ,CAST(ISNULL(C.sequential, 0) AS varchar(100)) document
				  ,P.[name] nameDocumentState
				  ,D.id_documentType
				  ,E.[name] documentType
				  ,C.id_inventoryReason
				  ,F.[name] inventoryReason
				  ,D.emissionDate
				  ,A.id_item
				  ,(G.masterCode + ' - ' + G.[name]) code_item
				  ,A.id_metricUnit
				  ,H.code metricUnit
				  ,A.id_lot
				  ,(IIF((ISNULL(I.number, '') = ''), ISNULL(I.internalNumber, ''), (I.number + IIF(ISNULL(Pl.internalNumber, ISNULL(I.internalNumber, '')) = '', '', ' - ' + ISNULL(Pl.internalNumber, I.internalNumber))))) number
				  ,Isnull(Pl.internalNumber,I.internalNumber) internalNumber
				  ,A.id_warehouse
				  ,J.[name] warehouse
				  ,A.id_warehouseLocation
				  ,K.[name] warehouseLocation
				  ,1 id_warehouseExit
				  ,'' warehouseExit
				  ,2 id_warehouseLocationExit
				  ,'' warehouseLocationExit
				  ,2 id_warehouseEntry
				  ,'' warehouseEntry
				  ,(IIF((A.entryAmount > 0), A.id_warehouseLocation, NULL)) id_warehouseLocationEntry
				  ,(IIF((A.entryAmount > 0), (ISNULL(K.[name], '')), '')) warehouseLocationEntry
				  ,B.previousBalance
				  ,A.entryAmount [entry]
				  ,A.exitAmount [exit]
				  ,B.balance
				  ,B.balanceCutting
				  ,B.numberRemissionGuide
				  ,L.id_company idCompany
				  ,M.[name] nameBranchOffice
				  ,N.[name] nameDivision
				  ,O.businessName nameCompany
				  ,A.dateCreate
				  ,PProveedor.fullname_businessName Provider_name
				  ,isnull(PProveedor.isCopacking,0) isCopacking
				  ,PLPUP.name nameProviderShrimp
				  ,PLPUPP.name productionUnitProviderPool
				  ,[itemSize].[name] itemSize
				  ,[itemType].[name] itemType
				  ,[presentationMetricUnit].[code] ItemMetricUnit
				  ,[presentation].[minimum] * [presentation].[maximum] ItemPresentationValue,
				  (ISNULL(A.entryAmount,0)-ISNULL(A.exitAmount,0) ) [amount], -- Cantidad
				  ISNULL(A.unitPriceMove,0) [amountCostUnit], -- CostoUnitario
				  ((ISNULL(A.entryAmount,0)-ISNULL(A.exitAmount,0) ) * ISNULL(A.unitPriceMove,0)) [amountCostTotal],
				  presentationMetricUnit.id [MetricUnitId],
				  presentation.description [itemPresentationDescrip]
			FROM [dbo].[InventoryMoveDetail] A WITH (NOLOCK)
			INNER JOIN #TmpIdentificadoresInventoryMoveDetailReduced B ON (B.id = A.id) AND (B.codeDocumentState = IIF((@codeReport = 'IMIPV1'), (B.codeDocumentState), ('03')))--'03' APROBADA
			--AND (((B.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (B.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
			INNER JOIN dbo.InventoryMove C WITH (NOLOCK) ON (C.id = A.id_inventoryMove)
			INNER JOIN dbo.Document D WITH (NOLOCK) ON (D.id = C.id)
			INNER JOIN dbo.DocumentState P WITH (NOLOCK) ON (P.id = D.id_documentState)
			INNER JOIN dbo.DocumentType E WITH (NOLOCK) ON (E.id = D.id_documentType)
			LEFT JOIN dbo.InventoryReason F WITH (NOLOCK) ON (F.id = C.id_inventoryReason)
			INNER JOIN dbo.Item G WITH (NOLOCK) ON (G.id = A.id_item)
			INNER JOIN dbo.MetricUnit H WITH (NOLOCK) ON (H.id = A.id_metricUnit)
			LEFT JOIN dbo.Lot I WITH (NOLOCK) ON (I.id = A.id_lot)
			INNER JOIN dbo.Warehouse J WITH (NOLOCK) ON (J.id = A.id_warehouse)
			LEFT JOIN dbo.WarehouseLocation K WITH (NOLOCK) ON (K.id = A.id_warehouseLocation)
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
		) X LEFT JOIN MetricUnitConversion Y
		  ON X.MetricUnitId = Y.id_metricOrigin
		  WHERE Y.id_metricDestiny = @UnidadMedidaLbsId
	 ) X
		END
END

/*
-- Pruebas:			
EXEC [dbo].[inv_Consultar_Kardex_Saldo_Costo_StoredProcedure] '{"id_documentType":0,"number":"Todos","reference":"Todas","startEmissionDate":"2021-07-01T00:00:00","endEmissionDate":"2021-07-31T00:00:00","idNatureMove":0,"id_inventoryReason":0,"id_warehouseExit":0,"id_warehouseLocationExit":0,"id_dispatcher":0,"id_warehouseEntry":22,"id_warehouseLocationEntry":0,"id_receiver":0,"numberLot":"Todos","internalNumberLot":"Todos","items":"Todos","id_user":0,"codeReport":"KEDST"}'
*/