using BibliotecaReporte.Dataset.FacturasExterior;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.FacturasExterior;
using BibliotecaReporte.Reportes.FacturasExterior;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.FacturasExterior
{
    internal class NoomWoodAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_NomWoodCR";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static NoomWoodDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptNoonWood = new NoomWoodDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<NoomWoodModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptNoonWood.CabeceraNoonWood.NewCabeceraNoonWoodRow();
                    cabeceraRow.ID = detailResult.ID;
                    cabeceraRow.Contenedores = detailResult.Contenedores;
                    cabeceraRow.FechaEmisiON = detailResult.FechaEmisiON;
                    cabeceraRow.TotalCartones = detailResult.TotalCartones;
                    cabeceraRow.Buyer = detailResult.Buyer;
                    cabeceraRow.PuertoDeEmbaruqe = detailResult.PuertoDeEmbaruqe;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.PaisDestino = detailResult.PaisDestino;
                    cabeceraRow.GlaseoKilos = detailResult.GlaseoKilos;
                    cabeceraRow.Marcas = detailResult.Marcas;
                    cabeceraRow.ProductProforma = detailResult.ProductProforma;
                    cabeceraRow.FechaETD = detailResult.FechaETD;
                    cabeceraRow.PesoNeto = detailResult.PesoNeto;
                    cabeceraRow.Logo = detailResult.Logo;
                   

                    rptNoonWood.CabeceraNoonWood.AddCabeceraNoonWoodRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptNoonWood;
        }

        private static Stream SetDataReport(NoomWoodDataSet rptNoonWoodDataSet)
        {
            using (var report = new RptNoomWood())
            {
                report.SetDataSource(rptNoonWoodDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}