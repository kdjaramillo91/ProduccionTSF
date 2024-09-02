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
   internal class SaldosSinLoteTodoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_SaldosSinLoteTodo";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static SaldoSinLoteTodoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptSaldosSinLoteTodos = new SaldoSinLoteTodoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<SaldoSinLoteTodoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptSaldosSinLoteTodos.CabeceraSaldoSinLoteTodo.NewCabeceraSaldoSinLoteTodoRow();
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.IdBodega = detailResult.IdBodega;
                    cabeceraRow.NombreBodega = detailResult.NombreBodega;
                    cabeceraRow.IdUbicacion = detailResult.IdUbicacion;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.IdProducto = detailResult.IdProducto;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.NombreUnidadMedida = detailResult.NombreUnidadMedida;
                    cabeceraRow.MontoEntrada = detailResult.MontoEntrada;
                    cabeceraRow.MontoSalida = detailResult.MontoSalida;
                    cabeceraRow.NameCompania = detailResult.NameCompania;
                    cabeceraRow.NameDivision = detailResult.NameDivision;
                    cabeceraRow.NameBranchOffice = detailResult.NameBranchOffice;
                    cabeceraRow.ItemType = detailResult.ItemType;
                    cabeceraRow.ItemMetricUnit = detailResult.ItemMetricUnit;
                    cabeceraRow.ItemPresentationValue = detailResult.ItemPresentationValue;
                    rptSaldosSinLoteTodos.CabeceraSaldoSinLoteTodo.AddCabeceraSaldoSinLoteTodoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptSaldosSinLoteTodos;
        }
        private static Stream SetDataReport(SaldoSinLoteTodoDataSet rptSaldosSinLoteTodosDataSet)
        {
            using (var report = new RptSaldoSinLoteTodo())
            {
                report.SetDataSource(rptSaldosSinLoteTodosDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }

    }
}
