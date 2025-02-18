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
    
    public partial class ForeignCustomerIdentification
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ForeignCustomerIdentification()
        {
            this.InvoiceExterior = new HashSet<InvoiceExterior>();
            this.SalesQuotationExterior = new HashSet<SalesQuotationExterior>();
            this.InvoiceCommercial = new HashSet<InvoiceCommercial>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_ForeignCustomer { get; set; }
        public int id_Country_IdentificationType { get; set; }
        public string numberIdentification { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public Nullable<System.DateTime> dateUpdate { get; set; }
        public Nullable<bool> printInInvoice { get; set; }
        public int AddressType { get; set; }
        public int id_country { get; set; }
        public int id_city { get; set; }
        public string addressForeign { get; set; }
        public string emailInterno { get; set; }
        public string emailInternoCC { get; set; }
        public string phone1FC { get; set; }
        public string fax1FC { get; set; }
        public int id_invoiceLanguage { get; set; }
    
        public virtual City City { get; set; }
        public virtual Country Country { get; set; }
        public virtual Country_IdentificationType Country_IdentificationType { get; set; }
        public virtual Person Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceExterior> InvoiceExterior { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesQuotationExterior> SalesQuotationExterior { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceCommercial> InvoiceCommercial { get; set; }
    }
}
