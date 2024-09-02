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
    internal class OrdenCompraCamaronAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_OrdenCompraCamaron";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static OrdenCompraCamaronDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptOrdenCompraCamaron = new OrdenCompraCamaronDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<OrdenCompraCamaronModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptOrdenCompraCamaron.CabeceraOrdenCompraCamaron.NewCabeceraOrdenCompraCamaronRow();
                    cabeceraRow.Codigo = detailResult.Codigo;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.Unidad = detailResult.Unidad;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.Precio = detailResult.Precio;
                    cabeceraRow.Valor = detailResult.Valor;
                    cabeceraRow.Fecha = detailResult.Fecha;
                    cabeceraRow.N = detailResult.N;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.Unidad_de_produccion = detailResult.Unidad_de_produccion;
                    cabeceraRow.Via_de_embarque = detailResult.Via_de_embarque;
                    cabeceraRow.Plazo_pago = detailResult.Plazo_pago;
                    cabeceraRow.Forma_pago = detailResult.Forma_pago;
                    cabeceraRow.Comprador = detailResult.Comprador;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Cantidadletras = detailResult.Cantidadletras;
                    cabeceraRow.ProcessPlant = detailResult.ProcessPlant;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    cabeceraRow.Suma_total = detailResult.Suma_total;
                    

                    rptOrdenCompraCamaron.CabeceraOrdenCompraCamaron.AddCabeceraOrdenCompraCamaronRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptOrdenCompraCamaron;
        }

        private static Stream SetDataReport(OrdenCompraCamaronDataSet rptOrdenCompraCamaronDataSet)
        {
            using (var report = new RptOrdenCompraCamaron())
            {
                report.SetDataSource(rptOrdenCompraCamaronDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}