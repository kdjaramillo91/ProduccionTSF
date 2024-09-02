using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Filter
{
    public class BaseFilterForm
    {
        public BaseFilterForm()
        {
            //this.octcodigo = "";
            this.ListFilterType = new List<FilterType>();
            //this.ListFilterText = new List<FilterText>();
            //this.ListFilterNumber = new List<FilterNumber>();
            //this.ListFilterSelect = new List<FilterSelect>();
        }


        //public string octcodigo { get; set; }

        public List<FilterType> ListFilterType { get; set; }
        //public List<FilterDate> ListFilterDate { get; set; }
        //public List<FilterDate> ListFilterDate { get; set; }
        //public List<FilterDate> ListFilterDate { get; set; }

        //public class FilterType
        //{
        //    public int id { get; set; }
        //    public string type { get; set; }//Date, Text, Number, Select, Check
        //    public string name { get; set; }
        //    public string alias { get; set; }
        //    public string nameDetail { get; set; }
        //    public string dataSource { get; set; }
        //}

        //public Phase phase { get; set; }
        //public string listId_BusinessOportunity { get; set; }
        //public int quantity { get; set; }
        //public decimal amountExpected { get; set; }
        //public decimal weightedAmount { get; set; }
        //public decimal percent { get; set; }

        
    }
}