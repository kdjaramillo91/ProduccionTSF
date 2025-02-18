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
    
    public partial class RemissionGuideRiver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RemissionGuideRiver()
        {
            this.RemissionGuideRiverDetail = new HashSet<RemissionGuideRiverDetail>();
        }
    
        public int id { get; set; }
        public int id_reciver { get; set; }
        public int id_reason { get; set; }
        public string route { get; set; }
        public string startAdress { get; set; }
        public System.DateTime despachureDate { get; set; }
        public Nullable<System.TimeSpan> despachurehour { get; set; }
        public Nullable<int> id_providerRemisionGuideRiver { get; set; }
        public Nullable<int> id_productionUnitProvider { get; set; }
        public Nullable<int> id_shippingType { get; set; }
        public Nullable<int> id_TransportTariffType { get; set; }
    
        public virtual Document Document { get; set; }
        public virtual Person Person { get; set; }
        public virtual ProductionUnitProvider ProductionUnitProvider { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual PurchaseOrderShippingType PurchaseOrderShippingType { get; set; }
        public virtual RemissionGuideReason RemissionGuideReason { get; set; }
        public virtual RemissionGuideRiverTransportation RemissionGuideRiverTransportation { get; set; }
        public virtual TransportTariffType TransportTariffType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RemissionGuideRiverDetail> RemissionGuideRiverDetail { get; set; }
    }
}
