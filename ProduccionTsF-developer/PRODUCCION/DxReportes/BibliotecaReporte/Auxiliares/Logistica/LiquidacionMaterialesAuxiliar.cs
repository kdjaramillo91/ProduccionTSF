using BibliotecaReporte.Dataset.Logistica;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Logistica;
using BibliotecaReporte.Reportes.Logistica;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.Logistica
{
   internal class LiquidacionMaterialesAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LiquidacionMateriales";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }


        private static LiquidacionMaterialesDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptLiquidacionMateriales = new LiquidacionMaterialesDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LiquidacionMaterialesModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptLiquidacionMateriales.CabeceraLiquidacionMateriales.NewCabeceraLiquidacionMaterialesRow();
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.RUC = detailResult.RUC;
                    cabeceraRow.INP = detailResult.INP;
                    cabeceraRow.FechaLiquidacion = detailResult.FechaLiquidacion;
                    cabeceraRow.NumeroLiquidacion = detailResult.NumeroLiquidacion;
                    cabeceraRow.SecuenciaGuia = detailResult.SecuenciaGuia;
                    cabeceraRow.FechaEmisionGuia = detailResult.FechaEmisionGuia;
                    cabeceraRow.IdProducto = detailResult.IdProducto;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.CodigoProducto = detailResult.CodigoProducto;
                    cabeceraRow.CantidadDetail = detailResult.CantidadDetail; 
                    cabeceraRow.CantidadFacturada = detailResult.CantidadFacturada;
                    cabeceraRow.PrecioUnitarioDetail = detailResult.PrecioUnitarioDetail;
                    cabeceraRow.SubTotalDetail = detailResult.SubTotalDetail;
                    cabeceraRow.SubtotalTaxDetail = detailResult.SubtotalTaxDetail;
                    cabeceraRow.TotalDetail = detailResult.TotalDetail;
                    cabeceraRow.NombreCompania = detailResult.NombreCompania;
                    cabeceraRow.Telefono = detailResult.Telefono; 
                    cabeceraRow.Unidad_Medida = detailResult.Unidad_Medida;
                    cabeceraRow.Ruc_Compania = detailResult.Ruc_Compania;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.EsPreliminar = detailResult.EsPreliminar;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;

                    rptLiquidacionMateriales.CabeceraLiquidacionMateriales.AddCabeceraLiquidacionMaterialesRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptLiquidacionMateriales;
        }
        private static Stream SetDataReport(LiquidacionMaterialesDataSet rptLiquidacionMaterialesDataSet)
        {
            using (var report = new RptLiquidacionMateriales())
            {
                report.SetDataSource(rptLiquidacionMaterialesDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
