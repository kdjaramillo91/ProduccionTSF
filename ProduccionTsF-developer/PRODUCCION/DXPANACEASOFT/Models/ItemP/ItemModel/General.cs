
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ItemP.ItemModel
{
    public class ItemModelP
    {
        public int idModelP { get; set; }
        public string masterCodeModelP { get; set; }
        public string auxCodeModelP { get; set; }
        public string nameModelP { get; set; }
        public string descriptionModelP { get; set; }
        public decimal?  quantity { get; set; }

    }
    public class ItemInventoryModelP
    {
        public int idItemModelP { get; set; }
        public int idWaresHouseModelP { get; set; }
        public int idWarehouseLocationModelP { get; set; }

        public int idMetricUnitInventoryModelP { get; set; }
        
    }

  
}