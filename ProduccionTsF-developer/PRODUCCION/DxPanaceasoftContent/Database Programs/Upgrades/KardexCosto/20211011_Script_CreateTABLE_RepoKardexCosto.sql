inventorymoveDetail

USE [Produccion_2019]
GO
/****** Object:  Table [dbo].[RepoKardexSaldo]    Script Date: 10/5/2021 11:46:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- drop TABLE [dbo].[RepoKardexCosto]
CREATE TABLE [dbo].[RepoKardexCosto](
	[idDetalleInventario] [int] NOT NULL,
	[idCabeceraInventario] [int] NOT NULL,
	[fechaInicio] [datetime] NULL,
	[fechaFin] [datetime] NULL,
	[numeroDocumentoInventario] [varchar](50) NOT NULL,
	[idBodega] [int] NOT NULL,
	[nombreBodega] [varchar](200) NOT NULL,
	[idUbicacion] [int] NOT NULL,
	[nombreUbicacion] [varchar](200) NOT NULL,
	[idProducto] [int] NOT NULL,
	[nombreProducto] [varchar](200) NOT NULL,
	[fechaEmison] [datetime] NOT NULL,
	[idMotivoInventario] [int] NOT NULL,
	[nombreMotivoInventario] [varchar](200) NOT NULL,
	[idUnidadMedida] [int] NOT NULL,
	[nombreUnidadMedida] [varchar](50) NOT NULL,
	[montoEntrada] [decimal](20, 6) NULL,
	[montoSalida] [decimal](20, 6) NULL,
	[balance] [decimal](20, 6) NULL,
	[previousBalance] [decimal](20, 6) NULL,
	[idEstado] [int] NULL,
	[nombreEstado] [varchar](200) NULL,
	[idCompania] [int] NULL,
	[nameCompania] [varchar](200) NULL,
	[nameDivision] [varchar](200) NULL,
	[nameBranchOffice] [varchar](200) NULL,
	[numberRemissionGuide] [varchar](50) NULL,
	[numberLot] [varchar](50) NULL,
	[Provider_name] [varchar](250) NULL,
	[isCopacking] [bit] NULL,
	[nameProviderShrimp] [varchar](250) NULL,
	[productionUnitProviderPool] [varchar](250) NULL,
	[itemSize] [varchar](250) NULL,
	[itemType] [varchar](250) NULL,
	[ItemMetricUnit] [varchar](50) NULL,
	[ItemPresentationValue] [decimal](14, 4) NULL,

	amount [decimal](20, 6) NULL,
	amountCostUnit [decimal](20, 6) NULL,
	amountCostTotal [decimal](20, 6) NULL,

	previousPound  [decimal](20, 6) NULL,
	previousCostPound  [decimal](20, 6) NULL,
	previousTotalCostPound  [decimal](20, 6) NULL,

	entradaPound [decimal](20, 6) NULL,
	entradaCostPound  [decimal](20, 6) NULL,
	entradaTotalCostPound  [decimal](20, 6) NULL,

	salidaPound [decimal](20, 6) NULL,
	salidaCostPound  [decimal](20, 6) NULL,
	salidaTotalCostPound  [decimal](20, 6) NULL,

	finalPound [decimal](20, 6) NULL,
	finalCostPound  [decimal](20, 6) NULL,
	finalTotalCostPound  [decimal](20, 6) NULL,

	[itemPresentationDescrip] [varchar](100) NULL, --  Filtrar 100
	[oneItemPound] [decimal](20, 6) NULL,


 CONSTRAINT [PK_RepoKardexCosto] PRIMARY KEY CLUSTERED 
(
	[idDetalleInventario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO




	RepoKardexCosto."previousPound",
	RepoKardexCosto."previousCostPound",
	RepoKardexCosto."previousTotalCostPound",
	RepoKardexCosto."entradaPound",
	RepoKardexCosto."entradaCostPound",
	RepoKardexCosto."entradaTotalCostPound",
	RepoKardexCosto."salidaPound",
	RepoKardexCosto."salidaCostPound",
	RepoKardexCosto."salidaTotalCostPound",
	RepoKardexCosto."finalPound",
	RepoKardexCosto."finalCostPound",
	RepoKardexCosto."finalTotalCostPound",