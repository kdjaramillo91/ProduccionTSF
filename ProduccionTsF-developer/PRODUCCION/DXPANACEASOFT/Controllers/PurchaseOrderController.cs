using DevExpress.Utils.Extensions;
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Mvc;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentLogDTO;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utilitarios.Logs;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace DXPANACEASOFT.Controllers
{
    public class PurchaseOrderController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            if (TempData["model"] != null)
            {
                TempData.Remove("model");
            }
            return PartialView();
        }

        #region PURCHESE ORDER FILTERS RESULTS

        [HttpPost]
        public ActionResult PurchaseOrderResults(PurchaseOrder purchaseOrder,
                                                 Document document,
                                                 DateTime? startEmissionDate, DateTime? endEmissionDate,
                                                 DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                                 int[] items, int? id_logicalOperator)
        {

            List<PurchaseOrder> model = null;

            tbsysUserRecordSecurity UserRecordSecurity = db.tbsysUserRecordSecurity.FirstOrDefault(r => r.id_user == ActiveUser.id && r.tbsysObjSecurityRecord.obj == "PurchaseOrder");

            if (UserRecordSecurity != null)
            {
                model = db.PurchaseOrder.Where(r => r.id_buyer == ActiveUser.Employee.id).ToList();

            }
            else
            {
                model = db.PurchaseOrder.AsEnumerable().ToList();
            }



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

            if (purchaseOrder.id_provider != 0)
            {
                model = model.Where(o => o.id_provider == purchaseOrder.id_provider).ToList();
            }

            if (purchaseOrder.id_buyer != 0)
            {
                model = model.Where(o => o.id_buyer == purchaseOrder.id_buyer).ToList();
            }

            if (items != null && items.Length > 0)
            {
                var tempModel = new List<PurchaseOrder>();
                foreach (var order in model)
                {
                    var details = order.PurchaseOrderDetail.Where(d => items.Contains(d.id_item));
                    if (details.Any())
                    {
                        tempModel.Add(order);
                    }
                }

                model = tempModel;
            }

            if (purchaseOrder.id_priceList != null && purchaseOrder.id_priceList != 0)
            {
                model = model.Where(o => o.id_priceList == purchaseOrder.id_priceList).ToList();
            }

            if (purchaseOrder.id_certification != null && purchaseOrder.id_certification != 0)
            {
                model = model.Where(o => o.id_certification == purchaseOrder.id_certification).ToList();
            }

            //if (purchaseOrder.id_paymentTerm != 0)
            //{
            //    model = model.Where(o => o.id_paymentTerm == purchaseOrder.id_paymentTerm).ToList();
            //}

            if (purchaseOrder.id_paymentMethod != 0)
            {
                model = model.Where(o => o.id_paymentMethod == purchaseOrder.id_paymentMethod).ToList();
            }

            if (id_logicalOperator != null && id_logicalOperator > 0)
            {
                LogicalOperator vlogical = DataProviders.DataProviderLogicalOperator.LogicalBoleambyID(id_logicalOperator);

                purchaseOrder.requiredLogistic = bool.Parse(vlogical.code);


                model = model.Where(o => o.requiredLogistic == purchaseOrder.requiredLogistic).ToList();

            }

            #endregion


            var idPersonProcessPlant = db.Person.FirstOrDefault(p => p.identification_number == ActiveCompany.ruc)?.id ?? 0;

            foreach (var item in model)
            {
                var aPerson = db.Person.FirstOrDefault(fod=> fod.id == (item.id_personProcessPlant != null ? item.id_personProcessPlant : idPersonProcessPlant));
                item.processPlantName = aPerson != null ? aPerson.processPlant : "";
            }

            TempData["model"] = model;
            TempData.Keep("model");


            return PartialView("_PurchaseOrderResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }
        [HttpPost]
        public ActionResult GetResultsAdvancedFilter()
        {
            List<int> listIds = (TempData["listIds"] as List<int>);
            listIds = listIds ?? new List<int>();
            //if (codeAdvancedFiltersConfiguration == "OC")
            //{
            List<PurchaseOrder> model = null;

            tbsysUserRecordSecurity UserRecordSecurity = db.tbsysUserRecordSecurity.FirstOrDefault(r => r.id_user == ActiveUser.id && r.tbsysObjSecurityRecord.obj == "PurchaseOrder");
            if (UserRecordSecurity != null)
            {
                model = db.PurchaseOrder.Where(r => r.id_buyer == ActiveUser.Employee.id && listIds.Contains(r.id)).ToList();

            }
            else
            {
                model = db.PurchaseOrder.AsEnumerable().Where(w => listIds.Contains(w.id)).ToList();
            }


            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_PurchaseOrderResultsPartial", model.OrderByDescending(o => o.id).ToList());
            //}
            //else
            //if (codeAdvancedFiltersConfiguration == "RP")
            //{
            //    List<ProductionLot> model = db.ProductionLot.AsEnumerable().Where(w => w.ProductionProcess.code == "REC" && listIds.Contains(w.id)).ToList();
            //    TempData["model"] = model;
            //    TempData.Keep("model");

            //    return PartialView("ProductionLotReception/_ProductionLotReceptionResultsPartial", model.OrderByDescending(o => o.id).ToList());
            //}


            //return null;

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseRequestDetailsResults()
        {
            // db.PurchaseOrderDetail.Where(X=> )
            ////var ListPurchaseResquest = from e in db.PurchaseOrderDetail
            ////                           where e.PurchaseOrder.Document.DocumentState.code == "01" && e.PurchaseOrderDetailPurchaseRequest != null
            ////                           select e.PurchaseOrderDetailPurchaseRequest;

            TempData.Keep("model");
            var ListPurchaseResquest = (from e in db.PurchaseOrderDetailPurchaseRequest
                                        where e.PurchaseOrderDetail.PurchaseOrder.Document.DocumentState.code != "05" && //"05" Anulada
                                              e.PurchaseOrderDetail.isActive
                                        select e.id_purchaseRequestDetail).ToList();

            var model = db.PurchaseRequestDetail.Where(d => d.PurchaseRequest.Document.DocumentState.code == "06" && d.quantityOutstandingPurchase > 0)//"06" AUTORIZADA
                          .OrderByDescending(d => d.id_purchaseRequest);

            if (ListPurchaseResquest != null && ListPurchaseResquest.Count > 0)
            {
                model = model.Where(X => !ListPurchaseResquest.Contains(X.id)).OrderByDescending(d => d.id_purchaseRequest);

            }



            return PartialView("_PurchaseRequestDetailsResultsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseRequestDetailsPartial()
        {

            TempData.Keep("model");
            var ListPurchaseResquest = (from e in db.PurchaseOrderDetailPurchaseRequest
                                        where e.PurchaseOrderDetail.PurchaseOrder.Document.DocumentState.code != "05" && //"05" Anulada
                                              e.PurchaseOrderDetail.isActive
                                        select e.id_purchaseRequestDetail).ToList();

            var model = db.PurchaseRequestDetail.Where(d => d.PurchaseRequest.Document.DocumentState.code == "06" && d.quantityOutstandingPurchase > 0)//"06" AUTORIZADA
                          .OrderByDescending(d => d.id_purchaseRequest);

            if (ListPurchaseResquest != null && ListPurchaseResquest.Count > 0)
            {
                model = model.Where(X => !ListPurchaseResquest.Contains(X.id)).OrderByDescending(d => d.id_purchaseRequest);

            }
            return PartialView("_PurchaseRequestDetailsPartial", model.ToList());
        }

        #endregion

        #region PURCHASE ORDER MASTER DETAILS

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderResultsDetailViewDetailsPartial()
        {
            TempData.Keep("model");
            int id_purchaseOrder = (Request.Params["id_purchaseOrder"] != null && Request.Params["id_purchaseOrder"] != "") ? int.Parse(Request.Params["id_purchaseOrder"]) : -1;
            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id_purchaseOrder);
            return PartialView("_PurchaseOrderResultsDetailViewDetailsPartial", purchaseOrder.PurchaseOrderDetail.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderResultsDetailViewDetailsPartialBG()
        {
            TempData.Keep("model");
            int id_purchaseOrder = (Request.Params["id_purchaseOrder"] != null && Request.Params["id_purchaseOrder"] != "") ? int.Parse(Request.Params["id_purchaseOrder"]) : -1;
            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id_purchaseOrder);
            return PartialView("_PurchaseOrderResultsDetailViewDetailsPartialBG", purchaseOrder.PurchaseOrderDetailByGrammage.ToList());
        }

        #endregion

        #region PURCHASE ORDER EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditPurchaseOrder(int id, int[] requestDetails)
        {
            TempData.Keep("model");
            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(o => o.id == id);

            string parOCDetail = DataProviderSetting.ValueSetting("DPGOCD");

            #region"Se crea la Orden de Compra"
            int id_company = (int)ViewData["id_company"];

            if (purchaseOrder == null)
            {
                tbsysUserRecordSecurity UserRecordSecurity = null;
                int id_us = ActiveUser.id;
                if (id_us != 0)
                {
                    UserRecordSecurity = db.tbsysUserRecordSecurity.FirstOrDefault(r => r.id_user == id_us && r.tbsysObjSecurityRecord.obj == "PurchaseOrder");
                }

                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("02"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");


                Employee employee = ActiveUser.Employee;
                var requiereLogistica = db.Company.FirstOrDefault(r => r.id == id_company).requiredLogistic ?? false;
                //var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
                //this.ViewBag.IdPersonProcess = idPersonProcess;


                PaymentMethod paymentMethod = db.PaymentMethod.Where(x => x.code.Equals("02") && x.id_company == id_company).FirstOrDefault();

                var motivoCompraMateriaPrimera = db.PurchaseReason
                    .FirstOrDefault(pr => pr.code == "MP");

                purchaseOrder = new PurchaseOrder
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
                    deliveryDate = DateTime.Now,
                    pricePerList = true,
                    requiredLogistic = (requiereLogistica) ? true : false,
                    id_personRequesting = employee?.id ?? 0,
                    Employee = employee,
                    id_purchaseReason = motivoCompraMateriaPrimera?.id ?? 0,
                    PurchaseReason = motivoCompraMateriaPrimera,

                    PurchaseOrderDetail = new List<PurchaseOrderDetail>()


                };
                List<tbsysDocumentsPersonalizationDetail> lstDocPersDetail = db.tbsysDocumentsPersonalizationDetail
                                                                                .Where(fod => fod.id_DocumentType == documentType.id)
                                                                                .ToList();

                if (lstDocPersDetail != null && lstDocPersDetail.Count > 0)
                {
                    foreach (var det in lstDocPersDetail)
                    {
                        if (det.nameObjectTable == "PurchaseOrderCustomizedInformation")
                        {
                            purchaseOrder.PurchaseOrderCustomizedInformation = new PurchaseOrderCustomizedInformation();
                            purchaseOrder.PurchaseOrderCustomizedInformation.hasSecurity = false;
                        }
                    }
                }

                if (parOCDetail == "1")
                {
                    purchaseOrder.PurchaseOrderDetailByGrammage = new List<PurchaseOrderDetailByGrammage>();
                }
                if (UserRecordSecurity != null)
                {
                    var id_employee = db.User.FirstOrDefault(fod => fod.id == id_us)?.id_employee ?? 0;
                    if (db.Person.FirstOrDefault(fod => fod.Rol.Any(a => a.name == "Comprador") && fod.id == id_employee) != null)
                    {
                        purchaseOrder.id_buyer = id_employee;
                    }
                }

                if (paymentMethod != null)
                {
                    purchaseOrder.id_paymentMethod = paymentMethod.id;
                }
            }
            #endregion
            string rucCompany = db.Company.FirstOrDefault(r => r.id == id_company).ruc;
            var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
            this.ViewBag.IdPersonProcess = idPersonProcess;

            if (purchaseOrder != null && purchaseOrder.Document != null)
            {
                purchaseOrder.descriptionDocPO = purchaseOrder.Document.description;
            }
            var id_providerAux = 0;
            var distinctProvider = false;

            //Por Ahora no se usa
            #region"Detalle de Requerimiento de Compras"
            if (requestDetails != null)
            {
                foreach (var id_purchaseRequestDetail in requestDetails)
                {
                    PurchaseRequestDetail
                    requestDetail =
                        db.PurchaseRequestDetail.FirstOrDefault(
                            d =>
                                //d.id_purchaseRequest == purchaseRequestDetail.id_purchaseRequest &&
                                //d.id_item == purchaseRequestDetail.id_item);
                                d.id == id_purchaseRequestDetail);

                    if (requestDetail != null)
                    {
                        if (id_providerAux == 0)
                        {
                            id_providerAux = requestDetail.id_proposedProvider ?? 0;
                        }
                        else
                        {
                            if (requestDetail.id_proposedProvider != null && id_providerAux != requestDetail.id_proposedProvider && !distinctProvider)
                            {
                                distinctProvider = true;
                            }
                        }

                        if (purchaseOrder.id_purchaseReason == 0)
                        {
                            PurchaseReason purchaseReason = null;
                            if (requestDetail.Item.InventoryLine.code.Equals("MP"))
                            {
                                purchaseReason = db.PurchaseReason.FirstOrDefault(fod => fod.code == "MP");
                                if (purchaseReason == null)
                                {
                                    throw (new Exception("Debe definirse Motivo de Compra: COMPRA DE MATERIA PRIMA, con código : MP, configúrelo e intente de nuevo"));
                                }
                            }
                            else
                            {
                                purchaseReason = db.PurchaseReason.FirstOrDefault(fod => fod.code == "MI");
                                if (purchaseReason == null)
                                {
                                    throw (new Exception("Debe definirse Motivo de Compra: COMPRA DE MATERIALES/INSUMOS, con código : MI, configúrelo e intente de nuevo"));
                                }
                                purchaseOrder.pricePerList = false;
                            }

                            purchaseOrder.id_purchaseReason = purchaseReason.id;
                            purchaseOrder.PurchaseReason = purchaseReason;
                        }

                        decimal price = PurchaseDetailPrice(requestDetail.id_item, purchaseOrder);
                        decimal iva = PurchaseDetailIVA(requestDetail.id_item, requestDetail.quantityOutstandingPurchase, price);

                        PurchaseOrderDetail newOrderDetail = new PurchaseOrderDetail
                        {
                            id = purchaseOrder.PurchaseOrderDetail.Count() > 0 ? purchaseOrder.PurchaseOrderDetail.Max(pld => pld.id) + 1 : 1,
                            id_item = requestDetail.id_item,
                            Item = requestDetail.Item,
                            //id_Grammage = requestDetail.id_grammage,
                            quantityOrdered = requestDetail.quantityOutstandingPurchase,
                            quantityApproved = requestDetail.quantityOutstandingPurchase,
                            quantityReceived = 0.0M,
                            quantityRequested = requestDetail.quantityOutstandingPurchase,

                            price = price,
                            iva = iva,
                            subtotal = price * requestDetail.quantityOutstandingPurchase,
                            total = price * requestDetail.quantityOutstandingPurchase + iva,

                            isActive = true,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,

                            PurchaseOrderDetailPurchaseRequest = new List<PurchaseOrderDetailPurchaseRequest>()
                        };

                        PurchaseOrderDetailPurchaseRequest newPurchaseOrderDetailPurchaseRequest = new PurchaseOrderDetailPurchaseRequest
                        {
                            id_purchaseRequest = requestDetail.id_purchaseRequest,
                            id_purchaseRequestDetail = requestDetail.id,
                            quantity = requestDetail.quantityOutstandingPurchase
                        };
                        newOrderDetail.PurchaseOrderDetailPurchaseRequest.Add(newPurchaseOrderDetailPurchaseRequest);

                        purchaseOrder.PurchaseOrderDetail.Add(newOrderDetail);
                    }
                }
            }
            #endregion

            if (id_providerAux != 0 && !distinctProvider)
            {
                purchaseOrder.id_provider = id_providerAux;
                purchaseOrder.Provider = db.Provider.FirstOrDefault(fod => fod.id == id_providerAux);

                purchaseOrder.id_providerapparent = db.ProviderTypeShrimp.Where(x => x.id_provider == id_providerAux).FirstOrDefault()?.id_protectiveProvider;
                purchaseOrder.Provider1 = db.Provider.FirstOrDefault(fod => fod.id == purchaseOrder.id_providerapparent);

                int? id_paymentTerm = db.Provider.FirstOrDefault(fod => fod.id == id_providerAux)?.id_paymentTerm;

                if (id_paymentTerm != null && id_paymentTerm > 0)
                {
                    purchaseOrder.id_paymentTerm = (int)id_paymentTerm;
                }
            }

            if (parOCDetail == "1")
            {
                purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder.id, purchaseOrder.PurchaseOrderDetailByGrammage.ToList());
            }
            else
            {
                purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder.id, purchaseOrder.PurchaseOrderDetail.ToList());
            }


            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            return PartialView("_FormEditPurchaseOrder", purchaseOrder);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderCopy(int id)
        {
            TempData.Keep("model");
            PurchaseOrder purchaseOrder = db.PurchaseOrder.AsEnumerable().FirstOrDefault(o => o.id == id);

            DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("02"));
            DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.id == 1);

            Employee employee = db.Employee.FirstOrDefault(e => e.id == ActiveUser.id_employee);

            if (purchaseOrder != null)
            {
                purchaseOrder.id = 0;
                purchaseOrder.Document = new Document
                {
                    id = 0,
                    id_documentType = documentType?.id ?? 0,
                    DocumentType = documentType,
                    id_documentState = documentState?.id ?? 0,
                    DocumentState = documentState,
                    emissionDate = DateTime.Now
                };
                purchaseOrder.id_personRequesting = employee?.id ?? 0;
                purchaseOrder.Employee = employee;
            }

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            return PartialView("_FormEditPurchaseOrder", purchaseOrder);
        }

        #endregion

        #region PURCHASE ORDER HEADER

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrdersPartial()
        {
            var model = (TempData["model"] as List<PurchaseOrder>);
            model = model ?? new List<PurchaseOrder>();
            //int i = ActiveUser.Employee.id;m

            tbsysUserRecordSecurity UserRecordSecurity = db.tbsysUserRecordSecurity.FirstOrDefault(r => r.id_user == ActiveUser.id && r.tbsysObjSecurityRecord.obj == "PurchaseOrder");
            if (UserRecordSecurity != null)
            {
                model = model.Where(r => r.id_buyer == ActiveUser.Employee.id).ToList();

            }

            //this.ViewBag.IdPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == ActiveCompany.ruc)?.id ?? 0;

            TempData.Keep("model");

            return PartialView("_PurchaseOrdersPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrdersPartialAddNew(string delivery, bool approve, PurchaseOrder itemPO
                                            , PurchaseOrderCustomizedInformation itemPOci, Document itemDoc
                                            , PurchaseOrderImportationInformation importationInformation)
        {
            TempData.Keep("model");
            var model = db.PurchaseOrder;
            //int _answer = 0;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            if (!string.IsNullOrEmpty(delivery)) itemPO.deliveryhour = TimeSpan.Parse(delivery);

            PurchaseOrder tempPurchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            tempPurchaseOrder = tempPurchaseOrder ?? new PurchaseOrder();
            string parOCDetail = DataProviderSetting.ValueSetting("DPGOCD");

            var id_companyAux = (int)ViewData["id_company"];
            string rucCompany = db.Company.FirstOrDefault(r => r.id == id_companyAux).ruc;
            var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
            this.ViewBag.IdPersonProcess = idPersonProcess;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region Document

                    itemDoc.id_userCreate = ActiveUser.id;
                    itemDoc.dateCreate = DateTime.Now;
                    itemDoc.id_userUpdate = ActiveUser.id;
                    itemDoc.dateUpdate = DateTime.Now;
                    itemDoc.sequential = GetDocumentSequential(itemDoc.id_documentType);
                    itemDoc.number = GetDocumentNumber(itemDoc.id_documentType);

                    //Descripción del Documento en el Tab de Órdenes de Compra
                    itemDoc.description = itemPO.descriptionDocPO;

                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == itemDoc.id_documentType);
                    itemDoc.DocumentType = documentType;

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == itemDoc.id_documentState);
                    itemDoc.DocumentState = documentState;

                    Employee employee = db.Employee.FirstOrDefault(e => e.id == ActiveUser.id_employee);
                    itemPO.id_personRequesting = ActiveUser.id_employee ?? 0;
                    itemPO.Employee = employee;

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

                    itemDoc.isOpen = false;
                    //Actualiza Secuencial
                    if (documentType != null)
                    {
                        documentType.currentNumber = documentType.currentNumber + 1;
                        db.DocumentType.Attach(documentType);
                        db.Entry(documentType).State = EntityState.Modified;
                    }
                    //itemPR.Employee.id = itemPR.id_personRequesting;


                    #endregion

                    #region PurchaseOrder

                    itemPO.Document = itemDoc;

                    itemPO.sendTo = itemPO.sendTo ?? "";
                    itemPO.deliveryTo = itemPO.deliveryTo ?? "";
                    #endregion

                    List<tbsysDocumentsPersonalizationDetail> lstDocPersDetail = db.tbsysDocumentsPersonalizationDetail
                                                                                .Where(fod => fod.id_DocumentType == documentType.id)
                                                                                .ToList();

                    if (lstDocPersDetail != null && lstDocPersDetail.Count > 0)
                    {
                        foreach (var det in lstDocPersDetail)
                        {
                            if (det.nameObjectTable == "PurchaseOrderCustomizedInformation")
                            {
                                itemPO.PurchaseOrderCustomizedInformation = new PurchaseOrderCustomizedInformation();
                                itemPO.PurchaseOrderCustomizedInformation.hasSecurity = itemPOci?.hasSecurity ?? false;
                            }
                        }
                    }

                    if (parOCDetail == "1")
                    {
                        #region"PurchaseOrderDetails with Grammaje"

                        #region  Carga Info al Temporal
                        tempPurchaseOrder.deliveryDate = itemPO.deliveryDate;
                        tempPurchaseOrder.deliveryhour = itemPO.deliveryhour;
                        tempPurchaseOrder.deliveryTo = itemPO.deliveryTo ?? "";
                        tempPurchaseOrder.Document = itemPO.Document;
                        tempPurchaseOrder.Employee = itemPO.Employee;
                        tempPurchaseOrder.FishingSite = itemPO.FishingSite;
                        tempPurchaseOrder.id_buyer = itemPO.id_buyer;
                        tempPurchaseOrder.id_FishingSite = itemPO.id_FishingSite;
                        tempPurchaseOrder.id_paymentMethod = itemPO.id_paymentMethod;
                        tempPurchaseOrder.id_paymentTerm = itemPO.id_paymentTerm;
                        tempPurchaseOrder.id_personRequesting = itemPO.id_personRequesting;
                        tempPurchaseOrder.id_priceList = itemPO.id_priceList;
                        tempPurchaseOrder.id_productionUnitProvider = itemPO.id_productionUnitProvider;
                        tempPurchaseOrder.id_productionUnitProviderProtective = itemPO.id_productionUnitProviderProtective;
                        tempPurchaseOrder.id_provider = itemPO.id_provider;
                        tempPurchaseOrder.id_providerapparent = itemPO.id_provider;
                        tempPurchaseOrder.id_purchaseReason = itemPO.id_purchaseReason;
                        tempPurchaseOrder.id_shippingType = itemPO.id_shippingType;
                        tempPurchaseOrder.isImportation = itemPO.isImportation;
                        tempPurchaseOrder.requiredLogistic = itemPO.requiredLogistic;
                        tempPurchaseOrder.sendTo = itemPO.sendTo ?? "";
                        itemPO.id_providerapparent = itemPO.id_provider;
                        tempPurchaseOrder.id_certification = itemPO.id_certification;
                        #endregion

                        #region"Transaccion"
                        if (tempPurchaseOrder?.PurchaseOrderDetailByGrammage != null)
                        {
                            itemPO.PurchaseOrderDetailByGrammage = new List<PurchaseOrderDetailByGrammage>();
                            var itemPODetails = tempPurchaseOrder.PurchaseOrderDetailByGrammage.ToList();

                            //List<int> listProcesstype = new List<int>();

                            //listProcesstype = (from hr in db.PriceListDetail
                            //                   join de in db.ItemProcessType on hr.id_item equals de.Id_Item
                            //                   where hr.id_priceList == itemPO.id_priceList
                            //                   select de.Id_ProcessType).ToList();


                            if (itemPO.pricePerList)
                            {
                                if (itemPO.id_priceList == null || itemPO.id_priceList <= 0)
                                {
                                    TempData.Keep("purchaseOrder");
                                    ViewData["EditMessage"] = ErrorMessage("Seleccione la Lista de Precio.");
                                    return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                }
                            }

                            foreach (var detail in itemPODetails)
                            {
                                var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                                if (approve && detail.quantityApproved <= 0)
                                {
                                    TempData.Keep("purchaseOrder");
                                    ViewData["EditMessage"] = ErrorMessage("No se puede aprobar la orden de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada mayor que cero.");
                                    return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                }

                                if (detail.isActive)
                                {
                                    if (itemPO.pricePerList)
                                    {

                                        //if (listProcesstype != null && listProcesstype.Count > 0)
                                        //{
                                        //var isExists = (from hr in db.ItemProcessType
                                        //                where hr.Id_Item == detail.id_item
                                        //                select hr.Id_ProcessType).ToList().Count;

                                        //if (isExists <= 0)
                                        //{
                                        //    TempData.Keep("purchaseOrder");
                                        //    ViewData["EditMessage"] = ErrorMessage("El Producto no tiene el mismo Tipo de la Lista de Precio");
                                        //    return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                        //}
                                        //}
                                    }

                                    if (detail.id_Grammage == null || detail.id_Grammage < 0)
                                    {
                                        TempData.Keep("purchaseOrder");
                                        ViewData["EditMessage"] = ErrorMessage("Existe detalle sin Gramaje.");
                                        return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                    }
                                }
                                var tempItemPODetail = new PurchaseOrderDetailByGrammage
                                {
                                    id_item = detail.id_item,
                                    Item = itemAux,

                                    quantityRequested = detail.quantityRequested,
                                    quantityOrdered = detail.quantityOrdered,
                                    quantityApproved = detail.quantityApproved,
                                    quantityReceived = detail.quantityReceived,
                                    productionUnitProviderPoolreference = detail.productionUnitProviderPoolreference,
                                    price = detail.price,
                                    iva = detail.iva,

                                    subtotal = detail.subtotal,
                                    total = detail.total,

                                    isActive = detail.isActive,
                                    id_userCreate = detail.id_userCreate,
                                    dateCreate = detail.dateCreate,
                                    id_userUpdate = detail.id_userUpdate,
                                    dateUpdate = detail.dateUpdate,
                                    id_Grammage = detail.id_Grammage,
                                    PurchaseOrderDetailByGrammagePurchaseRequest = new List<PurchaseOrderDetailByGrammagePurchaseRequest>()
                                };

                                foreach (var requestDetail in detail.PurchaseOrderDetailByGrammagePurchaseRequest)
                                {
                                    if (requestDetail.PurchaseRequestDetail.quantityOutstandingPurchase < detail.quantityApproved)
                                    {
                                        TempData.Keep("purchaseOrder");
                                        ViewData["EditMessage"] = ErrorMessage("No se puede aprobar la orden de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada menor e igual a la Cantidad Requerida.");
                                        return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                    }
                                    tempItemPODetail.PurchaseOrderDetailByGrammagePurchaseRequest.Add(new PurchaseOrderDetailByGrammagePurchaseRequest
                                    {
                                        id_purchaseRequest = requestDetail.id_purchaseRequest,
                                        id_purchaseRequestDetail = requestDetail.id_purchaseRequestDetail,
                                        quantity = detail.quantityApproved
                                    });
                                }

                                if (tempItemPODetail.isActive)
                                    itemPO.PurchaseOrderDetailByGrammage.Add(tempItemPODetail);
                            }

                            #region"Aqui agrupo los detalles Falsos de Orden de Compra"
                            var lstIdItems = itemPO.PurchaseOrderDetailByGrammage.Select(s => s.id_item).Distinct().ToList();
                            if (lstIdItems != null && lstIdItems.Count > 0)
                            {
                                foreach (var i in lstIdItems)
                                {
                                    //Obtengo la lista de items en el detalle con cantidades
                                    var itNlst = itemPO
                                                    .PurchaseOrderDetailByGrammage
                                                    .Where(w => w.id_item == i && w.isActive)
                                                    .Select(s => new
                                                    {
                                                        s.id_item,
                                                        valGrammage = DataProviderGrammage.GrammageById(s.id_Grammage)?.value ?? 0,
                                                        s.quantityApproved,
                                                        s.quantityDispatched,
                                                        s.quantityOrdered,
                                                        s.quantityReceived,
                                                        s.quantityRequested,
                                                        s.price,
                                                        s.iva,
                                                        s.subtotal,
                                                        s.total,
                                                    }).ToList();
                                    //A partir de esta lista obtengo lista y gramajes
                                    if (itNlst != null && itNlst.Count > 0)
                                    {
                                        var iNLstFn = itNlst
                                                        .GroupBy(t => new { id = t.id_item })
                                                        .Select(g => new
                                                        {
                                                            ID = g.Key.id,
                                                            AvGrama = g.Average(p => p.valGrammage),
                                                            SuQuApproved = g.Sum(p => p.quantityApproved),
                                                            SuQuDispatched = g.Sum(p => p.quantityDispatched),
                                                            SuQuOrdered = g.Sum(p => p.quantityOrdered),
                                                            SuQuReceived = g.Sum(p => p.quantityReceived),
                                                            SuQuRequested = g.Sum(p => p.quantityRequested),
                                                            Avprice = g.Average(p => p.price),
                                                            Aviva = g.Average(p => p.iva),
                                                            Susubtotal = g.Sum(p => p.subtotal),
                                                            SuTotal = g.Sum(p => p.total)
                                                        }).ToList();

                                        // Si esta lista tiene un solo elemento, lo agrego
                                        if (iNLstFn != null && iNLstFn.Count == 1)
                                        {
                                            int id_g = 0;
                                            //Obtengo este único elemento
                                            var objF = iNLstFn.FirstOrDefault();
                                            if (objF != null)
                                            {
                                                //Obtengo el primer gramaje que sea aproximado
                                                //al valor promedio, por defecto traigo el cualquier gramaje
                                                id_g = db.Grammage
                                                            .FirstOrDefault(fod => fod.value >= objF.AvGrama)?.id ?? db.Grammage.FirstOrDefault().id;

                                            }

                                            var tempItemPODetailNew = new PurchaseOrderDetail
                                            {
                                                id_item = objF.ID,

                                                quantityRequested = objF.SuQuRequested,
                                                quantityOrdered = objF.SuQuOrdered,
                                                quantityApproved = objF.SuQuApproved,
                                                quantityReceived = objF.SuQuReceived,
                                                productionUnitProviderPoolreference = "",//itemPO.PurchaseOrderDetailByGrammage.FirstOrDefault(fod => fod.id_item == objF.ID)?.productionUnitProviderPoolreference ?? "",

                                                price = objF.Avprice,
                                                iva = objF.Aviva,
                                                subtotal = objF.Susubtotal,
                                                total = objF.SuTotal,

                                                isActive = true,
                                                id_userCreate = ActiveUser.id,
                                                dateCreate = DateTime.Now,
                                                id_userUpdate = ActiveUser.id,
                                                dateUpdate = DateTime.Now,
                                                id_Grammage = id_g

                                            };
                                            var ls = itemPO.PurchaseOrderDetailByGrammage.Where(w => w.id_item == objF.ID).ToList();
                                            tempItemPODetailNew.productionUnitProviderPoolreference = "";
                                            foreach (var det in ls)
                                            {
                                                tempItemPODetailNew.productionUnitProviderPoolreference += det.productionUnitProviderPoolreference + ", ";
                                            }
                                            itemPO.PurchaseOrderDetail.Add(tempItemPODetailNew);
                                        }
                                    }

                                }
                            }
                            #endregion
                        }
                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region PurchaseOrderDetails

                        #region  Carga Info al Temporal
                        tempPurchaseOrder.deliveryDate = itemPO.deliveryDate;
                        tempPurchaseOrder.deliveryhour = itemPO.deliveryhour;
                        tempPurchaseOrder.deliveryTo = itemPO.deliveryTo ?? "";
                        tempPurchaseOrder.Document = itemPO.Document;
                        tempPurchaseOrder.Employee = itemPO.Employee;
                        tempPurchaseOrder.FishingSite = itemPO.FishingSite;
                        tempPurchaseOrder.id_buyer = itemPO.id_buyer;
                        tempPurchaseOrder.id_FishingSite = itemPO.id_FishingSite;
                        tempPurchaseOrder.id_paymentMethod = itemPO.id_paymentMethod;
                        tempPurchaseOrder.id_paymentTerm = itemPO.id_paymentTerm;
                        tempPurchaseOrder.id_personRequesting = itemPO.id_personRequesting;
                        tempPurchaseOrder.id_priceList = itemPO.id_priceList;
                        tempPurchaseOrder.id_productionUnitProvider = itemPO.id_productionUnitProvider;
                        tempPurchaseOrder.id_productionUnitProviderProtective = itemPO.id_productionUnitProviderProtective;
                        tempPurchaseOrder.id_provider = itemPO.id_provider;
                        tempPurchaseOrder.id_providerapparent = itemPO.id_provider;
                        tempPurchaseOrder.id_purchaseReason = itemPO.id_purchaseReason;
                        tempPurchaseOrder.id_shippingType = itemPO.id_shippingType;
                        tempPurchaseOrder.isImportation = itemPO.isImportation;
                        tempPurchaseOrder.requiredLogistic = itemPO.requiredLogistic;
                        tempPurchaseOrder.sendTo = itemPO.sendTo ?? "";
                        tempPurchaseOrder.id_certification = itemPO.id_certification;
                        #endregion

                        #region"Transaccion"
                        if (tempPurchaseOrder?.PurchaseOrderDetail != null)
                        {
                            itemPO.PurchaseOrderDetail = new List<PurchaseOrderDetail>();
                            var itemPODetails = tempPurchaseOrder.PurchaseOrderDetail.ToList();

                            List<int> listProcesstype = new List<int>();

                            listProcesstype = (from hr in db.PriceListDetail
                                               join de in db.ItemProcessType on hr.id_item equals de.Id_Item
                                               where hr.id_priceList == itemPO.id_priceList
                                               select de.Id_ProcessType).ToList();


                            if (itemPO.pricePerList)
                            {
                                if (itemPO.id_priceList == null || itemPO.id_priceList <= 0)
                                {
                                    TempData.Keep("purchaseOrder");
                                    ViewData["EditMessage"] = ErrorMessage("Seleccione la Lista de Precio.");
                                    return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                }
                            }
                            foreach (var detail in itemPODetails)
                            {
                                var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                                if (approve && detail.quantityApproved <= 0)
                                {
                                    TempData.Keep("purchaseOrder");
                                    ViewData["EditMessage"] = ErrorMessage("No se puede aprobar la orden de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada mayor que cero.");
                                    return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                }

                                if (detail.isActive)
                                {
                                    if (itemPO.pricePerList)
                                    {

                                        if (listProcesstype != null && listProcesstype.Count > 0)
                                        {
                                            //var isExists = (from hr in db.ItemProcessType
                                            //                where hr.Id_Item == detail.id_item && listProcesstype.Contains(hr.Id_ProcessType)
                                            //                select hr.Id_ProcessType).ToList().Count;

                                            //if (isExists <= 0)
                                            //{

                                            //    TempData.Keep("purchaseOrder");
                                            //    ViewData["EditMessage"] = ErrorMessage("El Producto no tiene el mismo Tipo de la Lista de Precio");
                                            //    return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                            //}

                                        }
                                    }


                                    if (detail.id_Grammage == null || detail.id_Grammage < 0)
                                    {

                                        TempData.Keep("purchaseOrder");
                                        ViewData["EditMessage"] = ErrorMessage("Existe detalle sin Gramaje.");
                                        return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                    }


                                }
                                var tempItemPODetail = new PurchaseOrderDetail
                                {
                                    id_item = detail.id_item,
                                    Item = itemAux,

                                    quantityRequested = detail.quantityRequested,
                                    quantityOrdered = detail.quantityOrdered,
                                    quantityApproved = detail.quantityApproved,
                                    quantityReceived = detail.quantityReceived,
                                    productionUnitProviderPoolreference = detail.productionUnitProviderPoolreference,
                                    price = detail.price,
                                    iva = detail.iva,

                                    subtotal = detail.subtotal,
                                    total = detail.total,

                                    isActive = detail.isActive,
                                    id_userCreate = detail.id_userCreate,
                                    dateCreate = detail.dateCreate,
                                    id_userUpdate = detail.id_userUpdate,
                                    dateUpdate = detail.dateUpdate,
                                    id_Grammage = detail.id_Grammage,

                                    PurchaseOrderDetailPurchaseRequest = new List<PurchaseOrderDetailPurchaseRequest>()
                                };

                                foreach (var requestDetail in detail.PurchaseOrderDetailPurchaseRequest)
                                {
                                    tempItemPODetail.PurchaseOrderDetailPurchaseRequest.Add(new PurchaseOrderDetailPurchaseRequest
                                    {
                                        id_purchaseRequest = requestDetail.id_purchaseRequest,
                                        id_purchaseRequestDetail = requestDetail.id_purchaseRequestDetail,
                                        quantity = detail.quantityApproved
                                    });
                                }

                                if (tempItemPODetail.isActive)
                                    itemPO.PurchaseOrderDetail.Add(tempItemPODetail);
                            }
                        }
                        #endregion

                        #endregion
                    }

                    #region IMPORTATION INFORMATION

                    if (itemPO.isImportation)
                    {
                        importationInformation.shipmentDate = FormatDate(importationInformation.shipmentDate, Request.UserLanguages);
                        importationInformation.departureCustomsDate = FormatDate(importationInformation.departureCustomsDate, Request.UserLanguages);
                        importationInformation.arrivalDate = FormatDate(importationInformation.arrivalDate, Request.UserLanguages);
                        itemPO.PurchaseOrderImportationInformation = importationInformation;
                    }

                    #endregion

                    #region TOTALS

                    itemPO.PurchaseOrderTotal = PurchaseOrderTotals(itemPO.id, itemPO.PurchaseOrderDetail.ToList());

                    #endregion

                    if (itemPO.PurchaseOrderDetail.Count == 0)
                    {
                        TempData.Keep("purchaseOrder");
                        ViewData["EditMessage"] = ErrorMessage("No se puede guardar una orden de compra sin detalles");
                        return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                    }


                    if (itemPO.PurchaseOrderDetail.Count == 0)
                    {
                        TempData.Keep("purchaseOrder");
                        ViewData["EditMessage"] = ErrorMessage("No se puede guardar una orden de compra sin detalles");
                        return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                    }


                    if (approve)
                    {
                        foreach (var detail in itemPO.PurchaseOrderDetailByGrammage)
                        {
                            foreach (var purchaseOrderDetailByGrammagePurchaseRequest in detail.PurchaseOrderDetailByGrammagePurchaseRequest)
                            {
                                ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseOrderDetailByGrammage,
                                                                               purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseRequestDetail,
                                                                               purchaseOrderDetailByGrammagePurchaseRequest.quantity);
                            }
                        }
                        //foreach (var detail in itemPO.PurchaseOrderDetail)
                        //{
                        //    foreach (var purchaseOrderDetailPurchaseRequest in detail.PurchaseOrderDetailPurchaseRequest)
                        //    {
                        //        ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailPurchaseRequest.id_purchaseOrderDetail,
                        //                                                       purchaseOrderDetailPurchaseRequest.id_purchaseRequestDetail,
                        //                                                       purchaseOrderDetailPurchaseRequest.quantity);
                        //    }
                        //}
                        itemPO.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA

                    }

                    //Guarda Orden de Compra
                    db.PurchaseOrder.Add(itemPO);
                    db.SaveChanges();
                    trans.Commit();

                    TempData["purchaseOrder"] = itemPO;
                    TempData.Keep("purchaseOrder");
                    itemPO.descriptionDocPO = itemPO.Document.description;

                    ViewData["EditMessage"] = SuccessMessage("Orden de Compra: " + itemPO.Document.number + " guardada exitosamente");
                    //_answer = 1;
                }
                catch (Exception e)
                {
                    TempData.Keep("purchaseOrder");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            return PartialView("_PurchaseOrderMainFormPartial", itemPO);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrdersPartialUpdate(string delivery, bool approve, PurchaseOrder itemPO
            , PurchaseOrderCustomizedInformation itemPOci
            , Document itemDoc, PurchaseOrderImportationInformation importationInformation)
        {

            
            string extraInfoPO = "";

            TempData.Keep("model");
            var model = db.PurchaseOrder;
            int count = 0;
            //int _answer = 0;
            PurchaseOrder modelItem = null;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string parOCDetail = DataProviderSetting.ValueSetting("DPGOCD");

            var id_companyAux = (int)ViewData["id_company"];
            string rucCompany = db.Company.FirstOrDefault(r => r.id == id_companyAux).ruc;
            var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
            this.ViewBag.IdPersonProcess = idPersonProcess;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    modelItem = model.FirstOrDefault(it => it.id == itemPO.id);
                    if (modelItem != null)
                    {
                        PurchaseOrder tempPurchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

                        #region DOCUMENT

                        modelItem.Document.description = itemDoc.description;
                        modelItem.Document.emissionDate = itemDoc.emissionDate;

                        modelItem.Document.id_userUpdate = ActiveUser.id;
                        modelItem.Document.dateUpdate = DateTime.Now;

                        //Descripción del Documento en el Tab de Órdenes de Compra
                        modelItem.Document.description = itemPO.descriptionDocPO;

                        #endregion

                        #region PURCHASE ORDER

                        modelItem.id_provider = itemPO.id_provider;
                        modelItem.id_buyer = itemPO.id_buyer;
                        modelItem.id_productionUnitProvider = itemPO.id_productionUnitProvider;
                        modelItem.id_priceList = itemPO.id_priceList;
                        modelItem.pricePerList = itemPO.pricePerList;
                        modelItem.requiredLogistic = itemPO.requiredLogistic;
                        modelItem.isImportation = itemPO.isImportation;
                        modelItem.deliveryDate = itemPO.deliveryDate;

                        itemPO.sendTo = itemPO.sendTo ?? "";
                        itemPO.deliveryTo = itemPO.deliveryTo ?? "";

                        modelItem.sendTo = itemPO.sendTo ?? "";
                        modelItem.deliveryTo = itemPO.deliveryTo ?? "";

                        modelItem.id_shippingType = itemPO.id_shippingType;
                        modelItem.id_personRequesting = itemPO.id_personRequesting;
                        modelItem.id_purchaseReason = itemPO.id_purchaseReason;
                        modelItem.id_paymentTerm = itemPO.id_paymentTerm;
                        modelItem.id_paymentMethod = itemPO.id_paymentMethod;
                        modelItem.id_providerapparent = itemPO.id_provider;
                        modelItem.id_productionUnitProviderProtective = itemPO.id_productionUnitProviderProtective;
                        modelItem.id_certification = itemPO.id_certification;

                        modelItem.id_FishingSite = itemPO.id_FishingSite;

                        List<tbsysDocumentsPersonalizationDetail> lstDocPersDetail = db.tbsysDocumentsPersonalizationDetail
                                                                                .Where(fod => fod.id_DocumentType == modelItem.Document.id_documentType)
                                                                                .ToList();

                        if (lstDocPersDetail != null && lstDocPersDetail.Count > 0)
                        {
                            foreach (var det in lstDocPersDetail)
                            {
                                if (det.nameObjectTable == "PurchaseOrderCustomizedInformation")
                                {
                                    modelItem.PurchaseOrderCustomizedInformation = modelItem.PurchaseOrderCustomizedInformation ?? new PurchaseOrderCustomizedInformation();
                                    modelItem.PurchaseOrderCustomizedInformation.hasSecurity = itemPOci?.hasSecurity ?? false;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(delivery)) modelItem.deliveryhour = TimeSpan.Parse(delivery);

                        #endregion

                        if (parOCDetail == "1")
                        {
                            #region"PurchaseOrderDetails with Grammaje"

                            #region  Carga Info al Temporal
                            tempPurchaseOrder.deliveryDate = itemPO.deliveryDate;
                            tempPurchaseOrder.deliveryhour = itemPO.deliveryhour;
                            if (!string.IsNullOrEmpty(delivery)) tempPurchaseOrder.deliveryhour = TimeSpan.Parse(delivery);

                            tempPurchaseOrder.deliveryTo = itemPO.deliveryTo ?? "";
                            tempPurchaseOrder.Document = modelItem.Document;
                            tempPurchaseOrder.Employee = itemPO.Employee;
                            tempPurchaseOrder.FishingSite = itemPO.FishingSite;
                            tempPurchaseOrder.id_buyer = itemPO.id_buyer;
                            tempPurchaseOrder.id_FishingSite = itemPO.id_FishingSite;
                            tempPurchaseOrder.id_paymentMethod = itemPO.id_paymentMethod;
                            tempPurchaseOrder.id_paymentTerm = itemPO.id_paymentTerm;
                            tempPurchaseOrder.id_personRequesting = itemPO.id_personRequesting;
                            tempPurchaseOrder.id_priceList = itemPO.id_priceList;
                            tempPurchaseOrder.id_productionUnitProvider = itemPO.id_productionUnitProvider;
                            tempPurchaseOrder.id_productionUnitProviderProtective = itemPO.id_productionUnitProviderProtective;
                            tempPurchaseOrder.id_provider = itemPO.id_provider;
                            tempPurchaseOrder.id_providerapparent = itemPO.id_provider;
                            tempPurchaseOrder.id_purchaseReason = itemPO.id_purchaseReason;
                            tempPurchaseOrder.id_shippingType = itemPO.id_shippingType;
                            tempPurchaseOrder.isImportation = itemPO.isImportation;
                            tempPurchaseOrder.requiredLogistic = itemPO.requiredLogistic;
                            tempPurchaseOrder.sendTo = itemPO.sendTo ?? "";
                            tempPurchaseOrder.descriptionDocPO = itemPO.descriptionDocPO;
                            tempPurchaseOrder.id_certification = itemPO.id_certification;
                            #endregion

                            #region"Transaccion"

                            if (tempPurchaseOrder?.PurchaseOrderDetailByGrammage != null)
                            {
                                var details = tempPurchaseOrder.PurchaseOrderDetailByGrammage.ToList();

                                //List<int> listProcesstype = new List<int>();

                                //listProcesstype = (from hr in db.PriceListDetail
                                //                   join de in db.ItemProcessType on hr.id_item equals de.Id_Item
                                //                   where hr.id_priceList == itemPO.id_priceList
                                //                   select de.Id_ProcessType).ToList();

                                if (itemPO.pricePerList)
                                {
                                    if (itemPO.id_priceList == null || itemPO.id_priceList <= 0)
                                    {
                                        TempData.Keep("purchaseOrder");
                                        ViewData["EditMessage"] = ErrorMessage("Seleccione la Lista de Precio.");
                                        return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                    }
                                }



                                foreach (var detail in details)
                                {
                                    PurchaseOrderDetailByGrammage orderDetail = modelItem.PurchaseOrderDetailByGrammage.FirstOrDefault(d => d.id == detail.id);

                                    var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                                    if (approve && detail.quantityApproved <= 0)
                                    {
                                        TempData.Keep("purchaseOrder");
                                        ViewData["EditMessage"] = ErrorMessage("No se puede aprobar la orden de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada mayor que cero.");
                                        return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                    }

                                    if (detail.isActive)
                                    {
                                        if (itemPO.pricePerList)
                                        {

                                            //if (listProcesstype != null && listProcesstype.Count > 0)
                                            //{
                                            //var isExists = (from hr in db.ItemProcessType
                                            //                where hr.Id_Item == detail.id_item
                                            //                select hr.Id_ProcessType).ToList().Count;

                                            //if (isExists <= 0)
                                            //{

                                            //    TempData.Keep("purchaseOrder");
                                            //    ViewData["EditMessage"] = ErrorMessage("El Producto no tiene el mismo Tipo de la Lista de Precio");
                                            //    return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                            //}

                                            //}
                                        }

                                        if (detail.id_Grammage == null || detail.id_Grammage < 0)
                                        {

                                            TempData.Keep("purchaseOrder");
                                            ViewData["EditMessage"] = ErrorMessage("Existe detalle sin Gramaje.");
                                            return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                        }

                                    }

                                    if (orderDetail == null)
                                    {
                                        orderDetail = new PurchaseOrderDetailByGrammage
                                        {
                                            id_item = detail.id_item,
                                            Item = itemAux,

                                            quantityRequested = detail.quantityRequested,
                                            quantityOrdered = detail.quantityOrdered,
                                            quantityApproved = detail.quantityApproved,
                                            quantityReceived = detail.quantityReceived,
                                            productionUnitProviderPoolreference = detail.productionUnitProviderPoolreference,
                                            price = detail.price,
                                            iva = detail.iva,

                                            subtotal = detail.subtotal,
                                            total = detail.total,

                                            isActive = detail.isActive,
                                            id_userCreate = detail.id_userCreate,
                                            dateCreate = detail.dateCreate,
                                            id_userUpdate = detail.id_userUpdate,
                                            dateUpdate = detail.dateUpdate,
                                            id_Grammage = detail.id_Grammage,

                                            PurchaseOrderDetailByGrammagePurchaseRequest = new List<PurchaseOrderDetailByGrammagePurchaseRequest>()
                                        };

                                        foreach (var requestDetail in detail.PurchaseOrderDetailByGrammagePurchaseRequest)
                                        {
                                            if (requestDetail.PurchaseRequestDetail.quantityOutstandingPurchase < detail.quantityApproved)
                                            {
                                                TempData.Keep("purchaseOrder");
                                                ViewData["EditMessage"] = ErrorMessage("No se puede aprobar la orden de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada menor e igual a la Cantidad Requerida.");
                                                return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                            }
                                            orderDetail.PurchaseOrderDetailByGrammagePurchaseRequest.Add(new PurchaseOrderDetailByGrammagePurchaseRequest
                                            {
                                                id_purchaseRequest = requestDetail.id_purchaseRequest,
                                                id_purchaseRequestDetail = requestDetail.id_purchaseRequestDetail,
                                                quantity = detail.quantityApproved
                                            });
                                        }

                                        if (orderDetail.isActive)
                                        {
                                            modelItem.PurchaseOrderDetailByGrammage.Add(orderDetail);
                                            count++;
                                        }

                                    }
                                    else
                                    {
                                        orderDetail.id_item = detail.id_item;
                                        orderDetail.Item = db.Item.FirstOrDefault(i => i.id == detail.id_item);

                                        orderDetail.quantityRequested = detail.quantityRequested;
                                        orderDetail.quantityOrdered = detail.quantityOrdered;
                                        orderDetail.quantityApproved = detail.quantityApproved;
                                        orderDetail.quantityReceived = detail.quantityReceived;

                                        orderDetail.price = detail.price;
                                        orderDetail.iva = detail.iva;
                                        orderDetail.productionUnitProviderPoolreference = detail.productionUnitProviderPoolreference;
                                        orderDetail.subtotal = detail.subtotal;
                                        orderDetail.total = detail.total;

                                        orderDetail.isActive = detail.isActive;
                                        orderDetail.id_userCreate = detail.id_userCreate;
                                        orderDetail.dateCreate = detail.dateCreate;
                                        orderDetail.id_userUpdate = detail.id_userUpdate;
                                        orderDetail.dateUpdate = detail.dateUpdate;
                                        orderDetail.id_Grammage = detail.id_Grammage;

                                        for (int j = orderDetail.PurchaseOrderDetailByGrammagePurchaseRequest.Count - 1; j >= 0; j--)
                                        {
                                            var detailPurchaseOrderDetailByGrammagePurchaseRequest = orderDetail.PurchaseOrderDetailByGrammagePurchaseRequest.ElementAt(j);
                                            orderDetail.PurchaseOrderDetailByGrammagePurchaseRequest.Remove(detailPurchaseOrderDetailByGrammagePurchaseRequest);
                                            db.Entry(detailPurchaseOrderDetailByGrammagePurchaseRequest).State = EntityState.Deleted;
                                        }

                                        orderDetail.PurchaseOrderDetailByGrammagePurchaseRequest = new List<PurchaseOrderDetailByGrammagePurchaseRequest>();
                                        foreach (var requestDetail in detail.PurchaseOrderDetailByGrammagePurchaseRequest)
                                        {
                                            if (requestDetail.PurchaseRequestDetail.quantityOutstandingPurchase < detail.quantityApproved)
                                            {
                                                TempData.Keep("purchaseOrder");
                                                ViewData["EditMessage"] = ErrorMessage("No se puede aprobar la orden de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada menor e igual a la Cantidad Requerida.");
                                                return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                            }
                                            orderDetail.PurchaseOrderDetailByGrammagePurchaseRequest.Add(new PurchaseOrderDetailByGrammagePurchaseRequest
                                            {
                                                id_purchaseRequest = requestDetail.id_purchaseRequest,
                                                id_purchaseRequestDetail = requestDetail.id_purchaseRequestDetail,
                                                quantity = detail.quantityApproved
                                            });
                                        }

                                        if (orderDetail.isActive)
                                            count++;
                                    }
                                }

                                // UPDATE TOTALS
                                modelItem.PurchaseOrderTotal = PurchaseOrderTotalsBG(modelItem.id, details);
                            }

                            #region"Aqui agrupo los detalles Falsos de Orden de Compra"

                            var lstIdItems = modelItem.PurchaseOrderDetailByGrammage.Select(s => s.id_item).Distinct().ToList();
                            if (lstIdItems != null && lstIdItems.Count > 0)
                            {
                                foreach (var i in lstIdItems)
                                {
                                    //Obtengo la lista de items en el detalle con cantidades
                                    var itNlst = modelItem
                                                    .PurchaseOrderDetailByGrammage
                                                    .Where(w => w.id_item == i && w.isActive)
                                                    .Select(s => new
                                                    {
                                                        s.id_item,
                                                        valGrammage = DataProviderGrammage.GrammageById(s.id_Grammage)?.value ?? 0,
                                                        s.quantityApproved,
                                                        s.quantityDispatched,
                                                        s.quantityOrdered,
                                                        s.quantityReceived,
                                                        s.quantityRequested,
                                                        s.price,
                                                        s.iva,
                                                        s.subtotal,
                                                        s.total,
                                                    }).ToList();
                                    //A partir de esta lista obtengo lista y gramajes
                                    if (itNlst != null && itNlst.Count > 0)
                                    {
                                        var iNLstFn = itNlst
                                                        .GroupBy(t => new { id = t.id_item })
                                                        .Select(g => new
                                                        {
                                                            ID = g.Key.id,
                                                            AvGrama = g.Average(p => p.valGrammage),
                                                            SuQuApproved = g.Sum(p => p.quantityApproved),
                                                            SuQuDispatched = g.Sum(p => p.quantityDispatched),
                                                            SuQuOrdered = g.Sum(p => p.quantityOrdered),
                                                            SuQuReceived = g.Sum(p => p.quantityReceived),
                                                            SuQuRequested = g.Sum(p => p.quantityRequested),
                                                            Avprice = g.Average(p => p.price),
                                                            Aviva = g.Average(p => p.iva),
                                                            Susubtotal = g.Sum(p => p.subtotal),
                                                            SuTotal = g.Sum(p => p.total)
                                                        }).ToList();


                                        // Si esta lista tiene un solo elemento, lo agrego o lo actualizo
                                        // Esto depende si existe o no el item en PurchaseOrderDetail

                                        if (iNLstFn != null && iNLstFn.Count == 1)
                                        {
                                            int id_g = 0;
                                            //Obtengo este único elemento
                                            var objF = iNLstFn.FirstOrDefault();

                                            if (objF != null)
                                            {
                                                //Obtengo el primer gramaje que sea aproximado
                                                //al valor promedio, por defecto traigo el cualquier gramaje
                                                id_g = db.Grammage
                                                            .FirstOrDefault(fod => fod.value >= objF.AvGrama)?.id ?? db.Grammage.FirstOrDefault().id;

                                            }

                                            var podTmp = modelItem.PurchaseOrderDetail
                                                                    .FirstOrDefault(fod => fod.id_item == objF.ID);
                                            if (podTmp == null)
                                            {
                                                var tempItemPODetailNew = new PurchaseOrderDetail
                                                {
                                                    id_item = objF.ID,
                                                    quantityRequested = objF.SuQuRequested,
                                                    quantityOrdered = objF.SuQuOrdered,
                                                    quantityApproved = objF.SuQuApproved,
                                                    quantityReceived = objF.SuQuReceived,
                                                    productionUnitProviderPoolreference = itemPO.PurchaseOrderDetailByGrammage.FirstOrDefault(fod => fod.id_item == objF.ID)?.productionUnitProviderPoolreference ?? "",

                                                    price = objF.Avprice,
                                                    iva = objF.Aviva,
                                                    subtotal = objF.Susubtotal,
                                                    total = objF.SuTotal,

                                                    isActive = true,
                                                    id_userCreate = ActiveUser.id,
                                                    dateCreate = DateTime.Now,
                                                    id_userUpdate = ActiveUser.id,
                                                    dateUpdate = DateTime.Now,
                                                    id_Grammage = id_g
                                                };
                                                modelItem.PurchaseOrderDetail.Add(tempItemPODetailNew);
                                            }
                                            else
                                            {
                                                podTmp.quantityRequested = objF.SuQuRequested;
                                                podTmp.quantityOrdered = objF.SuQuOrdered;
                                                podTmp.quantityApproved = objF.SuQuApproved;
                                                podTmp.quantityReceived = objF.SuQuReceived;

                                                var ls = modelItem.PurchaseOrderDetailByGrammage.Where(w => w.id_item == objF.ID).ToList();
                                                podTmp.productionUnitProviderPoolreference = "";
                                                foreach (var det in ls)
                                                {
                                                    podTmp.productionUnitProviderPoolreference += det.productionUnitProviderPoolreference + ", ";
                                                }

                                                podTmp.price = objF.Avprice; podTmp.iva = objF.Aviva;
                                                podTmp.subtotal = objF.Susubtotal; podTmp.total = objF.SuTotal;

                                                podTmp.isActive = true;
                                                podTmp.id_userUpdate = ActiveUser.id;
                                                podTmp.dateUpdate = DateTime.Now;
                                                podTmp.id_Grammage = id_g;
                                            }



                                        }
                                    }

                                }
                            }
                            #endregion

                            #endregion

                            #endregion
                        }
                        else
                        {
                            #region PURCHASE ORDER DETAILS

                            #region  Carga Info al Temporal
                            tempPurchaseOrder.deliveryDate = itemPO.deliveryDate;
                            tempPurchaseOrder.deliveryhour = itemPO.deliveryhour;
                            if (!string.IsNullOrEmpty(delivery)) tempPurchaseOrder.deliveryhour = TimeSpan.Parse(delivery);

                            tempPurchaseOrder.deliveryTo = itemPO.deliveryTo ?? "";
                            tempPurchaseOrder.Document = modelItem.Document;
                            tempPurchaseOrder.Employee = itemPO.Employee;
                            tempPurchaseOrder.FishingSite = itemPO.FishingSite;
                            tempPurchaseOrder.id_buyer = itemPO.id_buyer;
                            tempPurchaseOrder.id_FishingSite = itemPO.id_FishingSite;
                            tempPurchaseOrder.id_paymentMethod = itemPO.id_paymentMethod;
                            tempPurchaseOrder.id_paymentTerm = itemPO.id_paymentTerm;
                            tempPurchaseOrder.id_personRequesting = itemPO.id_personRequesting;
                            tempPurchaseOrder.id_priceList = itemPO.id_priceList;
                            tempPurchaseOrder.id_productionUnitProvider = itemPO.id_productionUnitProvider;
                            tempPurchaseOrder.id_productionUnitProviderProtective = itemPO.id_productionUnitProviderProtective;
                            tempPurchaseOrder.id_provider = itemPO.id_provider;
                            tempPurchaseOrder.id_providerapparent = itemPO.id_provider;
                            tempPurchaseOrder.id_purchaseReason = itemPO.id_purchaseReason;
                            tempPurchaseOrder.id_shippingType = itemPO.id_shippingType;
                            tempPurchaseOrder.isImportation = itemPO.isImportation;
                            tempPurchaseOrder.requiredLogistic = itemPO.requiredLogistic;
                            tempPurchaseOrder.sendTo = itemPO.sendTo ?? "";
                            tempPurchaseOrder.descriptionDocPO = itemPO.descriptionDocPO;
                            tempPurchaseOrder.id_certification = itemPO.id_certification;
                            #endregion

                            #region"Transaccion"

                            if (tempPurchaseOrder?.PurchaseOrderDetail != null)
                            {
                                var details = tempPurchaseOrder.PurchaseOrderDetail.ToList();

                                List<int> listProcesstype = new List<int>();

                                listProcesstype = (from hr in db.PriceListDetail
                                                   join de in db.ItemProcessType on hr.id_item equals de.Id_Item
                                                   where hr.id_priceList == itemPO.id_priceList
                                                   select de.Id_ProcessType).ToList();

                                if (itemPO.pricePerList)
                                {
                                    if (itemPO.id_priceList == null || itemPO.id_priceList <= 0)
                                    {
                                        TempData.Keep("purchaseOrder");
                                        ViewData["EditMessage"] = ErrorMessage("Seleccione la Lista de Precio.");
                                        return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                    }
                                }



                                foreach (var detail in details)
                                {
                                    PurchaseOrderDetail orderDetail = modelItem.PurchaseOrderDetail.FirstOrDefault(d => d.id == detail.id);
                                    var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                                    if (approve && detail.quantityApproved <= 0)
                                    {
                                        TempData.Keep("purchaseOrder");
                                        ViewData["EditMessage"] = ErrorMessage("No se puede aprobar la orden de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada mayor que cero.");
                                        return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                    }

                                    if (detail.isActive)
                                    {
                                        if (itemPO.pricePerList)
                                        {

                                            if (listProcesstype != null && listProcesstype.Count > 0)
                                            {
                                                //var isExists = (from hr in db.ItemProcessType
                                                //                where hr.Id_Item == detail.id_item && listProcesstype.Contains(hr.Id_ProcessType)
                                                //                select hr.Id_ProcessType).ToList().Count;

                                                //if (isExists <= 0)
                                                //{

                                                //    TempData.Keep("purchaseOrder");
                                                //    ViewData["EditMessage"] = ErrorMessage("El Producto no tiene el mismo Tipo de la Lista de Precio");
                                                //    return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                                //}

                                            }
                                        }

                                        if (detail.id_Grammage == null || detail.id_Grammage < 0)
                                        {

                                            TempData.Keep("purchaseOrder");
                                            ViewData["EditMessage"] = ErrorMessage("Existe detalle sin Gramaje.");
                                            return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                                        }

                                    }

                                    if (orderDetail == null)
                                    {
                                        orderDetail = new PurchaseOrderDetail
                                        {
                                            id_item = detail.id_item,
                                            Item = itemAux,

                                            quantityRequested = detail.quantityRequested,
                                            quantityOrdered = detail.quantityOrdered,
                                            quantityApproved = detail.quantityApproved,
                                            quantityReceived = detail.quantityReceived,
                                            productionUnitProviderPoolreference = detail.productionUnitProviderPoolreference,
                                            price = detail.price,
                                            iva = detail.iva,

                                            subtotal = detail.subtotal,
                                            total = detail.total,

                                            isActive = detail.isActive,
                                            id_userCreate = detail.id_userCreate,
                                            dateCreate = detail.dateCreate,
                                            id_userUpdate = detail.id_userUpdate,
                                            dateUpdate = detail.dateUpdate,
                                            id_Grammage = detail.id_Grammage

                                        };

                                        if (orderDetail.isActive)
                                        {
                                            modelItem.PurchaseOrderDetail.Add(orderDetail);
                                            count++;
                                        }

                                    }
                                    else
                                    {
                                        orderDetail.id_item = detail.id_item;
                                        orderDetail.Item = db.Item.FirstOrDefault(i => i.id == detail.id_item);

                                        orderDetail.quantityRequested = detail.quantityRequested;
                                        orderDetail.quantityOrdered = detail.quantityOrdered;
                                        orderDetail.quantityApproved = detail.quantityApproved;
                                        orderDetail.quantityReceived = detail.quantityReceived;

                                        orderDetail.price = detail.price;
                                        orderDetail.iva = detail.iva;
                                        orderDetail.productionUnitProviderPoolreference = detail.productionUnitProviderPoolreference;
                                        orderDetail.subtotal = detail.subtotal;
                                        orderDetail.total = detail.total;

                                        orderDetail.isActive = detail.isActive;
                                        orderDetail.id_userCreate = detail.id_userCreate;
                                        orderDetail.dateCreate = detail.dateCreate;
                                        orderDetail.id_userUpdate = detail.id_userUpdate;
                                        orderDetail.dateUpdate = detail.dateUpdate;
                                        orderDetail.id_Grammage = detail.id_Grammage;
                                        foreach (var requestDetail in orderDetail.PurchaseOrderDetailPurchaseRequest)
                                        {
                                            requestDetail.quantity = detail.quantityApproved;
                                            db.PurchaseOrderDetailPurchaseRequest.Attach(requestDetail);
                                            db.Entry(requestDetail).State = EntityState.Modified;
                                        }

                                        if (orderDetail.isActive)
                                            count++;
                                    }
                                }
                            }
                            #endregion

                            #endregion
                        }

                        #region IMPORTATION INFORMATION

                        if (itemPO.isImportation)
                        {
                            if (modelItem.isImportation && modelItem.PurchaseOrderImportationInformation != null)
                            {
                                modelItem.PurchaseOrderImportationInformation.customsDocumentNumber = importationInformation.customsDocumentNumber;
                                modelItem.PurchaseOrderImportationInformation.referendumNumber = importationInformation.referendumNumber;
                                modelItem.PurchaseOrderImportationInformation.shipmentDate = FormatDate(importationInformation.shipmentDate, Request.UserLanguages);
                                modelItem.PurchaseOrderImportationInformation.departureCustomsDate = FormatDate(importationInformation.departureCustomsDate, Request.UserLanguages);
                                modelItem.PurchaseOrderImportationInformation.arrivalDate = FormatDate(importationInformation.arrivalDate, Request.UserLanguages);

                                db.Entry(modelItem.PurchaseOrderImportationInformation).State = EntityState.Modified;

                            }
                            else
                            {
                                importationInformation.shipmentDate = FormatDate(importationInformation.shipmentDate, Request.UserLanguages);
                                importationInformation.departureCustomsDate = FormatDate(importationInformation.departureCustomsDate, Request.UserLanguages);
                                importationInformation.arrivalDate = FormatDate(importationInformation.arrivalDate, Request.UserLanguages);
                                modelItem.PurchaseOrderImportationInformation = importationInformation;
                            }
                        }
                        else
                        {
                            if (modelItem.isImportation)
                            {
                                db.PurchaseOrderImportationInformation.Remove(modelItem.PurchaseOrderImportationInformation);
                                db.Entry(modelItem.PurchaseOrderImportationInformation).State = EntityState.Deleted;
                            }
                        }

                        #endregion

                        if (count == 0)
                        {
                            TempData.Keep("purchaseOrder");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una orden de compra sin detalles");
                            return PartialView("_PurchaseOrderMainFormPartial", tempPurchaseOrder);
                        }

                        if (approve)
                        {
                            foreach (var detail in itemPO.PurchaseOrderDetailByGrammage)
                            {
                                foreach (var purchaseOrderDetailByGrammagePurchaseRequest in detail.PurchaseOrderDetailByGrammagePurchaseRequest)
                                {
                                    ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseOrderDetailByGrammage,
                                                                                   purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseRequestDetail,
                                                                                   purchaseOrderDetailByGrammagePurchaseRequest.quantity);
                                }
                            }
                            //foreach (var detail in modelItem.PurchaseOrderDetail)
                            //{
                            //    foreach (var purchaseOrderDetailPurchaseRequest in detail.PurchaseOrderDetailPurchaseRequest)
                            //    {
                            //        ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailPurchaseRequest.id_purchaseOrderDetail,
                            //                                                       purchaseOrderDetailPurchaseRequest.id_purchaseRequestDetail,
                            //                                                       purchaseOrderDetailPurchaseRequest.quantity);
                            //    }
                            //}

                            modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA


                        }

                        modelItem.descriptionDocPO = modelItem.Document.description;
                        modelItem.id_personProcessPlant = itemPO.id_personProcessPlant;


                        extraInfoPO = JsonConvert.SerializeObject(new 
                        {
                            PurchaseOrderImportationInformation =  new 
                            {
                                id = modelItem.PurchaseOrderImportationInformation?.id
                                ,customsDocumentNumber = modelItem.PurchaseOrderImportationInformation?.customsDocumentNumber
                                ,referendumNumber = modelItem.PurchaseOrderImportationInformation?.referendumNumber
                                ,shipmentDate = modelItem.PurchaseOrderImportationInformation?.shipmentDate
                                ,departureCustomsDate = modelItem.PurchaseOrderImportationInformation?.departureCustomsDate
                                ,arrivalDate = modelItem.PurchaseOrderImportationInformation?.arrivalDate
                            },
                            PurchaseOrderDetailByGrammage = modelItem.PurchaseOrderDetailByGrammage?.Select(r=> new {
                                id = r.id,
                                dateCreate = r.dateCreate,
                                dateUpdate = r.dateUpdate,

                            })?.ToArray(),
                            PurchaseOrderDetail = modelItem.PurchaseOrderDetail?.Select(r=> new {
                                dateCreate = r.dateCreate,
                                dateUpdate = r.dateUpdate,  
                            })?.ToArray(),
                            PurchaseOrder = new 
                            {
                                deliveryDate = modelItem.deliveryDate,
                                deliveryhour = modelItem.deliveryhour,

                            }},  Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
                        { 
                            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        });

                        db.PurchaseOrder.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        //_answer = 1;
                        TempData["purchaseOrder"] = modelItem;
                        TempData.Keep("purchaseOrder");

                        ViewData["EditMessage"] = SuccessMessage("Orden de Compra: " + modelItem.Document.number + " guardada exitosamente");
                    }
                }
                catch (Exception e)
                {
                    MetodosEscrituraLogs.EscribeExcepcionLogNest(e, ruta, "PurchaseOrder", "PROD","update", extraInfoPO);
                    TempData.Keep("purchaseOrder");
                    ViewData["EditMessage"] = ErrorMessage();
                    trans.Rollback();
                }
            }

            #region AUDITORIA DE ORDEN COMPRA

            #endregion

            return PartialView("_PurchaseOrderMainFormPartial", modelItem);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrdersPartialDelete(System.Int32 id)
        {
            TempData.Keep("model");
            List<PurchaseOrder> _PurchaseOrder = null;
            var model = db.PurchaseOrder;
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


            _PurchaseOrder = model.ToList();

            tbsysUserRecordSecurity UserRecordSecurity = db.tbsysUserRecordSecurity.FirstOrDefault(r => r.id_user == ActiveUser.id && r.tbsysObjSecurityRecord.obj == "PurchaseOrder");
            if (UserRecordSecurity != null)
            {
                _PurchaseOrder = model.Where(r => r.id_buyer == ActiveUser.Employee.id).ToList();

            }

            return PartialView("_PurchaseOrdersPartial", _PurchaseOrder);
        }

        #endregion

        #region PURCHASE ORDER DETAILS

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartial()
        {
            TempData.Keep("model");
            PurchaseOrder purchaseOrder = (PurchaseOrder)TempData["purchaseOrder"];

            var model = (purchaseOrder != null) ? purchaseOrder.PurchaseOrderDetail.Where(d => d.isActive).ToList() : new List<PurchaseOrderDetail>();

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            return PartialView("_PurchaseOrderDetailsPartial", model.ToList());
        }

        //By Grammage 
        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartialBG()
        {
            TempData.Keep("model");
            //TempData.Keep("id_provider");
            PurchaseOrder purchaseOrder = (PurchaseOrder)TempData["purchaseOrder"];
            var id_productionUnitProvider = purchaseOrder.id_productionUnitProvider;
            var model = (purchaseOrder != null) ? purchaseOrder.PurchaseOrderDetailByGrammage.Where(d => d.isActive).ToList() : new List<PurchaseOrderDetailByGrammage>();

            TempData["purchaseOrder"] = purchaseOrder;
            //TempData["id_provider"] = id_productionUnitProvider;
            TempData.Keep("purchaseOrder");
            //TempData.Keep("id_provider");
            return PartialView("_PurchaseOrderDetailsPartialBG", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartialAddNew(PurchaseOrderDetail orderDetail)
        {
            TempData.Keep("model");
            PurchaseOrder order = (TempData["purchaseOrder"] as PurchaseOrder);
            order = order ?? db.PurchaseOrder.FirstOrDefault(i => i.id == order.id);
            order = order ?? new PurchaseOrder();

            if (ModelState.IsValid)
            {
                try
                {
                    orderDetail.id = order.PurchaseOrderDetail.Count() > 0 ? order.PurchaseOrderDetail.Max(pld => pld.id) + 1 : 1;

                    orderDetail.id_purchaseOrder = order.id;
                    orderDetail.Item = db.Item.FirstOrDefault(i => i.id == orderDetail.id_item);

                    orderDetail.isActive = true;
                    orderDetail.id_userCreate = ActiveUser.id;
                    orderDetail.dateCreate = DateTime.Now;
                    orderDetail.id_userUpdate = ActiveUser.id;
                    orderDetail.dateUpdate = DateTime.Now;

                    order.PurchaseOrderDetail.Add(orderDetail);

                    order.PurchaseOrderTotal = PurchaseOrderTotals(order.id, order.PurchaseOrderDetail.ToList());
                    TempData["purchaseOrder"] = order;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;

                }
            }
            else


                TempData.Keep("purchaseOrder");

            var model = order?.PurchaseOrderDetail.Where(d => d.isActive).ToList() ?? new List<PurchaseOrderDetail>();
            return PartialView("_PurchaseOrderDetailsPartial", model.ToList());
        }

        //BY GRAMMAGE
        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartialAddNewBG(PurchaseOrderDetailByGrammage orderDetail)
        {
            TempData.Keep("model");
            //TempData.Keep("id_provider");
            PurchaseOrder order = (TempData["purchaseOrder"] as PurchaseOrder);
            order = order ?? db.PurchaseOrder.FirstOrDefault(i => i.id == order.id);
            order = order ?? new PurchaseOrder();

            var parameter = db.Setting.ToList();
            var productionUnitProviderPool = db.ProductionUnitProviderPool.ToList();
            var parameterSSP = parameter.Where(d => d.code == "SPP").Select(x => x.value).FirstOrDefault();
            var msg = "";
            var id_productionUnitProvider = order.id_productionUnitProvider;
            if (ModelState.IsValid)
            {
                try
                {
                    orderDetail.id = order.PurchaseOrderDetailByGrammage.Count() > 0 ? order.PurchaseOrderDetailByGrammage.Max(pld => pld.id) + 1 : 1;

                    orderDetail.id_purchaseOrder = order.id;
                    orderDetail.Item = db.Item.FirstOrDefault(i => i.id == orderDetail.id_item);
                    orderDetail.Grammage = db.Grammage.FirstOrDefault(i => i.id == orderDetail.id_Grammage);

                    var id_purchaseRequestDetail = int.Parse(string.IsNullOrEmpty(Request.Params["id_purchaseRequestDetailValue"]) ? "0" : Request.Params["id_purchaseRequestDetailValue"]);
                    var aPurchaseRequestDetail = db.PurchaseRequestDetail.FirstOrDefault(fod => fod.id == id_purchaseRequestDetail);
                    if (aPurchaseRequestDetail != null)
                    {
                        PurchaseOrderDetailByGrammagePurchaseRequest newPurchaseOrderDetailByGrammagePurchaseRequest = new PurchaseOrderDetailByGrammagePurchaseRequest
                        {
                            id_purchaseOrderDetailByGrammage = orderDetail.id,
                            id_purchaseRequest = aPurchaseRequestDetail.id_purchaseRequest,
                            PurchaseRequest = aPurchaseRequestDetail.PurchaseRequest,
                            id_purchaseRequestDetail = aPurchaseRequestDetail.id,
                            PurchaseRequestDetail = aPurchaseRequestDetail,
                            quantity = aPurchaseRequestDetail.quantityOutstandingPurchase
                        };
                        orderDetail.PurchaseOrderDetailByGrammagePurchaseRequest = new List<PurchaseOrderDetailByGrammagePurchaseRequest>();
                        orderDetail.PurchaseOrderDetailByGrammagePurchaseRequest.Add(newPurchaseOrderDetailByGrammagePurchaseRequest);
                    }

                    orderDetail.isActive = true;
                    orderDetail.id_userCreate = ActiveUser.id;
                    orderDetail.dateCreate = DateTime.Now;
                    orderDetail.id_userUpdate = ActiveUser.id;
                    orderDetail.dateUpdate = DateTime.Now;

                    if (parameterSSP == "SI")
                    {
                        if (orderDetail.productionUnitProviderPoolreference != null)
                        {
                            var certification = productionUnitProviderPool.FirstOrDefault(p => p.id_productionUnitProvider == id_productionUnitProvider
                        && p.name == orderDetail.productionUnitProviderPoolreference);

                            if (order.id_certification.HasValue && certification.id_certification == null)
                            {
                                msg = "PISCINA DEBE TENER ASC";
                            }

                            if (order.id_certification == null && certification.id_certification.HasValue)
                            {
                                msg = "PISCINA TIENE ASC";
                            }
                        }
                        else
                        {
                            msg = "Debe seleccionar una piscina ";
                        }
                        

                    }
                    if (parameterSSP == "SI" && msg == "")
                    {
                        order.PurchaseOrderDetailByGrammage.Add(orderDetail);
                    }

                    if (parameterSSP == "NO")
                    {
                        order.PurchaseOrderDetailByGrammage.Add(orderDetail);
                    }

                    order.PurchaseOrderTotal = PurchaseOrderTotalsBG(order.id, order.PurchaseOrderDetailByGrammage.ToList());
                    TempData["purchaseOrder"] = order;
                    //TempData["id_provider"] = order.id_productionUnitProvider;
                    
                    if (parameterSSP == "SI" && msg != "")
                    {
                        //throw new Exception(msg);
                        ViewData["EditError"] = msg;
                    }

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;

                }
            }
            


                TempData.Keep("purchaseOrder");
                //TempData.Keep("id_provider");

            var model = order?.PurchaseOrderDetailByGrammage.Where(d => d.isActive).ToList() ?? new List<PurchaseOrderDetailByGrammage>();
            return PartialView("_PurchaseOrderDetailsPartialBG", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult OrderTotals()
        {
            TempData.Keep("model");
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            purchaseOrder = purchaseOrder ?? new PurchaseOrder();
            purchaseOrder.PurchaseOrderDetail = purchaseOrder.PurchaseOrderDetail ?? new List<PurchaseOrderDetail>();

            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder.id, purchaseOrder.PurchaseOrderDetail.ToList());

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            var result = new
            {
                orderSubtotal = purchaseOrder.PurchaseOrderTotal.subtotal,
                orderSubtotalIVA12Percent = purchaseOrder.PurchaseOrderTotal.subtotalIVA12Percent,
                orderTotalIVA12 = purchaseOrder.PurchaseOrderTotal.totalIVA12,
                orderDiscount = purchaseOrder.PurchaseOrderTotal.discount,
                orderSubtotalIVA14Percent = purchaseOrder.PurchaseOrderTotal.subtotalIVA14Percent,
                orderTotalIVA14 = purchaseOrder.PurchaseOrderTotal.totalIVA14,
                orderTotal = purchaseOrder.PurchaseOrderTotal.total
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult OrderTotalsBG()
        {
            TempData.Keep("model");
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            purchaseOrder = purchaseOrder ?? new PurchaseOrder();
            purchaseOrder.PurchaseOrderDetailByGrammage = purchaseOrder.PurchaseOrderDetailByGrammage ?? new List<PurchaseOrderDetailByGrammage>();

            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder.id, purchaseOrder.PurchaseOrderDetailByGrammage.ToList());

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            var result = new
            {
                orderSubtotal = purchaseOrder.PurchaseOrderTotal.subtotal,
                orderSubtotalIVA12Percent = purchaseOrder.PurchaseOrderTotal.subtotalIVA12Percent,
                orderTotalIVA12 = purchaseOrder.PurchaseOrderTotal.totalIVA12,
                orderDiscount = purchaseOrder.PurchaseOrderTotal.discount,
                orderSubtotalIVA14Percent = purchaseOrder.PurchaseOrderTotal.subtotalIVA14Percent,
                orderTotalIVA14 = purchaseOrder.PurchaseOrderTotal.totalIVA14,
                orderTotal = purchaseOrder.PurchaseOrderTotal.total
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult PurchaseOrderValidatePriceDetails(int? id_priceList, int? id_item, decimal? price)
        {
            string smensaje = "";
            if (price != null && price > 0)
            {
                PurchaseOrder order = (TempData["purchaseOrder"] as PurchaseOrder);

                order = order ?? db.PurchaseOrder.FirstOrDefault(i => i.id == order.id);
                order = order ?? new PurchaseOrder();

                if (id_priceList != null && id_priceList > 0)
                {

                    var oPriceList = db.PriceList.Where(X => X.id == id_priceList).FirstOrDefault();

                    #region validacion de Limite de la lista de Precio
                    try
                    {
                        string PIngremento = db.Setting.Where(X => X.code.Equals("PLLP")).FirstOrDefault().value;

                        if (!string.IsNullOrEmpty(PIngremento))
                        {
                            decimal Dingemento = decimal.Parse(PIngremento) / 100;

                            if (Dingemento > 0 && oPriceList != null)
                            {
                                Decimal dPricelist = oPriceList.PriceListDetail.Where(X => X.id_item == id_item).FirstOrDefault()?.purchasePrice ?? 0;

                                if (dPricelist > 0)
                                {
                                    if (price > (dPricelist + (dPricelist * Dingemento)))
                                    {
                                        smensaje = "Precio: No Debe ser mayor al Incremento Permitido.";

                                    }

                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        smensaje = e.Message;

                    }
                    #endregion


                }
            }
            TempData.Keep("purchaseOrder");

            var result = new
            {
                mensaje = smensaje
            };

            return Json(result, JsonRequestBehavior.AllowGet);




        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartialUpdate(PurchaseOrderDetail orderDetail)
        {
            PurchaseOrder order = (TempData["purchaseOrder"] as PurchaseOrder);

            order = order ?? db.PurchaseOrder.FirstOrDefault(i => i.id == order.id);
            order = order ?? new PurchaseOrder();


            if (ModelState.IsValid)
            {

                try
                {
                    //var modelItem = order.PurchaseOrderDetail.FirstOrDefault(it => it.id_item == orderDetail.id_item);
                    var modelItem = order.PurchaseOrderDetail.FirstOrDefault(it => it.id == orderDetail.id);
                    if (modelItem != null)
                    {
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateCreate = DateTime.Now;

                        this.UpdateModel(modelItem);

                        order.PurchaseOrderTotal = PurchaseOrderTotals(order.id, order.PurchaseOrderDetail.ToList());
                        TempData["purchaseOrder"] = order;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }

            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("purchaseOrder");

            var model = order?.PurchaseOrderDetail.Where(d => d.isActive).ToList() ?? new List<PurchaseOrderDetail>();

            return PartialView("_PurchaseOrderDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartialUpdateBG(PurchaseOrderDetailByGrammage orderDetail)
        {
            PurchaseOrder order = (TempData["purchaseOrder"] as PurchaseOrder);
            //TempData.Keep("id_provider");


            order = order ?? db.PurchaseOrder.FirstOrDefault(i => i.id == order.id);
            order = order ?? new PurchaseOrder();
            
            var parameter = db.Setting.ToList();
            var productionUnitProviderPool = db.ProductionUnitProviderPool.ToList();
            var parameterSSP = parameter.Where(d => d.code == "SPP").Select(x => x.value).FirstOrDefault();
            var msg = "";
            var id_productionUnitProvider = order.id_productionUnitProvider;

            if (ModelState.IsValid)
            {

                try
                {
                    //var modelItem = order.PurchaseOrderDetail.FirstOrDefault(it => it.id_item == orderDetail.id_item);
                    var modelItem = order.PurchaseOrderDetailByGrammage.FirstOrDefault(it => it.id == orderDetail.id);
                    if (modelItem != null)
                    {
                        modelItem.PurchaseOrderDetailByGrammagePurchaseRequest = new List<PurchaseOrderDetailByGrammagePurchaseRequest>();
                        var id_purchaseRequestDetail = int.Parse(string.IsNullOrEmpty(Request.Params["id_purchaseRequestDetailValue"]) ? "0" : Request.Params["id_purchaseRequestDetailValue"]);
                        var aPurchaseRequestDetail = db.PurchaseRequestDetail.FirstOrDefault(fod => fod.id == id_purchaseRequestDetail);
                        if (aPurchaseRequestDetail != null)
                        {
                            PurchaseOrderDetailByGrammagePurchaseRequest newPurchaseOrderDetailByGrammagePurchaseRequest = new PurchaseOrderDetailByGrammagePurchaseRequest
                            {
                                id_purchaseOrderDetailByGrammage = modelItem.id,
                                id_purchaseRequest = aPurchaseRequestDetail.id_purchaseRequest,
                                PurchaseRequest = aPurchaseRequestDetail.PurchaseRequest,
                                id_purchaseRequestDetail = aPurchaseRequestDetail.id,
                                PurchaseRequestDetail = aPurchaseRequestDetail,
                                quantity = aPurchaseRequestDetail.quantityOutstandingPurchase
                            };
                            modelItem.PurchaseOrderDetailByGrammagePurchaseRequest.Add(newPurchaseOrderDetailByGrammagePurchaseRequest);
                        }
                        else
                        {

                        }

                        if (parameterSSP == "SI")
                        {
                            if (orderDetail.productionUnitProviderPoolreference != null)
                            {
                                var certification = productionUnitProviderPool.FirstOrDefault(p => p.id_productionUnitProvider == id_productionUnitProvider
                            && p.name == orderDetail.productionUnitProviderPoolreference);

                                if (order.id_certification.HasValue && certification.id_certification == null)
                                {
                                    msg = "PISCINA DEBE TENER ASC";
                                }

                                if (order.id_certification == null && certification.id_certification.HasValue)
                                {
                                    msg = "PISCINA TIENE ASC";
                                }
                            }
                            else
                            {
                                msg = "Debe seleccionar una piscina ";
                            }

                        }

                        bool actualizar = false;
                        if (parameterSSP == "SI" && msg == "")
                        {
                            actualizar = true;
                        }

                        if (parameterSSP == "NO")
                        {
                            actualizar = true;
                        }

                        if (actualizar)
                        {
                            modelItem.quantityRequested = orderDetail.quantityRequested;
                            modelItem.quantityOrdered = orderDetail.quantityOrdered;
                            modelItem.quantityApproved = orderDetail.quantityApproved;
                            modelItem.quantityReceived = orderDetail.quantityReceived;
                            modelItem.quantityDispatched = modelItem.quantityDispatched;
                            modelItem.price = modelItem.price;
                            modelItem.iva = modelItem.iva;
                            modelItem.subtotal = modelItem.subtotal;
                            modelItem.total = modelItem.total;
                            modelItem.id_userUpdate = this.ActiveUserId;
                            modelItem.dateUpdate = DateTime.Now;
                            modelItem.id_Grammage = modelItem.id_Grammage;
                            modelItem.productionUnitProviderPoolreference = modelItem.productionUnitProviderPoolreference;

                        }

                        modelItem.Item = db.Item.FirstOrDefault(i => i.id == orderDetail.id_item);
                        modelItem.Grammage = db.Grammage.FirstOrDefault(i => i.id == orderDetail.id_Grammage);

                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateCreate = DateTime.Now;

                         this.UpdateModel(modelItem);

                        order.PurchaseOrderTotal = PurchaseOrderTotalsBG(order.id, order.PurchaseOrderDetailByGrammage.ToList());
                        //TempData["id_provider"] = id_productionUnitProvider;
                        TempData["purchaseOrder"] = order;

                        if (parameterSSP == "SI" && msg != "")
                        {
                            throw new Exception(msg);
                        }

                       
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }

            }
            else
            {
               msg += "Por Favor, corrija todos los errores.";
            }
                ViewData["EditError"] = msg;

            TempData.Keep("purchaseOrder");
            //TempData.Keep("id_provider");
            var model = order?.PurchaseOrderDetailByGrammage.Where(d => d.isActive).ToList() ?? new List<PurchaseOrderDetailByGrammage>();

            return PartialView("_PurchaseOrderDetailsPartialBG", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartialDelete(System.Int32 id)//id_item)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            purchaseOrder = purchaseOrder ?? db.PurchaseOrder.FirstOrDefault(i => i.id == purchaseOrder.id);
            purchaseOrder = purchaseOrder ?? new PurchaseOrder();

            //if (id_item >= 0)
            //{
            try
            {
                //var orderDetail = purchaseOrder.PurchaseOrderDetail.FirstOrDefault(p => p.id_item == id_item);
                var orderDetail = purchaseOrder.PurchaseOrderDetail.FirstOrDefault(p => p.id == id);
                if (orderDetail != null)
                {
                    orderDetail.isActive = false;
                    orderDetail.id_userUpdate = ActiveUser.id;
                    orderDetail.dateCreate = DateTime.Now;
                }

                purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder.id, purchaseOrder.PurchaseOrderDetail.ToList());
                TempData["purchaseOrder"] = purchaseOrder;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("purchaseOrder");

            var model = purchaseOrder?.PurchaseOrderDetail.Where(d => d.isActive).ToList() ?? new List<PurchaseOrderDetail>();
            return PartialView("_PurchaseOrderDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartialDeleteBG(System.Int32 id)//id_item)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);
            //TempData.Keep("id_provider");
            
            purchaseOrder = purchaseOrder ?? db.PurchaseOrder.FirstOrDefault(i => i.id == purchaseOrder.id);
            purchaseOrder = purchaseOrder ?? new PurchaseOrder();

            var id_productionUnitProvider = purchaseOrder.id_productionUnitProvider;
            //if (id_item >= 0)
            //{
            try
            {
                //var orderDetail = purchaseOrder.PurchaseOrderDetail.FirstOrDefault(p => p.id_item == id_item);
                var orderDetail = purchaseOrder.PurchaseOrderDetailByGrammage.FirstOrDefault(p => p.id == id);
                if (orderDetail != null)
                {
                    orderDetail.isActive = false;
                    orderDetail.id_userUpdate = ActiveUser.id;
                    orderDetail.dateCreate = DateTime.Now;
                }

                purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder.id, purchaseOrder.PurchaseOrderDetailByGrammage.ToList());
                TempData["purchaseOrder"] = purchaseOrder;
                //TempData["id_provider"] = id_productionUnitProvider;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("purchaseOrder");
            //TempData.Keep("id_provider");

            var model = purchaseOrder?.PurchaseOrderDetailByGrammage.Where(d => d.isActive).ToList() ?? new List<PurchaseOrderDetailByGrammage>();
            return PartialView("_PurchaseOrderDetailsPartialBG", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void PurchaseOrderDetailsDeleteSeleted(int[] ids)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);
            purchaseOrder = purchaseOrder ?? db.PurchaseOrder.FirstOrDefault(i => i.id == purchaseOrder.id);
            purchaseOrder = purchaseOrder ?? new PurchaseOrder();

            if (ids != null)
            {
                try
                {
                    var orderDetail = purchaseOrder.PurchaseOrderDetail.Where(i => ids.Contains(i.id_item));

                    foreach (var detail in orderDetail)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["purchaseOrder"] = purchaseOrder;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("purchaseOrder");

        }

        #endregion

        #region SINGLE CHANGE DOCUMENT STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            TempData.Keep("model");
            var id_companyAux = (int)ViewData["id_company"];
            string rucCompany = db.Company.FirstOrDefault(r => r.id == id_companyAux).ruc;
            var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
            this.ViewBag.IdPersonProcess = idPersonProcess;
            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);
            string ruta = ConfigurationManager.AppSettings["rutaLog"];

            string parOCDetail = DataProviderSetting.ValueSetting("DPGOCD");

            #region TRANSACCION
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                    if (purchaseOrder != null && documentState != null)
                    {
                        purchaseOrder.Document.id_documentState = documentState.id;
                        purchaseOrder.Document.DocumentState = documentState;//APROBADA


                        foreach (var detail in purchaseOrder.PurchaseOrderDetailByGrammage)
                        {
                            foreach (var purchaseOrderDetailByGrammagePurchaseRequest in detail.PurchaseOrderDetailByGrammagePurchaseRequest)
                            {
                                ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseOrderDetailByGrammage,
                                                                               purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseRequestDetail,
                                                                               purchaseOrderDetailByGrammagePurchaseRequest.quantity);
                            }
                        }

                        //foreach (var detail in purchaseOrder.PurchaseOrderDetail)
                        //{
                        //    foreach (var purchaseOrderDetailPurchaseRequest in detail.PurchaseOrderDetailPurchaseRequest)
                        //    {
                        //        ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailPurchaseRequest.id_purchaseOrderDetail,
                        //                                                        purchaseOrderDetailPurchaseRequest.id_purchaseRequestDetail,
                        //                                                        purchaseOrderDetailPurchaseRequest.quantity);
                        //    }
                        //}

                        if (parOCDetail == "1")
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder.id, purchaseOrder.PurchaseOrderDetailByGrammage.ToList());
                        }
                        else
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder.id, purchaseOrder.PurchaseOrderDetail.ToList());
                        }

                        db.PurchaseOrder.Attach(purchaseOrder);
                        db.Entry(purchaseOrder).State = EntityState.Modified;

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
            #endregion

            #region AUDITORIA DE ORDEN DE COMPRA
            //if (_answer == 1)
            //{

            //    DocumentLogDTO _doTmp = new DocumentLogDTO();
            //    _doTmp.id = purchaseOrder.id;
            //    _doTmp.id_User = ActiveUser.id;
            //    _doTmp.description = "EJECUCION MANUAL";
            //    _doTmp.code_Action = "AD";

            //    Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
            //}
            #endregion


            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");
            purchaseOrder.descriptionDocPO = purchaseOrder.Document.description;

            return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            TempData.Keep("model");
            var id_companyAux = (int)ViewData["id_company"];
            string rucCompany = db.Company.FirstOrDefault(r => r.id == id_companyAux).ruc;
            var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
            this.ViewBag.IdPersonProcess = idPersonProcess;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            int _answer = 0;
            int _answerCloseDoc = 0;
            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);
            string parOCDetail = DataProviderSetting.ValueSetting("DPGOCD");

            #region TRANSACCION
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada

                    if (purchaseOrder != null && documentState != null)
                    {
                        purchaseOrder.Document.id_documentState = documentState.id;
                        purchaseOrder.Document.DocumentState = documentState;
                        purchaseOrder.Document.authorizationDate = DateTime.Now;

                        if (parOCDetail == "1")
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder.id, purchaseOrder.PurchaseOrderDetailByGrammage.ToList());
                        }
                        else
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder.id, purchaseOrder.PurchaseOrderDetail.ToList());
                        }

                        db.PurchaseOrder.Attach(purchaseOrder);
                        db.Entry(purchaseOrder).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        _answer = 1;
                        purchaseOrder.descriptionDocPO = purchaseOrder.Document.description;
                        TempData["purchaseOrder"] = purchaseOrder;
                        TempData.Keep("purchaseOrder");

                        ViewData["EditMessage"] = SuccessMessage("Orden de Compra: " + purchaseOrder.Document.number + " autorizada exitosamente");
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("purchaseOrder");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            #endregion

            if (_answer == 1)
            {
                #region ABRIR DOCUMENTO
                Document _doPo = db.Document.FirstOrDefault(fod => fod.id == purchaseOrder.id);

                if (_doPo != null)
                {
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            _doPo.isOpen = true;
                            db.Document.Attach(_doPo);
                            db.Entry(_doPo).State = EntityState.Modified;
                            db.SaveChanges();
                            trans.Commit();
                            _answerCloseDoc = 1;
                        }
                        catch //(Exception ex)
                        {

                        }
                    }
                    #region AUDITORIA APERTURA DE DOCUMENTO ORDEN DE COMPRA
                    if (_answerCloseDoc == 1)
                    {
                        DocumentLogDTO _doTmp = new DocumentLogDTO();
                        _doTmp.id = _doPo.id;
                        _doTmp.code_Action = "ABD";
                        _doTmp.id_User = ActiveUser.id;
                        _doTmp.description = "EJECUCION MANUAL";
                        Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                    }
                    #endregion
                }
                #endregion

            }

            return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
        }

        [HttpPost]
        public ActionResult Protect(int id)
        {
            TempData.Keep("model");
            var id_companyAux = (int)ViewData["id_company"];
            string rucCompany = db.Company.FirstOrDefault(r => r.id == id_companyAux).ruc;
            var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
            this.ViewBag.IdPersonProcess = idPersonProcess;
            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string parOCDetail = DataProviderSetting.ValueSetting("DPGOCD");
            int _answerCloseDoc = 0;
            int _answer = 0;
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "04"); //Cerrada

                    if (purchaseOrder != null && documentState != null)
                    {
                        purchaseOrder.Document.id_documentState = documentState.id;
                        purchaseOrder.Document.DocumentState = documentState;

                        db.PurchaseOrder.Attach(purchaseOrder);
                        db.Entry(purchaseOrder).State = EntityState.Modified;
                        if (parOCDetail == "1")
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder.id, purchaseOrder.PurchaseOrderDetailByGrammage.ToList());
                        }
                        else
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder.id, purchaseOrder.PurchaseOrderDetail.ToList());
                        }

                        db.SaveChanges();
                        trans.Commit();
                        _answer = 1;
                        TempData["purchaseOrder"] = purchaseOrder;
                        purchaseOrder.descriptionDocPO = purchaseOrder.Document.description;

                        ViewData["EditMessage"] = SuccessMessage("Orden de Compra: " + purchaseOrder.Document.number + " cerrada exitosamente");
                    }
                }
                catch (Exception)
                {
                    ViewData["EditError"] = ErrorMessage();
                    trans.Rollback();
                }
                finally
                {
                    TempData.Keep("purchaseOrder");

                }
            }

            if (_answer == 1)
            {
                #region CERRAR DOCUMENTO
                Document _doPo = db.Document.FirstOrDefault(fod => fod.id == purchaseOrder.id);

                if (_doPo != null)
                {
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            _doPo.isOpen = false;
                            db.Document.Attach(_doPo);
                            db.Entry(_doPo).State = EntityState.Modified;
                            db.SaveChanges();
                            trans.Commit();
                            _answerCloseDoc = 1;
                        }
                        catch //(Exception ex)
                        {

                        }
                    }
                    #region AUDITORIA CERRADO DE DOCUMENTO ORDEN DE COMPRA
                    if (_answerCloseDoc == 1)
                    {
                        DocumentLogDTO _doTmp = new DocumentLogDTO();
                        _doTmp.id = _doPo.id;
                        _doTmp.code_Action = "CRD";
                        _doTmp.id_User = ActiveUser.id;
                        _doTmp.description = "EJECUCION MANUAL";
                        Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                    }
                    #endregion
                }
                #endregion
            }

            return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            var id_companyAux = (int)ViewData["id_company"];
            string rucCompany = db.Company.FirstOrDefault(r => r.id == id_companyAux).ruc;
            var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
            this.ViewBag.IdPersonProcess = idPersonProcess;

            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);
            string parOCDetail = DataProviderSetting.ValueSetting("DPGOCD");
            int _answerCloseDoc = 0;
            int _answer = 0;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];

            #region TRANSACCION
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region VALIDATIONS BEFORE 
                    if (purchaseOrder != null)
                    {
                        purchaseOrder.descriptionDocPO = purchaseOrder.Document.description;
                    }
                    var existInInventoryMoveDetailPurchaseOrder = purchaseOrder.InventoryMoveDetailPurchaseOrder.FirstOrDefault(fod => fod.InventoryMoveDetail.InventoryMove.Document.DocumentState.code != "05");//Diferente de Anulado

                    if (existInInventoryMoveDetailPurchaseOrder != null)
                    {
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular la orden de compra debido a pertenecer a un movimiento de inventario.");
                        return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }

                    var existInPurchaseOrderDetailInventoryMoveDetailPurchaseOrder = purchaseOrder.PurchaseOrderDetail.FirstOrDefault(fod => fod.InventoryMoveDetailPurchaseOrder.FirstOrDefault(fod2 => fod2.InventoryMoveDetail.InventoryMove.Document.DocumentState.code != "05") != null);//Diferente de Anulado

                    if (existInPurchaseOrderDetailInventoryMoveDetailPurchaseOrder != null)
                    {
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular la orden de compra debido a tener detalles que pertenecen a un movimiento de inventario.");
                        return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }

                    var existInRemissionGuideDetailPurchaseOrderDetail = purchaseOrder.PurchaseOrderDetail.FirstOrDefault(fod => fod.RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault(fod2 => fod2.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code != "05") != null);//Diferente de Anulado

                    if (existInRemissionGuideDetailPurchaseOrderDetail != null)
                    {
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular la orden de compra debido a tener detalles que pertenecen a requerimiento de guia de despacho(Remisión).");
                        return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }

                    var existInProductionLotDetailPurchaseDetail = purchaseOrder.PurchaseOrderDetail.FirstOrDefault(fod => fod.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod2 => fod2.ProductionLotDetail.ProductionLot.Lot.Document.DocumentState.code != "05") != null);//Diferente de Anulado

                    if (existInProductionLotDetailPurchaseDetail != null)
                    {
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular la orden de compra debido a tener detalles que pertenecen a requerimiento de recepción de Lote.");
                        return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }
                    #endregion

                    #region UPDATE DOCUMENT STATE 
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (purchaseOrder != null && documentState != null)
                    {
                        if (purchaseOrder.Document.DocumentState.code != "01")
                        {
                            foreach (var detail in purchaseOrder.PurchaseOrderDetailByGrammage)
                            {
                                foreach (var purchaseOrderDetailByGrammagePurchaseRequest in detail.PurchaseOrderDetailByGrammagePurchaseRequest)
                                {
                                    ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseOrderDetailByGrammage,
                                                                                   purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseRequestDetail,
                                                                                   -purchaseOrderDetailByGrammagePurchaseRequest.quantity);
                                }
                            }

                            //foreach (var detail in purchaseOrder.PurchaseOrderDetail)
                            //{
                            //    foreach (var purchaseOrderDetailPurchaseRequest in detail.PurchaseOrderDetailPurchaseRequest)
                            //    {
                            //        ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailPurchaseRequest.id_purchaseOrderDetail,
                            //                                                       purchaseOrderDetailPurchaseRequest.id_purchaseRequestDetail,
                            //                                                       -purchaseOrderDetailPurchaseRequest.quantity);
                            //    }
                            //}
                        }


                        purchaseOrder.Document.id_documentState = documentState.id;
                        purchaseOrder.Document.DocumentState = documentState;

                        if (parOCDetail == "1")
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder.id, purchaseOrder.PurchaseOrderDetailByGrammage.ToList());
                        }
                        else
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder.id, purchaseOrder.PurchaseOrderDetail.ToList());
                        }

                        db.PurchaseOrder.Attach(purchaseOrder);
                        db.Entry(purchaseOrder).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        _answer = 1;
                        TempData["purchaseOrder"] = purchaseOrder;
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");

                        ViewData["EditMessage"] = SuccessMessage("Orden de Compra: " + purchaseOrder.Document.number + " anulada exitosamente");
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    TempData.Keep("purchaseOrder");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            #endregion

            if (_answer == 1)
            {
                #region CERRAR DOCUMENTO
                Document _doPo = db.Document.FirstOrDefault(fod => fod.id == purchaseOrder.id);

                if (_doPo != null)
                {
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            _doPo.isOpen = false;
                            db.Document.Attach(_doPo);
                            db.Entry(_doPo).State = EntityState.Modified;
                            db.SaveChanges();
                            trans.Commit();
                            _answerCloseDoc = 1;
                        }
                        catch //(Exception ex)
                        {

                        }
                    }
                    #region AUDITORIA CERRADO DE DOCUMENTO ORDEN DE COMPRA
                    if (_answerCloseDoc == 1)
                    {
                        DocumentLogDTO _doTmp = new DocumentLogDTO();
                        _doTmp.id = _doPo.id;
                        _doTmp.code_Action = "CRD";
                        _doTmp.id_User = ActiveUser.id;
                        _doTmp.description = "EJECUCION MANUAL";
                        Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                    }
                    #endregion
                }
                #endregion
            }


            return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            var id_companyAux = (int)ViewData["id_company"];
            string rucCompany = db.Company.FirstOrDefault(r => r.id == id_companyAux).ruc;
            var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
            this.ViewBag.IdPersonProcess = idPersonProcess;

            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);
            string parOCDetail = DataProviderSetting.ValueSetting("DPGOCD");
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            int _answerCloseDoc = 0;
            int _answer = 0;

            #region TRANSACCION
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region DOCUMENT VALIDATION BEFORE REVERT
                    if (purchaseOrder != null)
                    {
                        purchaseOrder.descriptionDocPO = purchaseOrder.Document.description;
                    }
                    var existInInventoryMoveDetailPurchaseOrder = purchaseOrder.InventoryMoveDetailPurchaseOrder.FirstOrDefault(fod => fod.InventoryMoveDetail.InventoryMove.Document.DocumentState.code != "05");//Diferente de Anulado

                    if (existInInventoryMoveDetailPurchaseOrder != null)
                    {
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar la orden de compra debido a pertenecer a un movimiento de inventario.");
                        return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }

                    var existInPurchaseOrderDetailInventoryMoveDetailPurchaseOrder = purchaseOrder.PurchaseOrderDetail.FirstOrDefault(fod => fod.InventoryMoveDetailPurchaseOrder.FirstOrDefault(fod2 => fod2.InventoryMoveDetail.InventoryMove.Document.DocumentState.code != "05") != null);//Diferente de Anulado

                    if (existInPurchaseOrderDetailInventoryMoveDetailPurchaseOrder != null)
                    {
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar la orden de compra debido a tener detalles que pertenecen a un movimiento de inventario.");
                        return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }

                    var existInRemissionGuideDetailPurchaseOrderDetail = purchaseOrder.PurchaseOrderDetail.FirstOrDefault(fod => fod.RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault(fod2 => fod2.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code != "05") != null);//Diferente de Anulado

                    if (existInRemissionGuideDetailPurchaseOrderDetail != null)
                    {
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar la orden de compra debido a tener detalles que pertenecen a requerimiento de guia de despacho(Remisión).");
                        return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }

                    var existInProductionLotDetailPurchaseDetail = purchaseOrder.PurchaseOrderDetail.FirstOrDefault(fod => fod.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod2 => fod2.ProductionLotDetail.ProductionLot.Lot.Document.DocumentState.code != "05") != null);//Diferente de Anulado

                    if (existInProductionLotDetailPurchaseDetail != null)
                    {
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar la orden de compra debido a tener detalles que pertenecen a requerimiento de recepción de Lote.");
                        return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
                    }
                    #endregion

                    #region UPDATE CHANGE STATE DOCUMENT
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

                    if (purchaseOrder != null && documentState != null)
                    {
                        foreach (var detail in purchaseOrder.PurchaseOrderDetailByGrammage)
                        {
                            foreach (var purchaseOrderDetailByGrammagePurchaseRequest in detail.PurchaseOrderDetailByGrammagePurchaseRequest)
                            {
                                ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseOrderDetailByGrammage,
                                                                               purchaseOrderDetailByGrammagePurchaseRequest.id_purchaseRequestDetail,
                                                                               -purchaseOrderDetailByGrammagePurchaseRequest.quantity);
                            }
                        }

                        //foreach (var detail in purchaseOrder.PurchaseOrderDetail)
                        //{
                        //    foreach (var purchaseOrderDetailPurchaseRequest in detail.PurchaseOrderDetailPurchaseRequest)
                        //    {
                        //        ServicePurchaseRemission.UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(db, purchaseOrderDetailPurchaseRequest.id_purchaseOrderDetail,
                        //                                                       purchaseOrderDetailPurchaseRequest.id_purchaseRequestDetail,
                        //                                                       -purchaseOrderDetailPurchaseRequest.quantity);
                        //    }
                        //}

                        purchaseOrder.Document.id_documentState = documentState.id;
                        purchaseOrder.Document.DocumentState = documentState;

                        if (parOCDetail == "1")
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder.id, purchaseOrder.PurchaseOrderDetailByGrammage.ToList());
                        }
                        else
                        {
                            purchaseOrder.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder.id, purchaseOrder.PurchaseOrderDetail.ToList());
                        }

                        db.PurchaseOrder.Attach(purchaseOrder);
                        db.Entry(purchaseOrder).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        _answer = 1;
                        TempData["purchaseOrder"] = purchaseOrder;
                        TempData.Keep("purchaseOrder");
                        TempData.Keep("model");

                        ViewData["EditMessage"] = SuccessMessage("Orden de Compra: " + purchaseOrder.Document.number + " reversada exitosamente");
                    }
                    #endregion

                }
                catch (Exception)
                {
                    //ViewData["EditError"] = e.Message;
                    TempData.Keep("purchaseOrder");
                    TempData.Keep("model");
                    ViewData["EditError"] = ErrorMessage();
                    trans.Rollback();
                }
            }
            #endregion

            if (_answer == 1)
            {
                #region CERRAR DOCUMENTO
                Document _doPo = db.Document.FirstOrDefault(fod => fod.id == purchaseOrder.id);

                if (_doPo != null)
                {
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            _doPo.isOpen = false;
                            db.Document.Attach(_doPo);
                            db.Entry(_doPo).State = EntityState.Modified;
                            db.SaveChanges();
                            trans.Commit();
                            _answerCloseDoc = 1;
                        }
                        catch //(Exception ex)
                        {

                        }
                    }
                    #region AUDITORIA CERRADO DE DOCUMENTO ORDEN DE COMPRA
                    if (_answerCloseDoc == 1)
                    {
                        DocumentLogDTO _doTmp = new DocumentLogDTO();
                        _doTmp.id = _doPo.id;
                        _doTmp.code_Action = "CRD";
                        _doTmp.id_User = ActiveUser.id;
                        _doTmp.description = "EJECUCION MANUAL";
                        Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                    }
                    #endregion
                }
                #endregion
            }


            return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
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
                            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                            if (purchaseOrder != null && documentState != null)
                            {
                                purchaseOrder.Document.id_documentState = documentState.id;
                                purchaseOrder.Document.DocumentState = documentState;

                                foreach (var details in purchaseOrder.PurchaseOrderDetail)
                                {
                                    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityOrdered;
                                    db.PurchaseOrderDetail.Attach(details);
                                    db.Entry(details).State = EntityState.Modified;
                                }

                                db.PurchaseOrder.Attach(purchaseOrder);
                                db.Entry(purchaseOrder).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseOrder>);
            model = model ?? new List<PurchaseOrder>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

                            if (purchaseOrder != null && documentState != null)
                            {
                                purchaseOrder.Document.id_documentState = documentState.id;
                                purchaseOrder.Document.DocumentState = documentState;

                                foreach (var details in purchaseOrder.PurchaseOrderDetail)
                                {
                                    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityOrdered;

                                    db.PurchaseOrderDetail.Attach(details);
                                    db.Entry(details).State = EntityState.Modified;
                                }

                                db.PurchaseOrder.Attach(purchaseOrder);
                                db.Entry(purchaseOrder).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseOrder>);
            model = model ?? new List<PurchaseOrder>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                            if (purchaseOrder != null && documentState != null)
                            {
                                purchaseOrder.Document.id_documentState = documentState.id;
                                purchaseOrder.Document.DocumentState = documentState;

                                purchaseOrder.Document.isOpen = false;

                                db.PurchaseOrder.Attach(purchaseOrder);
                                db.Entry(purchaseOrder).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseOrder>);
            model = model ?? new List<PurchaseOrder>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

                            if (purchaseOrder != null && documentState != null)
                            {
                                purchaseOrder.Document.id_documentState = documentState.id;
                                purchaseOrder.Document.DocumentState = documentState;

                                db.PurchaseOrder.Attach(purchaseOrder);
                                db.Entry(purchaseOrder).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseOrder>);
            model = model ?? new List<PurchaseOrder>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                            if (purchaseOrder != null && documentState != null)
                            {
                                purchaseOrder.Document.id_documentState = documentState.id;
                                purchaseOrder.Document.DocumentState = documentState;

                                foreach (var details in purchaseOrder.PurchaseOrderDetail)
                                {
                                    details.quantityApproved = 0.0M;

                                    db.PurchaseOrderDetail.Attach(details);
                                    db.Entry(details).State = EntityState.Modified;
                                }

                                db.PurchaseOrder.Attach(purchaseOrder);
                                db.Entry(purchaseOrder).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<PurchaseOrder>);
            model = model ?? new List<PurchaseOrder>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.PurchaseOrder.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        #endregion

        #region PURCHASE ORDER REPORTS
        public JsonResult PurchaseOrderReportFilter(ReportModel reportModel)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            bool isvalid = false;
            string message = "";
            string strnamedata = "reportModel" + DateTime.Now.ToString("yyyyMMddmmssfff");

            try
            {

                int id_company = int.Parse(ViewData["id_company"].ToString());


                if (reportModel == null)
                {
                    reportModel = new ReportModel();
                    reportModel.ReportName = "PurchaseOrdersReport";

                }
                else
                {
                    if (reportModel.ListReportParameter == null)
                    {

                        reportModel.ListReportParameter = new List<ReportParameter>();

                    }
                    ReportParameter reportParameter = new ReportParameter();
                    reportParameter.Name = "id_company";
                    reportParameter.Value = ViewData["id_company"].ToString();
                    reportModel.ListReportParameter.Add(reportParameter);


                    reportParameter = new ReportParameter();
                    reportParameter.Name = "numberPurchaseOrder";
                    reportParameter.Value = purchaseOrder.Document.number.ToString();
                    reportModel.ListReportParameter.Add(reportParameter);
                }

                isvalid = true;
            }
            catch //(Exception ex)
            {

            }
            TempData[strnamedata] = reportModel;
            TempData.Keep(strnamedata);
            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            var result = new
            {
                isvalid,
                message,
                reportModel = strnamedata,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]

        public JsonResult PurchaseOrdersReport(ReportModel reportModel, int id)
        {

            bool isvalid = false;
            string message = "";
            string strnamedata = "reportModel" + DateTime.Now.ToString("yyyyMMddmmssfff");


            try
            {

                int id_company = int.Parse(ViewData["id_company"].ToString());



                if (reportModel == null)
                {
                    reportModel = new ReportModel();
                    reportModel.ReportName = "PurchaseOrdersReport";

                }
                else
                {
                    if (reportModel.ListReportParameter == null)
                    {

                        reportModel.ListReportParameter = new List<ReportParameter>();

                    }

                    printcontrol printcontrol = new printcontrol()
                    {
                        id_referencia = id,
                        namereport = reportModel.ReportName,
                        optiondescrip = "PurchaseOrdersReport",
                        dateCreate = DateTime.Now,
                        id_userCreate = ActiveUser.id,
                        isActive = true,
                        dateUpdate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        printnumber = 1
                    };


                    printcontrol = DataProviders.DataProviderPrintControl.SaveControlPrint(printcontrol);



                    ReportParameter reportParameter = new ReportParameter();
                    reportParameter.Name = "numberPurchaseOrder";
                    reportParameter.Value = id.ToString();
                    reportModel.ListReportParameter.Add(reportParameter);







                }

                isvalid = true;
            }
            catch //(Exception ex)
            {


            }


            TempData[strnamedata] = reportModel;
            TempData.Keep(strnamedata);
            TempData.Keep("model");




            var result = new
            {
                isvalid,
                message,
                reportModel = strnamedata,
            };

            return Json(result, JsonRequestBehavior.AllowGet);


            //return PartialView("_PurchaseOrderReport");
        }

        #endregion

        #region PURCHASE ORDER REPORTS CRYSTAL
        public JsonResult PrintReportPurchaseOrderCrystal(int id_im, string codeReport)
        {
            var inventoryMove = (TempData["purchaseOrder"] as InventoryMove);
            TempData["purchaseOrder"] = inventoryMove;
            TempData.Keep("purchaseOrder");

            #region "Armo Parametros"
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@num_purchaseorder";
            _param.Valor = id_im;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
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

            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(r => r.id == id);
            string state = purchaseOrder.Document.DocumentState.code;

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
                var purchaseOrderDetailAux = purchaseOrder.PurchaseOrderDetail.Where(w => w.isActive = true).FirstOrDefault(fod => fod.quantityReceived < fod.quantityApproved);

                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = purchaseOrderDetailAux != null,// true,
                                                                //btnProtect = true,
                    btnCancel = false,
                    btnRevert = true,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_purchaseOrder)
        {

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            int index = 0;
            int maxRecords = 0;
            List<PurchaseOrder> purchaseOrders = null;

            if (TempData["model"] == null)
            {
                purchaseOrders = db.PurchaseOrder.ToList();
            }
            else
            {
                purchaseOrders = (List<PurchaseOrder>)TempData["model"];
            }
            maxRecords = purchaseOrders.Count();
            index = purchaseOrders.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_purchaseOrder);


            //  

            var result = new
            {
                maximunPages = maxRecords,
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {

            TempData.Keep("model");

            PurchaseOrder purchaseOrder = null;

            if (TempData["model"] == null)
            {
                purchaseOrder = db.PurchaseOrder.OrderByDescending(p => p.id).Take(page).ToList().Last();
            }
            else
            {
                List<PurchaseOrder> purchaseOrders = (List<PurchaseOrder>)TempData["model"];
                purchaseOrder = purchaseOrders.OrderByDescending(p => p.id).Take(page).ToList().Last();

            }




            //  PurchaseOrder purchaseOrder = db.PurchaseOrder.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (purchaseOrder != null)
            {
                var id_companyAux = (int)ViewData["id_company"];
                string rucCompany = db.Company.FirstOrDefault(r => r.id == id_companyAux).ruc;
                var idPersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany)?.id ?? 0;
                this.ViewBag.IdPersonProcess = idPersonProcess;

                TempData["purchaseOrder"] = purchaseOrder;
                TempData.Keep("purchaseOrder");
                purchaseOrder.descriptionDocPO = purchaseOrder.Document.description;
                return PartialView("_PurchaseOrderMainFormPartial", purchaseOrder);
            }

            TempData.Keep("purchaseOrder");

            return PartialView("_PurchaseOrderMainFormPartial", new PurchaseOrder());
        }

        #endregion

        #region AXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult GetDataPurchaseRequestDetail(int id_purchaseRequestDetail)
        {
            string code_grammageFrom = "";
            decimal grammageFrom = 0.00M;
            string code_grammageTo = "";
            decimal grammageTo = 0.00M;
            int? id_item = null;
            decimal quantityRequested = 0.00M;
            string mensaje = "OK";

            //Aquí habrá cambios

            try
            {
                var aPurchaseRequestDetail = db.PurchaseRequestDetail.FirstOrDefault(fod => fod.id == id_purchaseRequestDetail);
                if (aPurchaseRequestDetail != null)
                {
                    id_item = aPurchaseRequestDetail.id_item;
                    code_grammageFrom = aPurchaseRequestDetail.Grammage.code;
                    grammageFrom = aPurchaseRequestDetail.Grammage.value;
                    code_grammageTo = aPurchaseRequestDetail.Grammage1.code;
                    grammageTo = aPurchaseRequestDetail.Grammage1.value;
                    quantityRequested = aPurchaseRequestDetail.quantityOutstandingPurchase;
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message.ToString();

            }

            var result = new
            {
                id_item,
                code_grammageFrom,
                grammageFrom,
                code_grammageTo,
                grammageTo,
                quantityRequested,
                mensaje

            };
            TempData.Keep("model");


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LockedDocument(int id_document, string nameDocument, string code_sourceLockedDocument, string namesourceLockedDocument)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Message = ServiceLockedDocument.LockedDocument(db, ActiveUser, id_document, nameDocument, code_sourceLockedDocument, namesourceLockedDocument);
                        if (result.Message != "OK")
                        {
                            result.Code = -1;
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult UnlockedDocument(int id_document, string nameDocument, string code_sourceLockedDocument)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Message = ServiceLockedDocument.UnlockedDocument(db, ActiveUser, id_document, nameDocument, code_sourceLockedDocument);
                        if (result.Message != "OK")
                        {
                            result.Code = -1;
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult ProductionUnitProviderByProvider(int? id_productionUnitProviderCurrent, int? id_provider)
        {

            if (id_provider == null || id_provider < 0)
            {
                if (Request.Params["id_provider"] != null && Request.Params["id_provider"] != "")
                {
                    id_provider = int.Parse(Request.Params["id_provider"]);
                }
                else
                {
                    id_provider = -1;
                }
            }
            if (id_productionUnitProviderCurrent == null || id_productionUnitProviderCurrent < 0)
            {
                if (Request.Params["id_productionUnitProviderCurrent"] != null && Request.Params["id_productionUnitProviderCurrent"] != "")
                {
                    id_productionUnitProviderCurrent = int.Parse(Request.Params["id_productionUnitProviderCurrent"]);
                }
                else
                {
                    id_productionUnitProviderCurrent = -1;
                }
            }
            //var productionUnitProviderAux = db.ProductionUnitProvider.Where(t => t.isActive && t.id_provider == id_provider).ToList();

            //var productionUnitProviderCurrentAux = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionUnitProviderCurrent);
            //if (productionUnitProviderCurrentAux != null && !productionUnitProviderAux.Contains(productionUnitProviderCurrentAux)) productionUnitProviderAux.Add(productionUnitProviderCurrentAux);


            //TempData["ProductionUnitProviderByProvider"] = productionUnitProviderAux.Select(s => new {
            //    s.id,
            //    name = s.name
            //}).OrderBy(t => t.id).ToList();

            //TempData.Keep("ProductionUnitProviderByProvider");
            //RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            //TempData.Keep("remissionGuide");
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);
            purchaseOrder = purchaseOrder ?? new PurchaseOrder();
            //purchaseOrder.id_productionUnitProvider = id_productionUnitProviderCurrent;
            TempData.Keep("purchaseOrder");

            return PartialView("comboboxcascading/_cmbProviderProductionUnitPartial", purchaseOrder);
            //return PartialView("comboboxcascading/_cmbProviderProductionUnitPartial", remissionGuide);
        }

        private PurchaseOrderTotal PurchaseOrderTotals(int id_purcharseOrder, List<PurchaseOrderDetail> orderDetails)
        {
            PurchaseOrderTotal purchaseOrderTotal = db.PurchaseOrderTotal.FirstOrDefault(t => t.id_purcharseOrder == id_purcharseOrder);

            purchaseOrderTotal = purchaseOrderTotal ?? new PurchaseOrderTotal
            {
                id_purcharseOrder = id_purcharseOrder
            };

            decimal subtotalIVA12Percent = 0.0M;
            //decimal subtotalIVA14Percent = 0.0M;
            decimal subtotalIVA0Percent = 0.0M;
            decimal subtotalIVANoObjectIVA = 0.0M;
            decimal subtotalExentedIVA = 0.0M;

            decimal subtotal = 0.0M;

            decimal discount = 0.0M;
            decimal valueICE = 0.0M;
            decimal valueIRBPNR = 0.0M;

            decimal totalIVA12 = 0.0M;
            //decimal totalIVA14 = 0.0M;

            decimal total = 0.0M;

            foreach (var detail in orderDetails)
            {
                if (detail.isActive)
                {
                    Item item = db.Item.FirstOrDefault(t => t.id == detail.id_item);
                    ItemTaxation rateIVA12 = item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("2"));//Taxtype: "2": Impuesto IVA con Rate: "2": tarifa 12% 
                                                                                                                                                 //                                                                                                                                     //     ItemTaxation rateIVA14 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("5"));//Taxtype: "2": Impuesto IVA con Rate: "5": tarifa 14% 

                    if (rateIVA12 != null)
                    {
                        subtotalIVA12Percent += detail.quantityApproved * detail.price;
                    }
                    subtotal += detail.quantityApproved * detail.price;
                }

            }

            var percent12 = db.Rate.FirstOrDefault(fod => fod.code.Equals("2"))?.percentage / 100 ?? 0.12M; //"2" Tarifa 12%
                                                                                                            //  var percent14 = db.Rate.FirstOrDefault(fod => fod.code.Equals("5"))?.percentage/100 ?? 0.14M; //"5" Tarifa 14%
            totalIVA12 = subtotalIVA12Percent * percent12;// 0.12M;
                                                          /// totalIVA14 = subtotalIVA14Percent * percent14;//0.14M;

            //total = subtotal + totalIVA12 + totalIVA14 + valueICE + valueIRBPNR - discount;
            total = subtotal + totalIVA12 + valueICE + valueIRBPNR - discount;

            purchaseOrderTotal.subtotalIVA12Percent = subtotalIVA12Percent;
            //purchaseOrderTotal.subtotalIVA14Percent = subtotalIVA14Percent;
            purchaseOrderTotal.subtotalIVA0Percent = subtotalIVA0Percent;
            purchaseOrderTotal.subtotalIVANoObjectIVA = subtotalIVANoObjectIVA;
            purchaseOrderTotal.subtotalExentedIVA = subtotalExentedIVA;

            purchaseOrderTotal.subtotal = subtotal;
            purchaseOrderTotal.discount = discount;
            purchaseOrderTotal.valueICE = valueICE;
            purchaseOrderTotal.valueIRBPNR = valueIRBPNR;

            purchaseOrderTotal.totalIVA12 = totalIVA12;
            //            purchaseOrderTotal.totalIVA14 = totalIVA14;

            purchaseOrderTotal.total = total;

            return purchaseOrderTotal;
        }

        private PurchaseOrderTotal PurchaseOrderTotalsBG(int id_purcharseOrder, List<PurchaseOrderDetailByGrammage> orderDetails)
        {
            PurchaseOrderTotal purchaseOrderTotal = db.PurchaseOrderTotal.FirstOrDefault(t => t.id_purcharseOrder == id_purcharseOrder);

            purchaseOrderTotal = purchaseOrderTotal ?? new PurchaseOrderTotal
            {
                id_purcharseOrder = id_purcharseOrder
            };

            decimal subtotalIVA12Percent = 0.0M;
            //decimal subtotalIVA14Percent = 0.0M;
            decimal subtotalIVA0Percent = 0.0M;
            decimal subtotalIVANoObjectIVA = 0.0M;
            decimal subtotalExentedIVA = 0.0M;

            decimal subtotal = 0.0M;

            decimal discount = 0.0M;
            decimal valueICE = 0.0M;
            decimal valueIRBPNR = 0.0M;

            decimal totalIVA12 = 0.0M;
            //decimal totalIVA14 = 0.0M;

            decimal total = 0.0M;

            foreach (var detail in orderDetails)
            {
                if (detail.Item != null && detail.isActive)
                {
                    ItemTaxation rateIVA12 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("2"));//Taxtype: "2": Impuesto IVA con Rate: "2": tarifa 12% 
                                                                                                                                                 //     ItemTaxation rateIVA14 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("5"));//Taxtype: "2": Impuesto IVA con Rate: "5": tarifa 14% 

                    if (rateIVA12 != null)
                    {
                        subtotalIVA12Percent += detail.quantityApproved * detail.price;
                    }

                    subtotal += detail.quantityApproved * detail.price;
                }

            }

            var percent12 = db.Rate.FirstOrDefault(fod => fod.code.Equals("2"))?.percentage / 100 ?? 0.12M; //"2" Tarifa 12%
                                                                                                            //  var percent14 = db.Rate.FirstOrDefault(fod => fod.code.Equals("5"))?.percentage/100 ?? 0.14M; //"5" Tarifa 14%
            totalIVA12 = subtotalIVA12Percent * percent12;// 0.12M;
                                                          /// totalIVA14 = subtotalIVA14Percent * percent14;//0.14M;

            //total = subtotal + totalIVA12 + totalIVA14 + valueICE + valueIRBPNR - discount;
            total = subtotal + totalIVA12 + valueICE + valueIRBPNR - discount;

            purchaseOrderTotal.subtotalIVA12Percent = subtotalIVA12Percent;
            //purchaseOrderTotal.subtotalIVA14Percent = subtotalIVA14Percent;
            purchaseOrderTotal.subtotalIVA0Percent = subtotalIVA0Percent;
            purchaseOrderTotal.subtotalIVANoObjectIVA = subtotalIVANoObjectIVA;
            purchaseOrderTotal.subtotalExentedIVA = subtotalExentedIVA;

            purchaseOrderTotal.subtotal = subtotal;
            purchaseOrderTotal.discount = discount;
            purchaseOrderTotal.valueICE = valueICE;
            purchaseOrderTotal.valueIRBPNR = valueIRBPNR;

            purchaseOrderTotal.totalIVA12 = totalIVA12;
            //            purchaseOrderTotal.totalIVA14 = totalIVA14;

            purchaseOrderTotal.total = total;

            return purchaseOrderTotal;
        }

        private decimal PurchaseDetailPrice(int id_item, PurchaseOrder purchaseOrder)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return 0.0M;
            }

            //Error no esta asiganada el valor de la lista como objecto 
            //sino el id_priceList
            // PriceList list = purchaseOrder.PriceList;

            PriceList list = db.PriceList.Where(x => x.id == purchaseOrder.id_priceList).FirstOrDefault();

            decimal price = item.ItemPurchaseInformation.purchasePrice ?? 0.0M;

            if (list != null)
            {
                var codeAux = item.ItemGeneral.ItemGroupCategory?.code;
                var codeClassAux = item.ItemTypeCategory?.code;
                //PriceListDetail listDetail = list.PriceListDetail.FirstOrDefault(d => d.id_item == item.id);
                PriceListItemSizeDetail listDetail = list.PriceListItemSizeDetail.FirstOrDefault(d => d.Id_Itemsize == item.ItemGeneral.id_size &&
                                                                                                      d.ClassShrimp.code == codeAux &&
                                                                                                      d.Class.code == codeClassAux);
                price = listDetail?.price ?? price;
            }

            return price;
        }

        private decimal PurchaseDetailIVA(int id_item, decimal quantity, decimal price)
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

        private PurchaseOrder Copy(PurchaseOrder purchaseOrder, PurchaseOrder purchaseOrder2)
        {
            purchaseOrder2 = new PurchaseOrder()
            {
                id = 0,
                id_buyer = purchaseOrder.id_buyer,
                id_paymentMethod = purchaseOrder.id_paymentMethod,
                id_paymentTerm = purchaseOrder.id_paymentTerm,
                id_personRequesting = purchaseOrder.id_personRequesting,
                id_priceList = purchaseOrder.id_priceList,
                id_provider = purchaseOrder.id_provider,
                id_productionUnitProvider = purchaseOrder.id_productionUnitProvider,
                id_purchaseReason = purchaseOrder.id_purchaseReason,
                id_shippingType = purchaseOrder.id_shippingType,
                id_certification = purchaseOrder.id_certification,
            };
            foreach (var detail in purchaseOrder.PurchaseOrderDetail)
            {
                if (detail.Item != null && detail.isActive)
                {
                    var tempItemPODetail = new PurchaseOrderDetail
                    {
                        id_item = detail.id_item,
                        Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                        quantityRequested = detail.quantityRequested,
                        quantityOrdered = detail.quantityOrdered,
                        quantityApproved = detail.quantityApproved,
                        quantityReceived = detail.quantityReceived,

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
                    purchaseOrder2.PurchaseOrderDetail.Add(tempItemPODetail);
                }

            }
            return purchaseOrder2;
        }

        private PurchaseOrder CopyBG(PurchaseOrder purchaseOrder, PurchaseOrder purchaseOrder2)
        {
            purchaseOrder2 = new PurchaseOrder()
            {
                id = 0,
                id_buyer = purchaseOrder.id_buyer,
                id_paymentMethod = purchaseOrder.id_paymentMethod,
                id_paymentTerm = purchaseOrder.id_paymentTerm,
                id_personRequesting = purchaseOrder.id_personRequesting,
                id_priceList = purchaseOrder.id_priceList,
                id_provider = purchaseOrder.id_provider,
                id_productionUnitProvider = purchaseOrder.id_productionUnitProvider,
                id_purchaseReason = purchaseOrder.id_purchaseReason,
                id_shippingType = purchaseOrder.id_shippingType,
                id_certification = purchaseOrder.id_certification,
            };
            foreach (var detail in purchaseOrder.PurchaseOrderDetailByGrammage)
            {
                if (detail.Item != null && detail.isActive)
                {
                    var tempItemPODetail = new PurchaseOrderDetailByGrammage
                    {
                        id_item = detail.id_item,
                        Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                        quantityRequested = detail.quantityRequested,
                        quantityOrdered = detail.quantityOrdered,
                        quantityApproved = detail.quantityApproved,
                        quantityReceived = detail.quantityReceived,

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
                    purchaseOrder2.PurchaseOrderDetailByGrammage.Add(tempItemPODetail);
                }

            }
            return purchaseOrder2;
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePurchaseOrder2()
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            PurchaseOrder purchaseOrder2 = (TempData["purchaseOrder2"] as PurchaseOrder);

            purchaseOrder2 = Copy(purchaseOrder, purchaseOrder2);

            var result = new
            {
                Message = "OK"
            };

            TempData["purchaseOrder2"] = purchaseOrder2;
            TempData.Keep("purchaseOrder2");

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePurchaseOrder2BG()
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            PurchaseOrder purchaseOrder2 = (TempData["purchaseOrder2"] as PurchaseOrder);

            purchaseOrder2 = CopyBG(purchaseOrder, purchaseOrder2);

            var result = new
            {
                Message = "OK"
            };

            TempData["purchaseOrder2"] = purchaseOrder2;
            TempData.Keep("purchaseOrder2");

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemDetailData(int id_item, string quantityApproved, string price)
        {
            decimal _quantityApproved = Convert.ToDecimal(quantityApproved);

            //decimal _price = Convert.ToDecimal(price.Replace(",","."));
            decimal _price = Convert.ToDecimal(price);

            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            PurchaseOrder purchaseOrder2 = (TempData["purchaseOrder2"] as PurchaseOrder);

            purchaseOrder2 = purchaseOrder2 ?? Copy(purchaseOrder, purchaseOrder2);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            if (_price <= 0.0M)
            {
                _price = PurchaseDetailPrice(item.id, purchaseOrder2);
            }

            decimal iva = PurchaseDetailIVA(item.id, _quantityApproved, _price);

            PurchaseOrderDetail detail = purchaseOrder2.PurchaseOrderDetail.FirstOrDefault(d => d.id_item == id_item);


            if (detail != null)
            {
                detail.price = _price;
                detail.quantityApproved = _quantityApproved;
                purchaseOrder2.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder2.id, purchaseOrder2.PurchaseOrderDetail.ToList());
            }
            else
            {
                detail = new PurchaseOrderDetail()
                {
                    id = 0,
                    id_item = id_item,
                    Item = db.Item.FirstOrDefault(i => i.id == id_item),
                    isActive = true,
                    price = _price,
                    iva = iva,
                    subtotal = _price * _quantityApproved,
                    total = _price * _quantityApproved + iva,
                    PurchaseOrder = purchaseOrder2,
                    id_purchaseOrder = purchaseOrder2.id,
                    quantityApproved = _quantityApproved
                };
                purchaseOrder2.PurchaseOrderDetail.Add(detail);
                purchaseOrder2.PurchaseOrderTotal = PurchaseOrderTotals(purchaseOrder2.id, purchaseOrder2.PurchaseOrderDetail.ToList());

            }

            var result = new
            {
                ItemDetailData = new
                {
                    masterCode = item.masterCode,
                    um = item.ItemPurchaseInformation.MetricUnit.code,
                    price = _price,
                    iva = iva,
                    subtotal = _price * _quantityApproved,
                    total = _price * _quantityApproved + iva
                },
                OrderTotal = new
                {
                    subtotal = purchaseOrder2.PurchaseOrderTotal.subtotal,
                    subtotalIVA12Percent = purchaseOrder2.PurchaseOrderTotal.subtotalIVA12Percent,
                    totalIVA12 = purchaseOrder2.PurchaseOrderTotal.totalIVA12,
                    discount = purchaseOrder2.PurchaseOrderTotal.discount,
                    subtotalIVA14Percent = purchaseOrder2.PurchaseOrderTotal.subtotalIVA14Percent,
                    totalIVA14 = purchaseOrder2.PurchaseOrderTotal.totalIVA14,
                    total = purchaseOrder2.PurchaseOrderTotal.total
                }
            };

            TempData["purchaseOrder2"] = purchaseOrder2;
            TempData.Keep("purchaseOrder2");

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemDetailDataBG(int id_item, string quantityApproved, string price)
        {
            decimal _quantityApproved = Convert.ToDecimal(quantityApproved);

            //decimal _price = Convert.ToDecimal(price.Replace(",","."));
            decimal _price = Convert.ToDecimal(price);

            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            PurchaseOrder purchaseOrder2 = (TempData["purchaseOrder2"] as PurchaseOrder);

            purchaseOrder2 = purchaseOrder2 ?? Copy(purchaseOrder, purchaseOrder2);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            if (_price <= 0.0M)
            {
                _price = PurchaseDetailPrice(item.id, purchaseOrder2);
            }

            decimal iva = PurchaseDetailIVA(item.id, _quantityApproved, _price);

            PurchaseOrderDetailByGrammage detail = purchaseOrder2.PurchaseOrderDetailByGrammage.FirstOrDefault(d => d.id_item == id_item);


            if (detail != null)
            {
                detail.price = _price;
                detail.quantityApproved = _quantityApproved;
                purchaseOrder2.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder2.id, purchaseOrder2.PurchaseOrderDetailByGrammage.ToList());
            }
            else
            {
                detail = new PurchaseOrderDetailByGrammage()
                {
                    id = 0,
                    id_item = id_item,
                    Item = db.Item.FirstOrDefault(i => i.id == id_item),
                    isActive = true,
                    price = _price,
                    iva = iva,
                    subtotal = _price * _quantityApproved,
                    total = _price * _quantityApproved + iva,
                    PurchaseOrder = purchaseOrder2,
                    id_purchaseOrder = purchaseOrder2.id,
                    quantityApproved = _quantityApproved
                };
                purchaseOrder2.PurchaseOrderDetailByGrammage.Add(detail);
                purchaseOrder2.PurchaseOrderTotal = PurchaseOrderTotalsBG(purchaseOrder2.id, purchaseOrder2.PurchaseOrderDetailByGrammage.ToList());

            }

            var result = new
            {
                ItemDetailData = new
                {
                    masterCode = item.masterCode,
                    um = item.ItemPurchaseInformation.MetricUnit.code,
                    price = _price,
                    iva = iva,
                    subtotal = _price * _quantityApproved,
                    total = _price * _quantityApproved + iva
                },
                OrderTotal = new
                {
                    subtotal = purchaseOrder2.PurchaseOrderTotal.subtotal,
                    subtotalIVA12Percent = purchaseOrder2.PurchaseOrderTotal.subtotalIVA12Percent,
                    totalIVA12 = purchaseOrder2.PurchaseOrderTotal.totalIVA12,
                    discount = purchaseOrder2.PurchaseOrderTotal.discount,
                    subtotalIVA14Percent = purchaseOrder2.PurchaseOrderTotal.subtotalIVA14Percent,
                    totalIVA14 = purchaseOrder2.PurchaseOrderTotal.totalIVA14,
                    total = purchaseOrder2.PurchaseOrderTotal.total
                }
            };

            TempData["purchaseOrder2"] = purchaseOrder2;
            TempData.Keep("purchaseOrder2");

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PurchaseOrderDetails(int? id_purchaseReason, int? id_itemCurrent)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            purchaseOrder = purchaseOrder ?? new PurchaseOrder();
            purchaseOrder.PurchaseOrderDetail = purchaseOrder.PurchaseOrderDetail ?? new List<PurchaseOrderDetail>();

            var purchaseItemsByPurchaseReason = new List<Item>();
            var items = new List<Item>();

            var purchaseReason = db.PurchaseReason.FirstOrDefault(fod => fod.id == id_purchaseReason);
            var code_purchaseReason = purchaseReason?.code ?? "";

            if (code_purchaseReason == "")
            {
                purchaseItemsByPurchaseReason = db.Item.Where(i => i.id_company == this.ActiveCompanyId && i.isPurchased && i.isActive && i.id != id_itemCurrent).ToList();
            }
            else
            {
                if (code_purchaseReason == "MP")
                {
                    purchaseItemsByPurchaseReason = db.Item.Where(i => i.id_company == this.ActiveCompanyId && i.isPurchased && i.isActive && i.id != id_itemCurrent && i.InventoryLine.code != "MP").ToList();
                    items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.isPurchased && w.InventoryLine.code == "MP") || w.id == id_itemCurrent).ToList();
                }
                else
                {
                    purchaseItemsByPurchaseReason = db.Item.Where(i => i.id_company == this.ActiveCompanyId && i.isPurchased && i.isActive && i.id != id_itemCurrent && i.InventoryLine.code == "MP").ToList();
                    items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.isPurchased && w.InventoryLine.code != "MP") || w.id == id_itemCurrent).ToList();
                }
            }


            var result = new
            {
                purchaseOrderDetails = purchaseOrder.PurchaseOrderDetail.Where(d => d.isActive && d.id_item != id_itemCurrent).Select(d => d.id_item).ToList(),
                purchaseItemsByPurchaseReason = purchaseItemsByPurchaseReason.Select(d => d.id).ToList(),
                items = items.Select(s => new
                {
                    id = s.id,
                    masterCode = s.masterCode,
                    itemPurchaseInformationMetricUnitCode = (s.ItemPurchaseInformation != null) ? s.ItemPurchaseInformation.MetricUnit.code : "",
                    name = s.name
                }).ToList(),
                Message = "Ok"
            };

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PurchaseOrderDetailsBG(int? id_purchaseReason, int? id_itemCurrent)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            purchaseOrder = purchaseOrder ?? new PurchaseOrder();
            purchaseOrder.PurchaseOrderDetailByGrammage = purchaseOrder.PurchaseOrderDetailByGrammage ?? new List<PurchaseOrderDetailByGrammage>();

            var purchaseItemsByPurchaseReason = new List<Item>();
            var items = new List<Item>();

            var purchaseReason = db.PurchaseReason.FirstOrDefault(fod => fod.id == id_purchaseReason);
            var code_purchaseReason = purchaseReason?.code ?? "";

            if (code_purchaseReason == "")
            {
                purchaseItemsByPurchaseReason = db.Item.Where(i => i.id_company == this.ActiveCompanyId && i.isPurchased && i.isActive && i.id != id_itemCurrent).ToList();
            }
            else
            {
                if (code_purchaseReason == "MP")
                {
                    purchaseItemsByPurchaseReason = db.Item.Where(i => i.id_company == this.ActiveCompanyId && i.isPurchased && i.isActive && i.id != id_itemCurrent && i.InventoryLine.code != "MP").ToList();
                    items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.isPurchased && w.InventoryLine.code == "MP") || w.id == id_itemCurrent).ToList();
                }
                else
                {
                    purchaseItemsByPurchaseReason = db.Item.Where(i => i.id_company == this.ActiveCompanyId && i.isPurchased && i.isActive && i.id != id_itemCurrent && i.InventoryLine.code == "MP").ToList();
                    items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.isPurchased && w.InventoryLine.code != "MP") || w.id == id_itemCurrent).ToList();
                }
            }


            var result = new
            {
                purchaseOrderDetails = purchaseOrder.PurchaseOrderDetailByGrammage.Where(d => d.isActive && d.id_item != id_itemCurrent).Select(d => d.id_item).ToList(),
                purchaseItemsByPurchaseReason = purchaseItemsByPurchaseReason.Select(d => d.id).ToList(),
                items = items.Select(s => new
                {
                    id = s.id,
                    masterCode = s.masterCode,
                    itemPurchaseInformationMetricUnitCode = (s.ItemPurchaseInformation != null) ? s.ItemPurchaseInformation.MetricUnit.code : "",
                    name = s.name
                }).ToList(),
                Message = "Ok"
            };

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePurchaseOrderPriceListsAndProductionUnitProviders(int? id_provider)
        {
            var providersAux = db.Provider.FirstOrDefault(t => (t.id == id_provider));
            var productionUnitProvidersAux = db.ProductionUnitProvider.Where(t => (t.isActive && t.id_provider == id_provider)).ToList();

            //var priceListAux = db.PriceList.Where(t => t.id_company == this.ActiveCompanyId && t.Document.DocumentState.code.Equals("03") && t.isForPurchase &&
            //                                            (!t.isQuotation || (t.isQuotation && t.id_provider == id_provider))).ToList();//03 Codigo de APROBADA

            //var nowAux = DateTime.Now;

            //priceListAux = priceListAux.AsEnumerable().Where(w => DateTime.Compare(w.CalendarPriceList.startDate.Date, nowAux.Date) <= 0 && DateTime.Compare(nowAux.Date, w.CalendarPriceList.endDate.Date) <= 0).ToList();

            var result = new
            {
                //priceLists = priceListAux.Select(s => new {
                //                           id = s.id,
                //    name = s.name + " (" + s.Document.DocumentType.name + ") " + s.CalendarPriceList.CalendarPriceListType.name + " [" + s.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
                //                                                           s.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" + (s.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + s.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") +"]": "")

                //}),
                productionUnitProviders = productionUnitProvidersAux.Select(s => new
                {
                    id = s.id,
                    name = s.name
                }),
                id_paymentTerm = providersAux?.id_paymentTerm
            };
            //TempData["id_provider"] = result.productionUnitProviders.Select(c => c.id).FirstOrDefault();
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);
            purchaseOrder.id_productionUnitProvider = null;
            //purchaseOrder.id_productionUnitProvider = result.productionUnitProviders.Select(c => c.id).FirstOrDefault();
            //TempData.Keep("id_provider");
            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePurchaseOrderUnitProviderProtective(int? id_productionunitprovider)
        {
            var ProductionUnitProviderAux = db.ProductionUnitProvider.FirstOrDefault(t => (t.id == id_productionunitprovider));


            var INPnumber = ProductionUnitProviderAux?.INPnumber ?? "";
            var ministerialAgreement = ProductionUnitProviderAux?.ministerialAgreement ?? "";
            var tramitNumber = ProductionUnitProviderAux?.tramitNumber ?? "";
            var FishingSite = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionunitprovider)?.FishingSite?.name ?? "";
            var FishingZone = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionunitprovider)?.FishingSite?.FishingZone?.name ?? "";
            var shippingType = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionunitprovider)?.id_shippingType ?? 0;


            var result = new
            {
                address = ProductionUnitProviderAux?.address,
                SiteUnitProvider = ProductionUnitProviderAux?.SiteUnitProvider,
                FishingSite = FishingSite == "" ? null : FishingSite.ToString(),
                FishingZone = FishingZone == "" ? null : FishingZone.ToString(),
                INPnumber = INPnumber == "" ? null : INPnumber.ToString(),
                ministerialAgreement = ministerialAgreement == "" ? null : ministerialAgreement.ToString(),
                tramitNumber = tramitNumber == "" ? null : tramitNumber.ToString()
            };

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProductionUnitProvidersProtectiveByProvider(int? id_provider)
        {
            var providersAux = db.Provider.FirstOrDefault(t => (t.id == id_provider));
            var productionUnitProvidersAux = db.ProductionUnitProvider.Where(t => (t.isActive && t.id_provider == id_provider)).ToList();

            var result = new
            {
                productionUnitProviders = productionUnitProvidersAux.Select(s => new
                {
                    id = s.id,
                    name = s.name
                })
            };

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePurchaseOrderUnitProvider(int? id_productionunitprovider)
        {

            var ProductionUnitProviderAux = db.ProductionUnitProvider.FirstOrDefault(t => (t.id == id_productionunitprovider));


            var INPnumber = ProductionUnitProviderAux?.INPnumber ?? "";
            var ministerialAgreement = ProductionUnitProviderAux?.ministerialAgreement ?? "";
            var tramitNumber = ProductionUnitProviderAux?.tramitNumber ?? "";
            var FishingSite = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionunitprovider)?.FishingSite?.name ?? "";
            var FishingZone = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionunitprovider)?.FishingSite?.FishingZone?.name ?? "";
            var shippingType = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionunitprovider)?.id_shippingType ?? 0;


            var result = new
            {
                address = ProductionUnitProviderAux?.address,
                SiteUnitProvider = ProductionUnitProviderAux?.SiteUnitProvider,
                FishingSite = FishingSite == "" ? null : FishingSite.ToString(),
                FishingZone = FishingZone == "" ? null : FishingZone.ToString(),
                shippingType = shippingType == 0 ? null : shippingType.ToString(),
                INPnumber = INPnumber == "" ? null : INPnumber.ToString(),
                ministerialAgreement = ministerialAgreement == "" ? null : ministerialAgreement.ToString(),
                tramitNumber = tramitNumber == "" ? null : tramitNumber.ToString(),
                id_productionunitprovider = id_productionunitprovider
            };

            //TempData["id_provider"] = id_productionunitprovider;
            TempData["productionUnitProviderPoolreferenceList"] = DataProviderProductionUnitProviderPool.ComboProductionUnitProviderPoolByUnitProvider(id_productionunitprovider);

            var purchaseOrder = ((PurchaseOrder)TempData["purchaseOrder"]) ?? new PurchaseOrder();
            purchaseOrder.id_productionUnitProvider = id_productionunitprovider;

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");
            //TempData.Keep("id_provider");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePurchaseOrderprotectiveProvider(int? id_provider)
        {

            var TypeShrimpAux = db.ProviderTypeShrimp.FirstOrDefault(t => (t.id_provider == id_provider));

            var productionUnitProvidersAux = db.ProductionUnitProvider.Where(t => (t.isActive && t.id_provider == id_provider && (bool)t.isCopackingDetail == false)).ToList();
            var id_protectiveProvider = TypeShrimpAux?.id_protectiveProvider;
            var protectiveProvider = db.ProviderTypeShrimp.FirstOrDefault(t => (t.id_provider == id_protectiveProvider));

            var Provider_address = db.Provider.FirstOrDefault(fod => fod.id == id_provider)?.Person.address ?? "";


            var result = new
            {
                id_protectiveProvider = TypeShrimpAux?.id_protectiveProvider,
                Provider_address = Provider_address,
            };

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePurchaseOrderprotectiveProviderCopacking(int? id_provider)
        {

            var TypeShrimpAux = db.ProviderTypeShrimp.FirstOrDefault(t => (t.id_provider == id_provider));

            var productionUnitProvidersAux = db.ProductionUnitProvider.Where(t => (t.isActive && (bool)t.isCopackingDetail == true && t.id_provider == id_provider)).ToList();
            var id_protectiveProvider = TypeShrimpAux?.id_protectiveProvider;
            var protectiveProvider = db.ProviderTypeShrimp.FirstOrDefault(t => (t.id_provider == id_protectiveProvider));

            var Provider_address = db.Provider.FirstOrDefault(fod => fod.id == id_provider)?.Person.address ?? "";


            var result = new
            {
                id_protectiveProvider = TypeShrimpAux?.id_protectiveProvider,
                Provider_address = Provider_address,
            };

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCodePurchaseReason(int? id_purchaseReason)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);
            purchaseOrder = purchaseOrder ?? new PurchaseOrder();

            var listPurchaseOrderDetail = purchaseOrder.PurchaseOrderDetail.ToList();
            foreach (var purchaseOrderDetail in listPurchaseOrderDetail)
            {
                purchaseOrder.PurchaseOrderDetail.Remove(purchaseOrderDetail);
            }

            var code_purchaseReason = db.PurchaseReason.FirstOrDefault(fod => fod.id == id_purchaseReason)?.code ?? "";
            var result = new
            {
                code_purchaseReason = code_purchaseReason,
                Message = "Ok"
            };

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCodePurchaseReasonBG(int? id_purchaseReason)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);
            purchaseOrder = purchaseOrder ?? new PurchaseOrder();

            var listPurchaseOrderDetail = purchaseOrder.PurchaseOrderDetailByGrammage.ToList();
            foreach (var purchaseOrderDetail in listPurchaseOrderDetail)
            {
                purchaseOrder.PurchaseOrderDetailByGrammage.Remove(purchaseOrderDetail);
            }

            var code_purchaseReason = db.PurchaseReason.FirstOrDefault(fod => fod.id == id_purchaseReason)?.code ?? "";
            var result = new
            {
                code_purchaseReason = code_purchaseReason,
                Message = "Ok"
            };

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateViewDataEditMessage()
        {
            ViewData["EditMessage"] = WarningMessage("La Fecha de Entrega es menor a la actual.");

            var result = new
            {
                Message = WarningMessage("La Fecha de Entrega es menor a la actual.")
            };

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateSelectedRowsPurchaseRequest(int[] ids)
        {
            var result = new
            {
                Message = "OK"
            };

            InventoryLine inventoryLineFirst = null;
            InventoryLine inventoryLineCurrent = null;
            Provider providerFirst = null;
            Provider providerCurrent = null;
            int count = 0;
            foreach (var i in ids)
            {
                inventoryLineCurrent = db.PurchaseRequestDetail.FirstOrDefault(fod => fod.id == i)?.Item.InventoryLine;
                providerCurrent = db.PurchaseRequestDetail.FirstOrDefault(fod => fod.id == i)?.Provider;

                if (count == 0)
                {
                    inventoryLineFirst = inventoryLineCurrent;
                    providerFirst = providerCurrent;
                }
                else
                {
                    if (providerFirst == null)
                    {
                        providerFirst = providerCurrent;
                    }
                }
                if (inventoryLineCurrent != inventoryLineFirst && (inventoryLineFirst?.code == "MP" || inventoryLineCurrent?.code == "MP")) //"MP" : Materia Prima
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar Ítem de MATERIA PRIMA con Ítem de MATERIALES/INSUMOS")
                    };
                    TempData.Keep("purchaseOrder");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                if (providerFirst != null && providerCurrent != null && providerCurrent != providerFirst)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar detalles con proveedores propuestos diferentes")
                    };
                    TempData.Keep("purchaseOrder");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                count++;
            }

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAddressPurchaseRemisionGuideProvider(int? id_ProductionUnitProvider)
        {
            var ProductionUnitProvider_address = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_ProductionUnitProvider)?.address ?? "";
            var FishingSite = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_ProductionUnitProvider)?.id_FishingSite ?? 0;
            String nameFishingSite = "";
            String nameFishingZone = "";

            if (FishingSite > 0)
            {
                var tempFishingSite = db.FishingSite.Where(x => x.id == FishingSite).FirstOrDefault();
                if (tempFishingSite != null)
                {
                    nameFishingSite = tempFishingSite.name;
                    nameFishingZone = tempFishingSite.FishingZone.name;
                }



            }


            var result = new
            {
                ProductionUnitProvider_address = ProductionUnitProvider_address,
                Message = "Ok",
                FishingSite = FishingSite == 0 ? null : FishingSite.ToString(),
                nameFishingSite = nameFishingSite,
                nameFishingZone = nameFishingZone

            };

            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItemDetail(int? id_itemNew, int? id_purchaseRequestDetail, int? id_priceList, Boolean pricePerList)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            purchaseOrder = purchaseOrder ?? new PurchaseOrder();
            string smensaje = "";

            var result = new
            {
                itsRepeated = 0,
                Error = "",
                ErrorGeneral = ""

            };


            var purchaseRequestDetailAux = purchaseOrder.PurchaseOrderDetail.Where(w => w.id_item == id_itemNew);

            string varParSys = DataProviderSetting.ValueSetting("VPOCRC");

            if (varParSys != "1")
            {
                foreach (var detail in purchaseRequestDetailAux)
                {
                    if (detail.PurchaseOrderDetailPurchaseRequest == null || detail.PurchaseOrderDetailPurchaseRequest.Count <= 0)
                    {
                        var itemNewAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                        smensaje = "No se puede repetir el Ítem: " + itemNewAux.name +
                                    ",  sin requerimiento de compra asignado,  en los detalles.";
                        result = new
                        {
                            itsRepeated = 1,
                            Error = smensaje,
                            ErrorGeneral = ""
                        };
                    }
                }
            }

            if (pricePerList)
            {

                if (id_priceList != null && id_priceList > 0)
                {
                    if (id_itemNew > 0)
                    {
                        int idpt = db.PriceList
                                        .FirstOrDefault(fod => fod.id == id_priceList
                                        //&& fod.ProcessType.code =="ENT"
                                        )?.id_processtype ?? 0;

                        if (idpt > 0)
                        {
                            int ipt = db.ItemProcessType
                                            .FirstOrDefault(fod => fod.Id_Item == id_itemNew
                                            //&& fod.ProcessType.code.Equals("ENT")
                                            )?.Id_ProcessType ?? 0;

                            if (ipt != idpt)
                            {
                                result = new
                                {
                                    itsRepeated = 0,
                                    Error = smensaje,
                                    ErrorGeneral = "El Producto no tiene el mismo Tipo de la Lista de Precio"

                                };
                            }
                        }
                    }
                }
                else
                {

                    result = new
                    {
                        itsRepeated = 0,
                        Error = smensaje,
                        ErrorGeneral = "Seleccione la Lista de Precio."

                    };
                }
            }


            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");
            TempData.Keep("model");
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItemDetailBG(int? id_itemNew, int? id_grammage, int? id_purchaseRequestDetail, int? id_priceList, Boolean pricePerList)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            purchaseOrder = purchaseOrder ?? new PurchaseOrder();
            string smensaje = "";

            var result = new
            {
                itsRepeated = 0,
                Error = "",
                ErrorGeneral = ""

            };


            var purchaseRequestDetailAux = purchaseOrder.PurchaseOrderDetailByGrammage.FirstOrDefault(w => w.id_item == id_itemNew &&
                                                                                                  w.id_Grammage == id_grammage &&
                                                                                                  ((id_purchaseRequestDetail == null && w.PurchaseOrderDetailByGrammagePurchaseRequest.Count == 0) ||
                                                                                                  w.PurchaseOrderDetailByGrammagePurchaseRequest.FirstOrDefault(fod => fod.id_purchaseRequestDetail == id_purchaseRequestDetail) != null));

            var parametro = db.Setting.FirstOrDefault(e => e.code == "OCGR");
            if (parametro != null)
            {
                if (parametro.value == "0" && purchaseRequestDetailAux != null)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No. Requerimiento: " + (purchaseRequestDetailAux.PurchaseOrderDetailByGrammagePurchaseRequest?.FirstOrDefault()?.PurchaseRequest.Document.number ?? "") +
            ", Nombre del Producto: " + purchaseRequestDetailAux.Item.name +
            ", Gramaje: " + purchaseRequestDetailAux.Grammage.code + ", esta repetido en otro detalle.",
                        ErrorGeneral = ""

                    };
                }
            }
            //if (purchaseRequestDetailAux != null)
            //{
            //    result = new
            //    {
            //        itsRepeated = 1,
            //        Error = "No. Requerimiento: " + (purchaseRequestDetailAux.PurchaseOrderDetailByGrammagePurchaseRequest?.FirstOrDefault()?.PurchaseRequest.Document.number ?? "") +
            //                ", Nombre del Producto: " + purchaseRequestDetailAux.Item.name +
            //                ", Gramaje: " + purchaseRequestDetailAux.Grammage.code + ", esta repetido en otro detalle.",
            //        ErrorGeneral = ""

            //    };
            //}
            if (pricePerList)
            {

                if (id_priceList != null && id_priceList > 0)
                {
                    if (id_itemNew > 0)
                    {
                        int idpt = db.PriceList
                                        .FirstOrDefault(fod => fod.id == id_priceList
                                        //&& fod.ProcessType.code =="ENT"
                                        )?.id_processtype ?? 0;

                        if (idpt > 0)
                        {
                            int ipt = db.ItemProcessType
                                            .FirstOrDefault(fod => fod.Id_Item == id_itemNew)?
                                            .Id_ProcessType ?? 0;
                            if (ipt != idpt)
                            {
                                result = new
                                {
                                    itsRepeated = 0,
                                    Error = smensaje,
                                    ErrorGeneral = "El Producto no tiene el mismo Tipo de la Lista de Precio"

                                };
                            }
                        }
                    }
                }
                else
                {

                    result = new
                    {
                        itsRepeated = 0,
                        Error = smensaje,
                        ErrorGeneral = "Seleccione la Lista de Precio."

                    };
                }
            }


            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");
            TempData.Keep("model");
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetPriceListsByProvider(int? id_provider, DateTime emissionDate)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);

            purchaseOrder = purchaseOrder ?? new PurchaseOrder();

            if (id_provider == null || id_provider < 0)
            {
                if (Request.Params["id_provider"] != null && Request.Params["id_provider"] != "")
                    id_provider = int.Parse(Request.Params["id_provider"]);
                else id_provider = -1;
            }

            purchaseOrder.Document.emissionDate = emissionDate;

            purchaseOrder.id_provider = (int)id_provider;

            TempData["purchaseOrder"] = purchaseOrder;
            TempData.Keep("purchaseOrder");

            return PartialView("comboboxcascading/_cmbPriceListByProvider", purchaseOrder);
        }

        [HttpPost]
        public ActionResult GetCertificationsByPriceList(int? id_priceList, int? id_certificationCurrent)
        {
            TempData.Keep("purchaseOrder");

            var aPriceList = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
            var id_certificationPriceList = aPriceList?.id_certification;
            var items = db.Certification.Where(w => w.id == id_certificationPriceList || w.id == id_certificationCurrent || w.isActive).ToList();
            Session["id_certificationUpdate"] = id_certificationPriceList ?? id_certificationCurrent;
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_certification";
                p.Width = Unit.Percentage(100);
                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);
                p.CallbackRouteValues = new { Controller = "PurchaseOrder", Action = "GetCertificationsByPriceList" };
                p.ClientSideEvents.BeginCallback = "function(s, e) { e.customArgs['id_priceList'] = id_priceList.GetValue(); e.customArgs['id_certificationCurrent'] = id_certification.GetValue();}";
                p.ClientSideEvents.EndCallback = "OnCertification_EndCallback";
                p.CallbackPageSize = 5;
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                p.BindList(items);// .Bind(id_person).ge;
            });
        }

        public JsonResult UpdateCertification()
        {
            TempData.Keep("purchaseOrder");

            var result = new
            {
                id_certificationUpdate = (int?)Session["id_certificationUpdate"]
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Calculo Gramage
        [HttpPost, ValidateInput(false)]
        public JsonResult PriceCalculationGramage(int id_grammage
            , decimal Libras, int? id_processtype
            , int? id_ListPrice, int? id_Item
            , int? idProvider)
        {
            decimal grammageCurrent = 0;
            decimal price = 0;
            string mensaje = "";

            //Aquí habrá cambios

            try
            {
                var aGrammage = db.Grammage.FirstOrDefault(fod => fod.id == id_grammage);
                grammageCurrent = aGrammage?.value ?? 0;
                price = DataProviders.DataProviderFanSeriesGrammage.FanSeriesGrammageGetValueFixed(id_grammage, Libras
                    , id_processtype, id_ListPrice, id_Item, idProvider);
            }
            catch (Exception ex)
            {
                mensaje = ex.Message.ToString();

            }

            var result = new
            {
                price = price,
                mensaje = mensaje,
                grammageCurrent

            };
            TempData.Keep("model");


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdatePriceListThird()
        {
            string strCon = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string strRLog = ConfigurationManager.AppSettings["rutaLog"];

            ServicePriceListThird.HomologationUpdatePriceList(strCon, strRLog);

            var result = new { result = "OK" };

            return null;
        }
        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult StateOfCmbProductUnitPool(int? id_productionUnitProvider)
        {
            
            TempData["productionUnitProviderPoolreferenceList"] = DataProviderProductionUnitProviderPool.ComboProductionUnitProviderPoolByUnitProvider(id_productionUnitProvider);


            TempData.Keep("productionUnitProviderPoolreferenceList");
            ProductionUnitProviderPool ProductionUnitProviderPool = (TempData["productionUnitProviderPoolreferenceList"] as ProductionUnitProviderPool);

            return Json(ProductionUnitProviderPool, JsonRequestBehavior.AllowGet);

        }
        
        
        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateCertificationProtectiveByProvider(int? id_certification)
        {
            PurchaseOrder purchaseOrder = (TempData["purchaseOrder"] as PurchaseOrder);
            purchaseOrder.id_certification = id_certification;

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            var result = new
            {
                msg="OK"
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }

}
