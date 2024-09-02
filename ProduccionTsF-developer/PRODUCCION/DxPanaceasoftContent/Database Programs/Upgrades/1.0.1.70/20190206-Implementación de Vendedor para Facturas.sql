/*
 Script de Implementación de Vendedor para Facturas
 Fecha: 2019-02-06 --> Campos adicionales.
 */

If ColumnProperty(OBJECT_ID('dbo.Invoice'), 'id_seller', 'ColumnId') Is Null
Begin
	Alter Table dbo.Invoice Add id_seller Int Null
End
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_Invoice_PersonSeller') And parent_object_id = OBJECT_ID('dbo.Invoice'))
	Alter Table dbo.Invoice Add
		Constraint FK_Invoice_PersonSeller Foreign Key (id_seller) References dbo.Person(id)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Invoice' And object_id = OBJECT_ID('dbo.Invoice'))
	Create Index IX_Invoice On dbo.Invoice(id_InvoiceMode)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Invoice_1' And object_id = OBJECT_ID('dbo.Invoice'))
	Create Index IX_Invoice_1 On dbo.Invoice(id_InvoiceType)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Invoice_2' And object_id = OBJECT_ID('dbo.Invoice'))
	Create Index IX_Invoice_2 On dbo.Invoice(id_buyer)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Invoice_3' And object_id = OBJECT_ID('dbo.Invoice'))
	Create Index IX_Invoice_3 On dbo.Invoice(id_seller)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Invoice_4' And object_id = OBJECT_ID('dbo.Invoice'))
	Create Index IX_Invoice_4 On dbo.Invoice(id_remissionGuide)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Invoice_5' And object_id = OBJECT_ID('dbo.Invoice'))
	Create Index IX_Invoice_5 On dbo.Invoice(id_saleOrder)
Go
