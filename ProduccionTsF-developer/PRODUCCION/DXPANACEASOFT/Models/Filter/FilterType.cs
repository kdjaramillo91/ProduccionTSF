using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Filter
{
    public class FilterType
    {
        public int id { get; set; }
        public string type { get; set; }//Date, Text, Number, Select, Check
        public string name { get; set; }
        public string alias { get; set; }
        public string nameDetail { get; set; }
        public string dataSource { get; set; }
            

    }
}