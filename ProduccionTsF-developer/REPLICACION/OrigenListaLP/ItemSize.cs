//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrigenListaLP
{
    using System;
    using System.Collections.Generic;
    
    public partial class ItemSize
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ItemSize()
        {
            this.ItemSizeClass = new HashSet<ItemSizeClass>();
            this.ItemSizeProcessPLOrder = new HashSet<ItemSizeProcessPLOrder>();
            this.ItemSizeProcessTypePriceList = new HashSet<ItemSizeProcessTypePriceList>();
            this.PriceListItemSizeDetail = new HashSet<PriceListItemSizeDetail>();
        }
    
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int orderSize { get; set; }
        public int id_company { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemSizeClass> ItemSizeClass { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemSizeProcessPLOrder> ItemSizeProcessPLOrder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemSizeProcessTypePriceList> ItemSizeProcessTypePriceList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceListItemSizeDetail> PriceListItemSizeDetail { get; set; }
    }
}
