using EntidadesAuxiliares.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Utilitarios.Logs;

namespace AccesoDatos.MSSQL
{
    public static class Extensions
    {
        /// <summary>
        /// Extension del método toList() de una Tabla
        /// Retorna una lista de clase genérica T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tabla">Quien invoca al método</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this System.Data.DataTable tabla) where T : new()
        {
            IList<PropertyInfo> propiedades = typeof(T).GetProperties().ToList();
            List<T> resultado = new List<T>();

            foreach (var fila in tabla.Rows)
            {
                var item = CrearItemObjectoDesdeFila<T>((System.Data.DataRow)fila, propiedades);
                resultado.Add(item);
            }

            return resultado;
        }

        private static T CrearItemObjectoDesdeFila<T>(System.Data.DataRow row, IList<PropertyInfo> propiedades) where T : new()
        {
            T item = new T();
            foreach (var propiedad in propiedades)
            {
                if (propiedad.PropertyType == typeof(System.DayOfWeek))
                {
                    DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[propiedad.Name].ToString());
                    propiedad.SetValue(item, day, null);
                }
                else
                {
                    if (row[propiedad.Name] == DBNull.Value)
                        propiedad.SetValue(item, null, null);
                    else
                        propiedad.SetValue(item, row[propiedad.Name], null);
                }
            }
            return item;
        }
    }

    public static class MetodosDatos2
    {
        public static DataSet ObtieneDatos(string _cadenaConexion
                                    , string procedimientoAlmacenado
                                    , string rutaLog
                                    , string controlador
                                    , string app)
        {
            DataSet ds = new DataSet();
            SqlConnection con = null;
            SqlParameter sq = new SqlParameter();

            try
            {
                if (_cadenaConexion != string.Empty)
                {
                    con = new SqlConnection(_cadenaConexion);
                    con.Open();
                    SqlCommand com = new SqlCommand(procedimientoAlmacenado, con);
                    com.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    da.Fill(ds);
                    con.Close();
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, rutaLog, controlador, app);
            }

            return ds;
        }

        public static DataSet ObtieneDatos(string _cadenaConexion
                                            , string procedimientoAlmacenado
                                            , string rutaLog
                                            , string controlador
                                            , string app
                                            , List<ParamSQL> paramSqlList
                                            , bool hasXmlParameter = false
                                            , int timeout = 90
                                            , SqlConnection connection = null
                                            , IDbTransaction transaction = null
            )
        {
            DataSet ds = new DataSet();
            SqlConnection con = null;
            SqlParameter sq = new SqlParameter();

            try
            {
                bool sinCadenaConexion = string.IsNullOrWhiteSpace(_cadenaConexion);

                if (!sinCadenaConexion || connection != null)
                {
                    con = sinCadenaConexion ? connection : new SqlConnection(_cadenaConexion);

                    if (!sinCadenaConexion) con.Open();

                    SqlCommand com = new SqlCommand(procedimientoAlmacenado, con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandTimeout = timeout;
                    if (transaction != null) com.Transaction = (SqlTransaction)transaction;

                    if (paramSqlList != null && paramSqlList.Count > 0)
                    {
                        foreach (var de in paramSqlList)
                        {
                            com.Parameters.Add(DevuelveParametroSQLDeParamSQL(de));
                        }
                    }

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    da.Fill(ds);
                    if (!sinCadenaConexion)
                    {
                        con.Close();
                        con.Dispose();
                    }

                }
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, rutaLog, controlador, app);
                //throw new Exception($"Error al ejecutar procedimiento almacenado: {procedimientoAlmacenado}");
            }

            return ds;
        }

        public static void ExecuteNoQuery(string _cadenaConexion
                                            , string sentencia
                                            , string rutaLog
                                            , string controlador
                                            , string app
                                            , List<ParamSQL> paramSqlList
                                            , bool hasXmlParameter = false)
        {
            DataSet ds = new DataSet();
            SqlConnection con = null;
            SqlParameter sq = new SqlParameter();

            try
            {
                if (_cadenaConexion != string.Empty)
                {
                    con = new SqlConnection(_cadenaConexion);
                    con.Open();
                    SqlCommand com = new SqlCommand(sentencia, con);
                    com.CommandType = CommandType.Text;

                    if (paramSqlList != null && paramSqlList.Count > 0)
                    {
                        foreach (var de in paramSqlList)
                        {
                            com.Parameters.Add(DevuelveParametroSQLDeParamSQL(de));
                        }
                    }

                    com.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                con.Close();
                con.Dispose();
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, rutaLog, controlador, app);
            }
        }


        public static int? ExecuteIntFunction(string _cadenaConexion
            , string functionName, string rutaLog, string controlador, string app
            , List<ParamSQL> paramSqlList)
        {
            using (var con = new SqlConnection(_cadenaConexion))
            {
                try
                {
                    int? scalarValue;

                    if (con.State != ConnectionState.Open)
                        con.Open();

                    using (var cmd = new SqlCommand(functionName, con))
                    {
                        cmd.Parameters.Clear(); // limpiamos los parametros

                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (var parameter in paramSqlList)
                        {
                            cmd.Parameters.Add(DevuelveParametroSQLDeParamSQL(parameter));
                        }

                        scalarValue = (int?)cmd.ExecuteScalar();

                        cmd.Parameters.Clear(); // limpiamos los parametros
                    }

                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return scalarValue;
                }
                catch (Exception ex)
                {
                    con.Close();
                    con.Dispose();
                    MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, rutaLog, controlador, app);
                    return null;
                }
            }
        }
        public static decimal? ExecuteDecimalProcedure(string _cadenaConexion
            , string functionName, string rutaLog, string controlador, string app
            , List<ParamSQL> paramSqlList)
        {
            using (var con = new SqlConnection(_cadenaConexion))
            {
                try
                {
                    decimal? scalarValue;

                    if (con.State != ConnectionState.Open)
                        con.Open();

                    using (var cmd = new SqlCommand(functionName, con))
                    {
                        cmd.Parameters.Clear(); // limpiamos los parametros

                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach (var parameter in paramSqlList)
                        {
                            cmd.Parameters.Add(DevuelveParametroSQLDeParamSQL(parameter));
                        }

                        scalarValue = (decimal?)cmd.ExecuteScalar();

                        cmd.Parameters.Clear(); // limpiamos los parametros
                    }

                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return scalarValue;
                }
                catch (Exception ex)
                {
                    con.Close();
                    con.Dispose();
                    MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, rutaLog, controlador, app);
                    return null;
                }
            }
        }
        public static decimal? ExecuteDecimalFunction(string _cadenaConexion
            , string functionName, string rutaLog, string controlador, string app,
            List<ParamSQL> paramSqlList)
        {
            using (var con = new SqlConnection(_cadenaConexion))
            {
                try
                {
                    decimal? scalarValue;

                    if (con.State != ConnectionState.Open)
                        con.Open();

                    using (var cmd = new SqlCommand(functionName, con))
                    {
                        cmd.Parameters.Clear(); // limpiamos los parametros
                        cmd.CommandType = CommandType.Text;

                        var parameters = string.Join(",", paramSqlList.Select(e => $"'{e.Valor}'"));
                        var comandText = $"SET Language us_english; Select dbo.{functionName}({parameters});";

                        cmd.CommandText = comandText;
                        scalarValue = (decimal?)cmd.ExecuteScalar();

                        cmd.Parameters.Clear(); // limpiamos los parametros
                    }

                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return scalarValue;
                }
                catch (Exception ex)
                {
                    con.Close();
                    con.Dispose();
                    MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, rutaLog, controlador, app);
                    return null;
                }
            }
        }
        
        private static SqlParameter DevuelveParametroSQLDeParamSQL(ParamSQL param, bool isXmlParemter = false)
        {
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = param.Nombre;
            sqlParam.DbType = param.TipoDato;

            sqlParam.Value = param.Valor != null ? param.Valor : DBNull.Value;

            return sqlParam;
        }
    }
}