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
    
    public partial class SalesRequestDetailSalesQuotation
    {
        public int id { get; set; }
        public int id_salesRequestDetail { get; set; }
        public Nullable<int> id_salesQuotationDetail { get; set; }
        public int id_salesQuotation { get; set; }
        public decimal quantity { get; set; }
    
        public virtual SalesQuotation SalesQuotation { get; set; }
        public virtual SalesQuotationDetail SalesQuotationDetail { get; set; }
        public virtual SalesRequestDetail SalesRequestDetail { get; set; }
    }
}
