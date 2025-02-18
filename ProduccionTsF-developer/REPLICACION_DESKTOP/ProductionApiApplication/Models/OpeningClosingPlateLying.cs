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
        public int id_maintenanceWarehouse { get; set; }
        public int id_maintenanceWarehouseLocation { get; set; }
        public int id_company { get; set; }
    
        public virtual Document Document { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Warehouse Warehouse1 { get; set; }
        public virtual WarehouseLocation WarehouseLocation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpeningClosingPlateLyingDetail> OpeningClosingPlateLyingDetail { get; set; }
    }
}
