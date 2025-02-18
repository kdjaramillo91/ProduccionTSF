USE [Produccion_2019]
GO
/****** Object:  StoredProcedure [dbo].[AproveeCostAllocation]    Script Date: 10/8/2021 8:14:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[AproveeCostAllocation]
@idCostAllocation int
as

Begin 
		set nocount on

		Update imd
		set imd.lastestProductoCost = imd.productoCost,
		    imd.id_lastestCostAllocationDetail =  imd.id_CostAllocationDetail,
			imd.productoCost = cad.productionCost,
			imd.id_CostAllocationDetail = cad.id,
			imd.unitPriceMove = cad.productionCost
		From InventoryMoveDetail imd
		inner join CostAllocationDetail cad 
		on cad.id_InventoryMoveDetail = imd.id
		and cad.id_Item = imd.id_item
		Where cad.id_CostAllocation = @idCostAllocation;
End