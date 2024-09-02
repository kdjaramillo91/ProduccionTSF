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
    internal class MovimientoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_Movimiento";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static MovimientoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMovimientos = new MovimientoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MovimientoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMovimientos.CabeceraMovimiento.NewCabeceraMovimientoRow();
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.NumeroDocumentoInventario = detailResult.NumeroDocumentoInventario;
                    cabeceraRow.IdBodega = detailResult.IdBodega;
                    cabeceraRow.NombreBodega = detailResult.NombreBodega;
                    cabeceraRow.IdUbicacion = detailResult.IdUbicacion;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.FechaEmison = detailResult.FechaEmison;
                    cabeceraRow.NombreMotivoInventario = detailResult.NombreMotivoInventario;
                    cabeceraRow.NombreUnidadMedida = detailResult.NombreUnidadMedida;
                    cabeceraRow.MontoEntrada = detailResult.MontoEntrada;
                    cabeceraRow.MontoSalida = detailResult.MontoSalida;
                    cabeceraRow.NombreEstado = detailResult.NombreEstado;
                    cabeceraRow.NameCompania = detailResult.NameCompania;
                    cabeceraRow.NameBranchOffice = detailResult.NameBranchOffice;
                    cabeceraRow.NumberRemissionGuide = detailResult.NumberRemissionGuide;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.IdMotivoInventario = detailResult.IdMotivoInventario;
                    rptMovimientos.CabeceraMovimiento.AddCabeceraMovimientoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }
            return rptMovimientos;
        }
        private static Stream SetDataReport(MovimientoDataSet rptMovimientoDataSet)
        {
            using (var report = new RptMovimiento())
            {
                report.SetDataSource(rptMovimientoDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }

    }
}
