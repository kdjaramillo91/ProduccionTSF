using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.Logistica;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Logistica;
using BibliotecaReporte.Reportes.Logistica;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Logistica
{
    internal class RequerimientoInventarioAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_RequerimientoInventario";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptRequerimientoInventario = new RequerimientoInventarioDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<RequerimientoInventarioModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptRequerimientoInventario.CabeceraRequerimientoInventario.NewCabeceraRequerimientoInventarioRow();
                    cabeceraRow.IdRequerimiento = detailResult.IdRequerimiento;
                    cabeceraRow.NumeroRequerimiento = detailResult.NumeroRequerimiento;
                    cabeceraRow.NumeroRequisicion = detailResult.NumeroRequisicion;
                    cabeceraRow.NaturalezaMovimiento = detailResult.NaturalezaMovimiento;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.PersonaRequiere = detailResult.PersonaRequiere;
                    cabeceraRow.CodigoProducto = detailResult.CodigoProducto;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.NombreBodega = detailResult.NombreBodega;
                    cabeceraRow.Ubicacion = detailResult.Ubicacion;
                    cabeceraRow.NumeroGuia = detailResult.NumeroGuia;
                    cabeceraRow.Medida = detailResult.Medida;
                    cabeceraRow.FechaGuia = detailResult.FechaGuia;
                    cabeceraRow.CantidadRequerida = detailResult.CantidadRequerida;
                    cabeceraRow.CantidadEntregada = detailResult.CantidadEntregada;

                    rptRequerimientoInventario.CabeceraRequerimientoInventario.AddCabeceraRequerimientoInventarioRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptRequerimientoInventario,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptRequerimientoInventario)
        {
            using (var report = new RptRequerimientoInventario())
            {
                report.SetDataSource(rptRequerimientoInventario[0]);

                // Subreporte Cia's
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptRequerimientoInventario[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
