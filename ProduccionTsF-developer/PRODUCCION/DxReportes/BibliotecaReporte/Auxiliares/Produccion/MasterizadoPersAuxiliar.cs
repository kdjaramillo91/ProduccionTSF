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
    internal class MasterizadoPersAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_MasterizadoPers";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static MasterizadoPersDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMasterizadoPers = new MasterizadoPersDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MasterizadoPersModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMasterizadoPers.CabeceraMasterpers.NewCabeceraMasterpersRow();
                    cabeceraRow.FECHAEMISION = detailResult.FECHAEMISION;
                    cabeceraRow.NDOCUMENTO = detailResult.NDOCUMENTO;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.TALLA1 = detailResult.TALLA1;
                    cabeceraRow.MARCA1 = detailResult.MARCA1;
                    cabeceraRow.BODEGA1 = detailResult.BODEGA1;
                    cabeceraRow.BODEGA2 = detailResult.BODEGA2;
                    cabeceraRow.Cantidad2 = detailResult.Cantidad2;
                    cabeceraRow.Cantidad3 = detailResult.Cantidad3;
                    cabeceraRow.CLIENTEPers = detailResult.CLIENTEPers;
                    cabeceraRow.PRESENTACION2 = detailResult.PRESENTACION2;
                    cabeceraRow.Lotepers = detailResult.Lotepers;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    cabeceraRow.Lote1 = detailResult.Lote1;
                    

                    rptMasterizadoPers.CabeceraMasterpers.AddCabeceraMasterpersRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptMasterizadoPers;
        }

        private static Stream SetDataReport(MasterizadoPersDataSet rptMasterizadoPersDataSet)
        {
            using (var report = new RptMasterizadoPers())
            {
                report.SetDataSource(rptMasterizadoPersDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}