/*
 Script de Implementación de ROMANEO
 Fecha: 2019-01-22
 */

If Not Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.ResultProdLotRomaneo') And type = 'U')
Begin
	Create Table dbo.ResultProdLotRomaneo
	(
		idProductionLot		Int				Not Null,
		numberLot			VarChar(20)		Not Null,
		numberLotSequential	VarChar(20)		Not Null,
		nameProvider		VarChar(250)	Not Null,
		nameProviderShrimp	VarChar(250)	Not Null,
		namePool			VarChar(50)		Not Null,
		INPnumber			VarChar(20)		Not Null,
		dateTimeReception	DateTime		Not Null,
		nameItem			VarChar(200)	Not Null,
		nameWarehouseItem	VarChar(20)		Not Null,
		nameWarehouseLocationItem VarChar(20) Not Null,
		codeProcessType		VarChar(20)		Not Null,
		quantityRemitted	Decimal(18,6)	Not Null,
		idMetricUnit		Int				Not Null,
		gavetaNumber		Int				Not Null,
		Constraint PK_ResultProdLotRomaneo Primary Key ( idProductionLot )
	)
End
Go

If Not Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.Romaneo') And type = 'U')
Begin
	Create Table dbo.Romaneo
	(
		id						Int		Not Null,
		idResultProdLotRomaneo	Int		Not Null,
		idWeigher				Int		Not Null,
		idAnalist				Int		Not Null,
		gavetaNumber			Int		Not Null,
		idRomaneoType			Int		Not Null,
		idItemSize				Int		Null,
		PoundsGarbage			Decimal(14,6)	Not Null,
		TotalPoundsWeigthGross	Decimal(14,6)	Not Null,
		TotalPoundsWeigthNet	Decimal(14,6)	Not Null,
		porcTara				Decimal(8,2)	Not Null,
		Constraint PK_Romaneo Primary Key ( id )
	)
End
Go

If Not Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.RomaneoDetail') And type = 'U')
Begin
	Create Table dbo.RomaneoDetail
	(
		id				Int Identity(1,1)	Not Null,
		idRomaneo		Int		Not Null,
		orderDetail		Int		Not Null,
		grossWeight		Int		Not Null,
		idMetricUnit	Int		Not Null,
		poundsGarbage	Int		Not Null,
		Constraint PK_RomaneoDetail Primary Key ( id )
	)
End
Go


If Not Exists (Select * From sys.indexes where name = 'IX_ResultProdLotRomaneo' And object_id = OBJECT_ID('dbo.ResultProdLotRomaneo'))
	Create Index IX_ResultProdLotRomaneo On dbo.ResultProdLotRomaneo (idMetricUnit)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Romaneo' And object_id = OBJECT_ID('dbo.Romaneo'))
	Create Index IX_Romaneo On dbo.Romaneo(idResultProdLotRomaneo)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Romaneo_1' And object_id = OBJECT_ID('dbo.Romaneo'))
	Create Index IX_Romaneo_1 On dbo.Romaneo(idWeigher)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Romaneo_2' And object_id = OBJECT_ID('dbo.Romaneo'))
	Create Index IX_Romaneo_2 On dbo.Romaneo(idAnalist)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Romaneo_3' And object_id = OBJECT_ID('dbo.Romaneo'))
	Create Index IX_Romaneo_3 On dbo.Romaneo(idRomaneoType)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_Romaneo_4' And object_id = OBJECT_ID('dbo.Romaneo'))
	Create Index IX_Romaneo_4 On dbo.Romaneo(idItemSize)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_RomaneoDetail' And object_id = OBJECT_ID('dbo.RomaneoDetail'))
	Create Index IX_RomaneoDetail On dbo.RomaneoDetail(idRomaneo)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_RomaneoDetail_1' And object_id = OBJECT_ID('dbo.RomaneoDetail'))
	Create Index IX_RomaneoDetail_1 On dbo.RomaneoDetail(idMetricUnit)
Go


