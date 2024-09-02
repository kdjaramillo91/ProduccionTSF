 
GO
/****** Object:  Table [dbo].[CostAllocation]    Script Date: 10/4/2021 11:02:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CostAllocation](
	[id] [int] NOT NULL,
	[anio] [int] NOT NULL,
	[mes] [int] NOT NULL,
	[fechaIncio] [datetime] NOT NULL,
	[fechaFin] [datetime] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CostAllocationDetail]    Script Date: 10/4/2021 11:02:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CostAllocationDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_CostAllocation] [int] NOT NULL,
	[id_Item] [int] NOT NULL,
	[id_metricUnitMove] [int] NOT NULL,
	[id_InventoryMoveDetail] [int] NOT NULL,
	[id_WarehouseLocation] [int] NOT NULL,
	[id_Lot] [int] NULL,
	[amountBox] [int] NOT NULL,
	[productionCost] [decimal](16, 6) NOT NULL,
	[totalCost] [decimal](16, 6) NOT NULL,
	[amountPound] [decimal](16, 6) NOT NULL,
	[costPounds] [decimal](16, 6) NOT NULL,
	[totalCostPounds] [decimal](16, 6) NOT NULL,
	[amountKg] [decimal](16, 6) NOT NULL,
	[costKg] [decimal](16, 6) NOT NULL,
	[totalCostKg] [decimal](16, 6) NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
	[id_Warehouse] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[CostAllocationResumido]    Script Date: 10/4/2021 11:02:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CostAllocationResumido](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_CostAllocation] [int] NOT NULL,
	[id_InventoryLine] [int] NOT NULL,
	[id_ItemType] [int] NOT NULL,
	[id_ItemTypeCategory] [int] NOT NULL,
	[amountBox] [int] NOT NULL,
	[amountPound] [decimal](16, 6) NOT NULL,
	[amountKg] [decimal](16, 6) NOT NULL,
	[unitCostPounds] [decimal](16, 6) NOT NULL,
	[unitCostKg] [decimal](16, 6) NOT NULL,
	[averageCostUnit] [decimal](16, 6) NOT NULL,
	[totalCostPounds] [decimal](16, 6) NOT NULL,
	[totalCostKg] [decimal](16, 6) NOT NULL,
	[totalCostUnit] [decimal](16, 6) NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CostAllocationWarehouse]    Script Date: 10/4/2021 11:02:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CostAllocationWarehouse](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_CostAllocation] [int] NOT NULL,
	[id_Warehouse] [int] NOT NULL,
	[id_InventoryPeriodDetail] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CostAllocationDetail] ADD  DEFAULT ((0)) FOR [amountBox]
GO
ALTER TABLE [dbo].[CostAllocationDetail] ADD  DEFAULT ((0)) FOR [productionCost]
GO
ALTER TABLE [dbo].[CostAllocationDetail] ADD  DEFAULT ((0)) FOR [totalCost]
GO
ALTER TABLE [dbo].[CostAllocationDetail] ADD  DEFAULT ((0)) FOR [amountPound]
GO
ALTER TABLE [dbo].[CostAllocationDetail] ADD  DEFAULT ((0)) FOR [costPounds]
GO
ALTER TABLE [dbo].[CostAllocationDetail] ADD  DEFAULT ((0)) FOR [totalCostPounds]
GO
ALTER TABLE [dbo].[CostAllocationDetail] ADD  DEFAULT ((0)) FOR [amountKg]
GO
ALTER TABLE [dbo].[CostAllocationDetail] ADD  DEFAULT ((0)) FOR [costKg]
GO
ALTER TABLE [dbo].[CostAllocationDetail] ADD  DEFAULT ((0)) FOR [totalCostKg]
GO
ALTER TABLE [dbo].[CostAllocationResumido] ADD  DEFAULT ((0)) FOR [amountBox]
GO
ALTER TABLE [dbo].[CostAllocationResumido] ADD  DEFAULT ((0)) FOR [amountPound]
GO
ALTER TABLE [dbo].[CostAllocationResumido] ADD  DEFAULT ((0)) FOR [amountKg]
GO
ALTER TABLE [dbo].[CostAllocationResumido] ADD  DEFAULT ((0)) FOR [unitCostPounds]
GO
ALTER TABLE [dbo].[CostAllocationResumido] ADD  DEFAULT ((0)) FOR [unitCostKg]
GO
ALTER TABLE [dbo].[CostAllocationResumido] ADD  DEFAULT ((0)) FOR [averageCostUnit]
GO
ALTER TABLE [dbo].[CostAllocationResumido] ADD  DEFAULT ((0)) FOR [totalCostPounds]
GO
ALTER TABLE [dbo].[CostAllocationResumido] ADD  DEFAULT ((0)) FOR [totalCostKg]
GO
ALTER TABLE [dbo].[CostAllocationResumido] ADD  DEFAULT ((0)) FOR [totalCostUnit]
GO

ALTER TABLE [dbo].[CostAllocation]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocation_Document] FOREIGN KEY([id])
REFERENCES [dbo].[Document] ([id])
GO
ALTER TABLE [dbo].[CostAllocation] CHECK CONSTRAINT [FK_CostAllocation_Document]
GO
ALTER TABLE [dbo].[CostAllocationDetail]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationDetail_CostAllocation] FOREIGN KEY([id_CostAllocation])
REFERENCES [dbo].[CostAllocation] ([id])
GO
ALTER TABLE [dbo].[CostAllocationDetail] CHECK CONSTRAINT [FK_CostAllocationDetail_CostAllocation]
GO
ALTER TABLE [dbo].[CostAllocationDetail]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationDetail_InventoryMoveDetail] FOREIGN KEY([id_InventoryMoveDetail])
REFERENCES [dbo].[InventoryMoveDetail] ([id])
GO
ALTER TABLE [dbo].[CostAllocationDetail] CHECK CONSTRAINT [FK_CostAllocationDetail_InventoryMoveDetail]
GO
ALTER TABLE [dbo].[CostAllocationDetail]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationDetail_Item] FOREIGN KEY([id_Item])
REFERENCES [dbo].[Item] ([id])
GO
ALTER TABLE [dbo].[CostAllocationDetail] CHECK CONSTRAINT [FK_CostAllocationDetail_Item]
GO
ALTER TABLE [dbo].[CostAllocationDetail]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationDetail_Warehouse] FOREIGN KEY([id_Warehouse])
REFERENCES [dbo].[Warehouse] ([id])
GO
ALTER TABLE [dbo].[CostAllocationDetail] CHECK CONSTRAINT [FK_CostAllocationDetail_Warehouse]
GO
ALTER TABLE [dbo].[CostAllocationResumido]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationResumido_CostAllocation] FOREIGN KEY([id_CostAllocation])
REFERENCES [dbo].[CostAllocation] ([id])
GO
ALTER TABLE [dbo].[CostAllocationResumido] CHECK CONSTRAINT [FK_CostAllocationResumido_CostAllocation]
GO
ALTER TABLE [dbo].[CostAllocationResumido]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationResumido_InventoryLine] FOREIGN KEY([id_InventoryLine])
REFERENCES [dbo].[InventoryLine] ([id])
GO
ALTER TABLE [dbo].[CostAllocationResumido] CHECK CONSTRAINT [FK_CostAllocationResumido_InventoryLine]
GO
ALTER TABLE [dbo].[CostAllocationResumido]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationResumido_ItemType] FOREIGN KEY([id_ItemType])
REFERENCES [dbo].[ItemType] ([id])
GO
ALTER TABLE [dbo].[CostAllocationResumido] CHECK CONSTRAINT [FK_CostAllocationResumido_ItemType]
GO
ALTER TABLE [dbo].[CostAllocationResumido]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationResumido_ItemTypeCategory] FOREIGN KEY([id_ItemTypeCategory])
REFERENCES [dbo].[ItemTypeCategory] ([id])
GO
ALTER TABLE [dbo].[CostAllocationResumido] CHECK CONSTRAINT [FK_CostAllocationResumido_ItemTypeCategory]
GO
ALTER TABLE [dbo].[CostAllocationWarehouse]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationWarehouse_CostAllocation] FOREIGN KEY([id_CostAllocation])
REFERENCES [dbo].[CostAllocation] ([id])
GO
ALTER TABLE [dbo].[CostAllocationWarehouse] CHECK CONSTRAINT [FK_CostAllocationWarehouse_CostAllocation]
GO
ALTER TABLE [dbo].[CostAllocationWarehouse]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationWarehouse_InventoryPeriodDetail] FOREIGN KEY([id_InventoryPeriodDetail])
REFERENCES [dbo].[InventoryPeriodDetail] ([id])
GO
ALTER TABLE [dbo].[CostAllocationWarehouse] CHECK CONSTRAINT [FK_CostAllocationWarehouse_InventoryPeriodDetail]
GO
ALTER TABLE [dbo].[CostAllocationWarehouse]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocationWarehouse_Warehouse] FOREIGN KEY([id_Warehouse])
REFERENCES [dbo].[Warehouse] ([id])
GO
ALTER TABLE [dbo].[CostAllocationWarehouse] CHECK CONSTRAINT [FK_CostAllocationWarehouse_Warehouse]
GO



------------- FALTA ACTUALIZAR
------------- 

alter table InventoryMoveDetail
add productoCost [decimal](20, 6) default(0) NOT NULL;

alter table InventoryMoveDetail
add lastestProductoCost [decimal](20, 6)  default(0)  NOT NULL;

alter table InventoryMoveDetail
add [id_CostAllocationDetail] [int] NULL;

alter table InventoryMoveDetail
add [id_lastestCostAllocationDetail] [int] NULL;

alter table InventoryReason
add [isCostAllocation] [bit] NOT NULL;



--- Aprobacion Asignacion de Costos -- Tablas de Inventario Move Detail
create procedure [dbo].[AproveeCostAllocation]
@idCostAllocation int
as

Begin 
		set nocount on

		Update imd
		set imd.lastestProductoCost = imd.productoCost,
		    imd.id_lastestCostAllocationDetail =  imd.id_CostAllocationDetail,
			imd.productoCost = cad.productionCost,
			imd.id_CostAllocationDetail = cad.id
		From InventoryMoveDetail imd
		inner join CostAllocationDetail cad 
		on cad.id_InventoryMoveDetail = imd.id
		and cad.id_Item = imd.id_item
		Where cad.id_CostAllocation = @idCostAllocation;
End



--- Reverso Asignacion de Costos -- Tablas de Inventario Move Detail
create procedure [dbo].[ReverseCostAllocation]
@idCostAllocation int
as

Begin 
		set nocount on

		Update imd
		set imd.productoCost = imd.lastestProductoCost,
			imd.id_CostAllocationDetail = imd.id_lastestCostAllocationDetail,
			imd.lastestProductoCost= 0,
		    imd.id_lastestCostAllocationDetail=  null
		From InventoryMoveDetail imd
		inner join CostAllocationDetail cad 
		on cad.id = imd.id_CostAllocationDetail
		Where cad.id_CostAllocation = @idCostAllocation;
End


