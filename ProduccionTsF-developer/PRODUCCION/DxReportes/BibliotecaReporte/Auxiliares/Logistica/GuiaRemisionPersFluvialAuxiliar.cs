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
    internal class GuiaRemisionPersFluvialAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_GuiaRemisionPersFluvial";
        private const string m_StoredProcedureNameSub = "spPar_GuiaRemisionMatDespFluvial";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }


        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptGuiaRemisionPersFluvial = new GuiaRemisionPersFluvialDataSet();
            var rptMaterialDespacho = new MaterialDespachoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<GuiaRemisionPersFluvialModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptGuiaRemisionPersFluvial.CabeceraGuiaRemisionPersFluvial.NewCabeceraGuiaRemisionPersFluvialRow();
                    cabeceraRow.NumeroDocumento = detailResult.NumeroDocumento;
                    cabeceraRow.ProveedorNombre = detailResult.ProveedorNombre;
                    cabeceraRow.DiaEmision = detailResult.DiaEmision;
                    cabeceraRow.MesEmision = detailResult.MesEmision;
                    cabeceraRow.AnioEmision = detailResult.AnioEmision;
                    cabeceraRow.SitioCompleto = detailResult.SitioCompleto;
                    cabeceraRow.NombreZona = detailResult.NombreZona;
                    cabeceraRow.NombreConductor = detailResult.NombreConductor;
                    cabeceraRow.IdConductor = detailResult.IdConductor;
                    cabeceraRow.NombreColor = detailResult.NombreColor;
                    cabeceraRow.PlacaVehiculo = detailResult.PlacaVehiculo;
                    cabeceraRow.SelloSalida = detailResult.SelloSalida;
                    cabeceraRow.SelloEntrada = detailResult.SelloEntrada;
                    cabeceraRow.NombreSeguridad = detailResult.NombreSeguridad;
                    cabeceraRow.TipoProceso = detailResult.TipoProceso;
                    cabeceraRow.NombreBiologo = detailResult.NombreBiologo;
                    cabeceraRow.DescripcionDocumento = detailResult.DescripcionDocumento;
                    cabeceraRow.LibrasProgramadas = detailResult.LibrasProgramadas;
                    cabeceraRow.TipoProceso = detailResult.TipoProceso;
                    cabeceraRow.ClaveAcceso = detailResult.ClaveAcceso;
                    cabeceraRow.ProveedorAmparante = detailResult.ProveedorAmparante;
                    cabeceraRow.INP = detailResult.INP;
                    cabeceraRow.FechaDespacho = detailResult.FechaDespacho;
                    cabeceraRow.HoraDespacho = detailResult.HoraDespacho;
                    cabeceraRow.DescripcionTransporte = detailResult.DescripcionTransporte;
                    rptGuiaRemisionPersFluvial.CabeceraGuiaRemisionPersFluvial.AddCabeceraGuiaRemisionPersFluvialRow(cabeceraRow);
                }

                var subDetail = db.Query<MaterialDespachoModel>(m_StoredProcedureNameSub, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in subDetail)
                {
                    var cabeceraRow = rptMaterialDespacho.MaterialDespacho.NewMaterialDespachoRow();
                    cabeceraRow.NombreMatDesp = detailResult.NombreMatDesp;
                    cabeceraRow.CantidadMatDesp = detailResult.CantidadMatDesp;
                    rptMaterialDespacho.MaterialDespacho.AddMaterialDespachoRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptGuiaRemisionPersFluvial,
                rptMaterialDespacho,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptGuiaRemisionPersFluvial())
            {
                report.SetDataSource(dataSet[0]);
                //SubReporte
                report.OpenSubreport("RptMatDespacho.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }


    }
}
