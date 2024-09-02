using BibliotecaReporte.Dataset.Produccion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Produccion;
using BibliotecaReporte.Reportes.Produccion;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.Produccion
{
   internal  class MasterizadoAuxiliar
    {

        private const string m_StoredProcedureName = "spPar_Masterizado";
        private const string m_StoredProcedureNameSub = "spPar_MasterizadoMovInventario";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptCierreTurnoTemporal = new MasterizadoDataSet();
            var rptMasterizadoMovInventarioTerminado = new MasterizadoMovInventarioDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MasterizadoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptCierreTurnoTemporal.CabeceraMasterizado.NewCabeceraMasterizadoRow();
                    cabeceraRow.FECHAEMISION = detailResult.FECHAEMISION;
                    cabeceraRow.FECHAINICIO = detailResult.FECHAINICIO;
                    cabeceraRow.FECHAFIN = detailResult.FECHAFIN;
                    cabeceraRow.NDOCUMENTO = detailResult.NDOCUMENTO;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.ESTADO = detailResult.ESTADO;
                    cabeceraRow.RESPONSABLE = detailResult.RESPONSABLE;
                    cabeceraRow.TURNO = detailResult.TURNO;
                    cabeceraRow.INLOTE = detailResult.INLOTE;
                    cabeceraRow.CODIGO1 = detailResult.CODIGO1;
                    cabeceraRow.NPRODUCTO1 = detailResult.NPRODUCTO1;
                    cabeceraRow.TALLA1 = detailResult.TALLA1;
                    cabeceraRow.MARCA1 = detailResult.MARCA1;
                    cabeceraRow.BODEGA1 = detailResult.BODEGA1;
                    cabeceraRow.Cantidad1 = detailResult.Cantidad1;
                    cabeceraRow.PRESENTACION1 = detailResult.PRESENTACION1; 
                    cabeceraRow.LBSOKG1 = detailResult.LBSOKG1;
                    cabeceraRow.Kgolb1 = detailResult.Kgolb1;
                    cabeceraRow.SecEgreso = detailResult.SecEgreso;
                    cabeceraRow.CODIGO2 = detailResult.CODIGO2;
                    cabeceraRow.NPRODUCTO2 = detailResult.NPRODUCTO2;
                    cabeceraRow.TALLA2 = detailResult.TALLA2;
                    cabeceraRow.MARCA2 = detailResult.MARCA2;
                    cabeceraRow.BODEGA2 = detailResult.BODEGA2;
                    cabeceraRow.OP= detailResult.OP;
                    cabeceraRow.CLIENTE = detailResult.CLIENTE;
                    cabeceraRow.PRESENTACION2 = detailResult.PRESENTACION2;
                    cabeceraRow.KILOS2 = detailResult.KILOS2;
                    cabeceraRow.LIBRAS2 = detailResult.LIBRAS2;
                    cabeceraRow.LBSOKG2 = detailResult.LBSOKG2;
                    cabeceraRow.Kgolb3 = detailResult.Kgolb3;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    cabeceraRow.Cantidad2 = detailResult.Cantidad2;
                    cabeceraRow.LoteMP = detailResult.LoteMP;
                    rptCierreTurnoTemporal.CabeceraMasterizado.AddCabeceraMasterizadoRow(cabeceraRow);
                }

                var subDetail = db.Query<MasterizadoMovInventarioModel>(m_StoredProcedureNameSub, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in subDetail)
                {
                    var cabeceraRow = rptMasterizadoMovInventarioTerminado.MasterizadoMovInventario.NewMasterizadoMovInventarioRow();
                    cabeceraRow.IdLote = detailResult.IdLote;
                    cabeceraRow.Codigo2 = detailResult.Codigo2;
                    cabeceraRow.Producto2 = detailResult.Producto2;
                    cabeceraRow.Talla2 = detailResult.Talla2;
                    cabeceraRow.Marca2 = detailResult.Marca2;
                    cabeceraRow.Bodega2 = detailResult.Bodega2;
                    cabeceraRow.Ubicacion2 = detailResult.Ubicacion2;
                    cabeceraRow.Cantidad2 = detailResult.Cantidad2;
                    cabeceraRow.OPCliente = detailResult.OPCliente;
                    cabeceraRow.Cliente = detailResult.Cliente;
                    cabeceraRow.Presentacion2 = detailResult.Presentacion2;
                    cabeceraRow.LbsoKg2 = detailResult.LbsoKg2;
                    cabeceraRow.KgoLb2 = detailResult.KgoLb2;
                    cabeceraRow.SecMaster = detailResult.SecMaster;
                    cabeceraRow.Codigo1 = detailResult.Codigo1;
                    cabeceraRow.Producto1 = detailResult.Producto1;
                    cabeceraRow.Talla1 = detailResult.Talla1;
                    cabeceraRow.Cantidad1 = detailResult.Cantidad1;
                    cabeceraRow.Bodega3 = detailResult.Bodega3;
                    cabeceraRow.Ubicacion3 = detailResult.Ubicacion3;
                    cabeceraRow.Cantidad3 = detailResult.Cantidad3;
                    cabeceraRow.Presentacion1 = detailResult.Presentacion1;
                    cabeceraRow.Kilo1 = detailResult.Kilo1;
                    cabeceraRow.Libras1 = detailResult.Libras1;                    
                    cabeceraRow.LbsoKg3 = detailResult.LbsoKg3;
                    cabeceraRow.KgoLb3 = detailResult.KgoLb3;
                    cabeceraRow.SecCajas = detailResult.SecCajas;
                    // Asignamos a los dos DATASET'S pues recogen la misma info a los 2 subreportes
                    rptMasterizadoMovInventarioTerminado.MasterizadoMovInventario.AddMasterizadoMovInventarioRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptCierreTurnoTemporal,
                rptMasterizadoMovInventarioTerminado,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptMasterizado())
            {
                // report.FileName = "C:/Users/PROGRA~1/AppData/Local/Temp";
                report.SetDataSource(dataSet[0]);
                //SubReporte
                report.OpenSubreport("RptMasterizadoTerminado.rpt").SetDataSource(dataSet[1]);
                report.OpenSubreport("RptMasterizadoCajasSueltas.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
