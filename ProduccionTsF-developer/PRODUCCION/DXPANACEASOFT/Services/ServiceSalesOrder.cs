using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;

namespace DXPANACEASOFT.Services
{
    public class ServiceSalesOrder
    {
        public static SalesOrder CreateSalesOrderFromProductionScheduleProductionOrderDetail(DBContext db, User activeUser, Company activeCompany, EmissionPoint activeEmissionPoint, ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail)
        {
            string result = "";
            SalesOrder salesOrder = new SalesOrder();
            var salesRequestDetail = productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail?.FirstOrDefault()?.SalesRequestDetail;
            try
            {
                #region Document

                Document document = new Document();
                document.id_userCreate = activeUser.id;
                document.dateCreate = DateTime.Now;
                document.id_userUpdate = activeUser.id;
                document.dateUpdate = DateTime.Now;

                DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == "11");//Orden de Producción
                if (documentType == null)
                {
                    throw new Exception("No se puedo Crear el Documento Orden de Producción porque no existe el Tipo de Documento: Orden de Producción con Código: 11, configúrelo e inténtelo de nuevo");
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

                #region SalesOrder


                salesOrder.id = document.id;
                salesOrder.Document = document;

                var id_customerAux = salesRequestDetail?.SalesRequest.id_customer;
                salesOrder.id_customer = id_customerAux;
                salesOrder.Customer = db.Customer.FirstOrDefault(fod=> fod.id == id_customerAux);
                var id_employeeSellerAux = salesRequestDetail?.SalesRequest.id_employeeSeller;
                salesOrder.id_employeeSeller = id_employeeSellerAux;
                salesOrder.Employee = db.Employee.FirstOrDefault(fod=> fod.id == id_employeeSellerAux);
                var id_priceListAux = salesRequestDetail?.SalesRequest.id_priceList;
                salesOrder.id_priceList = id_priceListAux;
                salesOrder.PriceList = db.PriceList.FirstOrDefault(fod=> fod.id == id_priceListAux);
                salesOrder.requiredLogistic = false;

                //lot.id_company = activeCompany.id;
                #endregion

                #region SalesOrderDetails

                //if (tempSalesOrder?.SalesOrderDetail != null)
                //{
                //    itemPO.SalesOrderDetail = new List<SalesOrderDetail>();
                //var itemPODetails = tempSalesOrder.SalesOrderDetail.ToList();

                //foreach (var detail in itemPODetails)
                //{
                decimal _price = salesRequestDetail?.price ?? 0;
                decimal _iva = SalesDetailIVA(productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.id_item,
                                            productionScheduleProductionOrderDetail.quantityRequest,
                                            _price, db);
                var tempItemPODetail = new SalesOrderDetail
                {
                    id_item = productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.id_item,
                    Item = db.Item.FirstOrDefault(i => i.id == productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.id_item),

                    quantityRequested = 0,
                    quantityOrdered = productionScheduleProductionOrderDetail.quantityRequest,
                    quantityApproved = productionScheduleProductionOrderDetail.quantityRequest,
                    quantityDelivered = 0,

                    price = _price,
                    iva = _iva,

                    subtotal = _price * productionScheduleProductionOrderDetail.quantityRequest,
                    total = (_price * productionScheduleProductionOrderDetail.quantityRequest) + _iva,

                    isActive = true,
                    id_userCreate = activeUser.id,
                    dateCreate = DateTime.Now,
                    id_userUpdate = activeUser.id,
                    dateUpdate = DateTime.Now,

                    ProductionScheduleProductionOrderDetailSalesOrderDetail = new List<ProductionScheduleProductionOrderDetailSalesOrderDetail>()
                    //SalesOrderDetailSalesRequest = new List<SalesOrderDetailSalesRequest>()
                };

                //foreach (var requestDetail in detail.SalesOrderDetailSalesRequest)
                //{
                   var productionScheduleProductionOrderDetailSalesOrderDetail= (new ProductionScheduleProductionOrderDetailSalesOrderDetail
                    {
                        id_productionScheduleProductionOrderDetail = productionScheduleProductionOrderDetail.id,
                        ProductionScheduleProductionOrderDetail = productionScheduleProductionOrderDetail,
                        SalesOrderDetail = tempItemPODetail,
                        quantity = productionScheduleProductionOrderDetail.quantityRequest
                    });

                tempItemPODetail.ProductionScheduleProductionOrderDetailSalesOrderDetail.Add(productionScheduleProductionOrderDetailSalesOrderDetail);
                productionScheduleProductionOrderDetail.ProductionScheduleProductionOrderDetailSalesOrderDetail.Add(productionScheduleProductionOrderDetailSalesOrderDetail);
                //}

                //if (tempItemPODetail.isActive)
                salesOrder.SalesOrderDetail.Add(tempItemPODetail);
                //}
                //}

                #endregion

                #region TOTALS

                salesOrder.SalesOrderTotal = SalesOrderTotals(salesOrder.id, salesOrder.SalesOrderDetail.ToList(), db);

                #endregion

                db.SalesOrder.Add(salesOrder);
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            return salesOrder;
        }

        public static void UpdateSalesOrderDetail(User ActiveUser, Company ActiveCompany, EmissionPoint ActiveEmissionPoint, List<ProductionScheduleProductionOrderDetail> productionScheduleProductionOrderDetail, DBContext db, bool reverse)
        {
            string result = "";
            SalesOrder salesOrder = new SalesOrder();
            try
            {
                if (reverse)
                {
                    for (int i = productionScheduleProductionOrderDetail.Count - 1; i >= 0; i--)
                    {
                        var detail = productionScheduleProductionOrderDetail.ElementAt(i);

                        var salesOrderDetailAux = detail.ProductionScheduleProductionOrderDetailSalesOrderDetail?.FirstOrDefault(fod => fod.SalesOrderDetail.SalesOrder.Document.DocumentState.code.Equals("03") ||//APROBADA
                                                                                                                                        fod.SalesOrderDetail.SalesOrder.Document.DocumentState.code.Equals("06"));//AUTORIZADA

                        if (salesOrderDetailAux != null)
                        {
                            throw new Exception("No puede Reversarse la Programación de Producción debido a tener asociada Orden de Producción(Pedido) Aprobado o Autorizado. Revéreselo e inténtelo de nuevo.");
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
                    foreach (var detail in productionScheduleProductionOrderDetail)
                    {
                        var salesOrderDetailAux = detail.ProductionScheduleProductionOrderDetailSalesOrderDetail.FirstOrDefault()?.SalesOrderDetail;
                        if (salesOrderDetailAux != null)
                        {
                            salesOrderDetailAux.quantityOrdered = detail.quantityRequest;
                            db.SalesOrderDetail.Attach(salesOrderDetailAux);
                            db.Entry(salesOrderDetailAux).State = EntityState.Modified;

                            var productionScheduleProductionOrderDetailSalesOrderDetail = detail.ProductionScheduleProductionOrderDetailSalesOrderDetail.FirstOrDefault();
                            productionScheduleProductionOrderDetailSalesOrderDetail.quantity = detail.quantityRequest;
                            db.ProductionScheduleProductionOrderDetailSalesOrderDetail.Attach(productionScheduleProductionOrderDetailSalesOrderDetail);
                            db.Entry(productionScheduleProductionOrderDetailSalesOrderDetail).State = EntityState.Modified;
                        }
                        else
                        {
                            CreateSalesOrderFromProductionScheduleProductionOrderDetail(db, ActiveUser, ActiveCompany, ActiveEmissionPoint, detail);
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

        public static void UpdateQuantityOutstandingOrderSalesRequestDetail(DBContext db, int? id_salesRequestDetail, decimal quantity)
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

                //var purchaseOrderDetail = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == id_purchaseOrderDetail);
                //purchaseOrderDetail.quantityReceived += quantity;
                //db.PurchaseOrderDetail.Attach(purchaseOrderDetail);
                //db.Entry(purchaseOrderDetail).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            //return lot;
        }

        public static void UpdateQuantityDeliverySalesOrderDetail(DBContext db, MasteredDTO paramMasteredDTO, bool reversar)
        {
            string result = "";
            //Lot lot = new Lot();
            try
            {
                var listSalesOrder = new List<SalesOrder>();
                foreach (var item in paramMasteredDTO.MasteredDetails)
                {
                    if (item.id_sales != null) {
                        var salesOrder = db.SalesOrder.FirstOrDefault(fod => fod.id == item.id_sales);
                        listSalesOrder.Add(salesOrder);
                        var aSalesOrderDetail = salesOrder.SalesOrderDetail.FirstOrDefault(fod => fod.id_item == item.id_productPT);
                        aSalesOrderDetail.quantityDelivered += item.quantityPT*(reversar?-1:1);
                        if (aSalesOrderDetail.quantityDelivered < 0) aSalesOrderDetail.quantityDelivered = 0;
                        db.SalesOrderDetail.Attach(aSalesOrderDetail);
                        db.Entry(aSalesOrderDetail).State = EntityState.Modified;
                    }
                }

                foreach (var item in listSalesOrder)
                {
                    if (item.SalesOrderDetail.FirstOrDefault(fod => fod.quantityApproved > fod.quantityDelivered) == null) {
                        var closeState = db.DocumentState.FirstOrDefault(d => d.code.Equals("04"));//04 : CERRADA

                        item.Document.id_documentState = closeState.id;
                        item.Document.DocumentState = closeState;
                        db.SalesOrder.Attach(item);
                        db.Entry(item).State = EntityState.Modified;
                    }
                }
                
            }
            catch (Exception e)
            {
                result = e.Message;
                throw;
            }

            //return lot;
        }

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