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
    
    public partial class TblFaDetPlanillaCobro
    {
        public int NIdFaPlanillaCobro { get; set; }
        public int NNuSecuencia { get; set; }
        public string CCiProducto { get; set; }
        public string CCiUnidadVenta { get; set; }
        public Nullable<decimal> NNuCantidad { get; set; }
        public Nullable<decimal> NNuPreUnitario { get; set; }
        public Nullable<decimal> NVtBrutoBaseIva { get; set; }
        public Nullable<decimal> NNuPorcDsctoBaseIva { get; set; }
        public Nullable<decimal> NNuValDsctoBaseIva { get; set; }
        public Nullable<decimal> NNuSubTotalBaseIva { get; set; }
        public Nullable<decimal> NVtBrutoBaseCero { get; set; }
        public Nullable<decimal> NNuPorcDsctoBaseCero { get; set; }
        public Nullable<decimal> NNuValDsctoBaseCero { get; set; }
        public Nullable<decimal> NNuSubTotalBase0 { get; set; }
        public decimal NNuSubTotal { get; set; }
        public string CSnGrabaIva { get; set; }
        public Nullable<decimal> NVtMontoBruto { get; set; }
        public Nullable<decimal> NNuPorDescuento { get; set; }
        public Nullable<decimal> NNuDescProducto { get; set; }
        public Nullable<decimal> NNuSubTotalNeto { get; set; }
        public Nullable<decimal> NNuPorIva { get; set; }
        public Nullable<decimal> NNuValIva { get; set; }
        public decimal NNuTotalxPagar { get; set; }
        public string CDxObservacion { get; set; }
        public string CCiDpto { get; set; }
        public string CCiProyecto { get; set; }
        public string CCiSubProyecto { get; set; }
        public string CCiTipoPrecioProducto { get; set; }
        public Nullable<decimal> NNuPrecioOriginal { get; set; }
    }
}
