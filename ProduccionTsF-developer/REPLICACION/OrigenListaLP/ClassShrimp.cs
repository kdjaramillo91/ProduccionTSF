//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrigenListaLP
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClassShrimp
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClassShrimp()
        {
            this.PenalityClassShrimpDetails = new HashSet<PenalityClassShrimpDetails>();
            this.PriceListClassShrimp = new HashSet<PriceListClassShrimp>();
            this.PriceListItemSizeDetail = new HashSet<PriceListItemSizeDetail>();
        }
    
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool isActive { get; set; }
        public int order { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PenalityClassShrimpDetails> PenalityClassShrimpDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceListClassShrimp> PriceListClassShrimp { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceListItemSizeDetail> PriceListItemSizeDetail { get; set; }
    }
}
