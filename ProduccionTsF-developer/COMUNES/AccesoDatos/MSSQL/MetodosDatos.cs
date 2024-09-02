using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

using EntidadesAuxiliares.SQL;
using Utilitarios.Logs;

// Modificación de prueba...

namespace AccesoDatos.MSSQL
{
    public static class MetodosDatos
    {
        #region"CONSULTAS PROCEDIMIENTOS ALMACENADOS"

        /// <summary>
        /// Ejecuta un Procedimiento almacenado con parametros para Consulta de Datos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedimientoAlmacenado">Nombre del Procedimiento Almacenado</param>
        /// <param name="cadenaConexion">Cadena de Conexion</param>
        /// <param name="parametros">Lista de parametros</param>
        /// <param name="cuerpo">Clave valor de SqlDataReader y una clase genérica para llenar resultado</param>
        /// <returns></returns>
        public static IEnumerable<T> EjecutarConsultaProcedimientoAlmacenado<T>(string procedimientoAlmacenado, string cadenaConexion, SqlParameter[] parametros, Func<SqlDataReader, T> cuerpo)
        {
            List<T> results = new List<T>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    SqlCommand _commando = new SqlCommand(procedimientoAlmacenado, conexion);
                    _commando.Parameters.AddRange(parametros);
                    SqlDataReader reader = _commando.ExecuteReader();
                    while (reader.Read())
                    {
                        results.Add(cuerpo(reader));
                    }
                    reader.Close();
                }
            }
            catch //(Exception ex)
            {

            }
            return results;
        }
        
        /// <summary>
        /// Ejecuta un Procedimiento almacenado para Consulta de Datos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedimientoAlmacenado">Nombre del Procedimiento Almacenado</param>
        /// <param name="cadenaConexion">Cadena de Conexion</param>
        /// <param name="cuerpo">Clave valor de SqlDataReader y una clase genérica para llenar resultado</param>
        /// <returns></returns>
        public static IEnumerable<T> EjecutarConsultaProcedimientoAlmacenado<T>(string procedimientoAlmacenado, string cadenaConexion, Func<SqlDataReader, T> cuerpo)
        {
            List<T> results = new List<T>();
            StreamWriter sw = null;
            string rutaLog = "c:\\TempLog";
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    SqlCommand _commando = new SqlCommand(procedimientoAlmacenado, conexion);
                    SqlDataReader reader = _commando.ExecuteReader();
                    while (reader.Read())
                    {
                        results.Add(cuerpo(reader));
                    }
                    reader.Close();
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                //re = ex.Message;

                
                string fileLog = "\\vic.txt";

                sw = new StreamWriter(rutaLog + fileLog, true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Message.ToString().Trim() + (ex.InnerException != null ? "-Inner: " + ex.InnerException.Message.ToString() : "") + (ex.Source != null ? "-Source: " + ex.Source.ToString() : ""));
                sw.Flush();
                sw.Close();
            }
            return results;
        }

        public static int EjecutarProcesoProcedimientoAlmacenado(string procedimientoAlmacenado
                                                                , string cadenaConexion
                                                                , string rutaLog
                                                                , string controlador
                                                                , string app)
        {
            int res = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    SqlCommand _commando = new SqlCommand(procedimientoAlmacenado, conexion);
                    _commando.ExecuteNonQuery();
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                res = -1;
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, rutaLog, controlador, app);
            }
            return res;
        }
        #endregion
    }
}
