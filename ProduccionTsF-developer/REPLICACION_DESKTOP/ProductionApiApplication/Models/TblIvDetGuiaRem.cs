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
    
    public partial class TblIvDetGuiaRem
    {
        public int NNuControl { get; set; }
        public short NNuSecuencia { get; set; }
        public string CCiUbicacion { get; set; }
        public string CCiProducto { get; set; }
        public string CCiUnidad { get; set; }
        public decimal NQnCantidad { get; set; }
        public decimal NVtPesoBruto { get; set; }
        public string CDxObservacion { get; set; }
        public Nullable<decimal> NQnUnidad { get; set; }
        public Nullable<decimal> NIdIvProductoSerie { get; set; }
        public string CCiUnidadPeso { get; set; }
        public Nullable<decimal> NQnPesoNetoUnitario { get; set; }
        public Nullable<decimal> NQnPesoNetoTotal { get; set; }
    }
}
