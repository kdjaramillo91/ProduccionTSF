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
    internal class MateriaPrimaPendienteAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_MateriaPrimaPendiente";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptRecepMateiraPrimaPend = new RecepMateriaPrimaPendienteDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }
                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MateriaPrimaPendienteModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptRecepMateiraPrimaPend.CabeceraRecepMateriaPrimaPendiente.NewCabeceraRecepMateriaPrimaPendienteRow();
                    cabeceraRow.PL_number = detailResult.PL_number;
                    cabeceraRow.PL_internalNumber = detailResult.PL_internalNumber;
                    cabeceraRow.PL_receptionDate = detailResult.PL_receptionDate;
                    cabeceraRow.lbsremitidas = detailResult.lbsremitidas;
                    cabeceraRow.NamePool = detailResult.NamePool;
                    cabeceraRow.Nameproveedor = detailResult.Nameproveedor;
                    cabeceraRow.RemissionGuide = detailResult.RemissionGuide;
                    cabeceraRow.Name_cia = detailResult.Name_cia;
                    cabeceraRow.Ruc_cia = detailResult.Ruc_cia;
                    cabeceraRow.Telephone_cia = detailResult.Telephone_cia;
                    cabeceraRow.Rendimiento = detailResult.Rendimiento;
                    cabeceraRow.Lbsrecibidas = detailResult.Lbsrecibidas;
                    cabeceraRow.Gramagentero = detailResult.Gramagentero;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.Producto = detailResult.Producto;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.Estadolote = detailResult.Estadolote;
                    cabeceraRow.ProcesoPlanta = detailResult.ProcesoPlanta;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    

                    rptRecepMateiraPrimaPend.CabeceraRecepMateriaPrimaPendiente.AddCabeceraRecepMateriaPrimaPendienteRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptRecepMateiraPrimaPend,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptMateriaPrimaPendiente())
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