using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class AllocationCostPeriodController : DefaultController
    {
        private const string m_TipoDocumentoAllocationCostPeriod = "152";

        private const string m_PendienteDocumentState = "01";
        private const string m_AprobadoDocumentState = "03";
        private const string m_AnuladoDocumentState = "05";

        private const string m_ResultQueryViewKeyName = "Allocation_QueryResults";
        private const string m_AllocationCostPeriodModelKey = "allocationCostPeriod";


        [HttpPost]
        public PartialViewResult Index()
        {
            return this.PartialView();
        }


        #region Vista de consulta principal

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult AllocationCostPeriodResults(int? id_documentState,
            string number, string reference, DateTime? startEmissionDate, DateTime? endEmissionDate,
            int? anio, int? mes, int? id_executionType)
        {
            // Preparar el Query para los datos resultantes
            var resultsQuery = db.ProductionCostAllocationPeriod
                .Include("Document")
                .Include("Document.DocumentState")
                .Include("ProductionCostCoefficient")
                .Include("ProductionCostExecutionType")
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

            if (id_executionType.HasValue)
            {
                resultsQuery = resultsQuery
                    .Where(i => i.id_executionType == id_executionType.Value);
            }


            // suprimr los elementos que NO se usaràn para el filtro!!!
            var model = resultsQuery
                .OrderByDescending(i => i.id)
                .ToList();

            this.TempData[m_ResultQueryViewKeyName] = model;
            this.TempData.Keep(m_ResultQueryViewKeyName);

            return this.PartialView("_AllocationCostPeriodQueryResultsPartial", model);
        }

        [HttpPost]
        public ActionResult AllocationCostPeriodPartial()
        {
            var model = (this.TempData[m_ResultQueryViewKeyName] as List<ProductionCostAllocationPeriod>)
                ?? new List<ProductionCostAllocationPeriod>();

            this.TempData.Keep(m_ResultQueryViewKeyName);

            return PartialView("_AllocationCostPeriodQueryGridViewPartial", model);
        }

        #endregion

        #region Vista de edición de transacción

        [HttpPost]
        public PartialViewResult EditForm(int? id, string successMessage)
        {
            this.TempData.Remove(m_AllocationCostPeriodModelKey);

            var allocationCostPeriod = this.GetEditingAllocationCostPeriod(id, null, null, null);
            this.TempData[m_AllocationCostPeriodModelKey] = allocationCostPeriod;

            this.PrepareEditViewBag(allocationCostPeriod);

            if (!String.IsNullOrWhiteSpace(successMessage))
            {
                this.ViewBag.EditMessage = this.SuccessMessage(successMessage);
            }

            return PartialView("_EditForm", allocationCostPeriod);
        }


        [HttpPost]
        public JsonResult Create(
            DateTime emissionDate, int anio, int mes, bool accountingValue,
            int idExecutionType, int idCoefficient, string description, string reference)
        {
            int? idAllocationCostPeriod;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Validamos que no exista otro documento similar
                    var currentDocument = db.ProductionCostAllocationPeriod
                        .FirstOrDefault(a => (a.id_company == this.ActiveCompanyId)
                                            && (a.anio == anio) && (a.mes == mes)
                                            && (a.id_coefficient == idCoefficient)
                                            && (a.Document.DocumentState.code != m_AnuladoDocumentState));

                    if (currentDocument != null)
                    {
                        throw new ApplicationException("Ya existe otro documento para el período y número de operación indicados.");
                    }

                    // Recuperamos el tipo de documento
                    var documentType = db.DocumentType
                        .FirstOrDefault(dt => dt.code == m_TipoDocumentoAllocationCostPeriod
                                            && dt.id_company == this.ActiveCompanyId);

                    if (documentType == null)
                    {
                        throw new ApplicationException("No existe registrado el tipo de documento: Asignación de Costos.");
                    }

                    // Recuperamos el estado PENDIENTE
                    var documentState = DataProviderAllocationCostPeriod
                        .GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

                    if (documentState == null)
                    {
                        throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
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

                    // Creamos el documento de asignación de costos
                    var allocationCostPeriod = new ProductionCostAllocationPeriod()
                    {
                        Document = document,

                        id_company = this.ActiveCompanyId,
                        anio = anio,
                        mes = mes,
                        id_executionType = idExecutionType,
                        id_coefficient = idCoefficient,
                        accountingValue = accountingValue,
                        description = description,

                        id_userCreate = this.ActiveUserId,
                        dateCreate = DateTime.Now,
                        id_userUpdate = this.ActiveUserId,
                        dateUpdate = DateTime.Now,
                    };

                    // Agregar los detalles de la asignación
                    var allocationCostPeriodTemp = (this.TempData[m_AllocationCostPeriodModelKey] as ProductionCostAllocationPeriod);

                    if (allocationCostPeriodTemp?.ProductionCostAllocationPeriodDetails != null)
                    {
                        allocationCostPeriod.ProductionCostAllocationPeriodDetails = allocationCostPeriodTemp
                            .ProductionCostAllocationPeriodDetails
                            .Select(d => new ProductionCostAllocationPeriodDetail()
                            {
                                id_allocationPeriod = d.id_allocationPeriod,
                                id_productionCost = d.id_productionCost,
                                id_productionCostDetail = d.id_productionCostDetail,
                                id_productionPlant = d.id_productionPlant,
                                coeficiente = d.coeficiente,
                                valor = d.valor,
                                isActive = true,

                                id_userCreate = this.ActiveUserId,
                                dateCreate = DateTime.Now,
                                id_userUpdate = this.ActiveUserId,
                                dateUpdate = DateTime.Now,
                            })
                            .ToList();
                    }

                    // Guardamos el documento
                    db.Document.Add(document);
                    db.ProductionCostAllocationPeriod.Add(allocationCostPeriod);
                    db.SaveChanges();
                    transaction.Commit();

                    idAllocationCostPeriod = allocationCostPeriod.id;
                    message = "Documento creado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_AllocationCostPeriodModelKey);

                    idAllocationCostPeriod = null;
                    message = "Error al crear documento: " + exception.Message;
                    isValid = false;
                }
            }

            var result = new
            {
                idAllocationCostPeriod,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Save(int idAllocationCostPeriod,
            DateTime emissionDate, int anio, int mes, bool accountingValue,
            int idExecutionType, int idCoefficient, string description, string reference,
            bool approveDocument)
        {
            int? idAllocationCostPeriodResult;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Validamos que no exista otro documento similar
                    var currentDocument = db.ProductionCostAllocationPeriod
                        .FirstOrDefault(a => (a.id_company == this.ActiveCompanyId)
                                            && (a.anio == anio) && (a.mes == mes)
                                            && (a.id_coefficient == idCoefficient)
                                            && (a.Document.DocumentState.code != m_AnuladoDocumentState)
                                            && (a.id != idAllocationCostPeriod));

                    if (currentDocument != null)
                    {
                        throw new ApplicationException("Ya existe otro documento para el período y número de operación indicados.");
                    }

                    // Recuperar la entidad actual
                    var allocationCostPeriod = db.ProductionCostAllocationPeriod
                        .First(c => c.id == idAllocationCostPeriod);

                    // Verificamos el estado actual del documento
                    var documentStateCode = allocationCostPeriod.Document.DocumentState.code;

                    if (documentStateCode != m_PendienteDocumentState)
                    {
                        throw new ApplicationException("Acción es permitida solo para Documento en estado PENDIENTE.");
                    }

                    // Actualizamos el documento
                    var document = allocationCostPeriod.Document;

                    document.emissionDate = emissionDate;
                    document.description = description;
                    document.reference = reference;

                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;

                    // Actualizamos el documento de asignación de costos
                    allocationCostPeriod.anio = anio;
                    allocationCostPeriod.mes = mes;
                    allocationCostPeriod.id_executionType = idExecutionType;
                    allocationCostPeriod.id_coefficient = idCoefficient;
                    allocationCostPeriod.accountingValue = accountingValue;
                    allocationCostPeriod.description = description;

                    allocationCostPeriod.id_userUpdate = this.ActiveUserId;
                    allocationCostPeriod.dateUpdate = DateTime.Now;

                    // Actualizar los detalles de la asignación de costos
                    var allocationCostPeriodTemp = (this.TempData[m_AllocationCostPeriodModelKey] as ProductionCostAllocationPeriod);
                    bool tieneDetallesActivos;

                    if (allocationCostPeriodTemp?.ProductionCostAllocationPeriodDetails != null)
                    {
                        var productionCoefficientDetailsTemp = allocationCostPeriodTemp
                            .ProductionCostAllocationPeriodDetails
                            .ToList();

                        tieneDetallesActivos = productionCoefficientDetailsTemp.Any();

                        // Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
                        foreach (var detail in allocationCostPeriod.ProductionCostAllocationPeriodDetails)
                        {
                            if (productionCoefficientDetailsTemp.Any())
                            {
                                // Actualizamos los detalles
                                var detailTemp = productionCoefficientDetailsTemp[0];
                                productionCoefficientDetailsTemp.RemoveAt(0);

                                detail.id_productionCost = detailTemp.id_productionCost;
                                detail.id_productionCostDetail = detailTemp.id_productionCostDetail;
                                detail.id_productionPlant = detailTemp.id_productionPlant;
                                detail.coeficiente = detailTemp.coeficiente;
                                detail.valor = detailTemp.valor;
                                detail.isActive = true;

                                detail.id_userUpdate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                            else
                            {
                                // Ya no hay detalles nuevos, desactivar...
                                detail.id_productionPlant = null;
                                detail.coeficiente = false;
                                detail.valor = 0;
                                detail.isActive = false;

                                detail.id_userUpdate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                        }

                        // Agregamos los detalles que faltan de agregar
                        foreach (var detailTemp in productionCoefficientDetailsTemp)
                        {
                            allocationCostPeriod.ProductionCostAllocationPeriodDetails
                                .Add(new ProductionCostAllocationPeriodDetail()
                                {
                                    id_allocationPeriod = idAllocationCostPeriod,
                                    id_productionCost = detailTemp.id_productionCost,
                                    id_productionCostDetail = detailTemp.id_productionCostDetail,
                                    id_productionPlant = detailTemp.id_productionPlant,
                                    coeficiente = detailTemp.coeficiente,
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
                        tieneDetallesActivos = false;

                        foreach (var detail in allocationCostPeriod.ProductionCostAllocationPeriodDetails)
                        {
                            detail.id_productionPlant = null;
                            detail.coeficiente = false;
                            detail.valor = 0;
                            detail.isActive = false;

                            detail.id_userUpdate = this.ActiveUserId;
                            detail.dateUpdate = DateTime.Now;
                        }
                    }


                    // Verificamos si se trata de la aprobación del documento
                    if (approveDocument)
                    {
                        // Solo se aprueba si se tienen detalles
                        if (!tieneDetallesActivos)
                        {
                            throw new ApplicationException("No se puede aprobar el documento sin detalles.");
                        }

                        // Recuperamos el estado APROBADO
                        var documentState = DataProviderAllocationCostPeriod
                            .GetDocumentStateByCode(this.ActiveCompanyId, m_AprobadoDocumentState);

                        if (documentState == null)
                        {
                            throw new ApplicationException("No existe registrado el estado APROBADO para los documentos.");
                        }

                        document.id_documentState = documentState.id;
                    }

                    // Guardamos el documento
                    db.SaveChanges();
                    transaction.Commit();

                    idAllocationCostPeriodResult = allocationCostPeriod.id;
                    message = approveDocument
                        ? "Documento aprobado exitosamente."
                        : "Documento actualizado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_AllocationCostPeriodModelKey);

                    idAllocationCostPeriodResult = null;
                    message = approveDocument
                        ? "Error al aprobar documento: " + exception.Message
                        : "Error al actualizar documento: " + exception.Message;
                    isValid = false;
                }
            }

            var result = new
            {
                idAllocationCostPeriod = idAllocationCostPeriodResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Cancel(int idAllocationCostPeriod)
        {
            int? idAllocationCostPeriodResult;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var allocationCostPeriod = db.ProductionCostAllocationPeriod
                        .First(c => c.id == idAllocationCostPeriod);

                    // Verificamos el estado actual del documento
                    var documentStateCode = allocationCostPeriod.Document.DocumentState.code;

                    if (documentStateCode != m_PendienteDocumentState)
                    {
                        throw new ApplicationException("Acción es permitida solo para Documento en estado PENDIENTE.");
                    }

                    // Recuperamos el estado ANULADO
                    var documentState = DataProviderAllocationCostPeriod
                        .GetDocumentStateByCode(this.ActiveCompanyId, m_AnuladoDocumentState);

                    if (documentState == null)
                    {
                        throw new ApplicationException("No existe registrado el estado CANCELADO para los documentos.");
                    }

                    // Actualizamos el documento
                    var document = allocationCostPeriod.Document;

                    document.id_documentState = documentState.id;

                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;

                    // Anulamos el documento
                    db.SaveChanges();
                    transaction.Commit();

                    idAllocationCostPeriodResult = allocationCostPeriod.id;
                    message = "Documento anulado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_AllocationCostPeriodModelKey);

                    idAllocationCostPeriodResult = null;
                    message = "Error al anular documento: " + exception.Message;
                    isValid = false;
                }
            }

            var result = new
            {
                idAllocationCostPeriod = idAllocationCostPeriodResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Revert(int idAllocationCostPeriod)
        {
            int? idAllocationCostPeriodResult;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var allocationCostPeriod = db.ProductionCostAllocationPeriod
                        .First(c => c.id == idAllocationCostPeriod);

                    // Verificamos el estado actual del documento
                    var documentStateCode = allocationCostPeriod.Document.DocumentState.code;

                    if (documentStateCode != m_AprobadoDocumentState)
                    {
                        throw new ApplicationException("Acción es permitida solo para Documento en estado APROBADO.");
                    }

                    // Recuperamos el estado PENDIENTE
                    var documentState = DataProviderAllocationCostPeriod
                        .GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

                    if (documentState == null)
                    {
                        throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
                    }

                    // Actualizamos el documento
                    var document = allocationCostPeriod.Document;

                    document.id_documentState = documentState.id;

                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;

                    // Reversamos el documento
                    db.SaveChanges();
                    transaction.Commit();

                    idAllocationCostPeriodResult = allocationCostPeriod.id;
                    message = "Documento reversado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_AllocationCostPeriodModelKey);

                    idAllocationCostPeriodResult = null;
                    message = "Error al reversar documento: " + exception.Message;
                    isValid = false;
                }
            }

            var result = new
            {
                idAllocationCostPeriod = idAllocationCostPeriodResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Manejadores del Grid de detalles

        [ValidateInput(false)]
        public PartialViewResult AllocationCostPeriodDetail(int idAllocationCostPeriod,
            bool? accountingValue, int? idExecutionType, int? idCoefficient, bool editable)
        {
            var allocationCostPeriod = this.GetEditingAllocationCostPeriod(idAllocationCostPeriod,
                accountingValue, idExecutionType, idCoefficient);

            this.TempData[m_AllocationCostPeriodModelKey] = allocationCostPeriod;
            this.TempData.Keep(m_AllocationCostPeriodModelKey);
            this.PrepareDetailsEditViewBag(accountingValue, idExecutionType, editable);

            return this.GetAllocationCostPeriodDetailsPartialView(allocationCostPeriod);
        }

        [HttpPost, ValidateInput(false)]
        public PartialViewResult AllocationCostPeriodDetailAddNew(int idAllocationCostPeriod,
            bool accountingValue, int? idExecutionType, int? idCoefficient, ProductionCostAllocationPeriodDetail allocationCostPeriodDetail)
        {
            // Recuperamos el modelo actualmente en edición
            var allocationCostPeriod = this.GetEditingAllocationCostPeriod(idAllocationCostPeriod,
                accountingValue, idExecutionType, idCoefficient);

            // Verificamos que los datos sean válidos
            if (accountingValue)
            {
                this.ViewBag.EditError = "No se puede agregar detalles a este documento.";
            }
            else if (this.ModelState.IsValid)
            {
                // Recuperamos el detalle con el código indicado (si hubiera)...
                var currentDetail = allocationCostPeriod.ProductionCostAllocationPeriodDetails?
                    .FirstOrDefault(d => d.id_productionCost == allocationCostPeriodDetail.id_productionCost
                                            && d.id_productionCostDetail == allocationCostPeriodDetail.id_productionCostDetail
                                            && d.id_productionPlant == allocationCostPeriodDetail.id_productionPlant);

                if (currentDetail != null)
                {
                    // Si ya existe y está activo, no permitimos duplicados...
                    this.ViewBag.EditError = $"Ya existe el elemento indicado.";
                }
                else
                {
                    // Si no existe, lo agregamos...
                    var idNew = allocationCostPeriod.ProductionCostAllocationPeriodDetails.Any()
                        ? allocationCostPeriod.ProductionCostAllocationPeriodDetails.Max(d => d.id) + 1
                        : 1;

                    allocationCostPeriod.ProductionCostAllocationPeriodDetails
                        .Add(new ProductionCostAllocationPeriodDetail()
                        {
                            id = idNew,
                            id_productionCost = allocationCostPeriodDetail.id_productionCost,
                            id_productionCostDetail = allocationCostPeriodDetail.id_productionCostDetail,
                            id_productionPlant = allocationCostPeriodDetail.id_productionPlant,
                            coeficiente = allocationCostPeriodDetail.coeficiente,
                            valor = allocationCostPeriodDetail.valor,
                            isActive = true,
                        });
                }
            }
            else
            {
                this.ViewBag.EditError = "Hay errores de validación en los datos recibidos.";
            }

            this.TempData[m_AllocationCostPeriodModelKey] = allocationCostPeriod;
            this.TempData.Keep(m_AllocationCostPeriodModelKey);
            this.PrepareDetailsEditViewBag(accountingValue, idExecutionType, true);

            return this.GetAllocationCostPeriodDetailsPartialView(allocationCostPeriod);
        }

        [HttpPost, ValidateInput(false)]
        public PartialViewResult AllocationCostPeriodDetailUpdate(int idAllocationCostPeriod,
            bool accountingValue, int? idExecutionType, int? idCoefficient, ProductionCostAllocationPeriodDetail allocationCostPeriodDetail)
        {
            // Recuperamos el modelo actualmente en edición
            var allocationCostPeriod = this.GetEditingAllocationCostPeriod(idAllocationCostPeriod,
                accountingValue, idExecutionType, idCoefficient);

            // Verificamos que los datos sean válidos
            if (this.ModelState.IsValid)
            {
                // Recuperamos el detalle con el código indicado (si hubiera)...
                var currentDetail = allocationCostPeriod.ProductionCostAllocationPeriodDetails?
                    .FirstOrDefault(d => d.id_productionCost == allocationCostPeriodDetail.id_productionCost
                                            && d.id_productionCostDetail == allocationCostPeriodDetail.id_productionCostDetail
                                            && d.id_productionPlant == allocationCostPeriodDetail.id_productionPlant
                                            && d.id != allocationCostPeriodDetail.id);

                if (currentDetail != null)
                {
                    // Si ya existe y está activo, no permitimos duplicados...
                    this.ViewBag.EditError = $"Ya existe el elemento indicado.";
                }
                else
                {
                    // Recuperamos el detalle con el código indicado (si hubiera)...
                    currentDetail = allocationCostPeriod.ProductionCostAllocationPeriodDetails?
                        .FirstOrDefault(d => d.id == allocationCostPeriodDetail.id);

                    if (currentDetail != null)
                    {
                        // Si existe, lo actualizamos...
                        if (accountingValue)
                        {
                            currentDetail.coeficiente = allocationCostPeriodDetail.coeficiente;
                            currentDetail.valor = allocationCostPeriodDetail.valor;
                            currentDetail.isActive = true;
                        }
                        else
                        {
                            currentDetail.id_productionCost = allocationCostPeriodDetail.id_productionCost;
                            currentDetail.id_productionCostDetail = allocationCostPeriodDetail.id_productionCostDetail;
                            currentDetail.id_productionPlant = allocationCostPeriodDetail.id_productionPlant;
                            currentDetail.coeficiente = allocationCostPeriodDetail.coeficiente;
                            currentDetail.valor = allocationCostPeriodDetail.valor;
                            currentDetail.isActive = true;
                        }
                    }
                    else
                    {
                        this.ViewBag.EditError = $"No existe el elemento a actualizar con ID: {allocationCostPeriodDetail.id}.";
                    }
                }
            }
            else
            {
                this.ViewBag.EditError = "Hay errores de validación en los datos recibidos.";
            }

            this.TempData[m_AllocationCostPeriodModelKey] = allocationCostPeriod;
            this.TempData.Keep(m_AllocationCostPeriodModelKey);
            this.PrepareDetailsEditViewBag(accountingValue, idExecutionType, true);

            return this.GetAllocationCostPeriodDetailsPartialView(allocationCostPeriod);
        }

        [HttpPost, ValidateInput(false)]
        public PartialViewResult AllocationCostPeriodDetailDelete(int idAllocationCostPeriod,
            bool accountingValue, int? idExecutionType, int? idCoefficient, int id)
        {
            // Recuperamos el modelo actualmente en edición
            var allocationCostPeriod = this.GetEditingAllocationCostPeriod(idAllocationCostPeriod,
                accountingValue, idExecutionType, idCoefficient);

            if (accountingValue)
            {
                this.ViewBag.EditError = "No se puede eliminar detalles a este documento.";
            }
            else
            {
                // Recuperamos el detalle con el código indicado (si hubiera)...
                var currentDetail = allocationCostPeriod.ProductionCostAllocationPeriodDetails?
                    .FirstOrDefault(d => d.id == id);

                if (currentDetail != null)
                {
                    // Si existe, lo inactivamos...
                    allocationCostPeriod.ProductionCostAllocationPeriodDetails.Remove(currentDetail);
                }
                else
                {
                    this.ViewBag.EditError = $"No existe el elemento con ID: {id}.";
                }
            }

            this.TempData[m_AllocationCostPeriodModelKey] = allocationCostPeriod;
            this.TempData.Keep(m_AllocationCostPeriodModelKey);
            this.PrepareDetailsEditViewBag(accountingValue, idExecutionType, true);

            return this.GetAllocationCostPeriodDetailsPartialView(allocationCostPeriod);
        }

        #endregion

        #region Manejadores de consultas auxiliares

        [HttpPost]
        public JsonResult QueryProductionCoefficients(int? idExecutionType, bool accountingValue)
        {
            var items = DataProviderAllocationCostPeriod
                .ProductionCoefficients(idExecutionType, accountingValue, null)
                .Select(c => new
                {
                    c.id,
                    c.sequence,
                })
                .ToArray();

            var result = new
            {
                isValid = true,
                items,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private class ProcesarSaldoContable
        {
            public string id_tipoAuxContab { get; set; }
            public string id_auxiliarContab { get; set; }
            public string id_centroCtoContab { get; set; }
            public string id_subcentroCtoContab { get; set; }
            public decimal amountDebe { get; set; }
            public decimal amountHaber { get; set; }
        }


        [HttpPost]
        public JsonResult QueryAccountValue(int idAllocationCostPeriod,
            bool accountingValue, int idExecutionType, int idCoefficient, int anio, int mes)
        {
            var isValid = false;

            // Recuperamos los detalles activos del coeficiente
            var coefficientDetails = db.ProductionCostCoefficient
                .FirstOrDefault(c => c.id == idCoefficient)?
                .ProductionCostCoefficientDetails
                .Where(d => d.isActive)
                .ToArray();

            // Ejecutamos la operación para cada detalle recuperado
            var dbIntegracion = new DBContextIntegration();
            var tablaSaldosContables = dbIntegracion
                .TblCiSaldosDpto;

            var tablaSaldosContables2 = dbIntegracion
                .TblCiSaldos;

            var totalAmount = 0m;
            var totalDebe = 0m;
            var totalHaber = 0m;

            var idCompany = this.ActiveCompany.code;
            var idDivision = this.ActiveDivision.code;
            var idSucursal = this.ActiveSucursal.code;

            foreach (var coefficientDetail in coefficientDetails)
            {
                IEnumerable<ProcesarSaldoContable> detalleSaldos;
                var requiereCentroCosto = dbIntegracion
                    .TblciCuenta.FirstOrDefault(a => a.CCiCuenta == coefficientDetail.id_cuentaContab)?
                    .BsnAceptaProyecto ?? false; // Preguntar si la cuenta requiere centro de costo

                if (requiereCentroCosto)
                {
                    detalleSaldos = tablaSaldosContables
                        .Where(s => (s.CciCia == idCompany)
                                    && (s.CciDivision == idDivision)
                                    && (s.CciSucursal == idSucursal)
                                    && (s.NNuAnio == anio)
                                    && (s.NNuPeriodo == mes)
                                    && (s.CciCuenta == coefficientDetail.id_cuentaContab))
                        .Select(s => new ProcesarSaldoContable()
                        {
                            id_tipoAuxContab = s.CCiTipoAuxiliar,
                            id_auxiliarContab = s.CCiAuxiliar,
                            id_centroCtoContab = s.CCiProyecto,
                            id_subcentroCtoContab = s.CCiSubProyecto,
                            amountDebe = s.NNuDebe ?? 0m,
                            amountHaber = s.NNuHaber ?? 0m,
                        })
                        .ToArray()
                        .AsEnumerable();
                }
                else
                {
                    detalleSaldos = tablaSaldosContables2
                       .Where(s => (s.CCiCia == idCompany)
                                   && (s.CCiDivision == idDivision)
                                   && (s.CCiSucursal == idSucursal)
                                   && (s.NNuAnio == anio)
                                   && (s.NNuPeriodo == mes)
                                   && (s.CCiCuenta == coefficientDetail.id_cuentaContab))
                       .Select(s => new ProcesarSaldoContable()
                       {
                           id_tipoAuxContab = s.CCiTipoAuxiliar,
                           id_auxiliarContab = s.CCiAuxiliar,
                           id_centroCtoContab = "",
                           id_subcentroCtoContab = "",
                           amountDebe = s.NNuDebe ?? 0m,
                           amountHaber = s.NNuHaber ?? 0m,
                       })
                       .ToArray()
                       .AsEnumerable();
                }                

                if (!String.IsNullOrEmpty(coefficientDetail.id_tipoAuxContab))
                {
                    detalleSaldos = detalleSaldos
                        .Where(s => !String.IsNullOrWhiteSpace(s.id_tipoAuxContab)
                                    && s.id_tipoAuxContab.TrimEnd() == coefficientDetail.id_tipoAuxContab.TrimEnd());
                }
                else
                {
                    detalleSaldos = detalleSaldos
                        .Where(s => String.IsNullOrWhiteSpace(s.id_tipoAuxContab));
                }

                if (!String.IsNullOrEmpty(coefficientDetail.id_auxiliarContab))
                {
                    detalleSaldos = detalleSaldos
                        .Where(s => !String.IsNullOrWhiteSpace(s.id_auxiliarContab)
                                    && s.id_auxiliarContab.TrimEnd() == coefficientDetail.id_auxiliarContab.TrimEnd());
                }
                else
                {
                    detalleSaldos = detalleSaldos
                        .Where(s => String.IsNullOrWhiteSpace(s.id_auxiliarContab));
                }

                if (!String.IsNullOrEmpty(coefficientDetail.id_centroCtoContab))
                {
                    detalleSaldos = detalleSaldos
                        .Where(s => !String.IsNullOrWhiteSpace(s.id_centroCtoContab)
                                    && s.id_centroCtoContab.TrimEnd() == coefficientDetail.id_centroCtoContab.TrimEnd());
                }
                else
                {
                    detalleSaldos = detalleSaldos
                        .Where(s => String.IsNullOrWhiteSpace(s.id_centroCtoContab));
                }

                if (!String.IsNullOrEmpty(coefficientDetail.id_subcentroCtoContab))
                {
                    detalleSaldos = detalleSaldos
                        .Where(s => !String.IsNullOrWhiteSpace(s.id_subcentroCtoContab)
                                    && s.id_subcentroCtoContab.TrimEnd() == coefficientDetail.id_subcentroCtoContab.TrimEnd());
                }
                else
                {
                    detalleSaldos = detalleSaldos
                        .Where(s => String.IsNullOrWhiteSpace(s.id_subcentroCtoContab));
                }

                totalDebe += detalleSaldos.Sum(d => d.amountDebe);
                totalHaber += detalleSaldos.Sum(d => d.amountHaber);
            }

            totalAmount = totalDebe - totalHaber;

            // Recuperamos el modelo actualmente en edición
            var allocationCostPeriod = this.GetEditingAllocationCostPeriod(idAllocationCostPeriod,
                accountingValue, idExecutionType, idCoefficient);

            var allocationCostPeriodDetail = allocationCostPeriod
                .ProductionCostAllocationPeriodDetails?
                .FirstOrDefault();

            if (allocationCostPeriodDetail != null)
            {
                allocationCostPeriodDetail.coeficiente = false;
                allocationCostPeriodDetail.valor = totalAmount;
                isValid = true;
            }

            this.TempData[m_AllocationCostPeriodModelKey] = allocationCostPeriod;
            this.TempData.Keep(m_AllocationCostPeriodModelKey);

            var result = new
            {
                isValid
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Métodos adicionales

        private ProductionCostAllocationPeriod GetEditingAllocationCostPeriod(int? id_allocationCostPeriod,
            bool? accountingValue, int? idExecutionType, int? idCoefficient)
        {
            // Recuperamos el elemento del caché local
            var allocationCostPeriod = (this.TempData[m_AllocationCostPeriodModelKey] as ProductionCostAllocationPeriod);

            // Si no hay elemento en el caché, consultamos desde la base
            if ((allocationCostPeriod == null) && id_allocationCostPeriod.HasValue)
            {
                allocationCostPeriod = db.ProductionCostAllocationPeriod
                    .FirstOrDefault(i => i.id == id_allocationCostPeriod);

                if (allocationCostPeriod != null)
                {
                    accountingValue = accountingValue ?? allocationCostPeriod.accountingValue;
                    idExecutionType = idExecutionType ?? allocationCostPeriod.id_executionType;
                    idCoefficient = idCoefficient ?? allocationCostPeriod.id_coefficient;
                }
            }

            // Si no existe, creamos un nuevo elemento
            if (allocationCostPeriod == null)
            {
                allocationCostPeriod = this.CreateNewAllocationCostPeriod();
            }

            // Si el indicador de cuenta contable está activo, tendremos solo un detalle fijo
            if (accountingValue.HasValue && accountingValue.Value)
            {
                var coefficient = db.ProductionCostCoefficient
                    .FirstOrDefault(c => c.id == idCoefficient);

                if (coefficient != null)
                {
                    // Verificamos si ya existe un detalle con datos actualmente
                    var allocationCostPeriodDetail = allocationCostPeriod
                        .ProductionCostAllocationPeriodDetails?
                        .FirstOrDefault(d => d.id_productionCost == coefficient.id_productionCost
                                            && d.id_productionCostDetail == coefficient.id_productionCostDetail
                                            && d.isActive);

                    // Agregamos el detalle único para la asignación
                    var allocationCostPeriodDetails = new[]
                    {
                        new ProductionCostAllocationPeriodDetail()
                        {
                            id = 1,
                            id_allocationPeriod = id_allocationCostPeriod ?? 0,
                            id_productionCost = coefficient.id_productionCost,
                            id_productionCostDetail = coefficient.id_productionCostDetail,
                            id_productionPlant = coefficient.id_productionPlant,
                            coeficiente = allocationCostPeriodDetail?.coeficiente ?? false,
                            valor = allocationCostPeriodDetail?.valor ?? 0,
                            isActive = true,
                        },
                    };

                    allocationCostPeriod.ProductionCostAllocationPeriodDetails = allocationCostPeriodDetails.ToList();
                }
                else
                {
                    // No hay coeficiente, no hay detalles...
                    allocationCostPeriod.ProductionCostAllocationPeriodDetails.Clear();
                }
            }
            else
            {
                // Verificamos que los detalles son válidos para el tipo de ejecución indicado
                foreach (var allocationCostPeriodDetail in allocationCostPeriod.ProductionCostAllocationPeriodDetails)
                {
                    if (allocationCostPeriodDetail.isActive)
                    {
                        var productionCostExists = db.ProductionCost
                            .Any(d => d.id == allocationCostPeriodDetail.id_productionCost
                                        && d.id_executionType == idExecutionType);

                        if (!productionCostExists)
                        {
                            // Si no corresponde, se inactiva este detalle...
                            allocationCostPeriodDetail.isActive = false;
                        }
                    }
                }

                // Si no se tiene el indicador activo, se usarán los detalles manuales actuales
                allocationCostPeriod.ProductionCostAllocationPeriodDetails = allocationCostPeriod
                    .ProductionCostAllocationPeriodDetails
                    .Where(d => d.isActive)
                    .OrderBy(d => d.id_productionCost).ThenBy(d => d.id_productionCostDetail)
                    .ToList();
            }

            return allocationCostPeriod;
        }

        private ProductionCostAllocationPeriod CreateNewAllocationCostPeriod()
        {
            // Recuperamos el tipo de documento
            var documentType = db.DocumentType
                .FirstOrDefault(dt => dt.code == m_TipoDocumentoAllocationCostPeriod
                                    && dt.id_company == this.ActiveCompanyId);

            if (documentType == null)
            {
                throw new ApplicationException("No existe registrado el tipo de documento: Asignación de Costos.");
            }

            // Recuperamos el estado PENDIENTE
            var documentState = DataProviderAllocationCostPeriod
                .GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

            if (documentState == null)
            {
                throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
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

            // Creamos el documento de asignación de costos
            return new ProductionCostAllocationPeriod()
            {
                Document = document,
                anio = DateTime.Today.Year,
                mes = DateTime.Today.Month,
            };
        }

        private void PrepareEditViewBag(ProductionCostAllocationPeriod allocationCostPeriod)
        {
            // Verificamos el estado actual del documento
            var documentStateCode = allocationCostPeriod.Document.DocumentState.code;
            var documentExists = allocationCostPeriod.id > 0;
            var canEditDocument = (documentStateCode == m_PendienteDocumentState);

            // Agregamos los elementos al ViewBag
            this.ViewBag.DocumentoExistente = documentExists;
            this.ViewBag.PuedeEditarDocumento = canEditDocument;
            this.ViewBag.PuedeAprobarDocumento = documentExists && (documentStateCode == m_PendienteDocumentState);
            this.ViewBag.PuedeReversarDocumento = documentExists && (documentStateCode == m_AprobadoDocumentState);
            this.ViewBag.PuedeAnularDocumento = documentExists && (documentStateCode == m_PendienteDocumentState);

            this.PrepareDetailsEditViewBag(allocationCostPeriod.accountingValue, allocationCostPeriod.id_executionType, canEditDocument);
        }

        private PartialViewResult GetAllocationCostPeriodDetailsPartialView(ProductionCostAllocationPeriod allocationCostPeriod)
        {
            var model = allocationCostPeriod.ProductionCostAllocationPeriodDetails?
                .OrderBy(d => d.id_productionCost).ThenBy(d => d.id_productionCostDetail)
                .ToList() ?? new List<ProductionCostAllocationPeriodDetail>();

            return PartialView("_AllocationCostPeriodDetailsPartial", model);
        }

        private void PrepareDetailsEditViewBag(bool? accountingValue, int? idExecutionType, bool editable)
        {
            // Recuperamos la lista de plantas
            var plantasProceso = db.Person
                .Where(p => p.isActive && p.Rol.FirstOrDefault(r => r.name.Equals("Planta Proceso")) != null)
                .Select(p => new
                {
                    p.id,
                    planta = p.identification_number,
                    name = p.fullname_businessName,
                    processPlant = p.processPlant ?? p.fullname_businessName,
                })
                .ToList();

            // Agregamos los elementos al ViewBag
            this.ViewBag.PlantasProceso = plantasProceso;
            this.ViewBag.IdExecutionType = idExecutionType;
            this.ViewBag.AccountingValue = accountingValue ?? false;
            this.ViewBag.DetailsEditable = editable;
        }

        #endregion
    }
}
