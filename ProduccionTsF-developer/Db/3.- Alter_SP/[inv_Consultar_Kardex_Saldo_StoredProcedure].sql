IF OBJECT_ID('inv_Consultar_Kardex_Saldo_StoredProcedure') IS NULL
EXEC('CREATE PROCEDURE inv_Consultar_Kardex_Saldo_StoredProcedure AS')

GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   Procedure [dbo].[inv_Consultar_Kardex_Saldo_StoredProcedure]
(
	@ParametrosBusquedaKardexSaldo		nVarChar(Max),
	@printDebug		Bit = 0
)
As
Begin

	Set NoCount On


	--==================================================================================================
	-- MAPEO DE PAR�METROS
	--==================================================================================================
	Declare	@id_documentType			Int 			= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.id_documentType'),
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
			@codeReport					VarChar(Max)	= JSON_VALUE(@ParametrosBusquedaKardexSaldo, '$.codeReport');

	DECLARE @year INT, @month INT
	DECLARE @idmax INT
	DECLARE @id_warehouse_vitrual INT
	DECLARE @dateBefore DATETIME
	declare @anio_saldo_inicial int;
	declare @mes_saldo_inicial int;


	Declare @ids_items Table ( id_item Int Primary Key );
	Declare @ids_warehouse Table ( id_warehouse Int Primary Key );
	Declare @ids_warehouseLocation Table ( ids_warehouseLocation Int Primary Key );
	
	-- Variables Control Fecha 
	Declare @dateInitConfig Datetime;
	Declare @dateMaxMinBalance Datetime;
	Declare @periodoMaxMinBalance Varchar(6);
	Declare @startEmissionDateInventoryMoveDetail Datetime;
	Declare @dateEnd Datetime;


	-- Optimizacion Tabla Control de Saldo
	
	Declare @setting_periodo_inicial varchar(6) = (select top 1 [value]  from Setting where code = 'PSALINI');	
	SET @anio_saldo_inicial = SUBSTRING(@setting_periodo_inicial,1,4);
	SET @mes_saldo_inicial = SUBSTRING(@setting_periodo_inicial,5,2);
	SET @dateInitConfig = DATEFROMPARTS(@anio_saldo_inicial, @mes_saldo_inicial, 1);
	SET @dateEnd = ISNULL(DateAdd (DAY,1,@endEmissionDate),DateAdd(DAY,1,GETDATE()));

	-- Linea de tiempo mov Consulta
	-- FI(param) | <= PeriodoMaxMin (TablaSaldo) < FechaCorte | > PeriodoMaxMin (InventoryMoveDetail) < FechaCorte 
	
	
	-- Obtener La Fecha Maxima/Minima de Los periodos en el Control de Saldo de Inventario
	-- Con esta fecha tengo la fecha inicial para la busqueda de documentos de Inventario
	Set @dateMaxMinBalance = 
	(Select MIN(fechaControl)  
	from
	(
		select id_warehouse, MAX(fechaControl) fechaControl
		from (
			select  id_warehouse, 
					--DATEADD(month,1,DATEFROMPARTS(Anio, Mes, 1) )  fechaControl
					DATEFROMPARTS(Anio, Mes, 1)  fechaControl
			from MonthlyBalanceControl 
			where 
			--id_company = 2 and 
			IsValid =1
		) a 
		Where a.fechaControl >= @dateInitConfig AND a.fechaControl  < @dateEnd
		group by id_warehouse	
	) b);

	SET @periodoMaxMinBalance = SUBSTRING(trim(CONVERT(varchar,@dateMaxMinBalance,112)),1,6 ) 
	SEt @startEmissionDateInventoryMoveDetail = DATEADD(month,1,@dateMaxMinBalance );

	if(MONTH(@startEmissionDate) = 1)begin
	set @year = YEAR(@startEmissionDate)  - 1
	set @month = 12
	
	end else begin
	set @year = YEAR(@startEmissionDate)  
	set @month = month(@startEmissionDate)  - 1
	end 

	
	If IsNull(@items, 'Todos') != 'Todos'
	Begin
		Insert	Into @ids_items
		Select	Cast(Value As Int)
		From	STRING_SPLIT(@items, ',');
	End
	else 
	begin

		Insert	Into @ids_items
		select id from Item
		where isActive = 1

	end

	Declare	@configuradoWAH	Int;
	Select	@configuradoWAH = Count(1)
	From	dbo.UserEntityDetail A
			Inner Join dbo.UserEntity B On B.id = A.id_userEntity
			Inner Join dbo.Entity C On C.id = B.id_entity
	Where	B.id_user = @id_user
	And		C.code = 'WAH';	--Entidad de Bodega del Sistema


	--==================================================================================================
	-- CREACIoN DE TABLA TEMPORAL: #TmpUserEntityDetail
	--==================================================================================================

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
	

	--========================
	-- Bodega / Ubicacion
	--========================

	If @id_warehouseEntry != 0
	Begin
	   if( @configuradoWAH != 0 AND (select COUNT(*) from #TmpUserEntityDetail p where p.id_entityValue =  Cast(@id_warehouseEntry As Int)) >0)
	   Begin
			Insert	Into @ids_warehouse
			Select	Cast(@id_warehouseEntry As Int)
	   end
		
	End
	else 
	begin
		IF(@configuradoWAH != 0)
		BEGIN
			Insert	Into @ids_warehouse
			select id from Warehouse  w 
			inner join #TmpUserEntityDetail p on p.id_entityValue = w.id
			where	isActive = 1
					and w.code != 'VIREMP'
		END
		ELSE
		BEGIN
			Insert	Into @ids_warehouse
			select id from Warehouse  w 			
			where	isActive = 1
					and w.code != 'VIREMP'
		END
		

	end

	if @id_warehouseLocationEntry != 0 
	begin
		Insert	Into @ids_warehouseLocation
		Select	Cast(@id_warehouseLocationEntry As Int)
	end
	else 
	begin

		Insert	Into @ids_warehouseLocation
		select id from WarehouseLocation
		where isActive = 1
	end
	
	Declare @DocumentType as Table
	(
		id int,
		code nvarchar(5),
		name nvarchar(max)
	)

	Declare @DocumentTypeFiltered as Table
	(
		id int,
		code nvarchar(5),
		name nvarchar(max)
	)

	INSERT INTO @DocumentType
	SELECT  id, code, name 
	FROM DocumentType 
	WHERE code in ('03','04','05','06','23','24','25','26','27','28','32','34','155','137','142','143','156','146','147','148','151','152','57','59','75','150','136','135')

	--#Region OPTMIZACION dOCUMENT TYPE FILTERED
	if( IsNull(@id_documentType, 0) = 0 )
	begin
		INSERT INTO @DocumentTypeFiltered
		SELECT  id, code, name 
		FROM @DocumentType
	end
	else		
	begin
		INSERT INTO @DocumentTypeFiltered
		SELECT  id, code, name 
		FROM DocumentType 
		WHERE id = @id_documentType
	end
	
	--#EndRegion 

	DECLARE @DOCUMENT AS TABLE
	(
		id INT,
		emissionDate datetime,
		id_documentState int,
		id_documentType int,
		number nvarchar(max), 
		reference nvarchar(max),
		sequential int,
		id_emissionPoint int	
	)

	
	SELECT @id_warehouse_vitrual = id FROM Warehouse W WHERE W.code = 'VIREMP'
	
	--#Region OPTMIZACION CONSULTA PRIMARIA DE DOCUMENTOS
	-- En Primera instancia no usar los filtros number y reference, dejarlos para filtrarlos desde la memoria
	-- condicionar si existe los filtros number y reference dejarlos para filtrarlos desde la memoria

	INSERT INTO @DOCUMENT
	SELECT C.id, C.emissionDate, C.id_documentState, C.id_documentType,C.number, C.reference, C.sequential,C.id_emissionPoint
	FROM Document C 
	inner join	@DocumentTypeFiltered dt on dt.id = C.id_documentType
	WHERE		
	-- OPTIMIZACION: ESTOS CAMPOS FILTRARLOS DESDE LA MEMORIA
	    (
        (DAY(@startEmissionDate) <> 1 AND C.emissionDate < @dateEnd) 
        OR 
        (DAY(@startEmissionDate) = 1 AND C.emissionDate >= @startEmissionDate AND C.emissionDate < @dateEnd)
    )
    AND 
    C.id_documentState in (3, 21);
	--#EndRegion
	
	if(@number != 'Todos' )
	Begin
		Delete @DOCUMENT where Cast(number As VarChar(20)) NOT LIKE '%' + @number + '%'
	End

	if(@reference  != 'Todas' )
	Begin
		Delete @DOCUMENT where Cast(reference As VarChar(20)) NOT LIKE '%' + @reference + '%'
	End

	--==================================================================================================
	-- CREACION DE TABLA TEMPORAL: #TmpIdentificadoresInventoryMoveDetail
	--==================================================================================================
	create table #TmpIdentificadoresInventoryMoveDetailPre
	(
		id					Int Primary Key,
		id_document			Int,
		emissionDate		DateTime,
		sequential			Int,
		id_item				Int,
		id_metricUnit		Int,
		id_warehouse		Int,
		id_warehouseLocation Int,
		id_lot				Int,
		priceCost			Decimal(14,6),
		entryAmount			Decimal(14,6),
		entryAmountCost		Decimal(14,6),
		exitAmount			Decimal(14,6),
		exitAmountCost		Decimal(14,6),
		id_inventoryMove	Int,
		previousBalance		Decimal(14,6),
		previousBalanceCost	Decimal(14,6),
		balance				Decimal(14,6),
		balanceCost			Decimal(14,6),
		balanceCutting		Decimal(14,6),
		balanceCuttingCost	Decimal(14,6),
		numberRemissionGuide VarChar(100),
		codeDocumentState	VarChar(20),
		dateCreate			DateTime,
		lotMarked			VarChar(20),
		idNatureMove		int,
		id_inventoryReason	int
	);

	create table #TmpIdentificadoresInventoryMoveDetail
	(
		id					Int Primary Key,
		id_document			Int,
		emissionDate		DateTime,
		sequential			Int,
		id_item				Int,
		id_metricUnit		Int,
		id_warehouse		Int,
		id_warehouseLocation Int,
		id_lot				Int,
		priceCost			Decimal(14,6),
		entryAmount			Decimal(14,6),
		entryAmountCost		Decimal(14,6),
		exitAmount			Decimal(14,6),
		exitAmountCost		Decimal(14,6),
		id_inventoryMove	Int,
		previousBalance		Decimal(14,6),
		previousBalanceCost	Decimal(14,6),
		balance				Decimal(14,6),
		balanceCost			Decimal(14,6),
		balanceCutting		Decimal(14,6),
		balanceCuttingCost	Decimal(14,6),
		numberRemissionGuide VarChar(100),
		codeDocumentState	VarChar(20),
		dateCreate			DateTime,
		lotMarked			VarChar(20)
	);

	--#REGION OPTIMIZACION Correcion Rango Inventory Move Detail 
	Insert	Into #TmpIdentificadoresInventoryMoveDetailPre
	(
		id,
		id_document,
		emissionDate,
		sequential,
		id_item,
		id_metricUnit,
		id_warehouse,
		id_warehouseLocation,
		id_lot,
		priceCost,
		entryAmount,
		entryAmountCost,
		exitAmount,
		exitAmountCost,
		id_inventoryMove,
		previousBalance,
		previousBalanceCost,
		balance,
		balanceCost,
		balanceCutting,
		balanceCuttingCost,
		numberRemissionGuide,
		codeDocumentState,
		dateCreate,
		lotMarked,
		idNatureMove,
		id_inventoryReason
	)
	Select	A.id,
			A.id_inventoryMove As id_document,
			C.emissionDate,
			B.sequential,
			A.id_item,
			A.id_metricUnit,
			A.id_warehouse,
			A.id_warehouseLocation,
			A.id_lot,
			A.unitPrice,
			A.entryAmount,
			A.entryAmountCost,
			A.exitAmount,
			A.exitAmountCost,
			A.id_inventoryMove,
			0 As previousBalance,
			0 As previousBalanceCost,
			0 As balance,
			0 As balanceCost,
			0 As balanceCutting,
			0 As balanceCuttingCost,
			'' As numberRemissionGuide,
			D.code As codeDocumentState,
			A.dateCreate,
			Isnull(A.lotMarked, ''),
			B.idNatureMove,
			B.id_inventoryReason
	From	dbo.InventoryMoveDetail A
			Inner Join dbo.InventoryMove B On B.id = A.id_inventoryMove
			Inner Join @DOCUMENT C On C.id = B.id
			Inner Join dbo.DocumentState D On D.id = C.id_documentState
			Inner join @ids_warehouse W1 On W1.id_warehouse = A.id_warehouse 
			Inner Join dbo.Warehouse W On W.id = W1.id_warehouse
			Inner Join @ids_warehouseLocation Wl1 On Wl1.ids_warehouseLocation = A.id_warehouseLocation
			Inner Join @ids_items It1 On It1.id_item =A.id_item
			Where (IsNull(@id_warehouseExit, 0) = 0 Or A.id_warehouse = @id_warehouseExit)
			And		(IsNull(@id_warehouseLocationExit, 0) = 0 Or A.id_warehouseLocation = @id_warehouseLocationExit)
   -- Optimizacion Se crearan indices especificos para cada cfdiltro 

   if(IsNull(@idNatureMove, 0) != 0)
   Begin
		delete from #TmpIdentificadoresInventoryMoveDetailPre where idNatureMove != @idNatureMove;
   End

   if(IsNull(@id_inventoryReason, 0) != 0)
   Begin
		delete from #TmpIdentificadoresInventoryMoveDetailPre where id_inventoryReason != @id_inventoryReason;
   End


	--// UPDATE MEMORY DETAIL 
	Insert Into #TmpIdentificadoresInventoryMoveDetail
	(
		id,
		id_document,
		emissionDate,
		sequential,
		id_item,
		id_metricUnit,
		id_warehouse,
		id_warehouseLocation,
		id_lot,
		priceCost,
		entryAmount,
		entryAmountCost,
		exitAmount,
		exitAmountCost,
		id_inventoryMove,
		previousBalance,
		previousBalanceCost,
		balance,
		balanceCost,
		balanceCutting,
		balanceCuttingCost,
		numberRemissionGuide,
		codeDocumentState,
		dateCreate,
		lotMarked
	)
	Select	Z.id,
			Z.id_inventoryMove As id_document,
			Z.emissionDate,
			Z.sequential,
			Z.id_item,
			Z.id_metricUnit,
			Z.id_warehouse,
			Z.id_warehouseLocation,
			Z.id_lot,
			Z.priceCost,
			Z.entryAmount,
			Z.entryAmountCost,
			Z.exitAmount,
			Z.exitAmountCost,
			Z.id_inventoryMove,
			0 As previousBalance,
			0 As previousBalanceCost,
			0 As balance,
			0 As balanceCost,
			0 As balanceCutting,
			0 As balanceCuttingCost,
			'' As numberRemissionGuide,
			Z.codeDocumentState,
			Z.dateCreate,
			Isnull(Z.lotMarked, '')
	From	#TmpIdentificadoresInventoryMoveDetailPre Z
			Left Join 
			(	Select		id,id_dispatcher
				From		dbo.InventoryExitMove 
				Where		(IsNull(@id_dispatcher, 0) = 0 Or id_dispatcher = @id_dispatcher)	
			 ) E On E.id = Z.id_inventoryMove			 
			Left Join 
			(
				Select  id,id_receiver
				from	dbo.InventoryEntryMove 
			)F On F.id = Z.id_inventoryMove
			Left Join  dbo.Lot G On G.id = Z.id_lot				
			--(	
			--	Select	id,number,internalNumber
			--	from	dbo.Lot 
			--	-- where	(@numberLot = 'Todos' Or Cast(number As VarChar(20)) Like '%' + @numberLot + '%')
			--	--		AND ( @internalNumberLot = 'Todos' Or  Cast(internalNumber As VarChar(20)) Like '%' + @internalNumberLot + '%')
			--) 
			Left Join dbo.ProductionLot  H On H.id = G.id
			--(
			--	Select id,internalNumber
			--	from dbo.ProductionLot 
			--	--Where @internalNumberLot = 'Todos' Or Cast(internalNumber As VarChar(20)) Like '%' + @internalNumberLot + '%' 
			--) H On H.id = G.id
			--  AND @internalNumberLot = 'Todos' Or Cast(H.internalNumber As VarChar(20)) Like '%' + @internalNumberLot + '%' 
	Where	(IsNull(@id_dispatcher, 0) = 0 Or E.id_dispatcher = @id_dispatcher)	
	And		(F.id_receiver = @id_receiver OR IsNull(@id_receiver, 0) = 0 )
	And		(@numberLot = 'Todos' Or Cast(G.number As VarChar(20)) Like '%' + @numberLot + '%')
	And     (@internalNumberLot = 'Todos' Or Cast(H.internalNumber As VarChar(20)) Like '%' + @internalNumberLot + '%' Or Cast(G.internalNumber As VarChar(20)) Like '%' + @internalNumberLot + '%')
	--And		(@configuradoWAH = 0 Or (A.id_warehouse In ( Select id_entityValue From #TmpUserEntityDetail))) --Entidad de Bodega del Sistema
	--And		W.code not in ('VIREMP')
	--#ENDREGION
	--==================================================================================================
	-- CREACI�N DE TABLA TEMPORAL: #TmpIdentificadoresInventoryMoveDetailReduced
	--==================================================================================================
	
	create table #TmpIdentificadoresInventoryMoveDetailReduced 
	(
		id					Int Primary Key,
		id_document			Int,
		emissionDate		DateTime,
		sequential			Int,
		id_item				Int,
		id_metricUnit		Int,
		id_warehouse		Int,
		id_warehouseLocation Int,
		id_lot				Int,
		priceCost			Decimal(14,6),
		entryAmount			Decimal(14,6),
		entryAmountCost		Decimal(14,6),
		exitAmount			Decimal(14,6),
		exitAmountCost		Decimal(14,6),
		id_inventoryMove	Int,
		previousBalance		Decimal(14,6),
		previousBalanceCost	Decimal(14,6),
		balance				Decimal(14,6),
		balanceCost			Decimal(14,6),
		balanceCutting		Decimal(14,6),
		balanceCuttingCost	Decimal(14,6),
		numberRemissionGuide VarChar(100),
		codeDocumentState	VarChar(20),
		dateCreate			DateTime,
		lotMarked			VarChar(20)
	);
	
	Create Index IDX_TmpIdentificadoresInventoryMoveDetailReduced_items On #TmpIdentificadoresInventoryMoveDetailReduced (id_item, id_warehouse, id_warehouseLocation);
	Create Index IDX_TmpIdentificadoresInventoryMoveDetailReduced_inventoryMove On #TmpIdentificadoresInventoryMoveDetailReduced (id_inventoryMove);

	Insert	Into #TmpIdentificadoresInventoryMoveDetailReduced
	(
		id,
		id_document,
		emissionDate,
		sequential,
		id_item,
		id_metricUnit,
		id_warehouse,
		id_warehouseLocation,
		id_lot,
		priceCost,
		entryAmount,
		entryAmountCost,
		exitAmount,
		exitAmountCost,
		id_inventoryMove,
		previousBalance,
		previousBalanceCost,
		balance,
		balanceCost,
		balanceCutting,
		balanceCuttingCost,
		numberRemissionGuide,
		codeDocumentState,
		dateCreate,
		lotMarked
	)
	Select	id,
			id_document,
			emissionDate,
			sequential,
			id_item,
			id_metricUnit,
			id_warehouse,
			id_warehouseLocation,
			id_lot,
			priceCost,
			entryAmount,
			entryAmountCost,
			exitAmount,
			exitAmountCost,
			id_inventoryMove,
			previousBalance,
			previousBalanceCost,
			balance,
			balanceCost,
			balanceCutting,
			balanceCuttingCost,
			numberRemissionGuide,
			codeDocumentState,
			dateCreate,
			lotMarked
	From	#TmpIdentificadoresInventoryMoveDetail
	Where	emissionDate >= @startEmissionDate AND emissionDate < @dateEnd;
	SET @dateBefore = DATEFROMPARTS(YEAR(@startEmissionDate), MONTH(@startEmissionDate), 1)

--	==================================================================================================
--	 CREACI�N DE TABLA TEMPORAL: #TmpInventoryMoveDetailWithPreviousBalance
--	==================================================================================================


	create table #TmpIdentificadoresInventoryMoveDetailReducedFilter 
	(
	id int,
	id_item int,
	id_warehouse int, 
	id_warehouseLocation int, 
	id_lot int, 
	emissionDate DateTime,
	dateCreate DateTime
	)

	create table #TmpIdentificadoresInventoryMoveDetailFilter 
	(
	id int,
	id_item int,
	id_warehouse int, 
	id_warehouseLocation int, 
	id_lot int, 
	emissionDate DateTime,
	dateCreate DateTime,
	entryAmount DECIMAL(14,6),
	exitAmount DECIMAL(14,6),
	entryAmountCost DECIMAL(14,6),
	exitAmountCost DECIMAL(14,6),
	id_metricUnit int,
	priceCost DECIMAL(14,6),
	)

	create table #TmpInventoryMoveDetailWithPreviousBalance 
	(
		id					Int Primary Key,
		previousBalance		Decimal(14,6),
		previousBalanceCost		Decimal(14,6),
		previousBalanceCutting Decimal(14,6),
		previousBalanceCuttingCost Decimal(14,6),
		id_item				Int,
		id_warehouse		Int,
		id_warehouseLocation Int,
		id_lot				Int,
		emissionDate		DateTime,
		dateCreate			DateTime
	);

	Create Index IDX_TmpInventoryMoveDetailWithPreviousBalance_items On #TmpInventoryMoveDetailWithPreviousBalance (id_item,id_lot, id_warehouse, id_warehouseLocation);

	INSERT INTO #TmpIdentificadoresInventoryMoveDetailReducedFilter
	SELECT id ,id_item , id_warehouse , id_warehouseLocation , id_lot , emissionDate, dateCreate  
	FROM #TmpIdentificadoresInventoryMoveDetailReduced
	WHERE codeDocumentState IN ('03', '16')


	INSERT INTO #TmpIdentificadoresInventoryMoveDetailFilter
	SELECT id, id_item , id_warehouse , id_warehouseLocation , id_lot , emissionDate, dateCreate,entryAmount, exitAmount,
	entryAmountCost,exitAmountCost,id_metricUnit,priceCost
	FROM #TmpIdentificadoresInventoryMoveDetail 
	WHERE codeDocumentState IN ('03', '16')


	Create NONCLUSTERED Index IDX_#TmpIdentificadoresInventoryMoveDetailReducedFilter_PK On #TmpIdentificadoresInventoryMoveDetailReducedFilter (id);
	Create NONCLUSTERED Index IDX_#TmpIdentificadoresInventoryMoveDetailReducedFilter_item On #TmpIdentificadoresInventoryMoveDetailReducedFilter (id_item);
	Create NONCLUSTERED Index IDX_#TmpIdentificadoresInventoryMoveDetailReducedFilter_lot On #TmpIdentificadoresInventoryMoveDetailReducedFilter (id_lot);
	Create NONCLUSTERED Index IDX_#TmpIdentificadoresInventoryMoveDetailReducedFilter_warehouse On #TmpIdentificadoresInventoryMoveDetailReducedFilter (id_warehouse);
	Create NONCLUSTERED Index IDX_#TmpIdentificadoresInventoryMoveDetailReducedFilter_warehouseLocation On #TmpIdentificadoresInventoryMoveDetailReducedFilter (id_warehouseLocation);


	Create NONCLUSTERED INDEX IDX_#TmpIdentificadoresInventoryMoveDetailFilter_item On #TmpIdentificadoresInventoryMoveDetailFilter (id_item);
	Create NONCLUSTERED INDEX IDX_#TmpIdentificadoresInventoryMoveDetailFilter_lot On #TmpIdentificadoresInventoryMoveDetailFilter (id_lot);
	Create NONCLUSTERED INDEX IDX_#TmpIdentificadoresInventoryMoveDetailFilter_warehouse On #TmpIdentificadoresInventoryMoveDetailFilter (id_warehouse);
	Create NONCLUSTERED INDEX IDX_#TmpIdentificadoresInventoryMoveDetailFilter_warehouseLocation On #TmpIdentificadoresInventoryMoveDetailFilter (id_warehouseLocation);


	CREATE NONCLUSTERED INDEX TmpIdentificadoresInventoryMoveDetailFilter_item ON #TmpInventoryMoveDetailWithPreviousBalance(id_item);
	Create NONCLUSTERED INDEX TmpIdentificadoresInventoryMoveDetailFilter_item_lot On #TmpInventoryMoveDetailWithPreviousBalance (id_lot);
	Create NONCLUSTERED INDEX TmpIdentificadoresInventoryMoveDetailFilter_item_warehouse On #TmpInventoryMoveDetailWithPreviousBalance (id_warehouse);
	Create NONCLUSTERED INDEX TmpIdentificadoresInventoryMoveDetailFilter_item_warehouseLocation On #TmpInventoryMoveDetailWithPreviousBalance (id_warehouseLocation);
	Create NONCLUSTERED INDEX TmpIdentificadoresInventoryMoveDetailFilter_item_previousBalance On #TmpInventoryMoveDetailWithPreviousBalance (previousBalance);
	Create NONCLUSTERED INDEX TmpIdentificadoresInventoryMoveDetailFilter_item_previousBalanceCost On #TmpInventoryMoveDetailWithPreviousBalance (previousBalanceCost);
	Create NONCLUSTERED INDEX TmpIdentificadoresInventoryMoveDetailFilter_item_previousBalanceCutting On #TmpInventoryMoveDetailWithPreviousBalance (previousBalanceCutting);
	Create NONCLUSTERED INDEX TmpIdentificadoresInventoryMoveDetailFilter_item_previousBalanceCuttingCost On #TmpInventoryMoveDetailWithPreviousBalance (previousBalanceCuttingCost);


	Insert	Into #TmpInventoryMoveDetailWithPreviousBalance
	(
		id
		
	)
	Select	A.id
	FROM
    #TmpIdentificadoresInventoryMoveDetailReducedFilter A
	INNER JOIN
    #TmpIdentificadoresInventoryMoveDetailFilter AA ON
        AA.id_item = A.id_item
        AND AA.id_warehouse = A.id_warehouse
        AND AA.id_warehouseLocation = A.id_warehouseLocation
        AND COALESCE(AA.id_lot, 0) = COALESCE(A.id_lot, 0)
        AND (AA.emissionDate < A.emissionDate OR (AA.emissionDate = A.emissionDate AND AA.dateCreate < A.dateCreate))
GROUP BY
    A.id;



	update T set T.previousBalance = (T.previousBalance - C.previousBalance), t.previousBalanceCost = c.previousBalanceCost
	from #TmpInventoryMoveDetailWithPreviousBalance T
	inner join 
	(Select T.id, 
	Sum(IsNull(X.entryAmount, 0) - IsNull(X.exitAmount, 0)) previousBalance,
	Sum(IsNull(X.entryAmountCost, 0) - IsNull(X.exitAmountCost, 0))previousBalanceCost
	from #TmpIdentificadoresInventoryMoveDetailReducedFilter T
	inner join #TmpIdentificadoresInventoryMoveDetailFilter X 
	on  X.id_item = T.id_item
	AND X.id_warehouse = T.id_warehouse
	AND X.id_warehouseLocation = T.id_warehouseLocation
	AND COALESCE(X.id_lot, 0) = COALESCE(T.id_lot, 0)
	AND (X.emissionDate < T.emissionDate OR (X.emissionDate = T.emissionDate AND X.dateCreate < T.dateCreate))
	group by T.id ) C on T.id = c.Id 



	If	@startEmissionDate Is Not Null
	Begin


		Update	A
		Set		A.id_item = AA.id_item,
				A.id_warehouse = AA.id_warehouse,
				A.id_warehouseLocation = AA.id_warehouseLocation,
				A.id_lot = AA.id_lot,
				A.emissionDate = AA.emissionDate,
				A.dateCreate = AA.dateCreate,
				A.previousBalanceCutting = A.previousBalance,
				A.previousBalanceCuttingCost = A.previousBalanceCost
		From	#TmpInventoryMoveDetailWithPreviousBalance As A
				Inner Join #TmpIdentificadoresInventoryMoveDetailReduced AA On (AA.id = A.id);

		Update	A
		Set		A.previousBalanceCutting = 0, A.previousBalanceCuttingCost = 0
		From	#TmpInventoryMoveDetailWithPreviousBalance As A, #TmpIdentificadoresInventoryMoveDetailReducedFilter AA
		Where	AA.id_item = A.id_item
		And		AA.id_warehouse = A.id_warehouse
		And		AA.id_warehouseLocation = A.id_warehouseLocation
		And		IsNull(AA.id_lot, 0) = IsNull(A.id_lot, 0)
		And		(AA.emissionDate < A.emissionDate Or (AA.emissionDate = A.emissionDate And (AA.dateCreate < A.dateCreate)))


		--==================================================================================================
		-- CREACI�N DE TABLA TEMPORAL: @TmpInventoryMoveDetailToNoShow
		--==================================================================================================

		Declare @TmpInventoryMoveDetailToNoShow as table
		(
			id					Int Primary Key,
			id_item				Int,
			id_warehouse		Int,
			id_warehouseLocation Int,
			id_lot				Int,
			id_metricUnit		Int,
			priceCost			Decimal(14,6),
			entryAmount			Decimal(14,6),
			entryAmountCost		Decimal(14,6),
			exitAmount			Decimal(14,6),
			exitAmountCost		Decimal(14,6)
		);

		Insert	Into @TmpInventoryMoveDetailToNoShow
		(
			id,
			id_item,
			id_warehouse,
			id_warehouseLocation,
			id_lot,
			id_metricUnit,
			priceCost,
			entryAmount,
			entryAmountCost,
			exitAmount,
			exitAmountCost
		)
		Select	A.id,
				A.id_item,
				A.id_warehouse,
				A.id_warehouseLocation,
				A.id_lot,
				A.id_metricUnit,
				A.priceCost,
				A.entryAmount,
				A.entryAmountCost,
				A.exitAmount,
				A.exitAmountCost
		From	#TmpIdentificadoresInventoryMoveDetailFilter A
		where		(@dateEnd Is Null Or A.emissionDate < DateAdd(day, 1, @dateEnd));


		Delete	A
		From	@TmpInventoryMoveDetailToNoShow A, #TmpIdentificadoresInventoryMoveDetailReducedFilter AA
		Where	AA.id_item = A.id_item
		And		AA.id_warehouse = A.id_warehouse
		And		AA.id_warehouseLocation = A.id_warehouseLocation
		And		IsNull(AA.id_lot, 0) = IsNull(A.id_lot, 0)


		--==================================================================================================
		-- CREACI�N DE TABLA TEMPORAL: @TmpInventoryMoveDetailNoShow
		--==================================================================================================


		declare @TmpInventoryMoveDetailNoShow as table
		(
			id					Int Primary Key,
			previousBalance		Decimal(14,6),
			previousBalanceCost	Decimal(14,6),
			id_item				Int,
			id_warehouse		Int,
			id_warehouseLocation Int,
			id_lot				Int,
			id_metricUnit		Int
		);

		Insert	Into @TmpInventoryMoveDetailNoShow
		(
			id,
			previousBalance,
			previousBalanceCost,
			id_item,
			id_warehouse,
			id_warehouseLocation,
			id_lot,
			id_metricUnit
		)
		Select	Max(A.id) As id,
				Sum(IsNull(A.entryAmount, 0) - IsNull(A.exitAmount, 0)) As previoBalance,
				Sum(IsNull(A.entryAmountCost, 0) - IsNull(A.exitAmountCost, 0)) As previoBalanceCost,
				A.id_item,
				A.id_warehouse,
				A.id_warehouseLocation,
				A.id_lot,
				A.id_metricUnit
		From	@TmpInventoryMoveDetailToNoShow A
		Group	By A.id_item, A.id_warehouse, A.id_warehouseLocation, A.id_lot, A.id_metricUnit;
	End


	UPDATE te SET te.previousBalance =  mb.SaldoActual +  te.previousBalance
	from #TmpIdentificadoresInventoryMoveDetailReduced  te
	inner join MonthlyBalance mb on te.id_item = mb.id_item
	and te.id_warehouse = mb.id_warehouse and te.id_warehouseLocation = mb.id_warehouseLocation
	and te.id_lot = mb.id_productionLot
	where mb.Anio = @year
	AND mb.Periodo = @month

	select @idmax = isnull(max(id),0) + 1 from #TmpIdentificadoresInventoryMoveDetailReduced
	

	
	insert into #TmpIdentificadoresInventoryMoveDetailReduced

	select 
	@idmax + ROW_NUMBER() OVER (ORDER BY mb.id_productionLot) AS NewID,
	@idmax + ROW_NUMBER() OVER (ORDER BY mb.id_productionLot) AS NewID,
	CAST(cast(@year as varchar(4)) + '-' + cast(@month as varchar(2)) + '-01' as datetime) ,0, mb.id_item,
	id_metric_unit,id_warehouse, id_warehouseLocation, id_productionLot,0,
	mb.SaldoActual, 0, 0,0,0, 0, 0,0,0,0,0,'',16,GETDATE(),''
	from MonthlyBalance mb	
	where mb.Anio = @year
	AND mb.Periodo = @month
	And		mb.id_warehouse IN (select id_warehouse from @ids_warehouse)  
	And		mb.id_warehouseLocation IN (select ids_warehouseLocation from @ids_warehouseLocation )
	And		mb.id_item IN (Select id_item From @ids_items)
	And     (@internalNumberLot = 'Todos' Or Cast(mb.number_productionLot As VarChar(20)) Like '%' + @internalNumberLot + '%' Or Cast(mb.number_productionLot As VarChar(20)) Like '%' + @internalNumberLot + '%')
	And		(@numberLot = 'Todos' Or Cast(mb.sequencial_productionLot As VarChar(20)) Like '%' + @numberLot + '%')
	And		(IsNull(@id_warehouseExit, 0) = 0 Or mb.id_warehouse = @id_warehouseExit)
	And		(IsNull(@id_warehouseLocationExit, 0) = 0 Or mb.id_warehouseLocation = @id_warehouseLocationExit)
	and mb.id not in (
	select mb.id
	from #TmpIdentificadoresInventoryMoveDetailReduced  te
	inner join MonthlyBalance mb on te.id_item = mb.id_item
	and te.id_warehouse = mb.id_warehouse 
	and te.id_warehouseLocation = mb.id_warehouseLocation
	and te.id_lot =  mb.id_productionLot
	where mb.Anio = @year
	AND mb.Periodo = @month)
	AND mb.id_warehouse <> @id_warehouse_vitrual;


	IF(DAY(@startEmissionDate) <> 1 )BEGIN

	create table #TmpIdentificadoresInventoryMoveDetailReducedDocumentPrevius 
	(
		id					Int Primary Key,
		id_document			Int,
		emissionDate		DateTime,
		sequential			Int,
		id_item				Int,
		id_metricUnit		Int,
		id_warehouse		Int,
		id_warehouseLocation Int,
		id_lot				Int,
		priceCost			Decimal(14,6),
		entryAmount			Decimal(14,6),
		entryAmountCost		Decimal(14,6),
		exitAmount			Decimal(14,6),
		exitAmountCost		Decimal(14,6),
		id_inventoryMove	Int,
		previousBalance		Decimal(14,6),
		previousBalanceCost	Decimal(14,6),
		balance				Decimal(14,6),
		balanceCost			Decimal(14,6),
		balanceCutting		Decimal(14,6),
		balanceCuttingCost	Decimal(14,6),
		numberRemissionGuide VarChar(100),
		codeDocumentState	VarChar(20),
		dateCreate			DateTime,
		lotMarked			VarChar(20)
	);
	
	Create Index IDX_#TmpIdentificadoresInventoryMoveDetailReducedDocumentPrevius_items On #TmpIdentificadoresInventoryMoveDetailReducedDocumentPrevius (id_item, id_warehouse, id_warehouseLocation);
	Create Index IDX_#TmpIdentificadoresInventoryMoveDetailReducedDocumentPrevius_inventoryMove On #TmpIdentificadoresInventoryMoveDetailReducedDocumentPrevius (id_inventoryMove);

	Insert	Into #TmpIdentificadoresInventoryMoveDetailReducedDocumentPrevius
	(
		id,
		id_document,
		emissionDate,
		sequential,
		id_item,
		id_metricUnit,
		id_warehouse,
		id_warehouseLocation,
		id_lot,
		priceCost,
		entryAmount,
		entryAmountCost,
		exitAmount,
		exitAmountCost,
		id_inventoryMove,
		previousBalance,
		previousBalanceCost,
		balance,
		balanceCost,
		balanceCutting,
		balanceCuttingCost,
		numberRemissionGuide,
		codeDocumentState,
		dateCreate,
		lotMarked
	)
	Select	id,
			id,
			@dateBefore,
			0,
			id_item,
			id_metricUnit,
			id_warehouse,
			id_warehouseLocation,
			id_lot,
			sum(priceCost),
			sum(entryAmount),
			sum(entryAmountCost),
			sum(exitAmount),
			sum(exitAmountCost),
			0,
			sum(previousBalance),
			sum(previousBalanceCost),
			sum(balance),
			sum(balanceCost),
			sum(balanceCutting),
			sum(balanceCuttingCost),
			'',
			16,
			GETDATE(),
			''
	From	#TmpIdentificadoresInventoryMoveDetail
	Where ((emissionDate >= @dateBefore ) 
				And (emissionDate < DateAdd(day, 1, @startEmissionDate)))
	AND id_document not in (select id_document from  #TmpIdentificadoresInventoryMoveDetailReduced)
	group by id,id_item,id_metricUnit, id_lot,id_warehouse, id_warehouseLocation;


		if((select COUNT(*) from #TmpIdentificadoresInventoryMoveDetailReducedDocumentPrevius) =  0) begin
		update d set 
		d.exitAmount = dt.exitAmount,
		d.entryAmount = dt.entryAmount,
		d.balance = d.balance  + dt.saldo,
		d.balanceCutting = d.balanceCutting + dt.saldo
		from #TmpIdentificadoresInventoryMoveDetailReduced d
		inner join (
		select id_item,id_lot,id_warehouse, id_warehouseLocation ,(sum(entryAmount) - SUM(exitAmount)) saldo,
		SUM(entryAmount) entryAmount, SUM(exitAmount) exitAmount 
		from #TmpIdentificadoresInventoryMoveDetailReducedDocumentPrevius
		group by id_item, id_lot,id_warehouse, id_warehouseLocation) dt
		on d.id_item = dt.id_item
		and d.id_warehouse = dt.id_warehouse
		and d.id_lot = dt.id_lot
		and d.id_warehouseLocation = dt.id_warehouseLocation;


		end else begin

			

		insert into #TmpIdentificadoresInventoryMoveDetailReduced
			select * from #TmpIdentificadoresInventoryMoveDetailReducedDocumentPrevius

		

		update t set t.previousBalance  = isnull(t2.previousBalance,0)
		from #TmpIdentificadoresInventoryMoveDetailReduced t
		left join (select id_item, id_warehouse, id_warehouseLocation, id_lot,
		isnull(previousBalance,0)previousBalance,
		ROW_NUMBER() OVER (PARTITION BY id_item, id_warehouse, id_warehouseLocation, id_lot ORDER BY id_item, id_warehouse, id_warehouseLocation, id_lot) AS NewID
		from #TmpIdentificadoresInventoryMoveDetailReduced where sequential <> 0 )t2
		on t.id_item = t2.id_item
		and t.id_warehouse = t2.id_warehouse
		and t.id_lot = t2.id_lot
		and t.id_warehouseLocation = t2.id_warehouseLocation
		and t2.NewID = 1
		where t.sequential = 0;
		end;



	END;
	

		WITH KardexCalculado AS (
    SELECT
        id,
        id_item,
		id_warehouse,
		id_warehouseLocation,
		id_lot,
        emissionDate,
        sequential,
        entryAmount,
        exitAmount,
		previousBalance,
        previousBalance + SUM(entryAmount - exitAmount) OVER (PARTITION BY id_item, id_warehouse, id_warehouseLocation, id_lot ORDER BY emissionDate,id_item, id_warehouse, id_warehouseLocation, id_lot) AS SaldoAcumulado
    FROM
        #TmpIdentificadoresInventoryMoveDetailReduced
),
KardexCalculadoConPrevio AS (
    SELECT
        id,
        id_item,
		id_warehouse,
		id_warehouseLocation,
		id_lot,
        emissionDate,
        sequential,
        entryAmount,
        exitAmount,
        SaldoAcumulado,
		previousBalance,
        LAG(SaldoAcumulado) OVER (PARTITION BY id_item, id_warehouse, id_warehouseLocation, id_lot ORDER BY emissionDate,id_item, id_warehouse, id_warehouseLocation, id_lot) AS previousBalanceCalculated
    FROM
        KardexCalculado
)

UPDATE A SET
	A.entryAmount =  k.entryAmount , 
	A.exitAmount =  k.exitAmount,
    A.balance = k.SaldoAcumulado,
    A.previousBalance = ISNULL(k.previousBalanceCalculated, k.previousBalance),
    A.balanceCutting = k.SaldoAcumulado - ISNULL(k.previousBalanceCalculated, 0) 
FROM #TmpIdentificadoresInventoryMoveDetailReduced A
JOIN KardexCalculadoConPrevio k ON A.id = k.id and A.id_warehouse = k.id_warehouse 
and A.id_warehouseLocation = k.id_warehouseLocation and A.id_lot = k.id_lot;



	Update	A
	Set		A.numberRemissionGuide = Cast(AA.sequential As VarChar(100)) + ' - '  + Convert(VarChar(10), AA.emissionDate, 103)
	From	#TmpIdentificadoresInventoryMoveDetailReduced As A
			Inner Join dbo.DocumentSource AB On AB.id_document = A.id_inventoryMove
			Inner Join @DOCUMENT AA On AB.id_documentOrigin = AA.id
			Inner Join @DocumentType AC On AC.id = AA.id_documentType
	Where	AC.code = '08';	--C�digo de tipo de documento de Gu�a de Remisi�n


--	==================================================================================================
--	 CREACI�N DE TABLA TEMPORAL: #TmpResults
--	==================================================================================================


	Create Table #TmpResults
	(
		id						Int,
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
		previousBalanceCost		Decimal(14,6),
		[priceCost]				Decimal(14,6),
		[entry]					Decimal(14,6),
		[entryCost]				Decimal(14,6),
		[exit]					Decimal(14,6),
		[exitCost]				Decimal(14,6),
		balance					Decimal(14,6),
		balanceCost				Decimal(14,6),
		balanceCutting			Decimal(14,6),
		balanceCuttingCost		Decimal(14,6),
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
		lotMarked				VarChar(20) Collate DATABASE_DEFAULT
	);



		Insert	Into #TmpResults
		(
			id,
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
			previousBalanceCost,
			[priceCost],
			[entry],
			[entryCost],
			[exit],
			[exitCost],
			balance,
			balanceCost,
			balanceCutting,
			balanceCuttingCost,
			numberRemissionGuide,
			dateCreate,
			lotMarked
		)
		Select	id,
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
				previousBalanceCost,
				priceCost,
				entryAmount,
				entryAmountCost,
				exitAmount,
				exitAmountCost,
				balance,
				balanceCost,
				balanceCutting,
				balanceCuttingCost,
				numberRemissionGuide,
				dateCreate,
				lotMarked
		From	#TmpIdentificadoresInventoryMoveDetailReduced
		Where	(@codeReport = 'IMIPV1' Or codeDocumentState in ('03','16'));	--'03' APROBADA


	

	Create Index IDX_TmpResults_PK On #TmpResults (id);
	Create Index IDX_TmpResults_document On #TmpResults (id_document);
	Create Index IDX_TmpResults_item On #TmpResults (id_item);
	Create Index IDX_TmpResults_metricUnit On #TmpResults (id_metricUnit);
	Create Index IDX_TmpResults_lot On #TmpResults (id_lot);
	Create Index IDX_TmpResults_warehouse On #TmpResults (id_warehouse);
	Create Index IDX_TmpResults_warehouseLocation On #TmpResults (id_warehouseLocation);
	Create Index IDX_TmpResults_warehouseLocationEntry On #TmpResults (id_warehouseLocationEntry);


	Update	TMP
	Set		TMP.document = IsNull(C.sequential, 0),
			TMP.id_inventoryReason = C.id_inventoryReason
	From	#TmpResults TMP
			Inner Join dbo.InventoryMove C On (C.id = TMP.id_document);



	Update	TMP
	Set		TMP.id_documentType = D.id_documentType,
			TMP.id_documentState = D.id_documentState,
			TMP.id_emissionPoint = D.id_emissionPoint,
			TMP.emissionDate = D.emissionDate
	From	#TmpResults TMP
			Inner Join @DOCUMENT D On (D.id = TMP.id_document);

			

	UPDATE TMP SET TMP.emissionDate = CAST(cast(@year as varchar(4)) + '-' + cast(@month as varchar(2)) + '-01' as datetime)
	FROM #TmpResults TMP WHERE TMP.emissionDate IS NULL 

	
	Update	TMP
	Set		TMP.nameDocumentState = P.[name],
			TMP.documentType =  E.[name]
	From	#TmpResults TMP
			Inner Join dbo.DocumentState P On (P.id = TMP.id_documentState)
			Inner Join @DocumentType E On (E.id = TMP.id_documentType);
	
	Update	TMP
	Set		
			TMP.documentType = 'SALDO INICIAL' 
	From	#TmpResults TMP
			WHERE ISNULL(TMP.id_documentType,0) = 0;

	Update	TMP
	Set		TMP.inventoryReason = F.[name]
	From	#TmpResults TMP
			Inner Join dbo.InventoryReason F On (F.id = TMP.id_inventoryReason);


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


	Update	TMP
	Set		TMP.warehouse = J.[name]
	From	#TmpResults TMP
			Inner Join dbo.Warehouse J On (J.id = TMP.id_warehouse);



	Update	TMP
	Set		TMP.warehouseLocation = K.[name]
	From	#TmpResults TMP
			Inner Join dbo.WarehouseLocation K On (K.id = TMP.id_warehouseLocation);

	Update	#TmpResults
	Set		warehouseLocationEntry = Case When [entry] > 0 Then warehouseLocation Else '' End;


	Update	TMP
	Set		TMP.metricUnit = H.code
	From	#TmpResults TMP
			Inner Join dbo.MetricUnit H On (H.id = TMP.id_metricUnit);


	Update	TMP
	Set		TMP.code_item = G.masterCode + ' - ' + G.[name],
			TMP.id_itemType = G.id_itemType,
			TMP.id_presentation = G.id_presentation
	From	#TmpResults TMP
			Inner Join dbo.Item G On (G.id = TMP.id_item);


	Update	TMP
	Set		TMP.itemType = itemType.[name]
	From	#TmpResults TMP
			Inner Join dbo.ItemType itemType On (TMP.id_itemType = itemType.id);

	Update	TMP
	Set		TMP.itemSize = itemSize.[name]
	From	#TmpResults TMP
			Inner Join dbo.ItemGeneral itemGeneral On (itemGeneral.id_Item = TMP.id_item)
			Inner Join dbo.ItemSize itemSize On (itemSize.id = itemGeneral.id_size);

	Update	TMP
	Set		TMP.id_presentacionMetricUnit = presentation.id_metricUnit,
			TMP.ItemPresentationValue = presentation.minimum * presentation.maximum
	From	#TmpResults TMP
			Inner Join dbo.presentation presentation On (TMP.id_presentation = presentation.id);

	Update	TMP
	Set		TMP.ItemMetricUnit = presentationMetricUnit.code
	From	#TmpResults TMP
			Inner Join dbo.MetricUnit presentationMetricUnit On (TMP.id_presentacionMetricUnit = presentationMetricUnit.id);


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


	Update	TMP
	Set		TMP.Provider_name = PProveedor.fullname_businessName,
			TMP.isCopacking = IsNull(PProveedor.isCopacking, 0),
			TMP.nameProviderShrimp = PLPUP.[name],
			TMP.productionUnitProviderPool = PLPUPP.[name]
	From	#TmpResults TMP
			Left Join dbo.Person PProveedor On (PProveedor.id = TMP.id_provider) 
			Left Join dbo.ProductionUnitProvider PLPUP on (PLPUP.id = TMP.id_productionUnitProvider)
			Left Join dbo.ProductionUnitProviderPool PLPUPP  on (PLPUPP.id = TMP.id_productionUnitProviderPool);


	Select	id,
			id_document,
			document,
			nameDocumentState,
			id_documentType,
			documentType,
			id_inventoryReason,
			inventoryReason,
			emissionDate,
			id_item,
			code_item,
			id_metricUnit,
			metricUnit,
			id_lot,
			number,
			internalNumber,
			id_warehouse,
			warehouse,
			id_warehouseLocation,
			warehouseLocation,
			id_warehouseExit,
			warehouseExit,
			id_warehouseLocationExit,
			warehouseLocationExit,
			id_warehouseEntry,
			warehouseEntry,
			id_warehouseLocationEntry,
			warehouseLocationEntry,
			previousBalance,
			previousBalanceCost,
			[priceCost],
			[entry],
			[entryCost],
			[exit],
			[exitCost],
			balance,
			balanceCost,
			balanceCutting,
			balanceCuttingCost,
			numberRemissionGuide,
			idCompany,
			nameBranchOffice,
			nameDivision,
			nameCompany,
			dateCreate,
			Provider_name,
			isCopacking,
			nameProviderShrimp,
			productionUnitProviderPool,
			itemSize,
			itemType,
			ItemMetricUnit,
			ItemPresentationValue,
			lotMarked
	From	#TmpResults
	Order	By emissionDate Asc, dateCreate Desc;

End
