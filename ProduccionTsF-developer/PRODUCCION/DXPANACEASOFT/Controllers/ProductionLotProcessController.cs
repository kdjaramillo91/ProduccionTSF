using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc;
using DocumentFormat.OpenXml.Drawing;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.Models.InventoryBalance;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using SpreadsheetLight;
using SpreadsheetLight.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utilitarios.Logs;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static DXPANACEASOFT.Services.ServiceInventoryBalance;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ProductionLotProcessController : DefaultController
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

            return PartialView();
        }

        #region PRODUCTION LOTE PROCESS EDITFORM

        [ValidateInput(false)]
        public ActionResult ProductionLotProcessFormEditPartial(int id, int[] ids_productionOrdersDetails, string[] arrayTempDataKeep, bool? toReturn = null, int? tabSelected = null)
        {
            UpdateArrayTempDataKeep(arrayTempDataKeep);
            
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            ProductionLot productionLot = new ProductionLot();

            ViewData["tabSelected"] = tabSelected;

            if (toReturn == true)
            {
                productionLot = (TempData["productionLotProcess"] as ProductionLot);
            }
            else
            {
                productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            }
            var requestliquidationmachine = productionLot != null
                        ? productionLot.ProductionProcess?.requestliquidationmachine ?? false
                        : false;

            var generateTransfer = productionLot != null
                        ? productionLot.ProductionProcess?.generateTransfer ?? false
                        : false;

            if (solicitaMaquina == "SI" && requestliquidationmachine)
            {
                ViewBag.solicitaMaquina = true;
            }
            else
            {
                ViewBag.solicitaMaquina = false;
            }
            if (solicitaMaquina == "SI" && generateTransfer)
            {
                ViewBag.generaTransferencia = true;
            }
            else
            {
                ViewBag.generaTransferencia = false;
            }
            int inventoryMoveEntry = 0;
            int inventoryMoveExit = 0;
            if (productionLot == null)
            {
                ProductionProcess process = new ProductionProcess();

                ProductionLotState state = db.ProductionLotState.FirstOrDefault(s => s.code.Equals("01"));

                DateTime receptionDate = DateTime.Now;

                var DCLP = db.Setting.FirstOrDefault(fod => fod.code == "DCLP" && fod.id_company == this.ActiveCompanyId)?.value ?? "0";
                var int_DCLP = int.Parse(DCLP);
                DateTime expirationDate = DateTime.Now.AddDays(int_DCLP);

                Employee employee = ActiveUser.Employee;

                productionLot = new ProductionLot
                {
                    id_ProductionLotState = state?.id ?? 0,
                    ProductionLotState = state,

                    id_productionProcess = process?.id ?? 0,
                    ProductionProcess = process,
                    receptionDate = receptionDate,
                    expirationDate = expirationDate,
                    id_personRequesting = employee?.id ?? 0,
                    Employee = employee,
                    id_personReceiving = employee?.id ?? 0,
                    Employee1 = employee
                };
                productionLot.julianoNumber = DataProviderJulianoNumber.GetJulianoNumber(receptionDate);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(productionLot.internalNumber))
                {
                    if (productionLot.internalNumber.Trim().Length > 5)
                    {
                        productionLot.julianoNumber = productionLot.internalNumber.Trim().Substring(0, 5);
                        productionLot.internalNumber = productionLot.internalNumber.Trim().Substring(5);
                    }
                    else
                    {
                        productionLot.julianoNumber = productionLot.internalNumber;
                        productionLot.internalNumber = "";
                    }
                }

                productionLot.internalNumberConcatenated = (productionLot.julianoNumber ?? string.Empty) + (productionLot.internalNumber ?? string.Empty);

                var stateLot = productionLot.ProductionLotState.code;
                ViewBag.codeStateLiqNoval = stateLot;
                if (stateLot != "01")
                {
                    var natureMoves = db.AdvanceParametersDetail.Where(r => r.valueCode.Trim() == "I" || r.valueCode.Trim() == "E").ToList();
                    var inventoryMoves = db.InventoryMove.Where(r => r.id_productionLot == productionLot.id && r.Document.DocumentState.code != "05").ToList();
                    var natureMoveEgreso = natureMoves.FirstOrDefault(r => r.valueCode.Trim() == "E");
                    inventoryMoveExit = inventoryMoves.FirstOrDefault(r => r.idNatureMove == natureMoveEgreso.id)?.id ?? 0;
                    if (stateLot != "02" && stateLot != "03")
                    {
                        var natureMoveIngreso = natureMoves.FirstOrDefault(r => r.valueCode.Trim() == "I");
                        inventoryMoveEntry = inventoryMoves.FirstOrDefault(r => r.idNatureMove == natureMoveIngreso.id)?.id ?? 0;
                    }
                }
            }

            UpdateProductionLotTotals(productionLot);
            ViewBag.imoveEntry = inventoryMoveEntry;
            ViewBag.imovExit = inventoryMoveExit;
            ViewBag.isProductionProcess = "S";
            ViewBag.id_MachineProdOpeningDetailInit = db.MachineProdOpeningDetail
                .FirstOrDefault(fod => fod.id_MachineForProd == productionLot.id_MachineForProd
                                    && fod.id_MachineProdOpening == productionLot.id_MachineProdOpening)?
                .MachineForProd?.id;

            #region Liquiacion Valorizada - 6115
            string setting_liquidacionValorizada = (db.Setting.FirstOrDefault(r=> r.code == "LIQNOVAL")?.value??"NO");
            bool isRequestCarMachine = false;
            if (setting_liquidacionValorizada == "SI")
            {                
                isRequestCarMachine = (ProductionProcess.GetOneById(productionLot.id_productionProcess)?.requestCarMachine?? false) ;
            }
            ViewBag.isRequestCarMachine = isRequestCarMachine;

            if (isRequestCarMachine) 
            {
                ProductionLotMachineTurn productionLotMachineTurn = ProductionLotMachineTurn.GetOneByProductionLot(productionLot.id);
                
                int idMachineForProd = (productionLotMachineTurn?.idMachineForProd ?? 0);
                int id_MachineProdOpening = (productionLotMachineTurn?.idMachineProdOpening ?? 0);
                ViewBag.id_MachineProdOpeningDetailInitLiqNoVal = db.MachineProdOpeningDetail
                                                                        .FirstOrDefault(fod => fod.id_MachineForProd == idMachineForProd
                                                                                                && fod.id_MachineProdOpening == id_MachineProdOpening)?
                                                                        .MachineForProd?.id;


                ViewBag.productionLotMachineTurn = (productionLotMachineTurn??new ProductionLotMachineTurn());
            }
            #endregion
            BuildViewDataEdit();
            TempData["fechaProceso"] = productionLot.receptionDate;
            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");
            ViewData["id_productionLot"] = productionLot.id;
            if (arrayTempDataKeep != null)
            {
                ViewData["ModelLink"] = productionLot;
                return PartialView("LinkBoxTemplates/_LinkBox", "_FormEditProductionLotProcess");
            }
            else
            {
                return PartialView("_FormEditProductionLotProcess", productionLot);
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetTimesTurn(int? id_turn)
        {
            var turn = db.Turn.FirstOrDefault(fod => fod.id == id_turn);

            var result = new
            {
                Message = "Ok",
                timeInitTurn = turn?.timeInit,
                timeEndTurn = turn?.timeEnd
            };

            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetProductionLotWarehouseLocation(int? id_warehouse, int? id_warehouseLocation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var warehouseLocationAux = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse).ToList();
            var warehouseLocationCurrentAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocation);
            if (warehouseLocationCurrentAux != null && !warehouseLocationAux.Contains(warehouseLocationCurrentAux)) warehouseLocationAux.Add(warehouseLocationCurrentAux);

            var result = new
            {
                warehouseLocations = warehouseLocationAux
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       })
            };

            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductionLotProcessCopy(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

            ProductionLot productionLotCopy = null;
            if (productionLot != null)
            {
                ProductionLotState state = db.ProductionLotState.FirstOrDefault(s => s.code.Equals("01"));
                ProductionProcess process = db.ProductionProcess.FirstOrDefault(p => p.id == productionLot.id_productionProcess);
                ProductionUnit productionUnit = db.ProductionUnit.FirstOrDefault(p => p.id == productionLot.id_productionUnit);

                productionLotCopy = new ProductionLot
                {
                    id_ProductionLotState = state?.id ?? 0,
                    id_productionProcess = process?.id ?? 0,
                    id_productionUnit = productionUnit?.id ?? 0,
                    ProductionUnit = productionUnit,
                    id_personRequesting = productionLot.id_personRequesting,
                    Employee = productionLot.Employee,
                    Employee1 = productionLot.Employee1,
                    receptionDate = DateTime.Now
                };

                if (productionLot.ProductionLotDetail != null)
                {
                    productionLotCopy.ProductionLotDetail = new List<ProductionLotDetail>();
                    foreach (var detail in productionLot.ProductionLotDetail.AsEnumerable())
                    {
                        productionLotCopy.ProductionLotDetail.Add(new ProductionLotDetail
                        {
                            id_item = detail.id_item,
                            id_warehouse = detail.id_warehouse,
                            id_warehouseLocation = detail.id_warehouseLocation,
                            quantityOrdered = detail.quantityOrdered,
                            quantityRecived = detail.quantityRecived,
                            quantityRemitted = detail.quantityRemitted,
                            quantityProcessed = detail.quantityProcessed
                        });
                    }
                }

                if (productionLot.ProductionLotLiquidation != null)
                {
                    productionLotCopy.ProductionLotLiquidation = new List<ProductionLotLiquidation>();
                    foreach (var detail in productionLot.ProductionLotLiquidation)
                    {
                        productionLotCopy.ProductionLotLiquidation.Add(new ProductionLotLiquidation
                        {
                            id_item = detail.id_item,
                            id_warehouse = detail.id_warehouse,
                            id_warehouseLocation = detail.id_warehouseLocation,
                            quantity = detail.quantity
                        });
                    }
                }

                if (productionLot.ProductionLotTrash != null)
                {
                    productionLotCopy.ProductionLotTrash = new List<ProductionLotTrash>();
                    foreach (var detail in productionLot.ProductionLotTrash)
                    {
                        productionLotCopy.ProductionLotTrash.Add(new ProductionLotTrash
                        {
                            id_item = detail.id_item,
                            id_warehouse = detail.id_warehouse,
                            id_warehouseLocation = detail.id_warehouseLocation,
                        });
                    }
                }

                if (productionLot.ProductionLotLoss != null)
                {
                    productionLotCopy.ProductionLotLoss = new List<ProductionLotLoss>();
                    foreach (var detail in productionLot.ProductionLotLoss)
                    {
                        productionLotCopy.ProductionLotLoss.Add(new ProductionLotLoss
                        {
                            id_item = detail.id_item,
                            id_warehouse = detail.id_warehouse,
                            id_warehouseLocation = detail.id_warehouseLocation,
                        });
                    }
                }

                UpdateProductionLotTotals(productionLotCopy);
            }

            TempData["productionLotProcess"] = productionLotCopy;
            TempData.Keep("productionLotProcess");

            return PartialView("_FormEditProductionLotProcess", productionLotCopy);
        }

        #endregion PRODUCTION LOTE PROCESS EDITFORM

        #region ITEMS

        [ValidateInput(false)]
        public ActionResult ProductionLotProcessDetailItemsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(i => i.id == id_productionLot);
            productionLot = productionLot ?? (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            ViewData["id_productionLot"] = id_productionLot;
            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLot;
            TempData.Keep("productionLotProcess");

            return PartialView("_ProductionLotProcessDetailItemsPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormItemsDetailPartial(DateTime? fechaProceso)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            TempData["fechaProceso"] = fechaProceso;
            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLot;
            TempData.Keep("productionLotProcess");

            return PartialView("_ProductionLotProcessEditFormItemsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormItemsDetailAddNew(ProductionLotDetail item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    ProductionProcess proceso = db.ProductionProcess.FirstOrDefault(r => r.id == productionLot.id_productionProcess);
                    if (proceso != null)
                    {
                        item.id_warehouse = (proceso.id_warehouse ?? 0);
                        item.id_warehouseLocation = (proceso.id_WarehouseLocation ?? 0);
                    }

                    var productionLotDetailAux = productionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id_item == item.id_item &&
                                                                                fod.id_originLot == item.id_originLot &&
                                                                                fod.lotMarked == item.lotMarked);

                    if (productionLotDetailAux != null)
                    {
                        var itemAux = db.Item.FirstOrDefault(fod => fod.id == item.id_item);
                        var productionLotAux = db.ProductionLot.FirstOrDefault(fod => fod.id == item.id_originLot);

                        if (itemAux != null)
                        {
                            throw new Exception("No se puede repetir el Item: " + itemAux.name + ",  en el lote: " + productionLotAux?.internalNumber + ", en el detalle de Materia Prima.");
                        }
                    }



                    item.id = productionLot.ProductionLotDetail.Count() > 0 ? productionLot.ProductionLotDetail.Max(pld => pld.id) + 1 : 1;

                    InvParameterBalanceGeneral invParameterBalance = new InvParameterBalanceGeneral();

                    invParameterBalance.id_company = this.ActiveCompanyId;
                    invParameterBalance.id_Item = item.id_item;
                    invParameterBalance.cut_Date = productionLot.receptionDate;
                    invParameterBalance.consolidado = true;
                    invParameterBalance.groupby = ServiceInventoryBalance.ServiceInventoryGroupBy.GROUPBY_BODEGA_UBICA_LOTE_ITEM;
                    invParameterBalance.id_Warehouse = item.id_warehouse;
                    invParameterBalance.id_WarehouseLocation = item.id_warehouseLocation;
                    invParameterBalance.id_ProductionLot = item.id_originLot;
                    var resultBalance = ServiceInventoryBalance.ValidateBalanceGeneral(invParameterBalance, modelSaldoProductlote:false );
                    var resultBalaceItem = resultBalance.Item1;
                    var balanceItemSaldo = resultBalaceItem.Where(b =>   b.id_warehouse == item.id_warehouse 
                                                                         && b.id_warehouseLocation == item.id_warehouseLocation 
                                                                         && b.id_productionLot == item.id_originLot
                                                                         && b.id_item == item.id_item)
                                                      .Sum(c => c.SaldoActual);

                    var sumBalanceItem = productionLot
                                            .ProductionLotDetail
                                            .Where(b =>     b.id_warehouse == item.id_warehouse 
                                                            && b.id_warehouseLocation == item.id_warehouseLocation 
                                                            && b.id_originLot == item.id_originLot
                                                            && b.id_item == item.id_item)
                                            .Sum(c => c.quantityRecived);
                    
                    if (balanceItemSaldo < (sumBalanceItem + item.quantityRecived))
                    {
                        throw new Exception("La cantidad ingresada supera la suma total del saldo de los item agregados en el detalle.");
                    }


                    productionLot.ProductionLotDetail.Add(item);
                    UpdateProductionLotTotals(productionLot);

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            ViewData["fechaProceso"] = productionLot.receptionDate;
            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            return PartialView("_ProductionLotProcessEditFormItemsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormItemsDetailUpdate(ProductionLotDetail item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotDetail.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);

                        ProductionProcess proceso = db.ProductionProcess.FirstOrDefault(r => r.id == productionLot.id_productionProcess);
                        if (proceso != null)
                        {
                            modelItem.id_warehouse = (proceso.id_warehouse ?? 0);
                            modelItem.id_warehouseLocation = (proceso.id_WarehouseLocation ?? 0);
                        }

                        InvParameterBalanceGeneral invParameterBalance = new InvParameterBalanceGeneral();

                        invParameterBalance.id_company = this.ActiveCompanyId;
                        invParameterBalance.id_Item = item.id_item;
                        invParameterBalance.cut_Date = productionLot.receptionDate;
                        invParameterBalance.id_Warehouse = modelItem.id_warehouse;
                        invParameterBalance.id_WarehouseLocation = modelItem.id_warehouseLocation;
                        invParameterBalance.consolidado = true;
                        invParameterBalance.groupby = ServiceInventoryBalance.ServiceInventoryGroupBy.GROUPBY_BODEGA_UBICA_LOTE_ITEM;
                        invParameterBalance.id_ProductionLot = modelItem.id_originLot;
                        var resultBalance = ServiceInventoryBalance.ValidateBalanceGeneral(invParameterBalance, modelSaldoProductlote: false);
                        var resultBalaceItem = resultBalance.Item1;
                        var balanceItemSaldo = resultBalaceItem.Where(b =>  b.id_warehouse == modelItem.id_warehouse 
                                                                            && b.id_warehouseLocation == modelItem.id_warehouseLocation
                                                                            && b.id_productionLot  == modelItem.id_originLot
                                                                            && b.id_item == item.id_item)
                                                          .Sum(c => c.SaldoActual);

                        //var balaceItem = ServiceInventoryBalance.ValidateBalance(db, invParameterBalance);
                        var sumBalanceItem = productionLot
                                                    .ProductionLotDetail
                                                    .Where(b => b.id_warehouse == modelItem.id_warehouse
                                                                && b.id_warehouseLocation == modelItem.id_warehouseLocation
                                                                && b.id_originLot == modelItem.id_originLot
                                                                && b.id_item == item.id_item
                                                                && b.id != item.id)
                                                    .Sum(c => c.quantityRecived);

                        if (balanceItemSaldo < (sumBalanceItem + item.quantityRecived))
                        {
                            throw new Exception("La cantidad ingresada supera la suma total del saldo de los item agregados en el detalle.");
                        }

                        UpdateProductionLotTotals(productionLot);
                        TempData["productionLotProcess"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            return PartialView("_ProductionLotProcessEditFormItemsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormItemsDetailDelete(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();

            try
            {
                var productionLotDetails = productionLot.ProductionLotDetail.FirstOrDefault(p => p.id == id);
                if (productionLotDetails != null)
                {
                    var id_qualityControlAux = productionLotDetails.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.QualityControl.Document.DocumentState.code != ("05"))?.id_qualityControl;
                    if (id_qualityControlAux != null)
                    {
                        using (DbContextTransaction trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

                                var qualityControlAux = db.QualityControl.FirstOrDefault(fod => fod.id == id_qualityControlAux);
                                qualityControlAux.Document.id_documentState = documentState.id;
                                qualityControlAux.Document.DocumentState = documentState;

                                db.QualityControl.Attach(qualityControlAux);
                                db.Entry(qualityControlAux).State = EntityState.Modified;

                                db.SaveChanges();
                                trans.Commit();
                            }
                            catch (Exception e)
                            {
                                trans.Rollback();
                                throw e;
                            }
                        }
                    }

                    productionLot.ProductionLotDetail.Remove(productionLotDetails);
                    UpdateProductionLotTotals(productionLot);
                    TempData["productionLotProcess"] = productionLot;
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            return PartialView("_ProductionLotProcessEditFormItemsDetailPartial", model);
        }

        private void UpdateProductionLotTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityOrdered = 0.0M;
            productionLot.totalQuantityRemitted = 0.0M;
            productionLot.totalQuantityRecived = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;

            foreach (var productionLotDetail in productionLot.ProductionLotDetail)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotDetail.id_item);
                var id_metricUnitInventoryAux = ItemAux?.ItemInventory?.MetricUnit.id ?? 0;
                var id_metricUnitAux = ItemAux?.Presentation?.MetricUnit.id ?? id_metricUnitInventoryAux;
                var minimumAux = DataProviderItem.GetMinimoProductionProcessWMasterCalc(productionLotDetail.id_item);
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                productionLot.totalQuantityRecived += (productionLotDetail.quantityRecived * (minimumAux)) * factor;
            }

        }

        #endregion ITEMS

        #region PRODUCTION LOT LIQUIDATION

        [ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotLiquidationsDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLot;
            TempData.Keep("productionLotProcess");

            return PartialView("_ProductionLotProcessEditFormProductionLotLiquidationsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotLiquidationsDetailAddNew(int? id_salesOrder, ProductionLotLiquidation productionLotLiquidation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();

            if (ModelState.IsValid)
            {
                productionLotLiquidation.id = productionLot.ProductionLotLiquidation.Count() > 0 ? productionLot.ProductionLotLiquidation.Max(pld => pld.id) + 1 : 1;
                productionLotLiquidation.SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod => fod.id_salesOrder == id_salesOrder && fod.id_item == productionLotLiquidation.id_item);
                productionLotLiquidation.id_salesOrderDetail = productionLotLiquidation.SalesOrderDetail?.id;

                productionLotLiquidation.Item = db.Item.FirstOrDefault(i => i.id == productionLotLiquidation.id_item);
                productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>();

                productionLot.ProductionLotLiquidation.Add(productionLotLiquidation);
                UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

                UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, productionLotLiquidation);
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            return PartialView("_ProductionLotProcessEditFormProductionLotLiquidationsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotLiquidationsDetailUpdate(int? id_salesOrder, ProductionLotLiquidation productionLotLiquidation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotLiquidation.FirstOrDefault(pll => pll.id == productionLotLiquidation.id);
                    if (modelItem != null)
                    {
                        modelItem.SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod => fod.id_salesOrder == id_salesOrder && fod.id_item == productionLotLiquidation.id_item);
                        modelItem.id_salesOrderDetail = productionLotLiquidation.SalesOrderDetail?.id;
                        this.UpdateModel(modelItem);
                        UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

                        modelItem.Item = db.Item.FirstOrDefault(i => i.id == productionLotLiquidation.id_item);

                        UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, modelItem);

                        TempData["productionLotProcess"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            return PartialView("_ProductionLotProcessEditFormProductionLotLiquidationsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotLiquidationsDetailDelete(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();

            try
            {
                var productionLotLiquidation = productionLot.ProductionLotLiquidation.FirstOrDefault(p => p.id == id);
                if (productionLotLiquidation != null)
                {
                    productionLot.ProductionLotLiquidation.Remove(productionLotLiquidation);
                    UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);
                    UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, productionLotLiquidation);
                }

                TempData["productionLotProcess"] = productionLot;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();
            return PartialView("_ProductionLotProcessEditFormProductionLotLiquidationsDetailPartial", model);
        }

        private void UpdateProductionLotProductionLotLiquidationsDetailTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityLiquidation = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);
            var id_metricUnitLbsAux = metricUnitUMTP?.id;

            var metricUnitFixedPund = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs");
            var id_metricUnitFixedPund = metricUnitFixedPund?.id ?? 0;

            var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitFixedPund &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
            var factor = id_metricUnitLbsAux == id_metricUnitFixedPund && id_metricUnitFixedPund != 0 ? 1 : (metricUnitConversion?.factor ?? 0);

            foreach (var productionLotLiquidation in productionLot.ProductionLotLiquidation)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotLiquidation.id_item);
                var valueAux = decimal.Round(productionLotLiquidation.quantityPoundsLiquidation.Value * factor, 2);
                productionLot.totalQuantityLiquidation += valueAux;
            }
        }

        #endregion PRODUCTION LOT LIQUIDATION

        #region PRODUCTION LOT PACKING MATERIAL

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessPackingMaterialsPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotPackingMaterial = productionLot.ProductionLotPackingMaterial ?? new List<ProductionLotPackingMaterial>();

            var model = productionLot.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0);

            TempData.Keep("productionLotProcess");

            return PartialView("_ProductionLotProcessEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessPackingMaterialsPartialAddNew(ProductionLotPackingMaterial productionLotPackingMaterial)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotPackingMaterial.FirstOrDefault(it => it.id_item == productionLotPackingMaterial.id_item);
                    if (modelItem != null)
                    {
                        modelItem.isActive = true;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                    }
                    else
                    {
                        productionLotPackingMaterial.Item = db.Item.FirstOrDefault(i => i.id == productionLotPackingMaterial.id_item);

                        productionLotPackingMaterial.isActive = true;
                        productionLotPackingMaterial.manual = true;
                        productionLotPackingMaterial.id_userCreate = ActiveUser.id;
                        productionLotPackingMaterial.dateCreate = DateTime.Now;
                        productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                        productionLotPackingMaterial.dateUpdate = DateTime.Now;

                        productionLot.ProductionLotPackingMaterial.Add(productionLotPackingMaterial);
                    }

                    TempData["productionLotProcess"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0).ToList() ?? new List<ProductionLotPackingMaterial>();
            return PartialView("_ProductionLotProcessEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessPackingMaterialsPartialUpdate(ProductionLotPackingMaterial productionLotPackingMaterial)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotPackingMaterial.FirstOrDefault(it => it.id_item == productionLotPackingMaterial.id_item);
                    if (modelItem != null)
                    {
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        TempData["productionLotProcess"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0).ToList() ?? new List<ProductionLotPackingMaterial>();

            return PartialView("_ProductionLotProcessEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessPackingMaterialsPartialDelete(System.Int32 id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (id_item >= 0)
            {
                try
                {
                    var productionLotPackingMaterial = productionLot.ProductionLotPackingMaterial.FirstOrDefault(p => p.id_item == id_item);
                    if (productionLotPackingMaterial != null)
                    {
                        if (!productionLotPackingMaterial.manual)
                        {
                            throw (new Exception("Este Ítem de Materiales de Empaque no se puede eliminar debido a que fue cargado desde la formulación de algún producto de la liquidación"));
                        }

                        productionLotPackingMaterial.isActive = false;
                        productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                        productionLotPackingMaterial.dateUpdate = DateTime.Now;
                    }

                    TempData["productionLotProcess"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0).ToList() ?? new List<ProductionLotPackingMaterial>();
            return PartialView("_ProductionLotProcessEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedProductionLotProcessPackingMaterials(int[] ids)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ids != null)
            {
                try
                {
                    var productionLotPackingMaterial = productionLot.ProductionLotPackingMaterial.Where(i => ids.Contains(i.id_item));

                    foreach (var detail in productionLotPackingMaterial)
                    {
                        if (detail.manual)
                        {
                            detail.isActive = false;
                            detail.id_userUpdate = ActiveUser.id;
                            detail.dateUpdate = DateTime.Now;
                        }
                    }

                    TempData["productionLotProcess"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("productionLotProcess");
        }

        #endregion PRODUCTION LOT PACKING MATERIAL

        #region PRODUCTION LOT TRASH

        [ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotTrashsDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();

            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLot;
            TempData.Keep("productionLotProcess");

            return PartialView("_ProductionLotProcessEditFormProductionLotTrashsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotTrashsDetailAddNew(ProductionLotTrash productionLotTrash)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotTrash = productionLot.ProductionLotTrash ?? new List<ProductionLotTrash>();

            if (ModelState.IsValid)
            {
                productionLotTrash.id = productionLot.ProductionLotTrash.Count() > 0 ? productionLot.ProductionLotTrash.Max(plt => plt.id) + 1 : 1;
                productionLot.ProductionLotTrash.Add(productionLotTrash);
                UpdateProductionLotProductionLotTrashsDetailTotals(productionLot);
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();

            return PartialView("_ProductionLotProcessEditFormProductionLotTrashsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotTrashsDetailUpdate(ProductionLotTrash productionLotTrash)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotTrash = productionLot.ProductionLotTrash ?? new List<ProductionLotTrash>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotTrash.FirstOrDefault(plt => plt.id == productionLotTrash.id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        UpdateProductionLotProductionLotTrashsDetailTotals(productionLot);
                        TempData["productionLotProcess"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();

            return PartialView("_ProductionLotProcessEditFormProductionLotTrashsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotTrashsDetailDelete(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotTrash = productionLot.ProductionLotTrash ?? new List<ProductionLotTrash>();

            try
            {
                var productionLotTrash = productionLot.ProductionLotTrash.FirstOrDefault(p => p.id == id);
                if (productionLotTrash != null)
                {
                    productionLot.ProductionLotTrash.Remove(productionLotTrash);
                    UpdateProductionLotProductionLotTrashsDetailTotals(productionLot);
                }

                TempData["productionLotProcess"] = productionLot;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();
            return PartialView("_ProductionLotProcessEditFormProductionLotTrashsDetailPartial", model);
        }

        private void UpdateProductionLotProductionLotTrashsDetailTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityTrash = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;

            foreach (var productionLotTrash in productionLot.ProductionLotTrash)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotTrash.id_item);
                var id_metricUnitAux = productionLotTrash.id_metricUnit;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                productionLot.totalQuantityTrash += productionLotTrash.quantity * factor;
            }
        }

        #endregion PRODUCTION LOT TRASH

        #region PRODUCTION LOT LOSS

        [ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotLossDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotLoss.ToList() ?? new List<ProductionLotLoss>();

            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLot;
            TempData.Keep("productionLotProcess");

            return PartialView("_ProductionLotProcessEditFormProductionLotLossDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotLossDetailAddNew(ProductionLotLoss productionLotLoss)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLoss = productionLot.ProductionLotLoss ?? new List<ProductionLotLoss>();

            if (ModelState.IsValid)
            {
                productionLotLoss.id = productionLot.ProductionLotLoss.Count() > 0 ? productionLot.ProductionLotLoss.Max(plt => plt.id) + 1 : 1;
                productionLot.ProductionLotLoss.Add(productionLotLoss);
                UpdateProductionLotProductionLotLossDetailTotals(productionLot);
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotLoss.ToList() ?? new List<ProductionLotLoss>();

            return PartialView("_ProductionLotProcessEditFormProductionLotLossDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotLossDetailUpdate(ProductionLotLoss productionLotLoss)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLoss = productionLot.ProductionLotLoss ?? new List<ProductionLotLoss>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotLoss.FirstOrDefault(plt => plt.id == productionLotLoss.id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        UpdateProductionLotProductionLotLossDetailTotals(productionLot);
                        TempData["productionLotProcess"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotLoss.ToList() ?? new List<ProductionLotLoss>();

            return PartialView("_ProductionLotProcessEditFormProductionLotLossDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessEditFormProductionLotLossDetailDelete(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLoss = productionLot.ProductionLotLoss ?? new List<ProductionLotLoss>();

            try
            {
                var productionLotLoss = productionLot.ProductionLotLoss.FirstOrDefault(p => p.id == id);
                if (productionLotLoss != null)
                {
                    productionLot.ProductionLotLoss.Remove(productionLotLoss);
                    UpdateProductionLotProductionLotLossDetailTotals(productionLot);
                }

                TempData["productionLotProcess"] = productionLot;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotLoss.ToList() ?? new List<ProductionLotLoss>();
            return PartialView("_ProductionLotProcessEditFormProductionLotLossDetailPartial", model);
        }

        private void UpdateProductionLotProductionLotLossDetailTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityLoss = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;

            foreach (var productionLotLoss in productionLot.ProductionLotLoss)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotLoss.id_item);
                var id_metricUnitAux = productionLotLoss.id_metricUnit;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                productionLot.totalQuantityLoss += productionLotLoss.quantity * factor;
            }
        }

        #endregion PRODUCTION LOT LOSS

        #region COMMON - PROCESS

        [HttpPost]
        public ActionResult ProductionLotProcessResultsPartial(ProductionLot productionLot,
                                                                 int? filterWarehouse, int? filterWarehouseLocation,
                                                                 DateTime? startReceptionDate, DateTime? endReceptionDate,
                                                                 int[] items)
        {
            List<ProductionLot> model = db.ProductionLot.Where(p => p.ProductionProcess.code != "REC" && p.ProductionProcess.code != "RMM"
                                                                    && p.ProductionLotState.code != "100").AsEnumerable().ToList();

            if (productionLot.id_ProductionLotState != 0)
            {
                model = model.Where(o => o.id_ProductionLotState == productionLot.id_ProductionLotState).ToList();
            }

            if (!string.IsNullOrEmpty(productionLot.number))
            {
                model = model.Where(o => o.number.Contains(productionLot.number)).ToList();
            }

            if (!string.IsNullOrEmpty(productionLot.internalNumber))
            {
                model = model.Where(o => o.internalNumber.Contains(productionLot.internalNumber)).ToList();
            }

            if (!string.IsNullOrEmpty(productionLot.reference))
            {
                model = model.Where(o => (o.reference ?? String.Empty).Contains(productionLot.reference)).ToList();
            }

            if (productionLot.id_productionUnit != 0)
            {
                model = model.Where(o => o.id_productionUnit == productionLot.id_productionUnit).ToList();
            }

            if (productionLot.id_productionProcess != 0)
            {
                model = model.Where(o => o.id_productionProcess == productionLot.id_productionProcess).ToList();
            }

            if (filterWarehouse != null && filterWarehouse != 0)
            {
                var tempModel = new List<ProductionLot>();
                foreach (var production in model)
                {
                    var details = production.ProductionLotDetail.Where(d => d.id_warehouse == filterWarehouse.Value);
                    if (details.Any())
                    {
                        tempModel.Add(production);
                    }
                }

                model = tempModel;
            }

            if (filterWarehouseLocation != null && filterWarehouseLocation != 0)
            {
                var tempModel = new List<ProductionLot>();
                foreach (var production in model)
                {
                    var details = production.ProductionLotDetail.Where(d => d.id_warehouseLocation == filterWarehouseLocation.Value);
                    if (details.Any())
                    {
                        tempModel.Add(production);
                    }
                }

                model = tempModel;
            }

            if (startReceptionDate != null && endReceptionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startReceptionDate.Value.Date, o.receptionDate.Date) <= 0 && DateTime.Compare(o.receptionDate.Date, endReceptionDate.Value.Date) <= 0).ToList();
            }

            if (items != null && items.Length > 0)
            {
                var tempModel = new List<ProductionLot>();
                foreach (var production in model)
                {
                    var details = production.ProductionLotDetail.Where(d => items.Contains(d.id_item));
                    if (details.Any())
                    {
                        tempModel.Add(production);
                    }
                }

                model = tempModel;
            }

            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_ProductionLotProcessResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost]
        public ActionResult ProductionLotProcessPartial()
        {
            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();

            TempData.Keep("model");
            return PartialView("_ProductionLotProcessPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessAddNew(bool approve, ProductionLot item)
        {
            int empleadoId = 0;
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            string setting_liquidacionValorizada = (db.Setting.FirstOrDefault(r => r.code == "LIQNOVAL")?.value ?? "NO");
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            ViewBag.isProductionProcess = "S";

            int inventoryMoveEntry = 0;
            int inventoryMoveExit = 0;

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();

            productionLot.receptionDate = item.receptionDate;
            productionLot.id_productionProcess = item.id_productionProcess;
            ProductionProcess proceso = db.ProductionProcess.FirstOrDefault(r => r.id == productionLot.id_productionProcess);
            if (proceso != null)
            {
                productionLot.id_productionUnit = (proceso.id_ProductionUnit ?? 0);
                item.id_productionUnit = (proceso.id_ProductionUnit ?? 0);
            }

            productionLot.description = item.description;
            productionLot.reference = item.reference;
            productionLot.expirationDate = item.expirationDate;
            productionLot.totalQuantityOrdered = item.totalQuantityOrdered;
            productionLot.totalQuantityRemitted = item.totalQuantityRemitted;
            productionLot.totalQuantityRecived = item.totalQuantityRecived;
            productionLot.totalQuantityLiquidation = item.totalQuantityLiquidation;
            productionLot.totalQuantityTrash = item.totalQuantityTrash;
            productionLot.totalQuantityLiquidationAdjust = item.totalQuantityLiquidationAdjust;
            productionLot.liquidationDate = item.liquidationDate;
            productionLot.id_Turn = item.id_Turn;
            productionLot.Turn = db.Turn.FirstOrDefault(p => p.id == item.id_Turn);
            productionLot.id_MachineForProd = item.id_MachineForProd;
            productionLot.MachineForProd = db.MachineForProd.FirstOrDefault(p => p.id == item.id_MachineForProd);
            productionLot.id_MachineProdOpening = item.id_MachineProdOpening;
            productionLot.MachineProdOpening = db.MachineProdOpening.FirstOrDefault(p => p.id == item.id_MachineProdOpening);
            productionLot.id_liquidator = item.id_liquidator;
            productionLot.closeDate = item.closeDate;
            productionLot.totalQuantityLoss = item.totalQuantityLoss;
            productionLot.id_wareHouseTransfer = item.id_wareHouseTransfer;
            productionLot.Warehouse = db.Warehouse.FirstOrDefault(p => p.id == item.id_wareHouseTransfer);
            productionLot.id_wareHouseLocationTransfer = item.id_wareHouseLocationTransfer;
            productionLot.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(p => p.id == item.id_wareHouseLocationTransfer);
            productionLot.id_dischargeReason = item.id_dischargeReason;
            productionLot.InventoryReason = db.InventoryReason.FirstOrDefault(p => p.id == item.id_dischargeReason);
            productionLot.id_incomeReason = item.id_incomeReason;
            productionLot.InventoryReason1 = db.InventoryReason.FirstOrDefault(p => p.id == item.id_incomeReason);
            productionLot.id_personProcessPlant = item.id_personProcessPlant;

            var strJul = string.IsNullOrWhiteSpace(productionLot.julianoNumber) ? item.julianoNumber : productionLot.julianoNumber;
            var strInt = string.IsNullOrWhiteSpace(productionLot.internalNumber) ? item.internalNumber : productionLot.internalNumber;

            productionLot.internalNumberConcatenated = $"{productionLot.julianoNumber}{productionLot.internalNumber}";
            item.internalNumber = item.internalNumberConcatenated;

            Lot lot = new Lot();
            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.requestliquidationmachine == true)
            {
                ViewBag.solicitaMaquina = true;
            }
            else
            {
                ViewBag.solicitaMaquina = false;
            }
            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.generateTransfer == true)
            {
                ViewBag.generaTransferencia = true;
            }
            else
            {
                ViewBag.generaTransferencia = false;
            }
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            #region Liquiacion Valorizada - 6115
            bool isRequestCarMachine = false;
            if (setting_liquidacionValorizada == "SI")
            {
                isRequestCarMachine = (ProductionProcess.GetOneById(productionLot.id_productionProcess)?.requestCarMachine ?? false);
            }
            ViewBag.isRequestCarMachine = isRequestCarMachine;

            if (isRequestCarMachine)
            {
                ProductionLotMachineTurn productionLotMachineTurn = ProductionLotMachineTurn.GetOneByProductionLot(productionLot.id);

                int idMachineForProd = (productionLotMachineTurn?.idMachineForProd ?? 0);
                int id_MachineProdOpening = (productionLotMachineTurn?.idMachineProdOpening ?? 0);
                ViewBag.id_MachineProdOpeningDetailInitLiqNoVal = db.MachineProdOpeningDetail
                                                                        .FirstOrDefault(fod => fod.id_MachineForProd == idMachineForProd
                                                                                                && fod.id_MachineProdOpening == id_MachineProdOpening)?
                                                                        .MachineForProd?.id;


                ViewBag.productionLotMachineTurn = (productionLotMachineTurn ?? new ProductionLotMachineTurn());
            }

            #endregion
            if (ModelState.IsValid)
            {
                
                
                    try
                    {
                        #region Lot

                        lot = ServiceLot.CreateLot(db, ActiveUser, ActiveCompany, ActiveEmissionPoint);
                        item.id = lot.id;
                        item.Lot = lot;

                        #endregion Lot

                        #region ProductionLot

                        item.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.id == item.id_ProductionLotState);
                        productionLot.ProductionLotState = item.ProductionLotState;

                        ProductionProcess process = db.ProductionProcess.FirstOrDefault(p => p.id == item.id_productionProcess);

                        item.ProductionProcess = process;
                        productionLot.ProductionProcess = process;

                        item.ProductionUnit = db.ProductionUnit.FirstOrDefault(u => u.id == item.id_productionUnit);
                        productionLot.ProductionUnit = item.ProductionUnit;
                        item.Employee = db.Employee.FirstOrDefault(p => p.id == item.id_personRequesting);
                        item.Employee1 = item.Employee;

                        empleadoId = item.id_personRequesting;

                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        string code = process?.code ?? "";
                        string sequential = process?.sequential.ToString("D9") ?? "";

                        item.number = code + sequential;
                        item.internalNumber = (!string.IsNullOrEmpty(item.internalNumber)) ? item.internalNumber : item.number;

                        item.Lot.number = item.number;

                        if (process != null)
                        {
                            process.sequential++;
                            //db.ProductionProcess.Attach(process);
                            db.Entry(process).State = EntityState.Modified;
                        }

                        #endregion ProductionLot

                        #region ADD ITEMS DETAILS

                        if (productionLot.ProductionLotDetail != null)
                        {
                            item.ProductionLotDetail = new List<ProductionLotDetail>();

                            var lotDetailGroupList = productionLot
                                                    .ProductionLotDetail
                                                    .ToList()
                                                    .GroupBy(r => r.id_originLot);
                            var lotDetailGroup = lotDetailGroupList.Select(r => new
                                                    {
                                                        idLot = r.Key.Value,
                                                        qty = r.Sum(t => t.quantityRecived)
                                                    }).ToList();

                            int index = 1;
                            foreach (var detail in lotDetailGroup)
                            {
                                ProductionLot productionLotOriginDetail = ServicePoductionLot
                                                                                        .GetOriginProductionLotForProductProcess
                                                                                        (db,
                                                                                        ActiveCompany,
                                                                                        ActiveUser,
                                                                                        detail.idLot,
                                                                                        item.Lot.number,
                                                                                        item.id_productionProcess,
                                                                                        item.receptionDate,
                                                                                        item.expirationDate,
                                                                                        detail.qty,
                                                                                        index,
                                                                                        item.id_personRequesting);
                                index++;
                            }

                            foreach (var detail in productionLot.ProductionLotDetail.AsEnumerable())
                            {
                                var productionLotDetail = new ProductionLotDetail();

                                productionLotDetail.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();
                                foreach (var productionLotDetailQualityControl in detail.ProductionLotDetailQualityControl)
                                {
                                    var newProductionLotDetailQualityControl = new ProductionLotDetailQualityControl
                                    {
                                        id_productionLotDetail = productionLotDetailQualityControl.id_productionLotDetail,
                                        ProductionLotDetail = productionLotDetail,
                                        id_qualityControl = productionLotDetailQualityControl.id_qualityControl,
                                        QualityControl = db.QualityControl.FirstOrDefault(i => i.id == productionLotDetailQualityControl.id_qualityControl),
                                    };
                                    productionLotDetail.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);
                                    newProductionLotDetailQualityControl.QualityControl.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);
                                    newProductionLotDetailQualityControl.QualityControl.id_lot = item.id;
                                    newProductionLotDetailQualityControl.QualityControl.Lot = item.Lot;
                                }

                                productionLotDetail.id_item = detail.id_item;
                                productionLotDetail.Item = db.Item.FirstOrDefault(i => i.id == productionLotDetail.id_item);
                                productionLotDetail.id_originLot = detail.id_originLot;
                                productionLotDetail.id_warehouse = detail.id_warehouse;
                                productionLotDetail.Warehouse = db.Warehouse.FirstOrDefault(i => i.id == productionLotDetail.id_warehouse);
                                productionLotDetail.id_warehouseLocation = detail.id_warehouseLocation;
                                productionLotDetail.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(i => i.id == productionLotDetail.id_warehouseLocation);
                                productionLotDetail.quantityOrdered = detail.quantityOrdered;
                                productionLotDetail.quantityRemitted = detail.quantityRemitted;
                                productionLotDetail.quantityRecived = detail.quantityRecived;
                                productionLotDetail.quantityProcessed = detail.quantityProcessed;
                                productionLotDetail.lotMarked = detail.lotMarked;

                                item.ProductionLotDetail.Add(productionLotDetail);

                            }
                        }

                        if (item.ProductionLotDetail.Count == 0)
                        {
                            TempData.Keep("productionLotProcess");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar un lote sin detalles de Materia Prima");

                            item.julianoNumber = strJul;
                            item.internalNumber = strInt;
                            item.internalNumberConcatenated = $"{strJul}{strInt}";
                            return PartialView("_ProductionLotProcessEditFormPartial", item);
                        }

                        #endregion ADD ITEMS DETAILS

                        UpdateProductionLotTotals(item);
                        if (approve)
                        {
                            ServiceInventoryMove.UpdateInventaryMoveExitRawMaterialProcess(ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                            item.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "02");

                            var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");
                            item.Lot.Document.id_documentState = documentState.id;
                            item.Lot.Document.DocumentState = documentState;

                            var natureExitMoves = db.AdvanceParametersDetail.FirstOrDefault(r => r.valueCode.Trim() == "E");
                            inventoryMoveExit = db.InventoryMove.FirstOrDefault(r => r.id_productionLot == productionLot.id
                                                                                      && r.idNatureMove == natureExitMoves.id)?.id ?? 0;
                        }

                        if (item.id == 0)
                        {
                            db.ProductionLot.Add(item);
                        }
                        else
                        {
                            //db.ProductionLot.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.SaveChanges();
                            trans.Commit();
                            TempData["productionLotProcess"] = item;
                            TempData.Keep("productionLotProcess");

                            ViewData["EditMessage"] = SuccessMessage("Lote: " + item.number + " guardado exitosamente");
                        }
                        catch(Exception e)
                        {
                            trans.Rollback();
                        }
                    }
                       
                    }
                    catch (Exception e)
                    {
                        TempData["productionLotProcess"] = productionLot;
                        TempData.Keep("productionLotProcess");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        

                        try
                        {
                            var innerMensaje = string.Empty;
                            var inner3 = string.Empty;

                            string errorData = string.Empty;
                            foreach (DictionaryEntry data in e.Data)
                            {
                                errorData += $"{data.Key.ToString()}: {data.Value.ToString()} {Environment.NewLine}";
                            }

                            string errorDataInner = string.Empty;
                            if (e.InnerException != null)
                            {
                                innerMensaje = e.InnerException.Message;
                                foreach (DictionaryEntry data in e.InnerException.Data)
                                {
                                    errorDataInner += $"{data.Key.ToString()}: {data.Value.ToString()} {Environment.NewLine}";
                                }
                                if (e.InnerException.InnerException != null)
                                {
                                    inner3 = e.InnerException.InnerException.Message;
                                }
                            }

                            var mensaje = $"Mensaje:{e.Message}{Environment.NewLine} .Error:{errorData}{Environment.NewLine}  .InnerExc:{innerMensaje}{Environment.NewLine}{errorDataInner}{Environment.NewLine} .Level3:{inner3} {Environment.NewLine} empleadoid{empleadoId}";
                            MetodosEscrituraLogs.EscribeMensajeLog(mensaje, _rutaLog, "PROCESOSINTERNOS", "PROD");
                        }
                        catch
                        { }

                        productionLot.julianoNumber = strJul;
                        productionLot.internalNumber = strInt;
                        productionLot.internalNumberConcatenated = $"{strJul}{strInt}";

                        return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                    }
                
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            var stateLot = productionLot.ProductionLotState.code;
            if (stateLot != "01")
            {
                var natureMoves = db.AdvanceParametersDetail.Where(r => r.valueCode.Trim() == "I" || r.valueCode.Trim() == "E").ToList();
                var inventoryMoves = db.InventoryMove.Where(r => r.id_productionLot == productionLot.id && r.Document.DocumentState.code != "05").ToList();
                var natureMoveEgreso = natureMoves.FirstOrDefault(r => r.valueCode.Trim() == "E");
                inventoryMoveExit = inventoryMoves.FirstOrDefault(r => r.idNatureMove == natureMoveEgreso.id)?.id ?? 0;
                if (stateLot != "02" && stateLot != "03")
                {
                    var natureMoveIngreso = natureMoves.FirstOrDefault(r => r.valueCode.Trim() == "I");
                    inventoryMoveEntry = inventoryMoves.FirstOrDefault(r => r.idNatureMove == natureMoveIngreso.id)?.id ?? 0;
                }
            }

            item.julianoNumber = strJul;
            item.internalNumber = strInt;
            item.internalNumberConcatenated = $"{strJul}{strInt}";

            ViewBag.imoveEntry = inventoryMoveEntry;
            ViewBag.imovExit = inventoryMoveExit;

            return PartialView("_ProductionLotProcessEditFormPartial", item);
        }

        public ActionResult ProductionLotProcessUpdate(bool approve, ProductionLot item, ProductionLotMachineTurn productionLotMachineTurn)
        {
            int empleadoId = 0;
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            BuildViewDataEdit();
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            string setting_liquidacionValorizada = (db.Setting.FirstOrDefault(r => r.code == "LIQNOVAL")?.value ?? "NO");
            bool isSaveLiqNoVal = false;
            ViewBag.isProductionProcess = "S";
            var strJul = item.julianoNumber;
            var strInt = item.internalNumber;

            int inventoryMoveEntry = 0;
            int inventoryMoveExit = 0;
            bool isRequestCarMachine = false;

            int[] inventoryMoveDetailIdsForDelete = Array.Empty<int>();

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            productionLot.internalNumber = item.internalNumber;
            productionLot.receptionDate = item.receptionDate;
            productionLot.internalNumberConcatenated = item.internalNumberConcatenated;
            productionLot.id_productionProcess = item.id_productionProcess;
            productionLot.id_productionProcess = item.id_productionProcess;
            //ProductionProcess proceso = db.ProductionProcess.FirstOrDefault(r => r.id == productionLot.id_productionProcess);

            ProductionProcess proceso = ProductionProcess.GetOneById(productionLot.id_productionProcess);

            if (setting_liquidacionValorizada == "SI")
            {
                isRequestCarMachine = (proceso.requestCarMachine??false);
            }

            var itemList = db.Item.Where(i => i.isActive).ToList();
            var warehouse = db.Warehouse.Where(w => w.isActive).ToList();
            var warehouseLocation = db.WarehouseLocation.Where(wl => wl.isActive).ToList();

            if (productionLot?.ProductionLotLiquidation.Count() == 0 || productionLot?.ProductionLotLiquidation == null)
            {
                ProductionLot productionLotLiquidation = (TempData["productionLot"] as ProductionLot);
                if(productionLotLiquidation != null)
                {
                    productionLot.ProductionLotLiquidation = productionLotLiquidation?.ProductionLotLiquidation;
                }
            }

            if (productionLot?.ProductionLotLoss.Count() == 0 || productionLot?.ProductionLotLoss == null)
            {
                ProductionLot productionLotLoss = (TempData["productionLot"] as ProductionLot);
                if (productionLotLoss != null)
                {
                    productionLot.ProductionLotLoss = productionLotLoss?.ProductionLotLoss;
                }
            }

            if (productionLot?.ProductionLotTrash.Count() == 0 || productionLot?.ProductionLotTrash == null)
            {
                ProductionLot productionLotTrash = (TempData["productionLot"] as ProductionLot);
                if (productionLotTrash != null)
                {
                    productionLot.ProductionLotTrash = productionLotTrash?.ProductionLotTrash;
                }
            }

            if (proceso != null)
            {
                productionLot.id_productionUnit = (proceso.id_ProductionUnit ?? 0);
                item.id_productionUnit = (proceso.id_ProductionUnit ?? 0);
            }


            productionLot.description = item.description;
            productionLot.reference = item.reference;
            productionLot.expirationDate = item.expirationDate;
            productionLot.totalQuantityOrdered = item.totalQuantityOrdered;
            productionLot.totalQuantityRemitted = item.totalQuantityRemitted;
            productionLot.totalQuantityRecived = item.totalQuantityRecived;
            productionLot.totalQuantityLiquidation = item.totalQuantityLiquidation;
            productionLot.totalQuantityTrash = item.totalQuantityTrash;
            productionLot.totalQuantityLiquidationAdjust = item.totalQuantityLiquidationAdjust;
            productionLot.liquidationDate = item.liquidationDate;
            productionLot.id_Turn = item.id_Turn;
            productionLot.Turn = db.Turn.FirstOrDefault(p => p.id == item.id_Turn);
            productionLot.id_MachineForProd = item.id_MachineForProd;
            productionLot.id_MachineProdOpening = item.id_MachineProdOpening;
            productionLot.id_liquidator = item.id_liquidator;
            productionLot.closeDate = item.closeDate;
            productionLot.totalQuantityLoss = item.totalQuantityLoss;
            productionLot.id_wareHouseTransfer = item.id_wareHouseTransfer;
            productionLot.id_wareHouseLocationTransfer = item.id_wareHouseLocationTransfer;
            productionLot.id_dischargeReason = item.id_dischargeReason;
            productionLot.id_incomeReason = item.id_incomeReason;
            var modelItem = db.ProductionLot.FirstOrDefault(p => p.id == item.id);

            if (ModelState.IsValid && modelItem != null)
            {
               
                    ProductionProcess process = new ProductionProcess();
                    try
                    {
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        if (solicitaMaquina == "SI" && productionLot.ProductionProcess?.requestliquidationmachine == true)
                        {
                            ViewBag.solicitaMaquina = true;
                        }
                        else
                        {
                            ViewBag.solicitaMaquina = false;
                        }
                        if (solicitaMaquina == "SI" && productionLot.ProductionProcess?.generateTransfer == true)
                        {
                            ViewBag.generaTransferencia = true;
                        }
                        else
                        {
                            ViewBag.generaTransferencia = false;
                        }

                    #region Liquiacion Valorizada - 6115
                    

                    ViewBag.isRequestCarMachine = isRequestCarMachine;
                    if (isRequestCarMachine)
                    {
                        productionLotMachineTurn = productionLotMachineTurn ?? ProductionLotMachineTurn.GetOneByProductionLot(productionLot.id);

                        int idMachineForProd = (productionLotMachineTurn?.idMachineForProd ?? 0);
                        int id_MachineProdOpening = (productionLotMachineTurn?.idMachineProdOpening ?? 0);
                        ViewBag.id_MachineProdOpeningDetailInitLiqNoVal = db.MachineProdOpeningDetail
                                                                                .FirstOrDefault(fod => fod.id_MachineForProd == idMachineForProd
                                                                                                        && fod.id_MachineProdOpening == id_MachineProdOpening)?
                                                                                .MachineForProd?.id;


                        ViewBag.productionLotMachineTurn = (productionLotMachineTurn ?? new ProductionLotMachineTurn());
                    }

                    #endregion

                    if (modelItem.ProductionLotState.code == "01")
                    {
                            process = db.ProductionProcess.FirstOrDefault(p => p.id == item.id_productionProcess);
                            modelItem.ProductionProcess = process;
                            productionLot.ProductionProcess = process;

                            modelItem.ProductionUnit = db.ProductionUnit.FirstOrDefault(u => u.id == item.id_productionUnit);
                            productionLot.ProductionUnit = modelItem.ProductionUnit;

                            modelItem.id_MachineForProd = productionLot.id_MachineForProd;
                            modelItem.MachineForProd = db.MachineForProd.FirstOrDefault(p => p.id == productionLot.id_MachineForProd);
                            modelItem.id_MachineProdOpening = productionLot.id_MachineProdOpening;
                            modelItem.MachineProdOpening = db.MachineProdOpening.FirstOrDefault(p => p.id == productionLot.id_MachineProdOpening);
                            modelItem.id_liquidator = productionLot.id_liquidator;

                            modelItem.Employee = db.Employee.FirstOrDefault(p => p.id == item.id_personRequesting);
                            modelItem.Employee1 = modelItem.Employee;

                            empleadoId = item.id_personRequesting;

                            modelItem.internalNumber = productionLot.internalNumberConcatenated;
                            modelItem.reference = productionLot.reference;
                            modelItem.description = productionLot.description;
                            modelItem.id_Turn = productionLot.id_Turn;
                            modelItem.Turn = db.Turn.FirstOrDefault(p => p.id == productionLot.id_Turn);
                            modelItem.id_personProcessPlant = item.id_personProcessPlant;

                            modelItem.receptionDate = item.receptionDate;
                            modelItem.expirationDate = item.expirationDate;

                            #region UPDATE ITEMS DETAILS

                            for (int i = modelItem.ProductionLotDetail.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.ProductionLotDetail.ElementAt(i);

                                for (int j = detail.ProductionLotDetailQualityControl.Count - 1; j >= 0; j--)
                                {
                                    var detailProductionLotDetailQualityControl = detail.ProductionLotDetailQualityControl.ElementAt(j);

                                    if (productionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id == detail.id) == null)
                                    {
                                        var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

                                        var qualityControlAux = detailProductionLotDetailQualityControl.QualityControl;
                                        if (qualityControlAux != null)
                                        {
                                            qualityControlAux.Document.id_documentState = documentState.id;
                                            qualityControlAux.Document.DocumentState = documentState;

                                            db.QualityControl.Attach(qualityControlAux);
                                            db.Entry(qualityControlAux).State = EntityState.Modified;
                                        }
                                    }

                                    detail.ProductionLotDetailQualityControl.Remove(detailProductionLotDetailQualityControl);
                                    db.Entry(detailProductionLotDetailQualityControl).State = EntityState.Deleted;
                                }

                                modelItem.ProductionLotDetail.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            if (productionLot.ProductionLotDetail != null)
                            {
                                int index = 1;
                                Lot _lot = db.Lot.FirstOrDefault(r => r.id == item.id);

                                var lotDetailGroupList = productionLot
                                                    .ProductionLotDetail
                                                    .ToList()
                                                    .GroupBy(r => r.id_originLot);


                                var lotDetailGroup = lotDetailGroupList.Select(r => new
                                                    {
                                                        idLot = r.Key.Value,
                                                        qty = r.Sum(t => t.quantityRecived)
                                                    }).ToList();

                                foreach (var detail in lotDetailGroup)
                                {
                                    ProductionLot productionLotOriginDetail = ServicePoductionLot
                                                                                            .GetOriginProductionLotForProductProcess
                                                                                            (db,
                                                                                            ActiveCompany,
                                                                                            ActiveUser,
                                                                                            detail.idLot,
                                                                                            _lot.number,
                                                                                            item.id_productionProcess,
                                                                                            item.receptionDate,
                                                                                            item.expirationDate,
                                                                                            detail.qty,
                                                                                            index,
                                                                                            item.id_personRequesting);
                                    index++;
                                }

                                foreach (var detail in productionLot.ProductionLotDetail)
                                {
                                    var newDetail = new ProductionLotDetail
                                    {
                                        id_productionLot = modelItem.id,
                                        id_item = detail.id_item,
                                        Item = itemList.FirstOrDefault(i => i.id == detail.id_item),
                                        id_originLot = detail.id_originLot,
                                        id_warehouse = detail.id_warehouse,
                                        Warehouse = warehouse.FirstOrDefault(i => i.id == detail.id_warehouse),
                                        id_warehouseLocation = detail.id_warehouseLocation,
                                        WarehouseLocation = warehouseLocation.FirstOrDefault(i => i.id == detail.id_warehouseLocation),
                                        quantityRecived = detail.quantityRecived,
                                        lotMarked = detail.lotMarked
                                    };

                                    newDetail.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();
                                    foreach (var productionLotDetailQualityControl in detail.ProductionLotDetailQualityControl)
                                    {
                                        var newProductionLotDetailQualityControl = new ProductionLotDetailQualityControl
                                        {
                                            id_productionLotDetail = productionLotDetailQualityControl.id_productionLotDetail,
                                            ProductionLotDetail = newDetail,
                                            id_qualityControl = productionLotDetailQualityControl.id_qualityControl,
                                            QualityControl = db.QualityControl.FirstOrDefault(i => i.id == productionLotDetailQualityControl.id_qualityControl),
                                        };
                                        newDetail.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);
                                        newProductionLotDetailQualityControl.QualityControl.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);
                                        newProductionLotDetailQualityControl.QualityControl.id_lot = modelItem.id;
                                        newProductionLotDetailQualityControl.QualityControl.Lot = modelItem.Lot;
                                    }

                                    modelItem.ProductionLotDetail.Add(newDetail);
                                }
                            }

                            if (modelItem.ProductionLotDetail.Count == 0)
                            {
                                TempData.Keep("productionLotProcess");
                                ViewData["EditMessage"] = ErrorMessage("No se puede guardar un lote sin detalles de Materia Prima");

                                modelItem.julianoNumber = strJul;
                                modelItem.internalNumber = strInt;
                                modelItem.internalNumberConcatenated = $"{strJul}{strInt}";

                                return PartialView("_ProductionLotProcessEditFormPartial", modelItem);
                            }

                            #endregion UPDATE ITEMS DETAILS

                            UpdateProductionLotTotals(modelItem);
                        }

                        if (modelItem.ProductionLotState.code == "02")
                        {
                            modelItem.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "03");


                            if (setting_liquidacionValorizada == "SI")
                            {
                                ViewBag.isRequestCarMachine = isRequestCarMachine;

                                if (isRequestCarMachine)
                                {
                                
                                    int idMachineForProd = (productionLotMachineTurn?.idMachineForProd ?? 0);
                                    int id_MachineProdOpening = (productionLotMachineTurn?.idMachineProdOpening ?? 0);
                                    ViewBag.id_MachineProdOpeningDetailInitLiqNoVal = db.MachineProdOpeningDetail
                                                                                            .FirstOrDefault(fod => fod.id_MachineForProd == idMachineForProd
                                                                                                                    && fod.id_MachineProdOpening == id_MachineProdOpening)?
                                                                                            .MachineForProd?.id;

                                    ViewBag.productionLotMachineTurn = (productionLotMachineTurn ?? new ProductionLotMachineTurn());
                                    isSaveLiqNoVal = true;
                                }
                            }
                    }

                        if (modelItem.ProductionLotState.code == "03")
                        {
                            modelItem.liquidationDate = DateTime.Now;

                            #region UPDATE PRODUCTION LOT LIQUIDATIONS DETAILS

                            for (int i = modelItem.ProductionLotLiquidation.Count - 1; i >= 0; i--)
                        {
                                var detail = modelItem.ProductionLotLiquidation.ElementAt(i);

                                for (int j = detail.ProductionLotLiquidationPackingMaterialDetail.Count - 1; j >= 0; j--)
                                {
                                    var detailProductionLotLiquidationPackingMaterialDetail = detail.ProductionLotLiquidationPackingMaterialDetail.ElementAt(j);
                                    detail.ProductionLotLiquidationPackingMaterialDetail.Remove(detailProductionLotLiquidationPackingMaterialDetail);
                                    db.Entry(detailProductionLotLiquidationPackingMaterialDetail).State = EntityState.Deleted;
                                }

                                for (int j = detail.InventoryMoveDetailEntryProductionLotLiquidation.Count - 1; j >= 0; j--)
                                {
                                    var detailInventoryMoveDetailEntryProductionLotLiquidation = detail.InventoryMoveDetailEntryProductionLotLiquidation.ElementAt(j);
                                    detail.InventoryMoveDetailEntryProductionLotLiquidation.Remove(detailInventoryMoveDetailEntryProductionLotLiquidation);
                                    db.Entry(detailInventoryMoveDetailEntryProductionLotLiquidation).State = EntityState.Deleted;
                                }

                                modelItem.ProductionLotLiquidation.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            for (int i = modelItem.ProductionLotPackingMaterial.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.ProductionLotPackingMaterial.ElementAt(i);
                                modelItem.ProductionLotPackingMaterial.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            if (productionLot.ProductionLotLiquidation != null)
                            {
                                foreach (var detail in productionLot.ProductionLotLiquidation)
                                {
                                    var newDetail = new ProductionLotLiquidation
                                    {
                                        id_productionLot = modelItem.id,
                                        id_item = detail.id_item,
                                        Item = itemList.FirstOrDefault(fod => fod.id == detail.id_item),
                                        id_warehouse = detail.id_warehouse,
                                        Warehouse = warehouse.FirstOrDefault(fod => fod.id == detail.id_warehouse),
                                        id_warehouseLocation = detail.id_warehouseLocation,
                                        WarehouseLocation = warehouseLocation.FirstOrDefault(fod => fod.id == detail.id_warehouseLocation),
                                        id_salesOrder = detail.id_salesOrder,
                                        SalesOrder = db.SalesOrder.FirstOrDefault(fod => fod.id == detail.id_salesOrder),
                                        id_salesOrderDetail = detail.id_salesOrderDetail,
                                        SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod => fod.id == detail.id_salesOrderDetail),
                                        quantity = detail.quantity,
                                        id_metricUnit = detail.id_metricUnit,
                                        MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == detail.id_metricUnit),
                                        quantityTotal = detail.quantityTotal,
                                        quantityPoundsLiquidation = detail.quantityPoundsLiquidation,
                                        distributionPercentage = detail.distributionPercentage,
                                        id_metricUnitPresentation = detail.id_metricUnitPresentation,
                                        MetricUnit1 = db.MetricUnit.FirstOrDefault(fod => fod.id == detail.id_metricUnitPresentation),
                                        id_productionCart = detail.id_productionCart
                                    };

                                    foreach (var detailPackingMaterialDetail in detail.ProductionLotLiquidationPackingMaterialDetail)
                                    {
                                        var newDetailPackingMaterial = modelItem.ProductionLotPackingMaterial.FirstOrDefault(d => d.id == detailPackingMaterialDetail.id_productionLotPackingMaterial);
                                        if (newDetailPackingMaterial == null)
                                        {
                                            newDetailPackingMaterial = new ProductionLotPackingMaterial
                                            {
                                                id = detailPackingMaterialDetail.id_productionLotPackingMaterial,
                                                id_productionLot = modelItem.id,
                                                id_item = detailPackingMaterialDetail.ProductionLotPackingMaterial.id_item,
                                                Item = itemList.FirstOrDefault(fod => fod.id == detailPackingMaterialDetail.ProductionLotPackingMaterial.id_item),
                                                quantity = detailPackingMaterialDetail.ProductionLotPackingMaterial.quantity,
                                                quantityRequiredForProductionLot = detailPackingMaterialDetail.ProductionLotPackingMaterial.quantityRequiredForProductionLot,
                                                isActive = true,
                                                manual = false,
                                                id_userCreate = ActiveUser.id,
                                                dateCreate = DateTime.Now,
                                                id_userUpdate = ActiveUser.id
                                            };
                                            newDetailPackingMaterial.dateUpdate = newDetailPackingMaterial.dateCreate;

                                            modelItem.ProductionLotPackingMaterial.Add(newDetailPackingMaterial);
                                        }

                                        var newProductionLotLiquidationPackingMaterialDetail = new ProductionLotLiquidationPackingMaterialDetail
                                        {
                                            id_productionLotLiquidation = newDetail.id,
                                            ProductionLotLiquidation = newDetail,
                                            id_productionLotPackingMaterial = newDetailPackingMaterial.id,
                                            ProductionLotPackingMaterial = newDetailPackingMaterial,
                                            quantity = detailPackingMaterialDetail.quantity
                                        };
                                        newDetail.ProductionLotLiquidationPackingMaterialDetail.Add(newProductionLotLiquidationPackingMaterialDetail);
                                        newDetailPackingMaterial.ProductionLotLiquidationPackingMaterialDetail.Add(newProductionLotLiquidationPackingMaterialDetail);
                                    }

                                    modelItem.ProductionLotLiquidation.Add(newDetail);
                                }
                            }

                            if (isRequestCarMachine && productionLotMachineTurn != null)
                            {
                                if (productionLotMachineTurn.idMachineForProd == 0 )
                            {
                                TempData.Keep("productionLotProcess");
                                ViewData["EditMessage"] = ErrorMessage("Debe seleccionar una Máquina");
                                modelItem.julianoNumber = strJul;
                                modelItem.internalNumber = strInt;
                                modelItem.internalNumberConcatenated = $"{strJul}{strInt}";
                                return PartialView("_ProductionLotProcessEditFormPartial", modelItem);
                            }
                            
                                var machineProdOpeningDetailAux = db.MachineProdOpeningDetail
                                    .Where(w => w.MachineProdOpening.Document.DocumentState.code == "03"
                                    && w.id == productionLotMachineTurn.idMachineProdOpeningDetail
                                    && w.MachineForProd.available).FirstOrDefault();

                                var turn = db.Turn.Where(t => t.id == productionLotMachineTurn.idTurn).FirstOrDefault();

                                if (productionLotMachineTurn.timeInit < machineProdOpeningDetailAux.timeInit)
                            {
                                TempData.Keep("productionLotProcess");
                                ViewData["EditMessage"] = ErrorMessage("La hora inicio debe ser mayor a la hora de la Máquina");
                                modelItem.julianoNumber = strJul;
                                modelItem.internalNumber = strInt;
                                modelItem.internalNumberConcatenated = $"{strJul}{strInt}";
                                return PartialView("_ProductionLotProcessEditFormPartial", modelItem);
                            }

                                if ((productionLotMachineTurn.timeEnd < productionLotMachineTurn.timeInit) ||
                                    (productionLotMachineTurn.timeEnd > turn.timeEnd))
                                {
                                    TempData.Keep("productionLotProcess");
                                    ViewData["EditMessage"] = ErrorMessage("La hora fin debe ser mayor a la hora inicio de la Máquina");
                                    modelItem.julianoNumber = strJul;
                                    modelItem.internalNumber = strInt;
                                    modelItem.internalNumberConcatenated = $"{strJul}{strInt}";
                                    return PartialView("_ProductionLotProcessEditFormPartial", modelItem);
                                }
                            
                                if (productionLotMachineTurn.timeEnd > turn.timeEnd)
                            {
                                TempData.Keep("productionLotProcess");
                                ViewData["EditMessage"] = ErrorMessage("La hora fin de la máquina no debe ser mayor a la hora fin del turno");
                                modelItem.julianoNumber = strJul;
                                modelItem.internalNumber = strInt;
                                modelItem.internalNumberConcatenated = $"{strJul}{strInt}";
                                return PartialView("_ProductionLotProcessEditFormPartial", modelItem);
                            }

                            }

                            if (modelItem.ProductionLotLiquidation.Count == 0)
                            {
                                TempData.Keep("productionLotProcess");
                                ViewData["EditMessage"] = ErrorMessage("No se puede guardar un lote sin detalles de Liquidación");

                                modelItem.julianoNumber = strJul;
                                modelItem.internalNumber = strInt;
                                modelItem.internalNumberConcatenated = $"{strJul}{strInt}";

                                return PartialView("_ProductionLotProcessEditFormPartial", modelItem);
                            }

                            #endregion UPDATE PRODUCTION LOT LIQUIDATIONS DETAILS

                            #region UPDATE PRODUCTION LOT PACKING MATERIAL

                            if (productionLot.ProductionLotPackingMaterial != null)
                            {
                                foreach (var detail in productionLot.ProductionLotPackingMaterial)
                                {
                                    if ((detail.ProductionLotLiquidationPackingMaterialDetail?.Count() ?? 0) > 0)
                                    {
                                        continue;
                                    }

                                    var newDetailPackingMaterial = new ProductionLotPackingMaterial
                                    {
                                        id = detail.id,
                                        id_productionLot = modelItem.id,
                                        id_item = detail.id_item,
                                        Item = itemList.FirstOrDefault(fod => fod.id == detail.id_item),
                                        quantity = detail.quantity,
                                        quantityRequiredForProductionLot = 0,
                                        isActive = true,
                                        manual = true,
                                        id_userCreate = ActiveUser.id,
                                        dateCreate = DateTime.Now,
                                        id_userUpdate = ActiveUser.id
                                    };
                                    newDetailPackingMaterial.dateUpdate = newDetailPackingMaterial.dateCreate;

                                    modelItem.ProductionLotPackingMaterial.Add(newDetailPackingMaterial);
                                }
                            }

                            #endregion UPDATE PRODUCTION LOT PACKING MATERIAL

                            #region UPDATE PRODUCTION LOT TRASHS DETAILS

                            for (int i = modelItem.ProductionLotTrash.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.ProductionLotTrash.ElementAt(i);
                                modelItem.ProductionLotTrash.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            if (productionLot.ProductionLotTrash != null)
                            {
                                foreach (var detail in productionLot.ProductionLotTrash)
                                {
                                    var newDetail = new ProductionLotTrash
                                    {
                                        id_productionLot = modelItem.id,
                                        id_item = detail.id_item,
                                        id_metricUnit = detail.id_metricUnit,
                                        MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == detail.id_metricUnit),
                                        id_warehouse = detail.id_warehouse,
                                        id_warehouseLocation = detail.id_warehouseLocation,

                                        quantity = detail.quantity
                                    };

                                    modelItem.ProductionLotTrash.Add(newDetail);
                                }
                            }

                            #endregion UPDATE PRODUCTION LOT TRASHS DETAILS

                            #region UPDATE PRODUCTION LOT LOSS DETAILS

                            for (int i = modelItem.ProductionLotLoss.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.ProductionLotLoss.ElementAt(i);
                                modelItem.ProductionLotLoss.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            if (productionLot.ProductionLotLoss != null)
                            {
                                foreach (var detail in productionLot.ProductionLotLoss)
                                {
                                    var newDetail = new ProductionLotLoss
                                    {
                                        id_productionLot = modelItem.id,
                                        id_item = detail.id_item,
                                        id_metricUnit = detail.id_metricUnit,
                                        MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == detail.id_metricUnit),
                                        id_warehouse = detail.id_warehouse,
                                        id_warehouseLocation = detail.id_warehouseLocation,

                                        quantity = detail.quantity
                                    };

                                    modelItem.ProductionLotLoss.Add(newDetail);
                                }
                            }

                            #endregion UPDATE PRODUCTION LOT LOSS DETAILS

                            UpdateProductionLotProductionLotLiquidationsDetailTotals(modelItem);
                            UpdateProductionLotProductionLotTrashsDetailTotals(modelItem);
                            UpdateProductionLotProductionLotLossDetailTotals(modelItem);
                        }

                        if (modelItem.ProductionLotState.code == "04")
                        {
                            modelItem.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "05");
                        }

                        if (modelItem.ProductionLotState.code == "05" || modelItem.ProductionLotState.code == "07")
                        {
                        }
                        if (modelItem.ProductionLotState.code == "06")
                        {
                            modelItem.id_wareHouseTransfer = productionLot.id_wareHouseTransfer;
                            modelItem.Warehouse = warehouse.FirstOrDefault(p => p.id == productionLot.id_wareHouseTransfer);
                            modelItem.id_wareHouseLocationTransfer = productionLot.id_wareHouseLocationTransfer;
                            modelItem.WarehouseLocation = warehouseLocation.FirstOrDefault(p => p.id == productionLot.id_wareHouseLocationTransfer);
                            modelItem.id_dischargeReason = productionLot.id_dischargeReason;
                            modelItem.InventoryReason = db.InventoryReason.FirstOrDefault(p => p.id == productionLot.id_dischargeReason);
                            modelItem.id_incomeReason = productionLot.id_incomeReason;
                            modelItem.InventoryReason1 = db.InventoryReason.FirstOrDefault(p => p.id == productionLot.id_incomeReason);

                            if (productionLot.ProductionLotLiquidation != null)
                            {
                                var details = productionLot.ProductionLotLiquidation.ToList();
                                foreach (var detail in details)
                                {
                                    ProductionLotLiquidation productionLotLiquidation = modelItem.ProductionLotLiquidation.FirstOrDefault(d => d.id == detail.id);
                                    if (productionLotLiquidation != null)
                                    {
                                        productionLotLiquidation.id_wareHouseDetailTransfer = detail.id_wareHouseDetailTransfer;
                                        productionLotLiquidation.id_wareHouseLocationDetailTransfer = detail.id_wareHouseLocationDetailTransfer;

                                        db.Entry(productionLotLiquidation).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }

                        if (approve)
                        {
                            if (modelItem.ProductionLotState.code == "01")
                            {
                                ServiceInventoryMove
                                        .UpdateInventaryMoveExitRawMaterialProcess(ActiveUser,
                                                                                    ActiveCompany,
                                                                                    ActiveEmissionPoint,
                                                                                    modelItem,
                                                                                    db,
                                                                                    false,
                                                                                    null,
                                                                                    process.id_CostCenter,
                                                                                    process.id_SubCostCenter);
                                modelItem.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "02");

                                var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");
                                modelItem.Lot.Document.id_documentState = documentState.id;
                                modelItem.Lot.Document.DocumentState = documentState;
                            }
                            if (modelItem.ProductionLotState.code == "03")
                            {
                                var id_productionLotAux = modelItem.id;
                                var inventoryMoves = db.InventoryMove.Where(w => w.id_productionLot == id_productionLotAux &&
                                                                                 w.InventoryReason.code.Equals("EMPP")).OrderByDescending(d => d.Document.dateCreate).ToList();
                                InventoryMove lastInventoryMoveEMPP = (inventoryMoves.Count > 0)
                                                                ? inventoryMoves.First()
                                                                : null;
                                decimal priceXLbsAux = 0;
                                if (lastInventoryMoveEMPP != null)
                                {
                                    priceXLbsAux = lastInventoryMoveEMPP.InventoryMoveDetail.Sum(s => s.exitAmountCost) / modelItem.totalQuantityLiquidation;
                                }

                                ServiceInventoryMove
                                        .UpdateInventaryMoveEntryLiquidationPlant(ActiveUser,
                                                                                    ActiveCompany,
                                                                                    ActiveEmissionPoint,
                                                                                    modelItem,
                                                                                    db,
                                                                                    false,
                                                                                    null,
                                                                                    priceXLbsAux,
                                                                                    process.id_CostCenter,
                                                                                    process.id_SubCostCenter);
                                if (modelItem.ProductionLotTrash.Count > 0)
                                {
                                    ServiceInventoryMove.UpdateInventaryMoveEntryTrash(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem, db, false);
                                }
                                if (modelItem.ProductionLotLoss.Count > 0)
                                {
                                    ServiceInventoryMove.UpdateInventaryMoveEntryLoss(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem, db, false);
                                }
                                modelItem.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "04");
                            }
                            var _productionLotStatate = modelItem.ProductionLotState.code;
                            if (modelItem.ProductionLotState.code == "05")
                            {
                                modelItem.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "06");
                            }
                            if (_productionLotStatate == "06")
                            {
                                if (modelItem.ProductionLotLiquidation.Count > 0)
                                {
                                    ServiceInventoryMove.UpdateInventaryMoveEntryLiqudation(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem, db, false);
                                    ServiceInventoryMove.UpdateInventaryMoveExitLiqudation(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem, db, false);
                                }
                                modelItem.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "10");
                            }
                        }

                        //db.ProductionLot.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                    
                        using (DbContextTransaction trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                
                                db.SaveChanges();

                                if (isSaveLiqNoVal)
                                {

                                    DbTransaction transaction = trans.UnderlyingTransaction;
                                    SqlConnection connection = transaction.Connection as SqlConnection;

                                    ProductionLotMachineTurn productionLotMachineTurnCurrent = ProductionLotMachineTurn.GetOneByProductionLot(productionLot.id);

                                    if (productionLotMachineTurnCurrent == null)
                                    {

                                        productionLotMachineTurn.id = modelItem.id;
                                        productionLotMachineTurn.dateCreate = DateTime.Now;
                                        productionLotMachineTurn.id_userCreate = ActiveUserId;
                                        ProductionLotMachineTurn.InsertProductionLotMachineTurn(connection, transaction, productionLotMachineTurn);

                                    }
                                    else
                                    {
                                        productionLotMachineTurn.id = productionLotMachineTurnCurrent.id;
                                        productionLotMachineTurn.dateUpdate = DateTime.Now;
                                        productionLotMachineTurn.id_userUpdate = ActiveUserId;
                                        ProductionLotMachineTurn.UpdateProductionLotMachineTurn(connection, transaction, productionLotMachineTurn);
                                    }
                                }
                                trans.Commit();
                                ViewData["EditMessage"] = SuccessMessage("Lote: " + modelItem.number + " guardado exitosamente");
                                
                            }
                            catch(Exception e)
                            {
                                trans.Rollback();
                                FullLog(e);
                            }
                    
                        }
                    }
                    catch (Exception e)
                    {
                        TempData["productionLotProcess"] = productionLot;
                        TempData.Keep("productionLotProcess");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        FullLog(e);

                        //try
                        //{
                        //    var innerMensaje = string.Empty;
                        //    var inner3 = string.Empty;
                        //
                        //    string errorData = string.Empty;
                        //    foreach (DictionaryEntry data in e.Data)
                        //    {
                        //        errorData += $"{data.Key.ToString()}: {data.Value.ToString()} {Environment.NewLine}";
                        //    }
                        //
                        //    string errorDataInner = string.Empty;
                        //    if (e.InnerException != null)
                        //    {
                        //        innerMensaje = e.InnerException.Message;
                        //        foreach (DictionaryEntry data in e.InnerException.Data)
                        //        {
                        //            errorDataInner += $"{data.Key.ToString()}: {data.Value.ToString()} {Environment.NewLine}";
                        //        }
                        //        if (e.InnerException.InnerException != null)
                        //        {
                        //            inner3 = e.InnerException.InnerException.Message;
                        //        }
                        //    }
                        //
                        //    var mensaje = $"Mensaje:{e.Message}{Environment.NewLine} .Error:{errorData}{Environment.NewLine}  .InnerExc:{innerMensaje}{Environment.NewLine}{errorDataInner}{Environment.NewLine} .Level3:{inner3} {Environment.NewLine} empleadoid{empleadoId}";
                        //    MetodosEscrituraLogs.EscribeMensajeLog(mensaje, _rutaLog, "PROCESOSINTERNOS", "PROD");
                        //}
                        //catch
                        //{ }

                        productionLot.julianoNumber = strJul;
                        productionLot.internalNumber = strInt;
                        productionLot.internalNumberConcatenated = $"{strJul}{strInt}";

                        return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                    }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            using (DbContextTransaction trans2 = db.Database.BeginTransaction())
            {
                try
                {
                    if (approve && (modelItem.ProductionLotState.code == "04"))
                    {
                        var resultService = ServiceInventoryMove.UpdateInventaryMoveExitPackingMaterial(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem, db, false);
                        if (resultService != "")
                        {
                            ViewData["EditMessage"] = WarningMessage("Lote: " + modelItem.number + " guardado exitosamente. Pero: " + resultService);
                            trans2.Rollback();
                        }
                        else
                        {
                            db.SaveChanges();
                            trans2.Commit();
                        }
                    }
                }
                catch (Exception e)
                {
                    TempData["productionLotProcess"] = modelItem;
                    TempData.Keep("productionLotProcess");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans2.Rollback();

                    //try
                    //{
                    //    var innerMensaje = string.Empty;
                    //    var inner3 = string.Empty;
                    //
                    //    string errorData = string.Empty;
                    //    foreach (DictionaryEntry data in e.Data)
                    //    {
                    //        errorData += $"{data.Key.ToString()}: {data.Value.ToString()} {Environment.NewLine}";
                    //    }
                    //
                    //    string errorDataInner = string.Empty;
                    //    if (e.InnerException != null)
                    //    {
                    //        innerMensaje = e.InnerException.Message;
                    //        foreach (DictionaryEntry data in e.InnerException.Data)
                    //        {
                    //            errorDataInner += $"{data.Key.ToString()}: {data.Value.ToString()} {Environment.NewLine}";
                    //        }
                    //        if (e.InnerException.InnerException != null)
                    //        {
                    //            inner3 = e.InnerException.InnerException.Message;
                    //        }
                    //    }
                    //
                    //    var mensaje = $"Mensaje:{e.Message}{Environment.NewLine} .Error:{errorData}{Environment.NewLine}  .InnerExc:{innerMensaje}{Environment.NewLine}{errorDataInner}{Environment.NewLine} .Level3:{inner3}";
                    //    MetodosEscrituraLogs.EscribeMensajeLog(mensaje, _rutaLog, "PROCESOSINTERNOS", "PROD");
                    //}
                    //catch
                    //{ }

                    productionLot.julianoNumber = strJul;
                    productionLot.internalNumber = strInt;
                    productionLot.internalNumberConcatenated = $"{strJul}{strInt}";

                    FullLog(e);

                    return PartialView("_ProductionLotProcessEditFormPartial", modelItem);
                }
            }

            var stateLot = modelItem.ProductionLotState.code;
            if (stateLot != "01")
            {
                var natureMoves = db.AdvanceParametersDetail.Where(r => r.valueCode.Trim() == "I" || r.valueCode.Trim() == "E").ToList();
                var inventoryMoves = db.InventoryMove.Where(r => r.id_productionLot == modelItem.id && r.Document.DocumentState.code != "05").ToList();
                var natureMoveEgreso = natureMoves.FirstOrDefault(r => r.valueCode.Trim() == "E");
                inventoryMoveExit = inventoryMoves.FirstOrDefault(r => r.idNatureMove == natureMoveEgreso.id)?.id ?? 0;
                if (stateLot != "02" && stateLot != "03")
                {
                    var natureMoveIngreso = natureMoves.FirstOrDefault(r => r.valueCode.Trim() == "I");
                    inventoryMoveEntry = inventoryMoves.FirstOrDefault(r => r.idNatureMove == natureMoveIngreso.id)?.id ?? 0;
                }

                if (solicitaMaquina == "SI" && productionLot.ProductionProcess?.requestliquidationmachine == true)
                {
                    ViewBag.solicitaMaquina = true;
                }
                else
                {
                    ViewBag.solicitaMaquina = false;
                }
                if (solicitaMaquina == "SI" && productionLot.ProductionProcess?.generateTransfer == true)
                {
                    ViewBag.generaTransferencia = true;
                }
                else
                {
                    ViewBag.generaTransferencia = false;
                }

                #region Liquiacion Valorizada - 6115
                
                ViewBag.isRequestCarMachine = isRequestCarMachine;
                if (isRequestCarMachine)
                {
                    productionLotMachineTurn = productionLotMachineTurn ?? ProductionLotMachineTurn.GetOneByProductionLot(productionLot.id);

                    int idMachineForProd = (productionLotMachineTurn?.idMachineForProd ?? 0);
                    int id_MachineProdOpening = (productionLotMachineTurn?.idMachineProdOpening ?? 0);
                    ViewBag.id_MachineProdOpeningDetailInitLiqNoVal = db.MachineProdOpeningDetail
                                                                            .FirstOrDefault(fod => fod.id_MachineForProd == idMachineForProd
                                                                                                    && fod.id_MachineProdOpening == id_MachineProdOpening)?
                                                                            .MachineForProd?.id;


                    ViewBag.productionLotMachineTurn = (productionLotMachineTurn ?? new ProductionLotMachineTurn());
                }

                #endregion
            }
            ViewBag.codeStateLiqNoval = modelItem.ProductionLotState.code;
            BuildViewDataEdit();
            TempData["productionLotProcess"] = modelItem;
            TempData.Keep("productionLotProcess");

            ViewBag.imoveEntry = inventoryMoveEntry;
            ViewBag.imovExit = inventoryMoveExit;

            modelItem.julianoNumber = strJul;
            modelItem.internalNumber = strInt;
            modelItem.internalNumberConcatenated = $"{strJul}{strInt}";

            return PartialView("_ProductionLotProcessEditFormPartial", modelItem);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetDataMachineForProd(int? id_MachineForProd, int? id_MachineProdOpening)
        {
            var machineProdOpeningDetail = db.MachineProdOpeningDetail
                                                .FirstOrDefault(fod => fod.id_MachineProdOpening == id_MachineProdOpening
                                                && fod.id_MachineForProd == id_MachineForProd);

            var result = new
            {
                Message = "Ok",
                id_MachineForProd = machineProdOpeningDetail?.id_MachineForProd,
                id_MachineProdOpening = machineProdOpeningDetail?.id_MachineProdOpening,
                nameTurno = machineProdOpeningDetail?.MachineProdOpening.Turn?.name,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class ComboBoxMachinesProdOpeningModel
        {
            public int? id_MachineForProd { get; set; }
            public int? id_MachineProdOpening { get; set; }
            public string documentStateCode { get; set; }
            public DateTime? emissionDate { get; set; }
            public int? id_PersonProcessPlant { get; set; }
            public bool itemsReceivedTunnels { get; set; }
            public int? id_Turn { get; set; }
        }

        [HttpPost]
        public ActionResult GetMachinesForProdOpening(
            int? id_MachineForProd, int? id_MachineProdOpening, string documentStateCode, DateTime? emissionDate, int? id_PersonProcessPlant, int id_Turn)
        {
            var model = new ComboBoxMachinesProdOpeningModel()
            {
                id_MachineForProd = id_MachineForProd,
                id_MachineProdOpening = id_MachineProdOpening,
                documentStateCode = documentStateCode,
                emissionDate = emissionDate,
                id_PersonProcessPlant = id_PersonProcessPlant,
                id_Turn = id_Turn,
            };

            return PartialView("ProducionComboBox/_ComboBoxMachinesProdOpening", model);
        }

        #endregion COMMON - PROCESS

        #region SINGLE CHANGE PRODUCTION LOT STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "02");
                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        this.ActualizarRegistroRomaneo(productionLot);

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    if (solicitaMaquina == "SI" && productionLot.ProductionProcess.requestliquidationmachine == true)
                    {
                        ViewBag.solicitaMaquina = true;
                    }
                    else
                    {
                        ViewBag.solicitaMaquina = false;
                    }
                    if (solicitaMaquina == "SI" && productionLot.ProductionProcess.generateTransfer == true)
                    {
                        ViewBag.generaTransferencia = true;
                    }
                    else
                    {
                        ViewBag.generaTransferencia = false;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " aprobado exitosamente");

            return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "04");

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        this.ActualizarRegistroRomaneo(productionLot);

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

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

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " autorizado exitosamente");

            return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
        }

        [HttpPost]
        public ActionResult Protect(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "05");

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        this.ActualizarRegistroRomaneo(productionLot);

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

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

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " cerrado exitosamente");

            return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewBag.isProductionProcess = "S";
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            string julianoNumber = productionLot.internalNumber.Trim().Substring(0, 5);
            string internalNumber = productionLot.internalNumber.Trim().Substring(5);
            string internalNumberConcatenated = productionLot.internalNumber;
            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.requestliquidationmachine == true)
            {
                ViewBag.solicitaMaquina = true;
            }
            else
            {
                ViewBag.solicitaMaquina = false;
            }
            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.generateTransfer == true)
            {
                ViewBag.generaTransferencia = true;
            }
            else
            {
                ViewBag.generaTransferencia = false;
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("EMPP")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                    var inventoryMoveDetailAuxi = productionLot.InventoryMove.Where(w => w.InventoryReason.code.Equals("EMPP")).OrderByDescending(d => d.Document.dateCreate).ToList();
                    InventoryMove lastInventoryMoveEMPP = (inventoryMoveDetailAuxi.Count > 0)
                                                                        ? inventoryMoveDetailAuxi.First()
                                                                        : null;
                    ServiceInventoryMove.UpdateInventaryMoveExitRawMaterialProcess(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveEMPP);

                    if (productionLot.ProductionLotState.code != "01" && productionLot.ProductionLotState.code != "02" &&
                        productionLot.ProductionLotState.code != "03")
                    {
                        inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("ILP")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                        InventoryMove lastInventoryMoveILP = (inventoryMoveDetailAux.Count > 0)
                                                                            ? inventoryMoveDetailAux.First().InventoryMove
                                                                            : null;
                        ServiceInventoryMove.UpdateInventaryMoveEntryLiquidationPlant(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveILP);
                        if (productionLot.ProductionLotTrash.Count > 0)
                        {
                            inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("IDE")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveIDE = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().InventoryMove
                                                                : null;
                            ServiceInventoryMove.UpdateInventaryMoveEntryTrash(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveIDE);
                        }

                        if (productionLot.ProductionLotLoss.Count > 0)
                        {
                            inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("IPM")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveIDE = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().InventoryMove
                                                                : null;
                            ServiceInventoryMove.UpdateInventaryMoveEntryLoss(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveIDE);
                        }

                        var existInInventoryMoveDetailExitPackingMaterial = productionLot.ProductionLotPackingMaterial.FirstOrDefault(fod => fod.InventoryMoveDetailExitPackingMaterial.Count() > 0);

                        if (existInInventoryMoveDetailExitPackingMaterial != null)
                        {
                            var existInInventoryMoveEME = existInInventoryMoveDetailExitPackingMaterial.InventoryMoveDetailExitPackingMaterial.FirstOrDefault().InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EME");
                            if (existInInventoryMoveEME)
                            {
                                TempData.Keep("productionLotProcess");
                                ViewData["EditMessage"] = ErrorMessage("No se puede anular el lote debido a tener egresos de materiales de empaque en inventario, manual.");

                                productionLot.internalNumber = internalNumber;
                                productionLot.julianoNumber = julianoNumber;
                                productionLot.internalNumberConcatenated = internalNumberConcatenated;
                                return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                            }
                        }

                        var inventoryMoveEMEA = productionLot.ProductionLotPackingMaterial.FirstOrDefault()?.InventoryMoveDetailExitPackingMaterial?.FirstOrDefault(fod => fod.InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EMEA"))?.InventoryMoveDetail.InventoryMove;
                        if (inventoryMoveEMEA != null)
                        {
                            ServiceInventoryMove.UpdateInventaryMoveExitPackingMaterial(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, inventoryMoveEMEA);
                        }
                    }

                    if (productionLot.ProductionLotState.code == "10")
                    {
                        if (productionLot.ProductionLotLiquidation.Count > 0)
                        {
                            var _documentTypeCodeEntry = db.InventoryReason.FirstOrDefault(t => t.id == productionLot.id_incomeReason).code;
                            var _documentTypeCodeExit = db.InventoryReason.FirstOrDefault(t => t.id == productionLot.id_dischargeReason).code;
                            inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals(_documentTypeCodeEntry)).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoventry = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals(_documentTypeCodeExit)).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveExit = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().InventoryMove
                                                                : null;
                            ServiceInventoryMove.UpdateInventaryMoveEntryLiqudation(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoventry);
                            ServiceInventoryMove.UpdateInventaryMoveExitLiqudation(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveExit);
                        }
                    }
                    var existInProductionLotDetail = db.ProductionLotDetail.FirstOrDefault(fod => fod.id_originLot == id && fod.ProductionLot.ProductionLotState.code != "09");

                    if (existInProductionLotDetail != null)
                    {
                        TempData.Keep("productionLotProcess");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular el lote debido a tener Lotes de Procesos que dependen de él.");

                        productionLot.internalNumber = internalNumber;
                        productionLot.julianoNumber = julianoNumber;
                        productionLot.internalNumberConcatenated = internalNumberConcatenated;
                        return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                    }

                    var existInProductionLotDailyCloseDetail = db.ProductionLotDailyCloseDetail.FirstOrDefault(fod => fod.id_productionLot == id && fod.ProductionLotDailyClose.Document.DocumentState.code != "05");

                    if (existInProductionLotDailyCloseDetail != null)
                    {
                        TempData.Keep("productionLotProcess");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular el lote debido a pertenecer a un cierre diario/turno.");

                        productionLot.internalNumber = internalNumber;
                        productionLot.julianoNumber = julianoNumber;
                        productionLot.internalNumberConcatenated = internalNumberConcatenated;
                        return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                    }

                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "09");

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

                        foreach (var detail in productionLot.ProductionLotDetail)
                        {
                            var qualityControlAux = detail.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.QualityControl.Document.DocumentState.code != ("05"))?.QualityControl;
                            if (qualityControlAux != null)
                            {
                                qualityControlAux.Document.id_documentState = documentState.id;
                                qualityControlAux.Document.DocumentState = documentState;

                                db.QualityControl.Attach(qualityControlAux);
                                db.Entry(qualityControlAux).State = EntityState.Modified;
                            }
                        }

                        productionLot.Lot.Document.id_documentState = documentState.id;
                        productionLot.Lot.Document.DocumentState = documentState;
                        this.ActualizarRegistroRomaneo(productionLot);

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("productionLotProcess");
                    ViewData["EditMessage"] = ErrorMessage("No se puede anular el lote debido a: " + e.Message);

                    productionLot.internalNumber = internalNumber;
                    productionLot.julianoNumber = julianoNumber;
                    productionLot.internalNumberConcatenated = internalNumberConcatenated;
                    return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                }
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " anulado exitosamente");

            productionLot.internalNumber = internalNumber;
            productionLot.julianoNumber = julianoNumber;
            productionLot.internalNumberConcatenated = internalNumberConcatenated;
            return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            bool isRequestCarMachine = false;

            ViewBag.isProductionProcess = "S";
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";

            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            var secJul = productionLot.internalNumber.Trim().Substring(0, 5);
            var secInt = productionLot.internalNumber.Trim().Substring(5);
            var internalN = productionLot.internalNumber;
            var documentStateList = db.DocumentState.Where(d => d.isActive).ToList();

            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.requestliquidationmachine == true)
            {
                ViewBag.solicitaMaquina = true;
            }
            else
            {
                ViewBag.solicitaMaquina = false;
            }
            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.generateTransfer == true)
            {
                ViewBag.generaTransferencia = true;
            }
            else
            {
                ViewBag.generaTransferencia = false;
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    //Verificar si existe lotes relacionados al proceso de cierre
                    var productionLotAux = db.ProductionLot.FirstOrDefault(a => a.id == id && a.isClosed);
                    if (productionLotAux != null)
                    {
                        var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.id_lot == id && a.Document.DocumentState.code != "05"
                                                && a.isActive);

                        if (productionLotClose != null && productionLotAux.receptionDate.Date <= productionLotClose.Document.emissionDate.Date 
                            && productionLotAux.ProductionLotState.code == "11")
                        {
                            throw new Exception("El lote " + productionLotAux.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                        }
                    }

                    var codeStateCurrent = productionLot.ProductionLotState.code;

                    #region Liquiacion Valorizada - 6115
                    string setting_liquidacionValorizada = (db.Setting.FirstOrDefault(r => r.code == "LIQNOVAL")?.value ?? "NO");
                    if (setting_liquidacionValorizada == "SI")
                    {
                        isRequestCarMachine = (ProductionProcess.GetOneById(productionLot.id_productionProcess)?.requestCarMachine ?? false);
                    }
                    ViewBag.isRequestCarMachine = isRequestCarMachine;

                    if (isRequestCarMachine)
                    {
                        ProductionLotMachineTurn productionLotMachineTurn = ProductionLotMachineTurn.GetOneByProductionLot(productionLot.id);

                        int idMachineForProd = (productionLotMachineTurn?.idMachineForProd ?? 0);
                        int id_MachineProdOpening = (productionLotMachineTurn?.idMachineProdOpening ?? 0);
                        ViewBag.id_MachineProdOpeningDetailInitLiqNoVal = db.MachineProdOpeningDetail
                                                                                .FirstOrDefault(fod => fod.id_MachineForProd == idMachineForProd
                                                                                                        && fod.id_MachineProdOpening == id_MachineProdOpening)?
                                                                                .MachineForProd?.id;


                        ViewBag.productionLotMachineTurn = (productionLotMachineTurn ?? new ProductionLotMachineTurn());
                    }

                    #endregion

                    if (codeStateCurrent == "02" || codeStateCurrent == "03")
                    {
                        var inventoryMoveDetailAux = productionLot.InventoryMove.Where(w => w.InventoryReason.code.Equals("EMPP")).OrderByDescending(d => d.Document.dateCreate).ToList();
                        InventoryMove lastInventoryMoveEMPP = (inventoryMoveDetailAux.Count > 0)
                                                                            ? inventoryMoveDetailAux.First()
                                                                            : null;
                        ServiceInventoryMove.UpdateInventaryMoveExitRawMaterialProcess(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveEMPP);

                        var itemDetail = productionLot.ProductionLotLiquidation.ToList();
                        foreach (var i in itemDetail)
                        {
                            var detailInventoryMoveDetailEntryProductionLotLiquidation = i.InventoryMoveDetailEntryProductionLotLiquidation.FirstOrDefault(fod => fod.id_productionLotLiquidation == i.id);
                            if (detailInventoryMoveDetailEntryProductionLotLiquidation != null)
                            {
                                i.InventoryMoveDetailEntryProductionLotLiquidation.Remove(detailInventoryMoveDetailEntryProductionLotLiquidation);
                                db.Entry(detailInventoryMoveDetailEntryProductionLotLiquidation).State = EntityState.Deleted;
                            }
                        }

                    }

                    if (codeStateCurrent == "04" || codeStateCurrent == "05")
                    {
                        var inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("ILP")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                        InventoryMove lastInventoryMoveILP = (inventoryMoveDetailAux.Count > 0)
                                                                            ? inventoryMoveDetailAux.First().InventoryMove
                                                                            : null;
                        ServiceInventoryMove.UpdateInventaryMoveEntryLiquidationPlant(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveILP);
                        if (productionLot.ProductionLotTrash.Count > 0)
                        {
                            inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("IDE")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveIDE = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().InventoryMove
                                                                : null;
                            ServiceInventoryMove.UpdateInventaryMoveEntryTrash(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveIDE);
                        }
                        if (productionLot.ProductionLotLoss.Count > 0)
                        {
                            inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("IPM")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveIDE = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().InventoryMove
                                                                : null;
                            ServiceInventoryMove.UpdateInventaryMoveEntryLoss(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveIDE);
                        }

                        var existInInventoryMoveDetailExitPackingMaterial = productionLot.ProductionLotPackingMaterial.FirstOrDefault(fod => fod.InventoryMoveDetailExitPackingMaterial.Count() > 0);

                        if (existInInventoryMoveDetailExitPackingMaterial != null)
                        {
                            var existInInventoryMoveEME = existInInventoryMoveDetailExitPackingMaterial.InventoryMoveDetailExitPackingMaterial.FirstOrDefault().InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EME");
                            if (existInInventoryMoveEME)
                            {
                                TempData.Keep("productionLotProcess");
                                ViewData["EditMessage"] = ErrorMessage("No se puede reversar el lote debido a tener egresos de materiales de empaque en inventario, manual.");

                                productionLot.internalNumber = secInt;
                                productionLot.julianoNumber = secJul;
                                productionLot.internalNumberConcatenated = internalN;

                                return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                            }
                        }

                        var inventoryMoveEMEA = productionLot.ProductionLotPackingMaterial.FirstOrDefault()?.InventoryMoveDetailExitPackingMaterial?.FirstOrDefault(fod => fod.InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EMEA"))?.InventoryMoveDetail.InventoryMove;
                        if (inventoryMoveEMEA != null)
                        {
                            ServiceInventoryMove.UpdateInventaryMoveExitPackingMaterial(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, inventoryMoveEMEA);
                        }

                        var existInProductionLotDetail = db.ProductionLotDetail.FirstOrDefault(fod => fod.id_originLot == id && fod.ProductionLot.ProductionLotState.code != "09");

                        if (existInProductionLotDetail != null)
                        {
                            TempData.Keep("productionLotProcess");
                            ViewData["EditMessage"] = ErrorMessage("No se puede reversar el lote debido a tener Lotes de Procesos " + existInProductionLotDetail.ProductionLot.number + " que dependen de él.");

                            productionLot.internalNumber = secInt;
                            productionLot.julianoNumber = secJul;
                            productionLot.internalNumberConcatenated = internalN;

                            return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                        }
                    }

                    if (codeStateCurrent == "06")
                    {
                        var existInProductionLotDailyCloseDetail = db.ProductionLotDailyCloseDetail.FirstOrDefault(fod => fod.id_productionLot == id && fod.ProductionLotDailyClose.Document.DocumentState.code != "05");

                        if (existInProductionLotDailyCloseDetail != null)
                        {
                            TempData.Keep("productionLotProcess");

                            ViewData["EditMessage"] = ErrorMessage("No se puede reversar el lote debido a pertenecer a un cierre diario/turno.");

                            productionLot.internalNumber = secInt;
                            productionLot.julianoNumber = secJul;
                            productionLot.internalNumberConcatenated = internalN;

                            return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                        }
                    }
                    if (codeStateCurrent == "10")
                    {
                        var inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("ILP")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();

                        if (productionLot.ProductionLotLiquidation.Count > 0)
                        {
                            var _documentTypeCodeEntry = db.InventoryReason.FirstOrDefault(t => t.id == productionLot.id_incomeReason).code;
                            var _documentTypeCodeExit = db.InventoryReason.FirstOrDefault(t => t.id == productionLot.id_dischargeReason).code;
                            inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals(_documentTypeCodeEntry)).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoventry = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals(_documentTypeCodeExit)).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                            InventoryMove lastInventoryMoveExit = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().InventoryMove
                                                                : null;
                            ServiceInventoryMove.UpdateInventaryMoveEntryLiqudation(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoventry);
                            ServiceInventoryMove.UpdateInventaryMoveExitLiqudation(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveExit);
                        }
                    }

                    ProductionLotState productionLotState = null;

                    if (codeStateCurrent == "06")
                    {
                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "05");
                    }

                    if (codeStateCurrent == "05" || codeStateCurrent == "04")
                    {
                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "03");
                    }

                    if (codeStateCurrent == "03" || codeStateCurrent == "02")
                    {
                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "01");
                        var documentState = documentStateList.FirstOrDefault(s => s.code == "01");
                        productionLot.Lot.Document.id_documentState = documentState.id;
                        productionLot.Lot.Document.DocumentState = documentState;
                    }
                    if (codeStateCurrent == "10")
                    {
                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "06");
                    }
                    if (codeStateCurrent == "11")//CONCILIADO
                    {
                        int id_user = (int)ViewData["id_user"];
                        int id_menu = (int)ViewData["id_menu"];

                        User user = DataProviderUser.UserById(id_user);
                        UserMenu userMenu = user.UserMenu.FirstOrDefault(m => m.Menu.id == id_menu);
                        if (userMenu != null)
                        {
                            Permission permission = userMenu.Permission.FirstOrDefault(p => p.name == "Conciliar");
                            if (permission == null)
                            {
                                throw new Exception("No tiene Permiso para Reversar en estado Conciliado");
                            }
                        }

                        // Actualizo los movimientos de inventario
                        #region Actualizo los movimientos de inventario
                        // Busco el estado CONCILIADO para Movimiento de inventario
                        DocumentState documentState = documentStateList.FirstOrDefault(fod => fod.code == "03");
                        if (documentState == null)
                            throw new Exception("Estado de documento  no encontrado");

                        var documentsSource = db.DocumentSource.Where(fod => fod.id_documentOrigin == productionLot.id && fod.Document.DocumentState.code != "05").ToList();
                     
                        // Recorre los documentos de origen y luegos los documentos de M.I.
                        foreach (var documentSource in documentsSource)
                        {
                            var document = db.Document.FirstOrDefault(e => e.id == documentSource.id_document);

                            if (document == null)
                                throw new Exception("Documento de movimiento de inventario no encontrado");

                            document.id_documentState = documentState.id;
                            document.id_userUpdate = this.ActiveUserId;
                            document.dateUpdate = DateTime.Now;

                            db.Document.Attach(document);
                            db.Entry(document).State = EntityState.Modified;
                        }

                        #endregion                        

                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "06");//CERRADO
                    }

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("productionLotProcess");
                    ViewData["EditMessage"] = ErrorMessage("No se puede reversar el lote debido a: " + e.Message);

                    productionLot.internalNumber = secInt;
                    productionLot.julianoNumber = secJul;
                    productionLot.internalNumberConcatenated = internalN;

                    return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
                }
            }
            
            TempData["productionLotProcess"] = productionLot;
            ViewBag.codeStateLiqNoval = productionLot.ProductionLotState.code;
            BuildViewDataEdit();
            TempData.Keep("productionLotProcess");

            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " reversado exitosamente");

            productionLot.internalNumber = secInt;
            productionLot.julianoNumber = secJul;
            productionLot.internalNumberConcatenated = internalN;

            return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
        }

        [HttpPost]
        public ActionResult Conciliation(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.requestliquidationmachine == true)
            {
                ViewBag.solicitaMaquina = true;
            }
            else
            {
                ViewBag.solicitaMaquina = false;
            }
            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.generateTransfer == true)
            {
                ViewBag.generaTransferencia = true;
            }
            else
            {
                ViewBag.generaTransferencia = false;
            }


            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "11");

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;
                        productionLot.id_userUpdate = this.ActiveUserId;
                        productionLot.dateUpdate = DateTime.Now;

                        // Actualizo los movimientos de inventario
                        #region Actualizo los movimientos de inventario

                        // Busco el estado CONCILIADO para Movimiento de inventario
                        DocumentState documentState = db.DocumentState.FirstOrDefault(fod => fod.code == "16");
                        if (documentState == null)
                            throw new Exception("Estado de documento CONCILIADO no encontrado");

                        // Estado de documento ANULADO para OMITIRLOS 
                        var estadoAnulado = db.DocumentState.FirstOrDefault(e => e.code == "05");

                        if (estadoAnulado == null)
                            throw new Exception("Estado de documento ANULADO no encontrado");

                        // Busco el documento origen en base al proceso interno y actualizo estado
                        var documentsSource = db.DocumentSource.Where(fod => fod.id_documentOrigin == productionLot.id && fod.Document.id_documentState != estadoAnulado.id).ToList();
                        if (documentsSource == null)
                            throw new Exception("Documento(s) de origen no encontrado");

                        // Recorre los documentos de origen y luegos los documentos de M.I.
                        foreach (var documentSource in documentsSource)
                        {
                            var document = db.Document.FirstOrDefault(e => e.id == documentSource.id_document);

                            if (document == null)
                                throw new Exception("Documento de movimiento de inventario no encontrado");

                            document.id_documentState = documentState.id;
                            document.id_userUpdate = this.ActiveUserId;
                            document.dateUpdate = DateTime.Now;

                            db.Document.Attach(document);
                            db.Entry(document).State = EntityState.Modified;
                        }

                        #endregion

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

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


            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " conciliado exitosamente");

            return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
        }

        #endregion SINGLE CHANGE PRODUCTION LOT STATE

        #region SELECTED PRODUCTION LOT STATE CHANGE

        [HttpPost, ValidateInput(false)]
        public void ApproveLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "02");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;
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

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void AutorizeLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "04");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;
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

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void ProtectLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "05");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;
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

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void CancelLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "03");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;

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

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void RevertLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "01");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;

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

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProductionLotWarehouseLocation(int? id_warehouse)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var result = new
            {
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse && w.isActive)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       })
            };

            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProductionLotWarehouse(int? id_warehouse)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var result = new
            {
                warehouse = db.Warehouse.Where(w => w.id == id_warehouse && w.isActive)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       })
            };

            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion SELECTED PRODUCTION LOT STATE CHANGE

        #region PRODUCTION LOTE PROCESS REPORTS

        [HttpPost]
        public ActionResult ProductionLotProcessReport(int id)
        {
            try
            {
                Session["URLOTE"] = ConfigurationManager.AppSettings["URLOTE"];
            }
            catch (Exception ex)
            {
                ViewBag.IframeUrl = ex.Message;
            }

            ViewBag.IframeUrl = Session["URLOTE"] + "?id=" + id;

            return PartialView("IndexReportLote");
        }

        #endregion PRODUCTION LOTE PROCESS REPORTS

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            int id_menu = (int)ViewData["id_menu"];
            var tienePermisioConciliar = this.ActiveUser
                    .UserMenu.FirstOrDefault(e => e.id_menu == id_menu)?
                    .Permission?.FirstOrDefault(p => p.name == "Conciliar");
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";

            var actions = new
            {
                btnApprove = true,
                btnAutorize = false,
                btnProtect = false,
                btnCancel = false,
                btnRevert = false,
                btnPrint = false,
                btnTransfer = false,
                btnPrintIngresoBodega = false,
                btnPrintLiquidacionProcesoInterno = false,
                btnConciliation = false
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(r => r.id == productionLot.id_ProductionLotState);
            string code_state = productionLotState.code;

            if (code_state == "01")
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = false,
                    btnPrint = false,
                    btnTransfer = false,
                    btnPrintIngresoBodega = false,
                    btnPrintLiquidacionProcesoInterno = false,
                    btnConciliation = false
                };
            }
            else if (code_state == "02")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnPrint = true,
                    btnTransfer = false,
                    btnPrintIngresoBodega = false,
                    btnPrintLiquidacionProcesoInterno = false,
                    btnConciliation = false
                };
            }
            else if (code_state == "03")
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnPrint = true,
                    btnTransfer = false,
                    btnPrintIngresoBodega = false,
                    btnPrintLiquidacionProcesoInterno = false,
                    btnConciliation = false
                };
            }
            else if (code_state == "04")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnPrint = true,
                    btnTransfer = false,
                    btnPrintIngresoBodega = true,
                    btnPrintLiquidacionProcesoInterno = false,
                    btnConciliation = false
                };
            }
            else if (code_state == "05")
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnPrint = true,
                    btnTransfer = false,
                    btnPrintIngresoBodega = true,
                    btnPrintLiquidacionProcesoInterno = true,
                    btnConciliation = false
                };
            }
            else if (code_state == "06")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnPrint = true,
                    btnTransfer = (solicitaMaquina == "SI" && productionLot.ProductionProcess.generateTransfer == true) ? true : false,
                    btnPrintIngresoBodega = true,
                    btnPrintLiquidacionProcesoInterno = true,
                    btnConciliation = tienePermisioConciliar != null
                };
            }
            else if (code_state == "10")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnPrint = true,
                    btnTransfer = false,
                    btnPrintIngresoBodega = true,
                    btnPrintLiquidacionProcesoInterno = true,
                    btnConciliation = false
                };
            }
            else if (code_state == "09")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnPrint = false,
                    btnTransfer = false,
                    btnPrintIngresoBodega = false,
                    btnPrintLiquidacionProcesoInterno = false,
                    btnConciliation = false
                };
            }
            else if (code_state == "11") // CONCILIADO
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnPrint = true,
                    btnTransfer = (solicitaMaquina == "SI" && productionLot.ProductionProcess.generateTransfer == true) ? true : false,
                    btnPrintIngresoBodega = true,
                    btnPrintLiquidacionProcesoInterno = true,
                    btnConciliation = false
                };
            }
            else if (code_state == "13")
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnPrint = false,
                    btnTransfer = false,
                    btnPrintIngresoBodega = true,
                    btnPrintLiquidacionProcesoInterno = true,
                    btnConciliation = false
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion ACTIONS

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            TempData.Keep("productionLotProcess");

            int[] ids = db.ProductionLot
                                .Where(p => p.ProductionProcess.code != "REC")
                                .Select(r => r.id)
                                .ToArray();

            int index = ids.OrderByDescending(r => r).ToList().FindIndex(r => r == id_productionLot);

            var result = new
            {
                maximunPages = ids.Length,
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewBag.isProductionProcess = "S";

            ProductionLot productionLot = db.ProductionLot.Where(p => p.ProductionProcess.code != "REC").OrderByDescending(p => p.id).Take(page).ToList().Last();
            var strJul = productionLot.internalNumber.Trim().Substring(0, 5);
            var strInt = productionLot.internalNumber.Trim().Substring(5);
            var strJulConcat = productionLot.internalNumber;

            if (productionLot != null)
            {
                TempData["productionLotProcess"] = productionLot;
                TempData.Keep("productionLotProcess");

                productionLot.julianoNumber = strJul;
                productionLot.internalNumber = strInt;
                productionLot.internalNumberConcatenated = strJulConcat;

                return PartialView("_ProductionLotProcessEditFormPartial", productionLot);
            }

            TempData.Keep("productionLotProcess");

            return PartialView("_ProductionLotProcessEditFormPartial", new ProductionLot());
        }

        #endregion PAGINATION

        #region REPROCESS - PROCESS

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessReprocessFormEditPartial(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id);
            if (productionLot == null)
            {
                productionLot = new ProductionLot
                {
                    ProductionProcess = db.ProductionProcess.FirstOrDefault(p => p.code.Equals("REP"))
                };
            }

            return ProductionLotProcessFormEditPartial(0, null, null);
        }

        #endregion REPROCESS - PROCESS

        #region ADDED VALUE - PROCESS

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotProcessAddedValueFormEditPartial(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id);
            if (productionLot == null)
            {
                productionLot = new ProductionLot
                {
                    ProductionProcess = db.ProductionProcess.FirstOrDefault(p => p.code.Equals("VAG"))
                };
            }

            return ProductionLotProcessFormEditPartial(0, null, null);
        }

        #endregion ADDED VALUE - PROCESS

        #region AXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public ActionResult GetUnitProduction(int? id_ProductionUnitCurrent)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            if (productionLot == null) productionLot = new ProductionLot();
            if (id_ProductionUnitCurrent.HasValue && id_ProductionUnitCurrent != 0)
            {
                productionLot.id_productionUnit = (id_ProductionUnitCurrent ?? 0);
            }
            else
            {
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            return PartialView("ProducionComboBox/_ComboBoxUnitProcess", productionLot);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetOriginLotM(int? id_productionLot, DateTime? fechaProceso, int id_item)
        //[HttpPost]
        //public ActionResult GetOriginLotM(int? id_productionLot, DateTime? fechaProceso)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            TempData.Keep("productionLotProcess");

            int? id_warehouse = null;
            int? id_warehouseLocation = null;
            ProductionProcess proceso = db.ProductionProcess
                .FirstOrDefault(r => r.id == productionLot.id_productionProcess);
            if (proceso != null)
            {
                id_warehouse = proceso.id_warehouse;
                id_warehouseLocation = proceso.id_WarehouseLocation;
            }

            var natureMoveList = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            var idIngresoNature = natureMoveList.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "I");
            var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";

            // recuperamos productos con saldos
            //var saldos = new Services.ServiceInventoryMove()
            //    .GetSaldosProductoLote(true, id_warehouse, id_warehouseLocation, id_item, id_productionLot, null, fechaProceso);

            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral( new InvParameterBalanceGeneral
            {
                requiresLot = true,
                id_Warehouse = id_warehouse,
                id_WarehouseLocation = id_warehouseLocation,
                id_Item = id_item,
                id_ProductionLot = id_productionLot,
                lotMarket = null,
                id_productionCart = null,
                cut_Date = fechaProceso,
                id_company = this.ActiveCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;


            // filtramos por productos
            var codesInventoryLine = new[] { "PT", "PP" };
            var idsInventoryLines = db.InventoryLine
                .Where(e => codesInventoryLine.Contains(e.code))
                .Select(e => e.id)
                .ToList();

            var items = db.Item
                .Where(e => idsInventoryLines.Contains(e.id_inventoryLine) && e.isActive)
                .Select(e => new
                {
                    e.id,
                    id_metricUnit = e.ItemInventory != null
                        ? e.ItemInventory.id_metricUnitInventory : 0
                })
                .ToList();

            var lotes = saldos
                .Where(e => items.FirstOrDefault(w => w.id == e.id_item && w.id_metricUnit == e.id_metricUnit) != null)
                .Where(e => e.id_lote.HasValue)
                .GroupBy(e => new
                {
                    id_lote = e.id_lote.Value,
                    e.number,
                    e.internalNumber,
                    e.lot_market
                })
                .Select(e => e.Key)
                .ToArray();

            var originLots = lotes
                .Select(e => new ProductionLot()
                {
                    id = e.id_lote,
                    internalNumber = e.internalNumber
                        + (!String.IsNullOrEmpty(e.number) ? $" / {e.number}" : string.Empty)
                        + (!String.IsNullOrEmpty(e.lot_market) ? $" / {e.lot_market}" : string.Empty)

                });

            var result = new
            {
                originLots = originLots.Select(e => new { e.id, internalNumber = e.internalNumber ?? String.Empty }).ToArray(),
            };

            //originLots = originLots
            //                .GroupBy(r => r.id)
            //                .Select(r => new ProductionLot { id = r.Key, internalNumber = r.Max(t => t.internalNumber) })
            //                .ToList();

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            //return GridViewExtension.GetComboBoxCallbackResult(p =>
            //{
            //    p.ClientInstanceName = "id_originLot";
            //    p.Width = Unit.Percentage(100);
            //    p.ValueField = "id";
            //    p.TextField = "internalNumber";
            //    p.ValueType = typeof(int);
            //    p.CallbackRouteValues = new { Controller = "ProductionLotProcess", Action = "GetOriginLotM" };
            //    p.CallbackPageSize = 5;
            //    p.ClientSideEvents.BeginCallback = "ProductionLot_BeginCallback";
            //    p.ClientSideEvents.EndCallback = "ProductionLot_EndCallback";

            //    p.DropDownStyle = DropDownStyle.DropDownList;
            //    p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            //    p.EnableSynchronization = DefaultBoolean.False;
            //    p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //    p.ClientSideEvents.SelectedIndexChanged = "ComboProductionLot_SelectedIndexChanged";
            //    p.ClientSideEvents.Init = "ComboProductionLot_Init";
            //    p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

            //    p.ClientSideEvents.Validation = "OnOriginLotDetailValidation";

            //    p.BindList(originLots.Select(d => new
            //    {
            //        d.id,
            //        d.internalNumber,
            //    }).ToList());
            //});

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetOriginLot(int? id_productionLot,
            int? id_item, int? id_warehouse0, int? id_warehouseLocation0,
            DateTime? fechaProceso)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            TempData.Keep("productionLotProcess");

            int? id_warehouse = null;
            int? id_warehouseLocation = null;
            ProductionProcess proceso = db.ProductionProcess
                                              .FirstOrDefault(r => r.id == productionLot.id_productionProcess);

            if (proceso != null)
            {
                id_warehouse = proceso.id_warehouse;
                id_warehouseLocation = proceso.id_WarehouseLocation;
            }

            var natureMoveList = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            var idIngresoNature = natureMoveList.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "I");

            // recuperamos productos con saldos
            //var saldos = new Services.ServiceInventoryMove()
            //    .GetSaldosProductoLote(true, id_warehouse, id_warehouseLocation, null, id_productionLot, null, fechaProceso);


            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(    new InvParameterBalanceGeneral
            {
                requiresLot = true,
                id_Warehouse = id_warehouse,
                id_WarehouseLocation = id_warehouseLocation,
                id_Item = null,
                id_ProductionLot = id_productionLot,
                lotMarket = null,
                id_productionCart = null,
                cut_Date = fechaProceso,
                id_company = this.ActiveCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;


            // filtramos por productos
            var codesInventoryLine = new[] { "PT", "PP" };
            var idsInventoryLines = db.InventoryLine
                .Where(e => codesInventoryLine.Contains(e.code))
                .Select(e => e.id)
                .ToList();

            var items = db.Item
                .Where(e => idsInventoryLines.Contains(e.id_inventoryLine) && e.isActive)
                .Select(e => new
                {
                    e.id,
                    id_metricUnit = e.ItemInventory != null
                        ? e.ItemInventory.id_metricUnitInventory : 0
                })
                .ToList();


            //var lotes = saldos
            //    .Where(e => items.FirstOrDefault(w => w.id == e.id_item && w.id_metricUnit == e.id_metricUnit) != null)
            //    .Where(e => e.id_lote.HasValue)
            //    .GroupBy(e => new
            //    {
            //        id_lote = e.id_lote.Value,
            //        e.number,
            //        e.internalNumber,
            //        e.lot_market
            //    })
            //    .Select(e => e.Key)
            //    .ToArray();

            var lotes = saldos
                .Join(items,
                    saldo => new { id_item = saldo.id_item},
                    item => new { id_item = item.id},
                    (saldo, item) => saldo)
                .Where(e => e.id_lote.HasValue)
                .GroupBy(e => new
                {
                    id_lote = e.id_lote.Value,
                    e.number,
                    e.internalNumber,
                    e.lot_market
                })
                .Select(e => e.Key)
                .ToArray();

            var originLots = lotes
                .Select(e => new ProductionLot()
                {
                    id = e.id_lote,
                    internalNumber = e.internalNumber
                        + (!String.IsNullOrEmpty(e.number) ? $" / {e.number}" : string.Empty)
                        + (!String.IsNullOrEmpty(e.lot_market) ? $" / {e.lot_market}" : string.Empty)

                });

            originLots = originLots
                            .GroupBy(r => r.id)
                            .Select(r => new ProductionLot { id = r.Key, internalNumber = r.Max(t => t.internalNumber) })
                            .ToList();

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_originLot";
                p.Width = Unit.Percentage(100);
                p.ValueField = "id";
                p.TextField = "internalNumber";
                p.ValueType = typeof(int);
                p.CallbackRouteValues = new { Controller = "ProductionLotProcess", Action = "GetOriginLot" };
                p.CallbackPageSize = 5;
                p.ClientSideEvents.BeginCallback = "ProductionLot_BeginCallback";
                p.ClientSideEvents.EndCallback = "ProductionLot_EndCallback";

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.ClientSideEvents.SelectedIndexChanged = "ComboProductionLot_SelectedIndexChanged";
                p.ClientSideEvents.Init = "ComboProductionLot_Init";
                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.ClientSideEvents.Validation = "OnOriginLotDetailValidation";

                p.BindList(originLots.Select(d => new
                {
                    d.id,
                    d.internalNumber,
                }).ToList());
            });
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotOriginLotData(int? id_productionLot, int? id_item, int? id_warehouse, int? id_warehouseLocation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotTemp = (TempData["productionLotProcess"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();

            var model = productionLotTemp?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            var codigosEstados = new[] { "03", "16" };
            var inventoryDetailProductionLot = db.InventoryMoveDetail.Where(w => (codigosEstados.Contains(w.InventoryMove.Document.DocumentState.code) && w.id_inventoryMoveDetailNext == null) && (w.Item.InventoryLine.code.Equals("PT") || w.Item.InventoryLine.code.Equals("PP")) &&
                                                                                  w.id_metricUnit == w.Item.ItemInventory.id_metricUnitInventory &&
                                                                                  ((w.balance > 0 && w.Lot != null && w.Lot.ProductionLot != null) ||
                                                                                  (w.id_lot == id_productionLot && w.id_item == id_item && w.id_warehouse == id_warehouse && w.id_warehouseLocation == id_warehouseLocation))).ToList();

            var originLots = new List<ProductionLot>();
            var items = new List<Item>();
            var metricUnit = "";
            var metricUnitPresentation = "";
            var warehouses = new List<Warehouse>();
            var warehousesLocation = new List<WarehouseLocation>();
            decimal currentStock = 0;
            decimal minimumPresentation = 1;
            foreach (var inventoryDetail in inventoryDetailProductionLot)
            {
                if (model.Any(a => a.id_originLot == inventoryDetail.id_lot && a.id_originLot != id_productionLot &&
                                  a.id_item == inventoryDetail.id_item && a.id_item != id_item &&
                                  a.id_warehouse == inventoryDetail.id_warehouse && a.id_warehouse != id_warehouse &&
                                  a.id_warehouseLocation == inventoryDetail.id_warehouseLocation && a.id_warehouseLocation != id_warehouseLocation)) continue;

                if (!originLots.Contains(inventoryDetail.Lot.ProductionLot)) originLots.Add(inventoryDetail.Lot.ProductionLot);

                if (inventoryDetail.id_lot == id_productionLot)
                {
                    if (!items.Contains(inventoryDetail.Item)) items.Add(inventoryDetail.Item);

                    if (inventoryDetail.id_item == id_item)
                    {
                        metricUnit = inventoryDetail.MetricUnit.code;
                        metricUnitPresentation = inventoryDetail.Item.Presentation?.MetricUnit.code ?? metricUnit;
                        minimumPresentation = inventoryDetail.Item.Presentation?.minimum ?? 1;
                        if (!warehouses.Contains(inventoryDetail.Warehouse)) warehouses.Add(inventoryDetail.Warehouse);

                        if (inventoryDetail.id_warehouse == id_warehouse)
                        {
                            if (!warehousesLocation.Contains(inventoryDetail.WarehouseLocation)) warehousesLocation.Add(inventoryDetail.WarehouseLocation);

                            if (inventoryDetail.id_warehouseLocation == id_warehouseLocation)
                            {
                                currentStock = inventoryDetail.balance;
                            }
                        }
                    }
                }
            }
            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLotTemp;
            TempData.Keep("productionLotProcess");

            if (originLots.Count() == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                originLots = originLots.Select(d => new
                {
                    d.id,
                    name = d.number,
                }).ToList(),
                items = items.Select(d => new
                {
                    d.id,
                    d.name,
                }).ToList(),
                metricUnit,
                metricUnitPresentation,
                warehouses = warehouses.Select(d => new
                {
                    d.id,
                    d.name,
                }).ToList(),
                warehouseLocations = warehousesLocation.Select(d => new
                {
                    d.id,
                    d.name,
                }).ToList(),
                currentStock,
                minimumPresentation
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetItem(int? id_productionLot, int? id_originLot, int? id_item, string lotMarked, int? id_warehouse0, int? id_warehouseLocation0, DateTime fechaProceso)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";
            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            TempData.Keep("productionLotProcess");

            int? id_warehouse = null;
            int? id_warehouseLocation = null;
            ProductionProcess proceso = db.ProductionProcess
                                              .FirstOrDefault(r => r.id == productionLot.id_productionProcess);
            if (proceso != null)
            {
                id_warehouse = proceso.id_warehouse;
                id_warehouseLocation = proceso.id_WarehouseLocation;
            }

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            var codigosEstados = new[] { "03", "16" };
            var inventoryMovesDetails = db.InventoryMoveDetail
                                           .Where(w => codigosEstados.Contains(w.InventoryMove.Document.DocumentState.code)
                                                        && (w.Item.InventoryLine.code.Equals("PT") || w.Item.InventoryLine.code.Equals("PP"))
                                                        && w.id_metricUnit == w.Item.ItemInventory.id_metricUnitInventory
                                                        && w.Lot != null
                                                        && w.id_lot == id_productionLot
                                                        && w.id_warehouse == id_warehouse
                                                        && w.id_warehouseLocation == id_warehouseLocation
                                                        && w.InventoryMove.Document.emissionDate <= fechaProceso
                                                        || (w.id_item == id_item)
                                                        )
                                           .ToList();

            if (lotMarkedPar == "SI" && lotMarked != null)
            {
                inventoryMovesDetails = inventoryMovesDetails.Where(e => e.lotMarked == lotMarked).ToList();
            }

            var inventoryDetailProductionLot = inventoryMovesDetails
                                           .GroupBy(w => new
                                           {
                                               w.id_warehouse,
                                               w.id_warehouseLocation,
                                               w.id_item,
                                               w.id_lot,
                                               w.id_metricUnit
                                           }).Select(d => new
                                           {
                                               d.Key.id_warehouse,
                                               d.Key.id_warehouseLocation,
                                               d.Key.id_item,
                                               d.Key.id_lot,
                                               d.Key.id_metricUnit,
                                               remainingBalance = d.Sum(s => s.entryAmount - s.exitAmount)
                                           }).Where(w => (w.remainingBalance > 0
                                                                                                        )).ToList();

            var items = new List<Item>();
            foreach (var inventoryDetail in inventoryDetailProductionLot)
            {
                if (model.Any(a => a.id_originLot == inventoryDetail.id_lot && a.id_originLot != id_originLot &&
                                  a.id_item == inventoryDetail.id_item && a.id_item != id_item &&
                                  a.id_warehouse == inventoryDetail.id_warehouse && a.id_warehouse != id_warehouse &&
                                  a.id_warehouseLocation == inventoryDetail.id_warehouseLocation && a.id_warehouseLocation != id_warehouseLocation)) continue;

                var aItem = db.Item.FirstOrDefault(fod => fod.id == inventoryDetail.id_item);

                if (aItem != null && !items.Contains(aItem)) items.Add(aItem);
            }

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_item";
                p.Width = Unit.Percentage(100);
                p.TextFormatString = "{0} | {1}";
                p.ValueField = "id";
                p.ValueType = typeof(int);
                p.CallbackRouteValues = new { Controller = "ProductionLotProcess", Action = "GetItem" };
                p.CallbackPageSize = 20;
                p.Columns.Add("masterCode", "Código", 70);
                p.Columns.Add("name", "Nombre del Producto", 200);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.ClientSideEvents.SelectedIndexChanged = "ComboItem_SelectedIndexChanged";
                p.ClientSideEvents.BeginCallback = "ComboItem_BeginCallback";
                p.ClientSideEvents.Init = "ItemProductionLotDetailCombo_Init";

                p.ClientSideEvents.Validation = "OnItemDetailValidation";
                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.BindList(items.Select(d => new
                {
                    d.id,
                    d.name,
                    d.masterCode,
                }).ToList());
            });
        }

        [HttpPost]
        public ActionResult GetItemM(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";
            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            TempData.Keep("productionLotProcess");

            int? id_warehouse = null;
            int? id_warehouseLocation = null;
            ProductionProcess proceso = db.ProductionProcess
                                              .FirstOrDefault(r => r.id == productionLot.id_productionProcess);
            if (proceso != null)
            {
                id_warehouse = proceso.id_warehouse;
                id_warehouseLocation = proceso.id_WarehouseLocation;
            }

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            var codigosEstados = new[] { "03", "16" };
            var inventoryMovesDetails = db.InventoryMoveDetail
                                           .Where(w => codigosEstados.Contains(w.InventoryMove.Document.DocumentState.code)
                                                        && (w.Item.InventoryLine.code.Equals("PT") || w.Item.InventoryLine.code.Equals("PP"))
                                                        && w.id_metricUnit == w.Item.ItemInventory.id_metricUnitInventory
                                                        && w.Lot != null
                                                        //&& w.id_lot == id_productionLot
                                                        && w.id_warehouse == id_warehouse
                                                        && w.id_warehouseLocation == id_warehouseLocation
                                                        || (w.id_item == id_item))
                                           .ToList();

            var inventoryDetailProductionLot = inventoryMovesDetails
                                           .GroupBy(w => new
                                           {
                                               w.id_warehouse,
                                               w.id_warehouseLocation,
                                               w.id_item,
                                               w.id_lot,
                                               w.id_metricUnit
                                           }).Select(d => new
                                           {
                                               d.Key.id_warehouse,
                                               d.Key.id_warehouseLocation,
                                               d.Key.id_item,
                                               d.Key.id_lot,
                                               d.Key.id_metricUnit,
                                               remainingBalance = d.Sum(s => s.entryAmount - s.exitAmount)
                                           }).Where(w => (w.remainingBalance > 0)).ToList();

            var items = new List<Item>();
            foreach (var inventoryDetail in inventoryDetailProductionLot)
            {
                if (model.Any(a => a.id_warehouse == inventoryDetail.id_warehouse && a.id_warehouse != id_warehouse &&
                                  a.id_warehouseLocation == inventoryDetail.id_warehouseLocation && a.id_warehouseLocation != id_warehouseLocation)) continue;

                var aItem = db.Item.FirstOrDefault(fod => fod.id == inventoryDetail.id_item);

                if (aItem != null && !items.Contains(aItem)) items.Add(aItem);
            }

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_item";
                p.Width = Unit.Percentage(100);
                p.TextFormatString = "{0} | {1}";
                p.ValueField = "id";
                p.ValueType = typeof(int);
                p.CallbackRouteValues = new { Controller = "ProductionLotProcess", Action = "GetItemM" };
                p.CallbackPageSize = 20;
                p.Columns.Add("masterCode", "Código", 70);
                p.Columns.Add("name", "Nombre del Producto", 200);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                //p.ClientSideEvents.SelectedIndexChanged = "ComboItem_SelectedIndexChanged";
                //p.ClientSideEvents.BeginCallback = "ComboItem_BeginCallback";
                //p.ClientSideEvents.Init = "ItemProductionLotDetailCombo_Init";

                p.ClientSideEvents.Validation = "OnItemDetailValidation";
                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.BindList(items.Select(d => new
                {
                    d.id,
                    d.name,
                    d.masterCode,
                }).ToList());
            });
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotDetailData(int? id_originLot, int? id_item, int? id_warehouse, int? id_warehouseLocation,
                                                  int? id_productionLotSelected, int? id_itemSelected, int? id_warehouseSelected, int? id_warehouseLocationSelected)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotTemp = (TempData["productionLotProcess"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();

            var model = productionLotTemp?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            var codigosEstados = new[] { "03", "16" };
            var inventoryDetailProductionLot = db.InventoryMoveDetail.Where(w => (codigosEstados.Contains(w.InventoryMove.Document.DocumentState.code) && w.id_inventoryMoveDetailNext == null && w.balance > 0 &&
                                                                                  w.id_lot == id_productionLotSelected && (w.Item.InventoryLine.code.Equals("PT") || w.Item.InventoryLine.code.Equals("PP")) &&
                                                                                  w.id_metricUnit == w.Item.ItemInventory.id_metricUnitInventory)).ToList();
            var items = new List<Item>();
            foreach (var inventoryDetail in inventoryDetailProductionLot)
            {
                if (model.Any(a => a.id_originLot == inventoryDetail.id_lot && a.id_originLot != id_originLot &&
                                  a.id_item == inventoryDetail.id_item && a.id_item != id_item &&
                                  a.id_warehouse == inventoryDetail.id_warehouse && a.id_warehouse != id_warehouse &&
                                  a.id_warehouseLocation == inventoryDetail.id_warehouseLocation && a.id_warehouseLocation != id_warehouseLocation)) continue;

                if (!items.Contains(inventoryDetail.Item)) items.Add(inventoryDetail.Item);
            }

            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLotTemp;
            TempData.Keep("productionLotProcess");

            if (items == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                items = items.Select(d => new
                {
                    d.id,
                    d.name,
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetWarehouse(int? id_productionLotSelected, int? id_itemSelected, int? id_originLot, int? id_item, int? id_warehouse, int? id_warehouseLocation, int? idProductionProcess)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            TempData.Keep("productionLotProcess");

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            var warehouses = new List<Warehouse>();
            if (idProductionProcess != 0)
            {
                var productionProcess = db.ProductionProcess.FirstOrDefault(r => r.id == idProductionProcess);
                var idWarehouse = productionProcess?.id_warehouse ?? 0;
                if (idWarehouse != 0)
                {
                    warehouses = db.Warehouse
                                        .Where(r => r.id == idWarehouse)
                                        .ToList();
                }
            }

            var fisrtWarehouseid = (warehouses.FirstOrDefault()?.id ?? 0);
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_warehouse";
                p.Width = Unit.Percentage(100);
                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);
                p.CallbackRouteValues = new { Controller = "ProductionLotProcess", Action = "GetWarehouse" };
                p.CallbackPageSize = 5;
                p.ClientSideEvents.BeginCallback = "ComboWarehouse_BeginCallback";
                p.ClientSideEvents.EndCallback = "ComboWarehouse_EndCallback";

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.ClientSideEvents.SelectedIndexChanged = "ComboWarehouse_SelectedIndexChanged";
                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.ClientSideEvents.Validation = "OnWarehouseDetailValidation";

                p.BindList(warehouses.Select(d => new
                {
                    d.id,
                    d.name,
                }).ToList());
            });
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemDetailData(int? id_originLot,
            int? id_item, int? id_warehouse, int? id_warehouseLocation,
            int? id_productionLotSelected, int? id_itemSelected, int? id_warehouseSelected0,
            int? id_warehouseLocationSelected0, DateTime? fechaProceso = null)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotTemp = (TempData["productionLotProcess"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();

            int? id_warehouseSelected = null;
            int? id_warehouseLocationSelected = null;
            ProductionProcess proceso = db.ProductionProcess
                                              .FirstOrDefault(r => r.id == productionLotTemp.id_productionProcess);
            if (proceso != null)
            {
                id_warehouseSelected = proceso.id_warehouse;
                id_warehouseLocationSelected = proceso.id_WarehouseLocation;
            }

            var model = productionLotTemp?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            var codigosEstados = new[] { "03", "16" };
            var _preinventoryDetailProductionLot = db.InventoryMoveDetail
                                                                .Where(w => w.id_warehouse == id_warehouseSelected
                                                                            && w.id_warehouseLocation == id_warehouseLocationSelected
                                                                            && codigosEstados.Contains(w.InventoryMove.Document.DocumentState.code)
                                                                            && (w.Item.InventoryLine.code.Equals("PT") || w.Item.InventoryLine.code.Equals("PP"))
                                                                            && w.id_lot == (id_productionLotSelected == 0 ? null : id_productionLotSelected)
                                                                            && w.Lot != null
                                                                            && w.id_item == id_itemSelected
                                                                               )
                                                                .ToList();

            if (fechaProceso.HasValue)
            {
                _preinventoryDetailProductionLot = _preinventoryDetailProductionLot
                    .Where(e => DateTime.Compare(e.InventoryMove.Document.emissionDate.Date, fechaProceso.Value.Date) <= 0)
                    .ToList();
            }

            var inventoryDetailProductionLot = _preinventoryDetailProductionLot
                                                      .GroupBy(w => new
                                                      {
                                                          w.id_item,
                                                      }).Select(d => new
                                                      {
                                                          id_item = d.Key.id_item,
                                                          id_metricUnit = d.Max(t => t.id_metricUnit),
                                                          remainingBalance = d.Sum(s => s.entryAmount - s.exitAmount)
                                                      }).Where(w => (w.remainingBalance > 0))
                                                      .ToList();

            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLotTemp;
            TempData.Keep("productionLotProcess");

            if (inventoryDetailProductionLot == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var aid_metricUnit = (inventoryDetailProductionLot.FirstOrDefault() != null
                                                                    ? inventoryDetailProductionLot.FirstOrDefault().id_metricUnit
                                                                    : (int?)null);

            var aMetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == aid_metricUnit);
            var aItem = db.Item.FirstOrDefault(fod => fod.id == id_itemSelected);
            var minumimun = DataProviderItem.GetMinimoProductionProcessWMasterCalc(id_itemSelected);
            var result = new
            {
                masterCode = aItem?.masterCode ?? "",
                metricUnit = aMetricUnit?.code ?? "",
                metricUnitPresentation = aItem?.Presentation?.MetricUnit.code ?? (aMetricUnit?.code ?? ""),
                minimumPresentation = minumimun,
                currentStock = (inventoryDetailProductionLot.FirstOrDefault()?.remainingBalance ?? 0),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult ItemMDetailData(int? id_originLot,
            int? id_item, int? id_warehouse, int? id_warehouseLocation,
            int? id_productionLotSelected, int? id_itemSelected, int? id_warehouseSelected0,
            int? id_warehouseLocationSelected0, DateTime? fechaProceso = null)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotTemp = (TempData["productionLotProcess"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();

            int? id_warehouseSelected = null;
            int? id_warehouseLocationSelected = null;
            ProductionProcess proceso = db.ProductionProcess
                                              .FirstOrDefault(r => r.id == productionLotTemp.id_productionProcess);
            if (proceso != null)
            {
                id_warehouseSelected = proceso.id_warehouse;
                id_warehouseLocationSelected = proceso.id_WarehouseLocation;
            }

            var model = productionLotTemp?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            var codigosEstados = new[] { "03", "16" };
            var _preinventoryDetailProductionLot = db.InventoryMoveDetail
                                                                .Where(w => w.id_warehouse == id_warehouseSelected
                                                                            && w.id_warehouseLocation == id_warehouseLocationSelected
                                                                            && codigosEstados.Contains(w.InventoryMove.Document.DocumentState.code)
                                                                            && (w.Item.InventoryLine.code.Equals("PT") || w.Item.InventoryLine.code.Equals("PP"))
                                                                            && w.id_lot == (id_productionLotSelected == 0 ? null : id_productionLotSelected)
                                                                            && w.Lot != null
                                                                            && w.id_item == id_itemSelected
                                                                               )
                                                                .ToList();

            if (fechaProceso.HasValue)
            {
                _preinventoryDetailProductionLot = _preinventoryDetailProductionLot
                    .Where(e => DateTime.Compare(e.InventoryMove.Document.emissionDate.Date, fechaProceso.Value.Date) <= 0)
                    .ToList();
            }

            var inventoryDetailProductionLot = _preinventoryDetailProductionLot
                                                      .GroupBy(w => new
                                                      {
                                                          w.id_item,
                                                      }).Select(d => new
                                                      {
                                                          id_item = d.Key.id_item,
                                                          id_metricUnit = d.Max(t => t.id_metricUnit),
                                                          remainingBalance = d.Sum(s => s.entryAmount - s.exitAmount)
                                                      }).Where(w => (w.remainingBalance > 0))
                                                      .ToList();

            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLotTemp;
            TempData.Keep("productionLotProcess");

            if (inventoryDetailProductionLot == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var aid_metricUnit = (inventoryDetailProductionLot.FirstOrDefault() != null
                                                                    ? inventoryDetailProductionLot.FirstOrDefault().id_metricUnit
                                                                    : (int?)null);

            var aMetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == aid_metricUnit);
            var aItem = db.Item.FirstOrDefault(fod => fod.id == id_itemSelected);

            // recuperamos productos con saldos
            //var saldos = new ServiceInventoryMove()
            //            .GetSaldosProductoLote(true, id_warehouseSelected, id_warehouseLocationSelected, id_itemSelected, id_productionLotSelected, null, fechaProceso);
            
            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(   new InvParameterBalanceGeneral
            {
                requiresLot = true,
                id_Warehouse = id_warehouseSelected,
                id_WarehouseLocation = id_warehouseLocationSelected,
                id_Item = id_itemSelected,
                id_ProductionLot = id_productionLotSelected,
                lotMarket = null,
                id_productionCart = null,
                cut_Date = fechaProceso,
                id_company = this.ActiveCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;

            // filtramos por productos
            var codesInventoryLine = new[] { "PT", "PP" };
            var idsInventoryLines = db.InventoryLine
                .Where(e => codesInventoryLine.Contains(e.code))
                .Select(e => e.id)
                .ToList();

            var items = db.Item
                .Where(e => idsInventoryLines.Contains(e.id_inventoryLine) && e.isActive)
                .Select(e => new
                {
                    e.id,
                    id_metricUnit = e.ItemInventory != null
                        ? e.ItemInventory.id_metricUnitInventory : 0
                })
                .ToList();

            var lotes = saldos
                .Where(e => items.FirstOrDefault(w => w.id == e.id_item && w.id_metricUnit == e.id_metricUnit) != null)
                .Where(e => e.id_lote.HasValue)
                .GroupBy(e => new
                {
                    id_lote = e.id_lote.Value,
                    e.number,
                    e.internalNumber,
                    e.lot_market
                })
                .Select(e => e.Key)
                .ToArray();

            var originLots = lotes
                .Select(e => new ProductionLot()
                {
                    id = e.id_lote,
                    internalNumber = e.internalNumber
                        + (!String.IsNullOrEmpty(e.number) ? $" / {e.number}" : string.Empty)
                        + (!String.IsNullOrEmpty(e.lot_market) ? $" / {e.lot_market}" : string.Empty)

                });



            var minumimun = DataProviderItem.GetMinimoProductionProcessWMasterCalc(id_itemSelected);
            var result = new
            {
                masterCode = aItem?.masterCode ?? "",
                metricUnit = aMetricUnit?.code ?? "",
                metricUnitPresentation = aItem?.Presentation?.MetricUnit.code ?? (aMetricUnit?.code ?? ""),
                minimumPresentation = minumimun,
                currentStock = (inventoryDetailProductionLot.FirstOrDefault()?.remainingBalance ?? 0),
                originLots = originLots.Select(e => new { e.id, internalNumber = e.internalNumber ?? String.Empty }).ToArray(),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemLotDetailData(int? id_originLot,
                   int? id_item, int? id_warehouse, int? id_warehouseLocation,
                   int? id_productionLotSelected, int? id_itemSelected, int? id_warehouseSelected0,
                   int? id_warehouseLocationSelected0, DateTime? fechaProceso = null)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotTemp = (TempData["productionLotProcess"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();

            int? id_warehouseSelected = null;
            int? id_warehouseLocationSelected = null;
            ProductionProcess proceso = db.ProductionProcess
                                              .FirstOrDefault(r => r.id == productionLotTemp.id_productionProcess);
            if (proceso != null)
            {
                id_warehouseSelected = proceso.id_warehouse;
                id_warehouseLocationSelected = proceso.id_WarehouseLocation;
            }

            var model = productionLotTemp?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            var codigosEstados = new[] { "03", "16" };
            var _preinventoryDetailProductionLot = db.InventoryMoveDetail
                                                                .Where(w => w.id_warehouse == id_warehouseSelected
                                                                            && w.id_warehouseLocation == id_warehouseLocationSelected
                                                                            && codigosEstados.Contains(w.InventoryMove.Document.DocumentState.code)
                                                                            && (w.Item.InventoryLine.code.Equals("PT") || w.Item.InventoryLine.code.Equals("PP"))
                                                                            && w.id_lot == (id_productionLotSelected == 0 ? null : id_productionLotSelected)
                                                                            && w.Lot != null
                                                                            && w.id_item == id_itemSelected
                                                                               )
                                                                .ToList();

            if (fechaProceso.HasValue)
            {
                _preinventoryDetailProductionLot = _preinventoryDetailProductionLot
                    .Where(e => DateTime.Compare(e.InventoryMove.Document.emissionDate.Date, fechaProceso.Value.Date) <= 0)
                    .ToList();
            }

            var inventoryDetailProductionLot = _preinventoryDetailProductionLot
                                                      .GroupBy(w => new
                                                      {
                                                          w.id_item,
                                                      }).Select(d => new
                                                      {
                                                          id_item = d.Key.id_item,
                                                          id_metricUnit = d.Max(t => t.id_metricUnit),
                                                          remainingBalance = d.Sum(s => s.entryAmount - s.exitAmount)
                                                      }).Where(w => (w.remainingBalance > 0))
                                                      .ToList();

            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLotTemp;
            TempData.Keep("productionLotProcess");

            if (inventoryDetailProductionLot == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var aid_metricUnit = (inventoryDetailProductionLot.FirstOrDefault() != null
                                                                    ? inventoryDetailProductionLot.FirstOrDefault().id_metricUnit
                                                                    : (int?)null);

            var aMetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == aid_metricUnit);
            var aItem = db.Item.FirstOrDefault(fod => fod.id == id_itemSelected);

           

            var minumimun = DataProviderItem.GetMinimoProductionProcessWMasterCalc(id_itemSelected);
            var result = new
            {
                masterCode = aItem?.masterCode ?? "",
                metricUnit = aMetricUnit?.code ?? "",
                metricUnitPresentation = aItem?.Presentation?.MetricUnit.code ?? (aMetricUnit?.code ?? ""),
                minimumPresentation = minumimun,
                currentStock = (inventoryDetailProductionLot.FirstOrDefault()?.remainingBalance ?? 0),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotDetailWarehouseData(int? id_productionLot, int? id_item, int? id_warehouse)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);

            ProductionLot productionLotTemp = (TempData["productionLotProcess"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();

            var model = productionLotTemp?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLotTemp;
            TempData.Keep("productionLotProcess");

            if (productionLot == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                productionLot.id,
                warehouses = productionLot.ProductionLotLiquidation.Where(w => w.id_item == id_item).GroupBy(w => w.id_warehouse).Select(d => new
                {
                    id = d.Key,
                    db.Warehouse.FirstOrDefault(fod => fod.id == d.Key).name,
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetWarehouseLocation(int? id_productionLotSelected, int? id_itemSelected, int? id_warehouseSelected, int? id_originLot, int? id_item, int? id_warehouse, int? id_warehouseLocation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            TempData.Keep("productionLotProcess");

            var warehousesLocation = new List<WarehouseLocation>();
            if (id_warehouseSelected.HasValue)
            {
                warehousesLocation = db.WarehouseLocation
                                            .Where(r => r.id_warehouse == id_warehouseSelected)
                                            .ToList();
            }

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_warehouseLocation0";
                p.Width = Unit.Percentage(100);
                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);
                p.CallbackRouteValues = new { Controller = "ProductionLotProcess", Action = "GetWarehouseLocation" };
                p.CallbackPageSize = 5;
                p.ClientSideEvents.BeginCallback = "ComboWarehouseLocation_BeginCallback";

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.ClientSideEvents.SelectedIndexChanged = "ComboWarehouseLocation_SelectedIndexChanged";
                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.ClientSideEvents.Validation = "OnWarehouseLocationDetailValidation";

                p.BindList(warehousesLocation.Select(d => new
                {
                    d.id,
                    d.name,
                }).ToList());
            });
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult WarehouseDetailData(int? id_originLot, int? id_item, int? id_warehouse, int? id_warehouseLocation,
                                              int? id_productionLotSelected, int? id_itemSelected, int? id_warehouseSelected, int? id_warehouseLocationSelected)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotTemp = (TempData["productionLotProcess"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();

            var model = productionLotTemp?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            var codigosEstados = new[] { "03", "16" };
            var inventoryDetailProductionLot = db.InventoryMoveDetail.Where(w => (codigosEstados.Contains(w.InventoryMove.Document.DocumentState.code) && w.id_inventoryMoveDetailNext == null && w.balance > 0 &&
                                                                                  w.id_lot == id_productionLotSelected && w.id_item == id_itemSelected &&
                                                                                  w.id_warehouse == id_warehouseSelected)).ToList();
            var warehousesLocation = new List<WarehouseLocation>();
            foreach (var inventoryDetail in inventoryDetailProductionLot)
            {
                if (model.Any(a => a.id_originLot == inventoryDetail.id_lot && a.id_originLot != id_originLot &&
                                  a.id_item == inventoryDetail.id_item && a.id_item != id_item &&
                                  a.id_warehouse == inventoryDetail.id_warehouse && a.id_warehouse != id_warehouse &&
                                  a.id_warehouseLocation == inventoryDetail.id_warehouseLocation && a.id_warehouseLocation != id_warehouseLocation)) continue;

                warehousesLocation.Add(inventoryDetail.WarehouseLocation);
            }

            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLotTemp;
            TempData.Keep("productionLotProcess");

            if (inventoryDetailProductionLot == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                metricUnit = inventoryDetailProductionLot.FirstOrDefault()?.MetricUnit.code ?? "",
                warehouseLocations = warehousesLocation.Select(d => new
                {
                    d.id,
                    d.name,
                }).ToList()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotDetailWarehouseLocationData(int? id_productionLot, int? id_item, int? id_warehouse, int? id_warehouseLocation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotTemp = (TempData["productionLotProcess"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();

            var model = productionLotTemp?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot) ?? new ProductionLot();
            var productionLotLiquidations = productionLot.ProductionLotLiquidation.Where(w => w.id_item == id_item && w.id_warehouse == id_warehouse).ToList() ?? new List<ProductionLotLiquidation>();
            var warehousesLocation = new List<WarehouseLocation>();
            foreach (var detailProductionLotLiquidation in productionLotLiquidations)
            {
                if (warehousesLocation.FirstOrDefault(fod => fod.id == detailProductionLotLiquidation.id_warehouseLocation) == null)
                {
                    var productionLotDetail = db.ProductionLotDetail.Where(p => p.id_originLot == detailProductionLotLiquidation.id_productionLot &&
                                                                                p.id_productionLot != productionLotTemp.id &&
                                                                                p.id_item == detailProductionLotLiquidation.id_item &&
                                                                                p.id_warehouse == detailProductionLotLiquidation.id_warehouse &&
                                                                                p.id_warehouseLocation == detailProductionLotLiquidation.id_warehouseLocation &&
                                                                                p.ProductionLot.ProductionLotState.code != "09");
                    var usedProductionLotLiquidation = (productionLotDetail?.Count() ?? 0) > 0 ? productionLotDetail.Sum(s => s.quantityRecived) : 0;
                    if (((detailProductionLotLiquidation?.quantity ?? 0) - usedProductionLotLiquidation) > 0)
                    {
                        if (model.FirstOrDefault(fod => fod.id_originLot == detailProductionLotLiquidation.id_productionLot &&
                                                       fod.id_item == detailProductionLotLiquidation.id_item &&
                                                       fod.id_warehouse == detailProductionLotLiquidation.id_warehouse &&
                                                       fod.id_warehouseLocation == detailProductionLotLiquidation.id_warehouseLocation) == null ||
                                                       detailProductionLotLiquidation.id_warehouseLocation != id_warehouseLocation)
                        {
                            warehousesLocation.Add(detailProductionLotLiquidation.WarehouseLocation);
                        }

                    };
                }
            }
            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLotTemp;
            TempData.Keep("productionLotProcess");

            if (productionLot == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                productionLot.id,
                warehouseLocation = warehousesLocation.Select(d => new
                {
                    d.id,
                    d.name,
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult WarehouseLocationDetailData(int? id_originLot, int? id_item, int? id_warehouse, int? id_warehouseLocation,
                                                      int? id_productionLotSelected, int? id_itemSelected, int? id_warehouseSelected, int? id_warehouseLocationSelected)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotTemp = (TempData["productionLotProcess"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();
            var codigosEstados = new[] { "03", "16" };
            var inventoryDetailProductionLot = db.InventoryMoveDetail
                                           .Where(w => codigosEstados.Contains(w.InventoryMove.Document.DocumentState.code)
                                                        && (w.Item.InventoryLine.code.Equals("PT") || w.Item.InventoryLine.code.Equals("PP"))
                                                        && w.id_metricUnit == w.Item.ItemInventory.id_metricUnitInventory
                                                        && w.Lot != null && w.Lot.ProductionLot != null
                                                        || (w.id_lot == id_originLot && w.id_item == id_item && w.id_warehouse == id_warehouse && w.id_warehouseLocation == id_warehouseLocation))
                                           .GroupBy(w => new
                                           {
                                               w.id_warehouse,
                                               w.id_warehouseLocation,
                                               w.id_item,
                                               w.id_lot,
                                               w.id_metricUnit
                                           }).Select(d => new
                                           {
                                               d.Key.id_warehouse,
                                               d.Key.id_warehouseLocation,
                                               d.Key.id_item,
                                               d.Key.id_lot,
                                               d.Key.id_metricUnit,
                                               remainingBalance = d.Sum(s => s.entryAmount - s.exitAmount)
                                           }).Where(w => (w.remainingBalance > 0 && w.id_lot == id_productionLotSelected
                                                                                 && w.id_item == id_itemSelected
                                                                                 && w.id_warehouse == id_warehouseSelected
                                                                                 && w.id_warehouseLocation == id_warehouseLocationSelected
                                                                                 || (w.id_lot == id_originLot && w.id_item == id_item && w.id_warehouse == id_warehouse && w.id_warehouseLocation == id_warehouseLocation))).ToList();
            TempData["productionLotProcess"] = TempData["productionLotProcess"] ?? productionLotTemp;
            TempData.Keep("productionLotProcess");

            if (inventoryDetailProductionLot == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var aid_metricUnit = (inventoryDetailProductionLot.FirstOrDefault() != null
                                                                    ? inventoryDetailProductionLot.FirstOrDefault().id_metricUnit
                                                                    : (int?)null);
            var aMetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == aid_metricUnit);
            var balance = inventoryDetailProductionLot.FirstOrDefault()?.remainingBalance;

            var result = new
            {
                metricUnit = aMetricUnit?.code ?? "",
                currentStock = balance ?? 0
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemDetailDataLiquidationAndTrash(int id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var id_warehouseAux = item.ItemInventory?.Warehouse.id ?? 0;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var result = new
            {
                metricUnit = item.ItemPurchaseInformation?.MetricUnit.code ?? (metricUnitUMTP?.code ?? ""),
                id_warehouse = item.ItemInventory?.Warehouse.id ?? 0,
                id_warehouseLocation = item.ItemInventory?.WarehouseLocation.id ?? 0,
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouseAux)
                                       .Select(s => new
                                       {
                                           s.id,
                                           s.name
                                       })
            };

            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemDetailDataLiquidationAndLoss(int id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var id_warehouseAux = item.ItemInventory?.Warehouse.id ?? 0;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var result = new
            {
                metricUnit = item.ItemPurchaseInformation?.MetricUnit.code ?? (metricUnitUMTP?.code ?? ""),
                id_warehouse = item.ItemInventory?.Warehouse.id ?? 0,
                id_warehouseLocation = item.ItemInventory?.WarehouseLocation.id ?? 0,
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouseAux)
                                       .Select(s => new
                                       {
                                           s.id,
                                           s.name
                                       })
            };

            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductionLotTotals()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotLiquidation = (TempData["productionLotProcess"] as ProductionLot);
            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLotLiquidation = productionLotLiquidation ?? new ProductionLot();
            
            if (productionLotLiquidation != null || productionLot != null)
            {
                productionLotLiquidation.totalQuantityRecived = productionLotLiquidation.totalQuantityRecived > 0 ? productionLotLiquidation.totalQuantityRecived : productionLot != null ? productionLot.totalQuantityRecived : 0;
                productionLotLiquidation.totalQuantityLiquidation = productionLot != null ? productionLot.totalQuantityLiquidation : 0;
                productionLotLiquidation.totalQuantityTrash = productionLot != null ? productionLot.totalQuantityTrash : 0;
                productionLotLiquidation.totalQuantityLoss = productionLot != null ?  productionLot.totalQuantityLoss : 0;
            }

            TempData.Keep("productionLotProcess");
            TempData.Keep("productionLot");

            return Json(new
            {
                productionLotLiquidation.totalQuantityRecived,
                productionLotLiquidation.totalQuantityLiquidation,
                productionLotLiquidation.totalQuantityTrash,
                productionLotLiquidation.totalQuantityLoss
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotProcessItemDetails(int? id_originLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();
            TempData.Keep("productionLotProcess");

            return Json(productionLot.ProductionLotDetail.Where(pld => pld.id_originLot == id_originLot).Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotProcessLiquidationDetails()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();
            TempData.Keep("productionLotProcess");

            return Json(productionLot.ProductionLotLiquidation.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotProcessTrashDetails()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotTrash = productionLot.ProductionLotTrash ?? new List<ProductionLotTrash>();
            TempData.Keep("productionLotProcess");

            return Json(productionLot.ProductionLotTrash.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotProcessLossDetails()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLoss = productionLot.ProductionLotLoss ?? new List<ProductionLotLoss>();
            TempData.Keep("productionLotProcess");

            return Json(productionLot.ProductionLotLoss.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductionLotPerformances()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();

            TempData.Keep("productionLotProcess");

            var totalQuantityRecivedNet = productionLot.totalQuantityRecived;
            var performance = totalQuantityRecivedNet != 0 ? (productionLot.totalQuantityLiquidation / totalQuantityRecivedNet) : 0;

            return Json(new
            {
                totalQuantityRecivedNet,
                performance
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ExistsConversionWithLbs(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = db.Item.FirstOrDefault(p => p.id == id_item);

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;
            var id_metricUnitAux = item?.ItemPurchaseInformation?.MetricUnit.id ?? id_metricUnitLbsAux;

            var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
            return Json(new
            {
                metricUnitConversionValue = id_metricUnitAux == id_metricUnitLbsAux && id_metricUnitLbsAux != 0 ? 1 : metricUnitConversion?.factor ?? 0,
                metricUnitName = metricUnitUMTP?.name ?? "",
                metricUnitCode = metricUnitUMTP?.code ?? ""
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedLiquidation(int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var productionLotLiquidationAux = productionLot.ProductionLotLiquidation.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_warehouse == id_warehouseNew &&
                                                                                fod.id_warehouseLocation == id_warehouseLocationNew);
            if (productionLotLiquidationAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de esta Liquidación"
                };
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotProcessItemData(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = db.Item.FirstOrDefault(p => p.id == id_item);

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            return Json(new
            {
                metricUnit = item != null ? item.ItemPurchaseInformation?.MetricUnit.code ?? (metricUnitUMTP?.code ?? "") : ""
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProductionLotProcessWarehouseLocation(int? id_warehouse)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var result = new
            {
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse)
                                       .Select(s => new
                                       {
                                           s.id,
                                           s.name
                                       })
            };

            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetProductionLotProcessWarehouseLocation(int? id_warehouse, int? id_warehouseLocation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var warehouseLocationAux = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse).ToList();
            var warehouseLocationCurrentAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocation);
            if (warehouseLocationCurrentAux != null && !warehouseLocationAux.Contains(warehouseLocationCurrentAux)) warehouseLocationAux.Add(warehouseLocationCurrentAux);

            var result = new
            {
                warehouseLocations = warehouseLocationAux
                                       .Select(s => new
                                       {
                                           s.id,
                                           s.name
                                       })
            };

            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedTrash(int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var productionLotTrashAux = productionLot.ProductionLotTrash.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_warehouse == id_warehouseNew &&
                                                                                fod.id_warehouseLocation == id_warehouseLocationNew);
            if (productionLotTrashAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de Desperdicio"
                };
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedLoss(int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var productionLotLossAux = productionLot.ProductionLotLoss.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_warehouse == id_warehouseNew &&
                                                                                fod.id_warehouseLocation == id_warehouseLocationNew);
            if (productionLotLossAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de Merma"
                };
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItem(int? id_originLotNew, int? id_itemNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var productionLotDetailAux = productionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_originLot == id_originLotNew);

            if (productionLotDetailAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var productionLotAux = db.ProductionLot.FirstOrDefault(fod => fod.id == id_originLotNew);

                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en el lote: " + productionLotAux.internalNumber + ",  en los detalles de Materia Prima"
                };
            }

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [HttpPost, ValidateInput(false)]
        public JsonResult ItemPackingMaterialDetailsData(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                ItemDetailData = new
                {
                    item.masterCode,
                    metricUnit = item.ItemInventory?.MetricUnit?.code,
                }
            };

            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PackingMaterialDetails(int? id_itemCurrent)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotPackingMaterial = productionLot.ProductionLotPackingMaterial ?? new List<ProductionLotPackingMaterial>();

            var items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.InventoryLine.code.Equals("MI") && w.ItemType.code.Equals("INS") && w.ItemTypeCategory.code.Equals("MDE")) || w.id == id_itemCurrent).ToList();
            var tempItems = new List<Item>();
            foreach (var i in items)
            {
                if (!(productionLot.ProductionLotPackingMaterial.Any(a => a.id_item == i.id && a.quantityRequiredForProductionLot != 0)) || i.id == id_itemCurrent)
                {
                    tempItems.Add(i);
                }
            }
            items = tempItems;
            var result = new
            {
                items = items.Select(s => new
                {
                    s.id,
                    s.masterCode,
                    ItemInventoryMetricUnitCode = (s.ItemInventory != null) ? s.ItemInventory.MetricUnit.code : "",
                    s.name
                }).ToList(),
                Message = "Ok"
            };

            TempData.Keep("productionLotProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void UpdateProductionLotLiquidationPackingMaterialDetail(ProductionLot productionLot, ProductionLotLiquidation productionLotLiquidation)
        {
            for (int i = productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Count - 1; i >= 0; i--)
            {
                var detail = productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.ElementAt(i);

                detail.ProductionLotPackingMaterial.quantityRequiredForProductionLot -= detail.quantity;
                detail.ProductionLotPackingMaterial.manual = detail.ProductionLotPackingMaterial.quantityRequiredForProductionLot == 0;
                detail.ProductionLotPackingMaterial.quantity -= detail.quantity;

                productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Remove(detail);
                try
                {
                    db.ProductionLotLiquidationPackingMaterialDetail.Attach(detail);
                    db.Entry(detail).State = EntityState.Deleted;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (!productionLot.ProductionLotLiquidation.Any(a => a.id == productionLotLiquidation.id)) return;

            if (productionLotLiquidation.Item == null)
            {
                productionLotLiquidation.Item = db.Item.FirstOrDefault(fod => fod.id == productionLotLiquidation.id_item);
            }
            var itemIngredientMDE = productionLotLiquidation.Item.ItemIngredient.Where(w => w.Item1.InventoryLine.code.Equals("MI") && w.Item1.ItemType.code.Equals("INS") && w.Item1.ItemTypeCategory.code.Equals("MDE"));
            if (itemIngredientMDE.Count() == 0) return;
            var id_metricUnitLiquidation = productionLotLiquidation.id_metricUnit;
            var id_metricUnitItemHeadIngredient = productionLotLiquidation.Item.ItemHeadIngredient?.id_metricUnit;
            var factorConversionLiquidationFormulation = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                             muc.id_metricOrigin == id_metricUnitLiquidation &&
                                                                                             muc.id_metricDestiny == id_metricUnitItemHeadIngredient);
            if (id_metricUnitLiquidation != null && id_metricUnitLiquidation == id_metricUnitItemHeadIngredient)
            {
                factorConversionLiquidationFormulation = new MetricUnitConversion() { factor = 1 };
            }
            if (factorConversionLiquidationFormulation == null)
            {
                var metricUnitLiquidation = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitLiquidation);
                throw new Exception("Falta el Factor de Conversión entre : " + (metricUnitLiquidation?.code ?? "(UM No Existe)") + ", del Ítem: " + productionLotLiquidation.Item.name + " y " + (productionLotLiquidation.Item.ItemHeadIngredient?.MetricUnit?.code ?? "(UM No Existe)") + " configurado en la cabecera de la formulación del este Ítem. Necesario para cargar los Materiales de Empaque Configúrelo, e intente de nuevo");
            }

            foreach (var iimdd in itemIngredientMDE)
            {
                var quantityMetricUnitItemHeadIngredient = productionLotLiquidation.quantity * factorConversionLiquidationFormulation.factor;
                var amountItemHeadIngredient = (productionLotLiquidation.Item.ItemHeadIngredient?.amount ?? 0);
                if (amountItemHeadIngredient == 0)
                {
                    throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + productionLotLiquidation.Item.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
                }
                var quantityItemIngredientMDE = (quantityMetricUnitItemHeadIngredient * (iimdd.amount ?? 0)) / amountItemHeadIngredient;
                if (quantityItemIngredientMDE == 0) continue;

                var truncateQuantityItemIngredientMDE = decimal.Truncate(quantityItemIngredientMDE);
                if ((quantityItemIngredientMDE - truncateQuantityItemIngredientMDE) > 0)
                {
                    quantityItemIngredientMDE = truncateQuantityItemIngredientMDE + 1;
                };
                var id_metricUnitFormulation = iimdd.id_metricUnit;
                var id_metricUnitInventory = iimdd.Item1.ItemInventory?.id_metricUnitInventory;
                var factorConversionFormulationInventory = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                                  muc.id_metricOrigin == id_metricUnitFormulation &&
                                                                                                  muc.id_metricDestiny == id_metricUnitInventory);
                if (id_metricUnitFormulation != null && id_metricUnitFormulation == id_metricUnitInventory)
                {
                    factorConversionFormulationInventory = new MetricUnitConversion() { factor = 1 };
                }
                if (factorConversionFormulationInventory == null)
                {
                    throw new Exception("Falta el Factor de Conversión entre : " + iimdd.MetricUnit?.code ?? "(UM No Existe)" + ", del Ítem: " + iimdd.Item1.name + " y " + iimdd.Item1.ItemInventory?.MetricUnit.code ?? "(UM No Existe)" + " configurado en el detalle de la formulación del Ítem: " + productionLotLiquidation.Item.name + ". Necesario para cargar los Materiales de Empaque Configúrelo, e intente de nuevo");
                }

                var quantityUMInventory = quantityItemIngredientMDE * factorConversionFormulationInventory.factor;

                var truncateQuantityUMInventory = decimal.Truncate(quantityUMInventory);
                if ((quantityUMInventory - truncateQuantityUMInventory) > 0)
                {
                    quantityUMInventory = truncateQuantityUMInventory + 1;
                };

                ProductionLotPackingMaterial productionLotPackingMaterial = productionLot.ProductionLotPackingMaterial.Where(w => w.isActive).FirstOrDefault(fod => fod.id_item == iimdd.id_ingredientItem);
                if (productionLotPackingMaterial != null)
                {
                    productionLotPackingMaterial.quantityRequiredForProductionLot += quantityUMInventory;
                    productionLotPackingMaterial.quantity += quantityUMInventory;
                    productionLotPackingMaterial.manual = false;
                    productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                    productionLotPackingMaterial.dateUpdate = DateTime.Now;
                }
                else
                {
                    productionLotPackingMaterial = new ProductionLotPackingMaterial
                    {
                        id = productionLot.ProductionLotPackingMaterial.Count() > 0 ? productionLot.ProductionLotPackingMaterial.Max(pld => pld.id) + 1 : 1,
                        id_item = iimdd.id_ingredientItem,
                        Item = db.Item.FirstOrDefault(i => i.id == iimdd.id_ingredientItem),
                        quantityRequiredForProductionLot = quantityUMInventory,
                        quantity = quantityUMInventory,
                        manual = false,
                        isActive = true,
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now,
                        ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>()
                    };
                    productionLot.ProductionLotPackingMaterial.Add(productionLotPackingMaterial);
                }

                var productionLotLiquidationPackingMaterialDetail = new ProductionLotLiquidationPackingMaterialDetail
                {
                    ProductionLotLiquidation = productionLotLiquidation,
                    id_productionLotLiquidation = productionLotLiquidation.id,
                    ProductionLotPackingMaterial = productionLotPackingMaterial,
                    id_productionLotPackingMaterial = productionLotPackingMaterial.id,
                    quantity = quantityUMInventory
                };
                productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
                productionLotPackingMaterial.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
            }
        }

        #endregion AXILIAR FUNCTIONS

        #region Método auxiliar para crear/actualizar el registro de romaneo

        private void ActualizarRegistroRomaneo(ProductionLot productionLot)
        {
            var resultProdRomaneo = productionLot.ResultProdLotRomaneo;

            if (resultProdRomaneo == null)
            {
                resultProdRomaneo = new ResultProdLotRomaneo();
                productionLot.ResultProdLotRomaneo = resultProdRomaneo;
            }

            resultProdRomaneo.numberLot = productionLot.internalNumber;
            resultProdRomaneo.numberLotSequential = productionLot.number;
            resultProdRomaneo.nameProvider = productionLot.Provider?.Person?.fullname_businessName ?? string.Empty;
            resultProdRomaneo.nameProviderShrimp = db.ProductionUnitProvider.FirstOrDefault(p => p.id == productionLot.id_productionUnitProvider)?.name ?? string.Empty;
            resultProdRomaneo.namePool = db.ProductionUnitProviderPool.FirstOrDefault(p => p.id == productionLot.id_productionUnitProviderPool)?.name ?? string.Empty;
            resultProdRomaneo.INPnumber = productionLot.INPnumberPL ?? string.Empty;
            resultProdRomaneo.dateTimeReception = productionLot.receptionDate;

            var firstProductionLotDetail = productionLot.ProductionLotDetail.First();
            var firstProductionLotDetailItem = firstProductionLotDetail.Item;

            resultProdRomaneo.nameItem = firstProductionLotDetailItem.name;
            resultProdRomaneo.nameWarehouseItem = firstProductionLotDetail.Warehouse.name;
            resultProdRomaneo.nameWarehouseLocationItem = firstProductionLotDetail.WarehouseLocation.name;
            resultProdRomaneo.codeProcessType = firstProductionLotDetailItem.ItemProcessType.FirstOrDefault()?.ProcessType?.code;
            resultProdRomaneo.quantityRemitted = productionLot.totalQuantityRecived;
            resultProdRomaneo.idMetricUnit = firstProductionLotDetailItem.ItemPurchaseInformation?.id_metricUnitPurchase ?? 4;
            resultProdRomaneo.gavetaNumber = firstProductionLotDetail.drawersNumber ?? 0;
        }

        #endregion Método auxiliar para crear/actualizar el registro de romaneo

        #region << Cambios 2021 >>

        [HttpPost, ValidateInput(false)]
        public ActionResult SetProductionProcessId(int? idProductionProcess, ProductionLot productionLotClient)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot) ?? new ProductionLot();
            productionLot.id_productionProcess = (productionLotClient?.id_productionProcess ?? 0);
            string solicitaMaquina = db.Setting.FirstOrDefault(fod => fod.code == "SMAQPINT")?.value ?? "NO";
            ProductionProcess proceso = db.ProductionProcess
                                                .FirstOrDefault(r => r.id == productionLot.id_productionProcess);
            if (proceso != null)
            {
                productionLot.id_productionUnit = (proceso.id_ProductionUnit ?? 0);
                productionLot.id_productionProcess = proceso.id;
                productionLot.ProductionProcess = proceso;
            }
            else
            {
                productionLot.id_productionProcess = idProductionProcess ?? 0;
                productionLot.ProductionProcess = db.ProductionProcess.FirstOrDefault(r => r.id == (idProductionProcess ?? 0));
            }
            productionLot.id_productionUnit = (productionLotClient?.id_productionUnit ?? 0);
            productionLot.reference = (productionLotClient?.reference ?? string.Empty);
            productionLot.description = (productionLotClient?.description ?? string.Empty);
            productionLot.totalQuantityRecived = (productionLotClient?.totalQuantityRecived ?? 0);
            productionLot.totalQuantityLiquidation = (productionLotClient?.totalQuantityLiquidation ?? 0);
            productionLot.totalQuantityTrash = (productionLotClient?.totalQuantityTrash ?? 0);
            productionLot.totalQuantityLoss = (productionLotClient?.totalQuantityLoss ?? 0);

            productionLot.receptionDate = (productionLotClient?.receptionDate ?? DateTime.Now);
            productionLot.expirationDate = (productionLotClient?.expirationDate ?? DateTime.Now);

            var strJul = productionLotClient.julianoNumber;
            var strInt = productionLotClient.internalNumber;

            productionLot.internalNumberConcatenated = $"{productionLotClient.julianoNumber}{productionLotClient.internalNumber}";
            productionLot.internalNumber = productionLot.internalNumberConcatenated;
            productionLot.julianoNumber = productionLotClient?.julianoNumber;

            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.requestliquidationmachine == true)
            {
                ViewBag.solicitaMaquina = true;
            }
            else
            {
                ViewBag.solicitaMaquina = false;
            }
            if (solicitaMaquina == "SI" && productionLot.ProductionProcess.generateTransfer == true)
            {
                ViewBag.generaTransferencia = true;
            }
            else
            {
                ViewBag.generaTransferencia = false;
            }
            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            return ProductionLotProcessFormEditPartial(productionLot.id, productionLot
                                                                                .ProductionLotDetail
                                                                                .Select(r => r.id)
                                                                                .ToArray(), (string[])TempData["arrayTempDataKeep"], true, 0);
        }

        public JsonResult UpdateProductionLotJulianoNumber(int? id_pl, string intNumber, int yearR, int monthR, int dayR, DateTime emissionDate)
        {
            ProductionLot productionLot = (TempData["productionLotProcess"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(fod => fod.id == id_pl);
            productionLot = productionLot ?? new ProductionLot();

            DateTime? dateReceptionNow = new DateTime(yearR, monthR, dayR);

            dateReceptionNow = dateReceptionNow ?? DateTime.Now;

            productionLot.julianoNumber = DataProviderJulianoNumber.GetJulianoNumber(dateReceptionNow.Value);
            productionLot.internalNumber = intNumber;

            productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;
            productionLot.receptionDate = emissionDate;
            TempData["productionLotProcess"] = productionLot;
            TempData.Keep("productionLotProcess");

            return Json(new
            {
                julianoNumberTmp = productionLot.julianoNumber,
                internalNumberTmp = productionLot.internalNumber,
                internalNumberConcatenatedTmp = productionLot.internalNumberConcatenated
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion << Cambios 2021 >>

        #region << Reportes >>

        public JsonResult PrintEgreso(int id, string codeReport)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param;
            _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id
            };
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        public JsonResult PrintIngreso(int id, string codeReport)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param;
            _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id
            };
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        public JsonResult PrintLiquidacionProcesoInterno(string codeReport)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        #endregion << Reportes >>

        public JsonResult PrintResume(string codeReport, ProductionLot productionLot,
                                        int? id_ProductionLotState_VI,
                                        string id_productionUnit,
                                        string id_productionProcess,
                                        string warehouse,
                                        string warehouseLocation,
                                        int? Estado, string Nlote, string Ninterno, string Unidad,
                                        string Proceso, int[] Items,
                                        DateTime? startReceptionDate, DateTime? endReceptionDate
                                        )
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@estado",
                Valor = (id_ProductionLotState_VI.HasValue && id_ProductionLotState_VI != -1) ? id_ProductionLotState_VI.Value : 0
            };

            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@nlote",
                Valor = productionLot?.number ?? ""
            };

            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@ninterno",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            string productionUnitName = string.Empty;
            if (productionLot.id_productionUnit > 0)
            {
                var _productionUnit = db.ProductionUnit
                                            .FirstOrDefault(r => r.id == productionLot.id_productionUnit);
                if (_productionUnit != null)
                {
                    productionUnitName = _productionUnit.name;
                }
            }

            _param = new ParamCR
            {
                Nombre = "@unidad",
                Valor = productionUnitName
            };

            paramLst.Add(_param);

            string productionProcessName = string.Empty;
            if (productionLot.id_productionProcess > 0)
            {
                var _productionProcess = db.ProductionProcess
                                            .FirstOrDefault(r => r.id == productionLot.id_productionProcess);
                if (_productionProcess != null)
                {
                    productionProcessName = _productionProcess.name;
                }
            }
            _param = new ParamCR
            {
                Nombre = "@proceso",
                Valor = productionProcessName
            };

            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@producto",
                Valor = ""
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@bodega",
                Valor = warehouse ?? ""
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@ubicacion",
                Valor = warehouseLocation ?? ""
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            var procedureName = "";

            if(codeReport == "PIDET")
            {
                procedureName = "Par_ProcesosInternosDetallado";
            }

            if (codeReport == "PIRES")
            {
                procedureName = "Par_ProcesosInternosDetalladopxp";
            }

            db.Database.CommandTimeout = 2200;
            List<ResultProductLotProcessDetailed> modelAux = new List<ResultProductLotProcessDetailed>();
            modelAux = db.Database.SqlQuery<ResultProductLotProcessDetailed>
                    ("exec " + procedureName + " @id,@estado,@nlote,@ninterno," +
                    "@unidad,@proceso,@producto,@bodega,@ubicacion,@fi,@ff", 
                    new SqlParameter("id", paramLst[0].Valor),
                    new SqlParameter("estado", paramLst[1].Valor),
                    new SqlParameter("nlote", paramLst[2].Valor),
                    new SqlParameter("ninterno", paramLst[3].Valor),
                    new SqlParameter("unidad", paramLst[4].Valor),
                    new SqlParameter("proceso", paramLst[5].Valor),
                    new SqlParameter("producto", paramLst[6].Valor),
                    new SqlParameter("bodega", paramLst[7].Valor),
                    new SqlParameter("ubicacion", paramLst[8].Valor),
                    new SqlParameter("fi", paramLst[9].Valor),
                    new SqlParameter("ff", paramLst[10].Valor)
                    )
                    .OrderBy(obd => obd.IDPL).ThenBy(tbd => tbd.IDPL).ToList();

            TempData["modelProcessInternalDetailed"] = modelAux;
            TempData["par_initialDate"] = startReceptionDate.Value.Date.ToString("dd/MM/yyyy");
            TempData["par_endDate"] = endReceptionDate.Value.Date.ToString("dd/MM/yyyy");
            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        public JsonResult PrintResume2(string codeReport, ProductionLot productionLot,
                                        int? id_ProductionLotState_VI,
                                        string id_productionUnit,
                                        string id_productionProcess,

                                        int? Estado, string Unidad,
                                        string Proceso,
                                        DateTime? startReceptionDate, DateTime? endReceptionDate
                                        )
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@estado",
                Valor = (id_ProductionLotState_VI.HasValue && id_ProductionLotState_VI != -1) ? id_ProductionLotState_VI.Value : 0
            };

            paramLst.Add(_param);

            string productionUnitName = string.Empty;
            if (productionLot.id_productionUnit > 0)
            {
                var _productionUnit = db.ProductionUnit
                                            .FirstOrDefault(r => r.id == productionLot.id_productionUnit);
                if (_productionUnit != null)
                {
                    productionUnitName = _productionUnit.name;
                }
            }

            _param = new ParamCR
            {
                Nombre = "@unidad",
                Valor = productionUnitName
            };

            paramLst.Add(_param);

            string productionProcessName = string.Empty;
            if (productionLot.id_productionProcess > 0)
            {
                var _productionProcess = db.ProductionProcess
                                            .FirstOrDefault(r => r.id == productionLot.id_productionProcess);
                if (_productionProcess != null)
                {
                    productionProcessName = _productionProcess.name;
                }
            }
            _param = new ParamCR
            {
                Nombre = "@proceso",
                Valor = productionProcessName
            };

            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        public JsonResult PrintMovimientoCosto(string codeReport,
                                                DateTime? startReceptionDate,
                                                DateTime? endReceptionDate)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = 0
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate == null) { str_starReceptionDate = DateTime.Now.ToString("yyyy/MM/dd"); } 
            else { str_starReceptionDate = startReceptionDate?.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fechaInicio",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate == null) { str_endReceptionDate =  DateTime.Now.ToString("yyyy/MM/dd"); }
            else { str_endReceptionDate = endReceptionDate?.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fechaFin",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            db.Database.CommandTimeout = 2200;

            List<ResultMovementCost> modelAux = new List<ResultMovementCost>();
            modelAux = db.Database.SqlQuery<ResultMovementCost>
                    ("exec pi_Consultar_Movimientos_Procesos_Internos_Costo @id, @fechaInicio, @fechaFin",
                    new SqlParameter("id", paramLst[0].Valor),
                    new SqlParameter("fechaInicio", paramLst[1].Valor),
                    new SqlParameter("fechaFin", paramLst[2].Valor)
                    ).ToList();

            TempData["modelMovementCost"] = modelAux;
            TempData["repMod"] = _repMod;
            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        public JsonResult PrintMatrixProcesosInternos(string codeReport,
                                                DateTime? startReceptionDate,
                                                DateTime? endReceptionDate)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@fechaInicio",
                Valor = startReceptionDate
            };
            paramLst.Add(_param);
            _param = new ParamCR
            {
                Nombre = "@fechaFin",
                Valor = endReceptionDate
            };
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        private class ResultInternalProcessSaldo
        {
            public int id_warehouse { get; set; }

            public string Bodega { get; set; }

            public long IdMes { get; set; }
            public string Mes { get; set; }

            public string Tipo { get; set; }

            public decimal CantidadLbs { get; set; }

            public decimal Costo { get; set; }
        }

        public JsonResult InternalProcessesMatrix(DateTime? startReceptionDate,
                                                DateTime? endReceptionDate,
                                                int? warehouse, int? filterWarehouse,
                                                int? warehouseLocation, int? filterWarehouseLocation)
        {
            Parametros.ParametrosBusquedaInternalProcessSaldo parametrosBusquedaInternalProcessSaldo = new Parametros.ParametrosBusquedaInternalProcessSaldo();
            parametrosBusquedaInternalProcessSaldo.startEmissionDate = startReceptionDate;
            parametrosBusquedaInternalProcessSaldo.endEmissionDate = endReceptionDate;
            if (warehouse != null) parametrosBusquedaInternalProcessSaldo.id_warehouse = warehouse;
            if (filterWarehouse != null) parametrosBusquedaInternalProcessSaldo.id_warehouse = filterWarehouse;
            if (warehouseLocation != null) parametrosBusquedaInternalProcessSaldo.id_warehouseLocation = warehouseLocation;
            if (filterWarehouseLocation != null) parametrosBusquedaInternalProcessSaldo.id_warehouseLocation = filterWarehouseLocation;
            parametrosBusquedaInternalProcessSaldo.id_user = ActiveUser.id;

            var parametrosBusquedaInternalProcessSaldoAux = new SqlParameter();
            parametrosBusquedaInternalProcessSaldoAux.ParameterName = "@ParametrosBusquedaInternalProcessSaldo";
            parametrosBusquedaInternalProcessSaldoAux.Direction = ParameterDirection.Input;
            parametrosBusquedaInternalProcessSaldoAux.SqlDbType = SqlDbType.NVarChar;
            var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaInternalProcessSaldo);
            parametrosBusquedaInternalProcessSaldoAux.Value = jsonAux;
            db.Database.CommandTimeout = 1200;
            List<ResultInternalProcessSaldo> modelAux = db.Database.SqlQuery<ResultInternalProcessSaldo>("exec inv_Consultar_Internal_Process_Saldo_StoredProcedure @ParametrosBusquedaInternalProcessSaldo ", parametrosBusquedaInternalProcessSaldoAux).ToList();

            Session["startReceptionDate"] = startReceptionDate;
            Session["endReceptionDate"] = endReceptionDate;
            Session["ResultInternalProcessSaldo"] = modelAux;

            var result = new { mensaje = "OK" };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public SLDocument getData()
        {
            SLDocument sl = new SLDocument();
            SLStyle estiloBackGround = sl.CreateStyle();
            estiloBackGround.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
            for (int i = 1; i < 101; i++)
            {
                string[] letras = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                                                 "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                                                 "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD"};
                for (int j = 0; j < 30; j++)
                {
                    sl.SetCellStyle(letras[j] + i, estiloBackGround);
                }
            }
            SLPicture pic = new SLPicture(ActiveCompany.logo, DocumentFormat.OpenXml.Packaging.ImagePartType.Png);
            pic.SetPosition(0, 0);
            pic.ResizeInPixels(200, 100);
            sl.InsertPicture(pic);

            SLStyle estiloC1 = sl.CreateStyle();
            estiloC1.Font.FontName = "Arial";
            estiloC1.Font.FontSize = 12;
            estiloC1.Font.Bold = true;
            estiloC1.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
            estiloC1.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center;
            sl.SetCellValue("F1", "Compañía:");
            sl.SetCellStyle("F1", estiloC1);

            sl.SetCellValue("F3", "División:");
            sl.SetCellStyle("F3", estiloC1);

            sl.SetCellValue("F5", "Sucursal:");
            sl.SetCellStyle("F5", estiloC1);

            sl.SetCellValue("H5", "Fecha Inicio:");
            sl.SetCellStyle("H5", estiloC1);

            sl.SetCellValue("K5", "Fecha Final:");
            sl.SetCellStyle("K5", estiloC1);

            sl.SetCellValue("N1", "Fecha:");
            sl.SetCellStyle("N1", estiloC1);

            sl.SetCellValue("N3", "Hora:");
            sl.SetCellStyle("N3", estiloC1);

            sl.SetCellValue("N5", "Usuario:");
            sl.SetCellStyle("N5", estiloC1);

            SLStyle estiloC2 = sl.CreateStyle();
            estiloC2.Font.FontName = "Arial";
            estiloC2.Font.FontSize = 12;
            estiloC2.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
            estiloC2.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center;
            sl.SetCellValue("G1", ActiveCompany.businessName);
            sl.SetCellStyle("G1", estiloC2);

            sl.SetCellValue("G3", ActiveDivision.name);
            sl.SetCellStyle("G3", estiloC2);

            sl.SetCellValue("G5", ActiveSucursal.name);
            sl.SetCellStyle("G5", estiloC2);

            var aStartReceptionDate = Session["startReceptionDate"] as DateTime?;
            sl.SetCellValue("I5", aStartReceptionDate == null ? "Todas" : aStartReceptionDate.Value.ToString("dd/MM/yyyy"));
            sl.SetCellStyle("I5", estiloC2);

            var aEndReceptionDate = Session["endReceptionDate"] as DateTime?;
            sl.SetCellValue("L5", aEndReceptionDate == null ? "Todas" : aEndReceptionDate.Value.ToString("dd/MM/yyyy"));
            sl.SetCellStyle("L5", estiloC2);

            var hoy = DateTime.Now;
            sl.SetCellValue("O1", hoy.ToString("dd/MM/yyyy"));
            sl.SetCellStyle("O1", estiloC2);

            sl.SetCellValue("O3", hoy.ToString("HH:mm"));
            sl.SetCellStyle("O3", estiloC2);

            sl.SetCellValue("O5", ActiveUser.username);
            sl.SetCellStyle("O5", estiloC2);

            SLStyle estiloT = sl.CreateStyle();
            estiloT.Font.FontName = "Arial";
            estiloT.Font.FontSize = 20;
            estiloT.Font.Bold = true;
            estiloT.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
            estiloT.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center;
            sl.SetCellValue("H1", "PROCESOS INTERNOS");
            sl.SetCellStyle("H1", estiloT);
            sl.MergeWorksheetCells("H1", "M4");

            sl.AutoFitColumn("F", "G");
            sl.AutoFitColumn("H", "I");
            sl.AutoFitColumn("K", "L");
            sl.AutoFitColumn("N", "O");

            var aResultInternalProcessSaldo = Session["ResultInternalProcessSaldo"] as List<ResultInternalProcessSaldo>;
            var aCabeceraMeses = aResultInternalProcessSaldo.Select(s => new { s.IdMes, s.Mes }).Distinct().OrderBy(ob => ob.IdMes).ToList();

            SLStyle estiloTC1 = sl.CreateStyle();
            estiloTC1.Font.FontName = "Arial";
            estiloTC1.Font.FontSize = 12;
            estiloTC1.Font.Bold = true;
            estiloTC1.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
            estiloTC1.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Bottom;
            estiloTC1.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloTC1.Border.LeftBorder.Color = System.Drawing.Color.Black;
            estiloTC1.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloTC1.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloTC1.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            sl.SetCellValue("A9", "Bodegas");
            sl.SetCellStyle("A9", estiloTC1);
            sl.SetCellStyle("B9", estiloTC1);
            sl.SetCellStyle("C9", estiloTC1);
            sl.SetCellStyle("A10", estiloTC1);
            sl.SetCellStyle("B10", estiloTC1);
            sl.SetCellStyle("C10", estiloTC1);
            sl.MergeWorksheetCells("A9", "C10");

            int count = 0;
            int countShow = 0;
            foreach (var item in aCabeceraMeses)
            {
                count++;
                switch (count)
                {
                    case 1:
                        {
                            countShow++;
                            sl.SetCellValue("D9", item.Mes);
                            sl.SetCellStyle("D9", estiloTC1);
                            sl.SetCellStyle("E9", estiloTC1);
                            sl.MergeWorksheetCells("D9", "E9");

                            sl.SetCellValue("D10", "Cantidad Lbs");
                            sl.SetCellStyle("D10", estiloTC1);
                            sl.SetCellValue("E10", "Costo");
                            sl.SetCellStyle("E10", estiloTC1);
                            sl.AutoFitColumn("D", "E");
                        }
                        break;

                    case 2:
                        {
                            countShow++;
                            sl.SetCellValue("F9", item.Mes);
                            sl.SetCellStyle("F9", estiloTC1);
                            sl.SetCellStyle("G9", estiloTC1);
                            sl.MergeWorksheetCells("F9", "G9");

                            sl.SetCellValue("F10", "Cantidad Lbs");
                            sl.SetCellStyle("F10", estiloTC1);
                            sl.SetCellValue("G10", "Costo");
                            sl.SetCellStyle("G10", estiloTC1);
                            sl.AutoFitColumn("F", "G");
                        }
                        break;

                    case 3:
                        {
                            countShow++;
                            sl.SetCellValue("H9", item.Mes);
                            sl.SetCellStyle("H9", estiloTC1);
                            sl.SetCellStyle("I9", estiloTC1);
                            sl.MergeWorksheetCells("H9", "I9");

                            sl.SetCellValue("H10", "Cantidad Lbs");
                            sl.SetCellStyle("H10", estiloTC1);
                            sl.SetCellValue("I10", "Costo");
                            sl.SetCellStyle("I10", estiloTC1);
                            sl.AutoFitColumn("H", "I");
                        }
                        break;

                    case 4:
                        {
                            countShow++;
                            sl.SetCellValue("J9", item.Mes);
                            sl.SetCellStyle("J9", estiloTC1);
                            sl.SetCellStyle("K9", estiloTC1);
                            sl.MergeWorksheetCells("J9", "K9");

                            sl.SetCellValue("J10", "Cantidad Lbs");
                            sl.SetCellStyle("J10", estiloTC1);
                            sl.SetCellValue("K10", "Costo");
                            sl.SetCellStyle("K10", estiloTC1);
                            sl.AutoFitColumn("J", "K");
                        }
                        break;

                    case 5:
                        {
                            countShow++;
                            sl.SetCellValue("L9", item.Mes);
                            sl.SetCellStyle("L9", estiloTC1);
                            sl.SetCellStyle("M9", estiloTC1);
                            sl.MergeWorksheetCells("L9", "M9");

                            sl.SetCellValue("L10", "Cantidad Lbs");
                            sl.SetCellStyle("L10", estiloTC1);
                            sl.SetCellValue("M10", "Costo");
                            sl.SetCellStyle("M10", estiloTC1);
                            sl.AutoFitColumn("L", "M");
                        }
                        break;

                    case 6:
                        {
                            countShow++;
                            sl.SetCellValue("N9", item.Mes);
                            sl.SetCellStyle("N9", estiloTC1);
                            sl.SetCellStyle("O9", estiloTC1);
                            sl.MergeWorksheetCells("N9", "O9");

                            sl.SetCellValue("N10", "Cantidad Lbs");
                            sl.SetCellStyle("N10", estiloTC1);
                            sl.SetCellValue("O10", "Costo");
                            sl.SetCellStyle("O10", estiloTC1);
                            sl.AutoFitColumn("N", "O");
                        }
                        break;

                    case 7:
                        {
                            countShow++;
                            sl.SetCellValue("P9", item.Mes);
                            sl.SetCellStyle("P9", estiloTC1);
                            sl.SetCellStyle("Q9", estiloTC1);
                            sl.MergeWorksheetCells("P9", "Q9");

                            sl.SetCellValue("P10", "Cantidad Lbs");
                            sl.SetCellStyle("P10", estiloTC1);
                            sl.SetCellValue("Q10", "Costo");
                            sl.SetCellStyle("Q10", estiloTC1);
                            sl.AutoFitColumn("P", "Q");
                        }
                        break;

                    case 8:
                        {
                            countShow++;
                            sl.SetCellValue("R9", item.Mes);
                            sl.SetCellStyle("R9", estiloTC1);
                            sl.SetCellStyle("S9", estiloTC1);
                            sl.MergeWorksheetCells("R9", "S9");

                            sl.SetCellValue("R10", "Cantidad Lbs");
                            sl.SetCellStyle("R10", estiloTC1);
                            sl.SetCellValue("S10", "Costo");
                            sl.SetCellStyle("S10", estiloTC1);
                            sl.AutoFitColumn("R", "S");
                        }
                        break;

                    case 9:
                        {
                            countShow++;
                            sl.SetCellValue("T9", item.Mes);
                            sl.SetCellStyle("T9", estiloTC1);
                            sl.SetCellStyle("U9", estiloTC1);
                            sl.MergeWorksheetCells("T9", "U9");

                            sl.SetCellValue("T10", "Cantidad Lbs");
                            sl.SetCellStyle("T10", estiloTC1);
                            sl.SetCellValue("U10", "Costo");
                            sl.SetCellStyle("U10", estiloTC1);
                            sl.AutoFitColumn("T", "U");
                        }
                        break;

                    case 10:
                        {
                            countShow++;
                            sl.SetCellValue("V9", item.Mes);
                            sl.SetCellStyle("V9", estiloTC1);
                            sl.SetCellStyle("W9", estiloTC1);
                            sl.MergeWorksheetCells("V9", "W9");

                            sl.SetCellValue("V10", "Cantidad Lbs");
                            sl.SetCellStyle("V10", estiloTC1);
                            sl.SetCellValue("W10", "Costo");
                            sl.SetCellStyle("W10", estiloTC1);
                            sl.AutoFitColumn("V", "W");
                        }
                        break;

                    case 11:
                        {
                            countShow++;
                            sl.SetCellValue("X9", item.Mes);
                            sl.SetCellStyle("X9", estiloTC1);
                            sl.SetCellStyle("Y9", estiloTC1);
                            sl.MergeWorksheetCells("X9", "Y9");

                            sl.SetCellValue("X10", "Cantidad Lbs");
                            sl.SetCellStyle("X10", estiloTC1);
                            sl.SetCellValue("Y10", "Costo");
                            sl.SetCellStyle("Y10", estiloTC1);
                            sl.AutoFitColumn("X", "Y");
                        }
                        break;

                    case 12:
                        {
                            countShow++;
                            sl.SetCellValue("Z9", item.Mes);
                            sl.SetCellStyle("Z9", estiloTC1);
                            sl.SetCellStyle("AA9", estiloTC1);
                            sl.MergeWorksheetCells("Z9", "AA9");

                            sl.SetCellValue("Z10", "Cantidad Lbs");
                            sl.SetCellStyle("Z10", estiloTC1);
                            sl.SetCellValue("AA10", "Costo");
                            sl.SetCellStyle("AA10", estiloTC1);
                            sl.AutoFitColumn("Z", "AA");
                        }
                        break;

                    default:
                        break;
                }
            }
            countShow++;
            switch (countShow)
            {
                case 2:
                    {
                        sl.SetCellValue("F9", "Total");
                        sl.SetCellStyle("F9", estiloTC1);
                        sl.SetCellStyle("G9", estiloTC1);
                        sl.MergeWorksheetCells("F9", "G9");

                        sl.SetCellValue("F10", "Cantidad Lbs");
                        sl.SetCellStyle("F10", estiloTC1);
                        sl.SetCellValue("G10", "Costo");
                        sl.SetCellStyle("G10", estiloTC1);
                        sl.AutoFitColumn("F", "G");
                    }
                    break;

                case 3:
                    {
                        sl.SetCellValue("H9", "Total");
                        sl.SetCellStyle("H9", estiloTC1);
                        sl.SetCellStyle("I9", estiloTC1);
                        sl.MergeWorksheetCells("H9", "I9");

                        sl.SetCellValue("H10", "Cantidad Lbs");
                        sl.SetCellStyle("H10", estiloTC1);
                        sl.SetCellValue("I10", "Costo");
                        sl.SetCellStyle("I10", estiloTC1);
                        sl.AutoFitColumn("H", "I");
                    }
                    break;

                case 4:
                    {
                        sl.SetCellValue("J9", "Total");
                        sl.SetCellStyle("J9", estiloTC1);
                        sl.SetCellStyle("K9", estiloTC1);
                        sl.MergeWorksheetCells("J9", "K9");

                        sl.SetCellValue("J10", "Cantidad Lbs");
                        sl.SetCellStyle("J10", estiloTC1);
                        sl.SetCellValue("K10", "Costo");
                        sl.SetCellStyle("K10", estiloTC1);
                        sl.AutoFitColumn("J", "K");
                    }
                    break;

                case 5:
                    {
                        sl.SetCellValue("L9", "Total");
                        sl.SetCellStyle("L9", estiloTC1);
                        sl.SetCellStyle("M9", estiloTC1);
                        sl.MergeWorksheetCells("L9", "M9");

                        sl.SetCellValue("L10", "Cantidad Lbs");
                        sl.SetCellStyle("L10", estiloTC1);
                        sl.SetCellValue("M10", "Costo");
                        sl.SetCellStyle("M10", estiloTC1);
                        sl.AutoFitColumn("L", "M");
                    }
                    break;

                case 6:
                    {
                        sl.SetCellValue("N9", "Total");
                        sl.SetCellStyle("N9", estiloTC1);
                        sl.SetCellStyle("O9", estiloTC1);
                        sl.MergeWorksheetCells("N9", "O9");

                        sl.SetCellValue("N10", "Cantidad Lbs");
                        sl.SetCellStyle("N10", estiloTC1);
                        sl.SetCellValue("O10", "Costo");
                        sl.SetCellStyle("O10", estiloTC1);
                        sl.AutoFitColumn("N", "O");
                    }
                    break;

                case 7:
                    {
                        sl.SetCellValue("P9", "Total");
                        sl.SetCellStyle("P9", estiloTC1);
                        sl.SetCellStyle("Q9", estiloTC1);
                        sl.MergeWorksheetCells("P9", "Q9");

                        sl.SetCellValue("P10", "Cantidad Lbs");
                        sl.SetCellStyle("P10", estiloTC1);
                        sl.SetCellValue("Q10", "Costo");
                        sl.SetCellStyle("Q10", estiloTC1);
                        sl.AutoFitColumn("P", "Q");
                    }
                    break;

                case 8:
                    {
                        sl.SetCellValue("R9", "Total");
                        sl.SetCellStyle("R9", estiloTC1);
                        sl.SetCellStyle("S9", estiloTC1);
                        sl.MergeWorksheetCells("R9", "S9");

                        sl.SetCellValue("R10", "Cantidad Lbs");
                        sl.SetCellStyle("R10", estiloTC1);
                        sl.SetCellValue("S10", "Costo");
                        sl.SetCellStyle("S10", estiloTC1);
                        sl.AutoFitColumn("R", "S");
                    }
                    break;

                case 9:
                    {
                        sl.SetCellValue("T9", "Total");
                        sl.SetCellStyle("T9", estiloTC1);
                        sl.SetCellStyle("U9", estiloTC1);
                        sl.MergeWorksheetCells("T9", "U9");

                        sl.SetCellValue("T10", "Cantidad Lbs");
                        sl.SetCellStyle("T10", estiloTC1);
                        sl.SetCellValue("U10", "Costo");
                        sl.SetCellStyle("U10", estiloTC1);
                        sl.AutoFitColumn("T", "U");
                    }
                    break;

                case 10:
                    {
                        sl.SetCellValue("V9", "Total");
                        sl.SetCellStyle("V9", estiloTC1);
                        sl.SetCellStyle("W9", estiloTC1);
                        sl.MergeWorksheetCells("V9", "W9");

                        sl.SetCellValue("V10", "Cantidad Lbs");
                        sl.SetCellStyle("V10", estiloTC1);
                        sl.SetCellValue("W10", "Costo");
                        sl.SetCellStyle("W10", estiloTC1);
                        sl.AutoFitColumn("V", "W");
                    }
                    break;

                case 11:
                    {
                        sl.SetCellValue("X9", "Total");
                        sl.SetCellStyle("X9", estiloTC1);
                        sl.SetCellStyle("Y9", estiloTC1);
                        sl.MergeWorksheetCells("X9", "Y9");

                        sl.SetCellValue("X10", "Cantidad Lbs");
                        sl.SetCellStyle("X10", estiloTC1);
                        sl.SetCellValue("Y10", "Costo");
                        sl.SetCellStyle("Y10", estiloTC1);
                        sl.AutoFitColumn("X", "Y");
                    }
                    break;

                case 12:
                    {
                        sl.SetCellValue("Z9", "Total");
                        sl.SetCellStyle("Z9", estiloTC1);
                        sl.SetCellStyle("AA9", estiloTC1);
                        sl.MergeWorksheetCells("Z9", "AA9");

                        sl.SetCellValue("Z10", "Cantidad Lbs");
                        sl.SetCellStyle("Z10", estiloTC1);
                        sl.SetCellValue("AA10", "Costo");
                        sl.SetCellStyle("AA10", estiloTC1);
                        sl.AutoFitColumn("Z", "AA");
                    }
                    break;

                case 13:
                    {
                        sl.SetCellValue("AB9", "Total");
                        sl.SetCellStyle("AB9", estiloTC1);
                        sl.SetCellStyle("AC9", estiloTC1);
                        sl.MergeWorksheetCells("AB9", "AC9");

                        sl.SetCellValue("AB10", "Cantidad Lbs");
                        sl.SetCellStyle("AB10", estiloTC1);
                        sl.SetCellValue("AC10", "Costo");
                        sl.SetCellStyle("AC10", estiloTC1);
                        sl.AutoFitColumn("AB", "AC");
                    }
                    break;

                default:
                    break;
            }

            var aTotalMesesBodegas = aResultInternalProcessSaldo
                                .GroupBy(gb => new { gb.IdMes, gb.Bodega, gb.id_warehouse })
                                .Select(s => new
                                {
                                    s.Key.IdMes,
                                    s.Key.id_warehouse,
                                    s.Key.Bodega,
                                    CantLbs = s.Sum(ss => ss.CantidadLbs)
                                }).OrderBy(ob => ob.Bodega).ToList();

            var aBodegas = aTotalMesesBodegas
                                .Select(s => new
                                {
                                    s.id_warehouse,
                                    s.Bodega,
                                }).Distinct().OrderBy(ob => ob.Bodega).ToList();

            SLStyle estiloTC2 = sl.CreateStyle();
            estiloTC2.Font.FontName = "Arial";
            estiloTC2.Font.FontSize = 12;
            estiloTC2.Font.Bold = true;
            estiloTC2.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left;
            estiloTC2.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Bottom;

            SLStyle estiloTC3 = sl.CreateStyle();
            estiloTC3.Font.FontName = "Arial";
            estiloTC3.Font.FontSize = 12;
            estiloTC3.Font.Bold = true;
            estiloTC3.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right;
            estiloTC3.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Bottom;

            SLStyle estiloTC4 = sl.CreateStyle();
            estiloTC4.Font.FontName = "Arial";
            estiloTC4.Font.FontSize = 9;
            estiloTC4.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left;
            estiloTC4.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Bottom;
            estiloTC4.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloTC4.Border.LeftBorder.Color = System.Drawing.Color.Black;
            estiloTC4.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloTC4.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloTC4.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;

            SLStyle estiloTC5 = sl.CreateStyle();
            estiloTC5.Font.FontName = "Arial";
            estiloTC5.Font.FontSize = 9;
            estiloTC5.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right;
            estiloTC5.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Bottom;
            estiloTC5.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloTC5.Border.LeftBorder.Color = System.Drawing.Color.Black;
            estiloTC5.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloTC5.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloTC5.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;

            int posInitTabla = 10;
            decimal cantTotalAux = 0.00M;
            decimal costoTotalAux = 0.00M;
            foreach (var detail in aBodegas)
            {
                posInitTabla++;
                sl.SetCellValue("A" + posInitTabla, detail.Bodega);
                sl.SetCellStyle("A" + posInitTabla, estiloTC2);
                sl.MergeWorksheetCells("A" + posInitTabla, "C" + posInitTabla);
                count = 0;
                countShow = 0;
                cantTotalAux = 0.00M;
                costoTotalAux = 0.00M;
                foreach (var item in aCabeceraMeses)
                {
                    var aATotalMesesBodegas = aTotalMesesBodegas.FirstOrDefault(fod => fod.IdMes == item.IdMes && fod.id_warehouse == detail.id_warehouse);
                    var cantAux = (aATotalMesesBodegas != null ? aATotalMesesBodegas.CantLbs.ToString("#,##0.00") : 0.00M.ToString("#,##0.00"));
                    cantTotalAux += (aATotalMesesBodegas != null ? aATotalMesesBodegas.CantLbs : 0.00M);
                    var costoAux = 0.00M.ToString("#,##0.00");
                    count++;
                    switch (count)
                    {
                        case 1:
                            {
                                countShow++;

                                sl.SetCellValue("D" + posInitTabla, cantAux);
                                sl.SetCellStyle("D" + posInitTabla, estiloTC3);
                                sl.SetCellValue("E" + posInitTabla, costoAux);
                                sl.SetCellStyle("E" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 2:
                            {
                                countShow++;

                                sl.SetCellValue("F" + posInitTabla, cantAux);
                                sl.SetCellStyle("F" + posInitTabla, estiloTC3);
                                sl.SetCellValue("G" + posInitTabla, costoAux);
                                sl.SetCellStyle("G" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 3:
                            {
                                countShow++;

                                sl.SetCellValue("H" + posInitTabla, cantAux);
                                sl.SetCellStyle("H" + posInitTabla, estiloTC3);
                                sl.SetCellValue("I" + posInitTabla, costoAux);
                                sl.SetCellStyle("I" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 4:
                            {
                                countShow++;

                                sl.SetCellValue("J" + posInitTabla, cantAux);
                                sl.SetCellStyle("J" + posInitTabla, estiloTC3);
                                sl.SetCellValue("K" + posInitTabla, costoAux);
                                sl.SetCellStyle("K" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 5:
                            {
                                countShow++;

                                sl.SetCellValue("L" + posInitTabla, cantAux);
                                sl.SetCellStyle("L" + posInitTabla, estiloTC3);
                                sl.SetCellValue("M" + posInitTabla, costoAux);
                                sl.SetCellStyle("M" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 6:
                            {
                                countShow++;

                                sl.SetCellValue("N" + posInitTabla, cantAux);
                                sl.SetCellStyle("N" + posInitTabla, estiloTC3);
                                sl.SetCellValue("O" + posInitTabla, costoAux);
                                sl.SetCellStyle("O" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 7:
                            {
                                countShow++;

                                sl.SetCellValue("P" + posInitTabla, cantAux);
                                sl.SetCellStyle("P" + posInitTabla, estiloTC3);
                                sl.SetCellValue("Q" + posInitTabla, costoAux);
                                sl.SetCellStyle("Q" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 8:
                            {
                                countShow++;

                                sl.SetCellValue("R" + posInitTabla, cantAux);
                                sl.SetCellStyle("R" + posInitTabla, estiloTC3);
                                sl.SetCellValue("S" + posInitTabla, costoAux);
                                sl.SetCellStyle("S" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 9:
                            {
                                countShow++;

                                sl.SetCellValue("T" + posInitTabla, cantAux);
                                sl.SetCellStyle("T" + posInitTabla, estiloTC3);
                                sl.SetCellValue("U" + posInitTabla, costoAux);
                                sl.SetCellStyle("U" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 10:
                            {
                                countShow++;

                                sl.SetCellValue("V" + posInitTabla, cantAux);
                                sl.SetCellStyle("V" + posInitTabla, estiloTC3);
                                sl.SetCellValue("W" + posInitTabla, costoAux);
                                sl.SetCellStyle("W" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 11:
                            {
                                countShow++;

                                sl.SetCellValue("X" + posInitTabla, cantAux);
                                sl.SetCellStyle("X" + posInitTabla, estiloTC3);
                                sl.SetCellValue("Y" + posInitTabla, costoAux);
                                sl.SetCellStyle("Y" + posInitTabla, estiloTC3);
                            }
                            break;

                        case 12:
                            {
                                countShow++;

                                sl.SetCellValue("Z" + posInitTabla, cantAux);
                                sl.SetCellStyle("Z" + posInitTabla, estiloTC3);
                                sl.SetCellValue("AA" + posInitTabla, costoAux);
                                sl.SetCellStyle("AA" + posInitTabla, estiloTC3);
                            }
                            break;

                        default:
                            break;
                    }
                }

                countShow++;
                switch (countShow)
                {
                    case 2:
                        {
                            sl.SetCellValue("F" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("F" + posInitTabla, estiloTC3);
                            sl.SetCellValue("G" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("G" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 3:
                        {
                            sl.SetCellValue("H" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("H" + posInitTabla, estiloTC3);
                            sl.SetCellValue("I" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("I" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 4:
                        {
                            sl.SetCellValue("J" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("J" + posInitTabla, estiloTC3);
                            sl.SetCellValue("K" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("K" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 5:
                        {
                            sl.SetCellValue("L" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("L" + posInitTabla, estiloTC3);
                            sl.SetCellValue("M" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("M" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 6:
                        {
                            sl.SetCellValue("N" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("N" + posInitTabla, estiloTC3);
                            sl.SetCellValue("O" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("O" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 7:
                        {
                            sl.SetCellValue("P" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("P" + posInitTabla, estiloTC3);
                            sl.SetCellValue("Q" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("Q" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 8:
                        {
                            sl.SetCellValue("R" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("R" + posInitTabla, estiloTC3);
                            sl.SetCellValue("S" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("S" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 9:
                        {
                            sl.SetCellValue("T" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("T" + posInitTabla, estiloTC3);
                            sl.SetCellValue("U" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("U" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 10:
                        {
                            sl.SetCellValue("V" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("V" + posInitTabla, estiloTC3);
                            sl.SetCellValue("W" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("W" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 11:
                        {
                            sl.SetCellValue("X" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("X" + posInitTabla, estiloTC3);
                            sl.SetCellValue("Y" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("Y" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 12:
                        {
                            sl.SetCellValue("Z" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("Z" + posInitTabla, estiloTC3);
                            sl.SetCellValue("AA" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("AA" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 13:
                        {
                            sl.SetCellValue("AB" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("AB" + posInitTabla, estiloTC3);
                            sl.SetCellValue("AC" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                            sl.SetCellStyle("AC" + posInitTabla, estiloTC3);
                        }
                        break;

                    default:
                        break;
                }

                var aTipoMesesBodegas = aResultInternalProcessSaldo
                               .Where(w => w.id_warehouse == detail.id_warehouse).ToList();

                var aTipoBodegas = aTipoMesesBodegas
                                    .Select(s => new
                                    {
                                        s.Tipo
                                    }).Distinct().OrderBy(ob => ob.Tipo).ToList();
                foreach (var detailTipo in aTipoBodegas)
                {
                    posInitTabla++;
                    sl.SetCellValue("A" + posInitTabla, "  " + detailTipo.Tipo);
                    sl.SetCellStyle("A" + posInitTabla, estiloTC4);
                    sl.SetCellStyle("B" + posInitTabla, estiloTC4);
                    sl.SetCellStyle("C" + posInitTabla, estiloTC4);
                    sl.MergeWorksheetCells("A" + posInitTabla, "C" + posInitTabla);

                    count = 0;
                    countShow = 0;
                    cantTotalAux = 0.00M;
                    costoTotalAux = 0.00M;
                    foreach (var item in aCabeceraMeses)
                    {
                        var aATipoMesesBodegas = aTipoMesesBodegas.FirstOrDefault(fod => fod.IdMes == item.IdMes && fod.Tipo == detailTipo.Tipo);
                        var cantAux = (aATipoMesesBodegas != null ? aATipoMesesBodegas.CantidadLbs.ToString("#,##0.00") : "");
                        cantTotalAux += (aATipoMesesBodegas != null ? aATipoMesesBodegas.CantidadLbs : 0.00M);
                        var costoAux = "";
                        count++;
                        switch (count)
                        {
                            case 1:
                                {
                                    countShow++;

                                    sl.SetCellValue("D" + posInitTabla, cantAux);
                                    sl.SetCellStyle("D" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("E" + posInitTabla, costoAux);
                                    sl.SetCellStyle("E" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 2:
                                {
                                    countShow++;

                                    sl.SetCellValue("F" + posInitTabla, cantAux);
                                    sl.SetCellStyle("F" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("G" + posInitTabla, costoAux);
                                    sl.SetCellStyle("G" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 3:
                                {
                                    countShow++;

                                    sl.SetCellValue("H" + posInitTabla, cantAux);
                                    sl.SetCellStyle("H" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("I" + posInitTabla, costoAux);
                                    sl.SetCellStyle("I" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 4:
                                {
                                    countShow++;

                                    sl.SetCellValue("J" + posInitTabla, cantAux);
                                    sl.SetCellStyle("J" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("K" + posInitTabla, costoAux);
                                    sl.SetCellStyle("K" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 5:
                                {
                                    countShow++;

                                    sl.SetCellValue("L" + posInitTabla, cantAux);
                                    sl.SetCellStyle("L" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("M" + posInitTabla, costoAux);
                                    sl.SetCellStyle("M" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 6:
                                {
                                    countShow++;

                                    sl.SetCellValue("N" + posInitTabla, cantAux);
                                    sl.SetCellStyle("N" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("O" + posInitTabla, costoAux);
                                    sl.SetCellStyle("O" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 7:
                                {
                                    countShow++;

                                    sl.SetCellValue("P" + posInitTabla, cantAux);
                                    sl.SetCellStyle("P" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("Q" + posInitTabla, costoAux);
                                    sl.SetCellStyle("Q" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 8:
                                {
                                    countShow++;

                                    sl.SetCellValue("R" + posInitTabla, cantAux);
                                    sl.SetCellStyle("R" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("S" + posInitTabla, costoAux);
                                    sl.SetCellStyle("S" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 9:
                                {
                                    countShow++;

                                    sl.SetCellValue("T" + posInitTabla, cantAux);
                                    sl.SetCellStyle("T" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("U" + posInitTabla, costoAux);
                                    sl.SetCellStyle("U" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 10:
                                {
                                    countShow++;

                                    sl.SetCellValue("V" + posInitTabla, cantAux);
                                    sl.SetCellStyle("V" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("W" + posInitTabla, costoAux);
                                    sl.SetCellStyle("W" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 11:
                                {
                                    countShow++;

                                    sl.SetCellValue("X" + posInitTabla, cantAux);
                                    sl.SetCellStyle("X" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("Y" + posInitTabla, costoAux);
                                    sl.SetCellStyle("Y" + posInitTabla, estiloTC5);
                                }
                                break;

                            case 12:
                                {
                                    countShow++;

                                    sl.SetCellValue("Z" + posInitTabla, cantAux);
                                    sl.SetCellStyle("Z" + posInitTabla, estiloTC5);
                                    sl.SetCellValue("AA" + posInitTabla, costoAux);
                                    sl.SetCellStyle("AA" + posInitTabla, estiloTC5);
                                }
                                break;

                            default:
                                break;
                        }
                    }

                    countShow++;
                    switch (countShow)
                    {
                        case 2:
                            {
                                sl.SetCellValue("F" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("F" + posInitTabla, estiloTC5);
                                sl.SetCellValue("G" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("G" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 3:
                            {
                                sl.SetCellValue("H" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("H" + posInitTabla, estiloTC5);
                                sl.SetCellValue("I" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("I" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 4:
                            {
                                sl.SetCellValue("J" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("J" + posInitTabla, estiloTC5);
                                sl.SetCellValue("K" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("K" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 5:
                            {
                                sl.SetCellValue("L" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("L" + posInitTabla, estiloTC5);
                                sl.SetCellValue("M" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("M" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 6:
                            {
                                sl.SetCellValue("N" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("N" + posInitTabla, estiloTC5);
                                sl.SetCellValue("O" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("O" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 7:
                            {
                                sl.SetCellValue("P" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("P" + posInitTabla, estiloTC5);
                                sl.SetCellValue("Q" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("Q" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 8:
                            {
                                sl.SetCellValue("R" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("R" + posInitTabla, estiloTC5);
                                sl.SetCellValue("S" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("S" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 9:
                            {
                                sl.SetCellValue("T" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("T" + posInitTabla, estiloTC5);
                                sl.SetCellValue("U" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("U" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 10:
                            {
                                sl.SetCellValue("V" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("V" + posInitTabla, estiloTC5);
                                sl.SetCellValue("W" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("W" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 11:
                            {
                                sl.SetCellValue("X" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("X" + posInitTabla, estiloTC5);
                                sl.SetCellValue("Y" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("Y" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 12:
                            {
                                sl.SetCellValue("Z" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("Z" + posInitTabla, estiloTC5);
                                sl.SetCellValue("AA" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("AA" + posInitTabla, estiloTC5);
                            }
                            break;

                        case 13:
                            {
                                sl.SetCellValue("AB" + posInitTabla, (cantTotalAux == 0.00M ? "" : cantTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("AB" + posInitTabla, estiloTC5);
                                sl.SetCellValue("AC" + posInitTabla, (costoTotalAux == 0.00M ? "" : costoTotalAux.ToString("#,##0.00")));
                                sl.SetCellStyle("AC" + posInitTabla, estiloTC5);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            posInitTabla++;
            sl.SetCellValue("A" + posInitTabla, "Total General");
            sl.SetCellStyle("A" + posInitTabla, estiloTC2);
            sl.MergeWorksheetCells("A" + posInitTabla, "C" + posInitTabla);

            count = 0;
            countShow = 0;
            cantTotalAux = 0.00M;
            decimal cantTotalParcialAux = 0.00M;
            costoTotalAux = 0.00M;
            foreach (var item in aCabeceraMeses)
            {
                var aATotalMesesBodegas = aTotalMesesBodegas.Where(fod => fod.IdMes == item.IdMes).ToList();
                if (aATotalMesesBodegas != null && aATotalMesesBodegas.Count() > 0)
                {
                    cantTotalParcialAux = aATotalMesesBodegas.Sum(s => s.CantLbs);
                }
                else
                {
                    cantTotalParcialAux = 0.00M;
                }
                var cantAux = cantTotalParcialAux.ToString("#,##0.00");
                cantTotalAux += cantTotalParcialAux;
                var costoAux = 0.00M.ToString("#,##0.00");
                count++;
                switch (count)
                {
                    case 1:
                        {
                            countShow++;

                            sl.SetCellValue("D" + posInitTabla, cantAux);
                            sl.SetCellStyle("D" + posInitTabla, estiloTC3);
                            sl.SetCellValue("E" + posInitTabla, costoAux);
                            sl.SetCellStyle("E" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 2:
                        {
                            countShow++;

                            sl.SetCellValue("F" + posInitTabla, cantAux);
                            sl.SetCellStyle("F" + posInitTabla, estiloTC3);
                            sl.SetCellValue("G" + posInitTabla, costoAux);
                            sl.SetCellStyle("G" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 3:
                        {
                            countShow++;

                            sl.SetCellValue("H" + posInitTabla, cantAux);
                            sl.SetCellStyle("H" + posInitTabla, estiloTC3);
                            sl.SetCellValue("I" + posInitTabla, costoAux);
                            sl.SetCellStyle("I" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 4:
                        {
                            countShow++;

                            sl.SetCellValue("J" + posInitTabla, cantAux);
                            sl.SetCellStyle("J" + posInitTabla, estiloTC3);
                            sl.SetCellValue("K" + posInitTabla, costoAux);
                            sl.SetCellStyle("K" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 5:
                        {
                            countShow++;

                            sl.SetCellValue("L" + posInitTabla, cantAux);
                            sl.SetCellStyle("L" + posInitTabla, estiloTC3);
                            sl.SetCellValue("M" + posInitTabla, costoAux);
                            sl.SetCellStyle("M" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 6:
                        {
                            countShow++;

                            sl.SetCellValue("N" + posInitTabla, cantAux);
                            sl.SetCellStyle("N" + posInitTabla, estiloTC3);
                            sl.SetCellValue("O" + posInitTabla, costoAux);
                            sl.SetCellStyle("O" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 7:
                        {
                            countShow++;

                            sl.SetCellValue("P" + posInitTabla, cantAux);
                            sl.SetCellStyle("P" + posInitTabla, estiloTC3);
                            sl.SetCellValue("Q" + posInitTabla, costoAux);
                            sl.SetCellStyle("Q" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 8:
                        {
                            countShow++;

                            sl.SetCellValue("R" + posInitTabla, cantAux);
                            sl.SetCellStyle("R" + posInitTabla, estiloTC3);
                            sl.SetCellValue("S" + posInitTabla, costoAux);
                            sl.SetCellStyle("S" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 9:
                        {
                            countShow++;

                            sl.SetCellValue("T" + posInitTabla, cantAux);
                            sl.SetCellStyle("T" + posInitTabla, estiloTC3);
                            sl.SetCellValue("U" + posInitTabla, costoAux);
                            sl.SetCellStyle("U" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 10:
                        {
                            countShow++;

                            sl.SetCellValue("V" + posInitTabla, cantAux);
                            sl.SetCellStyle("V" + posInitTabla, estiloTC3);
                            sl.SetCellValue("W" + posInitTabla, costoAux);
                            sl.SetCellStyle("W" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 11:
                        {
                            countShow++;

                            sl.SetCellValue("X" + posInitTabla, cantAux);
                            sl.SetCellStyle("X" + posInitTabla, estiloTC3);
                            sl.SetCellValue("Y" + posInitTabla, costoAux);
                            sl.SetCellStyle("Y" + posInitTabla, estiloTC3);
                        }
                        break;

                    case 12:
                        {
                            countShow++;

                            sl.SetCellValue("Z" + posInitTabla, cantAux);
                            sl.SetCellStyle("Z" + posInitTabla, estiloTC3);
                            sl.SetCellValue("AA" + posInitTabla, costoAux);
                            sl.SetCellStyle("AA" + posInitTabla, estiloTC3);
                        }
                        break;

                    default:
                        break;
                }
            }

            countShow++;
            switch (countShow)
            {
                case 2:
                    {
                        sl.SetCellValue("F" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("F" + posInitTabla, estiloTC3);
                        sl.SetCellValue("G" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("G" + posInitTabla, estiloTC3);
                    }
                    break;

                case 3:
                    {
                        sl.SetCellValue("H" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("H" + posInitTabla, estiloTC3);
                        sl.SetCellValue("I" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("I" + posInitTabla, estiloTC3);
                    }
                    break;

                case 4:
                    {
                        sl.SetCellValue("J" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("J" + posInitTabla, estiloTC3);
                        sl.SetCellValue("K" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("K" + posInitTabla, estiloTC3);
                    }
                    break;

                case 5:
                    {
                        sl.SetCellValue("L" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("L" + posInitTabla, estiloTC3);
                        sl.SetCellValue("M" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("M" + posInitTabla, estiloTC3);
                    }
                    break;

                case 6:
                    {
                        sl.SetCellValue("N" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("N" + posInitTabla, estiloTC3);
                        sl.SetCellValue("O" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("O" + posInitTabla, estiloTC3);
                    }
                    break;

                case 7:
                    {
                        sl.SetCellValue("P" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("P" + posInitTabla, estiloTC3);
                        sl.SetCellValue("Q" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("Q" + posInitTabla, estiloTC3);
                    }
                    break;

                case 8:
                    {
                        sl.SetCellValue("R" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("R" + posInitTabla, estiloTC3);
                        sl.SetCellValue("S" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("S" + posInitTabla, estiloTC3);
                    }
                    break;

                case 9:
                    {
                        sl.SetCellValue("T" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("T" + posInitTabla, estiloTC3);
                        sl.SetCellValue("U" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("U" + posInitTabla, estiloTC3);
                    }
                    break;

                case 10:
                    {
                        sl.SetCellValue("V" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("V" + posInitTabla, estiloTC3);
                        sl.SetCellValue("W" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("W" + posInitTabla, estiloTC3);
                    }
                    break;

                case 11:
                    {
                        sl.SetCellValue("X" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("X" + posInitTabla, estiloTC3);
                        sl.SetCellValue("Y" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("Y" + posInitTabla, estiloTC3);
                    }
                    break;

                case 12:
                    {
                        sl.SetCellValue("Z" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("Z" + posInitTabla, estiloTC3);
                        sl.SetCellValue("AA" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("AA" + posInitTabla, estiloTC3);
                    }
                    break;

                case 13:
                    {
                        sl.SetCellValue("AB" + posInitTabla, cantTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("AB" + posInitTabla, estiloTC3);
                        sl.SetCellValue("AC" + posInitTabla, costoTotalAux.ToString("#,##0.00"));
                        sl.SetCellStyle("AC" + posInitTabla, estiloTC3);
                    }
                    break;

                default:
                    break;
            }

            return sl;
        }

        [HttpGet]
        public ActionResult WriteDataToExcel()
        {
            try
            {
                SLDocument sl = getData();
                string fileName = "Matriz_Procesos_Internos" + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm") + ".xlsx";
                using (MemoryStream stream = new MemoryStream())
                {
                    sl.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return File("ERROR, no se pudo generar el Archivo Excel: " + ex.ToString(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Error.xlsx");
            }
        }

        public class CustomProducionLot : ProductionLot
        {
            public string CustomId { get; set; }
        }

        #region Liquidacion No Valorizada
        [HttpPost]
        public ActionResult GetMachinesForProdOpeningLiqNoVal(
           int? id_MachineForProd, int? id_MachineProdOpening, string documentStateCode, DateTime? emissionDate, int? id_PersonProcessPlant, int id_Turn)
        {
            var model = new ComboBoxMachinesProdOpeningModel()
            {
                id_MachineForProd = id_MachineForProd,
                id_MachineProdOpening = id_MachineProdOpening,
                documentStateCode = documentStateCode,
                emissionDate = emissionDate,
                id_PersonProcessPlant = id_PersonProcessPlant,
                id_Turn = id_Turn,
            };

            return PartialView("ProducionComboBox/_ComboBoxMachinesProdOpeningLiqNoVal", model);
        }

        #endregion

        #region << Build ComboBoxes Edit >>
        [HttpPost]
        private void BuildViewDataEdit()
        {
            BuildComboBoxItemType();
            BuildComboBoxTrademark();
            BuildComboBoxPresentation();
            BuildComboBoxSize();
            BuildComboBoxItemTrademarkModel();
            BuildComboBoxItemGroupCategory();

        }

        private void BuildComboBoxItemType()
        {
            ViewData["ItemType"] = db.ItemType.Where(t => (t.InventoryLine.code == "PP" || t.InventoryLine.code == "PT") &&
                                                          t.id_company == ActiveCompany.id && t.isActive).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.InventoryLine.name + " - " + s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemType()
        {
            BuildComboBoxItemType();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxItemType");
        }

        private void BuildComboBoxTrademark()
        {
            ViewData["Trademark"] = (DataProviderItemTrademark.ItemTrademarks(ActiveCompany.id) as List<ItemTrademark>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxTrademark()
        {
            BuildComboBoxTrademark();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxTrademark");
        }

        private void BuildComboBoxPresentation()
        {
            ViewData["Presentation"] = (db.Presentation.Where(w => w.isActive && w.Item.FirstOrDefault(fod => fod.isActive && (fod.InventoryLine.code == "PP" || fod.InventoryLine.code == "PT")) != null))
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxPresentation()
        {
            BuildComboBoxPresentation();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxPresentation");
        }

        private void BuildComboBoxSize()
        {
            ViewData["Size"] = (DataProviderItemSize.ItemSizes() as List<ItemSize>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxSize()
        {
            BuildComboBoxSize();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxSize");
        }

        private void BuildComboBoxItemTrademarkModel()
        {
            ViewData["ItemTrademarkModel"] = (DataProviderItemTrademarkModel.ItemTrademarkModels() as List<ItemTrademarkModel>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemTrademarkModel()
        {
            BuildComboBoxItemTrademarkModel();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxItemTrademarkModel");
        }

        private void BuildComboBoxItemGroupCategory()
        {
            ViewData["ItemGroupCategory"] = (DataProviderItemGroupCategory.ItemGroupCategories(this.ActiveCompanyId) as List<ItemGroupCategory>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemGroupCategory()
        {
            BuildComboBoxItemGroupCategory();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxItemGroupCategory");
        }

        #endregion
    }
}