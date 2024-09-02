using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using System.Web.UI.WebControls;
using DevExpress.Web.Mvc;
using DevExpress.Web;
using DevExpress.Utils;


namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class SalesQuotationController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region SALES QUOTATIONS FILTERS RESULTS

        [HttpPost]
        public ActionResult SalesQuotationResults(SalesQuotation quotation,
                                                   Document document,
                                                   DateTime? startEmissionDate, DateTime? endEmissionDate,
                                                   DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                                   int[] items)
        {

            List<SalesQuotation> model = db.SalesQuotation.AsEnumerable().ToList();

            #region DOCUMENT FILTERS

            if (document.id_documentState != 0)
            {
                model = model.Where(o => o.Document.id_documentState == document.id_documentState).ToList();
            }

            if (!string.IsNullOrEmpty(document.number))
            {
                model = model.Where(o => o.Document.number.Contains(document.number)).ToList();
            }

            if (!string.IsNullOrEmpty(document.reference))
            {
                model = model.Where(o => o.Document.reference.Contains(document.reference)).ToList();
            }

            if (startEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
            }

            if (endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            if (startAuthorizationDate != null)
            {
                model = model.Where(o => o.Document.authorizationDate != null && DateTime.Compare(startAuthorizationDate.Value.Date, o.Document.authorizationDate.Value.Date) <= 0).ToList();
            }

            if (endAuthorizationDate != null)
            {
                model = model.Where(o => o.Document.authorizationDate != null && DateTime.Compare(o.Document.authorizationDate.Value.Date, endAuthorizationDate.Value.Date) <= 0).ToList();
            }

            if (!string.IsNullOrEmpty(document.accessKey))
            {
                model = model.Where(o => o.Document.accessKey.Contains(document.accessKey)).ToList();
            }

            if (!string.IsNullOrEmpty(document.authorizationNumber))
            {
                model = model.Where(o => o.Document.authorizationNumber.Contains(document.authorizationNumber)).ToList();
            }

            #endregion

            #region SALES QUOTATION FILTERS

            if(quotation.id_customer != 0)
            {
                model = model.Where(o => o.id_customer == quotation.id_customer).ToList();
            }

            if (quotation.id_employeeSeller != 0)
            {
                model = model.Where(o => o.id_employeeSeller == quotation.id_employeeSeller).ToList();
            }

            //if (quotation.id != 0)
            //{
            //    model = model.Where(o => o.id == quotation.id).ToList();
            //}

            if (items != null && items.Length > 0)
            {
                var tempModel = new List<SalesQuotation>();
                foreach (var request in model)
                {
                    var details = request.SalesQuotationDetail.Where(d => items.Contains(d.id_item));
                    if (details.Any())
                    {
                        tempModel.Add(request);
                    }
                }

                model = tempModel;
            }

            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_SalesQuotationResultPartial", model.OrderByDescending(o => o.id).ToList());
        }

        #endregion

        #region SALES QUOTATION EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationEditForm(int id)
        {
            SalesQuotation quotation = db.SalesQuotation.FirstOrDefault(o => o.id == id);

            if (quotation == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("09"));//Cotizacion de Venta
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");//Pendiente
                //DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.id == 1);

                User user = db.User.FirstOrDefault(u => u.id == ActiveUser.id);
                Employee employee = user?.Employee ?? null;

                quotation = new SalesQuotation()
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
                    id_employeeSeller = employee?.id ?? 0,
                    Employee = employee,
                    SalesQuotationDetail = new List<SalesQuotationDetail>()
                };

            }

            quotation.SalesQuotationTotal = SaleQuotationTotals(quotation.id, quotation.SalesQuotationDetail.ToList());

            TempData["quotation"] = quotation;
            TempData.Keep("quotation");

            return PartialView("_SalesQuotationEditFormPartial", quotation);
        }
        
        #endregion

        #region SALES QUOTATION HEADER

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationsPartial()
        {
            var model = (TempData["model"] as List<SalesQuotation>);
            model = model ?? new List<SalesQuotation>();

            TempData.Keep("model");

            return PartialView("_SalesQuotationsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationPartialAddNew(bool approve, SalesQuotation itemSQ, Document itemDoc)
        {
            var model = db.SalesQuotation;
            SalesQuotation tempQuotation = (TempData["quotation"] as SalesQuotation);
            tempQuotation = tempQuotation ?? new SalesQuotation();

            tempQuotation.id_customer = itemSQ.id_customer;
            tempQuotation.id_employeeSeller = itemSQ.id_employeeSeller;
            tempQuotation.id_priceList = itemSQ.id_priceList;
            tempQuotation.Document = itemDoc;
            

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region DOCUMENT

                    itemDoc.id_userCreate = ActiveUser.id;
                    itemDoc.dateCreate = DateTime.Now;
                    itemDoc.id_userUpdate = ActiveUser.id;
                    itemDoc.dateUpdate = DateTime.Now;

                    itemDoc.sequential = GetDocumentSequential(itemDoc.id_documentType);
                    itemDoc.number = GetDocumentNumber(itemDoc.id_documentType);

                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == itemDoc.id_documentType);
                    itemDoc.DocumentType = documentType;

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == itemDoc.id_documentState);
                    itemDoc.DocumentState = documentState;

                    Employee employee = db.Employee.FirstOrDefault(e => e.id == ActiveUser.id_employee);
                    itemSQ.Employee = employee;
                    tempQuotation.Employee = employee;

                    itemDoc.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
                    itemDoc.id_emissionPoint = ActiveEmissionPoint.id;

                    string emissionDate = itemDoc.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");

                    itemDoc.accessKey = AccessKey.GenerateAccessKey(emissionDate,
                                                                    itemDoc.DocumentType.code,
                                                                    itemDoc.EmissionPoint.BranchOffice.Division.Company.ruc,
                                                                    "1",
                                                                    itemDoc.EmissionPoint.BranchOffice.code.ToString() + itemDoc.EmissionPoint.code.ToString("D3"),
                                                                    itemDoc.sequential.ToString("D9"),
                                                                    itemDoc.sequential.ToString("D8"),
                                                                    "1");

                    //Actualiza Secuencial
                    if (documentType != null)
                    {
                        documentType.currentNumber = documentType.currentNumber + 1;
                        db.DocumentType.Attach(documentType);
                        db.Entry(documentType).State = EntityState.Modified;
                    }
                    //itemPR.Employee.id = itemPR.id_personRequesting;


                    #endregion

                    #region SaleQuotation

                    itemSQ.Document = itemDoc;

                    #endregion

                    #region DETAILS

                    if (tempQuotation?.SalesQuotationDetail != null)
                    {
                        itemSQ.SalesQuotationDetail = new List<SalesQuotationDetail>();
                        var itemSQDetail = tempQuotation.SalesQuotationDetail.ToList();

                        foreach (var detail in itemSQDetail)
                        {
                            var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                            if (approve && detail.price <= 0)
                            {
                                throw new Exception("No se puede aprobar la cotización, Ítem: " + itemAux.name + " debe tener el precio mayor que cero.");
                                //TempData.Keep("quotation");
                                //ViewData["EditMessage"] = ErrorMessage("No se puede aprobar la cotización, Ítem: " + itemAux.name + " debe tener el precio mayor que cero.");
                                //return PartialView("_SalesQuotationMainEditForm", tempQuotation);
                            }

                            var tempItemSQDetail = new SalesQuotationDetail
                            {
                                id_item = detail.id_item,
                                Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                                quantity = detail.quantity,
                                quantityTypeUMPresentation = detail.quantityTypeUMPresentation,
                                id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation,
                                MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == detail.id_metricUnitTypeUMPresentation),
                                price = detail.price,
                                discount = detail.discount,
                                iva = detail.iva,
                                subtotal = detail.subtotal,
                                total = detail.total,

                                isActive = detail.isActive,
                                id_userCreate = detail.id_userCreate,
                                dateCreate = detail.dateCreate,
                                id_userUpdate = detail.id_userUpdate,
                                dateUpdate = detail.dateUpdate
                            };

                            if (tempItemSQDetail.isActive)
                                itemSQ.SalesQuotationDetail.Add(tempItemSQDetail);
                        }
                    }

                    #endregion

                    #region TOTALS

                    itemSQ.SalesQuotationTotal = SaleQuotationTotals(itemSQ.id, itemSQ.SalesQuotationDetail.ToList());

                    #endregion

                    if (itemSQ.SalesQuotationDetail.Count == 0)
                    {
                                throw new Exception("No se puede guardar una cotización sin detalles");
                        //TempData.Keep("quotation");
                        //ViewData["EditMessage"] = ErrorMessage("No se puede guardar una cotización sin detalles");

                        //return PartialView("_SalesQuotationMainEditForm", tempQuotation);
                    }

                    if (approve)
                    {
                        itemSQ.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                    }

                    db.SalesQuotation.Add(itemSQ);
                    db.SaveChanges();
                    trans.Commit();

                    TempData["quotation"] = itemSQ;
                    TempData.Keep("quotation");

                    ViewData["EditMessage"] = SuccessMessage("Cotización: " + itemSQ.Document.number + " guardado exitosamente");
                
                }
                catch (Exception e)
                {
                    TempData["quotation"] = tempQuotation;
                    TempData.Keep("quotation");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    return PartialView("_SalesQuotationMainEditForm", tempQuotation);

                    //TempData.Keep("quotation");
                    //ViewData["EditMessage"] = ErrorMessage();
                    //trans.Rollback();
                }

                return PartialView("_SalesQuotationMainEditForm", itemSQ);

            }

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationPartialUpdate(bool approve, SalesQuotation itemSQ, Document itemDoc)
        {
            var model = db.SalesQuotation;

            SalesQuotation tempQuotation = (TempData["quotation"] as SalesQuotation);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.id == itemSQ.id);
                    if (modelItem != null)
                    {
                        

                        #region UPDATE TEMPQUOTATION

                        tempQuotation.Document.description = itemDoc.description;
                        tempQuotation.Document.reference = itemDoc.reference;
                        tempQuotation.Document.emissionDate = itemDoc.emissionDate;

                        tempQuotation.id_customer = itemSQ.id_customer;
                        tempQuotation.id_priceList = itemSQ.id_priceList;

                        #endregion

                        #region DOCUMENT

                        modelItem.Document.description = itemDoc.description;
                        modelItem.Document.reference = itemDoc.reference;
                        modelItem.Document.emissionDate = itemDoc.emissionDate;

                        modelItem.Document.id_userUpdate = ActiveUser.id;
                        modelItem.Document.dateUpdate = DateTime.Now;

                        #endregion

                        #region SALE QUOTATION

                        modelItem.id_customer = itemSQ.id_customer;
                        modelItem.Customer = db.Customer.FirstOrDefault(fod=> fod.id == itemSQ.id_customer);
                        tempQuotation.Customer = modelItem.Customer;

                        modelItem.id_priceList = itemSQ.id_priceList;
                        modelItem.PriceList = db.PriceList.FirstOrDefault(fod=> fod.id == itemSQ.id_priceList);
                        tempQuotation.PriceList = modelItem.PriceList;

                        #endregion

                        #region SALE QUOTATION DETAILS

                        int count = 0;
                        if (tempQuotation?.SalesQuotationDetail != null)
                        {
                            var details = tempQuotation.SalesQuotationDetail.ToList();

                            foreach (var detail in details)
                            {
                                SalesQuotationDetail quotationDetail = modelItem.SalesQuotationDetail.FirstOrDefault(d => d.id == detail.id);

                                var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                                if (approve && detail.price <= 0)
                                {
                                    throw new Exception("No se puede aprobar la cotización, Ítem: " + itemAux.name + " debe tener el precio mayor que cero.");
                                }

                                if (quotationDetail == null)
                                {
                                    quotationDetail = new SalesQuotationDetail()
                                    {
                                        id_item = detail.id_item,
                                        Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                                        quantity = detail.quantity,
                                        quantityTypeUMPresentation = detail.quantityTypeUMPresentation,
                                        id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation,
                                        MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == detail.id_metricUnitTypeUMPresentation),
                                        price = detail.price,
                                        discount = detail.discount,
                                        iva = detail.iva,
                                        subtotal = detail.subtotal,
                                        total = detail.total,

                                        id_salesQuotation = detail.id_salesQuotation,
                                        SalesQuotation = modelItem,

                                        isActive = detail.isActive,
                                        id_userCreate = detail.id_userCreate,
                                        dateCreate = detail.dateCreate,
                                        id_userUpdate = detail.id_userUpdate,
                                        dateUpdate = detail.dateUpdate
                                    };

                                    if (quotationDetail.isActive)
                                    {
                                        modelItem.SalesQuotationDetail.Add(quotationDetail);
                                        count++;
                                    }

                                }
                                else
                                {
                                    quotationDetail.id_item = detail.id_item;
                                    quotationDetail.Item = db.Item.FirstOrDefault(i => i.id == detail.id_item);

                                    quotationDetail.quantity = detail.quantity;
                                    quotationDetail.quantityTypeUMPresentation = detail.quantityTypeUMPresentation;
                                    quotationDetail.id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation;
                                    quotationDetail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == detail.id_metricUnitTypeUMPresentation);
                                    quotationDetail.price = detail.price;
                                    quotationDetail.discount = detail.discount;
                                    quotationDetail.iva = detail.iva;
                                    quotationDetail.subtotal = detail.subtotal;
                                    quotationDetail.total = detail.total;

                                    quotationDetail.isActive = detail.isActive;
                                    //quotationDetail.id_userCreate = detail.id_userCreate;
                                    //quotationDetail.dateCreate = detail.dateCreate;
                                    quotationDetail.id_userUpdate = detail.id_userUpdate;
                                    quotationDetail.dateUpdate = detail.dateUpdate;

                                    if (quotationDetail.isActive)
                                        count++;
                                }
                            }

                            // UPDATE TOTALS
                            modelItem.SalesQuotationTotal = SaleQuotationTotals(modelItem.id, details);
                        }

                        if (count == 0)
                        {
                            throw new Exception("No se puede guardar una cotización sin detalles");
                        }

                        #endregion

                        if (approve)
                        {
                            modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        }

                        db.SalesQuotation.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        itemSQ = modelItem;
                        TempData["quotation"] = itemSQ;
                        TempData.Keep("quotation");

                        ViewData["EditMessage"] = SuccessMessage("Cotatización: " + itemSQ.Document.number + " guardado exitosamente");

                    }
                }
                catch (Exception e)
                {
                    TempData["quotation"] = tempQuotation;
                    TempData.Keep("quotation");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    return PartialView("_SalesQuotationMainEditForm", tempQuotation);
                    //TempData.Keep("quotation");
                    //ViewData["EditMessage"] = ErrorMessage();
                    //trans.Rollback();
                }
            }

            return PartialView("_SalesQuotationMainEditForm", itemSQ);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotataionPartialDelete(System.Int32 id)
        {
            //var model = db.PurchaseRequest;
            var model = db.Document;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_SalesQuotationsPartial", model.ToList());
        }
        
        #endregion
        
        #region SALES QUOTATION DETAILS

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationDetailsPartial()
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);
            //quotation = quotation ?? new SalesQuotation();
            var model = quotation?.SalesQuotationDetail.Where(d => d.isActive).ToList() ?? new List<SalesQuotationDetail>();

            //TempData["quotation"] = quotation;
            TempData.Keep("quotation");

            return PartialView("_SalesQuotationEditFormDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationDetailsAddNew(SalesQuotationDetail quotationDetail)
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);
            //quotation = quotation ?? db.SalesQuotation.FirstOrDefault(i => i.id == quotation.id);
            quotation = quotation ?? new SalesQuotation();

            if (ModelState.IsValid)
            {
                try
                {
                    quotationDetail.id = quotation.SalesQuotationDetail.Count() > 0 ? quotation.SalesQuotationDetail.Max(pld => pld.id) + 1 : 1;

                    quotationDetail.id_salesQuotation = quotation.id;
                    quotationDetail.SalesQuotation = quotation;
                    quotationDetail.Item = db.Item.FirstOrDefault(i => i.id == quotationDetail.id_item);
                    quotationDetail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == quotationDetail.id_metricUnitTypeUMPresentation);

                    quotationDetail.isActive = true;
                    quotationDetail.id_userCreate = ActiveUser.id;
                    quotationDetail.dateCreate = DateTime.Now;
                    quotationDetail.id_userUpdate = ActiveUser.id;
                    quotationDetail.dateUpdate = DateTime.Now;

                    quotation.SalesQuotationDetail.Add(quotationDetail);

                    quotation.SalesQuotationTotal = SaleQuotationTotals(quotation.id, quotation.SalesQuotationDetail.ToList());
                    TempData["quotation"] = quotation;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("quotation");

            var model = quotation?.SalesQuotationDetail.Where(d => d.isActive).ToList() ?? new List<SalesQuotationDetail>();

            return PartialView("_SalesQuotationEditFormDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationDetailsUpdate(SalesQuotationDetail quotationsDetail)
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);
            //quotation = quotation ?? db.SalesQuotation.FirstOrDefault(i => i.id == quotation.id);
            quotation = quotation ?? new SalesQuotation();

            //var model = db.SalesQuotationDetail;
            if (ModelState.IsValid)
            {
                try
                {
                    SalesQuotationDetail detail = quotation.SalesQuotationDetail.FirstOrDefault(it => it.id == quotationsDetail.id);
                    if (detail != null)
                    {
                        detail.id_item = quotationsDetail.id_item;
                        detail.Item = db.Item.FirstOrDefault(i => i.id == quotationsDetail.id_item);

                        detail.quantity = quotationsDetail.quantity;
                        detail.quantityTypeUMPresentation = quotationsDetail.quantityTypeUMPresentation;
                        detail.id_metricUnitTypeUMPresentation = quotationsDetail.id_metricUnitTypeUMPresentation;
                        detail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == quotationsDetail.id_metricUnitTypeUMPresentation);

                        detail.price = quotationsDetail.price;
                        //detail.discount = quotationsDetail.discount;
                        detail.iva = quotationsDetail.iva;
                        detail.subtotal = quotationsDetail.subtotal;
                        detail.total = quotationsDetail.total;

                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateCreate = DateTime.Now;

                        this.UpdateModel(detail);

                        quotation.SalesQuotationTotal = SaleQuotationTotals(quotation.id, quotation.SalesQuotationDetail.ToList());

                        TempData["quotation"] = quotation;

                        // this.UpdateModel(modelItem);
                        //db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("quotation");

            var model = quotation?.SalesQuotationDetail.Where(d => d.isActive).ToList() ?? new List<SalesQuotationDetail>();

            return PartialView("_SalesQuotationEditFormDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesQuotationDetailsDelete(int id)
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);

            quotation = quotation ?? db.SalesQuotation.FirstOrDefault(i => i.id == quotation.id);
            quotation = quotation ?? new SalesQuotation();

            //if (id_item >= 0)
            //{
            try
            {
                var quotationDetail = quotation.SalesQuotationDetail.FirstOrDefault(p => p.id == id);
                if (quotationDetail != null)
                {
                    quotationDetail.isActive = false;
                    quotationDetail.id_userUpdate = ActiveUser.id;
                    quotationDetail.dateCreate = DateTime.Now;
                }
                quotation.SalesQuotationTotal = SaleQuotationTotals(quotation.id, quotation.SalesQuotationDetail.ToList());
                TempData["quotation"] = quotation;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("quotation");

            var model = quotation?.SalesQuotationDetail.Where(d => d.isActive).ToList() ?? new List<SalesQuotationDetail>();
            return PartialView("_SalesQuotationEditFormDetailsPartial", model.ToList());
        }
        
        #endregion
        
        #region SINGLE CHANGE DOCUMENT STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(i => i.id == id);
            using (DbContextTransaction trans = db.Database.BeginTransaction())

            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                    if (salesQuotation != null && documentState != null)
                    {
                        salesQuotation.Document.id_documentState = documentState.id;
                        salesQuotation.Document.DocumentState = documentState;

                        foreach (var details in salesQuotation.SalesQuotationDetail)
                        {
                            // details.quantity = (details.quantity> 0) ? details.quantity : details.quantity;
                            // details.quantityOutstandingPurchase = (details.quantityOutstandingPurchase > 0) ? details.quantityOutstandingPurchase : details.quantityRequested;

                            db.SalesQuotationDetail.Attach(details);
                            db.Entry(details).State = EntityState.Modified;
                        }

                        db.SalesQuotation.Attach(salesQuotation);
                        db.Entry(salesQuotation).State = EntityState.Modified;

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

            TempData["quotation"] = salesQuotation;
            TempData.Keep("quotation");

            return PartialView("_SalesQuotationMainEditForm", salesQuotation);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada

                    if (salesQuotation != null && documentState != null)
                    {
                        salesQuotation.Document.id_documentState = documentState.id;
                        salesQuotation.Document.DocumentState = documentState;

                        db.SalesQuotation.Attach(salesQuotation);
                        db.Entry(salesQuotation).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["quotation"] = salesQuotation;
                        TempData.Keep("quotation");

                        ViewData["EditMessage"] = SuccessMessage("Cotización: " + salesQuotation.Document.number + " autorizada exitosamente");
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("quotation");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();

                    //ViewData["EditError"] = e.Message;
                    //trans.Rollback();
                }
            }

            //TempData["quotation"] = salesQuotation;
            //TempData.Keep("quotation");

            return PartialView("_SalesQuotationMainEditForm", salesQuotation);
        }

        [HttpPost]
        public ActionResult Protect(int id)
        {
            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                    if (salesQuotation != null && documentState != null)
                    {
                        salesQuotation.Document.id_documentState = documentState.id;
                        salesQuotation.Document.DocumentState = documentState;

                        db.SalesQuotation.Attach(salesQuotation);
                        db.Entry(salesQuotation).State = EntityState.Modified;

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

            TempData["quotation"] = salesQuotation;
            TempData.Keep("quotation");

            return PartialView("_SalesQuotationMainEditForm", salesQuotation);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var existInSaleRequestDetailSaleQuotatioDetail = salesQuotation.SalesQuotationDetail.FirstOrDefault(fod => fod.SalesRequestDetailSalesQuotation.FirstOrDefault(fod2 => fod2.SalesRequestDetail.SalesRequest.Document.DocumentState.code != "05" &&
                                                                                                                                                                                           fod2.SalesRequestDetail.SalesRequest.Document.DocumentState.code != "01") != null);//Diferente de Anulado y Pendiente

                    if (existInSaleRequestDetailSaleQuotatioDetail != null)
                    {
                        throw new Exception("No se puede cancelar la cotización, debido a tener detalles que pertenecen a requerimiento de Pedido de Venta.");
                        //TempData.Keep("purchaseOrder");
                        //ViewData["EditMessage"] = ErrorMessage("No se puede anular la orden de compra debido a tener detalles que pertenecen a requerimiento de recepción de Lote.");
                        //return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (salesQuotation != null && documentState != null)
                    {
                        salesQuotation.Document.id_documentState = documentState.id;
                        salesQuotation.Document.DocumentState = documentState;

                        db.SalesQuotation.Attach(salesQuotation);
                        db.Entry(salesQuotation).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["quotation"] = salesQuotation;
                        TempData.Keep("quotation");

                        ViewData["EditMessage"] = SuccessMessage("Cotización: " + salesQuotation.Document.number + " cancelada exitosamente");
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("quotation");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    //ViewData["EditError"] = e.Message;
                    //trans.Rollback();
                }
            }

            //TempData["quotation"] = salesQuotation;
            //TempData.Keep("quotation");

            return PartialView("_SalesQuotationMainEditForm", salesQuotation);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var existInSaleRequestDetailSaleQuotatioDetail = salesQuotation.SalesQuotationDetail.FirstOrDefault(fod => fod.SalesRequestDetailSalesQuotation.FirstOrDefault(fod2 => fod2.SalesRequestDetail.SalesRequest.Document.DocumentState.code != "05" &&
                                                                                                                                                                                           fod2.SalesRequestDetail.SalesRequest.Document.DocumentState.code != "01") != null);//Diferente de Anulado y Pendiente

                    if (existInSaleRequestDetailSaleQuotatioDetail != null)
                    {
                        throw new Exception("No se puede reversar la cotización, debido a tener detalles que pertenecen a requerimiento de Pedido de Venta.");
                        //TempData.Keep("purchaseOrder");
                        //ViewData["EditMessage"] = ErrorMessage("No se puede anular la orden de compra debido a tener detalles que pertenecen a requerimiento de recepción de Lote.");
                        //return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

                    if (salesQuotation != null && documentState != null)
                    {
                        salesQuotation.Document.id_documentState = documentState.id;
                        salesQuotation.Document.DocumentState = documentState;

                        db.SalesQuotation.Attach(salesQuotation);
                        db.Entry(salesQuotation).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["quotation"] = salesQuotation;
                        TempData.Keep("quotation");

                        ViewData["EditMessage"] = SuccessMessage("Cotización: " + salesQuotation.Document.number + " reversada exitosamente");
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("quotation");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    //ViewData["EditError"] = e.Message;
                    //trans.Rollback();
                }
            }

            //TempData["quotation"] = salesQuotation;
            //TempData.Keep("quotation");

            return PartialView("_SalesQuotationMainEditForm", salesQuotation);
        }

        #endregion
        
        #region SELECT DOCUMENT STATE CHANGE
        [HttpPost, ValidateInput(false)]
        public void ApproveDocuments(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                            if (salesQuotation != null && documentState != null)
                            {
                                salesQuotation.Document.id_documentState = documentState.id;
                                salesQuotation.Document.DocumentState = documentState;

                                //foreach (var details in salesQuotation.SalesQuotationDetail)
                                //{
                                //    // details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;
                                //    //  details.quantityOutstandingPurchase = (details.quantityOutstandingPurchase > 0) ? details.quantityOutstandingPurchase : details.quantityRequested;

                                //    db.SalesQuotationDetail.Attach(details);
                                //    db.Entry(details).State = EntityState.Modified;
                                //}

                                db.SalesQuotation.Attach(salesQuotation);
                                db.Entry(salesQuotation).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<SalesQuotation>);
            model = model ?? new List<SalesQuotation>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.SalesQuotation.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                        foreach (var id in ids)
                        {
                            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

                            if (salesQuotation != null && documentState != null)
                            {
                                salesQuotation.Document.id_documentState = documentState.id;
                                salesQuotation.Document.DocumentState = documentState;

                                //foreach (var details in salesQuotation.SalesQuotationDetail)
                                //{
                                //    // details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;
                                //    // details.quantityOutstandingPurchase = (details.quantityOutstandingPurchase > 0) ? details.quantityOutstandingPurchase : details.quantityRequested;

                                //    db.SalesQuotationDetail.Attach(details);
                                //    db.Entry(details).State = EntityState.Modified;
                                //}

                                db.SalesQuotation.Attach(salesQuotation);
                                db.Entry(salesQuotation).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<SalesQuotation>);
            model = model ?? new List<SalesQuotation>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.SalesQuotation.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                        foreach (var id in ids)
                        {
                            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                            if (salesQuotation != null && documentState != null)
                            {
                                salesQuotation.Document.id_documentState = documentState.id;
                                salesQuotation.Document.DocumentState = documentState;

                                db.SalesQuotation.Attach(salesQuotation);
                                db.Entry(salesQuotation).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<SalesQuotation>);
            model = model ?? new List<SalesQuotation>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.SalesQuotation.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                        foreach (var id in ids)
                        {
                            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

                            if (salesQuotation != null && documentState != null)
                            {
                                salesQuotation.Document.id_documentState = documentState.id;
                                salesQuotation.Document.DocumentState = documentState;

                                db.SalesQuotation.Attach(salesQuotation);
                                db.Entry(salesQuotation).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<SalesQuotation>);
            model = model ?? new List<SalesQuotation>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.SalesQuotation.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                            if (salesQuotation != null && documentState != null)
                            {
                                salesQuotation.Document.id_documentState = documentState.id;
                                salesQuotation.Document.DocumentState = documentState;

                                foreach (var details in salesQuotation.SalesQuotationDetail)
                                {
                                    details.quantity = 0.0M;

                                    db.SalesQuotationDetail.Attach(details);
                                    db.Entry(details).State = EntityState.Modified;
                                }

                                db.SalesQuotation.Attach(salesQuotation);
                                db.Entry(salesQuotation).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<SalesQuotation>);
            model = model ?? new List<SalesQuotation>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.SalesQuotation.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
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
                btnRevert = false
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            SalesQuotation salesQuotation = db.SalesQuotation.FirstOrDefault(r => r.id == id);
            string state = salesQuotation.Document.DocumentState.code;

            if (state == "01") // PENDIENTE
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
            else if (state == "03")//|| state == 3) // APROBADA
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
            else if (state == "04" || state == "05") // CERRADA O ANULADA
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
            else if (state == "06") // AUTORIZADA
            {
                //var purchaseOrderDetailAux = purchaseOrder.PurchaseOrderDetail.Where(w => w.isActive = true).FirstOrDefault(fod => fod.quantityReceived < fod.quantityApproved);

                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_salesQuotation)
        {
            TempData.Keep("quotation");

            int index = db.SalesQuotation.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_salesQuotation);

            var result = new
            {
                maximunPages = db.SalesQuotation.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            SalesQuotation salesQuotation = db.SalesQuotation.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (salesQuotation != null)
            {
                TempData["quotation"] = salesQuotation;
                TempData.Keep("quotation");
                return PartialView("_SalesQuotationMainEditForm", salesQuotation);
            }

            TempData.Keep("quotation");

            return PartialView("_SalesQuotationMainEditForm", new SalesQuotation());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        private SalesQuotationTotal SaleQuotationTotals(int id_saleQuotation, List<SalesQuotationDetail> quotationDetails)
        {
            SalesQuotationTotal salesQuotationTotal = db.SalesQuotationTotal.FirstOrDefault(t => t.id_salesQuotation == id_saleQuotation);

            salesQuotationTotal = salesQuotationTotal ?? new SalesQuotationTotal
            {
                id_salesQuotation = id_saleQuotation
            };

            decimal subtotalIVA12Percent = 0.0M;
            decimal subtotalIVA14Percent = 0.0M;
            decimal subtotalIVA0Percent = 0.0M;
            decimal subtotalIVANoObjectIVA = 0.0M;
            decimal subtotalExentedIVA = 0.0M;

            decimal subtotal = 0.0M;

            decimal discount = 0.0M;
            decimal valueICE = 0.0M;
            decimal valueIRBPNR = 0.0M;

            decimal totalIVA12 = 0.0M;
            decimal totalIVA14 = 0.0M;

            decimal total = 0.0M;

            foreach (var detail in quotationDetails)
            {
                if (detail.Item != null && detail.isActive)
                {
                    //ItemTaxation rateIVA12 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("2"));
                    ItemTaxation rateIVA12 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("2"));//Taxtype: "2": Impuesto IVA con Rate: "2": tarifa 12% 
                    ItemTaxation rateIVA14 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("5"));//Taxtype: "2": Impuesto IVA con Rate: "5": tarifa 14% 

                    if (rateIVA12 != null)
                    {
                        subtotalIVA12Percent += detail.quantityTypeUMPresentation * detail.price;
                    }

                    if (rateIVA14 != null)
                    {
                        subtotalIVA14Percent += detail.quantityTypeUMPresentation * detail.price;
                    }

                    subtotal += detail.quantityTypeUMPresentation * detail.price;
                }

            }

            //totalIVA12 = subtotalIVA12Percent * 0.12M;
            //totalIVA14 = subtotalIVA14Percent * 0.14M;
            var percent12 = db.Rate.FirstOrDefault(fod => fod.code.Equals("2"))?.percentage/100 ?? 0.12M; //"2" Tarifa 12%
            var percent14 = db.Rate.FirstOrDefault(fod => fod.code.Equals("5"))?.percentage/100 ?? 0.14M; //"5" Tarifa 14%
            totalIVA12 = subtotalIVA12Percent * percent12;// 0.12M;
            totalIVA14 = subtotalIVA14Percent * percent14;//0.14M;

            total = subtotal + totalIVA12 + totalIVA14 + valueICE + valueIRBPNR - discount;

            salesQuotationTotal.subtotalIVA12Percent = subtotalIVA12Percent;
            salesQuotationTotal.subtotalIVA14Percent = subtotalIVA14Percent;
            salesQuotationTotal.subtotalIVA0Percent = subtotalIVA0Percent;
            salesQuotationTotal.subtotalIVANoObjectIVA = subtotalIVANoObjectIVA;
            salesQuotationTotal.subtotalExentedIVA = subtotalExentedIVA;

            salesQuotationTotal.subtotal = subtotal;
            salesQuotationTotal.discount = discount;
            salesQuotationTotal.valueICE = valueICE;
            salesQuotationTotal.valueIRBPNR = valueIRBPNR;

            salesQuotationTotal.totalIVA12 = totalIVA12;
            salesQuotationTotal.totalIVA14 = totalIVA14;

            salesQuotationTotal.total = total;

            return salesQuotationTotal;
        }
        
        private decimal SaleQuotationDetailPrice(int id_item, SalesQuotation quotation)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return 0.0M;
            }

            PriceList list = quotation.PriceList;

            decimal price = item.ItemPurchaseInformation?.purchasePrice ?? 0.0M;

            if (list != null)
            {
                PriceListDetail listDetail = list.PriceListDetail.FirstOrDefault(d => d.id_item == item.id);
                price = listDetail?.salePrice ?? price;
            }

            return price;
        }

        private decimal SaleQuotationDetailIVA(int id_item, decimal quantity, decimal price)
        {
            decimal iva = 0.0M;

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return 0.0M;
            }

            List<ItemTaxation> taxations = item.ItemTaxation.ToList();

            foreach (var taxation in taxations)
            {
                iva += quantity * price * taxation.Rate.percentage / 100.0M;
            }

            return iva;
        }
        
        //[HttpPost, ValidateInput(false)]
        //public JsonResult ItemDetailData(int id_item, string quantity, string price)
        //{
        //    decimal _quantity = decimal.Parse(quantity, NumberStyles.Any, new CultureInfo(Request.UserLanguages.First()));
        //    decimal _price = decimal.Parse(price, NumberStyles.Any, new CultureInfo(Request.UserLanguages.First()));


        //    SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);

        //    Item item = db.Item.FirstOrDefault(i => i.id == id_item);

        //    if (item == null)
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }

        //    if (_price == 0.0M)
        //    {
        //        _price = SaleQuotationDetailPrice(item.id, quotation);
        //    }

        //    decimal iva = SaleQuotationDetailIVA(item.id, _quantity, _price);

        //    SalesQuotationDetail detail = quotation.SalesQuotationDetail.FirstOrDefault(d => d.id_item == id_item);

        //    if (detail != null)
        //    {
        //        detail.price = _price;
        //        detail.quantity = _quantity;
        //        quotation.SalesQuotationTotal = SaleQuotationTotals(quotation.id, quotation.SalesQuotationDetail.ToList());
        //    }

        //    var result = new
        //    {
        //        ItemDetailData = new
        //        {
        //            masterCode = item.masterCode,
        //            um = item.ItemPurchaseInformation.MetricUnit.code,
        //            price = _price,
        //            iva = iva,
        //            subtotal = _price * _quantity,
        //            total = _price * _quantity + iva
        //        },
        //        QuotationTotal = new
        //        {
        //            subtotal = quotation.SalesQuotationTotal.subtotal,
        //            subtotalIVA12Percent = quotation.SalesQuotationTotal.subtotalIVA12Percent,
        //            totalIVA12 = quotation.SalesQuotationTotal.totalIVA12,
        //            discount = quotation.SalesQuotationTotal.discount,
        //            subtotalIVA14Percent = quotation.SalesQuotationTotal.subtotalIVA14Percent,
        //            totalIVA14 = quotation.SalesQuotationTotal.totalIVA14,
        //            total = quotation.SalesQuotationTotal.total
        //        }
        //    };

        //    TempData["quotation"] = quotation;
        //    TempData.Keep("quotation");

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost, ValidateInput(false)]
        public JsonResult SaleQuotationDetails()
        {
            SalesQuotation salesQuotation = (TempData["quotation"] as SalesQuotation);
            salesQuotation = salesQuotation ?? new SalesQuotation();

            TempData.Keep("quotation");

            return Json(salesQuotation.SalesQuotationDetail.Select(d => d.id_item).ToList(),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult QuotationTotals()
        {
            SalesQuotation salesQuotation = (TempData["quotation"] as SalesQuotation);

            salesQuotation = salesQuotation ?? new SalesQuotation();
            salesQuotation.SalesQuotationDetail = salesQuotation.SalesQuotationDetail ?? new List<SalesQuotationDetail>();

            salesQuotation.SalesQuotationTotal = SaleQuotationTotals(salesQuotation.id, salesQuotation.SalesQuotationDetail.ToList());

            TempData["quotation"] = salesQuotation;
            TempData.Keep("quotation");

            var result = new
            {
                quotationSubtotal = salesQuotation.SalesQuotationTotal.subtotal,
                quotationSubtotalIVA12Percent = salesQuotation.SalesQuotationTotal.subtotalIVA12Percent,
                quotationTotalIVA12 = salesQuotation.SalesQuotationTotal.totalIVA12,
                quotationDiscount = salesQuotation.SalesQuotationTotal.discount,
                quotationSubtotalIVA14Percent = salesQuotation.SalesQuotationTotal.subtotalIVA14Percent,
                quotationTotalIVA14 = salesQuotation.SalesQuotationTotal.totalIVA14,
                quotationTotal = salesQuotation.SalesQuotationTotal.total
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult OnPriceList_SelectedIndexChanged(int? id_priceList)
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);
            quotation = quotation ?? new SalesQuotation();

            var priceList = db.PriceList.FirstOrDefault(fod=> fod.id == id_priceList);
            quotation.PriceList = priceList;
            var listSalesQuotationDetail = quotation.SalesQuotationDetail.ToList();
            decimal priceAux = 0;
            
            foreach (var salesQuotationDetail in listSalesQuotationDetail)
            {
                
                if (priceList != null)
                {
                    //var metricUnitPresentation = salesQuotationDetail.Item.Presentation?.MetricUnit;
                    var detailtPriceList = priceList?.PriceListDetail.FirstOrDefault(fod => fod.id_item == salesQuotationDetail.id_item);
                    var metricUnitPriceList = detailtPriceList?.MetricUnit;

                    if (metricUnitPriceList != null)
                    {
                        var metricUnitSale = salesQuotationDetail.Item.ItemSaleInformation.MetricUnit;

                        var metricUnitAux = metricUnitPriceList;
                        decimal priceMetricUnitPresentation = detailtPriceList.salePrice;

                        if (metricUnitPriceList.id_metricType == metricUnitSale.id_metricType)
                        {
                            metricUnitAux = salesQuotationDetail.Item.Presentation?.MetricUnit;
                            //priceMetricUnitPresentation = detailtPriceList.salePrice / (salesQuotationDetail.Item.Presentation?.minimum ?? 1);

                            var factorConversionAux = (metricUnitSale.id != metricUnitPriceList.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitSale.id &&
                                                                                                                                             fod.id_metricDestiny == metricUnitPriceList.id)?.factor ?? 0 : 1;
                            //priceAux = 0;
                            if (factorConversionAux == 0)
                            {
                                throw new Exception("Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

                            }
                            else
                            {
                                priceMetricUnitPresentation = (priceMetricUnitPresentation * factorConversionAux) / (salesQuotationDetail.Item.Presentation?.minimum ?? 1);
                            }

                            //priceMetricUnitPresentation = detailtPriceList.salePrice / (item.Presentation?.minimum ?? 1);

                        }

                        var metricUnitDetail = salesQuotationDetail.MetricUnit;

                        var factorConversion = (metricUnitDetail.id != metricUnitAux.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitDetail.id &&
                                                                                                                                         fod.id_metricDestiny == metricUnitAux.id)?.factor ?? 0 : 1;
                        priceAux = 0;
                        if (factorConversion == 0)
                        {
                            throw new Exception("Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para recoste el detalle de la Cotización Configúrelo, e intente de nuevo");

                        }
                        else
                        {
                            priceAux = priceMetricUnitPresentation * factorConversion;
                        }
                    }
                    else
                    {
                        priceAux = 0;
                    }
                    
                }
                else
                {
                    priceAux = 0;
                }

                salesQuotationDetail.price = priceAux;
            }

            //quotation.SalesQuotationTotal = SaleQuotationTotals(quotation.id, quotation.SalesQuotationDetail.ToList());

            var result = new
            {
                Message = "Ok"
            };

            TempData["quotation"] = quotation;
            TempData.Keep("quotation");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItemDetail(int? id_itemNew)
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);

            quotation = quotation ?? new SalesQuotation();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };


            var quotationDetailAux = quotation.SalesQuotationDetail.FirstOrDefault(w => w.id_item == id_itemNew && w.isActive);
            //foreach (var detail in purchaseRequestDetailAux)
            //{
                if (quotationDetailAux != null)
                {
                    var itemNewAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                    //var providerAux = db.Provider.FirstOrDefault(fod => fod.id == id_proposedProviderNew);
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede repetir el Ítem: " + itemNewAux.name + " en los detalles."

                    };

                }
            //}




            TempData["quotation"] = quotation;
            TempData.Keep("quotation");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SalesQuotationDetails(int? id_itemCurrent, int? id_metricUnitTypeUMPresentationCurrent)
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);

            //quotation = quotation ?? new SalesQuotation();
            //quotation.SalesQuotationDetail = quotation.SalesQuotationDetail ?? new List<SalesQuotationDetail>();

            var items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.isSold && w.InventoryLine.code == "PT") || w.id == id_itemCurrent).ToList();
            var item = db.Item.FirstOrDefault(fod=> fod.id == id_itemCurrent);
            var metricUnits = item?.Presentation.MetricUnit.MetricType.MetricUnit.Where(w=> (w.isActive && w.id_company == this.ActiveCompanyId) || w.id == id_metricUnitTypeUMPresentationCurrent).ToList() ?? new List<MetricUnit>();

            var result = new
            {
                //purchaseOrderDetails = purchaseOrder.PurchaseOrderDetail.Where(d => d.isActive && d.id_item != id_itemCurrent).Select(d => d.id_item).ToList(),
                //purchaseItemsByPurchaseReason = purchaseItemsByPurchaseReason.Select(d => d.id).ToList(),
                items = items.Select(s => new
                {
                    id = s.id,
                    masterCode = s.masterCode,
                    itemSaleInformationMetricUnitCode = (s.ItemSaleInformation != null) ? s.ItemSaleInformation.MetricUnit.code : "",
                    name = s.name
                }).ToList(),
                metricUnits = metricUnits.Select(s => new
                {
                    id = s.id,
                    code = s.code
                }).ToList(),
                Message = "Ok"
            };

            TempData.Keep("quotation");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemDetailData(int? id, int? id_item, string quantityTypeUMPresentation, int? id_metricUnitTypeUMPresentation)
        {

            //decimal _quantityOrdered = decimal.Parse(quantityOrdered, NumberStyles.Any, new CultureInfo(Request.UserLanguages.First()));
            //decimal _price = decimal.Parse(price, NumberStyles.Any, new CultureInfo(Request.UserLanguages.First()));

            decimal _quantityTypeUMPresentation = Convert.ToDecimal(quantityTypeUMPresentation);
            //decimal _price = Convert.ToDecimal(price);

            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);

            SalesQuotation quotation2 = (TempData["quotation2"] as SalesQuotation);

            quotation2 = quotation2 ?? Copy(quotation, quotation2);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            MetricUnit metricUnitTypeUMPresentation = db.MetricUnit.FirstOrDefault(i => i.id == id_metricUnitTypeUMPresentation);
                      

            //Item item = db.Item.FirstOrDefault(i => i.id == id_itemRequest);
            decimal quantitySale = 0;
            string metricUnitSale = "";
            string msgErrorConversion = "";
            //decimal _quantitySchedule = Convert.ToDecimal(quantitySchedule ?? "0");

            if (item != null)
            {
                metricUnitSale = item.ItemSaleInformation?.MetricUnit?.code ?? "";

                //metricUnitTypeUMPresentation var metricUnitRequest = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitRequest);

                if (metricUnitTypeUMPresentation != null)
                {
                    var id_metricUnitPresentation = item.Presentation?.id_metricUnit;
                    var factorConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                         muc.id_metricOrigin == metricUnitTypeUMPresentation.id &&
                                                                                         muc.id_metricDestiny == id_metricUnitPresentation);
                    if (id_metricUnitPresentation != null && id_metricUnitPresentation == metricUnitTypeUMPresentation.id)
                    {
                        factorConversion = new MetricUnitConversion() { factor = 1 };
                    }
                    if (factorConversion == null)
                    {
                        //var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
                        msgErrorConversion = ("Falta el Factor de Conversión entre : " + (metricUnitTypeUMPresentation.code) + " y " + (item.Presentation?.MetricUnit?.code ?? "(UM de Presentación No Existe)") + ". Necesario para calcular las cantidades Configúrelo, e intente de nuevo");
                    }
                    else
                    {
                        var quantityAux = _quantityTypeUMPresentation * factorConversion.factor;
                        quantityAux /= (item.Presentation?.minimum ?? 1);
                        var truncateQuantityAux = decimal.Truncate(quantityAux);
                        if ((quantityAux - truncateQuantityAux) > 0)
                        {
                            quantityAux = truncateQuantityAux + 1;
                        };
                        quantitySale = quantityAux;

                        //var id_metricUnitPresentation = item.Presentation?.id_metricUnit;
                        factorConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                             muc.id_metricOrigin == id_metricUnitPresentation &&
                                                                                             muc.id_metricDestiny == metricUnitTypeUMPresentation.id);
                        if (id_metricUnitPresentation != null && id_metricUnitPresentation == metricUnitTypeUMPresentation.id)
                        {
                            factorConversion = new MetricUnitConversion() { factor = 1 };
                        }
                        if (factorConversion == null)
                        {
                            //var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
                            msgErrorConversion = ("Falta el Factor de Conversión entre : " + (item.Presentation?.MetricUnit?.code ?? "(UM de Presentación No Existe)") + " y " + (metricUnitTypeUMPresentation.code) + ". Necesario para calcular las cantidades Configúrelo, e intente de nuevo");
                        }
                        else
                        {
                            quantityAux = (quantitySale * (item.Presentation?.minimum ?? 1)) * factorConversion.factor;

                            _quantityTypeUMPresentation = quantityAux;
                        }
                    }
                }
            }

            decimal _price;
            try
            {
                _price = SaleDetailPrice(item.id, quotation2, id_metricUnitTypeUMPresentation);
            }
            catch(Exception e)
            {
                _price = 0;
                msgErrorConversion = e.Message;
            }
            

            decimal iva = SaleDetailIVA(item.id, _quantityTypeUMPresentation, _price);

            SalesQuotationDetail detail = quotation2.SalesQuotationDetail.FirstOrDefault(d => d.id_item == id_item && d.id == id);


            if (detail != null)
            {
                detail.price = _price;
                detail.quantityTypeUMPresentation = _quantityTypeUMPresentation;
                quotation2.SalesQuotationTotal = SaleQuotationTotals(quotation2.id, quotation2.SalesQuotationDetail.ToList());
            }
            else
            {
                detail = new SalesQuotationDetail()
                {
                    id = quotation2.SalesQuotationDetail.Count() > 0 ? quotation2.SalesQuotationDetail.Max(ppd => ppd.id) + 1 : 1,
                    //id_item = id_item.Value,
                    Item = db.Item.FirstOrDefault(i => i.id == id_item),
                    isActive = true,
                    price = _price,
                    iva = iva,
                    subtotal = _price * _quantityTypeUMPresentation,
                    total = _price * _quantityTypeUMPresentation + iva,
                    SalesQuotation = quotation2,
                    id_salesQuotation = quotation2.id,
                    quantityTypeUMPresentation = _quantityTypeUMPresentation,
                    //id_metricUnitTypeUMPresentation = id_metricUnitTypeUMPresentation.Value,
                    MetricUnit = metricUnitTypeUMPresentation
                };
                if (id_item != null) detail.id_item = id_item.Value;
                if (id_metricUnitTypeUMPresentation != null) detail.id_metricUnitTypeUMPresentation = id_metricUnitTypeUMPresentation.Value;

                quotation2.SalesQuotationDetail.Add(detail);
                quotation2.SalesQuotationTotal = SaleQuotationTotals(quotation2.id, quotation2.SalesQuotationDetail.ToList());

            }

            var metricUnits = item?.Presentation.MetricUnit.MetricType.MetricUnit.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId) || w.id == id_metricUnitTypeUMPresentation).ToList() ?? new List<MetricUnit>();

            var result = new
            {
                masterCode = item.masterCode,
                //metricUnit = item.ItemSaleInformation?.MetricUnit?.code ?? "",
                ItemDetailData = new
                {
                    price = _price,
                    iva = iva,
                    subtotal = _price * _quantityTypeUMPresentation,
                    total = _price * _quantityTypeUMPresentation + iva
                },
                    OrderTotal = new
                {
                    subtotal = quotation2.SalesQuotationTotal.subtotal,
                    subtotalIVA12Percent = quotation2.SalesQuotationTotal.subtotalIVA12Percent,
                    totalIVA12 = quotation2.SalesQuotationTotal.totalIVA12,
                    discount = quotation2.SalesQuotationTotal.discount,
                    subtotalIVA14Percent = quotation2.SalesQuotationTotal.subtotalIVA14Percent,
                    totalIVA14 = quotation2.SalesQuotationTotal.totalIVA14,
                    total = quotation2.SalesQuotationTotal.total
                },
                quantity = quantitySale,
                metricUnit = metricUnitSale,
                quantityTypeUMPresentation = _quantityTypeUMPresentation,
                msgErrorConversion = msgErrorConversion,
                metricUnits = metricUnits.Select(s => new
                {
                    id = s.id,
                    code = s.code
                }).ToList()
            };

            TempData["quotation2"] = quotation2;
            TempData.Keep("quotation2");

            TempData["quotation"] = quotation;
            TempData.Keep("quotation");

            //return Json(result, JsonRequestBehavior.AllowGet);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private decimal SaleDetailPrice(int id_item, SalesQuotation quotation, int? id_metricUnitTypeUMPresentation)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);
            MetricUnit metricUnitTypeUMPresentation = db.MetricUnit.FirstOrDefault(i => i.id == id_metricUnitTypeUMPresentation);

            if (item == null || metricUnitTypeUMPresentation == null)
            {
                return 0.0M;
            }

            PriceList list = quotation.PriceList;

            decimal priceUMPresentation = item.ItemSaleInformation.salePrice/((item.Presentation?.maximum ?? 1) * (item.Presentation?.minimum ?? 1)) ?? 0.0M;

            decimal priceAux = 0;
            var metricUnitPresentation = item.Presentation?.MetricUnit;

            if(metricUnitPresentation?.id != null)
            {
                var factorConversion = (metricUnitTypeUMPresentation.id != metricUnitPresentation.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitTypeUMPresentation.id &&
                                                                                                                                             fod.id_metricDestiny == metricUnitPresentation.id)?.factor ?? 0 : 1;
                priceAux = 0;
                if (factorConversion == 0)
                {
                    throw new Exception("Falta el Factor de Conversión entre : " + metricUnitTypeUMPresentation.code + " y " + (metricUnitPresentation.code) + ".Necesario para obtener el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

                }
                else
                {
                    priceAux = priceUMPresentation * factorConversion;
                }
            }
            

            if (list != null)
            {
                PriceListDetail listDetail = list.PriceListDetail.FirstOrDefault(d => d.id_item == item.id);
                var priceListDetail = listDetail?.salePrice;

                if (priceListDetail != null)
                {
                    var detailtPriceList = list?.PriceListDetail.FirstOrDefault(fod => fod.id_item == item.id);
                    var metricUnitPriceList = detailtPriceList?.MetricUnit;

                    if (metricUnitPriceList != null)
                    {
                        var metricUnitSale = item.ItemSaleInformation.MetricUnit;

                        var metricUnitAux = metricUnitPriceList;
                        decimal priceMetricUnitPresentation = detailtPriceList.salePrice;

                        if (metricUnitPriceList.id_metricType == metricUnitSale.id_metricType)
                        {
                            metricUnitAux = item.Presentation?.MetricUnit;

                            //metricUnitDetail = metricUnitTypeUMPresentation;

                            var factorConversionAux = (metricUnitSale.id != metricUnitPriceList.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitSale.id &&
                                                                                                                                             fod.id_metricDestiny == metricUnitPriceList.id)?.factor ?? 0 : 1;
                            priceAux = 0;
                            if (factorConversionAux == 0)
                            {
                                throw new Exception("Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

                            }
                            else
                            {
                                priceMetricUnitPresentation = (priceMetricUnitPresentation * factorConversionAux) / (item.Presentation?.minimum ?? 1);
                            }

                            //priceMetricUnitPresentation = detailtPriceList.salePrice / (item.Presentation?.minimum ?? 1);
                        }

                        var metricUnitDetail = metricUnitTypeUMPresentation;

                        var factorConversion = (metricUnitDetail.id != metricUnitAux.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitDetail.id &&
                                                                                                                                         fod.id_metricDestiny == metricUnitAux.id)?.factor ?? 0 : 1;
                        priceAux = 0;
                        if (factorConversion == 0)
                        {
                            throw new Exception("Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

                        }
                        else
                        {
                            priceAux = priceMetricUnitPresentation * factorConversion;
                        }
                    }
                    else
                    {
                        priceAux = 0;
                    }

                }
                else
                {
                    priceAux = 0;
                }
            }

            return priceAux;
        }

        private decimal SaleDetailIVA(int id_item, decimal quantity, decimal price)
        {
            decimal iva = 0.0M;

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return 0.0M;
            }

            List<ItemTaxation> taxations = item.ItemTaxation.Where(w => w.TaxType.code.Equals("2")).ToList();//"2" Es el Impuesto de IVA

            foreach (var taxation in taxations)
            {
                iva += quantity * price * taxation.Rate.percentage / 100.0M;
            }

            return iva;
        }

        private SalesQuotation Copy(SalesQuotation quotation, SalesQuotation quotation2)
        {
            quotation2 = new SalesQuotation()
            {
                id = 0,
                id_customer = quotation.id_customer,
                Customer = quotation.Customer,
                id_employeeSeller = quotation.id_employeeSeller,
                Employee = quotation.Employee,
                id_priceList = quotation.id_priceList,
                PriceList = quotation.PriceList,
            };
            foreach (var detail in quotation.SalesQuotationDetail)
            {
                if (detail.Item != null && detail.isActive)
                {
                    var tempItemSQDetail = new SalesQuotationDetail
                    {
                        id = detail.id,
                        id_item = detail.id_item,
                        Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                        quantity = detail.quantity,
                        quantityTypeUMPresentation = detail.quantityTypeUMPresentation,
                        id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation,
                        MetricUnit = detail.MetricUnit,

                        price = detail.price,
                        iva = detail.iva,

                        subtotal = detail.subtotal,
                        total = detail.total,

                        isActive = detail.isActive,
                        id_userCreate = detail.id_userCreate,
                        dateCreate = detail.dateCreate,
                        id_userUpdate = detail.id_userUpdate,
                        dateUpdate = detail.dateUpdate

                    };
                    quotation2.SalesQuotationDetail.Add(tempItemSQDetail);
                }

            }
            return quotation2;
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateSalesQuotation2()
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);

            SalesQuotation quotation2 = (TempData["quotation2"] as SalesQuotation);

            quotation2 = Copy(quotation, quotation2);

            var result = new
            {
                Message = "OK"
            };

            TempData["quotation2"] = quotation2;
            TempData.Keep("quotation2");

            TempData["quotation"] = quotation;
            TempData.Keep("quotation");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPriceList(int? id_customer)
        {
            //BusinessOportunity businessOportunity = (TempData["businessOportunity"] as BusinessOportunity);
            //businessOportunity = businessOportunity ?? new BusinessOportunity();
            TempData.Keep("quotation");
            var salesQuotationAux = id_customer == null ? new SalesQuotation() : new SalesQuotation { id_customer = id_customer.Value };
            return PartialView("Component/_ComboBoxPriceList", salesQuotationAux);
            //return GridViewExtension.GetComboBoxCallbackResult(p => {
            //    p.DataSource = DataProviderPerson.PersonByCompanyDocumentTypeOportunityAndCurrent(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", null);
            //    p.ValueField = "id";
            //    p.TextField = "fullname_businessName";
            //    p.CallbackPageSize = 15;
            //});
        }

        [HttpPost]
        public ActionResult GetItem(int? id_item)
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);
            quotation = quotation ?? new SalesQuotation();
            TempData.Keep("quotation");

            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_person";
                p.ClientInstanceName = "id_item";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "SalesQuotation", Action = "GetItem" };
                p.CallbackPageSize = 5;
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.ClientSideEvents.BeginCallback = "SalesQuotationItem_BeginCallback";
                p.ClientSideEvents.EndCallback = "SalesQuotationItem_EndCallback";

                p.DataSource = DataProviderItem.SalesItemsPTByCompanyAndCurrent(this.ActiveCompanyId, id_item);
                p.ValueField = "id";
                //p.TextField = "name";
                p.TextFormatString = "{1}";
                p.ValueType = typeof(int);
                
                p.Columns.Add("masterCode", "Código", 70);
                p.Columns.Add("name", "Nombre del Producto", 300);
                p.Columns.Add("ItemSaleInformation.MetricUnit.code", "UM", 50);
                //p.ClientSideEvents.Init = "ItemCombo_OnInit";
                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
                p.ClientSideEvents.SelectedIndexChanged = "ItemCombo_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnItemValidation";

                //p.TextField = textField;
                //p.BindList(DataProviderPerson.RolByCompanyAndCurrentDistinctInBusinessOportunityCompetition(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_competitor, "Competidor", businessOportunity.BusinessOportunityCompetition.ToList()));//.Bind(id_person);

            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        [HttpPost]
        public ActionResult GetMetricUnitTypeUMPresentation(int? id_item, int? id_metricUnitTypeUMPresentation)
        {
            SalesQuotation quotation = (TempData["quotation"] as SalesQuotation);
            quotation = quotation ?? new SalesQuotation();
            TempData.Keep("quotation");

            return  GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_person";
                p.ClientInstanceName = "id_metricUnitTypeUMPresentation";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "SalesQuotation", Action = "GetMetricUnitTypeUMPresentation" };
                p.CallbackPageSize = 5;
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.ClientSideEvents.BeginCallback = "SalesQuotationMetricUnitTypeUMPresentation_BeginCallback";
                p.ClientSideEvents.EndCallback = "SalesQuotationMetricUnitTypeUMPresentation_EndCallback";

                p.DataSource = DataProviderMetricUnit.MetricUnitTypeUMPresentation(this.ActiveCompanyId, id_item, id_metricUnitTypeUMPresentation);
                p.ValueField = "id";
                p.TextField = "code";
                p.Width = Unit.Percentage(100);
                p.ValueType = typeof(int);
                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                p.ClientSideEvents.SelectedIndexChanged = "MetricUnitTypeUMPresentationCombo_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnMetricUnitTypeUMPresentationValidation";

                //p.BindList(DataProviderPerson.RolByCompanyAndCurrentDistinctInBusinessOportunityCompetition(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_competitor, "Competidor", businessOportunity.BusinessOportunityCompetition.ToList()));//.Bind(id_person);

            });

        }
        #endregion

    }

}
