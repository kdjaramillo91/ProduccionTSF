
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Models.PriceListDet
{
    public class PriceListDetItemSize
    {
        public int id { get; set; }

        public int id_ProcessType { get; set; }

        public string sProcessType { get; set; }

        public int Id_Itemsize { get; set; }

        public string sItemSize { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceA { get; set; }

        public decimal? PriceB { get; set; }

        public int Order { get; set; }

        public int id_Class { get; set; }
        public string codeClass { get; set; }
        public string nameClass { get; set; }
        public Nullable<decimal> Libras { get; set; }
        public Nullable<decimal> Total { get; set; }
    }
}