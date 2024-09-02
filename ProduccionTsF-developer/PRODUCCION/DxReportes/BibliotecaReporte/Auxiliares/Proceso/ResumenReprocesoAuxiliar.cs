using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.Procesos;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Procesos;
using BibliotecaReporte.Reportes.Procesos;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Proceso
{
    internal class ResumenReprocesoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ResumenReproceso";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptProcesoInternoResumen = new ResumenDeReprocesoDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();
            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ResumenReprocesoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptProcesoInternoResumen.CabeceraResumenDeReproceso.NewCabeceraResumenDeReprocesoRow();
                    cabeceraRow.LineaInventario = detailResult.LineaInventario;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.TipoProducto = detailResult.TipoProducto;
                    cabeceraRow.G1 = detailResult.G1;
                    cabeceraRow.G2 = detailResult.G2;
                    cabeceraRow.LibrasEgreso = detailResult.LibrasEgreso;
                    cabeceraRow.LibrasReproceso = detailResult.LibrasReproceso;
                    cabeceraRow.LibrasBajas = detailResult.LibrasBajas;
                    cabeceraRow.Porcentaje = detailResult.Porcentaje;
                    cabeceraRow.G3 = detailResult.G3;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    

                    rptProcesoInternoResumen.CabeceraResumenDeReproceso.AddCabeceraResumenDeReprocesoRow(cabeceraRow);
                }
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);
                if (db.State == ConnectionState.Open) { db.Close(); }
            }
            var dataSet = new DataSet[]
{
                rptProcesoInternoResumen,
                rptCompaniaInfo,
};
            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptResumenDeReprocesoDataSet)
        {
            using (var report = new RptResumenReproceso())
            {
                report.SetDataSource(rptResumenDeReprocesoDataSet[0]);
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptResumenDeReprocesoDataSet[1]);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}