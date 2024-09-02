SET NOCOUNT ON 
GO

CREATE OR ALTER procedure [dbo].[spPar_GuiaRemisionMatDesp]
	@id_RemissionGuide int
as 
set nocount on

select 
it.[name] as NombreMatDesp
, rgdm.[sourceExitQuantity] as CantidadMatDesp
from [dbo].[RemissionGuideDispatchMaterial] rgdm
join [dbo].[Item] it on rgdm.[id_item] = it.[id]
where rgdm.[id_remisionGuide] = @id_RemissionGuide
and isnull(it.[notShowInReport],0) = 0
And it.masterCode not in ('MI000331')
order by rgdm.[id]
-- [dbo].[par_Guia_Remision_Personalizada] 129274