using BibliotecaReporte.Dataset.Produccion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Produccion;
using BibliotecaReporte.Reportes.Produccion;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Produccion
{
    internal class HorasNoProductivasSubAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_RptHorasNoTrabajadasDetalleReport";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static HorasNoProductivasSubDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptHorasNoProductivasSub = new HorasNoProductivasSubDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<HorasNoProductivasSubModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptHorasNoProductivasSub.HorasNoProductivasSubReporte.NewHorasNoProductivasSubReporteRow();
                    cabeceraRow.Motivo = detailResult.Motivo;
                    cabeceraRow.Paradas = detailResult.Paradas;
                    cabeceraRow.Minutos = detailResult.Minutos;
                    cabeceraRow.TotalMinutos = detailResult.TotalMinutos;
                    cabeceraRow.IdMot = detailResult.IdMotivo;
                    cabeceraRow.Code = detailResult.CodigoMotivo;
                    cabeceraRow.HoraMinutos = detailResult.HoraMinutos;
                    cabeceraRow.HoraTotal = detailResult.HoraTotal;
                    rptHorasNoProductivasSub.HorasNoProductivasSubReporte.AddHorasNoProductivasSubReporteRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptHorasNoProductivasSub;
        }

        private static Stream SetDataReport(HorasNoProductivasSubDataSet rptHorasNoProductivasSubDataSet)
        {
            using (var report = new RptSubHorasNoProductivas())
            {
                report.SetDataSource(rptHorasNoProductivasSubDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
