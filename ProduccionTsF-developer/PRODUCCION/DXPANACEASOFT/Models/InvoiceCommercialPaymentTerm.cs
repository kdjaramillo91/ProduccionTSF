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
    
    public partial class InvoiceCommercialPaymentTerm
    {
        public int id { get; set; }
        public int idInvoiceCommercial { get; set; }
        public int orderPayment { get; set; }
        public decimal valuePayment { get; set; }
        public System.DateTime dueDate { get; set; }
    
        public virtual InvoiceCommercial InvoiceCommercial { get; set; }
    }
}
