//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DXPANACEASOFT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductionCostProductValuation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductionCostProductValuation()
        {
            this.ProductionCostProductValuationExecutions = new HashSet<ProductionCostProductValuationExecution>();
            this.ProductionCostProductValuationWarehouse = new HashSet<ProductionCostProductValuationWarehouse>();
            this.ProductionCostProductValuationInventoryMove = new HashSet<ProductionCostProductValuationInventoryMove>();
            this.ProductionCostProductValuationWarning = new HashSet<ProductionCostProductValuationWarning>();
            this.ProductionCostProductValuationBalance = new HashSet<ProductionCostProductValuationBalance>();
        }
    
        public int id { get; set; }
        public int id_company { get; set; }
        public int id_allocationType { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public bool processed { get; set; }
        public string description { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public Nullable<System.DateTime> fechaInicio { get; set; }
        public Nullable<System.DateTime> fechaFin { get; set; }
        public Nullable<int> idProducto { get; set; }
    
        public virtual Document Document { get; set; }
        public virtual ProductionCostAllocationType ProductionCostAllocationType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionCostProductValuationExecution> ProductionCostProductValuationExecutions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionCostProductValuationWarehouse> ProductionCostProductValuationWarehouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionCostProductValuationInventoryMove> ProductionCostProductValuationInventoryMove { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionCostProductValuationWarning> ProductionCostProductValuationWarning { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionCostProductValuationBalance> ProductionCostProductValuationBalance { get; set; }
    }
}
