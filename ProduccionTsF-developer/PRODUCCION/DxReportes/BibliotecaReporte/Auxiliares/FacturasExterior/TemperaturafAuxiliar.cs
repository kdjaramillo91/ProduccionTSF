using BibliotecaReporte.Dataset.FacturasExterior;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.FacturasExterior;
using BibliotecaReporte.Reportes.FacturasExterior;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.FacturasExterior
{
    internal class TemperaturafAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ISF";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }


        private static TemperaturaDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptProforma = new TemperaturaDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<TemperaturaModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptProforma.CabeceraTemperatura.NewCabeceraTemperaturaRow();
                    cabeceraRow.Buque = detailResult.Buque;
                    cabeceraRow.Viaje = detailResult.Viaje;
                    cabeceraRow.Nombrecia = detailResult.Nombrecia;
                    cabeceraRow.CiudadPais = detailResult.CiudadPais;
                    cabeceraRow.ClienteExterior = detailResult.ClienteExterior;
                    cabeceraRow.PuertoDescarga3 = detailResult.PuertoDescarga3;
                    cabeceraRow.NFactura = detailResult.NFactura;
                    cabeceraRow.Contenedores = detailResult.Contenedores;
                    cabeceraRow.TemperaturaInstruccion = detailResult.TemperaturaInstruccion;
                    cabeceraRow.TipoTemperatura = detailResult.TipoTemperatura;
                    cabeceraRow.NumeroBooking = detailResult.NumeroBooking;
                    cabeceraRow.Po = detailResult.Po;
                    cabeceraRow.Logo = detailResult.Logo;
                    rptProforma.CabeceraTemperatura.AddCabeceraTemperaturaRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptProforma;
        }



        private static Stream SetDataReport(TemperaturaDataSet rptTemperaturaFASet)
        {
            using (var report = new RptTemperatura())
            {
                report.SetDataSource(rptTemperaturaFASet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }


    }
}
