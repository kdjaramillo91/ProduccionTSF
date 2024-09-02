using BibliotecaReporte.Dataset;
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
    internal class PagosAnticiposTerrestreAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_PagosAnticiposTerrestre";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptPagosAnticipadosTerrestre = new PagosAnticiposTerrestreDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<PagosAnticiposTerrestreModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptPagosAnticipadosTerrestre.CabeceraPagosAnticiposTerrestre.NewCabeceraPagosAnticiposTerrestreRow();
                    cabeceraRow.Documento = detailResult.Documento;
                    cabeceraRow.EmissionDate = detailResult.EmissionDate;
                    cabeceraRow.Viatico = detailResult.Viatico;
                    cabeceraRow.Chofer = detailResult.Chofer;
                    cabeceraRow.Transportista = detailResult.Transportista;
                    cabeceraRow.CipersonaTransportista = detailResult.CipersonaTransportista;
                    cabeceraRow.Placa = detailResult.Placa;
                    cabeceraRow.Ruc = detailResult.Ruc;
                    cabeceraRow.Telefono = detailResult.Telefono;
                    cabeceraRow.Description = detailResult.Description;
                    cabeceraRow.NumeroAnticipo = detailResult.NumeroAnticipo;
                    cabeceraRow.UsuarioPago = detailResult.UsuarioPago;                  

                    rptPagosAnticipadosTerrestre.CabeceraPagosAnticiposTerrestre.AddCabeceraPagosAnticiposTerrestreRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptPagosAnticipadosTerrestre,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptpagosAnticiposTerrestreDataSet)
        {
            using (var report = new RptPagosAnticiposTerrestre())
            {
                report.SetDataSource(rptpagosAnticiposTerrestreDataSet[0]);

                // Sub reporte Cia's
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptpagosAnticiposTerrestreDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}