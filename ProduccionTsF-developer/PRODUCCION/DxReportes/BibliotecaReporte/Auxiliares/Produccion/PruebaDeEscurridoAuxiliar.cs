using BibliotecaReporte.Dataset;
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
    internal class PruebaDeEscurridoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_PruebaDeEscurrido";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptPruebaEscurrido = new PruebaDeEscurridoDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<PruebaDeEscurridoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptPruebaEscurrido.CabeceraPruebaDeEscurrido.NewCabeceraPruebaDeEscurridoRow();
                    cabeceraRow.DateDraining = detailResult.DateDraining;
                    cabeceraRow.NameProviderShrimp = detailResult.NameProviderShrimp;
                    cabeceraRow.PoolNumber = detailResult.PoolNumber;
                    cabeceraRow.LotNumber = detailResult.LotNumber;
                    cabeceraRow.LotSystem = detailResult.LotSystem;
                    cabeceraRow.PoundsRemitted = detailResult.PoundsRemitted;
                    cabeceraRow.DrawersNumbers = detailResult.DrawersNumbers;
                    cabeceraRow.GuideRemission = detailResult.GuideRemission;
                    cabeceraRow.PoundsProyected = detailResult.PoundsProyected;
                    cabeceraRow.FullNameBusisnessName = detailResult.FullNameBusisnessName;
                    cabeceraRow.Sampling = detailResult.Sampling;
                    cabeceraRow.ColumnWeight1 = detailResult.ColumnWeight1;
                    cabeceraRow.PoundsAverage1 = detailResult.PoundsAverage1;
                    cabeceraRow.ColumnWeight2 = detailResult.ColumnWeight2;
                    cabeceraRow.PoundsAverage2 = detailResult.PoundsAverage2;
                    cabeceraRow.ColumnWeight3 = detailResult.ColumnWeight3;
                    cabeceraRow.PoundsAverage3 = detailResult.PoundsAverage3;
                    cabeceraRow.ColumnWeight4 = detailResult.ColumnWeight4;
                    cabeceraRow.PoundsAverage4 = detailResult.PoundsAverage4;
                    cabeceraRow.ColumnWeight5 = detailResult.ColumnWeight5;
                    cabeceraRow.PoundsAverage5 = detailResult.PoundsAverage5;
                    cabeceraRow.ProcessPlant = detailResult.ProcessPlant;
                    

                    rptPruebaEscurrido.CabeceraPruebaDeEscurrido.AddCabeceraPruebaDeEscurridoRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptPruebaEscurrido,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptPruebaDeEscurrido())
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