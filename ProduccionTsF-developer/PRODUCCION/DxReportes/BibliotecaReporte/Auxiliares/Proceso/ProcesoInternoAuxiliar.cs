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
    internal class ProcesoInternoAuxiliar
    {

        private const string m_StoredProcedureName = "spPar_ProcesosInternos";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static ProcesoInternoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptPorcesoInterno = new ProcesoInternoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ProcesoInternoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptPorcesoInterno.CabeceraProcesoInterno.NewCabeceraProcesoInternoRow();
                    cabeceraRow.Mproliq = detailResult.Mproliq;
                    cabeceraRow.ProdunNameNombreUnidadLoteProduccion = detailResult.ProdunNameNombreUnidadLoteProduccion;
                    cabeceraRow.ProdprnameProcesoLoteProduccion = detailResult.ProdprnameProcesoLoteProduccion;
                    cabeceraRow.PRODLreceptionDateFechaRecepcion = detailResult.PRODLreceptionDateFechaRecepcion;
                    cabeceraRow.PRODLnumberNumerodeLote = detailResult.PRODLnumberNumerodeLote;
                    cabeceraRow.PRODLtotalQuantityRecivedTotalCantidadRecibidaLibras = detailResult.PRODLtotalQuantityRecivedTotalCantidadRecibidaLibras;
                    cabeceraRow.PRODLinternalNumberNumeroInterno = detailResult.PRODLinternalNumberNumeroInterno;
                    cabeceraRow.PRODLtotalQuantityRemittedTCLREM = detailResult.PRODLtotalQuantityRemittedTCLREM;
                    cabeceraRow.WAREnameBodega = detailResult.WAREnameBodega;
                    cabeceraRow.Rendimiento = detailResult.Rendimiento;
                    cabeceraRow.LOTORInumberLoteOrigen = detailResult.LOTORInumberLoteOrigen;
                    cabeceraRow.ITEMmasterCodeCodigoProducto = detailResult.ITEMmasterCodeCodigoProducto;
                    cabeceraRow.ItemsznameTalla = detailResult.ItemsznameTalla;
                    cabeceraRow.ProdldquantityRecivedCantidadRecibidaDetalle = detailResult.ProdldquantityRecivedCantidadRecibidaDetalle;
                    cabeceraRow.MetricCodeUnidadMedida = detailResult.MetricCodeUnidadMedida;
                    cabeceraRow.TmpquantityPoundsLiquidationliqlibrastotalrecobidas = detailResult.TmpquantityPoundsLiquidationliqlibrastotalrecobidas;
                    cabeceraRow.PRODLDescriptionObservaciones = detailResult.PRODLDescriptionObservaciones;
                    cabeceraRow.ITEMnameNombreItem = detailResult.ITEMnameNombreItem;
                    rptPorcesoInterno.CabeceraProcesoInterno.AddCabeceraProcesoInternoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptPorcesoInterno;
        }
        private static Stream SetDataReport(ProcesoInternoDataSet rptProcesoInternoDataSet)
        {
            using (var report = new RptProcesoInterno())
            {
                report.SetDataSource(rptProcesoInternoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
