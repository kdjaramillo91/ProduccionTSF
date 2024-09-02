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
  internal class SaldoLoteAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_SaldosLote";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static SaldoLoteDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptSaldosLote = new SaldoLoteDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<SaldoLoteModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptSaldosLote.CabeceraSaldoLote.NewCabeceraSaldoLoteRow();
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.IdBodega = detailResult.IdBodega;
                    cabeceraRow.NombreBodega = detailResult.NombreBodega;
                    cabeceraRow.IdUbicacion = detailResult.IdUbicacion;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.NombreUnidadMedida = detailResult.NombreUnidadMedida;
                    cabeceraRow.MontoEntrada = detailResult.MontoEntrada;
                    cabeceraRow.NombreUnidadMedida = detailResult.NombreUnidadMedida;
                    cabeceraRow.MontoEntrada = detailResult.MontoEntrada;
                    cabeceraRow.MontoSalida = detailResult.MontoSalida;
                    cabeceraRow.NameCompania = detailResult.NameCompania;
                    cabeceraRow.NameDivision = detailResult.NameDivision;
                    cabeceraRow.NameBranchOffice = detailResult.NameBranchOffice;
                    cabeceraRow.NumberLot = detailResult.NumberLot;
                    cabeceraRow.Provider_name = detailResult.Provider_name;
                    cabeceraRow.IsCopacking = detailResult.IsCopacking;
                    cabeceraRow.NameProviderShrimp = detailResult.NameProviderShrimp;
                    cabeceraRow.ProductionUnitProviderPool = detailResult.ProductionUnitProviderPool;
                    cabeceraRow.ItemSize = detailResult.ItemSize;
                    cabeceraRow.ItemType = detailResult.ItemType;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.ItemMetricUnit = detailResult.ItemMetricUnit;
                    cabeceraRow.ItemPresentationValue = detailResult.ItemPresentationValue;
                    rptSaldosLote.CabeceraSaldoLote.AddCabeceraSaldoLoteRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptSaldosLote;
        }
        private static Stream SetDataReport(SaldoLoteDataSet rptKardexCostoDataSet)
        {
            using (var report = new RptSaldoLote())
            {
                report.SetDataSource(rptKardexCostoDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }
    }
}
