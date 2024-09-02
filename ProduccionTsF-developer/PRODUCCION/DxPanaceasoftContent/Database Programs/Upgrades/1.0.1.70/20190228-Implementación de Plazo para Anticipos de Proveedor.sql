/*
 Script de Implementación de Plazo para Anticipos de Proveedor
 Fecha: 2019-02-28 --> Campos adicionales.
 */

If ColumnProperty(OBJECT_ID('dbo.AdvanceProvider'), 'diasPlazo', 'ColumnId') Is Null
Begin
	Alter Table dbo.AdvanceProvider Add diasPlazo Int Null
End
Go
If Not Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.DEF_AdvanceProvider') And type = 'D')
Begin
	Alter Table dbo.AdvanceProvider
		Add Constraint DEF_AdvanceProvider Default 2 For diasPlazo
End
Go
-- Actualizar el valor predeterminado 2 para los que tienen NULL
Exec('Update dbo.AdvanceProvider Set diasPlazo = 2 Where diasPlazo Is Null')
Go

-- Agregar parámetros de valores mínimo y máximo para los días de plazo del Anticipo
If Not Exists(Select * From dbo.Setting Where code = 'MINDPANTPR')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'MINDPANTPR', 'Mínimo de Días Plazo para Anticipos a Proveedor', 'Mínimo de Días Plazo para Anticipos a Proveedor',
		'2', 2, 6, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
If Not Exists(Select * From dbo.Setting Where code = 'MAXDPANTPR')
Begin
	Insert Into dbo.Setting (
		code, name, description,
		value, id_settingDataType,  id_module, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'MAXDPANTPR', 'Máximo de Días Plazo para Anticipos a Proveedor', 'Máximo de Días Plazo para Anticipos a Proveedor',
		'7', 2, 6, 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
