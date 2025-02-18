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
    
    public partial class IntegrationProcessDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IntegrationProcessDetail()
        {
            this.IntegrationProcessDetailLog = new HashSet<IntegrationProcessDetailLog>();
            this.IntegrationProcessOutputDocument = new HashSet<IntegrationProcessOutputDocument>();
        }
    
        public int id { get; set; }
        public int id_Lote { get; set; }
        public int id_Document { get; set; }
        public int id_StatusDocument { get; set; }
        public string glossData { get; set; }
        public decimal totalValueDocument { get; set; }
        public int id_UserCreate { get; set; }
        public int id_UserUpdate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public Nullable<System.DateTime> dateLastUpdateDocument { get; set; }
    
        public virtual Document Document { get; set; }
        public virtual IntegrationProcess IntegrationProcess { get; set; }
        public virtual IntegrationState IntegrationState { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntegrationProcessDetailLog> IntegrationProcessDetailLog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntegrationProcessOutputDocument> IntegrationProcessOutputDocument { get; set; }
        public string relatedDocument { get; set; }
    }
}
