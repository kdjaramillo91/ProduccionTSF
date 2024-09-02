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
    internal class AnticipoComprarCamaronAuxiliar
    {
        private const string m_StoredProcedureName = "spAnticipoCompraCamaron";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static AnticipoCompraCamaronDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptAnticipoCompraCamaron = new AnticipoCompraCamaronDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<AnticipoCompraCamaronModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptAnticipoCompraCamaron.CabeceraAnticipoCompraCamaron.NewCabeceraAnticipoCompraCamaronRow();
                    cabeceraRow.NombreCompania = detailResult.NombreCompania;
                    cabeceraRow.RucCompania = detailResult.RucCompania;
                    cabeceraRow.LogoCompania = detailResult.LogoCompania;
                    cabeceraRow.NombreCompania = detailResult.NombreCompania;
                    cabeceraRow.RucCompania = detailResult.RucCompania;
                    cabeceraRow.NumeroCompania = detailResult.NumeroCompania;
                    cabeceraRow.FechaRecepcion = detailResult.FechaRecepcion;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.LoteInterno = detailResult.LoteInterno;
                    cabeceraRow.LibrasRecibidas = detailResult.LibrasRecibidas;
                    cabeceraRow.PrecioPromedio = detailResult.PrecioPromedio;
                    cabeceraRow.ValorAnticipo = detailResult.ValorAnticipo;
                    cabeceraRow.EstadoAnticipo = detailResult.EstadoAnticipo;
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.Secuencia = detailResult.Secuencia;
                    cabeceraRow.ProcesoPlanta = detailResult.ProcesoPlanta;
                    rptAnticipoCompraCamaron.CabeceraAnticipoCompraCamaron.AddCabeceraAnticipoCompraCamaronRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptAnticipoCompraCamaron;
        }
        private static Stream SetDataReport(AnticipoCompraCamaronDataSet rptAnticipoCompraCamaronDataSet)
        {
            using (var report = new RptAnticipoCompraCamaron())
            {
                report.SetDataSource(rptAnticipoCompraCamaronDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
