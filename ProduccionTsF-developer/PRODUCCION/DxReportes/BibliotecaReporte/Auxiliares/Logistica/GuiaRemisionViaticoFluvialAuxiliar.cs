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
   internal  class GuiaRemisionViaticoFluvialAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_GuiaRemisionViaticoFluvial";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static GuiaRemisionViaticoFluvialDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptGuiaRemisionPersFluvial = new GuiaRemisionViaticoFluvialDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<GuiaRemisionViaticoFluvialModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptGuiaRemisionPersFluvial.CabeceraGuiaRemisionViaticoFluvial.NewCabeceraGuiaRemisionViaticoFluvialRow();
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    cabeceraRow.Documento = detailResult.Documento;
                    cabeceraRow.EmissionDate = detailResult.EmissionDate;
                    cabeceraRow.Viatico = detailResult.Viatico;
                    cabeceraRow.Persona = detailResult.Persona;
                    cabeceraRow.CiPersona = detailResult.CiPersona;
                    cabeceraRow.Rol = detailResult.Rol;
                    cabeceraRow.Ruc = detailResult.Ruc; 
                    cabeceraRow.Telefono = detailResult.Telefono;
                    cabeceraRow.Description = detailResult.Description;
                    cabeceraRow.NumeroViaticoFluvial = detailResult.NumeroViaticoFluvial;
                    cabeceraRow.UsuarioPago = detailResult.UsuarioPago;
                    rptGuiaRemisionPersFluvial.CabeceraGuiaRemisionViaticoFluvial.AddCabeceraGuiaRemisionViaticoFluvialRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptGuiaRemisionPersFluvial;
        }
        private static Stream SetDataReport(GuiaRemisionViaticoFluvialDataSet rptGuiaRemisionPersFluvialDataSet)
        {
            using (var report = new RptGuiaRemisionViaticoFluvial())
            {
                report.SetDataSource(rptGuiaRemisionPersFluvialDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
