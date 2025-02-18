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
    
    public partial class TblIvCabTransfEnvio
    {
        public int NIdIvTransfEnvio { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public int NNuControl { get; set; }
        public short NNuAnio { get; set; }
        public string CNuDcto { get; set; }
        public string CCtTransferencia { get; set; }
        public string CSNTransfAutomatica { get; set; }
        public string CCiBodega { get; set; }
        public string CCiMotivo { get; set; }
        public int NNuMovimiento { get; set; }
        public System.DateTime DFxMovimiento { get; set; }
        public decimal NVtMontoTotal { get; set; }
        public string CCiBodegaRecibe { get; set; }
        public string CCiUbicacionRecibe { get; set; }
        public string CCiBodegaTransito { get; set; }
        public string CCiUbicacionTransito { get; set; }
        public string CCiMotivoIngresoTransito { get; set; }
        public Nullable<int> NNuMvtoIngresoTransito { get; set; }
        public string CCiEmpleadoSolicita { get; set; }
        public string CCeTransfEnvio { get; set; }
        public string CDsReferencia { get; set; }
        public string CDsObservacion { get; set; }
        public string CSNGuiaRemision { get; set; }
        public Nullable<int> NIdIvGuiaRemision { get; set; }
        public Nullable<int> NNuControlPeso { get; set; }
        public Nullable<int> NIdCiComprobante { get; set; }
        public Nullable<int> NIdTramite { get; set; }
        public string CCiProductoTramite { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public System.DateTime DFiIngreso { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
        public Nullable<System.DateTime> DFmModifica { get; set; }
        public string CCiUsuarioAprueba { get; set; }
        public string CDsEstacionAprueba { get; set; }
        public Nullable<System.DateTime> DFxAprueba { get; set; }
        public Nullable<int> NIdTablaPeriodo { get; set; }
        public string CCiCiaPrestamo { get; set; }
        public string CCiDivisionPrestamo { get; set; }
        public string CCiSucursalPrestamo { get; set; }
        public string CCtPrestamo { get; set; }
        public string CCePrestamo { get; set; }
        public Nullable<int> NIdIvTransfEnvioPrestamo { get; set; }
        public Nullable<int> NNuFaOrdenCompra { get; set; }
        public string CciCliente { get; set; }
        public string CSnPresupuesto { get; set; }
        public Nullable<int> NIdPrPresupuestoEnvio { get; set; }
        public Nullable<int> NIdPrPresupuestoRecibe { get; set; }
        public string CCiMotivoRecibe { get; set; }
        public Nullable<int> NIdIvTransfReqEnvio { get; set; }
        public string CsnAutorizado { get; set; }
        public string CCiUsuarioAutoriza { get; set; }
        public string CDsEstacionAutoriza { get; set; }
        public Nullable<System.DateTime> DFxAutoriza { get; set; }
    }
}
