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
    internal class ResumenComprasPorPeriodoGlaseoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ResumenComprasPorPeriodoGlaseo";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptResumenComprasPorPeriodoGlaseo = new ResumenComprasPorPeriodoGlaseoDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ResumenComprasPorPeriodoGlaseoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptResumenComprasPorPeriodoGlaseo.CabeceraResumenPorPeriodoGlaseo.NewCabeceraResumenPorPeriodoGlaseoRow();
                    cabeceraRow.Titulo = detailResult.Titulo;
                    cabeceraRow.Ag1Proveedor = detailResult.Ag1Proveedor;
                    cabeceraRow.ProductoGlaseo = detailResult.ProductoGlaseo;
                    cabeceraRow.Valor = detailResult.Valor;
                    cabeceraRow.PrecioPromedio = detailResult.PrecioPromedio;
                    cabeceraRow.Rendimiento = detailResult.Rendimiento;
                    cabeceraRow.FiltroProv = detailResult.FiltroProv;
                    cabeceraRow.Agr2Size = detailResult.Agr2Size;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    cabeceraRow.Compania = detailResult.Compania;
                    cabeceraRow.Ruc = detailResult.Ruc;
                    cabeceraRow.Direccion = detailResult.Direccion;
                    cabeceraRow.TipoProducto = detailResult.TipoProducto;
                    cabeceraRow.FechaDesde = detailResult.FechaDesde;
                    cabeceraRow.FechaHasta = detailResult.FechaHasta;
                    cabeceraRow.Ciudad = detailResult.Ciudad;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.Rendimiento2 = detailResult.Rendimiento2;
                    cabeceraRow.mu = detailResult.mu;
                    cabeceraRow.AgrTipo = detailResult.AgrTipo;
                    cabeceraRow.itty = detailResult.itty;
                    cabeceraRow.Marca = detailResult.Marca;


                    rptResumenComprasPorPeriodoGlaseo.CabeceraResumenPorPeriodoGlaseo.AddCabeceraResumenPorPeriodoGlaseoRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptResumenComprasPorPeriodoGlaseo,
                rptCompaniaInfo,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] dateSet)
        {
            using (var report = new RptResumenComprasPorPeriodoGlaseo())
            {
                report.SetDataSource(dateSet[0]);

                // Subreporte
                report.OpenSubreport("RptLogo.rpt").SetDataSource(dateSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}