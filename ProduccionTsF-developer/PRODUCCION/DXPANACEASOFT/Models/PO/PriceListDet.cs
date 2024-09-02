
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.PO
{
    public class PriceListDet
    {
        public int Id_Itemsize { get; set; }

        public decimal? Price { get; set; }
        public decimal? PriceA { get; set; }

        public decimal? PriceB { get; set; }
        public int? idClass { get; set; }
        public string codeClass { get; set; }
        public int? idClassShrimp { get; set; }
        public string codeClassShrimp { get; set; }

        public int? id_processtype { get; set; }
        public string code_ProcessType { get; set; }
        public string name_ProcessType { get; set; }
    }

    //public class 
}