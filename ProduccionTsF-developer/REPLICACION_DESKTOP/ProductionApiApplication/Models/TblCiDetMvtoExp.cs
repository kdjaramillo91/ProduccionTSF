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
    
    public partial class TblCiDetMvtoExp
    {
        public int NNuControl { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public string CCiTipoComprobante { get; set; }
        public Nullable<int> NNuComprobante { get; set; }
        public Nullable<short> NNuAnio { get; set; }
        public Nullable<short> NNuPeriodo { get; set; }
        public string CCiAuxiliar { get; set; }
        public Nullable<decimal> NNuDebito { get; set; }
        public Nullable<decimal> NNuCredito { get; set; }
        public string CDsReferencia { get; set; }
        public string CDsReferencia01 { get; set; }
        public Nullable<System.DateTime> DFcFechaVenc { get; set; }
        public string CCiTipoDocCta { get; set; }
        public string CCiTipoAuxiliar { get; set; }
    }
}
