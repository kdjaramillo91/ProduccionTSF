/*
 Script de Implementación de parametros para Productos fijos
 Fecha: 2019-05-17
 */

 -- Agregar parámetro para poner el codigo del producto del Flete Internacional
If Not Exists(Select * From dbo.Setting Where code = 'CODFLEI')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'CODFLEI', 'Código del producto para Flete Internacional', 'Código del producto para Flete Internacional',
		'4017', 2, 2, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
 -- Agregar parámetro para poner el codigo del producto del Seguro Internacional
If Not Exists(Select * From dbo.Setting Where code = 'CODSEGI')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'CODSEGI', 'Código del producto para Seguro Internacional', 'Código del producto para Seguro Internacional',
		'0', 2, 2, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
 -- Agregar parámetro para poner el codigo del producto de Gastos Aduaneros
If Not Exists(Select * From dbo.Setting Where code = 'CODGADU')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'CODGADU', 'Código del producto para Gastos Aduaneros', 'Código del producto para Gastos Aduaneros',
		'0', 2, 2, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
 -- Agregar parámetro para poner el codigo del producto de Gastos de Transporte
If Not Exists(Select * From dbo.Setting Where code = 'CODGTRA')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'CODGTRA', 'Código del producto para Gastos Transp/Otros', 'Código del producto para Gastos Transp/Otros',
		'0', 2, 2, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End