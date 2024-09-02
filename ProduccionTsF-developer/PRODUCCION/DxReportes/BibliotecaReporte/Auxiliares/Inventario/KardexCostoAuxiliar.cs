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
    internal class KardexCostoAuxiliar
    {

        private const string m_StoredProcedureName = "spPar_Kardex";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static KardexCostoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptKardexCosto = new KardexCostoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }
                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<KardexCostoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptKardexCosto.CabeceraKardexCosto.NewCabeceraKardexCostoRow();
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.NumeroDocumentoInventario = detailResult.NumeroDocumentoInventario;
                    cabeceraRow.NombreBodega = detailResult.NombreBodega;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.IdProducto = detailResult.IdProducto;
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
                    cabeceraRow.Provider_name = detailResult.Provider_name;
                    cabeceraRow.Amount = detailResult.Amount;
                    cabeceraRow.AmountCostUnit = detailResult.AmountCostUnit; 
                    cabeceraRow.AmountCostTotal = detailResult.AmountCostTotal;
                    cabeceraRow.PreviousPound = detailResult.PreviousPound;
                    cabeceraRow.PreviousCostPound = detailResult.PreviousCostPound;
                    cabeceraRow.PreviousTotalCostPound = detailResult.PreviousTotalCostPound;
                    cabeceraRow.EntradaPound = detailResult.EntradaPound;
                    cabeceraRow.EntradaCostPound = detailResult.EntradaCostPound;
                    cabeceraRow.EntradaTotalCostPound = detailResult.EntradaTotalCostPound;
                    cabeceraRow.SalidaPound = detailResult.SalidaPound;
                    cabeceraRow.SalidaCostPound = detailResult.SalidaCostPound;
                    cabeceraRow.SalidaTotalCostPound = detailResult.SalidaTotalCostPound;
                    cabeceraRow.FinalPound = detailResult.FinalPound;
                    cabeceraRow.FinalCostPound = detailResult.FinalCostPound;
                    cabeceraRow.FinalTotalCostPound = detailResult.FinalTotalCostPound;
                    cabeceraRow.ItemPresentationDescrip = detailResult.ItemPresentationDescrip; 
                    cabeceraRow.OneItemPound = detailResult.OneItemPound;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    rptKardexCosto.CabeceraKardexCosto.AddCabeceraKardexCostoRow(cabeceraRow);
                }
                if (db.State == ConnectionState.Open) { db.Close(); }
           }
            return rptKardexCosto;
        }
        private static Stream SetDataReport(KardexCostoDataSet rptKardexCostoDataSet)
        {
            using (var report = new RptKardexCosto())
            {
                report.SetDataSource(rptKardexCostoDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.Excel);
                report.Close();
                return streamReport;
            }
        }


    }
}
