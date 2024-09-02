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
    internal class LiquidacionCamaronAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LiquidacionCamaron";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static LiquidacionCamaronDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFacturaComercial = new LiquidacionCamaronDataSet();
            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }
                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LiquidacionCamaronModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFacturaComercial.CabeceraLiquidacionCamaron.NewCabeceraLiquidacionCamaronRow();
                    cabeceraRow.N = detailResult.N;
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.FechaRecepcion = detailResult.FechaRecepcion;
                    cabeceraRow.InpNumeroProveedor = detailResult.InpNumeroProveedor;
                    cabeceraRow.MinisterialAgreementPLProveedor = detailResult.MinisterialAgreementPLProveedor;
                    cabeceraRow.TramitNumberPLProveedor = detailResult.TramitNumberPLProveedor;
                    cabeceraRow.Piscina = detailResult.Piscina;
                    cabeceraRow.Nombreproveedor = detailResult.Nombreproveedor;
                    cabeceraRow.GuiaRemision = detailResult.GuiaRemision;
                    cabeceraRow.ItemSizeName = detailResult.ItemSizeName;
                    cabeceraRow.CabezaBasura = detailResult.CabezaBasura;
                    cabeceraRow.ColaBasura = detailResult.ColaBasura;
                    cabeceraRow.Avancerounded = detailResult.Avancerounded;
                    cabeceraRow.Nameitem = detailResult.Nameitem;
                    cabeceraRow.RucProveedor = detailResult.RucProveedor;
                    cabeceraRow.Lista = detailResult.Lista;
                    cabeceraRow.NombreTipoItem = detailResult.NombreTipoItem;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Comprador = detailResult.Comprador;
                    cabeceraRow.TipoProceso = detailResult.TipoProceso;
                    cabeceraRow.Librasremitidas = detailResult.Librasremitidas;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.Cajas = detailResult.Cajas;
                    cabeceraRow.BasuraCola = detailResult.BasuraCola;
                    cabeceraRow.Resta = detailResult.Resta;
                    cabeceraRow.Sobrante = detailResult.Sobrante;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.Secuencial = detailResult.Secuencial;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.PorcCola = detailResult.PorcCola;
                    cabeceraRow.PorcColaProductoEntero = detailResult.PorcColaProductoEntero;
                    cabeceraRow.Pl_wholeSubtotal = detailResult.Pl_wholeSubtotal;
                    cabeceraRow.Clase = detailResult.Clase;
                    cabeceraRow.TotalPagarLote = detailResult.TotalPagarLote;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    rptFacturaComercial.CabeceraLiquidacionCamaron.AddCabeceraLiquidacionCamaronRow(cabeceraRow);
                }
                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptFacturaComercial;
        }
        private static Stream SetDataReport(LiquidacionCamaronDataSet rptLiquidacionCamaronDataSet)
        {
            using (var report = new RptLiquidacionCamaron())
            {
                report.SetDataSource(rptLiquidacionCamaronDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
