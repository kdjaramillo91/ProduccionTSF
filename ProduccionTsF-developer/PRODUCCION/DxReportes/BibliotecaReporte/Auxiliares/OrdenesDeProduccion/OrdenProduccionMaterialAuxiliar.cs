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
   internal class OrdenProduccionMaterialAuxiliar
    {
        private const string m_StoredProcedureName = "sp_ParOrdenProduccionMaterial";
        private const string m_StoredProcedureNameSub = "spPar_OrdenProduccionMaterialResumen";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptOrdenProduccionMaterial = new OrdenProduccionMaterialDataSet();
            var rptOrdenProduccionMaterialResumen = new OrdenProduccionMaterialResumenDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<OrdenProduccionMaterialModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptOrdenProduccionMaterial.CabeceraOrdenProduccionMaterial.NewCabeceraOrdenProduccionMaterialRow();
                    cabeceraRow.Codigo = detailResult.Codigo;
                    cabeceraRow.NombreDelProducto = detailResult.NombreDelProducto;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.LineaDeInventario = detailResult.LineaDeInventario;
                    cabeceraRow.TipoDeProducto = detailResult.TipoDeProducto;
                    cabeceraRow.Categoria = detailResult.Categoria;
                    cabeceraRow.codigo = detailResult.codigo;
                    cabeceraRow.Ingrediente = detailResult.Ingrediente;
                    cabeceraRow.CantFormulada = detailResult.CantFormulada;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.UM = detailResult.UM;
                    cabeceraRow.TipodeDocuemnto = detailResult.TipodeDocuemnto;
                    cabeceraRow.NumeroDeProforma = detailResult.NumeroDeProforma;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.Cliente = detailResult.Cliente;
                    cabeceraRow.Destino = detailResult.Destino;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.FormaDepago = detailResult.FormaDepago;
                    cabeceraRow.NumeroDocumento = detailResult.NumeroDocumento;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.Solicitante = detailResult.Solicitante;
                    cabeceraRow.Logo = detailResult.Logo;
                    rptOrdenProduccionMaterial.CabeceraOrdenProduccionMaterial.AddCabeceraOrdenProduccionMaterialRow(cabeceraRow);
                }

                var subDetail = db.Query<OrdenProduccionMaterialResumenModel>(m_StoredProcedureNameSub, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in subDetail)
                {
                    var cabeceraRow = rptOrdenProduccionMaterialResumen.OrdenProduccionMaterialResumen.NewOrdenProduccionMaterialResumenRow();
                    cabeceraRow.MasterCode = detailResult.MasterCode;
                    cabeceraRow.Name = detailResult.Name;
                    cabeceraRow.CantidadFormulada = detailResult.CantidadFormulada;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.MU = detailResult.MU;
                    rptOrdenProduccionMaterialResumen.OrdenProduccionMaterialResumen.AddOrdenProduccionMaterialResumenRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptOrdenProduccionMaterial,
                rptOrdenProduccionMaterialResumen,
            };


            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptOrdenProduccionMaterial())
            {
                report.SetDataSource(dataSet[0]);
                //SubReporte
                report.OpenSubreport("RptOrdenProduccionMaterialResumen.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
