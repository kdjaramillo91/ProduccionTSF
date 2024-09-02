using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.OrdenesDeProduccion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.OrdenesDeProduccion;
using BibliotecaReporte.Reportes.OrdenesDeProduccion;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.OrdenesDeProduccion
{
    internal class OrdenesProduccionAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_OrdenProduccionReport";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptIngreso = new OrdenProduccionDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<OrdenesProduccionModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptIngreso.CabeceraOrdenProduccion.NewCabeceraOrdenProduccionRow();
                    cabeceraRow.TipoDocumento = detailResult.TipoDocumento;
                    cabeceraRow.NumeroDocumento = detailResult.NumeroDocumento;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.Solicitante = detailResult.Solicitante;
                    cabeceraRow.Cliente = detailResult.Cliente;
                    cabeceraRow.NumeroProforma = detailResult.NumeroProforma;
                    cabeceraRow.FormaDepago = detailResult.FormaDepago;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.Destino = detailResult.Destino;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.Codigo = detailResult.Codigo;
                    cabeceraRow.DescripcionDetalle = detailResult.DescripcionDetalle;
                    cabeceraRow.Marca = detailResult.Marca;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.Empaque = detailResult.Empaque;
                    cabeceraRow.Cartones = detailResult.Cartones;
                    cabeceraRow.Kilos = detailResult.Kilos;
                    cabeceraRow.Libras = detailResult.Libras;
                    cabeceraRow.Motivo = detailResult.Motivo;
                    cabeceraRow.NumeroLote = detailResult.NumeroLote;
                    cabeceraRow.VentaUsuario = detailResult.VentaUsuario;
                    cabeceraRow.DirectorVenta = detailResult.DirectorVenta;
                    rptIngreso.CabeceraOrdenProduccion.AddCabeceraOrdenProduccionRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptIngreso,
                rptCompaniaInfo,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] rptIngresoDataSet)
        {
            using (var report = new RptOrdenProduccion())
            {
                report.SetDataSource(rptIngresoDataSet[0]);

                // Subreporte Cia's
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptIngresoDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }


    }
}