If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_ResultProdLotRomaneo_MetricUnit') And parent_object_id = OBJECT_ID('dbo.ResultProdLotRomaneo'))
	Alter Table dbo.ResultProdLotRomaneo Add
		Constraint FK_ResultProdLotRomaneo_MetricUnit Foreign Key (idMetricUnit) References dbo.MetricUnit(id)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_ResultProdLotRomaneo_ProductionLot') And parent_object_id = OBJECT_ID('dbo.ResultProdLotRomaneo'))
	Alter Table dbo.ResultProdLotRomaneo Add
		Constraint FK_ResultProdLotRomaneo_ProductionLot Foreign Key (idProductionLot) References dbo.ProductionLot(id)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_Romaneo_Analist') And parent_object_id = OBJECT_ID('dbo.Romaneo'))
	Alter Table dbo.Romaneo Add
		Constraint FK_Romaneo_Analist Foreign Key (idAnalist) References dbo.Employee(id)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_Romaneo_Document') And parent_object_id = OBJECT_ID('dbo.Romaneo'))
	Alter Table dbo.Romaneo Add
		Constraint FK_Romaneo_Document Foreign Key (id) References dbo.Document(id)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_Romaneo_ItemSize') And parent_object_id = OBJECT_ID('dbo.Romaneo'))
	Alter Table dbo.Romaneo Add
		Constraint FK_Romaneo_ItemSize Foreign Key (idItemSize) References dbo.ItemSize(id)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_Romaneo_ResultProdLotRomaneo') And parent_object_id = OBJECT_ID('dbo.Romaneo'))
	Alter Table dbo.Romaneo Add
		Constraint FK_Romaneo_ResultProdLotRomaneo Foreign Key (idResultProdLotRomaneo) References dbo.ResultProdLotRomaneo(idProductionLot)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_Romaneo_tbsysCatalogueDetail') And parent_object_id = OBJECT_ID('dbo.Romaneo'))
	Alter Table dbo.Romaneo Add
		Constraint FK_Romaneo_tbsysCatalogueDetail Foreign Key (idRomaneoType) References dbo.tbsysCatalogueDetail(id)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_Romaneo_Weigher') And parent_object_id = OBJECT_ID('dbo.Romaneo'))
	Alter Table dbo.Romaneo Add
		Constraint FK_Romaneo_Weigher Foreign Key (idWeigher) References dbo.Employee(id)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_RomaneoDetail_MetricUnit') And parent_object_id = OBJECT_ID('dbo.RomaneoDetail'))
	Alter Table dbo.RomaneoDetail Add
		Constraint FK_RomaneoDetail_MetricUnit Foreign Key (idMetricUnit) References dbo.MetricUnit(id)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_RomaneoDetail_Romaneo') And parent_object_id = OBJECT_ID('dbo.RomaneoDetail'))
	Alter Table dbo.RomaneoDetail Add
		Constraint FK_RomaneoDetail_Romaneo Foreign Key (idRomaneo) References dbo.Romaneo(id)
Go

/* Creación del tipo de documento... */

Declare	@codTipoDocRomaneo Varchar(5),
		@nomTipoDocRomaneo Varchar(25)
Set @codTipoDocRomaneo = '106'
Set @nomTipoDocRomaneo = 'Romaneo'

If Not Exists(Select * From dbo.DocumentType Where code = @codTipoDocRomaneo And name = @nomTipoDocRomaneo)
Begin
	If Exists(Select * From dbo.DocumentType Where code = @codTipoDocRomaneo)
	Begin
		RaisError('Ya existe otro documento diferente a ROMANEO con el mismo código', 15, 1)
		Return
	End

	Insert Into dbo.DocumentType
	(
		code, name, description, currentNumber,
		daysToExpiration, isElectronic, codeSRI, id_company, isActive,
		id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		@codTipoDocRomaneo, @nomTipoDocRomaneo, @nomTipoDocRomaneo, 0,
		0, 0, '', 2, 1,
		1, GetDate(), 1, GetDate()
	)
End
Go

/* Creación del catálogo de tipo de romaneo... */

Declare	@codTipoRomaneo Varchar(5),
		@idTipoRomaneo Int
Set @codTipoRomaneo = 'MPLTR'

Select	Top 1 @idTipoRomaneo = id
From	dbo.tbsysCatalogue
Where	code = 'MPLTR'

If @@RowCount = 0
Begin
	Insert Into dbo.tbsysCatalogue
	(
		code, name, description, id_userCreate, dateCreate
	) Values (
		@codTipoRomaneo, 'Tipo de Romaneo', 'Tipo de Romaneo', 1, GetDate()
	)
End

Select	Top 1 @idTipoRomaneo = id
From	dbo.tbsysCatalogue
Where	code = 'MPLTR'

If @@RowCount = 0
Begin
	RaisError('No se ha podido crear el catálogo de Tipo de Romaneo', 15, 1)
	Return
End


If Not Exists(Select * From dbo.tbsysCatalogueDetail Where id_Catalogue = @idTipoRomaneo And code = 'ENT')
Begin
	Insert Into dbo.tbsysCatalogueDetail
	(
		id_Catalogue, code, name, description, isActive, id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		@idTipoRomaneo, 'ENT', 'ENTERO', 'ENTERO', 1, 1, GetDate(), 1, GetDate()
	)
End
If Not Exists(Select * From dbo.tbsysCatalogueDetail Where id_Catalogue = @idTipoRomaneo And code = 'COL')
Begin
	Insert Into dbo.tbsysCatalogueDetail
	(
		id_Catalogue, code, name, description, isActive, id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		@idTipoRomaneo, 'COL', 'COLA', 'COLA', 1, 1, GetDate(), 1, GetDate()
	)
End
If Not Exists(Select * From dbo.tbsysCatalogueDetail Where id_Catalogue = @idTipoRomaneo And code = 'REC')
Begin
	Insert Into dbo.tbsysCatalogueDetail
	(
		id_Catalogue, code, name, description, isActive, id_userCreate, dateCreate, id_userUpdate, dateUpdate
	) Values (
		@idTipoRomaneo, 'REC', 'RECHAZO', 'RECHAZO', 1, 1, GetDate(), 1, GetDate()
	)
End
Go
