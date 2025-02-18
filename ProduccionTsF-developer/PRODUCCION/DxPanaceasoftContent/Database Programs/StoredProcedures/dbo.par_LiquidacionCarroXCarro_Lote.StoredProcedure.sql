if OBJECT_ID(N'[dbo].[par_LiquidacionCarroXCarro_Lote]') IS NOT NULL
	DROP PROCEDURE [dbo].[par_LiquidacionCarroXCarro_Lote]
GO	 
create procedure [dbo].[par_LiquidacionCarroXCarro_Lote]
	@idProductionLot integer
as
set nocount on


select 
	a.[id] as [IdLiquidacionCarro]
	, f.[name] as [NombreMaquina]
	, b.[emissionDate] as [FechaEmision]
	, a.[timeInit] as [HoraInicio]
	, a.[timeEnd] as [HoraFin]
	, d.[PoundsProcess] as [LibrasProcesadas]
	, e.[name] as [TipoProceso]
from [dbo].[LiquidationCartOnCart] a
join [dbo].[Document] b on a.[id] = b.[id]
join [dbo].[DocumentState] c on b.[id_documentState] = c.[id] and c.[code] not in('05')
join 
(
	select 
		b.[id] as [IdLiquidationCartOnCart]
		, SUM(a.[quantityPoundsIL]) as [PoundsProcess]
	from [dbo].[LiquidationCartOnCartDetail] a
	join [dbo].[LiquidationCartOnCart] b on a.[id_LiquidationCartOnCart] = b.[id]
	join [dbo].[Document] c on b.[id] = c.[id] 
	join [dbo].[DocumentState] d on c.[id_documentState] = d.[id] and d.[code] not in('05')
	where b.[id_ProductionLot] = @idProductionLot
	group by b.[id]
) d on a.[id] = d.[IdLiquidationCartOnCart]
join [dbo].[ProcessType] e on a.[idProccesType] = e.[id]
join [dbo].[MachineForProd] f on a.[id_MachineForProd] = f.[id]

--[dbo].[par_LiquidacionCarroXCarro_Lote] 171638
--select * from LiquidationCartOnCartDetail
