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
    
    public partial class SalesOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalesOrder()
        {
            this.Invoice = new HashSet<Invoice>();
            this.LiquidationCartOnCartDetail = new HashSet<LiquidationCartOnCartDetail>();
            this.MasteredDetail = new HashSet<MasteredDetail>();
            this.ProductionLotLiquidation = new HashSet<ProductionLotLiquidation>();
            this.ProductionLotLiquidationTotal = new HashSet<ProductionLotLiquidationTotal>();
            this.SalesOrderDetail = new HashSet<SalesOrderDetail>();
            this.SalesOrderDetailInstructions = new HashSet<SalesOrderDetailInstructions>();
            this.SalesOrderMPMaterialDetail = new HashSet<SalesOrderMPMaterialDetail>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_customer { get; set; }
        public Nullable<int> id_employeeSeller { get; set; }
        public Nullable<int> id_priceList { get; set; }
        public bool requiredLogistic { get; set; }
        public Nullable<int> id_PaymentMethod { get; set; }
        public Nullable<System.DateTime> dateShipment { get; set; }
        public Nullable<int> id_portDestination { get; set; }
        public Nullable<int> id_portDischarge { get; set; }
        public Nullable<int> id_employeeApplicant { get; set; }
        public string numeroLote { get; set; }
        public Nullable<int> id_orderReason { get; set; }
        public Nullable<int> id_provider { get; set; }
        public string additionalInstructions { get; set; }
        public string shippingDocument { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Document Document { get; set; }
        public virtual Employee Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoice { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LiquidationCartOnCartDetail> LiquidationCartOnCartDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MasteredDetail> MasteredDetail { get; set; }
        public virtual OrderReason OrderReason { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual Person Person { get; set; }
        public virtual Person Person1 { get; set; }
        public virtual Port Port { get; set; }
        public virtual Port Port1 { get; set; }
        public virtual PriceList PriceList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionLotLiquidation> ProductionLotLiquidation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionLotLiquidationTotal> ProductionLotLiquidationTotal { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesOrderDetail> SalesOrderDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesOrderDetailInstructions> SalesOrderDetailInstructions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesOrderMPMaterialDetail> SalesOrderMPMaterialDetail { get; set; }
        public virtual SalesOrderTotal SalesOrderTotal { get; set; }
    }
}
