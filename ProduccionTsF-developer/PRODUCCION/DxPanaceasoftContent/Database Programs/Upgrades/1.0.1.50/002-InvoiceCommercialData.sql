/*
	Script.Alter.InvoiceCommercial.Data
*/

-- Configuracion Seguridad Campos nuevos Factura Comercial
Declare	@tbsysDocumentDocumentStateControlsState Table
(
	id_documentType		Int Not Null,
	id_documentState	Int Not Null,
	controlName			VarChar(50) Not Null,
	messageError		VarChar(100) Null,
	isReadOnly			Bit Not Null,
	isRequired			Bit Not Null,
	isActive			Bit Not Null,
	Primary Key (id_documentType, id_documentState, controlName)
)

Insert	Into @tbsysDocumentDocumentStateControlsState (
		id_documentType,
		id_documentState,
		controlName,
		messageError,
		isReadOnly,
		isRequired,
		isActive)
	Values
		   ( ( select top 1 id from documentType where code = '70' )
		   , ( select top 1 id  from documentState where code = '01')
		   ,'idVendor'
		   ,'Vendedor'
		   ,0
		   ,0
		   ,1),
		   ( ( select top 1 id from documentType where code = '70' )
		   , ( select top 1 id  from documentState where code = '03')
		   ,'idVendor'
		   ,'Vendedor'
		   ,1
		   ,1
		   ,1),
		   ( ( select top 1 id from documentType where code = '70' )
		   , ( select top 1 id  from documentState where code = '05')
		   ,'idVendor'
		   ,'Vendedor'
		   ,1
		   ,1
		   ,1),
		-- etaDate
		( ( select top 1 id from documentType where code = '70' )
		   , ( select top 1 id  from documentState where code = '01')
		   ,'etaDate'
		   ,'Fecha de Arribo',0
		   ,0
		   ,1),
		   ( ( select top 1 id from documentType where code = '70' )
		   , ( select top 1 id  from documentState where code = '03')
		   ,'etaDate'
		   ,'Fecha de Arribo'
		   ,0
		   ,0
		   ,1),
		   ( ( select top 1 id from documentType where code = '70' )
		   , ( select top 1 id  from documentState where code = '05')
		   ,'etaDate'
		   ,'Fecha de Arribo'
		   ,1
		   ,1
		   ,1),
		   --- blDate
		 ( ( select top 1 id from documentType where code = '70' )
		   , ( select top 1 id  from documentState where code = '01')
		   ,'blDate'
		   ,'Fecha de BL'
		   ,0
		   ,0
		   ,1),
		   ( ( select top 1 id from documentType where code = '70' )
		   , ( select top 1 id  from documentState where code = '03')
		   ,'blDate'
		   ,'Fecha de BL'
		   ,0
		   ,0
		   ,1),
		   ( ( select top 1 id from documentType where code = '70' )
		   , ( select top 1 id  from documentState where code = '05')
		   ,'blDate'
		   ,'Fecha de BL'
		   ,1
		   ,1
		   ,1);

Insert	Into dbo.tbsysDocumentDocumentStateControlsState (
		id_documentType,
		id_documentState,
		controlName,
		messageError,
		isReadOnly,
		isRequired,
		isActive)
Select	T.id_documentType,
		T.id_documentState,
		T.controlName,
		T.messageError,
		T.isReadOnly,
		T.isRequired,
		T.isActive
From	@tbsysDocumentDocumentStateControlsState T
		Left Outer Join dbo.tbsysDocumentDocumentStateControlsState F
		On T.id_documentType = F.id_documentType And T.id_documentState = F.id_documentState And T.controlName = F.controlName
Where	F.id_documentType Is Null
