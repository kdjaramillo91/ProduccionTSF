
GO
/****** Object:  StoredProcedure [dbo].[par_GuideRemisionViaticoFluvial]    Script Date: 02/02/2023 01:13:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create procedure [dbo].[spPar_GuiaRemisionViaticoFluvial]
@str_FEmisionDateStart varchar(10) = '',
@str_FEmisionDateEnd varchar(10) = ''

as
set nocount on 

set @str_FEmisionDateStart = convert(date,isnull(@str_FEmisionDateStart,'1900-01-01'))
set @str_FEmisionDateEnd = convert(date,isnull(@str_FEmisionDateEnd,'1900-01-01'),111)

select 
	RG.id  as RG_id ,
	logo2=company.logo,
	logo = Company.logo2,
	documento = Document.number,
	document.emissionDate,RGAS.
	viaticPrice as viatico,
	person4.fullname_businessName as persona,
	person4.identification_number as cipersona,
	rgasr.description as rol,
	rgtt.name as viaje,
	company.ruc as ruc,
	company.address as direccion,
	company.phoneNumber as telefono,
	company.description,
	estado = case ltrim(rtrim(tcs.descriptionState)) when 'Pagado' then ltrim(rtrim(tcs.descriptionState)) else 'Pendiente' end	
	, drgrcv.sequential as NumeroViaticoFluvial	
	, perUser.fullname_businessName as [UsuarioPago]					 
from RemissionGuideRiver RG
left outer join [dbo].[RemissionGuideRiverCustomizedViaticPersonalAssigned] rgrcvpa on rg.id = rgrcvpa.id_RemissionGuideRiver
left outer join [dbo].[Document] drgrcv on rgrcvpa.[id_ViaticPersonalAssigned] = drgrcv.[id]
inner join Person Person on Person.id =RG.id_providerRemisionGuideRiver  
inner join  RemissionGuideRiverAssignedStaff   RGAS on RGAS.id_remissionGuideRiver = rg.id and isnull(RGAS.[viaticPrice],0) > 0
inner  join Person person4 on person4.id = RGAS.id_person
inner  join RemissionGuideAssignedStaffRol rgasr on rgasr.id = RGAS.id_assignedStaffRol
inner  join RemissionGuideTravelType rgtt on rgtt.id = RGAS.id_travelType
inner join ProductionUnitProvider PUP on PUP.id = RG.id_productionunitprovider
inner join "dbo"."Document" "Document" on ("Document"."id" = RG.id)
inner join "dbo"."EmissionPoint" "EmissionPoint" on ("EmissionPoint"."id" = "Document"."id_emissionPoint")
inner join "dbo"."Company" "Company" on ("Company"."id" = "EmissionPoint"."id_company")
inner join  [dbo].[tbsysCatalogState] tcs on rgrcvpa.[id_paymentstate] = tcs.[id] and tcs.[codeClasification] = '01'
left outer join [dbo].[User] userap on rgrcvpa.id_UserApproved = userap.id
left outer join [dbo].[Person] perUser on userap.id_employee = perUser.id 
where 1 = 1
and convert(date,Document.[emissionDate]) >= case when year(@str_FEmisionDateStart) = 1900 then convert(date, Document.[emissionDate]) else @str_FEmisionDateStart end
and convert(date, Document.[emissionDate]) <= case when year(@str_FEmisionDateEnd) = 1900 then convert(date,Document.[emissionDate]) else @str_FEmisionDateEnd end
order by Document.number

	   --execute [dbo].[par_GuideRemisionViaticoFluvial]'',''
