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
   internal class DescabezadoAuxiliares
    {

        private const string m_StoredProcedureName = "spPar_RptDescabezado";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DescabezadoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptDescabezado = new DescabezadoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<DescabezadoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptDescabezado.CabeceraDescabezado.NewCabeceraDescabezadoRow();
                    cabeceraRow.FechaIngreso = detailResult.FechaIngreso;
                    cabeceraRow.Turno = detailResult.Turno;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.Piscina = detailResult.Piscina;
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.Gramage = detailResult.Gramage;
                    cabeceraRow.Color = detailResult.Color;
                    cabeceraRow.Rechazo = detailResult.Rechazo;
                    cabeceraRow.Directo = detailResult.Directo;
                    cabeceraRow.Kavetas = detailResult.Kavetas;
                    cabeceraRow.NPersonas = detailResult.NPersonas;
                    cabeceraRow.HoraInicio = detailResult.HoraInicio;
                    cabeceraRow.HoraFin = detailResult.HoraFin;
                    cabeceraRow.RendimientoManual = detailResult.RendimientoManual;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.SecuenciaAgrupacion = detailResult.SecuenciaAgrupacion;
                    cabeceraRow.SumaRechazo = detailResult.SumaRechazo;
                    cabeceraRow.SumaDirecto = detailResult.SumaDirecto;
                    cabeceraRow.Contador = detailResult.Contador;
                    cabeceraRow.Logo = detailResult.Logo;
              
                    rptDescabezado.CabeceraDescabezado.AddCabeceraDescabezadoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptDescabezado;
        }
        private static Stream SetDataReport(DescabezadoDataSet rptDescabezadoDataSet)
        {
            using (var report = new RptDescabezado())
            {
                // report.FileName = "C:/Users/PROGRA~1/AppData/Local/Temp";
                report.SetDataSource(rptDescabezadoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
