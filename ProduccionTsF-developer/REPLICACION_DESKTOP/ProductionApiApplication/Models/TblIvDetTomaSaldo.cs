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
    
    public partial class TblIvDetTomaSaldo
    {
        public int NIdIvTomaSaldo { get; set; }
        public short NNuSecuencia { get; set; }
        public short NNuHoja { get; set; }
        public string CCiUbicacion { get; set; }
        public string CCiProducto { get; set; }
        public string CCiUnidadMedida { get; set; }
        public decimal NQnExistencia { get; set; }
        public decimal NVrPromedio { get; set; }
        public Nullable<decimal> NIdIvProductoSerie { get; set; }
    }
}
