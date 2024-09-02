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
    internal class ISFAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ISF";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static ISFDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptISF = new ISFDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ISFModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptISF.CabeceraISF.NewCabeceraISFRow();
                    cabeceraRow.ID = detailResult.ID;
                    cabeceraRow.BL = detailResult.BL;
                    cabeceraRow.Buque = detailResult.Buque;
                    cabeceraRow.VIAJE = detailResult.VIAJE;
                    cabeceraRow.Direccion = detailResult.Direccion;
                    cabeceraRow.PurchaseOrder = detailResult.PurchaseOrder;
                    cabeceraRow.Naviera = detailResult.Naviera;
                    cabeceraRow.NOMBRECIA = detailResult.NOMBRECIA;
                    cabeceraRow.DIRECCIONCIA = detailResult.DIRECCIONCIA;
                    cabeceraRow.CIUDADPAIS = detailResult.CIUDADPAIS;
                    cabeceraRow.CLIENTEEXTERIOR = detailResult.CLIENTEEXTERIOR;
                    cabeceraRow.CONSIGNATARIO = detailResult.CONSIGNATARIO;
                    cabeceraRow.CONSIGNATARIODIRECCION = detailResult.CONSIGNATARIODIRECCION;
                    cabeceraRow.CIUDADEMBARQUE = detailResult.CIUDADEMBARQUE;
                    cabeceraRow.PUERTODESCARGA2 = detailResult.PUERTODESCARGA2;
                    cabeceraRow.Contenedores = detailResult.Contenedores;                    
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.Fechaeta = detailResult.Fechaeta;
                    cabeceraRow.FECHAISF = detailResult.FECHAISF;
                    cabeceraRow.FECHAETD2 = detailResult.FECHAETD2;

                    rptISF.CabeceraISF.AddCabeceraISFRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptISF;
        }

        private static Stream SetDataReport(ISFDataSet rptISFDataSet)
        {
            using (var report = new RptISF())
            {
                report.SetDataSource(rptISFDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}