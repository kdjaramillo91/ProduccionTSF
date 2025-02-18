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
    
    public partial class MetricUnit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MetricUnit()
        {
            this.AutomaticTransferDetail = new HashSet<AutomaticTransferDetail>();
            this.AutomaticTransferDetail1 = new HashSet<AutomaticTransferDetail>();
            this.CapacityContainer = new HashSet<CapacityContainer>();
            this.DrainingTestDetail = new HashSet<DrainingTestDetail>();
            this.FrameworkContractItem = new HashSet<FrameworkContractItem>();
            this.IceBagRange = new HashSet<IceBagRange>();
            this.InventoryMoveDetail = new HashSet<InventoryMoveDetail>();
            this.InventoryMoveDetail1 = new HashSet<InventoryMoveDetail>();
            this.InvoiceCommercial = new HashSet<InvoiceCommercial>();
            this.InvoiceCommercialDetail = new HashSet<InvoiceCommercialDetail>();
            this.InvoiceCommercialDetail1 = new HashSet<InvoiceCommercialDetail>();
            this.InvoiceDetail = new HashSet<InvoiceDetail>();
            this.InvoiceDetail1 = new HashSet<InvoiceDetail>();
            this.InvoiceExterior = new HashSet<InvoiceExterior>();
            this.InvoiceExteriorWeight = new HashSet<InvoiceExteriorWeight>();
            this.ItemHeadIngredient = new HashSet<ItemHeadIngredient>();
            this.ItemIngredient = new HashSet<ItemIngredient>();
            this.ItemIngredient1 = new HashSet<ItemIngredient>();
            this.ItemInventory = new HashSet<ItemInventory>();
            this.ItemPurchaseInformation = new HashSet<ItemPurchaseInformation>();
            this.ItemSaleInformation = new HashSet<ItemSaleInformation>();
            this.ItemWeight = new HashSet<ItemWeight>();
            this.ItemWeightConversionFreezen = new HashSet<ItemWeightConversionFreezen>();
            this.MasteredDetail = new HashSet<MasteredDetail>();
            this.MasteredDetail1 = new HashSet<MasteredDetail>();
            this.MetricUnitConversion = new HashSet<MetricUnitConversion>();
            this.MetricUnitConversion1 = new HashSet<MetricUnitConversion>();
            this.PoundsRange = new HashSet<PoundsRange>();
            this.VehicleType = new HashSet<VehicleType>();
            this.VehicleType1 = new HashSet<VehicleType>();
            this.OpeningClosingPlateLyingDetail = new HashSet<OpeningClosingPlateLyingDetail>();
            this.Presentation = new HashSet<Presentation>();
            this.PriceListDetail = new HashSet<PriceListDetail>();
            this.PriceListDetailFilterShow = new HashSet<PriceListDetailFilterShow>();
            this.ProductionLotLiquidation = new HashSet<ProductionLotLiquidation>();
            this.ProductionLotLiquidation1 = new HashSet<ProductionLotLiquidation>();
            this.ProductionLotLoss = new HashSet<ProductionLotLoss>();
            this.ProductionLotPayment = new HashSet<ProductionLotPayment>();
            this.ProductionLotPayment1 = new HashSet<ProductionLotPayment>();
            this.ProductionLotTrash = new HashSet<ProductionLotTrash>();
            this.ProductionScheduleRequestDetail = new HashSet<ProductionScheduleRequestDetail>();
            this.ResultProdLotReceptionDetail = new HashSet<ResultProdLotReceptionDetail>();
            this.ResultProdLotRomaneo = new HashSet<ResultProdLotRomaneo>();
            this.ResultReceptionDispatchMaterialDetail = new HashSet<ResultReceptionDispatchMaterialDetail>();
            this.RomaneoDetail = new HashSet<RomaneoDetail>();
            this.SalesOrderDetail = new HashSet<SalesOrderDetail>();
            this.SalesOrderMPMaterialDetail = new HashSet<SalesOrderMPMaterialDetail>();
            this.SalesQuotationDetail = new HashSet<SalesQuotationDetail>();
            this.SalesQuotationExterior = new HashSet<SalesQuotationExterior>();
            this.SalesRequestDetail = new HashSet<SalesRequestDetail>();
            this.SalesRequestOrQuotationDetailProductionScheduleRequestDetail = new HashSet<SalesRequestOrQuotationDetailProductionScheduleRequestDetail>();
        }
    
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int id_metricType { get; set; }
        public int id_company { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AutomaticTransferDetail> AutomaticTransferDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AutomaticTransferDetail> AutomaticTransferDetail1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CapacityContainer> CapacityContainer { get; set; }
        public virtual Company Company { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DrainingTestDetail> DrainingTestDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FrameworkContractItem> FrameworkContractItem { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IceBagRange> IceBagRange { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetail> InventoryMoveDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryMoveDetail> InventoryMoveDetail1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceCommercial> InvoiceCommercial { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceCommercialDetail> InvoiceCommercialDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceCommercialDetail> InvoiceCommercialDetail1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetail1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceExterior> InvoiceExterior { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceExteriorWeight> InvoiceExteriorWeight { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemHeadIngredient> ItemHeadIngredient { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemIngredient> ItemIngredient { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemIngredient> ItemIngredient1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemInventory> ItemInventory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemPurchaseInformation> ItemPurchaseInformation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemSaleInformation> ItemSaleInformation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemWeight> ItemWeight { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemWeightConversionFreezen> ItemWeightConversionFreezen { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MasteredDetail> MasteredDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MasteredDetail> MasteredDetail1 { get; set; }
        public virtual MetricType MetricType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MetricUnitConversion> MetricUnitConversion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MetricUnitConversion> MetricUnitConversion1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PoundsRange> PoundsRange { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehicleType> VehicleType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VehicleType> VehicleType1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpeningClosingPlateLyingDetail> OpeningClosingPlateLyingDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presentation> Presentation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceListDetail> PriceListDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceListDetailFilterShow> PriceListDetailFilterShow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionLotLiquidation> ProductionLotLiquidation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionLotLiquidation> ProductionLotLiquidation1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionLotLoss> ProductionLotLoss { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionLotPayment> ProductionLotPayment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionLotPayment> ProductionLotPayment1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionLotTrash> ProductionLotTrash { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionScheduleRequestDetail> ProductionScheduleRequestDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultProdLotReceptionDetail> ResultProdLotReceptionDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultProdLotRomaneo> ResultProdLotRomaneo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultReceptionDispatchMaterialDetail> ResultReceptionDispatchMaterialDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RomaneoDetail> RomaneoDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesOrderDetail> SalesOrderDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesOrderMPMaterialDetail> SalesOrderMPMaterialDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesQuotationDetail> SalesQuotationDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesQuotationExterior> SalesQuotationExterior { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesRequestDetail> SalesRequestDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesRequestOrQuotationDetailProductionScheduleRequestDetail> SalesRequestOrQuotationDetailProductionScheduleRequestDetail { get; set; }
    }
}
