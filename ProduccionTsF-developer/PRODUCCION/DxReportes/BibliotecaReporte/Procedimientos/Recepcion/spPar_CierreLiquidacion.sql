GO
/****** Object:  StoredProcedure [dbo].[par_CierreLiquidacion]    Script Date: 24/02/2023 05:34:42 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--case when b.id_processtype = 1 then (a.quantityPoundsIL/b.wholeSubtotal)*100 else (a.quantityPoundsIL/b.subtotalTail)*100 end
create procedure [dbo].[spPar_CierreLiquidacion]
	@id int
	, @codeProcessType varchar(5)
as
set nocount on 

declare @NumberRemissionGuide varchar(8000)
declare @ParametroCarroxCarro int
declare @DatosLiquidacionCount int

Select @ParametroCarroxCarro = (Select value from Setting where code='HLCXC')
Select @DatosLiquidacionCount = (Select count(*) from ProductionLotLiquidationTotal where id_productionLot = @id)

declare @DatosLiquidacion Table(
	id_productionLot int,
	id_item int,
	quantity Decimal(14,6),
	quantityPoundsLiquidation Decimal(18,6)
)
If (@ParametroCarroxCarro = 1 and @DatosLiquidacionCount > 1)
Begin 
	Insert Into @DatosLiquidacion
	Select id_productionLot,id_ItemLiquidation, quatityBoxesIL,quantityKgsIL from ProductionLotLiquidationTotal where id_productionLot = @id
End
Else
Begin
	Insert Into @DatosLiquidacion
	Select id_productionLot,id_item, quantity,quantityPoundsLiquidation from ProductionLotLiquidation where id_productionLot = @id
End

select @NumberRemissionGuide = COALESCE(@NumberRemissionGuide + ', ', '') + RTRIM(LTRIM(f.[sequential]))
from [dbo].[ProductionLotDetailPurchaseDetail] a 
join [dbo].[RemissionGuideDetail] b on a.[id_remissionGuideDetail] = b.[id]
join [dbo].[ProductionLotDetail] c on a.[id_productionLotDetail] = c.[id]
join [dbo].[RemissionGuide] d on b.[id_remisionGuide] = d.[id]
join [dbo].[ProductionLot] e on c.[id_productionLot] = e.[id]
join [dbo].[Document] f on d.[id] = f.[id]
where e.[id] = @id
-- [dbo].[par_CierreLiquidacion] 162608,'COL'
declare @Datos Table(
	id_productionLot int,
	nombreMaquina varchar(100),
	nombreTurno varchar(100),
	fechaEmission datetime,
	horaInicio  time,
	horaFin  time,
	librasProcesadas decimal,
	tipoProceso VARCHAR(100)
)
INSERT Into @Datos
exec [par_LiquidacionCarroXCarro_Lote] @id,@codeProcessType

declare @fechaProceso Datetime
declare @horaInicio Time
declare @horaFin Time
if(select count(*) from @Datos) = 1
begin
	set @fechaProceso = (select top(1) fechaEmission from @Datos order by nombreMaquina)
	set @horaInicio = (select top(1) horaInicio from @Datos order by nombreMaquina)
	set @horaFin = (select top(1) horaFin from @Datos order by nombreMaquina)
end
if(select count(*) from @Datos) > 1
begin
set @fechaProceso = (select top(1) fechaEmission from @Datos order by nombreMaquina)
	set @horaInicio = (select top(1) Min(horaInicio) from @Datos)
	set @horaFin = (select top(1) Max(horaFin) from @Datos)
end
select 
@codeProcessType as codetype,
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
	, case when @codeProcessType = 'ENT' then (a.[quantity] * g.[minimum]) Else Isnull(a.[quantityPoundsLiquidation],0) End as [CantidadMinimaXcajas]

,case when h.id_processtype = 1 then 
		case when l.id = 1 then case when h.wholeSubtotal > 0 then
		 round(((temp.KilosTmp  *2.2046)/h.wholeSubtotal)*100,2) else 0 end else case when h.subtotalTail > 0 then
		 round(((temp.KilosTmp  *2.2046)/h.subtotalTail)*100,2) else 0 end end else ---------------
		 case when h.id_processtype = 2 and h.subtotalTail > 0 then round(((temp.KilosTmp  *2.2046)/h.subtotalTail)*100,2) else
		case when h.wholeSubtotal > 0 then round(((temp.KilosTmp  *2.2046)/h.wholeSubtotal)*100,2) else 0 end
		 	 end end as Rendimiento 
--,case when h.id_processtype = 1 then 	
--case when l.id = 4 and h.subtotalTail > 0 then 
--	round(((temp.KilosTmp)/h.subtotalTail)*100,2) ELSE 
--	-case when h.wholeSubtotal > 0 then
--	round(((temp.KilosTmp /2.2046)/h.wholeSubtotal)*100,2) else 0 end end else
--	case when h.id_processtype = 2 then round(((temp.KilosTmp * 2.2046)/h.subtotalTail)*100,2)  else
--		round(((temp.KilosTmp  *2.2046)/h.subtotalTail)*100,2)
--	end end as Rendimiento2-------
	, convert(varchar(200), k.[fullname_businessName]) as [NombreProveedor]
	, convert(varchar(100), i.[name]) as [NombrePiscina]
	, isnull(h.[sequentialLiquidation],0) as [SecuenciaLiquidacion]
	, case when @codeProcessType = 'ENT' then h.[wholeSubtotal] else h.[subtotalTail] end as [LibrasProcesadas]
	, case when @codeProcessType = 'ENT' then h.[wholeGarbagePounds] else h.[poundsGarbageTail] end as [LibrasBasuras]
	, case when @codeProcessType = 'ENT' then Isnull(h.wholesubtotal,0)+Isnull(h.wholeleftover,0)+Isnull(h.wholeGarbagePounds,0) 
	  else Case when isnull(h.[tailLeftover],0) = 0 Then h.[wholeLeftover] 
		   Else isnull(h.[tailLeftover],0) End end as [LibrasRecibidas]
	, case when @codeProcessType = 'ENT' then Isnull(h.wholesubtotal,0)+Isnull(h.wholeleftover,0) --isnull(h.[wholeSubtotalAdjust],0) + isnull(h.[wholeLeftover],0) 
	  else Case when isnull(h.[tailLeftover],0) = 0 Then isnull(h.[wholeLeftover], 0) - isnull(h.[poundsGarbageTail] , 0) 
	            Else isnull(h.[tailLeftover],0) - isnull(h.[poundsGarbageTail],0) End end as [LibrasNetas]

	, case when @codeProcessType = 'ENT' then 
	       case when  isnull(h.[wholeSubtotalAdjust], 0) + isnull(h.[wholeLeftover], 0) > 0
	            then isnull(h.[wholeSubtotalAdjust],0)/(isnull(h.[wholeSubtotalAdjust], 0) + isnull(h.[wholeLeftover], 0)) else 0 end
	       else 
		   Case when isnull(h.[tailLeftover],0) = 0 Then		   
			    case when (isnull(h.[wholeLeftover], 0) - isnull(h.[poundsGarbageTail], 0)) > 0
					 then h.[subtotalTailAdjust]/(isnull(h.[wholeLeftover], 0) - isnull(h.[poundsGarbageTail], 0)) else 0 end 
				Else 
				case when (isnull(h.[tailLeftover], 0) - isnull(h.[poundsGarbageTail], 0)) > 0
					 then h.[subtotalTailAdjust]/(isnull(h.[tailLeftover], 0) - isnull(h.[poundsGarbageTail], 0)) else 0 end End
	 end * 100 as [PorcentajeRendimiento]
,[PorcentajeRendimiento2] =case when @codeProcessType = 'col' then 
case when h.tailleftover <> 0 then  isnull((h.subtotaltail/h.tailleftover)*100,0) else h.wholeSubtotal/(h.wholeSubtotalAdjust + h.wholeLeftover)*100 end end
,Case when (h.wholeSubtotal+h.wholeLeftover) > 0 Then round((h.wholeSubtotal/(h.wholeSubtotal+h.wholeLeftover))*100,2) else 0 End as RENDSR1
,case when @codeProcessType = 'col' then case when h.tailleftover <> 0 then 
 ( h.[subtotalTailAdjust]/(isnull(h.[tailLeftover], 0) - isnull(h.[poundsGarbageTail], 0))) *100 else
   h.[subtotalTailAdjust]/(isnull(h.[wholeLeftover], 0) - isnull(h.[poundsGarbageTail], 0)) *100 END end AS RENDSR2
 
	, h.[totalQuantityRecived] as [LibrasDespachadas]
	, case when @codeProcessType = 'ENT' then h.[wholeLeftover] else Case when isnull(h.[tailLeftover],0) = 0 Then 
	isnull(h.[wholeLeftover],0) Else 0 End End as [LibrasRechazo]
	, h.[id_company] as [IdCompany]
	, j.[name] as [Camaronera]
	, Isnull(m.[name],'') as [Estado]
	,@fechaProceso 'fechaProceso'
	,Convert(varchar(5),@horaInicio) 'horaInicio'
	,Convert(varchar(5),@horaFin) 'HoraFin'
	,g.id_metricUnit metricunit

	,isnull(u.username,'') 'UsuarioModifica'
from @DatosLiquidacion a
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
join [dbo].[ProductionLotState] m on h.[id_ProductionLotState] = m.[id]
left join [dbo].[User] u On u.id = h.id_userUpdate
inner join ProductionLotLiquidationTotal prl on prl.id_productionLot = h.id
and b.id = prl.id_ItemLiquidation
inner join   
(select d.id id_talla, sum(quantityKgsIL)KilosTmp 
from ProductionLotLiquidationTotal a
inner join item b on a.id_ItemLiquidation = b.id
inner join ItemGeneral c on c.id_item = b.id
inner join ItemSize d on d.id = c.id_size
where a.id_productionLot = @id 
group by d.id) temp On temp.id_talla = f.id
where a.[id_productionLot] = @id
and d.[code] = @codeProcessType


order by f.[orderSize]



