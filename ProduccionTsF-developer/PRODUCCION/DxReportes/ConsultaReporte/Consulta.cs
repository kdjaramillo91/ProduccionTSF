using BibliotecaReporte;
using BibliotecaReporte.Model;
using System.Data.SqlClient;
using System.IO;

namespace ConsultaReporte
{
    public static class Consulta
    {
        public static Stream ObtenerCrystalReportStream(SqlConnection sql, string codigoReporte, Parametro[] parametros)
        {
            return Biblioteca.GetReport(sql, codigoReporte, parametros);
        }
    }
}
