using Quartz;
using System.Threading.Tasks;
using System;
using System.Configuration;
using Utilitarios.Logs;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Dapper;
using System.Linq;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Reflection;
using System.Collections.Generic;
using DXPANACEASOFT.Models.Dto;
using System.Data;
using System.Xml.Linq;
using System.Collections;
using DevExpress.CodeParser.Diagnostics;
using DXPANACEASOFT.Models.Helpers;
using System.Data.Entity;
using System.Data.SqlClient;
using Dapper;
using System.Runtime.CompilerServices;
using Polly.Retry;
using Polly;


namespace DXPANACEASOFT.Services
{

    public class ServiceTransCtl
    {
        public class JobTransCtl : IJob
        {

            public Task Execute(IJobExecutionContext context)
            {
                try
                {
                    //TransCtlQueue processToExecute = ServiceTransCtl.TransCtlEval();
                    //if ((processToExecute?.DocumentId??0) != 0)
                    //{
                    //    ServiceTransCtl.ProcessTrans(processToExecute);
                    //}
                    
                }
                catch (Exception e)
                {
                    fulllog(e);
                }
                
                return Task.CompletedTask;
            }
        }

        
        public static int CODE_FOR_QUEUE = -6666;

        private static string QUEUE_ESTATE_PENDIENTE = "PEN";
        private static string QUEUE_ESTATE_EJECUCION = "EXE";        
        private static string QUEUE_ESTATE_FINALIZE = "FIN";
        private static string DISABLED_CONTRAINT_IMD = "ALTER TABLE InventoryMoveDetail NOCHECK CONSTRAINT FK_InventoryMoveDetail_InventoryMoveDetail; ALTER TABLE InventoryMoveDetail NOCHECK CONSTRAINT FK_InventoryMoveDetail_InventoryMoveDetail1; ALTER TABLE InventoryMoveDetail NOCHECK CONSTRAINT FK_InventoryMoveDetail_InventoryMoveDetail2;";
        private static string ENABLED_CONTRAINT_IMD = "ALTER TABLE InventoryMoveDetail CHECK CONSTRAINT FK_InventoryMoveDetail_InventoryMoveDetail; ALTER TABLE InventoryMoveDetail CHECK CONSTRAINT FK_InventoryMoveDetail_InventoryMoveDetail1; ALTER TABLE InventoryMoveDetail CHECK CONSTRAINT FK_InventoryMoveDetail_InventoryMoveDetail2;";
        private static string GET_ONE_DOCUMENTSTATE_BY_CODE = "SELECT id,name FROM DocumentState where code = @code;";

        public static Tuple<bool,Guid?> TransCtlExternal(    
                                                int documentId,
                                                string documentNumber,
                                                int? documentTypeId = null,
                                                string stage = null, 
                                                int? numdetails = null,
                                                string sessionInfoSerialize = null,
                                                string dataExecution = null,
                                                string dataExecutionTypes = null,                                                
                                                string temDataKey = null,
                                                string temDataValueSerialize = null,
                                                string temDataTypes = null)
        {
            bool isExecute = true;
            string QueueEstate = null;
            string DataExecution = null;
            string DataExecutionTypes = null;
            string DataTempKeys = null;
            string DataTempValues = null;
            string DataTempTypes = null;
            string DataSession = null;

            Guid? identificador = null;
            try
            {
                var queEval = TransCtlEval(documentId,documentTypeId, stage , numdetails);
                identificador = Guid.NewGuid();

                if (    (queEval?.DocumentTypeId??0) == documentTypeId &&
                        (queEval?.DocumentId ?? 0) == documentId &&
                        (queEval?.Stage ?? "") == stage &&
                        (queEval?.NumDetails ?? 0) == numdetails)
                {               
                    isExecute = true;
                    QueueEstate = "EXE";
                    

                    TransCtlQueue.Insert_TransCtlQueue(identificador.Value,
                                                        DateTime.Now,
                                                        TimeSpan.FromSeconds(1),
                                                        0,
                                                        documentTypeId.Value,
                                                        stage,
                                                        null,
                                                        null,
                                                        null,
                                                        null, 
                                                        null,
                                                        (numdetails??0),
                                                        0,
                                                        QueueEstate,
                                                        null,
                                                        documentId,
                                                        documentNumber);
                }
                else
                {
                    isExecute = false;
                    QueueEstate = "PEN";
                    DataExecution = dataExecution;
                    DataExecutionTypes = dataExecutionTypes;
                    DataTempKeys = temDataKey;
                    DataTempValues = temDataValueSerialize;
                    DataSession = sessionInfoSerialize;

                    TransCtlQueue.Insert_TransCtlQueue(identificador.Value,
                                                        DateTime.Now,
                                                        TimeSpan.FromSeconds(1),
                                                        1,
                                                        documentTypeId.Value,
                                                        "",
                                                        dataExecution,
                                                        dataExecutionTypes,
                                                        temDataKey,
                                                        temDataValueSerialize,
                                                        sessionInfoSerialize,
                                                        (numdetails ?? 0),
                                                        1,
                                                        QueueEstate,
                                                        temDataTypes,
                                                        documentId,
                                                        documentNumber);
                }

            }
            catch(Exception e) 
            {
               fulllog(e);
            }
            return new Tuple<bool, Guid?>(isExecute,identificador);
        }

