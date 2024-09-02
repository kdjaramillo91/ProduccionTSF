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
    internal class LibrasLiquidadasSequencialAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_LibrasLiquidadasSequencial";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptLibrasLiquidadasSequencial = new LibrasLiquidadasSequencialDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }
                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<LibrasLiquidadasSequencialModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptLibrasLiquidadasSequencial.CabeceraLibrasLiquidadasSequencial.NewCabeceraLibrasLiquidadasSequencialRow();
                    cabeceraRow.PL_number = detailResult.PL_number;
                    cabeceraRow.PL_InternalNumber = detailResult.PL_InternalNumber;
                    cabeceraRow.PL_ReceptionDate = detailResult.PL_ReceptionDate;
                    cabeceraRow.Lbsprocesadas = detailResult.Lbsprocesadas;
                    cabeceraRow.Lbsremitidas = detailResult.Lbsremitidas;
                    cabeceraRow.Lbsentero = detailResult.Lbsentero;
                    cabeceraRow.Lbscola = detailResult.Lbscola;
                    cabeceraRow.Basuraentero = detailResult.Basuraentero;
                    cabeceraRow.BasuraCola = detailResult.BasuraCola;
                    cabeceraRow.NamePool = detailResult.NamePool;
                    cabeceraRow.Nameproveedor = detailResult.Nameproveedor;
                    cabeceraRow.Lbsrecibidas = detailResult.Lbsrecibidas;
                    cabeceraRow.Ruc_cia = detailResult.Ruc_cia;
                    cabeceraRow.Telephone_cia = detailResult.Telephone_cia;
                    cabeceraRow.ProcessPlant = detailResult.ProcessPlant;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.SequentialLiquidation = detailResult.SequentialLiquidation;
                    cabeceraRow.Name = detailResult.Name;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    rptLibrasLiquidadasSequencial.CabeceraLibrasLiquidadasSequencial.AddCabeceraLibrasLiquidadasSequencialRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptLibrasLiquidadasSequencial,
                rptCompaniaInfo,
            };

            return dataSet;
        }
        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptLibrasLiquidadasSequencial())
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
