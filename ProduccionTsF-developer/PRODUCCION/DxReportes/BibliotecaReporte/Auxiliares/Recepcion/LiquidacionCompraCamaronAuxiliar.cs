using BibliotecaReporte.Dataset.Recepcion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Recepcion;
using BibliotecaReporte.Reportes.Recepcion;
using CrystalDecisions.Shared;
using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.Recepcion
{
    internal class LiquidacionCompraCamaronAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LiquidacionCompraCamaron";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static LiquidacionCompraCamaronDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptLiquidacionCompraCamaron = new LiquidacionCompraCamaronDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LiquidacionCompraCamaronModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptLiquidacionCompraCamaron.CabeceraLiquidacionCompraCamaron.NewCabeceraLiquidacionCompraCamaronRow();
                    cabeceraRow.Precio = detailResult.Precio;
                    cabeceraRow.N = detailResult.N;
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.FechaRecepcion = detailResult.FechaRecepcion;
                    cabeceraRow.InpNumeroProveedor = detailResult.InpNumeroProveedor;
                    cabeceraRow.MinisterialAgreementPLProveedor = detailResult.MinisterialAgreementPLProveedor;
                    cabeceraRow.TramitNumberPLProveedor = detailResult.TramitNumberPLProveedor;
                    cabeceraRow.Piscina = detailResult.Piscina;
                    cabeceraRow.GuiaRemision = detailResult.GuiaRemision;
                    cabeceraRow.ItemSizeName = detailResult.ItemSizeName;
                    cabeceraRow.PLTOTALTOPAY = detailResult.PLTOTALTOPAY;
                    cabeceraRow.CABEZABASURA = detailResult.CABEZABASURA;
                    cabeceraRow.COLABASURA = detailResult.COLABASURA;
                    cabeceraRow.AVANCEROUNDED = detailResult.AVANCEROUNDED;
                    cabeceraRow.NameItem = detailResult.NameItem;
                    cabeceraRow.Kilolibrasform = detailResult.Kilolibrasform;
                    cabeceraRow.RucProveedor = detailResult.RucProveedor;
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.Lista = detailResult.Lista;
                    cabeceraRow.NombreTipoItem = detailResult.NombreTipoItem;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Comprador = detailResult.Comprador;
                    cabeceraRow.TipoProceso = detailResult.TipoProceso;
                    cabeceraRow.LibrasRemitidas = detailResult.LibrasRemitidas;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.Cajas = detailResult.Cajas;
                    cabeceraRow.BasuraCola = detailResult.BasuraCola;
                    cabeceraRow.Resta = detailResult.Resta;
                    cabeceraRow.Sobrante = detailResult.Sobrante;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.Secuencial = detailResult.Secuencial;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.PorcCola = detailResult.PorcCola;
                    cabeceraRow.PorcColaProductoEntero = detailResult.PorcColaProductoEntero;
                    cabeceraRow.PL_wholeSubtotal = detailResult.PL_wholeSubtotal;
                    cabeceraRow.TotalPagarLote = detailResult.TotalPagarLote;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.Clase = detailResult.Clase;
                    rptLiquidacionCompraCamaron.CabeceraLiquidacionCompraCamaron.AddCabeceraLiquidacionCompraCamaronRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptLiquidacionCompraCamaron;
        }
        private static Stream SetDataReport(LiquidacionCompraCamaronDataSet rptLibrasLiquidadasCamaronDataSet)
        {
            using (var report = new RptLiquidacionCompraCamaron())
            {
                report.SetDataSource(rptLibrasLiquidadasCamaronDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
