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
    
    public partial class TblFaCabOrdenProduccion
    {
        public int NNuFaOrdenProduccion { get; set; }
        public int NNuFaOrdenCompra { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public string CciCliente { get; set; }
        public string CCiBodega { get; set; }
        public string CCtProduccion { get; set; }
        public System.DateTime DFxOrdenProduccion { get; set; }
        public string CDsObservacion { get; set; }
        public string CCeOrdenProduccion { get; set; }
        public System.DateTime DFiIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public Nullable<System.DateTime> DFmModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
    }
}
