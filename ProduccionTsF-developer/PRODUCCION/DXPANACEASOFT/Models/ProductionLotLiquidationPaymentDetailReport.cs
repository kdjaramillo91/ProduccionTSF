using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class ProductionLotLiquidationPaymentDetailReport
    {
        public int id_itemType { get; set; }
        public string itemType { get; set; }
        public int? id_groupCategory { get; set; }
        public string groupCategory { get; set; }
        public int? id_size { get; set; }
        public string size { get; set; }
        public int orderSize { get; set; }
        public decimal price { get; set; }
        public decimal totalMU { get; set; }
        public decimal totalToPay { get; set; }

    }
}