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
    
    public partial class TblCbCabCobro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblCbCabCobro()
        {
            this.TblCbDetCobroDpto = new HashSet<TblCbDetCobroDpto>();
        }
    
        public int NNuControl { get; set; }
        public string CCiCompania { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public string CNuSerieRecibero { get; set; }
        public int NNuCobro { get; set; }
        public int NNuRecibo { get; set; }
        public System.DateTime DFxCobro { get; set; }
        public string CciCliente { get; set; }
        public decimal EVtCobro { get; set; }
        public string CCeCobro { get; set; }
        public decimal EVtRealMora { get; set; }
        public Nullable<decimal> EQtExonera { get; set; }
        public Nullable<decimal> EQtTasa { get; set; }
        public string CTxReferencia { get; set; }
        public string CCtOrigen { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public System.DateTime DfxIngreso { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CdsEstacionModifica { get; set; }
        public Nullable<System.DateTime> DfxModifica { get; set; }
        public string CCiUsuarioAplica { get; set; }
        public string CDsEstacionAplica { get; set; }
        public Nullable<System.DateTime> DfxAplica { get; set; }
        public string CCiCarga { get; set; }
        public string CDSObservacion { get; set; }
        public string CCeContabalidad { get; set; }
        public string CCtContabalidad { get; set; }
        public Nullable<int> NIdCiMovimiento { get; set; }
        public string CSNExcedente { get; set; }
        public Nullable<short> NCtDocCobranzaExcedente { get; set; }
        public Nullable<int> NNuFacturaExcedente { get; set; }
        public string CNuSerieDocExcedente { get; set; }
        public Nullable<int> NuDocumentoExcedente { get; set; }
        public string CCeDeposito { get; set; }
        public string CSnAnticipo { get; set; }
        public Nullable<decimal> NVtRecibido { get; set; }
        public Nullable<decimal> NVtCambio { get; set; }
        public Nullable<decimal> NVtEfectivo { get; set; }
        public string CCtPrograma { get; set; }
        public Nullable<System.DateTime> DFxReciboManual { get; set; }
        public string CCtCobro { get; set; }
        public string CCiDpto { get; set; }
        public string CCtPantallaDestExce { get; set; }
        public string CCiTipoTransaccionUAF { get; set; }
        public string CCiTipoBienUAF { get; set; }
        public Nullable<int> NIdGeMonedaUAF { get; set; }
        public string CTxDireccionBien { get; set; }
        public string CCiPaisUAFBien { get; set; }
        public string CNuContrato { get; set; }
        public string CCiCantonBienUAF { get; set; }
        public string CCiProvinciaBienUAF { get; set; }
        public string CCiCanalVenta { get; set; }
        public string CCiVendedor { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblCbDetCobroDpto> TblCbDetCobroDpto { get; set; }
    }
}
