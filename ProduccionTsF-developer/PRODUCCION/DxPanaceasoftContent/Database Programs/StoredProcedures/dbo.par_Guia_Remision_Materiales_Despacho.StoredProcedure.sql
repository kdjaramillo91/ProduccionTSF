If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_Guia_Remision_Materiales_Despacho'
	)
Begin
	Drop Procedure dbo.par_Guia_Remision_Materiales_Despacho
End
Go
Create Procedure dbo.par_Guia_Remision_Materiales_Despacho
(
	@id_RemissionGuide int
)
As 
set nocount on

select 
it.[name] as NombreMatDesp
, rgdm.[sourceExitQuantity] as CantidadMatDesp
from [dbo].[RemissionGuideDispatchMaterial] rgdm
join [dbo].[Item] it on rgdm.[id_item] = it.[id]
where rgdm.[id_remisionGuide] = @id_RemissionGuide
and isnull(it.[notShowInReport],0) = 0
order by rgdm.[id]



-- [dbo].[par_Guia_Remision_Personalizada] 129274
Go
