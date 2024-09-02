using Dapper;
using DevExpress.CodeParser;
using DevExpress.Web.ASPxRichEdit.Internal;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Z.Dapper.Plus;

namespace DXPANACEASOFT.Dapper
{
    public static class DapperConnection
    {
        public static IDbConnection Connection()
        {
            string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            IDbConnection db = new SqlConnection(dapperDBContext);

            if(db == null)
            {
                throw new Exception(
                    "No se ha podido establecer la conexión a la base de datos desde DapperDBContext");
            }

            return db;
        }

        public static string EvalPred(string predicate)
        {
            return (string.IsNullOrEmpty(predicate) ? " WHERE " : " AND ");
        }

        public static void BulkInsert<T>(T data)
        {
            using (var db = DapperConnection.Connection())
            {
                db.Open();
                try
                {
                   db.BulkInsert<T>(data);
                }
                finally
                {
                    db.Close();
                }
            }
        }

        public static void BulkUpdate<T>(T data)
        {
            using (var db = DapperConnection.Connection())
            {
                db.Open();
                try
                {
                    db.BulkUpdate<T>(data);
                }
                finally
                {
                    db.Close();
                }
            }
        }

        #region Optimizacion Procesos
        public static async Task BulkInsert<T>(T[] data, IDbConnection conection, IDbTransaction transaction)
        {
            conection.BulkInsert<T>(new DapperPlusContext(conection, transaction), data);             
        }
        public static void BulkUpdate<T>(T[] data, IDbConnection conection, IDbTransaction transaction)
        {
            conection.BulkInsert<T>(new DapperPlusContext(conection, transaction), data);
        }

        public static void BulkInsertTransaction<T>(T[] data, IDbConnection conection, IDbTransaction transaction,params Func<T, object>[] selectors )
        {
            transaction.BulkInsert<T>(  data, selectors);
        }
        public static void BulkUpdateTransaction<T>(T[] data, IDbConnection conection, IDbTransaction transaction, params Func<T, object>[] selectors)
        {
            transaction.BulkUpdate<T>(data, selectors);
        }
        public static void BulkDeleteTransaction<T>(T[] data, IDbConnection conection, IDbTransaction transaction, params Func<T, object>[] selectors)
        {
            transaction.BulkDelete<T>( data, selectors);
        }
        #endregion

        public static T[] Execute<T>(string sentence, object param = null, System.Data.CommandType commandType = System.Data.CommandType.Text,int? timeout =  null)
        {
            T[] toReturn = null;
            using (var db = DapperConnection.Connection())
            {
                db.Open();
                try
                {
                    if (param != null)
                    {
                        toReturn = db.Query<T>(sentence, param,commandType: commandType, commandTimeout: timeout).ToArray();
                    }
                    else
                    {
                        toReturn = db.Query<T>(sentence, commandType: commandType, commandTimeout: timeout).ToArray();
                    }
                    
                }
                catch(Exception e)
                {
                    var f = e;
                }
                finally
                {
                    db.Close();
                }

            }
            return toReturn;
        }

        
        public static int Transaction<T>( T data, 
                                            Action<T, SqlConnection, SqlTransaction> actionDB = null, 
                                            Func<T, SqlConnection, SqlTransaction, int> functionDB = null,
                                            SqlConnection connection = null,
                                            SqlTransaction transaction = null
                                            )
        {
            int newId = 0;
            if (connection == null)
            {
                string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                using (var db = new System.Data.SqlClient.SqlConnection(dapperDBContext))
                {
                    try
                    {
                        db.Open();
                        using (var tr = db.BeginTransaction())
                        {
                            try
                            {
                                if (actionDB != null)
                                {
                                    actionDB(data, db, tr);
                                }
                                if (functionDB != null)
                                {
                                    newId = functionDB(data, db, tr);
                                }

                                tr.Commit();
                            }
                            catch (Exception e)
                            {
                                tr.Rollback();
                                throw;
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        db.Close();
                    }
                }
            }
            else
            {
                if (actionDB != null)
                {
                    actionDB(data, connection, transaction);
                }
                if (functionDB != null)
                {
                    newId = functionDB(data, connection, transaction);
                }

            }

            return newId;
        }

         
    }

    
}