﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBContextoListaLP : DbContext
    {
        public DBContextoListaLP()
            : base("name=DBContextoListaLP")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AccountDetailAssistantType> AccountDetailAssistantType { get; set; }
        public virtual DbSet<AccountFor> AccountFor { get; set; }
        public virtual DbSet<AccountingAssistant> AccountingAssistant { get; set; }
        public virtual DbSet<AccountingAssistantDetailType> AccountingAssistantDetailType { get; set; }
        public virtual DbSet<AccountPlan> AccountPlan { get; set; }
        public virtual DbSet<AccountType> AccountType { get; set; }
        public virtual DbSet<AccountTypeGeneral> AccountTypeGeneral { get; set; }
        public virtual DbSet<ActivityRise> ActivityRise { get; set; }
        public virtual DbSet<AddressType> AddressType { get; set; }
        public virtual DbSet<AssistantType> AssistantType { get; set; }
        public virtual DbSet<BasisForGeneralDiscounts> BasisForGeneralDiscounts { get; set; }
        public virtual DbSet<BoxCardAndBank> BoxCardAndBank { get; set; }
        public virtual DbSet<BranchOffice> BranchOffice { get; set; }
        public virtual DbSet<BusinessGroup> BusinessGroup { get; set; }
        public virtual DbSet<CalendarPriceList> CalendarPriceList { get; set; }
        public virtual DbSet<CalendarPriceListType> CalendarPriceListType { get; set; }
        public virtual DbSet<CategoryActivityRise> CategoryActivityRise { get; set; }
        public virtual DbSet<CategoryRise> CategoryRise { get; set; }
        public virtual DbSet<Certification> Certification { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<ClassShrimp> ClassShrimp { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<ComparisonOperator> ComparisonOperator { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Country_IdentificationType> Country_IdentificationType { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerType> CustomerType { get; set; }
        public virtual DbSet<DataType> DataType { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DiscountToDetailApplyTo> DiscountToDetailApplyTo { get; set; }
        public virtual DbSet<Division> Division { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentState> DocumentState { get; set; }
        public virtual DbSet<DocumentStateChange> DocumentStateChange { get; set; }
        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<DrainingTest> DrainingTest { get; set; }
        public virtual DbSet<DrainingTestDetail> DrainingTestDetail { get; set; }
        public virtual DbSet<EconomicGroup> EconomicGroup { get; set; }
        public virtual DbSet<EmissionPoint> EmissionPoint { get; set; }
        public virtual DbSet<EmissionType> EmissionType { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EnvironmentType> EnvironmentType { get; set; }
        public virtual DbSet<GeneralContactData> GeneralContactData { get; set; }
        public virtual DbSet<GroupPersonByRol> GroupPersonByRol { get; set; }
        public virtual DbSet<GroupPersonByRolDetail> GroupPersonByRolDetail { get; set; }
        public virtual DbSet<GroupPersonByRolhomologation> GroupPersonByRolhomologation { get; set; }
        public virtual DbSet<IdentificationType> IdentificationType { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemSaleInformation> ItemSaleInformation { get; set; }
        public virtual DbSet<ItemSize> ItemSize { get; set; }
        public virtual DbSet<ItemSizeClass> ItemSizeClass { get; set; }
        public virtual DbSet<ItemSizeProcessPLOrder> ItemSizeProcessPLOrder { get; set; }
        public virtual DbSet<ItemSizeProcessTypePriceList> ItemSizeProcessTypePriceList { get; set; }
        public virtual DbSet<ItemTaxation> ItemTaxation { get; set; }
        public virtual DbSet<LiquidationMaterialSupplies> LiquidationMaterialSupplies { get; set; }
        public virtual DbSet<LiquidationMaterialSuppliesDetail> LiquidationMaterialSuppliesDetail { get; set; }
        public virtual DbSet<LoginLog> LoginLog { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<MetricType> MetricType { get; set; }
        public virtual DbSet<MetricUnit> MetricUnit { get; set; }
        public virtual DbSet<MetricUnitConversion> MetricUnitConversion { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<ModuleTController> ModuleTController { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Origin> Origin { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethod { get; set; }
        public virtual DbSet<PaymentMethodPaymentTerm> PaymentMethodPaymentTerm { get; set; }
        public virtual DbSet<PaymentTerm> PaymentTerm { get; set; }
        public virtual DbSet<PenalityClassShrimp> PenalityClassShrimp { get; set; }
        public virtual DbSet<PenalityClassShrimpDetails> PenalityClassShrimpDetails { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PersonType> PersonType { get; set; }
        public virtual DbSet<PoundsRange> PoundsRange { get; set; }
        public virtual DbSet<PriceList> PriceList { get; set; }
        public virtual DbSet<PriceListClassShrimp> PriceListClassShrimp { get; set; }
        public virtual DbSet<PriceListItemSizeDetail> PriceListItemSizeDetail { get; set; }
        public virtual DbSet<PriceListPersonPersonRol> PriceListPersonPersonRol { get; set; }
        public virtual DbSet<ProcessType> ProcessType { get; set; }
        public virtual DbSet<ProductionLot> ProductionLot { get; set; }
        public virtual DbSet<ProductionLotDetail> ProductionLotDetail { get; set; }
        public virtual DbSet<ProductionLotState> ProductionLotState { get; set; }
        public virtual DbSet<Provider> Provider { get; set; }
        public virtual DbSet<ProviderAccountingAccounts> ProviderAccountingAccounts { get; set; }
        public virtual DbSet<ProviderGeneralData> ProviderGeneralData { get; set; }
        public virtual DbSet<ProviderGeneralDataEP> ProviderGeneralDataEP { get; set; }
        public virtual DbSet<ProviderGeneralDataRise> ProviderGeneralDataRise { get; set; }
        public virtual DbSet<ProviderMailByComDivBra> ProviderMailByComDivBra { get; set; }
        public virtual DbSet<ProviderPassportImportData> ProviderPassportImportData { get; set; }
        public virtual DbSet<ProviderPaymentMethod> ProviderPaymentMethod { get; set; }
        public virtual DbSet<ProviderPaymentTerm> ProviderPaymentTerm { get; set; }
        public virtual DbSet<ProviderPaymentTermMethod> ProviderPaymentTermMethod { get; set; }
        public virtual DbSet<ProviderPersonAuthorizedToPayTheBill> ProviderPersonAuthorizedToPayTheBill { get; set; }
        public virtual DbSet<ProviderRelatedCompany> ProviderRelatedCompany { get; set; }
        public virtual DbSet<ProviderRetention> ProviderRetention { get; set; }
        public virtual DbSet<ProviderSeriesForDocuments> ProviderSeriesForDocuments { get; set; }
        public virtual DbSet<ProviderType> ProviderType { get; set; }
        public virtual DbSet<PurchaseOrderShippingType> PurchaseOrderShippingType { get; set; }
        public virtual DbSet<PurchaseReason> PurchaseReason { get; set; }
        public virtual DbSet<Rate> Rate { get; set; }
        public virtual DbSet<ResultProdLotReceptionDetail> ResultProdLotReceptionDetail { get; set; }
        public virtual DbSet<ResultReceptionDispatchMaterial> ResultReceptionDispatchMaterial { get; set; }
        public virtual DbSet<ResultReceptionDispatchMaterialDetail> ResultReceptionDispatchMaterialDetail { get; set; }
        public virtual DbSet<Retention> Retention { get; set; }
        public virtual DbSet<RetentionGroup> RetentionGroup { get; set; }
        public virtual DbSet<RetentionSeriesForDocumentsType> RetentionSeriesForDocumentsType { get; set; }
        public virtual DbSet<RetentionType> RetentionType { get; set; }
        public virtual DbSet<Rol> Rol { get; set; }
        public virtual DbSet<RtInternational> RtInternational { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<SettingDataType> SettingDataType { get; set; }
        public virtual DbSet<SettingDetail> SettingDetail { get; set; }
        public virtual DbSet<SettingNotification> SettingNotification { get; set; }
        public virtual DbSet<SettingPriceList> SettingPriceList { get; set; }
        public virtual DbSet<StateOfContry> StateOfContry { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TAction> TAction { get; set; }
        public virtual DbSet<TaxType> TaxType { get; set; }
        public virtual DbSet<tbsysDocumentTypeDocumentState> tbsysDocumentTypeDocumentState { get; set; }
        public virtual DbSet<TController> TController { get; set; }
        public virtual DbSet<TypeBoxCardAndBank> TypeBoxCardAndBank { get; set; }
        public virtual DbSet<TypeFiltersConfiguration> TypeFiltersConfiguration { get; set; }
        public virtual DbSet<TypeFiltersConfigurationComparisonOperator> TypeFiltersConfigurationComparisonOperator { get; set; }
        public virtual DbSet<TypeINP> TypeINP { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserGroup> UserGroup { get; set; }
        public virtual DbSet<UserGroupMenu> UserGroupMenu { get; set; }
        public virtual DbSet<UserMenu> UserMenu { get; set; }
        public virtual DbSet<UserRol> UserRol { get; set; }
        public virtual DbSet<UserRolUser> UserRolUser { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<VisualizationTypeData> VisualizationTypeData { get; set; }
        public virtual DbSet<DocumentTemporalesvm> DocumentTemporalesvm { get; set; }
        public virtual DbSet<ItemSizePriceClass> ItemSizePriceClass { get; set; }
        public virtual DbSet<MailConfiguration> MailConfiguration { get; set; }
        public virtual DbSet<Prcarga> Prcarga { get; set; }
        public virtual DbSet<PrcargaCola> PrcargaCola { get; set; }
        public virtual DbSet<priceList226> priceList226 { get; set; }
        public virtual DbSet<PriceListItemSizeDetailResp> PriceListItemSizeDetailResp { get; set; }
        public virtual DbSet<PriceListItemSizeDetailTemp> PriceListItemSizeDetailTemp { get; set; }
    }
}
