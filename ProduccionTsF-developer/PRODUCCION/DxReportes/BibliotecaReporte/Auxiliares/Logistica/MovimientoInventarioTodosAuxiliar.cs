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
    internal class MovimientoInventarioTodosAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_MovimientoInventarioTodos";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMovimientoInventarioTodos = new MovimientoInventarioTodosDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MovimientoInventarioTodosModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMovimientoInventarioTodos.CabeceraMovimientoInventarioTodos.NewCabeceraMovimientoInventarioTodosRow();
                    cabeceraRow.Id = detailResult.Id;
                    cabeceraRow.TituloReporte = detailResult.TituloReporte;
                    cabeceraRow.Bodega = detailResult.Bodega;
                    cabeceraRow.Motivo = detailResult.Motivo;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.CodigoProducto = detailResult.CodigoProducto;
                    cabeceraRow.DescripcionProducto = detailResult.DescripcionProducto;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.NumeroSecuencia = detailResult.NumeroSecuencia;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.CodigoNaturaleza = detailResult.CodigoNaturaleza;
                    cabeceraRow.SecuenciaRequisicion = detailResult.SecuenciaRequisicion;
                    cabeceraRow.SecuenciaLiquidacionMateriales = detailResult.SecuenciaLiquidacionMateriales;
                    cabeceraRow.Libras = detailResult.Libras;
                    cabeceraRow.Kilos = detailResult.Kilos;

                    rptMovimientoInventarioTodos.CabeceraMovimientoInventarioTodos.AddCabeceraMovimientoInventarioTodosRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptMovimientoInventarioTodos,
                rptCompaniaInfo,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] rptMovimientoInventarioDataSet)
        {
            using (var report = new RptMovimientoInventarioTodos())
            {
                report.SetDataSource(rptMovimientoInventarioDataSet[0]);

                // Subreporte
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptMovimientoInventarioDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
