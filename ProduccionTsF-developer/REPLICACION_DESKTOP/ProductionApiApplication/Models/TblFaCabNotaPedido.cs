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
    
    public partial class TblFaCabNotaPedido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblFaCabNotaPedido()
        {
            this.TblFaDetNotaPedido = new HashSet<TblFaDetNotaPedido>();
        }
    
        public int NIdFaNotaPedido { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public Nullable<int> NNuNotaPedido { get; set; }
        public Nullable<int> NNuReferencia { get; set; }
        public string CciCliente { get; set; }
        public Nullable<System.DateTime> DFxNotaPedido { get; set; }
        public Nullable<decimal> NNuUnidad { get; set; }
        public Nullable<decimal> NNuPeso { get; set; }
        public Nullable<decimal> NNuPesoEquivalente { get; set; }
        public string CTxObservacion { get; set; }
        public string CCeNotaPedido { get; set; }
        public Nullable<System.DateTime> DFiIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public Nullable<System.DateTime> DFmModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
        public string CCiVendedor { get; set; }
        public string CCiFormaPago { get; set; }
        public string CCiCarga { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblFaDetNotaPedido> TblFaDetNotaPedido { get; set; }
    }
}
