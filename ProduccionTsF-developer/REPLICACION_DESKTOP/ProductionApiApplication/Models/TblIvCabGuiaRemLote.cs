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
    
    public partial class TblIvCabGuiaRemLote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblIvCabGuiaRemLote()
        {
            this.TblIvDetGuiaRemLote = new HashSet<TblIvDetGuiaRemLote>();
        }
    
        public int NNuControl { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public int NNuLote { get; set; }
        public string CCiBodega { get; set; }
        public string CCtMotivo { get; set; }
        public System.DateTime DFxFechaEmision { get; set; }
        public string CCiIdentificacionChofer { get; set; }
        public string CDsCiaTransp { get; set; }
        public string CDsRucCiaTransp { get; set; }
        public string CDsPlaca { get; set; }
        public string CDsCarro { get; set; }
        public string CDsReferencia { get; set; }
        public string CDsNovedad { get; set; }
        public string CDsObservacionGeneral1 { get; set; }
        public string CDsObservacionGeneral2 { get; set; }
        public string CDsObservacionGeneral3 { get; set; }
        public string CDsObservacionGeneral4 { get; set; }
        public string CDsObservacionGeneral5 { get; set; }
        public string CNuSerieGuia { get; set; }
        public string CCtFormaEmisionDcto { get; set; }
        public string CCiAmbienteEmisionDctoElectronico { get; set; }
        public string CCtDisponibilidadSistemaExterno { get; set; }
        public string CCtServicioConsume { get; set; }
        public string CCeGuiaLote { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public System.DateTime DFiIngreso { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
        public Nullable<System.DateTime> DFmModifica { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblIvDetGuiaRemLote> TblIvDetGuiaRemLote { get; set; }
    }
}
