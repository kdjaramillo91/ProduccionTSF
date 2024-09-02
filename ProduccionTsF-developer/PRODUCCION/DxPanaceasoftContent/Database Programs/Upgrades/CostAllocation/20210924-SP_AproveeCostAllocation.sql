--- Aprobacion Asignacion de Costos -- Tablas de Inventario Move Detail
create procedure AproveeCostAllocation
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
