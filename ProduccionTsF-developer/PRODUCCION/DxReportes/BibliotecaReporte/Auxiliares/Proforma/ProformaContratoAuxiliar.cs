using BibliotecaReporte.Dataset.Proforma;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Proforma;
using BibliotecaReporte.Reportes.Proforma;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.Proforma
{
    internal class ProformaContratoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ProformasReportContract";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static ProformaContratoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptProformaContrato = new ProformaContratoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ProformaContratoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptProformaContrato.CabeceraProformaContrato.NewCabeceraProformaContratoRow();
                    cabeceraRow.NombreCiaLargo = detailResult.NombreCiaLargo;
                    cabeceraRow.DirSucural = detailResult.DirSucural;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.RazonSocialSoldTo = detailResult.RazonSocialSoldTo;
                    cabeceraRow.AddressSoldTo1 = detailResult.AddressSoldTo1;
                    cabeceraRow.PuertoDestino = detailResult.PuertoDestino;
                    cabeceraRow.Cartones = detailResult.Cartones;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.Precio = detailResult.Precio;
                    cabeceraRow.CasePackDimension = detailResult.CasePackDimension;
                    cabeceraRow.Marca = detailResult.Marca;
                    cabeceraRow.DescripcionGeneral = detailResult.DescripcionGeneral;
                    cabeceraRow.NombreBanco = detailResult.NombreBanco;
                    cabeceraRow.DireccionBanco = detailResult.DireccionBanco;
                    cabeceraRow.EnrutamientoBanco = detailResult.EnrutamientoBanco;
                    cabeceraRow.CuentaBanco = detailResult.CuentaBanco;
                    cabeceraRow.NombreCompaniaBanco = detailResult.NombreCompaniaBanco;
                    cabeceraRow.NetoKilos = detailResult.NetoKilos;
                    cabeceraRow.BrutoKilos = detailResult.BrutoKilos;
                    cabeceraRow.CantidadContenedores = detailResult.CantidadContenedores;
                    cabeceraRow.DescripcionCliente = detailResult.DescripcionCliente;
                    cabeceraRow.Nproforma = detailResult.Nproforma;
                    cabeceraRow.Categoria2 = detailResult.Categoria2;
                    cabeceraRow.TallaMarcada = detailResult.TallaMarcada;
                    cabeceraRow.ValorTotalTermPago = detailResult.ValorTotalTermPago;
                    cabeceraRow.ValorTotalTermPagoString = detailResult.ValorTotalTermPagoString;
                    cabeceraRow.Balance = detailResult.Balance;
                    cabeceraRow.BalanceString = detailResult.BalanceString;
                    cabeceraRow.Logo = detailResult.Logo;
                    rptProformaContrato.CabeceraProformaContrato.AddCabeceraProformaContratoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptProformaContrato;
        }

        private static Stream SetDataReport(ProformaContratoDataSet rptProformaContratoDataSet)
        {
            using (var report = new RptProformaContrato())
            {
                report.SetDataSource(rptProformaContratoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }


    }
}
