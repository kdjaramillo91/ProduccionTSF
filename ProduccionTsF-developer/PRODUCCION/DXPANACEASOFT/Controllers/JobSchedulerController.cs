
using DXPANACEASOFT.Models;
using System.Web.Mvc;
using System;
using System.Data.SqlClient;
using Dapper;
using DocumentFormat.OpenXml.Drawing;
using System.Data;
using System.Configuration;
using DocumentFormat.OpenXml.EMMA;
using DXPANACEASOFT.Models.Dto;
using System.Globalization;
using System.ServiceModel.Security;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using Utilitarios.Logs;
using System.Linq;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using System.Text;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Operations;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class JobSchedulerController : DefaultController
    {
        const string CODIGO_DOCUMENT_TYPE_JOBSCHEDULE = "168";
        const string CODIGO_DOCUMENT_STATE_PROCESADO = "66";
        const string CODIGO_DOCUMENT_STATE_APROBADO = "03";
        const string CODIGO_ESTADO_ACTIVO = "01";
        const string CODIGO_ESTADO_ELIMINADO = "05";
        const string SELECT_JOBSCHEDULER_BY_STATE = "select  id,dateInit,dateEnd,timeHourExecute,serverHost,databaseHost, " +
                                                    "userdb,passwordb,storeProcedure,id_documentState,id_statusProcess,dataResult "+
                                                    "from JobScheduler where id_documentState = ";
        
        const string UPDATE_JOBSCHEDULER_FOR_DELETE = "UPDATE JobScheduler " +
                                                     "SET id_documentState = @id_documentState," +
                                                     "id_userDelete = @id_userDelete," +
                                                     "dateDelete =  @dateDelete " +
                                                     "WHERE ID = @id; ";

        const string INSERT_JOBSCHEDULER = "INSERT JobScheduler " +
                                          "(dateInit,dateEnd,timeHourExecute,serverHost,databaseHost,userdb,passwordb,storeProcedure,id_documentState,id_userCreate,dateCreate,id_statusProcess) values " +
                                          "(@dateInit,@dateEnd, @timeHourExecute,@serverHost,@databaseHost,@userdb,@passwordb,@storeProcedure,@id_documentState,@id_userCreate,@dateCreate,@id_statusProcess) ; SELECT SCOPE_IDENTITY()";
 

        [HttpPost]
        public ActionResult Index()
        {
            var jobInit = getJobSchedule();
            return PartialView("Index", jobInit);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult JobScheduleExecution(JobSchedulerDto jobScheduler, bool isRefresh)
        {
            string message = "";
            int status = 0;
            string seccion = "0";
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string data = "";
            bool ejecutarSp = false;
            
            try
            {
                var jobActual = getJobByState();

                if (jobActual == null)
                {
                    int jobId = transaction(jobScheduler);
                    jobScheduler.id = jobId;
                    ejecutarSp = true;
                }
                else if (jobActual.id_statusProcess == (int)JOBSCHEDULE_STATUS_ENUM.Enviado || jobActual.id_statusProcess == (int)JOBSCHEDULE_STATUS_ENUM.Iniciado)
                {
                    status = jobActual.id_statusProcess;
                }
                else
                {
                    if (!isRefresh)
                    {
                        int jobId = transaction(jobScheduler, jobScheduler.id);
                        jobScheduler.id = jobId;
                        ejecutarSp = true;
                    }
                    else
                    {
                        status = jobActual.id_statusProcess;
                    }
                    
                }
                if (ejecutarSp)
                {
                    var factory = new System.Threading.Tasks.TaskFactory();
                    factory.StartNew(executeStoreProcedure, jobScheduler, TaskCreationOptions.LongRunning).ConfigureAwait(false);
                    status = (int)JOBSCHEDULE_STATUS_ENUM.Enviado;
                }
                
            }
            catch (Exception e)
            {
                status = (int)JOBSCHEDULE_STATUS_ENUM.Fallido;
                message = e.Message;
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, ruta, "JobScheduler", "PROD", seccion, data);
            }

            TempData.Keep("jobscheduler");


            var result = new
            {
                id_jobSchedule = jobScheduler.id,
                message = message,
                timeExecution = DateTime.Now,
                status = status,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateJobResult()
        {
            var job = getJobByState();
            return File(Encoding.ASCII.GetBytes(job.dataResult), "text/plain", $"Produccion_Data_Procesada_{DateTime.Now.ToString("yyyy_MM_dd_HHmm")}.txt");
        }

        #region "Private"
        private JobSchedulerDto getJobSchedule()
        {
            JobScheduler jobScheduler = getJobByState();
            JobSchedulerDto jschedulerDto = new JobSchedulerDto();
            if (jobScheduler == null)
            {
                jschedulerDto = new JobSchedulerDto
                {
                    dateInit = DateTime.Now,
                    dateEnd = DateTime.Now,
                    timeHourExecute = DateTime.Now,
                };
            }
            else
            {
                jschedulerDto = new JobSchedulerDto
                {
                    id = jobScheduler.id,
                    dateInit = jobScheduler.dateInit,
                    dateEnd = jobScheduler.dateEnd,
                    timeHourExecute = jobScheduler.dateInit.Date + (jobScheduler.timeHourExecute ?? DateTime.Now.TimeOfDay),
                    databaseHost = jobScheduler.databaseHost,
                    id_documentState = jobScheduler.id_documentState,
                    passwordb = jobScheduler.passwordb,
                    serverHost = jobScheduler.serverHost,
                    storeProcedure = jobScheduler.storeProcedure,
                    userdb = jobScheduler.userdb,
                    dataResult = jobScheduler.dataResult,
                    id_statusProcess = jobScheduler.id_statusProcess

                };
            }

            TempData["jobscheduler"] = jschedulerDto;
            TempData.Keep("jobscheduler");
            return jschedulerDto;
        }
        private void transactionNotification(string fechaInicioEjecucion, int jobScheduleId, string dataReturn)
        {

            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code == CODIGO_DOCUMENT_TYPE_JOBSCHEDULE);
            DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == CODIGO_DOCUMENT_STATE_APROBADO);
            DocumentState documentStateProcesado = db.DocumentState.FirstOrDefault(r => r.code == CODIGO_DOCUMENT_STATE_PROCESADO);
            Document documentExist = db.Document.FirstOrDefault(r => r.id_documentType == documentType.id);
            bool crearDocument = false;
            string documentNumber = "JOB000000001";
            if (documentExist == null)
            {
                crearDocument = true;
            }
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
                            int documentoId = 0;
                            if (crearDocument)
                            {
                                documentoId = DapperConnection.Transaction<Document>(new Document
                                {
                                    number = documentNumber,
                                    sequential = 1,
                                    emissionDate = DateTime.Now,
                                    description = "Job Document",
                                    reference = "JOB1",
                                    id_emissionPoint = this.ActiveEmissionPoint.id,
                                    id_documentType = documentType.id,
                                    id_documentState = documentState.id,
                                    id_userCreate = this.ActiveUserId,
                                    dateCreate = DateTime.Now,
                                    id_userUpdate = this.ActiveUserId,
                                    dateUpdate = DateTime.Now

                                }, functionDB: DapperExt.InsertDocument, connection: db1, transaction: tr);
                            }
                            else
                            {
                                documentoId = documentExist.id;
                            }

                            DapperConnection.Transaction<Notification>(new Notification
                            {
                                id_user = this.ActiveUserId,
                                id_document = documentoId,
                                noDocument = documentNumber,
                                id_documentType = documentType.id,
                                documentType = documentType.name,
                                id_documentState = documentStateProcesado.id,
                                documentState = documentStateProcesado.name,
                                title = "Ejecucion de proceso Externo",
                                description = $"Ha finalizado la ejecucion del proceso externo con el rango: {fechaInicioEjecucion}, por el usuario {this.ActiveUser.username}",
                                dateTime = DateTime.Now
                            }, actionDB: DapperExt.InsertNotification, connection: db1, transaction: tr);

                            updateJobSchedulerExecution(jobScheduleId, JOBSCHEDULE_STATUS_ENUM.Finalizado, dataReturn);

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
        private int transaction(JobSchedulerDto jobScheduler, int? oldJobSchedulerId = null)
        {
            string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            int newJobId = 0;
            using (var db1 = new System.Data.SqlClient.SqlConnection(dapperDBContext))
            {
                try
                {
                    db1.Open();

                    using (var tr = db1.BeginTransaction())
                    {

                        try
                        {
                            int documentId = 0;
                            string documentNumber = null;


                            if ((oldJobSchedulerId ?? 0) > 0)
                            {
                                deleteJobScheduler(oldJobSchedulerId.Value, db1, tr);
                            }
                            newJobId = saveJobScheduler(jobScheduler, db1, tr);


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
            return newJobId;
        }
        private void deleteJobScheduler(int jobSchedulerId, SqlConnection db, SqlTransaction tr)
        {
            int idDocumentStateEliminado = getIdEstadoByCodigo(CODIGO_ESTADO_ELIMINADO);
            int currentUserId = this.ActiveUserId;

            using (var command = new SqlCommand())
            {
                var parameters = new DynamicParameters();

                command.Parameters.Add("@id_documentState", SqlDbType.Int).Value = idDocumentStateEliminado;
                command.Parameters.Add("@id_userDelete", SqlDbType.Int).Value = currentUserId;
                command.Parameters.Add("@dateDelete", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@id", SqlDbType.VarChar).Value = jobSchedulerId;

                string textCommand = UPDATE_JOBSCHEDULER_FOR_DELETE;

                command.CommandText = textCommand;
                command.CommandType = CommandType.Text;
                command.Connection = db;
                command.Transaction = tr;
                command.ExecuteNonQuery();
            }

        }
        private void updateJobSchedulerExecution(int jobSchedulerId, JOBSCHEDULE_STATUS_ENUM status, string dataResult, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            if (connection != null)
            {
                using (var command = new SqlCommand())
                {
                    var parameters = new DynamicParameters();

                    string optionalWhereCondition = "";
                    if (status == JOBSCHEDULE_STATUS_ENUM.Iniciado)
                    {
                        command.Parameters.Add("@dateInitExecution", SqlDbType.DateTime).Value = DateTime.Now;
                        optionalWhereCondition = "dateInitExecution = @dateInitExecution, ";
                    }
                    else if (status == JOBSCHEDULE_STATUS_ENUM.Finalizado)
                    {
                        command.Parameters.Add("@dateEndExecution", SqlDbType.DateTime).Value = DateTime.Now;
                        optionalWhereCondition = "dateEndExecution = @dateEndExecution, ";

                    }

                    command.Parameters.Add("@id_statusProcess", SqlDbType.Int).Value = status;
                    command.Parameters.Add("@dataResult", SqlDbType.NVarChar).Value = (dataResult ?? "");
                    command.Parameters.Add("@id", SqlDbType.VarChar).Value = jobSchedulerId;

                    string textCommand = $"UPDATE JobScheduler " +
                                         $"SET id_statusProcess = @id_statusProcess," +
                                         $"{optionalWhereCondition}" +
                                         $"dataResult = @dataResult " +
                                         $"WHERE ID = @id; ";


                    command.CommandText = textCommand;
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                }
            }
            else
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
                                using (var command = new SqlCommand())
                                {
                                    var parameters = new DynamicParameters();

                                    string optionalWhereCondition = "";
                                    if (status == JOBSCHEDULE_STATUS_ENUM.Iniciado)
                                    {
                                        command.Parameters.Add("@dateInitExecution", SqlDbType.DateTime).Value = DateTime.Now;
                                        optionalWhereCondition = "dateInitExecution = @dateInitExecution, ";
                                    }
                                    else if (status == JOBSCHEDULE_STATUS_ENUM.Finalizado)
                                    {
                                        command.Parameters.Add("@dateEndExecution", SqlDbType.DateTime).Value = DateTime.Now;
                                        optionalWhereCondition = "dateEndExecution = @dateEndExecution, ";

                                    }

                                    command.Parameters.Add("@id_statusProcess", SqlDbType.Int).Value = status;
                                    command.Parameters.Add("@dataResult", SqlDbType.NVarChar).Value = (dataResult ?? "");
                                    command.Parameters.Add("@id", SqlDbType.VarChar).Value = jobSchedulerId;

                                    string textCommand = $"UPDATE JobScheduler " +
                                                         $"SET id_statusProcess = @id_statusProcess," +
                                                         $"{optionalWhereCondition}" +
                                                         $"dataResult = @dataResult " +
                                                         $"WHERE ID = @id; ";


                                    command.CommandText = textCommand;
                                    command.CommandType = CommandType.Text;
                                    command.Connection = db;
                                    command.Transaction = tr;
                                    command.ExecuteNonQuery();

                                }

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
                        db.Close();
                    }
                }

            }

        }
         
          
        private int saveJobScheduler(JobSchedulerDto jobScheduler, SqlConnection db, SqlTransaction tr)
        {
            int idDocumentStateActivo = getIdEstadoByCodigo(CODIGO_ESTADO_ACTIVO);
            int currentUserId = this.ActiveUserId;
            int newJobId = 0;
            using (var command = new SqlCommand())
            {
                var parameters = new DynamicParameters();
                
                command.Parameters.Add("@dateInit", SqlDbType.DateTime).Value = jobScheduler.dateInit;
                command.Parameters.Add("@dateEnd", SqlDbType.DateTime).Value = jobScheduler.dateEnd;
                command.Parameters.Add("@timeHourExecute", SqlDbType.Time).Value = jobScheduler.timeHourExecute.TimeOfDay;

                command.Parameters.Add("@serverHost", SqlDbType.VarChar).Value = jobScheduler.serverHost;
                command.Parameters.Add("@databaseHost", SqlDbType.VarChar).Value = jobScheduler.databaseHost;
                command.Parameters.Add("@userdb", SqlDbType.VarChar).Value = jobScheduler.userdb;
                command.Parameters.Add("@passwordb", SqlDbType.VarChar).Value = jobScheduler.passwordb;
                command.Parameters.Add("@storeProcedure", SqlDbType.VarChar).Value = jobScheduler.storeProcedure;

                command.Parameters.Add("@id_documentState", SqlDbType.Int).Value = idDocumentStateActivo;
                command.Parameters.Add("@id_userCreate", SqlDbType.Int).Value = currentUserId;
                command.Parameters.Add("@dateCreate", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@id_statusProcess", SqlDbType.Int).Value = (int)JOBSCHEDULE_STATUS_ENUM.Enviado;

                string textCommand = INSERT_JOBSCHEDULER;

                command.CommandText = textCommand;
                command.CommandType = CommandType.Text;
                command.Connection = db;
                command.Transaction = tr;

                object returnObj = command.ExecuteScalar();
                if (returnObj != null)
                {
                    int.TryParse(returnObj.ToString(), out newJobId);
                }

            }
            return newJobId;
        }
        private int getIdEstadoByCodigo(string codigoEstado)
        {
            return (db.DocumentState.FirstOrDefault(r => r.code == codigoEstado)?.id ?? 0);
        }
        private void executeStoreProcedure<T>(T JobScheduleInput)
        {
            string seccion = "0";
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string data = "";
            string dataReturn = null;
            JobSchedulerDto JobSchedule = JobScheduleInput as JobSchedulerDto;
            try
            {
                updateJobSchedulerExecution(JobSchedule.id, JOBSCHEDULE_STATUS_ENUM.Iniciado, dataReturn);

                string remoteConnectionString = $"Data Source={JobSchedule.serverHost};Database={JobSchedule.databaseHost};User ID={JobSchedule.userdb};Password={JobSchedule.passwordb};App=EntityFramework;";
                using (var db = new System.Data.SqlClient.SqlConnection(remoteConnectionString))
                {
                    db.Open();

                    try
                    {

                        using (var command = new SqlCommand())
                        {
                            command.Parameters.Add("@parametro1", SqlDbType.DateTime).Value = JobSchedule.dateInit;
                            command.Parameters.Add("@parametro2", SqlDbType.DateTime).Value = JobSchedule.dateEnd;
                            string textCommand = $"Exec {JobSchedule.storeProcedure} @parametro1, @parametro2";
                            command.CommandText = textCommand;
                            command.CommandType = CommandType.Text;
                            command.Connection = db;
                            command.CommandTimeout = 120;
                            var oReturn = command.ExecuteScalar();
                            dataReturn = (oReturn == null) ? string.Empty : oReturn.ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        MetodosEscrituraLogs.EscribeExcepcionLogNest(e, ruta, "JobScheduler", "PROD", seccion, data);
                    }
                    finally
                    {
                        db.Close();
                    }
                    transactionNotification($"Desde {JobSchedule.dateInit.ToSpanishDateFormat()}, Hasta: {JobSchedule.dateEnd.ToSpanishDateFormat()}", JobSchedule.id, dataReturn);
                    //updateJobSchedulerExecution(JobSchedule.id, JOBSCHEDULE_STATUS_ENUM.Finalizado, dataReturn);
                }
            }
            catch (Exception e)
            {
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, ruta, "JobScheduler", "PROD", seccion, data);
                updateJobSchedulerExecution(JobSchedule.id, JOBSCHEDULE_STATUS_ENUM.Fallido, null);
            }

        }
        private JobScheduler getJobByState()
        {
            JobScheduler[] jobsScheduler = null;
            var idStateActivo = getIdEstadoByCodigo(CODIGO_ESTADO_ACTIVO);

            using (var db = DapperConnection.Connection())
            {
                db.Open();

                string m_sql = $"{SELECT_JOBSCHEDULER_BY_STATE}{idStateActivo}; ";
                try
                {
                    jobsScheduler = db.Query<JobScheduler>(m_sql).ToArray();
                }
                finally
                {
                    db.Close();
                }

            }
            return jobsScheduler?.LastOrDefault();
        }
        #endregion
    }

}