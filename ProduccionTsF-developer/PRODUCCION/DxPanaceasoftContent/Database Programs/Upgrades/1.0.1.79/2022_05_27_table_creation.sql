If Not Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.SimpleFormula') And type = 'U')
begin
create table "dbo"."SimpleFormula"(
	"id" [int] IDENTITY(1,1) NOT NULL,
	"code" [varchar](20) NOT NULL,
	"name" [varchar](250) NOT NULL,
	"description" [varchar](max) NULL,
	"type" [varchar](15) NOT NULL,
	"dataSources" [varchar](max) NOT NULL,
	"formula" [varchar](max) NULL,
	"formulaTranslated" [varchar](max) NULL,
	"id_company" [int] NOT NULL,
	"isActive" [bit] NOT NULL,
	"id_userCreate" [int] NOT NULL,
	"dateCreate" [datetime] NOT NULL,
	"id_userUpdate" [int] NOT NULL,
	"dateUpdate" [datetime] NOT NULL,
	Constraint PK_SimpleFormula Primary Key ( id )
)
end
go
If ColumnProperty(OBJECT_ID('dbo.WarehouseType'), 'productionCosting', 'ColumnId') Is Null
Begin
	Alter Table "dbo"."WarehouseType" Add "productionCosting" VARCHAR(2) DEFAULT('NO')
End
go
If ColumnProperty(OBJECT_ID('dbo.WarehouseType'), 'poundsType', 'ColumnId') Is Null
Begin
	Alter Table "dbo"."WarehouseType" Add "poundsType" VARCHAR(10) DEFAULT('AMBAS')
End
go
If ColumnProperty(OBJECT_ID('dbo.WarehouseType'), 'idProcessedPoundsSimpleFormula', 'ColumnId') Is Null
Begin
	Alter Table "dbo"."WarehouseType" Add "idProcessedPoundsSimpleFormula" int
End
go
If ColumnProperty(OBJECT_ID('dbo.WarehouseType'), 'idFinishedPoundsSimpleFormula', 'ColumnId') Is Null
Begin
	Alter Table "dbo"."WarehouseType" Add "idFinishedPoundsSimpleFormula" int
End
go
If ColumnProperty(OBJECT_ID('dbo.WarehouseType'), 'reasonCosts', 'ColumnId') Is Null
Begin
	Alter Table "dbo"."WarehouseType" Add "reasonCosts" varchar(max)
End
go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_WarehouseType_SimpleFormula1') And parent_object_id = OBJECT_ID('dbo.WarehouseType'))
	Alter Table "dbo"."WarehouseType" Add
		Constraint FK_WarehouseType_SimpleFormula1 Foreign Key (idProcessedPoundsSimpleFormula) References dbo.SimpleFormula(id)
Go
If Not Exists (Select * From sys.foreign_keys Where object_id = OBJECT_ID('dbo.FK_WarehouseType_SimpleFormula2') And parent_object_id = OBJECT_ID('dbo.WarehouseType'))
	Alter Table "dbo"."WarehouseType" Add
		Constraint FK_WarehouseType_SimpleFormula2 Foreign Key (idFinishedPoundsSimpleFormula) References dbo.SimpleFormula(id)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_SimpleFormula' And object_id = OBJECT_ID('dbo.SimpleFormula'))
	Create Index IX_SimpleFormula On dbo.SimpleFormula(id)
Go