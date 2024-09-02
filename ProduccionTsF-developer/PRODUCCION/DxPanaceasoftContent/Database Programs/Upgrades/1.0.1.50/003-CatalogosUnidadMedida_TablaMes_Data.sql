/* Script: Insertar Catalogos: Unidades de Medida, Tabla de Meses */

Declare	@tbsysCatalogue Table
(
	code		VarChar(50) Not Null,
	name		VarChar(50) Null,
	description	VarChar(150) Null,
	Primary Key (code)
)

Insert	Into @tbsysCatalogue (
		code,
		name,
		description)
	Values
		( 'LBLMU', 'ETIQUETAS UNIDAD MEDIDA', 'ETIQUETAS UNIDAD MEDIDA'),
		( 'MONTHT', 'TABLA DE MESES', 'TABLA DE MESES');

Insert	Into dbo.tbsysCatalogue (
		code,
		name,
		description,
		id_userCreate,
		dateCreate)
Select	T.code,
		T.name,
		T.description,
		1,
		GetDate()
From	@tbsysCatalogue T
		Left Outer Join dbo.tbsysCatalogue F
		On T.code = F.code
Where	F.id Is Null





Declare	@tbsysCatalogueDetail Table
(
	id_Catalogue	Int Not Null,
	code			VarChar(50) Not Null,
	name			VarChar(50) Null,
	description		VarChar(150) Null,
	fldvarchar1		VarChar(150) Null,
	Primary Key (code)
)

Insert	Into @tbsysCatalogueDetail (
		id_Catalogue,
		code,
		name,
		description,
		fldvarchar1)
	Values
		( (select top 1 id from [tbsysCatalogue] where code ='LBLMU'), 'Lbs', 'Libras', 'LBS', 'LB' ),
		( (select top 1 id from [tbsysCatalogue] where code ='LBLMU'), 'Kg', 'Kilogramos', 'KGS', 'KG' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '1', 'Enero', 'ENERO', 'JANUARY' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '2', 'Febrero', 'FEBRERO', 'FEBRUARY' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '3', 'Marzo', 'MARZO', 'MARCH' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '4', 'Abril', 'ABRIL', 'APRIL' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '5', 'Mayo', 'MAYO', 'MAY' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '6', 'Junio', 'JUNIO', 'JUNE' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '7', 'Julio', 'JULIO', 'JULY' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '8', 'Agosto', 'AGOSTO', 'AUGUST' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '9', 'Septiembre', 'SEPTIEMBRE', 'SEPTEMBER' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '10', 'Octubre', 'OCTUBRE', 'OCTOBER' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '11', 'Noviembre', 'NOVIEMBRE', 'NOVEMBER' ),
		( (select top 1 id from [tbsysCatalogue] where code ='MONTHT'), '12', 'Diciembre', 'DICIEMBRE', 'DECEMBER' );

Insert	Into dbo.tbsysCatalogueDetail (
		id_Catalogue,
		code,
		name,
		description,
		fldvarchar1,
		isActive,
		id_userCreate,
		dateCreate,
		id_userUpdate,
		dateUpdate)
Select	T.id_Catalogue,
		T.code,
		T.name,
		T.description,
		T.fldvarchar1,
		1,
		1,
		GetDate(),
		1,
		GetDate()
From	@tbsysCatalogueDetail T
		Left Outer Join dbo.tbsysCatalogueDetail F
		On T.code = F.code
Where	F.id Is Null

