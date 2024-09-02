/*
 Script de Implementación de la Integración con Romaneo
 Fecha: 2019-03-07
 */

 -- Agregar parámetro para habilitar el proceso de Romaneo
If Not Exists(Select * From dbo.Setting Where code = 'ROMANEO')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'ROMANEO', 'Habilita el proceso de Romaneo', 'Habilita el proceso de Romaneo',
		'1', 2, 6, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End

-- Verificar el tamaño de los campos de dbo.ResultProdLotRomaneo
If ColumnProperty(Object_ID('dbo.ResultProdLotRomaneo'), 'nameWarehouseItem', 'PRECISION') < 50
Begin
	Alter Table dbo.ResultProdLotRomaneo Alter Column nameWarehouseItem Varchar(50) Not Null
End
If ColumnProperty(Object_ID('dbo.ResultProdLotRomaneo'), 'nameWarehouseLocationItem', 'PRECISION') < 250
Begin
	Alter Table dbo.ResultProdLotRomaneo Alter Column nameWarehouseLocationItem Varchar(250) Not Null
End
