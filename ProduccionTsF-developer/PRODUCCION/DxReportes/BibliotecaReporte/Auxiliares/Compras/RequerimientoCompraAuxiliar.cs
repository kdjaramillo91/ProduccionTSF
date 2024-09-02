using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.Compras;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Compras;
using BibliotecaReporte.Reportes.Compras;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Compras
{
    internal class RequerimientoCompraAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_RequerimientoCompra";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptRequerimientoCompra = new RequerimientoCompraDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<RequerimientoCompraModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptRequerimientoCompra.CabeceraRequerimientoCompra.NewCabeceraRequerimientoCompraRow();
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.RucCia = detailResult.RucCia;
                    cabeceraRow.TelefonoCia = detailResult.TelefonoCia;
                    cabeceraRow.DireccionCia = detailResult.DireccionCia;
                    cabeceraRow.NumeroDocumento = detailResult.NumeroDocumento;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.Solicitante = detailResult.Solicitante;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.CodigoProducto = detailResult.CodigoProducto;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.GramajeDesde = detailResult.GramajeDesde;
                    cabeceraRow.GramajeHasta = detailResult.GramajeHasta;
                    cabeceraRow.ColorReferencia = detailResult.ColorReferencia;
                    cabeceraRow.CantidadRequerida = detailResult.CantidadRequerida;
                    cabeceraRow.CantidadAprobada = detailResult.CantidadAprobada;
                    cabeceraRow.CantidadPendiente = detailResult.CantidadPendiente;

                    rptRequerimientoCompra.CabeceraRequerimientoCompra.AddCabeceraRequerimientoCompraRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptRequerimientoCompra,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptDataSet)
        {
            using (var report = new RptRequerimientoCompra())
            {
                report.SetDataSource(rptDataSet[0]);

                // Subreporte Cia's
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}