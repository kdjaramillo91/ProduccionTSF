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
    internal class CierreTurnoTemporalAuxiliar
    {
        private const string m_StoredProcedureName = "spRptCierreTurnoTemporal";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptCierreTurnoTemporal = new CierreTurnoTemporalDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<CierreTurnoTemporalModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptCierreTurnoTemporal.CabeceraCierreTurnoTemporal.NewCabeceraCierreTurnoTemporalRow();
                    cabeceraRow.Turno = detailResult.Turno;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.NombreTallaProductoPrimario = detailResult.NombreTallaProductoPrimario;
                    cabeceraRow.Cajas = detailResult.Cajas;
                    cabeceraRow.Libras = detailResult.Libras;
                    cabeceraRow.NombreClientePrimario = detailResult.NombreClientePrimario;
                    cabeceraRow.FechaLiquidacion = detailResult.FechaLiquidacion;
                    cabeceraRow.Estado = detailResult.Estado;

                    rptCierreTurnoTemporal.CabeceraCierreTurnoTemporal.AddCabeceraCierreTurnoTemporalRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptCierreTurnoTemporal,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptCierreTurnoTemporalDataSet)
        {
            using (var report = new RptCierreTurnoTemporal())
            {
                report.SetDataSource(rptCierreTurnoTemporalDataSet[0]);

                // Subreporte
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptCierreTurnoTemporalDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}