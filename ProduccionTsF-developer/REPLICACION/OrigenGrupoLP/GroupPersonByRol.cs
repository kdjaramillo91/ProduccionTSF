//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrigenGrupoLP
{
    using System;
    using System.Collections.Generic;
    
    public partial class GroupPersonByRol
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GroupPersonByRol()
        {
            this.GroupPersonByRolDetail = new HashSet<GroupPersonByRolDetail>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int id_company { get; set; }
        public int id_rol { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
    
        public virtual Rol Rol { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GroupPersonByRolDetail> GroupPersonByRolDetail { get; set; }
    }
}
