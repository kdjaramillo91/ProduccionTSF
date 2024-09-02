using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.Procesos;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Procesos;
using BibliotecaReporte.Reportes.Procesos;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Proceso
{
    internal class ProcesoInternoResumenAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ProcesosInternoResumen";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptProcesoInternoResumen = new ProcesoInternoResumenDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();


            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);

                var detailsResult = db.Query<ProcesoInternoResumenModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptProcesoInternoResumen.CabeceraProcesoInternoResumen.NewCabeceraProcesoInternoResumenRow();
                    cabeceraRow.PRODUNnameNombreUnidadLoteProduccion = detailResult.PRODUNnameNombreUnidadLoteProduccion;
                    cabeceraRow.PRODPRnameProcesoLoteProduccion = detailResult.PRODPRnameProcesoLoteProduccion;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.Suma = detailResult.Suma;
                    cabeceraRow.Sumaliquidacion = detailResult.Sumaliquidacion;
                    cabeceraRow.Sumadesperdicio = detailResult.Sumadesperdicio;
                    cabeceraRow.Sumamerma = detailResult.Sumamerma;
                    cabeceraRow.LibsDetalle = detailResult.LibsDetalle;
                    cabeceraRow.Libsliquidacion = detailResult.Libsliquidacion;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    
                   
                    rptProcesoInternoResumen.CabeceraProcesoInternoResumen.AddCabeceraProcesoInternoResumenRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
           {
                rptProcesoInternoResumen,
                rptCompaniaInfo,
           };

            return dataSet;            
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptProcesoInternoResumen())
            {
                report.SetDataSource(dataSet[0]);

                report.OpenSubreport("RptLogo.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}