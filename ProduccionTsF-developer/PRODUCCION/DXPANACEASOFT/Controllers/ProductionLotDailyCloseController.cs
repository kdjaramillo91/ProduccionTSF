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
    public class ProductionLotDailyCloseController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region Production Lot Daily Close EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotDailyCloseFormEditPartial(int id)
        {
            ProductionLotDailyClose productionLotDailyClose = db.ProductionLotDailyClose.FirstOrDefault(r => r.id == id);

            if (productionLotDailyClose == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("22"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");//Estado Pendiente

                Employee employee = ActiveUser.Employee;

                productionLotDailyClose = new ProductionLotDailyClose
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
                    id_personClosing = employee?.id ?? 0,
                    Employee = employee,
                    ProductionLotDailyCloseDetail = new List<ProductionLotDailyCloseDetail>()
                };
                
            }

            if (productionLotDailyClose.Document.DocumentState.code.Equals("01"))
            {
                var productionLotDailyCloseDetail = productionLotDailyClose.ProductionLotDailyCloseDetail.ToList();
                var productionLotDailyCloseDetailAux = db.ProductionLot.Where(w=> (w.ProductionLotState.code.Equals("06") || w.ProductionLotState.code.Equals("07") || w.ProductionLotState.code.Equals("08")) &&
                                                                                   !w.ProductionLotDailyCloseDetail.Any(a=> a.ProductionLotDailyClose.Document.DocumentState.code != "05")).ToList();
                var idAux = productionLotDailyClose.ProductionLotDailyCloseDetail.Count() > 0 ? productionLotDailyClose.ProductionLotDailyCloseDetail.Max(ppd => ppd.id) : 0;
                foreach (var pldcda in productionLotDailyCloseDetailAux)
                {
                    // var details = m.PurchasePlanningDetail.Where(d => providers.Contains(d.id_provider));
                    productionLotDailyCloseDetail.Add(new ProductionLotDailyCloseDetail
                    {
                        id = ++idAux,
                        id_productionLot = pldcda.id,
                        ProductionLot = pldcda,
                        id_productionLotDailyClose = productionLotDailyClose.id,
                        ProductionLotDailyClose = productionLotDailyClose
                    });
                }

                productionLotDailyClose.ProductionLotDailyCloseDetail = productionLotDailyCloseDetail;
            }

            TempData["productionLotDailyClose"] = productionLotDailyClose;
            TempData.Keep("productionLotDailyClose");

            return PartialView("_FormEditProductionLotDailyClose", productionLotDailyClose);
        }

        #endregion

        #region ResultGridView

        [ValidateInput(false)]
        public ActionResult ProductionLotDailyCloseResultsPartial(int? id_documentState, string number, string reference, DateTime? startEmissionDate, DateTime? endEmissionDate,//Document
                                                                 int? id_personClosing//,//ProductionLotDailyClose
                                                                                      /*int[] providers, int[] buyers, int[] items, int[] itemTypeCategorys, DateTime? startDatePlanning, DateTime? endDatePlanning*/) //ProductionLotDailyClose Detail
        {
            List<ProductionLotDailyClose> model = db.ProductionLotDailyClose.ToList();

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

            #region ProductionLotDailyClose

            if (id_personClosing != 0 && id_personClosing != null)
            {
                model = model.Where(o => o.id_personClosing == id_personClosing).ToList();
            }

            #endregion

            #region ProductionLotDailyClose Detail

            //if (providers != null && providers.Length > 0)
            //{
            //    var tempModel = new List<PurchasePlanning>();
            //    foreach (var m in model)
            //    {
            //        var details = m.PurchasePlanningDetail.Where(d => providers.Contains(d.id_provider));
            //        if (details.Any())
            //        {
            //            tempModel.Add(m);
            //        }
            //    }

            //    model = tempModel;
            //}


            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_ProductionLotDailyCloseResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        //[ValidateInput(false)]
        //public ActionResult ProductionLotReceptionResultsOrdersPartial()
        //{
        //    var model = db.PurchaseOrder.OrderByDescending(o => o.id); ;

        //    return PartialView("_ProductionLotReceptionDetailPartial");
        //}

        #endregion

        #region Production Lot Daily Closes

        [HttpPost]
        public ActionResult ProductionLotDailyClosesPartial()
        {
            var model = (TempData["model"] as List<ProductionLotDailyClose>);
            model = model ?? new List<ProductionLotDailyClose>();

            TempData.Keep("model");
            return PartialView("_ProductionLotDailyClosesPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotDailyClosesAddNew(bool approve, ProductionLotDailyClose item, Document itemDoc)
        {
            ProductionLotDailyClose productionLotDailyClose = (TempData["productionLotDailyClose"] as ProductionLotDailyClose);
            productionLotDailyClose = productionLotDailyClose ?? new ProductionLotDailyClose();

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

                        DocumentType documentType = db.DocumentType.FirstOrDefault(dt=> dt.code == "22");
                        if (documentType == null)
                        {
                            TempData.Keep("productionLotDailyClose");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar el cierre porque no existe el Tipo de Documento: Cierre Diario/Turno con Código: 22, configúrelo e inténtelo de nuevo");
                            return PartialView("_ProductionLotDailyCloseEditFormPartial", item);

                        }
                        item.Document.id_documentType = documentType.id;
                        item.Document.DocumentType = documentType;
                        item.Document.sequential = GetDocumentSequential(item.Document.id_documentType);
                        item.Document.number = GetDocumentNumber(item.Document.id_documentType);


                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        if (documentState == null)
                        {
                            TempData.Keep("productionLotDailyClose");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar el cierre porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                            return PartialView("_ProductionLotDailyCloseEditFormPartial", item);

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

                        #region ProductionLotDailyClose

                        item.id_company = this.ActiveCompanyId;

                        #endregion

                        #region ProductionLotDailyCloseDetails

                        if (productionLotDailyClose.ProductionLotDailyCloseDetail != null)
                        {
                            item.ProductionLotDailyCloseDetail = new List<ProductionLotDailyCloseDetail>();
                            foreach (var detail in productionLotDailyClose.ProductionLotDailyCloseDetail)
                            {
                                var productionLotDailyCloseDetail = new ProductionLotDailyCloseDetail();
                                productionLotDailyCloseDetail.id_productionLot = detail.id_productionLot;
                                productionLotDailyCloseDetail.ProductionLot = db.ProductionLot.FirstOrDefault(i => i.id == detail.id_productionLot);

                                item.ProductionLotDailyCloseDetail.Add(productionLotDailyCloseDetail);
                            }
                        }

                        if (item.ProductionLotDailyCloseDetail.Count == 0)
                        {
                            TempData.Keep("productionLotDailyClose");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar un cierre sin detalles");
                            return PartialView("_ProductionLotDailyCloseEditFormPartial", item);
                        }

                        #endregion

                        if (approve)
                        {
                            item.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        }

                        db.ProductionLotDailyClose.Add(item);
                        db.SaveChanges();   
                        trans.Commit();

                        TempData["productionLotDailyClose"] = item;
                        TempData.Keep("productionLotDailyClose");

                        ViewData["EditMessage"] = SuccessMessage("Cierre Diario/Turno: " + item.Document.number + " guardado exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("productionLotDailyClose");
                        ViewData["EditMessage"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            //TempData["productionLot"] = item;
            //TempData.Keep("productionLot");

            return PartialView("_ProductionLotDailyCloseEditFormPartial", item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotDailyClosesUpdate(bool approve, ProductionLotDailyClose item, Document itemDoc)
        {
            ProductionLotDailyClose productionLotDailyClose = (TempData["productionLotDailyClose"] as ProductionLotDailyClose);
            productionLotDailyClose = productionLotDailyClose ?? new ProductionLotDailyClose();

            var modelItem = db.ProductionLotDailyClose.FirstOrDefault(p => p.id == item.id);

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

                        #region ProductionLotDailyClose

                        #endregion

                        #region ProductionLotDailyCloseDetail

                        for (int i = modelItem.ProductionLotDailyCloseDetail.Count - 1; i >= 0; i--)
                        {
                            var detail = modelItem.ProductionLotDailyCloseDetail.ElementAt(i);

                            modelItem.ProductionLotDailyCloseDetail.Remove(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }

                        if (productionLotDailyClose.ProductionLotDailyCloseDetail != null)
                        {
                            foreach (var detail in productionLotDailyClose.ProductionLotDailyCloseDetail)
                            {
                                var newDetail = new ProductionLotDailyCloseDetail
                                {
                                    id_productionLotDailyClose = modelItem.id,
                                    ProductionLotDailyClose = modelItem,
                                    id_productionLot = detail.id_productionLot,
                                    ProductionLot = db.ProductionLot.FirstOrDefault(p=> p.id == detail.id_productionLot)
                                };

                                modelItem.ProductionLotDailyCloseDetail.Add(newDetail);
                            }
                        }

                        if (modelItem.ProductionLotDailyCloseDetail.Count == 0)
                        {
                            TempData.Keep("productionLotDailyClose");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar un Cierre sin detalle");
                            return PartialView("_ProductionLotDailyCloseEditFormPartial", modelItem);
                        }

                        #endregion

                        if (approve)
                        {
                            modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        }

                        db.ProductionLotDailyClose.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["productionLotDailyClose"] = modelItem;
                        TempData.Keep("productionLotDailyClose");

                        ViewData["EditMessage"] = SuccessMessage("Cierre Diario/Turno: " + modelItem.Document.number + " guardado exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("productionLotDailyClose");
                        ViewData["EditMessage"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor , corrija todos los errores.";

            
            //TempData.Keep("productionLot");

            return PartialView("_ProductionLotDailyCloseEditFormPartial", modelItem);
        }

        #endregion

        #region ProductionLotDailyCloseDetail

        [ValidateInput(false)]
        public ActionResult ProductionLotDailyCloseEditFormProductionLotsDetailPartial()
        {
            ProductionLotDailyClose productionLotDailyClose = (TempData["productionLotDailyClose"] as ProductionLotDailyClose);

            productionLotDailyClose = productionLotDailyClose ?? new ProductionLotDailyClose();

            var model = productionLotDailyClose?.ProductionLotDailyCloseDetail.OrderBy(od => od.id_productionLot).ToList() ?? new List<ProductionLotDailyCloseDetail>();

            TempData["productionLotDailyClose"] = TempData["productionLotDailyClose"] ?? productionLotDailyClose;
            TempData.Keep("productionLotDailyClose");

            return PartialView("_ProductionLotDailyCloseEditFormProductionLotsDetailPartial", model);
        }

        //[HttpPost, ValidateInput(false)]
        //public ActionResult PurchasePlanningEditFormItemsDetailAddNew(PurchasePlanningDetail item)
        //{

        //    PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);
        //    //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
        //    purchasePlanning = purchasePlanning ?? new PurchasePlanning();
        //    purchasePlanning.PurchasePlanningDetail = purchasePlanning.PurchasePlanningDetail ?? new List<PurchasePlanningDetail>();

        //    if (ModelState.IsValid)
        //    {
        //        item.id = purchasePlanning.PurchasePlanningDetail.Count() > 0 ? purchasePlanning.PurchasePlanningDetail.Max(ppd => ppd.id) + 1 : 1;
        //        purchasePlanning.PurchasePlanningDetail.Add(item);
        //        //UpdateProductionLotTotals(purchasePlanning);
        //    }

        //    TempData["purchasePlanning"] = purchasePlanning;
        //    TempData.Keep("purchasePlanning");

        //    var model = purchasePlanning?.PurchasePlanningDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<PurchasePlanningDetail>();

        //    return PartialView("_PurchasePlanningEditFormItemsDetailPartial", model);
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult PurchasePlanningEditFormItemsDetailUpdate(PurchasePlanningDetail item)
        //{
        //    PurchasePlanning purchasePlanning = (TempData["purchasePlanning"] as PurchasePlanning);
        //    //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
        //    purchasePlanning = purchasePlanning ?? new PurchasePlanning();
        //    purchasePlanning.PurchasePlanningDetail = purchasePlanning.PurchasePlanningDetail ?? new List<PurchasePlanningDetail>();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var modelItem = purchasePlanning.PurchasePlanningDetail.FirstOrDefault(it => it.id == item.id);
        //            if (modelItem != null)
        //            {
        //                this.UpdateModel(modelItem);
        //                //UpdateProductionLotTotals(purchasePlanning);
        //                TempData["purchasePlanning"] = purchasePlanning;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //        ViewData["EditError"] = "Por favor, corrija todos los errores.";

        //    TempData.Keep("purchasePlanning");

        //    var model = purchasePlanning?.PurchasePlanningDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<PurchasePlanningDetail>();

        //    return PartialView("_PurchasePlanningEditFormItemsDetailPartial", model);
        //}

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotDailyCloseEditFormProductionLotsDetailDelete(System.Int32 id)
        {
            ProductionLotDailyClose productionLotDailyClose = (TempData["productionLotDailyClose"] as ProductionLotDailyClose);
            productionLotDailyClose = productionLotDailyClose ?? new ProductionLotDailyClose();
            productionLotDailyClose.ProductionLotDailyCloseDetail = productionLotDailyClose.ProductionLotDailyCloseDetail ?? new List<ProductionLotDailyCloseDetail>();

            //if (id_item >= 0)
            //{
            try
            {
                    var productionLotDailyCloseDetails = productionLotDailyClose.ProductionLotDailyCloseDetail.FirstOrDefault(p => p.id == id);
                    if (productionLotDailyCloseDetails != null)
                    productionLotDailyClose.ProductionLotDailyCloseDetail.Remove(productionLotDailyCloseDetails);

                    //UpdateProductionLotTotals(purchasePlanning);
                    TempData["productionLotDailyClose"] = productionLotDailyClose;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            //}

            TempData.Keep("productionLotDailyClose");

            var model = productionLotDailyClose?.ProductionLotDailyCloseDetail.OrderBy(od => od.id_productionLot).ToList() ?? new List<ProductionLotDailyCloseDetail>();
            return PartialView("_ProductionLotDailyCloseEditFormProductionLotsDetailPartial", model);
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

        public ActionResult ProductionLotDailyCloseDetailProductionLotsPartial(int? id_productionLotDailyClose)
        {
            ViewData["id_productionLotDailyClose"] = id_productionLotDailyClose;
            var productionLotDailyClose = db.ProductionLotDailyClose.FirstOrDefault(p => p.id == id_productionLotDailyClose);
            var model = productionLotDailyClose?.ProductionLotDailyCloseDetail.OrderBy(od => od.id_productionLot).ToList() ?? new List<ProductionLotDailyCloseDetail>();
            return PartialView("_ProductionLotDailyCloseDetailProductionLotsPartial", model.ToList());
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
            ProductionLotDailyClose productionLotDailyClose = db.ProductionLotDailyClose.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");//ANULADA

                    if (productionLotDailyClose != null && documentState != null)
                    {
                        var existOtherAfter = db.ProductionLotDailyClose.Any(a => a.id > productionLotDailyClose.id && !a.Document.DocumentState.code.Equals("05"));

                        if (existOtherAfter)
                        {
                            TempData.Keep("productionLotDailyClose");
                            ViewData["EditMessage"] = ErrorMessage("No se puede anular el cierre pues existen posterior al mismo en el sistema");
                            return PartialView("_ProductionLotDailyCloseEditFormPartial", productionLotDailyClose);
                        }

                        productionLotDailyClose.Document.id_documentState = documentState.id;
                        productionLotDailyClose.Document.DocumentState = documentState;

                        db.ProductionLotDailyClose.Attach(productionLotDailyClose);
                        db.Entry(productionLotDailyClose).State = EntityState.Modified;

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

            TempData["productionLotDailyClose"] = productionLotDailyClose;
            TempData.Keep("productionLotDailyClose");
            ViewData["EditMessage"] = SuccessMessage("Cierre Diario/Turno: " + productionLotDailyClose.Document.number + " anulado exitosamente");

            return PartialView("_ProductionLotDailyCloseEditFormPartial", productionLotDailyClose);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            ProductionLotDailyClose productionLotDailyClose = db.ProductionLotDailyClose.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE

                    if (productionLotDailyClose != null && documentState != null)
                    {
                        var existOtherAfter = db.ProductionLotDailyClose.Any(a => a.id > productionLotDailyClose.id && !a.Document.DocumentState.code.Equals("05"));

                        if (existOtherAfter)
                        {
                            TempData.Keep("productionLotDailyClose");
                            ViewData["EditMessage"] = ErrorMessage("No se puede reversar el cierre pues existen posterior al mismo en el sistema");
                            return PartialView("_ProductionLotDailyCloseEditFormPartial", productionLotDailyClose);
                        }

                        productionLotDailyClose.Document.id_documentState = documentState.id;
                        productionLotDailyClose.Document.DocumentState = documentState;

                        db.ProductionLotDailyClose.Attach(productionLotDailyClose);
                        db.Entry(productionLotDailyClose).State = EntityState.Modified;

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

            TempData["productionLotDailyClose"] = productionLotDailyClose;
            TempData.Keep("productionLotDailyClose");
            ViewData["EditMessage"] = SuccessMessage("Cierre Diario/Turno: " + productionLotDailyClose.Document.number + " anulado exitosamente");

            return PartialView("_ProductionLotDailyCloseEditFormPartial", productionLotDailyClose);
        }

        #endregion

        #region SELECTED ProductionLotDailyClose STATE CHANGE

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

        #region ProductionLotDailyClose REPORTS

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

            ProductionLotDailyClose productionLotDailyClose = db.ProductionLotDailyClose.FirstOrDefault(r => r.id == id);
            string code_state = productionLotDailyClose.Document.DocumentState.code;

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
                    btnAutorize = false,
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

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_productionLotDailyClose)
        {
            TempData.Keep("productionLotDailyClose");

            int index = db.ProductionLotDailyClose.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_productionLotDailyClose);

            var result = new
            {
                maximunPages = db.ProductionLotDailyClose.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            ProductionLotDailyClose productionLotDailyClose = db.ProductionLotDailyClose.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (productionLotDailyClose != null)
            {
                TempData["productionLotDailyClose"] = productionLotDailyClose;
                TempData.Keep("productionLotDailyClose");
                return PartialView("_ProductionLotDailyCloseEditFormPartial", productionLotDailyClose);
            }

            TempData.Keep("productionLotDailyClose");

            return PartialView("_ProductionLotDailyCloseEditFormPartial", new ProductionLotDailyClose());
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

        [HttpPost]
        public JsonResult UpdateDetail()
        {
            ProductionLotDailyClose productionLotDailyClose = (TempData["productionLotDailyClose"] as ProductionLotDailyClose);
            productionLotDailyClose = productionLotDailyClose ?? new ProductionLotDailyClose();
            productionLotDailyClose.ProductionLotDailyCloseDetail = productionLotDailyClose.ProductionLotDailyCloseDetail ?? new List<ProductionLotDailyCloseDetail>();

            
            var productionLotDailyCloseDetail = productionLotDailyClose.ProductionLotDailyCloseDetail.ToList();
            var productionLotDailyCloseDetailAux = db.ProductionLot.Where(w => (w.ProductionLotState.code.Equals("06") || w.ProductionLotState.code.Equals("07") || w.ProductionLotState.code.Equals("08")) &&
                                                                                !w.ProductionLotDailyCloseDetail.Any(a => a.ProductionLotDailyClose.Document.DocumentState.code != "05")).ToList();
            var idAux = productionLotDailyClose.ProductionLotDailyCloseDetail.Count() > 0 ? productionLotDailyClose.ProductionLotDailyCloseDetail.Max(ppd => ppd.id) : 0;
            foreach (var pldcda in productionLotDailyCloseDetailAux)
            {
                var detailsAux = productionLotDailyCloseDetail.FirstOrDefault(fod => fod.id_productionLot == pldcda.id);
                if(detailsAux == null)
                {
                    productionLotDailyCloseDetail.Add(new ProductionLotDailyCloseDetail
                    {
                        id = ++idAux,
                        id_productionLot = pldcda.id,
                        ProductionLot = pldcda,
                        id_productionLotDailyClose = productionLotDailyClose.id,
                        ProductionLotDailyClose = productionLotDailyClose
                    });
                }
                
            }

            productionLotDailyClose.ProductionLotDailyCloseDetail = productionLotDailyCloseDetail;
           

            TempData["productionLotDailyClose"] = productionLotDailyClose;
            TempData.Keep("productionLotDailyClose");

            var result = new
            {
                Message = "OK"
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductionLotDailyCloseProductionLotDetails()
        {
            ProductionLotDailyClose productionLotDailyClose = (TempData["productionLotDailyClose"] as ProductionLotDailyClose);
            productionLotDailyClose = productionLotDailyClose ?? new ProductionLotDailyClose();
            productionLotDailyClose.ProductionLotDailyCloseDetail = productionLotDailyClose.ProductionLotDailyCloseDetail ?? new List<ProductionLotDailyCloseDetail>();
            TempData.Keep("productionLotDailyClose");

            return Json(productionLotDailyClose.ProductionLotDailyCloseDetail.Select(d => d.id).ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}