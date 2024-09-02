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
    internal class ProcesoInternoDetalladoAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ProcesoInternoDetallado";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptPProcesoInternoDetallado = new ProcesoInternoDetalladoDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ProcesoInternoDetalladoModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptPProcesoInternoDetallado.CabeceraProcesoInternoDetallado.NewCabeceraProcesoInternoDetalladoRow();
                    cabeceraRow.MPROLIQ = detailResult.MPROLIQ;
                    cabeceraRow.PRODUNnameNombreUnidadLoteProduccion = detailResult.PRODUNnameNombreUnidadLoteProduccion;
                    cabeceraRow.PRODPRnameProcesoLoteProduccion = detailResult.PRODPRnameProcesoLoteProduccion;
                    cabeceraRow.PRODLreceptionDateFechaRecepcion = detailResult.PRODLreceptionDateFechaRecepcion;
                    cabeceraRow.PRODLnumberNumerodeLote = detailResult.PRODLnumberNumerodeLote;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.CODIGO = detailResult.CODIGO;
                    cabeceraRow.RESPONSABLE = detailResult.RESPONSABLE;
                    cabeceraRow.INTERNALLOT = detailResult.INTERNALLOT;
                    cabeceraRow.MERMA = detailResult.MERMA;
                    cabeceraRow.CODEESTADO = detailResult.CODEESTADO;
                    cabeceraRow.LOTORInumberLoteOrigen = detailResult.LOTORInumberLoteOrigen;
                    cabeceraRow.ITEMmasterCodeCodigoProducto = detailResult.ITEMmasterCodeCodigoProducto;
                    cabeceraRow.ITEMnameNombreItem = detailResult.ITEMnameNombreItem;
                    cabeceraRow.ITEMSZnameTalla = detailResult.ITEMSZnameTalla;
                    cabeceraRow.PRODLDquantityRecivedCantidadRecibidaDetalle = detailResult.PRODLDquantityRecivedCantidadRecibidaDetalle;
                    cabeceraRow.ITY2nameliqTipoProducto = detailResult.ITY2nameliqTipoProducto;
                    cabeceraRow.tmpquantityPoundsLiquidationliqlibrastotalrecobidas = detailResult.tmpquantityPoundsLiquidationliqlibrastotalrecobidas;
                    cabeceraRow.Suma = detailResult.Suma;
                    cabeceraRow.Sumaliquidacion = detailResult.Sumaliquidacion;
                    cabeceraRow.PMINIMUM = detailResult.PMINIMUM;
                    cabeceraRow.PMAXIMUM = detailResult.PMAXIMUM;
                    cabeceraRow.Fi = detailResult.Fi;
                    cabeceraRow.Ff = detailResult.Ff;
                    cabeceraRow.mermatotal = detailResult.mermatotal;
                    cabeceraRow.mermatotalGeneral = detailResult.mermatotalGeneral;
                    cabeceraRow.desperdiciototal = detailResult.desperdiciototal;
                    cabeceraRow.totalliq = detailResult.totalliq;
                    cabeceraRow.totallmp = detailResult.totallmp;
                    cabeceraRow.sumadesperdicio = detailResult.sumadesperdicio;
                    rptPProcesoInternoDetallado.CabeceraProcesoInternoDetallado.AddCabeceraProcesoInternoDetalladoRow(cabeceraRow);
                }

                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptPProcesoInternoDetallado,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptProcesoInternoDetalladoDataSet)
        {           
            using (var report = new RptProcesoInternoDetallado())
            {
                report.SetDataSource(rptProcesoInternoDetalladoDataSet[0]);
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptProcesoInternoDetalladoDataSet[1]);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}