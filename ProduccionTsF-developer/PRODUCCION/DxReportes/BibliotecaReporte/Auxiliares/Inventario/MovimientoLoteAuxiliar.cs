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
   internal class MovimientoLoteAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_MovimientoLote";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static MovimientoLoteDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMovimientosLote = new MovimientoLoteDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MovimientoLoteModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMovimientosLote.CabeceraMovimientoLote.NewCabeceraMovimientoLoteRow();
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.NumeroDocumentoInventario = detailResult.NumeroDocumentoInventario;
                    cabeceraRow.NombreBodega = detailResult.NombreBodega;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.FechaEmison = detailResult.FechaEmison;
                    cabeceraRow.NombreMotivoInventario = detailResult.NombreMotivoInventario;
                    cabeceraRow.NombreUnidadMedida = detailResult.NombreUnidadMedida;
                    cabeceraRow.MontoEntrada = detailResult.MontoEntrada;
                    cabeceraRow.MontoSalida = detailResult.MontoSalida;
                    cabeceraRow.NameCompania = detailResult.NameCompania;
                    cabeceraRow.NameDivision = detailResult.NameDivision;
                    cabeceraRow.NameBranchOffice = detailResult.NameBranchOffice;
                    cabeceraRow.NumberLot = detailResult.NumberLot;                   
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                   // cabeceraRow.NombreProducto = detailResult.NombreProducto;

                    rptMovimientosLote.CabeceraMovimientoLote.AddCabeceraMovimientoLoteRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptMovimientosLote;
        }
        private static Stream SetDataReport(MovimientoLoteDataSet rptMovimientoDataSet)
        {
            using (var report = new RptMovimientoLote())
            {
                report.SetDataSource(rptMovimientoDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }
    }
}
