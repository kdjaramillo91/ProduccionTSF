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
    
    public partial class ProductionCostAllocationPeriodDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductionCostAllocationPeriodDetail()
        {
            this.ProductionCostCoefficientExecutionDetail = new HashSet<ProductionCostCoefficientExecutionDetail>();
        }
    
        public int id { get; set; }
        public int id_allocationPeriod { get; set; }
        public int id_productionCost { get; set; }
        public int id_productionCostDetail { get; set; }
        public Nullable<int> id_productionPlant { get; set; }
        public bool coeficiente { get; set; }
        public decimal valor { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
    
        public virtual Person Person { get; set; }
        public virtual ProductionCost ProductionCost { get; set; }
        public virtual ProductionCostAllocationPeriod ProductionCostAllocationPeriod { get; set; }
        public virtual ProductionCostDetail ProductionCostDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionCostCoefficientExecutionDetail> ProductionCostCoefficientExecutionDetail { get; set; }
    }
}
