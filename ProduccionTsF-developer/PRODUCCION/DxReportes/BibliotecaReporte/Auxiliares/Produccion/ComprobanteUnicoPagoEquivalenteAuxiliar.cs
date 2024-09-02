using BibliotecaReporte.Dataset;
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
    internal class ComprobanteUnicoPagoEquivalenteAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ComprobanteUnicoPagoEquivalente";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptComprobanteUnicoPagoEquivalente = new ComprobanteUnicoPagoEquivalenteDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();


            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ComprobanteUnicoPagoEquivalenteModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptComprobanteUnicoPagoEquivalente.CabeceraComprobanteUnicoPagoEquivalente.NewCabeceraComprobanteUnicoPagoEquivalenteRow();
                    cabeceraRow.Compania = detailResult.Compania;
                    cabeceraRow.Nliquidacion = detailResult.Nliquidacion;
                    cabeceraRow.Titulo = detailResult.Titulo;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.FechaRecepcion = detailResult.FechaRecepcion;
                    cabeceraRow.IdentificacionProveedor = detailResult.IdentificacionProveedor;
                    cabeceraRow.Piscina = detailResult.Piscina;
                    cabeceraRow.Aguaje = detailResult.Aguaje;
                    cabeceraRow.Sector = detailResult.Sector;
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.INP = detailResult.INP;
                    cabeceraRow.NoASC = detailResult.NoASC;
                    cabeceraRow.SiASC = detailResult.SiASC;
                    cabeceraRow.LibrasRecibidas = detailResult.LibrasRecibidas;
                    cabeceraRow.LibrasRemitidas = detailResult.LibrasRemitidas;
                    cabeceraRow.Sobrante = detailResult.Sobrante;
                    cabeceraRow.BasuraEntero = detailResult.BasuraEntero;
                    cabeceraRow.BasuraDescabezado = detailResult.BasuraDescabezado;
                    cabeceraRow.LbsProcesadas = detailResult.LbsProcesadas;
                    cabeceraRow.RendimientoEntero = detailResult.RendimientoEntero;
                    cabeceraRow.RendimientoCOla = detailResult.RendimientoCOla;
                    cabeceraRow.AGR1EnteroCola = detailResult.AGR1EnteroCola;
                    cabeceraRow.TipoProducto = detailResult.TipoProducto;
                    cabeceraRow.AGR2Clase = detailResult.AGR2Clase;
                    cabeceraRow.Categoria = detailResult.Categoria;
                    cabeceraRow.AGR3Producto = detailResult.AGR3Producto;
                    cabeceraRow.Distribuido = detailResult.Distribuido;
                    cabeceraRow.Normal = detailResult.Normal;
                    cabeceraRow.CodigoProducto = detailResult.CodigoProducto;
                    cabeceraRow.Producto = detailResult.Producto;
                    cabeceraRow.AGR4Talla = detailResult.AGR4Talla;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.CantidadLibrasEntero = detailResult.CantidadLibrasEntero;
                    cabeceraRow.CantidadLibrasEntero2 = detailResult.CantidadLibrasEntero2;
                    cabeceraRow.Distributedd = detailResult.Distributedd;
                    cabeceraRow.TotalCabCol = detailResult.TotalCabCol;
                    cabeceraRow.Totaltopay = detailResult.Totaltopay;
                    cabeceraRow.PrecioLibras = detailResult.PrecioLibras;
                    cabeceraRow.PrecioKilos = detailResult.PrecioKilos;

                    rptComprobanteUnicoPagoEquivalente.CabeceraComprobanteUnicoPagoEquivalente.AddCabeceraComprobanteUnicoPagoEquivalenteRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
                                              
            }

            var dataSet = new DataSet[]
            {
                rptComprobanteUnicoPagoEquivalente,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptComprobanteUnicoPagoEquivalenteDataSet)
        {
            using (var report = new RptComprobanteUnicoPagoEquivalente())
            {
                report.SetDataSource(rptComprobanteUnicoPagoEquivalenteDataSet[0]);
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptComprobanteUnicoPagoEquivalenteDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);

                report.Close();

                return streamReport;
            }
        }
    }
}