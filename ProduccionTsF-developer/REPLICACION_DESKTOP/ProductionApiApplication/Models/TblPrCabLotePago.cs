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
    
    public partial class TblPrCabLotePago
    {
        public int NIdPrLotePago { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public int NNuLotePago { get; set; }
        public System.DateTime DFxLotePago { get; set; }
        public string CCeLotePago { get; set; }
        public decimal NNuPresupuesto { get; set; }
        public decimal NNuSecuenciaPresupuesto { get; set; }
        public string CCiTipoAuxiliar { get; set; }
        public string CCiProveedor { get; set; }
        public Nullable<int> NNuAnio { get; set; }
        public Nullable<int> NNuOrden { get; set; }
        public string CDsReferencia { get; set; }
        public string CDsObservacion { get; set; }
        public decimal NVtCostoTotal { get; set; }
        public Nullable<System.DateTime> DFiIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public Nullable<System.DateTime> DFmModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
        public Nullable<System.DateTime> DFxAprueba { get; set; }
        public string CCiUsuarioAprueba { get; set; }
        public string CDsEstacionAprueba { get; set; }
        public Nullable<int> NNuPlanillaPago { get; set; }
    }
}
