using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Dto
{
    public class CostAllocationDetalleDto
    {
        public int id { get; set; }
        public int id_CostAllocation { get; set; }
        public int id_Item { get; set; }
        public int id_metricUnitMove { get; set; }
        public int id_InventoryMoveDetail { get; set; }
        public int id_WarehouseLocation { get; set; }
        public int? id_Lot { get; set; }
        public decimal amountBox { get; set; }
        public decimal productionCost { get; set; }
        public decimal totalCost { get; set; }
        public decimal amountPound { get; set; }
        public decimal costPounds { get; set; }
        public decimal totalCostPounds { get; set; }
        public decimal amountKg { get; set; }
        public decimal costKg { get; set; }
        public decimal totalCostKg { get; set; }

        //-------------------------
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseLocationName { get; set; }
        public int InventoryLineId { get; set; }
        public string InventoryLineName { get; set; }
        public int ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public int ItemTypeCategoryId { get; set; }
        public string ItemTypeCategoryName { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public int PresentationId { get; set; }
        public string PresentationName { get; set; }
        public int ItemSizeId { get; set; }
        public string ItemSizeName { get; set; }
        public int ItemTrademarkId { get; set; }
        public string ItemTrademarkName { get; set; }
        public int ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }
        public int ItemSubGroupId { get; set; }
        public string ItemSubGroupName { get; set; }
        public int ItemTrademarkModelId { get; set; }
        public string ItemTrademarkModelName { get; set; }
        public string LotNumber { get; set; }
        public string InventaryNumber { get; set; }
        public DateTime DateMovement { get; set; }


        public int MotivoInventarioId { get; set; }
        public string MotivoInventarioName { get; set; }

        public string NaturalezaMovimeinto { get; set; }


    }
}