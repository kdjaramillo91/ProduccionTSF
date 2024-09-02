SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


Create PROCEDURE [dbo].[pi_Consultar_Movimientos_Procesos_Internos_Costo]
(
	@id INT,     
	@fechaInicio datetime,
	@fechaFin datetime
)
as



DECLARE @codigoParametroReportePI CHAR(5)	= 'PRMPI';
DECLARE @codigoTablaNaturaleza CHAR(5)	= 'NMMGI';
DECLARE @codigoNaturalezaIngreso CHAR(1)	= 'I';
DECLARE @codigoNaturalezaEgreso CHAR(1)	= 'E';
DECLARE @codigoStateAprobado CHAR(2)		= '03' ;
DECLARE @codigoGrupoOrigen CHAR(9)		= '1. ORIGEN' ;
DECLARE @codigoGrupoMateriaPrima CHAR(16)		= '2. MATERIA_PRIMA' ;
DECLARE @codigoGrupoLiquidacion CHAR(14)		= '3. LIQUIDACION' ;
DECLARE @codigoGrupoDestino CHAR(10)		= '4. DESTINO' ;
DECLARE @codigoReasonExcludeReversoMateriaPrima CHAR(5)		='IRMPP';
DECLARE @codigoReasonExcludeReversoLiquidacion CHAR(4)		='ERLP';


DECLARE @codigoLotStatePendienteRecepcion CHAR(2) ='01';
DECLARE @codigoLotStatePendienteAnulado CHAR(2) ='09';
DECLARE @codigoLotStatePendienteRecepcionado CHAR(2) ='02';
DECLARE @codigoLotStatePendienteLiquidacionParcial CHAR(2) ='03';

DECLARE @idStateDocumentoAprobado INT; 
DECLARE @idNaturalezaIngreso INT; 
DECLARE @idNaturalezaEgreso INT;
DECLARE @idReasonExcludeReversoMateriaPrima INT;
DECLARE @idReasonExcludeReversoLiquidacion  INT;

DECLARE @idLotStatePendienteRecepcion INT;
DECLARE @idLotStatePendienteAnulado INT;
DECLARE @idLotStatePendienteRecepcionado INT;
DECLARE @idLotStatePendienteLiquidacionParcial INT;

begin


Set NoCount On 

CREATE TABLE #TmpProcessForReport
(
	id_ProductionProcess INT,
	id_Warehouse INT
)

CREATE TABLE #TmpCostosMov
(
	id_Item INT,
	costo decimal(16,6),
	anio int,
	mes int, 
	Periodo  varchar(7)
)

CREATE TABLE #TmpMovimentosFiltrados
(
	IdLot INT,
	IdInventoryMoveDetail INT,
	
	periodo varchar(7),
	id_Warehouse INT,
	grupo varchar(16),
	id_item INT,
	cantidad decimal(14,6),
	amountCostUnit decimal(14,6),	
	id_originLot INT,
	anio INT,
	mes INT,
);

CREATE TABLE #TmpMovimientosView
(
	periodo varchar(7),
	periodoValores  varchar(50),
	nameBodega varchar(50),
	grupo varchar(16),
	libras decimal(14,6),
	costos decimal(14,6),
);

set @idStateDocumentoAprobado = (select	 top 1 id from	DocumentState where code = @codigoStateAprobado);

set @idReasonExcludeReversoMateriaPrima = 
      (select top 1 id from inventoryreason where code  = @codigoReasonExcludeReversoMateriaPrima);

set @idReasonExcludeReversoLiquidacion = 
      (select top 1 id from inventoryreason where code  = @codigoReasonExcludeReversoLiquidacion);
	
