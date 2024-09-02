SET NOCOUNT ON
GO

CREATE OR ALTER PROC [dbo].[spPar_GuiaRemisionMatDespFluvial]
	@id_RemissionGuideRiver int
as 
set nocount on

select 
it.[name] as NombreMatDesp
, rgdm.[sourceExitQuantity] as CantidadMatDesp
from [dbo].[RemissionGuideRiverDispatchMaterial] rgdm
join [dbo].[Item] it on rgdm.[id_item] = it.[id]
where rgdm.[id_remisionGuideRiver] = @id_RemissionGuideRiver
and isnull(it.[notShowInReport],0) = 0
order by rgdm.[id]


/*
	[dbo].[par_Guia_Remision_Fluvial_Materiales_Despacho] 128293
*/
