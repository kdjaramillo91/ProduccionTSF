If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_GuideRemisionViaticoSupervisorFluvialCR'
	)
Begin
	Drop Procedure dbo.par_GuideRemisionViaticoSupervisorFluvialCR
End
Go
Create Procedure dbo.par_GuideRemisionViaticoSupervisorFluvialCR
(
	@id int 
)
As 
set nocount on

declare @viatico decimal(18,6)

select  @viatico = SUM(viaticPrice) 
from [dbo].[RemissionGuideRiverAssignedStaff] 
where [id_remissionGuideRiver] = @id
and isActive = 1

select 
	do.[id] as [id]
	, CONVERT(VARCHAR(20),do.[number]) as [NumeroDocumento]
	, do.[emissionDate] as [FechaEmision]
	, peas.[identification_number] as [IdPersona]
	, peas.[fullname_businessName] as [NombrePersona]
	, rgasr.[name] as [RolPersona]
	, rgtt.[name] as [TipoViaje]
	, isnull(rgas.[viaticPrice],0) as [ViaticoPrecio]
	, @viatico as [ViaticoValorTotal]
	, prov.[fullname_businessName] as [NombreProveedor]
	, pup.[address] as [DireccionDestino]
	, fs.[address] as [SitioDestino]
	, [CantidadLibras] = (select sum(isnull(quantityProgrammed,0)) 
						from [dbo].[RemissionGuideRiverDetail] 
						where [id_remissionGuideRiver] = rgr.[id])
	, dbo.FUN_CantidadConLetracastellano(convert(INTEGER,@viatico))+ ' ' + SUBSTRING(RTRIM(LTRIM(PARSENAME(@viatico,1))),1,2) +   '/100 DOLARES' as  [ValorViaticoTotalLetras]
	, co.[logo2] as [Logo]
	, CONVERT(VARCHAR(20),drgrc.[sequential]) as NumeroViaticoFluvial
from [dbo].[RemissionGuideRiverAssignedStaff] rgas
left outer join [dbo].[RemissionGuideRiverCustomizedViaticPersonalAssigned] rgrcvpa on rgas.[id_remissionGuideRiver] = rgrcvpa.[id_RemissionGuideRiver]
left outer join [dbo].[Document] drgrc on rgrcvpa.[id_ViaticPersonalAssigned] = drgrc.[id]
inner join [dbo].[RemissionGuideAssignedStaffRol] rgasr on rgas.[id_assignedStaffRol] = rgasr.[id]
inner join [dbo].[RemissionGuideTravelType] rgtt on rgas.[id_travelType] = rgtt.[id]
inner join [dbo].[RemissionGuideRiver] rgr on rgas.[id_remissionGuideRiver] = rgr.[id] and rgr.[id] = @id
inner join [dbo].[RemissionGuideRiverTransportation] rgrt on rgr.[id] = rgrt.[id_remissionGuideRiver]
inner join [dbo].[Document] do on do.[id] = rgr.[id]
inner join [dbo].[EmissionPoint] ep on do.[id_emissionPoint] = ep.[id]
inner join [dbo].[BranchOffice] bo on ep.[id_branchOffice] = bo.[id]
inner join [dbo].[Division] di on bo.[id_division] = di.[id]
inner join [dbo].[Company] co on di.[id_company] = co.[id]
inner join [dbo].[Person] peas on rgas.[id_person] = peas.[id]
inner join [dbo].[Person] prov on rgr.[id_providerRemisionGuideRiver] = prov.[id]
inner join [dbo].[ProductionUnitProvider] pup on rgr.[id_productionUnitProvider] = pup.[id]
inner join [dbo].[FishingSite] fs on rgrt.[id_FishingSiteRGR] = fs.[id]
Go
