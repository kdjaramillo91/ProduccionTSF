/****************
	DESARROLLO DE PROCESO: DISTRIBUCIÓN DE COSTOS
 -------------------------------------------------------
	Autor: Pedro Luna
 *******************************************************/

Set NoCount On

-- 0. Eliminación de tablas del diseño anterior
/*
If Object_ID('[dbo].[CostsPerPeriodForProductionCoefficient]') Is Not Null
Begin
	Drop Table [dbo].[CostsPerPeriodForProductionCoefficient];
End
If Object_ID('[dbo].[WarehouseExpenseAccountingTemplate]') Is Not Null
Begin
	Drop Table [dbo].[WarehouseExpenseAccountingTemplate];
End
If Object_ID('[dbo].[AccountLedger]') Is Not Null
Begin
	Drop Table [dbo].[AccountLedger];
End
If Object_ID('[dbo].[AccountingTemplate]') Is Not Null
Begin
	Drop Table [dbo].[AccountingTemplate];
End
If Object_ID('[dbo].[ProductionCostsBuysDetail]') Is Not Null
Begin
	Drop Table [dbo].[ProductionCostsBuysDetail];
End
If Object_ID('[dbo].[ProductionCostsBuys]') Is Not Null
Begin
	Drop Table [dbo].[ProductionCostsBuys];
End
If Object_ID('[dbo].[ProductionCostsProcessDetail]') Is Not Null
Begin
	Drop Table [dbo].[ProductionCostsProcessDetail];
End
If Object_ID('[dbo].[ProductionCostBuysDistributionGroup]') Is Not Null
Begin
	Drop Table [dbo].[ProductionCostBuysDistributionGroup];
End
If Object_ID('[dbo].[ProductionCostBuysDistribution]') Is Not Null
Begin
	Drop Table [dbo].[ProductionCostBuysDistribution];
End
If Object_ID('[dbo].[ProductionCostsProcessGrouped]') Is Not Null
Begin
	Drop Table [dbo].[ProductionCostsProcessGrouped];
End
If Object_ID('[dbo].[ProductionExpense]') Is Not Null
Begin
	Drop Table [dbo].[ProductionExpense];
End
If Object_ID('[dbo].[ProductionCosts]') Is Not Null
Begin
	Drop Table [dbo].[ProductionCosts];
End
If Object_ID('[dbo].[ProductionCoefficient]') Is Not Null
Begin
	Drop Table [dbo].[ProductionCoefficient];
End
*/

-- 1. Preparación de la tabla [dbo].[ProductionCostPoundType]
If Object_ID('[dbo].[ProductionCostPoundType]') Is Null
Begin
	Create Table [dbo].[ProductionCostPoundType]
	(
		[id]			Int Identity	Not Null,
		[code]			VarChar(10)		Not Null,
		[name]			VarChar(50)		Not Null,
		[order]			Int				Not Null,
		[description]	VarChar(Max)	Null,
		[isActive]		Bit				Not Null,
		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostPoundType] Primary Key ( [id] ),
		Constraint [UQ_ProductionCostPoundType] Unique ( [code] )
	);
End;

-- Carga de valores iniciales
Declare	@ProductionCostPoundTypeTemp Table (
	[id]	Int Identity	Primary Key,
	[code]	VarChar(10),
	[name]	VarChar(50),
	[order]	Int
);

Insert	Into @ProductionCostPoundTypeTemp ( [code], [name], [order] )
Values	( 'LBREMIT', 'Libras Remitidas', 1 ),
		( 'LBPROCE', 'Libras Procesadas', 2 ),
		( 'LBTERMI', 'Libras Terminadas', 3 ),
		( 'LBAMBAS', 'Ambas (Libras Procesadas y Terminadas)', 4 ),
		( 'NINGUNA', 'Ninguna', 5 );

Insert Into [dbo].[ProductionCostPoundType]
	(
		[code],
		[name],
		[order],
		[isActive],
		id_userCreate,
		dateCreate,
		id_userUpdate,
		dateUpdate
	)
Select	[code],
		[name],
		[order],
		1,
		1,
		GetDate(),
		1,
		GetDate()
From	@ProductionCostPoundTypeTemp
Where	code Not In ( Select code From [dbo].[ProductionCostPoundType] )
Order	By id;


-- 2. Adición de campo [enableProductionCost] para tabla [dbo].[Warehouse]
If ColumnProperty(Object_ID('[dbo].[Warehouse]'), 'enableProductionCost', 'ColumnId') Is Null
Begin
	Alter Table [dbo].[Warehouse]
	Add [enableProductionCost] Bit Not Null Default (0);
End


-- 3. Adición de campo [id_productionCostPoundType] para tabla [dbo].[Warehouse]
If ColumnProperty(Object_ID('[dbo].[Warehouse]'), 'id_productionCostPoundType', 'ColumnId') Is Null
Begin
	Alter Table [dbo].[Warehouse]
	Add [id_productionCostPoundType] Int Null;
End

If IndexProperty(Object_ID('[dbo].[Warehouse]'), 'IX_Warehouse_ProductionCostPoundType', 'IndexID') Is Null
Begin
	Create Index [IX_Warehouse_ProductionCostPoundType]
		On [dbo].[Warehouse] ( [id_productionCostPoundType] );
End

If Object_ID('[dbo].[FK_Warehouse_ProductionCostPoundType]') Is Null
Begin
	Alter Table [dbo].[Warehouse]
	Add Constraint [FK_Warehouse_ProductionCostPoundType]
		Foreign Key ( [id_productionCostPoundType] )
		References [dbo].[ProductionCostPoundType] ( id );
End


-- 4. Eliminación de campos ya no usados para tabla [dbo].[Warehouse]
If ColumnProperty(Object_ID('[dbo].[Warehouse]'), 'id_costProduction', 'ColumnId') Is Not Null
Begin
	Alter Table [dbo].[Warehouse]
	Drop Column [id_costProduction];
End
If ColumnProperty(Object_ID('[dbo].[Warehouse]'), 'id_expenseProduction', 'ColumnId') Is Not Null
Begin
	Alter Table [dbo].[Warehouse]
	Drop Column [id_expenseProduction];
End
If ColumnProperty(Object_ID('[dbo].[Warehouse]'), 'id_accountingTemplateCost', 'ColumnId') Is Not Null
Begin
	If Object_ID('[dbo].[FK_Warehouse_AccountingTemplate]') Is Not Null
	Begin
		Alter Table [dbo].[Warehouse]
		Drop Constraint [FK_Warehouse_AccountingTemplate];
	End

	Alter Table [dbo].[Warehouse]
	Drop Column [id_accountingTemplateCost];
