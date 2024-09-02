using DevExpress.Data.ODataLinq.Helpers;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class ClosePlateTunnelCoolController : DefaultController
    {
        private const string m_TipoDocumentoClosePlateTunnelCool = "138";

        private ClosePlateTunnelCoolDTO GetClosePlateTunnelCoolDTO()
        {
            if (!(Session["ClosePlateTunnelCoolDTO"] is ClosePlateTunnelCoolDTO closePlateTunnelCool))
                closePlateTunnelCool = new ClosePlateTunnelCoolDTO();
            return closePlateTunnelCool;
        }

        private List<ClosePlateTunnelCoolResultConsultDTO> GetClosePlateTunnelCoolResultConsultDTO()
        {
            if (!(Session["ClosePlateTunnelCoolResultConsultDTO"] is List<ClosePlateTunnelCoolResultConsultDTO> closePlateTunnelCoolResultConsult))
                closePlateTunnelCoolResultConsult = new List<ClosePlateTunnelCoolResultConsultDTO>();
            return closePlateTunnelCoolResultConsult;
        }

        private void SetClosePlateTunnelCoolDTO(ClosePlateTunnelCoolDTO closePlateTunnelCoolDTO)
        {
            Session["ClosePlateTunnelCoolDTO"] = closePlateTunnelCoolDTO;
        }

        private void SetClosePlateTunnelCoolResultConsultDTO(List<ClosePlateTunnelCoolResultConsultDTO> closePlateTunnelCoolResultConsult)
        {
            Session["ClosePlateTunnelCoolResultConsultDTO"] = closePlateTunnelCoolResultConsult;
        }

        public ActionResult Index()
        {
            BuildViewDataIndex();
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult(ClosePlateTunnelCoolConsultDTO consult)
        {
            var result = GetListsConsultDto(consult);
            SetClosePlateTunnelCoolResultConsultDTO(result);
            return PartialView("ConsultResult", result);
        }

        [HttpPost]
        public ActionResult GridViewClosePlateTunnelCool()
        {
            return PartialView("_GridViewIndex", GetClosePlateTunnelCoolResultConsultDTO());
        }

        private List<ClosePlateTunnelCoolResultConsultDTO> GetListsConsultDto(ClosePlateTunnelCoolConsultDTO consulta)
        {
            using (var db = new DBContext())
            {
                var consultaAux = Session["consulta"] as ClosePlateTunnelCoolConsultDTO;
                if (consultaAux != null && consulta.initDate == null)
                {
                    consulta = consultaAux;
                }

                var consultResult = db.ClosePlateTunnelCool.Where(ClosePlateTunnelCoolQueryExtensions.GetRequestByFilter(consulta)).ToList();
                if (!String.IsNullOrEmpty(consulta.numberLot))
                {
                    consultResult = consultResult.Where(w => (db.InventoryMovePlantTransfer.FirstOrDefault(fod1 => fod1.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.LiquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.internalNumber.Contains(consulta.numberLot)) != null) != null)).ToList();
                }
                if ((consulta.id_provider != null && consulta.id_provider != 0))
                {
                    consultResult = consultResult.Where(w => (db.InventoryMovePlantTransfer.FirstOrDefault(fod1 => fod1.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.LiquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.id_provider == consulta.id_provider) != null) != null)).ToList();
                }
                if ((consulta.id_productionUnitProvider != null && consulta.id_productionUnitProvider != 0))
                {
                    consultResult = consultResult.Where(w => (db.InventoryMovePlantTransfer.FirstOrDefault(fod1 => fod1.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.LiquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.id_productionUnitProvider == consulta.id_productionUnitProvider) != null) != null)).ToList();
                }
                if (consulta.items != null && consulta.items.Length > 0)
                {
                    var tempModel = new List<ClosePlateTunnelCool>();
                    foreach (var closePlateTunnelCool in consultResult)
                    {
                        var details = closePlateTunnelCool.MachineProdOpeningDetail.InventoryMovePlantTransfer.Where(d => d.InventoryMove.Document.DocumentState.code != "05" &&
                                                                                                                          d.InventoryMovePlantTransferDetail.FirstOrDefault(fod1 => consulta.items.Contains(fod1.LiquidationCartOnCartDetail.id_ItemToWarehouse)) != null);
                        if (details.Any())
                        {
                            tempModel.Add(closePlateTunnelCool);
                        }
                    }

                    consultResult = tempModel;
                }
                if (consulta.customers != null && consulta.customers.Length > 0)
                {
                    var tempModel = new List<ClosePlateTunnelCool>();
                    foreach (var closePlateTunnelCool in consultResult)
                    {
                        var details = closePlateTunnelCool.MachineProdOpeningDetail.InventoryMovePlantTransfer.Where(d => d.InventoryMove.Document.DocumentState.code != "05" &&
                                                                                                                          d.InventoryMovePlantTransferDetail.FirstOrDefault(fod1 => consulta.customers.Contains(fod1.LiquidationCartOnCartDetail.id_Client != null ? fod1.LiquidationCartOnCartDetail.id_Client.Value
                                                                                                                                                                                                                                                                   : -1)) != null);
                        if (details.Any())
                        {
                            tempModel.Add(closePlateTunnelCool);
                        }
                    }

                    consultResult = tempModel;
                }
                var query = consultResult.Select(t => new ClosePlateTunnelCoolResultConsultDTO
                {
                    id = t.id,
                    number = t.Document.number,
                    closeDate = t.closeDate,
                    closeTime = t.closeTime,
                    startEmissionDate = t.MachineProdOpeningDetail.MachineProdOpening.Document.emissionDate,
                    id_machineForProd = t.MachineProdOpeningDetail.id_MachineForProd,
                    machineForProd = t.MachineProdOpeningDetail.MachineForProd.name,
                    plantProcess = t.MachineProdOpeningDetail.MachineForProd.Person.processPlant,
                    turn = t.MachineProdOpeningDetail.MachineProdOpening.Turn.name,
                    state = t.Document.DocumentState.name,

                    canEdit = t.Document.DocumentState.code.Equals("01"),
                    canAproved = t.Document.DocumentState.code.Equals("01"),
                    canAnnul = t.Document.DocumentState.code.Equals("01"),
                    canReverse = t.Document.DocumentState.code.Equals("03")
                }).OrderBy(ob => ob.number).ToList();

                var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                if (entityObjectPermissions != null)
                {
                    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                    if (entityPermissions != null)
                    {
                        var tempModel = new List<ClosePlateTunnelCoolResultConsultDTO>();
                        foreach (var item in query)
                        {
                            if (entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) != null)
                            {
                                tempModel.Add(item);
                            }
                        }
                        query = tempModel;
                    }
                }

                Session["consulta"] = consulta;

                return query;
            }
        }

        private static List<ClosePlateTunnelCoolPendingNewDTO> GetClosePlateTunnelCoolPendingNewDto()

        {
            using (var db = new DBContext())
            {
                // Ejecución de la consulta de pendientes
                var q = db.MachineProdOpeningDetail
                    .Where(r => r.MachineProdOpening.Document.DocumentState.code.Equals("03") && //03: APROBADA
                                (r.MachineForProd.tbsysTypeMachineForProd.code.Equals("FRE") ||
                                 r.MachineForProd.tbsysTypeMachineForProd.code.Equals("PLA") ||
                                 r.MachineForProd.tbsysTypeMachineForProd.code.Equals("TUN")))
                    .Select(r => new ClosePlateTunnelCoolPendingNewDTO
                    {
                        id_MachineProdOpeningDetail = r.id,
                        numberMachineProdOpening = r.MachineProdOpening.Document.number,
                        plantProcess = r.MachineForProd.Person.processPlant,
                        machineForProd = r.MachineForProd.name,
                        emissionDate = r.MachineProdOpening.Document.emissionDate,
                        id_turn = r.MachineProdOpening.id_Turn,
                        turn = r.MachineProdOpening.Turn.name,
                        timeInit = r.timeInit,
                        timeEnd = r.timeEnd,
                        state = r.MachineProdOpening.Document.DocumentState.name
                    }).ToList();

                return q;
            }
        }

        [HttpPost]
        public ActionResult PendingNew()
        {
            return View(GetClosePlateTunnelCoolPendingNewDto());
        }

        [ValidateInput(false)]
        public ActionResult GridViewPendingNew()
        {
            return PartialView("_GridViewPendingNew", GetClosePlateTunnelCoolPendingNewDto());
        }

        private ClosePlateTunnelCoolDTO Create(int id_MachineProdOpeningDetail)
        {
            using (var db = new DBContext())
            {
                var machineProdOpeningDetail =
                    db.MachineProdOpeningDetail.FirstOrDefault(r => r.id == id_MachineProdOpeningDetail);
                if (machineProdOpeningDetail == null)
                    return new ClosePlateTunnelCoolDTO();

                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoClosePlateTunnelCool));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var hoy = DateTime.Now;

                var closePlateTunnelCoolDTO = new ClosePlateTunnelCoolDTO
                {
                    id = 0,
                    id_machineProdOpeningDetail = id_MachineProdOpeningDetail,
                    description = "",
                    number = "",//GetDocumentNumber(documentType?.id ?? 0),
                    documentType = documentType?.name ?? "",
                    id_documentType = documentType?.id ?? 0,
                    reference = "",
                    dateTimeEmision = machineProdOpeningDetail.MachineProdOpening.Document.emissionDate,
                    dateTimeEmisionStr = machineProdOpeningDetail.MachineProdOpening.Document.emissionDate.ToString("dd-MM-yyyy"),
                    idSate = documentState?.id ?? 0,
                    state = documentState?.name ?? "",
                    id_machineForProd = machineProdOpeningDetail.id_MachineForProd,
                    machineForProd = machineProdOpeningDetail.MachineForProd.name,
                    id_turn = machineProdOpeningDetail.MachineProdOpening.id_Turn,
                    turn = machineProdOpeningDetail.MachineProdOpening.Turn.name,
                    noOfPerson = machineProdOpeningDetail.numPerson.Value,
                    idPerson = machineProdOpeningDetail.id_Person,
                    person = machineProdOpeningDetail.Person.fullname_businessName,
                    closeDate = hoy,
                    closeTime = hoy.TimeOfDay,
                    ClosePlateTunnelCoolDetails = new List<ClosePlateTunnelCoolDetailDTO>()
                };

                FillClosePlateTunnelCoolDetails(closePlateTunnelCoolDTO, machineProdOpeningDetail);

                return closePlateTunnelCoolDTO;
            }
        }

        private void FillClosePlateTunnelCoolDetails(ClosePlateTunnelCoolDTO closePlateTunnelCoolDTO, MachineProdOpeningDetail machineProdOpeningDetail)
        {
            var inventoryMovePlantTransfers = db.InventoryMovePlantTransfer
                    .Where(r => r.id_machineProdOpeningDetail == machineProdOpeningDetail.id &&
                                r.InventoryMove.Document.DocumentState.code != "05")
                    .ToList();

            foreach (var inventoryMovePlantTransferN in inventoryMovePlantTransfers)
            {
                foreach (var inventoryMoveDetailN in inventoryMovePlantTransferN.InventoryMovePlantTransferDetail)
                {
                    var liquidationCartOnCartDetailAux = inventoryMoveDetailN.LiquidationCartOnCartDetail;//inventoryMovePlantTransferN.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.LiquidationCartOnCartDetail.id_ItemToWarehouse == inventoryMoveDetailN.id_item).LiquidationCartOnCartDetail;
                    var liquidationCartOnCartAux = liquidationCartOnCartDetailAux.LiquidationCartOnCart;
                    var pt = db.ProcessType.FirstOrDefault(fod => fod.id == liquidationCartOnCartAux.idProccesType);
                    var id_inventaryMoveTransferAutomaticEntry = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == inventoryMovePlantTransferN.id &&
                                                                   fod.Document.DocumentType.code.Equals("136"))?.id_document;//136: Ingreso Por Transferencia Automática Por Recepción Placa
                    var inventaryMoveTransferAutomaticEntryAux = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveTransferAutomaticEntry);

                    bool isCopackingLot = liquidationCartOnCartDetailAux.LiquidationCartOnCart.ProductionLot.isCopackingLot ?? false;

                    var id_MachineForProdCogellingFresh = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == inventoryMovePlantTransferN.id_machineProdOpeningDetail)?.id_MachineForProd;
                    var machineForProdCogellingFresh = db.MachineForProd.FirstOrDefault(fod => fod.id == id_MachineForProdCogellingFresh);

                    var id_warehouseEntryAux = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdWarehouse : machineForProdCogellingFresh?.id_materialWarehouse);
                    var id_warehouseLocationEntryAux = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdWarehouseLocation : machineForProdCogellingFresh?.id_materialWarehouseLocation);

                    var closePlateTunnelCoolDetailDTO = new ClosePlateTunnelCoolDetailDTO
                    {
                        id_inventoryMoveDetail = inventoryMoveDetailN.id,
                        warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseEntryAux)?.name,//inventoryMoveDetailN.Warehouse1.name,
                        warehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationEntryAux)?.name,//inventoryMoveDetailN.WarehouseLocation1.name,
                        productionCart = liquidationCartOnCartDetailAux.ProductionCart.name,
                        size = inventoryMoveDetailN.LiquidationCartOnCartDetail.Item1.ItemGeneral.ItemSize.name,
                        numberInventoryMoveEntry = inventaryMoveTransferAutomaticEntryAux?.Document.number,
                        dateTimeEmisionStr = inventaryMoveTransferAutomaticEntryAux?.Document.emissionDate.ToString("dd-MM-yyyy"),
                        cod_state = inventaryMoveTransferAutomaticEntryAux?.Document.DocumentState.code,
                        state = inventaryMoveTransferAutomaticEntryAux?.Document.DocumentState.name,
                        nameItem = inventoryMoveDetailN.LiquidationCartOnCartDetail.Item1.name,
                        tail = pt.code.Equals("COL") ? inventoryMoveDetailN.boxesToReceive ?? 0.00M : 0.00M,
                        whole = pt.code.Equals("COL") ? 0.00M : inventoryMoveDetailN.boxesToReceive ?? 0.00M,
                        total = inventoryMoveDetailN.boxesToReceive ?? 0.00M,
                        numberLot = liquidationCartOnCartAux.ProductionLot.internalNumber,
                        plantProcess = liquidationCartOnCartAux.MachineForProd.Person.processPlant,
                        customer = liquidationCartOnCartDetailAux.Person?.fullname_businessName ?? "SIN CLIENTE",
                        provider = liquidationCartOnCartAux.ProductionLot.Provider.Person.fullname_businessName,
                        nameProviderShrimp = liquidationCartOnCartAux.ProductionLot.ProductionUnitProvider.name,
                        productionUnitProviderPool = liquidationCartOnCartAux.ProductionLot.ProductionUnitProviderPool.name,
                        machineForProd = liquidationCartOnCartAux.MachineForProd.name,
                        numberLiquidationCarOnCar = liquidationCartOnCartAux.Document.number
                    };

                    closePlateTunnelCoolDTO.ClosePlateTunnelCoolDetails.Add(closePlateTunnelCoolDetailDTO);
                }
            }
        }

        private ClosePlateTunnelCoolDTO ConvertToDto(ClosePlateTunnelCool closePlateTunnelCool)
        {
            var closePlateTunnelCoolDto = new ClosePlateTunnelCoolDTO
            {
                id = closePlateTunnelCool.id,
                id_machineProdOpeningDetail = closePlateTunnelCool.id_MachineProdOpeningDetail,
                description = closePlateTunnelCool.Document.description,
                id_documentType = closePlateTunnelCool.Document.id_documentType,
                number = closePlateTunnelCool.Document.number,
                state = closePlateTunnelCool.Document.DocumentState.name,
                documentType = closePlateTunnelCool.Document.DocumentType.name,
                dateTimeEmision = closePlateTunnelCool.Document.emissionDate,//.ToString("d"),
                dateTimeEmisionStr = closePlateTunnelCool.Document.emissionDate.ToString("dd-MM-yyyy"),
                idSate = closePlateTunnelCool.Document.id_documentState,
                reference = closePlateTunnelCool.Document.reference,
                id_machineForProd = closePlateTunnelCool.MachineProdOpeningDetail.id_MachineForProd,
                machineForProd = closePlateTunnelCool.MachineProdOpeningDetail.MachineForProd.name,
                id_turn = closePlateTunnelCool.MachineProdOpeningDetail.MachineProdOpening.id_Turn,
                turn = closePlateTunnelCool.MachineProdOpeningDetail.MachineProdOpening.Turn.name,
                noOfPerson = (int)closePlateTunnelCool.MachineProdOpeningDetail.numPerson,
                idPerson = closePlateTunnelCool.MachineProdOpeningDetail.id_Person,
                person = closePlateTunnelCool.MachineProdOpeningDetail.Person.fullname_businessName,
                closeDate = closePlateTunnelCool.closeDate,
                closeTime = closePlateTunnelCool.closeTime,
                ClosePlateTunnelCoolDetails = new List<ClosePlateTunnelCoolDetailDTO>()
            };

            FillClosePlateTunnelCoolDetails(closePlateTunnelCoolDto, closePlateTunnelCool.MachineProdOpeningDetail);

            return closePlateTunnelCoolDto;
        }

        private void BuildViewDataIndex()
        {
            BuildComboBoxState();
            BuildComboBoxTurnIndex();
            BuildComboBoxMachineForProdIndex();
            BuildComboBoxPersonIndex();
            BuildComboBoxProviderIndex();
            BuildComboBoxProductionUnitProviderIndex();
            BuildTokenBoxItemsIndex();
            BuildTokenBoxCustomersIndex();
        }

        private void BuildViewDataEdit()
        {
        }

        [HttpPost]
        public ActionResult Edit(int id = 0, int id_MachineProdOpeningDetail = 0, bool enabled = true)
        {
            var model = new ClosePlateTunnelCoolDTO();
            ClosePlateTunnelCool closePlateTunnelCool = db.ClosePlateTunnelCool.FirstOrDefault(d => d.id == id);

            model = closePlateTunnelCool == null
                ? Create(id_MachineProdOpeningDetail)
                : ConvertToDto(closePlateTunnelCool);

            SetTransferDetailsDTO(model);
            SetClosePlateTunnelCoolDTO(model);
            BuilViewBag(enabled);

            return PartialView(model);
        }

        #region Manejadores de cuadrículas de detalles de transferencia

        private void SetTransferDetailsDTO(ClosePlateTunnelCoolDTO model)
        {
            var idDocumentType = db.DocumentType.FirstOrDefault(e => e.code == "37")?.id;
            var idDocumentState = db.DocumentState.FirstOrDefault(e => e.code == "03")?.id;
            var idsDocuments = db.Document.Where(e => e.id_documentType == idDocumentType &&
                    e.id_documentState == idDocumentState && DbFunctions.TruncateTime(e.emissionDate) == model.dateTimeEmision.Date)
                .Select(e => e.id).ToArray();

            model.TransferDetailEntry = GetTransferEntryDetailsDTO(
                idsDocuments, model.id_turn, model.id_machineForProd);
            model.TransferDetailExit = GetTransferExitDetailsDTO(
                idsDocuments, model.id_turn, model.id_machineForProd);

            #region Resumen de transferencia
            var transferSummary = new List<OpeningClosingPlateLyingTransferSummaryDTO>();

            var summariesExit = model.TransferDetailExit
                .GroupBy(e => new { e.machineForProdDestiny, e.machineForProdOrigin })
                .Select(e => new OpeningClosingPlateLyingTransferSummaryDTO() 
                {
                    transferType = "Salida de Inventario",
                    machineForProdDestiny = e.Key.machineForProdDestiny,
                    machineForProdOrigin = e.Key.machineForProdOrigin,
                    tail = e.Sum(x => x.tail) * -1, // Todas las salidas de inventarios son negativas
                    whole = e.Sum(x => x.whole) * -1, // Todas las salidas de inventarios son negativas
                    total = e.Sum(x => x.total) * -1, // Todas las salidas de inventarios son negativas
                })
                .ToList();
            transferSummary.AddRange(summariesExit);

            var summariesEntry = model.TransferDetailEntry
                .GroupBy(e => new { e.machineForProdDestiny, e.machineForProdOrigin })
                .Select(e => new OpeningClosingPlateLyingTransferSummaryDTO() 
                {
                    transferType = "Entradas de Inventario",
                    machineForProdDestiny = e.Key.machineForProdDestiny,
                    machineForProdOrigin = e.Key.machineForProdOrigin,
                    tail = e.Sum(x => x.tail),
                    whole = e.Sum(x => x.whole),
                    total = e.Sum(x => x.total),
                })
                .ToList();
            transferSummary.AddRange(summariesEntry);

            model.TransferSummary = transferSummary.OrderBy(e => e.transferType).ToArray();
            #endregion
        }
        private OpeningClosingPlateLyingTransferDetailDTO[] GetTransferEntryDetailsDTO(
            int[] idsDocuments, int id_turn, int id_MachineForProdDestination)
        {
            var openingClosingPlateLyings = db.OpeningClosingPlateLying
                .Where(e => idsDocuments.Contains(e.id) &&
                            e.id_freezerMachineForProdDestination == id_MachineForProdDestination &&
                            e.id_turn == id_turn && e.tunnelTransferPlate)
                .ToList();

            var retorno = new List<OpeningClosingPlateLyingTransferDetailDTO>();
            foreach (var openingClosingPlateLying in openingClosingPlateLyings)
            {
                var detalles = db.OpeningClosingPlateLyingDetail
                    .Where(e => e.id_openingClosingPlateLying == openingClosingPlateLying.id)
                    .ToList();

                foreach (var detail in detalles)
                {
                    var liquidationCartOnCart = detail.Lot.ProductionLot.LiquidationCartOnCart.FirstOrDefault();
                    string dateTimeEmisionStr, cod_state, state;
                    if (liquidationCartOnCart == null)
                    {
                        dateTimeEmisionStr = openingClosingPlateLying.Document.emissionDate.ToString("dd-MM-yyyy");
                        cod_state = openingClosingPlateLying.Document.DocumentState.code;
                        state = openingClosingPlateLying.Document.DocumentState.name;
                    }
                    else
                    {
                        dateTimeEmisionStr = liquidationCartOnCart.Document.emissionDate.ToString("dd-MM-yyyy");
                        cod_state = liquidationCartOnCart.Document.DocumentState.code;
                        state = liquidationCartOnCart.Document.DocumentState.name;
                    }

                    retorno.Add(new OpeningClosingPlateLyingTransferDetailDTO()
                    {
                        warehouse = detail.Warehouse.name,
                        warehouseLocation = detail.WarehouseLocation.name,
                        OpeningClosingPlateLying = openingClosingPlateLying.Document.number,
                        dateTimeEmisionStr = dateTimeEmisionStr,
                        cod_state = cod_state,
                        state = state,
                        machineForProdOrigin = openingClosingPlateLying.MachineForProd.name,
                        machineForProdDestiny = openingClosingPlateLying.MachineForProd1.name,
                        productionCart = detail.ProductionCart?.name,
                        numberLot = detail.Lot.internalNumber,
                        nameItem = detail.Item.name,
                        size = detail.Item.ItemGeneral.ItemSize.name,

                        tail = detail.Item.ItemType.code == "COL" ? detail.amount : 0m,
                        whole = detail.Item.ItemType.code != "COL" ? detail.amount : 0m,
                        total = detail.amount,
                    });
                }
            }

            // Asignación de detalles de entrada en memoria temporal
            TempData["TransferEntryDetailsDTO"] = retorno.ToArray();
            TempData.Keep("TransferEntryDetailsDTO");

            // Retornamos los detalles
            return retorno
                .ToArray();
        }

        private OpeningClosingPlateLyingTransferDetailDTO[] GetTransferExitDetailsDTO(
            int[] idsDocuments, int id_turn, int id_MachineForProdOrigin)
        {
            var openingClosingPlateLyings = db.OpeningClosingPlateLying
                .Where(e => idsDocuments.Contains(e.id) &&
                            e.id_freezerMachineForProd == id_MachineForProdOrigin &&
                            e.id_turn == id_turn && e.tunnelTransferPlate)
                .ToList();

            var retorno = new List<OpeningClosingPlateLyingTransferDetailDTO>();
            foreach (var openingClosingPlateLying in openingClosingPlateLyings)
            {
                var detalles = db.OpeningClosingPlateLyingDetail
                    .Where(e => e.id_openingClosingPlateLying == openingClosingPlateLying.id)
                    .ToList();

                foreach (var detail in detalles)
                {
                    var liquidationCartOnCart = detail.Lot.ProductionLot.LiquidationCartOnCart.FirstOrDefault();
                    string dateTimeEmisionStr, cod_state, state;
                    if(liquidationCartOnCart == null)
                    {
                        dateTimeEmisionStr = openingClosingPlateLying.Document.emissionDate.ToString("dd-MM-yyyy");
                        cod_state = openingClosingPlateLying.Document.DocumentState.code;
                        state = openingClosingPlateLying.Document.DocumentState.name;
                    }
                    else
                    {
                        dateTimeEmisionStr = liquidationCartOnCart.Document.emissionDate.ToString("dd-MM-yyyy");
                        cod_state = liquidationCartOnCart.Document.DocumentState.code;
                        state = liquidationCartOnCart.Document.DocumentState.name;
                    }

                    retorno.Add(new OpeningClosingPlateLyingTransferDetailDTO()
                    {
                        warehouse = detail.Warehouse.name,
                        warehouseLocation = detail.WarehouseLocation.name,
                        OpeningClosingPlateLying = openingClosingPlateLying.Document.number,
                        dateTimeEmisionStr = dateTimeEmisionStr,
                        cod_state = cod_state,
                        state = state,
                        machineForProdOrigin = openingClosingPlateLying.MachineForProd.name,
                        machineForProdDestiny = openingClosingPlateLying.MachineForProd1.name,
                        productionCart = detail.ProductionCart?.name,
                        numberLot = detail.Lot.ProductionLot.internalNumber,
                        nameItem = detail.Item.name,
                        size = detail.Item.ItemGeneral.ItemSize.name,

                        tail = detail.Item.ItemType.code == "COL" ? detail.amount : 0m,
                        whole = detail.Item.ItemType.code != "COL" ? detail.amount : 0m,
                        total = detail.amount,
                    });
                }
            }

            // Asignación de detalles de entrada en memoria temporal
            TempData["TransferExitDetailsDTO"] = retorno.ToArray();
            TempData.Keep("TransferExitDetailsDTO");

            // Retornamos los detalles
            return retorno
                .ToArray();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TransferDetailEntryGridView()
        {
            // Recuperamos los detalles de entrada
            var transferDetailEntry = (TempData["TransferEntryDetailsDTO"] as OpeningClosingPlateLyingTransferDetailDTO[]);

            // Guardamos los detalles de entrada
            TempData["TransferEntryDetailsDTO"] = transferDetailEntry;
            TempData.Keep("TransferEntryDetailsDTO");

            return PartialView("_GridViewTransferEntryDetails", transferDetailEntry);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TransferDetailExitGridView()
        {
            // Recuperamos los detalles de entrada
            var transferDetailEntry = (TempData["TransferExitDetailsDTO"] as OpeningClosingPlateLyingTransferDetailDTO[]);

            // Guardamos los detalles de entrada
            TempData["TransferExitDetailsDTO"] = transferDetailEntry;
            TempData.Keep("TransferExitDetailsDTO");

            return PartialView("_GridViewTransferExitDetails", transferDetailEntry);
        }
        #endregion


        private void BuilViewBag(bool enabled)
        {
            var closePlateTunnelCoolDTO = GetClosePlateTunnelCoolDTO();
            ViewBag.enabled = enabled;
            ViewBag.canNew = closePlateTunnelCoolDTO.id != 0;
            ViewBag.canEdit = !enabled &&
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == closePlateTunnelCoolDTO.idSate)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == closePlateTunnelCoolDTO.idSate)
                                     ?.code.Equals("01") ?? false) && closePlateTunnelCoolDTO.id != 0;
            ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == closePlateTunnelCoolDTO.idSate)
                                     ?.code.Equals("03") ?? false) && !enabled;
            ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == closePlateTunnelCoolDTO.idSate)
                                      ?.code.Equals("01") ?? false) && closePlateTunnelCoolDTO.id != 0;
            ViewBag.CanViewTransfer = closePlateTunnelCoolDTO.TransferDetailEntry.Any() || closePlateTunnelCoolDTO.TransferDetailExit.Any();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewDetails(bool? enabled)
        {
            var closePlateTunnelCoolDetails = GetClosePlateTunnelCoolDTO().ClosePlateTunnelCoolDetails;

            ViewBag.enabled = enabled;

            return PartialView("_GridViewDetails", closePlateTunnelCoolDetails);
        }

        #region Combobox

        private void BuildComboBoxState()
        {
            ViewData["Estados"] = db.DocumentState
                .Where(e => e.isActive
                    && e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == m_TipoDocumentoClosePlateTunnelCool))
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

        private void BuildComboBoxTurnIndex()
        {
            ViewData["TurnIndex"] = db.Turn.Where(e => e.isActive)
               .Select(s => new SelectListItem
               {
                   Text = s.name,
                   Value = s.id.ToString(),
               }).ToList();
        }

        public ActionResult ComboBoxTurnIndex()
        {
            BuildComboBoxTurnIndex();
            return PartialView("_ComboBoxTurnIndex");
        }

        private void BuildComboBoxMachineForProdIndex()
        {
            var machineForProds = (DataProviderMachineForProd.MachineForProds((EntityObjectPermissions)ViewData["entityObjectPermissions"]) as List<MachineForProd>);
            machineForProds = machineForProds.Where(e => e.tbsysTypeMachineForProd.code.Equals("FRE") || e.tbsysTypeMachineForProd.code.Equals("PLA") || e.tbsysTypeMachineForProd.code.Equals("TUN")).ToList();
            ViewData["MachineForProdIndex"] = machineForProds
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxMachineForProdIndex()
        {
            BuildComboBoxMachineForProdIndex();
            return PartialView("_ComboBoxMachineForProdIndex");
        }

        private void BuildComboBoxPersonIndex()
        {
            ViewData["PersonIndex"] = (DataProviderPerson.RolsByCompany((int?)ViewData["id_company"], "Supervisor") as List<Person>)
                .Select(s => new SelectListItem
                {
                    Text = s.fullname_businessName,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxPersonIndex()
        {
            BuildComboBoxPersonIndex();
            return PartialView("_ComboBoxPersonIndex");
        }

        private void BuildComboBoxProviderIndex()
        {
            ViewData["ProviderIndex"] = new List<SelectListItem>();
            var providerIndexList = db.Provider.Where(g => (g.Person.isActive && g.Person.id_company == ActiveCompany.id)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
            //(DataProviderPerson.ProviderByCompany((int?)ViewData["id_company"]) as List<Person>);
            if (providerIndexList != null)
            {
                ViewData["ProviderIndex"] = providerIndexList
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
            }
        }

        public ActionResult ComboBoxProviderIndex()
        {
            BuildComboBoxProviderIndex();
            return PartialView("_ComboBoxProviderIndex");
        }

        private void BuildComboBoxProductionUnitProviderIndex(int? id_provider = null)
        {
            ViewData["ProductionUnitProviderIndex"] = new List<SelectListItem>();
            var productionUnitProviderIndexList = db.ProductionUnitProvider.Where(t => t.isActive && (t.id_provider == id_provider || id_provider == null)).Select(s => new
            {
                s.id,
                name = s.name
            }).OrderBy(t => t.id).ToList();
            //var productionUnitProviderIndexList = (DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, id_provider) as List<ProductionUnitProvider>);
            if (productionUnitProviderIndexList != null)
            {
                ViewData["ProductionUnitProviderIndex"] = productionUnitProviderIndexList
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
            }
        }

        public ActionResult ComboBoxProductionUnitProviderIndex(int? id_provider)
        {
            BuildComboBoxProductionUnitProviderIndex(id_provider);
            return PartialView("_ComboBoxProductionUnitProviderIndex");
        }

        private void BuildTokenBoxItemsIndex()
        {
            var codeMaster = db.Setting.FirstOrDefault(fod => fod.code == "PMASTER")?.value ?? "";
            ViewData["ItemsIndex"] = db.Item
                .Where(w => (w.InventoryLine.code.Equals("PP") || w.InventoryLine.code.Equals("PT"))
                        && (w.ItemType != null) && (w.ItemType.ProcessType != null)
                        && (w.Presentation != null)
                        && (w.Presentation.code.Substring(0, 1) != codeMaster)
                        && w.isActive)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();

            //ViewData["ItemsIndex"] = (DataProviderItem.ItemsByCompanyAndInventoryLine(ActiveCompany.id, "PT") as List<Item>)
            //    .Select(s => new SelectListItem
            //    {
            //        Text = s.name,
            //        Value = s.id.ToString()
            //    }).ToList();
        }

        public ActionResult TokenBoxItemsIndex()
        {
            BuildTokenBoxItemsIndex();
            return PartialView("_TokenBoxItemsIndex");
        }

        private void BuildTokenBoxCustomersIndex()
        {
            ViewData["CustomersIndex"] = db.Person.Where(p => (p.ForeignCustomer != null || p.Rol.Any(a => a.name == "Cliente Local")) &&
                                                               p.isActive &&
                                                               p.id_company == ActiveCompany.id)
                .Select(s => new SelectListItem
                {
                    Text = s.fullname_businessName,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult TokenBoxCustomersIndex()
        {
            BuildTokenBoxCustomersIndex();
            return PartialView("_TokenBoxCustomersIndex");
        }

        #endregion Combobox

        [HttpPost]
        public JsonResult Save(string jsonClosePlateTunnelCool)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var closePlateTunnelCoolDTO = GetClosePlateTunnelCoolDTO();

                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == closePlateTunnelCoolDTO.id_machineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para editar y guardar el cierre de tunel, placa, fresco para la máquina: " + closePlateTunnelCoolDTO.machineForProd);
                                }
                            }
                        }

                        #endregion Validación Permiso

                        foreach (var item in closePlateTunnelCoolDTO.ClosePlateTunnelCoolDetails)
                        {
                            if (item.cod_state == "01")//01: Pendiente
                            {
                                throw new Exception("No se puede guardar el cierre de tunel, placa, fresco, con detalle en estado Pendiente");
                            }
                        }

                        JToken token = JsonConvert.DeserializeObject<JToken>(jsonClosePlateTunnelCool);

                        var newObject = false;
                        var id = token.Value<int>("id");

                        var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoClosePlateTunnelCool));
                        var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                        var closePlateTunnelCool = db.ClosePlateTunnelCool.FirstOrDefault(d => d.id == id);
                        if (closePlateTunnelCool == null)
                        {
                            newObject = true;

                            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
                            if (id_emissionPoint == 0)
                                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

                            closePlateTunnelCool = new ClosePlateTunnelCool
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
                            closePlateTunnelCool.id_MachineProdOpeningDetail = closePlateTunnelCoolDTO.id_machineProdOpeningDetail;
                            closePlateTunnelCool.Document.emissionDate = closePlateTunnelCoolDTO.dateTimeEmision;//token.Value<DateTime>("dateTimeEmision");
                            closePlateTunnelCool.id_userCreate = closePlateTunnelCool.Document.id_userCreate;
                            closePlateTunnelCool.id_userUpdate = closePlateTunnelCool.Document.id_userCreate;

                            documentType.currentNumber++;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;
                        }

                        closePlateTunnelCool.Document.id_documentState = documentState.id;
                        closePlateTunnelCool.Document.id_userUpdate = ActiveUser.id;
                        closePlateTunnelCool.id_userUpdate = closePlateTunnelCool.Document.id_userUpdate;
                        closePlateTunnelCool.Document.dateUpdate = DateTime.Now;
                        closePlateTunnelCool.Document.description = token.Value<string>("description");
                        closePlateTunnelCool.closeDate = token.Value<DateTime>("closeDateTime");
                        closePlateTunnelCool.closeTime = token.Value<DateTime>("closeDateTime").TimeOfDay;

                        if (newObject)
                        {
                            db.ClosePlateTunnelCool.Add(closePlateTunnelCool);
                            db.Entry(closePlateTunnelCool).State = EntityState.Added;

                            var closurePendingState = db.DocumentState.FirstOrDefault(d => d.code.Equals("20"));//20: PENDIENTE DE CIERRE
                            if (closurePendingState != null)
                            {
                                var machineProdOpeningDetail = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == closePlateTunnelCoolDTO.id_machineProdOpeningDetail);
                                machineProdOpeningDetail.timeEnd = closePlateTunnelCool.closeTime;
                                db.MachineProdOpeningDetail.Attach(machineProdOpeningDetail);
                                db.Entry(machineProdOpeningDetail).State = EntityState.Modified;
                                machineProdOpeningDetail.MachineProdOpening.Document.id_documentState = closurePendingState.id;
                                db.MachineProdOpening.Attach(machineProdOpeningDetail.MachineProdOpening);
                                db.Entry(machineProdOpeningDetail.MachineProdOpening).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            db.ClosePlateTunnelCool.Attach(closePlateTunnelCool);
                            db.Entry(closePlateTunnelCool).State = EntityState.Modified;
                            var machineProdOpeningDetail = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == closePlateTunnelCoolDTO.id_machineProdOpeningDetail);
                            machineProdOpeningDetail.timeEnd = closePlateTunnelCool.closeTime;
                            db.MachineProdOpeningDetail.Attach(machineProdOpeningDetail);
                            db.Entry(machineProdOpeningDetail).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        trans.Commit();

                        result.Data = closePlateTunnelCool.id.ToString();
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

        [HttpPost]
        public JsonResult Approve(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Data = ApproveClosePlateTunnelCool(id).name;
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

        private DocumentState ApproveClosePlateTunnelCool(int id_closePlateTunnelCool)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var closePlateTunnelCool = db.ClosePlateTunnelCool.FirstOrDefault(p => p.id == id_closePlateTunnelCool);
                    if (closePlateTunnelCool != null)
                    {
                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == closePlateTunnelCool.MachineProdOpeningDetail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para aprobar el cierre de tunel, placa, fresco para la máquina:" + closePlateTunnelCool.MachineProdOpeningDetail.MachineForProd.name);
                                }
                            }
                        }

                        #endregion Validación Permiso

                        var closureState = db.DocumentState.FirstOrDefault(d => d.code.Equals("04"));//04: CERRADA
                        if (closureState != null)
                        {
                            var machineProdOpeningDetail = closePlateTunnelCool.MachineProdOpeningDetail;
                            machineProdOpeningDetail.MachineProdOpening.Document.id_documentState = closureState.id;
                            db.MachineProdOpening.Attach(machineProdOpeningDetail.MachineProdOpening);
                            db.Entry(machineProdOpeningDetail.MachineProdOpening).State = EntityState.Modified;
                        }

                        var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                        if (aprovedState == null)
                            return null;

                        closePlateTunnelCool.Document.id_documentState = aprovedState.id;
                        closePlateTunnelCool.Document.authorizationDate = DateTime.Now;

                        db.ClosePlateTunnelCool.Attach(closePlateTunnelCool);
                        db.Entry(closePlateTunnelCool).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return closePlateTunnelCool.Document.DocumentState;
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
                        result.Data = ReverseClosePlateTunnelCool(id).name;
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

        private DocumentState ReverseClosePlateTunnelCool(int id_closePlateTunnelCool)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var closePlateTunnelCool = db.ClosePlateTunnelCool.FirstOrDefault(p => p.id == id_closePlateTunnelCool);
                    if (closePlateTunnelCool != null)
                    {
                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == closePlateTunnelCool.MachineProdOpeningDetail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para reversar el cierre de tunel, placa, fresco para la máquina:" + closePlateTunnelCool.MachineProdOpeningDetail.MachineForProd.name);
                                }
                            }
                        }

                        #endregion Validación Permiso

                        var closurePendingState = db.DocumentState.FirstOrDefault(d => d.code.Equals("20"));//20: PENDIENTE DE CIERRE
                        if (closurePendingState != null)
                        {
                            var machineProdOpeningDetail = closePlateTunnelCool.MachineProdOpeningDetail;
                            machineProdOpeningDetail.MachineProdOpening.Document.id_documentState = closurePendingState.id;
                            db.MachineProdOpening.Attach(machineProdOpeningDetail.MachineProdOpening);
                            db.Entry(machineProdOpeningDetail.MachineProdOpening).State = EntityState.Modified;
                        }

                        var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
                        if (reverseState == null)
                            return

                        closePlateTunnelCool.Document.DocumentState;
                        closePlateTunnelCool.Document.id_documentState = reverseState.id;
                        closePlateTunnelCool.Document.authorizationDate = DateTime.Now;

                        db.ClosePlateTunnelCool.Attach(closePlateTunnelCool);
                        db.Entry(closePlateTunnelCool).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return closePlateTunnelCool.Document.DocumentState;
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
                        result.Data = AnnulClosePlateTunnelCool(id).name;
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

        private DocumentState AnnulClosePlateTunnelCool(int id_closePlateTunnelCool)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var closePlateTunnelCool = db.ClosePlateTunnelCool.FirstOrDefault(p => p.id == id_closePlateTunnelCool);
                    if (closePlateTunnelCool != null)
                    {
                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == closePlateTunnelCool.MachineProdOpeningDetail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para anular el cierre de tunel, placa, fresco para la máquina:" + closePlateTunnelCool.MachineProdOpeningDetail.MachineForProd.name);
                                }
                            }
                        }

                        #endregion Validación Permiso

                        var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));//03: APROBADA
                        if (aprovedState != null)
                        {
                            var machineProdOpeningDetail = closePlateTunnelCool.MachineProdOpeningDetail;

                            machineProdOpeningDetail.timeEnd = machineProdOpeningDetail.timeInit;
                            db.MachineProdOpeningDetail.Attach(machineProdOpeningDetail);
                            db.Entry(machineProdOpeningDetail).State = EntityState.Modified;
                            machineProdOpeningDetail.MachineProdOpening.Document.id_documentState = aprovedState.id;
                            db.MachineProdOpening.Attach(machineProdOpeningDetail.MachineProdOpening);
                            db.Entry(machineProdOpeningDetail.MachineProdOpening).State = EntityState.Modified;
                        }

                        var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        if (annulState == null)
                            return

                        closePlateTunnelCool.Document.DocumentState;
                        closePlateTunnelCool.Document.id_documentState = annulState.id;
                        closePlateTunnelCool.Document.authorizationDate = DateTime.Now;

                        db.ClosePlateTunnelCool.Attach(closePlateTunnelCool);
                        db.Entry(closePlateTunnelCool).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return closePlateTunnelCool.Document.DocumentState;
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id)
        {
            var index = GetClosePlateTunnelCoolResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

            var result = new
            {
                maximunPages = GetClosePlateTunnelCoolResultConsultDTO().Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            var element = GetClosePlateTunnelCoolResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
            var closePlateTunnelCool = db.ClosePlateTunnelCool.FirstOrDefault(d => d.id == element.id);
            if (closePlateTunnelCool == null)
                return PartialView("Edit", new ClosePlateTunnelCoolDTO());

            BuildViewDataEdit();
            var model = ConvertToDto(closePlateTunnelCool);
            SetClosePlateTunnelCoolDTO(model);
            BuilViewBag(false);

            return PartialView("Edit", model);
        }

        #region Reporteria

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReport(int id_closePlateTunnelCool, string processType, string codeReport)
        {
            List<ParamCR> paramLst = new List<ParamCR>();

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ParamCR _param = null;

            _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id_closePlateTunnelCool
            };

            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@processType",
                Valor = processType
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

        #endregion Reporteria
    }
}