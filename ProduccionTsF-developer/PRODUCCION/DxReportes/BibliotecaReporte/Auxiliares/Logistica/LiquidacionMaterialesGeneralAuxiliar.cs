using BibliotecaReporte.Dataset;
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
   internal  class LiquidacionMaterialesGeneralAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LiquidacionMaterialesGeneral";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptLiquidacionMaterialesGeneral = new LiquidacionMaterialesGeneralDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LiquidacionMaterialesGeneralModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptLiquidacionMaterialesGeneral.CabeceraLiquidacionMaterialesGeneral.NewCabeceraLiquidacionMaterialesGeneralRow();
                    cabeceraRow.IdProveedor = detailResult.IdProveedor;
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
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
                    cabeceraRow.Telefono = detailResult.Telefono;
                    cabeceraRow.NombreCompania = detailResult.NombreCompania;
                    cabeceraRow.Unidad_Medida = detailResult.Unidad_Medida;
                    cabeceraRow.Ruc_Compania = detailResult.Ruc_Compania;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.DateInit = detailResult.DateInit;
                    cabeceraRow.DateEnd = detailResult.DateEnd;
                    rptLiquidacionMaterialesGeneral.CabeceraLiquidacionMaterialesGeneral.AddCabeceraLiquidacionMaterialesGeneralRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptLiquidacionMaterialesGeneral,
                rptCompaniaInfo,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] rptLiquidacionMaterialesGeneralesDataSet)
        {
            using (var report = new RptLiquidacionMaterialesGeneral())
            {
                report.SetDataSource(rptLiquidacionMaterialesGeneralesDataSet[0]);

                // Subreporte Cia's
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptLiquidacionMaterialesGeneralesDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
