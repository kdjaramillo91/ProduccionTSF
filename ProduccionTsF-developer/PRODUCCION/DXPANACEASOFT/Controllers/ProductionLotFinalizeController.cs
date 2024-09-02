using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.ProductionLotCloseDTO;
using DXPANACEASOFT.Operations;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using EntidadesAuxiliares.SQL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Utilitarios.Logs;
using static DXPANACEASOFT.Services.ServiceInventoryMove;

namespace DXPANACEASOFT.Controllers
{
    public class ProductionLotFinalizeController : DefaultController
    {
        #region Constantes
        private const string m_TipoDocumentoProductionLotFinalize = "166";
        private const string m_PendienteDocumentState = "01";
        private const string m_AprobadoDocumentState = "03";
        private const string m_AnuladoDocumentState = "05";
        private const string m_LotePendienteState = "12";
        private const string m_LoteCerradoState = "13";

        private const string m_ProductionLotFinalizeModelKey = "productionLotFinalize";
        private const string m_procedureRecuperarMovimientosLoteManuales = "spc_RecuperarMovimientoLoteManuales";
        #endregion

        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Métodos de consulta principal
        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult ProductionLotFinalizeResults(
            int? id_documentState, string number, string reference, string numberLot,
            DateTime? startEmissionDate, DateTime? endEmissionDate, bool isCallback = false)
        {
            IList<ProductionLotFinalize> model;

            try
            {
                var idDocumentTyoe = db.DocumentType
                    .FirstOrDefault(e => e.code == m_TipoDocumentoProductionLotFinalize)?
                    .id;

                var documents = db.Document
                    .Where(e => e.id_documentType == idDocumentTyoe)
                    .ToList();

                if (id_documentState.HasValue)
                {
                    documents = documents.Where(e => e.id_documentState == id_documentState).ToList();
                }

                if (!String.IsNullOrEmpty(number))
                {
                    documents = documents.Where(e => e.number.Contains(number)).ToList();
                }

                if (!String.IsNullOrEmpty(reference))
                {
                    documents = documents.Where(e => e.reference.Contains(reference)).ToList();
                }

                if (startEmissionDate.HasValue)
                {
                    documents = documents.Where(e => DateTime.Compare(e.emissionDate.Date, startEmissionDate.Value.Date) >= 0).ToList();
                }

                if (endEmissionDate.HasValue)
                {
                    documents = documents.Where(e => DateTime.Compare(e.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
                }

                model = documents.ToProductionLotFinalize();

                if (!String.IsNullOrEmpty(number))
                {
                    model = model
                        .Where(e => e.NumeroLote.Contains(number))
                        .ToArray();
                }
            }
            catch (Exception ex)
            {
                model = new ProductionLotFinalize[] { };
                throw new Exception("Error al consultar los cierres de lote.", ex);
            }

            // Retornar la vista...
            this.ViewBag.QueryCriteria = new Dictionary<string, object>()
            {
                { "Id_documentState", id_documentState },
                { "Number", number },
                { "Reference", reference },
                { "NumberLot", numberLot },
                { "StartEmissionDate", startEmissionDate.ToIsoDateFormat() },
                { "EndEmissionDate", endEmissionDate.ToIsoDateFormat() },
            };

            // Retornamos la fecha
            if (isCallback)
            {
                return this.PartialView("_QueryGridViewPartial", model);
            }
            else
            {
                return this.PartialView("_QueryResultsPartial", model);
            }
        }
        #endregion

        #region Métodos de consulta para crear nuevo        
        public PartialViewResult GenerarNuevo(int[] idsLote)
        {
            ProductionLotFinalize.Detallado modelo;

            try
            {
                if (idsLote == null)
                {
                    throw new Exception("Listado de lotes es nulo.");
                }
                var solicitante = "";
                var documentState = db.DocumentState.FirstOrDefault(e => e.code == m_PendienteDocumentState);
                var documentType = db.DocumentType.FirstOrDefault(e => e.code == m_TipoDocumentoProductionLotFinalize);
                var loteid = idsLote[0];
                var lote = "";
                var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == loteid);
                var lot = db.Lot.FirstOrDefault(a => a.id == loteid);
                if (lot != null && lot.internalNumber != null)
                {
                    lote = lot.internalNumber.Substring(0,5);
                }
                else
                {
                    lote = productionLot.internalNumber.Substring(0, 5);
                }


                var user = db.User.FirstOrDefault(fod => fod.id == this.ActiveUserId);
                if (user != null)
                {
                    solicitante = db.Person.FirstOrDefault(fod => fod.id == user.id_employee).fullname_businessName;
                }
                modelo = new ProductionLotFinalize.Detallado()
                {
                    FechaEmision = DateTime.Today,
                    DocumentType = documentType.name,
                    Estado = documentState.name,
                    PuedeEditarse = true,
                    IdsLote = idsLote,
                    Solicitante = solicitante,
                };

                modelo.SetDetailLot();
                modelo.SetDetailKardex();
                ViewBag.lote = lote;
            }
            catch (Exception ex)
            {
                modelo = new ProductionLotFinalize.Detallado();
                throw new Exception("Error al generar cierre de lote.", ex.GetBaseException());
            }

            
            ViewBag.PuedeEditarDocumento = true;
            ViewBag.DocumentoExistente = false;
            ViewBag.PuedeAprobarDocumento = false;
            ViewBag.PuedeAnularDocumento = false;
            ViewBag.PuedeReversarDocumento = false;


            return this.PartialView("_FormEditProductionLotFinalize", modelo);
        }

        [HttpPost]
        public JsonResult Create(DateTime emissionDate,
            string reference, string description,
            int[] idsLote)
        {
            int? idProductionLotFinalize;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperamos el tipo de documento
                    var documentType = db.DocumentType
                        .FirstOrDefault(dt => dt.code == m_TipoDocumentoProductionLotFinalize
                                            && dt.id_company == this.ActiveCompanyId);

                    if (documentType == null)
                    {
                        throw new ApplicationException("No existe registrado el tipo de documento: Cierre de Lote.");
                    }

                    // Recuperamos el estado PENDIENTE
                    var documentState = DataProviderProductionLotFinalize
                        .GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

                    if (documentState == null)
                    {
                        throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
                    }

                    //Recuperamos el estado de lote Pendiente Cierre
                    var productionLotState = db.ProductionLotState.FirstOrDefault(a => a.code == m_LotePendienteState);

                    if (productionLotState == null)
                    {
                        throw new ApplicationException("No existe registrado el estado PENDIENTE DE CIERRE para los documentos.");
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

                    // Creamos el documento de Cierre de Lote
                    var list = new List<ProductionLotClose>();
                    foreach (var idLote in idsLote)
                    {
                        var dataLot = GetLotDataFromTempData(idLote);
                        var NumeroLote = "";
                        var NumeroInterno = "";
                        if (dataLot.NumeroLote == null)
                        {
                            NumeroLote = db.Lot.FirstOrDefault(fod => fod.id == idLote).internalNumber;
                            NumeroInterno = db.Lot.FirstOrDefault(fod => fod.id == idLote).number;
                        }
                        list.Add(new ProductionLotClose()
                        {
                            id_document = document.id,
                            tipo = dataLot.TipoLote,
                            id_lot = dataLot.IdLote,
                            internalNumber = (dataLot.NumeroLote == null) ? NumeroLote : dataLot.NumeroInterno,
                            number = (dataLot.NumeroLote == null) ? NumeroInterno : dataLot.NumeroLote,
                            id_lotState = productionLotState.id,
                            isActive = true,

                            id_userCreate = this.ActiveUserId,
                            dateCreate = DateTime.Now,
                            id_userUpdate = this.ActiveUserId,
                            dateUpdate = DateTime.Now,

                        });

                        // Actualizar isclosed ProductionLot
                        var tipoLote = dataLot.TipoLote == "PRD";
                        if (tipoLote)
                        {
                            var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == dataLot.IdLote);
                            productionLot.isClosed = true;
                            var lot = db.Lot.FirstOrDefault(a => a.id == dataLot.IdLote);
                            if(lot != null)
                            {
                                lot.isClosed = true;
                                db.Lot.Attach(lot);
                                db.Entry(lot).State = EntityState.Modified;
                            }

                            db.ProductionLot.Attach(productionLot);
                            db.Entry(productionLot).State = EntityState.Modified;
                        }
                        
                    }

                    // Guardamos el documento
                    db.Document.Add(document);
                    db.ProductionLotClose.AddRange(list);
                    db.SaveChanges();
                    transaction.Commit();

                    idProductionLotFinalize = document.id;
                    message = "Documento creado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_ProductionLotFinalizeModelKey);

                    var exceptionMessage = exception.GetBaseException() != null
                        ? exception.GetBaseException().Message
                        : "No se ha podido recuperar los detalles del error";

                    idProductionLotFinalize = null;
                    message = "Error al crear documento: " + exceptionMessage;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductionLotFinalize,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(int idProductionLotFinalize,
        DateTime emissionDate, string description, string reference, int[] idsLote)
        {
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var productionLotClose = db.ProductionLotClose
                        .First(c => c.id_document == idProductionLotFinalize && c.Document.DocumentState.code != "05");

                    // Verificamos el estado actual del documento
                    var documentStateCode = productionLotClose.Document.DocumentState.code;

                    if (documentStateCode != m_PendienteDocumentState)
                    {
                        throw new ApplicationException("Acción es permitida solo para Documento en estado PENDIENTE.");
                    }

                    // Actualizamos el documento
                    var document = db.Document.FirstOrDefault(a => a.id == productionLotClose.id_document);

                    document.emissionDate = emissionDate;
                    document.description = description;
                    document.reference = reference;

                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;

                    // Actualizamos el documento de Cierre de Lote

                    productionLotClose.id_userUpdate = this.ActiveUserId;
                    productionLotClose.dateUpdate = DateTime.Now;

                    // Guardamos en la base de datos
                    db.Document.Attach(document);
                    db.Entry(document).State = EntityState.Modified;
                    db.ProductionLotClose.Attach(productionLotClose);
                    db.Entry(productionLotClose).State = EntityState.Modified;

                    // Guardamos el documento
                    db.SaveChanges();
                    transaction.Commit();
                    message = "Documento actualizado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_ProductionLotFinalizeModelKey);

                    var exceptionMessage = exception.GetBaseException() != null
                        ? exception.GetBaseException().Message
                        : "No se ha podido recuperar los detalles del error";

                    message = "Error al actualizar documento: " + exceptionMessage;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductionLotFinalize,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Aprove(int idProductionLotFinalize)
        {
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var productionLotCloses = db.ProductionLotClose
                        .Where(c => c.id_document == idProductionLotFinalize && c.Document.DocumentState.code != "05").ToList();

                    var productionLotClose = productionLotCloses.FirstOrDefault();

                    //foreach (var lote in productionLotCloses)
                    //{
                    //    var lot = db.Lot.FirstOrDefault(a => a.id == lote.id_lot);
                    //    var productuonLot = db.ProductionLot.FirstOrDefault(a => a.id == lote.id_lot);
                    //    var secTransaccional = lot != null ? lot.number : productuonLot.number;
                    //    var wareHouseids = db.Warehouse.Where(a => a.WarehouseType.excludeClosure == true && a.isActive).ToList();
                    //    int[] idsWareHouse = wareHouseids.Select(i => i.id).ToArray();
                        
                    //    var inventoryMoveDetails = db.InventoryMoveDetail.Where(a => a.id_lot == lote.id_lot
                    //                                && a.InventoryMove.Document.DocumentState.code != "05"
                    //                                && a.Warehouse.code != "VIREMP"
                    //                                && !idsWareHouse.Contains(a.id_warehouse)).ToList();

                    //    if(inventoryMoveDetails != null)
                    //    {
                    //        foreach (var i in inventoryMoveDetails)
                    //        {
                    //                //Verificamos el saldo
                    //                var remainingBalance = ServiceInventoryMove.GetRemainingBalance(i.id_item, i.id_warehouse, i.id_warehouseLocation, i.id_lot, db);

                    //                if (remainingBalance < 0)
                    //                {
                    //                    throw new Exception("No puede Aprobar el cierre de lote debido a que no hay suficiente Stock y este quedara en negativo, para el producto: " +
                    //                                        i.Item.name + ", en la Bodega: " + i.Warehouse.name + ", en la Ubicación: " + i.WarehouseLocation.name +
                    //                                        ", perteneciente al Lote: " + secTransaccional);
                    //                }
                    //                else if (remainingBalance > 0) 
                    //                {
                    //                    throw new Exception("No puede Aprobar el cierre de lote debido a que el lote tiene saldo, para el producto: " +
                    //                                        i.Item.name + ", en la Bodega: " + i.Warehouse.name + ", en la Ubicación: " + i.WarehouseLocation.name +
                    //                                        ", perteneciente al Lote: " + secTransaccional);
                    //                }                                
                    //        }

                    //    }

                       
                    //}
                  


                    // Verificamos el estado actual del documento
                    var documentStateCode = productionLotClose.Document.DocumentState.code;

                    if (documentStateCode != m_PendienteDocumentState)
                    {
                        throw new ApplicationException("Acción es permitida solo para Documento en estado PENDIENTE.");
                    }

                    // Actualizamos el documento
                    var document = db.Document.FirstOrDefault(a => a.id == productionLotClose.id_document);
                    var documentState = db.DocumentState.FirstOrDefault(e => e.code == m_AprobadoDocumentState);
                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;
                    document.id_documentState = documentState.id;

                    // Actualizamos el documento de Cierre de Lote
                    productionLotClose.id_userUpdate = this.ActiveUserId;
                    productionLotClose.dateUpdate = DateTime.Now;


                    ////Actualizamos el estado del lote
                    //var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == dataLot.IdLote);
                    //productionLot.isClosed = true;

                    //db.ProductionLot.Attach(productionLot);
                    //db.Entry(productionLot).State = EntityState.Modified;

                    // Guardamos en la base de datos
                    db.Document.Attach(document);
                    db.Entry(document).State = EntityState.Modified;
                    db.ProductionLotClose.Attach(productionLotClose);
                    db.Entry(productionLotClose).State = EntityState.Modified;

                    // Guardamos el documento
                    db.SaveChanges();
                    transaction.Commit();
                    message = "Documento Aprobado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_ProductionLotFinalizeModelKey);

                    var exceptionMessage = exception.GetBaseException() != null
                        ? exception.GetBaseException().Message
                        : "No se ha podido recuperar los detalles del error";

                    message = "Error al aprobar documento: " + exceptionMessage;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductionLotFinalize,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Revert(int idProductionLotFinalize)
        {
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var productionLotCloses = db.ProductionLotClose
                        .Where(c => c.id_document == idProductionLotFinalize && c.Document.DocumentState.code != "05");

                    var productionLotClose = productionLotCloses.FirstOrDefault();

                    // Verificamos el estado actual del documento
                    var documentStateCode = productionLotClose.Document.DocumentState.code;

                    if (documentStateCode != m_AprobadoDocumentState)
                    {
                        throw new ApplicationException("Acción es permitida solo para Documento en estado APROBADO.");
                    }

                    // Actualizamos el documento
                    var document = db.Document.FirstOrDefault(a => a.id == productionLotClose.id_document);
                    var documentState = db.DocumentState.FirstOrDefault(e => e.code == m_PendienteDocumentState);
                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;
                    document.id_documentState = documentState.id;

                    // Actualizamos el documento de Cierre de Lote
                    productionLotClose.id_userUpdate = this.ActiveUserId;
                    productionLotClose.dateUpdate = DateTime.Now;


                    ////Actualizamos el estado del lote
                    //var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == dataLot.IdLote);
                    //productionLot.isClosed = true;

                    //db.ProductionLot.Attach(productionLot);
                    //db.Entry(productionLot).State = EntityState.Modified;

                    // Guardamos en la base de datos
                    db.Document.Attach(document);
                    db.Entry(document).State = EntityState.Modified;
                    db.ProductionLotClose.Attach(productionLotClose);
                    db.Entry(productionLotClose).State = EntityState.Modified;

                    // Guardamos el documento
                    db.SaveChanges();
                    transaction.Commit();
                    message = "Documento Reversado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_ProductionLotFinalizeModelKey);

                    var exceptionMessage = exception.GetBaseException() != null
                        ? exception.GetBaseException().Message
                        : "No se ha podido recuperar los detalles del error";

                    message = "Error al reversar documento: " + exceptionMessage;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductionLotFinalize,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Cancel(int idProductionLotFinalize)
        {
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var productionLotCloses = db.ProductionLotClose
                        .Where(c => c.id_document == idProductionLotFinalize && c.Document.DocumentState.code != "05");

                    var productionLotClose = productionLotCloses.FirstOrDefault();

                    // Actualizamos el documento
                    var document = db.Document.FirstOrDefault(a => a.id == productionLotClose.id_document);
                    var documentState = db.DocumentState.FirstOrDefault(e => e.code == m_AnuladoDocumentState);
                    document.id_userUpdate = this.ActiveUserId;
                    document.dateUpdate = DateTime.Now;
                    document.id_documentState = documentState.id;

                    // Actualizamos el documento de Cierre de Lote
                    productionLotClose.id_userUpdate = this.ActiveUserId;
                    productionLotClose.dateUpdate = DateTime.Now;


                    //Actualizamos el estado del lote
                    foreach (var lote in productionLotCloses)
                    {
                        var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == lote.id_lot);
                        if(productionLot != null)
                        {
                            productionLot.isClosed = false;
                            db.ProductionLot.Attach(productionLot);
                            db.Entry(productionLot).State = EntityState.Modified;
                        }
                            
                        var lot = db.Lot.FirstOrDefault(a => a.id == lote.id_lot);
                        if (lot != null)
                        {
                            lot.isClosed = false;
                            db.Lot.Attach(lot);
                            db.Entry(lot).State = EntityState.Modified;
                        }
                    }

                    // Guardamos en la base de datos
                    db.Document.Attach(document);
                    db.Entry(document).State = EntityState.Modified;
                    db.ProductionLotClose.Attach(productionLotClose);
                    db.Entry(productionLotClose).State = EntityState.Modified;

                    // Guardamos el documento
                    db.SaveChanges();
                    transaction.Commit();
                    message = "Documento Anulado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_ProductionLotFinalizeModelKey);

                    var exceptionMessage = exception.GetBaseException() != null
                        ? exception.GetBaseException().Message
                        : "No se ha podido recuperar los detalles del error";

                    message = "Error al anular documento: " + exceptionMessage;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductionLotFinalize,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintProductionLotFinalizeReport(int idProductionLotFinalize)
        {
            var productionLotFinalize = this.GetEditingProductionLotFinalize(idProductionLotFinalize);

            #region "Armo Parametros"
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = productionLotFinalize.Id;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "CIELOT";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }
        public PartialViewResult ProductionLotPendingCloseResults(bool isCallback = false)
        {
            IList<LotePendienteCierre> model;

            try
            {
                // Si no es callback, recuperamos la información desde la base y grabamos memoria temporal
                if (!isCallback)
                {
                    this.TempData.Remove("PendingLotQuery");

                    // Procesamos los lotes de produccion
                    var lotesPendientes = new List<LotePendienteCierre>();
                    var codesProductionLotState = new[] { "09", "12", "13", "100" };
                    var productionsLot = db.ProductionLot
                        .Where(e => !codesProductionLotState.Contains(e.ProductionLotState.code) && !e.isClosed)
                        .ToArray();

                    var saldos = new ServiceInventoryMove();
                    var lotesPendienteProduccion = productionsLot
                        .Select(e => new LotePendienteCierre()
                        {
                            IdLote = e.id,
                            TipoLote = "Producción",
                            SecuenciaTransaccional = e.number,
                            NumeroLote = e.internalNumber,
                            LoteJuliano = (e.internalNumber != null && e.internalNumber.Length > 5)
                                ? e.internalNumber.Substring(0, 5) : string.Empty,
                            UnidadProduccion = e.ProductionUnit?.name,
                            Proceso = e.Person1?.processPlant,
                            FechaProceso = e.receptionDate,
                            Estado = e.ProductionLotState.name,
                            Stock = saldos.GetSaldosProductoLoteFinalize(this.ActiveCompanyId,e.id)
                        })
                        .ToList();
                    lotesPendientes.AddRange(lotesPendienteProduccion);

                    //// Procesamos los lotes de inventarios
                    //var lotesPendientesManuales = this.GetDataProcesarInventario().ToList();
                    //lotesPendientesManuales.ForEach(e =>
                    //{
                    //    e.TipoLote = "Manual";
                    //    e.Stock = saldos.GetSaldosProductoLote(e.IdLote);
                    //});
                    //lotesPendientes.AddRange(lotesPendientesManuales);
                    model = lotesPendientes.ToArray();

                    // Guardamos la memoriaTemporal
                    this.TempData["PendingLotQuery"] = model;
                }
                else
                {
                    model = (this.TempData["PendingLotQuery"] as LotePendienteCierre[]);
                    this.TempData.Keep("PendingLotQuery");
                }
            }
            catch (Exception ex)
            {
                model = new LotePendienteCierre[] { };
                throw new Exception("Error al consultar los lotes pendientes de cierre.", ex);
            }

            var agrupaciones = model
                .GroupBy(e => e.LoteJuliano)
                .Select(e => new {
                    LoteJuliano = e.Key,
                    NumerosLote = e.Count(),
                })
                .ToArray();

            this.ViewBag.agrupacionLotes = agrupaciones;

            // Retornamos la fecha
            if (isCallback)
            {
                return this.PartialView("_QueryLotPendingGridViewPartial", model);
            }
            else
            {
                return this.PartialView("_QueryLotPendingResultsPartial", model);
            }
        }
        #endregion

        #region Métodos de edición
        [HttpPost]
        public PartialViewResult EditForm(int? id, string successMessage)
        {
            this.TempData.Remove(m_ProductionLotFinalizeModelKey);

            var productionLotFinalize = this.GetEditingProductionLotFinalize(id);
            this.TempData[m_ProductionLotFinalizeModelKey] = productionLotFinalize;
            this.ViewBag.lote = productionLotFinalize.NumeroLote.Substring(0, 5);
            this.PrepareEditViewBag(productionLotFinalize);

            if (!String.IsNullOrWhiteSpace(successMessage))
            {
                this.ViewBag.EditMessage = successMessage;
            }

            return PartialView("_FormEditProductionLotFinalize", productionLotFinalize);
        }
        #endregion

        #region Metodos de Gestión de Detalles
        [HttpPost]
        public PartialViewResult KardexPartial(DateTime? emissionDate, string internalNumberLot, int idproductLotFinalize)
        {
            ProductionLotFinalize.Detallado modelo;
            try
            {
                if (emissionDate == null)
                {
                    emissionDate = db.Document.FirstOrDefault(a => a.id == idproductLotFinalize).emissionDate;
                }
                modelo = new ProductionLotFinalize.Detallado()
                {
                    FechaEmision = emissionDate ?? DateTime.Now,
                    NumeroLote = internalNumberLot,
                };
                modelo.SetDetailKardex();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar cierre de lote.", ex.GetBaseException());
            }

            return PartialView("_DetailResulKardexPartialTabPage", modelo.DetallesKardex);
        }
        #endregion

        #region Métodos Adicionales
        private ProductionLotFinalize.Detallado GetEditingProductionLotFinalize(int? id_productionLotFinalize)
        {
            // Recuperamos el elemento del caché local
            var productionLotFinalize = (this.TempData[m_ProductionLotFinalizeModelKey] as ProductionLotFinalize.Detallado);

            // Si no hay elemento en el caché, consultamos desde la base
            if ((productionLotFinalize == null) && id_productionLotFinalize.HasValue)
            {
                var document = db.Document.FirstOrDefault(e => e.id == id_productionLotFinalize);

                productionLotFinalize = document.ToProductionLotFinalizeDetalled();
            }

            // Si no existe, creamos un nuevo elemento
            return productionLotFinalize ?? new ProductionLotFinalize.Detallado();
        }
        #endregion

        #region Ejecución de Procedimientos Almacenados
        private LotePendienteCierre[] GetDataProcesarInventario()
        {
            // Obtenemos la data
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            DataSet dataSet = AccesoDatos.MSSQL.MetodosDatos2
                .ObtieneDatos(_cadenaConexion, m_procedureRecuperarMovimientosLoteManuales, _rutaLog,
                    m_ProductionLotFinalizeModelKey, "PROD", lstParametersSql);

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                var resultados = dataSet.Tables[0].AsEnumerable();

                return resultados
                    .Select(e => new LotePendienteCierre()
                    {
                        IdLote = e.Field<Int32>("IdLote"),
                        SecuenciaTransaccional = e.Field<String>("SecuenciaTransaccional"),
                        NumeroLote = e.Field<String>("NumeroLote"),
                        LoteJuliano = e.Field<String>("LoteJuliano"),
                        FechaProceso = e.Field<DateTime>("FechaProceso"),
                        Proceso = e.Field<String>("Proceso"),
                        Estado = e.Field<String>("Estado"),
                    })
                    .ToArray();
            }
            else
            {
                return new LotePendienteCierre[] { };
            }
        }
        private ProductionLotFinalize.DetalleLote[] PrepararDetalleLoteDTO(int[] idsLote)
        {
            var lotes = new List<ProductionLotFinalize.DetalleLote>();
            var orden = 1;
            foreach (var idLote in idsLote)
            {
                var lotData = GetLotDataFromTempData(idLote);
                lotes.Add(new ProductionLotFinalize.DetalleLote()
                {
                    Orden = orden++,
                    NumeroLote = lotData.NumeroLote,
                    SecuenciaTransaccional = lotData.NumeroInterno,
                    Cantidad = lotData.CantidadMaster,
                    CantidadKilos = lotData.CantidadKilos,
                    CantidadLibras = lotData.CantidadLibras,
                });
            }
            return lotes.ToArray();
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
                var item = db.Item.FirstOrDefault(e => e.id == idItem);
                TempData[key] = item;
                TempData.Keep(key);

                return item;
            }
        }
        private class LotData
        {
            public int IdLote { get; set; }
            public string TipoLote { get; set; }
            public string NumeroLote { get; set; }
            public string NumeroInterno { get; set; }
            public decimal CantidadMaster { get; set; }
            public decimal CantidadLibras { get; set; }
            public decimal CantidadKilos { get; set; }
        }
        
        private LotData GetLotDataFromTempData(int idLote)
        {
            var key = $"LoteData_{idLote}";
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as LotData;
            }
            else
            {
                LotData data = null;
                var codigosEstados = new[] { "03", "16" };
                var productionLot = db.ProductionLot.FirstOrDefault(e => e.id == idLote);
                //var inventoryMoveDetail = db.InventoryMoveDetail.Where(a => codigosEstados.Contains(a.InventoryMove.Document.DocumentState.code)
                //                            && a.id_lot == idLote).ToList();

                if(productionLot != null)
                {
                    data = new LotData()
                    {
                        IdLote = idLote,
                        TipoLote = "PRD",
                        NumeroLote = productionLot.internalNumber,
                        NumeroInterno = productionLot.number,
                        CantidadMaster = productionLot.ProductionLotLiquidation.Sum(s => s.quantity),
                        CantidadKilos = productionLot.ProductionLotLiquidation.Sum(s => s.quantityTotal.Value),
                        CantidadLibras = productionLot.ProductionLotLiquidation.Sum(s => s.quantityPoundsLiquidation.Value)
                    };
                }
                else
                {
                    var lot = db.Lot.FirstOrDefault(e => e.id == idLote);
                    data = new LotData()
                    {
                        IdLote = idLote,
                        TipoLote = "MAN",
                        NumeroLote = lot.internalNumber,
                        NumeroInterno = lot.number,
                        //CantidadMaster = productionLot.ProductionLotLiquidation.Sum(s => s.quantity),
                    };
                }

                TempData[key] = data;
                TempData.Keep(key);

                return data;
            }
        }
        private void PrepareEditViewBag(ProductionLotFinalize.Detallado ProductionLotFinalize)
        {
            // Verificamos el estado actual del documento
            var productionLotClose = db.Document.FirstOrDefault(a => a.id == ProductionLotFinalize.Id);
            var documentStateCode = productionLotClose.DocumentState.code;
            var documentExists = productionLotClose.id > 0;
            var canEditDocument = (documentStateCode == m_PendienteDocumentState);


            // Agregamos los elementos al ViewBag
            this.ViewBag.DocumentoExistente = documentExists;
            this.ViewBag.PuedeEditarDocumento = canEditDocument;
            this.ViewBag.PuedeAprobarDocumento = documentExists && (documentStateCode == m_PendienteDocumentState);
            this.ViewBag.PuedeReversarDocumento = documentExists && (documentStateCode == m_AprobadoDocumentState);
            this.ViewBag.PuedeAnularDocumento = documentExists && (documentStateCode == m_PendienteDocumentState);

        }

        #endregion
    }
}