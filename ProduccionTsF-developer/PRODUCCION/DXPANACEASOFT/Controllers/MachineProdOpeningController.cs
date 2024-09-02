using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Reports.PurchasePlanning;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class MachineProdOpeningController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region Machine Prod Opening EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult MachineProdOpeningFormEditPartial(int id)
        {
            MachineProdOpening machineProdOpening = db.MachineProdOpening.FirstOrDefault(r => r.id == id);

            if (machineProdOpening == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("73"));   
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");  

                Employee employee = ActiveUser.Employee;

                machineProdOpening = new MachineProdOpening
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
                    MachineProdOpeningDetail = new List<MachineProdOpeningDetail>()
                };
            }

            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");

            return PartialView("_FormEditMachineProdOpening", machineProdOpening);
        }

        #endregion Machine Prod Opening EDITFORM

        #region ResultGridView

        [ValidateInput(false)]
        public ActionResult MachineProdOpeningResultsPartial(int? id_documentState, string number, 
                                                             DateTime? startEmissionDate, DateTime? endEmissionDate, 
                                                             int[] turns, int[] machineForProds, int[] persons)
        {
            List<MachineProdOpening> model = db.MachineProdOpening.ToList();

            #region Filtros Aplicados a cabeceras

            var idsMaquinas = ListMachineByUserRol();

            if (id_documentState != 0 && id_documentState != null)
            {
                model = model.Where(o => o.Document.id_documentState == id_documentState).ToList();
            }

            if (!string.IsNullOrEmpty(number))
            {
                model = model.Where(o => o.Document.number.Contains(number)).ToList();
            }

            if (startEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
            }

            if (endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            if (turns != null && turns.Length > 0)
            {
                model = model.Where(e => turns.Contains(e.id_Turn)).ToList();
            }

            #endregion Filtros Aplicados a cabeceras

            #region Filtros aplicados a detalles

            var details = new MachineProdOpeningDetail[] { };
            if ((idsMaquinas?.Any() ?? false)
                || (machineForProds?.Any() ?? false)
                || (persons?.Any() ?? false))
            {
                var idsMachineProdOpFiltred = model.Select(e => e.id).ToList();
                details = db.MachineProdOpeningDetail
                    .Where(e => idsMachineProdOpFiltred.Contains(e.id_MachineProdOpening))
                    .ToArray();

                if (idsMaquinas?.Any() ?? false)
                {
                    details = details.Where(e => idsMaquinas.Contains(e.id_MachineForProd)).ToArray();
                }

                if (machineForProds?.Any() ?? false)
                {
                    details = details.Where(e => machineForProds.Contains(e.id_MachineForProd)).ToArray();
                }

                if (persons?.Any() ?? false)
                {
                    details = details.Where(e => persons.Contains(e.id_Person)).ToArray();
                }

                var idsMachineProdOpDetails = details
                    .Select(e => e.id_MachineProdOpening)
                    .ToList()
                    .Distinct();

                model = model.Where(e => idsMachineProdOpDetails.Contains(e.id)).ToList();
            }

            #endregion Filtros aplicados a detalles

            if (model.Count() > 0)
            {
                foreach (var m in model)
                {
                    var detallesApertura = details.Where(e => e.id_MachineProdOpening == m.id).ToList();

                    if (!detallesApertura.Any())
                    {
                        detallesApertura = db.MachineProdOpeningDetail.Where(x => x.id_MachineProdOpening == m.id).ToList();
                    }

                    m.MachineProdOpeningDetail = detallesApertura;
                    foreach (var n in m.MachineProdOpeningDetail)
                    {
                        n.MachineForProd = db.MachineForProd.FirstOrDefault(y => y.id == n.id_MachineForProd);
                        m.MachineName = n.MachineForProd.name;
                        m.personProcessName = n.MachineForProd.Person.processPlant;
                    }
                }
            }

            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                if (entityPermissions != null)
                {
                    var tempModel = new List<MachineProdOpening>();
                    foreach (var item in model)
                    {
                        var inventoryMoveDetail = item.MachineProdOpeningDetail
                            .FirstOrDefault(fod => entityPermissions.listValue
                                .FirstOrDefault(fod2 => fod2.id_entityValue == fod.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) == null);
                        if (inventoryMoveDetail == null)
                        {
                            tempModel.Add(item);
                        }
                    }

                    model = tempModel;
                }
            }

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_MachineProdOpeningResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        #endregion ResultGridView

        #region Machine Prod Opening

        [HttpPost]
        public ActionResult MachineProdOpeningPartial()
        {
            var model = (TempData["model"] as List<MachineProdOpening>);
            model = model ?? new List<MachineProdOpening>();

            TempData.Keep("model");
            return PartialView("_MachineProdOpeningPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MachineProdOpeningAddNew(bool approve, MachineProdOpening item, Document itemDoc)
        {
            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);
            machineProdOpening = machineProdOpening ?? new MachineProdOpening();

            machineProdOpening.Document.emissionDate = itemDoc.emissionDate;
            machineProdOpening.Document.description = itemDoc.description;

            machineProdOpening.id_Turn = item.id_Turn;

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region Document

                        if (!machineProdOpening.MachineProdOpeningDetail.Any())
                        {
                            throw new Exception("Es obligatorio tener al menos un detalle en la apertura de máquina.");
                        }

                        if (!approve && machineProdOpening.MachineProdOpeningDetail.Count() > 1)
                        {
                            throw new Exception("No puede tener mas de un detalle en la apertura de máquina.");
                        }

                        item.Document = item.Document ?? new Document();
                        item.Document.id_userCreate = ActiveUser.id;
                        item.Document.dateCreate = DateTime.Now;
                        item.Document.id_userUpdate = ActiveUser.id;
                        item.Document.dateUpdate = DateTime.Now;

                        DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == "73");   
                        if (documentType == null)
                        {
                            TempData.Keep("machineProdOpening");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la Apertura de Máquina porque no existe el Tipo de Documento: Apertura de Máquina con Código: 73, configúrelo e inténtelo de nuevo");
                            return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
                        }
                        item.Document.id_documentType = documentType.id;
                        item.Document.DocumentType = documentType;
                        item.Document.sequential = GetDocumentSequential(item.Document.id_documentType);
                        item.Document.number = GetDocumentNumber(item.Document.id_documentType);

                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        if (documentState == null)
                        {
                            TempData.Keep("machineProdOpening");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la Apertura de Máquina porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                            return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
                        }
                        item.Document.id_documentState = documentState.id;
                        item.Document.DocumentState = documentState;

                        item.Document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
                        item.Document.id_emissionPoint = ActiveEmissionPoint.id;

                        item.Document.emissionDate = itemDoc.emissionDate;
                        item.Document.description = itemDoc.description;

                        if (documentType != null)
                        {
                            documentType.currentNumber = documentType.currentNumber + 1;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;
                        }

                        #endregion Document

                        #region MachineProdOpening

                        item.Turn = db.Turn.FirstOrDefault(fod => fod.id == item.id_Turn);
                        machineProdOpening.Turn = item.Turn;

                        #endregion MachineProdOpening

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                            if (entityPermissions != null)
                            {
                                foreach (var detail in machineProdOpening.MachineProdOpeningDetail)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new Exception("No tiene Permiso para editar y guardar la apertura de máquina.");
                                    }
                                }
                                if (approve)
                                {
                                    foreach (var detail in machineProdOpening.MachineProdOpeningDetail)
                                    {
                                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                        if (entityValuePermissions == null)
                                        {
                                            throw new Exception("No tiene Permiso para aprobar la apertura de máquina.");
                                        }
                                    }
                                }
                            }
                        }

                        #region MachineProdOpeningDetail

                        item.MachineProdOpeningDetail = new List<MachineProdOpeningDetail>();
                        if (machineProdOpening.MachineProdOpeningDetail != null)
                        {
                            foreach (var detail in machineProdOpening.MachineProdOpeningDetail)
                            {
                                var machineProdOpeningDetail = new MachineProdOpeningDetail();
                                machineProdOpeningDetail.id_MachineProdOpening = item.id;
                                machineProdOpeningDetail.id_MachineForProd = detail.id_MachineForProd;
                                machineProdOpeningDetail.MachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == detail.id_MachineForProd);
                                machineProdOpeningDetail.id_Person = detail.id_Person;
                                machineProdOpeningDetail.Person = db.Person.FirstOrDefault(fod => fod.id == detail.id_Person);
                                machineProdOpeningDetail.timeInit = detail.timeInit;
                                machineProdOpeningDetail.timeEnd = detail.timeEnd;
                                machineProdOpeningDetail.numPerson = detail.numPerson;

                                if (!machineProdOpeningDetail.MachineForProd.available)
                                {
                                    throw new Exception("Máquina: " + machineProdOpeningDetail.MachineForProd.name + "no está disponible por motivo: " + machineProdOpeningDetail.MachineForProd.reason + ".");
                                }

                                item.MachineProdOpeningDetail.Add(machineProdOpeningDetail);
                            }
                        }

                        #endregion MachineProdOpeningDetail

                        if (approve)
                        {
                            item.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); 
                        }

                        db.MachineProdOpening.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        TempData["machineProdOpening"] = item;
                        TempData.Keep("machineProdOpening");

                        ViewData["EditMessage"] = SuccessMessage("Apertura de Máquina: " + item.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData["machineProdOpening"] = machineProdOpening;
                        TempData.Keep("machineProdOpening");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                        return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            return PartialView("_MachineProdOpeningEditFormPartial", item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MachineProdOpeningUpdate(bool approve, MachineProdOpening item, Document itemDoc)
        {
            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);
            machineProdOpening = machineProdOpening ?? new MachineProdOpening();

            machineProdOpening.Document.emissionDate = itemDoc.emissionDate;
            machineProdOpening.Document.description = itemDoc.description;

            machineProdOpening.id_Turn = item.id_Turn;

            var modelItem = db.MachineProdOpening.FirstOrDefault(p => p.id == item.id);

            if (ModelState.IsValid && modelItem != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region DOCUMENT

                        if (!machineProdOpening.MachineProdOpeningDetail.Any())
                        {
                            throw new Exception("Es obligatorio tener al menos un detalle en la apertura de máquina.");
                        }

                        if (!approve && machineProdOpening.MachineProdOpeningDetail.Count() > 1)
                        {
                            throw new Exception("No puede tener mas de un detalle en la apertura de máquina.");
                        }

                        modelItem.Document.description = itemDoc.description;
                        modelItem.Document.id_userUpdate = ActiveUser.id;
                        modelItem.Document.dateUpdate = DateTime.Now;

                        #endregion DOCUMENT

                        #region MachineProdOpening

                        modelItem.id_Turn = item.id_Turn;
                        modelItem.Turn = db.Turn.FirstOrDefault(fod => fod.id == item.id_Turn);
                        machineProdOpening.Turn = item.Turn;

                        #endregion MachineProdOpening

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                            if (entityPermissions != null)
                            {
                                foreach (var detail in machineProdOpening.MachineProdOpeningDetail)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new Exception("No tiene Permiso para editar y guardar la apertura de máquina.");
                                    }
                                }
                                if (approve)
                                {
                                    foreach (var detail in machineProdOpening.MachineProdOpeningDetail)
                                    {
                                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                        if (entityValuePermissions == null)
                                        {
                                            throw new Exception("No tiene Permiso para aprobar la apertura de máquina.");
                                        }
                                    }
                                }
                            }
                        }

                        #region MachineProdOpeningDetail

                        if (modelItem.MachineProdOpeningDetail != null)
                        {
                            for (int i = modelItem.MachineProdOpeningDetail.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.MachineProdOpeningDetail.ElementAt(i);
                                modelItem.MachineProdOpeningDetail.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            foreach (var detail in machineProdOpening.MachineProdOpeningDetail)
                            {
                                var machineProdOpeningDetail = new MachineProdOpeningDetail();
                                machineProdOpeningDetail.id_MachineProdOpening = modelItem.id;

                                machineProdOpeningDetail.id_MachineForProd = detail.id_MachineForProd;
                                machineProdOpeningDetail.MachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == detail.id_MachineForProd);
                                machineProdOpeningDetail.id_Person = detail.id_Person;
                                machineProdOpeningDetail.Person = db.Person.FirstOrDefault(fod => fod.id == detail.id_Person);

                                machineProdOpeningDetail.timeInit = detail.timeInit;
                                machineProdOpeningDetail.timeEnd = detail.timeEnd;
                                machineProdOpeningDetail.numPerson = detail.numPerson;

                                if (!machineProdOpeningDetail.MachineForProd.available)
                                {
                                    throw new Exception("Máquina: " + machineProdOpeningDetail.MachineForProd.name + "no está disponible por motivo: " + machineProdOpeningDetail.MachineForProd.reason + ".");
                                }
                                modelItem.MachineProdOpeningDetail.Add(machineProdOpeningDetail);
                            }
                        }

                        #endregion MachineProdOpeningDetail

                        if (approve)
                        {
                            modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); 
                        }

                        db.MachineProdOpening.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["machineProdOpening"] = modelItem;
                        TempData.Keep("machineProdOpening");

                        ViewData["EditMessage"] = SuccessMessage("Apertura de Máquina: " + modelItem.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData["machineProdOpening"] = machineProdOpening;
                        TempData.Keep("machineProdOpening");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                        return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);

                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor , corrija todos los errores.";

            return PartialView("_MachineProdOpeningEditFormPartial", modelItem);
        }

        private void UpdateInvoiceCommercialTotals(InvoiceCommercial invoiceCommercial)
        {
            invoiceCommercial.totalBoxes = 0;
            invoiceCommercial.totalWeight = 0.0M;
            invoiceCommercial.totalValue = 0.0M;

            foreach (var invoiceCommercialDetail in invoiceCommercial.InvoiceCommercialDetail)
            {
                invoiceCommercial.totalBoxes += invoiceCommercialDetail.numBoxes;
                invoiceCommercial.totalWeight += decimal.Round(invoiceCommercialDetail.amountInvoice, 2);
                invoiceCommercial.totalValue += decimal.Round(invoiceCommercialDetail.total, 2);
            }
        }

        #endregion Machine Prod Opening

        #region MachineProdOpeningDetail

        [ValidateInput(false)]
        public ActionResult MachineProdOpeningEditFormDetailPartial()
        {
            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);

            machineProdOpening = machineProdOpening ?? new MachineProdOpening();

            var model = machineProdOpening?.MachineProdOpeningDetail.OrderBy(od => od.id).ToList() ?? new List<MachineProdOpeningDetail>();

            TempData["machineProdOpening"] = TempData["machineProdOpening"] ?? machineProdOpening;
            TempData.Keep("machineProdOpening");

            return PartialView("_MachineProdOpeningEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MachineProdOpeningEditFormDetailAddNew(MachineProdOpeningDetail item)
        {
            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);

            machineProdOpening = machineProdOpening ?? new MachineProdOpening();

            machineProdOpening.MachineProdOpeningDetail = machineProdOpening.MachineProdOpeningDetail ?? new List<MachineProdOpeningDetail>();

            try
            {
                if (Request.Params["errorMessage"] != "") throw new Exception("Por favor, corrija todos los errores.");

                item.id = machineProdOpening.MachineProdOpeningDetail.Count() > 0 ? machineProdOpening.MachineProdOpeningDetail.Max(ppd => ppd.id) + 1 : 1;
                item.MachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == item.id_MachineForProd);
                item.Person = db.Person.FirstOrDefault(fod => fod.id == item.id_Person);

                item.timeInit = TimeSpan.Parse(Request.Params["timeInitDetail"]);
                item.timeEnd = TimeSpan.Parse(Request.Params["timeEndDetail"]);
                item.numPerson = item.numPerson;
                machineProdOpening.MachineProdOpeningDetail.Add(item);
                TempData["machineProdOpening"] = machineProdOpening;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");
            var model = machineProdOpening?.MachineProdOpeningDetail.OrderBy(od => od.id).ToList() ?? new List<MachineProdOpeningDetail>();

            return PartialView("_MachineProdOpeningEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MachineProdOpeningEditFormDetailUpdate(MachineProdOpeningDetail item)
        {
            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);

            machineProdOpening = machineProdOpening ?? new MachineProdOpening();

            machineProdOpening.MachineProdOpeningDetail = machineProdOpening.MachineProdOpeningDetail ?? new List<MachineProdOpeningDetail>();

            try
            {
                if (Request.Params["errorMessage"] != "") throw new Exception("Por favor, corrija todos los errores.");

                var modelItem = machineProdOpening.MachineProdOpeningDetail.FirstOrDefault(it => it.id == item.id);
                if (modelItem != null)
                {
                    modelItem.id_MachineForProd = item.id_MachineForProd;
                    modelItem.MachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == item.id_MachineForProd);
                    modelItem.id_Person = item.id_Person;
                    modelItem.Person = db.Person.FirstOrDefault(fod => fod.id == item.id_Person);

                    modelItem.timeInit = TimeSpan.Parse(Request.Params["timeInitDetail"]);
                    modelItem.timeEnd = TimeSpan.Parse(Request.Params["timeEndDetail"]);
                    item.numPerson = item.numPerson;
                    this.UpdateModel(modelItem);
                    TempData["machineProdOpening"] = machineProdOpening;
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");

            var model = machineProdOpening?.MachineProdOpeningDetail.OrderBy(od => od.id).ToList() ?? new List<MachineProdOpeningDetail>();

            return PartialView("_MachineProdOpeningEditFormDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MachineProdOpeningEditFormDetailDelete(System.Int32 id)
        {
            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);

            machineProdOpening = machineProdOpening ?? new MachineProdOpening();

            machineProdOpening.MachineProdOpeningDetail = machineProdOpening.MachineProdOpeningDetail ?? new List<MachineProdOpeningDetail>();

            try
            {
                var machineProdOpeningDetail = machineProdOpening.MachineProdOpeningDetail.FirstOrDefault(p => p.id == id);
                if (machineProdOpeningDetail != null)
                    machineProdOpening.MachineProdOpeningDetail.Remove(machineProdOpeningDetail);

                TempData["machineProdOpening"] = machineProdOpening;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");

            var model = machineProdOpening?.MachineProdOpeningDetail.OrderBy(od => od.id).ToList() ?? new List<MachineProdOpeningDetail>();

            return PartialView("_MachineProdOpeningEditFormDetailPartial", model);
        }

        #endregion MachineProdOpeningDetail

        #region DETAILS VIEW

        public ActionResult MachineProdOpeningDetailPartial(int? id_machineProdOpening)
        {
            ViewData["id_machineProdOpening"] = id_machineProdOpening;
            var machineProdOpening = db.MachineProdOpening.FirstOrDefault(p => p.id == id_machineProdOpening);
            var model = machineProdOpening?.MachineProdOpeningDetail.OrderBy(od => od.id).ToList() ?? new List<MachineProdOpeningDetail>();
            TempData.Keep("machineProdOpening");
            return PartialView("_MachineProdOpeningDetailViewsPartial", model);
        }

        #endregion DETAILS VIEW

        #region SINGLE CHANGE InvoiceCommercial STATE

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
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); 

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
            MachineProdOpening machineProdOpening = db.MachineProdOpening.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                        if (entityPermissions != null)
                        {
                            foreach (var item in machineProdOpening.MachineProdOpeningDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Cerrar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Cerrar.");
                                }
                            }
                        }
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "04");

                    if (machineProdOpening != null && documentState != null)
                    {
                        if (machineProdOpening.MachineProdOpeningDetail.FirstOrDefault(fod => (db.LiquidationCartOnCart.FirstOrDefault(fod2 => fod2.id_MachineProdOpening == machineProdOpening.id &&
                                                                                                                                              fod2.id_MachineForProd == fod.id_MachineForProd &&
                                                                                                                                              fod2.Document.DocumentState.code != "05") != null)) != null)  
                        {
                            throw new Exception("tiene liquidaciones asociadas.");
                        }

                        machineProdOpening.Document.id_documentState = documentState.id;
                        machineProdOpening.Document.DocumentState = documentState;

                        db.MachineProdOpening.Attach(machineProdOpening);
                        db.Entry(machineProdOpening).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("machineProdOpening");
                    ViewData["EditMessage"] = ErrorMessage("No se puede cerrar la Apertura de Máquina porque " + e.Message);
                    return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
                }
            }

            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");
            ViewData["EditMessage"] = SuccessMessage("Apertura de Máquina: " + machineProdOpening.Document.number + " cerrada exitosamente");

            return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            MachineProdOpening machineProdOpening = db.MachineProdOpening.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                        if (entityPermissions != null)
                        {
                            foreach (var item in machineProdOpening.MachineProdOpeningDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Anular.");
                                }
                            }
                        }
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

                    if (machineProdOpening != null && documentState != null)
                    {
                        if (machineProdOpening.Document.DocumentState.code == "03")  
                        {
                            var existLiquidationCartOnCartNotCancel = db.LiquidationCartOnCart.FirstOrDefault(fod => fod.id_MachineProdOpening == id && fod.Document.DocumentState.code != "05");

                            if (existLiquidationCartOnCartNotCancel != null)
                            {
                                throw new Exception("Existe Liquidación de Carro x Carro asociado a esta Apertura de Máquina.");
                            }
                        }

                        machineProdOpening.Document.id_documentState = documentState.id;
                        machineProdOpening.Document.DocumentState = documentState;

                        db.MachineProdOpening.Attach(machineProdOpening);
                        db.Entry(machineProdOpening).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("machineProdOpening");
                    ViewData["EditMessage"] = ErrorMessage("No se puede anular la Apertura de Máquina debido a: " + e.Message);
                    return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
                }
            }

            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");
            ViewData["EditMessage"] = SuccessMessage("Apertura de Máquina: " + machineProdOpening.Document.number + " anulada exitosamente");

            return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            MachineProdOpening machineProdOpening = db.MachineProdOpening.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                        if (entityPermissions != null)
                        {
                            foreach (var item in machineProdOpening.MachineProdOpeningDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Reversar.");
                                }
                            }
                        }
                    }

                    var esTunel = machineProdOpening
                                    ?.MachineProdOpeningDetail
                                    ?.FirstOrDefault()
                                    ?.MachineForProd
                                    ?.tbsysTypeMachineForProd.code.Equals("TUN") ?? false;

                    if (esTunel)
                    {
                        var tunelesAsignado = machineProdOpening
                                    ?.MachineProdOpeningDetail
                                    ?.Where(e => e.InventoryMovePlantTransfer
                                            .Where(z => z.InventoryMove.Document.DocumentState.code.Equals("01") || z.InventoryMove.Document.DocumentState.code.Equals("03")).Any()).Any() ?? false;

                        if (tunelesAsignado)
                        {
                            throw new Exception("Existe una Transferencia de Túnel asociada a esta Apertura de Máquina.");
                        }
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");

                    if (machineProdOpening != null && documentState != null)
                    {
                        var existLiquidationCartOnCartNotCancel = db.LiquidationCartOnCart.FirstOrDefault(fod => fod.id_MachineProdOpening == id && fod.Document.DocumentState.code != "05");

                        if (existLiquidationCartOnCartNotCancel != null)
                        {
                            throw new Exception("Existe Liquidación de Carro x Carro asociado a esta Apertura de Máquina.");
                        }

                        machineProdOpening.Document.id_documentState = documentState.id;
                        machineProdOpening.Document.DocumentState = documentState;

                        db.MachineProdOpening.Attach(machineProdOpening);
                        db.Entry(machineProdOpening).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("machineProdOpening");
                    ViewData["EditMessage"] = ErrorMessage("No se puede Reversar la Apertura de Máquina debido a: " + e.Message);
                    return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
                }
            }

            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");
            ViewData["EditMessage"] = SuccessMessage("Apertura de Máquina: " + machineProdOpening.Document.number + " reversada exitosamente");

            return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
        }

        #endregion SINGLE CHANGE InvoiceCommercial STATE

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
                btnApprove = false,
                btnAutorize = false,
                btnProtect = false,
                btnCancel = false,
                btnRevert = false,
                btnSave = false,
                btnPrint = false,
            };

            if (id == 0)
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnSave = true,
                    btnPrint = false,
                };
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            MachineProdOpening machineProdOpening = db.MachineProdOpening.FirstOrDefault(r => r.id == id);
            string code_state = machineProdOpening.Document.DocumentState.code;

            if (code_state == "01")  
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = false,
                    btnSave = true,
                    btnPrint = false,
                };
            }
            else if (code_state == "03")  
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = true,
                    btnCancel = true,
                    btnRevert = true,
                    btnSave = false,
                    btnPrint = false,
                };
            }
            else if (code_state == "04" || code_state == "05")      
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnSave = false,
                    btnPrint = false,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion ACTIONS

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_machineProdOpening)
        {
            TempData.Keep("machineProdOpening");

            int index = db.MachineProdOpening.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_machineProdOpening);

            var result = new
            {
                maximunPages = db.MachineProdOpening.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            MachineProdOpening machineProdOpening = db.MachineProdOpening.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (machineProdOpening != null)
            {
                TempData["machineProdOpening"] = machineProdOpening;
                TempData.Keep("machineProdOpening");
                return PartialView("_MachineProdOpeningEditFormPartial", machineProdOpening);
            }

            TempData.Keep("machineProdOpening");

            return PartialView("_MachineProdOpeningEditFormPartial", new MachineProdOpening());
        }

        #endregion PAGINATION

        #region AXILIAR FUNCTIONS

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
                id_warehouseLocation = warehouseLocation?.id
            };

            TempData["receptionDispatchMaterials"] = receptionDispatchMaterials;

            TempData.Keep("receptionDispatchMaterials");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetWarehouseLocationReceptionDispatchMaterialsDetail(int? id_warehouseCurrent)
        {
            ReceptionDispatchMaterials receptionDispatchMaterials = (TempData["receptionDispatchMaterials"] as ReceptionDispatchMaterials);

            Warehouse warehouseAux = db.Warehouse.FirstOrDefault(i => i.id == id_warehouseCurrent);

            var warehouseLocation = warehouseAux?.WarehouseLocation.ToList() ?? new List<WarehouseLocation>();

            TempData.Keep("receptionDispatchMaterials");
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_warehouseLocationReceptionDispatchMaterialsDetail";
                p.Width = Unit.Percentage(100);

                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "ReceptionDispatchMaterials", Action = "GetWarehouseLocationReceptionDispatchMaterialsDetail"    };
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
        public JsonResult ItsRepeatedDetail(int? id_machineForProdNew)
        {
            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);
            machineProdOpening = machineProdOpening ?? new MachineProdOpening();

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };
            var machineForProdAux = db.MachineForProd.FirstOrDefault(fod => fod.id == id_machineForProdNew);

            var machineProdOpeningAux = machineProdOpening.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id_MachineForProd == id_machineForProdNew);
            if (machineProdOpeningAux != null)
            {
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir la Máquina: " + machineForProdAux.name + ",  en los detalles."
                };
            }
            else
            {
                if (!machineForProdAux.available)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = machineForProdAux.name + ",  no está disponible. " + machineForProdAux.reason
                    };
                }
            }

            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsAvailable(int? id_machineForProdNew)
        {
            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);
            machineProdOpening = machineProdOpening ?? new MachineProdOpening();

            var result = new
            {
                itsAvailable = 0,
                Error = ""
            };
            var machineForProdAux = db.MachineForProd.FirstOrDefault(fod => fod.id == id_machineForProdNew);

            if (!machineForProdAux.available)
            {
                result = new
                {
                    itsAvailable = 1,
                    Error = machineForProdAux.name + ",  no está disponible. " + machineForProdAux.reason
                };
            }

            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetTimesTurn(int? id_turn)
        {
            var turn = db.Turn.FirstOrDefault(fod => fod.id == id_turn);

            var result = new
            {
                Message = "Ok",
                timeInitTurn = turn?.timeInit,
                timeEndTurn = turn?.timeEnd
            };

            TempData.Keep("machineProdOpening");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult NameMachineProdOpeningDetailDetailCombo_SelectedIndexChanged(int? id_MachineForProd)
        {
            var machineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == id_MachineForProd);

            var result = new
            {
                Message = "Ok",
                codeMachineForProd = machineForProd?.code
            };

            TempData.Keep("machineProdOpening");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult IsValidScheduleTurn(int? id_turn, string emissionDate)
        {
            var turn = db.Turn.FirstOrDefault(fod => fod.id == id_turn);

            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);
            machineProdOpening = machineProdOpening ?? new MachineProdOpening();

            var result = new
            {
                isValid = 1,
                Error = ""
            };

            var time00Aux = new DateTime(2000, 1, 1, 0, 0, 0);
            var time11Aux = new DateTime(2000, 1, 1, 11, 59, 59);

            var timeInitAux = new DateTime(2000, 1, 1, turn.timeInit.Hours, turn.timeInit.Minutes, turn.timeInit.Seconds);
            var timeEndAux = new DateTime(2000, 1, 1, turn.timeEnd.Hours, turn.timeEnd.Minutes, turn.timeEnd.Seconds);

            var diurnoTimeInitRange = false;
            var diurnoTimeEndRange = false;

            var cumple = (DateTime.Compare(time00Aux, timeInitAux) <= 0);
            if (cumple)
            {
                cumple = (DateTime.Compare(timeInitAux, time11Aux) <= 0);
                if (cumple)
                {
                    diurnoTimeInitRange = cumple;
                }
            }

            cumple = (DateTime.Compare(time00Aux, timeEndAux) <= 0);
            if (cumple)
            {
                cumple = (DateTime.Compare(timeEndAux, time11Aux) <= 0);
                if (cumple)
                {
                    diurnoTimeEndRange = cumple;
                }
            }
            var diurnoTimeCurrent = false;

            var mayorTimeCurrentTimeInitRange = false;
            var menorTimeCurrentTimeEndRange = false;

            var emissionDateCurrent = emissionDate != "" ? DateTime.Parse(emissionDate) : (DateTime?)null;
            foreach (var item in machineProdOpening.MachineProdOpeningDetail)
            {
                var machineProdOpeningAux = db.MachineProdOpening.AsEnumerable().FirstOrDefault(fod => fod.id != machineProdOpening.id && emissionDateCurrent != null && DateTime.Compare(fod.Document.emissionDate.Date, emissionDateCurrent.Value.Date) == 0 &&
                                                                   fod.Document.DocumentState.code != "05" && fod.id_Turn == id_turn && fod.MachineProdOpeningDetail.FirstOrDefault(fod2 => fod2.id_MachineForProd == item.id_MachineForProd) != null); 

                if (machineProdOpeningAux != null)
                {
                    result = new
                    {
                        isValid = 0,
                        Error = "Existe otra apertura de máquina con la misma fecha de emisión: " +
                                 machineProdOpeningAux.Document.emissionDate.ToString("dd/MM/yyyy") + ", turno: " +
                                 machineProdOpeningAux.Turn.name + " y detalle de máquina: " + item.MachineForProd.name +
                                 ". No puede repetirse, verifique el caso e intentelo de nuevo."
                    };
                    break;
                }

                var timeCurrentAux = new DateTime(2000, 1, 1, item.timeInit.Hours, item.timeInit.Minutes, item.timeInit.Seconds);

                cumple = (DateTime.Compare(time00Aux, timeCurrentAux) <= 0);
                if (cumple)
                {
                    cumple = (DateTime.Compare(timeCurrentAux, time11Aux) <= 0);
                    if (cumple)
                    {
                        diurnoTimeCurrent = cumple;
                    }
                }

                cumple = (DateTime.Compare(timeInitAux, timeCurrentAux) <= 0);
                mayorTimeCurrentTimeInitRange = cumple;

                cumple = (DateTime.Compare(timeCurrentAux, timeEndAux) < 0);
                menorTimeCurrentTimeEndRange = cumple;

                cumple = ((diurnoTimeInitRange == diurnoTimeCurrent && mayorTimeCurrentTimeInitRange) || diurnoTimeInitRange != diurnoTimeCurrent) &&
                         ((diurnoTimeEndRange == diurnoTimeCurrent && menorTimeCurrentTimeEndRange) || diurnoTimeEndRange != diurnoTimeCurrent);
                if (!cumple)
                {
                    result = new
                    {
                        isValid = 0,
                        Error = "Hora de Inicio y Fin del Turno debe contener a las horas inicio y fin de todos los detalles."
                    };
                    break;
                }
            }

            TempData["machineProdOpening"] = machineProdOpening;
            TempData.Keep("machineProdOpening");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSupervisor(int? id_MachineForProd)
        {
            MachineProdOpening machineProdOpening = (TempData["machineProdOpening"] as MachineProdOpening);
            var aMachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == id_MachineForProd);
            var persons = new List<Person>();
            if (aMachineForProd != null)
            {
                persons = db.Person.Where(g => (g.isActive && g.id_company == this.ActiveCompanyId && g.Rol.Any(a => a.name == aMachineForProd.tbsysTypeMachineForProd.Rol.name))).ToList();
            }

            TempData.Keep("machineProdOpening");
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "personMachineProdOpening";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "MachineProdOpening", Action = "GetSupervisor" };
                p.ClientSideEvents.BeginCallback = "PersonMachineProdOpening_BeginCallback";
                p.ClientSideEvents.EndCallback = "PersonMachineProdOpening_EndCallback";
                p.ClientSideEvents.Validation = "OnPersonMachineProdOpeningDetailValidation";
                p.CallbackPageSize = 5;
                p.ValueField = "id";
                p.TextField = "fullname_businessName";
                p.ValueType = typeof(int);

                p.BindList(persons);
            });
        }

        private List<int> ListMachineByUserRol()
        {
            var idPerson = db.User.FirstOrDefault(u => u.id == this.ActiveUser.id).id_employee;
            var idsRol = db.Person.FirstOrDefault(p => p.id == idPerson).Rol.Select(r => r.id).Distinct();

            var Maquinas = db.MachineForProd.Where(z => idsRol.Contains(z.tbsysTypeMachineForProd.id_Rol));

            return Maquinas.Select(e => e.id).Distinct().ToList();
        }

        #endregion AXILIAR FUNCTIONS
    }
}