End
If ColumnProperty(Object_ID('[dbo].[Warehouse]'), 'nameProcessPlant', 'ColumnId') Is Not Null
Begin
	Alter Table [dbo].[Warehouse]
	Drop Column [nameProcessPlant];
End


-- 5. Preparación de la tabla [dbo].[ProductionCostExecutionType]
If Object_ID('[dbo].[ProductionCostExecutionType]') Is Null
Begin
	Create Table [dbo].[ProductionCostExecutionType]
	(
		[id]			Int Identity	Not Null,
		[code]			VarChar(10)		Not Null,
		[name]			VarChar(50)		Not Null,
		[order]			Int				Not Null,
		[description]	VarChar(Max)	Null,
		[isActive]		Bit				Not Null,
		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostExecutionType] Primary Key ( [id] ),
		Constraint [UQ_ProductionCostExecutionType] Unique ( [code] )
	);
End;

-- Carga de valores iniciales
Declare	@ProductionCostExecutionTypeTemp Table (
	[id]	Int Identity	Primary Key,
	[code]	VarChar(10),
	[name]	VarChar(50),
	[order]	Int
);

Insert	Into @ProductionCostExecutionTypeTemp ( [code], [name], [order] )
Values	( 'APOYO', 'Apoyo', 1 ),
		( 'PRODU', 'Producción', 2 ),
		( 'TERMI', 'Terminado', 3 );

Insert Into [dbo].[ProductionCostExecutionType]
	(
		[code],
		[name],
		[order],
		[isActive],
		id_userCreate,
		dateCreate,
		id_userUpdate,
		dateUpdate
	)
Select	[code],
		[name],
		[order],
		1,
		1,
		GetDate(),
		1,
		GetDate()
From	@ProductionCostExecutionTypeTemp
Where	code Not In ( Select code From [dbo].[ProductionCostExecutionType] )
Order	By id;


-- 6. Preparación de la tabla [dbo].[ProductionCost]
If Object_ID('[dbo].[ProductionCost]') Is Null
Begin
	Create Table [dbo].[ProductionCost]
	(
		[id]			Int Identity	Not Null,
		[code]			VarChar(20)		Not Null,
		[name]			VarChar(50)		Not Null,
		[order]			Int				Not Null,
		[id_executionType]	Int			Not Null,
		[description]	VarChar(Max)	Null,
		[isActive]		Bit				Not Null,
		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCost] Primary Key ( [id] ),
		Constraint [UQ_ProductionCost] Unique ( [code] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCost]'), 'IX_ProductionCost_ProductionCostExecutionType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCost_ProductionCostExecutionType]
		On [dbo].[ProductionCost] ( [id_executionType] );
End

If Object_ID('[dbo].[FK_ProductionCost_ProductionCostExecutionType]') Is Null
Begin
	Alter Table [dbo].[ProductionCost]
	Add Constraint [FK_ProductionCost_ProductionCostExecutionType]
		Foreign Key ( [id_executionType] )
		References [dbo].[ProductionCostExecutionType] ( id );
End


-- 7. Preparación de la tabla [dbo].[ProductionCostDetail]
If Object_ID('[dbo].[ProductionCostDetail]') Is Null
Begin
	Create Table [dbo].[ProductionCostDetail]
	(
		[id]			Int Identity	Not Null,
		[id_productionCost]	Int			Not Null,
		[code]			VarChar(20)		Not Null,
		[name]			VarChar(50)		Not Null,
		[order]			Int				Not Null,
		[description]	VarChar(Max)	Null,
		[isActive]		Bit				Not Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostDetail] Primary Key ( [id] ),
		Constraint [UQ_ProductionCostDetail] Unique ( [id_productionCost], [code] )
	);
End;

If Object_ID('[dbo].[FK_ProductionCostDetail_ProductionCost]') Is Null
Begin
	Alter Table [dbo].[ProductionCostDetail]
	Add Constraint [FK_ProductionCostDetail_ProductionCost]
		Foreign Key ( [id_productionCost] )
		References [dbo].[ProductionCost] ( id );
End


-- 8. Adición del parámetro para la habilitación de Integración con Sistema Contable
If Not Exists ( Select * From [dbo].[Setting] Where [code] = 'INTX-CONTAB' )
Begin
	Insert Into [dbo].[Setting]
		(
			[code],
			[name],
			[description],
			[id_module],
			[id_company],
			[id_settingDataType],
			[value],
			[isActive],
			id_userCreate,
			dateCreate,
			id_userUpdate,
			dateUpdate
		)
	Select	'INTX-CONTAB',
			'Integración con Sistema Contable',
			'Indicador para habilitar la integración con el Sistema Contable de Panaceasoft',
			M.[id],
			T.[id_company],
			T.[id],
			'0',
			1,
			1,
			GetDate(),
			1,
			GetDate()
	From	[dbo].[Module] As M, [dbo].[SettingDataType] As T
	Where	M.[code] = 'PRO'
	And		T.[code] = 'ENT';
End


-- 9. Preparación de la tabla [dbo].[ProductionCostCoefficient]
If Object_ID('[dbo].[ProductionCostCoefficient]') Is Null
Begin
	Create Table [dbo].[ProductionCostCoefficient]
	(
		[id]				Int Identity	Not Null,
		[sequence]			Int				Not Null,
		[id_executionType]	Int				Not Null,
		[id_planDeCuentas]	Char(2)			Null,
		[id_warehouseType]	Int				Null,
		[id_poundType]		Int				Not Null,
		[id_simpleFormula]	Int				Not Null,
		[id_productionCost]	Int				Not Null,
		[id_productionCostDetail]	Int		Not Null,
		[id_productionPlant]		Int		Null,
		[description]		VarChar(Max)	Null,
		[isActive]			Bit				Not Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostCoefficient] Primary Key ( [id] ),
		Constraint [UQ_ProductionCostCoefficient] Unique ( [sequence] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficient]'), 'IX_ProductionCostCoefficient_ProductionCostExecutionType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficient_ProductionCostExecutionType]
		On [dbo].[ProductionCostCoefficient] ( [id_executionType] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficient]'), 'IX_ProductionCostCoefficient_WarehouseType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficient_WarehouseType]
		On [dbo].[ProductionCostCoefficient] ( [id_warehouseType] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficient]'), 'IX_ProductionCostCoefficient_ProductionCostPoundType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficient_ProductionCostPoundType]
		On [dbo].[ProductionCostCoefficient] ( [id_poundType] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficient]'), 'IX_ProductionCostCoefficient_SimpleFormula', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficient_SimpleFormula]
		On [dbo].[ProductionCostCoefficient] ( [id_simpleFormula] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficient]'), 'IX_ProductionCostCoefficient_ProductionCost', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficient_ProductionCost]
		On [dbo].[ProductionCostCoefficient] ( [id_productionCost] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficient]'), 'IX_ProductionCostCoefficient_ProductionCostDetail', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficient_ProductionCostDetail]
		On [dbo].[ProductionCostCoefficient] ( [id_productionCostDetail] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficient]'), 'IX_ProductionCostCoefficient_ProductionPlant', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficient_ProductionPlant]
		On [dbo].[ProductionCostCoefficient] ( [id_productionPlant] );
