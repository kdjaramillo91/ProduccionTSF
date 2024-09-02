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
   internal class AnticipoAProveedoresAuxiliar
    {

        private const string m_StoredProcedureName = "spPar_AnticipoAProveedores";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static AnticipoAProveedoresDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptAnticipoAProveedores = new AnticipoAProveedoresDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<AnticipoAProveedoresModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptAnticipoAProveedores.CabeceraAnticipoAProveedores.NewCabeceraAnticipoAProveedoresRow();
                    cabeceraRow.IdAnticipo = detailResult.IdAnticipo;
                    cabeceraRow.Proveedor = detailResult.Proveedor;
                    cabeceraRow.Ruc = detailResult.Ruc;
                    cabeceraRow.Telefono = detailResult.Telefono;
                    cabeceraRow.Lote = detailResult.Lote;
                    cabeceraRow.N = detailResult.N;
                    cabeceraRow.Fecha = detailResult.Fecha;
                    cabeceraRow.Comprador = detailResult.Comprador;
                    cabeceraRow.Sitio = detailResult.Sitio;
                    cabeceraRow.Libras_recibidas = detailResult.Libras_recibidas;
                    cabeceraRow.Libras_remitidas = detailResult.Libras_remitidas;
                    cabeceraRow.Libras_despachadas = detailResult.Libras_despachadas;
                    cabeceraRow.Gramage_promedio = detailResult.Gramage_promedio;
                    cabeceraRow.Fecha_recepcion = detailResult.Fecha_recepcion;
                    cabeceraRow.Fecha_procesamiento = detailResult.Fecha_procesamiento;
                    cabeceraRow.Inp_camar = detailResult.inp_camar;
                    cabeceraRow.Acu_camar = detailResult.Acu_camar;
                    cabeceraRow.Tra_camar = detailResult.Tra_camar;
                    cabeceraRow.Inp_ampar = detailResult.Inp_ampar;
                    cabeceraRow.Acu_ampar = detailResult.Acu_ampar;
                    cabeceraRow.Tra_ampar = detailResult.Tra_ampar;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Lista = detailResult.Lista;
                    cabeceraRow.Porcentajeanticipo = detailResult.Porcentajeanticipo;
                    cabeceraRow.Valor_anticipo = detailResult.Valor_anticipo;
                    cabeceraRow.Proceso = detailResult.Proceso;
                    cabeceraRow.Clase = detailResult.Clase;
                    cabeceraRow.Talla = detailResult.Talla;
                    cabeceraRow.Libras = detailResult.Libras;
                    cabeceraRow.Precio = detailResult.Precio;
                    cabeceraRow.Total = detailResult.Total;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.NombreUsuario = detailResult.NombreUsuario;
                    cabeceraRow.Piscina = detailResult.Piscina;
                    cabeceraRow.PrecioPromedio = detailResult.PrecioPromedio;
                    cabeceraRow.ValorAproximado = detailResult.ValorAproximado;
                    cabeceraRow.ProcesoLote = detailResult.ProcesoLote; 
                    cabeceraRow.ProcesoPlanta = detailResult.ProcesoPlanta;
                    rptAnticipoAProveedores.CabeceraAnticipoAProveedores.AddCabeceraAnticipoAProveedoresRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptAnticipoAProveedores;
        }
        private static Stream SetDataReport(AnticipoAProveedoresDataSet rptAnticipoAProveedoresDataSet)
        {
            using (var report = new RptAnticipoAProveedores())
            {
                report.SetDataSource(rptAnticipoAProveedoresDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }

    }
}
