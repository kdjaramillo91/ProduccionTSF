SET NOCOUNT ON
GO

CREATE OR ALTER procedure [dbo].[spPar_LiquidacionCarroXCarroLote]
	@idProductionLot integer,
	@codeProcessType varchar(5)
as
set nocount on

select 
	a.[id] as [IdLiquidacionCarro]
	, f.[name] as [NombreMaquina]
	, h.[name] as [NombreTurno]
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
join [dbo].[MachineProdOpening] g on a.[id_MachineProdOpening] = g.[id]
join [dbo].[Turn] h on g.[id_turn] = h.[id]
where e.[code] = @codeProcessType
Order by f.[name]
--[dbo].[par_LiquidacionCarroXCarro_Lote] 232614, 'ENT'
--select * from LiquidationCartOnCartDetail