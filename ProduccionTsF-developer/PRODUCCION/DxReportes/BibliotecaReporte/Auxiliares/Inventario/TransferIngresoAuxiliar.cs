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
    internal class TransferIngresoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_TransferIngreso";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptIngreso = new TransferIngresoDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<TransferenciaIngresoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptIngreso.CabeceraTransferenciaIngreso.NewCabeceraTransferenciaIngresoRow();
                    cabeceraRow.ID = detailResult.ID;

                    cabeceraRow.Bodega = detailResult.Bodega;
                    cabeceraRow.Motivo = detailResult.Motivo;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.CodigoProducto = detailResult.CodigoProducto;
                    cabeceraRow.DescripcionProducto = detailResult.DescripcionProducto;
                    cabeceraRow.Cantidad = detailResult.Cantidad;

                    cabeceraRow.NumeroSecuencia = detailResult.NumeroSecuencia;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;

                    cabeceraRow.CodigoNaturaleza = detailResult.CodigoNaturaleza;
                    cabeceraRow.SecuenciaRequisicion = detailResult.SecuenciaRequisicion;
                    cabeceraRow.SecuenciaLiquidacionMateriales = detailResult.SecuenciaLiquidacionMateriales;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.numLote = detailResult.numLote;
                    cabeceraRow.SecTransac = detailResult.SecTransac;
                    cabeceraRow.EstadoDocumento = detailResult.EstadoDocumento;
                    cabeceraRow.itemTalla = detailResult.itemTalla;
                    cabeceraRow.tipoItem = detailResult.tipoItem;
                    cabeceraRow.nombreUsuario = detailResult.nombreUsuario;
                    cabeceraRow.bodegaEgreso = detailResult.bodegaEgreso;
                    cabeceraRow.UbicacionEgreso = detailResult.UbicacionEgreso;
                    cabeceraRow.numeroEgreso = detailResult.numeroEgreso;
                    cabeceraRow.Libras = detailResult.Libras;
                    cabeceraRow.Kilos = detailResult.kilos;
                    rptIngreso.CabeceraTransferenciaIngreso.AddCabeceraTransferenciaIngresoRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptIngreso,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptTransferingresoDataSet)
        {
            using (var report = new RptTransferIngreso())
            {
                report.SetDataSource(rptTransferingresoDataSet[0]);

                // Subreporte Logo
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptTransferingresoDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}