//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DXPANACEASOFT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RemGuideLiqTransportation
    {
        public int id_remisionGuide { get; set; }
        public decimal price { get; set; }
        public decimal pricedays { get; set; }
        public decimal priceextension { get; set; }
        public decimal priceadjustment { get; set; }
        public decimal pricetotal { get; set; }
        public decimal pricesubtotal { get; set; }
        public decimal priceadvance { get; set; }
        public string descriptionRG { get; set; }
        public Nullable<decimal> PriceCancelled { get; set; }
    
        public virtual RemissionGuide RemissionGuide { get; set; }
    }
}
