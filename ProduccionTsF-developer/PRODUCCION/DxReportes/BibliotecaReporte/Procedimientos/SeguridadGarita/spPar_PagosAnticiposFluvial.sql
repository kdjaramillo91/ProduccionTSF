GO
/****** Object:  StoredProcedure [dbo].[par_GuideRemisionViaticoTransportistaFluvial]    Script Date: 27/02/2023 12:51:12 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


Create procedure [dbo].[spPar_PagosAnticiposFluvial]
@str_FEmisionDateStart varchar(10) = '',
@str_FEmisionDateEnd varchar(10) = ''

as
set nocount on 

set @str_FEmisionDateStart = convert(date,isnull(@str_FEmisionDateStart,'1900-01-01'))
set @str_FEmisionDateEnd = convert(date,isnull(@str_FEmisionDateEnd,'1900-01-01'),111)

select 
RG.[id]  as RG_id ,
logo = co.[logo2],
documento = convert(varchar(50),do.[sequential]),
do.[emissionDate],
RGT.[advancePrice] as viatico,
cond.[fullname_businessName] as Chofer,
cond.[identification_number] as CipersonaChofer,
person3.[fullname_businessName] as Transportista,
person3.[identification_number] as CipersonaTransportista,
Vh.[carRegistration] as placa,
co.[ruc] as ruc,
co.[address] as direccion,
co.[phoneNumber] as telefono,
co.[description],
tcs.[id],
estado = case ltrim(rtrim(tcs.[descriptionState])) when 'Pagado' then ltrim(rtrim(tcs.[descriptionState])) else 'Pendiente' end
, drgcr.[sequential] as NumeroAnticipoFluvial
, perUser.fullname_businessName as [UsuarioPago]
from [dbo].[RemissionGuideRiver] RG
inner join [dbo].[RemissionGuideRiverCustomizedAdvancedTransportist] rgrcat on rg.id = rgrcat.[id_RemissionGuideRiver]
inner join [dbo].[Document] drgcr on rgrcat.[id_AdvancedTransportist] = drgcr.[id]
inner join [dbo].[Person] prov on prov.[id] = RG.[id_providerRemisionGuideRiver]  
inner join [dbo].[RemissionGuideRiverTransportation] RGT on RGT.[id_remissionGuideRiver] = RG.[id] and isnull(RGT.[advancePrice],0) > 0
left join  [dbo].[VeicleProviderTransport] DVPT on RGT.[id_vehicle] = DVPT.[id_vehicle] and RGT.[id_provider] = DVPT.[id_Provider]
left join [dbo].[Person] Person3 on Person3.[id] = DVPT.[id_Provider]
left join [dbo].[Vehicle] VH on RGT.[id_vehicle] = VH.[id]
left join [dbo].[Person] cond on cond.[id] = RGT.[id_driver]
inner join [dbo].[ProductionUnitProvider] PUP on PUP.[id] = RG.[id_productionUnitProvider]
inner join [dbo].[Document] do on do.[id] = RG.[id]
inner join [dbo].[EmissionPoint] ep on ep.[id] = do.[id_emissionPoint]
inner join [dbo].[Company] co on co.[id] = ep.[id_company]
inner join  [dbo].[tbsysCatalogState] tcs on rgrcat.[id_paymentstate] = tcs.[id] and tcs.[codeClasification] = '01' 
inner join [dbo].[TController] tcon on tcs.[id_TController] = tcon.[id] and tcon.[name] = 'RemissionGuideInternControl'
left outer join [dbo].[User] userap on rgrcat.id_UserApproved = userap.id
left outer join [dbo].[Person] perUser on userap.id_employee = perUser.id
where 1 = 1
and convert(date,do.[emissionDate]) >= case when year(@str_FEmisionDateStart) = 1900 then convert(date, do.[emissionDate]) else @str_FEmisionDateStart end
and convert(date, do.[emissionDate]) <= case when year(@str_FEmisionDateEnd) = 1900 then convert(date,do.[emissionDate]) else @str_FEmisionDateEnd end

order by convert(int,do.[sequential]) desc

	  --execute par_GuideRemisionViaticoTransportistaFluvial null,null
