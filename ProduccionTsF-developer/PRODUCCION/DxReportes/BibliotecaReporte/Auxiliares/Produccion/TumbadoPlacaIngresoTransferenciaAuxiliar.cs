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
    internal class TumbadoPlacaIngresoTransferenciaAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_TmbadoPlacaIngresoTransferencia";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static TumbadoPlacaIngresoTransferenciaDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptTmbadoPlacaIngresoTransferencia = new TumbadoPlacaIngresoTransferenciaDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<TumbadoPlacaIngresoTransferenciaModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptTmbadoPlacaIngresoTransferencia.CabeceraTumbadoPlacaIngresoTransferencia.NewCabeceraTumbadoPlacaIngresoTransferenciaRow();
                    cabeceraRow.TituloReporte = detailResult.TituloReporte;
                    cabeceraRow.Bodega = detailResult.Bodega;
                    cabeceraRow.Motivo = detailResult.Motivo;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.CodigoProducto = detailResult.CodigoProducto;
                    cabeceraRow.DescripcionProducto = detailResult.DescripcionProducto;
                    cabeceraRow.CodigoUnidadMedida = detailResult.CodigoUnidadMedida;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.NumeroSecuencia = detailResult.NumeroSecuencia;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.CentroCosto = detailResult.CentroCosto;
                    cabeceraRow.SubCentroCosto = detailResult.SubCentroCosto;
                    cabeceraRow.SecuenciaGuiaRemision = detailResult.SecuenciaGuiaRemision;
                    cabeceraRow.SecuenciaLiquidacionMateriales = detailResult.SecuenciaLiquidacionMateriales;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.Numlote = detailResult.Numlote;
                    cabeceraRow.SecTransac = detailResult.SecTransac;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.ItemTalla = detailResult.ItemTalla;
                    cabeceraRow.TipoItem = detailResult.TipoItem;
                    cabeceraRow.NombreUsuario = detailResult.NombreUsuario;
                    cabeceraRow.BodegaEgreso = detailResult.BodegaEgreso;
                    cabeceraRow.NumeroEgreso = detailResult.NumeroEgreso;
                    
                    rptTmbadoPlacaIngresoTransferencia.CabeceraTumbadoPlacaIngresoTransferencia.AddCabeceraTumbadoPlacaIngresoTransferenciaRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptTmbadoPlacaIngresoTransferencia;
        }
        private static Stream SetDataReport(TumbadoPlacaIngresoTransferenciaDataSet rptTumbadoPlacaIngresoTransferenciaDataSet)
        {
            using (var report = new RptTmbadoPlacaIngresoTransferencia())
            {
                report.SetDataSource(rptTumbadoPlacaIngresoTransferenciaDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
