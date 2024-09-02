drop TABLE [dbo].[MonthlyBalance]

CREATE TABLE [dbo].[MonthlyBalance](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_company] [int] NOT NULL,  --- COLUMNA NUEVA
	[Anio] [int] NOT NULL,
	[Periodo] [int] NOT NULL,
	[id_item] [int] NOT NULL,
	[masterCode] [varchar](50) NULL,
	[name_item] [varchar](1000) NULL,
	[id_warehouse] [int] NOT NULL,
	[name_warehouse] [varchar](1000) NULL,
	[id_warehouseLocation] [int] NOT NULL,   --- COLUMNA NUEVA
	[name_warehouseLocation] [varchar](1000) NOT NULL, --- COLUMNA NUEVA
	[id_productionLot] [int] NULL,   --- COLUMNA NUEVA
	[number_productionLot] [varchar](20) NULL, --- COLUMNA NUEVA
	[sequencial_productionLot] [varchar](20) NULL, --- COLUMNA NUEVA
	[id_presentation] [int] NOT NULL,
	[name_presentation] [varchar](1000) NULL,
	[id_metric_unit] [int] NULL,
	[code_metric_unit] [varchar](100) NULL,
	[name_metric_unit] [varchar](1000) NULL,
	[SaldoAnterior] [decimal](20, 6) NULL,
	[Entrada] [decimal](20, 6) NULL,
	[Salida] [decimal](20, 6) NULL,
	[SaldoActual] [decimal](20, 6) NULL,
	[minimum] [decimal](20, 6) NULL,
	[maximum] [decimal](20, 6) NULL,
	[LB_SaldoAnterior] [decimal](20, 6) NULL,
	[LB_Entrada] [decimal](20, 6) NULL,
	[LB_Salida] [decimal](20, 6) NULL,
	[LB_SaldoActual] [decimal](20, 6) NULL,
 CONSTRAINT [PK_MonthlyBalance] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF ) ON [PRIMARY]
) ON [PRIMARY]
GO


create index IX_MonthlyBalance_PeriodWarehouse on MonthlyBalance(id_company,Anio,Periodo, id_warehouse);
create index IX_MonthlyBalance_Period on MonthlyBalance(id_company,Anio,Periodo);