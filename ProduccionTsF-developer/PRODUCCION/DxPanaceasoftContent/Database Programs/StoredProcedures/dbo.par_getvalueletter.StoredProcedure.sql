If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_getvalueletter'
	)
Begin
	Drop Procedure dbo.par_getvalueletter
End
Go
Create Procedure dbo.par_getvalueletter
(
@id int
)
As
declare @valor decimal
declare @valorletra varchar
select @valor = SUM(RGAS.viaticPrice) from RemissionGuide RG
				inner join RemissionGuideAssignedStaff RGAS
				on RG.id = RGAS.id_remissionGuide 
				where rg.id = @id --129274
		select 	 dbo.FUN_CantidadConLetraCastellano(@valor)+ ' DOLARES'
		
--execute par_getvalueletter 129274

Go
