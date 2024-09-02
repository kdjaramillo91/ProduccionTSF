using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.Recepcion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Produccion;
using BibliotecaReporte.Reportes.Recepcion;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Recepcion
{
    internal class LibrasLiquidadasPendientePagoAprobadoProveedorAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LibrasLiquidadaspendientePagoAprobadoProveedor";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptLibrasLiquidadasPendPagoAprovadoProveedor = new LibrasLiquidadasPendientePagoAprovadoProveedorDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LibraLiquidadasPendientePagoAprobadoProveedorModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptLibrasLiquidadasPendPagoAprovadoProveedor.CabeceraLibrasLiquidadasPendientePagoAprovadoProveedor.NewCabeceraLibrasLiquidadasPendientePagoAprovadoProveedorRow();
                    cabeceraRow.PL_internalNumber = detailResult.PL_internalNumber;
                    cabeceraRow.PL_receptionDate = detailResult.PL_receptionDate;
                    cabeceraRow.PL_liquidationDate = detailResult.PL_liquidationDate;
                    cabeceraRow.Lbsprocesadas = detailResult.Lbsprocesadas;
                    cabeceraRow.Lbsremitidas = detailResult.Lbsremitidas;
                    cabeceraRow.PL_totalToPay = detailResult.PL_totalToPay;
                    cabeceraRow.Nameproveedor = detailResult.Nameproveedor;
                    cabeceraRow.ProductionUnitProviderName = detailResult.ProductionUnitProviderName;
                    cabeceraRow.Lbsrecibidas = detailResult.Lbsrecibidas;
                    cabeceraRow.Ruc_cia = detailResult.Ruc_cia;
                    cabeceraRow.Telephone_cia = detailResult.Telephone_cia;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.Pricelist = detailResult.Pricelist;
                    cabeceraRow.SequentialLiquidation = detailResult.SequentialLiquidation;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.Anticipo = detailResult.Anticipo;
                    cabeceraRow.Gramage = detailResult.Gramage;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.ProcesoPlanta = detailResult.ProcesoPlanta;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                  

                    rptLibrasLiquidadasPendPagoAprovadoProveedor.CabeceraLibrasLiquidadasPendientePagoAprovadoProveedor.AddCabeceraLibrasLiquidadasPendientePagoAprovadoProveedorRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptLibrasLiquidadasPendPagoAprovadoProveedor,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptLbsPendPagoAprobadoDataSet)
        {
            using (var report = new RptLibrasLiquidadasPendientePagoAprobadoProveedor())
            {
                report.SetDataSource(rptLbsPendPagoAprobadoDataSet[0]);

                // Subreporte Cia's
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptLbsPendPagoAprobadoDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}