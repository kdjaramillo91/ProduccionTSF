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
    
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.CustomerAddress = new HashSet<CustomerAddress>();
            this.CustomerPriceList = new HashSet<CustomerPriceList>();
            this.MasteredDetail = new HashSet<MasteredDetail>();
            this.PriceList = new HashSet<PriceList>();
            this.SalesOrder = new HashSet<SalesOrder>();
            this.SalesQuotation = new HashSet<SalesQuotation>();
            this.SalesRequest = new HashSet<SalesRequest>();
            this.SalesReturn = new HashSet<SalesReturn>();
        }
    
        public int id { get; set; }
        public int id_customerType { get; set; }
        public string name_customerType { get; set; }
        public bool forceToKeepAccountsCusm { get; set; }
        public bool specialTaxPayerCusm { get; set; }
        public Nullable<int> id_vendorAssigned { get; set; }
        public string name_vendorAssigned { get; set; }
        public Nullable<int> id_economicGroupCusm { get; set; }
        public string name_economicGroup { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public bool applyIva { get; set; }
        public Nullable<int> id_commissionAgent { get; set; }
        public Nullable<int> id_customerState { get; set; }
        public Nullable<int> id_clientCategory { get; set; }
        public Nullable<int> id_businessLine { get; set; }
    
        public virtual BusinessLine BusinessLine { get; set; }
        public virtual ClientCategory ClientCategory { get; set; }
        public virtual CustomerState CustomerState { get; set; }
        public virtual CustomerType CustomerType { get; set; }
        public virtual EconomicGroup EconomicGroup { get; set; }
        public virtual Person Person { get; set; }
        public virtual Person Person1 { get; set; }
        public virtual Person Person2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerAddress> CustomerAddress { get; set; }
        public virtual CustomerContact CustomerContact { get; set; }
        public virtual CustomerCreditInfo CustomerCreditInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerPriceList> CustomerPriceList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MasteredDetail> MasteredDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceList> PriceList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesOrder> SalesOrder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesQuotation> SalesQuotation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesRequest> SalesRequest { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesReturn> SalesReturn { get; set; }
    }
}
