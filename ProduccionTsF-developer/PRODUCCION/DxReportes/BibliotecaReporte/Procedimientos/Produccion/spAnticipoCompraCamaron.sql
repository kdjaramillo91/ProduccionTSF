GO
/****** Object:  StoredProcedure [dbo].[par_Anticipo_Proveedor_Lista]    Script Date: 14/02/2023 10:06:45 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


Create procedure [dbo].[spAnticipoCompraCamaron]
@id_provider int = 0,
@dt_start varchar(10) = '',
@dt_startTime varchar(5) = '',
@dt_end varchar(10) = '',
@dt_endTime varchar(5) = '',
@id_state int = 0
as
set nocount on

declare @dt_startWtime datetime
declare @dt_endWtime datetime

declare @dt_yearB CHAR(4)
declare @dt_yearE CHAR(4)
declare @dt_mesB CHAR(2)
declare @dt_mesE CHAR(2)
declare @dt_DayB CHAR(2)
declare @dt_DayE CHAR(2)

if (@dt_start <> '')
begin
	set @dt_yearB = SUBSTRING(@dt_start,1,4)
	set @dt_mesB = SUBSTRING(@dt_start,6,2)
	set @dt_DayB = SUBSTRING(@dt_start,9,2)
end

if (@dt_end <> '')
begin
	set @dt_yearE = SUBSTRING(@dt_end,1,4)
	set @dt_mesE = SUBSTRING(@dt_end,6,2)
	set @dt_DayE = SUBSTRING(@dt_end,9,2)
end

set @id_provider = isnull(@id_provider,0)
set @dt_startTime = isnull(@dt_startTime,'00:00')
set @dt_endTime = isnull(@dt_endTime,'00:00')



set @dt_startWtime = convert(datetime,isnull(@dt_yearB + '-' + @dt_DayB+ '-' + @dt_mesB + ' ' + @dt_startTime,'1900-01-01 '))
set @dt_endWtime = convert(datetime,isnull(@dt_yearE + '-' + @dt_DayE + '-' + @dt_mesE + ' ' + @dt_endTime,'1900-01-01 '))
set @id_state = isnull(@id_state,0)

declare @sqlCom varchar(5000)


select
convert(varchar(250),comp.[trademark]) as [NombreCompania]
, comp.[ruc] as [RucCompania]
, comp.[logo2] as [LogoCompania]
, comp.[phoneNumber] as [NumeroCompania]
, comp.[email] as [MailCompania]
, plot.[receptionDate] as [FechaRecepcion]
, comp.[address] as [DireccionCompania]
, b.[emissionDate] as [FechaEmision]
, b.[number] as [Numero]
, dopl.[number] as [SecTransaccional]
, plot.[internalNumber] as [LoteInterno]
, a.[QuantityPoundReceived] as [LibrasRecibidas]
, a.[valueAverage] as [PrecioPromedio]
, a.[valueAdvanceTotalRounded] as [ValorAnticipo]
, dost.[name] as [EstadoAnticipo]
, prov.[fullname_businessName] as [NombreProveedor]
, RTRIM(LTRIM(dopl.[sequential])) as [Secuencia]
, Convert(varchar(10),Proceso.[processPlant]) ProcesoPlanta
from [dbo].[AdvanceProvider] a join [dbo].[Document] b on a.[id] = b.[id]
inner join [dbo].[DocumentState] dost on b.[id_documentState] = dost.[id]
inner join [dbo].[ProductionLot] plot on a.[id_Lot] = plot.[id]
inner join [dbo].[Document] dopl on plot.[id] = dopl.[id]
inner join [dbo].[EmissionPoint] empo on b.[id_emissionPoint] = empo.[id]
inner join [dbo].[Company] comp on empo.[id_company] = comp.[id]
inner join [dbo].[Person] prov on a.[id_provider] = prov.[id]
inner join [dbo].[Person] Proceso On plot.[id_personProcessPlant] = Proceso.id
where 1 = 1
and a.[id_provider] = case when @id_provider = 0 then a.[id_provider] else @id_provider end
and b.[emissionDate] >= case when year(@dt_start) = 1900 then b.[emissionDate] else @dt_startWtime end
and b.[emissionDate] <= case when year(@dt_end) = 1900 then b.[emissionDate] else @dt_endWtime end
and b.[id_documentState] = case when @id_state = 0 then b.[id_documentState] else @id_state end

order by plot.[internalNumber] asc


--[par_Anticipo_Proveedor_Lista] @id_provider=0, @dt_start='2018-06-07', @dt_startTime='00:00', @dt_end='2018-06-08',@dt_endTime='00:00', @id_state = null



