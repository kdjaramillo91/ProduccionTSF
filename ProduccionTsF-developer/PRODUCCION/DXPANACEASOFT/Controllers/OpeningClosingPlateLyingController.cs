using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using Utilitarios.ProdException;
using DXPANACEASOFT.Models.Dto;
using static DXPANACEASOFT.Services.ServiceTransCtl;
using Dapper;
using System.Threading.Tasks;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class OpeningClosingPlateLyingController : DefaultController
    {
        private const string m_TipoDocumentoOpeningClosingPlateLying = "37";

        private OpeningClosingPlateLyingDTO GetOpeningClosingPlateLyingDTO()
        {
            if (!(Session["OpeningClosingPlateLyingDTO"] is OpeningClosingPlateLyingDTO openingClosingPlateLying))
                openingClosingPlateLying = new OpeningClosingPlateLyingDTO();
            return openingClosingPlateLying;
        }

        private List<OpeningClosingPlateLyingResultConsultDTO> GetOpeningClosingPlateLyingResultConsultDTO()
        {
            if (!(Session["OpeningClosingPlateLyingResultConsultDTO"] is List<OpeningClosingPlateLyingResultConsultDTO> openingClosingPlateLyingResultConsult))
                openingClosingPlateLyingResultConsult = new List<OpeningClosingPlateLyingResultConsultDTO>();
            return openingClosingPlateLyingResultConsult;
        }

        private void SetOpeningClosingPlateLyingDTO(OpeningClosingPlateLyingDTO openingClosingPlateLyingDTO)
        {
            Session["OpeningClosingPlateLyingDTO"] = openingClosingPlateLyingDTO;
        }

        private void SetOpeningClosingPlateLyingResultConsultDTO(List<OpeningClosingPlateLyingResultConsultDTO> openingClosingPlateLyingResultConsult)
        {
            Session["OpeningClosingPlateLyingResultConsultDTO"] = openingClosingPlateLyingResultConsult;
        }

        // GET: OpeningClosingPlateLying
        public ActionResult Index()
        {
            BuildViewDataIndex();
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult(OpeningClosingPlateLyingConsultDTO consult)
        {
            var result = GetListsConsultDto(consult);
            SetOpeningClosingPlateLyingResultConsultDTO(result);
            return PartialView("ConsultResult", result);
        }

        [HttpPost]
        public ActionResult GridViewOpeningClosingPlateLying()
        {
            return PartialView("_GridViewIndex", GetOpeningClosingPlateLyingResultConsultDTO());
        }

        private List<OpeningClosingPlateLyingResultConsultDTO> GetListsConsultDto(OpeningClosingPlateLyingConsultDTO consulta)
        {
            using (var db = new DBContext())
            {
                var consultaAux = Session["consulta"] as OpeningClosingPlateLyingConsultDTO;
                if (consultaAux != null && consulta.initDate == null)
                {
                    consulta = consultaAux;
                }

                var consultResult = db.OpeningClosingPlateLying.ToList();

                #region Aplicación de filtros
                if (!string.IsNullOrEmpty(consulta.initDate))
                {
                    var fechaInicioEmision = DateTime.Parse(consulta.initDate).Date;
                    consultResult = consultResult
                        .Where(e => fechaInicioEmision <= e.Document.emissionDate.Date)
                        .ToList();
                }
                
                if (!string.IsNullOrEmpty(consulta.endtDate))
                {
                    var fechaFinEmision = DateTime.Parse(consulta.endtDate).Date;
                    consultResult = consultResult
                        .Where(e => fechaFinEmision >= e.Document.emissionDate.Date)
                        .ToList();
                }

                if(consulta.id_state.HasValue && consulta.id_state != 0)
                {
                    consultResult = consultResult.Where(e => e.Document.id_documentState == consulta.id_state).ToList();
                }

                if(!String.IsNullOrEmpty(consulta.number))
                {
                    consultResult = consultResult.Where(e => e.Document.number.Contains(consulta.number)).ToList();
                }

                if(!String.IsNullOrEmpty(consulta.reference))
                {
                    consultResult = consultResult.Where(e => e.Document.description.Contains(consulta.reference)).ToList();
                }

                if (consulta.id_responsable.HasValue && consulta.id_responsable != 0)
                {
                    consultResult = consultResult.Where(e => e.id_responsable == consulta.id_responsable).ToList();
                }
                
                if (consulta.id_freezerWarehouse.HasValue && consulta.id_freezerWarehouse != 0)
                {
                    consultResult = consultResult.Where(e => e.id_freezerWarehouse == consulta.id_freezerWarehouse).ToList();
                }

                if ((consulta.id_freezerWarehouseLocation.HasValue && consulta.id_freezerWarehouseLocation != 0))
                {
                    consultResult = consultResult.Where(w => (w.OpeningClosingPlateLyingDetail.FirstOrDefault(fod => fod.id_warehouseLocation == consulta.id_freezerWarehouseLocation) != null)).ToList();
                }
                if ((consulta.id_boxedWarehouse.HasValue && consulta.id_boxedWarehouse != 0))
                {
                    consultResult = consultResult.Where(w => (w.OpeningClosingPlateLyingDetail.FirstOrDefault(fod => fod.id_boxedWarehouse == consulta.id_boxedWarehouse) != null)).ToList();
                }
                if ((consulta.id_boxedWarehouseLocation.HasValue && consulta.id_boxedWarehouseLocation != 0))
                {
                    consultResult = consultResult.Where(w => (w.OpeningClosingPlateLyingDetail.FirstOrDefault(fod => fod.id_boxedWarehouseLocation == consulta.id_boxedWarehouseLocation) != null)).ToList();
                }
                if (!String.IsNullOrEmpty(consulta.numberLot))
                {
                    consultResult = consultResult.Where(w => (w.OpeningClosingPlateLyingDetail.FirstOrDefault(fod => fod.Lot.ProductionLot != null ? fod.Lot.ProductionLot.internalNumber.Contains(consulta.numberLot)
                                                                                                                                                  : fod.Lot.internalNumber.Contains(consulta.numberLot)) != null)).ToList();
                }
                if (!String.IsNullOrEmpty(consulta.secTransLot))
                {
                    consultResult = consultResult.Where(w => (w.OpeningClosingPlateLyingDetail.FirstOrDefault(fod => fod.Lot.ProductionLot != null ? fod.Lot.ProductionLot.number.Contains(consulta.secTransLot)
                                                                                                                                                   : fod.Lot.number.Contains(consulta.secTransLot)) != null)).ToList();
                }

                if (consulta.items != null && consulta.items.Any(e => e != 0))
                {
                    var tempModel = new List<OpeningClosingPlateLying>();
                    foreach (var order in consultResult)
                    {
                        var details = order.OpeningClosingPlateLyingDetail.Where(d => consulta.items.Contains(d.id_item));
                        if (details.Any())
                        {
                            tempModel.Add(order);
                        }
                    }

                    consultResult = tempModel;
                }
                #endregion

                var query = consultResult.Select(t => new OpeningClosingPlateLyingResultConsultDTO
                {
                    id = t.id,
                    number = t.Document.number,
                    emissionDate = t.Document.emissionDate,
                    responsable = db.Person.FirstOrDefault(fod => fod.id == t.id_responsable).fullname_businessName,
                    freezerMachineForProd = t.MachineForProd?.name,
                    freezerWarehouse = t.Warehouse.name,
                    turn = t.Turn?.name,
                    warehouse = "",//db.Warehouse.FirstOrDefault(fod => fod.id == t.OpeningClosingPlateLyingDetail.FirstOrDefault().id_boxedWarehouse).name,
                    tunnelTransferPlate = t.tunnelTransferPlate,
                    selectedQuantity = t.selectedQuantity,
                    state = t.Document.DocumentState.name,

                    canEdit = t.Document.DocumentState.code.Equals("01"),
                    canAproved = t.Document.DocumentState.code.Equals("01"),
                    canAnnul = t.Document.DocumentState.code.Equals("01"),
                    canReverse = t.Document.DocumentState.code.Equals("03")
                }).OrderBy(ob => ob.number).ToList();

                if (db.OpeningClosingPlateLyingDetail.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        var aWarehouseName = db.OpeningClosingPlateLyingDetail.FirstOrDefault(fod2 => fod2.id_openingClosingPlateLying == item.id).Warehouse1.name;
                        item.warehouse = aWarehouseName;
                    }
                } 

                Session["consulta"] = consulta;

                return query;
            }
        }

        private OpeningClosingPlateLyingDTO Create()
        {
            using (var db = new DBContext())
            {
                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoOpeningClosingPlateLying));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var hoy = DateTime.Now;
                Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
                int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
                var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

                var openingClosingPlateLyingDTO = new OpeningClosingPlateLyingDTO
                {
                    id_documentType = documentType?.id ?? 0,
                    documentType = documentType?.name ?? "",
                    number = "",//GetDocumentNumber(documentType?.id ?? 0),
                    idSate = documentState?.id ?? 0,
                    state = documentState?.name ?? "",
                    dateTimeEmision = hoy,
                    dateTimeEmisionStr = hoy.ToString("dd-MM-yyyy"),
                    description = "",
                    reference = "",
                    id_responsable = ActiveUser.id_employee,
                    responsable = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee)?.Person.fullname_businessName,
                    department = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee)?.Department.name,
                    freezerWarehouse = "",
                    id_freezerWarehouse = null,
                    ids_freezerWarehouseLocation = "",
                    ids_productionCart = "",
                    ids_item = "",
                    ids_lot = "",
                    id_company = ActiveCompany.id,
                    selectedQuantity = 0.00M,
                    selectedQuantityStr = "0,00",
                    dateHoy = hoy.ToString("dd-MM-yyyy"),
                    dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),
                    dateTimeStartLying = hoy,
                    dateTimeEndLying = hoy,
                    temperature = null,
                    freezerMachineForProd = "",
                    id_freezerMachineForProd = null,
                    freezerMachineForProdDestination = "",
                    id_freezerMachineForProdDestination = null,
                    turn = "",
                    id_turn = null,
                    timeInitTurn = null,
                    timeEndTurn = null,
                    tunnelTransferPlate = false,

                    OpeningClosingPlateLyingDetails = new List<OpeningClosingPlateLyingDetailDTO>()
                };

                return openingClosingPlateLyingDTO;
            }
        }

        private OpeningClosingPlateLyingDTO ConvertToDto(OpeningClosingPlateLying openingClosingPlateLying)
        {
            var hoy = DateTime.Now;
            Setting settingDFEAFA = db.Setting.FirstOrDefault(t => t.code == "DFEAFA");
            int valueSettingDFEAFA = int.Parse(settingDFEAFA?.value ?? "0");
            var hoyMin = hoy.AddDays(-valueSettingDFEAFA);

            var openingClosingPlateLyingDto = new OpeningClosingPlateLyingDTO
            {
                id = openingClosingPlateLying.id,
                id_documentType = openingClosingPlateLying.Document.id_documentType,
                documentType = openingClosingPlateLying.Document.DocumentType.name,
                number = openingClosingPlateLying.Document.number,
                idSate = openingClosingPlateLying.Document.id_documentState,
                state = openingClosingPlateLying.Document.DocumentState.name,
                dateTimeEmisionStr = openingClosingPlateLying.Document.emissionDate.ToString("dd-MM-yyyy"),
                dateTimeEmision = openingClosingPlateLying.Document.emissionDate,
                description = openingClosingPlateLying.Document.description,
                reference = openingClosingPlateLying.Document.reference,
                responsable = db.Employee.FirstOrDefault(fod => fod.id == openingClosingPlateLying.id_responsable)?.Person.fullname_businessName,
                id_responsable = openingClosingPlateLying.id_responsable,
                department = openingClosingPlateLying.Employee.Department.name,
                freezerWarehouse = openingClosingPlateLying.Warehouse.name,
                id_freezerWarehouse = openingClosingPlateLying.id_freezerWarehouse,
                ids_freezerWarehouseLocation = openingClosingPlateLying.ids_freezerWarehouseLocation,
                ids_productionCart = openingClosingPlateLying.ids_productionCart,
                ids_item = openingClosingPlateLying.ids_item,
                ids_lot = openingClosingPlateLying.ids_lot,
                id_company = openingClosingPlateLying.id_company,
                selectedQuantity = openingClosingPlateLying.selectedQuantity,
                selectedQuantityStr = openingClosingPlateLying.selectedQuantity.ToString("#.00"),
                dateHoy = hoy.ToString("dd-MM-yyyy"),
                dateHoyMin = hoyMin.ToString("dd-MM-yyyy"),
                dateTimeStartLying = openingClosingPlateLying.dateTimeStartLying,
                dateTimeEndLying = openingClosingPlateLying.dateTimeEndLying,
                temperature = openingClosingPlateLying.temperature,
                freezerMachineForProd = openingClosingPlateLying.MachineForProd?.name,
                id_freezerMachineForProd = openingClosingPlateLying.id_freezerMachineForProd,
                freezerMachineForProdDestination = openingClosingPlateLying.MachineForProd1?.name,
                id_freezerMachineForProdDestination = openingClosingPlateLying.id_freezerMachineForProdDestination,
                turn = openingClosingPlateLying.Turn?.name,
                id_turn = openingClosingPlateLying.id_turn,
                timeInitTurn = openingClosingPlateLying.Turn != null ? openingClosingPlateLying.Turn.timeInit.Hours + ":" + openingClosingPlateLying.Turn.timeInit.Minutes : null,
                timeEndTurn = openingClosingPlateLying.Turn != null ? openingClosingPlateLying.Turn.timeEnd.Hours + ":" + openingClosingPlateLying.Turn.timeEnd.Minutes : null,
                tunnelTransferPlate = openingClosingPlateLying.tunnelTransferPlate,

                OpeningClosingPlateLyingDetails = new List<OpeningClosingPlateLyingDetailDTO>()
            };

            if (openingClosingPlateLying.id_warehouseDestiny.HasValue)
            {
                var warehouse = db.Warehouse.FirstOrDefault(e => e.id == openingClosingPlateLying.id_warehouseDestiny);
                openingClosingPlateLyingDto.id_warehouseDestiny = warehouse.id;
                openingClosingPlateLyingDto.name_warehouseDestiny = warehouse.name;
            }
            

            if (openingClosingPlateLying.id_warehouseLocationDestiny.HasValue)
            {
                var warehouseLocation = db.WarehouseLocation.FirstOrDefault(e => e.id == openingClosingPlateLying.id_warehouseLocationDestiny);
                openingClosingPlateLyingDto.id_warehouseLocationDestiny = warehouseLocation.id;
                openingClosingPlateLyingDto.name_warehouseLocationDestiny = warehouseLocation.name;
            }


            var id_inventaryMoveTransferAutomaticExit = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == openingClosingPlateLyingDto.id &&
                                                                   fod.Document.DocumentState.code.Equals("03") &&
                                                                   fod.Document.DocumentType.code.Equals("142"))?.id_document;//142: Egreso Por Transferencia Automática Por Tumbada Placa
            var inventoryMoveTransferAutomaticExit = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveTransferAutomaticExit);

            var ids_inventaryMoveTransferAutomaticEntry = db.DocumentSource.Where(fod => fod.id_documentOrigin == id_inventaryMoveTransferAutomaticExit &&
                                                                   fod.Document.DocumentState.code.Equals("03") &&
                                                                   fod.Document.DocumentType.code.Equals("143"));//143: Ingreso Por Transferencia Automática Por Tumbada Placa

            foreach (var itemDetail in openingClosingPlateLying.OpeningClosingPlateLyingDetail)
            {
                var numberInventoryExitAux = "";
                var numberInventoryEntryAux = "";
                if (inventoryMoveTransferAutomaticExit != null)
                {
                    foreach (var item in ids_inventaryMoveTransferAutomaticEntry)
                    {
                        var inventaryMoveTransferAutomaticEntry = db.InventoryMove.FirstOrDefault(fod => fod.id == item.id_document);
                        if (inventaryMoveTransferAutomaticEntry.idWarehouse == itemDetail.id_boxedWarehouse)
                        {
                            numberInventoryExitAux = inventoryMoveTransferAutomaticExit.natureSequential;
                            numberInventoryEntryAux = inventaryMoveTransferAutomaticEntry.natureSequential;
                            break;
                        }
                    }
                }

                var carroPorCarroDetail = db.LiquidationCartOnCartDetail.Where(fod => fod.id_ItemLiquidation == itemDetail.id_item &&
                                                                                      fod.id_ProductionCart == itemDetail.id_productionCart &&
                                                                                      fod.LiquidationCartOnCart.id_ProductionLot == itemDetail.id_lot).FirstOrDefault(); 
                                                                               
                var cliente = carroPorCarroDetail != null ? db.Person.FirstOrDefault(fod => fod.id == carroPorCarroDetail.id_Client).fullname_businessName : "";

                openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.Add(new OpeningClosingPlateLyingDetailDTO
                {
                    id = itemDetail.id,
                    id_lot = itemDetail.id_lot,
                    noSecTransLote = itemDetail.Lot.ProductionLot != null ? itemDetail.Lot.ProductionLot.number : itemDetail.Lot.number,
                    noLote = itemDetail.Lot.ProductionLot != null ? itemDetail.Lot.ProductionLot.internalNumber : itemDetail.Lot.internalNumber,
                    id_item = itemDetail.id_item,
                    name_item = itemDetail.Item.name,
                    id_warehouse = itemDetail.id_warehouse,
                    warehouse = itemDetail.Warehouse.name,
                    id_warehouseLocation = itemDetail.id_warehouseLocation,
                    id_costCenterExit = itemDetail.id_costCenterExit,
                    id_subCostCenterExit = itemDetail.id_subCostCenterExit,
                    warehouseLocation = itemDetail.WarehouseLocation.name,
                    id_productionCart = itemDetail.id_productionCart,
                    productionCart = itemDetail.ProductionCart?.name,
                    amount = itemDetail.amount,
                    id_metricUnit = itemDetail.id_metricUnit,
                    metricUnit = itemDetail.MetricUnit.code,
                    id_boxedWarehouse = itemDetail.id_boxedWarehouse,
                    boxedWarehouse = itemDetail.Warehouse1?.name ?? "",
                    id_boxedWarehouseLocation = itemDetail.id_boxedWarehouseLocation,
                    boxedWarehouseLocation = itemDetail.WarehouseLocation1?.name ?? "",
                    id_costCenter = itemDetail.id_costCenter,
                    id_subCostCenter = itemDetail.id_subCostCenter,
                    numberInventoryExit = numberInventoryExitAux,
                    numberInventoryEntry = numberInventoryEntryAux,
                    cliente = cliente
                });
            }
            return openingClosingPlateLyingDto;
        }

        private void BuildViewDataIndex()
        {
            BuildComboBoxState();
            BuildComboBoxResponsableIndex();
            BuildComboBoxFreezerWarehouseIndex();
            BuildComboBoxFreezerWarehouseLocationIndex();
            BuildComboBoxBoxedWarehouseIndex();
            BuildComboBoxBoxedWarehouseLocationIndex();
            BuildTokenBoxItemsIndex();
        }

        private void BuildViewDataEdit()
        {
            BuildComboBoxResponsable();
            BuildComboBoxTurn();
            BuildComboBoxFreezerMachineForProd();
            var openingClosingPlateLying = GetOpeningClosingPlateLyingDTO();
            BuildComboBoxFreezerMachineForProdDestination(openingClosingPlateLying.id_freezerMachineForProd);
            BuildComboBoxFreezerWarehouse(openingClosingPlateLying.id_freezerMachineForProd);
            BuildTokenBoxFreezerWarehouseLocations(openingClosingPlateLying.id_freezerMachineForProd, openingClosingPlateLying.id_freezerWarehouse);
            BuildTokenBoxProductionCarts();
            BuildTokenBoxItems();

            string data = openingClosingPlateLying.ids_lot;
            string[] words = (data ?? "").Split(',');
            int[] ids_lotInt = null;
            if (words.Count() > 0)
            {
                ids_lotInt = new int[words.Count()];
                int count = 0;
                foreach (string word in words)
                {
                    if (word == "") continue; //ids_lotInt[count] = 0;
                    else ids_lotInt[count] = int.Parse(word);

                    count++;
                }
            }

            BuildTokenBoxLots(ids_lotInt, openingClosingPlateLying.id_freezerWarehouse);
        }

        [HttpPost]
        public ActionResult Edit(int id = 0, bool enabled = true)
        {
            var model = new OpeningClosingPlateLyingDTO();
            OpeningClosingPlateLying openingClosingPlateLying = db.OpeningClosingPlateLying.FirstOrDefault(d => d.id == id);
            if (openingClosingPlateLying == null)
            {
                model = Create();
            }
            else
            {
                model = ConvertToDto(openingClosingPlateLying);
            }
            SetOpeningClosingPlateLyingDTO(model);
            BuilViewBag(enabled);

            BuildViewDataEdit();
            return PartialView(model);
        }

        private void BuilViewBag(bool enabled)
        {
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            var idMenu = db.Menu.FirstOrDefault(fod => fod.TController.name == "OpeningClosingPlateLying").id;
            var tienePermisioConciliar = this.ActiveUser
                    .UserMenu.FirstOrDefault(e => e.id_menu == idMenu)?
                    .Permission?.FirstOrDefault(p => p.name == "Conciliar");

            ViewBag.enabled = enabled;
            ViewBag.canNew = openingClosingPlateLyingDTO.id != 0;
            ViewBag.canEdit = !enabled &&
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == openingClosingPlateLyingDTO.idSate)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == openingClosingPlateLyingDTO.idSate)
                                     ?.code.Equals("01") ?? false) && openingClosingPlateLyingDTO.id != 0;
            ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == openingClosingPlateLyingDTO.idSate)
                                     ?.code.Equals("03") ?? false) && !enabled;
            ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == openingClosingPlateLyingDTO.idSate)
                                      ?.code.Equals("01") ?? false) && openingClosingPlateLyingDTO.id != 0;
            ViewBag.canConciliate = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == openingClosingPlateLyingDTO.idSate)
                ?.code.Equals("03") ?? false) && !enabled && tienePermisioConciliar != null;

            var estado = db.DocumentState.FirstOrDefault(e => e.id == openingClosingPlateLyingDTO.idSate);

            var estadosReverso = new[] { "03", "16" };
            var puedeReversar = db.DocumentState
                .Any(e => estadosReverso.Contains(e.code)
                    && e.id == openingClosingPlateLyingDTO.idSate) && openingClosingPlateLyingDTO.id != 0;

            ViewBag.canReverse = estado.code == "16"
                ? puedeReversar && !enabled && tienePermisioConciliar != null
                : puedeReversar && !enabled;

            ViewBag.dateTimeEmision = openingClosingPlateLyingDTO.dateTimeEmision;

            ViewBag.showInventory = db.DocumentState.FirstOrDefault(fod => fod.id == openingClosingPlateLyingDTO.idSate && fod.code == "03") != null;
        }

        #region GridViewDetails

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewDetails(bool? enabled, bool? updateDetail, int? id_freezerMachineForProd,
            int? id_freezerWarehouse, int[] ids_freezerWarehouseLocation = null, int[] ids_productionCart = null,
            int[] ids_item = null, int? id_freezerMachineForProdDestination = null, bool tunnelTransferPlate = false,
            int[] ids_lot = null, int? id_warehouseDestiny = null, int? id_warehouseLocationDestiny = null)
        {
            var openingClosingPlateLyingDto = GetOpeningClosingPlateLyingDTO();
            if (updateDetail ?? false)
            {
                List<OpeningClosingPlateLyingDetailDTO> openingClosingPlateLyingDetails = new List<OpeningClosingPlateLyingDetailDTO>();
                if (id_freezerWarehouse != null)
                {
                    Parametros.ParametrosBusquedaOpeningClosingPlateLying parametrosBusquedaOpeningClosingPlateLying = new Parametros.ParametrosBusquedaOpeningClosingPlateLying();
                    parametrosBusquedaOpeningClosingPlateLying.id_warehouse = id_freezerWarehouse;
                    parametrosBusquedaOpeningClosingPlateLying.id_openingClosingPlateLyingDto = openingClosingPlateLyingDto.id;
                    parametrosBusquedaOpeningClosingPlateLying.id_warehouseType = db.WarehouseType.FirstOrDefault(t => t.code.Equals("BCO01"))?.id;//BCO01:Tipo Bodega: Bodega de Congelación
                    parametrosBusquedaOpeningClosingPlateLying.for_lot = 0;

                    var parametrosBusquedaOpeningClosingPlateLyingAux = new SqlParameter();
                    parametrosBusquedaOpeningClosingPlateLyingAux.ParameterName = "@ParametrosBusquedaOpeningClosingPlateLying";
                    parametrosBusquedaOpeningClosingPlateLyingAux.Direction = ParameterDirection.Input;
                    parametrosBusquedaOpeningClosingPlateLyingAux.SqlDbType = SqlDbType.NVarChar;
                    var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaOpeningClosingPlateLying);
                    parametrosBusquedaOpeningClosingPlateLyingAux.Value = jsonAux;
                    db.Database.CommandTimeout = 1200;
                    openingClosingPlateLyingDetails = db.Database.SqlQuery<OpeningClosingPlateLyingDetailDTO>("exec inv_Consultar_OpeningClosingPlateLying_StoredProcedure @ParametrosBusquedaOpeningClosingPlateLying ", parametrosBusquedaOpeningClosingPlateLyingAux).ToList();
                }

                var aFreezerMachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == id_freezerMachineForProd);
                if (ids_freezerWarehouseLocation != null && ids_freezerWarehouseLocation.Length > 0)
                {
                    openingClosingPlateLyingDetails = openingClosingPlateLyingDetails.Where(d => ids_freezerWarehouseLocation.Contains(d.id_warehouseLocation.Value)).ToList();
                }
                else
                {
                    List<int> aIdWarehouseLocations = new List<int>();
                    if (aFreezerMachineForProd?.id_materialWarehouse != null && aFreezerMachineForProd?.id_materialWarehouse == id_freezerWarehouse)
                    {
                        if (aFreezerMachineForProd?.id_materialWarehouseLocation != null)
                        {
                            aIdWarehouseLocations.Add(aFreezerMachineForProd.id_materialWarehouseLocation.Value);
                        }
                    }
                    if (aFreezerMachineForProd?.id_materialthirdWarehouse != null && aFreezerMachineForProd?.id_materialthirdWarehouse == id_freezerWarehouse)
                    {
                        if (aFreezerMachineForProd?.id_materialthirdWarehouseLocation != null && aFreezerMachineForProd?.id_materialthirdWarehouseLocation != aFreezerMachineForProd?.id_materialWarehouseLocation)
                        {
                            aIdWarehouseLocations.Add(aFreezerMachineForProd.id_materialthirdWarehouseLocation.Value);
                        }
                    }
                    openingClosingPlateLyingDetails = openingClosingPlateLyingDetails.Where(d => aIdWarehouseLocations.Contains(d.id_warehouseLocation.Value)).ToList();
                }
                if (ids_productionCart != null && ids_productionCart.Length > 0)
                {
                    openingClosingPlateLyingDetails = openingClosingPlateLyingDetails.Where(d => ids_productionCart.Contains(d.id_productionCart != null ? d.id_productionCart.Value : 0)).ToList();
                }
                if (ids_item != null && ids_item.Length > 0)
                {
                    openingClosingPlateLyingDetails = openingClosingPlateLyingDetails.Where(d => ids_item.Contains(d.id_item)).ToList();
                }
                if (ids_lot != null && ids_lot.Length > 0)
                {
                    openingClosingPlateLyingDetails = openingClosingPlateLyingDetails.Where(d => ids_lot.Contains(d.id_lot ?? 0)).ToList();
                }

                var id_inventaryMoveTransferAutomaticExit = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == openingClosingPlateLyingDto.id &&
                                                                       fod.Document.DocumentState.code.Equals("03") &&
                                                                       fod.Document.DocumentType.code.Equals("142"))?.id_document;//142: Egreso Por Transferencia Automática Por Tumbada Placa
                var inventoryMoveTransferAutomaticExit = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveTransferAutomaticExit);

                var ids_inventaryMoveTransferAutomaticEntry = db.DocumentSource.Where(fod => fod.id_documentOrigin == id_inventaryMoveTransferAutomaticExit &&
                                                                       fod.Document.DocumentState.code.Equals("03") &&
                                                                       fod.Document.DocumentType.code.Equals("143"));//143: Ingreso Por Transferencia Automática Por Tumbada Placa

                var aFreezerMachineForProdDestination = db.MachineForProd.FirstOrDefault(fod => fod.id == id_freezerMachineForProdDestination);
                var idsOpeningClosingPlateRemove = new List<int>();
                foreach (var item in openingClosingPlateLyingDetails)
                {
                    var aOpeningClosingPlateLyingDetails = openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.FirstOrDefault(fod => fod.id_lot == item.id_lot && fod.id_item == item.id_item && fod.id_productionCart == item.id_productionCart &&
                                                                                                                                            fod.id_warehouse == item.id_warehouse && fod.id_warehouseLocation == item.id_warehouseLocation);
                    if (aOpeningClosingPlateLyingDetails != null)
                    {
                        item.id = aOpeningClosingPlateLyingDetails.id;
                    }
                    else
                    {
                        var idAux1 = openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.Count() > 0 ? openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.Max(ppd => ppd.id) + 1 : 1;
                        var idAux2 = openingClosingPlateLyingDetails.Count() > 0 ? openingClosingPlateLyingDetails.Max(ppd => ppd.id) + 1 : 1;
                        item.id = (idAux1 > idAux2) ? idAux1 : idAux2;
                    }
                    var aProductionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == item.id_lot);
                    if (aProductionLot == null)// Si el lote es manual, no lo consideramos
                    {
                        idsOpeningClosingPlateRemove.Add(item.id);
                        continue;                    
                    }

                    var isCopackingLot = aProductionLot?.isCopackingLot ?? false;
                    if (isCopackingLot)
                    {
                        item.id_costCenterExit = aFreezerMachineForProd?.id_materialthirdCostCenter;
                        item.id_subCostCenterExit = aFreezerMachineForProd?.id_materialthirdSubCostCenter;
                        if (!tunnelTransferPlate)
                        {
                            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == aProductionLot.id_provider);
                            item.id_boxedWarehouse = aProviderRawMaterial?.id_WarehouseCarton;
                            item.boxedWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_boxedWarehouse)?.name;
                            item.id_boxedWarehouseLocation = aProviderRawMaterial?.id_warehouseLocationCarton;
                            item.boxedWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_boxedWarehouseLocation)?.name;
                            item.id_costCenter = aProviderRawMaterial?.id_CostCenterCarton;
                            item.id_subCostCenter = aProviderRawMaterial?.id_SubCostCenterCarton;
                        }
                        else
                        {
                            item.id_boxedWarehouse = aFreezerMachineForProdDestination?.id_materialthirdWarehouse;
                            item.boxedWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_boxedWarehouse)?.name;
                            item.id_boxedWarehouseLocation = aFreezerMachineForProdDestination?.id_materialthirdWarehouseLocation;
                            item.boxedWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_boxedWarehouseLocation)?.name;
                            item.id_costCenter = aFreezerMachineForProdDestination?.id_materialthirdCostCenter;
                            item.id_subCostCenter = aFreezerMachineForProdDestination?.id_materialthirdSubCostCenter;
                        }
                    }
                    else
                    {
                        item.id_costCenterExit = aFreezerMachineForProd?.id_materialCostCenter;
                        item.id_subCostCenterExit = aFreezerMachineForProd?.id_materialSubCostCenter;
                        if (!tunnelTransferPlate)
                        {
                            var aProviderRawMaterial = db.ProviderRawMaterial.FirstOrDefault(fod => fod.id_provider == aProductionLot.id_personProcessPlant);
                            var aCostCenter = db.CostCenter.FirstOrDefault(fod => fod.id == item.id_costCenterExit);
                            var aSubCostCenter = db.CostCenter.FirstOrDefault(fod => fod.id == item.id_subCostCenterExit);
                            item.id_boxedWarehouse = aProviderRawMaterial?.id_WarehouseCarton;
                            item.boxedWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_boxedWarehouse)?.name;
                            item.id_boxedWarehouseLocation = aProviderRawMaterial?.id_warehouseLocationCarton;
                            item.boxedWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_boxedWarehouseLocation)?.name;
                            item.id_costCenter = aProviderRawMaterial?.id_CostCenterCarton;
                            item.id_subCostCenter = aProviderRawMaterial?.id_SubCostCenterCarton;
                        }
                        else
                        {
                            item.id_boxedWarehouse = aFreezerMachineForProdDestination?.id_materialWarehouse;
                            item.boxedWarehouse = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_boxedWarehouse)?.name;
                            item.id_boxedWarehouseLocation = aFreezerMachineForProdDestination?.id_materialWarehouseLocation;
                            item.boxedWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_boxedWarehouseLocation)?.name;
                            item.id_costCenter = aFreezerMachineForProdDestination?.id_materialCostCenter;
                            item.id_subCostCenter = aFreezerMachineForProdDestination?.id_materialSubCostCenter;
                        }
                    }

                    var numberInventoryExitAux = "";
                    var numberInventoryEntryAux = "";
                    if (inventoryMoveTransferAutomaticExit != null)
                    {
                        foreach (var detail in ids_inventaryMoveTransferAutomaticEntry)
                        {
                            var inventaryMoveTransferAutomaticEntry = db.InventoryMove.FirstOrDefault(fod => fod.id == detail.id_document);
                            if (inventaryMoveTransferAutomaticEntry.idWarehouse == item.id_boxedWarehouse)
                            {
                                numberInventoryExitAux = inventoryMoveTransferAutomaticExit.natureSequential;
                                numberInventoryEntryAux = inventaryMoveTransferAutomaticEntry.natureSequential;
                                break;
                            }
                        }
                    }

                    item.numberInventoryExit = numberInventoryExitAux;
                    item.numberInventoryEntry = numberInventoryEntryAux;
                }
                openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails = openingClosingPlateLyingDetails
                    .Where(e => !idsOpeningClosingPlateRemove.Contains(e.id))
                    .ToList();

                openingClosingPlateLyingDto.selectedQuantity = openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.Sum(s => s.amount);
                openingClosingPlateLyingDto.selectedQuantityStr = openingClosingPlateLyingDto.selectedQuantity.ToString("#0,00");
            }

            // Asignamos la nueva bodega
            var habModificacionUbiDestino = DataProviderSetting.SettingByCode("MODUNTP")?.value == "SI";
            if (habModificacionUbiDestino && !tunnelTransferPlate)
            {
                var numDetalles = openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.Count;
                for (int i = 0; i < numDetalles; i++)
                {
                    var warehouse = db.Warehouse.FirstOrDefault(e => e.id == id_warehouseDestiny);
                    var warehouseLocation = db.WarehouseLocation
                        .FirstOrDefault(e => e.id_warehouse == id_warehouseDestiny
                            && e.id == id_warehouseLocationDestiny);

                    openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.ElementAt(i).id_boxedWarehouse = warehouse?.id; 
                    openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.ElementAt(i).boxedWarehouse = warehouse?.name; 
                    openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.ElementAt(i).id_boxedWarehouseLocation = warehouseLocation?.id; 
                    openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails.ElementAt(i).boxedWarehouseLocation = warehouseLocation?.name; 
                }
            }


            ViewBag.enabled = enabled;
            ViewBag.showInventory = db.DocumentState.FirstOrDefault(fod => fod.id == openingClosingPlateLyingDto.idSate && fod.code == "03") != null;

            return PartialView("_GridViewDetails", openingClosingPlateLyingDto.OpeningClosingPlateLyingDetails);
        }

        #endregion GridViewDetails

        #region Combobox

        private void BuildComboBoxState()
        {
            ViewData["Estados"] = db.DocumentState
                .Where(e => e.isActive
                    && e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoOpeningClosingPlateLying))
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

        private void BuildComboBoxFreezerWarehouseIndex()
        {
            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            var model = db.Warehouse.Where(t => t.id_company == ActiveCompany.id && t.isActive &&
                                                (t.WarehouseType.code.Equals("BCO01")/* || t.WarehouseType.code.Equals("BF01")*/)).ToList();//BCO01:Tipo Bodega: Bodega de Congelación y BF01: Tipo Bodega: Bodega Productos Frescos

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            ViewData["FreezerWarehouseIndex"] = model
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxFreezerWarehouseIndex()
        {
            BuildComboBoxFreezerWarehouseIndex();
            return PartialView("_ComboBoxFreezerWarehouseIndex");
        }

        private void BuildComboBoxFreezerWarehouseLocationIndex(int? id_freezerWarehouse = null)
        {
            ViewData["FreezerWarehouseLocationIndex"] = db.WarehouseLocation.Where(t => t.id_warehouse == id_freezerWarehouse && t.id_company == ActiveCompany.id && t.isActive).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxFreezerWarehouseLocationIndex(int? id_freezerWarehouse)
        {
            BuildComboBoxFreezerWarehouseLocationIndex(id_freezerWarehouse);
            return PartialView("_ComboBoxFreezerWarehouseLocationIndex");
        }

        private void BuildComboBoxBoxedWarehouseIndex()
        {
            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            var model = db.Warehouse.Where(t => t.id_company == ActiveCompany.id && t.isActive &&
                                                t.WarehouseType.code.Equals("BE01")).ToList();//BE01:Tipo Bodega: Bodega de Encartonado

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
        public ActionResult ComboBoxDestinyWarehouseLocation(
            int? idWarehouse, int? idWarehouseLocation, bool enabled = false)
        {
            var model = db.WarehouseLocation
                .Where(e => e.isActive && e.id_warehouse == idWarehouse)
                .ToArray();

            ViewBag.enabled = enabled;
            ViewBag.id_DestinyWarehouseLocation = idWarehouseLocation;
            return PartialView("_ComboBoxDestinyWarehouseLocation", model);
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

        private void BuildTokenBoxItemsIndex()
        {
            var codeMaster = db.Setting.FirstOrDefault(fod => fod.code == "PMASTER")?.value ?? "";
            ViewData["ItemsIndex"] = db.Item.Where(i => i.id_company == ActiveCompany.id &&
                                                         i.isActive &&
                                                         (i.InventoryLine.code.Equals("PP") || i.InventoryLine.code.Equals("PT")) &&
                                                         (i.ItemType != null) && (i.ItemType.ProcessType != null) &&
                                                         (i.Presentation != null) &&
                                                         (i.Presentation.code.Substring(0, 1) != codeMaster))
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult TokenBoxItemsIndex()
        {
            BuildTokenBoxItemsIndex();
            return PartialView("_TokenBoxItemsIndex");
        }

        private void BuildComboBoxResponsable()
        {
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = openingClosingPlateLyingDTO.responsable,
                Value = openingClosingPlateLyingDTO.id_responsable.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Employee.Where(g => g.Person.isActive && g.id != openingClosingPlateLyingDTO.id_responsable)
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
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = openingClosingPlateLyingDTO.turn,
                Value = openingClosingPlateLyingDTO.id_turn.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Turn.Where(g => g.isActive && g.id != openingClosingPlateLyingDTO.id_turn)
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

        private void BuildComboBoxFreezerMachineForProd()
        {
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aFreezerMachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == openingClosingPlateLyingDTO.id_freezerMachineForProd);
            var aText = aFreezerMachineForProd != null ? openingClosingPlateLyingDTO.freezerMachineForProd + (aFreezerMachineForProd.available ? " - Disponible" : (" - No Disponible - " + aFreezerMachineForProd.reason)) : "";
            var aSelectListItem = new SelectListItem
            {
                Text = aText,
                Value = openingClosingPlateLyingDTO.id_freezerMachineForProd.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange((DataProviderMachineForProd.MachineForProds((EntityObjectPermissions)ViewData["entityObjectPermissions"]) as List<MachineForProd>)
                                  .Where(w => (w.tbsysTypeMachineForProd.code.Equals("TUN") || w.tbsysTypeMachineForProd.code.Equals("PLA")) &&
                                               w.isActive && !w.available && w.id != openingClosingPlateLyingDTO.id_freezerMachineForProd)
                .Select(s => new SelectListItem
                {
                    Text = s.name + (s.available ? " - Disponible" : (" - No Disponible - " + s.reason)),
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["FreezerMachineForProd"] = aSelectListItems;
        }

        public ActionResult ComboBoxFreezerMachineForProd()
        {
            BuildComboBoxFreezerMachineForProd();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxFreezerMachineForProd");
        }

        private void BuildComboBoxFreezerMachineForProdDestination(int? id_freezerMachineForProdOrigin)
        {
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            SelectListItem aSelectListItem = null;
            var aFreezerMachineForProdDestination = db.MachineForProd.FirstOrDefault(fod => fod.id == openingClosingPlateLyingDTO.id_freezerMachineForProdDestination);
            var aText = aFreezerMachineForProdDestination != null ? openingClosingPlateLyingDTO.freezerMachineForProdDestination + (aFreezerMachineForProdDestination.available ? " - Disponible" : (" - No Disponible - " + aFreezerMachineForProdDestination.reason)) : "";
            if (openingClosingPlateLyingDTO.id_freezerMachineForProdDestination != null && openingClosingPlateLyingDTO.id_freezerMachineForProdDestination != id_freezerMachineForProdOrigin)
            {
                aSelectListItem = new SelectListItem
                {
                    Text = aText,
                    Value = openingClosingPlateLyingDTO.id_freezerMachineForProdDestination.ToString(),
                    Selected = true
                };
            }
            aSelectListItems.AddRange((DataProviderMachineForProd.MachineForProds((EntityObjectPermissions)ViewData["entityObjectPermissions"]) as List<MachineForProd>)
                                  .Where(w => (w.tbsysTypeMachineForProd.code.Equals("TUN") || w.tbsysTypeMachineForProd.code.Equals("PLA")) &&
                                               w.isActive && w.id != openingClosingPlateLyingDTO.id_freezerMachineForProdDestination &&
                                               w.id != id_freezerMachineForProdOrigin)
                .Select(s => new SelectListItem
                {
                    Text = s.name + (s.available ? " - Disponible" : (" - No Disponible - " + s.reason)),
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            if (openingClosingPlateLyingDTO.id_freezerMachineForProdDestination != null && openingClosingPlateLyingDTO.id_freezerMachineForProdDestination != id_freezerMachineForProdOrigin)
            {
                aSelectListItems.Insert(0, aSelectListItem);
            }

            ViewData["FreezerMachineForProdDestination"] = aSelectListItems;
        }

        public ActionResult ComboBoxFreezerMachineForProdDestination(int? id_freezerMachineForProdOrigin)
        {
            BuildComboBoxFreezerMachineForProdDestination(id_freezerMachineForProdOrigin);
            ViewBag.enabled = true;
            return PartialView("_ComboBoxFreezerMachineForProdDestination");
        }

        private void BuildComboBoxFreezerWarehouse(int? id_freezerMachineForProd = null)
        {
            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            var model = new List<Warehouse>();
            var aFreezerMachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == id_freezerMachineForProd);
            if (aFreezerMachineForProd?.id_materialWarehouse != null)
            {
                model.Add(new Warehouse
                {
                    id = aFreezerMachineForProd.id_materialWarehouse.Value,
                    name = db.Warehouse.FirstOrDefault(fod => fod.id == aFreezerMachineForProd.id_materialWarehouse)?.name
                });
            }
            if (aFreezerMachineForProd?.id_materialthirdWarehouse != null && aFreezerMachineForProd?.id_materialWarehouse != aFreezerMachineForProd?.id_materialthirdWarehouse)
            {
                model.Add(new Warehouse
                {
                    id = aFreezerMachineForProd.id_materialthirdWarehouse.Value,
                    name = db.Warehouse.FirstOrDefault(fod => fod.id == aFreezerMachineForProd.id_materialthirdWarehouse)?.name
                });
            }
            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = openingClosingPlateLyingDTO.freezerWarehouse,
                Value = openingClosingPlateLyingDTO.id_freezerWarehouse.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(model.Where(g => g.id != openingClosingPlateLyingDTO.id_freezerWarehouse)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);

            ViewData["FreezerWarehouse"] = aSelectListItems;
        }

        public ActionResult ComboBoxFreezerWarehouse(int? id_freezerMachineForProd)
        {
            BuildComboBoxFreezerWarehouse(id_freezerMachineForProd);
            ViewBag.enabled = true;
            return PartialView("_ComboBoxFreezerWarehouse");
        }

        private void BuildTokenBoxFreezerWarehouseLocations(int? id_freezerMachineForProd = null, int? id_freezerWarehouse = null)
        {
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            string data = openingClosingPlateLyingDTO.ids_freezerWarehouseLocation;
            string[] words = "".Split(',');
            if (id_freezerWarehouse == openingClosingPlateLyingDTO.id_freezerWarehouse)
            {
                words = data.Split(',');
            }
            List<WarehouseLocation> aWarehouseLocations = new List<WarehouseLocation>();
            var aFreezerMachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == id_freezerMachineForProd);
            if (aFreezerMachineForProd?.id_materialWarehouse != null && aFreezerMachineForProd?.id_materialWarehouse == id_freezerWarehouse)
            {
                if (aFreezerMachineForProd?.id_materialWarehouseLocation != null)
                {
                    var aWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == aFreezerMachineForProd.id_materialWarehouseLocation);
                    aWarehouseLocations.Add(new WarehouseLocation
                    {
                        id = aFreezerMachineForProd.id_materialWarehouseLocation.Value,
                        isActive = aWarehouseLocation?.isActive ?? true,
                        id_company = aWarehouseLocation?.id_company ?? ActiveCompany.id,
                        name = aWarehouseLocation?.name
                    });
                }
            }
            if (aFreezerMachineForProd?.id_materialthirdWarehouse != null && aFreezerMachineForProd?.id_materialthirdWarehouse == id_freezerWarehouse)
            {
                if (aFreezerMachineForProd?.id_materialthirdWarehouseLocation != null && aFreezerMachineForProd?.id_materialthirdWarehouseLocation != aFreezerMachineForProd?.id_materialWarehouseLocation)
                {
                    var aWarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == aFreezerMachineForProd.id_materialthirdWarehouseLocation);
                    aWarehouseLocations.Add(new WarehouseLocation
                    {
                        id = aFreezerMachineForProd.id_materialthirdWarehouseLocation.Value,
                        isActive = aWarehouseLocation?.isActive ?? true,
                        id_company = aWarehouseLocation?.id_company ?? ActiveCompany.id,
                        name = aWarehouseLocation?.name
                    });
                }
            }
            aSelectListItems.AddRange(aWarehouseLocations.Where(g => g.id_company == ActiveCompany.id && g.isActive &&
                                                                      !words.Contains(g.id.ToString()))
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            if (id_freezerWarehouse == openingClosingPlateLyingDTO.id_freezerWarehouse)
            {
                // Part 3: loop over result array.
                int count = 0;
                foreach (string word in words)
                {
                    if (word == "") continue;
                    var idAux = int.Parse(word);
                    var aSelectListItem = new SelectListItem
                    {
                        Text = db.WarehouseLocation.FirstOrDefault(fod => fod.id == idAux).name,
                        Value = word,
                        Selected = true
                    };
                    aSelectListItems.Insert(count, aSelectListItem);
                    count++;
                }
            }

            ViewData["FreezerWarehouseLocations"] = aSelectListItems;
        }

        public ActionResult TokenBoxFreezerWarehouseLocations(int? id_freezerMachineForProd, int? id_freezerWarehouse)
        {
            BuildTokenBoxFreezerWarehouseLocations(id_freezerMachineForProd, id_freezerWarehouse);
            ViewBag.enabled = true;
            return PartialView("_TokenBoxFreezerWarehouseLocations");
        }

        private void BuildTokenBoxProductionCarts()
        {
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            string data = openingClosingPlateLyingDTO.ids_productionCart;
            string[] words = data.Split(',');

            aSelectListItems.AddRange(db.ProductionCart.Where(g => g.isActive && !words.Contains(g.id.ToString()))
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            if (words.Count() > 0)
            {
                // Part 3: loop over result array.
                int count = 0;
                foreach (string word in words)
                {
                    if (word == "") continue;
                    var idAux = int.Parse(word);
                    var aSelectListItem = new SelectListItem
                    {
                        Text = db.ProductionCart.FirstOrDefault(fod => fod.id == idAux).name,
                        Value = word,
                        Selected = true
                    };
                    aSelectListItems.Insert(count, aSelectListItem);
                    count++;
                }
            }

            ViewData["ProductionCarts"] = aSelectListItems;
        }

        public ActionResult TokenBoxProductionCarts()
        {
            BuildTokenBoxProductionCarts();
            ViewBag.enabled = true;
            return PartialView("_TokenBoxProductionCarts");
        }

        private void BuildTokenBoxItems()
        {
            var codeMaster = db.Setting.FirstOrDefault(fod => fod.code == "PMASTER")?.value ?? "";
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            string data = openingClosingPlateLyingDTO.ids_item;
            string[] words = data.Split(',');

            aSelectListItems.AddRange(db.Item.Where(i => i.id_company == ActiveCompany.id &&
                                                         i.isActive &&
                                                         (i.InventoryLine.code.Equals("PP") || i.InventoryLine.code.Equals("PT")) &&
                                                         (i.ItemType != null) && (i.ItemType.ProcessType != null) &&
                                                         (i.Presentation != null) &&
                                                         (i.Presentation.code.Substring(0, 1) != codeMaster)).Where(g => g.isActive && !words.Contains(g.id.ToString()))
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());

            if (words.Count() > 0)
            {
                // Part 3: loop over result array.
                int count = 0;
                foreach (string word in words)
                {
                    //Console.WriteLine("WORD: " + word);
                    if (word == "") continue;
                    var idAux = int.Parse(word);
                    var aSelectListItem = new SelectListItem
                    {
                        Text = db.Item.FirstOrDefault(fod => fod.id == idAux).name,
                        Value = word,
                        Selected = true
                    };
                    aSelectListItems.Insert(count, aSelectListItem);
                    count++;
                }
            }

            ViewData["Items"] = aSelectListItems;
        }

        public ActionResult TokenBoxItems()
        {
            BuildTokenBoxItems();
            ViewBag.enabled = true;
            return PartialView("_TokenBoxItems");
        }

        private void BuildTokenBoxLots(int[] ids_lot, int? id_freezerWarehouse)
        {
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();

            Parametros.ParametrosBusquedaOpeningClosingPlateLying parametrosBusquedaOpeningClosingPlateLying = new Parametros.ParametrosBusquedaOpeningClosingPlateLying();
            parametrosBusquedaOpeningClosingPlateLying.id_warehouse = id_freezerWarehouse ?? 0;
            parametrosBusquedaOpeningClosingPlateLying.id_openingClosingPlateLyingDto = openingClosingPlateLyingDTO.id;
            parametrosBusquedaOpeningClosingPlateLying.id_warehouseType = db.WarehouseType.FirstOrDefault(t => t.code.Equals("BCO01"))?.id;//BCO01:Tipo Bodega: Bodega de Congelación
            parametrosBusquedaOpeningClosingPlateLying.for_lot = 1;

            var parametrosBusquedaOpeningClosingPlateLyingAux = new SqlParameter();
            parametrosBusquedaOpeningClosingPlateLyingAux.ParameterName = "@ParametrosBusquedaOpeningClosingPlateLying";
            parametrosBusquedaOpeningClosingPlateLyingAux.Direction = ParameterDirection.Input;
            parametrosBusquedaOpeningClosingPlateLyingAux.SqlDbType = SqlDbType.NVarChar;
            var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaOpeningClosingPlateLying);
            parametrosBusquedaOpeningClosingPlateLyingAux.Value = jsonAux;
            db.Database.CommandTimeout = 1200;
            List<OpeningClosingPlateLyingDetailDTO> openingClosingPlateLyingDetails = db.Database
                .SqlQuery<OpeningClosingPlateLyingDetailDTO>("exec inv_Consultar_OpeningClosingPlateLying_StoredProcedure @ParametrosBusquedaOpeningClosingPlateLying ", parametrosBusquedaOpeningClosingPlateLyingAux)
                .ToList();

            if (ids_lot != null && ids_lot.Length > 0)
            {
                openingClosingPlateLyingDetails = openingClosingPlateLyingDetails.Where(d => !ids_lot.Contains(d.id_lot ?? 0)).ToList();
            }
            aSelectListItems.AddRange(openingClosingPlateLyingDetails
                .Select(s => new SelectListItem
                {
                    Text = s.noLote + " - " + s.noSecTransLote,
                    Value = s.id_lot == null ? "0" : s.id_lot.Value.ToString(),
                    Selected = false
                }).ToList());

            if (ids_lot != null && ids_lot.Count() > 0)
            {
                // Part 3: loop over result array.
                int count = 0;
                foreach (int lot in ids_lot)
                {
                    if (lot == 0) continue;
                    var aProductionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == lot);
                    var aSelectListItem = new SelectListItem
                    {
                        Text = aProductionLot.number + " - " + aProductionLot.internalNumber,
                        Value = lot.ToString(),
                        Selected = true
                    };
                    aSelectListItems.Insert(count, aSelectListItem);
                    count++;
                }
            }

            ViewData["Lots"] = aSelectListItems;
        }

        public ActionResult TokenBoxLots(int[] ids_lot = null, int? id_freezerWarehouse = null)
        {
            BuildTokenBoxLots(ids_lot, id_freezerWarehouse);
            ViewBag.enabled = true;
            return PartialView("_TokenBoxLots");
        }
        #endregion Combobox

        [HttpPost]
        public JsonResult Save(string jsonOpeningClosingPlateLying)
        {

            var newObject = false;
            bool isValid = false;
            OpeningClosingPlateLying openingClosingPlateLying = null;
            var result = new ApiResult();

            using (var db = new DBContext())
            {
                try
                {
                    var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();

                    JToken token = JsonConvert.DeserializeObject<JToken>(jsonOpeningClosingPlateLying);


                    var id = token.Value<int>("id");

                    var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoOpeningClosingPlateLying));
                    var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                    openingClosingPlateLying = db.OpeningClosingPlateLying.FirstOrDefault(d => d.id == id);
                    if (openingClosingPlateLying == null)
                    {
                        newObject = true;

                        var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                            ? ActiveUser.EmissionPoint.First().id
                            : 0;
                        if (id_emissionPoint == 0)
                            throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

                        openingClosingPlateLying = new OpeningClosingPlateLying
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

                        documentType.currentNumber++;
                        db.DocumentType.Attach(documentType);
                        db.Entry(documentType).State = EntityState.Modified;
                    }
                    openingClosingPlateLyingDTO.freezerWarehouse = token.Value<string>("freezerWarehouse");
                    openingClosingPlateLyingDTO.id_freezerWarehouse = token.Value<int>("id_freezerWarehouse");
                    openingClosingPlateLying.id_freezerWarehouse = token.Value<int>("id_freezerWarehouse");

                    #region Validación Permiso

                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == openingClosingPlateLying.id_freezerWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                            if (entityValuePermissions == null)
                            {
                                throw new Exception("No tiene Permiso para editar y guardar la Tumbada de Placa para la Bodega de Congelación: " + openingClosingPlateLyingDTO.freezerWarehouse);
                            }
                        }
                    }

                    #endregion Validación Permiso

                    openingClosingPlateLying.Document.emissionDate = token.Value<DateTime>("dateTimeEmision");
                    openingClosingPlateLying.Document.reference = token.Value<string>("reference");
                    openingClosingPlateLying.Document.description = token.Value<string>("description");
                    openingClosingPlateLying.Document.id_documentState = documentState.id;
                    openingClosingPlateLying.Document.id_userUpdate = ActiveUser.id;
                    openingClosingPlateLying.Document.dateUpdate = DateTime.Now;
                    openingClosingPlateLying.id_responsable = token.Value<int>("id_responsable");
                    openingClosingPlateLying.dateTimeStartLying = token.Value<DateTime>("dateTimeStartLying");
                    openingClosingPlateLying.dateTimeEndLying = token.Value<DateTime>("dateTimeEndLying");
                    openingClosingPlateLying.temperature = token.Value<decimal>("temperature");
                    openingClosingPlateLying.id_freezerMachineForProd = token.Value<int>("id_freezerMachineForProd");
                    openingClosingPlateLying.id_turn = token.Value<int?>("id_turn");
                    openingClosingPlateLying.tunnelTransferPlate = token.Value<bool>("tunnelTransferPlate");
                    openingClosingPlateLying.id_freezerMachineForProdDestination = token.Value<int?>("id_freezerMachineForProdDestination");
                    openingClosingPlateLying.id_warehouseDestiny = token.Value<int?>("id_warehouseDestiny");
                    openingClosingPlateLying.id_warehouseLocationDestiny = token.Value<int?>("id_warehouseLocationDestiny");
                    openingClosingPlateLyingDTO.id_turn = openingClosingPlateLying.id_turn;
                    openingClosingPlateLyingDTO.id_freezerMachineForProdDestination = openingClosingPlateLying.id_freezerMachineForProdDestination;
                    openingClosingPlateLyingDTO.id_freezerMachineForProd = openingClosingPlateLying.id_freezerMachineForProd;

                    if (openingClosingPlateLying.tunnelTransferPlate)
                    {
                        var aMachineForProdDestination = db.MachineForProd.FirstOrDefault(fod => fod.id == openingClosingPlateLyingDTO.id_freezerMachineForProdDestination);
                        if (!aMachineForProdDestination.available)
                        {
                            throw new Exception("El túnel - Placa a Transferir: " + aMachineForProdDestination.name + " no está disponible por motivo: " + aMachineForProdDestination.reason + ".");
                        }
                    }

                    var ids_freezerWarehouseLocationAux = token.Value<JArray>("ids_freezerWarehouseLocation");
                    var ids_freezerWarehouseLocationStrAux = "";
                    foreach (var item in ids_freezerWarehouseLocationAux.Children())
                    {
                        if (ids_freezerWarehouseLocationStrAux == "") ids_freezerWarehouseLocationStrAux = item.Value<int>().ToString();
                        else ids_freezerWarehouseLocationStrAux += "," + item.Value<int>().ToString();
                    }
                    openingClosingPlateLying.ids_freezerWarehouseLocation = ids_freezerWarehouseLocationStrAux;

                    var ids_productionCartAux = token.Value<JArray>("ids_productionCart");
                    var ids_productionCartStrAux = "";
                    foreach (var item in ids_productionCartAux.Children())
                    {
                        if (ids_productionCartStrAux == "") ids_productionCartStrAux = item.Value<int>().ToString();
                        else ids_productionCartStrAux += "," + item.Value<int>().ToString();
                    }
                    openingClosingPlateLying.ids_productionCart = ids_productionCartStrAux;

                    var ids_itemAux = token.Value<JArray>("ids_item");
                    var ids_itemStrAux = "";
                    foreach (var item in ids_itemAux.Children())
                    {
                        if (ids_itemStrAux == "") ids_itemStrAux = item.Value<int>().ToString();
                        else ids_itemStrAux += "," + item.Value<int>().ToString();
                    }
                    openingClosingPlateLying.ids_item = ids_itemStrAux;

                    var ids_lotAux = token.Value<JArray>("ids_lot");
                    var ids_lotStrAux = "";
                    foreach (var lot in ids_lotAux.Children())
                    {
                        if (ids_lotStrAux == "") ids_lotStrAux = lot.Value<int>().ToString();
                        else ids_lotStrAux += "," + lot.Value<int>().ToString();
                    }
                    openingClosingPlateLying.ids_lot = ids_lotStrAux;

                    openingClosingPlateLying.id_company = ActiveCompany.id;
                    openingClosingPlateLying.selectedQuantity = openingClosingPlateLyingDTO.selectedQuantity;//token.Value<decimal>("selectedQuantity");

                    var ids_selectAux = token.Value<JArray>("ids");
                    //Details
                    if (ids_selectAux.Count() <= 0)
                    {
                        throw new Exception("No se puede guardar una Tumbada de Placa. Sin detalle.");
                    }
                    var lastDetails = db.OpeningClosingPlateLyingDetail.Where(d => d.id_openingClosingPlateLying == openingClosingPlateLying.id);
                    foreach (var detail in lastDetails)
                    {
                        db.OpeningClosingPlateLyingDetail.Remove(detail);
                        db.OpeningClosingPlateLyingDetail.Attach(detail);
                        db.Entry(detail).State = EntityState.Deleted;
                    }

                    foreach (var detail in ids_selectAux.Children())
                    {
                        int aId = detail.Value<int>();
                        var aDetail = openingClosingPlateLyingDTO.OpeningClosingPlateLyingDetails.AsEnumerable().FirstOrDefault(d => d.id == aId);
                        if (aDetail.id_costCenterExit == null)
                        {
                            throw new Exception("Existe, detalle sin Centro de Costo en la configuración del Túnel/Placa. Verifique el caso e intente de nuevo.");
                        }
                        if (aDetail.id_subCostCenter == null)
                        {
                            throw new Exception("Existe, detalle sin SubCentro de Costo en la configuración del Túnel/Placa. Verifique el caso e intente de nuevo.");
                        }
                        if (aDetail.id_boxedWarehouse == null)
                        {
                            throw new Exception("Existe, detalle sin Bodega de Destino Verifique el caso e intente de nuevo.");
                        }
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == aDetail.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                            if (entityValuePermissions == null)
                            {
                                throw new Exception("No tiene Permiso para editar y guardar la Tumbada de Placa para la Bodega de Destino: " + aDetail.boxedWarehouse);
                            }
                        }
                        if (aDetail.id_boxedWarehouseLocation == null)
                        {
                            throw new Exception("Existe, detalle sin Ubicación de Bodega de Destino. Verifique el caso e intente de nuevo.");
                        }
                        if (aDetail.id_costCenter == null)
                        {
                            throw new Exception("Existe, detalle sin Centro de Costo de Destino. Verifique el caso e intente de nuevo.");
                        }
                        if (aDetail.id_subCostCenter == null)
                        {
                            throw new Exception("Existe, detalle sin SubCentro de Costo de Destino. Verifique el caso e intente de nuevo.");
                        }
                        var aOpeningClosingPlateLyingDetail = new OpeningClosingPlateLyingDetail
                        {
                            id_openingClosingPlateLying = openingClosingPlateLying.id,
                            id_lot = aDetail.id_lot.Value,
                            id_item = aDetail.id_item,
                            id_warehouse = aDetail.id_warehouse,
                            id_warehouseLocation = aDetail.id_warehouseLocation.Value,
                            id_costCenterExit = aDetail.id_costCenterExit.Value,
                            id_subCostCenterExit = aDetail.id_subCostCenterExit.Value,
                            id_productionCart = aDetail.id_productionCart,
                            amount = aDetail.amount,
                            id_metricUnit = aDetail.id_metricUnit,
                            id_boxedWarehouse = aDetail.id_boxedWarehouse.Value,
                            id_boxedWarehouseLocation = aDetail.id_boxedWarehouseLocation.Value,
                            id_costCenter = aDetail.id_costCenter.Value,
                            id_subCostCenter = aDetail.id_subCostCenter.Value
                        };
                        openingClosingPlateLying.OpeningClosingPlateLyingDetail.Add(aOpeningClosingPlateLyingDetail);
                    }

                    isValid = true;
                }
                catch (Exception e)
                {
                    result.Code = e.HResult;
                    result.Message = e.Message;
                    FullLog(e);
                }

                if (isValid)
            {

                using (var trans = db.Database.BeginTransaction())
                {

                    try
                    {

                        if (newObject)
                        {
                            db.OpeningClosingPlateLying.Add(openingClosingPlateLying);
                            db.Entry(openingClosingPlateLying).State = EntityState.Added;
                        }
                        else
                        {
                            db.OpeningClosingPlateLying.Attach(openingClosingPlateLying);
                            db.Entry(openingClosingPlateLying).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        trans.Commit();

                        result.Data = openingClosingPlateLying.id.ToString();
                    }
                    catch (Exception e)
                    {

                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                        FullLog(e);
                    }

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
                var resultTransac = ApproveOpeningClosingPlateLying(id);
                result.Data = resultTransac.Item1?.name;
                result.Code = (resultTransac.Item2) ? 0 : ServiceTransCtl.CODE_FOR_QUEUE; // retorna valor -6666 si se encola
                //result.Data = ApproveOpeningClosingPlateLying(id).name;
            }
            catch (ProdHandlerException e)
            {
                result.Code = 1000;
                result.Message = e.Message;
            }
            catch (Exception e)
            {
                result.Code = e.HResult;
                result.Message = GenericError.ErrorGeneralOpeningClosingPlateLying;
                FullLog(e);
            }

            return Json(result, JsonRequestBehavior.AllowGet);


            //using (var db = new DBContext())
            //{
            //
            //    //using (var trans = db.Database.BeginTransaction())
            //    //{
            //    //    
            //    //}
            //}
        }


        public Tuple<DocumentState, bool> ApproveOpeningClosingPlateLying(int id_openingClosingPlateLying,
                                                                               bool isInternalTrans = false,
                                                                               ActiveUserDto sessionInfoTrans = null,
                                                                               string tempKeyTrans = null,
                                                                               string tempValueTrans = null,
                                                                               string tempTypeTrans = null)
        {
            DocumentState documentState = null;
            bool isExecute = false;
            Guid? identificadorTran = null;
            bool isValid = false;
            int[] inventoryMoveDetailIdsForDelete = Array.Empty<int>();
            try
            {
                using (var db = new DBContext())
                {
                    var openingClosingPlateLying = db.OpeningClosingPlateLying.FirstOrDefault(p => p.id == id_openingClosingPlateLying);
                    // -> obtener el modelo para los detalles
                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
                    // -> obtener los permisos para luego almacenarlos
                    string documentNumber = db.Document.FirstOrDefault(r => r.id == id_openingClosingPlateLying)?.number;
                    // -> Obtener el numero de documento para asignarlo al proceso

                    //isExecute = true;
                    if (!isInternalTrans)
                    {
                        int documentTypeId = db.DocumentType.FirstOrDefault(r => r.code == "37").id;
                        int numDetails = openingClosingPlateLying.OpeningClosingPlateLyingDetail.Count();
                        string sessionInfo = ServiceTransCtl.GetSessionInfoSerialize(ActiveUserId,
                                                                                        ActiveUser.username,
                                                                                        ActiveCompanyId,
                                                                                        ActiveEmissionPoint.id,
                                                                                        entityObjectPermissions);

                        string dataExecution = JsonConvert.SerializeObject(new object[] { id_openingClosingPlateLying });
                        string dataExecutionTypes = JsonConvert.SerializeObject(new object[] { "System.Int32" });

                        var result = ServiceTransCtl.TransCtlExternal(
                                                            id_openingClosingPlateLying,
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
                        var openingClosingPlateLyingDTO = ConvertToDto(openingClosingPlateLying);
                        if (openingClosingPlateLying != null)
                        {
                            #region Validación Permiso

                            if (entityObjectPermissions != null)
                            {
                                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                if (entityPermissions != null)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == openingClosingPlateLying.id_freezerWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new ProdHandlerException("No tiene Permiso para aprobar la Tumbada de Placa para la Bodega de Congelación: " + openingClosingPlateLyingDTO.freezerWarehouse + ".");
                                    }
                                }
                            }

                            #endregion Validación Permiso
                            foreach (var item in openingClosingPlateLyingDTO.OpeningClosingPlateLyingDetails)
                            {
                                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                if (entityPermissions != null)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new ProdHandlerException("No tiene Permiso para aprobar la Tumbada de Placa para la Bodega de Encartonado: " + item.boxedWarehouse + ".");
                                    }
                                }
                            }
                             
                            using (var trans = db.Database.BeginTransaction())
                            {
                                try
                                {


                                    var result = ServiceInventoryMove.UpdateInventaryMoveLyingDownExit(
                                       true,
                                       ActiveUser,
                                       ActiveCompany,
                                       ActiveEmissionPoint,
                                       openingClosingPlateLyingDTO,
                                       db,                                    
                                       false,
                                       trans: trans);

                                    if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                                    inventoryMoveDetailIdsForDelete = result.inventoryMoveDetailIdsForDelete;


                                    var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                                    if (aprovedState == null)
                                        throw new ProdHandlerException("No se ha definido el estado aprobado");

                                    //Actualizar Disponibilidad
                                    var aMachineForProd = db.MachineForProd.FirstOrDefault(fod => fod.id == openingClosingPlateLying.id_freezerMachineForProd);
                                    aMachineForProd.available = true;
                                    aMachineForProd.reason = "";
                                    aMachineForProd.id_userUpdateAvailable = ActiveUser.id;
                                    aMachineForProd.dateUpdateAvailable = DateTime.Now;

                                    //db.MachineForProd.Attach(aMachineForProd);
                                    db.Entry(aMachineForProd).State = EntityState.Modified;

                                    openingClosingPlateLying.Document.id_documentState = aprovedState.id;
                                    openingClosingPlateLying.Document.authorizationDate = DateTime.Now;

                                    //db.OpeningClosingPlateLying.Attach(openingClosingPlateLying);
                                    db.Entry(openingClosingPlateLying).State = EntityState.Modified;

                                    db.SaveChanges();
                                    trans.Commit();

                                    isValid = true;

                                }
                                catch (Exception ex)
                                {
                                    trans.Rollback();
                                    throw;
                                }
                            }
                             
                        
                        }
                        else
                        {
                            throw new ProdHandlerException("No se encontró el objeto seleccionado");
                        }

                        documentState = openingClosingPlateLying.Document.DocumentState;
                    }

                }

                if (isValid)
                {

                    if ((inventoryMoveDetailIdsForDelete?.Length ?? 0) > 0)
                    {
                        Task.Run(async() =>
                        {

                            await ServiceTransCtl.DeleteBulkSp(inventoryMoveDetailIdsForDelete, this.GetType().Name, id_openingClosingPlateLying);
                            //var db1 = new DBContext();
                            //using (DbContextTransaction trans = db1.Database.BeginTransaction())
                            //{
                            //    try
                            //    {
                            //
                            //        string ids = inventoryMoveDetailIdsForDelete
                            //                        .Select(r => r.ToString())
                            //                        .Aggregate((i, j) => $"{i}|{j}");
                            //
                            //        System.Data.Common.DbTransaction transaction = trans.UnderlyingTransaction;
                            //        SqlConnection connectionDel = transaction.Connection as SqlConnection;
                            //
                            //        connectionDel.Execute("dbo.TransCtlDeleteInventoryMoveDetail", new
                            //        {
                            //            inventoryMoveDetailIds = ids
                            //        }, transaction, 300, CommandType.StoredProcedure);
                            //
                            //        trans.Commit();
                            //
                            //    }
                            //    catch (Exception e)
                            //    {
                            //        trans.Rollback();
                            //        FullLog(e);
                            //        throw;
                            //    }
                            //
                            //}

                        });

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
                // cerrar TransacCTL
                if (isExecute)
                {
                    ServiceTransCtl.Finalize(identificadorTran.Value);
                }

            }

            return new Tuple<DocumentState, bool>(documentState, isExecute);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Conciliate(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();
                    try
                    {
                        result.Data = ConciliateApproveOpeningClosingPlateLying(id).name;
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

        private DocumentState ConciliateApproveOpeningClosingPlateLying(int id_openingClosingPlateLying)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var openingClosingPlateLying = db.OpeningClosingPlateLying.FirstOrDefault(p => p.id == id_openingClosingPlateLying);
                    try
                    {
                        if (openingClosingPlateLying != null)
                        {                            
                            // Actualizo los movimientos de inventario
                            var openingClosingPlateLyingDTO = ConvertToDto(openingClosingPlateLying);

                            // Buscamos el estado 'Conciliado'
                            var estadoConciliado = db.DocumentState.FirstOrDefault(e => e.code == "16");
                            if (estadoConciliado == null)
                                throw new Exception("No se ha encontrado el estado: [16] - CONCILIADO");

                            // Actualizamos los movimientos de inventario
                            ConciliateInventoryMoveLyingDownExit(estadoConciliado.code, db, openingClosingPlateLyingDTO);
                            
                            // Actualizao el estado del documento
                            openingClosingPlateLying.Document.id_documentState = estadoConciliado.id;
                            openingClosingPlateLying.Document.id_userUpdate = this.ActiveUserId;
                            openingClosingPlateLying.Document.dateUpdate = DateTime.Now;

                            db.OpeningClosingPlateLying.Attach(openingClosingPlateLying);
                            db.Entry(openingClosingPlateLying).State = EntityState.Modified;
                            db.SaveChanges();

                            trans.Commit();
                        }
                        else
                        {
                            throw new Exception("No se encontro el objeto seleccionado");
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = ex.InnerException?.Message ?? ex.Message;
                        throw new Exception($"Error al conciliar el documento: {message}");
                    }

                    return openingClosingPlateLying.Document.DocumentState;
                }
            }                
        }

        private void ConciliateInventoryMoveLyingDownExit(string codeState, DBContext db, OpeningClosingPlateLyingDTO paramOpeningClosingPlateLyingDTO)
        {
            try
            {
                // Buscamos el estado 'Conciliado'
                var estado = db.DocumentState.FirstOrDefault(e => e.code == codeState);
                if (estado == null)
                    throw new Exception($"No se ha encontrado el estado: [{codeState}] - CONCILIADO");

                string[] codigoMovsTransferencia = new string[] { "142", "143" };
                switch (codeState)
                {
                    // Caso Conciliado
                    //142: Egreso Por Transferencia Automática Por Tumbada Placa
                    //143: Ingreso Por Transferencia Automática Por Tumbada Placa
                    case "16":
                        
                        foreach (var codMovTransf in codigoMovsTransferencia)
                        {
                            var id_inventaryMoveTransferAutomaticConc = db.DocumentSource
                                                                            .FirstOrDefault(fod => fod.id_documentOrigin == paramOpeningClosingPlateLyingDTO.id &&
                                                                            fod.Document.DocumentState.code.Equals("03") &&
                                                                            fod.Document.DocumentType.code.Equals(codMovTransf))?.id_document;
                            var inventoryMoveTransferAutomatic = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveTransferAutomaticConc);
                            if (inventoryMoveTransferAutomatic != null)
                            {
                                inventoryMoveTransferAutomatic.Document.id_documentState = estado.id;
                                inventoryMoveTransferAutomatic.Document.id_userUpdate = this.ActiveUserId;
                                inventoryMoveTransferAutomatic.Document.dateUpdate = DateTime.Now;

                                db.InventoryMove.Attach(inventoryMoveTransferAutomatic);
                                db.Entry(inventoryMoveTransferAutomatic).State = EntityState.Modified;
                                db.SaveChanges();
                            } 
                        }
                        break;
                    // Caso Reversar a Aprobado
                    case "03":
                        foreach (var codMovTransf in codigoMovsTransferencia)
                        {
                            var id_inventaryMoveTransferAutomatic = db.DocumentSource
                                                                            .FirstOrDefault(fod => fod.id_documentOrigin == paramOpeningClosingPlateLyingDTO.id &&
                                                                            fod.Document.DocumentState.code.Equals("16") &&
                                                                            fod.Document.DocumentType.code.Equals(codMovTransf))?.id_document;
                            var inventoryMoveTransferAutomaticApr = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveTransferAutomatic);
                            if (inventoryMoveTransferAutomaticApr != null)
                            {
                                inventoryMoveTransferAutomaticApr.Document.id_documentState = estado.id;
                                inventoryMoveTransferAutomaticApr.Document.id_userUpdate = this.ActiveUserId;
                                inventoryMoveTransferAutomaticApr.Document.dateUpdate = DateTime.Now;

                                db.InventoryMove.Attach(inventoryMoveTransferAutomaticApr);
                                db.Entry(inventoryMoveTransferAutomaticApr).State = EntityState.Modified;
                                db.SaveChanges();
                            } 
                        }
                        break;
                    default:
                        throw new Exception("Estado de documento no encontrado");
                }
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(message);
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
                        // Revisión del documento 
                        var document = db.Document.FirstOrDefault(e => e.id == id);
                        if(document.DocumentState.code == "16")
                        {
                            result.Data = ReverseConciliate(id).name;
                        }
                        else
                        {
                            result.Data = ReverseOpeningClosingPlateLying(id).name;
                        }                        
                    }
                    catch (Exception e)
                    {
                        FullLog(e);
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private DocumentState ReverseConciliate(int id_openingClosingPlateLying)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var openingClosingPlateLying = db.OpeningClosingPlateLying.FirstOrDefault(p => p.id == id_openingClosingPlateLying);
                    try
                    {
                        if (openingClosingPlateLying != null)
                        {
                            // Actualizo los movimientos de inventario
                            var openingClosingPlateLyingDTO = ConvertToDto(openingClosingPlateLying);

                            // Estado Aprobado
                            var approvedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                            if (approvedState == null)
                                throw new Exception("No se ha encontrado el estado: [03] - APROBADO");

                            //Verificar si existe lotes relacionados al proceso de cierre
                            var openingClosingPlateLyingDetail = openingClosingPlateLying.OpeningClosingPlateLyingDetail.ToList();
                            foreach (var item in openingClosingPlateLyingDetail)
                            {
                                var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == item.id_lot && a.isClosed);
                                if (productionLot != null)
                                {
                                    var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.id_lot == item.id_lot && a.Document.DocumentState.code != "05"
                                                            && a.isActive);

                                    if (productionLotClose != null && productionLot.receptionDate.Date <= productionLotClose.Document.emissionDate.Date 
                                        && productionLot.ProductionLotState.code == "11")
                                    {
                                        throw new Exception("El lote " + productionLot.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                                    }
                                }
                            }

                            // Actualizamos los movimientos de inventario
                            ConciliateInventoryMoveLyingDownExit(approvedState.code, db, openingClosingPlateLyingDTO);

                            // Actualizao el estado del documento
                            openingClosingPlateLying.Document.id_documentState = approvedState.id;
                            openingClosingPlateLying.Document.id_userUpdate = this.ActiveUserId;
                            openingClosingPlateLying.Document.dateUpdate = DateTime.Now;

                            db.OpeningClosingPlateLying.Attach(openingClosingPlateLying);
                            db.Entry(openingClosingPlateLying).State = EntityState.Modified;
                            db.SaveChanges();

                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = ex.InnerException?.Message ?? ex.Message;
                        throw new Exception($"Error al reversar el documento: {message}");
                    }

                    return openingClosingPlateLying.Document.DocumentState;
                }
            }
        }
        private DocumentState ReverseOpeningClosingPlateLying(int id_openingClosingPlateLying)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var openingClosingPlateLying = db.OpeningClosingPlateLying.FirstOrDefault(p => p.id == id_openingClosingPlateLying);
                    var openingClosingPlateLyingDTO = ConvertToDto(openingClosingPlateLying);
                    if (openingClosingPlateLying != null)
                    {
                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == openingClosingPlateLying.id_freezerWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para reversar la Tumbada de Placa para la Bodega de Congelación: " + openingClosingPlateLyingDTO.freezerWarehouse + ".");
                                }
                            }
                        }

                        #endregion Validación Permiso

                        //Verificar si existe lotes relacionados al proceso de cierre
                        var openingClosingPlateLyingDetail = openingClosingPlateLying.OpeningClosingPlateLyingDetail.ToList();
                        foreach (var item in openingClosingPlateLyingDetail)
                        {
                            var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == item.id_lot && a.isClosed);
                            if (productionLot != null)
                            {
                                var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.id_lot == item.id_lot && a.Document.DocumentState.code != "05"
                                                        && a.isActive);

                                if (productionLotClose != null && productionLot.receptionDate.Date <= productionLotClose.Document.emissionDate.Date 
                                    && productionLot.ProductionLotState.code == "11")
                                {
                                    throw new Exception("El lote " + productionLot.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                                }
                            }
                        }

                        foreach (var item in openingClosingPlateLyingDTO.OpeningClosingPlateLyingDetails)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para reversar la Tumbada de Placa para la Bodega de Encartonado: " + item.boxedWarehouse + ".");
                                }
                            }
                        }
                        var id_inventaryMoveTransferAutomaticExit = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == openingClosingPlateLyingDTO.id &&
                                                                   fod.Document.DocumentState.code.Equals("03") &&
                                                                   fod.Document.DocumentType.code.Equals("142"))?.id_document;//142: Egreso Por Transferencia Automática Por Tumbada Placa
                        var inventoryMoveTransferAutomaticExit = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveTransferAutomaticExit);
                        ServiceInventoryMove.UpdateInventaryMoveLyingDownExit(false, ActiveUser, ActiveCompany, ActiveEmissionPoint, openingClosingPlateLyingDTO, db, true, inventoryMoveTransferAutomaticExit);

                        var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
                        if (reverseState == null)
                            return

                        openingClosingPlateLying.Document.DocumentState;

                        inventoryMoveTransferAutomaticExit.Document.id_documentState = reverseState.id;
                        inventoryMoveTransferAutomaticExit.Document.DocumentState = reverseState;

                        db.InventoryMove.Attach(inventoryMoveTransferAutomaticExit);
                        db.Entry(inventoryMoveTransferAutomaticExit).State = EntityState.Modified;

                        openingClosingPlateLying.Document.id_documentState = reverseState.id;
                        openingClosingPlateLying.Document.authorizationDate = DateTime.Now;

                        db.OpeningClosingPlateLying.Attach(openingClosingPlateLying);
                        db.Entry(openingClosingPlateLying).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return openingClosingPlateLying.Document.DocumentState;
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
                        result.Data = AnnulOpeningClosingPlateLying(id).name;
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

        private DocumentState AnnulOpeningClosingPlateLying(int id_openingClosingPlateLying)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var openingClosingPlateLying = db.OpeningClosingPlateLying.FirstOrDefault(p => p.id == id_openingClosingPlateLying);
                    var openingClosingPlateLyingDTO = ConvertToDto(openingClosingPlateLying);
                    if (openingClosingPlateLying != null)
                    {
                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == openingClosingPlateLying.id_freezerWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para anular la Tumbada de Placa para la Bodega de Congelación: " + openingClosingPlateLyingDTO.freezerWarehouse + ".");
                                }
                            }
                        }

                        #endregion Validación Permiso
                        foreach (var item in openingClosingPlateLyingDTO.OpeningClosingPlateLyingDetails)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_boxedWarehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para anular la Tumbada de Placa para la Bodega de Encartonado: " + item.boxedWarehouse + ".");
                                }
                            }
                        }

                        var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        if (annulState == null)
                            return

                        openingClosingPlateLying.Document.DocumentState;
                        openingClosingPlateLying.Document.id_documentState = annulState.id;
                        openingClosingPlateLying.Document.authorizationDate = DateTime.Now;

                        db.OpeningClosingPlateLying.Attach(openingClosingPlateLying);
                        db.Entry(openingClosingPlateLying).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return openingClosingPlateLying.Document.DocumentState;
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id)
        {
            var index = GetOpeningClosingPlateLyingResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

            var result = new
            {
                maximunPages = GetOpeningClosingPlateLyingResultConsultDTO().Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            var element = GetOpeningClosingPlateLyingResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
            var openingClosingPlateLying = db.OpeningClosingPlateLying.FirstOrDefault(d => d.id == element.id);
            if (openingClosingPlateLying == null)
                return PartialView("Edit", new OpeningClosingPlateLyingDTO());

            BuildViewDataEdit();
            var model = ConvertToDto(openingClosingPlateLying);
            SetOpeningClosingPlateLyingDTO(model);
            BuilViewBag(false);

            return PartialView("Edit", model);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetTotal()
        {
            var openingClosingPlateLying = GetOpeningClosingPlateLyingDTO();

            var result = new
            {
                message = "OK",
                openingClosingPlateLying.selectedQuantityStr,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetTotalBySelected(int[] ids = null)
        {
            var openingClosingPlateLyingDTO = GetOpeningClosingPlateLyingDTO();

            var result = new
            {
                message = "OK",
                openingClosingPlateLyingDTO.selectedQuantityStr
            };

            if (ids == null || ids.Count() <= 0)
            {
                result = new
                {
                    message = "OK",//"Debe seleccionar algun detalle.",
                    selectedQuantityStr = "0,00"
                };
                //throw new Exception("No se puede guardar una Tumbada de Placa. Sin detalle.");
            }
            else
            {
                openingClosingPlateLyingDTO.selectedQuantity = openingClosingPlateLyingDTO.OpeningClosingPlateLyingDetails.Where(w => ids.Contains(w.id)).Sum(s => s.amount);
                openingClosingPlateLyingDTO.selectedQuantityStr = openingClosingPlateLyingDTO.selectedQuantity.ToString("#.00");
                result = new
                {
                    message = "OK",
                    openingClosingPlateLyingDTO.selectedQuantityStr
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult TurnChanged(int? id_turn)
        {
            var openingClosingPlateLyingDto = GetOpeningClosingPlateLyingDTO();
            var result = new
            {
                message = "",
                turn = "",
                timeInitTurn = "",
                timeEndTurn = ""
            };
            var aTurn = db.Turn.FirstOrDefault(fod => fod.id == id_turn);

            openingClosingPlateLyingDto.timeInitTurn = aTurn != null ? (aTurn.timeInit.Hours + ":" + aTurn.timeInit.Minutes) : null;
            openingClosingPlateLyingDto.timeEndTurn = aTurn != null ? (aTurn.timeEnd.Hours + ":" + aTurn.timeEnd.Minutes) : null;
            openingClosingPlateLyingDto.turn = aTurn?.name;
            openingClosingPlateLyingDto.id_turn = id_turn;

            if (aTurn != null)
            {
                result = new
                {
                    message = "",
                    openingClosingPlateLyingDto.turn,
                    openingClosingPlateLyingDto.timeInitTurn,
                    openingClosingPlateLyingDto.timeEndTurn
                };
            }

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


        [HttpPost, ValidateInput(false)]
        public JsonResult GetWarehouseCartoningProcess(int idTunel)
        {
            object result;
            try
            {
                var maquina = db.MachineForProd.FirstOrDefault(e => e.id == idTunel);
                var warehouseCarton = maquina.Person.Provider?.ProviderRawMaterial?.Warehouse2;
                var warehouseLocationCarton = maquina.Person.Provider?.ProviderRawMaterial?.WarehouseLocation2;

                result = new
                {
                    id_warehouseCarton = warehouseCarton?.id,
                    name_warehouseCarton = warehouseCarton?.name,
                    id_warehouseLocationCarton = warehouseLocationCarton?.id,
                    name_warehouseLocationCarton = warehouseLocationCarton?.name,
                    message = "OK",
                };
            }
            catch (Exception e)
            {
                result = new
                {
                    message = e.Message,
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}