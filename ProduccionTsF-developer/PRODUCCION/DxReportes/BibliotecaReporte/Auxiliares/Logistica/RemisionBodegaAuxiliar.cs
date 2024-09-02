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
    internal class RemisionBodegaAuxiliar
    {
            private const string m_StoredProcedureName = "spPar_RequisicionBodega";

            internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
            {
                return SetDataReport(PrepareData(sql, parametros));
            }

            private static RequisicionBodegaDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
            {
                var rptRemisionBodega = new RequisicionBodegaDataSet();

                using (SqlConnection db = sqlConnection)
                {
                    if (db.State == ConnectionState.Closed) { db.Open(); }

                    var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                    var detailsResult = db.Query<RequisicionBodegaModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                        .ToList();

                    foreach (var detailResult in detailsResult)
                    {
                        var cabeceraRow = rptRemisionBodega.CabeceraRequisionBodega.NewCabeceraRequisionBodegaRow();
                        cabeceraRow.Placa = detailResult.Placa;
                        cabeceraRow.Cantidad = detailResult.Cantidad;
                        cabeceraRow.RG_DespachureDate = detailResult.RG_DespachureDate;
                        cabeceraRow.Nameproveedor = detailResult.Nameproveedor;
                        cabeceraRow.CIproveedor = detailResult.CIproveedor;
                        cabeceraRow.Conductorm = detailResult.Conductorm; 
                        cabeceraRow.CIconductor = detailResult.CIconductor; 
                        cabeceraRow.AdressConductor = detailResult.AdressConductor;
                        cabeceraRow.Observatrans = detailResult.Observatrans;
                        cabeceraRow.Logo = detailResult.Logo;
                        cabeceraRow.Logo2 = detailResult.Logo2;
                        cabeceraRow.Documento = detailResult.Documento;
                        cabeceraRow.CodeItem = detailResult.CodeItem;
                        cabeceraRow.NameItem = detailResult.NameItem;
                        cabeceraRow.Unidades = detailResult.Unidades;
                        cabeceraRow.Cantidad_1 = detailResult.Cantidad_1;
                        cabeceraRow.Zonasitioprovedor = detailResult.Zonasitioprovedor;
                        cabeceraRow.Sello1 = detailResult.Sello1;
                        cabeceraRow.Sello2 = detailResult.Sello2;
                        cabeceraRow.Bodegaubicacion = detailResult.Bodegaubicacion;
                        cabeceraRow.DireccionProvee = detailResult.DireccionProvee;
                        cabeceraRow.NumeroRequisicion = detailResult.NumeroRequisicion;
                    rptRemisionBodega.CabeceraRequisionBodega.AddCabeceraRequisionBodegaRow(cabeceraRow);
                    }

                    if (db.State == ConnectionState.Open) { db.Close(); }
                }

                return rptRemisionBodega;
        }
            private static Stream SetDataReport(RequisicionBodegaDataSet rptRequisionBodegaDataSet)
            {
                using (var report = new RptRequisionBodega())
                {
                    report.SetDataSource(rptRequisionBodegaDataSet);

                    var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                    report.Close();

                    return streamReport;
                }
            }        
    }
}