/*
 Script de Implementación y Configuración de Interfaces de Facturas Fiscales
 Fecha: 2019-02-21
 */


If ColumnProperty(OBJECT_ID('dbo.tbsysInvoiceType'), 'codigoExterno', 'ColumnId') Is Null
Begin
	Alter Table dbo.tbsysInvoiceType Add codigoExterno VarChar(5) Null
End
Go
Exec('Update dbo.tbsysInvoiceType Set codigoExterno = ''PL'' Where code = ''LOC'' And codigoExterno Is Null')
Exec('Update dbo.tbsysInvoiceType Set codigoExterno = ''PE'' Where code = ''EXT'' And codigoExterno Is Null')
Go


If ColumnProperty(OBJECT_ID('dbo.InvoiceExteriorPaymentTerm'), 'porcentaje', 'ColumnId') Is Null
Begin
	Alter Table dbo.InvoiceExteriorPaymentTerm Add porcentaje Decimal(5,2) Null
End
Go