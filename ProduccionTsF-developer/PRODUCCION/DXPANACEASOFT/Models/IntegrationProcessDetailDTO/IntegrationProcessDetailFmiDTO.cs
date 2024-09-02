
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.IntegrationProcessDetailDTO
{
    public class IntegrationProcessDetailDTO
    {
        private static IntegrationProcessDetailDTO GetIntegrationProcessDetailDTO(IntegrationProcessDetail integrationProcessDetail)
        {
            var integrationProcessDetailDTO = new IntegrationProcessDetailDTO
                {
                    id = integrationProcessDetail.id,
                    id_Lote = integrationProcessDetail.id_Lote,
                    integrationProcessCodeLote = integrationProcessDetail.IntegrationProcess.codeLote,
                    integrationProcessIntegrationStateName =
                        integrationProcessDetail.IntegrationProcess.IntegrationState.name,
                    integrationProcessDocumentTypeDescription =
                        integrationProcessDetail.IntegrationProcess.DocumentType.description,
                    integrationProcessDateAccounting = integrationProcessDetail.IntegrationProcess.dateAccounting,
                    id_Document = integrationProcessDetail.id_Document,
                    documentNumber = integrationProcessDetail.Document.number,
                    documentEmissionDate = integrationProcessDetail.Document.emissionDate,
                    sequentialLiquidation = integrationProcessDetail.Document.Lot?.ProductionLot?.sequentialLiquidation
                        ?.ToString(),
                    id_StatusDocument = integrationProcessDetail.id_StatusDocument,
                    glossDataHTML = integrationProcessDetail.glossData,
                    glossData = GetOnlyText(integrationProcessDetail.glossData),
                    totalValueDocument = integrationProcessDetail.totalValueDocument,
                    id_UserCreate = integrationProcessDetail.id_UserCreate,
                    id_UserUpdate = integrationProcessDetail.id_UserUpdate,
                    dateCreate = integrationProcessDetail.dateCreate,
                    dateUpdate = integrationProcessDetail.dateUpdate,
                    dateLastUpdateDocument = integrationProcessDetail.dateLastUpdateDocument
                };
            return integrationProcessDetailDTO;
        }

        private static string GetOnlyText(string glossData)
        {
            string[] stringSeparators = new string[] { "<span><span class='ip_title'>" };
            string[] stringSeparators2 = new string[] { "</span><span class='ip_text'>" };
            string[] stringSeparators3 = new string[] { "</span></span>" };
            var arrayGlossData = glossData.Split(stringSeparators, StringSplitOptions.None);

            string text = string.Empty;
            if (arrayGlossData.Length > 0)
            {
                for (int i = 1; i < arrayGlossData.Length; i++)
                {
                    var arrayGlossData2 = arrayGlossData[i].Split(stringSeparators2, StringSplitOptions.None);
                    if (arrayGlossData2.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(text)) text += " ";
                        //text += "<b>" + arrayGlossData2[0] + "</b>";
                        text += arrayGlossData2[0];
                        var arrayGlossData3 = arrayGlossData2[1].Split(stringSeparators3, StringSplitOptions.None);
                        if (arrayGlossData3.Length > 0)
                        {
                            text += " " + arrayGlossData3[0];
                        }
                    }
                }
            }
            return text;
        }

        public static List<IntegrationProcessDetailDTO> GetListIntegrationProcessDetailDTO(List<IntegrationProcessDetail> listIntegrationProcessDetail)
        {
            return listIntegrationProcessDetail.Select(item => GetIntegrationProcessDetailDTO(item)).ToList();
        }

        public int id { get; set; }
        public int id_Lote { get; set; }
        public string integrationProcessCodeLote { get; set; }
        public string integrationProcessIntegrationStateName { get; set; }
        public string integrationProcessDocumentTypeDescription { get; set; }
        public System.DateTime integrationProcessDateAccounting { get; set; }
        public int id_Document { get; set; }
        public string documentNumber { get; set; }
        public System.DateTime documentEmissionDate { get; set; }
        public string sequentialLiquidation { get; set; }
        public int id_StatusDocument { get; set; }
        public string glossData { get; set; }
        public string glossDataHTML { get; set; }
        public decimal totalValueDocument { get; set; }
        public int id_UserCreate { get; set; }
        public int id_UserUpdate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public Nullable<System.DateTime> dateLastUpdateDocument { get; set; }

    }

    
}