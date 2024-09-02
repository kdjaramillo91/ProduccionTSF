using DevExpress.CodeParser;
using DevExpress.PivotGrid.Filtering;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Rendering;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Operations;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderCustodianIncome
    {
        private static DBContext db1 = null;
        const string DOCUMENT_STATE_APROBADO_CODE = "03";
        const string SELECT_CUSTODIANINCOME =   "SELECT	top 1 ci.Id, id_RemissionGuide, id_PersonCompanyCustodian1, id_PersonCompanyCustodian2, " +
                                                "id_FishingSite1, id_FishingSite2, " +
                                                "id_FishingCustodian1, id_FishingCustodian2, " +
                                                "fishingCustodianField1, fishingCustodianField2, " +
                                                "fishingCustodianValue1, fishingCustodianValue2 " +
                                                "FROM CustodianIncome ci inner join " +
                                                " Document d on d.id = ci.id inner join " +
                                                " DocumentState dt on dt.id = d.id_documentState " +
                                                " WHERE ci.id_RemissionGuide = @id_RemissionGuide AND "+
                                                " dt.code = @codestatus";

        public static CustodianIncomeDto CustodianIncomeForView(int remissionGuideId)
        {

            var result = DapperConnection.Execute<CustodianIncome>(SELECT_CUSTODIANINCOME, new
            {
                id_RemissionGuide = remissionGuideId,
                codestatus = DOCUMENT_STATE_APROBADO_CODE
            })?.FirstOrDefault();

            if(result == null) return null;

            db1 = new DBContext();
            //DocumentState documentState = db1.DocumentState.FirstOrDefault(r => r.code == DOCUMENT_STATE_APROBADO_CODE);
            //Document document = db1.Document.Include("DocumentState").FirstOrDefault(r => r.id == result.Id);
            
            //if (document.DocumentState.code == DOCUMENT_STATE_APROBADO_CODE)
            //{
                CustodianIncomeDto custodianIncome = new CustodianIncomeDto();
                custodianIncome.personCompanyCustodian1Name = db1.Person.FirstOrDefault(r => r.id == result.id_PersonCompanyCustodian1)?.fullname_businessName;
                if(result.id_PersonCompanyCustodian2.HasValue)
                {
                    custodianIncome.personCompanyCustodian2Name = db1.Person.FirstOrDefault(r => r.id == result.id_PersonCompanyCustodian2)?.fullname_businessName;
                }

                //custodianIncome.fishingZoneSite1Name = db1.FishingSite.Include("FishingZone").Where(r=> r.id == result.id_FishingSite1).Select(r => $"{r.FishingZone.name} | {r.name}" ).FirstOrDefault();
                var fishingSite1 = db1.FishingSite.Include("FishingZone").FirstOrDefault(r => r.id == result.id_FishingSite1);
                custodianIncome.fishingZoneSite1Name = $"{fishingSite1.FishingZone.name} | {fishingSite1.name}";
                if (result.id_FishingSite2.HasValue)
                {
                    var fishingSite2 = db1.FishingSite.Include("FishingZone").FirstOrDefault(r => r.id == result.id_FishingSite2);
                    custodianIncome.fishingZoneSite2Name = $"{fishingSite2.FishingZone.name} | {fishingSite2.name}";
                }

                var resultFishingCustodian = DapperConnection.Execute<CustodianIncomeDto>($"SELECT {result.fishingCustodianField1} fishingCustodianValue  FROM FishingCustodian Where id= @id_FishingCustodian", new
                {
                    id_FishingCustodian = result.id_FishingCustodian1
                })?.FirstOrDefault();

                custodianIncome.fishingCustodian1ValueView = $"{switchFieldDesrip(result.fishingCustodianField1)} | {resultFishingCustodian.fishingCustodianValue.ToCurrencyFormat()}";

                if (result.id_FishingCustodian2.HasValue)
                {
                    var resultFishingCustodian2 = DapperConnection.Execute<CustodianIncomeDto>($"SELECT {result.fishingCustodianField2} fishingCustodianValue  FROM FishingCustodian Where id= @id_FishingCustodian", new
                    {
                        id_FishingCustodian = result.id_FishingCustodian2
                    })?.FirstOrDefault();

                    custodianIncome.fishingCustodian2ValueView = $"{switchFieldDesrip(result.fishingCustodianField2)} | {resultFishingCustodian2.fishingCustodianValue.ToCurrencyFormat()}";
                }
                return custodianIncome;
            //}
            //return null;
        }

        private static string switchFieldDesrip(string field)
        {
            string fieldDescrip = null;
            switch (field)
            {
                case "patrol":
                    fieldDescrip = "Patrulla";
                    break;
                case "semiComplete":
                    fieldDescrip = "Semi Completa";
                    break;
                case "truckDriver":
                    fieldDescrip = "Camioneta";
                    break;
                case "changeHG":
                    fieldDescrip = "H/Cambina Gye";
                    break;
                case "cabinHR":
                    fieldDescrip = "H/Cabina Ruta";
                    break;
            }
            return fieldDescrip;
        }
    }
}