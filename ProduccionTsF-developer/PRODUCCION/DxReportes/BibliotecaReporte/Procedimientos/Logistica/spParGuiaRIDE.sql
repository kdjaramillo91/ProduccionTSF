GO
/****** Object:  StoredProcedure [dbo].[par_Guia_Remision_RIDE]    Script Date: 01/02/2023 00:11:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create procedure [dbo].[spParGuiaRIDE]  

 @id_RemissionGuide int  
  
as   
set nocount on  
select   
rgp.id as ID
,cp.logo as Logo
,cp.logo2 as logo2
,cp.trademark as Compania
,cp.address as Direccion
,cp.address as Sucursal
,cef.resolutionNumber as ContribuyenteEspecial
,CP.ruc as Ruc
,doc.[number] as NumeroDocumento  
,doc.emissionDate as FechaEmision
, DATEPART(DAY,doc.[emissionDate]) as DiaEmision  
, DATEPART(MONTH,doc.[emissionDate]) as MesEmision  
, DATEPART(YEAR,doc.[emissionDate]) as AnioEmision  
, perP.[identification_number] as ProveedorId  
, perP.[fullname_businessName] as ProveedorNombre  
, perP.[identification_number] + ' ' + RTRIM(LTRIM(perP.[fullname_businessName])) as ProveedorCompleto  
, case when rgt.[isOwn] = 0 then fsTn.[name] else fsTn3.[name] end as SitioNombre  
, case when rgt.[isOwn] = 0 then fsTn.[name] + ' *' + fzTn.[name] + '*' else fsTn3.[name] + ' *' + fzTn3.[name] + '*' end as SitioCompleto  
, case when rgt.[isOwn] = 0 then fzTn.[code] else fzTn3.[code] end as CodigoZona  
, case when rgt.[isOwn] = 0 then fzTn.[code] + ': ' + fzTn.[name] else fzTn3.[code] + ': ' + fzTn3.[name] end as NombreZona  
, case when rgt.[isOwn] = 0 then prgT.[fullname_businessName] else rgt.[driverName] end as NombreConductor  
, isnull(prgT.[identification_number],'') as IdConductor  
, case when rgt.[isOwn] = 0 then veh.mark + ' / ' + itco.[name] else '' end as NombreColor  
, rgt.[carRegistration] as PlacaVehiculo  
, rgrl.SealNumberOneExit as SelloSalida  
, rgrl.SealNumberTwoEntrance as SelloEntrada  
, case when veh.[hasHunterDevice] = 1 then 'HUN HUNTER' ELSE '' END + isnull((select top 1 convert(varchar(250),fullname_businessName)  
from [dbo].[RemissionGuideAssignedStaff] rgas1  
inner join [dbo].[RemissionGuideAssignedStaffRol] rgasr1 on rgas1.[id_assignedStaffRol] = rgasr1.[id]   
and rgasr1.[code] = 'SEG'  
inner join [dbo].[Person] prgas1 on rgas1.[id_person] = prgas1.[id]   
where rgas1.[id_remissionGuide] = rgp.[id] and rgas1.[isActive] = 1),'') as NombreSeguridad  
, isnull((select top 1 convert(varchar(250),fullname_businessName)  
from [dbo].[RemissionGuideAssignedStaff] rgas1  
inner join [dbo].[RemissionGuideAssignedStaffRol] rgasr1 on rgas1.[id_assignedStaffRol] = rgasr1.[id]   
and rgasr1.[code] = 'BIO'  
inner join [dbo].[Person] prgas1 on rgas1.[id_person] = prgas1.[id]   
where rgas1.[id_remissionGuide] = rgp.[id] and rgas1.[isActive] = 1 ),'') as NombreBiologo  
, doc.[description] as DescripcionDocumento  
, (select sum([QuantityProgrammed])  
 from [dbo].[RemissionGuideDetail]  
 where [id_remisionGuide] = rgp.[id]) as [LibrasProgramadas]  
, (select top 1 pt2.[name]  
 from [dbo].[RemissionGuideDetail] rgd2  
 join [dbo].[ItemProcessType] ipt2 on rgd2.[id_item] = ipt2.[Id_Item]  
 join [dbo].[ProcessType] pt2 on ipt2.[Id_ProcessType] = pt2.[id]  
 where [id_remisionGuide] = rgp.[id]) as [TipoProceso]  
,  RTRIM(LTRIM(doc.[accessKey])) as ClaveAcceso  
, pupA.[fullname_businessName] as ProveedorAmparante  
, rgt.[descriptionTrans] as DescripcionTransporte  
, isnull((select TOP 1 pupDe.[INPnumber]  
  from [dbo].[RemissionGuideDetail] rgde   
  join [dbo].[RemissionGuideDetailPurchaseOrderDetail] plpd on rgde.[id] = plpd.[id_remissionGuideDetail]  
  join [dbo].[PurchaseOrderDetail] pode  on pode.[id] = plpd.[id_purchaseOrderDetail]  
  join [dbo].[PurchaseOrder] po on po.[id] = pode.[id_purchaseOrder]  
  left outer join [dbo].[ProductionUnitProvider] pupDe on pupDe.[id] = po.[id_productionUnitProvider]  
  where rgde.[id_remisionGuide] = @id_RemissionGuide),'') as INP  
, isnull((select TOP 1 pode.[productionUnitProviderPoolreference]  
from [dbo].[RemissionGuideDetail] rgde   
join [dbo].[RemissionGuideDetailPurchaseOrderDetail] plpd on rgde.[id] = plpd.[id_remissionGuideDetail]  
join [dbo].[PurchaseOrderDetail] pode  on pode.[id] = plpd.[id_purchaseOrderDetail]  
where rgde.[id_remisionGuide] = @id_RemissionGuide),'') as Piscina 
, convert(varchar, rgp.[despachureDate], 103) as FechaDespacho  
, RIGHT('00'+RTRIM(LTRIM(DATEPART(HOUR,rgp.[despachureHour]))),2) + ':'  
 + RIGHT('00'+RTRIM(LTRIM(DATEPART(MINUTE,rgp.[despachureHour]))),2) + ':'  
 + RIGHT('00'+RTRIM(LTRIM(DATEPART(SECOND,rgp.[despachureHour]))),2) as HoraDespacho,  
isnull((select TOP 1 fullname_businessName  
  from [dbo].[RemissionGuideDetail] rgde   
  join [dbo].[RemissionGuideDetailPurchaseOrderDetail] plpd on rgde.[id] = plpd.[id_remissionGuideDetail]  
  join [dbo].[PurchaseOrderDetail] pode  on pode.[id] = plpd.[id_purchaseOrderDetail]  
  join [dbo].[PurchaseOrder] po on po.[id] = pode.[id_purchaseOrder]  
  join [dbo].[Person] pe on pe.[id] = po.[id_buyer]  
  where rgde.[id_remisionGuide] = @id_RemissionGuide),'') as Comprador,  
  Convert(varchar(250),procPlan.[processPlant]) as ProcesoPlanta,  
  Isnull(tra.fullname_businessName,'') as CiaTransporte,  
  Isnull(tra.identification_number,0) as RucTransporte,  
  Isnull(tra.address,'') as DireccionTransporte,  
  doc.emissionDate as FechaEmision,  
  isnull(cp.address,'') as DireccionEmpresa,  
  Isnull(veht.Description,'') as TipoVehiculo,  
  Isnull(pupP.name,'') as Camaronera,  
  fzTn4.name + ' / ' + fsTn3.name + ' / ' + pupP.address as DireccionDestino  
  ,rgr.name as Razon
  ,case when rgt.isOwn=1 then rgp.route else 
  rgp.startAdress end as PuntoPartida
  ,case when rgt.isOwn=1 then 
  rgp.startAdress else 
  rgp.route end  as Destino
  ,upper(dct.name) as tipoDocumento
from [dbo].[RemissionGuide] rgp  
inner join [dbo].[Document] doc on rgp.[id] = doc.[id]  
inner join [dbo].[RemissionGuideTransportation] rgt on rgp.[id] = rgt.[id_remionGuide]  
left outer join [dbo].[Vehicle] veh on rgt.[id_vehicle] = veh.[id]  
left outer join [dbo].[VehicleType] veht on veh.[id_VehicleType] = veht.[id]  
left outer join [dbo].[ItemColor] itco on veh.[id_itemColor] = itco.[id]  
left outer join [dbo].[RemissionGuideReportLinealDataHelp] rgrl on rgrl.[id_RemissionGuide] = rgp.[id]  
left outer join [dbo].[Person] prgT on rgt.[id_driver] = prgT.[id]  
left outer join [dbo].[ProductionUnitProvider] pupP on rgp.[id_productionUnitProvider] = pupP.[id]  
left outer join [dbo].[Person] pupA on pupA.[id] = rgp.[id_protectiveProvider]  
left outer join [dbo].[Person] perP on perP.[id] = pupP.[id_provider]  
left outer join [dbo].[FishingSite] fsTn on rgt.[id_FishingSiteRG] = fsTn.[id]  
left outer join [dbo].[FishingZone] fzTn on fsTn.[id_FishingZone] = fzTn.[id]  
left outer join [dbo].[FishingSite] fsTn3 on pupP.[id_FishingSite] = fsTn3.[id]  
left outer join [dbo].[FishingZone] fzTn3 on fsTn3.[id_FishingZone] = fzTn3.[id]   
left outer join [dbo].[FishingZone] fzTn4 on pupP.[id_FishingZone] = fzTn4.[id]   
inner join [dbo].[Person] procPlan on procPlan.[id] = rgp.[id_personProcessPlant]  
inner Join EmissionPoint emp On emp.id = doc.id_emissionPoint  
Inner Join Company cp on cp.id = emp.id_company  
inner join RemissionGuideReason rgr on rgr.id = rgp.id_reason
inner join CompanyElectronicFacturation cef on cef.id_company = cp.id
inner join documenttype dct on dct.id = doc.id_documentType
left outer join (Select drv.id,per.fullname_businessName, identification_number, address    
      from DriverVeicleProviderTransport drv  
     Inner Join VeicleProviderTransport vpt On vpt.id = drv.idVeicleProviderTransport  
     Inner Join Person per On per.id = vpt.id_Provider) tra  
    On tra.id = rgt.id_DriverVeicleProviderTransport  
	

--left outer join [dbo].[RemissionGuideSecuritySeal] rgse on rgse.id_remissionGuide = rgp.id  
where rgp.[id] = @id_RemissionGuide  
  
  
  --select *from RemissionGuideTransportation