End

If Object_ID('[dbo].[FK_ProductionCostCoefficient_ProductionCostExecutionType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficient]
	Add Constraint [FK_ProductionCostCoefficient_ProductionCostExecutionType]
		Foreign Key ( [id_executionType] )
		References [dbo].[ProductionCostExecutionType] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficient_WarehouseType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficient]
	Add Constraint [FK_ProductionCostCoefficient_WarehouseType]
		Foreign Key ( [id_warehouseType] )
		References [dbo].[WarehouseType] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficient_ProductionCostPoundType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficient]
	Add Constraint [FK_ProductionCostCoefficient_ProductionCostPoundType]
		Foreign Key ( [id_poundType] )
		References [dbo].[ProductionCostPoundType] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficient_SimpleFormula]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficient]
	Add Constraint [FK_ProductionCostCoefficient_SimpleFormula]
		Foreign Key ( [id_simpleFormula] )
		References [dbo].[SimpleFormula] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficient_ProductionCost]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficient]
	Add Constraint [FK_ProductionCostCoefficient_ProductionCost]
		Foreign Key ( [id_productionCost] )
		References [dbo].[ProductionCost] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficient_ProductionCostDetail]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficient]
	Add Constraint [FK_ProductionCostCoefficient_ProductionCostDetail]
		Foreign Key ( [id_productionCostDetail] )
		References [dbo].[ProductionCostDetail] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficient_ProductionPlant]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficient]
	Add Constraint [FK_ProductionCostCoefficient_ProductionPlant]
		Foreign Key ( [id_productionPlant] )
		References [dbo].[Person] ( id );
End


-- 10. Preparación de la tabla de relación [dbo].[ProductionCostCoefficientWarehouse]
If Object_ID('[dbo].[ProductionCostCoefficientWarehouse]') Is Null
Begin
	Create Table [dbo].[ProductionCostCoefficientWarehouse]
	(
		[id]				Int Identity	Not Null,
		[id_coefficient]	Int				Not Null,
		[id_warehouse]		Int				Not Null,

		Constraint [PK_ProductionCostCoefficientWarehouse] Primary Key ( [id] ),
		Constraint [UQ_ProductionCostCoefficientWarehouse] Unique ( [id_coefficient], [id_warehouse] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientWarehouse]'), 'IX_ProductionCostCoefficientWarehouse_Warehouse', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientWarehouse_Warehouse]
		On [dbo].[ProductionCostCoefficientWarehouse] ( [id_warehouse] );
End

If Object_ID('[dbo].[FK_ProductionCostCoefficientWarehouse_ProductionCostCoefficient]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientWarehouse]
	Add Constraint [FK_ProductionCostCoefficientWarehouse_ProductionCostCoefficient]
		Foreign Key ( [id_coefficient] )
		References [dbo].[ProductionCostCoefficient] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientWarehouse_Warehouse]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientWarehouse]
	Add Constraint [FK_ProductionCostCoefficientWarehouse_Warehouse]
		Foreign Key ( [id_warehouse] )
		References [dbo].[Warehouse] ( id );
End


-- 11. Preparación de la tabla de relación [dbo].[ProductionCostCoefficientWarehouseLocation]
If Object_ID('[dbo].[ProductionCostCoefficientWarehouseLocation]') Is Null
Begin
	Create Table [dbo].[ProductionCostCoefficientWarehouseLocation]
	(
		[id]					Int Identity	Not Null,
		[id_coefficient]		Int				Not Null,
		[id_warehouseLocation]	Int				Not Null,

		Constraint [PK_ProductionCostCoefficientWarehouseLocation] Primary Key ( [id] ),
		Constraint [UQ_ProductionCostCoefficientWarehouseLocation] Unique ( [id_coefficient], [id_warehouseLocation] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientWarehouseLocation]'), 'IX_ProductionCostCoefficientWarehouse_WarehouseLocation', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientWarehouse_WarehouseLocation]
		On [dbo].[ProductionCostCoefficientWarehouseLocation] ( [id_warehouseLocation] );
End

If Object_ID('[dbo].[FK_ProductionCostCoefficientWarehouseLocation_ProductionCostCoefficient]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientWarehouseLocation]
	Add Constraint [FK_ProductionCostCoefficientWarehouseLocation_ProductionCostCoefficient]
		Foreign Key ( [id_coefficient] )
		References [dbo].[ProductionCostCoefficient] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientWarehouseLocation_WarehouseLocation]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientWarehouseLocation]
	Add Constraint [FK_ProductionCostCoefficientWarehouseLocation_WarehouseLocation]
		Foreign Key ( [id_warehouseLocation] )
		References [dbo].[WarehouseLocation] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientWarehouse_WarehouseLocation]') Is Not Null
Begin
	-- Eliminar relación creado con nombre equivocado...
	Alter Table [dbo].[ProductionCostCoefficientWarehouseLocation]
	Drop Constraint [FK_ProductionCostCoefficientWarehouse_WarehouseLocation];
End


-- 12. Preparación de la tabla [dbo].[ProductionCostCoefficientDetail]
If Object_ID('[dbo].[ProductionCostCoefficientDetail]') Is Not Null
	And ColumnProperty(Object_ID('[dbo].[ProductionCostCoefficientDetail]'), 'id_planDeCuentas', 'ColumnId') Is Null
Begin
	-- No existe el campo agregado posteriormente... regenerar la nueva tabla...
	Drop Table [dbo].[ProductionCostCoefficientDetail];
End

