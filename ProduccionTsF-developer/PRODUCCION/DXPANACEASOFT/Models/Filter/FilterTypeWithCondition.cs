using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Filter
{
    public class FilterTypeWithCondition
    {
        public int id { get; set; }
        public FilterType filterType { get; set; }//Date, Text, Number, Select, Check
        public int id_logicalOperator { get; set; }
        public LogicalOperator logicalOperator { get; set; }
        public DateTime? valueConditionFromDateTime { get; set; }
        public DateTime? valueConditionToDateTime { get; set; }
        public decimal? valueConditionFromDecimal { get; set; }
        public decimal? valueConditionToDecimal { get; set; }
        public string valueConditionTextOrSelect { get; set; }
        public bool? valueConditionCheck { get; set; }

        public int[] valueConditionSelectValue { get; set; }

        public string valueConditionSelectValueText { get; set; }

    }
}