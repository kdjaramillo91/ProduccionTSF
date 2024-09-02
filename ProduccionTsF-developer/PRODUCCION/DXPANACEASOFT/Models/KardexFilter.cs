using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class KardexFilter
    {
        public InventoryMoveDetail inventoryMoveDetail { get; set; }
        //public InventoryEntryMove entryMove { get; set; }
        //public InventoryExitMove exitMove { get; set; }
        //public Document document { get; set; }
        public DateTime? startEmissionDate { get; set; }
        public DateTime? endEmissionDate { get; set; }
        //public string numberLot { get; set; }
        //public string internalNumberLot { get; set; }
        public int[] items { get; set; }

        public GridViewSettings settingGridViewKardex { get; set; }

    }
}