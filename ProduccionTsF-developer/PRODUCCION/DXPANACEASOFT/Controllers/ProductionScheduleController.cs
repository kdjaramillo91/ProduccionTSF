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
using DXPANACEASOFT.Services;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ProductionScheduleController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region Production Schedule EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionScheduleFormEditPartial(int id, int[] ids_salesRequestDetail)
        {
            ProductionSchedule productionSchedule = db.ProductionSchedule.FirstOrDefault(r => r.id == id);

            if (productionSchedule == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("56"));//"56": Programación de la Producción
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

                Employee employee = ActiveUser.Employee;

                productionSchedule = new ProductionSchedule
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
                    id_personSchedule = employee?.id ?? 0,
                    Employee = employee,
                    ProductionScheduleRequestDetail = new List<ProductionScheduleRequestDetail>(),
                    ProductionSchedulePurchaseRequestDetail = new List<ProductionSchedulePurchaseRequestDetail>(),
                    ProductionScheduleProductionOrderDetail = new List<ProductionScheduleProductionOrderDetail>(),
                    ProductionScheduleInventoryReservationDetail = new List<ProductionScheduleInventoryReservationDetail>(),
                    ProductionScheduleScheduleDetail = new List<ProductionScheduleScheduleDetail>()
                };

                productionSchedule.Document.ProductionSchedule = productionSchedule;
                if (ids_salesRequestDetail != null)
                {
                    
                    //List<ProductionScheduleRequestDetail> productionScheduleRequestDetails = new List<ProductionScheduleRequestDetail>();

                    foreach (var i in ids_salesRequestDetail)
                    {
                        SalesRequestDetail salesRequestDetails = db.SalesRequestDetail.FirstOrDefault(d => d.id == i);

                        ProductionScheduleRequestDetail productionScheduleRequestDetail = new ProductionScheduleRequestDetail
                        {
                            id = productionSchedule.ProductionScheduleRequestDetail.Count() > 0 ? productionSchedule.ProductionScheduleRequestDetail.Max(rpsd => rpsd.id) + 1 : 1,
                            id_item = salesRequestDetails.id_item,
                            Item = db.Item.FirstOrDefault(fod=> fod.id == salesRequestDetails.id_item),
                            quantityRequest = salesRequestDetails.quantityApproved,
                            quantitySchedule = salesRequestDetails.quantityApproved,
                            reservedInInventory = true,
                            id_metricUnit = salesRequestDetails.id_metricUnitTypeUMPresentation,
                            MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == salesRequestDetails.id_metricUnitTypeUMPresentation),
                            quantitySale = salesRequestDetails.quantityTypeUMSale,
                            SalesRequestOrQuotationDetailProductionScheduleRequestDetail = new List<SalesRequestOrQuotationDetailProductionScheduleRequestDetail>(),
                            ProductionScheduleInventoryReservationDetail = new List<ProductionScheduleInventoryReservationDetail>(),
                            ProductionScheduleProductionOrderDetail = new List<ProductionScheduleProductionOrderDetail>()
                        };

                        productionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Add(new SalesRequestOrQuotationDetailProductionScheduleRequestDetail
                        {
                            id_salesRequestDetail = salesRequestDetails.id,
                            SalesRequestDetail = salesRequestDetails,
                            id_salesRequest = salesRequestDetails.id_salesRequest,
                            SalesRequest = db.SalesRequest.FirstOrDefault(fod => fod.id == salesRequestDetails.id_salesRequest),
                            id_metricUnitRequestDetail = salesRequestDetails.id_metricUnitTypeUMPresentation,
                            MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == salesRequestDetails.id_metricUnitTypeUMPresentation),
                            id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                            ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                            quantity = salesRequestDetails.quantityApproved
                        });

                        
                        

                        var inventoryMoveDetailBalancesLasts = db.InventoryMoveDetail.Where(w=> w.InventoryMove.Document.DocumentState.code.Equals("03") &&
                                                                                                w.id_inventoryMoveDetailNext == null &&
                                                                                                w.balance > 0 &&
                                                                                                w.id_item == salesRequestDetails.id_item &&
                                                                                                w.Warehouse.WarehouseType.code != ("RES01")).OrderBy(ob=> ob.id_lot).ThenByDescending(tbd=> tbd.inMaximumUnit).ToList();
                        var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
                        var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
                        var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

                        int? id_metricUnitPresentation;
                        MetricUnitConversion metricUnitConversion = null;
                        decimal factor;

                        var quantityAux = salesRequestDetails.quantityApproved;

                        //id_metricUnitPresentation = inventoryMoveDetailBalancesLast.Item.Presentation?.id_metricUnit ?? metricUnitUMTP.id;
                        metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                                 muc.id_metricOrigin == productionScheduleRequestDetail.id_metricUnit &&
                                                                                                 muc.id_metricDestiny == metricUnitUMTP.id);

                        if (metricUnitUMTP.id != productionScheduleRequestDetail.id_metricUnit && metricUnitConversion == null)
                            throw new Exception("No existe factor de conversión entre " + (productionScheduleRequestDetail.MetricUnit.code ?? metricUnitUMTP.code) + " y " + metricUnitUMTP.code + " necesario para reservar producto en Invetario, configúrelo e intentelo de nuevo.");

                        factor = (metricUnitUMTP.id == productionScheduleRequestDetail.id_metricUnit) ? 1 : (metricUnitConversion.factor);

                        quantityAux *= factor;

                        var quantityReservated = productionSchedule.ProductionScheduleInventoryReservationDetail.Where(w => w.ProductionScheduleRequestDetail.id_item == salesRequestDetails.id_item).Sum(s => s.quantity);

                        decimal balacesAux = -quantityReservated;

                        foreach (var inventoryMoveDetailBalancesLast in inventoryMoveDetailBalancesLasts)
                        {
                            id_metricUnitPresentation = inventoryMoveDetailBalancesLast.Item.Presentation?.id_metricUnit ?? metricUnitUMTP.id;
                            metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                                     muc.id_metricOrigin == id_metricUnitPresentation &&
                                                                                                     muc.id_metricDestiny == metricUnitUMTP.id);

                            if (metricUnitUMTP.id != id_metricUnitPresentation && metricUnitConversion == null)
                                throw new Exception("No existe factor de conversión entre " + (inventoryMoveDetailBalancesLast.Item.Presentation?.MetricUnit.code ?? metricUnitUMTP.code) + " y " + metricUnitUMTP.code + " necesario para reservar producto en Invetario, configúrelo e intentelo de nuevo.");

                            factor = (metricUnitUMTP.id == id_metricUnitPresentation) ? 1 : (metricUnitConversion.factor);

                            if (inventoryMoveDetailBalancesLast.inMaximumUnit)
                            {
                                balacesAux += (inventoryMoveDetailBalancesLast.balance *
                                              (inventoryMoveDetailBalancesLast.Item.Presentation?.maximum ?? 1) *
                                              (inventoryMoveDetailBalancesLast.Item.Presentation?.minimum ?? 1) *
                                              factor);
                            }else
                            {
                                balacesAux += (inventoryMoveDetailBalancesLast.balance *
                                              (inventoryMoveDetailBalancesLast.Item.Presentation?.minimum ?? 1) *
                                              factor);
                            }

                            if(balacesAux >= quantityAux)
                            {
                                //ProductionScheduleInventoryReservationDetail productionScheduleInventoryReservationDetail = new ProductionScheduleInventoryReservationDetail
                                //{
                                //    id = productionSchedule.ProductionScheduleInventoryReservationDetail.Count() > 0 ? productionSchedule.ProductionScheduleInventoryReservationDetail.Max(rpsd => rpsd.id) + 1 : 1,
                                //    id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                                //    ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                                //    quantity = quantityAux
                                //};
                                //productionScheduleRequestDetail.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                //productionSchedule.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                break;
                            }
                        }

                        id_metricUnitPresentation = salesRequestDetails.Item.Presentation?.id_metricUnit ?? metricUnitUMTP.id;
                        metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                                 muc.id_metricOrigin == metricUnitUMTP.id &&
                                                                                                 muc.id_metricDestiny == id_metricUnitPresentation);

                        if (metricUnitUMTP.id != id_metricUnitPresentation && metricUnitConversion == null)
                            throw new Exception("No existe factor de conversión entre " + metricUnitUMTP.code + " y " + (salesRequestDetails.Item.Presentation?.MetricUnit.code ?? metricUnitUMTP.code) + " necesario para generar los producto en la Orden de Producción, configúrelo e intentelo de nuevo.");

                        factor = (metricUnitUMTP.id == id_metricUnitPresentation) ? 1 : (metricUnitConversion.factor);

                        if (balacesAux == 0)
                        {
                            ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail = new ProductionScheduleProductionOrderDetail
                            {
                                id = productionSchedule.ProductionScheduleProductionOrderDetail.Count() > 0 ? productionSchedule.ProductionScheduleProductionOrderDetail.Max(rpsd => rpsd.id) + 1 : 1,
                                id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                                ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                                quantityRequest = quantityAux
                            };

                            productionScheduleProductionOrderDetail.quantityProductionOrder = (quantityAux * factor) /
                                                                                              (salesRequestDetails.Item.Presentation?.minimum ?? 1);

                            var truncateQuantityProductionOrder = decimal.Truncate(productionScheduleProductionOrderDetail.quantityProductionOrder);
                            if ((productionScheduleProductionOrderDetail.quantityProductionOrder - truncateQuantityProductionOrder) > 0)
                            {
                                productionScheduleProductionOrderDetail.quantityProductionOrder = truncateQuantityProductionOrder + 1;
                            };

                            productionScheduleRequestDetail.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                            productionSchedule.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);

                            UpdateProductionSchedulePurchaseRequestDetail(productionSchedule, productionScheduleProductionOrderDetail);
                        }
                        else
                        {
                            if (balacesAux >= quantityAux)
                            {
                                ProductionScheduleInventoryReservationDetail productionScheduleInventoryReservationDetail = new ProductionScheduleInventoryReservationDetail
                                {
                                    id = productionSchedule.ProductionScheduleInventoryReservationDetail.Count() > 0 ? productionSchedule.ProductionScheduleInventoryReservationDetail.Max(rpsd => rpsd.id) + 1 : 1,
                                    id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                                    ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                                    quantity = quantityAux
                                };
                                productionScheduleRequestDetail.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                productionSchedule.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                            }else
                            {
                                ProductionScheduleInventoryReservationDetail productionScheduleInventoryReservationDetail = new ProductionScheduleInventoryReservationDetail
                                {
                                    id = productionSchedule.ProductionScheduleInventoryReservationDetail.Count() > 0 ? productionSchedule.ProductionScheduleInventoryReservationDetail.Max(rpsd => rpsd.id) + 1 : 1,
                                    id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                                    ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                                    quantity = balacesAux
                                };
                                productionScheduleRequestDetail.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                productionSchedule.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);

                                ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail = new ProductionScheduleProductionOrderDetail
                                {
                                    id = productionSchedule.ProductionScheduleProductionOrderDetail.Count() > 0 ? productionSchedule.ProductionScheduleProductionOrderDetail.Max(rpsd => rpsd.id) + 1 : 1,
                                    id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                                    ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                                    quantityRequest = (quantityAux - balacesAux)
                                };

                                productionScheduleProductionOrderDetail.quantityProductionOrder = (productionScheduleProductionOrderDetail.quantityRequest * factor) /
                                                                                                  (salesRequestDetails.Item.Presentation?.minimum ?? 1);

                                var truncateQuantityProductionOrder = decimal.Truncate(productionScheduleProductionOrderDetail.quantityProductionOrder);
                                if ((productionScheduleProductionOrderDetail.quantityProductionOrder - truncateQuantityProductionOrder) > 0)
                                {
                                    productionScheduleProductionOrderDetail.quantityProductionOrder = truncateQuantityProductionOrder + 1;
                                };

                                productionScheduleRequestDetail.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                                productionSchedule.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);

                                UpdateProductionSchedulePurchaseRequestDetail(productionSchedule, productionScheduleProductionOrderDetail);
                            }
                        }

                        //ReservationProductionScheduleDetailRequestProductionScheduleDetail
                        //if (pendingPurchaseOrdersAndRemissionGuides != null)


                        productionSchedule.ProductionScheduleRequestDetail.Add(productionScheduleRequestDetail);
                    }

                }

            }


            TempData["productionSchedule"] = productionSchedule;
            TempData.Keep("productionSchedule");

            return PartialView("_FormEditProductionSchedule", productionSchedule);
        }


        #endregion

        #region ResultGridView

        [ValidateInput(false)]
        public ActionResult ProductionScheduleResultsPartial(int? id_documentState, string number/*, string reference*/, DateTime? startEmissionDate, DateTime? endEmissionDate,//Document
                                                                 int? id_productionSchedulePeriod, int? filterPersonSchedule,//Planning
                                                                 int[] providers, int[] buyers, int[] items/*, int[] itemTypeCategorys*/, DateTime? startDatePlanning, DateTime? endDatePlanning) //Planning Detail
        {
            List<ProductionSchedule> model = db.ProductionSchedule.ToList();

            #region Document

            if (id_documentState != 0 && id_documentState != null)
            {
                model = model.Where(o => o.Document.id_documentState == id_documentState).ToList();
            }

            if (!string.IsNullOrEmpty(number))
            {
                model = model.Where(o => o.Document.number.Contains(number)).ToList();
            }

            //if (!string.IsNullOrEmpty(reference))
            //{
            //    model = model.Where(o => o.Document.reference.Contains(reference)).ToList();
            //}

            if (startEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
            }

            if (endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            #endregion

            #region Planning

            if (id_productionSchedulePeriod != 0 && id_productionSchedulePeriod != null)
            {
                model = model.Where(o => o.id_productionSchedulePeriod == id_productionSchedulePeriod).ToList();
            }

            if (filterPersonSchedule != 0 && filterPersonSchedule != null)
            {
                model = model.Where(o => o.id_personSchedule == filterPersonSchedule).ToList();
            }

            #endregion

            #region Planning Detail

            if (providers != null && providers.Length > 0)
            {
                var tempModel = new List<ProductionSchedule>();
                foreach (var m in model)
                {
                    var details = m.ProductionScheduleScheduleDetail.Where(d => providers.Contains(d.id_provider ?? 0));
                    if (details.Any())
                    {
                        tempModel.Add(m);
                    }
                }

                model = tempModel;
            }

            if (buyers != null && buyers.Length > 0)
            {
                var tempModel = new List<ProductionSchedule>();
                foreach (var m in model)
                {
                    var details = m.ProductionScheduleScheduleDetail.Where(d => buyers.Contains(d.id_buyer ?? 0));
                    if (details.Any())
                    {
                        tempModel.Add(m);
                    }
                }

                model = tempModel;
            }

            if (items != null && items.Length > 0)
            {
                var tempModel = new List<ProductionSchedule>();
                foreach (var m in model)
                {
                    var details = m.ProductionScheduleScheduleDetail.Where(d => items.Contains(d.id_item));
                    if (details.Any())
                    {
                        tempModel.Add(m);
                    }
                }

                model = tempModel;
            }

            //if (itemTypeCategorys != null && itemTypeCategorys.Length > 0)
            //{
            //    var tempModel = new List<PurchasePlanning>();
            //    foreach (var m in model)
            //    {
            //        var details = m.PurchasePlanningDetail.Where(d => itemTypeCategorys.Contains(d.id_itemTypeCategory));
            //        if (details.Any())
            //        {
            //            tempModel.Add(m);
            //        }
            //    }

            //    model = tempModel;
            //}

            if (startDatePlanning != null)
            {
                var tempModel = new List<ProductionSchedule>();
                foreach (var m in model)
                {
                    var details = m.ProductionScheduleScheduleDetail.Where(d => DateTime.Compare(startDatePlanning.Value.Date, d.datePlanning.Date) <= 0);
                    if (details.Any())
                    {
                        tempModel.Add(m);
                    }
                }

                model = tempModel;
            }

            if (endDatePlanning != null)
            {
                var tempModel = new List<ProductionSchedule>();
                foreach (var m in model)
                {
                    var details = m.ProductionScheduleScheduleDetail.Where(d => DateTime.Compare(d.datePlanning.Date, endDatePlanning.Value.Date) <= 0);
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

            return PartialView("_ProductionScheduleResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesRequestDetailsResults()
        {
            var model = db.SalesRequestDetail.Where(d => d.isActive && d.SalesRequest.Document.DocumentState.code == "06" && d.quantityOutstandingOrder > 0)//"06" AUTORIZADA
                          .OrderByDescending(d => d.id_salesRequest).ThenByDescending(d => d.id_item);
            return PartialView("_SalesRequestDetailsResultsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SalesRequestDetailsPartial()
        {
            var model = db.SalesRequestDetail.Where(d => d.isActive && d.SalesRequest.Document.DocumentState.code == "06" && d.quantityOutstandingOrder > 0)//"06" AUTORIZADA
                          .OrderByDescending(d => d.id_salesRequest).ThenByDescending(d => d.id_item);
            return PartialView("_SalesRequestDetailsPartial", model.ToList());
        }

        #endregion

        #region Production Schedules

        [HttpPost]
        public ActionResult ProductionSchedulesPartial()
        {
            var model = (TempData["model"] as List<ProductionSchedule>);
            model = model ?? new List<ProductionSchedule>();

            TempData.Keep("model");
            return PartialView("_ProductionSchedulesPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionSchedulesPartialAddNew(bool approve, ProductionSchedule item, Document itemDoc)
        {
            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);
            productionSchedule = productionSchedule ?? new ProductionSchedule();

            var productionScheduleError = productionSchedule;
            productionScheduleError.Document = itemDoc;
            productionScheduleError.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.id == itemDoc.id_documentState);
            DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == "56");//56: Programación de la Producción
            productionScheduleError.Document.DocumentType = documentType;
            productionScheduleError.Document.id_documentType = documentType?.id ?? 0;
            productionScheduleError.id_personSchedule = item.id_personSchedule;
            productionScheduleError.Employee = db.Employee.FirstOrDefault(fod=> fod.id == item.id_personSchedule);
            productionScheduleError.id_productionSchedulePeriod = item.id_productionSchedulePeriod;
            productionScheduleError.ProductionSchedulePeriod = db.ProductionSchedulePeriod.FirstOrDefault(fod=> fod.id == item.id_productionSchedulePeriod);

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

                        
                        if (documentType == null)
                        {
                            TempData["productionSchedule"] = productionScheduleError;
                            TempData.Keep("productionSchedule");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la programación porque no existe el Tipo de Documento: Programación de la Producción con Código: 56, configúrelo e inténtelo de nuevo");
                            return PartialView("_ProductionScheduleEditFormPartial", productionScheduleError);

                        }
                        item.Document.id_documentType = documentType.id;
                        item.Document.DocumentType = documentType;
                        item.Document.sequential = GetDocumentSequential(item.Document.id_documentType);
                        item.Document.number = GetDocumentNumber(item.Document.id_documentType);


                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == itemDoc.id_documentState);
                        //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        if (documentState == null)
                        {
                            TempData["productionSchedule"] = productionScheduleError;
                            TempData.Keep("productionSchedule");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la programación porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                            return PartialView("_ProductionScheduleEditFormPartial", productionScheduleError);

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

                        #region ProductionSchedule

                        item.id_company = this.ActiveCompanyId;

                        #endregion

                        #region ProductionScheduleRequestDetail

                        if (productionSchedule.ProductionScheduleRequestDetail != null)
                        {
                            item.ProductionScheduleRequestDetail = new List<ProductionScheduleRequestDetail>();
                            foreach (var detail in productionSchedule.ProductionScheduleRequestDetail)
                            {
                                var productionScheduleRequestDetail = new ProductionScheduleRequestDetail();
                                productionScheduleRequestDetail.id = detail.id;
                                productionScheduleRequestDetail.id_item = detail.id_item;
                                productionScheduleRequestDetail.Item = db.Item.FirstOrDefault(i => i.id == productionScheduleRequestDetail.id_item);
                                productionScheduleRequestDetail.quantityRequest = detail.quantityRequest;
                                productionScheduleRequestDetail.quantitySchedule = detail.quantitySchedule;
                                productionScheduleRequestDetail.id_metricUnit = detail.id_metricUnit;
                                productionScheduleRequestDetail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == productionScheduleRequestDetail.id_metricUnit);
                                productionScheduleRequestDetail.quantitySale = detail.quantitySale;

                                productionScheduleRequestDetail.reservedInInventory = detail.reservedInInventory;

                                productionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail = new List<SalesRequestOrQuotationDetailProductionScheduleRequestDetail>();
                                productionScheduleRequestDetail.ProductionScheduleInventoryReservationDetail = new List<ProductionScheduleInventoryReservationDetail>();

                                foreach (var requestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                                {
                                    //ProductionScheduleRequestDetail
                                    //var inventoryMoveDetailAux = db.InventoryMoveDetail.FirstOrDefault(fod => fod.id == detailInventoryMoveDetailTransfer.id_inventoryMoveDetailExit);
                                    //var id_metricUnitMovExitAux = inventoryMoveDetailAux.id_metricUnitMove;

                                    decimal amountSaleAux = 0;
                                    var factorConversion = (productionScheduleRequestDetail.id_metricUnit != requestDetail.id_metricUnitRequestDetail) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == productionScheduleRequestDetail.id_metricUnit &&
                                                                                                                                                                                                       fod.id_metricDestiny == requestDetail.id_metricUnitRequestDetail)?.factor ?? 0 : 1;
                                    if (factorConversion == 0)
                                    {
                                        throw new Exception("Falta el Factor de Conversión entre : " + productionScheduleRequestDetail.MetricUnit.code + " y " + requestDetail.MetricUnit.code + ".Necesario para la cantidad recibida del requerimiento de pedido Origen Configúrelo, e intente de nuevo");
                                    }
                                    else
                                    {
                                        amountSaleAux = productionScheduleRequestDetail.quantitySchedule * factorConversion;
                                    }

                                    productionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Add(new SalesRequestOrQuotationDetailProductionScheduleRequestDetail
                                    {
                                        id_salesRequestDetail = requestDetail.id_salesRequestDetail,
                                        SalesRequestDetail = db.SalesRequestDetail.FirstOrDefault(i => i.id == requestDetail.id_salesRequestDetail),
                                        id_salesRequest = requestDetail.id_salesRequest,
                                        SalesRequest = db.SalesRequest.FirstOrDefault(i => i.id == requestDetail.id_salesRequest),
                                        id_metricUnitRequestDetail = requestDetail.id_metricUnitRequestDetail,
                                        MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == requestDetail.id_metricUnitRequestDetail),
                                        quantity = amountSaleAux
                                    });
                                    if (approve)
                                    {
                                        ServiceSalesOrder.UpdateQuantityOutstandingOrderSalesRequestDetail(db, requestDetail.id_salesRequestDetail, amountSaleAux);
                                    }
                                }

                                item.ProductionScheduleRequestDetail.Add(productionScheduleRequestDetail);
                            }
                        }

                        if (item.ProductionScheduleRequestDetail.Count == 0)
                        {
                            TempData["productionSchedule"] = productionScheduleError;
                            TempData.Keep("productionSchedule");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una programación sin detalles de requerimientos");
                            return PartialView("_ProductionScheduleEditFormPartial", productionScheduleError);
                        }

                        #endregion

                        #region ProductionScheduleInventoryReservationDetail

                        if (productionSchedule.ProductionScheduleInventoryReservationDetail != null)
                        {
                            item.ProductionScheduleInventoryReservationDetail = new List<ProductionScheduleInventoryReservationDetail>();
                            foreach (var detail in productionSchedule.ProductionScheduleInventoryReservationDetail)
                            {
                                var productionScheduleInventoryReservationDetail = new ProductionScheduleInventoryReservationDetail();
                                productionScheduleInventoryReservationDetail.id = detail.id;
                                productionScheduleInventoryReservationDetail.quantity = detail.quantity;

                                //productionScheduleInventoryReservationDetail.InventoryReservation = new List<InventoryReservation>();

                                //if (approve)
                                //{
                                //    //productionScheduleInventoryReservationDetail.InventoryReservation = Services.ServiceInventoryMove.UpdateInventaryMoveTransferReservation();
                                //}

                                var productionScheduleRequestDetailAux = item.ProductionScheduleRequestDetail.FirstOrDefault(fod=> fod.id == detail.id_productionScheduleRequestDetail);
                                productionScheduleRequestDetailAux.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                productionScheduleInventoryReservationDetail.ProductionScheduleRequestDetail = productionScheduleRequestDetailAux;

                                item.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                productionScheduleInventoryReservationDetail.ProductionSchedule = item;

                            }
                        }

                        #endregion

                        #region ProductionScheduleProductionOrderDetail

                        if (productionSchedule.ProductionScheduleProductionOrderDetail != null)
                        {
                            item.ProductionScheduleProductionOrderDetail = new List<ProductionScheduleProductionOrderDetail>();
                            foreach (var detail in productionSchedule.ProductionScheduleProductionOrderDetail)
                            {
                                var productionScheduleProductionOrderDetail = new ProductionScheduleProductionOrderDetail();
                                productionScheduleProductionOrderDetail.id = detail.id;
                                productionScheduleProductionOrderDetail.quantityRequest = detail.quantityRequest;
                                productionScheduleProductionOrderDetail.quantityProductionOrder = detail.quantityProductionOrder;

                                
                                //productionScheduleInventoryReservationDetail.InventoryReservation = new List<InventoryReservation>();

                                //if (approve)
                                //{
                                //    //productionScheduleInventoryReservationDetail.InventoryReservation = Services.ServiceInventoryMove.UpdateInventaryMoveTransferReservation();
                                //}

                                var productionScheduleRequestDetailAux = item.ProductionScheduleRequestDetail.FirstOrDefault(fod => fod.id == detail.id_productionScheduleRequestDetail);
                                productionScheduleRequestDetailAux.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                                productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail = productionScheduleRequestDetailAux;

                                item.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                                productionScheduleProductionOrderDetail.ProductionSchedule = item;
                            }
                        }

                        #endregion

                        #region ProductionSchedulePurchaseRequestDetail

                        if (productionSchedule.ProductionSchedulePurchaseRequestDetail != null)
                        {
                            item.ProductionSchedulePurchaseRequestDetail = new List<ProductionSchedulePurchaseRequestDetail>();
                            foreach (var detail in productionSchedule.ProductionSchedulePurchaseRequestDetail)
                            {
                                var productionSchedulePurchaseRequestDetail = new ProductionSchedulePurchaseRequestDetail();
                                productionSchedulePurchaseRequestDetail.id = detail.id;
                                productionSchedulePurchaseRequestDetail.id_item = detail.id_item;
                                productionSchedulePurchaseRequestDetail.Item = db.Item.FirstOrDefault(fod=> fod.id == detail.id_item);
                                productionSchedulePurchaseRequestDetail.quantity = detail.quantity;

                                //productionScheduleInventoryReservationDetail.InventoryReservation = new List<InventoryReservation>();

                                //if (approve)
                                //{
                                //    //productionScheduleInventoryReservationDetail.InventoryReservation = Services.ServiceInventoryMove.UpdateInventaryMoveTransferReservation();
                                //}

                                var productionScheduleProductionOrderDetailAux = item.ProductionScheduleProductionOrderDetail.FirstOrDefault(fod => fod.id == detail.id_productionScheduleProductionOrderDetail);
                                productionScheduleProductionOrderDetailAux.ProductionSchedulePurchaseRequestDetail.Add(productionSchedulePurchaseRequestDetail);
                                productionSchedulePurchaseRequestDetail.ProductionScheduleProductionOrderDetail = productionScheduleProductionOrderDetailAux;

                                item.ProductionSchedulePurchaseRequestDetail.Add(productionSchedulePurchaseRequestDetail);
                                productionSchedulePurchaseRequestDetail.ProductionSchedule = item;

                            }
                        }

                        #endregion

                        #region ProductionScheduleScheduleDetail

                        if (productionSchedule.ProductionScheduleScheduleDetail != null)
                        {
                            item.ProductionScheduleScheduleDetail = new List<ProductionScheduleScheduleDetail>();
                            foreach (var detail in productionSchedule.ProductionScheduleScheduleDetail)
                            {
                                var productionScheduleScheduleDetail = new ProductionScheduleScheduleDetail();
                                productionScheduleScheduleDetail.id = detail.id;
                                productionScheduleScheduleDetail.id_provider = detail.id_provider;
                                productionScheduleScheduleDetail.Provider = db.Provider.FirstOrDefault(i => i.id == detail.id_provider);
                                productionScheduleScheduleDetail.id_buyer = detail.id_buyer;
                                productionScheduleScheduleDetail.Person = db.Person.FirstOrDefault(i => i.id == detail.id_buyer);
                                productionScheduleScheduleDetail.id_item = detail.id_item;
                                productionScheduleScheduleDetail.Item = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                                productionScheduleScheduleDetail.quantity = detail.quantity;
                                productionScheduleScheduleDetail.datePlanning = detail.datePlanning;

                                //productionScheduleScheduleDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail = new List<SalesRequestOrQuotationDetailProductionScheduleRequestDetail>();
                                //productionScheduleScheduleDetail.ProductionScheduleInventoryReservationDetail = new List<ProductionScheduleInventoryReservationDetail>();

                                //foreach (var requestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                                //{
                                //    //ProductionScheduleRequestDetail
                                //    //var inventoryMoveDetailAux = db.InventoryMoveDetail.FirstOrDefault(fod => fod.id == detailInventoryMoveDetailTransfer.id_inventoryMoveDetailExit);
                                //    //var id_metricUnitMovExitAux = inventoryMoveDetailAux.id_metricUnitMove;

                                //    decimal amountSaleAux = 0;
                                //    var factorConversion = (productionScheduleRequestDetail.id_metricUnit != requestDetail.id_metricUnitRequestDetail) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == productionScheduleRequestDetail.id_metricUnit &&
                                //                                                                                                                                                                       fod.id_metricDestiny == requestDetail.id_metricUnitRequestDetail)?.factor ?? 0 : 1;
                                //    if (factorConversion == 0)
                                //    {
                                //        throw new Exception("Falta el Factor de Conversión entre : " + productionScheduleRequestDetail.MetricUnit.code + " y " + requestDetail.MetricUnit.code + ".Necesario para la cantidad recibida del requerimiento de pedido Origen Configúrelo, e intente de nuevo");
                                //    }
                                //    else
                                //    {
                                //        amountSaleAux = productionScheduleRequestDetail.quantitySchedule * factorConversion;
                                //    }

                                //    productionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Add(new SalesRequestOrQuotationDetailProductionScheduleRequestDetail
                                //    {
                                //        id_salesRequestDetail = requestDetail.id_salesRequestDetail,
                                //        SalesRequestDetail = db.SalesRequestDetail.FirstOrDefault(i => i.id == requestDetail.id_salesRequestDetail),
                                //        id_salesRequest = requestDetail.id_salesRequest,
                                //        SalesRequest = db.SalesRequest.FirstOrDefault(i => i.id == requestDetail.id_salesRequest),
                                //        id_metricUnitRequestDetail = requestDetail.id_metricUnitRequestDetail,
                                //        MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == requestDetail.id_metricUnitRequestDetail),
                                //        quantity = amountSaleAux
                                //    });
                                //}

                                var productionSchedulePurchaseRequestDetailAux = item.ProductionSchedulePurchaseRequestDetail.FirstOrDefault(fod => fod.id == detail.id_productionSchedulePurchaseRequestDetail);
                                productionSchedulePurchaseRequestDetailAux.ProductionScheduleScheduleDetail.Add(productionScheduleScheduleDetail);
                                productionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail = productionSchedulePurchaseRequestDetailAux;

                                item.ProductionScheduleScheduleDetail.Add(productionScheduleScheduleDetail);
                                productionScheduleScheduleDetail.ProductionSchedule = item;
                            }
                        }

                        if (item.ProductionScheduleScheduleDetail.Count == 0)
                        {
                            TempData["productionSchedule"] = productionScheduleError;
                            TempData.Keep("productionSchedule");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una programación sin detalles de planificación");
                            return PartialView("_ProductionScheduleEditFormPartial", productionScheduleError);
                        }

                        #endregion

                        
                        

                        if (approve)
                        {
                            foreach (var detail in item.ProductionScheduleRequestDetail)
                            {
                                foreach (var salesRequestOrQuotationDetailProductionScheduleRequestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                                {
                                    ServiceSalesRequest.UpdateQuantityRecivedSalesRequestDetail(db, salesRequestOrQuotationDetailProductionScheduleRequestDetail.id_salesRequestDetail,
                                                                                   salesRequestOrQuotationDetailProductionScheduleRequestDetail.quantity);
                                }
                            }

                            ServiceInventoryMove.UpdateInventoryReservationDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, item.ProductionScheduleInventoryReservationDetail.OrderByDescending(obd=> obd.id).ToList(), db, false);
                            ServiceSalesOrder.UpdateSalesOrderDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, item.ProductionScheduleProductionOrderDetail.OrderByDescending(obd=> obd.id).ToList(), db, false);
                            ServicePurchaseRequest.UpdatePurchaseRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, item.ProductionScheduleScheduleDetail.OrderBy(obd=> obd.datePlanning).ToList(), db, false);

                            documentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                            item.Document.id_documentState = documentState.id;
                            item.Document.DocumentState = documentState;

                        }

                        //db.ProductionSchedule.Attach(item);
                        //db.Entry(item).State = EntityState.Modified;
                        db.ProductionSchedule.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        TempData["productionSchedule"] = item;
                        TempData.Keep("productionSchedule");

                        ViewData["EditMessage"] = SuccessMessage("Programación de Producción: " + item.Document.number + " guardado exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData["productionSchedule"] = productionScheduleError;
                        TempData.Keep("productionSchedule");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                        return PartialView("_ProductionScheduleEditFormPartial", productionScheduleError);
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            //TempData["productionLot"] = item;
            //TempData.Keep("productionLot");

            return PartialView("_ProductionScheduleEditFormPartial", item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionSchedulesPartialUpdate(bool approve, ProductionSchedule item, Document itemDoc)
        {
            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);
            productionSchedule = productionSchedule ?? new ProductionSchedule();

            var modelItem = db.ProductionSchedule.FirstOrDefault(p => p.id == item.id);

            var productionScheduleError = productionSchedule;
            productionScheduleError.Document = modelItem.Document;
            productionScheduleError.Document.emissionDate = itemDoc.emissionDate;
            productionScheduleError.Document.description = itemDoc.description;
            productionScheduleError.id_personSchedule = item.id_personSchedule;
            productionScheduleError.Employee = db.Employee.FirstOrDefault(fod => fod.id == item.id_personSchedule);
            productionScheduleError.id_productionSchedulePeriod = modelItem.id_productionSchedulePeriod;
            productionScheduleError.ProductionSchedulePeriod = db.ProductionSchedulePeriod.FirstOrDefault(fod => fod.id == modelItem.id_productionSchedulePeriod);


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

                        #region ProductionSchedule

                        modelItem.id_personSchedule = item.id_personSchedule;
                        modelItem.Employee = db.Employee.FirstOrDefault(fod=> fod.id == item.id_personSchedule);
                        //modelItem.id_productionSchedulePeriod = item.id_productionSchedulePeriod;
                        //modelItem.ProductionSchedulePeriod = db.ProductionSchedulePeriod.FirstOrDefault(fod=> fod.id == item.id_productionSchedulePeriod);

                        #endregion

                        #region ProductionScheduleRequestDetail

                        if (productionSchedule.ProductionScheduleRequestDetail != null)
                        {
                            #region DeleteAll ProductionScheduleRequestDetail Don't Find
                            for (int i = modelItem.ProductionScheduleRequestDetail.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.ProductionScheduleRequestDetail.ElementAt(i);

                                if(productionSchedule.ProductionScheduleRequestDetail.FirstOrDefault(fod=> fod.id == detail.id && fod.id_item == detail.id_item) == null)
                                {
                                    for (int j = detail.ProductionScheduleInventoryReservationDetail.Count - 1; j >= 0; j--)
                                    {
                                        var detailProductionScheduleInventoryReservationDetail = detail.ProductionScheduleInventoryReservationDetail.ElementAt(j);

                                        for (int k = detailProductionScheduleInventoryReservationDetail.InventoryReservation.Count - 1; k >= 0; k--)
                                        {
                                            var detailInventoryReservation = detailProductionScheduleInventoryReservationDetail.InventoryReservation.ElementAt(k);

                                            detailInventoryReservation.id_salesRequestDetail = null;
                                            detailInventoryReservation.SalesRequestDetail = null;

                                            detailInventoryReservation.id_salesQuotationDetail = null;
                                            detailInventoryReservation.SalesQuotationDetail = null;

                                            detailInventoryReservation.id_productionScheduleInventoryReservationDetail = null;
                                            detailInventoryReservation.ProductionScheduleInventoryReservationDetail = null;

                                            db.InventoryReservation.Attach(detailInventoryReservation);
                                            db.Entry(detailInventoryReservation).State = EntityState.Modified;
                                        }

                                        detail.ProductionScheduleInventoryReservationDetail.Remove(detailProductionScheduleInventoryReservationDetail);
                                        db.Entry(detailProductionScheduleInventoryReservationDetail).State = EntityState.Deleted;
                                    }

                                    for (int j = detail.ProductionScheduleProductionOrderDetail.Count - 1; j >= 0; j--)
                                    {
                                        var detailProductionScheduleProductionOrderDetail = detail.ProductionScheduleProductionOrderDetail.ElementAt(j);

                                        for (int k = detailProductionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Count - 1; k >= 0; k--)
                                        {
                                            var detailProductionSchedulePurchaseRequestDetail = detailProductionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.ElementAt(k);

                                            for (int m = detailProductionSchedulePurchaseRequestDetail.ProductionScheduleScheduleDetail.Count - 1; m >= 0; m--)
                                            {
                                                var detailProductionScheduleScheduleDetail = detailProductionSchedulePurchaseRequestDetail.ProductionScheduleScheduleDetail.ElementAt(m);

                                                for (int n = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Count - 1; n >= 0; n--)
                                                {
                                                    var detailProductionScheduleScheduleDetailPurchaseRequestDetail = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.ElementAt(n);

                                                    detailProductionScheduleScheduleDetailPurchaseRequestDetail.id_productionScheduleScheduleDetail = null;
                                                    detailProductionScheduleScheduleDetailPurchaseRequestDetail.ProductionScheduleScheduleDetail = null;

                                                    db.ProductionScheduleScheduleDetailPurchaseRequestDetail.Attach(detailProductionScheduleScheduleDetailPurchaseRequestDetail);
                                                    db.Entry(detailProductionScheduleScheduleDetailPurchaseRequestDetail).State = EntityState.Modified;
                                                }

                                                detailProductionSchedulePurchaseRequestDetail.ProductionScheduleScheduleDetail.Remove(detailProductionScheduleScheduleDetail);
                                                db.Entry(detailProductionScheduleScheduleDetail).State = EntityState.Deleted;
                                            }

                                            detailProductionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Remove(detailProductionSchedulePurchaseRequestDetail);
                                            db.Entry(detailProductionSchedulePurchaseRequestDetail).State = EntityState.Deleted;
                                        }

                                        for (int k = detailProductionScheduleProductionOrderDetail.ProductionScheduleProductionOrderDetailSalesOrderDetail.Count - 1; k >= 0; k--)
                                        {
                                            var detailProductionScheduleProductionOrderDetailSalesOrderDetail = detailProductionScheduleProductionOrderDetail.ProductionScheduleProductionOrderDetailSalesOrderDetail.ElementAt(k);

                                            detailProductionScheduleProductionOrderDetailSalesOrderDetail.id_productionScheduleProductionOrderDetail = null;
                                            detailProductionScheduleProductionOrderDetailSalesOrderDetail.ProductionScheduleProductionOrderDetail = null;

                                            db.ProductionScheduleProductionOrderDetailSalesOrderDetail.Attach(detailProductionScheduleProductionOrderDetailSalesOrderDetail);
                                            db.Entry(detailProductionScheduleProductionOrderDetailSalesOrderDetail).State = EntityState.Modified;
                                        }

                                        detail.ProductionScheduleProductionOrderDetail.Remove(detailProductionScheduleProductionOrderDetail);
                                        db.Entry(detailProductionScheduleProductionOrderDetail).State = EntityState.Deleted;
                                    }

                                    for (int j = detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Count - 1; j >= 0; j--)
                                    {
                                        var detailSalesRequestOrQuotationDetailProductionScheduleRequestDetail = detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.ElementAt(j);
                                        detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Remove(detailSalesRequestOrQuotationDetailProductionScheduleRequestDetail);
                                        db.Entry(detailSalesRequestOrQuotationDetailProductionScheduleRequestDetail).State = EntityState.Deleted;
                                    }

                                    modelItem.ProductionScheduleRequestDetail.Remove(detail);
                                    db.Entry(detail).State = EntityState.Deleted;
                                }
                                
                            }

                            #endregion

                            #region Update ProductionScheduleRequestDetail

                            foreach (var detail in productionSchedule.ProductionScheduleRequestDetail)
                            {
                                var productionScheduleRequestDetail = modelItem.ProductionScheduleRequestDetail.FirstOrDefault(fod=> fod.id == detail.id && fod.id_item == detail.id_item);
                                if(productionScheduleRequestDetail == null)
                                {
                                    productionScheduleRequestDetail = new ProductionScheduleRequestDetail();
                                    productionScheduleRequestDetail.id = detail.id;
                                    productionScheduleRequestDetail.id_item = detail.id_item;
                                    productionScheduleRequestDetail.Item = db.Item.FirstOrDefault(i => i.id == productionScheduleRequestDetail.id_item);
                                    productionScheduleRequestDetail.quantityRequest = detail.quantityRequest;
                                    productionScheduleRequestDetail.quantitySchedule = detail.quantitySchedule;
                                    productionScheduleRequestDetail.id_metricUnit = detail.id_metricUnit;
                                    productionScheduleRequestDetail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == productionScheduleRequestDetail.id_metricUnit);
                                    productionScheduleRequestDetail.quantitySale = detail.quantitySale;

                                    productionScheduleRequestDetail.reservedInInventory = detail.reservedInInventory;

                                    productionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail = new List<SalesRequestOrQuotationDetailProductionScheduleRequestDetail>();
                                    productionScheduleRequestDetail.ProductionScheduleInventoryReservationDetail = new List<ProductionScheduleInventoryReservationDetail>();

                                    foreach (var requestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                                    {
                                        //ProductionScheduleRequestDetail
                                        //var inventoryMoveDetailAux = db.InventoryMoveDetail.FirstOrDefault(fod => fod.id == detailInventoryMoveDetailTransfer.id_inventoryMoveDetailExit);
                                        //var id_metricUnitMovExitAux = inventoryMoveDetailAux.id_metricUnitMove;

                                        decimal amountSaleAux = 0;
                                        var factorConversion = (productionScheduleRequestDetail.id_metricUnit != requestDetail.id_metricUnitRequestDetail) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == productionScheduleRequestDetail.id_metricUnit &&
                                                                                                                                                                                                           fod.id_metricDestiny == requestDetail.id_metricUnitRequestDetail)?.factor ?? 0 : 1;
                                        if (factorConversion == 0)
                                        {
                                            throw new Exception("Falta el Factor de Conversión entre : " + productionScheduleRequestDetail.MetricUnit.code + " y " + requestDetail.MetricUnit.code + ".Necesario para la cantidad recibida del requerimiento de pedido Origen Configúrelo, e intente de nuevo");
                                        }
                                        else
                                        {
                                            amountSaleAux = productionScheduleRequestDetail.quantitySchedule * factorConversion;
                                        }

                                        productionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Add(new SalesRequestOrQuotationDetailProductionScheduleRequestDetail
                                        {
                                            id_salesRequestDetail = requestDetail.id_salesRequestDetail,
                                            SalesRequestDetail = db.SalesRequestDetail.FirstOrDefault(i => i.id == requestDetail.id_salesRequestDetail),
                                            id_salesRequest = requestDetail.id_salesRequest,
                                            SalesRequest = db.SalesRequest.FirstOrDefault(i => i.id == requestDetail.id_salesRequest),
                                            id_metricUnitRequestDetail = requestDetail.id_metricUnitRequestDetail,
                                            MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == requestDetail.id_metricUnitRequestDetail),
                                            quantity = amountSaleAux
                                        });
                                        if (approve)
                                        {
                                            ServiceSalesOrder.UpdateQuantityOutstandingOrderSalesRequestDetail(db, requestDetail.id_salesRequestDetail, amountSaleAux);
                                        }
                                    }

                                    modelItem.ProductionScheduleRequestDetail.Add(productionScheduleRequestDetail);
                                }
                                else
                                {
                                    productionScheduleRequestDetail.quantitySchedule = detail.quantitySchedule;
                                    productionScheduleRequestDetail.id_metricUnit = detail.id_metricUnit;
                                    productionScheduleRequestDetail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == productionScheduleRequestDetail.id_metricUnit);
                                    productionScheduleRequestDetail.quantitySale = detail.quantitySale;
                                    productionScheduleRequestDetail.reservedInInventory = detail.reservedInInventory;

                                    foreach (var requestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                                    {
                                        //ProductionScheduleRequestDetail
                                        //var inventoryMoveDetailAux = db.InventoryMoveDetail.FirstOrDefault(fod => fod.id == detailInventoryMoveDetailTransfer.id_inventoryMoveDetailExit);
                                        //var id_metricUnitMovExitAux = inventoryMoveDetailAux.id_metricUnitMove;

                                        decimal amountSaleAux = 0;
                                        var factorConversion = (productionScheduleRequestDetail.id_metricUnit != requestDetail.id_metricUnitRequestDetail) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == productionScheduleRequestDetail.id_metricUnit &&
                                                                                                                                                                                                           fod.id_metricDestiny == requestDetail.id_metricUnitRequestDetail)?.factor ?? 0 : 1;
                                        if (factorConversion == 0)
                                        {
                                            throw new Exception("Falta el Factor de Conversión entre : " + productionScheduleRequestDetail.MetricUnit.code + " y " + requestDetail.MetricUnit.code + ".Necesario para la cantidad recibida del requerimiento de pedido Origen Configúrelo, e intente de nuevo");
                                        }
                                        else
                                        {
                                            amountSaleAux = productionScheduleRequestDetail.quantitySchedule * factorConversion;
                                        }
                                        var detailSalesRequestOrQuotationDetailProductionScheduleRequestDetail = productionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.FirstOrDefault(fod=> fod.id == requestDetail.id);
                                        detailSalesRequestOrQuotationDetailProductionScheduleRequestDetail.quantity = amountSaleAux;
                                        //productionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Add(new SalesRequestOrQuotationDetailProductionScheduleRequestDetail
                                        //{
                                        //    id_salesRequestDetail = requestDetail.id_salesRequestDetail,
                                        //    SalesRequestDetail = db.SalesRequestDetail.FirstOrDefault(i => i.id == requestDetail.id_salesRequestDetail),
                                        //    id_salesRequest = requestDetail.id_salesRequest,
                                        //    SalesRequest = db.SalesRequest.FirstOrDefault(i => i.id == requestDetail.id_salesRequest),
                                        //    id_metricUnitRequestDetail = requestDetail.id_metricUnitRequestDetail,
                                        //    MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == requestDetail.id_metricUnitRequestDetail),
                                        //    quantity = amountSaleAux
                                        //});
                                        db.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Attach(detailSalesRequestOrQuotationDetailProductionScheduleRequestDetail);
                                        db.Entry(detailSalesRequestOrQuotationDetailProductionScheduleRequestDetail).State = EntityState.Modified;
                                    }

                                    db.ProductionScheduleRequestDetail.Attach(productionScheduleRequestDetail);
                                    db.Entry(productionScheduleRequestDetail).State = EntityState.Modified;
                                }
                                
                            }
                            #endregion
                        }

                        if (modelItem.ProductionScheduleRequestDetail.Count == 0)
                        {
                            TempData["productionSchedule"] = productionScheduleError;
                            TempData.Keep("productionSchedule");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una programación sin detalles de requerimientos");
                            return PartialView("_ProductionScheduleEditFormPartial", productionScheduleError);
                        }

                        #endregion

                        #region ProductionScheduleInventoryReservationDetail

                        if (productionSchedule.ProductionScheduleInventoryReservationDetail != null)
                        {
                            //item.ProductionScheduleInventoryReservationDetail = new List<ProductionScheduleInventoryReservationDetail>();
                            foreach (var detail in productionSchedule.ProductionScheduleInventoryReservationDetail)
                            {
                                var productionScheduleInventoryReservationDetail = modelItem.ProductionScheduleInventoryReservationDetail.FirstOrDefault(fod => fod.id == detail.id);
                                if(productionScheduleInventoryReservationDetail == null)
                                {
                                    productionScheduleInventoryReservationDetail = new ProductionScheduleInventoryReservationDetail();
                                    productionScheduleInventoryReservationDetail.id = detail.id;
                                    productionScheduleInventoryReservationDetail.quantity = detail.quantity;

                                    var productionScheduleRequestDetailAux = modelItem.ProductionScheduleRequestDetail.FirstOrDefault(fod => fod.id == detail.id_productionScheduleRequestDetail);
                                    productionScheduleRequestDetailAux.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                    modelItem.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                }
                                else
                                {
                                    productionScheduleInventoryReservationDetail.quantity = detail.quantity;

                                    db.ProductionScheduleInventoryReservationDetail.Attach(productionScheduleInventoryReservationDetail);
                                    db.Entry(productionScheduleInventoryReservationDetail).State = EntityState.Modified;
                                    //var productionScheduleRequestDetailAux = modelItem.ProductionScheduleRequestDetail.FirstOrDefault(fod => fod.id == detail.id_productionScheduleRequestDetail);
                                    //productionScheduleRequestDetailAux.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                    //modelItem.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                }
                                
                            }
                        }

                        #endregion

                        #region ProductionScheduleProductionOrderDetail

                        if (productionSchedule.ProductionScheduleProductionOrderDetail != null)
                        {
                            foreach (var detail in productionSchedule.ProductionScheduleProductionOrderDetail)
                            {
                                var productionScheduleProductionOrderDetail = modelItem.ProductionScheduleProductionOrderDetail.FirstOrDefault(fod => fod.id == detail.id);
                                if (productionScheduleProductionOrderDetail == null)
                                {
                                    productionScheduleProductionOrderDetail = new ProductionScheduleProductionOrderDetail();
                                    productionScheduleProductionOrderDetail.id = detail.id;
                                    productionScheduleProductionOrderDetail.quantityRequest = detail.quantityRequest;
                                    productionScheduleProductionOrderDetail.quantityProductionOrder = detail.quantityProductionOrder;

                                    var productionScheduleRequestDetailAux = modelItem.ProductionScheduleRequestDetail.FirstOrDefault(fod => fod.id == detail.id_productionScheduleRequestDetail);
                                    productionScheduleRequestDetailAux.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                                    modelItem.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                                }
                                else
                                {
                                    productionScheduleProductionOrderDetail.quantityRequest = detail.quantityRequest;
                                    productionScheduleProductionOrderDetail.quantityProductionOrder = detail.quantityProductionOrder;

                                    db.ProductionScheduleProductionOrderDetail.Attach(productionScheduleProductionOrderDetail);
                                    db.Entry(productionScheduleProductionOrderDetail).State = EntityState.Modified;
                                    //var productionScheduleRequestDetailAux = modelItem.ProductionScheduleRequestDetail.FirstOrDefault(fod => fod.id == detail.id_productionScheduleRequestDetail);
                                    //productionScheduleRequestDetailAux.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                    //modelItem.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                }
                                
                            }
                        }

                        #endregion

                        #region ProductionSchedulePurchaseRequestDetail

                        if (productionSchedule.ProductionSchedulePurchaseRequestDetail != null)
                        {
                            foreach (var detail in productionSchedule.ProductionSchedulePurchaseRequestDetail)
                            {
                                var productionSchedulePurchaseRequestDetail = modelItem.ProductionSchedulePurchaseRequestDetail.FirstOrDefault(fod => fod.id == detail.id);
                                if (productionSchedulePurchaseRequestDetail == null)
                                {
                                    productionSchedulePurchaseRequestDetail = new ProductionSchedulePurchaseRequestDetail();
                                    productionSchedulePurchaseRequestDetail.id = detail.id;
                                    productionSchedulePurchaseRequestDetail.id_item = detail.id_item;
                                    productionSchedulePurchaseRequestDetail.Item = db.Item.FirstOrDefault(fod => fod.id == detail.id_item);
                                    productionSchedulePurchaseRequestDetail.quantity = detail.quantity;

                                    var productionScheduleProductionOrderDetailAux = modelItem.ProductionScheduleProductionOrderDetail.FirstOrDefault(fod => fod.id == detail.id_productionScheduleProductionOrderDetail);
                                    productionScheduleProductionOrderDetailAux.ProductionSchedulePurchaseRequestDetail.Add(productionSchedulePurchaseRequestDetail);
                                    modelItem.ProductionSchedulePurchaseRequestDetail.Add(productionSchedulePurchaseRequestDetail);

                                }
                                else
                                {
                                    productionSchedulePurchaseRequestDetail.id_item = detail.id_item;
                                    productionSchedulePurchaseRequestDetail.Item = db.Item.FirstOrDefault(fod => fod.id == detail.id_item);
                                    productionSchedulePurchaseRequestDetail.quantity = detail.quantity;

                                    db.ProductionSchedulePurchaseRequestDetail.Attach(productionSchedulePurchaseRequestDetail);
                                    db.Entry(productionSchedulePurchaseRequestDetail).State = EntityState.Modified;
                                    //var productionScheduleRequestDetailAux = modelItem.ProductionScheduleRequestDetail.FirstOrDefault(fod => fod.id == detail.id_productionScheduleRequestDetail);
                                    //productionScheduleRequestDetailAux.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                    //modelItem.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                }

                            }
                        }

                        #endregion

                        #region ProductionScheduleScheduleDetail

                        if (productionSchedule.ProductionScheduleScheduleDetail != null)
                        {
                            //item.ProductionScheduleScheduleDetail = new List<ProductionScheduleScheduleDetail>();
                            foreach (var detail in productionSchedule.ProductionScheduleScheduleDetail)
                            {
                                var productionScheduleScheduleDetail = modelItem.ProductionScheduleScheduleDetail.FirstOrDefault(fod => fod.id == detail.id);
                                if (productionScheduleScheduleDetail == null)
                                {
                                    productionScheduleScheduleDetail = new ProductionScheduleScheduleDetail();
                                    productionScheduleScheduleDetail.id = detail.id;
                                    productionScheduleScheduleDetail.id_provider = detail.id_provider;
                                    productionScheduleScheduleDetail.Provider = db.Provider.FirstOrDefault(i => i.id == detail.id_provider);
                                    productionScheduleScheduleDetail.id_buyer = detail.id_buyer;
                                    productionScheduleScheduleDetail.Person = db.Person.FirstOrDefault(i => i.id == detail.id_buyer);
                                    productionScheduleScheduleDetail.id_item = detail.id_item;
                                    productionScheduleScheduleDetail.Item = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                                    productionScheduleScheduleDetail.quantity = detail.quantity;
                                    productionScheduleScheduleDetail.datePlanning = detail.datePlanning;

                                    var productionSchedulePurchaseRequestDetailAux = modelItem.ProductionSchedulePurchaseRequestDetail.FirstOrDefault(fod => fod.id == detail.id_productionSchedulePurchaseRequestDetail);
                                    productionSchedulePurchaseRequestDetailAux.ProductionScheduleScheduleDetail.Add(productionScheduleScheduleDetail);
                                    modelItem.ProductionScheduleScheduleDetail.Add(productionScheduleScheduleDetail);

                                }
                                else
                                {
                                    productionScheduleScheduleDetail.id_provider = detail.id_provider;
                                    productionScheduleScheduleDetail.Provider = db.Provider.FirstOrDefault(i => i.id == detail.id_provider);
                                    productionScheduleScheduleDetail.id_buyer = detail.id_buyer;
                                    productionScheduleScheduleDetail.Person = db.Person.FirstOrDefault(i => i.id == detail.id_buyer);
                                    productionScheduleScheduleDetail.id_item = detail.id_item;
                                    productionScheduleScheduleDetail.Item = db.Item.FirstOrDefault(i => i.id == detail.id_item);
                                    productionScheduleScheduleDetail.quantity = detail.quantity;
                                    productionScheduleScheduleDetail.datePlanning = detail.datePlanning;

                                    db.ProductionScheduleScheduleDetail.Attach(productionScheduleScheduleDetail);
                                    db.Entry(productionScheduleScheduleDetail).State = EntityState.Modified;
                                    //var productionScheduleRequestDetailAux = modelItem.ProductionScheduleRequestDetail.FirstOrDefault(fod => fod.id == detail.id_productionScheduleRequestDetail);
                                    //productionScheduleRequestDetailAux.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                    //modelItem.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                                }

                            }
                        }

                        if (modelItem.ProductionScheduleScheduleDetail.Count == 0)
                        {
                            TempData["productionSchedule"] = productionScheduleError;
                            TempData.Keep("productionSchedule");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una programación sin detalles de planificación");
                            return PartialView("_ProductionScheduleEditFormPartial", productionScheduleError);
                        }

                        #endregion

                        if (approve)
                        {
                            foreach (var detail in modelItem.ProductionScheduleRequestDetail)
                            {
                                foreach (var salesRequestOrQuotationDetailProductionScheduleRequestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                                {
                                    ServiceSalesRequest.UpdateQuantityRecivedSalesRequestDetail(db, salesRequestOrQuotationDetailProductionScheduleRequestDetail.id_salesRequestDetail,
                                                                                   salesRequestOrQuotationDetailProductionScheduleRequestDetail.quantity);
                                }
                            }

                            ServiceInventoryMove.UpdateInventoryReservationDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem.ProductionScheduleInventoryReservationDetail.OrderByDescending(obd => obd.id).ToList(), db, false);
                            ServiceSalesOrder.UpdateSalesOrderDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem.ProductionScheduleProductionOrderDetail.OrderByDescending(obd => obd.id).ToList(), db, false);
                            ServicePurchaseRequest.UpdatePurchaseRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem.ProductionScheduleScheduleDetail.OrderBy(obd => obd.datePlanning).ToList(), db, false);

                            var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                            modelItem.Document.id_documentState = documentState.id;
                            modelItem.Document.DocumentState = documentState;
                        }

                        db.ProductionSchedule.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["productionSchedule"] = modelItem;
                        TempData.Keep("productionSchedule");

                        ViewData["EditMessage"] = SuccessMessage("Programación de Producción: " + modelItem.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData["productionSchedule"] = productionScheduleError;
                        TempData.Keep("productionSchedule");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                        return PartialView("_ProductionScheduleEditFormPartial", productionScheduleError);

                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor , corrija todos los errores.";

            
            //TempData.Keep("productionLot");

            return PartialView("_ProductionScheduleEditFormPartial", modelItem);
        }

        #endregion

        #region Production Schedule Request Detail

        [ValidateInput(false)]
        public ActionResult ProductionScheduleViewFormRequestsDetailPartial(int? id_productionSchedule)
        {
            ViewData["id_productionSchedule"] = id_productionSchedule;
            var productionSchedule = db.ProductionSchedule.FirstOrDefault(p => p.id == id_productionSchedule);
            productionSchedule = productionSchedule ?? (TempData["ProductionSchedule"] as ProductionSchedule);
            var model = productionSchedule?.ProductionScheduleRequestDetail.OrderByDescending(od=> od.id).ToList() ?? new List<ProductionScheduleRequestDetail>();
            TempData.Keep("productionSchedule");
            return PartialView("_ProductionScheduleViewFormProductionScheduleRequestsDetailPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult ProductionScheduleEditFormRequestsDetailPartial()
        {
            ProductionSchedule productionSchedule = (TempData["ProductionSchedule"] as ProductionSchedule);

            //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
            productionSchedule = productionSchedule ?? new ProductionSchedule();

            var model = productionSchedule?.ProductionScheduleRequestDetail.OrderByDescending(od => od.id).ToList() ?? new List<ProductionScheduleRequestDetail>();

            

            TempData["productionSchedule"] = TempData["productionSchedule"] ?? productionSchedule;
            TempData.Keep("productionSchedule");

            return PartialView("_ProductionScheduleEditFormProductionScheduleRequestsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionScheduleEditFormRequestsDetailAddNew(ProductionScheduleRequestDetail item)
        {

            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);
            //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
            productionSchedule = productionSchedule ?? new ProductionSchedule();
            productionSchedule.ProductionScheduleRequestDetail = productionSchedule.ProductionScheduleRequestDetail ?? new List<ProductionScheduleRequestDetail>();

            if (ModelState.IsValid)
            {
                item.id = productionSchedule.ProductionScheduleRequestDetail.Count() > 0 ? productionSchedule.ProductionScheduleRequestDetail.Max(ppd => ppd.id) + 1 : 1;
                item.Item = db.Item.FirstOrDefault(fod=> fod.id == item.id_item);
                item.MetricUnit = db.MetricUnit.FirstOrDefault(fod=> fod.id == item.id_metricUnit);
                item.SalesRequestOrQuotationDetailProductionScheduleRequestDetail = new List<SalesRequestOrQuotationDetailProductionScheduleRequestDetail>();
                item.ProductionScheduleInventoryReservationDetail = new List<ProductionScheduleInventoryReservationDetail>();
                item.ProductionScheduleProductionOrderDetail = new List<ProductionScheduleProductionOrderDetail>();

                productionSchedule.ProductionScheduleRequestDetail.Add(item);
                UpdateInventoryReservationDetailAndProductionOrderDetail(productionSchedule, item);
            }

            TempData["productionSchedule"] = productionSchedule;
            TempData.Keep("productionSchedule");

            var model = productionSchedule?.ProductionScheduleRequestDetail.OrderByDescending(od => od.id).ToList() ?? new List<ProductionScheduleRequestDetail>();

            ViewData["RefreshRequest"] = true;
            return PartialView("_ProductionScheduleEditFormProductionScheduleRequestsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionScheduleEditFormRequestsDetailUpdate(ProductionScheduleRequestDetail item)
        {
            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);
            //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
            productionSchedule = productionSchedule ?? new ProductionSchedule();
            productionSchedule.ProductionScheduleRequestDetail = productionSchedule.ProductionScheduleRequestDetail ?? new List<ProductionScheduleRequestDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionSchedule.ProductionScheduleRequestDetail.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.id_item = item.id_item;
                        modelItem.Item = db.Item.FirstOrDefault(fod => fod.id == item.id_item);
                        modelItem.id_metricUnit = item.id_metricUnit;
                        modelItem.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == item.id_metricUnit);
                        //modelItem.quantitySale = item.quantitySale;
                        //modelItem.quantitySchedule = item.quantitySchedule;
                        this.UpdateModel(modelItem);
                        UpdateInventoryReservationDetailAndProductionOrderDetail(productionSchedule, modelItem);
                        TempData["productionSchedule"] = productionSchedule;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("productionSchedule");

            var model = productionSchedule?.ProductionScheduleRequestDetail.OrderByDescending(od => od.id).ToList() ?? new List<ProductionScheduleRequestDetail>();

            ViewData["RefreshRequest"] = true;
            return PartialView("_ProductionScheduleEditFormProductionScheduleRequestsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionScheduleEditFormRequestsDetailDelete(System.Int32 id)
        {
            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);
            //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
            productionSchedule = productionSchedule ?? new ProductionSchedule();
            productionSchedule.ProductionScheduleRequestDetail = productionSchedule.ProductionScheduleRequestDetail ?? new List<ProductionScheduleRequestDetail>();

            try
            {
                var productionScheduleRequestDetail = productionSchedule.ProductionScheduleRequestDetail.FirstOrDefault(p => p.id == id);
                if (productionScheduleRequestDetail != null)
                {
                    if(productionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Count() <= 0)
                    {
                        ProductionScheduleInventoryReservationDetail productionScheduleInventoryReservationDetail = productionSchedule.ProductionScheduleInventoryReservationDetail.FirstOrDefault(fod => fod.id_productionScheduleRequestDetail == productionScheduleRequestDetail.id);
                        if (productionScheduleInventoryReservationDetail != null)
                        {
                            DeleteProductionScheduleInventoryReservationDetail(productionSchedule, productionScheduleInventoryReservationDetail);

                        }

                        ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail = productionSchedule.ProductionScheduleProductionOrderDetail.FirstOrDefault(fod => fod.id_productionScheduleRequestDetail == productionScheduleRequestDetail.id);
                        if (productionScheduleProductionOrderDetail != null)
                        {
                            DeleteProductionScheduleProductionOrderDetailDetail(productionSchedule, productionScheduleProductionOrderDetail);

                        }
                        productionSchedule.ProductionScheduleRequestDetail.Remove(productionScheduleRequestDetail);
                    }
                }

                //UpdateProductionLotTotals(purchasePlanning);
                TempData["productionSchedule"] = productionSchedule;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("productionSchedule");

            var model = productionSchedule?.ProductionScheduleRequestDetail.OrderByDescending(od => od.id).ToList() ?? new List<ProductionScheduleRequestDetail>();
            ViewData["RefreshRequest"] = true;
            return PartialView("_ProductionScheduleEditFormProductionScheduleRequestsDetailPartial", model);
        }

        #endregion

        #region Production Schedule Inventory Reservation Detail

        [ValidateInput(false)]
        public ActionResult ProductionScheduleViewFormInventoryReservationsDetailPartial(int? id_productionSchedule)
        {
            ViewData["id_productionSchedule"] = id_productionSchedule;
            var productionSchedule = (TempData["ProductionSchedule"] as ProductionSchedule);
            productionSchedule = productionSchedule ?? db.ProductionSchedule.FirstOrDefault(p => p.id == id_productionSchedule);
            var model = productionSchedule?.ProductionScheduleInventoryReservationDetail.OrderByDescending(od => od.id).ToList() ?? new List<ProductionScheduleInventoryReservationDetail>();
            TempData.Keep("productionSchedule");

            return PartialView("_ProductionScheduleViewFormProductionScheduleInventoryReservationsDetailPartial", model.ToList());
        }

        #endregion

        #region Production Schedule Production Order Detail

        [ValidateInput(false)]
        public ActionResult ProductionScheduleViewFormProductionOrdersDetailPartial(int? id_productionSchedule)
        {
            ViewData["id_productionSchedule"] = id_productionSchedule;
            var productionSchedule = (TempData["ProductionSchedule"] as ProductionSchedule);
            productionSchedule = productionSchedule ?? db.ProductionSchedule.FirstOrDefault(p => p.id == id_productionSchedule);
            var model = productionSchedule?.ProductionScheduleProductionOrderDetail.OrderByDescending(od => od.id).ToList() ?? new List<ProductionScheduleProductionOrderDetail>();
            TempData.Keep("productionSchedule");

            return PartialView("_ProductionScheduleViewFormProductionScheduleProductionOrderDetailsDetailPartial", model.ToList());
            
        }

        #endregion

        #region Production Schedule Purchase Request Detail

        [ValidateInput(false)]
        public ActionResult ProductionScheduleViewFormPurchaseRequestsDetailPartial(int? id_productionSchedule)
        {
            ViewData["id_productionSchedule"] = id_productionSchedule;
            var productionSchedule = (TempData["ProductionSchedule"] as ProductionSchedule);
            productionSchedule = productionSchedule ?? db.ProductionSchedule.FirstOrDefault(p => p.id == id_productionSchedule);
            var model = productionSchedule?.ProductionSchedulePurchaseRequestDetail.OrderByDescending(od => od.id).ToList() ?? new List<ProductionSchedulePurchaseRequestDetail>();
            TempData.Keep("productionSchedule");

            return PartialView("_ProductionScheduleViewFormProductionSchedulePurchaseRequestsDetailPartial", model.ToList());
        }

        #endregion

        #region Production Schedule Schedule Detail

        [ValidateInput(false)]
        public ActionResult ProductionScheduleViewFormSchedulesDetailPartial(int? id_productionSchedule)
        {
            ViewData["id_productionSchedule"] = id_productionSchedule;
            var productionSchedule = db.ProductionSchedule.FirstOrDefault(p => p.id == id_productionSchedule);
            productionSchedule = productionSchedule ?? (TempData["ProductionSchedule"] as ProductionSchedule);
            var model = productionSchedule?.ProductionScheduleScheduleDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<ProductionScheduleScheduleDetail>();
            TempData.Keep("productionSchedule");

            return PartialView("_ProductionScheduleViewFormProductionScheduleSchedulesDetailPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult ProductionScheduleEditFormSchedulesDetailPartial()
        {
            ProductionSchedule productionSchedule = (TempData["ProductionSchedule"] as ProductionSchedule);

            //purchasePlanning = purchasePlanning ?? db.PurchasePlanning.FirstOrDefault(i => i.id == purchasePlanning.id);
            productionSchedule = productionSchedule ?? new ProductionSchedule();

            var model = productionSchedule?.ProductionScheduleScheduleDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<ProductionScheduleScheduleDetail>();

            UpdateViewDataDataSourceProductionSchedulePurchaseRequestDetail(productionSchedule);
            TempData["productionSchedule"] = TempData["productionSchedule"] ?? productionSchedule;
            TempData.Keep("productionSchedule");

            return PartialView("_ProductionScheduleEditFormProductionScheduleSchedulesDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionScheduleEditFormSchedulesDetailAddNew(ProductionScheduleScheduleDetail item)
        {

            ProductionSchedule productionSchedule = (TempData["ProductionSchedule"] as ProductionSchedule);
            productionSchedule = productionSchedule ?? new ProductionSchedule();

            productionSchedule.ProductionScheduleScheduleDetail = productionSchedule.ProductionScheduleScheduleDetail ?? new List<ProductionScheduleScheduleDetail>();

            if (ModelState.IsValid)
            {
                item.id = productionSchedule.ProductionScheduleScheduleDetail.Count() > 0 ? productionSchedule.ProductionScheduleScheduleDetail.Max(ppd => ppd.id) + 1 : 1;
                item.ProductionSchedulePurchaseRequestDetail = productionSchedule.ProductionSchedulePurchaseRequestDetail.FirstOrDefault(fod=> fod.id == item.id_productionSchedulePurchaseRequestDetail);
                 
                item.Provider = db.Provider.FirstOrDefault(fod=> fod.id == item.id_provider);
                item.Person = db.Person.FirstOrDefault(fod=> fod.id == item.id_buyer);
                item.id_item = item.ProductionSchedulePurchaseRequestDetail.id_item;
                item.Item = db.Item.FirstOrDefault(fod=> fod.id == item.ProductionSchedulePurchaseRequestDetail.id_item);

                productionSchedule.ProductionScheduleScheduleDetail.Add(item);
                item.ProductionSchedulePurchaseRequestDetail.ProductionScheduleScheduleDetail.Add(item);
                //UpdateProductionLotTotals(purchasePlanning);
            }

            TempData["productionSchedule"] = productionSchedule;
            TempData.Keep("productionSchedule");

            UpdateViewDataDataSourceProductionSchedulePurchaseRequestDetail(productionSchedule);
            var model = productionSchedule?.ProductionScheduleScheduleDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<ProductionScheduleScheduleDetail>();

            return PartialView("_ProductionScheduleEditFormProductionScheduleSchedulesDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionScheduleEditFormSchedulesDetailUpdate(ProductionScheduleScheduleDetail item)
        {
            ProductionSchedule productionSchedule = (TempData["ProductionSchedule"] as ProductionSchedule);
            productionSchedule = productionSchedule ?? new ProductionSchedule();

            productionSchedule.ProductionScheduleScheduleDetail = productionSchedule.ProductionScheduleScheduleDetail ?? new List<ProductionScheduleScheduleDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionSchedule.ProductionScheduleScheduleDetail.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.ProductionSchedulePurchaseRequestDetail = productionSchedule.ProductionSchedulePurchaseRequestDetail.FirstOrDefault(fod => fod.id == item.id_productionSchedulePurchaseRequestDetail);
                        modelItem.Provider = db.Provider.FirstOrDefault(fod => fod.id == item.id_provider);
                        modelItem.Person = db.Person.FirstOrDefault(fod => fod.id == item.id_buyer);
                        modelItem.id_item = modelItem.ProductionSchedulePurchaseRequestDetail.id_item;
                        modelItem.Item = db.Item.FirstOrDefault(fod => fod.id == modelItem.ProductionSchedulePurchaseRequestDetail.id_item);
                        this.UpdateModel(modelItem);
                        UpdateProductionScheduleScheduleDetailPurchaseRequestDetail(modelItem);
                        TempData["productionSchedule"] = productionSchedule;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    UpdateViewDataDataSourceProductionSchedulePurchaseRequestDetail(productionSchedule);
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("productionSchedule");

            UpdateViewDataDataSourceProductionSchedulePurchaseRequestDetail(productionSchedule);
            var model = productionSchedule?.ProductionScheduleScheduleDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<ProductionScheduleScheduleDetail>();

            return PartialView("_ProductionScheduleEditFormProductionScheduleSchedulesDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionScheduleEditFormSchedulesDetailDelete(System.Int32 id)
        {
            ProductionSchedule productionSchedule = (TempData["ProductionSchedule"] as ProductionSchedule);
            productionSchedule = productionSchedule ?? new ProductionSchedule();

            productionSchedule.ProductionScheduleScheduleDetail = productionSchedule.ProductionScheduleScheduleDetail ?? new List<ProductionScheduleScheduleDetail>();

            try
            {
                var modelItem = productionSchedule.ProductionScheduleScheduleDetail.FirstOrDefault(it => it.id == id);
                if (modelItem != null)
                {
                    DeleteProductionScheduleScheduleDetailPurchaseRequestDetail(modelItem);
                    productionSchedule.ProductionScheduleScheduleDetail.Remove(modelItem);
                }
                    

                //UpdateProductionLotTotals(purchasePlanning);
                TempData["productionSchedule"] = productionSchedule;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
                UpdateViewDataDataSourceProductionSchedulePurchaseRequestDetail(productionSchedule);
            }

            TempData.Keep("productionSchedule");

            UpdateViewDataDataSourceProductionSchedulePurchaseRequestDetail(productionSchedule);
            var model = productionSchedule?.ProductionScheduleScheduleDetail.OrderBy(od => od.datePlanning).ToList() ?? new List<ProductionScheduleScheduleDetail>();
            return PartialView("_ProductionScheduleEditFormProductionScheduleSchedulesDetailPartial", model);
        }

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

        #region SINGLE CHANGE ProductionSchedule STATE

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
            ProductionSchedule productionSchedule = db.ProductionSchedule.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "04");//Cerrada

                    if (productionSchedule != null && documentState != null)
                    {
                        productionSchedule.Document.id_documentState = documentState.id;
                        productionSchedule.Document.DocumentState = documentState;

                        db.ProductionSchedule.Attach(productionSchedule);
                        db.Entry(productionSchedule).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["productionSchedule"] = productionSchedule;
                        TempData.Keep("productionSchedule");

                        ViewData["EditMessage"] = SuccessMessage("Programación de Producción: " + productionSchedule.Document.number + " cerrado exitosamente");

                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("productionSchedule");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            //TempData["purchasePlanning"] = purchasePlanning;
            //TempData.Keep("purchasePlanning");
            //ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + purchasePlanning.Document.number + " cerrada exitosamente");

            return PartialView("_ProductionScheduleEditFormPartial", productionSchedule);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            ProductionSchedule productionSchedule = db.ProductionSchedule.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");//Anulado

                    if (productionSchedule != null && documentState != null)
                    {
                        if (productionSchedule.Document.DocumentState.code != "01")// Pendiente
                        {
                            ServiceInventoryMove.UpdateInventoryReservationDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionSchedule.ProductionScheduleInventoryReservationDetail.OrderByDescending(obd => obd.id).ToList(), db, true);
                            ServiceSalesOrder.UpdateSalesOrderDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionSchedule.ProductionScheduleProductionOrderDetail.OrderByDescending(obd => obd.id).ToList(), db, true);
                            ServicePurchaseRequest.UpdatePurchaseRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionSchedule.ProductionScheduleScheduleDetail.OrderBy(obd => obd.datePlanning).ToList(), db, true);

                            //foreach (var detail in productionSchedule.ProductionScheduleRequestDetail)
                            //{
                            //    foreach (var detailProductionScheduleRequestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                            //    {
                            //        ServiceSalesOrder.UpdateQuantityOutstandingOrderSalesRequestDetail(db, detailProductionScheduleRequestDetail.id_salesRequestDetail, -detailProductionScheduleRequestDetail.quantity);
                            //    }
                            //}
                            foreach (var detail in productionSchedule.ProductionScheduleRequestDetail)
                            {
                                foreach (var salesRequestOrQuotationDetailProductionScheduleRequestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                                {
                                    ServiceSalesRequest.UpdateQuantityRecivedSalesRequestDetail(db, salesRequestOrQuotationDetailProductionScheduleRequestDetail.id_salesRequestDetail,
                                                                                   -salesRequestOrQuotationDetailProductionScheduleRequestDetail.quantity);
                                }
                            }
                        }

                        productionSchedule.Document.id_documentState = documentState.id;
                        productionSchedule.Document.DocumentState = documentState;

                        db.ProductionSchedule.Attach(productionSchedule);
                        db.Entry(productionSchedule).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["productionSchedule"] = productionSchedule;
                        TempData.Keep("productionSchedule");

                        ViewData["EditMessage"] = SuccessMessage("Programación de Producción: " + productionSchedule.Document.number + " anulada exitosamente");
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("productionSchedule");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            //TempData["purchasePlanning"] = purchasePlanning;
            //TempData.Keep("purchasePlanning");
            //ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + purchasePlanning.Document.number + " anulada exitosamente");

            return PartialView("_ProductionScheduleEditFormPartial", productionSchedule);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            ProductionSchedule productionSchedule = db.ProductionSchedule.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

                    if (productionSchedule != null && documentState != null)
                    {
                        ServiceInventoryMove.UpdateInventoryReservationDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionSchedule.ProductionScheduleInventoryReservationDetail.OrderByDescending(obd => obd.id).ToList(), db, true);
                        ServiceSalesOrder.UpdateSalesOrderDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionSchedule.ProductionScheduleProductionOrderDetail.OrderByDescending(obd => obd.id).ToList(), db, true);
                        ServicePurchaseRequest.UpdatePurchaseRequestDetail(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionSchedule.ProductionScheduleScheduleDetail.OrderBy(obd => obd.datePlanning).ToList(), db, true);

                        //foreach (var detail in productionSchedule.ProductionScheduleRequestDetail)
                        //{
                        //    foreach (var detailProductionScheduleRequestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                        //    {
                        //        ServiceSalesOrder.UpdateQuantityOutstandingOrderSalesRequestDetail(db, detailProductionScheduleRequestDetail.id_salesRequestDetail, -detailProductionScheduleRequestDetail.quantity);
                        //    }
                        //}

                        foreach (var detail in productionSchedule.ProductionScheduleRequestDetail)
                        {
                            foreach (var salesRequestOrQuotationDetailProductionScheduleRequestDetail in detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail)
                            {
                                ServiceSalesRequest.UpdateQuantityRecivedSalesRequestDetail(db, salesRequestOrQuotationDetailProductionScheduleRequestDetail.id_salesRequestDetail,
                                                                               -salesRequestOrQuotationDetailProductionScheduleRequestDetail.quantity);
                            }
                        }

                        productionSchedule.Document.id_documentState = documentState.id;
                        productionSchedule.Document.DocumentState = documentState;

                        db.ProductionSchedule.Attach(productionSchedule);
                        db.Entry(productionSchedule).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["productionSchedule"] = productionSchedule;
                        TempData.Keep("productionSchedule");

                        ViewData["EditMessage"] = SuccessMessage("Programación de Producción: " + productionSchedule.Document.number + " reversada exitosamente");
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("productionSchedule");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            //TempData["purchasePlanning"] = purchasePlanning;
            //TempData.Keep("purchasePlanning");
            //ViewData["EditMessage"] = SuccessMessage("Planificación de Compra: " + purchasePlanning.Document.number + " anulada exitosamente");

            return PartialView("_ProductionScheduleEditFormPartial", productionSchedule);
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
            //purchasePlanningReport.DataSource = new PurchasePlanningCompany
            //{
            //    number = purchasePlanning?.Document.number ?? "",
            //    state = purchasePlanning?.Document.DocumentState.name ?? "",
            //    emissionDate = purchasePlanning?.Document.emissionDate.ToString("dd/MM/yyyy") ?? "",
            //    period = purchasePlanning?.PurchasePlanningPeriod.name ?? "",
            //    personPlanning = purchasePlanning?.Employee.Person.fullname_businessName ?? "",
            //    description = purchasePlanning?.Document.description ?? "",
            //    list_id_purchasePlanning = new List<int>(id),
            //    listPurchasePlanningDetail = purchasePlanning?.PurchasePlanningDetail.AsEnumerable()
            //                                                                         .Select(s => new PurchasePlanningDetailReport
            //                                                                         {
            //                                                                             datePlanningStr = s.datePlanning.ToString("ddd").ToUpper() +
            //                                                                                               s.datePlanning.ToString("_dd"),
            //                                                                             datePlanning = s.datePlanning,
            //                                                                             id_provider = s.id_provider,
            //                                                                             provider = s.Provider.Person.fullname_businessName,
            //                                                                             id_buyer = s.id_buyer,
            //                                                                             buyer = s.Person.fullname_businessName,
            //                                                                             id_item = s.id_item ?? 0,
            //                                                                             item = s.Item?.name ?? "",
            //                                                                             id_itemTypeCategory = s.id_itemTypeCategory,
            //                                                                             itemTypeCategory = s.ItemTypeCategory.name,
            //                                                                             quantity = s.quantity
            //                                                                         }).OrderBy(ob => ob.id_itemTypeCategory)
            //                                                                         .OrderBy(ob => ob.id_item)
            //                                                                         .OrderBy(ob => ob.id_buyer)
            //                                                                         .OrderBy(ob => ob.id_provider)
            //                                                                         .OrderBy(ob => ob.datePlanning)
            //                                                                         .ToList(),
            //    company = company
            //};
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

            ProductionSchedule productionSchedule = db.ProductionSchedule.FirstOrDefault(r => r.id == id);
            string code_state = productionSchedule.Document.DocumentState.code;

            if (code_state == "01") // PENDIENTE
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = false
                };
            }
            else if (code_state == "03") // APROBADA
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
            else if (code_state == "04") // CERRADA
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

        private void UpdateInventoryReservationDetailAndProductionOrderDetail(ProductionSchedule productionSchedule, ProductionScheduleRequestDetail productionScheduleRequestDetail)
        {
            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            int? id_metricUnitPresentation;
            MetricUnitConversion metricUnitConversion = null;
            decimal factor;

            var quantityAux = productionScheduleRequestDetail.quantitySchedule;

            //id_metricUnitPresentation = inventoryMoveDetailBalancesLast.Item.Presentation?.id_metricUnit ?? metricUnitUMTP.id;
            metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                     muc.id_metricOrigin == productionScheduleRequestDetail.id_metricUnit &&
                                                                                     muc.id_metricDestiny == metricUnitUMTP.id);

            if (metricUnitUMTP.id != productionScheduleRequestDetail.id_metricUnit && metricUnitConversion == null)
                throw new Exception("No existe factor de conversión entre " + (productionScheduleRequestDetail.MetricUnit.code ?? metricUnitUMTP.code) + " y " + metricUnitUMTP.code + " necesario para reservar producto en Invetario, configúrelo e intentelo de nuevo.");

            factor = (metricUnitUMTP.id == productionScheduleRequestDetail.id_metricUnit) ? 1 : (metricUnitConversion.factor);

            quantityAux *= factor;

            var quantityReservated = productionSchedule.ProductionScheduleInventoryReservationDetail.Where(w => w.ProductionScheduleRequestDetail.id_item == productionScheduleRequestDetail.id_item &&
                                                                                                                w.id_productionScheduleRequestDetail != productionScheduleRequestDetail.id).Sum(s => s.quantity);

            decimal balacesAux = -quantityReservated;

            if (productionScheduleRequestDetail.reservedInInventory)
            {
                var inventoryMoveDetailBalancesLasts = db.InventoryMoveDetail.Where(w => w.InventoryMove.Document.DocumentState.code.Equals("03") &&
                                                                                                w.id_inventoryMoveDetailNext == null &&
                                                                                                w.balance > 0 &&
                                                                                                w.id_item == productionScheduleRequestDetail.id_item &&
                                                                                                w.Warehouse.WarehouseType.code != ("RES01")).OrderBy(ob => ob.id_lot).ThenByDescending(tbd => tbd.inMaximumUnit).ToList();

                foreach (var inventoryMoveDetailBalancesLast in inventoryMoveDetailBalancesLasts)
                {
                    id_metricUnitPresentation = inventoryMoveDetailBalancesLast.Item.Presentation?.id_metricUnit ?? metricUnitUMTP.id;
                    metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                             muc.id_metricOrigin == id_metricUnitPresentation &&
                                                                                             muc.id_metricDestiny == metricUnitUMTP.id);

                    if (metricUnitUMTP.id != id_metricUnitPresentation && metricUnitConversion == null)
                        throw new Exception("No existe factor de conversión entre " + (inventoryMoveDetailBalancesLast.Item.Presentation?.MetricUnit.code ?? metricUnitUMTP.code) + " y " + metricUnitUMTP.code + " necesario para reservar producto en Invetario, configúrelo e intentelo de nuevo.");

                    factor = (metricUnitUMTP.id == id_metricUnitPresentation) ? 1 : (metricUnitConversion.factor);

                    if (inventoryMoveDetailBalancesLast.inMaximumUnit)
                    {
                        balacesAux += (inventoryMoveDetailBalancesLast.balance *
                                      (inventoryMoveDetailBalancesLast.Item.Presentation?.maximum ?? 1) *
                                      (inventoryMoveDetailBalancesLast.Item.Presentation?.minimum ?? 1) *
                                      factor);
                    }
                    else
                    {
                        balacesAux += (inventoryMoveDetailBalancesLast.balance *
                                      (inventoryMoveDetailBalancesLast.Item.Presentation?.minimum ?? 1) *
                                      factor);
                    }

                    if (balacesAux >= quantityAux)
                    {
                        //ProductionScheduleInventoryReservationDetail productionScheduleInventoryReservationDetail = new ProductionScheduleInventoryReservationDetail
                        //{
                        //    id = productionSchedule.ProductionScheduleInventoryReservationDetail.Count() > 0 ? productionSchedule.ProductionScheduleInventoryReservationDetail.Max(rpsd => rpsd.id) + 1 : 1,
                        //    id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                        //    ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                        //    quantity = quantityAux
                        //};
                        //productionScheduleRequestDetail.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                        //productionSchedule.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                        break;
                    }
                }
            }else
            {
                balacesAux = 0;
            }
            

            id_metricUnitPresentation = productionScheduleRequestDetail.Item.Presentation?.id_metricUnit ?? metricUnitUMTP.id;
            metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                     muc.id_metricOrigin == metricUnitUMTP.id &&
                                                                                     muc.id_metricDestiny == id_metricUnitPresentation);

            if (metricUnitUMTP.id != id_metricUnitPresentation && metricUnitConversion == null)
                throw new Exception("No existe factor de conversión entre " + metricUnitUMTP.code + " y " + (productionScheduleRequestDetail.Item.Presentation?.MetricUnit.code ?? metricUnitUMTP.code) + " necesario para generar los producto en la Orden de Producción, configúrelo e intentelo de nuevo.");

            factor = (metricUnitUMTP.id == id_metricUnitPresentation) ? 1 : (metricUnitConversion.factor);

            if (balacesAux == 0)
            {
                ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail = productionSchedule.ProductionScheduleProductionOrderDetail.FirstOrDefault(fod=> fod.id_productionScheduleRequestDetail == productionScheduleRequestDetail.id);
                if(productionScheduleProductionOrderDetail != null)
                {
                    productionScheduleProductionOrderDetail.quantityRequest = quantityAux;

                }else
                {
                    productionScheduleProductionOrderDetail = new ProductionScheduleProductionOrderDetail
                    {
                        id = productionSchedule.ProductionScheduleProductionOrderDetail.Count() > 0 ? productionSchedule.ProductionScheduleProductionOrderDetail.Max(rpsd => rpsd.id) + 1 : 1,
                        id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                        ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                        quantityRequest = quantityAux
                    };
                    productionScheduleRequestDetail.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                    productionSchedule.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                }
                

                productionScheduleProductionOrderDetail.quantityProductionOrder = (quantityAux * factor) /
                                                                                  (productionScheduleRequestDetail.Item.Presentation?.minimum ?? 1);

                var truncateQuantityProductionOrder = decimal.Truncate(productionScheduleProductionOrderDetail.quantityProductionOrder);
                if ((productionScheduleProductionOrderDetail.quantityProductionOrder - truncateQuantityProductionOrder) > 0)
                {
                    productionScheduleProductionOrderDetail.quantityProductionOrder = truncateQuantityProductionOrder + 1;
                };

                UpdateProductionSchedulePurchaseRequestDetail(productionSchedule, productionScheduleProductionOrderDetail);

                ProductionScheduleInventoryReservationDetail productionScheduleInventoryReservationDetail = productionSchedule.ProductionScheduleInventoryReservationDetail.FirstOrDefault(fod => fod.id_productionScheduleRequestDetail == productionScheduleRequestDetail.id);
                if (productionScheduleInventoryReservationDetail != null)
                {
                    DeleteProductionScheduleInventoryReservationDetail(productionSchedule, productionScheduleInventoryReservationDetail);

                }
            }
            else
            {
                if (balacesAux >= quantityAux)
                {
                    ProductionScheduleInventoryReservationDetail productionScheduleInventoryReservationDetail = productionSchedule.ProductionScheduleInventoryReservationDetail.FirstOrDefault(fod => fod.id_productionScheduleRequestDetail == productionScheduleRequestDetail.id);
                    if (productionScheduleInventoryReservationDetail != null)
                    {
                        productionScheduleInventoryReservationDetail.quantity = quantityAux;

                    }
                    else
                    {
                        productionScheduleInventoryReservationDetail = new ProductionScheduleInventoryReservationDetail
                        {
                            id = productionSchedule.ProductionScheduleInventoryReservationDetail.Count() > 0 ? productionSchedule.ProductionScheduleInventoryReservationDetail.Max(rpsd => rpsd.id) + 1 : 1,
                            id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                            ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                            quantity = quantityAux
                        };
                        productionScheduleRequestDetail.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                        productionSchedule.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                    }

                    ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail = productionSchedule.ProductionScheduleProductionOrderDetail.FirstOrDefault(fod => fod.id_productionScheduleRequestDetail == productionScheduleRequestDetail.id);
                    if (productionScheduleProductionOrderDetail != null)
                    {
                        DeleteProductionScheduleProductionOrderDetailDetail(productionSchedule, productionScheduleProductionOrderDetail);

                    }

                }
                else
                {
                    ProductionScheduleInventoryReservationDetail productionScheduleInventoryReservationDetail = productionSchedule.ProductionScheduleInventoryReservationDetail.FirstOrDefault(fod => fod.id_productionScheduleRequestDetail == productionScheduleRequestDetail.id);
                    if (productionScheduleInventoryReservationDetail != null)
                    {
                        productionScheduleInventoryReservationDetail.quantity = balacesAux;

                    }
                    else
                    {
                        productionScheduleInventoryReservationDetail = new ProductionScheduleInventoryReservationDetail
                        {
                            id = productionSchedule.ProductionScheduleInventoryReservationDetail.Count() > 0 ? productionSchedule.ProductionScheduleInventoryReservationDetail.Max(rpsd => rpsd.id) + 1 : 1,
                            id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                            ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                            quantity = balacesAux
                        };
                        productionScheduleRequestDetail.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                        productionSchedule.ProductionScheduleInventoryReservationDetail.Add(productionScheduleInventoryReservationDetail);
                    }

                    ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail = productionSchedule.ProductionScheduleProductionOrderDetail.FirstOrDefault(fod => fod.id_productionScheduleRequestDetail == productionScheduleRequestDetail.id);
                    if (productionScheduleProductionOrderDetail != null)
                    {
                        productionScheduleProductionOrderDetail.quantityRequest = (quantityAux - balacesAux);

                    }
                    else
                    {
                        productionScheduleProductionOrderDetail = new ProductionScheduleProductionOrderDetail
                        {
                            id = productionSchedule.ProductionScheduleProductionOrderDetail.Count() > 0 ? productionSchedule.ProductionScheduleProductionOrderDetail.Max(rpsd => rpsd.id) + 1 : 1,
                            id_productionScheduleRequestDetail = productionScheduleRequestDetail.id,
                            ProductionScheduleRequestDetail = productionScheduleRequestDetail,
                            quantityRequest = (quantityAux - balacesAux)
                        };
                        productionScheduleRequestDetail.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                        productionSchedule.ProductionScheduleProductionOrderDetail.Add(productionScheduleProductionOrderDetail);
                    }


                    productionScheduleProductionOrderDetail.quantityProductionOrder = (productionScheduleProductionOrderDetail.quantityRequest * factor) /
                                                                                      (productionScheduleRequestDetail.Item.Presentation?.minimum ?? 1);

                    var truncateQuantityProductionOrder = decimal.Truncate(productionScheduleProductionOrderDetail.quantityProductionOrder);
                    if ((productionScheduleProductionOrderDetail.quantityProductionOrder - truncateQuantityProductionOrder) > 0)
                    {
                        productionScheduleProductionOrderDetail.quantityProductionOrder = truncateQuantityProductionOrder + 1;
                    };

                    UpdateProductionSchedulePurchaseRequestDetail(productionSchedule, productionScheduleProductionOrderDetail);

                }
            }

            
        }

        private void DeleteProductionScheduleInventoryReservationDetail(ProductionSchedule productionSchedule, ProductionScheduleInventoryReservationDetail productionScheduleInventoryReservationDetail)
        {
            productionSchedule.ProductionScheduleInventoryReservationDetail.Remove(productionScheduleInventoryReservationDetail);
            productionScheduleInventoryReservationDetail.ProductionScheduleRequestDetail.ProductionScheduleInventoryReservationDetail.Remove(productionScheduleInventoryReservationDetail);
            //for (int i = productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Count - 1; i >= 0; i--)
            //{

            //    var detail = productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.ElementAt(i);

            //    if (!idItems.Contains(detail.id_item))
            //    {
            //        for (int j = detail.ProductionScheduleScheduleDetail.Count - 1; j >= 0; j--)
            //        {
            //            var detailProductionScheduleScheduleDetail = detail.ProductionScheduleScheduleDetail.ElementAt(j);

            //            for (int k = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Count - 1; k >= 0; k--)
            //            {
            //                var detailProductionScheduleScheduleDetailPurchaseRequestDetail = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.ElementAt(k);
            //                //detail.ProductionScheduleScheduleDetail.Remove(detailProductionScheduleScheduleDetail);
            //                //detailProductionScheduleScheduleDetail.id_productionSchedulePurchaseRequestDetail = null;
            //                detailProductionScheduleScheduleDetailPurchaseRequestDetail.id_productionScheduleScheduleDetail = null;
            //                detailProductionScheduleScheduleDetailPurchaseRequestDetail.ProductionScheduleScheduleDetail = null;
            //                //db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
            //            }


            //            detail.ProductionScheduleScheduleDetail.Remove(detailProductionScheduleScheduleDetail);
            //            //detailProductionScheduleScheduleDetail.id_productionSchedulePurchaseRequestDetail = null;
            //            //detailProductionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail = null;
            //            //db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
            //        }
            //        //detail.ProductionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail .Remove(detail);
            //        productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Remove(detail);
            //        productionSchedule.ProductionSchedulePurchaseRequestDetail.Remove(detail);

            //    }
            //}
        }

        private void DeleteProductionScheduleProductionOrderDetailDetail(ProductionSchedule productionSchedule, ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail)
        {
            
            for (int i = productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Count - 1; i >= 0; i--)
            {

                var detail = productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.ElementAt(i);

                //if (!idItems.Contains(detail.id_item))
                //{
                    for (int j = detail.ProductionScheduleScheduleDetail.Count - 1; j >= 0; j--)
                    {
                        var detailProductionScheduleScheduleDetail = detail.ProductionScheduleScheduleDetail.ElementAt(j);

                        for (int k = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Count - 1; k >= 0; k--)
                        {
                            var detailProductionScheduleScheduleDetailPurchaseRequestDetail = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.ElementAt(k);
                            //detail.ProductionScheduleScheduleDetail.Remove(detailProductionScheduleScheduleDetail);
                            //detailProductionScheduleScheduleDetail.id_productionSchedulePurchaseRequestDetail = null;
                            detailProductionScheduleScheduleDetailPurchaseRequestDetail.id_productionScheduleScheduleDetail = null;
                            detailProductionScheduleScheduleDetailPurchaseRequestDetail.ProductionScheduleScheduleDetail = null;
                            //db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
                        }


                        detail.ProductionScheduleScheduleDetail.Remove(detailProductionScheduleScheduleDetail);
                        //detailProductionScheduleScheduleDetail.id_productionSchedulePurchaseRequestDetail = null;
                        //detailProductionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail = null;
                        //db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
                    }
                    //detail.ProductionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail .Remove(detail);
                    productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Remove(detail);
                    productionSchedule.ProductionSchedulePurchaseRequestDetail.Remove(detail);

            }
            productionSchedule.ProductionScheduleProductionOrderDetail.Remove(productionScheduleProductionOrderDetail);
            productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.ProductionScheduleProductionOrderDetail.Remove(productionScheduleProductionOrderDetail);
            //}
        }

        private void UpdateProductionSchedulePurchaseRequestDetail(ProductionSchedule productionSchedule, ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail)
        {

            //if (!productionLotLiquidation.isActive) return;
            if (!productionSchedule.ProductionScheduleProductionOrderDetail.Any(a => a.id == productionScheduleProductionOrderDetail.id)) return;

            if (productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item == null)
            {
                productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item = db.Item.FirstOrDefault(fod => fod.id == productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.id_item);
            }
            var itemIngredientMP = productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemIngredient.Where(w => w.Item1.InventoryLine.code.Equals("MP"));//"MP": Linea de Inventario Materia Prima
            var itemIngredientPP = productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemIngredient.Where(w => w.Item1.InventoryLine.code.Equals("PP"));//"PP": Linea de Inventario Producto en Proceso

            var idItems = new List<int>();


            //if (itemIngredientMP.Count() == 0) return;
            var id_metricUnitProductionScheduleProductionOrderDetail = productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemSaleInformation?.id_metricUnitSale;//ItemPurchaseInformation?.id_metricUnitPurchase;
            var id_metricUnitItemHeadIngredient = productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemHeadIngredient?.id_metricUnit;
            var factorConversionLiquidationFormulation = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                             muc.id_metricOrigin == id_metricUnitProductionScheduleProductionOrderDetail &&
                                                                                             muc.id_metricDestiny == id_metricUnitItemHeadIngredient);
            if (id_metricUnitProductionScheduleProductionOrderDetail != null && id_metricUnitProductionScheduleProductionOrderDetail == id_metricUnitItemHeadIngredient)
            {
                factorConversionLiquidationFormulation = new MetricUnitConversion() { factor = 1 };
            }
            if (factorConversionLiquidationFormulation == null)
            {
                var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
                throw new Exception("Falta el Factor de Conversión entre : " + (metricUnitProductionScheduleProductionOrderDetail?.code ?? "(UM No Existe)") + ", del Ítem: " + productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.name + " y " + (productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemHeadIngredient?.MetricUnit?.code ?? "(UM No Existe)") + " configurado en la cabecera de la formulación del este Ítem. Necesario para cargar el requerimiento de compra Configúrelo, e intente de nuevo");
            }

            var quantityMetricUnitItemHeadIngredient = productionScheduleProductionOrderDetail.quantityProductionOrder * factorConversionLiquidationFormulation.factor;

            foreach (var iipp in itemIngredientPP)
            {
                var amountItemHeadIngredient = (productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemHeadIngredient?.amount ?? 0);
                if (amountItemHeadIngredient == 0)
                {
                    throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
                }
                var quantityItemIngredientPP = (quantityMetricUnitItemHeadIngredient * (iipp.amount ?? 0)) / amountItemHeadIngredient;
                if (quantityItemIngredientPP == 0) continue;
                var truncateQuantityItemIngredientPP = decimal.Truncate(quantityItemIngredientPP);
                if ((quantityItemIngredientPP - truncateQuantityItemIngredientPP) > 0)
                {
                    quantityItemIngredientPP = truncateQuantityItemIngredientPP + 1;
                };

                idItems = UpdateProductionSchedulePurchaseRequestDetailPP(productionSchedule, productionScheduleProductionOrderDetail, idItems, iipp.Item1, quantityItemIngredientPP, iipp.MetricUnit);
            }

            foreach (var iimdd in itemIngredientMP)
            {
                var amountItemHeadIngredient = (productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemHeadIngredient?.amount ?? 0);
                if (amountItemHeadIngredient == 0)
                {
                    throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
                }
                var quantityItemIngredientMP = (quantityMetricUnitItemHeadIngredient * (iimdd.amount ?? 0)) / amountItemHeadIngredient;
                if (quantityItemIngredientMP == 0) continue;

                //if(iimdd.Item1.MetricType.DataType.code.Equals("ENTE01"))//"ENTE01" Codigo de Entero de Tipo de Datos en la unidad de medida
                //{
                var truncateQuantityItemIngredientMP = decimal.Truncate(quantityItemIngredientMP);
                if ((quantityItemIngredientMP - truncateQuantityItemIngredientMP) > 0)
                {
                    quantityItemIngredientMP = truncateQuantityItemIngredientMP + 1;
                };
                //}
                var id_metricUnitFormulation = iimdd.id_metricUnit;
                var id_metricUnitPurchase = iimdd.Item1.ItemPurchaseInformation?.id_metricUnitPurchase;
                var factorConversionFormulationPurchase = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                                  muc.id_metricOrigin == id_metricUnitFormulation &&
                                                                                                  muc.id_metricDestiny == id_metricUnitPurchase);
                if (id_metricUnitFormulation != null && id_metricUnitFormulation == id_metricUnitPurchase)
                {
                    factorConversionFormulationPurchase = new MetricUnitConversion() { factor = 1 };
                }
                if (factorConversionFormulationPurchase == null)
                {
                    throw new Exception("Falta el Factor de Conversión entre : " + iimdd.MetricUnit?.code ?? "(UM No Existe)" + ", del Ítem: " + iimdd.Item1.name + " y " + iimdd.Item1.ItemPurchaseInformation?.MetricUnit.code ?? "(UM No Existe)" + " configurado en el detalle de la formulación del Ítem: " + productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.name + ". Necesario para cargar el requerimiento de compra Configúrelo, e intente de nuevo");
                }

                var quantityUMPurchase = quantityItemIngredientMP * factorConversionFormulationPurchase.factor;

                var truncateQuantityUMPurchase = decimal.Truncate(quantityUMPurchase);
                if ((quantityUMPurchase - truncateQuantityUMPurchase) > 0)
                {
                    quantityUMPurchase = truncateQuantityUMPurchase + 1;
                };

                ProductionSchedulePurchaseRequestDetail productionSchedulePurchaseRequestDetail = productionSchedule.
                                                                                                  ProductionSchedulePurchaseRequestDetail.
                                                                                                  FirstOrDefault(fod => fod.id_item == iimdd.id_ingredientItem &&
                                                                                                                        fod.id_productionScheduleProductionOrderDetail == productionScheduleProductionOrderDetail.id);
                if (productionSchedulePurchaseRequestDetail != null)
                {
                    productionSchedulePurchaseRequestDetail.quantity = !idItems.Contains(iimdd.id_ingredientItem) ? quantityUMPurchase: +quantityUMPurchase;
                    //productionLotPackingMaterial.manual = false;
                    //productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                    //productionLotPackingMaterial.dateUpdate = DateTime.Now;
                }
                else
                {
                    productionSchedulePurchaseRequestDetail = new ProductionSchedulePurchaseRequestDetail
                    {
                        id = productionSchedule.ProductionSchedulePurchaseRequestDetail.Count() > 0 ? productionSchedule.ProductionSchedulePurchaseRequestDetail.Max(pld => pld.id) + 1 : 1,
                        id_item = iimdd.id_ingredientItem,
                        Item = db.Item.FirstOrDefault(i => i.id == iimdd.id_ingredientItem),
                        quantity = quantityUMPurchase,
                        id_productionScheduleProductionOrderDetail = productionScheduleProductionOrderDetail.id,
                        ProductionScheduleProductionOrderDetail = productionScheduleProductionOrderDetail
                        //quantity = quantityUMInventory,
                        //manual = false,
                        //isActive = true,
                        //id_userCreate = ActiveUser.id,
                        //dateCreate = DateTime.Now,
                        //id_userUpdate = ActiveUser.id,
                        //dateUpdate = DateTime.Now,
                        //ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>()
                    };
                    productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Add(productionSchedulePurchaseRequestDetail);
                    productionSchedule.ProductionSchedulePurchaseRequestDetail.Add(productionSchedulePurchaseRequestDetail);
                }
                idItems.Add(iimdd.id_ingredientItem);
                //var productionLotLiquidationPackingMaterialDetail = new ProductionLotLiquidationPackingMaterialDetail
                //{
                //    ProductionLotLiquidation = productionLotLiquidation,
                //    id_productionLotLiquidation = productionLotLiquidation.id,
                //    ProductionLotPackingMaterial = productionLotPackingMaterial,
                //    id_productionLotPackingMaterial = productionLotPackingMaterial.id,
                //    quantity = quantityUMInventory
                //};
                //productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
                //productionLotPackingMaterial.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
            }

            for (int i = productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Count - 1; i >= 0; i--)
            {

                var detail = productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.ElementAt(i);

                if (!idItems.Contains(detail.id_item))
                {
                    for (int j = detail.ProductionScheduleScheduleDetail.Count - 1; j >= 0; j--)
                    {
                        var detailProductionScheduleScheduleDetail = detail.ProductionScheduleScheduleDetail.ElementAt(j);

                        for (int k = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Count - 1; k >= 0; k--)
                        {
                            var detailProductionScheduleScheduleDetailPurchaseRequestDetail = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.ElementAt(k);
                            //detail.ProductionScheduleScheduleDetail.Remove(detailProductionScheduleScheduleDetail);
                            //detailProductionScheduleScheduleDetail.id_productionSchedulePurchaseRequestDetail = null;
                            detailProductionScheduleScheduleDetailPurchaseRequestDetail.id_productionScheduleScheduleDetail = null;
                            detailProductionScheduleScheduleDetailPurchaseRequestDetail.ProductionScheduleScheduleDetail = null;
                            //db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
                        }

                        
                        detail.ProductionScheduleScheduleDetail.Remove(detailProductionScheduleScheduleDetail);
                        //detailProductionScheduleScheduleDetail.id_productionSchedulePurchaseRequestDetail = null;
                        //detailProductionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail = null;
                        //db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
                    }
                    //detail.ProductionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail .Remove(detail);
                    productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Remove(detail);
                    productionSchedule.ProductionSchedulePurchaseRequestDetail.Remove(detail);

                }
            }
        }

        private List<int> UpdateProductionSchedulePurchaseRequestDetailPP(ProductionSchedule productionSchedule, ProductionScheduleProductionOrderDetail productionScheduleProductionOrderDetail, List<int> idItems, Item itemPP, decimal quantityItemPP, MetricUnit metricUnitPP)
        {

            //if (!productionSchedule.ProductionScheduleProductionOrderDetail.Any(a => a.id == productionScheduleProductionOrderDetail.id)) return;

            //if (productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item == null)
            //{
            //    productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item = db.Item.FirstOrDefault(fod => fod.id == productionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.id_item);
            //}
            var itemIngredientMP = itemPP.ItemIngredient.Where(w => w.Item1.InventoryLine.code.Equals("MP"));//"MP": Linea de Inventario Materia Prima
            var itemIngredientPP = itemPP.ItemIngredient.Where(w => w.Item1.InventoryLine.code.Equals("PP"));//"PP": Linea de Inventario Producto en Proceso

            //var idItems = new List<int>();


            //if (itemIngredientMP.Count() == 0) return;
            var id_metricUnitAuxDetail = metricUnitPP?.id;
            var id_metricUnitItemHeadIngredient = itemPP.ItemHeadIngredient?.id_metricUnit;
            var factorConversionFormulation = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                             muc.id_metricOrigin == id_metricUnitAuxDetail &&
                                                                                             muc.id_metricDestiny == id_metricUnitItemHeadIngredient);
            if (id_metricUnitAuxDetail != null && id_metricUnitAuxDetail == id_metricUnitItemHeadIngredient)
            {
                factorConversionFormulation = new MetricUnitConversion() { factor = 1 };
            }
            if (factorConversionFormulation == null)
            {
                //var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
                throw new Exception("Falta el Factor de Conversión entre : " + (metricUnitPP?.code ?? "(UM No Existe)") + ", del Ítem: " + itemPP.name + " y " + (itemPP.ItemHeadIngredient?.MetricUnit?.code ?? "(UM No Existe)") + " configurado en la cabecera de la formulación del este Ítem. Necesario para cargar el requerimiento de compra Configúrelo, e intente de nuevo");
            }

            var quantityMetricUnitItemHeadIngredient = quantityItemPP * factorConversionFormulation.factor;

            foreach (var iipp in itemIngredientPP)
            {
                var amountItemHeadIngredient = (itemPP.ItemHeadIngredient?.amount ?? 0);
                if (amountItemHeadIngredient == 0)
                {
                    throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + itemPP.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
                }
                var quantityItemIngredientPP = (quantityMetricUnitItemHeadIngredient * (iipp.amount ?? 0)) / amountItemHeadIngredient;
                if (quantityItemIngredientPP == 0) continue;
                var truncateQuantityItemIngredientPP = decimal.Truncate(quantityItemIngredientPP);
                if ((quantityItemIngredientPP - truncateQuantityItemIngredientPP) > 0)
                {
                    quantityItemIngredientPP = truncateQuantityItemIngredientPP + 1;
                };

                idItems = UpdateProductionSchedulePurchaseRequestDetailPP(productionSchedule, productionScheduleProductionOrderDetail, idItems, iipp.Item1, quantityItemIngredientPP, iipp.MetricUnit);
            }

            foreach (var iimp in itemIngredientMP)
            {
                var amountItemHeadIngredient = (itemPP.ItemHeadIngredient?.amount ?? 0);
                if (amountItemHeadIngredient == 0)
                {
                    throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + itemPP.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
                }
                var quantityItemIngredientMP = (quantityMetricUnitItemHeadIngredient * (iimp.amount ?? 0)) / amountItemHeadIngredient;
                if (quantityItemIngredientMP == 0) continue;

                //if(iimdd.Item1.MetricType.DataType.code.Equals("ENTE01"))//"ENTE01" Codigo de Entero de Tipo de Datos en la unidad de medida
                //{
                var truncateQuantityItemIngredientMP = decimal.Truncate(quantityItemIngredientMP);
                if ((quantityItemIngredientMP - truncateQuantityItemIngredientMP) > 0)
                {
                    quantityItemIngredientMP = truncateQuantityItemIngredientMP + 1;
                };
                //}
                var id_metricUnitFormulation = iimp.id_metricUnit;
                var id_metricUnitPurchase = iimp.Item1.ItemPurchaseInformation?.id_metricUnitPurchase;
                var factorConversionFormulationPurchase = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                                  muc.id_metricOrigin == id_metricUnitFormulation &&
                                                                                                  muc.id_metricDestiny == id_metricUnitPurchase);
                if (id_metricUnitFormulation != null && id_metricUnitFormulation == id_metricUnitPurchase)
                {
                    factorConversionFormulationPurchase = new MetricUnitConversion() { factor = 1 };
                }
                if (factorConversionFormulationPurchase == null)
                {
                    throw new Exception("Falta el Factor de Conversión entre : " + iimp.MetricUnit?.code ?? "(UM No Existe)" + ", del Ítem: " + iimp.Item1.name + " y " + iimp.Item1.ItemPurchaseInformation?.MetricUnit.code ?? "(UM No Existe)" + " configurado en el detalle de la formulación del Ítem: " + itemPP.name + ". Necesario para cargar el requerimiento de compra Configúrelo, e intente de nuevo");
                }

                var quantityUMPurchase = quantityItemIngredientMP * factorConversionFormulationPurchase.factor;

                var truncateQuantityUMPurchase = decimal.Truncate(quantityUMPurchase);
                if ((quantityUMPurchase - truncateQuantityUMPurchase) > 0)
                {
                    quantityUMPurchase = truncateQuantityUMPurchase + 1;
                };

                ProductionSchedulePurchaseRequestDetail productionSchedulePurchaseRequestDetail = productionSchedule.
                                                                                                  ProductionSchedulePurchaseRequestDetail.
                                                                                                  FirstOrDefault(fod => fod.id_item == iimp.id_ingredientItem &&
                                                                                                                        fod.id_productionScheduleProductionOrderDetail == productionScheduleProductionOrderDetail.id);
                if (productionSchedulePurchaseRequestDetail != null)
                {
                    productionSchedulePurchaseRequestDetail.quantity = !idItems.Contains(iimp.id_ingredientItem) ? quantityUMPurchase : +quantityUMPurchase;
                    //productionLotPackingMaterial.manual = false;
                    //productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                    //productionLotPackingMaterial.dateUpdate = DateTime.Now;
                }
                else
                {
                    productionSchedulePurchaseRequestDetail = new ProductionSchedulePurchaseRequestDetail
                    {
                        id = productionSchedule.ProductionSchedulePurchaseRequestDetail.Count() > 0 ? productionSchedule.ProductionSchedulePurchaseRequestDetail.Max(pld => pld.id) + 1 : 1,
                        id_item = iimp.id_ingredientItem,
                        Item = db.Item.FirstOrDefault(i => i.id == iimp.id_ingredientItem),
                        quantity = quantityUMPurchase,
                        id_productionScheduleProductionOrderDetail = productionScheduleProductionOrderDetail.id,
                        ProductionScheduleProductionOrderDetail = productionScheduleProductionOrderDetail
                        //quantity = quantityUMInventory,
                        //manual = false,
                        //isActive = true,
                        //id_userCreate = ActiveUser.id,
                        //dateCreate = DateTime.Now,
                        //id_userUpdate = ActiveUser.id,
                        //dateUpdate = DateTime.Now,
                        //ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>()
                    };
                    productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Add(productionSchedulePurchaseRequestDetail);
                    productionSchedule.ProductionSchedulePurchaseRequestDetail.Add(productionSchedulePurchaseRequestDetail);
                }
                if (!idItems.Contains(iimp.id_ingredientItem))
                    idItems.Add(iimp.id_ingredientItem);
            }

            for (int i = productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Count - 1; i >= 0; i--)
            {

                var detail = productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.ElementAt(i);

                if (!idItems.Contains(detail.id_item))
                {
                    for (int j = detail.ProductionScheduleScheduleDetail.Count - 1; j >= 0; j--)
                    {
                        var detailProductionScheduleScheduleDetail = detail.ProductionScheduleScheduleDetail.ElementAt(j);

                        for (int k = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Count - 1; k >= 0; k--)
                        {
                            var detailProductionScheduleScheduleDetailPurchaseRequestDetail = detailProductionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.ElementAt(k);
                            //detail.ProductionScheduleScheduleDetail.Remove(detailProductionScheduleScheduleDetail);
                            //detailProductionScheduleScheduleDetail.id_productionSchedulePurchaseRequestDetail = null;
                            detailProductionScheduleScheduleDetailPurchaseRequestDetail.id_productionScheduleScheduleDetail = null;
                            detailProductionScheduleScheduleDetailPurchaseRequestDetail.ProductionScheduleScheduleDetail = null;
                            //db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
                        }


                        detail.ProductionScheduleScheduleDetail.Remove(detailProductionScheduleScheduleDetail);
                        //detailProductionScheduleScheduleDetail.id_productionSchedulePurchaseRequestDetail = null;
                        //detailProductionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail = null;
                        //db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
                    }
                    //detail.ProductionScheduleScheduleDetail.ProductionSchedulePurchaseRequestDetail .Remove(detail);
                    productionScheduleProductionOrderDetail.ProductionSchedulePurchaseRequestDetail.Remove(detail);
                    productionSchedule.ProductionSchedulePurchaseRequestDetail.Remove(detail);

                }
            }

            return idItems;
        }

        private void UpdateProductionScheduleScheduleDetailPurchaseRequestDetail(ProductionScheduleScheduleDetail productionScheduleScheduleDetail)
        {
            for (int i = productionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Count - 1; i >= 0; i--)
            {
                var detail = productionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.ElementAt(i);

                if (productionScheduleScheduleDetail.id_item != detail.PurchaseRequestDetail.id_item)
                {
                    detail.id_productionScheduleScheduleDetail = null;
                    detail.ProductionScheduleScheduleDetail = null;
                }
            }
        }

        private void DeleteProductionScheduleScheduleDetailPurchaseRequestDetail(ProductionScheduleScheduleDetail productionScheduleScheduleDetail)
        {
            for (int i = productionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.Count - 1; i >= 0; i--)
            {
                var detail = productionScheduleScheduleDetail.ProductionScheduleScheduleDetailPurchaseRequestDetail.ElementAt(i);
                
                detail.id_productionScheduleScheduleDetail = null;
                detail.ProductionScheduleScheduleDetail = null;
            }
        }

        private void UpdateViewDataDataSourceProductionSchedulePurchaseRequestDetail(ProductionSchedule productionSchedule)
        {
            ViewData["dataSourceProductionSchedulePurchaseRequestDetail"] = productionSchedule.ProductionSchedulePurchaseRequestDetail.
                                                                                    Select(s => new {
                                                                                        id = s.id,
                                                                                        numberRequest = (s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail != null &&
                                                                                                         s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Count() > 0) ?
                                                                                                        (s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.FirstOrDefault().SalesRequest.Document.number) :
                                                                                                        ("Stock"),
                                                                                        itemRequest = s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.name,
                                                                                        codeRequest = (s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemSaleInformation != null &&
                                                                                                       s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemSaleInformation.MetricUnit != null) ?
                                                                                                       s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemSaleInformation.MetricUnit.code : "",
                                                                                        namePurchase = s.Item.name,
                                                                                        codePurchase = (s.Item.ItemPurchaseInformation != null && s.Item.ItemPurchaseInformation.MetricUnit != null) ? s.Item.ItemPurchaseInformation.MetricUnit.code : ""

                                                                                    }).ToList();
        }

        [HttpPost]
        public JsonResult ValidateProductionSchedulePeriod(int id_productionSchedulePeriod)
        {
            var result = new
            {
                code = 0,
                message = "Ok"
            };
            try
            {
                ProductionSchedulePeriod productionSchedulePeriod = db.ProductionSchedulePeriod.FirstOrDefault(i => i.id == id_productionSchedulePeriod);

                if (productionSchedulePeriod == null)
                {
                    result = new
                    {
                        code = -1,
                        message = "Debe Asignar un período de Planificación antes de adicionar Detalles."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);
                productionSchedule = productionSchedule ?? new ProductionSchedule();
                productionSchedule.ProductionScheduleScheduleDetail = productionSchedule.ProductionScheduleScheduleDetail ?? new List<ProductionScheduleScheduleDetail>();

                foreach (var detail in productionSchedule.ProductionScheduleScheduleDetail)
                {
                    if (DateTime.Compare(productionSchedulePeriod.dateStar.Date, detail.datePlanning.Date) > 0 || DateTime.Compare(detail.datePlanning.Date, productionSchedulePeriod.dateEnd.Date) > 0)
                    {
                        result = new
                        {
                            code = -1,
                            message = "Existen Fecha Planificadas en los detalle fuera de rango del Período"
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                TempData.Keep("productionSchedule");
            }
            catch (Exception e)
            {
                TempData.Keep("productionSchedule");
                ViewData["EditMessage"] = e.Message;
                result = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductionScheduleItemRequestDetails(int? id_itemCurrent, /*string quantityScheduleCurrent,*/ int? id_metricUnitRequestCurrent)
        {
            //Item item = db.Item.FirstOrDefault(i => i.id == id_itemCurrent);

            //if (item == null)
            //{
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}
            //decimal _quantityScheduleCurrent = Convert.ToDecimal(quantityScheduleCurrent ?? "0");

            var id_metricUnitRequest = id_metricUnitRequestCurrent;

            if(id_metricUnitRequest == null)
            {
                var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
                var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
                //var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);
                id_metricUnitRequest = id_metricUnitUMTPValueAux;
            }
            var metricUnitRequest = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitRequest);
            var metricUnits = metricUnitRequest.MetricType.MetricUnit.Where(w => w.isActive || w.id == id_metricUnitRequest);

            //decimal quantitySchedule = 0;
            //decimal quantitySale = 0;

            //if (id_itemCurrent != null)
            //{
            //    var quantityScheduleAux =
            //}
             

            var items = db.Item.Where(w=> (w.isActive && w.isSold && w.id_company == this.ActiveCompanyId) || w.id == id_itemCurrent).ToList();
            var result = new
            {
                items = items.Select(s=> new { id = s.id,
                                               masterCode = s.masterCode,
                                               name = s.name,
                                               itemSaleInformationMetricUnitCode = s.ItemSaleInformation?.MetricUnit?.code ?? ""
                }),
                metricUnits = metricUnits.Select(s => new {
                    id =  s.id,
                    code = s.code,
                }),
                id_metricUnitRequest = id_metricUnitRequest
                //quantitySchedule = quantitySchedule,
                //quantitySale = quantitySale
            };

            TempData.Keep("productionSchedule");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemRequestDetailData(int? id_itemRequest, string quantitySchedule, int? id_metricUnitRequest)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_itemRequest);
            decimal quantitySale = 0;
            string metricUnitSale = "";
            string msgErrorConversion = "";
            decimal _quantitySchedule = Convert.ToDecimal(quantitySchedule ?? "0");

            if (item != null)
            {
                metricUnitSale = item.ItemSaleInformation?.MetricUnit?.code ?? "";

                var metricUnitRequest = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitRequest);

                if (metricUnitRequest != null)
                {
                    var id_metricUnitPresentation = item.Presentation?.id_metricUnit;
                    var factorConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                         muc.id_metricOrigin == metricUnitRequest.id &&
                                                                                         muc.id_metricDestiny == id_metricUnitPresentation);
                    if (id_metricUnitPresentation != null && id_metricUnitPresentation == metricUnitRequest.id)
                    {
                        factorConversion = new MetricUnitConversion() { factor = 1 };
                    }
                    if (factorConversion == null)
                    {
                        //var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
                        msgErrorConversion = ("Falta el Factor de Conversión entre : " + (metricUnitRequest.code) + " y " + (item.Presentation?.MetricUnit?.code ?? "(UM de Presentación No Existe)") + ". Necesario para calcular las cantidades en el requerimiento Configúrelo, e intente de nuevo");
                    }else
                    {
                        var quantityAux = _quantitySchedule * factorConversion.factor;
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
                                                                                             muc.id_metricDestiny == metricUnitRequest.id);
                        if (id_metricUnitPresentation != null && id_metricUnitPresentation == metricUnitRequest.id)
                        {
                            factorConversion = new MetricUnitConversion() { factor = 1 };
                        }
                        if (factorConversion == null)
                        {
                            //var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
                            msgErrorConversion = ("Falta el Factor de Conversión entre : " + (item.Presentation?.MetricUnit?.code ?? "(UM de Presentación No Existe)") + " y " + (metricUnitRequest.code) + ". Necesario para calcular las cantidades en el requerimiento Configúrelo, e intente de nuevo");
                        }else
                        {
                            quantityAux = (quantitySale * (item.Presentation?.minimum ?? 1)) * factorConversion.factor;

                            _quantitySchedule = quantityAux;
                        }
                    }
                }
                //return Json(null, JsonRequestBehavior.AllowGet);
            }

            //var id_metricUnitRequest = id_metricUnitRequestCurrent;

            //if (id_metricUnitRequest == null)
            //{
            //    var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            //    var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            //    //var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);
            //    id_metricUnitRequest = id_metricUnitUMTPValueAux;
            //}
            //var metricUnits = metricUnitRequest.MetricType.MetricUnit.Where(w => w.isActive || w.id == id_metricUnitRequest);

            //decimal quantitySchedule = 0;
            //decimal quantitySale = 0;

            //if (id_itemCurrent != null)
            //{
            //    var quantityScheduleAux =
            //}


            //var items = db.Item.Where(w => (w.isActive && w.isSold && w.id_company == this.ActiveCompanyId) || w.id == id_itemCurrent).ToList();
            var result = new
            {
                quantitySale = quantitySale,
                metricUnitSale = metricUnitSale,
                quantitySchedule = _quantitySchedule,
                msgErrorConversion = msgErrorConversion
            };

            TempData.Keep("productionSchedule");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItemRequestDetail(int? id_itemNew, int? id_saleRequestDetail)
        {
            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);

            productionSchedule = productionSchedule ?? new ProductionSchedule();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };


            var saleRequestDetailAux = productionSchedule.ProductionScheduleRequestDetail.Where(w => w.id_item == id_itemNew);
            foreach (var detail in saleRequestDetailAux)
            {
                if (detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail == null || detail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Count <= 0)
                {
                    var itemNewAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                    //var providerAux = db.Provider.FirstOrDefault(fod => fod.id == id_proposedProviderNew);
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede repetir el Ítem: " + itemNewAux.name +
                                ",  sin requerimiento de pedido asignado,  en los detalles."

                    };

                }
            }




            TempData["productionSchedule"] = productionSchedule;
            TempData.Keep("productionSchedule");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ValidateDatePlanning(string datePlanning, int? id_productionSchedulePeriod)
        {
            var result = new
            {
                code = 0,
                message = "Ok"
            };
            try
            {
                ProductionSchedulePeriod productionSchedulePeriod = db.ProductionSchedulePeriod.FirstOrDefault(i => i.id == id_productionSchedulePeriod);

                if (productionSchedulePeriod == null)
                {
                    result = new
                    {
                        code = -1,
                        message = "Debe Asignar un período de Planificación antes de adicionar Detalles."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                if (datePlanning != null)
                {
                    DateTime datePlanningAux = DateTime.Parse(datePlanning, new CultureInfo("es-EC"));
                    if (DateTime.Compare(productionSchedulePeriod.dateStar.Date, datePlanningAux.Date) <= 0 && DateTime.Compare(datePlanningAux.Date, productionSchedulePeriod.dateEnd.Date) <= 0)
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = new
                        {
                            code = -1,
                            message = "Fuera del rango del Período de Planificación: " + productionSchedulePeriod.name
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                }

                TempData.Keep("productionSchedule");


            }
            catch (Exception e)
            {
                TempData.Keep("productionSchedule");
                ViewData["EditMessage"] = e.Message;
                result = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductionScheduleItemPurchaseRequestDetails(int? id_productionSchedulePurchaseRequestDetail, int? id_provider, int? id_buyer)
        {
            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);

            productionSchedule = productionSchedule ?? new ProductionSchedule();

            var productionSchedulePurchaseRequestDetails = productionSchedule.ProductionSchedulePurchaseRequestDetail.Where(w => w.ProductionScheduleScheduleDetail.Sum(s => s.quantity) < w.quantity || w.id == id_productionSchedulePurchaseRequestDetail).ToList();

            var productionSchedulePurchaseRequestDetail = productionSchedulePurchaseRequestDetails.FirstOrDefault(fod => fod.id == id_productionSchedulePurchaseRequestDetail);

            var providers = db.Provider.Where(g => ((g.Person.isActive && g.Person.id_company == this.ActiveCompanyId) || g.id == id_provider)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();

            var buyers =  db.Person.Where(g => (g.isActive && g.id_company == this.ActiveCompanyId && g.Rol.Any(a => a.name == "Comprador")) || g.id == id_buyer).Select(p => new { p.id, p.fullname_businessName }).ToList();

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            decimal outstandingAmountRequired = productionSchedulePurchaseRequestDetail?.quantity ?? 0;
            outstandingAmountRequired -= productionSchedulePurchaseRequestDetail?.ProductionScheduleScheduleDetail.Sum(s => s.quantity) ?? 0;

            var metricUnitAux = productionSchedulePurchaseRequestDetail?.Item.ItemPurchaseInformation?.MetricUnit?.code ?? (/*metricUnitUMTP?.code ?? */"");

            var result = new
            {
                productionSchedulePurchaseRequestDetails = productionSchedulePurchaseRequestDetails.
                                                           Select(s => new {
                                                               id = s.id,
                                                               numberRequest = (s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail != null &&
                                                                                                s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.Count() > 0) ?
                                                                                            (s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.FirstOrDefault().SalesRequest.Document.number) :
                                                                                            ("Stock"),
                                                               itemRequest = s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.name,
                                                               codeRequest = (s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemSaleInformation != null &&
                                                                                            s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemSaleInformation.MetricUnit != null) ?
                                                                                            s.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemSaleInformation.MetricUnit.code : "",
                                                               namePurchase = s.Item.name,
                                                               codePurchase = (s.Item.ItemPurchaseInformation != null && s.Item.ItemPurchaseInformation.MetricUnit != null) ? s.Item.ItemPurchaseInformation.MetricUnit.code : ""

                                                           }).ToList(),
                salesPurchaseRequest = productionSchedulePurchaseRequestDetail?.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail?.FirstOrDefault()?.SalesRequest.Document.number ?? "Stock",
                outstandingAmountRequired = outstandingAmountRequired > 0 ? outstandingAmountRequired : 0,
                providers = providers,
                buyers = buyers,
                metricUnitPurchaseRequest = metricUnitAux,
                metricUnitScheduleRequest = metricUnitAux
            };

            TempData.Keep("productionSchedule");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductionScheduleItemRequestDetailData(int? id_productionSchedulePurchaseRequestDetail)
        {
            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);

            productionSchedule = productionSchedule ?? new ProductionSchedule();

            var productionSchedulePurchaseRequestDetails = productionSchedule.ProductionSchedulePurchaseRequestDetail.ToList();

            var productionSchedulePurchaseRequestDetail = productionSchedulePurchaseRequestDetails.FirstOrDefault(fod => fod.id == id_productionSchedulePurchaseRequestDetail);

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            decimal outstandingAmountRequired = productionSchedulePurchaseRequestDetail?.quantity ?? 0;
            outstandingAmountRequired -= productionSchedulePurchaseRequestDetail?.ProductionScheduleScheduleDetail.Sum(s => s.quantity) ?? 0;

            var metricUnitAux = productionSchedulePurchaseRequestDetail?.Item.ItemPurchaseInformation?.MetricUnit?.code ?? (metricUnitUMTP?.code ?? "");

            var result = new
            {
                salesPurchaseRequest = productionSchedulePurchaseRequestDetail?.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail?.FirstOrDefault()?.SalesRequest.Document.number ?? "Stock",
                outstandingAmountRequired = outstandingAmountRequired > 0 ? outstandingAmountRequired : 0,
                metricUnitPurchaseRequest = metricUnitAux,
                metricUnitScheduleRequest = metricUnitAux
            };

            TempData.Keep("productionSchedule");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedProductionSchedulePurchaseRequestDetail(string datePlanningNew, int? id_providerNew, int? id_buyerNew, int? id_productionSchedulePurchaseRequestDetailNew)
        {
            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);

            productionSchedule = productionSchedule ?? new ProductionSchedule();

            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };


            var productionScheduleScheduleDetailAux = productionSchedule.ProductionScheduleScheduleDetail.Where(w => w.id_productionSchedulePurchaseRequestDetail == id_productionSchedulePurchaseRequestDetailNew &&
                                                                                                      w.id_provider == id_providerNew && w.id_buyer == id_buyerNew);
            foreach (var detail in productionScheduleScheduleDetailAux)
            {
                DateTime datePlanningAux = DateTime.Parse(datePlanningNew, new CultureInfo("es-EC"));
                if (DateTime.Compare(detail.datePlanning.Date, datePlanningAux.Date) == 0)
                {
                    var itemNewAux = db.Item.FirstOrDefault(fod => fod.id == detail.id_item);
                    var datePlanningNewAux = detail.datePlanning.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() +
                                             detail.datePlanning.ToString("_dd");
                    var providerNewAux = db.Provider.FirstOrDefault(fod => fod.id == detail.id_provider);
                    var buyerNewAux = db.Person.FirstOrDefault(fod => fod.id == detail.id_buyer);
                    var productionSchedulePurchaseRequestDetailNewAux = detail.ProductionSchedulePurchaseRequestDetail.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail?.FirstOrDefault()?.SalesRequest.Document.number ?? "Stock";
                    //var providerAux = db.Provider.FirstOrDefault(fod => fod.id == id_proposedProviderNew);
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede repetir el Ítem: " + itemNewAux.name + "(" + productionSchedulePurchaseRequestDetailNewAux + ")" +
                                ",  en la Fecha de Planificación: " + datePlanningNewAux + 
                                ", para el Proveedor: " + (providerNewAux?.Person.fullname_businessName ?? ("(Sin Proveedor)")) +
                                ", el comprador: " + (buyerNewAux?.fullname_businessName ?? ("(Sin Comprador)")) + 
                                ", en los detalles."

                    };

                }
            }




            TempData["productionSchedule"] = productionSchedule;
            TempData.Keep("productionSchedule");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ProductionSchedulePeriodDetailDate(int? id_productionSchedulePeriod)
        {
            ProductionSchedulePeriod productionSchedulePeriod = db.ProductionSchedulePeriod.FirstOrDefault(i => i.id == id_productionSchedulePeriod);

            //if (item == null)
            //{
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}

            var result = new
            {
                dateStarProductionSchedulePeriod = productionSchedulePeriod != null ? DateTime.Now.ToString("dd/MM/yyyy") : "",//productionSchedulePeriod?.dateStar.ToString("dd/MM/yyyy") ?? "",
                dateEndProductionSchedulePeriod = productionSchedulePeriod?.dateEnd.ToString("dd/MM/yyyy") ?? ""
            };

            TempData.Keep("productionSchedule");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionSchedulesAllSchedules()
        {
            ProductionSchedule productionSchedule = (TempData["productionSchedule"] as ProductionSchedule);

            productionSchedule = productionSchedule ?? new ProductionSchedule();

            var result = new
            {
                itsAllSchedules = 1,
                Error = ""

            };

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            foreach (var detail in productionSchedule.ProductionSchedulePurchaseRequestDetail)
            {
                var quantityScheduleAux = detail.ProductionScheduleScheduleDetail.Count() > 0 ? detail.ProductionScheduleScheduleDetail.Sum(s => s.quantity) : 0;
                if (detail.quantity > quantityScheduleAux)
                {
                    //var itemNewAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                    //var providerAux = db.Provider.FirstOrDefault(fod => fod.id == id_proposedProviderNew);
                    if(result.itsAllSchedules == 0)
                    {
                        result = new
                        {
                            itsAllSchedules = 0,
                            Error = result.Error + "</br>- Falta: " +
                                 (detail.quantity - quantityScheduleAux).ToString("N2") + (detail.Item.ItemPurchaseInformation?.MetricUnit?.code ?? (metricUnitUMTP?.code ?? "")) +
                                " de " + detail.Item.name + " requerido por los " + detail.ProductionScheduleProductionOrderDetail.quantityProductionOrder +
                                (detail.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemSaleInformation?.MetricUnit?.code ?? "Un") + " del Ítem " +
                                detail.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.name + " con No. Requerimiento de Pedido " +
                                (detail.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail?.FirstOrDefault()?.SalesRequest.Document.number ?? "(Sin requerimiento de pedido asignado)") + ",  en los detalles de la Planificación."

                        };
                    }else
                    {
                        result = new
                        {
                            itsAllSchedules = 0,
                            Error = "No se puede aprobar la Programación debido a que: </br>- Falta: " +
                                                         (detail.quantity - quantityScheduleAux).ToString("N2") + (detail.Item.ItemPurchaseInformation?.MetricUnit?.code ?? (metricUnitUMTP?.code ?? "")) +
                                                        " de " + detail.Item.name + " requerido por los " + detail.ProductionScheduleProductionOrderDetail.quantityProductionOrder +
                                                        (detail.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.ItemSaleInformation?.MetricUnit?.code ?? "Un") + " del Ítem " +
                                                        detail.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.Item.name + " con No. Requerimiento de Pedido " +
                                                        (detail.ProductionScheduleProductionOrderDetail.ProductionScheduleRequestDetail.SalesRequestOrQuotationDetailProductionScheduleRequestDetail?.FirstOrDefault()?.SalesRequest.Document.number ?? "(Sin requerimiento de pedido asignado)") + ",  en los detalles de la Planificación."

                        };
                    }
                    

                }
            }




            TempData["productionSchedule"] = productionSchedule;
            TempData.Keep("productionSchedule");

            return Json(result, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public JsonResult ValidateSelectedRowsSalesRequestDetail(int[] ids)
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

                if (inventoryLineCurrent != inventoryLineFirst && (inventoryLineFirst?.code == "MP" || inventoryLineCurrent?.code == "MP")) //"MP" : Materia Prima
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar Ítem de MATERIA PRIMA con Ítem de MATERIALES/INSUMOS")
                    };
                    TempData.Keep("purchaseOrder");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                if (providerCurrent != providerFirst)
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
            return Json(result, JsonRequestBehavior.AllowGet);
        }

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

        //public class tempDatePlanningId_purchasePlanningPeriod
        //{
        //    public string datePlanning { get; set; }
        //    public int id_purchasePlanningPeriod { get; set; }

        //}

        

        
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