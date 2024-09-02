/*
	Script.Alter.InvoiceCommercial.Model
*/

-- Add Field idVendor
If	ColumnProperty(Object_ID('InvoiceCommercial'), 'idVendor', 'ColumnId') Is Null
Begin
	Alter Table InvoiceCommercial Add idVendor Int Null
End

If	Not Exists(
		Select	*
		From	sys.foreign_keys
		Where	name = 'FK_InvoiceCommercial_Vendedor'
		And		parent_object_id = Object_ID('InvoiceCommercial')
	)
Begin
	Alter Table InvoiceCommercial
	Add Constraint FK_InvoiceCommercial_Vendedor Foreign Key (idVendor)
	References Person(id)
End

-- Add Field etaDate
If	ColumnProperty(Object_ID('InvoiceCommercial'), 'etaDate', 'ColumnId') Is Null
Begin
	Alter Table InvoiceCommercial Add etaDate DateTime Null
End

-- Add Field blDate
If	ColumnProperty(Object_ID('InvoiceCommercial'), 'blDate', 'ColumnId') Is Null
Begin
	Alter Table InvoiceCommercial Add blDate DateTime Null
End


-- Tabla de Plazos Factura Comercial
If	Not Exists(
		Select	*
		From	sys.tables
		Where	name = 'InvoiceCommercialPaymentTerm'
	)
Begin
	Create Table InvoiceCommercialPaymentTerm
	(
		id				Int Identity Not Null,
		idInvoiceCommercial Int  Not Null,
		orderPayment		Int Not Null Constraint DF_ICPT_orderPayment Default(0),
		valuePayment		Decimal(16,6) Not Null Constraint DF_ICPT_valuePayment Default(0),
		dueDate			Date Not Null,	
		Constraint		PK_InvoiceCommercialPayment Primary Key (id)
	)
End

If	Not Exists(
		Select	*
		From	sys.foreign_keys
		Where	name = 'FK_InvoiceCommercialPaymentTerm_InvoiceExterior'
		And		parent_object_id = Object_ID('InvoiceCommercialPaymentTerm')
	)
Begin
	Alter Table InvoiceCommercialPaymentTerm
	Add Constraint FK_InvoiceCommercialPaymentTerm_InvoiceExterior Foreign Key (idInvoiceCommercial)
	References InvoiceCommercial(id)
End