If Object_ID('[dbo].[ProductionCostCoefficientDetail]') Is Null
Begin
	Create Table [dbo].[ProductionCostCoefficientDetail]
	(
		[id]					Int Identity	Not Null,
		[id_coefficient]		Int				Not Null,
		[id_planDeCuentas]		Char(2)			Null,
		[id_cuentaContab]		VarChar(15)		Null,
		[id_tipoAuxContab]		VarChar(3)		Null,
		[id_auxiliarContab]		VarChar(8)		Null,
		[id_tipoPresContab]		Char(1)			Null,
		[id_centroCtoContab]	VarChar(8)		Null,
		[id_subcentroCtoContab]	VarChar(8)		Null,
		[isActive]				Bit				Not Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostCoefficientDetail] Primary Key ( [id] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientDetail]'), 'IX_ProductionCostCoefficientDetail_ProductionCostCoefficient', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientDetail_ProductionCostCoefficient]
		On [dbo].[ProductionCostCoefficientDetail] ( [id_coefficient] );
End

If Object_ID('[dbo].[FK_ProductionCostCoefficientDetail_ProductionCostCoefficient]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientDetail]
	Add Constraint [FK_ProductionCostCoefficientDetail_ProductionCostCoefficient]
		Foreign Key ( [id_coefficient] )
		References [dbo].[ProductionCostCoefficient] ( id );
End


-- 13. Creación del tipo de documento "Costos Por Periodo"
If	Not Exists(Select * From dbo.DocumentType Where code = '152')
Begin
	Insert Into [dbo].[DocumentType]
		(
			[code],
			[name],
			[description],
			[currentNumber],
			[daysToExpiration],
			[isElectronic],
			[codeSRI],
			[id_company],
			[isActive],
			id_userCreate,
			dateCreate,
			id_userUpdate,
			dateUpdate
		) Values (
			'152',
			'Costos Por Período',
			'Costos Por Período',
			1,
			0,
			0,
			'',
			(Select idCompany = id From dbo.Company Where code = '01'),
			1,
			1,
			GetDate(),
			1,
			GetDate()
		);
End
Else
Begin
	Update	[dbo].[DocumentType]
	Set		[name] = 'Costos Por Período',
			[description] ='Costos Por Período',
			id_userUpdate = 1,
			dateUpdate = GetDate()
	Where	code = '152'
	And		[name] != 'Costos Por Período';
End


-- 14. Preparación de la tabla [dbo].[ProductionCostAllocationPeriod]
If Object_ID('[dbo].[ProductionCostAllocationPeriod]') Is Not Null
	And ColumnProperty(Object_ID('[dbo].[ProductionCostAllocationPeriod]'), 'id', 'ColumnId') Is Not Null
	And ColumnProperty(Object_ID('[dbo].[ProductionCostAllocationPeriod]'), 'id', 'IsIdentity') = 1
Begin
	-- Corrección de campo mal creado
	If Object_ID('[dbo].[ProductionCostAllocationPeriodDetail]') Is Not Null
	Begin
		Drop Table [dbo].[ProductionCostAllocationPeriodDetail];
	End
	Drop Table [dbo].[ProductionCostAllocationPeriod];
End

If Object_ID('[dbo].[ProductionCostAllocationPeriod]') Is Null
Begin
	Create Table [dbo].[ProductionCostAllocationPeriod]
	(
		[id]				Int				Not Null,
		[id_company]		Int				Not Null,
		[anio]				Int				Not Null,
		[mes]				Int				Not Null,
		[id_executionType]	Int				Not Null,
		[id_coefficient]	Int				Not Null,
		[accountingValue]	Bit				Not Null,
		[description]		VarChar(Max)	Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostAllocationPeriod] Primary Key ( [id] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostAllocationPeriod]'), 'IX_ProductionCostAllocationPeriod_ProductionCostExecutionType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostAllocationPeriod_ProductionCostExecutionType]
		On [dbo].[ProductionCostAllocationPeriod] ( [id_executionType] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostAllocationPeriod]'), 'IX_ProductionCostAllocationPeriod_ProductionCostCoefficient', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostAllocationPeriod_ProductionCostCoefficient]
		On [dbo].[ProductionCostAllocationPeriod] ( [id_coefficient] );
End

If Object_ID('[dbo].[FK_ProductionCostAllocationPeriod_Document]') Is Null
Begin
	Alter Table [dbo].[ProductionCostAllocationPeriod]
	Add Constraint [FK_ProductionCostAllocationPeriod_Document]
		Foreign Key ( [id] )
		References [dbo].[Document] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostAllocationPeriod_ProductionCostExecutionType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostAllocationPeriod]
	Add Constraint [FK_ProductionCostAllocationPeriod_ProductionCostExecutionType]
		Foreign Key ( [id_executionType] )
		References [dbo].[ProductionCostExecutionType] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostAllocationPeriod_ProductionCostCoefficient]') Is Null
Begin
	Alter Table [dbo].[ProductionCostAllocationPeriod]
	Add Constraint [FK_ProductionCostAllocationPeriod_ProductionCostCoefficient]
		Foreign Key ( [id_coefficient] )
		References [dbo].[ProductionCostCoefficient] ( id );
End


-- 15. Preparación de la tabla [dbo].[ProductionCostAllocationPeriodDetail]
If Object_ID('[dbo].[ProductionCostAllocationPeriodDetail]') Is Null
Begin
	Create Table [dbo].[ProductionCostAllocationPeriodDetail]
	(
		[id]					Int Identity	Not Null,
		[id_allocationPeriod]	Int				Not Null,
		[id_productionCost]		Int				Not Null,
		[id_productionCostDetail]	Int			Not Null,
		[id_productionPlant]	Int				Null,
		[coeficiente]			Bit				Not Null,
		[valor]					Decimal(16, 6)	Not Null,
		[isActive]				Bit				Not Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostAllocationPeriodDetail] Primary Key ( [id] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostAllocationPeriodDetail]'), 'IX_ProductionCostAllocationPeriodDetail_ProductionCostAllocationPeriod', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostAllocationPeriodDetail_ProductionCostAllocationPeriod]
		On [dbo].[ProductionCostAllocationPeriodDetail] ( [id_allocationPeriod] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostAllocationPeriodDetail]'), 'IX_ProductionCostAllocationPeriodDetail_ProductionCost', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostAllocationPeriodDetail_ProductionCost]
		On [dbo].[ProductionCostAllocationPeriodDetail] ( [id_productionCost] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostAllocationPeriodDetail]'), 'IX_ProductionCostAllocationPeriodDetail_ProductionCostDetail', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostAllocationPeriodDetail_ProductionCostDetail]
		On [dbo].[ProductionCostAllocationPeriodDetail] ( [id_productionCostDetail] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostAllocationPeriodDetail]'), 'IX_ProductionCostAllocationPeriodDetail_ProductionPlant', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostAllocationPeriodDetail_ProductionPlant]
		On [dbo].[ProductionCostAllocationPeriodDetail] ( [id_productionPlant] );
