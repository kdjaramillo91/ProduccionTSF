//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrigenPersonaProduccion
{
    using System;
    using System.Collections.Generic;
    
    public partial class Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Person()
        {
            this.Rol = new HashSet<Rol>();
        }
    
        public int id { get; set; }
        public int id_personType { get; set; }
        public int id_identificationType { get; set; }
        public string identification_number { get; set; }
        public string fullname_businessName { get; set; }
        public byte[] photo { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public Nullable<int> id_company { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public string codeCI { get; set; }
        public string bCC { get; set; }
        public string tradeName { get; set; }
        public string cellPhoneNumberPerson { get; set; }
        public string emailCC { get; set; }
    
        public virtual IdentificationType IdentificationType { get; set; }
        public virtual PersonType PersonType { get; set; }
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rol> Rol { get; set; }
    }
}
