using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Services
{
    public class ServicePurchaseRequest
    {
        public static PurchaseRequest CreatePurchaseRequestFromProductionScheduleScheduleDetail(DBContext db, User activeUser, Company activeCompany, EmissionPoint activeEmissionPoint, List<ProductionScheduleScheduleDetail> productionScheduleScheduleDetail)
        {
            string result = "";
            PurchaseRequest purchaseRequest = new PurchaseRequest();
            //var purchaseRequestDetail = productionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail?.FirstOrDefault()?.SalesRequestDetail;
            try
            {
                #region Document

                Document document = new Document();
                document.id_userCreate = activeUser.id;
                document.dateCreate = DateTime.Now;
                document.id_userUpdate = activeUser.id;
                document.dateUpdate = DateTime.Now;

                DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == "01");//Requerimiento de Compras
                if (documentType == null)
                {
                    throw new Exception("No se puedo Crear el Documento Requerimiento de Compras porque no existe el Tipo de Documento: Requerimiento de Compras con Código: 01, configúrelo e inténtelo de nuevo");
                }
                document.id_documentType = documentType.id;
                document.DocumentType = documentType;
                document.sequential = GetDocumentSequential(document.id_documentType, db, activeCompany);
                document.number = GetDocumentNumber(document.id_documentType, db, activeCompany, activeEmissionPoint);


                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE
                if (documentState == null)
                {
                    throw new Exception("No se puedo Crear el Documento Orden de Producción porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                }
                document.id_documentState = documentState.id;
                document.DocumentState = documentState;

                document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == activeEmissionPoint.id);
                document.id_emissionPoint = activeEmissionPoint.id;

                document.emissionDate = productionScheduleScheduleDetail.FirstOrDefault().datePlanning;//DateTime.Now;
                document.description = null;

                //Actualiza Secuencial
                if (documentType != null)
                {
                    documentType.currentNumber = documentType.currentNumber + 1;
                    db.DocumentType.Attach(documentType);
                    db.Entry(documentType).State = EntityState.Modified;
                }

                #endregion

                #region PurchaseRequest

                purchaseRequest.id = document.id;
                purchaseRequest.Document = document;

                purchaseRequest.id_personRequesting = activeUser.id_employee.Value;
                purchaseRequest.Employee = activeUser.Employee;

                //lot.id_company = activeCompany.id;
                #endregion

                #region SalesOrderDetails

                
                //var details = tempRequest.PurchaseRequestDetail.ToList();

                foreach (var detail in productionScheduleScheduleDetail)
                {
                    //PurchaseRequestDetail requestDetail = modelItem.PurchaseRequestDetail.FirstOrDefault(d => d.id == detail.id);
                    //var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                    //if (approve && detail.quantityApproved <= 0)
                    //{
                    //    TempData.Keep("request");
                    //    ViewData["EditMessage"] = ErrorMessage("No se puede aprobar el requerimiento de compra, Ítem: " + itemAux.name + " debe tener la Cantidad Aprobada mayor que cero.");
                    //    return PartialView("_PurchaseRequestMainFormPartial", tempRequest);
                    //}
                    //if (requestDetail == null)
                    //{
                    var itemAux = db.Item.FirstOrDefault(fod => fod.id == detail.id_item);
                    var purchaseRequestDetail = new PurchaseRequestDetail
                    {
                        id_item = detail.id_item,
                        Item = itemAux,

                        id_proposedProvider = detail.id_provider,
                        Provider = db.Provider.FirstOrDefault(fod => fod.id == detail.id_provider),

                        proposedPrice = itemAux.ItemPurchaseInformation.purchasePrice.Value,

                        quantityRequested = detail.quantity,
                        quantityApproved = detail.quantity,
                        quantityOutstandingPurchase = detail.quantity,

                        isActive = true,
                        id_userCreate = activeUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = activeUser.id,
                        dateUpdate = DateTime.Now,

                        ProductionScheduleScheduleDetailPurchaseRequestDetail    = new List<ProductionScheduleScheduleDetailPurchaseRequestDetail>()

                    };

                    var productionScheduleScheduleDetailPurchaseRequestDetail = (new ProductionScheduleScheduleDetailPurchaseRequestDetail
                    {
                        id_productionScheduleScheduleDetail = detail.id,
                        ProductionScheduleScheduleDetail = detail,
                        id_productionSchedule = detail.id_productionSchedule,
                        ProductionSchedule = detail.ProductionSchedule,
                        id_purchaseRequestDetail = purchaseRequestDetail.id,
                        PurchaseRequestDetail = purchaseRequestDetail,

                        quantity = detail.quantity
                    });


                    //if (requestDetail.isActive)
                    //{
                    purchaseRequestDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Add(productionScheduleScheduleDetailPurchaseRequestDetail);
                    detail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Add(productionScheduleScheduleDetailPurchaseRequestDetail);
                    //    count++;
                    //}

                    //}
                    //else
                    //{
                    //    requestDetail.id_item = detail.id_item;
                    //    requestDetail.id_proposedProvider = detail.id_proposedProvider;
                    //    requestDetail.proposedPrice = detail.proposedPrice;

                    //    requestDetail.quantityRequested = detail.quantityRequested;
                    //    requestDetail.quantityApproved = detail.quantityApproved;
                    //    requestDetail.quantityOutstandingPurchase = detail.quantityOutstandingPurchase;

                    //    requestDetail.isActive = detail.isActive;
                    //    requestDetail.id_userCreate = detail.id_userCreate;
                    //    requestDetail.dateCreate = detail.dateCreate;
                    //    requestDetail.id_userUpdate = detail.id_userUpdate;
                    //    requestDetail.dateUpdate = detail.dateUpdate;

                    //    if (requestDetail.isActive)
                    //        count++;
                    //}
                    purchaseRequest.PurchaseRequestDetail.Add(purchaseRequestDetail);

                }
                //}

                //if (tempItemPODetail.isActive)
                //}
                //}

                #endregion

                db.PurchaseRequest.Add(purchaseRequest);
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            return purchaseRequest;
        }

        public static void UpdatePurchaseRequestDetail(User ActiveUser, Company ActiveCompany, EmissionPoint ActiveEmissionPoint, List<ProductionScheduleScheduleDetail> productionScheduleScheduleDetail, DBContext db, bool reverse)
        {
            string result = "";
            SalesOrder salesOrder = new SalesOrder();
            try
            {
                if (reverse)
                {
                    for (int i = productionScheduleScheduleDetail.Count - 1; i >= 0; i--)
                    {
                        var detail = productionScheduleScheduleDetail.ElementAt(i);

                        var purchaseRequestDetailAux = detail.ProductionScheduleScheduleDetailPurchaseRequestDetail?.FirstOrDefault(fod => fod.PurchaseRequestDetail.PurchaseRequest.Document.DocumentState.code.Equals("03") ||//APROBADA
                                                                                                                                           fod.PurchaseRequestDetail.PurchaseRequest.Document.DocumentState.code.Equals("06"));//AUTORIZADA

                        if (purchaseRequestDetailAux != null)
                        {
                            throw new Exception("No puede Reversarse la Programación de Producción debido a tener asociada Requerimiento de Compra Aprobado o Autorizado. Revéreselo e inténtelo de nuevo.");
                        }

                    }

                    //var itemDetail = inventoryMoveToReverse.InventoryMoveDetail.ToList();

                    //foreach (var i in itemDetail)
                    //{


                    //    //Actualiza Referencia de los movimentos de inventario anterior y proximo de i para de esta forma estaría fuera de la cadena de movimientos de inventarios detalles
                    //    RemoveInventoryMoveDetail(i, db, ActiveUser);

                    //    //Actulizar Stock del Item
                    //    UpdateStockItem(i.Item, i.exitAmount, db);

                    //}

                    //inventoryMove = inventoryMoveToReverse;

                }
                else
                {
                    List<ProductionScheduleScheduleDetail> productionScheduleScheduleDetailAux = new List<ProductionScheduleScheduleDetail>();
                    foreach (var detail in productionScheduleScheduleDetail)
                    {
                        var purchaseRequestDetailAux = detail.ProductionScheduleScheduleDetailPurchaseRequestDetail.FirstOrDefault()?.PurchaseRequestDetail;
                        if (purchaseRequestDetailAux != null)
                        {
                            purchaseRequestDetailAux.quantityRequested = detail.quantity;
                            db.PurchaseRequestDetail.Attach(purchaseRequestDetailAux);
                            db.Entry(purchaseRequestDetailAux).State = EntityState.Modified;

                            var productionScheduleScheduleDetailPurchaseRequestDetail = detail.ProductionScheduleScheduleDetailPurchaseRequestDetail.FirstOrDefault();
                            productionScheduleScheduleDetailPurchaseRequestDetail.quantity = detail.quantity;
                            db.ProductionScheduleScheduleDetailPurchaseRequestDetail.Attach(productionScheduleScheduleDetailPurchaseRequestDetail);
                            db.Entry(productionScheduleScheduleDetailPurchaseRequestDetail).State = EntityState.Modified;
                        }
                        else
                        {
                            var datePlanningAux = productionScheduleScheduleDetailAux.FirstOrDefault()?.datePlanning.Date;
                            if (productionScheduleScheduleDetailAux.Count() > 0 &&
                                (DateTime.Compare(datePlanningAux.Value, detail.datePlanning.Date) != 0))
                            {
                                CreatePurchaseRequestFromProductionScheduleScheduleDetail(db, ActiveUser, ActiveCompany, ActiveEmissionPoint, productionScheduleScheduleDetailAux);
                                productionScheduleScheduleDetailAux = new List<ProductionScheduleScheduleDetail>();
                            }
                            productionScheduleScheduleDetailAux.Add(detail);
                        }
                    }
                    if (productionScheduleScheduleDetailAux.Count() > 0)
                    {
                        CreatePurchaseRequestFromProductionScheduleScheduleDetail(db, ActiveUser, ActiveCompany, ActiveEmissionPoint, productionScheduleScheduleDetailAux);
                    }
                }
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            //return salesOrder;
        }

        public static PurchaseRequest CreatePurchaseRequestFromBusinessOportunityPlanningDetail(DBContext db, User activeUser, Company activeCompany, EmissionPoint activeEmissionPoint, List<BusinessOportunityPlanningDetail> businessOportunityPlanningDetail, PurchaseRequest purchaseRequest)
        {
            string result = "";
            PurchaseRequest purchaseRequestAux = new PurchaseRequest();
            try
            {
                if(purchaseRequest == null)
                {
                    #region Document

                    Document document = new Document();
                    document.id_userCreate = activeUser.id;
                    document.dateCreate = DateTime.Now;
                    document.id_userUpdate = activeUser.id;
                    document.dateUpdate = DateTime.Now;

                    DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == "01");//Requerimiento de Compras
                    if (documentType == null)
                    {
                        throw new Exception("No se puedo Crear el Documento Requerimiento de Compras porque no existe el Tipo de Documento: Requerimiento de Compras con Código: 01, configúrelo e inténtelo de nuevo");
                    }
                    document.id_documentType = documentType.id;
                    document.DocumentType = documentType;
                    document.sequential = GetDocumentSequential(document.id_documentType, db, activeCompany);
                    document.number = GetDocumentNumber(document.id_documentType, db, activeCompany, activeEmissionPoint);


                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE
                    if (documentState == null)
                    {
                        throw new Exception("No se puedo Crear el Documento Orden de Producción porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                    }
                    document.id_documentState = documentState.id;
                    document.DocumentState = documentState;

                    document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == activeEmissionPoint.id);
                    document.id_emissionPoint = activeEmissionPoint.id;

                    document.emissionDate = DateTime.Now;
                    document.description = null;

                    //Actualiza Secuencial
                    if (documentType != null)
                    {
                        documentType.currentNumber = documentType.currentNumber + 1;
                        db.DocumentType.Attach(documentType);
                        db.Entry(documentType).State = EntityState.Modified;
                    }

                    #endregion

                    #region PurchaseRequest

                    purchaseRequestAux.id = document.id;
                    purchaseRequestAux.Document = document;

                    purchaseRequestAux.id_personRequesting = activeUser.id_employee.Value;
                    purchaseRequestAux.Employee = activeUser.Employee;

                    //lot.id_company = activeCompany.id;
                    #endregion
                }
                else
                {
                    purchaseRequestAux = purchaseRequest;
                }


                #region PurchaseRequestDetail


                foreach (var detail in businessOportunityPlanningDetail)
                {
                    var itemAux = db.Item.FirstOrDefault(fod => fod.id == detail.id_item);
                    var purchaseRequestDetail = new PurchaseRequestDetail
                    {
                        id_item = detail.id_item,
                        Item = itemAux,

                        id_proposedProvider = detail.id_person,
                        Provider = db.Provider.FirstOrDefault(fod => fod.id == detail.id_person),

                        proposedPrice = detail.price,

                        quantityRequested = detail.quantity,
                        quantityApproved = detail.quantity,
                        quantityOutstandingPurchase = detail.quantity,

                        isActive = true,
                        id_userCreate = activeUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = activeUser.id,
                        dateUpdate = DateTime.Now,

                        PurchaseRequestDetailBusinessOportunity = new List<PurchaseRequestDetailBusinessOportunity>()

                    };

                    var purchaseRequestDetailBusinessOportunity = (new PurchaseRequestDetailBusinessOportunity
                    {
                        id_businessOportunityPlanningDetail = detail.id,
                        BusinessOportunityPlanningDetail = detail,
                        id_businessOportunity = detail.BusinessOportunityPlaninng.id,
                        BusinessOportunity = detail.BusinessOportunityPlaninng.BusinessOportunity,
                        id_purchaseRequestDetail = purchaseRequestDetail.id,
                        PurchaseRequestDetail = purchaseRequestDetail,

                        quantity = detail.quantity
                    });
                   
                    purchaseRequestDetail.PurchaseRequestDetailBusinessOportunity.Add(purchaseRequestDetailBusinessOportunity);
                    detail.PurchaseRequestDetailBusinessOportunity.Add(purchaseRequestDetailBusinessOportunity);

                    purchaseRequestAux.PurchaseRequestDetail.Add(purchaseRequestDetail);

                }

                #endregion

                if (purchaseRequest == null)
                {
                    db.PurchaseRequest.Add(purchaseRequestAux);
                }
                else
                {
                    db.PurchaseRequest.Attach(purchaseRequestAux);
                    db.Entry(purchaseRequestAux).State = EntityState.Modified;
                }
                    
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            return purchaseRequestAux;
        }

        public static void UpdatePurchaseRequestDetail(User ActiveUser, Company ActiveCompany, EmissionPoint ActiveEmissionPoint, BusinessOportunityPlaninng businessOportunityPlaninng, DBContext db, bool reverse)
        {
            string result = "";
            //SalesOrder salesOrder = new SalesOrder();
            try
            {
                if (reverse)
                {
                    if (businessOportunityPlaninng.BusinessOportunityPlanningDetail != null && businessOportunityPlaninng.BusinessOportunityPlanningDetail.Count() > 0)
                    {
                        for (int i = businessOportunityPlaninng.BusinessOportunityPlanningDetail.Count - 1; i >= 0; i--)
                        {
                            var detail = businessOportunityPlaninng.BusinessOportunityPlanningDetail.ElementAt(i);

                            var purchaseRequestDetailAux = detail.PurchaseRequestDetailBusinessOportunity.FirstOrDefault(fod => fod.PurchaseRequestDetail.PurchaseRequest.Document.DocumentState.code.Equals("03") ||//APROBADA
                                                                                                                                 fod.PurchaseRequestDetail.PurchaseRequest.Document.DocumentState.code.Equals("06"));//AUTORIZADA

                            if (purchaseRequestDetailAux != null)
                            {
                                throw new Exception("No puede Reversarse la Oportunidad debido a tener asociada Requerimiento de Compra Aprobado o Autorizado. Revéreselo e inténtelo de nuevo.");
                            }

                        }
                    }
                }
                else
                {
                    if (businessOportunityPlaninng.BusinessOportunityPlanningDetail != null && businessOportunityPlaninng.BusinessOportunityPlanningDetail.Count() > 0)
                    {
                        List<BusinessOportunityPlanningDetail> businessOportunityPlanningDetailAux = new List<BusinessOportunityPlanningDetail>();
                        PurchaseRequest purchaseRequestAux = null;
                        foreach (var detail in businessOportunityPlaninng.BusinessOportunityPlanningDetail)
                        {
                            var purchaseRequestDetailAux = detail.PurchaseRequestDetailBusinessOportunity.FirstOrDefault()?.PurchaseRequestDetail;
                            if (purchaseRequestDetailAux != null)
                            {
                                purchaseRequestAux = purchaseRequestDetailAux.PurchaseRequest;
                                purchaseRequestDetailAux.id_item = detail.id_item;
                                purchaseRequestDetailAux.Item = db.Item.FirstOrDefault(fod=> fod.id == detail.id_item);
                                purchaseRequestDetailAux.id_proposedProvider = detail.id_person;
                                purchaseRequestDetailAux.Provider = db.Provider.FirstOrDefault(fod => fod.id == detail.id_person);
                                purchaseRequestDetailAux.proposedPrice = detail.price;
                                purchaseRequestDetailAux.quantityRequested = detail.quantity;
                                db.PurchaseRequestDetail.Attach(purchaseRequestDetailAux);
                                db.Entry(purchaseRequestDetailAux).State = EntityState.Modified;

                                var purchaseRequestDetailBusinessOportunity = detail.PurchaseRequestDetailBusinessOportunity.FirstOrDefault();
                                purchaseRequestDetailBusinessOportunity.quantity = detail.quantity;
                                db.PurchaseRequestDetailBusinessOportunity.Attach(purchaseRequestDetailBusinessOportunity);
                                db.Entry(purchaseRequestDetailBusinessOportunity).State = EntityState.Modified;
                            }
                            else
                            {
                                //var datePlanningAux = productionScheduleScheduleDetailAux.FirstOrDefault()?.datePlanning.Date;
                                //if (productionScheduleScheduleDetailAux.Count() > 0 &&
                                //    (DateTime.Compare(datePlanningAux.Value, detail.datePlanning.Date) != 0))
                                //{
                                    //CreatePurchaseRequestFromProductionScheduleScheduleDetail(db, ActiveUser, ActiveCompany, ActiveEmissionPoint, productionScheduleScheduleDetailAux);
                                    //productionScheduleScheduleDetailAux = new List<ProductionScheduleScheduleDetail>();
                                //}
                                businessOportunityPlanningDetailAux.Add(detail);
                            }
                        }
                        if (businessOportunityPlanningDetailAux.Count() > 0)
                        {
                            purchaseRequestAux = CreatePurchaseRequestFromBusinessOportunityPlanningDetail(db, ActiveUser, ActiveCompany, ActiveEmissionPoint, businessOportunityPlanningDetailAux, purchaseRequestAux);
                            foreach (var detail in businessOportunityPlanningDetailAux)
                            {
                                detail.id_document = purchaseRequestAux.Document.id;
                                detail.Document = purchaseRequestAux.Document;
                                db.BusinessOportunityPlanningDetail.Attach(detail);
                                db.Entry(detail).State = EntityState.Modified;
                            }
                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            //return salesOrder;
        }

        #region Auxiliar

        private static decimal SalesDetailIVA(int id_item, decimal quantity, decimal price, DBContext db)
        {
            decimal iva = 0.0M;

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return 0.0M;
            }

            List<ItemTaxation> taxations = item.ItemTaxation.Where(t=> t.TaxType.code.Equals("2")).ToList();//"2": Código del IVA

            foreach (var taxation in taxations)
            {
                iva += quantity * price * taxation.Rate.percentage / 100.0M;
            }

            return iva;
        }

        private static SalesOrderTotal SalesOrderTotals(int id_salesOrder, List<SalesOrderDetail> salesOrderDetails, DBContext db)
        {
            SalesOrderTotal salesOrderTotal = db.SalesOrderTotal.FirstOrDefault(t => t.id_salesOrder == id_salesOrder);

            salesOrderTotal = salesOrderTotal ?? new SalesOrderTotal
            {
                id_salesOrder = id_salesOrder
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

            foreach (var detail in salesOrderDetails)
            {
                if (detail.Item != null && detail.isActive)
                {
                    ItemTaxation rateIVA12 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("2"));//"2": IVA "2": IVA 12%
                    ItemTaxation rateIVA14 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("5"));//"2": IVA "5": IVA 14%

                    if (rateIVA12 != null)
                    {
                        subtotalIVA12Percent += detail.quantityOrdered * detail.price;
                    }

                    if (rateIVA14 != null)
                    {
                        subtotalIVA14Percent += detail.quantityOrdered * detail.price;
                    }
                     
                    subtotal += detail.quantityOrdered * detail.price;
                }

            }

            totalIVA12 = subtotalIVA12Percent * 0.12M;
            totalIVA14 = subtotalIVA14Percent * 0.14M;

            total = subtotal + totalIVA12 + totalIVA14 + valueICE + valueIRBPNR - discount;

            salesOrderTotal.subtotalIVA12Percent = subtotalIVA12Percent;
            salesOrderTotal.subtotalIVA14Percent = subtotalIVA14Percent;
            salesOrderTotal.subtotalIVA0Percent = subtotalIVA0Percent;
            salesOrderTotal.subtotalIVANoObjectIVA = subtotalIVANoObjectIVA;
            salesOrderTotal.subtotalExentedIVA = subtotalExentedIVA;

            salesOrderTotal.subtotal = subtotal;
            salesOrderTotal.discount = discount;
            salesOrderTotal.valueICE = valueICE;
            salesOrderTotal.valueIRBPNR = valueIRBPNR;

            salesOrderTotal.totalIVA12 = totalIVA12;
            salesOrderTotal.totalIVA14 = totalIVA14;

            salesOrderTotal.total = total;

            return salesOrderTotal;
        }

        protected static int GetDocumentSequential(int id_documentType, DBContext db, Company activeCompany)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.id == id_documentType && d.id_company == activeCompany.id);
            return documentType?.currentNumber ?? 0;
        }

        protected static string GetDocumentNumber(int id_documentType, DBContext db, Company activeCompany, EmissionPoint activeEmissionPoint)
        {
            string number = GetDocumentSequential(id_documentType, db, activeCompany).ToString().PadLeft(9, '0');
            string documentNumber = string.Empty;
            documentNumber = $"{activeEmissionPoint.BranchOffice.code.ToString().PadLeft(3, '0')}-{activeEmissionPoint.code.ToString().PadLeft(3, '0')}-{number}";
            return documentNumber;
        }

        #endregion

    }
}