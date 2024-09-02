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
    internal class FactExteriorBLModelAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FactExteriorBL";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static FactExteriorBLDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFactExteriorBL = new FactExteriorBLDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FactExteriorBLModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFactExteriorBL.CabeceraFactExteriorBL.NewCabeceraFactExteriorBLRow();
                    cabeceraRow.Cbl = detailResult.Cbl;
                    cabeceraRow.Id_metricUnit = detailResult.Id_metricUnit;
                    cabeceraRow.Dae = detailResult.Dae;
                    cabeceraRow.PuertoDeEmbaruqe = detailResult.PuertoDeEmbaruqe;
                    cabeceraRow.BusinessName = detailResult.BusinessName;
                    cabeceraRow.Partida = detailResult.Partida;
                    cabeceraRow.Ruc = detailResult.Ruc;
                    cabeceraRow.TelefonoCompania = detailResult.TelefonoCompania;
                    cabeceraRow.Contenedores = detailResult.Contenedores;
                    cabeceraRow.Consignatario = detailResult.Consignatario;
                    cabeceraRow.ConsignatarioTipoIdent = detailResult.ConsignatarioTipoIdent;
                    cabeceraRow.ConsignatarioIdent = detailResult.ConsignatarioIdent;
                    cabeceraRow.ConsignatarioDireccion = detailResult.ConsignatarioDireccion;
                    cabeceraRow.ConsignatarioEmail = detailResult.ConsignatarioEmail;
                    cabeceraRow.ConsignatarioTlFno = detailResult.ConsignatarioTlFno;
                    cabeceraRow.Nbooking = detailResult.Nbooking;
                    cabeceraRow.ReferenciaProforma = detailResult.ReferenciaProforma;
                    cabeceraRow.ExportCarrier = detailResult.ExportCarrier;
                    cabeceraRow.PuertoDescargaBL = detailResult.PuertoDescargaBL;
                    cabeceraRow.ClienteExteriorBl = detailResult.ClienteExteriorBl;
                    cabeceraRow.DireccionCLiexBL = detailResult.DireccionCLiexBL;
                    cabeceraRow.MailCliextBl = detailResult.MailCliextBl;
                    cabeceraRow.TelefonocliexBL = detailResult.TelefonocliexBL;
                    cabeceraRow.SelloBL = detailResult.SelloBL;
                    cabeceraRow.CartonesBL = detailResult.CartonesBL;
                    cabeceraRow.CartonesBL2 = detailResult.CartonesBL2;
                    cabeceraRow.NContendoresBL = detailResult.NContendoresBL;
                    cabeceraRow.NinvoiceBL = detailResult.NinvoiceBL;
                    cabeceraRow.PesonetoKilosBL = detailResult.PesonetoKilosBL;
                    cabeceraRow.PesonetoLibrasBL = detailResult.PesonetoLibrasBL;
                    cabeceraRow.PesoBrutoKilos = detailResult.PesoBrutoKilos;
                    cabeceraRow.PesoBrutoLibras = detailResult.PesoBrutoLibras;
                    cabeceraRow.InstruccionTempbl = detailResult.InstruccionTempbl;
                    cabeceraRow.BlNumber = detailResult.BlNumber;
                    cabeceraRow.FechaeTdBL = detailResult.FechaeTdBL;
                    cabeceraRow.DescripcionPR = detailResult.DescripcionPR;
                    cabeceraRow.DescripcionBL = detailResult.DescripcionBL;
                    cabeceraRow.PO = detailResult.PO;
                    cabeceraRow.Ncontrato = detailResult.Ncontrato;
                    cabeceraRow.ObservacionBL = detailResult.ObservacionBL;
                    cabeceraRow.Transport = detailResult.Transport;
                    cabeceraRow.Proforma = detailResult.Proforma;
                    cabeceraRow.DirMatriz = detailResult.DirMatriz;

                    rptFactExteriorBL.CabeceraFactExteriorBL.AddCabeceraFactExteriorBLRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptFactExteriorBL;
        }
        private static Stream SetDataReport(FactExteriorBLDataSet rptpackinglistDataSet)
        {
            using (var report = new RptFactExteriorBL())
            {
                report.SetDataSource(rptpackinglistDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}
