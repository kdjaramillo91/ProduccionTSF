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
    
    public partial class TblCiCabMovBatchtemp
    {
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public string CCiTipoComprobante { get; set; }
        public decimal NNuComprobanteBatch { get; set; }
        public decimal NNuAnio { get; set; }
        public decimal NNuPeriodo { get; set; }
        public string CCiSistema { get; set; }
        public System.DateTime DfmFechaContab { get; set; }
        public string CTxReferencia { get; set; }
        public decimal NNuTotDebito { get; set; }
        public decimal NNuTotCredito { get; set; }
        public string CCtEstadoComp { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public System.DateTime DFiFechaIngreso { get; set; }
        public string CTxEstacionIng { get; set; }
        public string CCiUsuarioAprueba { get; set; }
        public Nullable<System.DateTime> DFcFechaAprueba { get; set; }
        public string CTxEstacionAprueba { get; set; }
        public string CdsGlosa { get; set; }
        public string CCiOrigen { get; set; }
    }
}
