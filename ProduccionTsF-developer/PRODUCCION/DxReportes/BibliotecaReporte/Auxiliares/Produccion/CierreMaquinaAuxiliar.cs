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
    internal class CierreMaquinaAuxiliar
    {
        private const string m_StoredProcedureName = "spRptCierreMaquina";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static CierreMaquinaDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptIngreso = new CierreMaquinaDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<CierreMaquinaModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptIngreso.CabeceraCierreMaquina.NewCabeceraCierreMaquinaRow();
                    cabeceraRow.FechaEmission = detailResult.FechaEmission;
                    cabeceraRow.Maquina = detailResult.Maquina;
                    cabeceraRow.Responsable = detailResult.Responsable;
                    cabeceraRow.Turno = detailResult.Turno;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.NumeroPersona = detailResult.NumeroPersona;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.NumeroLiquidacion = detailResult.NumeroLiquidacion;
                    cabeceraRow.Provider = detailResult.Provider;
                    cabeceraRow.NameProviderShrimp = detailResult.NameProviderShrimp;
                    cabeceraRow.ProductionUnitProviderPool = detailResult.ProductionUnitProviderPool;
                    cabeceraRow.Detailweight = detailResult.Detailweight;
                    cabeceraRow.ProccesType = detailResult.ProccesType;
                    cabeceraRow.NumberLote = detailResult.NumberLote;
                    cabeceraRow.NumberLot = detailResult.NumberLot;
                    cabeceraRow.PlantProcess = detailResult.PlantProcess;
                    cabeceraRow.PoundsRemitted = detailResult.PoundsRemitted;
                    cabeceraRow.PoundsProcessed = detailResult.PoundsProcessed;
                    cabeceraRow.PoundsCooling = detailResult.PoundsCooling;
                    cabeceraRow.noOfBox = detailResult.noOfBox;
                    cabeceraRow.Detailstate = detailResult.Detailstate;
                    cabeceraRow.Cabeza = detailResult.Cabeza;
                    cabeceraRow.Cola = detailResult.Cola;
                    cabeceraRow.CabezaRefri = detailResult.CabezaRefri;
                    cabeceraRow.ColaRefri = detailResult.ColaRefri;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    


                    rptIngreso.CabeceraCierreMaquina.AddCabeceraCierreMaquinaRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptIngreso;
        }

        private static Stream SetDataReport(CierreMaquinaDataSet rptIngresoDataSet)
        {
            using (var report = new RptCierreMaquina())
            {
                report.SetDataSource(rptIngresoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}