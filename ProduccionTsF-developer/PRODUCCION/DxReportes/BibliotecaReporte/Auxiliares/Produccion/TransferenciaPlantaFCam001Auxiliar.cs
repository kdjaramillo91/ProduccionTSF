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
    internal class TransferenciaPlantaFCam001Auxiliar
    {

        private const string m_StoredProcedureName = "spPar_TransferenciaPlantaFCam001";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static TransferenciaPlantaFCam001DataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptTransferenciaPlantaFCam001 = new TransferenciaPlantaFCam001DataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<TransferenciaPlantaFCam001Model>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptTransferenciaPlantaFCam001.CabeceraTransferenciaPlantaFcam001.NewCabeceraTransferenciaPlantaFcam001Row();
                    cabeceraRow.Turno = detailResult.Turno;
                    cabeceraRow.HoraInicio = detailResult.HoraInicio;
                    cabeceraRow.HoraCierre = detailResult.HoraCierre;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.PlacaTurno = detailResult.PlacaTurno;
                    cabeceraRow.Turno = detailResult.Turno;
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.NumeroDocumento = detailResult.NumeroDocumento;
                    cabeceraRow.TipoDocumento = detailResult.TipoDocumento;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.CodigoItem = detailResult.CodigoItem;
                    cabeceraRow.NombreItem = detailResult.NombreItem;
                    cabeceraRow.Marc = detailResult.Marc;
                    cabeceraRow.Peso = detailResult.Peso;
                    cabeceraRow.Cliente = detailResult.Cliente;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.NumeroCarro = detailResult.NumeroCarro;
                    cabeceraRow.NumeroCajas = detailResult.NumeroCajas;
                    cabeceraRow.Logo = detailResult.Logo;
                    rptTransferenciaPlantaFCam001.CabeceraTransferenciaPlantaFcam001.AddCabeceraTransferenciaPlantaFcam001Row(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptTransferenciaPlantaFCam001;
        }
        private static Stream SetDataReport(TransferenciaPlantaFCam001DataSet rptTransferenciaPlantaFCam001DataSet)
        {
            using (var report = new RptTransferenciaPlantaFCam001())
            {
                report.SetDataSource(rptTransferenciaPlantaFCam001DataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
