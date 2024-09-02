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
    internal class CierreLiquidacionRendimientoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_CierreLiquidacionRendimiento";
        private const string m_StoredProcedureNameSubReport = "spPar_CierreLiquidacionRendimientoResumen";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptCierreLiquidacionRendimiento = new CierreLiquidacionRendimientoDataSet();
            var rptCierreLiquidacionRendimientoResumen = new CierreLiquidacionRendimientoResumenDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<CierreLiquidacionRendimientoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptCierreLiquidacionRendimiento.CabeceraCierreLiquidacionRendimiento.NewCabeceraCierreLiquidacionRendimientoRow();
                    cabeceraRow.IdProduccion = detailResult.IdProduccion;
                    cabeceraRow.FechaHoraRecepcion = detailResult.FechaHoraRecepcion;
                    cabeceraRow.NumeroLote = detailResult.NumeroLote;
                    cabeceraRow.CodeProcessType = detailResult.CodeProcessType;
                    cabeceraRow.NameProcessType = detailResult.NameProcessType;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.TallaProducto = detailResult.TallaProducto;
                    cabeceraRow.CantidadCajas = detailResult.CantidadCajas;
                    cabeceraRow.GuiaRemision = detailResult.GuiaRemision;
                    cabeceraRow.CantidadMinimaXcajas = detailResult.CantidadMinimaXcajas;
                    cabeceraRow.Rendimiento = detailResult.Rendimiento;
                    cabeceraRow.Rendimiento2 = detailResult.Rendimiento2;
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.NombrePiscina = detailResult.NombrePiscina;
                    cabeceraRow.SecuenciaLiquidacion = detailResult.SecuenciaLiquidacion;
                    cabeceraRow.LibrasProcesadas = detailResult.LibrasProcesadas;
                    cabeceraRow.LibrasBasuras = detailResult.LibrasBasuras;
                    cabeceraRow.LibrasRecibidas = detailResult.LibrasRecibidas;
                    cabeceraRow.LibrasNetas = detailResult.LibrasNetas;
                    cabeceraRow.PorcentajeRendimiento = detailResult.PorcentajeRendimiento;
                    cabeceraRow.PorcentajeRendimiento2 = detailResult.PorcentajeRendimiento2;
                    cabeceraRow.LibrasDespachadas = detailResult.LibrasDespachadas;
                    cabeceraRow.LibrasRechazo = detailResult.LibrasRechazo;
                    cabeceraRow.IdCompany = detailResult.IdCompany;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.FechaProceso = detailResult.FechaProceso; 
                    cabeceraRow.HoraInicio = detailResult.HoraInicio;
                    cabeceraRow.HoraFin = detailResult.HoraFin;
                    cabeceraRow.Metricunit = detailResult.Metricunit;
                    cabeceraRow.TipoProces = detailResult.TipoProces;                    
                    rptCierreLiquidacionRendimiento.CabeceraCierreLiquidacionRendimiento.AddCabeceraCierreLiquidacionRendimientoRow(cabeceraRow);
                }

                var subDetail = db.Query<CierreLiquidacionRendimientoResumenModel>(m_StoredProcedureNameSubReport, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in subDetail)
                {
                    var detailRow = rptCierreLiquidacionRendimientoResumen.CierreLiquidacionRendimientoResumen.NewCierreLiquidacionRendimientoResumenRow();
                    detailRow.NombreMaquina = detailResult.NombreMaquina;
                    detailRow.NombreTurno = detailResult.NombreTurno;
                    detailRow.FechaEmision = detailResult.FechaEmision;                    
                    detailRow.HoraInicio = detailResult.HoraInicio;
                    detailRow.HoraFin = detailResult.HoraFin;
                    detailRow.LibrasProcesadas = detailResult.LibrasProcesadas;
                    detailRow.TipoProceso = detailResult.TipoProceso;
                    rptCierreLiquidacionRendimientoResumen.CierreLiquidacionRendimientoResumen.AddCierreLiquidacionRendimientoResumenRow(detailRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptCierreLiquidacionRendimiento,
                rptCierreLiquidacionRendimientoResumen,
                rptCompaniaInfo,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptCierreLiquidacionRendimiento())
            {
                report.SetDataSource(dataSet[0]);

                //SubReporte
                report.OpenSubreport("RptCierreLiquidacionRendimientoResumen.rpt").SetDataSource(dataSet[1]);
                report.OpenSubreport("RptLogo.rpt").SetDataSource(dataSet[2]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
