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
    
    public partial class TblFaEstadisticaProducto
    {
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public short NNuAnio { get; set; }
        public short NNuPeriodo { get; set; }
        public string CCiBodega { get; set; }
        public string CCiUbicacion { get; set; }
        public string CCiProducto { get; set; }
        public string CCiCliente { get; set; }
        public Nullable<decimal> NQnVendida { get; set; }
        public Nullable<decimal> NQnDevuelta { get; set; }
        public Nullable<decimal> NQnPerdida { get; set; }
        public Nullable<decimal> NQnAnulada { get; set; }
        public Nullable<decimal> NVtVendida { get; set; }
        public Nullable<decimal> NVtDevuelta { get; set; }
        public Nullable<decimal> NVtPerdida { get; set; }
        public Nullable<decimal> NVtAnulada { get; set; }
        public string CCtOrigen { get; set; }
    
        public virtual tblCiCias tblCiCias { get; set; }
        public virtual TblCiDivision TblCiDivision { get; set; }
        public virtual tblCiSucursal tblCiSucursal { get; set; }
        public virtual TblIvProducto TblIvProducto { get; set; }
    }
}
