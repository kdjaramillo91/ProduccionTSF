using BibliotecaReporte.Dataset.Configuraciones;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Configuracion;
using BibliotecaReporte.Reportes.Configuracion;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.Configuracion
{
    internal class IntegrationProcessAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_IntegrationProcess";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static IntegrationProcessDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptIntegrationprocess = new IntegrationProcessDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<IntegrationProcessModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptIntegrationprocess.CabeceraIntegrationProcess.NewCabeceraIntegrationProcessRow();
                    cabeceraRow.Code_lote = detailResult.Code_lote;
                    cabeceraRow.Tipo_Integracion = detailResult.Tipo_Integracion;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.Fecha_Contabilizacion = detailResult.Fecha_Contabilizacion;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.Documento = detailResult.Documento;
                    cabeceraRow.Fecha_Emision = detailResult.Fecha_Emision;
                    cabeceraRow.GlossData1 = detailResult.GlossData1;
                    cabeceraRow.Valor_Documento = detailResult.Valor_Documento;
                    cabeceraRow.Total_Lote_Integracion = detailResult.Total_Lote_Integracion;
                    cabeceraRow.Numero_Documentos = detailResult.Numero_Documentos;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;


                    rptIntegrationprocess.CabeceraIntegrationProcess.AddCabeceraIntegrationProcessRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptIntegrationprocess;
        }

        private static Stream SetDataReport(IntegrationProcessDataSet rptFacturacomercialDataSet)
        {
            using (var report = new RptIntegrationProcess())
            {
                report.SetDataSource(rptFacturacomercialDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
