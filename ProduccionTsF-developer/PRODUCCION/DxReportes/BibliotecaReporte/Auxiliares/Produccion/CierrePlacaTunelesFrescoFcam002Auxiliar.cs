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
   internal class CierrePlacaTunelesFrescoFcam002Auxiliar
    {
        private const string m_StoredProcedureName = "spPar_ReportFCam002";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static CierrePlacatunelesFrescoFcam002DataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptIngreso = new CierrePlacatunelesFrescoFcam002DataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<CierrePlacaTunelesFrescoFcam002Model>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptIngreso.CabeceraCierrePlacaTunelesFrescoFcam002.NewCabeceraCierrePlacaTunelesFrescoFcam002Row();
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.HoraInicio = detailResult.HoraInicio;
                    cabeceraRow.HFinal = detailResult.HFinal;
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
                    rptIngreso.CabeceraCierrePlacaTunelesFrescoFcam002.AddCabeceraCierrePlacaTunelesFrescoFcam002Row(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptIngreso;
        }
        private static Stream SetDataReport(CierrePlacatunelesFrescoFcam002DataSet rptCierrePlacaTunelesFrescoFcam002DataSet)
        {
            using (var report = new RptCierrePlacaTunelesFrescoFcam002())
            {
                report.SetDataSource(rptCierrePlacaTunelesFrescoFcam002DataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
