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
    
    public partial class TblCbCabDepositoTransito
    {
        public int NIdCbDepositoTransito { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public int NNuDepositoTransito { get; set; }
        public Nullable<int> NIdCbCuadreCobranza { get; set; }
        public System.DateTime DFxDepositoTransito { get; set; }
        public string CCeDepositoTransito { get; set; }
        public string CDsReferencia { get; set; }
        public string CDsObservacion { get; set; }
        public Nullable<decimal> NVtTotalEfectivo { get; set; }
        public Nullable<decimal> NVtTotalCheque { get; set; }
        public Nullable<decimal> NVtTotalFaltante { get; set; }
        public Nullable<decimal> NVtTotalSobrante { get; set; }
        public Nullable<int> NIdCiComprobante { get; set; }
        public System.DateTime DFxIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CCiEstacionIngreso { get; set; }
        public Nullable<System.DateTime> DFxModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CCiEstacionModifica { get; set; }
        public Nullable<decimal> NVtTotalIngresos { get; set; }
        public Nullable<decimal> NVtTotalEgresos { get; set; }
    }
}
