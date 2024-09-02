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
    internal class ResumenLiquidacionProveedorAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ResumenLiquidacionProveedor";
        private const string m_StoredProcedureNameSub = "spPar_ResumenLiquidacionesProveedorResumen";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptResumenLiquidacionProveedor = new ResumenLiquidacionProveedorDataSet();
            var rptResumenLiquidacionProveedorResumen = new ResumenLiquidacionProveedorResumenDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ResumenLiquidacionProveedorModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptResumenLiquidacionProveedor.cabeceraResumenLiquidacionProveedor.NewcabeceraResumenLiquidacionProveedorRow();
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.LibrasDespachadas = detailResult.LibrasDespachadas;
                    cabeceraRow.KilosCabeza = detailResult.KilosCabeza;
                    cabeceraRow.LibrasCola = detailResult.LibrasCola;
                    cabeceraRow.RendmimientoCabeza = detailResult.RendmimientoCabeza;
                    cabeceraRow.RendimientoCola = detailResult.RendimientoCola;
                    cabeceraRow.ValorCabeza = detailResult.ValorCabeza;
                    cabeceraRow.ValorCola = detailResult.ValorCola;
                    cabeceraRow.DolaresTotal = detailResult.DolaresTotal;
                    cabeceraRow.PrecioCabeza = detailResult.PrecioCabeza;
                    cabeceraRow.PrecioCola = detailResult.PrecioCola;
                    cabeceraRow.PrecioTotal = detailResult.PrecioTotal;

                    rptResumenLiquidacionProveedor.cabeceraResumenLiquidacionProveedor.AddcabeceraResumenLiquidacionProveedorRow(cabeceraRow);
                }

                var subDetail = db.Query<ResumenLiquidacionProveedorResumenModel>(m_StoredProcedureNameSub, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in subDetail)
                {
                    var cabeceraRow = rptResumenLiquidacionProveedorResumen.ResumenLiquidacionProveedorResumen.NewResumenLiquidacionProveedorResumenRow();
                    cabeceraRow.NumLiquidaciones = detailResult.NumLiquidaciones;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.LibrasDespachadas = detailResult.LibrasDespachadas;
                    cabeceraRow.LibrasRemitidas = detailResult.LibrasRemitidas;
                    cabeceraRow.LibrasProcesadas = detailResult.LibrasProcesadas;
                    cabeceraRow.KilosCabeza = detailResult.KilosCabeza;
                    cabeceraRow.LibrasCola = detailResult.LibrasCola;
                    cabeceraRow.RendimientoCabeza = detailResult.RendimientoCabeza;
                    cabeceraRow.RendimientoCola = detailResult.RendimientoCola;
                    cabeceraRow.ValorCabeza = detailResult.ValorCabeza;
                    cabeceraRow.ValorCola = detailResult.ValorCola;
                    cabeceraRow.DolaresTotal = detailResult.DolaresTotal;
                    cabeceraRow.PrecioCabeza = detailResult.PrecioCabeza;
                    cabeceraRow.PrecioCola = detailResult.PrecioCola;
                    cabeceraRow.PrecioTotal = detailResult.PrecioTotal;
                    rptResumenLiquidacionProveedorResumen.ResumenLiquidacionProveedorResumen.AddResumenLiquidacionProveedorResumenRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptResumenLiquidacionProveedor,
                rptResumenLiquidacionProveedorResumen,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptResumenLiquidacionProveedor())
            {
                report.SetDataSource(dataSet[0]);
                //SubReporte
                report.OpenSubreport("RptResumenLiquidacionProveedorResumen.rpt").SetDataSource(dataSet[1]);
                report.OpenSubreport("RptLogo.rpt").SetDataSource(dataSet[2]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}