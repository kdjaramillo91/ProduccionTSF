If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_GetpPurchaseOrdervalueletter'
	)
Begin
	Drop Procedure dbo.par_GetpPurchaseOrdervalueletter
End
Go
Create Procedure dbo.par_GetpPurchaseOrdervalueletter
(
	@id int
)
As
declare @valor Decimal(19,4)
declare @valorletra varchar
select @valor = total from PurchaseOrderTotal
				where id_purcharseOrder = @id --129274
		select 	@id, dbo.FUN_CantidadConLetraCastellano(@valor)+ ' DOLARES'
Go
