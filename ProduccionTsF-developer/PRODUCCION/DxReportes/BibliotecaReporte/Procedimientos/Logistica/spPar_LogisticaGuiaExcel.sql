
GO
/****** Object:  StoredProcedure [dbo].[par_Guia_Remision_TerrestreFluvial]    Script Date: 07/01/2023 04:20:02 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[spPar_LogisticaGuiaExcel]
@fi varchar(10) = '',
@ff varchar(10) = ''


as 
set nocount on
declare @fiDt datetime
declare @ffDt datetime

set @fiDt = convert(date,isnull(@fi,'1900-01-01'))
set @ffDt = convert(date,isnull(@ff,'1900-01-01'))


select
doc.[id]
, doc.[sequential] as [Secuencial]
, doc.[number] as [NumeroGuia]
, doc.[emissionDate] as [FechaEmision]
, perP.[identification_number] as [ProveedorId]
, perP.[fullname_businessName] as [ProveedorNombre]
, perP.[identification_number] + ' ' + RTRIM(LTRIM(perP.[fullname_businessName])) as [ProveedorCompleto]
, pupP.[Name] as [Camaronera]
, pupC.[fullname_businessName] as [Comprador]
, convert(varchar, rgp.[despachureDate], 103) as [FechaDespacho]
, [LibrasProgramadas] = (select sum([quantityProgrammed]) 
						from [dbo].[RemissionGuideDetail] rgd 
						where rgd.[id_remisionGuide] = rgp.[id] and rgd.[isActive] = 1 )
, [LibrasRemitidas] = isnull((select sum(pld.[quantityRecived])
						from [dbo].[ProductionLotDetailPurchaseDetail] pldpd
						inner join [dbo].[RemissionGuideDetail] rgd on pldpd.[id_remissionGuideDetail] = rgd.[id] and rgd.[isActive] = 1
						inner join [dbo].[ProductionLotDetail] pld on pldpd.[id_productionLotDetail] = pld.[id]
						inner join [dbo].[RemissionGuide] rg on rgd.[id_remisionGuide] = rg.[id]
						inner join [dbo].[ProductionLot] pl on pld.[id_productionLot] = pl.[id] 
						inner join [dbo].[ProductionLotState] pls on pls.[id] = pl.[id_ProductionLotState] and pls.[code] not in ('09','01')
						where rg.[id] = rgp.[id]),0)
, RIGHT('00'+RTRIM(LTRIM(DATEPART(HOUR,rgp.[despachureHour]))),2) + ':'
	+ RIGHT('00'+RTRIM(LTRIM(DATEPART(MINUTE,rgp.[despachureHour]))),2) + ':'
	+ RIGHT('00'+RTRIM(LTRIM(DATEPART(SECOND,rgp.[despachureHour]))),2) as [HoraDespacho]
, convert(varchar, rgcv.[entranceDateProductionBuilding], 103) as [FechaEntrada]
, convert(varchar, rgcv.[exitDateProductionBuilding], 103) as [FechaSalida]
, ds.[name] as [Estado]
, vepbdc.[id_company] as [IdCompania]
, vepbdc.[businessNameCompany] as [NombreCompania]
, vepbdc.[rucCompany] as [RucCompania]
, vepbdc.[phoneNumberCompany] as [TelefonoCompania]
, convert(char(100),rgty.[name]) as [TipoGuia]
,convert(char(100),procP.[processPlant]) as [PlantaProceso]
,com.logo as Logo
,com.logo2 as logo2
from [dbo].[RemissionGuide] rgp
inner join [dbo].[Document] doc on rgp.[id] = doc.[id]
inner Join EmissionPoint emi on doc.id_emissionPoint=emi.id
inner join Company Com on emi.id_company=Com.id
inner join [dbo].[DocumentState] Ds on ds.id = doc.id_documentState
inner join [dbo].[vw_EmissionPointBranchDivisionCompany] vepbdc on doc.[id_emissionPoint] = vepbdc.[id_emissionPoint]
inner join [dbo].[RemissionGuideTransportation] rgt on rgp.[id] = rgt.[id_remionGuide]
inner join [dbo].[RemissionGuideType] rgty on rgp.[id_RemissionGuideType] = rgty.[id]
left outer join [dbo].[RemissionGuideControlVehicle] rgcv on rgcv.[id_remissionGuide] = rgp.[id]
left outer join [dbo].[ProductionUnitProvider] pupP on rgp.[id_productionUnitProvider] = pupP.[id]
join [dbo].[Person] pupC on pupC.[id] = rgp.[id_buyer]
join [dbo].[Person] perP on perP.[id] = pupP.[id_provider]
join [dbo].[Person] procP on procP.[id] = rgp.[id_personProcessPlant]
where  convert(date,doc.[emissionDate]) >= case when year(@fiDt) = 1900 then convert(date, doc.[emissionDate]) else @fiDt end
and convert(date,doc.[emissionDate]) <= case when year(@ffDt) = 1900 then convert(date, doc.[emissionDate]) else @ffDt end
union all
select 
doc.[id] as id
, doc.sequential as [Secuencial]
, doc.[number] as [NumeroGuia]
, doc.[emissionDate] as [FechaEmision]
, perP.[identification_number] as [ProveedorId]
, perP.[fullname_businessName] as [ProveedorNombre]
, perP.[identification_number] + ' ' + RTRIM(LTRIM(perP.[fullname_businessName])) as [ProveedorCompleto]
, pupP.[Name] as [camaronera]
, [comprador] = (select top 1 convert(varchar(250),isnull(pers.fullname_businessName,''))
				from [dbo].[RemissionGuideRiverDetail] rgrd 
				inner join [dbo].[RemissionGuide] rg on rgrd.[id_remisionGuide] = rg.[id]
				inner join [dbo].[Person] pers on rg.[id_buyer] = pers.[id]
				where rgrd.[id_remissionGuideRiver] = rgp.[id] )
, convert(varchar, rgp.[despachureDate], 103) as [FechaDespacho]
, [LibrasProgramadas] = (select sum([quantityProgrammed]) 
						from [dbo].[RemissionGuideRiverDetail] rgrd 
						where rgrd.[id_remissionGuideRiver] = rgp.[id] and rgrd.[isActive] = 1)
, [LibrasRemitidas] = 0
, RIGHT('00'+RTRIM(LTRIM(DATEPART(HOUR,rgp.[despachureHour]))),2) + ':'
	+ RIGHT('00'+RTRIM(LTRIM(DATEPART(MINUTE,rgp.[despachureHour]))),2) + ':'
	+ RIGHT('00'+RTRIM(LTRIM(DATEPART(SECOND,rgp.[despachureHour]))),2) as [HoraDespacho]
, convert(varchar, rgcv.[entranceDateProductionBuilding], 103) as [FechaEntrada]
, convert(varchar, rgcv.[exitDateProductionBuilding], 103) as [FechaSalida]
, ds.[name] as [estado]
, vepbdc.[id_company] as [IdCompania]
, vepbdc.[businessNameCompany] as [NombreCompania]
, vepbdc.[rucCompany] as [RucCompania]
, vepbdc.[phoneNumberCompany] as [TelefonoCompania]
, CONVERT(VARCHAR(100),'FLUVIAL') AS [TipoGuia]
,convert(char(100),procP.[processPlant]) as [PlantaProceso]
,com.logo as Logo
,com.logo2 as logo2
from [dbo].[RemissionGuideRiver] rgp
inner join [dbo].[Document] doc on rgp.[id] = doc.[id]
inner Join EmissionPoint emi on doc.id_emissionPoint=emi.id
inner join Company Com on emi.id_company=Com.id
inner join [dbo].[vw_EmissionPointBranchDivisionCompany] vepbdc on doc.[id_emissionPoint] = vepbdc.[id_emissionPoint]
inner join [dbo].[DocumentState] Ds on ds.id = doc.id_documentState
inner join [dbo].[RemissionGuideRiverTransportation] rgt on rgp.[id] = rgt.[id_remissionGuideRiver]
left outer join [dbo].[RemissionGuideRiverControlVehicle] rgcv on rgcv.id_remissionGuideRiver = rgp.[id]
left outer join [dbo].[ProductionUnitProvider] pupP on rgp.[id_productionUnitProvider] = pupP.[id]
join [dbo].[Person] perP on perP.[id] = pupP.[id_provider]
join [dbo].[Person] procP on procP.[id] = rgp.[id_personProcessPlant]
where  convert(date,doc.[emissionDate]) >= case when year(@fiDt) = 1900 then convert(date, doc.[emissionDate]) else @fiDt end
and convert(date,doc.[emissionDate]) <= case when year(@ffDt) = 1900 then convert(date, doc.[emissionDate]) else @ffDt end

order by doc.sequential desc



