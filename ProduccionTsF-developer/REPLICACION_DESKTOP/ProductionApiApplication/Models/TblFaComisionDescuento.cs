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
    
    public partial class TblFaComisionDescuento
    {
        public int CCiComisionDscto { get; set; }
        public string CDsComisionDscto { get; set; }
        public Nullable<int> NQnDiasBase { get; set; }
        public Nullable<decimal> NQnPorcBonifDiaria { get; set; }
        public Nullable<decimal> NQnPorcDsctoDiario { get; set; }
        public string CCeComisionDscto { get; set; }
        public System.DateTime DFiIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public Nullable<System.DateTime> DFmModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
    }
}
