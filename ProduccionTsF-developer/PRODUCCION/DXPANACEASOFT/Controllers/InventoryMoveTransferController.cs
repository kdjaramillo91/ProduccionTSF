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
using System.Collections;
using static DXPANACEASOFT.Services.ServiceInventoryMove;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryMoveTransferController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView("Index", new InventoryMove());
        }

        [HttpPost]
        public ActionResult IndexEntryMove()
        {
            var model = new DXPANACEASOFT.Models.InventoryMove
            {
                Document = new Document
                {
                    DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("03"))
                }
            };
            var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            var entryNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("I"));
            ViewData["_natureMove"] = "I";

            model.idNatureMove = entryNatureMove.idAdvanceDetailModelP;

            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult IndexEntryMovePurchaseOrder()
        {
            var model = new DXPANACEASOFT.Models.InventoryMove
            {
                Document = new Document
                {
                    DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("04"))//Ingreso x Orden de Compra
                }
            };
            var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            var entryNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("I"));

            model.idNatureMove = entryNatureMove.idAdvanceDetailModelP;
            ViewData["_natureMove"] = "I";

            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult IndexExitMove()
        {
            var model = new DXPANACEASOFT.Models.InventoryMove
            {
                Document = new Document
                {
                    DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("05"))
                }
            };
            var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            var entryNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("E"));

            model.idNatureMove = entryNatureMove.idAdvanceDetailModelP;
            ViewData["_natureMove"] = "E";

            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult IndexTransferExitMove()
        {
            var model = new DXPANACEASOFT.Models.InventoryMove
            {
                Document = new Document
                {
                    DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("32"))
                }
            };
            var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            var entryNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("E"));

            model.idNatureMove = entryNatureMove.idAdvanceDetailModelP;
            ViewData["_natureMove"] = "E";

            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult IndexTransferEntryMove()
        {
            var model = new DXPANACEASOFT.Models.InventoryMove
            {
                Document = new Document
                {
                    DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("34"))
                }
            };
            var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            var entryNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("I"));

            model.idNatureMove = entryNatureMove.idAdvanceDetailModelP;
            ViewData["_natureMove"] = "I";

            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult IndexTransferMove()
        {
            var model = new DXPANACEASOFT.Models.InventoryMove
            {
                Document = new Document
                {
                    DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("06"))
                }
            };

            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult IndexKardex()
        {
            //ViewData["documentTypeCodeCurrent"] = "KARDEX";
            TempData["documentTypeCodeCurrent"] = "KARDEX";
            TempData.Keep("documentTypeCodeCurrent");

            return Index();
        }

        #region INVENTORY MOVE EDIT FORM

        [HttpPost]
        public ActionResult InventoryMoveEditFormPartial(int id, string code, string natureMoveType, int [] ordersDetails, int[] inventoryMoveDetailTransferExitsDetails)
        {
            var inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id);
            var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;

            string codeNatureMove = (natureMoveType != null && natureMoveType != "") ? natureMoveType.Trim() : db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == inventoryMove.idNatureMove)?.valueCode;
            codeNatureMove = codeNatureMove.Trim();
            ViewData["_natureMove"] = codeNatureMove.Trim();
            var structNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals(codeNatureMove));

            RefresshDataForEditForm();
            if (inventoryMove == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(code));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

                inventoryMove = new InventoryMove
                {
                    Document = new Document
                    {
                        id_documentType = documentType?.id ?? 0,
                        DocumentType = documentType,
                        id_documentState = documentState?.id ?? 0,
                        DocumentState = documentState,
                        emissionDate = DateTime.Now
                    },
                    InventoryMoveDetail = new List<InventoryMoveDetail>()
                  //  id_inventoryReason = db.InventoryReason.FirstOrDefault(fod => fod.DocumentType.code == code)?.id
                };

                inventoryMove.idNatureMove = structNatureMove.idAdvanceDetailModelP;
                inventoryMove.AdvanceParametersDetail = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == structNatureMove.idAdvanceDetailModelP);

                #region OrdersDetails

                if (ordersDetails != null)
                {
                    List<InventoryMoveDetail> inventoryMoveDetails = new List<InventoryMoveDetail>();

                    foreach (var od in ordersDetails)
                    {
                        PurchaseOrderDetail tempPurchaseOrderDetail = db.PurchaseOrderDetail.FirstOrDefault(d => d.id == od);

                        //if (remissionGuide.route == "" || remissionGuide.route == null) remissionGuide.route = tempPurchaseOrderDetail.PurchaseOrder.Provider.Person.address;

                        //var quantityDispatchPendingAux = tempPurchaseOrderDetail.quantityApproved - tempPurchaseOrderDetail.quantityDispatched;
                        var amountMoveAux = tempPurchaseOrderDetail.quantityApproved - tempPurchaseOrderDetail.quantityReceived;
                        var unitPriceMoveAux = (tempPurchaseOrderDetail.total / tempPurchaseOrderDetail.quantityApproved);
                        var itemAux = db.Item.FirstOrDefault(i => i.id == tempPurchaseOrderDetail.id_item);
                        InventoryMoveDetail inventoryMoveDetail = new InventoryMoveDetail
                        {
                            id = inventoryMove.InventoryMoveDetail.Count() > 0 ? inventoryMove.InventoryMoveDetail.Max(pld => pld.id) + 1 : 1,
                            id_item = tempPurchaseOrderDetail.id_item,
                            Item = itemAux,
                            //quantityOrdered = quantityDispatchPendingAux,
                            //quantityDispatchPending = quantityDispatchPendingAux,
                            //quantityProgrammed = quantityDispatchPendingAux,
                            //quantityReceived = 0,
                            id_warehouse = itemAux.ItemInventory.id_warehouse,
                            Warehouse = db.Warehouse.FirstOrDefault(w => w.id == itemAux.ItemInventory.id_warehouse),
                            id_warehouseLocation = itemAux.ItemInventory.id_warehouseLocation,
                            WarehouseLocation = db.WarehouseLocation.FirstOrDefault(w => w.id == itemAux.ItemInventory.id_warehouseLocation),
                            amountMove = amountMoveAux,
                            unitPriceMove = unitPriceMoveAux,
                            id_metricUnitMove = tempPurchaseOrderDetail.Item.ItemPurchaseInformation.id_metricUnitPurchase,
                            MetricUnit1 = tempPurchaseOrderDetail.Item.ItemPurchaseInformation.MetricUnit,
                            balanceCost = (amountMoveAux * unitPriceMoveAux),
                            //isActive = true,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            InventoryMoveDetailPurchaseOrder = new List<InventoryMoveDetailPurchaseOrder>(),
                        };

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMoveDetail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null);
                                if (entityValuePermissions == null)
                                {
                                    inventoryMoveDetail.id_warehouse = 0;
                                    inventoryMoveDetail.Warehouse = null;
                                    inventoryMoveDetail.id_warehouseLocation = 0;
                                    inventoryMoveDetail.WarehouseLocation = null;
                                }
                            }
                        }

                        inventoryMoveDetail.InventoryMoveDetailPurchaseOrder.Add(new InventoryMoveDetailPurchaseOrder
                        {
                            id_purchaseOrderDetail = tempPurchaseOrderDetail.id,
                            PurchaseOrderDetail = tempPurchaseOrderDetail,
                            id_purchaseOrder = tempPurchaseOrderDetail.id_purchaseOrder,
                            PurchaseOrder = tempPurchaseOrderDetail.PurchaseOrder,
                            quantity = inventoryMoveDetail.amountMove.Value
                        });

                        inventoryMove.InventoryMoveDetail.Add(inventoryMoveDetail);

                    }
                }

                #endregion

                #region InventoryMoveDetailTransferExitsDetails

                if (inventoryMoveDetailTransferExitsDetails != null)
                {
                    List<InventoryMoveDetail> inventoryMoveDetails2 = new List<InventoryMoveDetail>();

                    foreach (var od in inventoryMoveDetailTransferExitsDetails)
                    {
                        InventoryMoveDetail tempInventoryMoveDetail = db.InventoryMoveDetail.FirstOrDefault(d => d.id == od);

                        //tempPurchaseOrderDetail.quantityReceived
                        decimal receivedAmount = (tempInventoryMoveDetail != null && tempInventoryMoveDetail.InventoryMoveDetailTransfer != null &&
                                tempInventoryMoveDetail.InventoryMoveDetailTransfer.Where(w => w.InventoryMoveDetail1.InventoryMove.Document.DocumentState.code.Equals("03")).Count() > 0)
                                ? tempInventoryMoveDetail.InventoryMoveDetailTransfer.Where(w => w.InventoryMoveDetail1.InventoryMove.Document.DocumentState.code.Equals("03")).Sum(s => s.quantity)
                                : 0;
                        decimal amountMove = tempInventoryMoveDetail != null && tempInventoryMoveDetail.amountMove != null ? tempInventoryMoveDetail.amountMove.Value : 0;

                        var amountMoveAux = amountMove - receivedAmount;
                        var unitPriceMoveAux = tempInventoryMoveDetail.unitPriceMove;
                        var itemAux = db.Item.FirstOrDefault(i => i.id == tempInventoryMoveDetail.id_item);
                        InventoryMoveDetail inventoryMoveDetail = new InventoryMoveDetail
                        {
                            id = inventoryMove.InventoryMoveDetail.Count() > 0 ? inventoryMove.InventoryMoveDetail.Max(pld => pld.id) + 1 : 1,
                            id_item = tempInventoryMoveDetail.id_item,
                            Item = itemAux,
                            id_warehouse = tempInventoryMoveDetail.id_warehouseEntry.Value,
                            Warehouse = db.Warehouse.FirstOrDefault(w => w.id == tempInventoryMoveDetail.id_warehouseEntry),
                            id_warehouseLocation = null,
                            WarehouseLocation = null,
                            amountMove = amountMoveAux,
                            unitPriceMove = unitPriceMoveAux,
                            id_metricUnitMove = tempInventoryMoveDetail.id_metricUnitMove,
                            MetricUnit1 = db.MetricUnit.FirstOrDefault(w => w.id == tempInventoryMoveDetail.id_metricUnitMove),
                            id_lot = tempInventoryMoveDetail.id_lot,
                            Lot = tempInventoryMoveDetail.Lot,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            InventoryMoveDetailTransfer1 = new List<InventoryMoveDetailTransfer>(),
                            id_costCenter = tempInventoryMoveDetail.id_costCenter,
                            id_subCostCenter = tempInventoryMoveDetail.id_subCostCenter

                        };

                        var inventoryMoveDetailTransfer = (new InventoryMoveDetailTransfer
                        {
                            id_inventoryMoveDetailExit = tempInventoryMoveDetail.id,
                            InventoryMoveDetail = tempInventoryMoveDetail,
                            id_inventoryMoveExit = tempInventoryMoveDetail.id_inventoryMove,
                            InventoryMove = tempInventoryMoveDetail.InventoryMove,
                            id_warehouseExit = tempInventoryMoveDetail.id_warehouse,
                            Warehouse = tempInventoryMoveDetail.Warehouse,
                            id_warehouseLocationExit = tempInventoryMoveDetail.id_warehouseLocation.Value,
                            WarehouseLocation = tempInventoryMoveDetail.WarehouseLocation,
                            id_inventoryMoveDetailEntry = inventoryMoveDetail.id,
                            InventoryMoveDetail1 = inventoryMoveDetail,
                            quantity = amountMoveAux
                        });
                        inventoryMoveDetail.InventoryMoveDetailTransfer1.Add(inventoryMoveDetailTransfer);
                        tempInventoryMoveDetail.InventoryMoveDetailTransfer = new List<InventoryMoveDetailTransfer>();
                        tempInventoryMoveDetail.InventoryMoveDetailTransfer.Add(inventoryMoveDetailTransfer);
                        inventoryMove.InventoryMoveDetail.Add(inventoryMoveDetail);

                    }
                }
                #endregion
            }
            
            

            var settingORLS = db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
            ViewBag.withLotSystem = settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == code)?.valueAux == "1" : false;
            var settingORLC = db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
            ViewBag.withLotCustomer = settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == code)?.valueAux == "1" : false;

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveEditFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult InventoryMoveCopy(int id)
        {
            var inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            InventoryMove copyInventoryMove = null;
            if(inventoryMove.id != 0)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(inventoryMove.Document.DocumentType.code));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.id == 1);

                copyInventoryMove = new InventoryMove
                {
                    Document = new Document
                    {
                        id_documentType = documentType?.id ?? 0,
                        DocumentType = documentType,
                        id_documentState = documentState?.id ?? 0,
                        DocumentState = documentState,
                        emissionDate = DateTime.Now
                    }
                };

                if (inventoryMove.Document.DocumentType.code.Equals("03") || inventoryMove.Document.DocumentType.code.Equals("04"))
                {
                    copyInventoryMove.InventoryEntryMove = new InventoryEntryMove
                    {
                        id_warehouseEntry = inventoryMove.InventoryEntryMove.id_warehouseEntry,
                        id_warehouseLocationEntry = inventoryMove.InventoryEntryMove.id_warehouseLocationEntry,
                        id_receiver = inventoryMove.InventoryEntryMove.id_receiver,
                        dateEntry = DateTime.Now
                    };
                }
                else if (inventoryMove.Document.DocumentType.code.Equals("05"))
                {
                    inventoryMove.InventoryExitMove = new InventoryExitMove
                    {
                        id_warehouseExit = inventoryMove.InventoryExitMove.id_warehouseExit,
                        id_warehouseLocationExit = inventoryMove.InventoryExitMove.id_warehouseLocationExit,
                        id_dispatcher = inventoryMove.InventoryExitMove.id_dispatcher,
                        dateExit = DateTime.Now
                    };
                }
                else if (inventoryMove.Document.DocumentType.code.Equals("06"))
                {
                    copyInventoryMove.InventoryExitMove = new InventoryExitMove
                    {
                        id_warehouseExit = inventoryMove.InventoryExitMove.id_warehouseExit,
                        id_warehouseLocationExit = inventoryMove.InventoryExitMove.id_warehouseLocationExit,
                        id_dispatcher = inventoryMove.InventoryExitMove.id_dispatcher,
                        dateExit = DateTime.Now
                    };

                    copyInventoryMove.InventoryEntryMove = new InventoryEntryMove
                    {
                        id_warehouseEntry = inventoryMove.InventoryEntryMove.id_warehouseEntry,
                        id_warehouseLocationEntry = inventoryMove.InventoryEntryMove.id_warehouseLocationEntry,
                        id_receiver = inventoryMove.InventoryEntryMove.id_receiver,
                        dateEntry = DateTime.Now

                    };

                    copyInventoryMove.InventoryTransferMove = new InventoryTransferMove
                    {
                        InventoryExitMove = copyInventoryMove.InventoryExitMove,
                        InventoryEntryMove = copyInventoryMove.InventoryEntryMove
                    };
                }

                foreach (var detail in inventoryMove.InventoryMoveDetail)
                {
                    var tempDetail = new InventoryMoveDetail
                    {
                        id_item = detail.id_item,
                        entryAmount = detail.entryAmount,
                        exitAmount = detail.exitAmount,
                        id_warehouse = detail.id_warehouse,
                        id_warehouseLocation = detail.id_warehouseLocation,
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now,
                    };
                    
                    copyInventoryMove.InventoryMoveDetail.Add(tempDetail);
                }
            }

            TempData["inventoryMove"] = copyInventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveEditFormPartial", copyInventoryMove);
        }

        #endregion

        #region INVENTORY MOVE FILTER RESULTS

        [HttpPost]
        public ActionResult InventoryResults(InventoryMove inventoryMove,
                                             InventoryEntryMove entryMove,
                                             InventoryExitMove exitMove,
                                             Document document,
                                             DateTime? startEmissionDate, DateTime? endEmissionDate,
                                             DateTime? startAuthorizationDate, DateTime? endAuthorizationDate)
        {
            var model = db.InventoryMove.ToList();




            #region DOCUMENT FILTERS

            if (document.id_documentType != 0)
            {
                model = model.Where(o => o.Document.DocumentType.id == document.id_documentType).ToList();
            }

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

            if (inventoryMove.id_inventoryReason != 0 && inventoryMove.id_inventoryReason != null)
            {
                model = model.Where(o => o.id_inventoryReason == inventoryMove.id_inventoryReason).ToList();
            }

            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var tempModel = new List<InventoryMove>();
                    foreach (var item in model)
                    {
                        var inventoryMoveDetail = item.InventoryMoveDetail.FirstOrDefault(fod => entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == fod.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) == null);
                        if (inventoryMoveDetail == null)
                        {
                            tempModel.Add(item);
                        }
                    }

                    model = tempModel;

                }
            }

            TempData["model"] = model;
            TempData.Keep("model");

            ViewData["code"] = db.DocumentType.FirstOrDefault(fod=> fod.id == document.id_documentType)?.code ?? "";

            return PartialView("_InventoryResults", model.OrderByDescending(i => i.id).ToList());
        }

        #endregion

        #region INVENTORY MOVE HEADER

        [ValidateInput(false)]
        public ActionResult InventoryMovesPartial(string code)
        {
            var model = TempData["model"] as List<InventoryMove>;
            model = model ?? new List<InventoryMove>();

            TempData.Keep("model");

            ViewData["code"] = code;

            return PartialView("_InventoryMovesPartial", model.OrderByDescending(i => i.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMovesPartialAddNew(bool approve, string codeDocumentType
                                                        , string natureMoveIMTmp
                                                        , DXPANACEASOFT.Models.InventoryMove item, Document document,
                                                        InventoryEntryMove entryMove, InventoryExitMove exitMove)
        {
            RefresshDataForEditForm();
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            int idInventoryReason = 0;
            int idDocumentType = 0;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    //Assign Document
                    idInventoryReason = item?.id_inventoryReason ?? 0;
                    idDocumentType = db.InventoryReason.FirstOrDefault(fod => fod.id == idInventoryReason)?.id_documentType ?? 0;
                    DocumentType _doIt = db.DocumentType.FirstOrDefault(fod => fod.id == idDocumentType);
                    document.DocumentType = _doIt;
                    document.id_documentType = idDocumentType;

                    var settingORLS = db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
                    ViewBag.withLotSystem = settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == _doIt?.code)?.valueAux == "1" : false;
                    var settingORLC = db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
                    ViewBag.withLotCustomer = settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == _doIt?.code)?.valueAux == "1" : false;

                    item.Document = document;
                    item.InventoryEntryMove = entryMove;
                    item.InventoryExitMove = exitMove;
                    item.InventoryMoveDetail = tempInventoryMove.InventoryMoveDetail;
                    tempInventoryMove.InventoryEntryMove = entryMove;
                    tempInventoryMove.InventoryExitMove = exitMove;

                    item.idNatureMove =  db.InventoryReason.FirstOrDefault(fod => fod.id == item.id_inventoryReason)?.idNatureMove;

                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            foreach (var detail in tempInventoryMove.InventoryMoveDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para editar y guardar el movimiento de inventario.");
                                }
                            }
                            if (approve)
                            {
                                foreach (var detail in tempInventoryMove.InventoryMoveDetail)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new Exception("No tiene Permiso para aprobar el movimiento de inventario.");
                                    }
                                }
                            }
                        }
                    }

                    if (item.InventoryMoveDetail.Count == 0)
                    {
                        throw new Exception("No se puede guardar un movimiento de inventario sin detalles.");
                    }
                    if (ViewBag.withLotSystem)
                    {
                        var inventoryMoveDetailAux = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.Lot?.number == "" || fod.Lot?.number == null);
                        if (inventoryMoveDetailAux != null)
                        {
                            throw new Exception("No se puede guardar el movimiento de inventario sin lote de Sistema, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                        }
                    }
                    if (ViewBag.withLotCustomer)
                    {
                        var inventoryMoveDetailAux = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.Lot?.internalNumber == "" || fod.Lot?.internalNumber == null);
                        if (inventoryMoveDetailAux != null)
                        {
                            throw new Exception("No se puede guardar el movimiento de inventario sin lote de Cliente, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                        }
                    }
                    var inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_costCenter == null);
                    if (inventoryMoveDetailAux2 != null)
                    {
                        throw new Exception("No se puede guardar el movimiento de inventario sin centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                    }

                    inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_subCostCenter == null);
                    if (inventoryMoveDetailAux2 != null)
                    {
                        throw new Exception("No se puede guardar el movimiento de inventario sin sub-centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                    }
                    ServiceInventoryMoveAux result = null;
                    if (natureMoveIMTmp.Trim().Equals("I"))
                    {
                        result = UpdateInventaryMoveTransferEntry(approve, ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                    }
                    else if(natureMoveIMTmp.Trim().Equals("E"))
                    {
                        result = UpdateInventaryMoveTransferExit(approve, ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                    }

                    item = result?.inventoryMove;
                    

                    if (item.id == 0)
                    {
                        db.InventoryMove.Add(item);
                    }
                    else
                    {
                        db.InventoryMove.Attach(item);
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    trans.Commit();

                    ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + item.Document.number + " guardado exitosamente");

                    TempData["inventoryMove"] = item;
                    TempData.Keep("inventoryMove");
                    ViewData["_natureMove"] = natureMoveIMTmp.Trim();


                }
                catch (Exception e)
                {
                    TempData.Keep("inventoryMove");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    item = tempInventoryMove;
                    trans.Rollback();
                }
            }
            
            
            return PartialView("_InventoryMoveMainFormPartial", item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMovesPartialUpdate(bool approve, string codeDocumentType
                                                        , string natureMoveIMTmp
                                                        , DXPANACEASOFT.Models.InventoryMove item, Document document, 
                                                        InventoryEntryMove entryMove, InventoryExitMove exitMove)
        {
            RefresshDataForEditForm();
            return InventoryMovesPartialAddNew(approve, codeDocumentType, natureMoveIMTmp, item, document, entryMove, exitMove);
            //var model = db.InventoryMove;

            //var modelItem = new InventoryMove();
            ////if (ModelState.IsValid)
            ////{
            //    using (DbContextTransaction trans = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            modelItem = model.FirstOrDefault(it => it.id == id_inventoryMove);
            //            if (modelItem != null)
            //            {
            //                #region DOCUMENT

            //                modelItem.Document.description = document.description;
            //                modelItem.Document.id_userUpdate = ActiveUser.id;
            //                modelItem.Document.dateUpdate = DateTime.Now;

            //                #endregion

            //                #region INVENTORYMOVE

            //                if (modelItem.Document.DocumentType.code.Equals("03") || modelItem.Document.DocumentType.code.Equals("04"))
            //                {
            //                    modelItem.InventoryEntryMove.id_warehouseEntry = entryMove.id_warehouseEntry;
            //                    modelItem.InventoryEntryMove.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
            //                    modelItem.InventoryEntryMove.id_receiver = entryMove.id_receiver;
            //                    modelItem.InventoryEntryMove.dateEntry = DateTime.Now;
            //                }
            //                else if (modelItem.Document.DocumentType.code.Equals("05"))
            //                {
            //                    modelItem.InventoryExitMove.id_warehouseExit = exitMove.id_warehouseExit;
            //                    modelItem.InventoryExitMove.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
            //                    modelItem.InventoryExitMove.id_dispatcher = exitMove.id_dispatcher;
            //                    modelItem.InventoryExitMove.dateExit = DateTime.Now;
            //                }
            //                else if (modelItem.Document.DocumentType.code.Equals("06"))
            //                {
            //                    exitMove.dateExit = DateTime.Now;
            //                    entryMove.dateEntry = DateTime.Now;

            //                    modelItem.InventoryExitMove.id_warehouseExit = exitMove.id_warehouseExit;
            //                    modelItem.InventoryExitMove.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
            //                    modelItem.InventoryExitMove.id_dispatcher = exitMove.id_dispatcher;
            //                    modelItem.InventoryExitMove.dateExit = DateTime.Now;

            //                    modelItem.InventoryEntryMove.id_warehouseEntry = entryMove.id_warehouseEntry;
            //                    modelItem.InventoryEntryMove.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
            //                    modelItem.InventoryEntryMove.id_receiver = entryMove.id_receiver;
            //                    modelItem.InventoryEntryMove.dateEntry = DateTime.Now;

            //                    //item.InventoryTransferMove = new InventoryTransferMove
            //                    //{
            //                    //    InventoryExitMove = item.InventoryExitMove,
            //                    //    InventoryEntryMove = item.InventoryEntryMove
            //                    //};
            //                }

            //                #endregion

            //                #region DETAILS

            //                var inventoryMoveDetails = db.InventoryMoveDetail.Where(i => i.id_inventoryMove == id_inventoryMove);

            //                foreach (var detail in inventoryMoveDetails)
            //                {
            //                    db.InventoryMoveDetail.Remove(detail);
            //                    db.Entry(detail).State = EntityState.Deleted;
            //                }

            //                InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);

            //                if (tempInventoryMove?.InventoryMoveDetail != null)
            //                {
            //                    var itemDetail = tempInventoryMove.InventoryMoveDetail.ToList();

            //                    foreach (var i in itemDetail)
            //                    {
            //                        //List<InventoryMoveDetail> lastsMoveDetails =
            //                        //                        db.InventoryMoveDetail.Where(d =>
            //                        //                                                     d.id_productionLote == i.id_productionLote &&
            //                        //                                                     d.id_item == i.id_item &&
            //                        //                                                     d.id_warehouse == i.id_warehouse &&
            //                        //                                                     d.id_warehouseLocation == i.id_warehouseLocation).ToList();

            //                        //lastsMoveDetails = lastsMoveDetails.OrderByDescending(d => d.dateUpdate).ToList();

            //                        //InventoryMoveDetail lastInventoryMove = (lastsMoveDetails.Count > 0)
            //                        //                                        ? inventoryMoveDetails.First()
            //                        //                                        : null;

            //                        var tempDetail = new InventoryMoveDetail
            //                        {
            //                            id_item = i.id_item,
            //                            entryAmount = i.entryAmount,
            //                            exitAmount = i.exitAmount,
            //                            id_warehouse = i.id_warehouse,
            //                            id_warehouseLocation = i.id_warehouseLocation,
            //                            InventoryMoveDetailPurchaseOrder = i.InventoryMoveDetailPurchaseOrder,
            //                            id_userCreate = ActiveUser.id,
            //                            dateCreate = DateTime.Now,
            //                            id_userUpdate = ActiveUser.id,
            //                            dateUpdate = DateTime.Now,
            //                            //balance = (lastInventoryMove?.balance ?? 0) + (i.entryAmount - i.exitAmount)
            //                        };

            //                        modelItem.InventoryMoveDetail.Add(tempDetail);
            //                    }
            //                }

            //                #endregion

            //                if (modelItem.InventoryMoveDetail.Count == 0)
            //                {
            //                    TempData.Keep("inventoryMove");
            //                    ViewData["EditMessage"] = ErrorMessage("No se puede guardar un movimiento de inventario sin detalles");
            //                    return PartialView("_InventoryMoveMainFormPartial", tempInventoryMove);
            //                }

            //                db.InventoryMove.Attach(modelItem);
            //                db.Entry(modelItem).State = EntityState.Modified;

            //                db.SaveChanges();
            //                trans.Commit();

            //                TempData["inventoryMove"] = modelItem;
            //            }

            //        }
            //        catch (Exception)
            //        {
            //            TempData.Keep("inventoryMove");
            //            ViewData["EditError"] = ErrorMessage();
            //            trans.Rollback();
            //            //ViewData["EditError"] = e.Message;
            //            //trans.Rollback();
            //        }
            //    }
            ////}
            ////else
            ////    ViewData["EditError"] = "Please, correct all errors.";

            //TempData.Keep("inventoryMove");

            //return PartialView("_InventoryMoveEditFormPartial", modelItem);
        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMovesPartialDelete(System.Int32 id)
        {
            var model = db.InventoryMove;
            if (id >= 0){
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)
                    {
                        //var inventoryMoveDetails = db.InventoryMoveDetail.Where(i => i.id_inventoryMove == id_inventoryMove);

                        foreach (var detail in item.InventoryMoveDetail)
                        {
                            db.InventoryMoveDetail.Remove(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }
                        
                        model.Remove(item);
                    }
                        
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_InventoryMovesPartial", model.ToList());
        }

        #endregion

        #region DETAILS

        [ValidateInput(false)]
        public ActionResult InventoryMoveDetails(int id_inventoryMove)
        {
            RefresshDataForEditForm();
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id_inventoryMove);
            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            ViewData["id_inventoryMove"] = id_inventoryMove;
            ViewData["code"] = inventoryMove.Document.DocumentType.code;

            return PartialView("_InventoryMoveDetailsPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartial(string code)
        {
            RefresshDataForEditForm();
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            TempData["inventoryMove"] = TempData["inventoryMove"] ?? inventoryMove;
            TempData.Keep("inventoryMove");

            ViewData["id_inventoryMove"] = inventoryMove?.id ?? 0;
            ViewData["code"] = code;//inventoryMove?.Document?.DocumentType?.code ?? "";
            //ViewBag.codeDocumentTypeAux = code;
            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartialAddNew(string code, string lotNumber, string lotInternalNumber, DXPANACEASOFT.Models.InventoryMoveDetail item, int id_item2)
        {
            RefresshDataForEditForm();
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Params["errorMessage"] != "") throw new Exception("Por favor, corrija todos los errores.");

                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
                    var showCost = true;
                    if (entityObjectPermissions != null)
                    {
                        var objectPermissions = entityObjectPermissions.listObjectPermissions.FirstOrDefault(fod => fod.codeObject == "COS");
                        showCost = objectPermissions == null;
                    }
                    if (!showCost)
                    {
                        item.unitPriceMove = decimal.Parse(Request.Params["cpEditingRowUnitPriceMove"]);
                        item.balanceCost = decimal.Parse(Request.Params["cpEditingRowBalanceCost"]);
                    }

                    item.id = inventoryMove.InventoryMoveDetail.Count() > 0 ? inventoryMove.InventoryMoveDetail.Max(pld => pld.id) + 1 : 1;
                    item.id_userCreate = ActiveUser.id;
                    item.dateCreate = DateTime.Now;
                    item.id_userUpdate = ActiveUser.id;
                    item.dateUpdate = DateTime.Now;
                    item.id_item = id_item2;
                    item.Item = db.Item.FirstOrDefault(fod => fod.id == id_item2);
                    item.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouse);
                    item.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_warehouseLocation);
                    item.Warehouse1 = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouseEntry);

                    if (code != null && (code.Equals("03") || code.Equals("04") /*|| code.Equals("34")*/))
                    {
                        item.Lot = getLot(lotNumber, lotInternalNumber);
                        item.id_lot = item.Lot?.id;
                    }
                    else
                    {
                        item.Lot = db.Lot.FirstOrDefault(fod => fod.id == item.id_lot);
                    }

                    inventoryMove.InventoryMoveDetail.Add(item);
                    TempData["inventoryMove"] = inventoryMove;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    ViewData["code"] = code;
                    ViewData["id_inventoryMove"] = inventoryMove.id;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("inventoryMove");

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            ViewData["code"] = code;
            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartialUpdate(string code, string lotNumber, string lotInternalNumber, DXPANACEASOFT.Models.InventoryMoveDetail item, int id_item2)
        {
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            RefresshDataForEditForm();
            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Params["errorMessage"] != "") throw new Exception("Por favor, corrija todos los errores.");

                    var modelItem = inventoryMove.InventoryMoveDetail.FirstOrDefault(it => it.id == item.id);
                    //var modelItem = inventoryMove.InventoryMoveDetail.FirstOrDefault(it => it.id_inventoryMove == inventoryMove.id && it.id_item == item.id_item);
                    if (modelItem != null)
                    {

                        modelItem.entryAmount = item.entryAmount;
                        modelItem.exitAmount = item.exitAmount;
                        modelItem.amountMove = item.amountMove;

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
                        var showCost = true;
                        if (entityObjectPermissions != null)
                        {
                            var objectPermissions = entityObjectPermissions.listObjectPermissions.FirstOrDefault(fod => fod.codeObject == "COS");
                            showCost = objectPermissions == null;
                        }
                        if (showCost)
                        {
                            modelItem.unitPriceMove = item.unitPriceMove;
                            modelItem.balanceCost = item.balanceCost;
                        }
                        else
                        {
                            modelItem.unitPriceMove = decimal.Parse(Request.Params["cpEditingRowUnitPriceMove"]);
                            modelItem.balanceCost = decimal.Parse(Request.Params["cpEditingRowBalanceCost"]);
                        }

                        modelItem.id_metricUnitMove = item.id_metricUnitMove;
                        modelItem.MetricUnit1 = db.MetricUnit.FirstOrDefault(fod => fod.id == item.id_metricUnitMove);

                        modelItem.id_item = id_item2;
                        modelItem.id_warehouse = item.id_warehouse;
                        modelItem.id_warehouseLocation = item.id_warehouseLocation;
                        modelItem.id_warehouseEntry = item.id_warehouseEntry;

                        modelItem.Item = db.Item.FirstOrDefault(fod => fod.id == id_item2);
                        modelItem.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouse);
                        modelItem.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_warehouseLocation);
                        modelItem.Warehouse1 = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouseEntry);

                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        if (code != null && (code.Equals("03") || code.Equals("04") /*|| code.Equals("34")*/))
                        {
                            var lotAux = getLot(lotNumber, lotInternalNumber);
                            modelItem.id_lot = lotAux?.id;//modelItem.Lot?.id;
                            modelItem.Lot = db.Lot.FirstOrDefault(fod => fod.id == modelItem.id_lot);
                        }
                        else
                        if (!code.Equals("34"))
                        {
                            modelItem.id_lot = item.id_lot;
                            modelItem.Lot = db.Lot.FirstOrDefault(fod => fod.id == item.id_lot);
                        }

                        this.UpdateModel(modelItem);
                        TempData["inventoryMove"] = inventoryMove;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    ViewData["code"] = code;
                    ViewData["id_inventoryMove"] = inventoryMove.id;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("inventoryMove");

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            ViewData["id_inventoryMove"] = inventoryMove.id;
            ViewData["code"] = code;
            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartialDelete(string code, System.Int32 id)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            //if (id_item >= 0)
            //{
            try
            {
                var item = inventoryMove.InventoryMoveDetail.FirstOrDefault(p => p.id == id);
                //var item = inventoryMove.InventoryMoveDetail.FirstOrDefault(it => it.id_item == id_item);
                if (item != null)
                    inventoryMove.InventoryMoveDetail.Remove(item);
                    
                TempData["inventoryMove"] = inventoryMove;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
                ViewData["code"] = code;
            }
            //}

            TempData.Keep("inventoryMove");

            ViewData["code"] = code;
            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();
            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        #endregion
        
        #region PURCHASE ORDERS

        [HttpPost]
        public ActionResult PurchaseOrdersResult()
        {   
            return PartialView("_PurchaseOrdersResult");
        }

        [ValidateInput(false)]
        public ActionResult PurchaseOrdersDetailsPartial()
        {
            var model = db.PurchaseOrderDetail.Where(d => d.PurchaseOrder.Document.DocumentState.code == "06" && //"06" AUTORIZADA
                                                          d.PurchaseOrder.PurchaseReason.code.Equals("MI") && 
                                                          (d.quantityApproved - d.quantityReceived) > 0 && 
                                                          d.Item.InventoryLine.code != "MP" &&//Materia Prima
                                                          d.Item.InventoryLine.code != "PT" &&//Producto Terminado
                                                          d.Item.InventoryLine.code != "PP")//Producto en Proceso
                          .OrderByDescending(d => d.id_purchaseOrder).ToList().OrderByDescending(d => d.id_item);
            //return PartialView("_PurchaseOrderDetailsResultsPartial", model.ToList());

            //var model = db.PurchaseOrderDetail.Where(o => (o.PurchaseOrder.Document.id_documentState == 6 || o.PurchaseOrder.Document.id_documentState == 3)
            //                                           && (o.quantityApproved - o.quantityReceived > 0)).OrderByDescending(o => o.id);
            return PartialView("_PurchaseOrdersDetailsPartial", model.ToList());
        }

        #endregion

        #region INVENTORY MOVE DETAIL TRANSFER EXITS

        [HttpPost]
        public ActionResult InventoryMoveDetailTransferExitsResult()
        {
            return PartialView("_InventoryMoveDetailTransferExitsResult");
        }

        [ValidateInput(false)]
        public ActionResult InventoryMoveDetailTransferExitsPartial()
        {
            var model = db.InventoryMoveDetail.Where(d => d.InventoryMove.Document.DocumentType.code.Equals("32") && //"32" Egreso Por Transferencia
                                                          d.InventoryMove.Document.DocumentState.code.Equals("03"))
                                                          .AsEnumerable().
                                                          Where(d => ((d.InventoryMoveDetailTransfer != null && 
                                                           d.InventoryMoveDetailTransfer.Where(w=> w.InventoryMoveDetail1.InventoryMove.Document.DocumentState.code.Equals("03")).Count() > 0 &&
                                                           d.InventoryMoveDetailTransfer.Where(w => w.InventoryMoveDetail1.InventoryMove.Document.DocumentState.code.Equals("03")).Sum(s=> s.quantity) < d.amountMove) ||
                                                           d.InventoryMoveDetailTransfer == null || d.InventoryMoveDetailTransfer.Where(w => w.InventoryMoveDetail1.InventoryMove.Document.DocumentState.code.Equals("03")).Count() <= 0))
                                                           .OrderByDescending(d => d.id_inventoryMove).ThenByDescending(d => d.id_item).ToList();

            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var tempModel = new List<InventoryMoveDetail>();
                    foreach (var item in model)
                    {
                        var inventoryMoveDetail = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null);
                        if (inventoryMoveDetail != null)
                        {
                            tempModel.Add(item);
                        }
                    }

                    model = tempModel;

                    var tempModel2 = new List<InventoryMoveDetail>();
                    foreach (var item in model)
                    {
                        var inventoryMoveDetail = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouseEntry && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null);
                        if (inventoryMoveDetail != null)
                        {
                            tempModel2.Add(item);
                        }
                    }

                    model = tempModel2;

                }
            }

            return PartialView("_InventoryMoveDetailTransferExitsPartial", model.ToList());
        }

        #endregion

        #region SINGLE CHANGE DOCUMENT STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                    if (inventoryMove != null && documentState != null)
                    {
                        inventoryMove.Document.id_documentState = documentState.id;
                        inventoryMove.Document.DocumentState = documentState;

                        ServiceInventoryMove.Commit(inventoryMove, db);
                        
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

                    if (inventoryMove != null && documentState != null)
                    {
                        DocumentState currentState = inventoryMove.Document.DocumentState;

                        inventoryMove.Document.id_documentState = documentState.id;
                        inventoryMove.Document.DocumentState = documentState;

                        if(currentState.id != 3)
                        {
                            ServiceInventoryMove.Commit(inventoryMove, db);
                        }
                        else
                        {
                            db.InventoryMove.Attach(inventoryMove);
                            db.Entry(inventoryMove).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult Protect(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                    if (inventoryMove != null && documentState != null)
                    {
                        inventoryMove.Document.id_documentState = documentState.id;
                        inventoryMove.Document.DocumentState = documentState;

                        db.InventoryMove.Attach(inventoryMove);
                        db.Entry(inventoryMove).State = EntityState.Modified;

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

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    //if(inventoryMove.Document.DocumentType.Equals("04"))//Ingreso x Orden de Compra
                    //{
                    //    //var existInInventoryMoveDetailExitDispatchMaterials = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.InventoryMoveDetailPurchaseOrder.Count() > 0);//Tiene Ingresos de Inventario de MDD

                    //    //if (existInInventoryMoveDetailExitDispatchMaterials != null)
                    //    //{
                    //    //    var existInInventoryMoveEMD = existInInventoryMoveDetailExitDispatchMaterials.InventoryMoveDetailExitDispatchMaterials.FirstOrDefault().InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EMD");
                    //    //    if (existInInventoryMoveEMD)
                    //    //    {
                    //    //        TempData.Keep("remissionGuide");
                    //    //        ViewData["EditMessage"] = ErrorMessage("No se puede anular la guía de remisión debido a tener egresos de materiales de despacho en inventario, manual.");
                    //    //        return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                    //    //    }

                    //    //}
                    //}
                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            foreach (var item in inventoryMove.InventoryMoveDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Anular.");
                                }
                            }

                        }
                    }

                    //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (inventoryMove != null && documentState != null)
                    {
                        if (inventoryMove.Document.DocumentState.code != "01")// Pendiente
                        {
                            //foreach (var detail in remissionGuide.RemissionGuideDetail)
                            //{
                            //    foreach (var remissionGuideDetailPurchaseOrderDetail in detail.RemissionGuideDetailPurchaseOrderDetail)
                            //    {
                            //        ServicePurchaseRemission.UpdateQuantityDispatchedPurchaseOrderDetailRemissionGuide(db, remissionGuideDetailPurchaseOrderDetail.id_purchaseOrderDetail,
                            //                                                       remissionGuideDetailPurchaseOrderDetail.id_remissionGuideDetail,
                            //                                                       -remissionGuideDetailPurchaseOrderDetail.quantity);
                            //    }
                            //}

                            //var inventoryMoveIOC = inventoryMove.InventoryMoveDetail.FirstOrDefault()?.InventoryMoveDetailPurchaseOrder?.FirstOrDefault(fod => fod.InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("IOC"))?.InventoryMoveDetail.InventoryMove;//IOC: Ingreso x Orden de Compra
                            //Where(w => w.InventoryMoveDetailExitDispatchMaterials..InventoryReason.code.Equals("ILP")).OrderByDescending(d => d.InventoryMove.Document.emissionDate).ToList();
                            //InventoryMove lastInventoryMoveILP = (inventoryMoveDetailAux.Count > 0)
                            //                                                    ? inventoryMoveDetailAux.First().InventoryMove
                            //                                                    : null;
                            if (inventoryMove.Document.DocumentType.code.Equals("04"))//Ingreso x Orden de Compra
                            {
                                var inventoryMoveIOC = inventoryMove;
                                var result = UpdateInventaryMoveEntryPurchaseOrder(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveIOC);
                                inventoryMove = result.inventoryMove;
                            }

                            if (inventoryMove.Document.DocumentType.code.Equals("03"))//Ingreso
                            {
                                var inventoryMoveING = inventoryMove;
                                var result = UpdateInventaryMoveEntry(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveING);
                                inventoryMove = result.inventoryMove;
                            }

                            if (inventoryMove.Document.DocumentType.code.Equals("05"))//Egreso
                            {
                                var inventoryMoveEGR = inventoryMove;
                                var result = UpdateInventaryMoveExit(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveEGR);
                                inventoryMove = result.inventoryMove;
                            }

                            if (inventoryMove.Document.DocumentType.code.Equals("32"))//Egreso Por Transferencia
                            {
                                var inventoryMoveTranferExit = inventoryMove;
                                var result = UpdateInventaryMoveTransferExit(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveTranferExit);
                                inventoryMove = result.inventoryMove;
                            }

                            if (inventoryMove.Document.DocumentType.code.Equals("34"))//Ingreso Por Transferencia
                            {
                                var inventoryMoveTranferEntry = inventoryMove;
                                var result = UpdateInventaryMoveTransferEntry(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveTranferEntry);
                                inventoryMove = result.inventoryMove;
                            }

                        }

                        inventoryMove.Document.id_documentState = documentState.id;
                        inventoryMove.Document.DocumentState = documentState;

                        db.InventoryMove.Attach(inventoryMove);
                        db.Entry(inventoryMove).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["inventoryMove"] = inventoryMove;
                        TempData.Keep("inventoryMove");

                        ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + inventoryMove.Document.number + " anulado exitosamente");

                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("inventoryMove");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            var settingORLS = db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
            ViewBag.withLotSystem = settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false;
            var settingORLC = db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
            ViewBag.withLotCustomer = settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false;

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);

        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);
            string natureMove = "";
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            foreach (var item in inventoryMove.InventoryMoveDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Reversar.");
                                }
                            }
                        }
                    }

                    DocumentState documentStatePendiente = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

                    if (inventoryMove != null && documentStatePendiente != null)
                    {
                        natureMove = inventoryMove.AdvanceParametersDetail.valueCode.Trim();
                        ViewData["_natureMove"] = natureMove;
                        if (natureMove.Equals("I"))
                        {
                            var inventoryMoveTranferEntry = inventoryMove;
                            var result = UpdateInventaryMoveTransferEntry(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveTranferEntry);
                            inventoryMove = result.inventoryMove;
                        }
                        else if (natureMove.Equals("E"))
                        {
                            var inventoryMoveTranferExit = inventoryMove;
                            var result = UpdateInventaryMoveTransferExit(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveTranferExit);
                            inventoryMove = result.inventoryMove;
                        }


                        inventoryMove.Document.id_documentState = documentStatePendiente.id;
                        inventoryMove.Document.DocumentState = documentStatePendiente;

                        db.InventoryMove.Attach(inventoryMove);
                        db.Entry(inventoryMove).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["inventoryMove"] = inventoryMove;
                        TempData.Keep("inventoryMove");

                        ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + inventoryMove.Document.number + " reversado exitosamente");

                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("inventoryMove");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            var settingORLS = db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
            ViewBag.withLotSystem = settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false;
            var settingORLC = db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
            ViewBag.withLotCustomer = settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false;

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
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
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                            if (inventoryMove != null && documentState != null)
                            {
                                ServiceInventoryMove.Commit(inventoryMove, db);

                                inventoryMove.Document.id_documentState = documentState.id;
                                inventoryMove.Document.DocumentState = documentState;
                            }

                            db.InventoryMove.Attach(inventoryMove);
                            db.Entry(inventoryMove).State = EntityState.Modified;
                            
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

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

                            if (inventoryMove != null && documentState != null)
                            {
                                DocumentState currentState = inventoryMove.Document.DocumentState;

                                if (currentState.id != 3)
                                {
                                    ServiceInventoryMove.Commit(inventoryMove, db);
                                }

                                inventoryMove.Document.id_documentState = documentState.id;
                                inventoryMove.Document.DocumentState = documentState;
                                
                                db.InventoryMove.Attach(inventoryMove);
                                db.Entry(inventoryMove).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                            if (inventoryMove != null && documentState != null)
                            {
                                inventoryMove.Document.id_documentState = documentState.id;
                                inventoryMove.Document.DocumentState = documentState;

                                db.InventoryMove.Attach(inventoryMove);
                                db.Entry(inventoryMove).State = EntityState.Modified;
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

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

                            //if (purchaseOrder != null && documentState != null)
                            //{
                            //    purchaseOrder.Document.id_documentState = documentState.id;
                            //    purchaseOrder.Document.DocumentState = documentState;

                            //    db.PurchaseOrder.Attach(purchaseOrder);
                            //    db.Entry(purchaseOrder).State = EntityState.Modified;
                            //}
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

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

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
                            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);

                            //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                            //if (purchaseOrder != null && documentState != null)
                            //{
                            //    purchaseOrder.Document.id_documentState = documentState.id;
                            //    purchaseOrder.Document.DocumentState = documentState;

                            //    foreach (var details in purchaseOrder.PurchaseOrderDetail)
                            //    {
                            //        details.quantityApproved = 0.0M;

                            //        db.PurchaseOrderDetail.Attach(details);
                            //        db.Entry(details).State = EntityState.Modified;
                            //    }

                            //    db.PurchaseOrder.Attach(purchaseOrder);
                            //    db.Entry(purchaseOrder).State = EntityState.Modified;
                            //}
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

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_inventoryMove, string codeDocumentType)
        {
            TempData.Keep("inventoryMove");
            
            var inventoryMoveAux = db.InventoryMove.Where(w => w.Document.DocumentType.code.Equals(codeDocumentType)).ToList();
            int index = inventoryMoveAux.Count() > 0 ? inventoryMoveAux.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_inventoryMove) : 0;

            var result = new
            {
                maximunPages = db.InventoryMove.Where(w => w.Document.DocumentType.code.Equals(codeDocumentType)).Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page, string codeDocumentType)
        {
            InventoryMove inventoryMove = db.InventoryMove.Where(w => w.Document.DocumentType.Document.Equals(codeDocumentType)).OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (inventoryMove != null)
            {
                TempData["inventoryMove"] = inventoryMove;
                TempData.Keep("inventoryMove");
                return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
            }

            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", new InventoryMove());
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

            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);
            //int state = remissionGuide.Document.DocumentState.id;
            string state = inventoryMove.Document.DocumentState.code;

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
                    btnAutorize = false,
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
                    btnProtect = false,//purchaseOrderDetailAux != null,// true,
                    //btnProtect = true,
                    btnCancel = true,
                    btnRevert = true,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORTS

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveReportList()
        {
            return PartialView("_InventoryMoveListReport");
        }

        #endregion

        #region AUXILIAR FUNCTIONS
        public JsonResult InventoryReasonChanged(int idIR)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? new InventoryMove();
            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();


            var objIr = db.InventoryReason.FirstOrDefault(fod => fod.id == idIR);

            var result = new
            {
                codeIR = objIr?.DocumentType?.code ?? "",
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InventoryMoveItemDetails(int? id_itemCurrent, string codeDocumentType, int? id_metricUnitMove, int? id_warehouse, int? id_warehouseLocation, int? id_lot)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? new InventoryMove();
            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();

            var items = new List<Item>();

            if (codeDocumentType == "04")//Ingreso x Orden de Compra
            {
                items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.InventoryLine.code.Equals("MI")) || w.id == id_itemCurrent).ToList();
            }
            else
            {
                items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.inventoryControl) || w.id == id_itemCurrent).ToList();
            }

            var item = items.FirstOrDefault(i => i.id == id_itemCurrent);

            var id_metricTypeAux = item?.ItemInventory?.MetricUnit.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.id_metricType == id_metricTypeAux) || w.id == id_metricUnitMove).ToList();

            var warehouseLocations = db.WarehouseLocation.Where(w => (w.id_warehouse == id_warehouse && w.isActive) || w.id == id_warehouseLocation)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            var id_lotAux = id_lot == 0 ? null : id_lot;

            var lots = db.InventoryMoveDetail.Where(w => w.id_warehouse == id_warehouse &&
                                                          w.id_warehouseLocation == id_warehouseLocation &&
                                                          w.id_item == id_itemCurrent &&
                                                          w.id_inventoryMoveDetailNext == null &&
                                                          w.InventoryMove.Document.DocumentState.code.Equals("03") &&//"03": APROBADA
                                                          w.id_lot != null)
                                       .Select(s => new
                                       {
                                           id = s.id_lot,
                                           number = ((s.Lot.number ?? "") + ((s.Lot.number != "" && s.Lot.number != null && s.Lot.internalNumber != "" && s.Lot.internalNumber != "") ? "-" : "") + (s.Lot.internalNumber ?? ""))//s.Lot.number
                                       });

            var remainingBalance = db.InventoryMoveDetail.FirstOrDefault(w => w.id_warehouse == id_warehouse &&
                                                                         w.id_warehouseLocation == id_warehouseLocation &&
                                                                         w.id_item == id_itemCurrent &&
                                                                         w.id_inventoryMoveDetailNext == null &&
                                                                         w.InventoryMove.Document.DocumentState.code.Equals("03") &&//"03": APROBADA
                                                                         w.id_lot == id_lotAux)?.balance ?? 0;
            var result = new
            {
                masterCode = item?.masterCode ?? "",
                items = items.Select(s => new
                {
                    id = s.id,
                    masterCode = s.masterCode,
                    name = s.name
                }).ToList(),
                Message = "Ok",
                
                warehouseLocations = warehouseLocations,
                metricUnits = metricUnits.Select(s => new
                {
                    id = s.id,
                    code = s.code,
                    name = s.name
                }).ToList(),
                lots = lots,
                remainingBalance = remainingBalance
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemDetailsData(int? id_item)
        {

            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var id_metricTypeAux = item.ItemInventory?.MetricUnit.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.id_metricType == id_metricTypeAux)).ToList();

            var id_warehouseAux = item?.ItemInventory?.Warehouse.id;
            var id_warehouseLocationAux = item?.ItemInventory?.WarehouseLocation.id;

            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == id_warehouseAux && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null);
                    if (entityValuePermissions == null)
                    {
                        id_warehouseAux = null;
                        id_warehouseLocationAux = null;
                    }
                }
            }

            var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouseAux)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });
            var ListInventoryMoveDetailAux = db.InventoryMoveDetail.Where(w => w.unitPrice > 0 && w.id_item == id_item).ToList();
            ListInventoryMoveDetailAux = ListInventoryMoveDetailAux.OrderByDescending(o => o.InventoryMove.Document.emissionDate).ThenByDescending(t => t.dateCreate).ToList();
            var inventoryMoveDetailAux = ListInventoryMoveDetailAux.Count() > 0 ?  ListInventoryMoveDetailAux.First() : null;
            var unitPriceMove = inventoryMoveDetailAux?.unitPrice ?? (item.ItemPurchaseInformation?.purchasePrice ?? 0);

            var lots = db.InventoryMoveDetail.Where(w => w.id_warehouse == id_warehouseAux &&
                                                          w.id_warehouseLocation == id_warehouseLocationAux &&
                                                          w.id_item == id_item &&
                                                          w.id_inventoryMoveDetailNext == null &&
                                                          w.InventoryMove.Document.DocumentState.code.Equals("03") &&//"03": APROBADA
                                                          w.id_lot != null)
                                       .Select(s => new
                                       {
                                           id = s.id_lot,
                                           number = s.Lot.number
                                       });

            var inventoryMoveDetailBalanceAux = db.InventoryMoveDetail.FirstOrDefault(w => w.id_warehouse == id_warehouseAux &&
                                                                         w.id_warehouseLocation == id_warehouseLocationAux &&
                                                                         w.id_item == id_item &&
                                                                         w.id_inventoryMoveDetailNext == null &&
                                                                         w.InventoryMove.Document.DocumentState.code.Equals("03") &&//"03": APROBADA
                                                                         w.id_lot == null);
            var remainingBalance = inventoryMoveDetailBalanceAux?.balance ?? 0;
            var averagePrice = inventoryMoveDetailBalanceAux?.averagePrice ?? 0;

            var result = new
            {
                masterCode = item.masterCode,
                metricUnitInventoryPurchase = (item.ItemInventory?.MetricUnit?.code ?? ""),
                metricUnits = metricUnits.Select(s => new
                {
                    id = s.id,
                    code = s.code,
                    name = s.name
                }).ToList(),
                id_metricUnitMove = (item.ItemInventory?.id_metricUnitInventory),
                id_warehouse = id_warehouseAux,
                warehouseLocations = warehouseLocations,
                id_warehouseLocation = id_warehouseLocationAux,
                unitPriceMove = unitPriceMove,
                lots = lots,
                remainingBalance = remainingBalance,
                averagePrice = averagePrice
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
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
            var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse && w.isActive)
                                       .Select(s => new {
                                           id = s.id,
                                           name = s.name
                                       });
            var id_warehouseLocationAux = warehouseLocations?.FirstOrDefault().id;
            if (inventoryMove?.InventoryMoveDetail != null)
            {
                var itemDetail = inventoryMove.InventoryMoveDetail.ToList();

                foreach (var i in itemDetail)
                {
                    i.id_warehouse = id_warehouse;
                    if(id_warehouseLocationAux != null) i.id_warehouseLocation = id_warehouseLocationAux.Value;
                    UpdateModel(i);
                }
            }

            var result = new
            {
               id_warehouse,
               warehouse.name,
               warehouseLocations = warehouseLocations
                                    .Select(s => new {
                                        id = s.id,
                                        name = s.name
                                    }),
               id_warehouseLocation = id_warehouseLocationAux
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

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateWarehouseLocation(int? id_warehouse)
        {
            var result = new
            {
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse && w.isActive)
                                       .Select(s => new {
                                           id = s.id,
                                           name = s.name
                                       })

            };

            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateQuantityTotal(string unitPriceMove, int? id_metricUnitMove, int? id_metricUnitMoveCurrent, string amountMove)
        {

            //var item = db.Item.FirstOrDefault(fod => fod.id == id_item);

            decimal _unitPriceMove = Convert.ToDecimal(unitPriceMove);
            //var presentation = item?.Presentation;
            decimal _amountMove = Convert.ToDecimal(amountMove);

            var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitMoveCurrent &&
                                                                                     fod.id_metricDestiny == id_metricUnitMove);

            var factor = id_metricUnitMove == null || id_metricUnitMoveCurrent == null ? 0 : (id_metricUnitMove == id_metricUnitMoveCurrent ? (1) : metricUnitConversion?.factor ?? 0);
            var unitPriceMoveAux = _unitPriceMove * factor;
            var balanceCost = unitPriceMoveAux * _amountMove ;
            var metricOrigin = db.MetricUnit.FirstOrDefault(fod=> fod.id == id_metricUnitMove);
            var metricDestiny = db.MetricUnit.FirstOrDefault(fod=> fod.id == id_metricUnitMoveCurrent);
            //var UMCI = code == "04" ? "UM Compra" : "UM Inv.";//"04": Ingreso x Orden de Compra
            var Message = metricOrigin == null ? ("La UM Mov. es nula") : (metricDestiny == null ? ("La UM Mov. es nula") : (factor == 0 ? ("No existe el factor de conversión entre " + metricOrigin.code + " y " + metricDestiny.code + "Configúrelo e intente de nuevo") : ("OK")));
            var result = new
            {
                balanceCost = balanceCost,
                id_metricUnitMove = (Message == "OK") ? id_metricUnitMoveCurrent : id_metricUnitMove,
                Message = Message,
                unitPriceMove = unitPriceMoveAux
            };


            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private Lot getLot(string lot, string lotInternal)
        {
            //List<Lot> lots = (TempData["lot"] as List<Lot>);
            var strLotAux = lot?.Trim() ?? "";
            var strLotInternalAux = lotInternal?.Trim() ?? "";
            if (strLotAux == "" && strLotInternalAux == "")
            {
                return null;
            }
            var lotAux = db.Lot.FirstOrDefault(fod => fod.number == strLotAux && fod.internalNumber == strLotInternalAux);
            if (lotAux == null)
            {
                lotAux = CreateNewLot(strLotAux, strLotInternalAux);
            }
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            return lotAux;
        }

        private Lot CreateNewLot(string lot, string lotInternal)
        {
            RefresshDataForEditForm();
            Lot lotAux = null;
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region Lot

                    lotAux = ServiceLot.CreateLot(db, ActiveUser, ActiveCompany, ActiveEmissionPoint);
                    lotAux.number = lot;
                    lotAux.internalNumber = lotInternal;

                    //Actualizando Estado del Documento Lote Base
                    var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                    lotAux.Document.id_documentState = documentState.id;
                    lotAux.Document.DocumentState = documentState;

                    db.SaveChanges();
                    trans.Commit();

                    #endregion
                }
                catch (Exception e)
                {
                    TempData.Keep("inventoryMove");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    return null;
                }
            }

            TempData.Keep("inventoryMove");

            return lotAux;
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateLot(int? id_item, int? id_warehouse, int? id_warehouseLocation)
        {

            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            //if (item == null)
            //{
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}

            //var id_metricTypeAux = item.ItemInventory?.MetricUnit.id_metricType;
            //var metricUnits = db.MetricUnit.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.id_metricType == id_metricTypeAux)).ToList();

            //var id_warehouseAux = item?.ItemInventory?.Warehouse.id;
            //var id_warehouseLocationAux = item?.ItemInventory?.WarehouseLocation.id;
            //var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouseAux)
            //                           .Select(s => new
            //                           {
            //                               id = s.id,
            //                               name = s.name
            //                           });
            //var ListInventoryMoveDetailAux = db.InventoryMoveDetail.Where(w => w.unitPrice > 0 && w.id_item == id_item).ToList();
            //ListInventoryMoveDetailAux = ListInventoryMoveDetailAux.OrderByDescending(o => o.InventoryMove.Document.emissionDate).ThenByDescending(t => t.dateCreate).ToList();
            //var inventoryMoveDetailAux = ListInventoryMoveDetailAux.Count() > 0 ?  ListInventoryMoveDetailAux.First() : null;
            //var unitPriceMove = inventoryMoveDetailAux?.unitPrice ?? (item.ItemPurchaseInformation?.purchasePrice ?? 0);

            var lots = db.InventoryMoveDetail.Where(w => w.id_warehouse == id_warehouse &&
                                                          w.id_warehouseLocation == id_warehouseLocation &&
                                                          w.id_item == id_item &&
                                                          w.id_inventoryMoveDetailNext == null &&
                                                          w.InventoryMove.Document.DocumentState.code.Equals("03") &&//"03": APROBADA
                                                          w.id_lot != null)
                                       .Select(s => new
                                       {
                                           id = s.id_lot,
                                           number = s.Lot.number
                                       });

            var inventoryMoveDetailBalanceAux = db.InventoryMoveDetail.FirstOrDefault(w => w.id_warehouse == id_warehouse &&
                                                                         w.id_warehouseLocation == id_warehouseLocation &&
                                                                         w.id_item == id_item &&
                                                                         w.id_inventoryMoveDetailNext == null &&
                                                                         w.InventoryMove.Document.DocumentState.code.Equals("03") &&//"03": APROBADA
                                                                         w.id_lot == null);
            var remainingBalance = inventoryMoveDetailBalanceAux?.balance ?? 0;
            var averagePrice = inventoryMoveDetailBalanceAux?.averagePrice ?? 0;

            var result = new
            {
                //masterCode = item.masterCode,
                //metricUnitInventoryPurchase = (item.ItemInventory?.MetricUnit?.code ?? ""),
                //metricUnits = metricUnits.Select(s => new
                //{
                //    id = s.id,
                //    code = s.code,
                //    name = s.name
                //}).ToList(),
                id_metricUnitMove = (item.ItemInventory?.id_metricUnitInventory),
                //id_warehouse = id_warehouseAux,
                //warehouseLocations = warehouseLocations,
                //id_warehouseLocation = id_warehouseLocationAux,
                //unitPriceMove = unitPriceMove,
                lots = lots,
                remainingBalance = remainingBalance,
                averagePrice = averagePrice
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult LotDetail(int? id_item, int? id_warehouse, int? id_warehouseLocation, int? id_lot)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            RefresshDataForEditForm();
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            var inventoryMoveDetailBalanceAux = db.InventoryMoveDetail.FirstOrDefault(w => w.id_warehouse == id_warehouse &&
                                                                         w.id_warehouseLocation == id_warehouseLocation &&
                                                                         w.id_item == id_item &&
                                                                         w.id_inventoryMoveDetailNext == null &&
                                                                         w.InventoryMove.Document.DocumentState.code.Equals("03") &&//"03": APROBADA
                                                                         w.id_lot == id_lot);
            var remainingBalance = inventoryMoveDetailBalanceAux?.balance ?? 0;
            var averagePrice = inventoryMoveDetailBalanceAux?.averagePrice ?? 0;

            var result = new
            {
                id_metricUnitMove = (item.ItemInventory?.id_metricUnitInventory),
                remainingBalance = remainingBalance,
                averagePrice = averagePrice
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSubCostCenter(int? id_costCenterDetail, int? id_subCostCenterDetail)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            var items = db.CostCenter.Where(w => (w.id_higherCostCenter == id_costCenterDetail && w.isActive) ||
                                                (w.id == id_subCostCenterDetail)).ToList();
            TempData.Keep("inventoryMove");
            RefresshDataForEditForm();
            return GridViewExtension.GetComboBoxCallbackResult(p => {
                p.ClientInstanceName = "id_subCostCenterDetail";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "InventoryMove", Action = "GetSubCostCenter" };
                p.ClientSideEvents.BeginCallback = "InventoryMoveSubCostCenter_BeginCallback";
                p.ClientSideEvents.EndCallback = "InventoryMoveSubCostCenter_EndCallback";
                p.ClientSideEvents.Validation = "OnSubCostCenterDetailValidation";
                p.CallbackPageSize = 5;
                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);
                p.BindList(items);
            });
        }

        [HttpPost]
        public ActionResult GetItems(int? indice)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            RefresshDataForEditForm();
            TempData.Keep("inventoryMove");
            InventoryMoveDetail imd = inventoryMove?.InventoryMoveDetail.FirstOrDefault(fod => fod.id_item == indice);
            return this.PartialView("ComponentsDetail/_ComboBoxItems", imd ?? new InventoryMoveDetail());
        }
        #endregion

        #region New Version
        private void RefresshDataForEditForm()
        {
            ViewData["_ItemsDetailEditIM"] = (ViewData["code"] != null && ViewData["code"].Equals("04")) ? DataProviderItem.AllMIItemsByCompany((int?)ViewData["id_company"]) :
            (DataProviderItem.AllInventoryItemsByCompany((int?)ViewData["id_company"]));
            
        }
        #endregion
    }
}