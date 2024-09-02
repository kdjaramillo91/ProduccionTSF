using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.Recepcion;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Recepcion;
using BibliotecaReporte.Reportes.Recepcion;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.Recepcion
{
    internal class LibrasLiquidadasAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LibrasLiquidadas";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptLibrasLiquidas = new LibrasLiquidadasDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LibrasLiquidadasModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptLibrasLiquidas.CabeceraLibrasLiquidadas.NewCabeceraLibrasLiquidadasRow();
                    cabeceraRow.PL_number = detailResult.PL_number;
                    cabeceraRow.PL_internalNumber = detailResult.PL_internalNumber;
                    cabeceraRow.PL_receptionDate = detailResult.PL_receptionDate;
                    cabeceraRow.lbsprocesadas = detailResult.lbsprocesadas;
                    cabeceraRow.lbsrecibidas = detailResult.lbsrecibidas;
                    cabeceraRow.lbsremitidas = detailResult.lbsremitidas;
                    cabeceraRow.lbsentero = detailResult.lbsentero;
                    cabeceraRow.lbscola = detailResult.lbscola;
                    cabeceraRow.basuraentero = detailResult.basuraentero;
                    cabeceraRow.basuracola = detailResult.basuracola;
                    cabeceraRow.NamePool = detailResult.NamePool;
                    cabeceraRow.Nameproveedor = detailResult.Nameproveedor;
                    cabeceraRow.ProductionUnitProviderName = detailResult.ProductionUnitProviderName;
                    cabeceraRow.name_cia = detailResult.name_cia;
                    cabeceraRow.RucCia = detailResult.RucCia;
                    cabeceraRow.telephone_cia = detailResult.telephone_cia;
                    cabeceraRow.proceso_ = detailResult.proceso;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    cabeceraRow.sequentialLiquidation = detailResult.sequentialLiquidation;
                    cabeceraRow.ProcesoPlanta = detailResult.ProcesoPlanta;

                    rptLibrasLiquidas.CabeceraLibrasLiquidadas.AddCabeceraLibrasLiquidadasRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptLibrasLiquidas,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptLibrasLiquidadas())
            {
                report.SetDataSource(dataSet[0]);

                // Subreporte
                report.OpenSubreport("RptLogo.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}