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
    
    public partial class RemissionGuideRiverCustomizedAdvancedTransportist
    {
        public int id_RemissionGuideRiver { get; set; }
        public int id_AdvancedTransportist { get; set; }
        public Nullable<System.DateTime> dateApproved { get; set; }
        public Nullable<int> id_UserApproved { get; set; }
        public decimal valueTotal { get; set; }
        public bool hasPayment { get; set; }
        public bool isActive { get; set; }
        public Nullable<int> id_PaymentState { get; set; }
    
        public virtual Document Document { get; set; }
        public virtual RemissionGuideRiver RemissionGuideRiver { get; set; }
        public virtual tbsysCatalogState tbsysCatalogState { get; set; }
    }
}
