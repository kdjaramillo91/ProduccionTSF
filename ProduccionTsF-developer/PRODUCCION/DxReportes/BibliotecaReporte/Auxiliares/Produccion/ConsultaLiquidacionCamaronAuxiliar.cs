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
   internal  class ConsultaLiquidacionCamaronAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ConsultaLiquidacionCamaron";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static ConsultaLiquidacionCamaronDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptConsultaLiquidacionCamaron = new ConsultaLiquidacionCamaronDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ConsultaLiquidacionCamaronModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptConsultaLiquidacionCamaron.CabeceraConsultaLiquidacionCamaron.NewCabeceraConsultaLiquidacionCamaronRow();
                    cabeceraRow.NombreCompania = detailResult.NombreCompania;
                    cabeceraRow.RucCompania = detailResult.RucCompania;
                    cabeceraRow.NumeroCompania = detailResult.NumeroCompania;
                    cabeceraRow.SecuenciaTransaccional = detailResult.SecuenciaTransaccional;
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.FechaLiquidacion = detailResult.FechaLiquidacion;
                    cabeceraRow.NumeroLoteInterno = detailResult.NumeroLoteInterno;
                    cabeceraRow.LibrasRecibidas = detailResult.LibrasRecibidas;
                    cabeceraRow.NombreProceso = detailResult.NombreProceso;
                    cabeceraRow.TotalPagar = detailResult.TotalPagar;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    cabeceraRow.Inicio = detailResult.Inicio;
                    cabeceraRow.Fin = detailResult.Fin;
                    rptConsultaLiquidacionCamaron.CabeceraConsultaLiquidacionCamaron.AddCabeceraConsultaLiquidacionCamaronRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptConsultaLiquidacionCamaron;
        }
        private static Stream SetDataReport(ConsultaLiquidacionCamaronDataSet rptConsultaLiquidacionCamaronDataSet)
        {
            using (var report = new RptConsultaLiquidacionCamaron())
            {
                report.SetDataSource(rptConsultaLiquidacionCamaronDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
