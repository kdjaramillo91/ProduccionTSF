using BibliotecaReporte.Dataset.Inventario;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Inventario;
using BibliotecaReporte.Reportes.Inventario;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.Inventario
{
    internal class MovimientoInventarioEgresoTransferenciaAAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_MovimientoInventarioEgresoTransferenciaA";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static MovimientoInventarioEgresoTransferenciaADataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptIngreso = new MovimientoInventarioEgresoTransferenciaADataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MovimientoInventarioEgresoTransferenciaAModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptIngreso.CabeceraMovimientoInventarioEgresoTransferenciaA.NewCabeceraMovimientoInventarioEgresoTransferenciaARow();
                    cabeceraRow.Compania = detailResult.Compania;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.NDocumento = detailResult.NDocumento;
                    cabeceraRow.BodegaEnvio = detailResult.BodegaEnvio;
                    cabeceraRow.UbicacionBenvio = detailResult.UbicacionBenvio;
                    cabeceraRow.Motivoenvio = detailResult.Motivoenvio;
                    cabeceraRow.Despachador = detailResult.Despachador;
                    cabeceraRow.Codigo = detailResult.Codigo;
                    cabeceraRow.Producto = detailResult.Producto;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.CntLibras = detailResult.CntLibras;
                    cabeceraRow.CntKilos = detailResult.CntKilos;
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.SecuenciaTransaccional = detailResult.SecuenciaTransaccional;
                    cabeceraRow.Tipo = detailResult.Tipo;
                    cabeceraRow.Talla = detailResult.Talla;
                    rptIngreso.CabeceraMovimientoInventarioEgresoTransferenciaA.AddCabeceraMovimientoInventarioEgresoTransferenciaARow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptIngreso;
        }

        private static Stream SetDataReport(MovimientoInventarioEgresoTransferenciaADataSet rptTransferegresoDataSet)
        {
            using (var report = new RptMovimientoInventarioEgresoTransferenciaA())
            {
                report.SetDataSource(rptTransferegresoDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }



    }
}
