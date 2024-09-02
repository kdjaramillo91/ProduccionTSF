--select * from RemissionGuide where id = 118773
-- insert into ObjectPermission values ('ENTVEHTER','Entrada de Vehiculos Terceros','Reporte Entrada de Vehiculos Terceros',1,1,GETDATE(),1,GETDATE())
-- insert into ObjectPermission values ('SALVEHTER','Salida de Vehiculos Terceros','Reporte Salida de Vehiculos Terceros',1,1,GETDATE(),1,GETDATE())
-- HACER INSERT PRIMERO
create or alter procedure SP_EntradaVehiculos
@FechaEmisionInicio			datetime,
@FechaEmisionFinal			datetime
as
SET NOCOUNT ON
declare @fiDt date
declare @ffDt date

set @fiDt = convert(date,isnull(@FechaEmisionInicio,'1900-01-01'))

set @ffDt = convert(date,isnull(@FechaEmisionFinal,'1900-01-01'))

select 
a.number as Numero
,b.name as Estado
,d.processPlant as Proceso
,e.fullname_businessName as Proveedor
,f.name as Camaronera
,g.name as Zona
,h.name as Sitio
,i.drivername as Chofer
,i.carRegistration as Placa
,j.exitDateProductionBuilding as Fecha_Salida
,j.exitTimeProductionBuilding as Hora_Salida
,j.entranceDateProductionBuilding as Fecha_Entrada
,j.entranceTimeProductionBuilding as Hora_Entrada
,j.entranceDateProductionBuilding as Fecha_llegada_Camaronera
,j.entranceTimeProductionUnitProviderBuilding as Hora_llegada_Camaronera
,j.exitDateProductionUnitProviderBuilding as Fecha_Salida_Camaronera
,j.exitTimeProductionUnitProviderBuilding as Hora_Salida_Camaronera
,k.quantityReceived as Libras_remitidas
,k.productionUnitProviderPoolreference as Piscinas
,j.ObservationEntrance as Observacion
,l.username as Usuario_Creacion
,a.dateUpdate as Fecha_Creacion
,m.username as Usuario_Modificacion
,a.dateUpdate as Fecha_Modificacion
,c.hasEntrancePlanctProduction
,c.hasExitPlanctProduction
from document a 
inner join documentstate b on b.id = a.id_documentState
inner join remissionguide c on c.id = a.id
inner join person e on e.id = c.id_providerRemisionGuide
inner join ProductionUnitProvider f on f.id = c.id_productionUnitProvider
inner join FishingZone g on g.id = f.id_FishingZone
inner join FishingSite h on h.id = f.id_FishingSite
inner join remissionguidetransportation i on i.id_remionguide = c.id
inner join RemissionGuideControlVehicle j on j.id_remissionguide = c.id
inner join RemissionGuideDetail k on k.id_remisionGuide = c.id
left join [User] l on l.id_employee = a.id_userCreate
left join [User] m on m.id_employee = a.id_userUpdate
left join person d on d.id = c.id_personProcessPlant
where c.hasEntrancePlanctProduction =1 --and c.hasExitPlanctProduction =1
and
convert(date,a.emissionDate) >= case when year(@fiDt) = 1900 then convert(date, a.emissionDate) else @fiDt end
and convert(date,a.emissionDate) <= case when year(@ffDt) = 1900 then convert(date, a.emissionDate) else @ffDt end


--select * from RemissionGuide 
--select * from RemissionGuideControlVehicle  where id_remissionguide = 118773

--select * from RemissionGuideDetail where id_remisionguide = 118773


--select * from document where id_documentType = 8 and sequential = 88550



--select * from RemissionGuideControlVehicle  where id_remissionguide = 118773

--select * from RemissionGuideDetail where id_remisionguide = 118773