End

If Object_ID('[dbo].[FK_ProductionCostAllocationPeriodDetail_ProductionCostAllocationPeriod]') Is Null
Begin
	Alter Table [dbo].[ProductionCostAllocationPeriodDetail]
	Add Constraint [FK_ProductionCostAllocationPeriodDetail_ProductionCostAllocationPeriod]
		Foreign Key ( [id_allocationPeriod] )
		References [dbo].[ProductionCostAllocationPeriod] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostAllocationPeriodDetail_ProductionCost]') Is Null
Begin
	Alter Table [dbo].[ProductionCostAllocationPeriodDetail]
	Add Constraint [FK_ProductionCostAllocationPeriodDetail_ProductionCost]
		Foreign Key ( [id_productionCost] )
		References [dbo].[ProductionCost] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostAllocationPeriodDetail_ProductionCostDetail]') Is Null
Begin
	Alter Table [dbo].[ProductionCostAllocationPeriodDetail]
	Add Constraint [FK_ProductionCostAllocationPeriodDetail_ProductionCostDetail]
		Foreign Key ( [id_productionCostDetail] )
		References [dbo].[ProductionCostDetail] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostAllocationPeriodDetail_ProductionPlant]') Is Null
Begin
	Alter Table [dbo].[ProductionCostAllocationPeriodDetail]
	Add Constraint [FK_ProductionCostAllocationPeriodDetail_ProductionPlant]
		Foreign Key ( [id_productionPlant] )
		References [dbo].[Person] ( id );
End


-- 16. Creación del tipo de documento "Coeficientes de Produccion"
If	Not Exists(Select * From dbo.DocumentType Where code = '153')
Begin
	Insert Into [dbo].[DocumentType]
		(
			[code],
			[name],
			[description],
			[currentNumber],
			[daysToExpiration],
			[isElectronic],
			[codeSRI],
			[id_company],
			[isActive],
			id_userCreate,
			dateCreate,
			id_userUpdate,
			dateUpdate
		) Values (
			'153',
			'Coeficientes de Producción',
			'Coeficientes de Producción',
			1,
			0,
			0,
			'',
			(Select idCompany = id From dbo.Company Where code = '01'),
			1,
			1,
			GetDate(),
			1,
			GetDate()
		);
End
Else
Begin
	Update	[dbo].[DocumentType]
	Set		[name] = 'Coeficientes de Producción',
			[description] ='Coeficientes de Producción',
			id_userUpdate = 1,
			dateUpdate = GetDate()
	Where	code = '153'
	And		[name] != 'Coeficientes de Producción';
End


-- 17. Preparación de la tabla [dbo].[ProductionCostAllocationType]
If Object_ID('[dbo].[ProductionCostAllocationType]') Is Null
Begin
	Create Table [dbo].[ProductionCostAllocationType]
	(
		[id]			Int Identity	Not Null,
		[code]			VarChar(10)		Not Null,
		[name]			VarChar(50)		Not Null,
		[order]			Int				Not Null,
		[description]	VarChar(Max)	Null,
		[isActive]		Bit				Not Null,
		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostAllocationType] Primary Key ( [id] ),
		Constraint [UQ_ProductionCostAllocationType] Unique ( [code] )
	);
End;

-- Carga de valores iniciales
Declare	@ProductionCostAllocationTypeTemp Table (
	[id]	Int Identity	Primary Key,
	[code]	VarChar(10),
	[name]	VarChar(50),
	[order]	Int
);

Insert	Into @ProductionCostAllocationTypeTemp ( [code], [name], [order] )
Values	( 'REAL', 'Costo Real', 1 ),
		( 'PROJ', 'Costo Proyectado', 2 );

Insert Into [dbo].[ProductionCostAllocationType]
	(
		[code],
		[name],
		[order],
		[isActive],
		id_userCreate,
		dateCreate,
		id_userUpdate,
		dateUpdate
	)
Select	[code],
		[name],
		[order],
		1,
		1,
		GetDate(),
		1,
		GetDate()
From	@ProductionCostAllocationTypeTemp
Where	code Not In ( Select code From [dbo].[ProductionCostAllocationType] )
Order	By id;


-- 18. Adición del estado PROCESADO a la tabla de estados de documentos
--If	Not Exists(Select * From dbo.DocumentState Where code = '18')
--Begin
--	Insert Into [dbo].[DocumentState]
--		(
--			[code],
--			[name],
--			[description],
--			[id_company],
--			[isActive],
--			id_userCreate,
--			dateCreate,
--			id_userUpdate,
--			dateUpdate
--		) Values (
--			'18',
--			'PROCESADO',
--			'PROCESADO',
--			(Select idCompany = id From dbo.Company Where code = '01'),
--			1,
--			1,
--			GetDate(),
--			1,
--			GetDate()
--		);
--End


-- 19. Preparación de la tabla [dbo].[ProductionCostCoefficientExecution]
If Object_ID('[dbo].[ProductionCostCoefficientExecution]') Is Not Null
	And (
		ColumnProperty(Object_ID('[dbo].[ProductionCostCoefficientExecution]'), 'processed', 'ColumnId') Is Null
		Or
		ColumnProperty(Object_ID('[dbo].[ProductionCostCoefficientExecution]'), 'dateStart', 'ColumnId') Is Not Null
		Or
		ColumnProperty(Object_ID('[dbo].[ProductionCostCoefficientExecution]'), 'dateEnd', 'ColumnId') Is Not Null
		)
Begin
	-- Corrección de campo mal creado
	If Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]') Is Not Null
	Begin
		Drop Table [dbo].[ProductionCostCoefficientExecutionDetail];
	End
	If Object_ID('[dbo].[ProductionCostCoefficientExecutionPlant]') Is Not Null
	Begin
		Drop Table [dbo].[ProductionCostCoefficientExecutionPlant];
	End
	Drop Table [dbo].[ProductionCostCoefficientExecution];
End

If Object_ID('[dbo].[ProductionCostCoefficientExecution]') Is Null
Begin
	Create Table [dbo].[ProductionCostCoefficientExecution]
	(
		[id]				Int				Not Null,
		[id_company]		Int				Not Null,
		[id_allocationType]	Int				Not Null,
		[anio]				Int				Not Null,
		[mes]				Int				Not Null,
		[startDate]			Date			Not Null,
		[endDate]			Date			Not Null,
		[processed]			Bit				Not Null,
		[value_processed]	Bit				Not Null,
		[description]		VarChar(Max)	Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostCoefficientExecution] Primary Key ( [id] )
	);
