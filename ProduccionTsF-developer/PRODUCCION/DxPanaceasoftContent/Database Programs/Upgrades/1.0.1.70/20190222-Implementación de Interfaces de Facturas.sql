/*
 Script de Implementación y Configuración de Interfaces de Facturas Fiscales
 Fecha: 2019-02-22
 */

 -- Agregar la definición del proceso de integración
 If Not Exists(Select * From dbo.tbsysIntegrationDocumentConfig Where code = 'FAF')
 Begin
	Insert Into dbo.tbsysIntegrationDocumentConfig (
		code, codeDocumentType, secuence, implementingIntegration,
		description, dateInitFindDocument, isGroupValue, reprocess, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'FAF', '07', 0, 'DXPANACEASOFT.Auxiliares.IntegrationProcessService.IntegrationProcessActionInvoiceFiscal',
		'Factura Fiscal', '2018-02-01', 0, 1, 1,
		1, GetDate(), 1, GetDate()
	)
End

Declare	@integrationDocumentConfig Int
Select	@integrationDocumentConfig = id
From	dbo.tbsysIntegrationDocumentConfig
Where	code = 'FAF'


-- Agregar la definición de los campos a utilizar para la validación
If Not Exists(Select * From dbo.tbsysValidateEntityDefinition Where code = 'DOC')
Begin
	Insert Into dbo.tbsysValidateEntityDefinition (
		code, entityValidate, fieldValidate, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'DOC', 'Document', 'id_documentState', 1,
		1, GetDate(), 1, GetDate()
	)
End

Declare	@validateEntityDocState Int
Select	@validateEntityDocState = id
From	dbo.tbsysValidateEntityDefinition
Where	code = 'DOC'


If Not Exists(Select * From dbo.tbsysValidateEntityDefinition Where code = 'FESTA')
Begin
	Insert Into dbo.tbsysValidateEntityDefinition (
		code, entityValidate, fieldValidate, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'FESTA', 'ElectronicDocument', 'id_electronicDocumentState', 1,
		1, GetDate(), 1, GetDate()
	)
End

Declare	@validateEntityFacElectState Int
Select	@validateEntityFacElectState = id
From	dbo.tbsysValidateEntityDefinition
Where	code = 'FESTA'


-- Agregar la definición de las validaciones
If Not Exists(Select * From dbo.tbsysValidateConfig Where code = 'VDOC')
Begin
	Insert Into dbo.tbsysValidateConfig (
		code, id_ValidateEntity, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'VDOC', @validateEntityDocState, 1,
		1, GetDate(), 1, GetDate()
	)
End

Declare	@validateDocState Int
Select	@validateDocState = id
From	dbo.tbsysValidateConfig
Where	code = 'VDOC'


If Not Exists(Select * From dbo.tbsysValidateConfig Where code = 'VFEST')
Begin
	Insert Into dbo.tbsysValidateConfig (
		code, id_ValidateEntity, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'VFEST', @validateEntityFacElectState, 1,
		1, GetDate(), 1, GetDate()
	)
End

Declare	@validateFacElectState Int
Select	@validateFacElectState = id
From	dbo.tbsysValidateConfig
Where	code = 'VFEST'


-- Agregar las validaciones para la factura fiscal
If Not Exists(Select * From dbo.tbsysIntegrationDocumentValidationConfig Where code = 'VFAF1')
Begin
	Insert Into dbo.tbsysIntegrationDocumentValidationConfig (
		code, id_IntegrationDocumentConfig, id_ValidateConfig, orderValidate, valueDirectValidate, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'VFAF1', @integrationDocumentConfig, @validateDocState, 1, '06', 1,
		1, GetDate(), 1, GetDate()
	)
End

If Not Exists(Select * From dbo.tbsysIntegrationDocumentValidationConfig Where code = 'VFAF2')
Begin
	Insert Into dbo.tbsysIntegrationDocumentValidationConfig (
		code, id_IntegrationDocumentConfig, id_ValidateConfig, orderValidate, valueDirectValidate, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		'VFAF2', @integrationDocumentConfig, @validateFacElectState, 2, '03', 1,
		1, GetDate(), 1, GetDate()
	)
End
