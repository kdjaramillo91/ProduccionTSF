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
    
    public partial class CategoryActivityRise
    {
        public int id { get; set; }
        public int id_categoryRise { get; set; }
        public int id_activityRise { get; set; }
        public decimal invoiceAmountRise { get; set; }
    
        public virtual ActivityRise ActivityRise { get; set; }
        public virtual CategoryRise CategoryRise { get; set; }
    }
}
