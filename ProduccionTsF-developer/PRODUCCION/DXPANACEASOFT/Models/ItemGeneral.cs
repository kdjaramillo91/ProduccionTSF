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
    
    public partial class ItemGeneral
    {
        public int id_item { get; set; }
        public Nullable<int> id_group { get; set; }
        public Nullable<int> id_subgroup { get; set; }
        public Nullable<int> id_groupCategory { get; set; }
        public string manufacturer { get; set; }
        public Nullable<int> id_trademark { get; set; }
        public Nullable<int> id_trademarkModel { get; set; }
        public Nullable<int> id_size { get; set; }
        public Nullable<int> id_color { get; set; }
        public Nullable<int> id_countryOrigin { get; set; }
        public Nullable<int> id_Person { get; set; }
        public Nullable<bool> isASC { get; set; }
        public Nullable<int> id_certification { get; set; }
        public Nullable<int> mesVidaUtil { get; set; }
    
        public virtual Certification Certification { get; set; }
        public virtual Country Country { get; set; }
        public virtual Item Item { get; set; }
        public virtual ItemColor ItemColor { get; set; }
        public virtual ItemGroup ItemGroup { get; set; }
        public virtual ItemGroup ItemGroup1 { get; set; }
        public virtual ItemGroupCategory ItemGroupCategory { get; set; }
        public virtual ItemSize ItemSize { get; set; }
        public virtual ItemTrademark ItemTrademark { get; set; }
        public virtual ItemTrademarkModel ItemTrademarkModel { get; set; }
        public virtual Person Person { get; set; }
    }
}
