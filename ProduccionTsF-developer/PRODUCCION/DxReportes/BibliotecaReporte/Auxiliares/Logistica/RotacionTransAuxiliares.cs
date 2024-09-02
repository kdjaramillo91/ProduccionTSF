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
    internal class RotacionTransAuxiliares
    {
        private const string m_StoredProcedureName = "spPar_RotacionTrans";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static RotacionTransDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptRemisionTrans = new RotacionTransDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<RotacionTransModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptRemisionTrans.CabeceraRotacionTrans.NewCabeceraRotacionTransRow();
                    cabeceraRow.NombreCiaFactura = detailResult.NombreCiaFactura;
                    cabeceraRow.NombreDueno = detailResult.NombreDueno;
                    cabeceraRow.Matricula = detailResult.Matricula;
                    cabeceraRow.ContadorLiqidaciones = detailResult.ContadorLiqidaciones;
                    cabeceraRow.TotalValorGuia = detailResult.TotalValorGuia;
                    cabeceraRow.TotalValor = detailResult.TotalValor;
                    cabeceraRow.TotalPendiente = detailResult.TotalPendiente;
                    cabeceraRow.TieneHunter = detailResult.TieneHunter;
                    cabeceraRow.Ruc = detailResult.Ruc;
                    cabeceraRow.Telefono = detailResult.Telefono;
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    

                    rptRemisionTrans.CabeceraRotacionTrans.AddCabeceraRotacionTransRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptRemisionTrans;
        }
        private static Stream SetDataReport(RotacionTransDataSet rptRotacionTransDataSet)
        {
            using (var report = new RptRotacionTrans())
            {
                report.SetDataSource(rptRotacionTransDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}