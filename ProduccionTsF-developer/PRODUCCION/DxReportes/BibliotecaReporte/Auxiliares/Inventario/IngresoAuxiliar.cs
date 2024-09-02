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
    internal class IngresoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_Ingreso";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static IngresoDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptIngreso = new IngresoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<IngresoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptIngreso.CabeceraIngreso.NewCabeceraIngresoRow();
                    cabeceraRow.ID = detailResult.ID;
                    cabeceraRow.TituloReporte = detailResult.TituloReporte;
                    cabeceraRow.Bodega = detailResult.Bodega;
                    cabeceraRow.Motivo = detailResult.Motivo;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.CodigoProducto = detailResult.CodigoProducto;
                    cabeceraRow.DescripcionProducto = detailResult.DescripcionProducto;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.enLibras = detailResult.enLibras;
                    cabeceraRow.enKilos = detailResult.enKilos;
                    cabeceraRow.NumeroSecuencia = detailResult.NumeroSecuencia;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;
                    cabeceraRow.CentroCosto = detailResult.CentroCosto;
                    cabeceraRow.SubCentroCosto = detailResult.SubCentroCosto;
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
                    cabeceraRow.Compania = detailResult.Compania;
                    cabeceraRow.NumLoteP = detailResult.NumLoteP;

                    rptIngreso.CabeceraIngreso.AddCabeceraIngresoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptIngreso;
        }

        private static Stream SetDataReport(IngresoDataSet rptIngresoDataSet)
        {
            using (var report = new RptIngreso())
            {
                report.SetDataSource(rptIngresoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}