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
    internal class FacturaFiscalAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FacturaFiscal";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static FacturasFiscalDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFacturaFiscal = new FacturasFiscalDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FacturaFiscalModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFacturaFiscal.CabeceraFacturasFiscales.NewCabeceraFacturasFiscalesRow();
                    cabeceraRow.ID = detailResult.ID;
                    cabeceraRow.NumeroFactura = detailResult.NumeroFactura;
                    cabeceraRow.RazonSocialComprador = detailResult.RazonSocialComprador;
                    cabeceraRow.NumeroDae = detailResult.NumeroDae;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.ValorFob = detailResult.ValorFob;
                    cabeceraRow.PesoKilos = detailResult.PesoKilos;
                    cabeceraRow.PesoLibras = detailResult.PesoLibras;
                    cabeceraRow.TotalLibras = detailResult.TotalLibras;
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.RucCia = detailResult.RucCia;
                    cabeceraRow.LogoCia = detailResult.LogoCia;
                    cabeceraRow.Logocompany = detailResult.Logocompany;
                    cabeceraRow.Telefono = detailResult.Telefono;

                    rptFacturaFiscal.CabeceraFacturasFiscales.AddCabeceraFacturasFiscalesRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptFacturaFiscal;
        }

        private static Stream SetDataReport(FacturasFiscalDataSet rptFacturaFiscalDataSet)
        {
            using (var report = new RptFacturaFiscal())
            {
                report.SetDataSource(rptFacturaFiscalDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}