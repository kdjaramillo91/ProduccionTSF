using Dapper;
using DevExpress.CodeParser;
using DevExpress.DataProcessing;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraSpreadsheet.Commands;
using DocumentFormat.OpenXml.Drawing.Charts;
//using DocumentFormat.OpenXml.Office2010.Excel;
//using DocumentFormat.OpenXml.Office2010.Excel;
using DXPANACEASOFT.Controllers;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE.Xmls.NotaDebito;
using DXPANACEASOFT.Operations;
using EntidadesAuxiliares.SQL;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;


namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class CostProductValuationController : DefaultController
    {
        private const string m_VariableParametroMetodoValorizacionKey = "METVAL";
        private const string m_MetodoValorizacionProceso = "PROCESO";
        private const string m_MetodoValorizacionLote = "LOTE";

        private const string m_TipoDocumentoCostProductValuation = "158";

        private const string m_PendienteDocumentState = "01";
        private const string m_AprobadoDocumentState = "03";
        private const string m_AnuladoDocumentState = "05";

        private const string m_ResultQueryViewKeyName = "ProductValuation_QueryResults";
        private const string m_CostProductValuationModelKey = "costProductValuation";
        private const string m_CostProductValuationExcelModelKey = "costProductValuationExcel";
        private const string m_ProductionCostProductValuationWarningesGeneradasModelKey = "novedades_generadas";

        private const string m_ValorizacionManualKey = "Manual";
        private const string m_ValorizacionAutomaticaKey = "Automático";

        private const string m_NaturalezaIngresoKey = "I";
        private const string m_NaturalezaEgresoKey = "E";

        private const string m_TipoCalculoPromedioKey = "Promedio";
        private const string m_TipoCalculoHeredadoKey = "Heredado";
        private const string m_TipoCalculoProcesoKey = "Proceso";
        private const string m_TipoCalculoAbsorcionPTKey = "AbsorcionPT";
        private const string m_TipoCalculoAbsorcionSBKey = "AbsorcionSB";

        private const string m_MantenerCostoAsignadoKey = "MANCOSTASIG";
        private const string m_CalcularCostoPromedioKey = "CALCOSTPROM";
        private const string m_AsignarCostoCoeficienteKey = "ASIGCOSTCOEF";
        private const string m_AsignarCostoEgresoTransKey = "ASIGCOSTEGRE";
        private const string m_AsignarCostoProcesoTransKey = "ASIGCOSTPROC";

        private const string m_procedureDataInventario = "ProductionCostProductValuation_GetInventoryData";
        private const string m_procedureDataInventarioProducto = "ProductionCostProductValuation_GetInventoryProductData";
        private const string m_procedureGetInventoryMoveDetailMasterizado = "GetInventoryMoveDetailMasterizado";
        private const string m_procedureGetAutomaticTransferExit = "GetAutomaticTransferExit";
        private const string m_procedureGetInventoryMoveExitMPProcess = "GetInventoryMoveExitMPProcess";
        private const string m_procedureGetCostManualRelation = "GetCostManualRelation";
        public const string m_inventoryMoveDetailDTO = "ProductionCostProductValuationInventoryMove_DTO";
        public const string m_inventoryBalanceDTO = "ProductionCostProductValuationBalance_DTO";
        public const string m_inventoryResumenDTO = "ProductionCostProductValuationResumen_DTO";

        public const string m_tipoPeriodoInventarioMensual = "M";
        public const string m_tipoPeriodoInventarioDiario = "D";

        public const string m_CostOrderSpecialList = "ProductionCostDataProcesarInventario_List";

        public const string m_plantaProcesoPersonKey = "PlantaProcesoKey";

        private const string ExcelXlsxMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private static OrdenamientoBodegaMotivo[] _ordenamientos;

        private static List<DataProcesarInventario> _ordenamientos2da;
        private static List<InventoryDetailCostTranferDto> _movimientosInventoryDetail;
        private static CosteoValidacionesExcepcionDto[] _validacionCosteoManualExcepcion;

        private static TestCostoSaldoMovimiento[] _costosMovimientosTest;
        private static List<InventoryMoveDetail> _inventoryMoveDetailTest = new List<InventoryMoveDetail>();


        [HttpPost]
        public PartialViewResult Index()
        {
            return this.PartialView();
        }

        #region Vista de consulta principal

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult CostProductValuationResults(int? id_documentState,
            string number, string reference, int? anio, int? mes, int? id_allocationType,
            DateTime? startEmissionDate, DateTime? endEmissionDate)
        {
            // Preparar el Query para los datos resultantes
            var resultsQuery = db.ProductionCostProductValuation
                .Include("Document")
                .Include("Document.DocumentState")
                .Include("ProductionCostAllocationType")
                .AsQueryable();

            // Aplicamos los filtros según los criterios recibidos
            if (id_documentState.HasValue)
            {
                resultsQuery = resultsQuery
                    .Where(i => i.Document.id_documentState == id_documentState.Value);
            }

            if (!String.IsNullOrWhiteSpace(number))
            {
                resultsQuery = resultsQuery
                    .Where(i => i.Document.number.Trim().Contains(number.Trim()));
            }

            if (!String.IsNullOrWhiteSpace(reference))
            {
                resultsQuery = resultsQuery
                    .Where(i => reference.Contains(i.Document.reference));
            }

            if (anio.HasValue)
            {
                resultsQuery = resultsQuery
                    .Where(i => i.anio == anio.Value);
            }

            if (mes.HasValue)
            {
                resultsQuery = resultsQuery
                    .Where(i => i.mes == mes.Value);
            }

            if (id_allocationType.HasValue)
            {
                resultsQuery = resultsQuery
                    .Where(i => i.id_allocationType == id_allocationType.Value);
            }

            if (startEmissionDate.HasValue)
            {
                resultsQuery = resultsQuery
                    .Where(i => DateTime.Compare(startEmissionDate.Value, i.Document.emissionDate) <= 0);
            }

            if (endEmissionDate.HasValue)
            {
                resultsQuery = resultsQuery
                    .Where(i => DateTime.Compare(i.Document.emissionDate, endEmissionDate.Value) <= 0);
            }

            var model = resultsQuery
                .OrderByDescending(i => i.id)
                .ToList();

            this.TempData[m_ResultQueryViewKeyName] = model;
            this.TempData.Keep(m_ResultQueryViewKeyName);

            return this.PartialView("_ProductValuationQueryResultsPartial", model);
        }

        [HttpPost]
        public ActionResult CostProductValuationPartial()
        {
            var model = (this.TempData[m_ResultQueryViewKeyName] as List<ProductionCostProductValuation>)
                ?? new List<ProductionCostProductValuation>();

            this.TempData.Keep(m_ResultQueryViewKeyName);

            return this.PartialView("_ProductValuationQueryGridViewPartial", model);
        }

        #endregion

        #region Vista de edición de transacción

        [HttpPost]
        public PartialViewResult EditForm(int? id, string successMessage)
        {
            this.TempData.Remove(m_CostProductValuationModelKey);

            var productValuation = this.GetEditingCostProductValuation(id);
            this.TempData[m_CostProductValuationModelKey] = productValuation;

            this.PrepareEditViewBag(productValuation);

            if (!String.IsNullOrWhiteSpace(successMessage))
            {
                this.ViewBag.EditMessage = this.SuccessMessage(successMessage);
            }

            return PartialView("_EditForm", productValuation);
        }


        [HttpPost]
        public JsonResult Create(
            int anio, int mes, DateTime emissionDate,
            DateTime? fechaInicio, DateTime? fechaFin, int? idProducto,
            int idAllocationType, string description, string reference)
        {
            int? idProductValuation;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperamos el tipo de documento
                    var documentType = db.DocumentType
                        .FirstOrDefault(dt => dt.code == m_TipoDocumentoCostProductValuation
                                            && dt.id_company == this.ActiveCompanyId);

                    if (documentType == null)
                    {
                        throw new ApplicationException("No existe registrado el tipo de documento: Valorización de Productos.");
                    }

                    // Si el costeo es real, modificamos los movimientos de inventario.
                    var tipoCostoReal = db.ProductionCostAllocationType.FirstOrDefault(e => e.code == "REAL");
                    if (tipoCostoReal == null)
                    {
                        throw new ApplicationException("No existe registrado el tipo de COSTO REAL para los documentos.");
                    }

                    // Recuperamos el estado PENDIENTE
                    var documentState = DataProviderCostProductValuation
                        .GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

                    if (documentState == null)
                    {
                        throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
                    }

                    // Verificar que no exista un control pendiente
                    var controlPendiente = db.ProductionCostProductValuationControl
                        .FirstOrDefault(e => e.año == anio
                            && e.mes == mes && e.id_allocationType == idAllocationType
                            && e.idEstado == documentState.id && e.activo);

                    if (controlPendiente != null)
                    {
                        var documentControl = db.Document.FirstOrDefault(e => e.id == controlPendiente.idProductValuation);
                        throw new ApplicationException("Existe una valorización de productos pendientes de aprobar al periodo. " +
                            $"N°: {documentControl.number}.");
                    }

                    // Generamos el secuencial y el número de documento correspondiente
                    var documentSequential = documentType.currentNumber;
                    var documentNumber = $"{this.ActiveEmissionPoint.BranchOffice.code:000}-{this.ActiveEmissionPoint.code:000}-{documentSequential}";

                    documentType.currentNumber += 1;

                    // Creamos el documento
                    var document = new Document()
                    {
                        number = documentNumber,
                        sequential = documentSequential,
                        emissionDate = emissionDate,
                        description = description,
                        reference = reference,
                        id_emissionPoint = this.ActiveEmissionPoint.id,
                        id_documentType = documentType.id,
                        id_documentState = documentState.id,

                        id_userCreate = this.ActiveUserId,
                        dateCreate = DateTime.Now,
                        id_userUpdate = this.ActiveUserId,
                        dateUpdate = DateTime.Now,
                    };

                    // Creamos el documento de valoración de costos
                    var productValuation = new ProductionCostProductValuation()
                    {
                        Document = document,

                        id_company = this.ActiveCompanyId,
                        id_allocationType = idAllocationType,
                        anio = anio,
                        mes = mes,

                        fechaInicio = fechaInicio,
                        fechaFin = fechaFin,
                        idProducto = idProducto,

                        processed = false,
                        description = description,

                        id_userCreate = this.ActiveUserId,
                        dateCreate = DateTime.Now,
                        id_userUpdate = this.ActiveUserId,
                        dateUpdate = DateTime.Now,
                    };

                    // Agregar los detalles de la valoración de costos
                    var productValuationTemp = (this.TempData[m_CostProductValuationModelKey] as ProductionCostProductValuation);

                    if ((productValuationTemp != null) && (productValuationTemp.processed))
                    {
                        productValuation.processed = true;

                        if (productValuationTemp.ProductionCostProductValuationExecutions != null)
                        {
                            productValuation.ProductionCostProductValuationExecutions = productValuationTemp
                                .ProductionCostProductValuationExecutions
                                .Where(d => d.isActive)
                                .Select(d => new ProductionCostProductValuationExecution()
                                {
                                    id_coefficientExecution = d.id_coefficientExecution,
                                    valor = d.valor,
                                    isActive = true,

                                    id_userCreate = this.ActiveUserId,
                                    dateCreate = DateTime.Now,
                                    id_userUpdate = this.ActiveUserId,
                                    dateUpdate = DateTime.Now,
                                })
                                .ToList();
                        }

                        // TODO: Agregar los otros detalles del documento...
                        if (productValuationTemp.ProductionCostProductValuationInventoryMove != null)
                        {
                            productValuation.ProductionCostProductValuationInventoryMove = productValuationTemp
                                .ProductionCostProductValuationInventoryMove
                                .Where(d => d.activo)
                                .Select(d => new ProductionCostProductValuationInventoryMove()
                                {
                                    orden = d.orden,
                                    ordenAsigCost = d.ordenAsigCost,
                                    idInventoryMove = d.idInventoryMove,
                                    idInventoryMoveDetail = d.idInventoryMoveDetail,
                                    idMotivo = d.idMotivo,
                                    idBodega = d.idBodega,
                                    idItem = d.idItem,
                                    naturaleza = d.naturaleza,
                                    esTransferencia = d.esTransferencia,
                                    id_lot = d.id_lot,
                                    valorizacion = d.valorizacion,
                                    tipoCalculo = d.tipoCalculo,
                                    accion = d.accion,
                                    costoTotalAnterior = d.costoTotalAnterior,
                                    costoUnitarioAnterior = d.costoUnitarioAnterior,

                                    coeficiente = d.coeficiente,
                                    cantidad = d.cantidad,
                                    costoTotal = d.costoTotal,
                                    costoUnitario = d.costoUnitario,
                                    activo = true,

                                    idUsuarioCreacion = this.ActiveUserId,
                                    fechaHoraCreacion = DateTime.Now,
                                    idUsuarioModificacion = this.ActiveUserId,
                                    fechaHoraModificacion = DateTime.Now,
                                })
                                .ToList();
                        }

                        if (productValuationTemp.ProductionCostProductValuationWarehouse != null)
                        {
                            productValuation.ProductionCostProductValuationWarehouse = productValuationTemp
                                .ProductionCostProductValuationWarehouse
                                .Where(d => d.isActive)
                                .Select(d => new ProductionCostProductValuationWarehouse()
                                {
                                    id_periodState = d.id_periodState,
                                    id_warehouse = d.id_warehouse,
                                    process = d.process,
                                    isActive = d.isActive,

                                    id_userCreate = this.ActiveUserId,
                                    dateCreate = DateTime.Now,
                                    id_userUpdate = this.ActiveUserId,
                                    dateUpdate = DateTime.Now,
                                })
                                .ToList();

                            // Recuperamos y actualizamos los periodos
                            if (productValuationTemp.id_allocationType == tipoCostoReal.id)
                            {
                                var idsBodega1 = productValuation
                                    .ProductionCostProductValuationWarehouse
                                    .Select(e => e.id_warehouse).Distinct().ToArray();

                                this.ActualizarEstadosPeriodoBodega("PV", anio, mes,
                                    fechaInicio, fechaFin, idsBodega1);
                            }
                        }

                        if (productValuationTemp.ProductionCostProductValuationBalance != null)
                        {
                            productValuation.ProductionCostProductValuationBalance = productValuationTemp
                                .ProductionCostProductValuationBalance
                                .Where(d => d.activo)
                                .Select(d => new ProductionCostProductValuationBalance()
                                {
                                    idBodega = d.idBodega,
                                    idItem = d.idItem,
                                    cantidad = d.cantidad,
                                    precioUnitario = d.precioUnitario,
                                    costoTotal = d.costoTotal,
                                    activo = d.activo,

                                    idUsuarioCreacion = this.ActiveUserId,
                                    fechaHoraCreacion = DateTime.Now,
                                    idUsuarioModificacion = this.ActiveUserId,
                                    fechaHoraModificacion = DateTime.Now,
                                })
                                .ToList();
                        }

                        if (productValuationTemp.ProductionCostProductValuationWarning != null)
                        {
                            productValuation.ProductionCostProductValuationWarning = productValuationTemp
                                .ProductionCostProductValuationWarning
                                .Where(d => d.activo)
                                .Select(d => new ProductionCostProductValuationWarning()
                                {
                                    orden = d.orden,
                                    descripcion = d.descripcion,
                                    activo = true,

                                    idUsuarioCreacion = this.ActiveUserId,
                                    fechaHoraCreacion = DateTime.Now,
                                    idUsuarioModificacion = this.ActiveUserId,
                                    fechaHoraModificacion = DateTime.Now,
                                })
                                .ToList();
                        }
                    }

                    // Guardamos el documento
                    db.Document.Add(document);
                    db.ProductionCostProductValuation.Add(productValuation);
                    db.SaveChanges();
                    transaction.Commit();

                    // Creamos el control
                    using (var transaction1 = db.Database.BeginTransaction())
                    {
                        var fechaInicioPeriodo = new DateTime(anio, mes, 1);
                        var fechaFinPeriodo = new DateTime(anio, mes, 1);
                        var control = new ProductionCostProductValuationControl()
                        {
                            año = anio,
                            mes = mes,
                            fechaInicioInteger = (fechaInicio ?? fechaInicioPeriodo).ToIntegerDate(),
                            fechaFinInteger = (fechaFin ?? fechaFinPeriodo).ToIntegerDate(),
                            id_allocationType = idAllocationType,
                            idEstado = documentState.id,
                            idProducto = idProducto,
                            idProductValuation = productValuation.id,
                            activo = true,

                            id_userCreate = this.ActiveUserId,
                            dateCreate = DateTime.Now,
                            id_userUpdate = this.ActiveUserId,
                            dateUpdate = DateTime.Now,
                        };

                        db.ProductionCostProductValuationControl.Add(control);
                        db.SaveChanges();
                        transaction1.Commit();
                    }

                    idProductValuation = productValuation.id;
                    message = "Documento creado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_CostProductValuationModelKey);

                    var exceptionMessage = exception.GetBaseException() != null
                        ? exception.GetBaseException().Message
                        : "No se ha podido recuperar los detalles del error";

                    idProductValuation = null;
                    message = "Error al crear documento: " + exceptionMessage;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductValuation,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Save(int idProductValuation,
            int anio, int mes, DateTime emissionDate,
            DateTime? fechaInicio, DateTime? fechaFin, int? idProducto,
            int idAllocationType, string description, string reference,
            bool approveDocument)
        {
            int? idProductValuationResult;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var productValuation = db.ProductionCostProductValuation
                        .First(c => c.id == idProductValuation);

                    // Verificamos el estado actual del documento
                    var documentStateCode = productValuation.Document.DocumentState.code;

                    if (documentStateCode != m_PendienteDocumentState)
                    {
                        throw new ApplicationException("Acción es permitida solo para Documento en estado PENDIENTE.");
                    }

                    // Si el costeo es real, modificamos los movimientos de inventario.
                    var tipoCostoReal = db.ProductionCostAllocationType.FirstOrDefault(e => e.code == "REAL");
                    if (tipoCostoReal == null)
                    {
                        throw new ApplicationException("No existe registrado el tipo de COSTO REAL para los documentos.");
                    }

                    // Actualizamos el documento
                    var document = productValuation.Document;

                    document.emissionDate = emissionDate;
                    document.description = description;
                    document.reference = reference;

                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;

                    // Actualizamos el documento de valoración de costos
                    productValuation.id_allocationType = idAllocationType;
                    productValuation.anio = anio;
                    productValuation.mes = mes;

                    productValuation.fechaInicio = fechaInicio;
                    productValuation.fechaFin = fechaFin;
                    productValuation.idProducto = idProducto;

                    productValuation.processed = false;
                    productValuation.description = description;

                    productValuation.id_userUpdate = this.ActiveUserId;
                    productValuation.dateUpdate = DateTime.Now;

                    // Procesamos la valorización para aprobar la transacción.
                    var productValuationTemp = (this.TempData[m_CostProductValuationModelKey] as ProductionCostProductValuation);
                    if (approveDocument)
                    {
                        var idsWarehouse = productValuationTemp
                            .ProductionCostProductValuationWarehouse
                            .Where(e => e.process)
                            .Select(e => e.id_warehouse)
                            .ToArray();

                        this.ProcesarProductionCostProductValuationWarninges(productValuationTemp, idsWarehouse);

                        /*
                            * 2023 08 03
                            * Temp, procesar por articulo
                        */
                        this.ProcesarValorizacionBodegas(productValuationTemp, fechaInicio, fechaFin, idProducto, idsWarehouse);
                        //this.ProcesarValorizacionBodegas(productValuationTemp, fechaInicio, fechaFin, 29046, idsWarehouse);

                        productValuationTemp.processed = true;
                    }

                    var tieneDetallesActivos = false;

                    if (productValuationTemp != null)
                    {
                        productValuation.processed = productValuationTemp.processed;

                        if ((productValuationTemp.ProductionCostProductValuationExecutions != null)
                            && (productValuationTemp.processed))
                        {
                            var productValuationExecutionsTemp = productValuationTemp
                                .ProductionCostProductValuationExecutions
                                .Where(d => d.isActive)
                                .ToList();

                            tieneDetallesActivos = tieneDetallesActivos
                                || productValuationExecutionsTemp.Any();

                            // Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
                            foreach (var detail in productValuation.ProductionCostProductValuationExecutions)
                            {
                                if (productValuationExecutionsTemp.Any())
                                {
                                    // Actualizamos los detalles
                                    var detailTemp = productValuationExecutionsTemp[0];
                                    productValuationExecutionsTemp.RemoveAt(0);

                                    detail.id_coefficientExecution = detailTemp.id_coefficientExecution;
                                    detail.valor = detailTemp.valor;
                                    detail.isActive = true;

                                    detail.id_userUpdate = this.ActiveUserId;
                                    detail.dateUpdate = DateTime.Now;
                                }
                                else
                                {
                                    // Ya no hay detalles nuevos, desactivar...
                                    detail.valor = 0;
                                    detail.isActive = false;

                                    detail.id_userUpdate = this.ActiveUserId;
                                    detail.dateUpdate = DateTime.Now;
                                }
                            }

                            // Agregamos los detalles que faltan de agregar
                            foreach (var detailTemp in productValuationExecutionsTemp)
                            {
                                productValuation.ProductionCostProductValuationExecutions
                                    .Add(new ProductionCostProductValuationExecution()
                                    {
                                        id_productValuation = idProductValuation,
                                        id_coefficientExecution = detailTemp.id_coefficientExecution,
                                        valor = detailTemp.valor,
                                        isActive = true,

                                        id_userCreate = this.ActiveUserId,
                                        dateCreate = DateTime.Now,
                                        id_userUpdate = this.ActiveUserId,
                                        dateUpdate = DateTime.Now,
                                    });
                            }
                        }
                        else
                        {
                            // No hay detalles: desactivar todos los elementos actuales
                            foreach (var detail in productValuation.ProductionCostProductValuationExecutions)
                            {
                                detail.valor = 0;
                                detail.isActive = false;

                                detail.id_userUpdate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                        }

                        if ((productValuationTemp.ProductionCostProductValuationWarehouse != null)
                            && (productValuationTemp.processed))
                        {
                            var productValuationExecutionsTemp = productValuationTemp
                                .ProductionCostProductValuationWarehouse
                                .Where(d => d.isActive)
                                .ToList();

                            tieneDetallesActivos = tieneDetallesActivos
                                || productValuationExecutionsTemp.Any();

                            // Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
                            foreach (var detail in productValuation.ProductionCostProductValuationWarehouse)
                            {
                                if (productValuationExecutionsTemp.Any())
                                {
                                    // Actualizamos los detalles
                                    var detailTemp = productValuationExecutionsTemp[0];
                                    productValuationExecutionsTemp.RemoveAt(0);

                                    detail.id_productValuation = idProductValuation;
                                    detail.id_periodState = detailTemp.id_periodState;
                                    detail.id_warehouse = detailTemp.id_warehouse;
                                    detail.process = detailTemp.process;
                                    detail.isActive = true;

                                    detail.id_userUpdate = this.ActiveUserId;
                                    detail.dateUpdate = DateTime.Now;
                                }
                                else
                                {
                                    // Ya no hay detalles nuevos, desactivar...
                                    detail.process = false;
                                    detail.isActive = false;

                                    detail.id_userUpdate = this.ActiveUserId;
                                    detail.dateUpdate = DateTime.Now;
                                }
                            }

                            // Agregamos los detalles que faltan de agregar
                            foreach (var detailTemp in productValuationExecutionsTemp)
                            {
                                productValuation.ProductionCostProductValuationWarehouse
                                    .Add(new ProductionCostProductValuationWarehouse()
                                    {
                                        id_productValuation = idProductValuation,
                                        id_periodState = detailTemp.id_periodState,
                                        id_warehouse = detailTemp.id_warehouse,
                                        process = detailTemp.process,
                                        isActive = true,

                                        id_userCreate = this.ActiveUserId,
                                        dateCreate = DateTime.Now,
                                        id_userUpdate = this.ActiveUserId,
                                        dateUpdate = DateTime.Now,
                                    });
                            }
                        }
                        else
                        {
                            // No hay detalles: desactivar todos los elementos actuales
                            foreach (var detail in productValuation.ProductionCostProductValuationWarehouse)
                            {
                                detail.process = false;
                                detail.isActive = false;

                                detail.id_userUpdate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                        }


                        if ((productValuationTemp.ProductionCostProductValuationInventoryMove != null)
                            && (productValuationTemp.processed))
                        {
                            var productValuationInventoriesMoveTemp = productValuationTemp
                                .ProductionCostProductValuationInventoryMove
                                .Where(d => d.activo)
                                .ToList();

                            tieneDetallesActivos = tieneDetallesActivos
                                || productValuationInventoriesMoveTemp.Any();

                            // Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
                            foreach (var detail in productValuation.ProductionCostProductValuationInventoryMove)
                            {
                                if (productValuationInventoriesMoveTemp.Any())
                                {
                                    // Actualizamos los detalles
                                    var detailTemp = productValuationInventoriesMoveTemp[0];
                                    productValuationInventoriesMoveTemp.RemoveAt(0);

                                    detail.orden = detailTemp.orden;
                                    detail.ordenAsigCost = detailTemp.ordenAsigCost;
                                    detail.idInventoryMove = detailTemp.idInventoryMove;
                                    detail.idInventoryMoveDetail = detailTemp.idInventoryMoveDetail;
                                    detail.idMotivo = detailTemp.idMotivo;
                                    detail.idBodega = detailTemp.idBodega;
                                    detail.idItem = detailTemp.idItem;
                                    detail.naturaleza = detailTemp.naturaleza;
                                    detail.esTransferencia = detailTemp.esTransferencia;
                                    detail.valorizacion = detailTemp.valorizacion;
                                    detail.tipoCalculo = detailTemp.tipoCalculo;
                                    detail.accion = detailTemp.accion;
                                    detail.id_lot = detailTemp.id_lot;
                                    detail.cantidad = detailTemp.cantidad;
                                    detail.costoTotalAnterior = detailTemp.costoTotalAnterior;
                                    detail.costoUnitarioAnterior = detailTemp.costoUnitarioAnterior;
                                    detail.coeficiente = detailTemp.coeficiente;
                                    detail.costoTotal = detailTemp.costoTotal;
                                    detail.costoUnitario = detailTemp.costoUnitario;
                                    detail.activo = true;

                                    detail.idUsuarioModificacion = this.ActiveUserId;
                                    detail.fechaHoraModificacion = DateTime.Now;
                                }
                                else
                                {
                                    // Ya no hay detalles nuevos, desactivar...
                                    detail.orden = 0;
                                    detail.coeficiente = null;
                                    detail.costoTotalAnterior = 0m;
                                    detail.costoUnitarioAnterior = 0m;
                                    detail.costoTotal = 0m;
                                    detail.costoUnitario = 0m;
                                    detail.activo = false;

                                    detail.idUsuarioModificacion = this.ActiveUserId;
                                    detail.fechaHoraModificacion = DateTime.Now;
                                }
                            }

                            // Agregamos los detalles que faltan de agregar
                            foreach (var detailTemp in productValuationInventoriesMoveTemp)
                            {
                                productValuation.ProductionCostProductValuationInventoryMove
                                    .Add(new ProductionCostProductValuationInventoryMove()
                                    {
                                        orden = detailTemp.orden,
                                        ordenAsigCost = detailTemp.ordenAsigCost,
                                        id_productValuation = idProductValuation,
                                        idInventoryMove = detailTemp.idInventoryMove,
                                        idInventoryMoveDetail = detailTemp.idInventoryMoveDetail,
                                        idMotivo = detailTemp.idMotivo,
                                        idBodega = detailTemp.idBodega,
                                        idItem = detailTemp.idItem,
                                        naturaleza = detailTemp.naturaleza,
                                        esTransferencia = detailTemp.esTransferencia,
                                        valorizacion = detailTemp.valorizacion,
                                        tipoCalculo = detailTemp.tipoCalculo,
                                        accion = detailTemp.accion,
                                        id_lot = detailTemp.id_lot,
                                        cantidad = detailTemp.cantidad,
                                        costoTotalAnterior = detailTemp.costoTotalAnterior,
                                        costoUnitarioAnterior = detailTemp.costoUnitarioAnterior,
                                        coeficiente = detailTemp.coeficiente,
                                        costoTotal = detailTemp.costoTotal,
                                        costoUnitario = detailTemp.costoUnitario,
                                        activo = true,

                                        idUsuarioCreacion = this.ActiveUserId,
                                        fechaHoraCreacion = DateTime.Now,
                                        idUsuarioModificacion = this.ActiveUserId,
                                        fechaHoraModificacion = DateTime.Now,
                                    });
                            }
                        }
                        else
                        {
                            // No hay detalles: desactivar todos los elementos actuales

                            foreach (var detail in productValuation.ProductionCostProductValuationInventoryMove)
                            {
                                // Ya no hay detalles nuevos, desactivar...
                                detail.orden = 0;
                                detail.costoTotalAnterior = 0m;
                                detail.costoUnitarioAnterior = 0m;
                                detail.coeficiente = null;
                                detail.cantidad = 0m;
                                detail.costoTotal = 0m;
                                detail.costoUnitario = 0m;
                                detail.activo = false;

                                detail.idUsuarioModificacion = this.ActiveUserId;
                                detail.fechaHoraModificacion = DateTime.Now;
                            }
                        }


                        if ((productValuationTemp.ProductionCostProductValuationBalance != null)
                            && (productValuationTemp.processed))
                        {
                            var productValuationInventoriesMoveTemp = productValuationTemp
                                .ProductionCostProductValuationBalance
                                .Where(d => d.activo)
                                .ToList();

                            tieneDetallesActivos = tieneDetallesActivos
                                || productValuationInventoriesMoveTemp.Any();

                            // Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
                            foreach (var detail in productValuation.ProductionCostProductValuationBalance)
                            {
                                if (productValuationInventoriesMoveTemp.Any())
                                {
                                    // Actualizamos los detalles
                                    var detailTemp = productValuationInventoriesMoveTemp[0];
                                    productValuationInventoriesMoveTemp.RemoveAt(0);

                                    detail.idBodega = detailTemp.idBodega;
                                    detail.idItem = detailTemp.idItem;
                                    detail.cantidad = detailTemp.cantidad;
                                    detail.precioUnitario = detailTemp.precioUnitario;
                                    detail.costoTotal = detailTemp.costoTotal;
                                    detail.activo = true;

                                    detail.id_personProcessPlant = detailTemp.id_personProcessPlant;

                                    detail.idUsuarioModificacion = this.ActiveUserId;
                                    detail.fechaHoraModificacion = DateTime.Now;
                                }
                                else
                                {
                                    // Ya no hay detalles nuevos, desactivar...
                                    detail.idBodega = 0;
                                    detail.idItem = 0;
                                    detail.cantidad = 0;
                                    detail.costoTotal = 0m;
                                    detail.precioUnitario = 0m;
                                    detail.activo = false;

                                    detail.idUsuarioModificacion = this.ActiveUserId;
                                    detail.fechaHoraModificacion = DateTime.Now;
                                }
                            }

                            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
                            SaldoInicial[] saldoInicialTemporal = null;
                            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                            {
                                saldoInicialTemporal = getSaldoInicialTemporal(0);
                            }
                            // Agregamos los detalles que faltan de agregar
                            foreach (var detailTemp in productValuationInventoriesMoveTemp)
                            {
                                productValuation.ProductionCostProductValuationBalance
                                    .Add(new ProductionCostProductValuationBalance()
                                    {
                                        id_productValuation = idProductValuation,
                                        idBodega = detailTemp.idBodega,
                                        idItem = detailTemp.idItem,
                                        cantidad = detailTemp.cantidad,
                                        precioUnitario = detailTemp.precioUnitario,
                                        costoTotal = detailTemp.costoTotal,
                                        activo = true,

                                        idUsuarioCreacion = this.ActiveUserId,
                                        fechaHoraCreacion = DateTime.Now,
                                        idUsuarioModificacion = this.ActiveUserId,
                                        fechaHoraModificacion = DateTime.Now,
                                        id_personProcessPlant = parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso) ? (saldoInicialTemporal.FirstOrDefault(r => r.idItem == detailTemp.idItem)?.id_personProcessPlant) : null
                                    });
                            }
                        }
                        else
                        {
                            // No hay detalles: desactivar todos los elementos actuales

                            foreach (var detail in productValuation.ProductionCostProductValuationBalance)
                            {
                                // Ya no hay detalles nuevos, desactivar...
                                detail.idBodega = 0;
                                detail.idItem = 0;
                                detail.cantidad = 0;
                                detail.costoTotal = 0m;
                                detail.precioUnitario = 0m;
                                detail.activo = false;

                                detail.idUsuarioModificacion = this.ActiveUserId;
                                detail.fechaHoraModificacion = DateTime.Now;
                            }
                        }


                        if ((productValuationTemp.ProductionCostProductValuationWarning != null)
                            && (productValuationTemp.processed))
                        {
                            var productionCostProductValuationWarningTemp = productValuationTemp
                                .ProductionCostProductValuationWarning
                                .Where(d => d.activo)
                                .ToList();

                            tieneDetallesActivos = tieneDetallesActivos
                                || productionCostProductValuationWarningTemp.Any();

                            // Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
                            foreach (var detail in productValuation.ProductionCostProductValuationWarning)
                            {
                                if (productionCostProductValuationWarningTemp.Any())
                                {
                                    // Actualizamos los detalles
                                    var detailTemp = productionCostProductValuationWarningTemp[0];
                                    productionCostProductValuationWarningTemp.RemoveAt(0);

                                    detail.orden = detailTemp.orden;
                                    detail.descripcion = detailTemp.descripcion;
                                    detail.activo = true;

                                    detail.idUsuarioModificacion = this.ActiveUserId;
                                    detail.fechaHoraModificacion = DateTime.Now;
                                }
                                else
                                {
                                    // Ya no hay detalles nuevos, desactivar...
                                    detail.descripcion = "";
                                    detail.activo = false;

                                    detail.idUsuarioModificacion = this.ActiveUserId;
                                    detail.fechaHoraModificacion = DateTime.Now;
                                }
                            }

                            // Agregamos los detalles que faltan de agregar
                            foreach (var detailTemp in productionCostProductValuationWarningTemp)
                            {
                                productValuation.ProductionCostProductValuationWarning
                                    .Add(new ProductionCostProductValuationWarning()
                                    {
                                        id_productValuation = idProductValuation,
                                        orden = detailTemp.orden,
                                        descripcion = detailTemp.descripcion,
                                        activo = true,

                                        idUsuarioCreacion = this.ActiveUserId,
                                        fechaHoraCreacion = DateTime.Now,
                                        idUsuarioModificacion = this.ActiveUserId,
                                        fechaHoraModificacion = DateTime.Now,
                                    });
                            }
                        }
                        else
                        {
                            // No hay detalles: desactivar todos los elementos actuales

                            foreach (var detail in productValuation.ProductionCostProductValuationWarning)
                            {
                                // Ya no hay detalles nuevos, desactivar...
                                detail.descripcion = "";
                                detail.activo = false;

                                detail.idUsuarioModificacion = this.ActiveUserId;
                                detail.fechaHoraModificacion = DateTime.Now;
                            }
                        }

                        // Afectamos inventario
                        if (approveDocument && productValuationTemp.processed)
                        {
                            // Solo se aprueba si se tienen detalles
                            if (!tieneDetallesActivos)
                            {
                                throw new ApplicationException("No se puede aprobar el documento sin detalles.");
                            }

                            var novedades = productValuationTemp
                                .ProductionCostProductValuationWarning
                                .Where(e => e.activo);

                            //TODO: agregar validación de los otros indicadores de detalles
                            if (novedades.Any() && productValuation.id_allocationType == tipoCostoReal.id)
                            {
                                throw new ApplicationException("No se puede aprobar el documento, tiene novedades sin resolver.");
                            }

                            // Recuperamos el estado APROBADO
                            var documentState = DataProviderCostProductValuation
                                .GetDocumentStateByCode(this.ActiveCompanyId, m_AprobadoDocumentState);

                            if (documentState == null)
                            {
                                throw new ApplicationException("No existe registrado el estado APROBADO para los documentos.");
                            }

                            document.id_documentState = documentState.id;

                            if (productValuation.id_allocationType == tipoCostoReal.id)
                            {
                                var productionProductValuationInventoryMoves = productValuation
                                    .ProductionCostProductValuationInventoryMove
                                    .Where(e => e.activo)
                                    .ToArray();

                                string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                                using (var db = new System.Data.SqlClient.SqlConnection(dapperDBContext))
                                {
                                    db.Open();

                                    using (var tr = db.BeginTransaction())
                                    {
                                        try
                                        {
                                            // Procesamos los movimientos de inventarios
                                            foreach (var productionProductValuationInventoryMove in productionProductValuationInventoryMoves)
                                            {
                                                decimal entryAmountCost = 0m, exitAmountCost = 0m;
                                                if (productionProductValuationInventoryMove.naturaleza == m_NaturalezaIngresoKey)
                                                {
                                                    entryAmountCost = productionProductValuationInventoryMove.costoTotal;
                                                }
                                                else if (productionProductValuationInventoryMove.naturaleza == m_NaturalezaEgresoKey)
                                                {
                                                    exitAmountCost = productionProductValuationInventoryMove.costoTotal;
                                                }

                                                InventoryMoveDetailDapper.ActualizarCostos(db, tr,
                                                    productionProductValuationInventoryMove.idInventoryMoveDetail, entryAmountCost,
                                                    exitAmountCost, productionProductValuationInventoryMove.costoUnitario,
                                                    this.ActiveUserId, DateTime.Now);
                                            }

                                            tr.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            tr.Rollback();
                                            throw ex;
                                        }
                                    }

                                    db.Close();
                                }
                            }
                        }

                        // Afectamos los balances procesados
                        if (approveDocument && productValuationTemp.processed)
                        {
                            // Insertamos los saldos nuevos
                            var productionProductValuationBalances = productValuation
                                .ProductionCostProductValuationBalance
                                .Where(e => e.activo)
                                .ToArray();

                            // Procesamos los saldos resultantes
                            var saldosIniciales = productionProductValuationBalances
                                .Select(e => new SaldoInicial()
                                {
                                    id = idProductValuation,
                                    fechaCorteInteger = productValuation.fechaFin.ToIntegerDate() ?? 0,
                                    id_allocationType = productValuation.id_allocationType,
                                    idBodega = e.idBodega,
                                    idItem = e.idItem,
                                    cantidad = e.cantidad,
                                    costo_unitario = e.precioUnitario,
                                    costo_total = e.costoTotal,
                                    activo = e.activo,


                                    idUsuarioCreacion = this.ActiveUserId,
                                    fechaCreacion = DateTime.Now,
                                    idUsuarioModificacion = this.ActiveUserId,
                                    fechaModificacion = DateTime.Now,
                                    id_personProcessPlant = e.id_personProcessPlant
                                });



                            string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                            using (var db = new System.Data.SqlClient.SqlConnection(dapperDBContext))
                            {
                                db.Open();

                                using (var tr = db.BeginTransaction())
                                {
                                    try
                                    {
                                        //this.db.SaldoInicial.AddRange(saldosIniciales);
                                        registerSaldoFinal(saldosIniciales.ToArray(), db, tr);

                                        tr.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        tr.Rollback();
                                        throw ex;
                                    }
                                }

                                db.Close();
                            }


                            // Recuperamos y actualizamos los periodos
                            if (productValuationTemp.id_allocationType == tipoCostoReal.id)
                            {
                                var idsBodega1 = productValuation
                                    .ProductionCostProductValuationWarehouse
                                    .Select(e => e.id_warehouse).Distinct().ToArray();

                                this.ActualizarEstadosPeriodoBodega("VA", anio, mes,
                                    fechaInicio, fechaFin, idsBodega1);
                            }
                        }
                    }

                    var control = db.ProductionCostProductValuationControl
                        .FirstOrDefault(e => e.idProductValuation == productValuation.id);
                    if (control != null)
                    {
                        var fechaInicioPeriodo = new DateTime(anio, mes, 1);
                        var fechaFinPeriodo = new DateTime(anio, mes, 1);

                        control.año = anio;
                        control.mes = mes;
                        control.fechaInicioInteger = (fechaInicio ?? fechaInicioPeriodo).ToIntegerDate();
                        control.fechaFinInteger = (fechaFin ?? fechaFinPeriodo).ToIntegerDate();
                        control.idProducto = idProducto;
                        control.idEstado = document.id_documentState;

                        control.id_userUpdate = this.ActiveUserId;
                        control.dateUpdate = DateTime.Now;

                        db.ProductionCostProductValuationControl.Attach(control);
                        db.Entry(control).State = EntityState.Modified;
                    }

                    // Guardamos en la base de datos
                    db.ProductionCostProductValuation.Attach(productValuation);
                    db.Entry(productValuation).State = EntityState.Modified;

                    // Guardamos el documento
                    db.SaveChanges();
                    transaction.Commit();

                    idProductValuationResult = productValuation.id;
                    message = approveDocument
                        ? "Documento aprobado exitosamente."
                        : "Documento actualizado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_CostProductValuationModelKey);

                    var exceptionMessage = exception.GetBaseException() != null
                        ? exception.GetBaseException().Message
                        : "No se ha podido recuperar los detalles del error";

                    idProductValuationResult = null;
                    message = approveDocument
                        ? "Error al aprobar documento: " + exceptionMessage
                        : "Error al actualizar documento: " + exceptionMessage;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductValuation = idProductValuationResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Cancel(int idProductValuation)
        {
            int? idProductValuationResult;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var productValuation = db.ProductionCostProductValuation
                        .First(c => c.id == idProductValuation);

                    // Verificamos el estado actual del documento
                    var documentStateCode = productValuation.Document.DocumentState.code;

                    if (documentStateCode != m_PendienteDocumentState)
                    {
                        throw new ApplicationException("Acción es permitida solo para Documento en estado PENDIENTE.");
                    }

                    // Recuperamos el estado ANULADO
                    var documentState = DataProviderCostProductValuation
                        .GetDocumentStateByCode(this.ActiveCompanyId, m_AnuladoDocumentState);

                    if (documentState == null)
                    {
                        throw new ApplicationException("No existe registrado el estado CANCELADO para los documentos.");
                    }

                    var control = db.ProductionCostProductValuationControl
                        .FirstOrDefault(e => e.idProductValuation == productValuation.id);
                    if (control != null)
                    {
                        control.idEstado = documentState.id;
                        control.id_userUpdate = this.ActiveUserId;
                        control.dateUpdate = DateTime.Now;

                        db.ProductionCostProductValuationControl.Attach(control);
                        db.Entry(control).State = EntityState.Modified;
                    }


                    // Si el costeo es real, modificamos los movimientos de inventario.
                    var tipoCostoReal = db.ProductionCostAllocationType.FirstOrDefault(e => e.code == "REAL");
                    if (tipoCostoReal == null)
                    {
                        throw new ApplicationException("No existe registrado el tipo de COSTO REAL para los documentos.");
                    }

                    if (productValuation.id_allocationType == tipoCostoReal.id)
                    {
                        var idsBodega1 = productValuation
                            .ProductionCostProductValuationWarehouse
                            .Select(e => e.id_warehouse).Distinct().ToArray();

                        this.ActualizarEstadosPeriodoBodega("C", productValuation.anio, productValuation.mes,
                            productValuation.fechaInicio, productValuation.fechaFin, idsBodega1);
                    }

                    // Actualizamos el documento
                    var document = productValuation.Document;
                    document.id_documentState = documentState.id;

                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;

                    // Anulamos el documento
                    db.SaveChanges();
                    transaction.Commit();

                    idProductValuationResult = productValuation.id;
                    message = "Documento anulado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_CostProductValuationModelKey);

                    idProductValuationResult = null;
                    message = "Error al anular documento: " + exception.Message;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductValuation = idProductValuationResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Revert(int idProductValuation)
        {
            int? idProductValuationResult;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var productValuation = db.ProductionCostProductValuation
                        .First(c => c.id == idProductValuation);

                    // Verificamos el estado actual del documento
                    var documentStateCode = productValuation.Document.DocumentState.code;

                    if (documentStateCode != m_AprobadoDocumentState)
                    {
                        throw new ApplicationException("Acción es permitida solo para Documento en estado APROBADO.");
                    }

                    // Recuperamos el estado PENDIENTE
                    var documentState = DataProviderCostProductValuation
                        .GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

                    if (documentState == null)
                    {
                        throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
                    }


                    var controlPendiente = db.ProductionCostProductValuationControl
                        .FirstOrDefault(e => e.año == productValuation.anio
                            && e.mes == productValuation.mes && e.id_allocationType == productValuation.id_allocationType
                            && e.idProductValuation != productValuation.id && e.idEstado == documentState.id);

                    if (controlPendiente != null)
                    {
                        var documento = db.Document.FirstOrDefault(e => e.id == controlPendiente.idProductValuation);
                        throw new Exception($"Ya existe una valorización pendiente. N°: {documento.number}.");
                    }

                    // Actualizamos el documento
                    var document = productValuation.Document;

                    document.id_documentState = documentState.id;
                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;


                    var control = db.ProductionCostProductValuationControl
                        .FirstOrDefault(e => e.idProductValuation == productValuation.id);
                    if (control != null)
                    {
                        control.idEstado = documentState.id;
                        control.id_userUpdate = this.ActiveUserId;
                        control.dateUpdate = DateTime.Now;

                        db.ProductionCostProductValuationControl.Attach(control);
                        db.Entry(control).State = EntityState.Modified;
                    }

                    // Si el costeo es real, modificamos los movimientos de inventario.
                    var tipoCostoReal = db.ProductionCostAllocationType.FirstOrDefault(e => e.code == "REAL");
                    if (tipoCostoReal == null)
                    {
                        throw new ApplicationException("No existe registrado el tipo de COSTO REAL para los documentos.");
                    }

                    if (productValuation.id_allocationType == tipoCostoReal.id)
                    {
                        var productionProductValuationInventoryMoves = productValuation
                            .ProductionCostProductValuationInventoryMove
                            .Where(e => e.activo)
                            .ToArray();

                        // Procesamos los detalles de movimientos de inventarios
                        string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                        using (var db = new System.Data.SqlClient.SqlConnection(dapperDBContext))
                        {
                            db.Open();

                            using (var tr = db.BeginTransaction())
                            {
                                try
                                {
                                    foreach (var productionProductValuationInventoryMove in productionProductValuationInventoryMoves)
                                    {
                                        decimal entryAmountCost = 0m, exitAmountCost = 0m;
                                        if (productionProductValuationInventoryMove.naturaleza == m_NaturalezaIngresoKey)
                                        {
                                            entryAmountCost = productionProductValuationInventoryMove.costoTotalAnterior;
                                        }
                                        else if (productionProductValuationInventoryMove.naturaleza == m_NaturalezaEgresoKey)
                                        {
                                            exitAmountCost = productionProductValuationInventoryMove.costoTotalAnterior;
                                        }

                                        InventoryMoveDetailDapper.ActualizarCostos(db, tr,
                                            productionProductValuationInventoryMove.idInventoryMoveDetail, entryAmountCost,
                                            exitAmountCost, productionProductValuationInventoryMove.costoUnitarioAnterior,
                                            this.ActiveUserId, DateTime.Now);
                                    }

                                    tr.Commit();
                                }
                                catch (Exception ex)
                                {
                                    tr.Rollback();
                                    throw ex;
                                }
                            }

                            db.Close();
                        }


                        // Recuperamos y actualizamos los periodos
                        if (productValuation.id_allocationType == tipoCostoReal.id)
                        {
                            var idsBodega1 = productValuation
                                .ProductionCostProductValuationWarehouse
                                .Select(e => e.id_warehouse).Distinct().ToArray();

                            this.ActualizarEstadosPeriodoBodega("PV", productValuation.anio, productValuation.mes,
                                productValuation.fechaInicio, productValuation.fechaFin, idsBodega1);
                        }
                    }

                    // procesamos los detalles de saldos
                    var fechaFinInteger = productValuation.fechaFin.HasValue
                        ? productValuation.fechaFin.Value.ToIntegerDate()
                        : 0;

                    var saldos = db.SaldoInicial.Where(e => e.fechaCorteInteger == fechaFinInteger);
                    db.SaldoInicial.RemoveRange(saldos);

                    // Reversamos el documento
                    db.SaveChanges();
                    transaction.Commit();

                    idProductValuationResult = productValuation.id;
                    message = "Documento reversado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_CostProductValuationModelKey);

                    idProductValuationResult = null;
                    message = "Error al reversar documento: " + exception.Message;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductValuation = idProductValuationResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Execute(int idProductValuation,
            int anio, int mes, int idAllocationType)
        {
            int? idProductValuationResult;
            string message;
            bool isValid;

            try
            {
                var productValuation = this.GetEditingCostProductValuation(idProductValuation);

                productValuation.id_allocationType = idAllocationType;
                productValuation.anio = anio;
                productValuation.mes = mes;

                // Calcular los coeficientes...
                this.CalcularDetallesEjecucionCoeficientes(productValuation);

                this.TempData[m_CostProductValuationModelKey] = productValuation;
                this.TempData.Keep(m_CostProductValuationModelKey);

                idProductValuationResult = productValuation.id;
                message = "Documento procesado exitosamente.";
                isValid = true;
            }
            catch (Exception exception)
            {
                this.TempData.Keep(m_CostProductValuationModelKey);

                idProductValuationResult = null;
                message = "Error al procesar documento: " + exception.Message;
                isValid = false;
            }

            var result = new
            {
                idProductValuation = idProductValuationResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Reset(int idProductValuation)
        {
            int? idProductValuationResult;
            string message;
            bool isValid;

            try
            {
                var productValuation = this.GetEditingCostProductValuation(idProductValuation);

                // Eliminar los detalles y marcar como no procesado...
                productValuation.processed = false;

                // TODO: dcddddd
                productValuation.ProductionCostProductValuationExecutions = new List<ProductionCostProductValuationExecution>();
                productValuation.ProductionCostProductValuationWarehouse = new List<ProductionCostProductValuationWarehouse>();
                productValuation.ProductionCostProductValuationInventoryMove = new List<ProductionCostProductValuationInventoryMove>();
                productValuation.ProductionCostProductValuationWarning = new List<ProductionCostProductValuationWarning>();
                productValuation.ProductionCostProductValuationBalance = new List<ProductionCostProductValuationBalance>();

                this.TempData[m_CostProductValuationModelKey] = productValuation;
                this.TempData.Keep(m_CostProductValuationModelKey);

                idProductValuationResult = productValuation.id;
                message = "Documento restablecido exitosamente.";
                isValid = true;
            }
            catch (Exception exception)
            {
                this.TempData.Keep(m_CostProductValuationModelKey);

                idProductValuationResult = null;
                message = "Error al restablecer documento: " + exception.Message;
                isValid = false;
            }

            var result = new
            {
                idProductValuation = idProductValuationResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Valorize(int idProductValuation,
            DateTime? fechaInicio, DateTime? fechaFin, int? idProducto,
            int[] warehouseIds)
        {
            int? idProductValuationResult;
            string message;
            bool isValid;

            try
            {
                var productValuation = this.GetEditingCostProductValuation(idProductValuation);
                productValuation.fechaInicio = fechaInicio;
                productValuation.fechaFin = fechaFin;

                
                // Limpiamos la data temporal antes usada
                this.ClearTempData();

                // Revisamos en búsqueda de novedades
                this.ProcesarProductionCostProductValuationWarninges(productValuation, warehouseIds);

                this.ProcesarValorizacionBodegas(productValuation, fechaInicio, fechaFin, idProducto, warehouseIds);

                this.TempData[m_CostProductValuationModelKey] = productValuation;
                this.TempData.Keep(m_CostProductValuationModelKey);

                this.TempData[m_CostProductValuationExcelModelKey] = productValuation;
                this.TempData.Keep(m_CostProductValuationExcelModelKey);

                idProductValuationResult = productValuation.id;
                message = "Valorización calculada exitosamente.";
                isValid = true;
            }
            catch (Exception exception)
            {
                this.TempData.Keep(m_CostProductValuationModelKey);

                idProductValuationResult = null;
                message = "Error al calcular la valorización: " + exception.GetBaseException()?.Message ?? string.Empty;
                isValid = false;
            }

            var result = new
            {
                idProductValuation = idProductValuationResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Manejadores de los Detalles de la Ejecución

        public PartialViewResult CostProductValuationDetails(int idProductValuation)
        {
            var productValuation = this.GetEditingCostProductValuation(idProductValuation);

            this.TempData[m_CostProductValuationModelKey] = productValuation;
            this.TempData.Keep(m_CostProductValuationModelKey);
            this.PrepareDetailsEditViewBag(true, productValuation.processed);

            return this.PartialView("_ProductValuationDetailsPartial", productValuation);
        }

        #endregion

        #region Métodos para la generación de novedades
        public class ProductionCostProductValuationWarningDTO
        {
            public int Orden { get; set; }
            public string Descripcion { get; set; }
        }

        public static ProductionCostProductValuationWarningDTO[] ConvertToDTO(IList<ProductionCostProductValuationWarning> advertencias)
        {
            return advertencias
                .Select(e => new ProductionCostProductValuationWarningDTO()
                {
                    Orden = e.orden,
                    Descripcion = e.descripcion,
                })
                .ToArray();
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult QueryProductionCostProductValuationWarningValorizacionDetails(int? idProductValuation)
        {
            var productValuation = this.GetEditingCostProductValuation(idProductValuation);

            this.TempData.Keep(m_CostOrderSpecialList);
            this.TempData.Keep(m_CostProductValuationExcelModelKey);


            this.TempData[m_CostProductValuationModelKey] = productValuation;
            this.TempData.Keep(m_CostProductValuationModelKey);

            var novedades = productValuation.ProductionCostProductValuationWarning.Where(e => e.activo).ToArray();
            return this.PartialView("_ProductValuationWarningGridViewPartial", novedades);
        }

        private void ProcesarProductionCostProductValuationWarninges(ProductionCostProductValuation costValuation, int[] warehouseIds)
        {
            var novedades = new List<ProductionCostProductValuationWarning>();
            int ordenProductionCostProductValuationWarning = 1;

            var idTipoAsignacionCostoProyectado = this.db.ProductionCostAllocationType
                                                                    .FirstOrDefault(t => t.code == "REAL")?
                                                                    .id ?? 0;

            #region Si el costo es real, validamos que las bodegas estén cerradas
            if (costValuation.id_allocationType == idTipoAsignacionCostoProyectado)
            {
                var idCompany = this.ActiveCompanyId;
                var idDivision = this.ActiveDivision.id;
                var idSucursal = this.ActiveSucursal.id;

                var año = costValuation.anio;
                var mes = costValuation.mes;
                var idParametro = this.db.AdvanceParameters.FirstOrDefault(e => e.code == "EPIV1")?.id;
                var idParametroDetalleCierre = this.db.AdvanceParametersDetail
                                                            .FirstOrDefault(e => e.id == idParametro && e.valueCode.Trim() == "C")?
                                                            .id;

                if (!idParametroDetalleCierre.HasValue)
                {
                    throw new Exception("No se ha encontrado el estado Cerrado para los periodos de inventarios.");
                }

                for (int i = 0; i < warehouseIds.Length; i++)
                {
                    var idWarehouse = warehouseIds[i];
                    var detallePeriodo = db.InventoryPeriodDetail
                        .FirstOrDefault(d => d.InventoryPeriod.id_warehouse == idWarehouse
                            && d.InventoryPeriod.year == año
                            && d.periodNumber == mes
                            && d.InventoryPeriod.id_Company == idCompany
                            && d.InventoryPeriod.id_Division == idDivision
                            && d.InventoryPeriod.id_BranchOffice == idSucursal
                            && d.InventoryPeriod.isActive);

                    var bodega = costValuation
                        .ProductionCostProductValuationWarehouse
                        .FirstOrDefault(e => e.id_warehouse == idWarehouse)
                        .Warehouse;

                    if (detallePeriodo?.id_PeriodState != idParametroDetalleCierre)
                    {
                        novedades.Add(new ProductionCostProductValuationWarning()
                        {
                            orden = ordenProductionCostProductValuationWarning++,
                            descripcion = $"La bodega {bodega.name} no se encuentra cerrada.",

                            idUsuarioCreacion = this.ActiveUserId,
                            fechaHoraCreacion = DateTime.Now,
                            idUsuarioModificacion = this.ActiveUserId,
                            fechaHoraModificacion = DateTime.Now,
                        });
                    }
                }
            }
            #endregion


            //TODO: Validar que existen materia Prima para Motivo Ingreso Planta, y materia prima para Masterizado Sobrante y Masterizado Resultante
            //

            // Guardamos las novedades
            costValuation.ProductionCostProductValuationWarning = novedades;
            this.TempData[m_CostProductValuationModelKey] = costValuation;
            this.TempData.Keep(m_CostProductValuationModelKey);
        }

        #endregion

        #region Procesamiento de saldos
        public class ProductionCostProductValuationSaldoDTO
        {
            public int Orden { get; set; }
            public string NombreBodega { get; set; }
            public string CodigoNombreItem { get; set; }

            public decimal Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal CostoTotal { get; set; }

            public decimal? CantidadLibras { get; set; }
            public decimal? PrecioUnitarioLibras { get; set; }
            public decimal? CostoTotalLibras { get; set; }

            public decimal? CantidadKilogramos { get; set; }
            public decimal? PrecioUnitarioKilogramos { get; set; }
            public decimal? CostoTotalKilogramos { get; set; }
        }


        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult QueryProductionCostProductValuationBalanceValorizacionDetails(int? idProductValuation)
        {
            var productValuation = this.GetEditingCostProductValuation(idProductValuation);

            this.TempData.Keep(m_CostOrderSpecialList);
            this.TempData.Keep(m_CostProductValuationExcelModelKey);

            this.TempData[m_CostProductValuationModelKey] = productValuation;
            this.TempData.Keep(m_CostProductValuationModelKey);

            var saldos = productValuation.ProductionCostProductValuationBalance.Where(e => e.activo).ToArray();
            return this.PartialView("_ProductValuationBalanceGridViewPartial", saldos);
        }


        private void SaveProductValuationBalanceDTOTempData(ProductionCostProductValuationBalance[] detalles)
        {
            ProductionCostProductValuationSaldoDTO[] retorno;
            if (this.TempData.ContainsKey(m_inventoryBalanceDTO))
            {
                retorno = TempData[m_inventoryBalanceDTO] as ProductionCostProductValuationSaldoDTO[];
            }
            else
            {
                retorno = ConvertToDTO(detalles);
            }

            this.TempData[m_inventoryBalanceDTO] = retorno;
            this.TempData.Keep(m_inventoryBalanceDTO);
        }

        private ProductionCostProductValuationSaldoDTO[] ConvertToDTO(IList<ProductionCostProductValuationBalance> saldos)
        {
            var retorno = new List<ProductionCostProductValuationSaldoDTO>();
            int orden = 0;

            var unidadMedidaLibras = this.GetMetricUnitFromTempData("Lbs");
            var unidadMedidaKilos = this.GetMetricUnitFromTempData("Kg");

            foreach (var saldo in saldos)
            {
                var bodega = this.GetWarehouseFromTempData(saldo.idBodega);
                var item = this.GetItemFromTempData(saldo.idItem);
                var presentacion = item.Presentation;

                var factorPresentacion = presentacion != null
                    ? (decimal?)(presentacion.minimum * presentacion.maximum)
                    : null;

                var factorLibras = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaLibras.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaLibras.id)?.factor
                    : 1m;

                decimal? cantidadLibras, precioUnitarioLibras, costoTotalLibras;
                if (factorLibras.HasValue && factorPresentacion.HasValue)
                {
                    cantidadLibras = saldo.cantidad * factorPresentacion.Value * factorLibras.Value;
                    costoTotalLibras = saldo.costoTotal;
                    precioUnitarioLibras = cantidadLibras != 0m
                        ? Math.Abs(saldo.costoTotal / cantidadLibras.Value)
                        : 0m;
                }
                else
                {
                    cantidadLibras = precioUnitarioLibras = costoTotalLibras = null;
                }

                var factorKilos = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaKilos.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaKilos.id)?.factor
                    : 1m;

                decimal? cantidadKilos, precioUnitarioKilos, costoTotalKilos;
                if (factorKilos.HasValue && factorPresentacion.HasValue)
                {
                    cantidadKilos = saldo.cantidad * factorPresentacion.Value * factorKilos.Value;
                    costoTotalKilos = saldo.costoTotal;
                    precioUnitarioKilos = cantidadKilos != 0m
                        ? Math.Abs(saldo.costoTotal / cantidadKilos.Value)
                        : 0m;
                }
                else
                {
                    cantidadKilos = precioUnitarioKilos = costoTotalKilos = null;
                }

                orden++;
                retorno.Add(new ProductionCostProductValuationSaldoDTO()
                {
                    Orden = orden,
                    NombreBodega = bodega?.name,
                    CodigoNombreItem = string.Concat(item.masterCode, " - ", item.name),

                    Cantidad = saldo.cantidad,
                    PrecioUnitario = saldo.precioUnitario,
                    CostoTotal = saldo.costoTotal,

                    CantidadLibras = cantidadLibras,
                    PrecioUnitarioLibras = precioUnitarioLibras,
                    CostoTotalLibras = costoTotalLibras,

                    CantidadKilogramos = cantidadKilos,
                    PrecioUnitarioKilogramos = precioUnitarioKilos,
                    CostoTotalKilogramos = costoTotalKilos,
                });
            }

            return retorno
                .ToArray();
        }

        #endregion

        #region Procesamiento de saldos Iniciales
        private ProductValuationInventoryMoveDTO[] GetSaldosInventoryMoves(DateTime fechaCorte, int id_allocationType)
        {
            // preparamos el factor de conversión
            var unidadMedidaLibras = this.GetMetricUnitFromTempData("Lbs");
            var unidadMedidaKilos = this.GetMetricUnitFromTempData("Kg");

            var returnSaldosIniciales = this.GetSaldosIniciales(fechaCorte, id_allocationType);
            var parametroMetodoValorizacion = this.getParametroMetodoValorizacion();
            var saldosIniciales = Array.Empty<SaldoInicialDto>();
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                saldosIniciales = returnSaldosIniciales
                                        .Select(r => new SaldoInicialDto
                                        {
                                            id = r.id,
                                            idBodega = r.idBodega,
                                            idItem = r.idItem,
                                            cantidad = r.cantidad,
                                            costo_unitario = r.costo_unitario,
                                            costo_total = r.costo_total,
                                            activo = r.activo,
                                            idUsuarioCreacion = r.idUsuarioCreacion,
                                            fechaCreacion = r.fechaCreacion,
                                            idUsuarioModificacion = r.idUsuarioCreacion,
                                            fechaModificacion = r.fechaModificacion,
                                            id_allocationType = r.id_allocationType,
                                            fechaCorteInteger = r.fechaCorteInteger,
                                            plantaProceso = this.GetPlantaProcesoFromTempData(r.id_personProcessPlant)
                                        })
                                        .OrderByDescending(r => r.plantaProceso)
                                        .ThenBy(r => r.fechaCreacion)
                                        .ToArray();
            }
            else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
            {
                saldosIniciales = returnSaldosIniciales
                                           .Select(r => new SaldoInicialDto
                                           {
                                               id = r.id,
                                               idBodega = r.idBodega,
                                               idItem = r.idItem,
                                               cantidad = r.cantidad,
                                               costo_unitario = r.costo_unitario,
                                               costo_total = r.costo_total,
                                               activo = r.activo,
                                               idUsuarioCreacion = r.idUsuarioCreacion,
                                               fechaCreacion = r.fechaCreacion,
                                               idUsuarioModificacion = r.idUsuarioCreacion,
                                               fechaModificacion = r.fechaModificacion,
                                               id_allocationType = r.id_allocationType,
                                               fechaCorteInteger = r.fechaCorteInteger,

                                           })
                                           .ToArray();
            }


            var retorno = new List<ProductValuationInventoryMoveDTO>();
            for (int i = 0; i < saldosIniciales.Length; i++)
            {
                var saldoInicial = saldosIniciales[i];
                var bodega = this.GetWarehouseFromTempData(saldoInicial.idBodega);
                var item = this.GetItemFromTempData(saldoInicial.idItem);
                var presentacion = item.Presentation;

                var factorPresentacion = presentacion != null
                    ? (decimal?)(presentacion.minimum * presentacion.maximum)
                    : null;

                var factorLibras = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaLibras.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaLibras.id)?.factor
                    : 1m;

                decimal? cantidadLibras, precioUnitarioLibras, costoTotalLibras;
                if (factorLibras.HasValue && factorPresentacion.HasValue)
                {
                    cantidadLibras = saldoInicial.cantidad * factorPresentacion.Value * factorLibras.Value;
                    costoTotalLibras = saldoInicial.costo_total;
                    precioUnitarioLibras = cantidadLibras != 0m
                        ? Math.Abs(saldoInicial.costo_total / cantidadLibras.Value)
                        : 0m;
                }
                else
                {
                    cantidadLibras = precioUnitarioLibras = costoTotalLibras = null;
                }

                var factorKilos = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaKilos.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaKilos.id)?.factor
                    : 1m;

                decimal? cantidadKilos, precioUnitarioKilos, costoTotalKilos;
                if (factorKilos.HasValue && factorPresentacion.HasValue)
                {
                    cantidadKilos = saldoInicial.cantidad * factorPresentacion.Value * factorKilos.Value;
                    costoTotalKilos = saldoInicial.costo_total;
                    precioUnitarioKilos = cantidadKilos != 0m
                        ? Math.Abs(saldoInicial.costo_total / cantidadKilos.Value)
                        : 0m;
                }
                else
                {
                    cantidadKilos = precioUnitarioKilos = costoTotalKilos = null;
                }

                retorno.Add(new ProductValuationInventoryMoveDTO()
                {
                    Orden = i + 1,
                    OrdenCosteo = 0,
                    FechaEmision = fechaCorte.ToDateFormat(),
                    NombreBodega = bodega.name,
                    CodigoCategoriaMotivoMovimiento = DataProviderInventoryReason.m_SaldoInicial,
                    CategoriaMotivoMovimiento = DataProviderInventoryReason.GetCategoriaCostoString(DataProviderInventoryReason.m_SaldoInicial),
                    NombreMotivoMovimiento = "Saldo Inicial",
                    CodigoNombreItem = string.Concat(item.masterCode, " - ", item.name),

                    Cantidad = saldoInicial.cantidad,
                    PrecioUnitario = saldoInicial.costo_unitario,
                    CostoTotal = saldoInicial.cantidad > 0
                        ? saldoInicial.costo_total : -saldoInicial.costo_total,

                    CantidadLibras = cantidadLibras,
                    PrecioUnitarioLibras = precioUnitarioLibras,
                    CostoTotalLibras = saldoInicial.cantidad > 0
                        ? costoTotalLibras : -costoTotalLibras,

                    CantidadKilogramos = cantidadKilos,
                    PrecioUnitarioKilogramos = precioUnitarioKilos,
                    CostoTotalKilogramos = saldoInicial.cantidad > 0
                        ? costoTotalKilos : -costoTotalKilos,
                    ProcesoPlanta = saldoInicial.plantaProceso,
                    FechaOrden = fechaCorte.Date

                });
            }

            return retorno
                .ToArray();
        }
        private SaldoInicial[] GetSaldosIniciales(DateTime fechaCorte, int id_allocationType)
        {

            SaldoInicial[] saldoInicial = null;

            string m_sql = $"select  * from SaldoInicial where fechaCorteInteger ={fechaCorte.ToIntegerDate()} " +
                           $" AND id_allocationType ={id_allocationType} AND activo=1 ";

            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();

                saldoInicial = cnn.Query<SaldoInicial>(m_sql).ToArray();

                cnn.Close();
            };

            return saldoInicial;
            //var fechaCorteInteger = fechaCorte.ToIntegerDate();
            //return db.SaldoInicial
            //    .Where(e => e.fechaCorteInteger == fechaCorteInteger
            //        && e.id_allocationType == id_allocationType && e.activo)
            //    .ToArray();
        }
        private SaldoProductoBodega[] CalcularSaldosDiaBodega(DateTime fechaInicio, DateTime fechaFin, int id_allocationType)
        {
            var saldosProductoBodega = new List<SaldoProductoBodega>();

            var fechaSaldoAnt = fechaInicio.AddDays(-1);
            var saldoIniciales = this.GetSaldosIniciales(fechaSaldoAnt, id_allocationType);

            // Actualizamos la data de productos a procesar en caso de ser necesario
            this.PoblarMovimientosProcesar(fechaFin);

            var movimientosCorte = new List<Movimiento>();
            var fechaPivote = fechaInicio;
            while (fechaPivote <= fechaFin)
            {
                movimientosCorte.AddRange(this.RecuperarMovimientosFecha(fechaPivote.ToIntegerDate()));
                fechaPivote = fechaPivote.AddDays(1);
            }

            var agrupacionesProcesar = movimientosCorte
                .GroupBy(e => new {
                    e.IdBodega,
                    e.IdItem,
                })
                .Select(e => new {
                    e.Key.IdBodega,
                    e.Key.IdItem,
                    FechasMovimientos = e.Select(x => x.FechaMovimiento).Distinct()
                        .OrderBy(x => x)
                        .ToArray()
                })
                .ToArray();

            foreach (var agrupacionProcesar in agrupacionesProcesar)
            {
                var bodega = this.GetWarehouseFromTempData(agrupacionProcesar.IdBodega);
                var item = this.GetItemFromTempData(agrupacionProcesar.IdItem);

                var saldoInicial = saldoIniciales
                    .FirstOrDefault(e => e.idBodega == agrupacionProcesar.IdBodega
                        && e.idItem == agrupacionProcesar.IdItem);
                var cantidadInicial = saldoInicial?.cantidad ?? 0m;

                if (saldoInicial != null)
                {
                    saldosProductoBodega.Add(new SaldoProductoBodega()
                    {
                        FechaSaldoDt = fechaSaldoAnt,
                        FechaSaldo = fechaSaldoAnt.ToDateFormat(),
                        Bodega = bodega.name,
                        Item = $"{item.masterCode} - {item.name}",
                        EntradaDia = cantidadInicial,
                        SaldoAnterior = 0,
                        SalidaDia = 0,
                        SaldoDia = cantidadInicial,
                        SaldoFinal = cantidadInicial,
                        IdsInventoryMoveDetails = string.Empty,
                    });
                }

                foreach (var fechaMovimiento in agrupacionProcesar.FechasMovimientos)
                {
                    var movimientos = movimientosCorte
                        .Where(e => e.IdBodega == agrupacionProcesar.IdBodega
                            && e.IdItem == agrupacionProcesar.IdItem
                            && e.FechaMovimiento == fechaMovimiento)
                        .ToArray();
                    var cantidad = movimientos.Sum(e => e.Cantidad);
                    var entrada = movimientos
                        .Where(e => e.Cantidad > 0)
                        .Sum(e => e.Cantidad);

                    var salida = movimientos
                        .Where(e => e.Cantidad < 0)
                        .Sum(e => e.Cantidad);

                    var saldoAnterior = cantidadInicial;
                    cantidadInicial += movimientos.Sum(e => e.Cantidad);
                    var idsMovimientos = movimientos.Select(e => e.IdInventoryMoveDetail).ToArray();

                    saldosProductoBodega.Add(new SaldoProductoBodega()
                    {
                        FechaSaldoDt = fechaMovimiento.ToDateInteger(),
                        FechaSaldo = fechaMovimiento.ToDateInteger().ToDateFormat(),
                        Bodega = bodega.name,
                        Item = $"{item.masterCode} - {item.name}",
                        SaldoAnterior = saldoAnterior,
                        EntradaDia = entrada,
                        SalidaDia = salida,
                        SaldoDia = entrada - Math.Abs(salida),
                        SaldoFinal = cantidadInicial,
                        IdsInventoryMoveDetails = string.Join(",", idsMovimientos),
                    });
                }
            }

            return saldosProductoBodega
                .OrderBy(e => e.Bodega)
                .ThenBy(e => e.Item)
                .ThenBy(e => e.FechaSaldoDt)
                .ToArray();
        }

        private void PoblarMovimientosProcesar(DateTime fechaProcesar)
        {
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();

                var parameters = new { año = fechaProcesar.Year, mes = fechaProcesar.Month, dia = fechaProcesar.Day };
                cnn.Execute("inv_PoblarDataInventario", parameters, commandType: CommandType.StoredProcedure);

                cnn.Close();
            }
        }
        private Movimiento[] RecuperarMovimientosFecha(int fechaProcesar)
        {
            const string m_sql = "Select * from inv_MovimientosProcesar where fechaMovimiento = @fechaProcesar;";
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;

            IEnumerable<Movimiento> retorno;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open(); ;
                retorno = cnn.Query<Movimiento>(m_sql, param: new { fechaProcesar });

                cnn.Close();
            }

            return retorno
                .ToArray();
        }

        private class SaldoProductoBodega
        {
            public DateTime FechaSaldoDt { get; set; }

            [Description("Bodega")]
            public string Bodega { get; set; }

            [Description("Ítem")]
            public string Item { get; set; }

            [Description("Fecha de Saldo")]
            public string FechaSaldo { get; set; }

            [Description("Saldo Anterior")]
            public decimal SaldoAnterior { get; set; }

            [Description("Ingresos Master/Cajas Día")]
            public decimal EntradaDia { get; set; }

            [Description("Salida Master/Cajas Día")]
            public decimal SalidaDia { get; set; }

            [Description("Saldo Master/Cajas Día")]
            public decimal SaldoDia { get; set; }

            [Description("Saldo Stock Final")]
            public decimal SaldoFinal { get; set; }
            public string IdsInventoryMoveDetails { get; set; }
        }

        private class Movimiento
        {
            public int IdInventoryMoveDetail { get; set; }
            public int FechaMovimiento { get; set; }
            public int IdMotivo { get; set; }
            public int IdBodega { get; set; }
            public int IdItem { get; set; }
            public decimal Cantidad { get; set; }
            public decimal CostoUnitario { get; set; }
            public decimal CostoTotal { get; set; }
            public bool Procesado { get; set; }
        }

        #endregion

        #region Métodos para el control de los movimientos generados
        public class ConfigExcelColumn
        {
            public string Label { get; set; }
            public int IndexColumn { get; set; }
            public string LetterColumn { get; set; }
            public bool HasFormula { get; set; }
            public string Formula { get; set; }
        }
        public class ProductValuationInventoryMoveDTO
        {

            [Description("Nº")]
            public int Orden { get; set; }

            [Description("Nº Asig. Costo")]
            public int OrdenCosteo { get; set; }
            public int IdInventoryMove { get; set; }

            [Description("Planta Proceso")]
            public string ProcesoPlanta { get; set; }

            [Description("Ordenamiento Motivo bodega")]
            public decimal NumOrden { get; set; }
            public decimal OrdenTransferencia { get; set; }


            [Description("Bodega")]
            public string NombreBodega { get; set; }

            [Description("Ítem")]
            public string CodigoNombreItem { get; set; }

            [Description("Fecha Emisión")]
            public string FechaEmision { get; set; }

            [Description("Fecha Creación")]
            public string FechaCreacion { get; set; }

            [Description("Còd. Categoría de Motivos")]
            public string CodigoCategoriaMotivoMovimiento { get; set; }

            [Description("Categoría de Motivos")]
            public string CategoriaMotivoMovimiento { get; set; }

            [Description("Motivo")]
            public string NombreMotivoMovimiento { get; set; }

            [Description("Nº Movimiento")]
            public string NumeroMovimiento { get; set; }

            [Description("Sec. Transac.")]
            public string SecuenciaTransaccional { get; set; }

            [Description("Nº Lote")]
            public string NumeroLote { get; set; }

            [Description("Tipo Cálculo")]
            public string TipoCalculo { get; set; }

            [Description("Acción")]
            public string Accion { get; set; }
            public decimal? Coeficiente { get; set; }


            [Description("Cantidad Cajas / Masters")]
            public decimal Cantidad { get; set; }

            [Description("P.U. Cajas / Masters")]
            public decimal PrecioUnitario { get; set; }

            [Description("Costo Total Cajas / Masters")]
            public decimal CostoTotal { get; set; }


            [Description("Cantidad Libras")]
            public decimal? CantidadLibras { get; set; }

            [Description("P.U. Libras")]
            public decimal? PrecioUnitarioLibras { get; set; }

            [Description("Costo Total Libras")]
            public decimal? CostoTotalLibras { get; set; }


            [Description("Cantidad Kilogramos")]
            public decimal? CantidadKilogramos { get; set; }

            [Description("P.U. Kilogramos")]
            public decimal? PrecioUnitarioKilogramos { get; set; }

            [Description("Costo Total Kilogramos")]
            public decimal? CostoTotalKilogramos { get; set; }

            public DateTime FechaOrden { get; set; }
        }

        private ProductValuationInventoryMoveDTO[] ConvertToDTO(IList<ProductionCostProductValuationInventoryMove> productionCostProductValuationInventoryMoves)
        {
            var retorno = new List<ProductValuationInventoryMoveDTO>();

            // preparamos el factor de conversión
            var unidadMedidaLibras = this.GetMetricUnitFromTempData("Lbs");
            var unidadMedidaKilos = this.GetMetricUnitFromTempData("Kg");

            foreach (var process in productionCostProductValuationInventoryMoves)
            {
                var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(process.idInventoryMoveDetail);
                var inventoryMove = inventoryMoveDetail.InventoryMove;
                var motivo = inventoryMove.InventoryReason.name;
                var bodega = inventoryMoveDetail.Warehouse.name;
                var numeroDocumento = inventoryMove.sequential.ToString();
                var codigoItem = inventoryMoveDetail.Item.masterCode;
                var nombreItem = inventoryMoveDetail.Item.name;
                var presentacion = inventoryMoveDetail.Item?.Presentation;
                var numeroLote = this.GetInternalNumberLotFromTempData(process.id_lot);
                var secuenciaTransaccional = this.GetNumberLotFromTempData(process.id_lot);

                var factorPresentacion = presentacion != null
                    ? (decimal?)(presentacion.minimum * presentacion.maximum)
                    : null;

                var factorLibras = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaLibras.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaLibras.id)?.factor
                    : 1m;

                decimal? cantidadLibras, precioUnitarioLibras, costoTotalLibras;
                if (factorLibras.HasValue && factorPresentacion.HasValue)
                {
                    cantidadLibras = process.cantidad * factorPresentacion.Value * factorLibras.Value;
                    costoTotalLibras = process.costoTotal;
                    precioUnitarioLibras = cantidadLibras != 0m
                        ? Math.Abs(process.costoTotal / cantidadLibras.Value)
                        : 0m;
                }
                else
                {
                    cantidadLibras = precioUnitarioLibras = costoTotalLibras = null;
                }

                var factorKilos = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaKilos.id))
                    ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaKilos.id)?.factor
                    : 1m;

                decimal? cantidadKilos, precioUnitarioKilos, costoTotalKilos;
                if (factorKilos.HasValue && factorPresentacion.HasValue)
                {
                    cantidadKilos = process.cantidad * factorPresentacion.Value * factorKilos.Value;
                    costoTotalKilos = process.costoTotal;
                    precioUnitarioKilos = cantidadKilos != 0m
                        ? Math.Abs(process.costoTotal / cantidadKilos.Value)
                        : 0m;
                }
                else
                {
                    cantidadKilos = precioUnitarioKilos = costoTotalKilos = null;
                }

                var productionLot = db.ProductionLot.FirstOrDefault(e => e.id == inventoryMoveDetail.id_lot);

                var parametroMetodoValorizacion = getParametroMetodoValorizacion();

                decimal numOrden = 0;
                decimal? ordenTransferencias = null;
                string plantaProceso = "";

                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                {
                    var ordenSpecial = getOrdenSpecial(inventoryMoveDetail.id);

                    if (!ordenSpecial.Item1.HasValue && !ordenSpecial.Item1.HasValue)
                    {
                        //numOrden = this.GetOrdenBodegaMotivo(   inventoryMoveDetail.id_warehouse,
                        //                                        inventoryMove.id_inventoryReason ?? 0, process.idInventoryMoveDetail, process.naturaleza,
                        //                                        out var ordenTransferenciasP);
                        //ordenTransferencias = ordenTransferenciasP;
                    }
                    else
                    {
                        numOrden = ordenSpecial.Item1.Value;
                        ordenTransferencias = ordenSpecial.Item2.Value;
                        plantaProceso = ordenSpecial.Item3;
                    }
                }

                retorno.Add(new ProductValuationInventoryMoveDTO()
                {
                    ProcesoPlanta = plantaProceso,
                    NumOrden = numOrden,
                    OrdenTransferencia = ordenTransferencias ?? 0,
                    IdInventoryMove = process.idInventoryMoveDetail,
                    OrdenCosteo = process.ordenAsigCost,
                    Orden = process.orden,
                    FechaEmision = inventoryMove.Document.emissionDate.ToDateFormat(),
                    FechaCreacion = inventoryMoveDetail.dateCreate.ToDateTimeFormat(),
                    NombreMotivoMovimiento = motivo,
                    NombreBodega = bodega,
                    NumeroMovimiento = numeroDocumento,
                    CodigoCategoriaMotivoMovimiento = inventoryMove.InventoryReason.CategoriaCosto,
                    CategoriaMotivoMovimiento = DataProviderInventoryReason.GetCategoriaCostoString(inventoryMove.InventoryReason.CategoriaCosto),
                    CodigoNombreItem = string.Concat(codigoItem, " - ", nombreItem),
                    TipoCalculo = process.tipoCalculo,
                    Accion = GetAccionCosto(process.accion),
                    Coeficiente = process.coeficiente,
                    NumeroLote = numeroLote,
                    SecuenciaTransaccional = secuenciaTransaccional,

                    Cantidad = process.cantidad,
                    PrecioUnitario = process.costoUnitario,
                    CostoTotal = process.cantidad > 0
                        ? process.costoTotal : -process.costoTotal,

                    CantidadLibras = cantidadLibras,
                    PrecioUnitarioLibras = precioUnitarioLibras,
                    CostoTotalLibras = process.cantidad > 0
                        ? costoTotalLibras : -costoTotalLibras,

                    CantidadKilogramos = cantidadKilos,
                    PrecioUnitarioKilogramos = precioUnitarioKilos,
                    CostoTotalKilogramos = process.cantidad > 0
                        ? costoTotalKilos : -costoTotalKilos,
                    FechaOrden = inventoryMove.Document.emissionDate.Date
                });
            }

            return retorno
                .OrderBy(e => e.Orden)
                .ToArray();
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult QueryMovimientosValorizacionDetails(int? idProductValuation)
        {
            var productValuation = this.GetEditingCostProductValuation(idProductValuation);

            this.TempData.Keep(m_CostOrderSpecialList);
            this.TempData.Keep(m_CostProductValuationExcelModelKey);

            this.TempData[m_CostProductValuationModelKey] = productValuation;
            this.TempData.Keep(m_CostProductValuationModelKey);

            var detalles = productValuation
                .ProductionCostProductValuationInventoryMove
                .Where(e => e.activo)
                .OrderBy(e => e.orden)
                .ToArray();

            this.SetViewBagAllocationType(productValuation);
            this.SaveProductValuationInventoryMoveDTOTempData(detalles);

            return this.PartialView("_ProductValuationInventoryMoveGridViewPartial", new ProductionCostProductValuationInventoryMove[] { });
        }


        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult QueryResumenDetailsCallback(int? idProductValuation)
        {
            var productValuation = this.GetEditingCostProductValuation(idProductValuation);

            this.TempData.Keep(m_CostOrderSpecialList);
            this.TempData.Keep(m_CostProductValuationExcelModelKey);

            this.TempData[m_CostProductValuationModelKey] = productValuation;
            this.TempData.Keep(m_CostProductValuationModelKey);

            CriterioResumen[] resumen;
            if (this.TempData.ContainsKey("Resumen"))
            {
                resumen = TempData["Resumen"] as CriterioResumen[];
            }
            else
            {
                resumen = this.PrepareResumenData(productValuation);
            }

            this.TempData["Resumen"] = resumen;
            this.TempData.Keep("Resumen");

            return this.PartialView("_ProductValuationInventoryMoveResumenCallbackPanel");
        }

        private CriterioResumen[] PrepareResumenData(ProductionCostProductValuation productValuation)
        {
            var detalles = productValuation
                .ProductionCostProductValuationInventoryMove
                .Where(e => e.activo)
                .OrderBy(e => e.orden)
                .ToArray();
            var fechaValorizacion = new DateTime(productValuation.anio, productValuation.mes, 1);
            var fechaSaldoInicial = (productValuation.fechaInicio ?? fechaValorizacion).AddDays(-1);
            var saldosIniciales = this.GetSaldosInventoryMoves(fechaSaldoInicial, productValuation.id_allocationType);
            var retorno = ConvertToDTO(detalles);



            return this.GenerateResumen(retorno.Union(saldosIniciales).ToArray());
        }

        private void SetViewBagAllocationType(ProductionCostProductValuation productValuation)
        {
            var costoReal = db.ProductionCostAllocationType.FirstOrDefault(e => e.code == "REAL");
            var esCostoReal = productValuation?.id_allocationType == costoReal?.id;

            this.ViewBag.EsCostoReal = esCostoReal;
        }

        private void SaveProductValuationInventoryMoveDTOTempData(ProductionCostProductValuationInventoryMove[] detalles)
        {
            ProductValuationInventoryMoveDTO[] retorno;
            if (this.TempData.ContainsKey(m_inventoryMoveDetailDTO))
            {
                retorno = TempData[m_inventoryMoveDetailDTO] as ProductValuationInventoryMoveDTO[];
            }
            else
            {
                retorno = ConvertToDTO(detalles);
            }

            this.TempData[m_inventoryMoveDetailDTO] = retorno;
            this.TempData.Keep(m_inventoryMoveDetailDTO);
        }

        private void SaveProductValuationResumenDTOTempData(ProductionCostProductValuation productValuation)
        {
            CriterioResumen[] retorno;
            if (this.TempData.ContainsKey(m_inventoryResumenDTO))
            {
                retorno = TempData[m_inventoryResumenDTO] as CriterioResumen[];
            }
            else
            {
                var fechainicio = productValuation.fechaInicio.Value.AddDays(-1);
                var saldosIniciales = this.GetSaldosInventoryMoves(fechainicio, productValuation.id_allocationType);
                var detalles = this.ConvertToDTO(productValuation.ProductionCostProductValuationInventoryMove.ToArray());
                retorno = this.GenerateResumen(detalles.Union(saldosIniciales).ToArray());
            }

            this.TempData[m_inventoryResumenDTO] = retorno;
            this.TempData.Keep(m_inventoryResumenDTO);
        }
        #endregion

        #region Métodos adicionales

        private ProductionCostProductValuation GetEditingCostProductValuation(int? id_productValuation)
        {
            // Recuperamos el elemento del caché local
            var productValuation = (this.TempData[m_CostProductValuationModelKey] as ProductionCostProductValuation);

            // Removemos la data del detalle de movimiento
            if (this.TempData.ContainsKey(m_inventoryMoveDetailDTO))
            {
                this.TempData.Remove(m_inventoryMoveDetailDTO);
            }
            if (this.TempData.ContainsKey(m_inventoryBalanceDTO))
            {
                this.TempData.Remove(m_inventoryBalanceDTO);
            }

            // Si no hay elemento en el caché, consultamos desde la base
            if ((productValuation == null) && id_productValuation.HasValue)
            {
                productValuation = db.ProductionCostProductValuation
                    .Include("Document")
                    .Include("ProductionCostAllocationType")
                    .FirstOrDefault(i => i.id == id_productValuation);
            }

            if (productValuation != null)
            {
                if (productValuation.processed)
                {
                    productValuation.ProductionCostProductValuationExecutions = productValuation
                        .ProductionCostProductValuationExecutions
                        .Where(d => d.isActive)
                        .ToList();

                    productValuation.ProductionCostProductValuationInventoryMove = productValuation
                        .ProductionCostProductValuationInventoryMove
                        .Where(d => d.activo)
                        .ToList();

                    productValuation.ProductionCostProductValuationBalance = productValuation
                        .ProductionCostProductValuationBalance
                        .Where(d => d.activo)
                        .ToList();

                    productValuation.ProductionCostProductValuationWarehouse = productValuation
                        .ProductionCostProductValuationWarehouse
                        .Where(d => d.isActive)
                        .ToList();

                    productValuation.ProductionCostProductValuationWarning = productValuation
                        .ProductionCostProductValuationWarning
                        .Where(d => d.activo)
                        .ToList();


                    this.SetViewBagAllocationType(productValuation);
                    this.SaveProductValuationInventoryMoveDTOTempData(productValuation.ProductionCostProductValuationInventoryMove.ToArray());
                    var detalles = productValuation
                        .ProductionCostProductValuationInventoryMove
                        .Where(e => e.activo)
                        .OrderBy(e => e.orden)
                        .ToArray();
                    this.SaveProductValuationResumenDTOTempData(productValuation);
                    this.SaveProductValuationBalanceDTOTempData(productValuation.ProductionCostProductValuationBalance.ToArray());
                }
                else
                {
                    productValuation.ProductionCostProductValuationExecutions = new List<ProductionCostProductValuationExecution>();
                    productValuation.ProductionCostProductValuationInventoryMove = new List<ProductionCostProductValuationInventoryMove>();
                    productValuation.ProductionCostProductValuationBalance = new List<ProductionCostProductValuationBalance>();
                    productValuation.ProductionCostProductValuationWarehouse = new List<ProductionCostProductValuationWarehouse>();
                    productValuation.ProductionCostProductValuationWarning = new List<ProductionCostProductValuationWarning>();
                }
            }

            // Si no existe, creamos un nuevo elemento
            return productValuation ?? this.CreateNewCostProductValuation();
        }

        private ProductionCostProductValuation CreateNewCostProductValuation()
        {
            // Recuperamos el tipo de documento
            var documentType = db.DocumentType
                .FirstOrDefault(dt => dt.code == m_TipoDocumentoCostProductValuation
                                    && dt.id_company == this.ActiveCompanyId);

            if (documentType == null)
            {
                throw new ApplicationException("No existe registrado el tipo de documento: Valorización de Productos.");
            }

            // Recuperamos el estado PENDIENTE
            var documentState = DataProviderCostProductValuation
                .GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

            if (documentState == null)
            {
                throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
            }

            // Recuperamos el tipo de asignación COSTO PROYECTADO
            var allocationType = db.ProductionCostAllocationType
                .FirstOrDefault(t => t.code == "PROJ");

            if (allocationType == null)
            {
                throw new ApplicationException("No existe registrado el tipo de asignación COSTO PROYECTADO [PROJ].");
            }

            // Calculamos el secuencial y el número de documento siguiente
            var documentSequential = documentType.currentNumber;
            var documentNumber = $"{this.ActiveEmissionPoint.BranchOffice.code:000}-{this.ActiveEmissionPoint.code:000}-{documentSequential}";

            // Preparamos el documento
            var document = new Document()
            {
                number = documentNumber,
                sequential = documentSequential,
                emissionDate = DateTime.Today,
                DocumentType = documentType,
                DocumentState = documentState,
            };

            // Verificamos la última fecha de valorización aplicada
            this.VerificarUltimoControlPeriodo(DateTime.Today.Year, DateTime.Today.Month,
                allocationType.id, out var fechaInicio, out var fechaFin);

            // Creamos el documento de asignación de costos
            return new ProductionCostProductValuation()
            {
                Document = document,
                id_allocationType = allocationType.id,
                ProductionCostAllocationType = allocationType,
                anio = DateTime.Today.Year,
                mes = DateTime.Today.Month,
                id_userUpdate = this.ActiveUserId,
                fechaInicio = fechaInicio,
                fechaFin = fechaFin,

                ProductionCostProductValuationExecutions = new ProductionCostProductValuationExecution[] { },
                ProductionCostProductValuationWarehouse = new ProductionCostProductValuationWarehouse[] { },
            };
        }

        private void PrepareEditViewBag(ProductionCostProductValuation productValuation)
        {
            // Verificamos el estado actual del documento
            var documentStateCode = productValuation.Document.DocumentState.code;
            var documentExists = productValuation.id > 0;
            var canEditDocument = (documentStateCode == m_PendienteDocumentState);
            var estaProcesadoDocumento = productValuation.processed;

            // Agregamos los elementos al ViewBag
            this.ViewBag.DocumentoExistente = documentExists;
            this.ViewBag.PuedeEditarDocumento = canEditDocument;
            this.ViewBag.EstaProcesadoDocumento = estaProcesadoDocumento;
            this.ViewBag.PuedeAprobarDocumento = documentExists && (documentStateCode == m_PendienteDocumentState);
            this.ViewBag.PuedeReversarDocumento = documentExists && (documentStateCode == m_AprobadoDocumentState);
            this.ViewBag.PuedeAnularDocumento = documentExists && (documentStateCode == m_PendienteDocumentState);

            // Agregamos elementos auxiliares de control de períodos
            if (canEditDocument)
            {
                // Si el documento es editable --> lista de años y periodos disponibles para uso
                var periodosUsables = this.db.InventoryValuationPeriodDetail
                    .Where(d => !d.isClosed && d.InventoryValuationPeriod.isActive
                                && (d.id_PeriodState == 2 || d.id_PeriodState == 3))
                    .OrderBy(d => d.InventoryValuationPeriod.year).ThenBy(d => d.periodNumber)
                    .ToArray()
                    .Select(d => new
                    {
                        Anio = d.InventoryValuationPeriod.year,
                        Mes = d.periodNumber,
                        IsActive = (d.id_PeriodState == 2),
                    })
                    .ToArray();

                this.ViewBag.PeriodosUsables = periodosUsables;

                this.ViewBag.AnioListModel = periodosUsables
                    .Select(d => new { d.Anio })
                    .Distinct()
                    .ToArray();

                this.ViewBag.MesListModel = periodosUsables
                    .Where(d => d.Anio == productValuation.anio)
                    .Select(d => new { d.Mes })
                    .Distinct()
                    .ToArray();


                var idTipoAsignacionCostoProyectado = this.db.ProductionCostAllocationType
                    .FirstOrDefault(t => t.code == "PROJ")?
                    .id ?? 0;

                this.ViewBag.IdTipoAsignacionCostoProyectado = idTipoAsignacionCostoProyectado;
                this.ViewBag.PuedeEditarCamposFecha = (productValuation.id_allocationType == idTipoAsignacionCostoProyectado);
            }
            else
            {
                this.ViewBag.PeriodosUsables = new object[] { };

                this.ViewBag.AnioListModel = new[]
                {
                    new { Anio = productValuation.anio, },
                };

                this.ViewBag.MesListModel = new[]
                {
                    new { Mes = productValuation.mes, },
                };

                this.ViewBag.IdTipoAsignacionCostoProyectado = 0;
                this.ViewBag.PuedeEditarCamposFecha = false;
            }

            this.ViewBag.ResponsableNombre = db.User
                .FirstOrDefault(u => u.id == productValuation.id_userUpdate)?
                .Employee?
                .Person?
                .fullname_businessName;


            this.SetViewBagAllocationType(productValuation);
            this.PrepareDetailsEditViewBag(canEditDocument, estaProcesadoDocumento);
            this.SaveProductValuationInventoryMoveDTOTempData(productValuation.ProductionCostProductValuationInventoryMove.ToArray());
            this.TempData["Resumen"] = this.PrepareResumenData(productValuation);
        }

        private void PrepareDetailsEditViewBag(bool editable, bool estaProcesado)
        {
            // Agregamos los elementos al ViewBag
            this.ViewBag.DetailsEditable = editable;
            this.ViewBag.EstaProcesadoDocumento = estaProcesado;
        }

        private InventoryReason GetInventoryReasonFromTempData(int idInventoryReason)
        {
            var key = $"InventoryReason_{idInventoryReason}";

            if (TempData.ContainsKey(key))
            {
                return TempData[key] as InventoryReason;
            }
            else
            {
                var inventoryReason = db.InventoryReason.FirstOrDefault(e => e.id == idInventoryReason);
                if (inventoryReason != null)
                {
                    TempData[key] = inventoryReason;
                    TempData.Keep(key);
                }
                return inventoryReason;
            }
        }
        private Warehouse GetWarehouseFromTempData(int idWarehouse)
        {
            var key = $"WareHouse_{idWarehouse}";
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as Warehouse;
            }
            else
            {
                var warehouse = db.Warehouse.FirstOrDefault(e => e.id == idWarehouse);
                if (warehouse == null)
                {
                    warehouse = new Warehouse
                    {
                        id = 0,
                        name = "General Costeo"
                    };
                }
                TempData[key] = warehouse;
                TempData.Keep(key);

                return warehouse;
            }
        }
        private Item GetItemFromTempData(int idItem)
        {
            var key = $"Item_{idItem}";
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as Item;
            }
            else
            {
                var warehouse = db.Item.FirstOrDefault(e => e.id == idItem);
                TempData[key] = warehouse;
                TempData.Keep(key);

                return warehouse;
            }
        }

        private ProductionCostCoefficientExecutionWarehouse[] GetProductionCostCoefficientExecutionWarehouseFromTempData(IEnumerable<int> ids_coefficientExecution)
        {
            var list = new List<ProductionCostCoefficientExecutionWarehouse>();
            foreach (var id_coefficientExecution in ids_coefficientExecution)
            {
                var key = $"ProductionCostCoefficientExecutionWarehouse_{id_coefficientExecution}";

                ProductionCostCoefficientExecutionWarehouse[] data;
                if (TempData.ContainsKey(key))
                {
                    data = TempData[key] as ProductionCostCoefficientExecutionWarehouse[];
                }
                else
                {
                    data = db.ProductionCostCoefficientExecutionWarehouse
                        .Where(e => e.id_coefficientExecution == id_coefficientExecution)
                        .ToArray();

                    TempData[key] = data;
                    TempData.Keep(key);
                }

                if ((data != null) && (data.Any()))
                {
                    list.AddRange(data);
                }
            }

            return list
                .ToArray();
        }

        private InventoryMoveDetail GetInventoryMoveDetailFromTempData(int id_inventoryMoveDetail)
        {
            var key = $"InventoryMoveDetail_{id_inventoryMoveDetail}";
            InventoryMoveDetail inventoryMoveDetail;
            if (TempData.ContainsKey(key))
            {
                inventoryMoveDetail = TempData[key] as InventoryMoveDetail;
            }
            else
            {
                inventoryMoveDetail = db.InventoryMoveDetail.FirstOrDefault(e => e.id == id_inventoryMoveDetail);
            }

            TempData[key] = inventoryMoveDetail;
            TempData.Keep(key);

            return inventoryMoveDetail;
        }

        private Tuple<decimal?, decimal?, string> getOrdenSpecial(int inventoryMoveDetailId)
        {
            if (TempData.ContainsKey(m_CostOrderSpecialList))
            {
                var datos = TempData[m_CostOrderSpecialList] as List<DataProcesarInventario>;
                var movimiento = datos.FirstOrDefault(r => r.IdInventoryMoveDetail == inventoryMoveDetailId);
                if (movimiento != null)
                {
                    return new Tuple<decimal?, decimal?, string>(movimiento.OrdenMotivoTipoDocum, movimiento.OrdenTransferencias, movimiento.Proceso);
                }

            }
            return new Tuple<decimal?, decimal?, string>(null, null, null);
        }

        private string GetInternalNumberLotFromTempData(int id_Lote)
        {
            var key = $"InternalNumber_{id_Lote}";
            string internalNumber;
            if (TempData.ContainsKey(key))
            {
                internalNumber = TempData[key] as string;
            }
            else
            {
                internalNumber = db.Lot.FirstOrDefault(e => e.id == id_Lote)?.internalNumber;
                if (String.IsNullOrEmpty(internalNumber))
                {
                    var lot = db.ProductionLot.FirstOrDefault(e => e.id == id_Lote);
                    internalNumber = lot?.internalNumber ?? string.Empty;
                }
            }

            TempData[key] = internalNumber;
            TempData.Keep(key);

            return internalNumber;
        }

        private string GetNumberLotFromTempData(int id_Lote)
        {
            var key = $"Number_{id_Lote}";
            string number;
            if (TempData.ContainsKey(key))
            {
                number = TempData[key] as string;
            }
            else
            {
                number = db.Lot.FirstOrDefault(e => e.id == id_Lote)?.number;
                if (String.IsNullOrEmpty(number))
                {
                    var lot = db.ProductionLot.FirstOrDefault(e => e.id == id_Lote);
                    number = lot?.number ?? string.Empty;
                }
            }

            TempData[key] = number;
            TempData.Keep(key);

            return number;
        }


        private string GetPlantaProcesoFromTempData(int? id_personProcessPlant)
        {
            if (!id_personProcessPlant.HasValue) return null;

            var key = $"m_plantaProcesoPersonKey_{id_personProcessPlant.Value}";
            string plantaProcesoNombre;
            if (TempData.ContainsKey(key))
            {
                plantaProcesoNombre = TempData[key] as string;
            }
            else
            {
                plantaProcesoNombre = db.Person.FirstOrDefault(e => e.id == id_personProcessPlant)?.processPlant;
            }

            TempData[key] = plantaProcesoNombre;
            TempData.Keep(key);

            return plantaProcesoNombre;
        }

        private MetricUnitConversion GetMetricUnitConversionFromTempData(int id_metricOrigin, int id_metricDestiny)
        {
            var key = $"MetricUnitConversion_{id_metricOrigin}_{id_metricDestiny}";
            MetricUnitConversion conversion;
            if (TempData.ContainsKey(key))
            {
                conversion = TempData[key] as MetricUnitConversion;
            }
            else
            {
                conversion = db.MetricUnitConversion
                    .FirstOrDefault(t => t.id_metricOrigin == id_metricOrigin &&
                        t.id_metricDestiny == id_metricDestiny && t.isActive);
            }

            TempData[key] = conversion;
            TempData.Keep(key);

            return conversion;
        }
        private void VerificarUltimoControlPeriodo(int año, int mes,
            int id_allocationType, out DateTime fechaInicio, out DateTime fechaFin)
        {
            var idEstadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05").id;
            var ultimoControl = db.ProductionCostProductValuationControl
                .Where(e => e.año == año && e.mes == mes
                    && e.id_allocationType == id_allocationType
                    && e.idEstado != idEstadoAnulado && e.activo)
                .OrderByDescending(e => e.fechaFinInteger)
                .FirstOrDefault();

            fechaInicio = ultimoControl != null
                ? ultimoControl.fechaFinInteger.ToDateInteger().AddDays(1)
                : new DateTime(año, mes, 1);

            fechaFin = new DateTime(año, mes, 1).AddMonths(1).AddDays(-1);
            if (fechaInicio > fechaFin) fechaInicio = fechaFin;
        }

        [HttpPost]
        public JsonResult CalcularUltimaValorizacion(
            int año, int mes, int id_allocationType)
        {
            string message;
            bool isValid;
            DateTime? fechaInicio, fechaFin;

            try
            {
                this.VerificarUltimoControlPeriodo(año, mes, id_allocationType,
                    out var fechaInicioUt, out var fechaFinUt);
                fechaInicio = fechaInicioUt;
                fechaFin = fechaFinUt;

                // Asignamos las fechas por defecto a la memoria temporal
                var productValuation = (this.TempData[m_CostProductValuationModelKey] as ProductionCostProductValuation);
                productValuation.fechaInicio = fechaInicioUt;
                productValuation.fechaFin = fechaFinUt;

                this.TempData[m_CostProductValuationModelKey] = productValuation;
                this.TempData.Keep(m_CostProductValuationModelKey);

                message = "";
                isValid = true;
            }
            catch (Exception exception)
            {
                fechaInicio = fechaFin = null;
                message = "Error al Consultar el control de valorización: " + exception.GetBaseException()?.Message ?? string.Empty;
                isValid = false;
            }

            var result = new
            {
                message,
                isValid,
                fechaInicio = fechaInicio.ToDateFormat(),
                fechaFin = fechaFin.ToDateFormat(),
                fechaFinInteger = fechaFin.ToIntegerDate(),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Cálculos de asignación y distribución de costos

        private void CalcularDetallesEjecucionCoeficientes(ProductionCostProductValuation productValuation)
        {
            // Recuperamos las ejecuciones aprobadas no valorizadas para el período indicado
            var idAprobadoDocumentState = DataProviderCostCoefficientExecution
                .GetDocumentStateByCode(this.ActiveCompanyId, m_AprobadoDocumentState)?
                .id;

            var ejecucionesCoeficientes = db.ProductionCostCoefficientExecution
                .Where(a => a.anio == productValuation.anio
                            && a.mes == productValuation.mes
                            && a.id_allocationType == productValuation.id_allocationType
                            && a.value_processed == false
                            && a.Document.id_documentState == idAprobadoDocumentState)
                .ToArray();

            // Procesamos las ejecuciones y generamos los detalles de la valorización
            var idValorizacionProductoEjecucion = 1;
            var costProductValuationExecutions = new List<ProductionCostProductValuationExecution>();

            foreach (var ejecucionCoeficiente in ejecucionesCoeficientes)
            {
                costProductValuationExecutions.Add(new ProductionCostProductValuationExecution()
                {
                    id = idValorizacionProductoEjecucion++,

                    id_coefficientExecution = ejecucionCoeficiente.id,
                    ProductionCostCoefficientExecution = ejecucionCoeficiente,

                    valor = ejecucionCoeficiente.ProductionCostCoefficientExecutionDetails
                        .Where(d => d.isActive)
                        .Sum(d => d.valor),
                    isActive = true,
                });
            }

            productValuation.processed = true;
            productValuation.ProductionCostProductValuationExecutions = costProductValuationExecutions;

            // Procesamos los detalles de bodegas a valorizar
            var idCompany = this.ActiveCompanyId;
            var idDivision = this.ActiveDivision.id;
            var idSucursal = this.ActiveSucursal.id;

            var idValorizacionWarehouse = 1;
            var costProductValuationWarehouses = new List<ProductionCostProductValuationWarehouse>();
            foreach (var ejecucionCoeficiente in ejecucionesCoeficientes)
            {
                var año = ejecucionCoeficiente.anio;
                var mes = ejecucionCoeficiente.mes;
                var executionWarehouses = ejecucionCoeficiente
                    .ProductionCostCoefficientExecutionWarehouses
                    .Where(e => e.isActive)
                    .ToArray();

                foreach (var executionWarehouse in executionWarehouses)
                {
                    var idWarehouse = executionWarehouse.id_warehouse;
                    if (!costProductValuationWarehouses.Any(e => e.id_warehouse == idWarehouse))
                    {
                        var detalle = db.InventoryPeriodDetail
                            .FirstOrDefault(d => d.InventoryPeriod.id_warehouse == idWarehouse
                                && d.InventoryPeriod.year == año
                                && d.periodNumber == mes
                                && d.InventoryPeriod.id_Company == idCompany
                                && d.InventoryPeriod.id_Division == idDivision
                                && d.InventoryPeriod.id_BranchOffice == idSucursal
                                && d.InventoryPeriod.isActive);

                        costProductValuationWarehouses.Add(new ProductionCostProductValuationWarehouse()
                        {
                            id = idValorizacionWarehouse++,
                            id_warehouse = executionWarehouse.id_warehouse,
                            Warehouse = executionWarehouse.Warehouse,
                            id_periodState = detalle.id_PeriodState,
                            AdvanceParametersDetail = detalle.AdvanceParametersDetail,
                            process = true,
                            isActive = true,
                        });
                    }
                }
            }

            productValuation.ProductionCostProductValuationWarehouse = costProductValuationWarehouses;
        }

        private class ProcesamientoValorizacion
        {
            public int IdMotivoInventario { get; set; }
            public string Naturaleza { get; set; }
            public bool EsTransferencia { get; set; }
            public string Valorizacion { get; set; }
            public string TipoCalculo { get; set; }
        }

        private class DataProcesarInventario
        {
            public int IdInventoryMoveDetail { get; set; }
            public int IdWarehouse { get; set; }
            public int IdItem { get; set; }
            public DateTime EmissionDate { get; set; }
            public DateTime DateCreate { get; set; }
            public int ProcessCreate { get; set; }
            public int IdInventoryReason { get; set; }
            public string NatureMove { get; set; }
            public string MasterCode { get; set; }
            public int IdLote { get; set; }
            public decimal OrdenMotivoTipoDocum { get; set; }
            public decimal OrdenTransferencias { get; set; }
            public int? IdInventoryReasonCostManual { get; set; }
            public string ConfiguracionCostManual { get; set; }
            public string Proceso { get; set; }
            public int? PlantaProcesoId { get; set; }
            public int? ProductionCartId { get; set; }

            public decimal EntryAmount { get; set; }
            public decimal ExitAmount { get; set; }
            public bool IsResultingProduct { get; set; }

        }

        private class DataSalidaMPProceso
        {
            public int IdInventoryMoveDetail { get; set; }
            public int IdLoteOrigen { get; set; }
            public int IdWarehouse { get; set; }
            public int IdItem { get; set; }

        }

        private class CostoProcesar
        {
            public int Orden { get; set; }
            public int IdBodega { get; set; }
            public int[] PlantasProceso { get; set; }
            public int IdItem { get; set; }
            public decimal Cantidad { get; set; }
            public decimal CostoUnitario { get; set; }
            public decimal CostoTotal { get; set; }

            public int? LastProcessPlantId { get; set; }
            public decimal LastUnitCost { get; set; }

        }

         

        private class CostManualRelation
        {
            public string IdsIngresos { get; set; }
            public string IdsEgresos { get; set; }
        }

        private class CosteoValidacionesDinamicas
        {
            public int Id { get; set; }
            public int CompanyId { get; set; }
            public int WarehouseId { get; set; }
            public int InventoryReasonId { get; set; }
            public string EvaluateScript { get; set; }
            public string TypeAction { get; set; }
            public string ActionValue { get; set; }
            public DateTime DateInit { get; set; }
            public DateTime DateEnd { get; set; }
            public int id_userCreate { get; set; }
            public DateTime dateCreate { get; set; }
            public int id_userUpdate { get; set; }
            public DateTime dateUpdate { get; set; }

        }
        private DataProcesarInventario[] GetDataProcesarInventario(
            DateTime fechaInicio, DateTime fechaFin, int idBodega, int? idProducto)
        {
            // Obtenemos la data
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            #region Parámetros de búsqueda
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@fechaInicio",
                TipoDato = DbType.String,
                Valor = fechaInicio.ToString("yyyy-MM-dd"),
            });

            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@fechaFin",
                TipoDato = DbType.String,
                Valor = fechaFin.ToString("yyyy-MM-dd"),
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@idWarehouse",
                TipoDato = DbType.Int32,
                Valor = idBodega,
            });
            /*
             * 20203 08 03 
             * agregar filtro temporal para procesar por ITEM
             */
            //lstParametersSql.Add(new ParamSQL()
            //{
            //    Nombre = "@Producto",
            //    TipoDato = DbType.Int32,
            //    Valor = idProducto,
            //});


            #endregion

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            var procedimiento = ((idProducto.HasValue) && (idProducto > 0))
                ? m_procedureDataInventarioProducto
                : m_procedureDataInventario;


            DataSet dataSet = AccesoDatos.MSSQL.MetodosDatos2
                                .ObtieneDatos(_cadenaConexion, procedimiento, _rutaLog,
                                    m_CostProductValuationModelKey, "PROD", lstParametersSql);

            //DataSet dataSet = null;
            //try
            //{
            //    //DataSet dataSet = AccesoDatos.MSSQL.MetodosDatos2
            //    dataSet = AccesoDatos.MSSQL.MetodosDatos2
            //                    .ObtieneDatos(_cadenaConexion, procedimiento, _rutaLog,
            //                        m_CostProductValuationModelKey, "PROD", lstParametersSql);
            //}
            //catch (Exception e)
            //{
            //    var r = e;
            //}


            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                var resultados = dataSet.Tables[0].AsEnumerable();

                //var testItems = new int[] { 4747 }; 
                DataProcesarInventario[] result = null;
                try
                {
                    result = resultados
                                 .Select(e => new DataProcesarInventario()
                                 {
                                     IdInventoryMoveDetail = e.Field<Int32>("IdInventoryMoveDetail"),
                                     IdWarehouse = e.Field<Int32>("IdWarehouse"),
                                     IdItem = e.Field<Int32>("IdItem"),
                                     EmissionDate = e.Field<DateTime>("EmissionDate"),
                                     DateCreate = e.Field<DateTime>("DateCreate"),
                                     ProcessCreate = e.Field<Int32>("ProcessDate"),
                                     IdInventoryReason = e.Field<Int32>("IdInventoryReason"),
                                     NatureMove = e.Field<String>("NatureMove"),
                                     MasterCode = e.Field<String>("MasterCode"),
                                     IdLote = e.Field<Int32>("IdLote"),
                                     ConfiguracionCostManual = e.Field<String>("ConfiguracionCostManual"),
                                     IdInventoryReasonCostManual = e.Field<Int32?>("IdInventoryReasonCostManual"),
                                     Proceso = e.Field<String>("Proceso"),
                                     PlantaProcesoId = e.Field<Int32?>("PlantaProcesoId"),
                                     ProductionCartId = e.Field<Int32?>("ProductionCartId"),
                                     EntryAmount = e.Field<Decimal>("EntryAmount"),
                                     ExitAmount = e.Field<Decimal>("ExitAmount"),
                                 })
                                 .ToArray();
                }
                catch (Exception e)
                {
                    var f = e;
                }
                    
                
                return result
                        //.Where(r=> testItems.Contains(r.IdItem) ) // r.IdItem == 6970) // Test 2023
                        .ToArray();
            }
            else
            {
                return new DataProcesarInventario[] { };
            }
        }
        private DataSalidaMPProceso[] GetDataSalidaMPProceso(int id_lote)
        {
            // Obtenemos la data
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            #region Parámetros de búsqueda
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_lote",
                TipoDato = DbType.Int32,
                Valor = id_lote,
            });
            #endregion

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            DataSet dataSet = AccesoDatos.MSSQL.MetodosDatos2
                .ObtieneDatos(_cadenaConexion, m_procedureGetInventoryMoveExitMPProcess, _rutaLog,
                    m_CostProductValuationModelKey, "PROD", lstParametersSql);

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                var resultados = dataSet.Tables[0].AsEnumerable();

                return resultados
                    .Select(e => new DataSalidaMPProceso()
                    {
                        IdInventoryMoveDetail = e.Field<Int32>("IdInventoryMoveDetail"),
                        IdLoteOrigen = e.Field<Int32>("IdLoteOrigen"),
                        IdWarehouse = e.Field<Int32>("IdWarehouse"),
                        IdItem = e.Field<Int32>("IdItem"),
                    })
                    .ToArray();
            }
            else
            {
                return new DataSalidaMPProceso[] { };
            }
        }
        private int? GetInventoryMoveDetailMasterizado(int id_inventoryMove, int id_itemPT, int? id_lot, string tipo)
        {
            // Obtenemos la data
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            #region Parámetros de búsqueda
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_inventoryMove",
                TipoDato = DbType.Int32,
                Valor = id_inventoryMove,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_itemPT",
                TipoDato = DbType.Int32,
                Valor = id_itemPT,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_lot",
                TipoDato = DbType.Int32,
                Valor = id_lot,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@tipo",
                TipoDato = DbType.String,
                Valor = tipo,
            });
            #endregion

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            return AccesoDatos.MSSQL.MetodosDatos2
                .ExecuteIntFunction(_cadenaConexion, m_procedureGetInventoryMoveDetailMasterizado, _rutaLog,
                    m_CostProductValuationModelKey, "PROD", lstParametersSql);
        }
        private int? GetInventoryMoveDetailMasterizado(int id_inventoryMove, int id_itemPT, int? id_lot)
        {
            // Obtenemos la data
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            #region Parámetros de búsqueda
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_inventoryMove",
                TipoDato = DbType.Int32,
                Valor = id_inventoryMove,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_itemPT",
                TipoDato = DbType.Int32,
                Valor = id_itemPT,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_lot",
                TipoDato = DbType.Int32,
                Valor = id_lot,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@tipo",
                TipoDato = DbType.String,
                Valor = "IMD",
            });
            #endregion

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            return AccesoDatos.MSSQL.MetodosDatos2
                .ExecuteIntFunction(_cadenaConexion, m_procedureGetInventoryMoveDetailMasterizado, _rutaLog,
                    m_CostProductValuationModelKey, "PROD", lstParametersSql);
        }
        private int? GetDetalleMovimientoSalidaTransferenciaAutomatica(int id_inventoryMOveDetail)
        {
            // Obtenemos la data
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            #region Parámetros de búsqueda
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_inventoryMOveDetail",
                TipoDato = DbType.Int32,
                Valor = id_inventoryMOveDetail,
            });
            #endregion

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            return AccesoDatos.MSSQL.MetodosDatos2
                .ExecuteIntFunction(_cadenaConexion, m_procedureGetAutomaticTransferExit, _rutaLog,
                    m_CostProductValuationModelKey, "PROD", lstParametersSql);
        }
        private decimal? GetFactorInventoryMoveDetailMasterizado(int id_inventoryMove, int id_itemPT, int? id_lot)
        {
            // Obtenemos la data
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            #region Parámetros de búsqueda
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_inventoryMove",
                TipoDato = DbType.Int32,
                Valor = id_inventoryMove,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_itemPT",
                TipoDato = DbType.Int32,
                Valor = id_itemPT,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_lot",
                TipoDato = DbType.Int32,
                Valor = id_lot,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@tipo",
                TipoDato = DbType.String,
                Valor = "FAC",
            });
            #endregion

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            return AccesoDatos.MSSQL.MetodosDatos2
                .ExecuteDecimalProcedure(_cadenaConexion, m_procedureGetInventoryMoveDetailMasterizado, _rutaLog,
                    m_CostProductValuationModelKey, "PROD", lstParametersSql);
        }
        private decimal? GetCantidadMPDetailMasterizado(int id_inventoryMove, int id_itemPT, int? id_lot)
        {
            // Obtenemos la data
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            #region Parámetros de búsqueda
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_inventoryMove",
                TipoDato = DbType.Int32,
                Valor = id_inventoryMove,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_itemPT",
                TipoDato = DbType.Int32,
                Valor = id_itemPT,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_lot",
                TipoDato = DbType.Int32,
                Valor = id_lot,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@tipo",
                TipoDato = DbType.String,
                Valor = "CMP",
            });
            #endregion

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            return AccesoDatos.MSSQL.MetodosDatos2
                .ExecuteDecimalProcedure(_cadenaConexion, m_procedureGetInventoryMoveDetailMasterizado, _rutaLog,
                    m_CostProductValuationModelKey, "PROD", lstParametersSql);
        }
        private CostManualRelation[] GetCostManualRelation(int año, int mes, int idInventoryMoveDetail)
        {
            // Obtenemos la data
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            #region Parámetros de búsqueda
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@año",
                TipoDato = DbType.Int32,
                Valor = año,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@mes",
                TipoDato = DbType.Int32,
                Valor = mes,
            });
            lstParametersSql.Add(new ParamSQL()
            {
                Nombre = "@id_detalleIngreso",
                TipoDato = DbType.Int32,
                Valor = idInventoryMoveDetail,
            });
            #endregion

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            DataSet dataSet = AccesoDatos.MSSQL.MetodosDatos2
               .ObtieneDatos(_cadenaConexion, m_procedureGetCostManualRelation, _rutaLog,
                   m_CostProductValuationModelKey, "PROD", lstParametersSql);

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                var resultados = dataSet.Tables[0].AsEnumerable();

                return resultados
                    .Select(e => new CostManualRelation()
                    {
                        IdsIngresos = e.Field<String>("IdsIngresos"),
                        IdsEgresos = e.Field<String>("IdsEgresos"),
                    })
                    .ToArray();
            }
            else
            {
                return new CostManualRelation[] { };
            }
        }
        private void GetValoresCostoProcesar(   DateTime fechaConsulta,
                                                int? idBodega,
                                                int idItem,
                                                int id_allocationType,
                                                int? id_personProcessPlant,
                                                out decimal cantidadTotal,
                                                out decimal precioUnitario,
                                                out decimal costoTotal)
        {

            //int[] bodegasProcesos = null;
            //
            //switch (proceso)
            //{
            //    case "PROCESO 1":
            //        bodegasProcesos = new int[] { 29, 30 };
            //        break;
            //    case "PROCESO 2":
            //        bodegasProcesos = new int[] { 80, 81 };
            //        break;
            //} 

            // recuperamos el periodo de fecha anterior
            var fechaPeriodo = fechaConsulta.AddDays(-1);
            var fechaPeriodoInteger = fechaPeriodo.ToIntegerDate();

            //SaldoInicial[] saldoInicial = null;
            //if (bodegasProcesos != null)
            //{
            //    saldoInicial = db.SaldoInicial
            //                                    .Where(e => e.fechaCorteInteger == fechaPeriodoInteger
            //                                                && e.id_allocationType == id_allocationType
            //                                                && e.idItem == idItem
            //                                                && e.activo
            //                                                && bodegasProcesos.Contains(e.idBodega))
            //                                    .ToArray();
            //}
            //else 
            //{
            //    saldoInicial = db.SaldoInicial
            //                                    .Where(e => e.fechaCorteInteger == fechaPeriodoInteger
            //                                                && e.id_allocationType == id_allocationType
            //                                                && e.idItem == idItem && e.activo)
            //                                    .ToArray();
            //}

            //if ((saldoInicial?.Length ?? 0) == 0)
            //{
            SaldoInicial[] saldoInicial = null;

             string m_sql = $"select  * from SaldoInicial where fechaCorteInteger ={fechaPeriodoInteger} " +
                            $" AND id_allocationType ={id_allocationType} " +
                            $" AND idItem ={idItem} AND activo=1";

             var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
             using (var cnn = new SqlConnection(dapperDBContext))
             {
                 cnn.Open();

                 saldoInicial = cnn.Query<SaldoInicial>(m_sql).ToArray();

                 cnn.Close();
             };


            //var saldoInicial = db.SaldoInicial
            //                                    .Where(e => e.fechaCorteInteger == fechaPeriodoInteger
            //                                                && e.id_allocationType == id_allocationType
            //                                                && e.idItem == idItem && e.activo
            //                                                /*&& e.idBodega == idBodega*/
            //                                                );
            //.ToArray();
            //    esBodegaVirtual = true;
            //}
            var parametroMetodoValorizacion = getParametroMetodoValorizacion();
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
            {
                if (idBodega.HasValue)
                {
                    saldoInicial = saldoInicial
                                        .Where(e => e.idBodega == idBodega.Value)
                                        .ToArray();
                }
            }

            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                if (id_personProcessPlant.HasValue)
                {
                    saldoInicial = saldoInicial
                                            .Where(e => e.id_personProcessPlant == id_personProcessPlant.Value)
                                            .ToArray();
                }
            }


            if (saldoInicial.Any())
            {
                cantidadTotal = saldoInicial.Sum(e => e.cantidad);
                costoTotal = saldoInicial.Sum(e => e.costo_total);
                precioUnitario = cantidadTotal != 0
                    ? costoTotal / cantidadTotal
                    : 0m;
            }
            else
            {
                cantidadTotal = precioUnitario = costoTotal = 0m;
            }

        }

        private SaldoInicial[] getSaldoInicialTemporal(int ejecucion)
        {

           SaldoInicial[] saldoInicialTemporal = null;
           string m_sql = $"select  * from SaldoInicialTemporal where id_ejecucion = {ejecucion} ";

           var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
           using (var cnn = new SqlConnection(dapperDBContext))
           {
               cnn.Open();

               saldoInicialTemporal = cnn.Query<SaldoInicial>(m_sql).ToArray();

               cnn.Close();
           }

           return saldoInicialTemporal;


        }

        private void RecuperarSaldos(   ref List<CostoProcesar> saldos,
                                        DateTime emissionDate,
                                        int idBodega,
                                        int idItem,
                                        int id_allocationType,
                                        int? id_personProcessPlant)
        {
            var parametroMetodoValorizacion = getParametroMetodoValorizacion();
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                idBodega = 0;
            }

            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
            {
                id_personProcessPlant = null;
            }

            var index = saldos
                            .FindIndex(e => e.IdItem == idItem
                                            && e.IdBodega == idBodega
                                            && (id_personProcessPlant.HasValue && e.LastProcessPlantId == id_personProcessPlant.Value)
                                            );
                                            //&& ((id_personProcessPlant.HasValue && e.PlantasProceso.Contains(id_personProcessPlant.Value)) || !id_personProcessPlant.HasValue));
            if (index < 0)
            {
                this.GetValoresCostoProcesar(   emissionDate,
                                                idBodega,
                                                idItem,
                                                id_allocationType,
                                                id_personProcessPlant,
                                                out var cantidadAnterior,
                                                out var costoUnitarioAnterior,
                                                out var costoTotalAnterior);


                saldos.Add(new CostoProcesar()
                {
                    IdBodega = idBodega,
                    IdItem = idItem,
                    Cantidad = cantidadAnterior,
                    CostoUnitario = costoUnitarioAnterior,
                    CostoTotal = costoTotalAnterior,
                    PlantasProceso = Array.Empty<int>(),
                    LastProcessPlantId = id_personProcessPlant,
                    LastUnitCost = costoUnitarioAnterior
                });

                //if (!id_personProcessPlant.HasValue)
                //{
                //    saldos.Add(new CostoProcesar()
                //    {
                //        IdBodega = idBodega,
                //        IdItem = idItem,
                //        Cantidad = cantidadAnterior,
                //        CostoUnitario = costoUnitarioAnterior,
                //        CostoTotal = costoTotalAnterior,
                //        PlantasProceso = Array.Empty<int>()
                //    });
                //}
                //else
                //{
                //    var saldoRegistro = saldos.FirstOrDefault(e => e.IdItem == idItem && e.IdBodega == idBodega);
                //    if (saldoRegistro == null)
                //    {
                //        saldos.Add(new CostoProcesar()
                //        {
                //            IdBodega = idBodega,
                //            IdItem = idItem,
                //            Cantidad = cantidadAnterior,
                //            CostoUnitario = costoUnitarioAnterior,
                //            CostoTotal = costoTotalAnterior,
                //            PlantasProceso = new int[] { id_personProcessPlant.Value },
                //            LastProcessPlantId= id_personProcessPlant.Value
                //
                //        });
                //
                //    }
                //    else
                //    {
                //        decimal costoTotal = saldoRegistro.CostoTotal + costoTotalAnterior;
                //        decimal cantidadTotal = saldoRegistro.Cantidad + cantidadAnterior;
                //        decimal precioUnitario = cantidadTotal != 0
                //                                        ? costoTotal / cantidadTotal
                //                                        : 0m;
                //
                //        var plantas = saldoRegistro.PlantasProceso.ToList();
                //        plantas.Add(id_personProcessPlant.Value);
                //        saldoRegistro.PlantasProceso = plantas.ToArray();
                //        saldoRegistro.CostoTotal = costoTotal;
                //        saldoRegistro.Cantidad = cantidadTotal;
                //        saldoRegistro.CostoUnitario = precioUnitario;
                //        saldoRegistro.LastProcessPlantId = id_personProcessPlant.Value;
                //    }
                //
                //}


            }
        }

        private void ProcesarValorizacionBodegas(ProductionCostProductValuation productValuation,
            DateTime? fechaInicio, DateTime? fechaFin, int? idProducto, int[] idsWarehouse, bool esAprobacion = false)
        {
            var parametroMetodoValorizacion = getParametroMetodoValorizacion();
            // Removemos la data del detalle de movimiento
            if (this.TempData.ContainsKey(m_inventoryMoveDetailDTO))
            {
                this.TempData.Remove(m_inventoryMoveDetailDTO);
            }

           

            // Preparamos variables a utilizar
            var idCompany = this.ActiveCompanyId;
            var idDivision = this.ActiveDivision.id;
            var idSucursal = this.ActiveSucursal.id;
            var año = productValuation.anio;
            var mes = productValuation.mes;

            List<DataProcesarInventario> datosProcesadosTodasFechas = new List<DataProcesarInventario>();

            // Códigos de documentos a procesar
            var codesDocumentosValidos = new[]
            {
                "02", // Aprobado parcial
                "03", // Aprobado
                "04", // Cerrado
                "06", // Autorizado
                "16"  // Conciliado
            };

            // Procesamos las bodegas
            var novedades = new List<ProductionCostProductValuationWarning>();
            var productionsCostProductValuation = new List<ProductionCostProductValuationInventoryMove>();
            var datosTotProcesar = new List<DataProcesarInventario>();
            var numsWarehouses = idsWarehouse?.Length ?? 0;

            // Obtener todos los ordenamientos, almacenar en un buffer
            CosteoValidacionesDinamicas[] casosValidacionesDinamicas = null;
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                getOrdenamientoCodigo(año, mes);
                getValidacionExcepcion();
                #region Test Absorcion Costo
                //getCostoMovimientoList(); // Test
                #endregion
                casosValidacionesDinamicas = getValidacionesDinamicas();
            }

            List<Tuple<int, string, int>> procesosBodegas = new List<Tuple<int, string, int>>();


            for (int i = 0; i < numsWarehouses; i++)
            {
                var idWarehouse = idsWarehouse[i];
                var resultDetallesPeriodo = db.InventoryPeriodDetail
                                            .Include(r => r.AdvanceParametersDetail)
                                            .Where(d => d.InventoryPeriod.id_warehouse == idWarehouse
                                                        && d.InventoryPeriod.year == año
                                                        && d.dateInit.Month == mes
                                                        && d.dateEnd.Month == mes
                                                        && d.InventoryPeriod.id_Company == idCompany
                                                        && d.InventoryPeriod.id_Division == idDivision
                                                        && d.InventoryPeriod.id_BranchOffice == idSucursal
                                                        && d.isClosed && d.InventoryPeriod.isActive);

                if ((resultDetallesPeriodo.ToArray()?.Length ?? 0) == 0)
                {
                    var bodega = this.GetWarehouseFromTempData(idWarehouse);
                    throw new Exception($"No existe periodo de inventario para la Bodega: {bodega.code}");
                }

                string tipoPeriodoInventario = resultDetallesPeriodo
                                                         .FirstOrDefault()
                                                         .InventoryPeriod
                                                         .AdvanceParametersDetail
                                                         .valueCode;

                var detallesPeriodo = resultDetallesPeriodo
                                                .ToArray();

                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    var fechaPivote = fechaInicio.Value;
                    while (fechaPivote <= fechaFin.Value)
                    {
                        if (tipoPeriodoInventario.Equals(m_tipoPeriodoInventarioDiario))
                        {
                            if (!detallesPeriodo.Any(e => e.dateInit >= fechaPivote && e.dateEnd <= fechaPivote))
                            {
                                var bodega = this.GetWarehouseFromTempData(idWarehouse);
                                throw new Exception($"El día: {fechaPivote.ToDateFormat()} no se encuentra en un periodo válido para la bodega: {bodega.name}.");
                            }
                        }

                        datosTotProcesar.AddRange(this.GetDataProcesarInventario(fechaPivote, fechaPivote, idWarehouse, idProducto));

                        fechaPivote = fechaPivote.AddDays(1);

                    }
                }
                else
                {
                    throw new Exception("No se ha establecido un rango de ejecución válido.");
                }
            }

            // Procesamos
            #region Procesamos los detalles de inventario
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                datosTotProcesar = datosTotProcesar
                                        .OrderBy(e => e.ProcessCreate)
                                        .ThenBy(e => e.EmissionDate.Date)
                                        .ThenBy(e => e.DateCreate)
                                        .ToList();
            }
            else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
            {
                datosTotProcesar = datosTotProcesar
                                        .OrderBy(e => e.EmissionDate.Date)
                                        .ThenBy(e => e.ProcessCreate)
                                        .ThenBy(e => e.DateCreate)
                                        .ToList();
            }


            var fechaInicialProcesamiento = datosTotProcesar.Any()
                ? datosTotProcesar.Min(e => e.EmissionDate) : new DateTime();
            var fechaFinalProcesamiento = datosTotProcesar.Any()
                ? datosTotProcesar.Max(e => e.EmissionDate) : new DateTime();


            // Eliminar Saldo Temporal
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                deleteSaldoTemporal(fechaFinalProcesamiento.ToIntegerDate());
            }

            var saldos = new List<CostoProcesar>();
            int ordenAsigCost = 0;

            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                procesarMovimientoMetodoProceso(datosTotProcesar,
                                                    fechaFinalProcesamiento,
                                                    fechaInicialProcesamiento,
                                                    casosValidacionesDinamicas,
                                                    fechaInicio,
                                                    productValuation,
                                                    ref datosProcesadosTodasFechas,
                                                    ref saldos,
                                                    ref productionsCostProductValuation,
                                                    ref novedades,
                                                    ref ordenAsigCost);
            }
            else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
            {
                procesarMovimientoMetodoLote(datosTotProcesar,
                                                fechaFinalProcesamiento,
                                                fechaInicialProcesamiento,
                                                fechaInicio,
                                                productValuation,
                                                ref saldos,
                                                ref productionsCostProductValuation,
                                                ref novedades,
                                                ref ordenAsigCost);
            }


            #endregion

            this.TempData[m_CostOrderSpecialList] = datosProcesadosTodasFechas;
            this.TempData.Keep(m_CostOrderSpecialList);

            // Procesar los saldos pendientes
            var fechaCorte = fechaInicio.Value.AddDays(-1);
            var productionsCostProductValuationBalance = this.CalcularSaldoValorizacion(
                fechaCorte, productValuation.id_allocationType, productionsCostProductValuation, datosTotProcesar)
                .Select(e => new ProductionCostProductValuationBalance()
                {
                    idBodega = e.IdBodega,
                    idItem = e.IdItem,
                    cantidad = Convert.ToInt32(e.Cantidad),
                    precioUnitario = e.CostoUnitario,
                    costoTotal = e.CostoTotal,
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                    id_personProcessPlant = e.LastProcessPlantId
                })
                .ToList();

            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso) && esAprobacion)
            {
                registerSaldoTemporal(
                    productionsCostProductValuationBalance.ToArray(), 
                    fechaFinalProcesamiento.ToIntegerDate()
                    );
            }

                // Reagrupamos las novedades
                int ordenNovedades = 1;
            novedades = novedades
                .GroupBy(e => e.descripcion)
                .Select(e => new ProductionCostProductValuationWarning()
                {
                    orden = ordenNovedades++,
                    descripcion = e.Key,
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                })
                .ToList();

            // Procesamiento de movimientos de inventarios
            productValuation.ProductionCostProductValuationInventoryMove = productionsCostProductValuation;
            productValuation.ProductionCostProductValuationWarning = novedades;
            productValuation.ProductionCostProductValuationBalance = productionsCostProductValuationBalance;

            // Procesamiento de bodegas
            var bodegas = productValuation.ProductionCostProductValuationWarehouse.ToArray();
            for (int i = 0; i < bodegas.Length; i++)
            {
                bodegas[i].process = idsWarehouse.Contains(bodegas[i].id_warehouse);
            }
            productValuation.ProductionCostProductValuationWarehouse = bodegas;

            // Grabar en memoria temporal la conversión a DTO
            this.SaveProductValuationInventoryMoveDTOTempData(productionsCostProductValuation.OrderBy(e => e.orden).ToArray());
        }

        private void procesarMovimientoMetodoLote(List<DataProcesarInventario> datosTotProcesar,
                                                    DateTime fechaFinalProcesamiento,
                                                    DateTime fechaInicialProcesamiento,
                                                    DateTime? fechaInicio,
                                                    ProductionCostProductValuation productValuation,

                                                    ref List<CostoProcesar> saldos,
                                                    ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
                                                    ref List<ProductionCostProductValuationWarning> novedades,
                                                    ref int ordenAsigCost)
        {
            while (fechaInicialProcesamiento <= fechaFinalProcesamiento)
            {
                var datosProcesar = datosTotProcesar
                    .Where(e => DateTime.Compare(e.EmissionDate.Date, fechaInicialProcesamiento.Date) == 0)
                    .ToArray();
                var idsMovimientosProcesar = datosProcesar.Select(e => e.IdInventoryMoveDetail).ToArray();

                int idProductoAnterior = 0, idBodegaAnterior = 0;
                foreach (var datoProcesar in datosProcesar)
                {
                    try
                    {
                        // Verificamos si es un nuevo producto
                        if (idProductoAnterior != datoProcesar.IdItem
                            || idBodegaAnterior != datoProcesar.IdWarehouse)
                        {
                            idProductoAnterior = datoProcesar.IdItem;
                            idBodegaAnterior = datoProcesar.IdWarehouse;

                            // Si no existe un registro de saldo en la colección, se extrae desde la base de datos.
                            this.RecuperarSaldos(ref saldos,
                                                    fechaInicio ?? datoProcesar.EmissionDate,
                                                    datoProcesar.IdWarehouse,
                                                    datoProcesar.IdItem,
                                                    productValuation.id_allocationType,
                                                    null);
                        }

                        var parametroCalculo = this.GetParametroCalculo(datoProcesar.IdInventoryReason);
                        this.ProcesarTipoCalculoValorizacion(productValuation, datoProcesar,
                            parametroCalculo, ref saldos, ref productionsCostProductValuation,
                            novedades, ref ordenAsigCost, null, null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error al procesar la valorización.", ex);
                    }
                }

                // Reprocesamos los registros en cero
                int numReprocesamientos = 5; // Obtener desde parámetro 
                int numErrores = 0;
                for (int i = 0; i <= numReprocesamientos; i++)
                {
                    var registrosCero = productionsCostProductValuation
                        .Where(e => idsMovimientosProcesar.Contains(e.idInventoryMoveDetail))
                        .Where(e => e.costoUnitario == 0m)
                        .OrderBy(e => e.orden)
                        .ToArray();
                    var numFilasCero = registrosCero.Length;

                    if (!registrosCero.Any() || numErrores == numFilasCero) break;

                    numErrores = numFilasCero;

                    // Reprocesamos los registros en cero
                    foreach (var registroCero in registrosCero)
                    {
                        try
                        {
                            var detalleInventario = this.GetInventoryMoveDetailFromTempData(registroCero.idInventoryMoveDetail);
                            var movimientoInventario = detalleInventario.InventoryMove;
                            var fechaEmisionInteger = movimientoInventario.Document.emissionDate.ToIntegerDate();
                            var parametroCalculo = this.GetParametroCalculo(movimientoInventario.id_inventoryReason ?? 0);

                            // Acortamos la lista de registros ya procesada a la fecha del movimiento actual
                            var detallesProcesar = productionsCostProductValuation
                                .Where(e => this.GetInventoryMoveDetailFromTempData(e.idInventoryMoveDetail).InventoryMove.Document.emissionDate.ToIntegerDate() <= fechaEmisionInteger)
                                .ToList();

                            // Instanciamos el registro a procesar
                            var datoProcesar = new DataProcesarInventario()
                            {
                                IdInventoryMoveDetail = registroCero.idInventoryMoveDetail,
                                IdItem = detalleInventario.id_item,
                                IdLote = detalleInventario.id_lot ?? 0,
                                NatureMove = registroCero.naturaleza,
                                IdWarehouse = detalleInventario.id_warehouse,
                                IdInventoryReason = movimientoInventario.id_inventoryReason ?? 0,
                                EmissionDate = detalleInventario.InventoryMove.Document.emissionDate,
                            };

                            // Preparamos el índice del registro a actualizar
                            int? indiceReproceso = detallesProcesar
                                .FindIndex(e => e.idInventoryMoveDetail == registroCero.idInventoryMoveDetail);
                            if (indiceReproceso < 0) indiceReproceso = null;

                            this.ProcesarTipoCalculoValorizacion(productValuation, datoProcesar,
                                parametroCalculo, ref saldos, ref detallesProcesar,
                                novedades, ref ordenAsigCost, indiceReproceso, null);

                            // Asignamos el costo reprocesado a la lista original
                            int? indexOriginal = productionsCostProductValuation
                                .FindIndex(e => e.orden == registroCero.orden);
                            if (indexOriginal < 0) indexOriginal = null;
                            if (indexOriginal.HasValue)
                            {
                                productionsCostProductValuation[indexOriginal.Value].costoUnitario = detallesProcesar[indiceReproceso.Value].costoUnitario;
                                productionsCostProductValuation[indexOriginal.Value].costoTotal = detallesProcesar[indiceReproceso.Value].costoTotal;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error al reprocesar la valorización.", ex);
                        }
                    }

                    // Procesamos los ingresos manuales
                    RecalcularCostosIngresoManuales(ref ordenAsigCost, datosProcesar,
                        ref saldos, ref novedades, ref productionsCostProductValuation);
                }

                fechaInicialProcesamiento = fechaInicialProcesamiento.AddDays(1);
            }
        }
        private void procesarMovimientoMetodoProceso(List<DataProcesarInventario> datosTotProcesar,
                                                        DateTime fechaFinalProcesamiento,
                                                        DateTime fechaInicialProcesamiento,
                                                        CosteoValidacionesDinamicas[] casosValidacionesDinamicas,
                                                        DateTime? fechaInicio,
                                                        ProductionCostProductValuation productValuation,

                                                        ref List<DataProcesarInventario> datosProcesadosTodasFechas,
                                                        ref List<CostoProcesar> saldos,
                                                        ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
                                                        ref List<ProductionCostProductValuationWarning> novedades,
                                                        ref int ordenAsigCost)
        {
            foreach (var proceso in datosTotProcesar.GroupBy(r => r.Proceso).Select(r => r.Key).OrderByDescending(r => r))
            //foreach (var proceso in datosTotProcesar.Where(r => r.Proceso == "PROCESO 2").Select(r => r.Proceso).ToList())
            {
                int idProductoAnterior = 0, idPlantaProcesoAnterior = 0;

                while (fechaInicialProcesamiento <= fechaFinalProcesamiento)
                {
                    var datosProcesar = datosTotProcesar
                                                .Where(e => DateTime.Compare(e.EmissionDate.Date, fechaInicialProcesamiento.Date) == 0 && e.Proceso == proceso)
                                                .ToList();

                    var idsMovimientosProcesar = datosProcesar.Select(e => e.IdInventoryMoveDetail).ToArray();

                    #region Test Code
                    //getInventoryMoveDetailBulk(idsMovimientosProcesar);
                    #endregion

                    // Aplicamos el orden
                    var datosProcesarPre = new List<DataProcesarInventario>();
                    datosProcesar.ForEach(e =>
                    {
                        try
                        {
                            e.OrdenMotivoTipoDocum = this.GetOrdenBodegaMotivo(e.IdWarehouse,
                                                                                e.IdInventoryReason,
                                                                                e.IdInventoryMoveDetail,
                                                                                e.NatureMove,
                                                                                e,
                                                                                datosProcesar,
                                                                                out var ordenTransferencias);
                            e.OrdenTransferencias = ordenTransferencias ?? 0;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Ha ocurrido un error en ordenamiento: {ex.Message}, Movimiento: {e.IdInventoryMoveDetail}");
                        }

                    });

                    /*
                     * TEMP Incluir columna en saldo inicial proceso bodega proceso
                     */

                    datosProcesar = datosProcesar
                                        .OrderBy(e => e.OrdenMotivoTipoDocum)
                                        .ThenBy(e => e.OrdenTransferencias)
                                        .ToList();

                    var datosProcesar2 = ejecutarValidacionesDinamicas(datosProcesar, casosValidacionesDinamicas);
                    // m_CostOrderSpecialList
                    datosProcesadosTodasFechas.AddRange(datosProcesar2);

                    foreach (var datoProcesar in datosProcesar2)
                    {
                        try
                        {

                            if (idProductoAnterior != datoProcesar.IdItem || idPlantaProcesoAnterior != datoProcesar.PlantaProcesoId)
                            {
                                idProductoAnterior = datoProcesar.IdItem;
                                idPlantaProcesoAnterior = (datoProcesar.PlantaProcesoId??0);

                                try
                                {
                                    this.RecuperarSaldos(   ref saldos,
                                                            fechaInicio ?? datoProcesar.EmissionDate,
                                                            datoProcesar.IdWarehouse,
                                                            datoProcesar.IdItem,
                                                            productValuation.id_allocationType,
                                                            datoProcesar.PlantaProcesoId
                                                            );
                                }
                                catch (Exception e)
                                {
                                    var g = e;
                                }


                            }

                            var parametroCalculo = this.GetParametroCalculo(datoProcesar.IdInventoryReason);
                            try
                            {
                                #region Test Code
                                // Validar si el movimiento tiene costo calculado
                                // Tiene Costo Calculado: generar el registro ProductValuation
                                // y actualizar el saldo

                                //var movimientoCosto = _costosMovimientosTest.FirstOrDefault(r => r.InventoryMoveDetailId == datoProcesar.IdInventoryMoveDetail);
                                //if (movimientoCosto != null)
                                //{
                                //    var inventoryMoveDetail = _inventoryMoveDetailTest.FirstOrDefault(r => r.id == datoProcesar.IdInventoryMoveDetail);
                                //
                                //    decimal cantidadMovimiento = (inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount);
                                //    var costoTotal = GlobalCalculator.RedondearMontoTotalPrecision(cantidadMovimiento * movimientoCosto.PrecioUnitario);
                                //    costoTotal = Math.Abs(costoTotal);
                                //
                                //    var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                //        datoProcesar.IdWarehouse, datoProcesar.IdItem, cantidadMovimiento, costoTotal);
                                //
                                //    productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                                //    {
                                //        orden = productionsCostProductValuation.Count + 1,
                                //        ordenAsigCost = ordenAsigCost++,
                                //        idInventoryMoveDetail = inventoryMoveDetail.id,
                                //        idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                                //        idBodega = inventoryMoveDetail.id_warehouse,
                                //        idItem = datoProcesar.IdItem,
                                //        idMotivo = datoProcesar.IdInventoryReason,
                                //        esTransferencia = parametroCalculo.EsTransferencia,
                                //        naturaleza = parametroCalculo.Naturaleza,
                                //        tipoCalculo = parametroCalculo.TipoCalculo,
                                //        valorizacion = parametroCalculo.Valorizacion,
                                //        accion = m_MantenerCostoAsignadoKey,
                                //        id_lot = inventoryMoveDetail.id_lot ?? 0,
                                //        costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                                //        costoTotalAnterior = inventoryMoveDetail.balanceCost,
                                //
                                //        cantidad = cantidadMovimiento,
                                //        costoTotal = costoTotal,
                                //        costoUnitario = movimientoCosto.PrecioUnitario,
                                //        activo = true,
                                //
                                //        idUsuarioCreacion = this.ActiveUserId,
                                //        fechaHoraCreacion = DateTime.Now,
                                //        idUsuarioModificacion = this.ActiveUserId,
                                //        fechaHoraModificacion = DateTime.Now,
                                //    });
                                //}
                                #endregion
                                //else
                                //{
                                this.ProcesarTipoCalculoValorizacion(productValuation, datoProcesar,
                                    parametroCalculo, ref saldos, ref productionsCostProductValuation,
                                    novedades, ref ordenAsigCost, null, datosProcesar2);
                                #region Test Code II
                                //}
                                #endregion

                            }
                            catch (Exception e)
                            {
                                var r = e;
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error al procesar la valorización.", ex);
                        }
                    }

                    // Reprocesamos los registros en cero
                    int numReprocesamientos = 5; // Obtener desde parámetro 
                    int numErrores = 0;
                    for (int i = 0; i <= numReprocesamientos; i++)
                    {
                        var registrosCero = productionsCostProductValuation
                            .Where(e => idsMovimientosProcesar.Contains(e.idInventoryMoveDetail))
                            .Where(e => e.costoUnitario == 0m)
                            .OrderBy(e => e.orden)
                            .ToArray();
                        var numFilasCero = registrosCero.Length;

                        if (!registrosCero.Any() || numErrores == numFilasCero) break;

                        numErrores = numFilasCero;

                        // Reprocesamos los registros en cero
                        foreach (var registroCero in registrosCero)
                        {
                            try
                            {
                                var detalleInventario = this.GetInventoryMoveDetailFromTempData(registroCero.idInventoryMoveDetail);
                                var movimientoInventario = detalleInventario.InventoryMove;
                                var fechaEmisionInteger = movimientoInventario.Document.emissionDate.ToIntegerDate();
                                var parametroCalculo = this.GetParametroCalculo(movimientoInventario.id_inventoryReason ?? 0);

                                // Acortamos la lista de registros ya procesada a la fecha del movimiento actual
                                var detallesProcesar = productionsCostProductValuation
                                    .Where(e => this.GetInventoryMoveDetailFromTempData(e.idInventoryMoveDetail).InventoryMove.Document.emissionDate.ToIntegerDate() <= fechaEmisionInteger)
                                    .ToList();

                                // Instanciamos el registro a procesar
                                var datoProcesar = new DataProcesarInventario()
                                {
                                    IdInventoryMoveDetail = registroCero.idInventoryMoveDetail,
                                    IdItem = detalleInventario.id_item,
                                    IdLote = detalleInventario.id_lot ?? 0,
                                    NatureMove = registroCero.naturaleza,
                                    IdWarehouse = detalleInventario.id_warehouse,
                                    IdInventoryReason = movimientoInventario.id_inventoryReason ?? 0,
                                    EmissionDate = detalleInventario.InventoryMove.Document.emissionDate,
                                    IsResultingProduct = registroCero.IsResultingProduct,
                                    PlantaProcesoId = datosTotProcesar.FirstOrDefault(r=> r.IdInventoryMoveDetail == registroCero.idInventoryMoveDetail)?.PlantaProcesoId

                                };

                                // Preparamos el índice del registro a actualizar
                                int? indiceReproceso = detallesProcesar
                                    .FindIndex(e => e.idInventoryMoveDetail == registroCero.idInventoryMoveDetail);
                                if (indiceReproceso < 0) indiceReproceso = null;

                                this.ProcesarTipoCalculoValorizacion(productValuation, datoProcesar,
                                    parametroCalculo, ref saldos, ref detallesProcesar,
                                    novedades, ref ordenAsigCost, indiceReproceso, datosProcesar2);

                                // Asignamos el costo reprocesado a la lista original
                                int? indexOriginal = productionsCostProductValuation
                                    .FindIndex(e => e.orden == registroCero.orden);
                                if (indexOriginal < 0) indexOriginal = null;
                                if (indexOriginal.HasValue)
                                {
                                    productionsCostProductValuation[indexOriginal.Value].costoUnitario = detallesProcesar[indiceReproceso.Value].costoUnitario;
                                    productionsCostProductValuation[indexOriginal.Value].costoTotal = detallesProcesar[indiceReproceso.Value].costoTotal;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Error al reprocesar la valorización.", ex);
                            }
                        }

                        // Procesamos los ingresos manuales
                        RecalcularCostosIngresoManuales(ref ordenAsigCost, datosProcesar2,
                            ref saldos, ref novedades, ref productionsCostProductValuation);

                        //RecalcularCostosManualesExcepciones(ref ordenAsigCost,
                        //                                    datosProcesar2,
                        //                                    datosProcesadosTodasFechas,
                        //                                    ref saldos,
                        //                                    ref novedades,
                        //                                    ref productionsCostProductValuation);

                    }

                    fechaInicialProcesamiento = fechaInicialProcesamiento.AddDays(1);
                }

                fechaInicialProcesamiento = datosTotProcesar.Any()
                                                ? datosTotProcesar.Min(e => e.EmissionDate) : new DateTime();
            }
        }
        private void ProcesarTipoCalculoValorizacion(
            ProductionCostProductValuation productValuation, DataProcesarInventario datoProcesar,
            ProcesamientoValorizacion parametroCalculo, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades, ref int ordenAsigCost, int? indiceReproceso,
            List<DataProcesarInventario> dataOrdenamiento)
        {

            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
            int[] idsReglasExcepcionManualMetodoProceso = null;
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                idsReglasExcepcionManualMetodoProceso = _validacionCosteoManualExcepcion
                                                                  .Select(r => r.EntryInventoryReasonId)
                                                                  .ToArray();

            }

            string[] specialTipoCalculoMasterizados = new string[] { m_TipoCalculoAbsorcionPTKey, m_TipoCalculoAbsorcionSBKey };
            // Valorización manual
            if (parametroCalculo.Valorizacion == m_ValorizacionManualKey
                 && parametroCalculo.Naturaleza == m_NaturalezaIngresoKey
                 && !parametroCalculo.EsTransferencia
                 && ((parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso)
                      && !idsReglasExcepcionManualMetodoProceso.Contains(datoProcesar.IdInventoryReason))
                      || parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))

                )
            {
                this.MantenerCostoAsignado(datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation, novedades,
                    ref ordenAsigCost, indiceReproceso);
            }
            // Valorizacion Metodo Proceso Regla Excepciones Manuales
            else if (parametroCalculo.Valorizacion == m_ValorizacionManualKey
                 && parametroCalculo.Naturaleza == m_NaturalezaIngresoKey
                 && !parametroCalculo.EsTransferencia
                 && ((parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso)
                      && idsReglasExcepcionManualMetodoProceso.Contains(datoProcesar.IdInventoryReason))
                      || parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote)))
            {
                var configuracionExcepcion = _validacionCosteoManualExcepcion
                                                                  .FirstOrDefault(r => r.EntryInventoryReasonId == datoProcesar.IdInventoryReason);

                this.CalularCostosManualesExcepciones(  datoProcesar,
                                                        parametroCalculo,
                                                        configuracionExcepcion,
                                                        ref saldos,
                                                        ref productionsCostProductValuation,
                                                        novedades,
                                                        ref ordenAsigCost,
                                                        indiceReproceso);
            }

            // Valorizacion Especial Masterizados
            else if (parametroCalculo.Valorizacion == m_ValorizacionAutomaticaKey
                    && specialTipoCalculoMasterizados.Contains(parametroCalculo.TipoCalculo)
                    && parametroCalculo.Naturaleza == m_NaturalezaIngresoKey)
            {
                this.AsignarCostoCoeficiente(
                    productValuation, datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation, novedades,
                    ref ordenAsigCost, indiceReproceso, dataOrdenamiento);

            }
            // Valorización automática
            // Valorización promedio
            else if (parametroCalculo.Valorizacion == m_ValorizacionAutomaticaKey
                && parametroCalculo.TipoCalculo == m_TipoCalculoPromedioKey
                && parametroCalculo.Naturaleza == m_NaturalezaIngresoKey
                && !parametroCalculo.EsTransferencia)
            {
                this.AsignarCostoPromedio(datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation, novedades,
                    ref ordenAsigCost, indiceReproceso);
            }
            else if (parametroCalculo.Valorizacion == m_ValorizacionAutomaticaKey
                && parametroCalculo.TipoCalculo == m_TipoCalculoPromedioKey
                && parametroCalculo.Naturaleza == m_NaturalezaIngresoKey
                && parametroCalculo.EsTransferencia)
            {
                this.AsignarCostoEgresoTransferencia(datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation, novedades,
                    ref ordenAsigCost, indiceReproceso);
            }
            else if (parametroCalculo.Valorizacion == m_ValorizacionAutomaticaKey
                && parametroCalculo.TipoCalculo == m_TipoCalculoPromedioKey
                && parametroCalculo.Naturaleza == m_NaturalezaEgresoKey)
            {
                this.AsignarCostoPromedio(datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation, novedades,
                    ref ordenAsigCost, indiceReproceso);
            }
            // Valorización automática
            // Valorización proceso
            else if (parametroCalculo.Valorizacion == m_ValorizacionAutomaticaKey
                && parametroCalculo.TipoCalculo == m_TipoCalculoProcesoKey
                && parametroCalculo.Naturaleza == m_NaturalezaIngresoKey)
            {
                this.AsignarCostoCoeficiente(
                    productValuation, datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation, novedades,
                    ref ordenAsigCost, indiceReproceso, dataOrdenamiento);
            }
            else if (parametroCalculo.Valorizacion == m_ValorizacionAutomaticaKey
                && parametroCalculo.TipoCalculo == m_TipoCalculoProcesoKey
                && parametroCalculo.Naturaleza == m_NaturalezaEgresoKey)
            {
                this.AsignarCostoProceso(datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation,
                    novedades, dataOrdenamiento, ref ordenAsigCost, indiceReproceso);
            }
            // Valorización automática
            // Valorización Heredado
            else if (parametroCalculo.Valorizacion == m_ValorizacionAutomaticaKey
                && parametroCalculo.TipoCalculo == m_TipoCalculoHeredadoKey
                && parametroCalculo.Naturaleza == m_NaturalezaEgresoKey)
            {
                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                {
                    AsignarCostoHeredadoMetodoProceso(datoProcesar,
                                                        parametroCalculo,
                                                        ref saldos,
                                                        ref productionsCostProductValuation,
                                                        novedades,
                                                        false,
                                                        ref ordenAsigCost,
                                                        indiceReproceso);
                }
                else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
                {
                    AsignarCostoHeredadoMetodoLote(datoProcesar,
                                                    parametroCalculo,
                                                    ref saldos,
                                                    ref productionsCostProductValuation,
                                                    novedades,
                                                    false,
                                                    ref ordenAsigCost,
                                                    indiceReproceso);
                }
            }
            else if (parametroCalculo.Valorizacion == m_ValorizacionAutomaticaKey
                && parametroCalculo.TipoCalculo == m_TipoCalculoHeredadoKey
                && parametroCalculo.Naturaleza == m_NaturalezaIngresoKey)
            {
                this.AsignarCostoHeredado(datoProcesar,
                    parametroCalculo, ref saldos, ref productionsCostProductValuation,
                    novedades, true, ref ordenAsigCost, indiceReproceso);
            }
            else
            {
                var motivo = this.GetInventoryReasonFromTempData(parametroCalculo.IdMotivoInventario);
                novedades.Add(new ProductionCostProductValuationWarning()
                {
                    idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                    orden = novedades.Count + 1,
                    descripcion = $"Verificar configuración de motivo: {motivo.name}. Tipo Valorización: {parametroCalculo.Valorizacion}.",
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                });
            }
        }

        private void MantenerCostoAsignado(DataProcesarInventario datoProcesar,
            ProcesamientoValorizacion parametroCalculo, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades,
            ref int ordenAsigCost, int? indiceReproceso)
        {
            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            if (inventoryMoveDetail == null)
            {
                throw new Exception("Error al recuperar el detalle del movimiento de inventario.");
            }

            var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
            if (inventoryMoveDetail.unitPrice <= 0m)
            {
                this.SetErrorCostoCero(datoProcesar, novedades);
            }

            // Intentamos recuperar el costo unitario desde los egresos en caso de ser costos manuales, parametrizados
            decimal unitPrice = inventoryMoveDetail.unitPrice;
            var costoTotal = GlobalCalculator.RedondearMontoTotalPrecision(cantidadMovimiento * unitPrice);

            var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                        datoProcesar.IdWarehouse,
                                                        datoProcesar.IdItem,
                                                        cantidadMovimiento,
                                                        costoTotal,
                                                        datoProcesar.PlantaProcesoId);

            // El saldo quedaría en negativo, por lo que no se debe procesar
            if (!asignarCosto) costoTotal = unitPrice = 0m;
            costoTotal = Math.Abs(costoTotal);
            if (costoTotal > 0)
            {
                ordenAsigCost++;
                this.RemoverErrores(datoProcesar.IdInventoryMoveDetail, novedades);
            }

            if (indiceReproceso.HasValue)
            {
                productionsCostProductValuation[indiceReproceso.Value].ordenAsigCost = ordenAsigCost;
                productionsCostProductValuation[indiceReproceso.Value].costoTotal = costoTotal;
                productionsCostProductValuation[indiceReproceso.Value].costoUnitario = unitPrice;
            }
            else
            {
                productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                {
                    orden = productionsCostProductValuation.Count + 1,
                    ordenAsigCost = ordenAsigCost,
                    idInventoryMoveDetail = inventoryMoveDetail.id,
                    idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                    idBodega = inventoryMoveDetail.id_warehouse,
                    idItem = datoProcesar.IdItem,
                    idMotivo = datoProcesar.IdInventoryReason,
                    esTransferencia = parametroCalculo.EsTransferencia,
                    naturaleza = parametroCalculo.Naturaleza,
                    tipoCalculo = parametroCalculo.TipoCalculo,
                    valorizacion = parametroCalculo.Valorizacion,
                    accion = m_MantenerCostoAsignadoKey,
                    id_lot = inventoryMoveDetail.id_lot ?? 0,
                    costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                    costoTotalAnterior = inventoryMoveDetail.balanceCost,

                    cantidad = cantidadMovimiento,
                    costoTotal = costoTotal,
                    costoUnitario = unitPrice,
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                });
            }
        }
        private void AsignarCostoPromedio(DataProcesarInventario datoProcesar,
                                            ProcesamientoValorizacion parametroCalculo,
                                            ref List<CostoProcesar> saldos,
                                            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
                                            List<ProductionCostProductValuationWarning> novedades,
                                            ref int ordenAsigCost,
                                            int? indiceReproceso)
        {
            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
            //var saldoActual = (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
            //                                ? saldos.FirstOrDefault(e => e.IdBodega == datoProcesar.IdWarehouse
            //                                                                && e.IdItem == datoProcesar.IdItem)
            //                                : saldos.FirstOrDefault(e => e.IdItem == datoProcesar.IdItem
            //                                                             && e.LastProcessPlantId == datoProcesar.PlantaProcesoId
            //                                );

            decimal costoUnitario = getCostoUnitarioSaldo( saldos,
                                                            datoProcesar.IdItem,
                                                            datoProcesar.IdWarehouse,
                                                            datoProcesar.PlantaProcesoId);
            //if (saldoActual.CostoUnitario <= 0m)
            if (costoUnitario <= 0m)
            {
                this.SetErrorCostoCero(datoProcesar, novedades);
            }

            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            if (inventoryMoveDetail == null)
            {
                throw new Exception("Error al recuperar el detalle del movimiento de inventario.");
            }

            var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
            decimal precioUnitario = 0m;
            if (parametroCalculo.Naturaleza == m_NaturalezaEgresoKey)
            {
                //precioUnitario = saldoActual.CostoUnitario;
                precioUnitario = costoUnitario;
            }
            else if (parametroCalculo.Naturaleza == m_NaturalezaIngresoKey)
            {
                //precioUnitario = (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote) ? inventoryMoveDetail.unitPrice : saldoActual.CostoUnitario);
                precioUnitario = (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote) ? inventoryMoveDetail.unitPrice : costoUnitario);
            }

            var costoTotal = GlobalCalculator.RedondearMontoTotalPrecision(cantidadMovimiento * precioUnitario);
            // Ajustamos el saldo del producto

            var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                        datoProcesar.IdWarehouse,
                                                        datoProcesar.IdItem,
                                                        cantidadMovimiento,
                                                        costoTotal,
                                                        datoProcesar.PlantaProcesoId);

            // El saldo quedaría en negativo, por lo que no se debe procesar
            if (!asignarCosto) costoTotal = precioUnitario = 0m;
            costoTotal = Math.Abs(costoTotal);
            if (costoTotal > 0)
            {
                ordenAsigCost++;
                this.RemoverErrores(datoProcesar.IdInventoryMoveDetail, novedades);
            }

            if (indiceReproceso.HasValue)
            {
                productionsCostProductValuation[indiceReproceso.Value].ordenAsigCost = ordenAsigCost;
                productionsCostProductValuation[indiceReproceso.Value].costoTotal = costoTotal;
                productionsCostProductValuation[indiceReproceso.Value].costoUnitario = precioUnitario;
            }
            else
            {
                productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                {
                    orden = productionsCostProductValuation.Count + 1,
                    ordenAsigCost = ordenAsigCost,
                    idInventoryMoveDetail = inventoryMoveDetail.id,
                    idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                    idBodega = inventoryMoveDetail.id_warehouse,
                    idItem = datoProcesar.IdItem,
                    idMotivo = datoProcesar.IdInventoryReason,
                    esTransferencia = parametroCalculo.EsTransferencia,
                    naturaleza = parametroCalculo.Naturaleza,
                    tipoCalculo = parametroCalculo.TipoCalculo,
                    accion = m_CalcularCostoPromedioKey,
                    id_lot = inventoryMoveDetail.id_lot ?? 0,
                    valorizacion = parametroCalculo.Valorizacion,
                    costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                    costoTotalAnterior = GlobalCalculator
                        .RedondearMontoTotalPrecision(cantidadMovimiento * inventoryMoveDetail.unitPrice),

                    cantidad = cantidadMovimiento,
                    costoTotal = costoTotal,
                    costoUnitario = precioUnitario,
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                });
            }
        }
        private void AsignarCostoCoeficiente(
            ProductionCostProductValuation productValuation, DataProcesarInventario datoProcesar,
            ProcesamientoValorizacion parametroCalculo, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades, ref int ordenAsigCost,
            int? indiceReproceso,
            List<DataProcesarInventario> dataOrdenamiento)
        {
            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
            var idsCoefficientExecution = productValuation
                .ProductionCostProductValuationExecutions
                .Where(e => e.isActive)
                .Select(e => e.id_coefficientExecution);

            var item = this.GetItemFromTempData(datoProcesar.IdItem);
            var bodega = this.GetWarehouseFromTempData(datoProcesar.IdWarehouse);
            var inventoryReason = GetInventoryReasonFromTempData(datoProcesar.IdInventoryReason);

            var detalleEjecucionCoeficiente = this.GetProductionCostCoefficientExecutionWarehouseFromTempData(idsCoefficientExecution)
                .FirstOrDefault(e => e.id_warehouse == datoProcesar.IdWarehouse
                    && e.id_inventoryLine == item.id_inventoryLine && e.id_itemType == item.id_itemType && e.isActive);

            if (detalleEjecucionCoeficiente == null)
            {
                novedades.Add(new ProductionCostProductValuationWarning()
                {
                    idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                    orden = novedades.Count + 1,
                    descripcion = $"Coeficiente no encontrado para el tipo de Ítem: [{item.masterCode}] - {item.name}. Bodega: {bodega.name}." +
                        $"Línea de Inventario: {item.InventoryLine.name}",
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                });

                detalleEjecucionCoeficiente = new ProductionCostCoefficientExecutionWarehouse();
            }

            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
            decimal precioUnitario;
            bool? isResultingProduct = null;
            if (inventoryMoveDetail.unitPrice == 0)
            {
                // recuperamos el costo de los detalles procesados anteriormente
                int? idUltimoDetalleMovimientoLote = null;
                decimal? factor = null;
                decimal? precioUnitarioMateriaPrima = null;
                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                {
                    idUltimoDetalleMovimientoLote = this.RecuperarCostoMovimientoProcesoMetodoProceso(inventoryMoveDetail,
                                                                                                        datoProcesar,
                                                                                                        ref saldos,
                                                                                                        ref productionsCostProductValuation,
                                                                                                        novedades,
                                                                                                        parametroCalculo,
                                                                                                        dataOrdenamiento,
                                                                                                        out factor,
                                                                                                        out precioUnitarioMateriaPrima,
                                                                                                        out isResultingProduct);
                }
                else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
                {
                    idUltimoDetalleMovimientoLote = RecuperarCostoMovimientoProcesoMetodoLote(inventoryMoveDetail,
                                                                                                datoProcesar,
                                                                                                ref saldos,
                                                                                                ref productionsCostProductValuation,
                                                                                                novedades,
                                                                                                out factor,
                                                                                                out precioUnitarioMateriaPrima);
                }

                // Calcular el precio unitario
                if (idUltimoDetalleMovimientoLote.HasValue && factor.HasValue)
                {
                    // Recuperamos el costo unitario de la materia prima
                    var costoUnitarioMP = productionsCostProductValuation
                        .FirstOrDefault(e => e.idInventoryMoveDetail == idUltimoDetalleMovimientoLote)?
                        .costoUnitario;

                    // Recuperamos la cantidad de materia prima usada en el masterizado
                    var cantidadMP = this.GetCantidadMPDetailMasterizado(
                        inventoryMoveDetail.id_inventoryMove, datoProcesar.IdItem, inventoryMoveDetail.id_lot) ?? 0m;

                    // calculamos el costo total de la materia prima en base a la cantidad usada y al precio.
                    var costoTotalMovAnt = costoUnitarioMP.HasValue
                        ? (decimal?)GlobalCalculator.RedondearMontoTotalPrecision(cantidadMP * costoUnitarioMP.Value)
                        : null;

                    // Aplicamos el factor de prorrateo en base al factor de materia prima usada.
                    var costoTotalFactor = costoTotalMovAnt.HasValue
                        ? GlobalCalculator.RedondearMontoTotalPrecision(costoTotalMovAnt.Value * (1m - factor.Value))
                        : 0m;

                    precioUnitario = cantidadMovimiento != 0m
                        ? Math.Abs(GlobalCalculator.RedondearMontoTotalPrecision(costoTotalFactor / cantidadMovimiento))
                        : 0m;
                }
                else if (idUltimoDetalleMovimientoLote.HasValue)
                {
                    precioUnitario = productionsCostProductValuation
                        .FirstOrDefault(e => e.idInventoryMoveDetail == idUltimoDetalleMovimientoLote)?
                        .costoUnitario ?? 0m;
                }
                else if (precioUnitarioMateriaPrima.HasValue)
                {
                    precioUnitario = precioUnitarioMateriaPrima.Value;
                }
                else
                {
                    precioUnitario = 0m;

                    var item1 = this.GetItemFromTempData(datoProcesar.IdItem);
                    var bodega1 = this.GetWarehouseFromTempData(datoProcesar.IdWarehouse);
                    novedades.Add(new ProductionCostProductValuationWarning()
                    {
                        idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                        orden = novedades.Count + 1,
                        descripcion = $"No se ha logrado recuperar el costo del proceso para el producto: [{item1.masterCode}] - {item1.name}." +
                            $"Bodega: {bodega1.name}. Nº Movimiento: {inventoryMoveDetail.InventoryMove.Document.sequential}. ",
                        activo = true,

                        idUsuarioCreacion = this.ActiveUserId,
                        fechaHoraCreacion = DateTime.Now,
                        idUsuarioModificacion = this.ActiveUserId,
                        fechaHoraModificacion = DateTime.Now,
                    });
                }
            }
            else
            {
                precioUnitario = inventoryMoveDetail.unitPrice;
            }

            var presentacion = item.Presentation;

            decimal costoCoeficiente = 0;
            decimal costoTotal = 0;
            decimal newUnitPrice = 0;

            string parametroMetodoValorizaion = getParametroMetodoValorizacion();
            if (parametroMetodoValorizaion.Equals(m_MetodoValorizacionProceso))
            {
                if (parametroCalculo.TipoCalculo == m_TipoCalculoAbsorcionPTKey)
                {
                    precioUnitario = precioUnitario * presentacion.maximum;
                    costoTotal = GlobalCalculator.RedondearMontoTotalPrecision(cantidadMovimiento * precioUnitario);
                    newUnitPrice = cantidadMovimiento != 0m
                        ? precioUnitario
                        : 0m;
                }
                else
                {
                    costoCoeficiente = detalleEjecucionCoeficiente.coeficiente * presentacion.minimum * presentacion.maximum;
                    costoTotal = GlobalCalculator.RedondearMontoTotalPrecision((cantidadMovimiento * precioUnitario) + costoCoeficiente);
                    newUnitPrice = cantidadMovimiento != 0m
                        ? Math.Abs(GlobalCalculator.RedondearMontoTotalPrecision(costoTotal / cantidadMovimiento))
                        : 0m;
                }
            }
            else if (parametroMetodoValorizaion.Equals(m_MetodoValorizacionLote))
            {
                costoCoeficiente = detalleEjecucionCoeficiente.coeficiente * presentacion.minimum * presentacion.maximum;
                costoTotal = GlobalCalculator.RedondearMontoTotalPrecision((cantidadMovimiento * precioUnitario) + costoCoeficiente);
                newUnitPrice = cantidadMovimiento != 0m
                    ? Math.Abs(GlobalCalculator.RedondearMontoTotalPrecision(costoTotal / cantidadMovimiento))
                    : 0m;
            }



            if (newUnitPrice <= 0)
            {
                this.SetErrorCostoCero(datoProcesar, novedades);
            }

            var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                        datoProcesar.IdWarehouse,
                                                        datoProcesar.IdItem,
                                                        cantidadMovimiento,
                                                        costoTotal,
                                                        datoProcesar.PlantaProcesoId);

            // El saldo quedaría en negativo, por lo que no se debe procesar
            if (!asignarCosto) costoTotal = newUnitPrice = 0m;
            costoTotal = Math.Abs(costoTotal);
            if (costoTotal > 0)
            {
                ordenAsigCost++;
                this.RemoverErrores(datoProcesar.IdInventoryMoveDetail, novedades);
            }

            if (indiceReproceso.HasValue)
            {
                productionsCostProductValuation[indiceReproceso.Value].ordenAsigCost = ordenAsigCost;
                productionsCostProductValuation[indiceReproceso.Value].costoTotal = costoTotal;
                productionsCostProductValuation[indiceReproceso.Value].costoUnitario = newUnitPrice;
            }
            else
            {
                productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                {
                    ordenAsigCost = ordenAsigCost,
                    orden = productionsCostProductValuation.Count + 1,
                    idInventoryMoveDetail = inventoryMoveDetail.id,
                    idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                    idBodega = inventoryMoveDetail.id_warehouse,
                    idItem = datoProcesar.IdItem,
                    idMotivo = datoProcesar.IdInventoryReason,
                    esTransferencia = parametroCalculo.EsTransferencia,
                    naturaleza = parametroCalculo.Naturaleza,
                    tipoCalculo = parametroCalculo.TipoCalculo,
                    accion = m_AsignarCostoCoeficienteKey,
                    id_lot = inventoryMoveDetail.id_lot ?? 0,
                    valorizacion = parametroCalculo.Valorizacion,
                    costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                    costoTotalAnterior = cantidadMovimiento * inventoryMoveDetail.unitPrice,
                    coeficiente = detalleEjecucionCoeficiente.coeficiente,

                    cantidad = cantidadMovimiento,
                    costoTotal = costoTotal,
                    costoUnitario = newUnitPrice,
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                    IsResultingProduct = (isResultingProduct ?? false)
                });
            }
        }
        private void AsignarCostoProceso(DataProcesarInventario datoProcesar,
            ProcesamientoValorizacion parametroCalculo, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades,
            List<DataProcesarInventario> dataOrdenamiento,
            ref int ordenAsigCost,
            int? indiceReproceso)
        {
            string parametroMetodoValorizacion = getParametroMetodoValorizacion();

            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            if (inventoryMoveDetail == null)
            {
                throw new Exception("Error al recuperar el detalle del movimiento de inventario.");
            }

            // verificamos el proceso interno
            var codeProductionProcessExclude = new[] { "REC", "RMM" };

            var productionLot = db.ProductionLot
                .FirstOrDefault(e => e.id == inventoryMoveDetail.id_lot &&
                    !codeProductionProcessExclude.Contains(e.ProductionProcess.code) && e.ProductionLotState.code != "09");

            var productionProcess = productionLot?
                .ProductionProcess ?? new ProductionProcess();

            var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
            decimal costoUnitario = 0, costoTotal = 0;
            decimal? coeficiente = null;
            bool? isResultingProduct = null;
            if (productionProcess.generatesAbsorption ?? false)
            {
                this.RecuperarCostoLoteMateriaPrima(datoProcesar, ref saldos,
                    productionsCostProductValuation, novedades, inventoryMoveDetail.id_lot,
                    cantidadMovimiento, out costoUnitario, out costoTotal, out isResultingProduct);
            }
            else
            {
                // recuperamos el costo de los detalles procesados anteriormente
                int? idUltimoDetalleMovimientoLote = null;
                decimal? factor = null;
                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                {
                    idUltimoDetalleMovimientoLote = this.RecuperarCostoMovimientoProcesoMetodoProceso(inventoryMoveDetail,
                                                                                                        datoProcesar,
                                                                                                        ref saldos,
                                                                                                        ref productionsCostProductValuation,
                                                                                                        novedades,
                                                                                                        parametroCalculo,
                                                                                                        dataOrdenamiento,
                                                                                                        out factor,
                                                                                                        out _,
                                                                                                        out isResultingProduct);
                }
                else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
                {
                    idUltimoDetalleMovimientoLote = RecuperarCostoMovimientoProcesoMetodoLote(inventoryMoveDetail,
                                                                                                datoProcesar,
                                                                                                ref saldos,
                                                                                                ref productionsCostProductValuation,
                                                                                                novedades,
                                                                                                out factor,
                                                                                                out _);
                }

                if (!idUltimoDetalleMovimientoLote.HasValue)
                {
                    var item = this.GetItemFromTempData(datoProcesar.IdItem);
                    var bodega = this.GetWarehouseFromTempData(datoProcesar.IdWarehouse);

                    novedades.Add(new ProductionCostProductValuationWarning()
                    {
                        idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                        orden = novedades.Count + 1,
                        descripcion = $"No se ha logrado recuperar el costo del proceso para el producto: [{item.masterCode}] - {item.name}." +
                            $"Bodega: {bodega.name}. Nº Movimiento: {inventoryMoveDetail.InventoryMove.Document.sequential}. ",
                        activo = true,

                        idUsuarioCreacion = this.ActiveUserId,
                        fechaHoraCreacion = DateTime.Now,
                        idUsuarioModificacion = this.ActiveUserId,
                        fechaHoraModificacion = DateTime.Now,
                    });

                    costoUnitario = 0m;
                    coeficiente = null;
                }
                else
                {
                    var ultimoProcesamientoLote = productionsCostProductValuation
                       .FirstOrDefault(e => e.idInventoryMoveDetail == idUltimoDetalleMovimientoLote);

                    if (factor.HasValue)
                    {
                        var costoTotalMovAnt = ultimoProcesamientoLote.costoTotal;
                        var costoTotalFactor = GlobalCalculator.RedondearMontoTotalPrecision(costoTotalMovAnt * (1m - factor.Value));

                        costoUnitario = cantidadMovimiento != 0m
                            ? Math.Abs(GlobalCalculator.RedondearMontoTotalPrecision(costoTotalFactor / cantidadMovimiento))
                            : 0m;
                    }
                    else
                    {
                        costoUnitario = ultimoProcesamientoLote.costoUnitario;
                    }

                    coeficiente = ultimoProcesamientoLote.coeficiente;
                }

                if (costoUnitario <= 0)
                {
                    this.SetErrorCostoCero(datoProcesar, novedades);
                }

                costoTotal = GlobalCalculator.RedondearMontoTotalPrecision((cantidadMovimiento * costoUnitario));
            }

            var costoProcesar = (cantidadMovimiento < 0 && costoTotal > 0)
                ? -costoTotal : costoTotal;

            var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                        datoProcesar.IdWarehouse,
                                                        datoProcesar.IdItem,
                                                        cantidadMovimiento,
                                                        costoProcesar,
                                                        datoProcesar.PlantaProcesoId);

            // El saldo quedaría en negativo, por lo que no se debe procesar
            if (!asignarCosto) costoTotal = costoUnitario = 0m;
            costoTotal = Math.Abs(costoTotal);
            if (costoTotal > 0)
            {
                ordenAsigCost++;
                this.RemoverErrores(datoProcesar.IdInventoryMoveDetail, novedades);
            }

            if (indiceReproceso.HasValue)
            {
                productionsCostProductValuation[indiceReproceso.Value].ordenAsigCost = ordenAsigCost;
                productionsCostProductValuation[indiceReproceso.Value].costoTotal = costoTotal;
                productionsCostProductValuation[indiceReproceso.Value].costoUnitario = costoUnitario;
            }
            else
            {
                productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                {
                    ordenAsigCost = ordenAsigCost,
                    orden = productionsCostProductValuation.Count + 1,
                    idInventoryMoveDetail = inventoryMoveDetail.id,
                    idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                    idBodega = inventoryMoveDetail.id_warehouse,
                    idItem = datoProcesar.IdItem,
                    idMotivo = datoProcesar.IdInventoryReason,
                    esTransferencia = parametroCalculo.EsTransferencia,
                    naturaleza = parametroCalculo.Naturaleza,
                    tipoCalculo = parametroCalculo.TipoCalculo,
                    accion = m_AsignarCostoProcesoTransKey,
                    id_lot = inventoryMoveDetail.id_lot ?? 0,
                    valorizacion = parametroCalculo.Valorizacion,
                    costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                    costoTotalAnterior = cantidadMovimiento * inventoryMoveDetail.unitPrice,

                    coeficiente = coeficiente,
                    cantidad = cantidadMovimiento,
                    costoTotal = costoTotal,
                    costoUnitario = costoUnitario,
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                    IsResultingProduct = (isResultingProduct ?? false)
                });
            }
        }

        private void AsignarCostoEgresoTransferencia(DataProcesarInventario datoProcesar,
            ProcesamientoValorizacion parametroCalculo, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades, ref int ordenAsigCost, int? indiceReproceso)
        {
            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            if (inventoryMoveDetail == null)
            {
                throw new Exception("Error al recuperar el detalle del movimiento de inventario.");
            }

            var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;

            // Recuperamos el detalle por transferencia
            var inventoryMoveDetailExit = db.InventoryMoveDetailTransfer
                .FirstOrDefault(e => e.id_inventoryMoveDetailEntry == datoProcesar.IdInventoryMoveDetail)?
                .InventoryMoveDetail1;

            decimal precioUnitario;
            if (inventoryMoveDetailExit == null)
            {
                var item = this.GetItemFromTempData(datoProcesar.IdItem);
                var bodega = this.GetWarehouseFromTempData(datoProcesar.IdWarehouse);
                var fechaMovimiento = inventoryMoveDetail.InventoryMove.Document.emissionDate.ToString("dd/MM/yyyy");
                novedades.Add(new ProductionCostProductValuationWarning()
                {
                    idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                    orden = novedades.Count + 1,
                    descripcion = "Error al recuperar el detalle del movimiento de inventario TRANSFERENCIA DE EGRESO. " +
                        $"Bodega {bodega.name}. Ítem: [{item.masterCode}] - {item.name}. Fecha movimiento: {fechaMovimiento}." +
                        $"IdDetalle: {datoProcesar.IdInventoryMoveDetail}.",
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                });

                precioUnitario = 0m;
            }
            else
            {
                precioUnitario = productionsCostProductValuation
                    .FirstOrDefault(e => e.idInventoryMoveDetail == inventoryMoveDetailExit.id)?
                    .costoUnitario ?? inventoryMoveDetailExit.unitPrice;
            }

            if (precioUnitario <= 0)
            {
                this.SetErrorCostoCero(datoProcesar, novedades);
            }

            var costoTotal = GlobalCalculator.RedondearMontoTotalPrecision(cantidadMovimiento * precioUnitario);
            // Ajustamos el saldo del producto
            var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                        datoProcesar.IdWarehouse,
                                                        datoProcesar.IdItem,
                                                        cantidadMovimiento,
                                                        costoTotal,
                                                        datoProcesar.PlantaProcesoId
                                                        );

            // El saldo quedaría en negativo, por lo que no se debe procesar
            if (!asignarCosto) costoTotal = precioUnitario = 0m;
            costoTotal = Math.Abs(costoTotal);
            if (costoTotal > 0)
            {
                ordenAsigCost++;
                this.RemoverErrores(datoProcesar.IdInventoryMoveDetail, novedades);
            }

            if (indiceReproceso.HasValue)
            {
                productionsCostProductValuation[indiceReproceso.Value].ordenAsigCost = ordenAsigCost;
                productionsCostProductValuation[indiceReproceso.Value].costoTotal = costoTotal;
                productionsCostProductValuation[indiceReproceso.Value].costoUnitario = precioUnitario;
            }
            else
            {
                productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                {
                    orden = productionsCostProductValuation.Count + 1,
                    ordenAsigCost = ordenAsigCost,
                    idInventoryMoveDetail = inventoryMoveDetail.id,
                    idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                    idBodega = inventoryMoveDetail.id_warehouse,
                    idItem = datoProcesar.IdItem,
                    idMotivo = datoProcesar.IdInventoryReason,
                    esTransferencia = parametroCalculo.EsTransferencia,
                    naturaleza = parametroCalculo.Naturaleza,
                    tipoCalculo = parametroCalculo.TipoCalculo,
                    accion = m_AsignarCostoEgresoTransKey,
                    id_lot = inventoryMoveDetail.id_lot ?? 0,
                    valorizacion = parametroCalculo.Valorizacion,
                    costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                    costoTotalAnterior = cantidadMovimiento * inventoryMoveDetail.unitPrice,

                    cantidad = cantidadMovimiento,
                    costoTotal = costoTotal,
                    costoUnitario = precioUnitario,
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                });
            }
        }

        private void AsignarCostoHeredado(DataProcesarInventario datoProcesar,
            ProcesamientoValorizacion parametroCalculo, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades, bool generarCostoPromedio,
            ref int ordenAsigCost, int? indiceReproceso)
        {
            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            var inventoryReason = this.GetInventoryReasonFromTempData(datoProcesar.IdInventoryReason);
            if (inventoryReason.id_inventoryReasonRelated.HasValue)
            {
                var parametroCalculoHeredado = this.GetParametroCalculo(inventoryReason.id_inventoryReasonRelated.Value);
                datoProcesar.IdInventoryReason = inventoryReason.id_inventoryReasonRelated.Value;

                int? id_detalleSalida = null;
                var inventoryMoveDetailExit = db.InventoryMoveDetailTransfer
                                                        .FirstOrDefault(e => e.id_inventoryMoveDetailEntry == datoProcesar.IdInventoryMoveDetail)?
                                                        .InventoryMoveDetail;

                // Verificamos la transferencia normal
                if (inventoryMoveDetailExit != null)
                {
                    id_detalleSalida = inventoryMoveDetailExit.id;
                }
                else
                {
                    // Verificamos si es una transferencia automática
                    id_detalleSalida = this.GetDetalleMovimientoSalidaTransferenciaAutomatica(datoProcesar.IdInventoryMoveDetail);

                    if (!id_detalleSalida.HasValue)
                    {
                        // validar si la relacion esta en DocumentSource
                        var documentSourcesOriginIds = db.DocumentSource
                                                                .Where(r => r.id_document == inventoryMoveDetail.id_inventoryMove)
                                                                .Select(r => r.id_documentOrigin)
                                                                .ToArray();
                        // obtener informacion motivo Egreso 

                        var inventoryReasonEgreso = this.GetInventoryReasonFromTempData((inventoryReason.id_inventoryReasonRelated ?? 0));

                        if (inventoryReasonEgreso != null)
                        {
                            int documentTypeEgresoId = inventoryReasonEgreso.id_documentType;
                            int? documentoEgresoId = db.InventoryMove
                                                            .FirstOrDefault(r => documentSourcesOriginIds.Contains(r.id)
                                                                                && r.id_inventoryReason == inventoryReasonEgreso.id)?.id;
                            if (!documentoEgresoId.HasValue)
                            {
                                documentoEgresoId = db.InventoryMove
                                                            .FirstOrDefault(r => documentSourcesOriginIds.Contains(r.id))?.id;
                            }
                            //db.Document
                            //                            .FirstOrDefault(r => documentSourcesOriginIds.Contains(r.id) 
                            //                                                 && r.id_documentType == documentTypeEgresoId)?.id;

                            if (documentoEgresoId.HasValue)
                            {
                                id_detalleSalida = db.InventoryMoveDetail
                                                            .FirstOrDefault(r => r.id_inventoryMove == documentoEgresoId.Value
                                                                                 && r.id_warehouseEntry == inventoryMoveDetail.id_warehouse
                                                                                 && r.id_warehouseLocationEntry == inventoryMoveDetail.id_warehouseLocation
                                                                                 && r.id_lot == inventoryMoveDetail.id_lot
                                                                                 && r.id_item == inventoryMoveDetail.id_item
                                                                                 && r.id_productionCart == inventoryMoveDetail.id_productionCart
                                                                                 && r.exitAmount == inventoryMoveDetail.entryAmount
                                                                                 )?.id;
                                /*
                                 movimientosDetallesIngresos = db.InventoryMoveDetail
                                                        .Where(r =>    r.id_inventoryMove == documentSourceIngreso.id_document
                                                                       && r.id_warehouse == movimientoDetalleEgreso.id_warehouseEntry
                                                                       && r.id_warehouseLocation == movimientoDetalleEgreso.id_warehouseLocationEntry
                                                                       && r.id_lot == movimientoDetalleEgreso.id_lot
                                                                       && r.id_item == movimientoDetalleEgreso.id_item
                                                                       && r.id_productionCart == movimientoDetalleEgreso.id_productionCart
                                                                       )
                                                        .ToArray();
                                 
                                 */
                            }



                        }


                        if (!id_detalleSalida.HasValue)
                        {
                            var item = this.GetItemFromTempData(datoProcesar.IdItem);
                            var bodega = this.GetWarehouseFromTempData(datoProcesar.IdWarehouse);
                            var fechaMovimiento = inventoryMoveDetail != null
                                ? inventoryMoveDetail.InventoryMove.Document.emissionDate.ToString("dd/MM/yyyy")
                                : string.Empty;

                            novedades.Add(new ProductionCostProductValuationWarning()
                            {
                                idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                                orden = novedades.Count + 1,
                                descripcion = "Error al recuperar el detalle del movimiento de inventario de EGRESO. " +
                                    $"Bodega {bodega.name}. Ítem: [{item.masterCode}] - {item.name}. Fecha movimiento: {fechaMovimiento}.",
                                activo = true,

                                idUsuarioCreacion = this.ActiveUserId,
                                fechaHoraCreacion = DateTime.Now,
                                idUsuarioModificacion = this.ActiveUserId,
                                fechaHoraModificacion = DateTime.Now,
                            });
                        }

                    }
                }

                var costoTotal = productionsCostProductValuation
                    .FirstOrDefault(e => e.idInventoryMoveDetail == id_detalleSalida)?
                    .costoTotal;

                // Validamos si no encuentra el costo total de salida
                if (!costoTotal.HasValue && costoTotal <= 0)
                {
                    this.SetErrorCostoCero(datoProcesar, novedades);
                }

                var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
                var precioUnitario = cantidadMovimiento != 0m
                    ? Math.Abs(GlobalCalculator.RedondearMontoTotalPrecision((costoTotal ?? 0m) / cantidadMovimiento))
                    : 0m;

                // Ajustamos el saldo del producto
                var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                            datoProcesar.IdWarehouse,
                                                            datoProcesar.IdItem,
                                                            cantidadMovimiento,
                                                            costoTotal ?? 0m,
                                                            datoProcesar.PlantaProcesoId
                                                            );

                // El saldo quedaría en negativo, por lo que no se debe procesar
                if (!asignarCosto) costoTotal = precioUnitario = 0m;
                costoTotal = Math.Abs(costoTotal ?? 0m);
                if (costoTotal > 0)
                {
                    ordenAsigCost++;
                    this.RemoverErrores(datoProcesar.IdInventoryMoveDetail, novedades);
                }

                if (indiceReproceso.HasValue)
                {
                    productionsCostProductValuation[indiceReproceso.Value].ordenAsigCost = ordenAsigCost;
                    productionsCostProductValuation[indiceReproceso.Value].costoTotal = costoTotal ?? 0m;
                    productionsCostProductValuation[indiceReproceso.Value].costoUnitario = precioUnitario;
                }
                else
                {
                    productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                    {
                        ordenAsigCost = ordenAsigCost,
                        orden = productionsCostProductValuation.Count + 1,
                        idInventoryMoveDetail = inventoryMoveDetail.id,
                        idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                        idBodega = inventoryMoveDetail.id_warehouse,
                        idItem = datoProcesar.IdItem,
                        idMotivo = datoProcesar.IdInventoryReason,
                        esTransferencia = parametroCalculo.EsTransferencia,
                        naturaleza = parametroCalculo.Naturaleza,
                        tipoCalculo = parametroCalculo.TipoCalculo,
                        accion = m_AsignarCostoEgresoTransKey,
                        id_lot = inventoryMoveDetail.id_lot ?? 0,
                        valorizacion = parametroCalculo.Valorizacion,
                        costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                        costoTotalAnterior = cantidadMovimiento * inventoryMoveDetail.unitPrice,

                        cantidad = cantidadMovimiento,
                        costoTotal = costoTotal ?? 0m,
                        costoUnitario = precioUnitario,
                        activo = true,

                        idUsuarioCreacion = this.ActiveUserId,
                        fechaHoraCreacion = DateTime.Now,
                        idUsuarioModificacion = this.ActiveUserId,
                        fechaHoraModificacion = DateTime.Now,
                    });
                }
            }
            else
            {
                this.AsignarCostoPromedio(datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation, novedades,
                    ref ordenAsigCost, indiceReproceso);
            }
        }
        private int? RecuperarCostoMovimientoProceso(InventoryMoveDetail inventoryMoveDetail,
            DataProcesarInventario datoProcesar, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades, ProcesamientoValorizacion parametroCalculo,
            out decimal? factor, out decimal? costoUnitarioInventario, out bool? isResultingProduct)
        {
            // recuperamos el costo de los detalles procesados anteriormente
            var id_inventoryMove = inventoryMoveDetail.id_inventoryMove;
            var idItem = inventoryMoveDetail.id_item;
            var id_lot = inventoryMoveDetail.id_lot;

            //var inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id_inventoryMove);

            var movimientosLote = productionsCostProductValuation
                                        .Where(e => e.idItem == idItem && e.id_lot == id_lot)
                                        .Where(e => e.costoUnitario != 0m); // donde su costo sea mayor a cero;

            // Si encontramos un movimiento con la misma bodega se filtra a esa bodega.
            var movimientosBodegaLote = movimientosLote
                .Select(e => new
                {
                    e.idInventoryMoveDetail,
                    id_bodega = e.idBodega,
                    e.idItem,
                    e.id_lot,
                    e.idMotivo
                })
                .ToArray();

            var inventoryReason = GetInventoryReasonFromTempData(datoProcesar.IdInventoryReason);
            var bodegaProcesar = GetWarehouseFromTempData(datoProcesar.IdWarehouse);
            // ANALIZAR, CONFIGURAR EN MOTIVO DE INVENTARIO
            if (movimientosBodegaLote.Any(e => e.id_bodega == datoProcesar.IdWarehouse))
            {
                movimientosBodegaLote = movimientosBodegaLote
                    .Where(e => e.id_bodega == datoProcesar.IdWarehouse)
                    .ToArray();
            }

            var idsInventoryMoveDetail = movimientosBodegaLote
                                                .Select(e => e.idInventoryMoveDetail)
                                                .ToArray();

            // Si encontramos detalles con ese producto, recuperamos el último detalle 
            factor = costoUnitarioInventario = null;
            isResultingProduct = false;

            // Absorcion Costo: Forzar +1 pasada Material resuiltando o Motivo ILP
            var codeProductionProcessExclude = new[] { "REC", "RMM" };

            var productionLot = db.ProductionLot
                .FirstOrDefault(e => e.id == inventoryMoveDetail.id_lot &&
                    !codeProductionProcessExclude.Contains(e.ProductionProcess.code) && e.ProductionLotState.code != "09");

            var productionProcess = productionLot?
                .ProductionProcess ?? new ProductionProcess();

            string[] specialTipoCalculoMasterizado = new string[] { m_TipoCalculoAbsorcionPTKey, m_TipoCalculoAbsorcionSBKey };
            //if (datoProcesar.IsResultingProduct || inventoryReason.code == "ILP")
            if ((productionProcess.generatesAbsorption ?? false) && !specialTipoCalculoMasterizado.Contains(parametroCalculo.TipoCalculo))
            {
                var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
                this.RecuperarCostoLoteMateriaPrima(datoProcesar,
                    ref saldos, productionsCostProductValuation, novedades, inventoryMoveDetail.id_lot,
                    cantidadMovimiento, out var precioUnitario, out _, out isResultingProduct);
                costoUnitarioInventario = precioUnitario;

                return null;
            }

            // Masterizado: Forzar obtener masterizado Cajas resultantes y Sobrante  
            //string[] inventoryReasonMasterResults = new string[] { "IPAMM", "IPACS" };


            //if (idsInventoryMoveDetail.Any() && !inventoryReasonMasterResults.Contains(inventoryReason.code) )
            if (idsInventoryMoveDetail.Any() && !specialTipoCalculoMasterizado.Contains(parametroCalculo.TipoCalculo))
            {
                var detallesMovimientoLote = new List<InventoryMoveDetail>();
                foreach (var idInventoryMoveDetail in idsInventoryMoveDetail)
                {
                    var detalleInventarioProcesar = this.GetInventoryMoveDetailFromTempData(idInventoryMoveDetail);

                    detallesMovimientoLote.Add(detalleInventarioProcesar);
                }

                return detallesMovimientoLote
                            .OrderBy(e => e.InventoryMove.Document.emissionDate)
                            .ThenBy(e => e.dateCreate)
                            .LastOrDefault()?
                            .id;
            }
            else
            {
                // Sobrante
                //if (inventoryReason.code == "IPACS" )
                if (parametroCalculo.TipoCalculo == m_TipoCalculoAbsorcionSBKey)
                {
                    var idMasterizadoSobrante = this.GetInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot, "SOB");
                    if (idMasterizadoSobrante.HasValue)
                    {
                        return idMasterizadoSobrante;
                    }
                    else
                    {
                        throw new Exception("Sin mov materia prima para movimiento sobrante");
                    }
                }
                // masterizado
                if (parametroCalculo.TipoCalculo == m_TipoCalculoAbsorcionPTKey)
                //else if (inventoryReason.code == "IPAMM")
                {
                    var idMasterizado = this.GetInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot);
                    if (idMasterizado.HasValue)
                    {
                        return idMasterizado;
                    }
                    else
                    {
                        throw new Exception("Sin mov materia prima para movimiento masterizado");
                    }

                }
                else
                {

                    var idMasterizado = this.GetInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot);
                    if (idMasterizado.HasValue)
                    {
                        factor = this.GetFactorInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot);
                        return idMasterizado;
                    }
                    else
                    {
                        var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
                        this.RecuperarCostoLoteMateriaPrima(datoProcesar,
                            ref saldos, productionsCostProductValuation, novedades, inventoryMoveDetail.id_lot,
                            cantidadMovimiento, out var precioUnitario, out _, out isResultingProduct);
                        costoUnitarioInventario = precioUnitario;
                    }
                }

            }

            return null;
        }

        private void RecuperarCostoLoteMateriaPrima(
            DataProcesarInventario datoProcesar, ref List<CostoProcesar> saldos,
            List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades, int? id_lot,
            decimal cantidadMovimiento, out decimal costoUnitario, out decimal costoTotal, out bool? isResultingProduct)
        {

            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
            // Obtener la presentacion del item
            // recalcular las cantidades a Libra
            // en adelante usar el valor en libras

            var detalleLiquidacion = db.ProductionLotLiquidation
                .FirstOrDefault(e => e.id_productionLot == id_lot
                    && e.id_item == datoProcesar.IdItem
                    && e.quantity == Math.Abs(cantidadMovimiento));

            // TODO: Pendiente de validación por busqueda de transferencia saliente
            decimal precioUnitario = 0;
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                precioUnitario = saldos
                                    .FirstOrDefault(e => e.IdItem == datoProcesar.IdItem)?
                                    .CostoUnitario ?? 0m;
            }
            else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
            {
                precioUnitario = saldos
                                    .FirstOrDefault(e => e.IdBodega == datoProcesar.IdWarehouse
                                                         && e.IdItem == datoProcesar.IdItem)?
                                    .CostoUnitario ?? 0m;
            }

            // recuperamos la liquidación
            costoUnitario = costoTotal = 0m;
            isResultingProduct = false;
            bool asignarCosto = true;
            if (detalleLiquidacion != null)
            {
                // Recuperamos toda la salida por liquidación de ese lote
                var detallesLiquidacion = db.ProductionLotLiquidation
                                                    .Where(e => e.id_productionLot == id_lot);
                var mastersTotal = detallesLiquidacion.Sum(e => e.quantity);

                var itemTMP = this.GetItemFromTempData(datoProcesar.IdItem);

                // Distribución del valor por peso
                var distributionPercentage = mastersTotal > 0
                    ? GlobalCalculator.RedondearMontoTotalPrecision(detalleLiquidacion.quantity / mastersTotal * 100)
                    : 0m;

                //var distributionPercentage1 = detalleLiquidacion.distributionPercentage ?? 0m;

                var idsMovimientosInventario = this.GetDataSalidaMPProceso(id_lot ?? 0)
                    .Select(e => e.IdInventoryMoveDetail)
                    .ToArray();

                decimal costoTotalAbsorcion = 0m;
                int movimientosInventarioConCosto = 0;

                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                {
                    foreach (var idMovimientoInventario in idsMovimientosInventario)
                    {
                        var detalle = productionsCostProductValuation.FirstOrDefault(e => e.idInventoryMoveDetail == idMovimientoInventario);
                        if (detalle != null)
                        {
                            if (detalle.costoTotal == 0)
                            {
                                costoTotalAbsorcion = 0;
                                asignarCosto = false;
                                isResultingProduct = true;
                                break;
                            }
                            else
                            {
                                movimientosInventarioConCosto++;
                                costoTotalAbsorcion += detalle.costoTotal;
                            }

                        }
                        else
                        {
                            costoTotalAbsorcion = 0;
                            asignarCosto = false;
                            isResultingProduct = true;
                            break;
                            //var movimientoValorizado = this.GetInventoryMoveDetailFromTempData(idMovimientoInventario) ?? new InventoryMoveDetail(); ;
                            //costoTotalAbsorcion += Math.Abs(movimientoValorizado.entryAmountCost - movimientoValorizado.exitAmountCost);
                        }
                    }
                }
                else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
                {
                    foreach (var idMovimientoInventario in idsMovimientosInventario)
                    {
                        var detalle = productionsCostProductValuation.FirstOrDefault(e => e.idInventoryMoveDetail == idMovimientoInventario);
                        if (detalle != null)
                        {
                            costoTotalAbsorcion += detalle.costoTotal;
                        }
                        else
                        {
                            var movimientoValorizado = this.GetInventoryMoveDetailFromTempData(idMovimientoInventario) ?? new InventoryMoveDetail(); ;
                            costoTotalAbsorcion += Math.Abs(movimientoValorizado.entryAmountCost - movimientoValorizado.exitAmountCost);
                        }
                    }

                }


                // Si el costo unitario por absorción es cero, aplica el costo promedio del saldo actual
                //if (asignarCosto && (idsMovimientosInventario.Length == movimientosInventarioConCosto))
                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                {
                    if (asignarCosto)
                    {
                        if (costoTotalAbsorcion == 0)
                        {
                            costoUnitario = precioUnitario;
                            costoTotal = precioUnitario * cantidadMovimiento;
                        }
                        else
                        {
                            var costoAsignado = distributionPercentage > 0
                                ? GlobalCalculator.RedondearMontoTotalPrecision(costoTotalAbsorcion * distributionPercentage / 100m)
                                : 0m;

                            costoUnitario = cantidadMovimiento != 0m
                                ? Math.Abs(GlobalCalculator.RedondearMontoTotalPrecision(costoAsignado / cantidadMovimiento))
                                : 0m;
                            costoTotal = costoAsignado;
                        }
                    }
                }
                else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
                {
                    if (costoTotalAbsorcion == 0)
                    {
                        costoUnitario = precioUnitario;
                        costoTotal = precioUnitario * cantidadMovimiento;
                    }
                    else
                    {
                        var costoAsignado = distributionPercentage > 0
                            ? GlobalCalculator.RedondearMontoTotalPrecision(costoTotalAbsorcion * distributionPercentage / 100m)
                            : 0m;

                        costoUnitario = cantidadMovimiento != 0m
                            ? Math.Abs(GlobalCalculator.RedondearMontoTotalPrecision(costoAsignado / cantidadMovimiento))
                            : 0m;
                        costoTotal = costoAsignado;
                    }
                }



            }
            // Validamos por saldo inicial
            else if (precioUnitario > 0m)
            {
                costoTotal = GlobalCalculator.RedondearMontoTotalPrecision(cantidadMovimiento * precioUnitario);
                costoUnitario = precioUnitario;
            }
            else
            {
                var item = this.GetItemFromTempData(datoProcesar.IdItem);
                var productionLot = this.db.ProductionLot.FirstOrDefault(e => e.id == id_lot);
                novedades.Add(new ProductionCostProductValuationWarning()
                {
                    idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                    orden = novedades.Count + 1,
                    descripcion = $"No se ha encontrado el detalle de liquidación del lote." +
                           $"Producto: [{item.masterCode}] - {item.name}. Lote: {productionLot?.internalNumber}.",
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                });
            }

        }
        private decimal AjustarCostoTotal(decimal cantidadMovimiento, decimal costoTotal)
        {
            return (cantidadMovimiento < 0 && costoTotal > 0)
                ? -costoTotal : costoTotal;
        }

        private ProcesamientoValorizacion GetParametroCalculo(int idInventoryReason)
        {
            var inventoryReason = this.GetInventoryReasonFromTempData(idInventoryReason);

            return new ProcesamientoValorizacion()
            {
                IdMotivoInventario = inventoryReason.id,
                Naturaleza = inventoryReason.AdvanceParametersDetail.valueCode.Trim(),
                EsTransferencia = inventoryReason.isForTransfer ?? false,
                TipoCalculo = inventoryReason.typeOfCalculation,
                Valorizacion = inventoryReason.valorization,

            };
        }
        private void RemoverErrores(int idInventoryMoveDetail, List<ProductionCostProductValuationWarning> novedades)
        {
            var elementosLimpiar = novedades
                .Where(e => e.idInventoryMoveDetail == idInventoryMoveDetail)
                .ToList();

            foreach (var elemento in elementosLimpiar)
            {
                novedades.Remove(elemento);
            }
        }
        private void SetErrorCostoCero(DataProcesarInventario datoProcesar, List<ProductionCostProductValuationWarning> novedades)
        {
            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            var numeroDocumento = inventoryMoveDetail.InventoryMove.Document.sequential;

            var bodega = this.GetWarehouseFromTempData(datoProcesar.IdWarehouse);
            var item = this.GetItemFromTempData(datoProcesar.IdItem);
            var emissionDate = datoProcesar.EmissionDate.ToString("dd/MM/yyyy");

            var orden = novedades.Count + 1;
            novedades.Add(new ProductionCostProductValuationWarning()
            {
                idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                orden = orden,
                descripcion = $"El costo generado debe ser mayor a cero. Bodega: {bodega.name}. " +
                    $"Producto: [{item.masterCode}] - {item.name}. Movimiento: {numeroDocumento}. Fecha: {emissionDate}.",
                activo = true,

                idUsuarioCreacion = this.ActiveUserId,
                fechaHoraCreacion = DateTime.Now,
                idUsuarioModificacion = this.ActiveUserId,
                fechaHoraModificacion = DateTime.Now,
            });
        }
        private bool CalcularSaldoFinal(ref List<CostoProcesar> saldos,
                                        int idBodega, int idItem, decimal cantidad, decimal costoTotal,
                                        int? id_personProcessPlant)
        {
            // Ajustamos la naturaleza del costo
            int index = -1;
            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                idBodega = 0;
                index = saldos.FindIndex(e => e.IdBodega == idBodega && e.IdItem == idItem && e.LastProcessPlantId== id_personProcessPlant);
            }

            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
            {
                index = saldos.FindIndex(e => e.IdBodega == idBodega && e.IdItem == idItem);
            }
            CostoProcesar saldoValidar = null;
            try
            {
                saldoValidar = saldos.ElementAt(index);
            }
            catch (Exception e)
            {
                var f = e;
            }

            // Ajustamos el costo Actual
            costoTotal = this.AjustarCostoTotal(cantidad, costoTotal);

            bool asignarCosto = true;
            if (costoTotal == 0) asignarCosto = false;
            if (GlobalCalculator.RedondearMonto(saldoValidar.Cantidad + cantidad) < 0) asignarCosto = false;

            // Solo si se asigna el costo procesamos el saldo del movimiento
            try
            {
                if (asignarCosto)
                {
                    saldos[index].Cantidad += cantidad;
                    saldos[index].CostoTotal += costoTotal;
                    decimal lastCost = saldos[index].CostoUnitario;
                    saldos[index].CostoUnitario = saldos[index].Cantidad != 0m
                        ? GlobalCalculator.RedondearMontoTotalPrecision(saldos[index].CostoTotal / saldos[index].Cantidad)
                        : 0m;
                    saldos[index].LastUnitCost = (saldos[index].CostoUnitario ==0)? lastCost : saldos[index].CostoUnitario;

                    // Si hay diferencias con el costo total es menor al valor indicado, enviamos ceros
                    if (saldos[index].CostoTotal < 0.00001m)
                    {
                        saldos[index].CostoTotal = 0m;
                    }

                    if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                    {
                        saldos[index].LastProcessPlantId = id_personProcessPlant;
                    }
                }
            }
            catch (Exception e)
            {
                var f = e;
            }
            

            return asignarCosto;
        }

        private CostoProcesar[] CalcularSaldoValorizacion(DateTime fechaCorte, int id_allocationType,
            IList<ProductionCostProductValuationInventoryMove> detallesProcesar, List<DataProcesarInventario> datosTotProcesar)
        {
            string parametroMetodoValorizacion = getParametroMetodoValorizacion();

            // Recuperamos el saldo Inicial
            var saldoInicial = this.GetSaldosIniciales(fechaCorte, id_allocationType)
                .Select(e => new CostoProcesar()
                {
                    Orden = 0,
                    IdBodega = e.idBodega,
                    IdItem = e.idItem,
                    Cantidad = e.cantidad,
                    CostoUnitario = e.costo_unitario,
                    CostoTotal = e.costo_total,
                    LastProcessPlantId = e.id_personProcessPlant
                });


            var movimientosProcesar = detallesProcesar
            .Select(e => new CostoProcesar()
            {
                Orden = e.ordenAsigCost,
                IdBodega = e.idBodega,
                IdItem = e.idItem,
                Cantidad = e.cantidad,
                CostoUnitario = e.costoUnitario,
                CostoTotal = e.costoTotal,
                LastProcessPlantId = datosTotProcesar.FirstOrDefault(r => r.IdInventoryMoveDetail == e.idInventoryMoveDetail)?.PlantaProcesoId
                //PlantaProceso = OrdenamientoBodegaAux.GetProcesoCostoInicialDesdeBodega(e.idBodega)
            });

            var registrosProcesar = saldoInicial
                                        .Union(movimientosProcesar)
                                        .OrderBy(e => e.Orden);

            var saldos = new List<CostoProcesar>();
            foreach (var registro in registrosProcesar)
            {
                // inicializamos la lista
                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                {
                    registro.IdBodega = 0;
                    if (!saldos.Any(e => e.IdBodega == registro.IdBodega 
                                         && e.IdItem == registro.IdItem
                                         && e.LastProcessPlantId == registro.LastProcessPlantId
                                         ))
                    {
                        saldos.Add(new CostoProcesar()
                        {
                            IdBodega = registro.IdBodega,
                            IdItem = registro.IdItem,
                            LastProcessPlantId = registro.LastProcessPlantId
                        });
                    }

                }

                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
                {
                    if (!saldos.Any(e => e.IdBodega == registro.IdBodega && e.IdItem == registro.IdItem))
                    {
                        saldos.Add(new CostoProcesar()
                        {
                            IdBodega = registro.IdBodega,
                            IdItem = registro.IdItem,
                        });
                    }
                }
                    

                this.CalcularSaldoFinal(ref saldos,
                                            registro.IdBodega,
                                            registro.IdItem,
                                            registro.Cantidad,
                                            registro.CostoTotal,
                                            registro.LastProcessPlantId
                                            );
            }

            return saldos
                .Where(e => e.Cantidad > 0)
                .ToArray();
        }

        private void CalularCostosManualesExcepciones(  DataProcesarInventario datoProcesar,
                                                        ProcesamientoValorizacion parametroCalculo,
                                                        CosteoValidacionesExcepcionDto configuracionExcepcion,
                                                        ref List<CostoProcesar> saldos,
                                                        ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
                                                        List<ProductionCostProductValuationWarning> novedades,
                                                        ref int ordenAsigCost,
                                                        int? indiceReproceso)
        {
            decimal precioUnitario = 0m;
            decimal? costoTotal = 0m;

            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;


            switch (configuracionExcepcion.CodeBehavior)
            {
                case "ULTIMOCOSTOITEM":
                    var ResultregistrosItemMotivo = productionsCostProductValuation
                                                                .Where(r => /* r.IdInventoryReason == movimiento.inventoryReasonExitId  &&*/ 
                                                                            r.idItem == datoProcesar.IdItem
                                                                            && r.idInventoryMoveDetail != datoProcesar.IdInventoryMoveDetail
                                                                            && r.costoUnitario > 0
                                                                            )
                                                                .ToList();

                    //var ResultregistrosItemMotivo = datosProcesarTodos
                    //                                .Where(r => /* r.IdInventoryReason == movimiento.inventoryReasonExitId  &&*/ r.IdItem == movimiento.dataProcesar.IdItem)
                    //                                .ToList();

                    var registrosItemMotivo = ResultregistrosItemMotivo
                                                                .LastOrDefault();

                    if (registrosItemMotivo == null)
                    {
                        var item = this.GetItemFromTempData(datoProcesar.IdItem);
                        var bodega = this.GetWarehouseFromTempData(datoProcesar.IdWarehouse);
                        var fechaMovimiento = inventoryMoveDetail != null
                            ? inventoryMoveDetail.InventoryMove.Document.emissionDate.ToString("dd/MM/yyyy")
                            : string.Empty;

                        novedades.Add(new ProductionCostProductValuationWarning()
                        {
                            idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                            orden = novedades.Count + 1,
                            descripcion = $"Error al recuperar el detalle del movimiento de excepcion inventario: {datoProcesar.IdInventoryMoveDetail} " +
                                    $"Bodega {bodega.name}. Ítem: [{item.masterCode}] - {item.name}. Fecha movimiento: {fechaMovimiento}.",
                            activo = true,

                            idUsuarioCreacion = this.ActiveUserId,
                            fechaHoraCreacion = DateTime.Now,
                            idUsuarioModificacion = this.ActiveUserId,
                            fechaHoraModificacion = DateTime.Now,
                        });
                    }

                    if ((registrosItemMotivo?.costoUnitario ?? 0) > 0)
                    {

                        precioUnitario = registrosItemMotivo.costoUnitario;
                        costoTotal = registrosItemMotivo.costoUnitario * cantidadMovimiento ;

                    }
                    else
                    {
                        var saldo = saldos.FirstOrDefault(r => r.IdItem == datoProcesar.IdItem);
                        //if ((saldo?.CostoUnitario ?? 0) == 0) continue;
                        if ((saldo?.CostoUnitario ?? 0) == 0)
                        {
                            precioUnitario = 0;
                            costoTotal = 0;
                        }
                        else
                        {
                            precioUnitario = saldo.CostoUnitario;
                            costoTotal = saldo.CostoUnitario * cantidadMovimiento;
                        }
                    }

                    break;
            }

            if (!costoTotal.HasValue && costoTotal <= 0)
            {
                this.SetErrorCostoCero(datoProcesar, novedades);
            }


            var indice = productionsCostProductValuation
                                    .FindIndex(e => e.idInventoryMoveDetail == datoProcesar.IdInventoryMoveDetail);

            var asignarCosto = this.CalcularSaldoFinal( ref saldos,
                                                        inventoryMoveDetail.id_warehouse,
                                                        inventoryMoveDetail.id_item,
                                                        cantidadMovimiento,
                                                        costoTotal ?? 0m,
                                                        inventoryMoveDetail.id_personProcessPlant);


            if (!asignarCosto) costoTotal = precioUnitario = 0m;
            costoTotal = Math.Abs(costoTotal ?? 0m);
            if (costoTotal > 0)
            {
                ordenAsigCost++;
                this.RemoverErrores(datoProcesar.IdInventoryMoveDetail, novedades);
            }

            if (indiceReproceso.HasValue)
            {

                productionsCostProductValuation[indice].ordenAsigCost = ordenAsigCost;
                productionsCostProductValuation[indice].costoUnitario = precioUnitario;
                productionsCostProductValuation[indice].costoTotal = costoTotal ?? 0m;

            }
            else
            {
                productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                {
                    ordenAsigCost = ordenAsigCost,
                    orden = productionsCostProductValuation.Count + 1,
                    idInventoryMoveDetail = inventoryMoveDetail.id,
                    idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                    idBodega = inventoryMoveDetail.id_warehouse,
                    idItem = datoProcesar.IdItem,
                    idMotivo = datoProcesar.IdInventoryReason,
                    esTransferencia = parametroCalculo.EsTransferencia,
                    naturaleza = parametroCalculo.Naturaleza,
                    tipoCalculo = parametroCalculo.TipoCalculo,
                    accion = m_AsignarCostoEgresoTransKey,
                    id_lot = inventoryMoveDetail.id_lot ?? 0,
                    valorizacion = parametroCalculo.Valorizacion,
                    costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                    costoTotalAnterior = cantidadMovimiento * inventoryMoveDetail.unitPrice,

                    cantidad = cantidadMovimiento,
                    costoTotal = costoTotal ?? 0m,
                    costoUnitario = precioUnitario,
                    activo = true,

                    idUsuarioCreacion = this.ActiveUserId,
                    fechaHoraCreacion = DateTime.Now,
                    idUsuarioModificacion = this.ActiveUserId,
                    fechaHoraModificacion = DateTime.Now,
                });

            }

        }
        private void RecalcularCostosManualesExcepciones(ref int ordenAsigCost,
                                                            IList<DataProcesarInventario> datosProcesar,
                                                            IList<DataProcesarInventario> datosProcesarTodos,
                                                            ref List<CostoProcesar> saldos,
                                                            ref List<ProductionCostProductValuationWarning> novedades,
                                                            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation)
        {
            // obtener movimientos que cumplen alguna validacion de excepcion
            var aProcesarList = (from data in datosProcesar
                                 join regla in _validacionCosteoManualExcepcion
                                 on data.IdInventoryReason equals regla.EntryInventoryReasonId
                                 join mov in productionsCostProductValuation
                                 on data.IdInventoryMoveDetail equals mov.idInventoryMoveDetail
                                 select new
                                 {
                                     codigoRegla = regla.CodeBehavior,
                                     inventoryReasonExitId = regla.ExitInventoryReasonId,
                                     dataProcesar = data,
                                     movimiento = mov
                                 }).ToArray();

            foreach (var movimiento in aProcesarList)
            {
                decimal precioUnitario = 0m, costoTotal = 0m;

                switch (movimiento.codigoRegla)
                {
                    case "ULTIMOCOSTOITEM":
                        var ResultregistrosItemMotivo = productionsCostProductValuation
                                                                    .Where(r => /* r.IdInventoryReason == movimiento.inventoryReasonExitId  &&*/ r.idItem == movimiento.dataProcesar.IdItem
                                                                                && r.idInventoryMoveDetail != movimiento.dataProcesar.IdInventoryMoveDetail
                                                                                && r.costoUnitario > 0
                                                                                )
                                                                    .ToList();

                        //var ResultregistrosItemMotivo = datosProcesarTodos
                        //                                .Where(r => /* r.IdInventoryReason == movimiento.inventoryReasonExitId  &&*/ r.IdItem == movimiento.dataProcesar.IdItem)
                        //                                .ToList();

                        var registrosItemMotivo = ResultregistrosItemMotivo
                                                                    .LastOrDefault();

                        if ((registrosItemMotivo?.costoUnitario ?? 0) > 0)
                        {

                            precioUnitario = registrosItemMotivo.costoUnitario;
                            costoTotal = registrosItemMotivo.costoUnitario * movimiento.movimiento.cantidad;

                        }
                        else
                        {
                            var saldo = saldos.FirstOrDefault(r => r.IdItem == movimiento.movimiento.idItem);
                            if ((saldo?.CostoUnitario ?? 0) == 0) continue;

                            precioUnitario = saldo.CostoUnitario;
                            costoTotal = saldo.CostoUnitario * movimiento.movimiento.cantidad;
                        }

                        break;
                }


                var indice = productionsCostProductValuation
                                        .FindIndex(e => e.idInventoryMoveDetail == movimiento.dataProcesar.IdInventoryMoveDetail);

                var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                            movimiento.movimiento.idBodega,
                                                            movimiento.movimiento.idItem,
                                                            movimiento.movimiento.cantidad,
                                                            costoTotal,
                                                            movimiento.dataProcesar.PlantaProcesoId);

                costoTotal = Math.Abs(costoTotal);
                if (costoTotal > 0 && precioUnitario > 0 && indice >= 0)
                {
                    ordenAsigCost++;
                    productionsCostProductValuation[indice].ordenAsigCost = ordenAsigCost;
                    productionsCostProductValuation[indice].costoUnitario = precioUnitario;
                    productionsCostProductValuation[indice].costoTotal = costoTotal;

                    this.RemoverErrores(movimiento.movimiento.idInventoryMoveDetail, novedades);
                }
            }
        }

        private void RecalcularCostosIngresoManuales(ref int ordenAsigCost,
            IList<DataProcesarInventario> datosProcesar, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationWarning> novedades,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation)
        {
            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
            // Reprocesaremos los registros manuales
            var idsInventoryMoveDetail = datosProcesar.Select(e => e.IdInventoryMoveDetail);
            var agrupacionMotivosConfiguracion = datosProcesar
                .Where(e => e.ConfiguracionCostManual != DataProviderInventoryReason.m_CalculoNinguno)
                .GroupBy(e => new {
                    e.ConfiguracionCostManual,
                    e.IdInventoryReasonCostManual,
                })
                .Select(e => new {
                    e.Key.ConfiguracionCostManual,
                    e.Key.IdInventoryReasonCostManual,
                    IdsInventoryMoveDetail = e.Select(x => x.IdInventoryMoveDetail).ToArray(),
                })
                .ToArray();

            foreach (var agrupacionProcesar in agrupacionMotivosConfiguracion)
            {
                var movimientosIngreso = productionsCostProductValuation
                    .Where(e => agrupacionProcesar.IdsInventoryMoveDetail.Contains(e.idInventoryMoveDetail))
                    .ToList();
                if (!movimientosIngreso.Any(e => e.costoTotal == 0m)) continue;

                var movimientosEgreso = productionsCostProductValuation
                    .Where(e => e.naturaleza == m_NaturalezaEgresoKey && e.idMotivo == agrupacionProcesar.IdInventoryReasonCostManual
                        && idsInventoryMoveDetail.Contains(e.idInventoryMoveDetail))
                    .ToArray();

                // Si alguno de los movimientos de egreso no tiene costo, continuamos
                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
                {
                    if (movimientosEgreso.Any(e => e.costoTotal == 0m) && movimientosEgreso.Length > 0) continue;
                }

                var movimimientosEgresoProductoTalla = movimientosEgreso
                    .Select(e => this.ConvertToMovimientoProductoTalla(e))
                    .ToArray();

                foreach (var movimientoIngreso in movimientosIngreso)
                {
                    decimal precioUnitario = 0m, costoTotal = 0m;

                    var movimientoIngresoProductoTalla = this.ConvertToMovimientoProductoTalla(movimientoIngreso);
                    if (agrupacionProcesar.ConfiguracionCostManual == DataProviderInventoryReason.m_CalculoProductoTalla)
                    {
                        var agrupacionesCostoTalla = movimimientosEgresoProductoTalla
                            .GroupBy(e => new
                            {
                                e.IdItem,
                                e.IdTalla,
                            })
                            .Select(e => new MovimientoProductoTalla()
                            {
                                IdItem = e.Key.IdItem,
                                IdTalla = e.Key.IdTalla,
                                CantidadMaster = e.Sum(x => x.CantidadMaster),
                                CostoTotalMaster = e.Sum(x => x.CostoTotalMaster),
                                CantidadLibras = e.Sum(x => x.CantidadLibras),
                                CostoTotalLibras = e.Sum(x => x.CostoTotalLibras),
                            })
                            .ToList();

                        var totalLibrasIngresos = movimientosIngreso
                            .Select(e => this.ConvertToMovimientoProductoTalla(e))
                            .Where(e => e.IdItem == movimientoIngresoProductoTalla.IdItem && e.IdTalla == movimientoIngresoProductoTalla.IdTalla)
                            .Sum(e => e.CantidadLibras);


                        var agrupacionProductoTalla = agrupacionesCostoTalla
                            .FirstOrDefault(e => e.IdTalla == movimientoIngresoProductoTalla.IdTalla
                                && Math.Abs(e.CantidadLibras - totalLibrasIngresos) <= 0.0001m);

                        //if (agrupacionesCostoTalla.Any(e => e.CostoTotalLibras == 0m) && agrupacionesCostoTalla.Count > 0) continue;

                        if (agrupacionProductoTalla != null)
                        {
                            var factorCosto = totalLibrasIngresos > 0m
                                ? GlobalCalculator.RedondearMontoTotalPrecision(movimientoIngresoProductoTalla.CantidadLibras / totalLibrasIngresos)
                                : 0m;

                            costoTotal = GlobalCalculator.RedondearMontoTotalPrecision(agrupacionProductoTalla.CostoTotalMaster * factorCosto);
                            precioUnitario = movimientoIngresoProductoTalla.CantidadMaster != 0m
                                ? GlobalCalculator.RedondearMontoTotalPrecision(costoTotal / movimientoIngresoProductoTalla.CantidadMaster * 1m)
                                : 0m;
                        }
                    }
                    else if (agrupacionProcesar.ConfiguracionCostManual == DataProviderInventoryReason.m_CalculoPromedioLibra)
                    {
                        var totalLibrasIngresos = movimientosIngreso
                            .Select(e => this.ConvertToMovimientoProductoTalla(e))
                            .Sum(e => e.CantidadLibras);

                        var costoTotalEgresos = movimimientosEgresoProductoTalla
                            .Sum(e => e.CostoTotalMaster);

                        var factorCosto = totalLibrasIngresos > 0m
                            ? GlobalCalculator.RedondearMontoTotalPrecision(movimientoIngresoProductoTalla.CantidadLibras / totalLibrasIngresos)
                            : 0m;

                        costoTotal = GlobalCalculator.RedondearMontoTotalPrecision(costoTotalEgresos * factorCosto);
                        precioUnitario = movimientoIngresoProductoTalla.CantidadMaster != 0m
                            ? GlobalCalculator.RedondearMontoTotalPrecision(costoTotal / movimientoIngresoProductoTalla.CantidadMaster * 1m)
                            : 0m;
                    }

                    // Asignamos y procesamos el costo encontrado
                    var indice = productionsCostProductValuation
                        .FindIndex(e => e.idInventoryMoveDetail == movimientoIngreso.idInventoryMoveDetail);

                    var configMovimiento = datosProcesar.FirstOrDefault(r => r.IdInventoryMoveDetail == movimientoIngreso.idInventoryMoveDetail);
                    var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                                movimientoIngreso.idBodega,
                                                                movimientoIngreso.idItem,
                                                                movimientoIngreso.cantidad,
                                                                costoTotal,
                                                                configMovimiento.PlantaProcesoId);

                    if (!asignarCosto) costoTotal = precioUnitario = 0m;

                    costoTotal = Math.Abs(costoTotal);
                    if (costoTotal > 0 && precioUnitario > 0 && indice >= 0)
                    {
                        ordenAsigCost++;
                        productionsCostProductValuation[indice].ordenAsigCost = ordenAsigCost;
                        productionsCostProductValuation[indice].costoUnitario = precioUnitario;
                        productionsCostProductValuation[indice].costoTotal = costoTotal;

                        this.RemoverErrores(movimientoIngreso.idInventoryMoveDetail, novedades);
                    }
                }
            }
        }

        private class MovimientoProductoTalla
        {
            public int IdItem { get; set; }
            public int IdTalla { get; set; }
            public decimal CantidadMaster { get; set; }
            public decimal PrecioUnitarioMaster { get; set; }
            public decimal CostoTotalMaster { get; set; }
            public decimal CantidadLibras { get; set; }
            public decimal PrecioUnitarioLibras { get; set; }
            public decimal CostoTotalLibras { get; set; }
        }
        private MovimientoProductoTalla ConvertToMovimientoProductoTalla(ProductionCostProductValuationInventoryMove detalle)
        {
            var unidadMedidaLibras = this.GetMetricUnitFromTempData("Lbs");
            var item = this.GetItemFromTempData(detalle.idItem);
            var presentacion = item?.Presentation;
            var factorPresentacion = presentacion != null
                ? (decimal?)(presentacion.minimum * presentacion.maximum)
                : null;
            var factorLibras = ((presentacion != null) && (presentacion.id_metricUnit != unidadMedidaLibras.id))
                ? this.GetMetricUnitConversionFromTempData(presentacion.id_metricUnit, unidadMedidaLibras.id)?.factor
                : 1m;

            decimal cantidadLibras, precioUnitarioLibras, costoTotalLibras;
            if (factorLibras.HasValue && factorPresentacion.HasValue)
            {
                cantidadLibras = GlobalCalculator.RedondearMontoMayorPrecision(detalle.cantidad * factorPresentacion.Value * factorLibras.Value);
                costoTotalLibras = detalle.costoTotal;
                precioUnitarioLibras = cantidadLibras != 0m
                    ? Math.Abs(detalle.costoTotal / cantidadLibras)
                    : 0m;
            }
            else
            {
                cantidadLibras = precioUnitarioLibras = costoTotalLibras = 0m;
            }

            return new MovimientoProductoTalla()
            {
                IdItem = detalle.idItem,
                IdTalla = item.ItemGeneral.id_size ?? 0,
                CantidadMaster = Math.Abs(detalle.cantidad),
                PrecioUnitarioMaster = detalle.costoUnitario,
                CostoTotalMaster = detalle.costoTotal,
                CantidadLibras = Math.Abs(cantidadLibras),
                CostoTotalLibras = costoTotalLibras,
                PrecioUnitarioLibras = precioUnitarioLibras,
            };
        }
        private MetricUnit GetMetricUnitFromTempData(string codigo)
        {
            var key = $"MetricUnit_{codigo}";
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as MetricUnit;
            }
            else
            {
                var metricUnit = DataProviderMetricUnit.MetricUnitByCode(codigo);
                TempData[key] = metricUnit;
                TempData.Keep(key);

                return metricUnit;
            }
        }

        private void ActualizarEstadosPeriodoBodega(string codigoEstado, int año, int mes,
            DateTime? fechaInicio, DateTime? fechaFin, int[] idsBodegas)
        {
            var idEstadosPerido = db.AdvanceParameters.FirstOrDefault(e => e.code == "EPIV1").id;
            var idEstadoEnValorizacion = db.AdvanceParametersDetail
                .FirstOrDefault(e => e.id_AdvanceParameters == idEstadosPerido && e.valueCode.Trim() == codigoEstado)?
                .id;

            var idCompany = this.ActiveCompanyId;
            var idDivision = this.ActiveDivision.id;
            var idSucursal = this.ActiveSucursal.id;
            foreach (var idBodega in idsBodegas)
            {
                var detallesPeriodo = db.InventoryPeriodDetail
                    .Where(d => d.InventoryPeriod.id_warehouse == idBodega
                        && d.InventoryPeriod.year == año
                        && d.dateInit.Month == mes
                        && d.dateEnd.Month == mes
                        && d.InventoryPeriod.id_Company == idCompany
                        && d.InventoryPeriod.id_Division == idDivision
                        && d.InventoryPeriod.id_BranchOffice == idSucursal
                        && d.isClosed && d.InventoryPeriod.isActive)
                    .ToArray();

                var fechaPivote = fechaInicio.Value;
                while (fechaPivote <= fechaFin.Value)
                {
                    var periodosActualizar = detallesPeriodo
                        .Where(e => e.dateInit >= fechaPivote && e.dateEnd <= fechaPivote);

                    foreach (var periodoActualizar in periodosActualizar)
                    {
                        periodoActualizar.id_PeriodState = idEstadoEnValorizacion ?? 0;
                        db.InventoryPeriodDetail.Attach(periodoActualizar);
                        db.Entry(periodoActualizar).State = EntityState.Modified;
                    }

                    fechaPivote = fechaPivote.AddDays(1);
                }
            }
        }

        private static string GetAccionCosto(string accion)
        {
            switch (accion)
            {
                case m_MantenerCostoAsignadoKey:
                    return "Mantener Costo Asignado";
                case m_CalcularCostoPromedioKey:
                    return "Calcular Costo Promedio";
                case m_AsignarCostoCoeficienteKey:
                    return "Asignar Costo Coeficiente";
                case m_AsignarCostoEgresoTransKey:
                    return "Asignar Costo Egreso Transferencia";
                case m_AsignarCostoProcesoTransKey:
                    return "Asignar Costo Proceso";

                default:
                    throw new Exception("acción no reconocida.");
            }
        }

        private List<InventoryDetailCostTranferDto> prepareMovimientos(int[] inventoryMoveDetailIds)
        {
            InventoryMoveDetail[] movimientosDetalles = db.InventoryMoveDetail
                                                                .Where(r => inventoryMoveDetailIds.Contains(r.id))
                                                                .ToArray();

            var tranferData = db.InventoryMoveDetailTransfer
                                                    .Where(r => inventoryMoveDetailIds.Contains((r.id_inventoryMoveDetailExit ?? 0)))
                                                    .ToArray();

            int[] movimientoTransferIngresoIds = tranferData
                                                    .Select(r => r.id_inventoryMoveDetailEntry)
                                                    .ToArray();

            InventoryMoveDetail[] movimientosDetallesTransferIngreso = db.InventoryMoveDetail
                                                                             .Where(r => movimientoTransferIngresoIds.Contains(r.id))
                                                                             .ToArray();
            return new List<InventoryDetailCostTranferDto>();
        }
        private void ClearTempData()
        {

            _ordenamientos2da = new List<DataProcesarInventario>();
            _movimientosInventoryDetail = new List<InventoryDetailCostTranferDto>();
            _validacionCosteoManualExcepcion = null;
            _costosMovimientosTest = null;

            var tempCategories = new[]
            {
                "InventoryReason_",
                "WareHouse_",
                "MetricUnitConversion_",
                "InventoryMoveDetail_",
                "MetricUnit_",
            };

            var clavesBorrar = new List<string>();
            foreach (var tempCategory in tempCategories)
            {
                var tempsKey = this.TempData
                    .Where(e => e.Key.StartsWith(tempCategory))
                    .Select(e => e.Key);

                clavesBorrar.AddRange(tempsKey);
            }

            foreach (var claveBorrar in clavesBorrar)
            {
                this.TempData.Remove(claveBorrar);
            }
        }
        #endregion

        #region Métodos para la generación de reportes
        [HttpPost]
        public JsonResult Report(int id)
        {
            string message;
            bool isValid;

            try
            {
                // Consultamos la valorización desde la bs
                var productValuation = db.ProductionCostProductValuation
                    .FirstOrDefault(e => e.id == id);

                productValuation.ProductionCostProductValuationExecutions = productValuation
                        .ProductionCostProductValuationExecutions
                        .Where(d => d.isActive)
                        .ToList();

                productValuation.ProductionCostProductValuationInventoryMove = productValuation
                    .ProductionCostProductValuationInventoryMove
                    .Where(d => d.activo)
                    .ToList();

                productValuation.ProductionCostProductValuationBalance = productValuation
                    .ProductionCostProductValuationBalance
                    .Where(d => d.activo)
                    .ToList();

                productValuation.ProductionCostProductValuationWarehouse = productValuation
                    .ProductionCostProductValuationWarehouse
                    .Where(d => d.isActive)
                    .ToList();

                productValuation.ProductionCostProductValuationWarning = productValuation
                    .ProductionCostProductValuationWarning
                    .Where(d => d.activo)
                    .ToList();

                // Guardamos en memoria Temporal
                TempData[m_CostProductValuationExcelModelKey] = productValuation;
                TempData.Keep(m_CostProductValuationExcelModelKey);

                message = "Generando Excel....";
                isValid = true;
            }
            catch (Exception exception)
            {
                message = "Error al generar el excel: " + exception.GetBaseException()?.Message ?? string.Empty;
                isValid = false;
            }

            var result = new
            {
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileContentResult GenerateExcelDynamic()
        {
            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
            {
                return generateExcelDynamicMetodoProceso();
            }
            else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
            {
                return generateExcelDynamicMetodoLote();
            }
            return new FileContentResult(null, null);
        }

        private void AddFormulaToColumn(ExcelWorksheet ws, string colTitle, string formula, int colNumber)
        {
            ws.Cells[1, colNumber].Value = colTitle;
            var col_range = ws.Cells[2, colNumber, ws.Dimension.End.Row, colNumber];
            col_range.Formula = formula;
        }

        private CriterioResumen[] GenerateResumen(IList<ProductValuationInventoryMoveDTO> detalles)
        {
            var categorias = detalles
                .Select(e => e.CodigoCategoriaMotivoMovimiento)
                .Distinct()
                .ToArray();

            var criterios = new List<CriterioResumen>();
            foreach (var categoria in categorias)
            {
                var nombreCategoria = DataProviderInventoryReason.GetCategoriaCostoString(categoria);
                var detallesProcesar = detalles
                    .Where(e => e.CodigoCategoriaMotivoMovimiento == categoria)
                    .ToArray();
                var totalPeso = detallesProcesar.Sum(d => d.CantidadLibras);
                var totalCosto = detallesProcesar.Sum(d => d.CostoTotalLibras);

                var criterio = new CriterioResumen(categoria, nombreCategoria, totalPeso, totalCosto);
                criterios.Add(criterio);
            }

            return this.ProcesarCriterios(criterios.ToArray());
        }

        private CriterioResumen[] GenerateResumen(IList<ProductionCostProductValuationInventoryMove> detalles)
        {
            var idsMotivos = detalles.Select(e => e.idMotivo).Distinct();
            var categorias = idsMotivos
                .Select(e => this.GetInventoryReasonFromTempData(e))
                .GroupBy(e => e.CategoriaCosto)
                .Select(e => new {
                    Categoria = e.Key,
                    IdsMotivos = e.Select(x => x.id).ToArray()
                })
                .ToArray();

            var criterios = new List<CriterioResumen>();
            foreach (var categoria in categorias)
            {
                var nombreCategoria = DataProviderInventoryReason.GetCategoriaCostoString(categoria.Categoria);
                var detallesProcesar = detalles
                    .Where(e => categoria.IdsMotivos.Contains(e.idMotivo))
                    .ToArray();
                var totalPeso = detallesProcesar.Sum(d => d.cantidad);
                var totalCosto = detallesProcesar.Sum(d => d.costoTotal);

                var criterio = new CriterioResumen(categoria.Categoria, nombreCategoria, totalPeso, totalCosto);
                criterios.Add(criterio);
            }

            return this.ProcesarCriterios(criterios.ToArray());
        }

        private CriterioResumen[] ProcesarCriterios(CriterioResumen[] criterios)
        {
            var saldoInicial = criterios
                .FirstOrDefault(e => e.Codigo == DataProviderInventoryReason.m_SaldoInicial)
                    ?? new CriterioResumen(DataProviderInventoryReason.m_SaldoInicial,
                        DataProviderInventoryReason.GetCategoriaCostoString(DataProviderInventoryReason.m_SaldoInicial));

            var compras = criterios.FirstOrDefault(e => e.Codigo == DataProviderInventoryReason.m_Compras)
                ?? new CriterioResumen(DataProviderInventoryReason.m_Compras,
                    DataProviderInventoryReason.GetCategoriaCostoString(DataProviderInventoryReason.m_Compras));

            var subtotal1_peso = saldoInicial.Peso + compras.Peso;
            var subtotal1_dolares = saldoInicial.Dolares + compras.Dolares;
            var subtotal1 = new CriterioResumen("SBTTL1", "Subtotal 1", subtotal1_peso, subtotal1_dolares, true);


            var costoVentas = criterios
                .FirstOrDefault(e => e.Codigo == DataProviderInventoryReason.m_CostoVentas)
                    ?? new CriterioResumen(DataProviderInventoryReason.m_CostoVentas,
                         DataProviderInventoryReason.GetCategoriaCostoString(DataProviderInventoryReason.m_CostoVentas));

            var ajusteInventario = criterios
                .FirstOrDefault(e => e.Codigo == DataProviderInventoryReason.m_AjusteInventario)
                    ?? new CriterioResumen(DataProviderInventoryReason.m_AjusteInventario,
                        DataProviderInventoryReason.GetCategoriaCostoString(DataProviderInventoryReason.m_AjusteInventario));



            var ingresosTransferencias = criterios
                .FirstOrDefault(e => e.Codigo == DataProviderInventoryReason.m_IngresosTransferencias)
                    ?? new CriterioResumen(DataProviderInventoryReason.m_IngresosTransferencias,
                         DataProviderInventoryReason.GetCategoriaCostoString(DataProviderInventoryReason.m_IngresosTransferencias));

            var egresosTransferencias = criterios
                .FirstOrDefault(e => e.Codigo == DataProviderInventoryReason.m_EgresosTransferencias)
                    ?? new CriterioResumen(DataProviderInventoryReason.m_EgresosTransferencias,
                         DataProviderInventoryReason.GetCategoriaCostoString(DataProviderInventoryReason.m_EgresosTransferencias));

            var inventarioFinal_peso = (saldoInicial.Peso + compras.Peso + costoVentas.Peso + ajusteInventario.Peso + ingresosTransferencias.Peso + egresosTransferencias.Peso) * (-1);
            var inventarioFinal_dolares = (saldoInicial.Dolares + compras.Dolares + costoVentas.Dolares + ajusteInventario.Dolares + ingresosTransferencias.Dolares + egresosTransferencias.Dolares) * (-1);
            var inventarioFinal = new CriterioResumen("CTCSINVFI", "Inventario Final", (decimal)inventarioFinal_peso, (decimal)inventarioFinal_dolares);

            var subtotal2_peso = inventarioFinal.Peso + costoVentas.Peso + ajusteInventario.Peso;
            var subtotal2_dolares = inventarioFinal.Dolares + costoVentas.Dolares + ajusteInventario.Dolares;
            var subtotal2 = new CriterioResumen("", "Subtotal 2", subtotal2_peso, subtotal2_dolares, true);

            var diferencia = new CriterioResumen("", "Diferencia", subtotal1_peso + subtotal2_peso, subtotal1_dolares + subtotal2_dolares, true);


            var diferenciaConciliacion_peso = ingresosTransferencias.Peso + egresosTransferencias.Peso;
            var diferenciaConciliacion_dolares = ingresosTransferencias.Dolares + egresosTransferencias.Dolares;
            var diferenciaConciliacion = new CriterioResumen("", "Diferencia Conciliación", diferenciaConciliacion_peso, diferenciaConciliacion_dolares, true);

            var resumen = new[]
            {
                saldoInicial,
                compras,
                subtotal1,
                new CriterioResumen(),
                inventarioFinal,
                costoVentas,
                ajusteInventario,
                subtotal2,
                new CriterioResumen(),
                diferencia,
                new CriterioResumen(),
                new CriterioResumen("Conciliación de Reprocesos y Transferencias:"),
                ingresosTransferencias,
                egresosTransferencias,
                diferenciaConciliacion
            };

            return resumen.ToArray();
        }

        public class CriterioResumen
        {
            public string Codigo { get; set; }
            public string Titulo { get; set; }
            public decimal Peso { get; set; }
            public decimal Dolares { get; set; }
            public bool EsTitulo { get; set; }
            public bool EsTotal { get; set; }

            public CriterioResumen(string codigo, string titulo, decimal? peso, decimal? dolares)
            {
                this.Codigo = codigo;
                this.Titulo = titulo;
                this.Peso = peso ?? 0m;
                this.Dolares = dolares ?? 0m;
                this.EsTitulo = false;
            }
            public CriterioResumen(string codigo, string titulo, decimal? peso, decimal? dolares, bool esTotal)
            {
                this.Codigo = codigo;
                this.Titulo = titulo;
                this.Peso = peso ?? 0m;
                this.Dolares = dolares ?? 0m;
                this.EsTitulo = false;
                this.EsTotal = esTotal;
            }
            public CriterioResumen(string codigo, string titulo)
            {
                this.Codigo = codigo;
                this.Titulo = titulo;
                this.Peso = 0m;
                this.Dolares = 0m;
                this.EsTitulo = false;
            }

            public CriterioResumen(string titulo)
            {
                this.Codigo = "";
                this.Titulo = titulo;
                this.Peso = 0m;
                this.Dolares = 0m;
                this.EsTitulo = true;
            }

            public CriterioResumen()
            {
                this.Codigo = "";
                this.Titulo = "";
                this.Peso = 0m;
                this.Dolares = 0m;
                this.EsTitulo = true;
            }
        }

        #endregion

        #region -de lk--
        //private InventoryDetailCostTranferDto[] getBulkDataForOrderCost(int[] inventoryMoveDetailsIds )
        //{
        //    List<InventoryDetailCostDto> inventoryDetailCost = new List<InventoryDetailCostDto>();
        //
        //    // obtener todos los movimientos
        //    var inventoryMoveDetails = db.InventoryMoveDetail
        //                                            .Where(r => inventoryMoveDetailsIds.Contains(r.id))
        //                                            .Select(r=> new InventoryDetailCostDto
        //                                            {
        //                                               ItemId = r.id_item,
        //                                               IdInventoryMoveDetail = r.id,
        //                                               id_inventoryMove = r.id_inventoryMove
        //                                            })
        //                                            .ToArray();
        //
        //    inventoryDetailCost.AddRange(inventoryMoveDetails);
        //
        //    // obtener las transferencias manuales
        //    var moveTransfers = db.InventoryMoveDetailTransfer
        //                                            .Where(r => r.id_inventoryMoveDetailExit.HasValue 
        //                                                        && inventoryMoveDetailsIds.Contains(r.id_inventoryMoveDetailExit.Value))
        //                                            .Select(r=> new InventoryDetailCostTranferLinkedDto
        //                                            {
        //                                                
        //                                                id_inventoryMoveDetailExit = r.id_inventoryMoveDetailExit.Value,
        //                                                id_inventoryMoveDetailEntry= r.id_inventoryMoveDetailEntry,
        //                                                IsAutomatic = false,
        //                                                 
        //
        //                                            })
        //                                            .ToArray();
        //
        //    int[] inventoryMoveDetailEntryIds = moveTransfers
        //                                            .Select(r => r.id_inventoryMoveDetailEntry)
        //                                            .ToArray();
        //
        //    //Obtener los Ingresos de esas transferencias
        //    var inventoryMoveDetailsTransferEntry = db.InventoryMoveDetail
        //                                                    .Where(r => inventoryMoveDetailEntryIds.Contains(r.id))
        //                                                    .Select(r => new InventoryDetailCostTranferDto
        //                                                    {
        //                                                         IdInventoryMoveDetail = r.id,
        //                                                         id_inventoryMove= r.id_inventoryMove,
        //                                                          
        //                                                    })
        //                                                    .ToArray();
        //
        //    // Crear Documento
        //    InventoryDetailCostTranferLinkedDto[] transferLinkendManual = (from traf in moveTransfers
        //                                                                   join movEntry in inventoryMoveDetailsTransferEntry
        //                                                                   on traf.id_inventoryMoveDetailExit equals movEntry.IdInventoryMoveDetail
        //                                                                   group movEntry by movEntry.
        //                                                                   select new InventoryDetailCostTranferLinkedDto
        //                                                                   {
        //                                                                        MoveDetailEntry = 
        //
        //                                                                   }).ToArray();
        //
        //    InventoryDetailCostTranferDto[] moveDetailManualTransfer = (from det in inventoryMoveDetails
        //                                                                join tranf in moveTransfers
        //                                                                on det.IdInventoryMoveDetail equals tranf.id_inventoryMoveDetailExit
        //                                                                join detEntry in inventoryMoveDetailsTransferEntry
        //                                                                on tranf.id_inventoryMoveDetailEntry equals detEntry.IdInventoryMoveDetail
        //                                                                select new InventoryDetailCostTranferDto
        //                                                                {
        //                                                                     
        //
        //                                                                }).ToArray();
        //
        //
        //    // procesar los mov que no son transferencias manuales
        //    int[] inventoryMoveDetailTranferManualIds = moveTransfers
        //                                                    .Select(r => r.id_inventoryMoveDetailExit)
        //                                                    .ToArray();
        //
        //    var idsForSeekTransferAutoIds = inventoryMoveDetailsIds
        //                                        .Where(r => !inventoryMoveDetailTranferManualIds.Contains(r))
        //                                        .ToArray();
        //
        //    var automaticTransferEntryIds = db.AutomaticTransfer
        //                                                    .Where(r => r.id_InventoryMoveExit.HasValue && idsForSeekTransferAutoIds.Contains(r.id_InventoryMoveExit.Value) )
        //                                                    .Select(r => new 
        //                                                    { 
        //                                                         id_inventoryMoveDetailExit = r.id_InventoryMoveExit.Value,
        //                                                         id_inventoryMoveEntry = r.id_InventoryMoveEntry
        //                                                    })
        //                                                    .ToArray();
        //    movimientosDetalles = db.InventoryMoveDetail
        //                                             .Where(r => automaticTransferEntryIds
        //                                                                .Select(r=> r.id_inventoryMoveEntry)
        //                                                                .Contains(r.id_inventoryMove)
        //                                                            && r.id_item == current.IdItem)
        //                                             .ToArray();
        //
        //
        //}
        #endregion

        private decimal GetOrdenBodegaMotivo(int idBodega, int idMotivo,
            int idInventoryMoveDetail, string naturaleza, DataProcesarInventario current, List<DataProcesarInventario> data, out decimal? ordenEgreso)
        {
            var ordenamiento = _ordenamientos.FirstOrDefault(r => r.Id_Warehouse == idBodega && r.Id_inventoryreason == idMotivo); // this.RecuperarOrdenamiento(idBodega, idMotivo);
            var motivo = this.GetInventoryReasonFromTempData(idMotivo);
            var esTransferencia = motivo.isForTransfer ?? false;

            if (ordenamiento == null)
            {
                var bodega = this.GetWarehouseFromTempData(idBodega);

                throw new Exception("No se ha encontrado un criterio de ordenamiento " +
                    $"para la bodega: {bodega.name} y el motivo: {motivo.name}. " +
                    $"IdMovimiento: {idInventoryMoveDetail}.");
            }

            decimal orden;
            if (esTransferencia && naturaleza == m_NaturalezaEgresoKey && (!string.IsNullOrEmpty(ordenamiento.Conector2) || !string.IsNullOrEmpty(ordenamiento.OrdenConector)))
            {
                OrdenamientoBodegaMotivo[] ordenamientosIngresos = null;
                if (!string.IsNullOrEmpty(ordenamiento.Conector2))
                {
                    ordenamientosIngresos = _ordenamientos.Where(r => r.Conector == ordenamiento.Conector2).ToArray();
                }
                else if (!string.IsNullOrEmpty(ordenamiento.OrdenConector))
                {
                    ordenamientosIngresos = _ordenamientos
                                                    .Where(r => r.OrdenConector == ordenamiento.OrdenConector 
                                                                && !( r.Id_Warehouse == ordenamiento.Id_Warehouse 
                                                                      && r.Id_inventoryreason == ordenamiento.Id_inventoryreason))
                                                    .ToArray();
                }
                

                InventoryMoveDetail movimientoDetalleEgreso = db.InventoryMoveDetail.FirstOrDefault(r => r.id == current.IdInventoryMoveDetail);
                if (movimientoDetalleEgreso == null)
                {
                    throw new Exception($"No se ha encontrado el Detalle del movimiento de Inventario {current.IdInventoryMoveDetail}");
                }

                var documentosRelacionados = db.InventoryMoveDetailTransfer
                                                    .Where(r => r.id_inventoryMoveDetailExit == current.IdInventoryMoveDetail)
                                                    .ToArray();

                InventoryMoveDetail[] movimientosDetallesIngresos = null;
                if ((documentosRelacionados?.Length ?? 0) > 0)
                {
                    int[] movimientosDetallesIds = documentosRelacionados.Select(r => r.id_inventoryMoveDetailEntry).ToArray();
                    movimientosDetallesIngresos = db.InventoryMoveDetail
                                                         .Where(r => movimientosDetallesIds.Contains(r.id))
                                                         .ToArray();
                }
                else
                {
                    var automaticTransferEntryIds = db.AutomaticTransfer
                                                            .Where(r => r.id_InventoryMoveExit == movimientoDetalleEgreso.id_inventoryMove)
                                                            .Select(r => r.id_InventoryMoveEntry)
                                                            .ToArray();

                    if ((automaticTransferEntryIds?.Length ?? 0) > 0)
                    {
                        movimientosDetallesIngresos = db.InventoryMoveDetail
                                                        .Where(r => automaticTransferEntryIds.Contains(r.id_inventoryMove)
                                                                    && r.id_item == current.IdItem)
                                                        .ToArray();

                    }
                    else
                    {
                        var documentSourceIngreso = db.DocumentSource
                                                             .FirstOrDefault(r => r.id_documentOrigin == movimientoDetalleEgreso.id_inventoryMove);

                        movimientosDetallesIngresos = db.InventoryMoveDetail
                                                        .Where(r => r.id_inventoryMove == documentSourceIngreso.id_document
                                                                       && r.id_warehouse == movimientoDetalleEgreso.id_warehouseEntry
                                                                       && r.id_warehouseLocation == movimientoDetalleEgreso.id_warehouseLocationEntry
                                                                       && r.id_lot == movimientoDetalleEgreso.id_lot
                                                                       && r.id_item == movimientoDetalleEgreso.id_item
                                                                       && r.id_productionCart == movimientoDetalleEgreso.id_productionCart
                                                                       && r.entryAmount == movimientoDetalleEgreso.exitAmount
                                                                       )
                                                        .ToArray();

                    }

                }


                if ((movimientosDetallesIngresos?.Length ?? 0) == 0)
                {
                    orden = ordenamiento.Orden;
                    ordenEgreso = null;
                }
                else
                {


                    int[] inventoryMoveIds = movimientosDetallesIngresos.Select(r => r.id_inventoryMove).ToArray();

                    InventoryMove[] movimientosIngresos = db.InventoryMove
                                                                    .Where(r => inventoryMoveIds.Contains(r.id))
                                                                    .ToArray();

                    //validar si el movimiento aplica en la regla de ordenamiento
                    var movimientosFiltrados = (from movin in movimientosIngresos
                                                join ors in ordenamientosIngresos
                                                on new { bod = (movin.idWarehouse ?? 0), reas = (movin.id_inventoryReason ?? 0) }
                                                equals new { bod = ors.Id_Warehouse, reas = ors.Id_inventoryreason }
                                                select new
                                                {
                                                    inventoryMoveId = movin.id,

                                                })
                                               .ToArray();

                    DataProcesarInventario[] ingresosOrden = null;
                    if ((movimientosFiltrados?.Length ?? 0) > 0)
                    {
                        #region -- s --
                        //join dets in movimientosDetalles
                        //                            on movin.id equals dets.id_inventoryMove
                        //                            where dets.id_item == current.IdItem

                        //var movimientosIngresosIds = movimientosFiltrados
                        //                                .Select(r => r.inventoryMoveId)
                        //                                .ToArray();

                        //InventoryMoveDetail[] movimientoIngresoDetalleRelacionado = db.InventoryMoveDetail
                        //                                                                .Where(r => movimientosIngresosIds.Contains(r.id_inventoryMove)
                        //                                                                        && r.id_item == current.IdItem)
                        //                                                                .ToArray();
                        #endregion

                        decimal segOrden = 0.001M;
                        ingresosOrden = (from det in movimientosDetallesIngresos
                                         join or in data
                                          on det.id equals or.IdInventoryMoveDetail
                                         select new DataProcesarInventario
                                         {
                                             ConfiguracionCostManual = or.ConfiguracionCostManual,
                                             DateCreate = or.DateCreate,
                                             EmissionDate = or.EmissionDate,
                                             IdInventoryMoveDetail = or.IdInventoryMoveDetail,
                                             IdInventoryReason = or.IdInventoryReason,
                                             IdInventoryReasonCostManual = or.IdInventoryReasonCostManual,
                                             IdItem = or.IdItem,
                                             IdLote = or.IdLote,
                                             IdWarehouse = or.IdWarehouse,
                                             MasterCode = or.MasterCode,
                                             NatureMove = or.NatureMove,
                                             Proceso = or.Proceso,
                                             ProcessCreate = or.ProcessCreate,
                                             OrdenMotivoTipoDocum = ordenamiento.Orden,
                                             OrdenTransferencias = (det.id_lot ?? 0) + det.id_item + (det.id_productionCart ?? 0) + det.entryAmount + (segOrden + 0.001M)

                                         }).ToArray();

                        _ordenamientos2da.AddRange(ingresosOrden);
                    }


                    if ((ingresosOrden?.Length ?? 0) > 0)
                    {
                        orden = ordenamiento.Orden;
                        ordenEgreso = current.IdLote + current.IdItem + (current.ProductionCartId ?? 0) + current.ExitAmount + (0.001M);
                    }
                    else
                    {
                        orden = ordenamiento.Orden;
                        ordenEgreso = null;
                    }

                }


            }
            else if (esTransferencia && naturaleza == m_NaturalezaIngresoKey)
            {
                var procesarIngresoOrden = _ordenamientos2da.FirstOrDefault(r => r.IdInventoryMoveDetail == current.IdInventoryMoveDetail);
                if (procesarIngresoOrden == null)
                {
                    orden = ordenamiento.Orden;
                    ordenEgreso = null;
                }
                else
                {
                    orden = procesarIngresoOrden.OrdenMotivoTipoDocum;
                    ordenEgreso = procesarIngresoOrden.OrdenTransferencias;
                }

            }
            else
            {
                orden = ordenamiento.Orden;
                ordenEgreso = null;
            }

            return orden;
        }
        private CosteoValidacionesDinamicas[] getValidacionesDinamicas()
        {
            const string m_sql = "Select * from CosteoValidacionesDinamicas;";
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            CosteoValidacionesDinamicas[] _validacionesDinamicas = null;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();
                var result = cnn.Query<CosteoValidacionesDinamicas>(m_sql);
                _validacionesDinamicas = result.ToArray();
                cnn.Close();
            }
            return _validacionesDinamicas;
        }
        private void getOrdenamientoCodigo(int anio, int mes)
        {

            string m_sql = $"Select * from orden_motivos_bodegas where  Anio = {anio} And Mes = {mes};";
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            _ordenamientos = null;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();
                var result = cnn.Query<OrdenamientoBodegaMotivoDb>(m_sql);
                _ordenamientos = result.Select(r => r.ToOrdenamientoBodegaMotivo()).ToArray();
                cnn.Close();
            }
        }

        private void getValidacionExcepcion()
        {

            const string m_sql = "Select * from CosteoValidacionesExcepcion;";
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            _validacionCosteoManualExcepcion = null;
            IEnumerable<CosteoValidacionesExcepcionDto> result = null;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();
                result = cnn.Query<CosteoValidacionesExcepcionDto>(m_sql);
                cnn.Close();
            }
            if ((result?.Count() ?? 0) > 0)
            {
                var reglas = result.ToList();
                string[] entryCodigos = reglas.Select(r => r.EntryInventoryReasonCode).ToArray();
                string[] exitCodigos = reglas.Select(r => r.ExitInventoryReasonCode).ToArray();

                var inventoryReasonEntryList = db.InventoryReason
                                                    .Where(r => entryCodigos.Contains(r.code))
                                                    .ToArray();
                var inventoryReasonExitList = db.InventoryReason
                                                    .Where(r => exitCodigos.Contains(r.code))
                                                    .ToArray();

                _validacionCosteoManualExcepcion = (from regla in reglas
                                                    join irEntry in inventoryReasonEntryList
                                                    on regla.EntryInventoryReasonCode equals irEntry.code
                                                    join irExit in inventoryReasonExitList
                                                    on regla.ExitInventoryReasonCode equals irExit.code
                                                    select new CosteoValidacionesExcepcionDto
                                                    {
                                                        CodeBehavior = regla.CodeBehavior,
                                                        CompanyId = regla.CompanyId,
                                                        DateInit = regla.DateInit,
                                                        DateEnd = regla.DateEnd,
                                                        EntryInventoryReasonCode = regla.EntryInventoryReasonCode,
                                                        EntryInventoryReasonId = irEntry.id,
                                                        ExitInventoryReasonCode = regla.ExitInventoryReasonCode,
                                                        ExitInventoryReasonId = irExit.id,
                                                    }).ToArray();



            }

        }

        #region TEST CODE 
        private void getInventoryMoveDetailBulk(int[] inventoryMoveDetailIds)
        {
            try
            {
                var inventoryMoveDetailList = db.InventoryMoveDetail.Where(r => inventoryMoveDetailIds.Contains(r.id)).ToArray();
                _inventoryMoveDetailTest.AddRange(inventoryMoveDetailList);
            }
            catch (Exception e)
            {
                var g = e;
            }


        }

        private void getCostoMovimientoList()
        {

            const string m_sql = "Select * from TMP_costo_saldo_movimiento;";
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            _costosMovimientosTest = null;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();
                var result = cnn.Query<TestCostoSaldoMovimiento>(m_sql);
                _costosMovimientosTest = result.ToArray();
                cnn.Close();
            }
        }
        #endregion

        #region Parametrizacion Metodo Valorizacion
        private string getParametroMetodoValorizacion()
        {
            var key = $"AdvanceParameters_{m_VariableParametroMetodoValorizacionKey}";

            if (TempData.ContainsKey(key))
            {
                return TempData[key] as string;
            }
            else
            {
                var parametroValorizacion = db.AdvanceParameters.FirstOrDefault(e => e.code.Equals(m_VariableParametroMetodoValorizacionKey));
                if (parametroValorizacion == null)
                {
                    throw new Exception($"No se ha encontrado parametro de sistema para el Metodo de Valorizacion. {m_VariableParametroMetodoValorizacionKey}");
                }
                else
                {
                    TempData[key] = parametroValorizacion.valueVarchar;
                    TempData.Keep(key);
                }

                return parametroValorizacion.valueVarchar;
            }
        }

        private void AsignarCostoHeredadoMetodoProceso(DataProcesarInventario datoProcesar,
            ProcesamientoValorizacion parametroCalculo, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades, bool generarCostoPromedio,
            ref int ordenAsigCost, int? indiceReproceso)
        {
            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            var inventoryReason = this.GetInventoryReasonFromTempData(datoProcesar.IdInventoryReason);
            if (inventoryReason.id_inventoryReasonRelated.HasValue)
            {
                var parametroCalculoHeredado = this.GetParametroCalculo(inventoryReason.id_inventoryReasonRelated.Value);
                datoProcesar.IdInventoryReason = inventoryReason.id_inventoryReasonRelated.Value;

                int? id_detalleSalida = null;
                var inventoryMoveDetailExit = db.InventoryMoveDetailTransfer
                                                        .FirstOrDefault(e => e.id_inventoryMoveDetailEntry == datoProcesar.IdInventoryMoveDetail)?
                                                        .InventoryMoveDetail;

                // Verificamos la transferencia normal
                if (inventoryMoveDetailExit != null)
                {
                    id_detalleSalida = inventoryMoveDetailExit.id;
                }
                else
                {
                    // Verificamos si es una transferencia automática
                    id_detalleSalida = this.GetDetalleMovimientoSalidaTransferenciaAutomatica(datoProcesar.IdInventoryMoveDetail);

                    if (!id_detalleSalida.HasValue)
                    {
                        // validar si la relacion esta en DocumentSource
                        var documentSourcesOriginIds = db.DocumentSource
                                                                .Where(r => r.id_document == inventoryMoveDetail.id_inventoryMove)
                                                                .Select(r => r.id_documentOrigin)
                                                                .ToArray();
                        // obtener informacion motivo Egreso 

                        var inventoryReasonEgreso = this.GetInventoryReasonFromTempData((inventoryReason.id_inventoryReasonRelated ?? 0));

                        if (inventoryReasonEgreso != null)
                        {
                            int documentTypeEgresoId = inventoryReasonEgreso.id_documentType;
                            int? documentoEgresoId = db.Document
                                                        .FirstOrDefault(r => documentSourcesOriginIds.Contains(r.id)
                                                                             && r.id_documentType == documentTypeEgresoId)?.id;

                            if (documentoEgresoId.HasValue)
                            {
                                id_detalleSalida = db.InventoryMoveDetail
                                                            .FirstOrDefault(r => r.id_inventoryMove == documentoEgresoId.Value
                                                                                 && r.id_warehouseEntry == inventoryMoveDetail.id_warehouse
                                                                                 && r.id_warehouseLocationEntry == inventoryMoveDetail.id_warehouseLocation
                                                                                 && r.id_lot == inventoryMoveDetail.id_lot
                                                                                 && r.id_item == inventoryMoveDetail.id_item
                                                                                 && r.id_productionCart == inventoryMoveDetail.id_productionCart
                                                                                 && r.exitAmount == inventoryMoveDetail.entryAmount
                                                                                 )?.id;

                            }

                        }

                        if (!id_detalleSalida.HasValue)
                        {
                            var item = this.GetItemFromTempData(datoProcesar.IdItem);
                            var bodega = this.GetWarehouseFromTempData(datoProcesar.IdWarehouse);
                            var fechaMovimiento = inventoryMoveDetail != null
                                ? inventoryMoveDetail.InventoryMove.Document.emissionDate.ToString("dd/MM/yyyy")
                                : string.Empty;

                            novedades.Add(new ProductionCostProductValuationWarning()
                            {
                                idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                                orden = novedades.Count + 1,
                                descripcion = "Error al recuperar el detalle del movimiento de inventario de EGRESO. " +
                                    $"Bodega {bodega.name}. Ítem: [{item.masterCode}] - {item.name}. Fecha movimiento: {fechaMovimiento}.",
                                activo = true,

                                idUsuarioCreacion = this.ActiveUserId,
                                fechaHoraCreacion = DateTime.Now,
                                idUsuarioModificacion = this.ActiveUserId,
                                fechaHoraModificacion = DateTime.Now,
                            });
                        }

                    }
                }

                var costoTotal = productionsCostProductValuation
                    .FirstOrDefault(e => e.idInventoryMoveDetail == id_detalleSalida)?
                    .costoTotal;

                // Validamos si no encuentra el costo total de salida
                if (!costoTotal.HasValue && costoTotal <= 0)
                {
                    this.SetErrorCostoCero(datoProcesar, novedades);
                }

                var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
                var precioUnitario = cantidadMovimiento != 0m
                    ? Math.Abs(GlobalCalculator.RedondearMontoTotalPrecision((costoTotal ?? 0m) / cantidadMovimiento))
                    : 0m;

                // Ajustamos el saldo del producto
                var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                            datoProcesar.IdWarehouse,
                                                            datoProcesar.IdItem,
                                                            cantidadMovimiento,
                                                            costoTotal ?? 0m,
                                                            datoProcesar.PlantaProcesoId);

                // El saldo quedaría en negativo, por lo que no se debe procesar
                if (!asignarCosto) costoTotal = precioUnitario = 0m;
                costoTotal = Math.Abs(costoTotal ?? 0m);
                if (costoTotal > 0)
                {
                    ordenAsigCost++;
                    this.RemoverErrores(datoProcesar.IdInventoryMoveDetail, novedades);
                }

                if (indiceReproceso.HasValue)
                {
                    productionsCostProductValuation[indiceReproceso.Value].ordenAsigCost = ordenAsigCost;
                    productionsCostProductValuation[indiceReproceso.Value].costoTotal = costoTotal ?? 0m;
                    productionsCostProductValuation[indiceReproceso.Value].costoUnitario = precioUnitario;
                }
                else
                {
                    productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                    {
                        ordenAsigCost = ordenAsigCost,
                        orden = productionsCostProductValuation.Count + 1,
                        idInventoryMoveDetail = inventoryMoveDetail.id,
                        idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                        idBodega = inventoryMoveDetail.id_warehouse,
                        idItem = datoProcesar.IdItem,
                        idMotivo = datoProcesar.IdInventoryReason,
                        esTransferencia = parametroCalculo.EsTransferencia,
                        naturaleza = parametroCalculo.Naturaleza,
                        tipoCalculo = parametroCalculo.TipoCalculo,
                        accion = m_AsignarCostoEgresoTransKey,
                        id_lot = inventoryMoveDetail.id_lot ?? 0,
                        valorizacion = parametroCalculo.Valorizacion,
                        costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                        costoTotalAnterior = cantidadMovimiento * inventoryMoveDetail.unitPrice,

                        cantidad = cantidadMovimiento,
                        costoTotal = costoTotal ?? 0m,
                        costoUnitario = precioUnitario,
                        activo = true,

                        idUsuarioCreacion = this.ActiveUserId,
                        fechaHoraCreacion = DateTime.Now,
                        idUsuarioModificacion = this.ActiveUserId,
                        fechaHoraModificacion = DateTime.Now,
                    });
                }
            }
            else
            {
                this.AsignarCostoPromedio(datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation, novedades,
                    ref ordenAsigCost, indiceReproceso);
            }
        }

        private void AsignarCostoHeredadoMetodoLote(DataProcesarInventario datoProcesar,
            ProcesamientoValorizacion parametroCalculo, ref List<CostoProcesar> saldos,
            ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
            List<ProductionCostProductValuationWarning> novedades, bool generarCostoPromedio,
            ref int ordenAsigCost, int? indiceReproceso)
        {
            var inventoryMoveDetail = this.GetInventoryMoveDetailFromTempData(datoProcesar.IdInventoryMoveDetail);
            var inventoryReason = this.GetInventoryReasonFromTempData(datoProcesar.IdInventoryReason);
            if (inventoryReason.id_inventoryReasonRelated.HasValue)
            {
                var parametroCalculoHeredado = this.GetParametroCalculo(inventoryReason.id_inventoryReasonRelated.Value);
                datoProcesar.IdInventoryReason = inventoryReason.id_inventoryReasonRelated.Value;

                int? id_detalleSalida = null;
                var inventoryMoveDetailExit = db.InventoryMoveDetailTransfer
                    .FirstOrDefault(e => e.id_inventoryMoveDetailEntry == datoProcesar.IdInventoryMoveDetail)?
                    .InventoryMoveDetail;

                // Verificamos la transferencia normal
                if (inventoryMoveDetailExit != null)
                {
                    id_detalleSalida = inventoryMoveDetailExit.id;
                }
                else
                {
                    // Verificamos si es una transferencia automática
                    id_detalleSalida = this.GetDetalleMovimientoSalidaTransferenciaAutomatica(datoProcesar.IdInventoryMoveDetail);
                    if (!id_detalleSalida.HasValue)
                    {
                        var item = this.GetItemFromTempData(datoProcesar.IdItem);
                        var bodega = this.GetWarehouseFromTempData(datoProcesar.IdWarehouse);
                        var fechaMovimiento = inventoryMoveDetail != null
                            ? inventoryMoveDetail.InventoryMove.Document.emissionDate.ToString("dd/MM/yyyy")
                            : string.Empty;

                        novedades.Add(new ProductionCostProductValuationWarning()
                        {
                            idInventoryMoveDetail = datoProcesar.IdInventoryMoveDetail,
                            orden = novedades.Count + 1,
                            descripcion = "Error al recuperar el detalle del movimiento de inventario de EGRESO. " +
                                $"Bodega {bodega.name}. Ítem: [{item.masterCode}] - {item.name}. Fecha movimiento: {fechaMovimiento}.",
                            activo = true,

                            idUsuarioCreacion = this.ActiveUserId,
                            fechaHoraCreacion = DateTime.Now,
                            idUsuarioModificacion = this.ActiveUserId,
                            fechaHoraModificacion = DateTime.Now,
                        });
                    }
                }

                var costoTotal = productionsCostProductValuation
                    .FirstOrDefault(e => e.idInventoryMoveDetail == id_detalleSalida)?
                    .costoTotal;

                // Validamos si no encuentra el costo total de salida
                if (!costoTotal.HasValue && costoTotal <= 0)
                {
                    this.SetErrorCostoCero(datoProcesar, novedades);
                }

                var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
                var precioUnitario = cantidadMovimiento != 0m
                    ? Math.Abs(GlobalCalculator.RedondearMontoTotalPrecision((costoTotal ?? 0m) / cantidadMovimiento))
                    : 0m;

                // Ajustamos el saldo del producto
                var asignarCosto = this.CalcularSaldoFinal(ref saldos,
                                                            datoProcesar.IdWarehouse,
                                                            datoProcesar.IdItem,
                                                            cantidadMovimiento,
                                                            costoTotal ?? 0m,
                                                            datoProcesar.PlantaProcesoId);

                // El saldo quedaría en negativo, por lo que no se debe procesar
                if (!asignarCosto) costoTotal = precioUnitario = 0m;
                costoTotal = Math.Abs(costoTotal ?? 0m);
                if (costoTotal > 0)
                {
                    ordenAsigCost++;
                    this.RemoverErrores(datoProcesar.IdInventoryMoveDetail, novedades);
                }

                if (indiceReproceso.HasValue)
                {
                    productionsCostProductValuation[indiceReproceso.Value].ordenAsigCost = ordenAsigCost;
                    productionsCostProductValuation[indiceReproceso.Value].costoTotal = costoTotal ?? 0m;
                    productionsCostProductValuation[indiceReproceso.Value].costoUnitario = precioUnitario;
                }
                else
                {
                    productionsCostProductValuation.Add(new ProductionCostProductValuationInventoryMove()
                    {
                        ordenAsigCost = ordenAsigCost,
                        orden = productionsCostProductValuation.Count + 1,
                        idInventoryMoveDetail = inventoryMoveDetail.id,
                        idInventoryMove = inventoryMoveDetail.id_inventoryMove,
                        idBodega = inventoryMoveDetail.id_warehouse,
                        idItem = datoProcesar.IdItem,
                        idMotivo = datoProcesar.IdInventoryReason,
                        esTransferencia = parametroCalculo.EsTransferencia,
                        naturaleza = parametroCalculo.Naturaleza,
                        tipoCalculo = parametroCalculo.TipoCalculo,
                        accion = m_AsignarCostoEgresoTransKey,
                        id_lot = inventoryMoveDetail.id_lot ?? 0,
                        valorizacion = parametroCalculo.Valorizacion,
                        costoUnitarioAnterior = inventoryMoveDetail.unitPrice,
                        costoTotalAnterior = cantidadMovimiento * inventoryMoveDetail.unitPrice,

                        cantidad = cantidadMovimiento,
                        costoTotal = costoTotal ?? 0m,
                        costoUnitario = precioUnitario,
                        activo = true,

                        idUsuarioCreacion = this.ActiveUserId,
                        fechaHoraCreacion = DateTime.Now,
                        idUsuarioModificacion = this.ActiveUserId,
                        fechaHoraModificacion = DateTime.Now,
                    });
                }
            }
            else
            {
                this.AsignarCostoPromedio(datoProcesar, parametroCalculo,
                    ref saldos, ref productionsCostProductValuation, novedades,
                    ref ordenAsigCost, indiceReproceso);
            }
        }

        private int? RecuperarCostoMovimientoProcesoMetodoProceso(InventoryMoveDetail inventoryMoveDetail,
                                                                    DataProcesarInventario datoProcesar,
                                                                    ref List<CostoProcesar> saldos,
                                                                    ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
                                                                    List<ProductionCostProductValuationWarning> novedades,
                                                                    ProcesamientoValorizacion parametroCalculo,
                                                                    List<DataProcesarInventario> dataOrdenamiento,
                                                                    out decimal? factor,
                                                                    out decimal? costoUnitarioInventario,
                                                                    out bool? isResultingProduct)
        {
            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
            // recuperamos el costo de los detalles procesados anteriormente
            var id_inventoryMove = inventoryMoveDetail.id_inventoryMove;
            var idItem = inventoryMoveDetail.id_item;
            var id_lot = inventoryMoveDetail.id_lot;

            var movimientosLote = productionsCostProductValuation
                                        .Where(e => e.idItem == idItem && e.id_lot == id_lot)
                                        .Where(e => e.costoUnitario != 0m); // donde su costo sea mayor a cero;

            // Si encontramos un movimiento con la misma bodega se filtra a esa bodega.
            var movimientosBodegaLote = movimientosLote
                .Select(e => new
                {
                    e.idInventoryMoveDetail,
                    id_bodega = e.idBodega,
                    e.idItem,
                    e.id_lot,
                    e.idMotivo
                })
                .ToArray();

            var inventoryReason = GetInventoryReasonFromTempData(datoProcesar.IdInventoryReason);
            var bodegaProcesar = GetWarehouseFromTempData(datoProcesar.IdWarehouse);
            // ANALIZAR, CONFIGURAR EN MOTIVO DE INVENTARIO
            if (movimientosBodegaLote.Any(e => e.id_bodega == datoProcesar.IdWarehouse))
            {
                movimientosBodegaLote = movimientosBodegaLote
                    .Where(e => e.id_bodega == datoProcesar.IdWarehouse)
                    .ToArray();
            }

            var idsInventoryMoveDetail = movimientosBodegaLote
                                                .Select(e => e.idInventoryMoveDetail)
                                                .ToArray();

            // Si encontramos detalles con ese producto, recuperamos el último detalle 
            factor = costoUnitarioInventario = null;
            isResultingProduct = false;

            // Absorcion Costo: Forzar +1 pasada Material resuiltando o Motivo ILP
            var codeProductionProcessExclude = new[] { "REC", "RMM" };

            var productionLot = db.ProductionLot
                .FirstOrDefault(e => e.id == inventoryMoveDetail.id_lot &&
                    !codeProductionProcessExclude.Contains(e.ProductionProcess.code) && e.ProductionLotState.code != "09");

            var productionProcess = productionLot?
                .ProductionProcess ?? new ProductionProcess();

            string[] specialTipoCalculoMasterizado = new string[] { m_TipoCalculoAbsorcionPTKey, m_TipoCalculoAbsorcionSBKey };

            //if (datoProcesar.IsResultingProduct || inventoryReason.code == "ILP")
            if ((productionProcess.generatesAbsorption ?? false) && !specialTipoCalculoMasterizado.Contains(parametroCalculo.TipoCalculo))
            {
                var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
                this.RecuperarCostoLoteMateriaPrima(datoProcesar,
                    ref saldos, productionsCostProductValuation, novedades, inventoryMoveDetail.id_lot,
                    cantidadMovimiento, out var precioUnitario, out _, out isResultingProduct);
                costoUnitarioInventario = precioUnitario;

                return null;
            }

            // Masterizado: Forzar obtener masterizado Cajas resultantes y Sobrante  
            if (idsInventoryMoveDetail.Any() && !specialTipoCalculoMasterizado.Contains(parametroCalculo.TipoCalculo))
            {
                var detallesMovimientoLote = new List<InventoryMoveDetail>();
                foreach (var idInventoryMoveDetail in idsInventoryMoveDetail)
                {
                    var detalleInventarioProcesar = this.GetInventoryMoveDetailFromTempData(idInventoryMoveDetail);

                    detallesMovimientoLote.Add(detalleInventarioProcesar);
                }

                if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso))
                {
                    var movimientosordenados = (from movlote in detallesMovimientoLote
                                                join orden in dataOrdenamiento
                                                on movlote.id equals orden.IdInventoryMoveDetail
                                                //orderby new { movlote.InventoryMove.Document.emissionDate, orden.OrdenMotivoTipoDocum, orden.OrdenTransferencias }
                                                orderby movlote.InventoryMove.Document.emissionDate, orden.OrdenMotivoTipoDocum, orden.OrdenTransferencias
                                                select new
                                                {
                                                    movlote.id,
                                                    movlote.InventoryMove.Document.emissionDate,
                                                    orden.OrdenMotivoTipoDocum,
                                                    orden.OrdenTransferencias
                                                }).ToList();

                    return movimientosordenados.FirstOrDefault()?.id;
                }
                else if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
                {
                    return detallesMovimientoLote
                                    .OrderBy(e => e.InventoryMove.Document.emissionDate)
                                    .ThenBy(e => e.dateCreate)
                                    .LastOrDefault()?
                                    .id;
                }

            }
            else
            {
                // Sobrante
                //if (inventoryReason.code == "IPACS" )
                if (parametroCalculo.TipoCalculo == m_TipoCalculoAbsorcionSBKey)
                {
                    var idMasterizadoSobrante = this.GetInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot, "SOB");
                    if (idMasterizadoSobrante.HasValue)
                    {
                        return idMasterizadoSobrante;
                    }
                    else
                    {
                        throw new Exception("Sin mov materia prima para movimiento sobrante");
                    }
                }
                // masterizado
                if (parametroCalculo.TipoCalculo == m_TipoCalculoAbsorcionPTKey)
                {
                    var idMasterizado = this.GetInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot);
                    if (idMasterizado.HasValue)
                    {
                        return idMasterizado;
                    }
                    else
                    {
                        throw new Exception("Sin mov materia prima para movimiento masterizado");
                    }

                }
                else
                {
                    var idMasterizado = this.GetInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot);
                    if (idMasterizado.HasValue)
                    {
                        factor = this.GetFactorInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot);
                        return idMasterizado;
                    }
                    else
                    {
                        var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
                        this.RecuperarCostoLoteMateriaPrima(datoProcesar,
                            ref saldos, productionsCostProductValuation, novedades, inventoryMoveDetail.id_lot,
                            cantidadMovimiento, out var precioUnitario, out _, out isResultingProduct);
                        costoUnitarioInventario = precioUnitario;
                    }
                }

            }

            return null;
        }
        private int? RecuperarCostoMovimientoProcesoMetodoLote(InventoryMoveDetail inventoryMoveDetail,
                                                                  DataProcesarInventario datoProcesar,
                                                                  ref List<CostoProcesar> saldos,
                                                                  ref List<ProductionCostProductValuationInventoryMove> productionsCostProductValuation,
                                                                  List<ProductionCostProductValuationWarning> novedades,
                                                                  out decimal? factor,
                                                                  out decimal? costoUnitarioInventario)
        {
            // recuperamos el costo de los detalles procesados anteriormente
            var id_inventoryMove = inventoryMoveDetail.id_inventoryMove;
            var idItem = inventoryMoveDetail.id_item;
            var id_lot = inventoryMoveDetail.id_lot;

            var movimientosLote = productionsCostProductValuation
                .Where(e => e.idItem == idItem && e.id_lot == id_lot)
                .Where(e => e.costoUnitario != 0m); // donde su costo sea mayor a cero;

            // Si encontramos un movimiento con la misma bodega se filtra a esa bodega.
            var movimientosBodegaLote = movimientosLote
                .Select(e => new
                {
                    e.idInventoryMoveDetail,
                    id_bodega = e.idBodega,
                    e.idItem,
                    e.id_lot,
                })
                .ToArray();

            if (movimientosBodegaLote.Any(e => e.id_bodega == datoProcesar.IdWarehouse))
            {
                movimientosBodegaLote = movimientosBodegaLote
                    .Where(e => e.id_bodega == datoProcesar.IdWarehouse)
                    .ToArray();
            }

            var idsInventoryMoveDetail = movimientosBodegaLote
                .Select(e => e.idInventoryMoveDetail)
                .ToArray();

            // Si encontramos detalles con ese producto, recuperamos el último detalle 
            factor = costoUnitarioInventario = null;
            if (idsInventoryMoveDetail.Any())
            {
                var detallesMovimientoLote = new List<InventoryMoveDetail>();
                foreach (var idInventoryMoveDetail in idsInventoryMoveDetail)
                {
                    var detalleInventarioProcesar = this.GetInventoryMoveDetailFromTempData(idInventoryMoveDetail);

                    detallesMovimientoLote.Add(detalleInventarioProcesar);
                }

                return detallesMovimientoLote
                    .OrderBy(e => e.InventoryMove.Document.emissionDate)
                    .ThenBy(e => e.dateCreate)
                    .LastOrDefault()?
                    .id;
            }
            else
            {
                var idMasterizado = this.GetInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot);
                if (idMasterizado.HasValue)
                {
                    factor = this.GetFactorInventoryMoveDetailMasterizado(id_inventoryMove, idItem, id_lot);
                    return idMasterizado;
                }
                else
                {
                    var cantidadMovimiento = inventoryMoveDetail.entryAmount - inventoryMoveDetail.exitAmount;
                    this.RecuperarCostoLoteMateriaPrima(datoProcesar,
                                                            ref saldos,
                                                            productionsCostProductValuation,
                                                            novedades,
                                                            inventoryMoveDetail.id_lot,
                                                            cantidadMovimiento,
                                                            out var precioUnitario,
                                                            out _,
                                                            out _);
                    costoUnitarioInventario = precioUnitario;
                }
            }

            return null;
        }

        public FileContentResult generateExcelDynamicMetodoLote()
        {

            // Recuperamos y asignamos la valorización
            var productValuationTemp = (this.TempData[m_CostProductValuationExcelModelKey] as ProductionCostProductValuation);
            var detallesValorizacion = this.ConvertToDTO(productValuationTemp.ProductionCostProductValuationInventoryMove.ToArray());

            // Recuperamos los saldos iniciales
            var fechaSaldo = new DateTime(productValuationTemp.anio, productValuationTemp.mes, 1).AddDays(-1);
            var fechaInicio = (productValuationTemp.fechaInicio ?? fechaSaldo).AddDays(-1);
            var saldosInicialesDTO = this.GetSaldosInventoryMoves(fechaInicio, productValuationTemp.id_allocationType);
            var SaldosDiaBodega = this.CalcularSaldosDiaBodega(
                productValuationTemp.fechaInicio.Value, productValuationTemp.fechaFin.Value, productValuationTemp.id_allocationType);

            var detalles = saldosInicialesDTO
                .Union(detallesValorizacion)
                .ToArray();
            var resumen = this.GenerateResumen(detalles);

            byte[] pdfBuffer;
            using (var outputStream = new System.IO.MemoryStream())
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage())
                {

                    // Agregar hojas al documento de Excel
                    ExcelWorksheet ws_resumen = pck.Workbook.Worksheets.Add("Resumen Valorización");
                    ExcelWorksheet ws_detalle = pck.Workbook.Worksheets.Add("Tabla Dinámica");
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Movimientos");
                    ExcelWorksheet ws_saldos = pck.Workbook.Worksheets.Add("Saldo por Día");
                    ExcelWorksheet ws_kardex = pck.Workbook.Worksheets.Add("Kardex Valorizado");

                    // Agregar datos a las hojas
                    #region Tabla Resumen
                    // Hoja Resumen de resultados totales

                    ws_resumen.Cells["A1"].Value = this.ActiveUser.Company.businessName; // "PROEXPO";
                    ws_resumen.Cells["A1:C1"].Merge = true;
                    ws_resumen.Cells["A1:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws_resumen.Cells["A1:C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A1:C2"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    ws_resumen.Cells["A1:C2"].Style.Font.Bold = true;
                    ws_resumen.Cells["A3"].Style.Font.Bold = true;

                    ws_resumen.Cells["A2"].Value = "CUADRATURA DE LIBRAS PANACEA";
                    ws_resumen.Cells["A2:B2"].Merge = true;

                    ws_resumen.Cells["A3"].Value = "Fecha Proceso: ";
                    ws_resumen.Cells["B3"].Value = DateTime.Now.ToDateTimeFormat();
                    ws_resumen.Cells["B3:C3"].Merge = true;

                    ws_resumen.Cells["A4"].Value = "SEGÚN AUDITORÍA";
                    ws_resumen.Cells["A4:C4"].Merge = true;
                    ws_resumen.Cells["A4:C4"].Style.Font.Bold = true;
                    ws_resumen.Cells["A4:C4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws_resumen.Cells["A4:C4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A4:C4"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                    ws_resumen.Cells["A5"].Value = "BODEGAS";
                    ws_resumen.Cells["B5"].Value = "LIBRAS";
                    ws_resumen.Cells["C5"].Value = "DÓLARES";
                    ws_resumen.Cells["A5:C5"].Style.Font.Bold = true;
                    ws_resumen.Cells["A5:C5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A5:C5"].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);


                    int numInicio = 6;
                    for (int i = 0; i < resumen.Length; i++)
                    {
                        numInicio++;
                        var criterio = resumen[i];
                        ws_resumen.Cells[$"A{numInicio}"].Value = criterio.Titulo;

                        if (criterio.EsTitulo)
                        {
                            ws_resumen.Cells[$"A{numInicio}"].Style.Font.Bold = true;
                            ws_resumen.Cells[$"A{numInicio}:C{numInicio}"].Merge = true;

                        }
                        else
                        {
                            ws_resumen.Cells[$"B{numInicio}"].Value = criterio.Peso;
                            ws_resumen.Cells[$"C{numInicio}"].Value = criterio.Dolares;
                            ws_resumen.Cells[$"B{numInicio}:C{numInicio}"].Style.Numberformat.Format = "#,##0";
                        }

                    }

                    ws_resumen.Cells["A16:C16"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A16:C16"].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);

                    ws_resumen.Cells["A21:C21"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A21:C21"].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);


                    ws_resumen.Cells.AutoFitColumns();

                    #endregion

                    #region Tabla resultado

                    ws.Cells["A1"].LoadFromCollection(Collection: detalles, PrintHeaders: true);

                    // Eliminar columnas Ordenamientos /fecha orden
                    int[] columnsToDeleteResultado = { 29,6,5 };
                    foreach (var col in columnsToDeleteResultado)
                    {
                        ws.DeleteColumn(col);
                    }
                    //create a range for the table
                    var range = ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column];

                    //add a table to the range
                    var tab = ws.Tables.Add(range, "Movimientos");

                    //format the table
                    tab.TableStyle = TableStyles.Light1;
                    ws.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Row(1).Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    ws.Cells.AutoFitColumns();

                    #endregion

                    #region Tabla Dinamica (Detalles)
                    var dataRange = ws.Cells[ws.Dimension.Address];

                    // Crear la tabla dinámica
                    var pivotTable = ws_detalle.PivotTables.Add(ws_detalle.Cells["A3"], dataRange, "Tabla");

                    // Filtros
                    ExcelPivotTableField categoriaPageField = pivotTable.PageFields.Add(
                        pivotTable.Fields["Categoría de Motivos"]
                    );
                    categoriaPageField.Sort = eSortType.Ascending;
                    ws_detalle.View.FreezePanes(5, 1);

                    // Campos descriptivos
                    pivotTable.RowFields.Add(pivotTable.Fields["Bodega"]);
                    pivotTable.RowFields.Add(pivotTable.Fields["Motivo"]);
                    pivotTable.RowFields.Add(pivotTable.Fields["Ítem"]);
                    pivotTable.DataOnRows = false;

                    // Campos de datos
                    ExcelPivotTableDataField cantidadField = pivotTable.DataFields.Add(
                        pivotTable.Fields["Cantidad Libras"]
                    );
                    cantidadField.Function = DataFieldFunctions.Sum;
                    cantidadField.Name = "Cantidad Libras";
                    cantidadField.Format = "#,##0";

                    ExcelPivotTableDataField costoLibrasField = pivotTable.DataFields.Add(
                        pivotTable.Fields["Costo Total Libras"]
                    );
                    costoLibrasField.Function = DataFieldFunctions.Sum;
                    costoLibrasField.Name = "Costo Total Libras";
                    costoLibrasField.Format = "$#,##0";

                    #endregion

                    #region Tabla Saldos por días bodega

                    ws_saldos.Cells["A1"].LoadFromCollection(Collection: SaldosDiaBodega, PrintHeaders: true);
                    ws_saldos.Cells.AutoFitColumns();
                    var range_saldos = ws_saldos.Cells[1, 1, ws_saldos.Dimension.End.Row, ws_saldos.Dimension.End.Column];
                    var tab_saldos = ws_saldos.Tables.Add(range_saldos, "Saldos");
                    ws_saldos.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_saldos.Row(1).Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    tab_saldos.TableStyle = TableStyles.Light1;
                    #endregion

                    #region Tabla Kardex Valorizado

                    var detalles_ordenados = detalles.OrderBy(d => d.CodigoNombreItem)
                        .ThenBy(d => d.NombreBodega)
                        .ThenBy(d => d.OrdenCosteo);

                    ws_kardex.Cells["A1"].LoadFromCollection(Collection: detalles_ordenados, PrintHeaders: true);

                    int[] columnsToDelete = { 29, 28, 27, 25, 24, 21, 19, 17, 11, 10,6,5, 2 };
                    //{ 1, 6, 6, 11, 12, 13, 15, 15, 16, 16 };

                    foreach (var col in columnsToDelete)
                    {
                        ws_kardex.DeleteColumn(col);
                    }

                    // Renombrar columnas
                    ws_kardex.Cells["A1"].Value = "Secuencia Costo";
                    ws_kardex.Cells["B1"].Value = "Id Mov. Inv.";
                    ws_kardex.Cells["M1"].Value = "Cantidad Unidades";
                    ws_kardex.Cells["N1"].Value = "Costo Total";

                    // Agregar nuevas columnas
                    this.AddFormulaToColumn(ws_kardex, "QUIEBRE", "IF(CONCATENATE(D2, E2) = CONCATENATE(D1, E1), \"\", \"X\")", 17);
                    this.AddFormulaToColumn(ws_kardex, "LIB.INICIAL", "IF(Q2=\"X\", 0, U1)", 18);
                    this.AddFormulaToColumn(ws_kardex, "LIB.INGRESOS", "IF(O2 > 0, O2, 0)", 19);
                    this.AddFormulaToColumn(ws_kardex, "LIB.EGRESOS", "IF(O2 < 0, O2, 0)", 20);
                    this.AddFormulaToColumn(ws_kardex, "LIB.FINAL", "IF(SUM(R2, S2, T2) < 0.0001, 0, SUM(R2, S2, T2))", 21);
                    this.AddFormulaToColumn(ws_kardex, "DOL.INICIAL", "IF(Q2=\"X\", 0, Y1)", 22);
                    this.AddFormulaToColumn(ws_kardex, "DOL.INGRESOS", "IF(N2 > 0, N2, 0)", 23);
                    this.AddFormulaToColumn(ws_kardex, "DOL.EGRESOS", "IF(N2 < 0, N2, 0)", 24);
                    this.AddFormulaToColumn(ws_kardex, "DOL.FINAL", "SUM(V2, W2, X2)", 25);
                    var range_calcs = ws_kardex.Cells[2, 17, ws_kardex.Dimension.End.Row, 25];
                    range_calcs.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range_calcs.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(153, 204, 255));
                    var range_calcs_header = ws_kardex.Cells[1, 17, 1, 25];
                    range_calcs_header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range_calcs_header.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 51, 102));

                    this.AddFormulaToColumn(ws_kardex, "Ingreso.Libras", "S2", 26);
                    this.AddFormulaToColumn(ws_kardex, "Ingreso.PU.Libras", "IF(S2 = 0, 0, W2/S2)", 27);
                    this.AddFormulaToColumn(ws_kardex, "Ingreso.CosTotal", "W2", 28);
                    this.AddFormulaToColumn(ws_kardex, "Egreso.Libras", "T2", 29);
                    this.AddFormulaToColumn(ws_kardex, "Egreso.PU.Libras", "IF(T2 = 0, 0, X2/T2)", 30);
                    this.AddFormulaToColumn(ws_kardex, "Egreso.CosTotal", "X2", 31);
                    this.AddFormulaToColumn(ws_kardex, "Saldo.Libras", "U2", 32);
                    this.AddFormulaToColumn(ws_kardex, "Saldo.PU.Libras", "IF(U2 = 0, 0, Y2/U2)", 33);
                    this.AddFormulaToColumn(ws_kardex, "Saldo.CosTotal", "Y2", 34);
                    var range_calcs2 = ws_kardex.Cells[2, 26, ws_kardex.Dimension.End.Row, 34];
                    range_calcs2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range_calcs2.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 255, 204));
                    var range_calcs2_header = ws_kardex.Cells[1, 26, 1, 34];
                    range_calcs2_header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range_calcs2_header.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 51, 0));


                    var range_kardex = ws_kardex.Cells[1, 1, ws_kardex.Dimension.End.Row, ws_kardex.Dimension.End.Column];
                    var tab_kardex = ws_kardex.Tables.Add(range_kardex, "Kardex");

                    ws_kardex.Cells.AutoFitColumns();
                    #endregion

                    pck.SaveAs(outputStream);
                }

                pdfBuffer = outputStream.ToArray();
            }

            var fechaInicioVal = productValuationTemp.fechaInicio;
            var fechaFinVal = productValuationTemp.fechaFin;
            return new FileContentResult(pdfBuffer, ExcelXlsxMime)
            {
                FileDownloadName = $"Valorización Productos. Rango: {fechaInicioVal.ToDateFormat()} - {fechaFinVal.ToDateFormat()}." +
                    $"Generado: {DateTime.Now.ToDateTimeFormat()}.xlsx",
            };


        }

        public FileContentResult generateExcelDynamicMetodoProceso()
        {

            this.TempData.Keep(m_CostOrderSpecialList);
            // Recuperamos y asignamos la valorización
            var productValuationTemp = (this.TempData[m_CostProductValuationExcelModelKey] as ProductionCostProductValuation);
            var detallesValorizacion = this.ConvertToDTO(productValuationTemp.ProductionCostProductValuationInventoryMove.ToArray());

            // Recuperamos los saldos iniciales
            var fechaSaldo = new DateTime(productValuationTemp.anio, productValuationTemp.mes, 1).AddDays(-1);
            var fechaInicio = (productValuationTemp.fechaInicio ?? fechaSaldo).AddDays(-1);
            var saldosInicialesDTO = this.GetSaldosInventoryMoves(fechaInicio, productValuationTemp.id_allocationType);
            var SaldosDiaBodega = this.CalcularSaldosDiaBodega(
                productValuationTemp.fechaInicio.Value, productValuationTemp.fechaFin.Value, productValuationTemp.id_allocationType);

            int ordenNuevo = 0;
            var detalles = saldosInicialesDTO
                                .Union(detallesValorizacion)
                                .ToArray();
            detalles = detalles
                             .OrderByDescending(r => r.ProcesoPlanta)
                             .ThenBy(r => r.FechaOrden)
                             .ThenBy(r => r.NumOrden)
                             .ThenBy(r => r.OrdenTransferencia)
                             .ToArray();

            detalles.ForEach(r =>
            {

                r.Orden = ordenNuevo++;
            });

            var resumen = this.GenerateResumen(detalles);

            byte[] pdfBuffer;
            using (var outputStream = new System.IO.MemoryStream())
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage())
                {

                    // Agregar hojas al documento de Excel
                    ExcelWorksheet ws_resumen = pck.Workbook.Worksheets.Add("Resumen Valorización");
                    ExcelWorksheet ws_detalle = pck.Workbook.Worksheets.Add("Tabla Dinámica");
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Movimientos");
                    ExcelWorksheet ws_saldos = pck.Workbook.Worksheets.Add("Saldo por Día");
                    ExcelWorksheet ws_kardex = pck.Workbook.Worksheets.Add("Kardex Valorizado");

                    // Agregar datos a las hojas
                    #region Tabla Resumen
                    // Hoja Resumen de resultados totales

                    ws_resumen.Cells["A1"].Value = this.ActiveUser.Company.businessName;
                    ws_resumen.Cells["A1:C1"].Merge = true;
                    ws_resumen.Cells["A1:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws_resumen.Cells["A1:C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A1:C2"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    ws_resumen.Cells["A1:C2"].Style.Font.Bold = true;
                    ws_resumen.Cells["A3"].Style.Font.Bold = true;

                    ws_resumen.Cells["A2"].Value = "CUADRATURA DE LIBRAS PANACEA";
                    ws_resumen.Cells["A2:B2"].Merge = true;

                    ws_resumen.Cells["A3"].Value = "Fecha Proceso: ";
                    ws_resumen.Cells["B3"].Value = DateTime.Now.ToDateTimeFormat();
                    ws_resumen.Cells["B3:C3"].Merge = true;

                    ws_resumen.Cells["A4"].Value = "SEGÚN AUDITORÍA";
                    ws_resumen.Cells["A4:C4"].Merge = true;
                    ws_resumen.Cells["A4:C4"].Style.Font.Bold = true;
                    ws_resumen.Cells["A4:C4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws_resumen.Cells["A4:C4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A4:C4"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                    ws_resumen.Cells["A5"].Value = "BODEGAS";
                    ws_resumen.Cells["B5"].Value = "LIBRAS";
                    ws_resumen.Cells["C5"].Value = "DÓLARES";
                    ws_resumen.Cells["A5:C5"].Style.Font.Bold = true;
                    ws_resumen.Cells["A5:C5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A5:C5"].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);


                    int numInicio = 6;
                    for (int i = 0; i < resumen.Length; i++)
                    {
                        numInicio++;
                        var criterio = resumen[i];
                        ws_resumen.Cells[$"A{numInicio}"].Value = criterio.Titulo;

                        if (criterio.EsTitulo)
                        {
                            ws_resumen.Cells[$"A{numInicio}"].Style.Font.Bold = true;
                            ws_resumen.Cells[$"A{numInicio}:C{numInicio}"].Merge = true;

                        }
                        else
                        {
                            ws_resumen.Cells[$"B{numInicio}"].Value = criterio.Peso;
                            ws_resumen.Cells[$"C{numInicio}"].Value = criterio.Dolares;
                            ws_resumen.Cells[$"B{numInicio}:C{numInicio}"].Style.Numberformat.Format = "#,##0";
                        }

                    }

                    ws_resumen.Cells["A16:C16"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A16:C16"].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);

                    ws_resumen.Cells["A21:C21"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_resumen.Cells["A21:C21"].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);


                    ws_resumen.Cells.AutoFitColumns();

                    #endregion

                    #region Tabla resultado

                    ws.Cells["A1"].LoadFromCollection(Collection: detalles, PrintHeaders: true);
                    //create a range for the table
                    var range = ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column];

                    //add a table to the range
                    var tab = ws.Tables.Add(range, "Movimientos");

                    //format the table
                    tab.TableStyle = TableStyles.Light1;
                    ws.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Row(1).Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    ws.Cells.AutoFitColumns();

                    #endregion

                    #region Tabla Dinamica (Detalles)
                    var dataRange = ws.Cells[ws.Dimension.Address];

                    // Crear la tabla dinámica
                    var pivotTable = ws_detalle.PivotTables.Add(ws_detalle.Cells["A3"], dataRange, "Tabla");

                    // Filtros
                    ExcelPivotTableField categoriaPageField = pivotTable.PageFields.Add(
                        pivotTable.Fields["Categoría de Motivos"]
                    );
                    categoriaPageField.Sort = eSortType.Ascending;
                    ws_detalle.View.FreezePanes(5, 1);

                    // Campos descriptivos
                    pivotTable.RowFields.Add(pivotTable.Fields["Bodega"]);
                    pivotTable.RowFields.Add(pivotTable.Fields["Motivo"]);
                    pivotTable.RowFields.Add(pivotTable.Fields["Ítem"]);
                    pivotTable.DataOnRows = false;

                    // Campos de datos
                    ExcelPivotTableDataField cantidadField = pivotTable.DataFields.Add(
                        pivotTable.Fields["Cantidad Libras"]
                    );
                    cantidadField.Function = DataFieldFunctions.Sum;
                    cantidadField.Name = "Cantidad Libras";
                    cantidadField.Format = "#,##0";

                    ExcelPivotTableDataField costoLibrasField = pivotTable.DataFields.Add(
                        pivotTable.Fields["Costo Total Libras"]
                    );
                    costoLibrasField.Function = DataFieldFunctions.Sum;
                    costoLibrasField.Name = "Costo Total Libras";
                    costoLibrasField.Format = "$#,##0";

                    #endregion

                    #region Tabla Saldos por días bodega

                    ws_saldos.Cells["A1"].LoadFromCollection(Collection: SaldosDiaBodega, PrintHeaders: true);
                    ws_saldos.Cells.AutoFitColumns();
                    var range_saldos = ws_saldos.Cells[1, 1, ws_saldos.Dimension.End.Row, ws_saldos.Dimension.End.Column];
                    var tab_saldos = ws_saldos.Tables.Add(range_saldos, "Saldos");
                    ws_saldos.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws_saldos.Row(1).Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    tab_saldos.TableStyle = TableStyles.Light1;
                    #endregion

                    #region Tabla Kardex Valorizado

                    var detalles_ordenados = detalles
                                                .OrderByDescending(d => d.ProcesoPlanta)
                                                .ThenBy(d => d.CodigoNombreItem)
                                                .ThenBy(d => d.FechaOrden)
                                                .ThenBy(d => d.NumOrden)
                                                .ThenBy(d => d.OrdenTransferencia);


                    ws_kardex.Cells["A1"].LoadFromCollection(Collection: detalles_ordenados, PrintHeaders: true);

                    int[] columnsToDelete = { 29, 28, 27, 25, 24, 21, 19, 17, 11, 10, 2 };

                    //17, 19, 21, 24, 25, 27, 28, 29
                    foreach (var col in columnsToDelete)
                    {
                        ws_kardex.DeleteColumn(col);
                    }
                    // Renombrar columnas
                    ws_kardex.Cells["A1"].Value = "Secuencia Costo";
                    ws_kardex.Cells["B1"].Value = "Id Mov. Inv.";
                    ws_kardex.Cells["O1"].Value = "Cantidad Unidades";
                    ws_kardex.Cells["P1"].Value = "Costo Total";
                    // c d e
                    // Agregar nuevas columnas
                    //this.AddFormulaToColumn(ws_kardex, "QUIEBRE", "IF(CONCATENATE(F2, G2) = CONCATENATE(F1, G1), \"\", \"X\")", 18);
                    this.AddFormulaToColumn(ws_kardex, "QUIEBRE", "IF( G2 =  G1, \"\", \"X\")", 19);
                    this.AddFormulaToColumn(ws_kardex, "LIB.INICIAL", "IF(S2=\"X\", 0, W1)", 20);
                    this.AddFormulaToColumn(ws_kardex, "LIB.INGRESOS", "IF(Q2 > 0, Q2, 0)", 21);
                    this.AddFormulaToColumn(ws_kardex, "LIB.EGRESOS", "IF(Q2 < 0, Q2, 0)", 22);
                    this.AddFormulaToColumn(ws_kardex, "LIB.FINAL", "IF(SUM(T2, U2, V2) < 0.0001, 0, SUM(T2, U2, V2))", 23);
                    this.AddFormulaToColumn(ws_kardex, "DOL.INICIAL", "IF(S2=\"X\", 0, AA1)", 24);
                    this.AddFormulaToColumn(ws_kardex, "DOL.INGRESOS", "IF(P2 > 0, P2, 0)", 25);
                    this.AddFormulaToColumn(ws_kardex, "DOL.EGRESOS", "IF(P2 < 0, P2, 0)", 26);
                    this.AddFormulaToColumn(ws_kardex, "DOL.FINAL", "SUM(X2, Y2, Z2)", 27);
                    var range_calcs = ws_kardex.Cells[2, 19, ws_kardex.Dimension.End.Row, 27];
                    range_calcs.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range_calcs.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(153, 204, 255));
                    range_calcs.Style.Numberformat.Format = "#,##0.00";
                    var range_calcs_header = ws_kardex.Cells[1, 19, 1, 27];
                    range_calcs_header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range_calcs_header.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 51, 102));

                    this.AddFormulaToColumn(ws_kardex, "Ingreso.Libras", "U2", 28);
                    this.AddFormulaToColumn(ws_kardex, "Ingreso.PU.Libras", "IF(U2 = 0, 0, Y2/U2)", 29); // = SI(U2 = 0, 0, Y2 / U2)
                    this.AddFormulaToColumn(ws_kardex, "Ingreso.CosTotal", "Y2", 30);   //
                    this.AddFormulaToColumn(ws_kardex, "Egreso.Libras", "V2", 31);      //
                    this.AddFormulaToColumn(ws_kardex, "Egreso.PU.Libras", "IF(V2 = 0, 0, Z2/V2)", 32);  // =SI(V2 = 0, 0, Z2/V2)
                    this.AddFormulaToColumn(ws_kardex, "Egreso.CosTotal", "Z2", 33);
                    this.AddFormulaToColumn(ws_kardex, "Saldo.Libras", "W2", 34);
                    this.AddFormulaToColumn(ws_kardex, "Saldo.PU.Libras", "IF(W2 = 0, 0, AA2/W2)", 35); //=SI(W2 = 0, 0, AA2/W2)
                    this.AddFormulaToColumn(ws_kardex, "Saldo.CosTotal", "AA2", 36); //
                    var range_calcs2 = ws_kardex.Cells[2, 28, ws_kardex.Dimension.End.Row, 36];
                    range_calcs2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range_calcs2.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 255, 204));
                    range_calcs2.Style.Numberformat.Format = "#,##0.00";
                    var range_calcs2_header = ws_kardex.Cells[1, 28, 1, 36];
                    range_calcs2_header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range_calcs2_header.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 51, 0));

                    // Formatos
                    ws_kardex.Cells[2, 15, ws_kardex.Dimension.End.Row, 18].Style.Numberformat.Format = "#,##0";
                    ws_kardex.Cells[2, 20, ws_kardex.Dimension.End.Row, 29].Style.Numberformat.Format = "#,##0.00";
                    ws_kardex.Cells[2, 30, ws_kardex.Dimension.End.Row, 30].Style.Numberformat.Format = "#,##0.000000";
                    ws_kardex.Cells[2, 31, ws_kardex.Dimension.End.Row, 32].Style.Numberformat.Format = "#,##0.00";
                    ws_kardex.Cells[2, 33, ws_kardex.Dimension.End.Row, 33].Style.Numberformat.Format = "#,##0.000000";
                    ws_kardex.Cells[2, 34, ws_kardex.Dimension.End.Row, 35].Style.Numberformat.Format = "#,##0.00";
                    ws_kardex.Cells[2, 36, ws_kardex.Dimension.End.Row, 36].Style.Numberformat.Format = "#,##0.000000";


                    var range_kardex = ws_kardex.Cells[1, 1, ws_kardex.Dimension.End.Row, ws_kardex.Dimension.End.Column];
                    var tab_kardex = ws_kardex.Tables.Add(range_kardex, "Kardex");

                    ws_kardex.Cells.AutoFitColumns();
                    #endregion

                    pck.SaveAs(outputStream);
                }

                pdfBuffer = outputStream.ToArray();
            }

            var fechaInicioVal = productValuationTemp.fechaInicio;
            var fechaFinVal = productValuationTemp.fechaFin;
            return new FileContentResult(pdfBuffer, ExcelXlsxMime)
            {
                FileDownloadName = $"Valorización Productos. Rango: {fechaInicioVal.ToDateFormat()} - {fechaFinVal.ToDateFormat()}." +
                    $"Generado: {DateTime.Now.ToDateTimeFormat()}.xlsx",
            };

            return new FileContentResult(null, null);
        }
        private void deleteSaldoTemporal(int periodoEjecucion)
        {
            string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            using (var db = new System.Data.SqlClient.SqlConnection(dapperDBContext))
            {
                db.Open();

                using (var tr = db.BeginTransaction())
                {
                    try
                    {
                        // Procesamos los movimientos de inventarios
                         
                            using (var command = new SqlCommand())
                            {

                                var parameters = new DynamicParameters();

                                parameters.Add("@Periodo", periodoEjecucion, DbType.Int32, ParameterDirection.Input, null);


                                string textCommnad = $"DELETE SaldoInicialTemporal " +
                                                     $" WHERE fechaCorteInteger  = @Periodo ";
                                                    
                                command.CommandText = textCommnad;

                                 

                                db.Execute(textCommnad, parameters, tr, null, CommandType.Text);

                            }

                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        throw ex;
                    }
                }

                db.Close();
            }
        }
        private void registerSaldoTemporal(ProductionCostProductValuationBalance[] valuationBalance, int periodoEjecucion)
        {
            string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            using (var db = new System.Data.SqlClient.SqlConnection(dapperDBContext))
            {
                db.Open();

                using (var tr = db.BeginTransaction())
                {
                    try
                    {
                        // Procesamos los movimientos de inventarios
                        foreach (var reg in valuationBalance)
                        {
                            using (var command = new SqlCommand())
                            {

                                var parameters = new DynamicParameters();
                                
                                parameters.Add("@ProcessPlant", reg.id_personProcessPlant, DbType.Int32, ParameterDirection.Input, null);
                                parameters.Add("@Item", reg.idItem, DbType.Int32, ParameterDirection.Input, null);
                                parameters.Add("@Item", reg.idItem, DbType.Int32, ParameterDirection.Input, null);
                                parameters.Add("@Cantidad", reg.cantidad, DbType.Int32, ParameterDirection.Input, null);
                                parameters.Add("@PrecioUnitario", reg.precioUnitario, DbType.Decimal, ParameterDirection.Input, null);
                                parameters.Add("@CostoTotal", reg.costoTotal, DbType.Decimal, ParameterDirection.Input, null);
                                parameters.Add("@Periodo", periodoEjecucion, DbType.Int32, ParameterDirection.Input, null);


                                string textCommnad = $"INSERT SaldoInicialTemporal " +
                                                    $" (id_personProcessPlant,idItem,cantidad,costo_unitario,costo_total,fechaCorteInteger,id_ejecucion) values " +
                                                    $" (@ProcessPlant,@Item,@Cantidad,@PrecioUnitario,@CostoTotal,@Periodo,0) ;";
                                command.CommandText = textCommnad;

                                //command.Parameters.Add(m_IdParamName, SqlDbType.Int).Value = id;
                                //command.Parameters.Add(m_EntryAmountCostParamName, SqlDbType.Decimal).Value = entryAmountCost;
                                //command.Parameters.Add(m_ExitAmountCostParamName, SqlDbType.Decimal).Value = exitAmountCost;
                                //command.Parameters.Add(m_UnitPriceParamName, SqlDbType.Decimal).Value = unitPrice;
                                //command.Parameters.Add(m_UnitPriceMoveParamName, SqlDbType.Decimal).Value = unitPrice;
                                //command.Parameters.Add(m_Id_userUpdateParamName, SqlDbType.Int).Value = id_userUpdate;
                                //command.Parameters.Add(m_DateUpdateParamName, SqlDbType.DateTime).Value = dateUpdate;

                                //command.CommandType = CommandType.Text;
                                //command.Connection = db;
                                //command.Transaction = tr;
                                //command.Execute() .ExecuteNonQuery();
                                
                                db.Execute(textCommnad, parameters, tr,null, CommandType.Text );

                            }
                        }

                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        throw ex;
                    }
                }

                db.Close();
            }
        }

        private void registerSaldoFinal(SaldoInicial[] saldoInicialList, SqlConnection db, SqlTransaction tr)
        {
            //string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            //using (var db = new System.Data.SqlClient.SqlConnection(dapperDBContext))
            //{
            //    db.Open();

                //using (var tr = db.BeginTransaction())
                //
                    try
                    {
                        // Procesamos los movimientos de inventarios
                        foreach (var reg in saldoInicialList)
                        {
                            using (var command = new SqlCommand())
                            {

                                var parameters = new DynamicParameters();

                                //command.Parameters.Add("@Id", SqlDbType.Int).Value = reg.id;
                                command.Parameters.Add("@Bodega", SqlDbType.Int).Value = reg.idBodega;
                                command.Parameters.Add("@Item", SqlDbType.Int).Value = reg.idItem;
                                command.Parameters.Add("@Cantidad", SqlDbType.Int).Value = reg.cantidad;
                                command.Parameters.Add("@CostoUnitario", SqlDbType.Decimal).Value = reg.costo_unitario;
                                command.Parameters.Add("@CostoTotal", SqlDbType.Decimal).Value = reg.costo_total;
                                command.Parameters.Add("@Activo", SqlDbType.Bit).Value = reg.activo;
                                command.Parameters.Add("@UsuarioCreacion", SqlDbType.Int).Value = reg.idUsuarioCreacion;
                                command.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = reg.fechaCreacion;
                                command.Parameters.Add("@UsuarioModificacion", SqlDbType.Int).Value = reg.idUsuarioModificacion;
                                command.Parameters.Add("@FechaModificacion", SqlDbType.DateTime).Value = reg.fechaModificacion;
                                command.Parameters.Add("@Allocation", SqlDbType.Int).Value = reg.id_allocationType;
                                command.Parameters.Add("@Corte", SqlDbType.Int).Value = reg.fechaCorteInteger;
                                command.Parameters.Add("@PlantaProceso", SqlDbType.Int).Value = reg.id_personProcessPlant;


                                string textCommnad = $"INSERT SaldoInicial " +
                                                    $" (idBodega,idItem,cantidad,costo_unitario,costo_total,activo,idUsuarioCreacion,fechaCreacion,idUsuarioModificacion,fechaModificacion,id_allocationType,fechaCorteInteger,id_personProcessPlant) values " +
                                                    $" (@Bodega,@Item,@Cantidad,@CostoUnitario,@CostoTotal,@Activo,@UsuarioCreacion,@FechaCreacion,@UsuarioModificacion,@FechaModificacion,@Allocation,@Corte,@PlantaProceso) ;";
                                command.CommandText = textCommnad;
                                command.CommandType = CommandType.Text;
                                command.Connection = db;
                                command.Transaction = tr;
                                command.ExecuteNonQuery();

                        //command.Parameters.Add(m_IdParamName, SqlDbType.Int).Value = id;
                        //command.Parameters.Add(m_EntryAmountCostParamName, SqlDbType.Decimal).Value = entryAmountCost;
                        //command.Parameters.Add(m_ExitAmountCostParamName, SqlDbType.Decimal).Value = exitAmountCost;
                        //command.Parameters.Add(m_UnitPriceParamName, SqlDbType.Decimal).Value = unitPrice;
                        //command.Parameters.Add(m_UnitPriceMoveParamName, SqlDbType.Decimal).Value = unitPrice;
                        //command.Parameters.Add(m_Id_userUpdateParamName, SqlDbType.Int).Value = id_userUpdate;
                        //command.Parameters.Add(m_DateUpdateParamName, SqlDbType.DateTime).Value = dateUpdate;

                        //command.CommandType = CommandType.Text;
                        //command.Connection = db;
                        //command.Transaction = tr;
                        //command.Execute() .ExecuteNonQuery();

                        //db.Execute(textCommnad, parameters, tr, null, CommandType.Text);

                            }
                        }

                        //tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        //tr.Rollback();
                        throw ex;
                    }
               //}

                //db.Close();
           // }
        }

        #endregion



        private List<DataProcesarInventario> ejecutarValidacionesDinamicas(List<DataProcesarInventario> input, CosteoValidacionesDinamicas[] reglas)
        {
            List<DataProcesarInventario> model = new List<DataProcesarInventario>();
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;

            for (int i = 0; i < input.Count; i++)
            {
                DataProcesarInventario data = input[i];
                var regla = reglas.FirstOrDefault(r => r.WarehouseId == data.IdWarehouse && r.InventoryReasonId == data.IdInventoryReason);
                if (regla == null)
                {
                    model.Add(data);
                    continue;
                }

                var setData = findIn(input.ToArray(), index: i);
                string dataString = JsonConvert.SerializeObject(setData);

                bool result = false;
                using (var cnn = new SqlConnection(dapperDBContext))
                {
                    cnn.Open();
                    var parametros = new
                    {
                        data = dataString,
                        pars = ""
                    };

                    result = cnn.QueryFirstOrDefault<bool>(regla.EvaluateScript, parametros, commandType: CommandType.StoredProcedure);
                    cnn.Close();
                }

                if (!result)
                {
                    model.Add(data);
                    continue;
                }

                switch (regla.TypeAction)
                {
                    case "ORDEN":
                        data.OrdenMotivoTipoDocum = int.Parse(regla.ActionValue);
                        data.OrdenTransferencias = 0;
                        model.Add(data);
                        break;
                }
            }

            return model
                    .OrderByDescending(e => e.Proceso)
                    .ThenBy(e => e.OrdenMotivoTipoDocum)
                    .ThenBy(e => e.OrdenTransferencias)
                    .ToList(); ;
        }
        private DataProcesarInventario[] findIn(DataProcesarInventario[] input, int index)
        {
            List<DataProcesarInventario> result = new List<DataProcesarInventario>();
            bool encontrado = false;
            DataProcesarInventario current = input[index];
            for (int i = 0; i < input.Length; i++)
            {
                //if (encontrado && input[i].EmissionDate.Date == current.EmissionDate)
                if (input[i].EmissionDate.Date == current.EmissionDate.Date && i != index &&
                    input[i].IdItem == current.IdItem)
                {
                    result.Add(input[i]);
                }
                //else if (i== index)
                //{
                //    encontrado = true;  
                //}

            }
            return result.ToArray();

        }
        private OrdenamientoBodegaMotivo RecuperarOrdenamiento(int idBodega, int idMotivo)
        {
            var key = $"Ordenaminento_{idBodega}_{idMotivo}";
            OrdenamientoBodegaMotivo retorno;
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as OrdenamientoBodegaMotivo;
            }
            else
            {
                //const string m_sql = "Select top(1) * from orden_motivos_bodegas where id_warehose = @idBodega and id_inventoryreason = @idMotivo;";
                //var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;

                //using (var cnn = new SqlConnection(dapperDBContext))
                //{
                //    cnn.Open();
                //    retorno = cnn.QuerySingleOrDefault<OrdenamientoBodegaMotivo>(m_sql, param: new { idBodega, idMotivo });
                //
                //    cnn.Close();
                //}
                retorno = _ordenamientos.FirstOrDefault(r => r.Id_Warehouse == idBodega && r.Id_inventoryreason == idMotivo);
            }

            TempData[key] = retorno;
            TempData.Keep(key);

            return retorno;
        }
        private OrdenamientoBodegaMotivo RecuperarOrdenamiento(string conector1)
        {
            var key = $"OrdenaminentoC1_{conector1}";
            OrdenamientoBodegaMotivo retorno;
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as OrdenamientoBodegaMotivo;
            }
            else
            {
                //const string m_sql = "Select top(1) * from orden_motivos_bodegas where conector2 = @conector1;";
                //var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                //
                //using (var cnn = new SqlConnection(dapperDBContext))
                //{
                //    cnn.Open();
                //    retorno = cnn.QuerySingleOrDefault<OrdenamientoBodegaMotivo>(m_sql, param: new { conector1 });
                //
                //    cnn.Close();
                //}
                retorno = _ordenamientos.FirstOrDefault(r => r.Conector2 == conector1);
            }

            TempData[key] = retorno;
            TempData.Keep(key);

            return retorno;
        }

        private decimal getCostoUnitarioSaldo(  List<CostoProcesar> saldos,
                                                int itemId, 
                                                int? warehouseId, 
                                                int? processPlantId)
        {
            string parametroMetodoValorizacion = getParametroMetodoValorizacion();
            var saldoActual = (parametroMetodoValorizacion.Equals(m_MetodoValorizacionLote))
                                            ? saldos.FirstOrDefault(e => e.IdBodega == warehouseId
                                                                            && e.IdItem == itemId)
                                            : saldos.FirstOrDefault(e => e.IdItem == itemId
                                                                         && e.LastProcessPlantId == processPlantId
                                            );
            if (parametroMetodoValorizacion.Equals(m_MetodoValorizacionProceso) && (saldoActual?.CostoUnitario??0)  <= 0)
            {
                saldoActual = saldos.FirstOrDefault(r => r.IdItem == itemId && r.LastUnitCost > 0);
            }

            return (saldoActual?.CostoUnitario ?? 0);
        }
    }
    public class OrdenamientoBodegaMotivoDb
    {
        public decimal Id_Warehose { get; set; }
        public decimal Id_inventoryreason { get; set; }
        public string Conector { get; set; }
        public string Conector2 { get; set; }
        public decimal Orden { get; set; }
        public string OrdenConector { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
    }

    public class OrdenamientoBodegaMotivo
    {
        public int Id_Warehouse { get; set; }
        public int Id_inventoryreason { get; set; }
        public string Conector { get; set; }
        public string Conector2 { get; set; }
        public decimal Orden { get; set; }
        public string OrdenConector { get; set; }
    }

    public class SaldoInicialDto
    {
        public int id { get; set; }
        public int idBodega { get; set; }
        public int idItem { get; set; }
        public int cantidad { get; set; }
        public decimal costo_unitario { get; set; }
        public decimal costo_total { get; set; }
        public bool activo { get; set; }
        public int idUsuarioCreacion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int idUsuarioModificacion { get; set; }
        public System.DateTime fechaModificacion { get; set; }
        public int id_allocationType { get; set; }
        public int fechaCorteInteger { get; set; }
        public int? id_personProcessPlant { get; set; }
        public string plantaProceso { get; set; }
    }
    public static class OrdenamientoBodegaAux
    {
        public static OrdenamientoBodegaMotivo ToOrdenamientoBodegaMotivo(this OrdenamientoBodegaMotivoDb input)
        {
            return new OrdenamientoBodegaMotivo
            {
                Conector = input.Conector,
                Conector2 = input.Conector2,
                Id_inventoryreason = Convert.ToInt32(input.Id_inventoryreason),
                Id_Warehouse = Convert.ToInt32(input.Id_Warehose),
                Orden = input.Orden,
                OrdenConector = input.OrdenConector
            };
        }
        public static string GetProcesoCostoInicialDesdeBodega(int bodega)
        {
            string plantaProceso = null;
            switch (bodega)
            {


                case 29:
                case 30:
                    plantaProceso = "PROCESO 1";
                    break;
                case 0:
                case 80:
                case 81:
                    plantaProceso = "PROCESO 2";
                    break;


            }

            return plantaProceso;
        }
    }

    public class InventoryDetailCostDto
    {

        public int ItemId { get; set; }
        public int IdInventoryMoveDetail { get; set; }
        public int id_inventoryMove { get; set; }
        public InventoryDetailCostEntryDto[] MoveDetailEntry { get; set; }
    }

    public class InventoryDetailCostEntryDto
    {
        public int ItemId { get; set; }
        public int IdInventoryMoveDetail { get; set; }
        public int id_inventoryMove { get; set; }
    }
    public class InventoryDetailCostTranferDto
    {
        public int ItemId { get; set; }
        public int IdInventoryMoveDetail { get; set; }
        public int id_inventoryMove { get; set; }

        public InventoryDetailCostTranferLinkedDto[] TransferLinked { get; set; }
        public InventoryMoveCost InventoryMoveHead { get; set; }
    }

    public class InventoryMoveCost
    {
        public int id_inventoryMove { get; set; }
        public int idWarehouse { get; set; }
        public int id_inventoryReason { get; set; }
        public int id_lot { get; set; }
    }
    public class InventoryDetailCostTranferLinkedDto
    {
        public int id_inventoryMoveDetailExit { get; set; }
        public int id_inventoryMoveDetailEntry { get; set; }
        public bool IsAutomatic { get; set; }
        public InventoryDetailCostTranferDto[] MoveDetailEntry { get; set; }
    }

    public class CosteoValidacionesExcepcionDto
    {
        public int CompanyId { get; set; }
        public int EntryInventoryReasonId { get; set; }
        public string EntryInventoryReasonCode { get; set; }
        public int ExitInventoryReasonId { get; set; }
        public string ExitInventoryReasonCode { get; set; }
        public string CodeBehavior { get; set; }
        public DateTime DateInit { get; set; }
        public DateTime? DateEnd { get; set; }

    }

    public class TestCostoSaldoMovimiento
    {
        public int InventoryMoveDetailId { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

}
