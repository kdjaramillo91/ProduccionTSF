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
    
    public partial class TblIvMotivoInventario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblIvMotivoInventario()
        {
            this.TblIvDetMotivoInventarioClasif = new HashSet<TblIvDetMotivoInventarioClasif>();
        }
    
        public string CCiMotivo { get; set; }
        public string CNoMotivo { get; set; }
        public string CCtIngrEgr { get; set; }
        public string CSNAfectaCantidad { get; set; }
        public string CSNRevaloriza { get; set; }
        public string CSNValidaSaldo { get; set; }
        public string CCeMotivo { get; set; }
        public string CSNCambiaPrecio { get; set; }
        public string CCtModo { get; set; }
        public string CSNReversoConsumo { get; set; }
        public string CCtOrigen { get; set; }
        public string CsnIngresoProveedor { get; set; }
        public string CSNNotaDebito { get; set; }
        public string CSNContabilidad { get; set; }
        public string CCtValoriza1 { get; set; }
        public string CCtValoriza2 { get; set; }
        public string CCtValoriza3 { get; set; }
        public string CCtContabiliza { get; set; }
        public string CSNCalculaCosto { get; set; }
        public string CSnValidaDcto { get; set; }
        public System.DateTime DFiIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public Nullable<System.DateTime> DFmModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
        public string CSnExigeGuiaRemision { get; set; }
        public string CSnExigeOrdenCompraClte { get; set; }
        public string CSnExigeOrdenProduccion { get; set; }
        public string CSnExigeEmpleadoRRHH { get; set; }
        public string CSnActualizaUltimoCosto { get; set; }
        public string CCtValorizaIOC { get; set; }
        public string CSnClasificacionProducto { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblIvDetMotivoInventarioClasif> TblIvDetMotivoInventarioClasif { get; set; }
    }
}
