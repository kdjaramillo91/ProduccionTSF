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
    
    public partial class TblAcCabDepreciacion
    {
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public decimal NNuAnio { get; set; }
        public decimal NNuPeriodo { get; set; }
        public System.DateTime DFxContabiliza { get; set; }
        public string CCiTipoComprobante { get; set; }
        public decimal NNuComprobante { get; set; }
        public Nullable<System.DateTime> DFiFechaIngreso { get; set; }
        public string CCiUsuario { get; set; }
        public string CDsEstacion { get; set; }
        public string CCeDepreciacion { get; set; }
    }
}
