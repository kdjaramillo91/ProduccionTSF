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
    
    public partial class TblOcDetRequerimiento
    {
        public int NIdOcRequerimiento { get; set; }
        public short NNuSecuencia { get; set; }
        public string CCiProducto { get; set; }
        public string CCiUnidadMedida { get; set; }
        public Nullable<decimal> NQnPedida { get; set; }
        public Nullable<decimal> NQnOrdenada { get; set; }
        public Nullable<decimal> NQnRecibida { get; set; }
        public string CDsObservacion { get; set; }
        public string CCeItem { get; set; }
        public string CDsReferencia { get; set; }
        public decimal NVrUnitario { get; set; }
        public Nullable<decimal> NVtMontoItem { get; set; }
        public string CCiBodega { get; set; }
        public string CCiProyecto { get; set; }
        public string CCiSubProyecto { get; set; }
        public string CCiDpto { get; set; }
        public string CCiArea { get; set; }
        public string CCiTipoPres { get; set; }
        public string CSNActivoFijo { get; set; }
        public string CCiProveedor { get; set; }
        public string CCiSubDpto { get; set; }
        public string CCiProductoCompra { get; set; }
        public Nullable<int> NIdPrPresupuesto { get; set; }
        public string CCiModelo { get; set; }
        public string CCiUnidadPeso { get; set; }
        public Nullable<decimal> NQnPesoNetoUnitario { get; set; }
        public Nullable<decimal> NQnPesoNetoTotal { get; set; }
    }
}