End;
If ColumnProperty(Object_ID('[dbo].[ProductionCostCoefficientExecution]'), 'value_processed', 'ColumnId') Is Null
Begin
	-- No existe el campo nuevo requerido...
	Alter Table [dbo].[ProductionCostCoefficientExecution] Add [value_processed] Bit;
End
If ColumnProperty(Object_ID('[dbo].[ProductionCostCoefficientExecution]'), 'value_processed', 'AllowsNull') = 1
Begin
	Exec('Update [dbo].[ProductionCostCoefficientExecution] Set [value_processed] = 0 Where [value_processed] Is Null');
	Alter Table [dbo].[ProductionCostCoefficientExecution] Alter Column [value_processed] Bit Not Null;
End

If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecution]'), 'IX_ProductionCostCoefficientExecution_AllocationType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecution_AllocationType]
		On [dbo].[ProductionCostCoefficientExecution] ( [id_allocationType] );
End

If Object_ID('[dbo].[FK_ProductionCostCoefficientExecution_Document]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecution]
	Add Constraint [FK_ProductionCostCoefficientExecution_Document]
		Foreign Key ( [id] )
		References [dbo].[Document] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecution_ProductionCostAllocationType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecution]
	Add Constraint [FK_ProductionCostCoefficientExecution_ProductionCostAllocationType]
		Foreign Key ( [id_allocationType] )
		References [dbo].[ProductionCostAllocationType] ( id );
End


-- 20. Preparación de la tabla [dbo].[ProductionCostCoefficientExecutionDetail]
If Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]') Is Not Null
	And ColumnProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'id_executionType', 'ColumnId') Is Not Null
Begin
	-- Corrección de campo mal creado
	Drop Table [dbo].[ProductionCostCoefficientExecutionDetail];
End

If Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]') Is Null
Begin
	Create Table [dbo].[ProductionCostCoefficientExecutionDetail]
	(
		[id]						Int Identity	Not Null,
		[id_coefficientExecution]	Int				Not Null,
		[id_allocationPeriod]		Int				Not Null,
		[id_allocationPeriodDetail]	Int				Not Null,
		[anio]						Int				Not Null,
		[mes]						Int				Not Null,
		[id_coefficient]			Int				Not Null,
		[id_poundType]				Int				Not Null,
		[id_simpleFormula]			Int				Not Null,
		[accountingValue]			Bit				Not Null,
		[id_productionCost]			Int				Not Null,
		[id_productionCostDetail]	Int				Not Null,
		[id_productionPlant]		Int				Null,
		[coeficiente]				Bit				Not Null,
		[valor]						Decimal(16, 6)	Not Null,
		[isActive]					Bit				Not Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostCoefficientExecutionDetail] Primary Key ( [id] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'IX_ProductionCostCoefficientExecutionDetail_CoefficientExecution', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionDetail_CoefficientExecution]
		On [dbo].[ProductionCostCoefficientExecutionDetail] ( [id_coefficientExecution] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'IX_ProductionCostCoefficientExecutionDetail_AllocationPeriod', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionDetail_AllocationPeriod]
		On [dbo].[ProductionCostCoefficientExecutionDetail] ( [id_allocationPeriod] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'IX_ProductionCostCoefficientExecutionDetail_AllocationPeriodDetail', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionDetail_AllocationPeriodDetail]
		On [dbo].[ProductionCostCoefficientExecutionDetail] ( [id_allocationPeriodDetail] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'IX_ProductionCostCoefficientExecutionDetail_Coefficient', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionDetail_Coefficient]
		On [dbo].[ProductionCostCoefficientExecutionDetail] ( [id_coefficient] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'IX_ProductionCostCoefficientExecutionDetail_PoundType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionDetail_PoundType]
		On [dbo].[ProductionCostCoefficientExecutionDetail] ( [id_poundType] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'IX_ProductionCostCoefficientExecutionDetail_SimpleFormula', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionDetail_SimpleFormula]
		On [dbo].[ProductionCostCoefficientExecutionDetail] ( [id_simpleFormula] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'IX_ProductionCostCoefficientExecutionDetail_ProductionCost', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionDetail_ProductionCost]
		On [dbo].[ProductionCostCoefficientExecutionDetail] ( [id_productionCost] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'IX_ProductionCostCoefficientExecutionDetail_ProductionCostDetail', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionDetail_ProductionCostDetail]
		On [dbo].[ProductionCostCoefficientExecutionDetail] ( [id_productionCostDetail] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionDetail]'), 'IX_ProductionCostCoefficientExecutionDetail_ProductionPlant', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionDetail_ProductionPlant]
		On [dbo].[ProductionCostCoefficientExecutionDetail] ( [id_productionPlant] );
End

If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionDetail_ProductionCostCoefficientExecution]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionDetail]
	Add Constraint [FK_ProductionCostCoefficientExecutionDetail_ProductionCostCoefficientExecution]
		Foreign Key ( [id_coefficientExecution] )
		References [dbo].[ProductionCostCoefficientExecution] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionDetail_ProductionCostAllocationPeriod]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionDetail]
	Add Constraint [FK_ProductionCostCoefficientExecutionDetail_ProductionCostAllocationPeriod]
		Foreign Key ( [id_allocationPeriod] )
		References [dbo].[ProductionCostAllocationPeriod] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionDetail_ProductionCostAllocationPeriodDetail]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionDetail]
	Add Constraint [FK_ProductionCostCoefficientExecutionDetail_ProductionCostAllocationPeriodDetail]
		Foreign Key ( [id_allocationPeriodDetail] )
		References [dbo].[ProductionCostAllocationPeriodDetail] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionDetail_ProductionCostCoefficient]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionDetail]
	Add Constraint [FK_ProductionCostCoefficientExecutionDetail_ProductionCostCoefficient]
		Foreign Key ( [id_coefficient] )
		References [dbo].[ProductionCostCoefficient] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionDetail_ProductionCostPoundType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionDetail]
	Add Constraint [FK_ProductionCostCoefficientExecutionDetail_ProductionCostPoundType]
		Foreign Key ( [id_poundType] )
		References [dbo].[ProductionCostPoundType] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionDetail_SimpleFormula]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionDetail]
	Add Constraint [FK_ProductionCostCoefficientExecutionDetail_SimpleFormula]
		Foreign Key ( [id_simpleFormula] )
		References [dbo].[SimpleFormula] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionDetail_ProductionCost]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionDetail]
	Add Constraint [FK_ProductionCostCoefficientExecutionDetail_ProductionCost]
		Foreign Key ( [id_productionCost] )
		References [dbo].[ProductionCost] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionDetail_ProductionCostDetail]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionDetail]
	Add Constraint [FK_ProductionCostCoefficientExecutionDetail_ProductionCostDetail]
		Foreign Key ( [id_productionCostDetail] )
		References [dbo].[ProductionCostDetail] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionDetail_ProductionPlant]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionDetail]
	Add Constraint [FK_ProductionCostCoefficientExecutionDetail_ProductionPlant]
		Foreign Key ( [id_productionPlant] )
		References [dbo].[Person] ( id );
