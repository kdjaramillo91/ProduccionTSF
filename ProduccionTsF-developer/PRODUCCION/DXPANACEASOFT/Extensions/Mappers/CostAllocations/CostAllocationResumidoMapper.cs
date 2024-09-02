using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Extensions
{
    public static class CostAllocationResumidoMapper
    {
        public static CostAllocationResumidoDto ToDto(this CostAllocationResumido input)
        {
            if (input == null) return new CostAllocationResumidoDto();
            
            return new CostAllocationResumidoDto
            {
                id = input.id,
                  
                amountBox = input.amountBox,
                amountKg = input.amountKg,
                amountPound = input.amountPound,
                averageCostUnit = input.averageCostUnit,
                id_CostAllocation = input.id_CostAllocation,
                id_InventoryLine = input.id_InventoryLine,
                id_ItemType = input.id_ItemType,
                id_ItemTypeCategory = input.id_ItemTypeCategory,
                id_ItemSize = input.id_ItemSize,
                //ItemType
                //ItemTypeCategory
                totalCostKg = input.totalCostKg,
                totalCostPounds= input.totalCostPounds,
                totalCostUnit = input.totalCostUnit,
                unitCostKg = input.unitCostKg,
                unitCostPounds = input.unitCostPounds,
                InventoryLineName = (input.InventoryLine != null) ? input.InventoryLine.name : "",
                ItemTypeName = (input.ItemType != null) ? input.ItemType.name : "",
                ItemTypeCategoryName = (input.ItemTypeCategory != null) ? input.ItemTypeCategory.name : "",
                ItemSizeName = (input.ItemSize != null) ? input.ItemSize.name : "",

            };
        }
    }
}