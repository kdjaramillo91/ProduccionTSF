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
    
    public partial class CalendarPriceListTypeAdvanceValuePercentage
    {
        public int id_CalendarPriceListType { get; set; }
        public Nullable<decimal> AdvanceValuePercentage { get; set; }
    
        public virtual CalendarPriceListType CalendarPriceListType { get; set; }
    }
}
