/*
 Script de Implementación de validación de Anticipo a Proveedores
 Fecha: 2019-03-07
 */

 -- Agregar parámetro para habilitar el poder modificar el valor del anticipo
If Not Exists(Select * From dbo.Setting Where code = 'MODANT')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'MODANT', 'Permite modificar el valor del anticipo', 'Permite modificar el valor del anticipo',
		'1', 2, 6, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
