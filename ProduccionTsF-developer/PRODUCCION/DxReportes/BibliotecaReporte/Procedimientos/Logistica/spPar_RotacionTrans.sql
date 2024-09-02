
GO
/****** Object:  StoredProcedure [dbo].[par_LiquidacionRotacionTransportista] '','',0   Script Date: 07/01/2023 08:48:17 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[spPar_RotacionTrans]

 @dt_start varchar(10) = '',  
 @dt_end  varchar(10) = '',  
 @idCompany int = 0  
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
 set @dt_mesB = SUBSTRING(@dt_start,9,2)  
 set @dt_DayB = SUBSTRING(@dt_start,6,2)  
end  
  
if (@dt_end <> '')  
begin  
 set @dt_yearE = SUBSTRING(@dt_end,1,4)  
 set @dt_mesE = SUBSTRING(@dt_end,9,2)  
 set @dt_DayE = SUBSTRING(@dt_end,6,2)  
end  
  
set @dtStart = convert(datetime,isnull(@dt_yearB + '-' + @dt_mesB + '-' +  @dt_DayB,'1900-01-01 '))  
set @dtEnd = convert(datetime,isnull(@dt_yearE + '-' + @dt_mesE + '-' +  @dt_DayE,'1900-01-01 '))  
  
select @ruc = [ruc] , @telefono = [phoneNumber]  
from [dbo].[Company] where [id] = @idCompany  
  
declare @tmpLiquidacion table  
(  
 [idLiquidationFreight] INT NOT NULL  
)  
  
declare @Vehiculos table  
(  
 [idVehiculos] INT NOT NULL  
)  
declare @VehiculosLiquidacion table  
(  
 [idVehiculos] INT NOT NULL  
)  
declare @VehiculosSinLiquidacion table  
(  
 [idVehiculos] INT NOT NULL  
)  
declare @EstadosExcluidos table   
(  
 [idEstados]  int not null  
)  
  
declare @ResultadosSinLiquidacion table   
(  
 [idVehiculos] INT NOT NULL,  
 [idCiaFactura] int not null  
)  
declare @Resultados table  
(  
 [idPersonFact]	int,
 [NombreCiaFactura] varchar(250),  
 [NombreDueno]  varchar(250),  
 [idVehiculos]  int NOT NULL,  
 [Matricula]   varchar(50) null,  
 [ContadorLiqidaciones] int null,  
 [TotalValorGuia] decimal(18,6),
 [TotalValor]   decimal(18,6), 
 [TotalPendiente]	decimal(18,6), 
 [TieneHunter]   bit,  
 [IdCompany]    int,  
 [Ruc]     varchar(50),  
 [Telefono]    varchar(80),  
 [FechaInicio]  datetime,  
 [FechaFin]   datetime  
)  
declare @tmpLiquidacionFleteDetalle table
(
	[idDetail]	int,
	[idRemissionGuide] int,
	[priceSubTotal] decimal(18,6)
)  
declare @tmpValorPendiente table
(
	[idPersonFact]	int,
	[idVehicle]	int,
	[valuePending]	decimal(20,6)
)
insert into @EstadosExcluidos  
select [id]   
from [dbo].[DocumentState] where [code] in('01','05')   
  
-- obtengo las liquidaciones de flete
insert into @tmpLiquidacionFleteDetalle
select 
	lfd.[id]
	, lfd.[id_remisionGuide]
	, lfd.[pricesubtotal]
from [dbo].[LiquidationFreightDetail] lfd
join [dbo].[LiquidationFreight] lf on lfd.[id_LiquidationFreight] = lf.[id]
join [dbo].[Document] do on lf.[id] = do.[id]
join EmissionPoint emi on do.id_emissionPoint=emi.id
join Company com on emi.id_company=com.id
and do.[id_documentState] not in(1, 5)
-- (select [idEstados] from @EstadosExcluidos)
----Selecciono Todas las liquidaciones  
--Selecciono Todos los vehiculos que están en liquidacion con ciertos estados  
insert into @VehiculosLiquidacion  
select rgt.[id_vehicle]   
from [dbo].[RemissionGuideTransportation] rgt  
join [dbo].[Document] rg on rgt.[id_remionGuide] = rg.[id]
join EmissionPoint emi on rg.id_emissionPoint=emi.id
join Company com on emi.id_company=com.id
left outer join @tmpLiquidacionFleteDetalle det on rgt.[id_remionGuide] = det.[idRemissionGuide]
where 1=1
and convert(date,rg.[emissionDate]) >= case when year(@dtStart) = 1900 then convert(date,rg.[emissionDate]) else @dtStart end  
and convert(date,rg.[emissionDate]) <= case when year(@dtEnd) = 1900 then convert(date,rg.[emissionDate]) else @dtEnd end  
and rgt.[id_vehicle] is not null
group by rgt.[id_vehicle]  
 --select * from @VehiculosLiquidacion
--Selecciono TODOS lo Vehiculos SIN DISCRIMINAR  
insert into @Vehiculos  
select a.[id]  
from [dbo].[Vehicle] a join [dbo].[VehicleType] b on a.[id_VehicleType] = b.[id]  and b.[name] in ('CAMPQ',  
'CAMMD',  
'CAMGD',  
'CAMEXG',  
'CAMFG',  
'CAMEFG')  
insert into @VehiculosSinLiquidacion  
select [idVehiculos]   
from @Vehiculos where [idVehiculos] not in (select * from @VehiculosLiquidacion)  

 
insert into @ResultadosSinLiquidacion  
select [idVehiculos], (select top 1 [id_provider]  
from [dbo].[VehicleProviderTransportBilling] vptb  
where [state] = 1 and [datefin] is null and vptb.[id_vehicle] = v.[idVehiculos])   
from @VehiculosSinLiquidacion v  



--Obtengo Valores Pendientes
insert into @tmpValorPendiente
select 
ciaf.id
, t.[id_vehicle]
, sum([valuePrice]) 
from [dbo].[RemissionGuideTransportation] t
join [dbo].[Document] x on t.[id_remionGuide] = x.[id] and x.[id_documentState] not in(1, 5)
join EmissionPoint emi on x.id_emissionPoint=emi.id
join Company com on emi.id_company=com.id
join [dbo].[Vehicle] vehi on t.[id_vehicle] = vehi.[id]
join [dbo].[VehicleProviderTransportBilling] vpt on vehi.[id] = vpt.[id_vehicle] and vpt.[datefin] is null 
join [dbo].[Person] ciaf on vpt.[id_provider] = ciaf.[id]  
join [dbo].[Person] due on vehi.[id_personOwner] = due.[id]  
where
convert(date,x.[emissionDate]) >= case when year(@dtStart) = 1900 then convert(date,x.[emissionDate]) else @dtStart end  
and convert(date,x.[emissionDate]) <= case when year(@dtEnd) = 1900 then convert(date,x.[emissionDate]) else @dtEnd end   
and not exists(select a.[id_remisionGuide] 
from [dbo].[LiquidationFreightDetail] a 
join [dbo].[LiquidationFreight] b on a.[id_LiquidationFreight] = b.[id]
join [dbo].[document] d on b.[id] = d.[id] and d.id_documentState not in (1, 5) 
join EmissionPoint emi on d.id_emissionPoint=emi.id
join Company com on emi.id_company=com.id
where a.[id_remisionGuide] = t.[id_remionGuide])
and t.isown = 0
group by ciaf.id
, t.[id_vehicle]

--Obtengo Resultados Parte 1  
insert into @Resultados  
select  
 ciaf.id
 , convert(varchar(250),ciaf.[fullname_businessName])   
 , convert(varchar(250),due.[fullname_businessName])   
 , b.[id_vehicle]  
 , vehi.[carRegistration]  
 ,count(*)  
 ,sum(isnull(b.[valuePrice],0))
 ,sum(isnull(det.[pricesubtotal],0)) 
 ,0
 ,vehi.[hasHunterDevice]  
 ,@idCompany  
 ,@ruc  
 ,@telefono  
 ,@dtStart  
 ,@dtEnd  
from [dbo].[RemissionGuideTransportation] b
join [dbo].[Document] rg on rg.[id] = b.[id_remionGuide]
join EmissionPoint emi on rg.id_emissionPoint=emi.id
join Company com on emi.id_company=com.id
join [dbo].[Vehicle] vehi on b.[id_vehicle] = vehi.[id]
join [dbo].[VehicleProviderTransportBilling] vpt on vehi.[id] = vpt.[id_vehicle] and vpt.[datefin] is null 
join [dbo].[Person] ciaf on vpt.[id_provider] = ciaf.[id]  
join [dbo].[Person] due on vehi.[id_personOwner] = due.[id]  
left outer join @tmpLiquidacionFleteDetalle det on b.[id_remionGuide] = det.[idRemissionGuide]
where 1=1 
and convert(date,rg.[emissionDate]) >= case when year(@dtStart) = 1900 then convert(date,rg.[emissionDate]) else @dtStart end  
and convert(date,rg.[emissionDate]) <= case when year(@dtEnd) = 1900 then convert(date,rg.[emissionDate]) else @dtEnd end  
group by ciaf.id, ciaf.[fullname_businessName], due.[fullname_businessName], b.[id_vehicle], vehi.[carRegistration], vehi.[hasHunterDevice]
--Obtengo Resultados Parte 2 Vehiculos sin liquidacion  
union all  
select 
 ciaf.id 
 , convert(varchar(250),ciaf.[fullname_businessName])   
 , convert(varchar(250),due.[fullname_businessName])   
 , ve.[id]  
 , ve.[carRegistration]  
 , 0  
 , 0  
 , 0
 , 0
 , [hasHunterDevice]   
 , @idCompany  
 , @ruc  
 , @telefono  
 , @dtStart  
 , @dtEnd 
from @ResultadosSinLiquidacion rsl   
join [dbo].[Vehicle] ve on rsl.[idVehiculos] = ve.[id]  
left outer join [dbo].[Person] ciaf on rsl.[idCiaFactura] = ciaf.[id]   
left outer join [dbo].[Person] due on ve.[id_personOwner] = due.[id]  


--Resultado Final  
update a
set [TotalPendiente] = b.[valuePending]
from @Resultados a
left outer join @tmpValorPendiente b on a.[idVehiculos] = b.[idVehicle] and a.[idPersonFact] = b.[idPersonFact]

select *, 
	(select top(1) logo from Company where id = @idCompany) logo,
	(select top(1)logo2 from Company where id = @idCompany) logo2
from @Resultados
order by TieneHunter desc, ContadorLiqidaciones desc 
