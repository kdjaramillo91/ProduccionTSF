using System;

namespace DXPANACEASOFT.Models.Dto
{
    public class InventoryMoveDetailDto
    {
        public int id { get; set; }
        public Nullable<int> id_lot { get; set; }
        public int id_item { get; set; }
        public int id_inventoryMove { get; set; }
        public decimal entryAmount { get; set; }
        public decimal entryAmountCost { get; set; }
        public decimal exitAmount { get; set; }
        public decimal exitAmountCost { get; set; }
        public int id_metricUnit { get; set; }
        public int id_warehouse { get; set; }
        public Nullable<int> id_warehouseLocation { get; set; }
        public Nullable<int> id_warehouseEntry { get; set; }
        public Nullable<int> id_inventoryMoveDetailExit { get; set; }
        public bool inMaximumUnit { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public Nullable<int> id_inventoryMoveDetailPrevious { get; set; }
        public Nullable<int> id_inventoryMoveDetailNext { get; set; }
        public decimal unitPrice { get; set; }
        public decimal balance { get; set; }
        public decimal averagePrice { get; set; }
        public decimal balanceCost { get; set; }
        public Nullable<int> id_metricUnitMove { get; set; }
        public Nullable<decimal> unitPriceMove { get; set; }
        public Nullable<decimal> amountMove { get; set; }
        public Nullable<int> id_costCenter { get; set; }
        public Nullable<int> id_subCostCenter { get; set; }
        public string natureSequential { get; set; }
        public bool genSecTrans { get; set; }
        public Nullable<int> id_warehouseLocationEntry { get; set; }
        public Nullable<int> id_costCenterEntry { get; set; }
        public Nullable<int> id_subCostCenterEntry { get; set; }
        public Nullable<int> id_productionCart { get; set; }
        public string ordenProduccion { get; set; }
        public decimal productoCost { get; set; }
        public decimal lastestProductoCost { get; set; }
        public Nullable<int> id_CostAllocationDetail { get; set; }
        public Nullable<int> id_lastestCostAllocationDetail { get; set; }
        public string lotMarked { get; set; }
        public Nullable<int> id_personProcessPlant { get; set; }
    }
}
