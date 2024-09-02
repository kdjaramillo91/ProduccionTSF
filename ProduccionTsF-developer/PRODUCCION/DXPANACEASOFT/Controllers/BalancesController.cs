using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using DXPANACEASOFT.Services;
using DXPANACEASOFT.Reports.Balances;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class BalancesController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView("Index");
        }

        #region Balances RESULTS

        private InventoryMoveDetail GetInventoryMoveDetailLessEqualOf(DateTime cutoffDate, InventoryMoveDetail inventoryMoveDetailCurrent)
        {
            return null;
        }

        [HttpPost]
        public ActionResult BalancesResults(InventoryMove inventoryMove,
                                          InventoryEntryMove entryMove,
                                          InventoryExitMove exitMove,
                                          Document document,
                                          DateTime? cutoffDate,
                                          string numberLot, string internalNumberLot, int[] items)
        {
            var model = db.InventoryMoveDetail.Where(w=> w.InventoryMove.Document.DocumentState.code.Equals("03") &&//APROBADA
                                                         w.id_inventoryMoveDetailNext == null).ToList();
            var tempModel = new List<InventoryMoveDetail>();
            foreach (var inventoryMoveDetail in model)
            {
                var inventoryMoveDetailAux = GetInventoryMoveDetailLessEqualOf(cutoffDate ?? DateTime.Now, inventoryMoveDetail);
                if (inventoryMoveDetailAux != null)
                {
                    tempModel.Add(inventoryMoveDetail);
                }
            }

            model = tempModel;

            #region DOCUMENT FILTERS

            if (document.id_documentType!= 0)
            {
                model = model.Where(o => o.InventoryMove.Document.id_documentType == document.id_documentType).ToList();
            }

            //if (document.id_documentState != 0)
            //{
            //    model = model.Where(o => o.InventoryMove.Document.id_documentState == document.id_documentState).ToList();
            //}

            if (!string.IsNullOrEmpty(document.number))
            {
                model = model.Where(o => o.InventoryMove.Document.number.Contains(document.number)).ToList();
            }

            if (!string.IsNullOrEmpty(document.reference))
            {
                model = model.Where(o => o.InventoryMove.Document.reference.Contains(document.reference)).ToList();
            }

            //if (startEmissionDate != null && endEmissionDate != null)
            //{
            //    model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.InventoryMove.Document.emissionDate.Date) <= 0 && DateTime.Compare(o.InventoryMove.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            //}

            #endregion

            #region INVENTORY ENTRY MOVE

            if (inventoryMove.id_inventoryReason != 0 && inventoryMove.id_inventoryReason != null)
            {
                model = model.Where(o => o.InventoryMove.id_inventoryReason == inventoryMove.id_inventoryReason).ToList();
            }

            # region InventoryExitMove
            if (exitMove.id_warehouseExit != 0)
            {
                model = model.Where(o => o.InventoryMove.InventoryExitMove?.id_warehouseExit == exitMove.id_warehouseExit).ToList();
            }

            if (exitMove.id_warehouseLocationExit != 0)
            {
                model = model.Where(o => o.InventoryMove.InventoryExitMove?.id_warehouseLocationExit == exitMove.id_warehouseLocationExit).ToList();
            }

            if (exitMove.id_dispatcher != 0)
            {
                model = model.Where(o => o.InventoryMove.InventoryExitMove?.id_dispatcher == exitMove.id_dispatcher).ToList();
            }
            #endregion

            # region InventoryEntryMove
            if (entryMove.id_warehouseEntry != 0)
            {
                model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_warehouseEntry == entryMove.id_warehouseEntry).ToList();
            }

            if (entryMove.id_warehouseLocationEntry != 0)
            {
                model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_warehouseLocationEntry == entryMove.id_warehouseLocationEntry).ToList();
            }

            if (entryMove.id_receiver != 0)
            {
                model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_receiver == entryMove.id_receiver).ToList();
            }
            #endregion

            #endregion

            #region INVENTORY MOVE DETAIL

            if (!string.IsNullOrEmpty(numberLot))
            {
                model = model.Where(o => o.Lot?.number.Contains(numberLot) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(internalNumberLot))
            {
                model = model.Where(o => o.Lot?.ProductionLot?.internalNumber.Contains(internalNumberLot) ?? false).ToList();
            }

            if (items != null && items.Length > 0)
            {
                tempModel = new List<InventoryMoveDetail>();
                foreach (var inventoryMoveDetail in model)
                {
                    if (items.Contains(inventoryMoveDetail.id_item))
                    {
                        tempModel.Add(inventoryMoveDetail);
                    }
                }

                model = tempModel;
            }

            #endregion

            var modelAux = model.Select(s => new ResultKardex
            {
                id = s.id,
                id_document = s.id_inventoryMove,
                document = s.InventoryMove.Document.number,
                id_documentType = s.InventoryMove.Document.id_documentType,
                documentType = s.InventoryMove.Document.DocumentType.name,
                id_inventoryReason = s.InventoryMove.id_inventoryReason,
                inventoryReason = s.InventoryMove.InventoryReason?.name ?? "",
                emissionDate = s.InventoryMove.Document.emissionDate,
                id_item = s.id_item,
                code_item = s.Item.masterCode + " - " + s.Item.name,
                id_metricUnit = s.Item.ItemPurchaseInformation?.id_metricUnitPurchase ?? 0,
                metricUnit = s.Item.ItemPurchaseInformation?.MetricUnit?.code ?? "",
                id_lot = s.id_lot,
                number = s.Lot?.number ?? "",
                internalNumber = s.Lot?.ProductionLot?.internalNumber ?? "",
                id_warehouse = s.id_warehouse,
                warehouse = s.Warehouse.name,
                id_warehouseLocation = s.id_warehouseLocation,
                warehouseLocation = s.WarehouseLocation.name,
                id_warehouseExit = (s.exitAmount > 0)? s.id_warehouse : (s.InventoryMoveDetail2?.id_warehouse),
                warehouseExit = (s.exitAmount > 0)? s.Warehouse.name : (s.InventoryMoveDetail2?.Warehouse.name ?? ""),
                id_warehouseLocationExit = (s.exitAmount > 0) ? s.id_warehouseLocation : (s.InventoryMoveDetail2?.id_warehouseLocation),
                warehouseLocationExit = (s.exitAmount > 0) ? s.WarehouseLocation.name : (s.InventoryMoveDetail2?.WarehouseLocation.name ?? ""),
                id_warehouseEntry = (s.entryAmount > 0) ? s.id_warehouse : (s.id_warehouseEntry),
                warehouseEntry = (s.entryAmount > 0) ? s.Warehouse.name : (s.Warehouse1?.name ?? ""),
                id_warehouseLocationEntry = (s.entryAmount > 0) ? s.id_warehouseLocation : ((int?)null),
                warehouseLocationEntry = (s.entryAmount > 0) ? s.WarehouseLocation.name : (""),
                priceCost = s.unitPrice,
                previousBalance = s.balance - s.entryAmount + s.exitAmount,
                previousBalanceCost = s.InventoryMoveDetail3?.balanceCost ?? (decimal)0,
                entry = s.entryAmount,
                entryCost = s.entryAmountCost,
                exit = s.exitAmount,
                exitCost = s.exitAmountCost,
                balance = s.balance,
                balanceCost = s.balanceCost
            }).ToList();
            TempData["model"] = modelAux;
            TempData.Keep("model");
            //TempData.Keep("documentTypeCodeCurrent");

            return PartialView("_BalancesResults", modelAux.OrderByDescending(i => i.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult BalancesPartial()
        {
            var model = TempData["model"] as List<ResultKardex>;
            model = model ?? new List<ResultKardex>();

            TempData.Keep("model");

            return PartialView("_BalancesPartial", model.OrderByDescending(i => i.id).ToList());

        }

        #endregion

        #region REPORTS

        [HttpPost]
        public ActionResult BalancesReport()
        {
            var model = TempData["model"] as List<ResultKardex>;
            model = model ?? new List<ResultKardex>();

            BalancesReport BalancesReport = new BalancesReport();
            
            Company company = db.Company.FirstOrDefault(c => c.id == this.ActiveCompanyId);
            BalancesReport.DataSource = new KardexCompany
            {
                list_id_kardex = new List<int>(1),
                listResultKardex = model.OrderByDescending(i => i.id).ToList(),
                company = company
            };
            TempData.Keep("model");
            return PartialView("_BalancesReport", BalancesReport);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult InventoryMoveDetails()
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? new InventoryMove();
            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();

            TempData.Keep("inventoryMove");

            return Json(inventoryMove.InventoryMoveDetail.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult WarehouseChangeData(int id_warehouse)
        {
            Warehouse warehouse = db.Warehouse.FirstOrDefault(i => i.id == id_warehouse);

            if (warehouse == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);


            if (inventoryMove?.InventoryMoveDetail != null)
            {
                var itemDetail = inventoryMove.InventoryMoveDetail.ToList();

                foreach (var i in itemDetail)
                {
                    i.id_warehouse = id_warehouse;
                    UpdateModel(i);
                }
            }

            var result = new
            {
               id_warehouse,
               warehouse.name
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult WarehouseLocationChangeData(int id_warehouseLocation)
        {
            WarehouseLocation warehouseLocation = db.WarehouseLocation.FirstOrDefault(i => i.id == id_warehouseLocation);

            if (warehouseLocation == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);


            if (inventoryMove?.InventoryMoveDetail != null)
            {
                var itemDetail = inventoryMove.InventoryMoveDetail.ToList();

                foreach (var i in itemDetail)
                {
                    i.id_warehouseLocation = id_warehouseLocation;
                    UpdateModel(i);
                }
            }

            var result = new
            {
                id_warehouseLocation,
                warehouseLocation.name
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}