End


-- 21. Preparación de la tabla [dbo].[ProductionCostCoefficientExecutionPlant]
If Object_ID('[dbo].[ProductionCostCoefficientExecutionPlant]') Is Not Null
	And ColumnProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionPlant]'), 'coeficiente', 'ColumnId') Is Null
Begin
	-- Corrección de campo mal creado
	Drop Table [dbo].[ProductionCostCoefficientExecutionPlant];
End

If Object_ID('[dbo].[ProductionCostCoefficientExecutionPlant]') Is Null
Begin
	Create Table [dbo].[ProductionCostCoefficientExecutionPlant]
	(
		[id]						Int Identity	Not Null,
		[id_coefficientExecution]	Int				Not Null,
		[id_productionPlant]		Int				Not Null,
		[id_inventoryLine]			Int				Not Null,
		[id_itemType]				Int				Not Null,
		[libras]					Decimal(16, 6)	Not Null,
		[porcentaje]				Decimal(16, 6)	Not Null,
		[valor]						Decimal(16, 6)	Not Null,
		[coeficiente]				Decimal(16, 6)	Not Null,
		[isActive]					Bit				Not Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostCoefficientExecutionPlant] Primary Key ( [id] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionPlant]'), 'IX_ProductionCostCoefficientExecutionPlant_CoefficientExecution', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionPlant_CoefficientExecution]
		On [dbo].[ProductionCostCoefficientExecutionPlant] ( [id_coefficientExecution] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionPlant]'), 'IX_ProductionCostCoefficientExecutionPlant_Plant', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionPlant_Plant]
		On [dbo].[ProductionCostCoefficientExecutionPlant] ( [id_productionPlant] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionPlant]'), 'IX_ProductionCostCoefficientExecutionPlant_InventoryLine', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionPlant_InventoryLine]
		On [dbo].[ProductionCostCoefficientExecutionPlant] ( [id_inventoryLine] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionPlant]'), 'IX_ProductionCostCoefficientExecutionPlant_ItemType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionPlant_ItemType]
		On [dbo].[ProductionCostCoefficientExecutionPlant] ( [id_itemType] );
End

If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionPlant_ProductionCostCoefficientExecution]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionPlant]
	Add Constraint [FK_ProductionCostCoefficientExecutionPlant_ProductionCostCoefficientExecution]
		Foreign Key ( [id_coefficientExecution] )
		References [dbo].[ProductionCostCoefficientExecution] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionPlant_ProductionPlant]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionPlant]
	Add Constraint [FK_ProductionCostCoefficientExecutionPlant_ProductionPlant]
		Foreign Key ( [id_productionPlant] )
		References [dbo].[Person] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionPlant_InventoryLine]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionPlant]
	Add Constraint [FK_ProductionCostCoefficientExecutionPlant_InventoryLine]
		Foreign Key ( [id_inventoryLine] )
		References [dbo].[InventoryLine] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionPlant_ItemType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionPlant]
	Add Constraint [FK_ProductionCostCoefficientExecutionPlant_ItemType]
		Foreign Key ( [id_itemType] )
		References [dbo].[ItemType] ( id );
End


-- 22. Preparación de la tabla [dbo].[ProductionCostCoefficientExecutionWarehouse]
If Object_ID('[dbo].[ProductionCostCoefficientExecutionWarehouse]') Is Not Null
	And ColumnProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionWarehouse]'), 'coeficiente', 'ColumnId') Is Null
Begin
	-- Corrección de campo mal creado
	Drop Table [dbo].[ProductionCostCoefficientExecutionWarehouse];
End

If Object_ID('[dbo].[ProductionCostCoefficientExecutionWarehouse]') Is Null
Begin
	Create Table [dbo].[ProductionCostCoefficientExecutionWarehouse]
	(
		[id]						Int Identity	Not Null,
		[id_coefficientExecution]	Int				Not Null,
		[id_warehouse]				Int				Not Null,
		[id_inventoryLine]			Int				Not Null,
		[id_itemType]				Int				Not Null,
		[id_poundType]				Int				Not Null,
		[libras]					Decimal(16, 6)	Not Null,
		[porcentaje]				Decimal(16, 6)	Not Null,
		[valor]						Decimal(16, 6)	Not Null,
		[coeficiente]				Decimal(16, 6)	Not Null,
		[isActive]					Bit				Not Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostCoefficientExecutionWarehouse] Primary Key ( [id] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionWarehouse]'), 'IX_ProductionCostCoefficientExecutionWarehouse_CoefficientExecution', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionWarehouse_CoefficientExecution]
		On [dbo].[ProductionCostCoefficientExecutionWarehouse] ( [id_coefficientExecution] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionWarehouse]'), 'IX_ProductionCostCoefficientExecutionWarehouse_Warehouse', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionWarehouse_Warehouse]
		On [dbo].[ProductionCostCoefficientExecutionWarehouse] ( [id_warehouse]);
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionWarehouse]'), 'IX_ProductionCostCoefficientExecutionWarehouse_InventoryLine', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionWarehouse_InventoryLine]
		On [dbo].[ProductionCostCoefficientExecutionWarehouse] ( [id_inventoryLine] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionWarehouse]'), 'IX_ProductionCostCoefficientExecutionWarehouse_ItemType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionWarehouse_ItemType]
		On [dbo].[ProductionCostCoefficientExecutionWarehouse] ( [id_itemType] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostCoefficientExecutionWarehouse]'), 'IX_ProductionCostCoefficientExecutionWarehouse_PoundType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostCoefficientExecutionWarehouse_PoundType]
		On [dbo].[ProductionCostCoefficientExecutionWarehouse] ( [id_poundType] );
End

