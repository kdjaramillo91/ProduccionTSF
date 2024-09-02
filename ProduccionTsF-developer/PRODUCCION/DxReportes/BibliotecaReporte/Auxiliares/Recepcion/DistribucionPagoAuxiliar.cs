using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.Recepcion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Recepcion;
using BibliotecaReporte.Reportes.Recepcion;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Recepcion
{
    internal class DistribucionPagoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_DistribucionPago";
        private const string m_StoredProcedureNameSinDistribucion = "spPar_DistribucionPagoSinDistribucion";
        private const string m_StoredProcedureNameConDistribucion = "spPar_DistribucionPagoConDistribucion";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            // Declaro los DataSet's
            var rptDistribucionPago = new DistribucionPagoDataSet();
            var rptDistribucionPagoSinDistribucion = new DistribucionPagoResumenDataSet();
            var rptDistribucionPagoConDistribucion = new DistribucionPagoResumenDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            // Conexión a base y ejecución de SP
            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<DistribucionPagoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptDistribucionPago.CabeceraDistribucionPago.NewCabeceraDistribucionPagoRow();
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.SecTransaccional = detailResult.SecTransaccional;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.Piscina = detailResult.Piscina;
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.RendimientoEnteroSinDistribuir = detailResult.RendimientoEnteroSinDistribuir;
                    cabeceraRow.RendimientoColaSinDistribuir = detailResult.RendimientoColaSinDistribuir;
                    cabeceraRow.RendimientoTotalSinDistribuir = detailResult.RendimientoTotalSinDistribuir;
                    cabeceraRow.TotalEnteroSinDistribuir = detailResult.TotalEnteroSinDistribuir;
                    cabeceraRow.TotalColaSinDistribuir = detailResult.TotalColaSinDistribuir;
                    cabeceraRow.TotalSinDistribuir = detailResult.TotalSinDistribuir;
                    cabeceraRow.RendimientoEnteroConDistribucion = detailResult.RendimientoEnteroConDistribucion;
                    cabeceraRow.RendimientoColaConDistribucion = detailResult.RendimientoColaConDistribucion;
                    cabeceraRow.RendimientoTotalConDistribucion = detailResult.RendimientoTotalConDistribucion;
                    cabeceraRow.TotalEnteroConDistribucion = detailResult.TotalEnteroConDistribucion;
                    cabeceraRow.TotalColaConDistribucion = detailResult.TotalColaConDistribucion;
                    cabeceraRow.TotalConDistribucion = detailResult.TotalConDistribucion;
                    cabeceraRow.Nombre_Cia = detailResult.Nombre_Cia;
                    cabeceraRow.Ruc_Cia = detailResult.Ruc_Cia;
                    cabeceraRow.Telephone_Cia = detailResult.Telephone_Cia;
                    cabeceraRow.SecuenciaLiquidacion = detailResult.SecuenciaLiquidacion;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.Proceso = detailResult.Proceso;

                    rptDistribucionPago.CabeceraDistribucionPago.AddCabeceraDistribucionPagoRow(cabeceraRow);
                }

                // Detalles sin distribución
                var subDetailSinDistribucion = db.Query<DistribucionPagoResumenModel>(m_StoredProcedureNameSinDistribucion, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var subdetail in subDetailSinDistribucion)
                {
                    var detailRow = rptDistribucionPagoSinDistribucion.CabeceraDistribucionPagoResumen.NewCabeceraDistribucionPagoResumenRow();
                    detailRow.id = subdetail.Id;
                    detailRow.NombreProducto = subdetail.NombreProducto;
                    detailRow.Clase = subdetail.Clase;
                    detailRow.Talla = subdetail.Talla;
                    detailRow.ProcesoDetalle = subdetail.ProcesoDetalle;
                    detailRow.RendimientoTotal = subdetail.RendimientoTotal;
                    detailRow.UM = subdetail.UM;
                    detailRow.PrecioUnitario = subdetail.PrecioUnitario;
                    detailRow.ValorTotal = subdetail.ValorTotal;

                    rptDistribucionPagoSinDistribucion.CabeceraDistribucionPagoResumen.AddCabeceraDistribucionPagoResumenRow(detailRow);
                }

                // Detalles Con distribución
                var subDetailConDistribucion = db.Query<DistribucionPagoResumenModel>(m_StoredProcedureNameConDistribucion, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var subdetail in subDetailConDistribucion)
                {
                    var detailRow = rptDistribucionPagoConDistribucion.CabeceraDistribucionPagoResumen.NewCabeceraDistribucionPagoResumenRow();
                    detailRow.id = subdetail.Id;
                    detailRow.NombreProducto = subdetail.NombreProducto;
                    detailRow.Clase = subdetail.Clase;
                    detailRow.Talla = subdetail.Talla;
                    detailRow.ProcesoDetalle = subdetail.ProcesoDetalle;
                    detailRow.RendimientoTotal = subdetail.RendimientoTotal;
                    detailRow.UM = subdetail.UM;
                    detailRow.PrecioUnitario = subdetail.PrecioUnitario;
                    detailRow.ValorTotal = subdetail.ValorTotal;

                    rptDistribucionPagoConDistribucion.CabeceraDistribucionPagoResumen.AddCabeceraDistribucionPagoResumenRow(detailRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptDistribucionPago,
                rptDistribucionPagoSinDistribucion,
                rptDistribucionPagoConDistribucion,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptDataSet)
        {
            using (var report = new RptDistribucionPago())
            {
                report.SetDataSource(rptDataSet[0]);

                // SubReporte Detalles Distribución
                report.OpenSubreport("RptDetalleSinDistribucion").SetDataSource(rptDataSet[1]);
                report.OpenSubreport("RptDetalleConDistribucion").SetDataSource(rptDataSet[2]);

                // SubReporte Cia
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptDataSet[3]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);

                report.Close();

                return streamReport;
            }
        }
    }
}
