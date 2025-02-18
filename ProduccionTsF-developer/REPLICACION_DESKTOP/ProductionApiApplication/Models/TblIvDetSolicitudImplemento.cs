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
    
    public partial class TblIvDetSolicitudImplemento
    {
        public int NIdIvSolicitud { get; set; }
        public short NNuSecuencia { get; set; }
        public string CCiProducto { get; set; }
        public string CCiUnidadMedida { get; set; }
        public Nullable<decimal> NQnBodegaCentral { get; set; }
        public Nullable<decimal> NQnBodegaProdUsado { get; set; }
        public Nullable<decimal> NQnACobrar { get; set; }
        public Nullable<decimal> NQnCobrada { get; set; }
        public Nullable<decimal> NQnDevuelta { get; set; }
        public string CCiDpto { get; set; }
        public string CCiProyecto { get; set; }
        public string CCiSubProyecto { get; set; }
        public decimal NVtPrecioPromedioBodCentral { get; set; }
        public decimal NVtPrecioPromedioBodUsada { get; set; }
        public string CCiTipoPres { get; set; }
        public string CSnGrabaIVA { get; set; }
        public Nullable<decimal> NNuPorIva { get; set; }
        public Nullable<decimal> NNuValIva { get; set; }
    
        public virtual TblIvCabSolicitudImplemento TblIvCabSolicitudImplemento { get; set; }
        public virtual TblIvProducto TblIvProducto { get; set; }
    }
}
