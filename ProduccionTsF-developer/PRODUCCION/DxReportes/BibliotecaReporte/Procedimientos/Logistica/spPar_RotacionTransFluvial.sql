
GO
/****** Object:  StoredProcedure [dbo].[par_LiquidacionRotacionTransportistaFluvial] '','',0    Script Date: 07/01/2023 10:30:03 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[spPar_RotacionTransFluvial]
	@dt_start	varchar(10) = '',
	@dt_end		varchar(10) = '',
	@idCompany	int = 0
as 
set nocount on 

declare @dtStart datetime
declare @dtEnd datetime
declare @idDocumentState int

declare @ruc varchar(50)
declare @telefono varchar(80)

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

set @dtStart = convert(datetime,isnull(@dt_yearB + '-' + @dt_DayB+ '-' + @dt_mesB,'1900-01-01 '))
set @dtEnd = convert(datetime,isnull(@dt_yearE + '-' + @dt_DayE + '-' + @dt_mesE,'1900-01-01 '))

select @ruc = [ruc] , @telefono = [phoneNumber]
from [dbo].[Company] where [id] = @idCompany

declare @tmpLiquidacion	table
(
	[idLiquidationFreightRiver]	INT NOT NULL
)

declare @Vehiculos	table
(
	[idVehiculos]	INT NOT NULL
)
declare @VehiculosLiquidacion	table
(
	[idVehiculos]	INT NOT NULL
)
declare @VehiculosSinLiquidacion	table
(
	[idVehiculos]	INT NOT NULL
)
declare @EstadosExcluidos table 
(
	[idEstados]		int not null
)

declare @ResultadosSinLiquidacion table 
(
	[idVehiculos]	INT NOT NULL,
	[idCiaFactura]	int 
)
declare @Resultados	table
(
	[NombreCiaFactura]	varchar(250),
	[NombreDueno]		varchar(250),
	[idVehiculos]		int NOT NULL,
	[Matricula]			varchar(50) null,
	[ContadorLiqidaciones]	int null,
	[TotalValor]			decimal(18,6),
	[TieneHunter]			bit,
	[IdCompany]				int,
	[Ruc]					varchar(50),
	[Telefono]				varchar(80),
	[FechaInicio]		datetime,
	[FechaFin]			datetime
)

insert into @EstadosExcluidos
select [id] 
from [dbo].[DocumentState] where [code] in('05') 

--Selecciono Todas las liquidaciones
insert into @tmpLiquidacion
select a.[id]
from [dbo].[LiquidationFreightRiver] a 
join [dbo].[Document] b on a.[id] = b.[id]
where convert(date,b.[emissionDate]) >= case when year(@dtStart) = 1900 then convert(date,b.[emissionDate]) else @dtStart end
and convert(date,b.[emissionDate]) <= case when year(@dtEnd) = 1900 then convert(date,b.[emissionDate]) else @dtEnd end
and b.[id_documentState] not in (select [idEstados] from @EstadosExcluidos)

--Selecciono Todos los vehiculos que están en liquidacion con ciertos estados
insert into @VehiculosLiquidacion
select rgt.[id_vehicle] 
from [dbo].[LiquidationFreightRiverDetail] lfd 
join @tmpLiquidacion tl on lfd.[id_LiquidationFreightRiver] = tl.[idLiquidationFreightRiver]
join [dbo].[RemissionGuideRiverTransportation] rgt on lfd.[id_remisionGuideRiver] = rgt.[id_remissionGuideRiver]
group by rgt.[id_vehicle]

--Selecciono TODOS lo Vehiculos SIN DISCRIMINAR
insert into @Vehiculos
select a.[id]
from [dbo].[Vehicle] a join [dbo].[VehicleType] b on a.[id_VehicleType] = b.[id]  and b.[name] in (
'LAN01',
'GAB01',
'BOT01')


insert into @VehiculosSinLiquidacion
select [idVehiculos] 
from @Vehiculos where [idVehiculos] not in (select * from @VehiculosLiquidacion)

insert into @ResultadosSinLiquidacion
select [idVehiculos], (select top 1 [id_provider]
from [dbo].[VehicleProviderTransportBilling] vptb
where [state] = 1 and [datefin] is null and vptb.[id_vehicle] = v.[idVehiculos]) 
from @VehiculosSinLiquidacion v


--Obtengo Resultados Parte 1
insert into @Resultados
select 
	convert(varchar(250),ciaf.[fullname_businessName]) 
	, convert(varchar(250),due.[fullname_businessName]) 
	, b.[id_vehicle]
	, vehi.[carRegistration]
	, count(*)
	, sum(a.[pricesubtotal]) 
	, vehi.[hasHunterDevice]
	, @idCompany
	, @ruc
	, @telefono
	, @dtStart
	, @dtEnd
from [dbo].[LiquidationFreightRiverDetail] a
join [dbo].[LiquidationFreightRiver] c on a.[id_LiquidationFreightRiver] = c.[id] 
join @tmpLiquidacion lq on a.[id_LiquidationFreightRiver] = lq.[idLiquidationFreightRiver] 
join [dbo].[RemissionGuideRiverTransportation] b on a.[id_remisionGuideRiver] = b.[id_remissionGuideRiver]
join [dbo].[Person] ciaf on c.[id_providertransport] = ciaf.[id]
join [dbo].[Person] due on a.[idOwnerVehicle] = due.[id]
join [dbo].[Vehicle] vehi on b.[id_vehicle] = vehi.[id]
group by ciaf.[fullname_businessName], due.[fullname_businessName], b.[id_vehicle], vehi.[carRegistration], vehi.[hasHunterDevice]
--Obtengo Resultados Parte 2 Vehiculos sin liquidacion
union all
select  
	convert(varchar(250),ciaf.[fullname_businessName]) 
	, convert(varchar(250),due.[fullname_businessName]) 
	, ve.[id]
	, ve.[carRegistration]
	, 0
	, 0
	, [hasHunterDevice] 
	, @idCompany
	, @ruc
	, @telefono
	, @dtStart
	,@dtEnd
from @ResultadosSinLiquidacion rsl 
join [dbo].[Vehicle] ve on rsl.[idVehiculos] = ve.[id]
left outer join [dbo].[Person] ciaf on rsl.[idCiaFactura] = ciaf.[id] 
left outer join [dbo].[Person] due on ve.[id_personOwner] = due.[id]

--Resultado Final
select *,(select top(1) logo from Company where id = @idCompany) logo,
	(select top(1)logo2 from Company where id = @idCompany) logo2
	from @Resultados order by ContadorLiqidaciones desc, TieneHunter desc
--[dbo].[par_LiquidacionRotacionTransportistaFluvial] '' ,'',2