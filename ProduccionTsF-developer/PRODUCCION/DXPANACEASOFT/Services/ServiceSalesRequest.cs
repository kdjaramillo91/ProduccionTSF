using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Services
{
    public class ServiceSalesRequest
    {
        public static SalesRequest CreateSalesRequestFromBusinessOportunityPlanningDetail(DBContext db, User activeUser, Company activeCompany, EmissionPoint activeEmissionPoint, BusinessOportunityPlanningDetail businessOportunityPlanningDetail)
        {
            string result = "";
            SalesRequest salesRequest = new SalesRequest();
            try
            {
                #region Document

                Document document = new Document();
                document.id_userCreate = activeUser.id;
                document.dateCreate = DateTime.Now;
                document.id_userUpdate = activeUser.id;
                document.dateUpdate = DateTime.Now;

                DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == "10");//Requerimiento de Pedido
                if (documentType == null)
                {
                    throw new Exception("No se puedo Crear el Documento Requerimiento de Pedido porque no existe el Tipo de Documento: Requerimiento de Pedido con Código: 10, configúrelo e inténtelo de nuevo");
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

                #region SalesRequest


                salesRequest.id = document.id;
                salesRequest.Document = document;

                var id_customerAux = businessOportunityPlanningDetail.id_person;
                salesRequest.id_customer = id_customerAux;
                salesRequest.Customer = db.Customer.FirstOrDefault(fod => fod.id == id_customerAux);
                var id_priceListAux = businessOportunityPlanningDetail.id_priceList;
                salesRequest.id_priceList = id_priceListAux;
                salesRequest.PriceList = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListAux);
                var id_employeeSellerAux = activeUser.id_employee;
                salesRequest.id_employeeSeller = id_employeeSellerAux;
                salesRequest.Employee = db.Employee.FirstOrDefault(fod => fod.id == id_employeeSellerAux);
                //var id_priceListAux = null;
               //salesRequest.id_priceList = null;
                //salesRequest.PriceList = null;

                //lot.id_company = activeCompany.id;
                #endregion

                #region SalesRequestDetails
                var quantityPresentation = businessOportunityPlanningDetail.quantity * (businessOportunityPlanningDetail.Item.Presentation?.minimum ?? 1);
                var pricePresentation = businessOportunityPlanningDetail.price / (businessOportunityPlanningDetail.Item.Presentation?.minimum ?? 1);
                int? id_metricUnitPresentation = (int?)businessOportunityPlanningDetail.Item.Presentation.id_metricUnit ?? businessOportunityPlanningDetail.Item.ItemSaleInformation.id_metricUnitSale;

                decimal _price = pricePresentation;
                decimal _iva = SalesDetailIVA(businessOportunityPlanningDetail.id_item,
                                              quantityPresentation,
                                              _price, db);
                var tempItemPODetail = new SalesRequestDetail
                {
                    id_item = businessOportunityPlanningDetail.id_item,
                    Item = db.Item.FirstOrDefault(i => i.id == businessOportunityPlanningDetail.id_item),

                    quantityTypeUMSale = businessOportunityPlanningDetail.quantity,
                    quantityRequested = quantityPresentation,
                    quantityApproved = quantityPresentation,
                    quantityOutstandingOrder = quantityPresentation,
                    id_metricUnitTypeUMPresentation = id_metricUnitPresentation.Value,
                    MetricUnit = db.MetricUnit.FirstOrDefault(fod=> fod.id == id_metricUnitPresentation),

                    price = _price,
                    iva = _iva,

                    subtotal = _price * quantityPresentation,
                    total = (_price * quantityPresentation) + _iva,

                    isActive = true,
                    id_userCreate = activeUser.id,
                    dateCreate = DateTime.Now,
                    id_userUpdate = activeUser.id,
                    dateUpdate = DateTime.Now,

                    SalesRequestDetailBusinessOportunity = new List<SalesRequestDetailBusinessOportunity>()
                };

                
                var salesRequestDetailBusinessOportunity = (new SalesRequestDetailBusinessOportunity
                {
                    id_businessOportunityPlanningDetail = businessOportunityPlanningDetail.id,
                    BusinessOportunityPlanningDetail = businessOportunityPlanningDetail,
                    id_businessOportunity = businessOportunityPlanningDetail.BusinessOportunityPlaninng.id,
                    BusinessOportunity = businessOportunityPlanningDetail.BusinessOportunityPlaninng.BusinessOportunity,
                    SalesRequestDetail = tempItemPODetail,
                    quantity = businessOportunityPlanningDetail.quantity
                });

                tempItemPODetail.SalesRequestDetailBusinessOportunity.Add(salesRequestDetailBusinessOportunity);
                businessOportunityPlanningDetail.SalesRequestDetailBusinessOportunity.Add(salesRequestDetailBusinessOportunity);

                salesRequest.SalesRequestDetail.Add(tempItemPODetail);


                #endregion

                #region TOTALS

                salesRequest.SalesRequestTotal = SalesRequestTotals(salesRequest.id, salesRequest.SalesRequestDetail.ToList(), db);

                #endregion

                db.SalesRequest.Add(salesRequest);
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            return salesRequest;
        }

        public static void UpdateSalesRequestDetail(User activeUser, Company activeCompany, EmissionPoint activeEmissionPoint, BusinessOportunityPlaninng businessOportunityPlaninng, DBContext db, bool reverse)
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

                            var salesRequestDetailAux = detail.SalesRequestDetailBusinessOportunity.FirstOrDefault(fod => fod.SalesRequestDetail.SalesRequest.Document.DocumentState.code.Equals("03") ||//APROBADA
                                                                                                                          fod.SalesRequestDetail.SalesRequest.Document.DocumentState.code.Equals("06"));//AUTORIZADA

                            if (salesRequestDetailAux != null)
                            {
                                throw new Exception("No puede Reversarse la Oportunidad debido a tener asociada Requerimiento de Pedido de Venta Aprobado o Autorizado. Revéreselo e inténtelo de nuevo.");
                            }

                        }
                    }
                }
                else
                {
                    if (businessOportunityPlaninng.BusinessOportunityPlanningDetail != null && businessOportunityPlaninng.BusinessOportunityPlanningDetail.Count() > 0)
                    {
                        List<BusinessOportunityPlanningDetail> businessOportunityPlanningDetailAux = new List<BusinessOportunityPlanningDetail>();
                        
                        foreach (var detail in businessOportunityPlaninng.BusinessOportunityPlanningDetail)
                        {
                            var salesRequestDetailAux = detail.SalesRequestDetailBusinessOportunity.FirstOrDefault()?.SalesRequestDetail;
                            SalesRequest salesRequestAux = null;
                            var quantityPresentation = detail.quantity * (detail.Item.Presentation?.minimum ?? 1);
                            var pricePresentation = detail.price / (detail.Item.Presentation?.minimum ?? 1);
                            if (salesRequestDetailAux != null)
                            {
                                salesRequestAux = salesRequestDetailAux.SalesRequest;
                                
                                #region SalesRequest

                                salesRequestAux.id_customer = detail.id_person;
                                salesRequestAux.Customer = db.Customer.FirstOrDefault(fod => fod.id == detail.id_person);

                                salesRequestAux.id_priceList = detail.id_priceList;
                                salesRequestAux.PriceList = db.PriceList.FirstOrDefault(fod => fod.id == detail.id_priceList);
                                //var id_employeeSellerAux = salesRequestDetail?.SalesRequest.id_employeeSeller;
                                //salesOrder.id_employeeSeller = id_employeeSellerAux;
                                //salesOrder.Employee = db.Employee.FirstOrDefault(fod => fod.id == id_employeeSellerAux);
                                //var id_priceListAux = salesRequestDetail?.SalesRequest.id_priceList;
                                //salesOrder.id_priceList = id_priceListAux;
                                //salesOrder.PriceList = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListAux);
                                //salesOrder.requiredLogistic = false;

                                //lot.id_company = activeCompany.id;
                                #endregion

                                #region SalesRequestDetails

                                decimal _price = pricePresentation;
                                decimal _iva = SalesDetailIVA(detail.id_item,
                                                            salesRequestDetailAux.quantityApproved,
                                                            _price, db);

                                salesRequestDetailAux.id_item = detail.id_item;
                                salesRequestDetailAux.Item = db.Item.FirstOrDefault(i => i.id == detail.id_item);

                                salesRequestDetailAux.quantityTypeUMSale = detail.quantity;
                                salesRequestDetailAux.quantityRequested = quantityPresentation;
                                //salesRequestDetailAux.quantityApproved = detail.quantity;
                                //salesRequestDetailAux.quantityOutstandingOrder = detail.quantity;

                                salesRequestDetailAux.price = _price;
                                salesRequestDetailAux.iva = _iva;

                                salesRequestDetailAux.subtotal = _price * salesRequestDetailAux.quantityApproved;
                                salesRequestDetailAux.total = salesRequestDetailAux.subtotal + _iva;

                                salesRequestDetailAux.isActive = true;
                                salesRequestDetailAux.id_userUpdate = activeUser.id;
                                salesRequestDetailAux.dateUpdate = DateTime.Now;

                                //var salesRequestDetailBusinessOportunity = detail.SalesRequestDetailBusinessOportunity.FirstOrDefault();
                                //salesRequestDetailBusinessOportunity.quantity = detail.quantity;
                                //db.SalesRequestDetailBusinessOportunity.Attach(salesRequestDetailBusinessOportunity);
                                //db.Entry(salesRequestDetailBusinessOportunity).State = EntityState.Modified;

                                db.SalesRequestDetail.Attach(salesRequestDetailAux);
                                db.Entry(salesRequestDetailAux).State = EntityState.Modified;

                                var salesRequestDetailBusinessOportunity = detail.SalesRequestDetailBusinessOportunity.FirstOrDefault();
                                salesRequestDetailBusinessOportunity.quantity = detail.quantity;
                                db.SalesRequestDetailBusinessOportunity.Attach(salesRequestDetailBusinessOportunity);
                                db.Entry(salesRequestDetailBusinessOportunity).State = EntityState.Modified;

                                #endregion

                                #region TOTALS

                                salesRequestAux.SalesRequestTotal = SalesRequestTotals(salesRequestAux.id, salesRequestAux.SalesRequestDetail.ToList(), db);

                                db.SalesRequest.Attach(salesRequestAux);
                                db.Entry(salesRequestAux).State = EntityState.Modified;

                                #endregion

                            }
                            else
                            {
                                salesRequestAux = CreateSalesRequestFromBusinessOportunityPlanningDetail(db, activeUser, activeCompany, activeEmissionPoint, detail);
                                detail.id_document = salesRequestAux.Document.id;
                                detail.Document = salesRequestAux.Document;
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

        public static void UpdateQuantityRecivedSalesRequestDetail(DBContext db, int? id_salesRequestDetail, decimal quantity)
        {
            string result = "";
            //Lot lot = new Lot();
            try
            {
                if (id_salesRequestDetail != null)
                {
                    var salesRequestDetail = db.SalesRequestDetail.FirstOrDefault(fod => fod.id == id_salesRequestDetail);
                    salesRequestDetail.quantityOutstandingOrder -= quantity;
                    db.SalesRequestDetail.Attach(salesRequestDetail);
                    db.Entry(salesRequestDetail).State = EntityState.Modified;
                }
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            //return lot;
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

        private static SalesRequestTotal SalesRequestTotals(int id_salesRequest, List<SalesRequestDetail> salesRequestDetails, DBContext db)
        {
            SalesRequestTotal salesRequestTotal = db.SalesRequestTotal.FirstOrDefault(t => t.id_salesRequest == id_salesRequest);

            salesRequestTotal = salesRequestTotal ?? new SalesRequestTotal
            {
                id_salesRequest = id_salesRequest
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

            foreach (var detail in salesRequestDetails)
            {
                if (detail.Item != null && detail.isActive)
                {
                    ItemTaxation rateIVA12 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("2"));//"2": IVA "2": IVA 12%
                    ItemTaxation rateIVA14 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("5"));//"2": IVA "5": IVA 14%

                    if (rateIVA12 != null)
                    {
                        subtotalIVA12Percent += detail.quantityApproved * detail.price;
                    }

                    if (rateIVA14 != null)
                    {
                        subtotalIVA14Percent += detail.quantityApproved * detail.price;
                    }
                     
                    subtotal += detail.quantityApproved * detail.price;
                }

            }

            totalIVA12 = subtotalIVA12Percent * 0.12M;
            totalIVA14 = subtotalIVA14Percent * 0.14M;

            total = subtotal + totalIVA12 + totalIVA14 + valueICE + valueIRBPNR - discount;

            salesRequestTotal.subtotalIVA12Percent = subtotalIVA12Percent;
            salesRequestTotal.subtotalIVA14Percent = subtotalIVA14Percent;
            salesRequestTotal.subtotalIVA0Percent = subtotalIVA0Percent;
            salesRequestTotal.subtotalIVANoObjectIVA = subtotalIVANoObjectIVA;
            salesRequestTotal.subtotalExentedIVA = subtotalExentedIVA;

            salesRequestTotal.subtotal = subtotal;
            salesRequestTotal.discount = discount;
            salesRequestTotal.valueICE = valueICE;
            salesRequestTotal.valueIRBPNR = valueIRBPNR;

            salesRequestTotal.totalIVA12 = totalIVA12;
            salesRequestTotal.totalIVA14 = totalIVA14;

            salesRequestTotal.total = total;

            return salesRequestTotal;
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