If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionWarehouse_ProductionCostCoefficientExecution]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionWarehouse]
	Add Constraint [FK_ProductionCostCoefficientExecutionWarehouse_ProductionCostCoefficientExecution]
		Foreign Key ( [id_coefficientExecution] )
		References [dbo].[ProductionCostCoefficientExecution] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionWarehouse_Warehouse]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionWarehouse]
	Add Constraint [FK_ProductionCostCoefficientExecutionWarehouse_Warehouse]
		Foreign Key ( [id_warehouse] )
		References [dbo].[Warehouse] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionWarehouse_InventoryLine]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionWarehouse]
	Add Constraint [FK_ProductionCostCoefficientExecutionWarehouse_InventoryLine]
		Foreign Key ( [id_inventoryLine] )
		References [dbo].[InventoryLine] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionWarehouse_ItemType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionWarehouse]
	Add Constraint [FK_ProductionCostCoefficientExecutionWarehouse_ItemType]
		Foreign Key ( [id_itemType] )
		References [dbo].[ItemType] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostCoefficientExecutionWarehouse_PoundType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostCoefficientExecutionWarehouse]
	Add Constraint [FK_ProductionCostCoefficientExecutionWarehouse_PoundType]
		Foreign Key ( [id_poundType] )
		References [dbo].[ProductionCostPoundType] ( id );
End


-- 23. Creación del tipo de documento "Valorización de Productos"
If	Not Exists(Select * From dbo.DocumentType Where code = '158')
Begin
	Insert Into [dbo].[DocumentType]
		(
			[code],
			[name],
			[description],
			[currentNumber],
			[daysToExpiration],
			[isElectronic],
			[codeSRI],
			[id_company],
			[isActive],
			id_userCreate,
			dateCreate,
			id_userUpdate,
			dateUpdate
		) Values (
			'158',
			'Valorización de Productos',
			'Valorización de Productos',
			1,
			0,
			0,
			'',
			(Select idCompany = id From dbo.Company Where code = '01'),
			1,
			1,
			GetDate(),
			1,
			GetDate()
		);
End
Else
Begin
	Update	[dbo].[DocumentType]
	Set		[name] = 'Valorización de Productos',
			[description] ='Valorización de Productos',
			id_userUpdate = 1,
			dateUpdate = GetDate()
	Where	code = '158'
	And		[name] != 'Valorización de Productos';
End


-- 24. Preparación de la tabla [dbo].[ProductionCostProductValuation]
If Object_ID('[dbo].[ProductionCostProductValuation]') Is Not Null
	And (
		ColumnProperty(Object_ID('[dbo].[ProductionCostProductValuation]'), 'startDate', 'ColumnId') Is Not Null
		Or
		ColumnProperty(Object_ID('[dbo].[ProductionCostProductValuation]'), 'endDate', 'ColumnId') Is Not Null
		)
Begin
	-- Corrección de campo mal creado
	If Object_ID('[dbo].[ProductionCostProductValuationExecution]') Is Not Null
	Begin
		Drop Table [dbo].[ProductionCostProductValuationExecution];
	End;

	Drop Table [dbo].[ProductionCostProductValuation];
End

If Object_ID('[dbo].[ProductionCostProductValuation]') Is Null
Begin
	Create Table [dbo].[ProductionCostProductValuation]
	(
		[id]				Int				Not Null,
		[id_company]		Int				Not Null,
		[id_allocationType]	Int				Not Null,
		[anio]				Int				Not Null,
		[mes]				Int				Not Null,
		[processed]			Bit				Not Null,
		[description]		VarChar(Max)	Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostProductValuation] Primary Key ( [id] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostProductValuation]'), 'IX_ProductionCostProductValuation_AllocationType', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostProductValuation_AllocationType]
		On [dbo].[ProductionCostProductValuation] ( [id_allocationType] );
End

If Object_ID('[dbo].[FK_ProductionCostProductValuation_Document]') Is Null
Begin
	Alter Table [dbo].[ProductionCostProductValuation]
	Add Constraint [FK_ProductionCostProductValuation_Document]
		Foreign Key ( [id] )
		References [dbo].[Document] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostProductValuation_ProductionCostAllocationType]') Is Null
Begin
	Alter Table [dbo].[ProductionCostProductValuation]
	Add Constraint [FK_ProductionCostProductValuation_ProductionCostAllocationType]
		Foreign Key ( [id_allocationType] )
		References [dbo].[ProductionCostAllocationType] ( id );
End


-- 25. Preparación de la tabla [dbo].[ProductionCostProductValuationExecution]
If Object_ID('[dbo].[ProductionCostProductValuationExecution]') Is Null
Begin
	Create Table [dbo].[ProductionCostProductValuationExecution]
	(
		[id]						Int Identity	Not Null,
		[id_productValuation]		Int				Not Null,
		[id_coefficientExecution]	Int				Not Null,
		[valor]						Decimal(16, 6)	Not Null,
		[isActive]					Bit				Not Null,

		id_userCreate	Int			Not Null,
		dateCreate		DateTime	Not Null,
		id_userUpdate	Int			Not Null,
		dateUpdate		DateTime	Not Null,

		Constraint [PK_ProductionCostProductValuationExecution] Primary Key ( [id] )
	);
End;

If IndexProperty(Object_ID('[dbo].[ProductionCostProductValuationExecution]'), 'IX_ProductionCostProductValuationExecution_ProductValuation', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostProductValuationExecution_ProductValuation]
		On [dbo].[ProductionCostProductValuationExecution] ( [id_productValuation] );
End
If IndexProperty(Object_ID('[dbo].[ProductionCostProductValuationExecution]'), 'IX_ProductionCostProductValuationExecution_CoefficientExecution', 'IndexID') Is Null
Begin
	Create Index [IX_ProductionCostProductValuationExecution_CoefficientExecution]
		On [dbo].[ProductionCostProductValuationExecution] ( [id_coefficientExecution] );
End

If Object_ID('[dbo].[FK_ProductionCostProductValuationExecution_ProductValuation]') Is Null
Begin
	Alter Table [dbo].[ProductionCostProductValuationExecution]
	Add Constraint [FK_ProductionCostProductValuationExecution_ProductValuation]
		Foreign Key ( [id_productValuation] )
		References [dbo].[ProductionCostProductValuation] ( id );
End
If Object_ID('[dbo].[FK_ProductionCostProductValuationExecution_ProductionCostCoefficientExecution]') Is Null
Begin
	Alter Table [dbo].[ProductionCostProductValuationExecution]
	Add Constraint [FK_ProductionCostProductValuationExecution_ProductionCostCoefficientExecution]
		Foreign Key ( [id_coefficientExecution] )
		References [dbo].[ProductionCostCoefficientExecution] ( id );
End


