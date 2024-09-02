using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using DXPANACEASOFT.Models.RemGuideRiver;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace DXPANACEASOFT.Controllers
{
    public class RemissionGuideRiverController : DefaultController
    {
        const int LOGON_TYPE_NEW_CREDENTIALS = 9;
        const int LOGON32_PROVIDER_WINNT50 = 3;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderByProvider(int? id_productionUnitProviderCurrent, int? id_provider)
        {
            if (id_provider == null || id_provider < 0)
            {
                if (Request.Params["id_provider"] != null && Request.Params["id_provider"] != "") id_provider = int.Parse(Request.Params["id_provider"]);
                else id_provider = -1;
            }
            var productionUnitProviderAux = db.ProductionUnitProvider
                .Where(t => t.isActive && t.id_provider == id_provider)
                .ToList();

            var productionUnitProviderCurrentAux = db.ProductionUnitProvider
                .FirstOrDefault(fod => fod.id == id_productionUnitProviderCurrent);
            if (productionUnitProviderCurrentAux != null && !productionUnitProviderAux.Contains(productionUnitProviderCurrentAux))
            {
                productionUnitProviderAux.Add(productionUnitProviderCurrentAux);
            }

            TempData["ProductionUnitProviderByProvider"] = productionUnitProviderAux
                .Select(s => new
                {
                    s.id,
                    s.name,
                })
                .OrderBy(t => t.id)
                .ToList();

            TempData.Keep("ProductionUnitProviderByProvider");
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            TempData.Keep("remissionriverGuide");
            return PartialView("comboboxcascading/_cmbProviderProductionUnitPartial", remissionGuide);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportTariffTypeByshippingType(int? id_shippingType, int? id_TransportTariffTypeCurrent)
        {
            if (id_shippingType == null || id_shippingType < 0)
            {
                if (Request.Params["id_shippingType"] != null && Request.Params["id_shippingType"] != "")
                {
                    id_shippingType = int.Parse(Request.Params["id_shippingType"]);
                }
                else
                {
                    id_shippingType = -1;
                }
            }

            var transportTariffTyperAux = db.TransportTariffType
                .Where(t => t.isActive && t.id_shippingType == id_shippingType)
                .ToList();

            var transportTariffTyperCurrentAux = db.TransportTariffType
                .FirstOrDefault(fod => fod.id == id_TransportTariffTypeCurrent && fod.id_shippingType == id_shippingType);

            if (transportTariffTyperCurrentAux != null && !transportTariffTyperAux.Contains(transportTariffTyperCurrentAux))
            {
                transportTariffTyperAux.Add(transportTariffTyperCurrentAux);
            }

            TempData["TransportTariffTypeByshippingType"] = transportTariffTyperAux
                .Select(s => new
                {
                    s.id,
                    s.name,
                })
                .OrderBy(t => t.id)
                .ToList();

            TempData.Keep("TransportTariffTypeByshippingType");
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            TempData.Keep("remissionriverGuide");
            return PartialView("comboboxcascading/_cmbTransportTariffTypePartial", remissionGuide);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportTariffTypeVehicleType(int? id_TransportTariffType, int? id_vehicleCurrent)
        {
            List<int> listVehicleType = new List<int>();

            if (id_TransportTariffType == null || id_TransportTariffType < 0)
            {
                if (Request.Params["id_TransportTariffType"] != null && Request.Params["id_TransportTariffType"] != "")
                {
                    id_TransportTariffType = int.Parse(Request.Params["id_TransportTariffType"]);
                }
                else
                {
                    id_TransportTariffType = -1;
                }
            }

            var transportTariffTyperVehicleType = db.TransportTariffType_VehicleType
                .Where(t => t.isActive && t.id_transportTariffType == id_TransportTariffType)
                .ToList();

            if (transportTariffTyperVehicleType != null && transportTariffTyperVehicleType.Count > 0)
            {
                var listVehicleTypeax = (from e in transportTariffTyperVehicleType
                                         select e.id_vehicleType).ToList();

                if (listVehicleTypeax != null && listVehicleTypeax.Count > 0)
                {
                    listVehicleType.AddRange(listVehicleTypeax);
                }
            }

            var vehicle = db.Vehicle
                .Where(v => v.isActive && listVehicleType.Contains(v.id_VehicleType))
                .ToList();

            var VehicleCurrentAux = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehicleCurrent && listVehicleType.Contains(fod.id_VehicleType));
            if (VehicleCurrentAux != null && !vehicle.Contains(VehicleCurrentAux)) vehicle.Add(VehicleCurrentAux);


            TempData["TransportTariffTypeVehicleType"] = vehicle.OrderBy(t => t.id).ToList();

            TempData.Keep("TransportTariffTypeVehicleType");
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            TempData.Keep("remissionriverGuide");
            var RemissionGuideRiverTransportation = remissionGuide.RemissionGuideRiverTransportation ?? new RemissionGuideRiverTransportation();

            return PartialView("comboboxcascading/_cmbVehiclePartial", RemissionGuideRiverTransportation);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderPoolByUnitProvider(int? id_productionUnitProviderPoolCurrent, int? id_productionUnitProvider)
        {
            var productionUnitProviderPoolAux = db.ProductionUnitProviderPool.Where(t => t.isActive && t.id_productionUnitProvider == id_productionUnitProvider).ToList();
            var productionUnitProviderPoolCurrentAux = db.ProductionUnitProviderPool.FirstOrDefault(fod => fod.id == id_productionUnitProviderPoolCurrent);
            if (productionUnitProviderPoolCurrentAux != null && !productionUnitProviderPoolAux.Contains(productionUnitProviderPoolCurrentAux)) productionUnitProviderPoolAux.Add(productionUnitProviderPoolCurrentAux);
            TempData["ProductionUnitProviderPoolByUnitProvider"] = productionUnitProviderPoolAux.Select(s => new
            {
                s.id,
                name = s.name
            }).OrderBy(t => t.id).ToList();
            TempData.Keep("ProductionUnitProviderPoolByUnitProvider");
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            TempData.Keep("remissionriverGuide");
            return PartialView("comboboxcascading/_cmbProviderProductionUnitProviderPoolPartial", remissionGuide);
        }

        #region REMISSION GUIDE FILTERS RESULTS
        [HttpPost]
        public ActionResult RemissionGuideRiverResults(RemissionGuideRiver remissionGuide,
                                                  Document document,
                                                  DateTime? startEmissionDate, DateTime? endEmissionDate,
                                                  DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                                  DateTime? startDespachureDate, DateTime? endDespachureDate

                                                 )
        {
            var model = db.RemissionGuideRiver.ToList();

            #region DOCUMENT FILTERS

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

            #region PURCHASE ORDER FILTERS

            if (startDespachureDate != null && endDespachureDate != null)
            {
                model = model.Where(o => DateTime.Compare(startDespachureDate.Value.Date, o.despachureDate.Date) <= 0 && DateTime.Compare(o.despachureDate.Date, endDespachureDate.Value.Date) <= 0).ToList();
            }

            List<RemissionGuideRiver> lstModel = new List<RemissionGuideRiver>();
            lstModel = model
                        .Select(s => new RemissionGuideRiver
                        {
                            id = s.id,
                            Document = DataProviderDocument.Document(s.id),
                            Provider = DataProviderPerson.Provider(s.id_providerRemisionGuideRiver),
                            id_providerRemisionGuideRiver = s.id_providerRemisionGuideRiver,
                            ProductionUnitProvider = DataProviderProductionUnitProvider.ProductionUnitProviderById(s.id_productionUnitProvider),
                            requiredLogistic = s.RemissionGuideRiverDetail.FirstOrDefault().RemissionGuide != null &&
                                               s.RemissionGuideRiverDetail.FirstOrDefault().RemissionGuide.RemissionGuideDetail.FirstOrDefault() != null &&
                                               s.RemissionGuideRiverDetail.FirstOrDefault().RemissionGuide.RemissionGuideDetail.FirstOrDefault().RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault() != null
                                               ? s.RemissionGuideRiverDetail.FirstOrDefault().RemissionGuide.RemissionGuideDetail.FirstOrDefault().RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault().PurchaseOrderDetail.PurchaseOrder.requiredLogistic
                                               : false,
                            id_productionUnitProvider = s.id_productionUnitProvider,
                            id_reciver = s.id_reciver,
                            id_reason = s.id_reason,
                            route = s.route,
                            startAdress = s.startAdress,
                            despachureDate = s.despachureDate,
                            id_shippingType = s.id_shippingType,
                            despachurehour = s.despachurehour,
                            id_TransportTariffType = s.id_TransportTariffType,
                            id_personProcessPlant = s.id_personProcessPlant,
                            Person1 = db.Person.FirstOrDefault(p => p.id == s.id_personProcessPlant)
                        }).ToList();
            #endregion

            TempData["model"] = lstModel;
            TempData.Keep("model");

            return PartialView("_RemissionGuideRiverResultsPartial", lstModel.OrderByDescending(r => r.id).ToList());
        }



        #endregion

        #region REMISSION GUIDE RIVER MASTER DETAILS VIEW
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverDetailViewDetails()
        {
            if (TempData["remissionriverGuide"] != null)
            {
                TempData.Keep("remissionriverGuide");
            }
            int id_remissionriverGuide = (Request.Params["id_remissionriverGuide"] != null && Request.Params["id_remissionriverGuide"] != "") ? int.Parse(Request.Params["id_remissionriverGuide"]) : -1;
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id_remissionriverGuide);
            return PartialView("_RemissionGuideRiverDetailViewDetailsPartial", remissionGuideRiver.RemissionGuideRiverDetail.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverDetailViewDispatchMaterials()
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            TempData.Keep("remissionriverGuide");
            //int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            //RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideRiverDetailViewDispatchMaterialsPartial", remissionGuideRiver.RemissionGuideRiverDispatchMaterial.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverDetailViewAssignedStaff()
        {
            if (TempData["remissionriverGuide"] != null)
            {
                TempData.Keep("remissionriverGuide");
            }
            int id_remissionGuideRiver = (Request.Params["id_remissionriverGuide"] != null && Request.Params["id_remissionriverGuide"] != "") ? int.Parse(Request.Params["id_remissionriverGuide"]) : -1;
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id_remissionGuideRiver);
            return PartialView("_RemissionGuideRiverDetailViewAssignedStaffPartial", remissionGuideRiver.RemissionGuideRiverAssignedStaff.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverDetailViewSecuritySeals()
        {
            if (TempData["remissionriverGuide"] != null)
            {
                TempData.Keep("remissionriverGuide");
            }
            int id_remissionGuideRiver = (Request.Params["id_remissionriverGuide"] != null && Request.Params["id_remissionriverGuide"] != "") ? int.Parse(Request.Params["id_remissionriverGuide"]) : -1;
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id_remissionGuideRiver);
            return PartialView("_RemissionGuideRiverDetailViewSecuritySealsPartial", remissionGuideRiver.RemissionGuideRiverSecuritySeal.ToList());
        }


        #endregion

        #region REMISSION GUIDE HEADER

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverPartial()
        {
            var model = (TempData["model"] as List<RemissionGuideRiver>);
            model = model ?? new List<RemissionGuideRiver>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideRiverPartial", model.OrderByDescending(r => r.id).ToList());
        }

        public bool validtateRemision(int? id_shippingType, RemissionGuideRiver tempRemissionGuideRiver, RemissionGuideRiverTransportation tempRemissionGuideRiverTransportation)
        {
            bool wresult = true;
            try
            {

                if (tempRemissionGuideRiver != null)
                {
                    if (tempRemissionGuideRiver.id_shippingType != null || id_shippingType != null)
                    {
                        var shippi = id_shippingType ?? tempRemissionGuideRiver.id_shippingType;


                        var shippingtype = db.PurchaseOrderShippingType.Where(g => g.id == shippi).FirstOrDefault();

                        if (shippingtype != null && shippingtype.code == "TF")
                        {
                            ViewData["EditMessage"] = ErrorMessage("La Vía de Transporte no es valida");
                            wresult = false;
                        }

                    }

                    if (tempRemissionGuideRiver.id_productionUnitProvider != null)
                    {
                        var id_productionUnitProvider = tempRemissionGuideRiver.id_productionUnitProvider;


                        var ProductionUnitProvider = db.ProductionUnitProvider.Where(g => g.id == id_productionUnitProvider).FirstOrDefault();

                        if (ProductionUnitProvider != null && ProductionUnitProvider.id_FishingSite != null && ProductionUnitProvider.id_FishingSite > 0)
                        {

                            var TransportTariffDetail = db.TransportTariffDetail.Where(X => X.TransportTariff.id_TransportTariffType == tempRemissionGuideRiver.id_TransportTariffType && X.id_FishingSite == ProductionUnitProvider.id_FishingSite
                                                             && X.isActive && X.TransportTariff.isActive).FirstOrDefault();


                            if (TransportTariffDetail == null)
                            {

                                ViewData["EditMessage"] = ErrorMessage("No Tiene Sitio Definido para el Tipo de Tarifario");
                                wresult = false;
                            }
                        }
                        else
                        {
                            ViewData["EditMessage"] = ErrorMessage("La Unidad de Prduccion no Tiene Sitio Definido");
                            wresult = false;
                        }

                    }

                    if (tempRemissionGuideRiverTransportation != null)
                    {
                        if (tempRemissionGuideRiverTransportation.id_vehicle != null)
                        {

                            var vcont = db.RemissionGuideRiverTransportation.Where(g => g.id_vehicle == tempRemissionGuideRiverTransportation.id_vehicle
                                                                                && g.id_remissionGuideRiver != tempRemissionGuideRiver.id
                                                                                && g.RemissionGuideRiver.hasEntrancePlanctProduction != true
                                                                                && g.RemissionGuideRiver.Document.DocumentState.code != "05").ToList().Count;


                            if (vcont > 0)
                            {
                                ViewData["EditMessage"] = ErrorMessage("El Vehiculo ya Esta registrado en Otra Guia");
                                wresult = false;
                            }


                        }





                    }

                }

            }
            catch (Exception e)
            {

                TempData.Keep("remissionriverGuide");
                ViewData["EditMessage"] = ErrorMessage(e.Message);
                wresult = false;
            }

            return wresult;

        }

        public decimal pricefreight(RGRParamPriceFreight rgParam)
        {
            decimal wresult = 0;
            int? id_FishingSite = null;
            int? id_TransportTariffType = null;
            decimal? Pounds = null;

            try
            {


                id_FishingSite = rgParam.id_FishingSite;
                //if (tempRemissionGuideRiver.id_productionUnitProvider != null)
                //{
                //    id_FishingSite = db.ProductionUnitProvider.Where(x => x.id == tempRemissionGuideRiver.id_productionUnitProvider).FirstOrDefault()?.id_FishingSite;
                //}


                id_TransportTariffType = rgParam.id_TransportTariff;



                RemissionGuideRiver tempRemissionGuideRiverDetail = (TempData["remissionriverGuide"] as RemissionGuideRiver);
                tempRemissionGuideRiverDetail = tempRemissionGuideRiverDetail ?? new RemissionGuideRiver();

                if (tempRemissionGuideRiverDetail.RemissionGuideRiverDetail != null && tempRemissionGuideRiverDetail.RemissionGuideRiverDetail.Count > 0)
                {
                    var vPounds = (from e in tempRemissionGuideRiverDetail.RemissionGuideRiverDetail
                                   select e.quantityProgrammed).Sum();
                    if (vPounds > 0) Pounds = vPounds;
                }

                if (id_FishingSite != null && id_FishingSite > 0 && id_TransportTariffType != null && id_TransportTariffType > 0 &&
                    Pounds != null)
                {

                    decimal? PriceNotisTerrestriel = (from po in db.TransportTariff
                                                      join podet in db.TransportTariffDetail on po.id equals podet.id_TransportTariff
                                                      where po.isActive && podet.isActive &&
                                                          po.id_TransportTariffType == id_TransportTariffType &&
                                                          podet.id_FishingSite == id_FishingSite
                                                      select podet.tariff).FirstOrDefault();
                    if (PriceNotisTerrestriel != null) wresult = PriceNotisTerrestriel.Value;
                }

            }
            catch //(Exception ex)
            {

                wresult = 0;
            }

            return wresult;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverPartialAddNew(bool approve, RemissionGuideRiver item, Document document,
                                                        RemissionGuideRiverTransportation transportation,
                                                         string despachurehour)
        {

            #region Asignacion de Guia

            RemissionGuideRiver returnRemissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);


            var id_providerreturn = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle).FirstOrDefault()?.id_Provider;
            transportation.id_provider = id_providerreturn;


            if (transportation.id_vehicle != null)
            {
                transportation.Vehicle = transportation.Vehicle = db.Vehicle.Where(X => X.id == transportation.id_vehicle).FirstOrDefault();
            }

            returnRemissionGuideRiver.RemissionGuideRiverTransportation = transportation;
            returnRemissionGuideRiver.id_shippingType = item.id_shippingType;
            if (returnRemissionGuideRiver.id_shippingType > 0)
            {
                returnRemissionGuideRiver.PurchaseOrderShippingType = db.PurchaseOrderShippingType.Where(vb => vb.id == returnRemissionGuideRiver.id_shippingType).FirstOrDefault();
            }
            returnRemissionGuideRiver.id_TransportTariffType = item.id_TransportTariffType;
            returnRemissionGuideRiver.id_providerRemisionGuideRiver = item.id_providerRemisionGuideRiver;

            returnRemissionGuideRiver.despachureDate = returnRemissionGuideRiver.despachureDate != null ? returnRemissionGuideRiver.despachureDate : item.despachureDate;
            if (!String.IsNullOrEmpty(despachurehour)) returnRemissionGuideRiver.despachurehour = TimeSpan.Parse(despachurehour.Substring(10).ToString());

            returnRemissionGuideRiver.id_productionUnitProvider = returnRemissionGuideRiver.id_productionUnitProvider != null ? returnRemissionGuideRiver.id_productionUnitProvider : item.id_productionUnitProvider;
            if (returnRemissionGuideRiver.id_productionUnitProvider > 0)
            {
                returnRemissionGuideRiver.ProductionUnitProvider = db.ProductionUnitProvider.Where(x => x.id == returnRemissionGuideRiver.id_productionUnitProvider).FirstOrDefault();
                TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, returnRemissionGuideRiver.id_providerRemisionGuideRiver);
                TempData.Keep("ProductionUnitProviderByProvider");
            }

            returnRemissionGuideRiver.id_reason = returnRemissionGuideRiver.id_reason > 0 ? returnRemissionGuideRiver.id_reason : item.id_reason;
            returnRemissionGuideRiver.id_reciver = returnRemissionGuideRiver.id_reciver > 0 ? returnRemissionGuideRiver.id_reciver : item.id_reciver;

            returnRemissionGuideRiver.id_shippingType = returnRemissionGuideRiver.id_shippingType > 0 ? returnRemissionGuideRiver.id_shippingType : item.id_shippingType;
            returnRemissionGuideRiver.id_TransportTariffType = returnRemissionGuideRiver.id_TransportTariffType > 0 ? returnRemissionGuideRiver.id_TransportTariffType : item.id_TransportTariffType;

            returnRemissionGuideRiver.route = returnRemissionGuideRiver.route != null ? returnRemissionGuideRiver.route : item.route;
            returnRemissionGuideRiver.startAdress = returnRemissionGuideRiver.startAdress != null ? returnRemissionGuideRiver.startAdress : item.startAdress;


            #endregion

            RGRParamPriceFreight rgParamTmp = new RGRParamPriceFreight();
            rgParamTmp.id_FishingSite = (transportation?.id_FishingSiteRGR) ?? 0;
            rgParamTmp.id_TransportTariff = item.id_TransportTariffType;

            var flete = pricefreight(rgParamTmp);
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
                    DocumentType documentType2 = db.DocumentType.FirstOrDefault(fod => fod.code == "08");

                    int id_ep = 0;
                    if (TempData["id_ep"] != null)
                    {
                        id_ep = (int)TempData["id_ep"];
                    }
                    id_ep = ((id_ep > 0) ? id_ep : ActiveEmissionPoint.id);

                    document.sequential = documentType?.currentNumber ?? 0;
                    document.number = GetDocumentNumber(document.id_documentType);
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

                    document.accessKey = AccessKey.GenerateAccessKey(despDate,
                                                                    document.DocumentType.codeSRI,
                                                                    document.EmissionPoint.BranchOffice.Division.Company.ruc,
                                                                     //"1",
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

                    #region RemissionGuideRiver

                    item.despachureDate = item.despachureDate;
                    item.Document = document;
                    item.id_shippingType = item.id_shippingType;
                    if (!String.IsNullOrEmpty(despachurehour)) item.despachurehour = TimeSpan.Parse(despachurehour.Substring(10).ToString());
                    item.id_TransportTariffType = item.id_TransportTariffType;
                    item.id_personProcessPlant = returnRemissionGuideRiver.id_personProcessPlant;

                    #endregion

                    #region REMISSION GUIDE TRANSPORTATION
                    if (transportation != null)
                    {
                        item.RemissionGuideRiverTransportation = item.RemissionGuideRiverTransportation ?? new RemissionGuideRiverTransportation();
                        transportation.valuePrice = flete;
                        if (transportation.id_provider != null && transportation.id_provider > 0)
                        {
                            item.RemissionGuideRiverTransportation.id_provider = transportation.id_provider;
                        }
                        else
                        {
                            var id_provider = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle).FirstOrDefault()?.id_Provider;
                            transportation.id_provider = id_provider;
                        }

                        var id_VeicleProviderTransport = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle && x.datefin == null && x.Estado).FirstOrDefault()?.id;
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
                        item.RemissionGuideRiverTransportation = transportation;
                    }
                    #endregion

                    RemissionGuideRiver tempRemissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
                    tempRemissionGuideRiver = tempRemissionGuideRiver ?? new RemissionGuideRiver();

                    #region Valida Producto por Guia

                    var advancePrice = transportation.advancePrice ?? 0;
                    var valuePrice = transportation.valuePrice ?? 0;


                    if (advancePrice >= valuePrice && (advancePrice != 0 || valuePrice != 0))
                    {
                        tempRemissionGuideRiver.RemissionGuideRiverTransportation = transportation;
                        TempData["remissionriverGuide"] = returnRemissionGuideRiver;
                        TempData.Keep("remissionriverGuide");
                        ViewData["EditMessage"] = ErrorMessage("EL Valor del anticipo debe ser menor al valor del Flete, favor revisar...");
                        return PartialView("_RemissionGuideRiverMainFormPartial", returnRemissionGuideRiver);

                    }
                    #endregion

                    #region REMISSION GUIDE DETAILS

                    if (tempRemissionGuideRiver?.RemissionGuideRiverDetail != null)
                    {
                        item.RemissionGuideRiverDetail = new List<RemissionGuideRiverDetail>();



                        var details = tempRemissionGuideRiver.RemissionGuideRiverDetail.ToList();

                        foreach (var detail in details)
                        {
                            var remissionGuideDetail = new RemissionGuideRiverDetail
                            {
                                quantityOrdered = detail.quantityOrdered,
                                quantityProgrammed = detail.quantityProgrammed,
                                quantityDispatchMaterial = detail.quantityDispatchMaterial,
                                id_remisionGuide = detail.id_remisionGuide,
                                RemissionGuide = db.RemissionGuide.Where(x => x.id == detail.id_remisionGuide).FirstOrDefault(),
                                isActive = detail.isActive,
                                id_userCreate = detail.id_userCreate,
                                dateCreate = detail.dateCreate,
                                id_item = detail.id_item,
                                id_userUpdate = detail.id_userUpdate,
                                dateUpdate = detail.dateUpdate,
                            };
                            item.RemissionGuideRiverDetail.Add(remissionGuideDetail);
                        }
                    }

                    if (item.RemissionGuideRiverDetail.Count == 0)
                    {

                        TempData["remissionriverGuide"] = returnRemissionGuideRiver;
                        TempData.Keep("remissionriverGuide");
                        ViewData["EditMessage"] = ErrorMessage("No se puede guardar una guía de remisión sin detalles");
                        return PartialView("_RemissionGuideRiverMainFormPartial", returnRemissionGuideRiver);

                    }
                    else
                    {
                        //var id_provider = item.id_providerRemisionGuideRiver;
                        //var id_produccionunit = item.id_productionUnitProvider;
                        //var despachureDate = item.despachureDate.Date;
                        //var id_reason = item.id_reason;
                        //var id_shippingType = item.id_shippingType;

                        //var cnt = (from dre in item.RemissionGuideRiverDetail
                        //           where dre.RemissionGuide.id_providerRemisionGuide != id_provider ||
                        //           dre.RemissionGuide.id_productionUnitProvider != id_produccionunit ||
                        //           dre.RemissionGuide.id_reason != id_reason ||
                        //           // dre.RemissionGuide.ProductionUnitProvider.id_shippingType != id_shippingType ||
                        //          dre.RemissionGuide.despachureDate.ToString("ddMMYYYY") != despachureDate.ToString("ddMMYYYY")
                        //           //  DbFunctions.TruncateTime(dre.RemissionGuide.despachureDate) != DbFunctions.TruncateTime(despachureDate)
                        //           select dre.id
                        //              ).ToList().Count();
                        //if (cnt > 0)
                        //{

                        //    TempData["remissionriverGuide"] = returnRemissionGuideRiver;
                        //    TempData.Keep("remissionriverGuide");
                        //    ViewData["EditMessage"] = ErrorMessage("No se puede guardar una guía de remisión hay detalles diferntes a los definidos");
                        //    return PartialView("_RemissionGuideRiverMainFormPartial", returnRemissionGuideRiver);
                        //}








                    }

                    #endregion

                    #region REMISSION GUIDE RIVER SECURITY SEALS

                    if (tempRemissionGuideRiver?.RemissionGuideRiverSecuritySeal != null)
                    {
                        var details = tempRemissionGuideRiver.RemissionGuideRiverSecuritySeal.ToList();

                        foreach (var detail in details)
                        {

                            var remissionGuideRiverSecuritySeal = new RemissionGuideRiverSecuritySeal
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

                            item.RemissionGuideRiverSecuritySeal.Add(remissionGuideRiverSecuritySeal);
                        }
                    }

                    #endregion

                    #region REMISSION GUIDE RIVER ASSIGNED STAFF

                    if (tempRemissionGuideRiver?.RemissionGuideRiverAssignedStaff != null)
                    {
                        var details = tempRemissionGuideRiver.RemissionGuideRiverAssignedStaff.ToList();

                        foreach (var detail in details)
                        {

                            var remissionGuideRiverAssignedStaff = new RemissionGuideRiverAssignedStaff
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

                            item.RemissionGuideRiverAssignedStaff.Add(remissionGuideRiverAssignedStaff);
                        }
                    }

                    #endregion

                    #region REMISSION GUIDE RIVER DISPATCH MATERIALS

                    if (tempRemissionGuideRiver?.RemissionGuideRiverAssignedStaff != null)
                    {
                        var details = tempRemissionGuideRiver.RemissionGuideRiverDispatchMaterial.ToList();

                        foreach (var detail in details)
                        {

                            var remissionGuideRiverDispatchMaterial = new RemissionGuideRiverDispatchMaterial
                            {
                                id_item = detail.id_item,
                                id_warehouse = detail.id_warehouse,
                                id_warehouselocation = (int)detail.id_warehouselocation,
                                sourceExitQuantity = detail.sourceExitQuantity,
                                sendedDestinationQuantity = detail.sendedDestinationQuantity
                            };

                            item.RemissionGuideRiverDispatchMaterial.Add(remissionGuideRiverDispatchMaterial);
                        }
                    }

                    #endregion

                    #region DATA HELP FOR REMISSION GUIDE REPORT

                    item.RemissionGuideRiverReportLinealDataHelp = item.RemissionGuideRiverReportLinealDataHelp ?? new RemissionGuideRiverReportLinealDataHelp();

                    //Consulto Vehiculo Hunter
                    if (item.RemissionGuideRiverTransportation != null)
                    {
                        int id_vehTmp = item.RemissionGuideRiverTransportation.id_vehicle ?? 0;
                        if (id_vehTmp > 0)
                        {
                            hasHDTmp = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehTmp)?.hasHunterDevice ?? false;
                            if (hasHDTmp)
                            {
                                item.RemissionGuideRiverReportLinealDataHelp.nameSecurityPerson = db.Setting.FirstOrDefault(fod => fod.code == "NDHL")?.value ?? "";
                            }
                        }
                    }
                    //Person Asignado
                    int id_SegRol = db.RemissionGuideAssignedStaffRol
                                        .FirstOrDefault(fod => fod.code == "SEG")?.id ?? 0;
                    int id_SegTec = db.RemissionGuideAssignedStaffRol
                                        .FirstOrDefault(fod => fod.code == "BIO")?.id ?? 0;


                    int id_rgasSeg = item
                                    .RemissionGuideRiverAssignedStaff
                                    .FirstOrDefault(fod => fod.id_assignedStaffRol == id_SegRol)?.id_person ?? 0;

                    int id_rgasTec = item
                                        .RemissionGuideRiverAssignedStaff
                                        .FirstOrDefault(fod => fod.id_assignedStaffRol == id_SegTec)?.id_person ?? 0;
                    //SEGURIDAD
                    if (id_rgasSeg > 0)
                    {
                        item.RemissionGuideRiverReportLinealDataHelp.id_SecurityPerson = id_rgasSeg;
                        item.RemissionGuideRiverReportLinealDataHelp.nameSecurityPerson += (hasHDTmp ? "/" : "") + db.Person.FirstOrDefault(fod => fod.id == id_rgasSeg)?.fullname_businessName ?? "";
                    }
                    //BIOLOGO
                    if (id_rgasTec > 0)
                    {
                        item.RemissionGuideRiverReportLinealDataHelp.id_TechnicianPerson = id_rgasTec;
                        item
                            .RemissionGuideRiverReportLinealDataHelp
                            .nameTechnicianPerson = db.Person.FirstOrDefault(fod => fod.id == id_rgasTec)?.fullname_businessName ?? "";
                    }
                    //SELLO DE SEGURIDAD
                    int str_SellOut = db.RemissionGuideTravelType
                                        .FirstOrDefault(fod => fod.code == "IDA")?.id ?? 0;
                    int str_SellIn = db.RemissionGuideTravelType
                                        .FirstOrDefault(fod => fod.code == "REGRESO")?.id ?? 0;

                    item
                        .RemissionGuideRiverReportLinealDataHelp
                        .SealNumberOneExit = item.RemissionGuideRiverSecuritySeal.FirstOrDefault(fod => fod.id_travelType == str_SellOut)?.number ?? "";

                    item
                        .RemissionGuideRiverReportLinealDataHelp
                        .SealNumberTwoEntrance = item.RemissionGuideRiverSecuritySeal.FirstOrDefault(fod => fod.id_travelType == str_SellIn)?.number ?? "";
                    #endregion

                    #region PERSONALIZATION DETAIL BY DOCUMENT TYPE
                    List<tbsysDocumentsPersonalizationDetail> lstDocPersDetail = db.tbsysDocumentsPersonalizationDetail
                                                            .Where(fod => fod.id_DocumentType == documentType.id)
                                                            .ToList();

                    if (lstDocPersDetail != null && lstDocPersDetail.Count > 0)
                    {
                        foreach (var det in lstDocPersDetail)
                        {
                            #region INFORMACION CUSTOMIZABLE VIATICO PERSONAL ASIGNADO
                            if (det.nameObjectTable == "RemissionGuideRiverCustomizedViaticPersonalAssigned")
                            {
                                int iQuantityPersonalAssigned = item?.RemissionGuideRiverAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).ToList().Count() ?? 0;
                                if (iQuantityPersonalAssigned > 0)
                                {
                                    item.RemissionGuideRiverCustomizedViaticPersonalAssigned = item.RemissionGuideRiverCustomizedViaticPersonalAssigned ?? new RemissionGuideRiverCustomizedViaticPersonalAssigned();
                                    item.RemissionGuideRiverCustomizedViaticPersonalAssigned.Document = new Document();

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
                                    item.RemissionGuideRiverCustomizedViaticPersonalAssigned.Document = docViaticPersonal;
                                    #endregion

                                    item.RemissionGuideRiverCustomizedViaticPersonalAssigned.isActive = true;
                                    item.RemissionGuideRiverCustomizedViaticPersonalAssigned.hasPayment = false;
                                    item.RemissionGuideRiverCustomizedViaticPersonalAssigned.valueTotal = item?.RemissionGuideRiverAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() ?? 0;

                                }
                            }
                            #endregion

                            #region INFORMACION CUSTOMIZABLE ANTICIPO A TRANSPORTISTA
                            if (det.nameObjectTable == "RemissionGuideRiverCustomizedAdvancedTransportist")
                            {
                                if (item.RemissionGuideRiverTransportation?.advancePrice > 0)
                                {
                                    item.RemissionGuideRiverCustomizedAdvancedTransportist = item.RemissionGuideRiverCustomizedAdvancedTransportist ?? new RemissionGuideRiverCustomizedAdvancedTransportist();
                                    item.RemissionGuideRiverCustomizedAdvancedTransportist.Document = new Document();

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
                                    item.RemissionGuideRiverCustomizedAdvancedTransportist.Document = docAdvanceTransportist;
                                    #endregion

                                    item.RemissionGuideRiverCustomizedAdvancedTransportist.isActive = true;
                                    item.RemissionGuideRiverCustomizedAdvancedTransportist.hasPayment = false;
                                    item.RemissionGuideRiverCustomizedAdvancedTransportist.valueTotal = item?.RemissionGuideRiverTransportation?.advancePrice ?? 0;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    if (approve)
                    {
                        item.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                    }

                    item.route = item.route ?? "";
                    db.RemissionGuideRiver.Add(item);
                    db.SaveChanges();
                    trans.Commit();
                    item.getLiquidationInformation();
                    if (transportation != null)
                    {
                        transportation.Vehicle = db.Vehicle.Where(i => i.id == transportation.id_vehicle).FirstOrDefault();
                        item.RemissionGuideRiverTransportation = transportation;
                    }

                    TempData["remissionriverGuide"] = item;
                    TempData.Keep("remissionriverGuide");

                    ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + item.Document.number + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionriverGuide");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_RemissionGuideRiverMainFormPartial", item);
        }

        public JsonResult ValidateVehicleAnotherRemissionGuideRiverProviderCompany(int? id_vehicle, int id_remissionguideRiver)
        {
            RemissionGuideRiver returnRemissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            TempData["remissionriverGuide"] = returnRemissionGuide;
            TempData.Keep("remissionriverGuide");


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
            var vcont = db.RemissionGuideRiverTransportation
                                .Where(g => g.id_vehicle == id_vehicle &&
                                            g.id_remissionGuideRiver != returnRemissionGuide.id

                                            && g.RemissionGuideRiver.hasEntrancePlanctProduction != true
                                            //&& DbFunctions.TruncateTime(g.RemissionGuide.despachureDate) == DbFunctions.TruncateTime(dtDespachureHour) 
                                            && g.RemissionGuideRiver.Document.DocumentState.code != "05").ToList().Count;

            string lstVehicles = "";
            if (vcont > 0)
            {
                lstVehicles = string.Join(", ", db.RemissionGuideRiverTransportation
                    .Where(g => g.id_vehicle == id_vehicle &&
                                g.id_remissionGuideRiver != returnRemissionGuide.id

                                && g.RemissionGuideRiver.hasEntrancePlanctProduction != true
                                //&& DbFunctions.TruncateTime(g.RemissionGuide.despachureDate) == DbFunctions.TruncateTime(dtDespachureHour) 
                                && g.RemissionGuideRiver.Document.DocumentState.code != "05")
                                .Select(s => s.RemissionGuideRiver.Document.number)
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
        public ActionResult RemissionGuideRiverPartialUpdate(bool approve, RemissionGuideRiver item, Document document,
                                                        RemissionGuideRiverTransportation transportation,
                                                         string despachurehour)
        {
            RemissionGuideRiver returnRemissionGuideRiver = null;
            if (TempData["remissionriverGuide"] != null)
            {
                returnRemissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            }
            RGRParamPriceFreight rgParamTmp = new RGRParamPriceFreight();
            rgParamTmp.id_FishingSite = (transportation?.id_FishingSiteRGR) ?? 0;
            rgParamTmp.id_TransportTariff = item.id_TransportTariffType;
            var flete = pricefreight(rgParamTmp);

            returnRemissionGuideRiver = returnRemissionGuideRiver ?? db.RemissionGuideRiver.FirstOrDefault(r => r.id == item.id);
            var id_providerreturn = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle).FirstOrDefault()?.id_Provider;
            transportation.id_provider = id_providerreturn;
            if (transportation.id_vehicle != null)
            {
                transportation.Vehicle = transportation.Vehicle = db.Vehicle.Where(X => X.id == transportation.id_vehicle).FirstOrDefault();
            }
            returnRemissionGuideRiver.RemissionGuideRiverTransportation = transportation;
            returnRemissionGuideRiver.id_shippingType = item.id_shippingType;
            returnRemissionGuideRiver.id_providerRemisionGuideRiver = item.id_providerRemisionGuideRiver;
            if (returnRemissionGuideRiver.id_shippingType > 0)
            {
                returnRemissionGuideRiver.PurchaseOrderShippingType = db.PurchaseOrderShippingType.Where(vb => vb.id == returnRemissionGuideRiver.id_shippingType).FirstOrDefault();
            }
            returnRemissionGuideRiver.id_TransportTariffType = item.id_TransportTariffType;

            #region Asignacion de Guia
            returnRemissionGuideRiver.despachureDate = item.despachureDate;
            if (!String.IsNullOrEmpty(despachurehour)) returnRemissionGuideRiver.despachurehour = TimeSpan.Parse(despachurehour.Substring(10).ToString());
            returnRemissionGuideRiver.id_productionUnitProvider = item.id_productionUnitProvider;
            if (returnRemissionGuideRiver.id_productionUnitProvider > 0)
            {
                returnRemissionGuideRiver.ProductionUnitProvider = db.ProductionUnitProvider.Where(x => x.id == returnRemissionGuideRiver.id_productionUnitProvider).FirstOrDefault();
                TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, returnRemissionGuideRiver.id_providerRemisionGuideRiver);
                TempData.Keep("ProductionUnitProviderByProvider");
            }

            returnRemissionGuideRiver.id_reason = item.id_reason;
            returnRemissionGuideRiver.id_reciver = item.id_reciver;
            returnRemissionGuideRiver.id_shippingType = item.id_shippingType;
            returnRemissionGuideRiver.id_TransportTariffType = item.id_TransportTariffType;
            returnRemissionGuideRiver.route = item.route;
            returnRemissionGuideRiver.startAdress = item.startAdress;

            #endregion

            //if (!validtateRemision(item.id_shippingType, item, transportation))
            //{
            //    TempData["remissionriverGuide"] = returnRemissionGuideRiver;
            //    TempData.Keep("remissionriverGuide");
            //    return PartialView("_RemissionGuideRiverMainFormPartial", returnRemissionGuideRiver);
            //}

            RemissionGuideRiver modelItem = db.RemissionGuideRiver.FirstOrDefault(r => r.id == item.id);
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

                        #region RemissionGuideRiver

                        modelItem.id_reciver = item.id_reciver;
                        modelItem.Person = db.Person.FirstOrDefault(p => p.id == item.id_reciver);
                        modelItem.id_reason = item.id_reason;
                        modelItem.RemissionGuideReason = db.RemissionGuideReason.FirstOrDefault(r => r.id == item.id_reason);
                        modelItem.startAdress = item.startAdress;
                        modelItem.route = item.route ?? "";
                        modelItem.despachureDate = item.despachureDate;
                        modelItem.id_shippingType = item.id_shippingType;
                        modelItem.id_TransportTariffType = item.id_TransportTariffType;
                        modelItem.id_personProcessPlant = returnRemissionGuideRiver.id_personProcessPlant; //item.id_personProcessPlant;


                        if (!String.IsNullOrEmpty(despachurehour)) modelItem.despachurehour = TimeSpan.Parse(despachurehour.Substring(10).ToString());


                        #endregion

                        #region REMISSION GUIDE TRANSPORTATION

                        if (transportation != null)
                        {
                            modelItem.RemissionGuideRiverTransportation = modelItem.RemissionGuideRiverTransportation ?? new RemissionGuideRiverTransportation();
                            modelItem.RemissionGuideRiverTransportation.valuePrice = flete;
                            transportation.valuePrice = flete;
                            modelItem.RemissionGuideRiverTransportation.advancePrice = transportation.advancePrice;

                            if (transportation.id_provider != null && transportation.id_provider > 0)
                            {
                                modelItem.RemissionGuideRiverTransportation.id_provider = transportation.id_provider;
                            }
                            else
                            {
                                var id_provider = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle).FirstOrDefault()?.id_Provider;
                                modelItem.RemissionGuideRiverTransportation.id_provider = id_provider;
                                transportation.id_provider = id_provider;
                            }

                            var id_VeicleProviderTransport = db.VeicleProviderTransport.Where(x => x.id_vehicle == transportation.id_vehicle && x.datefin == null && x.Estado).FirstOrDefault()?.id;
                            var id_VehicleProviderTransportBilling = db.VehicleProviderTransportBilling.FirstOrDefault(fod => fod.id_vehicle == transportation.id_vehicle && fod.datefin == null && fod.state)?.id;

                            if (id_VeicleProviderTransport != null)
                            {
                                var DriverVeicleProviderTransportAx = db.DriverVeicleProviderTransport.Where(x => x.idVeicleProviderTransport == id_VeicleProviderTransport && x.id_driver == transportation.id_driver).FirstOrDefault();

                                if (DriverVeicleProviderTransportAx != null)
                                {
                                    modelItem.RemissionGuideRiverTransportation.id_DriverVeicleProviderTransport = DriverVeicleProviderTransportAx.id;
                                }
                                else
                                {
                                    DriverVeicleProviderTransport driverVeicleProviderTransport = new DriverVeicleProviderTransport()
                                    {
                                        Estado = true,
                                        idVeicleProviderTransport = id_VeicleProviderTransport,
                                        id_driver = transportation.id_driver,

                                    };

                                    modelItem.RemissionGuideRiverTransportation.DriverVeicleProviderTransport = driverVeicleProviderTransport;
                                    transportation.DriverVeicleProviderTransport = driverVeicleProviderTransport;
                                }
                            }
                            modelItem.RemissionGuideRiverTransportation.id_vehicle = transportation.id_vehicle;
                            modelItem.RemissionGuideRiverTransportation.id_driver = transportation.id_driver;
                            modelItem.RemissionGuideRiverTransportation.driverName = transportation.driverName;
                            modelItem.RemissionGuideRiverTransportation.id_VehicleProviderTranportistBilling = id_VehicleProviderTransportBilling;
                            modelItem.RemissionGuideRiverTransportation.id_FishingSiteRGR = transportation.id_FishingSiteRGR;
                            modelItem.RemissionGuideRiverTransportation.descriptionTrans = transportation.descriptionTrans;


                        }

                        #endregion

                        RemissionGuideRiver tempRemissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
                        tempRemissionGuideRiver = tempRemissionGuideRiver ?? new RemissionGuideRiver();


                        #region Valida Producto por Guia


                        var advancePrice = transportation.advancePrice ?? 0;
                        var valuePrice = transportation.valuePrice ?? 0;


                        if (advancePrice >= valuePrice && (advancePrice != 0 || valuePrice != 0))
                        {

                            tempRemissionGuideRiver.RemissionGuideRiverTransportation = transportation;
                            //TempData["remissionriverGuide"] = tempRemissionGuideRiver;
                            TempData["remissionriverGuide"] = returnRemissionGuideRiver;
                            TempData.Keep("remissionriverGuide");
                            ViewData["EditMessage"] = ErrorMessage("EL Valor del anticipo debe ser menor al valor del Flete, favor revisar...");
                            //      return PartialView("_RemissionGuideRiverMainFormPartial", tempRemissionGuideRiver);
                            return PartialView("_RemissionGuideRiverMainFormPartial", returnRemissionGuideRiver);

                        }
                        #endregion

                        #region REMISSION GUIDE DETAILS

                        int count = 0;

                        if (tempRemissionGuideRiver?.RemissionGuideRiverDetail != null)
                        {
                            var details = tempRemissionGuideRiver.RemissionGuideRiverDetail.ToList();

                            var ids_remissionGuideDispatchMaterial = new List<int>();

                            foreach (var detail in details)
                            {
                                RemissionGuideRiverDetail remissionGuideDetail = modelItem.RemissionGuideRiverDetail.FirstOrDefault(d => d.id == detail.id);

                                if (remissionGuideDetail == null)
                                {
                                    remissionGuideDetail = new RemissionGuideRiverDetail
                                    {
                                        quantityDispatchMaterial = detail.quantityDispatchMaterial,
                                        id_remisionGuide = detail.id_remisionGuide,
                                        RemissionGuide = db.RemissionGuide.Where(x => x.id == detail.id_remisionGuide).FirstOrDefault(),
                                        quantityOrdered = detail.quantityOrdered,
                                        quantityProgrammed = detail.quantityProgrammed,
                                        id_item = detail.id_item,
                                        isActive = detail.isActive,
                                        id_userCreate = detail.id_userCreate,
                                        dateCreate = detail.dateCreate,
                                        id_userUpdate = detail.id_userUpdate,
                                        dateUpdate = detail.dateUpdate
                                    };

                                    if (remissionGuideDetail.isActive)
                                    {
                                        modelItem.RemissionGuideRiverDetail.Add(remissionGuideDetail);
                                        count++;
                                    }
                                }
                                else
                                {
                                    remissionGuideDetail.id_item = detail.id_item;
                                    remissionGuideDetail.quantityDispatchMaterial = detail.quantityDispatchMaterial;
                                    remissionGuideDetail.quantityProgrammed = detail.quantityProgrammed;
                                    remissionGuideDetail.id_remisionGuide = detail.id_remisionGuide;
                                    remissionGuideDetail.RemissionGuide = db.RemissionGuide.Where(x => x.id == detail.id_remisionGuide).FirstOrDefault();
                                    remissionGuideDetail.isActive = detail.isActive;
                                    remissionGuideDetail.id_userCreate = detail.id_userCreate;
                                    remissionGuideDetail.dateCreate = detail.dateCreate;
                                    remissionGuideDetail.id_userUpdate = detail.id_userUpdate;
                                    remissionGuideDetail.dateUpdate = detail.dateUpdate;
                                    db.Entry(remissionGuideDetail).State = EntityState.Modified;
                                    if (remissionGuideDetail.isActive)
                                        count++;
                                }

                            }
                        }

                        if (count == 0)
                        {

                            TempData["remissionriverGuide"] = returnRemissionGuideRiver;
                            TempData.Keep("remissionriverGuide");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una guía de remisión sin detalles");
                            return PartialView("_RemissionGuideRiverMainFormPartial", returnRemissionGuideRiver);
                        }
                        else
                        {
                            //var id_provider = item.id_providerRemisionGuideRiver;
                            //var id_produccionunit = item.id_productionUnitProvider;
                            //var despachureDate = item.despachureDate.Date;
                            //var id_reason = item.id_reason;
                            //var id_shippingType = item.id_shippingType;

                            //var cnt = (from dre in modelItem.RemissionGuideRiverDetail
                            //           where dre.RemissionGuide.id_providerRemisionGuide != id_provider ||
                            //           dre.RemissionGuide.id_productionUnitProvider != id_produccionunit ||
                            //           dre.RemissionGuide.id_reason != id_reason ||
                            //            //dre.RemissionGuide.ProductionUnitProvider.id_shippingType != id_shippingType ||
                            //            dre.RemissionGuide.despachureDate.ToString("ddMMYYYY") != despachureDate.ToString("ddMMYYYY")
                            //           //  DbFunctions.TruncateTime(dre.RemissionGuide.despachureDate) != DbFunctions.TruncateTime(despachureDate)
                            //           select dre.id
                            //          ).ToList().Count();
                            //if (cnt > 0)
                            //{

                            //    TempData["remissionriverGuide"] = returnRemissionGuideRiver;
                            //    TempData.Keep("remissionriverGuide");
                            //    ViewData["EditMessage"] = ErrorMessage("No se puede guardar una guía de remisión hay detalles diferntes a los definidos");
                            //    return PartialView("_RemissionGuideRiverMainFormPartial", returnRemissionGuideRiver);
                            //}








                        }
                        #endregion

                        #region REMISSION GUIDE RIVER SECURITY SEALS

                        if (tempRemissionGuideRiver?.RemissionGuideRiverSecuritySeal != null)
                        {
                            var details = tempRemissionGuideRiver.RemissionGuideRiverSecuritySeal.ToList();

                            foreach (var detail in details)
                            {
                                RemissionGuideRiverSecuritySeal remissionGuideRiverSecuritySeal = modelItem.RemissionGuideRiverSecuritySeal.FirstOrDefault(d => d.id == detail.id);

                                if (remissionGuideRiverSecuritySeal == null)
                                {
                                    remissionGuideRiverSecuritySeal = new RemissionGuideRiverSecuritySeal
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

                                    if (remissionGuideRiverSecuritySeal.isActive)
                                    {
                                        modelItem.RemissionGuideRiverSecuritySeal.Add(remissionGuideRiverSecuritySeal);
                                    }
                                }
                                else
                                {
                                    remissionGuideRiverSecuritySeal.number = detail.number;

                                    remissionGuideRiverSecuritySeal.id_travelType = detail.id_travelType;
                                    remissionGuideRiverSecuritySeal.RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType);

                                    remissionGuideRiverSecuritySeal.id_exitState = detail.id_exitState;
                                    remissionGuideRiverSecuritySeal.SecuritySealState = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_exitState);
                                    remissionGuideRiverSecuritySeal.id_arrivalState = detail.id_arrivalState;
                                    remissionGuideRiverSecuritySeal.SecuritySealState1 = db.SecuritySealState.FirstOrDefault(s => s.id == detail.id_arrivalState);

                                    remissionGuideRiverSecuritySeal.isActive = detail.isActive;
                                    remissionGuideRiverSecuritySeal.id_userCreate = detail.id_userCreate;
                                    remissionGuideRiverSecuritySeal.dateCreate = detail.dateCreate;
                                    remissionGuideRiverSecuritySeal.id_userUpdate = detail.id_userUpdate;
                                    remissionGuideRiverSecuritySeal.dateUpdate = detail.dateUpdate;

                                }
                            }
                        }

                        #endregion

                        #region REMISSION GUIDE RIVER ASSIGNED STAFF

                        if (tempRemissionGuideRiver?.RemissionGuideRiverAssignedStaff != null)
                        {
                            var details = tempRemissionGuideRiver.RemissionGuideRiverAssignedStaff.ToList();

                            foreach (var detail in details)
                            {
                                RemissionGuideRiverAssignedStaff remissionGuideRiverAssignedStaff = modelItem.RemissionGuideRiverAssignedStaff.FirstOrDefault(d => d.id == detail.id);

                                if (remissionGuideRiverAssignedStaff == null)
                                {
                                    remissionGuideRiverAssignedStaff = new RemissionGuideRiverAssignedStaff
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

                                    if (remissionGuideRiverAssignedStaff.isActive)
                                    {
                                        modelItem.RemissionGuideRiverAssignedStaff.Add(remissionGuideRiverAssignedStaff);
                                    }
                                }
                                else
                                {
                                    remissionGuideRiverAssignedStaff.id_person = detail.id_person;
                                    remissionGuideRiverAssignedStaff.Person = db.Person.FirstOrDefault(p => p.id == detail.id_person);

                                    remissionGuideRiverAssignedStaff.id_assignedStaffRol = detail.id_assignedStaffRol;
                                    remissionGuideRiverAssignedStaff.RemissionGuideAssignedStaffRol = db.RemissionGuideAssignedStaffRol.FirstOrDefault(r => r.id == detail.id_assignedStaffRol);

                                    remissionGuideRiverAssignedStaff.id_travelType = detail.id_travelType;
                                    remissionGuideRiverAssignedStaff.RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == detail.id_travelType);

                                    remissionGuideRiverAssignedStaff.isActive = detail.isActive;
                                    remissionGuideRiverAssignedStaff.id_userCreate = detail.id_userCreate;
                                    remissionGuideRiverAssignedStaff.dateCreate = detail.dateCreate;
                                    remissionGuideRiverAssignedStaff.id_userUpdate = detail.id_userUpdate;
                                    remissionGuideRiverAssignedStaff.dateUpdate = detail.dateUpdate;
                                    remissionGuideRiverAssignedStaff.viaticPrice = detail.viaticPrice ?? 0;
                                }
                            }
                        }

                        #endregion

                        #region REMISSION GUIDE RIVER EXTRA DETAILS 
                        int id_rgTmp = 0;
                        int countRgrdm = 0;
                        tempRemissionGuideRiver
                            .RemissionGuideRiverDispatchMaterial = tempRemissionGuideRiver
                                                                    .RemissionGuideRiverDispatchMaterial ?? new List<RemissionGuideRiverDispatchMaterial>();
                        modelItem
                            .RemissionGuideRiverDispatchMaterial = modelItem
                                                                    .RemissionGuideRiverDispatchMaterial ?? new List<RemissionGuideRiverDispatchMaterial>();

                        if (tempRemissionGuideRiver.RemissionGuideRiverDispatchMaterial.Count > 0)
                        {
                            countRgrdm = tempRemissionGuideRiver.RemissionGuideRiverDispatchMaterial.Count;
                            for (int i = countRgrdm - 1; i >= 0; i--)
                            {
                                RemissionGuideRiverDispatchMaterial rgdT = tempRemissionGuideRiver
                                                                            .RemissionGuideRiverDispatchMaterial
                                                                            .ElementAt(i);

                                tempRemissionGuideRiver.RemissionGuideRiverDispatchMaterial.Remove(rgdT);

                            }
                        }

                        if (modelItem.RemissionGuideRiverDispatchMaterial.Count > 0)
                        {
                            countRgrdm = modelItem.RemissionGuideRiverDispatchMaterial.Count;
                            for (int i = countRgrdm - 1; i >= 0; i--)
                            {
                                RemissionGuideRiverDispatchMaterial rgdT = modelItem
                                                                            .RemissionGuideRiverDispatchMaterial
                                                                            .ElementAt(i);

                                tempRemissionGuideRiver.RemissionGuideRiverDispatchMaterial.Remove(rgdT);

                            }
                        }

                        if (tempRemissionGuideRiver != null && tempRemissionGuideRiver.RemissionGuideRiverDetail != null && tempRemissionGuideRiver.RemissionGuideRiverDetail.Count > 0)
                        {
                            var rgrdTmp = tempRemissionGuideRiver.RemissionGuideRiverDetail.Where(w => w.isActive).ToList();
                            foreach (var dTmp in rgrdTmp)
                            {
                                id_rgTmp = dTmp.id_remisionGuide;
                                if (id_rgTmp > 0)
                                {
                                    var rgdmLst = db.RemissionGuideDispatchMaterial
                                                    .Where(w => w.id_remisionGuide == id_rgTmp)
                                                    .ToList();

                                    foreach (var dTmp2 in rgdmLst)
                                    {
                                        RemissionGuideRiverDispatchMaterial rgrdmObj = tempRemissionGuideRiver
                                                                                        .RemissionGuideRiverDispatchMaterial
                                                                                        .FirstOrDefault(fod => fod.id_item == dTmp2.id_item);
                                        if (rgrdmObj == null)
                                        {
                                            rgrdmObj = new RemissionGuideRiverDispatchMaterial();
                                            rgrdmObj.id_item = dTmp2.id_item;
                                            rgrdmObj.Item = db.Item.FirstOrDefault(fod => fod.id == dTmp2.id_item);
                                            rgrdmObj.id_warehouse = dTmp2.id_warehouse;
                                            rgrdmObj.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == dTmp2.id_warehouse);
                                            rgrdmObj.id_warehouselocation = (int)dTmp2.id_warehouselocation;
                                            rgrdmObj.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == dTmp2.id_warehouselocation);
                                            rgrdmObj.sourceExitQuantity = dTmp2.sourceExitQuantity;
                                            rgrdmObj.sendedDestinationQuantity = dTmp2.sendedDestinationQuantity;
                                            tempRemissionGuideRiver.RemissionGuideRiverDispatchMaterial.Add(rgrdmObj);
                                        }
                                        else
                                        {
                                            rgrdmObj.Item = db.Item.FirstOrDefault(fod => fod.id == dTmp2.id_item);
                                            rgrdmObj.id_warehouse = dTmp2.id_warehouse;
                                            rgrdmObj.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == dTmp2.id_warehouse);
                                            rgrdmObj.id_warehouselocation = (int)dTmp2.id_warehouselocation;
                                            rgrdmObj.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == dTmp2.id_warehouselocation);
                                            rgrdmObj.sourceExitQuantity += dTmp2.sourceExitQuantity;
                                            rgrdmObj.sendedDestinationQuantity += dTmp2.sendedDestinationQuantity;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion


                        #region REMISSION GUIDE RIVER DISPATCH MATERIALS

                        if (tempRemissionGuideRiver?.RemissionGuideRiverDispatchMaterial != null)
                        {
                            var details = tempRemissionGuideRiver.RemissionGuideRiverDispatchMaterial.ToList();

                            foreach (var detail in details)
                            {
                                RemissionGuideRiverDispatchMaterial remissionGuideRiverDispatchMaterial = modelItem
                                                                                                            .RemissionGuideRiverDispatchMaterial
                                                                                                            .FirstOrDefault(d => d.id_item == detail.id_item);

                                if (remissionGuideRiverDispatchMaterial == null)
                                {
                                    remissionGuideRiverDispatchMaterial = new RemissionGuideRiverDispatchMaterial
                                    {
                                        id_item = detail.id_item,
                                        id_warehouse = detail.id_warehouse,
                                        id_warehouselocation = (int)detail.id_warehouselocation,
                                        sourceExitQuantity = detail.sourceExitQuantity,
                                        sendedDestinationQuantity = detail.sendedDestinationQuantity
                                    };
                                    modelItem.RemissionGuideRiverDispatchMaterial.Add(remissionGuideRiverDispatchMaterial);
                                }
                                else
                                {
                                    remissionGuideRiverDispatchMaterial.id_item = detail.id_item;
                                    remissionGuideRiverDispatchMaterial.id_warehouse = detail.id_warehouse;
                                    remissionGuideRiverDispatchMaterial.id_warehouselocation = (int)detail.id_warehouselocation;
                                    remissionGuideRiverDispatchMaterial.sourceExitQuantity = detail.sourceExitQuantity;
                                    remissionGuideRiverDispatchMaterial.sendedDestinationQuantity = detail.sendedDestinationQuantity;
                                }
                            }
                        }

                        #endregion

                        #region"DATA HELP FOR REMISSION GUIDE REPORT"

                        modelItem.RemissionGuideRiverReportLinealDataHelp = modelItem.RemissionGuideRiverReportLinealDataHelp ?? new RemissionGuideRiverReportLinealDataHelp();

                        //Consulto Vehiculo Hunter
                        if (modelItem.RemissionGuideRiverTransportation != null)
                        {
                            int id_vehTmp = modelItem.RemissionGuideRiverTransportation.id_vehicle ?? 0;
                            if (id_vehTmp > 0)
                            {
                                hasHDTmp = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehTmp)?.hasHunterDevice ?? false;
                                if (hasHDTmp)
                                {
                                    modelItem.RemissionGuideRiverReportLinealDataHelp.nameSecurityPerson = db.Setting.FirstOrDefault(fod => fod.code == "NDHL")?.value ?? "";
                                }
                            }
                        }
                        //Person Asignado
                        int id_SegRol = db.RemissionGuideAssignedStaffRol
                                            .FirstOrDefault(fod => fod.code == "SEG")?.id ?? 0;
                        int id_SegTec = db.RemissionGuideAssignedStaffRol
                                            .FirstOrDefault(fod => fod.code == "BIO")?.id ?? 0;


                        int id_rgasSeg = modelItem
                                        .RemissionGuideRiverAssignedStaff
                                        .FirstOrDefault(fod => fod.id_assignedStaffRol == id_SegRol)?.id_person ?? 0;

                        int id_rgasTec = modelItem
                                            .RemissionGuideRiverAssignedStaff
                                            .FirstOrDefault(fod => fod.id_assignedStaffRol == id_SegTec)?.id_person ?? 0;
                        //SEGURIDAD
                        if (id_rgasSeg > 0)
                        {
                            modelItem.RemissionGuideRiverReportLinealDataHelp.id_SecurityPerson = id_rgasSeg;
                            modelItem.RemissionGuideRiverReportLinealDataHelp.nameSecurityPerson += (hasHDTmp ? "/" : "") + db.Person.FirstOrDefault(fod => fod.id == id_rgasSeg)?.fullname_businessName ?? "";
                        }
                        //BIOLOGO
                        if (id_rgasTec > 0)
                        {
                            modelItem.RemissionGuideRiverReportLinealDataHelp.id_TechnicianPerson = id_rgasTec;
                            modelItem
                                .RemissionGuideRiverReportLinealDataHelp
                                .nameTechnicianPerson = db.Person.FirstOrDefault(fod => fod.id == id_rgasTec)?.fullname_businessName ?? "";
                        }
                        //SELLO DE SEGURIDAD
                        int str_SellOut = db.RemissionGuideTravelType
                                            .FirstOrDefault(fod => fod.code == "IDA")?.id ?? 0;
                        int str_SellIn = db.RemissionGuideTravelType
                                            .FirstOrDefault(fod => fod.code == "REGRESO")?.id ?? 0;

                        modelItem
                            .RemissionGuideRiverReportLinealDataHelp
                            .SealNumberOneExit = modelItem.RemissionGuideRiverSecuritySeal.FirstOrDefault(fod => fod.id_travelType == str_SellOut)?.number ?? "";

                        modelItem
                            .RemissionGuideRiverReportLinealDataHelp
                            .SealNumberTwoEntrance = modelItem.RemissionGuideRiverSecuritySeal.FirstOrDefault(fod => fod.id_travelType == str_SellIn)?.number ?? "";
                        #endregion

                        #region PERSONALIZATION DETAIL BY DOCUMENT TYPE
                        List<tbsysDocumentsPersonalizationDetail> lstDocPersDetail = db.tbsysDocumentsPersonalizationDetail
                                                                                        .Where(fod => fod.id_DocumentType == modelItem.Document.id_documentType)
                                                                                        .ToList();

                        if (lstDocPersDetail != null && lstDocPersDetail.Count > 0)
                        {

                            foreach (var det in lstDocPersDetail)
                            {
                                #region INFORMACION CUSTOMIZABLE VIATICO PERSONAL ASIGNADO
                                if (det.nameObjectTable == "RemissionGuideRiverCustomizedViaticPersonalAssigned")
                                {
                                    int iQuantityPersonalAssigned = modelItem?.RemissionGuideRiverAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).ToList().Count() ?? 0;
                                    if (iQuantityPersonalAssigned > 0)
                                    {
                                        if (modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned != null)
                                        {
                                            modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned.Document.id_userUpdate = ActiveUser.id;
                                            modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned.Document.dateUpdate = DateTime.Now;
                                            modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned.isActive = true;
                                            modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned.valueTotal = modelItem?.RemissionGuideRiverAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() ?? 0;
                                        }
                                        else
                                        {
                                            //modelItem.RemissionGuideCustomizedViaticPersonalAssigned = modelItem.RemissionGuideCustomizedViaticPersonalAssigned ?? new RemissionGuideCustomizedViaticPersonalAssigned();
                                            RemissionGuideRiverCustomizedViaticPersonalAssigned rgcvpa = new RemissionGuideRiverCustomizedViaticPersonalAssigned();

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

                                            //Actualiza Secuencial Viatico Personal Asignado
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
                                            rgcvpa.id_RemissionGuideRiver = modelItem.id;
                                            rgcvpa.valueTotal = modelItem?.RemissionGuideRiverAssignedStaff?.Where(w => w.isActive && w.viaticPrice > 0).Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() ?? 0;
                                            db.RemissionGuideRiverCustomizedViaticPersonalAssigned.Add(rgcvpa);
                                        }
                                    }
                                    else
                                    {
                                        if (modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned != null)
                                        {
                                            modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned.Document.id_userUpdate = ActiveUser.id;
                                            modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned.Document.dateUpdate = DateTime.Now;
                                            modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned.isActive = false;
                                            modelItem.RemissionGuideRiverCustomizedViaticPersonalAssigned.valueTotal = 0;
                                        }
                                    }
                                }
                                #endregion

                                #region INFORMACION CUSTOMIZABLE ANTICIPO A TRANSPORTISTA
                                if (det.nameObjectTable == "RemissionGuideRiverCustomizedAdvancedTransportist")
                                {
                                    if (modelItem.RemissionGuideRiverTransportation?.advancePrice > 0)
                                    {
                                        if (modelItem.RemissionGuideRiverCustomizedAdvancedTransportist != null)
                                        {
                                            modelItem.RemissionGuideRiverCustomizedAdvancedTransportist.Document.id_userUpdate = ActiveUser.id;
                                            modelItem.RemissionGuideRiverCustomizedAdvancedTransportist.Document.dateUpdate = DateTime.Now;
                                            modelItem.RemissionGuideRiverCustomizedAdvancedTransportist.isActive = true;
                                            modelItem.RemissionGuideRiverCustomizedAdvancedTransportist.valueTotal = modelItem.RemissionGuideRiverTransportation?.advancePrice ?? 0;

                                        }
                                        else
                                        {
                                            RemissionGuideRiverCustomizedAdvancedTransportist rgcat = new RemissionGuideRiverCustomizedAdvancedTransportist();
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
                                            rgcat.id_RemissionGuideRiver = modelItem.id;
                                            rgcat.valueTotal = modelItem.RemissionGuideRiverTransportation?.advancePrice ?? 0;
                                            db.RemissionGuideRiverCustomizedAdvancedTransportist.Add(rgcat);
                                        }
                                    }
                                    else
                                    {
                                        if (modelItem.RemissionGuideRiverCustomizedAdvancedTransportist != null)
                                        {
                                            modelItem.RemissionGuideRiverCustomizedAdvancedTransportist.Document.id_userUpdate = ActiveUser.id;
                                            modelItem.RemissionGuideRiverCustomizedAdvancedTransportist.Document.dateUpdate = DateTime.Now;
                                            modelItem.RemissionGuideRiverCustomizedAdvancedTransportist.isActive = false;
                                            modelItem.RemissionGuideRiverCustomizedAdvancedTransportist.valueTotal = 0;
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion

                        if (approve)
                        {
                            modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        }
                        db.RemissionGuideRiver.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();
                        modelItem.getLiquidationInformation();
                        if (transportation != null)
                        {
                            transportation.Vehicle = db.Vehicle.Where(i => i.id == transportation.id_vehicle).FirstOrDefault();
                            modelItem.RemissionGuideRiverTransportation = transportation;
                        }

                        TempData["remissionriverGuide"] = modelItem;
                        TempData.Keep("remissionriverGuide");
                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + modelItem.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("remissionriverGuide");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }
            TempData.Keep("remissionriverGuide");
            return PartialView("_RemissionGuideRiverMainFormPartial", modelItem);
        }

        #endregion

        #region REMISSION GUIDE TERRESTREL RESULTS
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideForRiverResults()
        {

            List<int> _lstRg = db.RemissionGuideRiverDetail
                                    .Where(w => w.RemissionGuideRiver.Document.DocumentState.code != "05")
                                    .Select(s => s.id_remisionGuide)
                                    .ToList();
            db.Database.CommandTimeout = 1200;
            List<RemGuideResultsForRiver> modelTmp = db.RemissionGuide
                                                    .Where(w => ((w.PurchaseOrderShippingType.code == "TF"
                                                            && w.RemissionGuideTransportation.isOwn == false) ||
                                                            (w.PurchaseOrderShippingType.code == "T"
                                                            && w.RemissionGuideTransportation.isOwn == true))
                                                            && (w.Document.DocumentState.code == "03" || w.Document.DocumentState.code == "06" || w.Document.DocumentState.code == "09")
                                                            && !_lstRg.Contains(w.id))
                                                            .Select(s => new RemGuideResultsForRiver
                                                            {
                                                                id = s.id,
                                                                numberRG = s.Document.number,
                                                                id_personProcessPlant = s.id_personProcessPlant,
                                                                personProcessPlant = db.Person.FirstOrDefault(p => p.id == s.id_personProcessPlant).processPlant,
                                                                nameProvider = db.Person.FirstOrDefault(fod => fod.id == s.id_providerRemisionGuide).fullname_businessName,
                                                                namerProductionUnitProvider = s.ProductionUnitProvider.name,
                                                                requiredLogistic = s.RemissionGuideDetail.FirstOrDefault() != null ? (s.RemissionGuideDetail.FirstOrDefault().RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault() != null ? s.RemissionGuideDetail.FirstOrDefault().RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault().PurchaseOrderDetail.PurchaseOrder.requiredLogistic
                                                                                                                                                                                                                                               : false)
                                                                                                                                   : false,
                                                                despachureDate = s.despachureDate,
                                                                nameItem = s.RemissionGuideDetail.Select(w => w.Item.name).FirstOrDefault(),
                                                                emissionDate = s.Document.emissionDate,
                                                                nameZone = s.RemissionGuideTransportation.FishingSite.FishingZone.name ?? "",
                                                                nameSite = s.RemissionGuideTransportation.FishingSite.name ?? "",
                                                                daysDiff = DbFunctions.DiffDays(s.Document.emissionDate, DateTime.Now) ?? 0
                                                            })
                                                            .ToList();
            List<RemGuideResultsForRiver> model = modelTmp
                                                    .Where(w => w.daysDiff <= 3).ToList();
            TempData["modelRG"] = model;
            TempData.Keep("modelRG");
            return PartialView("_RemissionGuideForRiverResultsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideForRiverPartial()
        {
            List<RemGuideResultsForRiver> model = (TempData["modelRG"] as List<RemGuideResultsForRiver>);

            model = model ?? new List<RemGuideResultsForRiver>();

            TempData["modelRG"] = model;
            TempData.Keep("modelRG");

            return PartialView("_RemissionGuideForRiverPartial", model.ToList());
        }
        #endregion

        #region REMISSION GUIDE EDITFORM
        public void LoadpartialComboxTransportTariff(int? id_shippingType, int? id_TransportTariffType)
        {
            try
            {
                if (id_shippingType != null && id_shippingType > 0)
                {


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
            catch //(Exception ex)
            {


            }

        }
        public void LoadpartialComboxTransportTariffVehicleType(int? id_TransportTariffType, int? id_vehicle)
        {
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
                        //var VehicleCurrentAux = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehicle);
                        var VehicleCurrentAux = db.Vehicle.FirstOrDefault(fod => fod.id == id_vehicle && listVehicleType.Contains(fod.id_VehicleType));

                        if (VehicleCurrentAux != null && !Vehicle.Contains(VehicleCurrentAux)) Vehicle.Add(VehicleCurrentAux);
                    }

                    TempData["TransportTariffTypeVehicleType"] = Vehicle.OrderBy(t => t.id).ToList();
                    TempData.Keep("TransportTariffTypeVehicleType");



                }
            }
            catch //(Exception ex)
            {


            }


        }

        [HttpPost]
        public JsonResult ValidateSelectedRowsRemissionGuide(int[] ids)
        {
            var result = new
            {
                Message = "OK"
            };

            Provider providerFirst = null;
            Provider providerCurrent = null;
            Person personProcessPlantFirst = null;
            Person personProcessPlantCurrent = null;
            ProductionUnitProvider productionUnitProviderFirst = null;
            ProductionUnitProvider productionUnitProviderCurrent = null;
            DateTime _dtFirst = new DateTime();
            DateTime _dtCurrent = new DateTime();

            var minEmissionDate = DateTime.MaxValue;
            var maxEmissionDate = DateTime.MinValue;
            var maxValidEmissionDate = DateTime.MinValue;

            string _setPUPname = db.Setting.FirstOrDefault(fod => fod.code == "EUPPPRIM").value ?? "";

            string codeState = "";
            string nameState = "";

            int count = 0;
            foreach (var i in ids)
            {
                var remissionGuideTemp = db.RemissionGuide
                    .FirstOrDefault(fod => fod.id == i);
                var remissionGuideDocument = remissionGuideTemp?.Document;

                codeState = remissionGuideDocument?.DocumentState?.code ?? "";
                nameState = remissionGuideDocument?.DocumentState?.name ?? "";

                if (!(codeState == "03" || codeState == "06"))
                {
                    result = new
                    {
                        Message = ErrorMessage("La Guía de Remisión debe estar en estado Aprobado y actualmente se encuentra en estado " + nameState + ".")
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                providerCurrent = db.Provider.FirstOrDefault(fod => fod.id == remissionGuideTemp.id_providerRemisionGuide);
                personProcessPlantCurrent = db.Person.FirstOrDefault(p => p.id == remissionGuideTemp.id_personProcessPlant);
                productionUnitProviderCurrent = remissionGuideTemp.ProductionUnitProvider;
                _dtCurrent = remissionGuideTemp.despachureDate;

                if (count == 0)
                {
                    providerFirst = providerCurrent;
                    personProcessPlantFirst = personProcessPlantCurrent;
                    productionUnitProviderFirst = productionUnitProviderCurrent;
                    _dtFirst = _dtCurrent;
                }
                if (providerCurrent != providerFirst)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar Guías con proveedores diferentes.")
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (personProcessPlantCurrent != personProcessPlantFirst)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar Guías con procesos diferentes.")
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //if (productionUnitProviderCurrent != productionUnitProviderFirst)
                //{
                //    result = new
                //    {
                //        Message = ErrorMessage("No se pueden mezclar Guías con " + ((_setPUPname != "") ? _setPUPname + "s": "Unidad de Producción" ) + " diferentes.")
                //    };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                if (DateTime.Compare(_dtFirst.Date, _dtCurrent.Date) != 0)
                {
                    result = new
                    {
                        Message = ErrorMessage("No se pueden mezclar Guías con Fechas de Despacho diferentes.")
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                // Verificación de los rangos de fechas de emisión
                var remissionGuideEmissionDate = remissionGuideDocument.emissionDate.Date;

                if (minEmissionDate > remissionGuideEmissionDate)
                {
                    minEmissionDate = remissionGuideEmissionDate;
                    maxValidEmissionDate = minEmissionDate.AddDays(1);
                }
                if (maxEmissionDate < remissionGuideEmissionDate)
                {
                    maxEmissionDate = remissionGuideEmissionDate;
                }

                if (maxEmissionDate > maxValidEmissionDate)
                {
                    result = new
                    {
                        Message = ErrorMessage($"La fecha de emisión máxima permitida es {maxValidEmissionDate:dd/MM/yyyy}."),
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                count++;
            }

            //TempData.Keep("remissionGuide");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuideRiver(int id, int[] remissionGuides)
        {
            RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(o => o.id == id);

            if (remissionGuide == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("69"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");
                PurchaseOrderShippingType shippingType = db.PurchaseOrderShippingType.Where(d => d.code == "M").FirstOrDefault();

                remissionGuide = new RemissionGuideRiver
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
                    startAdress = ActiveSucursal.address,
                    RemissionGuideRiverDetail = new List<RemissionGuideRiverDetail>(),
                    id_shippingType = shippingType.id,
                    PurchaseOrderShippingType = shippingType
                };
                remissionGuide.RemissionGuideRiverTransportation = new RemissionGuideRiverTransportation();
                LoadpartialComboxTransportTariff(remissionGuide.id_shippingType, remissionGuide.id_TransportTariffType);
            }
            else
            {
                remissionGuide.getLiquidationInformation();
                TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, remissionGuide.id_providerRemisionGuideRiver);
                TempData.Keep("ProductionUnitProviderByProvider");
                TempData["ProductionUnitProviderPoolByUnitProvider"] = DataProviders.DataProviderProductionUnitProviderPool.ProductionUnitProviderPoolByUnitProvider(null, remissionGuide.id_productionUnitProvider);
                TempData.Keep("ProductionUnitProviderPoolByUnitProvider");
                LoadpartialComboxTransportTariff(remissionGuide.id_shippingType, remissionGuide.id_TransportTariffType);
                LoadpartialComboxTransportTariffVehicleType(remissionGuide.id_TransportTariffType, remissionGuide?.RemissionGuideRiverTransportation?.id_vehicle);
            }
            if (remissionGuides != null && remissionGuides.Count() > 0)
            {
                int _irg = remissionGuides[0];
                int id_item = 0;
                if (_irg > 0)
                {
                    remissionGuide.RemissionGuideRiverDetail = new List<RemissionGuideRiverDetail>();
                    remissionGuide.RemissionGuideRiverDispatchMaterial = new List<RemissionGuideRiverDispatchMaterial>();

                    RemissionGuide _RgTmp = db.RemissionGuide.FirstOrDefault(fod => fod.id == _irg);
                    if (_RgTmp != null)
                    {
                        //Inserto Proveedor
                        remissionGuide.id_providerRemisionGuideRiver = _RgTmp.id_providerRemisionGuide;
                        //Inserto Unidad de Producción
                        remissionGuide.id_productionUnitProvider = _RgTmp.id_productionUnitProvider;
                        remissionGuide.ProductionUnitProvider = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == _RgTmp.id_productionUnitProvider);
                        //Inserto Motivo de Compra
                        remissionGuide.id_reason = _RgTmp.id_reason;
                        //Transporte
                        remissionGuide.RemissionGuideRiverTransportation = new RemissionGuideRiverTransportation();
                        remissionGuide.RemissionGuideRiverTransportation.id_FishingSiteRGR = _RgTmp.RemissionGuideTransportation.id_FishingSiteRG;
                        FishingSite _fs = db.FishingSite.FirstOrDefault(fod => fod.id == _RgTmp.RemissionGuideTransportation.id_FishingSiteRG);
                        remissionGuide.RemissionGuideRiverTransportation.FishingSite = _fs;
                        remissionGuide.RemissionGuideRiverTransportation.id_FishingZoneRGRNew = _fs.id_FishingZone;

                        //Inserto requiredLogistic
                        remissionGuide.requiredLogistic = _RgTmp.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

                        //Inserto Person Process Plant
                        remissionGuide.id_personProcessPlant = _RgTmp.id_personProcessPlant;

                        remissionGuide.route = _RgTmp.route ?? "";
                        //Inserto Detalle
                        foreach (int i in remissionGuides)
                        {
                            RemissionGuide _rgTmpDet = db.RemissionGuide.FirstOrDefault(fod => fod.id == i);

                            RemissionGuideRiverDetail _rgRd = new RemissionGuideRiverDetail();

                            id_item = _rgTmpDet.RemissionGuideDetail.Select(s => s.id_item).FirstOrDefault();

                            _rgRd.id_item = id_item;
                            _rgRd.Item = db.Item.FirstOrDefault(fod => fod.id == id_item);
                            _rgRd.id_remisionGuide = i;
                            _rgRd.RemissionGuide = _rgTmpDet;
                            _rgRd.quantityProgrammed = _rgTmpDet.RemissionGuideDetail.Select(s => s.quantityProgrammed).DefaultIfEmpty(0).Sum();
                            _rgRd.quantityOrdered = _rgTmpDet.RemissionGuideDetail.Select(s => s.quantityOrdered).DefaultIfEmpty(0).Sum();
                            _rgRd.isActive = true;
                            _rgRd.id_userCreate = ActiveUser.id;
                            _rgRd.dateCreate = DateTime.Now;
                            _rgRd.id_userUpdate = ActiveUser.id;
                            _rgRd.dateUpdate = DateTime.Now;

                            remissionGuide.RemissionGuideRiverDetail.Add(_rgRd);


                        }
                        int id_rgTmp = 0;
                        if (remissionGuide != null && remissionGuide.RemissionGuideRiverDetail != null && remissionGuide.RemissionGuideRiverDetail.Count > 0)
                        {
                            var rgrdTmp = remissionGuide.RemissionGuideRiverDetail.Where(w => w.isActive).ToList();
                            foreach (var dTmp in rgrdTmp)
                            {
                                id_rgTmp = dTmp.id_remisionGuide;
                                if (id_rgTmp > 0)
                                {
                                    var rgdmLst = db.RemissionGuideDispatchMaterial
                                                    .Where(w => w.id_remisionGuide == id_rgTmp)
                                                    .ToList();

                                    foreach (var dTmp2 in rgdmLst)
                                    {
                                        RemissionGuideRiverDispatchMaterial rgrdmObj = remissionGuide
                                                                                        .RemissionGuideRiverDispatchMaterial
                                                                                        .FirstOrDefault(fod => fod.id_item == dTmp2.id_item);
                                        if (rgrdmObj == null)
                                        {
                                            rgrdmObj = new RemissionGuideRiverDispatchMaterial();
                                            rgrdmObj.id_item = dTmp2.id_item;
                                            rgrdmObj.Item = db.Item.FirstOrDefault(fod => fod.id == dTmp2.id_item);
                                            rgrdmObj.id_warehouse = dTmp2.id_warehouse;
                                            rgrdmObj.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == dTmp2.id_warehouse);
                                            rgrdmObj.id_warehouselocation = (int)dTmp2.id_warehouselocation;
                                            rgrdmObj.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == dTmp2.id_warehouselocation);
                                            rgrdmObj.sourceExitQuantity = dTmp2.sourceExitQuantity;
                                            rgrdmObj.sendedDestinationQuantity = dTmp2.sendedDestinationQuantity;
                                            remissionGuide.RemissionGuideRiverDispatchMaterial.Add(rgrdmObj);
                                        }
                                        else
                                        {
                                            rgrdmObj.Item = db.Item.FirstOrDefault(fod => fod.id == dTmp2.id_item);
                                            rgrdmObj.id_warehouse = dTmp2.id_warehouse;
                                            rgrdmObj.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == dTmp2.id_warehouse);
                                            rgrdmObj.id_warehouselocation = (int)dTmp2.id_warehouselocation;
                                            rgrdmObj.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == dTmp2.id_warehouselocation);
                                            rgrdmObj.sourceExitQuantity += dTmp2.sourceExitQuantity;
                                            rgrdmObj.sendedDestinationQuantity += dTmp2.sendedDestinationQuantity;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            TempData["remissionriverGuide"] = remissionGuide;
            TempData.Keep("remissionriverGuide");
            return PartialView("_FormEditRemissionGuideRiver", remissionGuide);
        }
        #endregion

        #region REMISSION GUIDE SECURITY SEALS

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverSecuritySealsPartial()
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();
            remissionGuideRiver.RemissionGuideRiverSecuritySeal = remissionGuideRiver.RemissionGuideRiverSecuritySeal ?? new List<RemissionGuideRiverSecuritySeal>();

            var model = remissionGuideRiver.RemissionGuideRiverSecuritySeal.Where(d => d.isActive);

            TempData.Keep("remissionriverGuide");

            return PartialView("_SecuritySealsRiver", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverSecuritySealsPartialAddNew(RemissionGuideRiverSecuritySeal remissionGuideRiverSecuritySeal)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);

            remissionGuideRiver = remissionGuideRiver ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuideRiver.id);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            if (ModelState.IsValid)
            {
                try
                {

                    remissionGuideRiverSecuritySeal.id = remissionGuideRiver.RemissionGuideRiverSecuritySeal.Count() > 0 ? remissionGuideRiver.RemissionGuideRiverSecuritySeal.Max(pld => pld.id) + 1 : 1;

                    remissionGuideRiverSecuritySeal.isActive = true;
                    remissionGuideRiverSecuritySeal.id_userCreate = ActiveUser.id;
                    remissionGuideRiverSecuritySeal.dateCreate = DateTime.Now;
                    remissionGuideRiverSecuritySeal.id_userUpdate = ActiveUser.id;
                    remissionGuideRiverSecuritySeal.dateUpdate = DateTime.Now;

                    remissionGuideRiver.RemissionGuideRiverSecuritySeal.Add(remissionGuideRiverSecuritySeal);

                    TempData["remissionriverGuide"] = remissionGuideRiver;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("remissionriverGuide");

            var model = remissionGuideRiver?.RemissionGuideRiverSecuritySeal.Where(d => d.isActive).ToList() ?? new List<RemissionGuideRiverSecuritySeal>();
            return PartialView("_SecuritySealsRiver", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverSecuritySealsPartialUpdate(RemissionGuideRiverSecuritySeal remissionGuideRiverSecuritySeal)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);

            remissionGuideRiver = remissionGuideRiver ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuideRiver.id);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = remissionGuideRiver.RemissionGuideRiverSecuritySeal.FirstOrDefault(it => it.number.Equals(remissionGuideRiverSecuritySeal.number));
                    if (modelItem != null)
                    {
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        TempData["remissionriverGuide"] = remissionGuideRiver;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("remissionriverGuide");

            var model = remissionGuideRiver?.RemissionGuideRiverSecuritySeal.Where(d => d.isActive).ToList() ?? new List<RemissionGuideRiverSecuritySeal>();

            return PartialView("_SecuritySealsRiver", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverSecuritySealsPartialDelete(string number)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);

            remissionGuideRiver = remissionGuideRiver ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuideRiver.id);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            if (!string.IsNullOrEmpty(number))
            {
                try
                {
                    var remissionSecuritySeal = remissionGuideRiver.RemissionGuideRiverSecuritySeal.FirstOrDefault(p => p.number.Equals(number));
                    if (remissionSecuritySeal != null)
                    {
                        remissionSecuritySeal.isActive = false;
                        remissionSecuritySeal.id_userUpdate = ActiveUser.id;
                        remissionSecuritySeal.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionriverGuide"] = remissionGuideRiver;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionriverGuide");

            var model = remissionGuideRiver?.RemissionGuideRiverSecuritySeal.Where(d => d.isActive).ToList() ?? new List<RemissionGuideRiverSecuritySeal>();
            return PartialView("_SecuritySealsRiver", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedRemissionGuideRiverSecuritySeals(string[] ids)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuideRiver.id);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            if (ids != null)
            {
                try
                {
                    var remissionGuideRiverSecuritySeals = remissionGuideRiver.RemissionGuideRiverSecuritySeal.Where(i => ids.Contains(i.number));

                    foreach (var detail in remissionGuideRiverSecuritySeals)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionriverGuide"] = remissionGuideRiver;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionriverGuide");
        }

        #endregion

        #region REMISSION GUIDE ASSIGNED STAFF

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverAssignedStaffPartial()
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();
            remissionGuideRiver.RemissionGuideRiverAssignedStaff = remissionGuideRiver.RemissionGuideRiverAssignedStaff ?? new List<RemissionGuideRiverAssignedStaff>();

            var model = remissionGuideRiver.RemissionGuideRiverAssignedStaff.Where(d => d.isActive);

            TempData.Keep("remissionriverGuide");

            return PartialView("_AssignedStaffRiver", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverAssignedStaffPartialAddNew(RemissionGuideRiverAssignedStaff remissionGuideRiverAssignedStaff)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);

            remissionGuideRiver = remissionGuideRiver ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuideRiver.id);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            if (ModelState.IsValid)
            {
                try
                {
                    remissionGuideRiverAssignedStaff.id = remissionGuideRiver.RemissionGuideRiverAssignedStaff.Count() > 0 ? remissionGuideRiver.RemissionGuideRiverAssignedStaff.Max(pld => pld.id) + 1 : 1;
                    remissionGuideRiverAssignedStaff.Person = db.Person.FirstOrDefault(p => p.id == remissionGuideRiverAssignedStaff.id_person);
                    remissionGuideRiverAssignedStaff.RemissionGuideAssignedStaffRol = db.RemissionGuideAssignedStaffRol.FirstOrDefault(r => r.id == remissionGuideRiverAssignedStaff.id_assignedStaffRol);
                    remissionGuideRiverAssignedStaff.RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(t => t.id == remissionGuideRiverAssignedStaff.id_travelType);

                    remissionGuideRiverAssignedStaff.isActive = true;
                    remissionGuideRiverAssignedStaff.id_userCreate = ActiveUser.id;
                    remissionGuideRiverAssignedStaff.dateCreate = DateTime.Now;
                    remissionGuideRiverAssignedStaff.id_userUpdate = ActiveUser.id;
                    remissionGuideRiverAssignedStaff.dateUpdate = DateTime.Now;

                    remissionGuideRiver.RemissionGuideRiverAssignedStaff.Add(remissionGuideRiverAssignedStaff);

                    TempData["remissionriverGuide"] = remissionGuideRiver;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("remissionriverGuide");

            var model = remissionGuideRiver?.RemissionGuideRiverAssignedStaff.Where(d => d.isActive).ToList() ?? new List<RemissionGuideRiverAssignedStaff>();
            return PartialView("_AssignedStaffRiver", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverAssignedStaffPartialUpdate(RemissionGuideRiverAssignedStaff remissionGuideRiverAssignedStaff)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionGuide"] as RemissionGuideRiver);

            remissionGuideRiver = remissionGuideRiver ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuideRiver.id);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = remissionGuideRiver.RemissionGuideRiverAssignedStaff.FirstOrDefault(r => r.id_person == remissionGuideRiverAssignedStaff.id_person);
                    if (modelItem != null)
                    {
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        TempData["remissionriverGuide"] = remissionGuideRiver;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("remissionriverGuide");

            var model = remissionGuideRiver?.RemissionGuideRiverAssignedStaff.Where(d => d.isActive).ToList() ?? new List<RemissionGuideRiverAssignedStaff>();

            return PartialView("_AssignedStaffRiver", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverAssignedStaffPartialDelete(System.Int32 id_person)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);

            remissionGuideRiver = remissionGuideRiver ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuideRiver.id);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            if (id_person >= 0)
            {
                try
                {
                    var remissionGuideRiverAssignedStaff = remissionGuideRiver.RemissionGuideRiverAssignedStaff.FirstOrDefault(p => p.id_person == id_person);
                    if (remissionGuideRiverAssignedStaff != null)
                    {
                        remissionGuideRiverAssignedStaff.isActive = false;
                        remissionGuideRiverAssignedStaff.id_userUpdate = ActiveUser.id;
                        remissionGuideRiverAssignedStaff.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionriverGuide"] = remissionGuideRiver;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionriverGuide");

            var model = remissionGuideRiver?.RemissionGuideRiverAssignedStaff.Where(d => d.isActive).ToList() ?? new List<RemissionGuideRiverAssignedStaff>();
            return PartialView("_AssignedStaffRiver", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedRemissionGuideRiverAssignedStaff(int[] ids)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuideRiver.id);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            if (ids != null)
            {
                try
                {
                    var remissionGuideRiverAssignedStaff = remissionGuideRiver.RemissionGuideRiverAssignedStaff.Where(i => ids.Contains(i.id_person));

                    foreach (var detail in remissionGuideRiverAssignedStaff)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionriverGuide"] = remissionGuideRiver;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionriverGuide");
        }

        #endregion

        #region REMISSION GUIDE DETAILS

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverDetailsPartial()
        {
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuide = remissionGuide ?? new RemissionGuideRiver();
            remissionGuide.RemissionGuideRiverDetail = remissionGuide.RemissionGuideRiverDetail ?? new List<RemissionGuideRiverDetail>();

            var model = remissionGuide.RemissionGuideRiverDetail.Where(d => d.isActive);

            TempData.Keep("remissionriverGuide");

            return PartialView("_Details", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverDetailsPartialAddNew(RemissionGuideRiverDetail remissionGuideDetail)
        {
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);

            remissionGuide = remissionGuide ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuideRiver();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = remissionGuide.RemissionGuideRiverDetail.FirstOrDefault(it => (!it.isActive) &&
                                                                                               it.id_remisionGuide == remissionGuideDetail.id_remisionGuide);
                    if (modelItem != null)
                    {
                        modelItem.isActive = true;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.quantityDispatchMaterial = remissionGuideDetail.quantityDispatchMaterial;
                        modelItem.quantityOrdered = remissionGuideDetail.quantityOrdered;
                        modelItem.quantityProgrammed = remissionGuideDetail.quantityProgrammed;
                        this.UpdateModel(modelItem);
                        modelItem.RemissionGuide = db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuideDetail.id_remisionGuide);
                    }
                    else
                    {
                        //remissionGuideDetail.Item = db.Item.FirstOrDefault(i => i.id == remissionGuideDetail.id_item);
                        remissionGuideDetail.id = remissionGuide.RemissionGuideRiverDetail.Count() > 0 ? remissionGuide.RemissionGuideRiverDetail.Max(pld => pld.id) + 1 : 1;

                        remissionGuideDetail.isActive = true;
                        remissionGuideDetail.id_userCreate = ActiveUser.id;
                        remissionGuideDetail.dateCreate = DateTime.Now;
                        remissionGuideDetail.id_userUpdate = ActiveUser.id;
                        remissionGuideDetail.dateUpdate = DateTime.Now;
                        remissionGuideDetail.RemissionGuide = db.RemissionGuide.FirstOrDefault(i => i.id == remissionGuideDetail.id_remisionGuide);
                        remissionGuide.RemissionGuideRiverDetail.Add(remissionGuideDetail);
                    }
                    TempData["remissionriverGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("remissionriverGuide");

            var model = remissionGuide?.RemissionGuideRiverDetail.Where(d => d.isActive).ToList() ?? new List<RemissionGuideRiverDetail>();
            return PartialView("_Details", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverDetailsPartialDelete(System.Int32 id)//id_item)
        {
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);

            remissionGuide = remissionGuide ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuideRiver();
            try
            {
                var remissionDetail = remissionGuide.RemissionGuideRiverDetail.FirstOrDefault(p => p.id == id);
                if (remissionDetail != null)
                {
                    remissionDetail.isActive = false;
                    remissionDetail.id_userUpdate = ActiveUser.id;
                    remissionDetail.dateUpdate = DateTime.Now;
                }
                TempData["remissionriverGuide"] = remissionGuide;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            int id_rgTmp = 0;
            remissionGuide
                .RemissionGuideRiverDispatchMaterial = remissionGuide
                                                        .RemissionGuideRiverDispatchMaterial ?? new List<RemissionGuideRiverDispatchMaterial>();
            if (remissionGuide.RemissionGuideRiverDispatchMaterial.Count > 0)
            {
                int countRgrdm = remissionGuide.RemissionGuideRiverDispatchMaterial.Count;
                for (int i = countRgrdm - 1; i >= 0; i--)
                {
                    RemissionGuideRiverDispatchMaterial rgdT = remissionGuide
                                                                .RemissionGuideRiverDispatchMaterial
                                                                .ElementAt(i);

                    remissionGuide.RemissionGuideRiverDispatchMaterial.Remove(rgdT);

                }
            }

            if (remissionGuide != null && remissionGuide.RemissionGuideRiverDetail != null && remissionGuide.RemissionGuideRiverDetail.Count > 0)
            {
                var rgrdTmp = remissionGuide.RemissionGuideRiverDetail.Where(w => w.isActive).ToList();
                foreach (var dTmp in rgrdTmp)
                {
                    id_rgTmp = dTmp.id_remisionGuide;
                    if (id_rgTmp > 0)
                    {
                        var rgdmLst = db.RemissionGuideDispatchMaterial
                                        .Where(w => w.id_remisionGuide == id_rgTmp)
                                        .ToList();

                        foreach (var dTmp2 in rgdmLst)
                        {
                            RemissionGuideRiverDispatchMaterial rgrdmObj = remissionGuide
                                                                            .RemissionGuideRiverDispatchMaterial
                                                                            .FirstOrDefault(fod => fod.id_item == dTmp2.id_item);
                            if (rgrdmObj == null)
                            {
                                rgrdmObj = new RemissionGuideRiverDispatchMaterial();
                                rgrdmObj.id_item = dTmp2.id_item;
                                rgrdmObj.Item = db.Item.FirstOrDefault(fod => fod.id == dTmp2.id_item);
                                rgrdmObj.id_warehouse = dTmp2.id_warehouse;
                                rgrdmObj.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == dTmp2.id_warehouse);
                                rgrdmObj.id_warehouselocation = (int)dTmp2.id_warehouselocation;
                                rgrdmObj.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == dTmp2.id_warehouselocation);
                                rgrdmObj.sourceExitQuantity = dTmp2.sourceExitQuantity;
                                rgrdmObj.sendedDestinationQuantity = dTmp2.sendedDestinationQuantity;
                                remissionGuide.RemissionGuideRiverDispatchMaterial.Add(rgrdmObj);
                            }
                            else
                            {
                                rgrdmObj.Item = db.Item.FirstOrDefault(fod => fod.id == dTmp2.id_item);
                                rgrdmObj.id_warehouse = dTmp2.id_warehouse;
                                rgrdmObj.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == dTmp2.id_warehouse);
                                rgrdmObj.id_warehouselocation = (int)dTmp2.id_warehouselocation;
                                rgrdmObj.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == dTmp2.id_warehouselocation);
                                rgrdmObj.sourceExitQuantity += dTmp2.sourceExitQuantity;
                                rgrdmObj.sendedDestinationQuantity += dTmp2.sendedDestinationQuantity;
                            }
                        }
                    }
                }
            }

            TempData["remissionriverGuide"] = remissionGuide;
            TempData.Keep("remissionriverGuide");

            var model = remissionGuide?.RemissionGuideRiverDetail.Where(d => d.isActive).ToList() ?? new List<RemissionGuideRiverDetail>();
            return PartialView("_Details", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedRemissionGuideRiverDetails(int[] ids)
        {
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuide = remissionGuide ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuide.id);
            remissionGuide = remissionGuide ?? new RemissionGuideRiver();
            if (ids != null)
            {
                try
                {
                    var remissionGuideDetails = remissionGuide.RemissionGuideRiverDetail.Where(i => ids.Contains(i.id_remisionGuide));

                    foreach (var detail in remissionGuideDetails)
                    {
                        detail.isActive = false;
                        detail.id_userUpdate = ActiveUser.id;
                        detail.dateUpdate = DateTime.Now;
                    }

                    TempData["remissionriverGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("remissionriverGuide");
        }

        #endregion

        #region SINGLE CHANGE DOCUMENT STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);

            if (remissionGuide != null)
            {
                remissionGuide.getLiquidationInformation();
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                    if (remissionGuide != null && documentState != null)
                    {
                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;
                        db.RemissionGuideRiver.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;
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

            TempData["remissionriverGuide"] = remissionGuide;
            TempData.Keep("remissionriverGuide");

            return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);
        }

        public XmlDocument GenerateXML(RemissionGuideRiver pRemissionGuide, int company)
        {
            XmlDocument xmlout1 = new XmlDocument();
            //Boolean returnResult = false;


            // id documentState
            int? id_documentState = pRemissionGuide.Document.id_documentState;
            string code_documentState = db.DocumentState.FirstOrDefault(r => r.id == id_documentState)?.code ?? "";

            if (code_documentState != "09") return null;
            //if (this.InvoiceExterior == null) return null;
            //if (this.InvoiceDetail.Count() == 0) return null;
            if (pRemissionGuide.Document?.DocumentType?.isElectronic == false) return null;



            ElectronicDocument _electronicDocument = pRemissionGuide.Document.ElectronicDocument ?? new ElectronicDocument();
            ElectronicDocumentState electronicState = db.ElectronicDocumentState.FirstOrDefault(e => e.id_company == company && e.sriCode.Equals("01"));
            if (electronicState == null) return null;

            _electronicDocument = new ElectronicDocument
            {
                id_electronicDocumentState = electronicState.id
            };

            //XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            //XmlFactura xmlFactura = DB2XML.Factura2Xml(this.id);
            //xmlout1 = DB2XML.SerializeToXml(xmlFactura, ns);


            //String xml = xmlout1.OuterXml;
            //xml = xml.Replace(@"p2:nil=""true""", "");
            //xml = xml.Replace(@"xmlns:p2=""http://www.w3.org/2001/XMLSchema-instance"" /", "/");
            //xml = xml.Replace("<infoAdicional  />", "");

            //xmlout1.LoadXml(xml);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            var xmlGuiaRemision = DB2XML.GuiaRemision2Xml(pRemissionGuide.id);
            xmlout1 = SerializeToXml(xmlGuiaRemision, ns);


            String xml = xmlout1.OuterXml;
            xml = xml.Replace(@"p2:nil=""true""", "");
            xml = xml.Replace(@"xmlns:p2=""http://www.w3.org/2001/XMLSchema-instance"" /", "/");
            xml = xml.Replace("<infoAdicional  />", "");

            xmlout1.LoadXml(xml);

            /* if (!string.IsNullOrEmpty( fullPathXML))
			 {
				 string pathFileXmlFileName = fullPathXML + "\\" + this.Document.accessKey + ".xml";
				 xmlout1.Save(pathFileXmlFileName);
			 }*/

            _electronicDocument.xml = xmlout1.OuterXml;

            pRemissionGuide.Document.ElectronicDocument = _electronicDocument;

            //db.Invoice.Attach(this);
            //db.Entry(this).State = EntityState.Modified;



            return xmlout1;
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

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);
            XmlDocument xmlFEX = new XmlDocument();
            if (remissionGuide != null)
            {
                remissionGuide.getLiquidationInformation();
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "09"); //Pre - Autorizada

                    if (remissionGuide != null && documentState != null)
                    {
                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;

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

                        remissionGuide.Document.authorizationDate = DateTime.Now;
                        db.RemissionGuideRiver.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;

                        xmlFEX = GenerateXML(remissionGuide, (int)ViewData["id_company"]);

                        db.SaveChanges();
                        trans.Commit();
                        TempData["remissionriverGuide"] = remissionGuide;
                        TempData.Keep("remissionriverGuide");
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
                            //IntPtr token = IntPtr.Zero;
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
                    TempData.Keep("remissionriverGuide");
                    ViewData["EditError"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);
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
            RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);
            RemissionGuideRiver rgTmp = (RemissionGuideRiver)TempData["remissionGuide"];

            //if (remissionGuide != null)
            //{
            //    remissionGuide.isManual = rgTmp.isManual;
            //}

            //if (rgTmp.isManual == null)
            //    this.ViewBag.isManual = false;
            //else
            //    this.ViewBag.isManual = rgTmp.isManual;
            string msgXtraInfo = "";



            msgXtraInfo = "Obtener Ruta XML Guía De Remisión(B1.AutorizadoActualizado)";
            string routePathB1AutorizadoActualizado = ConfigurationManager.AppSettings["rutaXmlB1AutorizadoActualizado"];
            if (string.IsNullOrEmpty(routePathB1AutorizadoActualizado))
            {
                throw new Exception("No definida Configuración Ruta B1.AutorizadoActualizado.");
            }
            msgXtraInfo = "Guía De Remisión";
            DocumentState documentState09 = db.DocumentState.FirstOrDefault(s => s.code == "09"); //Estado de PRE-AUTORIZADA
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
                            //Release the context, and close user token



                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada
                            ElectronicDocumentState electronicDocumentState = db.ElectronicDocumentState.FirstOrDefault(s => s.code == "03"); //Autorizada
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

            return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);
        }


        [HttpPost]
        public ActionResult Protect(int id)
        {
            RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                    if (remissionGuide != null && documentState != null)
                    {
                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;

                        db.RemissionGuideRiver.Attach(remissionGuide);
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

            TempData["remissionriverGuide"] = remissionGuide;
            TempData.Keep("remissionriverGuide");

            return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);

            if (remissionGuide != null)
            {
                remissionGuide.getLiquidationInformation();
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                    if (remissionGuide != null && documentState != null)
                    {
                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;
                        db.RemissionGuideRiver.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["remissionriverGuide"] = remissionGuide;
                        TempData.Keep("remissionriverGuide");
                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + remissionGuide.Document.number + " anulada exitosamente");

                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionriverGuide");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);

            if (remissionGuide != null)
            {
                remissionGuide.getLiquidationInformation();
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {

                    var cantliquida = (from e in db.LiquidationFreightDetail
                                       where e.id_remisionGuide == id && e.LiquidationFreight.Document.DocumentState.code != "05"
                                       select e.id).Count();

                    if (cantliquida > 0)
                    {

                        TempData.Keep("remissionriverGuide");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido que ya posee una Liquidacion de Flete.");
                        return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);

                    }



                    if (remissionGuide.RemissionGuideRiverDetail != null && remissionGuide.RemissionGuideRiverDetail.Count > 0)
                    {

                        foreach (var detail in remissionGuide.RemissionGuideRiverDetail)
                        {

                            var cantliquidadetail = (from e in db.LiquidationFreightDetail
                                                     where e.id_remisionGuide == detail.id_remisionGuide && e.LiquidationFreight.Document.DocumentState.code != "05"
                                                     select e.id).Count();

                            if (cantliquidadetail > 0)
                            {

                                TempData.Keep("remissionriverGuide");
                                ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido que hay Guias Asocidas que ya posee una Liquidacion de Flete.");
                                return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);

                            }


                            if (!detail.RemissionGuide.Document.DocumentState.code.Equals("08") && detail.RemissionGuide.hasEntrancePlanctProduction != null && detail.RemissionGuide.hasEntrancePlanctProduction.Value == true)
                            {
                                TempData.Keep("remissionriverGuide");
                                ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido que hay Guias Asociadas que ya ingresaron a Planta.");
                                return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);
                            }
                        }


                    }

                    if (remissionGuide.Document.DocumentState.code != "06" && remissionGuide.Document.DocumentState.code != "03")
                    {
                        TempData.Keep("remissionriverGuide");
                        ViewData["EditMessage"] = ErrorMessage("No se puede reversar la guía de remisión debido que la Guia no esta AUTORIZADA o APROBADA.");
                        return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);
                    }
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

                    //DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                    if (remissionGuide != null && documentState != null)
                    {
                        remissionGuide.Document.id_documentState = documentState.id;
                        remissionGuide.Document.DocumentState = documentState;
                        db.RemissionGuideRiver.Attach(remissionGuide);
                        db.Entry(remissionGuide).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();
                        TempData["remissionriverGuide"] = remissionGuide;
                        TempData.Keep("remissionriverGuide");
                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + remissionGuide.Document.number + " reversada exitosamente");

                    }
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionriverGuide");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);
        }

        #endregion

        #region SELECTED DOCUMENT STATE CHANGE 

        [HttpPost, ValidateInput(false)]
        public void ApproveDocuments(int[] ids)
        {
            TempData.Keep("ProductionUnitProviderByProvider");
            var model = (TempData["model"] as List<RemissionGuideRiver>);
            model = model ?? new List<RemissionGuideRiver>();

            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            RemissionGuideRiver remissionGuide = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

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
        public JsonResult AutorizeDocuments(int[] ids)
        {
            TempData.Keep("ProductionUnitProviderByProvider");
            var model = (TempData["model"] as List<RemissionGuideRiver>);
            model = model ?? new List<RemissionGuideRiver>();

            GenericResultJson oJsonResult = new GenericResultJson();
            List<RemissionGuideRiver> _guiaList = new List<RemissionGuideRiver>();
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
                                RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);
                                if (remissionGuide == null) throw new Exception("Documento con identificador:" + id + " ,no se ha encontrado.");

                                msgXtraInfo = "Obtener Estado";
                                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "09");
                                if (documentState == null) throw new Exception("Estado con identificador: '09' ,no se ha encontrado.");

                                if (remissionGuide.Document.DocumentState.code != "03") throw new Exception("No se puede autorizar la guía de remisión:" + remissionGuide.Document.number + " ,debido a no tener su estado en Aprobado.");

                                _guiaList.Add(remissionGuide);

                                remissionGuide.Document.id_documentState = documentState.id;
                                remissionGuide.Document.DocumentState = documentState;

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

                                db.RemissionGuideRiver.Attach(remissionGuide);
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

                // Llamada Async
                System.Threading.Tasks.Task.Run(() =>
                    ServiceLogistics.CallXML(db,
                                            null,
                                            _guiaList,
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
            var model = (TempData["model"] as List<RemissionGuideRiver>);
            model = model ?? new List<RemissionGuideRiver>();


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
                    DocumentState documentState09 = db.DocumentState.FirstOrDefault(s => s.code == "09"); //Estado de PRE-AUTORIZADA
                    RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => ids.Contains(r.id) && r.Document.DocumentState.code != "09");
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
                                        remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);

                                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada
                                        ElectronicDocumentState electronicDocumentState = db.ElectronicDocumentState.FirstOrDefault(s => s.code == "03"); //Autorizada
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

                            if (change /*|| changeLista*/)
                            {
                                trans.Rollback();
                            }
                            LogController.WriteLog(e.Message);
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
            var model = (TempData["model"] as List<RemissionGuideRiver>);
            model = model ?? new List<RemissionGuideRiver>();

            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            RemissionGuideRiver remissionGuide = model.FirstOrDefault(r => r.id == id);

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
            var model = (TempData["model"] as List<RemissionGuideRiver>);
            model = model ?? new List<RemissionGuideRiver>();

            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            RemissionGuideRiver remissionGuide = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

                            if (remissionGuide != null && documentState != null)
                            {
                                remissionGuide.Document.id_documentState = documentState.id;
                                remissionGuide.Document.DocumentState = documentState;

                                //db.RemissionGuideRiver.Attach(remissionGuide);
                                //db.Entry(remissionGuide).State = EntityState.Modified;
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
            var model = (TempData["model"] as List<RemissionGuideRiver>);
            model = model ?? new List<RemissionGuideRiver>();

            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            RemissionGuideRiver remissionGuide = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                            if (remissionGuide != null && documentState != null)
                            {
                                remissionGuide.Document.id_documentState = documentState.id;
                                remissionGuide.Document.DocumentState = documentState;

                                foreach (var details in remissionGuide.RemissionGuideRiverDetail)
                                {
                                    //details.quantityApproved = 0.0M;

                                    //db.RemissionGuideRiverDetail.Attach(details);
                                    //db.Entry(details).State = EntityState.Modified;
                                }

                                //db.RemissionGuideRiver.Attach(remissionGuide);
                                //db.Entry(remissionGuide).State = EntityState.Modified;
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

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            var actions = new
            {
                btnApprove = true,
                btnAutorize = false,
                btnCheckAutorizeRSI = false,
                btnProtect = false,
                btnCancel = false,
                btnRevert = false,
                btnPrint = false,
                btnPrintAlldoc = false,
                btnPrintManual = false,
                btnPrintAlldocManual = false
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);
            //int state = remissionGuide.Document.DocumentState.id;
            string state = remissionGuide.Document.DocumentState.code;

            if (state == "01") // PENDIENTE
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnCheckAutorizeRSI = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = false,
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
                    btnPrint = true,
                    btnPrintAlldoc = true,
                    btnPrintManual = true,
                    btnPrintAlldocManual = true
                };
            }
            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_remissionGuide)
        {
            TempData.Keep("remissionriverGuide");

            int index = db.RemissionGuideRiver.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_remissionGuide);

            var result = new
            {
                maximunPages = db.RemissionGuideRiver.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            RemissionGuideRiver remissionGuide = db.RemissionGuideRiver.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (remissionGuide != null)
            {
                TempData["remissionriverGuide"] = remissionGuide;
                TempData.Keep("remissionriverGuide");
                return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuide);
            }

            TempData.Keep("remissionriverGuide");

            return PartialView("_RemissionGuideRiverMainFormPartial", new RemissionGuideRiver());
        }

        #endregion

        #region Popup
        public ActionResult AddSelectedRemissionGuideDetailsPopup(int[] ids)
        {
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuide = remissionGuide ?? new RemissionGuideRiver();
            remissionGuide = remissionGuide ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuide.id);

            var idsd = (from fr in ids
                        select fr).Distinct().ToList();


            List<RemissionGuideRiverDetail> lstremissionGuideDetail = TempData["remissionriverGuidedetailpopup"] as List<RemissionGuideRiverDetail>;

            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var i in idsd)
                    {
                        RemissionGuideRiverDetail remissionGuideDetail = lstremissionGuideDetail.Where(x => x.id_remisionGuide == i).FirstOrDefault();

                        var modelItem = remissionGuide.RemissionGuideRiverDetail.FirstOrDefault(it => (!it.isActive) &&
                                                                                                   it.id_remisionGuide == i);
                        if (modelItem != null)
                        {
                            modelItem.isActive = true;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;
                            modelItem.quantityDispatchMaterial = remissionGuideDetail.quantityDispatchMaterial;
                            modelItem.quantityOrdered = remissionGuideDetail.quantityOrdered;
                            modelItem.id_item = remissionGuideDetail.id_item;
                            modelItem.Item = db.Item.FirstOrDefault(fod => fod.id == remissionGuideDetail.id_item);
                            modelItem.quantityProgrammed = remissionGuideDetail.quantityProgrammed;
                            this.UpdateModel(modelItem);
                            modelItem.RemissionGuide = db.RemissionGuide.FirstOrDefault(y => y.id == remissionGuideDetail.id_remisionGuide);
                        }
                        else
                        {

                            var cont = (from fd in remissionGuide.RemissionGuideRiverDetail
                                        where fd.id_remisionGuide == i
                                        select fd).Count();

                            if (cont <= 0)
                            {
                                remissionGuideDetail = remissionGuideDetail ?? new RemissionGuideRiverDetail();

                                remissionGuideDetail.id = remissionGuide.RemissionGuideRiverDetail.Count() > 0 ? remissionGuide.RemissionGuideRiverDetail.Max(pld => pld.id) + 1 : 1;

                                remissionGuideDetail.isActive = true;
                                remissionGuideDetail.id_userCreate = ActiveUser.id;
                                remissionGuideDetail.dateCreate = DateTime.Now;
                                remissionGuideDetail.id_userUpdate = ActiveUser.id;
                                remissionGuideDetail.dateUpdate = DateTime.Now;
                                remissionGuideDetail.RemissionGuide = db.RemissionGuide.FirstOrDefault(y => y.id == remissionGuideDetail.id_remisionGuide);
                                remissionGuide.RemissionGuideRiverDetail.Add(remissionGuideDetail);
                            }
                        }

                    }
                    TempData["remissionriverGuide"] = remissionGuide;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            int id_rgTmp = 0;
            remissionGuide
                .RemissionGuideRiverDispatchMaterial = remissionGuide
                                                        .RemissionGuideRiverDispatchMaterial ?? new List<RemissionGuideRiverDispatchMaterial>();

            if (remissionGuide.RemissionGuideRiverDispatchMaterial.Count > 0)
            {
                int countRgrdm = remissionGuide.RemissionGuideRiverDispatchMaterial.Count;
                for (int i = countRgrdm - 1; i >= 0; i--)
                {
                    RemissionGuideRiverDispatchMaterial rgdT = remissionGuide
                                                                .RemissionGuideRiverDispatchMaterial
                                                                .ElementAt(i);

                    remissionGuide.RemissionGuideRiverDispatchMaterial.Remove(rgdT);

                }
            }

            if (remissionGuide != null && remissionGuide.RemissionGuideRiverDetail != null && remissionGuide.RemissionGuideRiverDetail.Count > 0)
            {
                var rgrdTmp = remissionGuide.RemissionGuideRiverDetail.Where(w => w.isActive).ToList();
                foreach (var dTmp in rgrdTmp)
                {
                    id_rgTmp = dTmp.id_remisionGuide;
                    if (id_rgTmp > 0)
                    {
                        var rgdmLst = db.RemissionGuideDispatchMaterial
                                        .Where(w => w.id_remisionGuide == id_rgTmp)
                                        .ToList();

                        foreach (var dTmp2 in rgdmLst)
                        {
                            RemissionGuideRiverDispatchMaterial rgrdmObj = remissionGuide
                                                                            .RemissionGuideRiverDispatchMaterial
                                                                            .FirstOrDefault(fod => fod.id_item == dTmp2.id_item);
                            if (rgrdmObj == null)
                            {
                                rgrdmObj = new RemissionGuideRiverDispatchMaterial();
                                rgrdmObj.id_item = dTmp2.id_item;
                                rgrdmObj.Item = db.Item.FirstOrDefault(fod => fod.id == dTmp2.id_item);
                                rgrdmObj.id_warehouse = dTmp2.id_warehouse;
                                rgrdmObj.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == dTmp2.id_warehouse);
                                rgrdmObj.id_warehouselocation = (int)dTmp2.id_warehouselocation;
                                rgrdmObj.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == dTmp2.id_warehouselocation);
                                rgrdmObj.sourceExitQuantity = dTmp2.sourceExitQuantity;
                                rgrdmObj.sendedDestinationQuantity = dTmp2.sendedDestinationQuantity;
                                remissionGuide.RemissionGuideRiverDispatchMaterial.Add(rgrdmObj);
                            }
                            else
                            {
                                rgrdmObj.Item = db.Item.FirstOrDefault(fod => fod.id == dTmp2.id_item);
                                rgrdmObj.id_warehouse = dTmp2.id_warehouse;
                                rgrdmObj.Warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == dTmp2.id_warehouse);
                                rgrdmObj.id_warehouselocation = (int)dTmp2.id_warehouselocation;
                                rgrdmObj.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(fod => fod.id == dTmp2.id_warehouselocation);
                                rgrdmObj.sourceExitQuantity += dTmp2.sourceExitQuantity;
                                rgrdmObj.sendedDestinationQuantity += dTmp2.sendedDestinationQuantity;
                            }
                        }
                    }
                }
            }

            TempData["remissionriverGuide"] = remissionGuide;
            TempData.Keep("remissionriverGuide");

            var model = remissionGuide?.RemissionGuideRiverDetail.Where(d => d.isActive).ToList() ?? new List<RemissionGuideRiverDetail>();
            return PartialView("_Details", model.ToList());



        }

        [HttpPost, ValidateInput(false)]
        public JsonResult RemissionGuideGeneredDetailspopupPartial(RemissionGuideRiver item, Document document,
                                                        RemissionGuideRiverTransportation transportation, string despachurehour)
        {


            var id_provider = item.id_providerRemisionGuideRiver;
            var id_produccionunit = item.id_productionUnitProvider;
            var despachureDate = item.despachureDate.Date;
            var id_reason = item.id_reason;
            var id_shippingType = item.id_shippingType;
            List<int> listmaindetail = new List<int>();
            List<int> listmaindetailriver = new List<int>();

            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuide = remissionGuide ?? new RemissionGuideRiver();
            remissionGuide = remissionGuide ?? db.RemissionGuideRiver.FirstOrDefault(i => i.id == remissionGuide.id);

            var id_remissionGuide = remissionGuide.id;

            if (remissionGuide != null && remissionGuide.RemissionGuideRiverDetail.Count > 0)
            {
                listmaindetail = (from de in remissionGuide.RemissionGuideRiverDetail
                                  where de.isActive
                                  select de.id_remisionGuide).ToList();
            }

            listmaindetailriver = (from de in db.RemissionGuideRiverDetail
                                   where de.isActive && de.RemissionGuideRiver.Document.DocumentState.code != "05" && de.id_remissionGuideRiver != id_remissionGuide
                                   select de.id_remisionGuide).ToList();


            List<RemissionGuideRiverDetail> listdetail = new List<RemissionGuideRiverDetail>();

            var lisdet = (from i in db.RemissionGuide
                          where
                          i.id_providerRemisionGuide == id_provider &&
                              DbFunctions.TruncateTime(i.despachureDate) == DbFunctions.TruncateTime(despachureDate) && i.id_reason == id_reason &&
                                i.id_productionUnitProvider == id_produccionunit && (i.Document.DocumentState.code == "03" || i.Document.DocumentState.code == "06") &&
                                !listmaindetail.Contains(i.id) && !listmaindetailriver.Contains(i.id)
                          //&& ((i.hasExitPlanctProduction == null) || (i.hasExitPlanctProduction.HasValue && i.hasExitPlanctProduction.Value == false)) 
                          //&& ((i.hasEntrancePlanctProduction == null) || (i.hasEntrancePlanctProduction.HasValue && i.hasEntrancePlanctProduction.Value == false))
                          //eliminar si no es necesario
                          select new
                          {
                              dateCreate = DateTime.Now,
                              dateUpdate = DateTime.Now,
                              id_remisionGuide = i.id,
                              id_userCreate = ActiveUser.id,
                              isActive = true,
                              id_userUpdate = ActiveUser.id,
                              RemissionGuide = i,
                              id_item = i.RemissionGuideDetail.FirstOrDefault().id_item,
                              quantityDispatchMaterial = i.RemissionGuideDispatchMaterial.Where(X => X.isActive).Select(l => l.sourceExitQuantity).DefaultIfEmpty(0).Sum(),
                              quantityOrdered = i.RemissionGuideDetail.Where(x => x.isActive).Select(l => l.quantityOrdered).DefaultIfEmpty(0).Sum(),
                              quantityProgrammed = i.RemissionGuideDetail.Where(x => x.isActive).Select(l => l.quantityProgrammed).Sum(),
                              id_remissionGuideRiver = 0,
                              id = i.id



                          }).ToList();


            foreach (var i in lisdet)
            {
                RemissionGuideRiverDetail det = new RemissionGuideRiverDetail()
                {
                    dateCreate = i.dateCreate,
                    dateUpdate = i.dateUpdate,
                    id_remisionGuide = i.id_remisionGuide,
                    id_userCreate = i.id_userCreate,
                    isActive = i.isActive,
                    Item = db.Item.FirstOrDefault(fod => fod.id == i.id_item),
                    id_item = i.id_item,
                    id_userUpdate = i.id_userUpdate,
                    RemissionGuide = db.RemissionGuide.Where(x => x.id == i.id_remisionGuide).FirstOrDefault(),
                    quantityDispatchMaterial = i.quantityDispatchMaterial,
                    quantityOrdered = i.quantityOrdered,
                    quantityProgrammed = i.quantityProgrammed,
                    id_remissionGuideRiver = 0,
                    id = i.id
                };

                listdetail.Add(det);
            }

            TempData["remissionriverGuidedetailpopup"] = listdetail;
            TempData.Keep("remissionriverGuidedetailpopup");
            TempData.Keep("remissionriverGuide");

            var result = new
            {
                register = lisdet.Count(),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverDetailsPopupPartial()
        {
            List<RemissionGuideRiverDetail> remissionGuideRiverDetail = (TempData["remissionriverGuidedetailpopup"] as List<RemissionGuideRiverDetail>);
            remissionGuideRiverDetail = remissionGuideRiverDetail ?? new List<RemissionGuideRiverDetail>();
            TempData.Keep("remissionriverGuidedetailpopup");

            TempData.Keep("remissionriverGuide");

            return PartialView("_RemissionGuidePartialFilter", remissionGuideRiverDetail);

        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult GetAddressPurchaseRemisionGuideProvider(int? id_ProductionUnitProvider)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();



            var ProductionUnitProvider_address = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_ProductionUnitProvider)?.address ?? "";
            var FishingSite = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_ProductionUnitProvider)?.id_FishingSite ?? 0;
            String nameFishingSite = "";
            String nameFishingZone = "";
            int id_fishingSite = 0;
            int id_fishingZone = 0;

            if (FishingSite > 0)
            {
                var tempFishingSite = db.FishingSite.Where(x => x.id == FishingSite).FirstOrDefault();
                if (tempFishingSite != null)
                {
                    nameFishingSite = tempFishingSite.name;
                    nameFishingZone = tempFishingSite.FishingZone.name;
                    id_fishingSite = tempFishingSite.id;
                    id_fishingZone = tempFishingSite.FishingZone.id;
                    remissionGuideRiver.RemissionGuideRiverTransportation.id_FishingSiteRGR = tempFishingSite.id;
                    remissionGuideRiver.RemissionGuideRiverTransportation.id_FishingZoneRGRNew = tempFishingSite.FishingZone.id;
                }



            }

            TempData["remissionriverGuide"] = remissionGuideRiver;
            TempData.Keep("remissionriverGuide");
            var result = new
            {
                ProductionUnitProvider_address = ProductionUnitProvider_address,
                Message = "Ok",
                FishingSite = FishingSite == 0 ? null : FishingSite.ToString(),
                nameFishingSite = nameFishingSite,
                nameFishingZone = nameFishingZone,
                idFishingSite = id_fishingSite,
                idFishingZone = id_fishingZone
            };

            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult pricefreightrefresh(RGRParamPriceFreight rgParam)
        {

            var wresult = pricefreight(rgParam);


            var result = new
            {
                pricefreight = wresult
            };


            TempData.Keep("remissionriverGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Internalrefresh(bool isInternal)
        {


            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);


            var result = new
            {
                ok = "ok"
            };

            TempData["remissionriverGuide"] = remissionGuide;
            TempData.Keep("remissionriverGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }




        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateVehicleSelection(int id_vehicle)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            TempData.Keep("remissionriverGuide");

            var result = new
            {
                hasError = 0,
                Error = ""
            };

            var vptBilling = db.VehicleProviderTransportBilling.FirstOrDefault(fod => fod.id_vehicle == id_vehicle &&
                                                                                        fod.datefin == null &&
                                                                                        fod.state == true);

            result = new
            {
                hasError = (vptBilling == null) ? 1 : 0,
                Error = (vptBilling == null) ? "El Vehículo no tiene compañía que factura" : ""
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult VehicleData(int? id_vehicle)
        {

            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);

            Vehicle vehicle = db.Vehicle.FirstOrDefault(i => i.id == id_vehicle);

            if (vehicle == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }


            VeicleProviderTransport veicleprovider = db.VeicleProviderTransport.Where(g => g.id_vehicle == vehicle.id && g.Estado && g.datefin == null).FirstOrDefault();

            var result = new
            {
                mark = vehicle.mark,
                model = vehicle.model,
                id_VeicleProvider = veicleprovider?.id_Provider
            };

            TempData["remissionriverGuide"] = remissionGuide;
            TempData.Keep("remissionriverGuide");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateSecuritySealNumber(int id_remissionGuide, string number, bool isNew)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            TempData.Keep("remissionriverGuide");


            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            remissionGuideRiver.RemissionGuideRiverSecuritySeal = remissionGuideRiver.RemissionGuideRiverSecuritySeal ?? new List<RemissionGuideRiverSecuritySeal>();

            List<RemissionGuideRiverSecuritySeal> tmp = remissionGuideRiver.RemissionGuideRiverSecuritySeal.ToList();
            List<RemissionGuideRiverSecuritySeal> tmp2 = remissionGuideRiver.RemissionGuideRiverSecuritySeal.ToList();
            if (remissionGuideRiver.RemissionGuideRiverSecuritySeal != null && remissionGuideRiver.RemissionGuideRiverSecuritySeal.Count > 0)
            {
                if (isNew == true)
                {
                    var securitySeal = remissionGuideRiver.RemissionGuideRiverSecuritySeal.FirstOrDefault(fod => fod.number == number);
                    if (securitySeal != null)
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
                    var securitySeal2 = tmp2.FirstOrDefault(fod => fod.number == number);
                    if (securitySeal2 != null)
                    {
                        result = new
                        {
                            itsRepeated = 1,
                            Error = "Número de sello en uso."
                        };
                    }
                }
            }

            TempData.Keep("remissionriverGuide");
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateAssigendPersonValidation(int id_remissionGuide, int id_person, bool isNew)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            TempData.Keep("remissionriverGuide");

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            remissionGuideRiver.RemissionGuideRiverAssignedStaff = remissionGuideRiver.RemissionGuideRiverAssignedStaff ?? new List<RemissionGuideRiverAssignedStaff>();

            List<RemissionGuideRiverAssignedStaff> tmp = remissionGuideRiver.RemissionGuideRiverAssignedStaff.ToList();
            List<RemissionGuideRiverAssignedStaff> tmp2 = remissionGuideRiver.RemissionGuideRiverAssignedStaff.ToList();
            if (remissionGuideRiver.RemissionGuideRiverAssignedStaff != null && remissionGuideRiver.RemissionGuideRiverAssignedStaff.Count > 0)
            {
                if (isNew == true)
                {
                    var assignedStaff = remissionGuideRiver.RemissionGuideRiverAssignedStaff.FirstOrDefault(fod => fod.id_person == id_person);
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

            TempData.Keep("remissionriverGuide");
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region TRANSPORTATION
        [HttpPost]
        public ActionResult GetFishingSiteRGR(int? id_FishingZoneRGRNew)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();

            if (id_FishingZoneRGRNew == null || id_FishingZoneRGRNew < 0)
            {
                if (Request.Params["id_FishingZoneRGRNew"] != null && Request.Params["id_FishingZoneRGRNew"] != "")
                    id_FishingZoneRGRNew = int.Parse(Request.Params["id_FishingZoneRGRNew"]);
                else id_FishingZoneRGRNew = -1;
            }

            remissionGuideRiver.RemissionGuideRiverTransportation = remissionGuideRiver.RemissionGuideRiverTransportation ?? new RemissionGuideRiverTransportation();
            remissionGuideRiver.RemissionGuideRiverTransportation.id_FishingZoneRGRNew = id_FishingZoneRGRNew;

            TempData["remissionriverGuide"] = remissionGuideRiver;
            TempData.Keep("remissionriverGuide");

            return PartialView("comboboxcascading/_ComboBoxFishingSite", remissionGuideRiver?.RemissionGuideRiverTransportation);
        }
        #endregion

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintRemissionGuideRiverReport(int id_rg, string typePrint)
        {
            RemissionGuideRiver remissionGuideRiver = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuideRiver = remissionGuideRiver ?? new RemissionGuideRiver();
            TempData["remissionriverGuide"] = remissionGuideRiver;
            TempData.Keep("remissionriverGuide");

            //string _printType = ConfigurationManager.AppSettings["printDirect"];

            #region "Armo Parametros"
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id_RemissionGuideRiver";
            _param.Valor = id_rg;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "RGRGRF";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);
            if (typePrint == "D")
            {
                _repMod.codeReport = "RGRVD1";
                rep.printType = "D";
                rep.codeReport = "RGRVD1";
            }

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintRemissionGuideRiverReports(int id_rg, int? id_warehouse, string codeReport, string typePrint)
        {
            RemissionGuideRiver remissionGuide = (TempData["remissionriverGuide"] as RemissionGuideRiver);
            remissionGuide = remissionGuide ?? new RemissionGuideRiver();
            TempData["remissionriverGuide"] = remissionGuide;
            TempData.Keep("remissionriverGuide");

            //string _printType = ConfigurationManager.AppSettings["printDirectLog"];

            #region "Armo Parametros"
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
                if (codeReport == "GRVSRF")
                {
                    _repMod.codeReport = "F1GRVS";
                    rep.codeReport = "F1GRVS";
                }
                else if (codeReport == "GRVCRF")
                {
                    _repMod.codeReport = "F1GRVC";
                    rep.codeReport = "F1GRVC";
                }
            }

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPrintInformation(int? id_remissionGuideRiver)
        {
            TempData.Keep("remissionriverGuide");
            var result = new
            {
                hasAdvanceP = "N",
                hasPersAss = "N"
            };
            decimal valAdvance = 0;
            valAdvance = db.RemissionGuideRiverTransportation
                            .FirstOrDefault(fod => fod.id_remissionGuideRiver == id_remissionGuideRiver)?
                            .advancePrice ?? 0;

            int countPers = 0;
            countPers = db.RemissionGuideRiverAssignedStaff.Where(w => w.viaticPrice > 0 && w.id_remissionGuideRiver == id_remissionGuideRiver).ToList().Count();

            result = new { hasAdvanceP = ((valAdvance > 0) ? "Y" : "N"), hasPersAss = ((countPers > 0) ? "Y" : "N") };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}