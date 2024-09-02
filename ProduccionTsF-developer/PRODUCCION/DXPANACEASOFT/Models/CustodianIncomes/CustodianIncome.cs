using DevExpress.XtraPrinting;
using System;

namespace DXPANACEASOFT.Models
{
    public partial class CustodianIncome
    {
        public int  Id { get; set; }
        public int id_RemissionGuide { get; set; }
        
        public int id_PersonCompanyCustodian1 { get; set; }
        public Nullable<int> id_PersonCompanyCustodian2 { get; set; }

        public int id_FishingSite1 { get; set; }
        public Nullable<int> id_FishingSite2 { get; set; }

        public int id_FishingCustodian1 { get; set; }
        public Nullable<int> id_FishingCustodian2 { get; set; }

        public string fishingCustodianField1 { get; set; }
        public string fishingCustodianField2 { get; set; }
        public decimal fishingCustodianValue1 { get; set; }
        public Nullable<decimal> fishingCustodianValue2 { get; set; }


        public string remissionGuideProviderName { get; set; }
        public string remissionGuideProductionUnitProviderName { get; set; }
        public string remissionGuidePoolReference { get; set; }
        public decimal remissionGuidePoundTotal { get; set; }
        public string remissionGuideFishingZoneName { get; set; }
        public string remissionGuideFishingSiteName { get; set; }
        public string remissionGuideRoute { get; set; }
        public string remissionGuideShippingTypeName { get; set; }
        public string remissionGuidesDriverName { get; set; }
        public string remissionGuidesProcessPlant { get; set; }
        public string remissionGuidesProviderTransportName { get; set; }
        public string remissionGuidesCarRegistration { get; set; }
        public string remissionGuidesTransportBillingName { get; set; }
        public decimal remissionGuidesTransportValuePrice { get; set; }

        //public bool remissionGuideValidateData { get; set; }

        public virtual Document Document { get; set; }
        public virtual RemissionGuide RemissionGuide { get; set; }

        public virtual Person PersonCompanyCustodian1 { get; set; }
        public virtual Person PersonCompanyCustodian2 { get; set; }

        public virtual FishingSite FishingSite1 { get; set; }
        public virtual FishingSite FishingSite2 { get; set; }

        public virtual FishingCustodian FishingCustodian1 { get; set; }
        public virtual FishingCustodian FishingCustodian2 { get; set; }

    }
}