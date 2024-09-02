using BibliotecaReporte.Dataset.Logistica;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Logistica;
using BibliotecaReporte.Reportes.Logistica;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;


namespace BibliotecaReporte.Auxiliares.Logistica
{
    internal class FleteTerrestreAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FleteTerrestre";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }


        private static FleteTerrestreDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFleteTerrestre = new FleteTerrestreDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FleteTerrestreModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFleteTerrestre.CabeceraFleteterrestre.NewCabeceraFleteterrestreRow();
                    cabeceraRow.CarRegistration = detailResult.CarRegistration;
                    cabeceraRow.Flete = detailResult.Flete;
                    cabeceraRow.Anticipo = detailResult.Anticipo;
                    cabeceraRow.Ajuste = detailResult.Ajuste;

                    cabeceraRow.Valordias = detailResult.Valordias;
                    cabeceraRow.Extension = detailResult.Extension;
                    cabeceraRow.Total = detailResult.Total;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.Guiaremision = detailResult.Guiaremision;
                    cabeceraRow.FechaGuiaRemision = detailResult.FechaGuiaRemision;
                    cabeceraRow.DuenoTransporte = detailResult.DuenoTransporte;
                    cabeceraRow.CiaFactura = detailResult.CiaFactura;
                    cabeceraRow.Nombrecia = detailResult.Nombrecia;
                    cabeceraRow.Ruc = detailResult.Ruc;
                    cabeceraRow.Numdoc = detailResult.Numdoc;
                    cabeceraRow.Estadodocumento = detailResult.Estadodocumento;
                    cabeceraRow.Telefonocia = detailResult.Telefonocia;
                    cabeceraRow.FleteCancelado = detailResult.FleteCancelado;
                    cabeceraRow.FleteCanceladoTotal = detailResult.FleteCanceladoTotal;
                    cabeceraRow.DescripcionRG = detailResult.DescripcionRG;
                    cabeceraRow.Chofer = detailResult.Chofer;
                    cabeceraRow.DescripcionGeneral = detailResult.DescripcionGeneral;
                    cabeceraRow.NumeroFactura = detailResult.NumeroFactura;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;

                    rptFleteTerrestre.CabeceraFleteterrestre.AddCabeceraFleteterrestreRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptFleteTerrestre;
        }






        private static Stream SetDataReport(FleteTerrestreDataSet rptFleteTerrestreDataSet)
        {
            using (var report = new RptFleteTerrestre())
            {
                report.SetDataSource(rptFleteTerrestreDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }


    }
}
