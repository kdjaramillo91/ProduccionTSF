using DXPANACEASOFT.Models.Dto;
using System;

namespace DXPANACEASOFT.Models.Helpers
{
    public static class InventoryMoveDetailHelper
    {

        public static InventoryMoveDetailDto ToDto(this InventoryMoveDetail input)
        {
            InventoryMoveDetailDto model = null;
            if (input == null) return model;
            model = new InventoryMoveDetailDto
            {
                id  = input.id,
                id_lot  = input.id_lot,
                id_item  = input.id_item,
                id_inventoryMove  = input.id_inventoryMove,
                entryAmount  = input.entryAmount,
                entryAmountCost  = input.entryAmountCost,
                exitAmount  = input.exitAmount,
                exitAmountCost  = input.exitAmountCost,
                id_metricUnit  = input.id_metricUnit,
                id_warehouse  = input.id_warehouse,
                id_warehouseLocation  = input.id_warehouseLocation,
                id_warehouseEntry  = input.id_warehouseEntry,
                id_inventoryMoveDetailExit  = input.id_inventoryMoveDetailExit,
                inMaximumUnit  = input.inMaximumUnit,
                id_userCreate  = input.id_userCreate,
                dateCreate  = input.dateCreate,
                id_userUpdate  = input.id_userUpdate,
                dateUpdate  = input.dateUpdate,
                id_inventoryMoveDetailPrevious  = input.id_inventoryMoveDetailPrevious,
                id_inventoryMoveDetailNext  = input.id_inventoryMoveDetailNext,
                unitPrice  = input.unitPrice,
                balance  = input.balance,
                averagePrice  = input.averagePrice,
                balanceCost  = input.balanceCost,
                id_metricUnitMove  = input.id_metricUnitMove,
                unitPriceMove  = input.unitPriceMove,
                amountMove  = input.amountMove,
                id_costCenter  = input.id_costCenter,
                id_subCostCenter  = input.id_subCostCenter,
                natureSequential  = input.natureSequential,
                genSecTrans  = input.genSecTrans,
                id_warehouseLocationEntry  = input.id_warehouseLocationEntry,
                id_costCenterEntry  = input.id_costCenterEntry,
                id_subCostCenterEntry  = input.id_subCostCenterEntry,
                id_productionCart  = input.id_productionCart,
                ordenProduccion  = input.ordenProduccion,
                productoCost  = input.productoCost,
                lastestProductoCost  = input.lastestProductoCost,
                id_CostAllocationDetail  = input.id_CostAllocationDetail,
                id_lastestCostAllocationDetail  = input.id_lastestCostAllocationDetail,
                lotMarked  = input.lotMarked,
                id_personProcessPlant  = input.id_personProcessPlant
            };

            return model;
        }
         

    }
}