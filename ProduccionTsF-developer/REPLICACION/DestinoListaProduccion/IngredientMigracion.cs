//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DestinoListaProduccion
{
    using System;
    using System.Collections.Generic;
    
    public partial class IngredientMigracion
    {
        public int id { get; set; }
        public int id_compoundItem { get; set; }
        public int id_ingredientItem { get; set; }
        public Nullable<int> id_metricUnit { get; set; }
        public Nullable<decimal> amount { get; set; }
        public Nullable<decimal> amountMax { get; set; }
        public Nullable<int> id_metricUnitMax { get; set; }
        public bool manual { get; set; }
    }
}
