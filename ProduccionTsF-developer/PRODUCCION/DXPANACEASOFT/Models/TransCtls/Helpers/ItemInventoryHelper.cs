using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Models.DTOModel;
using System;

namespace DXPANACEASOFT.Models.Helpers
{

    public static class ItemInventoryHelper
    {
        public static ItemInventoryDto ToDto(this ItemInventory input)
        {
            ItemInventoryDto model = null;
            if (input == null) return model;

            model = new ItemInventoryDto
            {

                id_item = input.id_item,
                id_inventoryControlType = input.id_inventoryControlType,
                id_valueValuationMethod = input.id_valueValuationMethod,
                isImported   = input.isImported,
                id_warehouse = input.id_warehouse,
                id_warehouseLocation = input.id_warehouseLocation,
                minimumStock = input.minimumStock,
                maximumStock = input.maximumStock,
                currentStock = input.currentStock,
                id_metricUnitInventory = input.id_metricUnitInventory,
                requiresLot = input.requiresLot
            };
            return model;
        }
    }
}