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
    internal class CierrePlacaTunelesFrescoFcam003Auxiliar
    {

        private const string m_StoredProcedureName = "spPar_ReportFCam003";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static CierrePlacaTunelesFrescoFcamDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptCierrePlacaTunelesFrescoFcam003 = new CierrePlacaTunelesFrescoFcamDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<CierrePlacaTunelesFrescoFcam003Model>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptCierrePlacaTunelesFrescoFcam003.CabeceraCierrePlacaTunelesFrescoFcam003.NewCabeceraCierrePlacaTunelesFrescoFcam003Row();
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.HoraInicio = detailResult.HoraInicio;
                    cabeceraRow.Hfinal = detailResult.Hfinal;
                    cabeceraRow.Tunel = detailResult.Tunel;
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.Marca = detailResult.Marca;
                    cabeceraRow.Cliente = detailResult.Cliente;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.Peso = detailResult.Peso;
                    cabeceraRow.Color = detailResult.Color;
                    cabeceraRow.NoCoche = detailResult.NoCoche;
                    cabeceraRow.Cajas = detailResult.Cajas;
                    cabeceraRow.Turno = detailResult.Turno;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Empaque = detailResult.Empaque;
                    rptCierrePlacaTunelesFrescoFcam003.CabeceraCierrePlacaTunelesFrescoFcam003.AddCabeceraCierrePlacaTunelesFrescoFcam003Row(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptCierrePlacaTunelesFrescoFcam003;
        }


        private static Stream SetDataReport(CierrePlacaTunelesFrescoFcamDataSet rptCierrePlacaTunelesFrescoFcam003DataSet)
        {
            using (var report = new RptCierrePlacaTunelesFrescoFcam003())
            {
                report.SetDataSource(rptCierrePlacaTunelesFrescoFcam003DataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
