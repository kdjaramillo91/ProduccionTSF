using DevExpress.Data.ODataLinq.Helpers;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System.Data.SqlClient;
using System.Data;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using DXPANACEASOFT.Models.InventoryMoveDTO.InventoryMovePlantTransfer;
using Utilitarios.ProdException;
using DXPANACEASOFT.Models.Dto;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryMovePlantTransferController : DefaultController
    {
        private const string m_TipoDocumentoInventoryMovePlantTransfer = "135";

        private InventoryMovePlantTransferDTO GetInventoryMovePlantTransferDTO()
        {
            if (!(Session["InventoryMovePlantTransferDTO"] is InventoryMovePlantTransferDTO inventoryMovePlantTransfer))
                inventoryMovePlantTransfer = new InventoryMovePlantTransferDTO();
            return inventoryMovePlantTransfer;
        }

        private List<InventoryMovePlantTransferResultConsultDTO> GetInventoryMovePlantTransferResultConsultDTO()
        {
            if (!(Session["InventoryMovePlantTransferResultConsultDTO"] is List<InventoryMovePlantTransferResultConsultDTO> inventoryMovePlantTransferResultConsult))
                inventoryMovePlantTransferResultConsult = new List<InventoryMovePlantTransferResultConsultDTO>();
            return inventoryMovePlantTransferResultConsult;
        }

        private void SetInventoryMovePlantTransferDTO(InventoryMovePlantTransferDTO inventoryMovePlantTransferDTO)
        {
            Session["InventoryMovePlantTransferDTO"] = inventoryMovePlantTransferDTO;
        }

        private void SetInventoryMovePlantTransferResultConsultDTO(List<InventoryMovePlantTransferResultConsultDTO> inventoryMovePlantTransferResultConsult)
        {
            Session["InventoryMovePlantTransferResultConsultDTO"] = inventoryMovePlantTransferResultConsult;
        }

        // GET: InventoryMovePlantTransfer
        public ActionResult Index()
        {
            BuildViewDataIndex();
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult(InventoryMovePlantTransferConsultDTO consult)
        {
            var result = GetListsConsultDto(consult);
            SetInventoryMovePlantTransferResultConsultDTO(result);
            return PartialView("ConsultResult", result);
        }

        [HttpPost]
        public ActionResult GridViewInventoryMovePlantTransfer()
        {
            return PartialView("_GridViewIndex", GetInventoryMovePlantTransferResultConsultDTO());
        }

        private List<InventoryMovePlantTransferResultConsultDTO> GetListsConsultDto(InventoryMovePlantTransferConsultDTO consulta)
        {
            using (var db = new DBContext())
            {
                var consultaAux = Session["consulta"] as InventoryMovePlantTransferConsultDTO;
                if (consultaAux != null && consulta.initDate == null)
                {
                    consulta = consultaAux;
                }

                var fechaInicioEmision = string.IsNullOrEmpty(consulta.initDate) ? (DateTime?)null : DateTime.Parse(consulta.initDate);
                var fechaFinEmision = string.IsNullOrEmpty(consulta.endtDate) ? (DateTime?)null : DateTime.Parse(consulta.endtDate);

                Parametros.ParametrosBusquedaInventoryMovePlantTransfer parametrosBusquedaInventoryMovePlantTransfer = new Parametros.ParametrosBusquedaInventoryMovePlantTransfer();
                parametrosBusquedaInventoryMovePlantTransfer.id_machineForProd = consulta.id_machineForProd;
                parametrosBusquedaInventoryMovePlantTransfer.number = consulta.number;
                parametrosBusquedaInventoryMovePlantTransfer.reference = consulta.reference;
                parametrosBusquedaInventoryMovePlantTransfer.startEmissionDate = fechaInicioEmision;
                parametrosBusquedaInventoryMovePlantTransfer.endEmissionDate = fechaFinEmision;
                parametrosBusquedaInventoryMovePlantTransfer.id_state = consulta.id_state;
                parametrosBusquedaInventoryMovePlantTransfer.id_productionCart = consulta.id_productionCart;
                parametrosBusquedaInventoryMovePlantTransfer.id_processType = consulta.id_processType;
                parametrosBusquedaInventoryMovePlantTransfer.id_provider = consulta.id_provider;
                parametrosBusquedaInventoryMovePlantTransfer.id_warehouseEntry = consulta.id_warehouseEntry;
                parametrosBusquedaInventoryMovePlantTransfer.id_warehouseLocationEntry = consulta.id_warehouseLocationEntry;
                parametrosBusquedaInventoryMovePlantTransfer.id_receiver = consulta.id_receiver;
                parametrosBusquedaInventoryMovePlantTransfer.numberLot = consulta.numberLot;
                parametrosBusquedaInventoryMovePlantTransfer.secTransaction = consulta.secTransaction;
                parametrosBusquedaInventoryMovePlantTransfer.id_turn = consulta.id_turn;
                parametrosBusquedaInventoryMovePlantTransfer.id_inventoryReason = consulta.id_inventoryReason;

                var parametrosBusquedaInventoryMovePlantTransferAux = new SqlParameter();
                parametrosBusquedaInventoryMovePlantTransferAux.ParameterName = "@ParametrosBusquedaInventoryMovePlantTransfer";
                parametrosBusquedaInventoryMovePlantTransferAux.Direction = ParameterDirection.Input;
                parametrosBusquedaInventoryMovePlantTransferAux.SqlDbType = SqlDbType.NVarChar;
                var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaInventoryMovePlantTransfer);
                parametrosBusquedaInventoryMovePlantTransferAux.Value = jsonAux;
                db.Database.CommandTimeout = 1200;
                List<InventoryMovePlantTransferResultConsultDTO> query = db.Database.SqlQuery<InventoryMovePlantTransferResultConsultDTO>("exec inv_Consultar_InventoryMovePlantTransfer_StoredProcedure @ParametrosBusquedaInventoryMovePlantTransfer ", parametrosBusquedaInventoryMovePlantTransferAux).ToList();

                //var consultResult = db.InventoryMovePlantTransfer.Where(InventoryMovePlantTransferQueryExtensions.GetRequestByFilter(consulta));
                //if (!String.IsNullOrEmpty(consulta.number))
                //{
                //    consultResult = consultResult.Where(w => w.id_inventoryMoveEntry != null ? w.InventoryMove1.natureSequential.Contains(consulta.number) : false);
                //    //consultResult = consultResult.Where(w => (db.InventoryMove.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == w.id &&
                //    //                                                                                                                            fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                            ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == w.id &&
                //    //                                                                                                                                                                fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                            : 0))) != null
                //    //                                                                                                                            ? (db.InventoryMove.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == w.id &&
                //    //                                                                                                                             fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                             ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == w.id &&
                //    //                                                                                                                                                                 fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                             : 0))).natureSequential.Contains(consulta.number)
                //    //                                                                                                                             : false);
                //}
                //if ((consulta.id_inventoryReason != null && consulta.id_inventoryReason != 0))
                //{
                //    consultResult = consultResult.Where(w => w.id_inventoryMoveEntry != null ? w.InventoryMove1.id_inventoryReason == consulta.id_inventoryReason : false);
                //    //consultResult = consultResult.Where(w => (db.InventoryMove.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == w.id &&
                //    //                                                                                                                            fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                            ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == w.id &&
                //    //                                                                                                                                                                fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                            : 0))) != null
                //    //                                                                                                                            ? (db.InventoryMove.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == w.id &&
                //    //                                                                                                                             fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                             ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == w.id &&
                //    //                                                                                                                                                                 fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                             : 0))).id_inventoryReason == consulta.id_inventoryReason
                //    //                                                                                                                             : false);
                //}

                //var inventoryReason =
                //   db.InventoryReason.FirstOrDefault(r => r.code == "IPTAPRP");

                //var query = consultResult.Select(t => new InventoryMovePlantTransferResultConsultDTO
                //{
                //    id = t.id,
                //    number = t.id_inventoryMoveEntry != null ? t.InventoryMove1.natureSequential: "",
                //    //number = (db.Document.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                            fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                            ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                                                                fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                            : 0))) != null
                //    //                                                                                                                            ? (db.Document.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                             fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                             ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                                                                 fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                             : 0))).number
                //    //                                                                                                                             : "",
                //    //number = t.InventoryMove.natureSequential,//t.Document.number,
                //    id_warehouse = (t.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.ProductionLot.isCopackingLot != null ? t.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.ProductionLot.isCopackingLot.Value
                //                                                                                                                                  : false) ? t.MachineProdOpeningDetail.MachineForProd.id_materialthirdWarehouse
                //                                                                                                                                           : t.MachineProdOpeningDetail.MachineForProd.id_materialWarehouse,
                //    warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == ((t.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.ProductionLot.isCopackingLot != null ? t.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.ProductionLot.isCopackingLot.Value
                //                                                                                                                                  : false) ? t.MachineProdOpeningDetail.MachineForProd.id_materialthirdWarehouse
                //                                                                                                                                           : t.MachineProdOpeningDetail.MachineForProd.id_materialWarehouse)).name,
                //    id_warehouseLocation = (t.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.ProductionLot.isCopackingLot != null ? t.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.ProductionLot.isCopackingLot.Value
                //                                                                                                                                  : false) ? t.MachineProdOpeningDetail.MachineForProd.id_materialthirdWarehouseLocation
                //                                                                                                                                           : t.MachineProdOpeningDetail.MachineForProd.id_materialWarehouseLocation,
                //    warehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == ((t.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.ProductionLot.isCopackingLot != null ? t.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.ProductionLot.isCopackingLot.Value
                //                                                                                                                                  : false) ? t.MachineProdOpeningDetail.MachineForProd.id_materialthirdWarehouseLocation
                //                                                                                                                                           : t.MachineProdOpeningDetail.MachineForProd.id_materialWarehouseLocation)).name,
                //    emissionDate = t.InventoryMove.Document.emissionDate,
                //    id_inventoryReason = t.id_inventoryMoveEntry != null ? t.InventoryMove1.id_inventoryReason : inventoryReason.id,
                //    //id_inventoryReason = (db.InventoryMove.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                            fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                            ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                                                                fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                            : 0))) != null
                //    //                                                                                                                            ? (db.InventoryMove.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                             fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                             ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                                                                 fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                             : 0))).id_inventoryReason
                //    //                                                                                                                             : inventoryReason.id,
                //    inventoryReason = t.id_inventoryMoveEntry != null ? t.InventoryMove1.InventoryReason.name : inventoryReason.name,
                //    //inventoryReason = (db.InventoryMove.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                            fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                            ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                                                                fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                            : 0))) != null
                //    //                                                                                                                            ? (db.InventoryMove.FirstOrDefault(fod => fod.id == (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                             fod2.Document.DocumentType.code.Equals("136")) != null
                //    //                                                                                                                             ? (db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == t.id &&
                //    //                                                                                                                                                                 fod2.Document.DocumentType.code.Equals("136")).id_document)
                //    //                                                                                                                             : 0))).InventoryReason.name
                //    //                                                                                                                             : inventoryReason.name,
                //    id_receiver = t.InventoryMove != null && t.InventoryMove.InventoryEntryMove != null ? t.InventoryMove.InventoryEntryMove.id_receiver : (int?)null,
                //    receiver = t.InventoryMove != null && t.InventoryMove.InventoryEntryMove != null ? t.InventoryMove.InventoryEntryMove.Employee.Person.fullname_businessName
                //                                                                                     : "",
                //    state = t.InventoryMove.Document.DocumentState.name,
                //    id_machineForProdEntry = t.MachineProdOpeningDetail.id_MachineForProd,
                //    id_machineForProdExit = t.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.id_MachineForProd,


                //    canEdit = t.InventoryMove.Document.DocumentState.code.Equals("01"),
                //    canAproved = t.InventoryMove.Document.DocumentState.code.Equals("01"),
                //    canAnnul = t.InventoryMove.Document.DocumentState.code.Equals("01"),
                //    canReverse = t.InventoryMove.Document.DocumentState.code.Equals("03")

                //})./*OrderBy(ob => ob.number).*/ToList();
                //query = query.Where(w => consulta.number == null || consulta.number == "" || w.number.Contains(consulta.number)).OrderBy(ob => ob.number).ToList();

                var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                if (entityObjectPermissions != null)
                {
                    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                    if (entityPermissions != null)
                    {
                        var tempModel = new List<InventoryMovePlantTransferResultConsultDTO>();
                        //foreach (var item in query)
                        //{
                        //    if (entityPermissions.listValue.FirstOrDefault(fod2 => (fod2.id_entityValue == item.id_machineForProdEntry || fod2.id_entityValue == item.id_machineForProdExit) && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) != null)
                        //    {
                        //        tempModel.Add(item);
                        //    }
                        //}
                        query = tempModel;
                    }
                    entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                    if (entityPermissions != null)
                    {
                        var tempModel = new List<InventoryMovePlantTransferResultConsultDTO>();
                        foreach (var item in query)
                        {
                            if (entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null) != null)
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

        private static List<InventoryMovePlantTransferPendingNewDTO> GetInventoryMovePlantTransferPendingNewDto()
        {
            using (var db = new DBContext())
            {
                //Parametros.ParametrosBusquedaMastered parametrosBusquedaMastered = new Parametros.ParametrosBusquedaMastered();
                //parametrosBusquedaMastered.id_warehouse = id_boxedWarehouse;
                //parametrosBusquedaMastered.id_warehouseLocation = id_boxedWarehouseLocation;
                //parametrosBusquedaMastered.id_warehouseType = db.WarehouseType.FirstOrDefault(t => t.code.Equals("BE01"))?.id;//BE01:Tipo Bodega: Bodega de Encartonado
                //parametrosBusquedaMastered.for_lot = 0;

                //var parametrosBusquedaMasteredAux = new SqlParameter();
                //parametrosBusquedaMasteredAux.ParameterName = "@ParametrosBusquedaSaldoMastered";
                //parametrosBusquedaMasteredAux.Direction = ParameterDirection.Input;
                //parametrosBusquedaMasteredAux.SqlDbType = SqlDbType.NVarChar;
                //var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaMastered);
                //parametrosBusquedaMasteredAux.Value = jsonAux;
                db.Database.CommandTimeout = 1200;
                List<InventoryMovePlantTransferPendingNewDTO> q = db.Database.SqlQuery<InventoryMovePlantTransferPendingNewDTO>("exec inv_Consultar_InventoryMovePlantTransferPendingNew_StoredProcedure").ToList();
                // Ejecución de la consulta de pendientes
                //var q = db.LiquidationCartOnCartDetail
                //    .Where(r => (//r.LiquidationCartOnCart.Document.DocumentState.code != ("05") && //05: ANULADA y
                //                 r.LiquidationCartOnCart.Document.DocumentState.code == ("01")) &&//01: PENDIENTE
                //                 (r.boxesReceived == null || (r.boxesReceived != null && r.boxesReceived < r.quatityBoxesIL)) &&
                //                 r.SubProcessIOProductionProcess != null && r.SubProcessIOProductionProcess.ProductionProcess.code.Equals("CNG"))//CONGELAMIENTO
                //                                                                                      //(r.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
                //                                                                                      //													  fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") == null)) //01: PENDIENTE y 05: APROBADO
                //                                                                                      //&& (db.InventoryMovePlantTransfer.FirstOrDefault(fod => (DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(fod.Document.emissionDate)) &&
                //                                                                                      //                                             (r.MachineProdOpening.id_Turn == fod.id_turn) &&
                //                                                                                      //                                             (r.id_MachineForProd == fod.id_machineForProd) &&
                //                                                                                      //                                             (fod.Document.DocumentState.code != "05")) == null)) //05: ANULADA
                //    .AsEnumerable()
                //    .Select(r => new InventoryMovePlantTransferPendingNewDTO
                //    {
                //        id_liquidationCartOnCartDetail = r.id,
                //        numberLiquidationCartOnCart = r.LiquidationCartOnCart.Document.number,
                //        emissionDateStr = r.LiquidationCartOnCart.Document.emissionDate.ToString("dd-MM-yyyy"),
                //        emissionDate = r.LiquidationCartOnCart.Document.emissionDate,
                //        id_machineForProd = r.LiquidationCartOnCart.id_MachineForProd,
                //        machineForProd = r.LiquidationCartOnCart.MachineForProd.name,
                //        id_productionCart = r.id_ProductionCart.Value,
                //        productionCart = r.ProductionCart.name,
                //        turn = r.LiquidationCartOnCart.MachineProdOpening.Turn.name,
                //        id_processType = r.LiquidationCartOnCart.idProccesType.Value,
                //        processType = db.ProcessType.FirstOrDefault(fod => fod.id == r.LiquidationCartOnCart.idProccesType).code.Equals("ENT") ? "ENTERO" : "COLA",
                //        numberLot = r.LiquidationCartOnCart.ProductionLot.internalNumber,
                //        processPlant = r.LiquidationCartOnCart.MachineForProd.Person.processPlant,
                //        provider = db.Provider.FirstOrDefault(fod => fod.id == r.LiquidationCartOnCart.ProductionLot.id_provider).Person.fullname_businessName,
                //        id_customer = r.id_Client,
                //        customer = (r.id_Client == null ? "SIN CLIENTE" : r.Person.fullname_businessName),
                //        itemWarehouse = r.Item1.name,
                //        box = r.quatityBoxesIL - (r.boxesReceived == null ? 0 : r.boxesReceived.Value)
                //    }).ToList();
                //.GroupBy(g => new
                //{
                //    emissionDate = DbFunctions.TruncateTime(g.MachineProdOpening.Document.emissionDate),
                //    id_turn = g.MachineProdOpening.id_Turn,
                //    nameTurn = g.MachineProdOpening.Turn.name,
                //    g.id_MachineForProd,
                //    machineForProd = g.MachineForProd.name,
                //    g.MachineForProd.Person.processPlant,
                //    g.Person.fullname_businessName,
                //    g.MachineProdOpening.Document.DocumentState.name,
                //    g.MachineProdOpening.Document.number

                //})
                //.AsEnumerable()
                //.Select(r => new InventoryMovePlantTransferPendingNewDTO
                //{
                //    numberMachineProdOpening = r.Key.number,
                //    emissionDate = r.Key.emissionDate.Value,
                //    emissionDateStr = r.Key.emissionDate.Value.ToString("dd-MM-yyyy"),
                //    id_turn = r.Key.id_turn,
                //    turn = r.Key.nameTurn,
                //    id_machineForProd = r.Key.id_MachineForProd,
                //    machineForProd = r.Key.machineForProd,
                //    processPlant = r.Key.processPlant,
                //    personRequire = r.Key.fullname_businessName,
                //    state = r.Key.name
                //}).ToList();

                return q;

            }
        }

        [HttpPost]
        public ActionResult PendingNew()
        {
            return View(GetInventoryMovePlantTransferPendingNewDto());
        }

        [ValidateInput(false)]
        public ActionResult GridViewPendingNew()
        {
            return PartialView("_GridViewPendingNew", GetInventoryMovePlantTransferPendingNewDto());
        }

        private InventoryMovePlantTransferDTO Create(int[] ids)// DateTime emissionDate, int id_turn, int id_machineForProd)
        {
            using (var db = new DBContext())
            {
                var ids0 = ids[0];
                var liquidationCartOnCartDetail =
                    db.LiquidationCartOnCartDetail.FirstOrDefault(r => r.id == ids0);
                var inventoryReason =
                    db.InventoryReason.FirstOrDefault(r => r.code == "IPTAPRP");
                //if (machineProdOpening == null)
                //	return new InventoryMovePlantTransferDTO();

                var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals("136"));
                var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                var hoy = DateTime.Now;

                var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
                var entryNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("I"));
                var inventoryMovePlantTransferDTO = new InventoryMovePlantTransferDTO
                {
                    id_documentType = documentType?.id ?? 0,
                    documentType = documentType?.name ?? "",
                    number = "",//GetDocumentNumber(documentType?.id ?? 0),
                    reference = "",
                    description = "",
                    idSate = documentState?.id ?? 0,
                    state = documentState?.name ?? "",
                    dateTimeEmision = hoy,//liquidationCartOnCartDetail.LiquidationCartOnCart.MachineProdOpening.Document.emissionDate,
                    dateTimeEmisionStr = hoy.ToString("dd-MM-yyyy"),//liquidationCartOnCartDetail.LiquidationCartOnCart.MachineProdOpening.Document.emissionDate.ToString("dd-MM-yyyy"),
                    id_machineForProdCartOnCart = liquidationCartOnCartDetail.LiquidationCartOnCart.id_MachineForProd,
                    machineForProdCartOnCart = liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.name,
                    numberLiquidationCartOnCart = liquidationCartOnCartDetail.LiquidationCartOnCart.Document.number,
                    //id_productionCart = liquidationCartOnCartDetail.id_ProductionCart,
                    //productionCart = liquidationCartOnCartDetail.ProductionCart.name,
                    processType = db.ProcessType.FirstOrDefault(fod => fod.id == liquidationCartOnCartDetail.LiquidationCartOnCart.idProccesType).code.Equals("ENT") ? "ENTERO" : "COLA",
                    processPlant = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.Person1.processPlant,
                    liquidator = liquidationCartOnCartDetail.LiquidationCartOnCart.Person?.fullname_businessName,
                    id_machineProdOpeningDetail = null,
                    id_machineForProdCogellingFresh = null,
                    machineForProdCogellingFresh = "",
                    id_turnCogellingFresh = null,
                    turnCogellingFresh = "",
                    timeInitTurn = null,
                    timeEndTurn = null,
                    dateTimeEntry = hoy,//liquidationCartOnCartDetail.LiquidationCartOnCart.MachineProdOpening.Document.emissionDate,
                    dateTimeEntryStr = hoy.ToString("dd-MM-yyyy"),//liquidationCartOnCartDetail.LiquidationCartOnCart.MachineProdOpening.Document.emissionDate.ToString("dd-MM-yyyy"),
                    id_natureMove = entryNatureMove.idAdvanceDetailModelP,
                    natureMove = entryNatureMove.nameAdvanceDetailModelP,
                    id_inventoryReason = inventoryReason?.id,
                    inventoryReason = inventoryReason?.name,
                    id_receiver = ActiveUser.id_employee,
                    receiver = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee)?.Person.fullname_businessName,
                    id_warehouseEntry = null,
                    warehouseEntry = "",
                    isCopackingLot = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.isCopackingLot ?? false,

                    InventoryMovePlantTransferDetails = new List<InventoryMovePlantTransferDetailDTO>()
                };

                FillInventoryMovePlantTransferDetails(inventoryMovePlantTransferDTO, ids);

                return inventoryMovePlantTransferDTO;
            }
        }
        private decimal Truncate2Decimals(decimal value)
        {
            return Decimal.Truncate(value * 100m) / 100m;
        }
        private void FillInventoryMovePlantTransferDetails(InventoryMovePlantTransferDTO inventoryMovePlantTransferDTO, int[] ids)
        {
            //var machineProdOpenings = db.MachineProdOpening
            //        .Where(r => (r.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code != "05") != null) //05: ANULADA
            //                    && (DbFunctions.TruncateTime(r.Document.emissionDate) == DbFunctions.TruncateTime(emissionDate))
            //                    && (r.id_Turn == id_turn))
            //        .ToList();

            foreach (var id in ids)
            {
                var liquidationCartOnCartDetail = db.LiquidationCartOnCartDetail.FirstOrDefault(fod => fod.id == id);
                var liquidationCartOnCart = db.LiquidationCartOnCart.FirstOrDefault(fod => fod.id == liquidationCartOnCartDetail.id_LiquidationCartOnCart);
                bool isCopackingLot = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.isCopackingLot ?? false;
                //foreach (var item in machineProdOpeningN.LiquidationCartOnCart.Where(w => w.MachineForProd.id_personProcessPlant == id_personProcessPlant).ToList())
                //{
                string nameLot = (liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot != null) ? ((liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.number ?? "") + ((liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.number != "" && liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.number != null && liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.internalNumber != "" && liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.internalNumber != "") ? "-" : "") + (liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.internalNumber ?? "")) : "";

                var id_warehouseExitAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdWarehouse : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialWarehouse);
                var id_warehouseLocationExitAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdWarehouseLocation : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialWarehouseLocation);
                var id_costCenterAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdCostCenter : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialCostCenter);
                var id_subCostCenterAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdSubCostCenter : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialSubCostCenter);
                var pendingBox = liquidationCartOnCartDetail.quatityBoxesIL - (liquidationCartOnCartDetail.boxesReceived ?? 0);

                inventoryMovePlantTransferDTO.InventoryMovePlantTransferDetails.Add(new InventoryMovePlantTransferDetailDTO
                {
                    id = inventoryMovePlantTransferDTO.InventoryMovePlantTransferDetails.Count() > 0 ? inventoryMovePlantTransferDTO.InventoryMovePlantTransferDetails.Max(pld => pld.id) + 1 : 1,
                    id_liquidationCartOnCartDetail = liquidationCartOnCartDetail.id,
                    id_liquidationCartOnCart = liquidationCartOnCartDetail.id_LiquidationCartOnCart,
                    id_inventoryMoveExit = null,
                    noInventoryMoveExit = "",
                    id_warehouseExit = id_warehouseExitAux,
                    warehouseExit = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseExitAux)?.name ?? "",
                    id_warehouseLocationExit = id_warehouseLocationExitAux,
                    warehouseLocationExit = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationExitAux)?.name ?? "",
                    id_warehouseEntry = null,
                    warehouseEntry = "",
                    id_warehouseLocationEntry = null,
                    warehouseLocationEntry = "",
                    id_productionCart = liquidationCartOnCartDetail.id_ProductionCart,
                    productionCart = liquidationCartOnCartDetail.ProductionCart.name,
                    id_item = liquidationCartOnCartDetail.id_ItemToWarehouse,
                    codItem = liquidationCartOnCartDetail.Item1.masterCode,
                    nameItem = liquidationCartOnCartDetail.Item1.name,
                    id_umMovExit = liquidationCartOnCartDetail.Item1.ItemInventory.id_metricUnitInventory,
                    umMovExit = liquidationCartOnCartDetail.Item1.ItemInventory.MetricUnit.code,
                    id_costCenter = id_costCenterAux,
                    costCenter = db.CostCenter.FirstOrDefault(fod => fod.id == id_costCenterAux)?.name ?? "",
                    id_subCostCenter = id_subCostCenterAux,
                    subCostCenter = db.CostCenter.FirstOrDefault(fod => fod.id == id_subCostCenterAux)?.name ?? "",
                    amountToEnter = pendingBox,
                    id_customer = liquidationCartOnCartDetail.id_Client,
                    customer = liquidationCartOnCartDetail.id_Client != null ? liquidationCartOnCartDetail.Person.fullname_businessName : "SIN CLIENTE",
                    id_umMov = liquidationCartOnCartDetail.Item1.ItemInventory.id_metricUnitInventory,
                    umMov = liquidationCartOnCartDetail.Item1.ItemInventory.MetricUnit.code,
                    cost = 0.00M,
                    total = 0.00M,
                    id_lot = liquidationCartOnCartDetail.LiquidationCartOnCart.id_ProductionLot,
                    lot = nameLot,
                    pending = pendingBox,
                    id_costCenterEntry = null,
                    costCenterEntry = "",
                    id_subCostCenterEntry = null,
                    subCostCenterEntry = "",
                    //quantityKgs = quantityKgsAux,
                    //quantityPounds = quantityPoundsAux
                    machineForProdCartOnCartDetail = liquidationCartOnCart.MachineForProd.name
                });
                //}
            }

        }

        private InventoryMovePlantTransferDTO ConvertToDto(InventoryMovePlantTransfer inventoryMovePlantTransfer)
        {
            var aLiquidationCartOnCart = inventoryMovePlantTransfer.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart;
            var aLiquidationCartOnCartDetail = inventoryMovePlantTransfer.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCartDetail;
            var inventoryReason =
                    db.InventoryReason.FirstOrDefault(r => r.code == "IPTAPRP");
            var lstNatureMove = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("NMMGI") as List<AdvanceParametersDetailModelP>;
            var entryNatureMove = lstNatureMove.FirstOrDefault(fod => fod.codeAdvanceDetailModelP.Trim().Equals("I"));
            var id_inventaryMoveTransferAutomaticEntry = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == inventoryMovePlantTransfer.id &&
                                                                   //(fod.Document.DocumentState.code.Equals("01") || fod.Document.DocumentState.code.Equals("03")) &&
                                                                   fod.Document.DocumentType.code.Equals("136"))?.id_document;//136: Ingreso Por Transferencia Automática Por Recepción Placa
            var inventaryMoveTransferAutomaticEntryAux = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveTransferAutomaticEntry);
            var aDocumentAux = db.Document.FirstOrDefault(fod => fod.id == id_inventaryMoveTransferAutomaticEntry);
            var inventoryMovePlantTransferDto = new InventoryMovePlantTransferDTO
            {
                id = inventoryMovePlantTransfer.id,
                id_documentType = inventaryMoveTransferAutomaticEntryAux?.Document.id_documentType ?? aDocumentAux.id_documentType,
                documentType = inventaryMoveTransferAutomaticEntryAux?.Document.DocumentType.name ?? aDocumentAux.DocumentType.name,
                number = inventaryMoveTransferAutomaticEntryAux?.natureSequential ?? aDocumentAux.number,
                reference = inventoryMovePlantTransfer.InventoryMove.Document.reference,
                description = inventoryMovePlantTransfer.InventoryMove.Document.description,
                idSate = inventoryMovePlantTransfer.InventoryMove.Document.id_documentState,
                state = inventoryMovePlantTransfer.InventoryMove.Document.DocumentState.name,
                dateTimeEmisionStr = inventoryMovePlantTransfer.InventoryMove.Document.emissionDate.ToString("dd-MM-yyyy"),
                dateTimeEmision = inventoryMovePlantTransfer.InventoryMove.Document.emissionDate,
                id_machineForProdCartOnCart = aLiquidationCartOnCart.id_MachineForProd,
                machineForProdCartOnCart = aLiquidationCartOnCart.MachineForProd.name,
                numberLiquidationCartOnCart = aLiquidationCartOnCart.Document.number,
                //id_productionCart = aLiquidationCartOnCartDetail?.id_ProductionCart,
                //productionCart = aLiquidationCartOnCartDetail?.ProductionCart.name,
                processType = db.ProcessType.FirstOrDefault(fod => fod.id == aLiquidationCartOnCart.idProccesType).code.Equals("ENT") ? "ENTERO" : "COLA",
                liquidator = aLiquidationCartOnCart.Person?.fullname_businessName,
                processPlant = aLiquidationCartOnCart.ProductionLot.Person1.processPlant,
                id_machineProdOpeningDetail = inventoryMovePlantTransfer.id_machineProdOpeningDetail,
                id_machineForProdCogellingFresh = inventoryMovePlantTransfer.MachineProdOpeningDetail.id_MachineForProd,
                machineForProdCogellingFresh = inventoryMovePlantTransfer.MachineProdOpeningDetail.MachineForProd.name,
                id_turnCogellingFresh = inventoryMovePlantTransfer.MachineProdOpeningDetail.MachineProdOpening.id_Turn,
                turnCogellingFresh = inventoryMovePlantTransfer.MachineProdOpeningDetail.MachineProdOpening.Turn.name,
                timeInitTurn = inventoryMovePlantTransfer.MachineProdOpeningDetail.MachineProdOpening.Turn.timeInit.Hours + ":" + inventoryMovePlantTransfer.MachineProdOpeningDetail.MachineProdOpening.Turn.timeInit.Minutes,
                timeEndTurn = inventoryMovePlantTransfer.MachineProdOpeningDetail.MachineProdOpening.Turn.timeEnd.Hours + ":" + inventoryMovePlantTransfer.MachineProdOpeningDetail.MachineProdOpening.Turn.timeEnd.Minutes,
                dateTimeEntryStr = inventoryMovePlantTransfer.dateTimeEntry.ToString("dd-MM-yyyy"),
                dateTimeEntry = inventoryMovePlantTransfer.dateTimeEntry,
                id_natureMove = entryNatureMove.idAdvanceDetailModelP,
                natureMove = entryNatureMove.nameAdvanceDetailModelP,
                id_inventoryReason = inventoryReason.id,
                inventoryReason = inventoryReason.name,
                id_receiver = inventoryMovePlantTransfer.InventoryMove.InventoryEntryMove.id_receiver,
                receiver = db.Employee.FirstOrDefault(fod => fod.id == inventoryMovePlantTransfer.InventoryMove.InventoryEntryMove.id_receiver)?.Person.fullname_businessName,
                id_warehouseEntry = inventoryMovePlantTransfer.InventoryMove.idWarehouseEntry,
                warehouseEntry = inventoryMovePlantTransfer.InventoryMove.Warehouse.name,
                isCopackingLot = aLiquidationCartOnCart.ProductionLot.isCopackingLot ?? false,
                InventoryMovePlantTransferDetails = new List<InventoryMovePlantTransferDetailDTO>()
            };

            //int cont = 0;
            foreach (var item in inventoryMovePlantTransfer.InventoryMovePlantTransferDetail)
            {
                //string numberAux = (item.Lot != null ? item.Lot.number : "");
                //string internalNumberAux = (item.Lot != null ? item.Lot.internalNumber : "");
                //internalNumberAux = (internalNumberAux != "" && internalNumberAux != null) ? internalNumberAux : (item.Lot.ProductionLot != null ? item.Lot.ProductionLot.internalNumber : "");
                //string nameLot = (numberAux + ((numberAux != "" && numberAux != null && internalNumberAux != "" && internalNumberAux != null) ? "-" : "") + internalNumberAux);

                //var inventoryMoveDetailEntryAux = inventaryMoveTransferAutomaticEntryAux.InventoryMoveDetail.FirstOrDefault(fod => fod.id_item == item.id_item);
                //decimal remainingQuantity = 0.0M;
                //if (inventoryMoveDetailEntryAux.InventoryMoveDetailTransfer1 != null && inventoryMoveDetailEntryAux.InventoryMoveDetailTransfer1.Count > 0)
                //{
                //	int? id_inventoryMoveDetailExit = inventoryMoveDetailEntryAux.InventoryMoveDetailTransfer1.First().id_inventoryMoveDetailExit;
                //	InventoryMoveDetail inventoryMoveDetail = DataProviderInventoryMove.InventoryMoveDetail(id_inventoryMoveDetailExit);

                //	var quantityMove = (inventoryMoveDetail != null && inventoryMoveDetail.amountMove != null) ? inventoryMoveDetail.amountMove.Value : 0;
                //	var quantityReceived = (inventoryMoveDetail != null &&
                //							inventoryMoveDetail.InventoryMoveDetailTransfer != null &&
                //							inventoryMoveDetail.InventoryMoveDetailTransfer.Where(w => w.InventoryMoveDetail1.InventoryMove.Document.DocumentState.code.Equals("03")).Count() > 0) ?
                //							inventoryMoveDetail.InventoryMoveDetailTransfer.Where(w => w.InventoryMoveDetail1.InventoryMove.Document.DocumentState.code.Equals("03")).Sum(s => s.quantity) :
                //							0;
                //	remainingQuantity = quantityMove - quantityReceived;
                //	remainingQuantity = remainingQuantity < 0 ? 0.0M : remainingQuantity;
                //}
                var dLiquidationCartOnCart = db.LiquidationCartOnCart.FirstOrDefault(fod => fod.id == item.LiquidationCartOnCartDetail.LiquidationCartOnCart.id);
                var liquidationCartOnCartDetail = item.LiquidationCartOnCartDetail;
                bool isCopackingLot = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.isCopackingLot ?? false;
                //foreach (var item in machineProdOpeningN.LiquidationCartOnCart.Where(w => w.MachineForProd.id_personProcessPlant == id_personProcessPlant).ToList())
                //{
                string nameLot = (liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot != null) ? ((liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.number ?? "") + ((liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.number != "" && liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.number != null && liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.internalNumber != "" && liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.internalNumber != "") ? "-" : "") + (liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.internalNumber ?? "")) : "";

                var id_warehouseExitAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdWarehouse : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialWarehouse);
                var id_warehouseLocationExitAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdWarehouseLocation : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialWarehouseLocation);
                var id_costCenterAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdCostCenter : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialCostCenter);
                var id_subCostCenterAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdSubCostCenter : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialSubCostCenter);

                var id_MachineForProdCogellingFresh = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == inventoryMovePlantTransfer.id_machineProdOpeningDetail)?.id_MachineForProd;
                var machineForProdCogellingFresh = db.MachineForProd.FirstOrDefault(fod => fod.id == id_MachineForProdCogellingFresh);

                var id_warehouseEntryAux = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdWarehouse : machineForProdCogellingFresh?.id_materialWarehouse);
                var id_warehouseLocationEntryAux = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdWarehouseLocation : machineForProdCogellingFresh?.id_materialWarehouseLocation);
                var id_costCenterEntryAux = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdCostCenter : machineForProdCogellingFresh?.id_materialCostCenter);
                var id_subCostCenterEntryAux = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdSubCostCenter : machineForProdCogellingFresh?.id_materialSubCostCenter);

                inventoryMovePlantTransferDto.InventoryMovePlantTransferDetails.Add(new InventoryMovePlantTransferDetailDTO
                {
                    id = item.id,
                    id_liquidationCartOnCartDetail = item.id_liquidationCartOnCartDetail,
                    id_liquidationCartOnCart = aLiquidationCartOnCart.id,
                    id_inventoryMoveExit = item.id_inventoryMovePlantTransfer,
                    noInventoryMoveExit = item.InventoryMovePlantTransfer.InventoryMove.natureSequential,//item.InventoryMoveDetailTransfer1.FirstOrDefault()?.InventoryMove?.Document.number ?? "",
                    id_warehouseExit = id_warehouseExitAux,//item.InventoryMoveDetailTransfer1.FirstOrDefault()?.id_warehouseExit,
                    warehouseExit = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseExitAux)?.name,//item.InventoryMoveDetailTransfer1.FirstOrDefault()?.Warehouse?.name ?? "",
                    id_warehouseLocationExit = id_warehouseLocationExitAux,//item.InventoryMoveDetailTransfer1.FirstOrDefault()?.id_warehouseLocationExit,
                    warehouseLocationExit = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationExitAux)?.name,//item.InventoryMoveDetailTransfer1.FirstOrDefault()?.WarehouseLocation?.name ?? "",
                    id_warehouseEntry = id_warehouseEntryAux,
                    warehouseEntry = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseEntryAux)?.name ?? "",
                    id_warehouseLocationEntry = id_warehouseLocationEntryAux,
                    warehouseLocationEntry = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationEntryAux)?.name ?? "",
                    id_productionCart = liquidationCartOnCartDetail.id_ProductionCart,
                    productionCart = liquidationCartOnCartDetail.ProductionCart.name,
                    id_item = liquidationCartOnCartDetail.id_ItemToWarehouse,
                    codItem = liquidationCartOnCartDetail.Item1.masterCode,
                    nameItem = liquidationCartOnCartDetail.Item1.name,
                    id_umMovExit = liquidationCartOnCartDetail.Item1.ItemInventory.id_metricUnitInventory,
                    umMovExit = liquidationCartOnCartDetail.Item1.ItemInventory.MetricUnit.code,
                    id_costCenter = id_costCenterAux,
                    costCenter = db.CostCenter.FirstOrDefault(fod => fod.id == id_costCenterAux)?.name ?? "",
                    id_subCostCenter = id_subCostCenterAux,
                    subCostCenter = db.CostCenter.FirstOrDefault(fod => fod.id == id_subCostCenterAux)?.name ?? "",
                    amountToEnter = item.boxesToReceive ?? 0,
                    id_customer = liquidationCartOnCartDetail.id_Client,
                    customer = liquidationCartOnCartDetail.id_Client != null ? liquidationCartOnCartDetail.Person.fullname_businessName : "SIN CLIENTE",
                    id_umMov = liquidationCartOnCartDetail.Item1.ItemInventory.id_metricUnitInventory,
                    umMov = liquidationCartOnCartDetail.Item1.ItemInventory.MetricUnit.code,
                    cost = 0.00M,
                    total = 0.00M,
                    id_lot = liquidationCartOnCartDetail.LiquidationCartOnCart.id_ProductionLot,
                    lot = nameLot,
                    pending = item.LiquidationCartOnCartDetail.quatityBoxesIL - (item.LiquidationCartOnCartDetail.boxesReceived ?? 0),
                    id_costCenterEntry = id_costCenterEntryAux,
                    costCenterEntry = db.CostCenter.FirstOrDefault(fod => fod.id == id_costCenterEntryAux)?.name ?? "",
                    id_subCostCenterEntry = id_subCostCenterEntryAux,
                    subCostCenterEntry = db.CostCenter.FirstOrDefault(fod => fod.id == id_subCostCenterEntryAux)?.name ?? "",
                    machineForProdCartOnCartDetail = dLiquidationCartOnCart.MachineForProd.name,
                });
            }

            return inventoryMovePlantTransferDto;
        }

        private void BuildViewDataIndex()
        {
            BuildComboBoxState();
            BuildComboBoxWarehouseEntryIndex();
            BuildComboBoxWarehouseLocationEntryIndex();
            BuildComboBoxInventoryReasonIndex();
            BuildComboBoxReceiverIndex();
            BuildComboBoxProcessTypeIndex();
            BuildComboBoxProviderIndex();
            BuildComboBoxTurnIndex();
            BuildComboBoxMachineForProdIndex();
            BuildComboBoxProductionCartIndex();
        }

        private void BuildViewDataEdit(DateTime? dateTimeEmision)
        {
            BuildComboBoxMachineProdOpeningDetailCogellingFresh(dateTimeEmision);
            BuildComboBoxReceiver();
        }

        [HttpPost]
        public ActionResult Edit(int id = 0, int[] ids = null, bool enabled = true)
        {

            var model = new InventoryMovePlantTransferDTO();
            InventoryMovePlantTransfer inventoryMovePlantTransfer = db.InventoryMovePlantTransfer.FirstOrDefault(d => d.id == id);
            if (inventoryMovePlantTransfer == null)
            {
                model = Create(ids);
                SetInventoryMovePlantTransferDTO(model);

                BuildViewDataEdit(model.dateTimeEmision);
                BuilViewBag(enabled);
                return PartialView(model);
            }

            model = ConvertToDto(inventoryMovePlantTransfer);
            SetInventoryMovePlantTransferDTO(model);
            BuildViewDataEdit(model.dateTimeEmision);

            BuilViewBag(enabled);

            return PartialView(model);
        }

        private void BuilViewBag(bool enabled)
        {
            var inventoryMovePlantTransferDTO = GetInventoryMovePlantTransferDTO();
            //int id_menu = (int)ViewData["id_menu"];
            var idMenu = db.Menu.FirstOrDefault(fod => fod.TController.name == "InventoryMovePlantTransfer").id;
            var tienePermisioConciliar = this.ActiveUser
                    .UserMenu.FirstOrDefault(e => e.id_menu == idMenu)?
                    .Permission?.FirstOrDefault(p => p.name == "Conciliar");

            ViewBag.enabled = enabled;
            ViewBag.canNew = inventoryMovePlantTransferDTO.id != 0;
            ViewBag.canEdit = !enabled &&
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == inventoryMovePlantTransferDTO.idSate)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == inventoryMovePlantTransferDTO.idSate)
                                     ?.code.Equals("01") ?? false) && inventoryMovePlantTransferDTO.id != 0;
            ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == inventoryMovePlantTransferDTO.idSate)
                                     ?.code.Equals("03") ?? false) && !enabled;
            ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == inventoryMovePlantTransferDTO.idSate)
                                      ?.code.Equals("01") ?? false) && inventoryMovePlantTransferDTO.id != 0;
            ViewBag.canConciliate = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == inventoryMovePlantTransferDTO.idSate)
                ?.code.Equals("03") ?? false) && !enabled && tienePermisioConciliar != null;

            var estado = db.DocumentState.FirstOrDefault(e => e.id == inventoryMovePlantTransferDTO.idSate);

            var estadosReverso = new[] { "03", "16" };
            var puedeReversar = db.DocumentState
                .Any(e => estadosReverso.Contains(e.code)
                    && e.id == inventoryMovePlantTransferDTO.idSate) && inventoryMovePlantTransferDTO.id != 0;

            ViewBag.canReverse = estado.code == "16"
                ? puedeReversar && !enabled && tienePermisioConciliar != null
                : puedeReversar && !enabled;

            ViewBag.dateTimeEmision = inventoryMovePlantTransferDTO.dateTimeEmision;
        }

        #region GridViewDetails

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewDetails(bool? enabled)
        {
            var inventoryMovePlantTransfer = GetInventoryMovePlantTransferDTO();

            ViewBag.enabled = enabled;
            ViewBag.dateTimeEmision = inventoryMovePlantTransfer.dateTimeEmision;

            var machineForProdCartOnCart = db.MachineForProd.FirstOrDefault(fod => fod.id == inventoryMovePlantTransfer.id_machineForProdCartOnCart);
            var id_MachineForProdCogellingFresh = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == inventoryMovePlantTransfer.id_machineProdOpeningDetail)?.id_MachineForProd;
            var machineForProdCogellingFresh = db.MachineForProd.FirstOrDefault(fod => fod.id == id_MachineForProdCogellingFresh);
            bool isCopackingLot = inventoryMovePlantTransfer.isCopackingLot;

            foreach (var item in inventoryMovePlantTransfer.InventoryMovePlantTransferDetails)
            {
                item.id_warehouseExit = (isCopackingLot ? machineForProdCartOnCart.id_materialthirdWarehouse : machineForProdCartOnCart.id_materialWarehouse);
                item.warehouseExit = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouseExit)?.name ?? "";
                item.id_warehouseLocationExit = (isCopackingLot ? machineForProdCartOnCart.id_materialthirdWarehouseLocation : machineForProdCartOnCart.id_materialWarehouseLocation);
                item.warehouseLocationExit = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_warehouseLocationExit)?.name ?? "";
                item.id_costCenter = (isCopackingLot ? machineForProdCartOnCart.id_materialthirdCostCenter : machineForProdCartOnCart.id_materialCostCenter);
                item.costCenter = db.CostCenter.FirstOrDefault(fod => fod.id == item.id_costCenter)?.name ?? "";
                item.id_subCostCenter = (isCopackingLot ? machineForProdCartOnCart.id_materialthirdSubCostCenter : machineForProdCartOnCart.id_materialSubCostCenter);
                item.subCostCenter = db.CostCenter.FirstOrDefault(fod => fod.id == item.id_subCostCenter)?.name ?? "";

                item.id_warehouseEntry = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdWarehouse : machineForProdCogellingFresh?.id_materialWarehouse);
                item.warehouseEntry = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouseEntry)?.name ?? "";
                item.id_warehouseLocationEntry = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdWarehouseLocation : machineForProdCogellingFresh?.id_materialWarehouseLocation);
                item.warehouseLocationEntry = db.WarehouseLocation.FirstOrDefault(fod => fod.id == item.id_warehouseLocationEntry)?.name ?? "";
                item.id_costCenterEntry = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdCostCenter : machineForProdCogellingFresh?.id_materialCostCenter);
                item.costCenterEntry = db.CostCenter.FirstOrDefault(fod => fod.id == item.id_costCenterEntry)?.name ?? "";
                item.id_subCostCenterEntry = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdSubCostCenter : machineForProdCogellingFresh?.id_materialSubCostCenter);
                item.subCostCenterEntry = db.CostCenter.FirstOrDefault(fod => fod.id == item.id_subCostCenterEntry)?.name ?? "";
            }

            return PartialView("_GridViewDetails", inventoryMovePlantTransfer.InventoryMovePlantTransferDetails);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewDetailsUpdate(InventoryMovePlantTransferDetailDTO item, bool? enabled)
        {
            var inventoryMovePlantTransfer = GetInventoryMovePlantTransferDTO();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = inventoryMovePlantTransfer.InventoryMovePlantTransferDetails.FirstOrDefault(it => it.id == item.id);

                    if (modelItem != null)
                    {
                        modelItem.amountToEnter = item.amountToEnter;
                        this.UpdateModel(modelItem);
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

            return PartialView("_GridViewDetails", inventoryMovePlantTransfer.InventoryMovePlantTransferDetails);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewDetailsDelete(System.Int32 id)
        {
            var inventoryMovePlantTransfer = GetInventoryMovePlantTransferDTO();

            try
            {
                var modelItem = inventoryMovePlantTransfer.InventoryMovePlantTransferDetails.FirstOrDefault(it => it.id == id);
                if (modelItem != null)
                    inventoryMovePlantTransfer.InventoryMovePlantTransferDetails.Remove(modelItem);

            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            return PartialView("_GridViewDetails", inventoryMovePlantTransfer.InventoryMovePlantTransferDetails);
        }

        #endregion

        #region Combobox
        private void BuildComboBoxState()
        {
            ViewData["Estados"] = db.DocumentState
                .Where(e => e.isActive
                    && e.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code == "136"))
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
            var machineForProds = (DataProviderMachineForProd.MachineForProds((EntityObjectPermissions)ViewData["entityObjectPermissions"]) as List<MachineForProd>)
                                  .Where(w => w.tbsysTypeMachineForProd.code.Equals("CLA"));
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

        private void BuildComboBoxWarehouseEntryIndex()
        {
            var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
            var model = db.Warehouse.Where(t => t.id_company == ActiveCompany.id && t.isActive &&
                                                (t.WarehouseType.code.Equals("BCO01") || t.WarehouseType.code.Equals("BF01"))).ToList();//BCO01:Tipo Bodega: Bodega de Congelación y BF01: Tipo Bodega: Bodega Productos Frescos

            if (entityObjectPermissions != null)
            {
                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                if (entityPermissions != null)
                {
                    var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                    model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
                }
            }
            ViewData["WarehouseEntryIndex"] = model
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxWarehouseEntryIndex()
        {
            BuildComboBoxWarehouseEntryIndex();
            return PartialView("_ComboBoxWarehouseEntryIndex");
        }

        private void BuildComboBoxWarehouseLocationEntryIndex(int? id_warehouseEntry = null)
        {
            ViewData["WarehouseLocationEntryIndex"] = db.WarehouseLocation.Where(t => t.id_warehouse == id_warehouseEntry && t.id_company == ActiveCompany.id && t.isActive).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxWarehouseLocationEntryIndex(int? id_warehouseEntry)
        {
            BuildComboBoxWarehouseLocationEntryIndex(id_warehouseEntry);
            return PartialView("_ComboBoxWarehouseLocationEntryIndex");
        }

        private void BuildComboBoxInventoryReasonIndex()
        {
            var id_documentType = db.DocumentType.FirstOrDefault(fod => fod.code == "136")?.id;
            ViewData["InventoryReasonIndex"] = db.InventoryReason.Where(g => g.isActive && g.id_documentType == id_documentType).Select(p => new { p.id, name = p.name }).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxInventoryReasonIndex()
        {
            BuildComboBoxInventoryReasonIndex();
            return PartialView("_ComboBoxInventoryReasonIndex");
        }

        private void BuildComboBoxReceiverIndex()
        {
            ViewData["ReceiverIndex"] = db.Employee.Where(g => g.Person.isActive).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxReceiverIndex()
        {
            BuildComboBoxReceiverIndex();
            return PartialView("_ComboBoxReceiverIndex");
        }

        private void BuildComboBoxProcessTypeIndex()
        {
            //var id_processType = db.ProcessType.FirstOrDefault(fod => fod.code == "ENT").id;
            ViewData["ProcessTypeIndex"] = db.ProcessType.Where(g => g.isActive && (g.code == "ENT" || g.code == "COL")).Select(p => new { p.id, p.name }).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxProcessTypeIndex()
        {
            BuildComboBoxProcessTypeIndex();
            return PartialView("_ComboBoxProcessTypeIndex");
        }

        private void BuildComboBoxProviderIndex()
        {
            ViewData["ProviderIndex"] = db.Provider.Where(g => g.Person.isActive).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxProviderIndex()
        {
            BuildComboBoxProviderIndex();
            return PartialView("_ComboBoxProviderIndex");
        }

        private void BuildComboBoxProductionCartIndex()
        {
            ViewData["ProductionCartIndex"] = db.ProductionCart.Where(e => e.isActive)
               .Select(s => new SelectListItem
               {
                   Text = s.name,
                   Value = s.id.ToString(),
               }).ToList();

        }

        public ActionResult ComboBoxProductionCartIndex()
        {
            BuildComboBoxProductionCartIndex();
            return PartialView("_ComboBoxProductionCartIndex");
        }

        private void BuildComboBoxMachineProdOpeningDetailCogellingFresh(DateTime? dateTimeEmision)
        {
            var inventoryMovePlantTransferDTO = GetInventoryMovePlantTransferDTO();
            ViewData["MachineProdOpeningDetailCogellingFresh"] = db.MachineProdOpeningDetail.Where(e => (e.MachineForProd.isActive && e.MachineProdOpening.Document.DocumentState.code.Equals("03") && e.MachineForProd.Person.processPlant == inventoryMovePlantTransferDTO.processPlant &&
                                                                                                        (DbFunctions.TruncateTime(e.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(dateTimeEmision == null ? e.MachineProdOpening.Document.emissionDate : dateTimeEmision)) &&
                                                                                                        (e.MachineForProd.tbsysTypeMachineForProd.code.Equals("FRE") || e.MachineForProd.tbsysTypeMachineForProd.code.Equals("PLA") || e.MachineForProd.tbsysTypeMachineForProd.code.Equals("TUN"))) || e.id == inventoryMovePlantTransferDTO.id_machineProdOpeningDetail)
               .Select(s => new SelectListItem
               {
                   Text = s.MachineForProd.name + " - " + s.MachineProdOpening.Turn.name + " - " + (s.MachineForProd.available ? "Disponible" : ("No Disponible - " + s.MachineForProd.reason)),
                   Value = s.id.ToString(),
               }).ToList();

        }

        public ActionResult ComboBoxMachineProdOpeningDetailCogellingFresh(DateTime? dateTimeEmision)
        {
            BuildComboBoxMachineProdOpeningDetailCogellingFresh(dateTimeEmision);
            ViewBag.enabled = true;
            return PartialView("_ComboBoxMachineProdOpeningDetailCogellingFresh");
        }

        private void BuildComboBoxReceiver()
        {
            var inventoryMovePlantTransferDTO = GetInventoryMovePlantTransferDTO();
            List<SelectListItem> aSelectListItems = new List<SelectListItem>();
            var aSelectListItem = new SelectListItem
            {
                Text = inventoryMovePlantTransferDTO.receiver,
                Value = inventoryMovePlantTransferDTO.id_receiver.ToString(),
                Selected = true
            };
            aSelectListItems.AddRange(db.Employee.Where(g => g.Person.isActive && g.id != inventoryMovePlantTransferDTO.id_receiver)//.Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.Person.fullname_businessName,
                    Value = s.id.ToString(),
                    Selected = false
                }).ToList());
            aSelectListItems.Insert(0, aSelectListItem);
            //var aSelectListItems = db.Employee.Where(g => g.Person.isActive && g.id != inventoryMovePlantTransferDTO.id_receiver)//.Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList()
            //    .Select(s => new SelectListItem
            //    {
            //        Text = s.Person.fullname_businessName,
            //        Value = s.id.ToString(),
            //        Selected = false
            //    }).ToList();

            ViewData["Receiver"] = aSelectListItems;
        }

        public ActionResult ComboBoxReceiver()
        {
            BuildComboBoxReceiver();
            ViewBag.enabled = true;
            return PartialView("_ComboBoxReceiver");
        }

        //[HttpPost]
        //public ActionResult ComboBoxMotivoLoteEdit(bool? stopCurrent)
        //{
        //    List<SelectListItem> model = new List<SelectListItem>();
        //    if (stopCurrent.Value)
        //    {
        //        model = db.productiveHoursReason.Where(w => w.isActive)
        //            .Select(s => new SelectListItem
        //            {
        //                Text = s.name,
        //                Value = s.id.ToString()
        //            }).ToList();
        //    }
        //    else
        //    {
        //        var inventoryMovePlantTransfer = GetInventoryMovePlantTransferDTO();

        //        //var model2 = db.LiquidationCartOnCart
        //        //    .Where(r => (r.Document.DocumentState.code != ("05")) &&//05: ANULADA
        //        //                                                            //(DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(inventoryMovePlantTransfer.dateTimeEmision)) &&
        //        //                (r.MachineProdOpening.id_Turn == inventoryMovePlantTransfer.id_turn) &&
        //        //                (r.id_MachineForProd == inventoryMovePlantTransfer.id_machineForProd)).ToList();
        //        model = db.LiquidationCartOnCart
        //            .Where(r => (r.Document.DocumentState.code != ("05")) &&//05: ANULADA
        //                        (DbFunctions.TruncateTime(r.MachineProdOpening.Document.emissionDate) == DbFunctions.TruncateTime(inventoryMovePlantTransfer.dateTimeEmision)) &&
        //                        (r.MachineProdOpening.id_Turn == inventoryMovePlantTransfer.id_turn) &&
        //                        (r.id_MachineForProd == inventoryMovePlantTransfer.id_machineForProd))
        //            .GroupBy(g => new
        //            {
        //                g.ProductionLot.internalNumber,
        //                g.id_ProductionLot,
        //                g.idProccesType
        //            })
        //            .Select(s => new SelectListItem
        //            {
        //                Text = s.Key.internalNumber + " " + (db.ProcessType.FirstOrDefault(fod => fod.id == s.Key.idProccesType).code == "ENT" ? "ENTERO" : "COLA"),
        //                Value = s.Key.id_ProductionLot.ToString() + s.Key.idProccesType.ToString()
        //            }).ToList();
        //    }

        //    return GridViewExtension.GetComboBoxCallbackResult(p =>
        //    {
        //        //settings.Name = "id_person";
        //        p.ClientInstanceName = "id_motiveLotProcessType";
        //        p.Width = Unit.Percentage(100);

        //        p.ValueField = "Value";
        //        p.TextField = "Text";
        //        p.ValueType = typeof(int);
        //        //p.DropDownStyle = DropDownStyle.DropDownList;
        //        //p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
        //        //p.EnableSynchronization = DefaultBoolean.False;
        //        //p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

        //        p.CallbackRouteValues = new
        //        {
        //            Controller = "InventoryMovePlantTransfer",
        //            Action = "ComboBoxMotivoLoteEdit",
        //        };
        //        p.CallbackPageSize = 15;
        //        p.ClientSideEvents.BeginCallback = "MotivoLoteComboBox_BeginCallback";
        //        p.ClientSideEvents.EndCallback = "MotivoLoteComboBox_EndCallback";
        //        p.ClientSideEvents.Validation = "MotivoLoteComboBox_Validation";
        //        p.ClientSideEvents.SelectedIndexChanged = "MotivoLoteComboBox_SelectedIndexChanged";

        //        p.ValidationSettings.RequiredField.IsRequired = true;
        //        p.ValidationSettings.RequiredField.ErrorText = "Campo Obligatorio";
        //        p.ValidationSettings.CausesValidation = true;
        //        //p.ValidationSettings.ErrorDisplayMode = DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
        //        p.ValidationSettings.ValidateOnLeave = true;
        //        p.ValidationSettings.SetFocusOnError = true;
        //        p.ValidationSettings.ErrorText = "Valor Incorrecto";

        //        p.ValidationSettings.EnableCustomValidation = true;
        //        p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

        //        p.BindList(model);

        //    });

        //}

        #endregion

        [HttpPost]
        public JsonResult Save(string jsonInventoryMovePlantTransfer)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var inventoryMovePlantTransferDTO = GetInventoryMovePlantTransferDTO();

                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMovePlantTransferDTO.id_machineForProdCartOnCart && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para editar y guardar la Transferencia en Planta para la máquina de Salida: " + inventoryMovePlantTransferDTO.machineForProdCartOnCart);
                                }
                                entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMovePlantTransferDTO.id_machineForProdCogellingFresh && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para editar y guardar la Transferencia en Planta para la máquina de Salida: " + inventoryMovePlantTransferDTO.machineForProdCogellingFresh);
                                }
                            }

                        }

                        #endregion


                        foreach (var item in inventoryMovePlantTransferDTO.InventoryMovePlantTransferDetails)
                        {
                            //var productioLot = db.ProductionLot.FirstOrDefault(fod=> fod.id == item.id_lot);
                            bool isCopaking = inventoryMovePlantTransferDTO.isCopackingLot;//productioLot?.isCopackingLot ?? false;
                            var placeWarehouse = isCopaking ? "Proceso Materia Prima Tercero" : "Compra de Materia Prima";
                            //Exit
                            if (item.id_warehouseExit == null)
                            {
                                throw new Exception("Debe definir la bodega de Salida, correspondiente a la Máquina: " + inventoryMovePlantTransferDTO.machineForProdCartOnCart + " en la sección: " + placeWarehouse);
                            }
                            if (item.id_warehouseLocationExit == null)
                            {
                                throw new Exception("Debe definir la ubicación de Salida, correspondiente a la Máquina: " + inventoryMovePlantTransferDTO.machineForProdCartOnCart + " en la sección: " + placeWarehouse);
                            }
                            if (item.id_costCenter == null)
                            {
                                throw new Exception("Debe definir el centro de Costo de Salida, correspondiente a la Máquina: " + inventoryMovePlantTransferDTO.machineForProdCartOnCart + " en la sección: " + placeWarehouse);
                            }
                            if (item.id_subCostCenter == null)
                            {
                                throw new Exception("Debe definir el Subcentro de Costo de Salida, correspondiente a la Máquina: " + inventoryMovePlantTransferDTO.machineForProdCartOnCart + " en la sección: " + placeWarehouse);
                            }
                            //Entry
                            if (item.id_warehouseEntry == null)
                            {
                                throw new Exception("Debe definir la bodega de Entrada, correspondiente a la Máquina: " + inventoryMovePlantTransferDTO.machineForProdCogellingFresh + " en la sección: " + placeWarehouse);
                            }
                            if (item.id_warehouseLocationExit == null)
                            {
                                throw new Exception("Debe definir la ubicación de Entrada, correspondiente a la Máquina: " + inventoryMovePlantTransferDTO.machineForProdCogellingFresh + " en la sección: " + placeWarehouse);
                            }
                            if (item.id_costCenterEntry == null)
                            {
                                throw new Exception("Debe definir el centro de Costo de Entrada, correspondiente a la Máquina: " + inventoryMovePlantTransferDTO.machineForProdCogellingFresh + " en la sección: " + placeWarehouse);
                            }
                            if (item.id_subCostCenterEntry == null)
                            {
                                throw new Exception("Debe definir el Subcentro de Costo de Entrada, correspondiente a la Máquina: " + inventoryMovePlantTransferDTO.machineForProdCogellingFresh + " en la sección: " + placeWarehouse);
                            }
                            var umMov = db.MetricUnit.FirstOrDefault(fod => fod.id == item.id_umMov);
                            if (umMov == null)
                            {
                                throw new Exception("Debe definir la UM en la sección de Inventario del Producto: " + item.nameItem);
                            }
                            else
                            {
                                if (umMov.MetricType.code != "UNI01")
                                {
                                    throw new Exception("La UM de movimiento debe ser del Tipo: Unidades, no puede ser de: " + umMov.MetricType.name + ", para el Producto: " + item.nameItem);
                                }
                            }
                            if (entityObjectPermissions != null)
                            {
                                var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                if (entityPermissions != null)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMovePlantTransferDTO.id_warehouseEntry && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                    if (entityValuePermissions == null)
                                    {
                                        throw new Exception("No tiene Permiso para editar y guardar la Transferencia en Planta para la bodega de Entrada: " + inventoryMovePlantTransferDTO.warehouseEntry);
                                    }
                                }
                                //entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                //if (entityPermissions != null)
                                //{
                                //    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_warehouseExit && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
                                //    if (entityValuePermissions == null)
                                //    {
                                //        throw new Exception("No tiene Permiso para editar y guardar la Transferencia en Planta para la bodega de Salida: " + item.warehouseExit + " del detalle.");
                                //    }
                                //}

                            }
                        }

                        JToken token = JsonConvert.DeserializeObject<JToken>(jsonInventoryMovePlantTransfer);

                        var newObject = false;
                        var id = token.Value<int>("id");

                        //var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals(m_TipoDocumentoInventoryMovePlantTransfer));
                        //var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                        var inventoryMovePlantTransfer = db.InventoryMovePlantTransfer.FirstOrDefault(d => d.id == id);
                        if (inventoryMovePlantTransfer == null)
                        {
                            newObject = true;

                            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
                            if (id_emissionPoint == 0)
                                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

                            inventoryMovePlantTransfer = new InventoryMovePlantTransfer();
                        }
                        //{
                        //    Document = new Document
                        //    {
                        //        number = GetDocumentNumber(documentType?.id ?? 0),
                        //        sequential = GetDocumentSequential(documentType?.id ?? 0),
                        //        emissionDate = DateTime.Now,
                        //        authorizationDate = DateTime.Now,
                        //        id_emissionPoint = id_emissionPoint,
                        //        id_documentType = documentType.id,
                        //        id_userCreate = ActiveUser.id,
                        //        dateCreate = DateTime.Now,
                        //    }
                        //};
                        //inventoryMovePlantTransfer.id_machineForProd = token.Value<int>("id_machineForProd");//inventoryMovePlantTransferDTO.id_machineForProd;
                        ////inventoryMovePlantTransfer.emissionDate = inventoryMovePlantTransferDTO.dateTimeEmision;
                        //inventoryMovePlantTransfer.dateTimeEntry = token.Value<DateTime>("dateTimeEntry");

                        //documentType.currentNumber++;
                        //db.DocumentType.Attach(documentType);
                        //db.Entry(documentType).State = EntityState.Modified;

                        //Details
                        if (inventoryMovePlantTransferDTO.InventoryMovePlantTransferDetails.Count() <= 0)
                        {
                            throw new Exception("No se puede guardar la Transferencia en Planta. Sin detalle.");
                        }
                        inventoryMovePlantTransferDTO.id_warehouseEntry = inventoryMovePlantTransferDTO.InventoryMovePlantTransferDetails.FirstOrDefault().id_warehouseEntry;

                        var lastDetails = db.InventoryMovePlantTransferDetail.Where(d => d.id_inventoryMovePlantTransfer == inventoryMovePlantTransfer.id);
                        foreach (var detail in lastDetails)
                        {
                            db.InventoryMovePlantTransferDetail.Remove(detail);
                            db.InventoryMovePlantTransferDetail.Attach(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }


                        var id_metricUnitKgAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Kg")?.id ?? 0;
                        var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;
                        //var newDetails = token.Value<JArray>("inventoryMovePlantTransferDetails");
                        foreach (var detail in inventoryMovePlantTransferDTO.InventoryMovePlantTransferDetails)
                        {

                            var itemToWarehouse = db.Item.FirstOrDefault(fod => fod.id == detail.id_item);
                            var quantityKgsAux = 0.00M;
                            var quantityPoundsAux = 0.00M;
                            var presentation = itemToWarehouse?.Presentation;
                            var quantityTotal = presentation == null ? detail.amountToEnter : presentation.minimum * detail.amountToEnter;
                            var id_metricUnitPresentation = presentation?.id_metricUnit
                                ?? itemToWarehouse?.ItemHeadIngredient?.id_metricUnit
                                ?? itemToWarehouse?.ItemInventory?.id_metricUnitInventory
                                ?? 0;

                            if ((quantityTotal > 0) && (id_metricUnitPresentation > 0))
                            {
                                //KG
                                if (id_metricUnitKgAux == id_metricUnitPresentation)
                                {
                                    quantityKgsAux = this.Truncate2Decimals(quantityTotal);
                                }
                                else
                                {
                                    var factor = db.MetricUnitConversion
                                        .FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
                                                    && muc.id_metricOrigin == id_metricUnitPresentation
                                                    && muc.id_metricDestiny == id_metricUnitKgAux)?
                                        .factor ?? 0;
                                    //decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 10000m) / 10000m;
                                    decimal _factorlb = (presentation.minimum * factor);

                                    //quantityKgsITW = this.Truncate2Decimals(quantity * factor);
                                    quantityKgsAux = Math.Truncate((detail.amountToEnter * _factorlb) * 10000m) / 10000m;
                                }


                                //LBS
                                if (id_metricUnitLbsAux == id_metricUnitPresentation)
                                {
                                    quantityPoundsAux = this.Truncate2Decimals(quantityTotal);
                                }
                                else
                                {
                                    var factor = db.MetricUnitConversion
                                        .FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
                                                    && muc.id_metricOrigin == id_metricUnitPresentation
                                                    && muc.id_metricDestiny == id_metricUnitLbsAux)?
                                        .factor ?? 0;
                                    //decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 10000m) / 10000m;
                                    decimal _factorlb = (presentation.minimum * factor);

                                    //quantityPoundsITW = this.Truncate2Decimals(quantity * factor);
                                    quantityPoundsAux = Math.Truncate((detail.amountToEnter * _factorlb) * 10000m) / 10000m;

                                }
                            }
                            else
                            {
                                quantityKgsAux = 0.00M;
                                quantityPoundsAux = 0.00M;
                            }

                            var liquidationCartOnCartDetail = db.LiquidationCartOnCartDetail.FirstOrDefault(fod => fod.id == detail.id_liquidationCartOnCartDetail);
                            var liquidationCartOnCart = db.LiquidationCartOnCart.FirstOrDefault(fod => fod.id == liquidationCartOnCartDetail.id_LiquidationCartOnCart);


                            inventoryMovePlantTransfer.InventoryMovePlantTransferDetail.Add(new InventoryMovePlantTransferDetail
                            {
                                id_inventoryMovePlantTransfer = inventoryMovePlantTransfer.id,
                                id_liquidationCartOnCartDetail = detail.id_liquidationCartOnCartDetail,
                                id_liquidationCartOnCart = liquidationCartOnCart.id,
                                boxesToReceive = detail.amountToEnter,
                                quantityKgs = quantityKgsAux,
                                quantityPounds = quantityPoundsAux
                            });
                        }


                        //var inventoryMovePlantTransferAux = db.InventoryMovePlantTransfer.AsEnumerable().FirstOrDefault(r => r.id != inventoryMovePlantTransfer.id &&
                        //					   r.idResultProdLotInventoryMovePlantTransfer == id_reception && r.idInventoryMovePlantTransferType == token.Value<int>("idTypeInventoryMovePlantTransfer") &&
                        //					   (r.Document.DocumentState.code == "01" || r.Document.DocumentState.code == "03"));
                        //if (inventoryMovePlantTransferAux != null)
                        //	throw new Exception("Existe la Recepción del Lote: " + inventoryMovePlantTransferAux.ResultProdLotInventoryMovePlantTransfer.numberLot +
                        //						" con el mismo tipo de InventoryMovePlantTransfer: " + inventoryMovePlantTransferAux.tbsysCatalogueDetail.name +
                        //						" en el InventoryMovePlantTransfer con número: " + inventoryMovePlantTransferAux.Document.number +
                        //						" con estado: " + inventoryMovePlantTransferAux.Document.DocumentState.name + ".");

                        //inventoryMovePlantTransfer.Document.id_documentState = documentState.id;
                        //inventoryMovePlantTransfer.Document.id_userUpdate = ActiveUser.id;
                        //inventoryMovePlantTransfer.id_userUpdate = inventoryMovePlantTransfer.Document.id_userUpdate;
                        //inventoryMovePlantTransfer.Document.dateUpdate = DateTime.Now;
                        inventoryMovePlantTransferDTO.reference = token.Value<string>("reference");
                        inventoryMovePlantTransferDTO.description = token.Value<string>("description");
                        inventoryMovePlantTransfer.id_machineProdOpeningDetail = token.Value<int>("id_machineProdOpeningDetail");

                        var aMachineProdOpeningDetail = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == inventoryMovePlantTransfer.id_machineProdOpeningDetail);
                        if (!aMachineProdOpeningDetail.MachineForProd.available)
                        {
                            throw new Exception("Máquina: " + aMachineProdOpeningDetail.MachineForProd.name + " no está disponible por motivo: " + aMachineProdOpeningDetail.MachineForProd.reason + ".");
                        }

                        inventoryMovePlantTransfer.dateTimeEntry = token.Value<DateTime>("dateTimeEntry");
                        inventoryMovePlantTransferDTO.dateTimeEmision = token.Value<DateTime>("dateTimeEmision");
                        //inventoryMovePlantTransfer.hoursStop = inventoryMovePlantTransferDTO.hoursStop;
                        //inventoryMovePlantTransfer.hoursProduction = inventoryMovePlantTransferDTO.hoursProduction;
                        //inventoryMovePlantTransfer.totalHours = inventoryMovePlantTransferDTO.totalHours;

                        //inventoryMovePlantTransfer.idWeigher = token.Value<int>("idWeigher");
                        //inventoryMovePlantTransfer.idAnalist = token.Value<int>("idAnalist");
                        //inventoryMovePlantTransfer.idInventoryMovePlantTransferType = token.Value<int>("idTypeInventoryMovePlantTransfer");



                        //inventoryMovePlantTransfer.PoundsGarbage = token.Value<decimal>("poundsTrash");
                        //inventoryMovePlantTransfer.gavetaNumber = token.Value<int>("drawersNumber");
                        //inventoryMovePlantTransfer.TotalPoundsWeigthGross = token.Value<decimal>("totalPoundsGrossWeight");
                        //inventoryMovePlantTransfer.porcTara = token.Value<decimal>("percentTara");
                        //inventoryMovePlantTransfer.TotalPoundsWeigthNet = token.Value<decimal>("totalPoundsNetWeight");
                        //inventoryMovePlantTransfer.idItemSize = token.Value<int?>("idSize");
                        //inventoryMovePlantTransfer.idResultProdLotInventoryMovePlantTransfer = id_reception;
                        ServiceInventoryMove.ServiceInventoryMoveAux resultAux = ServiceInventoryMove.UpdateInventaryMovePlantTransferExit(false, ActiveUser, ActiveCompany, ActiveEmissionPoint, inventoryMovePlantTransferDTO, db, false);
                        inventoryMovePlantTransfer.InventoryMove = resultAux?.inventoryMove;
                        inventoryMovePlantTransfer.InventoryMove1 = resultAux?.inventoryMoveAux;

                        if (newObject)
                        {
                            db.InventoryMovePlantTransfer.Add(inventoryMovePlantTransfer);
                            db.Entry(inventoryMovePlantTransfer).State = EntityState.Added;
                        }
                        else
                        {
                            db.InventoryMovePlantTransfer.Attach(inventoryMovePlantTransfer);
                            db.Entry(inventoryMovePlantTransfer).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        trans.Commit();

                        result.Data = inventoryMovePlantTransfer.id.ToString();

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

            var result = new ApiResult();

            try
            {
                var resultTransac = ApproveInventoryMovePlantTransfer(id);
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
                result.Message = GenericError.ErrorGeneralInventoryMovePlantTransfer;
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
            //            result.Data = ApproveInventoryMovePlantTransfer(id).name;
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

        public Tuple<DocumentState, bool> ApproveInventoryMovePlantTransfer(    int id_inventoryMovePlantTransfer,
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
                    var inventoryMovePlantTransfer = db.InventoryMovePlantTransfer.FirstOrDefault(p => p.id == id_inventoryMovePlantTransfer);
                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
                    string documentNumber = db.Document.FirstOrDefault(r => r.id == id_inventoryMovePlantTransfer)?.number;

                    if (!isInternalTrans)
                    {
                        int documentTypeId = db.DocumentType.FirstOrDefault(r => r.code == "135").id;
                        int numDetails = inventoryMovePlantTransfer.InventoryMovePlantTransferDetail.Count();
                        string sessionInfo = ServiceTransCtl.GetSessionInfoSerialize(   ActiveUserId,
                                                                                        ActiveUser.username,
                                                                                        ActiveCompanyId,
                                                                                        ActiveEmissionPoint.id,
                                                                                        entityObjectPermissions);

                        string dataExecution = JsonConvert.SerializeObject(new object[] { id_inventoryMovePlantTransfer });
                        string dataExecutionTypes = JsonConvert.SerializeObject(new object[] { "System.Int32" });

                        var result = ServiceTransCtl.TransCtlExternal(
                                                            id_inventoryMovePlantTransfer,
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

                            if (inventoryMovePlantTransfer != null)
                            {
                                #region Validación Permiso


                                if (entityObjectPermissions != null)
                                {
                                    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                                    if (entityPermissions != null)
                                    {
                                        var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMovePlantTransfer.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                        if (entityValuePermissions == null)
                                        {
                                            throw new ProdHandlerException("No tiene Permiso para aprobar la Transferencia en Planta para la máquina Salida.");
                                        }
                                        entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMovePlantTransfer.MachineProdOpeningDetail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                        if (entityValuePermissions == null)
                                        {
                                            throw new ProdHandlerException("No tiene Permiso para aprobar la Transferencia en Planta para la máquina Entrada.");
                                        }
                                    }
                                    foreach (var item in inventoryMovePlantTransfer.InventoryMovePlantTransferDetail)
                                    {
                                        var liquidationCartOnCartDetail = item.LiquidationCartOnCartDetail;
                                        bool isCopackingLot = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.isCopackingLot ?? false;

                                        var id_warehouseExitAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdWarehouse : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialWarehouse);
                                        var warehouseExitAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseExitAux);

                                        var id_MachineForProdCogellingFresh = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == inventoryMovePlantTransfer.id_machineProdOpeningDetail)?.id_MachineForProd;
                                        var machineForProdCogellingFresh = db.MachineForProd.FirstOrDefault(fod => fod.id == id_MachineForProdCogellingFresh);

                                        var id_warehouseEntryAux = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdWarehouse : machineForProdCogellingFresh?.id_materialWarehouse);
                                        var warehouseEntryAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseEntryAux);

                                        //var itemAux = inventoryMovePlantTransfer.InventoryMove.InventoryMoveDetail.FirstOrDefault();
                                        entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                        if (entityPermissions != null)
                                        {
                                            var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == id_warehouseExitAux && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                            //if (entityValuePermissions == null && warehouseExitAux != null)
                                            //{
                                            //    throw new Exception("No tiene Permiso para aprobar la Transferencia en Planta para la bodega de Salida: " + warehouseExitAux + " del detalle.");
                                            //}
                                            entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == id_warehouseEntryAux && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
                                            if (entityValuePermissions == null && warehouseEntryAux != null)
                                            {
                                                throw new ProdHandlerException("No tiene Permiso para aprobar la Transferencia en Planta para la bodega de Entrada: " + warehouseEntryAux + " del detalle.");
                                            }
                                        }
                                    }

                                }

                                #endregion
                                foreach (var item in inventoryMovePlantTransfer.InventoryMovePlantTransferDetail)
                                {
                                    var boxesReceivedNew = item.boxesToReceive + (item.LiquidationCartOnCartDetail.boxesReceived ?? 0);

                                    if (boxesReceivedNew > item.LiquidationCartOnCartDetail.quatityBoxesIL)
                                    {
                                        var pending = item.LiquidationCartOnCartDetail.quatityBoxesIL - (item.LiquidationCartOnCartDetail.boxesReceived ?? 0);
                                        if (pending > 0)
                                        {
                                            throw new ProdHandlerException("No puede aprobarse, porque el detalle con Coche: " + item.LiquidationCartOnCartDetail.ProductionCart.name +
                                                            "y producto: " + item.LiquidationCartOnCartDetail.Item1.name +
                                                            ", la cantidad a ingresa: " + item.boxesToReceive.Value.ToString("N2") +
                                                            " debe ser menor o igual que: " + pending.ToString("N2") +
                                                            " que es la cantidad pendiente, para este detalle.");
                                        }
                                        else
                                        {
                                            throw new ProdHandlerException("No puede aprobarse, porque el detalle con Coche: " + item.LiquidationCartOnCartDetail.ProductionCart.name +
                                                                                                "y producto: " + item.LiquidationCartOnCartDetail.Item1.name +
                                                                                                " no tiene cantidad pendiente de recibir, para este detalle.");
                                        }

                                    }
                                }

                                var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                                if (aprovedState == null)
                                    return null;

                                if (inventoryMovePlantTransfer.InventoryMove.Document.DocumentType.code.Equals("135"))//Egreso Por Transferencia Automática Por Recepción Placa
                                {
                                    //var inventoryMoveTransferAutomaticExit = inventoryMove;

                                    var result = ServiceInventoryMove.UpdateInventaryMovePlantTransferExit(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, ConvertToDto(inventoryMovePlantTransfer), db, false, null);
                                    inventoryMovePlantTransfer.InventoryMove = result.inventoryMove;
                                    UpdateInventoryMovePlantTransferDetails(inventoryMovePlantTransfer, true, db);
                                }

                                //inventoryMovePlantTransfer.Document.id_documentState = aprovedState.id;
                                //inventoryMovePlantTransfer.Document.authorizationDate = DateTime.Now;

                                db.InventoryMovePlantTransfer.Attach(inventoryMovePlantTransfer);
                                db.Entry(inventoryMovePlantTransfer).State = EntityState.Modified;
                                db.SaveChanges();

                                trans.Commit();
                            }
                            else
                            {
                                throw new ProdHandlerException("No se encontró el objeto seleccionado");
                            }

                            documentState = inventoryMovePlantTransfer.InventoryMove.Document.DocumentState;
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

        private void UpdateInventoryMovePlantTransferDetails(InventoryMovePlantTransfer inventoryMovePlantTransfer, bool approve, DBContext dbCurrent)
        {
            var signo = approve ? 1 : -1;

            foreach (var item in inventoryMovePlantTransfer.InventoryMovePlantTransferDetail)
            {
                item.LiquidationCartOnCartDetail.boxesReceived = (item.LiquidationCartOnCartDetail.boxesReceived ?? 0) + (item.boxesToReceive * signo);
                dbCurrent.LiquidationCartOnCartDetail.Attach(item.LiquidationCartOnCartDetail);
                dbCurrent.Entry(item.LiquidationCartOnCartDetail).State = EntityState.Modified;
            }

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
                        result.Data = ConciliateInventoryMovePlantTransfer(id).name;
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

        private DocumentState ConciliateInventoryMovePlantTransfer(int id_inventoryMovePlantTransfer)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var inventoryMovePlantTransfer = db.InventoryMovePlantTransfer.FirstOrDefault(p => p.id == id_inventoryMovePlantTransfer);
                    try
                    {                        
                        if(inventoryMovePlantTransfer != null)
                        {
                            // Buscamos el estado 'Conciliado'
                            var estadoConciliado = db.DocumentState.FirstOrDefault(e => e.code == "16");
                            if (estadoConciliado == null)
                                throw new Exception("No se ha encontrado el estado: [16] - CONCILIADO");

                            // Actualizo primero su documento de movimiento de inventario
                            var documentSource = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == inventoryMovePlantTransfer.id);
                            var document = db.Document.FirstOrDefault(fod => fod.id == documentSource.id_document);
                            document.id_documentState = estadoConciliado.id;
                            document.id_userUpdate = this.ActiveUserId;
                            document.dateUpdate = DateTime.Now;

                            db.Document.Attach(document);
                            db.Entry(document).State = EntityState.Modified;

                            // Posterior actualizo el proceso en si
                            inventoryMovePlantTransfer.InventoryMove.Document.id_documentState = estadoConciliado.id;
                            inventoryMovePlantTransfer.InventoryMove.Document.id_userUpdate = this.ActiveUserId;
                            inventoryMovePlantTransfer.InventoryMove.Document.dateUpdate = DateTime.Now;

                            // Busco el elemento, modifico y guardo
                            db.InventoryMovePlantTransfer.Attach(inventoryMovePlantTransfer);
                            db.Entry(inventoryMovePlantTransfer).State = EntityState.Modified;
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

                    return inventoryMovePlantTransfer.InventoryMove.Document.DocumentState;
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
                        // Revisión del documento 
                        var document = db.Document.FirstOrDefault(e => e.id == id);
                        if(document.DocumentState.code == "16")
                        {
                            result.Data = ReverseConciliateInventoryMovePlantTransfer(id).name;
                        }
                        else
                        {
                            result.Data = ReverseInventoryMovePlantTransfer(id).name;
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

        private DocumentState ReverseConciliateInventoryMovePlantTransfer(int id_inventoryMovePlantTransfer)
        {
            using (var db = new DBContext())
            {                
                using (var trans = db.Database.BeginTransaction())
                {
                    var inventoryMovePlantTransfer = db.InventoryMovePlantTransfer.FirstOrDefault(p => p.id == id_inventoryMovePlantTransfer);
                    try
                    {
                        if (inventoryMovePlantTransfer != null)
                        {
                            var approvedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                            if (approvedState == null)
                                return null;

                            //Verificar si existe lotes relacionados al proceso de cierre
                            var inventoryMoveDetail = db.InventoryMoveDetail.Where(w => w.id_inventoryMove == inventoryMovePlantTransfer.id_inventoryMoveEntry).ToList();
                            var inventoryMove = inventoryMoveDetail.FirstOrDefault().InventoryMove;

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

                            // Actualizo primero su documento
                            var documentSource = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == inventoryMovePlantTransfer.id);
                            var document = db.Document.FirstOrDefault(fod => fod.id == documentSource.id_document);
                            document.id_documentState = approvedState.id;
                            document.id_userUpdate = this.ActiveUserId;
                            document.dateUpdate = DateTime.Now;

                            db.Document.Attach(document);
                            db.Entry(document).State = EntityState.Modified;

                            // Posterior actualizo el proceso en si
                            inventoryMovePlantTransfer.InventoryMove.Document.id_documentState = approvedState.id;
                            inventoryMovePlantTransfer.InventoryMove.Document.id_userUpdate = this.ActiveUserId;
                            inventoryMovePlantTransfer.InventoryMove.Document.dateUpdate = DateTime.Now;

                            // Busco el elemento, modifico y guardo
                            db.InventoryMovePlantTransfer.Attach(inventoryMovePlantTransfer);
                            db.Entry(inventoryMovePlantTransfer).State = EntityState.Modified;
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

                    return inventoryMovePlantTransfer.InventoryMove.Document.DocumentState;
                }
            }
        }

        private DocumentState ReverseInventoryMovePlantTransfer(int id_inventoryMovePlantTransfer)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var inventoryMovePlantTransfer = db.InventoryMovePlantTransfer.FirstOrDefault(p => p.id == id_inventoryMovePlantTransfer);
                    if (inventoryMovePlantTransfer != null)
                    {
                        //Verificar si existe lotes relacionados al proceso de cierre
                        var inventoryMoveDetail = db.InventoryMoveDetail.Where(w => w.id_inventoryMove == inventoryMovePlantTransfer.id_inventoryMoveEntry).ToList();
                        var inventoryMove = inventoryMoveDetail.FirstOrDefault().InventoryMove;

                        foreach (var item in inventoryMoveDetail)
                        {
                            var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.number.Substring(0, 5) == item.Lot.internalNumber.Substring(0, 5) && a.Document.DocumentState.code != "05"
                                                       && a.isActive);

                            if (productionLotClose != null && inventoryMove.Document.emissionDate.Date <= productionLotClose.Document.emissionDate.Date 
                                && inventoryMove.Document.DocumentState.code == "03")
                            {
                                var lot = db.ProductionLot.FirstOrDefault(a => a.id == item.id_lot);
                                if (lot != null)
                                {
                                    throw new Exception("El lote " + lot.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                                }
                            }
                        }

                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMovePlantTransfer.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para reversar la Transferencia en Planta para la máquina Salida.");
                                }
                                entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMovePlantTransfer.MachineProdOpeningDetail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para reversar la Transferencia en Planta para la máquina Entrada.");
                                }
                            }
                            foreach (var item in inventoryMovePlantTransfer.InventoryMovePlantTransferDetail)
                            {
                                var liquidationCartOnCartDetail = item.LiquidationCartOnCartDetail;
                                bool isCopackingLot = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.isCopackingLot ?? false;

                                var id_warehouseExitAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdWarehouse : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialWarehouse);
                                var warehouseExitAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseExitAux);

                                var id_MachineForProdCogellingFresh = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == inventoryMovePlantTransfer.id_machineProdOpeningDetail)?.id_MachineForProd;
                                var machineForProdCogellingFresh = db.MachineForProd.FirstOrDefault(fod => fod.id == id_MachineForProdCogellingFresh);

                                var id_warehouseEntryAux = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdWarehouse : machineForProdCogellingFresh?.id_materialWarehouse);
                                var warehouseEntryAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseEntryAux);

                                //var itemAux = inventoryMovePlantTransfer.InventoryMove.InventoryMoveDetail.FirstOrDefault();
                                entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                if (entityPermissions != null)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == id_warehouseExitAux && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                    //if (entityValuePermissions == null && warehouseExitAux != null)
                                    //{
                                    //    throw new Exception("No tiene Permiso para reversar la Transferencia en Planta para la bodega de Salida: " + warehouseExitAux.name + " del detalle.");
                                    //}
                                    entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == id_warehouseEntryAux && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
                                    if (entityValuePermissions == null && warehouseEntryAux != null)
                                    {
                                        throw new Exception("No tiene Permiso para reversar la Transferencia en Planta para la bodega de Entrada: " + warehouseEntryAux.name + " del detalle.");
                                    }
                                }
                            }

                        }

                        #endregion
                        if (inventoryMovePlantTransfer.MachineProdOpeningDetail.MachineProdOpening.Document.DocumentState.code != "03")
                        {
                            throw new Exception("No puede reversar la Transferencia en Planta por no tener Aprobado el estado de la Apertura del Túnel: " + inventoryMovePlantTransfer.MachineProdOpeningDetail.MachineProdOpening.Document.number + ".");
                        }
                        foreach (var aItem in inventoryMovePlantTransfer.InventoryMovePlantTransferDetail)
                        {
                            if (aItem.LiquidationCartOnCart.Document.DocumentState.code != "01")
                            {
                                throw new Exception("No puede reversar la Transferencia en Planta por no tener Pendiente el estado de la liquidación: " + aItem.LiquidationCartOnCart.Document.number + ".");
                            }
                        }
                        var reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
                        if (reverseState == null)
                            return null;

                        if (inventoryMovePlantTransfer.InventoryMove.Document.DocumentType.code.Equals("135"))//Egreso Por Transferencia Automática Por Recepción Placa
                        {
                            var inventoryMoveTransferAutomaticExit = inventoryMovePlantTransfer.InventoryMove;
                            var result = ServiceInventoryMove.UpdateInventaryMovePlantTransferExit(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, ConvertToDto(inventoryMovePlantTransfer), db, true, inventoryMoveTransferAutomaticExit);
                            //inventoryMovePlantTransfer.InventoryMove = result.inventoryMove;
                            UpdateInventoryMovePlantTransferDetails(inventoryMovePlantTransfer, false, db);
                        }
                        inventoryMovePlantTransfer.InventoryMove.Document.id_documentState = reverseState.id;
                        //inventoryMovePlantTransfer.Document.authorizationDate = DateTime.Now;

                        db.InventoryMovePlantTransfer.Attach(inventoryMovePlantTransfer);
                        db.Entry(inventoryMovePlantTransfer).State = EntityState.Modified;
                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return inventoryMovePlantTransfer.InventoryMove.Document.DocumentState;
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
                        result.Data = AnnulInventoryMovePlantTransfer(id).name;
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

        private DocumentState AnnulInventoryMovePlantTransfer(int id_inventoryMovePlantTransfer)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var inventoryMovePlantTransfer = db.InventoryMovePlantTransfer.FirstOrDefault(p => p.id == id_inventoryMovePlantTransfer);
                    if (inventoryMovePlantTransfer != null)
                    {
                        #region Validación Permiso

                        var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                        if (entityObjectPermissions != null)
                        {
                            var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
                            if (entityPermissions != null)
                            {
                                var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMovePlantTransfer.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para anular la Transferencia en Planta para la máquina Salida.");
                                }
                                entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == inventoryMovePlantTransfer.MachineProdOpeningDetail.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para anular la Transferencia en Planta para la máquina Entrada.");
                                }
                            }

                            foreach (var item in inventoryMovePlantTransfer.InventoryMovePlantTransferDetail)
                            {
                                var liquidationCartOnCartDetail = item.LiquidationCartOnCartDetail;
                                bool isCopackingLot = liquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.isCopackingLot ?? false;

                                var id_warehouseExitAux = (isCopackingLot ? liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialthirdWarehouse : liquidationCartOnCartDetail.LiquidationCartOnCart.MachineForProd.id_materialWarehouse);
                                var warehouseExitAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseExitAux);

                                var id_MachineForProdCogellingFresh = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == inventoryMovePlantTransfer.id_machineProdOpeningDetail)?.id_MachineForProd;
                                var machineForProdCogellingFresh = db.MachineForProd.FirstOrDefault(fod => fod.id == id_MachineForProdCogellingFresh);

                                var id_warehouseEntryAux = (isCopackingLot ? machineForProdCogellingFresh?.id_materialthirdWarehouse : machineForProdCogellingFresh?.id_materialWarehouse);
                                var warehouseEntryAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseEntryAux);

                                //var itemAux = inventoryMovePlantTransfer.InventoryMove.InventoryMoveDetail.FirstOrDefault();
                                entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                if (entityPermissions != null)
                                {
                                    var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == id_warehouseExitAux && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                    //if (entityValuePermissions == null && warehouseExitAux != null)
                                    //{
                                    //    throw new Exception("No tiene Permiso para anular la Transferencia en Planta para la bodega de Salida: " + warehouseExitAux.name + " del detalle.");
                                    //}
                                    entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == id_warehouseEntryAux && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
                                    if (entityValuePermissions == null && warehouseEntryAux != null)
                                    {
                                        throw new Exception("No tiene Permiso para anular la Transferencia en Planta para la bodega de Entrada: " + warehouseEntryAux.name + " del detalle.");
                                    }
                                }
                            }

                        }

                        #endregion
                        var aLiquidationCartOnCart = inventoryMovePlantTransfer.InventoryMovePlantTransferDetail.FirstOrDefault().LiquidationCartOnCart;
                        var codeDocumentStateLiquidationCartOnCart = aLiquidationCartOnCart.Document.DocumentState.code;
                        if (codeDocumentStateLiquidationCartOnCart != "01" && codeDocumentStateLiquidationCartOnCart != "05")
                        {
                            throw new Exception("No puedes anular la Transferencia en Planta para la máquina de Salida: " + aLiquidationCartOnCart.MachineForProd.name + ". Porque la liquidación Carro x Carro: " +
                                                aLiquidationCartOnCart.Document.number + ", correspondiente esta en estado: " + aLiquidationCartOnCart.Document.DocumentState.name);
                        }

                        var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        if (annulState == null)
                            return null;

                        var id_inventaryMoveTransferAutomaticEntryToReverse = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == inventoryMovePlantTransfer.id &&
                                                                   fod.Document.DocumentState.code.Equals("01") &&
                                                                   fod.Document.DocumentType.code.Equals("136")).id_document;//136: Ingreso Por Transferencia Automática Por Recepción Placa
                        var inventaryMoveTransferAutomaticEntryToReverse = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveTransferAutomaticEntryToReverse);

                        inventaryMoveTransferAutomaticEntryToReverse.Document.id_documentState = annulState.id;
                        inventaryMoveTransferAutomaticEntryToReverse.Document.DocumentState = annulState;

                        db.InventoryMove.Attach(inventaryMoveTransferAutomaticEntryToReverse);
                        db.Entry(inventaryMoveTransferAutomaticEntryToReverse).State = EntityState.Modified;

                        //inventoryMovePlantTransfer.Document.DocumentState;
                        inventoryMovePlantTransfer.InventoryMove.Document.id_documentState = annulState.id;
                        inventoryMovePlantTransfer.InventoryMove.Document.DocumentState = annulState;
                        //inventoryMovePlantTransfer.Document.authorizationDate = DateTime.Now;

                        db.InventoryMovePlantTransfer.Attach(inventoryMovePlantTransfer);
                        db.Entry(inventoryMovePlantTransfer).State = EntityState.Modified;

                        db.SaveChanges();

                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return inventoryMovePlantTransfer.InventoryMove.Document.DocumentState;
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id)
        {
            var index = GetInventoryMovePlantTransferResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

            var result = new
            {
                maximunPages = GetInventoryMovePlantTransferResultConsultDTO().Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            var element = GetInventoryMovePlantTransferResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
            var inventoryMovePlantTransfer = db.InventoryMovePlantTransfer.FirstOrDefault(d => d.id == element.id);
            if (inventoryMovePlantTransfer == null)
                return PartialView("Edit", new InventoryMovePlantTransferDTO());


            var model = ConvertToDto(inventoryMovePlantTransfer);
            SetInventoryMovePlantTransferDTO(model);
            BuildViewDataEdit(model.dateTimeEmision);
            //var codeTypeInventoryMovePlantTransfer = db.tbsysCatalogueDetail.FirstOrDefault(fod => fod.id == model.idTypeInventoryMovePlantTransfer)?.code;
            //BuildComboBoxSizeInventoryMovePlantTransfer(codeTypeInventoryMovePlantTransfer);
            BuilViewBag(false);
            //BuildComboBoxTypeInventoryMovePlantTransfer(model.id, model.codeTypeProcess);

            return PartialView("Edit", model);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult MachineForProdCogellingFreshChanged(int? id_machineProdOpeningDetailCogellingFresh)
        {
            var inventoryMovePlantTransfer = GetInventoryMovePlantTransferDTO();
            bool isCopackingLot = inventoryMovePlantTransfer.isCopackingLot;
            //var proveedor = "";
            var result = new
            {
                message = "",
                turnCogellingFresh = "",
                warehouseEntry = "",
                timeInitTurn = "",
                timeEndTurn = ""
            };
            var machineProdOpeningDetail = db.MachineProdOpeningDetail.FirstOrDefault(fod => fod.id == id_machineProdOpeningDetailCogellingFresh);
            inventoryMovePlantTransfer.id_machineProdOpeningDetail = id_machineProdOpeningDetailCogellingFresh;
            inventoryMovePlantTransfer.id_machineForProdCogellingFresh = machineProdOpeningDetail?.id_MachineForProd;
            inventoryMovePlantTransfer.machineForProdCogellingFresh = machineProdOpeningDetail?.MachineForProd?.name ?? "";
            inventoryMovePlantTransfer.id_turnCogellingFresh = machineProdOpeningDetail?.MachineProdOpening?.id_Turn;
            inventoryMovePlantTransfer.turnCogellingFresh = machineProdOpeningDetail?.MachineProdOpening?.Turn.name;
            inventoryMovePlantTransfer.id_warehouseEntry = (isCopackingLot ? machineProdOpeningDetail?.MachineForProd?.id_materialthirdWarehouse : machineProdOpeningDetail?.MachineForProd?.id_materialWarehouse);
            inventoryMovePlantTransfer.warehouseEntry = db.Warehouse.FirstOrDefault(fod => fod.id == inventoryMovePlantTransfer.id_warehouseEntry)?.name ?? "";
            var id_warehouseLocationEntryAux = (isCopackingLot ? machineProdOpeningDetail?.MachineForProd?.id_materialthirdWarehouseLocation : machineProdOpeningDetail?.MachineForProd?.id_materialWarehouseLocation);
            var id_costCenterEntryAux = (isCopackingLot ? machineProdOpeningDetail?.MachineForProd?.id_materialthirdCostCenter : machineProdOpeningDetail?.MachineForProd?.id_materialCostCenter);
            var id_subCostCenterEntryAux = (isCopackingLot ? machineProdOpeningDetail?.MachineForProd?.id_materialthirdSubCostCenter : machineProdOpeningDetail?.MachineForProd?.id_materialSubCostCenter);
            var datosFaltan = (inventoryMovePlantTransfer.id_warehouseEntry == null ? " la bodega" : "");
            datosFaltan += (id_warehouseLocationEntryAux == null ? (datosFaltan != "" ? ", la ubicación" : " la ubicación") : "");
            datosFaltan += (id_costCenterEntryAux == null ? (datosFaltan != "" ? ", el centro de costo" : " el centro de costo") : "");
            datosFaltan += (id_subCostCenterEntryAux == null ? (datosFaltan != "" ? ", el subcentro de costo" : " el subcentro de costo") : "");

            inventoryMovePlantTransfer.timeInitTurn = machineProdOpeningDetail != null ? (machineProdOpeningDetail.MachineProdOpening.Turn.timeInit.Hours + ":" + machineProdOpeningDetail.MachineProdOpening.Turn.timeInit.Minutes) : null;
            inventoryMovePlantTransfer.timeEndTurn = machineProdOpeningDetail != null ? (machineProdOpeningDetail.MachineProdOpening.Turn.timeEnd.Hours + ":" + machineProdOpeningDetail.MachineProdOpening.Turn.timeEnd.Minutes) : null;
            if (machineProdOpeningDetail != null)
            {
                var placeWarehouse = isCopackingLot ? "Proceso Materia Prima Tercero" : "Compra de Materia Prima";
                var messageAux = (inventoryMovePlantTransfer.id_warehouseEntry == null || id_warehouseLocationEntryAux == null ||
                                  id_costCenterEntryAux == null || id_subCostCenterEntryAux == null) ? ("Falta definir" + datosFaltan + ". Correspondiente a la Máquina: " + inventoryMovePlantTransfer.machineForProdCogellingFresh + ", en la sección: " + placeWarehouse) : "";
                result = new
                {
                    message = messageAux,
                    inventoryMovePlantTransfer.turnCogellingFresh,
                    inventoryMovePlantTransfer.warehouseEntry,
                    inventoryMovePlantTransfer.timeInitTurn,
                    inventoryMovePlantTransfer.timeEndTurn
                };
            }



            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Reporteria
        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReport(int id_inventoryMovePlantTransfer, string codeReport)
        {
            List<ParamCR> paramLst = new List<ParamCR>();

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id_inventoryMovePlantTransfer
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

        public JsonResult DownloadReportPlantTransfer(DateTime startDate,DateTime endDate, int idWarehouse ,int idarehouseLocation )
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();

            ParamCR _param = new ParamCR
            {
                Nombre = "@startEmissionDate",
                Valor = startDate,
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@endEmissionDate",
                Valor = endDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@id_warehouse",
                Valor = idWarehouse
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@id_warehouseLocation",
                Valor = idarehouseLocation
            };
            paramLst.Add(_param);

            db.Database.CommandTimeout = 2200;

            List<ResultInventoryMovePlantTransferDTO> modelAux = new List<ResultInventoryMovePlantTransferDTO>();
            modelAux = db.Database.SqlQuery<ResultInventoryMovePlantTransferDTO>
                    ("exec inv_Exportar_Transf_Placa_Tunel @startEmissionDate, @endEmissionDate, @id_warehouse, @id_warehouseLocation",
                    new SqlParameter("startEmissionDate", paramLst[0].Valor),
                    new SqlParameter("endEmissionDate", paramLst[1].Valor),
                    new SqlParameter("id_warehouse", paramLst[2].Valor),
                    new SqlParameter("id_warehouseLocation", paramLst[3].Valor)
            ).ToList();

            TempData["modelMovePlantTransferDTO"] = modelAux;
            TempData["paramStartDate"] = paramLst[0].Valor;
            return Json(modelAux, JsonRequestBehavior.AllowGet);
            #endregion
        }



        #endregion

    }
}