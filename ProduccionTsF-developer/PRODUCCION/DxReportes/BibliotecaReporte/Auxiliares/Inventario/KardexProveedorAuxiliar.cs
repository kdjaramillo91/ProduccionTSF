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
   internal class KardexProveedorAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_KardexProveedor";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static KardexProveedorDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptKardexProveedor = new KardexProveedorDataSet       ();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<KardexProveedorModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptKardexProveedor.CabeceraKardexProveedor.NewCabeceraKardexProveedorRow();
                    cabeceraRow.FechaInicio = detailResult.FechaInicio;
                    cabeceraRow.FechaFin = detailResult.FechaFin;
                    cabeceraRow.NombreBodega = detailResult.NombreBodega;
                    cabeceraRow.NombreUbicacion = detailResult.NombreUbicacion;                    
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;                  
                    cabeceraRow.NombreMotivoInventario = detailResult.NombreMotivoInventario;                   
                    cabeceraRow.MontoEntrada = detailResult.MontoEntrada;                    
                    cabeceraRow.MontoSalida = detailResult.MontoSalida;
                    cabeceraRow.NameCompania = detailResult.NameCompania;
                    cabeceraRow.NameDivision = detailResult.NameDivision;
                    cabeceraRow.NameBranchOffice = detailResult.NameBranchOffice;
                    cabeceraRow.NumberRemissionGuide = detailResult.NumberRemissionGuide;
                    rptKardexProveedor.CabeceraKardexProveedor.AddCabeceraKardexProveedorRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptKardexProveedor;
        }
        private static Stream SetDataReport(KardexProveedorDataSet rptKardexProveedorDataSet)
        {
            using (var report = new RptKardexProveedor())
            {
                report.SetDataSource(rptKardexProveedorDataSet);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();
                return streamReport;
            }
        }
    }
}
