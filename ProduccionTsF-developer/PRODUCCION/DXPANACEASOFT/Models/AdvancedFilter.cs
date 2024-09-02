using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class AdvancedFilter
    {
        public int? id_attribute1 { get; set; }

        public int? id_comparisonOperator { get; set; }

        public int? id_condition { get; set; }

        public DateTime conditionDate { get; set; }

        public string condition { get; set; }

        public decimal? conditionNum { get; set; }

        public int? id_logical { get; set; }

        public string query { get; set; }

    }
}