        public static TransCtlQueue TransCtlEval(int? documentId = null, int? documentTypeId = null, string stage = null, int? numdetails = null)
        {
            TransCtlQueue evalprocess = null;
            try
            {
                evalprocess = DapperConnection.Execute<TransCtlQueue>("dbo.pat_TransCtlEval", new
                {
                    documentId  = documentId,
                    documentTypeId = documentTypeId,
                    stage = stage,
                    numdetails = numdetails
                }, System.Data.CommandType.StoredProcedure, timeout: 300)?.FirstOrDefault();
            }
            catch (Exception e)
            {
                fulllog(e);
            }

            return evalprocess;
        }

        public static void ProcessTrans(TransCtlQueue queue )
        {
            try
            {
                Guid identificador = queue.Identificador;
                int documentTypeId = queue.DocumentTypeId;
                string parameters = queue.DataExecution;
                string parametersTypes = queue.DataExecutionTypes;
                ActiveUserDto activeUserInfo = JsonConvert.DeserializeObject<ActiveUserDto>(queue.DataSession);
                string paramTempKeys = queue.DataTempKeys;
                string paramTempValues = queue.DataTempValues;
                string paramTempTypes = queue.DataTempTypes;
                int documentId = queue.DocumentId;
                string documentNumber = queue.DocumentNumber;

                

                TransCtlDocumentTypeConfig documentTypeConfig = TransCtlDocumentTypeConfig.GetOneById(documentTypeId);
                if (documentTypeConfig != null)
                {

                    TransCtlQueue.Update_Queue_State(identificador, QUEUE_ESTATE_EJECUCION);

                    Exec(   identificador,
                            documentTypeConfig, 
                            parameters, 
                            parametersTypes,
                            activeUserInfo,
                            paramTempKeys,
                            paramTempValues,
                            paramTempTypes,
                            documentId,
                            documentNumber
                            );
                }
 
            }
            catch (Exception e)
            {
                fulllog(e);
            }
        }
        
        // => int:  DocumentTypeId, string:  Parametros string[] deserializar
        public static Tuple<int, string[]> EvalTasnCtl() 
        {
            int documentTypeId = 0;
            string[] paramsArray = Array.Empty<string>();
            try
            {



            }
            catch (Exception  e) 
            {
                fulllog(e);
            }
            return new Tuple<int, string[]>(documentTypeId, paramsArray);
        }

