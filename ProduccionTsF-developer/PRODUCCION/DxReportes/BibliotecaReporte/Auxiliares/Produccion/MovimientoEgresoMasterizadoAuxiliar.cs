using BibliotecaReporte.Dataset.Produccion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Produccion;
using BibliotecaReporte.Reportes.Produccion;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.Produccion
{
   internal  class MovimientoEgresoMasterizadoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_MovimientoIngresoMasterizado";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static MovimientoEgresoMasterizadoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMovimientoIngresoMasterizado = new MovimientoEgresoMasterizadoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MovimientoEgresoMasterizadoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMovimientoIngresoMasterizado.CabeceraMovimientoEgresoMasterizado.NewCabeceraMovimientoEgresoMasterizadoRow();
                    cabeceraRow.FECHAEMISION = detailResult.FECHAEMISION;
                    cabeceraRow.ESTADO = detailResult.ESTADO;
                    cabeceraRow.CODIGO1 = detailResult.CODIGO1;
                    cabeceraRow.TIPOITEM1 = detailResult.TIPOITEM1;
                    cabeceraRow.NPRODUCTO1 = detailResult.NPRODUCTO1;
                    cabeceraRow.UN = detailResult.UN;
                    cabeceraRow.TALLA1 = detailResult.TALLA1;
                    cabeceraRow.BODEGA1 = detailResult.BODEGA1;
                    cabeceraRow.UBODEGA1 = detailResult.UBODEGA1;
                    cabeceraRow.Cantidad1 = detailResult.Cantidad1;
                    cabeceraRow.CENTROCOSTO2 = detailResult.CENTROCOSTO2;
                    cabeceraRow.SUBCENTROCOSTO2 = detailResult.SUBCENTROCOSTO2;
                    cabeceraRow.LBSOKG1 = detailResult.LBSOKG1;
                    cabeceraRow.Kgolb1 = detailResult.Kgolb1;
                    cabeceraRow.RazonEgreso = detailResult.RazonEgreso;
                    cabeceraRow.SecEgreso = detailResult.SecEgreso;
                    cabeceraRow.Lotenew = detailResult.Lotenew;
                    cabeceraRow.Inlote = detailResult.Inlote;
                    cabeceraRow.NLOTE = detailResult.NLOTE;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    cabeceraRow.LoteMP = detailResult.LoteMP;
                    rptMovimientoIngresoMasterizado.CabeceraMovimientoEgresoMasterizado.AddCabeceraMovimientoEgresoMasterizadoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptMovimientoIngresoMasterizado;
        }
        private static Stream SetDataReport(MovimientoEgresoMasterizadoDataSet rptMovimientoEgresoDataSet)
        {
            using (var report = new RptMovimientoEgresoMasterizado())
            {
                // report.FileName = "C:/Users/PROGRA~1/AppData/Local/Temp";
                report.SetDataSource(rptMovimientoEgresoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
