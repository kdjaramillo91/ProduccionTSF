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
    
    public partial class TblExLiquidaFrutas
    {
        public int NNuIdLiquidaFrutas { get; set; }
        public int NNuSecuencia { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public string CCiMotivo { get; set; }
        public Nullable<decimal> NNuValor { get; set; }
        public Nullable<int> NIdTablaPeriodo { get; set; }
        public string CCiProveedor { get; set; }
        public string CCeEstado { get; set; }
        public Nullable<System.DateTime> DfxFechaIngreso { get; set; }
        public string CCiEstacionIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public Nullable<System.DateTime> DfxFechaModifica { get; set; }
        public string CCiEstacionModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public Nullable<int> NIdIvMovBanc { get; set; }
        public string CCiTipoMovimiento { get; set; }
        public Nullable<int> NNidComprobante { get; set; }
    }
}
