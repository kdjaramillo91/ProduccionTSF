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
    
    public partial class TblIvDetTomaFisica
    {
        public int NIdIvTomaSaldo { get; set; }
        public short NNuTomaFisica { get; set; }
        public short NNuSecuencia { get; set; }
        public string CCiUbicacion { get; set; }
        public string CCiProducto { get; set; }
        public string CCiUnidadMedida { get; set; }
        public decimal NQnTomaFisica { get; set; }
        public string CCtDiferencia { get; set; }
        public decimal NQnDiferencia { get; set; }
        public Nullable<decimal> NVtPrecio { get; set; }
        public Nullable<decimal> NIdIvProductoSerie { get; set; }
    }
}
