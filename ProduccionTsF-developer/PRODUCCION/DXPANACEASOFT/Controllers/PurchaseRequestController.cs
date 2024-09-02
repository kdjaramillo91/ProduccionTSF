using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class PurchaseRequestController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region PURCHASE REQUEST FILTERS RESULTS

        [HttpPost]
        public ActionResult PurchaseRequestResults(PurchaseRequest purchaseRequest,
                                                   Document document,
                                                   DateTime? startEmissionDate, DateTime? endEmissionDate,
                                                   DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                                   String PurcharFilter,
                                                   int[] items)
        {

            List<PurchaseRequest> model = db.PurchaseRequest.AsEnumerable().ToList();

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

            if (startEmissionDate != null && endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0 && DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            if (startAuthorizationDate != null && endAuthorizationDate != null)
            {
                model = model.Where(o => o.Document.authorizationDate != null && DateTime.Compare(startAuthorizationDate.Value.Date, o.Document.authorizationDate.Value.Date) <= 0 && DateTime.Compare(o.Document.authorizationDate.Value.Date, endAuthorizationDate.Value.Date) <= 0).ToList();
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

            #region PURCHASE ORDER FILTERS
            
            if (purchaseRequest.id_personRequesting != 0)
            {
                model = model.Where(o => o.id_personRequesting == purchaseRequest.id_personRequesting).ToList();
            }

            if (items != null && items.Length > 0)
            {
                var tempModel = new List<PurchaseRequest>();
                foreach (var request in model)
                {
                    var details = request.PurchaseRequestDetail.Where(d => items.Contains(d.id_item));
                    if (details.Any())
                    {
                        tempModel.Add(request);
                    }
                }

                model = tempModel;
            }
            
            if(PurcharFilter !=null)
            {
                switch (PurcharFilter)
                {
                    case "01":
                        //                REQUERIMIENTOS TOTALMENTE ATENDIDOS.- Pendiente de compra es 0

                        var FILTER = model.Where(i => i.PurchaseRequestDetail.Any(X => X.quantityOutstandingPurchase <= 0 &&  X.quantityApproved >0)).ToList();
                                     
                        if (FILTER != null)
                        {
                            var listId = (from i in FILTER
                                          select i.id).ToList();
                            model = model.Where(o => listId.Contains(o.id)).ToList();

                        }


                        
                        break;
                    case "02":
                        //                 REQUERIMIENTOS PARCIALMENTE ATENDIDOS.- Pendiente de compra es diferente de compra autorizada

                        var FILTER02 = model.Where(i => i.PurchaseRequestDetail.Any(X => X.quantityOutstandingPurchase < X.quantityApproved && X.quantityOutstandingPurchase>0) ).ToList();

                        if (FILTER02 != null)
                        {
                            var listId = (from i in FILTER02
                                          select i.id).ToList();
                            model = model.Where(o => listId.Contains(o.id)).ToList();

                        }
                        break;

                    case "03":
                        //                 REQUERIMIENTOS SIN ATENDER.- Pendiente de compra es igual a compra autorizada

                        var FILTER03= model.Where(i => i.PurchaseRequestDetail.Any(X => X.quantityOutstandingPurchase == X.quantityApproved && X.quantityApproved >0 && X.quantityOutstandingPurchase>0)).ToList();

                        if (FILTER03 != null)
                        {
                            var listId = (from i in FILTER03
                                          select i.id).ToList();
                            model = model.Where(o => listId.Contains(o.id)).ToList();

                        }


                        break;
                }
            }
            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_PurchaseRequestResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        #endregion

        #region PURCHASE REQUEST MASTER DETAILS

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseRequestResultsDetailPartial()
        {
            int id_purchaseRequest = (Request.Params["id_purchaseRequest"] != null && Request.Params["id_purchaseRequest"] != "") ? int.Parse(Request.Params["id_purchaseRequest"]) : -1;
            PurchaseRequest request = db.PurchaseRequest.FirstOrDefault(r => r.id == id_purchaseRequest);
            return PartialView("_PurchaseRequestResultsDetailPartial", request.PurchaseRequestDetail.ToList());
        }

        #endregion

        #region PURCHASE REQUEST EDITFORM
        
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditPurchaseRequest(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

            if (purchaseRequest == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("01"));//Requerimiento de Compras
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");
                //DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.id == 1);

                Employee employee = ActiveUser.Employee;

                purchaseRequest = new PurchaseRequest
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
                    id_personRequesting = employee?.id ?? 0,
                    Employee = employee
                };
            }

            TempData["request"] = purchaseRequest;
            TempData.Keep("request");

            return PartialView("_PurchaseRequestEditFormPartial", purchaseRequest);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseRequestCopy(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequest.AsEnumerable().FirstOrDefault(r => r.id == id);

            DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("01"));
            DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.id == 1);

            User user = db.User.FirstOrDefault(u => u.id == ActiveUser.id);
            Employee employee = db.Employee.FirstOrDefault(e => e.id == ActiveUser.id);

            PurchaseRequest purchaseRequestCopy = null;
            if(purchaseRequest != null)
            {
                purchaseRequestCopy = new PurchaseRequest {
                    Document = new Document
                    {
                        id = 0,
                        id_documentType = documentType?.id ?? 0,
                        DocumentType = documentType,
                        id_documentState = documentState?.id ?? 0,
                        DocumentState = documentState,
                        emissionDate = DateTime.Now
                    },
                    id_personRequesting = employee?.id ?? 0,
                    Employee = employee,
                    PurchaseRequestDetail = new List<PurchaseRequestDetail>()
                };

                foreach (var detail in purchaseRequest.PurchaseRequestDetail)
                {
                    purchaseRequestCopy.PurchaseRequestDetail.Add(new PurchaseRequestDetail
                    {
                        id_item = detail.id_item,
                        id_proposedProvider = detail.id_proposedProvider,
                        quantityRequested = detail.quantityRequested,
                        quantityOutstandingPurchase = detail.quantityRequested,//detail.quantityOutstandingPurchase,
                        quantityApproved = detail.quantityApproved,

                        isActive = detail.isActive,
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now
                    });
                }
            }

            TempData["request"] = purchaseRequestCopy;
            TempData.Keep("request");

            return PartialView("_PurchaseRequestEditFormPartial", purchaseRequestCopy);
        }

        #endregion

        #region PURCHASE REQUEST HEADER

        [ValidateInput(false)]
        public ActionResult PurchaseRequestsPartial()
        {
            var model = (TempData["model"] as List<PurchaseRequest>);
            model = model ?? new List<PurchaseRequest>();

            TempData.Keep("model");

            return PartialView("_PurchaseRequestsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseRequestsPartialAddNew(bool approve, PurchaseRequest itemPR, Document itemDoc)
        {
            PurchaseRequest tempRequest = (TempData["request"] as PurchaseRequest);

            string respuestaFinal = string.Empty;
            respuestaFinal = approve == true ? "aprobado" : "guardado";

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
                    itemPR.Employee = employee;

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


                    #endregion

                    #region PurchaseRequest

                    //itemPR.Employee.id = itemPR.id_personRequesting;
                    itemPR.Document = itemDoc;

                    var employeeAux = db.Employee.FirstOrDefault(fod => fod.id == itemPR.id_personRequesting);
                    itemPR.Employee = employeeAux;

                    #endregion

                    #region DETAILS

                    if (tempRequest?.PurchaseRequestDetail != null)
                    {
                        itemPR.PurchaseRequestDetail = new List<PurchaseRequestDetail>();
                        var itemPRDetail = tempRequest.PurchaseRequestDetail.ToList();
                        
                        foreach (var detail in itemPRDetail)
                        {
                            var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                            if (approve && detail.quantityApproved <= 0)
                            {
                                TempData.Keep("request");
                                ViewData["EditMessage"] = ErrorMessage("No se puede aprobar el requerimiento de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada mayor que cero.");
                                return PartialView("_PurchaseRequestMainFormPartial", tempRequest);
                            }
                            var tempItemPRDetail = new PurchaseRequestDetail
                            {
                                id_item = detail.id_item,
                                id_proposedProvider = detail.id_proposedProvider,
                                proposedPrice = detail.proposedPrice,

                                colorReference = detail.colorReference,
                                id_grammageFrom = detail.id_grammageFrom,
                                id_grammageTo = detail.id_grammageTo,

                                quantityRequested = detail.quantityRequested,
                                quantityApproved = detail.quantityApproved,
                                quantityOutstandingPurchase = detail.quantityOutstandingPurchase,

                                isActive = detail.isActive,
                                id_userCreate = detail.id_userCreate,
                                dateCreate = detail.dateCreate,
                                id_userUpdate = detail.id_userUpdate,
                                dateUpdate = detail.dateUpdate
                            };

                            if (tempItemPRDetail.isActive)
                                itemPR.PurchaseRequestDetail.Add(tempItemPRDetail);
                        }
                    }

                    #endregion

                    if (itemPR.PurchaseRequestDetail.Count == 0)
                    {
                        TempData.Keep("request");
                        ViewData["EditMessage"] = ErrorMessage("No se puede guardar un requerimiento sin detalles");
                        return PartialView("_PurchaseRequestMainFormPartial", tempRequest);
                    }

                    if (approve)
                    {
                        itemPR.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                    }

                    db.PurchaseRequest.Add(itemPR);
                    db.SaveChanges();
                    trans.Commit();

                    TempData["request"] = itemPR;
                    TempData.Keep("request");

                    ViewData["EditMessage"] = SuccessMessage("Requerimiento: " + itemPR.Document.number + " " + respuestaFinal + " exitosamente");
                }
                catch (Exception)
                {
                    TempData.Keep("request");
                    ViewData["EditMessage"] = ErrorMessage();
                    trans.Rollback();

                    return PartialView("_PurchaseRequestMainFormPartial", tempRequest);
                }
            }

            return PartialView("_PurchaseRequestMainFormPartial", itemPR);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseRequestsPartialUpdate(bool approve, PurchaseRequest itemPR, Document itemDoc)
        {
            var model = db.PurchaseRequest;
            
            PurchaseRequest modelItem = null;
            string respuestaFinal = string.Empty;
            respuestaFinal = approve == true ? "aprobado" : "guardado";
            
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    modelItem = model.FirstOrDefault(it => it.id == itemPR.id);
                    if (modelItem != null)
                    {
                        PurchaseRequest tempRequest = (TempData["request"] as PurchaseRequest);

                        #region DOCUMENT

                        modelItem.Document.emissionDate = itemDoc.emissionDate;
                        modelItem.Document.description = itemDoc.description;
                        modelItem.Document.id_userUpdate = ActiveUser.id;
                        modelItem.Document.dateUpdate = DateTime.Now;

                        #endregion

                        #region PURCHASE REQUEST

                        //itemPR.Employee.id = itemPR.id_personRequesting;
                        var employeeAux = db.Employee.FirstOrDefault(fod => fod.id == itemPR.id_personRequesting);
                        modelItem.id_personRequesting = employeeAux != null ? itemPR.id_personRequesting : modelItem.id_personRequesting;
                        modelItem.Employee = employeeAux != null ? employeeAux : modelItem.Employee;


                        #endregion

                        #region PURCHASE REQUEST DETAILS

                        int count = 0;
                        if (tempRequest?.PurchaseRequestDetail != null)
                        {
                            var details = tempRequest.PurchaseRequestDetail.ToList();

                            foreach (var detail in details)
                            {
                                PurchaseRequestDetail requestDetail = modelItem.PurchaseRequestDetail.FirstOrDefault(d => d.id == detail.id);
                                var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                                if (approve && detail.quantityApproved <= 0)
                                {
                                    TempData.Keep("request");
                                    ViewData["EditMessage"] = ErrorMessage("No se puede aprobar el requerimiento de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada mayor que cero.");
                                    return PartialView("_PurchaseRequestMainFormPartial", tempRequest);
                                }
                                if (requestDetail == null)
                                {
                                    requestDetail = new PurchaseRequestDetail
                                    {
                                        id_item = detail.id_item,
                                        id_proposedProvider = detail.id_proposedProvider,
                                        proposedPrice = detail.proposedPrice,

                                        quantityRequested = detail.quantityRequested,
                                        quantityApproved = detail.quantityApproved,
                                        quantityOutstandingPurchase = detail.quantityOutstandingPurchase,
                                        //id_itemColor = detail.id_itemColor,
                                        //id_grammage = detail.id_grammage,
                                        colorReference = detail.colorReference,
                                        id_grammageFrom = detail.id_grammageFrom,
                                        id_grammageTo = detail.id_grammageTo,

                                        isActive = detail.isActive,
                                        id_userCreate = detail.id_userCreate,
                                        dateCreate = detail.dateCreate,
                                        id_userUpdate = detail.id_userUpdate,
                                        dateUpdate = detail.dateUpdate
                                    };

                                    if (requestDetail.isActive)
                                    {
                                        modelItem.PurchaseRequestDetail.Add(requestDetail);
                                        count++;
                                    }
                                        
                                }
                                else
                                {
                                    requestDetail.id_item = detail.id_item;
                                    requestDetail.id_proposedProvider = detail.id_proposedProvider;
                                    requestDetail.proposedPrice = detail.proposedPrice;
                                    //requestDetail.id_itemColor = detail.id_itemColor;
                                    //requestDetail.id_grammage = detail.id_grammage;
                                    requestDetail.quantityRequested = detail.quantityRequested;
                                    requestDetail.quantityApproved = detail.quantityApproved;
                                    requestDetail.quantityOutstandingPurchase = detail.quantityOutstandingPurchase;
                                    requestDetail.id_grammageFrom = detail.id_grammageFrom;
                                    requestDetail.id_grammageTo = detail.id_grammageTo;

                                    requestDetail.isActive = detail.isActive;
                                    requestDetail.id_userCreate = detail.id_userCreate;
                                    requestDetail.dateCreate = detail.dateCreate;
                                    requestDetail.id_userUpdate = detail.id_userUpdate;
                                    requestDetail.dateUpdate = detail.dateUpdate;

                                    if (requestDetail.isActive)
                                        count++;
                                }
                            }
                        }
                        #endregion

                        if (count == 0)
                        {
                            TempData.Keep("request");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar un requerimiento sin detalles");
                            return PartialView("_PurchaseRequestMainFormPartial", tempRequest);
                        }

                        if (approve)
                        {

                            modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA

                        }

                        db.PurchaseRequest.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        //itemPR = modelItem;
                        TempData["request"] = modelItem;
                        TempData.Keep("request");


                        ViewData["EditMessage"] = SuccessMessage("Requerimiento: " + modelItem.Document.number + " " + respuestaFinal + " exitosamente");

                    }
                }
                catch (Exception)
                {
                    TempData.Keep("request");
                    ViewData["EditMessage"] = ErrorMessage();
                    trans.Rollback();
                }
            }

            return PartialView("_PurchaseRequestMainFormPartial", modelItem);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseRequestsPartialDelete(System.Int32 id)
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
            return PartialView("_PurchaseRequestsPartial", model.ToList());
        }

        #endregion

        #region PURCHASE REQUEST DETAILS

        [ValidateInput(false)]
        public ActionResult PurchaseRequestDetails()
        {
            PurchaseRequest request = (TempData["request"] as PurchaseRequest);

            request = request ?? db.PurchaseRequest.FirstOrDefault(i => i.id == request.id);
            request = request ?? new PurchaseRequest();

            var model = request?.PurchaseRequestDetail.Where(d => d.isActive).ToList() ?? new List<PurchaseRequestDetail>();

            TempData.Keep("request");

            return PartialView("_PurchaseRequestEditFormDetailPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult PurchaseRequestDetailsAddNew(PurchaseRequestDetail requestDetail)
        {
            PurchaseRequest request = (TempData["request"] as PurchaseRequest);

            request = request ?? db.PurchaseRequest.FirstOrDefault(i => i.id == request.id);
            request = request ?? new PurchaseRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    requestDetail.id = request.PurchaseRequestDetail.Count() > 0 ? request.PurchaseRequestDetail.Max(pld => pld.id) + 1 : 1;

                    requestDetail.Item = db.Item.FirstOrDefault(i => i.id == requestDetail.id_item);

                    requestDetail.Provider = db.Provider.FirstOrDefault(p => p.id == requestDetail.id_proposedProvider);

                    requestDetail.isActive = true;
                    requestDetail.id_userCreate = ActiveUser.id;
                    requestDetail.dateCreate = DateTime.Now;
                    requestDetail.id_userUpdate = ActiveUser.id;
                    requestDetail.dateUpdate = DateTime.Now;

                    request.PurchaseRequestDetail.Add(requestDetail);

                    TempData["request"] = request;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else 
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("request");

            var model = request?.PurchaseRequestDetail.Where(d => d.isActive).ToList() ?? new List<PurchaseRequestDetail>();
            return PartialView("_PurchaseRequestEditFormDetailPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult PurchaseRequestDetailsUpdate(PurchaseRequestDetail requestDetail)
        {
            PurchaseRequest request = (TempData["request"] as PurchaseRequest);

            request = request ?? db.PurchaseRequest.FirstOrDefault(i => i.id == request.id);
            request = request ?? new PurchaseRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    //PurchaseRequestDetail detail = request.PurchaseRequestDetail.FirstOrDefault(r => r.id_item == requestDetail.id_item);
                    var detail = request.PurchaseRequestDetail.FirstOrDefault(it => it.id == requestDetail.id);

                    if (detail != null)
                    {
                        detail.id_item = requestDetail.id_item;
                        detail.Item = db.Item.FirstOrDefault(i => i.id == requestDetail.id_item);

                        detail.id_proposedProvider = requestDetail.id_proposedProvider;
                        detail.Provider = db.Provider.FirstOrDefault(p => p.id == requestDetail.id_proposedProvider);

                        detail.proposedPrice = requestDetail.proposedPrice;

                        detail.quantityRequested = requestDetail.quantityRequested;
                        detail.quantityApproved = requestDetail.quantityApproved;
                        detail.quantityOutstandingPurchase = requestDetail.quantityOutstandingPurchase;

                        detail.colorReference = requestDetail.colorReference;
                        detail.id_grammageFrom = requestDetail.id_grammageFrom;
                        detail.id_grammageTo = requestDetail.id_grammageTo;

                        requestDetail.id_userUpdate = ActiveUser.id;
                        requestDetail.dateCreate = DateTime.Now;

                        this.UpdateModel(detail);
                        TempData["request"] = request;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";
           
            TempData.Keep("request");

            var model = request?.PurchaseRequestDetail.Where(d => d.isActive).ToList() ?? new List<PurchaseRequestDetail>();
            return PartialView("_PurchaseRequestEditFormDetailPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult PurchaseRequestDetailsDelete(int id)//int id_item)
        {
            PurchaseRequest request = (TempData["request"] as PurchaseRequest);

            request = request ?? db.PurchaseRequest.FirstOrDefault(i => i.id == request.id);
            request = request ?? new PurchaseRequest();

            //if (id_item >= 0)
            //{
                try
                {
                    //var requestDetail = request.PurchaseRequestDetail.FirstOrDefault(p => p.id_item == id_item);
                    var requestDetail = request.PurchaseRequestDetail.FirstOrDefault(p => p.id == id);

                    if (requestDetail != null)
                    {
                        requestDetail.isActive = false;
                        requestDetail.id_userUpdate = ActiveUser.id;
                        requestDetail.dateCreate = DateTime.Now;
                    }

                    TempData["request"] = request;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            //}

            TempData.Keep("request");

            var model = request?.PurchaseRequestDetail.Where(d => d.isActive).ToList() ?? new List<PurchaseRequestDetail>();
            return PartialView("_PurchaseRequestEditFormDetailPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult PurchaseRequestDetailsDeleteSelectedItems(int[] itemIds)
        {
            PurchaseRequest request = (TempData["request"] as PurchaseRequest);

            request = request ?? db.PurchaseRequest.FirstOrDefault(i => i.id == request.id);
            request = request ?? new PurchaseRequest();

            if (itemIds != null && itemIds.Length > 0)
            {
                try
                {
                    //var requestDetail = request.PurchaseRequestDetail.Where(i => itemIds.Contains(i.id_item));
                    var requestDetail = request.PurchaseRequestDetail.Where(i => itemIds.Contains(i.id));

                    foreach (var item in requestDetail)
                    {
                        item.isActive = false;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;
                    }

                    TempData["request"] = request;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("request");

            var model = request?.PurchaseRequestDetail.Where(d => d.isActive).ToList() ?? new List<PurchaseRequestDetail>();
            return PartialView("_PurchaseRequestEditFormDetailPartial", model.ToList());
        }

        #endregion

        #region DETAILS VIEW
        public ActionResult PurchaseRequestDetailViewDetailsPartial(int? id_purchaseRequest)
        {
            ViewData["id_purchaseRequest"] = id_purchaseRequest;
            var purchaseRequest = db.PurchaseRequest.FirstOrDefault(p => p.id == id_purchaseRequest);
            var model = purchaseRequest?.PurchaseRequestDetail.ToList() ?? new List<PurchaseRequestDetail>();
            
            return PartialView("_PurchaseRequestDetailViewDetailsPartial", model.ToList());
        }
        #endregion

        #region SINGLE CHANGE DOCUMENT STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                    if (purchaseRequest != null && documentState != null)
                    {
                        purchaseRequest.Document.id_documentState = documentState.id;
                        purchaseRequest.Document.DocumentState = documentState;

                        foreach (var details in purchaseRequest.PurchaseRequestDetail)
                        {
                            details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;
                            details.quantityOutstandingPurchase = (details.quantityOutstandingPurchase > 0) ? details.quantityOutstandingPurchase : details.quantityRequested;

                            db.PurchaseRequestDetail.Attach(details);
                            db.Entry(details).State = EntityState.Modified;
                        }

                        db.PurchaseRequest.Attach(purchaseRequest);
                        db.Entry(purchaseRequest).State = EntityState.Modified;

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

            TempData["request"] = purchaseRequest;
            TempData.Keep("request");

            return PartialView("_PurchaseRequestMainFormPartial", purchaseRequest);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada

                    if (purchaseRequest != null && documentState != null)
                    {
                        purchaseRequest.Document.id_documentState = documentState.id;
                        purchaseRequest.Document.DocumentState = documentState;

                        //foreach (var details in purchaseRequest.PurchaseRequestDetail)
                        //{
                        //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;
                        //    details.quantityOutstandingPurchase = (details.quantityOutstandingPurchase > 0) ? details.quantityOutstandingPurchase : details.quantityRequested;

                        //    db.PurchaseRequestDetail.Attach(details);
                        //    db.Entry(details).State = EntityState.Modified;
                        //}

                        db.PurchaseRequest.Attach(purchaseRequest);
                        db.Entry(purchaseRequest).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["request"] = purchaseRequest;
                        TempData.Keep("request");

                        ViewData["EditMessage"] = SuccessMessage("Requerimiento de Compra: " + purchaseRequest.Document.number + " autorizado exitosamente");

                    }
                }
                catch (Exception)
                {
                    //ViewData["EditError"] = e.Message;
                    TempData.Keep("request");
                    ViewData["EditError"] = ErrorMessage();
                    trans.Rollback();
                }
            }

           

            return PartialView("_PurchaseRequestMainFormPartial", purchaseRequest);
        }

        [HttpPost]
        public ActionResult Protect(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "04"); //Cerrada

                    if (purchaseRequest != null && documentState != null)
                    {
                        purchaseRequest.Document.id_documentState = documentState.id;
                        purchaseRequest.Document.DocumentState = documentState;

                        db.PurchaseRequest.Attach(purchaseRequest);
                        db.Entry(purchaseRequest).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["request"] = purchaseRequest;
                        TempData.Keep("request");

                        ViewData["EditMessage"] = SuccessMessage("Requerimiento de Compra: " + purchaseRequest.Document.number + " cerrado exitosamente");
                    }
                }
                catch (Exception e)
                {
                    //ViewData["EditError"] = e.Message;
                    TempData.Keep("request");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            //TempData["request"] = purchaseRequest;
            //TempData.Keep("request");

            return PartialView("_PurchaseRequestMainFormPartial", purchaseRequest);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var existInPurchaseOrderDetailPurchaseRequest = purchaseRequest.PurchaseOrderDetailPurchaseRequest.FirstOrDefault(fod => fod.PurchaseOrderDetail.PurchaseOrder.Document.DocumentState.code != "05");//Diferente de Anulado

                    if (existInPurchaseOrderDetailPurchaseRequest != null)
                    {
                        TempData.Keep("request");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular el requerimiento de compra debido a tener algun detalle relacionado con una Orden de Compra.");
                        return PartialView("_PurchaseRequestMainFormPartial", purchaseRequest);
                    }


                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (purchaseRequest != null && documentState != null)
                    {
                        purchaseRequest.Document.id_documentState = documentState.id;
                        purchaseRequest.Document.DocumentState = documentState;

                        db.PurchaseRequest.Attach(purchaseRequest);
                        db.Entry(purchaseRequest).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["request"] = purchaseRequest;
                        TempData.Keep("request");

                        ViewData["EditMessage"] = SuccessMessage("Requerimiento de Compra: " + purchaseRequest.Document.number + " anulado exitosamente");
                    }
                }
                catch (Exception)
                {
                    //ViewData["EditError"] = e.Message;
                    TempData.Keep("request");
                    ViewData["EditError"] = ErrorMessage();
                    trans.Rollback();
                }
            }

            //TempData["request"] = purchaseRequest;
            //TempData.Keep("request");

            return PartialView("_PurchaseRequestMainFormPartial", purchaseRequest);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var existInPurchaseOrderDetailPurchaseRequest = purchaseRequest.PurchaseOrderDetailPurchaseRequest.FirstOrDefault(fod => fod.PurchaseOrderDetail.PurchaseOrder.Document.DocumentState.code != "05");//Diferente de Anulado

                    if (existInPurchaseOrderDetailPurchaseRequest != null)
                    {
                        TempData.Keep("request");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar el requerimiento de compra debido a tener algun detalle relacionado con una Orden de Compra.");
                        return PartialView("_PurchaseRequestMainFormPartial", purchaseRequest);
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

                    if (purchaseRequest != null && documentState != null)
                    {
                        purchaseRequest.Document.id_documentState = documentState.id;
                        purchaseRequest.Document.DocumentState = documentState;

                        //foreach (var details in purchaseRequest.PurchaseRequestDetail)
                        //{
                        //    details.quantityApproved = 0.0M;

                        //    db.PurchaseRequestDetail.Attach(details);
                        //    db.Entry(details).State = EntityState.Modified;
                        //}

                        db.PurchaseRequest.Attach(purchaseRequest);
                        db.Entry(purchaseRequest).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["request"] = purchaseRequest;
                        TempData.Keep("request");

                        ViewData["EditMessage"] = SuccessMessage("Requerimiento de Compra: " + purchaseRequest.Document.number + " reversado exitosamente");
                    }
                }
                catch (Exception)
                {
                    //ViewData["EditError"] = e.Message;
                    TempData.Keep("request");
                    ViewData["EditError"] = ErrorMessage();
                    trans.Rollback();
                }
            }

            //TempData["request"] = purchaseRequest;
            //TempData.Keep("request");

            return PartialView("_PurchaseRequestMainFormPartial", purchaseRequest);
        }

        #endregion

        #region SELECTED DOCUMENT STATE CHANGE 

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
                            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                            if (purchaseRequest != null && documentState != null)
                            {
                                purchaseRequest.Document.id_documentState = documentState.id;
                                purchaseRequest.Document.DocumentState = documentState;

                                foreach (var details in purchaseRequest.PurchaseRequestDetail)
                                {
                                    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;
                                    details.quantityOutstandingPurchase = (details.quantityOutstandingPurchase > 0) ? details.quantityOutstandingPurchase : details.quantityRequested;

                                    db.PurchaseRequestDetail.Attach(details);
                                    db.Entry(details).State = EntityState.Modified;
                                }

                                db.PurchaseRequest.Attach(purchaseRequest);
                                db.Entry(purchaseRequest).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseRequest>);
            model = model ?? new List<PurchaseRequest>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

                            if (purchaseRequest != null && documentState != null)
                            {
                                purchaseRequest.Document.id_documentState = documentState.id;
                                purchaseRequest.Document.DocumentState = documentState;

                                foreach (var details in purchaseRequest.PurchaseRequestDetail)
                                {
                                    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;
                                    details.quantityOutstandingPurchase = (details.quantityOutstandingPurchase > 0) ? details.quantityOutstandingPurchase : details.quantityRequested;

                                    db.PurchaseRequestDetail.Attach(details);
                                    db.Entry(details).State = EntityState.Modified;
                                }

                                db.PurchaseRequest.Attach(purchaseRequest);
                                db.Entry(purchaseRequest).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseRequest>);
            model = model ?? new List<PurchaseRequest>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                            if (purchaseRequest != null && documentState != null)
                            {
                                purchaseRequest.Document.id_documentState = documentState.id;
                                purchaseRequest.Document.DocumentState = documentState;

                                db.PurchaseRequest.Attach(purchaseRequest);
                                db.Entry(purchaseRequest).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseRequest>);
            model = model ?? new List<PurchaseRequest>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

                            if (purchaseRequest != null && documentState != null)
                            {
                                purchaseRequest.Document.id_documentState = documentState.id;
                                purchaseRequest.Document.DocumentState = documentState;

                                db.PurchaseRequest.Attach(purchaseRequest);
                                db.Entry(purchaseRequest).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseRequest>);
            model = model ?? new List<PurchaseRequest>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                            if (purchaseRequest != null && documentState != null)
                            {
                                purchaseRequest.Document.id_documentState = documentState.id;
                                purchaseRequest.Document.DocumentState = documentState;

                                foreach (var details in purchaseRequest.PurchaseRequestDetail)
                                {
                                    details.quantityApproved = 0.0M;

                                    db.PurchaseRequestDetail.Attach(details);
                                    db.Entry(details).State = EntityState.Modified;
                                }

                                db.PurchaseRequest.Attach(purchaseRequest);
                                db.Entry(purchaseRequest).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseRequest>);
            model = model ?? new List<PurchaseRequest>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model.ToList();
            TempData.Keep("model");
        }

        #endregion

        #region PURCHASE REQUEST REPORTS

        [HttpPost, ValidateInput(false)]
        public JsonResult PurchaseRequestReport(int idPurchaseRequest)
        {

            #region Armo parámetros

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = idPurchaseRequest,
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "REQCMP";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            #endregion

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
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

            PurchaseRequest purchaseRequest = db.PurchaseRequest.FirstOrDefault(r => r.id == id);
            string state = purchaseRequest.Document.DocumentState.code;
            //int state = purchaseRequest.Document.DocumentState.id;

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
            else if (state == "03" )//|| state == 3) // APROBADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = true,
                    btnProtect = false,
                    btnCancel = false,
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
                var purchaseRequestDetailAux = purchaseRequest.PurchaseRequestDetail.Where(w=> w.isActive = true).FirstOrDefault(fod => fod.quantityOutstandingPurchase > 0);
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    
                    btnProtect = purchaseRequestDetailAux != null,// true,
                    btnCancel = false,
                    btnRevert = true,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_purchaseRequest)
        {
            TempData.Keep("request");

            int index = db.PurchaseRequest.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_purchaseRequest);

            var result = new
            {
                maximunPages = db.PurchaseRequest.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            PurchaseRequest purchaseRequest = db.PurchaseRequest.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if(purchaseRequest != null)
            {
                TempData["request"] = purchaseRequest;
                TempData.Keep("request");
                return PartialView("_PurchaseRequestMainFormPartial", purchaseRequest);
            }

            TempData.Keep("request");

            return PartialView("_PurchaseRequestMainFormPartial", new PurchaseRequest());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult GetPurchaseRequestDetails()
        {
            PurchaseRequest purchaseRequest = (TempData["request"] as PurchaseRequest);

            purchaseRequest = purchaseRequest ?? new PurchaseRequest();
            purchaseRequest.PurchaseRequestDetail = purchaseRequest.PurchaseRequestDetail ?? new List<PurchaseRequestDetail>();

            TempData.Keep("request");

            return Json(purchaseRequest.PurchaseRequestDetail.Where(d => d.isActive).Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ComboBoxItemProvidersDetailPartial()
        {
            int id_item = (Request.Params["id_item"] != null && Request.Params["id_item"] != "") ? int.Parse(Request.Params["id_item"]) : -1;
            return PartialView("_ComboBoxItemProvidersDetailPartial", DataProviderPerson.ProvidersByItem(id_item));
        }
        

        public ActionResult ComboBoxItemsDetailPartial()
        {
            int id_item = (Request.Params["id_item"] != null && Request.Params["id_item"] != "") ? int.Parse(Request.Params["id_item"]) : -1;
            return PartialView("_ComboBoxItemsDetailPartial", DataProviderItem.PurchaseItems());
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemDetailData(int id_item)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                masterCode = item.masterCode,
                metricUnit = item.ItemPurchaseInformation?.MetricUnit.code ?? ""
            };

            TempData.Keep("request");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItemDetail(int? id_itemNew, int? id_proposedProviderNew)
        {
            PurchaseRequest request = (TempData["request"] as PurchaseRequest);

            request = request ?? new PurchaseRequest();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };


            var purchaseRequestDetailAux = request.PurchaseRequestDetail.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                            fod.id_proposedProvider == id_proposedProviderNew);
            if (purchaseRequestDetailAux != null)
            {
                var itemNewAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var providerAux = db.Provider.FirstOrDefault(fod => fod.id == id_proposedProviderNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Ítem: " + itemNewAux.name +
                            ",  para el mismo proveedor: " + (providerAux?.Person.fullname_businessName ?? "(No asignado)") + ",  en los detalles."

                };

            }



            TempData["request"] = request;
            TempData.Keep("request");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitItemCombo(int? id_item, int? id_proposedProvider)
        {
            PurchaseRequest request = (TempData["request"] as PurchaseRequest);

            request = request ?? new PurchaseRequest();


            var items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.isPurchased) || w.id == id_item).Select(s => new
            {
                id = s.id,
                masterCode = s.masterCode,
                itemPurchaseInformationMetricUnitCode = (s.ItemPurchaseInformation != null) ? s.ItemPurchaseInformation.MetricUnit.code : "",
                name = s.name
            }).ToList();

            var proposedProviders = db.Provider.Where(w => (w.Person.isActive && w.Person.id_company == this.ActiveCompanyId) || w.id == id_proposedProvider).Select(s => new
            {
                id = s.id,
                name = s.Person.fullname_businessName
            }).ToList();

            var result = new
            {
                items = items,
                proposedProviders = proposedProviders

            };

            TempData.Keep("request");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsAllProductionScheduleOK()
        {
            PurchaseRequest request = (TempData["request"] as PurchaseRequest);

            request = request ?? new PurchaseRequest();

            var result = new
            {
                ItsAllProductionScheduleOK = 1,
                Error = ""

            };

            foreach (var detail in request.PurchaseRequestDetail)
            {
                if (detail.ProductionScheduleScheduleDetailPurchaseRequestDetail != null && detail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Count() > 0 &&
                    (detail.ProductionScheduleScheduleDetailPurchaseRequestDetail.FirstOrDefault().id_productionScheduleScheduleDetail == null ||
                     detail.ProductionScheduleScheduleDetailPurchaseRequestDetail.FirstOrDefault().ProductionSchedule.Document.DocumentState.code != "03"))
                {
                    
                    result = new
                    {
                        ItsAllProductionScheduleOK = 0,
                        Error = "No se puede aprobar el requerimiento de compra, debido a que existe Programación de producción con estado no aprobado o se ha perdido la referencia al detalle de planificación correspondiente,  en los detalles del Requerimiento."

                    };

                }
            }




            TempData["request"] = request;
            TempData.Keep("request");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsAllBusinessOportunityOK()
        {
            PurchaseRequest request = (TempData["request"] as PurchaseRequest);

            request = request ?? new PurchaseRequest();

            var result = new
            {
                ItsAllBusinessOportunityOK = 1,
                Error = ""

            };

            foreach (var detail in request.PurchaseRequestDetail)
            {
                if (detail.PurchaseRequestDetailBusinessOportunity != null && detail.PurchaseRequestDetailBusinessOportunity.Count() > 0 &&
                    (detail.PurchaseRequestDetailBusinessOportunity.FirstOrDefault().id_businessOportunityPlanningDetail == null ||
                     detail.PurchaseRequestDetailBusinessOportunity.FirstOrDefault().BusinessOportunity.Document.DocumentState.code != "03"))
                {

                    result = new
                    {
                        ItsAllBusinessOportunityOK = 0,
                        Error = "No se puede aprobar el requerimiento de compra, debido a que existe Oportunidad de Compra con estado no aprobado o se ha perdido la referencia al detalle de planificación correspondiente,  en los detalles del Requerimiento."

                    };

                }
            }




            TempData["request"] = request;
            TempData.Keep("request");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        #endregion

    }
}