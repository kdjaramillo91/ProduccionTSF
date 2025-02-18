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
    
    public partial class ProductionScheduleRequestDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductionScheduleRequestDetail()
        {
            this.ProductionScheduleInventoryReservationDetail = new HashSet<ProductionScheduleInventoryReservationDetail>();
            this.ProductionScheduleProductionOrderDetail = new HashSet<ProductionScheduleProductionOrderDetail>();
            this.SalesRequestOrQuotationDetailProductionScheduleRequestDetail = new HashSet<SalesRequestOrQuotationDetailProductionScheduleRequestDetail>();
        }
    
        public int id { get; set; }
        public int id_productionSchedule { get; set; }
        public int id_item { get; set; }
        public decimal quantityRequest { get; set; }
        public decimal quantitySchedule { get; set; }
        public int id_metricUnit { get; set; }
        public decimal quantitySale { get; set; }
        public bool reservedInInventory { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual MetricUnit MetricUnit { get; set; }
        public virtual ProductionSchedule ProductionSchedule { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionScheduleInventoryReservationDetail> ProductionScheduleInventoryReservationDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionScheduleProductionOrderDetail> ProductionScheduleProductionOrderDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesRequestOrQuotationDetailProductionScheduleRequestDetail> SalesRequestOrQuotationDetailProductionScheduleRequestDetail { get; set; }
    }
}
