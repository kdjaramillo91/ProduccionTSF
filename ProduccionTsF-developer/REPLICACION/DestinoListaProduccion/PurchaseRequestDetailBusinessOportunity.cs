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
    
    public partial class PurchaseRequestDetailBusinessOportunity
    {
        public int id { get; set; }
        public int id_purchaseRequestDetail { get; set; }
        public Nullable<int> id_businessOportunityPlanningDetail { get; set; }
        public int id_businessOportunity { get; set; }
        public decimal quantity { get; set; }
    
        public virtual BusinessOportunity BusinessOportunity { get; set; }
        public virtual BusinessOportunityPlanningDetail BusinessOportunityPlanningDetail { get; set; }
        public virtual PurchaseRequestDetail PurchaseRequestDetail { get; set; }
    }
}
