	
create or ALTER Procedure [dbo].[inv_Genera_Saldo_Mes_Control]
(
	@ParametrosBusquedaKardexSaldo		nVarChar(Max),
	@printDebug		Bit = 0
)
As
Begin

	Set NoCount On

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Iniciando la ejecución del procedimiento...'
	End

	--==================================================================================================
	-- MAPEO DE PARÁMETROS
	--==================================================================================================
	Declare	@id_company					Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_company'),
			@id_documentType			Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_documentType'),
			@number						VarChar(Max)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.number'),
			@reference					VarChar(Max)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.reference'),
			@startEmissionDate			DateTime	    = JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.startEmissionDate'),
			@endEmissionDate			DateTime	    = JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.endEmissionDate'),
			@idNatureMove				Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.idNatureMove'),
			@id_inventoryReason			Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_inventoryReason'),
			@id_warehouseExit			Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseExit'),
			@id_warehouseLocationExit	Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseLocationExit'),
			@id_dispatcher				Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_dispatcher'),
			@id_warehouseEntry			Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseEntry'),
			@id_warehouseLocationEntry	Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_warehouseLocationEntry'),
			@id_receiver				Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_receiver'),
			@numberLot					VarChar(Max)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.numberLot'),
			@internalNumberLot			VarChar(Max)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.internalNumberLot'),
			@lotMarked					VarChar(Max)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.lotMarked'),
			@items						VarChar(Max)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.items'),
			@id_user					Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_user'),
			@codeReport					VarChar(Max)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.codeReport'),
			@codeTypePeriod				Char(1)			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.codeTypePeriod');


	--==================================================================================================
	-- OBTENCION PARAMETRO SALDO INICIAL Y CONSULTA DE SALDO DESDE MOVIMIENTOS
	--==================================================================================================
	Declare @setting_periodo_inicial	varchar(6) = (select top 1 [value]  from Setting where code = 'PSALINI');	
	DECLARE @CODIGO_ESTADO_APROBADO		VARCHAR(2) = '03';
	DECLARE @CODIGO_ESTADO_CONCILIADO	VARCHAR(2) = '16';
	DECLARE @es_saldo_inicial		bit = 0;
	DECLARE @anio_saldo_inicial		int;
	DECLARE @mes_saldo_inicial		int;
	DECLARE @fechaInicialBalance	DateTime;
	declare @periodo_anterior		VARCHAR(6);

	SET @anio_saldo_inicial = SUBSTRING(@setting_periodo_inicial,1,4);
	SET @mes_saldo_inicial = SUBSTRING(@setting_periodo_inicial,5,2);
	SET @fechaInicialBalance =  DATEFROMPARTS(@anio_saldo_inicial, @mes_saldo_inicial, 1);
	SET @periodo_anterior = SUBSTRING(trim(CONVERT(varchar, DATEADD(month,-1,@startEmissionDate) ,112)),1,6 );
	--SUBSTRING(trim(CONVERT(varchar, @startEmissionDate,112)),1,6 ) 

	IF(YEAR(@startEmissionDate) = @anio_saldo_inicial AND MONTH(@startEmissionDate)= @mes_saldo_inicial)
	BEGIN
		SET @es_saldo_inicial = 1;
	END

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Preparando la lista de ítems...'
	End

	Declare @ids_items Table ( id_item Int Primary Key );

	If IsNull(@items, 'Todos') != 'Todos'
	Begin
		Insert	Into @ids_items
		Select	Cast(Value As Int)
		From	STRING_SPLIT(@items, ',');
	End


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Recuperando el parámetro WAH...'
	End
	
	Declare	@configuradoWAH	Int;
	Select	@configuradoWAH = Count(1)
	From	dbo.UserEntityDetail A
			Inner Join dbo.UserEntity B On B.id = A.id_userEntity
			Inner Join dbo.Entity C On C.id = B.id_entity
	Where	B.id_user = @id_user
	And		C.code = 'WAH';	--Entidad de Bodega del Sistema


	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL: #TmpUserEntityDetail
	--==================================================================================================

	DECLARE @WarehouseLocation AS TABLE(
	Id int,
	Id_warehouse Int,
	Name varchar(200)
	)

	INSERT INTO @WarehouseLocation
	SELECT Id,id_warehouse, name 
	FROM WarehouseLocation 
	WHERE  id_company = @id_company


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Recuperando código de entidad de bodega...'
	End

	Create Table #TmpUserEntityDetail
	(
		id_entityValue	Int Primary Key
	);

	Insert	Into #TmpUserEntityDetail
	(
		id_entityValue
	)
	Select	Distinct A.id_entityValue
	From	dbo.UserEntityDetail A
			Inner Join dbo.UserEntity B On B.id = A.id_userEntity
			Inner Join dbo.Entity C On C.id = B.id_entity
			Inner Join dbo.UserEntityDetailPermission D On D.id_userEntityDetail = A.id
			Inner Join dbo.Permission E On E.id = D.id_permission
	Where	B.id_user = @id_user
	And		C.code = 'WAH'			--Entidad de Bodega del Sistema
	And		E.[name] = 'Visualizar';	--Permiso para visualizar


	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL: #TmpIdentificadoresInventoryMoveDetail
	--==================================================================================================
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' ! Preparando #TmpIdentificadoresInventoryMoveDetail !!! ...'
	End

	Create Table #TmpIdentificadoresInventoryMoveDetail
	(
		id					Int Primary Key,
		id_document			Int,
		emissionDate		DateTime,
		anio				Int,
		periodoNumber		Int,
		sequential			Int,
		id_item				Int,
		id_metricUnit		Int,
		id_warehouse		Int,
		id_warehouseLocation Int,
		id_lot				Int,
		entryAmount			Decimal(14,6),
		exitAmount			Decimal(14,6),
		id_inventoryMove	Int,
		previousBalance		Decimal(14,6),
		balance				Decimal(14,6),
		balanceCutting		Decimal(14,6),
		numberRemissionGuide VarChar(100),
		codeDocumentState	VarChar(20),
		dateCreate			DateTime,
		lotMarked			VarChar(20),
		id_ProductionLot	Int,
		sequencial_productionLot nvarchar(20),
		number_productionLot nvarchar(20)
	);
	print 'Tabla 1'
	
	Insert	Into #TmpIdentificadoresInventoryMoveDetail
	(
		id,
		id_document,
		emissionDate,
		anio,
		periodoNumber,
		sequential,
		id_item,
		id_metricUnit,
		id_warehouse,
		id_warehouseLocation,
		id_lot,
		entryAmount,
		exitAmount,
		id_inventoryMove,
		previousBalance,
		balance,
		balanceCutting,
		numberRemissionGuide,
		codeDocumentState,
		dateCreate,
		lotMarked,
		id_ProductionLot,
		sequencial_productionLot,
		number_productionLot
	)
	Select	A.id,
			A.id_inventoryMove As id_document,
			C.emissionDate,
			Year(C.emissionDate), 
			dbo.inv_getPeriodNumber(@codeTypePeriod,C.emissionDate),
			B.sequential,
			A.id_item,
			A.id_metricUnit,
			A.id_warehouse,
			A.id_warehouseLocation,
			A.id_lot,
			A.entryAmount,
			A.exitAmount,
			A.id_inventoryMove,
			0 As previousBalance,
			0 As balance,
			0 As balanceCutting,
			'' As numberRemissionGuide,
			D.code As codeDocumentState,
			A.dateCreate,
			Isnull(A.lotMarked, ''),
			G.id,
			G.number,
			G.internalNumber
	From	dbo.InventoryMoveDetail A
			Inner Join dbo.InventoryMove B On (B.id = A.id_inventoryMove)
			Inner Join 
			(
			  SELECT		d.id ,
							d.id_documentState,
							d.emissionDate,
							d.id_documentType,
							d.number,
							d.reference
			  FROM			dbo.Document d 
			  INNER JOIN	EmissionPoint e 
			  on e.id = d.id_emissionPoint
			  WHERE e.id_company = @id_company
			  ) C On (C.id = B.id)
			Inner Join dbo.DocumentState D On (D.id = C.id_documentState)
			left Join 
			(
				Select		G.id,
							G.number,
							ISNULL(G.internalNumber,H.internalNumber) AS internalNumber
				From dbo.Lot G 
				left join dbo.ProductionLot H On
				(H.id = G.id)
			) G ON (G.id = A.id_lot)			
	Where	(((@startEmissionDate Is Null Or C.emissionDate >= @startEmissionDate) And (@endEmissionDate Is Null Or C.emissionDate < DateAdd(day, 1, @endEmissionDate))))	
	And		(IsNull(@id_warehouseExit, 0) = 0 Or A.id_warehouse = @id_warehouseExit)
	And		(IsNull(@id_warehouseLocationExit, 0) = 0 Or A.id_warehouseLocation = @id_warehouseLocationExit)
	And		(IsNull(@id_warehouseEntry, 0) = 0 Or A.id_warehouse = @id_warehouseEntry)
	And		(IsNull(@id_warehouseLocationEntry, 0) = 0 Or A.id_warehouseLocation = @id_warehouseLocationEntry)	
	And		(IsNull(@items, 'Todos') = 'Todos' Or A.id_item IN (Select id_item From @ids_items))
	And		(@configuradoWAH = 0 Or (A.id_warehouse In ( Select id_entityValue From #TmpUserEntityDetail))); --Entidad de Bodega del Sistema

	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL: #TmpIdentificadoresInventoryMoveDetailReduced
	--==================================================================================================
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Preparando #TmpIdentificadoresInventoryMoveDetailReduced...'
	End
	print 'Tabla 2'
	Create Table #TmpIdentificadoresInventoryMoveDetailReduced
	(
		id					Int Primary Key,
		id_document			Int,
		emissionDate		DateTime,
		anio				Int,
		periodoNumber		Int,
		sequential			Int,
		id_item				Int,
		id_metricUnit		Int,
		id_warehouse		Int,
		id_warehouseLocation Int,
		id_lot				Int,
		entryAmount			Decimal(14,6),
		exitAmount			Decimal(14,6),
		id_inventoryMove	Int,
		previousBalance		Decimal(14,6),
		balance				Decimal(14,6),
		balanceCutting		Decimal(14,6),
		numberRemissionGuide VarChar(100),
		codeDocumentState	VarChar(20),
		dateCreate			DateTime,
		lotMarked			VarChar(20),
		id_ProductionLot	Int,
		sequencial_productionLot nvarchar(20),
		number_productionLot nvarchar(20)
	);

	Insert	Into #TmpIdentificadoresInventoryMoveDetailReduced
	(
		id,
		id_document,
		emissionDate,
		anio,
		periodoNumber,
		sequential,
		id_item,
		id_metricUnit,
		id_warehouse,
		id_warehouseLocation,
		id_lot,
		entryAmount,
		exitAmount,
		id_inventoryMove,
		previousBalance,
		balance,
		balanceCutting,
		numberRemissionGuide,
		codeDocumentState,
		dateCreate,
		lotMarked,
		id_ProductionLot,
		sequencial_productionLot,
		number_productionLot
	)
	Select	id,
			id_document,
			emissionDate,
			anio,
			periodoNumber,
			sequential,
			id_item,
			id_metricUnit,
			id_warehouse,
			id_warehouseLocation,
			id_lot,
			entryAmount,
			exitAmount,
			id_inventoryMove,
			previousBalance,
			balance,
			balanceCutting,
			numberRemissionGuide,
			codeDocumentState,
			dateCreate,
			lotMarked,
			id_ProductionLot,
			sequencial_productionLot,
			number_productionLot 
	From	#TmpIdentificadoresInventoryMoveDetail
	Where (((@startEmissionDate Is Null Or emissionDate >= @startEmissionDate) And (@endEmissionDate Is Null Or emissionDate < DateAdd(day, 1, @endEmissionDate))))
	AND emissionDate >= @fechaInicialBalance;

	
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Preparando índices en tabla #TmpIdentificadoresInventoryMoveDetailReduced...'
	End

	Create Index IDX_TmpIdentificadoresInventoryMoveDetailReduced_items On #TmpIdentificadoresInventoryMoveDetailReduced (id_item, id_warehouse, id_warehouseLocation);
	Create Index IDX_TmpIdentificadoresInventoryMoveDetailReduced_inventoryMove On #TmpIdentificadoresInventoryMoveDetailReduced (id_inventoryMove);


	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL: #TmpInventoryMoveDetailWithPreviousBalance
	--==================================================================================================
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Preparando #TmpInventoryMoveDetailWithPreviousBalance...'
	End

	Create Table #TmpInventoryMoveDetailWithPreviousBalance
	(
		id					Int Primary Key,
		previousBalance		Decimal(14,6),
		previousBalanceCutting Decimal(14,6),
		id_item				Int,
		id_warehouse		Int,
		id_warehouseLocation Int,
		id_lot				Int,
		emissionDate		DateTime,
		dateCreate			DateTime,
		id_ProductionLot	Int,
		sequencial_productionLot nvarchar(20),
		number_productionLot nvarchar(20)
	);

	If @printDebug = 1
	Begin
	    declare @count_TmpIdentificadoresInventoryMoveDetailReduced int = (select count(*) from #TmpIdentificadoresInventoryMoveDetailReduced);
		declare @count_TmpIdentificadoresInventoryMoveDetail int =(select count(*) from #TmpIdentificadoresInventoryMoveDetail);
		Print Convert(VarChar,@count_TmpIdentificadoresInventoryMoveDetailReduced , 114) + ' - Count  #TmpIdentificadoresInventoryMoveDetailReduced...'
		Print Convert(VarChar, @count_TmpIdentificadoresInventoryMoveDetail, 114) + ' - Count  #TmpIdentificadoresInventoryMoveDetail...'
	End

	
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Preparando #TmpInventoryMoveDetailWithPreviousBalance...MES CORRIENTE'
	End


	Insert	Into #TmpInventoryMoveDetailWithPreviousBalance
	(
		id,
		previousBalance,
		previousBalanceCutting,
		id_item,
		id_warehouse,
		id_warehouseLocation,		
		id_ProductionLot,
		sequencial_productionLot,
		number_productionLot 
	)
	Select	max(AA.Id),
			Sum( (ISNULL(AA.SaldoAnterior,0) +IsNull(AA.Entrada, 0) ) - IsNull(AA.Salida, 0)) As previoBalance,
			Null							As previousBalanceCutting,
			AA.id_item						As id_item,
			AA.id_warehouse					As id_warehouse,
			AA.id_warehouseLocation			As id_warehouseLocation,
			ISNULL( AA.id_productionLot,0)	As id_lot,			
			max(isnull(AA.sequencial_productionLot,'')),
			max(isnull(AA.number_productionLot,''))	
	From	
			(
			    
				SELECT CAST(Anio AS VARCHAR)+ right( '00' + cast( Periodo AS varchar(2)), 2 ) AS Periodo,
				Id,
				id_company,
				id_item,
				id_warehouse,
				id_warehouseLocation,
				id_productionLot,
				sequencial_productionLot,
				number_productionLot,
				SaldoAnterior,
				Entrada,
				Salida
				FROM MonthlyBalance
			) AA 	
	WHERE	AA.id_company  =  @id_company
			AND AA.Periodo = @periodo_anterior
			And		(IsNull(@id_warehouseExit, 0) = 0 Or AA.id_warehouse= @id_warehouseExit)
			And		(IsNull(@id_warehouseLocationExit, 0) = 0 Or AA.id_warehouseLocation = @id_warehouseLocationExit)
			And		(IsNull(@id_warehouseEntry, 0) = 0 Or AA.id_warehouse = @id_warehouseEntry)
			And		(IsNull(@id_warehouseLocationEntry, 0) = 0 Or AA.id_warehouseLocation = @id_warehouseLocationEntry)	
			And		(IsNull(@items, 'Todos') = 'Todos' Or AA.id_item IN (Select id_item From @ids_items))
			Group By	AA.id_warehouse,
						AA.id_warehouseLocation,
						AA.id_productionLot,
						AA.id_item;
	
	If	@startEmissionDate Is Not Null
	Begin
		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Actualizando #TmpInventoryMoveDetailWithPreviousBalance (Paso 1)...'
		End


		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Preparando índices en tabla #TmpInventoryMoveDetailWithPreviousBalance...'
		End

		Create Index IDX_TmpInventoryMoveDetailWithPreviousBalance_items On #TmpInventoryMoveDetailWithPreviousBalance (id_item, id_warehouse, id_warehouseLocation);
		

		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Actualizando #TmpInventoryMoveDetailWithPreviousBalance (Paso 2)...'
		End
 

		--==================================================================================================
		-- CREACIÓN DE TABLA TEMPORAL: #TmpInventoryMoveDetailToNoShow
		--==================================================================================================
		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Preparando #TmpInventoryMoveDetailToNoShow...'
		End

		Create Table #TmpInventoryMoveDetailToNoShow
		(
			id					Int Primary Key,
			anio				Int,
			periodoNumber		Int,
			id_item				Int,
			id_warehouse		Int,
			id_warehouseLocation Int,
			id_lot				Int,
			id_metricUnit		Int,
			entryAmount			Decimal(14,6),
			exitAmount			Decimal(14,6),
			id_ProductionLot	Int,
			sequencial_productionLot nvarchar(20),
			number_productionLot nvarchar(20)
		);

		Insert	Into #TmpInventoryMoveDetailToNoShow
		(
			id,
			anio,
			periodoNumber,
			id_item,
			id_warehouse,
			id_warehouseLocation,
			id_lot,
			id_metricUnit,
			entryAmount,
			exitAmount,
			id_ProductionLot,
			sequencial_productionLot,
			number_productionLot
		)
		Select	A.id,
				A.anio,
				A.periodoNumber,
				A.id_item,
				A.id_warehouse,
				A.id_warehouseLocation,
				A.id_lot,
				A.id_metricUnit,
				A.entryAmount,
				A.exitAmount,
				A.id_ProductionLot,
				A.sequencial_productionLot,
				A.number_productionLot
		From	#TmpIdentificadoresInventoryMoveDetail A
		Where	A.codeDocumentState in(@CODIGO_ESTADO_APROBADO,@CODIGO_ESTADO_CONCILIADO ) --'03'
		And		(@endEmissionDate Is Null Or A.emissionDate < DateAdd(day, 1, @endEmissionDate));

		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Preparando índices en tabla #TmpInventoryMoveDetailToNoShow...'
		End

		Create Index IDX_TmpInventoryMoveDetailToNoShow On #TmpInventoryMoveDetailToNoShow (id_item, id_warehouse, id_warehouseLocation);


		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Eliminando de #TmpInventoryMoveDetailToNoShow...'
		End

		Delete	A
		From	#TmpInventoryMoveDetailToNoShow A, #TmpIdentificadoresInventoryMoveDetailReduced AA
		Where	AA.id_item = A.id_item
		And		AA.id_warehouse = A.id_warehouse
		And		AA.id_warehouseLocation = A.id_warehouseLocation
		And		IsNull(AA.id_lot, 0) = IsNull(A.id_lot, 0)
		And		AA.codeDocumentState in(@CODIGO_ESTADO_APROBADO,@CODIGO_ESTADO_CONCILIADO ); --'03';


		--==================================================================================================
		-- CREACIÓN DE TABLA TEMPORAL: #TmpInventoryMoveDetailNoShow
		--==================================================================================================
		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Preparando #TmpInventoryMoveDetailNoShow...'
		End

		Create Table #TmpInventoryMoveDetailNoShow
		(
			id					Int Primary Key,
			anio				Int,
			periodoNumber		Int,
			previousBalance		Decimal(14,6),
			id_item				Int,
			id_warehouse		Int,
			id_warehouseLocation Int,
			id_lot				Int,
			id_metricUnit		Int,
			id_ProductionLot	Int,
			sequencial_productionLot nvarchar(20),
			number_productionLot nvarchar(20)
		);

		Insert	Into #TmpInventoryMoveDetailNoShow
		(
			id,
			anio,
			periodoNumber,
			previousBalance,
			id_item,
			id_warehouse,
			id_warehouseLocation,
			id_lot,
			id_metricUnit,
			id_ProductionLot,
			sequencial_productionLot,
			number_productionLot
		)
		Select	Max(A.id) As id,
				A.anio,
				A.periodoNumber,
				Sum(IsNull(A.entryAmount, 0) - IsNull(A.exitAmount, 0)) As previoBalance,
				A.id_item,
				A.id_warehouse,
				A.id_warehouseLocation,
				A.id_lot,
				max(A.id_metricUnit),
				max(A.id_ProductionLot),
				max(A.sequencial_productionLot),
				max(A.number_productionLot) as number_productionLot 
		From	#TmpInventoryMoveDetailToNoShow A
		--Group	By A.anio,A.periodoNumber,A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot, A.id_metricUnit, a.id_ProductionLot, A.sequencial_productionLot;
		Group	By A.anio,A.periodoNumber,A.id_warehouse, A.id_warehouseLocation, A.id_lot,A.id_item;
	End

	--Actualizar previousBalance y balanceCutting
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando previousBalance y balanceCutting...'
	End


	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL: #TmpResults
	--==================================================================================================
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Preparando tabla de resultados...'
	End

	Create Table #TmpResults
	(
		id						Int,
		anio					Int,
		periodoNumber			Int,
		id_document				Int,
		document				VarChar(Max),
		id_documentState		Int,
		nameDocumentState		VarChar(Max) Collate DATABASE_DEFAULT,
		id_documentType			Int,
		documentType			VarChar(Max) Collate DATABASE_DEFAULT,
		id_inventoryReason		Int,
		inventoryReason			VarChar(Max) Collate DATABASE_DEFAULT,
		emissionDate			DateTime,
		id_item					Int,
		code_item				VarChar(Max) Collate DATABASE_DEFAULT,
		id_metricUnit			Int,
		metricUnit				VarChar(20),
		id_lot					Int,
		number					VarChar(Max) Collate DATABASE_DEFAULT,
		internalNumber			VarChar(Max) Collate DATABASE_DEFAULT,
		numberLot				VarChar(20) Collate DATABASE_DEFAULT,
		internalNumberLot		VarChar(20) Collate DATABASE_DEFAULT,
		id_warehouse			Int,
		warehouse				VarChar(50) Collate DATABASE_DEFAULT,
		id_warehouseLocation	Int,
		warehouseLocation		VarChar(250) Collate DATABASE_DEFAULT,
		id_warehouseExit		Int,
		warehouseExit			VarChar(Max) Collate DATABASE_DEFAULT,
		id_warehouseLocationExit Int,
		warehouseLocationExit	VarChar(Max) Collate DATABASE_DEFAULT,
		id_warehouseEntry		Int,
		warehouseEntry			VarChar(Max) Collate DATABASE_DEFAULT,
		id_warehouseLocationEntry Int,
		warehouseLocationEntry	VarChar(Max) Collate DATABASE_DEFAULT,
		previousBalance			Decimal(14,6),
		[entry]					Decimal(14,6),
		[exit]					Decimal(14,6),
		balance					Decimal(14,6),
		balanceCutting			Decimal(14,6),
		numberRemissionGuide	VarChar(100),
		idCompany				Int,
		nameBranchOffice		VarChar(50),
		nameDivision			VarChar(Max) Collate DATABASE_DEFAULT,
		nameCompany				VarChar(Max) Collate DATABASE_DEFAULT,
		id_emissionPoint		Int,
		dateCreate				DateTime,
		Provider_name			VarChar(Max) Collate DATABASE_DEFAULT,
		isCopacking				Bit,
		id_provider				Int,
		id_ProductionLot		Int,
		id_productionUnitProvider Int,
		id_productionUnitProviderPool Int,
		nameProviderShrimp		VarChar(Max) Collate DATABASE_DEFAULT,
		productionUnitProviderPool VarChar(Max) Collate DATABASE_DEFAULT,
		itemSize				VarChar(50) Collate DATABASE_DEFAULT,
		id_itemType				Int,
		itemType				VarChar(50) Collate DATABASE_DEFAULT,
		id_presentation			Int,
		id_presentacionMetricUnit Int,
		ItemMetricUnit			VarChar(20) Collate DATABASE_DEFAULT,
		ItemPresentationValue	Decimal(14,6),
		lotMarked				VarChar(20) Collate DATABASE_DEFAULT,
		sequencial_productionLot nvarchar(20),
		number_productionLot	nvarchar(20)
	);

	If	@startEmissionDate Is Not Null
	Begin
		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Cargando datos iniciales a tabla de resultados - FASE (1/2)...'
		End

		Insert	Into #TmpResults
		(
			id,
			anio,
			periodoNumber,
			id_document,
			id_item,
			id_metricUnit,
			id_lot,
			id_warehouse,
			id_warehouseLocation,
			id_warehouseExit,
			warehouseExit,
			id_warehouseLocationExit,
			warehouseLocationExit,
			id_warehouseEntry,
			warehouseEntry,
			id_warehouseLocationEntry,
			previousBalance,
			[entry],
			[exit],
			balance,
			balanceCutting,
			numberRemissionGuide,
			dateCreate,
			lotMarked,
			id_ProductionLot,
			sequencial_productionLot,
			number_productionLot
		)
		Select	id,
				anio,
				periodoNumber,
				id_document,
				id_item,
				id_metricUnit,
				id_lot,
				id_warehouse,
				id_warehouseLocation,
				1 As id_warehouseExit,
				'' As warehouseExit,
				2 As id_warehouseLocationExit,
				'' As warehouseLocationExit,
				2 As id_warehouseEntry,
				'' As warehouseEntry,
				Case When (entryAmount > 0) Then id_warehouseLocation Else Null End As id_warehouseLocationEntry,
				previousBalance,
				entryAmount,
				exitAmount,
				balance,
				balanceCutting,
				numberRemissionGuide,
				dateCreate,
				lotMarked,
				id_ProductionLot,
				sequencial_productionLot,
				number_productionLot
		From	#TmpIdentificadoresInventoryMoveDetailReduced
		Where	(@codeReport = 'IMIPV1' Or codeDocumentState in(@CODIGO_ESTADO_APROBADO,@CODIGO_ESTADO_CONCILIADO) );--'03');	--'03' APROBADA

		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Cargando datos iniciales a tabla de resultados - FASE (2/2)...'
		End


	End
	Else
	Begin
		If @printDebug = 1
		Begin
			Print Convert(VarChar, GetDate(), 114) + ' - Cargando datos iniciales a tabla de resultados...'
		End

		Insert	Into #TmpResults
		(
			id,
			anio,
			periodoNumber,
			id_document,
			id_item,
			id_metricUnit,
			id_lot,
			id_warehouse,
			id_warehouseLocation,
			id_warehouseExit,
			warehouseExit,
			id_warehouseLocationExit,
			warehouseLocationExit,
			id_warehouseEntry,
			warehouseEntry,
			id_warehouseLocationEntry,
			previousBalance,
			[entry],
			[exit],
			balance,
			balanceCutting,
			numberRemissionGuide,
			dateCreate,
			lotMarked,
			id_ProductionLot,
			sequencial_productionLot,
			number_productionLot
		)
		Select	id,
				anio,
				periodoNumber,
				id_document,
				id_item,
				id_metricUnit,
				id_lot,
				id_warehouse,
				id_warehouseLocation,
				1 As id_warehouseExit,
				'' As warehouseExit,
				2 As id_warehouseLocationExit,
				'' As warehouseLocationExit,
				2 As id_warehouseEntry,
				'' As warehouseEntry,
				Case When (entryAmount > 0) Then id_warehouseLocation Else Null End As id_warehouseLocationEntry,
				previousBalance,
				entryAmount,
				exitAmount,
				balance,
				balanceCutting,
				numberRemissionGuide,
				dateCreate,
				lotMarked,
				id_ProductionLot,
				sequencial_productionLot,
				number_productionLot
		From	#TmpIdentificadoresInventoryMoveDetailReduced
		Where	(@codeReport = 'IMIPV1' Or codeDocumentState in (@CODIGO_ESTADO_APROBADO,@CODIGO_ESTADO_CONCILIADO)); --'03');	--'03' APROBADA
	End

	-- Completamos la información de salida
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Creando índices de primera carga de datos iniciales...'
	End

	Create Index IDX_TmpResults_PK On #TmpResults (id);
	Create Index IDX_TmpResults_document On #TmpResults (id_document);
	Create Index IDX_TmpResults_item On #TmpResults (id_item);
	Create Index IDX_TmpResults_metricUnit On #TmpResults (id_metricUnit);
	Create Index IDX_TmpResults_lot On #TmpResults (id_lot);
	Create Index IDX_TmpResults_warehouse On #TmpResults (id_warehouse);
	Create Index IDX_TmpResults_warehouseLocation On #TmpResults (id_warehouseLocation);
	Create Index IDX_TmpResults_warehouseLocationEntry On #TmpResults (id_warehouseLocationEntry);

	-- Agregamos campos informativos de documento...
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' ! Actualizando datos de documento (Paso 1) !!! ...'
	End

	Update	TMP
	Set		TMP.document = IsNull(C.sequential, 0),
			TMP.id_inventoryReason = C.id_inventoryReason
	From	#TmpResults TMP
			Inner Join dbo.InventoryMove C On (C.id = TMP.id_document);

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Creando índices de datos de documento (Paso 1)...'
	End

	Create Index IDX_TmpResults_inventoryReason On #TmpResults (id_inventoryReason);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' ! Actualizando datos de documento (Paso 2) !!! ...'
	End

	Update	TMP
	Set		TMP.id_documentType = D.id_documentType,
			TMP.id_documentState = D.id_documentState,
			TMP.id_emissionPoint = D.id_emissionPoint,
			TMP.emissionDate = D.emissionDate
	From	#TmpResults TMP
			Inner Join dbo.Document D On (D.id = TMP.id_document);

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Creando índices de datos de documento (Paso 2)...'
	End

	Create Index IDX_TmpResults_documentType On #TmpResults (id_documentType);
	Create Index IDX_TmpResults_documentState On #TmpResults (id_documentState);
	Create Index IDX_TmpResults_emissionPoint On #TmpResults (id_emissionPoint);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de documento (Paso 3)...'
	End

	Update	TMP
	Set		TMP.nameDocumentState = P.[name],
			TMP.documentType = E.[name]
	From	#TmpResults TMP
			Inner Join dbo.DocumentState P On (P.id = TMP.id_documentState)
			Inner Join dbo.DocumentType E On (E.id = TMP.id_documentType);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de documento (Paso 4)...'
	End

	Update	TMP
	Set		TMP.inventoryReason = F.[name]
	From	#TmpResults TMP
			Inner Join dbo.InventoryReason F On (F.id = TMP.id_inventoryReason);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de documento (Paso 5)...'
	End

	Update	TMP
	Set		TMP.idCompany = L.id_company,
			TMP.nameBranchOffice = M.[name],
			TMP.nameDivision = N.[name],
			TMP.nameCompany = O.businessName
	From	#TmpResults TMP
			Inner Join dbo.EmissionPoint L On (L.id = TMP.id_emissionPoint)
			Inner Join dbo.BranchOffice M On (M.id = L.id_branchOffice)
			Inner Join dbo.Division N On (N.id = L.id_division)
			Inner Join dbo.Company O On (O.id = L.id_company);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de documento (Paso 6)...'
	End

	Update	TMP
	Set		TMP.warehouse = J.[name]
	From	#TmpResults TMP
			Inner Join dbo.Warehouse J On (J.id = TMP.id_warehouse);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de documento (Paso 7)...'
	End

	Update	TMP
	Set		TMP.warehouseLocation = K.[name]
	From	#TmpResults TMP
			Inner Join dbo.WarehouseLocation K On (K.id = TMP.id_warehouseLocation);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de documento (Paso 8)...'
	End

	Update	#TmpResults
	Set		warehouseLocationEntry = Case When [entry] > 0 Then warehouseLocation Else '' End;

	-- Agregamos campos informativos del ítem...
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de ítem (Paso 1)...'
	End

	Update	TMP
	Set		TMP.metricUnit = H.code
	From	#TmpResults TMP
			Inner Join dbo.MetricUnit H On (H.id = TMP.id_metricUnit);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de ítem (Paso 2)...'
	End

	Update	TMP
	Set		TMP.code_item = G.masterCode + ' - ' + G.[name],
			TMP.id_itemType = G.id_itemType,
			TMP.id_presentation = G.id_presentation
	From	#TmpResults TMP
			Inner Join dbo.Item G On (G.id = TMP.id_item);

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Creando índices de datos de ítem (Paso 2)...'
	End

	Create Index IDX_TmpResults_itemType On #TmpResults (id_itemType);
	Create Index IDX_TmpResults_presentation On #TmpResults (id_presentation);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de ítem (Paso 3)...'
	End

	Update	TMP
	Set		TMP.itemType = itemType.[name]
	From	#TmpResults TMP
			Inner Join dbo.ItemType itemType On (TMP.id_itemType = itemType.id);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de ítem (Paso 4)...'
	End

	Update	TMP
	Set		TMP.itemSize = itemSize.[name]
	From	#TmpResults TMP
			Inner Join dbo.ItemGeneral itemGeneral On (itemGeneral.id_Item = TMP.id_item)
			Inner Join dbo.ItemSize itemSize On (itemSize.id = itemGeneral.id_size);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de ítem (Paso 5)...'
	End

	Update	TMP
	Set		TMP.id_presentacionMetricUnit = presentation.id_metricUnit,
			TMP.ItemPresentationValue = presentation.minimum * presentation.maximum
	From	#TmpResults TMP
			Inner Join dbo.presentation presentation On (TMP.id_presentation = presentation.id);

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Creando índices de datos de ítem (Paso 5)...'
	End

	Create Index IDX_TmpResults_presentacionMetricUnit On #TmpResults (id_presentacionMetricUnit);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de ítem (Paso 6)...'
	End

	Update	TMP
	Set		TMP.ItemMetricUnit = presentationMetricUnit.code
	From	#TmpResults TMP
			Inner Join dbo.MetricUnit presentationMetricUnit On (TMP.id_presentacionMetricUnit = presentationMetricUnit.id);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' ! Actualizando datos de lotes (Paso 1) !!! ...'
	End

	Update	TMP
	Set		TMP.number =
			Case
				When (I.number Is Not Null And I.internalNumber Is Null) Then (I.number)
				When (I.number Is Not Null And I.internalNumber Is Not Null) Then (I.number + ' - ' + I.internalNumber)
				Else IsNull(I.internalNumber, '')
			End,
			TMP.numberLot = I.number,
			TMP.internalNumberLot = I.internalNumber
	From	#TmpResults TMP
			Inner Join dbo.Lot I On (I.id = TMP.id_lot);

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Creando índices de datos de lotes (Paso 1)...'
	End

	Create Index IDX_TmpResults_numberLot On #TmpResults (numberLot);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' ! Actualizando datos de lotes (Paso 2) !!! ...'
	End

	Update	TMP
	Set		TMP.number =
			Case
				When (TMP.numberLot Is Not Null And PL.internalNumber Is Not Null) Then (TMP.numberLot + ' - ' + PL.internalNumber)
				When (TMP.numberLot Is Null  And PL.internalNumber Is Not Null) Then PL.internalNumber
				Else TMP.number
			End,
			TMP.internalNumber = IsNull(PL.internalNumber, TMP.internalNumberLot),
			TMP.id_ProductionLot = PL.id,
			TMP.id_provider	= PL.id_provider,
			TMP.id_productionUnitProvider = PL.id_productionUnitProvider,
			TMP.id_productionUnitProviderPool = PL.id_productionUnitProviderPool
	From	#TmpResults TMP
			Inner Join dbo.ProductionLot PL On (PL.id = TMP.id_lot);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Creando índices de datos de proveedor (Paso 1)...'
	End

	Create Index IDX_TmpResults_provider On #TmpResults (id_provider);
	Create Index IDX_TmpResults_productionUnitProvider On #TmpResults (id_productionUnitProvider);
	Create Index IDX_TmpResults_productionUnitProviderPool On #TmpResults (id_productionUnitProviderPool);


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando datos de proveedor (Paso 1)...'
	End

	Update	TMP
	Set		TMP.Provider_name = PProveedor.fullname_businessName,
			TMP.isCopacking = IsNull(PProveedor.isCopacking, 0),
			TMP.nameProviderShrimp = PLPUP.[name],
			TMP.productionUnitProviderPool = PLPUPP.[name]
	From	#TmpResults TMP
			Left Join dbo.Person PProveedor On (PProveedor.id = TMP.id_provider) 
			Left Join dbo.ProductionUnitProvider PLPUP on (PLPUP.id = TMP.id_productionUnitProvider)
			Left Join dbo.ProductionUnitProviderPool PLPUPP  on (PLPUPP.id = TMP.id_productionUnitProviderPool);


	 
			
	-----------------------------
	--AGRUPACIONES
	-----------------------------
		Create Table #TmpSamResults
	(
		id int,
		Anio int,
		Periodo int,
		id_item int,
		id_warehouse int,
		id_warehouseLocation int,
		id_presentation int,
		SaldoAnterior	Decimal(20,6),
		[Entrada]	Decimal(20,6),
		[Salida]	Decimal(20,6),
		[SaldoActual]	Decimal(20,6),
		"LB_SaldoAnterior" decimal(20,6),
		"LB_Entrada" decimal(20,6),
		"LB_Salida" decimal(20,6),
		"LB_SaldoActual" decimal(20,6),
		id_ProductionLot	Int,
		sequencial_productionLot  nvarchar(20),
		number_productionLot nvarchar(20)
	)
	
	

	insert into #TmpSamResults	
	Select ltrim(ROW_NUMBER() OVER(order BY id_warehouse,id_warehouseLocation,id_ProductionLot, id_item,anio, periodoNumber)),
	anio,periodoNumber,id_item,
	id_warehouse,id_warehouseLocation,id_presentation,
	sum(previousBalance) SaldoAnterior, Sum([entry]) [Entrada], sum([exit]) [Salida]
	, sum([entry]-[exit]) [SaldoActual]
	, 0 as LB_SaldoAnterior, 0 as LB_Entrada, 0 as LB_Salida, 0 as LB_SaldoActual, id_ProductionLot, max(sequencial_productionLot), max(number_productionLot) 
	from #TmpResults  
	group by anio, periodoNumber,id_warehouse,id_warehouseLocation, id_ProductionLot, id_item,id_presentation
	
	--Actualizar previousBalance y balanceCutting
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' - Actualizando previousBalance y balanceCutting...donde existe la relacion'

	End

	Update	A
	Set		A.SaldoAnterior = IsNull(AA.previousBalance, 0),
			A.SaldoActual = IsNull(AA.previousBalance, 0) + (A.[Entrada] - A.[Salida])
	From	#TmpSamResults As A
			inner Join #TmpInventoryMoveDetailWithPreviousBalance AA On 
			A.id_warehouse				=	AA.id_warehouse 
			AND A.id_warehouseLocation	=	AA.id_warehouseLocation	
			AND A.id_ProductionLot		=	AA.id_ProductionLot
			AND A.id_item				=	AA.id_item;
			
   Create Table #TmpNoInPeriod
	(
		id int,		
		id_item int,
		id_warehouse int,
		id_warehouseLocation int,
		id_ProductionLot	Int
	);

	insert into #TmpNoInPeriod
	select  id,
			id_item,
			id_warehouse,
			id_warehouseLocation,
			id_ProductionLot
	from #TmpInventoryMoveDetailWithPreviousBalance


	Delete	A
	From	#TmpNoInPeriod A, #TmpSamResults AA
	Where	AA.id_item = A.id_item
	And		AA.id_warehouse = A.id_warehouse
	And		AA.id_warehouseLocation = A.id_warehouseLocation
	And		IsNull(AA.id_ProductionLot, 0) = IsNull(A.id_ProductionLot, 0);



	insert into #TmpSamResults
	(
		id,
		Anio,
		Periodo,
		id_item,
		id_warehouse,
		id_warehouseLocation,		
		id_ProductionLot,
		sequencial_productionLot,
		number_productionLot,
		SaldoAnterior,		
		[SaldoActual],
		"LB_SaldoAnterior",
		"LB_Entrada",
		"LB_Salida",
		"LB_SaldoActual"		
	)
	select	b.id,
			year(@startEmissionDate),
			month(@startEmissionDate),
			b.id_item,
			b.id_warehouse,
			b.id_warehouseLocation,			
			b.id_ProductionLot,
			b.sequencial_productionLot,
			b.number_productionLot,
			b.previousBalance,
			b.previousBalance,
			0 "LB_SaldoAnterior",
			0 "LB_Entrada",
			0 "LB_Salida",
			0 "LB_SaldoActual"
	from  #TmpNoInPeriod  a
	inner join   #TmpInventoryMoveDetailWithPreviousBalance b
	on  a.id_item = b.id_item
		and a.id_warehouse = b.id_warehouse
		and a.id_warehouseLocation = b.id_warehouseLocation
		and isnull(a.id_ProductionLot,0) = isnull(b.id_ProductionLot,0)


	Update	TMP
	Set		TMP.id_presentation = G.id_presentation
	From	#TmpSamResults TMP
			Inner Join dbo.Item G On (G.id = TMP.id_item);

	select 
		res.id ,
		Anio,
		Periodo ,
		id_item ,
		cast(it.masterCode as varchar(50)) as "masterCode",
		cast(it."name" as varchar(1000)) as "name_item",
		res.id_warehouse ,
		cast(wa."name" as varchar(1000)) as "name_warehouse",
		res.id_presentation,
		cast(pr."name" as varchar(1000)) as "name_presentation",
		pr.id_metricUnit as "id_metric_unit",
		UPPER(mu."code") as "code_metric_unit",
		mu."name" as "name_metric_unit",
		SaldoAnterior,
		[Entrada],
		[Salida],
		[SaldoActual],	
		cast(pr."minimum" as decimal(20,6)) as minimum,
		cast(pr."maximum" as decimal(20,6)) as maximum,
		"LB_SaldoAnterior",
		"LB_Entrada" ,
		"LB_Salida",
		"LB_SaldoActual",
		wl.Id id_warehouseLocation,
		wl.Name name_warehouseLocation,
		id_ProductionLot,
		sequencial_productionLot,
		number_productionLot

	from #TmpSamResults res
	inner join Item it with(nolock) on res.id_item = it.id
	inner join Presentation pr with(nolock) on res.id_presentation = pr.id
	inner join MetricUnit mu with(nolock) on pr.id_metricUnit = mu.id
	inner join Warehouse wa with(nolock) on wa.id = res.id_warehouse
	inner join @WarehouseLocation wl on  res.id_warehouseLocation = wl.Id
	order by res.id;
	
END
