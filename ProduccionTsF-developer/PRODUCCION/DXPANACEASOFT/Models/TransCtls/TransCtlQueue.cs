using Dapper;
using DXPANACEASOFT.Dapper;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace DXPANACEASOFT.Models
{

    public partial class TransCtlQueue
    {
        public Guid Identificador { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TimeSpan TiempoEjecucion { get; set; }
        public int Intentos { get; set; }
        public int DocumentTypeId { get; set; }
        public int DocumentId { get; set; }
        public string DocumentNumber { get; set; }
        public string Stage { get; set; }
        public string DataExecution { get; set; }
        public string DataExecutionTypes { get; set; }
        public string DataTempKeys { get; set; }
        public string DataTempValues { get; set; }
        public string DataTempTypes { get; set; }
        public string DataSession { get; set; }
        public int NumDetails { get; set; }
        public int QueueSec { get; set; }
        public string QueueEstate { get; set; }


        private static string INSERT_TRANS_CTL_QUEUE =  "INSERT INTO TransCtlQueue " +
                                                        "(Identificador, FechaInicio,TiempoEjecucion,Intentos, DocumentTypeId, " +
                                                        "Stage,DataExecution,DataExecutionTypes,DataTempKeys,DataTempValues,DataSession, " +
                                                        "NumDetails,QueueSec, QueueEstate,DataTempTypes,DocumentId,DocumentNumber ) VALUES ( " +
                                                        "@Identificador, @FechaInicio, @TiempoEjecucion, @Intentos, @DocumentTypeId, " +
                                                        "@Stage, @DataExecution, @DataExecutionTypes, @DataTempKeys, @DataTempValues, @DataSession, " +
                                                        "@NumDetails, @QueueSec, @QueueEstate, @DataTempTypes, @DocumentId, @DocumentNumber); ";


        private static string UPDATE_TRANS_CTL_QUEUE = "UPDATE TransCtlQueue " +
                                                       "SET  FechaInicio=@FechaInicio,FechaFin=@FechaFin, TiempoEjecucion=@TiempoEjecucion,Intentos=@Intentos,DocumentTypeId=@DocumentTypeId, " +
                                                       "Stage=@Stage,DataExecution=@DataExecution,DataExecutionTypes=@DataExecutionTypes,DataTempKeys=@DataTempKeys, " +
                                                       "DataTempValues=@DataTempValues,DataSession=@DataSession,NumDetails=@NumDetails,QueueSec=@QueueSec,QueueEstate=@QueueEstate " +
                                                       "DataTempTypes=@DataTempTypes, DocumentId=@DocumentId, DocumentNumber=@DocumentNumber " +
                                                       "WHERE Identificador=@Identificador; ";

        private static string UPDATE_QUEUE_STATE =  "UPDATE TransCtlQueue "+
                                                    "SET  QueueEstate=@QueueEstate,FechaFin=@FechaFin " +
                                                    "WHERE Identificador=@Identificador; ";

        public static void Update_Queue_State (  Guid identificador,
                                                 string queueEstate,
                                                 SqlConnection connection= null, 
                                                 DbTransaction transaction = null
                                                 )
        {
            if (connection == null)
            {
                DapperConnection.Execute<TransCtlQueue>(UPDATE_QUEUE_STATE, new
                {
                    QueueEstate = queueEstate,
                    Identificador = identificador,
                    FechaFin = DateTime.Now
                });
            }
            else 
            {
                connection.Execute(UPDATE_QUEUE_STATE, new
                {
                    QueueEstate = queueEstate,
                    Identificador = identificador,
                    FechaFin = DateTime.Now
                }, transaction);
            }
            
        }

        public static void Update_TransCtlQueue(    Guid Identificador,
                                                    DateTime FechaInicio,
                                                    DateTime FechaFin,
                                                    TimeSpan TiempoEjecucion,
                                                    int Intentos,
                                                    int DocumentTypeId,
                                                    string Stage,
                                                    string DataExecution,
                                                    string DataExecutionTypes,
                                                    string DataTempKeys,
                                                    string DataTempValues,
                                                    string DataSession,
                                                    int NumDetails,
                                                    int QueueSec,
                                                    string QueueEstate,
                                                    string DataTempTypes,
                                                    int DocumentId,
                                                    string DocumentNumber)
        {
            DapperConnection.Execute<TransCtlQueue>(UPDATE_TRANS_CTL_QUEUE, new
            {
                
                FechaInicio = FechaInicio,
                FechaFin = FechaFin,
                TiempoEjecucion = TiempoEjecucion,
                Intentos = Intentos,
                DocumentTypeId = DocumentTypeId,
                Stage = Stage,
                DataExecution = DataExecution,
                DataExecutionTypes = DataExecutionTypes,
                DataTempKeys = DataTempKeys,
                DataTempValues = DataTempValues,
                DataSession = DataSession,
                NumDetails = NumDetails,
                QueueSec = QueueSec,
                QueueEstate = QueueEstate,
                DataTempTypes = DataTempTypes,
                DocumentId = DocumentId,
                DocumentNumber = DocumentNumber,
                Identificador = Identificador
            });
        }

        public static void Insert_TransCtlQueue(    Guid Identificador,
                                                    DateTime FechaInicio,
                                                    TimeSpan TiempoEjecucion,
                                                    int Intentos,
                                                    int DocumentTypeId,
                                                    string Stage,
                                                    string DataExecution,
                                                    string DataExecutionTypes,
                                                    string DataTempKeys,
                                                    string DataTempValues,
                                                    string DataSession,
                                                    int NumDetails,
                                                    int QueueSec,
                                                    string QueueEstate,
                                                    string DataTempTypes,
                                                    int DocumentId,
                                                    string DocumentNumber)
        {
            DapperConnection.Execute<TransCtlQueue>(INSERT_TRANS_CTL_QUEUE, new
            {
                Identificador = Identificador,
                FechaInicio = FechaInicio,
                TiempoEjecucion = TiempoEjecucion,
                Intentos = Intentos,
                DocumentTypeId = DocumentTypeId,
                Stage = Stage,
                DataExecution = DataExecution,
                DataExecutionTypes = DataExecutionTypes,
                DataTempKeys = DataTempKeys,
                DataTempValues = DataTempValues,
                DataSession = DataSession,
                NumDetails = NumDetails,
                QueueSec = QueueSec,
                QueueEstate = QueueEstate,
                DataTempTypes = DataTempTypes,
                DocumentId = DocumentId,
                DocumentNumber = DocumentNumber
            });
        }
    }
}