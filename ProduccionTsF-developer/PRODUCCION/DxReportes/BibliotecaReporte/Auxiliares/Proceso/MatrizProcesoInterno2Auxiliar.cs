using BibliotecaReporte.Dataset.Procesos;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.Procesos;
using BibliotecaReporte.Reportes.Procesos;
using CrystalDecisions.Shared;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaReporte.Auxiliares.Proceso
{
   internal class MatrizProcesoInterno2Auxiliar
    {
        private const string m_StoredProcedureName = "spPar_MatrizProcesosInterno2";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static MatrizProcesoInterno2DataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptMatrizProcesoInterno2DataSet = new MatrizProcesoInterno2DataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<MatrizProcesoInterno2Model>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptMatrizProcesoInterno2DataSet.CabeceraMatrizProcesoInterno2.NewCabeceraMatrizProcesoInterno2Row();
                    cabeceraRow.ID = detailResult.ID;
                    cabeceraRow.Tipo = detailResult.Tipo;
                    cabeceraRow.UnidadDeProduccion = detailResult.UnidadDeProduccion;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.FechaDeRecepcion = detailResult.FechaDeRecepcion;
                    cabeceraRow.LoteSecuenciaInterna = detailResult.LoteSecuenciaInterna;
                    cabeceraRow.NumeroDeLote = detailResult.NumeroDeLote;
                    cabeceraRow.Bodega = detailResult.Bodega;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.Responsable = detailResult.Responsable;
                    cabeceraRow.NombreDeLote = detailResult.NombreDeLote;
                    cabeceraRow.CodigoDeItem = detailResult.CodigoDeItem;
                    cabeceraRow.NombreDeItem = detailResult.NombreDeItem;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.UnidadDeMedida = detailResult.UnidadDeMedida;
                    cabeceraRow.TipoDeProducto = detailResult.TipoDeProducto;
                    cabeceraRow.Libras = detailResult.Libras;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.Presentacion = detailResult.Presentacion;
                    cabeceraRow.Minimo = detailResult.Minimo;
                    cabeceraRow.Maximo = detailResult.Maximo;
                    cabeceraRow.UnidadDeLaPresentacion = detailResult.UnidadDeLaPresentacion;
                    cabeceraRow.Categoria = detailResult.Categoria;
                    cabeceraRow.Marca = detailResult.Marca;
                    cabeceraRow.Grupo = detailResult.Grupo;
                    cabeceraRow.Subgrupo = detailResult.Subgrupo;
                    rptMatrizProcesoInterno2DataSet.CabeceraMatrizProcesoInterno2.AddCabeceraMatrizProcesoInterno2Row(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptMatrizProcesoInterno2DataSet;
        }
        private static Stream SetDataReport(MatrizProcesoInterno2DataSet rptMatrizProcesoInterno2DataSet)
        {
            using (var report = new RptMatrizProcesoInterno2())
            {
                report.SetDataSource(rptMatrizProcesoInterno2DataSet);

                var streamReport = report.ExportToStream(ExportFormatType.Excel);
                report.Close();

                return streamReport;
            }
        }



    }
}
