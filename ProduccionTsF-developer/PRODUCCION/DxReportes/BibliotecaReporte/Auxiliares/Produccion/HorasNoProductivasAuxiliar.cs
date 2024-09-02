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
    internal class HorasNoProductivasAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_RptHorasNoTrabajadasReport";
        private const string m_StoredProcedureNameSub = "spPar_RptHorasNoTrabajadasDetalleReport";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptHorasNoProductivas = new HorasNoProductivasDataSet();
            var rptHorasNoProductivasDetalle = new HorasNoProductivasSubDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<HorasNoProductivasModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptHorasNoProductivas.CabeceraHorasNoProductivas.NewCabeceraHorasNoProductivasRow();
                    cabeceraRow.NumeroDocumento = detailResult.NumeroDocumento;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.Maquina = detailResult.Maquina;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.Turno = detailResult.Turno;
                    cabeceraRow.TotalParada = detailResult.TotalParada;
                    cabeceraRow.TotalProduccion = detailResult.TotalProduccion;
                    cabeceraRow.Total = detailResult.Total;
                    cabeceraRow.DProveedor = detailResult.DProveedor;
                    cabeceraRow.DPiscina = detailResult.DPiscina;
                    cabeceraRow.DGrammage = detailResult.DGrammage;
                    cabeceraRow.DLote = detailResult.DLote;
                    cabeceraRow.Dcc = detailResult.Dcc;
                    cabeceraRow.Dsc = detailResult.Dsc;
                    cabeceraRow.DLibrasIngresadas = detailResult.DLibrasIngresadas;
                    cabeceraRow.DHoraInicio = detailResult.DHoraInicio;
                    cabeceraRow.DHoraFin = detailResult.DHoraFin;
                    cabeceraRow.DLibrasProcesadas = detailResult.DLibrasProcesadas;
                    cabeceraRow.DNumeroPersonas = detailResult.DNumeroPersonas;
                    cabeceraRow.DMotivo = detailResult.DMotivo;
                    cabeceraRow.DObservacion = detailResult.DObservacion;
                    cabeceraRow.Cabeza = detailResult.Cabeza;
                    cabeceraRow.HoraCabeza = detailResult.HoraCabeza;
                    cabeceraRow.Cola = detailResult.Cola;
                    cabeceraRow.Totald2 = detailResult.Totald2;
                    cabeceraRow.HoraCola = detailResult.HoraCola;
                    cabeceraRow.TotalHoras = detailResult.TotalHoras;
                    cabeceraRow.Personas = detailResult.Personas;
                    cabeceraRow.MinutosNoProductivo = detailResult.MinutosNoProductivo;
                    cabeceraRow.MinutosProductivo = detailResult.MinutosProductivo;
                    cabeceraRow.CabezaMinuto = detailResult.CabezaMinuto;
                    cabeceraRow.Colaminuto = detailResult.Colaminuto;
                    cabeceraRow.Totalminuto = detailResult.Totalminuto;
                    cabeceraRow.Logo = detailResult.Logo;
                    rptHorasNoProductivas.CabeceraHorasNoProductivas.AddCabeceraHorasNoProductivasRow(cabeceraRow);
                }


                var subDetailsResult = db.Query<HorasNoProductivasSubModel>(m_StoredProcedureNameSub, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in subDetailsResult)
                {
                    var cabeceraRow = rptHorasNoProductivasDetalle.HorasNoProductivasSubReporte.NewHorasNoProductivasSubReporteRow();
                    cabeceraRow.Motivo = detailResult.Motivo;
                    cabeceraRow.Paradas = detailResult.Paradas;
                    cabeceraRow.Minutos = detailResult.Minutos;
                    cabeceraRow.TotalMinutos = detailResult.TotalMinutos;
                    cabeceraRow.IdMot = detailResult.IdMotivo;
                    cabeceraRow.Code = detailResult.CodigoMotivo;
                    cabeceraRow.HoraMinutos = detailResult.HoraMinutos;
                    cabeceraRow.HoraTotal = detailResult.HoraTotal;
                    rptHorasNoProductivasDetalle.HorasNoProductivasSubReporte.AddHorasNoProductivasSubReporteRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptHorasNoProductivas,
                rptHorasNoProductivasDetalle,
            };
            
            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptHorasNoProductivas())
            {
                report.SetDataSource(dataSet[0]);
                //Subreportes
                report.OpenSubreport("RptSubHorasNoProductivas.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
