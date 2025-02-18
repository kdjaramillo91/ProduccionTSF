If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_GuideRemisionViaticoCustomizeFluvialCR'
	)
Begin
	Drop Procedure dbo.par_GuideRemisionViaticoCustomizeFluvialCR
End
Go
Create Procedure dbo.par_GuideRemisionViaticoCustomizeFluvialCR
(
	@id int 
)
As 
set nocount on

declare @viatico decimal(18,6)

select 
	do.[id] as [id]
	, CONVERT(VARCHAR(20),do.[sequential]) as [NumeroGuia]
	, do.[emissionDate] as [FechaEmision]
	, isnull(rgrt.[advancePrice], 0) as [AnticipoTransportista]
	, cond.[identification_number] as [IdConductor]
	, cond.[fullname_businessName] as [NombreConductor]
	, veh.[carRegistration] as [PlacaVehiculo]
	, veh.[mark] as [MarcaVehiculo]
	, veh.[model] as [ModeloVehiculo]
	, prov.[fullname_businessName] as [NombreProveedor]
	, citra.[fullname_businessName] as [CiaTransportista]
	, citra.[identification_number] as [IdTransportista]
	, pup.[address] as [DireccionDestino]
	, fs.[address] as [SitioDestino]
	, [CantidadLibras] = (select sum(isnull(quantityProgrammed,0)) 
						from [dbo].[RemissionGuideRiverDetail] 
						where [id_remissionGuideRiver] = rgr.[id])
	, 'IDA Y VUELTA' as [TipoViaje]
	, dbo.FUN_CantidadConLetracastellano(convert(INTEGER,isnull(rgrt.[advancePrice], 0)))+ ' ' + SUBSTRING(RTRIM(LTRIM(PARSENAME(isnull(rgrt.[advancePrice], 0),1))),1,2) +   '/100 DOLARES' as  [AnticipoTransportistaLetras]
	, rgrt.[descriptionTrans] as [DescripcionTransportista]
	, CONVERT(VARCHAR(20),drgrc.[sequential]) as NumeroAnticipoFluvial
from [dbo].[RemissionGuideRiverTransportation] rgrt 
--inner join [dbo].[VeicleProviderTransport] vpt on rgrt.[id_provider] = vpt
left outer join [dbo].[RemissionGuideRiverCustomizedAdvancedTransportist] rgrcvpa on rgrt.[id_remissionGuideRiver] = rgrcvpa.[id_RemissionGuideRiver]
left outer join [dbo].[Document] drgrc on rgrcvpa.[id_AdvancedTransportist] = drgrc.[id]
inner join [dbo].[RemissionGuideRiver] rgr on rgrt.[id_remissionGuideRiver] = rgr.[id] and rgr.[id] = @id
inner join [dbo].[Document] do on do.[id] = rgr.[id]
inner join [dbo].[EmissionPoint] ep on do.[id_emissionPoint] = ep.[id]
inner join [dbo].[BranchOffice] bo on ep.[id_branchOffice] = bo.[id]
inner join [dbo].[Division] di on bo.[id_division] = di.[id]
inner join [dbo].[Company] co on di.[id_company] = co.[id]
inner join [dbo].[Vehicle] veh on rgrt.[id_vehicle] = veh.[id]
inner join [dbo].[Person] cond on rgrt.[id_driver] = cond.[id]
inner join [dbo].[Person] prov on rgr.[id_providerRemisionGuideRiver] = prov.[id]
inner join [dbo].[Person] citra on rgrt.[id_provider] = citra.[id]
inner join [dbo].[ProductionUnitProvider] pup on rgr.[id_productionUnitProvider] = pup.[id]
inner join [dbo].[FishingSite] fs on rgrt.[id_FishingSiteRGR] = fs.[id]
Go
