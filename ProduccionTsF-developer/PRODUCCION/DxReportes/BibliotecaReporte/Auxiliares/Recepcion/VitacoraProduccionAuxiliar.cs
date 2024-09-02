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
    internal class VitacoraProduccionAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_VitacoraProduccion";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptVitacoraProduccion = new VitacotaProduccionDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();


            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<VitacoraProducionModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptVitacoraProduccion.CabeceraVitacoreProduccion.NewCabeceraVitacoreProduccionRow();
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.NombrePiscina = detailResult.NombrePiscina;
                    cabeceraRow.NumeroGavetas = detailResult.NumeroGavetas;
                    cabeceraRow.NombreSitio = detailResult.NombreSitio;
                    cabeceraRow.NombreConductor = detailResult.NombreConductor;
                    cabeceraRow.NumeroInterno = detailResult.NumeroInterno;
                    cabeceraRow.NumeroGuia = detailResult.NumeroGuia;
                    cabeceraRow.SelloRetorno = detailResult.SelloRetorno;
                    cabeceraRow.QuantityRecived = detailResult.QuantityRecived;
                    cabeceraRow.FechaRecepcionInicio = detailResult.FechaRecepcionInicio;
                    cabeceraRow.FechaRecepcionFin = detailResult.FechaRecepcionFin;
                    cabeceraRow.FechaEntrada = detailResult.FechaEntrada;
                    cabeceraRow.ProcesoPlanta = detailResult.ProcesoPlanta;
                    cabeceraRow.DireccionCia = detailResult.DireccionCia;

                    rptVitacoraProduccion.CabeceraVitacoreProduccion.AddCabeceraVitacoreProduccionRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
                                              
            }

            var dataSet = new DataSet[]
            {
                rptVitacoraProduccion,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptVitacoraProducionDataSet)
        {
            using (var report = new RptVitacoraProduccion())
            {
                report.SetDataSource(rptVitacoraProducionDataSet[0]);
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptVitacoraProducionDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);

                report.Close();

                return streamReport;
            }
        }
    }
}