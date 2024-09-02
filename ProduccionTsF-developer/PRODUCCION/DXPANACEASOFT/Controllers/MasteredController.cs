using DevExpress.DataAccess.Sql;
using DevExpress.Office.PInvoke;
using DevExpress.Utils;
using DevExpress.Utils.About;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.XtraSpreadsheet.Model;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.Models.InventoryBalance;
using DXPANACEASOFT.Models.Mastereds;
using DXPANACEASOFT.Operations;
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
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using Utilitarios.Logs;
using Utilitarios.ProdException;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static DXPANACEASOFT.Models.Parametros;
using static DXPANACEASOFT.Services.ServiceInventoryBalance;
using static DXPANACEASOFT.Services.ServiceInventoryMove;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class MasteredController : DefaultController
    {
        private const string m_TipoDocumentoMastered = "145";

        private MasteredDTO GetMasteredDTO()
        {
            if (!(Session["MasteredDTO"] is MasteredDTO mastered))
                mastered = new MasteredDTO();
            return mastered;
        }

        private List<MasteredResultConsultDTO> GetMasteredResultConsultDTO()
        {
            if (!(Session["MasteredResultConsultDTO"] is List<MasteredResultConsultDTO> masteredResultConsult))
                masteredResultConsult = new List<MasteredResultConsultDTO>();
            return masteredResultConsult;
        }

        private void SetMasteredDTO(MasteredDTO masteredDTO)
        {
            Session["MasteredDTO"] = masteredDTO;
        }

        private void SetMasteredResultConsultDTO(List<MasteredResultConsultDTO> masteredResultConsult)
        {
            Session["MasteredResultConsultDTO"] = masteredResultConsult;
        }

        public ActionResult Index()
        {
            BuildViewDataIndex();
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult(MasteredConsultDTO consult)
        {
            var result = GetListsConsultDto(consult);
            SetMasteredResultConsultDTO(result);
            return PartialView("ConsultResult", result);
        }

        [HttpPost]
        public ActionResult GridViewMastered()
        {
            return PartialView("_GridViewIndex", GetMasteredResultConsultDTO());
        }

        private List<MasteredResultConsultDTO> GetListsConsultDto(MasteredConsultDTO consulta)
        {
            using (var db = new DBContext())
            {
                var consultaAux = Session["consulta"] as MasteredConsultDTO;
                if (consultaAux != null && consulta.initDate == null)
                {
                    consulta = consultaAux;
                }

                var consultResult = db.Mastered.Where(MasteredQueryExtensions.GetRequestByFilter(consulta)).ToList();
                if (consulta.boxedItems != null && consulta.boxedItems.Length > 0)
                {
                    var tempModel = new List<Mastered>();
                    foreach (var order in consultResult)
                    {
                        var details = order.MasteredDetail.Where(d => consulta.boxedItems.Contains(d.id_productMP));
                        if (details.Any())
                        {
                            tempModel.Add(order);
                        }
                    }

                    consultResult = tempModel;
                }
                if (!String.IsNullOrEmpty(consulta.boxedNumberLot))
                {
                    consultResult = consultResult.Where(w => (w.MasteredDetail.FirstOrDefault(fod => fod.Lot.ProductionLot != null ? fod.Lot.ProductionLot.internalNumber.Contains(consulta.boxedNumberLot)
                                                                                                                                                   : fod.Lot.internalNumber.Contains(consulta.boxedNumberLot)) != null)).ToList();
                }

                if (consulta.masteredItems != null && consulta.masteredItems.Length > 0)
                {
                    var tempModel = new List<Mastered>();
                    foreach (var order in consultResult)
                    {
                        var details = order.MasteredDetail.Where(d => consulta.masteredItems.Contains(d.id_productPT));
                        if (details.Any())
                        {
                            tempModel.Add(order);
                        }
                    }

                    consultResult = tempModel;
                }
                if (!String.IsNullOrEmpty(consulta.masteredNumberLot))
                {
                    consultResult = consultResult.Where(w => (w.MasteredDetail.FirstOrDefault(fod => fod.Lot.ProductionLot != null ? fod.Lot.ProductionLot.internalNumber.Contains(consulta.masteredNumberLot)
                                                                                                                                                   : fod.Lot.internalNumber.Contains(consulta.masteredNumberLot)) != null)).ToList();
                }
                var query = consultResult.Select(t => new MasteredResultConsultDTO
                {
                    id = t.id,
                    number = t.Document.number,
                    emissionDate = t.Document.emissionDate,
                    responsable = db.Person.FirstOrDefault(fod => fod.id == t.id_responsable).fullname_businessName,
                    turn = t.Turn?.name,
                    boxedWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == t.id_boxedWarehouse).name,
                    masteredWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == t.id_masteredWarehouse).name,
                    state = t.Document.DocumentState.name,

                    canEdit = t.Document.DocumentState.code.Equals("01"),
                    canAproved = t.Document.DocumentState.code.Equals("01"),
                    canAnnul = t.Document.DocumentState.code.Equals("01"),
                    canReverse = t.Document.DocumentState.code.Equals("03")
                }).OrderBy(ob => ob.number).ToList();

                Session["consulta"] = consulta;

                return query;
            }
        }

        private MasteredDTO Create()
        {
            using (var db = new DBContext())
            {
                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoMastered));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var hoy = DateTime.Now;
                Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
                int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
                var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

                var masteredDTO = new MasteredDTO
                {
                    id_documentType = documentType?.id ?? 0,
                    documentType = documentType?.name ?? "",
                    number = "",
                    idSate = documentState?.id ?? 0,
                    state = documentState?.name ?? "",
                    dateTimeEmision = hoy,
                    dateTimeEmisionStr = hoy.ToString("dd-MM-yyyy"),
                    description = "",
                    id_responsable = ActiveUser.id_employee,
                    responsable = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee)?.Person.fullname_businessName,
                    turn = "",
                    id_turn = null,
                    timeInitTurn = null,
                    timeEndTurn = null,
                    dateHoy = hoy.ToString("dd-MM-yyyy"),
                    dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),
                    dateTimeStartMastered = hoy,
                    dateTimeEndMastered = hoy,

                    boxedWarehouse = "",
                    id_boxedWarehouse = null,
                    boxedWarehouseLocation = "",
                    id_boxedWarehouseLocation = null,

                    masteredWarehouse = "",
                    id_masteredWarehouse = null,
                    masteredWarehouseLocation = "",
                    id_masteredWarehouseLocation = null,

                    warehouseBoxes = "",
                    id_warehouseBoxes = null,
                    warehouseLocationBoxes = "",
                    id_warehouseLocationBoxes = null,

                    id_company = ActiveCompany.id,
                    numberExitBoxed = "",
                    numberEntryMastered = "",
                    numberEntryBoxes = "",

                    amountMP = 0.00M,
                    amountMPStr = "0.00",
                    amountPT = 0.00M,
                    amountPTStr = "0.00",
                    amountBoxes = 0.00M,
                    amountBoxesStr = "0.00",

                    lbsMP = 0.00M,
                    lbsMPStr = "0.00",
                    lbsPT = 0.00M,
                    lbsPTStr = "0.00",
                    lbsBoxes = 0.00M,
                    lbsBoxesStr = "0.00",

                    kgMP = 0.00M,
                    kgMPStr = "0.00",
                    kgPT = 0.00M,
                    kgPTStr = "0.00",
                    kgBoxes = 0.00M,
                    kgBoxesStr = "0.00",

                    MasteredDetails = new List<MasteredDetailDTO>()
                };

                return masteredDTO;
            }
        }

        private MasteredDTO ConvertToDto(Mastered mastered)
        {
            var hoy = DateTime.Now;
            Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
            int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
            var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

            var codigosEstados = new[] { "03", "16" };
            var id_inventaryMoveProcessAutomaticExit = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == mastered.id &&
                                                                   codigosEstados.Contains(fod.Document.DocumentState.code) &&
                                                                   fod.Document.DocumentType.code.Equals("146"))?.id_document;
            var inventoryMoveProcessAutomaticExit = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticExit);

            var id_inventaryMoveProcessAutomaticEntryMastered = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == mastered.id &&
                                                                   codigosEstados.Contains(fod.Document.DocumentState.code) &&
                                                                   fod.Document.DocumentType.code.Equals("147"))?.id_document;
            var inventaryMoveProcessAutomaticEntryMastered = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticEntryMastered);

            var id_inventaryMoveProcessAutomaticEntryBoxes = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == mastered.id &&
                                                                   codigosEstados.Contains(fod.Document.DocumentState.code) &&
                                                                   fod.Document.DocumentType.code.Equals("148"))?.id_document;
            var inventaryMoveProcessAutomaticEntryBoxes = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticEntryBoxes);

            var masteredDto = new MasteredDTO
            {
                id = mastered.id,
                id_documentType = mastered.Document.id_documentType,
                documentType = mastered.Document.DocumentType.name,
                number = mastered.Document.number,
                idSate = mastered.Document.id_documentState,
                state = mastered.Document.DocumentState.name,
                dateTimeEmisionStr = mastered.Document.emissionDate.ToString("dd-MM-yyyy"),
                dateTimeEmision = mastered.Document.emissionDate,
                description = mastered.Document.description,
                responsable = db.Employee.FirstOrDefault(fod => fod.id == mastered.id_responsable)?.Person.fullname_businessName,
                id_responsable = mastered.id_responsable,
                turn = mastered.Turn?.name,
                id_turn = mastered.id_turn,
                timeInitTurn = mastered.Turn != null ? mastered.Turn.timeInit.Hours + ":" + mastered.Turn.timeInit.Minutes : null,
                timeEndTurn = mastered.Turn != null ? mastered.Turn.timeEnd.Hours + ":" + mastered.Turn.timeEnd.Minutes : null,
                dateHoy = hoy.ToString("dd-MM-yyyy"),
                dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),
                dateTimeStartMastered = mastered.dateTimeStartMastered,
                dateTimeEndMastered = mastered.dateTimeEndMastered,

                boxedWarehouse = mastered.Warehouse.name,
                id_boxedWarehouse = mastered.id_boxedWarehouse,
                boxedWarehouseLocation = mastered.WarehouseLocation.name,
                id_boxedWarehouseLocation = mastered.id_boxedWarehouseLocation,

                masteredWarehouse = mastered.Warehouse1.name,
                id_masteredWarehouse = mastered.id_masteredWarehouse,
                masteredWarehouseLocation = mastered.WarehouseLocation1.name,
                id_masteredWarehouseLocation = mastered.id_masteredWarehouseLocation,

                warehouseBoxes = mastered.Warehouse2.name,
                id_warehouseBoxes = mastered.id_warehouseBoxes,
                warehouseLocationBoxes = mastered.WarehouseLocation2.name,
                id_warehouseLocationBoxes = mastered.id_warehouseLocationBoxes,

                id_company = mastered.id_company,
                numberExitBoxed = inventoryMoveProcessAutomaticExit?.natureSequential ?? "",
                numberEntryMastered = inventaryMoveProcessAutomaticEntryMastered?.natureSequential ?? "",
                numberEntryBoxes = inventaryMoveProcessAutomaticEntryBoxes?.natureSequential ?? "",

                amountMP = mastered.amountMP,
                amountMPStr = mastered.amountMP.ToString("#.00"),
                amountPT = mastered.amountPT,
                amountPTStr = mastered.amountPT.ToString("#.00"),
                amountBoxes = mastered.amountBoxes,
                amountBoxesStr = mastered.amountBoxes.ToString("#.00"),

                lbsMP = mastered.lbsMP,
                lbsMPStr = mastered.lbsMP.ToString("#.00"),
                lbsPT = mastered.lbsPT,
                lbsPTStr = mastered.lbsPT.ToString("#.00"),
                lbsBoxes = mastered.lbsBoxes,
                lbsBoxesStr = mastered.lbsBoxes.ToString("#.00"),

                kgMP = mastered.kgMP,
                kgMPStr = mastered.kgMP.ToString("#.00"),
                kgPT = mastered.kgPT,
                kgPTStr = mastered.kgPT.ToString("#.00"),
                kgBoxes = mastered.kgBoxes,
                kgBoxesStr = mastered.kgBoxes.ToString("#.00"),

                MasteredDetails = new List<MasteredDetailDTO>()
            };
            #region  -- Optimizacion consulta Saldo --
            int? id_WarehouseRemainingBalance = mastered?.MasteredDetail?.FirstOrDefault()?.id_boxedWarehouse;
            SaldoProductoLote[] SaldoValidaAprobacion = ServiceInventoryMove.GetRemainingBalanceBulk(   ActiveCompany.id,
                                                                                                        id_WarehouseRemainingBalance,
                                                                                                        mastered.MasteredDetail.Select(r => new SaldoProductoLote
                                                                                                        {
                                                                                                            id_item = r.id_productMP,
                                                                                                            id_warehouseLocation = r.id_boxedWarehouseLocation,
                                                                                                            id_lote = r.id_lotMP
                                                                                                        }).ToArray(),
                                                                                                        db,
                                                                                                        fechaCorte: mastered.Document.emissionDate);
                                        #endregion

            foreach (var itemDetail in mastered.MasteredDetail)
            {
                decimal saldoMP = (SaldoValidaAprobacion?
                                                        .Where(r => r.id_warehouseLocation == itemDetail.id_boxedWarehouseLocation
                                                                    && r.id_lote == itemDetail.id_lotMP
                                                                    && r.id_item == itemDetail.id_productMP)?
                                                        .Sum(r => r.saldo) ?? 0);
                masteredDto.MasteredDetails.Add(new MasteredDetailDTO
                {
                    id = itemDetail.id,
                    id_sales = itemDetail.id_sales,
                    codProductMP = itemDetail.Item.masterCode,
                    id_productLotMP = itemDetail.id_productMP.ToString() + ":" + itemDetail.id_lotMP.ToString() ?? "",
                    id_productMP = itemDetail.id_productMP,
                    id_lotMP = itemDetail.id_lotMP,
                    loteMP = (itemDetail?.Lot?.ProductionLot != null ? itemDetail?.Lot?.ProductionLot.internalNumber : itemDetail?.Lot?.internalNumber) ?? "",
                    saldoMP = saldoMP,
                    
                    quantityMP = itemDetail.quantityMP,
                    id_boxedWarehouse = itemDetail.id_boxedWarehouse,
                    boxedWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == itemDetail.id_boxedWarehouse).name,
                    id_boxedWarehouseLocation = itemDetail.id_boxedWarehouseLocation,
                    boxedWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == itemDetail.id_boxedWarehouseLocation).name,
                    id_costCenterExitBoxed = itemDetail.id_costCenterExitBoxed,
                    id_subCostCenterExitBoxed = itemDetail.id_subCostCenterExitBoxed,
                    id_metricUnitBoxed = itemDetail.id_metricUnitBoxed,
                    metricUnitBoxed = db.MetricUnit.FirstOrDefault(fod => fod.id == itemDetail.id_metricUnitBoxed).code,
                    codProductPT = itemDetail.Item1.masterCode,
                    id_productPT = itemDetail.id_productPT,
                    id_customer = itemDetail.id_customer,
                    quantityPT = itemDetail.quantityPT,
                    id_masteredWarehouse = itemDetail.id_masteredWarehouse,
                    masteredWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == itemDetail.id_masteredWarehouse).name,
                    id_masteredWarehouseLocation = itemDetail.id_masteredWarehouseLocation,
                    masteredWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == itemDetail.id_masteredWarehouseLocation).name,
                    id_costCenterEntryMastered = itemDetail.id_costCenterEntryMastered,
                    id_subCostCenterEntryMastered = itemDetail.id_subCostCenterEntryMastered,
                    id_metricUnitMastered = itemDetail.id_metricUnitMastered,
                    metricUnitMastered = db.MetricUnit.FirstOrDefault(fod => fod.id == itemDetail.id_metricUnitMastered).code,
                    id_lotBoxes = itemDetail.id_lotBoxes,
                    loteBoxes = (itemDetail?.Lot1?.ProductionLot != null ? itemDetail?.Lot1?.ProductionLot.internalNumber : itemDetail?.Lot1?.internalNumber) ?? "",
                    quantityBoxes = itemDetail.quantityBoxes,
                    id_warehouseBoxes = itemDetail.id_warehouseBoxes,
                    warehouseBoxes = db.Warehouse.FirstOrDefault(fod => fod.id == itemDetail.id_warehouseBoxes).name,
                    id_warehouseLocationBoxes = itemDetail.id_warehouseLocationBoxes,
                    warehouseLocationBoxes = db.WarehouseLocation.FirstOrDefault(fod => fod.id == itemDetail.id_warehouseLocationBoxes).name,
                    id_costCenterEntryBoxes = itemDetail.id_costCenterEntryBoxes,
                    id_subCostCenterEntryBoxes = itemDetail.id_subCostCenterEntryBoxes,
                    lotMarked = itemDetail.lotMarked
                });
            }
            return masteredDto;
        }

        private void BuildViewDataIndex()
        {
            BuildComboBoxState();
            BuildComboBoxResponsableIndex();
            BuildComboBoxTurnIndex();
            BuildComboBoxBoxedWarehouseIndex();
            BuildComboBoxBoxedWarehouseLocationIndex();
            BuildTokenBoxBoxedItemsIndex();
            BuildComboBoxMasteredWarehouseIndex();
            BuildComboBoxMasteredWarehouseLocationIndex();
            BuildTokenBoxMasteredItemsIndex();
        }

        private void BuildViewDataEdit()
        {
            BuildComboBoxResponsable();
            BuildComboBoxTurn();

            BuildComboBoxItemType();
            BuildComboBoxSize();
            BuildComboBoxTrademark();
            BuildComboBoxPresentationMP();
            BuildComboBoxPresentationPT();
            BuildComboBoxCustomer();
            var mastered = GetMasteredDTO();
            BuildComboBoxBoxedWarehouse(mastered.id_responsable);
            BuildComboBoxBoxedWarehouseLocation(mastered.id_responsable, mastered.id_boxedWarehouse);
            BuildComboBoxMasteredWarehouse(mastered.id_responsable);
            BuildComboBoxMasteredWarehouseLocation(mastered.id_responsable, mastered.id_masteredWarehouse);
            BuildComboBoxWarehouseBoxes(mastered.id_responsable);
            BuildComboBoxWarehouseLocationBoxes(mastered.id_responsable, mastered.id_warehouseBoxes);
            BuildTokenBoxLotes(mastered.id_boxedWarehouse, mastered.id_boxedWarehouseLocation, mastered.dateTimeEmision);
        }

        [HttpPost]
        public ActionResult Edit(int id = 0, bool enabled = true)
        {
            var model = new MasteredDTO();
            Mastered mastered = db.Mastered.FirstOrDefault(d => d.id == id);
            if (mastered == null)
            {
                model = Create();
            }
            else
            {
                model = ConvertToDto(mastered);
            }
            SetMasteredDTO(model);
            BuilViewBag(enabled);

            BuildViewDataEdit();
            return PartialView(model);
        }

        private void BuilViewBag(bool enabled)
        {
            var masteredDTO = GetMasteredDTO();
            string utizaAutorizacion = db.Setting.FirstOrDefault(fod => fod.code == "APRMAST")?.value ?? "NO";
            var estadoMasterizado = db.DocumentState.FirstOrDefault(e => e.id == masteredDTO.idSate);

            int id_menu = (int)ViewData["id_menu"];
            var tienePermisioConciliar = this.ActiveUser
                .UserMenu.FirstOrDefault(e => e.id_menu == id_menu)?
                .Permission?.FirstOrDefault(p => p.name == "Conciliar");

            ViewBag.utizaAutorizacion = (utizaAutorizacion == "SI");
            ViewBag.enabled = enabled;
            ViewBag.canNew = masteredDTO.id != 0;
            ViewBag.canEdit = !enabled &&
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == masteredDTO.idSate)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == masteredDTO.idSate)
                                     ?.code.Equals("01") ?? false) && masteredDTO.id != 0;
            
            var estadosReversoMasterizado = new[] { "03", "06", "16" };
            var puedeReversar = db.DocumentState
                .Any(e => estadosReversoMasterizado.Contains(e.code)
                    && e.id == masteredDTO.idSate) && masteredDTO.id != 0;

            ViewBag.canReverse = estadoMasterizado.code == "16"
                ? puedeReversar && !enabled && tienePermisioConciliar != null
                : puedeReversar && !enabled;

            ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == masteredDTO.idSate)
                                      ?.code.Equals("01") ?? false) && masteredDTO.id != 0;
            ViewBag.canAutorice = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == masteredDTO.idSate)
                                      ?.code.Equals("03") ?? false) && masteredDTO.id != 0;

            var estadosPuedeConciliar = (utizaAutorizacion == "SI")
                ? new[] { "06" }
                : new[] { "03" };

            var puedeConciliar = db.DocumentState
                .Any(e => estadosPuedeConciliar.Contains(e.code)
                    && e.id == masteredDTO.idSate) && masteredDTO.id != 0;
            ViewBag.canConciliate = puedeConciliar;

            ViewBag.dateTimeEmision = masteredDTO.dateTimeEmision;
        }

        #region GridViewDetails

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewDetails(bool? enabled)
        {
            var masteredDto = GetMasteredDTO();

            ViewBag.enabled = enabled;

            return PartialView("_GridViewDetails", masteredDto.MasteredDetails.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewDetailsAddNew(MasteredDetailDTO item, bool? enabled)
        {
            var masteredDto = GetMasteredDTO();
            try
            {

                if (ModelState.IsValid)
                {
                    item.id = masteredDto.MasteredDetails.Count() > 0 ? masteredDto.MasteredDetails.Max(ppd => ppd.id) + 1 : 1;

                    

                    if (!string.IsNullOrEmpty(item.id_productLotMP))
                    {
                        string data = item.id_productLotMP;
                        string[] words = "".Split(':');
                        words = data.Split(':');
                        item.id_productMP = int.Parse(words[0]);
                        item.id_lotMP = words[1] == "" ? (int?)null : int.Parse(words[1]);
                        item.id_lotBoxes = item.id_lotMP;

                        var aItem = db.Item.FirstOrDefault(s => s.id == item.id_productMP);
                        item.codProductMP = aItem.masterCode;
                        var aProductionLot = db.ProductionLot.FirstOrDefault(s => s.id == item.id_lotMP);
                        item.loteMP = aProductionLot?.internalNumber ?? "";
                        item.loteBoxes = item.loteMP;
                        item.id_metricUnitBoxed = aItem.ItemInventory.id_metricUnitInventory;
                        item.metricUnitBoxed = aItem.ItemInventory.MetricUnit.code;

                        var aItemPT = db.Item.FirstOrDefault(s => s.id == item.id_productPT);
                        item.codProductPT = aItemPT.masterCode;
                        item.id_metricUnitMastered = aItemPT.ItemInventory.id_metricUnitInventory;
                        item.metricUnitMastered = aItemPT.ItemInventory.MetricUnit.code;


                        bool isNotUnique = validaNotIsUniqueDetail(masteredDto.MasteredDetails.ToArray(), item);
                        if (isNotUnique)
                        {
                            throw new ProdHandlerException("No debe repetir item, lote y cliente");
                        }
                    }

                    InvParameterBalance invParameterBalance = new InvParameterBalance();
                    invParameterBalance.id_company = this.ActiveCompanyId;
                    invParameterBalance.id_Item = item.id_productMP;
                    invParameterBalance.cut_Date = masteredDto.dateHoy.AsDateTime();
                    invParameterBalance.consolidado = true;
                    invParameterBalance.id_Warehouse = masteredDto.id_boxedWarehouse;
                    invParameterBalance.id_WarehouseLocation = masteredDto.id_boxedWarehouseLocation;
                    invParameterBalance.id_ProductionLot = null;

                    //var balaceItem = ServiceInventoryBalance.ValidateBalance(db, invParameterBalance);
                    //
                    //
                    //var sumBalanceItem = masteredDto.MasteredDetails.Where(b => b.id_boxedWarehouse == item.id_boxedWarehouse &&
                    //            b.id_boxedWarehouseLocation == item.id_boxedWarehouseLocation && b.id_productMP == item.id_productMP).Sum(c => c.quantityMP);
                    //
                    //if (balaceItem.SaldoActual < (sumBalanceItem + item.quantityMP))
                    //{
                    //    throw new Exception("La cantidad ingresada supera la suma total del saldo de los item agregados en el detalle.");
                    //}

                    masteredDto.MasteredDetails.Add(item);

                    UpdateSummary();
                }
                else
                {
                    foreach (var modelState in this.ModelState.Values)
                    {
                        if (modelState.Errors.Count > 0)
                        {
                            throw new ProdHandlerException(modelState.Errors.Last().ErrorMessage);
                            //ViewData["EditError"] = modelState.Errors.Last().ErrorMessage;
                            //break;
                        }
                    }
                }
                
            }
            catch (ProdHandlerException e)
            {
                ViewData["EditError"] = e.Message;
                item.id = 0;                
            }
            catch (Exception e)
            {
                item.id = 0;                
                ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                FullLog(e);
            }

            ViewBag.enabled = enabled;



            return PartialView("_GridViewDetails", masteredDto.MasteredDetails.ToList());
        }

        private void UpdateSummary()
        {
            var masteredDto = GetMasteredDTO();

            decimal amountMP = 0.00M;
            decimal amountPT = 0.00M;
            decimal amountBoxes = 0.00M;

            decimal lbsMP = 0.00M;
            decimal lbsPT = 0.00M;
            decimal lbsBoxes = 0.00M;

            decimal kgMP = 0.00M;
            decimal kgPT = 0.00M;
            decimal kgBoxes = 0.00M;

            foreach (var itemDetail in masteredDto.MasteredDetails)
            {
                var productMP = db.Item.FirstOrDefault(fod => fod.id == itemDetail.id_productMP);

                var quantityMP = itemDetail.quantityMP * (productMP?.Presentation?.minimum ?? 1);
                var quantityPT = (itemDetail.quantityMP - itemDetail.quantityBoxes) * (productMP?.Presentation?.minimum ?? 1);
                var quantityBoxes = itemDetail.quantityBoxes * (productMP?.Presentation?.minimum ?? 1);

                amountMP += itemDetail.quantityMP;
                amountPT += itemDetail.quantityPT;
                amountBoxes += itemDetail.quantityBoxes;

                var id_metricUnitPresentation = productMP?.Presentation?.id_metricUnit;
                var metricUnitLbs = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Lbs"));
                var id_metricUnitLbs = metricUnitLbs?.id;
                var factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitLbs)?.factor ?? 1;
                lbsMP += Math.Round(quantityMP * factorAux, 2);
                lbsPT += Math.Round(quantityPT * factorAux, 2);
                lbsBoxes += Math.Round(quantityBoxes * factorAux, 2);

                var metricUnitKg = db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Kg"));
                var id_metricUnitKg = metricUnitKg?.id;
                factorAux = db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == id_metricUnitPresentation &&
                                                                             fod.id_metricDestiny == id_metricUnitKg)?.factor ?? 1;
                kgMP += Math.Round(quantityMP * factorAux, 2);
                kgPT += Math.Round(quantityPT * factorAux, 2);
                kgBoxes += Math.Round(quantityBoxes * factorAux, 2);
            }
            masteredDto.amountMP = amountMP;
            masteredDto.amountMPStr = amountMP.ToString("#");
            masteredDto.lbsMP = lbsMP;
            masteredDto.lbsMPStr = lbsMP.ToString();
            masteredDto.kgMP = kgMP;
            masteredDto.kgMPStr = kgMP.ToString();

            masteredDto.amountPT = amountPT;
            masteredDto.amountPTStr = amountPT.ToString("#");
            masteredDto.lbsPT = lbsPT;
            masteredDto.lbsPTStr = lbsPT.ToString();
            masteredDto.kgPT = kgPT;
            masteredDto.kgPTStr = kgPT.ToString();

            masteredDto.amountBoxes = amountBoxes;
            masteredDto.amountBoxesStr = amountBoxes.ToString("#");
            masteredDto.lbsBoxes = lbsBoxes;
            masteredDto.lbsBoxesStr = lbsBoxes.ToString();
            masteredDto.kgBoxes = kgBoxes;
            masteredDto.kgBoxesStr = kgBoxes.ToString();
        }

        

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewDetailsUpdate(MasteredDetailDTO item, bool? enabled)
        {
            var masteredDto = GetMasteredDTO();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = masteredDto.MasteredDetails.FirstOrDefault(it => it.id == item.id);

                    if (modelItem != null)
                    {
                        modelItem.id_sales = item.id_sales;
                        modelItem.saldoMP = item.saldoMP;

                        modelItem.quantityMP = item.quantityMP;

                        modelItem.id_productLotMP = item.id_productLotMP;

                        if (!string.IsNullOrEmpty(item.id_productLotMP))
                        {
                            string data = item.id_productLotMP;
                            string[] words = "".Split(':');
                            words = data.Split(':');
                            modelItem.id_productMP = int.Parse(words[0]);
                            modelItem.id_lotMP = words[1] == "" ? (int?)null : int.Parse(words[1]);
                            modelItem.id_lotBoxes = modelItem.id_lotMP;

                            var aItem = db.Item.FirstOrDefault(s => s.id == modelItem.id_productMP);
                            modelItem.codProductMP = aItem.masterCode;
                            var aProductionLot = db.ProductionLot.FirstOrDefault(s => s.id == modelItem.id_lotMP);
                            modelItem.loteMP = aProductionLot?.internalNumber ?? "";
                            modelItem.loteBoxes = modelItem.loteMP;
                            modelItem.lotMarked = item.lotMarked;
                            modelItem.id_metricUnitBoxed = aItem.ItemInventory.id_metricUnitInventory;
                            modelItem.metricUnitBoxed = aItem.ItemInventory.MetricUnit.code;
                        }
                        modelItem.id_productPT = item.id_productPT;
                        modelItem.id_customer = item.id_customer;
                        modelItem.quantityPT = item.quantityPT;
                        var aItemPT = db.Item.FirstOrDefault(s => s.id == item.id_productPT);
                        modelItem.codProductPT = aItemPT.masterCode;
                        modelItem.id_metricUnitMastered = aItemPT.ItemInventory.id_metricUnitInventory;
                        modelItem.metricUnitMastered = aItemPT.ItemInventory.MetricUnit.code;
                        modelItem.id_masteredWarehouseLocation = item.id_masteredWarehouseLocation;
                        modelItem.id_warehouseLocationBoxes = item.id_warehouseLocationBoxes;

                        modelItem.quantityBoxes = item.quantityBoxes;

                        InvParameterBalance invParameterBalance = new InvParameterBalance();
                        invParameterBalance.id_company = this.ActiveCompanyId;
                        invParameterBalance.id_Item = modelItem.id_productMP;
                        invParameterBalance.cut_Date = masteredDto.dateHoy.AsDateTime();
                        invParameterBalance.consolidado = true;
                        invParameterBalance.id_Warehouse = masteredDto.id_boxedWarehouse;
                        invParameterBalance.id_WarehouseLocation = masteredDto.id_boxedWarehouseLocation;
                        invParameterBalance.id_ProductionLot = null;

                        //var balaceItem = ServiceInventoryBalance.ValidateBalance(db, invParameterBalance);
                        //
                        //
                        //var sumBalanceItem = masteredDto.MasteredDetails.Where(b => b.id_boxedWarehouse == item.id_boxedWarehouse &&
                        //            b.id_boxedWarehouseLocation == item.id_boxedWarehouseLocation && b.id_productMP == modelItem.id_productMP).Sum(c => c.quantityMP);
                        //
                        ////if (balaceItem.SaldoActual < (sumBalanceItem + item.quantityMP))
                        //if (balaceItem.SaldoActual < (sumBalanceItem ))
                        //{
                        //    throw new Exception("La cantidad ingresada supera la suma total del saldo de los item agregados en el detalle.");
                        //}

                        UpdateSummary();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                foreach (var modelState in this.ModelState.Values)
                {
                    if (modelState.Errors.Count > 0)
                    {
                        ViewData["EditError"] = modelState.Errors.Last().ErrorMessage;
                        break;
                    }
                }
            }
            ViewBag.enabled = enabled;

            return PartialView("_GridViewDetails", masteredDto.MasteredDetails.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewDetailsDelete(System.Int32 id)
        {
            var masteredDto = GetMasteredDTO();

            try
            {
                var modelItem = masteredDto.MasteredDetails.FirstOrDefault(it => it.id == id);
                if (modelItem != null)
                {
                    masteredDto.MasteredDetails.Remove(modelItem);

                    UpdateSummary();
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            return PartialView("_GridViewDetails", masteredDto.MasteredDetails.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult DeleteDetail()
        {
            var masteredDto = GetMasteredDTO();

            for (int j = masteredDto.MasteredDetails.Count() - 1; j >= 0; j--)
            {
                var detail = masteredDto.MasteredDetails.ElementAt(j);
                masteredDto.MasteredDetails.Remove(detail);
            }

            var result = new
            {
                message = "OK",
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetTotales()
        {
            var masteredDto = GetMasteredDTO();

            var result = new
            {
                message = "OK",
                masteredDto.amountMPStr,
                masteredDto.lbsMPStr,
                masteredDto.kgMPStr,

                masteredDto.amountPTStr,
                masteredDto.lbsPTStr,
                masteredDto.kgPTStr,

                masteredDto.amountBoxesStr,
                masteredDto.lbsBoxesStr,
                masteredDto.kgBoxesStr
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadDetailSales(int? id_sales, int? id_customer)
        {
            var aSalesOrders = db.SalesOrder.Where(w => w.id == id_sales ||
                                                        ((id_customer == null ? true : w.id_customer == id_customer) && w.Document.DocumentState.code == "03"))
                                                        .AsEnumerable()
                                                        .Select(t => new
                                                        {
                                                            t.id,
                                                            t.Document.number,
                                                            fecha = t.Document.emissionDate.ToString("dd/MM/yyyy"),
                                                            nomClienteExt = t.Customer.Person.fullname_businessName
                                                        }).ToList();
            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_sales";
            p.ValueType = typeof(int);
            p.TextFormatString = "{0}";
            p.ValueField = "id";

            p.Columns.Add("number", "Número", 120);
            p.Columns.Add("fecha", "Fecha", 80);
            p.Columns.Add("nomClienteExt", "Nombre Cliente Ext.", 320);

            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            p.CallbackRouteValues = new { Controller = "Mastered", Action = "LoadDetailSales" };
            p.ClientSideEvents.BeginCallback = "DetailSales_BeginCallback";
            p.ClientSideEvents.EndCallback = "DetailSales_EndCallback";
            p.ClientSideEvents.Init = "OnItemDetailSalesInit";

            p.BindList(aSalesOrders);

            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadDetailItemMP(string id_productLotMP, int? id_sales, int? id_itemType, int? id_size, int? id_trademark, int? id_presentationMP,
                                             string decProducto, string codigoProducto, int[] id_tokenBoxLotes, int? id_boxedWarehouse, int? id_boxedWarehouseLocation
                                             , DateTime emissionDate
                                             )
        {
            var masteredDto = GetMasteredDTO();
            masteredDto.dateTimeEmision = emissionDate;
            masteredDto.dateTimeEmisionStr = emissionDate.ToString("dd-MM-yyyy");
            SetMasteredDTO(masteredDto);
            //Parametros.ParametrosBusquedaMastered parametrosBusquedaMastered = new Parametros.ParametrosBusquedaMastered();
            //parametrosBusquedaMastered.id_warehouse = id_boxedWarehouse;
            //parametrosBusquedaMastered.id_warehouseLocation = id_boxedWarehouseLocation;
            //parametrosBusquedaMastered.id_warehouseType = db.WarehouseType.FirstOrDefault(t => t.code.Equals("BE01"))?.id;
            //parametrosBusquedaMastered.emissionDate = emissionDate.ToIsoDateFormat();
            //parametrosBusquedaMastered.for_lot = 0;
            //
            //var parametrosBusquedaMasteredAux = new SqlParameter();
            //parametrosBusquedaMasteredAux.ParameterName = "@ParametrosBusquedaSaldoMastered";
            //parametrosBusquedaMasteredAux.Direction = ParameterDirection.Input;
            //parametrosBusquedaMasteredAux.SqlDbType = SqlDbType.NVarChar;
            //var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaMastered);
            //parametrosBusquedaMasteredAux.Value = jsonAux;
            //db.Database.CommandTimeout = 1200;
            //List<ResultMastered> productsLoteSaldo = db.Database.SqlQuery<ResultMastered>("exec inv_Consultar_Saldo_Mastered_StoredProcedure @ParametrosBusquedaSaldoMastered ", parametrosBusquedaMasteredAux).ToList();


            List<ResultMastered> productsLoteSaldo = ServiceInventoryMove
                                                                .GetSaldoMastered(  db,
                                                                                    this.ActiveCompanyId,
                                                                                    id_boxedWarehouse,
                                                                                    id_boxedWarehouseLocation,
                                                                                    masteredDto.dateTimeEmision.Date );
            if (id_sales != null && id_sales != 0)
            {
                var products = db.SalesOrderMPMaterialDetail.Where(w => w.id_salesOrder == id_sales).ToList();
                productsLoteSaldo = productsLoteSaldo.Where(w => products.FirstOrDefault(fod => fod.id_item == w.id_item) != null).ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(decProducto))
                {
                    productsLoteSaldo = productsLoteSaldo
                        .Where(w => w.name.Contains(decProducto)).ToList();
                }
                if (!String.IsNullOrEmpty(codigoProducto))
                {
                    productsLoteSaldo = productsLoteSaldo
                        .Where(w => w.code.Contains(codigoProducto)).ToList();
                }

                if (id_size != null && id_size > 0)
                {
                    productsLoteSaldo = productsLoteSaldo
                        .Where(w => w.id_size == id_size).ToList();
                }
                if (id_itemType != null && id_itemType > 0)
                {
                    productsLoteSaldo = productsLoteSaldo
                        .Where(w => w.id_itemType == id_itemType).ToList();
                }

                if (id_trademark != null && id_trademark > 0)
                {
                    productsLoteSaldo = productsLoteSaldo
                        .Where(w => w.id_trademark == id_trademark).ToList();
                }

                if (id_presentationMP != null && id_presentationMP > 0)
                {
                    productsLoteSaldo = productsLoteSaldo
                        .Where(w => w.id_presentationMP == id_presentationMP).ToList();
                }
            }

            if (id_tokenBoxLotes != null && id_tokenBoxLotes.Length > 0)
            {
                productsLoteSaldo = productsLoteSaldo.Where(w => id_tokenBoxLotes.Contains(w.id_lote)).ToList();
            }

            //  AQUI TAMBIEN HACER EL CAMBIO
            if (!string.IsNullOrEmpty(id_productLotMP) && productsLoteSaldo.FirstOrDefault(fod => fod.id == id_productLotMP) == null)            
            {
                string data = id_productLotMP;
                string[] words = "".Split(':');
                words = data.Split(':');
                
                int idItem = int.Parse(words[0]);
                int idLot = int.Parse(words[1]);

                var aItem = db.Item.FirstOrDefault(s => s.id == idItem);
                
                var aProductionLot = db.ProductionLot.FirstOrDefault(s => s.id == idLot);
                var aLot = db.Lot.FirstOrDefault(s => s.id == idLot);
                var aIdLote = aProductionLot?.id ?? aLot?.id;
                var aInternalNumber = aProductionLot?.internalNumber ?? (aLot?.internalNumber ?? "Sin Lote");
                decimal saldo = ServiceInventoryMove.GetRemainingBalance(   this.ActiveCompanyId, 
                                                                            aItem.id, 
                                                                            id_boxedWarehouse, 
                                                                            id_boxedWarehouseLocation, 
                                                                            aIdLote, 
                                                                            db,
                                                                            fechaCorte: masteredDto.dateTimeEmision.Date);
                productsLoteSaldo.Insert(0, new ResultMastered
                {
                    id = id_productLotMP,
                    code = aItem.masterCode,
                    name = aItem.name,
                    noLote = aInternalNumber,
                    saldoStr = saldo.ToString("0.00"),
                    id_item = aItem.id,
                    id_lote = aIdLote ?? 0,
                    saldo = saldo
                });
            }

            if(masteredDto.MasteredDetails.Count >= 1)
            {

                #region Correccion cargar saldo en de item lote

                productsLoteSaldo = (from result in productsLoteSaldo
                                     join detail in masteredDto
                                                            .MasteredDetails
                                                            .GroupBy(r => new { item = r.id_productMP, lote = r.loteMP })
                                                            .ToArray()
                                     on new { item = result.id_item, lote = result.noLote }
                                     equals new { item = detail.Key.item, lote = detail.Key.lote }
                                     into detgrp from det in detgrp.DefaultIfEmpty()
                                     select new ResultMastered
                                     {

                                         id = result.id,
                                         code = result.code,
                                         name = result.name,
                                         noLote = result.noLote,

                                         id_item = result.id_item,
                                         id_lote = result.id_lote,
                                         saldo = result.saldo - ((det?.Sum(t => t.quantityMP) ?? 0)),
                                         exitsInDetail = (det?.Key?.item != null)
                                     })
                                     .Where(r=> r.saldo > 0 || r.exitsInDetail)
                                     .Select(r=> new ResultMastered
                                     {
                                         id = r.id,
                                         code = r.code,
                                         name = r.name,
                                         noLote = r.noLote,
                                         saldoStr = r.saldo.ToString("0.00"),
                                         id_item = r.id_item,
                                         id_lote = r.id_lote,
                                         saldo = r.saldo
                                     })
                                    .ToList();

                //var detailsToRemove = masteredDto.MasteredDetails
                //        .Select(detail => new { Id = detail.id_productMP, Lote = detail.loteMP })
                //        .ToList();
                //List<int> idsToRemove = masteredDto.MasteredDetails.Select(detail => detail.id_productMP).ToList();
                //
                //productsLoteSaldo.RemoveAll(item =>detailsToRemove
                //.Any(detail =>item.id_item == detail.Id && item.noLote == detail.Lote));     
                #endregion

            }

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_productLotMP";

            p.ValueType = typeof(string);
            p.TextFormatString = "{0}, {1}";
            p.ValueField = "id";

            p.Columns.Add("code", "Cod.", 70);
            p.Columns.Add("name", "Nombre", 320);
            p.Columns.Add("noLote", "No. Lote", 120);
            p.Columns.Add("saldoStr", "Saldo", 70);


            p.CallbackPageSize = 5;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            p.CallbackRouteValues = new { Controller = "Mastered", Action = "LoadDetailItemMP" };
            p.ClientSideEvents.BeginCallback = "DetailItemMP_BeginCallback";
            p.ClientSideEvents.EndCallback = "DetailItemMP_EndCallback";
            p.ClientSideEvents.Validation = "OnItemDetailItemMPValidation";
            p.BindList(productsLoteSaldo);

            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductLotMPChanged(  string id_productLotMP, 
                                                int? id_boxedWarehouse, 
                                                int? id_boxedWarehouseLocation)
        {
            var masteredDto = GetMasteredDTO();

            var result = new
            {
                message = "",
                codProductMP = "",
                loteMP = "",
                saldoMP = 0.00M,
                quantityMP = 0.00M
            };

            if (!string.IsNullOrEmpty(id_productLotMP))
            {
                string data = id_productLotMP;
                string[] words = "".Split(':');
                words = data.Split(':');
                int id_item = int.Parse(words[0]);
                int? id_lot = words[1] == "" ? (int?)null : int.Parse(words[1]);

                var aItem = db.Item.FirstOrDefault(s => s.id == id_item);
                var aProductionLot = db.ProductionLot.FirstOrDefault(s => s.id == id_lot);
                var aLot = db.Lot.FirstOrDefault(s => s.id == id_lot);
                decimal saldo = ServiceInventoryMove.GetRemainingBalance(   this.ActiveCompanyId,
                                                                            aItem.id, 
                                                                            id_boxedWarehouse, 
                                                                            id_boxedWarehouseLocation, 
                                                                            id_lot, 
                                                                            db,
                                                                            fechaCorte: masteredDto.dateTimeEmision.Date);
                #region Correccion cargar saldo en de item lote
                string loteMP = aProductionLot?.internalNumber ?? (aLot?.internalNumber ?? "");
                
                if (masteredDto.MasteredDetails.Count >= 1)
                {
                    decimal sumaCantidad = masteredDto
                                                .MasteredDetails
                                                .Where(r => r.id_productMP == id_item
                                                            && r.loteMP == loteMP)
                                                .Sum(r => r.quantityMP);

                    saldo = saldo - sumaCantidad;


                }

                #endregion
                decimal saldo2Decimal = decimal.Truncate(saldo * 100) / 100;
                result = new
                {
                    message = "",
                    codProductMP = aItem.masterCode,
                    loteMP = loteMP,
                    saldoMP = saldo2Decimal,
                    quantityMP = saldo2Decimal
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult QuantityMPValueChanged(int? id_sales, string id_productLotMP, decimal quantityMP, int? id_productPT)
        {
            var result = new
            {
                message = "",
                quantityPT = 0.00M,
                quantityBoxes = 0.00M
            };

            if (!string.IsNullOrEmpty(id_productLotMP) && id_productPT != null)
            {
                string data = id_productLotMP;
                string[] words = "".Split(':');
                words = data.Split(':');
                int id_item = int.Parse(words[0]);
                int? id_lot = words[1] == "" ? (int?)null : int.Parse(words[1]);

                var aItem = db.Item.FirstOrDefault(s => s.id == id_item);
                var aProductionLot = db.ProductionLot.FirstOrDefault(s => s.id == id_lot);

                if (id_sales != null && id_sales != 0)
                {
                    var aSalesOrder = db.SalesOrder.FirstOrDefault(s => s.id == id_sales);
                    var aSalesOrderMPMaterialDetail = aSalesOrder.SalesOrderMPMaterialDetail.FirstOrDefault(s => s.id_item == aItem.id && s.id_metricUnit == aItem.ItemInventory.id_metricUnitInventory);
                    var aSalesOrderDetailMPMaterialDetail = aSalesOrderMPMaterialDetail.SalesOrderDetailMPMaterialDetail.FirstOrDefault(s => s.SalesOrderDetail.id_item == id_productPT);
                    var quantyFormula = aSalesOrderMPMaterialDetail.quantity / aSalesOrderDetailMPMaterialDetail.SalesOrderDetail.quantityApproved;

                    var aQuantityPT = decimal.Truncate(quantityMP / quantyFormula);
                    result = new
                    {
                        message = "",
                        quantityPT = aQuantityPT,
                        quantityBoxes = decimal.Truncate(quantityMP - (aQuantityPT * quantyFormula))
                    };
                }
                else
                {
                    var aItemPT = db.Item.FirstOrDefault(s => s.id == id_productPT);
                    if (!aItemPT.hasFormulation)
                    {
                        result = new
                        {
                            message = "El Producto: " + aItemPT.name + " no tiene Formulación requerida para gestionar este detalle, configúrelo e intente de nuevo",
                            quantityPT = 0.00M,
                            quantityBoxes = 0.00M
                        };
                    }
                    else
                    {
                        var quantyFormula = aItemPT.ItemIngredient.FirstOrDefault(fod => fod.id_ingredientItem == aItem.id && fod.id_metricUnit == aItem.ItemInventory.id_metricUnitInventory)?.amount;
                        if (quantyFormula == null)
                        {
                            result = new
                            {
                                message = "El Producto: " + aItemPT.name + " no tiene definido en su Formulación la materia prima (MP) : " + aItem.name + " requerida para gestionar este detalle, configúrelo e intente de nuevo",
                                quantityPT = 0.00M,
                                quantityBoxes = 0.00M
                            };
                        }
                        else
                        {
                            var aQuantityPT = decimal.Truncate(quantityMP / quantyFormula.Value);
                            result = new
                            {
                                message = "",
                                quantityPT = aQuantityPT,
                                quantityBoxes = quantityMP - (aQuantityPT * quantyFormula.Value)
                            };
                        }
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadDetailItemPT(int? id_productPT, int? id_sales, string id_productLotMP, int? id_presentationPT)
        {
            List<Item> listItem = new List<Item>();
            Item aItem = new Item();
            if (!string.IsNullOrEmpty(id_productLotMP))
            {
                string data = id_productLotMP;
                string[] words = "".Split(':');
                words = data.Split(':');
                var id_item = int.Parse(words[0]);
                aItem = db.Item.FirstOrDefault(s => s.id == id_item);
            }
            var id_metricUnitInventoryAux = (aItem.ItemInventory == null ? (int?)null : aItem.ItemInventory.id_metricUnitInventory);
            if (id_sales != null && id_sales != 0)
            {
                var aSalesOrder = db.SalesOrder.FirstOrDefault(s => s.id == id_sales);
                var aSalesOrderMPMaterialDetails = aSalesOrder.SalesOrderMPMaterialDetail.Where(s => s.id_item == aItem.id && s.id_metricUnit == id_metricUnitInventoryAux).ToList();
                foreach (var item in aSalesOrderMPMaterialDetails)
                {
                    var aItemPT = item.SalesOrderDetailMPMaterialDetail.FirstOrDefault()?.SalesOrderDetail.Item;
                    listItem.Add(aItemPT);
                }
            }
            else
            {
                listItem = db.Item
                     .Where(w => (w.InventoryLine.code.Equals("PT"))
                             && (w.ItemType != null)
                             && (w.Presentation != null)
                             && w.isActive
                             && w.hasFormulation).ToList();

                if (id_presentationPT != null && id_presentationPT > 0)
                {
                    listItem = listItem
                        .Where(w => w.Presentation.id == id_presentationPT).ToList();
                }
                for (int i = listItem.Count - 1; i >= 0; i--)
                {
                    var detail = listItem.ElementAt(i);
                    if (!(detail.ItemIngredient != null && detail.ItemIngredient.Count() > 0 &&
                        detail.ItemIngredient.FirstOrDefault(fod => fod.id_ingredientItem == aItem.id &&
                                                                    fod.id_metricUnit == id_metricUnitInventoryAux) != null))
                    {
                        listItem.Remove(detail);
                    }
                }
            }

            var aItemPTAux = db.Item.FirstOrDefault(s => s.id == id_presentationPT);
            if (aItemPTAux != null && !listItem.Contains(aItemPTAux))
            {
                listItem.Insert(0, aItemPTAux);
            }

            var aItemPTAux2 = db.Item.FirstOrDefault(s => s.id == id_productPT);
            if (aItemPTAux2 != null && !listItem.Contains(aItemPTAux2))
            {
                listItem.Insert(0, aItemPTAux2);
            }

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_productPT";

            p.ValueType = typeof(int);

            p.TextFormatString = "{0}, {1}";
            p.ValueField = "id";

            p.Columns.Add("code", "Cod.", 70);
            p.Columns.Add("name", "Nombre", 320);

            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            p.CallbackRouteValues = new { Controller = "Mastered", Action = "LoadDetailItemPT" };
            p.ClientSideEvents.BeginCallback = "DetailItemPT_BeginCallback";
            p.ClientSideEvents.EndCallback = "DetailItemPT_EndCallback";
            p.ClientSideEvents.SelectedIndexChanged = "DetailItemPT_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnItemDetailItemPTValidation";
            p.BindList(listItem.Select(s => new { s.id, code = s.masterCode, s.name }).ToList());

            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductPTChanged(int? id_productPT)
        {
            var result = new
            {
                message = "",
                codProductPT = ""
            };

            if (id_productPT != null)
            {
                var aItem = db.Item.FirstOrDefault(s => s.id == id_productPT);
                result = new
                {
                    message = "",
                    codProductPT = aItem.masterCode
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadDetailCustomer(int? id_customerCurrent, int? id_sales, int? id_customer)
        {
            List<string> listRol = new List<string>();
            listRol.Add("Cliente Exterior");
            var aCustomers = (DataProviderPerson.RolsByCompanyAndCurrent(ActiveCompany.id, null, listRol) as List<Person>).Where(w => w.id != id_customerCurrent)
                .Select(s => new
                {
                    s.id,
                    name = s.fullname_businessName
                }).ToList();

            if (id_sales != null)
            {
                var aSalesOrder = db.SalesOrder.FirstOrDefault(s => s.id == id_sales);
                var aACustomer = aCustomers.FirstOrDefault(fod => fod.id == aSalesOrder.id_customer);
                if (aACustomer != null)
                {
                    aCustomers.Remove(aACustomer);
                }
                else
                {
                    aACustomer = new { id = aSalesOrder.id_customer.Value, name = aSalesOrder.Customer.Person.fullname_businessName };
                }
                aCustomers.Insert(0, aACustomer);
            }
            else
            {
                if (id_customer != null)
                {
                    var aPerson = db.Person.FirstOrDefault(s => s.id == id_customer);
                    var aACustomer = aCustomers.FirstOrDefault(fod => fod.id == id_customer);
                    if (aACustomer != null)
                    {
                        aCustomers.Remove(aACustomer);
                    }
                    else
                    {
                        aACustomer = new { id = id_customer.Value, name = aPerson.fullname_businessName };
                    }
                    aCustomers.Insert(0, aACustomer);
                }
            }

            if (id_customerCurrent != null)
            {
                var aPerson = db.Person.FirstOrDefault(s => s.id == id_customerCurrent);
                var aACustomer = aCustomers.FirstOrDefault(fod => fod.id == id_customerCurrent);
                if (aACustomer != null)
                {
                    if (aCustomers[0].id != aACustomer.id)
                    {
                        aCustomers.Remove(aACustomer);
                        aCustomers.Insert(0, aACustomer);
                    }
                }
                else
                {
                    aACustomer = new { id = id_customerCurrent.Value, name = aPerson.fullname_businessName };
                    aCustomers.Insert(0, aACustomer);
                }
            }

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_customer";

            p.TextField = "name";
            p.ValueField = "id";
            p.ValueType = typeof(int);

            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            p.CallbackRouteValues = new { Controller = "Mastered", Action = "LoadDetailCustomer" };
            p.ClientSideEvents.BeginCallback = "DetailCustomer_BeginCallback";
            p.ClientSideEvents.EndCallback = "DetailCustomer_EndCallback";
            p.BindList(aCustomers);

            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadDetailWareHouseLocation(int? id_responsable, int? id_masteredWarehouse)
        {
            id_responsable = db.Person.FirstOrDefault(fod => fod.identification_number == ActiveCompany.ruc)?.id;
            var masteredDTO = GetMasteredDTO();
            if (id_masteredWarehouse != masteredDTO.id_masteredWarehouse)
            {
                masteredDTO.masteredWarehouseLocation = "";
                masteredDTO.id_masteredWarehouseLocation = null;
            }
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            List<WarehouseLocation> aWarehouseLocations = new List<WarehouseLocation>();
            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == id_responsable);
            var id_WarehouseFreezeAux = aProviderRawMaterial?.id_WarehouseFreeze;
            int? id_warehouseLocationFreezeAux = null;
            if (id_WarehouseFreezeAux != null && id_WarehouseFreezeAux == id_masteredWarehouse)
            {
                id_warehouseLocationFreezeAux = aProviderRawMaterial?.id_warehouseLocationFreeze;
                if (id_warehouseLocationFreezeAux != null)
                {
                    var aWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationFreezeAux);
                    aWarehouseLocations.Add(new WarehouseLocation
                    {
                        id = id_warehouseLocationFreezeAux.Value,
                        isActive = aWarehouseLocation?.isActive ?? true,
                        id_company = aWarehouseLocation?.id_company ?? ActiveCompany.id,
                        name = aWarehouseLocation?.name
                    });
                }
            }
            SelectListItem aSelectListItem = null;
            if (masteredDTO.id_masteredWarehouseLocation != null)
            {
                aSelectListItem = new SelectListItem
                {
                    Text = masteredDTO.masteredWarehouseLocation,
                    Value = masteredDTO.id_masteredWarehouseLocation.ToString(),
                    Selected = true
                };
            }

            aSelectListItems.AddRange(aWarehouseLocations.Where(g => g.id != masteredDTO.id_masteredWarehouseLocation &&
                                                                      g.id_company == ActiveCompany.id && g.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            aSelectListItems.AddRange(db.WarehouseLocation.Where(t => t.id_warehouse == id_masteredWarehouse && t.id_company == ActiveCompany.id &&
                                                                      t.isActive && t.id != masteredDTO.id_masteredWarehouseLocation &&
                                                                      t.id != id_warehouseLocationFreezeAux).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            if (masteredDTO.id_masteredWarehouseLocation != null)
            {
                aSelectListItems.Insert(0, aSelectListItem);
            }
            else
            {
                var aId_WarehouseLocationFreezeStr = id_warehouseLocationFreezeAux == null ? "" : id_warehouseLocationFreezeAux.Value.ToString();
                var aASelectListItem = aSelectListItems.FirstOrDefault(fod => fod.Value == aId_WarehouseLocationFreezeStr);
                if (aASelectListItem != null)
                {
                    aASelectListItem.Selected = true;
                    masteredDTO.id_masteredWarehouseLocation = id_warehouseLocationFreezeAux;
                    masteredDTO.masteredWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationFreezeAux)?.name;
                }
            }

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_masteredWarehouseLocation";

            p.TextField = "Text";
            p.ValueField = "Value";
            p.ValueType = typeof(int);

            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            p.CallbackRouteValues = new { Controller = "Mastered", Action = "LoadDetailWareHouseLocation" };
            p.ClientSideEvents.BeginCallback = "DetailWarehouseLocation_BeginCallback";
            p.BindList(aSelectListItems);

            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadDetailWarehouseLocationBoxes(int? id_responsable, int? id_warehouseBoxes)
        {
            id_responsable = db.Person.FirstOrDefault(fod => fod.identification_number == ActiveCompany.ruc)?.id;
            var masteredDTO = GetMasteredDTO();
            if (id_warehouseBoxes != masteredDTO.id_warehouseBoxes)
            {
                masteredDTO.warehouseLocationBoxes = "";
                masteredDTO.id_warehouseLocationBoxes = null;
            }
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            List<WarehouseLocation> aWarehouseLocations = new List<WarehouseLocation>();
            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == id_responsable);
            var id_WarehouseLooseCartonAux = aProviderRawMaterial?.id_WarehouseLooseCarton;
            int? id_WarehouseLocationLooseCartonAux = null;
            if (id_WarehouseLooseCartonAux != null && id_WarehouseLooseCartonAux == id_warehouseBoxes)
            {
                id_WarehouseLocationLooseCartonAux = aProviderRawMaterial?.id_WarehouseLocationLooseCarton;
                if (id_WarehouseLocationLooseCartonAux != null)
                {
                    var aWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_WarehouseLocationLooseCartonAux);
                    aWarehouseLocations.Add(new WarehouseLocation
                    {
                        id = id_WarehouseLocationLooseCartonAux.Value,
                        isActive = aWarehouseLocation?.isActive ?? true,
                        id_company = aWarehouseLocation?.id_company ?? ActiveCompany.id,
                        name = aWarehouseLocation?.name
                    });
                }
            }
            SelectListItem aSelectListItem = null;
            if (masteredDTO.id_warehouseLocationBoxes != null)
            {
                aSelectListItem = new SelectListItem
                {
                    Text = masteredDTO.warehouseLocationBoxes,
                    Value = masteredDTO.id_warehouseLocationBoxes.ToString(),
                    Selected = true
                };
            }

            aSelectListItems.AddRange(aWarehouseLocations.Where(g => g.id != masteredDTO.id_warehouseLocationBoxes &&
                                                                      g.id_company == ActiveCompany.id && g.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            aSelectListItems.AddRange(db.WarehouseLocation.Where(t => t.id_warehouse == id_warehouseBoxes && t.id_company == ActiveCompany.id &&
                                                                      t.isActive && t.id != masteredDTO.id_warehouseLocationBoxes &&
                                                                      t.id != id_WarehouseLocationLooseCartonAux).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            if (masteredDTO.id_warehouseLocationBoxes != null)
            {
                aSelectListItems.Insert(0, aSelectListItem);
            }
            else
            {
                var aId_WarehouseLocationLooseCartonStr = id_WarehouseLocationLooseCartonAux == null ? "" : id_WarehouseLocationLooseCartonAux.Value.ToString();
                var aASelectListItem = aSelectListItems.FirstOrDefault(fod => fod.Value == aId_WarehouseLocationLooseCartonStr);
                if (aASelectListItem != null)
                {
                    aASelectListItem.Selected = true;
                    masteredDTO.id_warehouseLocationBoxes = id_WarehouseLocationLooseCartonAux;
                    masteredDTO.warehouseLocationBoxes = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_WarehouseLocationLooseCartonAux)?.name;
                }
            }

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_warehouseLocationBoxes";

            p.TextField = "Text";
            p.ValueField = "Value";
            p.ValueType = typeof(int);

            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            p.CallbackRouteValues = new { Controller = "Mastered", Action = "LoadDetailWarehouseLocationBoxes" };
            p.ClientSideEvents.BeginCallback = "DetailWarehouseLocationBoxes_BeginCallback";
            p.BindList(aSelectListItems);

            return GridViewExtension.GetComboBoxCallbackResult(p);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetCustomerSales(int? id_sales)
        {
            var result = new
            {
                message = "",
                id_customerSale = (int?)null
            };

            if (id_sales != null)
            {
                var aSalesOrder = db.SalesOrder.FirstOrDefault(s => s.id == id_sales);
                result = new
                {
                    message = "",
                    id_customerSale = aSalesOrder.id_customer
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItsRepeatedProductLotMP(string id_productLotMP)
        {
            var masteredDto = GetMasteredDTO();
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };
            if (!string.IsNullOrEmpty(id_productLotMP))
            {
                string data = id_productLotMP;
                string[] words = "".Split(':');
                words = data.Split(':');
                int id_item = int.Parse(words[0]);
                int? id_lot = words[1] == "" ? (int?)null : int.Parse(words[1]);

                var aItem = db.Item.FirstOrDefault(s => s.id == id_item);
                var aProductionLot = db.ProductionLot.FirstOrDefault(s => s.id == id_lot);

                var itemItemProductLotMPAux = masteredDto.MasteredDetails.FirstOrDefault(fod => fod.id_productLotMP == id_productLotMP);

                if (itemItemProductLotMPAux != null)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede repetir el Producto de MP: " + (aItem?.name ?? "SIN PRODUCTO") + " con el Lote: " + (aProductionLot?.internalNumber ?? "SIN LOTE") + " en el detalle."
                    };
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion GridViewDetails

        #region Combobox

        private void BuildComboBoxState()
        {
            ViewData["Estados"] = db.DocumentState
                .Where(e => e.isActive
                    && e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoMastered))
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                }).ToList();
        }

        public ActionResult ComboBoxState()
        {
            BuildComboBoxState();
            return PartialView("_ComboBoxState");
        }

        private void BuildComboBoxResponsableIndex()
        {
            ViewData["ResponsableIndex"] = (db.Employee.Where(g => g.Person.isActive))
                .Select(s => new SelectListItem
                {
                    Text = s.Person.fullname_businessName,
                    Value = s.id.ToString(),
                }).ToList();
        }

        public ActionResult ComboBoxResponsableIndex()
        {
            BuildComboBoxResponsableIndex();
            return PartialView("_ComboBoxResponsableIndex");
        }

        private void BuildComboBoxTurnIndex()
        {
            ViewData["TurnIndex"] = (db.Turn.Where(g => g.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
        }

        public ActionResult ComboBoxTurnIndex()
        {
            BuildComboBoxTurnIndex();
            return PartialView("_ComboBoxTurnIndex");
        }

        private void BuildComboBoxBoxedWarehouseIndex()
        {
            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            var model = db.Warehouse.Where(t => t.id_company == ActiveCompany.id && t.isActive &&
                                                t.WarehouseType.code.Equals("BE01")).ToList();

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            ViewData["BoxedWarehouseIndex"] = model
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxBoxedWarehouseIndex()
        {
            BuildComboBoxBoxedWarehouseIndex();
            return PartialView("_ComboBoxBoxedWarehouseIndex");
        }

        private void BuildComboBoxBoxedWarehouseLocationIndex(int? id_boxedWarehouse = null)
        {
            ViewData["BoxedWarehouseLocationIndex"] = db.WarehouseLocation.Where(t => t.id_warehouse == id_boxedWarehouse && t.id_company == ActiveCompany.id && t.isActive).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxBoxedWarehouseLocationIndex(int? id_boxedWarehouse)
        {
            BuildComboBoxBoxedWarehouseLocationIndex(id_boxedWarehouse);
            return PartialView("_ComboBoxBoxedWarehouseLocationIndex");
        }

        private void BuildTokenBoxBoxedItemsIndex()
        {
            var codeMaster = db.Setting.FirstOrDefault(fod => fod.code == "PMASTER")?.value ?? "";
            ViewData["BoxedItemsIndex"] = db.Item.Where(i => i.id_company == ActiveCompany.id &&
                                                         i.isActive &&
                                                         (i.InventoryLine.code.Equals("PP")) &&
                                                         (i.ItemType != null) && (i.ItemType.ProcessType != null) &&
                                                         (i.Presentation != null) &&
                                                         (i.Presentation.code.Substring(0, 1) != codeMaster))
                .Select(s => new SelectListItem
                {
                    Text = s.masterCode + " - " + s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult TokenBoxBoxedItemsIndex()
        {
            BuildTokenBoxBoxedItemsIndex();
            return PartialView("_TokenBoxBoxedItemsIndex");
        }

        private void BuildComboBoxMasteredWarehouseIndex()
        {
            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            var model = db.Warehouse.Where(t => t.id_company == ActiveCompany.id && t.isActive &&
                                                t.WarehouseType.code.Equals("BCC01")).ToList();

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            ViewData["MasteredWarehouseIndex"] = model
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxMasteredWarehouseIndex()
        {
            BuildComboBoxMasteredWarehouseIndex();
            return PartialView("_ComboBoxMasteredWarehouseIndex");
        }

        private void BuildComboBoxMasteredWarehouseLocationIndex(int? id_masteredWarehouse = null)
        {
            ViewData["MasteredWarehouseLocationIndex"] = db.WarehouseLocation.Where(t => t.id_warehouse == id_masteredWarehouse && t.id_company == ActiveCompany.id && t.isActive).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxMasteredWarehouseLocationIndex(int? id_masteredWarehouse)
        {
            BuildComboBoxMasteredWarehouseLocationIndex(id_masteredWarehouse);
            return PartialView("_ComboBoxMasteredWarehouseLocationIndex");
        }

        private void BuildTokenBoxMasteredItemsIndex()
        {
            ViewData["MasteredItemsIndex"] = db.Item.Where(i => i.id_company == ActiveCompany.id &&
                                                         i.isActive &&
                                                         (i.InventoryLine.code.Equals("PP") || i.InventoryLine.code.Equals("PT")) &&
                                                         (i.ItemType != null) && (i.ItemType.ProcessType != null) &&
                                                         (i.Presentation != null))
                .Select(s => new SelectListItem
                {
                    Text = s.masterCode + " - " + s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult TokenBoxMasteredItemsIndex()
        {
            BuildTokenBoxMasteredItemsIndex();
            return PartialView("_TokenBoxMasteredItemsIndex");
        }

        private void BuildComboBoxResponsable()
        {
            var masteredDTO = GetMasteredDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = masteredDTO.responsable,
                Value = masteredDTO.id_responsable.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Employee.Where(g => g.Person.isActive && g.id != masteredDTO.id_responsable)
                .Select(s => new SelectListItem
                {
                    Text = s.Person.fullname_businessName,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["Responsable"] = aSelectListItems;
        }

        public ActionResult ComboBoxResponsable()
        {
            BuildComboBoxResponsable();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxResponsable");
        }

        private void BuildComboBoxTurn()
        {
            var masteredDTO = GetMasteredDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = masteredDTO.turn,
                Value = masteredDTO.id_turn.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Turn.Where(g => g.isActive && g.id != masteredDTO.id_turn)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["Turn"] = aSelectListItems;
        }

        public ActionResult ComboBoxTurn()
        {
            BuildComboBoxTurn();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxTurn");
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
            return PartialView("_ComboBoxItemType");
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
            return PartialView("_ComboBoxSize");
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
            return PartialView("_ComboBoxTrademark");
        }

        private void BuildComboBoxCustomer()
        {
            List<string> listRol = new List<string>();
            listRol.Add("Cliente Exterior");

            ViewData["Customer"] = (DataProviderPerson.RolsByCompanyAndCurrent(ActiveCompany.id, null, listRol) as List<Person>)
                .Select(s => new SelectListItem
                {
                    Text = s.fullname_businessName,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxCustomer()
        {
            BuildComboBoxCustomer();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxCustomer");
        }

        private void BuildComboBoxPresentationMP()
        {
            ViewData["PresentationMP"] = (db.Presentation.Where(w => w.isActive && w.Item.FirstOrDefault(fod => fod.isActive && (fod.InventoryLine.code == "PP" || fod.InventoryLine.code == "PT")) != null))
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxPresentationMP()
        {
            BuildComboBoxPresentationMP();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxPresentationMP");
        }

        private void BuildComboBoxPresentationPT()
        {
            ViewData["PresentationPT"] = (db.Presentation.Where(w => w.isActive && w.Item.FirstOrDefault(fod => fod.isActive && (fod.InventoryLine.code == "PT")) != null))
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxPresentationPT()
        {
            BuildComboBoxPresentationPT();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxPresentationPT");
        }

        private void BuildTokenBoxLotes(int? id_boxedWarehouse, int? id_boxedWarehouseLocation, DateTime emissionDate)
        {
            Parametros.ParametrosBusquedaMastered parametrosBusquedaMastered = new Parametros.ParametrosBusquedaMastered();
            parametrosBusquedaMastered.id_warehouse = id_boxedWarehouse;
            parametrosBusquedaMastered.id_warehouseLocation = id_boxedWarehouseLocation;
            parametrosBusquedaMastered.emissionDate = emissionDate.ToIsoDateFormat();
            parametrosBusquedaMastered.id_warehouseType = db.WarehouseType.FirstOrDefault(t => t.code.Equals("BE01"))?.id;
            parametrosBusquedaMastered.for_lot = 1;

            var parametrosBusquedaMasteredAux = new SqlParameter();
            parametrosBusquedaMasteredAux.ParameterName = "@ParametrosBusquedaSaldoMastered";
            parametrosBusquedaMasteredAux.Direction = ParameterDirection.Input;
            parametrosBusquedaMasteredAux.SqlDbType = SqlDbType.NVarChar;
            var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaMastered);
            parametrosBusquedaMasteredAux.Value = jsonAux;
            db.Database.CommandTimeout = 1200;
            List<ResultMastered> aAAAInventoryMoveDetail = db.Database.SqlQuery<ResultMastered>("exec inv_Consultar_Saldo_Mastered_StoredProcedure @ParametrosBusquedaSaldoMastered ", parametrosBusquedaMasteredAux).ToList();

            ViewData["Lotes"] = aAAAInventoryMoveDetail
               .Select(s => new SelectListItem
               {
                   Text = s.noLote,
                   Value = s.id_lote == 0 ? "" : s.id_lote.ToString()
               }).ToList();
        }

        public ActionResult TokenBoxLotes(int? id_boxedWarehouse, int? id_boxedWarehouseLocation, DateTime emissionDate)
        {
            BuildTokenBoxLotes(id_boxedWarehouse, id_boxedWarehouseLocation, emissionDate);
            ViewBag.enabled = true;
            return PartialView("_TokenBoxLotes");
        }

        private void BuildComboBoxBoxedWarehouse(int? id_responsable = null)
        {
            id_responsable = db.Person.FirstOrDefault(fod => fod.identification_number == ActiveCompany.ruc)?.id;
            var masteredDTO = GetMasteredDTO();
            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            var model = new List<Warehouse>();
            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == id_responsable);
            if (aProviderRawMaterial?.id_WarehouseCarton != null)
            {
                model.Add(new Warehouse
                {
                    id = aProviderRawMaterial.id_WarehouseCarton.Value,
                    name = db.Warehouse.FirstOrDefault(fod => fod.id == aProviderRawMaterial.id_WarehouseCarton)?.name
                });
            }
            model.AddRange(db.Warehouse.Where(t => t.id_company == ActiveCompany.id && t.isActive && t.id != aProviderRawMaterial.id_WarehouseCarton &&
                                              t.WarehouseType.code.Equals("BE01")).ToList());

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            SelectListItem aSelectListItem = null;
            if (masteredDTO.id_boxedWarehouse != null)
            {
                aSelectListItem = new SelectListItem
                {
                    Text = masteredDTO.boxedWarehouse,
                    Value = masteredDTO.id_boxedWarehouse.ToString(),
                    Selected = true
                };
            }

            aSelectListItems.AddRange(model.Where(g => g.id != masteredDTO.id_boxedWarehouse)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            if (masteredDTO.id_boxedWarehouse != null)
            {
                aSelectListItems.Insert(0, aSelectListItem);
            }
            else
            {
                var aId_WarehouseCartonStr = aProviderRawMaterial?.id_WarehouseCarton == null ? "" : aProviderRawMaterial.id_WarehouseCarton.Value.ToString();
                var aASelectListItem = aSelectListItems.FirstOrDefault(fod => fod.Value == aId_WarehouseCartonStr);
                if (aASelectListItem != null)
                {
                    aASelectListItem.Selected = true;
                    masteredDTO.id_boxedWarehouse = aProviderRawMaterial.id_WarehouseCarton;
                    masteredDTO.boxedWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == aProviderRawMaterial.id_WarehouseCarton)?.name;
                }
            }

            ViewData["BoxedWarehouse"] = aSelectListItems;
        }

        public ActionResult ComboBoxBoxedWarehouse(int? id_responsable)
        {
            BuildComboBoxBoxedWarehouse(id_responsable);
            ViewBag.enabled = true;
            return PartialView("_ComboBoxBoxedWarehouse");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetValueBoxedWarehouse()
        {
            var masteredDto = GetMasteredDTO();
            var result = new
            {
                message = "",
                id_boxedWarehouse = masteredDto.id_boxedWarehouse
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void BuildComboBoxBoxedWarehouseLocation(int? id_responsable = null, int? id_boxedWarehouse = null)
        {
            id_responsable = db.Person.FirstOrDefault(fod => fod.identification_number == ActiveCompany.ruc)?.id;
            var masteredDTO = GetMasteredDTO();
            if (id_boxedWarehouse != masteredDTO.id_boxedWarehouse)
            {
                masteredDTO.boxedWarehouseLocation = "";
                masteredDTO.id_boxedWarehouseLocation = null;
            }
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            List<WarehouseLocation> aWarehouseLocations = new List<WarehouseLocation>();
            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == id_responsable);
            var id_WarehouseCartonAux = aProviderRawMaterial?.id_WarehouseCarton;
            int? id_warehouseLocationCartonAux = null;
            if (id_WarehouseCartonAux != null && id_WarehouseCartonAux == id_boxedWarehouse)
            {
                id_warehouseLocationCartonAux = aProviderRawMaterial?.id_warehouseLocationCarton;
                if (id_warehouseLocationCartonAux != null)
                {
                    var aWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationCartonAux);
                    aWarehouseLocations.Add(new WarehouseLocation
                    {
                        id = id_warehouseLocationCartonAux.Value,
                        isActive = aWarehouseLocation?.isActive ?? true,
                        id_company = aWarehouseLocation?.id_company ?? ActiveCompany.id,
                        name = aWarehouseLocation?.name
                    });
                }
            }
            SelectListItem aSelectListItem = null;
            if (masteredDTO.id_boxedWarehouseLocation != null)
            {
                aSelectListItem = new SelectListItem
                {
                    Text = masteredDTO.boxedWarehouseLocation,
                    Value = masteredDTO.id_boxedWarehouseLocation.ToString(),
                    Selected = true
                };
            }

            aSelectListItems.AddRange(aWarehouseLocations.Where(g => g.id != masteredDTO.id_boxedWarehouseLocation &&
                                                                      g.id_company == ActiveCompany.id && g.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            aSelectListItems.AddRange(db.WarehouseLocation.Where(t => t.id_warehouse == id_boxedWarehouse && t.id_company == ActiveCompany.id &&
                                                                      t.isActive && t.id != masteredDTO.id_boxedWarehouseLocation &&
                                                                      t.id != id_warehouseLocationCartonAux).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            if (masteredDTO.id_boxedWarehouseLocation != null)
            {
                aSelectListItems.Insert(0, aSelectListItem);
            }
            else
            {
                var aId_WarehouseLocationCartonStr = id_warehouseLocationCartonAux == null ? "" : id_warehouseLocationCartonAux.Value.ToString();
                var aASelectListItem = aSelectListItems.FirstOrDefault(fod => fod.Value == aId_WarehouseLocationCartonStr);
                if (aASelectListItem != null)
                {
                    aASelectListItem.Selected = true;
                    masteredDTO.id_boxedWarehouseLocation = id_warehouseLocationCartonAux;
                    masteredDTO.boxedWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationCartonAux)?.name;
                }
            }
            ViewData["BoxedWarehouseLocation"] = aSelectListItems;
        }

        public ActionResult ComboBoxBoxedWarehouseLocation(int? id_responsable, int? id_boxedWarehouse)
        {
            BuildComboBoxBoxedWarehouseLocation(id_responsable, id_boxedWarehouse);
            ViewBag.enabled = true;
            return PartialView("_ComboBoxBoxedWarehouseLocation");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetValueBoxedWarehouseLocation()
        {
            var masteredDto = GetMasteredDTO();
            var result = new
            {
                message = "",
                id_boxedWarehouseLocation = masteredDto.id_boxedWarehouseLocation
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void BuildComboBoxMasteredWarehouse(int? id_responsable = null)
        {
            id_responsable = db.Person.FirstOrDefault(fod => fod.identification_number == ActiveCompany.ruc)?.id;
            var masteredDTO = GetMasteredDTO();
            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            var model = new List<Warehouse>();
            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == id_responsable);
            var id_WarehouseFreezeAux = aProviderRawMaterial?.id_WarehouseFreeze;
            if (id_WarehouseFreezeAux != null)
            {
                model.Add(new Warehouse
                {
                    id = id_WarehouseFreezeAux.Value,
                    name = db.Warehouse.FirstOrDefault(fod => fod.id == id_WarehouseFreezeAux)?.name
                });
            }

            model.AddRange(db.Warehouse.Where(t => t.id_company == ActiveCompany.id && t.isActive && t.id != id_WarehouseFreezeAux &&
                                               t.WarehouseType.code.Equals("BCC01")).ToList());

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            SelectListItem aSelectListItem = null;
            if (masteredDTO.id_masteredWarehouse != null)
            {
                aSelectListItem = new SelectListItem
                {
                    Text = masteredDTO.masteredWarehouse,
                    Value = masteredDTO.id_masteredWarehouse.ToString(),
                    Selected = true
                };
            }

            aSelectListItems.AddRange(model.Where(g => g.id != masteredDTO.id_masteredWarehouse)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            if (masteredDTO.id_masteredWarehouse != null)
            {
                aSelectListItems.Insert(0, aSelectListItem);
            }
            else
            {
                var aId_WarehouseFreezeStr = aProviderRawMaterial?.id_WarehouseFreeze == null ? "" : aProviderRawMaterial.id_WarehouseFreeze.Value.ToString();
                var aASelectListItem = aSelectListItems.FirstOrDefault(fod => fod.Value == aId_WarehouseFreezeStr);
                if (aASelectListItem != null)
                {
                    aASelectListItem.Selected = true;
                    masteredDTO.id_masteredWarehouse = aProviderRawMaterial.id_WarehouseFreeze;
                    masteredDTO.masteredWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == aProviderRawMaterial.id_WarehouseFreeze)?.name;
                }
            }

            ViewData["MasteredWarehouse"] = aSelectListItems;
        }

        public ActionResult ComboBoxMasteredWarehouse(int? id_responsable)
        {
            BuildComboBoxMasteredWarehouse(id_responsable);
            ViewBag.enabled = true;
            return PartialView("_ComboBoxMasteredWarehouse");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetValueMasteredWarehouse()
        {
            var masteredDto = GetMasteredDTO();
            var result = new
            {
                message = "",
                masteredDto.id_masteredWarehouse
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void BuildComboBoxMasteredWarehouseLocation(int? id_responsable = null, int? id_masteredWarehouse = null)
        {
            id_responsable = db.Person.FirstOrDefault(fod => fod.identification_number == ActiveCompany.ruc)?.id;
            var masteredDTO = GetMasteredDTO();
            if (id_masteredWarehouse != masteredDTO.id_masteredWarehouse)
            {
                masteredDTO.masteredWarehouseLocation = "";
                masteredDTO.id_masteredWarehouseLocation = null;
            }
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            List<WarehouseLocation> aWarehouseLocations = new List<WarehouseLocation>();
            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == id_responsable);
            var id_WarehouseFreezeAux = aProviderRawMaterial?.id_WarehouseFreeze;
            int? id_warehouseLocationFreezeAux = null;
            if (id_WarehouseFreezeAux != null && id_WarehouseFreezeAux == id_masteredWarehouse)
            {
                id_warehouseLocationFreezeAux = aProviderRawMaterial?.id_warehouseLocationFreeze;
                if (id_warehouseLocationFreezeAux != null)
                {
                    var aWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationFreezeAux);
                    aWarehouseLocations.Add(new WarehouseLocation
                    {
                        id = id_warehouseLocationFreezeAux.Value,
                        isActive = aWarehouseLocation?.isActive ?? true,
                        id_company = aWarehouseLocation?.id_company ?? ActiveCompany.id,
                        name = aWarehouseLocation?.name
                    });
                }
            }
            SelectListItem aSelectListItem = null;
            if (masteredDTO.id_masteredWarehouseLocation != null)
            {
                aSelectListItem = new SelectListItem
                {
                    Text = masteredDTO.masteredWarehouseLocation,
                    Value = masteredDTO.id_masteredWarehouseLocation.ToString(),
                    Selected = true
                };
            }

            aSelectListItems.AddRange(aWarehouseLocations.Where(g => g.id != masteredDTO.id_masteredWarehouseLocation &&
                                                                      g.id_company == ActiveCompany.id && g.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            aSelectListItems.AddRange(db.WarehouseLocation.Where(t => t.id_warehouse == id_masteredWarehouse && t.id_company == ActiveCompany.id &&
                                                                      t.isActive && t.id != masteredDTO.id_masteredWarehouseLocation &&
                                                                      t.id != id_warehouseLocationFreezeAux).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            if (masteredDTO.id_masteredWarehouseLocation != null)
            {
                aSelectListItems.Insert(0, aSelectListItem);
            }
            else
            {
                var aId_WarehouseLocationFreezeStr = id_warehouseLocationFreezeAux == null ? "" : id_warehouseLocationFreezeAux.Value.ToString();
                var aASelectListItem = aSelectListItems.FirstOrDefault(fod => fod.Value == aId_WarehouseLocationFreezeStr);
                if (aASelectListItem != null)
                {
                    aASelectListItem.Selected = true;
                    masteredDTO.id_masteredWarehouseLocation = id_warehouseLocationFreezeAux;
                    masteredDTO.masteredWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationFreezeAux)?.name;
                }
            }
            ViewData["MasteredWarehouseLocation"] = aSelectListItems;
        }

        public ActionResult ComboBoxMasteredWarehouseLocation(int? id_responsable, int? id_masteredWarehouse)
        {
            BuildComboBoxMasteredWarehouseLocation(id_responsable, id_masteredWarehouse);
            ViewBag.enabled = true;
            return PartialView("_ComboBoxMasteredWarehouseLocation");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetValueMasteredWarehouseLocation()
        {
            var masteredDto = GetMasteredDTO();
            var result = new
            {
                message = "",
                masteredDto.id_masteredWarehouseLocation
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void BuildComboBoxWarehouseBoxes(int? id_responsable = null)
        {
            id_responsable = db.Person.FirstOrDefault(fod => fod.identification_number == ActiveCompany.ruc)?.id;
            var masteredDTO = GetMasteredDTO();
            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            var model = new List<Warehouse>();
            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == id_responsable);
            var id_WarehouseLooseCartonAux = aProviderRawMaterial?.id_WarehouseLooseCarton;
            if (id_WarehouseLooseCartonAux != null)
            {
                model.Add(new Warehouse
                {
                    id = id_WarehouseLooseCartonAux.Value,
                    name = db.Warehouse.FirstOrDefault(fod => fod.id == id_WarehouseLooseCartonAux)?.name
                });
            }

            model.AddRange(db.Warehouse.Where(t => t.id_company == ActiveCompany.id && t.isActive && t.id != id_WarehouseLooseCartonAux &&
                                               t.WarehouseType.code.Equals("BCC01")).ToList());

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            SelectListItem aSelectListItem = null;
            if (masteredDTO.id_warehouseBoxes != null)
            {
                aSelectListItem = new SelectListItem
                {
                    Text = masteredDTO.warehouseBoxes,
                    Value = masteredDTO.id_warehouseBoxes.ToString(),
                    Selected = true
                };
            }

            aSelectListItems.AddRange(model.Where(g => g.id != masteredDTO.id_warehouseBoxes)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            if (masteredDTO.id_warehouseBoxes != null)
            {
                aSelectListItems.Insert(0, aSelectListItem);
            }
            else
            {
                var aId_WarehouseLooseCartonStr = aProviderRawMaterial?.id_WarehouseLooseCarton == null ? "" : aProviderRawMaterial.id_WarehouseLooseCarton.Value.ToString();
                var aASelectListItem = aSelectListItems.FirstOrDefault(fod => fod.Value == aId_WarehouseLooseCartonStr);
                if (aASelectListItem != null)
                {
                    aASelectListItem.Selected = true;
                    masteredDTO.id_warehouseBoxes = aProviderRawMaterial.id_WarehouseLooseCarton;
                    masteredDTO.warehouseBoxes = db.Warehouse.FirstOrDefault(fod => fod.id == aProviderRawMaterial.id_WarehouseLooseCarton)?.name;
                }
            }

            ViewData["WarehouseBoxes"] = aSelectListItems;
        }

        public ActionResult ComboBoxWarehouseBoxes(int? id_responsable)
        {
            BuildComboBoxWarehouseBoxes(id_responsable);
            ViewBag.enabled = true;
            return PartialView("_ComboBoxWarehouseBoxes");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetValueWarehouseBoxes()
        {
            var masteredDto = GetMasteredDTO();
            var result = new
            {
                message = "",
                masteredDto.id_warehouseBoxes
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void BuildComboBoxWarehouseLocationBoxes(int? id_responsable = null, int? id_warehouseBoxes = null)
        {
            id_responsable = db.Person.FirstOrDefault(fod => fod.identification_number == ActiveCompany.ruc)?.id;
            var masteredDTO = GetMasteredDTO();
            if (id_warehouseBoxes != masteredDTO.id_warehouseBoxes)
            {
                masteredDTO.warehouseLocationBoxes = "";
                masteredDTO.id_warehouseLocationBoxes = null;
            }
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            List<WarehouseLocation> aWarehouseLocations = new List<WarehouseLocation>();
            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == id_responsable);
            var id_WarehouseLooseCartonAux = aProviderRawMaterial?.id_WarehouseLooseCarton;
            int? id_WarehouseLocationLooseCartonAux = null;
            if (id_WarehouseLooseCartonAux != null && id_WarehouseLooseCartonAux == id_warehouseBoxes)
            {
                id_WarehouseLocationLooseCartonAux = aProviderRawMaterial?.id_WarehouseLocationLooseCarton;
                if (id_WarehouseLocationLooseCartonAux != null)
                {
                    var aWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_WarehouseLocationLooseCartonAux);
                    aWarehouseLocations.Add(new WarehouseLocation
                    {
                        id = id_WarehouseLocationLooseCartonAux.Value,
                        isActive = aWarehouseLocation?.isActive ?? true,
                        id_company = aWarehouseLocation?.id_company ?? ActiveCompany.id,
                        name = aWarehouseLocation?.name
                    });
                }
            }
            SelectListItem aSelectListItem = null;
            if (masteredDTO.id_warehouseLocationBoxes != null)
            {
                aSelectListItem = new SelectListItem
                {
                    Text = masteredDTO.warehouseLocationBoxes,
                    Value = masteredDTO.id_warehouseLocationBoxes.ToString(),
                    Selected = true
                };
            }

            aSelectListItems.AddRange(aWarehouseLocations.Where(g => g.id != masteredDTO.id_warehouseLocationBoxes &&
                                                                      g.id_company == ActiveCompany.id && g.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            aSelectListItems.AddRange(db.WarehouseLocation.Where(t => t.id_warehouse == id_warehouseBoxes && t.id_company == ActiveCompany.id &&
                                                                      t.isActive && t.id != masteredDTO.id_warehouseLocationBoxes &&
                                                                      t.id != id_WarehouseLocationLooseCartonAux).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            if (masteredDTO.id_warehouseLocationBoxes != null)
            {
                aSelectListItems.Insert(0, aSelectListItem);
            }
            else
            {
                var aId_WarehouseLocationLooseCartonStr = id_WarehouseLocationLooseCartonAux == null ? "" : id_WarehouseLocationLooseCartonAux.Value.ToString();
                var aASelectListItem = aSelectListItems.FirstOrDefault(fod => fod.Value == aId_WarehouseLocationLooseCartonStr);
                if (aASelectListItem != null)
                {
                    aASelectListItem.Selected = true;
                    masteredDTO.id_warehouseLocationBoxes = id_WarehouseLocationLooseCartonAux;
                    masteredDTO.warehouseLocationBoxes = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_WarehouseLocationLooseCartonAux)?.name;
                }
            }

            ViewData["WarehouseLocationBoxes"] = aSelectListItems;
        }

        public ActionResult ComboBoxWarehouseLocationBoxes(int? id_responsable, int? id_warehouseBoxes)
        {
            BuildComboBoxWarehouseLocationBoxes(id_responsable, id_warehouseBoxes);
            ViewBag.enabled = true;
            return PartialView("_ComboBoxWarehouseLocationBoxes");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetValueWarehouseLocationBoxes()
        {
            var masteredDto = GetMasteredDTO();
            var result = new
            {
                message = "",
                masteredDto.id_warehouseLocationBoxes
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion Combobox

        [HttpPost]
        public JsonResult Save(string jsonMastered)
        {
            var result = new ApiResult();
            bool isValidate = false;
            MasteredDTO masteredDTO = null;
            DocumentType documentType = null;
            var newObject = false;
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            Mastered mastered = null;
            try
            {
                masteredDTO = GetMasteredDTO();

                JToken token = JsonConvert.DeserializeObject<JToken>(jsonMastered);
                
                var id = token.Value<int>("id");

                documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoMastered));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                mastered = db.Mastered.FirstOrDefault(d => d.id == id);
                if (mastered == null)
                {
                    newObject = true;

                    var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                        ? ActiveUser.EmissionPoint.First().id
                        : 0;
                    if (id_emissionPoint == 0)
                        throw new ProdHandlerException("Su usuario no tiene asociado ningún punto de emisión.");

                    mastered = new Mastered
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
                        }
                    };

                }

                masteredDTO.boxedWarehouse = token.Value<string>("boxedWarehouse");
                masteredDTO.id_boxedWarehouse = token.Value<int>("id_boxedWarehouse");
                mastered.id_boxedWarehouse = token.Value<int>("id_boxedWarehouse");
                masteredDTO.boxedWarehouseLocation = token.Value<string>("boxedWarehouseLocation");
                masteredDTO.id_boxedWarehouseLocation = token.Value<int>("id_boxedWarehouseLocation");
                mastered.id_boxedWarehouseLocation = token.Value<int>("id_boxedWarehouseLocation");

                masteredDTO.masteredWarehouse = token.Value<string>("masteredWarehouse");
                masteredDTO.id_masteredWarehouse = token.Value<int>("id_masteredWarehouse");
                mastered.id_masteredWarehouse = token.Value<int>("id_masteredWarehouse");
                masteredDTO.masteredWarehouseLocation = token.Value<string>("masteredWarehouseLocation");
                masteredDTO.id_masteredWarehouseLocation = token.Value<int>("id_masteredWarehouseLocation");
                mastered.id_masteredWarehouseLocation = token.Value<int>("id_masteredWarehouseLocation");

                masteredDTO.warehouseBoxes = token.Value<string>("warehouseBoxes");
                masteredDTO.id_warehouseBoxes = token.Value<int>("id_warehouseBoxes");
                mastered.id_warehouseBoxes = token.Value<int>("id_warehouseBoxes");
                masteredDTO.warehouseLocationBoxes = token.Value<string>("warehouseLocationBoxes");
                masteredDTO.id_warehouseLocationBoxes = token.Value<int>("id_warehouseLocationBoxes");
                mastered.id_warehouseLocationBoxes = token.Value<int>("id_warehouseLocationBoxes");

                #region Validación Permiso

                var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                if (entityObjectPermissions != null)
                {
                    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                    if (entityPermissions != null)
                    {
                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                        if (entityValuePermissions == null)
                        {
                            throw new ProdHandlerException("No tiene Permiso para editar y guardar el Masterizado para la Bodega de Encartonado: " + masteredDTO.boxedWarehouse);
                        }
                        entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_masteredWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                        if (entityValuePermissions == null)
                        {
                            throw new ProdHandlerException("No tiene Permiso para editar y guardar el Masterizado para la Bodega de Cámara de Congelación para Masters: " + masteredDTO.masteredWarehouse);
                        }
                        entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_warehouseBoxes && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                        if (entityValuePermissions == null)
                        {
                            throw new ProdHandlerException("No tiene Permiso para editar y guardar el Masterizado para la Bodega de Cámara de Congelación para Cajas Sueltas: " + masteredDTO.warehouseBoxes);
                        }
                    }
                }

                #endregion Validación Permiso

                mastered.Document.emissionDate = token.Value<DateTime>("dateTimeEmision");
                mastered.Document.description = token.Value<string>("description");
                mastered.Document.id_documentState = documentState.id;
                mastered.Document.id_userUpdate = ActiveUser.id;
                mastered.Document.dateUpdate = DateTime.Now;
                mastered.id_responsable = token.Value<int>("id_responsable");
                mastered.dateTimeStartMastered = token.Value<DateTime>("dateTimeStartMastered");
                mastered.dateTimeEndMastered = token.Value<DateTime>("dateTimeEndMastered");
                mastered.id_turn = token.Value<int>("id_turn");
                masteredDTO.id_turn = mastered.id_turn;

                mastered.id_company = ActiveCompany.id;
                mastered.amountMP = masteredDTO.amountMP;
                mastered.amountPT = masteredDTO.amountPT;
                mastered.amountBoxes = masteredDTO.amountBoxes;
                mastered.lbsMP = masteredDTO.lbsMP;
                mastered.lbsPT = masteredDTO.lbsPT;
                mastered.lbsBoxes = masteredDTO.lbsBoxes;
                mastered.kgMP = masteredDTO.kgMP;
                mastered.kgPT = masteredDTO.kgPT;
                mastered.kgBoxes = masteredDTO.kgBoxes;

                #region Validaciones Generales
                if (masteredDTO.MasteredDetails.Count() <= 0)
                {
                    throw new ProdHandlerException("No se puede guardar un Masterizado Sin detalle.");
                }

                var id_stakeHolder = db.Person.FirstOrDefault(fod => fod.identification_number == ActiveCompany.ruc)?.id;

                var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == id_stakeHolder);
                var id_costCenterExitBoxed = aProviderRawMaterial?.id_CostCenterCarton;
                if (id_costCenterExitBoxed == null)
                {
                    throw new ProdHandlerException("No Existe, Centro de Costo en la configuración del Stakeholder con Rol Planta, en Bodega de Encartonado. Verifique el caso e intente de nuevo.");
                }
                var id_subCostCenterExitBoxed = aProviderRawMaterial?.id_SubCostCenterCarton;
                if (id_subCostCenterExitBoxed == null)
                {
                    throw new ProdHandlerException("No Existe, SubCentro de Costo en la configuración del Stakeholder con Rol Planta, en Bodega de Encartonado. Verifique el caso e intente de nuevo.");
                }
                var id_costCenterEntryMastered = aProviderRawMaterial?.id_CostCenterFreeze;
                if (id_costCenterEntryMastered == null)
                {
                    throw new ProdHandlerException("No Existe, Centro de Costo en la configuración del Stakeholder con Rol Planta, en Bodega de Cámara de Congelación para Masters. Verifique el caso e intente de nuevo.");
                }
                var id_subCostCenterEntryMastered = aProviderRawMaterial?.id_SubCostCenterFreeze;
                if (id_subCostCenterEntryMastered == null)
                {
                    throw new ProdHandlerException("No Existe, SubCentro de Costo en la configuración del Stakeholder con Rol Planta, en Bodega de Cámara de Congelación para Masters. Verifique el caso e intente de nuevo.");
                }
                var id_costCenterEntryBoxes = aProviderRawMaterial?.id_CostCenterLooseCarton;
                if (id_costCenterEntryBoxes == null)
                {
                    throw new ProdHandlerException("No Existe, Centro de Costo en la configuración del Stakeholder con Rol Planta, en Bodega de Cámara de Congelación para Cajas Sueltas. Verifique el caso e intente de nuevo.");
                }
                var id_subCostCenterEntryBoxes = aProviderRawMaterial?.id_SubCostCenterLooseCarton;
                if (id_subCostCenterEntryBoxes == null)
                {
                    throw new ProdHandlerException("No Existe, SubCentro de Costo en la configuración del Stakeholder con Rol Planta, en Bodega de Cámara de Congelación para Cajas Sueltas. Verifique el caso e intente de nuevo.");
                }

                #endregion Validaciones Generales

                #region Contruccion Detalles
                foreach (var detail in masteredDTO.MasteredDetails)
                {
                    var aMasteredDetail = new MasteredDetail
                    {
                        id_mastered = mastered.id,
                        id_sales = detail.id_sales,
                        id_productMP = detail.id_productMP,
                        id_lotMP = detail.id_lotMP.Value,
                        quantityMP = detail.quantityMP,
                        id_boxedWarehouse = mastered.id_boxedWarehouse,
                        id_boxedWarehouseLocation = mastered.id_boxedWarehouseLocation,
                        id_costCenterExitBoxed = id_costCenterExitBoxed.Value,
                        id_subCostCenterExitBoxed = id_subCostCenterExitBoxed.Value,
                        id_metricUnitBoxed = detail.id_metricUnitBoxed,
                        id_productPT = detail.id_productPT,
                        id_customer = detail.id_customer,
                        quantityPT = detail.quantityPT,
                        id_masteredWarehouse = mastered.id_masteredWarehouse,
                        id_masteredWarehouseLocation = detail.id_masteredWarehouseLocation.Value,
                        id_costCenterEntryMastered = id_costCenterEntryMastered.Value,
                        id_subCostCenterEntryMastered = id_subCostCenterEntryMastered.Value,
                        id_metricUnitMastered = detail.id_metricUnitMastered,
                        id_lotBoxes = detail.id_lotBoxes.Value,
                        quantityBoxes = detail.quantityBoxes,
                        id_warehouseBoxes = mastered.id_warehouseBoxes,
                        id_warehouseLocationBoxes = detail.id_warehouseLocationBoxes.Value,
                        id_costCenterEntryBoxes = id_costCenterEntryBoxes.Value,
                        id_subCostCenterEntryBoxes = id_subCostCenterEntryBoxes.Value,
                        lotMarked = detail.lotMarked,
                    };
                    mastered.MasteredDetail.Add(aMasteredDetail);
                }
                #endregion


                isValidate = true;
            }
            catch (ProdHandlerException e)
            {
                result.Code = 1000;
                result.Message = e.Message;
            }
            catch (Exception e) 
            {
                result.Code = 1001;
                result.Message = GenericError.ErrorGeneral;
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "Mastered", "Produccion");
            }

            if (isValidate)
            {
                
                using (var trans = db.Database.BeginTransaction())
                {

                    try
                    {
                        if (newObject)
                        {
                            documentType.currentNumber++;
                            //db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;
                        }

                        var lastDetails = db.MasteredDetail.Where(d => d.id_mastered == mastered.id);
                        foreach (var detail in lastDetails)
                            {
                                db.MasteredDetail.Remove(detail);
                                db.MasteredDetail.Attach(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                        if (newObject)
                        {
                            db.Mastered.Add(mastered);
                            db.Entry(mastered).State = EntityState.Added;
                        }
                        else
                        {
                            //db.Mastered.Attach(mastered);
                            db.Entry(mastered).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        trans.Commit();

                        result.Data = mastered.id.ToString();
                    }
                    catch (ProdHandlerException e)
                    {
                        result.Code = 1000;
                        result.Message = e.Message;
                        trans.Rollback();
                    }
                    catch (Exception e)
                    {
                        //result.Code = e.HResult;
                        result.Code = 1001;
                        result.Message = GenericError.ErrorGeneral;
                        trans.Rollback();
                        MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "Mastered", "Produccion");
                        
                    }
                    
                }
                 
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Approve(int id)
        {
            var result = new ApiResult();

            try
            {
                //result.Data = ApproveMastered(id).name;
                var resultTransac = ApproveMastered(id);
                result.Data = resultTransac.Item1?.name;
                result.Code = (resultTransac.Item2) ? 0 : ServiceTransCtl.CODE_FOR_QUEUE;
            }
            catch (ProdHandlerException e)
            {
                result.Code = 1000;
                result.Message = e.Message;
            }
            catch (Exception e)
            {
                result.Code = e.HResult;
                result.Message = GenericError.ErrorGeneralMastered;
                FullLog(e);
            }

            return Json(result, JsonRequestBehavior.AllowGet);


            //using (var db = new DBContext())
            //{
            //    using (var trans = db.Database.BeginTransaction())
            //    {
            //        
            //
            //        try
            //        {
            //            result.Data = ApproveMastered(id).name;
            //        }
            //        catch (Exception e)
            //        {
            //            result.Code = e.HResult;
            //            result.Message = e.Message;
            //            trans.Rollback();
            //        }
            //
            //        return Json(result, JsonRequestBehavior.AllowGet);
            //    }
            //}
        }

        //private ApiResult ApproveMastered(int id_mastered)
        //{
        //    bool isValid = false;
        //    Mastered mastered = null;
        //    ApiResult result = new ApiResult();
        //    try
        //    {
        //        isValid  =true;
        //    }
        //    catch (ProdHandlerException e)
        //    {
        //        
        //    }
        //    catch (Exception e)
        //    { 
        //    
        //    }
        //
        //    if (isValid)
        //    {
        //        
        //        using (var trans = db.Database.BeginTransaction())
        //        {
        //
        //            try
        //            {
        //                string utizaAutorizacion = db.Setting.FirstOrDefault(fod => fod.code == "APRMAST")?.value ?? "NO";
        //                mastered = db.Mastered.FirstOrDefault(p => p.id == id_mastered);
        //                var masteredDTO = ConvertToDto(mastered);
        //                if (mastered != null)
        //                {
        //                    #region Validación Permiso
        //
        //                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
        //
        //                    if (entityObjectPermissions != null)
        //                    {
        //                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
        //                        if (entityPermissions != null)
        //                        {
        //                            var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
        //                            if (entityValuePermissions == null)
        //                            {
        //                                throw new Exception("No tiene Permiso para aprobar el Masterizado para la Bodega de Encartonado: " + masteredDTO.boxedWarehouse);
        //                            }
        //                            entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_masteredWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
        //                            if (entityValuePermissions == null)
        //                            {
        //                                throw new Exception("No tiene Permiso para aprobar el Masterizado para la Bodega de Cámara de Congelación para Masters: " + masteredDTO.masteredWarehouse);
        //                            }
        //                            entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_warehouseBoxes && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
        //                            if (entityValuePermissions == null)
        //                            {
        //                                throw new Exception("No tiene Permiso para aprobar el Masterizado para la Bodega de Cámara de Congelación para Cajas Sueltas: " + masteredDTO.warehouseBoxes);
        //                            }
        //                        }
        //                    }
        //
        //                    #endregion Validación Permiso
        //                    if (utizaAutorizacion == "NO")
        //                    {
        //                        var _inventoryDetailPeriod = db.InventoryPeriodDetail.Where(a => a.AdvanceParametersDetail.valueCode == "A" && a.InventoryPeriod.isActive).ToList();
        //                        var _emissionDateMastered = mastered.Document.emissionDate;
        //                        var añoEmission = _emissionDateMastered.Year;
        //                        var mesEmission = _emissionDateMastered.Month;
        //
        //                        if (_inventoryDetailPeriod != null)
        //                        {
        //                            var _inventoryPeriodActivoBEncartonado = _inventoryDetailPeriod.Any(e =>
        //                                e.InventoryPeriod.year == añoEmission && e.dateInit.Month == mesEmission && e.InventoryPeriod.id_warehouse == mastered.id_boxedWarehouse);
        //                            var _inventoryPeriodActivoBCongelacion = _inventoryDetailPeriod.Any(e =>
        //                                e.InventoryPeriod.year == añoEmission && e.dateInit.Month == mesEmission && e.InventoryPeriod.id_warehouse == mastered.id_masteredWarehouse);
        //                            var _inventoryPeriodActivoBCamara = _inventoryDetailPeriod.Any(e =>
        //                                e.InventoryPeriod.year == añoEmission && e.dateInit.Month == mesEmission && e.InventoryPeriod.id_warehouse == mastered.id_warehouseBoxes);
        //                            if (!_inventoryPeriodActivoBEncartonado)
        //                            {
        //                                throw new Exception("No existe periodo de inventario abierto para la Bodega de Encartonado: " + masteredDTO.boxedWarehouse);
        //                            }
        //                            if (!_inventoryPeriodActivoBCongelacion)
        //                            {
        //                                throw new Exception("No existe periodo de inventario abierto para la Bodega de Cámara de Congelación para Masters: " + masteredDTO.masteredWarehouse);
        //                            }
        //                            if (!_inventoryPeriodActivoBCamara)
        //                            {
        //                                throw new Exception("No existe periodo de inventario abierto para la Bodega de Cámara de Congelación para Cajas Sueltas: " + masteredDTO.warehouseBoxes);
        //                            }
        //                        }
        //                        ServiceInventoryMove.UpdateInventaryMoveMasteredExitBoxed(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, masteredDTO, db, false);
        //                    }
        //
        //                    MasteredDetail aMasteredDetail = mastered.MasteredDetail.FirstOrDefault(fod => fod.SalesOrder != null && fod.SalesOrder.Document.DocumentState.code.Equals("04"));
        //                    if (aMasteredDetail != null)
        //                    {
        //                        throw new Exception("No se puede aprobar el Masterizado porque tiene la Orden de Producción: " + aMasteredDetail.SalesOrder.Document.number + " asociada a un detalle");
        //                    }
        //                    ServiceSalesOrder.UpdateQuantityDeliverySalesOrderDetail(db, masteredDTO, false);
        //
        //                    var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
        //                    if (aprovedState == null)
        //                        return null;
        //
        //                    mastered.Document.id_documentState = aprovedState.id;
        //                    mastered.Document.authorizationDate = DateTime.Now;
        //
        //                    db.Mastered.Attach(mastered);
        //                    db.Entry(mastered).State = EntityState.Modified;
        //                    db.SaveChanges();
        //
        //                    trans.Commit();
        //                }
        //                else
        //                {
        //                    throw new Exception("No se encontro el objeto seleccionado");
        //                }
        //            }
        //            catch (ProdHandlerException e)
        //            {
        //
        //            }
        //            catch (Exception e)
        //            {
        //
        //            }
        //            finally
        //            {
        //                trans.Rollback();
        //            }
        //            
        //
        //        }
        //
        //    }
        //
        //    result.Data = mastered.Document.DocumentState.name;
        //    return result;
        //    
        //}
        public Tuple<DocumentState, bool> ApproveMastered(  int id_mastered,
                                                            bool isInternalTrans = false,
                                                            ActiveUserDto sessionInfoTrans = null,
                                                            string tempKeyTrans = null,
                                                            string tempValueTrans = null,
                                                            string tempTypeTrans = null)
        {
            DocumentState documentState = null;
            bool isExecute = false;
            Guid? identificadorTran = null;

            try
            {
                using (var db = new DBContext())
                {
                    //isExecute = true;
                    var mastered = db.Mastered.FirstOrDefault(r => r.id == id_mastered);
                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
                    string documentNumber = db.Document.FirstOrDefault(r => r.id == id_mastered)?.number;

                    if (!isInternalTrans)
                    {
                        int documentTypeId = db.DocumentType.FirstOrDefault(r => r.code == "145").id;
                        int numDetails = mastered.MasteredDetail.Count();
                        string sessionInfo = ServiceTransCtl.GetSessionInfoSerialize(   ActiveUserId,
                                                                                        ActiveUser.username,
                                                                                        ActiveCompanyId,
                                                                                        ActiveEmissionPoint.id,
                                                                                        entityObjectPermissions);

                        string dataExecution = JsonConvert.SerializeObject(new object[] { id_mastered });
                        string dataExecutionTypes = JsonConvert.SerializeObject(new object[] { "System.Int32" });

                        var result = ServiceTransCtl.TransCtlExternal(
                                                            id_mastered,
                                                            documentNumber,
                                                            documentTypeId: documentTypeId,
                                                            stage: "0",
                                                            numdetails: numDetails,
                                                            sessionInfoSerialize: sessionInfo,
                                                            dataExecution: dataExecution,
                                                            dataExecutionTypes: dataExecutionTypes,
                                                            temDataKey: null,
                                                            temDataValueSerialize: null,
                                                            temDataTypes: null);

                        isExecute = result.Item1;
                        identificadorTran = result.Item2;

                    }
                    else
                    {
                        SetInfoForTrans(sessionInfoTrans, tempKeyTrans, tempValueTrans, tempTypeTrans);

                        entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
                    }

                    if (isInternalTrans || isExecute)
                    {
                        using (var trans = db.Database.BeginTransaction())
                        {
                            string utizaAutorizacion = db.Setting.FirstOrDefault(fod => fod.code == "APRMAST")?.value ?? "NO";
                            //var mastered = db.Mastered.FirstOrDefault(p => p.id == id_mastered);
                            var masteredDTO = ConvertToDto(mastered);
                            if (mastered != null)
                            {
                                #region Validación Permiso

                                //var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                                if (entityObjectPermissions != null)
                                {
                                    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                    if (entityPermissions != null)
                                    {
                                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                        if (entityValuePermissions == null)
                                        {
                                            throw new ProdHandlerException("No tiene Permiso para aprobar el Masterizado para la Bodega de Encartonado: " + masteredDTO.boxedWarehouse);
                                        }
                                        entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_masteredWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                        if (entityValuePermissions == null)
                                        {
                                            throw new ProdHandlerException("No tiene Permiso para aprobar el Masterizado para la Bodega de Cámara de Congelación para Masters: " + masteredDTO.masteredWarehouse);
                                        }
                                        entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_warehouseBoxes && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                        if (entityValuePermissions == null)
                                        {
                                            throw new ProdHandlerException("No tiene Permiso para aprobar el Masterizado para la Bodega de Cámara de Congelación para Cajas Sueltas: " + masteredDTO.warehouseBoxes);
                                        }
                                    }
                                }

                                #endregion Validación Permiso
                                if (utizaAutorizacion == "NO")
                                {
                                    var _inventoryDetailPeriod = db.InventoryPeriodDetail.Where(a => a.AdvanceParametersDetail.valueCode == "A" && a.InventoryPeriod.isActive).ToList();
                                    var _emissionDateMastered = mastered.Document.emissionDate;
                                    var añoEmission = _emissionDateMastered.Year;
                                    var mesEmission = _emissionDateMastered.Month;

                                    if (_inventoryDetailPeriod != null)
                                    {
                                        var _inventoryPeriodActivoBEncartonado = _inventoryDetailPeriod.Any(e =>
                                            e.InventoryPeriod.year == añoEmission && e.dateInit.Month == mesEmission && e.InventoryPeriod.id_warehouse == mastered.id_boxedWarehouse);
                                        var _inventoryPeriodActivoBCongelacion = _inventoryDetailPeriod.Any(e =>
                                            e.InventoryPeriod.year == añoEmission && e.dateInit.Month == mesEmission && e.InventoryPeriod.id_warehouse == mastered.id_masteredWarehouse);
                                        var _inventoryPeriodActivoBCamara = _inventoryDetailPeriod.Any(e =>
                                            e.InventoryPeriod.year == añoEmission && e.dateInit.Month == mesEmission && e.InventoryPeriod.id_warehouse == mastered.id_warehouseBoxes);
                                        if (!_inventoryPeriodActivoBEncartonado)
                                        {
                                            throw new ProdHandlerException("No existe periodo de inventario abierto para la Bodega de Encartonado: " + masteredDTO.boxedWarehouse);
                                        }
                                        if (!_inventoryPeriodActivoBCongelacion)
                                        {
                                            throw new ProdHandlerException("No existe periodo de inventario abierto para la Bodega de Cámara de Congelación para Masters: " + masteredDTO.masteredWarehouse);
                                        }
                                        if (!_inventoryPeriodActivoBCamara)
                                        {
                                            throw new ProdHandlerException("No existe periodo de inventario abierto para la Bodega de Cámara de Congelación para Cajas Sueltas: " + masteredDTO.warehouseBoxes);
                                        }
                                    }
                                    ServiceInventoryMove.UpdateInventaryMoveMasteredExitBoxed(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, masteredDTO, db, false);
                                }

                                MasteredDetail aMasteredDetail = mastered.MasteredDetail.FirstOrDefault(fod => fod.SalesOrder != null && fod.SalesOrder.Document.DocumentState.code.Equals("04"));
                                if (aMasteredDetail != null)
                                {
                                    throw new ProdHandlerException("No se puede aprobar el Masterizado porque tiene la Orden de Producción: " + aMasteredDetail.SalesOrder.Document.number + " asociada a un detalle");
                                }
                                ServiceSalesOrder.UpdateQuantityDeliverySalesOrderDetail(db, masteredDTO, false);

                                var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                                if (aprovedState == null)
                                    return null;

                                mastered.Document.id_documentState = aprovedState.id;
                                mastered.Document.authorizationDate = DateTime.Now;

                                db.Mastered.Attach(mastered);
                                db.Entry(mastered).State = EntityState.Modified;
                                db.SaveChanges();

                                trans.Commit();
                            }
                            else
                            {
                                throw new ProdHandlerException("No se encontro el objeto seleccionado");
                            }

                            documentState = mastered.Document.DocumentState;
                        }
                    }

                    
                }

            }
            catch (Exception e)
            {
                FullLog(e);
                throw;
            }
            finally
            {                
                if (isExecute)
                {
                    ServiceTransCtl.Finalize(identificadorTran.Value);
                }

            }
            return new Tuple<DocumentState, bool>(documentState, isExecute);

            
        }

        [HttpPost]
        public JsonResult Autorice(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Data = AutoriceMastered(id).name;
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

        private DocumentState AutoriceMastered(int id_mastered)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var mastered = db.Mastered.FirstOrDefault(p => p.id == id_mastered);
                    var masteredDTO = ConvertToDto(mastered);
                    if (mastered != null)
                    {
                        #region Validación Permiso

                        int id_user = (int)ViewData["id_user"];
                        int id_menu = (int)ViewData["id_menu"];

                        User user = DataProviderUser.UserById(id_user);
                        UserMenu userMenu = user.UserMenu.FirstOrDefault(m => m.Menu.id == id_menu);
                        if (userMenu != null)
                        {
                            Permission permission = userMenu.Permission.FirstOrDefault(p => p.name == "Autorizar");
                            if (permission == null)
                            {
                                throw new Exception("No tiene Permiso para autorizar el Masterizado");
                            }
                        }
                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para autorizar el Masterizado para la Bodega de Encartonado: " + masteredDTO.boxedWarehouse);
                                }
                                entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_masteredWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para autorizar el Masterizado para la Bodega de Cámara de Congelación para Masters: " + masteredDTO.masteredWarehouse);
                                }
                                entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_warehouseBoxes && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para autorizar el Masterizado para la Bodega de Cámara de Congelación para Cajas Sueltas: " + masteredDTO.warehouseBoxes);
                                }
                            }
                        }

                        #endregion Validación Permiso
                        var _inventoryDetailPeriod = db.InventoryPeriodDetail.Where(a => a.AdvanceParametersDetail.valueCode == "A" && a.InventoryPeriod.isActive).ToList();
                        var _emissionDateMastered = mastered.Document.emissionDate;
                        var añoEmission = _emissionDateMastered.Year;
                        var mesEmission = _emissionDateMastered.Month;

                        if (_inventoryDetailPeriod != null)
                        {
                            var _inventoryPeriodActivoBEncartonado = _inventoryDetailPeriod.Any(e =>
                                e.InventoryPeriod.year == añoEmission && e.dateInit.Month == mesEmission && e.InventoryPeriod.id_warehouse == mastered.id_boxedWarehouse);
                            var _inventoryPeriodActivoBCongelacion = _inventoryDetailPeriod.Any(e =>
                                e.InventoryPeriod.year == añoEmission && e.dateInit.Month == mesEmission && e.InventoryPeriod.id_warehouse == mastered.id_masteredWarehouse);
                            var _inventoryPeriodActivoBCamara = _inventoryDetailPeriod.Any(e =>
                                e.InventoryPeriod.year == añoEmission && e.dateInit.Month == mesEmission && e.InventoryPeriod.id_warehouse == mastered.id_warehouseBoxes);
                            if (!_inventoryPeriodActivoBEncartonado)
                            {
                                throw new Exception("No existe periodo de inventario abierto para la Bodega de Encartonado: " + masteredDTO.boxedWarehouse);
                            }
                            if (!_inventoryPeriodActivoBCongelacion)
                            {
                                throw new Exception("No existe periodo de inventario abierto para la Bodega de Cámara de Congelación para Masters: " + masteredDTO.masteredWarehouse);
                            }
                            if (!_inventoryPeriodActivoBCamara)
                            {
                                throw new Exception("No existe periodo de inventario abierto para la Bodega de Cámara de Congelación para Cajas Sueltas: " + masteredDTO.warehouseBoxes);
                            }
                        }

                        ServiceInventoryMove.UpdateInventaryMoveMasteredExitBoxed(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, masteredDTO, db, false);

                        MasteredDetail aMasteredDetail = mastered.MasteredDetail.FirstOrDefault(fod => fod.SalesOrder != null && fod.SalesOrder.Document.DocumentState.code.Equals("04"));
                        if (aMasteredDetail != null)
                        {
                            throw new Exception("No se puede autorizar el Masterizado porque tiene la Orden de Producción: " + aMasteredDetail.SalesOrder.Document.number + " asociada a un detalle");
                        }
                        ServiceSalesOrder.UpdateQuantityDeliverySalesOrderDetail(db, masteredDTO, false);

                        var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("06"));
                        if (aprovedState == null)
                            return null;

                        mastered.Document.id_documentState = aprovedState.id;
                        mastered.Document.authorizationDate = DateTime.Now;

                        db.Mastered.Attach(mastered);
                        db.Entry(mastered).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return mastered.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult Reverse(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var document = db.Document.FirstOrDefault(e => e.id == id);
                        if(document.DocumentState.code == "16")
                        {
                            result.Data = this.ReversarConciliacion(id).name;
                        }
                        else
                        {
                            result.Data = ReverseMastered(id).name;
                        }
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

        private DocumentState ReversarConciliacion(int id_mastered)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        string utizaAutorizacion = db.Setting.FirstOrDefault(fod => fod.code == "APRMAST")?.value ?? "NO";
                        var mastered = db.Mastered.FirstOrDefault(p => p.id == id_mastered);

                        //Verificar si existe lotes relacionados al proceso de cierre
                        var masteredDetails = mastered.MasteredDetail.ToList();
                        foreach (var item in masteredDetails)
                        {
                            var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == item.id_lotMP && a.isClosed);
                            if (productionLot != null)
                            {
                                var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.id_lot == item.id_lotMP && a.Document.DocumentState.code != "05"
                                                        && a.isActive);
                                if (productionLotClose != null && productionLot.receptionDate.Date <= productionLotClose.Document.emissionDate.Date 
                                    && productionLot.ProductionLotState.code == "11")
                                {
                                    throw new Exception("El lote " + productionLot.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                                }
                            }
                        }

                        var codigoEstadoReverso = (utizaAutorizacion == "SI") ? "06" : "03";
                        var reverseStateMastered = db.DocumentState.FirstOrDefault(d => d.code.Equals(codigoEstadoReverso));
                        mastered.Document.id_documentState = reverseStateMastered.id;
                        mastered.Document.id_userUpdate = this.ActiveUserId;
                        mastered.Document.dateUpdate = DateTime.Now;

                        db.Mastered.Attach(mastered);
                        db.Entry(mastered).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();

                        return reverseStateMastered;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error al reversar. {ex.GetBaseException().Message}");
                    }
                }
            }
        }

        private DocumentState ReverseMastered(int id_mastered)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    string utizaAutorizacion = db.Setting.FirstOrDefault(fod => fod.code == "APRMAST")?.value ?? "NO";
                    var mastered = db.Mastered.FirstOrDefault(p => p.id == id_mastered);
                    var masteredDTO = ConvertToDto(mastered);
                    if (mastered != null)
                    {
                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para reversar el Masterizado para la Bodega de Encartonado: " + masteredDTO.boxedWarehouse);
                                }
                                entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_masteredWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para reversar el Masterizado para la Bodega de Cámara de Congelación para Masters: " + masteredDTO.masteredWarehouse);
                                }
                                entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_warehouseBoxes && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para reversar el Masterizado para la Bodega de Cámara de Congelación para Cajas Sueltas: " + masteredDTO.warehouseBoxes);
                                }
                            }
                        }

                        //Verificar si existe lotes relacionados al proceso de cierre
                        var masteredDetails = mastered.MasteredDetail.ToList();
                        foreach (var item in masteredDetails)
                        {
                            var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == item.id_lotMP && a.isClosed);
                            if (productionLot != null)
                            {
                                var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.id_lot == item.id_lotMP && a.Document.DocumentState.code != "05"
                                                        && a.isActive);
                                if (productionLotClose != null && productionLot.receptionDate.Date <= productionLotClose.Document.emissionDate.Date 
                                    && productionLot.ProductionLotState.code == "11")
                                {
                                    throw new Exception("El lote " + productionLot.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                                }
                            }
                        }

                        #endregion Validación Permiso

                        MasteredDetail aMasteredDetail = mastered.MasteredDetail.FirstOrDefault(fod => fod.SalesOrder != null && fod.SalesOrder.Document.DocumentState.code.Equals("04"));
                        if (aMasteredDetail != null)
                        {
                            throw new Exception("No se puede reversar el Masterizado porque tiene la Orden de Producción: " + aMasteredDetail.SalesOrder.Document.number + " asociada a un detalle en estado CERRADA");
                        }
                        ServiceSalesOrder.UpdateQuantityDeliverySalesOrderDetail(db, masteredDTO, true);

                        var codigosEstados = new[] { "03", "16" };
                        var id_inventaryMoveProcessAutomaticExit = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == masteredDTO.id &&
                                                                codigosEstados.Contains(fod.Document.DocumentState.code) &&
                                                                fod.Document.DocumentType.code.Equals("146"))?.id_document;
                        var id_inventaryMoveProcessAutomaticExitMaster = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == masteredDTO.id &&
                                                                codigosEstados.Contains(fod.Document.DocumentState.code) &&
                                                                fod.Document.DocumentType.code.Equals("147"))?.id_document;
                        var id_inventaryMoveProcessAutomaticExitCajas = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == masteredDTO.id &&
                                                                codigosEstados.Contains(fod.Document.DocumentState.code) &&
                                                                fod.Document.DocumentType.code.Equals("148"))?.id_document;

                        var inventaryMoveProcessAutomaticExit = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticExit);
                        var inventaryMoveProcessAutomaticExitDocument = db.Document.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticExit);
                        var inventaryMoveProcessAutomaticExitMaster = db.Document.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticExitMaster);
                        var inventaryMoveProcessAutomaticExitCajas = db.Document.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticExitCajas);

                        var fechaMasterizado = mastered.Document.emissionDate.Date;
                        var detallesSalidaMasters = inventaryMoveProcessAutomaticExitMaster?.InventoryMove?
                            .InventoryMoveDetail.ToArray() ?? new InventoryMoveDetail[] { };
                        foreach (var detalleSalidaMaster in detallesSalidaMasters)
                        {
                            var fechaValidacionStock = fechaMasterizado;
                            var cantidadMovimiento = -(detalleSalidaMaster.entryAmount - detalleSalidaMaster.exitAmount);
                            while (fechaValidacionStock <= DateTime.Today)
                            {
                                //var saldos = new ServiceInventoryMove()
                                //    .GetSaldosProductoLote(true, detalleSalidaMaster.id_warehouse,
                                //        detalleSalidaMaster.id_warehouseLocation, detalleSalidaMaster.id_item,
                                //        detalleSalidaMaster.id_lot, null, fechaValidacionStock, null);

                                var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
                                {
                                    requiresLot = true,
                                    id_Warehouse = detalleSalidaMaster.id_warehouse,
                                    id_WarehouseLocation = detalleSalidaMaster.id_warehouseLocation,
                                    id_Item = detalleSalidaMaster.id_item,
                                    id_ProductionLot = detalleSalidaMaster.id_lot,
                                    lotMarket = null,
                                    id_productionCart = null,
                                    cut_Date = fechaValidacionStock,
                                    id_company = this.ActiveCompanyId,
                                    consolidado = true,
                                    groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

                                }, modelSaldoProductlote: true);
                                var saldos = resultItemsLotSaldo.Item2;

                                var saldoCorte = saldos.Sum(e => e.saldo) + cantidadMovimiento;
                                if (saldoCorte < 0m)
                                {
                                    var item = detalleSalidaMaster.Item;
                                    throw new Exception($"El producto: {item.masterCode} - {item.name} quedaría en negativo a la fecha: {fechaValidacionStock.ToDateFormat()}.");
                                }

                                fechaValidacionStock = fechaValidacionStock.AddDays(1);
                            }
                        }

                        var detallesSalidaCajas = inventaryMoveProcessAutomaticExitCajas?.InventoryMove?
                            .InventoryMoveDetail.ToArray() ?? new InventoryMoveDetail[] { };
                        foreach (var detalleSalidaCajas in detallesSalidaCajas)
                        {
                            var fechaValidacionStock = fechaMasterizado;
                            var cantidadMovimiento = -(detalleSalidaCajas.entryAmount - detalleSalidaCajas.exitAmount);
                            while (fechaValidacionStock <= DateTime.Today)
                            {
                                //var saldos = new ServiceInventoryMove()
                                //    .GetSaldosProductoLote(true, detalleSalidaCajas.id_warehouse,
                                //        detalleSalidaCajas.id_warehouseLocation, detalleSalidaCajas.id_item,
                                //        detalleSalidaCajas.id_lot, null, fechaValidacionStock, null);

                                var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
                                {
                                    requiresLot = true,
                                    id_Warehouse = detalleSalidaCajas.id_warehouse,
                                    id_WarehouseLocation = detalleSalidaCajas.id_warehouseLocation,
                                    id_Item = detalleSalidaCajas.id_item,
                                    id_ProductionLot = detalleSalidaCajas.id_lot,
                                    lotMarket = null,
                                    id_productionCart = null,
                                    cut_Date = fechaValidacionStock,
                                    id_company = this.ActiveCompanyId,
                                    consolidado = true,
                                    groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

                                }, modelSaldoProductlote: true);
                                var saldos = resultItemsLotSaldo.Item2;

                                var saldoCorte = saldos.Sum(e => e.saldo) + cantidadMovimiento;
                                if (saldoCorte < 0m)
                                {
                                    var item = detalleSalidaCajas.Item;
                                    throw new Exception($"El producto: {item.masterCode} - {item.name} quedaría en negativo a la fecha: {fechaValidacionStock.ToDateFormat()}.");
                                }

                                fechaValidacionStock = fechaValidacionStock.AddDays(1);
                            }
                        }

                        if (inventaryMoveProcessAutomaticExit != null)
                            ServiceInventoryMove.UpdateInventaryMoveMasteredExitBoxed(false, ActiveUser, ActiveCompany, ActiveEmissionPoint, masteredDTO, db, true, inventaryMoveProcessAutomaticExit);

                        var codigoEstadoReverso = "01";
                        if (utizaAutorizacion == "SI" && mastered.Document.DocumentState.code == "06")
                        {
                            codigoEstadoReverso = "03";
                        }
                        var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        var reverseStateMastered = db.DocumentState.FirstOrDefault(d => d.code.Equals(codigoEstadoReverso));
                        if (reverseStateMastered == null)
                            return mastered.Document.DocumentState;

                        if (inventaryMoveProcessAutomaticExitDocument != null)
                        {
                            inventaryMoveProcessAutomaticExitDocument.id_documentState = reverseState.id;
                            inventaryMoveProcessAutomaticExitDocument.DocumentState = reverseState;

                            db.Document.Attach(inventaryMoveProcessAutomaticExitDocument);
                            db.Entry(inventaryMoveProcessAutomaticExitDocument).State = EntityState.Modified;
                        }

                        if (inventaryMoveProcessAutomaticExitMaster != null)
                        {
                            if (inventaryMoveProcessAutomaticExitMaster != null)
                            {
                                inventaryMoveProcessAutomaticExitMaster.id_documentState = reverseState.id;
                                inventaryMoveProcessAutomaticExitMaster.DocumentState = reverseState;

                                db.Document.Attach(inventaryMoveProcessAutomaticExitMaster);
                                db.Entry(inventaryMoveProcessAutomaticExitMaster).State = EntityState.Modified;
                            }
                        }
                        if (inventaryMoveProcessAutomaticExitCajas != null)
                        {
                            if (inventaryMoveProcessAutomaticExitCajas != null)
                            {
                                inventaryMoveProcessAutomaticExitCajas.id_documentState = reverseState.id;
                                inventaryMoveProcessAutomaticExitCajas.DocumentState = reverseState;

                                db.Document.Attach(inventaryMoveProcessAutomaticExitCajas);
                                db.Entry(inventaryMoveProcessAutomaticExitCajas).State = EntityState.Modified;
                            }
                        }

                        mastered.Document.id_documentState = reverseStateMastered.id;
                        mastered.Document.authorizationDate = DateTime.Now;

                        db.Mastered.Attach(mastered);
                        db.Entry(mastered).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return mastered.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult Annul(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Data = AnnulMastered(id).name;
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

        private DocumentState AnnulMastered(int id_mastered)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var mastered = db.Mastered.FirstOrDefault(p => p.id == id_mastered);
                    var masteredDTO = ConvertToDto(mastered);
                    if (mastered != null)
                    {
                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para anular el Masterizado para la Bodega de Encartonado: " + masteredDTO.boxedWarehouse);
                                }
                                entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_masteredWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para anular el Masterizado para la Bodega de Cámara de Congelación para Masters: " + masteredDTO.masteredWarehouse);
                                }
                                entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == mastered.id_warehouseBoxes && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para anular el Masterizado para la Bodega de Cámara de Congelación para Cajas Sueltas: " + masteredDTO.warehouseBoxes);
                                }
                            }
                        }

                        #endregion Validación Permiso
                        var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        if (annulState == null)
                            return mastered.Document.DocumentState;

                        mastered.Document.id_documentState = annulState.id;
                        mastered.Document.authorizationDate = DateTime.Now;

                        db.Mastered.Attach(mastered);
                        db.Entry(mastered).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return mastered.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult Conciliate(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var estadoConciliado = db.DocumentState.FirstOrDefault(e => e.code == "16");
                        if (estadoConciliado == null)
                            throw new Exception("No se ha encontrado el estado: [16] - CONCILIADO");

                        var mastered = db.Mastered.FirstOrDefault(p => p.id == id);
                        mastered.Document.id_documentState = estadoConciliado.id;
                        mastered.Document.id_userUpdate = this.ActiveUserId;
                        mastered.Document.dateUpdate = DateTime.Now;


                        var id_inventaryMoveProcessAutomaticExit = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == mastered.id &&
                                                               fod.Document.DocumentState.code.Equals("03") &&
                                                               fod.Document.DocumentType.code.Equals("146"))?.id_document;
                        var id_inventaryMoveProcessAutomaticExitMaster = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == mastered.id &&
                                                                fod.Document.DocumentState.code.Equals("03") &&
                                                                fod.Document.DocumentType.code.Equals("147"))?.id_document;
                        var id_inventaryMoveProcessAutomaticExitCajas = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == mastered.id &&
                                                                fod.Document.DocumentState.code.Equals("03") &&
                                                                fod.Document.DocumentType.code.Equals("148"))?.id_document;

                        var inventaryMoveProcessAutomaticExitDocument = db.Document.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticExit);
                        var inventaryMoveProcessAutomaticExitMaster = db.Document.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticExitMaster);
                        var inventaryMoveProcessAutomaticExitCajas = db.Document.FirstOrDefault(fod => fod.id == id_inventaryMoveProcessAutomaticExitCajas);

                        if (inventaryMoveProcessAutomaticExitDocument != null)
                        {
                            inventaryMoveProcessAutomaticExitDocument.id_documentState = estadoConciliado.id;
                            inventaryMoveProcessAutomaticExitDocument.DocumentState = estadoConciliado;

                            db.Document.Attach(inventaryMoveProcessAutomaticExitDocument);
                            db.Entry(inventaryMoveProcessAutomaticExitDocument).State = EntityState.Modified;
                        }

                        if (inventaryMoveProcessAutomaticExitMaster != null)
                        {
                            if (inventaryMoveProcessAutomaticExitMaster != null)
                            {
                                inventaryMoveProcessAutomaticExitMaster.id_documentState = estadoConciliado.id;
                                inventaryMoveProcessAutomaticExitMaster.DocumentState = estadoConciliado;

                                db.Document.Attach(inventaryMoveProcessAutomaticExitMaster);
                                db.Entry(inventaryMoveProcessAutomaticExitMaster).State = EntityState.Modified;
                            }
                        }
                        if (inventaryMoveProcessAutomaticExitCajas != null)
                        {
                            if (inventaryMoveProcessAutomaticExitCajas != null)
                            {
                                inventaryMoveProcessAutomaticExitCajas.id_documentState = estadoConciliado.id;
                                inventaryMoveProcessAutomaticExitCajas.DocumentState = estadoConciliado;

                                db.Document.Attach(inventaryMoveProcessAutomaticExitCajas);
                                db.Entry(inventaryMoveProcessAutomaticExitCajas).State = EntityState.Modified;
                            }
                        }

                        db.Mastered.Attach(mastered);
                        db.Entry(mastered).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();

                        result.Data = estadoConciliado.name;
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

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id)
        {
            var index = GetMasteredResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

            var result = new
            {
                maximunPages = GetMasteredResultConsultDTO().Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            var element = GetMasteredResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
            var mastered = db.Mastered.FirstOrDefault(d => d.id == element.id);
            if (mastered == null)
                return PartialView("Edit", new MasteredDTO());

            BuildViewDataEdit();
            var model = ConvertToDto(mastered);
            SetMasteredDTO(model);
            BuilViewBag(false);

            return PartialView("Edit", model);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult TurnChanged(int? id_turn)
        {
            var masteredDto = GetMasteredDTO();
            var result = new
            {
                message = "",
                turn = "",
                timeInitTurn = "",
                timeEndTurn = ""
            };
            var aTurn = db.Turn.FirstOrDefault(fod => fod.id == id_turn);

            masteredDto.timeInitTurn = aTurn != null ? (aTurn.timeInit.Hours + ":" + aTurn.timeInit.Minutes) : null;
            masteredDto.timeEndTurn = aTurn != null ? (aTurn.timeEnd.Hours + ":" + aTurn.timeEnd.Minutes) : null;
            masteredDto.turn = aTurn?.name;
            masteredDto.id_turn = id_turn;

            if (aTurn != null)
            {
                result = new
                {
                    message = "",
                    masteredDto.turn,
                    masteredDto.timeInitTurn,
                    masteredDto.timeEndTurn
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MasteredReportIndex(MasteredConsultDTO report, string codeReport)
        {
            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@estado";
            _param.Valor = report.id_state;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@nlote";
            _param.Valor = report.number;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@responsable";
            _param.Valor = report.id_responsable;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@turno";
            _param.Valor = report.id_turn;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@bodega1";
            _param.Valor = report.id_boxedWarehouse;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@ubicacion1";
            _param.Valor = report.id_boxedWarehouseLocation;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@producto1";
            _param.Valor = report.boxedItems;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@loteenc";
            _param.Valor = report.boxedNumberLot;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@bodega2";
            _param.Valor = report.id_masteredWarehouse;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@ubicacion2";
            _param.Valor = report.id_masteredWarehouseLocation;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@producto2";
            _param.Valor = report.masteredItems;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@loteterm";
            _param.Valor = report.masteredNumberLot;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@fi";
            _param.Valor = report.initDate;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@ff";
            _param.Valor = report.endtDate;
            paramLst.Add(_param);

            #endregion Armo Parametros

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "MASTEN";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MasteredColaReportIndex(MasteredConsultDTO report, string codeReport)
        {
            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@estado";
            _param.Valor = report.id_state;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@nlote";
            _param.Valor = report.number;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@responsable";
            _param.Valor = report.id_responsable;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@turno";
            _param.Valor = report.id_turn;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@bodega1";
            _param.Valor = report.id_boxedWarehouse;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@ubicacion1";
            _param.Valor = report.id_boxedWarehouseLocation;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@producto1";
            _param.Valor = report.boxedItems;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@loteenc";
            _param.Valor = report.boxedNumberLot;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@bodega2";
            _param.Valor = report.id_masteredWarehouse;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@ubicacion2";
            _param.Valor = report.id_masteredWarehouseLocation;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@producto2";
            _param.Valor = report.masteredItems;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@loteterm";
            _param.Valor = report.masteredNumberLot;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@fi";
            _param.Valor = report.initDate;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@ff";
            _param.Valor = report.endtDate;
            paramLst.Add(_param);

            #endregion Armo Parametros

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "MASTCO";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrintReportEgreso(int id, string codeReport)
        {
            List<ParamCR> paramLst = new List<ParamCR>();

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ParamCR _param = null;

            var idEgreso = db.DocumentSource.FirstOrDefault(e => e.id_documentOrigin.Equals(id))?.id_document ?? 0;

            _param = new ParamCR
            {
                Nombre = "@id",
                Valor = codeReport.Equals("MASING") || codeReport.Equals("MASEGR") || codeReport.Equals("MASGEN") || codeReport.Equals("MASPER") || codeReport.Equals("MAREEN") || codeReport.Equals("MARETP") ? id : idEgreso,
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

        public JsonResult PrintReport(int id, string codeReport)
        {
            List<ParamCR> paramLst = new List<ParamCR>();

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ParamCR _param = null;

            var idEgreso = db.DocumentSource.FirstOrDefault(e => e.id_documentOrigin.Equals(id))?.id_document ?? 0;

            _param = new ParamCR
            {
                Nombre = "@id",
                Valor = codeReport.Equals("IDMET") ? idEgreso : id,
            };
            paramLst.Add(_param);

            if (codeReport.Equals("RPTIT") || codeReport.Equals("IDMET"))
            {
                _param = new ParamCR
                {
                    Nombre = "@id_user",
                    Valor = this.ActiveUserId,
                };
                paramLst.Add(_param);
            }

            if (codeReport.Equals("RPTIT"))
            {
                _param = new ParamCR
                {
                    Nombre = "@naturaleza",
                    Valor = "I",
                };
                paramLst.Add(_param);
            }

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

        public JsonResult PrintMatrixMasterizadoExcel(DateTime? startReceptionDate,
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
                codeReport = "MASTEXC",
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

        #region Validacion Unique Item Lote Cliente
        private bool validaNotIsUniqueDetail(MasteredDetailDTO[] details, MasteredDetailDTO item)
        {
            var countExists = details
                                .Where(r => r.id_productMP == item.id_productMP
                                            && r.id_lotMP == item.id_lotMP
                                            && r.id_customer == item.id_customer
                                            && r.id != item.id)
                                .Count();

            return (countExists > 0);

        }
        #endregion
    }
}