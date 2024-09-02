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
   internal class SaldosSinLoteAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_SaldosSinLote";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static SaldoSinLoteDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptKardex = new SaldoSinLoteDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<SaldoSinLoteModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptKardex.CabeceraSaldoSinLote.NewCabeceraSaldoSinLoteRow();
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
                    rptKardex.CabeceraSaldoSinLote.AddCabeceraSaldoSinLoteRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptKardex;
        }
        private static Stream SetDataReport(SaldoSinLoteDataSet rptSaldosSinLoteDataSet)
        {
            using (var report = new RptSaldoSinLote())
            {
                report.SetDataSource(rptSaldosSinLoteDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }
    }
}