        public static void Exec(    Guid identificador, 
                                    TransCtlDocumentTypeConfig documentTypeConfig, 
                                    string param, 
                                    string paramType, 
                                    ActiveUserDto activeUser,
                                    string tempKeyTrans,
                                    string tempValueTrans,
                                    string tempTypeTrans,
                                    int documentId,
                                    string documentNumber
                                    )
        {
            List<object> fullParams = new List<object>();
            try
            {

                string controller = documentTypeConfig.Controller;
                string method = documentTypeConfig.Method;
                int documentTypeId = documentTypeConfig.Id;
                string documentTypeName = documentTypeConfig.DtName;
                string stateCodeOK = documentTypeConfig.CodeStateOK;
                string stateCodeERROR = documentTypeConfig.CodeStateError;


                string[] paramDes =  JsonConvert.DeserializeObject<string[]>(param);
                string[] paramDesTypes = JsonConvert.DeserializeObject<string[]>(paramType);
                //var paramObject = paramDes.Select(r => (object)r ).ToArray();
                object[] paramObject = prepareDataParam(paramDes, paramDesTypes);
                fullParams.AddRange(paramObject);
                fullParams.Add(true);
                fullParams.Add(activeUser);
                fullParams.Add(tempKeyTrans);
                fullParams.Add(tempValueTrans);
                fullParams.Add(tempTypeTrans);

                Type controllerType = Type.GetType(controller);
                string exceptionMessage = null;

                if (controllerType != null)
                {
                    object controllerInstance = Activator.CreateInstance(controllerType);
                    MethodInfo methodImpl = controllerType.GetMethod(method);
                    if (methodImpl != null)
                    {
                        try
                        {
                            var result = methodImpl.Invoke(controllerInstance, fullParams.ToArray());
                        }
                        catch (Exception e)
                        {
                            exceptionMessage = e.InnerException?.Message ?? e.Message;
                            fulllog(e.InnerException ?? e);                            
                        }
                        finally 
                        {
                            // obtener el inner excepciopn ,. validarsi es tipo HadlerExcecion presentar error o GEnericError
                            // enviar el estado que esta en la condfiguracion en caso de error
                            // en la configuracion esta el codigo del estado en caso de OK y el en caso de error
                            string stateCodeEndTransac = null;
                            string textoEstadoMensajeFinalizacion = null;
                            if (string.IsNullOrEmpty(exceptionMessage))
                            {
                                stateCodeEndTransac = stateCodeOK;
                                textoEstadoMensajeFinalizacion = "satisfactoriamente";
                            }
                            else
                            {
                                stateCodeEndTransac = stateCodeERROR;
                                textoEstadoMensajeFinalizacion = "con error";
                            }
                            DocumentState documentState = DapperConnection.Execute<DocumentState>(GET_ONE_DOCUMENTSTATE_BY_CODE, new { code = stateCodeEndTransac })?.FirstOrDefault();
                            FinalizeAndNotify(  identificador,
                                                activeUser.id,
                                                activeUser.username,
                                                documentId,
                                                documentNumber,
                                                documentTypeId,
                                                documentTypeName,
                                                documentState.id,
                                                documentState.name,
                                                textoEstadoMensajeFinalizacion);
                        }
                        
                        //var result = methodImpl.Invoke(controllerInstance, new object [1]);
                        
                    }
                }

            }
            catch (Exception e)
            {
                fulllog(e);  // no se han definido las rutas
            }
        }
        public static void FinalizeAndNotify(   Guid identificadorProcess,
                                                int activeUserId,
                                                string activeUserName,
                                                int documentoId,
                                                string documentNumber,
                                                int documentTypeId,
                                                string documentTypeName,
                                                int documentStateId,
                                                string documentStateName,
                                                string textoEstadoMensajeFinalizacion)
        {
            try
            {

                string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;

                using (var db1 = new System.Data.SqlClient.SqlConnection(dapperDBContext))
                {
                    try
                    {
                        db1.Open();

                        using (var tr = db1.BeginTransaction())
                        {

                            try
                            {
                                 
                                DapperConnection.Transaction<Notification>(new Notification
                                {
                                    id_user = activeUserId,
                                    id_document = documentoId,
                                    noDocument = documentNumber,
                                    id_documentType = documentTypeId,
                                    documentType = documentTypeName,
                                    id_documentState = documentStateId,
                                    documentState = documentStateName,
                                    title = "Control de transacciones",
                                    description = $"Ha finalizado {textoEstadoMensajeFinalizacion} el proceso {documentTypeName}:#{documentNumber}, por el usuario {activeUserName}",
                                    dateTime = DateTime.Now
                                }, actionDB: DapperExt.InsertNotification, connection: db1, transaction: tr);

                                TransCtlQueue.Update_Queue_State(identificadorProcess, QUEUE_ESTATE_FINALIZE, connection: db1, transaction: tr);


                                tr.Commit();
                            }
                            catch
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
                        db1.Close();
                    }
                }
            }
            catch (Exception e)
            {
                fulllog(e);
            }
        }
        public static void Finalize(Guid identificadorProcess)
        {
            try
            {
                TransCtlQueue.Update_Queue_State(identificadorProcess, QUEUE_ESTATE_FINALIZE);

            }
            catch (Exception e)
            {
                fulllog(e);
            }
        }

        public static string GetSessionInfoSerialize(int activeUserId,string userName, int activeCompanyId, int activeEmissionPointId, EntityObjectPermissions permisos)
        {
            string sessionInfoSerialize = null;
            try
            {
                ActiveUserDto sessionInfo = new ActiveUserDto
                {
                    id = activeUserId,
                    id_company = activeCompanyId,
                    id_emissionPoint = activeEmissionPointId,
                    username = userName,
                    permisos = permisos.ToDto()
                };
                sessionInfoSerialize = JsonConvert.SerializeObject(sessionInfo);
            }
            catch (Exception e)
            {
                fulllog(e);
            }
            return sessionInfoSerialize;
        }

        private static void fulllog(Exception e)
        {
            string rutaLog = getRutaPathLog();
            MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, nameof(ServiceTransCtl), "PROD");
        }

        private static string getRutaPathLog()
        { 
            return ConfigurationManager.AppSettings["rutalog"];
        }

