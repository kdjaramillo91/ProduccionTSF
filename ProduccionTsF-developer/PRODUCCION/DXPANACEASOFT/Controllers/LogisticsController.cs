using DevExpress.Web.Mvc;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentLogDTO;
using DXPANACEASOFT.Models.FE;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using DXPANACEASOFT.Models.General;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using DXPANACEASOFT.Models.RemGuide;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using EntidadesAuxiliares.SQL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Utilitarios.Logs;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class LogisticsController : DefaultController
    {
        const string SELECT_TRANSPORTSIZE = "SELECT id, code,name,description,id_poundsRange,id_iceBagRange,isActive, " +
                                            " id_company,id_userCreate,dateCreate,id_userUpdate,dateUpdate,id_transportTariffType, " +
                                            " binRangeMinimum,binRangeMaximun " +
                                            " FROM TransportSize ";

        private const int LOGON_TYPE_NEW_CREDENTIALS = 9;
        private const int LOGON32_PROVIDER_WINNT50 = 3;

        private ButtonsOnEditFormRemissionGuide _btnOnEditFormRG { get; set; }
        private AnswerForActionRemissionGuide _AnswerfaRG { get; set; }

        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IndexReportGuia(string id)
        {
            try
            {
                Session["URGUIA"] = ConfigurationManager.AppSettings["URGUIA"];
            }
            catch (Exception ex)
            {
                ViewBag.IframeUrl = ex.Message;
            }

            ViewBag.IframeUrl = Session["URGUIA"] + "?ID=" + id;

            return View();
        }

        public Boolean GetProvider(int? id_person)
        {
            var model = db.Person;
            Boolean? copack = null;
            var modelItem = model.FirstOrDefault(it => it.id == id_person);

            if (modelItem.isCopacking == null)
                copack = false;
            else
                copack = modelItem.isCopacking;

            return (Boolean)copack;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderByProvider(int? id_productionUnitProviderCurrent, int? id_provider)
        {
            if (id_provider == null || id_provider < 0)
            {
                if (Request.Params["id_provider"] != null && Request.Params["id_provider"] != "") id_provider = int.Parse(Request.Params["id_provider"]);
                else id_provider = -1;
            }

            var Copack = GetProvider(id_provider);

            var productionUnitProviderAux = db.ProductionUnitProvider.Where(t => t.isActive && t.id_provider == id_provider && t.isCopackingDetail == Copack).ToList();

            var productionUnitProviderCurrentAux = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionUnitProviderCurrent && fod.isCopackingDetail == Copack);
            if (productionUnitProviderCurrentAux != null && !productionUnitProviderAux.Contains(productionUnitProviderCurrentAux)) productionUnitProviderAux.Add(productionUnitProviderCurrentAux);

            TempData["ProductionUnitProviderByProvider"] = productionUnitProviderAux.Select(s => new
            {
                s.id,
                name = s.name
            }).OrderBy(t => t.id).ToList();

            TempData.Keep("ProductionUnitProviderByProvider");
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            this.ViewBag.isManual = remissionGuide.isManual;
            TempData.Keep("remissionGuide");
            return PartialView("comboboxcascading/_cmbProviderProductionUnitPartial", remissionGuide);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportTariffTypeByshippingType(int? id_shippingType, int? id_TransportTariffTypeCurrent)
        {
            if (id_shippingType == null || id_shippingType < 0)
            {
                if (Request.Params["id_shippingType"] != null && Request.Params["id_shippingType"] != "") id_shippingType = int.Parse(Request.Params["id_shippingType"]);
                else id_shippingType = -1;
            }
            if (id_shippingType == 8)
            {
                id_shippingType = 1;
            }
            var TransportTariffTyperAux = db.TransportTariffType.Where(t => t.isActive && t.id_shippingType == id_shippingType).ToList();

            var TransportTariffTyperCurrentAux = db.TransportTariffType
                                                    .FirstOrDefault(fod => fod.id == id_TransportTariffTypeCurrent
                                                                            && fod.id_shippingType == id_shippingType);

            if (TransportTariffTyperCurrentAux != null && !TransportTariffTyperAux.Contains(TransportTariffTyperCurrentAux)) TransportTariffTyperAux.Add(TransportTariffTyperCurrentAux);

            TempData["TransportTariffTypeByshippingType"] = TransportTariffTyperAux.Select(s => new
            {
                s.id,
                name = s.name
            }).OrderBy(t => t.id).ToList();

            TempData.Keep("TransportTariffTypeByshippingType");
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            TempData.Keep("remissionGuide");
            return PartialView("comboboxcascading/_cmbTransportTariffTypePartial", remissionGuide);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportTariffTypeVehicleType(int? id_TransportTariffType, int? id_vehicleCurrent)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            List<int> listVehicleType = new List<int>();

            if (id_TransportTariffType == null || id_TransportTariffType < 0)
            {
                if (Request.Params["id_TransportTariffType"] != null && Request.Params["id_TransportTariffType"] != "") id_TransportTariffType = int.Parse(Request.Params["id_TransportTariffType"]);
                else id_TransportTariffType = -1;
            }
            var TransportTariffTyperVehicleType = db.TransportTariffType_VehicleType.Where(t => t.isActive && t.id_transportTariffType == id_TransportTariffType).ToList();

            if (TransportTariffTyperVehicleType != null && TransportTariffTyperVehicleType.Count > 0)
            {
                var listVehicleTypeax = (from e in TransportTariffTyperVehicleType
                                         select e.id_vehicleType).ToList();

                if (listVehicleTypeax != null && listVehicleTypeax.Count > 0)
                {
                    listVehicleType.AddRange(listVehicleTypeax);
                }
            }

            var Vehicle = db.Vehicle.Where(v => v.isActive && listVehicleType.Contains(v.id_VehicleType)).ToList();

            var VehicleCurrentAux = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehicleCurrent && listVehicleType.Contains(fod.id_VehicleType));
            if (VehicleCurrentAux != null && !Vehicle.Contains(VehicleCurrentAux)) Vehicle.Add(VehicleCurrentAux);

            TempData["TransportTariffTypeVehicleType"] = Vehicle.OrderBy(t => t.id).ToList();

            TempData.Keep("TransportTariffTypeVehicleType");
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            TempData.Keep("remissionGuide");
            var _RemissionGuideTransportation = remissionGuide.RemissionGuideTransportation ?? new RemissionGuideTransportation();

            _RemissionGuideTransportation.id_TransportationType = id_TransportTariffType;
            return PartialView("comboboxcascading/_cmbVehiclePartial", _RemissionGuideTransportation);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderPoolByUnitProvider(int? id_productionUnitProviderPoolCurrent, int? id_productionUnitProvider)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }

            var productionUnitProviderPoolAux = db.ProductionUnitProviderPool.Where(t => t.isActive && t.id_productionUnitProvider == id_productionUnitProvider).ToList();

            var productionUnitProviderPoolCurrentAux = db.ProductionUnitProviderPool.FirstOrDefault(fod => fod.id == id_productionUnitProviderPoolCurrent);
            if (productionUnitProviderPoolCurrentAux != null && !productionUnitProviderPoolAux.Contains(productionUnitProviderPoolCurrentAux)) productionUnitProviderPoolAux.Add(productionUnitProviderPoolCurrentAux);

            TempData["ProductionUnitProviderPoolByUnitProvider"] = productionUnitProviderPoolAux.Select(s => new
            {
                s.id,
                name = s.name
            }).OrderBy(t => t.id).ToList();
            TempData.Keep("ProductionUnitProviderPoolByUnitProvider");
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            TempData.Keep("remissionGuide");
            return PartialView("comboboxcascading/_cmbProviderProductionUnitProviderPoolPartial", remissionGuide);
        }

        [HttpPost]
        public ActionResult IndexReportLote(string ID)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }

            try
            {
                Session["URLOTE"] = ConfigurationManager.AppSettings["URLOTE"];
            }
            catch (Exception ex)
            {
                ViewBag.IframeUrl = ex.Message;
            }

            ViewBag.IframeUrl = Session["URLOTE"] + "?id=" + ID;

            return View();
        }

        #region REMISSION GUIDE FILTERS RESULTS

        [HttpPost]
        public ActionResult RemissionGuideResults(RemissionGuide remissionGuide,
                                                  Document document,
                                                  string carRegistration,
                                                  DateTime? startEmissionDate, DateTime? endEmissionDate,
                                                  DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                                  DateTime? startDespachureDate, DateTime? endDespachureDate,
                                                  DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                                  DateTime? startentranceDateProductionBuilding, DateTime? endentranceDateProductionBuilding,
                                                  int[] items, int[] businessOportunities)
        {
            List<RGResultsQuery> lstRemissionGuides = ServiceLogistics.GetAllRemissionGuide(remissionGuide,
                                                                document,
                                                                carRegistration,
                                                                startEmissionDate, endEmissionDate,
                                                                startAuthorizationDate, endAuthorizationDate,
                                                                startDespachureDate, endDespachureDate,
                                                                startexitDateProductionBuilding, endexitDateProductionBuilding,
                                                                startentranceDateProductionBuilding, endentranceDateProductionBuilding,
                                                                items, businessOportunities, null);

            TempData["model"] = lstRemissionGuides;
            TempData.Keep("model");

            return PartialView("_RemissionGuideResultsPartial", lstRemissionGuides.OrderByDescending(r => r.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsResults()
        {
            var lstPodQP = db.RemissionGuideDetailPurchaseOrderDetail
                                .Where(w => !(w.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code == "01"
                                || w.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code == "05"))
                                .GroupBy(g => g.id_purchaseOrderDetail)
                                .Select(s => new
                                {
                                    id_pod = s.FirstOrDefault().id_purchaseOrderDetail,
                                    qp = s.Sum(sp => sp.quantity)
                                }).ToList();

            var lstPodQO = db.PurchaseOrderDetail
                                .Where(w => w.PurchaseOrder.Document.DocumentState.code == "06")
                                .GroupBy(g => g.id)
                                .Select(s => new
                                {
                                    id_pod = s.FirstOrDefault().id,
                                    qo = s.Sum(so => so.quantityApproved)
                                }).ToList();

            var lstPodQOResult = (from _qo in lstPodQO
                                  join _qp in lstPodQP on _qo.id_pod equals _qp.id_pod into qol
                                  from _qol in qol.DefaultIfEmpty()
                                  select new
                                  {
                                      id_q = _qo.id_pod,
                                      qo = _qo.qo,
                                      qp = (_qol == null ? 0 : _qol.qp)
                                  }).Distinct().ToList();

            var lstPodQOResultFiltered = lstPodQOResult.Where(w => w.qo > w.qp).Select(s => s.id_q).Distinct().ToList();

            var model = db.PurchaseOrderDetail
                            .Where(d => d.PurchaseOrder.Document.DocumentState.code == "06"
                                        && lstPodQOResultFiltered.Contains(d.id)
                                    )
                          .OrderByDescending(d => d.id_purchaseOrder).ToList().OrderByDescending(d => d.id);

            var idPersonProcessPlant = db.Person.FirstOrDefault(p => p.identification_number == ActiveCompany.ruc)?.id ?? 0;

            foreach (var item in model)
            {
                var aPerson = db.Person.FirstOrDefault(fod => fod.id == (item.PurchaseOrder.id_personProcessPlant != null ? item.PurchaseOrder.id_personProcessPlant : idPersonProcessPlant));
            }

            TempData["modelPod"] = model;
            TempData.Keep("modelPod");
            return PartialView("_PurchaseOrderDetailsResultsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsResultsForReassignment()
        {
            var model = db.PurchaseOrderDetail
                            .Where(d => d.PurchaseOrder.Document.DocumentState.code == "06"
                                    && (d.quantityApproved - d.quantityDispatched) > 0
                                    && d.PurchaseOrder.requiredLogistic == true
                                    )
                          .OrderByDescending(d => d.id_purchaseOrder).ToList().OrderByDescending(d => d.id);

            return PartialView("_PurchaseOrderDetailsForReassignmentResultsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartial()
        {
            var lstPodQP = db.RemissionGuideDetailPurchaseOrderDetail
                    .Where(w => !(w.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code == "01"
                    || w.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code == "05"))
                    .GroupBy(g => g.id_purchaseOrderDetail)
                    .Select(s => new
                    {
                        id_pod = s.FirstOrDefault().id_purchaseOrderDetail,
                        qp = s.Sum(sp => sp.quantity)
                    }).ToList();

            var lstPodQO = db.PurchaseOrderDetail
                                .Where(w => w.PurchaseOrder.Document.DocumentState.code == "06")
                                .GroupBy(g => g.id)
                                .Select(s => new
                                {
                                    id_pod = s.FirstOrDefault().id,
                                    qo = s.Sum(so => so.quantityApproved)
                                }).ToList();

            var lstPodQOResult = (from _qo in lstPodQO
                                  join _qp in lstPodQP on _qo.id_pod equals _qp.id_pod into qol
                                  from _qol in qol.DefaultIfEmpty()
                                  select new
                                  {
                                      id_q = _qo.id_pod,
                                      qo = _qo.qo,
                                      qp = (_qol == null ? 0 : _qol.qp)
                                  }).Distinct().ToList();

            var lstPodQOResultFiltered = lstPodQOResult.Where(w => w.qo > w.qp).Select(s => s.id_q).Distinct().ToList();

            var model = db.PurchaseOrderDetail
                            .Where(d => d.PurchaseOrder.Document.DocumentState.code == "06"
                                        && lstPodQOResultFiltered.Contains(d.id)
                                    )
                          .OrderByDescending(d => d.id_purchaseOrder).ToList().OrderByDescending(d => d.id);

            var idPersonProcessPlant = db.Person.FirstOrDefault(p => p.identification_number == ActiveCompany.ruc)?.id ?? 0;

            foreach (var item in model)
            {
                var aPerson = db.Person.FirstOrDefault(fod => fod.id == (item.PurchaseOrder.id_personProcessPlant != null ? item.PurchaseOrder.id_personProcessPlant : idPersonProcessPlant));
            }

            TempData["modelPod"] = model;
            TempData.Keep("modelPod");

            return PartialView("_PurchaseOrderDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsForReassignmentPartial()
        {
            var model = db.PurchaseOrderDetail
                            .Where(d => d.PurchaseOrder.Document.DocumentState.code == "06"
                                    && (d.quantityApproved - d.quantityDispatched) > 0
                                    && d.PurchaseOrder.requiredLogistic == true
                                    )
                          .OrderByDescending(d => d.id_purchaseOrder).ToList().OrderByDescending(d => d.id);

            return PartialView("_PurchaseOrderDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideListForReassignment()
        {
            if (TempData["PurchaseDetailsForReassignment"] != null)
            {
                TempData.Keep("PurchaseDetailsForReassignment");
            }

            var lst = db.ProductionLotDetailPurchaseDetail
                            .Where(w => w.ProductionLotDetail.ProductionLot.ProductionLotState.code != "09")
                            .Select(s => s.RemissionGuideDetail.RemissionGuide.id)
                            .Distinct()
                            .ToList();

            var model = db.RemissionGuide
                            .Where(w => (w.hasExitPlanctProduction != null ? (bool)w.hasExitPlanctProduction : false)
                                            && !w.RemissionGuideTransportation.isOwn
                                            && w.Document.DocumentState.code == "06"
                                            && !lst.Contains(w.id))
                                            .Select(s => new RGRResultsQuery
                                            {
                                                id = s.id,
                                                numberDoc = s.Document.number,
                                                emissionDateDoc = s.Document.emissionDate,
                                                providerName = s.Provider.Person.fullname_businessName,
                                                productionUnitProviderName = s.ProductionUnitProvider.name,
                                                despachureDateDoc = s.despachureDate,
                                                exitTimePlanctDoc = s.RemissionGuideControlVehicle.exitDateProductionBuilding,
                                                isThird = s.RemissionGuideTransportation.isOwn,
                                                stateDoc = s.Document.DocumentState.name
                                            })
                                            .ToList();

            return PartialView("_RemissionGuideForReassignmentPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideListForReassignmentResults()
        {
            if (TempData["PurchaseDetailsForReassignment"] != null)
            {
                TempData.Keep("PurchaseDetailsForReassignment");
            }

            var lst = db.ProductionLotDetailPurchaseDetail
                            .Where(w => w.ProductionLotDetail.ProductionLot.ProductionLotState.code != "09")
                            .Select(s => s.RemissionGuideDetail.RemissionGuide.id)
                            .Distinct()
                            .ToList();

            var model = db.RemissionGuide
                            .Where(w => (w.hasExitPlanctProduction != null ? (bool)w.hasExitPlanctProduction : false)
                                            && !w.RemissionGuideTransportation.isOwn
                                            && w.Document.DocumentState.code == "06"
                                            && !lst.Contains(w.id))
                                            .Select(s => new RGRResultsQuery
                                            {
                                                id = s.id,
                                                numberDoc = s.Document.number,
                                                emissionDateDoc = s.Document.emissionDate,
                                                providerName = s.Provider.Person.fullname_businessName,
                                                productionUnitProviderName = s.ProductionUnitProvider.name,
                                                despachureDateDoc = s.despachureDate,
                                                exitTimePlanctDoc = s.RemissionGuideControlVehicle.exitDateProductionBuilding,
                                                isThird = s.RemissionGuideTransportation.isOwn,
                                                stateDoc = s.Document.DocumentState.name
                                            })
                                            .ToList();

            return PartialView("_RemissionGuideForReassignmentResultsPartial", model.ToList());
        }

        #endregion REMISSION GUIDE FILTERS RESULTS

        #region REMISSION GUIDE MASTER DETAILS VIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailViewDetails()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideDetailViewDetailsPartial", remissionGuide.RemissionGuideDetail.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailViewDispatchMaterials()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideDetailViewDispatchMaterialsPartial", remissionGuide.RemissionGuideDispatchMaterial.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailViewAssignedStaff()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideDetailViewAssignedStaffPartial", remissionGuide.RemissionGuideAssignedStaff.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailViewSecuritySeals()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideDetailViewSecuritySealsPartial", remissionGuide.RemissionGuideSecuritySeal.ToList());
        }

        public ActionResult ReceptionDispatchMaterialsDetailPartial()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);

            var receptionDispatchMaterials = remissionGuide?.ReceptionDispatchMaterials.FirstOrDefault(p => p.Document.DocumentState.code != "05");
            var model = receptionDispatchMaterials?.ReceptionDispatchMaterialsDetail.OrderBy(od => od.id).ToList() ?? new List<ReceptionDispatchMaterialsDetail>();

            return PartialView("_ReceptionDispatchMaterialsViewsDetailPartial", model);
        }

        #endregion REMISSION GUIDE MASTER DETAILS VIEW

        #region REMISSION GUIDE HEADER

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuidePartial()
        {
            var model = (TempData["model"] as List<RGResultsQuery>);
            model = model ?? new List<RGResultsQuery>();

            TempData.Keep("model");

            return PartialView("_RemissionGuidePartial", model.OrderByDescending(r => r.id).ToList());
        }

        public bool validtateRemision(int? id_shippingType, RemissionGuide tempRemissionGuide, RemissionGuideTransportation tempRemissionGuideTransportation)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            bool wresult = true;
            try
            {
                if (tempRemissionGuide != null)
                {
                    #region"VALIDA VIA DE TRANSPORTE"
                    if (tempRemissionGuide.id_shippingType != null || id_shippingType != null)
                    {
                        var shippi = id_shippingType ?? tempRemissionGuide.id_shippingType;

                        var shippingtype = db.PurchaseOrderShippingType.Where(g => g.id == shippi).FirstOrDefault();

                        if (shippingtype != null && (shippingtype.code == "M" || shippingtype.code == "A"))
                        {
                            ViewData["EditMessage"] = ErrorMessage("La Vía de Transporte no es valida");
                            wresult = false;
                        }
                    }
                    #endregion REMISSION GUIDE HEADER

                    #region"VALIDA SITIO DE UNIDAD DE PRODUCCIÓN DEFINIDO "
                    if (tempRemissionGuide.id_productionUnitProvider != null)
                    {
                        if (tempRemissionGuideTransportation.id_FishingSiteRG > 0)
                        {
                            var TransportTariffDetail = db.TransportTariffDetail.Where(X => X.TransportTariff.id_TransportTariffType == tempRemissionGuide.id_TransportTariffType && X.id_FishingSite == tempRemissionGuideTransportation.id_FishingSiteRG
                                                               && X.isActive && X.TransportTariff.isActive).FirstOrDefault();

                            if (TransportTariffDetail == null)
                            {
                                ViewData["EditMessage"] = ErrorMessage("No Tiene Sitio Definido para el Tipo de Tarifario");
                                wresult = false;
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                TempData.Keep("remissionGuide");
                ViewData["EditMessage"] = ErrorMessage(e.Message);
                wresult = false;
            }

            return wresult;
        }

        public JsonResult ValidateVehicleAnotherRemissionGuideProviderCompany(int? id_vehicle, int id_remissionguide, string despachureHourTmp)
        {
            RemissionGuide returnRemissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            TempData["remissionGuide"] = returnRemissionGuide;
            TempData.Keep("remissionGuide");

            DateTime dtDespachureHour = new DateTime();

            if (despachureHourTmp != null)
            {
                DateTime.TryParse(despachureHourTmp, out dtDespachureHour);
            }

            var result = new
            {
                itsAssigned = "NO",
                noneProvider = "NO",
                noneProviderBilling = "NO",
                Error1 = "",
                Error2 = "",
                Error3 = ""
            };
            var vpt = db.VeicleProviderTransport.FirstOrDefault(fod => fod.id_vehicle == id_vehicle &&
                                                                                        fod.datefin == null &&
                                                                                        fod.Estado == true);

            var vptBilling = db.VehicleProviderTransportBilling.FirstOrDefault(fod => fod.id_vehicle == id_vehicle &&
                                                                                        fod.datefin == null &&
                                                                                        fod.state == true);
            var vcont = db.RemissionGuideTransportation
                                .Where(g => g.id_vehicle == id_vehicle &&
                                            g.id_remionGuide != returnRemissionGuide.id
                                            && g.isOwn == false
                                            && g.RemissionGuide.hasEntrancePlanctProduction != true
                                            && g.RemissionGuide.Document.DocumentState.code != "05").ToList().Count;

            string lstVehicles = "";
            if (vcont > 0)
            {
                lstVehicles = string.Join(", ", db.RemissionGuideTransportation
                    .Where(g => g.id_vehicle == id_vehicle &&
                                g.id_remionGuide != returnRemissionGuide.id
                                && g.isOwn == false
                                && g.RemissionGuide.hasEntrancePlanctProduction != true
                                && g.RemissionGuide.Document.DocumentState.code != "05")
                                .Select(s => s.RemissionGuide.Document.number)
                                .DefaultIfEmpty("").ToArray());
            }

            result = new
            {
                itsAssigned = (vcont > 0) ? "YES" : "NO",
                noneProvider = (vpt == null) ? "YES" : "NO",
                noneProviderBilling = (vptBilling == null) ? "YES" : "NO",
                Error1 = (vcont > 0) ? "El Vehiculo ya Esta registrado en Otra Guía " + lstVehicles : "",
                Error2 = (vpt == null) ? "El Vehículo no tiene proveedor de transporte" : "",
                Error3 = (vptBilling == null) ? "El Vehículo no tiene compañía que factura" : ""
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult RemissionGuidePartialAddNew(bool approve, RemissionGuide item,
                                                        RemissionGuideCustomizedInformation itemCi,
                                                        RemissionGuideCustomizedIceBuyInformation itemCustIceBuy,
                                                        Document document,
                                                        RemissionGuideTransportation transportation,
                                                        RemissionGuideExportInformation exportInformation, string despachurehour)
        {
            _btnOnEditFormRG = new ButtonsOnEditFormRemissionGuide();
            _AnswerfaRG = new AnswerForActionRemissionGuide();
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }

            #region Asignacion de Guia
            RemissionGuide returnRemissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            var id_providerreturn = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle).FirstOrDefault()?.id_Provider;
            transportation.id_provider = id_providerreturn;

            if (transportation.id_vehicle != null)
            {
                transportation.Vehicle = transportation.Vehicle = db.Vehicle.Where(X => X.id == transportation.id_vehicle).FirstOrDefault();
            }

            returnRemissionGuide.RemissionGuideTransportation = transportation;
            returnRemissionGuide.id_shippingType = item.id_shippingType;
            returnRemissionGuide.id_TransportTariffType = item.id_TransportTariffType;

            returnRemissionGuide.arrivalDate = returnRemissionGuide.arrivalDate != null ? returnRemissionGuide.arrivalDate : item.arrivalDate;
            returnRemissionGuide.descriptionpurchaseorder = returnRemissionGuide.descriptionpurchaseorder != null ? returnRemissionGuide.descriptionpurchaseorder : item.descriptionpurchaseorder;
            returnRemissionGuide.despachureDate = returnRemissionGuide.despachureDate != null ? returnRemissionGuide.despachureDate : item.despachureDate;
            if (!String.IsNullOrEmpty(despachurehour)) returnRemissionGuide.despachurehour = TimeSpan.Parse(despachurehour.Substring(10).ToString());
            returnRemissionGuide.hasEntrancePlanctProduction = returnRemissionGuide.hasEntrancePlanctProduction != null ? returnRemissionGuide.hasEntrancePlanctProduction : item.hasEntrancePlanctProduction;
            returnRemissionGuide.hasExitPlanctProduction = returnRemissionGuide.hasExitPlanctProduction != null ? returnRemissionGuide.hasExitPlanctProduction : item.hasExitPlanctProduction;
            returnRemissionGuide.id_buyer = returnRemissionGuide.id_buyer != null ? returnRemissionGuide.id_buyer : item.id_buyer;
            returnRemissionGuide.id_priceList = returnRemissionGuide.id_priceList != null ? returnRemissionGuide.id_priceList : item.id_priceList;
            returnRemissionGuide.id_certification = returnRemissionGuide.id_certification != null ? returnRemissionGuide.id_certification : item.id_certification;
            returnRemissionGuide.id_productionUnitProvider = returnRemissionGuide.id_productionUnitProvider != null ? returnRemissionGuide.id_productionUnitProvider : item.id_productionUnitProvider;
            if (returnRemissionGuide.id_productionUnitProvider > 0)
            {
                returnRemissionGuide.ProductionUnitProvider = db.ProductionUnitProvider.Where(x => x.id == returnRemissionGuide.id_productionUnitProvider).FirstOrDefault();
            }
            returnRemissionGuide.id_productionUnitProviderPool = returnRemissionGuide.id_productionUnitProviderPool != null ? returnRemissionGuide.id_productionUnitProviderPool : item.id_productionUnitProviderPool;
            returnRemissionGuide.id_protectiveProvider = returnRemissionGuide.id_protectiveProvider != null ? returnRemissionGuide.id_protectiveProvider : item.id_protectiveProvider;
            returnRemissionGuide.id_providerRemisionGuide = returnRemissionGuide.id_providerRemisionGuide != null ? returnRemissionGuide.id_providerRemisionGuide : item.id_providerRemisionGuide;

            returnRemissionGuide.id_reason = returnRemissionGuide.id_reason > 0 ? returnRemissionGuide.id_reason : item.id_reason;
            returnRemissionGuide.id_reciver = returnRemissionGuide.id_reciver > 0 ? returnRemissionGuide.id_reciver : item.id_reciver;
            returnRemissionGuide.id_RemisionGuideReassignment = returnRemissionGuide.id_RemisionGuideReassignment > 0 ? returnRemissionGuide.id_RemisionGuideReassignment : item.id_RemisionGuideReassignment;
            returnRemissionGuide.id_shippingType = returnRemissionGuide.id_shippingType > 0 ? returnRemissionGuide.id_shippingType : item.id_shippingType;
            returnRemissionGuide.id_TransportTariffType = returnRemissionGuide.id_TransportTariffType > 0 ? returnRemissionGuide.id_TransportTariffType : item.id_TransportTariffType;
            returnRemissionGuide.isExport = false;
            returnRemissionGuide.isInternal = false;
            returnRemissionGuide.returnDate = returnRemissionGuide.returnDate != null ? returnRemissionGuide.returnDate : item.returnDate;
            returnRemissionGuide.route = returnRemissionGuide.route != null ? returnRemissionGuide.route : item.route;
            returnRemissionGuide.startAdress = returnRemissionGuide.startAdress != null ? returnRemissionGuide.startAdress : item.startAdress;
            returnRemissionGuide.uniqueCustomDocument = returnRemissionGuide.uniqueCustomDocument != null ? returnRemissionGuide.uniqueCustomDocument : item.uniqueCustomDocument;
            returnRemissionGuide.RemissionGuideExportInformation = returnRemissionGuide.RemissionGuideExportInformation != null ? returnRemissionGuide.RemissionGuideExportInformation : exportInformation;
            #endregion

            decimal flete = 0;
            int _answer = 0;
            int _answerCloseDoc = 0;
            if (transportation != null && !transportation.isOwn)
            {
                if (transportation != null && transportation.id_FishingSiteRG > 0
                    && item != null && item.id_TransportTariffType > 0)
                {
                    RGParamPriceFreight rgPpf = new RGParamPriceFreight();
                    rgPpf.id_FishingSite = transportation.id_FishingSiteRG;
                    rgPpf.id_TransportTariff = item.id_TransportTariffType;
                    flete = CalculatePriceFreight(rgPpf);
                }
            }

            RemissionGuide tempRemissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            tempRemissionGuide = tempRemissionGuide ?? new RemissionGuide();
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            bool hasHDTmp = false;
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region Document

                    document.id_userCreate = ActiveUser.id;
                    document.dateCreate = DateTime.Now;
                    document.id_userUpdate = ActiveUser.id;
                    document.dateUpdate = DateTime.Now;

                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == document.id_documentType);
                    DocumentType documentType2 = db.DocumentType.FirstOrDefault(fod => fod.code == "69");

                    int id_ep = 0;
                    if (TempData["id_ep"] != null)
                    {
                        id_ep = (int)TempData["id_ep"];
                    }
                    id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                    document.sequential = documentType?.currentNumber ?? 0;
                    document.number = GetDocumentNumber(document.id_documentType, id_ep);
                    document.DocumentType = documentType;

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == document.id_documentState);
                    document.DocumentState = documentState;

                    EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_ep);
                    document.id_emissionPoint = emissionPoint?.id ?? 0;
                    document.EmissionPoint = emissionPoint;

                    string emissionDate = document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");
                    string despDate = item.despachureDate.ToString("dd/MM/yyyy").Replace("/", "");

                    var id_company = document.EmissionPoint.id_company;

                    Company vrCompa = db.Company.Where(h => h.id == id_company).FirstOrDefault();
                    var enviromentCode = vrCompa.CompanyElectronicFacturation.EnvironmentType.codeSRI.ToString();

                    document.accessKey = AccessKey.GenerateAccessKey(emissionDate,
                                                                    document.DocumentType.codeSRI,
                                                                    document.EmissionPoint.BranchOffice.Division.Company.ruc,
                                                                     enviromentCode,
                                                                    document.EmissionPoint.BranchOffice.code.PadLeft(3, '0') + document.EmissionPoint.code.ToString("D3"),
                                                                    document.sequential.ToString("D9"),
                                                                    document.sequential.ToString("D8"),
                                                                    "1");

                    //Actualiza Secuencial
                    if (documentType != null)
                    {
                        documentType.currentNumber = documentType.currentNumber + 1;
                        db.DocumentType.Attach(documentType);
                        db.Entry(documentType).State = EntityState.Modified;
                    }
                    if (documentType2 != null)
                    {
                        documentType2.currentNumber = documentType2.currentNumber + 1;
                        db.DocumentType.Attach(documentType2);
                        db.Entry(documentType2).State = EntityState.Modified;
                    }

                    #endregion

                    #region RemissionGuide

                    item.despachureDate = item.despachureDate;
                    item.arrivalDate = item.arrivalDate;
                    item.returnDate = item.returnDate;
                    item.isInternal = false;
                    item.id_providerRemisionGuide = item.id_providerRemisionGuide;
                    item.Document = document;
                    item.id_RemisionGuideReassignment = item.id_RemisionGuideReassignment;
                    item.id_shippingType = item.id_shippingType;
                    item.isCopackingRG = item.isCopackingRG;

                    #region TEMPORAL REMISSION GUIDE TYPE
                    PurchaseOrderShippingType postTmp = db.PurchaseOrderShippingType.FirstOrDefault(fod => fod.id == item.id_shippingType);

                    RemissionGuideType rgtTmp = new RemissionGuideType();
                    if (postTmp != null)
                    {
                        if (postTmp.code == "T")
                        {
                            rgtTmp = db.RemissionGuideType.FirstOrDefault(fod => fod.code == "T");
                        }
                        else if (postTmp.code == "M")
                        {
                            rgtTmp = db.RemissionGuideType.FirstOrDefault(fod => fod.code == "F");
                        }
                        else if (postTmp.code == "TF")
                        {
                            rgtTmp = db.RemissionGuideType.FirstOrDefault(fod => fod.code == "TF");
                        }
                    }
                    item.id_RemissionGuideType = rgtTmp?.id ?? (int)item.id_shippingType;
                    #endregion

                    if (!String.IsNullOrEmpty(despachurehour)) item.despachurehour = TimeSpan.Parse(despachurehour.Substring(10).ToString());
                    item.id_TransportTariffType = item.id_TransportTariffType;

                    #endregion

                    #region REMISSION GUIDE TRANSPORTATION
                    if (returnRemissionGuide != null && returnRemissionGuide.RemissionGuideTransportation != null && returnRemissionGuide.RemissionGuideTransportation.isOwn == true)
                    {
                        item.RemissionGuideTransportation = item.RemissionGuideTransportation ?? new RemissionGuideTransportation();

                        item.RemissionGuideTransportation.isOwn = true;
                        if (transportation != null)
                        {
                            item.RemissionGuideTransportation.driverName = transportation.driverNameThird ?? "";
                            item.RemissionGuideTransportation.carRegistration = transportation.carRegistrationThird ?? "";
                            item.RemissionGuideTransportation.descriptionTrans = transportation.descriptionTrans ?? "";
                            item.RemissionGuideTransportation.id_FishingSiteRG = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == item.id_productionUnitProvider)?.id_FishingSite ?? null;
                        }
                    }
                    else if (transportation != null)
                    {
                        item.RemissionGuideTransportation = item.RemissionGuideTransportation ?? new RemissionGuideTransportation();

                        if (item.isInternal != null && (bool)item.isInternal)
                        {
                            transportation.advancePrice = 0;
                            transportation.valuePrice = 0;
                        }
                        else
                        {
                            transportation.valuePrice = flete;
                        }
                        var id_provider = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle && x.datefin == null && x.Estado).FirstOrDefault()?.id_Provider;
                        transportation.id_provider = id_provider;

                        var id_VeicleProviderTransport = db.VeicleProviderTransport
                                                            .Where(x => x.id_vehicle == transportation.id_vehicle && x.datefin == null && x.Estado).FirstOrDefault()?.id;
                        var id_VehicleProviderTransportBilling = db.VehicleProviderTransportBilling.FirstOrDefault(fod => fod.id_vehicle == transportation.id_vehicle && fod.datefin == null && fod.state)?.id;

                        if (id_VeicleProviderTransport != null)
                        {
                            var DriverVeicleProviderTransportAx = db.DriverVeicleProviderTransport.Where(x => x.idVeicleProviderTransport == id_VeicleProviderTransport && x.id_driver == transportation.id_driver).FirstOrDefault();

                            if (DriverVeicleProviderTransportAx != null)
                            {
                                transportation.id_DriverVeicleProviderTransport = DriverVeicleProviderTransportAx.id;
                            }
                            else
                            {
                                DriverVeicleProviderTransport driverVeicleProviderTransport = new DriverVeicleProviderTransport()
                                {
                                    Estado = true,
                                    idVeicleProviderTransport = id_VeicleProviderTransport,
                                    id_driver = transportation.id_driver,
                                };
                                transportation.DriverVeicleProviderTransport = driverVeicleProviderTransport;
                            }
                        }
                        transportation.id_VehicleProviderTranportistBilling = id_VehicleProviderTransportBilling;

                        item.RemissionGuideTransportation = transportation;
                    }
                    #endregion

                    #region Remission Guide Export Information

                    if (item.isExport && !string.IsNullOrEmpty(exportInformation?.uniqueCustomsDocument))
                    {
                        item.RemissionGuideExportInformation = new RemissionGuideExportInformation
                        {
                            uniqueCustomsDocument = exportInformation.uniqueCustomsDocument
                        };
                    }

                    #endregion

                    #region Valida Producto por Guia
                    if (tempRemissionGuide?.RemissionGuideDetail != null)
                    {
                        if (item.isInternal != true)
                        {
                            var details = tempRemissionGuide.RemissionGuideDetail.ToList();

                            int id_inventoryLine = db.InventoryLine.Where(x => x.code == "MP").FirstOrDefault().id;

                            var conut = details.Where(x => x.Item.id_inventoryLine != id_inventoryLine && x.isActive).ToList().Count;

                            if (conut > 0)

                            {
                                TempData["remissionGuide"] = returnRemissionGuide;
                                TempData.Keep("remissionGuide");
                                ViewData["EditMessage"] = ErrorMessage("No Todos Los Productos de ingresado en son de Materia Prima, favor revisar...");
                                _messageAnswer = ErrorMessage("No Todos Los Productos de ingresado en son de Materia Prima, favor revisar...");
                                _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(item.id, _codeStateDocument);
                                _AnswerfaRG.idEntity = item.id;
                                _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                                _AnswerfaRG.messageAnswer = _messageAnswer;
                                _AnswerfaRG.nameStateAnswer = _nameStateDocument;
                                _AnswerfaRG.codeStateAnswer = _codeStateDocument;
                                _AnswerfaRG.strNumberDocAnswer = _strNumberDocAnswer;
                                return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    var advancePrice = transportation.advancePrice ?? 0;
                    var valuePrice = transportation.valuePrice ?? 0;

                    #endregion

                    #region REMISSION GUIDE DETAILS

                    if (tempRemissionGuide?.RemissionGuideDetail != null)
                    {
                        item.RemissionGuideDetail = new List<RemissionGuideDetail>();

                        if (item != null && item.RemissionGuideTransportation != null && item.RemissionGuideTransportation.isOwn == false)
                        {
                            item.RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>();
                        }

                        var details = tempRemissionGuide.RemissionGuideDetail.ToList();

                        foreach (var detail in details)
                        {
                            var remissionGuideDetail = new RemissionGuideDetail
                            {
                                id_item = detail.id_item,
                                Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                                id_businessOportunityPlanningDetail = detail.id_businessOportunityPlanningDetail,
                                BusinessOportunityPlanningDetail = db.BusinessOportunityPlanningDetail.FirstOrDefault(i => i.id == detail.id_businessOportunityPlanningDetail),

                                quantityOrdered = detail.quantityOrdered,
                                quantityProgrammed = detail.quantityProgrammed,
                                quantityDispatchPending = detail.quantityDispatchPending,
                                quantityReceived = detail.quantityReceived,
                                productionUnitProviderPoolreference = detail.productionUnitProviderPoolreference ?? "",
                                RemissionGuideDetailPurchaseOrderDetail = detail.RemissionGuideDetailPurchaseOrderDetail,

                                RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>(),

                                isActive = detail.isActive,
                                id_userCreate = detail.id_userCreate,
                                dateCreate = detail.dateCreate,
                                id_userUpdate = detail.id_userUpdate,
                                dateUpdate = detail.dateUpdate,
                                id_Grammage = detail.id_Grammage
                            };

                            item.RemissionGuideDetail.Add(remissionGuideDetail);

                            if (item != null && item.RemissionGuideTransportation != null && item.RemissionGuideTransportation.isOwn == false)
                            {
                                #region "Remission Guide Detail Dispatch Material Detail"
                                foreach (var remissionGuideDetailDispatchMaterialDetail in detail.RemissionGuideDetailDispatchMaterialDetail)
                                {
                                    var remissionGuideDispatchMaterialAux = item.RemissionGuideDispatchMaterial.FirstOrDefault(d => d.id == remissionGuideDetailDispatchMaterialDetail.id_remissionGuideDispatchMaterial);
                                    if (remissionGuideDispatchMaterialAux == null)
                                    {
                                        var detailRemissionGuideDispatchMaterial = tempRemissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id == remissionGuideDetailDispatchMaterialDetail.id_remissionGuideDispatchMaterial);
                                        remissionGuideDispatchMaterialAux = new RemissionGuideDispatchMaterial
                                        {
                                            id = detailRemissionGuideDispatchMaterial.id,
                                            id_item = detailRemissionGuideDispatchMaterial.id_item,
                                            Item = db.Item.FirstOrDefault(i => i.id == detailRemissionGuideDispatchMaterial.id_item),

                                            quantityRequiredForPurchase = detailRemissionGuideDispatchMaterial.quantityRequiredForPurchase,
                                            sourceExitQuantity = detailRemissionGuideDispatchMaterial.sourceExitQuantity,
                                            sendedDestinationQuantity = detailRemissionGuideDispatchMaterial.sendedDestinationQuantity,
                                            arrivalDestinationQuantity = detailRemissionGuideDispatchMaterial.arrivalDestinationQuantity,
                                            arrivalGoodCondition = detailRemissionGuideDispatchMaterial.arrivalGoodCondition,
                                            arrivalBadCondition = detailRemissionGuideDispatchMaterial.arrivalBadCondition,

                                            RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>(),

                                            manual = detailRemissionGuideDispatchMaterial.manual,
                                            isActive = detailRemissionGuideDispatchMaterial.isActive,
                                            id_userCreate = detailRemissionGuideDispatchMaterial.id_userCreate,
                                            dateCreate = detailRemissionGuideDispatchMaterial.dateCreate,
                                            id_userUpdate = detailRemissionGuideDispatchMaterial.id_userUpdate,
                                            dateUpdate = detailRemissionGuideDispatchMaterial.dateUpdate
                                        };

                                        item.RemissionGuideDispatchMaterial.Add(remissionGuideDispatchMaterialAux);
                                    }

                                    var newRemissionGuideDetailDispatchMaterialDetail = new RemissionGuideDetailDispatchMaterialDetail
                                    {
                                        id_remissionGuideDispatchMaterial = remissionGuideDispatchMaterialAux.id,
                                        RemissionGuideDispatchMaterial = remissionGuideDispatchMaterialAux,
                                        id_remissionGuideDetail = remissionGuideDetail.id,
                                        quantity = remissionGuideDetailDispatchMaterialDetail.quantity
                                    };
                                    remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail.Add(newRemissionGuideDetailDispatchMaterialDetail);
                                    remissionGuideDispatchMaterialAux.RemissionGuideDetailDispatchMaterialDetail.Add(newRemissionGuideDetailDispatchMaterialDetail);
                                }
                                #endregion
                            }
                        }
                    }

                    if (item.RemissionGuideDetail.Count == 0)
                    {
                        TempData["remissionGuide"] = returnRemissionGuide;
                        TempData.Keep("remissionGuide");
                        ViewData["EditMessage"] = ErrorMessage("No se puede guardar una guía de remisión sin detalles");
                        _messageAnswer = ErrorMessage("No se puede guardar una guía de remisión sin detalles");
                        _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(item.id, _codeStateDocument);
                        _AnswerfaRG.idEntity = item.id;
                        _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                        _AnswerfaRG.messageAnswer = _messageAnswer;
                        _AnswerfaRG.nameStateAnswer = _nameStateDocument;
                        _AnswerfaRG.codeStateAnswer = _codeStateDocument;
                        _AnswerfaRG.strNumberDocAnswer = _strNumberDocAnswer;
                        return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                    }

                    #endregion

                    if (returnRemissionGuide != null && returnRemissionGuide.RemissionGuideTransportation != null && returnRemissionGuide.RemissionGuideTransportation.isOwn == false)
                    {
                        #region REMISSION GUIDE DISPATCH MATERIALS

                        if (tempRemissionGuide?.RemissionGuideDispatchMaterial != null)
                        {
                            if (item.RemissionGuideDispatchMaterial == null)
                            {
                                item.RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>();
                            }

                            var details = tempRemissionGuide.RemissionGuideDispatchMaterial.ToList();

                            foreach (var detail in details)
                            {
                                if ((detail.RemissionGuideDetailDispatchMaterialDetail?.Count() ?? 0) > 0)
                                {
                                    continue;
                                }
                                var id_ware = db.Item.FirstOrDefault(i => i.id == detail.id_item)?.ItemInventory?.id_warehouse;
                                var id_warelo = (int)db.Item.FirstOrDefault(fod => fod.id == detail.id_item)?.ItemInventory?.id_warehouseLocation;
                                var remissionGuideDispatchMaterial = new RemissionGuideDispatchMaterial
                                {
                                    id_item = detail.id_item,
                                    Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                                    quantityRequiredForPurchase = detail.quantityRequiredForPurchase,
                                    sourceExitQuantity = detail.sourceExitQuantity,
                                    sendedDestinationQuantity = detail.sendedDestinationQuantity,
                                    arrivalDestinationQuantity = detail.arrivalDestinationQuantity,
                                    arrivalGoodCondition = detail.arrivalGoodCondition,
                                    arrivalBadCondition = detail.arrivalBadCondition,
                                    id_warehouse = id_ware,
                                    id_warehouselocation = id_warelo,
                                    Warehouse = db.Warehouse.FirstOrDefault(w => w.id == id_ware),
                                    manual = detail.manual,
                                    isActive = detail.isActive,
                                    id_userCreate = detail.id_userCreate,
                                    dateCreate = detail.dateCreate,
                                    id_userUpdate = detail.id_userUpdate,
                                    dateUpdate = detail.dateUpdate
                                };

                                item.RemissionGuideDispatchMaterial.Add(remissionGuideDispatchMaterial);
                            }
                            var it = item.RemissionGuideDispatchMaterial.ToList();
                            foreach (var d in it)
                            {
                                var id_war = db.Item.FirstOrDefault(i => i.id == d.id_item)?.ItemInventory?.id_warehouse;
                                var id_warlo = (int)db.Item.FirstOrDefault(fod => fod.id == d.id_item)?.ItemInventory?.id_warehouseLocation;
                                d.id_warehouse = id_war;
                                d.id_warehouselocation = id_warlo;
                                if (d.sourceExitQuantity < 0) d.sourceExitQuantity = 0;
                            }
                        }

                        #endregion
                    }

                    #region REMISSION GUIDE SECURITY SEALS

                    if (tempRemissionGuide?.RemissionGuideSecuritySeal != null)
                    {
                        var details = tempRemissionGuide.RemissionGuideSecuritySeal.ToList();

                        foreach (var detail in details)
                        {
                            var remissionGuideSecuritySeal = new RemissionGuideSecuritySeal
                            {
                                number = detail.number,

                                id_travelType = detail.id_travelType,
                                RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType),

                                id_exitState = detail.id_exitState,
                                SecuritySealState = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_exitState),
                                id_arrivalState = detail.id_arrivalState,
                                SecuritySealState1 = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_arrivalState),

                                isActive = detail.isActive,
                                id_userCreate = detail.id_userCreate,
                                dateCreate = detail.dateCreate,
                                id_userUpdate = detail.id_userUpdate,
                                dateUpdate = detail.dateUpdate
                            };

                            item.RemissionGuideSecuritySeal.Add(remissionGuideSecuritySeal);
                        }
                    }

                    #endregion

                    #region REMISSION GUIDE ASSIGNED STAFF

                    if (tempRemissionGuide?.RemissionGuideAssignedStaff != null)
                    {
                        var details = tempRemissionGuide.RemissionGuideAssignedStaff.ToList();

                        foreach (var detail in details)
                        {
                            var remissionGuideAssignedStaff = new RemissionGuideAssignedStaff
                            {
                                id_person = detail.id_person,
                                Person = db.Person.FirstOrDefault(p => p.id == detail.id_person),

                                id_assignedStaffRol = detail.id_assignedStaffRol,
                                RemissionGuideAssignedStaffRol = db.RemissionGuideAssignedStaffRol.FirstOrDefault(r => r.id == detail.id_assignedStaffRol),

                                id_travelType = detail.id_travelType,
                                RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType),
                                viaticPrice = detail.viaticPrice ?? 0,
                                isActive = detail.isActive,
                                id_userCreate = detail.id_userCreate,
                                dateCreate = detail.dateCreate,
                                id_userUpdate = detail.id_userUpdate,
                                dateUpdate = detail.dateUpdate
                            };

                            item.RemissionGuideAssignedStaff.Add(remissionGuideAssignedStaff);
                        }
                    }

                    #endregion

                    #region"CUANDO SE MANDA A APROBAR DIRECTAMENTE"
                    if (approve)
                    {
                        foreach (var detail in item.RemissionGuideDetail)
                        {
                            foreach (var remissionGuideDetailPurchaseOrderDetail in detail.RemissionGuideDetailPurchaseOrderDetail)
                            {
                                ServicePurchaseRemission.UpdateQuantityDispatchedPurchaseOrderDetailRemissionGuide(db, remissionGuideDetailPurchaseOrderDetail.id_purchaseOrderDetail,
                                                                                remissionGuideDetailPurchaseOrderDetail.id_remissionGuideDetail,
                                                                                remissionGuideDetailPurchaseOrderDetail.quantity);
                            }
                        }
                        if (item.RemissionGuideTransportation.isOwn == false)
                        {
                            EmissionPoint _epTmp = db.EmissionPoint.FirstOrDefault(fod => fod.id == id_ep);
                            foreach (var rgdm in item.RemissionGuideDispatchMaterial.ToList())
                            {
                                if (rgdm.InventoryMoveDetailExitDispatchMaterials != null)
                                {
                                    var quantity = rgdm.InventoryMoveDetailExitDispatchMaterials.Sum(s => s.quantity);
                                    rgdm.sendedDestinationQuantity = quantity;
                                }
                                else
                                {
                                    rgdm.sendedDestinationQuantity = 0;
                                    db.Entry(rgdm).State = EntityState.Modified;
                                }
                            }
                        }
                        item.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03");
                    }
                    #endregion

                    #region"DATA HELP FOR REMISSION GUIDE REPORT"

                    item.RemissionGuideReportLinealDataHelp = item.RemissionGuideReportLinealDataHelp ?? new RemissionGuideReportLinealDataHelp();

                    //Consulto Vehiculo Hunter
                    if (item.RemissionGuideTransportation != null)
                    {
                        int id_vehTmp = item.RemissionGuideTransportation.id_vehicle ?? 0;
                        if (id_vehTmp > 0)
                        {
                            hasHDTmp = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehTmp)?.hasHunterDevice ?? false;
                            if (hasHDTmp)
                            {
                                item.RemissionGuideReportLinealDataHelp.nameSecurityPerson = db.Setting.FirstOrDefault(fod => fod.code == "NDHL")?.value ?? "";
                            }
                        }
                    }
                    //Person Asignado
                    int id_SegRol = db.RemissionGuideAssignedStaffRol
                                        .FirstOrDefault(fod => fod.code == "SEG")?.id ?? 0;
                    int id_SegTec = db.RemissionGuideAssignedStaffRol
                                        .FirstOrDefault(fod => fod.code == "BIO")?.id ?? 0;

                    int id_rgasSeg = item
                                    .RemissionGuideAssignedStaff
                                    .FirstOrDefault(fod => fod.id_assignedStaffRol == id_SegRol && fod.isActive)?.id_person ?? 0;

                    int id_rgasTec = item
                                        .RemissionGuideAssignedStaff
                                        .FirstOrDefault(fod => fod.id_assignedStaffRol == id_SegTec && fod.isActive)?.id_person ?? 0;
                    //SEGURIDAD
                    if (id_rgasSeg > 0)
                    {
                        item.RemissionGuideReportLinealDataHelp.id_SecurityPerson = id_rgasSeg;
                        item.RemissionGuideReportLinealDataHelp.nameSecurityPerson += (hasHDTmp ? "/" : "") + db.Person.FirstOrDefault(fod => fod.id == id_rgasSeg)?.fullname_businessName ?? "";
                    }
                    //BIOLOGO
                    if (id_rgasTec > 0)
                    {
                        item.RemissionGuideReportLinealDataHelp.id_TechnicianPerson = id_rgasTec;
                        item
                            .RemissionGuideReportLinealDataHelp
                            .nameTechnicianPerson = db.Person.FirstOrDefault(fod => fod.id == id_rgasTec)?.fullname_businessName ?? "";
                    }
                    //SELLO DE SEGURIDAD
                    int str_SellOut = db.RemissionGuideTravelType
                                        .FirstOrDefault(fod => fod.code == "IDA")?.id ?? 0;
                    int str_SellIn = db.RemissionGuideTravelType
                                        .FirstOrDefault(fod => fod.code == "REGRESO")?.id ?? 0;

                    item
                        .RemissionGuideReportLinealDataHelp
                        .SealNumberOneExit = item.RemissionGuideSecuritySeal.FirstOrDefault(fod => fod.id_travelType == str_SellOut && fod.isActive)?.number ?? "";

                    item
                        .RemissionGuideReportLinealDataHelp
                        .SealNumberTwoEntrance = item.RemissionGuideSecuritySeal.FirstOrDefault(fod => fod.id_travelType == str_SellIn && fod.isActive)?.number ?? "";
                    #endregion

                    #region PERSONALIZATION DETAIL BY DOCUMENT TYPE
                    List<tbsysDocumentsPersonalizationDetail> lstDocPersDetail = db.tbsysDocumentsPersonalizationDetail
                                                            .Where(fod => fod.id_DocumentType == documentType.id)
                                                            .ToList();

                    if (lstDocPersDetail != null && lstDocPersDetail.Count > 0)
                    {
                        foreach (var det in lstDocPersDetail)
                        {
                            #region INFORMACION CUSTOMIZABLE
                            if (det.nameObjectTable == "RemissionGuideCustomizedInformation")
                            {
                                item.RemissionGuideCustomizedInformation = item.RemissionGuideCustomizedInformation ?? new RemissionGuideCustomizedInformation();
                                item.RemissionGuideCustomizedInformation.hasSecurity = itemCi?.hasSecurity ?? false;
                            }
                            #endregion

                            #region INFORMACION CUSTOMIZABLE VIATICO PERSONAL ASIGNADO
                            if (det.nameObjectTable == "RemissionGuideCustomizedViaticPersonalAssigned")
                            {
                                int iQuantityPersonalAssigned = item?.RemissionGuideAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).ToList().Count() ?? 0;
                                if (iQuantityPersonalAssigned > 0)
                                {
                                    item.RemissionGuideCustomizedViaticPersonalAssigned = item.RemissionGuideCustomizedViaticPersonalAssigned ?? new RemissionGuideCustomizedViaticPersonalAssigned();
                                    item.RemissionGuideCustomizedViaticPersonalAssigned.Document = new Document();

                                    #region CREACION DEL DOCUMENTO
                                    Document docViaticPersonal = new Document();

                                    docViaticPersonal.id_userCreate = ActiveUser.id;
                                    docViaticPersonal.dateCreate = DateTime.Now;
                                    docViaticPersonal.id_userUpdate = ActiveUser.id;
                                    docViaticPersonal.dateUpdate = DateTime.Now;

                                    docViaticPersonal.emissionDate = item.Document.emissionDate;

                                    DocumentType docTypeViaticPersonal = db.DocumentType.FirstOrDefault(fod => fod.code == "77");

                                    docViaticPersonal.sequential = docTypeViaticPersonal?.currentNumber ?? 0;
                                    docViaticPersonal.number = GetDocumentNumber(docTypeViaticPersonal.id, id_ep);
                                    docViaticPersonal.DocumentType = docTypeViaticPersonal;

                                    DocumentState docStateViaticPersonal = db.DocumentState
                                                                                .FirstOrDefault(s => s.code == "01"
                                                                                && s.tbsysDocumentTypeDocumentState
                                                                                .Any(a => a.DocumentType.code == "77"));
                                    docViaticPersonal.DocumentState = docStateViaticPersonal;
                                    docViaticPersonal.id_documentState = docStateViaticPersonal.id;

                                    docViaticPersonal.id_emissionPoint = emissionPoint?.id ?? 0;
                                    docViaticPersonal.EmissionPoint = emissionPoint;

                                    //Actualiza Secuencial Viatico Personal Asignado
                                    if (docTypeViaticPersonal != null)
                                    {
                                        docTypeViaticPersonal.currentNumber = docTypeViaticPersonal.currentNumber + 1;
                                        db.DocumentType.Attach(docTypeViaticPersonal);
                                        db.Entry(docTypeViaticPersonal).State = EntityState.Modified;
                                    }
                                    item.RemissionGuideCustomizedViaticPersonalAssigned.Document = docViaticPersonal;
                                    #endregion

                                    item.RemissionGuideCustomizedViaticPersonalAssigned.isActive = true;
                                    item.RemissionGuideCustomizedViaticPersonalAssigned.hasPayment = false;
                                    item.RemissionGuideCustomizedViaticPersonalAssigned.valueTotal = item?.RemissionGuideAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() ?? 0;
                                }
                            }
                            #endregion

                            #region INFORMACION CUSTOMIZABLE ANTICIPO A TRANSPORTISTA
                            if (det.nameObjectTable == "RemissionGuideCustomizedAdvancedTransportist")
                            {
                                if (item.RemissionGuideTransportation?.advancePrice > 0)
                                {
                                    item.RemissionGuideCustomizedAdvancedTransportist = item.RemissionGuideCustomizedAdvancedTransportist ?? new RemissionGuideCustomizedAdvancedTransportist();
                                    item.RemissionGuideCustomizedAdvancedTransportist.Document = new Document();

                                    #region CREACION DEL DOCUMENTO
                                    Document docAdvanceTransportist = new Document();

                                    docAdvanceTransportist.id_userCreate = ActiveUser.id;
                                    docAdvanceTransportist.dateCreate = DateTime.Now;
                                    docAdvanceTransportist.id_userUpdate = ActiveUser.id;
                                    docAdvanceTransportist.dateUpdate = DateTime.Now;

                                    docAdvanceTransportist.emissionDate = item.Document.emissionDate;

                                    DocumentType docTypeAdvanceTransportist = db.DocumentType.FirstOrDefault(fod => fod.code == "78");

                                    docAdvanceTransportist.sequential = docTypeAdvanceTransportist?.currentNumber ?? 0;
                                    docAdvanceTransportist.number = GetDocumentNumber(docTypeAdvanceTransportist.id, id_ep);
                                    docAdvanceTransportist.DocumentType = docTypeAdvanceTransportist;

                                    DocumentState docStateAdvanceTransportis = db.DocumentState
                                                                                .FirstOrDefault(s => s.code == "01"
                                                                                && s.tbsysDocumentTypeDocumentState
                                                                                .Any(a => a.DocumentType.code == "78"));

                                    docAdvanceTransportist.DocumentState = docStateAdvanceTransportis;
                                    docAdvanceTransportist.id_documentState = docStateAdvanceTransportis.id;

                                    docAdvanceTransportist.id_emissionPoint = emissionPoint?.id ?? 0;
                                    docAdvanceTransportist.EmissionPoint = emissionPoint;

                                    //Actualiza Secuencial Viatico Personal Asignado
                                    if (docTypeAdvanceTransportist != null)
                                    {
                                        docTypeAdvanceTransportist.currentNumber = docTypeAdvanceTransportist.currentNumber + 1;
                                        db.DocumentType.Attach(docTypeAdvanceTransportist);
                                        db.Entry(docTypeAdvanceTransportist).State = EntityState.Modified;
                                    }
                                    item.RemissionGuideCustomizedAdvancedTransportist.Document = docAdvanceTransportist;
                                    #endregion

                                    item.RemissionGuideCustomizedAdvancedTransportist.isActive = true;
                                    item.RemissionGuideCustomizedAdvancedTransportist.hasPayment = false;
                                    item.RemissionGuideCustomizedAdvancedTransportist.valueTotal = item?.RemissionGuideTransportation?.advancePrice ?? 0;
                                }
                            }
                            #endregion

                            #region INFORMATION CUSTOMIZABLE FOR WAREHOUSE SEQUENTIAL
                            if (det.nameObjectTable == "DispatchMaterialSequential")
                            {
                                var lstRgdm = item.RemissionGuideDispatchMaterial
                                                    .Where(w => w.isActive && w.sourceExitQuantity > 0)
                                                    .Select(s => s.id_warehouse).Distinct().ToList();

                                foreach (int i in lstRgdm)
                                {
                                    Warehouse wTmp = db.Warehouse.FirstOrDefault(fod => fod.id == i);
                                    if (wTmp != null)
                                    {
                                        SequentialGeneral sqTmp = db.SequentialGeneral.FirstOrDefault(fod => fod.id_break1 == wTmp.id && fod.nameObject1 == "Warehouse");

                                        if (sqTmp != null)
                                        {
                                            DispatchMaterialSequential dmsTmp = new DispatchMaterialSequential();
                                            dmsTmp.id_Warehouse = wTmp.id;
                                            dmsTmp.id_RemissionGuide = item.id;
                                            dmsTmp.sequential = sqTmp.sequential1;

                                            db.DispatchMaterialSequential.Add(dmsTmp);

                                            sqTmp.sequential1 = sqTmp.sequential1 + 1;
                                            db.SequentialGeneral.Attach(sqTmp);
                                            db.Entry(sqTmp).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region TRANSPORTATION INFORMATION CUSTOMIZABLE
                            if (item.RemissionGuideTransportation.isOwn)
                            {
                                item.RemissionGuideTransportationCustomizedInformation = item.RemissionGuideTransportationCustomizedInformation ?? new RemissionGuideTransportationCustomizedInformation();
                            }
                            #endregion

                            #region TRANSPORTATION INFORMATION BUY ICE
                            if (det.nameObjectTable == "RemissionGuideCustomizedIceBuyInformation")
                            {
                                string _strIceThirdId = db.Setting.FirstOrDefault(fod => fod.code == "CODHIT")?.value;

                                if (!string.IsNullOrEmpty(_strIceThirdId) && Convert.ToInt32(_strIceThirdId) > 0)
                                {
                                    int _iIceThirdId = Convert.ToInt32(_strIceThirdId);

                                    var _rgdmTmp = item.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id_item == _iIceThirdId && fod.sourceExitQuantity > 0);
                                    if (_rgdmTmp != null)
                                    {
                                        item.RemissionGuideCustomizedIceBuyInformation = item.RemissionGuideCustomizedIceBuyInformation ?? new RemissionGuideCustomizedIceBuyInformation();
                                        item.RemissionGuideCustomizedIceBuyInformation.Document = new Document();

                                        #region CREACION DEL DOCUMENTO
                                        Document docBuyIceThird = new Document();

                                        docBuyIceThird.id_userCreate = ActiveUser.id;
                                        docBuyIceThird.dateCreate = DateTime.Now;
                                        docBuyIceThird.id_userUpdate = ActiveUser.id;
                                        docBuyIceThird.dateUpdate = DateTime.Now;

                                        docBuyIceThird.emissionDate = item.Document.emissionDate;

                                        DocumentType docTypeBuyIceThird = db.DocumentType.FirstOrDefault(fod => fod.code == "80");

                                        docBuyIceThird.sequential = docTypeBuyIceThird?.currentNumber ?? 0;
                                        docBuyIceThird.number = GetDocumentNumber(docTypeBuyIceThird.id, id_ep);
                                        docBuyIceThird.DocumentType = docTypeBuyIceThird;

                                        DocumentState docStateBuyIce = db.DocumentState
                                                                                    .FirstOrDefault(s => s.code == "01"
                                                                                    && s.tbsysDocumentTypeDocumentState
                                                                                    .Any(a => a.DocumentType.code == "80"));

                                        docBuyIceThird.DocumentState = docStateBuyIce;
                                        docBuyIceThird.id_documentState = docStateBuyIce.id;

                                        docBuyIceThird.id_emissionPoint = emissionPoint?.id ?? 0;
                                        docBuyIceThird.EmissionPoint = emissionPoint;

                                        //Actualiza Secuencial Compra Hielo
                                        if (docTypeBuyIceThird != null)
                                        {
                                            docTypeBuyIceThird.currentNumber = docTypeBuyIceThird.currentNumber + 1;
                                            db.DocumentType.Attach(docTypeBuyIceThird);
                                            db.Entry(docTypeBuyIceThird).State = EntityState.Modified;
                                        }
                                        #endregion

                                        item.RemissionGuideCustomizedIceBuyInformation.Document = docBuyIceThird;
                                        item.RemissionGuideCustomizedIceBuyInformation.quantityIceBagsRequested = Convert.ToInt32(_rgdmTmp.sourceExitQuantity);
                                        item.RemissionGuideCustomizedIceBuyInformation.isActive = true;
                                        item.RemissionGuideCustomizedIceBuyInformation.name_ProviderIceBags = itemCustIceBuy?.name_ProviderIceBags;
                                        item.RemissionGuideCustomizedIceBuyInformation.id_ProviderIceBags = (itemCustIceBuy != null && itemCustIceBuy.id_ProviderIceBags > 0) ? itemCustIceBuy.id_ProviderIceBags : null;
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    db.RemissionGuide.Add(item);
                    db.SaveChanges();
                    trans.Commit();
                    _answer = 1;
                    item.getLiquidationInformation();
                    if (transportation != null)
                    {
                        transportation.Vehicle = db.Vehicle.Where(i => i.id == transportation.id_vehicle).FirstOrDefault();
                        item.RemissionGuideTransportation = transportation;
                    }

                    TempData["remissionGuide"] = item;
                    TempData.Keep("remissionGuide");

                    ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + item.Document.number + " guardada exitosamente");

                    _messageAnswer = SuccessMessage("Guía de Remisión: " + item.Document.number + " guardada exitosamente");
                    _codeStateDocument = (approve) ? "03" : "01";
                    _nameStateDocument = (approve) ? "APROBADO" : "PENDIENTE";
                    _strNumberDocAnswer = item?.Document?.number;
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuide");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    _messageAnswer = ErrorMessage(e.Message);
                    _codeStateDocument = "";
                    _nameStateDocument = "";
                    _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(item.id, _codeStateDocument);
                    _AnswerfaRG.idEntity = item.id;
                    _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                    _AnswerfaRG.messageAnswer = _messageAnswer;
                    _AnswerfaRG.nameStateAnswer = _nameStateDocument;
                    _AnswerfaRG.codeStateAnswer = _codeStateDocument;
                    _AnswerfaRG.strNumberDocAnswer = _strNumberDocAnswer;
                    MetodosEscrituraLogs.EscribeMensajeLog(e.Message, ruta, "LOGISTICAADDNEW", "PROD");
                    return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                }
            }

            #region AUDITORIA
            #endregion

            if (_answer == 1)
            {
                #region CERRADO DE DOCUMENTOS
                if (approve)
                {
                    if (item.id > 0)
                    {
                        List<int> iLstPod = db.RemissionGuideDetailPurchaseOrderDetail
                                            .Where(w => w.RemissionGuideDetail.RemissionGuide.id == item.id)
                                            .Select(s => s.id_purchaseOrderDetail)
                                            .ToList();

                        if (iLstPod != null && iLstPod.Count > 0)
                        {
                            var _iLstPo = db.PurchaseOrderDetail
                                        .Where(w => iLstPod.Contains(w.id))
                                        .GroupBy(g => g.PurchaseOrder.id)
                                        .Select(s => new
                                        {
                                            id_po = s.Max(m => m.PurchaseOrder.id),
                                            qAp = s.Sum(su => su.quantityApproved),
                                            qPe = s.Sum(su => su.quantityDispatched)
                                        }).ToList();

                            if (_iLstPo != null && _iLstPo.Count > 0)
                            {
                                #region Listado de Ordenes de Compra
                                foreach (var _de in _iLstPo)
                                {
                                    if (_de.id_po > 0)
                                    {
                                        if ((_de.qAp - _de.qPe) <= 0)
                                        {
                                            Document _doPo = db.Document.FirstOrDefault(fod => fod.id == _de.id_po);

                                            if (_doPo != null)
                                            {
                                                using (DbContextTransaction trans = db.Database.BeginTransaction())
                                                {
                                                    try
                                                    {
                                                        _doPo.isOpen = false;
                                                        db.Document.Attach(_doPo);
                                                        db.Entry(_doPo).State = EntityState.Modified;
                                                        db.SaveChanges();
                                                        trans.Commit();
                                                        _answerCloseDoc = 1;
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                #region AUDITORIA CERRADO DE DOCUMENTO ORDEN DE COMPRA
                                                if (_answerCloseDoc == 1)
                                                {
                                                    DocumentLogDTO _doTmp = new DocumentLogDTO();
                                                    _doTmp.id = _de.id_po;
                                                    _doTmp.code_Action = "CRD";
                                                    _doTmp.id_User = ActiveUser.id;
                                                    _doTmp.description = "EJECUCION MANUAL";
                                                    Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #region GENERATE REQUEST
                    GenerateRequestToWarehouse(item);
                    #endregion
                }
                #endregion
            }

            item.isManual = returnRemissionGuide.isManual;

            _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(item.id, _codeStateDocument);
            _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
            _AnswerfaRG.messageAnswer = _messageAnswer;
            _AnswerfaRG.idEntity = item.id;
            _AnswerfaRG.nameStateAnswer = _nameStateDocument;
            _AnswerfaRG.codeStateAnswer = _codeStateDocument;
            _AnswerfaRG.strNumberDocAnswer = _strNumberDocAnswer;
            return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
        }

        public void UpdateRemisionGuideReassignment(bool approve, RemissionGuide objRemisionGuide)
        {
            if (objRemisionGuide != null && objRemisionGuide.id_providerRemisionGuide != null && objRemisionGuide.id_providerRemisionGuide > 0 && approve)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        RemissionGuide modelItem = db.RemissionGuide.FirstOrDefault(r => r.id == objRemisionGuide.id_RemisionGuideReassignment);
                        modelItem.id_RemisionGuideReassignment = objRemisionGuide.id;
                        db.RemissionGuide.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult RemissionGuidePartialUpdate(bool approve, RemissionGuide item,
                                                        RemissionGuideCustomizedInformation itemCi,
                                                        RemissionGuideCustomizedIceBuyInformation itemCustIceBuy,
                                                        Document document,
                                                        RemissionGuideTransportation transportation,
                                                        RemissionGuideExportInformation exportInformation, string despachurehour)
        {
            _btnOnEditFormRG = new ButtonsOnEditFormRemissionGuide();
            _AnswerfaRG = new AnswerForActionRemissionGuide();
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide returnRemissionGuide = null;
            if (TempData["remissionGuide"] != null)
            {
                returnRemissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            }
            decimal flete = 0;
            int _answerCloseDoc = 0;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            if (transportation != null && transportation.id_FishingSiteRG > 0
                    && item != null && item.id_TransportTariffType > 0)
            {
                RGParamPriceFreight rgPpf = new RGParamPriceFreight();
                rgPpf.id_FishingSite = transportation.id_FishingSiteRG;
                rgPpf.id_TransportTariff = item.id_TransportTariffType;
                flete = CalculatePriceFreight(rgPpf);
            }

            returnRemissionGuide = returnRemissionGuide ?? db.RemissionGuide.FirstOrDefault(r => r.id == item.id);

            var id_providerreturn = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle).FirstOrDefault()?.id_Provider;
            transportation.id_provider = id_providerreturn;

            if (transportation.id_vehicle != null)
            {
                transportation.Vehicle = transportation.Vehicle = db.Vehicle.Where(X => X.id == transportation.id_vehicle).FirstOrDefault();
            }

            returnRemissionGuide.RemissionGuideTransportation = transportation;

            if (transportation.isOwn == false)
            {
                returnRemissionGuide.id_shippingType = item.id_shippingType;
                returnRemissionGuide.id_TransportTariffType = item.id_TransportTariffType;
            }

            #region Asignacion de Guia

            returnRemissionGuide.arrivalDate = item.arrivalDate;
            returnRemissionGuide.descriptionpurchaseorder = item.descriptionpurchaseorder;
            returnRemissionGuide.despachureDate = item.despachureDate;
            if (!String.IsNullOrEmpty(despachurehour)) returnRemissionGuide.despachurehour = TimeSpan.Parse(despachurehour.Substring(10).ToString());
            returnRemissionGuide.hasEntrancePlanctProduction = item.hasEntrancePlanctProduction;

            if (transportation.isOwn == false)
            {
                returnRemissionGuide.hasExitPlanctProduction = item.hasExitPlanctProduction;
            }

            returnRemissionGuide.id_buyer = item.id_buyer;
            returnRemissionGuide.id_priceList = item.id_priceList;
            returnRemissionGuide.id_certification = item.id_certification;
            returnRemissionGuide.id_productionUnitProvider = item.id_productionUnitProvider;

            if (returnRemissionGuide.id_productionUnitProvider > 0)
            {
                returnRemissionGuide.ProductionUnitProvider = db.ProductionUnitProvider.Where(x => x.id == returnRemissionGuide.id_productionUnitProvider).FirstOrDefault();
            }

            returnRemissionGuide.id_productionUnitProviderPool = item.id_productionUnitProviderPool;
            returnRemissionGuide.id_protectiveProvider = item.id_protectiveProvider;
            returnRemissionGuide.id_providerRemisionGuide = item.id_providerRemisionGuide;
            returnRemissionGuide.id_reason = item.id_reason;
            returnRemissionGuide.id_reciver = item.id_reciver;
            returnRemissionGuide.id_RemisionGuideReassignment = item.id_RemisionGuideReassignment;
            returnRemissionGuide.id_shippingType = item.id_shippingType;
            returnRemissionGuide.id_TransportTariffType = item.id_TransportTariffType;
            returnRemissionGuide.isExport = false;
            returnRemissionGuide.isInternal = false;
            returnRemissionGuide.returnDate = item.returnDate;
            returnRemissionGuide.route = item.route;
            returnRemissionGuide.startAdress = item.startAdress;
            returnRemissionGuide.uniqueCustomDocument = item.uniqueCustomDocument;
            returnRemissionGuide.Guia_Externa = item.Guia_Externa;
            returnRemissionGuide.RemissionGuideExportInformation = exportInformation;

            #endregion

            RemissionGuide modelItem = db.RemissionGuide.FirstOrDefault(r => r.id == item.id);
            _codeStateDocument = modelItem.Document?.DocumentState?.code ?? "";
            _nameStateDocument = modelItem.Document?.DocumentState?.name ?? "";
            _AnswerfaRG.idEntity = item.id;
            _AnswerfaRG.strNumberDocAnswer = modelItem.Document?.number ?? "";

            bool hasHDTmp = false;

            if (modelItem != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        int id_ep = 0;
                        if (TempData["id_ep"] != null)
                        {
                            id_ep = (int)TempData["id_ep"];
                        }
                        id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                        EmissionPoint _epTmp = db.EmissionPoint.FirstOrDefault(fod => fod.id == id_ep);

                        #region Document

                        modelItem.Document.description = document.description;
                        modelItem.Document.emissionDate = document.emissionDate;

                        modelItem.Document.id_userUpdate = ActiveUser.id;
                        modelItem.Document.dateUpdate = DateTime.Now;

                        #endregion

                        #region RemissionGuide

                        modelItem.id_reciver = item.id_reciver;
                        modelItem.Person = db.Person.FirstOrDefault(p => p.id == item.id_reciver);

                        modelItem.id_reason = item.id_reason;
                        modelItem.RemissionGuideReason = db.RemissionGuideReason.FirstOrDefault(r => r.id == item.id_reason);

                        modelItem.isExport = false;

                        modelItem.startAdress = item.startAdress;
                        modelItem.route = item.route;
                        modelItem.Guia_Externa = item.Guia_Externa;

                        modelItem.despachureDate = item.despachureDate;
                        modelItem.arrivalDate = item.arrivalDate;
                        modelItem.returnDate = item.returnDate;
                        modelItem.id_providerRemisionGuide = item.id_providerRemisionGuide;
                        modelItem.id_protectiveProvider = item.id_protectiveProvider;
                        modelItem.id_buyer = item.id_buyer;
                        modelItem.id_priceList = item.id_priceList;
                        modelItem.id_certification = item.id_certification;
                        modelItem.id_protectiveProvider = item.id_protectiveProvider;

                        modelItem.descriptionpurchaseorder = item.descriptionpurchaseorder;
                        modelItem.isInternal = false;
                        modelItem.id_RemisionGuideReassignment = item.id_RemisionGuideReassignment;

                        modelItem.id_shippingType = item.id_shippingType;
                        modelItem.id_TransportTariffType = item.id_TransportTariffType;
                        modelItem.id_personProcessPlant = item.id_personProcessPlant;
                        modelItem.isCopackingRG = item.isCopackingRG;

                        if (!String.IsNullOrEmpty(despachurehour)) modelItem.despachurehour = TimeSpan.Parse(despachurehour.Substring(10).ToString());

                        if (item.id_RemisionGuideReassignment != null)
                        {
                            int id_RemisionGuideReassignment = int.Parse(item.id_RemisionGuideReassignment.ToString());
                            modelItem.RemissionGuide2 = db.RemissionGuide.Where(x => x.id == id_RemisionGuideReassignment).FirstOrDefault();
                        }

                        #region TEMPORAL REMISSION GUIDE TYPE
                        PurchaseOrderShippingType postTmp = db.PurchaseOrderShippingType.FirstOrDefault(fod => fod.id == item.id_shippingType);

                        RemissionGuideType rgtTmp = new RemissionGuideType();
                        if (postTmp != null)
                        {
                            if (postTmp.code == "T")
                            {
                                rgtTmp = db.RemissionGuideType.FirstOrDefault(fod => fod.code == "T");
                            }
                            else if (postTmp.code == "M")
                            {
                                rgtTmp = db.RemissionGuideType.FirstOrDefault(fod => fod.code == "F");
                            }
                            else if (postTmp.code == "TF")
                            {
                                rgtTmp = db.RemissionGuideType.FirstOrDefault(fod => fod.code == "TF");
                            }
                        }
                        modelItem.id_RemissionGuideType = rgtTmp?.id ?? (int)item.id_shippingType;
                        #endregion
                        #endregion

                        #region REMISSION GUIDE TRANSPORTATION
                        if (returnRemissionGuide != null && returnRemissionGuide.RemissionGuideTransportation != null && returnRemissionGuide.RemissionGuideTransportation.isOwn == true)
                        {
                            modelItem.RemissionGuideTransportation = modelItem.RemissionGuideTransportation ?? new RemissionGuideTransportation();
                            modelItem.RemissionGuideTransportation.isOwn = true;
                            if (transportation != null)
                            {
                                modelItem.RemissionGuideTransportation.driverName = transportation.driverNameThird ?? "";
                                modelItem.RemissionGuideTransportation.carRegistration = transportation.carRegistrationThird ?? "";
                                modelItem.RemissionGuideTransportation.descriptionTrans = transportation.descriptionTrans ?? "";

                                modelItem.RemissionGuideTransportation.driverNameThird = transportation.driverNameThird;
                                modelItem.RemissionGuideTransportation.carRegistrationThird = transportation.carRegistrationThird;
                                modelItem.RemissionGuideTransportation.id_FishingSiteRG = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == modelItem.id_productionUnitProvider)?.id_FishingSite ?? null;
                            }
                        }
                        else if (transportation != null)
                        {
                            if (item.isInternal != null && (bool)item.isInternal)
                            {
                                modelItem.RemissionGuideTransportation.advancePrice = 0;
                                modelItem.RemissionGuideTransportation.valuePrice = 0;
                                transportation.advancePrice = 0;
                                transportation.valuePrice = 0;
                            }
                            else
                            {
                                modelItem.RemissionGuideTransportation.valuePrice = flete;
                                transportation.valuePrice = flete;
                                modelItem.RemissionGuideTransportation.advancePrice = transportation.advancePrice;
                            }

                            var id_provider = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle && x.datefin == null && x.Estado).FirstOrDefault()?.id_Provider;
                            transportation.id_provider = id_provider;

                            var id_VeicleProviderTransport = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle && x.datefin == null && x.Estado).FirstOrDefault()?.id;
                            var VehicleProviderTransportBilling = db.VehicleProviderTransportBilling.FirstOrDefault(fod => fod.id_vehicle == transportation.id_vehicle && fod.datefin == null && fod.state);

                            if (id_VeicleProviderTransport != null)
                            {
                                var DriverVeicleProviderTransportAx = db.DriverVeicleProviderTransport.Where(x => x.idVeicleProviderTransport == id_VeicleProviderTransport && x.id_driver == transportation.id_driver).FirstOrDefault();

                                if (DriverVeicleProviderTransportAx != null)
                                {
                                    modelItem.RemissionGuideTransportation.id_DriverVeicleProviderTransport = DriverVeicleProviderTransportAx.id;
                                }
                                else
                                {
                                    DriverVeicleProviderTransport driverVeicleProviderTransport = new DriverVeicleProviderTransport()
                                    {
                                        Estado = true,
                                        idVeicleProviderTransport = id_VeicleProviderTransport,
                                        id_driver = transportation.id_driver,
                                    };

                                    modelItem.RemissionGuideTransportation.DriverVeicleProviderTransport = driverVeicleProviderTransport;
                                    transportation.DriverVeicleProviderTransport = driverVeicleProviderTransport;
                                }
                            }
                            if (VehicleProviderTransportBilling != null)
                            {
                                modelItem.RemissionGuideTransportation.id_VehicleProviderTranportistBilling = VehicleProviderTransportBilling.id;
                            }
                            else
                            {
                                VehicleProviderTransportBilling vehicleProviderTransportBilling = new VehicleProviderTransportBilling()
                                {
                                    id_vehicle = transportation.id_vehicle.Value,
                                    id_provider = transportation.id_provider.Value,
                                    dateinit = DateTime.Now,
                                    datefin = null,
                                    state = true,

                                };

                                modelItem.RemissionGuideTransportation.VehicleProviderTransportBilling = VehicleProviderTransportBilling;
                                transportation.VehicleProviderTransportBilling = VehicleProviderTransportBilling;
                            }

                            modelItem.RemissionGuideTransportation.id_vehicle = transportation.id_vehicle;
                            modelItem.RemissionGuideTransportation.carRegistration = transportation.carRegistration;
                            modelItem.RemissionGuideTransportation.id_driver = transportation.id_driver;
                            modelItem.RemissionGuideTransportation.driverName = transportation.driverName;
                            modelItem.RemissionGuideTransportation.id_VehicleProviderTranportistBilling = VehicleProviderTransportBilling.id;
                            modelItem.RemissionGuideTransportation.valuePrice = transportation.valuePrice;
                            modelItem.RemissionGuideTransportation.advancePrice = transportation.advancePrice;
                            modelItem.RemissionGuideTransportation.descriptionTrans = transportation.descriptionTrans;
                            modelItem.RemissionGuideTransportation.VehicleHunterLockText = transportation.VehicleHunterLockText;
                            modelItem.RemissionGuideTransportation.id_FishingSiteRG = transportation.id_FishingSiteRG;

                            transportation.id_vehicle = modelItem.RemissionGuideTransportation.id_vehicle;
                            transportation.carRegistration = modelItem.RemissionGuideTransportation.carRegistration;
                            transportation.id_driver = modelItem.RemissionGuideTransportation.id_driver;
                            transportation.driverName = modelItem.RemissionGuideTransportation.driverName;
                            transportation.id_VehicleProviderTranportistBilling = VehicleProviderTransportBilling.id;
                            transportation.valuePrice = modelItem.RemissionGuideTransportation.valuePrice;
                            transportation.advancePrice = modelItem.RemissionGuideTransportation.advancePrice;
                            transportation.descriptionTrans = modelItem.RemissionGuideTransportation.descriptionTrans;
                            transportation.VehicleHunterLockText = modelItem.RemissionGuideTransportation.VehicleHunterLockText;
                            transportation.id_FishingSiteRG = modelItem.RemissionGuideTransportation.id_FishingSiteRG;
                            transportation.id_DriverVeicleProviderTransport = modelItem.RemissionGuideTransportation.id_DriverVeicleProviderTransport;
                        }
                        #endregion

                        #region Remission Guide Export Information

                        if (modelItem.isExport)
                        {
                            if (!string.IsNullOrEmpty(exportInformation?.uniqueCustomsDocument))
                            {
                                if (modelItem.RemissionGuideExportInformation == null)
                                {
                                    modelItem.RemissionGuideExportInformation = new RemissionGuideExportInformation
                                    {
                                        uniqueCustomsDocument = exportInformation.uniqueCustomsDocument
                                    };
                                }
                                else
                                {
                                    modelItem.RemissionGuideExportInformation.uniqueCustomsDocument = exportInformation.uniqueCustomsDocument;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(exportInformation?.uniqueCustomsDocument))
                            {
                                modelItem.RemissionGuideExportInformation = new RemissionGuideExportInformation
                                {
                                    uniqueCustomsDocument = exportInformation.uniqueCustomsDocument
                                };
                            }
                            else
                            {
                                RemissionGuideExportInformation remissionGuideExportInformation = db.RemissionGuideExportInformation.FirstOrDefault(e => e.id == modelItem.id);
                                if (remissionGuideExportInformation != null)
                                {
                                    db.RemissionGuideExportInformation.Remove(remissionGuideExportInformation);
                                    db.Entry(remissionGuideExportInformation).State = EntityState.Deleted;
                                }
                            }
                        }

                        #endregion

                        RemissionGuide tempRemissionGuide = (TempData["remissionGuide"] as RemissionGuide);
                        tempRemissionGuide = tempRemissionGuide ?? new RemissionGuide();

                        #region Valida Producto por Guia
                        if (tempRemissionGuide?.RemissionGuideDetail != null)
                        {
                            if (item.isInternal != true)
                            {
                                var details = tempRemissionGuide.RemissionGuideDetail.ToList();

                                int id_inventoryLine = db.InventoryLine.Where(x => x.code == "MP").FirstOrDefault().id;

                                var conut = details.Where(x => x.Item.id_inventoryLine != id_inventoryLine && x.isActive).ToList().Count;

                                if (conut > 0)
                                {
                                    TempData["remissionGuide"] = returnRemissionGuide;
                                    TempData.Keep("remissionGuide");
                                    ViewData["EditMessage"] = ErrorMessage("No Todos Los Productos de ingresado en son de Materia Prima, favor revisar...");
                                    _messageAnswer = ErrorMessage("No Todos Los Productos de ingresado en son de Materia Prima, favor revisar...");
                                    _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(item.id, _codeStateDocument);
                                    _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                                    _AnswerfaRG.messageAnswer = _messageAnswer;
                                    _AnswerfaRG.nameStateAnswer = _nameStateDocument;
                                    _AnswerfaRG.codeStateAnswer = _codeStateDocument;
                                    return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }

                        var advancePrice = transportation.advancePrice ?? 0;
                        var valuePrice = transportation.valuePrice ?? 0;

                        if (advancePrice >= valuePrice && (advancePrice != 0 || valuePrice != 0))
                        {
                            tempRemissionGuide.RemissionGuideTransportation = transportation;
                            TempData["remissionGuide"] = returnRemissionGuide;
                            TempData.Keep("remissionGuide");
                            ViewData["EditMessage"] = ErrorMessage("EL Valor del anticipo debe ser menor al valor del Flete, favor revisar...");
                            _messageAnswer = ErrorMessage("EL Valor del anticipo debe ser menor al valor del Flete, favor revisar...");
                            _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(item.id, _codeStateDocument);
                            _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                            _AnswerfaRG.messageAnswer = _messageAnswer;
                            _AnswerfaRG.strNumberDocAnswer = _strNumberDocAnswer;
                            return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                        }
                        #endregion

                        #region REMISSION GUIDE DETAILS

                        int count = 0;

                        if (tempRemissionGuide?.RemissionGuideDetail != null)
                        {
                            var details = tempRemissionGuide.RemissionGuideDetail.ToList();

                            var ids_remissionGuideDispatchMaterial = new List<int>();

                            #region "Recorro Detalles de Guías de Remisión"
                            foreach (var detail in details)
                            {
                                RemissionGuideDetail remissionGuideDetail = modelItem.RemissionGuideDetail.FirstOrDefault(d => d.id == detail.id);

                                if (remissionGuideDetail == null)
                                {
                                    remissionGuideDetail = new RemissionGuideDetail
                                    {
                                        id_item = detail.id_item,
                                        Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                                        id_businessOportunityPlanningDetail = detail.id_businessOportunityPlanningDetail,
                                        BusinessOportunityPlanningDetail = db.BusinessOportunityPlanningDetail.FirstOrDefault(i => i.id == detail.id_businessOportunityPlanningDetail),

                                        quantityOrdered = detail.quantityOrdered,
                                        quantityProgrammed = detail.quantityProgrammed,
                                        quantityDispatchPending = detail.quantityDispatchPending,
                                        quantityReceived = detail.quantityReceived,
                                        productionUnitProviderPoolreference = detail.productionUnitProviderPoolreference,

                                        RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>(),

                                        isActive = detail.isActive,
                                        id_userCreate = detail.id_userCreate,
                                        dateCreate = detail.dateCreate,
                                        id_userUpdate = detail.id_userUpdate,
                                        dateUpdate = detail.dateUpdate,
                                        id_Grammage = detail.id_Grammage
                                    };

                                    if (remissionGuideDetail.isActive)
                                    {
                                        modelItem.RemissionGuideDetail.Add(remissionGuideDetail);
                                        count++;
                                    }
                                }
                                else
                                {
                                    remissionGuideDetail.id_item = detail.id_item;
                                    remissionGuideDetail.Item = db.Item.FirstOrDefault(i => i.id == detail.id_item);

                                    remissionGuideDetail.productionUnitProviderPoolreference = detail.productionUnitProviderPoolreference;
                                    remissionGuideDetail.quantityProgrammed = detail.quantityProgrammed;
                                    remissionGuideDetail.quantityDispatchPending = detail.quantityDispatchPending;
                                    remissionGuideDetail.quantityReceived = detail.quantityReceived;

                                    remissionGuideDetail.isActive = detail.isActive;
                                    remissionGuideDetail.id_userCreate = detail.id_userCreate;
                                    remissionGuideDetail.dateCreate = detail.dateCreate;
                                    remissionGuideDetail.id_userUpdate = detail.id_userUpdate;
                                    remissionGuideDetail.dateUpdate = detail.dateUpdate;
                                    remissionGuideDetail.id_Grammage = detail.id_Grammage;

                                    foreach (var remissionGuideDetailPurchaseOrderDetail in remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail)
                                    {
                                        remissionGuideDetailPurchaseOrderDetail.quantity = detail.quantityProgrammed;
                                        db.RemissionGuideDetailPurchaseOrderDetail.Attach(remissionGuideDetailPurchaseOrderDetail);
                                        db.Entry(remissionGuideDetailPurchaseOrderDetail).State = EntityState.Modified;
                                    }

                                    db.Entry(remissionGuideDetail).State = EntityState.Modified;

                                    if (remissionGuideDetail.isActive)
                                        count++;
                                }
                                if (modelItem != null && modelItem.RemissionGuideTransportation != null && !modelItem.RemissionGuideTransportation.isOwn)
                                {
                                    #region "Remission Guide Detail Dispatch Material Detail"
                                    foreach (var remissionGuideDetailDispatchMaterialDetail in detail.RemissionGuideDetailDispatchMaterialDetail)
                                    {
                                        var remissionGuideDispatchMaterialAux = modelItem.RemissionGuideDispatchMaterial.FirstOrDefault(d => d.id == remissionGuideDetailDispatchMaterialDetail.id_remissionGuideDispatchMaterial);
                                        var detailRemissionGuideDispatchMaterial = tempRemissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id == remissionGuideDetailDispatchMaterialDetail.id_remissionGuideDispatchMaterial);
                                        if (remissionGuideDispatchMaterialAux == null)
                                        {
                                            remissionGuideDispatchMaterialAux = new RemissionGuideDispatchMaterial
                                            {
                                                id = detailRemissionGuideDispatchMaterial.id,
                                                id_item = detailRemissionGuideDispatchMaterial.id_item,
                                                Item = db.Item.FirstOrDefault(i => i.id == detailRemissionGuideDispatchMaterial.id_item),

                                                quantityRequiredForPurchase = detailRemissionGuideDispatchMaterial.quantityRequiredForPurchase,
                                                sourceExitQuantity = detailRemissionGuideDispatchMaterial.sourceExitQuantity,
                                                sendedDestinationQuantity = detailRemissionGuideDispatchMaterial.sendedDestinationQuantity,
                                                arrivalDestinationQuantity = detailRemissionGuideDispatchMaterial.arrivalDestinationQuantity,
                                                arrivalGoodCondition = detailRemissionGuideDispatchMaterial.arrivalGoodCondition,
                                                arrivalBadCondition = detailRemissionGuideDispatchMaterial.arrivalBadCondition,

                                                RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>(),

                                                manual = detailRemissionGuideDispatchMaterial.manual,
                                                isActive = detailRemissionGuideDispatchMaterial.isActive,
                                                id_userCreate = detailRemissionGuideDispatchMaterial.id_userCreate,
                                                dateCreate = detailRemissionGuideDispatchMaterial.dateCreate,
                                                id_userUpdate = detailRemissionGuideDispatchMaterial.id_userUpdate,
                                                dateUpdate = detailRemissionGuideDispatchMaterial.dateUpdate
                                            };
                                            if (modelItem.RemissionGuideDispatchMaterial == null)
                                            {
                                                modelItem.RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>();
                                            }

                                            modelItem.RemissionGuideDispatchMaterial.Add(remissionGuideDispatchMaterialAux);

                                            ids_remissionGuideDispatchMaterial.Add(detailRemissionGuideDispatchMaterial.id);
                                        }
                                        else
                                        {
                                            if (!ids_remissionGuideDispatchMaterial.Contains(detailRemissionGuideDispatchMaterial.id))
                                            {
                                                remissionGuideDispatchMaterialAux.id_item = detailRemissionGuideDispatchMaterial.id_item;
                                                remissionGuideDispatchMaterialAux.Item = db.Item.FirstOrDefault(i => i.id == detailRemissionGuideDispatchMaterial.id_item);

                                                remissionGuideDispatchMaterialAux.quantityRequiredForPurchase = detailRemissionGuideDispatchMaterial.quantityRequiredForPurchase;
                                                remissionGuideDispatchMaterialAux.sourceExitQuantity = detailRemissionGuideDispatchMaterial.sourceExitQuantity;

                                                for (int j = remissionGuideDispatchMaterialAux.RemissionGuideDetailDispatchMaterialDetail.Count - 1; j >= 0; j--)
                                                {
                                                    var detailRemissionGuideDetailDispatchMaterialDetail = remissionGuideDispatchMaterialAux.RemissionGuideDetailDispatchMaterialDetail.ElementAt(j);
                                                    remissionGuideDispatchMaterialAux.RemissionGuideDetailDispatchMaterialDetail.Remove(detailRemissionGuideDetailDispatchMaterialDetail);
                                                    db.Entry(detailRemissionGuideDetailDispatchMaterialDetail).State = EntityState.Deleted;
                                                }

                                                remissionGuideDispatchMaterialAux.RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>();

                                                remissionGuideDispatchMaterialAux.manual = detailRemissionGuideDispatchMaterial.manual;
                                                remissionGuideDispatchMaterialAux.isActive = detailRemissionGuideDispatchMaterial.isActive;
                                                remissionGuideDispatchMaterialAux.id_userUpdate = detailRemissionGuideDispatchMaterial.id_userUpdate;
                                                remissionGuideDispatchMaterialAux.dateUpdate = detailRemissionGuideDispatchMaterial.dateUpdate;

                                                db.Entry(remissionGuideDispatchMaterialAux).State = EntityState.Modified;

                                                ids_remissionGuideDispatchMaterial.Add(detailRemissionGuideDispatchMaterial.id);
                                            }
                                        }

                                        var newRemissionGuideDetailDispatchMaterialDetail = new RemissionGuideDetailDispatchMaterialDetail
                                        {
                                            id_remissionGuideDispatchMaterial = remissionGuideDispatchMaterialAux.id,
                                            RemissionGuideDispatchMaterial = remissionGuideDispatchMaterialAux,
                                            id_remissionGuideDetail = remissionGuideDetail.id,
                                            RemissionGuideDetail = remissionGuideDetail,
                                            quantity = remissionGuideDetailDispatchMaterialDetail.quantity
                                        };
                                        remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail.Add(newRemissionGuideDetailDispatchMaterialDetail);
                                        remissionGuideDispatchMaterialAux.RemissionGuideDetailDispatchMaterialDetail.Add(newRemissionGuideDetailDispatchMaterialDetail);
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }

                        if (count == 0)
                        {
                            TempData["remissionGuide"] = returnRemissionGuide;
                            TempData.Keep("remissionGuide");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una guía de remisión sin detalles");
                            _messageAnswer = ErrorMessage("No se puede guardar una guía de remisión sin detalles");
                            _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(item.id, _codeStateDocument);
                            _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                            _AnswerfaRG.messageAnswer = _messageAnswer;
                            _AnswerfaRG.nameStateAnswer = _nameStateDocument;
                            _AnswerfaRG.codeStateAnswer = _codeStateDocument;
                            _AnswerfaRG.strNumberDocAnswer = _strNumberDocAnswer;
                            return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                        }

                        #endregion

                        #region REMISSION GUIDE DISPATCH MATERIALS
                        if (!modelItem.RemissionGuideTransportation.isOwn)
                        {
                            if (tempRemissionGuide?.RemissionGuideDispatchMaterial != null)
                            {
                                var details = tempRemissionGuide.RemissionGuideDispatchMaterial.ToList();

                                foreach (var detail in details)
                                {
                                    if ((detail.RemissionGuideDetailDispatchMaterialDetail?.Count() ?? 0) > 0)
                                    {
                                        continue;
                                    }
                                    var id_ware = db.Item.FirstOrDefault(i => i.id == detail.id_item)?.ItemInventory?.id_warehouse;
                                    var id_warelo = (int)db.Item.FirstOrDefault(fod => fod.id == detail.id_item)?.ItemInventory?.id_warehouseLocation;
                                    RemissionGuideDispatchMaterial remissionGuideDispatchMaterial = modelItem.RemissionGuideDispatchMaterial.FirstOrDefault(d => d.id == detail.id);

                                    if (remissionGuideDispatchMaterial == null)
                                    {
                                        remissionGuideDispatchMaterial = new RemissionGuideDispatchMaterial
                                        {
                                            id_item = detail.id_item,
                                            Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                                            quantityRequiredForPurchase = detail.quantityRequiredForPurchase,
                                            sourceExitQuantity = detail.sourceExitQuantity,
                                            sendedDestinationQuantity = detail.sendedDestinationQuantity,
                                            arrivalDestinationQuantity = detail.arrivalDestinationQuantity,
                                            arrivalGoodCondition = detail.arrivalGoodCondition,
                                            arrivalBadCondition = detail.arrivalBadCondition,
                                            id_warehouse = id_ware,
                                            id_warehouselocation = id_warelo,
                                            Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == id_ware),
                                            manual = detail.manual,
                                            isActive = detail.isActive,
                                            id_userCreate = detail.id_userCreate,
                                            dateCreate = detail.dateCreate,
                                            id_userUpdate = detail.id_userUpdate,
                                            dateUpdate = detail.dateUpdate
                                        };

                                        if (remissionGuideDispatchMaterial.isActive)
                                        {
                                            if (modelItem.RemissionGuideDispatchMaterial == null)
                                            {
                                                modelItem.RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>();
                                            }
                                            modelItem.RemissionGuideDispatchMaterial.Add(remissionGuideDispatchMaterial);
                                        }
                                    }
                                    else
                                    {
                                        remissionGuideDispatchMaterial.quantityRequiredForPurchase = detail.quantityRequiredForPurchase;
                                        remissionGuideDispatchMaterial.sourceExitQuantity = detail.sourceExitQuantity;
                                        remissionGuideDispatchMaterial.manual = detail.manual;
                                        remissionGuideDispatchMaterial.isActive = detail.isActive;
                                        remissionGuideDispatchMaterial.id_userUpdate = detail.id_userUpdate;
                                        remissionGuideDispatchMaterial.dateUpdate = detail.dateUpdate;

                                        db.Entry(remissionGuideDispatchMaterial).State = EntityState.Modified;

                                        db.SaveChanges();
                                    }
                                }
                                var it = modelItem.RemissionGuideDispatchMaterial.ToList();
                                foreach (var d in it)
                                {
                                    var id_war = db.Item.FirstOrDefault(i => i.id == d.id_item)?.ItemInventory?.id_warehouse;
                                    var id_warlo = (int)db.Item.FirstOrDefault(fod => fod.id == d.id_item)?.ItemInventory?.id_warehouseLocation;
                                    d.id_warehouse = id_war;
                                    d.id_warehouselocation = id_warlo;
                                    if (d.sourceExitQuantity < 0) d.sourceExitQuantity = 0;
                                }
                            }
                        }
                        #endregion

                        #region REMISSION GUIDE SECURITY SEALS

                        if (tempRemissionGuide?.RemissionGuideSecuritySeal != null)
                        {
                            var details = tempRemissionGuide.RemissionGuideSecuritySeal.ToList();

                            foreach (var detail in details)
                            {
                                RemissionGuideSecuritySeal remissionGuideSecuritySeal = modelItem.RemissionGuideSecuritySeal.FirstOrDefault(d => d.id == detail.id);

                                if (remissionGuideSecuritySeal == null)
                                {
                                    remissionGuideSecuritySeal = new RemissionGuideSecuritySeal
                                    {
                                        number = detail.number,

                                        id_travelType = detail.id_travelType,
                                        RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType),

                                        id_exitState = detail.id_exitState,
                                        SecuritySealState = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_exitState),
                                        id_arrivalState = detail.id_arrivalState,
                                        SecuritySealState1 = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_arrivalState),

                                        isActive = detail.isActive,
                                        id_userCreate = detail.id_userCreate,
                                        dateCreate = detail.dateCreate,
                                        id_userUpdate = detail.id_userUpdate,
                                        dateUpdate = detail.dateUpdate
                                    };

                                    if (remissionGuideSecuritySeal.isActive)
                                    {
                                        modelItem.RemissionGuideSecuritySeal.Add(remissionGuideSecuritySeal);
                                    }
                                }
                                else
                                {
                                    remissionGuideSecuritySeal.number = detail.number;

                                    remissionGuideSecuritySeal.id_travelType = detail.id_travelType;
                                    remissionGuideSecuritySeal.RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType);

                                    remissionGuideSecuritySeal.id_exitState = detail.id_exitState;
                                    remissionGuideSecuritySeal.SecuritySealState = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_exitState);
                                    remissionGuideSecuritySeal.id_arrivalState = detail.id_arrivalState;
                                    remissionGuideSecuritySeal.SecuritySealState1 = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_arrivalState);

                                    remissionGuideSecuritySeal.isActive = detail.isActive;
                                    remissionGuideSecuritySeal.id_userCreate = detail.id_userCreate;
                                    remissionGuideSecuritySeal.dateCreate = detail.dateCreate;
                                    remissionGuideSecuritySeal.id_userUpdate = detail.id_userUpdate;
                                    remissionGuideSecuritySeal.dateUpdate = detail.dateUpdate;
                                }
                            }
                        }

                        #endregion

                        #region REMISSION GUIDE ASSIGNED STAFF

                        if (tempRemissionGuide?.RemissionGuideAssignedStaff != null)
                        {
                            var details = tempRemissionGuide.RemissionGuideAssignedStaff.ToList();

                            foreach (var detail in details)
                            {
                                RemissionGuideAssignedStaff remissionGuideAssignedStaff = modelItem.RemissionGuideAssignedStaff.FirstOrDefault(d => d.id == detail.id);

                                if (remissionGuideAssignedStaff == null)
                                {
                                    remissionGuideAssignedStaff = new RemissionGuideAssignedStaff
                                    {
                                        id_person = detail.id_person,
                                        Person = db.Person.FirstOrDefault(p => p.id == detail.id_person),

                                        id_assignedStaffRol = detail.id_assignedStaffRol,
                                        RemissionGuideAssignedStaffRol = db.RemissionGuideAssignedStaffRol.FirstOrDefault(r => r.id == detail.id_assignedStaffRol),

                                        id_travelType = detail.id_travelType,
                                        RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType),
                                        viaticPrice = detail.viaticPrice ?? 0,
                                        isActive = detail.isActive,
                                        id_userCreate = detail.id_userCreate,
                                        dateCreate = detail.dateCreate,
                                        id_userUpdate = detail.id_userUpdate,
                                        dateUpdate = detail.dateUpdate
                                    };

                                    if (remissionGuideAssignedStaff.isActive)
                                    {
                                        modelItem.RemissionGuideAssignedStaff.Add(remissionGuideAssignedStaff);
                                    }
                                }
                                else
                                {
                                    remissionGuideAssignedStaff.id_person = detail.id_person;
                                    remissionGuideAssignedStaff.Person = db.Person.FirstOrDefault(p => p.id == detail.id_person);

                                    remissionGuideAssignedStaff.id_assignedStaffRol = detail.id_assignedStaffRol;
                                    remissionGuideAssignedStaff.RemissionGuideAssignedStaffRol = db.RemissionGuideAssignedStaffRol.FirstOrDefault(r => r.id == detail.id_assignedStaffRol);

                                    remissionGuideAssignedStaff.id_travelType = detail.id_travelType;
                                    remissionGuideAssignedStaff.RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType);

                                    remissionGuideAssignedStaff.isActive = detail.isActive;
                                    remissionGuideAssignedStaff.id_userCreate = detail.id_userCreate;
                                    remissionGuideAssignedStaff.dateCreate = detail.dateCreate;
                                    remissionGuideAssignedStaff.id_userUpdate = detail.id_userUpdate;
                                    remissionGuideAssignedStaff.dateUpdate = detail.dateUpdate;
                                    remissionGuideAssignedStaff.viaticPrice = detail.viaticPrice ?? 0;
                                }
                            }
                        }

                        #endregion

                        #region"DATA HELP FOR REMISSION GUIDE REPORT"

                        modelItem.RemissionGuideReportLinealDataHelp = modelItem.RemissionGuideReportLinealDataHelp ?? new RemissionGuideReportLinealDataHelp();

                        //Consulto Vehiculo Hunter
                        if (modelItem.RemissionGuideTransportation != null)
                        {
                            int id_vehTmp = modelItem.RemissionGuideTransportation.id_vehicle ?? 0;
                            if (id_vehTmp > 0)
                            {
                                hasHDTmp = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehTmp)?.hasHunterDevice ?? false;
                                if (hasHDTmp)
                                {
                                    modelItem.RemissionGuideReportLinealDataHelp.nameSecurityPerson = db.Setting.FirstOrDefault(fod => fod.code == "NDHL")?.value ?? "";
                                }
                            }
                        }
                        //Person Asignado
                        int id_SegRol = db.RemissionGuideAssignedStaffRol
                                            .FirstOrDefault(fod => fod.code == "SEG")?.id ?? 0;
                        int id_SegTec = db.RemissionGuideAssignedStaffRol
                                            .FirstOrDefault(fod => fod.code == "BIO")?.id ?? 0;

                        int id_rgasSeg = modelItem
                                        .RemissionGuideAssignedStaff
                                        .FirstOrDefault(fod => fod.id_assignedStaffRol == id_SegRol && fod.isActive)?.id_person ?? 0;

                        int id_rgasTec = modelItem
                                            .RemissionGuideAssignedStaff
                                            .FirstOrDefault(fod => fod.id_assignedStaffRol == id_SegTec && fod.isActive)?.id_person ?? 0;
                        //SEGURIDAD
                        if (id_rgasSeg > 0)
                        {
                            modelItem.RemissionGuideReportLinealDataHelp.id_SecurityPerson = id_rgasSeg;
                            modelItem.RemissionGuideReportLinealDataHelp.nameSecurityPerson += (hasHDTmp ? "/" : "") + db.Person.FirstOrDefault(fod => fod.id == id_rgasSeg)?.fullname_businessName ?? "";
                        }
                        //BIOLOGO
                        if (id_rgasTec > 0)
                        {
                            modelItem.RemissionGuideReportLinealDataHelp.id_TechnicianPerson = id_rgasTec;
                            modelItem
                                .RemissionGuideReportLinealDataHelp
                                .nameTechnicianPerson = db.Person.FirstOrDefault(fod => fod.id == id_rgasTec)?.fullname_businessName ?? "";
                        }
                        //SELLO DE SEGURIDAD
                        int str_SellOut = db.RemissionGuideTravelType
                                            .FirstOrDefault(fod => fod.code == "IDA")?.id ?? 0;
                        int str_SellIn = db.RemissionGuideTravelType
                                            .FirstOrDefault(fod => fod.code == "REGRESO")?.id ?? 0;

                        modelItem
                            .RemissionGuideReportLinealDataHelp
                            .SealNumberOneExit = modelItem.RemissionGuideSecuritySeal.FirstOrDefault(fod => fod.id_travelType == str_SellOut && fod.isActive)?.number ?? "";

                        modelItem
                            .RemissionGuideReportLinealDataHelp
                            .SealNumberTwoEntrance = modelItem.RemissionGuideSecuritySeal.FirstOrDefault(fod => fod.id_travelType == str_SellIn && fod.isActive)?.number ?? "";
                        #endregion

                        #region APROBACION
                        if (approve)
                        {
                            foreach (var detail in modelItem.RemissionGuideDetail)
                            {
                                foreach (var remissionGuideDetailPurchaseOrderDetail in detail.RemissionGuideDetailPurchaseOrderDetail)
                                {
                                    ServicePurchaseRemission.UpdateQuantityDispatchedPurchaseOrderDetailRemissionGuide(db, remissionGuideDetailPurchaseOrderDetail.id_purchaseOrderDetail,
                                                                                   remissionGuideDetailPurchaseOrderDetail.id_remissionGuideDetail,
                                                                                   remissionGuideDetailPurchaseOrderDetail.quantity);
                                }
                            }
                            if (modelItem != null && modelItem.RemissionGuideTransportation != null && !modelItem.RemissionGuideTransportation.isOwn)
                            {
                                foreach (var rgdm in modelItem.RemissionGuideDispatchMaterial.ToList())
                                {
                                    if (rgdm.InventoryMoveDetailExitDispatchMaterials != null)
                                    {
                                        var quantity = rgdm.InventoryMoveDetailExitDispatchMaterials.Sum(s => s.quantity);
                                        rgdm.sendedDestinationQuantity = quantity;
                                        db.Entry(rgdm).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        rgdm.sendedDestinationQuantity = 0;
                                        db.Entry(rgdm).State = EntityState.Modified;
                                    }
                                }
                            }
                            modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03");

                            #region GENERATE REQUEST
                            GenerateRequestToWarehouse(modelItem);
                            #endregion
                        }
                        #endregion

                        #region PERSONALIZATION DETAIL BY DOCUMENT TYPE
                        List<tbsysDocumentsPersonalizationDetail> lstDocPersDetail = db.tbsysDocumentsPersonalizationDetail
                                                                                        .Where(fod => fod.id_DocumentType == modelItem.Document.id_documentType)
                                                                                        .ToList();

                        if (lstDocPersDetail != null && lstDocPersDetail.Count > 0)
                        {
                            foreach (var det in lstDocPersDetail)
                            {
                                #region INFORMACION CUSTOMIZADA
                                if (det.nameObjectTable == "RemissionGuideCustomizedInformation")
                                {
                                    modelItem.RemissionGuideCustomizedInformation = modelItem.RemissionGuideCustomizedInformation ?? new RemissionGuideCustomizedInformation();
                                    modelItem.RemissionGuideCustomizedInformation.hasSecurity = itemCi?.hasSecurity ?? false;
                                }
                                #endregion

                                #region INFORMACION CUSTOMIZABLE VIATICO PERSONAL ASIGNADO
                                if (det.nameObjectTable == "RemissionGuideCustomizedViaticPersonalAssigned")
                                {
                                    int iQuantityPersonalAssigned = modelItem?.RemissionGuideAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).ToList().Count() ?? 0;
                                    if (iQuantityPersonalAssigned > 0)
                                    {
                                        if (modelItem.RemissionGuideCustomizedViaticPersonalAssigned != null)
                                        {
                                            modelItem.RemissionGuideCustomizedViaticPersonalAssigned.Document.id_userUpdate = ActiveUser.id;
                                            modelItem.RemissionGuideCustomizedViaticPersonalAssigned.Document.dateUpdate = DateTime.Now;
                                            modelItem.RemissionGuideCustomizedViaticPersonalAssigned.isActive = true;
                                            modelItem.RemissionGuideCustomizedViaticPersonalAssigned.valueTotal = modelItem?.RemissionGuideAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() ?? 0;
                                        }
                                        else
                                        {
                                            RemissionGuideCustomizedViaticPersonalAssigned rgcvpa = new RemissionGuideCustomizedViaticPersonalAssigned();

                                            rgcvpa.Document = new Document();

                                            #region CREACION DEL DOCUMENTO
                                            Document docViaticPersonal = new Document();

                                            docViaticPersonal.id_userCreate = ActiveUser.id;
                                            docViaticPersonal.dateCreate = DateTime.Now;
                                            docViaticPersonal.id_userUpdate = ActiveUser.id;
                                            docViaticPersonal.dateUpdate = DateTime.Now;

                                            docViaticPersonal.emissionDate = modelItem.Document.emissionDate;

                                            DocumentType docTypeViaticPersonal = db.DocumentType.FirstOrDefault(fod => fod.code == "77");

                                            docViaticPersonal.sequential = docTypeViaticPersonal?.currentNumber ?? 0;
                                            docViaticPersonal.number = GetDocumentNumber(docTypeViaticPersonal.id, id_ep);
                                            docViaticPersonal.DocumentType = docTypeViaticPersonal;
                                            docViaticPersonal.id_documentType = docTypeViaticPersonal.id;

                                            DocumentState docStateViaticPersonal = db.DocumentState
                                                                                        .FirstOrDefault(s => s.code == "01"
                                                                                        && s.tbsysDocumentTypeDocumentState
                                                                                        .Any(a => a.DocumentType.code == "77"));
                                            docViaticPersonal.DocumentState = docStateViaticPersonal;
                                            docViaticPersonal.id_documentState = docStateViaticPersonal.id;

                                            docViaticPersonal.id_emissionPoint = _epTmp?.id ?? 0;
                                            docViaticPersonal.EmissionPoint = _epTmp;

                                            if (docTypeViaticPersonal != null)
                                            {
                                                docTypeViaticPersonal.currentNumber = docTypeViaticPersonal.currentNumber + 1;
                                                db.DocumentType.Attach(docTypeViaticPersonal);
                                                db.Entry(docTypeViaticPersonal).State = EntityState.Modified;
                                            }
                                            rgcvpa.Document = docViaticPersonal;
                                            #endregion

                                            rgcvpa.isActive = true;
                                            rgcvpa.hasPayment = false;
                                            rgcvpa.id_RemissionGuide = modelItem.id;
                                            rgcvpa.valueTotal = modelItem?.RemissionGuideAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() ?? 0;
                                            db.RemissionGuideCustomizedViaticPersonalAssigned.Add(rgcvpa);
                                        }
                                    }
                                    else
                                    {
                                        if (modelItem.RemissionGuideCustomizedViaticPersonalAssigned != null)
                                        {
                                            modelItem.RemissionGuideCustomizedViaticPersonalAssigned.Document.id_userUpdate = ActiveUser.id;
                                            modelItem.RemissionGuideCustomizedViaticPersonalAssigned.Document.dateUpdate = DateTime.Now;
                                            modelItem.RemissionGuideCustomizedViaticPersonalAssigned.isActive = false;
                                            modelItem.RemissionGuideCustomizedViaticPersonalAssigned.valueTotal = 0;
                                        }
                                    }
                                }
                                #endregion

                                #region INFORMACION CUSTOMIZABLE ANTICIPO A TRANSPORTISTA
                                if (det.nameObjectTable == "RemissionGuideCustomizedAdvancedTransportist")
                                {
                                    if (modelItem.RemissionGuideTransportation?.advancePrice > 0)
                                    {
                                        if (modelItem.RemissionGuideCustomizedAdvancedTransportist != null)
                                        {
                                            modelItem.RemissionGuideCustomizedAdvancedTransportist.Document.id_userUpdate = ActiveUser.id;
                                            modelItem.RemissionGuideCustomizedAdvancedTransportist.Document.dateUpdate = DateTime.Now;
                                            modelItem.RemissionGuideCustomizedAdvancedTransportist.isActive = true;
                                            modelItem.RemissionGuideCustomizedAdvancedTransportist.valueTotal = modelItem.RemissionGuideTransportation?.advancePrice ?? 0;
                                        }
                                        else
                                        {
                                            RemissionGuideCustomizedAdvancedTransportist rgcat = new RemissionGuideCustomizedAdvancedTransportist();
                                            rgcat.Document = new Document();

                                            #region CREACION DEL DOCUMENTO
                                            Document docAdvanceTransportist = new Document();

                                            docAdvanceTransportist.id_userCreate = ActiveUser.id;
                                            docAdvanceTransportist.dateCreate = DateTime.Now;
                                            docAdvanceTransportist.id_userUpdate = ActiveUser.id;
                                            docAdvanceTransportist.dateUpdate = DateTime.Now;

                                            docAdvanceTransportist.emissionDate = modelItem.Document.emissionDate;

                                            DocumentType docTypeAdvanceTransportist = db.DocumentType.FirstOrDefault(fod => fod.code == "78");

                                            docAdvanceTransportist.sequential = docTypeAdvanceTransportist?.currentNumber ?? 0;
                                            docAdvanceTransportist.number = GetDocumentNumber(docTypeAdvanceTransportist.id, id_ep);
                                            docAdvanceTransportist.DocumentType = docTypeAdvanceTransportist;
                                            docAdvanceTransportist.id_documentType = docTypeAdvanceTransportist.id;

                                            DocumentState docStateAdvanceTransportis = db.DocumentState
                                                                                        .FirstOrDefault(s => s.code == "01"
                                                                                        && s.tbsysDocumentTypeDocumentState
                                                                                        .Any(a => a.DocumentType.code == "78"));

                                            docAdvanceTransportist.DocumentState = docStateAdvanceTransportis;
                                            docAdvanceTransportist.id_documentState = docStateAdvanceTransportis.id;

                                            docAdvanceTransportist.id_emissionPoint = _epTmp?.id ?? 0;
                                            docAdvanceTransportist.EmissionPoint = _epTmp;

                                            //Actualiza Secuencial Viatico Personal Asignado
                                            if (docTypeAdvanceTransportist != null)
                                            {
                                                docTypeAdvanceTransportist.currentNumber = docTypeAdvanceTransportist.currentNumber + 1;
                                                db.DocumentType.Attach(docTypeAdvanceTransportist);
                                                db.Entry(docTypeAdvanceTransportist).State = EntityState.Modified;
                                            }
                                            rgcat.Document = docAdvanceTransportist;
                                            #endregion

                                            rgcat.isActive = true;
                                            rgcat.hasPayment = false;
                                            rgcat.id_RemissionGuide = modelItem.id;
                                            rgcat.valueTotal = modelItem.RemissionGuideTransportation?.advancePrice ?? 0;
                                            db.RemissionGuideCustomizedAdvancedTransportist.Add(rgcat);
                                        }
                                    }
                                    else
                                    {
                                        if (modelItem.RemissionGuideCustomizedAdvancedTransportist != null)
                                        {
                                            modelItem.RemissionGuideCustomizedAdvancedTransportist.Document.id_userUpdate = ActiveUser.id;
                                            modelItem.RemissionGuideCustomizedAdvancedTransportist.Document.dateUpdate = DateTime.Now;
                                            modelItem.RemissionGuideCustomizedAdvancedTransportist.isActive = false;
                                            modelItem.RemissionGuideCustomizedAdvancedTransportist.valueTotal = 0;
                                        }
                                    }
                                }
                                #endregion

                                #region INFORMATION CUSTOMIZABLE FOR WAREHOUSE SEQUENTIAL
                                if (det.nameObjectTable == "DispatchMaterialSequential")
                                {
                                    var lstRgdm = modelItem.RemissionGuideDispatchMaterial
                                                            .Where(w => w.isActive && w.sourceExitQuantity > 0)
                                                            .Select(s => s.id_warehouse).Distinct().ToList();

                                    var lstDms = modelItem.DispatchMaterialSequential
                                                            .Select(s => new { id_Warehouse = s.id_Warehouse }).Distinct().ToList();

                                    foreach (int i in lstRgdm)
                                    {
                                        Warehouse wTmp = db.Warehouse.FirstOrDefault(fod => fod.id == i);
                                        if (wTmp != null)
                                        {
                                            SequentialGeneral sqTmp = db.SequentialGeneral.FirstOrDefault(fod => fod.id_break1 == wTmp.id && fod.nameObject1 == "Warehouse");

                                            int id_Wtmp = lstDms.FirstOrDefault(fod => fod.id_Warehouse == wTmp.id)?.id_Warehouse ?? 0;

                                            if (sqTmp != null && id_Wtmp == 0)
                                            {
                                                DispatchMaterialSequential dmsTmp = new DispatchMaterialSequential();
                                                dmsTmp.id_Warehouse = wTmp.id;
                                                dmsTmp.id_RemissionGuide = modelItem.id;
                                                dmsTmp.sequential = sqTmp.sequential1;

                                                db.DispatchMaterialSequential.Add(dmsTmp);

                                                sqTmp.sequential1 = sqTmp.sequential1 + 1;
                                                db.SequentialGeneral.Attach(sqTmp);
                                                db.Entry(sqTmp).State = EntityState.Modified;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region TRANSPORTATION INFORMATION CUSTOMIZABLE
                                if (modelItem.RemissionGuideTransportation.isOwn)
                                {
                                    modelItem.RemissionGuideTransportationCustomizedInformation = modelItem.RemissionGuideTransportationCustomizedInformation ?? new RemissionGuideTransportationCustomizedInformation();
                                }
                                #endregion

                                #region TRANSPORTATION BUY ICE INFORMATION
                                if (det.nameObjectTable == "RemissionGuideCustomizedIceBuyInformation")
                                {
                                    string _strIceThirdId = db.Setting.FirstOrDefault(fod => fod.code == "CODHIT")?.value;

                                    if (!string.IsNullOrEmpty(_strIceThirdId) && Convert.ToInt32(_strIceThirdId) > 0)
                                    {
                                        int _iIceThirdId = Convert.ToInt32(_strIceThirdId);

                                        var _rgdmTmp = modelItem.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id_item == _iIceThirdId && fod.sourceExitQuantity > 0);

                                        if (_rgdmTmp != null)
                                        {
                                            if (modelItem.RemissionGuideCustomizedIceBuyInformation != null)
                                            {
                                                modelItem.RemissionGuideCustomizedIceBuyInformation.Document.id_userUpdate = ActiveUser.id;
                                                modelItem.RemissionGuideCustomizedIceBuyInformation.Document.dateUpdate = DateTime.Now;
                                                modelItem.RemissionGuideCustomizedIceBuyInformation.isActive = true;
                                                modelItem.RemissionGuideCustomizedIceBuyInformation.quantityIceBagsRequested = Convert.ToInt32(_rgdmTmp.sourceExitQuantity);
                                            }
                                            else
                                            {
                                                RemissionGuideCustomizedIceBuyInformation _rgcbi = new RemissionGuideCustomizedIceBuyInformation();
                                                _rgcbi.Document = new Document();
                                                #region CREACION DEL DOCUMENTO
                                                Document docBuyIceThird = new Document();

                                                docBuyIceThird.id_userCreate = ActiveUser.id;
                                                docBuyIceThird.dateCreate = DateTime.Now;
                                                docBuyIceThird.id_userUpdate = ActiveUser.id;
                                                docBuyIceThird.dateUpdate = DateTime.Now;

                                                docBuyIceThird.emissionDate = modelItem.Document.emissionDate;

                                                DocumentType docTypeBuyIceThird = db.DocumentType.FirstOrDefault(fod => fod.code == "80");

                                                docBuyIceThird.sequential = docTypeBuyIceThird?.currentNumber ?? 0;
                                                docBuyIceThird.number = GetDocumentNumber(docTypeBuyIceThird.id, id_ep);
                                                docBuyIceThird.DocumentType = docTypeBuyIceThird;

                                                DocumentState docStateBuyIce = db.DocumentState
                                                                                            .FirstOrDefault(s => s.code == "01"
                                                                                            && s.tbsysDocumentTypeDocumentState
                                                                                            .Any(a => a.DocumentType.code == "80"));

                                                docBuyIceThird.DocumentState = docStateBuyIce;
                                                docBuyIceThird.id_documentState = docStateBuyIce.id;

                                                docBuyIceThird.id_emissionPoint = _epTmp?.id ?? 0;
                                                docBuyIceThird.EmissionPoint = _epTmp;

                                                //Actualiza Secuencial Compra Hielo
                                                if (docTypeBuyIceThird != null)
                                                {
                                                    docTypeBuyIceThird.currentNumber = docTypeBuyIceThird.currentNumber + 1;
                                                    db.DocumentType.Attach(docTypeBuyIceThird);
                                                    db.Entry(docTypeBuyIceThird).State = EntityState.Modified;
                                                }
                                                #endregion

                                                _rgcbi.Document = docBuyIceThird;
                                                _rgcbi.quantityIceBagsRequested = Convert.ToInt32(_rgdmTmp.sourceExitQuantity);
                                                _rgcbi.isActive = true;
                                                _rgcbi.id_ProviderIceBags = (itemCustIceBuy != null && itemCustIceBuy.id_ProviderIceBags > 0) ? itemCustIceBuy.id_ProviderIceBags : null;

                                                modelItem.RemissionGuideCustomizedIceBuyInformation = modelItem.RemissionGuideCustomizedIceBuyInformation ?? _rgcbi;

                                                db.RemissionGuideCustomizedIceBuyInformation.Attach(_rgcbi);
                                                db.Entry(_rgcbi).State = EntityState.Added;
                                            }
                                        }
                                        else
                                        {
                                            if (modelItem.RemissionGuideCustomizedIceBuyInformation != null)
                                            {
                                                modelItem.RemissionGuideCustomizedIceBuyInformation.Document.id_userUpdate = ActiveUser.id;
                                                modelItem.RemissionGuideCustomizedIceBuyInformation.Document.dateUpdate = DateTime.Now;
                                                modelItem.RemissionGuideCustomizedIceBuyInformation.isActive = false;
                                                modelItem.RemissionGuideCustomizedIceBuyInformation.quantityIceBagsRequested = 0;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion

                        db.RemissionGuide.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        modelItem.getLiquidationInformation();

                        TempData["remissionGuide"] = modelItem;

                        TempData.Keep("remissionGuide");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + modelItem.Document.number + " guardada exitosamente");
                        _messageAnswer = SuccessMessage("Guía de Remisión: " + modelItem.Document.number + ((approve) ? " aprobada" : " guardada") + " exitosamente");
                        _codeStateDocument = (approve) ? "03" : "01";
                        _nameStateDocument = (approve) ? "APROBADO" : "PENDIENTE";
                        _strNumberDocAnswer = modelItem.Document.number;
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("remissionGuide");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                        _messageAnswer = ErrorMessage(e.Message);
                        MetodosEscrituraLogs.EscribeMensajeLog(e.Message, ruta, "LOGISTICAADDNEW", "PROD");
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("remissionGuide");

            #region AUDITORIA DE GUIA DE REMISION
            #endregion

            #region CERRADO DE DOCUMENTOS Y GENERACION REQUERIMIENTO BODEGA
            if (approve)
            {
                if (item.id > 0)
                {
                    List<int> iLstPod = db.RemissionGuideDetailPurchaseOrderDetail
                                        .Where(w => w.RemissionGuideDetail.RemissionGuide.id == modelItem.id)
                                        .Select(s => s.id_purchaseOrderDetail)
                                        .ToList();

                    if (iLstPod != null && iLstPod.Count > 0)
                    {
                        var _iLstPo = db.PurchaseOrderDetail
                                    .Where(w => iLstPod.Contains(w.id))
                                    .GroupBy(g => g.PurchaseOrder.id)
                                    .Select(s => new
                                    {
                                        id_po = s.FirstOrDefault().PurchaseOrder.id,
                                        qAp = s.Sum(su => su.quantityApproved),
                                        qPe = s.Sum(su => su.quantityDispatched)
                                    }).ToList();

                        if (_iLstPo != null && _iLstPo.Count > 0)
                        {
                            #region Listado de Ordenes de Compra
                            foreach (var _de in _iLstPo)
                            {
                                if (_de.id_po > 0)
                                {
                                    if ((_de.qAp - _de.qPe) <= 0)
                                    {
                                        Document _doPo = db.Document.FirstOrDefault(fod => fod.id == _de.id_po);

                                        if (_doPo != null)
                                        {
                                            using (DbContextTransaction trans = db.Database.BeginTransaction())
                                            {
                                                try
                                                {
                                                    _doPo.isOpen = false;
                                                    db.Document.Attach(_doPo);
                                                    db.Entry(_doPo).State = EntityState.Modified;
                                                    db.SaveChanges();
                                                    trans.Commit();
                                                    _answerCloseDoc = 1;
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            #region AUDITORIA CERRADO DE DOCUMENTO ORDEN DE COMPRA
                                            if (_answerCloseDoc == 1)
                                            {
                                                DocumentLogDTO _doTmp = new DocumentLogDTO();
                                                _doTmp.id = _de.id_po;
                                                _doTmp.code_Action = "CRD";
                                                _doTmp.id_User = ActiveUser.id;
                                                _doTmp.description = "EJECUCION MANUAL";
                                                Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            #endregion

            modelItem.isManual = returnRemissionGuide.isManual;

            _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(item.id, _codeStateDocument);

            _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
            _AnswerfaRG.messageAnswer = _messageAnswer;
            _AnswerfaRG.nameStateAnswer = _nameStateDocument;
            _AnswerfaRG.codeStateAnswer = _codeStateDocument;
            _AnswerfaRG.strNumberDocAnswer = _strNumberDocAnswer;
            return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GUIA DE REMISION XML

        private void GeneraGuiaElectronicaXml(RemissionGuide pRemissionGuide)
        {
            try
            {
                ElectronicDocument pElectronicDocument = new ElectronicDocument();
                bool isNew = false;
                if (pRemissionGuide != null)
                {
                    // FACTURACION ELECTRONICA - GENERACION DE XML
                    if (pRemissionGuide.Document.DocumentType.isElectronic)
                    {
                        ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == this.ActiveCompanyId && e.sriCode.Equals("01"));

                        if (electronicState != null)
                        {
                            if (pRemissionGuide.Document.ElectronicDocument == null)
                            {
                                pElectronicDocument = new ElectronicDocument
                                {
                                    id_electronicDocumentState = electronicState.id
                                };
                                isNew = true;
                            }
                            else
                            {
                                pElectronicDocument = pRemissionGuide.Document.ElectronicDocument;
                            }

                            pElectronicDocument.isGenerated = false;

                            var ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
                            var xmlGuiaRemision = DB2XML.GuiaRemision2Xml(pRemissionGuide.id);
                            var xmlout1 = SerializeToXml(xmlGuiaRemision, ns);

                            String xml = xmlout1.OuterXml;
                            xml = xml.Replace(@"p2:nil=""true""", "");
                            xml = xml.Replace(@"xmlns:p2=""http://www.w3.org/2001/XMLSchema-instance"" /", "/");
                            xml = xml.Replace("<infoAdicional  />", "");

                            xmlout1.LoadXml(xml);

                            pElectronicDocument.xml = xmlout1.OuterXml;
                        }
                    }
                    else
                    {
                        return;
                    }

                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (isNew)
                            {
                                db.ElectronicDocument.Add(pElectronicDocument);
                            }
                            else
                            {
                                db.ElectronicDocument.Attach(pElectronicDocument);
                                db.Entry(pElectronicDocument).State = EntityState.Modified;
                            }

                            pElectronicDocument.id = pRemissionGuide.id;
                            db.SaveChanges();
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public XmlDocument SerializeToXml<T>(T source, XmlSerializerNamespaces ns)
        {
            var document = new XmlDocument();
            var navigator = document.CreateNavigator();

            using (var writer = navigator.AppendChild())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, source, ns);
            }
            return document;
        }

        public XmlDocument GenerateXML(RemissionGuide pRemissionGuide, int company)
        {
            XmlDocument xmlout1 = new XmlDocument();

            int? id_documentState = pRemissionGuide.Document.id_documentState;
            string code_documentState = db.DocumentState.FirstOrDefault(r => r.id == id_documentState)?.code ?? "";

            if (code_documentState != "09") return null;
            if (pRemissionGuide.Document?.DocumentType?.isElectronic == false) return null;

            ElectronicDocument _electronicDocument = pRemissionGuide.Document.ElectronicDocument ?? new ElectronicDocument();
            ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == company && e.sriCode.Equals("01"));
            if (electronicState == null) return null;

            _electronicDocument = new ElectronicDocument
            {
                id_electronicDocumentState = electronicState.id
            };

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            var xmlGuiaRemision = DB2XML.GuiaRemision2Xml(pRemissionGuide);
            xmlout1 = SerializeToXml(xmlGuiaRemision, ns);

            String xml = xmlout1.OuterXml;
            xml = xml.Replace(@"p2:nil=""true""", "");
            xml = xml.Replace(@"xmlns:p2=""http://www.w3.org/2001/XMLSchema-instance"" /", "/");
            xml = xml.Replace("<infoAdicional  />", "");

            xmlout1.LoadXml(xml);

            _electronicDocument.xml = xmlout1.OuterXml;

            pRemissionGuide.Document.ElectronicDocument = _electronicDocument;

            return xmlout1;
        }

        #endregion

        #region REMISSION GUIDE EDITFORM

        public void LoadpartialComboxTransportTariff(int? id_shippingType, int? id_TransportTariffType)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            try
            {
                if (id_shippingType != null && id_shippingType > 0)
                {
                    if (id_shippingType == 8) { id_shippingType = 1; }

                    var TransportTariffTyperAux = db.TransportTariffType.Where(t => t.isActive && t.id_shippingType == id_shippingType).ToList();

                    if (id_TransportTariffType != null && id_TransportTariffType > 0)
                    {
                        var TransportTariffTyperCurrentAux = db.TransportTariffType.FirstOrDefault(fod => fod.id == id_TransportTariffType && fod.id_shippingType == id_shippingType);
                        if (TransportTariffTyperCurrentAux != null && !TransportTariffTyperAux.Contains(TransportTariffTyperCurrentAux)) TransportTariffTyperAux.Add(TransportTariffTyperCurrentAux);
                    }

                    TempData["TransportTariffTypeByshippingType"] = TransportTariffTyperAux.Select(s => new
                    {
                        s.id,
                        name = s.name
                    }).OrderBy(t => t.id).ToList();

                    TempData.Keep("TransportTariffTypeByshippingType");
                }
            }
            catch
            {
            }
        }

        public void LoadpartialComboxTransportTariffVehicleType(int? id_TransportTariffType, int? id_vehicle)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            try
            {
                List<int> listVehicleType = new List<int>();

                if (id_TransportTariffType != null && id_TransportTariffType > 0)
                {
                    var TransportTariffTyperVehicleType = db.TransportTariffType_VehicleType.Where(t => t.isActive && t.id_transportTariffType == id_TransportTariffType).ToList();

                    if (TransportTariffTyperVehicleType != null && TransportTariffTyperVehicleType.Count > 0)
                    {
                        var listVehicleTypeax = (from e in TransportTariffTyperVehicleType
                                                 select e.id_vehicleType).ToList();

                        if (listVehicleTypeax != null && listVehicleTypeax.Count > 0)
                        {
                            listVehicleType.AddRange(listVehicleTypeax);
                        }
                    }
                    var Vehicle = db.Vehicle.Where(v => v.isActive && listVehicleType.Contains(v.id_VehicleType)).ToList();

                    if (id_vehicle != null)
                    {
                        var VehicleCurrentAux = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehicle && listVehicleType.Contains(fod.id_VehicleType));

                        if (VehicleCurrentAux != null && !Vehicle.Contains(VehicleCurrentAux)) Vehicle.Add(VehicleCurrentAux);
                    }

                    TempData["TransportTariffTypeVehicleType"] = Vehicle.OrderBy(t => t.id).ToList();
                    TempData.Keep("TransportTariffTypeVehicleType");
                }
            }
            catch
            {
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuide(int id, int[] orderDetails)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(o => o.id == id);
            var isNew = false;
            if (remissionGuide != null)
            {
                remissionGuide.isManual = true;
            }
            if (remissionGuide != null && remissionGuide.RemissionGuideTransportation != null)
            {
                if (remissionGuide.RemissionGuideTransportation.isOwn == true)
                {
                    remissionGuide.RemissionGuideTransportation.driverNameThird = remissionGuide.RemissionGuideTransportation.driverName;
                    remissionGuide.RemissionGuideTransportation.carRegistrationThird = remissionGuide.RemissionGuideTransportation.carRegistration;
                }
            }
            if (remissionGuide != null && remissionGuide.RemissionGuideDetail != null)
            {
                var lstRmgDetail = remissionGuide.RemissionGuideDetail.Select(s => s.id).ToList();

                var lstRmgDetail2 = db.RemissionGuideDetailPurchaseOrderDetail.Where(w => lstRmgDetail.Contains(w.id_remissionGuideDetail)).ToList();
                if (lstRmgDetail2 != null && lstRmgDetail2.Count > 0)
                {
                    remissionGuide.isManual = false;
                }
                else
                {
                    remissionGuide.isManual = true;
                }
            }
            DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("08"));

            if (remissionGuide == null)
            {
                isNew = true;

                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

                if (orderDetails == null)
                {
                    remissionGuide = new RemissionGuide
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
                        despachureDate = DateTime.Now,
                        arrivalDate = DateTime.Now,
                        returnDate = DateTime.Now,
                        startAdress = ActiveSucursal.address,
                        RemissionGuideDetail = new List<RemissionGuideDetail>(),
                        RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>(),
                        isInternal = false,
                        isManual = true,
                        id_personProcessPlant = 0
                    };
                }
                else
                {
                    var id_detail = orderDetails[0];
                    remissionGuide = new RemissionGuide
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
                        despachureDate = DateTime.Now,
                        arrivalDate = DateTime.Now,
                        returnDate = DateTime.Now,
                        startAdress = ActiveSucursal.address,
                        RemissionGuideDetail = new List<RemissionGuideDetail>(),
                        RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>(),
                        isInternal = false,
                        isManual = true,
                        id_personProcessPlant = db.PurchaseOrderDetail.FirstOrDefault(od => od.id == id_detail) != null
                            ? db.PurchaseOrderDetail.FirstOrDefault(od => od.id == id_detail).PurchaseOrder.id_personProcessPlant
                            : 0
                    };
                }

                var personReceiverRG = db.Person.FirstOrDefault(fod => fod.id == ActiveUser.Employee.id
                        && fod.Rol.Any(a => a.name == "Recibidor"));

                if (personReceiverRG != null)
                {
                    remissionGuide.id_reciver = personReceiverRG.id;
                }
                remissionGuide.RemissionGuideTransportation = new RemissionGuideTransportation();
            }
            else
            {
                TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, remissionGuide.id_providerRemisionGuide);
                TempData.Keep("ProductionUnitProviderByProvider");

                TempData["ProductionUnitProviderPoolByUnitProvider"] = DataProviders.DataProviderProductionUnitProviderPool.ProductionUnitProviderPoolByUnitProvider(null, remissionGuide.id_productionUnitProvider);
                TempData.Keep("ProductionUnitProviderPoolByUnitProvider");

                LoadpartialComboxTransportTariff(remissionGuide.id_shippingType, remissionGuide.id_TransportTariffType);
                LoadpartialComboxTransportTariffVehicleType(remissionGuide.id_TransportTariffType, remissionGuide?.RemissionGuideTransportation?.id_vehicle);
                remissionGuide.RemissionGuideTransportation.id_TransportationType = remissionGuide.id_TransportTariffType;

                remissionGuide.getLiquidationInformation();
            }
            remissionGuide.RemissionGuideTransportation = remissionGuide.RemissionGuideTransportation ?? new RemissionGuideTransportation();
            if (orderDetails != null)
            {
                List<RemissionGuideDetail> remissionGuideDetails = new List<RemissionGuideDetail>();

                if (orderDetails.Count() > 0)
                {
                    int tmpE = orderDetails[0];
                    PurchaseOrderDetail podTemporal = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == tmpE);
                    TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, podTemporal.PurchaseOrder.id_provider);
                    TempData.Keep("ProductionUnitProviderByProvider");

                    TempData["ProductionUnitProviderPoolByUnitProvider"] = DataProviders.DataProviderProductionUnitProviderPool.ProductionUnitProviderPoolByUnitProvider(null, podTemporal.PurchaseOrder.id_productionUnitProvider);
                    TempData.Keep("ProductionUnitProviderPoolByUnitProvider");

                    remissionGuide.RemissionGuideTransportation = remissionGuide.RemissionGuideTransportation ?? new RemissionGuideTransportation();
                    remissionGuide.RemissionGuideTransportation.isOwn = !podTemporal.PurchaseOrder.requiredLogistic;

                    if (!remissionGuide.RemissionGuideTransportation.isOwn)
                    {
                        remissionGuide.RemissionGuideTransportation.id_FishingSiteRG = podTemporal?
                                                                                        .PurchaseOrder?
                                                                                        .ProductionUnitProvider?
                                                                                        .id_FishingSite ?? null;
                    }

                    remissionGuide.id_shippingType = podTemporal.PurchaseOrder.id_shippingType;

                    List<tbsysDocumentsPersonalizationDetail> lstDocPersDetail = db.tbsysDocumentsPersonalizationDetail
                                                                                .Where(fod => fod.id_DocumentType == documentType.id)
                                                                                .ToList();

                    if (lstDocPersDetail != null && lstDocPersDetail.Count > 0)
                    {
                        foreach (var det in lstDocPersDetail)
                        {
                            if (det.nameObjectTable == "RemissionGuideCustomizedInformation")
                            {
                                remissionGuide.RemissionGuideCustomizedInformation = new RemissionGuideCustomizedInformation();
                                remissionGuide.RemissionGuideCustomizedInformation.hasSecurity = podTemporal?.PurchaseOrder?.PurchaseOrderCustomizedInformation?.hasSecurity ?? false;
                            }
                        }
                    }

                    var id_TTT2 = podTemporal.PurchaseOrder?.ProductionUnitProvider?.id_shippingType ?? null;

                    if (id_TTT2 != null && id_TTT2 == 8)
                    {
                        id_TTT2 = 1;
                    }

                    remissionGuide.id_TransportTariffType = db.TransportTariffType
                        .FirstOrDefault(fod => fod.id_shippingType == id_TTT2 && fod.isInternal != true)?.id ?? null;

                    remissionGuide.RemissionGuideTransportation.id_TransportationType = remissionGuide.id_TransportTariffType;
                    remissionGuide.id_productionUnitProvider = podTemporal.PurchaseOrder.id_productionUnitProvider;

                    LoadpartialComboxTransportTariff(remissionGuide.id_shippingType, remissionGuide.id_TransportTariffType);
                    LoadpartialComboxTransportTariffVehicleType(remissionGuide.id_TransportTariffType, remissionGuide?.RemissionGuideTransportation?.id_vehicle);
                }

                foreach (var od in orderDetails)
                {
                    PurchaseOrderDetail tempPurchaseOrderDetail = db.PurchaseOrderDetail.FirstOrDefault(d => d.id == od);

                    if (tempPurchaseOrderDetail != null)
                    {
                        var id_TTT = tempPurchaseOrderDetail.PurchaseOrder?.ProductionUnitProvider?.PurchaseOrderShippingType?.id ?? null;
                        remissionGuide.id_TransportTariffType = db.TransportTariffType.FirstOrDefault(fod => fod.id_shippingType == id_TTT)?.id ?? null;
                    }

                    var quantityDispatchPendingAux = tempPurchaseOrderDetail.quantityApproved - tempPurchaseOrderDetail.quantityDispatched;
                    RemissionGuideDetail remissionGuideDetail = new RemissionGuideDetail
                    {
                        id = remissionGuide.RemissionGuideDetail.Count() > 0 ? remissionGuide.RemissionGuideDetail.Max(pld => pld.id) + 1 : 1,
                        id_item = tempPurchaseOrderDetail.id_item,
                        Item = db.Item.FirstOrDefault(i => i.id == tempPurchaseOrderDetail.id_item),
                        quantityOrdered = quantityDispatchPendingAux,
                        quantityDispatchPending = quantityDispatchPendingAux,
                        quantityProgrammed = quantityDispatchPendingAux,
                        quantityReceived = 0,
                        isActive = true,
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now,
                        RemissionGuideDetailPurchaseOrderDetail = new List<RemissionGuideDetailPurchaseOrderDetail>(),
                        RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>(),
                        id_Grammage = tempPurchaseOrderDetail.id_Grammage
                    };

                    remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail.Add(new RemissionGuideDetailPurchaseOrderDetail
                    {
                        id_purchaseOrderDetail = tempPurchaseOrderDetail.id,
                        quantity = quantityDispatchPendingAux
                    });
                    if (tempPurchaseOrderDetail.productionUnitProviderPoolreference != null)
                    {
                        remissionGuideDetail.productionUnitProviderPoolreference = tempPurchaseOrderDetail.productionUnitProviderPoolreference;
                    }
                    if (remissionGuide.id_reason <= 0)
                    {
                        remissionGuide.id_reason = tempPurchaseOrderDetail.PurchaseOrder.id_purchaseReason;
                    }

                    if (remissionGuide.id_protectiveProvider == null)
                    {
                        remissionGuide.id_protectiveProvider = tempPurchaseOrderDetail.PurchaseOrder.id_providerapparent;
                    }

                    if (remissionGuide.id_productionUnitProvider == null)
                    {
                        remissionGuide.id_productionUnitProvider = tempPurchaseOrderDetail.PurchaseOrder.id_productionUnitProvider;

                        if (remissionGuide.id_productionUnitProvider != null && remissionGuide.id_productionUnitProvider > 0)
                        {
                            remissionGuide.ProductionUnitProvider = db.ProductionUnitProvider.Where(x => x.id == remissionGuide.id_productionUnitProvider).FirstOrDefault();
                        }
                    }
                    //Se agrega el shipping type cuando este proviene de la orden de compra
                    if (remissionGuide.id_shippingType == null)
                    {
                        remissionGuide.id_shippingType = tempPurchaseOrderDetail.PurchaseOrder.id_shippingType;

                        if (remissionGuide.id_shippingType != null)
                        {
                            LoadpartialComboxTransportTariff(remissionGuide.id_shippingType, remissionGuide.id_TransportTariffType);
                            LoadpartialComboxTransportTariffVehicleType(remissionGuide.id_TransportTariffType, null);
                        }
                    }

                    if (remissionGuide.id_providerRemisionGuide == null)
                    {
                        remissionGuide.id_providerRemisionGuide = tempPurchaseOrderDetail.PurchaseOrder.id_provider;
                        TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, remissionGuide.id_providerRemisionGuide);
                        TempData.Keep("ProductionUnitProviderByProvider");
                    }

                    if (remissionGuide.id_protectiveProvider == null)
                    {
                        remissionGuide.id_protectiveProvider = tempPurchaseOrderDetail.PurchaseOrder.id_providerapparent;
                    }

                    if (remissionGuide.id_RemisionGuideReassignment != null && remissionGuide.id_RemisionGuideReassignment > 0)
                    {
                        int id_RemisionGuideReassignment = int.Parse(remissionGuide.id_RemisionGuideReassignment.ToString());
                        remissionGuide.RemissionGuide2 = db.RemissionGuide.Where(x => x.id == id_RemisionGuideReassignment).FirstOrDefault();
                    }

                    if (remissionGuide.id_buyer == null)
                    {
                        remissionGuide.id_buyer = tempPurchaseOrderDetail.PurchaseOrder.id_buyer;
                    }
                    //Unidad de Producción
                    if (remissionGuide.id_productionUnitProvider == null)
                    {
                        remissionGuide.id_productionUnitProvider = tempPurchaseOrderDetail.PurchaseOrder.id_productionUnitProvider;
                    }
                    //Lista de Precios
                    if (remissionGuide.id_priceList == null)
                    {
                        remissionGuide.id_priceList = tempPurchaseOrderDetail.PurchaseOrder.id_priceList;
                    }

                    //Certificado
                    if (remissionGuide.id_certification == null)
                    {
                        remissionGuide.id_certification = tempPurchaseOrderDetail.PurchaseOrder.id_certification;
                    }

                    if (tempPurchaseOrderDetail != null
                        && tempPurchaseOrderDetail.PurchaseOrder != null
                        && tempPurchaseOrderDetail.PurchaseOrder.Document != null
                        && tempPurchaseOrderDetail.PurchaseOrder.Document.description != ""
                        && tempPurchaseOrderDetail.PurchaseOrder.Document.description != null)

                    {
                        remissionGuide.descriptionpurchaseorder += (" Desc. OC" + tempPurchaseOrderDetail.PurchaseOrder.Document.number + ": " + tempPurchaseOrderDetail.PurchaseOrder.Document.description);
                    }

                    if (remissionGuide.arrivalDate == null)
                    {
                        remissionGuide.arrivalDate = tempPurchaseOrderDetail.PurchaseOrder.deliveryDate;
                    }

                    if (tempPurchaseOrderDetail.PurchaseOrder.deliveryDate != null)
                    {
                        remissionGuide.despachureDate = tempPurchaseOrderDetail.PurchaseOrder.deliveryDate;
                    }

                    if (tempPurchaseOrderDetail.PurchaseOrder.deliveryDate != null)
                    {
                        remissionGuide.returnDate = tempPurchaseOrderDetail.PurchaseOrder.deliveryDate;
                    }

                    if (tempPurchaseOrderDetail.PurchaseOrder.deliveryhour != null)
                    {
                        remissionGuide.despachurehour = tempPurchaseOrderDetail.PurchaseOrder.deliveryhour;
                    }

                    if (remissionGuide.route == "" || remissionGuide.route == null) remissionGuide.route = tempPurchaseOrderDetail.PurchaseOrder.ProductionUnitProvider?.address;

                    remissionGuide.RemissionGuideDetail.Add(remissionGuideDetail);

                    UpdateRemissionGuideDetailDispatchMaterialDetail(remissionGuide, remissionGuideDetail);

                    if (remissionGuide.RemissionGuideDispatchMaterial.Count() <= 0)
                    {
                        var itemMDD = db.Item.Where(w => w.InventoryLine.code.Equals("MI") && (w.ItemType.code.Equals("MDD")) && w.isActive && w.id_company == this.ActiveCompanyId);//"MI": Linea de Inventario Materiales e Insumos y "MDD": Tipo de Producto Materiales de Despacho

                        foreach (var iimdd in itemMDD)
                        {
                            RemissionGuideDispatchMaterial remissionGuideDispatchMaterial = new RemissionGuideDispatchMaterial
                            {
                                id = remissionGuide.RemissionGuideDispatchMaterial.Count() > 0 ? remissionGuide.RemissionGuideDispatchMaterial.Max(pld => pld.id) + 1 : 1,
                                id_item = iimdd.id,
                                Item = db.Item.FirstOrDefault(i => i.id == iimdd.id),
                                quantityRequiredForPurchase = 0,
                                sourceExitQuantity = 0,
                                sendedDestinationQuantity = 0,
                                arrivalDestinationQuantity = 0,
                                arrivalGoodCondition = 0,
                                arrivalBadCondition = 0,
                                manual = false,
                                isActive = true,
                                id_userCreate = ActiveUser.id,
                                dateCreate = DateTime.Now,
                                id_userUpdate = ActiveUser.id,
                                dateUpdate = DateTime.Now,
                                RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>()
                            };
                            remissionGuide.RemissionGuideDispatchMaterial.Add(remissionGuideDispatchMaterial);

                            var remissionGuideDetailDispatchMaterialDetail = new RemissionGuideDetailDispatchMaterialDetail
                            {
                                RemissionGuideDetail = remissionGuideDetail,
                                id_remissionGuideDetail = remissionGuideDetail.id,
                                RemissionGuideDispatchMaterial = remissionGuideDispatchMaterial,
                                id_remissionGuideDispatchMaterial = remissionGuideDispatchMaterial.id,
                                quantity = 0
                            };
                            remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail.Add(remissionGuideDetailDispatchMaterialDetail);
                            remissionGuideDispatchMaterial.RemissionGuideDetailDispatchMaterialDetail.Add(remissionGuideDetailDispatchMaterialDetail);
                        }
                    }
                    string _codeIce = db.Setting.FirstOrDefault(fod => fod.code == "CODHIT")?.value ?? "0";
                    int _iCodeIce = Convert.ToInt32(_codeIce);
                    if (_iCodeIce > 0)
                    {
                        RemissionGuideDispatchMaterial rgdTmp = remissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id_item == _iCodeIce);
                        if (rgdTmp != null) rgdTmp.sourceExitQuantity = 0;
                    }
                }
                remissionGuide.isManual = false;
            }

            if (remissionGuide.isInternal == null) remissionGuide.isInternal = false;

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");
            if (!remissionGuide.isManual.Value && isNew && remissionGuide.RemissionGuideTransportation != null && remissionGuide.RemissionGuideTransportation.isOwn == false)
            {
                RGParamPriceFreight rgTmp2 = new RGParamPriceFreight();
                rgTmp2.id_TransportTariff = remissionGuide.id_TransportTariffType;
                rgTmp2.id_FishingSite = remissionGuide.RemissionGuideTransportation.id_FishingSiteRG;
                remissionGuide.RemissionGuideTransportation.valuePrice = CalculatePriceFreight(rgTmp2);
            }

            var duplicateItem = remissionGuide
                                         .RemissionGuideDispatchMaterial
                                         .Where(r => r.isActive)
                                         .GroupBy(r => r.id_item)
                                         .Select(r => new
                                         {
                                             _cantidad = r.Count(),
                                             _item = r.Key
                                         })
                                         .Where(r => r._cantidad > 1)
                                         .Count();
            if (duplicateItem > 0)
            {
                this.ViewBag.duplicateItem = "S";
            }

            this.ViewBag.activeCompany = this.ActiveCompanyId;
            this.ViewBag.isManual = remissionGuide.isManual;

            return PartialView("_FormEditRemissionGuide", remissionGuide);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuideForReassignment(int id, int[] rgs)
        {
            int[] ids = null;
            if (TempData["PurchaseDetailsForReassignment"] != null)
            {
                TempData.Keep("PurchaseDetailsForReassignment");
                ids = (int[])TempData["PurchaseDetailsForReassignment"];
            }

            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(o => o.id == id);
            if (remissionGuide != null)
            {
                remissionGuide.isManual = false;
            }

            DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("08"));

            if (remissionGuide == null)
            {
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");
                this.ViewBag.isManual = false;

                remissionGuide = new RemissionGuide
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

                    RemissionGuideDetail = new List<RemissionGuideDetail>(),
                    RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>(),
                    isInternal = false,
                    isManual = false
                };

                if (rgs != null && rgs.Count() == 1 && ids != null && ids.Count() > 0)
                {
                    #region ASIGNACION DE GUIA DE REMISION
                    int tmpE = rgs[0];
                    RemissionGuide _RgToReassign = db.RemissionGuide.FirstOrDefault(fod => fod.id == tmpE);

                    if (_RgToReassign != null)
                    {
                        #region ASIGNACION DE DATOS GENERALES

                        remissionGuide.id_reciver = _RgToReassign.id_reciver;
                        remissionGuide.id_reason = _RgToReassign.id_reason;
                        remissionGuide.startAdress = _RgToReassign.startAdress;
                        remissionGuide.despachureDate = _RgToReassign.despachureDate;
                        remissionGuide.arrivalDate = _RgToReassign.arrivalDate;
                        remissionGuide.returnDate = _RgToReassign.returnDate;
                        remissionGuide.uniqueCustomDocument = _RgToReassign.uniqueCustomDocument;
                        remissionGuide.isExport = _RgToReassign.isExport;
                        remissionGuide.isInternal = _RgToReassign.isInternal;
                        remissionGuide.id_shippingType = _RgToReassign.id_shippingType;
                        remissionGuide.hasEntrancePlanctProduction = _RgToReassign.hasEntrancePlanctProduction;
                        remissionGuide.hasExitPlanctProduction = _RgToReassign.hasExitPlanctProduction;
                        remissionGuide.despachurehour = _RgToReassign.despachurehour;
                        remissionGuide.id_TransportTariffType = _RgToReassign.id_TransportTariffType;
                        remissionGuide.id_tbsysCatalogState = _RgToReassign.id_tbsysCatalogState;
                        remissionGuide.despachureHourDt = _RgToReassign.despachureHourDt;
                        remissionGuide.id_RemissionGuideType = db.RemissionGuideType.FirstOrDefault(fod => fod.code == "TR").id;

                        #endregion

                        #region ASIGNACION DE MATERIALES DE DESPACHO
                        remissionGuide.RemissionGuideDispatchMaterial = remissionGuide.RemissionGuideDispatchMaterial ?? new List<RemissionGuideDispatchMaterial>();

                        if (_RgToReassign.RemissionGuideDispatchMaterial != null)
                        {
                            List<RemissionGuideDispatchMaterial> __RgToReassignDispatchMaterial = _RgToReassign.RemissionGuideDispatchMaterial.ToList();
                            foreach (var _rgrDispatchMaterial in _RgToReassign.RemissionGuideDispatchMaterial)
                            {
                                RemissionGuideDispatchMaterial _rgrDisMatTmp = new RemissionGuideDispatchMaterial();
                                _rgrDisMatTmp.id_item = _rgrDispatchMaterial.id_item;
                                _rgrDisMatTmp.quantityRequiredForPurchase = _rgrDispatchMaterial.quantityRequiredForPurchase;
                                _rgrDisMatTmp.sourceExitQuantity = _rgrDispatchMaterial.sourceExitQuantity;
                                _rgrDisMatTmp.sendedDestinationQuantity = _rgrDispatchMaterial.sendedDestinationQuantity;
                                _rgrDisMatTmp.arrivalDestinationQuantity = _rgrDispatchMaterial.arrivalDestinationQuantity;
                                _rgrDisMatTmp.arrivalGoodCondition = _rgrDispatchMaterial.arrivalGoodCondition;
                                _rgrDisMatTmp.arrivalBadCondition = _rgrDispatchMaterial.arrivalBadCondition;
                                _rgrDisMatTmp.manual = _rgrDispatchMaterial.manual;
                                _rgrDisMatTmp.isActive = _rgrDispatchMaterial.isActive;
                                _rgrDisMatTmp.id_userCreate = _rgrDispatchMaterial.id_userCreate;
                                _rgrDisMatTmp.dateCreate = DateTime.Now;
                                _rgrDisMatTmp.id_userUpdate = _rgrDispatchMaterial.id_userUpdate;
                                _rgrDisMatTmp.dateUpdate = DateTime.Now;
                                _rgrDisMatTmp.id_warehouse = _rgrDispatchMaterial.id_warehouse;
                                _rgrDisMatTmp.id_warehouselocation = _rgrDispatchMaterial.id_warehouselocation;

                                remissionGuide.RemissionGuideDispatchMaterial.Add(_rgrDisMatTmp);
                            }
                        }

                        #endregion

                        #region ENTRADA Y SALIDA DE VEHICULOS
                        if (_RgToReassign.RemissionGuideControlVehicle != null)
                        {
                            remissionGuide.RemissionGuideControlVehicle = new RemissionGuideControlVehicle();
                            remissionGuide.RemissionGuideControlVehicle.exitDateProductionBuilding = _RgToReassign.RemissionGuideControlVehicle.exitDateProductionBuilding;
                            remissionGuide.RemissionGuideControlVehicle.exitTimeProductionBuilding = _RgToReassign.RemissionGuideControlVehicle.exitTimeProductionBuilding;
                            remissionGuide.RemissionGuideControlVehicle.entranceDateProductionUnitProviderBuilding = _RgToReassign.RemissionGuideControlVehicle.entranceDateProductionUnitProviderBuilding;
                            remissionGuide.RemissionGuideControlVehicle.entranceTimeProductionUnitProviderBuilding = _RgToReassign.RemissionGuideControlVehicle.entranceTimeProductionUnitProviderBuilding;
                            remissionGuide.RemissionGuideControlVehicle.exitDateProductionUnitProviderBuilding = _RgToReassign.RemissionGuideControlVehicle.exitDateProductionUnitProviderBuilding;
                            remissionGuide.RemissionGuideControlVehicle.exitTimeProductionUnitProviderBuilding = _RgToReassign.RemissionGuideControlVehicle.exitTimeProductionUnitProviderBuilding;
                            remissionGuide.RemissionGuideControlVehicle.entranceDateProductionBuilding = _RgToReassign.RemissionGuideControlVehicle.entranceDateProductionBuilding;
                            remissionGuide.RemissionGuideControlVehicle.entranceTimeProductionBuilding = _RgToReassign.RemissionGuideControlVehicle.entranceTimeProductionBuilding;
                            remissionGuide.RemissionGuideControlVehicle.hasEntrancePlanctProduction = _RgToReassign.RemissionGuideControlVehicle.hasEntrancePlanctProduction;
                            remissionGuide.RemissionGuideControlVehicle.hasExitPlanctProduction = _RgToReassign.RemissionGuideControlVehicle.hasExitPlanctProduction;
                            remissionGuide.RemissionGuideControlVehicle.ObservationEntrance = _RgToReassign.RemissionGuideControlVehicle.ObservationEntrance;
                            remissionGuide.RemissionGuideControlVehicle.ObservationExit = _RgToReassign.RemissionGuideControlVehicle.ObservationExit;
                            remissionGuide.RemissionGuideControlVehicle.hasSentEmail = _RgToReassign.RemissionGuideControlVehicle.hasSentEmail;
                        }
                        #endregion
                    }
                    #endregion

                    #region ASIGNACION DE ORDENES DE COMPRA PARA MATERIA PRIMA
                    foreach (var od in ids)
                    {
                        PurchaseOrderDetail tempPurchaseOrderDetail = db.PurchaseOrderDetail.FirstOrDefault(d => d.id == od);

                        if (tempPurchaseOrderDetail != null)
                        {
                            var id_TTT = tempPurchaseOrderDetail.PurchaseOrder?.ProductionUnitProvider?.PurchaseOrderShippingType?.id ?? null;
                            remissionGuide.id_TransportTariffType = db.TransportTariffType.FirstOrDefault(fod => fod.id_shippingType == id_TTT)?.id ?? null;
                        }

                        var quantityDispatchPendingAux = tempPurchaseOrderDetail.quantityApproved - tempPurchaseOrderDetail.quantityDispatched;
                        RemissionGuideDetail remissionGuideDetail = new RemissionGuideDetail
                        {
                            id = remissionGuide.RemissionGuideDetail.Count() > 0 ? remissionGuide.RemissionGuideDetail.Max(pld => pld.id) + 1 : 1,
                            id_item = tempPurchaseOrderDetail.id_item,
                            Item = db.Item.FirstOrDefault(i => i.id == tempPurchaseOrderDetail.id_item),
                            quantityOrdered = quantityDispatchPendingAux,
                            quantityDispatchPending = quantityDispatchPendingAux,
                            quantityProgrammed = quantityDispatchPendingAux,
                            quantityReceived = 0,
                            isActive = true,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            RemissionGuideDetailPurchaseOrderDetail = new List<RemissionGuideDetailPurchaseOrderDetail>(),
                            RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>(),
                            id_Grammage = tempPurchaseOrderDetail.id_Grammage
                        };

                        remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail.Add(new RemissionGuideDetailPurchaseOrderDetail
                        {
                            id_purchaseOrderDetail = tempPurchaseOrderDetail.id,
                            quantity = quantityDispatchPendingAux
                        });
                        if (tempPurchaseOrderDetail.productionUnitProviderPoolreference != null)
                        {
                            remissionGuideDetail.productionUnitProviderPoolreference = tempPurchaseOrderDetail.productionUnitProviderPoolreference;
                        }

                        if (remissionGuide.id_protectiveProvider == null)
                        {
                            remissionGuide.id_protectiveProvider = tempPurchaseOrderDetail.PurchaseOrder.id_providerapparent;
                        }

                        if (remissionGuide.id_productionUnitProvider == null)
                        {
                            remissionGuide.id_productionUnitProvider = tempPurchaseOrderDetail.PurchaseOrder.id_productionUnitProvider;

                            if (remissionGuide.id_productionUnitProvider != null && remissionGuide.id_productionUnitProvider > 0)
                            {
                                remissionGuide.ProductionUnitProvider = db.ProductionUnitProvider.Where(x => x.id == remissionGuide.id_productionUnitProvider).FirstOrDefault();
                            }
                        }
                        //Se agrega el shipping type cuando este proviene de la orden de compra
                        if (remissionGuide.id_shippingType == null)
                        {
                            remissionGuide.id_shippingType = tempPurchaseOrderDetail.PurchaseOrder.id_shippingType;

                            if (remissionGuide.id_shippingType != null)
                            {
                                LoadpartialComboxTransportTariff(remissionGuide.id_shippingType, remissionGuide.id_TransportTariffType);
                                LoadpartialComboxTransportTariffVehicleType(remissionGuide.id_TransportTariffType, null);
                            }
                        }

                        if (remissionGuide.id_providerRemisionGuide == null)
                        {
                            remissionGuide.id_providerRemisionGuide = tempPurchaseOrderDetail.PurchaseOrder.id_provider;
                            TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, remissionGuide.id_providerRemisionGuide);
                            TempData.Keep("ProductionUnitProviderByProvider");
                        }

                        if (remissionGuide.id_protectiveProvider == null)
                        {
                            remissionGuide.id_protectiveProvider = tempPurchaseOrderDetail.PurchaseOrder.id_providerapparent;
                        }

                        if (remissionGuide.id_RemisionGuideReassignment != null && remissionGuide.id_RemisionGuideReassignment > 0)
                        {
                            int id_RemisionGuideReassignment = int.Parse(remissionGuide.id_RemisionGuideReassignment.ToString());
                            remissionGuide.RemissionGuide2 = db.RemissionGuide.Where(x => x.id == id_RemisionGuideReassignment).FirstOrDefault();
                        }

                        if (remissionGuide.id_buyer == null)
                        {
                            remissionGuide.id_buyer = tempPurchaseOrderDetail.PurchaseOrder.id_buyer;
                        }
                        //Unidad de Producción
                        if (remissionGuide.id_productionUnitProvider == null)
                        {
                            remissionGuide.id_productionUnitProvider = tempPurchaseOrderDetail.PurchaseOrder.id_productionUnitProvider;
                        }
                        //Lista de Precios
                        if (remissionGuide.id_priceList == null)
                        {
                            remissionGuide.id_priceList = tempPurchaseOrderDetail.PurchaseOrder.id_priceList;
                        }

                        //Certificado
                        if (remissionGuide.id_certification == null)
                        {
                            remissionGuide.id_certification = tempPurchaseOrderDetail.PurchaseOrder.id_certification;
                        }

                        if (tempPurchaseOrderDetail != null
                            && tempPurchaseOrderDetail.PurchaseOrder != null
                            && tempPurchaseOrderDetail.PurchaseOrder.Document != null
                            && tempPurchaseOrderDetail.PurchaseOrder.Document.description != ""
                            && tempPurchaseOrderDetail.PurchaseOrder.Document.description != null)

                        {
                            remissionGuide.descriptionpurchaseorder += (" Desc. OC" + tempPurchaseOrderDetail.PurchaseOrder.Document.number + ": " + tempPurchaseOrderDetail.PurchaseOrder.Document.description);
                        }

                        if (remissionGuide.arrivalDate == null)
                        {
                            remissionGuide.arrivalDate = tempPurchaseOrderDetail.PurchaseOrder.deliveryDate;
                        }

                        if (tempPurchaseOrderDetail.PurchaseOrder.deliveryDate != null)
                        {
                            remissionGuide.despachureDate = tempPurchaseOrderDetail.PurchaseOrder.deliveryDate;
                        }

                        if (tempPurchaseOrderDetail.PurchaseOrder.deliveryDate != null)
                        {
                            remissionGuide.returnDate = tempPurchaseOrderDetail.PurchaseOrder.deliveryDate;
                        }

                        if (tempPurchaseOrderDetail.PurchaseOrder.deliveryhour != null)
                        {
                            remissionGuide.despachurehour = tempPurchaseOrderDetail.PurchaseOrder.deliveryhour;
                        }

                        if (remissionGuide.route == "" || remissionGuide.route == null) remissionGuide.route = tempPurchaseOrderDetail.PurchaseOrder.ProductionUnitProvider?.address;

                        remissionGuide.RemissionGuideDetail.Add(remissionGuideDetail);
                    }
                    #endregion
                }
            }
            else
            {
                TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, remissionGuide.id_providerRemisionGuide);
                TempData.Keep("ProductionUnitProviderByProvider");

                TempData["ProductionUnitProviderPoolByUnitProvider"] = DataProviders.DataProviderProductionUnitProviderPool.ProductionUnitProviderPoolByUnitProvider(null, remissionGuide.id_productionUnitProvider);
                TempData.Keep("ProductionUnitProviderPoolByUnitProvider");

                LoadpartialComboxTransportTariff(remissionGuide.id_shippingType, remissionGuide.id_TransportTariffType);
                LoadpartialComboxTransportTariffVehicleType(remissionGuide.id_TransportTariffType, remissionGuide?.RemissionGuideTransportation?.id_vehicle);
            }

            return PartialView("RGReassignment/_FormEditRemissionGuide", remissionGuide);
        }

        private void UpdateRemissionGuideDetailDispatchMaterialDetail(RemissionGuide remissionGuide, RemissionGuideDetail remissionGuideDetail)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            for (int i = remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail.Count - 1; i >= 0; i--)
            {
                var detail = remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail.ElementAt(i);

                detail.RemissionGuideDispatchMaterial.quantityRequiredForPurchase -= detail.quantity;
                detail.RemissionGuideDispatchMaterial.manual = detail.RemissionGuideDispatchMaterial.quantityRequiredForPurchase == 0;
                detail.RemissionGuideDispatchMaterial.sourceExitQuantity -= detail.quantity;

                remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail.Remove(detail);
                try
                {
                    db.RemissionGuideDetailDispatchMaterialDetail.Attach(detail);
                    db.Entry(detail).State = EntityState.Deleted;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (!remissionGuideDetail.isActive) return;

            if (remissionGuideDetail.Item == null)
            {
                remissionGuideDetail.Item = db.Item.FirstOrDefault(fod => fod.id == remissionGuideDetail.id_item);
            }

            var itemIngredientMDD = remissionGuideDetail.Item.ItemIngredient.Where(w => w.Item1.InventoryLine.code.Equals("MI") && (w.Item1.ItemType.code.Equals("MDD")));
            if (itemIngredientMDD.Count() == 0) return;
            var id_metricUnitPurchase = remissionGuideDetail.Item.ItemPurchaseInformation?.id_metricUnitPurchase;
            var id_metricUnitItemHeadIngredient = remissionGuideDetail.Item.ItemHeadIngredient?.id_metricUnit;
            var factorConversionPurchaseFormulation = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                             muc.id_metricOrigin == id_metricUnitPurchase &&
                                                                                             muc.id_metricDestiny == id_metricUnitItemHeadIngredient);
            if (id_metricUnitPurchase != null && id_metricUnitPurchase == id_metricUnitItemHeadIngredient)
            {
                factorConversionPurchaseFormulation = new MetricUnitConversion() { factor = 1 };
            }

            if (factorConversionPurchaseFormulation == null)
            {
                throw new Exception("Falta el Factor de Conversión entre : " + (remissionGuideDetail.Item.ItemPurchaseInformation?.MetricUnit?.code ?? "(UM No Existe)") + ", del Ítem: " + remissionGuideDetail.Item.name + " y " + (remissionGuideDetail.Item.ItemHeadIngredient?.MetricUnit?.code ?? "(UM No Existe)") + " configurado en la cabecera de la formulación del este Ítem. Necesario para cargar los Materiales de Despacho Configúrelo, e intente de nuevo");
            }

            foreach (var iimdd in itemIngredientMDD)
            {
                var quantityMetricUnitItemHeadIngredient = remissionGuideDetail.quantityProgrammed * factorConversionPurchaseFormulation.factor;
                var amountItemHeadIngredient = (remissionGuideDetail.Item.ItemHeadIngredient?.amount ?? 0);
                if (amountItemHeadIngredient == 0)
                {
                    throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + remissionGuideDetail.Item.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
                }
                var quantityItemIngredientMDD = (quantityMetricUnitItemHeadIngredient * (iimdd.amount ?? 0)) / amountItemHeadIngredient;
                //"ENTE01" Codigo de Entero de Tipo de Datos en la unidad de medida
                var truncateQuantityItemIngredientMDD = decimal.Truncate(quantityItemIngredientMDD);
                if ((quantityItemIngredientMDD - truncateQuantityItemIngredientMDD) > 0)
                {
                    quantityItemIngredientMDD = truncateQuantityItemIngredientMDD + 1;
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
                    throw new Exception("Falta el Factor de Conversión entre : " + iimdd.MetricUnit?.code ?? "(UM No Existe)" + ", del Ítem: " + iimdd.Item1.name + " y " + iimdd.Item1.ItemInventory?.MetricUnit.code ?? "(UM No Existe)" + " configurado en el detalle de la formulación del Ítem: " + remissionGuideDetail.Item.name + ". Necesario para cargar los Materiales de Despacho Configúrelo, e intente de nuevo");
                }

                var quantityUMInventory = quantityItemIngredientMDD * factorConversionFormulationInventory.factor;

                var truncateQuantityUMInventory = decimal.Truncate(quantityUMInventory);
                if ((quantityUMInventory - truncateQuantityUMInventory) > 0)
                {
                    quantityUMInventory = truncateQuantityUMInventory + 1;
                };

                RemissionGuideDispatchMaterial remissionGuideDispatchMaterial = remissionGuide.RemissionGuideDispatchMaterial.Where(w => w.isActive).FirstOrDefault(fod => fod.id_item == iimdd.id_ingredientItem);
                if (remissionGuideDispatchMaterial != null)
                {
                    remissionGuideDispatchMaterial.quantityRequiredForPurchase += quantityUMInventory;
                    remissionGuideDispatchMaterial.sourceExitQuantity += quantityUMInventory;
                    remissionGuideDispatchMaterial.manual = false;
                    remissionGuideDispatchMaterial.id_userUpdate = ActiveUser.id;
                    remissionGuideDispatchMaterial.dateUpdate = DateTime.Now;
                }
                else
                {
                    remissionGuideDispatchMaterial = new RemissionGuideDispatchMaterial
                    {
                        id = remissionGuide.RemissionGuideDispatchMaterial.Count() > 0 ? remissionGuide.RemissionGuideDispatchMaterial.Max(pld => pld.id) + 1 : 1,
                        id_item = iimdd.id_ingredientItem,
                        Item = db.Item.FirstOrDefault(i => i.id == iimdd.id_ingredientItem),
                        quantityRequiredForPurchase = quantityUMInventory,
                        sourceExitQuantity = quantityUMInventory,
                        sendedDestinationQuantity = 0,
                        arrivalDestinationQuantity = 0,
                        arrivalGoodCondition = 0,
                        arrivalBadCondition = 0,
                        manual = false,
                        isActive = true,
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now,
                        RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>()
                    };
                    remissionGuide.RemissionGuideDispatchMaterial.Add(remissionGuideDispatchMaterial);
                }

                var remissionGuideDetailDispatchMaterialDetail = new RemissionGuideDetailDispatchMaterialDetail
                {
                    RemissionGuideDetail = remissionGuideDetail,
                    id_remissionGuideDetail = remissionGuideDetail.id,
                    RemissionGuideDispatchMaterial = remissionGuideDispatchMaterial,
                    id_remissionGuideDispatchMaterial = remissionGuideDispatchMaterial.id,
                    quantity = quantityUMInventory
                };
                remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail.Add(remissionGuideDetailDispatchMaterialDetail);
                remissionGuideDispatchMaterial.RemissionGuideDetailDispatchMaterialDetail.Add(remissionGuideDetailDispatchMaterialDetail);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideCopy(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(o => o.id == id);

            RemissionGuide remissionGuideCopy = null;
            if (remissionGuide != null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == remissionGuide.Document.id_documentType);
                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code.Equals("01"));

                remissionGuideCopy = new RemissionGuide
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
                    despachureDate = remissionGuide.despachureDate,
                    arrivalDate = remissionGuide.arrivalDate,
                    returnDate = remissionGuide.returnDate,
                    id_reason = remissionGuide.id_reason,
                    id_reciver = remissionGuide.id_reciver,
                    isExport = false,
                    route = remissionGuide.route,
                    startAdress = remissionGuide.startAdress,
                    descriptionpurchaseorder = remissionGuide.descriptionpurchaseorder,
                    despachurehour = remissionGuide.despachurehour,
                    id_buyer = remissionGuide.id_buyer,
                    id_priceList = remissionGuide.id_priceList,
                    id_certification = remissionGuide.id_certification,
                    id_productionUnitProvider = remissionGuide.id_productionUnitProvider,
                    ProductionUnitProvider = db.ProductionUnitProvider.Where(x => x.id == remissionGuide.id_productionUnitProvider).FirstOrDefault(),
                    id_productionUnitProviderPool = remissionGuide.id_productionUnitProviderPool,
                    ProductionUnitProviderPool = db.ProductionUnitProviderPool.Where(x => x.id == remissionGuide.id_productionUnitProviderPool).FirstOrDefault(),
                    id_protectiveProvider = remissionGuide.id_protectiveProvider,
                    id_providerRemisionGuide = remissionGuide.id_providerRemisionGuide,
                    id_shippingType = remissionGuide.id_shippingType,
                    id_TransportTariffType = remissionGuide.id_TransportTariffType,
                    isInternal = false,
                    uniqueCustomDocument = remissionGuide.uniqueCustomDocument,
                };

                if (remissionGuide.id_providerRemisionGuide != null)
                {
                    TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, remissionGuide.id_providerRemisionGuide);
                    TempData.Keep("ProductionUnitProviderByProvider");

                    TempData["ProductionUnitProviderPoolByUnitProvider"] = DataProviders.DataProviderProductionUnitProviderPool.ProductionUnitProviderPoolByUnitProvider(null, remissionGuide.id_productionUnitProvider);
                    TempData.Keep("ProductionUnitProviderPoolByUnitProvider");
                }

                if (remissionGuide.id_shippingType != null)
                {
                    LoadpartialComboxTransportTariff(remissionGuide.id_shippingType, remissionGuide.id_TransportTariffType);
                    LoadpartialComboxTransportTariffVehicleType(remissionGuide.id_TransportTariffType, remissionGuide?.RemissionGuideTransportation?.id_vehicle);
                }

                #region REMISSION GUIDE TRANSPORTATION

                if (remissionGuide.RemissionGuideTransportation != null)
                {
                    remissionGuideCopy.RemissionGuideTransportation = new RemissionGuideTransportation
                    {
                        carRegistration = remissionGuide.RemissionGuideTransportation.carRegistration,
                        driverName = remissionGuide.RemissionGuideTransportation.driverName,
                        id_driver = remissionGuide.RemissionGuideTransportation.id_driver,
                        id_provider = remissionGuide.RemissionGuideTransportation.id_provider,
                        id_vehicle = remissionGuide.RemissionGuideTransportation.id_vehicle,
                        Vehicle = db.Vehicle.Where(x => x.id == remissionGuide.RemissionGuideTransportation.id_vehicle).FirstOrDefault(),
                        isOwn = remissionGuide.RemissionGuideTransportation.isOwn,
                        advancePrice = remissionGuide.RemissionGuideTransportation.advancePrice,
                        valuePrice = remissionGuide.RemissionGuideTransportation.valuePrice,
                        id_DriverVeicleProviderTransport = remissionGuide.RemissionGuideTransportation.id_DriverVeicleProviderTransport,
                    };
                }

                #endregion

                #region Remission Guide Export Information

                if (remissionGuide.isExport)
                {
                    remissionGuideCopy.RemissionGuideExportInformation = new RemissionGuideExportInformation
                    {
                        uniqueCustomsDocument = remissionGuide.RemissionGuideExportInformation.uniqueCustomsDocument
                    };
                }

                #endregion

                #region REMISSION GUIDE DETAILS

                if (remissionGuide.RemissionGuideDetail != null)
                {
                    remissionGuideCopy.RemissionGuideDetail = new List<RemissionGuideDetail>();

                    foreach (var detail in remissionGuide.RemissionGuideDetail)
                    {
                        var remissionGuideDetail = new RemissionGuideDetail
                        {
                            id_item = detail.id_item,
                            Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),
                            quantityOrdered = detail.quantityOrdered,
                            quantityProgrammed = detail.quantityProgrammed,
                            quantityDispatchPending = detail.quantityDispatchPending,
                            quantityReceived = detail.quantityReceived,
                            isActive = detail.isActive,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            id_Grammage = detail.id_Grammage
                        };

                        remissionGuideCopy.RemissionGuideDetail.Add(remissionGuideDetail);
                    }
                }

                #endregion

                #region REMISSION GUIDE DISPATCH MATERIALS

                if (remissionGuide.RemissionGuideDispatchMaterial != null)
                {
                    remissionGuideCopy.RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>();

                    foreach (var detail in remissionGuide.RemissionGuideDispatchMaterial)
                    {
                        var remissionGuideDispatchMaterial = new RemissionGuideDispatchMaterial
                        {
                            id_item = detail.id_item,
                            Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                            sourceExitQuantity = detail.sourceExitQuantity,
                            sendedDestinationQuantity = detail.sendedDestinationQuantity,
                            arrivalDestinationQuantity = detail.arrivalDestinationQuantity,
                            arrivalGoodCondition = detail.arrivalGoodCondition,
                            arrivalBadCondition = detail.arrivalBadCondition,

                            isActive = detail.isActive,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now
                        };

                        remissionGuideCopy.RemissionGuideDispatchMaterial.Add(remissionGuideDispatchMaterial);
                    }
                }

                #endregion

                #region REMISSION GUIDE SECURITY SEALS

                #endregion

                #region REMISSION GUIDE ASSIGNED STAFF

                if (remissionGuide.RemissionGuideAssignedStaff != null)
                {
                    remissionGuideCopy.RemissionGuideAssignedStaff = new List<RemissionGuideAssignedStaff>();

                    foreach (var detail in remissionGuide.RemissionGuideAssignedStaff)
                    {
                        var remissionGuideAssignedStaff = new RemissionGuideAssignedStaff
                        {
                            id_person = detail.id_person,
                            Person = db.Person.FirstOrDefault(p => p.id == detail.id_person),

                            id_assignedStaffRol = detail.id_assignedStaffRol,
                            RemissionGuideAssignedStaffRol = db.RemissionGuideAssignedStaffRol.FirstOrDefault(r => r.id == detail.id_assignedStaffRol),

                            id_travelType = detail.id_travelType,
                            RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType),

                            isActive = detail.isActive,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            viaticPrice = detail.viaticPrice
                        };

                        remissionGuideCopy.RemissionGuideAssignedStaff.Add(remissionGuideAssignedStaff);
                    }
                }

                #endregion
            }

            TempData["remissionGuide"] = remissionGuideCopy;
            TempData.Keep("remissionGuide");

            return PartialView("_FormEditRemissionGuide", remissionGuideCopy);
        }

        #endregion

        #region REASIGNACION

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideResignacion(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(o => o.id == id);

            RemissionGuide remissionGuideResignacion = null;
            RemissionGuide AuxRemissionGuide = null;
            if (remissionGuide != null)
            {
                if (remissionGuide.Document.DocumentState.code != "03" && remissionGuide.Document.DocumentState.code != "06")
                {
                    TempData.Keep("remissionGuide");

                    ViewData["EditMessage"] = ErrorMessage("La Guia de Remision debe estar Aprobada...");

                    return PartialView("_FormEditRemissionGuide", remissionGuide);
                }
                else
                {
                    if (remissionGuide.id_RemisionGuideReassignment != null && remissionGuide.id_RemisionGuideReassignment > 0)
                    {
                        AuxRemissionGuide = db.RemissionGuide.Where(x => x.id == remissionGuide.id_RemisionGuideReassignment).FirstOrDefault();

                        TempData.Keep("remissionGuide");
                        ViewData["EditMessage"] = ErrorMessage("La Guia de Remision ya tiene una Reasignacion con la Guia " + AuxRemissionGuide.Document.number + " ...");
                        return PartialView("_FormEditRemissionGuide", remissionGuide);
                    }
                    else
                    {
                        AuxRemissionGuide = db.RemissionGuide.Where(x => x.id_RemisionGuideReassignment == remissionGuide.id && x.Document.DocumentState.code != "05").FirstOrDefault();

                        if (AuxRemissionGuide != null)
                        {
                            TempData.Keep("remissionGuide");

                            ViewData["EditMessage"] = ErrorMessage("La Guia de Remision ya esta Reasignada con la Guia " + AuxRemissionGuide.Document.number + " ...");

                            return PartialView("_FormEditRemissionGuide", remissionGuide);
                        }
                    }
                }

                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == remissionGuide.Document.id_documentType);
                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code.Equals("01"));

                remissionGuideResignacion = new RemissionGuide
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
                    despachureDate = remissionGuide.despachureDate,
                    arrivalDate = remissionGuide.arrivalDate,
                    returnDate = remissionGuide.returnDate,
                    id_reason = remissionGuide.id_reason,
                    id_reciver = remissionGuide.id_reciver,
                    isExport = false,
                    route = remissionGuide.route,
                    startAdress = remissionGuide.startAdress,
                    descriptionpurchaseorder = remissionGuide.descriptionpurchaseorder,
                    id_buyer = remissionGuide.id_buyer,
                    id_priceList = remissionGuide.id_priceList,
                    id_certification = remissionGuide.id_certification,
                    id_productionUnitProvider = remissionGuide.id_productionUnitProvider,
                    id_productionUnitProviderPool = remissionGuide.id_productionUnitProviderPool,
                    id_protectiveProvider = remissionGuide.id_protectiveProvider,
                    id_providerRemisionGuide = remissionGuide.id_providerRemisionGuide,
                    isInternal = false,
                    id_RemisionGuideReassignment = remissionGuide.id,
                    RemissionGuide2 = remissionGuide,
                    despachurehour = remissionGuide.despachurehour,
                    ProductionUnitProvider = db.ProductionUnitProvider.Where(x => x.id == remissionGuide.id_productionUnitProvider).FirstOrDefault(),
                    ProductionUnitProviderPool = db.ProductionUnitProviderPool.Where(x => x.id == remissionGuide.id_productionUnitProviderPool).FirstOrDefault(),
                    id_shippingType = remissionGuide.id_shippingType,
                    id_TransportTariffType = remissionGuide.id_TransportTariffType,
                    uniqueCustomDocument = remissionGuide.uniqueCustomDocument
                };

                if (remissionGuide.id_providerRemisionGuide != null)
                {
                    TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, remissionGuide.id_providerRemisionGuide);
                    TempData.Keep("ProductionUnitProviderByProvider");

                    TempData["ProductionUnitProviderPoolByUnitProvider"] = DataProviders.DataProviderProductionUnitProviderPool.ProductionUnitProviderPoolByUnitProvider(null, remissionGuide.id_productionUnitProvider);
                    TempData.Keep("ProductionUnitProviderPoolByUnitProvider");
                }

                if (remissionGuide.id_shippingType != null)
                {
                    LoadpartialComboxTransportTariff(remissionGuide.id_shippingType, remissionGuide.id_TransportTariffType);
                    LoadpartialComboxTransportTariffVehicleType(remissionGuide.id_TransportTariffType, remissionGuide?.RemissionGuideTransportation?.id_vehicle);
                }

                #region REMISSION GUIDE TRANSPORTATION

                if (remissionGuide.RemissionGuideTransportation != null)
                {
                    remissionGuideResignacion.RemissionGuideTransportation = new RemissionGuideTransportation
                    {
                        carRegistration = remissionGuide.RemissionGuideTransportation.carRegistration,
                        driverName = remissionGuide.RemissionGuideTransportation.driverName,
                        id_driver = remissionGuide.RemissionGuideTransportation.id_driver,
                        id_provider = remissionGuide.RemissionGuideTransportation.id_provider,
                        id_vehicle = remissionGuide.RemissionGuideTransportation.id_vehicle,
                        isOwn = remissionGuide.RemissionGuideTransportation.isOwn,
                        advancePrice = remissionGuide.RemissionGuideTransportation.advancePrice,
                        valuePrice = remissionGuide.RemissionGuideTransportation.valuePrice,
                        Vehicle = db.Vehicle.Where(x => x.id == remissionGuide.RemissionGuideTransportation.id_vehicle).FirstOrDefault(),
                        id_DriverVeicleProviderTransport = remissionGuide.RemissionGuideTransportation.id_DriverVeicleProviderTransport,
                    };
                }

                #endregion

                #region Remission Guide Export Information

                if (remissionGuide.isExport)
                {
                    remissionGuideResignacion.RemissionGuideExportInformation = new RemissionGuideExportInformation
                    {
                        uniqueCustomsDocument = remissionGuide.RemissionGuideExportInformation.uniqueCustomsDocument
                    };
                }

                #endregion

                #region REMISSION GUIDE DETAILS

                if (remissionGuide.RemissionGuideDetail != null)
                {
                    remissionGuideResignacion.RemissionGuideDetail = new List<RemissionGuideDetail>();

                    foreach (var detail in remissionGuide.RemissionGuideDetail)
                    {
                        var remissionGuideDetail = new RemissionGuideDetail
                        {
                            id_item = detail.id_item,
                            Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                            quantityOrdered = detail.quantityOrdered,
                            quantityProgrammed = detail.quantityProgrammed,
                            quantityDispatchPending = detail.quantityDispatchPending,
                            quantityReceived = detail.quantityReceived,

                            isActive = detail.isActive,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            id_businessOportunityPlanningDetail = detail.id_businessOportunityPlanningDetail,
                            id_Grammage = detail.id_Grammage
                        };

                        remissionGuideResignacion.RemissionGuideDetail.Add(remissionGuideDetail);
                    }
                }

                #endregion

                #region REMISSION GUIDE DISPATCH MATERIALS

                if (remissionGuide.RemissionGuideDispatchMaterial != null)
                {
                    remissionGuideResignacion.RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>();

                    foreach (var detail in remissionGuide.RemissionGuideDispatchMaterial)
                    {
                        var remissionGuideDispatchMaterial = new RemissionGuideDispatchMaterial
                        {
                            id_item = detail.id_item,
                            Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

                            sourceExitQuantity = detail.sourceExitQuantity,
                            sendedDestinationQuantity = detail.sendedDestinationQuantity,
                            arrivalDestinationQuantity = detail.arrivalDestinationQuantity,
                            arrivalGoodCondition = detail.arrivalGoodCondition,
                            arrivalBadCondition = detail.arrivalBadCondition,

                            isActive = detail.isActive,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            manual = detail.manual
                        };

                        remissionGuideResignacion.RemissionGuideDispatchMaterial.Add(remissionGuideDispatchMaterial);
                    }
                }

                #endregion

                #region REMISSION GUIDE SECURITY SEALS

                if (remissionGuide.RemissionGuideSecuritySeal != null)
                {
                    var details = remissionGuide.RemissionGuideSecuritySeal.ToList();

                    remissionGuideResignacion.RemissionGuideSecuritySeal = new List<RemissionGuideSecuritySeal>();

                    foreach (var detail in details)
                    {
                        var remissionGuideSecuritySeal = new RemissionGuideSecuritySeal
                        {
                            number = detail.number,

                            id_travelType = detail.id_travelType,
                            RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType),

                            id_exitState = detail.id_exitState,
                            SecuritySealState = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_exitState),
                            id_arrivalState = detail.id_arrivalState,
                            SecuritySealState1 = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_arrivalState),

                            isActive = detail.isActive,
                            id_userCreate = detail.id_userCreate,
                            dateCreate = detail.dateCreate,
                            id_userUpdate = detail.id_userUpdate,
                            dateUpdate = detail.dateUpdate
                        };

                        remissionGuideResignacion.RemissionGuideSecuritySeal.Add(remissionGuideSecuritySeal);
                    }
                }

                #endregion

                #region REMISSION GUIDE ASSIGNED STAFF

                if (remissionGuide.RemissionGuideAssignedStaff != null)
                {
                    remissionGuideResignacion.RemissionGuideAssignedStaff = new List<RemissionGuideAssignedStaff>();

                    foreach (var detail in remissionGuide.RemissionGuideAssignedStaff)
                    {
                        var remissionGuideAssignedStaff = new RemissionGuideAssignedStaff
                        {
                            id_person = detail.id_person,
                            Person = db.Person.FirstOrDefault(p => p.id == detail.id_person),

                            id_assignedStaffRol = detail.id_assignedStaffRol,
                            RemissionGuideAssignedStaffRol = db.RemissionGuideAssignedStaffRol.FirstOrDefault(r => r.id == detail.id_assignedStaffRol),

                            id_travelType = detail.id_travelType,
                            RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType),

                            isActive = detail.isActive,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            viaticPrice = detail.viaticPrice
                        };

                        remissionGuideResignacion.RemissionGuideAssignedStaff.Add(remissionGuideAssignedStaff);
                    }
                }

                #endregion
            }
            else
            {
                RemissionGuide tempRemissionGuide = (TempData["remissionGuide"] as RemissionGuide);
                tempRemissionGuide = tempRemissionGuide ?? new RemissionGuide();

                TempData.Keep("remissionGuide");

                ViewData["EditMessage"] = ErrorMessage("La Guia de Remision debe estar Aprobada...");

                return PartialView("_FormEditRemissionGuide", tempRemissionGuide);
            }

            TempData["remissionGuide"] = remissionGuideResignacion;
            TempData.Keep("remissionGuide");

            return PartialView("_FormEditRemissionGuide", remissionGuideResignacion);
        }

        #endregion

        #region REMISSION GUIDE DETAILS

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailsPartial()
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();
            remissionGuide.RemissionGuideDetail = remissionGuide.RemissionGuideDetail ?? new List<RemissionGuideDetail>();

            var model = remissionGuide.RemissionGuideDetail.Where(d => d.isActive);

            TempData.Keep("remissionGuide");

            return PartialView("_Details", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailsPartialAddNew(RemissionGuideDetail remissionGuideDetail)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = remissionGuide.RemissionGuideDetail.FirstOrDefault(it => (!it.isActive) && ((it.RemissionGuideDetailPurchaseOrderDetail == null || it.RemissionGuideDetailPurchaseOrderDetail.Count <= 0)) &&
                                                                                               it.id_item == remissionGuideDetail.id_item);
                    if (modelItem != null)
                    {
                        modelItem.isActive = true;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);

                        modelItem.Item = db.Item.FirstOrDefault(i => i.id == remissionGuideDetail.id_item);

                        UpdateRemissionGuideDetailDispatchMaterialDetail(remissionGuide, modelItem);
                    }
                    else
                    {
                        remissionGuideDetail.id = remissionGuide.RemissionGuideDetail.Count() > 0 ? remissionGuide.RemissionGuideDetail.Max(pld => pld.id) + 1 : 1;

                        remissionGuideDetail.isActive = true;
                        remissionGuideDetail.id_userCreate = ActiveUser.id;
                        remissionGuideDetail.dateCreate = DateTime.Now;
                        remissionGuideDetail.id_userUpdate = ActiveUser.id;
                        remissionGuideDetail.dateUpdate = DateTime.Now;

                        remissionGuideDetail.Item = db.Item.FirstOrDefault(i => i.id == remissionGuideDetail.id_item);
                        remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail = new List<RemissionGuideDetailPurchaseOrderDetail>();
                        remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail = new List<RemissionGuideDetailDispatchMaterialDetail>();

                        remissionGuide.RemissionGuideDetail.Add(remissionGuideDetail);

                        UpdateRemissionGuideDetailDispatchMaterialDetail(remissionGuide, remissionGuideDetail);
                    }

                    TempData["remissionGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("remissionGuide");

            var model = remissionGuide?.RemissionGuideDetail.Where(d => d.isActive).ToList() ?? new List<RemissionGuideDetail>();
            return PartialView("_Details", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailsPartialUpdate(RemissionGuideDetail remissionGuideDetail)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = remissionGuide.RemissionGuideDetail.FirstOrDefault(it => it.id == remissionGuideDetail.id);
                    if (modelItem != null)
                    {
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.Item = db.Item.FirstOrDefault(i => i.id == remissionGuideDetail.id_item);

                        this.UpdateModel(modelItem);

                        modelItem.Item = db.Item.FirstOrDefault(i => i.id == remissionGuideDetail.id_item);

                        UpdateRemissionGuideDetailDispatchMaterialDetail(remissionGuide, modelItem);

                        TempData["remissionGuide"] = remissionGuide;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("remissionGuide");

            var model = remissionGuide?.RemissionGuideDetail.Where(d => d.isActive).ToList() ?? new List<RemissionGuideDetail>();

            return PartialView("_Details", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDetailsPartialDelete(System.Int32 id)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            try
            {
                var remissionDetail = remissionGuide.RemissionGuideDetail.FirstOrDefault(p => p.id == id);
                if (remissionDetail != null)
                {
                    remissionDetail.isActive = false;
                    remissionDetail.id_userUpdate = ActiveUser.id;
                    remissionDetail.dateUpdate = DateTime.Now;

                    UpdateRemissionGuideDetailDispatchMaterialDetail(remissionGuide, remissionDetail);
                }

                TempData["remissionGuide"] = remissionGuide;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            TempData.Keep("remissionGuide");

            var model = remissionGuide?.RemissionGuideDetail.Where(d => d.isActive).ToList() ?? new List<RemissionGuideDetail>();
            return PartialView("_Details", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedRemissionGuideDetails(int[] ids)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ids != null)
            {
                try
                {
                    var remissionGuideDetails = remissionGuide.RemissionGuideDetail.Where(i => ids.Contains(i.id_item));

                    foreach (var detail in remissionGuideDetails)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionGuide");
        }

        #endregion

        #region REMISSION GUIDE DISPATCH MATERIALS

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideDispatchMaterialsPartial()
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();
            remissionGuide.RemissionGuideDispatchMaterial = remissionGuide.RemissionGuideDispatchMaterial ?? new List<RemissionGuideDispatchMaterial>();

            var model = remissionGuide.RemissionGuideDispatchMaterial.Where(d => d.isActive);

            TempData.Keep("remissionGuide");

            return PartialView("_DispatchMaterials", model.ToList());
        }

        public ActionResult RemissionGuideDispatchMaterialsBatchUpdate(MVCxGridViewBatchUpdateValues<RemissionGuideDispatchMaterial, int> updateValues)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            List<RemissionGuideDispatchMaterial> _rgdm = new List<RemissionGuideDispatchMaterial>();
            _rgdm = remissionGuide.RemissionGuideDispatchMaterial.ToList();

            foreach (var _dpTmp in updateValues.Update)
            {
                RemissionGuideDispatchMaterial rgdmTmp = _rgdm.FirstOrDefault(fod => fod.id_item == _dpTmp.id_item);

                rgdmTmp.sourceExitQuantity = _dpTmp.sourceExitQuantity;
                rgdmTmp.isActive = true;
                rgdmTmp.sendedDestinationQuantity = _dpTmp.sourceExitQuantity;
                rgdmTmp.id_userUpdate = _dpTmp.id_userUpdate;
                rgdmTmp.dateUpdate = DateTime.Now;
            }
            foreach (var _dpTmp in updateValues.Insert)
            {
                RemissionGuideDispatchMaterial rgdmTmp = new RemissionGuideDispatchMaterial();
                rgdmTmp.id_item = _dpTmp.id_item;
                rgdmTmp.Item = db.Item.FirstOrDefault(fod => fod.id == _dpTmp.id_item);
                rgdmTmp.sourceExitQuantity = _dpTmp.sourceExitQuantity;
                rgdmTmp.sendedDestinationQuantity = _dpTmp.sendedDestinationQuantity;
                rgdmTmp.isActive = true;
                rgdmTmp.id_userCreate = ActiveUser.id;
                rgdmTmp.dateCreate = DateTime.Now;
                rgdmTmp.id_userUpdate = ActiveUser.id;
                rgdmTmp.dateUpdate = DateTime.Now;
                _rgdm.Add(rgdmTmp);
            }
            decimal _sourceExitQuantity = 0;
            foreach (var det in _rgdm)
            {
                var iti = db.ItemIngredient.FirstOrDefault(fod => fod.id_compoundItem == det.id_item);
                if (iti != null)
                {
                    RemissionGuideDispatchMaterial rgdmTmp = _rgdm.FirstOrDefault(fod => fod.id_item == iti.id_ingredientItem);
                    _sourceExitQuantity = _sourceExitQuantity + det.sourceExitQuantity;
                    rgdmTmp.sourceExitQuantity = _sourceExitQuantity;
                }
            }

            remissionGuide.RemissionGuideDispatchMaterial = _rgdm;
            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");
            return PartialView("_DispatchMaterials", _rgdm);
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedRemissionGuideDispatchMaterials(int[] ids)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ids != null)
            {
                try
                {
                    var remissionGuideDispatchMaterials = remissionGuide.RemissionGuideDispatchMaterial.Where(i => ids.Contains(i.id_item));

                    foreach (var detail in remissionGuideDispatchMaterials)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionGuide");
        }

        #endregion

        #region REMISSION GUIDE SECURITY SEALS

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideSecuritySealsPartial()
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();
            remissionGuide.RemissionGuideSecuritySeal = remissionGuide.RemissionGuideSecuritySeal ?? new List<RemissionGuideSecuritySeal>();

            var model = remissionGuide.RemissionGuideSecuritySeal.Where(d => d.isActive);

            TempData.Keep("remissionGuide");

            return PartialView("_SecuritySeals", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideSecuritySealsPartialAddNew(RemissionGuideSecuritySeal remissionGuideSecuritySeal)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ModelState.IsValid)
            {
                try
                {
                    remissionGuideSecuritySeal.id = remissionGuide.RemissionGuideSecuritySeal.Count() > 0 ? remissionGuide.RemissionGuideSecuritySeal.Max(pld => pld.id) + 1 : 1;

                    remissionGuideSecuritySeal.isActive = true;
                    remissionGuideSecuritySeal.id_userCreate = ActiveUser.id;
                    remissionGuideSecuritySeal.dateCreate = DateTime.Now;
                    remissionGuideSecuritySeal.id_userUpdate = ActiveUser.id;
                    remissionGuideSecuritySeal.dateUpdate = DateTime.Now;

                    remissionGuide.RemissionGuideSecuritySeal.Add(remissionGuideSecuritySeal);

                    TempData["remissionGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("remissionGuide");

            var model = remissionGuide?.RemissionGuideSecuritySeal.Where(d => d.isActive).ToList() ?? new List<RemissionGuideSecuritySeal>();
            return PartialView("_SecuritySeals", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideSecuritySealsPartialUpdate(RemissionGuideSecuritySeal remissionGuideSecuritySeal)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = remissionGuide.RemissionGuideSecuritySeal.FirstOrDefault(it => it.id == remissionGuideSecuritySeal.id);
                    if (modelItem != null)
                    {
                        modelItem.number = remissionGuideSecuritySeal.number;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        TempData["remissionGuide"] = remissionGuide;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("remissionGuide");

            var model = remissionGuide?.RemissionGuideSecuritySeal.Where(d => d.isActive).ToList() ?? new List<RemissionGuideSecuritySeal>();

            return PartialView("_SecuritySeals", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideSecuritySealsPartialDelete(string number)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (!string.IsNullOrEmpty(number))
            {
                try
                {
                    var remissionSecuritySeal = remissionGuide.RemissionGuideSecuritySeal.FirstOrDefault(p => p.number.Equals(number));
                    if (remissionSecuritySeal != null)
                    {
                        remissionSecuritySeal.isActive = false;
                        remissionSecuritySeal.id_userUpdate = ActiveUser.id;
                        remissionSecuritySeal.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionGuide");

            var model = remissionGuide?.RemissionGuideSecuritySeal.Where(d => d.isActive).ToList() ?? new List<RemissionGuideSecuritySeal>();
            return PartialView("_SecuritySeals", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedRemissionGuideSecuritySeals(string[] ids)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ids != null)
            {
                try
                {
                    var remissionGuideSecuritySeals = remissionGuide.RemissionGuideSecuritySeal.Where(i => ids.Contains(i.number));

                    foreach (var detail in remissionGuideSecuritySeals)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionGuide");
        }

        #endregion

        #region REMISSION GUIDE ASSIGNED STAFF

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideAssignedStaffPartial()
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();
            remissionGuide.RemissionGuideAssignedStaff = remissionGuide.RemissionGuideAssignedStaff ?? new List<RemissionGuideAssignedStaff>();

            var model = remissionGuide.RemissionGuideAssignedStaff.Where(d => d.isActive);

            TempData.Keep("remissionGuide");

            return PartialView("_AssignedStaff", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideAssignedStaffPartialAddNew(RemissionGuideAssignedStaff remissionGuideAssignedStaff)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ModelState.IsValid)
            {
                try
                {
                    remissionGuideAssignedStaff.id = remissionGuide.RemissionGuideAssignedStaff.Count() > 0 ? remissionGuide.RemissionGuideAssignedStaff.Max(pld => pld.id) + 1 : 1;
                    remissionGuideAssignedStaff.Person = db.Person.FirstOrDefault(p => p.id == remissionGuideAssignedStaff.id_person);
                    remissionGuideAssignedStaff.RemissionGuideAssignedStaffRol = db.RemissionGuideAssignedStaffRol.FirstOrDefault(r => r.id == remissionGuideAssignedStaff.id_assignedStaffRol);
                    remissionGuideAssignedStaff.RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == remissionGuideAssignedStaff.id_travelType);

                    remissionGuideAssignedStaff.isActive = true;
                    remissionGuideAssignedStaff.id_userCreate = ActiveUser.id;
                    remissionGuideAssignedStaff.dateCreate = DateTime.Now;
                    remissionGuideAssignedStaff.id_userUpdate = ActiveUser.id;
                    remissionGuideAssignedStaff.dateUpdate = DateTime.Now;

                    remissionGuide.RemissionGuideAssignedStaff.Add(remissionGuideAssignedStaff);

                    TempData["remissionGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("remissionGuide");

            var model = remissionGuide?.RemissionGuideAssignedStaff.Where(d => d.isActive).ToList() ?? new List<RemissionGuideAssignedStaff>();
            return PartialView("_AssignedStaff", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideAssignedStaffPartialUpdate(RemissionGuideAssignedStaff remissionGuideAssignedStaff)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = remissionGuide.RemissionGuideAssignedStaff.FirstOrDefault(r => r.id == remissionGuideAssignedStaff.id);
                    if (modelItem != null)
                    {
                        modelItem.id_person = remissionGuideAssignedStaff.id_person;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        TempData["remissionGuide"] = remissionGuide;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("remissionGuide");

            var model = remissionGuide?.RemissionGuideAssignedStaff.Where(d => d.isActive).ToList() ?? new List<RemissionGuideAssignedStaff>();

            return PartialView("_AssignedStaff", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideAssignedStaffPartialDelete(System.Int32 id_person)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (id_person >= 0)
            {
                try
                {
                    var remissionGuideAssignedStaff = remissionGuide.RemissionGuideAssignedStaff.FirstOrDefault(p => p.id_person == id_person);
                    if (remissionGuideAssignedStaff != null)
                    {
                        remissionGuideAssignedStaff.isActive = false;
                        remissionGuideAssignedStaff.id_userUpdate = ActiveUser.id;
                        remissionGuideAssignedStaff.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionGuide");

            var model = remissionGuide?.RemissionGuideAssignedStaff.Where(d => d.isActive).ToList() ?? new List<RemissionGuideAssignedStaff>();
            return PartialView("_AssignedStaff", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedRemissionGuideAssignedStaff(int[] ids)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (ids != null)
            {
                try
                {
                    var remissionGuideAssignedStaff = remissionGuide.RemissionGuideAssignedStaff.Where(i => ids.Contains(i.id_person));

                    foreach (var detail in remissionGuideAssignedStaff)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionGuide");
        }

        #endregion

        #region SINGLE CHANGE DOCUMENT STATE

        [HttpPost]
        public ActionResult RevertLV(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
            RemissionGuide rgTmp = (RemissionGuide)TempData["remissionGuide"];
            int _answer = 0;
            int _answerCloseDoc = 0;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            var lstRgd = rgTmp
                            .RemissionGuideDetail
                            .Select(s => s.id).ToList();

            if (rgTmp.isManual == null)
                this.ViewBag.isManual = false;
            else
                this.ViewBag.isManual = rgTmp.isManual;

            if (remissionGuide != null)
            {
                remissionGuide.isManual = rgTmp.isManual;
                remissionGuide.getLiquidationInformation();
            }
            var rgrd = db.RemissionGuideRiverDetail.FirstOrDefault(fod => fod.id_remisionGuide == id)?.RemissionGuideRiver;

            if (rgrd != null)
            {
                if (remissionGuide.Document.DocumentState.code == "03")
                {
                    if (rgrd.Document.DocumentState.code == "03" || rgrd.Document.DocumentState.code == "06")
                    {
                        TempData.Keep("remissionGuide");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido a tener una guía de Remisión Fluvial anexa");
                        return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                    }
                }
                else if (remissionGuide.Document.DocumentState.code == "06")
                {
                    if (rgrd.Document.DocumentState.code == "06")
                    {
                        TempData.Keep("remissionGuide");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido a tener una guía de Remisión Fluvial anexa en Estado Autorizado");
                        return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                    }
                }
            }
            var rgPld = db.ProductionLotDetailPurchaseDetail
                            .FirstOrDefault(fod => lstRgd.Contains((int)fod.id_remissionGuideDetail));
            if (rgPld != null)
            {
                if (rgPld.ProductionLotDetail.ProductionLot.Lot.Document.DocumentState.code != "05")
                {
                    TempData.Keep("remissionGuide");
                    ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido a ser parte de la Recepción " + rgPld.ProductionLotDetail.ProductionLot.internalNumber + ".");
                    return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                }
            }

            var existInInventoryMoveDetailExitDispatchMaterials = remissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.InventoryMoveDetailExitDispatchMaterials.Count() > 0);

            if (existInInventoryMoveDetailExitDispatchMaterials != null)
            {
                var existInInventoryMoveEPTAMDL = existInInventoryMoveDetailExitDispatchMaterials.InventoryMoveDetailExitDispatchMaterials.FirstOrDefault().InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EPTAMDL");
                if (!existInInventoryMoveEPTAMDL)
                {
                    TempData.Keep("remissionGuide");
                    ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido a tener egresos de materiales de despacho en inventario, manual.");
                    return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                }
            }

            int[] depDocumentReqMoveIds = db.DocumentSource
                                                      .Where(r => r.id_documentOrigin == id)
                                                      .Select(r => r.id_document)
                                                      .ToArray();

            var reqInventario = db.Document
                                      .Where(r => depDocumentReqMoveIds.Contains(r.id)
                                                  && r.DocumentType.code == "79"
                                                  && r.DocumentState.code == "03")
                                      .Select(r => r.sequential.ToString())
                                      .ToArray();

            if (reqInventario.Length > 0)
            {
                var depDocumentReqMoveList = reqInventario
                                                    .Aggregate((i, j) => string.Format("#{0}, #{1}", i, j));
                string textoRequerimiento = reqInventario.Length > 1 ? "los Requerimientos" : "el Requerimiento";
                TempData.Keep("remissionGuide");
                ViewData["EditMessage"] = ErrorMessage($"No se puede reversar la Guía de Remisión, debe reversar {textoRequerimiento} de Movimiento de Inventario: {depDocumentReqMoveList}");
                return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
            }

            var cantliquida = (from e in db.LiquidationFreightDetail
                               where e.id_remisionGuide == id && e.LiquidationFreight.Document.DocumentState.code != "05"
                               select e.id).Count();

            if (cantliquida > 0)
            {
                TempData.Keep("remissionGuide");
                ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido que ya posee una Liquidacion de Flete.");
                return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
            }

            if (remissionGuide.hasExitPlanctProduction != null && remissionGuide.hasExitPlanctProduction.Value == true)
            {
                TempData.Keep("remissionGuide");
                ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido que ya salio de La Planta.");
                return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
            }

            if (remissionGuide.Document.DocumentState.code != "06" && remissionGuide.Document.DocumentState.code != "03")
            {
                TempData.Keep("remissionGuide");
                ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido que la Guia no esta AUTORIZADA o APROBADA.");
                return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region << Reverso Requermimiento de inventario >>

                    var estadoDocumentoAnulado = db.DocumentState.FirstOrDefault(r => r.code == "05");
                    var auditActionDocumentoAnulado = db.tbsysActionOnDocument.FirstOrDefault(r => r.code == "AND");
                    int[] documentReqMoveIds = db.DocumentSource
                                                      .Where(r => r.id_documentOrigin == id)
                                                      .Select(r => r.id_document)
                                                      .ToArray();

                    var reqInventarioPendientes = db.Document
                                                        .Where(r => documentReqMoveIds.Contains(r.id)
                                                                    && r.DocumentType.code == "79"
                                                                    && r.DocumentState.code == "01")
                                                        .ToList();
                    foreach (var requerimiento in reqInventarioPendientes)
                    {
                        var documentCancel = db.Document.FirstOrDefault(r => r.id == requerimiento.id);

                        documentCancel.id_documentState = estadoDocumentoAnulado?.id ?? 0;

                        ServiceDocument.SaveTrackState(documentCancel, auditActionDocumentoAnulado.id, ActiveUser.id, db);

                        db.Document.Attach(documentCancel);
                        db.Entry(documentCancel).State = EntityState.Modified;
                    }

                    #endregion

                    #region << Reverso Guia de Remision >>
                    int id_ep = 0;
                    if (TempData["id_ep"] != null)
                    {
                        id_ep = (int)TempData["id_ep"];
                    }
                    id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                    EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_ep);

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");

                    if (remissionGuide != null && documentState != null)
                    {
                        foreach (var detail in remissionGuide.RemissionGuideDetail)
                        {
                            foreach (var remissionGuideDetailPurchaseOrderDetail in detail.RemissionGuideDetailPurchaseOrderDetail)
                            {
                                ServicePurchaseRemission.UpdateQuantityDispatchedPurchaseOrderDetailRemissionGuide(db, remissionGuideDetailPurchaseOrderDetail.id_purchaseOrderDetail,
                                                                               remissionGuideDetailPurchaseOrderDetail.id_remissionGuideDetail,
                                                                               -remissionGuideDetailPurchaseOrderDetail.quantity);
                            }
                        }

                        var inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == remissionGuide.id &&
                                                                                  (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                        InventoryMove lastInventoryMoveEPTAMDL = (inventoryMoveDetailAux.Count > 0)
                                                            ? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
                                                            : null;
                        inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == remissionGuide.id &&
                                                                             (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                        InventoryMove lastInventoryMoveIPTAMDL = (inventoryMoveDetailAux.Count > 0)
                                                            ? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
                                                            : null;

                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;

                        db.RemissionGuide.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        _answer = 1;
                        TempData["remissionGuide"] = remissionGuide;
                        TempData.Keep("remissionGuide");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + remissionGuide.Document.number + " reversada exitosamente");
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuide");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    MetodosEscrituraLogs.EscribeMensajeLog(e.Message, ruta, "LOGISTICAREVERT", "PROD");
                }
            }
            if (_answer == 1)
            {
                #region CERRADO DE DOCUMENTOS
                if (_answer == 1)
                {
                    if (remissionGuide.id > 0)
                    {
                        List<int> iLstPod = db.RemissionGuideDetailPurchaseOrderDetail
                                            .Where(w => w.RemissionGuideDetail.RemissionGuide.id == remissionGuide.id)
                                            .Select(s => s.id_purchaseOrderDetail)
                                            .ToList();

                        if (iLstPod != null && iLstPod.Count > 0)
                        {
                            var _iLstPo = db.PurchaseOrderDetail
                                        .Where(w => iLstPod.Contains(w.id))
                                        .GroupBy(g => g.PurchaseOrder.id)
                                        .Select(s => new
                                        {
                                            id_po = s.FirstOrDefault().PurchaseOrder.id,
                                            qAp = s.Sum(su => su.quantityApproved),
                                            qPe = s.Sum(su => su.quantityDispatched)
                                        }).ToList();

                            if (_iLstPo != null && _iLstPo.Count > 0)
                            {
                                #region Listado de Ordenes de Compra
                                foreach (var _de in _iLstPo)
                                {
                                    if (_de.id_po > 0)
                                    {
                                        if ((_de.qAp - _de.qPe) > 0)
                                        {
                                            Document _doPo = db.Document.FirstOrDefault(fod => fod.id == _de.id_po);

                                            if (_doPo != null)
                                            {
                                                using (DbContextTransaction trans = db.Database.BeginTransaction())
                                                {
                                                    try
                                                    {
                                                        _doPo.isOpen = true;
                                                        db.Document.Attach(_doPo);
                                                        db.Entry(_doPo).State = EntityState.Modified;
                                                        db.SaveChanges();
                                                        trans.Commit();
                                                        _answerCloseDoc = 1;
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                #region AUDITORIA APERTURA DE DOCUMENTO ORDEN DE COMPRA
                                                if (_answerCloseDoc == 1)
                                                {
                                                    DocumentLogDTO _doTmp = new DocumentLogDTO();
                                                    _doTmp.id = _de.id_po;
                                                    _doTmp.code_Action = "ABD";
                                                    _doTmp.id_User = ActiveUser.id;
                                                    _doTmp.description = "EJECUCION MANUAL";
                                                    Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                #endregion
            }

            remissionGuide.RemissionGuideTransportation.id_TransportationType = remissionGuide.id_TransportTariffType;

            return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
        }

        [HttpPost]
        public ActionResult Approve(int id)
        {
            _codeStateDocument = "";
            _messageAnswer = "";
            _btnOnEditFormRG = new ButtonsOnEditFormRemissionGuide();
            _AnswerfaRG = new AnswerForActionRemissionGuide();
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
            RemissionGuide rgTmp = (RemissionGuide)TempData["remissionGuide"];
            int _answerCloseDoc = 0;
            int _answer = 0;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];

            if (remissionGuide != null)
            {
                remissionGuide.isManual = rgTmp.isManual;
                remissionGuide.getLiquidationInformation();
            }
            if (rgTmp.isManual == null)
                this.ViewBag.isManual = false;
            else
                this.ViewBag.isManual = rgTmp.isManual;
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                    if (remissionGuide != null && documentState != null)
                    {
                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;

                        foreach (var details in remissionGuide.RemissionGuideDetail)
                        {
                            db.RemissionGuideDetail.Attach(details);
                            db.Entry(details).State = EntityState.Modified;
                        }

                        db.RemissionGuide.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        _answer = 1;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }
            if (_answer == 1)
            {
                #region CERRADO DE DOCUMENTOS
                if (_answer == 1)
                {
                    if (remissionGuide.id > 0)
                    {
                        List<int> iLstPod = db.RemissionGuideDetailPurchaseOrderDetail
                                            .Where(w => w.RemissionGuideDetail.RemissionGuide.id == remissionGuide.id)
                                            .Select(s => s.id_purchaseOrderDetail)
                                            .ToList();

                        if (iLstPod != null && iLstPod.Count > 0)
                        {
                            var _iLstPo = db.PurchaseOrderDetail
                                        .Where(w => iLstPod.Contains(w.id))
                                        .GroupBy(g => g.PurchaseOrder.id)
                                        .Select(s => new
                                        {
                                            id_po = s.FirstOrDefault().PurchaseOrder.id,
                                            qAp = s.Sum(su => su.quantityApproved),
                                            qPe = s.Sum(su => su.quantityDispatched)
                                        }).ToList();

                            if (_iLstPo != null && _iLstPo.Count > 0)
                            {
                                #region Listado de Ordenes de Compra
                                foreach (var _de in _iLstPo)
                                {
                                    if (_de.id_po > 0)
                                    {
                                        if ((_de.qAp - _de.qPe) <= 0)
                                        {
                                            Document _doPo = db.Document.FirstOrDefault(fod => fod.id == _de.id_po);

                                            if (_doPo != null)
                                            {
                                                using (DbContextTransaction trans = db.Database.BeginTransaction())
                                                {
                                                    try
                                                    {
                                                        _doPo.isOpen = false;
                                                        db.Document.Attach(_doPo);
                                                        db.Entry(_doPo).State = EntityState.Modified;
                                                        db.SaveChanges();
                                                        trans.Commit();
                                                        _answerCloseDoc = 1;
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                #region AUDITORIA CERRADO DE DOCUMENTO ORDEN DE COMPRA
                                                if (_answerCloseDoc == 1)
                                                {
                                                    DocumentLogDTO _doTmp = new DocumentLogDTO();
                                                    _doTmp.id = _de.id_po;
                                                    _doTmp.code_Action = "CRD";
                                                    _doTmp.id_User = ActiveUser.id;
                                                    _doTmp.description = "EJECUCION MANUAL";
                                                    Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                #endregion
            }

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
            RemissionGuide rgTmp = (RemissionGuide)TempData["remissionGuide"];
            XmlDocument xmlFEX = new XmlDocument();

            if (remissionGuide != null)
            {
                remissionGuide.isManual = rgTmp.isManual;
            }

            if (rgTmp.isManual == null)
                this.ViewBag.isManual = false;
            else
                this.ViewBag.isManual = rgTmp.isManual;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "09");

                    if (remissionGuide != null && documentState != null)
                    {
                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;

                        foreach (var details in remissionGuide.RemissionGuideDispatchMaterial)
                        {
                            var sumQuanty = details.InventoryMoveDetailExitDispatchMaterials?.Sum(s => s.quantity) ?? (decimal)0;
                            if (sumQuanty != details.sourceExitQuantity)
                            {
                                TempData.Keep("remissionGuide");
                                ViewData["EditMessage"] = ErrorMessage("No se puede autorizar la guía de remisión debido a no tener hecho el egreso de materiales de despacho en inventario.");
                                return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                            }
                        }

                        #region Regeneramos la clave de acceso

                        var document = remissionGuide.Document;
                        string emissionDate = document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");
                        var id_company = document.EmissionPoint.id_company;
                        Company vrCompa = db.Company.Where(h => h.id == id_company).FirstOrDefault();
                        var enviromentCode = vrCompa.CompanyElectronicFacturation.EnvironmentType.codeSRI.ToString();
                        remissionGuide.Document.accessKey = AccessKey.GenerateAccessKey(emissionDate,
                                document.DocumentType.codeSRI,
                                document.EmissionPoint.BranchOffice.Division.Company.ruc,
                                    enviromentCode,
                                document.EmissionPoint.BranchOffice.code.PadLeft(3, '0') + document.EmissionPoint.code.ToString("D3"),
                                document.sequential.ToString("D9"),
                                document.sequential.ToString("D8"),
                                "1");

                        #endregion

                        xmlFEX = GenerateXML(remissionGuide, (int)ViewData["id_company"]);

                        db.RemissionGuide.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["remissionGuide"] = remissionGuide;
                        TempData.Keep("remissionGuide");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + remissionGuide.Document.number + " autorizada exitosamente");

                        string routePath = ConfigurationManager.AppSettings["rutaXmlFEX"];
                        string routePathA1Firmar = ConfigurationManager.AppSettings["rutaXmlA1Firmar"];

                        try
                        {
                            //User token that represents the authorized user account
                            IntPtr token = IntPtr.Zero;

                            string USER_rutaXmlFEX = ConfigurationManager.AppSettings["USER_rutaXmlFEX"];
                            USER_rutaXmlFEX = string.IsNullOrEmpty(USER_rutaXmlFEX) ? "admin" : USER_rutaXmlFEX;
                            string PASS_rutaXmlFEX = ConfigurationManager.AppSettings["PASS_rutaXmlFEX"];
                            PASS_rutaXmlFEX = string.IsNullOrEmpty(PASS_rutaXmlFEX) ? "admin" : PASS_rutaXmlFEX;
                            string DOMAIN_rutaXmlFEX = ConfigurationManager.AppSettings["DOMAIN_rutaXmlFEX"];
                            DOMAIN_rutaXmlFEX = string.IsNullOrEmpty(DOMAIN_rutaXmlFEX) ? "" : DOMAIN_rutaXmlFEX;

                            bool result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                            if (result == true)
                            {
                                //Use token to setup a WindowsImpersonationContext 
                                using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                {
                                    if (!string.IsNullOrEmpty(routePath))
                                    {
                                        if (!(Directory.Exists(routePath)))
                                        {
                                            Directory.CreateDirectory(routePath);
                                            LogController.WriteLog(routePath + ": Creado Exitosamente");
                                        }
                                        if (Directory.Exists(routePath))
                                        {
                                            string pathFileXmlFileName = routePath + "\\" + remissionGuide.Document.accessKey + ".xml";
                                            xmlFEX.Save(pathFileXmlFileName);
                                            LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");
                                        }
                                    }
                                    else
                                    {
                                        LogController.WriteLog("No existe la ruta de XML de FEX usado en Guía.");
                                    }
                                    ctx.Undo();
                                    CloseHandle(token);
                                }
                            }

                            //User token that represents the authorized user account
                            string USER_rutaXmlA1Firmar = ConfigurationManager.AppSettings["USER_rutaXmlA1Firmar"];
                            USER_rutaXmlA1Firmar = string.IsNullOrEmpty(USER_rutaXmlA1Firmar) ? "admin" : USER_rutaXmlA1Firmar;
                            string PASS_rutaXmlA1Firmar = ConfigurationManager.AppSettings["PASS_rutaXmlA1Firmar"];
                            PASS_rutaXmlA1Firmar = string.IsNullOrEmpty(PASS_rutaXmlA1Firmar) ? "admin" : PASS_rutaXmlA1Firmar;
                            string DOMAIN_rutaXmlA1Firmar = ConfigurationManager.AppSettings["DOMAIN_rutaXmlA1Firmar"];
                            DOMAIN_rutaXmlA1Firmar = string.IsNullOrEmpty(DOMAIN_rutaXmlA1Firmar) ? "" : DOMAIN_rutaXmlA1Firmar;

                            result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                            if (result == true)
                            {
                                //Use token to setup a WindowsImpersonationContext 
                                using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                {
                                    if (!string.IsNullOrEmpty(routePathA1Firmar))
                                    {
                                        if (!(Directory.Exists(routePathA1Firmar)))
                                        {
                                            Directory.CreateDirectory(routePathA1Firmar);
                                            LogController.WriteLog(routePathA1Firmar + ": Creado Exitosamente");
                                        }
                                        if (Directory.Exists(routePathA1Firmar))
                                        {
                                            string pathFileXmlFileName = routePathA1Firmar + "\\" + remissionGuide.Document.accessKey + ".xml";
                                            xmlFEX.Save(pathFileXmlFileName);
                                            LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");

                                            if (!string.IsNullOrEmpty(routePath))
                                            {
                                                if (Directory.Exists(routePath))
                                                {
                                                    string pathFileXmlFileName2 = routePath + "\\" + remissionGuide.Document.accessKey + ".xml";
                                                    System.IO.File.Delete(pathFileXmlFileName2);
                                                    LogController.WriteLog(pathFileXmlFileName2 + ": Eliminado Exitosamente");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LogController.WriteLog("No existe la ruta de A1.Firmar.");
                                    }
                                    ctx.Undo();
                                    CloseHandle(token);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogController.WriteLog(ex.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuide");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
        }

        //Impersonation functionality
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        //Disconnection after file operations
        [DllImport("kernel32.dll")]
        private static extern Boolean CloseHandle(IntPtr hObject);

        [HttpPost]
        public ActionResult CheckAutorizeRSI(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
            RemissionGuide rgTmp = (RemissionGuide)TempData["remissionGuide"];

            if (remissionGuide != null)
            {
                remissionGuide.isManual = rgTmp.isManual;
            }

            if (rgTmp.isManual == null)
                this.ViewBag.isManual = false;
            else
                this.ViewBag.isManual = rgTmp.isManual;
            string msgXtraInfo = "";

            msgXtraInfo = "Obtener Ruta XML Guía De Remisión(B1.AutorizadoActualizado)";
            string routePathB1AutorizadoActualizado = ConfigurationManager.AppSettings["rutaXmlB1AutorizadoActualizado"];
            if (string.IsNullOrEmpty(routePathB1AutorizadoActualizado))
            {
                throw new Exception("No definida Configuración Ruta B1.AutorizadoActualizado.");
            }
            msgXtraInfo = "Guía De Remisión";
            DocumentState documentState09 = db.DocumentState.FirstOrDefault(s => s.code == "09");
            if (remissionGuide == null) throw new Exception("Guía De Remisón" + id + " ,no se ha encontrado.");

            LogController.tratarFicheroLog();
            bool change = false;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {

                    //User token that represents the authorized user account
                    IntPtr token = IntPtr.Zero;
                    string USER_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["USER_rutaXmlB1AutorizadoActualizado"];
                    USER_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(USER_rutaXmlB1AutorizadoActualizado) ? "admin" : USER_rutaXmlB1AutorizadoActualizado;
                    string PASS_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["PASS_rutaXmlB1AutorizadoActualizado"];
                    PASS_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(PASS_rutaXmlB1AutorizadoActualizado) ? "admin" : PASS_rutaXmlB1AutorizadoActualizado;
                    string DOMAIN_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["DOMAIN_rutaXmlB1AutorizadoActualizado"];
                    DOMAIN_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(DOMAIN_rutaXmlB1AutorizadoActualizado) ? "" : DOMAIN_rutaXmlB1AutorizadoActualizado;

                    bool result = LogonUser(USER_rutaXmlB1AutorizadoActualizado, DOMAIN_rutaXmlB1AutorizadoActualizado, PASS_rutaXmlB1AutorizadoActualizado, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                    if (result == true)
                    {
                        //Use token to setup a WindowsImpersonationContext 
                        using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                        {
                            String[] dirs = System.IO.Directory.GetDirectories(routePathB1AutorizadoActualizado);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06");
                            ElectronicDocumentState electronicDocumentState = db.ElectronicDocumentState.FirstOrDefault(s => s.code == "03");
                            var yearMothCurrent = remissionGuide.Document.accessKey.Substring(4, 4).ToString() +
                                                    remissionGuide.Document.accessKey.Substring(2, 2).ToString();
                            foreach (String dir in dirs)
                            {
                                var directoryNameAux = dir.Substring(dir.Length - 8, 6);
                                if (directoryNameAux == yearMothCurrent)
                                {
                                    String[] files = System.IO.Directory.GetFileSystemEntries(dir);
                                    foreach (String file in files)
                                    {
                                        string[] words = Path.GetFileNameWithoutExtension(file).Split('_');
                                        string accessKeyAux = words[0];
                                        var aDocument = remissionGuide.Document;
                                        if (accessKeyAux == aDocument.accessKey)
                                        {
                                            aDocument.dateUpdate = DateTime.Now;
                                            aDocument.id_userUpdate = ActiveUser.id;
                                            aDocument.id_documentState = documentState.id;
                                            aDocument.DocumentState = documentState;
                                            aDocument.authorizationNumber = aDocument.accessKey;

                                            db.Document.Attach(aDocument);
                                            db.Entry(aDocument).State = EntityState.Modified;

                                            var aElectronicDocument = db.ElectronicDocument.FirstOrDefault(fod => fod.Document.accessKey == accessKeyAux);
                                            if (aElectronicDocument != null)
                                            {
                                                aElectronicDocument.id_electronicDocumentState = electronicDocumentState.id;
                                                aElectronicDocument.ElectronicDocumentState = electronicDocumentState;

                                                db.ElectronicDocument.Attach(aElectronicDocument);
                                                db.Entry(aElectronicDocument).State = EntityState.Modified;
                                            }
                                            if (!change) change = true;
                                        }
                                    }
                                };
                            }

                            ctx.Undo();
                            CloseHandle(token);
                        }
                    }

                    if (change)
                    {
                        db.SaveChanges();
                        trans.Commit();
                        TempData["remissionGuide"] = remissionGuide;
                        ViewData["EditMessage"] = SuccessMessage("Guía De Remisión : " + remissionGuide.Document.number + " verificada y actualizada la autorización del SRI");
                        LogController.WriteLog("Actualización de Estado de la Guía De Remisión a Estado Autorizado Satisfactoriamente(Con Cambio)");
                    }
                    else
                    {
                        ViewData["EditMessage"] = WarningMessage("Guía De Remisión : " + remissionGuide.Document.number + " aun no esta autorizada por el SRI");
                        LogController.WriteLog("Actualización de Estado de las Guía De Remisión a Estado Autorizado Satisfactoriamente(Sin Cambio)");
                    }
                }
                catch (Exception e)
                {
                    if (change)
                    {
                        trans.Rollback();
                    }
                    LogController.WriteLog(e.Message);
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    LogWrite(e, null, "CheckAutorizeRSIDocument=>" + msgXtraInfo);
                }
                finally
                {
                    TempData.Keep("remissionGuide");
                }
            }

            return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
        }

        [HttpPost]
        public ActionResult Protect(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
            RemissionGuide rgTmp = (RemissionGuide)TempData["remissionGuide"];

            if (remissionGuide != null)
            {
                remissionGuide.isManual = rgTmp.isManual;
            }

            if (rgTmp.isManual == null)
                this.ViewBag.isManual = false;
            else
                this.ViewBag.isManual = rgTmp.isManual;

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                    if (remissionGuide != null && documentState != null)
                    {
                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;

                        db.RemissionGuide.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditMessage"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
            RemissionGuide rgTmp = (RemissionGuide)TempData["remissionGuide"];
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            int _answer = 0;
            int _answerCloseDoc = 0;

            if (rgTmp.isManual == null)
                this.ViewBag.isManual = false;
            else
                this.ViewBag.isManual = rgTmp.isManual;

            if (remissionGuide != null)
            {
                remissionGuide.isManual = rgTmp.isManual;
                remissionGuide.getLiquidationInformation();
            }
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    int id_ep = 0;
                    if (TempData["id_ep"] != null)
                    {
                        id_ep = (int)TempData["id_ep"];
                    }
                    id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                    EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_ep);

                    var rgrd = db.RemissionGuideRiverDetail.FirstOrDefault(fod => fod.id_remisionGuide == id)?.RemissionGuideRiver;

                    LiquidationFreightDetail lfdTmp = db.LiquidationFreightDetail
                                                        .FirstOrDefault(fod => fod.id_remisionGuide == id
                                                        && (fod.RemissionGuide.Document.DocumentState.code == "01"
                                                        || fod.RemissionGuide.Document.DocumentState.code == "03"));

                    if (lfdTmp != null)
                    {
                        TempData.Keep("remissionGuide");
                        ViewData["EditMessage"] = ErrorMessage("No se puede Cancelar la guía de remisión debido a estar en la Liquidación de Flete N. " + lfdTmp?.LiquidationFreight?.Document?.number);
                        return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                    }

                    if (rgrd != null)
                    {
                        if (remissionGuide.Document.DocumentState.code == "03")
                        {
                            if (rgrd.Document.DocumentState.code == "03" || rgrd.Document.DocumentState.code == "06")
                            {
                                TempData.Keep("remissionGuide");
                                ViewData["EditMessage"] = ErrorMessage("No se puede cancelar la guía de remisión debido a tener una guía de Remisión Fluvial anexa");
                                return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                            }
                        }
                        else if (remissionGuide.Document.DocumentState.code == "06")
                        {
                            if (rgrd.Document.DocumentState.code == "06")
                            {
                                TempData.Keep("remissionGuide");
                                ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido a tener una guía de Remisión Fluvial anexa en Estado Autorizado");
                                return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                            }
                        }
                    }

                    var existInInventoryMoveDetailExitDispatchMaterials = remissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.InventoryMoveDetailExitDispatchMaterials.Count() > 0);

                    if (existInInventoryMoveDetailExitDispatchMaterials != null)
                    {
                        var existInInventoryMoveEMD = existInInventoryMoveDetailExitDispatchMaterials.InventoryMoveDetailExitDispatchMaterials.FirstOrDefault().InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EMD");
                        if (existInInventoryMoveEMD)
                        {
                            TempData.Keep("remissionGuide");
                            ViewData["EditMessage"] = ErrorMessage("No se puede anular la guía de remisión debido a tener egresos de materiales de despacho en inventario, manual.");
                            return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                        }
                    }

                    // Recuperamos los estados de documento a utilizar
                    var documentStatePendiente = db.DocumentState.FirstOrDefault(s => s.code == "01");
                    var documentStateAnulado = db.DocumentState.FirstOrDefault(s => s.code == "05");
                    var requerimientosInventarioDocs = (
                        from ds in db.DocumentSource
                        join rm in db.RequestInventoryMove on ds.id_document equals rm.id
                        join dc in db.Document on rm.id equals dc.id
                        where ds.id_documentOrigin == id && dc.DocumentType.code == "79" && dc.id_documentState != documentStateAnulado.id
                        select dc);

                    // Si existe alguno que NO está pendiente, es un error!
                    if (requerimientosInventarioDocs.Any(rm => rm.id_documentState != documentStatePendiente.id))
                    {
                        TempData.Keep("remissionGuide");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular la guía de remisión debido a que tiene requerimientos de inventario que NO están PENDIENTES.");
                        return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                    }

                    if (remissionGuide != null && documentStateAnulado != null)
                    {
                        if (remissionGuide.Document.DocumentState.code != "01")
                        {
                            foreach (var detail in remissionGuide.RemissionGuideDetail)
                            {
                                foreach (var remissionGuideDetailPurchaseOrderDetail in detail.RemissionGuideDetailPurchaseOrderDetail)
                                {
                                    ServicePurchaseRemission.UpdateQuantityDispatchedPurchaseOrderDetailRemissionGuide(db, remissionGuideDetailPurchaseOrderDetail.id_purchaseOrderDetail,
                                                                                   remissionGuideDetailPurchaseOrderDetail.id_remissionGuideDetail,
                                                                                   -remissionGuideDetailPurchaseOrderDetail.quantity);
                                }
                            }

                            var inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == remissionGuide.id &&
                                                                                  (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                            InventoryMove lastInventoryMoveEPTAMDL = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
                                                                : null;
                            inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == remissionGuide.id &&
                                                                                 (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                            InventoryMove lastInventoryMoveIPTAMDL = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
                                                                : null;

                            ServiceInventoryMove.UpdateInventaryMoveTransferDispatchMaterialsLogistic(ActiveUser, ActiveCompany, emissionPoint, remissionGuide, db, true, lastInventoryMoveEPTAMDL, lastInventoryMoveIPTAMDL);
                            foreach (var item in remissionGuide.RemissionGuideDispatchMaterial.ToList())
                            {
                                if (item.InventoryMoveDetailExitDispatchMaterials != null)
                                {
                                    var quantity = item.InventoryMoveDetailExitDispatchMaterials.Sum(s => s.quantity);
                                    item.sendedDestinationQuantity = quantity;
                                    db.RemissionGuideDispatchMaterial.Attach(item);
                                    db.Entry(item).State = EntityState.Modified;
                                }
                                else
                                {
                                    item.sendedDestinationQuantity = 0;
                                    db.RemissionGuideDispatchMaterial.Attach(item);
                                    db.Entry(item).State = EntityState.Modified;
                                }
                            }
                        }


                        // Anular los requerimientos de inventario pendientes relacionados
                        var idDocumentAction = 0;

                        foreach (var requerimientoInventarioDoc in requerimientosInventarioDocs)
                        {
                            requerimientoInventarioDoc.id_documentState = documentStateAnulado.id;
                            requerimientoInventarioDoc.DocumentState = documentStateAnulado;

                            db.Document.Attach(requerimientoInventarioDoc);
                            db.Entry(requerimientoInventarioDoc).State = EntityState.Modified;

                            if (idDocumentAction <= 0)
                            {
                                idDocumentAction = db.tbsysActionOnDocument
                                    .FirstOrDefault(fod => fod.code.Equals("AND"))
                                    .id;
                            }

                            ServiceDocument.SaveTrackState(requerimientoInventarioDoc, idDocumentAction, this.ActiveUserId, db);
                        }

                        remissionGuide.Document.id_documentState = documentStateAnulado.id;
                        remissionGuide.Document.DocumentState = documentStateAnulado;

                        db.RemissionGuide.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        _answer = 1;

                        TempData["remissionGuide"] = remissionGuide;
                        TempData.Keep("remissionGuide");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + remissionGuide.Document.number + " anulada exitosamente");
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuide");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            if (_answer == 1)
            {
                #region APERTURA DE DOCUMENTOS
                if (_answer == 1)
                {
                    if (remissionGuide.id > 0)
                    {
                        List<int> iLstPod = db.RemissionGuideDetailPurchaseOrderDetail
                                            .Where(w => w.RemissionGuideDetail.RemissionGuide.id == remissionGuide.id)
                                            .Select(s => s.id_purchaseOrderDetail)
                                            .ToList();

                        if (iLstPod != null && iLstPod.Count > 0)
                        {
                            var _iLstPo = db.PurchaseOrderDetail
                                        .Where(w => iLstPod.Contains(w.id))
                                        .GroupBy(g => g.PurchaseOrder.id)
                                        .Select(s => new
                                        {
                                            id_po = s.FirstOrDefault().PurchaseOrder.id,
                                            qAp = s.Sum(su => su.quantityApproved),
                                            qPe = s.Sum(su => su.quantityDispatched)
                                        }).ToList();

                            if (_iLstPo != null && _iLstPo.Count > 0)
                            {
                                #region Listado de Ordenes de Compra
                                foreach (var _de in _iLstPo)
                                {
                                    if (_de.id_po > 0)
                                    {
                                        if ((_de.qAp - _de.qPe) > 0)
                                        {
                                            Document _doPo = db.Document.FirstOrDefault(fod => fod.id == _de.id_po);

                                            if (_doPo != null)
                                            {
                                                using (DbContextTransaction trans = db.Database.BeginTransaction())
                                                {
                                                    try
                                                    {
                                                        _doPo.isOpen = true;
                                                        db.Document.Attach(_doPo);
                                                        db.Entry(_doPo).State = EntityState.Modified;
                                                        db.SaveChanges();
                                                        trans.Commit();
                                                        _answerCloseDoc = 1;
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                #region AUDITORIA CERRADO DE DOCUMENTO ORDEN DE COMPRA
                                                if (_answerCloseDoc == 1)
                                                {
                                                    DocumentLogDTO _doTmp = new DocumentLogDTO();
                                                    _doTmp.id = _de.id_po;
                                                    _doTmp.code_Action = "ABD";
                                                    _doTmp.id_User = ActiveUser.id;
                                                    _doTmp.description = "EJECUCION MANUAL";
                                                    Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                #endregion
            }

            return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Revert(int id)
        {
            _AnswerfaRG = new AnswerForActionRemissionGuide();
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
            _codeStateDocument = remissionGuide.Document?.DocumentState?.code ?? "";

            _AnswerfaRG.idEntity = id;
            RemissionGuide rgTmp = (RemissionGuide)TempData["remissionGuide"];
            int _answer = 0;
            int _answerCloseDoc = 0;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            var lstRgd = rgTmp
                            .RemissionGuideDetail
                            .Select(s => s.id).ToList();

            if (rgTmp.isManual == null)
                this.ViewBag.isManual = false;
            else
                this.ViewBag.isManual = rgTmp.isManual;

            if (remissionGuide != null)
            {
                remissionGuide.isManual = rgTmp.isManual;
            }
            var rgrd = db.RemissionGuideRiverDetail.FirstOrDefault(fod => fod.id_remisionGuide == id)?.RemissionGuideRiver;

            if (rgrd != null)
            {
                if (remissionGuide.Document.DocumentState.code == "03")
                {
                    if (rgrd.Document.DocumentState.code == "03" || rgrd.Document.DocumentState.code == "06")
                    {
                        TempData.Keep("remissionGuide");
                        _messageAnswer = ErrorMessage("No se puede reversar la guía de remisión debido a tener una guía de Remisión Fluvial anexa");
                        _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(remissionGuide.id, _codeStateDocument);
                        _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                        _AnswerfaRG.messageAnswer = _messageAnswer;

                        return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (remissionGuide.Document.DocumentState.code == "06")
                {
                    if (rgrd.Document.DocumentState.code == "06")
                    {
                        TempData.Keep("remissionGuide");
                        _messageAnswer = ErrorMessage("No se puede reversar la guía de remisión debido a tener una guía de Remisión Fluvial anexa en Estado Autorizado");
                        _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(remissionGuide.id, _codeStateDocument);
                        _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                        _AnswerfaRG.messageAnswer = _messageAnswer;

                        return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var rgPld = db.ProductionLotDetailPurchaseDetail
                            .FirstOrDefault(fod => lstRgd.Contains((int)fod.id_remissionGuideDetail));
            if (rgPld != null)
            {
                if (rgPld.ProductionLotDetail.ProductionLot.Lot.Document.DocumentState.code != "05")
                {
                    TempData.Keep("remissionGuide");
                    _messageAnswer = ErrorMessage("No se puede reversar la guía de remisión debido a ser parte de la Recepción " + rgPld.ProductionLotDetail.ProductionLot.internalNumber + ".");
                    _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(remissionGuide.id, _codeStateDocument);
                    _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                    _AnswerfaRG.messageAnswer = _messageAnswer;

                    return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                }
            }
            var existInInventoryMoveDetailExitDispatchMaterials = remissionGuide.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.InventoryMoveDetailExitDispatchMaterials.Count() > 0);

            if (existInInventoryMoveDetailExitDispatchMaterials != null)
            {
                var existInInventoryMoveEPTAMDL = existInInventoryMoveDetailExitDispatchMaterials.InventoryMoveDetailExitDispatchMaterials.FirstOrDefault().InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EPTAMDL");
                if (!existInInventoryMoveEPTAMDL)
                {
                    TempData.Keep("remissionGuide");
                    _messageAnswer = ErrorMessage("No se puede reversar la guía de remisión debido a tener egresos de materiales de despacho en inventario, manual.");
                    _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(remissionGuide.id, _codeStateDocument);
                    _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                    _AnswerfaRG.messageAnswer = _messageAnswer;

                    return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
                }
            }

            var cantliquida = (from e in db.LiquidationFreightDetail
                               where e.id_remisionGuide == id && e.LiquidationFreight.Document.DocumentState.code != "05"
                               select e.id).Count();

            if (cantliquida > 0)
            {
                TempData.Keep("remissionGuide");
                _messageAnswer = ErrorMessage("No se puede reversar la guía de remisión debido que ya posee una Liquidacion de Flete.");
                _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(remissionGuide.id, _codeStateDocument);
                _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                _AnswerfaRG.messageAnswer = _messageAnswer;

                return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
            }

            if (remissionGuide.hasExitPlanctProduction != null && remissionGuide.hasExitPlanctProduction.Value == true)
            {
                TempData.Keep("remissionGuide");
                _messageAnswer = ErrorMessage("No se puede reversar la guía de remisión debido que ya salio de La Planta.");
                _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(remissionGuide.id, _codeStateDocument);
                _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                _AnswerfaRG.messageAnswer = _messageAnswer;

                return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
            }

            if (remissionGuide.Document.DocumentState.code != "06" && remissionGuide.Document.DocumentState.code != "03")
            {
                TempData.Keep("remissionGuide");
                _messageAnswer = ErrorMessage("No se puede reversar la guía de remisión debido que la Guia no esta AUTORIZADA o APROBADA.");
                _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(remissionGuide.id, _codeStateDocument);
                _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
                _AnswerfaRG.messageAnswer = _messageAnswer;

                return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
            }
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    int id_ep = 0;
                    if (TempData["id_ep"] != null)
                    {
                        id_ep = (int)TempData["id_ep"];
                    }
                    id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                    EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_ep);

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");

                    if (remissionGuide != null && documentState != null)
                    {
                        foreach (var detail in remissionGuide.RemissionGuideDetail)
                        {
                            foreach (var remissionGuideDetailPurchaseOrderDetail in detail.RemissionGuideDetailPurchaseOrderDetail)
                            {
                                ServicePurchaseRemission.UpdateQuantityDispatchedPurchaseOrderDetailRemissionGuide(db, remissionGuideDetailPurchaseOrderDetail.id_purchaseOrderDetail,
                                                                               remissionGuideDetailPurchaseOrderDetail.id_remissionGuideDetail,
                                                                               -remissionGuideDetailPurchaseOrderDetail.quantity);
                            }
                        }

                        var inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == remissionGuide.id &&
                                                                                  (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                        InventoryMove lastInventoryMoveEPTAMDL = (inventoryMoveDetailAux.Count > 0)
                                                            ? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
                                                            : null;
                        inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == remissionGuide.id &&
                                                                             (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                        InventoryMove lastInventoryMoveIPTAMDL = (inventoryMoveDetailAux.Count > 0)
                                                            ? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
                                                            : null;

                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;

                        db.RemissionGuide.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        _answer = 1;
                        TempData["remissionGuide"] = remissionGuide;
                        TempData.Keep("remissionGuide");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + remissionGuide.Document.number + " reversada exitosamente");
                        _messageAnswer = SuccessMessage("Guía de Remisión: " + remissionGuide.Document.number + " reversada exitosamente");
                        _codeStateDocument = documentState.code;
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuide");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    _messageAnswer = ErrorMessage(e.Message);
                }
            }
            if (_answer == 1)
            {
                #region CERRADO DE DOCUMENTOS
                if (_answer == 1)
                {
                    if (remissionGuide.id > 0)
                    {
                        List<int> iLstPod = db.RemissionGuideDetailPurchaseOrderDetail
                                            .Where(w => w.RemissionGuideDetail.RemissionGuide.id == remissionGuide.id)
                                            .Select(s => s.id_purchaseOrderDetail)
                                            .ToList();

                        if (iLstPod != null && iLstPod.Count > 0)
                        {
                            var _iLstPo = db.PurchaseOrderDetail
                                        .Where(w => iLstPod.Contains(w.id))
                                        .GroupBy(g => g.PurchaseOrder.id)
                                        .Select(s => new
                                        {
                                            id_po = s.FirstOrDefault().PurchaseOrder.id,
                                            qAp = s.Sum(su => su.quantityApproved),
                                            qPe = s.Sum(su => su.quantityDispatched)
                                        }).ToList();

                            if (_iLstPo != null && _iLstPo.Count > 0)
                            {
                                #region Listado de Ordenes de Compra
                                foreach (var _de in _iLstPo)
                                {
                                    if (_de.id_po > 0)
                                    {
                                        if ((_de.qAp - _de.qPe) > 0)
                                        {
                                            Document _doPo = db.Document.FirstOrDefault(fod => fod.id == _de.id_po);

                                            if (_doPo != null)
                                            {
                                                using (DbContextTransaction trans = db.Database.BeginTransaction())
                                                {
                                                    try
                                                    {
                                                        _doPo.isOpen = true;
                                                        db.Document.Attach(_doPo);
                                                        db.Entry(_doPo).State = EntityState.Modified;
                                                        db.SaveChanges();
                                                        trans.Commit();
                                                        _answerCloseDoc = 1;
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                #region AUDITORIA APERTURA DE DOCUMENTO ORDEN DE COMPRA
                                                if (_answerCloseDoc == 1)
                                                {
                                                    DocumentLogDTO _doTmp = new DocumentLogDTO();
                                                    _doTmp.id = _de.id_po;
                                                    _doTmp.code_Action = "ABD";
                                                    _doTmp.id_User = ActiveUser.id;
                                                    _doTmp.description = "EJECUCION MANUAL";
                                                    Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                #endregion
            }

            _btnOnEditFormRG = GetActionsOnButtonsRemissionGuide(remissionGuide.id, _codeStateDocument);
            _AnswerfaRG.btnOnEditFormRemissionGuide = _btnOnEditFormRG;
            _AnswerfaRG.messageAnswer = _messageAnswer;

            return Json(_AnswerfaRG, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CancelRG(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
            string codeDocStateRG = remissionGuide?.Document?.DocumentState?.code ?? "";
            RemissionGuide rgTmp = (RemissionGuide)TempData["remissionGuide"];

            if (rgTmp.isManual == null)
                this.ViewBag.isManual = false;
            else
                this.ViewBag.isManual = rgTmp.isManual;

            if (remissionGuide != null)
            {
                remissionGuide.isManual = rgTmp.isManual;
                remissionGuide.getLiquidationInformation();
            }
            var lstRgd = rgTmp
                .RemissionGuideDetail
                .Select(s => s.id).ToList();
            var rgPld = db.ProductionLotDetailPurchaseDetail
                            .FirstOrDefault(fod => lstRgd.Contains((int)fod.id_remissionGuideDetail));
            if (rgPld != null)
            {
                if (rgPld.ProductionLotDetail.ProductionLot.Lot.Document.DocumentState.code != "05")
                {
                    TempData.Keep("remissionGuide");
                    ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido a ser parte de la Recepción " + rgPld.ProductionLotDetail.ProductionLot.internalNumber + ".");
                    return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                }
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    int id_ep = 0;
                    if (TempData["id_ep"] != null)
                    {
                        id_ep = (int)TempData["id_ep"];
                    }
                    id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                    EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == id_ep);

                    var existInInventoryMoveDetailExitDispatchMaterials = remissionGuide
                                                                            .RemissionGuideDispatchMaterial
                                                                            .FirstOrDefault(fod => fod.InventoryMoveDetailExitDispatchMaterials.Count() > 0);//Tiene egreso de Inventario de MDD

                    if (existInInventoryMoveDetailExitDispatchMaterials != null)
                    {
                        var existInInventoryMoveEMD = existInInventoryMoveDetailExitDispatchMaterials
                                                        .InventoryMoveDetailExitDispatchMaterials
                                                        .FirstOrDefault()
                                                        .InventoryMoveDetail
                                                        .InventoryMove
                                                        .InventoryReason.code.Equals("EMD");
                        if (existInInventoryMoveEMD)
                        {
                            TempData.Keep("remissionGuide");
                            ViewData["EditMessage"] = ErrorMessage("No se puede cancelar la guía de remisión debido a tener egresos de materiales de despacho en inventario, manual.");
                            return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
                        }
                    }

                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "08");

                    if (remissionGuide != null && documentState != null)
                    {
                        if (remissionGuide.Document.DocumentState.code != "01")
                        {
                            var inventoryMoveEMDA = remissionGuide
                                                        .RemissionGuideDispatchMaterial
                                                        .FirstOrDefault()?
                                                        .InventoryMoveDetailExitDispatchMaterials?
                                                        .FirstOrDefault(fod => fod.InventoryMoveDetail
                                                                                    .InventoryMove
                                                                                    .InventoryReason.code.Equals("EMDA"))?.InventoryMoveDetail.InventoryMove;//EMDA: Egreso Materiales de Despacho Automatica

                            if (inventoryMoveEMDA != null)
                            {
                                ServiceInventoryMove
                                    .UpdateInventaryMoveExitDispatchMaterials(ActiveUser, ActiveCompany, emissionPoint, remissionGuide, db, true, inventoryMoveEMDA);
                            }
                        }

                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;

                        db.RemissionGuide.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["remissionGuide"] = remissionGuide;
                        TempData.Keep("remissionGuide");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + remissionGuide.Document.number + " cancelado exitosamente");
                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuide");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }

            return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
        }

        #endregion

        #region SELECTED DOCUMENT STATE CHANGE

        [HttpPost, ValidateInput(false)]
        public void ApproveDocuments(int[] ids)
        {
            TempData.Keep("ProductionUnitProviderByProvider");
            var model = (TempData["model"] as List<RemissionGuide>);
            model = model ?? new List<RemissionGuide>();

            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            RemissionGuide remissionGuide = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                            if (remissionGuide != null && documentState != null)
                            {
                                remissionGuide.Document.id_documentState = documentState.id;
                                remissionGuide.Document.DocumentState = documentState;

                                foreach (var details in remissionGuide.RemissionGuideDetail)
                                {
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

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult AutorizeDocuments(int[] ids)
        {
            TempData.Keep("ProductionUnitProviderByProvider");
            var model = (TempData["model"] as List<RGResultsQuery>);
            model = model ?? new List<RGResultsQuery>();

            GenericResultJson oJsonResult = new GenericResultJson();
            List<RemissionGuide> _guiaList = new List<RemissionGuide>();
            string msgXtraInfo = "";
            try
            {
                msgXtraInfo = "Obtener Ruta XML Guía Remisión";
                string routePath = ConfigurationManager.AppSettings["rutaXmlFEX"];
                if (routePath == null)
                {
                    throw new Exception("No definida Configuración Ruta Guía Remisión.");
                }

                msgXtraInfo = "Obtener Ruta XML Guía Remisión(A1.Firmar)";
                string routePathA1Firmar = ConfigurationManager.AppSettings["rutaXmlA1Firmar"];
                if (routePath == null)
                {
                    throw new Exception("No definida Configuración Ruta A1.Firmar.");
                }

                msgXtraInfo = "Obtener Configuración Adicional XML Guía Remisión";
                string stringDelayFEX = ConfigurationManager.AppSettings["delayFEX"];
                int intDelayFEX = 0;
                if (stringDelayFEX == null || !(int.TryParse(stringDelayFEX, out intDelayFEX)))
                {
                    throw new Exception("No definida Configuración Adicional Guía Remisión.");
                }

                if (ids != null && ids.Length > 0)
                {
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var id in ids)
                            {
                                msgXtraInfo = "Obtener Guía Remisión";
                                RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
                                if (remissionGuide == null) throw new Exception("Documento con identificador:" + id + " ,no se ha encontrado.");

                                msgXtraInfo = "Obtener Estado";
                                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "09");
                                if (documentState == null) throw new Exception("Estado con identificador: '09' ,no se ha encontrado.");

                                if (remissionGuide.Document.DocumentState.code != "03") throw new Exception("No se puede autorizar la guía de remisión:" + remissionGuide.Document.number + " ,debido a no tener su estado en Aprobado.");

                                foreach (var details in remissionGuide.RemissionGuideDispatchMaterial)
                                {
                                    var sumQuanty = details.InventoryMoveDetailExitDispatchMaterials?.Sum(s => s.quantity) ?? (decimal)0;
                                    if (sumQuanty != details.sourceExitQuantity)
                                    {
                                        throw new Exception("No se puede autorizar la guía de remisión:" + remissionGuide.Document.number + " ,debido a no tener hecho el egreso de materiales de despacho en inventario.");
                                    }
                                }

                                #region Regeneramos la clave de acceso

                                var document = remissionGuide.Document;
                                string emissionDate = document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");
                                var id_company = document.EmissionPoint.id_company;
                                Company vrCompa = db.Company.Where(h => h.id == id_company).FirstOrDefault();
                                var enviromentCode = vrCompa.CompanyElectronicFacturation.EnvironmentType.codeSRI.ToString();
                                remissionGuide.Document.accessKey = AccessKey.GenerateAccessKey(emissionDate,
                                        document.DocumentType.codeSRI,
                                        document.EmissionPoint.BranchOffice.Division.Company.ruc,
                                            enviromentCode,
                                        document.EmissionPoint.BranchOffice.code.PadLeft(3, '0') + document.EmissionPoint.code.ToString("D3"),
                                        document.sequential.ToString("D9"),
                                        document.sequential.ToString("D8"),
                                        "1");

                                #endregion

                                _guiaList.Add(remissionGuide);

                                remissionGuide.Document.id_documentState = documentState.id;
                                remissionGuide.Document.DocumentState = documentState;

                                db.RemissionGuide.Attach(remissionGuide);
                                db.Entry(remissionGuide).State = EntityState.Modified;
                            }

                            db.SaveChanges();
                            trans.Commit();

                            foreach (var item in _guiaList)
                            {
                                var aModel = model.FirstOrDefault(fod => fod.id == item.id);
                                if (aModel != null)
                                {
                                    model.Remove(aModel);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            trans.Rollback();
                            throw new Exception(e.Message);
                        }
                    }
                }
                else
                {
                    throw new Exception("Debe seleccionar guías de remisión en estado Aprobado.");
                }

                System.Threading.Tasks.Task.Run(() =>
                    ServiceLogistics.CallXML(db,
                                            _guiaList,
                                            null,
                                            (int)ViewData["id_company"],
                                            routePath,
                                            routePathA1Firmar,
                                            intDelayFEX
                                            ));

                oJsonResult.codeReturn = 1;
                oJsonResult.message = SuccessMessage("Guía(s) de remisión(es) en proceso de ser autorizada(s).");
            }
            catch (Exception e)
            {
                oJsonResult.codeReturn = -1;
                oJsonResult.message = ErrorMessage(e.Message);
                LogWrite(e, null, "AutorizeDocuments=>" + msgXtraInfo);
            }

            TempData["model"] = model;
            TempData.Keep("model");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CheckAutorizeRSIDocuments(int[] ids)
        {
            TempData.Keep("ProductionUnitProviderByProvider");
            string msgXtraInfo = "";

            GenericResultJson oJsonResult = new GenericResultJson();
            var model = (TempData["model"] as List<RGResultsQuery>);
            model = model ?? new List<RGResultsQuery>();

            try
            {
                msgXtraInfo = "Obtener Ruta XML Guía De Remisión(B1.AutorizadoActualizado)";
                string routePathB1AutorizadoActualizado = ConfigurationManager.AppSettings["rutaXmlB1AutorizadoActualizado"];
                if (string.IsNullOrEmpty(routePathB1AutorizadoActualizado))
                {
                    throw new Exception("No definida Configuración Ruta B1.AutorizadoActualizado.");
                }

                if (ids != null && ids.Length > 0)
                {
                    msgXtraInfo = "Obtener Guía De Remisión";
                    DocumentState documentState09 = db.DocumentState.FirstOrDefault(s => s.code == "09");
                    RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => ids.Contains(r.id) && r.Document.DocumentState.code != "09");
                    if (remissionGuide != null) throw new Exception("Guía De Remisión: " + remissionGuide.Document.number + " ,tiene estado: " + remissionGuide.Document.DocumentState.name +
                                                             ". Las Guías De Remisión selecionadas deben tener estado: " + documentState09.name);

                    LogController.tratarFicheroLog();
                    bool change = false;

                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {


                            //User token that represents the authorized user account
                            IntPtr token = IntPtr.Zero;
                            string USER_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["USER_rutaXmlB1AutorizadoActualizado"];
                            USER_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(USER_rutaXmlB1AutorizadoActualizado) ? "admin" : USER_rutaXmlB1AutorizadoActualizado;
                            string PASS_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["PASS_rutaXmlB1AutorizadoActualizado"];
                            PASS_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(PASS_rutaXmlB1AutorizadoActualizado) ? "admin" : PASS_rutaXmlB1AutorizadoActualizado;
                            string DOMAIN_rutaXmlB1AutorizadoActualizado = ConfigurationManager.AppSettings["DOMAIN_rutaXmlB1AutorizadoActualizado"];
                            DOMAIN_rutaXmlB1AutorizadoActualizado = string.IsNullOrEmpty(DOMAIN_rutaXmlB1AutorizadoActualizado) ? "" : DOMAIN_rutaXmlB1AutorizadoActualizado;

                            bool result = LogonUser(USER_rutaXmlB1AutorizadoActualizado, DOMAIN_rutaXmlB1AutorizadoActualizado, PASS_rutaXmlB1AutorizadoActualizado, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                            if (result == true)
                            {
                                //Use token to setup a WindowsImpersonationContext 
                                using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                {
                                    String[] dirs = System.IO.Directory.GetDirectories(routePathB1AutorizadoActualizado);

                                    foreach (var id in ids)
                                    {
                                        remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);

                                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06");
                                        ElectronicDocumentState electronicDocumentState = db.ElectronicDocumentState.FirstOrDefault(s => s.code == "03");
                                        var yearMothCurrent = remissionGuide.Document.accessKey.Substring(4, 4).ToString() +
                                                              remissionGuide.Document.accessKey.Substring(2, 2).ToString();
                                        foreach (String dir in dirs)
                                        {
                                            var directoryNameAux = dir.Substring(dir.Length - 8, 6);
                                            if (directoryNameAux == yearMothCurrent)
                                            {
                                                String[] files = System.IO.Directory.GetFileSystemEntries(dir);
                                                foreach (String file in files)
                                                {
                                                    string[] words = Path.GetFileNameWithoutExtension(file).Split('_');
                                                    string accessKeyAux = words[0];
                                                    var aDocument = remissionGuide.Document;
                                                    if (accessKeyAux == aDocument.accessKey)
                                                    {
                                                        aDocument.dateUpdate = DateTime.Now;
                                                        aDocument.id_userUpdate = ActiveUser.id;
                                                        aDocument.id_documentState = documentState.id;
                                                        aDocument.DocumentState = documentState;
                                                        aDocument.authorizationNumber = aDocument.accessKey;

                                                        db.Document.Attach(aDocument);
                                                        db.Entry(aDocument).State = EntityState.Modified;

                                                        var aElectronicDocument = db.ElectronicDocument.FirstOrDefault(fod => fod.Document.accessKey == accessKeyAux);
                                                        if (aElectronicDocument != null)
                                                        {
                                                            aElectronicDocument.id_electronicDocumentState = electronicDocumentState.id;
                                                            aElectronicDocument.ElectronicDocumentState = electronicDocumentState;

                                                            db.ElectronicDocument.Attach(aElectronicDocument);
                                                            db.Entry(aElectronicDocument).State = EntityState.Modified;
                                                        }
                                                        if (!change) change = true;
                                                        var aModel = model.FirstOrDefault(fod => fod.id == aDocument.id);
                                                        if (aModel != null)
                                                        {
                                                            model.Remove(aModel);
                                                        }
                                                    }
                                                }
                                            };
                                        }
                                    }
                                    if (change)
                                    {
                                        db.SaveChanges();
                                        trans.Commit();
                                        oJsonResult.codeReturn = 1;
                                        oJsonResult.message = SuccessMessage("Guías De Remisión verificadas y actualizadas de la autorización del SRI.");
                                        LogController.WriteLog("Actualización de Estados de las Guías De Rmisión a Estado Autorizado Satisfactoriamente(Con Cambios)");
                                    }
                                    else
                                    {
                                        oJsonResult.codeReturn = 1;
                                        oJsonResult.message = WarningMessage("Guías De Remisión verificadas y aun no estan autorizadas por el SRI.");
                                        LogController.WriteLog("Actualización de Estados de las Guías De Remisión a Estado Autorizado Satisfactoriamente(Sin Cambios)");
                                    }
                                    ctx.Undo();
                                    CloseHandle(token);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (change)
                            {
                                trans.Rollback();
                            }
                            LogWrite(e, e.Message, "CheckAutorizeRSIDocuments=>Transacction" + msgXtraInfo);
                            //LogController.WriteLog(e.Message);
                            throw new Exception(e.Message);
                        }
                    }
                }
                else
                {
                    throw new Exception("Debe seleccionar guías de remisión en estado Pre-Autorizado.");
                }
            }
            catch (Exception e)
            {
                oJsonResult.codeReturn = -1;
                oJsonResult.message = ErrorMessage(e.Message);
                LogWrite(e, null, "CheckAutorizeRSIDocuments=>" + msgXtraInfo);
            }

            TempData["model"] = model;
            TempData.Keep("model");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public void ProtectDocuments(int[] ids)
        {
            var model = (TempData["model"] as List<RemissionGuide>);
            model = model ?? new List<RemissionGuide>();

            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            RemissionGuide remissionGuide = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                            if (remissionGuide != null && documentState != null)
                            {
                                remissionGuide.Document.id_documentState = documentState.id;
                                remissionGuide.Document.DocumentState = documentState;
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

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void CancelDocuments(int[] ids)
        {
            var model = (TempData["model"] as List<RemissionGuide>);
            model = model ?? new List<RemissionGuide>();

            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            RemissionGuide remissionGuide = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

                            if (remissionGuide != null && documentState != null)
                            {
                                remissionGuide.Document.id_documentState = documentState.id;
                                remissionGuide.Document.DocumentState = documentState;
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

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void RevertDocuments(int[] ids)
        {
            var model = (TempData["model"] as List<RemissionGuide>);
            model = model ?? new List<RemissionGuide>();

            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            RemissionGuide remissionGuide = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                            if (remissionGuide != null && documentState != null)
                            {
                                remissionGuide.Document.id_documentState = documentState.id;
                                remissionGuide.Document.DocumentState = documentState;

                                foreach (var details in remissionGuide.RemissionGuideDetail)
                                {
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

            TempData["model"] = model;
            TempData.Keep("model");
            TempData.Keep("ProductionUnitProviderByProvider");
        }

        #endregion

        #region REMISSION GUIDE REPORTS

        [HttpPost]
        public JsonResult GetWarehouseListFromDispatchMaterial(int? id_remissionGuide)
        {
            TempData.Keep("remissionGuide");
            var idWarehouses = new
            {
                id = 0
            };

            var lstIdWarehouses = db.RemissionGuideDispatchMaterial
                                        .Where(w => w.id_remisionGuide == id_remissionGuide)
                                        .Select(w => new
                                        {
                                            id = w.Item.ItemInventory.id_warehouse
                                        })
                                        .Distinct();

            return Json(lstIdWarehouses, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPrintInformation(int? id_remissionGuide)
        {
            TempData.Keep("remissionGuide");
            var result = new
            {
                hasAdvanceP = "N",
                hasPersAss = "N",
                hasIceThird = "N"
            };
            decimal valAdvance = 0;
            valAdvance = db.RemissionGuideTransportation
                            .FirstOrDefault(fod => fod.id_remionGuide == id_remissionGuide)?
                            .advancePrice ?? 0;

            int countPers = 0;
            countPers = db.RemissionGuideAssignedStaff.Where(w => w.viaticPrice > 0 && w.id_remissionGuide == id_remissionGuide).ToList().Count();

            var rgciviTmp = db.RemissionGuideCustomizedIceBuyInformation.FirstOrDefault(fod => fod.isActive && fod.quantityIceBagsRequested > 0 && fod.id_RemissionGuide == id_remissionGuide);

            result = new { hasAdvanceP = ((valAdvance > 0) ? "Y" : "N"), hasPersAss = ((countPers > 0) ? "Y" : "N"), hasIceThird = ((rgciviTmp != null) ? "Y" : "N") };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemissionGuideReportFilter(ReportModel reportModel, int? id_warehouse)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            bool isvalid = false;
            string message = "";
            string strnamedata = "reportModel" + DateTime.Now.ToString("yyyyMMddmmssfff");
            int cantidad = 1;

            try
            {
                int id_company = int.Parse(ViewData["id_company"].ToString());
                Setting settingCREI = db.Setting.FirstOrDefault(t => t.code == "CREI" && t.id_company == id_company);//CREI-Parametro de Cantidad antes de la Reimpresion
                if (settingCREI != null)
                {
                    try
                    {
                        cantidad = settingCREI.value != null ? int.Parse(settingCREI.value.ToString()) : 1;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (reportModel == null)
                {
                    reportModel = new ReportModel();
                    reportModel.ReportName = "RemisionGuideReportUnitPreint";
                }
                else
                {
                    if (reportModel.ListReportParameter == null)
                    {
                        reportModel.ListReportParameter = new List<ReportParameter>();
                    }

                    printcontrol printcontrol = new printcontrol()
                    {
                        id_referencia = remissionGuide.id,
                        namereport = reportModel.ReportName,
                        optiondescrip = "RemisionGuide",
                        dateCreate = DateTime.Now,
                        id_userCreate = ActiveUser.id,
                        isActive = true,
                        dateUpdate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        printnumber = 1
                    };

                    printcontrol = DataProviders.DataProviderPrintControl.SaveControlPrint(printcontrol);
                }

                isvalid = true;
            }
            catch
            {
            }

            TempData[strnamedata] = reportModel;
            TempData.Keep(strnamedata);
            TempData.Keep("remissionGuide");

            var result = new
            {
                isvalid,
                message,
                reportModel = strnamedata,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemissionGuideReport(int id)
        {
            try
            {
                Session["URGUIA"] = ConfigurationManager.AppSettings["URGUIA"];
            }
            catch (Exception ex)
            {
                ViewBag.IframeUrl = ex.Message;
            }

            ViewBag.IframeUrl = Session["URGUIA"] + "?ID=" + id;

            return PartialView("IndexReportGuia");
        }

        #endregion

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            var actions = new
            {
                btnApprove = false,
                btnAutorize = false,
                btnCheckAutorizeRSI = false,
                btnProtect = false,
                btnCancel = false,
                btnRevert = false,
                btnreassignment = false,
                btnCancelRG = false,
                btnPrint = false,
                btnPrintAlldoc = false,
                btnPrintManual = false,
                btnPrintAlldocManual = false
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);

            string state = remissionGuide.Document.DocumentState.code;

            //Verifico que se guía de Tercero
            bool rg = remissionGuide?.RemissionGuideTransportation?.isOwn ?? true;

            //Verifico que no tenga recepción asignada
            var lstReceptions = remissionGuide.RemissionGuideDetail.Where(w => w.RemissionGuideDetailPurchaseOrderDetail.Count > 0).ToList();
            bool rgReceptions = (lstReceptions.Count > 0) ? true : false;

            if (state == "01")
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = false,
                    btnreassignment = false,
                    btnCancelRG = false,
                    btnPrint = false,
                    btnPrintAlldoc = false,
                    btnPrintManual = false,
                    btnPrintAlldocManual = false
                };
            }
            else if (state == "03")//|| state == 3) // APROBADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = true,
                    btnCheckAutorizeRSI = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnreassignment = false,
                    btnCancelRG = false,
                    btnPrint = true,
                    btnPrintAlldoc = true,
                    btnPrintManual = true,
                    btnPrintAlldocManual = true
                };
            }
            else if (state == "04" || state == "05") // CERRADA O ANULADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnreassignment = false,
                    btnCancelRG = false,
                    btnPrint = false,
                    btnPrintAlldoc = false,
                    btnPrintManual = false,
                    btnPrintAlldocManual = false
                };
            }
            else if (state == "06") // AUTORIZADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnreassignment = true,
                    btnCancelRG = true,
                    btnPrint = true,
                    btnPrintAlldoc = true,
                    btnPrintManual = true,
                    btnPrintAlldocManual = true
                };
            }
            else if (state == "09") // AUTORIZADA o Pre Autorizada
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = true,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnreassignment = false,
                    btnCancelRG = false,
                    btnPrint = true,
                    btnPrintAlldoc = true,
                    btnPrintManual = true,
                    btnPrintAlldocManual = true
                };
            }
            else if (state == "08")
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnreassignment = false,
                    btnCancelRG = false,
                    btnPrint = false,
                    btnPrintAlldoc = false,
                    btnPrintManual = false,
                    btnPrintAlldocManual = false
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_remissionGuide)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            TempData.Keep("remissionGuide");

            var idsArray = db.RemissionGuide.Select(r => r.id).ToArray();
            int index = idsArray.OrderByDescending(r => r)
                                .ToList()
                                .FindIndex(r => r == id_remissionGuide);
            var result = new
            {
                maximunPages = idsArray.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (remissionGuide != null)
            {
                TempData["remissionGuide"] = remissionGuide;
                this.ViewBag.isManual = (remissionGuide.isManual ?? false);
                TempData.Keep("remissionGuide");
                return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
            }

            TempData.Keep("remissionGuide");

            return PartialView("_RemissionGuideMainFormPartial", new RemissionGuide());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult LockedDocumentOC(int[] ids, string nameDocument, string code_sourceLockedDocument, string namesourceLockedDocument)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        foreach (var item in ids)
                        {
                            var aPurchaseOrderDetail = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == item);
                            result.Message = ServiceLockedDocument.LockedDocument(db, ActiveUser, (aPurchaseOrderDetail?.id_purchaseOrder ?? 0), nameDocument, code_sourceLockedDocument, namesourceLockedDocument);
                            if (result.Message != "OK")
                            {
                                result.Code = -1;
                                break;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
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
        public JsonResult LockedDocument(int id_document, string nameDocument, string code_sourceLockedDocument, string namesourceLockedDocument)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var aRemissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == id_document);
                        if (aRemissionGuide != null)
                        {
                            foreach (var item in aRemissionGuide.RemissionGuideDetail.ToList())
                            {
                                var aIdPurchaseOrder = item.RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault()?.PurchaseOrderDetail.id_purchaseOrder;
                                if (aIdPurchaseOrder != null)
                                {
                                    result.Message = ServiceLockedDocument.LockedDocument(db, ActiveUser, aIdPurchaseOrder.Value, nameDocument, code_sourceLockedDocument, namesourceLockedDocument);
                                    if (result.Message != "OK")
                                    {
                                        result.Code = -1;
                                        break;
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
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
        public JsonResult UnlockedDocument(int id_document, string nameDocument, string code_sourceLockedDocument)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var aRemissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == id_document);
                        aRemissionGuide = aRemissionGuide ?? (RemissionGuide)TempData["remissionGuide"];
                        TempData.Keep("remissionGuide");
                        if (aRemissionGuide != null)
                        {
                            foreach (var item in aRemissionGuide.RemissionGuideDetail.ToList())
                            {
                                var aId_purchaseOrderDetail = item.RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault()?.id_purchaseOrderDetail;
                                var aIdPurchaseOrder = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == aId_purchaseOrderDetail)?.id_purchaseOrder;
                                if (aIdPurchaseOrder != null)
                                {
                                    result.Message = ServiceLockedDocument.UnlockedDocument(db, ActiveUser, aIdPurchaseOrder.Value, nameDocument, code_sourceLockedDocument);
                                    if (result.Message != "OK")
                                    {
                                        result.Code = -1;
                                        break;
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
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

        private void GenerateRequestToWarehouse(RemissionGuide rg)
        {
            ParamRGRequestWarehouse args = new ParamRGRequestWarehouse();
            args.dbTmp = db;
            args.rgTmp = rg;
            args.id_RemissionGuide = rg.id;
            ServiceLogistics.GenerateRequestToWarehouseFromRemissionGuide(args);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult pricefreightrefresh(RemissionGuide pRemissionGuide, Document document,
                                                        RemissionGuideTransportation transportation,
                                                        RemissionGuideExportInformation exportInformation)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            var wresult = pricefreight(pRemissionGuide);

            var result = new
            {
                pricefreight = wresult
            };

            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public decimal pricefreight(RemissionGuide tempRemissionGuide)
        {
            decimal wresult = 0;

            int? id_FishingSite = null;
            int? id_TransportTariffType = null;
            decimal? Pounds = null;
            decimal? IceBag = null;

            try
            {
                var idIceBag = db.Setting.Where(x => x.code == "CODHI" && x.isActive).FirstOrDefault()?.value;

                if (tempRemissionGuide != null)
                {
                    if (tempRemissionGuide.id_productionUnitProvider != null)
                    {
                        id_FishingSite = db.ProductionUnitProvider.Where(x => x.id == tempRemissionGuide.id_productionUnitProvider).FirstOrDefault()?.id_FishingSite;
                    }

                    if (tempRemissionGuide.id_TransportTariffType != null)
                    {
                        id_TransportTariffType = tempRemissionGuide.id_TransportTariffType;
                    }

                    RemissionGuide tempRemissionGuideDetail = (TempData["remissionGuide"] as RemissionGuide);
                    tempRemissionGuideDetail = tempRemissionGuideDetail ?? new RemissionGuide();

                    if (tempRemissionGuideDetail.RemissionGuideDetail != null && tempRemissionGuideDetail.RemissionGuideDetail.Count > 0)
                    {
                        var vPounds = (from e in tempRemissionGuideDetail.RemissionGuideDetail
                                       select e.quantityProgrammed).Sum();
                        if (vPounds > 0) Pounds = vPounds;
                    }

                    if (tempRemissionGuideDetail.RemissionGuideDispatchMaterial != null && tempRemissionGuideDetail.RemissionGuideDispatchMaterial.Count > 0 && !string.IsNullOrEmpty(idIceBag))
                    {
                        var vIceBag = (from e in tempRemissionGuideDetail.RemissionGuideDispatchMaterial
                                       where e.id_item == int.Parse(idIceBag)
                                       select e.sourceExitQuantity).Sum();
                        if (vIceBag > 0) IceBag = vIceBag;
                    }

                    if (id_FishingSite != null && id_FishingSite > 0 && id_TransportTariffType != null && id_TransportTariffType > 0 &&
                        Pounds != null && Pounds > 0 && IceBag != null && IceBag > 0)
                    {
                        var isTerrestriel = db.PurchaseOrderShippingType.Where(x => x.id == tempRemissionGuide.id_shippingType).FirstOrDefault()?.isTerrestriel;

                        if (isTerrestriel != null && isTerrestriel.Value)
                        {
                            var PriceList = (from po in db.TransportTariff
                                             join podet in db.TransportTariffDetail on po.id equals podet.id_TransportTariff
                                             join trasi in db.TransportSize on podet.id_TransportSize equals trasi.id
                                             join pounra in db.PoundsRange on trasi.id_poundsRange equals pounra.id
                                             join ice in db.IceBagRange on trasi.id_iceBagRange equals ice.id
                                             where po.isActive && podet.isActive && trasi.isActive && pounra.isActive && ice.isActive &&
                                                   po.id_TransportTariffType == id_TransportTariffType && podet.id_FishingSite == id_FishingSite
                                             select new { podet.orderTariff, poundsini = pounra.range_ini, poundsend = pounra.range_end, iceini = ice.range_ini, iceend = ice.range_end, podet.tariff }).ToList();

                            if (PriceList != null && PriceList.Count > 0)
                            {
                                var result = (from we in PriceList
                                              where Pounds >= we.poundsini && Pounds <= we.poundsend && IceBag <= we.iceend
                                              select we).ToList();

                                if (result != null && result.Count > 0)
                                {
                                    var data = result.OrderBy(x => x.orderTariff).First().tariff;
                                    wresult = data;
                                }
                                else
                                {
                                    var resultup = (from we in PriceList
                                                    where we.iceend > IceBag
                                                    select we).ToList();

                                    if (resultup != null && resultup.Count > 0)
                                    {
                                        var dataup = resultup.OrderBy(x => x.orderTariff).First().tariff;
                                        wresult = dataup;
                                    }
                                }
                                if (wresult == 0)
                                {
                                    ViewBag.ErrorPriceFreight = ErrorMessage("El valor de flete para este sitio resultó 0, puede ser que no exista el Tarifario de Transporte o Los sacos de Hielo sobrepasen los rangos existenes");
                                }
                            }
                            else
                            {
                                ViewBag.ErrorPriceFreight = ErrorMessage("El valor de flete para este sitio resultó 0, puede ser que no exista el Tarifario de Transporte o Los sacos de Hielo sobrepasen los rangos existenes");
                            }
                        }
                        else
                        {
                            decimal? PriceNotisTerrestriel = (from po in db.TransportTariff
                                                              join podet in db.TransportTariffDetail on po.id equals podet.id_TransportTariff
                                                              where po.isActive && podet.isActive &&
                                                                    po.id_TransportTariffType == id_TransportTariffType &&
                                                                    podet.id_FishingSite == id_FishingSite
                                                              select podet.tariff).FirstOrDefault();

                            if (PriceNotisTerrestriel != null)
                            {
                                wresult = PriceNotisTerrestriel.Value;
                            }
                            else
                            {
                                ViewBag.ErrorPriceFreight = ErrorMessage("El valor de flete para este sitio resultó 0, puede ser que no exista el Tarifario de Transporte o Los sacos de Hielo sobrepasen los rangos existenes");
                            }
                        }
                    }
                }
            }
            catch
            {
                wresult = 0;
            }
            if (wresult == 0)
            {
                ViewBag.ErrorPriceFreight = ErrorMessage("El valor de flete para este sitio resultó 0, puede ser que no exista el Tarifario de Transporte o Los sacos de Hielo sobrepasen los rangos existenes");
            }
            return wresult;
        }

        public JsonResult CalculatePriceFreightRefresh(RGParamPriceFreight rgParam)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            decimal dPriceFreight = CalculatePriceFreight(rgParam);

            var result = new
            {
                priceFreight = dPriceFreight
            };

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public decimal CalculatePriceFreight(RGParamPriceFreight rgParam)
        {
            if (TempData["remissionGuide"] != null)
            {
                TempData.Keep("remissionGuide");
            }
            decimal dPriceFreight = 0;

            int? id_FishingSite = null;
            int? id_TransportTariffType = null;
            decimal? dPounds = 0;
            decimal? dIceBags = 0;
            decimal? dIceBagsThird = 0;
            try
            {
                string idIceBag = db.Setting.Where(x => x.code == "CODHI" && x.isActive).FirstOrDefault()?.value ?? "";
                string idIceBagThird = db.Setting.Where(x => x.code == "CODHIT" && x.isActive).FirstOrDefault()?.value ?? "";

                if (rgParam != null)
                {
                    id_FishingSite = rgParam.id_FishingSite;
                    id_TransportTariffType = rgParam.id_TransportTariff;

                    RemissionGuide tempRemissionGuide = (TempData["remissionGuide"] as RemissionGuide);
                    tempRemissionGuide = tempRemissionGuide ?? new RemissionGuide();
                    #region"Obtengo Libras a transportar y Numero de Sacos de Hielo"
                    //Calculo Libras a despachar en la guía de Remisión
                    if (tempRemissionGuide.RemissionGuideDetail != null
                        && tempRemissionGuide.RemissionGuideDetail.Count > 0)
                    {
                        dPounds = tempRemissionGuide
                                    .RemissionGuideDetail
                                    .Select(s => s.quantityProgrammed)
                                    .DefaultIfEmpty(0).Sum();
                        dPounds = dPounds ?? 0;
                    }

                    //Calculo Sacos de Hielo, se ha utilizado el parámetro CODHI
                    if (tempRemissionGuide.RemissionGuideDispatchMaterial != null
                        && tempRemissionGuide.RemissionGuideDispatchMaterial.Count > 0)
                    {
                        dIceBags = tempRemissionGuide
                                    .RemissionGuideDispatchMaterial
                                    .Where(w => w.id_item == int.Parse(idIceBag))
                                    .Select(s => s.sourceExitQuantity)
                                    .DefaultIfEmpty(0)
                                    .Sum();

                        dIceBagsThird = tempRemissionGuide
                                    .RemissionGuideDispatchMaterial
                                    .Where(w => w.id_item == int.Parse(idIceBagThird))
                                    .Select(s => s.sourceExitQuantity)
                                    .DefaultIfEmpty(0)
                                    .Sum();

                        dIceBags = (dIceBags ?? 0) + (dIceBagsThird ?? 0);
                    }
                    #endregion

                    if (id_FishingSite > 0 && id_TransportTariffType > 0)
                    {
                        //Se Obtiene vía de Transporte
                        var ttTypeTmp = db.TransportTariffType
                                            .FirstOrDefault(fod => fod.id == id_TransportTariffType);

                        ttTypeTmp = ttTypeTmp ?? db.TransportTariffType.FirstOrDefault();

                        var postTmp = db.PurchaseOrderShippingType
                                            .FirstOrDefault(fod => fod.id == ttTypeTmp.id_shippingType);

                        postTmp = postTmp ?? db.PurchaseOrderShippingType.FirstOrDefault();

                        //Se Calculan Tarifarios Terrestres o Fluviales
                        #region"Tarifarios Terrestres"
                        if (postTmp.isTerrestriel)
                        {
                            // Se obtiene lista de precio de Tarifario de Transporte, Detalle, Rango de Hielo, Sacos de Hielo
                            var tariffList = (from TtH in db.TransportTariff.ToList()
                                              join TtD in db.TransportTariffDetail.ToList() on TtH.id equals TtD.id_TransportTariff
                                              join Ts in db.TransportSize.ToList() on TtD.id_TransportSize equals Ts.id
                                              join Pr in db.PoundsRange.ToList() on Ts.id_poundsRange equals Pr.id
                                              join Ibr in db.IceBagRange.ToList() on Ts.id_iceBagRange equals Ibr.id
                                              where TtH.isActive && TtD.isActive && Ts.isActive && Pr.isActive && Ibr.isActive &&
                                                    TtH.id_TransportTariffType == id_TransportTariffType && TtD.id_FishingSite == id_FishingSite
                                              select new
                                              {
                                                  TtD.orderTariff,
                                                  poundsIni = Pr.range_ini,
                                                  poundsEnd = Pr.range_end,
                                                  iceBagsIni = Ibr.range_ini,
                                                  iceBagsEnd = Ibr.range_end,
                                                  TtD.tariff
                                              }).ToList();

                            if (tariffList != null && tariffList.Count > 0)
                            {
                                var ttList = tariffList.Where(w => dPounds >= w.poundsIni
                                                            && dPounds <= w.poundsEnd
                                                            && dIceBags <= w.iceBagsEnd)
                                                        .ToList();

                                if (ttList != null && ttList.Count > 0)
                                {
                                    dPriceFreight = ttList.OrderBy(o => o.orderTariff).First()?.tariff ?? 0;
                                }
                                else
                                {
                                    //Nos Vamos  por el rango de Número de Sacos de Hielo
                                    var ttIceBagList = tariffList
                                                        .Where(w => dIceBags < w.iceBagsEnd)
                                                        .ToList();
                                    if (ttIceBagList != null && ttIceBagList.Count > 0)
                                    {
                                        dPriceFreight = ttIceBagList.OrderBy(o => o.orderTariff).First()?.tariff ?? 0;
                                    }
                                }
                                if (dPriceFreight == 0)
                                {
                                    ViewBag.ErrorPriceFreight = ErrorMessage("El valor de flete para este sitio resultó 0, puede ser que no exista el Tarifario de Transporte o Los sacos de Hielo sobrepasen los rangos existenes");
                                }
                            }
                            else
                            {
                                ViewBag.ErrorPriceFreight = ErrorMessage("El valor de flete para este sitio resultó 0, puede ser que no exista el Tarifario de Transporte o Los sacos de Hielo sobrepasen los rangos existenes");
                            }
                        }
                        #endregion
                        #region"Tarifarios Otros"
                        else
                        {
                            dPriceFreight = db.TransportTariffDetail
                                                .FirstOrDefault(fod => fod.TransportTariff.isActive
                                                        && fod.isActive
                                                        && fod.TransportTariff.id_TransportTariffType == id_TransportTariffType
                                                        && fod.id_FishingSite == id_FishingSite)?.tariff ?? 0;

                            if (dPriceFreight == 0)
                            {
                                ViewBag.ErrorPriceFreight = ErrorMessage("El valor de flete para este sitio resultó 0, puede ser que no exista el Tarifario de Transporte o Los sacos de Hielo sobrepasen los rangos existenes");
                            }
                        }
                        #endregion
                        if (dPriceFreight == 0)
                        {
                            ViewBag.ErrorPriceFreight = ErrorMessage("El valor de flete para este sitio resultó 0, puede ser que no exista el Tarifario de Transporte o Los sacos de Hielo sobrepasen los rangos existenes");
                        }
                    }
                }
            }
            catch (Exception)
            {
                dPriceFreight = 0;
            }

            return dPriceFreight;
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Internalrefresh(bool isInternal)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide.isInternal = false;

            var result = new
            {
                ok = "ok"
            };

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemDetailData(int? id_item)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            RemissionGuideDetail detail = remissionGuide?.RemissionGuideDetail.FirstOrDefault(d => d.id_item == id_item);

            if (detail != null)
            {
            }
            var businessOportunityPlanningDetails = db.BusinessOportunityPlanningDetail.Where(w => w.BusinessOportunityPlaninng.BusinessOportunity.id_company == this.ActiveCompanyId && w.id_item == id_item && w.BusinessOportunityPlaninng.BusinessOportunity.Document.DocumentType.code.Equals("16")).ToList();

            var result = new
            {
                ItemDetailData = new
                {
                    masterCode = item.masterCode,
                    metricUnit = item.ItemPurchaseInformation.MetricUnit.code,
                    businessOportunityPlanningDetails = businessOportunityPlanningDetails.Select(s => new { id = s.id, name = s.BusinessOportunityPlaninng?.BusinessOportunity?.name ?? "" })
                }
            };

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult BusinessOportunityPlanningDetailData(int? id_item)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            var businessOportunityPlanningDetails = db.BusinessOportunityPlanningDetail.Where(w => w.BusinessOportunityPlaninng.BusinessOportunity.id_company == this.ActiveCompanyId && w.id_item == id_item && w.BusinessOportunityPlaninng.BusinessOportunity.Document.DocumentType.code.Equals("16")).ToList();

            var result = new
            {
                businessOportunityPlanningDetails = businessOportunityPlanningDetails.Select(s => new { id = s.id, name = s.BusinessOportunityPlaninng?.BusinessOportunity?.name ?? "" })
            };

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateSecuritySealNumber(int id_remissionGuide, string number, bool isNew)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            TempData.Keep("remissionGuide");

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            remissionGuide.RemissionGuideSecuritySeal = remissionGuide.RemissionGuideSecuritySeal ?? new List<RemissionGuideSecuritySeal>();

            List<RemissionGuideSecuritySeal> tmp = remissionGuide.RemissionGuideSecuritySeal.ToList();
            List<RemissionGuideSecuritySeal> tmp2 = remissionGuide.RemissionGuideSecuritySeal.ToList();
            List<RemissionGuideSecuritySeal> tmp3 = db.RemissionGuideSecuritySeal
                                                        .Where(w => w.isActive && !(w.RemissionGuide.Document.DocumentState.code == "05"
                                                        || w.RemissionGuide.Document.DocumentState.code == "08")).ToList();

            if (isNew == true)
            {
                var securitySeal = remissionGuide.RemissionGuideSecuritySeal.FirstOrDefault(fod => fod.number == number);
                var securitySealAll = tmp3.FirstOrDefault(fod => fod.number == number);
                if (securitySeal != null || securitySealAll != null)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "Número de sello en uso."
                    };
                }
            }
            else
            {
                tmp2 = tmp.Where(w => w.number != number).ToList();
                tmp3 = tmp3.Where(w => w.number != number).ToList();
                var securitySeal2 = tmp2.FirstOrDefault(fod => fod.number == number);
                var securitySeal2All = tmp3.FirstOrDefault(fod => fod.number == number);
                if (securitySeal2 != null || securitySeal2All != null)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "Número de sello en uso."
                    };
                }
            }
            TempData.Keep("remissionGuide");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateAssigendPersonValidation(int id_remissionGuide, int id_person, bool isNew)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            TempData.Keep("remissionGuide");

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            remissionGuide.RemissionGuideAssignedStaff = remissionGuide.RemissionGuideAssignedStaff ?? new List<RemissionGuideAssignedStaff>();

            List<RemissionGuideAssignedStaff> tmp = remissionGuide.RemissionGuideAssignedStaff.ToList();
            List<RemissionGuideAssignedStaff> tmp2 = remissionGuide.RemissionGuideAssignedStaff.ToList();
            if (remissionGuide.RemissionGuideAssignedStaff != null && remissionGuide.RemissionGuideAssignedStaff.Count > 0)
            {
                if (isNew == true)
                {
                    var assignedStaff = remissionGuide.RemissionGuideAssignedStaff.FirstOrDefault(fod => fod.id_person == id_person);
                    if (assignedStaff != null)
                    {
                        result = new
                        {
                            itsRepeated = 1,
                            Error = "La Persona ya esta asignada."
                        };
                    }
                }
                else
                {
                    tmp2 = tmp.Where(w => w.id_person != id_person).ToList();
                    var assignedStaff2 = tmp2.FirstOrDefault(fod => fod.id_person == id_person);
                    if (assignedStaff2 != null)
                    {
                        result = new
                        {
                            itsRepeated = 1,
                            Error = "La Persona ya esta asignada."
                        };
                    }
                }
            }

            TempData.Keep("remissionGuide");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateSelectedRowsPurchaseOrder(int[] ids)
        {
            var result = new
            {
                Message = "OK"
            };

            Provider providerFirst = null;
            Provider providerCurrent = null;
            Person procesPlantFirst = null;
            Person procesPlantCurrent = null;
            ProductionUnitProvider productionUnitProviderFirst = null;
            ProductionUnitProvider productionUnitProviderCurrent = null;
            Person personBuyer = null;
            Person personBuyerCurrent = null;
            bool requiredLogistic = false;
            bool requiredLogisticCurrent = false;
            int? id_priceList = null;
            int? id_priceListCurrent = null;

            string codeState = "";
            string nameState = "";

            int count = 0;
            foreach (var i in ids)
            {
                codeState = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder?.Document?.DocumentState?.code ?? "";
                nameState = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder?.Document?.DocumentState?.name ?? "";

                if (codeState != "06")
                {
                    result = new
                    {
                        Message = ErrorMessage("La orden de compra debe estar en estado Autorizado y actualmente se encuentra en estado " + nameState + ".")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                providerCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder.Provider;
                procesPlantCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder.Person1;
                productionUnitProviderCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder.ProductionUnitProvider;
                personBuyerCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i)?.PurchaseOrder.Person;
                requiredLogisticCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i).PurchaseOrder.requiredLogistic;
                id_priceListCurrent = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == i).PurchaseOrder.id_priceList;

                if (count == 0)
                {
                    providerFirst = providerCurrent;
                    procesPlantFirst = procesPlantCurrent;
                    productionUnitProviderFirst = productionUnitProviderCurrent;
                    personBuyer = personBuyerCurrent;
                    requiredLogistic = requiredLogisticCurrent;
                    id_priceList = id_priceListCurrent;
                }

                if (providerCurrent != providerFirst)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar detalles con proveedores diferentes")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (procesPlantCurrent != procesPlantFirst)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar detalles con proceso diferentes")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (productionUnitProviderFirst != productionUnitProviderCurrent)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar detalles con Unidades de Producción diferentes")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (personBuyer != personBuyerCurrent)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar detalles con Compradores diferentes")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (requiredLogisticCurrent != requiredLogistic)
                {
                    result = new
                    {
                        Message = ErrorMessage("Los Detalles seleccionados provienen de órdenes de compra con distinto tipo de Logística")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (id_priceListCurrent != id_priceList)
                {
                    result = new
                    {
                        Message = ErrorMessage("Los Detalles seleccionados provienen de órdenes de compra con distinta Lista de Precio")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                count++;
            }
            TempData["PurchaseDetailsForReassignment"] = ids;
            TempData.Keep("PurchaseDetailsForReassignment");
            TempData.Keep("remissionGuide");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PurchaseOrderDetails(int? id_itemCurrent, String InventoryLine)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? new RemissionGuide();
            remissionGuide.RemissionGuideDetail = remissionGuide.RemissionGuideDetail ?? new List<RemissionGuideDetail>();

            var items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.isPurchased) || w.id == id_itemCurrent).ToList();

            if (!String.IsNullOrEmpty(InventoryLine) && remissionGuide.isInternal != true)
            {
                int Id_inventoryline = db.InventoryLine.Where(x => x.code == InventoryLine).FirstOrDefault().id;
                items = items.Where(x => x.id_inventoryLine == Id_inventoryline).ToList();
            }
            var result = new
            {
                items = items.Select(s => new
                {
                    id = s.id,
                    masterCode = s.masterCode,
                    itemPurchaseInformationMetricUnitCode = (s.ItemPurchaseInformation != null) ? s.ItemPurchaseInformation.MetricUnit.code : "",
                    name = s.name
                }).ToList(),
                Message = "Ok"
            };

            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItemDetail(int? id_itemNew, int? id_purchaseOrderDetail)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? new RemissionGuide();

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var remissionGuideDetailAux = remissionGuide.RemissionGuideDetail.Where(w => w.isActive && w.id_item == id_itemNew);
            foreach (var detail in remissionGuideDetailAux)
            {
                if (detail.RemissionGuideDetailPurchaseOrderDetail == null || detail.RemissionGuideDetailPurchaseOrderDetail.Count <= 0)
                {
                    var itemNewAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede repetir el Ítem: " + itemNewAux.name +
                                ",  adicionado manualmente,  en los detalles."
                    };
                }
            }

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult DispatchMaterialDetails(int? id_itemCurrent)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            remissionGuide = remissionGuide ?? new RemissionGuide();
            remissionGuide.RemissionGuideDispatchMaterial = remissionGuide.RemissionGuideDispatchMaterial ?? new List<RemissionGuideDispatchMaterial>();

            var items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.InventoryLine.code.Equals("MI") && (w.ItemType.code.Equals("MDD"))) || w.id == id_itemCurrent).ToList();
            var tempItems = new List<Item>();
            foreach (var i in items)
            {
                if (!(remissionGuide.RemissionGuideDispatchMaterial.Any(a => a.id_item == i.id && a.quantityRequiredForPurchase != 0)) || i.id == id_itemCurrent)
                {
                    tempItems.Add(i);
                }
            }
            items = tempItems;
            var result = new
            {
                items = items.Select(s => new
                {
                    id = s.id,
                    masterCode = s.masterCode,
                    ItemInventoryMetricUnitCode = (s.ItemInventory != null) ? s.ItemInventory.MetricUnit.code : "",
                    name = s.name
                }).ToList(),
                Message = "Ok"
            };

            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemDispatchMaterialDetailsData(int? id_item)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                ItemDetailData = new
                {
                    masterCode = item.masterCode,
                    metricUnit = item.ItemInventory?.MetricUnit?.code,
                }
            };

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult VehicleData(int? id_vehicle)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);

            Vehicle vehicle = db.Vehicle.FirstOrDefault(i => i.id == id_vehicle);

            if (vehicle == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var id_providerTBilling = DataProviderVehicle.VehicleProviderTransportistBillingId(id_vehicle);
            VeicleProviderTransport veicleprovider = db.VeicleProviderTransport.Where(g => g.id_vehicle == vehicle.id && g.Estado && g.datefin == null).FirstOrDefault();

            var result = new
            {
                mark = vehicle.mark,
                model = vehicle.model,
                id_VeicleProvider = veicleprovider?.id_Provider,
                hunterLock = vehicle.hunterLockText ?? "",
                id_providerTBilling = id_providerTBilling
            };

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region TRANSPORTATION

        [HttpPost]
        public ActionResult GetFishingSiteRG(int? id_FishingZoneRGNew)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();

            if (id_FishingZoneRGNew == null || id_FishingZoneRGNew < 0)
            {
                if (Request.Params["id_FishingZoneRGNew"] != null && Request.Params["id_FishingZoneRGNew"] != "")
                    id_FishingZoneRGNew = int.Parse(Request.Params["id_FishingZoneRGNew"]);
                else id_FishingZoneRGNew = -1;
            }

            remissionGuide.RemissionGuideTransportation = remissionGuide.RemissionGuideTransportation ?? new RemissionGuideTransportation();
            remissionGuide.RemissionGuideTransportation.id_FishingZoneRGNew = id_FishingZoneRGNew;

            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            return PartialView("componentCascading/_ComboBoxFishingSite", remissionGuide?.RemissionGuideTransportation);
        }

        #endregion

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintRemissionGuideRideReport(int id_rg)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();
            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id_RemissionGuide";
            _param.Valor = id_rg;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "RIDE";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintRemissionGuideReport(int id_rg, string typePrint)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();
            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id_RemissionGuide";
            _param.Valor = id_rg;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "LGRRP1";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);
            if (typePrint == "D")
            {
                _repMod.codeReport = "LGRVD1";
                rep.printType = "D";
                rep.codeReport = "LGRVD1";
            }

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintRemissionGuideReports(int id_rg, int? id_warehouse, string codeReport, string typePrint)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();
            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_rg;

            paramLst.Add(_param);

            if (codeReport == "GRDM1")
            {
                _param = new ParamCR();
                _param.Nombre = "@id_warehouse";
                _param.Valor = (int)id_warehouse;
                paramLst.Add(_param);
            }
            #endregion

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);
            if (typePrint == "D")
            {
                rep.printType = "D";
                if (codeReport == "GRVCR")
                {
                    _repMod.codeReport = "D1GRVC";
                    rep.codeReport = "D1GRVC";
                }
                else if (codeReport == "GRDM1")
                {
                    _repMod.codeReport = "D1GRDM";
                    rep.codeReport = "D1GRDM";
                }
                else if (codeReport == "GRVSR")
                {
                    _repMod.codeReport = "D1GRVS";
                    rep.codeReport = "D1GRVS";
                }
                else if (codeReport == "SHTGR")
                {
                    _repMod.codeReport = "D1SHTV";
                    rep.codeReport = "D1SHTV";
                }
            }

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void UpdatePrintControl()
        {
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintRemissionGuideReportList(RGRFilterWindow rgrfw)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();
            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@fi";
            _param.Valor = rgrfw.str_emissionDateStart;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@ff";
            _param.Valor = rgrfw.str_emissionDateEnd;
            paramLst.Add(_param);

            #endregion

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = rgrfw.codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintRemissionGuideReportListExcel(RGRFilterWindow rgrfw)
        {
            RemissionGuide remissionGuide = (TempData["remissionGuide"] as RemissionGuide);
            remissionGuide = remissionGuide ?? new RemissionGuide();
            TempData["remissionGuide"] = remissionGuide;
            TempData.Keep("remissionGuide");

            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@fi";
            _param.Valor = rgrfw.str_emissionDateStart == null ? "" : rgrfw.str_emissionDateStart;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@ff";
            _param.Valor = rgrfw.str_emissionDateEnd == null ? "" : rgrfw.str_emissionDateEnd;
            paramLst.Add(_param);

            #endregion

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = rgrfw.codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;
            _repMod.nameReport = "Guias de Remision";

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            db.Database.CommandTimeout = 2200;

            List<ResultRemisionGuieT> modelAux = new List<ResultRemisionGuieT>();
            modelAux = db.Database.SqlQuery<ResultRemisionGuieT>
                    ("exec par_Guia_Remision_TerrestreFluvial @fi, @ff",
                    new SqlParameter("fi", paramLst[0].Valor),
                    new SqlParameter("ff", paramLst[1].Valor)
                    ).ToList();

            TempData["modelRemisionGuieT"] = modelAux;


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /*
        private List<RGResultsQuery> GetAllRemissionGuide(RemissionGuide remissionGuide,
                                                  Document document,
                                                  string carRegistration,
                                                  DateTime? startEmissionDate,
                                                  DateTime? endEmissionDate,
                                                  DateTime? startAuthorizationDate,
                                                  DateTime? endAuthorizationDate,
                                                  DateTime? startDespachureDate, DateTime? endDespachureDate,
                                                  DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                                  DateTime? startentranceDateProductionBuilding, DateTime? endentranceDateProductionBuilding,
                                                  int[] items, int[] businessOportunities)
        {
            List<RGResultsQuery> lstRemissionGuides = new List<RGResultsQuery>();

            List<ParamSQL> lstParametersSql = new List<ParamSQL>();

            #region Armo Parametros de Consulta
            int id_docState = 0;
            string str_numberDoc = string.Empty;
            string str_referenceDoc = string.Empty;
            string str_startEmissionDate = string.Empty;
            string str_endEmissionDate = string.Empty;
            string str_startAuthDate = string.Empty;
            string str_endAuthDate = string.Empty;
            string str_accesKey = string.Empty;
            string str_authNumber = string.Empty;
            string str_startDespDate = string.Empty;
            string str_endDespDate = string.Empty;
            string str_startExitPlanctDate = string.Empty;
            string str_endExitPlanctDate = string.Empty;
            string str_startEntrancePlanctDate = string.Empty;
            string str_endEntrancePlanctDate = string.Empty;
            string str_isExport = string.Empty;
            string str_items = string.Empty;
            string str_carRegistration = string.Empty;
            string str_gruia_externa = string.Empty;

            if (document.id_documentState > 0) { id_docState = document.id_documentState; }
            if (!string.IsNullOrEmpty(remissionGuide.Guia_Externa)) { str_gruia_externa = remissionGuide.Guia_Externa; }
            if (!string.IsNullOrEmpty(document.number)) { str_numberDoc = document.number; }
            if (!string.IsNullOrEmpty(document.reference)) { str_referenceDoc = document.reference; }
            if (startEmissionDate != null) { str_startEmissionDate = startEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (endEmissionDate != null) { str_endEmissionDate = endEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (startAuthorizationDate != null) { str_startAuthDate = startAuthorizationDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (endAuthorizationDate != null) { str_endAuthDate = endAuthorizationDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (!string.IsNullOrEmpty(document.accessKey)) { str_accesKey = document.accessKey; }
            if (!string.IsNullOrEmpty(document.authorizationNumber)) { str_authNumber = document.authorizationNumber; }
            if (startDespachureDate != null) { str_startDespDate = startDespachureDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (endDespachureDate != null) { str_endDespDate = endDespachureDate.Value.Date.ToString("yyyy/MM/dd"); }
            if (startexitDateProductionBuilding != null) { str_startExitPlanctDate = startexitDateProductionBuilding.Value.Date.ToString("yyyy/MM/dd"); }
            if (endexitDateProductionBuilding != null) { str_endExitPlanctDate = endexitDateProductionBuilding.Value.Date.ToString("yyyy/MM/dd"); }
            if (startentranceDateProductionBuilding != null) { str_startEntrancePlanctDate = startentranceDateProductionBuilding.Value.Date.ToString("yyyy/MM/dd"); }
            if (endentranceDateProductionBuilding != null) { str_endEntrancePlanctDate = endentranceDateProductionBuilding.Value.Date.ToString("yyyy/MM/dd"); }
            if (!string.IsNullOrEmpty(carRegistration)) { str_carRegistration = carRegistration; }

            #endregion

            XElement xe_parameter = new XElement("Root",
                                            new XElement("RGQ",
                                                new XAttribute("id_docState", id_docState),
                                                new XAttribute("str_numberDoc", str_numberDoc),
                                                new XAttribute("str_referenceDoc", str_referenceDoc),
                                                new XAttribute("str_startEmissionDate", str_startEmissionDate),
                                                new XAttribute("str_endEmissionDate", str_endEmissionDate),
                                                new XAttribute("str_startAuthDate", str_startAuthDate),
                                                new XAttribute("str_endAuthDate", str_endAuthDate),
                                                new XAttribute("str_accesKey", str_accesKey),
                                                new XAttribute("str_authNumber", str_authNumber),
                                                new XAttribute("str_startDespDate", str_startDespDate),
                                                new XAttribute("str_endDespDate", str_endDespDate),
                                                new XAttribute("str_startExitPlanctDate", str_startExitPlanctDate),
                                                new XAttribute("str_endExitPlanctDate", str_endExitPlanctDate),
                                                new XAttribute("str_startEntrancePlanctDate", str_startEntrancePlanctDate),
                                                new XAttribute("str_endEntrancePlanctDate", str_endEntrancePlanctDate),
                                                new XAttribute("str_carRegistration", str_carRegistration),
                                                new XAttribute("str_GuiaExterna", str_gruia_externa)
                                            ));

            ParamSQL _param = new ParamSQL();
            _param.Nombre = "@P_xml";
            _param.TipoDato = DbType.Xml;
            _param.Valor = Utilitarios.General.GeneralStr.InnerXML(xe_parameter);
            lstParametersSql.Add(_param);

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];
            DataSet ds = AccesoDatos.MSSQL.MetodosDatos2.ObtieneDatos(_cadenaConexion
                                                , "pac_Guia_Remision_Resultado"
                                                , _rutaLog
                                                , "Logistics"
                                                , "PROD"
                                                , lstParametersSql);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                lstRemissionGuides = dt.AsEnumerable().Select(s => new RGResultsQuery()
                {
                    id = s.Field<Int32>("id"),
                    numberDoc = s.Field<String>("NumeroDocumento"),
                    numberDocPurchaseOrder = s.Field<String>("NumeroOrdenCompra"),
                    emissionDateDoc = s.Field<DateTime>("FechaEmision"),
                    providerName = s.Field<String>("NombreProveedor"),
                    productionUnitProviderName = s.Field<String>("NombreUnidadProd"),
                    despachureDateDoc = s.Field<DateTime>("FechaDespacho"),
                    certificadoName = s.Field<String>("NombreCertificado"),
                    exitTimePlanctDoc = s.Field<DateTime?>("SalidaPlanta"),
                    entranceTimePlanctDoc = s.Field<DateTime?>("EntradaPlanta"),
                    isThird = s.Field<Boolean>("LogisticaPropia"),
                    stateDoc = s.Field<String>("EstadoDocumento"),
                    stateDocElectronic = s.Field<String>("EstadoElectronico"),
                    personProcesPlant = s.Field<String>("personProcesPlant"),
                    guia_externa = s.Field<String>("GuiaExterna"),
                }).ToList();
            }
            return lstRemissionGuides;
        }

        */
        [HttpPost, ValidateInput(false)]
        public JsonResult GetTypeFromRemissionGuide(int id_rg)
        {
            if (TempData["PurchaseDetailsForReassignment"] != null)
            {
                TempData.Keep("PurchaseDetailsForReassignment");
            }
            string _codeRemGuideType = "";

            _codeRemGuideType = db.RemissionGuide
                                    .FirstOrDefault(fod => fod.id == id_rg)?
                                    .RemissionGuideType?.code;
            var result = new
            {
                RemGuideType = _codeRemGuideType,
                id_RemGuide = id_rg
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult ProviderCopacking()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            var model = db.Provider.Where(g => (g.Person.isActive && (bool)g.Person.isCopacking) && g.Person.id_company == this.ActiveCompanyId)
                    .Select(t => new { name = t.id, id = t.Person.fullname_businessName }).ToList();
            TempData.Keep("item");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Provider()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            var db = new DBContext();

            var model = db.Provider.Where(g => (g.Person.isActive && g.Person.id_company == this.ActiveCompanyId) && g.ProviderGeneralData.ProviderType.code.Equals("40"))
                    .Select(p => new { name = p.id, id = p.Person.fullname_businessName }).ToList();
            TempData.Keep("item");
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}