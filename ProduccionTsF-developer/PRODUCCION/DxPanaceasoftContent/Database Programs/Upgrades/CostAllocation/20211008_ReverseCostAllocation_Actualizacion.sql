USE [Produccion_2019]
GO
/****** Object:  StoredProcedure [dbo].[ReverseCostAllocation]    Script Date: 10/8/2021 8:14:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[ReverseCostAllocation]
@idCostAllocation int
as

Begin 
		set nocount on

		Update imd
		set imd.productoCost = imd.lastestProductoCost,
			imd.id_CostAllocationDetail = imd.id_lastestCostAllocationDetail,
			imd.lastestProductoCost= 0,
		    imd.id_lastestCostAllocationDetail=  null,
			imd.unitPriceMove = 0
		From InventoryMoveDetail imd
		inner join CostAllocationDetail cad 
		on cad.id = imd.id_CostAllocationDetail
		Where cad.id_CostAllocation = @idCostAllocation;
End