using BibliotecaReporte.Dataset;
using BibliotecaReporte.Dataset.ControldeCalidad;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.ControlDeCalidad;
using BibliotecaReporte.Reportes.ControlDeCalidad;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.ControlDeCalidad
{
    internal class AnalisisDeCalidadAuxiliar
    {
        private const string m_StoredProcedureName = "spRpt_AnalisisCalidad";
        private const string m_StoredProcedureNameOtrasEspecies = "spRpt_AnalisisCalidadOtrasEspecies";
        private const string m_StoredProcedureNameDefectos = "spRpt_AnalisisCalidadDefectos";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFacturaComercial = new AnalisisDeCalidadDataSet();
            var rptAnalisisCalidadOtrasEspecies = new AnalisisCalidadOtrasEspeciesDataSet();
            var rptAnalisisCalidadDefectos = new AnalisisCalidadDefectosDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();


            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }
                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<AnalisisDeCalidadModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();
                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFacturaComercial.cabeceraAnalisisCalidad.NewcabeceraAnalisisCalidadRow();
                    cabeceraRow.TipodeAnalisis = detailResult.TipodeAnalisis;
                    cabeceraRow.NAnalisis = detailResult.NAnalisis;
                    cabeceraRow.SecTransaccional = detailResult.SecTransaccional;
                    cabeceraRow.NLote = detailResult.NLote;
                    cabeceraRow.FechaHoradeLlegada = detailResult.FechaHoradeLlegada;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.Camaronera = detailResult.Camaronera;
                    cabeceraRow.NPiscina = detailResult.NPiscina;
                    cabeceraRow.INP = detailResult.INP;
                    cabeceraRow.NAcuerdoMinisterial = detailResult.NAcuerdoMinisterial;
                    cabeceraRow.NTramite = detailResult.NTramite;
                    cabeceraRow.NGuiaRemision = detailResult.NGuiaRemision;
                    cabeceraRow.Producto = detailResult.Producto;
                    cabeceraRow.TipoProceso = detailResult.TipoProceso;
                    cabeceraRow.NGavetas = detailResult.NGavetas;
                    cabeceraRow.LibrasProgramadas = detailResult.LibrasProgramadas;
                    cabeceraRow.LibrasRemitidas = detailResult.LibrasRemitidas;
                    cabeceraRow.LibrasRecibidas = detailResult.LibrasRecibidas;
                    cabeceraRow.FechadeAnalisis = detailResult.FechadeAnalisis;
                    cabeceraRow.HoradeAnalisis = detailResult.HoradeAnalisis;
                    cabeceraRow.ResidualS02 = detailResult.ResidualS02;
                    cabeceraRow.Temperatura = detailResult.Temperatura;
                    cabeceraRow.Gramaje = detailResult.Gramaje;
                    cabeceraRow.OlorCrudo = detailResult.OlorCrudo;
                    cabeceraRow.SaborCabeza = detailResult.SaborCabeza;
                    cabeceraRow.OlorCocido = detailResult.OlorCocido;
                    cabeceraRow.SaborCola = detailResult.SaborCola;
                    cabeceraRow.Color = detailResult.Color;
                    cabeceraRow.Flacidez = detailResult.Flacidez;
                    cabeceraRow.Mudado = detailResult.Mudado;
                    cabeceraRow.DeshidratadoLeve = detailResult.DeshidratadoLeve;
                    cabeceraRow.DeshidratadoModer = detailResult.DeshidratadoModer;
                    cabeceraRow.CabezaFloja = detailResult.CabezaFloja;
                    cabeceraRow.CabezaRoja = detailResult.CabezaRoja;
                    cabeceraRow.CabezaAnaranjada = detailResult.CabezaAnaranjada;
                    cabeceraRow.HepatoRojo = detailResult.HepatoRojo;
                    cabeceraRow.BranquiasSucias = detailResult.BranquiasSucias;
                    cabeceraRow.PicadoLeve = detailResult.PicadoLeve;
                    cabeceraRow.PicadoFuerte = detailResult.PicadoFuerte;
                    cabeceraRow.Quebrado = detailResult.Quebrado;
                    cabeceraRow.Melanosis = detailResult.Melanosis;
                    cabeceraRow.Rosado = detailResult.Rosado;
                    cabeceraRow.Semirosado = detailResult.Semirosado;
                    cabeceraRow.Corbata = detailResult.Corbata;
                    cabeceraRow.Juvenil = detailResult.Juvenil;
                    cabeceraRow.Otros = detailResult.Otros;
                    cabeceraRow.TotalDefectos = detailResult.TotalDefectos;
                    cabeceraRow.RendimientoEntero = detailResult.RendimientoEntero;
                    cabeceraRow.PCaliforniense = detailResult.PCaliforniense;
                    cabeceraRow.PStylirrostris = detailResult.PStylirrostris;
                    cabeceraRow.POccidental = detailResult.POccidental;
                    cabeceraRow.MaterialExtrano = detailResult.MaterialExtrano;
                    cabeceraRow.TotalOtrasEspecies = detailResult.TotalOtrasEspecies;
                    cabeceraRow.AccionesCorrectivas = detailResult.AccionesCorrectivas;
                    cabeceraRow.Referencia = detailResult.Referencia;
                    cabeceraRow.Analista = detailResult.Analista;
                    cabeceraRow.EsConforme = detailResult.EsConforme;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.idcompany = detailResult.Idcompany;
                    cabeceraRow.TotalMuestra = detailResult.TotalMuestra;
                    cabeceraRow.Especie = detailResult.Especie;
                    cabeceraRow.TotalPiezas = detailResult.TotalPiezas;
                    cabeceraRow.ContaminacionHielo = detailResult.ContaminacionHielo;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.CondicionTransporte = detailResult.CondicionTransporte;
                    rptFacturaComercial.cabeceraAnalisisCalidad.AddcabeceraAnalisisCalidadRow(cabeceraRow);
                }

                // OtrasEspecies
                var subDetailAnalisisCalidadOtrasEspecies = db.Query<AnalisisDeCalidadOtrasEspeciesModel>(m_StoredProcedureNameOtrasEspecies, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var subdetail in subDetailAnalisisCalidadOtrasEspecies)
                {
                    var detailRow = rptAnalisisCalidadOtrasEspecies.CabeceraAnalisisCalidadOtrasEspecies.NewCabeceraAnalisisCalidadOtrasEspeciesRow();
                    detailRow.id = subdetail.id;
                    detailRow.resultValue = subdetail.resultValue;
                    detailRow.name = subdetail.name;

                    rptAnalisisCalidadOtrasEspecies.CabeceraAnalisisCalidadOtrasEspecies.AddCabeceraAnalisisCalidadOtrasEspeciesRow(detailRow);
                }

                // Defectos
                var subDetailAnalisisCalidadDefectos = db.Query<AnalisisDeCalidadDefectosModel>(m_StoredProcedureNameDefectos, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var subdetail in subDetailAnalisisCalidadDefectos)
                {
                    var detailRow = rptAnalisisCalidadDefectos.cabeceraAnalisisCalidadDefectos.NewcabeceraAnalisisCalidadDefectosRow();
                    detailRow.id = subdetail.id;
                    detailRow.resultValue = subdetail.resultValue;
                    detailRow.name = subdetail.name;
                    detailRow.unidades = subdetail.unidades;

                    rptAnalisisCalidadDefectos.cabeceraAnalisisCalidadDefectos.AddcabeceraAnalisisCalidadDefectosRow(detailRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);




                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptFacturaComercial,
                rptAnalisisCalidadOtrasEspecies,
                rptAnalisisCalidadDefectos,
                rptCompaniaInfo,
            };

            return dataSet;
        
        }

        private static Stream SetDataReport(DataSet[] rptDataSet)
        {
            using (var report = new RptAnalisisDeCalidad())
            {
                report.SetDataSource(rptDataSet[0]);
                // SubReporte Detalles Calidad
                report.OpenSubreport("RptOtrasEspecies").SetDataSource(rptDataSet[1]);
                report.OpenSubreport("RptDefectos").SetDataSource(rptDataSet[2]);
                // SubReporte Cia
                report.OpenSubreport("RptLogo").SetDataSource(rptDataSet[3]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }


    }
}
