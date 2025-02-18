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
    
    public partial class OpeningClosingPlateLying
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OpeningClosingPlateLying()
        {
            this.OpeningClosingPlateLyingDetail = new HashSet<OpeningClosingPlateLyingDetail>();
        }
    
        public int id { get; set; }
        public int id_responsable { get; set; }
        public int id_freezerWarehouse { get; set; }
        public string ids_freezerWarehouseLocation { get; set; }
        public int id_company { get; set; }
        public string ids_productionCart { get; set; }
        public string ids_item { get; set; }
        public decimal selectedQuantity { get; set; }
        public Nullable<System.DateTime> dateTimeStartLying { get; set; }
        public Nullable<System.DateTime> dateTimeEndLying { get; set; }
        public Nullable<decimal> temperature { get; set; }
        public Nullable<int> id_freezerMachineForProd { get; set; }
        public Nullable<int> id_turn { get; set; }
        public bool tunnelTransferPlate { get; set; }
        public Nullable<int> id_freezerMachineForProdDestination { get; set; }
        public string ids_lot { get; set; }
        public Nullable<int> id_warehouseDestiny { get; set; }
        public Nullable<int> id_warehouseLocationDestiny { get; set; }
    
        public virtual Document Document { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual MachineForProd MachineForProd { get; set; }
        public virtual MachineForProd MachineForProd1 { get; set; }
        public virtual Turn Turn { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpeningClosingPlateLyingDetail> OpeningClosingPlateLyingDetail { get; set; }
    }
}
