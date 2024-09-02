using BibliotecaReporte.Dataset;
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
    internal class CierreTurnoAuxiliar
    {
        private const string m_StoredProcedureName = "spRptCierreTurno";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptCierreTurno = new CierreTurnoDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<CierreturnoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptCierreTurno.CabeceraCierreTurno.NewCabeceraCierreTurnoRow();
                    cabeceraRow.Turno = detailResult.Turno;
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.NumeroLoquidacion = detailResult.NumeroLoquidacion;
                    cabeceraRow.LiquidacionTurno = detailResult.LiquidacionTurno;
                    cabeceraRow.Cola = detailResult.Cola;
                    cabeceraRow.Entero = detailResult.Entero;
                    cabeceraRow.Total = detailResult.Total;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.HoraInicio = detailResult.HoraInicio;
                    cabeceraRow.HoraFin = detailResult.HoraFin;
                    cabeceraRow.Fecha = detailResult.Fecha;
                    cabeceraRow.Horas = detailResult.Horas;
                    cabeceraRow.LibrasEnviadas = detailResult.LibrasEnviadas;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.Rendimiento = detailResult.Rendimiento;
                    
                    rptCierreTurno.CabeceraCierreTurno.AddCabeceraCierreTurnoRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptCierreTurno,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptCierreTurno())
            {
                report.SetDataSource(dataSet[0]);

                // Subreporte
                report.OpenSubreport("RptLogo.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}