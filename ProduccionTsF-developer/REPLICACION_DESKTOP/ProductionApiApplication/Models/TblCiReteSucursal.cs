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
    
    public partial class TblCiReteSucursal
    {
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public int NNuSecuencia { get; set; }
        public string CNuSerie { get; set; }
        public string CNuAutorizacion { get; set; }
        public decimal NNuReteIni { get; set; }
        public decimal NNuReteFin { get; set; }
        public Nullable<System.DateTime> DFxEmision { get; set; }
        public System.DateTime DFxCaducidad { get; set; }
        public string CCeEstado { get; set; }
        public string CCtEmisionDcto { get; set; }
        public string CCtDisponibilidadSistemaExterno { get; set; }
    }
}
