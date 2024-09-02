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
    internal class MargenPorTallasAuxiliar
    {
        private const string m_StoredProcedureName = "SP_MargenPorTallas";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMargenPorTallas = new MargenPorTallasDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();
            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MargenPorTallasModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMargenPorTallas.CabeceraMargenPorTallas.NewCabeceraMargenPorTallasRow();
                    cabeceraRow.Comisionista = detailResult.Comisionista;
                    cabeceraRow.Idestado = detailResult.Idestado;
                    cabeceraRow.Producto = detailResult.Producto;
                    cabeceraRow.AgruparTipoProducto = detailResult.AgruparTipoProducto;
                    cabeceraRow.TipoProducto = detailResult.TipoProducto;
                    cabeceraRow.AgruparCategoriaProducto = detailResult.AgruparCategoriaProducto;
                    cabeceraRow.CategoriaProducto = detailResult.CategoriaProducto;
                    cabeceraRow.AgruparTallas = detailResult.AgruparTallas;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.Rendimiento = detailResult.Rendimiento;
                    cabeceraRow.Valor = detailResult.Valor;
                    cabeceraRow.PrecioPromedio = detailResult.PrecioPromedio;
                    cabeceraRow.ValorRef = detailResult.ValorRef;
                    cabeceraRow.PrecioRef = detailResult.PrecioRef;
                    cabeceraRow.Margen = detailResult.Margen;
                    cabeceraRow.FILTROPROV = detailResult.FILTROPROV;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;                    
                    rptMargenPorTallas.CabeceraMargenPorTallas.AddCabeceraMargenPorTallasRow(cabeceraRow);
                }
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);
                if (db.State == ConnectionState.Open) { db.Close(); }
            }
            var dataSet = new DataSet[]
        {

                rptMargenPorTallas,
                rptCompaniaInfo,
        };
            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptMargenPorTallasDataSet)
        {
            using (var report = new RptMargenPorTallas())
            {
                report.SetDataSource(rptMargenPorTallasDataSet[0]);
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptMargenPorTallasDataSet[1]);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}