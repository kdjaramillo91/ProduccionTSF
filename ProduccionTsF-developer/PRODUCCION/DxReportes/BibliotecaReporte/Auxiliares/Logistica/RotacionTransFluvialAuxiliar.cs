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
    internal class RotacionTransFluvialAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_RotacionTransFluvial";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static RotacionTransFluvialDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptRotacionTransFluvial = new RotacionTransFluvialDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<RotacionTransFluvialModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptRotacionTransFluvial.CabeceraRotacionTransFluvial.NewCabeceraRotacionTransFluvialRow();
                    cabeceraRow.NombreCiaFactura = detailResult.NombreCiaFactura;
                    cabeceraRow.NombreDueno = detailResult.NombreDueno;
                    cabeceraRow.Matricula = detailResult.Matricula;
                    cabeceraRow.ContadorLiqidaciones = detailResult.ContadorLiqidaciones;
                    cabeceraRow.TotalValor = detailResult.TotalValor;
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;

                    rptRotacionTransFluvial.CabeceraRotacionTransFluvial.AddCabeceraRotacionTransFluvialRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptRotacionTransFluvial;
        }

        private static Stream SetDataReport(RotacionTransFluvialDataSet rptRotaciontransfluvialDataSet)
        {
            using (var report = new RptRotacionTransFluvial())
            {
                report.SetDataSource(rptRotaciontransfluvialDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}