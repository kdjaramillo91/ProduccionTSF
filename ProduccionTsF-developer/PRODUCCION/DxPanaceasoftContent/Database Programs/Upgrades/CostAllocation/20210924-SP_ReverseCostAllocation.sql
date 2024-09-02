--- Reverso Asignacion de Costos -- Tablas de Inventario Move Detail
create procedure ReverseCostAllocation
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
