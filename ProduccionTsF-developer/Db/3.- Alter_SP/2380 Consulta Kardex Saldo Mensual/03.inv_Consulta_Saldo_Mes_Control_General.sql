-- inv_Consulta_Saldo_Mes_Control_General 2, 6973, '20240731',1,'GITEMLOT',29,15581,null,1,NULL,NULL,0
                                                                                    --/=>PLOT NULL           


create or ALTER Procedure [dbo].[inv_Consulta_Saldo_Mes_Control_General]
(
	@id_company				int,
	@id_item				int  null,
	@fecha_corte			DateTime null,
	@consolidado			bit,	
	@groupby				varchar(8) null,
	@id_warehouse   		int null,
	@id_warehouseLocation 	int null,
	@id_productionLot		int null,	
	@requiresLot			bit	null,  
	@id_itemlist			varchar(max) = null,
	@id_inventorydetails	varchar(max) = null,
	@with_negatives			bit = 0,
	@printDebug				bit = 0
)
As
Begin

	Set NoCount On

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Declaración de Variables y Objetos Temporales...'
	End

	Declare @setting_periodo_inicial varchar(6) = (select top 1 [value]  from Setting where code = 'PSALINI');	
	DECLARE @CODIGO_ESTADO_APROBADO		VARCHAR(2) = '03';
	DECLARE @CODIGO_ESTADO_CONCILIADO	VARCHAR(2) = '16';

	DECLARE @GROUPBY_ITEM  CHAR(8)						=  'G000ITEM'; 
	DECLARE @GROUPBY_ITEM_LOTE CHAR(8)					=  'GITEMLOT';
	DECLARE @GROUPBY_LOTE CHAR(8)					=  'GITLOT';
	DECLARE @GROUPBY_BODEGA_ITEM  CHAR(8)				=  'GBODITEM';
	DECLARE @GROUPBY_BODEGA_UBICA_ITEM  CHAR(8)			=  'GBODUBIT';
	DECLARE @GROUPBY_BODEGA_UBICA_LOTE_ITEM  CHAR(8)	=  'GBUBLTIT';
	DECLARE @MAX_INT_NEGATIVE INT						=  -2147483648 ;
	

	declare @anio		INT;
	declare @mes		INT;
	declare @periodo	VARCHAR(6);
	declare @fechaConsultaMovimientos DateTime;
	declare @anio_saldo_inicial int;
	declare @mes_saldo_inicial int;
	declare @max_having_sum_value int = 0;


	create table  #ItemsSelected 
	(
		Id int		
	);

	Declare @ControlPeriodo Table   
	(
		id_company			Int,	
		Anio				Int,	
		Mes					Int,	
		id_warehouse		Int				
	);

	CREATE TABLE  #ControlSaldoNoFiltrado (
		Id int,
		id_item					Int,
		id_warehouse			Int,
		id_warehouseLocation	Int,
		id_productionLot		Int,
		numberLot				varchar(20),
		internalNumberLot		varchar(20),
		saldo					decimal(20,6),
		id_metricUnit			Int,
		fecha_ini_movimientos   Datetime
	);

	create table  #ControlSaldo 
	(
		Id int,
		id_item					Int,
		id_warehouse			Int,
		id_warehouseLocation	Int,
		id_productionLot		Int,
		numberLot				varchar(20),
		internalNumberLot		varchar(20),
		fechaRecepcion			datetime,
		saldo					decimal(20,6),
		id_metricUnit			Int,
		fecha_ini_movimientos   Datetime
	);

	Declare @ControlSaldoMovimiento Table  (
		Id int,
		id_item					Int,
		id_warehouse			Int,
		id_warehouseLocation	Int,
		id_productionLot		Int,
		saldo					decimal(20,6),
		id_metricUnit			Int,
		fecha_ini_movimientos   Datetime
	)

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Creacion de Indices y Objetos Temporales...'
	End

	--Create Index IDX_Tmp_items_selected On #ItemsSelected (Id);
	
	--delete from inventorymovedetailids;
	 
	
	if(ISNULL(@id_warehouse,0) != 0)
	BEGIN 
		Create Index IDX_Tmp_ControlSaldoMovimiento_id_warehouse On #ControlSaldo (id_warehouse);								
	END
	
	if(ISNULL(@id_warehouseLocation,0) != 0)
	BEGIN 
		Create Index IDX_Tmp_ControlSaldoMovimiento_id_warehouseLocation On #ControlSaldo (id_warehouseLocation);		
	END
	
	if(ISNULL(@id_productionLot,0) != 0)
	BEGIN 
		Create Index IDX_Tmp_ControlSaldoMovimiento_id_productionLot On #ControlSaldo (id_productionLot);				
	END
	
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Asignación de Variables...'
	End
	
	SET @fecha_corte = ISNULL(@fecha_corte,GETDATE());	
	SET @anio = YEAR(@fecha_corte); 
	SET @mes = MONTH(@fecha_corte);
	SET @anio_saldo_inicial = SUBSTRING(@setting_periodo_inicial,1,4);
	SET @mes_saldo_inicial = SUBSTRING(@setting_periodo_inicial,5,2);
	SET @periodo = SUBSTRING(trim(CONVERT(varchar,@fecha_corte,112)),1,6 ) 

    SET @id_item				=  ISNULL(@id_item,0);
    SET @id_warehouse   		=	ISNULL(@id_warehouse,0);
    SET @id_warehouseLocation 	=	ISNULL(@id_warehouseLocation,0);
    SET @id_productionLot		=	ISNULL(@id_productionLot,0);

	IF ISNULL(@with_negatives,0) = 1
	BEGIN
		SET @max_having_sum_value = @MAX_INT_NEGATIVE;
	END

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Filtrado e Items...'
	End

	if(@requiresLot is not null)
	BEGIN
		insert into #ItemsSelected  
		SELECT	id_item  			
		FROM	ItemInventory   
		WHERE	requiresLot = @requiresLot  
	
		IF @id_item !=  0
			DELETE #ItemsSelected WHERE Id != @id_item    
	END
	ELSE
	BEGIN
		IF @id_item !=0
		BEGIN
			insert into #ItemsSelected  values(@id_item );
		END		
		ELSE
		BEGIN
			IF (LEN(ISNULL(@id_itemlist,'')) > 0)
			BEGIN
				insert into #ItemsSelected  
				SELECT value from STRING_SPLIT ( @id_itemlist, '|' )
			END
			ELSE
			BEGIN
				insert into #ItemsSelected  
				select  E.id  
				from	dbo.Item E WITH (NOLOCK)
				INNER JOIN dbo.InventoryLine F WITH (NOLOCK) ON (F.id = E.id_inventoryLine)
				WHERE	E.isActive = 1
						AND  (F.code = 'PP' OR F.code = 'PT') 
			END
		END

	END
	


	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Obtencion Control Saldo: Insert tabla: ##ControlPeriodo... Periodo:'+@periodo
	End		

	INSERT INTO @ControlPeriodo
	SELECT	grp.id_company,  
			SUBSTRING(grp.periodoControl,1,4) anio, 
			SUBSTRING(grp.periodoControl,5,2) mes ,
			grp.id_warehouse
	FROM
	(
		SELECT	CTL.id_company, 
				CTL.id_warehouse,
				cast(max(CTL.periodoControl) as varchar) periodoControl
		FROM 
		(
			SELECT  id_company,						
					id_warehouse,
					CAST(CAST(Anio AS VARCHAR)+ right( '00' + cast( Mes AS varchar(2)), 2 ) as int) as periodoControl 
			FROM	MonthlyBalanceControl
			WHERE	id_company = @id_company
					and id_warehouse =  (case when @id_warehouse = 0 then id_warehouse  else @id_warehouse END)
					AND IsValid = 1	
		) CTL
		WHERE CTL.periodoControl < @periodo
		GROUP BY CTL.id_company, CTL.id_warehouse
	)GRP

	
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Obtencion saldo: Insert tabla: #ControlSaldoNoFiltrado... Periodo:'+@periodo
	End	
	

	INSERT INTO #ControlSaldoNoFiltrado
	SELECT  b.id, 
			b.id_item,
			b.id_warehouse,
			b.id_warehouseLocation,
			b.id_productionLot,
			b.sequencial_productionLot,
			b.number_productionLot,
			b.SaldoActual,
			b.id_metric_unit,
			DATEADD(month,1,DATEFROMPARTS(c.Anio, c.Mes, 1) ) 
	from @ControlPeriodo c	
	inner join MonthlyBalance b on	
	b.id_company = c.id_company
	and b.Anio  = c.Anio
	and b.Periodo =  c.Mes
	and b.id_warehouse= c.id_warehouse
	

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Obtencion saldo: Insert tabla: #ControlSaldo... Periodo:'+@periodo
	End	


	INSERT INTO #ControlSaldo
	SELECT  b.Id,
			b.id_item,
			b.id_warehouse,
			b.id_warehouseLocation,
			b.id_productionLot,
			b.numberLot,
			b.internalNumberLot,
			pl.receptionDate,
			b.saldo,
			b.id_metricUnit,
			b.fecha_ini_movimientos 
	FROM	#ControlSaldoNoFiltrado b
			INNER JOIN  #ItemsSelected i ON i.Id = b.id_item
			LEFT JOIN ProductionLot pl  ON b.id_productionLot = pl.id  
	WHERE   b.id_warehouseLocation = (CASE WHEN @id_warehouseLocation = 0 THEN  b.id_warehouseLocation ELSE @id_warehouseLocation END )
			AND b.id_productionLot  = (CASE WHEN @id_productionLot = 0 THEN  b.id_productionLot ELSE @id_productionLot END )
	

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Filtrado de informacion del saldo...'
	End	
 

	SET @fechaConsultaMovimientos = (select  top 1 fecha_ini_movimientos from #ControlSaldo);
	
	if(@fechaConsultaMovimientos is null)
	BEGIN
		SET @fechaConsultaMovimientos =  DATEFROMPARTS(@anio_saldo_inicial, @mes_saldo_inicial, 1);
		
	END

	
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Obtencion movimientos: Insert tabla: @ControlSaldoMovimiento..., fecha Inicio:'+cast( @fechaConsultaMovimientos as varchar);
	End	

	Insert	Into @ControlSaldoMovimiento	 
	Select	A.id,
			A.id_item,
			A.id_warehouse,
			A.id_warehouseLocation,
			A.id_lot,
			A.entryAmount-A.exitAmount,
			A.id_metricUnit,
			C.emissionDate			
	From	(SELECT imv.id,
					imv.id_inventoryMove,
			        imv.id_item,
			        imv.id_warehouse,	
			        imv.id_warehouseLocation,
			        imv.id_lot,
			        imv.entryAmount,
					imv.exitAmount,
					imv.id_metricUnit
			 FROM dbo.InventoryMoveDetail imv  WITH (NOLOCK)
			 --INNER JOIN #ItemsSelected  i ON 
			 --i.Id = imv.id_item
			 WHERE imv.id_warehouse = (CASE WHEN @id_warehouse=0 THEN imv.id_warehouse ELSE @id_warehouse END)
			 AND  imv.id_warehouseLocation = (CASE WHEN @id_warehouseLocation=0 THEN imv.id_warehouseLocation ELSE @id_warehouseLocation END)
			 AND  imv.id_lot = (CASE WHEN @id_productionLot=0 THEN imv.id_lot ELSE @id_productionLot END)
			 
			 ) A			
			Inner Join (
			  SELECT		d.id ,
							d.id_documentState,
							d.emissionDate,
							d.id_documentType,
							d.number,
							d.reference
			  FROM			dbo.Document d  WITH (NOLOCK)
			  INNER JOIN	EmissionPoint e 
			  on e.id = d.id_emissionPoint
			  WHERE e.id_company = @id_company
					AND (d.emissionDate >= @fechaConsultaMovimientos AND d.emissionDate < DateAdd(day, 1, @fecha_corte))
			  )  C On (C.id = A.id_inventoryMove)
			Inner Join dbo.DocumentState D On (D.id = C.id_documentState)		
			INNER JOIN #ItemsSelected  i ON 
			i.Id = A.id_item
	Where	D.CODE in(@CODIGO_ESTADO_APROBADO,@CODIGO_ESTADO_CONCILIADO );
				
	-- Filtro 
	IF (LEN(ISNULL(@id_inventorydetails,'')) > 0)
	Begin
		DELETE T
		FROM @ControlSaldoMovimiento T
		INNER JOIN (SELECT value as Id from STRING_SPLIT ( @id_inventorydetails, '|' )) TT
		ON T.Id = TT.Id;
	End

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Filtrado de informacion de movimientos...'
	End	
	--1401201
	--1431344
	select * from @ControlSaldoMovimiento
  

	Insert	Into #ControlSaldo	 
	Select	MAX(a.Id),
			a.id_item,
			a.id_warehouse,
			a.id_warehouseLocation,
			a.id_productionLot,
			MAX(ISNULL(pl.number, lt.number)) as number,  
			MAX(ISNULL(pl.internalNumber, lt.internalNumber)) as internalNumber,  
			max(pl.receptionDate) as receptionDate,
			sum(a.saldo),
			Max(a.id_metricUnit),
			MAX(a.fecha_ini_movimientos) 			 
	From	@ControlSaldoMovimiento	 a
			LEFT JOIN ProductionLot pl ON a.id_productionLot = pl.id  
			LEFT JOIN Lot lt ON a.id_productionLot = lt.id  
	GROUP BY	id_warehouse,
				id_warehouseLocation,
				id_productionLot,
				id_item

	
			
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Retorno del saldo...'
	End

	if(@consolidado = 1)
	BEGIN
		IF(ISNULL(@groupby,'') != '')
		BEGIN
			IF(@groupby = @GROUPBY_ITEM )
			BEGIN
				SELECT	MAX(0) AS id,
						MAX(@anio) as Anio,
						MAX(@mes) as Periodo,
						id_item,
						0 AS id_warehouse,
						0 AS id_warehouseLocation,
						0 AS id_productionLot,
						null as numberLot,
						null as internalNumberLot,
						0 as id_metricUnit,
						SUM(isnull(saldo,0)) as SaldoActual,
						null as fechaRecepcion
				FROM	#ControlSaldo
				GROUP BY id_item
				HAVING SUM(saldo) > @max_having_sum_value  
			END
			ELSE
			BEGIN
				IF(@groupby = @GROUPBY_ITEM_LOTE)
				BEGIN
				 --select * from #ControlSaldo
					SELECT	MAX(0) AS id,
							MAX(@anio) as Anio,
							MAX(@mes) as Periodo,
							a.id_item,
							0  AS id_warehouse,
							0 AS id_warehouseLocation,
							isnull(a.id_productionLot,0) id_productionLot,
							max(a.numberLot) as numberLot,
							max(a.internalNumberLot) as internalNumberLot,							
							0 as id_metricUnit,
							SUM(isnull(a.saldo,0)) as SaldoActual,
							max(a.fechaRecepcion) as fechaRecepcion
					FROM	#ControlSaldo a							
					GROUP BY a.id_item, a.id_productionLot, a.fechaRecepcion  
					HAVING SUM(a.saldo) > @max_having_sum_value  
				END
				ELSE 
				BEGIN
				IF(@groupby = @GROUPBY_LOTE)
				BEGIN
				SELECT	
							isnull(a.id_productionLot,0) id_productionLot,
							SUM(isnull(a.saldo,0)) as SaldoActual
					FROM	#ControlSaldo a							
					GROUP BY a.id_productionLot
					HAVING SUM(a.saldo) > @max_having_sum_value 
				END
				ELSE
				BEGIN
					IF(@groupby = @GROUPBY_BODEGA_ITEM)
					BEGIN
						SELECT	MAX(0) AS id,
								MAX(@anio) as Anio,
								MAX(@mes) as Periodo,
								id_item,
								isnull(id_warehouse,0) id_productionLot,
								0 AS id_warehouseLocation,
								0 AS id_productionLot,
								null as numberLot,
								null as internalNumberLot,
								0 as id_metricUnit,
								SUM(isnull(saldo,0)) as SaldoActual,
								null as fechaRecepcion
						FROM	#ControlSaldo
						GROUP BY id_warehouse, id_item
						HAVING SUM(saldo) > @max_having_sum_value  
					END
					ELSE
					BEGIN
						IF(@groupby = @GROUPBY_BODEGA_UBICA_ITEM)
						BEGIN
								SELECT	MAX(0) AS id,
								MAX(@anio) as Anio,
								MAX(@mes) as Periodo,
								id_item,
								isnull(id_warehouse,0) id_warehouse,
								isnull(id_warehouseLocation,0) id_warehouseLocation,
								0 AS id_productionLot,
								null as numberLot,
								null as internalNumberLot,
								Max(isnull(id_metricUnit,0)) as id_metricUnit,
								SUM(isnull(saldo,0)) as SaldoActual,
								null as fechaRecepcion
							FROM	#ControlSaldo
							GROUP BY id_warehouse,id_warehouseLocation, id_item
							HAVING SUM(saldo) > @max_having_sum_value  
						END
						ELSE
						BEGIN
							IF(@groupby = @GROUPBY_BODEGA_UBICA_LOTE_ITEM)
							BEGIN
							    
								
								SELECT	MAX(0) AS id,
										MAX(@anio) as Anio,
										MAX(@mes) as Periodo,
										id_item,
										isnull(id_warehouse,0) id_warehouse,
										isnull(id_warehouseLocation,0) id_warehouseLocation,
										isnull(id_productionLot,0)  id_productionLot,
										MAX(numberLot) as numberLot,
										MAX(internalNumberLot) as internalNumberLot,
										Max(isnull(id_metricUnit,0)) as id_metricUnit,
										SUM(isnull(saldo,0)) as SaldoActual,
										MAX(fechaRecepcion) as fechaRecepcion
								FROM	#ControlSaldo
								GROUP BY id_warehouse,id_warehouseLocation,id_productionLot, id_item
								HAVING SUM(saldo) >  @max_having_sum_value    
							END
						END
					END
				END
			END
		END
	END	 
	END
	ELSE
	BEGIN	
			SELECT  0 AS id,
					@anio as Anio,
					@mes as Periodo,
					a.id_item,
					isnull(a.id_warehouse,0) id_warehouse,
					isnull( a.id_warehouseLocation,0) id_warehouseLocation,
					isnull(a.id_productionLot,0) id_productionLot,
					a.numberLot,
					a.internalNumberLot,
					isnull(a.id_metricUnit,0) id_metricUnit,
					isnull(a.saldo,0) as SaldoActual,
					a.fechaRecepcion
			FROM   #ControlSaldo a
					LEFT JOIN ProductionLot pl ON a.id_productionLot= pl.id  
	END
 
  
END
