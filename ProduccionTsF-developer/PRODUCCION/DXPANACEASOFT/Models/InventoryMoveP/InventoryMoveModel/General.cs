
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.InventoryMoveP.InventoryMoveModel
{
    public class InventoryMoveModelP
    {
        public int idInventoryMoveModelP { get; set; }

        public int? idInventoryReasonModelP { get; set; }

        public int? idProductionLot { get; set; }
    
        public int? idInventoryMoveToReverse { get; set; }

        public string natureSequential { get; set; }

        public int iSequential { get; set; }
    }

    public class InventoryMoveDetailModelP
    {
        public int id { get; set; }
        public int? id_lot { get; set; }
        public int id_item { get; set; }
        public int id_inventoryMove { get; set; }
        public decimal entryAmount { get; set; }
        public decimal entryAmountCost { get; set; }
        public decimal exitAmount { get; set; }
        public decimal exitAmountCost { get; set; }
        public int id_metricUnit { get; set; }
        public int id_warehouse { get; set; }
        public int? id_warehouseLocation { get; set; }
        public int? id_warehouseEntry { get; set; }
        public int? id_inventoryMoveDetailExit { get; set; }
        public bool inMaximumUnit { get; set; }
        public int id_userCreate { get; set; }
        public DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public DateTime dateUpdate { get; set; }
        public int? id_inventoryMoveDetailPrevious { get; set; }
        public int? id_inventoryMoveDetailNext { get; set; }
        public decimal unitPrice { get; set; }
        public decimal balance { get; set; }
        public decimal averagePrice { get; set; }
        public decimal balanceCost { get; set; }
        public int? id_metricUnitMove { get; set; }
        public decimal? unitPriceMove { get; set; }
        public decimal? amountMove { get; set; }
        public int? id_costCenter { get; set; }
        public int? id_subCostCenter { get; set; }
        public string natureSequential { get; set; }

        public int? iSequential { get; set; }

        public int? idNatureMove { get; set; }
        public string lotMarked { get; set; }
    }
    public class InventoryMoveDetailCabModelP
    {
        public int id { get; set; }
        public int? id_lot { get; set; }
        public int id_item { get; set; }
        public int id_inventoryMove { get; set; }
        public DateTime emissionDate { get; set; }
        public decimal entryAmount { get; set; }
        public decimal entryAmountCost { get; set; }
        public decimal exitAmount { get; set; }
        public decimal exitAmountCost { get; set; }
        public int id_metricUnit { get; set; }
        public int id_warehouse { get; set; }
        public int? id_warehouseLocation { get; set; }
        public int? id_warehouseEntry { get; set; }
        public int? id_inventoryMoveDetailExit { get; set; }
        public bool inMaximumUnit { get; set; }
        public int id_userCreate { get; set; }
        public DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public DateTime dateUpdate { get; set; }
        public int? id_inventoryMoveDetailPrevious { get; set; }
        public int? id_inventoryMoveDetailNext { get; set; }
        public decimal unitPrice { get; set; }

        public decimal? previousBalance { get; set; }
        public decimal balance { get; set; }
        public decimal averagePrice { get; set; }
        public decimal balanceCost { get; set; }
        public int? id_metricUnitMove { get; set; }
        public decimal? unitPriceMove { get; set; }
        public decimal? amountMove { get; set; }
        public int? id_costCenter { get; set; }
        public int? id_subCostCenter { get; set; }
        public string natureSequential { get; set; }

        public decimal quantityMove { get; set; }
        public int? iSequential { get; set; }

        public int? idNatureMove { get; set; }

    }

    public class InventoryReasonModelP
    {
        public int idInventoryReasonModelP { get; set; }
        public string codeInventoryReasonModelP { get; set; }
        public string nameInventoryReasonModelP { get; set; }
        public string descriptionInventoryReasonModelP { get; set; }
        public int id_documentTypeInventoryReasonModelP { get; set; }
        public int idNatureMoveInventoryReasonModelP { get; set; }
    }

    public class CustomLot : Lot
    {
        public DateTime? FechaLote { get; set; }
        public string FechaLoteStr { get; set; }
        public decimal Saldo { get; set; }
    }
}
  
