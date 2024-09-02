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
    internal class LiquidacionCarroXCarroAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LiquidacionCarroXCarro";
        private const string m_StoredProcedureNameSub = "spPar_LiquidacionCarroCarroResumen";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptAnticipoCompraCamaron = new LiquidacionCarroXCarroDataSet();
            var rptAnticipoCompraCamaronResumen = new LiquidacionCarroXCarroResumenDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LiquidacionCarroXCarroModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptAnticipoCompraCamaron.CabeceraLiquidacionCarroXCarro.NewCabeceraLiquidacionCarroXCarroRow();
                    cabeceraRow.NumeroLote = detailResult.NumeroLote;
                    cabeceraRow.NombreCarro = detailResult.NombreCarro;
                    cabeceraRow.NombreProductoPrimario = detailResult.NombreProductoPrimario;
                    cabeceraRow.NombreTallaProductoPrimario = detailResult.NombreTallaProductoPrimario;
                    cabeceraRow.CajasCantidad = detailResult.CajasCantidad;
                    cabeceraRow.Piscina = detailResult.Piscina;
                    cabeceraRow.FechaRecepcion = detailResult.FechaRecepcion;
                    cabeceraRow.LibrasDespachadas = detailResult.LibrasDespachadas;
                    cabeceraRow.LibrasProcesadas = detailResult.LibrasProcesadas;
                    cabeceraRow.FechaProceso = detailResult.FechaProceso;
                    cabeceraRow.HoraInicio = detailResult.HoraInicio;
                    cabeceraRow.HoraFin = detailResult.HoraFin;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.Turno = detailResult.Turno;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.SecTransaccional = detailResult.SecTransaccional;
                    cabeceraRow.NumeroLiq = detailResult.NumeroLiq;
                    cabeceraRow.Maquina = detailResult.Maquina;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.NombreLiquidador = detailResult.NombreLiquidador;
                    cabeceraRow.Proceso_Producto = detailResult.Proceso_Producto;
                    rptAnticipoCompraCamaron.CabeceraLiquidacionCarroXCarro.AddCabeceraLiquidacionCarroXCarroRow(cabeceraRow);
                }

                var subDetail = db.Query<LiquidacionCarroXCarroResumenModel>(m_StoredProcedureNameSub, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in subDetail)
                {
                    var cabeceraRow = rptAnticipoCompraCamaronResumen.LiquidacionCarroXCarroResumen.NewLiquidacionCarroXCarroResumenRow();
                    cabeceraRow.CodigoResumen = detailResult.CodigoResumen;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.TotalLibras = detailResult.TotalLibras;
                    cabeceraRow.ProcesoProducto = detailResult.ProcesoProducto;
                    rptAnticipoCompraCamaronResumen.LiquidacionCarroXCarroResumen.AddLiquidacionCarroXCarroResumenRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptAnticipoCompraCamaron,
                rptAnticipoCompraCamaronResumen,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptLiquidacionCarroXCarro())
            {
                report.SetDataSource(dataSet[0]);
                //SubReporte
                report.OpenSubreport("RptLiquidacionCarroXCarroResumen.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
