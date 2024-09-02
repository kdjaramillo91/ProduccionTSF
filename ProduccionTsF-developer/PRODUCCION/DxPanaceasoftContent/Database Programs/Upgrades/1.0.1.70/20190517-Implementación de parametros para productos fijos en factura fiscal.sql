/*
 Script de Implementaci�n de parametros para Productos fijos
 Fecha: 2019-05-17
 */

 -- Agregar par�metro para poner el codigo del producto del Flete Internacional
If Not Exists(Select * From dbo.Setting Where code = 'CODFLEI')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'CODFLEI', 'C�digo del producto para Flete Internacional', 'C�digo del producto para Flete Internacional',
		'4017', 2, 2, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
 -- Agregar par�metro para poner el codigo del producto del Seguro Internacional
If Not Exists(Select * From dbo.Setting Where code = 'CODSEGI')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'CODSEGI', 'C�digo del producto para Seguro Internacional', 'C�digo del producto para Seguro Internacional',
		'0', 2, 2, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
 -- Agregar par�metro para poner el codigo del producto de Gastos Aduaneros
If Not Exists(Select * From dbo.Setting Where code = 'CODGADU')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'CODGADU', 'C�digo del producto para Gastos Aduaneros', 'C�digo del producto para Gastos Aduaneros',
		'0', 2, 2, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
 -- Agregar par�metro para poner el codigo del producto de Gastos de Transporte
If Not Exists(Select * From dbo.Setting Where code = 'CODGTRA')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'CODGTRA', 'C�digo del producto para Gastos Transp/Otros', 'C�digo del producto para Gastos Transp/Otros',
		'0', 2, 2, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End