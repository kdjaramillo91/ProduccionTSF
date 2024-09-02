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
   internal class GuiaRideAuxiliar
    {
        private const string m_StoredProcedureName = "spParGuiaRIDE";
        private const string m_StoredProcedureNameSub = "spPar_GuiaRemisionDetalleRIDE";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptGuiaRide = new GuiaRideDataSet();
            var rptGuiaRemisionDetalleRIDE = new GuiaRemisionDetalleRIDEDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<GuiaRideModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptGuiaRide.CabeceraGuiaRide.NewCabeceraGuiaRideRow();
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Compania = detailResult.Compania;
                    cabeceraRow.Direccion = detailResult.Direccion;
                    cabeceraRow.Sucursal = detailResult.Sucursal;
                    cabeceraRow.ContribuyenteEspecial = detailResult.ContribuyenteEspecial;
                    cabeceraRow.Ruc = detailResult.Ruc;
                    cabeceraRow.NumeroDocumento = detailResult.NumeroDocumento;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.ProveedorId = detailResult.ProveedorId;
                    cabeceraRow.ProveedorNombre = detailResult.ProveedorNombre;
                    cabeceraRow.NombreConductor = detailResult.NombreConductor;
                    cabeceraRow.IdConductor = detailResult.IdConductor;
                    cabeceraRow.PlacaVehiculo = detailResult.PlacaVehiculo;
                    cabeceraRow.ClaveAcceso = detailResult.ClaveAcceso;
                    cabeceraRow.Razon = detailResult.Razon;
                    cabeceraRow.PuntoPartida = detailResult.PuntoPartida;
                    cabeceraRow.Destino = detailResult.Destino;
                    cabeceraRow.TipoDocumento = detailResult.TipoDocumento;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    rptGuiaRide.CabeceraGuiaRide.AddCabeceraGuiaRideRow(cabeceraRow);
                }

                var subDetail = db.Query<GuiaRemisionDetalleRIDEModel>(m_StoredProcedureNameSub, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in subDetail)
                {
                    var cabeceraRow = rptGuiaRemisionDetalleRIDE.GuiaRemisionDetalleRIDE.NewGuiaRemisionDetalleRIDERow();
                    cabeceraRow.Producto = detailResult.Producto;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.Codigo = detailResult.Codigo;
                    cabeceraRow.Auxiliar = detailResult.Auxiliar;
                    rptGuiaRemisionDetalleRIDE.GuiaRemisionDetalleRIDE.AddGuiaRemisionDetalleRIDERow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataset = new DataSet[]
            {
                rptGuiaRide,
                rptGuiaRemisionDetalleRIDE,
            };

            return dataset;
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptGuiaRide())
            {
                report.SetDataSource(dataSet[0]);
                //SubReporte
                report.OpenSubreport("RptGuiaRemisionDetalleRIDE.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
