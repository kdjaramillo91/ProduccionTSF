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
    
    public partial class QualityControlResultConformity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QualityControlResultConformity()
        {
            this.QualityControlResultConformityOnHeaderValue = new HashSet<QualityControlResultConformityOnHeaderValue>();
        }
    
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Nullable<bool> isSystem { get; set; }
        public Nullable<int> id_Company { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<bool> isConforms { get; set; }
        public Nullable<int> id_userCreate { get; set; }
        public Nullable<System.DateTime> dateCreate { get; set; }
        public Nullable<int> id_userUpdate { get; set; }
        public Nullable<System.DateTime> dateUpdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QualityControlResultConformityOnHeaderValue> QualityControlResultConformityOnHeaderValue { get; set; }
    }
}
