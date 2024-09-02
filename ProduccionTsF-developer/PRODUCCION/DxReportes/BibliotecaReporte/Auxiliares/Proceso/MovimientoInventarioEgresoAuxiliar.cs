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
    internal class MovimientoInventarioEgresoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_Movimiento_InventarioEgreso";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static MovimientoInventarioEgresoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMovimientoInventarioEgreso = new MovimientoInventarioEgresoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MovimientoInventarioEgresoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMovimientoInventarioEgreso.CabeceraMovimientoInventarioEgreso.NewCabeceraMovimientoInventarioEgresoRow();
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
                    cabeceraRow.SecuenciaGuiaRemision = detailResult.SecuenciaGuiaRemision;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.SecTransac = detailResult.SecTransac;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.TipoItem = detailResult.TipoItem;
                    cabeceraRow.ItemTalla = detailResult.ItemTalla;
                    cabeceraRow.LoteOrigenIngreso = detailResult.LoteOrigenIngreso;
                    cabeceraRow.Kg = detailResult.Kg;
                    cabeceraRow.Lbskg2 = detailResult.Lbskg2;
                    cabeceraRow.Loteegreso = detailResult.Loteegreso;
                    rptMovimientoInventarioEgreso.CabeceraMovimientoInventarioEgreso.AddCabeceraMovimientoInventarioEgresoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptMovimientoInventarioEgreso;
        }
        private static Stream SetDataReport(MovimientoInventarioEgresoDataSet rptMovimientoInventarioEgresoDataSet)
        {
            using (var report = new RptMovimientoInventarioEgreso())
            {
                report.SetDataSource(rptMovimientoInventarioEgresoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
