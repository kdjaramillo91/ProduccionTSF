//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProductionApiApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblHcCinta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblHcCinta()
        {
            this.TblHcDetRacimoProduccion = new HashSet<TblHcDetRacimoProduccion>();
        }
    
        public short NIdHcCinta { get; set; }
        public string CCiCinta { get; set; }
        public string CNoCinta { get; set; }
        public string CCeCinta { get; set; }
        public System.DateTime DFxIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public Nullable<System.DateTime> DFxModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblHcDetRacimoProduccion> TblHcDetRacimoProduccion { get; set; }
    }
}
