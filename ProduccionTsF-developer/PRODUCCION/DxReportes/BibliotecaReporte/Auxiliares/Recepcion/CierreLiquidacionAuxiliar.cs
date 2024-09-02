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
   internal class CierreLiquidacionAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_CierreLiquidacion";
        private const string m_StoredProcedureNameSub = "spPar_LiquidacionCarroXCarroLote";
        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFacturaComercial = new CierreLiquidacionDataSet();
            var rptLiquidacionCarroXCarroLote = new LiquidacionCarroXCarroLoteDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }
                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<CierreLiquidacionModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFacturaComercial.CabeceraCierreLiquidacion.NewCabeceraCierreLiquidacionRow();
                    cabeceraRow.IdProduccion = detailResult.IdProduccion;
                    cabeceraRow.FechaHoraRecepcion = detailResult.FechaHoraRecepcion;
                    cabeceraRow.NumeroLote = detailResult.NumeroLote;
                    cabeceraRow.CodeProcessType = detailResult.CodeProcessType;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.TallaProducto = detailResult.TallaProducto;
                    cabeceraRow.CantidadCajas = detailResult.CantidadCajas;
                    cabeceraRow.GuiaRemision = detailResult.GuiaRemision;
                    cabeceraRow.CantidadmaximaXcajas = detailResult.CantidadmaximaXcajas;
                    cabeceraRow.NombreProveedor = detailResult.NombreProveedor;
                    cabeceraRow.NombrePiscina = detailResult.NombrePiscina;
                    cabeceraRow.SecuenciaLiquidacion = detailResult.SecuenciaLiquidacion;
                    cabeceraRow.LibrasProcesadas = detailResult.LibrasProcesadas;
                    cabeceraRow.LibrasBasuras = detailResult.LibrasBasuras;
                    cabeceraRow.LibrasRecibidas = detailResult.LibrasRecibidas;
                    cabeceraRow.LibrasNetas = detailResult.LibrasNetas;
                    cabeceraRow.Rendsr1 = detailResult.Rendsr1;
                    cabeceraRow.Rendsr2 = detailResult.Rendsr2;
                    cabeceraRow.LibrasDespachadas = detailResult.LibrasDespachadas;
                    cabeceraRow.LibrasRechazo = detailResult.LibrasRechazo;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.FechaProceso = detailResult.FechaProceso;
                    cabeceraRow.HoraInicio = detailResult.HoraInicio;
                    cabeceraRow.HoraFin = detailResult.HoraFin;
                    cabeceraRow.UsuarioModifica = detailResult.UsuarioModifica;
                    cabeceraRow.Codetype = detailResult.Codetype;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.CantidadMinimaXcajas = detailResult.CantidadMinimaXcajas;
                    
                    rptFacturaComercial.CabeceraCierreLiquidacion.AddCabeceraCierreLiquidacionRow(cabeceraRow);
                }

                var subDetail = db.Query<LiquidacionCarroXCarroLoteModel>(m_StoredProcedureNameSub, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in subDetail)
                {
                    var cabeceraRow = rptLiquidacionCarroXCarroLote.LiquidacionCarroXCarroLote.NewLiquidacionCarroXCarroLoteRow();
                    cabeceraRow.IdLiquidacionCarro = detailResult.IdLiquidacionCarro;
                    cabeceraRow.NombreMaquina = detailResult.NombreMaquina;
                    cabeceraRow.NombreTurno = detailResult.NombreTurno;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.HoraInicio  = detailResult.HoraInicio;
                    cabeceraRow.HoraFin = detailResult.HoraFin;
                    cabeceraRow.LibrasProcesadas = detailResult.LibrasProcesadas;
                    cabeceraRow.TipoProceso = detailResult.TipoProceso;
                    rptLiquidacionCarroXCarroLote.LiquidacionCarroXCarroLote.AddLiquidacionCarroXCarroLoteRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptFacturaComercial,
                rptLiquidacionCarroXCarroLote,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptCierreLiquidacion())
            {
                report.SetDataSource(dataSet[0]);
                //SubReporte
                report.OpenSubreport("RptLiquidacionCarroXCarro.rpt").SetDataSource(dataSet[1]);
                report.OpenSubreport("RptLogo.rpt").SetDataSource(dataSet[2]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
