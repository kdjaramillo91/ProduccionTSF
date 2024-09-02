using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
    public interface IIntegrationProcessActionOutput
    {
        string FindDocument(
            DBContext db,
            tbsysIntegrationDocumentConfig integrationConfig,
            ref IEnumerable<Document> preDocument);

        Dictionary<string, string> GetGlossX(
            DBContext db,
            int id_document,
            string code_documentType);

        decimal GetTotalValue(Document document);

        List<IntegrationProcessPrintGroup> PrintGroup(
            DBContext db,
            List<IntegrationProcessDetail> integrationProcessDetailList);

        string Validate(
            DBContext db,
            int id_ActiveUser,
            DocumentType documentType,
            IntegrationProcess integrationProcessLote,
            List<IntegrationProcessDetail> integrationProcessDetailList,
            List<AdvanceParametersDetail> _parametersDetail,
            Func<DBContext, int, int, string, string, string, List<AdvanceParametersDetail>, int> saveMessageError);

        string SaveOutput(
            DBContext db,
            int id_ActiveUser,
            DocumentType documentType,
            ref IntegrationProcess integrationProcessLote,
            List<IntegrationProcessDetail> integrationProcessDetailList,
            List<AdvanceParametersDetail> _parametersDetail,
            Func<DBContext, int, int, string, string, string, List<AdvanceParametersDetail>, int> saveMessageError);

        string TransferData(
            DBContext db,
            int id_ActiveUser,
            int id_IntegrationProcessLote,
            List<IntegrationProcessOutput> _integrationProcessOutputs,
            Func<DBContext, int, int, string, string, string, List<AdvanceParametersDetail>, int> saveMessageError);
    }
}
