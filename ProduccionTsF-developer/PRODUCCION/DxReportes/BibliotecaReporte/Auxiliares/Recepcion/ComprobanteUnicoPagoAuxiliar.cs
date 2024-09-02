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
    internal class ComprobanteUnicoPagoAuxiliar
    {

        private const string m_StoredProcedureName = "spPar_ComprobanteUnicoPago";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static ComprobanteUnicoPagoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFacturaComercial = new ComprobanteUnicoPagoDataSet();
            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }
                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ComprobanteUnicoPagoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFacturaComercial.CabeceraComprobanteUnicoPago.NewCabeceraComprobanteUnicoPagoRow();
                    cabeceraRow.PrecioUnitario = detailResult.PrecioUnitario;
                    cabeceraRow.ValorTotal = detailResult.ValorTotal;
                    cabeceraRow.Redimientototal = detailResult.Redimientototal;
                    cabeceraRow.GRecepcion = detailResult.GRecepcion;
                    cabeceraRow.Guiarecepcion = detailResult.Guiarecepcion;
                    cabeceraRow.FechaRecepcion = detailResult.FechaRecepcion;
                    cabeceraRow.Ruc_proveedor = detailResult.Ruc_proveedor;
                    cabeceraRow.Pl_totalQuantityRecived = detailResult.Pl_totalQuantityRecived;
                    cabeceraRow.Pl_totalQuantityLiquidationAdjust = detailResult.Pl_totalQuantityLiquidationAdjust;
                    cabeceraRow.Pl_wholeSubtotal = detailResult.Pl_wholeSubtotal;
                    cabeceraRow.PL_wholeSubtotalAdjust = detailResult.PL_wholeSubtotalAdjust;
                    cabeceraRow.Pl_subtotalTailAdjust = detailResult.Pl_subtotalTailAdjust;
                    cabeceraRow.Imp = detailResult.Imp;
                    cabeceraRow.AcuerdoMinisterial = detailResult.AcuerdoMinisterial;
                    cabeceraRow.NumeroTramite = detailResult.NumeroTramite;
                    cabeceraRow.NamePool = detailResult.NamePool;
                    cabeceraRow.NameProveedor = detailResult.NameProveedor;
                    cabeceraRow.GuiaTransporte = detailResult.GuiaTransporte;
                    cabeceraRow.Guia_Proveedor = detailResult.Guia_Proveedor;
                    cabeceraRow.Name_Cia = detailResult.Name_Cia;
                    cabeceraRow.Ruc_Cia = detailResult.Ruc_Cia;
                    cabeceraRow.Telephone_Cia = detailResult.Telephone_Cia;
                    cabeceraRow.Grammage = detailResult.Grammage;
                    cabeceraRow.Typename = detailResult.Typename;
                    cabeceraRow.Name_item_short = detailResult.Name_item_short;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Itemsizename = detailResult.Itemsizename;
                    cabeceraRow.WholeGarbagePounds = detailResult.WholeGarbagePounds;
                    cabeceraRow.PoundsGarbageTail = detailResult.PoundsGarbageTail;
                    cabeceraRow.Lista = detailResult.Lista;
                    cabeceraRow.BasuraCola = detailResult.BasuraCola;
                    cabeceraRow.Clase = detailResult.Clase;
                    cabeceraRow.Resta = detailResult.Resta;
                    cabeceraRow.Avancerounded = detailResult.Avancerounded;
                    cabeceraRow.Sobrante = detailResult.Sobrante;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.Secuencial = detailResult.Secuencial;
                    cabeceraRow.RendimientoCola = detailResult.RendimientoCola;
                    cabeceraRow.TotalPagarLote = detailResult.TotalPagarLote;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.FirmaAutorizadoPor = detailResult.FirmaAutorizadoPor;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    rptFacturaComercial.CabeceraComprobanteUnicoPago.AddCabeceraComprobanteUnicoPagoRow(cabeceraRow);
                }
                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptFacturaComercial;
        }
        private static Stream SetDataReport(ComprobanteUnicoPagoDataSet rptComprobanteUnicoPagoDataSet)
        {
            using (var report = new RptComprobanteUnicoPago())
            {
                report.SetDataSource(rptComprobanteUnicoPagoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
