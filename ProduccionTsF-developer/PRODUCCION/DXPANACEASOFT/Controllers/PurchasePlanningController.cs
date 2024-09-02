using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Configuration;
using DXPANACEASOFT.Reports.PurchasePlanning;
using System.Globalization;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class PurchasePlanningController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IndexReportLote(string ID)
        {

            try
            {
                Session["URLOTE"] = ConfigurationManager.AppSettings["URLOTE"];
            }
            catch (Exception ex)
            {

                ViewBag.IframeUrl = ex.Message;

            }

            ViewBag.IframeUrl = Session["URLOTE"] + "?id=" + ID;



            //return Redirect("../Views/AditionalReport/WReportGuia.aspx"); //  View("WReportGuia"); //Aspx file Views/Products/WebForm1.aspx
            return PartialView();
        }

        #region Purchase Planning EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchasePlanningFormEditPartial(int id)
        {
            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

            if (purchasePlanning == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("17"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

                Employee employee = ActiveUser.Employee;

                purchasePlanning = new PurchasePlanning
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
                    id_personPlanning = employee?.id ?? 0,
                    Employee = employee,
                    PurchasePlanningDetail = new List<PurchasePlanningDetail>()
                };
                
            }


            TempData["purchasePlanning"] = purchasePlanning;
            TempData.Keep("purchasePlanning");

            return PartialView("_FormEditPurchasePlanning", purchasePlanning);
        }


        #endregion

        #region ResultGridView

        [ValidateInput(false)]
        public ActionResult PurchasePlanningResultsPartial(int? id_documentState, string number, string reference, DateTime? startEmissionDate, DateTime? endEmissionDate,//Document
                                                                 int? id_purchasePlanningPeriod, int? filterPersonPlanning,//Planning
                                                                 int[] providers, int[] buyers, int[] items, int[] itemTypeCategorys, DateTime? startDatePlanning, DateTime? endDatePlanning) //Planning Detail
        {
            List<PurchasePlanning> model = db.PurchasePlanning.ToList();

            #region Document

            if (id_documentState != 0 && id_documentState != null)
            {
                model = model.Where(o => o.Document.id_documentState == id_documentState).ToList();
            }

            if (!string.IsNullOrEmpty(number))
            {
                model = model.Where(o => o.Document.number.Contains(number)).ToList();
            }

            if (!string.IsNullOrEmpty(reference))
            {
                model = model.Where(o => o.Document.reference.Contains(reference)).ToList();
            }

            if (startEmissionDate != null && endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0 && DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            #endregion

            #region Planning

            if (id_purchasePlanningPeriod != 0 && id_purchasePlanningPeriod != null)
            {
                model = model.Where(o => o.id_purchasePlanningPeriod == id_purchasePlanningPeriod).ToList();
            }

            if (filterPersonPlanning != 0 && filterPersonPlanning != null)
            {
                model = model.Where(o => o.id_personPlanning == filterPersonPlanning).ToList();
            }

            #endregion

            #region Planning Detail

            if (providers != null && providers.Length > 0)
            {
                var tempModel = new List<PurchasePlanning>();
                foreach (var m in model)
                {
                    var details = m.PurchasePlanningDetail.Where(d => providers.Contains(d.id_provider));
                    if (details.Any())
                    {
                        tempModel.Add(m);
                    }
                }

                model = tempModel;
            }

            if (buyers != null && buyers.Length > 0)
            {
                var tempModel = new List<PurchasePlanning>();
                foreach (var m in model)
                {
                    var details = m.PurchasePlanningDetail.Where(d => buyers.Contains(d.id_buyer));
                    if (details.Any())
                    {
                        tempModel.Add(m);
                    }
                }

                model = tempModel;
            }

            if (items != null && items.Length > 0)
            {
                var tempModel = new List<PurchasePlanning>();
                foreach (var m in model)
                {
                    var details = m.PurchasePlanningDetail.Where(d => items.Contains(d.id_item ?? 0));
                    if (details.Any())
                    {
                        tempModel.Add(m);
                    }
                }

                model = tempModel;
            }

            if (itemTypeCategorys != null && itemTypeCategorys.Length > 0)
            {
                var tempModel = new List<PurchasePlanning>();
                foreach (var m in model)
                {
                    var details = m.PurchasePlanningDetail.Where(d => itemTypeCategorys.Contains(d.id_itemTypeCategory));
                    if (details.Any())
                    {
                        tempModel.Add(m);
                    }
                }

                model = tempModel;
            }

            if (startDatePlanning != null && endDatePlanning != null)
            {
                var tempModel = new List<PurchasePlanning>();
                foreach (var m in model)
                {
                    var details = m.PurchasePlanningDetail.Where(d => DateTime.Compare(startDatePlanning.Value.Date, d.datePlanning.Date) <= 0 && DateTime.Compare(d.datePlanning.Date, endDatePlanning.Value.Date) <= 0);
                    if (details.Any())
                    {
                        tempModel.Add(m);
                    }
                }

                model = tempModel;
            }

            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_PurchasePlanningResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        //[ValidateInput(false)]
        //public ActionResult ProductionLotReceptionResultsOrdersPartial()
        //{
        //    var model = db.PurchaseOrder.OrderByDescending(o => o.id); ;

        //    return PartialView("_ProductionLotReceptionDetailPartial");
        //}

        #endregion

        #region Purchase Plannings

        [HttpPost]
        public ActionResult PurchasePlanningsPartial()
        {
            var model = (TempData["model"] as List<PurchasePlanning>);
            model = model ?? new List<PurchasePlanning>();

            TempData.Keep("model");
            return PartialView("_PurchasePlanningsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchasePlanningsAddNew(PurchasePlanning item, Document itemDoc)
        {
            PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);
            purchasePlanning = purchasePlanning ?? new PurchasePlanning();

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

                        DocumentType documentType = db.DocumentType.FirstOrDefault(dt=> dt.code == "17");
                        if (documentType == null)
                        {
                            TempData.Keep("purchasePlanning");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la planificación porque no existe el Tipo de Documento: Planificación de Compra con Código: 17, configúrelo e inténtelo de nuevo");
                            return PartialView("_PurchasePlanningEditFormPartial", item);

                        }
                        item.Document.id_documentType = documentType.id;
                        item.Document.DocumentType = documentType;
                        item.Document.sequential = GetDocumentSequential(item.Document.id_documentType);
                        item.Document.number = GetDocumentNumber(item.Document.id_documentType);


                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        if (documentState == null)
                        {
                            TempData.Keep("purchasePlanning");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la planificación porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                            return PartialView("_PurchasePlanningEditFormPartial", item);

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

                        #endregion

                        #region PurchasePlanning

                        item.id_company = this.ActiveCompanyId;

                        #endregion

                        #region PurchasePlanningDetails

                        if (purchasePlanning.PurchasePlanningDetail != null)
                        {
                            item.PurchasePlanningDetail = new List<PurchasePlanningDetail>();
                            foreach (var detail in purchasePlanning.PurchasePlanningDetail)
                            {
                                var purchasePlanningDetail = new PurchasePlanningDetail();
                                purchasePlanningDetail.id_provider = detail.id_provider;
                                purchasePlanningDetail.Provider = db.Provider.FirstOrDefault(i => i.id == purchasePlanningDetail.id_provider);
                                purchasePlanningDetail.id_buyer = detail.id_buyer;
                                purchasePlanningDetail.Person = db.Person.FirstOrDefault(i => i.id == purchasePlanningDetail.id_buyer);
                                purchasePlanningDetail.id_item = detail.id_item;
                                purchasePlanningDetail.Item = db.Item.FirstOrDefault(i => i.id == purchasePlanningDetail.id_item);
                                purchasePlanningDetail.id_itemTypeCategory = detail.id_itemTypeCategory;
                                purchasePlanningDetail.ItemTypeCategory = db.ItemTypeCategory.FirstOrDefault(i => i.id == purchasePlanningDetail.id_itemTypeCategory);
                                purchasePlanningDetail.quantity = detail.quantity;
                                purchasePlanningDetail.datePlanning = detail.datePlanning;

                                item.PurchasePlanningDetail.Add(purchasePlanningDetail);
                            }
                        }

                        if (item.PurchasePlanningDetail.Count == 0)
                        {
                            TempData.Keep("purchasePlanning");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una planificación sin detalles");
                            return PartialView("_PurchasePlanningEditFormPartial", item);
                        }

                        #endregion

                        db.PurchasePlanning.Add(item);
                        db.SaveChanges();   
                        trans.Commit();

                        TempData["purchasePlanning"] = item;
                        TempData.Keep("purchasePlanning");

                        ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + item.Document.number + " guardado exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("purchasePlanning");
                        ViewData["EditMessage"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            //TempData["productionLot"] = item;
            //TempData.Keep("productionLot");

            return PartialView("_PurchasePlanningEditFormPartial", item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchasePlanningsUpdate(PurchasePlanning item, Document itemDoc)
        {
            PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);
            purchasePlanning = purchasePlanning ?? new PurchasePlanning();

            var modelItem = db.PurchasePlanning.FirstOrDefault(p => p.id == item.id);

            if (ModelState.IsValid && modelItem != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region DOCUMENT

                        modelItem.Document.emissionDate = itemDoc.emissionDate;
                        modelItem.Document.description = itemDoc.description;
                        modelItem.Document.id_userUpdate = ActiveUser.id;
                        modelItem.Document.dateUpdate = DateTime.Now;

                        #endregion

                        #region PurchasePlanning

                        #endregion

                        #region PurchasePlanningDetail

                        for (int i = modelItem.PurchasePlanningDetail.Count - 1; i >= 0; i--)
                        {
                            var detail = modelItem.PurchasePlanningDetail.ElementAt(i);

                            modelItem.PurchasePlanningDetail.Remove(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }

                        if (purchasePlanning.PurchasePlanningDetail != null)
                        {
                            foreach (var detail in purchasePlanning.PurchasePlanningDetail)
                            {
                                var newDetail = new PurchasePlanningDetail
                                {
                                    id_purchasePlanning = modelItem.id,
                                    PurchasePlanning = modelItem,
                                    id_provider = detail.id_provider,
                                    Provider = db.Provider.FirstOrDefault(p=> p.id == detail.id_provider),
                                    id_buyer = detail.id_buyer,
                                    Person = db.Person.FirstOrDefault(p => p.id == detail.id_buyer),
                                    id_item = detail.id_item,
                                    Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),
                                    id_itemTypeCategory = detail.id_itemTypeCategory,
                                    ItemTypeCategory = db.ItemTypeCategory.FirstOrDefault(i => i.id == detail.id_itemTypeCategory),
                                    quantity = detail.quantity,
                                    datePlanning = detail.datePlanning
                                };

                                modelItem.PurchasePlanningDetail.Add(newDetail);
                            }
                        }

                        if (modelItem.PurchasePlanningDetail.Count == 0)
                        {
                            TempData.Keep("purchasePlanning");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una planificación de compra sin detalle");
                            return PartialView("_PurchasePlanningEditFormPartial", modelItem);
                        }

                        #endregion

                        db.PurchasePlanning.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["purchasePlanning"] = modelItem;
                        TempData.Keep("purchasePlanning");

                        ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + modelItem.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("purchasePlanning");
                        ViewData["EditMessage"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor , corrija todos los errores.";

            
            //TempData.Keep("productionLot");

            return PartialView("_PurchasePlanningEditFormPartial", modelItem);
        }

        #endregion

        #region PurchasePlanningDetail

        [ValidateInput(false)]
        public ActionResult PurchasePlanningEditFormItemsDetailPartial()
        {
            PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);

            //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
            purchasePlanning = purchasePlanning ?? new PurchasePlanning();

            var model = purchasePlanning?.PurchasePlanningDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<PurchasePlanningDetail>();

            TempData["purchasePlanning"] = TempData["purchasePlanning"] ?? purchasePlanning;
            TempData.Keep("purchasePlanning");

            return PartialView("_PurchasePlanningEditFormItemsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchasePlanningEditFormItemsDetailAddNew(PurchasePlanningDetail item)
        {

            PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);
            //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
            purchasePlanning = purchasePlanning ?? new PurchasePlanning();
            purchasePlanning.PurchasePlanningDetail = purchasePlanning.PurchasePlanningDetail ?? new List<PurchasePlanningDetail>();

            if (ModelState.IsValid)
            {
                item.id = purchasePlanning.PurchasePlanningDetail.Count() > 0 ? purchasePlanning.PurchasePlanningDetail.Max(ppd => ppd.id) + 1 : 1;
                purchasePlanning.PurchasePlanningDetail.Add(item);
                //UpdateProductionLotTotals(purchasePlanning);
            }

            TempData["purchasePlanning"] = purchasePlanning;
            TempData.Keep("purchasePlanning");

            var model = purchasePlanning?.PurchasePlanningDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<PurchasePlanningDetail>();

            return PartialView("_PurchasePlanningEditFormItemsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchasePlanningEditFormItemsDetailUpdate(PurchasePlanningDetail item)
        {
            PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);
            //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
            purchasePlanning = purchasePlanning ?? new PurchasePlanning();
            purchasePlanning.PurchasePlanningDetail = purchasePlanning.PurchasePlanningDetail ?? new List<PurchasePlanningDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = purchasePlanning.PurchasePlanningDetail.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        //UpdateProductionLotTotals(purchasePlanning);
                        TempData["purchasePlanning"] = purchasePlanning;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("purchasePlanning");

            var model = purchasePlanning?.PurchasePlanningDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<PurchasePlanningDetail>();

            return PartialView("_PurchasePlanningEditFormItemsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchasePlanningEditFormItemsDetailDelete(System.Int32 id)
        {
            PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);
            //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
            purchasePlanning = purchasePlanning ?? new PurchasePlanning();
            purchasePlanning.PurchasePlanningDetail = purchasePlanning.PurchasePlanningDetail ?? new List<PurchasePlanningDetail>();

            //if (id_item >= 0)
            //{
            try
            {
                    var purchasePlanningDetails = purchasePlanning.PurchasePlanningDetail.FirstOrDefault(p => p.id == id);
                    if (purchasePlanningDetails != null)
                        purchasePlanning.PurchasePlanningDetail.Remove(purchasePlanningDetails);

                    //UpdateProductionLotTotals(purchasePlanning);
                    TempData["purchasePlanning"] = purchasePlanning;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            //}

            TempData.Keep("purchasePlanning");

            var model = purchasePlanning?.PurchasePlanningDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<PurchasePlanningDetail>();
            return PartialView("_PurchasePlanningEditFormItemsDetailPartial", model);
        }

        //private void UpdateProductionLotTotals(ProductionLot productionLot)
        //{
        //    productionLot.totalQuantityOrdered = 0.0M;
        //    productionLot.totalQuantityRemitted = 0.0M;
        //    productionLot.totalQuantityRecived = 0.0M;

        //    foreach (var productionLotDetail in productionLot.ProductionLotDetail)
        //    {
        //        productionLot.totalQuantityOrdered += productionLotDetail.quantityOrdered;
        //        productionLot.totalQuantityRemitted += productionLotDetail.quantityRemitted;
        //        productionLot.totalQuantityRecived += productionLotDetail.quantityRecived;
        //    }
        //}

        #endregion

        #region DETAILS VIEW

        public ActionResult PurchasePlanningDetailItemsPartial(int? id_purchasePlanning)
        {
            ViewData["id_purchasePlanning"] = id_purchasePlanning;
            var purchasePlanning = db.PurchasePlanning.FirstOrDefault(p => p.id == id_purchasePlanning);
            var model = purchasePlanning?.PurchasePlanningDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<PurchasePlanningDetail>();
            return PartialView("_PurchasePlanningDetailItemsPartial", model.ToList());
        }

        #endregion

        #region SINGLE CHANGE PurchasePlanning STATE

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
            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06");

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
            ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + purchasePlanning.Document.number + " autorizada exitosamente");

            return PartialView("_PurchasePlanningEditFormPartial", purchasePlanning);
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
            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

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
            ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + purchasePlanning.Document.number + " anulada exitosamente");

            return PartialView("_PurchasePlanningEditFormPartial", purchasePlanning);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
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
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["purchasePlanning"] = purchasePlanning;
            TempData.Keep("purchasePlanning");
            ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + purchasePlanning.Document.number + " reversada exitosamente");

            return PartialView("_PurchasePlanningEditFormPartial", purchasePlanning);
        }

        #endregion

        #region SELECTED PurchasePlanning STATE CHANGE

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

        #endregion

        #region PurchasePlanning REPORTS

        [HttpPost]
        public ActionResult PurchasePlanningReport(int id)
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

        #endregion

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
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);
            string code_state = purchasePlanning.Document.DocumentState.code;

            if (code_state == "01") // PENDIENTE
            {
                actions = new
                {
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
                    btnApprove = false,
                    btnAutorize = true,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                };
            }
            else if (code_state == "05") // ANULADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                };
            }
            else if (code_state == "06") // AUTORIZADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = true,
                    btnCancel = true,
                    btnRevert = true,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_purchasePlanning)
        {
            TempData.Keep("purchasePlanning");

            int index = db.PurchasePlanning.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_purchasePlanning);

            var result = new
            {
                maximunPages = db.PurchasePlanning.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            PurchasePlanning purchasePlanning = db.PurchasePlanning.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (purchasePlanning != null)
            {
                TempData["purchasePlanning"] = purchasePlanning;
                TempData.Keep("purchasePlanning");
                return PartialView("_PurchasePlanningEditFormPartial", purchasePlanning);
            }

            TempData.Keep("purchasePlanning");

            return PartialView("_PurchasePlanningEditFormPartial", new PurchasePlanning());
        }

        #endregion

        #region AXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ItemDetailData(int id_item)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                metricUnit = item.ItemPurchaseInformation?.MetricUnit.code ?? "",
                id_itemTypeCategory = item.ItemTypeCategory?.id ?? 0
            };
            
            TempData.Keep("purchasePlanning");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class tempDatePlanningId_purchasePlanningPeriod
        {
            public string datePlanning { get; set; }
            public int id_purchasePlanningPeriod { get; set; }

        }

        [HttpPost]
        public JsonResult ValidateDatePlanning(tempDatePlanningId_purchasePlanningPeriod item)
        {
            var result = new
            {
                code = 0,
                message = "Ok"
            };
            try
            {
                PurchasePlanningPeriod purchasePlanningPeriod = db.PurchasePlanningPeriod.FirstOrDefault(i => i.id == item.id_purchasePlanningPeriod);

                if (purchasePlanningPeriod == null)
                {
                    result = new
                    {
                        code = -1,
                        message = "Debe Asignar un período de Planificación antes de adicionar Detalles."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                if (item.datePlanning != null)
                {
                    DateTime datePlanning = DateTime.Parse(item.datePlanning, new CultureInfo("es-EC"));
                    if (DateTime.Compare(purchasePlanningPeriod.dateStar.Date, datePlanning.Date) <= 0 && DateTime.Compare(datePlanning.Date, purchasePlanningPeriod.dateEnd.Date) <= 0)
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }else
                    {
                        result = new
                        {
                            code = -1,
                            message = "Fuera del rango del Período de Planificación: " + purchasePlanningPeriod.name
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    
                }

                TempData.Keep("purchasePlanning");

                
            }
            catch (Exception e)
            {
                TempData.Keep("purchasePlanning");
                ViewData["EditMessage"] = e.Message;
                result = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidatePurchasePlanningPeriod(int id_purchasePlanningPeriod)
        {
            var result = new
            {
                code = 0,
                message = "Ok"
            };
            try
            {
                PurchasePlanningPeriod purchasePlanningPeriod = db.PurchasePlanningPeriod.FirstOrDefault(i => i.id == id_purchasePlanningPeriod);

                if (purchasePlanningPeriod == null)
                {
                    result = new
                    {
                        code = -1,
                        message = "Debe Asignar un período de Planificación antes de adicionar Detalles."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);
                purchasePlanning = purchasePlanning ?? new PurchasePlanning();
                purchasePlanning.PurchasePlanningDetail = purchasePlanning.PurchasePlanningDetail ?? new List<PurchasePlanningDetail>();

                foreach (var detail in purchasePlanning.PurchasePlanningDetail)
                {
                    if (DateTime.Compare(purchasePlanningPeriod.dateStar.Date, detail.datePlanning.Date) > 0 || DateTime.Compare(detail.datePlanning.Date, purchasePlanningPeriod.dateEnd.Date) > 0)
                    {
                        result = new
                        {
                            code = -1,
                            message = "Existen Fecha Planificadas en los detalle fuera de rango del Período"
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                TempData.Keep("purchasePlanning");
            }
            catch (Exception e)
            {
                TempData.Keep("purchasePlanning");
                ViewData["EditMessage"] = e.Message;
                result = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PurchasePlanningItemDetails()
        {
            PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);
            purchasePlanning = purchasePlanning ?? new PurchasePlanning();
            purchasePlanning.PurchasePlanningDetail = purchasePlanning.PurchasePlanningDetail ?? new List<PurchasePlanningDetail>();
            TempData.Keep("purchasePlanning");

            return Json(purchasePlanning.PurchasePlanningDetail.Select(d => d.id).ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}