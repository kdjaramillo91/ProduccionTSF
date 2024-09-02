using BibliotecaReporte.Dataset.Produccion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Produccion;
using BibliotecaReporte.Reportes.Produccion;
using CrystalDecisions.Shared;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.Produccion
{
   internal class MovimientoIngresoMasterizadoAuxiliar
    {

        private const string m_StoredProcedureName = "spPar_MovimientoIngresoMasterizado";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static MovimientoMasterizadoIngresoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMovimientoIngresoMasterizado = new MovimientoMasterizadoIngresoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MovimientoIngresoMasterizadoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMovimientoIngresoMasterizado.CabeceraMovimientoingresoMasterizado.NewCabeceraMovimientoingresoMasterizadoRow();
                    cabeceraRow.ID = detailResult.ID;
                    cabeceraRow.FECHAEMISION = detailResult.FECHAEMISION;
                    cabeceraRow.ESTADO = detailResult.ESTADO;
                    cabeceraRow.NLOTE = detailResult.NLOTE;
                    cabeceraRow.INLOTE = detailResult.INLOTE;
                    cabeceraRow.TIPOITEM1 = detailResult.TIPOITEM1;
                    cabeceraRow.CODIGO2 = detailResult.CODIGO2;
                    cabeceraRow.NPRODUCTO1 = detailResult.NPRODUCTO1;
                    cabeceraRow.UN = detailResult.UN;
                    cabeceraRow.TALLA1 = detailResult.TALLA1;
                    cabeceraRow.CENTROCOSTO1 = detailResult.CENTROCOSTO1;
                    cabeceraRow.SUBCENTROCOSTO1 = detailResult.SUBCENTROCOSTO1;
                    cabeceraRow.BODEGA2 = detailResult.BODEGA2;
                    cabeceraRow.UBODEGA2 = detailResult.UBODEGA2;
                    cabeceraRow.Cantidad2 = detailResult.Cantidad2;
                    cabeceraRow.Cantidad3 = detailResult.Cantidad3;
                    cabeceraRow.LBSOKG2 = detailResult.LBSOKG2;
                    cabeceraRow.Kgolb2 = detailResult.Kgolb2;
                    cabeceraRow.RazonMaster = detailResult.RazonMaster;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    cabeceraRow.LoteMP = detailResult.LoteMP;
                    cabeceraRow.NLoteP = detailResult.NLoteP;
                    rptMovimientoIngresoMasterizado.CabeceraMovimientoingresoMasterizado.AddCabeceraMovimientoingresoMasterizadoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptMovimientoIngresoMasterizado;
        }
        private static Stream SetDataReport(MovimientoMasterizadoIngresoDataSet rptMovimientoIngresoDataSet)
        {
            using (var report = new RptMovimientoIngresoMasterizado())
            {
                // report.FileName = "C:/Users/PROGRA~1/AppData/Local/Temp";
                report.SetDataSource(rptMovimientoIngresoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
