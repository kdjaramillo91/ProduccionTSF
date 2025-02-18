If Exists(
	Select	*
	From	sys.objects
	Where	name = 'FUN_GetRoundTotal'
	And		type = 'FN'
	)
Begin
	Drop Function dbo.FUN_GetRoundTotal
End
Go
Create Function dbo.FUN_GetRoundTotal
(
  @opcion    char,
  @idInvoice int,
  @numDec	 int  
)
Returns Decimal(16,6)
As
BEGIN
Declare @Sentencia varchar(500);
DECLARE @subTotalRound Decimal(16,6);
DECLARE @subDiscountRound Decimal(16,6);
DECLARE @subIncoterms Decimal(16,6);
DECLARE @subFinaltotalRound Decimal(16,6);
DECLARE @totalRound Decimal(16,6);
  
  if  @opcion =  'S'  
  begin
	( Select  @totalRound = sum(round(totalPriceWithoutTax, @numDec)) from InvoiceDetail where id_invoice = @idInvoice and isActive=1 );
  end;

  if  @opcion =  'F'  
  begin

	( Select  @subTotalRound = sum(round(totalPriceWithoutTax, @numDec)) ,
	          @subDiscountRound = sum(round(discount, @numDec))
		from InvoiceDetail where id_invoice = @idInvoice and isActive=1
	);
	set @totalRound = @subTotalRound - @subDiscountRound;
  end;


  if  @opcion =  'T'  
  begin

	( Select  @subTotalRound = sum(round(totalPriceWithoutTax, @numDec)) ,
	          @subDiscountRound = sum(round(discount, @numDec))
		from InvoiceDetail where id_invoice = @idInvoice and isActive=1
	);
	set @subFinaltotalRound = ( @subTotalRound - @subDiscountRound);

	select 	@subIncoterms = (valueInternationalFreight+valueInternationalInsurance+
							 valueCustomsExpenditures+valueTransportationExpenses)
	from  invoiceexterior where id = @idInvoice; 

	set @totalRound = @subFinaltotalRound +@subIncoterms;
	--invoiceexterior
  end;

RETURN @totalRound 
end;
Go
