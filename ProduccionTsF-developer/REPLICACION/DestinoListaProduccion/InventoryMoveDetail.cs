//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DestinoListaProduccion
{
    using System;
    using System.Collections.Generic;
    
    public partial class InventoryMoveDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InventoryMoveDetail()
        {
            this.InventoryMoveDetail1 = new HashSet<InventoryMoveDetail>();
            this.InventoryMoveDetail11 = new HashSet<InventoryMoveDetail>();
            this.InventoryMoveDetail12 = new HashSet<InventoryMoveDetail>();
            this.InventoryMoveDetailEntryDispatchMaterialsInProductionLot = new HashSet<InventoryMoveDetailEntryDispatchMaterialsInProductionLot>();
            this.InventoryMoveDetailEntryProductionLotDetail = new HashSet<InventoryMoveDetailEntryProductionLotDetail>();
            this.InventoryMoveDetailEntryProductionLotLiquidation = new HashSet<InventoryMoveDetailEntryProductionLotLiquidation>();
            this.InventoryMoveDetailExitDispatchMaterials = new HashSet<InventoryMoveDetailExitDispatchMaterials>();
            this.InventoryMoveDetailExitPackingMaterial = new HashSet<InventoryMoveDetailExitPackingMaterial>();
            this.InventoryMoveDetailExitProductionLotDetail = new HashSet<InventoryMoveDetailExitProductionLotDetail>();
            this.InventoryMoveDetailPurchaseOrder = new HashSet<InventoryMoveDetailPurchaseOrder>();
            this.InventoryMoveDetailRemisionMaterial = new HashSet<InventoryMoveDetailRemisionMaterial>();
            this.InventoryMoveDetailTransfer = new HashSet<InventoryMoveDetailTransfer>();
            this.InventoryMoveDetailTransfer1 = new HashSet<InventoryMoveDetailTransfer>();
            this.InventoryReservation = new HashSet<InventoryReservation>();
            this.InventoryReservation1 = new HashSet<InventoryReservation>();
            this.ProductionCostsProcessDetail = new HashSet<ProductionCostsProcessDetail>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_lot { get; set; }
        public int id_item { get; set; }
        public int id_inventoryMove { get; set; }
        public decimal entryAmount { get; set; }
        public decimal entryAmountCost { get; set; }
        public decimal exitAmount { get; set; }
        public decimal exitAmountCost { get; set; }
        public int id_metricUnit { get; set; }
        public int id_warehouse { get; set; }
        public Nullable<int> id_warehouseLocation { get; set; }
        public Nullable<int> id_warehouseEntry { get; set; }
        public Nullable<int> id_inventoryMoveDetailExit { get; set; }
        public bool inMaximumUnit { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public Nullable<int> id_inventoryMoveDetailPrevious { get; set; }
        public Nullable<int> id_inventoryMoveDetailNext { get; set; }
        public decimal unitPrice { get; set; }
        public decimal balance { get; set; }
        public decimal averagePrice { get; set; }
        public decimal balanceCost { get; set; }
        public Nullable<int> id_metricUnitMove { get; set; }
        public Nullable<decimal> unitPriceMove { get; set; }
        public Nullable<decimal> amountMove { get; set; }
        public Nullable<int> id_costCenter { get; set; }
        public Nullable<int> id_subCostCenter { get; set; }
        public string natureSequential { get; set; }
        public bool genSecTrans { get; set; }
        public Nullable<int> id_warehouseLocationEntry { get; set; }
        public Nullable<int> id_costCenterEntry { get; set; }
        public Nullable<int> id_subCostCenterEntry { get; set; }
        public Nullable<int> id_productionCart { get; set; }
        public string ordenProduccion { get; set; }
        public decimal productoCost { get; set; }
        public decimal lastestProductoCost { get; set; }
        public Nullable<int> id_CostAllocationDetail { get; set; }
        public Nullable<int> id_lastestCostAllocationDetail { get; set; }
        public string lotMarked { get; set; }
        public Nullable<int> id_personProcessPlant { get; set; }
    
        public virtual CostCenter CostCenter { get; set; }
        public virtual CostCenter CostCenter1 { get; set; }
        public virtual CostCenter CostCenter2 { get; set; }
        public virtual CostCenter CostCenter3 { get; set; }
        public virtual InventoryMove InventoryMove { get; set; }
        public virtual Item Item { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetail> InventoryMoveDetail1 { get; set; }
        public virtual InventoryMoveDetail InventoryMoveDetail2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetail> InventoryMoveDetail11 { get; set; }
        public virtual InventoryMoveDetail InventoryMoveDetail3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetail> InventoryMoveDetail12 { get; set; }
        public virtual InventoryMoveDetail InventoryMoveDetail4 { get; set; }
        public virtual Lot Lot { get; set; }
        public virtual MetricUnit MetricUnit { get; set; }
        public virtual MetricUnit MetricUnit1 { get; set; }
        public virtual Person Person { get; set; }
        public virtual ProductionCart ProductionCart { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Warehouse Warehouse1 { get; set; }
        public virtual WarehouseLocation WarehouseLocation { get; set; }
        public virtual WarehouseLocation WarehouseLocation1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailEntryDispatchMaterialsInProductionLot> InventoryMoveDetailEntryDispatchMaterialsInProductionLot { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailEntryProductionLotDetail> InventoryMoveDetailEntryProductionLotDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailEntryProductionLotLiquidation> InventoryMoveDetailEntryProductionLotLiquidation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailExitDispatchMaterials> InventoryMoveDetailExitDispatchMaterials { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailExitPackingMaterial> InventoryMoveDetailExitPackingMaterial { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailExitProductionLotDetail> InventoryMoveDetailExitProductionLotDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailPurchaseOrder> InventoryMoveDetailPurchaseOrder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailRemisionMaterial> InventoryMoveDetailRemisionMaterial { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailTransfer> InventoryMoveDetailTransfer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetailTransfer> InventoryMoveDetailTransfer1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryReservation> InventoryReservation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryReservation> InventoryReservation1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionCostsProcessDetail> ProductionCostsProcessDetail { get; set; }
    }
}
