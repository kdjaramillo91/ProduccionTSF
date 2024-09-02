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
    internal class KardexLoteAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_KardexLote";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static KardexLoteDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptKardex = new KardexLoteDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<KardexLoteModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptKardex.CabeceraKardexLote.NewCabeceraKardexLoteRow();
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.NumeroDocumentoInventario = detailResult.NumeroDocumentoInventario;
                    cabeceraRow.IdBodega = detailResult.IdBodega;
                    cabeceraRow.NombreBodega = detailResult.NombreBodega;
                    cabeceraRow.IdUbicacion = detailResult.IdUbicacion;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.IdProducto = detailResult.IdProducto;
                    cabeceraRow.FechaEmison = detailResult.FechaEmison;
                    cabeceraRow.NombreMotivoInventario = detailResult.NombreMotivoInventario;
                    cabeceraRow.NombreUnidadMedida = detailResult.NombreUnidadMedida;
                    cabeceraRow.MontoEntrada = detailResult.MontoEntrada;
                    cabeceraRow.MontoSalida = detailResult.MontoSalida;
                    cabeceraRow.Balance = detailResult.Balance;
                    cabeceraRow.PreviousBalance = detailResult.PreviousBalance;
                    cabeceraRow.NameCompania = detailResult.NameCompania;
                    cabeceraRow.NameBranchOffice = detailResult.NameBranchOffice;
                    cabeceraRow.NumberRemissionGuide = detailResult.NumberRemissionGuide;
                    cabeceraRow.NameDivision = detailResult.NameDivision;
                    cabeceraRow.NumberLot = detailResult.NumberLot;
                    cabeceraRow.Provider_name = detailResult.Provider_name;
                    cabeceraRow.IsCopacking = detailResult.IsCopacking;
                    cabeceraRow.NameProviderShrimp = detailResult.NameProviderShrimp;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.ProductionUnitProviderPool = detailResult.ProductionUnitProviderPool;                    
                    rptKardex.CabeceraKardexLote.AddCabeceraKardexLoteRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptKardex;
        }


        private static Stream SetDataReport(KardexLoteDataSet rptKardexLoteDataSet)
        {
            using (var report = new RptKardexlote())
            {
                report.SetDataSource(rptKardexLoteDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }





    }
}
