using BibliotecaReporte.Dataset;
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
    internal class MovimientoInventarioAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_MovimientoInventario";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMovimientoInventario = new MovimientoInventarioDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MovimientoInventarioModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMovimientoInventario.CabeceraMovInventario.NewCabeceraMovInventarioRow();
                    cabeceraRow.TituloReporte = detailResult.TituloReporte;
                    cabeceraRow.Bodega = detailResult.Bodega;
                    cabeceraRow.Motivo = detailResult.Motivo;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.CodigoProducto = detailResult.CodigoProducto;
                    cabeceraRow.DescripcionProducto = detailResult.DescripcionProducto;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.NumeroSecuencia = detailResult.NumeroSecuencia;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.CodigoNaturaleza = detailResult.CodigoNaturaleza;
                    cabeceraRow.SecuenciaGuiaRemision = detailResult.SecuenciaGuiaRemision;
                    cabeceraRow.SecuenciaRequisicion = detailResult.SecuenciaRequisicion;                    
                    cabeceraRow.SecuenciaLiquidacionMateriales = detailResult.SecuenciaLiquidacionMateriales;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.numLote = detailResult.NumLote;
                    cabeceraRow.SecTransac = detailResult.SecTransac;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.itemTalla = detailResult.ItemTalla;
                    cabeceraRow.tipoItem = detailResult.TipoItem;
                    cabeceraRow.nombreUsuario = detailResult.NombreUsuario;
                    cabeceraRow.Libras = detailResult.Libras;
                    cabeceraRow.Kilos = detailResult.Kilos;


                    rptMovimientoInventario.CabeceraMovInventario.AddCabeceraMovInventarioRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptMovimientoInventario,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptDataSet)
        {
            using (var report = new RptMovimientoInventario())
            {
                report.SetDataSource(rptDataSet[0]);

                // Subreporte (Cia's) 
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }
    }
}
