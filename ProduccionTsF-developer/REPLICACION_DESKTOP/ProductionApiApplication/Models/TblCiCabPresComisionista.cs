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
    
    public partial class TblCiCabPresComisionista
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblCiCabPresComisionista()
        {
            this.TblCiDetPresComisionista = new HashSet<TblCiDetPresComisionista>();
        }
    
        public int NIdPresComisionista { get; set; }
        public int NNuAnio { get; set; }
        public int NNuPeriodo { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CCiEstacionIngreso { get; set; }
        public System.DateTime DFxIngreso { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CCiEstacionModifica { get; set; }
        public Nullable<System.DateTime> DFxModifica { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblCiDetPresComisionista> TblCiDetPresComisionista { get; set; }
    }
}
