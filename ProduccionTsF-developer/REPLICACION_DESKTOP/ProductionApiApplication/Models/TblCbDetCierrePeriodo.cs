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
    
    public partial class TblCbDetCierrePeriodo
    {
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public string CNuSerieRecibero { get; set; }
        public short NNuAnio { get; set; }
        public short NNuPeriodo { get; set; }
        public Nullable<System.DateTime> DFiPeriodo { get; set; }
        public Nullable<System.DateTime> DFfPeriodo { get; set; }
        public string CCePeriodo { get; set; }
        public string CSnCierre { get; set; }
        public Nullable<System.DateTime> DFxCierre { get; set; }
        public string CCiUsuarioCierre { get; set; }
        public string CDsEstacionCierre { get; set; }
        public string CcePeriodoFactura { get; set; }
    }
}
