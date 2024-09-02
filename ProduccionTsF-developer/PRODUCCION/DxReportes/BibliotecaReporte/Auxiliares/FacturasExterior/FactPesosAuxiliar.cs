using BibliotecaReporte.Dataset.FacturasExterior;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.FacturasExterior;
using BibliotecaReporte.Reportes.FacturasExterior;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.FacturasExterior
{
    internal class FactPesosAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FactPesos";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static FactpesosDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptProforma = new FactpesosDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FactpesosModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptProforma.CabeceraFactPesos.NewCabeceraFactPesosRow();
                    cabeceraRow.Factura = detailResult.Factura;
                    cabeceraRow.FechaEmisiOn = detailResult.FechaEmisiOn;
                    cabeceraRow.CartOnes = detailResult.CartOnes;
                    cabeceraRow.Size = detailResult.Size;
                    cabeceraRow.BrutoKilos = detailResult.BrutoKilos;
                    cabeceraRow.GlaseoKilos = detailResult.GlaseoKilos;
                    cabeceraRow.VFOB = detailResult.VFOB;
                    cabeceraRow.TSeguro = detailResult.TSeguro;
                    cabeceraRow.TFlete = detailResult.TFlete;
                    cabeceraRow.TFOB = detailResult.TFOB;
                    cabeceraRow.Tvalortotal = detailResult.Tvalortotal;
                    cabeceraRow.Kgbrutosd = detailResult.Kgbrutosd;
                    cabeceraRow.Kgnetos = detailResult.Kgnetos;
                    cabeceraRow.Kgglaseoneto = detailResult.Kgglaseoneto;
                    cabeceraRow.Nproducto = detailResult.Nproducto;
                    cabeceraRow.RazonSocialSoldTo = detailResult.RazonSocialSoldTo;
                    cabeceraRow.Logo = detailResult.Logo;
                    rptProforma.CabeceraFactPesos.AddCabeceraFactPesosRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptProforma;
        }






        private static Stream SetDataReport(FactpesosDataSet rptFactpesosDataSet)
        {
            using (var report = new RptFactPesos())
            {
                report.SetDataSource(rptFactpesosDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }







    }
}
