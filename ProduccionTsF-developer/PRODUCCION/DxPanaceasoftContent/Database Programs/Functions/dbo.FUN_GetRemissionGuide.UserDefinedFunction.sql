If Exists(
	Select	*
	From	sys.objects
	Where	name = 'FUN_GetRemissionGuide'
	And		type = 'FN'
	)
Begin
	Drop Function dbo.FUN_GetRemissionGuide
End
Go
Create Function dbo.FUN_GetRemissionGuide
(
@id Integer
)
Returns Varchar(1000)
As
BEGIN

DECLARE @strConcatenated VARCHAR(1000)

select @strConcatenated = COALESCE(@strConcatenated + ', ', '') + convert(varchar,do.sequential)
from [dbo].[ProductionLotDetailPurchaseDetail] pldpd
inner join [dbo].[ProductionLotDetail] pld on pldpd.[id_productionLotDetail] = pld.[id]
inner join [dbo].[ProductionLot] pl on pld.[id_productionLot] = pl.[id]
inner join [dbo].[RemissionGuideDetail] rgd on rgd.[id] = pldpd.[id_remissionGuideDetail]
inner join [dbo].[Document] do on do.[id] = rgd.[id_remisionGuide]
where pl.[id] = @id
order by [sequential]

RETURN RTRIM(LTRIM(@strConcatenated))

END
Go
