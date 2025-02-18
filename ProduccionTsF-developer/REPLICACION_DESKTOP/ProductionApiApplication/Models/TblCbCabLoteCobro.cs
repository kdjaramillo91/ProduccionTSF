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
    
    public partial class TblCbCabLoteCobro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblCbCabLoteCobro()
        {
            this.TblCbDetLoteCobro = new HashSet<TblCbDetLoteCobro>();
            this.TblCbDetLoteFPago = new HashSet<TblCbDetLoteFPago>();
        }
    
        public int NIdCbLoteCobro { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public int NNuLote { get; set; }
        public System.DateTime DFxCobro { get; set; }
        public string CNuSerieRecibero { get; set; }
        public short NCiTipoCobranza { get; set; }
        public System.DateTime DFxIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public Nullable<System.DateTime> DFxModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CCiBanco { get; set; }
        public string CCiCtaCte { get; set; }
        public Nullable<decimal> NVtCobro { get; set; }
        public string CNuDcto { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblCbDetLoteCobro> TblCbDetLoteCobro { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblCbDetLoteFPago> TblCbDetLoteFPago { get; set; }
    }
}
