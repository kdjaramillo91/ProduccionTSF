using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MigracionProduccionCIWebApi.Models
{
    public partial class AnswerPersonProvider
    {
        public string message { get; set; }
        public virtual PersonProvider personProvider { get; set; }

    }

    public partial class AnswerPersonCustomer
    {
        public string message { get; set; }
        public virtual PersonCustomer personCustomer { get; set; }

    }
    public partial class AnswerMigration
    {
        public string message { get; set; }

        public bool? resultado { get; set; }

    }

    public class PersonCustomer
    {
        public int id { get; set; }
        public int id_personType { get; set; }
        public string code_personType { get; set; }
        public int id_identificationType { get; set; }
        public string codeSRI_identificationType { get; set; }
        public string identification_number { get; set; }
        public string fullname_businessName { get; set; }
        public byte[] photo { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public int? id_company { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }

        //Customer
    }
    public class PersonProvider
    {
        public int id { get; set; }
        public int id_personType { get; set; }
        public string code_personType { get; set; }
        public int id_identificationType { get; set; }
        public string codeSRI_identificationType { get; set; }
        public string identification_number { get; set; }
        public string fullname_businessName { get; set; }
        public byte[] photo { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public int? id_company { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }

        //Provider

        public bool electronicDocumentIssuance { get; set; }
        public virtual ICollection<PersonProviderAccountingAccounts> ProviderAccountingAccounts { get; set; }
        public virtual PersonProviderGeneralData ProviderGeneralData { get; set; }
        public virtual PersonProviderGeneralDataEP ProviderGeneralDataEP { get; set; }
        public virtual PersonProviderGeneralDataRise ProviderGeneralDataRise { get; set; }
        public virtual ICollection<PersonProviderItem> ProviderItem { get; set; }
        public virtual ICollection<PersonProviderMailByComDivBra> ProviderMailByComDivBra { get; set; }
        public virtual PersonProviderPassportImportData ProviderPassportImportData { get; set; }
        public virtual ICollection<PersonProviderPaymentMethod> ProviderPaymentMethod { get; set; }
        public virtual ICollection<PersonProviderPaymentTerm> ProviderPaymentTerm { get; set; }
        public virtual ICollection<PersonProviderPersonAuthorizedToPayTheBill> ProviderPersonAuthorizedToPayTheBill { get; set; }
        public virtual ICollection<PersonProviderRelatedCompany> ProviderRelatedCompany { get; set; }
        public virtual ICollection<PersonProviderRetention> ProviderRetention { get; set; }
        public virtual ICollection<PersonProviderSeriesForDocuments> ProviderSeriesForDocuments { get; set; }
    }

    public partial class PersonProviderAccountingAccounts
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_company { get; set; }
        public int id_division { get; set; }
        public int id_branchOffice { get; set; }
        public int id_accountFor { get; set; }
        public int id_accountPlan { get; set; }
        public int id_account { get; set; }
        public int id_accountingAssistantDetailType { get; set; }

    }

    public partial class PersonProviderGeneralData
    {
        public int id_provider { get; set; }
        public string cod_alternate { get; set; }
        public string tradeName { get; set; }
        public Nullable<int> id_providerType { get; set; }
        public string code_providerType { get; set; }
        public string accountExecutive { get; set; }
        public string observation { get; set; }
        public string reference { get; set; }
        public Nullable<int> id_origin { get; set; }
        public Nullable<int> id_country { get; set; }
        public string code_country { get; set; }
        public Nullable<int> id_city { get; set; }
        public string code_city { get; set; }
        public Nullable<int> id_refCountry { get; set; }
        public Nullable<int> id_stateOfContry { get; set; }
        public string noFax { get; set; }
        public string phoneNumber1 { get; set; }
        public string phoneNumber2 { get; set; }
        public string bCC { get; set; }
        public Nullable<decimal> cupoCredit { get; set; }
        public Nullable<decimal> cupoEmergent { get; set; }
        public Nullable<decimal> cupoAvailable { get; set; }
        public string contactName { get; set; }
        public string contactPhoneNumber { get; set; }
        public string contactNoFax { get; set; }
        public bool electronicPayment { get; set; }
        public bool rise { get; set; }
        public bool specialTaxPayer { get; set; }
        public bool forcedToKeepAccounts { get; set; }
        public bool habitualExporter { get; set; }
        public bool taxHavenExporter { get; set; }
        public bool sponsoredLinks { get; set; }
        public Nullable<decimal> creditDiscount { get; set; }
        public Nullable<decimal> discountedDiscount { get; set; }
        public Nullable<int> id_discountToDetailApplyTo { get; set; }
        public Nullable<int> id_basisForGeneralDiscounts { get; set; }
        public Nullable<int> id_typeBoxCardAndBankAD { get; set; }
        public Nullable<int> id_boxCardAndBankAD { get; set; }

    }

    public partial class PersonProviderGeneralDataEP
    {
        public int id_provider { get; set; }
        public int id_identificationTypeEP { get; set; }
        public int id_paymentMethodEP { get; set; }
        public Nullable<int> id_bankToBelieve { get; set; }
        public Nullable<int> id_accountTypeGeneralEP { get; set; }
        public string noAccountEP { get; set; }
        public string noRefTransfer { get; set; }
        public Nullable<int> id_rtInternational { get; set; }
        public string noRoute { get; set; }
        public string emailEP { get; set; }

    }

    public partial class PersonProviderGeneralDataRise
    {
        public int id_provider { get; set; }
        public int id_categoryRise { get; set; }
        public int id_activityRise { get; set; }
        public decimal invoiceAmountRise { get; set; }

    }

    public partial class PersonProviderItem
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_item { get; set; }

    }

    public partial class PersonProviderMailByComDivBra
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_company { get; set; }
        public int id_division { get; set; }
        public int id_branchOffice { get; set; }
        public string email { get; set; }

    }

    public partial class PersonProviderPassportImportData
    {
        public int id_provider { get; set; }
        public bool appliesDoubleTaxationAgreementOnPayment { get; set; }
        public bool subjectRetentionApplicationLegalNorm { get; set; }
        public bool relatedParty { get; set; }

    }

    public partial class PersonProviderPaymentMethod
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_company { get; set; }
        public int id_division { get; set; }
        public int id_branchOffice { get; set; }
        public int id_paymentMethod { get; set; }
        public bool isPredetermined { get; set; }
        public bool isActive { get; set; }

    }

    public partial class PersonProviderPaymentTerm
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_company { get; set; }
        public int id_division { get; set; }
        public int id_branchOffice { get; set; }
        public int id_paymentTerm { get; set; }
        public bool isPredetermined { get; set; }
        public bool isActive { get; set; }

    }

    public partial class PersonProviderPersonAuthorizedToPayTheBill
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_identificationType { get; set; }
        public string identification_number { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phoneNumber1 { get; set; }
        public string phoneNumber2 { get; set; }
        public string typeReg { get; set; }
        public string codeProd { get; set; }
        public string codeEmpr { get; set; }
        public string type { get; set; }
        public Nullable<int> id_country { get; set; }
        public Nullable<int> id_bank { get; set; }
        public Nullable<int> id_accountType { get; set; }
        public string noAccount { get; set; }
        public Nullable<decimal> amount { get; set; }
        public Nullable<int> noPayments { get; set; }
        public Nullable<System.DateTime> date { get; set; }

    }

    public partial class PersonProviderRelatedCompany
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_company { get; set; }
        public int id_division { get; set; }
        public int id_branchOffice { get; set; }

    }

    public partial class PersonProviderRetention
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_retentionType { get; set; }
        public int id_retentionGroup { get; set; }
        public int id_retention { get; set; }
        public decimal percentRetencion { get; set; }

    }

    public partial class PersonProviderSeriesForDocuments
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_documentType { get; set; }
        public string serialNumber { get; set; }
        public string authorizationNumber { get; set; }
        public int initialNumber { get; set; }
        public int finalNumber { get; set; }
        public System.DateTime dateOfExpiry { get; set; }
        public int currentNumber { get; set; }
        public int id_retentionSeriesForDocumentsType { get; set; }
        public bool isActive { get; set; }

    }
}