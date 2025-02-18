If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'SP_mensaje_Error'
	)
Begin
	Drop Procedure dbo.SP_mensaje_Error
End
Go
Create Procedure dbo.SP_mensaje_Error
(
@sp varchar(100)
)
As
  Declare @ErrorMessage nvarchar(4000), @ErrorSeverity int, @ErrorState int 
  Select  @ErrorMessage  = 'SP(' +  @sp + ') : ' + ERROR_MESSAGE(),
		  @ErrorSeverity = ERROR_SEVERITY(),  
		  @ErrorState    = ERROR_STATE()
  IF @ErrorState = 0
	   SET @ErrorState=1
  RAISERROR(@ErrorMessage,@ErrorSeverity,@ErrorState)
Go
