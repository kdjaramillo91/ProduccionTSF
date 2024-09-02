
create or ALTER Procedure [dbo].[inv_Consulta_Saldo_Mes_Control_General]
(
	@id_company				int,
	@id_item				int  null,
	@fecha_corte			DateTime null,
	@consolidado			bit,	
	@id_warehouse   		int null,
	@id_warehouseLocation 	int null,
	@id_productionLot		int null,	
	@requiresLot			bit	= 0,  
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

	declare @anio		INT;
	declare @mes		INT;
	declare @periodo	VARCHAR(6);
	declare @fechaConsultaMovimientos DateTime;
	declare @anio_saldo_inicial int;
	declare @mes_saldo_inicial int;

	
	Create Table  #ItemsSelected (
		Id int		
	);

	Create Table  #ControlSaldo (
		Id int,
		id_item					Int,
		id_warehouse			Int,
		id_warehouseLocation	Int,
		id_productionLot		Int,
		saldo					decimal(20,6),
		id_metricUnit			Int,
		fecha_ini_movimientos   Datetime
	);

	Create Table  #ControlSaldoMovimiento (
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
		Print Convert(VarChar, GetDate(), 114) + ' * Creaion de Indices y Objetos Temporales...'
	End

	Create Index IDX_Tmp_items_selected On #ItemsSelected (Id);

	if(ISNULL(@id_warehouse,0) != 0)
	BEGIN 	
		Create Index IDX_Tmp_ControlSaldo_id_warehouse On #ControlSaldo (id_warehouse);		
	END

	if(ISNULL(@id_warehouseLocation,0) != 0)
	BEGIN 
		Create Index IDX_Tmp_ControlSaldo_id_warehouseLocation On #ControlSaldo (id_warehouseLocation);				
	END

	if(ISNULL(@id_productionLot,0) != 0)
	BEGIN 
		Create Index IDX_Tmp_ControlSaldo_id_productionLot On #ControlSaldo (id_productionLot);						
	END

	
	if(ISNULL(@id_warehouse,0) != 0)
	BEGIN 
		Create Index IDX_Tmp_ControlSaldoMovimiento_id_warehouse On #ControlSaldoMovimiento (id_warehouse);								
	END

	if(ISNULL(@id_warehouseLocation,0) != 0)
	BEGIN 
		Create Index IDX_Tmp_ControlSaldoMovimiento_id_warehouseLocation On #ControlSaldoMovimiento (id_warehouseLocation);		
	END

	if(ISNULL(@id_productionLot,0) != 0)
	BEGIN 
		Create Index IDX_Tmp_ControlSaldoMovimiento_id_productionLot On #ControlSaldoMovimiento (id_productionLot);				
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


	IF @id_item IS NOT NULL  
		DELETE #ItemsSelected WHERE Id != @id_item  

	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Obtencion saldo: Insert tabla: #ControlSaldo... Periodo:'+@periodo
	End	

	INSERT INTO #ControlSaldo
	SELECT  b.id, 
			b.id_item,
			b.id_warehouse,
			b.id_warehouseLocation,
			b.id_productionLot,
			b.SaldoActual,
			b.id_metric_unit,
			DATEADD(month,1,DATEFROMPARTS(c.Anio, c.Mes, 1) ) 
	from 
	(  
	  SELECT [Anio] = MAX(CTL.Anio) over (partition by CTL.id_warehouse ),
			 [Mes] = MAX(CTL.Mes) over (partition by CTL.id_warehouse ),
			 CTL.id_company, 
			 CTL.id_warehouse
	  FROM 
	  (
		SELECT  id_company,
				Anio,
				Mes,
				id_warehouse,
				CAST(CAST(Anio AS VARCHAR)+ right( '00' + cast( Mes AS varchar(2)), 2 ) as int) as periodoControl 
		FROM	MonthlyBalanceControl
		WHERE	id_company = @id_company
				AND IsValid = 1
	 )  CTL 
	 WHERE CTL.periodoControl < @periodo
	 /*GROUP BY CTL.id_company, CTL.id_warehouse*/) c 
	inner join MonthlyBalance b on
	b.id_company = c.id_company
	and b.Anio  = c.Anio
	and b.Periodo =  c.Mes
	WHERE EXISTS
			(SELECT 'x' FROM #ItemsSelected i WHERE i.Id = b.id_item)
	
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Filtrado de informacion del saldo...'
	End	


	if(ISNULL(@id_warehouse,0) != 0)
	BEGIN 
		DELETE FROM #ControlSaldo WHERE id_warehouse != @id_warehouse;
	END

	if(ISNULL(@id_warehouseLocation,0) != 0)
	BEGIN 
		DELETE FROM #ControlSaldo WHERE id_warehouseLocation != @id_warehouseLocation;
	END

	if(ISNULL(@id_productionLot,0) != 0)
	BEGIN 
		DELETE FROM #ControlSaldo WHERE id_productionLot != @id_productionLot;
	END

	SET @fechaConsultaMovimientos = (select  top 1 fecha_ini_movimientos from #ControlSaldo);
	
	if(@fechaConsultaMovimientos is null)
	BEGIN
		SET @fechaConsultaMovimientos =  DATEFROMPARTS(@anio_saldo_inicial, @mes_saldo_inicial, 1);
	END
	

	If @printDebug = 1
	Begin
	select @fechaConsultaMovimientos;
		Print Convert(VarChar, GetDate(), 114) + ' * Obtencion movimientos: Insert tabla: #ControlSaldoMovimiento...';
	End	

	Insert	Into #ControlSaldoMovimiento	 
	Select	A.id,
			A.id_item,
			A.id_warehouse,
			A.id_warehouseLocation,
			A.id_lot,
			A.entryAmount-A.exitAmount,
			A.id_metricUnit,
			C.emissionDate			
	From	(SELECT id,
					id_inventoryMove,
			        id_item,
			        id_warehouse,
			        id_warehouseLocation,
			        id_lot,
			        entryAmount,
					exitAmount,
					id_metricUnit
			 FROM dbo.InventoryMoveDetail 
			 WHERE 
				EXISTS
					(SELECT 'x' FROM #ItemsSelected i WHERE i.Id = id_item)
			 ) A
			Inner Join dbo.InventoryMove B On (B.id = A.id_inventoryMove)
			Inner Join (
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
			  )  C On (C.id = B.id)
			Inner Join dbo.DocumentState D On (D.id = C.id_documentState)				
	Where	D.CODE in(@CODIGO_ESTADO_APROBADO,@CODIGO_ESTADO_CONCILIADO )
			AND (C.emissionDate >= @fechaConsultaMovimientos AND C.emissionDate < DateAdd(day, 1, @fecha_corte)) 			
			
			
	
	If @printDebug = 1
	Begin
		Print Convert(VarChar, GetDate(), 114) + ' * Filtrado de informacion de movimientos...'
	End	

	if(ISNULL(@id_warehouse,0) != 0)
	BEGIN 
		DELETE FROM #ControlSaldoMovimiento WHERE id_warehouse != @id_warehouse;
	END

	if(ISNULL(@id_warehouseLocation,0) != 0)
	BEGIN 
		DELETE FROM #ControlSaldoMovimiento WHERE id_warehouseLocation != @id_warehouseLocation;
	END

	if(ISNULL(@id_productionLot,0) != 0)
	BEGIN 
		DELETE FROM #ControlSaldoMovimiento WHERE id_productionLot != @id_productionLot;
	END

	Insert	Into #ControlSaldo	 
	Select	MAX(Id),
			id_item,
			id_warehouse,
			id_warehouseLocation,
			id_productionLot,
			sum(saldo),
			Max(id_metricUnit),
			MAX(fecha_ini_movimientos) 			 
	From	#ControlSaldoMovimiento	
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
			 SELECT MAX(0) AS id,
					MAX(@anio) as Anio,
					MAX(@mes) as Periodo,
					id_item,
					MAX(id_warehouse) AS id_warehouse,
					MAX(id_warehouseLocation) AS id_warehouseLocation,
					MAX(id_productionLot) AS id_productionLot,
					Max(id_metricUnit) as id_metricUnit,
					SUM(saldo) as SaldoActual
			FROM   #ControlSaldo
			GROUP BY id_item
	END
	ELSE
	BEGIN	
			SELECT  0 AS id,
				@anio as Anio,
				@mes as Periodo,
				id_item,
				id_warehouse,
				id_warehouseLocation,
				id_productionLot,
				id_metricUnit,
				saldo as SaldoActual
			FROM   #ControlSaldo
	END
 
  
END
