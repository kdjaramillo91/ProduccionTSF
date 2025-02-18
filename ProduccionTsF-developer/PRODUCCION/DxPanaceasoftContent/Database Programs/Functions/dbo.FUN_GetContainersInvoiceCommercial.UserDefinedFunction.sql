If Exists(
	Select	*
	From	sys.objects
	Where	name = 'FUN_GetContainersInvoiceCommercial'
	And		type = 'FN'
	)
Begin
	Drop Function dbo.FUN_GetContainersInvoiceCommercial
End
Go
Create Function dbo.FUN_GetContainersInvoiceCommercial
(
@id Integer
)
Returns Varchar(1000)
As
BEGIN

DECLARE @strConcatenated VARCHAR(1000)
SELECT @strConcatenated= COALESCE(@strConcatenated + ', ', '') + numberContainer 
FROM InvoiceCommercialContainer
where id_invoiceCommercial = @id


RETURN RTRIM(LTRIM(@strConcatenated))

END

Go