set @idLotStatePendienteRecepcion = (select	top 1 id  from	ProductionLotState  where  code = @codigoLotStatePendienteRecepcion);
set @idLotStatePendienteAnulado = (select	top 1 id  from	ProductionLotState  where  code = @codigoLotStatePendienteAnulado);
set @idLotStatePendienteRecepcionado = (select	top 1 id  from	ProductionLotState  where  code = @codigoLotStatePendienteRecepcionado);
set @idLotStatePendienteLiquidacionParcial = (select	top 1 id  from	ProductionLotState  where  code = @codigoLotStatePendienteLiquidacionParcial);


	set @idNaturalezaIngreso = 
	(select top 1 id 
	from AdvanceParametersDetail 
	where id_AdvanceParameters  = 
	(select top 1 id from AdvanceParameters where code = @codigoTablaNaturaleza)
	and valueCode = @codigoNaturalezaIngreso);

	set @idNaturalezaEgreso = 
	(select top 1 id 
	from AdvanceParametersDetail 
	where id_AdvanceParameters  = 
	(select top 1 id from AdvanceParameters where code = @codigoTablaNaturaleza)
	and valueCode = @codigoNaturalezaEgreso);

	-- Bodegas / Procesos
	insert into #TmpProcessForReport
	select	p.id, p.id_warehouse 
	from	Setting  a
			inner join SettingDetail b
			on b.id_setting = a.id	    
			inner join ProductionProcess p 
			on p.code = b.value
	where	a.code = @codigoParametroReportePI
			and  p.isActive = 1 

	-- movimientos de EGRESOS MATERIALES
	insert into #TmpMovimentosFiltrados
	select  p.id [IdLot],
			0 [IdInventoryMoveDetail],
			cast(YEAR(p.receptionDate) as varchar(4))+'-' +right('00'+ cast(MONTH(p.receptionDate) as varchar(2)) ,2) [periodo],
			d.id_warehouse,
			@codigoGrupoMateriaPrima [grupo],
			d.id_item,
			(d.quantityRecived*-1) [cantidad],
			0 amountCostUnit,
			d.id_originLot,
			YEAR(p.receptionDate) [anio],
			MONTH(p.receptionDate)[mes]
	from  Productionlot p 
	inner join #TmpProcessForReport pp
	on pp.id_ProductionProcess = p.id_productionProcess
	inner join ProductionLotDetail d  on
	d.id_productionLot = p.id
	where p.id_ProductionLotState not in (@idLotStatePendienteRecepcion,@idLotStatePendienteAnulado)
	and p.receptionDate <= isnull(@fechaFin,p.receptionDate )
	and p.receptionDate >= isnull(@fechaInicio,p.receptionDate)

	-- ORIGEN
	insert into #TmpMovimentosFiltrados
	select  a.id_originLot [IdLot],
			d.id [IdInventoryMoveDetail],
			cast(YEAR(m.emissionDate) as varchar(4))+'-'   +right('00'+ cast(MONTH(m.emissionDate) as varchar(2)) ,2) [periodo],
			d.id_warehouse,
			@codigoGrupoOrigen [grupo],
			d.id_item,
			(d.entryAmount- d.exitAmount) [cantidad],
			d.unitPriceMove [amountCostUnit],
			0,
			YEAR(m.emissionDate) [anio],
			MONTH(m.emissionDate)[mes]
	from 
	#TmpMovimentosFiltrados a
	inner join inventoryMoveDetail d
	on d.id_lot = a.id_originLot
	and d.id_item = a.id_item
	and d.id_warehouse = a.id_Warehouse
	inner join inventoryMove e on
	e.id = d.id_inventoryMove
	inner join InventoryReason r on
	r.id = e.id_inventoryReason
	
	inner join Document m on 
	m.id = e.id

	where m.emissionDate <= isnull( @fechaFin, m.emissionDate  )
	and m.emissionDate  >= isnull(@fechaInicio,m.emissionDate )
	and m.id_documentState = @idStateDocumentoAprobado
	and e.idNatureMove =  @idNaturalezaIngreso
	and r.id not in (@idReasonExcludeReversoMateriaPrima);
	
	-- LIQUIDACION
	insert into #TmpMovimentosFiltrados
	select  p.id [IdLot],
			0 [IdInventoryMoveDetail],
			cast(YEAR(p.receptionDate) as varchar(4)) +'-'  +right('00'+ cast(MONTH(p.receptionDate) as varchar(2)) ,2) [periodo],
			d.id_warehouse,
			@codigoGrupoLiquidacion [grupo],
			d.id_item,
			(d.quantity ) [cantidad],
			0 amountCostUnit,
			0,
			YEAR(p.receptionDate) [anio],
			MONTH(p.receptionDate)[mes]
	from  Productionlot p 
	inner join #TmpProcessForReport pp
	on pp.id_ProductionProcess = p.id_productionProcess
	inner join ProductionLotLiquidation d  on
	d.id_productionLot = p.id
	where p.id_ProductionLotState 
	not in (@idLotStatePendienteRecepcion,@idLotStatePendienteAnulado,
	@idLotStatePendienteRecepcionado, @idLotStatePendienteLiquidacionParcial)
	and p.receptionDate <=isnull( @fechaFin,p.receptionDate)
	and p.receptionDate >=ISNULL( @fechaInicio, p.receptionDate  )

	-- DESTINO
	insert into #TmpMovimentosFiltrados
	select  p.IdLot [IdLot],
			d.id [IdInventoryMoveDetail],
			cast(YEAR(m.emissionDate) as varchar(4)) +'-'  +right('00'+ cast(MONTH(m.emissionDate) as varchar(2)) ,2) [periodo],
			d.id_warehouse,
			@codigoGrupoDestino [grupo],
			d.id_item,
			((d.entryAmount - d.exitAmount)) [cantidad],
			d.unitPriceMove [amountCostUnit],
			0,
			YEAR(m.emissionDate) [anio],
			MONTH(m.emissionDate)[mes]
	from #TmpMovimentosFiltrados p
			inner join inventoryMoveDetail d
			on d.id_lot = p.IdLot
			and d.id_item = p.id_item
			and d.id_warehouse = p.id_Warehouse
			inner join inventoryMove e on
			e.id = d.id_inventoryMove
			inner join InventoryReason r on
			r.id = e.id_inventoryReason
			
			inner join Document m on 
			m.id = e.id

			where 
			/*m.emissionDate <= @fechaFin 
			and m.emissionDate  >= @fechaInicio and */
			m.id_documentState = @idStateDocumentoAprobado
			and e.idNatureMove =  @idNaturalezaEgreso
			and r.id not in (@idReasonExcludeReversoLiquidacion)
			and p.grupo = @codigoGrupoLiquidacion;


	insert into #TmpCostosMov
	select	l.id_item,
			max(d.unitPriceMove) [costo],
			max(l.anio), max(l.mes),
			 l.periodo
	from	inventoryMoveDetail d
			inner join inventoryMove i
			on i.id = d.id_inventoryMove
			inner join Document t on t.id = i.id
			inner join 
			(
				select periodo,id_item,
					   max(anio) anio, max(mes) mes
				from #TmpMovimentosFiltrados
				where  grupo in (@codigoGrupoMateriaPrima,@codigoGrupoLiquidacion ) 
				group by periodo,id_item  
						 
			)  l on 
			l.id_item = d.id_item
			and l.anio = YEAR(t.emissionDate) 
			and l.mes = MONTH(t.emissionDate)
	where  unitPriceMove > 0
	and t.id_documentState = @idStateDocumentoAprobado
	group by l.periodo, l.id_item
		    	
	update a
	set  a.amountCostUnit = b.costo
	from #TmpMovimentosFiltrados a
	inner join #TmpCostosMov b
	on b.anio = a.anio
	and b.mes = a.mes
	and b.id_Item = a.id_item
	where a.grupo in(@codigoGrupoMateriaPrima,@codigoGrupoLiquidacion )
	
	insert into #TmpMovimientosView
	select   x.periodo [periodo],
			'' [periodoValores],
			x.name [nameBodega],
			x.grupo [grupo],
			x.libras [libras],
			x.costoLibras [costos]
	from 
	(
	select  a.id_Warehouse,
			w.name,
			it.id,
			it.masterCode,
			a.grupo,
			a.periodo,
			a.cantidad,
			a.amountCostUnit,
			(CASE 
			  WHEN Y.factor IS NULL THEN (presentation.minimum * presentation.maximum  ) 
			  ELSE ( (presentation.minimum * presentation.maximum  )  * Y.factor) 
			END ) * a.cantidad [libras],
			(CASE 
			  WHEN Y.factor IS NULL THEN (presentation.minimum * presentation.maximum  ) 
			  ELSE ( (presentation.minimum * presentation.maximum  )  * Y.factor) 
			END ) * a.cantidad  * a.amountCostUnit [costoLibras],
			a.IdLot,
			a.IdInventoryMoveDetail

	from   #TmpMovimentosFiltrados a
	inner join Warehouse w on 
	w.id  = a.id_Warehouse
	inner join item it on it.id = a.id_item
	LEFT JOIN dbo.presentation presentation WITH (NOLOCK) 
	ON (it.id_presentation = presentation.id)
	LEFT JOIN dbo.MetricUnit presentationMetricUnit WITH (NOLOCK) 
	ON (presentation.id_metricUnit = presentationMetricUnit.id)

	LEFT JOIN MetricUnitConversion Y
	ON presentationMetricUnit.id = Y.id_metricOrigin
	and Y.id_metricDestiny = 4
	) x
	/*order by periodo,
			 w.name,
			 grupo*/


	select periodo,
		  'Suma de Cantidad Lbs' [periodoValores],
		   nameBodega,
		   grupo,
		   sum(libras) [valores]
	from #TmpMovimientosView a
	group by periodo,
		     nameBodega,
		     grupo
	union all
	select periodo,
		   'Suma de Costo Total' [periodoValores],
		   nameBodega,
		   grupo,
		   sum(costos) [valores]
	from #TmpMovimientosView a
	group by periodo,
		   nameBodega,
		   grupo

	order by periodo,
			 periodoValores,
			 nameBodega,
			 grupo
	 
/*
	order by a.periodo,
	a.periodoCantidad, 
	a.periodoCosto,
	a.grupo
	*/


END

-- exec  pi_Consultar_Movimientos_Procesos_Internos_Costo 0, '2021-08-01', '2021-08-30'

-- exec  pi_Consultar_Movimientos_Procesos_Internos_Costo null,null