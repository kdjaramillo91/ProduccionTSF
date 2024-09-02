
GO
/****** Object:  StoredProcedure [dbo].[par_Guia_Remision_Personalizada]    Script Date: 07/01/2023 18:22:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[spPar_GuiaRemisionPers]
	@id_RemissionGuide int

as 
set nocount on

select 
doc.[number] as NumeroDocumento
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
, case when rgt.[isOwn] = 0 then itco.[name] else '' end as NombreColor
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
, 'Clave Acceso: ' + RTRIM(LTRIM(doc.[accessKey])) as ClaveAcceso
, pupA.[fullname_businessName] as ProveedorAmparante
, rgt.[descriptionTrans] as DescripcionTransporte
, isnull((select TOP 1 pupDe.[INPnumber]
  from [dbo].[RemissionGuideDetail] rgde 
  join [dbo].[RemissionGuideDetailPurchaseOrderDetail] plpd on rgde.[id] = plpd.[id_remissionGuideDetail]
  join [dbo].[PurchaseOrderDetail] pode  on pode.[id] = plpd.[id_purchaseOrderDetail]
  join [dbo].[PurchaseOrder] po on po.[id] = pode.[id_purchaseOrder]
  left outer join [dbo].[ProductionUnitProvider] pupDe on pupDe.[id] = po.[id_productionUnitProvider]
  where rgde.[id_remisionGuide] = @id_RemissionGuide),'') as INP
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
  pupP.name As Camaronera,
  isnull((select TOP 1 Name
  from [dbo].[RemissionGuideDetail] rgde 
  join [dbo].[RemissionGuideDetailPurchaseOrderDetail] plpd on rgde.[id] = plpd.[id_remissionGuideDetail]
  join [dbo].[PurchaseOrderDetail] pode  on pode.[id] = plpd.[id_purchaseOrderDetail]
  join [dbo].[PurchaseOrder] po on po.[id] = pode.[id_purchaseOrder]
  join [dbo].[PriceList] PL on PL.[id] = po.[id_priceList]
  where rgde.[id_remisionGuide] = @id_RemissionGuide),'') as ListaPrecio
--EV---------------------------------------------------------------------------------------
, crt.[name] as Certificado
--EV---------------------------------------------------------------------------------------

from [dbo].[RemissionGuide] rgp
inner join [dbo].[Document] doc on rgp.[id] = doc.[id]
inner join [dbo].[RemissionGuideTransportation] rgt on rgp.[id] = rgt.[id_remionGuide]
left outer join [dbo].[Vehicle] veh on rgt.[id_vehicle] = veh.[id]
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
inner join [dbo].[Person] procPlan on procPlan.[id] = rgp.[id_personProcessPlant]
---EV--------------------------------------------------------------------------------
left outer join [dbo].[Certification] crt on crt.id = rgp.id_certification
---EV--------------------------------------------------------------------------------
where rgp.[id] = @id_RemissionGuide

--exec [par_Guia_Remision_Personalizada] 508544
