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
    
    public partial class TblOcDetOrdenTrabajo
    {
        public int NNuControl { get; set; }
        public short NNuSecuencia { get; set; }
        public string CNoProducto { get; set; }
        public string CSNPagaIva { get; set; }
        public string CSNManoObra { get; set; }
        public decimal NQnOrdenada { get; set; }
        public decimal NVtPrecioUnitario { get; set; }
        public decimal NVtMontoItem { get; set; }
        public string CCiDpto { get; set; }
        public string CCiProyecto { get; set; }
        public string CCiSubProyecto { get; set; }
        public string CCiArea { get; set; }
        public Nullable<decimal> NVtPorcDscto1 { get; set; }
        public Nullable<decimal> NVtMontoBruto { get; set; }
        public Nullable<decimal> NVtMontoDscto1 { get; set; }
    }
}
