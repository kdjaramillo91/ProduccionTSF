if OBJECT_ID(N'[dbo].[par_CierreLiquidacion]') IS NOT NULL
	DROP PROCEDURE [dbo].[par_CierreLiquidacion]
GO	 
create procedure [dbo].[par_CierreLiquidacion]
	@id int
	, @codeProcessType varchar(5)
as
set nocount on 

declare @NumberRemissionGuide varchar(8000)

select @NumberRemissionGuide = COALESCE(@NumberRemissionGuide + ', ', '') + RTRIM(LTRIM(f.[sequential]))
from [dbo].[ProductionLotDetailPurchaseDetail] a 
join [dbo].[RemissionGuideDetail] b on a.[id_remissionGuideDetail] = b.[id]
join [dbo].[ProductionLotDetail] c on a.[id_productionLotDetail] = c.[id]
join [dbo].[RemissionGuide] d on b.[id_remisionGuide] = d.[id]
join [dbo].[ProductionLot] e on c.[id_productionLot] = e.[id]
join [dbo].[Document] f on d.[id] = f.[id]
where e.[id] = @id

-- [dbo].[par_CierreLiquidacion] 162608,'COL'

select 
	h.[id] as [IdProduccion]
	, h.[receptionDate] as [FechaHoraRecepcion]
	, h.[internalNumber] as [NumeroLote]
	, d.[code] as [CodeProcessType] 
	, d.[name] as [NameProcessType]
	, convert(varchar(150), b.[name]) as [NombreProducto]
	, f.[name] as [TallaProducto]
	, a.[quantity] as [CantidadCajas]
	, g.[code] as [CodePresentation]
	, g.[name] as [NombrePresentacion]
	, g.[minimum] as [MinimaPresentacion]
	, isnull(@NumberRemissionGuide, '') as [GuiaRemision]
	, (a.[quantity] * g.[minimum]) as [CantidadMinimaXcajas] 
	, convert(varchar(200), k.[fullname_businessName]) as [NombreProveedor]
	, convert(varchar(100), i.[name]) as [NombrePiscina]
	, h.[sequentialLiquidation] as [SecuenciaLiquidacion]
	, case when @codeProcessType = 'ENT' then h.[wholeSubtotal] else h.[subtotalTail] end as [LibrasProcesadas]
	, case when @codeProcessType = 'ENT' then h.[wholeGarbagePounds] else h.[poundsGarbageTail] end as [LibrasBasuras]
	, case when @codeProcessType = 'ENT' then h.[wholeLeftover] else h.[tailLeftOver] end as [LibrasRecibidas]
	, case when @codeProcessType = 'ENT' then h.[wholeSubtotalAdjust] + h.[wholeLeftover] - h.[wholeGarbagePounds]
	else isnull(h.[tailLeftOver], 0) - isnull(h.[poundsGarbageTail], 0) end as [LibrasNetas]
	, case when @codeProcessType = 'ENT' then 
	case when  isnull(h.[wholeSubtotalAdjust], 0) + isnull(h.[wholeLeftover], 0) - isnull(h.[wholeGarbagePounds], 0) > 0
	then h.[wholeSubtotalAdjust]/(isnull(h.[wholeSubtotalAdjust], 0) + isnull(h.[wholeLeftover], 0) - isnull(h.[wholeGarbagePounds], 0)) else 0 end
	else 
	case when (isnull(h.[tailLeftOver], 0) - isnull(h.[poundsGarbageTail], 0)) > 0
	then h.[subtotalTailAdjust]/(isnull(h.[tailLeftOver], 0) - isnull(h.[poundsGarbageTail], 0)) else 0 end 
	end * 100 as [PorcentajeRendimiento]
	, h.[totalQuantityRecived] as [LibrasDespachadas]
	, case when @codeProcessType = 'ENT' then h.[wholeLeftover] else h.[tailLeftOver] end as [LibrasRechazo]
	, h.[id_company] as [IdCompany]
from [dbo].[ProductionLotLiquidation] a
join [dbo].[ProductionLot] h on a.[id_productionLot] = h.[id]
join [dbo].[Item] b on a.[id_item] = b.[id]
join [dbo].[ItemTypeItemProcessType] c on b.[id_itemType] = c.[idItemType]
join [dbo].[ProcessType] d on c.[idProcessType] = d.[id]
join [dbo].[ItemGeneral] e on b.[id] = e.[id_item]
join [dbo].[ItemSize] f on e.[id_size] = f.[id]
join [dbo].[Presentation] g on g.[id] = b.[id_presentation]
join [dbo].[MetricUnit] l on g.[id_metricUnit] = l.[id]
join [dbo].[ProductionUnitProvider] j on h.[id_productionUnitProvider] = j.[id]
join [dbo].[ProductionUnitProviderPool] i on h.[id_productionUnitProviderPool] = i.[id]
join [dbo].[Person] k on h.[id_provider] = k.[id]
where a.[id_productionLot] = @id
and d.[code] = @codeProcessType
order by f.[orderSize]
-- [dbo].[par_CierreLiquidacion] 162608,'COL'