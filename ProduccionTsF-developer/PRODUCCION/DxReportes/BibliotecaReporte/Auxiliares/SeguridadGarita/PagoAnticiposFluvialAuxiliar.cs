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
    internal class PagoAnticiposFluvialAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_PagosAnticiposFluvial";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static PagosAnticiposFluvialDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptPagosAnticipadosTerrestre = new PagosAnticiposFluvialDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<PagosAnticiposFluvialModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptPagosAnticipadosTerrestre.CabeceraPagosAnticiposFluvial.NewCabeceraPagosAnticiposFluvialRow();
                    cabeceraRow.Logo = detailResult.Logo;
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
                    cabeceraRow.NumeroAnticipoFluvial = detailResult.NumeroAnticipoFluvial;
                    cabeceraRow.UsuarioPago = detailResult.UsuarioPago;
                    rptPagosAnticipadosTerrestre.CabeceraPagosAnticiposFluvial.AddCabeceraPagosAnticiposFluvialRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptPagosAnticipadosTerrestre;
        }

        private static Stream SetDataReport(PagosAnticiposFluvialDataSet rptPagosAnticiposFluvialDataSet)
        {
            using (var report = new RptPagosAnticiposFluvial())
            {
                report.SetDataSource(rptPagosAnticiposFluvialDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }




    }
}
