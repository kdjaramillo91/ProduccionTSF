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
    
    public partial class TblHcMotivoRechazo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblHcMotivoRechazo()
        {
            this.TblHcDetRecusadoProduccion = new HashSet<TblHcDetRecusadoProduccion>();
        }
    
        public short NIdHcMotivoRechazo { get; set; }
        public string CCiMotivoRechazo { get; set; }
        public string CNoMotivoRechazo { get; set; }
        public string CCeMotivoRechazo { get; set; }
        public string CCtOrigenMotivo { get; set; }
        public string CCtSumaResta { get; set; }
        public System.DateTime DFxIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public Nullable<System.DateTime> DFxModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblHcDetRecusadoProduccion> TblHcDetRecusadoProduccion { get; set; }
    }
}
