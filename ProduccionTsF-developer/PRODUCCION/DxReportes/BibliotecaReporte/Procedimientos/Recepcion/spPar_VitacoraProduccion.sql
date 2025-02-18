/****** Object:  StoredProcedure [dbo].[spPar_VitacoraProduccion]    Script Date: 14/04/2023 14:15:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROC [dbo].[spPar_VitacoraProduccion]
	--@dt_start varchar(10) = '',
	--@dt_startTime varchar(5) = '',
	--@dt_end varchar(10) = ''
	--@dt_endTime varchar(5) = ''
	@dt_start varchar(10) = '',
@dt_end varchar(10) = ''

as
set nocount on

if @dt_start = '' set @dt_start = null
if @dt_end = '' set @dt_end = null

declare @fiDt date
declare @ffDt date


set @fiDt = convert(date,isnull(@dt_start,'1900-01-01'))
set @ffDt = convert(date,isnull(@dt_end,'1900-01-01'))




--declare @dt_startWtime datetime
--declare @dt_endWtime datetime

--declare @dt_yearB CHAR(4)
--declare @dt_yearE CHAR(4)
--declare @dt_mesB CHAR(2)
--declare @dt_mesE CHAR(2)
--declare @dt_DayB CHAR(2)
--declare @dt_DayE CHAR(2)

--if (@dt_start <> '')
--begin
--	set @dt_yearB = SUBSTRING(@dt_start,1,4)
--	set @dt_mesB = SUBSTRING(@dt_start,6,2)
--	set @dt_DayB = SUBSTRING(@dt_start,9,2)
--end

--if (@dt_end <> '')
--begin
--	set @dt_yearE = SUBSTRING(@dt_end,1,4)
--	set @dt_mesE = SUBSTRING(@dt_end,6,2)
--	set @dt_DayE = SUBSTRING(@dt_end,9,2)
--end

----set @dt_startTime = isnull(@dt_startTime,'00:00')
----set @dt_endTime = isnull(@dt_endTime,'00:00')

----set @dt_startWtime = convert(datetime,isnull(@dt_yearB + '-' + @dt_DayB+ '-' + @dt_mesB + ' ' + @dt_startTime,'1900-01-01 '))
----set @dt_endWtime = convert(datetime,isnull(@dt_yearE + '-' + @dt_DayE + '-' + @dt_mesE + ' ' + @dt_endTime,'1900-01-01 '))
--set @dt_startWtime = convert(datetime,isnull(@dt_yearB + '-' + @dt_DayB+ '-' + @dt_mesB,''))
--set @dt_endWtime = convert(datetime,isnull(@dt_yearE + '-' + @dt_DayE + '-' + @dt_mesE,''))


select 
	pl.[internalNumber] as [NumeroLote]
	, pl.[receptionDate] as [FechaRecepcion]
	, convert(varchar(200), pro.[fullname_businessName]) as [NombreProveedor]
	, pupp.[name] as [NombrePiscina]
	, isnull(pld.[drawersNumber],0) as [NumeroGavetas]
	, fs.[name] as [NombreSitio]
	, rgt.[driverName] as [NombreConductor]
	, pl.[internalNumber] as [NumeroInterno]
	, convert(varchar(50), do.[sequential]) as [NumeroGuia]
	, (select top 1 rgss.[number]
      from [dbo].[RemissionGuideSecuritySeal] rgss 
	  join [dbo].[RemissionGuideTravelType] rgtt on rgss.[id_travelType] = rgtt.[id] 
	  and RTRIM(LTRIM(rgtt.[code])) = 'REGRESO'
	  and rgss.[id_remissionGuide] = rg.[id]) as [SelloRetorno]
	, pld.[quantityRecived]
	,CASE WHEN CONVERT(VARCHAR,year(@fiDt)) = '1900' then 'TODOS' ELSE CONVERT(VARCHAR,@fiDt,103) END
	 as FechaRecepcionInicio

	, CASE WHEN CONVERT(VARCHAR,year(@ffDt)) = '1900' then 'TODOS' ELSE CONVERT(VARCHAR,@ffDt,103) END
	 as FechaRecepcionFin
	, CONVERT(DATETIME, rgcv.entranceDateProductionBuilding) + CONVERT(DATETIME, rgcv.entranceTimeProductionBuilding) as [FechaEntrada]
	, Convert(varchar(250),proPlan.[processPlant]) as ProcesoPlanta
	,con.address as DireccionCia

	
from [dbo].[ProductionLotDetail] pld
join [dbo].[ProductionLotDetailPurchaseDetail] pldpd on pld.[id] = pldpd.[id_productionLotDetail]
join [dbo].[ProductionLot] pl on pld.[id_productionLot] = pl.[id]
join [dbo].[ProductionLotState] pls on pl.[id_ProductionLotState] = pls.[id] and pls.[code] not in ('09')
join [dbo].[RemissionGuideDetail] rgd on rgd.[id] = pldpd.[id_remissionguidedetail]
join [dbo].[RemissionGuide] rg on rgd.[id_remisionGuide] = rg.[id]
join [dbo].[Document] do on rg.[id] = do.[id]
join EmissionPoint emi on do.id_emissionPoint=emi.id
join Company con on con.id = emi.id_company
join [dbo].[RemissionGuideTransportation] rgt on rg.[id] = rgt.[id_remionGuide]
join [dbo].[RemissionGuideControlVehicle] rgcv on rg.[id] = rgcv.[id_remissionGuide]
join [dbo].[Person] pro on pl.[id_provider] = pro.[id]
join [dbo].[ProductionUnitProviderPool] pupp on pl.[id_productionUnitProviderPool] = pupp.[id]
join [dbo].[ProductionUnitProvider] pup on pl.[id_productionUnitProvider] = pup.[id]
join [dbo].[FishingSite] fs on case when rgt.[isOwn] = 1 then pup.[id_FishingSite] else rgt.[id_FishingSiteRG] end = fs.[id]
join [dbo].[Person] proPlan on pl.[id_personProcessPlant] = proPlan.[id]
where 1=1 and
convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.receptionDate) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date, pl.receptionDate) else @ffDt end
order by CONVERT(DATETIME, rgcv.entranceDateProductionBuilding) + CONVERT(DATETIME, rgcv.entranceTimeProductionBuilding) desc

/*
	EXEC spPar_VitacoraProduccion @dt_start=N'2022/02/01',@dt_end=N'2022/02/28'
*/

GO