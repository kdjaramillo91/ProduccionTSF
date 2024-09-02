using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.InventoryMoveDTO.InventoryMovePlantTransfer
{
    public class ResultInventoryMovePlantTransferDTO
    {
        public string warehouseLocation { get; set; }
        public int id_Lot { get; set; }
        public DateTime dateTimeEntry { get; set; }
        public string numberLot { get; set; }
        public string car { get; set; }
        public decimal quantity { get; set; }
        public string metricUnit { get; set; }
        public string masterCode { get; set; }
        public string nameItem { get; set; }

    }
}