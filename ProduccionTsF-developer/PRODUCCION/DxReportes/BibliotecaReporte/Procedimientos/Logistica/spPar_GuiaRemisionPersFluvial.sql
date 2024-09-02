
GO
/****** Object:  StoredProcedure [dbo].[par_Guia_Remision_Fluvial_Personalizada]    Script Date: 01/02/2023 03:01:58 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create  procedure [dbo].[spPar_GuiaRemisionPersFluvial]
	@id_RemissionGuideRiver int

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
, fsTn.[name] as SitioNombre
, fsTn.[name] + ' *' + fzTn.[name] + '*' as SitioCompleto
, fzTn.[code] as CodigoZona
, fzTn.[code] + ': ' + fzTn.[name] as NombreZona
, prgT.[fullname_businessName] as NombreConductor
, prgT.[identification_number] as IdConductor
, itco.[name] as NombreColor
, veh.[carRegistration] as PlacaVehiculo
, rgrl.SealNumberOneExit as SelloSalida
, rgrl.SealNumberTwoEntrance as SelloEntrada
, case when veh.[hasHunterDevice] = 1 then 'HUN HUNTER' ELSE '' END + isnull((select top 1 convert(varchar(250),fullname_businessName)
from [dbo].[RemissionGuideRiverAssignedStaff] rgas1
inner join [dbo].[RemissionGuideAssignedStaffRol] rgasr1 on rgas1.[id_assignedStaffRol] = rgasr1.[id] 
and rgasr1.[code] = 'SEG'
inner join [dbo].[Person] prgas1 on rgas1.[id_person] = prgas1.[id] 
where rgas1.[id_remissionGuideRiver] = rgp.[id] and rgas1.[isActive] = 1),'') as NombreSeguridad
, isnull((select top 1 convert(varchar(250),fullname_businessName)
from [dbo].[RemissionGuideRiverAssignedStaff] rgas1
inner join [dbo].[RemissionGuideAssignedStaffRol] rgasr1 on rgas1.[id_assignedStaffRol] = rgasr1.[id] 
and rgasr1.[code] = 'BIO'
inner join [dbo].[Person] prgas1 on rgas1.[id_person] = prgas1.[id] 
where rgas1.[id_remissionGuideRiver] = rgp.[id] and rgas1.[isActive] = 1 ),'') as NombreBiologo

, doc.[description] as DescripcionDocumento
, (select sum([QuantityProgrammed])
	from [dbo].[RemissionGuideRiverDetail]
	where [id_remissionGuideRiver] = rgp.[id]) as [LibrasProgramadas]
, (select top 1 pt2.[name]
	from [dbo].[RemissionGuideRiverDetail] rgd2
	join [dbo].[ItemProcessType] ipt2 on rgd2.[id_item] = ipt2.[Id_Item]
	join [dbo].[ProcessType] pt2 on ipt2.[Id_ProcessType] = pt2.[id]
	where [id_remissionGuideRiver] = rgp.[id]) as [TipoProceso]
, 'Clave Acceso: ' + RTRIM(LTRIM(doc.[accessKey])) as ClaveAcceso
, '' as [ProveedorAmparante]
, rgt.[descriptionTrans] as [DescripcionTransporte]
, isnull((select TOP 1 pupDe.[INPnumber]
  from [dbo].[RemissionGuideDetail] rgde 
  join [dbo].[RemissionGuideDetailPurchaseOrderDetail] plpd on rgde.[id] = plpd.[id_remissionGuideDetail]
  join [dbo].[PurchaseOrderDetail] pode  on pode.[id] = plpd.[id_purchaseOrderDetail]
  join [dbo].[PurchaseOrder] po on po.[id] = pode.[id_purchaseOrder]
  left outer join [dbo].[ProductionUnitProvider] pupDe on pupDe.[id] = po.[id_productionUnitProviderProtective]
  where rgde.[id_remisionGuide] = @id_RemissionGuideRiver),'') as INP
, convert(varchar, rgp.[despachureDate], 103) as FechaDespacho
, RIGHT('00'+RTRIM(LTRIM(DATEPART(HOUR,rgp.[despachureHour]))),2) + ':'
	+ RIGHT('00'+RTRIM(LTRIM(DATEPART(MINUTE,rgp.[despachureHour]))),2) + ':'
	+ RIGHT('00'+RTRIM(LTRIM(DATEPART(SECOND,rgp.[despachureHour]))),2) as HoraDespacho
from [dbo].[RemissionGuideRiver] rgp
inner join [dbo].[Document] doc on rgp.[id] = doc.[id]
inner join [dbo].[RemissionGuideRiverTransportation] rgt on rgp.[id] = rgt.[id_remissionGuideRiver]
inner join [dbo].[Vehicle] veh on rgt.[id_vehicle] = veh.[id]
inner join [dbo].[ItemColor] itco on veh.[id_itemColor] = itco.[id]
left outer join [dbo].[RemissionGuideRiverReportLinealDataHelp] rgrl on rgrl.[id_RemissionGuideRiver] = rgp.[id]
inner join [dbo].[Person] prgT on rgt.[id_driver] = prgT.[id]
join [dbo].[ProductionUnitProvider] pupP on rgp.[id_productionUnitProvider] = pupP.[id]
--join [dbo].[Person] pupA on pupA.[id] = rgp.[id_protectiveProvider]
join [dbo].[Person] perP on perP.[id] = pupP.[id_provider]
left outer join [dbo].[FishingSite] fsTn on rgt.[id_FishingSiteRGR] = fsTn.[id]
left outer join [dbo].[FishingZone] fzTn on fsTn.[id_FishingZone] = fzTn.[id]
where rgp.[id] = @id_RemissionGuideRiver


--[dbo].[par_Guia_Remision_Fluvial_Personalizada] 140287
