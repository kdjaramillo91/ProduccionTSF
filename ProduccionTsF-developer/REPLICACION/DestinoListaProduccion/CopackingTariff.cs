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
    
    public partial class CopackingTariff
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CopackingTariff()
        {
            this.CopackingTariffDetail = new HashSet<CopackingTariffDetail>();
            this.ProductionLot = new HashSet<ProductionLot>();
        }
    
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int id_provider { get; set; }
        public System.DateTime dateInit { get; set; }
        public System.DateTime dateEnd { get; set; }
        public bool isActive { get; set; }
        public int id_company { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CopackingTariffDetail> CopackingTariffDetail { get; set; }
        public virtual Person Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionLot> ProductionLot { get; set; }
    }
}
