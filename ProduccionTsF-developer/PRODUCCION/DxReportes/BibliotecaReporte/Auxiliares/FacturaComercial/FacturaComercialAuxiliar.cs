using BibliotecaReporte.Dataset.FacturasComercial;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.facturasComercial;
using BibliotecaReporte.Reportes.FacturaComercial;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.FacturaComercial
{
    internal class FacturaComercialAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FacturaComercial";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static FacturaComercialDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFacturaComercial = new FacturaComercialDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FacturaComercialModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFacturaComercial.CabeceraFacturaComercial.NewCabeceraFacturaComercialRow();
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.RazonSocialComprador = detailResult.RazonSocialComprador;
                    cabeceraRow.NumeroDae = detailResult.NumeroDae;
                    cabeceraRow.NumeroFactura = detailResult.NumeroFactura;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.ValorFob = detailResult.ValorFob;
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.RucCia = detailResult.RucCia;
                    cabeceraRow.Logo2Cia = detailResult.Logo2Cia;
                    cabeceraRow.Telefono = detailResult.Telefono;
                    cabeceraRow.IGCAPITAL = detailResult.IGCAPITAL;
                    cabeceraRow.CARTADECREDITO = detailResult.CARTADECREDITO;
                    cabeceraRow.BANCOBOLIVARIANO = detailResult.BANCOBOLIVARIANO;
                    cabeceraRow.COBRANZASBANCARIAS = detailResult.COBRANZASBANCARIAS;
                    cabeceraRow.NOPROBADAS = detailResult.NOPROBADAS;
                    cabeceraRow.PRONTOPAGO = detailResult.PRONTOPAGO;
                    cabeceraRow.valorkilos = detailResult.valorkilos;
                    cabeceraRow.valorlibras = detailResult.valorlibras;
                    cabeceraRow.totalLibras = detailResult.totalLibras;
                    


                    rptFacturaComercial.CabeceraFacturaComercial.AddCabeceraFacturaComercialRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptFacturaComercial;
        }

        private static Stream SetDataReport(FacturaComercialDataSet rptFacturacomercialDataSet)
        {
            using (var report = new RptFacturaComercial())
            {
                report.SetDataSource(rptFacturacomercialDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}