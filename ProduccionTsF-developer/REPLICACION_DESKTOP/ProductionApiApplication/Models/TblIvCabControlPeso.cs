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
    
    public partial class TblIvCabControlPeso
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblIvCabControlPeso()
        {
            this.TblIvDetControlPeso = new HashSet<TblIvDetControlPeso>();
        }
    
        public int NIdIvControlPeso { get; set; }
        public int NNuControlPeso { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public string CCtControlPeso { get; set; }
        public string CCiBodegaControlPeso { get; set; }
        public string CCtRelacion { get; set; }
        public string CCiCiaRelacion { get; set; }
        public string CCiDivisionRelacion { get; set; }
        public string CCiSucursalRelacion { get; set; }
        public string CCiBodegaRelacion { get; set; }
        public Nullable<int> NIdFaNotaPedido { get; set; }
        public string CciCliente { get; set; }
        public string CCiProveedorRelacion { get; set; }
        public string CCiSucursalDestino { get; set; }
        public string CCiIdentifChofer { get; set; }
        public string CCiPlaca { get; set; }
        public Nullable<int> NNuTemperatura { get; set; }
        public Nullable<System.DateTime> DfxControlPeso { get; set; }
        public Nullable<System.DateTime> DfxHoraControlPeso { get; set; }
        public string CCiUnidadMedida { get; set; }
        public string CdsReferencia { get; set; }
        public string CdsObservacion { get; set; }
        public Nullable<decimal> NVtBruto { get; set; }
        public Nullable<decimal> NVtNeto { get; set; }
        public string CCeControlPeso { get; set; }
        public Nullable<System.DateTime> DFiIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public Nullable<System.DateTime> DFmModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
        public string CCiCarga { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblIvDetControlPeso> TblIvDetControlPeso { get; set; }
    }
}
