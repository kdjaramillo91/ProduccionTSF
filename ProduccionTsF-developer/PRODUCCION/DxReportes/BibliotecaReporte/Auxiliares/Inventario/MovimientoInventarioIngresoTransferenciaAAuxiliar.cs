using BibliotecaReporte.Dataset.Inventario;
using BibliotecaReporte.Model;
using BibliotecaReporte.Reportes.Inventario;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.Inventario
{
    internal class MovimientoInventarioIngresoTransferenciaAAuxiliar
    {

        private const string m_StoredProcedureName = "spPar_MovimientoInventarioIngresoTransferenciaA";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static MovimientoInventarioIngresoTransferenciaADataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptIngreso = new MovimientoInventarioIngresoTransferenciaADataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MovimientoInventarioIngresoTransferenciaAModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptIngreso.CabeceraMovimientoInventarioIngresoTransferenciaA.NewCabeceraMovimientoInventarioIngresoTransferenciaARow();
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
                    cabeceraRow.MotivoIngreso = detailResult.MotivoIngreso;
                    rptIngreso.CabeceraMovimientoInventarioIngresoTransferenciaA.AddCabeceraMovimientoInventarioIngresoTransferenciaARow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptIngreso;
        }


        private static Stream SetDataReport(MovimientoInventarioIngresoTransferenciaADataSet rptTransferegresoDataSet)
        {
            using (var report = new RptMovimientoInventarioIngresoTransferenciaA())
            {
                report.SetDataSource(rptTransferegresoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }


    }
}