        public static object ConvertirTextoATipo(string texto, string tipoNombre)
        {
            try
            {
                // Obtener el tipo usando reflexión
                Type tipo = Type.GetType(tipoNombre);

                if (tipo == null)
                {
                    throw new ArgumentException("El tipo especificado no se encontró.");
                }

                object valorConvertido = Convert.ChangeType(texto, tipo);

                return valorConvertido;
            }
            catch (Exception e)
            {
                fulllog(e);
                return null;
            }
        }

        private static object[] prepareDataParam(string[] data, string[] dataType)
        {
            if (data.Length != dataType.Length)
            {
                throw new ArgumentException("Los arrays deben tener el mismo número de elementos.");
            }

            object[] resultado = new object[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                //if (dataType[i])
                //{ 
                //
                //}
                resultado[i] = ConvertirTextoATipo(data[i], dataType[i]);
                    //$"{array1[i]}{array2[i]}"; // Puedes ajustar el formato según tus necesidades
            }

            return resultado;
        }

        public static async Task DeleteForSp(int[] inventoryMoveDetailIdsForDelete, string process,int iddocument)
        {
            var db1 = new DBContext();
            string xtraInfo = $"Proceso: {process}_{nameof(DeleteForSp)}, Documento: {iddocument}";
            using (DbContextTransaction trans = db1.Database.BeginTransaction())
            {
                System.Data.Common.DbTransaction transaction = trans.UnderlyingTransaction;
                SqlConnection connectionDel = transaction.Connection as SqlConnection;
                try
                {

                    string ids = inventoryMoveDetailIdsForDelete
                                    .Select(r => r.ToString())
                                    .Aggregate((i, j) => $"{i}|{j}");
                    
                    logInfo($"{xtraInfo}, IdsForDelete: {ids}", DateTime.Now);

                    connectionDel.Execute(DISABLED_CONTRAINT_IMD,transaction: transaction, commandTimeout: 200,  commandType: CommandType.Text);

                    connectionDel.Execute("dbo.TransCtlDeleteInventoryMoveDetail", new
                    {
                        inventoryMoveDetailIds = ids
                    }, transaction, 600, CommandType.StoredProcedure);

                    trans.Commit();
                    
                    logInfo($"{xtraInfo}-END", DateTime.Now);
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    connectionDel.Execute(ENABLED_CONTRAINT_IMD, transaction: transaction, commandTimeout: 200, commandType: CommandType.Text);
                    fullLog(e, extraInfo: xtraInfo);
                    throw;
                }

                connectionDel.Execute(ENABLED_CONTRAINT_IMD, transaction: transaction, commandTimeout: 200, commandType: CommandType.Text);


            }
        }

        private static void logInfo(string mensaje, DateTime fechaHora)
        {
            string app = "Produccion";
            string origen = "Service.Transacciones";
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            MetodosEscrituraLogs.EscribeMensajeLog($"{mensaje} - {fechaHora}", rutaLog, origen, app);
        }

        private static void fullLog(Exception e,
                                string seccion = null,
                                string extraInfo = null,
                                [CallerFilePath] string callFilePath = "",
                                [CallerMemberName] string memberName = "",
                                [CallerLineNumber] int lineNumber = 0)
        {
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            string origen = "Service.Transacciones";
            string app = "Produccion";
            MetodosEscrituraLogs.EscribeExcepcionLogNest(e,
                                                            rutaLog,
                                                            origen,
                                                            app,
                                                            seccion: seccion,
                                                            extraInfo: extraInfo,
                                                            callFilePath: callFilePath,
                                                            memberName: memberName,
                                                            lineNumber: lineNumber);
        }
        #region Resilent
        static TimeSpan retryAttemptConfig(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
        static AsyncRetryPolicy _retryPolicy = Policy
                                                  .Handle<Exception>()
                                                  .WaitAndRetryAsync(7, retryAttemptConfig);

        //public static Task RetrySpecial<T>(Func<Task<T>> action)
        public static async Task DeleteBulkSp(int[] inventoryMoveDetailIdsForDelete, string process, int iddocument)
        {
           await _retryPolicy.ExecuteAsync(()=> DeleteForSp(inventoryMoveDetailIdsForDelete, process, iddocument));
        }
        #endregion

        #region ItemInventory
        public static void UpdateItemInventory(ItemInventoryDto[] itemInventories, IDbTransaction transaction, SqlConnection conn)
        {


            string itemInventoriesParam = JsonConvert.SerializeObject(itemInventories);

            conn.Execute("dbo.TransCtlUpdateItemInventory", new
            {
                itemInventorys = itemInventoriesParam
            }, transaction: transaction, commandTimeout: 600, commandType: CommandType.StoredProcedure);
        }
        #endregion
    }
}