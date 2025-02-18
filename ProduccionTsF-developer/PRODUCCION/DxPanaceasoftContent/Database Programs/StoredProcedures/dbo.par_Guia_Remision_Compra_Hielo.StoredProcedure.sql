If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_Guia_Remision_Compra_Hielo'
	)
Begin
	Drop Procedure dbo.par_Guia_Remision_Compra_Hielo
End
Go
Create Procedure dbo.par_Guia_Remision_Compra_Hielo
(
	@id int
)	
As
set nocount on 



-------BIOLOGO-------------------------------------------------

declare @IdBiologo varchar(50)
declare @NombreBiologo varchar(200)

select TOP 1 @IdBiologo = pers.[identification_number] 
, @NombreBiologo = convert(varchar(200),pers.[fullname_businessName])
from [dbo].[RemissionGuideAssignedStaff] rgas
join [dbo].[RemissionGuideAssignedStaffRol] rgasr 
on rgas.[id_assignedStaffRol] = rgasr.[id] and rgasr.[code] = 'BIO'
join [dbo].[Person] pers on rgas.[id_person] = pers.[id]
where [id_remissionGuide] = @id

---------CHOFER-------------------------------------------------
--DECLARACION DE VARIABLE
declare @IdChofer varchar(50)
declare @NombreChofer varchar(200)

select TOP 1 @IdChofer = pers.[identification_number]
, @NombreChofer = convert(varchar(200),pers.[fullname_businessName])
from [dbo].[remissionguidetransportation] rgt
join [dbo].[Person] pers on rgt.[id_driver] = pers.[id]
where [id_remionGuide] = @id
--------------RECIBIDOR--------------------------------------------
--DECLARACION DE VARIABLE
declare @Idrec varchar(50)
declare @Nombrerec varchar(200)

select TOP 1 @Idrec = pers.[identification_number]
, @Nombrerec = convert(varchar(200),pers.[fullname_businessName])
from [dbo].[remissionguide] rg
join [dbo].[Person] pers on rg.[id_reciver] = pers.[id]
where rg.[id] = @id


select 
	--[prohi].[fullname_businessName] as [NombreProveedorHielo]
	rgcibi.[name_ProviderIceBags] as [NombreProveedorHielo]
	, [dorg].[emissionDate] as [FechaCompra] 
	, convert(char(50),[dorg].[sequential]) as [NumeroGuia]
	, rgcibi.[quantityIceBagsRequested] as [CantidadSolicitada]
	, veh.[carRegistration] as [PlacaVehiculo]
	, convert(char(50),do.[sequential]) as [NumeroOrdenCompraHielo]
	, [EtiquetaEmpresa] = (select top 1 [value] 
						from [dbo].[Setting]
						where [code] = 'EERSL')
	, epbdc.[id_company] as [IdCompania]
	, isnull(dorg.[description],'') as [DescripcionGuiaRemision]
	, @IdBiologo as [IdBiologo]
	, @NombreBiologo as [NombreBiologo]
	, @IdChofer as [IdChofer]
	, @NombreChofer as [NombreChofer]
	, @Idrec as [IdRecive]
	, @Nombrerec as [NombreRecive]
from [dbo].[RemissionGuideCustomizedIceBuyInformation] rgcibi
inner join [dbo].[RemissionGuide] rg on rgcibi.[id_RemissionGuide] = rg.[id]
inner join [dbo].[Document] do on rgcibi.[id_BuyIce] = do.[id]
inner join [dbo].[Document] dorg on rg.[id] = dorg.[id]
inner join [dbo].[vw_EmissionPointBranchDivisionCompany] epbdc on do.[id_emissionPoint] = epbdc.[id_emissionPoint]
inner join [dbo].[RemissionGuideTransportation] rgt on rg.[id] = rgt.[id_remionGuide]
inner join [dbo].[Vehicle] veh on rgt.[id_vehicle] = veh.[id]
--left outer join [dbo].[Person] prohi on rgcibi.[id_ProviderIceBags] = prohi.[id]
where rgcibi.[id_RemissionGuide] = @id




Go
