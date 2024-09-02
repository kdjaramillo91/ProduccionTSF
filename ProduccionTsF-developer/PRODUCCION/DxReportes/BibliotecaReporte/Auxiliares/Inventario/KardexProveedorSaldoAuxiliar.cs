using BibliotecaReporte.Dataset.Inventario;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Inventario;
using BibliotecaReporte.Reportes.Inventario;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Inventario
{
    internal class KardexProveedorSaldoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_KardexProveedorSaldo";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static KardexProveedorSaldoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptKardexProveedorSaldo = new KardexProveedorSaldoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<KardexProveedorSaldoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptKardexProveedorSaldo.CabeceraKardexProveedorSaldo.NewCabeceraKardexProveedorSaldoRow();
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.NombreBodega = detailResult.NombreBodega;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.NombreMotivoInventario = detailResult.NombreMotivoInventario;
                    cabeceraRow.MontoEntrada = detailResult.MontoEntrada;
                    cabeceraRow.MontoSalida = detailResult.MontoSalida;
                    cabeceraRow.NameCompania = detailResult.NameCompania;
                    cabeceraRow.NameDivision = detailResult.NameDivision;
                    cabeceraRow.NameBranchOffice = detailResult.NameBranchOffice;                   
                    rptKardexProveedorSaldo.CabeceraKardexProveedorSaldo.AddCabeceraKardexProveedorSaldoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptKardexProveedorSaldo;
        }
        private static Stream SetDataReport(KardexProveedorSaldoDataSet rptKardexProveedorSaldoDataSet)
        {
            using (var report = new RptKardexProveedorSaldo())
            {
                report.SetDataSource(rptKardexProveedorSaldoDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }

    }
}
