using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Dto
{
    public class CostAllocationResumidoDto
    {
        public int id { get; set; }
        public int id_CostAllocation { get; set; }
        public int id_InventoryLine { get; set; }
        public int id_ItemType { get; set; }
        public int id_ItemTypeCategory { get; set; }
        public decimal amountBox { get; set; }
        public decimal amountPound { get; set; }
        public decimal amountKg { get; set; }
        public decimal unitCostPounds { get; set; }
        public decimal unitCostKg { get; set; }
        public decimal averageCostUnit { get; set; }
        public decimal totalCostPounds { get; set; }
        public decimal totalCostKg { get; set; }
        public decimal totalCostUnit { get; set; }

        public string InventoryLineName { get; set; }

        public string ItemTypeName { get; set; }

        public string ItemTypeCategoryName { get; set; }
        public int id_ItemSize { get; set; }
        public string ItemSizeName { get; set; }

    }
     
}