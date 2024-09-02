using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ModelExtension
{
    internal class ItemIngredientCalculate
    {


        internal int idItem { get; set; }
        internal decimal amount { get; set; }
        internal int idMetricUnit { get; set; }

        internal List<ItemIngredientDetailCalculate> ItemIngredientDetail { get; set; }

        internal List<ItemIngredientCalculateFinal> ItemIngredientFinal { get; set; }
        internal ItemIngredientCalculate()
        {
            this.ItemIngredientDetail = new List<ItemIngredientDetailCalculate>();
            this.ItemIngredientFinal = new List<ItemIngredientCalculateFinal>();
        }


    }

    internal class ItemIngredientDetailCalculate
    {

        internal int idItemDetail { get; set; }
        internal int idMetricUnitDetail { get; set; }
        internal decimal amountDetail { get; set; }
        

    }

    internal class ItemIngredientCalculateFinal
    {

        internal int idItemCalculate { get; set; }
        internal int idMetricUnitCalculate { get; set; }
        internal decimal amountCalculate { get; set; }

    }

}