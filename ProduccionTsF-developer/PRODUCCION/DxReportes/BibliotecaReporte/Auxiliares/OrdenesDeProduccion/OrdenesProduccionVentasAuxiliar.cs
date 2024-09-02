using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.OrdenesDeProduccion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.OrdenesDeProduccion;
using BibliotecaReporte.Reportes.OrdenesDeProduccion;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.OrdenesDeProduccion
{
    internal class OrdenesProduccionVentasAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_OrdenProduccionReport";
        private const string m_StoredProcedureNameInstrucion = "spPar_OrdenProduccionInstrucciones";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptOrdenProduccionVentas = new OrdenProduccionVentasDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<OrdenProduccionVentasModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptOrdenProduccionVentas.CabeceraOrdenProduccionVentas.NewCabeceraOrdenProduccionVentasRow();
                    cabeceraRow.TipoDocumento = detailResult.TipoDocumento;
                    cabeceraRow.NumeroDocumento = detailResult.NumeroDocumento;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.Solicitante = detailResult.Solicitante;
                    cabeceraRow.Cliente = detailResult.Cliente;
                    cabeceraRow.NumeroProforma = detailResult.NumeroProforma;
                    cabeceraRow.FormadePago = detailResult.FormaDePago;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.Destino = detailResult.Destino;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.Codigo = detailResult.Codigo;
                    cabeceraRow.DescripcionDetalle = detailResult.DescripcionDetalle;
                    cabeceraRow.Marca = detailResult.Marca;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.Empaque = detailResult.Empaque;
                    cabeceraRow.Cartones = detailResult.Cartones;
                    cabeceraRow.Kilos = detailResult.Kilos;
                    cabeceraRow.Libras = detailResult.Libras;
                    cabeceraRow.VentaUsuario = detailResult.VentaUsuario;
                    cabeceraRow.DirectorVenta = detailResult.DirectorVenta;
                    cabeceraRow.NumeroLote = detailResult.NumeroLote;
                    cabeceraRow.Motivo = detailResult.Motivo;
                    cabeceraRow.Instrucciones = detailResult.Instrucciones;
                    cabeceraRow.RucProveedor = detailResult.RucProveedor;
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.TelefonoProveedor = detailResult.TelefonoProveedor;
                    cabeceraRow.DireccionProveedor = detailResult.DireccionProveedor;
                    cabeceraRow.CodigoPlanta = detailResult.CodigoPlanta;
                    cabeceraRow.FDA = detailResult.FDA;

                    rptOrdenProduccionVentas.CabeceraOrdenProduccionVentas.AddCabeceraOrdenProduccionVentasRow(cabeceraRow);
                }

                // Busqueda y recorrido de detalles
                var subDetailsResult = db.Query<OrdenProduccionVentasModel.Detalles>(m_StoredProcedureNameInstrucion, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var subDetailResult in subDetailsResult)
                {
                    var detalleRow = rptOrdenProduccionVentas.DetalleOrdenProduccionVentas.NewDetalleOrdenProduccionVentasRow();
                    detalleRow.Document = subDetailResult.Document;
                    detalleRow.Copy = subDetailResult.Copy;
                    detalleRow.Digital = subDetailResult.Digital;

                    rptOrdenProduccionVentas.DetalleOrdenProduccionVentas.AddDetalleOrdenProduccionVentasRow(detalleRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptOrdenProduccionVentas,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptOrdenProduccionVentas)
        {
            using (var report = new RptOrdenProduccionVentas())
            {
                report.SetDataSource(rptOrdenProduccionVentas[0]);

                // Subreporte
                report.OpenSubreport("OrdenProduccionInstrucciones").SetDataSource(rptOrdenProduccionVentas[0]);
                // Subreporte Cia's
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptOrdenProduccionVentas[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
