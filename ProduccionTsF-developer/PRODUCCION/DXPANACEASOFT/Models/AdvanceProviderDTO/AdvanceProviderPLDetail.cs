
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.AdvanceProviderDTO
{
    public class AdvanceProviderPLDetail
    {
        public int id_processType { get; set; }

        public int id_itemSize { get; set; }

        public int id_class { get; set; }

        public decimal value { get; set; }

        public decimal price { get; set; }

        public decimal valuePounds { get; set; }
    }
}