/****** Object:  StoredProcedure [dbo].[spRptCierreTurno]    Script Date: 12/05/2023 03:23:39 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROC [dbo].[spRptCierreTurno]--1245
	@id INT
AS
DECLARE @Fecha VARCHAR(15)
DECLARE @HoraInicio VARCHAR(5)
DECLARE @HoraFinal VARCHAR(5)
DECLARE @HoraI Datetime
DECLARE @HoraF Datetime
DECLARE @id_Turno INT
DECLARE @numeroLiquidacion VARCHAR(250)
DECLARE @estado VARCHAR(250)

Set NoCount On

set @Fecha = (Select CONVERT(VARCHAR(10),emissionDate,103) from LiquidationTurn where id = @id)
set @HoraInicio = (Select CONVERT(VARCHAR(5),liquidationTime) from LiquidationTurn where id = @id)
set @HoraI = (Select liquidationTime from LiquidationTurn where id = @id)
set @id_Turno = (Select id_turn from LiquidationTurn where id = @id)
set @numeroLiquidacion = (Select d.number from LiquidationTurn lt inner join document d on d.id = lt.id where lt.id = @id)
set @estado = (Select ds.name from LiquidationTurn lt 
							inner join document d on d.id = lt.id 
							inner join documentState ds on ds.id = d.id_documentState where lt.id = @id)

CREATE TABLE #TotalProceso(
	id INT,
	Cola NUMERIC(13,4),
	Entero NUMERIC(13,4),
	Total NUMERIC(13,4)
)

CREATE TABLE #Horas(
	idLote INT,
	HoraInicio	VARCHAR(6),
	HoraFin	VARCHAR(6)
)

CREATE TABLE #Data(
	Turno VARCHAR(50),
	Lote VARCHAR(50),
	proceso varchar(10),
	Proveedor VARCHAR(250),
	numeroLoquidacion	varchar(50),
	LiquidacionTurno	VARCHAR(250),
	Cola NUMERIC(13,4),
	Entero	NUMERIC(13,4),
	Total	NUMERIC(13,4),
	Observacion	VARCHAR(250),
	HoraInicio	VARCHAR(6),
	HoraFin	VARCHAR(6),
	Fecha	VARCHAR(12),
	Horas decimal(6,2),
	librasEnviadas NUMERIC(13,4),
	rendimiento NUMERIC(13,4),
	proces varchar(5),
	EstadoLote varchar(25)
)

INSERT INTO #TotalProceso
SELECT 
LCOC.id,
CASE WHEN PT.code = 'COL' THEN SUM(LCOD.quantityPoundsIL) else 0 end Cola,
CASE WHEN PT.code = 'COL' THEN 0 else SUM(LCOD.quantityPoundsIL) end Entero,
SUM(LCOD.quantityPoundsIL) Total
FROM LiquidationCartOnCart LCOC
INNER JOIN LiquidationCartOnCartDetail LCOD
on LCOD.id_LiquidationCartOnCart = LCOC.id
INNER JOIN ProcessType PT
on PT.id = LCOC.idProccesType
GROUP BY PT.code, LCOC.id

Insert Into #Horas
select Pl.id, Min(CONVERT(VARCHAR(5),OPD.timeInit)) HoraInicio,
	Max(CONVERT(VARCHAR(5),OPD.timeEnd)) HoraFin
from LiquidationCartOnCart LCOC
INNER JOIN LiquidationCartOnCartDetail LCOD
on LCOD.id_LiquidationCartOnCart = LCOC.id
INNER JOIN MachineProdOpening MPO
on LCOC.id_MachineProdOpening = MPO.id
Inner Join MachineProdOpeningDetail OPD
On OPD.id_MachineProdOpening = MPO.id
INNER JOIN MachineForProd MFP
ON MFP.id = LCOC.id_MachineForProd
INNER JOIN Turn T
on T.id = MPO.id_Turn
INNER JOIN ProcessType PT
on PT.id = LCOC.idProccesType
INNER JOIN Document DLOC
on DLOC.id = LCOC.id
INNER JOIN DocumentState DSDLOC
on DSDLOC.id = DLOC.id_documentState
 And DSDLOC.code <> '05'
INNER JOIN Document DMPO
on DMPO.id = MPO.id
INNER JOIN ProductionLot PL
on LCOC.id_ProductionLot = PL.id
WHERE Convert(varchar(10),DLOC.emissionDate,103) = @Fecha
AND t.ID = @id_Turno
group by Pl.id

INSERT INTO #Data
select distinct
	T.name Turno,
	Pl.internalNumber Lote,
	PT.name Proceso,
	PPLProvider.fullname_businessName Proveedor,
	pl.sequentialLiquidation numeroLoquidacion,
	@numeroLiquidacion,
	--MFP.name Máquina,
	--DSDLOC.code CodigoEstado,
	--DSDLOC.name Estado,
	TempTp.cola Cola,
	TempTp.entero Entero,
	TempTp.Total Total,
	(Select d.description from Document d where id = @id) Observacion,
	CONVERT(VARCHAR(5),TmpH.HoraInicio) HoraInicio,
	--@HoraInicio HoraInicio,
	--@HoraFinal HoraFin,
	CONVERT(VARCHAR(5),TmpH.HoraFin) HoraFin,
	Convert(varchar(10),DLOC.emissionDate,103) Fecha,
	--(DATEDIFF(MINUTE,@HoraI,CONVERT(TIME,T.timeInit)))/60.0 Horas,
	Case when TmpH.HoraFin = '00:00:00.0000000' then ((DATEDIFF(MINUTE,CONVERT(DATETIME,TmpH.HoraInicio),'23:59:59.000')) + 1) /60.0 else
	(DATEDIFF(MINUTE,CONVERT(DATETIME,TmpH.HoraFin),CONVERT(DATETIME,TmpH.HoraInicio)))/60.0 end Horas,
	PL.totalQuantityRecived librasEnviadas,
	case when pt.code = 'ENT' then 
	       case when  isnull(pl.[wholeSubtotalAdjust], 0) + isnull(pl.[wholeLeftover], 0) > 0
	            then isnull(pl.[wholeSubtotalAdjust],0)/(isnull(pl.[wholeSubtotalAdjust], 0) + isnull(pl.[wholeLeftover], 0)) else 0 end
	       else 
		   Case when isnull(pl.[tailLeftover],0) = 0 Then		   
			    case when (isnull(pl.[wholeLeftover], 0) - isnull(pl.[poundsGarbageTail], 0)) > 0
					 then pl.[subtotalTailAdjust]/(isnull(pl.[wholeLeftover], 0) - isnull(pl.[poundsGarbageTail], 0)) else 0 end 
				Else 
				case when (isnull(pl.[tailLeftover], 0) - isnull(pl.[poundsGarbageTail], 0)) > 0
					 then pl.[subtotalTailAdjust]/(isnull(pl.[tailLeftover], 0) - isnull(pl.[poundsGarbageTail], 0)) else 0 end End
	 end * 100 as [PorcentajeRendimiento],
	pt.code,
	--co.logo as LogoP,
	--co.logo2 as LogoP2,
	PLS.code as [EstadoLote]
from LiquidationCartOnCart LCOC
INNER JOIN LiquidationCartOnCartDetail LCOD
on LCOD.id_LiquidationCartOnCart = LCOC.id
INNER JOIN MachineProdOpening MPO
on LCOC.id_MachineProdOpening = MPO.id
Inner Join MachineProdOpeningDetail OPD
On OPD.id_MachineProdOpening = MPO.id
INNER JOIN MachineForProd MFP
ON MFP.id = LCOC.id_MachineForProd
INNER JOIN Turn T
on T.id = MPO.id_Turn
INNER JOIN ProcessType PT
on PT.id = LCOC.idProccesType
INNER JOIN Document DLOC
on DLOC.id = LCOC.id
iNNER jOIN EmissionPoint emi on emi.id= DLOC.id_emissionPoint
inner join Company co on emi.id_company=co.id
INNER JOIN DocumentState DSDLOC
on DSDLOC.id = DLOC.id_documentState
 And DSDLOC.code <> '05'
INNER JOIN Document DMPO
on DMPO.id = MPO.id
INNER JOIN ProductionLot PL
on LCOC.id_ProductionLot = PL.id
INNER JOIN ProductionLotState PLS
On PLS.id = PL.id_ProductionLotState
INNER JOIN Person PPLProvider
on PPLProvider.id = PL.id_provider
INNER JOIN #TotalProceso TempTp
on TempTp.id = LCOC.id
Inner Join #Horas TmpH On TmpH.idLote = Pl.id
WHERE Convert(varchar(10),DLOC.emissionDate,103) = @Fecha
AND t.ID = @id_Turno
order by pl.sequentialLiquidation


SELECT
	Turno,
	Lote,
	Proveedor,
	numeroLoquidacion,
	LiquidacionTurno,
	sum(Cola) Cola,
	sum(Entero) Entero,
	sum(Total) Total,
	Observacion,
	HoraInicio,
	HoraFin,
	Fecha,
	Horas,
	librasEnviadas,
	@estado estado,
	rendimiento,-- = case when EstadoLote in ('06','07','08') then rendimiento else 0 end ,
	proces,
	EstadoLote
	,(select convert(varchar(5),dateadd (minute,Horas,''),114)) 'totalHoras'
FROM #Data
group by Lote,Turno,Proveedor,numeroLoquidacion,LiquidacionTurno,Observacion,HoraInicio,HoraFin,Fecha,
	Horas,librasEnviadas,rendimiento,proces,EstadoLote
order by  numeroLoquidacion

/*
	EXEC spRptCierreTurno @id=874762
	exec spRptCierreTurno @id=874762
*/
GO


