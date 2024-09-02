
GO
/****** Object:  StoredProcedure [dbo].[RptCierreTurnoTemporal]    Script Date: 05/01/2023 8:49:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[spRptCierreTurnoTemporal]
	@id INT,
	@Fecha VARCHAR(25)
	--@idTurno INT
AS
--DECLARE @Fecha VARCHAR(15)
--DECLARE @HoraFinal VARCHAR(5)
--DECLARE @id_Turno INT
--DECLARE @numeroLiquidacion VARCHAR(250)
--DECLARE @estado VARCHAR(250)

Set NoCount On

CREATE TABLE #TotalProceso(
	id INT,
	Cola NUMERIC(13,4),
	Entero NUMERIC(13,4),
	Total NUMERIC(13,4)
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


select distinct
	T.name Turno,
	Pl.internalNumber Lote,
	PT.name Proceso,
	PPLProvider.fullname_businessName Proveedor,
	pl.sequentialLiquidation numeroLoquidacion,
	TempTp.cola Cola,
	TempTp.entero Entero,
	TempTp.Total Total,
	CONVERT(VARCHAR(5),T.timeInit) HoraInicio,
	Convert(varchar(10),DLOC.emissionDate,103) Fecha,
	PL.totalQuantityRecived librasEnviadas,
	pt.code,
	itp.masterCode CodigoProducto,
	convert(varchar(250),itp.[name]) as NombreProducto,
	CONVERT(VARCHAR(50),itsp.[name]) as [NombreTallaProductoPrimario],
	Round(sum(LCOD.[quatityBoxesIL]),2) as [Cajas],
	Round(sum(LCOD.[quantityPoundsIL]),2) as [Libras],
	convert(varchar(250),pers.[fullname_businessName]) as [NombreClientePrimario],
	--'CAJAS' as Tipo,
	CONVERT(VARCHAR(10),@Fecha,103) as FechaLiquidacion,
	'PENDIENTE' as Estado
from LiquidationCartOnCart LCOC
JOIN LiquidationCartOnCartDetail LCOD on LCOD.id_LiquidationCartOnCart = LCOC.id
JOIN [dbo].[Item] itp on LCOD.[id_ItemLiquidation] = itp.[id]
JOIN [dbo].[ItemGeneral] itgp on itp.[id] = itgp.[id_item] 
JOIN [dbo].[ItemSize] itsp on itgp.[id_size] = itsp.[id]
JOIN [dbo].[ItemSizeProcessType] ispt on itsp.[id] = ispt.[id_itemSize] 
JOIN [dbo].[Person] pers on pers.[id] = LCOD.[id_Client] 
JOIN MachineProdOpening MPO on LCOC.id_MachineProdOpening = MPO.id
JOIN MachineForProd MFP ON MFP.id = LCOC.id_MachineForProd
JOIN Turn T on T.id = MPO.id_Turn
JOIN ProcessType PT on PT.id = LCOC.idProccesType
JOIN Document DLOC on DLOC.id = LCOC.id
JOIN DocumentState DSDLOC on DSDLOC.id = DLOC.id_documentState
 And DSDLOC.code <> '05'
JOIN Document DMPO on DMPO.id = MPO.id
JOIN ProductionLot PL on LCOC.id_ProductionLot = PL.id
JOIN Person PPLProvider on PPLProvider.id = PL.id_provider
JOIN #TotalProceso TempTp on TempTp.id = LCOC.id
WHERE Convert(varchar(10),DLOC.emissionDate,103) = @Fecha-- '11/08/2020'
group by 
T.name,
	Pl.internalNumber,
	PT.name,
	PPLProvider.fullname_businessName,
	pl.sequentialLiquidation,
	TempTp.cola,
	TempTp.entero,
	TempTp.Total ,
	CONVERT(VARCHAR(5),T.timeInit) ,
	Convert(varchar(10),DLOC.emissionDate,103) ,
	PL.totalQuantityRecived ,
	pt.code,
	itp.mastercode,
	convert(varchar(250),itp.[name]),
	CONVERT(VARCHAR(50),itsp.[name]),
	convert(varchar(250),pers.[fullname_businessName])
--ORDER BY PT.name, CONVERT(VARCHAR(50),itsp.[name])
--UNION ALL

--select distinct
--	T.name Turno,
--	Pl.internalNumber Lote,
--	PT.name Proceso,
--	PPLProvider.fullname_businessName Proveedor,
--	pl.sequentialLiquidation numeroLoquidacion,
--	TempTp.cola Cola,
--	TempTp.entero Entero,
--	TempTp.Total Total,
--	CONVERT(VARCHAR(5),T.timeInit) HoraInicio,
--	Convert(varchar(10),DLOC.emissionDate,103) Fecha,
--	PL.totalQuantityRecived librasEnviadas,
--	pt.code,
--	itp.masterCode CodigoProducto,
--	convert(varchar(250),itp.[name]) as NombreProducto,
--	CONVERT(VARCHAR(50),itsp.[name]) as [NombreTallaProductoPrimario],
--	--Round(sum(LCOD.[quatityBoxesIL]),2) as [Valor],
--	Round(sum(LCOD.[quantityPoundsIL]),2) as [Valor],
--	convert(varchar(250),pers.[fullname_businessName]) as [NombreClientePrimario],
--	'LIBRAS' as Tipo,
--	CONVERT(VARCHAR(10),@Fecha,103) as FechaLiquidacion,
--	'PENDIENTE' as Estado
--from LiquidationCartOnCart LCOC
--JOIN LiquidationCartOnCartDetail LCOD on LCOD.id_LiquidationCartOnCart = LCOC.id
--JOIN [dbo].[Item] itp on LCOD.[id_ItemLiquidation] = itp.[id]
--JOIN [dbo].[ItemGeneral] itgp on itp.[id] = itgp.[id_item] 
--JOIN [dbo].[ItemSize] itsp on itgp.[id_size] = itsp.[id]
--JOIN [dbo].[ItemSizeProcessType] ispt on itsp.[id] = ispt.[id_itemSize] 
--JOIN [dbo].[Person] pers on pers.[id] = LCOD.[id_Client] 
--JOIN MachineProdOpening MPO on LCOC.id_MachineProdOpening = MPO.id
--JOIN MachineForProd MFP ON MFP.id = LCOC.id_MachineForProd
--JOIN Turn T on T.id = MPO.id_Turn
--JOIN ProcessType PT on PT.id = LCOC.idProccesType
--JOIN Document DLOC on DLOC.id = LCOC.id
--JOIN DocumentState DSDLOC on DSDLOC.id = DLOC.id_documentState
-- And DSDLOC.code <> '05'
--JOIN Document DMPO on DMPO.id = MPO.id
--JOIN ProductionLot PL on LCOC.id_ProductionLot = PL.id
--JOIN Person PPLProvider on PPLProvider.id = PL.id_provider
--JOIN #TotalProceso TempTp on TempTp.id = LCOC.id
--WHERE Convert(varchar(10),DLOC.emissionDate,103) = @Fecha
--group by 
--T.name,
--	Pl.internalNumber,
--	PT.name,
--	PPLProvider.fullname_businessName,
--	pl.sequentialLiquidation,
--	TempTp.cola,
--	TempTp.entero,
--	TempTp.Total ,
--	CONVERT(VARCHAR(5),T.timeInit) ,
--	Convert(varchar(10),DLOC.emissionDate,103) ,
--	PL.totalQuantityRecived ,
--	pt.code,
--	itp.mastercode,
--	convert(varchar(250),itp.[name]),
--	CONVERT(VARCHAR(50),itsp.[name]),
--	convert(varchar(250),pers.[fullname_businessName])
--ORDER BY PT.name, CONVERT(VARCHAR(50),itsp.[name])

--exec [RptCierreTurnoTemporal] 0,'11/08/2020',0
