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
    
    public partial class ResultProdLotRomaneo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ResultProdLotRomaneo()
        {
            this.Romaneo = new HashSet<Romaneo>();
        }
    
        public int idProductionLot { get; set; }
        public string numberLot { get; set; }
        public string numberLotSequential { get; set; }
        public string nameProvider { get; set; }
        public string nameProviderShrimp { get; set; }
        public string namePool { get; set; }
        public string INPnumber { get; set; }
        public System.DateTime dateTimeReception { get; set; }
        public string nameItem { get; set; }
        public string nameWarehouseItem { get; set; }
        public string nameWarehouseLocationItem { get; set; }
        public string codeProcessType { get; set; }
        public decimal quantityRemitted { get; set; }
        public int idMetricUnit { get; set; }
        public int gavetaNumber { get; set; }
    
        public virtual MetricUnit MetricUnit { get; set; }
        public virtual ProductionLot ProductionLot { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Romaneo> Romaneo { get; set; }
    }
}
