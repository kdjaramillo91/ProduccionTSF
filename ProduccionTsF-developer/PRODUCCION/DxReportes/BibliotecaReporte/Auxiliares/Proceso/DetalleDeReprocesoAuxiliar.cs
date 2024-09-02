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
    internal class DetalleDeReprocesoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_DetalleDeReproceso";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptPDetalleReproceso = new DetalleDeReprocesoDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<DetalleDeReprocesoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptPDetalleReproceso.cabeceraDetalleDeReproceso.NewcabeceraDetalleDeReprocesoRow();
                    cabeceraRow.Agr1Proceso = detailResult.Agr1Proceso;
                    cabeceraRow.PRProceso = detailResult.PRProceso;
                    cabeceraRow.Agr2Prproc = detailResult.Agr2Prproc;
                    cabeceraRow.LineaInv = detailResult.LineaInv;
                    cabeceraRow.ITEMEGRESO = detailResult.ITEMEGRESO;
                    cabeceraRow.ITEMINGRESO = detailResult.ITEMINGRESO;
                    cabeceraRow.LibrasIngreso = detailResult.LibrasIngreso;
                    cabeceraRow.LbrxBajas = detailResult.LbrxBajas;
                    cabeceraRow.Rend = detailResult.Rend;
                    cabeceraRow.Agr3Tipo = detailResult.Agr3Tipo;
                    cabeceraRow.Tipo = detailResult.Tipo;
                    cabeceraRow.RESULT = detailResult.RESULT;
                    cabeceraRow.Lbsrecibidas = detailResult.Lbsrecibidas;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.FF = detailResult.FF;
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.RucCia = detailResult.RucCia;
                    cabeceraRow.DireccionCia = detailResult.DireccionCia;
                    
                    rptPDetalleReproceso.cabeceraDetalleDeReproceso.AddcabeceraDetalleDeReprocesoRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptPDetalleReproceso,
                rptCompaniaInfo,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] rptDetalleReprocesoDataSet)
        {
            using (var report = new RptDetalleDeReproceso())
            {
                report.SetDataSource(rptDetalleReprocesoDataSet[0]);

                // Subreporte
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptDetalleReprocesoDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}