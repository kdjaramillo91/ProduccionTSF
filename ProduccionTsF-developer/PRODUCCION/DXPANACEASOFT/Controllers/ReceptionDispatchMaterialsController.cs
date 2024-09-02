using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.Models.Helpers;
using DXPANACEASOFT.Models.InventoryMoveDTO;
using DXPANACEASOFT.Reports.PurchasePlanning;
using DXPANACEASOFT.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utilitarios.General;
using Utilitarios.ProdException;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ReceptionDispatchMaterialsController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region Reception Dispatch Materials EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult ReceptionDispatchMaterialsFormEditPartial(int id, int? id_remissionGuide)
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = db.ReceptionDispatchMaterials.FirstOrDefault(r => r.id == id);

            if (receptionDispatchMaterials == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("72"));//Recepción de Materiales 72
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");//Estado Pendiente 01

                Employee employee = ActiveUser.Employee;

                receptionDispatchMaterials = new ReceptionDispatchMaterials
                {
                    Document = new Document
                    {
                        id = 0,
                        id_documentType = documentType?.id ?? 0,
                        DocumentType = documentType,
                        id_documentState = documentState?.id ?? 0,
                        DocumentState = documentState,
                        emissionDate = DateTime.Now
                    },
                    ReceptionDispatchMaterialsDetail = new List<ReceptionDispatchMaterialsDetail>()
                };

                if (id_remissionGuide != null)
                {
                    receptionDispatchMaterials.id_remissionGuide = id_remissionGuide.Value;
                    receptionDispatchMaterials.RemissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == id_remissionGuide);

                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            ServiceInventoryMove.GetWarehouseLocationProvider(receptionDispatchMaterials.RemissionGuide.id_providerRemisionGuide, db, ActiveCompany, ActiveUser);
                            db.SaveChanges();
                            trans.Commit();
                        }
                        catch (Exception e)
                        {
                            trans.Rollback();
                            throw e;
                        }
                    }
                }
            }
            if (id_remissionGuide != null)
            {
                SetDefaultQuantities(ref receptionDispatchMaterials);
            }

            TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");

            return PartialView("_FormEditReceptionDispatchMaterials", receptionDispatchMaterials);
        }

        private void SetDefaultQuantities(ref ReceptionDispatchMaterials model)
        {
            if (model.RemissionGuide == null) return;
            if (model.RemissionGuide.RemissionGuideDispatchMaterial == null) return;

            foreach (var rec in model.RemissionGuide.RemissionGuideDispatchMaterial)
            {
                if (rec.Item.isConsumed)
                {
                    rec.amountConsumed = rec.sendedDestinationQuantity;
                }
                else
                {
                    rec.arrivalDestinationQuantity = rec.sendedDestinationQuantity;
                    rec.arrivalGoodCondition = rec.sendedDestinationQuantity;
                }
            }
        }

        #endregion Reception Dispatch Materials EDITFORM

        #region ResultGridView

        [ValidateInput(false)]
        public ActionResult ReceptionDispatchMaterialsResultsPartial(int? id_documentState, string number, int? id_provider, //Document
                                                                     DateTime? startEmissionDate, DateTime? endEmissionDate, //Emission
                                                                     DateTime? startDateDispatch, DateTime? endDateDispatch, DateTime? startPlantExitDate, DateTime? endPlantExitDate, DateTime? startPlantEntryDate, DateTime? endPlantEntryDate, string numberGuia)//Logistics
        {
            List<ReceptionDispatchMaterials> model = db.ReceptionDispatchMaterials.ToList();

            #region Document

            //id_documentState
            if (id_documentState != 0 && id_documentState != null)
            {
                model = model.Where(o => o.Document.id_documentState == id_documentState).ToList();
            }

            //number
            if (!string.IsNullOrEmpty(number))
            {
                model = model.Where(o => o.Document.number.Contains(number)).ToList();
            }

            //id_provider
            if (id_provider != 0 && id_provider != null)
            {
                model = model.Where(o => o.RemissionGuide.id_providerRemisionGuide == id_provider).ToList();
            }

            #endregion Document

            #region Emission

            //startEmissionDate
            if (startEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
            }

            //endEmissionDate
            if (endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            #endregion Emission

            #region Logistics

            //startDateDispatch
            if (startDateDispatch != null)
            {
                model = model.Where(o => DateTime.Compare(startDateDispatch.Value.Date, o.RemissionGuide.despachureDate.Date) <= 0).ToList();
            }

            //endDateDispatch
            if (endDateDispatch != null)
            {
                model = model.Where(o => DateTime.Compare(o.RemissionGuide.despachureDate.Date, endDateDispatch.Value.Date) <= 0).ToList();
            }

            //startPlantExitDate
            if (startPlantExitDate != null)
            {
                model = model.Where(o => (o.RemissionGuide.RemissionGuideControlVehicle != null && o.RemissionGuide.RemissionGuideControlVehicle.exitDateProductionBuilding != null) &&
                                         (DateTime.Compare(startPlantExitDate.Value.Date, o.RemissionGuide.RemissionGuideControlVehicle.exitDateProductionBuilding.Value.Date) <= 0)).ToList();
            }

            //endPlantExitDate
            if (endPlantExitDate != null)
            {
                model = model.Where(o => (o.RemissionGuide.RemissionGuideControlVehicle != null && o.RemissionGuide.RemissionGuideControlVehicle.exitDateProductionBuilding != null) &&
                                         (DateTime.Compare(o.RemissionGuide.RemissionGuideControlVehicle.exitDateProductionBuilding.Value.Date, endPlantExitDate.Value.Date) <= 0)).ToList();
            }

            //startPlantEntryDate
            if (startPlantEntryDate != null)
            {
                model = model.Where(o => (o.RemissionGuide.RemissionGuideControlVehicle != null && o.RemissionGuide.RemissionGuideControlVehicle.entranceDateProductionBuilding != null) &&
                                         (DateTime.Compare(startPlantEntryDate.Value.Date, o.RemissionGuide.RemissionGuideControlVehicle.entranceDateProductionBuilding.Value.Date) <= 0)).ToList();
            }

            //endPlantEntryDate
            if (endPlantEntryDate != null)
            {
                model = model.Where(o => (o.RemissionGuide.RemissionGuideControlVehicle != null && o.RemissionGuide.RemissionGuideControlVehicle.entranceDateProductionBuilding != null) &&
                                         (DateTime.Compare(o.RemissionGuide.RemissionGuideControlVehicle.entranceDateProductionBuilding.Value.Date, endPlantEntryDate.Value.Date) <= 0)).ToList();
            }

            //numberGuia
            if (!string.IsNullOrEmpty(numberGuia))
            {
                model = model.Where(o => o.RemissionGuide.Document.number.Contains(numberGuia)).ToList();
            }

            #endregion Logistics

            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var tempModel = new List<ReceptionDispatchMaterials>();
                    foreach (var item in model)
                    {
                        var inventoryMoveDetail = item.ReceptionDispatchMaterialsDetail
                                                        .FirstOrDefault(fod => entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == fod.id_warehouse
                                                        && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) == null);
                        if (inventoryMoveDetail == null)
                        {
                            tempModel.Add(item);
                        }
                    }

                    model = tempModel;

                    var tempModel2 = new List<ReceptionDispatchMaterials>();
                    foreach (var item in model)
                    {
                        var inventoryMoveDetail = item.RemissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(fod => entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == fod.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) == null);
                        if (inventoryMoveDetail == null)
                        {
                            tempModel2.Add(item);
                        }
                    }

                    model = tempModel2;
                }
            }

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_ReceptionDispatchMaterialsResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuidesResults()
        {
            string[] orderShippingCodes = new string[] { "T", "M", "TF" };
            string[] documentStateCodes = new string[] { "06", "08", "09" };
            string[] documentStateCodesOp2 = new string[] { "03", "06" };

            var preGuiaRemision = db.RemissionGuide.Where(d => ((!d.RemissionGuideTransportation.isOwn && d.PurchaseOrderShippingType != null &&
                                                     (orderShippingCodes.Contains(d.PurchaseOrderShippingType.code)) &&
                                                     (documentStateCodes.Contains(d.Document.DocumentState.code))) ||
                                                     (d.RemissionGuideTransportation.isOwn && d.PurchaseOrderShippingType != null && d.PurchaseOrderShippingType.code == "T" &&
                                                      (documentStateCodesOp2.Contains(d.Document.DocumentState.code)))) &&
                                                      d.hasEntrancePlanctProduction != null && d.hasEntrancePlanctProduction.Value &&
                                                      d.ReceptionDispatchMaterials.FirstOrDefault(fod => fod.Document.DocumentState.code != "05") == null)//"06" AUTORIZADA, "08" CANCELADA, "09" REASIGNADA, "05" ANULADO
                                                .ToList();
            var guiaRemision = preGuiaRemision.OrderByDescending(d => d.id).ToList();

            var requestInventory = db.Document.Where(d => d.DocumentState.code == "01" && d.DocumentType.code.Equals("79")).Select(a => a.id).ToList();

            var documentSource = db.DocumentSource
                                        .Where(d => requestInventory.Contains(d.id_document)).Select(a => a.id_documentOrigin).ToList();

            var model = guiaRemision.Where(d => !documentSource.Contains(d.id)).ToList();

            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var tempModel = new List<RemissionGuide>();
                    foreach (var item in model)
                    {
                        var inventoryMoveDetail = item.RemissionGuideDispatchMaterial.FirstOrDefault(fod => entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == fod.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) == null);
                        if (inventoryMoveDetail == null)
                        {
                            tempModel.Add(item);
                        }
                    }

                    model = tempModel;
                }
            }

            return PartialView("_RemissionGuidesResultsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuidesPartial()
        {
            string[] orderShippingCodes = new string[] { "T", "M", "TF" };
            string[] documentStateCodes = new string[] { "03", "06", "08", "09" };
            string[] documentStateCodesOp2 = new string[] { "03", "06" };

            var preGuiaRemision = db.RemissionGuide.Where(d => (
                                                                (!d.RemissionGuideTransportation.isOwn
                                                                  && d.PurchaseOrderShippingType != null &&
                                                                  (orderShippingCodes.Contains(d.PurchaseOrderShippingType.code)
                                                                  ) &&
                                                                 (documentStateCodes.Contains(d.Document.DocumentState.code))
                                                                )
                                                                ||
                                                                (d.RemissionGuideTransportation.isOwn
                                                                  && d.PurchaseOrderShippingType != null
                                                                  && d.PurchaseOrderShippingType.code == "T"
                                                                  && (documentStateCodesOp2.Contains(d.Document.DocumentState.code))
                                                                )
                                                             )
                                                             &&
                                                             d.hasEntrancePlanctProduction != null && d.hasEntrancePlanctProduction.Value
                                                             && d.ReceptionDispatchMaterials.FirstOrDefault(fod => fod.Document.DocumentState.code != "05") == null)//"06" AUTORIZADA, "08" CANCELADA, "09" REASIGNADA, "05" ANULADO
                                                    .ToList();
            var guiaRemision = preGuiaRemision.OrderByDescending(d => d.id).ToList();

            var requestInventory = db.Document.Where(d => d.DocumentState.code == "01" && d.DocumentType.code.Equals("79")).Select(a => a.id).ToList();

            var documentSource = db.DocumentSource
                            .Where(d => requestInventory.Contains(d.id_document)).Select(a => a.id_documentOrigin).ToList();

            var model = guiaRemision.Where(d => !documentSource.Contains(d.id)).ToList();

            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var tempModel = new List<RemissionGuide>();
                    foreach (var item in model)
                    {
                        var inventoryMoveDetail = item.RemissionGuideDispatchMaterial
                                                        .FirstOrDefault(fod => entityPermissions.listValue
                                                                            .FirstOrDefault(fod2 => fod2.id_entityValue == fod.id_warehouse
                                                                            && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) == null);
                        if (inventoryMoveDetail == null)
                        {
                            tempModel.Add(item);
                        }
                    }

                    model = tempModel;
                }
            }

            return PartialView("_RemissionGuidesPartial", model.ToList());
        }

        #endregion ResultGridView

        #region Reception Dispatch Materials

        [HttpPost]
        public ActionResult ReceptionDispatchMaterialsPartial()
        {
            var model = (TempData["model"] as List<ReceptionDispatchMaterials>);
            model = model ?? new List<ReceptionDispatchMaterials>();

            TempData.Keep("model");
            return PartialView("_ReceptionDispatchMaterialsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReceptionDispatchMaterialsAddNew(bool approve, ReceptionDispatchMaterials item, Document itemDoc)
        {
            bool isSaved = false;
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);
            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            receptionDispatchMaterials.Document.emissionDate = itemDoc.emissionDate;
            receptionDispatchMaterials.Document.description = itemDoc.description;

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region Document

                        item.Document = item.Document ?? new Document();
                        item.Document.id_userCreate = ActiveUser.id;
                        item.Document.dateCreate = DateTime.Now;
                        item.Document.id_userUpdate = ActiveUser.id;
                        item.Document.dateUpdate = DateTime.Now;

                        DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == "72");//Recepción de Materiales 72
                        if (documentType == null)
                        {
                            TempData.Keep("receptionDispatchMaterials");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la Recepción de Materiales porque no existe el Tipo de Documento: Recepción de Materiales con Código: 72, configúrelo e inténtelo de nuevo");
                            return PartialView("_ReceptionDispatchMaterialsEditFormPartial", receptionDispatchMaterials);
                        }
                        item.Document.id_documentType = documentType.id;
                        item.Document.DocumentType = documentType;
                        item.Document.sequential = GetDocumentSequential(item.Document.id_documentType);
                        item.Document.number = GetDocumentNumber(item.Document.id_documentType);

                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        if (documentState == null)
                        {
                            TempData.Keep("receptionDispatchMaterials");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la Recepción de Materiales porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                            return PartialView("_ReceptionDispatchMaterialsEditFormPartial", receptionDispatchMaterials);
                        }
                        item.Document.id_documentState = documentState.id;
                        item.Document.DocumentState = documentState;

                        item.Document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
                        item.Document.id_emissionPoint = ActiveEmissionPoint.id;

                        item.Document.emissionDate = itemDoc.emissionDate;
                        item.Document.description = itemDoc.description;

                        //Actualiza Secuencial
                        if (documentType != null)
                        {
                            documentType.currentNumber = documentType.currentNumber + 1;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;
                        }

                        #endregion Document

                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                            if (entityPermissions != null)
                            {
                                foreach (var detail in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new Exception("No tiene Permiso para editar y guardar la Recepción de Materiales.");
                                    }
                                }
                                foreach (var detail in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new Exception("No tiene Permiso para editar y guardar la Recepción de Materiales.");
                                    }
                                }
                                if (approve)
                                {
                                    foreach (var detail in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
                                    {
                                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                        if (entityValuePermissions == null)
                                        {
                                            throw new Exception("No tiene Permiso para aprobar la Recepción de Materiales.");
                                        }
                                    }

                                    foreach (var detail in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
                                    {
                                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                        if (entityValuePermissions == null)
                                        {
                                            throw new Exception("No tiene Permiso para aprobar la Recepción de Materiales.");
                                        }
                                    }
                                }
                            }
                        }

                        #endregion Validación Permiso

                        var aReceptionDispatchMaterials = db.ReceptionDispatchMaterials.FirstOrDefault(fod => fod.id_remissionGuide == receptionDispatchMaterials.id_remissionGuide && fod.Document.DocumentState.code != "05");
                        if (aReceptionDispatchMaterials != null)
                        {
                            var aRemissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == receptionDispatchMaterials.id_remissionGuide);
                            throw new Exception("Ya existe grabada una Recepción de Materiales para esta guía(No: " + aRemissionGuide.Document.number + ").");
                        }

                        #region RemissionGuide.RemissionGuideDispatchMaterial

                        //item.RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>();
                        var remissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == receptionDispatchMaterials.id_remissionGuide);
                        if (remissionGuide.RemissionGuideDispatchMaterial != null && receptionDispatchMaterials.RemissionGuide?.RemissionGuideDispatchMaterial != null)
                        {
                            foreach (var detail in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
                            {
                                var remissionGuideDispatchMaterialAux = remissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id == detail.id);
                                if (remissionGuideDispatchMaterialAux != null)
                                {
                                    remissionGuideDispatchMaterialAux.sendedDestinationQuantity = detail.sendedDestinationQuantity;
                                    remissionGuideDispatchMaterialAux.amountConsumed = detail.amountConsumed;
                                    remissionGuideDispatchMaterialAux.arrivalDestinationQuantity = detail.arrivalDestinationQuantity;
                                    remissionGuideDispatchMaterialAux.arrivalGoodCondition = detail.arrivalGoodCondition;
                                    remissionGuideDispatchMaterialAux.arrivalBadCondition = detail.arrivalBadCondition;

                                    remissionGuideDispatchMaterialAux.sendedAdjustmentQuantity = detail.sendedAdjustmentQuantity;
                                    remissionGuideDispatchMaterialAux.stealQuantity = detail.stealQuantity;
                                    remissionGuideDispatchMaterialAux.transferQuantity = detail.transferQuantity;
                                }

                                db.RemissionGuideDispatchMaterial.Attach(remissionGuideDispatchMaterialAux);
                                db.Entry(remissionGuideDispatchMaterialAux).State = EntityState.Modified;
                            }
                            item.id_remissionGuide = remissionGuide.id;
                            item.RemissionGuide = remissionGuide;
                        }

                        #endregion RemissionGuide.RemissionGuideDispatchMaterial

                        #region ReceptionDispatchMaterialsDetail

                        item.ReceptionDispatchMaterialsDetail = new List<ReceptionDispatchMaterialsDetail>();
                        if (receptionDispatchMaterials.ReceptionDispatchMaterialsDetail != null)
                        {
                            foreach (var detail in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
                            {
                                var receptionDispatchMaterialsDetail = new ReceptionDispatchMaterialsDetail();
                                receptionDispatchMaterialsDetail.id_receptionDispatchMaterials = item.id;
                                receptionDispatchMaterialsDetail.id_warehouse = detail.id_warehouse;
                                receptionDispatchMaterialsDetail.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == detail.id_warehouse);
                                receptionDispatchMaterialsDetail.id_warehouseLocation = detail.id_warehouseLocation;
                                receptionDispatchMaterialsDetail.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == detail.id_warehouseLocation);
                                receptionDispatchMaterialsDetail.id_item = detail.id_item;
                                receptionDispatchMaterialsDetail.Item = db.Item.FirstOrDefault(fod => fod.id == detail.id_item);
                                receptionDispatchMaterialsDetail.arrivalDestinationQuantity = detail.arrivalDestinationQuantity;
                                receptionDispatchMaterialsDetail.arrivalGoodCondition = detail.arrivalGoodCondition;
                                receptionDispatchMaterialsDetail.arrivalBadCondition = detail.arrivalBadCondition;

                                receptionDispatchMaterialsDetail.sendedAdjustmentQuantity = detail.sendedAdjustmentQuantity;
                                receptionDispatchMaterialsDetail.stealQuantity = detail.stealQuantity;
                                receptionDispatchMaterialsDetail.transferQuantity = detail.transferQuantity;

                                item.ReceptionDispatchMaterialsDetail.Add(receptionDispatchMaterialsDetail);
                            }
                        }

                        #endregion ReceptionDispatchMaterialsDetail

                        if (approve)
                        {
                            item.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        }

                        db.ReceptionDispatchMaterials.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                        isSaved = true;

                        TempData["receptionDispatchMaterials"] = item;
                        TempData.Keep("receptionDispatchMaterials");

                        ViewData["EditMessage"] = SuccessMessage("Recepción de Materiales: " + item.Document.number + " guardado exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
                        TempData.Keep("receptionDispatchMaterials");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                        return PartialView("_ReceptionDispatchMaterialsEditFormPartial", receptionDispatchMaterials);
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            if (isSaved)
            {
                if (item.id > 0)
                {
                    GenerateResultForLiquidationMaterial(item.id);
                }
            }

            return PartialView("_ReceptionDispatchMaterialsEditFormPartial", item);
        }

        public Tuple<DocumentState, bool> ReceptionDispatchMaterialsUpdateIng(  bool approve, 
                                                                                ReceptionDispatchMaterials item, 
                                                                                Document itemDoc,
                                                                                bool isInternalTrans = false,
                                                                                ActiveUserDto sessionInfoTrans = null,
                                                                                string tempKeyTrans = null,
                                                                                string tempValueTrans = null,
                                                                                string tempTypeTrans = null)
        {
            DocumentState documentState = null;
            bool isExecute = false;
            Guid? identificadorTran = null;
            try
            {
                var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                if (!isInternalTrans)
                {
                    var receptionDispatchMaterialsInt = db.ReceptionDispatchMaterials.FirstOrDefault(r => r.id == item.id);
                    int documentTypeId = db.DocumentType.FirstOrDefault(r => r.code == "72").id;
                    string documentNumber = db.Document.FirstOrDefault(r => r.id == item.id)?.number;
                    int numDetails = receptionDispatchMaterialsInt.ReceptionDispatchMaterialsDetail.Count();

                    string sessionInfo = ServiceTransCtl.GetSessionInfoSerialize(ActiveUserId,
                                                                                    ActiveUser.username,
                                                                                    ActiveCompanyId,
                                                                                    ActiveEmissionPoint.id,
                                                                                    entityObjectPermissions);

                    ReceptionDispatchMaterialsDto receptionDispatchMaterialsParam = item.ToDto();
                    DocumentDTO documentDTO = itemDoc.ToDto();

                    string dataExecution = JsonConvert.SerializeObject(new object[] { true, receptionDispatchMaterialsParam, documentDTO });
                    string dataExecutionTypes = JsonConvert.SerializeObject(new object[] { "System.Boolean", "DXPANACEASOFT.Models.Dto.ReceptionDispatchMaterialsDto", "DXPANACEASOFT.Models.DTOModel.DocumentDTO" });

                    var result = ServiceTransCtl.TransCtlExternal(
                                                        item.id,
                                                        documentNumber,
                                                        documentTypeId: documentTypeId,
                                                        stage: "0",
                                                        numdetails: numDetails,
                                                        sessionInfoSerialize: sessionInfo,
                                                        dataExecution: dataExecution,
                                                        dataExecutionTypes: dataExecutionTypes,
                                                        temDataKey: null,
                                                        temDataValueSerialize: null,
                                                        temDataTypes: null);

                    isExecute = result.Item1;
                    identificadorTran = result.Item2;

                }
                else
                {
                    SetInfoForTrans(sessionInfoTrans, tempKeyTrans, tempValueTrans, tempTypeTrans);

                    entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
                }

                if (isInternalTrans || isExecute)
                { 
                
                }

            }
            catch (Exception e)
            {
                FullLog(e);
                throw;
            }
            finally
            {
                
                if (isExecute)
                {
                    ServiceTransCtl.Finalize(identificadorTran.Value);
                }
            }

            return new Tuple<DocumentState, bool>(documentState, isExecute);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReceptionDispatchMaterialsUpdate(bool approve, ReceptionDispatchMaterials item, Document itemDoc)
        {
            // OPTIMIZACION -- 
            string str_item = "";
            bool isSaved = false;
            int idWarehouseProvider = 0;
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);
            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            receptionDispatchMaterials.Document.emissionDate = itemDoc.emissionDate;
            receptionDispatchMaterials.Document.description = itemDoc.description;

            var modelItem = db
                            .ReceptionDispatchMaterials
                            .FirstOrDefault(p => p.id == item.id);

            List<ItemInvMoveDetail> _lstInvDetail = new List<ItemInvMoveDetail>();
            List<ItemInvMoveDetail> _lstInvDetailProvider = new List<ItemInvMoveDetail>();
            List<ItemInvMoveDetail> _lstInvDetailRecDis = new List<ItemInvMoveDetail>();
            int idItem = 0;
            string _valSettingHMIRMD = db.Setting.FirstOrDefault(fod => fod.code.Equals("HMIRMD"))?.value ?? "N";

            if (ModelState.IsValid && modelItem != null)
            {
                try
                {
                    #region DOCUMENT

                    modelItem.Document.emissionDate = itemDoc.emissionDate;
                    modelItem.Document.description = itemDoc.description;
                    modelItem.Document.id_userUpdate = ActiveUser.id;
                    modelItem.Document.dateUpdate = DateTime.Now;

                    #endregion DOCUMENT

                    #region Validación Permiso

                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            foreach (var detail in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new ProdHandlerException("No tiene Permiso para editar y guardar la Recepción de Materiales.");
                                }
                            }
                            foreach (var detail in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new ProdHandlerException("No tiene Permiso para editar y guardar la Recepción de Materiales.");
                                }
                            }
                            if (approve)
                            {
                                foreach (var detail in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new ProdHandlerException("No tiene Permiso para aprobar la Recepción de Materiales.");
                                    }
                                }

                                foreach (var detail in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new ProdHandlerException("No tiene Permiso para aprobar la Recepción de Materiales.");
                                    }
                                }
                            }
                        }
                    }

                    #endregion Validación Permiso

                    var aReceptionDispatchMaterials = db.ReceptionDispatchMaterials.FirstOrDefault(fod => fod.id != modelItem.id &&
                                                                                                          fod.id_remissionGuide == receptionDispatchMaterials.id_remissionGuide &&
                                                                                                          fod.Document.DocumentState.code != "05");
                    if (aReceptionDispatchMaterials != null)
                    {
                        var aRemissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == receptionDispatchMaterials.id_remissionGuide);
                        throw new ProdHandlerException("Ya existe grabada una Recepción de Materiales para esta guía(No: " + aRemissionGuide.Document.number + ").");
                    }

                    #region RemissionGuide.RemissionGuideDispatchMaterial

                    var remissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == receptionDispatchMaterials.id_remissionGuide);
                    if (remissionGuide.RemissionGuideDispatchMaterial != null && receptionDispatchMaterials.RemissionGuide?.RemissionGuideDispatchMaterial != null)
                    {
                        foreach (var detail in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
                        {
                            var remissionGuideDispatchMaterialAux = remissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id == detail.id);
                            if (remissionGuideDispatchMaterialAux != null)
                            {
                                remissionGuideDispatchMaterialAux.sendedDestinationQuantity = detail.sendedDestinationQuantity;
                                remissionGuideDispatchMaterialAux.amountConsumed = detail.amountConsumed;
                                remissionGuideDispatchMaterialAux.arrivalDestinationQuantity = detail.arrivalDestinationQuantity;
                                remissionGuideDispatchMaterialAux.arrivalGoodCondition = detail.arrivalGoodCondition;
                                remissionGuideDispatchMaterialAux.arrivalBadCondition = detail.arrivalBadCondition;

                                remissionGuideDispatchMaterialAux.sendedAdjustmentQuantity = detail.sendedAdjustmentQuantity;
                                remissionGuideDispatchMaterialAux.stealQuantity = detail.stealQuantity;
                                remissionGuideDispatchMaterialAux.transferQuantity = detail.transferQuantity;
                            }

                            db.RemissionGuideDispatchMaterial.Attach(remissionGuideDispatchMaterialAux);
                            db.Entry(remissionGuideDispatchMaterialAux).State = EntityState.Modified;
                        }
                    }

                    #endregion RemissionGuide.RemissionGuideDispatchMaterial

                    #region ReceptionDispatchMaterialsDetail

                    if (modelItem.ReceptionDispatchMaterialsDetail != null)
                    {
                        #region CR | Optimiza codigo
                        //for (int i = modelItem.ReceptionDispatchMaterialsDetail.Count - 1; i >= 0; i--)
                        //{
                        //    var detail = modelItem.ReceptionDispatchMaterialsDetail.ElementAt(i);
                        //    modelItem.ReceptionDispatchMaterialsDetail.Remove(detail);
                        //    db.Entry(detail).State = EntityState.Deleted;
                        //}

                        foreach (var detalle in modelItem.ReceptionDispatchMaterialsDetail.ToList())
                        {
                            modelItem.ReceptionDispatchMaterialsDetail.Remove(detalle);
                            db.Entry(detalle).State = EntityState.Deleted;
                        }
                        #endregion|



                        foreach (var detail in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
                        {
                            var receptionDispatchMaterialsDetail = new ReceptionDispatchMaterialsDetail();
                            receptionDispatchMaterialsDetail.id_receptionDispatchMaterials = modelItem.id;

                            receptionDispatchMaterialsDetail.id_warehouse = detail.id_warehouse;
                            receptionDispatchMaterialsDetail.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == detail.id_warehouse);
                            receptionDispatchMaterialsDetail.id_warehouseLocation = detail.id_warehouseLocation;
                            receptionDispatchMaterialsDetail.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == detail.id_warehouseLocation);
                            receptionDispatchMaterialsDetail.id_item = detail.id_item;
                            receptionDispatchMaterialsDetail.Item = db.Item.FirstOrDefault(fod => fod.id == detail.id_item);
                            receptionDispatchMaterialsDetail.arrivalDestinationQuantity = detail.arrivalDestinationQuantity;
                            receptionDispatchMaterialsDetail.arrivalGoodCondition = detail.arrivalGoodCondition;
                            receptionDispatchMaterialsDetail.arrivalBadCondition = detail.arrivalBadCondition;

                            receptionDispatchMaterialsDetail.sendedAdjustmentQuantity = detail.sendedAdjustmentQuantity;
                            receptionDispatchMaterialsDetail.stealQuantity = detail.stealQuantity;
                            receptionDispatchMaterialsDetail.transferQuantity = detail.transferQuantity;

                            modelItem.ReceptionDispatchMaterialsDetail.Add(receptionDispatchMaterialsDetail);
                        }
                    }

                    #endregion ReceptionDispatchMaterialsDetail

                    if (approve)
                    {
                        if (_valSettingHMIRMD.Equals("Y"))
                        {
                            #region New Version comentado

                            #region SOLUTION FOR INVENTORY MOVE DETAIL

                            var itemDetail = modelItem.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.isActive).ToList();
                            DateTime dt = modelItem.Document.emissionDate;
                            //DateTime.Now;
                            string dtEmissionDate = GeneralStr.GetDateFormatStringFromDatetime(dt);
                            string dtHourEmissionDate = GeneralStr.GetTimeFormatStringFromDatetime(dt);

                            foreach (var _detMat in itemDetail)
                            {
                                WarehouseLocation warehouseLocationAux = null;

                                Setting settingUUDEMD = db.Setting.FirstOrDefault(t => t.code == "UUDEMD");//UUDEMD-Parametro de Usar Ubicación Por Defecto En EMD
                                var id_warehouseLocationAuxInt = _detMat.id_warehouselocation;
                                if (settingUUDEMD == null || settingUUDEMD?.value == "1")
                                {
                                    Setting settingUDLI = db.Setting.FirstOrDefault(t => t.code == "UDLI");//UDLI-Parametro de Ubicación por Defecto por Linea de Inventario
                                    if (settingUDLI == null)
                                    {
                                        throw new ProdHandlerException("No se pudo aprobar la Guía porque no se pudo Egresar Automaticamente el material de despacho debido a no estar configurado el Parámetro de: Ubicación por Defecto por Linea de Inventario con código(UDLI) " +
                                                            "necesario para saber en que Bodega-Ubicación se egresa el mismo");
                                    }

                                    var id_inventoryLineAux = _detMat.Item.id_inventoryLine.ToString();
                                    var id_warehouseLocationAux = settingUDLI.SettingDetail.FirstOrDefault(fod => fod.value == id_inventoryLineAux)?.valueAux;
                                    if (id_warehouseLocationAux == null)
                                    {
                                        throw new ProdHandlerException("No se pudo aprobar la Guía porque no se pudo Egresar Automaticamente el material de despacho debido a no estar configurado el Parámetro de: Ubicación por Defecto por Linea de Inventario con código(UDLI) " +
                                                        " para la linea de inventario " + _detMat.Item.InventoryLine.name + " necesario para saber en que Bodega-Ubicación se egresa el mismo");
                                    }
                                    id_warehouseLocationAuxInt = int.Parse(id_warehouseLocationAux);
                                }
                                id_warehouseLocationAuxInt = id_warehouseLocationAuxInt ?? _detMat.Item.ItemInventory.id_warehouseLocation;
                                warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationAuxInt);

                                if (warehouseLocationAux != null)
                                {
                                    str_item = str_item + _detMat.id_item.ToString() + ","
                                    + warehouseLocationAux.Warehouse.id.ToString() + ","
                                    + warehouseLocationAux.id.ToString() + ",;";
                                }
                                /*consulta cambiada*/
                            }

                            #region QUERY LAST INVENTORYMOVEDETAIL

                            ParamForQueryInvMoveDetail _param = new ParamForQueryInvMoveDetail();
                            _param.str_item = str_item;
                            _param.emissiondate = dtEmissionDate;
                            _param.houremissiondate = dtHourEmissionDate;

                            _lstInvDetail = ServiceInventoryMove.GetLastMoveDetail(_param);

                            #endregion QUERY LAST INVENTORYMOVEDETAIL

                            #endregion SOLUTION FOR INVENTORY MOVE DETAIL

                            str_item = "";

                            #region SOLUTION FOR INVENTORY MOVE DETAIL

                            foreach (var _detMat in itemDetail)
                            {
                                WarehouseLocation warehouseLocationAux = null;

                                Provider providerAux = db.Provider.FirstOrDefault(fod => fod.id == remissionGuide.id_providerRemisionGuide);
                                warehouseLocationAux = ServiceInventoryMove.GetWarehouseLocationProvider(remissionGuide.id_providerRemisionGuide, db, ActiveCompany, ActiveUser);

                                if (warehouseLocationAux == null)
                                {
                                    throw new ProdHandlerException("No puede Aprobarse la Guía debido a no existir la bodega Virtual de Proveedores con codigo(VIRPRO) necesaria para crear la ubicación del proveedor: " +
                                                        providerAux.Person.fullname_businessName + " y realizar la reversión. El administrador del sistema debe configurar la opción y después intentelo de nuevo");
                                }
                                idWarehouseProvider = (int)warehouseLocationAux.Warehouse.id;
                                if (warehouseLocationAux != null)
                                {
                                    str_item = str_item + _detMat.id_item.ToString() + ","
                                    + warehouseLocationAux.Warehouse.id.ToString() + ","
                                    + warehouseLocationAux.id.ToString() + ",;";
                                }
                                /*consulta cambiada*/
                            }

                            #region QUERY LAST INVENTORYMOVEDETAIL

                            ParamForQueryInvMoveDetail _param2 = new ParamForQueryInvMoveDetail();
                            _param2.str_item = str_item;
                            _param2.emissiondate = dtEmissionDate;
                            _param2.houremissiondate = dtHourEmissionDate;

                            _lstInvDetailProvider = ServiceInventoryMove.GetLastMoveDetail(_param2);

                            #endregion QUERY LAST INVENTORYMOVEDETAIL

                            #endregion SOLUTION FOR INVENTORY MOVE DETAIL

                            str_item = "";

                            #region Estos son los items que no egresaron en guía de remisión pero que si ingresaron en la Recepción

                            var itemDetailsReception = modelItem.ReceptionDispatchMaterialsDetail.ToList();
                            foreach (var _detMat in itemDetailsReception)
                            {
                                if (_detMat.Warehouse != null)
                                {
                                    str_item = str_item + _detMat.id_item.ToString() + ","
                                    + _detMat.id_warehouse.ToString() + ","
                                    + _detMat.id_warehouseLocation.ToString() + ",;";
                                }
                            }

                            #region QUERY LAST INVENTORYMOVEDETAIL

                            ParamForQueryInvMoveDetail _param3 = new ParamForQueryInvMoveDetail();
                            _param3.str_item = str_item;
                            _param3.emissiondate = dtEmissionDate;
                            _param3.houremissiondate = dtHourEmissionDate;

                            _lstInvDetailRecDis = ServiceInventoryMove.GetLastMoveDetail(_param3);

                            #endregion QUERY LAST INVENTORYMOVEDETAIL

                            #endregion Estos son los items que no egresaron en guía de remisión pero que si ingresaron en la Recepción

                            #region Ahora se Divide por un documento por bodega ¿Cuáles serían estas bodegas? Las que indica la Guía de Remisión

                            var lstWarehouses = modelItem.RemissionGuide.RemissionGuideDispatchMaterial.Select(s => s.id_warehouse).Distinct().ToList();
                            List<DocumentSource> lstDocS = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.InventoryMove != null).ToList();

                            if (lstWarehouses != null && lstWarehouses.Count() > 0)
                            {
                                foreach (int i in lstWarehouses)
                                {
                                    var lstDispatchMaterials = modelItem.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.id_warehouse == i).ToList();

                                    #region Busco todos los movimientos a reversar

                                    //Si no existe liquidacion de materiales asociado a la Recepción de Materiales se hace lo de a continuación(Hay que hacer esta validación cuando exista la Liquidacion de Materiales)
                                    var inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                                                          (w.Document.InventoryMove != null && w.Document.InventoryMove.InventoryMoveDetail != null ?
                                                                                          w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDR") : false)
                                                                                          && w.Document.InventoryMove.idWarehouse == idWarehouseProvider
                                                                                          ).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();

                                    InventoryMove lastInventoryMoveEPTAMDR = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveEPTAMDR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ?
                                                                                         w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDR") : false)
                                                                                         && w.Document.InventoryMove.idWarehouse == i
                                                                                         ).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    InventoryMove lastInventoryMoveIPTAMDR = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveIPTAMDR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //Consumed
                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                                                          (w.Document.InventoryMove != null ?
                                                                                          w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDPCR") : false)
                                                                                          && w.Document.InventoryMove.idWarehouse == idWarehouseProvider).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    InventoryMove lastInventoryMoveEPTAMDPCR = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveEPTAMDPCR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ?
                                                                                         w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDPCR") : false)
                                                                                         && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    InventoryMove lastInventoryMoveIPTAMDPCR = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveIPTAMDPCR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ?
                                                                                         w.Document.InventoryMove.InventoryReason.code.Equals("EAAMDL") : false)
                                                                                         && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();

                                    InventoryMove lastInventoryMoveEAAMDL = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveEAAMDL = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                  (w.Document.InventoryMove != null ?
                                                  w.Document.InventoryMove.InventoryReason.code.Equals("IAAMDL") : false)
                                                  && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();

                                    InventoryMove lastInventoryMoveIAAMDL = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveIAAMDL = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                 (w.Document.InventoryMove != null ?
                                                 w.Document.InventoryMove.InventoryReason.code.Equals("ERMDA") : false)
                                                 && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();

                                    InventoryMove lastInventoryMoveERMDA = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveERMDA = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                 (w.Document.InventoryMove != null ?
                                                 w.Document.InventoryMove.InventoryReason.code.Equals("ETAPR") : false)
                                                 && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();

                                    InventoryMove lastInventoryMoveETAPR = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveETAPR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                 (w.Document.InventoryMove != null ?
                                                 w.Document.InventoryMove.InventoryReason.code.Equals("ITAPR") : false)
                                                 && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();

                                    InventoryMove lastInventoryMoveITAPR = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveITAPR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("ECMDA") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    InventoryMove lastInventoryMoveECMDA = (inventoryMoveDetailAux.Count > 0)
                                                                        ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                        : null;

                                    var isExit = (lastInventoryMoveEPTAMDPCR != null && lastInventoryMoveECMDA != null) ? (DateTime.Compare(lastInventoryMoveEPTAMDPCR.Document.dateCreate, lastInventoryMoveECMDA.Document.dateCreate) < 0) : lastInventoryMoveECMDA != null;
                                    if (isExit)
                                    {
                                        lastInventoryMoveEPTAMDPCR = null;
                                        lastInventoryMoveIPTAMDPCR = null;
                                    }
                                    else
                                    {
                                        lastInventoryMoveECMDA = null;
                                    }

                                    #endregion Busco todos los movimientos a reversar

                                    ServiceInventoryMove.UpdateInventaryMoveTransferReceptionDispatchMaterials(ActiveUser, ActiveCompany,
                                        ActiveEmissionPoint, modelItem, db, false, lastInventoryMoveEPTAMDR,
                                        lastInventoryMoveIPTAMDR, null, lastInventoryMoveEPTAMDPCR, lastInventoryMoveIPTAMDPCR,
                                        lastInventoryMoveECMDA, _lstInvDetailProvider, _lstInvDetail, lstDispatchMaterials,
                                        i, idWarehouseProvider, lastInventoryMoveEAAMDL, lastInventoryMoveIAAMDL, lastInventoryMoveERMDA, lastInventoryMoveETAPR, lastInventoryMoveITAPR);
                                }
                            }

                            #endregion Ahora se Divide por un documento por bodega ¿Cuáles serían estas bodegas? Las que indica la Guía de Remisión

                            #region Ahora las Recepciones en Logística también generan un movimiento por cada bodega

                            var lstWarehouses2 = modelItem.ReceptionDispatchMaterialsDetail.Select(s => s.id_warehouse).Distinct().ToList();
                            if (lstWarehouses2 != null && lstWarehouses2.Count() > 0)
                            {
                                foreach (int i in lstWarehouses2)
                                {
                                    var inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                     (w.Document.InventoryMove != null ?
                                                                                     w.Document.InventoryMove.InventoryReason.code.Equals("IMDA") : false)
                                                                                     && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    InventoryMove lastInventoryMoveIMDA = (inventoryMoveDetailAux.Count > 0)
                                                                        ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                        : null;
                                    var lstDispatchMaterials2 = modelItem.ReceptionDispatchMaterialsDetail.Where(w => w.id_warehouse == i).ToList();

                                    ServiceInventoryMove.UpdateInventaryMoveTransferReceptionDispatchMaterialsNoRemissionGuide(ActiveUser, ActiveCompany,
                                        ActiveEmissionPoint, modelItem, db, false, null, null, lastInventoryMoveIMDA, null, null, null,
                                        _lstInvDetailRecDis, lstDispatchMaterials2, i);
                                }
                            }

                            #endregion Ahora las Recepciones en Logística también generan un movimiento por cada bodega

                            #endregion New Version comentado
                        }

                        modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                    }

                    db.ReceptionDispatchMaterials.Attach(modelItem);
                    db.Entry(modelItem).State = EntityState.Modified;
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.SaveChanges();
                            trans.Commit();
                            isSaved = true;

                            TempData["receptionDispatchMaterials"] = modelItem;
                            TempData.Keep("receptionDispatchMaterials");

                            ViewData["EditMessage"] = SuccessMessage("Recepción de Materiales: " + modelItem.Document.number + " guardada exitosamente");
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                        }
                       
                    }
                }
                catch (ProdHandlerException e)
                {
                    TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
                    TempData.Keep("receptionDispatchMaterials");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                }

                catch (Exception e)
                {
                    TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
                    TempData.Keep("receptionDispatchMaterials");
                    ViewData["EditMessage"] = GenericError.ErrorGeneralRecepcionMaterialesDespacho;


                    return PartialView("_ReceptionDispatchMaterialsEditFormPartial", receptionDispatchMaterials);
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            if (isSaved)
            {
                if (modelItem.id > 0)
                {
                    GenerateResultForLiquidationMaterial(modelItem.id);
                }
            }

            return PartialView("_ReceptionDispatchMaterialsEditFormPartial", modelItem);
        }

        #endregion Reception Dispatch Materials

        #region ReceptionDispatchMaterialsDetail

        [ValidateInput(false)]
        public ActionResult RemissionGuideDispatchMaterialPartial()
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);

            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            var model = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial.OrderBy(od => od.id).ToList()
                ?? new List<RemissionGuideDispatchMaterial>();

            TempData["receptionDispatchMaterials"] = TempData["receptionDispatchMaterials"] ?? receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");

            return PartialView("_RemissionGuidesViewsRemissionGuideDispatchMaterialPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult ReceptionDispatchMaterialsEditFormDetailPartial()
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);

            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            var model = receptionDispatchMaterials?.ReceptionDispatchMaterialsDetail.OrderBy(od => od.id).ToList() ?? new List<ReceptionDispatchMaterialsDetail>();

            TempData["receptionDispatchMaterials"] = TempData["receptionDispatchMaterials"] ?? receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");

            return PartialView("_ReceptionDispatchMaterialsEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReceptionDispatchMaterialsEditFormDetailAddNew(ReceptionDispatchMaterialsDetail item)
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);

            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            receptionDispatchMaterials.ReceptionDispatchMaterialsDetail = receptionDispatchMaterials.ReceptionDispatchMaterialsDetail ?? new List<ReceptionDispatchMaterialsDetail>();

            if (ModelState.IsValid)
            {
                item.id = receptionDispatchMaterials.ReceptionDispatchMaterialsDetail.Count() > 0 ? receptionDispatchMaterials.ReceptionDispatchMaterialsDetail.Max(ppd => ppd.id) + 1 : 1;
                item.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouse);
                item.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_warehouseLocation);
                item.Item = db.Item.FirstOrDefault(fod => fod.id == item.id_item);

                receptionDispatchMaterials.ReceptionDispatchMaterialsDetail.Add(item);
            }

            TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");
            var model = receptionDispatchMaterials?.ReceptionDispatchMaterialsDetail.OrderBy(od => od.id).ToList() ?? new List<ReceptionDispatchMaterialsDetail>();

            return PartialView("_ReceptionDispatchMaterialsEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReceptionDispatchMaterialsEditFormDetailUpdate(ReceptionDispatchMaterialsDetail item)
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);

            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            receptionDispatchMaterials.ReceptionDispatchMaterialsDetail = receptionDispatchMaterials.ReceptionDispatchMaterialsDetail ?? new List<ReceptionDispatchMaterialsDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = receptionDispatchMaterials.ReceptionDispatchMaterialsDetail.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.id_warehouse = item.id_warehouse;
                        modelItem.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouse);
                        modelItem.id_warehouseLocation = item.id_warehouseLocation;
                        modelItem.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_warehouseLocation);
                        modelItem.id_item = item.id_item;
                        modelItem.Item = db.Item.FirstOrDefault(fod => fod.id == item.id_item);

                        modelItem.arrivalDestinationQuantity = item.arrivalDestinationQuantity;
                        modelItem.arrivalGoodCondition = item.arrivalGoodCondition;
                        modelItem.arrivalBadCondition = item.arrivalBadCondition;

                        modelItem.sendedAdjustmentQuantity = item.sendedAdjustmentQuantity;
                        modelItem.stealQuantity = item.stealQuantity;
                        modelItem.transferQuantity = item.transferQuantity;

                        this.UpdateModel(modelItem);
                        TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");

            var model = receptionDispatchMaterials?.ReceptionDispatchMaterialsDetail.OrderBy(od => od.id).ToList() ?? new List<ReceptionDispatchMaterialsDetail>();

            return PartialView("_ReceptionDispatchMaterialsEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReceptionDispatchMaterialsEditFormDetailDelete(System.Int32 id)
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);

            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            receptionDispatchMaterials.ReceptionDispatchMaterialsDetail = receptionDispatchMaterials.ReceptionDispatchMaterialsDetail ?? new List<ReceptionDispatchMaterialsDetail>();

            try
            {
                var receptionDispatchMaterialsDetail = receptionDispatchMaterials.ReceptionDispatchMaterialsDetail.FirstOrDefault(p => p.id == id);
                if (receptionDispatchMaterialsDetail != null)
                    receptionDispatchMaterials.ReceptionDispatchMaterialsDetail.Remove(receptionDispatchMaterialsDetail);

                TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");

            var model = receptionDispatchMaterials?.ReceptionDispatchMaterialsDetail.OrderBy(od => od.id).ToList() ?? new List<ReceptionDispatchMaterialsDetail>();

            return PartialView("_ReceptionDispatchMaterialsEditFormDetailPartial", model);
        }

        #endregion ReceptionDispatchMaterialsDetail

        #region RemissionGuide.RemissionGuideDispatchMaterial

        [ValidateInput(false)]
        public ActionResult RemissionGuideDispatchMaterialEditFormPartial()
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);

            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            var model = receptionDispatchMaterials?.RemissionGuide?.RemissionGuideDispatchMaterial.OrderBy(od => od.id).ToList() ?? new List<RemissionGuideDispatchMaterial>();

            TempData["receptionDispatchMaterials"] = TempData["receptionDispatchMaterials"] ?? receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");

            return PartialView("_RemissionGuidesEditFormRemissionGuideDispatchMaterialPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDispatchMaterialEditFormUpdate(RemissionGuideDispatchMaterial item)
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);

            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();
            receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial ?? new List<RemissionGuideDispatchMaterial>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.sendedDestinationQuantity = item.sendedDestinationQuantity;
                        modelItem.amountConsumed = item.amountConsumed;
                        modelItem.arrivalDestinationQuantity = item.arrivalDestinationQuantity;
                        modelItem.arrivalGoodCondition = item.arrivalGoodCondition;
                        modelItem.arrivalBadCondition = item.arrivalBadCondition;

                        modelItem.sendedAdjustmentQuantity = item.sendedAdjustmentQuantity;
                        modelItem.stealQuantity = item.stealQuantity;
                        modelItem.transferQuantity = item.transferQuantity;

                        this.UpdateModel(modelItem);
                        TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("invoiceCommercial");

            var model = receptionDispatchMaterials?.RemissionGuide?.RemissionGuideDispatchMaterial.OrderBy(od => od.id).ToList() ?? new List<RemissionGuideDispatchMaterial>();

            return PartialView("_RemissionGuidesEditFormRemissionGuideDispatchMaterialPartial", model);
        }

        #endregion RemissionGuide.RemissionGuideDispatchMaterial

        #region DETAILS VIEW

        public ActionResult ReceptionDispatchMaterialsDetailPartial(int? id_receptionDispatchMaterials)
        {
            ViewData["id_receptionDispatchMaterials"] = id_receptionDispatchMaterials;
            var receptionDispatchMaterials = db.ReceptionDispatchMaterials.FirstOrDefault(p => p.id == id_receptionDispatchMaterials);
            var model = receptionDispatchMaterials?.ReceptionDispatchMaterialsDetail.OrderBy(od => od.id).ToList() ?? new List<ReceptionDispatchMaterialsDetail>();
            TempData.Keep("receptionDispatchMaterials");
            return PartialView("_ReceptionDispatchMaterialsDetailViewsPartial", model);
        }

        public ActionResult RemissionGuideDispatchMaterialDetailPartial(int? id_receptionDispatchMaterials)
        {
            ViewData["id_receptionDispatchMaterials"] = id_receptionDispatchMaterials;
            var receptionDispatchMaterials = db.ReceptionDispatchMaterials.FirstOrDefault(p => p.id == id_receptionDispatchMaterials);

            var model = receptionDispatchMaterials?.RemissionGuide?.RemissionGuideDispatchMaterial.OrderBy(od => od.id).ToList() ?? new List<RemissionGuideDispatchMaterial>();
            TempData.Keep("receptionDispatchMaterials");

            return PartialView("_RemissionGuideDispatchMaterialViewsPartial", model);
        }

        #endregion DETAILS VIEW

        #region SINGLE CHANGE STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");

                    if (purchasePlanning != null && documentState != null)
                    {
                        purchasePlanning.Document.id_documentState = documentState.id;
                        purchasePlanning.Document.DocumentState = documentState;

                        db.PurchasePlanning.Attach(purchasePlanning);
                        db.Entry(purchasePlanning).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["purchasePlanning"] = purchasePlanning;
            TempData.Keep("purchasePlanning");
            ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + purchasePlanning.Document.number + " aprobada exitosamente");

            return PartialView("_PurchasePlanningEditFormPartial", purchasePlanning);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            InvoiceCommercial invoiceCommercial = db.InvoiceCommercial.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06");//06 Autorizado

                    if (invoiceCommercial != null && documentState != null)
                    {
                        invoiceCommercial.Document.id_documentState = documentState.id;
                        invoiceCommercial.Document.DocumentState = documentState;

                        db.InvoiceCommercial.Attach(invoiceCommercial);
                        db.Entry(invoiceCommercial).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("invoiceCommercial");
                    ViewData["EditMessage"] = ErrorMessage("No se puede Autorizar la Factura Comercial debido a: " + e.Message);
                    return PartialView("_InvoiceCommercialEditFormPartial", invoiceCommercial);
                }
            }

            TempData["invoiceCommercial"] = invoiceCommercial;
            TempData.Keep("invoiceCommercial");
            ViewData["EditMessage"] = SuccessMessage("Factura Comercial: " + invoiceCommercial.Document1.number + " autorizada exitosamente");

            return PartialView("_InvoiceCommercialEditFormPartial", invoiceCommercial);
        }

        [HttpPost]
        public ActionResult Protect(int id)
        {
            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "04");

                    if (purchasePlanning != null && documentState != null)
                    {
                        purchasePlanning.Document.id_documentState = documentState.id;
                        purchasePlanning.Document.DocumentState = documentState;

                        db.PurchasePlanning.Attach(purchasePlanning);
                        db.Entry(purchasePlanning).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["purchasePlanning"] = purchasePlanning;
            TempData.Keep("purchasePlanning");
            ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + purchasePlanning.Document.number + " cerrada exitosamente");

            return PartialView("_PurchasePlanningEditFormPartial", purchasePlanning);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            bool isSaved = false;
            ReceptionDispatchMaterials receptionDispatchMaterials = db.ReceptionDispatchMaterials.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    if (receptionDispatchMaterials.ResultReceptionDispatchMaterial != null
                        && receptionDispatchMaterials.ResultReceptionDispatchMaterial.LiquidationMaterialSupplies != null
                        && !receptionDispatchMaterials.ResultReceptionDispatchMaterial.LiquidationMaterialSupplies.Document.DocumentState.code.Equals("05"))
                    {
                        throw new Exception("La recepción de Materiales forma parte de una Liquidación de Materiales.");
                    }

                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            foreach (var item in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Anular.");
                                }
                            }

                            foreach (var item in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Anular.");
                                }
                            }
                        }
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");//ANULADA

                    if (receptionDispatchMaterials != null && documentState != null)
                    {
                        if (receptionDispatchMaterials.Document.DocumentState.code == "03")
                        {
                            //Si no existe liquidacion de materiales asociado a la Recepción de Materiales se hace lo de a continuación (Hay que hacer esta validación cuando exista la Liquidacion de Materiales)
                            var inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                  (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDR") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveEPTAMDR = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                 (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDR") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveIPTAMDR = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                 (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IMDA") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveIMDA = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EAAMDL") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();

                            InventoryMove lastInventoryMoveEAAMDL = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IAAMDL") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveIAAMDL = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("ERMDA") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveERMDA = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("ETAPR") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveETAPR = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                               (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("ITAPR") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveITAPR = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;

                            //Consumed
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                  (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDPCR") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveEPTAMDPCR = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                 (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDPCR") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveIPTAMDPCR = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id &&
                                                                                 (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("ECMDA") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveECMDA = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                : null;

                            var isExit = (lastInventoryMoveEPTAMDPCR != null && lastInventoryMoveECMDA != null) ? (DateTime.Compare(lastInventoryMoveEPTAMDPCR.Document.dateCreate, lastInventoryMoveECMDA.Document.dateCreate) < 0) : lastInventoryMoveECMDA != null;
                            if (isExit)
                            {
                                lastInventoryMoveEPTAMDPCR = null;
                                lastInventoryMoveIPTAMDPCR = null;
                            }
                            else
                            {
                                lastInventoryMoveECMDA = null;
                            }
                        }

                        foreach (var item in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
                        {
                            item.arrivalGoodCondition = 0;
                            item.arrivalDestinationQuantity = 0;
                            item.arrivalBadCondition = 0;
                            item.sendedAdjustmentQuantity = 0;
                            item.stealQuantity = 0;
                            item.transferQuantity = 0;
                            db.RemissionGuideDispatchMaterial.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        receptionDispatchMaterials.Document.id_documentState = documentState.id;
                        receptionDispatchMaterials.Document.DocumentState = documentState;

                        db.ReceptionDispatchMaterials.Attach(receptionDispatchMaterials);
                        db.Entry(receptionDispatchMaterials).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        isSaved = true;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("receptionDispatchMaterials");
                    ViewData["EditMessage"] = ErrorMessage("No se puede anular la Recepción de Materiales debido a: " + e.Message);
                    return PartialView("_ReceptionDispatchMaterialsEditFormPartial", receptionDispatchMaterials);
                }
            }

            TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");
            ViewData["EditMessage"] = SuccessMessage("Recepción de Materiales: " + receptionDispatchMaterials.Document.number + " anulada exitosamente");

            if (isSaved)
            {
                if (receptionDispatchMaterials.id > 0)
                {
                    GenerateResultForLiquidationMaterial(receptionDispatchMaterials.id);
                }
            }

            return PartialView("_ReceptionDispatchMaterialsEditFormPartial", receptionDispatchMaterials);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            bool isSaved = false;
            ReceptionDispatchMaterials receptionDispatchMaterials = db.ReceptionDispatchMaterials.FirstOrDefault(r => r.id == id);
            List<ItemInvMoveDetail> _lstInvDetail = new List<ItemInvMoveDetail>();
            List<ItemInvMoveDetail> _lstInvDetailProvider = new List<ItemInvMoveDetail>();
            List<ItemInvMoveDetail> _lstInvDetailRecDis = new List<ItemInvMoveDetail>();
            string str_item = "";
            int idWarehouseProvider = 0;
            int idItem = 0;
            string _valSettingHMIRMD = db.Setting.FirstOrDefault(fod => fod.code.Equals("HMIRMD"))?.value ?? "N";

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    if (receptionDispatchMaterials.ResultReceptionDispatchMaterial != null
                        && receptionDispatchMaterials.ResultReceptionDispatchMaterial.LiquidationMaterialSupplies != null
                        && !receptionDispatchMaterials.ResultReceptionDispatchMaterial.LiquidationMaterialSupplies.Document.DocumentState.code.Equals("05"))
                    {
                        throw new Exception("La recepción de Materiales forma parte de una Liquidación de Materiales.");
                    }

                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            foreach (var item in receptionDispatchMaterials.ReceptionDispatchMaterialsDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Reversar.");
                                }
                            }

                            foreach (var item in receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Reversar.");
                                }
                            }
                        }
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE

                    if (receptionDispatchMaterials != null && documentState != null)
                    {
                        if (_valSettingHMIRMD.Equals("Y"))
                        {
                            #region New Version

                            #region SOLUTION FOR INVENTORY MOVE DETAIL

                            var itemDetail = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.isActive).ToList();

                            DateTime dt = receptionDispatchMaterials.Document.emissionDate;
                            string dtEmissionDate = GeneralStr.GetDateFormatStringFromDatetime(dt);
                            string dtHourEmissionDate = GeneralStr.GetTimeFormatStringFromDatetime(dt);

                            foreach (var _detMat in itemDetail)
                            {
                                WarehouseLocation warehouseLocationAux = null;

                                Setting settingUUDEMD = db.Setting.FirstOrDefault(t => t.code == "UUDEMD");//UUDEMD-Parametro de Usar Ubicación Por Defecto En EMD
                                var id_warehouseLocationAuxInt = _detMat.id_warehouselocation;
                                if (settingUUDEMD == null || settingUUDEMD?.value == "1")
                                {
                                    Setting settingUDLI = db.Setting.FirstOrDefault(t => t.code == "UDLI");//UDLI-Parametro de Ubicación por Defecto por Linea de Inventario
                                    if (settingUDLI == null)
                                    {
                                        throw new Exception("No se pudo aprobar la Guía porque no se pudo Egresar Automaticamente el material de despacho debido a no estar configurado el Parámetro de: Ubicación por Defecto por Linea de Inventario con código(UDLI) " +
                                                            "necesario para saber en que Bodega-Ubicación se egresa el mismo");
                                    }

                                    var id_inventoryLineAux = _detMat.Item.id_inventoryLine.ToString();
                                    var id_warehouseLocationAux = settingUDLI.SettingDetail.FirstOrDefault(fod => fod.value == id_inventoryLineAux)?.valueAux;
                                    if (id_warehouseLocationAux == null)
                                    {
                                        throw new Exception("No se pudo aprobar la Guía porque no se pudo Egresar Automaticamente el material de despacho debido a no estar configurado el Parámetro de: Ubicación por Defecto por Linea de Inventario con código(UDLI) " +
                                                        " para la linea de inventario " + _detMat.Item.InventoryLine.name + " necesario para saber en que Bodega-Ubicación se egresa el mismo");
                                    }
                                    id_warehouseLocationAuxInt = int.Parse(id_warehouseLocationAux);
                                }
                                id_warehouseLocationAuxInt = id_warehouseLocationAuxInt ?? _detMat.Item.ItemInventory.id_warehouseLocation;
                                warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationAuxInt);

                                if (warehouseLocationAux != null)
                                {
                                    str_item = str_item + _detMat.id_item.ToString() + ","
                                    + warehouseLocationAux.Warehouse.id.ToString() + ","
                                    + warehouseLocationAux.id.ToString() + ",;";
                                }
                                /*consulta cambiada*/
                            }

                            #region QUERY LAST INVENTORYMOVEDETAIL

                            ParamForQueryInvMoveDetail _param = new ParamForQueryInvMoveDetail();
                            _param.str_item = str_item;
                            _param.emissiondate = dtEmissionDate;
                            _param.houremissiondate = dtHourEmissionDate;

                            _lstInvDetail = ServiceInventoryMove.GetLastMoveDetail(_param);

                            #endregion QUERY LAST INVENTORYMOVEDETAIL

                            #endregion SOLUTION FOR INVENTORY MOVE DETAIL

                            str_item = "";

                            #region SOLUTION FOR INVENTORY MOVE DETAIL

                            foreach (var _detMat in itemDetail)
                            {
                                WarehouseLocation warehouseLocationAux = null;

                                Provider providerAux = db.Provider.FirstOrDefault(fod => fod.id == receptionDispatchMaterials.RemissionGuide.id_providerRemisionGuide);
                                warehouseLocationAux = ServiceInventoryMove.GetWarehouseLocationProvider(receptionDispatchMaterials.RemissionGuide.id_providerRemisionGuide, db, ActiveCompany, ActiveUser);

                                if (warehouseLocationAux == null)
                                {
                                    throw new Exception("No puede Aprobarse la Guía debido a no existir la bodega Virtual de Proveedores con codigo(VIRPRO) necesaria para crear la ubicación del proveedor: " +
                                                        providerAux.Person.fullname_businessName + " y realizar la reversión. El administrador del sistema debe configurar la opción y después intentelo de nuevo");
                                }
                                idWarehouseProvider = (int)warehouseLocationAux.Warehouse.id;
                                if (warehouseLocationAux != null)
                                {
                                    str_item = str_item + _detMat.id_item.ToString() + ","
                                    + warehouseLocationAux.Warehouse.id.ToString() + ","
                                    + warehouseLocationAux.id.ToString() + ",;";
                                }
                                /*consulta cambiada*/
                            }

                            #region QUERY LAST INVENTORYMOVEDETAIL

                            ParamForQueryInvMoveDetail _param2 = new ParamForQueryInvMoveDetail();
                            _param2.str_item = str_item;
                            _param2.emissiondate = dtEmissionDate;
                            _param2.houremissiondate = dtHourEmissionDate;

                            _lstInvDetailProvider = ServiceInventoryMove.GetLastMoveDetail(_param2);

                            #endregion QUERY LAST INVENTORYMOVEDETAIL

                            #endregion SOLUTION FOR INVENTORY MOVE DETAIL

                            str_item = "";

                            #region Estos son los items que no egresaron en guía de remisión pero que si ingresaron en la Recepción

                            var itemDetailsReception = receptionDispatchMaterials.ReceptionDispatchMaterialsDetail.ToList();
                            foreach (var _detMat in itemDetailsReception)
                            {
                                if (_detMat.Warehouse != null)
                                {
                                    str_item = str_item + _detMat.id_item.ToString() + ","
                                    + _detMat.id_warehouse.ToString() + ","
                                    + _detMat.id_warehouseLocation.ToString() + ",;";
                                }
                            }

                            #region QUERY LAST INVENTORYMOVEDETAIL

                            ParamForQueryInvMoveDetail _param3 = new ParamForQueryInvMoveDetail();
                            _param3.str_item = str_item;
                            _param3.emissiondate = dtEmissionDate;
                            _param3.houremissiondate = dtHourEmissionDate;

                            _lstInvDetailRecDis = ServiceInventoryMove.GetLastMoveDetail(_param3);

                            #endregion QUERY LAST INVENTORYMOVEDETAIL

                            #endregion Estos son los items que no egresaron en guía de remisión pero que si ingresaron en la Recepción

                            int idNullState = db.DocumentState.FirstOrDefault(fod => fod.code == "05")?.id ?? 0;

                            #region Trae todos los documentos originados a partir de esta recepción

                            List<DocumentSource> lstDocS = db.DocumentSource.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && w.Document.InventoryMove != null).ToList();
                            var lstWarehouses = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial.Select(s => s.id_warehouse).Distinct().ToList();
                            if (lstWarehouses != null && lstWarehouses.Count() > 0)
                            {
                                foreach (int i in lstWarehouses)
                                {
                                    var lstDispatchMaterials = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.id_warehouse == i && w.arrivalDestinationQuantity > 0).ToList();

                                    //Si no existe liquidacion de materiales asociado a la Recepción de Materiales se hace lo de a continuación(Hay que hacer esta validación cuando exista la Liquidacion de Materiales)
                                    var inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                          (w.Document.InventoryMove != null && w.Document.InventoryMove.InventoryMoveDetail != null ?
                                                                                          w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDR") : false)
                                                                                          && w.Document.InventoryMove.idWarehouse == idWarehouseProvider
                                                                                          ).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();

                                    InventoryMove lastInventoryMoveEPTAMDR = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveEPTAMDR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveEPTAMDR != null)
                                    {
                                        Document _doEPTAMDR = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveEPTAMDR.id);
                                        _doEPTAMDR.id_documentState = idNullState;
                                        db.Document.Attach(_doEPTAMDR);
                                        db.Entry(_doEPTAMDR).State = EntityState.Modified;
                                        var lstDetailEPTAMDR = lastInventoryMoveEPTAMDR?.InventoryMoveDetail.ToList();
                                        if (lstDetailEPTAMDR != null)
                                        {
                                            foreach (var det in lstDetailEPTAMDR)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, det.exitAmount, db);
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ?
                                                                                         w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDR") : false)
                                                                                         && w.Document.InventoryMove.idWarehouse == i
                                                                                         ).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    InventoryMove lastInventoryMoveIPTAMDR = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveIPTAMDR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveIPTAMDR != null)
                                    {
                                        Document _doIPTAMDR = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveIPTAMDR.id);
                                        _doIPTAMDR.id_documentState = idNullState;
                                        db.Document.Attach(_doIPTAMDR);
                                        db.Entry(_doIPTAMDR).State = EntityState.Modified;
                                        var lstDetailIPTAMDR = lastInventoryMoveIPTAMDR?.InventoryMoveDetail.ToList();
                                        if (lstDetailIPTAMDR != null)
                                        {
                                            foreach (var det in lstDetailIPTAMDR)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, -det.entryAmount, db);
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                          (w.Document.InventoryMove != null ?
                                                                                          w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDPCR") : false)
                                                                                          && w.Document.InventoryMove.idWarehouse == idWarehouseProvider).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    InventoryMove lastInventoryMoveEPTAMDPCR = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveEPTAMDPCR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveEPTAMDPCR != null)
                                    {
                                        Document _doEPTAMDPCR = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveEPTAMDPCR.id);
                                        _doEPTAMDPCR.id_documentState = idNullState;
                                        db.Document.Attach(_doEPTAMDPCR);
                                        db.Entry(_doEPTAMDPCR).State = EntityState.Modified;
                                        var lstDetailEPTAMDPCR = lastInventoryMoveIPTAMDR?.InventoryMoveDetail.ToList();
                                        if (lstDetailEPTAMDPCR != null)
                                        {
                                            foreach (var det in lstDetailEPTAMDPCR)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, det.exitAmount, db);
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ?
                                                                                         w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDPCR") : false)
                                                                                         && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    InventoryMove lastInventoryMoveIPTAMDPCR = null;
                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveIPTAMDPCR = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveIPTAMDPCR != null)
                                    {
                                        Document _doIPTAMDPCR = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveIPTAMDPCR.id);
                                        _doIPTAMDPCR.id_documentState = idNullState;
                                        db.Document.Attach(_doIPTAMDPCR);
                                        db.Entry(_doIPTAMDPCR).State = EntityState.Modified;
                                        var lstDetailIPTAMDPCR = lastInventoryMoveIPTAMDR?.InventoryMoveDetail.ToList();
                                        if (lstDetailIPTAMDPCR != null)
                                        {
                                            foreach (var det in lstDetailIPTAMDPCR)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, -det.entryAmount, db);
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("ECMDA") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    InventoryMove lastInventoryMoveECMDA = (inventoryMoveDetailAux.Count > 0)
                                                                        ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                        : null;
                                    if (lastInventoryMoveECMDA != null)
                                    {
                                        Document _doECMDA = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveECMDA.id);
                                        _doECMDA.id_documentState = idNullState;
                                        db.Document.Attach(_doECMDA);
                                        db.Entry(_doECMDA).State = EntityState.Modified;
                                        var lstDetailECMDA = lastInventoryMoveECMDA?.InventoryMoveDetail.ToList();
                                        if (lstDetailECMDA != null)
                                        {
                                            foreach (var det in lstDetailECMDA)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, det.exitAmount, db);
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("ERMDA") : false)).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    InventoryMove lastInventoryMoveERMDA = (inventoryMoveDetailAux.Count > 0)
                                                                        ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                        : null;
                                    if (lastInventoryMoveERMDA != null)
                                    {
                                        Document _doECMDA = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveERMDA.id);
                                        _doECMDA.id_documentState = idNullState;
                                        db.Document.Attach(_doECMDA);
                                        db.Entry(_doECMDA).State = EntityState.Modified;
                                        var lstDetailECMDA = lastInventoryMoveERMDA?.InventoryMoveDetail.ToList();
                                        if (lstDetailECMDA != null)
                                        {
                                            foreach (var det in lstDetailECMDA)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, det.exitAmount, db);
                                            }
                                        }
                                    }

                                    lstDispatchMaterials = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.id_warehouse == i && w.sendedAdjustmentQuantity > 0).ToList();

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ?
                                                                                         w.Document.InventoryMove.InventoryReason.code.Equals("EAAMDL") : false)
                                                                                         && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();
                                    InventoryMove lastInventoryMoveEAAMDLWareHouse = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveEAAMDLWareHouse = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveEAAMDLWareHouse != null)
                                    {
                                        Document _doEAAMDL = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveEAAMDLWareHouse.id);
                                        _doEAAMDL.id_documentState = idNullState;
                                        db.Document.Attach(_doEAAMDL);
                                        db.Entry(_doEAAMDL).State = EntityState.Modified;
                                        var lstDetailEAAMDL = lastInventoryMoveEAAMDLWareHouse?.InventoryMoveDetail.ToList();
                                        if (lstDetailEAAMDL != null)
                                        {
                                            foreach (var det in lstDetailEAAMDL)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, det.exitAmount, db);
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                                                          (w.Document.InventoryMove != null ?
                                                                                                                          w.Document.InventoryMove.InventoryReason.code.Equals("IAAMDL") : false)
                                                                                                                          && w.Document.InventoryMove.idWarehouse == idWarehouseProvider
                                                                                                                          ).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    InventoryMove lastInventoryMoveIAAMDLVirtualWareHouse = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveIAAMDLVirtualWareHouse = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveIAAMDLVirtualWareHouse != null)
                                    {
                                        Document _doIAAMDL = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveIAAMDLVirtualWareHouse.id);
                                        _doIAAMDL.id_documentState = idNullState;
                                        db.Document.Attach(_doIAAMDL);
                                        db.Entry(_doIAAMDL).State = EntityState.Modified;
                                        var lstDetailIAAMDL = lastInventoryMoveIAAMDLVirtualWareHouse?.InventoryMoveDetail.ToList();
                                        if (lstDetailIAAMDL != null)
                                        {
                                            foreach (var det in lstDetailIAAMDL)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, -det.entryAmount, db);
                                            }
                                        }
                                    }

                                    //Ajuste Automático Materiales de Despacho en Logistica <0
                                    lstDispatchMaterials = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.id_warehouse == i && w.sendedAdjustmentQuantity < 0).ToList();

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                                                           (w.Document.InventoryMove != null ?
                                                                                                                           w.Document.InventoryMove.InventoryReason.code.Equals("IAAMDL") : false)
                                                                                                                           && w.Document.InventoryMove.idWarehouse == i
                                                                                                                           ).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();
                                    InventoryMove lastInventoryMoveIAAMDLWareHouse = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveIAAMDLWareHouse = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveIAAMDLWareHouse != null)
                                    {
                                        Document _doIAAMDL = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveIAAMDLWareHouse.id);
                                        _doIAAMDL.id_documentState = idNullState;
                                        db.Document.Attach(_doIAAMDL);
                                        db.Entry(_doIAAMDL).State = EntityState.Modified;
                                        var lstDetailIAAMDL = lastInventoryMoveIAAMDLWareHouse?.InventoryMoveDetail.ToList();
                                        if (lstDetailIAAMDL != null)
                                        {
                                            foreach (var det in lstDetailIAAMDL)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, -det.entryAmount, db);
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                       (w.Document.InventoryMove != null ?
                                                                                       w.Document.InventoryMove.InventoryReason.code.Equals("EAAMDL") : false)
                                                                                       && w.Document.InventoryMove.idWarehouse == idWarehouseProvider).OrderByDescending(d => d.Document.dateCreate).ToList();

                                    InventoryMove lastInventoryMoveEAAMDLVirtualWareHouse = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveEAAMDLVirtualWareHouse = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveEAAMDLVirtualWareHouse != null)
                                    {
                                        Document _doEAAMDL = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveEAAMDLVirtualWareHouse.id);
                                        _doEAAMDL.id_documentState = idNullState;
                                        db.Document.Attach(_doEAAMDL);
                                        db.Entry(_doEAAMDL).State = EntityState.Modified;
                                        var lstDetailEAAMDL = lastInventoryMoveEAAMDLVirtualWareHouse?.InventoryMoveDetail.ToList();
                                        if (lstDetailEAAMDL != null)
                                        {
                                            foreach (var det in lstDetailEAAMDL)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, det.exitAmount, db);
                                            }
                                        }
                                    }

                                    //Transferencia Automática entre proveedores en Recepción > 0
                                    lstDispatchMaterials = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.id_warehouse == i && w.transferQuantity > 0).ToList();

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                                                          (w.Document.InventoryMove != null ?
                                                                                                                          w.Document.InventoryMove.InventoryReason.code.Equals("ITAPR") : false)
                                                                                                                          && w.Document.InventoryMove.idWarehouse == i
                                                                                                                          ).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();
                                    InventoryMove lastInventoryMoveETAPRWareHouse = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveETAPRWareHouse = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveETAPRWareHouse != null)
                                    {
                                        Document _doETAPR = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveETAPRWareHouse.id);
                                        _doETAPR.id_documentState = idNullState;
                                        db.Document.Attach(_doETAPR);
                                        db.Entry(_doETAPR).State = EntityState.Modified;
                                        var lstDetailETAPR = lastInventoryMoveETAPRWareHouse?.InventoryMoveDetail.ToList();
                                        if (lstDetailETAPR != null)
                                        {
                                            foreach (var det in lstDetailETAPR)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, det.exitAmount, db);
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                                                          (w.Document.InventoryMove != null ?
                                                                                                                          w.Document.InventoryMove.InventoryReason.code.Equals("ETAPR") : false)
                                                                                                                          && w.Document.InventoryMove.idWarehouse == idWarehouseProvider
                                                                                                                          ).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    InventoryMove lastInventoryMoveETAPRVirtualWareHouse = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveETAPRVirtualWareHouse = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveETAPRVirtualWareHouse != null)
                                    {
                                        Document _doETAPR = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveETAPRVirtualWareHouse.id);
                                        _doETAPR.id_documentState = idNullState;
                                        db.Document.Attach(_doETAPR);
                                        db.Entry(_doETAPR).State = EntityState.Modified;
                                        var lstDetailETAPR = lastInventoryMoveETAPRVirtualWareHouse?.InventoryMoveDetail.ToList();
                                        if (lstDetailETAPR != null)
                                        {
                                            foreach (var det in lstDetailETAPR)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, det.exitAmount, db);
                                            }
                                        }
                                    }

                                    //Transferencia Automática entre proveedores en Recepción < 0
                                    lstDispatchMaterials = receptionDispatchMaterials.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.id_warehouse == i && w.transferQuantity < 0).ToList();

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                      (w.Document.InventoryMove != null ?
                                                                                      w.Document.InventoryMove.InventoryReason.code.Equals("ETAPR") : false)
                                                                                      && w.Document.InventoryMove.idWarehouse == i
                                                                                      ).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    idItem = lstDispatchMaterials.Select(s => s.id_item).FirstOrDefault();
                                    InventoryMove lastInventoryMoveITAPRWareHouse = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveITAPRWareHouse = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveITAPRWareHouse != null)
                                    {
                                        Document _doITAPR = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveITAPRWareHouse.id);
                                        _doITAPR.id_documentState = idNullState;
                                        db.Document.Attach(_doITAPR);
                                        db.Entry(_doITAPR).State = EntityState.Modified;
                                        var lstDetailITAPR = lastInventoryMoveITAPRWareHouse?.InventoryMoveDetail.ToList();
                                        if (lstDetailITAPR != null)
                                        {
                                            foreach (var det in lstDetailITAPR)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, -det.entryAmount, db);
                                            }
                                        }
                                    }

                                    inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                      (w.Document.InventoryMove != null ?
                                                                                      w.Document.InventoryMove.InventoryReason.code.Equals("ITAPR") : false)
                                                                                      && w.Document.InventoryMove.idWarehouse == idWarehouseProvider
                                                                                      ).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    InventoryMove lastInventoryMoveITAPRVirtualWareHouse = null;

                                    if (inventoryMoveDetailAux.Count > 0)
                                    {
                                        foreach (var de in inventoryMoveDetailAux)
                                        {
                                            if (de.Document.InventoryMove.InventoryMoveDetail.Count() > 0)
                                            {
                                                foreach (var dein in de.Document.InventoryMove.InventoryMoveDetail)
                                                {
                                                    if (dein.id_item == idItem)
                                                    {
                                                        lastInventoryMoveITAPRVirtualWareHouse = de.Document.InventoryMove;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (lastInventoryMoveITAPRVirtualWareHouse != null)
                                    {
                                        Document _doITAPR = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveITAPRVirtualWareHouse.id);
                                        _doITAPR.id_documentState = idNullState;
                                        db.Document.Attach(_doITAPR);
                                        db.Entry(_doITAPR).State = EntityState.Modified;
                                        var lstDetailITAPR = lastInventoryMoveITAPRVirtualWareHouse?.InventoryMoveDetail.ToList();
                                        if (lstDetailITAPR != null)
                                        {
                                            foreach (var det in lstDetailITAPR)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, -det.entryAmount, db);
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion Trae todos los documentos originados a partir de esta recepción

                            #region Ahora las Recepciones en Logística también generan un movimiento por cada bodega

                            var lstWarehouses2 = receptionDispatchMaterials.ReceptionDispatchMaterialsDetail.Select(s => s.id_warehouse).Distinct().ToList();
                            if (lstWarehouses2 != null && lstWarehouses2.Count() > 0)
                            {
                                foreach (int i in lstWarehouses2)
                                {
                                    var inventoryMoveDetailAux = lstDocS.Where(w => w.id_documentOrigin == receptionDispatchMaterials.id && !w.Document.DocumentState.code.Equals("05") &&
                                                                                         (w.Document.InventoryMove != null ?
                                                                                         w.Document.InventoryMove.InventoryReason.code.Equals("IMDA") : false)
                                                                                         && w.Document.InventoryMove.idWarehouse == i).OrderByDescending(d => d.Document.dateCreate).ToList();
                                    InventoryMove lastInventoryMoveIMDA = (inventoryMoveDetailAux.Count > 0)
                                                                        ? inventoryMoveDetailAux.First().Document.InventoryMove
                                                                        : null;

                                    if (lastInventoryMoveIMDA != null)
                                    {
                                        Document _doIMDA = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveIMDA.id);
                                        _doIMDA.id_documentState = idNullState;
                                        db.Document.Attach(_doIMDA);
                                        db.Entry(_doIMDA).State = EntityState.Modified;
                                        var lstDetailIMDA = lastInventoryMoveIMDA?.InventoryMoveDetail.ToList();
                                        if (lstDetailIMDA != null)
                                        {
                                            foreach (var det in lstDetailIMDA)
                                            {
                                                ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
                                                    det.InventoryMove.idWarehouse, det.id_warehouseLocation, det.exitAmount, db);
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion Ahora las Recepciones en Logística también generan un movimiento por cada bodega

                            #endregion New Version
                        }

                        receptionDispatchMaterials.Document.id_documentState = documentState.id;
                        receptionDispatchMaterials.Document.DocumentState = documentState;

                        db.ReceptionDispatchMaterials.Attach(receptionDispatchMaterials);
                        db.Entry(receptionDispatchMaterials).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        isSaved = true;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("receptionDispatchMaterials");
                    ViewData["EditMessage"] = ErrorMessage("No se puede Reversar la Recepción de Materiales debido a: " + e.Message);
                    return PartialView("_ReceptionDispatchMaterialsEditFormPartial", receptionDispatchMaterials);
                }
            }

            if (isSaved)
            {
                if (receptionDispatchMaterials.id > 0)
                {
                    GenerateResultForLiquidationMaterial(receptionDispatchMaterials.id);
                }
            }
            TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");
            ViewData["EditMessage"] = SuccessMessage("Recepción de Materiales: " + receptionDispatchMaterials.Document.number + " reversada exitosamente");

            return PartialView("_ReceptionDispatchMaterialsEditFormPartial", receptionDispatchMaterials);
        }

        #endregion SINGLE CHANGE STATE

        #region SELECTED ReceptionDispatchMaterials STATE CHANGE

        [HttpPost, ValidateInput(false)]
        public void ApproveDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");

                        foreach (var id in ids)
                        {
                            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

                            if (purchasePlanning != null && documentState != null)
                            {
                                purchasePlanning.Document.id_documentState = documentState.id;
                                purchasePlanning.Document.DocumentState = documentState;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<PurchasePlanning>);
            model = model ?? new List<PurchasePlanning>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void AutorizeDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06");

                        foreach (var id in ids)
                        {
                            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

                            if (purchasePlanning != null && documentState != null)
                            {
                                purchasePlanning.Document.id_documentState = documentState.id;
                                purchasePlanning.Document.DocumentState = documentState;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<PurchasePlanning>);
            model = model ?? new List<PurchasePlanning>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void ProtectDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "04");

                        foreach (var id in ids)
                        {
                            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

                            if (purchasePlanning != null && documentState != null)
                            {
                                purchasePlanning.Document.id_documentState = documentState.id;
                                purchasePlanning.Document.DocumentState = documentState;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<PurchasePlanning>);
            model = model ?? new List<PurchasePlanning>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void CancelDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

                        foreach (var id in ids)
                        {
                            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

                            if (purchasePlanning != null && documentState != null)
                            {
                                purchasePlanning.Document.id_documentState = documentState.id;
                                purchasePlanning.Document.DocumentState = documentState;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<PurchasePlanning>);
            model = model ?? new List<PurchasePlanning>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void RevertDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

                            if (purchasePlanning != null)
                            {
                                var codeAux = purchasePlanning.Document.DocumentState.code == "05"
                                    ? "04"
                                    : (purchasePlanning.Document.DocumentState.code == "04"
                                        ? "06"
                                        : (purchasePlanning.Document.DocumentState.code == "06"
                                            ? "03"
                                            : "01"));
                                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == codeAux);
                                if (documentState != null)
                                {
                                    purchasePlanning.Document.id_documentState = documentState.id;
                                    purchasePlanning.Document.DocumentState = documentState;

                                    db.PurchasePlanning.Attach(purchasePlanning);
                                    db.Entry(purchasePlanning).State = EntityState.Modified;

                                    db.SaveChanges();
                                    trans.Commit();
                                }
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<PurchasePlanning>);
            model = model ?? new List<PurchasePlanning>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        #endregion SELECTED ReceptionDispatchMaterials STATE CHANGE

        #region ReceptionDispatchMaterials REPORTS

        [HttpPost]
        public ActionResult OpeningClosingPlateLyingReport(int id)
        {
            PurchasePlanningReport purchasePlanningReport = new PurchasePlanningReport();
            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(pp => pp.id == id);
            var id_companyAux = (purchasePlanning != null ? purchasePlanning.id_company : this.ActiveCompanyId);
            Company company = db.Company.FirstOrDefault(c => c.id == id_companyAux);
            purchasePlanningReport.DataSource = new PurchasePlanningCompany
            {
                number = purchasePlanning?.Document.number ?? "",
                state = purchasePlanning?.Document.DocumentState.name ?? "",
                emissionDate = purchasePlanning?.Document.emissionDate.ToString("dd/MM/yyyy") ?? "",
                period = purchasePlanning?.PurchasePlanningPeriod.name ?? "",
                personPlanning = purchasePlanning?.Employee.Person.fullname_businessName ?? "",
                description = purchasePlanning?.Document.description ?? "",
                list_id_purchasePlanning = new List<int>(id),
                listPurchasePlanningDetail = purchasePlanning?.PurchasePlanningDetail.AsEnumerable()
                                                                                     .Select(s => new PurchasePlanningDetailReport
                                                                                     {
                                                                                         datePlanningStr = s.datePlanning.ToString("ddd").ToUpper() +
                                                                                                           s.datePlanning.ToString("_dd"),
                                                                                         datePlanning = s.datePlanning,
                                                                                         id_provider = s.id_provider,
                                                                                         provider = s.Provider.Person.fullname_businessName,
                                                                                         id_buyer = s.id_buyer,
                                                                                         buyer = s.Person.fullname_businessName,
                                                                                         id_item = s.id_item ?? 0,
                                                                                         item = s.Item?.name ?? "",
                                                                                         id_itemTypeCategory = s.id_itemTypeCategory,
                                                                                         itemTypeCategory = s.ItemTypeCategory.name,
                                                                                         quantity = s.quantity
                                                                                     }).OrderBy(ob => ob.id_itemTypeCategory)
                                                                                     .OrderBy(ob => ob.id_item)
                                                                                     .OrderBy(ob => ob.id_buyer)
                                                                                     .OrderBy(ob => ob.id_provider)
                                                                                     .OrderBy(ob => ob.datePlanning)
                                                                                     .ToList(),
                company = company
            };
            return PartialView("_PurchasePlanningReport", purchasePlanningReport);
        }

        #endregion ReceptionDispatchMaterials REPORTS

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            var actions = new
            {
                btnNew = true,
                btnApprove = true,
                btnAutorize = false,
                btnProtect = false,
                btnCancel = false,
                btnRevert = false,
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            ReceptionDispatchMaterials receptionDispatchMaterials = db.ReceptionDispatchMaterials.FirstOrDefault(r => r.id == id);
            string code_state = receptionDispatchMaterials.Document.DocumentState.code;

            if (code_state == "01") // PENDIENTE
            {
                actions = new
                {
                    btnNew = true,
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = false,
                };
            }
            else if (code_state == "03") // APROBADA
            {
                actions = new
                {
                    btnNew = true,
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                };
            }
            else if (code_state == "05") // 05 ANULADA
            {
                actions = new
                {
                    btnNew = true,
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion ACTIONS

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_receptionDispatchMaterials)
        {
            TempData.Keep("receptionDispatchMaterials");

            int[] ids = db.ReceptionDispatchMaterials.Select(r => r.id).ToArray();
            int index = ids.OrderByDescending(r => r)
                           .ToList()
                           .FindIndex(r => r == id_receptionDispatchMaterials);

            var result = new
            {
                maximunPages = ids.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = db.ReceptionDispatchMaterials.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (receptionDispatchMaterials != null)
            {
                TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
                TempData.Keep("receptionDispatchMaterials");
                return PartialView("_ReceptionDispatchMaterialsEditFormPartial", receptionDispatchMaterials);
            }

            TempData.Keep("receptionDispatchMaterials");

            return PartialView("_ReceptionDispatchMaterialsEditFormPartial", new ReceptionDispatchMaterials());
        }

        #endregion PAGINATION

        #region AXILIAR FUNCTIONS

        private void GenerateResultForLiquidationMaterial(int idReceptionDispatchMaterial)
        {
            using (DBContext db = new DBContext())
            {
                try
                {
                    var _rdm = db.ReceptionDispatchMaterials.FirstOrDefault(fod => fod.id == idReceptionDispatchMaterial);

                    if (_rdm != null)
                    {
                        ResultReceptionDispatchMaterial _rrdm;

                        if (_rdm.ResultReceptionDispatchMaterial == null)
                        {
                            _rrdm = new ResultReceptionDispatchMaterial
                            {
                                numberRemissionGuide = _rdm.RemissionGuide.Document.sequential.ToString(),
                                dateRemissionGuide = _rdm.RemissionGuide.despachureDate,
                                codeStateReceptionDispatchMaterial = _rdm.Document.DocumentState.code,
                                nameState = _rdm.RemissionGuide.Document.DocumentState.name,
                                idProvider = (int)_rdm.RemissionGuide.id_providerRemisionGuide,
                                nameProvider = _rdm.RemissionGuide.Provider1.Person.fullname_businessName,
                                nameProviderShrimp = _rdm.RemissionGuide.ProductionUnitProvider.name,
                                numberRecepctionDispatchMaterials = _rdm.Document.sequential.ToString(),
                                dateReception = _rdm.Document.emissionDate,
                                idPersonProcessPlant = _rdm.RemissionGuide.id_personProcessPlant,
                                personProcessPlant = db.Person
                                    .FirstOrDefault(p => p.id == _rdm.RemissionGuide.id_personProcessPlant)
                                    ?.processPlant
                            };
                            var lst = _rdm.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.isActive).ToList();

                            _rrdm.ResultReceptionDispatchMaterialDetail = new List<ResultReceptionDispatchMaterialDetail>();

                            foreach (var det in lst)
                            {
                                ResultReceptionDispatchMaterialDetail rrdmd = new ResultReceptionDispatchMaterialDetail
                                {
                                    idItem = det.id_item,
                                    idMetricUnit = det.Item.ItemInventory.id_metricUnitInventory,
                                    quantity = (det.sendedDestinationQuantity + det.sendedAdjustmentQuantity) - det.amountConsumed - det.arrivalDestinationQuantity - det.stealQuantity - det.transferQuantity
                                };
                                db.ResultReceptionDispatchMaterialDetail.Attach(rrdmd);
                                db.Entry(rrdmd).State = EntityState.Added;
                                _rrdm.ResultReceptionDispatchMaterialDetail.Add(rrdmd);
                            }
                            _rdm.ResultReceptionDispatchMaterial = _rrdm;
                            db.ResultReceptionDispatchMaterial.Attach(_rrdm);
                            db.Entry(_rrdm).State = EntityState.Added;
                        }
                        else
                        {
                            _rrdm = _rdm.ResultReceptionDispatchMaterial;
                            _rrdm.numberRemissionGuide = _rdm.RemissionGuide.Document.sequential.ToString();
                            _rrdm.dateRemissionGuide = _rdm.RemissionGuide.despachureDate;
                            _rrdm.codeStateReceptionDispatchMaterial = _rdm.Document.DocumentState.code;
                            _rrdm.nameState = _rdm.RemissionGuide.Document.DocumentState.name;
                            _rrdm.idProvider = (int)_rdm.RemissionGuide.id_providerRemisionGuide;
                            _rrdm.nameProvider = _rdm.RemissionGuide.Provider1.Person.fullname_businessName;
                            _rrdm.nameProviderShrimp = _rdm.RemissionGuide.ProductionUnitProvider.name;
                            _rrdm.numberRecepctionDispatchMaterials = _rdm.Document.sequential.ToString();
                            _rrdm.dateReception = _rdm.Document.emissionDate;
                            _rrdm.idPersonProcessPlant = _rdm.RemissionGuide.id_personProcessPlant;
                            _rrdm.personProcessPlant = db.Person
                                .FirstOrDefault(p => p.id == _rdm.RemissionGuide.id_personProcessPlant)
                                ?.processPlant;

                            var lst = _rdm.RemissionGuide.RemissionGuideDispatchMaterial.Where(w => w.isActive).ToList();
                            _rrdm.ResultReceptionDispatchMaterialDetail = _rrdm.ResultReceptionDispatchMaterialDetail ?? new List<ResultReceptionDispatchMaterialDetail>();

                            List<ResultReceptionDispatchMaterialDetail> lstDetail = _rrdm.ResultReceptionDispatchMaterialDetail.ToList();

                            foreach (var det in lst)
                            {
                                ResultReceptionDispatchMaterialDetail _rrdmd = lstDetail.FirstOrDefault(fod => fod.idItem == det.id_item);
                                if (_rrdmd != null)
                                {
                                    _rrdmd.idMetricUnit = det.Item.ItemInventory.id_metricUnitInventory;
                                    _rrdmd.quantity = (det.sendedDestinationQuantity + det.sendedAdjustmentQuantity) - det.amountConsumed - det.arrivalDestinationQuantity - det.stealQuantity - det.transferQuantity;

                                    db.ResultReceptionDispatchMaterialDetail.Attach(_rrdmd);
                                    db.Entry(_rrdmd).State = EntityState.Modified;
                                }
                                else
                                {
                                    _rrdmd = new ResultReceptionDispatchMaterialDetail
                                    {
                                        idItem = det.id_item,
                                        idMetricUnit = det.Item.ItemInventory.id_metricUnitInventory,
                                        quantity = (det.sendedDestinationQuantity + det.sendedAdjustmentQuantity) - det.amountConsumed - det.arrivalDestinationQuantity - det.stealQuantity - det.transferQuantity
                                    };
                                    db.ResultReceptionDispatchMaterialDetail.Attach(_rrdmd);
                                    db.Entry(_rrdmd).State = EntityState.Added;
                                    _rrdm.ResultReceptionDispatchMaterialDetail.Add(_rrdmd);
                                }
                            }

                            _rdm.ResultReceptionDispatchMaterial = _rrdm;
                            db.ResultReceptionDispatchMaterial.Attach(_rrdm);
                            db.Entry(_rrdm).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                }
                catch
                {
                }
            }
        }

        [HttpPost]
        public JsonResult ItemDetailData(int? id_item)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            var result = new
            {
                masterCode = item?.masterCode ?? "",
                metricUnit = item?.ItemInventory?.MetricUnit.code ?? "",
            };

            TempData.Keep("receptionDispatchMaterials");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetOpeningClosingPlateLyingIds_freezerWarehouseLocation()
        {
            OpeningClosingPlateLying openingClosingPlateLying = (TempData["openingClosingPlateLying"] as OpeningClosingPlateLying);
            openingClosingPlateLying = openingClosingPlateLying ?? new OpeningClosingPlateLying();
            List<int> ids_freezerWarehouseLocation = openingClosingPlateLying.ids_freezerWarehouseLocation == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(openingClosingPlateLying.ids_freezerWarehouseLocation);
            string list_idFreezerWarehouseLocationStr = "";
            foreach (var i in ids_freezerWarehouseLocation)
            {
                if (i != 0)
                {
                    if (list_idFreezerWarehouseLocationStr == "") list_idFreezerWarehouseLocationStr = i.ToString();
                    else list_idFreezerWarehouseLocationStr += "," + i.ToString();
                }
            }
            var result = new
            {
                ids_freezerWarehouseLocationParam = list_idFreezerWarehouseLocationStr,
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == openingClosingPlateLying.id_freezerWarehouse)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       })
            };

            TempData.Keep("openingClosingPlateLying");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateOpeningClosingPlateLyingMaintenanceWarehouseLocation(int? id_maintenanceWarehouse)
        {
            var result = new
            {
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_maintenanceWarehouse)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       })
            };

            TempData.Keep("openingClosingPlateLying");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateOpeningClosingPlateLyingFreezerWarehouseLocation(int? id_freezerWarehouse)
        {
            var result = new
            {
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_freezerWarehouse)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       })
            };

            UpdateOpeningClosingPlateLyingDetail(id_freezerWarehouse, null);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateOpeningClosingPlateLyingDetail(int? id_freezerWarehouse, int[] ids_freezerWarehouseLocation)
        {
            OpeningClosingPlateLying openingClosingPlateLying = (TempData["openingClosingPlateLying"] as OpeningClosingPlateLying);
            openingClosingPlateLying = openingClosingPlateLying ?? new OpeningClosingPlateLying();

            if (ids_freezerWarehouseLocation != null && ids_freezerWarehouseLocation.Length > 0)
            {
                var tempfreezerWarehouseLocation = new List<int>();
                foreach (var i in ids_freezerWarehouseLocation)
                {
                    tempfreezerWarehouseLocation.Add(i);
                }

                openingClosingPlateLying.ids_freezerWarehouseLocation = JsonConvert.SerializeObject(tempfreezerWarehouseLocation);
            }
            else
            {
                openingClosingPlateLying.ids_freezerWarehouseLocation = JsonConvert.SerializeObject(new List<int>());
            }

            openingClosingPlateLying.id_freezerWarehouse = id_freezerWarehouse ?? 0;
            openingClosingPlateLying.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == id_freezerWarehouse);

            var result = new
            {
                Message = "Ok"
            };

            TempData["openingClosingPlateLying"] = openingClosingPlateLying;

            //UpdateDetail(openingClosingPlateLying);

            TempData["openingClosingPlateLying"] = openingClosingPlateLying;
            TempData.Keep("openingClosingPlateLying");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateInvoiceCommercialDetail(int? id_itemCurrent, string numBoxesCurrent, string unitPriceCurrent)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_itemCurrent);

            InvoiceCommercial invoiceCommercial = (TempData["invoiceCommercial"] as InvoiceCommercial);

            invoiceCommercial = invoiceCommercial ?? new InvoiceCommercial();

            var result = new
            {
                itemInvoiceCommercialAuxCode = "",
                itemInvoiceCommercialMasterCode = "",
                amountInvoice = (decimal)0,
                itemInvoiceCommercialUM = "",
                invoiceCommercialTotal = (decimal)0
            };

            if (item == null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            decimal _numBoxesCurrent = Convert.ToDecimal(numBoxesCurrent);
            decimal _unitPriceCurrent = Convert.ToDecimal(unitPriceCurrent);
            _numBoxesCurrent = decimal.Round(_numBoxesCurrent, 2);
            _unitPriceCurrent = decimal.Round(_unitPriceCurrent, 2);
            decimal amount = 0;
            decimal invoiceCommercialTotal = 0;
            if (item.Presentation != null)
            {
                amount = _numBoxesCurrent * item.Presentation.maximum * item.Presentation.minimum;
                amount = decimal.Round(amount, 2);
            }
            var id_metricUnitAux = item.Presentation?.id_metricUnit;
            var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                            muc.id_metricOrigin == id_metricUnitAux &&
                                                                                            muc.id_metricDestiny == invoiceCommercial.id_metricUnitInvoice);
            var factor = (id_metricUnitAux == invoiceCommercial.id_metricUnitInvoice) ? 1 : (metricUnitConversion?.factor ?? 0);
            var amountInvoice = decimal.Round((amount * factor), 2);
            invoiceCommercialTotal = amountInvoice * _unitPriceCurrent;
            invoiceCommercialTotal = decimal.Round(invoiceCommercialTotal, 2);

            result = new
            {
                itemInvoiceCommercialAuxCode = item.auxCode,
                itemInvoiceCommercialMasterCode = item.masterCode,
                amountInvoice = amountInvoice,
                itemInvoiceCommercialUM = item.Presentation?.MetricUnit.code ?? "",
                invoiceCommercialTotal = invoiceCommercialTotal
            };

            TempData.Keep("invoiceCommercial");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult WarehouseLocationReceptionDispatchMaterialsDetailCombo_OnInit()
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);
            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            var warehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id_person == receptionDispatchMaterials.RemissionGuide.id_providerRemisionGuide &&
                                                                               fod.Warehouse.code == "VIRPRO");

            var result = new
            {
                Message = "Ok",
                id_warehouse = warehouseLocation?.id_warehouse,
                id_warehouseLocation = warehouseLocation?.id,
                name_warehouseLocation = warehouseLocation?.name
            };

            TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;

            TempData.Keep("receptionDispatchMaterials");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetWarehouseLocationReceptionDispatchMaterialsDetail(int? id_warehouseCurrent, int? id_warehouseLocationCurrent)
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);

            Warehouse warehouseAux = db.Warehouse.FirstOrDefault(i => i.id == id_warehouseCurrent);

            var warehouseLocation = warehouseAux?.WarehouseLocation.Where(w => w.id != id_warehouseLocationCurrent).ToList() ?? new List<WarehouseLocation>();

            TempData.Keep("receptionDispatchMaterials");
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                //settings.Name = "id_person";
                p.ClientInstanceName = "id_warehouseLocationReceptionDispatchMaterialsDetail";
                p.Width = Unit.Percentage(100);

                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "ReceptionDispatchMaterials", Action = "GetWarehouseLocationReceptionDispatchMaterialsDetail"/*, TextField = "CityName"*/ };
                p.ClientSideEvents.BeginCallback = "WarehouseLocationReceptionDispatchMaterialsDetail_BeginCallback";
                p.ClientSideEvents.EndCallback = "WarehouseLocationReceptionDispatchMaterialsDetail_EndCallback";
                p.CallbackPageSize = 5;
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);

                p.ClientSideEvents.Validation = "OnWarehouseLocationReceptionDispatchMaterialsDetailValidation";
                p.ClientSideEvents.Init = "WarehouseLocationReceptionDispatchMaterialsDetailCombo_OnInit";
                p.BindList(warehouseLocation);
            });
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedDetail(int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);
            receptionDispatchMaterials = receptionDispatchMaterials ?? new ReceptionDispatchMaterials();

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var receptionDispatchMaterialsAux = receptionDispatchMaterials.ReceptionDispatchMaterialsDetail.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_warehouse == id_warehouseNew &&
                                                                                fod.id_warehouseLocation == id_warehouseLocationNew);
            if (receptionDispatchMaterialsAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles."
                };
            }

            TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;
            TempData.Keep("receptionDispatchMaterials");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion AXILIAR FUNCTIONS
    }
}