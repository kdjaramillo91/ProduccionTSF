/*
 Script de Implementación y Configuración de Envío de Análisis de Calidad
 Fecha: 2019-03-27
 */
 
If ColumnProperty(OBJECT_ID('dbo.QualityControl'), 'hasSentEmail', 'ColumnId') Is Null
Begin
	Alter Table dbo.QualityControl Add hasSentEmail Bit Null
End
Go

If Not Exists (Select * From sys.foreign_keys Where name = 'FK_InvoiceCommercial_Vendedor' And parent_object_id = OBJECT_ID('dbo.InvoiceCommercial'))
Begin
	Alter Table dbo.InvoiceCommercial
		Add Constraint FK_InvoiceCommercial_Vendedor Foreign Key (idVendor) References dbo.Person (id)
End
Go

If Not Exists (Select * From sys.foreign_keys Where name = 'FK_InvoiceExterior_Vendedor' And parent_object_id = OBJECT_ID('dbo.InvoiceExterior'))
Begin
	Alter Table dbo.InvoiceExterior
		Add Constraint FK_InvoiceExterior_Vendedor Foreign Key (idVendor) References dbo.Person (id)
End
Go

 -- Agregar parámetro con direcciones de correo para Análisis de Calidad
If Not Exists(Select * From dbo.Setting Where code = 'LDANACAL')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'LDANACAL', 'Lista de Distribución para Análisis de Calidad', 'Lista de Distribución para correos electrónicos de Análisis de Calidad',
		'lastudillo@panaceasoft.ec;yreyes@panaceasoft.ec', 2, 6, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End

