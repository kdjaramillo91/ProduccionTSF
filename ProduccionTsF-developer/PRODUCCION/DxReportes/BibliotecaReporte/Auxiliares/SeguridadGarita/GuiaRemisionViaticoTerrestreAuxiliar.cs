using BibliotecaReporte.Dataset.SeguridadGarita;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.SeguridadGarita;
using BibliotecaReporte.Reportes.SeguridadGarita;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.SeguridadGarita
{
    internal class GuiaRemisionViaticoTerrestreAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_GuideRemisionViaticoTerrestre";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static GuiaRemisionViaticoTerrestreDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptPagosAnticipadosTerrestre = new GuiaRemisionViaticoTerrestreDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<GuiaRemisionViaticoTerrestreModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptPagosAnticipadosTerrestre.CabeceraGuiaRemisionViaticoTerrestre.NewCabeceraGuiaRemisionViaticoTerrestreRow();
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Documento = detailResult.Documento;
                    cabeceraRow.EmissionDate = detailResult.EmissionDate;
                    cabeceraRow.Viatico = detailResult.Viatico;
                    cabeceraRow.Persona = detailResult.Persona;
                    cabeceraRow.Cipersona = detailResult.Cipersona;
                    cabeceraRow.Rol = detailResult.Rol;
                    cabeceraRow.Ruc = detailResult.Ruc;
                    cabeceraRow.Telefono = detailResult.Telefono;
                    cabeceraRow.Description = detailResult.Description;
                    cabeceraRow.NumeroViaticoPersonal = detailResult.NumeroViaticoPersonal;
                    cabeceraRow.UsuarioPago = detailResult.UsuarioPago;                    
                    rptPagosAnticipadosTerrestre.CabeceraGuiaRemisionViaticoTerrestre.AddCabeceraGuiaRemisionViaticoTerrestreRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptPagosAnticipadosTerrestre;
        }
        private static Stream SetDataReport(GuiaRemisionViaticoTerrestreDataSet rptpagosAnticiposTerrestreDataSet)
        {
            using (var report = new RptGuiaRemisionViaticoTerrestre())
            {
                report.SetDataSource(rptpagosAnticiposTerrestreDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }


    }
}
