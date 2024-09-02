using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.Logistica;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Logistica;
using BibliotecaReporte.Reportes.Logistica;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Logistica
{
    internal class LogisticaGuiaAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LogisticaGuia";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptLogisticaGuia = new LogisticaGuiaDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LogisticaGuiaModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptLogisticaGuia.CabeceraLogisticaGuia.NewCabeceraLogisticaGuiaRow();
                    cabeceraRow.Secuencial = detailResult.Secuencial;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.ProveedorCompleto = detailResult.ProveedorCompleto;
                    cabeceraRow.Comprador = detailResult.Comprador;
                    cabeceraRow.FechaDespacho = detailResult.FechaDespacho;
                    cabeceraRow.LibrasProgramadas = detailResult.LibrasProgramadas;
                    cabeceraRow.LibrasRemitidas = detailResult.LibrasRemitidas;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.NombreCompania = detailResult.NombreCompania;
                    cabeceraRow.RucCompania = detailResult.RucCompania;
                    cabeceraRow.TelefonoCompania = detailResult.TelefonoCompania;
                    cabeceraRow.TipoGuia = detailResult.TipoGuia;
                    cabeceraRow.PlantaProceso = detailResult.PlantaProceso;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;

                    rptLogisticaGuia.CabeceraLogisticaGuia.AddCabeceraLogisticaGuiaRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptLogisticaGuia,
                rptCompaniaInfo,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] rptLogisticaGuiaDataSet)
        {
            using (var report = new RptLogisticaGuia())
            {
                report.SetDataSource(rptLogisticaGuiaDataSet[0]);

                // Subreporte Cia's
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptLogisticaGuiaDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}