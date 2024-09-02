using DevExpress.XtraPrinting;
using System;

namespace DXPANACEASOFT.Models.Dto
{
    public class CustodianIncomeDto
    {
        public int Id { get; set; }
        public int id_RemissionGuide { get; set; }

        public int id_PersonCompanyCustodian1 { get; set; }
        public Nullable<int> id_PersonCompanyCustodian2 { get; set; }

        public int id_FishingSite1 { get; set; }
        public Nullable<int> id_FishingSite2 { get; set; }

        public int id_FishingCustodian1 { get; set; }
        public Nullable<int> id_FishingCustodian2 { get; set; }

        public string fishingCustodianField1 { get; set; }
        public string fishingCustodianField2 { get; set; }

        public string fishingCustodianField1Descrip { get; set; }
        public string fishingCustodianField2Descrip { get; set; }

        public decimal fishingCustodian1Value { get; set; }
        public decimal? fishingCustodian2Value { get; set; }

        public virtual Document Document { get; set; }
        public virtual RemissionGuide RemissionGuide { get; set; }

        public virtual Person PersonCompanyCustodian1 { get; set; }
        public virtual Person PersonCompanyCustodian2 { get; set; }

        public virtual FishingSite FishingSite1 { get; set; }
        public virtual FishingSite FishingSite2 { get; set; }

        public virtual FishingCustodian FishingCustodian1 { get; set; }
        public virtual FishingCustodian FishingCustodian2 { get; set; }

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

        public string personCompanyCustodian1Name { get; set; }
        public string personCompanyCustodian2Name { get; set; }
        public string fishingZoneSite1Name { get; set; }
        public string fishingZoneSite2Name { get; set; }
        public string fishingCustodian1ValueView { get; set; }
        public string fishingCustodian2ValueView { get; set; }

        public decimal fishingCustodianValue { get; set; }

    }
}