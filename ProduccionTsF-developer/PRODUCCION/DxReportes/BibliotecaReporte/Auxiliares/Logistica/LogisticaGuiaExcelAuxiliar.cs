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
    internal class LogisticaGuiaExcelAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LogisticaGuiaExcel";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static LogisticaGuiaExcelDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptLogisticaGuiaExcel = new LogisticaGuiaExcelDataSet();
            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }
                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LogisticaGuiaExcelModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptLogisticaGuiaExcel.CabeceraLogisticaGuiaExcel.NewCabeceraLogisticaGuiaExcelRow();
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
                    rptLogisticaGuiaExcel.CabeceraLogisticaGuiaExcel.AddCabeceraLogisticaGuiaExcelRow(cabeceraRow);
                }
                if (db.State == ConnectionState.Open) { db.Close(); }
            }
            return rptLogisticaGuiaExcel;
        }
        private static Stream SetDataReport(LogisticaGuiaExcelDataSet rptLogisticaGuiaExcelDataSet)
        {
            using (var report = new RptLogisticaGuiaExcel())
            {
                report.SetDataSource(rptLogisticaGuiaExcelDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.Excel);
                report.Close();
                return streamReport;
            }
        }
    }
}