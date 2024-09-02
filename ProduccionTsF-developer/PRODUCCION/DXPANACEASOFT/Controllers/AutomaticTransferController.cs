using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.Models.InventoryBalance;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utilitarios.Logs;
using Utilitarios.ProdException;
using static DXPANACEASOFT.Services.ServiceInventoryBalance;
using static DXPANACEASOFT.Services.ServiceInventoryMove;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class AutomaticTransferController : DefaultController
    {
        private const string m_TipoDocumentoAutomaticTransfer = "154";

        public ActionResult Index()
        {
            BuildViewDataIndex();
            return View();
        }

        #region <<Consulta >>

        [HttpPost]
        public ActionResult SearchResult(AutomaticTransferFilterDTO consult)
        {
            var result = GetListsConsultDto(consult);
            SetAutomaticTransferResultConsultDTO(result);
            return PartialView("ConsultResult", result);
        }

        private List<AutomaticTransferResultDTO> GetListsConsultDto(AutomaticTransferFilterDTO consulta)
        {
            using (var db = new DBContext())
            {
                var consultResult = db.AutomaticTransfer.Where(AutomaticTransferQueryExtensions.GetRequestByFilter(consulta)).ToList();
                int id_menu = (int)ViewData["id_menu"];
                var tienePermisioConciliar = this.ActiveUser
                    .UserMenu.FirstOrDefault(e => e.id_menu == id_menu)?
                    .Permission?.FirstOrDefault(p => p.name == "Conciliar");

                var query = consultResult.Select(t => new AutomaticTransferResultDTO
                {
                    id = t.id,
                    number = t.Document.number,
                    dateEmission = t.Document.emissionDate,
                    warehouseExit = t.Warehouse1?.name,
                    warehouseLocationExit = t.WarehouseLocation1?.name,
                    numberExit = t.InventoryMove1?.natureSequential,
                    warehouseEntry = t.Warehouse?.name,
                    warehouseLocationEntry = t.WarehouseLocation?.name,
                    numberEntry = t.InventoryMove?.natureSequential,
                    dispatcher = t.User?.Employee?.Person?.fullname_businessName,
                    stateDocument = t.Document?.DocumentState?.name,
                    canEdit = t.Document.DocumentState.code.Equals("01"),
                    canAproved = t.Document.DocumentState.code.Equals("01"),
                    canAnnul = t.Document.DocumentState.code.Equals("01"),
                    canReverse = t.Document.DocumentState.code == "16"
                        ? t.Document.DocumentState.code.Equals("03") && (tienePermisioConciliar != null)
                        : t.Document.DocumentState.code.Equals("03")
                }).OrderByDescending(ob => ob.id).ToList();

                return query;
            }
        }

        private void SetAutomaticTransferResultConsultDTO(List<AutomaticTransferResultDTO> automaticTransferResultConsult)
        {
            Session["AutomaticTransferResultConsultDTO"] = automaticTransferResultConsult;
        }

        private List<AutomaticTransferResultDTO> GetAutomaticTransferResultConsultDTO()
        {
            if (!(Session["AutomaticTransferResultConsultDTO"] is List<AutomaticTransferResultDTO> automaticTransferResultConsult))
                automaticTransferResultConsult = new List<AutomaticTransferResultDTO>();
            return automaticTransferResultConsult;
        }

        #endregion <<Consulta >>

        #region << Private Methods >>

        private void BuildViewDataIndex()
        {
            BuildComboBoxState();
            BuildComboBoxReasonIndex("CmbBoxReasonExit");
            BuildComboBoxReasonIndex("CmbBoxReasonEntry");
            BuildComboBoxWarehouseIndex("CmbBoxWarehouseExit");
            BuildComboBoxWarehouseIndex("CmbBoxWarehouseEntry");
            BuildComboBoxUserDispatcher();
            BuildComboBoxItem();
        }

        private void BuilViewBag(bool enabled)
        {
            var automaticTransferDTO = GetAutomaticTransferDTO();
            ViewBag.enabled = enabled;
            ViewBag.canNew = automaticTransferDTO.id != 0;
            ViewBag.canCopy = automaticTransferDTO.id != 0 &&
                ((db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == automaticTransferDTO.idState)
                       ?.code.Equals("01") ?? false) ||
                    (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == automaticTransferDTO.idState)
                       ?.code.Equals("03") ?? false));
            ViewBag.canEdit = !enabled &&
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == automaticTransferDTO.idState)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == automaticTransferDTO.idState)
                                     ?.code.Equals("01") ?? false) && automaticTransferDTO.id != 0;            
            ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == automaticTransferDTO.idState)
                                      ?.code.Equals("01") ?? false) && automaticTransferDTO.id != 0;
            ViewBag.canConciliate = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == automaticTransferDTO.idState)
                                     ?.code.Equals("03") ?? false) && !enabled;

            var estado = db.DocumentState.FirstOrDefault(e => e.id == automaticTransferDTO.idState);

            int id_menu = (int)ViewData["id_menu"];
            var tienePermisioConciliar = this.ActiveUser
                .UserMenu.FirstOrDefault(e => e.id_menu == id_menu)?
                .Permission?.FirstOrDefault(p => p.name == "Conciliar");

            var estadosReverso= new[] { "03", "16" };
            var puedeReversar = db.DocumentState
                .Any(e => estadosReverso.Contains(e.code)
                    && e.id == automaticTransferDTO.idState) && automaticTransferDTO.id != 0;

            ViewBag.canReverse = estado.code == "16"
                ? puedeReversar && !enabled && tienePermisioConciliar != null
                : puedeReversar && !enabled;

            ViewBag.canShowButton = enabled && ((db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == automaticTransferDTO.idState)
                       ?.code.Equals("01") ?? false));

            ViewBag.dateTimeEmision = automaticTransferDTO.dateTimeEmision;
            automaticTransferDTO = null;
        }

        private void BuildViewDataEdit()
        {
            var automaticTransferDTO = GetAutomaticTransferDTO();
            BuildComboBoxWarehouseEdit("CmbBoxWarehouseExitEdit");
            BuildComboBoxWarehouseEdit("CmbBoxWarehouseEntryEdit");

            var id_WarehouseEntry = automaticTransferDTO.id_WarehouseEntry == null ? "" : automaticTransferDTO.id_WarehouseEntry.ToString();
            var id_WarehouseExit = automaticTransferDTO.id_WarehouseExit == null ? "" : automaticTransferDTO.id_WarehouseExit.ToString();

            BuildComboBoxWarehouseLocationEdit(id_WarehouseEntry, "CmbBoxWarehouseLocationEntryEdit");
            BuildComboBoxWarehouseLocationEdit(id_WarehouseExit, "CmbBoxWarehouseLocationEntryEdit");

            BuildComboBoxReasonEdit("CmbBoxReasonExitEdit");
            BuildComboBoxReasonEdit("CmbBoxReasonEntryEdit");
            BuildComboBoxCenterCostEdit("CmbBoxCostCenterEntryEdit");
            BuildComboBoxCenterCostEdit("CmbBoxCostCenterExitEdit");

            BuildComboBoxSubCenterCostEdit(automaticTransferDTO.id_CostCenterEntry, "CmbBoxSubCostCenterEntryEdit");
            BuildComboBoxSubCenterCostEdit(automaticTransferDTO.id_CostCenterExit, "CmbBoxSubCostCenterExitEdit");

            BuildComboBoxUserDispatcherEdit();
            BuildComboBoxItemType();
            BuildComboBoxTrademark();
            BuildComboBoxPresentation();
            BuildComboBoxSize();
            BuildComboBoxItemTrademarkModel();
            BuildComboBoxItemGroupCategory();

            automaticTransferDTO = null;
        }

        #endregion << Private Methods >>

        [HttpPost]
        public ActionResult GridViewAutomaticTransfer()
        {
            return PartialView("_GridViewResultList", GetAutomaticTransferResultConsultDTO());
        }

        #region << Build ComboBoxes >>

        private void BuildComboBoxState()
        {
            ViewData["CmbBoxEstados"] = db.DocumentState
                .Where(e => e.isActive
                    && e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoAutomaticTransfer)
                    )
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                }).ToList();
        }

        private void BuildComboBoxReasonIndex(string index)
        {
            var model = db.InventoryReason
                            .Where(w => w.isActive && w.isForTransfer == true
                                && (w.isAuthomatic == false || w.isAuthomatic == null)
                                )
                            .ToList();

            if (index == "CmbBoxReasonEntry")
            {
                model = model.Where(w => w.AdvanceParametersDetail.valueCode.Contains("I")).ToList();
            }
            else if (index == "CmbBoxReasonExit")
            {
                model = model.Where(w => w.AdvanceParametersDetail.valueCode.Contains("E")).ToList();
            }

            ViewData[index] = model
                 .Select(s => new
                 {
                     Text = s.name,
                     Value = s.id.ToString(),
                     UsoUsuarioLote = s.requiereUserLotNubmber == null ? "N" : (s.requiereUserLotNubmber.Value ? "S" : "N"),
                     UsoSistemaLote = s.requiereSystemLotNubmber == null ? "N" : (s.requiereSystemLotNubmber.Value ? "S" : "N"),
                 }).ToList();
        }

        private void BuildComboBoxWarehouseIndex(string index)
        {
            EntityObjectPermissions entityObjectPermissions = ViewData["entityObjectPermissions"] != null ? (EntityObjectPermissions)ViewData["entityObjectPermissions"] : null;
            var model = db.Warehouse.Where(t => t.id_company == ActiveCompany.id && t.isActive).ToList();

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            else
            {
                model.Clear();
            }
            ViewData[index] = model
                 .Where(e => e.isActive)
                 .Select(s => new SelectListItem
                 {
                     Text = s.name,
                     Value = s.id.ToString(),
                 }).ToList();
        }

        private void BuildComboBoxWarehouseLocationIndex(string id_Warehouse, string index)
        {
            ViewData[index] = db.WarehouseLocation
                 .Where(e => e.isActive
                 && e.id_warehouse.ToString() == id_Warehouse)
                 .Select(s => new SelectListItem
                 {
                     Text = s.name,
                     Value = s.id.ToString(),
                 }).ToList();
        }

        private void BuildComboBoxUserDispatcher()
        {
            ViewData["CmbBoxUserDispatcher"] = db.User
                .Where(e => e.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.Employee.Person.fullname_businessName,
                    Value = s.id.ToString(),
                }).ToList();
        }

        private void BuildComboBoxItem()
        {
            ViewData["CmbBoxItem"] = db.Item
                .Where(e => e.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.masterCode + " - " + s.name,
                    Value = s.id.ToString(),
                }).ToList();
        }

        #endregion << Build ComboBoxes >>

        #region << Build ComboBoxes Edit >>

        private void BuildComboBoxWarehouseEdit(string index)
        {
            var lsWarehousePeriodOpens = db.InventoryPeriodDetail
                .Where(w => w.AdvanceParametersDetail.valueCode.Contains("A")
                && w.InventoryPeriod.isActive)
                .Select(s => s.InventoryPeriod.id_warehouse).Distinct().ToList();

            EntityObjectPermissions entityObjectPermissions = ViewData["entityObjectPermissions"] != null ? (EntityObjectPermissions)ViewData["entityObjectPermissions"] : null;
            var model = db.Warehouse
                            .Where(t => t.id_company == ActiveCompany.id && t.isActive
                            && lsWarehousePeriodOpens.Contains(t.id))
                            .ToList();

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            else
            {
                model.Clear();
            }
            ViewData[index] = model
                 .Where(e => e.isActive)
                 .Select(s => new
                 {
                     Text = s.name,
                     Value = s.id.ToString(),
                     allowsNegativeBalances = s.allowsNegativeBalances == true ? "S" : "N"
                 }).ToList();
        }

        private void BuildComboBoxReasonEdit(string index)
        {
            var model = db.InventoryReason
                            .Where(w => w.isActive && w.isForTransfer == true
                            && (w.isAuthomatic == false || w.isAuthomatic == null)
                            )
                            .ToList();

            if (index == "CmbBoxReasonEntryEdit")
            {
                model = model.Where(w => w.AdvanceParametersDetail.valueCode.Contains("I")).ToList();
            }
            else if (index == "CmbBoxReasonExitEdit")
            {
                model = model.Where(w => w.AdvanceParametersDetail.valueCode.Contains("E")).ToList();
            }

            ViewData[index] = model
                 .Select(s => new
                 {
                     Text = s.name,
                     Value = s.id.ToString(),
                     s.valorization,
                     UsoUsuarioLote = s.requiereUserLotNubmber == null ? "N" : (s.requiereUserLotNubmber.Value ? "S" : "N"),
                     UsoSistemaLote = s.requiereSystemLotNubmber == null ? "N" : (s.requiereSystemLotNubmber.Value ? "S" : "N"),
                 }).ToList();
        }

        private void BuildComboBoxCenterCostEdit(string index)
        {
            ViewData[index] = db.CostCenter
                 .Where(e => e.isActive
                 && e.id_higherCostCenter == null)
                 .Select(s => new SelectListItem
                 {
                     Text = s.name,
                     Value = s.id.ToString(),
                 }).ToList();
        }

        private void BuildComboBoxSubCenterCostEdit(int? id_centercost, string index)
        {
            ViewData[index] = db.CostCenter
                 .Where(e => e.isActive
                 && e.id_higherCostCenter != null && e.id_higherCostCenter == id_centercost)
                 .Select(s => new SelectListItem
                 {
                     Text = s.name,
                     Value = s.id.ToString(),
                 }).ToList();
        }

        private void BuildComboBoxWarehouseLocationEdit(string id_Warehouse, string index)
        {
            ViewData[index] = db.WarehouseLocation
                 .Where(e => e.isActive
                 && e.id_warehouse.ToString() == id_Warehouse)
                 .Select(s => new SelectListItem
                 {
                     Text = s.name,
                     Value = s.id.ToString(),
                 }).ToList();
        }

        private void BuildComboBoxUserDispatcherEdit()
        {
            ViewData["CmbBoxUserDispatcherEdit"] = db.User
                .Where(e => e.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.Employee.Person.fullname_businessName,
                    Value = s.id.ToString(),
                }).ToList();
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

        #endregion << Build ComboBoxes Edit >>

        #region << Callback ComboBoxes >>

        public ActionResult ComboBoxState()
        {
            BuildComboBoxState();
            return PartialView("CmbBoxes/_ComboBoxState");
        }

        public ActionResult ComboBoxReasonExit()
        {
            BuildComboBoxReasonIndex("CmbBoxReasonExit");
            return PartialView("CmbBoxes/_ComboBoxReasonExit");
        }

        public ActionResult ComboBoxReasonEntry()
        {
            BuildComboBoxReasonIndex("CmbBoxReasonEntry");
            return PartialView("CmbBoxes/_ComboBoxReasonEntry");
        }

        public ActionResult ComboBoxWarehouseExit()
        {
            BuildComboBoxWarehouseIndex("CmbBoxWarehouseExit");
            return PartialView("CmbBoxes/_ComboBoxWarehouseExit");
        }

        public ActionResult ComboBoxWarehouseEntry()
        {
            BuildComboBoxWarehouseIndex("CmbBoxWarehouseEntry");
            return PartialView("CmbBoxes/_ComboBoxWarehouseEntry");
        }

        public ActionResult ComboBoxWarehouseLocationExit(string id_Warehouse)
        {
            BuildComboBoxWarehouseLocationIndex(id_Warehouse, "CmbBoxWarehouseLocationExit");
            return PartialView("CmbBoxes/_ComboBoxWarehouseLocationExit");
        }

        public ActionResult ComboBoxWarehouseLocationEntry(string id_Warehouse)
        {
            BuildComboBoxWarehouseLocationIndex(id_Warehouse, "CmbBoxWarehouseLocationEntry");
            return PartialView("CmbBoxes/_ComboBoxWarehouseLocationEntry");
        }

        public ActionResult ComboBoxUserDispatcher()
        {
            BuildComboBoxUserDispatcher();
            return PartialView("CmbBoxes/_ComboBoxUserDispatcher");
        }

        public ActionResult ComboBoxUserItem()
        {
            BuildComboBoxItem();
            return PartialView("CmbBoxes/_ComboBoxItem");
        }

        #endregion << Callback ComboBoxes >>

        #region << Callback ComboBoxes Edit >>

        public ActionResult ComboBoxWarehouseExitEdit(int? id_warehouse)
        {
            BuildComboBoxWarehouseEdit("CmbBoxWarehouseExitEdit");
            AutomaticTransferDTO _tmp = new AutomaticTransferDTO { id_WarehouseExit = id_warehouse };
            return PartialView("CmbBoxesEdit/_ComboBoxWarehouseExitEdit", _tmp);
        }

        public ActionResult ComboBoxWarehouseEntryEdit(int? id_warehouse)
        {
            BuildComboBoxWarehouseEdit("CmbBoxWarehouseEntryEdit");
            AutomaticTransferDTO _tmp = new AutomaticTransferDTO { id_WarehouseEntry = id_warehouse };
            return PartialView("CmbBoxesEdit/_ComboBoxWarehouseEntryEdit", _tmp);
        }

        public ActionResult ComboBoxWarehouseLocationExitEdit(string id_Warehouse, int? id_WarehouseLocation)
        {
            int iid_warehouse = 0;
            Int32.TryParse(id_Warehouse, out iid_warehouse);

            AutomaticTransferDTO _tmp = new AutomaticTransferDTO
            {
                id_WarehouseExit = iid_warehouse,
                id_WarehouseLocationExit = id_WarehouseLocation
            };

            return PartialView("CmbBoxesEdit/_ComboBoxWarehouseLocationExitEdit", _tmp);
        }

        public ActionResult ComboBoxWarehouseLocationEntryEdit(string id_Warehouse, int? id_WarehouseLocation)
        {
            Int32.TryParse(id_Warehouse, out var iid_warehouse);

            AutomaticTransferDTO _tmp = new AutomaticTransferDTO
            {
                id_WarehouseEntry = iid_warehouse,
                id_WarehouseLocationEntry = id_WarehouseLocation
            };

            return PartialView("CmbBoxesEdit/_ComboBoxWarehouseLocationEntryEdit", _tmp);
        }

        public ActionResult ComboBoxReasonExitEdit(int? id_ReasonEdit)
        {
            BuildComboBoxReasonEdit("CmbBoxReasonExitEdit");
            AutomaticTransferDTO _tmp = new AutomaticTransferDTO { id_InventoryReasonExit = id_ReasonEdit };

            return PartialView("CmbBoxesEdit/_ComboBoxReasonExitEdit", _tmp);
        }

        public ActionResult ComboBoxReasonEntryEdit(int? id_ReasonEdit)
        {
            BuildComboBoxReasonEdit("CmbBoxReasonEntryEdit");
            AutomaticTransferDTO _tmp = new AutomaticTransferDTO { id_InventoryReasonEntry = id_ReasonEdit };
            return PartialView("CmbBoxesEdit/_ComboBoxReasonEntryEdit", _tmp);
        }

        public ActionResult ComboBoxCostCenterExitEdit(int? id_costcenter)
        {
            BuildComboBoxCenterCostEdit("CmbBoxCostCenterExitEdit");
            AutomaticTransferDTO _tmp = new AutomaticTransferDTO { id_CostCenterExit = id_costcenter };
            return PartialView("CmbBoxesEdit/_ComboBoxCostCenterExitEdit", _tmp);
        }

        public ActionResult ComboBoxCostCenterEntryEdit(string change_cost_center, int? id_costcenter)
        {
            BuildComboBoxCenterCostEdit("CmbBoxCostCenterEntryEdit");
            AutomaticTransferDTO _tmp = new AutomaticTransferDTO { id_CostCenterEntry = id_costcenter };
            if (change_cost_center != "Y")
            {
                _tmp.id_CostCenterEntry = null;
            }
            return PartialView("CmbBoxesEdit/_ComboBoxCostCenterEntryEdit", _tmp);
        }

        public ActionResult ComboBoxSubCostCenterExitEdit(int? id_costcenter, int? id_subcostcenter)
        {
            BuildComboBoxSubCenterCostEdit(id_costcenter, "CmbBoxSubCostCenterExitEdit");
            AutomaticTransferDTO _tmp = new AutomaticTransferDTO { id_SubCostCenterExit = id_subcostcenter, id_CostCenterExit = id_costcenter };
            return PartialView("CmbBoxesEdit/_ComboBoxSubCostCenterExitEdit", _tmp);
        }

        public ActionResult ComboBoxSubCostCenterEntryEdit(string change_sub_cost_center, int? id_costcenter, int? id_subcostcenter)
        {
            BuildComboBoxSubCenterCostEdit(id_costcenter, "CmbBoxSubCostCenterEntryEdit");
            AutomaticTransferDTO _tmp = new AutomaticTransferDTO { id_SubCostCenterEntry = id_subcostcenter, id_CostCenterEntry = id_costcenter };

            if (change_sub_cost_center != "Y")
            {
                _tmp.id_SubCostCenterEntry = null;
            }
            return PartialView("CmbBoxesEdit/_ComboBoxSubCostCenterEntryEdit", _tmp);
        }

        public ActionResult ComboBoxDespachadorEdit(int? id_dispatcher)
        {
            BuildComboBoxUserDispatcherEdit();
            AutomaticTransferDTO _tmp = new AutomaticTransferDTO { id_Despachador = id_dispatcher };
            return PartialView("CmbBoxesEdit/_ComboBoxUserDispatcherEdit", _tmp);
        }

        public ActionResult ComboBoxItemEdit(int? id_warehouse_exit, int? id_warehouse_location
            , string requiereUsuarioLote
            , string requiereSistemaLote
            , int? id_item_selected,
            DateTime? fechaEmision,
            int? id_itemType, 
            int? id_size, 
            int? id_trademark, 
            int? id_trademarkModel, 
            int? id_presentation, 
            string codigoProducto, 
            int? categoriaProducto,
            int? modeloProducto)
        {
            AutomaticTransferDetailDTO automaticTransferDetailDto = new AutomaticTransferDetailDTO
            {
                id_warehouse_exit = id_warehouse_exit,
                id_warehouselocation_exit = id_warehouse_location,
                id_Item = id_item_selected,
                requiereUsuarioLote = requiereUsuarioLote,
                requiereSistemaLote = requiereSistemaLote
            };
            ViewData["id_warehouse_exit"] = id_warehouse_exit;
            ViewData["id_warehouse_location_exit"] = id_warehouse_location;
            ViewData["requiereUsuarioLote"] = requiereUsuarioLote;
            ViewData["requiereSistemaLote"] = requiereSistemaLote;
            ViewData["fechaEmision"] = fechaEmision;
            ViewData["id_itemType"] = id_itemType;
            ViewData["id_size"] = id_size;
            ViewData["id_trademark"] = id_trademark;
            ViewData["id_trademarkModel"] = id_trademarkModel;
            ViewData["id_presentation"] = id_presentation;
            ViewData["codigoProducto"] = codigoProducto;
            ViewData["categoriaProducto"] = categoriaProducto;
            ViewData["modeloProducto"] = modeloProducto;
            return PartialView("CmbBoxesEdit/_ComboBoxItemEdit", automaticTransferDetailDto);
        }

        public ActionResult ComboBoxMetricUnitInvEdit()
        {
            AutomaticTransferDetailDTO automaticTransferDetailDto = new AutomaticTransferDetailDTO();

            return PartialView("CmbBoxesEdit/_ComboBoxMetricUnitInvEdit", automaticTransferDetailDto);
        }

        public ActionResult ComboBoxMetricUnitMovEdit()
        {
            AutomaticTransferDetailDTO automaticTransferDetailDto = new AutomaticTransferDetailDTO();

            return PartialView("CmbBoxesEdit/_ComboBoxMetricUnitMovEdit", automaticTransferDetailDto);
        }

        public ActionResult ComboBoxLotEdit(int? id_warehouse
            , int? id_warehouse_location
            , int? id_warehouse_entry
            , int? id_warehouse_location_entry
            , int? id_MetricUnitMov
            , int? id_item
            , string requiereUsuarioLote
            , string requiereSistemaLote
            , DateTime? fechaEmision)
        {
            AutomaticTransferDetailDTO automaticTransferDetailDto = new AutomaticTransferDetailDTO();
            automaticTransferDetailDto.id_warehouse_exit = id_warehouse;
            automaticTransferDetailDto.id_warehouselocation_exit = id_warehouse_location;
            automaticTransferDetailDto.id_warehouse_entry = id_warehouse_entry;
            automaticTransferDetailDto.id_warehouselocation_entry = id_warehouse_location_entry;
            automaticTransferDetailDto.id_MetricUnitMov = id_MetricUnitMov;
            automaticTransferDetailDto.id_Item = id_item;

            ViewData["id_warehouse_exit"] = id_warehouse;
            ViewData["id_warehouse_location_exit"] = id_warehouse_location;
            ViewData["id_warehouse_entry"] = id_warehouse_entry;
            ViewData["id_warehouse_location_entry"] = id_warehouse_location_entry;
            ViewData["requiereUsuarioLote"] = requiereUsuarioLote;
            ViewData["requiereSistemaLote"] = requiereSistemaLote;
            ViewData["fechaEmision"] = fechaEmision;

            return PartialView("CmbBoxesEdit/_ComboBoxLotEdit", automaticTransferDetailDto);
        }

        #endregion << Callback ComboBoxes Edit >>

        #region << Edit >>

        [HttpPost]
        public ActionResult Edit(int id = 0, bool enabled = true)
        {
            var model = new AutomaticTransferDTO();
            AutomaticTransfer automaticTransfer = db.AutomaticTransfer.FirstOrDefault(d => d.id == id);
            if (automaticTransfer == null)
            {
                model = Create();
            }
            else
            {
                model = ConvertToDto(automaticTransfer);
            }
            SetAutomaticTransferDTO(model);
            BuilViewBag(enabled);

            BuildViewDataEdit();
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentCopy(int id)
        {
            var model = new AutomaticTransferDTO();
            AutomaticTransfer automaticTransfer = db.AutomaticTransfer.FirstOrDefault(d => d.id == id);
            if (automaticTransfer == null)
            {
                model = Create();
            }
            else
            {
                model = ConvertToDto(automaticTransfer);
                model.id = 0;
                model.id_InventoryMoveEntry = null;
                model.id_InventoryMoveExit = null;
                model.EntryNumberMove = "";
                model.ExitNumberMove = "";
                model.number = "";
            }
            SetAutomaticTransferDTO(model);
            BuilViewBag(true);

            BuildViewDataEdit();
            return PartialView("Edit", model);
        }

        private AutomaticTransferDTO Create()
        {
            using (var db = new DBContext())
            {
                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoAutomaticTransfer));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var hoy = DateTime.Now;
                Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
                int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
                var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

                var automaticTransferDTO = new AutomaticTransferDTO
                {
                    id = 0,
                    id_documentType = documentType?.id ?? 0,
                    documentType = documentType?.name ?? "",
                    number = "",
                    dateHoy = hoy.ToString("dd-MM-yyyy"),
                    dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),
                    idState = documentState?.id ?? 0,
                    state = documentState?.name ?? "",
                    dateTimeEmision = hoy,
                    dateTimeEmisionStr = hoy.ToString("dd-MM-yyyy"),
                    description = "",

                    lsDetail = new List<AutomaticTransferDetailDTO>()
                };

                automaticTransferDTO.id_Despachador = ActiveUser.id;

                return automaticTransferDTO;
            }
        }

        private AutomaticTransferDTO ConvertToDto(AutomaticTransfer automaticTransfer)
        {
            var hoy = DateTime.Now;
            Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
            int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
            var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

            var automaticTransferDto = new AutomaticTransferDTO
            {
                id = automaticTransfer.id,
                id_documentType = automaticTransfer.Document.id_documentType,
                documentType = automaticTransfer.Document.DocumentType.name,
                number = automaticTransfer.Document.number,
                idState = automaticTransfer.Document.id_documentState,
                state = automaticTransfer.Document.DocumentState.name,
                dateTimeEmisionStr = automaticTransfer.Document.emissionDate.ToString("dd-MM-yyyy"),
                dateTimeEmision = automaticTransfer.Document.emissionDate,
                description = automaticTransfer.Document.description,
                id_InventoryMoveExit = automaticTransfer.id_InventoryMoveExit,
                id_InventoryMoveEntry = automaticTransfer.id_InventoryMoveEntry,
                id_WarehouseExit = automaticTransfer.id_WarehouseExit,
                id_WarehouseLocationExit = automaticTransfer.id_WarehouseLocationExit,
                id_InventoryReasonExit = automaticTransfer.id_InventoryReasonExit,
                id_CostCenterExit = automaticTransfer.id_CostCenterExit,
                id_SubCostCenterExit = automaticTransfer.id_SubCostCenterExit,
                code_SubCostCenterExit = "",
                RequerimentNumber = automaticTransfer.RequerimentNumber,
                id_Despachador = automaticTransfer.id_Despachador,

                id_WarehouseEntry = automaticTransfer.id_WarehouseEntry,
                id_WarehouseLocationEntry = automaticTransfer.id_WarehouseLocationEntry,
                id_InventoryReasonEntry = automaticTransfer.id_InventoryReasonEntry,
                id_CostCenterEntry = automaticTransfer.id_CostCenterEntry,
                id_SubCostCenterEntry = automaticTransfer.id_SubCostCenterEntry,
                code_SubCostCenterEntry = "",

                ExitNumberMove = db.InventoryMove.FirstOrDefault(fod => fod.id == automaticTransfer.id_InventoryMoveExit)?.natureSequential,
                EntryNumberMove = db.InventoryMove.FirstOrDefault(fod => fod.id == automaticTransfer.id_InventoryMoveEntry)?.natureSequential,

                idProcessPlantExit = automaticTransfer.idProcessPlantExit,
                idProcessPlantEntry = automaticTransfer.idProcessPlantEntry,

                dateHoy = hoy.ToString("dd-MM-yyyy"),
                dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),

                lsDetail = new List<AutomaticTransferDetailDTO>()
            };
            var lsMetricUNit = db.MetricUnit.Select(s => new { s.id, s.name }).ToList();

            foreach (var itemDetail in automaticTransfer.AutomaticTransferDetail)
            {
                var lotAux = db.Lot.FirstOrDefault(fod => fod.id == itemDetail.id_Lot);
                string numberAux = (lotAux != null ? lotAux.number : "");
                string internalNumberAux = (lotAux != null ? lotAux.internalNumber : "");
                internalNumberAux = (internalNumberAux != "" && internalNumberAux != null) ? internalNumberAux : (lotAux?.ProductionLot != null ? lotAux.ProductionLot.internalNumber : "");

                string numero_lote = (numberAux + ((numberAux != "" && numberAux != null && internalNumberAux != "" && internalNumberAux != null) ? "-" : "") + (internalNumberAux));

                automaticTransferDto.lsDetail.Add(new AutomaticTransferDetailDTO
                {
                    id = itemDetail.id,
                    id_Item = itemDetail.id_Item,
                    str_ItemName = (itemDetail.Item?.masterCode ?? "") + " - " + (itemDetail.Item?.name ?? ""),
                    id_MetricUnitInv = itemDetail.id_MetricUnitInv,
                    id_MetricUnitMov = itemDetail.id_MetricUnitMov,
                    str_MetricUnitInv = lsMetricUNit.FirstOrDefault(fod => fod.id == itemDetail.id_MetricUnitInv)?.name ?? "",
                    str_MetricUnitMov = lsMetricUNit.FirstOrDefault(fod => fod.id == itemDetail.id_MetricUnitMov)?.name ?? "",
                    quantity = itemDetail.quantity ?? 0,
                    strQuantity = itemDetail.quantity != null ? itemDetail.quantity.Value.ToString("N6") : "0,000000",
                    cost = itemDetail.cost ?? 0,
                    total = itemDetail.cost != null ? itemDetail.cost * itemDetail.quantity : 0,
                    strTotal = itemDetail.cost != null ? (itemDetail.cost.Value * itemDetail.quantity.Value).ToString("N6") : "0,000000",
                    strCost = itemDetail.cost != null ? itemDetail.cost.Value.ToString("N6") : "0,000000",
                    id_lot = itemDetail.id_Lot,
                    numero_lote = numero_lote,
                    //itemDetail.Lot?.number + " " + itemDetail.Lot?.internalNumber,
                    saldo = itemDetail.saldo ?? 0,
                    strSaldo = itemDetail.saldo != null ? itemDetail.saldo.Value.ToString("N6") : "0,000000",
                });
            }

            return automaticTransferDto;
        }

        #endregion << Edit >>

        #region << Detail >>

        public ActionResult GridviewDetailItem(int? id_warehouse_exit
            , int? id_warehouse_location_exit
            , int? id_warehouse_entry
            , int? id_warehouse_location_entry
            , string requiereUsuarioLote
            , string requiereSistemaLote
            , bool? enabled
            , DateTime? fechaEmision,
            int? id_itemType, 
            int? id_size, 
            int? id_trademark, 
            int? id_trademarkModel, 
            int? id_presentation, 
            string codigoProducto, 
            int? categoriaProducto,
            int? modeloProducto)
        {
            var automaticTransferDTO = GetAutomaticTransferDTO();
            ViewData["id_warehouse_exit"] = id_warehouse_exit;
            ViewData["id_warehouse_location_exit"] = id_warehouse_location_exit;
            ViewData["id_warehouse_entry"] = id_warehouse_entry;
            ViewData["id_warehouse_location_entry"] = id_warehouse_location_entry;
            ViewData["requiereUsuarioLote"] = requiereUsuarioLote;
            ViewData["requiereSistemaLote"] = requiereSistemaLote;
            ViewData["fechaEmision"] = fechaEmision;
            ViewData["id_itemType"] = id_itemType;
            ViewData["id_size"] = id_size;
            ViewData["id_trademark"] = id_trademark;
            ViewData["id_trademarkModel"] = id_trademarkModel;
            ViewData["id_presentation"] = id_presentation;
            ViewData["codigoProducto"] = codigoProducto;
            ViewData["categoriaProducto"] = categoriaProducto;
            ViewData["modeloProducto"] = modeloProducto;

            automaticTransferDTO = automaticTransferDTO ?? new AutomaticTransferDTO();
            ViewBag.enabled = enabled;
            BuilViewBag(enabled ?? true);
            return PartialView("_GridViewDetailItem", automaticTransferDTO.lsDetail.ToList());
        }

        public ActionResult GridviewDetailItemAddNew(int? id_automatictransfer
            , int? id_warehouse_exit
            , int? id_warehouse_location_exit
            , decimal? cost_hidden
            , bool? enabled
             , string valorization
            , AutomaticTransferDetailDTO automaticTransferDetailDTO)
        {
            var automaticTransferDTO = GetAutomaticTransferDTO();

            automaticTransferDTO = automaticTransferDTO ?? new AutomaticTransferDTO();

            int max_id = 0;
            if (automaticTransferDTO.lsDetail.Count() > 0)
            {
                max_id = automaticTransferDTO.lsDetail.Max(s => s.id);
            }

            ViewData["id_warehouse_exit"] = id_warehouse_exit;
            ViewData["id_warehouse_location_exit"] = id_warehouse_location_exit;

            max_id = max_id + 1;
            automaticTransferDetailDTO.id = max_id;

            var data = GetDataDetail(automaticTransferDetailDTO.id_Item
                    , automaticTransferDetailDTO.id_MetricUnitInv
                    , automaticTransferDetailDTO.id_MetricUnitMov
                    , automaticTransferDetailDTO.id_lot);

            if (!IsEnabled("VerCosto"))
            {
                automaticTransferDetailDTO.cost = cost_hidden ?? 0;
            }
            else
            {
                if (valorization != "Manual")
                {
                    automaticTransferDetailDTO.cost = cost_hidden ?? 0;
                }
                else
                {
                }
            }
            automaticTransferDetailDTO.total = automaticTransferDetailDTO.cost * automaticTransferDetailDTO.quantity;

            automaticTransferDetailDTO.str_ItemName = data.Item1 + " - " + data.Item2;
            automaticTransferDetailDTO.str_MetricUnitInv = data.Item3;
            automaticTransferDetailDTO.str_MetricUnitMov = data.Item4;
            automaticTransferDetailDTO.numero_lote = data.Item5;

            automaticTransferDetailDTO.strQuantity = automaticTransferDetailDTO.quantity.ToString("N6");
            automaticTransferDetailDTO.strSaldo = automaticTransferDetailDTO.saldo != null ? automaticTransferDetailDTO.saldo.Value.ToString("N6") : "0,000000";
            automaticTransferDetailDTO.strCost = automaticTransferDetailDTO.cost.ToString("N6");
            automaticTransferDetailDTO.strTotal = automaticTransferDetailDTO.total != null ? automaticTransferDetailDTO.total.Value.ToString("N6") : "0,000000";


            // Verificamos que el detalle de producto no esté repetido
            var index = automaticTransferDTO.lsDetail
                .FindIndex(e => e.id_Item == automaticTransferDetailDTO.id_Item && e.id_lot == automaticTransferDetailDTO.id_lot);

            if (index >= 0)
            {
                throw new Exception($"Ya existe un detalle con el mismo Producto - Lote. Fila: {index + 1}.");
            }

            automaticTransferDTO.lsDetail.Add(automaticTransferDetailDTO);

            SetAutomaticTransferDTO(automaticTransferDTO);

           
            ViewBag.enabled = enabled;
            BuilViewBag(enabled ?? true);
            return PartialView("_GridViewDetailItem", automaticTransferDTO.lsDetail.ToList());
        }

        public ActionResult GridviewDetailItemUpdate(int? id_automatictransfer
            , int? id_warehouse_exit
            , int? id_warehouse_location_exit
            , decimal? cost_hidden
            , bool? enabled
            , string valorization
            , AutomaticTransferDetailDTO automaticTransferDetailDTO)
        {
            var automaticTransferDTO = GetAutomaticTransferDTO();

            automaticTransferDTO = automaticTransferDTO ?? new AutomaticTransferDTO();

            // Verificamos que el detalle de producto no esté repetido
            var index = automaticTransferDTO.lsDetail
                .FindIndex(e => e.id_Item == automaticTransferDetailDTO.id_Item 
                    && e.id_lot == automaticTransferDetailDTO.id_lot
                    && e.id != automaticTransferDetailDTO.id);

            if (index >= 0)
            {
                throw new Exception($"Ya existe un detalle con el mismo Producto - Lote. Fila: {index + 1}.");
            }

            var detail = automaticTransferDTO.lsDetail.FirstOrDefault(fod => fod.id == automaticTransferDetailDTO.id);

            ViewData["id_warehouse_exit"] = id_warehouse_exit;
            ViewData["id_warehouse_location_exit"] = id_warehouse_location_exit;
            if (detail != null)
            {
                detail.id_Item = automaticTransferDetailDTO.id_Item;
                detail.id_MetricUnitInv = automaticTransferDetailDTO.id_MetricUnitInv;
                detail.id_MetricUnitMov = automaticTransferDetailDTO.id_MetricUnitMov;
                detail.quantity = automaticTransferDetailDTO.quantity;
                detail.cost = automaticTransferDetailDTO.cost;
                detail.id_lot = automaticTransferDetailDTO.id_lot;
                detail.saldo = automaticTransferDetailDTO.saldo;
                detail.total = automaticTransferDetailDTO.total;
                detail.id_warehouse_exit = id_warehouse_exit;
                detail.id_warehouselocation_exit = id_warehouse_location_exit;

                if (!IsEnabled("VerCosto"))
                {
                    detail.cost = cost_hidden ?? 0;
                }
                else
                {
                    if (valorization != "Manual")
                    {
                        detail.cost = cost_hidden ?? 0;
                    }
                    else
                    {
                        detail.cost = automaticTransferDetailDTO.cost;
                    }
                }

                detail.total = detail.cost * detail.quantity;

                var data = GetDataDetail(automaticTransferDetailDTO.id_Item
                    , automaticTransferDetailDTO.id_MetricUnitInv
                    , automaticTransferDetailDTO.id_MetricUnitMov
                    , automaticTransferDetailDTO.id_lot);

                detail.str_ItemName = data.Item1 + " - " + data.Item2;
                detail.str_MetricUnitInv = data.Item3;
                detail.str_MetricUnitMov = data.Item4;
                detail.numero_lote = data.Item5;

                detail.strQuantity = detail.quantity.ToString("N6");
                detail.strSaldo = detail.saldo != null ? detail.saldo.Value.ToString("N6") : "0,000000";
                detail.strCost = detail.cost.ToString("N6");
                detail.strTotal = detail.total != null ? detail.total.Value.ToString("N6") : "0,000000";
            }

            SetAutomaticTransferDTO(automaticTransferDTO);
            ViewBag.enabled = enabled;
            BuilViewBag(enabled ?? true);
            return PartialView("_GridViewDetailItem", automaticTransferDTO.lsDetail.ToList());
        }

        public ActionResult GridviewDetailItemDelete(int? id_warehouse_exit
            , int? id_warehouse_location_exit
            , bool? enabled
            , string valorization
            , int id)
        {
            var automaticTransferDTO = GetAutomaticTransferDTO();

            automaticTransferDTO = automaticTransferDTO ?? new AutomaticTransferDTO();
            ViewData["id_warehouse_exit"] = id_warehouse_exit;
            ViewData["id_warehouse_location_exit"] = id_warehouse_location_exit;
            var detail = automaticTransferDTO.lsDetail.FirstOrDefault(fod => fod.id == id);
            if (detail != null)
            {
                automaticTransferDTO.lsDetail.Remove(detail);
            }

            SetAutomaticTransferDTO(automaticTransferDTO);
            ViewBag.enabled = enabled;
            BuilViewBag(enabled ?? true);
            return PartialView("_GridViewDetailItem", automaticTransferDTO.lsDetail.ToList());
        }

        private Tuple<string, string, string, string, string> GetDataDetail(int? id_item
            , int? id_metricunitinv
            , int? id_metricunitmov
            , int? id_lot)
        {
            var lsit = db.Item.Where(w => w.id == id_item).Select(s => new { s.masterCode, s.name }).ToList();

            string str_itemcode = "";
            string str_itemname = "";
            string str_MetricUnitInv = "";
            string str_MetricUnitMov = "";
            string numero_lote = "";
            if (lsit != null && lsit.Count() > 0)
            {
                var det = lsit.FirstOrDefault();
                if (det != null)
                {
                    str_itemcode = det.masterCode;
                    str_itemname = det.name;
                }
            }
            str_MetricUnitInv = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricunitinv)?.name;
            str_MetricUnitMov = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricunitmov)?.name;

            var lotAux = db.Lot.FirstOrDefault(fod => fod.id == id_lot);
            string numberAux = (lotAux != null ? lotAux.number : "");
            string internalNumberAux = (lotAux != null ? lotAux.internalNumber : "");
            internalNumberAux = (internalNumberAux != "" && internalNumberAux != null) ? internalNumberAux : (lotAux?.ProductionLot != null ? lotAux.ProductionLot.internalNumber : "");

            numero_lote = (numberAux + ((numberAux != "" && numberAux != null && internalNumberAux != "" && internalNumberAux != null) ? "-" : "") + (internalNumberAux));

            return new Tuple<string, string, string, string, string>(str_itemcode, str_itemname, str_MetricUnitInv, str_MetricUnitMov, numero_lote);
        }

        public JsonResult ValidateDetail()
        {
            return null;
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult LotDetail(int? id_item, 
            int? id_warehouse, int? id_warehouseLocation, int? id_lot,
            DateTime? fechaEmision)
        {
            InventoryMove inventoryMove = (TempData["inventoryMove"] as InventoryMove);
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);
            var requiresLot = item?.ItemInventory?.requiresLot ?? false;

            id_lot = id_lot == 0 ? null : id_lot;
            //var saldos = new Services.ServiceInventoryMove()
            //    .GetSaldosProductoLote(requiresLot, id_warehouse, id_warehouseLocation, id_item, id_lot, null, fechaEmision);
            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral( new InvParameterBalanceGeneral
            {
                requiresLot = requiresLot,
                id_Warehouse = id_warehouse,
                id_WarehouseLocation = id_warehouseLocation,
                id_Item = id_item,
                id_ProductionLot = id_lot,
                lotMarket = null,
                id_productionCart = null,
                cut_Date = fechaEmision,
                id_company = this.ActiveCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;

            decimal averagePrice = 0;
            decimal remainingBalance = saldos.Sum(e => e.saldo);

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

        #endregion << Detail >>

        #region << Save >>

        [HttpPost]
        public JsonResult Save(string jsonAutomaticTransfer)
        {
            string mensaje = "";
            using (var dbTmp = new DBContext())
            {
                using (var trans = dbTmp.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var automaticTransferDto = GetAutomaticTransferDTO();

                        bool respuesta1 = ValidationLayer1(ref automaticTransferDto, ref mensaje);

                        if (!respuesta1)
                        {
                            throw new Exception(mensaje);
                        }

                        JToken token = JsonConvert.DeserializeObject<JToken>(jsonAutomaticTransfer);

                        var newObject = false;
                        var id = token.Value<int>("id");

                        var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoAutomaticTransfer));
                        var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                        var automaticTransfer = db.AutomaticTransfer.FirstOrDefault(d => d.id == id);
                        if (automaticTransfer == null)
                        {
                            newObject = true;

                            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
                            if (id_emissionPoint == 0)
                                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

                            automaticTransfer = new AutomaticTransfer
                            {
                                Document = new Document
                                {
                                    number = GetDocumentNumber(documentType?.id ?? 0),
                                    sequential = GetDocumentSequential(documentType?.id ?? 0),
                                    emissionDate = DateTime.Now,
                                    authorizationDate = DateTime.Now,
                                    id_emissionPoint = id_emissionPoint,
                                    id_documentType = documentType.id,
                                    id_userCreate = ActiveUser.id,
                                    dateCreate = DateTime.Now,
                                    id_userUpdate = ActiveUser.id,
                                    dateUpdate = DateTime.Now,
                                    id_documentState = documentState.id
                                }
                            };

                            documentType.currentNumber++;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;
                        }

                        automaticTransfer.Document.emissionDate = token.Value<DateTime>("dateTimeEmision");
                        automaticTransfer.Document.description = token.Value<string>("description");
                        automaticTransfer.Document.id_documentState = documentState.id;
                        automaticTransfer.Document.id_userUpdate = ActiveUser.id;
                        automaticTransfer.Document.dateUpdate = DateTime.Now;

                        automaticTransfer.id_WarehouseExit = token.Value<int>("id_WarehouseExit");
                        automaticTransfer.id_WarehouseLocationExit = token.Value<int>("id_WarehouseLocationExit");
                        automaticTransfer.id_WarehouseEntry = token.Value<int>("id_WarehouseEntry");
                        automaticTransfer.id_WarehouseLocationEntry = token.Value<int>("id_WarehouseLocationEntry");
                        automaticTransfer.id_InventoryReasonExit = token.Value<int>("id_InventoryReasonExit");
                        automaticTransfer.id_InventoryReasonEntry = token.Value<int>("id_InventoryReasonEntry");
                        automaticTransfer.id_CostCenterExit = token.Value<int>("id_CostCenterExit");
                        automaticTransfer.id_SubCostCenterExit = token.Value<int>("id_SubCostCenterExit");
                        automaticTransfer.id_CostCenterEntry = token.Value<int>("id_CostCenterEntry");
                        automaticTransfer.id_SubCostCenterEntry = token.Value<int>("id_SubCostCenterEntry");

                        automaticTransfer.idProcessPlantExit = token.Value<int>("idProcessPlantExit");
                        automaticTransfer.idProcessPlantEntry = token.Value<int>("idProcessPlantEntry");

                        automaticTransfer.RequerimentNumber = token.Value<string>("RequerimentNumber");

                        automaticTransfer.id_Despachador = ActiveUser.id;

                        automaticTransfer.id_InventoryMoveExit = null;
                        automaticTransfer.id_InventoryMoveEntry = null;

                        SaveDetails(ref automaticTransferDto, ref automaticTransfer);

                        if (newObject)
                        {
                            db.AutomaticTransfer.Add(automaticTransfer);
                            db.Entry(automaticTransfer).State = EntityState.Added;
                        }
                        else
                        {
                            db.AutomaticTransfer.Attach(automaticTransfer);
                            db.Entry(automaticTransfer).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        trans.Commit();

                        result.Data = automaticTransfer.id.ToString();
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

        private void SaveDetails(ref AutomaticTransferDTO automaticTransferDto,
            ref AutomaticTransfer automaticTransfer)
        {
            int idAutomaticTransfer = automaticTransfer.id;
            var details1 = db.AutomaticTransferDetail
                                                .Where(d => d.id_AutomaticTransfer == idAutomaticTransfer);

            foreach (var detail in details1)
            {
                db.AutomaticTransferDetail.Remove(detail);
                db.AutomaticTransferDetail.Attach(detail);
                db.Entry(detail).State = EntityState.Deleted;
            }

            var newDetails = automaticTransferDto.lsDetail;
            foreach (var det in newDetails)
            {
                AutomaticTransferDetail tmp = new AutomaticTransferDetail();

                tmp.id_Item = det.id_Item;
                tmp.id_MetricUnitInv = det.id_MetricUnitInv;
                tmp.id_MetricUnitMov = det.id_MetricUnitMov;
                tmp.quantity = det.quantity;
                tmp.cost = det.cost;
                tmp.id_Lot = det.id_lot;
                tmp.saldo = det.saldo;

                automaticTransfer.AutomaticTransferDetail.Add(tmp);
            }
        }

        private bool ValidationLayer1(ref AutomaticTransferDTO automaticTransferDTO, ref string mensaje)
        {
            if (automaticTransferDTO == null)
            {
                mensaje = "No existe Documento para procesar.";
                return false;
            }
            if (automaticTransferDTO.lsDetail == null)
            {
                mensaje = "No se puede grabar documento sin detalles.";
                return false;
            }
            if (automaticTransferDTO.lsDetail.Count() == 0)
            {
                mensaje = "No se puede grabar documento sin detalles.";
                return false;
            }
            return true;
        }

        #endregion << Save >>

        #region << Change Status >>

        [HttpPost]
        public JsonResult Approve(int id)
        {
            ApiResult result = new ApiResult();

            ApproveAutomaticTransfer(id, ref result);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Reverse(int id)
        {
            ApiResult result = new ApiResult();

            try
            {
                var document = db.Document.FirstOrDefault(e => e.id == id);
                if(document.DocumentState.code == "16")
                {
                    ReversarConciliacion(id, ref result);
                }
                else
                {
                    ReverseAutomaticTransfer(id, ref result);
                }
            }
            catch (Exception ex)
            {
                result.Code = ex.HResult;
                result.Message = ex.GetBaseException().Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Annul(int id)
        {
            ApiResult result = new ApiResult();

            AnnulAutomaticTransfer(id, ref result);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Conciliate(int id)
        {
            ApiResult result = new ApiResult();

            ConciliateAutomaticTransfer(id, ref result);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void ApproveAutomaticTransfer(int id, ref ApiResult result)
        {
            result.Code = 0;
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            var _db = new DBContext();
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var automaticTransfer = _db.AutomaticTransfer.FirstOrDefault(p => p.id == id);
                        var automaticTransferDTO = ConvertToDto(automaticTransfer);
                        if (automaticTransfer != null)
                        {
                            var aprovedState = _db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                            if (aprovedState == null)
                            {
                                throw new Exception("El estado aprobado no existe.");
                            }

                            automaticTransfer.Document.id_documentState = aprovedState.id;
                            automaticTransfer.Document.dateUpdate = DateTime.Now;
                            automaticTransfer.Document.id_userUpdate = ActiveUser.id;

                            InventoryMove inventoryMoveExit = null;
                            InventoryMove inventoryMoveEntry = null;

                            var itemDetail = automaticTransferDTO.lsDetail.ToList();

                            foreach (var i in itemDetail)
                            {
                                var item = db.Item.FirstOrDefault(fod => fod.id == i.id_Item);
                                var remainingBalance = GetRemainingBalance( this.ActiveCompanyId,
                                                                            i.id_Item.Value, 
                                                                            automaticTransferDTO.id_WarehouseExit.Value, 
                                                                            automaticTransferDTO.id_WarehouseLocationExit.Value, 
                                                                            i.id_lot, 
                                                                            db, 
                                                                            null,
                                                                            null,
                                                                            null, 
                                                                            true);

                                var warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == automaticTransferDTO.id_WarehouseExit);
                                if (!warehouse.allowsNegativeBalances && (remainingBalance < 0 || remainingBalance < Convert.ToDecimal(i.strQuantity)))
                                {
                                    var warehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == automaticTransferDTO.id_WarehouseLocationExit);
                                    var lot = db.Lot.FirstOrDefault(fod => fod.id == i.id_lot);
                                    var action = "Aprobarse";
                                    var lotNumber = lot == null ? "" : ", perteneciente al Lote: " + lot.number;
                                    throw new ProdHandlerException("No puede " + action + " el Movimiento de Inventario debido a que no hay suficiente Stock y este quedara en negativo, para el producto: " +
                                                        item.name + ", en la Bodega: " + warehouse.name + ", en la Ubicación: " + warehouseLocation.name + lotNumber);
                                }
                            }

                            ValidateEmissionDateInventoryMove(db, automaticTransferDTO.dateTimeEmision, true, automaticTransferDTO.id_WarehouseExit);
                            ValidateEmissionDateInventoryMove(db, automaticTransferDTO.dateTimeEmision, true, automaticTransferDTO.id_WarehouseEntry);

                            var resultAux = ServiceInventoryMove
                                .UpdateInventaryMoveAutomaticTransferExit(true, ActiveUser, ActiveCompany
                                , ActiveEmissionPoint, automaticTransferDTO, db, false);
                            
                            if (!string.IsNullOrEmpty(resultAux?.message)) throw new ProdHandlerException(resultAux.message);

                            inventoryMoveExit = resultAux?.inventoryMove;
                            inventoryMoveEntry = resultAux?.inventoryMoveAux;

                            if (inventoryMoveExit == null)
                                throw new Exception("No se genero el documento de inventario de salida");
                            if (inventoryMoveEntry == null)
                                throw new Exception("No se genero el documento de inventario de entrada");

                            if (!(inventoryMoveExit.id > 0))
                                throw new Exception("No se genero el documento de inventario de salida");
                            if (!(inventoryMoveEntry.id > 0))
                                throw new Exception("No se genero el documento de inventario de entrada");

                            automaticTransfer.id_InventoryMoveExit = inventoryMoveExit.id;
                            automaticTransfer.id_InventoryMoveEntry = inventoryMoveEntry.id;

                            _db.AutomaticTransfer.Attach(automaticTransfer);
                            _db.Entry(automaticTransfer).State = EntityState.Modified;

                                _db.SaveChanges();
                                UpdateBalanceToApprove(ref automaticTransfer, ref _db);
                                trans.Commit();
                                result.Data = aprovedState.code;
                        }    
                        else
                        {
                            throw new Exception("No se encontro el objeto seleccionado");
                        }
                    }
                    catch (ProdHandlerException e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message + $"Base Exception: {e.GetBaseException()?.Message}";
                        trans.Rollback();
                        if (string.IsNullOrEmpty(e.Message))
                        {
                            MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "AutomaticTransfer", "Produccion");
                        }
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message + $"Base Exception: {e.GetBaseException()?.Message}";
                        trans.Rollback();

                    }
                }
            }
        }

        private void UpdateBalanceToApprove(ref AutomaticTransfer automaticTransfer, ref DBContext _db)
        {
            var detail = automaticTransfer.AutomaticTransferDetail;
            foreach (var det in detail)
            {
                det.saldoAnterior = det.saldo;
                det.saldo = DataProviderInventoryMove.GetRemainingBalance(  this.ActiveCompanyId,
                                                                            det.id_Item, 
                                                                            det.AutomaticTransfer.id_WarehouseExit, 
                                                                            det.AutomaticTransfer.id_WarehouseLocationExit, 
                                                                            det.id_Lot, 
                                                                            (det.Lot != null ? det.Lot.internalNumber : null));
            }
            _db.SaveChanges();
        }

        private void ReverseBalance(ref AutomaticTransfer automaticTransfer, ref DBContext _db)
        {
            var detail = automaticTransfer.AutomaticTransferDetail;
            foreach (var det in detail)
            {
                det.saldo = det.saldoAnterior;
            }
            _db.SaveChanges();
        }

        private void ReverseAutomaticTransfer(int id, ref ApiResult result)
        {
            result.Code = 0;
            var _db = new DBContext();
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var automaticTransfer = _db.AutomaticTransfer.FirstOrDefault(p => p.id == id);
                        var automaticTransferDTO = ConvertToDto(automaticTransfer);
                        if (automaticTransfer != null)
                        {
                            var pendingState = _db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
                            if (pendingState == null)
                            {
                                throw new Exception("El estado pendiente no existe.");
                            }

                            if (!(automaticTransfer.id_InventoryReasonEntry > 0))
                                throw new Exception("No existe movimiento de ingreso para reversar.");
                            if (!(automaticTransfer.id_InventoryMoveExit > 0))
                                throw new Exception("No existe movimiento de salida para reversar.");

                            //Verificar si existe lotes relacionados al proceso de cierre
                            var automaticTransferDetail = automaticTransfer.AutomaticTransferDetail.ToList();
                            foreach (var item in automaticTransferDetail)
                            {
                                var lot = db.Lot.FirstOrDefault(a => a.id == item.id_Lot);
                                var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == item.id_Lot);
                                var internalNumberAux = "";
                                var internalNumber = "";
                                internalNumberAux = (lot != null && lot.internalNumber != null)
                                    ? lot.internalNumber.Substring(0, 5)
                                    : productionLot.internalNumber.Substring(0, 5);

                                internalNumber = (lot != null && lot.internalNumber != null)
                                    ? lot.internalNumber
                                    : productionLot.internalNumber;

                                var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.number.Substring(0, 5) == internalNumberAux && a.Document.DocumentState.code != "05"
                                                           && a.isActive);

                                if (productionLotClose != null && automaticTransfer.Document.emissionDate.Date <= productionLotClose.Document.emissionDate.Date
                                    && automaticTransfer.Document.DocumentState.code == "03")
                                {
                                    throw new Exception("El lote " + internalNumber + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                                }
                            }


                            GenerateRevertInventoryMove(automaticTransfer.id_InventoryMoveExit.Value, automaticTransferDTO, ref _db);

                            automaticTransfer.Document.id_documentState = pendingState.id;
                            automaticTransfer.Document.dateUpdate = DateTime.Now;
                            automaticTransfer.Document.id_userUpdate = ActiveUser.id;

                            automaticTransfer.id_InventoryMoveEntry = null;
                            automaticTransfer.id_InventoryMoveExit = null;

                            _db.AutomaticTransfer.Attach(automaticTransfer);
                            _db.Entry(automaticTransfer).State = EntityState.Modified;
                            _db.SaveChanges();
                            ReverseBalance(ref automaticTransfer, ref _db);
                            trans.Commit();
                            result.Data = pendingState.code;
                        }
                        else
                        {
                            throw new Exception("No se encontro el objeto seleccionado");
                        }
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }
                }
            }
        }
        private void ReversarConciliacion(int id, ref ApiResult result)
        {
            result.Code = 0;
            using (var _db = new DBContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var automaticTransfer = _db.AutomaticTransfer.FirstOrDefault(p => p.id == id);
                        var automaticTransferDTO = ConvertToDto(automaticTransfer);
                        if (automaticTransfer != null)
                        {
                            var appState = _db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                            if (appState == null)
                            {
                                throw new Exception("El estado Aprobado no existe.");
                            }

                            //Verificar si existe lotes relacionados al proceso de cierre
                            var automaticTransferDetail = automaticTransfer.AutomaticTransferDetail.ToList();
                            foreach (var item in automaticTransferDetail)
                            {
                                var lot = db.Lot.FirstOrDefault(a => a.id == item.id_Lot);
                                var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == item.id_Lot);
                                var internalNumberAux = "";
                                var internalNumber = "";
                                internalNumberAux = (lot != null && lot.internalNumber != null)
                                    ? lot.internalNumber.Substring(0, 5)
                                    : productionLot.internalNumber.Substring(0, 5);

                                internalNumber = (lot != null && lot.internalNumber != null)
                                    ? lot.internalNumber
                                    : productionLot.internalNumber;

                                if (db.ProductionLotClose.Count() > 0)
                                {

                                    var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.number.Substring(0, 5) == internalNumberAux && a.Document.DocumentState.code != "05"
                                                               && a.isActive);

                                    if (productionLotClose != null && automaticTransfer.Document.emissionDate.Date <= productionLotClose.Document.emissionDate.Date
                                        && automaticTransfer.Document.DocumentState.code == "16")
                                    {
                                        throw new Exception("El lote " + internalNumber + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                                    }
                                }
                            }

                            automaticTransfer.Document.id_documentState = appState.id;
                            automaticTransfer.Document.dateUpdate = DateTime.Now;
                            automaticTransfer.Document.id_userUpdate = ActiveUser.id;

                            _db.AutomaticTransfer.Attach(automaticTransfer);
                            _db.Entry(automaticTransfer).State = EntityState.Modified;
                            _db.SaveChanges();

                            trans.Commit();
                            result.Data = appState.code;
                        }
                        else
                        {
                            throw new Exception("No se encontro el objeto seleccionado");
                        }
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }
                }
            }
        }
        private void ConciliateAutomaticTransfer(int id, ref ApiResult result)
        {
            result.Code = 0;
            var _db = new DBContext();
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var automaticTransfer = _db.AutomaticTransfer.FirstOrDefault(p => p.id == id);
                        var automaticTransferDTO = ConvertToDto(automaticTransfer);
                        if (automaticTransfer != null)
                        {
                            var pendingState = _db.DocumentState.FirstOrDefault(d => d.code.Equals("16"));
                            if (pendingState == null)
                            {
                                throw new Exception("El estado conciliado no existe.");
                            }

                            automaticTransfer.Document.id_documentState = pendingState.id;
                            automaticTransfer.Document.dateUpdate = DateTime.Now;
                            automaticTransfer.Document.id_userUpdate = ActiveUser.id;

                            _db.AutomaticTransfer.Attach(automaticTransfer);
                            _db.Entry(automaticTransfer).State = EntityState.Modified;
                            _db.SaveChanges();

                            trans.Commit();
                            result.Data = pendingState.code;
                        }
                        else
                        {
                            throw new Exception("No se encontro el objeto seleccionado");
                        }
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }
                }
            }
        }
        private void AnnulAutomaticTransfer(int id, ref ApiResult result)
        {
            result.Code = 0;
            using (var _db = new DBContext())
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var automaticTransfer = _db.AutomaticTransfer.FirstOrDefault(p => p.id == id);
                        var automaticTransferDTO = ConvertToDto(automaticTransfer);
                        if (automaticTransfer != null)
                        {
                            var annulState = _db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                            if (annulState == null)
                            {
                                throw new Exception("El estado anulado no existe.");
                            }

                            automaticTransfer.Document.id_documentState = annulState.id;
                            automaticTransfer.Document.dateUpdate = DateTime.Now;
                            automaticTransfer.Document.id_userUpdate = ActiveUser.id;

                            _db.AutomaticTransfer.Attach(automaticTransfer);
                            _db.Entry(automaticTransfer).State = EntityState.Modified;
                            _db.SaveChanges();

                            trans.Commit();
                            result.Data = annulState.code;
                        }
                        else
                        {
                            throw new Exception("No se encontro el objeto seleccionado");
                        }
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }
                }
            }
        }

        #endregion << Change Status >>

        #region << DTOs >>

        private AutomaticTransferDTO GetAutomaticTransferDTO()
        {
            if (!(Session["AutomaticTransferDTO"] is AutomaticTransferDTO automaticTransfer))
                automaticTransfer = new AutomaticTransferDTO();
            return automaticTransfer;
        }

        private void SetAutomaticTransferDTO(AutomaticTransferDTO automaticTransferDTO)
        {
            Session["AutomaticTransferDTO"] = automaticTransferDTO;
        }

        #endregion << DTOs >>

        #region << InventoryMove >>

        private void ValidateBalance(ref AutomaticTransfer automaticTransfer, ref DBContext _db)
        {
            var detail = automaticTransfer.AutomaticTransferDetail;
            int id_InventoryReasonExit = automaticTransfer.id_InventoryReasonExit ?? 0;

            bool requiereSystemLotNubmber = _db.InventoryReason.FirstOrDefault(fod => fod.id == id_InventoryReasonExit)?.requiereSystemLotNubmber ?? false;
            bool requiereUserLotNubmber = _db.InventoryReason.FirstOrDefault(fod => fod.id == id_InventoryReasonExit)?.requiereUserLotNubmber ?? false;
            var fechaEmision = automaticTransfer?.Document?.emissionDate;

            foreach (var det in detail)
            {
                var qtmp = det.quantity ?? 0;
                var respuesta = CalcularSaldoProducto(det.id_Item, det.id_MetricUnitMov, det.AutomaticTransfer.id_WarehouseExit
                    , det.AutomaticTransfer.id_WarehouseLocationExit, det.AutomaticTransfer.id_WarehouseEntry
                    , det.AutomaticTransfer.id_WarehouseLocationEntry, requiereSystemLotNubmber, requiereUserLotNubmber, fechaEmision);

                if (respuesta == null)
                {
                    throw new Exception("No se ha encontrado datos sobre el saldo de este producto " + det.Item?.name);
                }
                if (respuesta.remainingBalance < det.quantity)
                {
                    throw new Exception("El producto " + det.Item?.name + " no tiene el saldo suficiente para ser utilizado.");
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InventoryMoveItemDetails2(int? id_itemCurrent, int? id_metricUnitMove
            , int? id_warehouse, int? id_warehouseLocation, int? id_warehouseEntry, int? id_warehouseLocationEntry
            , bool? withLotSystem, bool? withLotCustomer, DateTime? fechaEmision)
        {
            var result = CalcularSaldoProducto(id_itemCurrent, id_metricUnitMove
                                            , id_warehouse, id_warehouseLocation
                                            , id_warehouseEntry, id_warehouseLocationEntry
                                            , withLotSystem
                                            , withLotCustomer
                                            , fechaEmision);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InventoryMoveItemDetails(int? id_inventoryMoveDetail
            , int? id_itemCurrent, int? id_metricUnitMove, int? id_warehouse, int? id_warehouseLocation
            , int? id_warehouseEntry, int? id_warehouseLocationEntry, int? id_lot, bool? withLotSystem
            , bool? withLotCustomer, DateTime? fechaEmision)
        {
            List<Item> items = new List<Item>();
            List<Lot> lots = new List<Lot>();
            decimal remainingBalance = 0;
            decimal unitPriceMove = 0;
            decimal averagePrice = 0;

            bool requiresLot = ((withLotSystem ?? false) || (withLotCustomer ?? false));
            //var saldos = new Services.ServiceInventoryMove()
            //    .GetSaldosProductoLote(requiresLot, id_warehouse, id_warehouseLocation, id_itemCurrent, id_lot, null, fechaEmision);
            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral( new InvParameterBalanceGeneral
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
            var saldos = resultItemsLotSaldo.Item2;
            remainingBalance = saldos.Sum(e => e.saldo);

            var idsItem = saldos.Select(e => e.id_item).Distinct();
            items = db.Item.Where(w => idsItem.Contains(w.id)).ToList();
            var itemAux = items.FirstOrDefault(fod => fod.id == id_itemCurrent);

            // TODO: Validar precio unitario y precio promedio a asignar
            unitPriceMove = 0;
            averagePrice = 0;

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

            var inventoryMoveDetail = db.InventoryMoveDetail.FirstOrDefault(fod => fod.id == id_inventoryMoveDetail);
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

            #endregion metricUnitInventoryPurchase

            var result = new
            {
                masterCode = itemAux?.masterCode ?? "",
                lotNumber = inventoryMoveDetail?.Lot?.number ?? "",
                lotInternalNumber = inventoryMoveDetail?.Lot?.internalNumber ?? "",

                Message = "Ok",
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
                remainingBalance = remainingBalance,
                unitPriceMove = unitPriceMove,
                averagePrice = averagePrice
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private ProductBalanceQueryResult CalcularSaldoProducto(
            int? id_itemCurrent, int? id_metricUnitMove, int? id_warehouse, int? id_warehouseLocation
            , int? id_warehouseEntry, int? id_warehouseLocationEntry, bool? withLotSystem
            , bool? withLotCustomer, DateTime? fechaEmision)
        {
            List<Item> items = new List<Item>();
            List<Lot> lots = new List<Lot>();
            decimal remainingBalance = 0;
            decimal unitPriceMove = 0;
            decimal averagePrice = 0;

            bool requiresLot = ((withLotSystem ?? false) || (withLotCustomer ?? false));
            //var saldos = new ServiceInventoryMove()
            //    .GetSaldosProductoLote(requiresLot, id_warehouse, id_warehouseLocation, id_itemCurrent, null, null, fechaEmision);

            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral( new InvParameterBalanceGeneral
            {
                requiresLot = requiresLot,
                id_Warehouse = id_warehouse,
                id_WarehouseLocation = id_warehouseLocation,
                id_Item = id_itemCurrent,
                id_ProductionLot = null,
                lotMarket = null,
                id_productionCart = null,
                cut_Date = fechaEmision,
                id_company = this.ActiveCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;

            remainingBalance = saldos.Sum(e => e.saldo);

            var idsItem = saldos.Select(e => e.id_item).Distinct();
            items = db.Item.Where(w => idsItem.Contains(w.id)).ToList();
            var itemAux = items.FirstOrDefault(fod => fod.id == id_itemCurrent);

            // TODO: verificar temas de costo y precio unitario promedio

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

            string metricUnitMovExit = metricUnitInventory;
            int? id_metricUnitMovExit = id_metricUnitInventory;

            #endregion metricUnitInventoryPurchase

            var result = new ProductBalanceQueryResult
            {
                masterCode = itemAux?.masterCode ?? "",
                Message = "Ok",
                remainingBalance = remainingBalance,
                unitPriceMove = unitPriceMove,
                averagePrice = averagePrice
            };

            return result;
        }

        #endregion << InventoryMove >>

        #region << Generate inventory move>>

        private void GenerateInventoryMoveExit(ref AutomaticTransfer automaticTransfer
            , ref DBContext _db
            , ref InventoryMove inventoryMove)
        {
            int id_invReason = automaticTransfer.id_InventoryReasonExit ?? 0;

            int idDocumentType = _db.InventoryReason.FirstOrDefault(fod => fod.id == id_invReason)?.id_documentType ?? 0;
            DocumentType _doIt = _db.DocumentType.FirstOrDefault(fod => fod.id == idDocumentType);
            var documentState = _db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));

            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
            if (id_emissionPoint == 0)
                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

            DateTime dtNow = DateTime.Now;

            inventoryMove = new InventoryMove
            {
                Document = new Document
                {
                    number = GetDocumentNumber(_doIt?.id ?? 0),
                    sequential = GetDocumentSequential(_doIt?.id ?? 0),
                    emissionDate = dtNow,
                    authorizationDate = dtNow,
                    id_emissionPoint = id_emissionPoint,
                    id_documentType = idDocumentType,
                    id_userCreate = ActiveUser.id,
                    dateCreate = dtNow,
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = dtNow,
                    id_documentState = documentState.id
                }
            };

            inventoryMove.id_inventoryReason = id_invReason;

            var settingORLS = _db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
            ViewBag.withLotSystem = settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == _doIt?.code)?.valueAux == "1" : false;
            var settingORLC = _db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
            ViewBag.withLotCustomer = settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == _doIt?.code)?.valueAux == "1" : false;

            int inventoryMoveId_inventoryReason = inventoryMove.id_inventoryReason ?? 0;
            inventoryMove.idNatureMove = _db.InventoryReason.FirstOrDefault(fod => fod.id == inventoryMoveId_inventoryReason)?.idNatureMove;

            inventoryMove.idWarehouse = automaticTransfer.id_WarehouseExit;
            inventoryMove.id_costCenter = automaticTransfer.id_CostCenterExit;
            inventoryMove.id_subCostCenter = automaticTransfer.id_SubCostCenterExit;

            #region Relate ExitMove

            inventoryMove.InventoryExitMove = new InventoryExitMove
            {
                id_warehouseExit = automaticTransfer.id_WarehouseExit,
                id_warehouseLocationExit = automaticTransfer.id_WarehouseLocationExit,
                id_dispatcher = ActiveUser.id,
                dateExit = dtNow
            };

            #endregion Relate ExitMove

            #region Create Details

            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();
            foreach (var det in automaticTransfer.AutomaticTransferDetail)
            {
                InventoryMoveDetail detail = new InventoryMoveDetail();
                detail.id_lot = det.id_Lot <= 0 ? null : det.id_Lot;
                detail.id_item = det.id_Item.Value;//ojo
                detail.entryAmount = 0;
                detail.entryAmountCost = 0;
                detail.exitAmount = det.quantity ?? 0;
                detail.exitAmountCost = det.cost ?? 0;
                detail.id_metricUnit = det.id_MetricUnitInv.Value;//ojo
                detail.id_metricUnitMove = det.id_MetricUnitMov.Value;//ojo
                detail.id_warehouse = automaticTransfer.id_WarehouseExit.Value;
                detail.id_warehouseLocation = automaticTransfer.id_WarehouseLocationExit.Value;

                detail.inMaximumUnit = false;//ojo

                detail.id_userCreate = ActiveUser.id;
                detail.dateCreate = dtNow;
                detail.id_userUpdate = ActiveUser.id;
                detail.dateUpdate = dtNow;
                detail.unitPrice = det.cost ?? 0; //ojo
                detail.balance = det.saldo ?? 0; // ojo
                detail.averagePrice = det.cost ?? 0;//ojo
                detail.balanceCost = det.cost ?? 0;//ojo
                detail.unitPriceMove = det.cost ?? 0;
                detail.amountMove = det.quantity ?? 0;
                detail.id_costCenter = automaticTransfer.id_CostCenterExit;
                detail.id_subCostCenter = automaticTransfer.id_SubCostCenterExit;
                detail.genSecTrans = true; // ojo ]
                detail.ordenProduccion = "";
                detail.productoCost = 0;
                detail.lastestProductoCost = 0;

                inventoryMove.InventoryMoveDetail.Add(detail);
            }

            #endregion Create Details

            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    foreach (var detail in inventoryMove.InventoryMoveDetail)
                    {
                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                        if (entityValuePermissions == null)
                        {
                            throw new Exception("No tiene Permiso para editar y guardar el movimiento de inventario.");
                        }
                    }
                    foreach (var detail in inventoryMove.InventoryMoveDetail)
                    {
                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                        if (entityValuePermissions == null)
                        {
                            throw new Exception("No tiene Permiso para aprobar el movimiento de inventario.");
                        }
                    }
                }
            }

            if (inventoryMove.InventoryMoveDetail.Count == 0)
            {
                throw new Exception("No se puede guardar un movimiento de inventario sin detalles.");
            }
            if (ViewBag.withLotSystem)
            {
                var inventoryMoveDetailAux = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.Lot?.number == "" || fod.Lot?.number == null);
                if (inventoryMoveDetailAux != null)
                {
                    throw new Exception("No se puede guardar el movimiento de inventario sin lote de Sistema, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }
            }
            if (ViewBag.withLotCustomer)
            {
                var inventoryMoveDetailAux = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.Lot?.internalNumber == "" || fod.Lot?.internalNumber == null);
                if (inventoryMoveDetailAux != null)
                {
                    throw new Exception("No se puede guardar el movimiento de inventario sin lote de Cliente, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }
            }
            var inventoryMoveDetailAux2 = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_costCenter == null);
            if (inventoryMoveDetailAux2 != null)
            {
                throw new Exception("No se puede guardar el movimiento de inventario sin centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
            }

            inventoryMoveDetailAux2 = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_subCostCenter == null);
            if (inventoryMoveDetailAux2 != null)
            {
                throw new Exception("No se puede guardar el movimiento de inventario sin sub-centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
            }
            ServiceInventoryMoveAux result = null;
            InventoryMove invMoveToReverse = null;

            _db.InventoryMove.Add(inventoryMove);
            _db.SaveChanges();
            result = UpdateInventaryMoveTransferExit(true, ActiveUser, ActiveCompany
                , ActiveEmissionPoint, inventoryMove, _db, false, invMoveToReverse);

            inventoryMove = result?.inventoryMove;

            _db.SaveChanges();
        }

        private void GenerateInventoryMoveEntry(ref AutomaticTransfer automaticTransfer
            , ref DBContext _db
            , ref InventoryMove inventoryMove)
        {
            //InventoryMove  = null;
            int id_invReason = automaticTransfer.id_InventoryReasonEntry ?? 0;

            int idDocumentType = _db.InventoryReason.FirstOrDefault(fod => fod.id == id_invReason)?.id_documentType ?? 0;
            DocumentType _doIt = _db.DocumentType.FirstOrDefault(fod => fod.id == idDocumentType);
            var documentState = _db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));

            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
            if (id_emissionPoint == 0)
                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

            DateTime dtNow = DateTime.Now;

            inventoryMove = new InventoryMove
            {
                Document = new Document
                {
                    number = GetDocumentNumber(_doIt?.id ?? 0),
                    sequential = GetDocumentSequential(_doIt?.id ?? 0),
                    emissionDate = dtNow,
                    authorizationDate = dtNow,
                    id_emissionPoint = id_emissionPoint,
                    id_documentType = idDocumentType,
                    id_userCreate = ActiveUser.id,
                    dateCreate = dtNow,
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = dtNow,
                    id_documentState = documentState.id
                }
            };

            inventoryMove.id_inventoryReason = id_invReason;

            var settingORLS = _db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
            ViewBag.withLotSystem = settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == _doIt?.code)?.valueAux == "1" : false;
            var settingORLC = _db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
            ViewBag.withLotCustomer = settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == _doIt?.code)?.valueAux == "1" : false;

            int inventoryMoveId_inventoryReason = inventoryMove.id_inventoryReason ?? 0;
            inventoryMove.idNatureMove = _db.InventoryReason.FirstOrDefault(fod => fod.id == inventoryMoveId_inventoryReason)?.idNatureMove;

            inventoryMove.idWarehouse = automaticTransfer.id_WarehouseEntry;
            inventoryMove.id_costCenter = automaticTransfer.id_CostCenterExit;
            inventoryMove.id_subCostCenter = automaticTransfer.id_SubCostCenterExit;

            #region Relate ExitMove

            inventoryMove.InventoryEntryMove = new InventoryEntryMove
            {
                id_warehouseEntry = automaticTransfer.id_WarehouseEntry,
                id_warehouseLocationEntry = automaticTransfer.id_WarehouseLocationEntry,
                id_receiver = ActiveUser.id,
                dateEntry = dtNow
            };

            #endregion Relate ExitMove

            #region Create Details

            inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();
            foreach (var det in automaticTransfer.AutomaticTransferDetail)
            {
                InventoryMoveDetail detail = new InventoryMoveDetail();
                detail.id_lot = det.id_Lot <= 0 ? null : det.id_Lot;
                detail.id_item = det.id_Item.Value;//ojo
                detail.entryAmount = det.quantity ?? 0;
                detail.entryAmountCost = det.quantity ?? 0;
                detail.exitAmount = 0;
                detail.exitAmountCost = 0;
                detail.id_metricUnit = det.id_MetricUnitInv.Value;//ojo
                detail.id_metricUnitMove = det.id_MetricUnitMov.Value;//ojo
                detail.id_warehouse = automaticTransfer.id_WarehouseEntry.Value;
                detail.id_warehouseLocation = automaticTransfer.id_WarehouseLocationEntry.Value;

                detail.inMaximumUnit = false;//ojo

                detail.id_userCreate = ActiveUser.id;
                detail.dateCreate = dtNow;
                detail.id_userUpdate = ActiveUser.id;
                detail.dateUpdate = dtNow;
                detail.unitPrice = det.cost ?? 0; //ojo
                detail.balance = det.saldo ?? 0; // ojo
                detail.averagePrice = det.cost ?? 0;//ojo
                detail.balanceCost = det.cost ?? 0;//ojo
                detail.unitPriceMove = det.cost ?? 0;
                detail.amountMove = det.quantity ?? 0;
                detail.id_costCenter = automaticTransfer.id_CostCenterEntry;
                detail.id_subCostCenter = automaticTransfer.id_SubCostCenterEntry;
                detail.genSecTrans = true; // ojo ]
                detail.ordenProduccion = "";
                detail.productoCost = 0;
                detail.lastestProductoCost = 0;

                inventoryMove.InventoryMoveDetail.Add(detail);
            }

            #endregion Create Details

            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    foreach (var detail in inventoryMove.InventoryMoveDetail)
                    {
                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                        if (entityValuePermissions == null)
                        {
                            throw new Exception("No tiene Permiso para editar y guardar el movimiento de inventario.");
                        }
                    }
                    foreach (var detail in inventoryMove.InventoryMoveDetail)
                    {
                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == detail.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                        if (entityValuePermissions == null)
                        {
                            throw new Exception("No tiene Permiso para aprobar el movimiento de inventario.");
                        }
                    }
                }
            }

            if (inventoryMove.InventoryMoveDetail.Count == 0)
            {
                throw new Exception("No se puede guardar un movimiento de inventario sin detalles.");
            }
            if (ViewBag.withLotSystem)
            {
                var inventoryMoveDetailAux = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.Lot?.number == "" || fod.Lot?.number == null);
                if (inventoryMoveDetailAux != null)
                {
                    throw new Exception("No se puede guardar el movimiento de inventario sin lote de Sistema, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }
            }
            if (ViewBag.withLotCustomer)
            {
                var inventoryMoveDetailAux = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.Lot?.internalNumber == "" || fod.Lot?.internalNumber == null);
                if (inventoryMoveDetailAux != null)
                {
                    throw new Exception("No se puede guardar el movimiento de inventario sin lote de Cliente, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
                }
            }
            var inventoryMoveDetailAux2 = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_costCenter == null);
            if (inventoryMoveDetailAux2 != null)
            {
                throw new Exception("No se puede guardar el movimiento de inventario sin centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
            }

            inventoryMoveDetailAux2 = inventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_subCostCenter == null);
            if (inventoryMoveDetailAux2 != null)
            {
                throw new Exception("No se puede guardar el movimiento de inventario sin sub-centro de costo, es obligatorio en todos los detalles, Configúrela e intente de nuevo.");
            }
            ServiceInventoryMoveAux result = null;

            InventoryMove inventoryMoveToReverse = null;
            _db.InventoryMove.Add(inventoryMove);
            _db.SaveChanges();

            result = UpdateInventaryMoveTransferEntry(true, ActiveUser, ActiveCompany, ActiveEmissionPoint
                , inventoryMove, _db, false, inventoryMoveToReverse);

            inventoryMove = result?.inventoryMove;

            // _db.InventoryMove.Add(inventoryMove);

            _db.SaveChanges();
        }

        private void GenerateRevertInventoryMove(int id, AutomaticTransferDTO automaticTransferDto, ref DBContext _db)
        {
            InventoryMove inventoryMove = _db.InventoryMove.FirstOrDefault(r => r.id == id);
            string natureMove = "";

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

            DocumentState documentStateReversado = _db.DocumentState.FirstOrDefault(s => s.code == "05"); //revrsado

            if (inventoryMove != null && documentStateReversado != null)
            {
                natureMove = inventoryMove.AdvanceParametersDetail.valueCode.Trim();
                ViewData["_natureMove"] = natureMove;

                var invMove = _db.InventoryMove.FirstOrDefault(fod => fod.id == id);
                if (invMove?.Document?.DocumentType?.code == "155")//Egreso Por Transferencia Automática Por Recepción Placa
                {
                    var inventoryMoveTransferAutomaticExit = invMove;
                    var result = ServiceInventoryMove
                        .UpdateInventaryMoveAutomaticTransferExit(true
                        , ActiveUser, ActiveCompany, ActiveEmissionPoint, automaticTransferDto, _db, true, inventoryMoveTransferAutomaticExit);
                }

                inventoryMove.Document.id_documentState = documentStateReversado.id;
                inventoryMove.Document.DocumentState = documentStateReversado;

                _db.InventoryMove.Attach(inventoryMove);
                _db.Entry(inventoryMove).State = EntityState.Modified;

                _db.SaveChanges();
            }

            var settingORLS = db.Setting.FirstOrDefault(fod => fod.code == "ORLS");
            ViewBag.withLotSystem = settingORLS != null ? settingORLS.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false;
            var settingORLC = db.Setting.FirstOrDefault(fod => fod.code == "ORLC");
            ViewBag.withLotCustomer = settingORLC != null ? settingORLC.SettingDetail.FirstOrDefault(fod => fod.value == inventoryMove.Document.DocumentType.code)?.valueAux == "1" : false;
        }

        #endregion << Generate inventory move>>

        #region << Paginacion >>

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id)
        {
            var index = GetAutomaticTransferResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

            var result = new
            {
                maximunPages = GetAutomaticTransferResultConsultDTO().Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            var element = GetAutomaticTransferResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
            var automaticTransfer = db.AutomaticTransfer.FirstOrDefault(d => d.id == element.id);
            if (automaticTransfer == null)
                return PartialView("Edit", new AutomaticTransferDTO());

            var model = ConvertToDto(automaticTransfer);
            SetAutomaticTransferDTO(model);
            BuildViewDataEdit();
            BuilViewBag(false);

            return PartialView("Edit", model);
        }

        #endregion << Paginacion >>

        private bool IsEnabled(string namePermission)
        {
            int id_user = (int)ViewData["id_user"];
            int id_menu = (int)ViewData["id_menu"];

            User user = DataProviderUser.UserById(id_user);

            if (user == null)
                return false;

            UserMenu userMenu = user.UserMenu.FirstOrDefault(m => m.Menu.id == id_menu);

            if (userMenu == null)
                return false;

            Permission permission = userMenu.Permission.FirstOrDefault(p => p.name.Equals(namePermission));

            return (permission != null);
        }

        #region REPORT

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReportGeneral(int id_automatictransfer, string codeReport)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_automatictransfer;

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

            #endregion "Armo Parametros"
        }

        #endregion REPORT

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public JsonResult PrintReportEGRING(int id, string codeReport)
        {
            List<ParamCR> paramLst = new List<ParamCR>();

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ParamCR _param = null;

            var idtransfer = db.AutomaticTransfer.FirstOrDefault(e => e.id.Equals(id))?.id ?? 0;

            _param = new ParamCR
            {
                Nombre = "@id",
                Valor = codeReport.Equals("TRAEG") ? id : idtransfer,
            };
            paramLst.Add(_param);

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrintReportEGRING2(int id, string codeReport)
        {
            List<ParamCR> paramLst = new List<ParamCR>();

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ParamCR _param = null;

            var idtransfer = db.AutomaticTransfer.FirstOrDefault(e => e.id.Equals(id))?.id ?? 0;

            _param = new ParamCR
            {
                Nombre = "@id",
                Valor = codeReport.Equals("TRAIN") ? id : idtransfer,
            };
            paramLst.Add(_param);

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}