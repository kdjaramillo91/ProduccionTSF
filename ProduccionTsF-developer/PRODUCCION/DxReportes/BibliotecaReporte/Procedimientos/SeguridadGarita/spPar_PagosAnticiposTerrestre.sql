/****** Object:  StoredProcedure [dbo].[spPar_PagosAnticiposTerrestre]    Script Date: 06/06/2023 01:38:41 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Query / SP  Original: Par_PagosAnticiposTerrestre
CREATE OR ALTER procedure [dbo].[spPar_PagosAnticiposTerrestre]
@str_FEmisionDateStart varchar(10) = '',
@str_FEmisionDateEnd varchar(10) = ''

as
set nocount on 

set @str_FEmisionDateStart = convert(date,isnull(@str_FEmisionDateStart,'1900-01-01'))
set @str_FEmisionDateEnd = convert(date,isnull(@str_FEmisionDateEnd,'1900-01-01'),111)

select 
	RG.[id]  as RG_id ,
	documento = convert(varchar(50),do.[sequential]),
	do.[emissionDate],
	RGT.[advancePrice] as viatico,
	cond.[fullname_businessName] as Chofer,
	cond.[identification_number] as CipersonaChofer,
	trans.[fullname_businessName] as Transportista,
	trans.[identification_number] as CipersonaTransportista,
	Vh.[carRegistration] as placa,
	co.[ruc] as ruc,
	co.[address] as direccion,
	co.[phoneNumber] as telefono,
	co.[description],
	tcs.[id],
	estado = case ltrim(rtrim(tcs.[descriptionState])) when 'Pagado' then ltrim(rtrim(tcs.[descriptionState])) else 'Pendiente' end
	, drgcat.[sequential] as [NumeroAnticipo]
	, perUser.fullname_businessName as [UsuarioPago]
from [dbo].[RemissionGuide] RG
inner join [dbo].[RemissionGuideCustomizedAdvancedTransportist] rgcat on rg.[id] = rgcat.[id_RemissionGuide]
inner join [dbo].[Document] drgcat on rgcat.[id_AdvancedTransportist] = drgcat.[id]

inner join [dbo].[Document] do on do.[id] = RG.[id]
inner join [dbo].[EmissionPoint] ep on ep.[id] = do.[id_emissionPoint]
inner join [dbo].[BranchOffice] bo on ep.[id_branchOffice] = bo.[id]
inner join [dbo].[Division] di on bo.[id_division] = di.[id]
inner join [dbo].[Company] co on di.[id_company] = co.[id]
inner join [dbo].[Person] prov on prov.[id] = RG.[id_providerRemisionGuide]  
inner join [dbo].[RemissionGuideTransportation] RGT on RGT.[id_remionGuide] = RG.[id] and isnull(RGT.[advancePrice],0) > 0
left join  [dbo].[VeicleProviderTransport] DVPT on RGT.id_vehicle = DVPT.id_vehicle and RGT.id_provider = DVPT.id_provider and DVPT.Estado = 1 and DVPT.datefin is null
left join [dbo].[Person] trans on trans.[id] = DVPT.[id_Provider]
left join [dbo].[Vehicle] VH on RGT.[id_vehicle] = VH.[id]
left join [dbo].[Person] cond on cond.[id] = RGT.[id_driver]
inner join [dbo].[ProductionUnitProvider] PUP on PUP.[id] = RG.[id_productionUnitProvider]

inner join [dbo].[tbsysCatalogState] tcs on rgcat.[id_PaymentState] = tcs.[id] and tcs.[codeClasification] = '01' 
inner join [dbo].[TController] tcon on tcs.[id_TController] = tcon.[id] and tcon.[name] = 'RemissionGuideInternControl'
left outer join [dbo].[User] userap on rgcat.id_UserApproved = userap.id
left outer join [dbo].[Person] perUser on userap.id_employee = perUser.id
where 1 = 1
and convert(date,do.[emissionDate]) >= case when year(@str_FEmisionDateStart) = 1900 then convert(date, do.[emissionDate]) else @str_FEmisionDateStart end
and convert(date, do.[emissionDate]) <= case when year(@str_FEmisionDateEnd) = 1900 then convert(date,do.[emissionDate]) else @str_FEmisionDateEnd end
order by convert(int,do.[sequential]) desc

/*
	EXEC spPar_PagosAnticiposTerrestre @str_FEmisionDateStart=N'',@str_FEmisionDateEnd=N''
*/
GO