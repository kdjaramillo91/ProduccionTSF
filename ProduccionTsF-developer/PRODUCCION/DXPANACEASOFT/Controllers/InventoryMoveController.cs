using Dapper;
using DevExpress.CodeParser;
using DevExpress.DataProcessing;
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Mvc;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;
using DXPANACEASOFT.Models.ComboBoxes;
using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Models.GenericProcess;
using DXPANACEASOFT.Models.InventoryBalance;
using DXPANACEASOFT.Models.InventoryMoveDTO;
using DXPANACEASOFT.Models.InventoryMoveP.InventoryMoveModel;
using DXPANACEASOFT.Models.ModelExtension;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utilitarios.Logs;
using Utilitarios.ProdException;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static DXPANACEASOFT.Services.ServiceInventoryBalance;
using static DXPANACEASOFT.Services.ServiceInventoryMove;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryMoveController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView("Index", new InventoryMove());
        }

        [HttpPost]
        public ActionResult IndexEntryMove(string natureMove)
        {
            var model = new InventoryMove
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
            var model = new InventoryMove
            {
                Document = new Document
                {
                    DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("04"))
                }
            };
            var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            var entryNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("I"));

            model.idNatureMove = entryNatureMove.idAdvanceDetailModelP;
            ViewData["_natureMove"] = "I";

            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult IndexExitMove(string natureMove)
        {
            var model = new InventoryMove
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
            var model = new InventoryMove
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
        public ActionResult IndexTransferExitAutomaticMove()
        {
            var model = new InventoryMove
            {
                Document = new Document
                {
                    DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("129"))
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
            var model = new InventoryMove
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
            var model = new InventoryMove
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
            TempData["documentTypeCodeCurrent"] = "KARDEX";
            TempData.Keep("documentTypeCodeCurrent");

            return Index();
        }

        #region INVENTORY MOVE EDIT FORM

        [HttpPost]
        public ActionResult InventoryMoveEditFormPartial(int id, string code,
            string natureMoveType, int[] ordersDetails, int[] inventoryMoveDetailTransferExitsDetails, string customParamOP = null)
        {
            // JINM
            string _customWarehouse = "";
            string _codeInventoryReason = "";
            var inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id);
            var idNatureMove = inventoryMove?.idNatureMove;
            var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            DateTime? emissionDateInicial = null;

            string codeNatureMove = (natureMoveType != null && natureMoveType != "")
                ? natureMoveType.Trim() : db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == idNatureMove)?.valueCode;

            codeNatureMove = codeNatureMove.Trim();
            ViewData["_natureMove"] = codeNatureMove.Trim();
            var structNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals(codeNatureMove));
            ViewBag.canConciliar = false;
            if (code == "129")
            {
                codeNatureMove = "A";
                ViewData["_natureMove"] = "A";
                structNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("E"));
            }
            if (codeNatureMove == "E" && customParamOP == "IPXM")
            {
                var paramPackagingMaterials = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("IPXM") as List<AdvanceParametersDetailModelP>;
                if (paramPackagingMaterials == null) throw new Exception("No se han definido parámetros para Egreso de Materiales de Empaque");

                var datCodeInventoryReason = paramPackagingMaterials.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "INRS");
                if (datCodeInventoryReason == null) throw new Exception("No se ha definido parámetros de Motivo de Inventario para Egreso de Materiales de Empaque");
                _codeInventoryReason = datCodeInventoryReason.nameAdvanceDetailModelP.Trim();

                var datCustomWarehouse = paramPackagingMaterials.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "CWAR");
                if (datCustomWarehouse == null) throw new Exception("No se han definido parámetros de Bodega Preederminada para Egreso de Materiales de Empaque");
                _customWarehouse = datCustomWarehouse.nameAdvanceDetailModelP.Trim();

                if (inventoryMove != null)
                {
                    InventoryMoveDetail _inventoryMoveDetail = inventoryMove.InventoryMoveDetail.First();

                    if (_inventoryMoveDetail != null)
                    {
                        inventoryMove.idLotePackagingMaterials = inventoryMove.id_productionLot;
                        inventoryMove.idWarehouseLocationPackagingMaterials = _inventoryMoveDetail.id_warehouseLocation;
                        inventoryMove.nameProviderPackagingMaterials = inventoryMove.ProductionLot?.Provider?.Person?.fullname_businessName;
                        inventoryMove.nameProductionUnitProviderPackagingMaterials = inventoryMove.ProductionLot?.ProductionUnitProvider?.name;
                        inventoryMove.idCostCenterPackagingMaterials = _inventoryMoveDetail.id_costCenter;
                        inventoryMove.idSubCostCenterPackagingMaterials = _inventoryMoveDetail.id_subCostCenter;
                        inventoryMove.idItemPackagingMaterials = _inventoryMoveDetail.id_item;
                        inventoryMove.nameItemDescriptionPackagingMaterials = _inventoryMoveDetail.Item?.description;
                    }
                }

                ViewData["_customParamOP"] = "IPXM";
                ViewData["_inventoryReason"] = _codeInventoryReason;
                ViewData["_customWarehousePackingM"] = _customWarehouse;
            }
            else
            {
                var paramPackagingMaterials = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("IPXM") as List<AdvanceParametersDetailModelP>;

                var datCodeInventoryReason = paramPackagingMaterials.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "INRS");
                if (datCodeInventoryReason == null) throw new Exception("No se ha definido parámetros de Motivo de Inventario.");
                _codeInventoryReason = datCodeInventoryReason.nameAdvanceDetailModelP.Trim();

                ViewData["_inventoryReason"] =
                ViewData["_customParamOP"] = "";
                ViewData["_customWarehousePackingM"] = "";
            }

            RefresshDataForEditForm(null);
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
                };

                inventoryMove.idNatureMove = structNatureMove.idAdvanceDetailModelP;
                inventoryMove.AdvanceParametersDetail = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == structNatureMove.idAdvanceDetailModelP);

                if (codeNatureMove == "E" && customParamOP == "IPXM")
                {
                    inventoryMove.id_inventoryReason = db.InventoryReason.FirstOrDefault(r => r.code == _codeInventoryReason)?.id;
                    inventoryMove.idWarehouse = db.Warehouse.FirstOrDefault(r => r.code == _customWarehouse)?.id;
                }

                #region OrdersDetails

                if (ordersDetails != null)
                {
                    List<InventoryMoveDetail> inventoryMoveDetails = new List<InventoryMoveDetail>();

                    foreach (var od in ordersDetails)
                    {
                        PurchaseOrderDetail tempPurchaseOrderDetail = db.PurchaseOrderDetail.FirstOrDefault(d => d.id == od);

                        var amountMoveAux = tempPurchaseOrderDetail.quantityApproved - tempPurchaseOrderDetail.quantityReceived;
                        var unitPriceMoveAux = (tempPurchaseOrderDetail.total / tempPurchaseOrderDetail.quantityApproved);
                        var itemAux = db.Item.FirstOrDefault(i => i.id == tempPurchaseOrderDetail.id_item);
                        InventoryMoveDetail inventoryMoveDetail = new InventoryMoveDetail
                        {
                            id = inventoryMove.InventoryMoveDetail.Count() > 0 ? inventoryMove.InventoryMoveDetail.Max(pld => pld.id) + 1 : 1,
                            id_item = tempPurchaseOrderDetail.id_item,
                            Item = itemAux,
                            id_warehouse = itemAux.ItemInventory.id_warehouse,
                            Warehouse = db.Warehouse.FirstOrDefault(w => w.id == itemAux.ItemInventory.id_warehouse),
                            id_warehouseLocation = itemAux.ItemInventory.id_warehouseLocation,
                            WarehouseLocation = db.WarehouseLocation.FirstOrDefault(w => w.id == itemAux.ItemInventory.id_warehouseLocation),
                            amountMove = amountMoveAux,
                            unitPriceMove = unitPriceMoveAux,
                            id_metricUnitMove = tempPurchaseOrderDetail.Item.ItemPurchaseInformation.id_metricUnitPurchase,
                            MetricUnit1 = tempPurchaseOrderDetail.Item.ItemPurchaseInformation.MetricUnit,
                            balanceCost = (amountMoveAux * unitPriceMoveAux),
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

                #endregion OrdersDetails

                #region InventoryMoveDetailTransferExitsDetails

                if (inventoryMoveDetailTransferExitsDetails != null)
                {
                    List<InventoryMoveDetail> inventoryMoveDetails2 = new List<InventoryMoveDetail>();

                    foreach (var od in inventoryMoveDetailTransferExitsDetails)
                    {
                        InventoryMoveDetail tempInventoryMoveDetail = db.InventoryMoveDetail.FirstOrDefault(d => d.id == od);
                        if (inventoryMove.idWarehouse == null)
                        {
                            inventoryMove.idWarehouse = tempInventoryMoveDetail.id_warehouseEntry;

                            inventoryMove.id_customer = tempInventoryMoveDetail.InventoryMove.id_customer;
                            inventoryMove.id_seller = tempInventoryMoveDetail.InventoryMove.id_seller;
                            inventoryMove.id_Invoice = tempInventoryMoveDetail.InventoryMove.id_Invoice;
                            inventoryMove.noFactura = tempInventoryMoveDetail.InventoryMove.noFactura;
                            inventoryMove.contenedor = tempInventoryMoveDetail.InventoryMove.contenedor;
                            inventoryMove.numberRemGuide = tempInventoryMoveDetail.InventoryMove.numberRemGuide;
                        }

                        emissionDateInicial = tempInventoryMoveDetail.InventoryMove.Document.emissionDate;
                        decimal receivedAmount = (tempInventoryMoveDetail != null && tempInventoryMoveDetail.InventoryMoveDetailTransfer != null &&
                                tempInventoryMoveDetail.InventoryMoveDetailTransfer.Where(w => w.InventoryMoveDetail1.InventoryMove.Document.DocumentState.code.Equals("03")).Count() > 0)
                                ? tempInventoryMoveDetail.InventoryMoveDetailTransfer.Where(w => w.InventoryMoveDetail1.InventoryMove.Document.DocumentState.code.Equals("03")).Sum(s => s.quantity)
                                : 0;
                        decimal amountMove = tempInventoryMoveDetail != null && tempInventoryMoveDetail.amountMove != null ? tempInventoryMoveDetail.amountMove.Value : 0;

                        var amountMoveAux = amountMove - receivedAmount;
                        var unitPriceMoveAux = tempInventoryMoveDetail.unitPriceMove;
                        var itemAux = db.Item.FirstOrDefault(i => i.id == tempInventoryMoveDetail.id_item);
                        var warehouse = db.Warehouse.FirstOrDefault(w => w.id == tempInventoryMoveDetail.id_warehouseEntry);
                        var whlocations = warehouse.WarehouseLocation.Where(r => r.isActive).ToList();
                        int? id_warehouseLocation = null;
                        WarehouseLocation warehouseLocation = null;
                        if (whlocations.Count == 1)
                        {
                            warehouseLocation = whlocations.FirstOrDefault();
                            id_warehouseLocation = warehouseLocation.id;
                        }
                        InventoryMoveDetail inventoryMoveDetail = new InventoryMoveDetail
                        {
                            id = inventoryMove.InventoryMoveDetail.Count() > 0 ? inventoryMove.InventoryMoveDetail.Max(pld => pld.id) + 1 : 1,
                            id_item = tempInventoryMoveDetail.id_item,
                            Item = itemAux,
                            id_warehouse = tempInventoryMoveDetail.id_warehouseEntry.Value,
                            Warehouse = warehouse,
                            id_warehouseLocation = id_warehouseLocation,
                            WarehouseLocation = warehouseLocation,
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
                            id_costCenter = tempInventoryMoveDetail.id_costCenter,
                            id_subCostCenter = tempInventoryMoveDetail.id_subCostCenter,
                            ordenProduccion = tempInventoryMoveDetail.ordenProduccion,
                            lotMarked = tempInventoryMoveDetail.lotMarked,
                            id_personProcessPlant = tempInventoryMoveDetail.id_personProcessPlant,
                            InventoryMoveDetailTransfer1 = new List<InventoryMoveDetailTransfer>(),
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
                    if (inventoryMove.InventoryMoveDetail == null && inventoryMove.InventoryMoveDetail.Count() > 0)
                    {
                        inventoryMove.id_costCenter = inventoryMove.InventoryMoveDetail.First().id_costCenter;
                        inventoryMove.id_subCostCenter = inventoryMove.InventoryMoveDetail.First().id_subCostCenter;
                    }
                }

                #endregion InventoryMoveDetailTransferExitsDetails

                #region Nature Move

                if (codeNatureMove.Equals("I"))
                {
                    inventoryMove.InventoryEntryMove = inventoryMove.InventoryEntryMove ?? new InventoryEntryMove { };
                    inventoryMove.InventoryEntryMove.id_receiver = ActiveUser.Employee.id;
                }
                else if (codeNatureMove.Equals("E"))
                {
                    inventoryMove.InventoryExitMove = inventoryMove.InventoryExitMove ?? new InventoryExitMove { };
                    inventoryMove.InventoryExitMove.id_dispatcher = ActiveUser.Employee.id;
                }
                else if (codeNatureMove.Equals("A"))
                {
                    inventoryMove.InventoryExitMove = inventoryMove.InventoryExitMove ?? new InventoryExitMove { };
                    inventoryMove.InventoryExitMove.id_dispatcher = ActiveUser.Employee.id;
                }

                #endregion Nature Move
            }

            ViewData["mostrarOP"] = inventoryMove.InventoryReason?.op ?? false;

            var settingORLS = db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
            ViewBag.withLotSystem = inventoryMove.InventoryReason?.requiereSystemLotNubmber ?? (settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == code)?.valueAux == "1" : false);
            var settingORLC = db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
            ViewBag.withLotCustomer = inventoryMove.InventoryReason?.requiereSystemLotNubmber ?? (settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == code)?.valueAux == "1" : false);
            ViewBag.idWarehouse = inventoryMove.idWarehouse;
            ViewBag.idWarehouseEntry = inventoryMove.idWarehouseEntry;
            ViewBag.FechaMinima = emissionDateInicial;
            inventoryMove.FillDocumentSourceInformation();

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            ViewData["numberDocument"] = inventoryMove?.sequential?.ToString();
            ViewData["numberSeqWarehouse"] = inventoryMove?.natureSequential;
            ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;

            // Búsqueda de documento            
            var estadoMovimientoInventario = inventoryMove?.Document != null
                ? db.DocumentState.FirstOrDefault(e => e.id == inventoryMove.Document.id_documentState)
                : db.DocumentState.FirstOrDefault(e => e.code == "01");

            // Permiso 
            int id_menu = (int)ViewData["id_menu"];
            var tienePermisioReversar = this.ActiveUser
                .UserMenu.FirstOrDefault(e => e.id_menu == id_menu)?
                .Permission?.FirstOrDefault(p => p.name == "Reversar");

            var tienePermisioConciliar = this.ActiveUser
                .UserMenu.FirstOrDefault(e => e.id_menu == id_menu)?
                .Permission?.FirstOrDefault(p => p.name == "Conciliar");

            var estadosMovimientoInventario = new[] { "03", "16" };
            var estadoParaReversar = db.DocumentState
                .Any(e => estadosMovimientoInventario.Contains(e.code) && e.id == estadoMovimientoInventario.id);

            ViewBag.canReversar = tienePermisioReversar != null && estadoParaReversar;

            var puedeConciliar = estadoMovimientoInventario.code == "03";

            ViewBag.canConciliar = tienePermisioConciliar != null && puedeConciliar;


            // Procesamos a DTO
            this.ConvertToDTO(inventoryMove.InventoryMoveDetail, inventoryMove.Document.emissionDate);
            BuildViewDataEdit();
            return PartialView("_InventoryMoveEditFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult InventoryMoveCopy(int id)
        {
            var inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            InventoryMove copyInventoryMove = null;
            if (inventoryMove.id != 0)
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

            ViewData["numberDocument"] = inventoryMove?.sequential?.ToString();
            ViewData["numberSeqWarehouse"] = inventoryMove?.natureSequential;
            ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;

            return PartialView("_InventoryMoveEditFormPartial", copyInventoryMove);
        }

        #endregion INVENTORY MOVE EDIT FORM

        #region INVENTORY MOVE FILTER RESULTS

        [HttpPost]
        public ActionResult InventoryResults(string natureMove,
                                            InventoryMove inventoryMove,
                                            InventoryEntryMove entryMove,
                                            InventoryExitMove exitMove,
                                            Document document,
                                            DateTime? startEmissionDate, DateTime? endEmissionDate,
                                            DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                            string customParamOP = null)
        {
            ViewData["_natureMove"] = natureMove;
            ViewData["_customParamOP"] = customParamOP;

            Parametros.ParametrosBusquedaInventoryMove parametrosBusquedaInventoryMove = new Parametros.ParametrosBusquedaInventoryMove();
            parametrosBusquedaInventoryMove.id_documentType = document.id_documentType;
            parametrosBusquedaInventoryMove.id_documentState = document.id_documentState;
            if (customParamOP == "IPXM")
            {
                var paramPackagingMaterials = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("IPXM") as List<AdvanceParametersDetailModelP>;
                if (paramPackagingMaterials == null) throw new Exception("No se han definido parámetros para Egreso de Materiales de Empaque");

                var datCodeInventoryReason = paramPackagingMaterials.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "INRS");
                if (datCodeInventoryReason == null) throw new Exception("No se ha definido parámetros de Motivo de Inventario para Egreso de Materiales de Empaque");
                string _codeInventoryReason = datCodeInventoryReason.nameAdvanceDetailModelP.Trim();
                parametrosBusquedaInventoryMove.codeInventoryReasonIPXM = _codeInventoryReason;
            }
            if (!string.IsNullOrEmpty(document.number)) parametrosBusquedaInventoryMove.number = document.number;
            if (!string.IsNullOrEmpty(document.reference)) parametrosBusquedaInventoryMove.reference = document.reference;
            parametrosBusquedaInventoryMove.startEmissionDate = startEmissionDate;
            parametrosBusquedaInventoryMove.endEmissionDate = endEmissionDate;
            parametrosBusquedaInventoryMove.startAuthorizationDate = startAuthorizationDate;
            parametrosBusquedaInventoryMove.endAuthorizationDate = endAuthorizationDate;
            if (!string.IsNullOrEmpty(document.accessKey)) parametrosBusquedaInventoryMove.accessKey = document.accessKey;
            if (!string.IsNullOrEmpty(document.authorizationNumber)) parametrosBusquedaInventoryMove.authorizationNumber = document.authorizationNumber;
            parametrosBusquedaInventoryMove.id_receiver = entryMove.id_receiver;
            parametrosBusquedaInventoryMove.id_dispatcher = exitMove.id_dispatcher;
            if (inventoryMove.id_inventoryReason != null) parametrosBusquedaInventoryMove.id_inventoryReason = inventoryMove.id_inventoryReason;
            if (entryMove.id_warehouseEntry != null) parametrosBusquedaInventoryMove.id_warehouseEntry = entryMove.id_warehouseEntry;
            if (entryMove.id_warehouseLocationEntry != null) parametrosBusquedaInventoryMove.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
            if (exitMove.id_warehouseExit != null) parametrosBusquedaInventoryMove.id_warehouseExit = exitMove.id_warehouseExit;
            if (exitMove.id_warehouseLocationExit != null) parametrosBusquedaInventoryMove.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
            parametrosBusquedaInventoryMove.id_user = ActiveUser.id;

            List<InventoryMoveDTO> modelAux = GetInventoryMove(parametrosBusquedaInventoryMove);

            TempData["model"] = modelAux;
            TempData.Keep("model");

            ViewData["code"] = db.DocumentType.FirstOrDefault(fod => fod.id == document.id_documentType)?.code ?? "";
            ViewData["valorization"] = inventoryMove?.InventoryReason?.valorization ?? "";

            return PartialView("_InventoryResults", modelAux);
        }

        private List<InventoryMoveDTO> GetInventoryMove(Parametros.ParametrosBusquedaInventoryMove parametrosBusquedaInventoryMove)
        {
            var parametrosBusquedaInventoryMoveAux = new SqlParameter();
            parametrosBusquedaInventoryMoveAux.ParameterName = "@ParametrosBusquedaInventoryMove";
            parametrosBusquedaInventoryMoveAux.Direction = ParameterDirection.Input;
            parametrosBusquedaInventoryMoveAux.SqlDbType = SqlDbType.NVarChar;
            var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaInventoryMove);
            parametrosBusquedaInventoryMoveAux.Value = jsonAux;
            db.Database.CommandTimeout = 1200;
            List<InventoryMoveDTO> modelAux = db.Database.SqlQuery<InventoryMoveDTO>("exec inv_Consultar_InventoryMove_StoredProcedure @ParametrosBusquedaInventoryMove ", parametrosBusquedaInventoryMoveAux).ToList();

            modelAux = modelAux.Where(w => !(w.code_documentType == "155" || w.code_documentType == "156")).ToList();

            return modelAux;
        }

        public ActionResult GetWarehouseEntry()
        {
            return PartialView("ComponentsDetail/_ComboBoxWarehouseEntry");
        }

        public ActionResult GetWarehouseExit()
        {
            return PartialView("ComponentsDetail/_ComboBoxWarehouseExit");
        }

        public ActionResult GetWarehouseLocationEntry(int? id_warehouseEntry)
        {
            int idWarehouseEntry = 0;
            if (id_warehouseEntry == null || id_warehouseEntry < 0)
            {
                if (Request.Params["id_warehouseEntry"] != null && Request.Params["id_warehouseEntry"] != "")
                    idWarehouseEntry = int.Parse(Request.Params["id_warehouseEntry"]);
                else idWarehouseEntry = -1;
            }

            ViewBag.IdWarehouseEntry = id_warehouseEntry;

            return PartialView("ComponentsDetail/_ComboBoxWarehouseEntryLocation");
        }

        public ActionResult GetWarehouseLocationExit(int? id_warehouseExit)
        {
            int idWarehouseExit = 0;
            if (id_warehouseExit == null || id_warehouseExit < 0)
            {
                if (Request.Params["id_warehouseExit"] != null && Request.Params["id_warehouseExit"] != "")
                    idWarehouseExit = int.Parse(Request.Params["id_warehouseExit"]);
                else idWarehouseExit = -1;
            }

            ViewBag.IdWarehouseExit = id_warehouseExit;

            return PartialView("ComponentsDetail/_ComboBoxWarehouseExitLocation");
        }

        #endregion INVENTORY MOVE FILTER RESULTS

        #region INVENTORY MOVE HEADER

        [ValidateInput(false)]
        public ActionResult InventoryMovesPartial(string code, string natureMove)
        {
            var model = TempData["model"] as List<InventoryMoveDTO>;
            model = model ?? new List<InventoryMoveDTO>();

            TempData.Keep("model");

            ViewData["_natureMove"] = natureMove;
            ViewData["code"] = code;

            return PartialView("_InventoryMovesPartial", model);
        }

        #region Optmizacion Ingreso Tranferencia

        private ActionResult InventoryMovesPartialExitTransferAddNew
            (bool approve, string codeDocumentType, string natureMoveIMTmp, InventoryMove item,
                Document document, InventoryEntryMove entryMove, InventoryExitMove exitMove,
                bool multiple, string customParamOP = null)
        {
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            InventoryMoveExitPackingMaterial _inventoryMoveExitPackingMaterial = new InventoryMoveExitPackingMaterial();

            int? idProductionLot = null;
            bool isValid = false;
            bool isValidTransacction = false;
            bool isNew = false;


            List<Item> itemList = new List<Item>();
            Setting[] settings = Array.Empty<Setting>();
            Warehouse[] warehouses = Array.Empty<Warehouse>();
            WarehouseLocation[] warehouseLocations = Array.Empty<WarehouseLocation>();
            DocumentState[] documentStates = Array.Empty<DocumentState>();
            EmissionPoint[] emissionPoints = Array.Empty<EmissionPoint>();
            InventoryReason[] inventoryReasons = Array.Empty<InventoryReason>();
            Employee[] employees = Array.Empty<Employee>();
            MetricUnit[] metricUnits = Array.Empty<MetricUnit>();
            MetricUnitConversion[] metricUnitConversions = Array.Empty<MetricUnitConversion>();
            DocumentType[] documentTypes = Array.Empty<DocumentType>();

            #region Informacion Log
            string identificador = (item.id != 0) ? item.id.ToString() : Guid.NewGuid().ToString();
            LogInfo($"ini-{codeDocumentType}-{identificador}-Aprovee:{approve}", DateTime.Now);
            #endregion


            string tag = "";
            ServiceInventoryMove.ServiceInventoryMoveAux prepareResults = null;
            IDbConnection connection = null;

            List<int> inventoryMoveDetailIdsForDeleteList = new List<int>();
            int[] inventoryMoveDetailIdsForDelete = Array.Empty<int>();

            try
            {
                LogInfo($"val-{identificador}", DateTime.Now);
                #region Optimiza Codigo

                settings = db.Setting.ToArray();
                warehouses = db.Warehouse.ToArray();
                warehouseLocations = db.WarehouseLocation.ToArray();
                documentStates = db.DocumentState.ToArray();
                emissionPoints = db.EmissionPoint.ToArray();
                inventoryReasons = db.InventoryReason.ToArray();
                employees = db.Employee.ToArray();
                metricUnits = db.MetricUnit.ToArray();
                metricUnitConversions = db.MetricUnitConversion.ToArray();
                documentTypes = db.DocumentType.ToArray();
                #endregion

                #region RA | Optimizacion Fx Inventario -000 
                if (tempInventoryMove == null)
                {
                    throw new ProdHandlerException("No existen datos para módelo de Movimiento de Inventario desde el cache");
                }
                if (item == null)
                {
                    throw new ProdHandlerException("No existen datos para módelo de Movimiento de Inventario");
                }
                int[] ids = tempInventoryMove.InventoryMoveDetail.Select(r => r.id_item).ToArray();
                itemList = db.Item.Where(it => it.isActive && ids.Contains(it.id)).ToList();

                tempInventoryMove.idWarehouse = item.idWarehouse;
                tempInventoryMove.id_inventoryReason = item.id_inventoryReason;
                if (inventoryReasons == null)
                {
                    throw new ProdHandlerException("No existen datos en el listado de Motivos de Inventario");
                }
                if (item.id_inventoryReason == null)
                {
                    throw new ProdHandlerException("Motivo de inventario no definido");
                }
                tempInventoryMove.InventoryReason = inventoryReasons.FirstOrDefault(fod => fod.id == item.id_inventoryReason);
                tempInventoryMove.idWarehouseEntry = item.idWarehouseEntry;
                tempInventoryMove.id_provider = item.id_provider;
                tempInventoryMove.id_costCenter = item.id_costCenter;
                tempInventoryMove.id_subCostCenter = item.id_subCostCenter;
                tempInventoryMove.id_customer = item.id_customer;
                tempInventoryMove.id_seller = item.id_seller;
                tempInventoryMove.noFactura = item.noFactura;
                tempInventoryMove.id_Invoice = item.id_Invoice;
                tempInventoryMove.contenedor = item.contenedor;
                tempInventoryMove.numberRemGuide = item.numberRemGuide;

                if (tempInventoryMove.Document == null || document == null)
                {
                    throw new ProdHandlerException("No existe modelo de Documento");
                }

                tempInventoryMove.Document.description = document.description;

                if (customParamOP == "IPXM")
                {
                    _inventoryMoveExitPackingMaterial = tempInventoryMove.InventoryMoveExitPackingMaterial;
                    idProductionLot = item.id_productionLot;
                }

                ViewData["_natureMove"] = natureMoveIMTmp?.Trim();
                ViewData["_customParamOP"] = customParamOP?.Trim();

                if (customParamOP == "IPXM")
                {
                    _inventoryMoveExitPackingMaterial = tempInventoryMove.InventoryMoveExitPackingMaterial;
                    Item _itemMaster;
                    idProductionLot = item.id_productionLot;
                    if (_inventoryMoveExitPackingMaterial != null)
                    {
                        _itemMaster = db.Item.FirstOrDefault(r => r.id == _inventoryMoveExitPackingMaterial.id_ItemMaster);
                        if (_itemMaster != null)
                        {
                            string _description = document.description;
                            string _newDescription = " Producto: " + _itemMaster.masterCode + " | " + _itemMaster.description + Environment.NewLine
                                                                    + "Cantidad dosificada: " + _inventoryMoveExitPackingMaterial.quantityExit;

                            if (_description == null)
                            {
                                document.description = _newDescription;
                            }
                            else
                            {
                                string splitDescritionOri = Regex.Replace(_description, @"\t|\n|\r", "");

                                string[] splitDescription = splitDescritionOri.Split(new string[] { " Producto:" }, StringSplitOptions.None).ToArray();

                                if (splitDescription.Count() >= 2)
                                {
                                    document.description = splitDescription[0] + Environment.NewLine + _newDescription;
                                }
                                else
                                {
                                    document.description = document.description + Environment.NewLine + _newDescription;
                                }
                            }
                        }
                    }
                }

                tempInventoryMove.Document.description = document.description != null ? document.description : "";

                item.Document = document;
                item.InventoryEntryMove = entryMove;
                item.InventoryExitMove = exitMove;

                item.InventoryMoveDetail = tempInventoryMove.InventoryMoveDetail;

                var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                if (entityObjectPermissions != null)
                {

                    var entityPermissions = entityObjectPermissions.listEntityPermissions?.FirstOrDefault(fod => fod.codeEntity == "WAH");

                    if (entityPermissions != null)
                    {

                        if (item.idWarehouse != null)
                        {

                            var entityValuePermissions = entityPermissions
                                                                .listValue?
                                                                .FirstOrDefault(fod2 => fod2?.id_entityValue == item.idWarehouse
                                                                                        && fod2?.listPermissions?.FirstOrDefault(fod3 => fod3.name == "Editar") != null);

                            if (entityValuePermissions == null)
                            {
                                throw new ProdHandlerException("No tiene Permiso para editar y guardar el movimiento de inventario.");
                            }
                        }

                        if (approve)
                        {
                            var _inventoryDetailPeriod = db.InventoryPeriodDetail.Where(a => a.AdvanceParametersDetail.valueCode == "A" && a.InventoryPeriod.isActive).ToList();
                            var _emissionDateInventoryMove = item.Document.emissionDate;
                            var añoEmission = _emissionDateInventoryMove.Year;
                            var mesEmission = _emissionDateInventoryMove.Month;

                            var _inventoryPeriodActivo = _inventoryDetailPeriod?.Any(e => e.InventoryPeriod.year == añoEmission
                                                                                          && e.dateInit.Month == mesEmission
                                                                                          && e.InventoryPeriod.id_warehouse == item.idWarehouse);

                            var _bodega = db.Warehouse.FirstOrDefault(a => a.id == item.idWarehouse);

                            if (!(_inventoryPeriodActivo ?? false))
                            {
                                throw new ProdHandlerException("No existe periodo de inventario abierto para la " + _bodega?.name);
                            }

                            if (item.idWarehouseEntry != null)
                            {
                                var _inventoryPeriodActivoBodegaEntrada = _inventoryDetailPeriod?.Any(e => e.InventoryPeriod.year == añoEmission
                                                                                                           && e.dateInit.Month == mesEmission
                                                                                                           && e.InventoryPeriod.id_warehouse == item.idWarehouseEntry);

                                var _bodegaEntrada = db.Warehouse.FirstOrDefault(a => a.id == item.idWarehouseEntry);

                                if (!(_inventoryPeriodActivoBodegaEntrada ?? false))
                                {
                                    throw new ProdHandlerException($"No existe periodo de inventario abierto para la {_bodegaEntrada?.name}");
                                }
                            }

                            if (item.idWarehouse != null)
                            {
                                var entityValuePermissions = entityPermissions
                                                                .listValue?
                                                                .FirstOrDefault(fod2 => fod2.id_entityValue == item.idWarehouse
                                                                                        && fod2.listPermissions?.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new ProdHandlerException("No tiene Permiso para editar y guardar el movimiento de inventario.");
                                }
                            }
                            InvoiceDetailValid[] InvoiceDetailValid = new InvoiceDetailValid[] { };
                            var cantidadCartinesFactura = 0;
                            var cantidadCartonesMovimiento = 0.00m;
                            if (tempInventoryMove.id_Invoice.HasValue)
                            {
                                InvoiceExterior invoiceExterior = db.InvoiceExterior.FirstOrDefault(a => a.id == tempInventoryMove.id_Invoice.Value);
                                if (invoiceExterior == null)
                                {
                                    throw new ProdHandlerException("No existe la Factura ingresada");
                                }

                                InvoiceDetailValid = invoiceExterior
                                                            .Invoice?
                                                            .InvoiceDetail?
                                                            .Where(e => e.id_invoice == tempInventoryMove.id_Invoice
                                                                        && e.isActive)?
                                                            .Select(e => new InvoiceDetailValid()
                                                            {
                                                                IdItem = e.id_item,
                                                                Cantidad = e.numBoxes ?? 0,
                                                            })?
                                                            .ToArray() ?? Array.Empty<InventoryMoveController.InvoiceDetailValid>();

                                var inventoryMoveDetailAux = item
                                                                .InventoryMoveDetail?
                                                                .GroupBy(a => a.id_item)?
                                                                .Select(r => new
                                                                {
                                                                    id_item = r.Key,
                                                                    cantidad = r.Sum(rr => rr.amountMove)
                                                                })?
                                                                .ToList();

                                foreach (var inventoryMoveDetail in inventoryMoveDetailAux)
                                {
                                    foreach (var InvoiceDetail in InvoiceDetailValid)
                                    {
                                        if (InvoiceDetail.IdItem == inventoryMoveDetail.id_item && InvoiceDetail.Cantidad != inventoryMoveDetail.cantidad)
                                        {
                                            var itemMasterCode = db.Item.FirstOrDefault(a => a.id == inventoryMoveDetail.id_item).masterCode;
                                            throw new ProdHandlerException("La cantidad del producto " + itemMasterCode + " debe ser igual a " + InvoiceDetail.Cantidad + " indicado en la factura.");
                                        }
                                    }
                                }

                                cantidadCartinesFactura = (invoiceExterior != null) ? invoiceExterior.totalBoxes : 0;
                                cantidadCartonesMovimiento = (inventoryMoveDetailAux != null) ? inventoryMoveDetailAux.Sum(a => a.cantidad.Value) : 0;
                                if (cantidadCartinesFactura != cantidadCartonesMovimiento)
                                {
                                    throw new ProdHandlerException("La cantidad total del movimiento, debe ser igual a " + cantidadCartinesFactura + " indicado en la factura.");
                                }
                            }

                        }
                    }
                }

                if (item.InventoryMoveDetail.Count == 0)
                {
                    throw new ProdHandlerException("No se puede guardar un movimiento de inventario sin detalles.");
                }

                var lotReceptionDatePar = db.Setting.FirstOrDefault(fod => fod.code == "VALLOT")?.value ?? "NO";

                if (lotReceptionDatePar == "SI" && codeDocumentType != "34")
                {

                    var fechaEmisionM = item.Document.emissionDate.Date;

                    foreach (var itemDetail in item.InventoryMoveDetail)
                    {

                        var fechaRecpecion = itemDetail?.Lot?.ProductionLot?.receptionDate ?? itemDetail?.Lot?.Document?.emissionDate ?? null;
                        var internalNumber = itemDetail?.Lot?.ProductionLot?.internalNumber ?? itemDetail?.Lot?.internalNumber ?? null;

                        if (!fechaRecpecion.HasValue)
                        {
                            throw new ProdHandlerException($"El lote {internalNumber} no tiene fecha.");
                        }

                        if (fechaRecpecion.Value.Date < fechaEmisionM)
                        {
                            throw new ProdHandlerException($"La fecha de Recepción del lote {internalNumber} es menor a la fecha del movimiento.");
                        }
                    }
                }


                var inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_costCenter == null);
                if (inventoryMoveDetailAux2 != null)
                {
                    throw new ProdHandlerException("No se puede guardar el movimiento de inventario sin centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }


                inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_subCostCenter == null);
                if (inventoryMoveDetailAux2 != null)
                {
                    throw new ProdHandlerException("No se puede guardar el movimiento de inventario sin sub-centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }

                #endregion RA  | Optimizacion Fx Inventario -000


                #region  RA  | Optimizacion Fx Inventario - 001

                prepareResults = ServiceInventoryMove.ValidateUpdateInventaryMoveTransferExit(approve, ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                if (!string.IsNullOrEmpty(prepareResults?.message)) throw new ProdHandlerException(prepareResults.message);
                item = prepareResults.inventoryMove;
                isNew = prepareResults.isNew;

                #endregion RA  | Optimizacion Fx Inventario - 001
                isValid = true;
            }
            catch (ProdHandlerException e)
            {
                item = tempInventoryMove;
                ViewData["ErrorMessage"] = $"{(e.Message ?? "")}";
                if (string.IsNullOrEmpty(e.Message))
                {
                    FullLog(e, seccion: identificador);
                }
            }
            catch (Exception e)
            {
                item = tempInventoryMove;
                ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                FullLog(e, seccion: identificador);
            }
            finally
            {
                TempData.Keep("inventoryMove");
            }

            if (isValid)
            {
                try
                {

                    LogInfo($"tran-{identificador}", DateTime.Now);
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {

                            LogInfo($"tran-{identificador}", DateTime.Now);
                            #region Update Gen. Sec. Trans.

                            InventoryReason inventoryReasonAux = db.InventoryReason.FirstOrDefault(i => i.id == item.id_inventoryReason);

                            if (inventoryReasonAux == null)
                            {
                                throw new ProdHandlerException("No se ha configurado el motivo de inventario seleccionado");
                            }


                            ViewBag.withLotSystem = inventoryReasonAux.requiereSystemLotNubmber ?? false;
                            ViewBag.withLotCustomer = inventoryReasonAux.requiereUserLotNubmber ?? false;

                            var inventoryMoveAux = db.InventoryMove.FirstOrDefault(fod => fod.id == item.id);

                            foreach (var itemDetailAux in tempInventoryMove.InventoryMoveDetail.Where(x => x.genSecTrans))
                            {

                                // if (itemDetailAux.genSecTrans)
                                //{

                                var sequentialAux = inventoryReasonAux?.sequential;
                                var secTransAux = (inventoryReasonAux?.code ?? "") + sequentialAux?.ToString("D9") ?? "";
                                Lot lotAux = null;

                                var inventoryMoveDetailAux = inventoryMoveAux?.InventoryMoveDetail?.FirstOrDefault(fod => fod.id == itemDetailAux.id);
                                if (inventoryMoveDetailAux != null)
                                {

                                    if (inventoryMoveDetailAux.genSecTrans)
                                    {
                                        lotAux = getLotSinTransaccion(inventoryMoveDetailAux.Lot?.number, itemDetailAux.Lot?.internalNumber);
                                        itemDetailAux.id_lot = lotAux?.id;
                                        itemDetailAux.Lot = lotAux;
                                    }
                                    else
                                    {

                                        lotAux = getLotSinTransaccion(secTransAux, itemDetailAux.Lot?.internalNumber);
                                        itemDetailAux.id_lot = lotAux?.id;
                                        itemDetailAux.Lot = lotAux;

                                        inventoryReasonAux.sequential++;
                                        db.InventoryReason.Attach(inventoryReasonAux);
                                        db.Entry(inventoryReasonAux).State = EntityState.Modified;
                                    }
                                }
                                else
                                {

                                    lotAux = getLotSinTransaccion(secTransAux, itemDetailAux.Lot?.internalNumber);
                                    itemDetailAux.id_lot = lotAux?.id;
                                    itemDetailAux.Lot = lotAux;

                                    inventoryReasonAux.sequential++;
                                    db.InventoryReason.Attach(inventoryReasonAux);
                                    db.Entry(inventoryReasonAux).State = EntityState.Modified;
                                }
                                //}
                            }

                            #endregion Update Gen. Sec. Trans.

                            tempInventoryMove.InventoryEntryMove = entryMove;
                            tempInventoryMove.InventoryExitMove = exitMove;

                            item.idNatureMove = inventoryReasons.FirstOrDefault(fod => fod.id == item.id_inventoryReason)?.idNatureMove;


                            ServiceInventoryMove.ServiceInventoryMoveAux result = null;

                            LogInfo($"tran-serv-exit-{identificador}", DateTime.Now);
                            result = ServiceInventoryMove
                                                        .ExecUpdateInventoryMoveTransferExit(approve,
                                                                                                ActiveUser,
                                                                                                ActiveCompany,
                                                                                                ActiveEmissionPoint,
                                                                                                item,
                                                                                                db,
                                                                                                false,
                                                                                                isNew,
                                                                                                settings,
                                                                                                warehouses,
                                                                                                warehouseLocations,
                                                                                                documentStates,
                                                                                                emissionPoints,
                                                                                                inventoryReasons,
                                                                                                employees,
                                                                                                metricUnits,
                                                                                                metricUnitConversions,
                                                                                                identificador: identificador
                                                                                                );
                            if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                            inventoryMoveDetailIdsForDeleteList.AddRange(result.inventoryMoveDetailIdsForDelete ?? Array.Empty<int>());
                            LogInfo($"tran-serv-exit-end-{identificador}", DateTime.Now);

                            if (approve)
                            {
                                LogInfo($"tran-serv-cmpy-entry-{identificador}", DateTime.Now);
                                var resultVirtual = UpdateInventaryMoveVirtualCompanyEntryOP(
                                                        approve,
                                                        ActiveUser,
                                                        ActiveCompany,
                                                        ActiveEmissionPoint,
                                                        result.inventoryMove,
                                                        db,
                                                        reverse: false,
                                                        settings,
                                                        warehouses,
                                                        warehouseLocations,
                                                        documentStates,
                                                        emissionPoints,
                                                        inventoryReasons,
                                                        employees,
                                                        metricUnits,
                                                        metricUnitConversions,
                                                        null,
                                                        identificador: identificador);
                                if (!string.IsNullOrEmpty(resultVirtual?.message)) throw new ProdHandlerException(resultVirtual.message);
                                inventoryMoveDetailIdsForDeleteList.AddRange(result.inventoryMoveDetailIdsForDelete ?? Array.Empty<int>());
                                LogInfo($"tran-serv-cmpy-entry-end-{identificador}", DateTime.Now);
                            }

                            if (customParamOP == "IPXM")
                            {
                                if (result != null)
                                {
                                    if (result.inventoryMove != null)
                                    {
                                        string codeWareHouse = warehouses.FirstOrDefault(r => r.id == result.inventoryMove.idWarehouse)?.code;
                                        ViewData["_codeWareHouse"] = codeWareHouse;

                                        result.inventoryMove.id_productionLot = idProductionLot;

                                        InventoryMoveDetail _inventoryMoveDetail = result.inventoryMove.InventoryMoveDetail.First();

                                        if (_inventoryMoveDetail != null)
                                        {
                                            ProductionLot _productionLot = db.ProductionLot.FirstOrDefault(r => r.id == result.inventoryMove.id_productionLot);

                                            result.inventoryMove.idLotePackagingMaterials = result.inventoryMove.id_productionLot;
                                            result.inventoryMove.idWarehouseLocationPackagingMaterials = _inventoryMoveDetail.id_warehouseLocation;
                                            result.inventoryMove.nameProviderPackagingMaterials = _productionLot?.Provider?.Person?.fullname_businessName;
                                            result.inventoryMove.nameProductionUnitProviderPackagingMaterials = _productionLot?.ProductionUnitProvider?.name;
                                            result.inventoryMove.idCostCenterPackagingMaterials = _inventoryMoveDetail.id_costCenter;
                                            result.inventoryMove.idSubCostCenterPackagingMaterials = _inventoryMoveDetail.id_subCostCenter;
                                            result.inventoryMove.idItemPackagingMaterials = _inventoryMoveDetail.id_item;
                                            result.inventoryMove.nameItemDescriptionPackagingMaterials = _inventoryMoveDetail.Item?.description;
                                        }

                                        if (result.inventoryMove.id == 0)
                                        {
                                            result.inventoryMove.InventoryMoveExitPackingMaterial = _inventoryMoveExitPackingMaterial;
                                        }
                                        else
                                        {
                                            InventoryMoveExitPackingMaterial _inventoryMoveExitPackingMaterialResult = db.InventoryMoveExitPackingMaterial
                                                .FirstOrDefault(r => r.id_InventoryMove == item.id);

                                            if (_inventoryMoveExitPackingMaterialResult != null
                                                && _inventoryMoveExitPackingMaterialResult.id_ItemMaster != _inventoryMoveExitPackingMaterial?.id_ItemMaster)
                                            {
                                                db.InventoryMoveExitPackingMaterial.Attach(_inventoryMoveExitPackingMaterialResult);
                                                db.Entry(_inventoryMoveExitPackingMaterialResult).State = EntityState.Deleted;

                                                result.inventoryMove.InventoryMoveExitPackingMaterial = _inventoryMoveExitPackingMaterial;
                                            }
                                            else
                                            {
                                                if (_inventoryMoveExitPackingMaterialResult == null)
                                                {
                                                    result.inventoryMove.InventoryMoveExitPackingMaterial = _inventoryMoveExitPackingMaterial;
                                                }
                                                else
                                                {
                                                    result.inventoryMove.InventoryMoveExitPackingMaterial.quantityExit = _inventoryMoveExitPackingMaterial.quantityExit;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            item = result?.inventoryMove;

                            LogInfo($"tran-track-{identificador}", DateTime.Now);
                            if (item.id == 0)
                            {
                                db.InventoryMove.Add(item);
                            }
                            else
                            {
                                db.Entry(item).State = EntityState.Modified;
                            }

                            db.SaveChanges();
                            trans.Commit();
                            isValidTransacction = true;
                        }
                        catch (Exception e)
                        {
                            trans.Rollback();
                            FullLog(e, seccion: tag);
                            throw;
                        }
                    }

                    if (isValidTransacction)
                    {
                        inventoryMoveDetailIdsForDelete = inventoryMoveDetailIdsForDeleteList.ToArray();
                        if (codeDocumentType == "32" && ((inventoryMoveDetailIdsForDelete?.Length ?? 0)) > 0)
                        {
                            LogInfo($"tran-has-delete-{identificador}", DateTime.Now);
                            Task.Run(async () =>
                            {

                                await ServiceTransCtl.DeleteBulkSp(inventoryMoveDetailIdsForDelete, GetType().Name, (item?.id ?? 0));

                            });

                        }

                    }

                    ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + item.Document.number + " guardado exitosamente");

                    TempData["inventoryMove"] = item;
                    TempData.Keep("inventoryMove");

                    ViewData["numberSeqWarehouse"] = item.natureSequential;
                    item.FillDocumentSourceInformation();
                    ViewData["numberDocument"] = item?.sequential?.ToString();
                    ViewData["numberSeqWarehouse"] = item?.natureSequential;
                    ViewData["fechaEmision"] = item?.Document?.emissionDate;

                    this.ConvertToDTO(item?.InventoryMoveDetail, item?.Document?.emissionDate);

                    BuildViewDataEdit();

                }
                catch (ProdHandlerException e)
                {
                    ViewData["ErrorMessage"] = $"{(e.Message ?? "")}";
                    item = tempInventoryMove;

                    if (string.IsNullOrEmpty(e.Message))
                    {
                        FullLog(e, seccion: identificador);
                    }
                }
                catch (Exception e)
                {
                    ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                    item = tempInventoryMove;
                    FullLog(e, seccion: identificador);
                }


            }

            LogInfo($"end-{identificador}", DateTime.Now);
            return PartialView("_InventoryMoveMainFormPartial", item);
        }



        private ActionResult InventoryMovesPartialEntryTransferAddNew(bool approve, string codeDocumentType,
            string natureMoveIMTmp, InventoryMove item, Document document, InventoryEntryMove entryMove, InventoryExitMove exitMove,
            bool multiple, string customParamOP = null)
        {

            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            InventoryMoveExitPackingMaterial _inventoryMoveExitPackingMaterial = new InventoryMoveExitPackingMaterial();

            int? idProductionLot = null;
            bool isValid = false;
            bool isValidTransacction = false;
            bool isNew = false;


            List<Item> itemList = new List<Item>();

            Setting[] settings = Array.Empty<Setting>();
            Warehouse[] warehouses = Array.Empty<Warehouse>();
            WarehouseLocation[] warehouseLocations = Array.Empty<WarehouseLocation>();
            DocumentState[] documentStates = Array.Empty<DocumentState>();
            EmissionPoint[] emissionPoints = Array.Empty<EmissionPoint>();
            InventoryReason[] inventoryReasons = Array.Empty<InventoryReason>();
            Employee[] employees = Array.Empty<Employee>();
            MetricUnit[] metricUnits = Array.Empty<MetricUnit>();
            MetricUnitConversion[] metricUnitConversions = Array.Empty<MetricUnitConversion>();
            DocumentType[] documentTypes = Array.Empty<DocumentType>();


            string tag = "";
            ServiceInventoryMove.ServiceInventoryMoveAux prepareResults = null;
            IDbConnection connection = null;

            int[] inventoryMoveDetailIdsForDelete = Array.Empty<int>();
            try
            {
                tag = "001";
                LogInfo(tag, DateTime.Now);

                #region Optimiza Codigo
                itemList = db.Item.Where(it => it.isActive).ToList();                
                settings = db.Setting.ToArray();
                warehouses = db.Warehouse.ToArray();
                warehouseLocations = db.WarehouseLocation.ToArray();
                documentStates = db.DocumentState.ToArray();
                emissionPoints = db.EmissionPoint.ToArray();
                inventoryReasons = db.InventoryReason.ToArray();
                employees = db.Employee.ToArray();
                metricUnits = db.MetricUnit.ToArray();
                metricUnitConversions = db.MetricUnitConversion.ToArray();
                documentTypes = db.DocumentType.ToArray();
                #endregion

                tag = "002";
                LogInfo(tag, DateTime.Now);
                #region RA | Optimizacion Fx Inventario -000 
                if (tempInventoryMove == null)
                {
                    throw new ProdHandlerException("No existen datos para módelo de Movimiento de Inventario desde el cache");
                }
                if (item == null)
                {
                    throw new ProdHandlerException("No existen datos para módelo de Movimiento de Inventario");
                }
                tempInventoryMove.idWarehouse = item.idWarehouse;
                tempInventoryMove.id_inventoryReason = item.id_inventoryReason;
                if (inventoryReasons == null)
                {
                    throw new ProdHandlerException("No existen datos en el listado de Motivos de Inventario");
                }
                if (item.id_inventoryReason == null)
                {
                    throw new ProdHandlerException("Motivo de inventario no definido");
                }
                tempInventoryMove.InventoryReason = inventoryReasons.FirstOrDefault(fod => fod.id == item.id_inventoryReason);
                tempInventoryMove.idWarehouseEntry = item.idWarehouseEntry;
                tempInventoryMove.id_provider = item.id_provider;
                tempInventoryMove.id_costCenter = item.id_costCenter;
                tempInventoryMove.id_subCostCenter = item.id_subCostCenter;
                tempInventoryMove.id_customer = item.id_customer;
                tempInventoryMove.id_seller = item.id_seller;
                tempInventoryMove.noFactura = item.noFactura;
                tempInventoryMove.id_Invoice = item.id_Invoice;
                tempInventoryMove.contenedor = item.contenedor;
                tempInventoryMove.numberRemGuide = item.numberRemGuide;

                tag = "003";
                LogInfo(tag, DateTime.Now);
                if (tempInventoryMove.Document == null || document == null)
                {
                    throw new ProdHandlerException("No existe modelo de Documento");
                }
                tag = "004";
                LogInfo(tag, DateTime.Now);
                tempInventoryMove.Document.description = document.description;

                tag = "005";
                LogInfo(tag, DateTime.Now);
                if (customParamOP == "IPXM")
                {
                    _inventoryMoveExitPackingMaterial = tempInventoryMove.InventoryMoveExitPackingMaterial;
                    idProductionLot = item.id_productionLot;
                }

                tag = "006";
                LogInfo(tag, DateTime.Now);
                ViewData["_natureMove"] = natureMoveIMTmp?.Trim();
                ViewData["_customParamOP"] = customParamOP?.Trim();

                tag = "007";
                LogInfo(tag, DateTime.Now);
                if (customParamOP == "IPXM")
                {
                    _inventoryMoveExitPackingMaterial = tempInventoryMove.InventoryMoveExitPackingMaterial;
                    Item _itemMaster;
                    idProductionLot = item.id_productionLot;
                    if (_inventoryMoveExitPackingMaterial != null)
                    {
                        _itemMaster = db.Item.FirstOrDefault(r => r.id == _inventoryMoveExitPackingMaterial.id_ItemMaster);
                        if (_itemMaster != null)
                        {
                            string _description = document.description;
                            string _newDescription = " Producto: " + _itemMaster.masterCode + " | " + _itemMaster.description + Environment.NewLine
                                                                    + "Cantidad dosificada: " + _inventoryMoveExitPackingMaterial.quantityExit;

                            if (_description == null)
                            {
                                document.description = _newDescription;
                            }
                            else
                            {
                                string splitDescritionOri = Regex.Replace(_description, @"\t|\n|\r", "");

                                string[] splitDescription = splitDescritionOri.Split(new string[] { " Producto:" }, StringSplitOptions.None).ToArray();

                                if (splitDescription.Count() >= 2)
                                {
                                    document.description = splitDescription[0] + Environment.NewLine + _newDescription;
                                }
                                else
                                {
                                    document.description = document.description + Environment.NewLine + _newDescription;
                                }
                            }
                        }
                    }
                }

                tag = "008";
                LogInfo(tag, DateTime.Now);
                tempInventoryMove.Document.description = document.description != null ? document.description : "";

                tag = "009";
                LogInfo(tag, DateTime.Now);
                item.Document = document;
                item.InventoryEntryMove = entryMove;
                item.InventoryExitMove = exitMove;

                tag = "010";
                LogInfo(tag, DateTime.Now);
                item.InventoryMoveDetail = tempInventoryMove.InventoryMoveDetail;

                tag = "011";
                LogInfo(tag, DateTime.Now);
                var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                tag = "012";
                LogInfo(tag, DateTime.Now);
                if (entityObjectPermissions != null)
                {
                    tag = "013";
                    LogInfo(tag, DateTime.Now);
                    var entityPermissions = entityObjectPermissions.listEntityPermissions?.FirstOrDefault(fod => fod.codeEntity == "WAH");

                    tag = "014";
                    LogInfo(tag, DateTime.Now);
                    if (entityPermissions != null)
                    {
                        tag = "015";
                        LogInfo(tag, DateTime.Now);
                        if (item.idWarehouse != null)
                        {

                            tag = "016";
                            LogInfo(tag, DateTime.Now);
                            var entityValuePermissions = entityPermissions
                                                                .listValue?
                                                                .FirstOrDefault(fod2 => fod2?.id_entityValue == item.idWarehouse
                                                                                        && fod2?.listPermissions?.FirstOrDefault(fod3 => fod3.name == "Editar") != null);

                            tag = "017";
                            LogInfo(tag, DateTime.Now);
                            if (entityValuePermissions == null)
                            {
                                throw new ProdHandlerException("No tiene Permiso para editar y guardar el movimiento de inventario.");
                            }
                        }

                        if (approve)
                        {
                            var _inventoryDetailPeriod = db.InventoryPeriodDetail.Where(a => a.AdvanceParametersDetail.valueCode == "A" && a.InventoryPeriod.isActive).ToList();
                            var _emissionDateInventoryMove = item.Document.emissionDate;
                            var añoEmission = _emissionDateInventoryMove.Year;
                            var mesEmission = _emissionDateInventoryMove.Month;

                            var _inventoryPeriodActivo = _inventoryDetailPeriod?.Any(e => e.InventoryPeriod.year == añoEmission
                                                                                          && e.dateInit.Month == mesEmission
                                                                                          && e.InventoryPeriod.id_warehouse == item.idWarehouse);

                            var _bodega = db.Warehouse.FirstOrDefault(a => a.id == item.idWarehouse);

                            if (!(_inventoryPeriodActivo ?? false))
                            {
                                throw new ProdHandlerException("No existe periodo de inventario abierto para la " + _bodega?.name);
                            }

                            if (item.idWarehouseEntry != null)
                            {
                                var _inventoryPeriodActivoBodegaEntrada = _inventoryDetailPeriod?.Any(e => e.InventoryPeriod.year == añoEmission
                                                                                                           && e.dateInit.Month == mesEmission
                                                                                                           && e.InventoryPeriod.id_warehouse == item.idWarehouseEntry);

                                var _bodegaEntrada = db.Warehouse.FirstOrDefault(a => a.id == item.idWarehouseEntry);

                                if (!(_inventoryPeriodActivoBodegaEntrada ?? false))
                                {
                                    throw new ProdHandlerException($"No existe periodo de inventario abierto para la {_bodegaEntrada?.name}");
                                }
                            }

                            if (item.idWarehouse != null)
                            {
                                var entityValuePermissions = entityPermissions
                                                                .listValue?
                                                                .FirstOrDefault(fod2 => fod2.id_entityValue == item.idWarehouse
                                                                                        && fod2.listPermissions?.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new ProdHandlerException("No tiene Permiso para editar y guardar el movimiento de inventario.");
                                }
                            }
                            InvoiceDetailValid[] InvoiceDetailValid = new InvoiceDetailValid[] { };
                            var cantidadCartinesFactura = 0;
                            var cantidadCartonesMovimiento = 0.00m;
                            if (tempInventoryMove.id_Invoice.HasValue)
                            {
                                InvoiceExterior invoiceExterior = db.InvoiceExterior.FirstOrDefault(a => a.id == tempInventoryMove.id_Invoice.Value);
                                if (invoiceExterior == null)
                                {
                                    throw new ProdHandlerException("No existe la Factura ingresada");
                                }

                                InvoiceDetailValid = invoiceExterior
                                                            .Invoice?
                                                            .InvoiceDetail?
                                                            .Where(e => e.id_invoice == tempInventoryMove.id_Invoice
                                                                        && e.isActive)?
                                                            .Select(e => new InvoiceDetailValid()
                                                            {
                                                                IdItem = e.id_item,
                                                                Cantidad = e.numBoxes ?? 0,
                                                            })?
                                                            .ToArray() ?? Array.Empty<InventoryMoveController.InvoiceDetailValid>();

                                var inventoryMoveDetailAux = item
                                                                .InventoryMoveDetail?
                                                                .GroupBy(a => a.id_item)?
                                                                .Select(r => new
                                                                {
                                                                    id_item = r.Key,
                                                                    cantidad = r.Sum(rr => rr.amountMove)
                                                                })?
                                                                .ToList();

                                foreach (var inventoryMoveDetail in inventoryMoveDetailAux)
                                {
                                    foreach (var InvoiceDetail in InvoiceDetailValid)
                                    {
                                        if (InvoiceDetail.IdItem == inventoryMoveDetail.id_item && InvoiceDetail.Cantidad != inventoryMoveDetail.cantidad)
                                        {
                                            var itemMasterCode = db.Item.FirstOrDefault(a => a.id == inventoryMoveDetail.id_item).masterCode;
                                            throw new ProdHandlerException("La cantidad del producto " + itemMasterCode + " debe ser igual a " + InvoiceDetail.Cantidad + " indicado en la factura.");
                                        }
                                    }
                                }

                                cantidadCartinesFactura = (invoiceExterior != null) ? invoiceExterior.totalBoxes : 0;
                                cantidadCartonesMovimiento = (inventoryMoveDetailAux != null) ? inventoryMoveDetailAux.Sum(a => a.cantidad.Value) : 0;
                                if (cantidadCartinesFactura != cantidadCartonesMovimiento)
                                {
                                    throw new ProdHandlerException("La cantidad total del movimiento, debe ser igual a " + cantidadCartinesFactura + " indicado en la factura.");
                                }
                            }

                        }
                    }
                }

                tag = "018";
                LogInfo(tag, DateTime.Now);
                if (item.InventoryMoveDetail.Count == 0)
                {
                    throw new ProdHandlerException("No se puede guardar un movimiento de inventario sin detalles.");
                }

                tag = "019";
                LogInfo(tag, DateTime.Now);
                var lotReceptionDatePar = db.Setting.FirstOrDefault(fod => fod.code == "VALLOT")?.value ?? "NO";

                tag = "020";
                LogInfo(tag, DateTime.Now);
                if (lotReceptionDatePar == "SI" && codeDocumentType != "34")
                {

                    tag = "021";
                    LogInfo(tag, DateTime.Now);
                    var fechaEmisionM = item.Document.emissionDate.Date;


                    tag = "022";
                    LogInfo(tag, DateTime.Now);
                    foreach (var itemDetail in item.InventoryMoveDetail)
                    {

                        tag = "023";
                        LogInfo(tag, DateTime.Now);
                        var fechaRecpecion = itemDetail?.Lot?.ProductionLot?.receptionDate ?? itemDetail?.Lot?.Document?.emissionDate ?? null;
                        var internalNumber = itemDetail?.Lot?.ProductionLot?.internalNumber ?? itemDetail?.Lot?.internalNumber ?? null;

                        tag = "024";
                        LogInfo(tag, DateTime.Now);
                        if (!fechaRecpecion.HasValue)
                        {
                            throw new ProdHandlerException($"El lote {internalNumber} no tiene fecha.");
                        }

                        tag = "025";
                        LogInfo(tag, DateTime.Now);
                        if (fechaRecpecion.Value.Date < fechaEmisionM)
                        {
                            throw new ProdHandlerException($"La fecha de Recepción del lote {internalNumber} es menor a la fecha del movimiento.");
                        }
                    }
                }

                tag = "026";
                LogInfo(tag, DateTime.Now);
                var inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_costCenter == null);
                if (inventoryMoveDetailAux2 != null)
                {
                    throw new ProdHandlerException("No se puede guardar el movimiento de inventario sin centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }

                tag = "027";
                LogInfo(tag, DateTime.Now);
                inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_subCostCenter == null);
                if (inventoryMoveDetailAux2 != null)
                {
                    throw new ProdHandlerException("No se puede guardar el movimiento de inventario sin sub-centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }

                #endregion RA  | Optimizacion Fx Inventario -000

                #region  RA  | Optimizacion Fx Inventario - 001

                if (natureMoveIMTmp.Trim().Equals("I"))
                {
                     
                    if (codeDocumentType == "34")
                    {
                        prepareResults = ServiceInventoryMove.UpdateInventaryMoveTransferEntryValidateOP(
                                                                approve,
                                                                ActiveUser,
                                                                ActiveCompany,
                                                                ActiveEmissionPoint,
                                                                item,
                                                                db,
                                                                false,
                                                                warehouses: warehouses,
                                                                warehouseLocations: warehouseLocations);
                        if (!string.IsNullOrEmpty(prepareResults?.message)) throw new ProdHandlerException(prepareResults.message);

                    }
                }
                 
                #endregion RA  | Optimizacion Fx Inventario - 001

                isValid = true;
            }
            catch (ProdHandlerException e)
            {
                item = tempInventoryMove;
                ViewData["ErrorMessage"] = $"{(e.Message ?? "")}";
                if (string.IsNullOrEmpty(e.Message))
                {
                    MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "InventoryMove", "Produccion", seccion: tag);
                }
            }
            catch (Exception e)
            {
                item = tempInventoryMove;
                ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "InventoryMove", "Produccion", seccion: tag);
            }
            finally
            {
                TempData.Keep("inventoryMove");
            }
            if (isValid)
            {
                if (natureMoveIMTmp.Trim().Equals("I") &&  codeDocumentType == "34")
                {
                    try
                    {

                        #region Update Gen. Sec. Trans.
                        tag = "028";
                        LogInfo(tag, DateTime.Now);
                        InventoryReason inventoryReasonAux = db.InventoryReason.FirstOrDefault(i => i.id == item.id_inventoryReason);

                        tag = "029";
                        LogInfo(tag, DateTime.Now);
                        if (inventoryReasonAux == null)
                        {
                            throw new ProdHandlerException("No se ha configurado el motivo de inventario seleccionado");
                        }

                        tag = "030";
                        LogInfo(tag, DateTime.Now);
                        ViewBag.withLotSystem = inventoryReasonAux.requiereSystemLotNubmber ?? false;
                        ViewBag.withLotCustomer = inventoryReasonAux.requiereUserLotNubmber ?? false;

                        tag = "031";
                        LogInfo(tag, DateTime.Now);
                        var inventoryMoveAux = db.InventoryMove.FirstOrDefault(fod => fod.id == item.id);

                        tag = "032";
                        LogInfo(tag, DateTime.Now);
                        foreach (var itemDetailAux in tempInventoryMove.InventoryMoveDetail.Where(x => x.genSecTrans))
                        {
                            tag = "033";
                            LogInfo(tag, DateTime.Now);
                            // if (itemDetailAux.genSecTrans)
                            //{
                            tag = "034";
                            LogInfo(tag, DateTime.Now);
                            var sequentialAux = inventoryReasonAux?.sequential;
                            var secTransAux = (inventoryReasonAux?.code ?? "") + sequentialAux?.ToString("D9") ?? "";
                            Lot lotAux = null;

                            tag = "035";
                            LogInfo(tag, DateTime.Now);
                            var inventoryMoveDetailAux = inventoryMoveAux?.InventoryMoveDetail?.FirstOrDefault(fod => fod.id == itemDetailAux.id);
                            if (inventoryMoveDetailAux != null)
                            {
                                tag = "036";
                                LogInfo(tag, DateTime.Now);
                                if (inventoryMoveDetailAux.genSecTrans)
                                {
                                    tag = "037";
                                    LogInfo(tag, DateTime.Now);
                                    lotAux = getLotSinTransaccion(inventoryMoveDetailAux.Lot?.number, itemDetailAux.Lot?.internalNumber);
                                    itemDetailAux.id_lot = lotAux?.id;
                                    itemDetailAux.Lot = lotAux;
                                }
                                else
                                {
                                    tag = "038";
                                    LogInfo(tag, DateTime.Now);
                                    lotAux = getLotSinTransaccion(secTransAux, itemDetailAux.Lot?.internalNumber);
                                    itemDetailAux.id_lot = lotAux?.id;
                                    itemDetailAux.Lot = lotAux;

                                    tag = "039";
                                    LogInfo(tag, DateTime.Now);
                                    inventoryReasonAux.sequential++;
                                    db.InventoryReason.Attach(inventoryReasonAux);
                                    db.Entry(inventoryReasonAux).State = EntityState.Modified;
                                }
                            }
                            else
                            {
                                tag = "040";
                                LogInfo(tag, DateTime.Now);
                                lotAux = getLotSinTransaccion(secTransAux, itemDetailAux.Lot?.internalNumber);
                                itemDetailAux.id_lot = lotAux?.id;
                                itemDetailAux.Lot = lotAux;


                                tag = "041";
                                LogInfo(tag, DateTime.Now);
                                inventoryReasonAux.sequential++;
                                db.InventoryReason.Attach(inventoryReasonAux);
                                db.Entry(inventoryReasonAux).State = EntityState.Modified;
                            }
                            //}
                        }

                        #endregion Update Gen. Sec. Trans.

                        tag = "042";
                        LogInfo(tag, DateTime.Now);
                        tempInventoryMove.InventoryEntryMove = entryMove;
                        tempInventoryMove.InventoryExitMove = exitMove;

                        tag = "043";
                        LogInfo(tag, DateTime.Now);
                        item.idNatureMove = inventoryReasons.FirstOrDefault(fod => fod.id == item.id_inventoryReason)?.idNatureMove;

                        ServiceInventoryMove.ServiceInventoryMoveAux result = null;

                        if (approve)
                        {
                            result = ServiceInventoryMove.UpdateInventaryMoveVirtualCompanyExitOP(                                                                                                        
                                                            approve, 
                                                            ActiveUser, 
                                                            ActiveCompany, 
                                                            ActiveEmissionPoint,
                                                            item, 
                                                            db, 
                                                            false, 
                                                            null,
                                                            serviceInventoryMoveAux: prepareResults,
                                                            documentStates: documentStates,
                                                            emissionPoints: emissionPoints,
                                                            inventoryReasons: inventoryReasons,
                                                            settings: settings,
                                                            warehouses: warehouses,
                                                            warehouseLocations: warehouseLocations,
                                                            employees: employees,
                                                            metricUnits: metricUnits,
                                                            metricUnitConversions: metricUnitConversions);
                            if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                            inventoryMoveDetailIdsForDelete = result.inventoryMoveDetailIdsForDelete;


                        }

                        result = ServiceInventoryMove.UpdateInventaryMoveTransferEntryOP
                                            (
                                            connection,
                                            approve,
                                            ActiveUser,
                                            ActiveCompany,
                                            ActiveEmissionPoint,
                                            item,
                                            db,
                                            false,
                                            serviceInventoryMoveAux: prepareResults,
                                            inventaryMoveVirtualCompanyExit: result?.inventoryMove,
                                            documentStates: documentStates,
                                            emissionPoints: emissionPoints,
                                            inventoryReasons: inventoryReasons,
                                            employees: employees,
                                            warehouses: warehouses,
                                            warehouseLocations: warehouseLocations,
                                            metricUnits: metricUnits,
                                            metricUnitConversions: metricUnitConversions,
                                            settings: settings);
                        if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);


                        item = result?.inventoryMove;
                        if (item.id == 0)
                        {
                            tag = "048";
                            LogInfo(tag, DateTime.Now);
                            db.InventoryMove.Add(item);
                        }
                        else
                        {
                            tag = "049";
                            LogInfo(tag, DateTime.Now);
                            // db.InventoryMove.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        using (var trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                tag= "049-01";
                                LogInfo(tag, DateTime.Now);
                                db.SaveChanges();
                                trans.Commit();
                                isValidTransacction = true;

                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                MetodosEscrituraLogs.EscribeExcepcionLogNest(ex, rutaLog, "InventoryMove", "Producion", seccion: tag);
                                throw;
                            }
                        }

                        tag = "049-02-predel";
                        LogInfo(tag, DateTime.Now);
                        if (isValidTransacction)
                        {
                            if (codeDocumentType == "32" && approve && ((inventoryMoveDetailIdsForDelete?.Length ?? 0)) > 0)
                            {

                                Task.Run(async () =>
                                {
                                    await ServiceTransCtl.DeleteBulkSp(inventoryMoveDetailIdsForDelete, this.GetType().Name, (item?.id ?? 0));
                                });

                            }

                        }

                        ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + item.Document.number + " guardado exitosamente");

                        TempData["inventoryMove"] = item;
                        TempData.Keep("inventoryMove");

                        tag = "052";
                        LogInfo(tag, DateTime.Now);
                        ViewData["numberSeqWarehouse"] = item.natureSequential;

                        tag = "053";
                        LogInfo(tag, DateTime.Now);
                        item.FillDocumentSourceInformation();
                        ViewData["numberDocument"] = item?.sequential?.ToString();

                        ViewData["numberSeqWarehouse"] = item?.natureSequential;
                        ViewData["fechaEmision"] = item?.Document?.emissionDate;

                        tag = "054";
                        LogInfo(tag, DateTime.Now);
                        this.ConvertToDTO(item?.InventoryMoveDetail, item?.Document?.emissionDate);


                    }
                    catch (ProdHandlerException e)
                    {

                        ViewData["ErrorMessage"] = $"{(e.Message ?? "")}";
                        item = tempInventoryMove;
                        if (string.IsNullOrEmpty(e.Message))
                        {
                            MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "InventoryMoveController", "Producion", seccion: tag);
                        }
                    }
                    catch (Exception e)
                    {

                        ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                        item = tempInventoryMove;
                        MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "InventoryMoveController", "Producion", seccion: tag);
                        //if (multiple) throw new Exception(e.Message);
                    }
                    finally
                    {
                        TempData.Keep("inventoryMove");
                    }


                }
            }
            return PartialView("_InventoryMoveMainFormPartial", item);

        }

        private ActionResult InventoryMovesPartialExitAddNew(bool approve, string codeDocumentType,
            string natureMoveIMTmp, InventoryMove item, Document document, InventoryEntryMove entryMove, InventoryExitMove exitMove,
            bool multiple, string customParamOP = null)
        {


            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            InventoryMoveExitPackingMaterial _inventoryMoveExitPackingMaterial = new InventoryMoveExitPackingMaterial();

            int? idProductionLot = null;
            bool isValid = false;
            bool isValidTransacction = false;
            bool isNew = false;

            List<Item> itemList = new List<Item>();
            Setting[] settings = Array.Empty<Setting>();
            Warehouse[] warehouses = Array.Empty<Warehouse>();
            WarehouseLocation[] warehouseLocations = Array.Empty<WarehouseLocation>();
            DocumentState[] documentStates = Array.Empty<DocumentState>();
            EmissionPoint[] emissionPoints = Array.Empty<EmissionPoint>();
            InventoryReason[] inventoryReasons = Array.Empty<InventoryReason>();
            Employee[] employees = Array.Empty<Employee>();
            MetricUnit[] metricUnits = Array.Empty<MetricUnit>();
            MetricUnitConversion[] metricUnitConversions = Array.Empty<MetricUnitConversion>();
            DocumentType[] documentTypes = Array.Empty<DocumentType>();

            #region Informacion Log
            string identificador = (item.id != 0) ? item.id.ToString() : Guid.NewGuid().ToString();
            LogInfo($"ini-{codeDocumentType}-{identificador}-Aprovee:{approve}", DateTime.Now);
            #endregion

            string tag = "";
            ServiceInventoryMove.ServiceInventoryMoveAux prepareResults = null;
            IDbConnection connection = null;

            int[] inventoryMoveDetailIdsForDelete = Array.Empty<int>();
            try
            {
                LogInfo($"val-{identificador}", DateTime.Now);
                #region Optimiza Codigo


                settings = db.Setting.ToArray();
                warehouses = db.Warehouse.ToArray();
                warehouseLocations = db.WarehouseLocation.ToArray();
                documentStates = db.DocumentState.ToArray();
                emissionPoints = db.EmissionPoint.ToArray();
                inventoryReasons = db.InventoryReason.ToArray();
                employees = db.Employee.ToArray();
                metricUnits = db.MetricUnit.ToArray();
                metricUnitConversions = db.MetricUnitConversion.ToArray();
                documentTypes = db.DocumentType.ToArray();
                #endregion

                #region RA | Optimizacion Fx Inventario -000 
                if (tempInventoryMove == null)
                {
                    throw new ProdHandlerException("No existen datos para módelo de Movimiento de Inventario desde el cache");
                }
                if (item == null)
                {
                    throw new ProdHandlerException("No existen datos para módelo de Movimiento de Inventario");
                }
                int[] ids = tempInventoryMove.InventoryMoveDetail.Select(r => r.id_item).ToArray();
                itemList = db.Item.Where(it => it.isActive && ids.Contains(it.id)).ToList();

                tempInventoryMove.idWarehouse = item.idWarehouse;
                tempInventoryMove.id_inventoryReason = item.id_inventoryReason;
                if (inventoryReasons == null)
                {
                    throw new ProdHandlerException("No existen datos en el listado de Motivos de Inventario");
                }
                if (item.id_inventoryReason == null)
                {
                    throw new ProdHandlerException("Motivo de inventario no definido");
                }
                tempInventoryMove.InventoryReason = inventoryReasons.FirstOrDefault(fod => fod.id == item.id_inventoryReason);
                tempInventoryMove.idWarehouseEntry = item.idWarehouseEntry;
                tempInventoryMove.id_provider = item.id_provider;
                tempInventoryMove.id_costCenter = item.id_costCenter;
                tempInventoryMove.id_subCostCenter = item.id_subCostCenter;
                tempInventoryMove.id_customer = item.id_customer;
                tempInventoryMove.id_seller = item.id_seller;
                tempInventoryMove.noFactura = item.noFactura;
                tempInventoryMove.id_Invoice = item.id_Invoice;
                tempInventoryMove.contenedor = item.contenedor;
                tempInventoryMove.numberRemGuide = item.numberRemGuide;

                if (tempInventoryMove.Document == null || document == null)
                {
                    throw new ProdHandlerException("No existe modelo de Documento");
                }

                tempInventoryMove.Document.description = document.description;

                if (customParamOP == "IPXM")
                {
                    _inventoryMoveExitPackingMaterial = tempInventoryMove.InventoryMoveExitPackingMaterial;
                    idProductionLot = item.id_productionLot;
                }

                ViewData["_natureMove"] = natureMoveIMTmp?.Trim();
                ViewData["_customParamOP"] = customParamOP?.Trim();

                if (customParamOP == "IPXM")
                {
                    _inventoryMoveExitPackingMaterial = tempInventoryMove.InventoryMoveExitPackingMaterial;
                    Item _itemMaster;
                    idProductionLot = item.id_productionLot;
                    if (_inventoryMoveExitPackingMaterial != null)
                    {
                        _itemMaster = db.Item.FirstOrDefault(r => r.id == _inventoryMoveExitPackingMaterial.id_ItemMaster);
                        if (_itemMaster != null)
                        {
                            string _description = document.description;
                            string _newDescription = " Producto: " + _itemMaster.masterCode + " | " + _itemMaster.description + Environment.NewLine
                                                                    + "Cantidad dosificada: " + _inventoryMoveExitPackingMaterial.quantityExit;

                            if (_description == null)
                            {
                                document.description = _newDescription;
                            }
                            else
                            {
                                string splitDescritionOri = Regex.Replace(_description, @"\t|\n|\r", "");

                                string[] splitDescription = splitDescritionOri.Split(new string[] { " Producto:" }, StringSplitOptions.None).ToArray();

                                if (splitDescription.Count() >= 2)
                                {
                                    document.description = splitDescription[0] + Environment.NewLine + _newDescription;
                                }
                                else
                                {
                                    document.description = document.description + Environment.NewLine + _newDescription;
                                }
                            }
                        }
                    }
                }

                tempInventoryMove.Document.description = document.description != null ? document.description : "";

                item.Document = document;
                item.InventoryEntryMove = entryMove;
                item.InventoryExitMove = exitMove;

                item.InventoryMoveDetail = tempInventoryMove.InventoryMoveDetail;

                var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                if (entityObjectPermissions != null)
                {

                    var entityPermissions = entityObjectPermissions.listEntityPermissions?.FirstOrDefault(fod => fod.codeEntity == "WAH");


                    if (entityPermissions != null)
                    {

                        if (item.idWarehouse != null)
                        {

                            var entityValuePermissions = entityPermissions
                                                                .listValue?
                                                                .FirstOrDefault(fod2 => fod2?.id_entityValue == item.idWarehouse
                                                                                        && fod2?.listPermissions?.FirstOrDefault(fod3 => fod3.name == "Editar") != null);

                            if (entityValuePermissions == null)
                            {
                                throw new ProdHandlerException("No tiene Permiso para editar y guardar el movimiento de inventario.");
                            }
                        }

                        if (approve)
                        {
                            var _inventoryDetailPeriod = db.InventoryPeriodDetail.Where(a => a.AdvanceParametersDetail.valueCode == "A" && a.InventoryPeriod.isActive).ToList();
                            var _emissionDateInventoryMove = item.Document.emissionDate;
                            var añoEmission = _emissionDateInventoryMove.Year;
                            var mesEmission = _emissionDateInventoryMove.Month;

                            var _inventoryPeriodActivo = _inventoryDetailPeriod?.Any(e => e.InventoryPeriod.year == añoEmission
                                                                                          && e.dateInit.Month == mesEmission
                                                                                          && e.InventoryPeriod.id_warehouse == item.idWarehouse);

                            var _bodega = db.Warehouse.FirstOrDefault(a => a.id == item.idWarehouse);

                            if (!(_inventoryPeriodActivo ?? false))
                            {
                                throw new ProdHandlerException("No existe periodo de inventario abierto para la " + _bodega?.name);
                            }

                            if (item.idWarehouseEntry != null)
                            {
                                var _inventoryPeriodActivoBodegaEntrada = _inventoryDetailPeriod?.Any(e => e.InventoryPeriod.year == añoEmission
                                                                                                           && e.dateInit.Month == mesEmission
                                                                                                           && e.InventoryPeriod.id_warehouse == item.idWarehouseEntry);

                                var _bodegaEntrada = db.Warehouse.FirstOrDefault(a => a.id == item.idWarehouseEntry);

                                if (!(_inventoryPeriodActivoBodegaEntrada ?? false))
                                {
                                    throw new ProdHandlerException($"No existe periodo de inventario abierto para la {_bodegaEntrada?.name}");
                                }
                            }

                            if (item.idWarehouse != null)
                            {
                                var entityValuePermissions = entityPermissions
                                                                .listValue?
                                                                .FirstOrDefault(fod2 => fod2.id_entityValue == item.idWarehouse
                                                                                        && fod2.listPermissions?.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new ProdHandlerException("No tiene Permiso para editar y guardar el movimiento de inventario.");
                                }
                            }
                            InvoiceDetailValid[] InvoiceDetailValid = new InvoiceDetailValid[] { };
                            var cantidadCartinesFactura = 0;
                            var cantidadCartonesMovimiento = 0.00m;
                            if (tempInventoryMove.id_Invoice.HasValue)
                            {
                                InvoiceExterior invoiceExterior = db.InvoiceExterior.FirstOrDefault(a => a.id == tempInventoryMove.id_Invoice.Value);
                                if (invoiceExterior == null)
                                {
                                    throw new ProdHandlerException("No existe la Factura ingresada");
                                }

                                InvoiceDetailValid = invoiceExterior
                                                            .Invoice?
                                                            .InvoiceDetail?
                                                            .Where(e => e.id_invoice == tempInventoryMove.id_Invoice
                                                                        && e.isActive)?
                                                            .Select(e => new InvoiceDetailValid()
                                                            {
                                                                IdItem = e.id_item,
                                                                Cantidad = e.numBoxes ?? 0,
                                                            })?
                                                            .ToArray() ?? Array.Empty<InventoryMoveController.InvoiceDetailValid>();

                                var inventoryMoveDetailAux = item
                                                                .InventoryMoveDetail?
                                                                .GroupBy(a => a.id_item)?
                                                                .Select(r => new
                                                                {
                                                                    id_item = r.Key,
                                                                    cantidad = r.Sum(rr => rr.amountMove)
                                                                })?
                                                                .ToList();

                                foreach (var inventoryMoveDetail in inventoryMoveDetailAux)
                                {
                                    foreach (var InvoiceDetail in InvoiceDetailValid)
                                    {
                                        if (InvoiceDetail.IdItem == inventoryMoveDetail.id_item && InvoiceDetail.Cantidad != inventoryMoveDetail.cantidad)
                                        {
                                            var itemMasterCode = db.Item.FirstOrDefault(a => a.id == inventoryMoveDetail.id_item).masterCode;
                                            throw new ProdHandlerException("La cantidad del producto " + itemMasterCode + " debe ser igual a " + InvoiceDetail.Cantidad + " indicado en la factura.");
                                        }
                                    }
                                }

                                cantidadCartinesFactura = (invoiceExterior != null) ? invoiceExterior.totalBoxes : 0;
                                cantidadCartonesMovimiento = (inventoryMoveDetailAux != null) ? inventoryMoveDetailAux.Sum(a => a.cantidad.Value) : 0;
                                if (cantidadCartinesFactura != cantidadCartonesMovimiento)
                                {
                                    throw new ProdHandlerException("La cantidad total del movimiento, debe ser igual a " + cantidadCartinesFactura + " indicado en la factura.");
                                }
                            }

                        }
                    }
                }


                if (item.InventoryMoveDetail.Count == 0)
                {
                    throw new ProdHandlerException("No se puede guardar un movimiento de inventario sin detalles.");
                }

                var lotReceptionDatePar = db.Setting.FirstOrDefault(fod => fod.code == "VALLOT")?.value ?? "NO";

                if (lotReceptionDatePar == "SI" && codeDocumentType != "34")
                {

                    var fechaEmisionM = item.Document.emissionDate.Date;

                    foreach (var itemDetail in item.InventoryMoveDetail)
                    {

                        var fechaRecpecion = itemDetail?.Lot?.ProductionLot?.receptionDate ?? itemDetail?.Lot?.Document?.emissionDate ?? null;
                        var internalNumber = itemDetail?.Lot?.ProductionLot?.internalNumber ?? itemDetail?.Lot?.internalNumber ?? null;


                        if (!fechaRecpecion.HasValue)
                        {
                            throw new ProdHandlerException($"El lote {internalNumber} no tiene fecha.");
                        }

                        if (fechaRecpecion.Value.Date < fechaEmisionM)
                        {
                            throw new ProdHandlerException($"La fecha de Recepción del lote {internalNumber} es menor a la fecha del movimiento.");
                        }
                    }
                }

                var inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_costCenter == null);
                if (inventoryMoveDetailAux2 != null)
                {
                    throw new ProdHandlerException("No se puede guardar el movimiento de inventario sin centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }

                inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_subCostCenter == null);
                if (inventoryMoveDetailAux2 != null)
                {
                    throw new ProdHandlerException("No se puede guardar el movimiento de inventario sin sub-centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }

                #endregion RA  | Optimizacion Fx Inventario -000

                isValid = true;
            }
            catch (ProdHandlerException e)
            {
                item = tempInventoryMove;
                ViewData["ErrorMessage"] = $"{(e.Message ?? "")}";
                if (string.IsNullOrEmpty(e.Message))
                {
                    FullLog(e, seccion: identificador);
                }
            }
            catch (Exception e)
            {
                item = tempInventoryMove;
                ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                FullLog(e, seccion: identificador);

            }
            finally
            {
                TempData.Keep("inventoryMove");
            }

            if (isValid)
            {
                try
                {
                    LogInfo($"tran-{identificador}", DateTime.Now);
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {

                            #region Update Gen. Sec. Trans.                            
                            InventoryReason inventoryReasonAux = db.InventoryReason.FirstOrDefault(i => i.id == item.id_inventoryReason);

                            if (inventoryReasonAux == null)
                            {
                                throw new ProdHandlerException("No se ha configurado el motivo de inventario seleccionado");
                            }

                            ViewBag.withLotSystem = inventoryReasonAux.requiereSystemLotNubmber ?? false;
                            ViewBag.withLotCustomer = inventoryReasonAux.requiereUserLotNubmber ?? false;


                            var inventoryMoveAux = db.InventoryMove.FirstOrDefault(fod => fod.id == item.id);

                            LogInfo($"tran-lote-{identificador}", DateTime.Now);
                            foreach (var itemDetailAux in tempInventoryMove.InventoryMoveDetail.Where(x => x.genSecTrans))
                            {

                                var sequentialAux = inventoryReasonAux?.sequential;
                                var secTransAux = (inventoryReasonAux?.code ?? "") + sequentialAux?.ToString("D9") ?? "";
                                Lot lotAux = null;


                                var inventoryMoveDetailAux = inventoryMoveAux?.InventoryMoveDetail?.FirstOrDefault(fod => fod.id == itemDetailAux.id);
                                if (inventoryMoveDetailAux != null)
                                {

                                    if (inventoryMoveDetailAux.genSecTrans)
                                    {
                                        lotAux = getLotSinTransaccion(inventoryMoveDetailAux.Lot?.number, itemDetailAux.Lot?.internalNumber);
                                        itemDetailAux.id_lot = lotAux?.id;
                                        itemDetailAux.Lot = lotAux;
                                    }
                                    else
                                    {

                                        lotAux = getLotSinTransaccion(secTransAux, itemDetailAux.Lot?.internalNumber);
                                        itemDetailAux.id_lot = lotAux?.id;
                                        itemDetailAux.Lot = lotAux;

                                        inventoryReasonAux.sequential++;
                                        db.InventoryReason.Attach(inventoryReasonAux);
                                        db.Entry(inventoryReasonAux).State = EntityState.Modified;
                                    }
                                }
                                else
                                {
                                    lotAux = getLotSinTransaccion(secTransAux, itemDetailAux.Lot?.internalNumber);
                                    itemDetailAux.id_lot = lotAux?.id;
                                    itemDetailAux.Lot = lotAux;

                                    inventoryReasonAux.sequential++;
                                    db.InventoryReason.Attach(inventoryReasonAux);
                                    db.Entry(inventoryReasonAux).State = EntityState.Modified;
                                }

                            }
                            #endregion Update Gen. Sec. Trans.

                            tempInventoryMove.InventoryEntryMove = entryMove;
                            tempInventoryMove.InventoryExitMove = exitMove;

                            item.idNatureMove = inventoryReasons.FirstOrDefault(fod => fod.id == item.id_inventoryReason)?.idNatureMove;

                            LogInfo($"tran-serv-{identificador}", DateTime.Now);
                            ServiceInventoryMove.ServiceInventoryMoveAux result = ServiceInventoryMove
                                                                                        .UpdateInventaryMoveExitOP(approve,
                                                                                                                    ActiveUser,
                                                                                                                    ActiveCompany,
                                                                                                                    ActiveEmissionPoint,
                                                                                                                    item,
                                                                                                                    db,
                                                                                                                    false,
                                                                                                                    warehouses,
                                                                                                                    documentStates,
                                                                                                                    emissionPoints,
                                                                                                                    inventoryReasons,
                                                                                                                    metricUnits,
                                                                                                                    documentTypes,
                                                                                                                    metricUnitConversions,
                                                                                                                    employees,
                                                                                                                    trans: trans,
                                                                                                                    identificador: identificador
                                                                                                                    );
                            if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                            inventoryMoveDetailIdsForDelete = result.inventoryMoveDetailIdsForDelete;
                            LogInfo($"tran-serv-end-{identificador}", DateTime.Now);

                            if (customParamOP == "IPXM")
                            {
                                if (result != null)
                                {
                                    if (result.inventoryMove != null)
                                    {
                                        string codeWareHouse = warehouses.FirstOrDefault(r => r.id == result.inventoryMove.idWarehouse)?.code;
                                        ViewData["_codeWareHouse"] = codeWareHouse;

                                        result.inventoryMove.id_productionLot = idProductionLot;

                                        InventoryMoveDetail _inventoryMoveDetail = result.inventoryMove.InventoryMoveDetail.First();

                                        if (_inventoryMoveDetail != null)
                                        {
                                            ProductionLot _productionLot = db.ProductionLot.FirstOrDefault(r => r.id == result.inventoryMove.id_productionLot);

                                            result.inventoryMove.idLotePackagingMaterials = result.inventoryMove.id_productionLot;
                                            result.inventoryMove.idWarehouseLocationPackagingMaterials = _inventoryMoveDetail.id_warehouseLocation;
                                            result.inventoryMove.nameProviderPackagingMaterials = _productionLot?.Provider?.Person?.fullname_businessName;
                                            result.inventoryMove.nameProductionUnitProviderPackagingMaterials = _productionLot?.ProductionUnitProvider?.name;
                                            result.inventoryMove.idCostCenterPackagingMaterials = _inventoryMoveDetail.id_costCenter;
                                            result.inventoryMove.idSubCostCenterPackagingMaterials = _inventoryMoveDetail.id_subCostCenter;
                                            result.inventoryMove.idItemPackagingMaterials = _inventoryMoveDetail.id_item;
                                            result.inventoryMove.nameItemDescriptionPackagingMaterials = _inventoryMoveDetail.Item?.description;
                                        }

                                        if (result.inventoryMove.id == 0)
                                        {
                                            result.inventoryMove.InventoryMoveExitPackingMaterial = _inventoryMoveExitPackingMaterial;
                                        }
                                        else
                                        {
                                            InventoryMoveExitPackingMaterial _inventoryMoveExitPackingMaterialResult = db.InventoryMoveExitPackingMaterial
                                                .FirstOrDefault(r => r.id_InventoryMove == item.id);

                                            if (_inventoryMoveExitPackingMaterialResult != null
                                                && _inventoryMoveExitPackingMaterialResult.id_ItemMaster != _inventoryMoveExitPackingMaterial?.id_ItemMaster)
                                            {
                                                db.InventoryMoveExitPackingMaterial.Attach(_inventoryMoveExitPackingMaterialResult);
                                                db.Entry(_inventoryMoveExitPackingMaterialResult).State = EntityState.Deleted;

                                                result.inventoryMove.InventoryMoveExitPackingMaterial = _inventoryMoveExitPackingMaterial;
                                            }
                                            else
                                            {
                                                if (_inventoryMoveExitPackingMaterialResult == null)
                                                {
                                                    result.inventoryMove.InventoryMoveExitPackingMaterial = _inventoryMoveExitPackingMaterial;
                                                }
                                                else
                                                {
                                                    result.inventoryMove.InventoryMoveExitPackingMaterial.quantityExit = _inventoryMoveExitPackingMaterial.quantityExit;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            item = result?.inventoryMove;

                            LogInfo($"tran-track-{identificador}", DateTime.Now);
                            if (item.id == 0)
                            {
                                db.InventoryMove.Add(item);
                            }
                            else
                            {
                                db.Entry(item).State = EntityState.Modified;
                            }

                            db.SaveChanges();
                            trans.Commit();
                            isValidTransacction = true;

                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            FullLog(ex, seccion: identificador);
                            throw;
                        }
                    }

                    if (isValidTransacction)
                    {

                        if ((inventoryMoveDetailIdsForDelete?.Length ?? 0) > 0)
                        {

                            LogInfo($"tran-has-delete-{identificador}", DateTime.Now);
                            Task.Run(async () =>
                            {
                                await ServiceTransCtl.DeleteBulkSp(inventoryMoveDetailIdsForDelete, this.GetType().Name, (item?.id ?? 0));
                            });

                        }

                    }

                    ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + item.Document.number + " guardado exitosamente");

                    TempData["inventoryMove"] = item;
                    TempData.Keep("inventoryMove");

                    ViewData["numberSeqWarehouse"] = item.natureSequential;

                    item.FillDocumentSourceInformation();
                    ViewData["numberDocument"] = item?.sequential?.ToString();

                    ViewData["numberSeqWarehouse"] = item?.natureSequential;
                    ViewData["fechaEmision"] = item?.Document?.emissionDate;

                    this.ConvertToDTO(item?.InventoryMoveDetail, item?.Document?.emissionDate);

                }
                catch (ProdHandlerException e)
                {

                    ViewData["ErrorMessage"] = (e.Message ?? "");
                    item = tempInventoryMove;
                    FullLog(e, seccion: identificador);
                }
                catch (Exception e)
                {

                    ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                    item = tempInventoryMove;
                    FullLog(e, seccion: identificador);

                    //if (multiple) throw new Exception(e.Message);
                }
                finally
                {
                    TempData.Keep("inventoryMove");
                }
            }

            LogInfo($"end-{identificador}", DateTime.Now);
            return PartialView("_InventoryMoveMainFormPartial", item);
        }
        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMovesPartialAddNew(bool approve, string codeDocumentType,
            string natureMoveIMTmp, InventoryMove item, Document document, InventoryEntryMove entryMove, InventoryExitMove exitMove,
            bool multiple, string customParamOP = null)
        {

            if (natureMoveIMTmp.Trim().Equals("I") && codeDocumentType == "34") return InventoryMovesPartialEntryTransferAddNew(approve, codeDocumentType, natureMoveIMTmp, item, document, entryMove, exitMove, multiple, customParamOP);

            if (natureMoveIMTmp.Trim().Equals("E") && codeDocumentType == "05") return InventoryMovesPartialExitAddNew(approve, codeDocumentType, natureMoveIMTmp, item, document, entryMove, exitMove, multiple, customParamOP);

            if (natureMoveIMTmp.Trim().Equals("E") && codeDocumentType == "32") return InventoryMovesPartialExitTransferAddNew(approve, codeDocumentType, natureMoveIMTmp, item, document, entryMove, exitMove, multiple, customParamOP);

            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            InventoryMoveExitPackingMaterial _inventoryMoveExitPackingMaterial = new InventoryMoveExitPackingMaterial();
            
            int? idProductionLot = null;
            bool isValid = false;
            bool isValidTransacction = false;
            bool isNew = false;

            
            List<Item> itemList = new List<Item>();
            //List<Warehouse> warehouses = new List<Warehouse>();
            //List<InventoryReason> inventoryReasons = new List<InventoryReason>();

            Setting[] settings = Array.Empty<Setting>();
            Warehouse[] warehouses = Array.Empty<Warehouse>();
            WarehouseLocation[] warehouseLocations = Array.Empty<WarehouseLocation>();
            DocumentState[] documentStates = Array.Empty<DocumentState>();
            EmissionPoint[] emissionPoints = Array.Empty<EmissionPoint>();
            InventoryReason[] inventoryReasons = Array.Empty<InventoryReason>();
            Employee[] employees = Array.Empty<Employee>();
            MetricUnit[] metricUnits = Array.Empty<MetricUnit>();
            MetricUnitConversion[] metricUnitConversions = Array.Empty<MetricUnitConversion>();
            DocumentType[] documentTypes = Array.Empty<DocumentType>();


            string tag = "";
            ServiceInventoryMove.ServiceInventoryMoveAux prepareResults = null;
            IDbConnection connection = null;

            int[] inventoryMoveDetailIdsForDelete = Array.Empty<int>();
            try
            {
                tag = "001";
                LogInfo(tag, DateTime.Now);
                #region Optimiza Codigo
                
                itemList = db.Item.Where(it => it.isActive).ToList();
                //warehouses = db.Warehouse.Where(w => w.isActive).ToList();
                //inventoryReasons = db.InventoryReason.Where(i => i.isActive).ToList();

                settings = db.Setting.ToArray();
                warehouses = db.Warehouse.ToArray();
                warehouseLocations = db.WarehouseLocation.ToArray();
                documentStates = db.DocumentState.ToArray();
                emissionPoints = db.EmissionPoint.ToArray();
                inventoryReasons = db.InventoryReason.ToArray();
                employees = db.Employee.ToArray();
                metricUnits = db.MetricUnit.ToArray();
                metricUnitConversions = db.MetricUnitConversion.ToArray();
                documentTypes = db.DocumentType.ToArray();
                #endregion

                tag = "002";
                LogInfo(tag, DateTime.Now);
                #region RA | Optimizacion Fx Inventario -000 
                if (tempInventoryMove == null)
                {
                    throw new ProdHandlerException("No existen datos para módelo de Movimiento de Inventario desde el cache");
                }
                if (item == null)
                {
                    throw new ProdHandlerException("No existen datos para módelo de Movimiento de Inventario");
                }
                tempInventoryMove.idWarehouse = item.idWarehouse;
                tempInventoryMove.id_inventoryReason = item.id_inventoryReason;
                if (inventoryReasons == null)
                {
                    throw new ProdHandlerException("No existen datos en el listado de Motivos de Inventario");
                }
                if (item.id_inventoryReason == null)
                {
                    throw new ProdHandlerException("Motivo de inventario no definido");
                }
                tempInventoryMove.InventoryReason = inventoryReasons.FirstOrDefault(fod => fod.id == item.id_inventoryReason);
                tempInventoryMove.idWarehouseEntry = item.idWarehouseEntry;
                tempInventoryMove.id_provider = item.id_provider;
                tempInventoryMove.id_costCenter = item.id_costCenter;
                tempInventoryMove.id_subCostCenter = item.id_subCostCenter;
                tempInventoryMove.id_customer = item.id_customer;
                tempInventoryMove.id_seller = item.id_seller;
                tempInventoryMove.noFactura = item.noFactura;
                tempInventoryMove.id_Invoice = item.id_Invoice;
                tempInventoryMove.contenedor = item.contenedor;
                tempInventoryMove.numberRemGuide = item.numberRemGuide;

                tag = "003";
                LogInfo(tag, DateTime.Now);
                if (tempInventoryMove.Document == null || document == null)
                {
                    throw new ProdHandlerException("No existe modelo de Documento");
                }
                tag = "004";
                LogInfo(tag, DateTime.Now);
                tempInventoryMove.Document.description = document.description;

                tag = "005";
                LogInfo(tag, DateTime.Now);
                if (customParamOP == "IPXM")
                {
                    _inventoryMoveExitPackingMaterial = tempInventoryMove.InventoryMoveExitPackingMaterial;
                    idProductionLot = item.id_productionLot;
                }

                tag = "006";
                LogInfo(tag, DateTime.Now);
                ViewData["_natureMove"] = natureMoveIMTmp?.Trim();
                ViewData["_customParamOP"] = customParamOP?.Trim();

                tag = "007";
                LogInfo(tag, DateTime.Now);
                if (customParamOP == "IPXM")
                {
                    _inventoryMoveExitPackingMaterial = tempInventoryMove.InventoryMoveExitPackingMaterial;
                    Item _itemMaster;
                    idProductionLot = item.id_productionLot;
                    if (_inventoryMoveExitPackingMaterial != null)
                    {
                        _itemMaster = db.Item.FirstOrDefault(r => r.id == _inventoryMoveExitPackingMaterial.id_ItemMaster);
                        if (_itemMaster != null)
                        {
                            string _description = document.description;
                            string _newDescription = " Producto: " + _itemMaster.masterCode + " | " + _itemMaster.description + Environment.NewLine
                                                                    + "Cantidad dosificada: " + _inventoryMoveExitPackingMaterial.quantityExit;

                            if (_description == null)
                            {
                                document.description = _newDescription;
                            }
                            else
                            {
                                string splitDescritionOri = Regex.Replace(_description, @"\t|\n|\r", "");

                                string[] splitDescription = splitDescritionOri.Split(new string[] { " Producto:" }, StringSplitOptions.None).ToArray();

                                if (splitDescription.Count() >= 2)
                                {
                                    document.description = splitDescription[0] + Environment.NewLine + _newDescription;
                                }
                                else
                                {
                                    document.description = document.description + Environment.NewLine + _newDescription;
                                }
                            }
                        }
                    }
                }

                tag = "008";
                LogInfo(tag, DateTime.Now);
                tempInventoryMove.Document.description = document.description != null ? document.description : "";

                tag = "009";
                LogInfo(tag, DateTime.Now);
                item.Document = document;
                item.InventoryEntryMove = entryMove;
                item.InventoryExitMove = exitMove;

                tag = "010";
                LogInfo(tag, DateTime.Now);
                item.InventoryMoveDetail = tempInventoryMove.InventoryMoveDetail;

                tag = "011";
                LogInfo(tag, DateTime.Now);
                var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                tag = "012";
                LogInfo(tag, DateTime.Now);
                if (entityObjectPermissions != null)
                {
                    tag = "013";
                    LogInfo(tag, DateTime.Now);
                    var entityPermissions = entityObjectPermissions.listEntityPermissions?.FirstOrDefault(fod => fod.codeEntity == "WAH");

                    tag = "014"; 
                    LogInfo(tag, DateTime.Now);
                    if (entityPermissions != null)
                    {
                        tag = "015";
                        LogInfo(tag, DateTime.Now);
                        if (item.idWarehouse != null)
                        {

                            tag = "016";
                            LogInfo(tag, DateTime.Now);
                            var entityValuePermissions = entityPermissions
                                                                .listValue?
                                                                .FirstOrDefault(fod2 => fod2?.id_entityValue == item.idWarehouse
                                                                                        && fod2?.listPermissions?.FirstOrDefault(fod3 => fod3.name == "Editar") != null);

                            tag = "017";
                            LogInfo(tag, DateTime.Now);
                            if (entityValuePermissions == null)
                            {
                                throw new ProdHandlerException("No tiene Permiso para editar y guardar el movimiento de inventario.");
                            }
                        }

                        if (approve)
                        {
                            var _inventoryDetailPeriod = db.InventoryPeriodDetail.Where(a => a.AdvanceParametersDetail.valueCode == "A" && a.InventoryPeriod.isActive).ToList();
                            var _emissionDateInventoryMove = item.Document.emissionDate;
                            var añoEmission = _emissionDateInventoryMove.Year;
                            var mesEmission = _emissionDateInventoryMove.Month;

                            var _inventoryPeriodActivo = _inventoryDetailPeriod?.Any(e => e.InventoryPeriod.year == añoEmission
                                                                                          && e.dateInit.Month == mesEmission
                                                                                          && e.InventoryPeriod.id_warehouse == item.idWarehouse);

                            var _bodega = db.Warehouse.FirstOrDefault(a => a.id == item.idWarehouse);

                            if (!(_inventoryPeriodActivo ?? false))
                            {
                                throw new ProdHandlerException("No existe periodo de inventario abierto para la " + _bodega?.name);
                            }

                            if (item.idWarehouseEntry != null)
                            {
                                var _inventoryPeriodActivoBodegaEntrada = _inventoryDetailPeriod?.Any(e => e.InventoryPeriod.year == añoEmission
                                                                                                           && e.dateInit.Month == mesEmission
                                                                                                           && e.InventoryPeriod.id_warehouse == item.idWarehouseEntry);

                                var _bodegaEntrada = db.Warehouse.FirstOrDefault(a => a.id == item.idWarehouseEntry);

                                if (!(_inventoryPeriodActivoBodegaEntrada ?? false))
                                {
                                    throw new ProdHandlerException($"No existe periodo de inventario abierto para la {_bodegaEntrada?.name}");
                                }
                            }

                            if (item.idWarehouse != null)
                            {
                                var entityValuePermissions = entityPermissions
                                                                .listValue?
                                                                .FirstOrDefault(fod2 => fod2.id_entityValue == item.idWarehouse
                                                                                        && fod2.listPermissions?.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new ProdHandlerException("No tiene Permiso para editar y guardar el movimiento de inventario.");
                                }
                            }
                            InvoiceDetailValid[] InvoiceDetailValid = new InvoiceDetailValid[] { };
                            var cantidadCartinesFactura = 0;
                            var cantidadCartonesMovimiento = 0.00m;
                            if (tempInventoryMove.id_Invoice.HasValue)
                            {
                                InvoiceExterior invoiceExterior = db.InvoiceExterior.FirstOrDefault(a => a.id == tempInventoryMove.id_Invoice.Value);
                                if (invoiceExterior == null)
                                {
                                    throw new ProdHandlerException("No existe la Factura ingresada");
                                }

                                InvoiceDetailValid = invoiceExterior
                                                            .Invoice?
                                                            .InvoiceDetail?
                                                            .Where(e => e.id_invoice == tempInventoryMove.id_Invoice
                                                                        && e.isActive)?
                                                            .Select(e => new InvoiceDetailValid()
                                                            {
                                                                IdItem = e.id_item,
                                                                Cantidad = e.numBoxes ?? 0,
                                                            })?
                                                            .ToArray() ?? Array.Empty<InventoryMoveController.InvoiceDetailValid>();

                                var inventoryMoveDetailAux = item
                                                                .InventoryMoveDetail?
                                                                .GroupBy(a => a.id_item)?
                                                                .Select(r => new
                                                                {
                                                                    id_item = r.Key,
                                                                    cantidad = r.Sum(rr => rr.amountMove)
                                                                })?
                                                                .ToList();

                                foreach (var inventoryMoveDetail in inventoryMoveDetailAux)
                                {
                                    foreach (var InvoiceDetail in InvoiceDetailValid)
                                    {
                                        if (InvoiceDetail.IdItem == inventoryMoveDetail.id_item && InvoiceDetail.Cantidad != inventoryMoveDetail.cantidad)
                                        {
                                            var itemMasterCode = db.Item.FirstOrDefault(a => a.id == inventoryMoveDetail.id_item).masterCode;
                                            throw new ProdHandlerException("La cantidad del producto " + itemMasterCode + " debe ser igual a " + InvoiceDetail.Cantidad + " indicado en la factura.");
                                        }
                                    }
                                }

                                cantidadCartinesFactura = (invoiceExterior != null) ? invoiceExterior.totalBoxes : 0;
                                cantidadCartonesMovimiento = (inventoryMoveDetailAux != null) ? inventoryMoveDetailAux.Sum(a => a.cantidad.Value) : 0;
                                if (cantidadCartinesFactura != cantidadCartonesMovimiento)
                                {
                                    throw new ProdHandlerException("La cantidad total del movimiento, debe ser igual a " + cantidadCartinesFactura + " indicado en la factura.");
                                }
                            }

                        }
                    }
                }

                tag = "018";
                LogInfo(tag, DateTime.Now);
                if (item.InventoryMoveDetail.Count == 0)
                {
                    throw new ProdHandlerException("No se puede guardar un movimiento de inventario sin detalles.");
                }

                tag = "019";
                LogInfo(tag, DateTime.Now);
                var lotReceptionDatePar = db.Setting.FirstOrDefault(fod => fod.code == "VALLOT")?.value ?? "NO";

                tag = "020";
                LogInfo(tag, DateTime.Now);
                if (lotReceptionDatePar == "SI" && codeDocumentType != "34")
                {

                    tag = "021";
                    LogInfo(tag, DateTime.Now);
                    var fechaEmisionM = item.Document.emissionDate.Date;


                    tag = "022";
                    LogInfo(tag, DateTime.Now);
                    foreach (var itemDetail in item.InventoryMoveDetail)
                    {

                        tag = "023";
                        LogInfo(tag, DateTime.Now);
                        var fechaRecpecion = itemDetail?.Lot?.ProductionLot?.receptionDate ?? itemDetail?.Lot?.Document?.emissionDate ?? null;
                        var internalNumber = itemDetail?.Lot?.ProductionLot?.internalNumber ?? itemDetail?.Lot?.internalNumber ?? null;

                        tag = "024";
                        LogInfo(tag, DateTime.Now);
                        if (!fechaRecpecion.HasValue)
                        {
                            throw new ProdHandlerException($"El lote {internalNumber} no tiene fecha.");
                        }

                        tag = "025";
                        LogInfo(tag, DateTime.Now);
                        if (fechaRecpecion.Value.Date < fechaEmisionM)
                        {
                            throw new ProdHandlerException($"La fecha de Recepción del lote {internalNumber} es menor a la fecha del movimiento.");
                        }
                    }
                }

                tag = "026";
                LogInfo(tag, DateTime.Now);
                var inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_costCenter == null);
                if (inventoryMoveDetailAux2 != null)
                {
                    throw new ProdHandlerException("No se puede guardar el movimiento de inventario sin centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }

                tag = "027";
                LogInfo(tag, DateTime.Now);
                inventoryMoveDetailAux2 = tempInventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_subCostCenter == null);
                if (inventoryMoveDetailAux2 != null)
                {
                    throw new ProdHandlerException("No se puede guardar el movimiento de inventario sin sub-centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }

                #endregion RA  | Optimizacion Fx Inventario -000

                #region  RA  | Optimizacion Fx Inventario - 001

                if (natureMoveIMTmp.Trim().Equals("I"))
                {
                    if (codeDocumentType == "03" || customParamOP == "IPXM")
                    {

                    }
                    if (codeDocumentType == "04")
                    {

                    }
                    if (codeDocumentType == "34")
                    {
                        prepareResults = ServiceInventoryMove.UpdateInventaryMoveTransferEntryValidateOP(
                                                                approve, 
                                                                ActiveUser, 
                                                                ActiveCompany, 
                                                                ActiveEmissionPoint, 
                                                                item, 
                                                                db, 
                                                                false,
                                                                warehouses: warehouses,
                                                                warehouseLocations: warehouseLocations);
                        if (!string.IsNullOrEmpty(prepareResults?.message)) throw new ProdHandlerException(prepareResults.message);
                        
                    }
                }
                else if (natureMoveIMTmp.Trim().Equals("E"))
                {
                    if (codeDocumentType == "05")
                    {

                    }
                    if (codeDocumentType == "32")
                    {
                        prepareResults = ServiceInventoryMove.ValidateUpdateInventaryMoveTransferExit(approve, ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                        if (!string.IsNullOrEmpty(prepareResults?.message)) throw new ProdHandlerException(prepareResults.message);
                        item = prepareResults.inventoryMove;
                        isNew = prepareResults.isNew;
                    }
                    if (codeDocumentType == "129")
                    {

                    }
                }
                else
                {
                    if (codeDocumentType == "129")
                    {

                    }
                }

                #endregion RA  | Optimizacion Fx Inventario - 001

                isValid = true;
            }
            catch (ProdHandlerException e)
            {
                item = tempInventoryMove;
                ViewData["ErrorMessage"] = $"{(e.Message ?? "")}";
                if (string.IsNullOrEmpty(e.Message))
                {
                    MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "InventoryMove", "Produccion", seccion: tag);
                }
            }
            catch (Exception e)
            {
                item = tempInventoryMove;
                ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "InventoryMove", "Produccion", seccion: tag);
            }
            finally
            {
                TempData.Keep("inventoryMove");
            }
            
            if (isValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    connection = trans.UnderlyingTransaction.Connection;
                    connection = db.Database.Connection;
                    try
                    {

                        #region Update Gen. Sec. Trans.
                        tag = "028";
                        LogInfo(tag, DateTime.Now);
                        InventoryReason inventoryReasonAux = db.InventoryReason.FirstOrDefault(i => i.id == item.id_inventoryReason);

                        tag = "029";
                        LogInfo(tag, DateTime.Now);
                        if (inventoryReasonAux == null)
                        {
                            throw new ProdHandlerException("No se ha configurado el motivo de inventario seleccionado");
                        }

                        tag = "030";
                        LogInfo(tag, DateTime.Now);
                        ViewBag.withLotSystem = inventoryReasonAux.requiereSystemLotNubmber ?? false;
                        ViewBag.withLotCustomer = inventoryReasonAux.requiereUserLotNubmber ?? false;

                        tag = "031";
                        LogInfo(tag, DateTime.Now);
                        var inventoryMoveAux = db.InventoryMove.FirstOrDefault(fod => fod.id == item.id);

                        tag = "032";
                        LogInfo(tag, DateTime.Now);
                        foreach (var itemDetailAux in tempInventoryMove.InventoryMoveDetail.Where(x => x.genSecTrans))
                        {
                            tag = "033";
                            LogInfo(tag, DateTime.Now);
                            // if (itemDetailAux.genSecTrans)
                            //{
                            tag = "034";
                            LogInfo(tag, DateTime.Now);
                            var sequentialAux = inventoryReasonAux?.sequential;
                            var secTransAux = (inventoryReasonAux?.code ?? "") + sequentialAux?.ToString("D9") ?? "";
                            Lot lotAux = null;

                            tag = "035";
                            LogInfo(tag, DateTime.Now);
                            var inventoryMoveDetailAux = inventoryMoveAux?.InventoryMoveDetail?.FirstOrDefault(fod => fod.id == itemDetailAux.id);
                            if (inventoryMoveDetailAux != null)
                            {
                                tag = "036";
                                LogInfo(tag, DateTime.Now);
                                if (inventoryMoveDetailAux.genSecTrans)
                                {
                                    tag = "037";
                                    LogInfo(tag, DateTime.Now);
                                    lotAux = getLotSinTransaccion(inventoryMoveDetailAux.Lot?.number, itemDetailAux.Lot?.internalNumber);
                                    itemDetailAux.id_lot = lotAux?.id;
                                    itemDetailAux.Lot = lotAux;
                                }
                                else
                                {
                                    tag = "038";
                                    LogInfo(tag, DateTime.Now);
                                    lotAux = getLotSinTransaccion(secTransAux, itemDetailAux.Lot?.internalNumber);
                                    itemDetailAux.id_lot = lotAux?.id;
                                    itemDetailAux.Lot = lotAux;

                                    tag = "039";
                                    LogInfo(tag, DateTime.Now);
                                    inventoryReasonAux.sequential++;
                                    db.InventoryReason.Attach(inventoryReasonAux);
                                    db.Entry(inventoryReasonAux).State = EntityState.Modified;
                                }
                            }
                            else
                            {
                                tag = "040";
                                LogInfo(tag, DateTime.Now);
                                lotAux = getLotSinTransaccion(secTransAux, itemDetailAux.Lot?.internalNumber);
                                itemDetailAux.id_lot = lotAux?.id;
                                itemDetailAux.Lot = lotAux;


                                tag = "041";
                                LogInfo(tag, DateTime.Now);
                                inventoryReasonAux.sequential++;
                                db.InventoryReason.Attach(inventoryReasonAux);
                                db.Entry(inventoryReasonAux).State = EntityState.Modified;
                            }
                            //}
                        }

                        #endregion Update Gen. Sec. Trans.

                        tag = "042";
                        LogInfo(tag, DateTime.Now);
                        tempInventoryMove.InventoryEntryMove = entryMove;
                        tempInventoryMove.InventoryExitMove = exitMove;

                        tag = "043";
                        LogInfo(tag, DateTime.Now);
                        item.idNatureMove = inventoryReasons.FirstOrDefault(fod => fod.id == item.id_inventoryReason)?.idNatureMove;

                        ServiceInventoryMove.ServiceInventoryMoveAux result = null;

                        tag = "044";
                        LogInfo(tag, DateTime.Now);
                        if (natureMoveIMTmp.Trim().Equals("I"))
                        {

                            if (codeDocumentType == "03" || customParamOP == "IPXM")
                            {
                                result = ServiceInventoryMove.UpdateInventaryMoveEntry(approve, ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                                if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                                inventoryMoveDetailIdsForDelete = result.inventoryMoveDetailIdsForDelete;
                            }
                            if (codeDocumentType == "04")
                            {
                                result = ServiceInventoryMove.UpdateInventaryMoveEntryPurchaseOrder(approve, ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                                if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                            }
                            if (codeDocumentType == "34")
                            {
                                //result = ServiceInventoryMove.UpdateInventaryMoveTransferEntry
                                //            (approve,
                                //            ActiveUser,
                                //            ActiveCompany,
                                //            ActiveEmissionPoint,
                                //            item,
                                //            db,
                                //            false);

                                result = ServiceInventoryMove.UpdateInventaryMoveTransferEntryOP
                                            (
                                            connection,
                                            approve,
                                            ActiveUser,
                                            ActiveCompany,
                                            ActiveEmissionPoint,
                                            item,
                                            db,
                                            false,
                                            serviceInventoryMoveAux: prepareResults,
                                            documentStates: documentStates,
                                            emissionPoints: emissionPoints,
                                            inventoryReasons: inventoryReasons,
                                            employees: employees,
                                            warehouses: warehouses,
                                            warehouseLocations: warehouseLocations,
                                            metricUnits: metricUnits,
                                            metricUnitConversions: metricUnitConversions,
                                            settings: settings);
                                if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                                inventoryMoveDetailIdsForDelete = result.inventoryMoveDetailIdsForDelete;

                            }
                        }
                        else if (natureMoveIMTmp.Trim().Equals("E"))
                        {
                            tag = "045";
                            LogInfo(tag, DateTime.Now);
                            if (codeDocumentType == "05")
                            {
                                tag = "046";
                                LogInfo(tag, DateTime.Now);
                                result = ServiceInventoryMove.UpdateInventaryMoveExitOP(approve,
                                    ActiveUser,
                                    ActiveCompany,
                                    ActiveEmissionPoint,
                                    item,
                                    db,
                                    false,
                                    warehouses,
                                    documentStates,
                                    emissionPoints,
                                    inventoryReasons,
                                    metricUnits,
                                    documentTypes,
                                    metricUnitConversions,
                                    employees
                                    );
                                if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                            }
                            if (codeDocumentType == "32")
                            {
                                result = ServiceInventoryMove
                                                .ExecUpdateInventoryMoveTransferExit(   approve, 
                                                                                        ActiveUser, 
                                                                                        ActiveCompany, 
                                                                                        ActiveEmissionPoint, 
                                                                                        item, 
                                                                                        db, 
                                                                                        false, 
                                                                                        isNew,
                                                                                        settings,
                                                                                        warehouses,
                                                                                        warehouseLocations,
                                                                                        documentStates,
                                                                                        emissionPoints,
                                                                                        inventoryReasons,
                                                                                        employees,
                                                                                        metricUnits,
                                                                                        metricUnitConversions
                                                                                        );
                                if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                            }
                            if (codeDocumentType == "129")
                            {
                                result = ServiceInventoryMove.UpdateInventaryMoveTransferAutomaticExit(approve, ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                                if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                            }
                        }
                        else
                        {
                            if (codeDocumentType == "129")
                            {
                                result = ServiceInventoryMove.UpdateInventaryMoveTransferAutomaticExit(approve, ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                                if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);

                            }
                        }

                        tag = "047";
                        LogInfo(tag, DateTime.Now);
                        if (customParamOP == "IPXM")
                        {
                            if (result != null)
                            {
                                if (result.inventoryMove != null)
                                {
                                    string codeWareHouse = warehouses.FirstOrDefault(r => r.id == result.inventoryMove.idWarehouse)?.code;
                                    ViewData["_codeWareHouse"] = codeWareHouse;

                                    result.inventoryMove.id_productionLot = idProductionLot;

                                    InventoryMoveDetail _inventoryMoveDetail = result.inventoryMove.InventoryMoveDetail.First();

                                    if (_inventoryMoveDetail != null)
                                    {
                                        ProductionLot _productionLot = db.ProductionLot.FirstOrDefault(r => r.id == result.inventoryMove.id_productionLot);

                                        result.inventoryMove.idLotePackagingMaterials = result.inventoryMove.id_productionLot;
                                        result.inventoryMove.idWarehouseLocationPackagingMaterials = _inventoryMoveDetail.id_warehouseLocation;
                                        result.inventoryMove.nameProviderPackagingMaterials = _productionLot?.Provider?.Person?.fullname_businessName;
                                        result.inventoryMove.nameProductionUnitProviderPackagingMaterials = _productionLot?.ProductionUnitProvider?.name;
                                        result.inventoryMove.idCostCenterPackagingMaterials = _inventoryMoveDetail.id_costCenter;
                                        result.inventoryMove.idSubCostCenterPackagingMaterials = _inventoryMoveDetail.id_subCostCenter;
                                        result.inventoryMove.idItemPackagingMaterials = _inventoryMoveDetail.id_item;
                                        result.inventoryMove.nameItemDescriptionPackagingMaterials = _inventoryMoveDetail.Item?.description;
                                    }

                                    if (result.inventoryMove.id == 0)
                                    {
                                        result.inventoryMove.InventoryMoveExitPackingMaterial = _inventoryMoveExitPackingMaterial;
                                    }
                                    else
                                    {
                                        InventoryMoveExitPackingMaterial _inventoryMoveExitPackingMaterialResult = db.InventoryMoveExitPackingMaterial
                                            .FirstOrDefault(r => r.id_InventoryMove == item.id);

                                        if (_inventoryMoveExitPackingMaterialResult != null
                                            && _inventoryMoveExitPackingMaterialResult.id_ItemMaster != _inventoryMoveExitPackingMaterial?.id_ItemMaster)
                                        {
                                            db.InventoryMoveExitPackingMaterial.Attach(_inventoryMoveExitPackingMaterialResult);
                                            db.Entry(_inventoryMoveExitPackingMaterialResult).State = EntityState.Deleted;

                                            result.inventoryMove.InventoryMoveExitPackingMaterial = _inventoryMoveExitPackingMaterial;
                                        }
                                        else
                                        {
                                            if (_inventoryMoveExitPackingMaterialResult == null)
                                            {
                                                result.inventoryMove.InventoryMoveExitPackingMaterial = _inventoryMoveExitPackingMaterial;
                                            }
                                            else
                                            {
                                                result.inventoryMove.InventoryMoveExitPackingMaterial.quantityExit = _inventoryMoveExitPackingMaterial.quantityExit;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        item = result?.inventoryMove;

                        if (item.id == 0)
                        {
                            tag = "048";
                            LogInfo(tag, DateTime.Now);
                            db.InventoryMove.Add(item);
                        }
                        else
                        {
                            tag = "049";
                            LogInfo(tag, DateTime.Now);
                            // db.InventoryMove.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        tag = "050";
                        LogInfo(tag, DateTime.Now);

                        db.SaveChanges();
                        trans.Commit();

                        tag = "051";
                        LogInfo(tag, DateTime.Now);

                        isValidTransacction = true;

                        ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + item.Document.number + " guardado exitosamente");

                        TempData["inventoryMove"] = item;
                        TempData.Keep("inventoryMove");
                    }
                    catch (ProdHandlerException e)
                    {

                        ViewData["ErrorMessage"] = $"{(e.Message ?? "")}";
                        item = tempInventoryMove;
                        trans.Rollback();
                        if (string.IsNullOrEmpty(e.Message))
                        {
                            MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "InventoryMove", "Producion", seccion: tag);
                        }
                    }
                    catch (Exception e)
                    {

                        ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                        item = tempInventoryMove;
                        trans.Rollback();
                        MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "InventoryMove", "Producion");
                        //if (multiple) throw new Exception(e.Message);
                    }
                    finally
                    {
                        TempData.Keep("inventoryMove");
                    }
                }

            }


            if (isValidTransacction)
            {
                if ((codeDocumentType == "03" || customParamOP == "IPXM" || codeDocumentType == "32")   && approve && ((inventoryMoveDetailIdsForDelete?.Length??0)) >0 )
                {
                    Task.Run(async () =>
                    {

                        await ServiceTransCtl.DeleteBulkSp(inventoryMoveDetailIdsForDelete, this.GetType().Name, (item?.id ?? 0));

                    });

                }
                
            }
            
            tag = "052";
            LogInfo(tag, DateTime.Now);
            ViewData["numberSeqWarehouse"] = item.natureSequential;

            tag = "053";
            LogInfo(tag, DateTime.Now);
            item.FillDocumentSourceInformation();
            ViewData["numberDocument"] = item?.sequential?.ToString();

            ViewData["numberSeqWarehouse"] = item?.natureSequential;
            ViewData["fechaEmision"] = item?.Document?.emissionDate;

            tag = "054";
            LogInfo(tag, DateTime.Now);
            this.ConvertToDTO(item?.InventoryMoveDetail, item?.Document?.emissionDate);


            tag = "055";
            LogInfo(tag, DateTime.Now);
            BuildViewDataEdit();
            return PartialView("_InventoryMoveMainFormPartial", item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMovesPartialUpdate(bool approve, string codeDocumentType,
            string natureMoveIMTmp, InventoryMove item, Document document, InventoryEntryMove entryMove,
            InventoryExitMove exitMove, bool multiple, string customParamOP = null)
        {
            RefresshDataForEditForm(item.idWarehouse);            
            return InventoryMovesPartialAddNew(approve, codeDocumentType, natureMoveIMTmp, item, document, entryMove, exitMove, multiple, customParamOP);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMovesPartialDelete(System.Int32 id)
        {
            var model = db.InventoryMove;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)
                    {
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

        #endregion INVENTORY MOVE HEADER

        #region DETAILS

        [ValidateInput(false)]
        public ActionResult InventoryMoveDetails(int id_inventoryMove)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id_inventoryMove);
            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            ViewData["id_inventoryMove"] = id_inventoryMove;
            ViewData["code"] = inventoryMove.Document.DocumentType.code;
            ViewData["fechaEmision"] = inventoryMove.Document.emissionDate;
            ViewData["valorization"] = inventoryMove?.InventoryReason?.valorization ?? "";

            this.ConvertToDTO(model, inventoryMove.Document.emissionDate);

            return PartialView("_InventoryMoveDetailsPartial", model);
        }

        [HttpPost]
        public ActionResult RecargaGridDetail(bool? op, int? idWareHouseLocation)
        {
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            if (idWareHouseLocation != null)
            {
                foreach (var item in inventoryMove.InventoryMoveDetail)
                {
                    item.id_warehouseLocation = idWareHouseLocation;
                    item.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(i => i.id == idWareHouseLocation);
                }
            }

            TempData.Keep("inventoryMove");

            ViewData["code"] = (inventoryMove != null && inventoryMove.Document != null && inventoryMove.Document.DocumentType != null)
                ? inventoryMove.Document.DocumentType.code : "";
            ViewData["id_inventoryMove"] = (inventoryMove != null) ? inventoryMove.id : 0;

            string codeDocumentType = (inventoryMove != null && inventoryMove.Document != null && inventoryMove.Document.DocumentType != null)
                ? inventoryMove.Document.DocumentType.code : "";
            bool readOnlyCode = codeDocumentType != "03" && codeDocumentType != "04" && codeDocumentType != "34" && codeDocumentType != "32" &&
                                codeDocumentType != "05" && codeDocumentType != "129";
            bool mostrarOP = op ?? false;

            ViewData["readOnlyCode"] = readOnlyCode;
            ViewData["mostrarOP"] = mostrarOP;

            this.SetInvoiceDetailValidationViewBagData(inventoryMove.id_Invoice);

            return PartialView("_InventoryMoveDetailsEditFormPartial", inventoryMove.InventoryMoveDetail.ToList());
        }

        private void SetInvoiceDetailValidationViewBagData(int? idInvoice)
        {
            if (idInvoice.HasValue)
            {
                this.ViewBag.InvoiceDetailValid = db.InvoiceDetail
                    .Where(e => e.id_invoice == idInvoice && e.isActive)
                    .Select(e => new InvoiceDetailValid()
                    {
                        IdItem = e.id_item,
                        Cantidad = e.numBoxes ?? 0,
                    })
                    .ToArray();
            }
            else
            {
                this.ViewBag.InvoiceDetailValid = new InvoiceDetailValid[] { };
            }
        }

        public class InvoiceDetailValid
        {
            public int IdItem { get; set; }
            public decimal Cantidad { get; set; }
        }

        [ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartial(string code, int? idWarehouse,
            int? idWarehouseLocation, int? idWarehouseEntry, int? id_costCenter, int? id_subCostCenter, bool? withLotSystem,
            bool? withLotCustomer, bool? mostrarOP, int? id_inventoryReason, DateTime? fechaEmision, int? id_Invoice,
            int? id_itemType, int? id_size, int? id_trademark, int? id_trademarkModel, int? id_presentation, string codigoProducto,int? categoriaProducto,
            int? modeloProducto,
            Boolean deletedAll = false, string natureMoveType = null,
            string customParamOP = null)
        {

            string vista = "";
            List<InventoryMoveDetail> model = new List<InventoryMoveDetail>();

            RefresshDataForEditForm(idWarehouse);
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            string valInvFact = DataProviderSetting.ValueSetting("INVFACT");
            int idInventoryMove = (inventoryMove?.id ?? 0);
            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == idInventoryMove);
            inventoryMove = inventoryMove ?? new InventoryMove();
            if (deletedAll && inventoryMove != null)
            {
                inventoryMove.InventoryMoveDetail = new List<InventoryMoveDetail>();
            }

            model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();
            if (id_costCenter != null && id_costCenter != 0 && id_subCostCenter != null && id_subCostCenter != 0)
            {
                foreach (var item in model)
                {
                    item.id_costCenter = id_costCenter;
                    item.id_subCostCenter = id_subCostCenter;
                }
            }
            var objIr = db.InventoryReason.FirstOrDefault(fod => fod.id == id_inventoryReason);
            var idWarehouseAux = idWarehouse ?? inventoryMove.idWarehouse;
            var idWarehouseEntryAux = idWarehouseEntry ?? inventoryMove.idWarehouseEntry;

            if (objIr.op == true && idWarehouseLocation == null && id_Invoice != null && valInvFact == "SI")
            {
                var id_person = db.Invoice.FirstOrDefault(a => a.id == id_Invoice).id_buyer;
                idWarehouseLocation = db.WarehouseLocation
                    .FirstOrDefault(i => i.id_warehouse == idWarehouse && i.id_person == id_person && i.isActive)?
                    .id;
            }

            ViewBag.withLotSystem = withLotSystem;
            ViewBag.withLotCustomer = withLotCustomer;

            ViewBag.idWarehouse = idWarehouse ?? inventoryMove.idWarehouse;
            ViewBag.idWarehouseLocation = idWarehouseLocation ?? inventoryMove.idWarehouse;
            ViewBag.idWarehouseEntry = idWarehouseEntryAux;

            TempData["inventoryMove"] = TempData["inventoryMove"] ?? inventoryMove;
            TempData["idWarehouseLocation"] = TempData["idWarehouseLocation"] ?? idWarehouseLocation;
            TempData.Keep("inventoryMove");

            TempData["inventoryMoveSaldoDetailItem"] = null;
            TempData.Keep("inventoryMoveSaldoDetailItem");

            ViewData["id_inventoryMove"] = inventoryMove?.id ?? 0;
            ViewData["id_WarehouseLocation"] = idWarehouseLocation ?? 0;
            ViewData["code"] = code;
            ViewData["valorization"] = objIr?.valorization ?? "";
            ViewData["_natureMove"] = natureMoveType;
            ViewData["_customParamOP"] = customParamOP;
            ViewData["mostrarOP"] = mostrarOP;

            ViewData["fechaEmision"] = fechaEmision;
            ViewData["id_itemType"] = id_itemType;
            ViewData["id_size"] = id_size;
            ViewData["id_trademark"] = id_trademark;
            ViewData["id_trademarkModel"] = id_trademarkModel;
            ViewData["id_presentation"] = id_presentation;
            ViewData["codigoProducto"] = codigoProducto;
            ViewData["categoriaProducto"] = categoriaProducto;
            ViewData["modeloProducto"] = modeloProducto;

            if (valInvFact == "SI")
                this.SetInvoiceDetailValidationViewBagData(id_Invoice);


            if (code == "129")
            {
                vista = "_InventoryMoveAutoDetailsEditFormPartial";
            }
            else
            {
                vista = "_InventoryMoveDetailsEditFormPartial";
            }


            return PartialView(vista, model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartialAddNew(string code, string lotNumber,
            string lotInternalNumber, InventoryMoveDetail item, int id_item2, int? idWarehouse, int? idWarehouseEntry,
            string customParamOP, int? id_metricUnitInventoryPurchase, string lotMarked, DateTime? fechaEmision,
            int? id_Invoice)
        {
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();
            string valInvFact = DataProviderSetting.ValueSetting("INVFACT");

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
                bool verCosto = bool.Parse(Request.Params["cpEditingVerCosto"]);

                if (!verCosto)
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
                item.id_warehouse = idWarehouse.Value;
                item.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouse);
                item.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_warehouseLocation);
                item.id_warehouseEntry = idWarehouseEntry;
                item.Warehouse1 = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouseEntry);
                item.WarehouseLocation1 = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_warehouseLocationEntry);
                item.id_metricUnit = id_metricUnitInventoryPurchase.Value;
                item.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitInventoryPurchase);
                item.MetricUnit1 = db.MetricUnit.FirstOrDefault(fod => fod.id == item.id_metricUnitMove);
                item.lotMarked = lotMarked != "" ? lotMarked : null;

                if (code != null && (code.Equals("03") || code.Equals("04") || customParamOP.Equals("IPXM")))
                {
                    item.Lot = getLot(lotNumber, lotInternalNumber);
                    item.id_lot = item.Lot?.id;
                }
                else
                {
                    item.Lot = db.Lot.FirstOrDefault(fod => fod.id == item.id_lot);
                }

                if (code != null && (code.Equals("05") || code.Equals("32")))
                {
                    InvParameterBalanceGeneral invParameterBalance = new InvParameterBalanceGeneral();
                    invParameterBalance.id_company = this.ActiveCompanyId;
                    invParameterBalance.id_Item = item.id_item;
                    invParameterBalance.cut_Date = (DateTime)fechaEmision;
                    invParameterBalance.consolidado = true;
                    invParameterBalance.groupby = ServiceInventoryBalance.ServiceInventoryGroupBy.GROUPBY_BODEGA_UBICA_LOTE_ITEM;
                    invParameterBalance.id_Warehouse = item.id_warehouse;
                    invParameterBalance.id_WarehouseLocation = item.id_warehouseLocation;
                    invParameterBalance.id_ProductionLot = item.id_lot;

                    var resultBalance = ServiceInventoryBalance.ValidateBalanceGeneral(invParameterBalance, modelSaldoProductlote: false);
                    var resultBalaceItem = resultBalance.Item1;
                    var balanceItemSaldo = resultBalaceItem.Where(b => b.id_warehouse == item.id_warehouse &&
                                                                    b.id_warehouseLocation == item.id_warehouseLocation
                                                                    && b.id_item == item.id_item
                                                                    && b.id_productionLot == item.id_lot)
                                                      .Sum(c => c.SaldoActual);

                    var sumBalanceItem = inventoryMove.InventoryMoveDetail.Where(b => b.id_warehouse == item.id_warehouse &&
                                b.id_warehouseLocation == item.id_warehouseLocation && b.id_item == item.id_item && b.id_lot == item.id_lot).Sum(c => c.amountMove);

                    if (balanceItemSaldo < (sumBalanceItem + item.amountMove))
                    {
                        throw new Exception("La cantidad ingresada supera la suma total del saldo de los item agregados en el detalle.");
                    }
                }



                inventoryMove.InventoryMoveDetail.Add(item);
                TempData["inventoryMove"] = inventoryMove;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
                ViewData["code"] = code;
                ViewData["valorization"] = inventoryMove?.InventoryReason?.valorization ?? "";
                ViewData["id_inventoryMove"] = inventoryMove.id;
                TempData["inventoryMove"] = inventoryMove;
            }
            finally
            {
                TempData.Keep("inventoryMove");
            }

            

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            ViewData["code"] = code;
            ViewData["fechaEmision"] = fechaEmision;
            ViewData["valorization"] = inventoryMove?.InventoryReason?.valorization ?? "";
            if (valInvFact == "SI")
                this.SetInvoiceDetailValidationViewBagData(id_Invoice);

            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartialUpdate(string code,
            string lotNumber, string lotInternalNumber, InventoryMoveDetail item, int id_item2, int? idWarehouse, int? idWarehouseEntry,
            string customParamOP, int? id_metricUnitInventoryPurchase, string lotMarked, DateTime? fechaEmision, int? id_Invoice)
        {
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();
            RefresshDataForEditForm(idWarehouse);
            string valInvFact = DataProviderSetting.ValueSetting("INVFACT");

            try
            {
                var a = Request.Params["errorMessage"];

                var modelItem = inventoryMove.InventoryMoveDetail.FirstOrDefault(it => it.id == item.id);
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

                    item.id_metricUnit = id_metricUnitInventoryPurchase.Value;
                    item.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitInventoryPurchase);

                    modelItem.id_metricUnitMove = item.id_metricUnitMove;
                    modelItem.MetricUnit1 = db.MetricUnit.FirstOrDefault(fod => fod.id == item.id_metricUnitMove);

                    modelItem.id_item = id_item2;
                    modelItem.id_warehouse = idWarehouse.Value;
                    modelItem.id_warehouseLocation = item.id_warehouseLocation;
                    modelItem.id_warehouseEntry = idWarehouseEntry;
                    modelItem.id_warehouseLocationEntry = item.id_warehouseLocationEntry;

                    modelItem.Item = db.Item.FirstOrDefault(fod => fod.id == id_item2);
                    modelItem.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == modelItem.id_warehouse);
                    modelItem.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == modelItem.id_warehouseLocation);
                    modelItem.Warehouse1 = db.Warehouse.FirstOrDefault(fod => fod.id == modelItem.id_warehouseEntry);
                    modelItem.WarehouseLocation1 = db.WarehouseLocation.FirstOrDefault(fod => fod.id == modelItem.id_warehouseLocationEntry);
                    modelItem.id_userUpdate = ActiveUser.id;
                    modelItem.dateUpdate = DateTime.Now;
                    modelItem.ordenProduccion = item.ordenProduccion;
                    modelItem.lotMarked = lotMarked;

                    if (code != null && (code.Equals("03") || code.Equals("04") || customParamOP.Equals("IPXM")))
                    {
                        var lotAux = getLot(lotNumber, lotInternalNumber);
                        modelItem.id_lot = lotAux?.id;
                        modelItem.Lot = db.Lot.FirstOrDefault(fod => fod.id == modelItem.id_lot);
                    }
                    else
                    if (!code.Equals("34"))
                    {
                        modelItem.id_lot = item.id_lot;
                        modelItem.Lot = db.Lot.FirstOrDefault(fod => fod.id == item.id_lot);
                    }

                    if (code != null && (code.Equals("05") || code.Equals("32")))
                    {
                        InvParameterBalanceGeneral invParameterBalance = new InvParameterBalanceGeneral();
                        invParameterBalance.id_company = this.ActiveCompanyId;
                        invParameterBalance.id_Item = item.id_item;
                        invParameterBalance.cut_Date = inventoryMove.Document.emissionDate;
                        invParameterBalance.consolidado = true;
                        invParameterBalance.groupby = ServiceInventoryBalance.ServiceInventoryGroupBy.GROUPBY_BODEGA_UBICA_LOTE_ITEM;
                        invParameterBalance.id_Warehouse = item.id_warehouse;
                        invParameterBalance.id_WarehouseLocation = item.id_warehouseLocation;
                        invParameterBalance.id_ProductionLot = item.id_lot;

                        var resultBalance = ServiceInventoryBalance.ValidateBalanceGeneral(invParameterBalance, modelSaldoProductlote: false);
                        var resultBalaceItem = resultBalance.Item1;
                        var balanceItemSaldo = resultBalaceItem.Where(b => b.id_warehouse == item.id_warehouse &&
                                                                        b.id_warehouseLocation == item.id_warehouseLocation
                                                                        && b.id_item == item.id_item
                                                                        && b.id_productionLot == item.id_lot)
                                                          .Sum(c => c.SaldoActual);

                        var sumBalanceItem = inventoryMove.InventoryMoveDetail.Where(b => b.id_warehouse == item.id_warehouse &&
                                    b.id_warehouseLocation == item.id_warehouseLocation && b.id_item == item.id_item && b.id_lot == item.id_lot).Sum(c => c.amountMove);

                        if (balanceItemSaldo < sumBalanceItem)
                        {
                            throw new Exception("La cantidad ingresada supera la suma total del saldo de los item agregados en el detalle.");
                        }
                    }


                    this.UpdateModel(modelItem);
                    TempData["inventoryMove"] = inventoryMove;
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
                ViewData["code"] = code;
                ViewData["valorization"] = inventoryMove?.InventoryReason?.valorization ?? "";
                ViewData["id_inventoryMove"] = inventoryMove.id;
            }
            finally
            {
                TempData.Keep("inventoryMove");
            }

            

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();

            ViewData["id_inventoryMove"] = inventoryMove.id;
            ViewData["code"] = code;
            ViewData["fechaEmision"] = fechaEmision;
            ViewData["valorization"] = inventoryMove?.InventoryReason?.valorization ?? "";
            if (valInvFact == "SI")
                this.SetInvoiceDetailValidationViewBagData(id_Invoice);

            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveDetailsEditFormPartialDelete(
            string code, DateTime? fechaEmision, System.Int32 id, int? id_Invoice)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            string valInvFact = DataProviderSetting.ValueSetting("INVFACT");
            inventoryMove = inventoryMove ?? db.InventoryMove.FirstOrDefault(i => i.id == inventoryMove.id);
            inventoryMove = inventoryMove ?? new InventoryMove();

            try
            {
                var item = inventoryMove.InventoryMoveDetail.FirstOrDefault(p => p.id == id);
                if (item != null)
                    inventoryMove.InventoryMoveDetail.Remove(item);

                TempData["inventoryMove"] = inventoryMove;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
                ViewData["code"] = code;
                ViewData["valorization"] = inventoryMove?.InventoryReason?.valorization ?? "";
            }
            finally
            {
                TempData.Keep("inventoryMove");
            }

            

            ViewData["code"] = code;
            ViewData["fechaEmision"] = fechaEmision;
            ViewData["valorization"] = inventoryMove?.InventoryReason?.valorization ?? "";
            if (valInvFact == "SI")
                this.SetInvoiceDetailValidationViewBagData(id_Invoice);

            var model = inventoryMove?.InventoryMoveDetail.ToList() ?? new List<InventoryMoveDetail>();
            return PartialView("_InventoryMoveDetailsEditFormPartial", model.ToList());
        }

        #endregion DETAILS

        #region PURCHASE ORDERS

        [HttpPost]
        public ActionResult PurchaseOrdersResult()
        {
            return PartialView("_PurchaseOrdersResult");
        }

        [ValidateInput(false)]
        public ActionResult PurchaseOrdersDetailsPartial()
        {
            var model = db.PurchaseOrderDetail.Where(d => d.PurchaseOrder.Document.DocumentState.code == "06" &&
                                                          d.PurchaseOrder.PurchaseReason.code.Equals("MI") &&
                                                          (d.quantityApproved - d.quantityReceived) > 0 &&
                                                          d.Item.InventoryLine.code != "MP" &&
                                                          d.Item.InventoryLine.code != "PT" &&
                                                          d.Item.InventoryLine.code != "PP")
                          .OrderByDescending(d => d.id_purchaseOrder).ToList().OrderByDescending(d => d.id_item);

            return PartialView("_PurchaseOrdersDetailsPartial", model.ToList());
        }

        #endregion PURCHASE ORDERS

        #region INVENTORY MOVE DETAIL TRANSFER EXITS

        [HttpPost]
        public ActionResult InventoryMoveDetailTransferExitsResult()
        {
            return PartialView("_InventoryMoveDetailTransferExitsResult");
        }

        [HttpPost]
        public ActionResult InventoryMoveDetailTransferExitsPartial()
        {
            string[] codigosAprobadoConciliado = new string[] { "03", "16" };
            int documentTypeId = db.DocumentType
                                        .FirstOrDefault(r => r.code == "32")?.id ?? 0;

            var documentStatesIds = db.DocumentState
                                            .Where(r => codigosAprobadoConciliado.Contains(r.code))
                                            .Select(r => new
                                            {
                                                r.id,
                                                r.code
                                            }).ToArray();

            int documentStateId = documentStatesIds?
                                        .FirstOrDefault(r => r.code == "03")?.id ?? 0;

            var documentTranferEgresodIds = db.Document
                                                .Where(r => r.id_documentType == documentTypeId
                                                            && r.id_documentState == documentStateId
                                                ).Select(r => r.id).ToArray();

            var model = db.InventoryMoveDetail
                                .Where(d => documentTranferEgresodIds.Contains(d.id_inventoryMove))
                                .ToList();

            int[] statesIdsForTransfer = documentStatesIds.Select(r => r.id).ToArray();
            var detailTransfer = db.InventoryMoveDetailTransfer
                                                    .Where(w => statesIdsForTransfer.Contains(w.InventoryMoveDetail1.InventoryMove.Document.id_documentState))
                                                    .Select(r => new
                                                    {
                                                        inventoryMoveDetail = r.id_inventoryMoveDetailExit,
                                                        quantity = r.quantity
                                                    }).ToArray();
            var modelDetailIds = (from detMove in model
                                  join detTra in detailTransfer
                                  on detMove.id equals detTra.inventoryMoveDetail
                                  into p1
                                  from detT in p1.DefaultIfEmpty()
                                  select new
                                  {
                                      inventoryMoveDetailId = detMove.id,
                                      haveTra = (detT == null) ? false : true,
                                      quantity = (detT == null) ? 0 : detT.quantity,
                                      amountMove = detMove.amountMove
                                  })
                               .GroupBy(r => r.inventoryMoveDetailId)
                               .Where(r => (r.Max(t => t.haveTra) == true
                                            && r.Sum(t => t.quantity) > 0
                                            && r.Sum(t => t.quantity) < r.Max(t => t.amountMove)
                                            )
                                            || r.Max(t => t.haveTra) == false)
                               .Select(r => new
                               {
                                   id = r.Key,
                                   quantityPending = r.Max(t => t.amountMove) - r.Sum(t => t.quantity)
                               })
                               .ToArray();

            var imids = modelDetailIds.Select(r => r.id).ToArray();
            model = db.InventoryMoveDetail
                            .Where(r => imids.Contains(r.id))
                            .ToList();
            model = model
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
                            decimal quantityPending = (modelDetailIds
                                                            .FirstOrDefault(r => r.id == item.id)?.quantityPending ?? 0);
                            item.QuantityPending = quantityPending;
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
                            decimal quantityPending = (modelDetailIds
                                                            .FirstOrDefault(r => r.id == item.id)?.quantityPending ?? 0);
                            item.QuantityPending = quantityPending;
                            tempModel2.Add(item);
                        }
                    }

                    model = tempModel2;
                }
            }

            return PartialView("_InventoryMoveDetailTransferExitsPartial", model.ToList());
        }

        #endregion INVENTORY MOVE DETAIL TRANSFER EXITS

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
                    ViewData["ErrorMessage"] = e.Message
                           + $" Base exception: {e.GetBaseException().Message}";
                    trans.Rollback();
                }
            }

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            ViewData["numberSeqWarehouse"] = inventoryMove?.natureSequential;
            ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;
            this.ConvertToDTO(inventoryMove?.InventoryMoveDetail, inventoryMove?.Document?.emissionDate);

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

                        if (currentState.id != 3)
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
                    ViewData["ErrorMessage"] = e.Message
                           + $" Base exception: {e.GetBaseException().Message}";
                    trans.Rollback();
                }
            }

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            inventoryMove.FillDocumentSourceInformation();
            ViewData["numberDocument"] = inventoryMove?.sequential?.ToString();
            ViewData["numberSeqWarehouse"] = inventoryMove?.natureSequential;
            ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;
            this.ConvertToDTO(inventoryMove?.InventoryMoveDetail, inventoryMove?.Document?.emissionDate);

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
                    ViewData["ErrorMessage"] = e.Message
                        + $" Base exception: {e.GetBaseException().Message}";
                    trans.Rollback();
                }
            }

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");
            ViewData["numberSeqWarehouse"] = inventoryMove?.natureSequential;
            ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;
            this.ConvertToDTO(inventoryMove?.InventoryMoveDetail, inventoryMove?.Document?.emissionDate);

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);
            string natureMove = "";
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var paramPackagingMaterials = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("IPXM") as List<AdvanceParametersDetailModelP>;
                    if (paramPackagingMaterials == null) throw new Exception("No se han definido parámetros para Egreso de Materiales de Empaque");

                    var datInventoryReason = paramPackagingMaterials.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "INRS");
                    if (datInventoryReason == null) throw new Exception("No se han definido parámetro de Razón de Inventario para Egreso de Materiales de Empaque");

                    if (inventoryMove.InventoryReason.code == datInventoryReason.nameAdvanceDetailModelP.Trim())
                    {
                        ViewData["_customParamOP"] = "IPXM";
                    }

                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            if (inventoryMove.idWarehouse != null)
                            {
                                var entityValuePermissions = entityPermissions
                                                                .listValue
                                                                .FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMove.idWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Reversar.");
                                }
                            }
                        }
                    }

                    natureMove = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == inventoryMove.idNatureMove)?.valueCode?.Trim();
                    ViewData["_natureMove"] = natureMove;

                    if (!(inventoryMove.Document.DocumentType.code.Equals("03") || (string)ViewData["_customParamOP"] == "IPXM" || inventoryMove.Document.DocumentType.code.Equals("04") ||
                            inventoryMove.Document.DocumentType.code.Equals("34") || inventoryMove.Document.DocumentType.code.Equals("05") || inventoryMove.Document.DocumentType.code.Equals("32") ||
                            inventoryMove.Document.DocumentType.code.Equals("129")))
                    {
                        throw new Exception("El movimiento de inventario, tiene un motivo de inventario: " + inventoryMove.InventoryReason.name + ", que tiene como tipo de documento: " + inventoryMove.Document.DocumentType.name + ", con código: " +
                                            inventoryMove.Document.DocumentType.code + ", el cual no se puede cancelar en esta opción. Verifique la configuración del motivo e intente de nuevo.");
                    }
                    else
                    {
                        ServiceInventoryMove.ValidateEmissionDateInventoryMove(db, inventoryMove.Document.emissionDate, true, inventoryMove.idWarehouse);
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

                    if (inventoryMove != null && documentState != null)
                    {
                        natureMove = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == inventoryMove.idNatureMove)?.valueCode?.Trim();
                        ViewData["_natureMove"] = natureMove;

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
                    ViewData["ErrorMessage"] = e.Message
                        + $" Base exception: {e.GetBaseException().Message}";
                    trans.Rollback();
                }
            }

            var settingORLS = db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
            ViewBag.withLotSystem = settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false;
            var settingORLC = db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
            ViewBag.withLotCustomer = settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false;

            inventoryMove.FillDocumentSourceInformation();
            ViewData["numberDocument"] = inventoryMove?.sequential?.ToString();
            ViewData["numberSeqWarehouse"] = inventoryMove?.natureSequential;
            ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;
            this.ConvertToDTO(inventoryMove?.InventoryMoveDetail, inventoryMove?.Document?.emissionDate);

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]

        public ActionResult Conciliate(int id)
        {
            string natureMove = "";
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);
            using (var trans = db.Database.BeginTransaction())
            {
                var result = new ApiResult();


                try
                {
                    // Buscamos el estado 'Conciliado'
                    var estadoConciliado = db.DocumentState.FirstOrDefault(e => e.code == "16");
                    if (estadoConciliado == null)
                        throw new Exception("No se ha encontrado el estado: [16] - CONCILIADO");

                    // Busco el elemento, modifico y guardo

                    inventoryMove.Document.id_documentState = estadoConciliado.id;
                    inventoryMove.Document.id_userUpdate = this.ActiveUserId;
                    inventoryMove.Document.dateUpdate = DateTime.Now;

                    db.InventoryMove.Attach(inventoryMove);
                    db.Entry(inventoryMove).State = EntityState.Modified;
                    db.SaveChanges();

                    trans.Commit();

                    result.Data = estadoConciliado.name;

                    TempData["inventoryMove"] = inventoryMove;
                    TempData.Keep("inventoryMove");

                    ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + inventoryMove.Document.number + " conciliado exitosamente");
                }
                catch (Exception ex)
                {
                    result.Code = ex.HResult;
                    result.Message = ex.Message;
                    trans.Rollback();
                }

            }


            natureMove = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == inventoryMove.idNatureMove)?.valueCode?.Trim();
            ViewData["_natureMove"] = natureMove;

            ViewData["numberSeqWarehouse"] = inventoryMove?.natureSequential;
            ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;
            this.ConvertToDTO(inventoryMove?.InventoryMoveDetail, inventoryMove?.Document?.emissionDate);

            //return Json(result, JsonRequestBehavior.AllowGet);
            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        [HttpPost]

        public ActionResult Reverse(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        // Revisión del documento 
                        var document = db.Document.FirstOrDefault(e => e.id == id);
                        BuildViewDataEdit();
                        // Reversión en base a estado
                        if (document.DocumentState.code == "16")
                        {
                            return ReversarConciliacion(id);
                        }
                        else
                        {
                            return Revert(id);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Code = ex.HResult;
                        result.Message = ex.Message;
                        trans.Rollback();
                        return null;
                    }
                }
            }
        }

        public ActionResult ReversarConciliacion(int idInventoryMove)
        {
            string natureMove = "";
            var inventoryMove = db.InventoryMove.FirstOrDefault(e => e.id == idInventoryMove);

            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var reverseStateInventoryMove = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));

                    // Búsqueda de documento            
                    var estadoMovimientoInventario = inventoryMove?.Document != null
                        ? db.DocumentState.FirstOrDefault(e => e.id == inventoryMove.Document.id_documentState)
                        : db.DocumentState.FirstOrDefault(e => e.code == "01");

                    // Permiso 
                    //int id_menu = (int)ViewData["id_menu"];
                    //var tienePermisioReversar = this.ActiveUser
                    //    .UserMenu.FirstOrDefault(e => e.id_menu == id_menu)?
                    //    .Permission?.FirstOrDefault(p => p.name == "Reversar");
                    //
                    //var tienePermisioConciliar = this.ActiveUser
                    //    .UserMenu.FirstOrDefault(e => e.id_menu == id_menu)?
                    //    .Permission?.FirstOrDefault(p => p.name == "Conciliar");

                    // No podrá reversar si no tiene ambos permisos

                    //if (tienePermisioReversar != null && tienePermisioConciliar != null)
                    //{
                        //Verificar si existe lotes relacionados al proceso de cierre
                        var inventoryMoveDetail = inventoryMove.InventoryMoveDetail.ToList();
                        if (db.ProductionLotClose.Count() > 0)
                        {

                            foreach (var item in inventoryMoveDetail)
                            {
                                var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.number.Substring(0, 5) == item.Lot.internalNumber.Substring(0, 5) && a.Document.DocumentState.code != "05"
                                                           && a.isActive);

                                if (productionLotClose != null && inventoryMove.Document.emissionDate.Date <= productionLotClose.Document.emissionDate.Date
                                    && inventoryMove.Document.DocumentState.code == "16")
                                {
                                    var lot = db.ProductionLot.FirstOrDefault(a => a.id == item.id_lot);
                                    if (lot != null)
                                    {
                                        throw new Exception("El lote " + lot.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                                    }
                                }
                            }
                        }

                        inventoryMove.Document.id_documentState = reverseStateInventoryMove.id;
                        inventoryMove.Document.id_userUpdate = this.ActiveUserId;
                        inventoryMove.Document.dateUpdate = DateTime.Now;

                        db.InventoryMove.Attach(inventoryMove);
                        db.Entry(inventoryMove).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Movimiento de Inventario: " + inventoryMove.Document.number + " reversado exitosamente");
                    //}
                    //else
                    //{
                    //    ViewData["EditMessage"] = ErrorMessage("Usuario no autorizado para reversar en estado CONCILIADO");
                    //}
                }
                catch (Exception ex)
                {
                     ViewData["ErrorMessage"] = "Error al reversar. " + ex.Message;
                     trans.Rollback();
                }
            }

            natureMove = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == inventoryMove.idNatureMove)?.valueCode?.Trim();
            ViewData["_natureMove"] = natureMove;

            ViewData["numberDocument"] = inventoryMove?.sequential?.ToString();
            ViewData["numberSeqWarehouse"] = inventoryMove?.natureSequential;
            ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;
            this.ConvertToDTO(inventoryMove?.InventoryMoveDetail, inventoryMove?.Document?.emissionDate);

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        public ActionResult Revert(int id)
        {
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);
            string natureMove = "";
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    string valInvFact = DataProviderSetting.ValueSetting("INVFACT");
                    var paramPackagingMaterials = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("IPXM") as List<AdvanceParametersDetailModelP>;
                    if (paramPackagingMaterials == null) throw new Exception("No se han definido parámetros para Egreso de Materiales de Empaque");

                    var datInventoryReason = paramPackagingMaterials.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "INRS");
                    if (datInventoryReason == null) throw new Exception("No se han definido parámetro de Razón de Inventario para Egreso de Materiales de Empaque");

                    if (inventoryMove.InventoryReason.code == datInventoryReason.nameAdvanceDetailModelP.Trim())
                    {
                        ViewData["_customParamOP"] = "IPXM";
                    }

                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            if (inventoryMove.idWarehouse != null)
                            {
                                var entityValuePermissions = entityPermissions
                                                                .listValue
                                                                .FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMove.idWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Reversar.");
                                }
                            }
                        }
                    }

                    natureMove = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == inventoryMove.idNatureMove)?.valueCode?.Trim();
                    ViewData["_natureMove"] = natureMove;

                    if (valInvFact == "SI")
                    {
                        //Verifico si existe una factura Relacionado
                        string[] documentStateCodes = new string[] { "03", "06", "09" };

                        var lstInvoiceInventory = db.Document.FirstOrDefault(w => documentStateCodes.Contains(w.DocumentState.code) && w.id == inventoryMove.id_Invoice);
                        if (lstInvoiceInventory != null)
                        {
                            throw new Exception("Existe Factura Fiscal: " + lstInvoiceInventory.number + "  relacionado al movimiento.");
                        }
                    }

                    #region Validación Permiso

                    int id_user = (int)ViewData["id_user"];
                    int id_menu = (int)ViewData["id_menu"];

                    User user = DataProviderUser.UserById(id_user);
                    UserMenu userMenu = user.UserMenu.FirstOrDefault(m => m.Menu.id == id_menu);
                    if (userMenu != null)
                    {
                        Permission permission = userMenu.Permission.FirstOrDefault(p => p.name == "Reversar");
                        if (permission == null)
                        {
                            throw new Exception("No tiene Permiso para Reversar el movimiento");
                        }
                    }

                    #endregion Validación Permiso

                    //Verificar si existe lotes relacionados al proceso de cierre
                    var inventoryMoveDetail = inventoryMove.InventoryMoveDetail.ToList();
                    if (db.ProductionLotClose.Count() > 0)
                    {


                        foreach (var item in inventoryMoveDetail)
                        {
                            var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.number.Substring(0, 5) == item.Lot.internalNumber.Substring(0, 5) && a.Document.DocumentState.code != "05"
                                                       && a.isActive);

                            if (productionLotClose != null && inventoryMove.Document.emissionDate.Date <= productionLotClose.Document.emissionDate.Date
                                && inventoryMove.Document.DocumentState.code == "03")
                            {
                                var lot = db.Lot.FirstOrDefault(a => a.id == item.id_lot);
                                if (lot != null)
                                {
                                    throw new Exception("El lote " + lot.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                                }
                            }
                        }
                    }
                    DocumentState documentStatePendiente = db.DocumentState.FirstOrDefault(s => s.code == "01");

                    if (inventoryMove != null && documentStatePendiente != null)
                    {
                        if (inventoryMove.Document.DocumentType.code.Equals("03") || (string)ViewData["_customParamOP"] == "IPXM" || inventoryMove.Document.DocumentType.code.Equals("04") ||
                            inventoryMove.Document.DocumentType.code.Equals("34") || inventoryMove.Document.DocumentType.code.Equals("05") || inventoryMove.Document.DocumentType.code.Equals("32") ||
                            inventoryMove.Document.DocumentType.code.Equals("129"))
                        {
                            if (natureMove.Equals("I"))
                            {
                                if (inventoryMove.Document.DocumentType.code.Equals("03") || (string)ViewData["_customParamOP"] == "IPXM")
                                {
                                    var inventoryMoveING = inventoryMove;
                                    var result = ServiceInventoryMove.UpdateInventaryMoveEntry(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveING);
                                    inventoryMove = result.inventoryMove;
                                }

                                if (inventoryMove.Document.DocumentType.code.Equals("04"))
                                {
                                    var inventoryMoveIOC = inventoryMove;
                                    var result = ServiceInventoryMove.UpdateInventaryMoveEntryPurchaseOrder(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveIOC);
                                    inventoryMove = result.inventoryMove;
                                }

                                if (inventoryMove.Document.DocumentType.code.Equals("34"))
                                {
                                    var inventoryMoveTranferEntry = inventoryMove;
                                    var result = ServiceInventoryMove.UpdateInventaryMoveTransferEntry(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveTranferEntry);
                                    inventoryMove = result.inventoryMove;
                                }
                            }
                            else if (natureMove.Equals("E"))
                            {
                                if (inventoryMove.Document.DocumentType.code.Equals("05"))
                                {
                                    var inventoryMoveEGR = inventoryMove;
                                    var result = ServiceInventoryMove.UpdateInventaryMoveExit(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveEGR);
                                    inventoryMove = result.inventoryMove;
                                }

                                if (inventoryMove.Document.DocumentType.code.Equals("32"))
                                {
                                    var inventoryMoveTranferExit = inventoryMove;
                                    var result = ServiceInventoryMove.UpdateInventaryMoveTransferExit(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveTranferExit);
                                    inventoryMove = result.inventoryMove;
                                }
                                if (inventoryMove.Document.DocumentType.code.Equals("129"))
                                {
                                    var inventoryMoveTransferAutomaticExit = inventoryMove;
                                    var result = ServiceInventoryMove.UpdateInventaryMoveTransferAutomaticExit(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMove, db, true, inventoryMoveTransferAutomaticExit);
                                    inventoryMove = result.inventoryMove;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("El movimiento de inventario, tiene un motivo de inventario: " + inventoryMove.InventoryReason.name + ", que tiene como tipo de documento: " + inventoryMove.Document.DocumentType.name + ", con código:" +
                                                inventoryMove.Document.DocumentType.code + ", el cual no se puede Reversar en esta opción. Verifique la configuración del motivo e intente de nuevo.");
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
                    ViewData["ErrorMessage"] = e.Message;
                    trans.Rollback();
                }
            }

            var settingORLS = db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
            ViewBag.withLotSystem = inventoryMove.InventoryReason?.requiereSystemLotNubmber ?? (settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false);
            var settingORLC = db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
            ViewBag.withLotCustomer = inventoryMove.InventoryReason?.requiereUserLotNubmber ?? (settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false);

            inventoryMove.FillDocumentSourceInformation();
            ViewData["numberDocument"] = inventoryMove?.sequential?.ToString();

            ViewData["numberSeqWarehouse"] = inventoryMove?.natureSequential;
            ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;
            this.ConvertToDTO(inventoryMove?.InventoryMoveDetail, inventoryMove?.Document?.emissionDate);

            return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
        }

        #endregion SINGLE CHANGE DOCUMENT STATE

        #region SELECTED DOCUMENT STATE CHANGE

        [HttpPost, ValidateInput(false)]
        public JsonResult ApproveDocuments(int[] ids)
        {
            GenericResultJson oJsonResult = new GenericResultJson();
            string strMessageFinal = "";

            if (ids != null)
            {

                try
                {
                    foreach (var id in ids)
                    {
                        InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");
                        Document document = db.Document.FirstOrDefault(r => r.id == id);
                        InventoryEntryMove entryMove = db.InventoryEntryMove.FirstOrDefault(r => r.id == id);
                        InventoryExitMove exitMove = db.InventoryExitMove.FirstOrDefault(r => r.id == id);
                        var codeDocumentType = inventoryMove.Document.DocumentType.code;
                        var natureMoveIMTmp = inventoryMove.AdvanceParametersDetail.valueCode;
                        TempData["inventoryMove"] = inventoryMove;

                        InventoryMovesPartialUpdate(true, codeDocumentType, natureMoveIMTmp, inventoryMove, document, entryMove, exitMove, true, "");
                    }

                }
                catch (Exception e)
                {

                    strMessageFinal = e.Message;
                    var result = new { message = strMessageFinal };
                    oJsonResult.codeReturn = -1;
                    oJsonResult.message = ErrorMessage(strMessageFinal);
                }

            }
            if (strMessageFinal == "")
            {
                oJsonResult.codeReturn = -1;
                oJsonResult.message = ErrorMessage("Los movimientos seleccionados han sido aprobados");
            }

            var model = (TempData["model"] as List<InventoryMove>);
            model = model ?? new List<InventoryMove>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.InventoryMove.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");


            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
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
                        ViewData["ErrorMessage"] = e.Message
                        + $" Base exception: {e.GetBaseException().Message}";
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
                        ViewData["ErrorMessage"] = e.Message
                        + $" Base exception: {e.GetBaseException().Message}";
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
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["ErrorMessage"] = e.Message
                        + $" Base exception: {e.GetBaseException().Message}";
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
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["ErrorMessage"] = e.Message
                        + $" Base exception: {e.GetBaseException().Message}";
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

        #endregion SELECTED DOCUMENT STATE CHANGE

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_inventoryMove, string codeDocumentType)
        {
            TempData.Keep("inventoryMove");

            Parametros.ParametrosBusquedaInventoryMove parametrosBusquedaInventoryMove = new Parametros.ParametrosBusquedaInventoryMove();
            parametrosBusquedaInventoryMove.id_documentType = db.DocumentType.FirstOrDefault(fod => fod.code.Equals(codeDocumentType))?.id ?? 0;
            parametrosBusquedaInventoryMove.id_user = ActiveUser.id;

            List<InventoryMoveDTO> inventoryMoveAux = GetInventoryMove(parametrosBusquedaInventoryMove);
            int index = inventoryMoveAux.Count() > 0 ? inventoryMoveAux.FindIndex(r => r.id == id_inventoryMove) : 0;

            var result = new
            {
                maximunPages = inventoryMoveAux.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page, string codeDocumentType)
        {
            Parametros.ParametrosBusquedaInventoryMove parametrosBusquedaInventoryMove = new Parametros.ParametrosBusquedaInventoryMove();
            parametrosBusquedaInventoryMove.id_documentType = db.DocumentType.FirstOrDefault(fod => fod.code.Equals(codeDocumentType))?.id ?? 0;
            parametrosBusquedaInventoryMove.id_user = ActiveUser.id;

            List<InventoryMoveDTO> inventoryMoveAux = GetInventoryMove(parametrosBusquedaInventoryMove);

            InventoryMoveDTO inventoryMoveDTO = inventoryMoveAux.Take(page).ToList().Last();
            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(fod => fod.id == inventoryMoveDTO.id);

            if (inventoryMove != null)
            {
                TempData["inventoryMove"] = inventoryMove;
                TempData.Keep("inventoryMove");
                ViewData["numberSeqWarehouse"] = inventoryMove.natureSequential;
                ViewData["fechaEmision"] = inventoryMove?.Document?.emissionDate;

                var idNatureMove = inventoryMove.idNatureMove;

                string codeNatureMove = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == idNatureMove)?.valueCode;
                codeNatureMove = codeNatureMove.Trim();
                ViewData["_natureMove"] = codeNatureMove.Trim();

                var settingORLS = db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
                ViewBag.withLotSystem = inventoryMove.InventoryReason?.requiereSystemLotNubmber ?? (settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == codeDocumentType)?.valueAux == "1" : false);
                var settingORLC = db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
                ViewBag.withLotCustomer = inventoryMove.InventoryReason?.requiereSystemLotNubmber ?? (settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == codeDocumentType)?.valueAux == "1" : false);

                this.ConvertToDTO(inventoryMove?.InventoryMoveDetail, inventoryMove?.Document?.emissionDate);

                return PartialView("_InventoryMoveMainFormPartial", inventoryMove);
            }

            TempData.Keep("inventoryMove");

            return PartialView("_InventoryMoveMainFormPartial", new InventoryMove());
        }

        #endregion PAGINATION

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
                btnConciliate = false,
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(r => r.id == id);
            string state = inventoryMove.Document.DocumentState.code;

            bool isAuthomatic = inventoryMove.isAuthomatic ?? false;

            if (state == "01")
            {
                actions = new
                {
                    btnApprove = true && isAuthomatic == false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true && isAuthomatic == false,
                    btnRevert = false,
                    btnConciliate = false,
                };
            }
            else if (state == "03")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true && isAuthomatic == false,
                    btnConciliate = true,
                };
            }
            else if (state == "04" || state == "05")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnConciliate = false,
                };
            }
            else if (state == "06")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true && isAuthomatic == false,
                    btnConciliate = true,
                };
            }
            else if (state == "16")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnConciliate = false,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion ACTIONS

        #region REPORTS

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveReportList()
        {
            return PartialView("_InventoryMoveListReport");
        }

        #endregion REPORTS

        #region AUXILIAR FUNCTIONS

        public JsonResult InventoryReasonChanged(int idIR)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? new InventoryMove();
            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();

            var objIr = db.InventoryReason.FirstOrDefault(fod => fod.id == idIR);
            var objIrCode = db.InventoryReason.FirstOrDefault(fod => fod.id == idIR && (fod.code.Equals("17") || fod.code.Equals("33")));

            var result = new
            {
                codeIR = objIr?.DocumentType?.code ?? "",
                requiereSystemLotNubmber = objIr?.requiereSystemLotNubmber ?? false,
                requiereUserLotNubmber = objIr?.requiereUserLotNubmber ?? false,
                codeReason = objIrCode?.code != null ? "SI" : "NO",
                op = objIr?.op ?? false,
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");
            ViewData["valorization"] = objIr?.valorization ?? "";

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItem(int? id, int? id_itemNew,
            int? id_warehouseNew, int? id_warehouseLocationNew, string lotNumberNew, string lotInternalNumberNew, int? id_lotNew, string lotMarked)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? new InventoryMove();
            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();

            var result = new
            {
                itsRepeated = 0,
                Error = "",
                //FormattedError = ""
            };

            id_lotNew = id_lotNew ?? getLot(lotNumberNew, lotInternalNumberNew)?.id;
            var lotAux = db.Lot.FirstOrDefault(fod => fod.id == id_lotNew);
            lotNumberNew = string.IsNullOrEmpty(lotNumberNew) ? lotAux?.number ?? "" : lotNumberNew;
            lotInternalNumberNew = string.IsNullOrEmpty(lotInternalNumberNew) ? lotAux?.internalNumber ?? "" : lotInternalNumberNew;
            foreach (var inventoryMoveDetail in inventoryMove.InventoryMoveDetail)
            {
                if ((inventoryMoveDetail.Lot?.number ?? "") == lotNumberNew && (inventoryMoveDetail.Lot?.internalNumber ?? "") == lotInternalNumberNew &&
                     inventoryMoveDetail.id_warehouse == id_warehouseNew && inventoryMoveDetail.id_warehouseLocation == id_warehouseLocationNew &&
                     inventoryMoveDetail.id_item == id_itemNew && inventoryMoveDetail.id_lot == id_lotNew && (inventoryMoveDetail.lotMarked ?? "") == lotMarked)
                {
                    var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                    var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                    var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                    string errorMessage = "No se puede repetir el Item: " + itemAux.name +
                                            ",  en la bodega: " + warehouseAux.name +
                                            ", en la ubicación: " + warehouseLocationAux.name +
                                            ", con el Lote: " + ((lotAux != null) ? ((lotAux.number ?? "") + ((lotAux.number != "" && lotAux.number != null && lotAux.internalNumber != "" && lotAux.internalNumber != "") ? "-" : "") + (lotAux.internalNumber ?? "")) : lotNumberNew + "-" + lotInternalNumberNew) + "  en los detalles";
                    result = new
                    {
                        itsRepeated = 1,
                        Error = errorMessage,
                        //FormattedError = ErrorMessage(errorMessage)
                    };

                    TempData.Keep("inventoryMove");
                    TempData.Keep("inventoryMoveSaldoDetailItem");

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

            TempData.Keep("inventoryMove");
            TempData.Keep("inventoryMoveSaldoDetailItem");

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ValidateInput(false)]
        public JsonResult InventoryMoveItemDetails(int? id_inventoryMoveDetail,
            int? id_itemCurrent, string codeDocumentType, int? id_metricUnitMove, int? id_warehouse,
            int? id_warehouseLocation, int? id_warehouseEntry, int? id_warehouseLocationEntry, int? id_lot,
            bool? withLotSystem, bool? withLotCustomer, DateTime? fechaEmision, int? idInvoice)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";
            var lotReceptionDatePar = db.Setting.FirstOrDefault(fod => fod.code == "VALLOT")?.value ?? "NO";

            inventoryMove = inventoryMove ?? new InventoryMove();
            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();

            if(inventoryMove.InventoryMoveDetail.Where(x => x.id_item == id_itemCurrent).Count() == 0) 
            {
                id_lot = 0;
            }

            List<Item> items = new List<Item>();
            var lots = new List<CustomLot>();
            decimal remainingBalance = 0;
            decimal unitPriceMove = 0;
            decimal averagePrice = 0;

            if (codeDocumentType == "03")
            {
                items = db.Item.Where(w => w.isActive && (w.ItemInventory.requiresLot == ((withLotSystem ?? false) || (withLotCustomer ?? false))) ||
                                                (w.id == id_itemCurrent)).ToList();
            }
            else
            {
                if ((codeDocumentType == "05" || codeDocumentType == "32" || codeDocumentType == "129") && id_warehouse != null && id_warehouseLocation != null)
                {
                    var requiresLot = ((withLotSystem ?? false) || (withLotCustomer ?? false));

                    var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
                    {
                        requiresLot = requiresLot,
                        id_Warehouse = id_warehouse,
                        id_WarehouseLocation = id_warehouseLocation,
                        id_Item = id_itemCurrent,
                        id_ProductionLot = id_lot,
                        lotMarket = null,
                        id_productionCart = null,
                        cut_Date = fechaEmision,
                        id_company = this.ActiveCompanyId,
                        consolidado = true,
                        groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

                    }, modelSaldoProductlote: true);
                    var itemsLotSaldo = resultItemsLotSaldo.Item2;
                    //var itemsLotSaldo = new ServiceInventoryMove()
                    //    .GetSaldosProductoLote(requiresLot, id_warehouse, id_warehouseLocation, id_itemCurrent, id_lot, null, fechaEmision);

                    // Validamos la existencia de la factura
                    if (idInvoice.HasValue)
                    {
                        var idsItemFacturar = db.InvoiceDetail
                            .Where(e => e.id_invoice == idInvoice.Value && e.isActive)
                            .Select(e => e.id_item)
                            .ToArray();

                        itemsLotSaldo = itemsLotSaldo
                            .Where(e => idsItemFacturar.Contains(e.id_item))
                            .ToArray();
                    }

                    var idsItem = itemsLotSaldo
                        .Select(e => e.id_item)
                        .Distinct();

                    lots = itemsLotSaldo
                        .Select(e => new CustomLot()
                        {
                            id = e.id_lote ?? 0,
                            number = e.number
                                + (!String.IsNullOrEmpty(e.internalNumber) ? $" / {e.internalNumber}" : string.Empty)
                                + (lotMarkedPar == "SI" ? $" / {e.lot_market}" : string.Empty),
                            FechaLote = e.receptionDate,
                            FechaLoteStr = e.receptionDate.HasValue
                                ? e.receptionDate.Value.ToString("dd/MM/yyyy")
                                : string.Empty,
                            Saldo = e.saldo,
                        })
                        .ToList();
                    TempData["inventoryMoveSaldoDetailItem"] = lots;
                    TempData.Keep("inventoryMoveSaldoDetailItem");

                    remainingBalance = itemsLotSaldo.Sum(e => e.saldo);
                    items = db.Item.Where(w => idsItem.Contains(w.id)).ToList();
                }
                else
                {
                    items = db.Item.Where(w => (w.id == id_itemCurrent)).ToList();
                }
            }

            var itemAux = items.FirstOrDefault(fod => fod.id == id_itemCurrent);

            // TODO_PENDIENTE: Calcular precio Unitario

            var id_metricTypeAux = itemAux?.ItemInventory?.MetricUnit.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.id_metricType == id_metricTypeAux) || w.id == id_metricUnitMove).ToList();

            var warehouseLocations = db.WarehouseLocation.Where(w => (w.id_warehouse == id_warehouse && w.isActive) || w.id == id_warehouseLocation)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            var warehouseLocationsEntry = db.WarehouseLocation.Where(w => (w.id_warehouse == id_warehouseEntry && w.isActive) || w.id == id_warehouseLocationEntry)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            #region metricUnitInventoryPurchase

            string metricUnitPurchase = (itemAux != null && itemAux.ItemPurchaseInformation != null && itemAux.ItemPurchaseInformation.MetricUnit != null) ? itemAux.ItemPurchaseInformation.MetricUnit.code : "";
            int? id_metricUnitPurchase = (itemAux != null && itemAux.ItemPurchaseInformation != null && itemAux.ItemPurchaseInformation.MetricUnit != null) ? itemAux.ItemPurchaseInformation.MetricUnit.id : (int?)null;
            string metricUnitInventory = (itemAux != null && itemAux.ItemInventory != null && itemAux.ItemInventory.MetricUnit != null) ? itemAux.ItemInventory.MetricUnit.code : "";
            int? id_metricUnitInventory = (itemAux != null && itemAux.ItemInventory != null && itemAux.ItemInventory.MetricUnit != null) ? itemAux.ItemInventory.MetricUnit.id : (int?)null;

            var inventoryMoveDetail = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id == id_inventoryMoveDetail);
            List<InventoryMoveDetailTransfer> inventoryMoveDetailExits = inventoryMoveDetail?.InventoryMoveDetailTransfer.ToList();

            string metricUnitMovExit = metricUnitInventory;
            int? id_metricUnitMovExit = id_metricUnitInventory;

            if (inventoryMoveDetailExits != null && inventoryMoveDetailExits.Count > 0)
            {
                int? id_inventoryMoveDetailExit = inventoryMoveDetailExits[0].id_inventoryMoveDetailExit;
                InventoryMoveDetail inventoryMoveDetailExit = DataProviderInventoryMove.InventoryMoveDetail(id_inventoryMoveDetailExit);

                metricUnitMovExit = (inventoryMoveDetailExit != null && inventoryMoveDetailExit.MetricUnit1 != null) ? inventoryMoveDetailExit.MetricUnit1.code : metricUnitInventory;
                id_metricUnitMovExit = (inventoryMoveDetailExit != null && inventoryMoveDetailExit.MetricUnit1 != null) ? inventoryMoveDetailExit.MetricUnit1.id : id_metricUnitInventory;
            }

            var metricUnitInventoryPurchase = ((codeDocumentType != null && codeDocumentType.Equals("04")) ? metricUnitPurchase : ((codeDocumentType != null && codeDocumentType.Equals("34")) ? metricUnitMovExit : metricUnitInventory));
            var id_metricUnitInventoryPurchase = ((codeDocumentType != null && codeDocumentType.Equals("04")) ? id_metricUnitPurchase : ((codeDocumentType != null && codeDocumentType.Equals("34")) ? id_metricUnitMovExit : id_metricUnitInventory));

            #endregion metricUnitInventoryPurchase

            var result = new
            {
                masterCode = itemAux?.masterCode ?? "",
                lotNumber = inventoryMoveDetail?.Lot?.number ?? "",
                lotInternalNumber = inventoryMoveDetail?.Lot?.internalNumber ?? "",

                Message = "Ok",
                metricUnitInventoryPurchase = metricUnitInventoryPurchase,
                id_metricUnitInventoryPurchase = id_metricUnitInventoryPurchase,
                id_metricUnitMove = id_metricUnitMove ?? id_metricTypeAux,
                warehouseLocations = warehouseLocations,
                warehouseLocationsEntry = warehouseLocationsEntry,
                metricUnits = metricUnits.Select(s => new
                {
                    id = s.id,
                    code = s.code,
                    name = s.name
                }).ToList(),
                lots = lots,
                remainingBalance = remainingBalance,
                unitPriceMove = unitPriceMove,
                averagePrice = averagePrice
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemCombo_OnInit(int? id_inventoryMoveDetail,
            int? id_itemCurrent, string codeDocumentType, int? id_metricUnitMove,
            int? id_warehouse, int? id_warehouseLocation, int? id_warehouseEntry,
            int? id_warehouseLocationEntry, int? id_lot, bool? withLotSystem,
            bool? withLotCustomer, int? idInvoice)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            inventoryMove = inventoryMove ?? new InventoryMove();
            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();

            List<Item> items = new List<Item>();
            List<Lot> lots = new List<Lot>();
            decimal remainingBalance = 0;
            decimal unitPriceMove = 0;
            decimal averagePrice = 0;

            items = db.Item.Where(w => (w.id == id_itemCurrent)).ToList();
            if ((codeDocumentType == "05" || codeDocumentType == "32" || codeDocumentType == "129") && id_warehouse != null && id_warehouseLocation != null)
            {
                var requiresLot = (withLotSystem ?? false) || (withLotCustomer ?? false);
                var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
                {
                    requiresLot = requiresLot,
                    id_Warehouse = id_warehouse,
                    id_WarehouseLocation = id_warehouseLocation,
                    id_Item = id_itemCurrent,
                    id_ProductionLot = id_lot,
                    lotMarket = null,
                    cut_Date = null,
                    id_productionCart = null,
                    id_company = this.ActiveCompanyId,
                    consolidado = true,
                    groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

                }, modelSaldoProductlote: true);
                var saldos = resultItemsLotSaldo.Item2;

                //var saldos = new Services.ServiceInventoryMove()
                //    .GetSaldosProductoLote(requiresLot, id_warehouse, 
                //        id_warehouseLocation, id_itemCurrent, id_lot, null, null, null);

                // Validamos la existencia de la factura
                if (idInvoice.HasValue)
                {
                    var idsItemFacturar = db.InvoiceDetail
                        .Where(e => e.id_invoice == idInvoice.Value && e.isActive)
                        .Select(e => e.id_item)
                        .ToArray();

                    saldos = saldos
                        .Where(e => idsItemFacturar.Contains(e.id_item))
                        .ToArray();
                }

                var idsItem = saldos.Select(e => e.id_item).Distinct();
                items = db.Item.Where(w => idsItem.Contains(w.id)).ToList();
                remainingBalance = saldos.Sum(e => e.saldo);
            }

            // TODO: verificar temas de costo y precio unitario promedio

            var itemAux = items.FirstOrDefault(fod => fod.id == id_itemCurrent);
            var id_metricTypeAux = itemAux?.ItemInventory?.MetricUnit.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.id_metricType == id_metricTypeAux) || w.id == id_metricUnitMove).ToList();

            var warehouseLocations = db.WarehouseLocation
                .Where(w => (w.id_warehouse == id_warehouse && w.isActive) || w.id == id_warehouseLocation)
                .Select(s => new
                {
                    id = s.id,
                    name = s.name
                });

            var warehouseLocationsEntry = db.WarehouseLocation
                .Where(w => (w.id_warehouse == id_warehouseEntry && w.isActive) || w.id == id_warehouseLocationEntry)
                .Select(s => new
                {
                    id = s.id,
                    name = s.name
                });

            #region metricUnitInventoryPurchase

            string metricUnitPurchase = (itemAux != null && itemAux.ItemPurchaseInformation != null && itemAux.ItemPurchaseInformation.MetricUnit != null) ? itemAux.ItemPurchaseInformation.MetricUnit.code : "";
            int? id_metricUnitPurchase = (itemAux != null && itemAux.ItemPurchaseInformation != null && itemAux.ItemPurchaseInformation.MetricUnit != null) ? itemAux.ItemPurchaseInformation.MetricUnit.id : (int?)null;
            string metricUnitInventory = (itemAux != null && itemAux.ItemInventory != null && itemAux.ItemInventory.MetricUnit != null) ? itemAux.ItemInventory.MetricUnit.code : "";
            int? id_metricUnitInventory = (itemAux != null && itemAux.ItemInventory != null && itemAux.ItemInventory.MetricUnit != null) ? itemAux.ItemInventory.MetricUnit.id : (int?)null;

            var inventoryMoveDetail = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id == id_inventoryMoveDetail);
            List<InventoryMoveDetailTransfer> inventoryMoveDetailExits = inventoryMoveDetail?.InventoryMoveDetailTransfer.ToList();

            string metricUnitMovExit = metricUnitInventory;
            int? id_metricUnitMovExit = id_metricUnitInventory;

            if (inventoryMoveDetailExits != null && inventoryMoveDetailExits.Count > 0)
            {
                int? id_inventoryMoveDetailExit = inventoryMoveDetailExits[0].id_inventoryMoveDetailExit;
                InventoryMoveDetail inventoryMoveDetailExit = DataProviderInventoryMove.InventoryMoveDetail(id_inventoryMoveDetailExit);

                metricUnitMovExit = (inventoryMoveDetailExit != null && inventoryMoveDetailExit.MetricUnit1 != null) ? inventoryMoveDetailExit.MetricUnit1.code : metricUnitInventory;
                id_metricUnitMovExit = (inventoryMoveDetailExit != null && inventoryMoveDetailExit.MetricUnit1 != null) ? inventoryMoveDetailExit.MetricUnit1.id : id_metricUnitInventory;
            }

            var metricUnitInventoryPurchase = ((codeDocumentType != null && codeDocumentType.Equals("04")) ? metricUnitPurchase : ((codeDocumentType != null && codeDocumentType.Equals("34")) ? metricUnitMovExit : metricUnitInventory));
            var id_metricUnitInventoryPurchase = ((codeDocumentType != null && codeDocumentType.Equals("04")) ? id_metricUnitPurchase : ((codeDocumentType != null && codeDocumentType.Equals("34")) ? id_metricUnitMovExit : id_metricUnitInventory));

            #endregion metricUnitInventoryPurchase

            var result = new
            {
                masterCode = itemAux?.masterCode ?? "",
                lotNumber = inventoryMoveDetail?.Lot?.number ?? "",
                lotInternalNumber = inventoryMoveDetail?.Lot?.internalNumber ?? "",

                Message = "Ok",
                metricUnitInventoryPurchase = metricUnitInventoryPurchase,
                id_metricUnitInventoryPurchase = id_metricUnitInventoryPurchase,
                id_metricUnitMove = id_metricUnitMove,
                warehouseLocations = warehouseLocations,
                warehouseLocationsEntry = warehouseLocationsEntry,
                metricUnits = metricUnits.Select(s => new
                {
                    id = s.id,
                    code = s.code,
                    name = s.name
                }).ToList(),
                lots = lots,
                items = items.Select(s => new
                {
                    id = s.id,
                    masterCode = s.masterCode,
                    name = s.name
                }).ToList(),
                remainingBalance = remainingBalance,
                unitPriceMove = unitPriceMove,
                averagePrice = averagePrice
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSecTrans(int? id_inventoryMoveDetail, int? id_inventoryReason)
        {
            InventoryReason inventoryReason = db.InventoryReason.FirstOrDefault(i => i.id == id_inventoryReason);

            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            string secTransAux = "";
            var inventoryMoveDetailAux = inventoryMove.InventoryMoveDetail?.FirstOrDefault(fod => fod.id == id_inventoryMoveDetail);
            if (inventoryMoveDetailAux != null && inventoryMoveDetailAux.genSecTrans)
            {
                secTransAux = inventoryMoveDetailAux.Lot.number;
            }
            else
            {
                var sequentialAux = inventoryReason?.sequential;
                secTransAux = (inventoryReason?.code ?? "") + sequentialAux?.ToString("D9") ?? "";
                while (secTransAux != "" && inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod?.Lot.number == secTransAux) != null)
                {
                    sequentialAux++;
                    secTransAux = (inventoryReason?.code ?? "") + sequentialAux?.ToString("D9") ?? "";
                }
            }

            var result = new
            {
                secTrans = secTransAux
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
            var metricUnits = db.MetricUnit
                .Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.id_metricType == id_metricTypeAux))
                .ToList();

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

            var warehouseLocations = db.WarehouseLocation
                .Where(w => w.id_warehouse == id_warehouseAux)
                .Select(s => new
                {
                    id = s.id,
                    name = s.name
                });

            // TODO: verificar temas de costo y precio unitario promedio
            var unitPriceMove = 0m;
            var averagePrice = 0m;

            var requiresLot = item?.ItemInventory?.requiresLot ?? false;

            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
            {
                requiresLot = requiresLot,
                id_Warehouse = id_warehouseAux,
                id_WarehouseLocation = id_warehouseLocationAux,
                id_Item = id_item,
                id_ProductionLot = null,
                lotMarket = null,
                cut_Date = null,
                id_productionCart = null,
                id_company = this.ActiveCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE
            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;

            //var saldos = new Services.ServiceInventoryMove()
            //    .GetSaldosProductoLote(requiresLot, id_warehouseAux, id_warehouseLocationAux, id_item, null, null, null);
            var remainingBalance = saldos.Sum(e => e.saldo);
            var lots = saldos
                .Select(e => new
                {
                    id = e.id_lote,
                    number = e.number,
                });

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
                                       .Select(s => new
                                       {
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
                    if (id_warehouseLocationAux != null) i.id_warehouseLocation = id_warehouseLocationAux.Value;
                    UpdateModel(i);
                }
            }

            var result = new
            {
                id_warehouse,
                warehouse.name,
                warehouseLocations = warehouseLocations
                                    .Select(s => new
                                    {
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
        public JsonResult InvoiceChangeData(int id_warehouse, int id_document)
        {
            var id_person = db.Invoice.FirstOrDefault(a => a.id == id_document)?.id_buyer ?? 0;
            WarehouseLocation warehouseLocation = db.WarehouseLocation
                .FirstOrDefault(i => i.id_warehouse == id_warehouse && i.id_person == id_person && i.isActive);

            if (warehouseLocation == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            var result = new
            {
                id_customer = warehouseLocation.id_person,
                customer = warehouseLocation.Person.fullname_businessName
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
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       })
            };

            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateQuantityTotal(string unitPriceMove, int? id_metricUnitMove, int? id_metricUnitMoveCurrent, string amountMove, int? idWarehouse)
        {
            decimal _unitPriceMove = Convert.ToDecimal(unitPriceMove);
            decimal _amountMove = Convert.ToDecimal(amountMove);

            var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitMoveCurrent &&
                                                                                     fod.id_metricDestiny == id_metricUnitMove);

            var factor = id_metricUnitMove == null || id_metricUnitMoveCurrent == null ? 0 : (id_metricUnitMove == id_metricUnitMoveCurrent ? (1) : metricUnitConversion?.factor ?? 0);
            var unitPriceMoveAux = _unitPriceMove * factor;
            var balanceCost = unitPriceMoveAux * _amountMove;
            var metricOrigin = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitMove);
            var metricDestiny = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitMoveCurrent);
            var Message = metricOrigin == null ? ("La UM Mov. es nula") : (metricDestiny == null ? ("La UM Mov. es nula") : (factor == 0 ? ("No existe el factor de conversión entre " + metricOrigin.code + " y " + metricDestiny.code + " Configúrelo e intente de nuevo") : ("OK")));
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
            //InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            return lotAux;
        }

        private Lot CreateNewLot(string lot, string lotInternal)
        {
            Lot lotAux = null;
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region Lot

                    lotAux = ServiceLot.CreateLot(db, ActiveUser, ActiveCompany, ActiveEmissionPoint);
                    lotAux.number = lot;
                    lotAux.internalNumber = lotInternal;

                    var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");
                    lotAux.Document.id_documentState = documentState.id;
                    lotAux.Document.DocumentState = documentState;

                    db.SaveChanges();
                    trans.Commit();

                    #endregion Lot
                }
                catch (Exception e)
                {
                    TempData.Keep("inventoryMove");
                    ViewData["ErrorMessage"] = e.Message
                        + $" Base exception: {e.GetBaseException().Message}";
                    trans.Rollback();
                    return null;
                }
            }

            TempData.Keep("inventoryMove");

            return lotAux;
        }

        private Lot getLotSinTransaccion(string lot, string lotInternal)
        {
            var strLotAux = lot?.Trim() ?? "";
            var strLotInternalAux = lotInternal?.Trim() ?? "";
            if (strLotAux == "" && strLotInternalAux == "")
            {
                return null;
            }
            var lotAux = db.Lot.FirstOrDefault(fod => fod.number == strLotAux && fod.internalNumber == strLotInternalAux);
            if (lotAux == null)
            {
                lotAux = CreateNewLotSinTransaccion(strLotAux, strLotInternalAux);
            }
            //InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            return lotAux;
        }

        private Lot CreateNewLotSinTransaccion(string lot, string lotInternal)
        {
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            Lot lotAux = null;
            try
            {
                #region Lot

                lotAux = ServiceLot.CreateLot(db, ActiveUser, ActiveCompany, ActiveEmissionPoint);
                lotAux.number = lot;
                lotAux.internalNumber = lotInternal;

                var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");
                lotAux.Document.id_documentState = documentState.id;
                lotAux.Document.DocumentState = documentState;

                db.SaveChanges();

                #endregion Lot
            }
            catch (Exception e)
            {
                
                ViewData["EditError"] = ErrorMessage(e.Message);
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "InventoryMoveController", "Producion");

                lotAux= null;
            }
            finally 
            {
                TempData.Keep("inventoryMove");
            }

            

            return lotAux;
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateLot(int? id_item, int? id_warehouse, int? id_warehouseLocation, string codeDocumentType)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);
            var requiresLot = item?.ItemInventory?.requiresLot ?? false;

            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
            {
                requiresLot = requiresLot,
                id_Warehouse = id_warehouse,
                id_WarehouseLocation = id_warehouseLocation,
                id_Item = id_item,
                id_ProductionLot = null,
                lotMarket = null,
                cut_Date = null,
                id_productionCart = null,
                id_company = this.ActiveCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE
            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;

            //var saldos = new Services.ServiceInventoryMove()
            //    .GetSaldosProductoLote(requiresLot, id_warehouse, id_warehouseLocation, id_item, null, null, null);
            var lots = saldos
                .Select(s => new
                {
                    id = s.id_lote,
                    number = s.number
                });

            var remainingBalance = saldos.Sum(e => e.saldo);

            // TODO: verificar temas de costo y precio unitario promedio
            var averagePrice = 0m;

            var result = new
            {
                id_metricUnitMove = (item.ItemInventory?.id_metricUnitInventory),
                lots = lots,
                remainingBalance = remainingBalance,
                averagePrice = averagePrice
            };

            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult LotDetail(int? id_item,
            int? id_warehouse, int? id_warehouseLocation, int? id_lot,
            string lotMarked, DateTime? fechaEmision)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            int? id_metricUnitMove = null;
            decimal remainingBalance = 0;
            decimal averagePrice = 0;
            
            lotMarked = lotMarked != "" ? lotMarked : null;                
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);
            var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";

            id_lot = id_lot == 0 ? null : id_lot;
            var requiresLot = item?.ItemInventory?.requiresLot ?? false;

            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
            {
                requiresLot = requiresLot,
                id_Warehouse = id_warehouse,
                id_WarehouseLocation = id_warehouseLocation,
                id_Item = id_item,
                id_ProductionLot = id_lot,
                lotMarket = lotMarked,
                cut_Date = fechaEmision,
                id_productionCart = null,
                id_company = this.ActiveCompanyId,
                consolidado = false,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE
            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;

            remainingBalance = saldos.Sum(e => e.saldo);                             
            id_metricUnitMove = (item?.ItemInventory?.id_metricUnitInventory??0);                               
            
            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");
            TempData.Keep("inventoryMoveSaldoDetailItem");

            var result = new
            {
                id_metricUnitMove = id_metricUnitMove,
                remainingBalance = remainingBalance,
                averagePrice = averagePrice
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private decimal GetAveragePrice(List<InventoryMoveDetail> inventoryMoveDetail)
        {
            decimal amount = 0;
            decimal total = 0;
            decimal averagePrice = 0;
            var tempInventoryMoveDetail = inventoryMoveDetail.OrderBy(ob => ob.InventoryMove.sequential).ToList();
            foreach (var item in tempInventoryMoveDetail)
            {
                var natureMove = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == item.InventoryMove.idNatureMove)?.valueCode?.Trim();
                if (natureMove.Equals("I"))
                {
                    amount += item.entryAmount;
                    total += (item.entryAmount * item.unitPrice);
                    averagePrice = (amount > 0) ? (Math.Round(total / amount, 6)) : 0;
                }
                if (natureMove.Equals("E"))
                {
                    amount -= item.entryAmount;
                    total -= (item.entryAmount * averagePrice);
                    averagePrice = (amount > 0) ? (Math.Round(total / amount, 6)) : 0;
                }
            }
            return averagePrice;
        }

        [HttpPost]
        public ActionResult GetSubCostCenter(int? id_costCenterDetail, int? id_subCostCenterDetail, int? idWarehouse)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            List<CostCenter> items = new List<CostCenter>();
            if (id_costCenterDetail != null)
            {
                items = db.CostCenter.Where(w => (w.id_higherCostCenter == id_costCenterDetail && w.isActive) ||
                                                    (w.id == id_subCostCenterDetail)).ToList();
            }
            else
            {
                items = db.CostCenter.Where(w => w.id == id_subCostCenterDetail).ToList();
            }
            TempData.Keep("inventoryMove");
            TempData.Keep("inventoryMoveSaldoDetailItem");

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
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
        public ActionResult LoadSubCostCenter(int? id_costCenter, int? id_subCostCenter)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            List<CostCenter> items = new List<CostCenter>();
            if (id_costCenter != null)
            {
                items = db.CostCenter.Where(w => (w.id_higherCostCenter == id_costCenter && w.isActive) ||
                                                    (w.id == id_subCostCenter)).ToList();
            }
            else
            {
                items = db.CostCenter.Where(w => w.id == id_subCostCenter).ToList();
            }
            TempData.Keep("inventoryMove");
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_subCostCenter";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "InventoryMove", Action = "LoadSubCostCenter" };
                p.ClientSideEvents.BeginCallback = "OnSubCostCenter_BeginCallback";
                p.CallbackPageSize = 5;
                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);
                p.BindList(items);
            });
        }

        [HttpPost]
        public ActionResult GetItems(int? id_itemIni,
            bool? withLotSystem, bool? withLotCustomer, string code, int? idWarehouse, int? idWarehouseLocation,
            int? indice, DateTime? fechaEmision, int? idInvoice,
            int? id_itemType, int? id_size, int? id_trademark, int? id_trademarkModel, int? id_presentation, string codigoProducto, int? categoriaProducto,
            int? modeloProducto)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            List<Item> items = new List<Item>();
            if (code == "03")
            {
                items = db.Item.Where(w => w.isActive && (w.ItemInventory.requiresLot == ((withLotSystem ?? false) || (withLotCustomer ?? false))) ||
                                                (w.id == id_itemIni)).ToList();
            }
            else
            {
                if ((code == "05" || code == "32" || code == "129") && idWarehouse != null && idWarehouseLocation != null)
                {
                    var requiresLot = ((withLotSystem ?? false) || (withLotCustomer ?? false));


                    var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
                    {
                        requiresLot = requiresLot,
                        id_Warehouse = idWarehouse,
                        id_WarehouseLocation = idWarehouseLocation,
                        id_Item = id_itemIni,
                        id_ProductionLot = null,
                        lotMarket = null,
                        cut_Date = fechaEmision,
                        id_productionCart = null,
                        id_company = this.ActiveCompanyId,
                        consolidado = true,
                        groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE
                    }, modelSaldoProductlote: true);
                    var itemsLotSaldoP = resultItemsLotSaldo.Item2;
                    var itemsLotSaldo = itemsLotSaldoP
                                        .Select(e => e.id_item)
                                        .Distinct();

                    //var itemsLotSaldo = new ServiceInventoryMove()
                    //    .GetSaldosProductoLote(requiresLot, idWarehouse, idWarehouseLocation, id_itemIni, null, null, fechaEmision)
                    //    .Select(e => e.id_item)
                    //    .Distinct();

                    // Validamos la existencia de la factura
                    if (idInvoice.HasValue)
                    {
                        var idsItemFacturar = db.InvoiceDetail
                            .Where(e => e.id_invoice == idInvoice.Value && e.isActive)
                            .Select(e => e.id_item)
                            .ToArray();

                        itemsLotSaldo = itemsLotSaldo
                            .Where(e => idsItemFacturar.Contains(e))
                            .ToArray();
                    }

                    items = db.Item
                        .Where(w => w.isActive && itemsLotSaldo.Contains(w.id))
                        .ToList();
                }
                else
                {
                    items = db.Item.Where(w => (w.id == id_itemIni)).ToList();
                }
            }

            if (id_size != null && id_size > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_size == id_size).ToList();
            }
            if (id_itemType != null && id_itemType > 0)
            {
                items = items
                    .Where(w => w.id_itemType == id_itemType).ToList();
            }

            if (id_trademark != null && id_trademark > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_trademark == id_trademark).ToList();
            }

            if (modeloProducto != null && modeloProducto > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_trademarkModel == modeloProducto).ToList();
            }

            if (categoriaProducto != null && categoriaProducto > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_groupCategory == categoriaProducto).ToList();
            }

            if (id_presentation != null && id_presentation > 0)
            {
                items = items
                    .Where(w => w.id_presentation == id_presentation).ToList();
            }

            if (!String.IsNullOrEmpty(codigoProducto))
            {
                items = items
                    .Where(w => w.masterCode.Contains(codigoProducto)).ToList();
            }

            TempData.Keep("inventoryMove");
            TempData.Remove("inventoryMoveSaldoDetailItem");

            var name = String.Concat("ItemDetail", indice ?? 0);
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = name;
                p.ValueField = "id";
                p.ValueType = typeof(int);
                p.TextFormatString = "{0},{1}";
                p.CallbackPageSize = 30;
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                p.IncrementalFilteringDelay = 250;

                p.Columns.Add("masterCode", "Código Auxiliar", Unit.Percentage(200));
                p.Columns.Add("name", "Nombre del Producto", Unit.Percentage(800));

                p.ClientSideEvents.Init = "ItemCombo_OnInit";
                p.ClientSideEvents.BeginCallback = "OnItemDetailBeginCallback";
                p.ClientSideEvents.EndCallback = "OnItemDetailEndCallback";
                p.ClientSideEvents.SelectedIndexChanged = "DetailsItemsCombo_SelectedIndexChanged";

                p.CallbackRouteValues = new { Controller = "InventoryMove", Action = "GetItems" };

                p.Width = Unit.Percentage(100);

                p.BindList(items);
            });
        }

        [HttpPost]
        public ActionResult FillLotDiff(InventoryMoveDetail inventoryMoveDetail,
                                            string code,
                                            int? idWarehouse,
                                            DateTime fechaEmision,
                                            bool? withLotSystem,
                                            bool? withLotCustomer,
                                            bool? forceResetLots
                                            )
        {
            ViewBag.idWarehouse = idWarehouse;
            ViewBag.withLotSystem = withLotSystem;
            ViewBag.withLotCustomer = withLotCustomer;
            ViewBag.forceresetlots = (forceResetLots ?? false);

            ViewData["code"] = code;
            ViewData["fechaEmision"] = fechaEmision;
            

            TempData.Keep("inventoryMoveSaldoDetailItem");
            TempData.Keep("inventoryMove");

            return PartialView("ComponentsDetail/_ComboBoxProductionLot", inventoryMoveDetail);
        }


        #endregion AUXILIAR FUNCTIONS

        #region New Version

        [HttpPost]
        public void RefresshDataForEditForm(int? idWarehouse)
        {
        }

        #endregion New Version

        [HttpPost, ValidateInput(false)]
        private bool ValidaLote(int id_im)
        {
            bool dat = false;
            var inventoryMove = db.InventoryMove.FirstOrDefault(i => i.id == id_im);
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
                    id_lot = detail.id_lot,
                };

                if (tempDetail.id_lot != null)
                    dat = true;
            }
            return dat;
        }

        [HttpPost, ValidateInput(false)]
        public bool ValidarTipoTransferencia(int id_im)
        {
            var motivo = db.InventoryMove.FirstOrDefault(e => e.id == id_im) ?? new InventoryMove();

            var isTransfer = motivo?.InventoryReason?.isForTransfer ?? false;
            return isTransfer;
        }

        #region Reports

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReportInventoryMove(int id_im, string codeReport, string codeNatureMove, string documentType)
        {
            var inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            TempData["inventoryMove"] = inventoryMove;
            TempData.Keep("inventoryMove");

            ViewData["_natureMove"] = codeNatureMove;

            if (ValidarTipoTransferencia(id_im) && ValidaLote(id_im))
            {
                if (codeNatureMove.Equals("I"))
                {
                    codeReport = "IDMIT";
                }
                else
                {
                    codeReport = "IDMET";
                }
            }
            else
            {
                if (ValidaLote(id_im))
                {
                    if (inventoryMove.id_inventoryReason.HasValue && ValidaMotivoInventarioRequiereLote(inventoryMove.id_inventoryReason.Value))
                    {
                        codeReport = "IDGV9";
                    }
                    else
                    {
                        codeReport = "IDGVM";
                    }
                }
            }

            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_im;
            paramLst.Add(_param);

            if (codeReport == "IDGVM" || codeReport == "IDMIT" || codeReport == "IDMET" || codeReport == "IDGV9")
            {
                _param = new ParamCR();
                _param.Nombre = "@id_user";
                _param.Valor = this.ActiveUserId;
                paramLst.Add(_param);
            }

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep;

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion "Armo Parametros"
        }

        #endregion Reports

        #region InventoryMovePackaging

        [HttpPost]
        public ActionResult IndexExitMovePackagingMaterials(string natureMove)
        {
            InventoryMove model = new InventoryMove();

            try
            {
                var paramPackagingMaterials = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("IPXM") as List<AdvanceParametersDetailModelP>;
                if (paramPackagingMaterials == null) throw new Exception("No se han definido parámetros para Egreso de Materiales de Empaque");

                var datInventoryReason = paramPackagingMaterials.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "INRS");
                if (datInventoryReason == null) throw new Exception("No se han definido parámetros de Razón de Inventario para Egreso de Materiales de Empaque");

                var datDocumentType = paramPackagingMaterials.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "DTIM");
                if (datDocumentType == null) throw new Exception("No se han definido parámetros de Tipo de Documento para Egreso de Materiales de Empaque");

                var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
                if (lstNatureMove == null) throw new Exception("No se han definido parámetros de naturaleza de Documento para Egreso de Materiales de Empaque");

                var exitNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("E"));
                if (exitNatureMove == null) throw new Exception("No se ha definido parámetro egreso para Egreso de Materiales de Empaque");

                string codInventoryReason = datInventoryReason.nameAdvanceDetailModelP;
                string codDocumentType = datDocumentType.nameAdvanceDetailModelP;

                InventoryReason _inventoryReason = db.InventoryReason.FirstOrDefault(r => r.code == codInventoryReason);
                if (_inventoryReason == null) throw new Exception("No existe definición de Egreso de Materiales de Empaque en las razones de inventario");

                model = new InventoryMove
                {
                    Document = new Document
                    {
                        DocumentType = db.DocumentType.FirstOrDefault(t => t.code.Equals(codDocumentType))
                    }
                };

                model.idNatureMove = exitNatureMove.idAdvanceDetailModelP;
                model.id_inventoryReason = _inventoryReason.id;

                ViewData["_inventoryReason"] = codInventoryReason;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = ErrorMessage("Error al cargar el Formulario.");
                LogWrite(e, null, "IndexExitMovePackagingMaterials");
            }
            finally
            {
                ViewData["_natureMove"] = "E";
                ViewData["_customParamOP"] = "IPXM";
            }

            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult GetLoteFilter(int? id_Warehouse)
        {
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            tempInventoryMove.idWarehousePackagingMaterials = id_Warehouse;

            TempData.Keep("inventoryMove");
            return PartialView("_InventoryExitMovePackaingLote", tempInventoryMove);
        }

        [HttpPost]
        public ActionResult GetLocationFilter(int? idWareHouse)
        {
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            tempInventoryMove.idWarehouse = idWareHouse;

            TempData.Keep("inventoryMove");
            return PartialView("_InventoryExitMovePackaingLocation", tempInventoryMove);
        }

        [HttpPost]
        public ActionResult GetCostSubCenterFilter(int? id_CostCenter)
        {
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            tempInventoryMove.idCostCenterPackagingMaterials = id_CostCenter;

            TempData.Keep("inventoryMove");
            return PartialView("_InventoryExitMovePackaingCostSubCenter", tempInventoryMove);
        }

        [HttpPost]
        public ActionResult GetItemFilter(int? id_ProductionLot)
        {
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            tempInventoryMove.idLotePackagingMaterials = id_ProductionLot;
            TempData.Keep("inventoryMove");
            return PartialView("_InventoryExitMovePackaingItem", tempInventoryMove);
        }

        [HttpPost]
        public JsonResult SaveInventoryItemPackagingMaterials(
            string code, string natureMoveType, int idProdutionLot, int idItemMaster, decimal quantity,
            int idWarehouse, int idLocation, int idCostCenter, int idSubCostCenter)
        {
            InventoryMove tempInventoryMove = (TempData["inventoryMove"] as InventoryMove);
            Random rnd = new Random();
            GenericResultJson oJsonResult = new GenericResultJson();

            try
            {
                tempInventoryMove.InventoryMoveExitPackingMaterial = new InventoryMoveExitPackingMaterial();
                tempInventoryMove.InventoryMoveExitPackingMaterial.id_ItemMaster = idItemMaster;
                tempInventoryMove.InventoryMoveExitPackingMaterial.quantityExit = quantity;

                ItemIngredientCalculate _itemIngredientCalculate = db.ItemHeadIngredient.CalculateDosage(idItemMaster, quantity, db);

                tempInventoryMove.InventoryMoveDetail = _itemIngredientCalculate
                    .ItemIngredientFinal
                    .Select(r => new InventoryMoveDetail
                    {
                        id = rnd.Next(-9999999, -999),
                        id_lot = idProdutionLot,
                        id_item = r.idItemCalculate,
                        exitAmount = r.amountCalculate,
                        id_metricUnit = r.idMetricUnitCalculate,
                        id_warehouse = idWarehouse,
                        id_warehouseLocation = idLocation,
                        id_userCreate = ActiveUser.id,
                        id_userUpdate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        dateUpdate = DateTime.Now,
                        id_metricUnitMove = r.idMetricUnitCalculate,
                        amountMove = r.amountCalculate,
                        id_costCenter = idCostCenter,
                        id_subCostCenter = idSubCostCenter,
                        unitPriceMove = 0,
                        Item = db.Item.FirstOrDefault(s => s.id == r.idItemCalculate)
                    })
                    .ToList();

                TempData["inventoryMove"] = tempInventoryMove;

                oJsonResult.codeReturn = 1;
                oJsonResult.message = SuccessMessage("");
            }
            catch (Exception e)
            {
                oJsonResult.codeReturn = -1;
                oJsonResult.message = ErrorMessage("Error al calcular la dosificación: " + e.Message);
                LogWrite(e, null, "CalculateItemPackagingMaterials");
            }
            finally
            {
                TempData.Keep("inventoryMove");
            }

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        #endregion InventoryMovePackaging

        private bool ValidaMotivoInventarioRequiereLote(int idInventoryReason)
        {
            bool valid = false;
            var inventoryReason = db.InventoryReason
                                        .FirstOrDefault(r => r.id == idInventoryReason);

            if (inventoryReason != null)
            {
                if ((inventoryReason.requiereSystemLotNubmber ?? false) == true || (inventoryReason.requiereUserLotNubmber ?? false) == true)
                {
                    valid = true;
                }
            }

            return valid;
        }



        #region Métodos de consultas adicionales
        [HttpPost]
        [ActionName("query-person-rol")]
        public PartialViewResult QueryPersonRol(int? idCompany, int? idPerson, string tipo, bool? viewInvoice)
        {
            return this.PartialView(
                "ComboBoxes/_PersonRolComboBoxPartial",
                GetPersonRolComboBoxModel(idCompany, idPerson, tipo, (viewInvoice ?? false)));
        }
        public static IEnumerable GetInvoiceFromProvider(int? idCompany, int? idPerson/*, int? idInvoice*/)
        {
            return Array.Empty<Invoice>();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryMoveValidateInvoice(int? id_customer, string codeState)
        {
            return PartialView("ComboBoxes/_InvoiceProviderComboBoxPartial", new InventoryMoveInvoiceDto
            {
                id_customer = id_customer,
                codeState = codeState
            });
        }
        public static PersonRolComboBoxModel GetPersonRolComboBoxModel(int? idCompany, int? idPerson, string tipo, bool viewInvoice = false)
        {
            if (tipo == "cliente")
            {
                var personOBJ = new PersonRolComboBoxModel()
                {
                    Name = "id_customer",
                    CallbackRouteValues = new { Controller = "inventoryMove", Action = "query-person-rol" },
                    ClientSideEvents = new DevExpress.Web.ComboBoxClientSideEvents()
                    {
                        BeginCallback = "OnCustomerBeginCallback",
                        Validation = "OnCustomerValidation",
                        //SelectedIndexChanged = (viewInvoice) ? "OnIdCustomer_SelectedIndexChanged" : null,
                        SelectedIndexChanged = "OnIdCustomer_SelectedIndexChanged"
                    },
                    IdCompany = idCompany,
                    IdPerson = idPerson,
                    OpViewInvoice = viewInvoice,
                    Rols = new[] {
                        DataProviderRol.m_RolClienteExterior,
                        DataProviderRol.m_RolClienteLocal,
                    },
                    CustomProperties = new Dictionary<string, object>
                    {
                        { "IdCompany", idCompany },
                        { "OpViewInvoice", viewInvoice },
                    },
                };
                return personOBJ;
            }
            else
            {
                return null;
            }
        }

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
            return PartialView("ComponentsDetail/_ComboBoxItemType");
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
            return PartialView("ComponentsDetail/_ComboBoxTrademark");
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
            return PartialView("ComponentsDetail/_ComboBoxPresentation");
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
            return PartialView("ComponentsDetail/_ComboBoxSize");
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
            return PartialView("ComponentsDetail/_ComboBoxItemTrademarkModel");
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
            return PartialView("ComponentsDetail/_ComboBoxItemGroupCategory");
        }

        #endregion

        #region Generación de clases auxiliares

        //public class CustomLot : Lot
        //{
        //    public DateTime? FechaLote { get; set; }
        //    public string FechaLoteStr { get; set; }
        //    public decimal Saldo { get; set; }
        //}

        #endregion Generación de clases auxiliares

        #region Generación de clase detalles DTO
        public class InventoryMoveDetailDTO
        {
            public int Id { get; set; }
            public string NumEgresoTransferencia { get; set; }
            public string NumOrdenCompra { get; set; }
            public string NombreBodegaEgreso { get; set; }
            public string NombreUbicacionEgreso { get; set; }
            public string NombreBodegaDetalle { get; set; }
            public string NombreUbicacionDetalle { get; set; }
            public string NombreBodegaIngreso { get; set; }
            public string NombreUbicacionIngreso { get; set; }
            public string MasterCodeProducto { get; set; }
            public string NombreProducto { get; set; }
            public string NombreUnidadMedidaCompra { get; set; }
            public string NombreCentoCosto { get; set; }
            public string NombreSubCentoCosto { get; set; }
            public decimal? AmountMove { get; set; }
            public string NombreUnidadMedidaItem { get; set; }
            public string NombreUnidadMedidaMovimiento { get; set; }
            public decimal? UnitPriceMove { get; set; }
            public decimal? BalanceCost { get; set; }
            public string Lote { get; set; }
            public string NumeroLoteMarcado { get; set; }
            public string SecTransaccionalLote { get; set; }
            public string NumeroInternoLote { get; set; }
            public bool GeneraSecuenciaTransaccional { get; set; }
            public decimal? CompraAprobada { get; set; }
            public decimal? CompraRecibida { get; set; }
            public decimal? Pendiente { get; set; }
            public decimal? Saldo { get; set; }
            public string OrdenProduccion { get; set; }
        }

        private void ConvertToDTO(IEnumerable<InventoryMoveDetail> details, DateTime? fechaEmision = null)
        {
            if (details == null)
            {
                TempData["inventoryMoveDetailDTO"] = new InventoryMoveDetail[] { };
                return;
            }

            #region Optimizacion Aprobacion, Uso Tabla Saldo             
            SaldoProductoLote[] SaldoValidaAprobacion = null;
            if ((details?.Count() ?? 0) > 0)
            {
                SaldoValidaAprobacion = ServiceInventoryMove
                                                        .GetRemainingBalanceBulk(ActiveCompany.id,
                                                                                    (details.FirstOrDefault()?.id_warehouse ?? 0),
                                                                                    details.Select(r => new SaldoProductoLote
                                                                                    {
                                                                                        id_item = r.id_item,
                                                                                        id_warehouseLocation = r.id_warehouseLocation,
                                                                                        id_lote = r.id_lot

                                                                                    }).ToArray(),
                                                                                    db,
                                                                                    null);
            }


            #endregion
            var retorno = new List<InventoryMoveDetailDTO>();
            foreach (var detail in details)
            {
                // Verificamos los Nº Egreso por transferencias
                string numEgresoTransferencia = string.Empty;
                var inventoryMoveDetailsTransfer1 = detail.InventoryMoveDetailTransfer1 != null
                    ? detail.InventoryMoveDetailTransfer1.ToList()
                    : new List<InventoryMoveDetailTransfer>();

                var inventoryMoveDetailTransfer1 = inventoryMoveDetailsTransfer1.FirstOrDefault();
                if (inventoryMoveDetailTransfer1 != null)
                {
                    var id_inventoryMoveExit = inventoryMoveDetailTransfer1?.id_inventoryMoveExit;
                    var inventoryMove = this.GetTempDocument(id_inventoryMoveExit);

                    if (inventoryMove != null)
                        numEgresoTransferencia += inventoryMove.number;

                    if (!inventoryMoveDetailTransfer1?.id_inventoryMoveDetailExit.HasValue ?? false
                        && inventoryMove.DocumentState.code != "03")
                        numEgresoTransferencia = "<font color='red'>" + numEgresoTransferencia + "</font>";
                }

                // Verificamos los Nº Ordenes de Compra
                string numOrdenCompra = string.Empty;
                decimal? compraAprobada = null, cantidadRecibida = null, cantidadPendiente = null;
                var inventoryMoveDetailsPurchaseOrder = detail.InventoryMoveDetailPurchaseOrder != null
                    ? detail.InventoryMoveDetailPurchaseOrder.ToList()
                    : new List<InventoryMoveDetailPurchaseOrder>();

                var inventoryMoveDetailPurchaseOrder = inventoryMoveDetailsPurchaseOrder.FirstOrDefault();
                if (inventoryMoveDetailPurchaseOrder != null)
                {
                    int id_purchaseOrder = inventoryMoveDetailPurchaseOrder.id_purchaseOrder;
                    var purchaseOrder = this.GetTempDocument(id_purchaseOrder);
                    if (purchaseOrder != null)
                        numOrdenCompra += purchaseOrder.number;

                    numOrdenCompra += (inventoryMoveDetailsPurchaseOrder.Count > 1)
                        ? @"&nbsp<a href=""#"">+" + (inventoryMoveDetailsPurchaseOrder.Count - 1).ToString() + "</a>" : "";

                    compraAprobada = inventoryMoveDetailPurchaseOrder.PurchaseOrderDetail?.quantityApproved;
                    cantidadRecibida = inventoryMoveDetailPurchaseOrder.PurchaseOrderDetail?.quantityReceived;
                    cantidadPendiente = (compraAprobada - cantidadRecibida) > 0
                        ? (compraAprobada - cantidadRecibida) : 0m;
                }

                var bodegaEgreso = this.GetTempWarehouse(inventoryMoveDetailTransfer1?.id_warehouseExit);
                var ubicacionEgreso = this.GetTempWarehouseLocation(inventoryMoveDetailTransfer1?.id_warehouseLocationExit);
                var bodegaDetalle = this.GetTempWarehouse(detail.id_warehouse);
                var ubicacionDetalle = this.GetTempWarehouseLocation(detail.id_warehouseLocation);
                var bodegaIngreso = this.GetTempWarehouse(detail.id_warehouseEntry);
                var ubicacionIngreso = this.GetTempWarehouseLocation(detail.id_warehouseLocationEntry);
                var item = this.GetTempItem(detail.id_item);

                var unidadMedidaItem = ((item.ItemInventory != null) && (item.ItemInventory?.MetricUnit != null))
                    ? item.ItemInventory.MetricUnit.code : "";
                var unidadMedidaItemCompra = ((item.ItemPurchaseInformation != null) && (item.ItemPurchaseInformation?.MetricUnit != null))
                    ? item.ItemPurchaseInformation.MetricUnit.code : "";

                // Recuperamos la unidad de medida del movimiento
                string unidadMedidaMovimiento = string.Empty;
                var inventoryMoveDetailsTransfer = detail.InventoryMoveDetailTransfer != null
                    ? detail.InventoryMoveDetailTransfer.ToList()
                    : new List<InventoryMoveDetailTransfer>();

                var inventoryMoveDetailTransfer = inventoryMoveDetailsTransfer.FirstOrDefault();
                if (inventoryMoveDetailTransfer?.id_inventoryMoveDetailExit.HasValue ?? false)
                {
                    var detalleInventario = this.GetTempInventoryMoveDetail(inventoryMoveDetailTransfer.id_inventoryMoveDetailExit);
                    unidadMedidaMovimiento = detalleInventario != null && detalleInventario?.MetricUnit != null
                        ? detalleInventario.MetricUnit.code : string.Empty;
                }

                var centroCosto = this.GetTempCostCenter(detail.id_costCenter);
                var subCentroCosto = this.GetTempCostCenter(detail.id_subCostCenter);

                // Procesamos el lote
                var lot = this.GetTempLot(detail.id_lot);
                var productionLot = this.GetTempProductionLot(detail.id_lot);
                var internalNumberAux = ((lot != null) && (!String.IsNullOrEmpty(lot?.internalNumber)))
                    ? lot.internalNumber : productionLot?.internalNumber ?? string.Empty;
                var loteConcatenado = (lot != null)
                    ? lot.number + (!String.IsNullOrEmpty(internalNumberAux) ? "-" : "") + internalNumberAux
                    : string.Empty;
                var secTransaccional = productionLot != null
                    ? productionLot.number : (lot != null) ? lot.number : string.Empty;

                // Recuperamos el saldo
                #region Optimizacion Aprobacion, Uso Tabla Saldo      
                decimal remainingBalance = (SaldoValidaAprobacion?
                                                          .Where(r => r.id_warehouseLocation == detail.id_warehouseLocation
                                                                      && r.id_lote == detail.id_lot
                                                                      && r.id_item == detail.id_item)?
                                                          .Sum(r => r.saldo) ?? 0);
                #endregion
                //decimal remainingBalance = DataProviderInventoryMove
                //                                        .GetRemainingBalance(   this.ActiveCompanyId, 
                //                                                                detail.id_item, 
                //                                                                detail.id_warehouse, 
                //                                                                detail.id_warehouseLocation,
                //                                                                detail.id_lot, 
                //                                                                detail.lotMarked, 
                //                                                                fechaEmision);

                // recuperamos información d ela compra aprobada
                retorno.Add(new InventoryMoveDetailDTO()
                {
                    Id = detail.id,
                    NumEgresoTransferencia = numEgresoTransferencia,
                    NumOrdenCompra = numOrdenCompra,
                    NombreBodegaEgreso = bodegaEgreso?.name,
                    NombreUbicacionEgreso = ubicacionEgreso?.name,
                    NombreBodegaDetalle = bodegaDetalle?.name,
                    NombreUbicacionDetalle = ubicacionDetalle?.name,
                    NombreBodegaIngreso = bodegaIngreso?.name,
                    NombreUbicacionIngreso = ubicacionIngreso?.name,
                    MasterCodeProducto = item?.masterCode,
                    NombreProducto = item?.name,
                    NombreUnidadMedidaItem = unidadMedidaItem,
                    NombreUnidadMedidaMovimiento = unidadMedidaMovimiento,
                    NombreUnidadMedidaCompra = unidadMedidaItemCompra,
                    NombreCentoCosto = centroCosto?.name,
                    NombreSubCentoCosto = subCentroCosto?.name,
                    AmountMove = detail.amountMove,
                    UnitPriceMove = detail.unitPriceMove ?? 0m,
                    BalanceCost = detail.unitPrice * detail.amountMove,
                    Lote = loteConcatenado,
                    NumeroLoteMarcado = detail.lotMarked,
                    SecTransaccionalLote = secTransaccional,
                    NumeroInternoLote = internalNumberAux,
                    GeneraSecuenciaTransaccional = detail.genSecTrans,
                    CompraAprobada = compraAprobada,
                    CompraRecibida = cantidadRecibida,
                    Pendiente = cantidadPendiente,
                    Saldo = remainingBalance,
                    OrdenProduccion = detail.ordenProduccion,
                });
            }

            TempData["inventoryMoveDetailDTO"] = retorno.ToArray();
        }

        #region Métodos de asignación recuperación de memoria temporal

        private Document GetTempDocument(int? idDocument)
        {
            if (idDocument.HasValue)
            {
                var key = $"Document_{idDocument}";
                var data = TempData[key];

                if (data != null)
                {
                    return data as Document;
                }
                else
                {
                    var result = db.Document.FirstOrDefault(e => e.id == idDocument);

                    if (result != null)
                        TempData[key] = result;

                    return result;
                }
            }

            return null;
        }
        private Warehouse GetTempWarehouse(int? idWarehouse)
        {
            if (idWarehouse.HasValue)
            {
                var key = $"Warehouse_{idWarehouse}";
                var data = TempData[key];
                if (data != null)
                {
                    return data as Warehouse;
                }
                else
                {
                    var result = db.Warehouse.FirstOrDefault(e => e.id == idWarehouse);

                    if (result != null)
                        TempData[key] = result;

                    return result;
                }
            }

            return null;
        }
        private WarehouseLocation GetTempWarehouseLocation(int? idWarehouseLocation)
        {
            if (idWarehouseLocation.HasValue)
            {
                var key = $"WarehouseLocation_{idWarehouseLocation}";
                var data = TempData[key];
                if (data != null)
                {
                    return data as WarehouseLocation;
                }
                else
                {
                    var result = db.WarehouseLocation.FirstOrDefault(e => e.id == idWarehouseLocation);

                    if (result != null)
                        TempData[key] = result;

                    return result;
                }
            }

            return null;
        }
        private Item GetTempItem(int? idItem)
        {
            if (idItem.HasValue)
            {
                var key = $"Item_{idItem}";

                var data = TempData[key];
                if (data != null)
                {
                    return data as Item;
                }
                else
                {
                    var result = db.Item.FirstOrDefault(e => e.id == idItem);
                    if (result != null)
                        TempData[key] = result;

                    return result;
                }
            }

            return null;
        }
        private InventoryMoveDetail GetTempInventoryMoveDetail(int? id)
        {
            if (id.HasValue)
            {
                var key = $"InventoryMoveDetail_{id}";
                var data = TempData[key];

                if (data != null)
                {
                    return data as InventoryMoveDetail;
                }
                else
                {
                    var result = db.InventoryMoveDetail.FirstOrDefault(e => e.id == id);

                    if (result != null)
                        TempData[key] = result;

                    return result;
                }
            }

            return null;
        }
        private CostCenter GetTempCostCenter(int? id)
        {
            if (id.HasValue)
            {
                var key = $"CostCenter_{id}";
                var data = TempData[key];

                if (data != null)
                {
                    return data as CostCenter;
                }
                else
                {
                    var result = db.CostCenter.FirstOrDefault(e => e.id == id);

                    if (result != null)
                        TempData[key] = result;

                    return result;
                }
            }

            return null;
        }
        private ProductionLot GetTempProductionLot(int? id)
        {
            if (id.HasValue)
            {
                var key = $"ProductionLot_{id}";
                var data = TempData[key];

                if (data != null)
                {
                    return data as ProductionLot;
                }
                else
                {
                    var result = db.ProductionLot.FirstOrDefault(e => e.id == id);

                    if (result != null)
                        TempData[key] = result;

                    return result;
                }
            }

            return null;
        }
        private Lot GetTempLot(int? id)
        {
            if (id.HasValue)
            {
                var key = $"Lot_{id}";
                var data = TempData[key];

                if (data != null)
                {
                    return data as Lot;
                }
                else
                {
                    var result = db.Lot.FirstOrDefault(e => e.id == id);

                    if (result != null)
                        TempData[key] = result;

                    return result;
                }
            }

            return null;
        }
        #endregion
        #endregion
    }
}
