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
    
    public partial class InventoryMoveExitPackingMaterial
    {
        public int id_InventoryMove { get; set; }
        public int id_ItemMaster { get; set; }
        public decimal quantityExit { get; set; }
    
        public virtual InventoryMove InventoryMove { get; set; }
        public virtual Item Item { get; set; }
    }
}
