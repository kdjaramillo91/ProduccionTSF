using DevExpress.Data.Linq.Helpers;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models.BackgroundProcessManagement;
using DXPANACEASOFT.Services;
using Newtonsoft.Json;
using OfficeOpenXml;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Mvc;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using DXPANACEASOFT.Models;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using Utilitarios.Logs;
using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Dapper;
using System.Data.SqlClient;
using Dapper;
using DevExpress.XtraGauges.Core.Model;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Activities.Expressions;
using DevExpress.XtraSpreadsheet.Commands;
using DXPANACEASOFT.Operations;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class BackgroundProcessManagementController : DefaultController
    {

        const string CODIGO_DOCUMENT_TYPE_GENERACIONSALDOINVENTARIO = "169";
        const string CODIGO_DOCUMENT_STATE_PROCESADO = "66";
        const string CODIGO_DOCUMENT_STATE_APROBADO = "03";
        const string CODIGO_DOCUMENT_STATE_ERROR = "69";
        const string CODIGO_PARAMETRO_SALDO_INICIAL = "PSALINI";
        const string CODIGO_ESTADO_PERIODO_INVENTARIO_CERRADO = "C";


        const string m_InProcessState = "INPROCESS";
        const string m_InProcessState_Description = "EN PROCESO";
        const string m_AvailableState = "AVAILABLE";
        const string m_AvailableState_Descrition = "DISPONIBLE";
        const string m_ResultQueryViewKeyName = "BackgroundProcessManagement_QueryResults";
        const string m_BackgroundProcessManagementModelKey = "backgroundProcessManagement";

        const string sentencia_select_MonthlyBalance =  "SELECT id,id_company,Anio " +
                                                                ",Periodo,id_item,name_item,masterCode " +
                                                                ",id_warehouse,name_warehouse,id_warehouseLocation " +
                                                                ",name_warehouseLocation,id_productionLot,sequencial_productionLot, number_productionLot " +
                                                                ",id_presentation,name_presentation,id_metric_unit " +
                                                                ",code_metric_unit,name_metric_unit,SaldoAnterior " +
                                                                ",Entrada,Salida,SaldoActual,minimum " +
                                                                ",maximum,LB_SaldoAnterior,LB_Entrada " +
                                                                ",LB_Salida,LB_SaldoActual " +
                                                                "FROM MonthlyBalance ";

        private const string sentencia_MonthlyBalanceControl_Estado_PeriodoBodega = "select id,id_company,id_warehouse,Anio,Mes,IsValid,DateIsNotValid,LastDateProcess from MonthlyBalanceControl where id_company = @id_company AND  Anio = @Anio AND Mes = @Mes;";


        [HttpPost]
        public PartialViewResult Index()
        {
            return this.PartialView();
        }
        [HttpPost]
        public JsonResult GetProcessState() 
        {
            string state = DataProviderBackgroundProcessManagement.ProcessState("BALCALPRO");
            string result = string.Empty;

            switch (state)
            {
                case m_InProcessState:
                    result = m_InProcessState_Description; break;
                case m_AvailableState:
                    result = m_AvailableState_Descrition; break;
                default:
                    result = "SIN ESTADO";
                    break;
            }
            return Json(new { description = result}, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SendDataToProcess(MonthlyBalanceProcessFilterDto dataToProcess) 
        {
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string mensaje = string.Empty;

            
            try
            {
                dataToProcess.isMassive = false;
                dataToProcess.idCompany = this.ActiveCompanyId;
                dataToProcess.idUser = this.ActiveUserId;
                TempData["MonthlyBalanceProcessFilterData"] = dataToProcess;


                var respVal = ServiceInventoryBalance.ValidateData(dataToProcess, ref mensaje);

                if (!respVal)
                {
                    return Json(new { isOk = false, description = mensaje }, JsonRequestBehavior.AllowGet);
                }

                

                var respSend = ServiceInventoryBalance.ConvertToParameter(dataToProcess, ref mensaje);

                if (respSend == null)
                {
                    return Json(new { isOk = false, description = mensaje }, JsonRequestBehavior.AllowGet);
                }

                var resd = ServiceInventoryBalance.ValidatePeriodoControl(this.db, respSend, ref mensaje);

                if (!resd)
                {
                    return Json(new { isOk = false, description = mensaje }, JsonRequestBehavior.AllowGet);
                }

                var redP = validateInventoryPeriod(respSend, ref mensaje);
                if (!redP)
                {
                    return Json(new { isOk = false, description = mensaje }, JsonRequestBehavior.AllowGet);
                }

                bool canContinue = false;
                ValidationProcessExecution(out canContinue);
                if (!canContinue)
                {
                    return Json(new { isOk = false, description = "EN PROCESO" }, JsonRequestBehavior.AllowGet);
                }

                var factory = new System.Threading.Tasks.TaskFactory();
                factory.StartNew(ExecuteData, respSend, TaskCreationOptions.LongRunning).ConfigureAwait(false);
                
            }
            catch (Exception ex)
            {
                TempData["MonthlyBalanceProcessFilterData"] = null;
                MetodosEscrituraLogs.EscribeExcepcionLog(ex, ruta, "BackgroundProcessManagement", "PROD");
                return Json(new { isOk = false, description = "Ha ocurrido un error interno, revise el log" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                TempData.Keep("MonthlyBalanceProcessFilterData");
            }
            

            //var respSend = SendMessageToQueue(dataToProcess, ref mensaje);
            //
            //if (!respSend)
            //{
            //    return Json(new { isOk = false, description = mensaje }, JsonRequestBehavior.AllowGet);
            //}

            return Json(new { isOk = true, description = "Se ha enviado la data para procesar." }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ExportExcel()
        {
            var rows = db.MonthlyBalance.Count();

            MonthlyBalanceProcessFilterDto dataToProcess = TempData["MonthlyBalanceProcessFilterData"] as MonthlyBalanceProcessFilterDto;


            bool _isOk = (rows > 0) && (dataToProcess != null);
            string _message = _isOk ? "Se generará el archivo para descarga" : "No existen datos para descargar";
            string _fileName = _isOk ? "SaldosMensuales_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx" : "";

            if (rows > 0)
            {
                TempData[_fileName] = _fileName;
                TempData.Keep(_fileName);
            }

            return Json(new { isOk = _isOk, message = _message, fileName =_fileName });
        }
        [HttpGet]
        public ActionResult DownloadExcel(string fileName)
        {
            string _fileName = (string)TempData[fileName];
            TempData.Remove(fileName);

            MonthlyBalanceProcessFilterDto dataToProcess = TempData["MonthlyBalanceProcessFilterData"] as MonthlyBalanceProcessFilterDto;

            if (dataToProcess == null) return null;

            List<MontlyBalanceExcelDto> preResultMonthlyBalance = new List<MontlyBalanceExcelDto>();
            if (!(dataToProcess.isMassive ?? false))
            {
                int anio = 0;
                int periodo = 0;

                if (!(string.IsNullOrEmpty(dataToProcess.codePeriod) ||
                    string.IsNullOrWhiteSpace(dataToProcess.codePeriod)))
                {
                    var arCode = dataToProcess.codePeriod.Split('-');
                    anio = Convert.ToInt32(arCode[0]);
                    periodo = Convert.ToInt32(arCode[1]);

                }
                
                var preResult = DapperConnection.Execute<MonthlyBalance>($"{sentencia_select_MonthlyBalance} WHERE Anio = @anio AND Periodo= @periodo;", new
                {
                    anio = anio,
                    periodo = periodo
                });

                if (dataToProcess.idWarehouse.HasValue)
                {
                    preResult = preResult
                                      .Where(r => r.id_warehouse == dataToProcess.idWarehouse.Value)
                                      .ToArray();
                }
                if (dataToProcess.idWarehouseLocation.HasValue)
                {
                    preResult = preResult
                                      .Where(r => r.id_warehouseLocation == dataToProcess.idWarehouseLocation.Value)
                                      .ToArray();
                }
                if (dataToProcess.idItem.HasValue)
                {
                    preResult = preResult
                                          .Where(r => r.id_item == dataToProcess.idItem)
                                          .ToArray();
                }

                preResultMonthlyBalance = preResult
                                                .Select(S => new MontlyBalanceExcelDto
                                                {
                                                    Anio = S.Anio,
                                                    Periodo = S.Periodo,
                                                    CodigoProducto = S.masterCode,
                                                    Producto = S.name_item,
                                                    Bodega = S.name_warehouse,
                                                    Ubicacion = S.name_warehouseLocation,
                                                    SequencialLote = S.sequencial_productionLot,
                                                    NumeroLote = S.number_productionLot,
                                                    Presentacion = S.name_presentation,
                                                    UnidadDeMedida = S.code_metric_unit,
                                                    SaldoAnterior = S.SaldoAnterior,
                                                    Entrada = S.Entrada,
                                                    Salida = S.Salida,
                                                    SaldoActual = S.SaldoActual,
                                                    LbSaldoAnterior = S.LB_SaldoAnterior,
                                                    LbEntrada = S.LB_Entrada,
                                                    LbSalida = S.LB_Salida,
                                                    LbSaldoActual = S.LB_SaldoActual
                                                }).ToList();

                
            }
            else
            {
                var preResult = DapperConnection.Execute<MonthlyBalance>($"{sentencia_select_MonthlyBalance} ;");

                preResultMonthlyBalance = preResult.Select(S => new MontlyBalanceExcelDto
                {
                    Anio = S.Anio,
                    Periodo = S.Periodo,
                    CodigoProducto = S.masterCode,
                    Producto = S.name_item,
                    Bodega = S.name_warehouse,
                    Ubicacion = S.name_warehouseLocation,
                    SequencialLote = S.sequencial_productionLot,
                    NumeroLote = S.number_productionLot,
                    Presentacion = S.name_presentation,
                    UnidadDeMedida = S.code_metric_unit,
                    SaldoAnterior = S.SaldoAnterior,
                    Entrada = S.Entrada,
                    Salida = S.Salida,
                    SaldoActual = S.SaldoActual,
                    LbSaldoAnterior = S.LB_SaldoAnterior,
                    LbEntrada = S.LB_Entrada,
                    LbSalida = S.LB_Salida,
                    LbSaldoActual = S.LB_SaldoActual
                }).ToList();

            }
            
            Stream stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Saldos");
                
                worksheet.Cells["A1"].LoadFromCollection(preResultMonthlyBalance, true);
                worksheet.Cells.AutoFitColumns();
                stream = new MemoryStream(package.GetAsByteArray());
                stream.Position = 0;
            }

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", (_fileName?? fileName));
        }
        private bool SendMessageToQueue(MonthlyBalanceProcessFilterDto dataToProcess
            , ref string message)
        {
            bool resp = false;
            try
            {
                var factory = new ConnectionFactory { HostName = ConfigurationManager.AppSettings["hostnamequeue"] };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "balanceprocess.queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                        var objToSend = new MonthlyBalanceProcessMessageDto();

                        if (!(string.IsNullOrEmpty(dataToProcess.codePeriod) ||
                            string.IsNullOrWhiteSpace(dataToProcess.codePeriod)))
                        {
                            objToSend.anio = Convert.ToInt32(dataToProcess.codePeriod.Split('-')[0]);
                            objToSend.mes = Convert.ToInt32(dataToProcess.codePeriod.Split('-')[1]);
                        }
                        objToSend.idProcess = "";
                        objToSend.idUser = ActiveUser.id;
                        objToSend.isMassive = dataToProcess.isMassive ?? false;
                        objToSend.idWarehouse = dataToProcess.idWarehouse;
                        objToSend.idItem = dataToProcess.idItem;

                        string serializedObj = JsonConvert.SerializeObject(objToSend);

                        var body = Encoding.UTF8.GetBytes(serializedObj);

                        channel.BasicPublish(exchange: "balanceprocess.exchange",
                                             routingKey: "balanceprocess.queue.bind",
                                             basicProperties: null,
                                             body: body);

                        if (channel != null && channel.IsOpen)
                            channel.Close();
                        if (connection != null && connection.IsOpen)
                            connection.Close();
                    }
                }
            }
            catch (Exception)
            {
                message = "Se produjo un error al comunicarse con el servicio.";
                return resp;
            }
            resp = true;
            return resp;
        }

        private bool validateInventoryPeriod(MonthlyBalanceProcessMessageDto dataToProcess, ref string mensaje)
        {
            List<int> bodegasIds = new List<int>();
            var resultDates = GlobalUtils.DatesInitEndFromPeriod(dataToProcess.anio, dataToProcess.mes);
            DateTime dateInit = resultDates.Item1;
            DateTime dateEnd = resultDates.Item2;
            string nombreBodogaParametro = null;
            if (dataToProcess.idWarehouse.HasValue)
            {
                bodegasIds.Add(dataToProcess.idWarehouse.Value);
                nombreBodogaParametro = db.Warehouse
                                                .FirstOrDefault(w => w.id == dataToProcess.idWarehouse.Value)?.name;
            }
            else
            {
                bodegasIds = db.Warehouse
                                    .Where(w => w.id_company == this.ActiveCompanyId && w.isActive == true)
                                    .Select(r => r.id)
                                    .ToList();
            }

            var settingSaldoInicial = db.Setting.FirstOrDefault(r => r.code == CODIGO_PARAMETRO_SALDO_INICIAL);
            if (settingSaldoInicial == null)
            {
                mensaje = $"No se ha definido el parámetro: {CODIGO_PARAMETRO_SALDO_INICIAL}";
                return false;
            }
            int anio = 0;
            int mes = 0;
            int.TryParse(settingSaldoInicial.value.Substring(0, 4), out anio);
            int.TryParse(settingSaldoInicial.value.Substring(4, 2), out mes);

            if (dataToProcess.anio == anio && dataToProcess.mes == mes) return true;

            var lsPeriods_initial_pre = db
                                        .InventoryPeriodDetail
                                        .Include("InventoryPeriod")
                                        .Include("InventoryPeriod/Warehouse")
                                        .Include("InventoryPeriod/AdvanceParametersDetail")
                                        .Include("AdvanceParametersDetail")
                                        .Where(r => bodegasIds.Contains(r.InventoryPeriod.id_warehouse)
                                                    && r.InventoryPeriod.year == dataToProcess.anio
                                                    && r.InventoryPeriod.isActive
                                                    && r.InventoryPeriod.id_Company == this.ActiveCompanyId)
                                        .Select(s => new
                                        {
                                            year = s.InventoryPeriod.year,
                                            period = s.periodNumber,
                                            dateInit = s.dateInit,
                                            dateEnd = s.dateEnd,
                                            warehouseId = s.InventoryPeriod.id_warehouse,
                                            warehouseName = s.InventoryPeriod.Warehouse.name,
                                            periodoStateCode = s.AdvanceParametersDetail.valueCode.Trim(),
                                            isClosed = s.isClosed,
                                            codigoTipoPeriodo = s.InventoryPeriod.AdvanceParametersDetail.valueCode.Trim(),
                                            nombreTipoPeriodo = s.InventoryPeriod.AdvanceParametersDetail.description
                                        }).ToArray();

            var lsPeriods_initial = lsPeriods_initial_pre
                                        .Where(r=> r.dateInit.Date <= dateEnd 
                                                   && r.dateEnd.Date >= dateInit)
                                        .ToArray();

            if ((lsPeriods_initial?.Length ?? 0) ==0)
            {
                string mensajePredicado = (string.IsNullOrEmpty(nombreBodogaParametro) ? "para todas las bodega" : $"para la bodega {nombreBodogaParametro}");
                mensaje = $"No existe periodo configurado para el año: {dataToProcess.anio}, mes: {dataToProcess.mes} {mensajePredicado}";
            }

            //int[] bodegasPeriodosIds = lsPeriods_initial
            //                                .Select(r => r.warehouseId)
            //                                .ToArray();
            //
            //var bodegaNoPeriodoIds = bodegasIds
            //                                .Where(r => !bodegasPeriodosIds.Contains(r))
            //                                .ToArray();
            //
            //if ((bodegaNoPeriodoIds?.Length ?? 0) > 0)
            //{
            //    string[] bodegasNombresAr = db.Warehouse
            //                                    .Where(r => bodegaNoPeriodoIds.Contains(r.id))
            //                                    .Select(r => r.name)
            //                                    .ToArray();
            //
            //    string bodegasNombres = bodegasNombresAr
            //                                    .Aggregate((i, j) => $"{i},{j}");
            //
            //    mensaje = $"No se ha procesado el periodo Anio/Mes: {dataToProcess.anio}/{dataToProcess.mes}, para la(s) bodega(s): {bodegasNombres}";
            //    return false;
            //}


            var bodegasPeriodosNoCerrados = lsPeriods_initial
                                                    // Parche: Hasta corregir la pantalla de Periodos
                                                    //.Where(r => !r.isClosed || r.periodoStateCode != CODIGO_ESTADO_PERIODO_INVENTARIO_CERRADO)
                                                    .Where(r => r.periodoStateCode != CODIGO_ESTADO_PERIODO_INVENTARIO_CERRADO)
                                                    .GroupBy(r=> r.warehouseName)
                                                    .Select(r=> new 
                                                    {
                                                       bodega = r.Key,
                                                       periodos = r.Select(t=> new 
                                                       {
                                                          nombreTipoPeriodo = t.nombreTipoPeriodo,
                                                          fechaPeriodoTexto = (t.codigoTipoPeriodo.Trim() == "M") ? $"{t.year}-{GlobalUtils.ConvertToMonthName(t.period)}" : GlobalUtils.DateFromDayYear(t.year, t.period).ToString("yyyy-MM-dd")
                                                       })
                                                       .Select(t=> $"{t.nombreTipoPeriodo}: {t.fechaPeriodoTexto}" )
                                                       .Aggregate((i,j) => $"{i}, {j}")
                                                       
                                                    })
                                                    .Select(r =>  $"Bodega: {r.bodega}, Periodos: {r.periodos} {Environment.NewLine}").ToArray();


            if ((bodegasPeriodosNoCerrados?.Length??0) > 0 )
            {
                mensaje = $"Periodos no cerrados: {bodegasPeriodosNoCerrados.Aggregate((i,j)=> $"{i},{j}" )}";
                return false;
            }

            return true;
        }

        private bool validatePeriodoControl(MonthlyBalanceProcessMessageDto dataToProcess, ref string mensaje)
        {
            List<int> bodegasIds = new List<int>();
            string nombreBodogaParametro = null;
            if (dataToProcess.idWarehouse.HasValue)
            {
                bodegasIds.Add(dataToProcess.idWarehouse.Value);
                nombreBodogaParametro = db.Warehouse
                                                .FirstOrDefault(w => w.id == dataToProcess.idWarehouse.Value)?.name;
            }
            else
            {
                bodegasIds = db.Warehouse
                                    .Where(w => w.id_company == this.ActiveCompanyId && w.isActive == true)
                                    .Select(r => r.id)
                                    .ToList();
            }

            var previewPeriod = ServiceInventoryBalance.GetPreviewPeriod(dataToProcess.anio, dataToProcess.mes);
            int anioPrev = previewPeriod.Item1;
            int mesPrev = previewPeriod.Item2;

            var settingSaldoInicial = db.Setting.FirstOrDefault(r => r.code == CODIGO_PARAMETRO_SALDO_INICIAL);
            if (settingSaldoInicial == null)
            {
                mensaje = $"No se ha definido el parámetro: {CODIGO_PARAMETRO_SALDO_INICIAL}";
                return false;
            }
            int anio = 0;
            int mes = 0;
            int.TryParse(settingSaldoInicial.value.Substring(0, 4), out anio);
            int.TryParse(settingSaldoInicial.value.Substring(4, 2), out mes);

            if (dataToProcess.anio == anio && dataToProcess.mes == mes) return true;

            var control = DapperConnection.Execute<MonthlyBalanceControl>(sentencia_MonthlyBalanceControl_Estado_PeriodoBodega, new 
            {
                id_company = this.ActiveCompanyId,
                Anio = anioPrev,
                Mes = mesPrev
            });

            if ((control?.Length ?? 0) == 0)
            {
                string mensajePredicado = (string.IsNullOrEmpty(nombreBodogaParametro) ? "para todas las bodega" : $"para la bodega {nombreBodogaParametro}");
                mensaje = $"No se ha procesado el periodo Anio Mes {anioPrev}/{mesPrev} {mensajePredicado}";
                return false;
            }

            int[] bodegasProcesadasIds = control.Where(r => r.IsValid).Select(r => r.id_warehouse).ToArray();
            var bodegaNoProcesadasIds = bodegasIds
                                            .Where(r => !bodegasProcesadasIds.Contains(r))
                                            .ToArray();

            if ((bodegaNoProcesadasIds?.Length??0) >0)
            {
                var warehouses = db.Warehouse
                                                .Where(r => bodegaNoProcesadasIds.Contains(r.id))
                                                .Select(r => r.name)
                                                .ToArray();

                string bodegasNombres = warehouses
                                                .Aggregate((i, j) => $"{i},{j}");

                mensaje = $"No se ha procesado el periodo Anio/Mes: {anioPrev}/{mesPrev}, para la(s) bodega(s): {bodegasNombres}";
                return false;
            }

            return true;
        }

        #region -- Proceso Asincrono --
        private void ValidationProcessExecution(out bool canContinue)
        {
            canContinue = false;
            canContinue = ServiceInventoryBalance.Execute(db);
        }

        private void ProcessExection(MonthlyBalanceProcessMessageDto monthlyBalanceMessage)
        {
            ServiceInventoryBalance.Execute(db, monthlyBalanceMessage);
        }

        protected void ExecuteData<T> (T data)
        {
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string fechaInicioEjecucion = "";
            try
            {
                    
                MonthlyBalanceProcessMessageDto monthlyBalanceProcessMessage = data as MonthlyBalanceProcessMessageDto;

                if(monthlyBalanceProcessMessage == null)  throw new Exception("El objeto de la consulta MonthlyBalanceProcessMessageDto es null");
                
                
                fechaInicioEjecucion = $"Año: {monthlyBalanceProcessMessage.anio}, Mes: {monthlyBalanceProcessMessage.mes}, Periodo:{monthlyBalanceProcessMessage.descripcionTipoPeriodo}";

                ProcessExection(monthlyBalanceProcessMessage);
                
                transactionNotification(fechaInicioEjecucion, CODIGO_DOCUMENT_STATE_PROCESADO, "éxito");
                //transactionNotification($"Año: 2022, Mes: 01");
            }
            catch (Exception e)
            {
                TempData["MonthlyBalanceProcessFilterData"] = null;
                ServiceInventoryBalance.SetAvailableProcessInt(db);
                transactionNotification(fechaInicioEjecucion, CODIGO_DOCUMENT_STATE_ERROR, "error");
                MetodosEscrituraLogs.EscribeExcepcionLog(e, ruta, "BackgroundProcessManagement", "PROD");
            }
 
        }

        private void transactionNotification(string fechaInicioEjecucion, string codeState, string bitMessage )
        {

            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.code == CODIGO_DOCUMENT_TYPE_GENERACIONSALDOINVENTARIO);
            DocumentState documentState = db.DocumentState.FirstOrDefault(r => r.code == CODIGO_DOCUMENT_STATE_APROBADO);
            DocumentState documentStateProceso = db.DocumentState.FirstOrDefault(r => r.code == codeState);
            Document documentExist = db.Document.FirstOrDefault(r => r.id_documentType == documentType.id);
            bool crearDocument = false;
            string documentNumber = "KARDX00000001";
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
                                    description = "Kardex Document",
                                    reference = "KARDX1",
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
                                id_documentState = documentStateProceso.id,
                                documentState = documentStateProceso.name,
                                title = "Generación de Kardex",
                                description = $"Ha finalizado con {bitMessage} la ejecución de la Generación de Saldo de Inventario con periodo: {fechaInicioEjecucion}, por el usuario {this.ActiveUser.username}",
                                dateTime = DateTime.Now
                            }, actionDB: DapperExt.InsertNotification, connection: db1, transaction: tr);

                            // definir cambio de estado de background process
                            //updateJobSchedulerExecution(jobScheduleId, JOBSCHEDULE_STATUS_ENUM.Finalizado, dataReturn);

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
        private MonthlyBalanceProcessMessageDto convertToParameter(MonthlyBalanceProcessFilterDto dataToProcess
                   , ref string message)
        {
            MonthlyBalanceProcessMessageDto objToSend = null;
            try
            {

                objToSend = new MonthlyBalanceProcessMessageDto();

                if (!(string.IsNullOrEmpty(dataToProcess.codePeriod) ||
                    string.IsNullOrWhiteSpace(dataToProcess.codePeriod)))
                {
                    var arCode = dataToProcess.codePeriod.Split('-');
                    objToSend.anio = Convert.ToInt32(arCode[0]);
                    objToSend.mes = Convert.ToInt32(arCode[1]);
                    objToSend.codigoTipoPeriodo = arCode[2];
                    objToSend.descripcionTipoPeriodo = arCode[3];
                }
                objToSend.idProcess = "";
                objToSend.idUser = ActiveUser.id;
                objToSend.isMassive = dataToProcess.isMassive ?? false;
                objToSend.idWarehouse = dataToProcess.idWarehouse;
                objToSend.idWarehouseLocation = dataToProcess.idWarehouseLocation;
                objToSend.idItem = dataToProcess.idItem;
                objToSend.id_company = this.ActiveCompanyId;

            }
            catch (Exception)
            {
                message = "Se produjo un error al parsear mensaje.";
                return null;
            }
            
            return objToSend;
        }
        #endregion

        #region ComboCascada
        [HttpPost, ValidateInput(false)]
        public ActionResult PeriodOfWarehouse(int? idWarehouse)
        {
            if (!idWarehouse.HasValue)
            {
                TempData["BackgroundProcessManagementPeriods"] = DataProviderBackgroundProcessManagement.PeriodsFixed();
            }
            else
            {
                //TempData["BackgroundProcessManagementPeriods"] = DataProviderBackgroundProcessManagement.Periods(new string[] { "D", "M" }, idWarehouse.Value);
                TempData["BackgroundProcessManagementPeriods"] = DataProviderBackgroundProcessManagement.PeriodsFixed();
            }

            TempData.Keep("BackgroundProcessManagementPeriods");
             
            return PartialView("comboboxcascading/_cmbPeriod");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult LocationOfWarehouse(int? idWarehouse)
        {
            if (!idWarehouse.HasValue)
            {
                TempData["BackgroundProcessManagementLocations"] = null;
            }
            else
            {
                TempData["BackgroundProcessManagementLocations"] = DataProviderWarehouseLocation.WarehouseLocationsByWarehouse(idWarehouse.Value);
            }

            TempData.Keep("BackgroundProcessManagementLocations");

            return PartialView("comboboxcascading/_cmbLocation");
        }
        #endregion
    }
}