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
    
    public partial class tbsysObjSecurityRecord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbsysObjSecurityRecord()
        {
            this.tbsysUserRecordSecurity = new HashSet<tbsysUserRecordSecurity>();
        }
    
        public int id { get; set; }
        public string obj { get; set; }
        public string field { get; set; }
        public string objFilter { get; set; }
        public string fieldFilter { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbsysUserRecordSecurity> tbsysUserRecordSecurity { get; set; }
    }
}
