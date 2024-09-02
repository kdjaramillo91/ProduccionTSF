
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.QualityControls
{
    public class QualityControlTallaDto
    {
        public int id { get; set; }
        public int id_QualityControl { get; set; }
        public int id_ProcessType { get; set; }
        public string sProcessType { get; set; }
        public int id_itemsize { get; set; }
        public int Order { get; set; }
        public string sItemSize { get; set; }
        public int id_Class { get; set; }
        public string codeClass { get; set; }
        public string nameClass { get; set; }
        public Nullable<decimal> poundsDetail { get; set; }
        public Nullable<decimal> porcentaje { get; set; }


    }
}