using System;

namespace DXPANACEASOFT.Models.Dto
{

    public class ItemInventoryDto
    {

        public int id_item { get; set; }
        public Nullable<int> id_inventoryControlType { get; set; }
        public Nullable<int> id_valueValuationMethod { get; set; }
        public bool isImported { get; set; }
        public int id_warehouse { get; set; }
        public int id_warehouseLocation { get; set; }
        public decimal minimumStock { get; set; }
        public decimal maximumStock { get; set; }
        public decimal currentStock { get; set; }
        public int id_metricUnitInventory { get; set; }
        public bool requiresLot { get; set; }
    }

}