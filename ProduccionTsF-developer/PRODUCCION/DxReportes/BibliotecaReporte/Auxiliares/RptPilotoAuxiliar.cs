using BibliotecaReporte.Dataset;
using BibliotecaReporte.Model;
using BibliotecaReporte.Reportes;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares
{
    internal static class RptPilotoAuxiliar
    {
        private const string m_StoredProcedureName = "spPilotoCrytalReport";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        #region Métodos adicionales
        private static rptPilotoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptPiloto = new rptPilotoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<RptPiloto>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptPiloto.Cabecera.NewCabeceraRow();
                    cabeceraRow.Codigo = detailResult.Codigo;
                    cabeceraRow.Nombre = detailResult.Nombre;
                    cabeceraRow.Direccion = detailResult.Direccion;
                    cabeceraRow.NumeroTelefono = detailResult.NumeroTelefono;

                    rptPiloto.Cabecera.AddCabeceraRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptPiloto;
        }

        private static Stream SetDataReport(rptPilotoDataSet rptPilotoDataSet)
        {
            using (var report = new PilotoCrytalReport())
            {
                report.SetDataSource(rptPilotoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
        #endregion
    }
}
