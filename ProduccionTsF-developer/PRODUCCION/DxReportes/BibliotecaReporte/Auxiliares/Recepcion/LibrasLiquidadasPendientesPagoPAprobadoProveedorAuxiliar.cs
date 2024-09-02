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
   internal  class LibrasLiquidadasPendientesPagoPAprobadoProveedorAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LibrasLiquidadasPendientesPagoPAprobadoProveedor";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static LibrasLiquidadasPendientesPagoPAprobadoProveedorDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptLibrasLiquidasPendientesPagoPAprobadoProveedorDataset = new LibrasLiquidadasPendientesPagoPAprobadoProveedorDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LibrasLiquidadasPendientesPagoPAprobadoProveedorModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptLibrasLiquidasPendientesPagoPAprobadoProveedorDataset.CabeceraLibrasLiquidadasPendientesPagoPAprobadoProveedor.NewCabeceraLibrasLiquidadasPendientesPagoPAprobadoProveedorRow();
                    cabeceraRow.PL_InternalNumber = detailResult.PL_InternalNumber;
                    cabeceraRow.PL_ReceptionDate = detailResult.PL_ReceptionDate;
                    cabeceraRow.PL_LiquidationDate = detailResult.PL_LiquidationDate;
                    cabeceraRow.LbsProcesadas = detailResult.LbsProcesadas;
                    cabeceraRow.LbsRemitidas = detailResult.LbsRemitidas;
                    cabeceraRow.PL_TotalToPay = detailResult.PL_TotalToPay;
                    cabeceraRow.NameProveedor = detailResult.NameProveedor;
                    cabeceraRow.ProductionUnitProviderName = detailResult.ProductionUnitProviderName;
                    cabeceraRow.LbsRecibidas = detailResult.LbsRecibidas;
                    cabeceraRow.Ruc_cia = detailResult.Ruc_cia;
                    cabeceraRow.Telephone_cia = detailResult.Telephone_cia;
                    cabeceraRow.Foto2 = detailResult.Foto2;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.PriceList = detailResult.PriceList;
                    cabeceraRow.SequentialLiquidation = detailResult.SequentialLiquidation;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.Anticipo = detailResult.Anticipo;
                    cabeceraRow.Gramage = detailResult.Gramage;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.ProcesoPlanta = detailResult.ProcesoPlanta;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    rptLibrasLiquidasPendientesPagoPAprobadoProveedorDataset.CabeceraLibrasLiquidadasPendientesPagoPAprobadoProveedor.AddCabeceraLibrasLiquidadasPendientesPagoPAprobadoProveedorRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptLibrasLiquidasPendientesPagoPAprobadoProveedorDataset;
        }
        private static Stream SetDataReport(LibrasLiquidadasPendientesPagoPAprobadoProveedorDataSet rptLibrasLiquidadasPendientesPagoPAprobadoProveedorDataSet)
        {
            using (var report = new RptLibrasLiquidadasPendientesPagoPAprobadoProveedor())
            {
                report.SetDataSource(rptLibrasLiquidadasPendientesPagoPAprobadoProveedorDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
