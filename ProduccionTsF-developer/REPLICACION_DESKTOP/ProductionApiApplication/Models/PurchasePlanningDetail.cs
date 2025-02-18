//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProductionApiApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchasePlanningDetail
    {
        public int id { get; set; }
        public int id_purchasePlanning { get; set; }
        public int id_provider { get; set; }
        public int id_buyer { get; set; }
        public Nullable<int> id_item { get; set; }
        public int id_itemTypeCategory { get; set; }
        public decimal quantity { get; set; }
        public System.DateTime datePlanning { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual ItemTypeCategory ItemTypeCategory { get; set; }
        public virtual Person Person { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual PurchasePlanning PurchasePlanning { get; set; }
    }
}
