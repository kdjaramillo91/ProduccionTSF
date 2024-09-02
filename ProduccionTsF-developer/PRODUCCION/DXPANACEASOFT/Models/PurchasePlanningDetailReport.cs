using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class PurchasePlanningDetailReport
    {
        public string datePlanningStr { get; set; }
        public DateTime datePlanning { get; set; }
        public int id_provider { get; set; }
        public string provider { get; set; }
        public int id_buyer { get; set; }
        public string buyer { get; set; }
        public int id_item { get; set; }
        public string item { get; set; }
        public int id_itemTypeCategory { get; set; }
        public string itemTypeCategory { get; set; }
        public decimal quantity { get; set; }

    }
}