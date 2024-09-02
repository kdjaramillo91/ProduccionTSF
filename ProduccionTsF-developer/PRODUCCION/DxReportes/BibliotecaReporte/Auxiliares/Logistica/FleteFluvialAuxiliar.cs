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
    internal class FleteFluvialAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FleteFluvial";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static FleteFluvialDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFletefluvial = new FleteFluvialDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FleteFluvialModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFletefluvial.CabeceraFleteFluvial.NewCabeceraFleteFluvialRow();
                    cabeceraRow.Flete = detailResult.Flete;
                    cabeceraRow.Anticipo = detailResult.Anticipo;
                    cabeceraRow.Ajuste = detailResult.Ajuste;
                    cabeceraRow.NumDoc= detailResult.NumDoc;

                    cabeceraRow.Valordias = detailResult.Valordias;
                    cabeceraRow.Extension = detailResult.Extension; 
                    cabeceraRow.Total = detailResult.Total;
                    cabeceraRow.CarRegistration = detailResult.CarRegistration;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.GuiaRemision = detailResult.GuiaRemision;
                    cabeceraRow.DuenoTransporte = detailResult.DuenoTransporte;
                    cabeceraRow.CiaFactura = detailResult.CiaFactura;
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.Ruc = detailResult.Ruc;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.TelefonoCia = detailResult.TelefonoCia;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    cabeceraRow.DescripcionGuia = detailResult.DescripcionGuia;
                    cabeceraRow.FleteCanceladoFluvial = detailResult.FleteCanceladoFluvial;
                    cabeceraRow.NumeroFactura = detailResult.NumeroFactura;
                    cabeceraRow.DescripcionLiquidacion = detailResult.DescripcionLiquidacion;


                    rptFletefluvial.CabeceraFleteFluvial.AddCabeceraFleteFluvialRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptFletefluvial;
        }



        private static Stream SetDataReport(FleteFluvialDataSet rptFleteTerrestreDataSet)
        {
            using (var report = new RptFleteFluvial())
            {
                report.SetDataSource(rptFleteTerrestreDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }


    }
}
