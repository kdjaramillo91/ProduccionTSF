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
    internal class RecepcionAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_Recepcion";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptRecepcion = new RecepcionDataset();
            var rptCompaniaInfo = new CompaniaInfoDataSet();
            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<RecepcionModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptRecepcion.CabeceraRecepcion.NewCabeceraRecepcionRow();
                    cabeceraRow.PL_id = detailResult.PL_id;
                    cabeceraRow.PL_number = detailResult.PL_number;
                    cabeceraRow.PL_internalNumber = detailResult.PL_internalNumber;
                    cabeceraRow.PL_recepctionDate = detailResult.PL_recepctionDate;
                    cabeceraRow.lbsremitidas = detailResult.lbsremitidas;
                    cabeceraRow.NamePool = detailResult.NamePool;
                    cabeceraRow.Nameproveedor = detailResult.Nameproveedor;
                    cabeceraRow.ProductionUnitProviderName = detailResult.ProductionUnitProviderName;
                    cabeceraRow.RemissionGuide = detailResult.RemissionGuide;
                    cabeceraRow.name_cia = detailResult.name_cia;
                    cabeceraRow.ruc_cia = detailResult.ruc_cia;
                    cabeceraRow.telephone_cia = detailResult.telephone_cia;
                    cabeceraRow.lbsrecibidas = detailResult.lbsrecibidas;
                    cabeceraRow.proceso = detailResult.proceso;
                    cabeceraRow.lbsrecibent = detailResult.lbsrecibent;
                    cabeceraRow.lbsrecibcola = detailResult.lbsrecibcola;
                    cabeceraRow.gramagentero = detailResult.gramagentero;
                    cabeceraRow.gramagcola = detailResult.gramagcola;
                    cabeceraRow.rentero = detailResult.rentero;
                    cabeceraRow.rentcola = detailResult.rentcola;
                    cabeceraRow.Inp = detailResult.Inp;
                    cabeceraRow.AcuerdoTramite = detailResult.AcuerdoTramite;
                    cabeceraRow.ProcesoPlanta = detailResult.ProcesoPlanta;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    rptRecepcion.CabeceraRecepcion.AddCabeceraRecepcionRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);
                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
           {
                rptRecepcion,
                rptCompaniaInfo,
           };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptRecepcionDataSet)
        {
            using (var report = new RptRecepcion())
            {
                report.SetDataSource(rptRecepcionDataSet[0]);

                // SubReporte
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptRecepcionDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}