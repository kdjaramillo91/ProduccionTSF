using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using System.IO;
using System.Configuration;
using Utilitarios.CorreoElectronico;
using Utilitarios.Encriptacion;
using static DXPANACEASOFT.Controllers.RemissionGuideControlVehicleEntranceController;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class RemissionGuideControlVehicleEntranceThirdController : DefaultController
    {
        // POST: RemissionGuideControlVehicleEntranceThird
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region "REMISSION GUIDE CONTROL VEHICLE ENTRANCE THIRD FILTER RESULTS"
        [HttpPost]
        public ActionResult RemissionGuideControlVehicleEntranceThirdResults(RemissionGuide remissionGuide,
                                          Document document,
                                          DateTime? startEmissionDate, DateTime? endEmissionDate,
                                          DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                          DateTime? startDespachureDate, DateTime? endDespachureDate,
                                          DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                          int[] items, int[] businessOportunities)
        {
            //var model = db.RemissionGuide.Where(w => w.RemissionGuideControlVehicle != null && w.RemissionGuideTransportation.isOwn == true && w.RemissionGuideControlVehicle.hasEntrancePlanctProduction == true).ToList();
            db.Database.CommandTimeout = 1200;
            List<RemissionGuideEntradaTerceroDTO> model = db.Database.SqlQuery<RemissionGuideEntradaTerceroDTO>("exec spc_Consultar_GuiasRemisionEntradaTerceroDTO_StoredProcedure").ToList();

            #region DOCUMENT FILTERS

            if (document.id_documentState != 0)
            {
                model = model.Where(o => o.iddocumentState == document.id_documentState).ToList();
            }

            if (!string.IsNullOrEmpty(document.number))
            {
                model = model.Where(o => o.Numero.Contains(document.number)).ToList();
            }

            if (!string.IsNullOrEmpty(document.reference))
            {
                model = model.Where(o => o.Referencia.Contains(document.reference)).ToList();
            }
            if (!string.IsNullOrEmpty(remissionGuide.Guia_Externa))
            {
                model = model.Where(o => o.GuiaExterna.Contains(remissionGuide.Guia_Externa)).ToList();
            }

            if (startEmissionDate != null && endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.FechaEmision.Date) <= 0 && DateTime.Compare(o.FechaEmision.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            if (startAuthorizationDate != null && endAuthorizationDate != null)
            {
                model = model.Where(o => o.FechaAutorizacion != null && DateTime.Compare(startAuthorizationDate.Value.Date, o.FechaAutorizacion.Value.Date) <= 0 && DateTime.Compare(o.FechaAutorizacion.Value.Date, endAuthorizationDate.Value.Date) <= 0).ToList();
            }

            if (!string.IsNullOrEmpty(document.accessKey))
            {
                model = model.Where(o => o.AccessKey.Contains(document.accessKey)).ToList();
            }

            if (!string.IsNullOrEmpty(document.authorizationNumber))
            {
                model = model.Where(o => o.NumeroAutorizacion.Contains(document.authorizationNumber)).ToList();
            }


            #endregion


            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_RemissionGuideControlVehicleEntranceThirdResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }

        public class RemissionGuideEntradaTerceroDTO
        {
            public int id { get; set; }
            public string Numero { get; set; }
            public string Referencia { get; set; }
            public System.DateTime FechaEmision { get; set; }
            public System.DateTime? FechaAutorizacion { get; set; }
            public string NumeroAutorizacion { get; set; }
            public string AccessKey { get; set; }
            public int iddocumentState { get; set; }
            public string Estado { get; set; }
            public string MedioTransporte { get; set; }
            public string Proceso { get; set; }
            public string GuiaExterna { get; set; }
            public string Proveedor { get; set; }
            public string UnidadProduccion { get; set; }
            public string Zona { get; set; }
            public string Sitio { get; set; }
            public string Chofer { get; set; }
            public string Placa { get; set; }
            public Nullable<System.DateTime> FechaEntrada { get; set; }
            public Nullable<System.TimeSpan> HoraEntrada { get; set; }
            public string UsuarioCreacion { get; set; }
            public DateTime? FechaCreacion { get; set; }
            public string UsuarioModificacion { get; set; }
            public DateTime? FechaModificacion { get; set; }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideResults()
        {
            var model = db.RemissionGuide.Where(w => (w.PurchaseOrderShippingType.code == "T" 
                                        || w.PurchaseOrderShippingType.code == "M"
                                        || w.PurchaseOrderShippingType.code == "TF") 
                                        && w.Document.DocumentState.code == "03" 
                                        && w.RemissionGuideControlVehicle == null 
                                        && (w.RemissionGuideTransportation.isOwn == true)).ToList();

            TempData["modelBegin"] = model;
            TempData.Keep("modelBegin");
            
            return PartialView("_RemissionGuideHeaderResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuidePartial()
        {
            List<RemissionGuide> RgFinalLst = new List<RemissionGuide>();

            if (TempData["modelBegin"] != null)
            {
                RgFinalLst = ((List<RemissionGuide>)TempData["modelBegin"] );

            }
            else
            {
                RgFinalLst = db.RemissionGuide.Where(w => (w.PurchaseOrderShippingType.code == "T"
                                    || w.PurchaseOrderShippingType.code == "M"
                                    || w.PurchaseOrderShippingType.code == "TF")
                                    && w.Document.DocumentState.code == "03"
                                    && w.RemissionGuideControlVehicle == null
                                    && (w.RemissionGuideTransportation.isOwn == true)).ToList();

            }

            TempData["modelBegin"] = RgFinalLst;
            TempData.Keep("modelBegin");

            return PartialView("_RemissionGuideResultsPartial", RgFinalLst.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        #region "REMISSION GUIDE CONTROL VEHICLE ENTRANCE THIRD HEADER"

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleEntranceThirdPartial()
        {
            var model = (TempData["model"] as List<RemissionGuideEntradaTerceroDTO>);
            model = model ?? new List<RemissionGuideEntradaTerceroDTO>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideControlVehicleEntranceThirdPartial", model.OrderByDescending(r => r.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleEntranceThirdPartialAddNew(bool approve
                                                                                , string entranceTimeBuilding
                                                                                , string carRegistrationThird
                                                                                , string driverNameThird
                                                                                , string sailEntranceThird
                                                                                , string guideNumber
																				, string identificationNumberDriverThird
																				, RemissionGuideControlVehicle itemModel)
        
        {
            TempData.Keep("remissionGuideForControlVehicleEntranceThird");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuide remissionGuideForControlVehicle = (RemissionGuide)TempData["remissionGuideForControlVehicleEntranceThird"];
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == remissionGuideForControlVehicle.id);

            string ruta = ConfigurationManager.AppSettings["rutaXmlFEX"];
            string rutaA1Firmar = ConfigurationManager.AppSettings["rutaXmlA1Firmar"];


            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    itemModel.id_remissionGuide = remissionGuideForControlVehicle.id;

                    RemissionGuideTransportation rgt = db.RemissionGuideTransportation.FirstOrDefault(fod => fod.id_remionGuide == remissionGuide.id);
                    RemissionGuideTransportationCustomizedInformation rgtcti = db.RemissionGuideTransportationCustomizedInformation.FirstOrDefault(fod => fod.id_RemissionGuide == remissionGuide.id);

                    rgt.carRegistration = carRegistrationThird;
                    rgt.driverName = driverNameThird;
					rgt.identificationNumberDriver = identificationNumberDriverThird;

					remissionGuide.hasEntrancePlanctProduction = true;
                    itemModel.hasEntrancePlanctProduction = true;

                    remissionGuide.RemissionGuideTransportation.carRegistration = carRegistrationThird;
                    remissionGuide.RemissionGuideTransportation.driverName = driverNameThird;
					remissionGuide.RemissionGuideTransportation.identificationNumberDriver = identificationNumberDriverThird;

					db.RemissionGuide.Attach(remissionGuide);
                    db.Entry(remissionGuide).State = EntityState.Modified;

                    remissionGuideForControlVehicle.RemissionGuideTransportation.carRegistration = carRegistrationThird;
                    remissionGuideForControlVehicle.RemissionGuideTransportation.driverName = driverNameThird;
					remissionGuideForControlVehicle.RemissionGuideTransportation.identificationNumberDriver = identificationNumberDriverThird;

					db.RemissionGuideTransportation.Attach(rgt);
                    db.Entry(rgt).State = EntityState.Modified;

                    remissionGuideForControlVehicle.RemissionGuideTransportationCustomizedInformation = remissionGuideForControlVehicle
                        .RemissionGuideTransportationCustomizedInformation ?? new RemissionGuideTransportationCustomizedInformation();
                    remissionGuideForControlVehicle.RemissionGuideTransportationCustomizedInformation.numberGuide = guideNumber;
                    remissionGuideForControlVehicle.RemissionGuideTransportationCustomizedInformation.SailEntranceThird = sailEntranceThird;

					rgtcti = rgtcti ?? new RemissionGuideTransportationCustomizedInformation();
                    rgtcti.id_RemissionGuide = remissionGuide.id;
                    rgtcti.numberGuide = guideNumber;
                    rgtcti.SailEntranceThird = sailEntranceThird;

					db.RemissionGuideTransportationCustomizedInformation.Attach(rgtcti);
                    db.Entry(rgtcti).State = EntityState.Modified;

                    if (!string.IsNullOrEmpty(entranceTimeBuilding)) itemModel.entranceTimeProductionBuilding = TimeSpan.Parse(entranceTimeBuilding);

                    if (approve)
                    {

                    }

                    #region Document
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "161");
                    EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);

                    var document = db.Document.FirstOrDefault(d => d.id_documentOrigen == itemModel.id_remissionGuide && d.id_documentType == documentType.id);

                    if (document == null)
                    {
                        document = new Document
                        {
                            number = GetDocumentNumber(documentType?.id ?? 0),
                            sequential = GetDocumentSequential(documentType?.id ?? 0),
                            emissionDate = DateTime.Now,
                            id_emissionPoint = emissionPoint.id,
                            id_documentType = documentType?.id ?? 0,
                            id_documentState = documentState?.id ?? 0,
                            id_userCreate = ActiveUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = ActiveUser.id,
                            dateUpdate = DateTime.Now,
                            id_documentOrigen = itemModel.id_remissionGuide,
                        };
                        //Actualiza Secuencial
                        if (documentType != null)
                        {
                            documentType.currentNumber = documentType.currentNumber + 1;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;
                        }
                        db.Document.Add(document);
                        db.Entry(document).State = EntityState.Added;

                    }
                    else
                    {
                        document.id_userUpdate = ActiveUser.id;
                        document.dateUpdate = DateTime.Now;
                        db.Document.Attach(document);
                        db.Entry(document).State = EntityState.Modified;
                    }

                    #endregion

                    string answer = Services.ServiceLogistics.AutorizeRemissionGuide(remissionGuide.id, this.ActiveCompanyId, ruta, rutaA1Firmar, driverNameThird, carRegistrationThird);
                    db.RemissionGuideControlVehicle.Add(itemModel);
                    db.SaveChanges();
                    trans.Commit();

                    itemModel.providerNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.providerNameAux;
                    itemModel.productionUnitProviderAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.productionUnitProviderAux;
                    itemModel.poolNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.poolNameAux;
                    itemModel.totalPoundsAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.totalPoundsAux;
                    itemModel.zoneNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.zoneNameAux;
                    itemModel.siteNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.siteNameAux;
                    itemModel.addressTargetAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.addressTargetAux;
                    itemModel.shippingTypeAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.shippingTypeAux;
                    itemModel.driverNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.driverNameAux;

                    itemModel.productionUnitProviderAuxLab = remissionGuideForControlVehicle.RemissionGuideControlVehicle.productionUnitProviderAuxLab;


                    TempData["remissionGuideForControlVehicleEntranceThird"] = remissionGuide;
                    TempData.Keep("remissionGuideForControlVehicleEntranceThird");

                    ViewData["EditMessage"] = SuccessMessage("Control de Entrada para Guía de Remisión: " + remissionGuideForControlVehicle.Document.number + " se ha guardado correctamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuideForControlVehicleEntranceThird");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            itemModel.RemissionGuide = remissionGuideForControlVehicle;

            
            
            return PartialView("_RemissionGuideControlVehicleEntranceThirdMainFormPartial", itemModel);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleEntranceThirdPartialUpdate(bool approve
                                                                                , string entranceTimeBuilding
                                                                                , string carRegistrationThird
                                                                                , string driverNameThird
                                                                                , string sailEntranceThird
                                                                                , string guideNumber
																				, string identificationNumberDriverThird
																				, RemissionGuideControlVehicle itemModel)
        {
            TempData.Keep("remissionGuideForControlVehicleEntranceThird");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuide remissionGuideForControlVehicle = (RemissionGuide)TempData["remissionGuideForControlVehicleEntranceThird"];

            RemissionGuideControlVehicle modelItem = db.RemissionGuideControlVehicle
                                                        .FirstOrDefault(r => r.id_remissionGuide == remissionGuideForControlVehicle.id);

            if (itemModel != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region "REMISSION GUIDE CONTROL VEHICLE ENTRANCE THIRD"

                        modelItem.entranceDateProductionBuilding = itemModel.entranceDateProductionBuilding;
                        modelItem.hasEntrancePlanctProduction = true;

                        if (!string.IsNullOrEmpty(entranceTimeBuilding)) modelItem.entranceTimeProductionBuilding = TimeSpan.Parse(entranceTimeBuilding);

                        modelItem.ObservationEntrance = itemModel.ObservationEntrance;

                        RemissionGuideTransportation rgt = db.RemissionGuideTransportation.FirstOrDefault(fod => fod.id_remionGuide == modelItem.id_remissionGuide);
                        RemissionGuideTransportationCustomizedInformation rgtcti = db.RemissionGuideTransportationCustomizedInformation.FirstOrDefault(fod => fod.id_RemissionGuide == modelItem.id_remissionGuide);


                        rgt.carRegistration = carRegistrationThird;
                        rgt.driverName = driverNameThird;
						rgt.identificationNumberDriver = identificationNumberDriverThird;


						modelItem.RemissionGuide.RemissionGuideTransportation.carRegistration = carRegistrationThird;
                        modelItem.RemissionGuide.RemissionGuideTransportation.driverName = driverNameThird;
						modelItem.RemissionGuide.RemissionGuideTransportation.identificationNumberDriver = identificationNumberDriverThird;


						remissionGuideForControlVehicle.RemissionGuideTransportation.carRegistration = carRegistrationThird;
                        remissionGuideForControlVehicle.RemissionGuideTransportation.driverName = driverNameThird;
						remissionGuideForControlVehicle.RemissionGuideTransportation.identificationNumberDriver = identificationNumberDriverThird;
						RemissionGuide rgTmp = db.RemissionGuide.FirstOrDefault(fod => fod.id == modelItem.id_remissionGuide);

                        rgTmp.hasEntrancePlanctProduction = true;
                        db.RemissionGuide.Attach(rgTmp);
                        db.Entry(rgTmp).State = EntityState.Modified;

                        db.RemissionGuideTransportation.Attach(rgt);
                        db.Entry(rgt).State = EntityState.Modified;

                        remissionGuideForControlVehicle.RemissionGuideTransportationCustomizedInformation = remissionGuideForControlVehicle
                        .RemissionGuideTransportationCustomizedInformation ?? new RemissionGuideTransportationCustomizedInformation();
                        remissionGuideForControlVehicle.RemissionGuideTransportationCustomizedInformation.numberGuide = guideNumber;
                        remissionGuideForControlVehicle.RemissionGuideTransportationCustomizedInformation.SailEntranceThird = sailEntranceThird;

						rgtcti = rgtcti ?? new RemissionGuideTransportationCustomizedInformation();
                        rgtcti.id_RemissionGuide = modelItem.id_remissionGuide;
                        rgtcti.numberGuide = guideNumber;
                        rgtcti.SailEntranceThird = sailEntranceThird;

						db.RemissionGuideTransportationCustomizedInformation.Attach(rgtcti);
                        db.Entry(rgtcti).State = EntityState.Modified;

                        #endregion

                        #region Document
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "161");
                        EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);

                        var document = db.Document.FirstOrDefault(d => d.id_documentOrigen == itemModel.id_remissionGuide && d.id_documentType == documentType.id);

                        if (document == null)
                        {
                            document = new Document
                            {
                                number = GetDocumentNumber(documentType?.id ?? 0),
                                sequential = GetDocumentSequential(documentType?.id ?? 0),
                                emissionDate = DateTime.Now,
                                id_emissionPoint = emissionPoint.id,
                                id_documentType = documentType?.id ?? 0,
                                id_documentState = documentState?.id ?? 0,
                                id_userCreate = ActiveUser.id,
                                dateCreate = DateTime.Now,
                                id_userUpdate = ActiveUser.id,
                                dateUpdate = DateTime.Now,
                                id_documentOrigen = itemModel.id_remissionGuide,
                            };
                            //Actualiza Secuencial
                            if (documentType != null)
                            {
                                documentType.currentNumber = documentType.currentNumber + 1;
                                db.DocumentType.Attach(documentType);
                                db.Entry(documentType).State = EntityState.Modified;
                            }
                            db.Document.Add(document);
                            db.Entry(document).State = EntityState.Added;

                        }
                        else
                        {
                            document.id_userUpdate = ActiveUser.id;
                            document.dateUpdate = DateTime.Now;
                            db.Document.Attach(document);
                            db.Entry(document).State = EntityState.Modified;
                        }

                        #endregion

                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["remissionGuideControlVehicleEntranceThird"] = modelItem;

                        TempData.Keep("remissionGuideControlVehicleEntranceThird");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + modelItem.RemissionGuide.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("remissionGuideControlVehicleEntranceThird");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }
            modelItem.providerNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.providerNameAux;
            modelItem.productionUnitProviderAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.productionUnitProviderAux;
            modelItem.poolNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.poolNameAux;
            modelItem.totalPoundsAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.totalPoundsAux;
            modelItem.zoneNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.zoneNameAux;
            modelItem.siteNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.siteNameAux;
            modelItem.addressTargetAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.addressTargetAux;
            modelItem.shippingTypeAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.shippingTypeAux;
            modelItem.driverNameAux = remissionGuideForControlVehicle.RemissionGuideControlVehicle.driverNameAux;

            modelItem.productionUnitProviderAuxLab = remissionGuideForControlVehicle.RemissionGuideControlVehicle.productionUnitProviderAuxLab;
            TempData.Keep("remissionGuideControlVehicleEntranceThird");

            modelItem.RemissionGuide = remissionGuideForControlVehicle;
            return PartialView("_RemissionGuideControlVehicleEntranceThirdMainFormPartial", modelItem);
        }
        #endregion

        #region "REMISSION GUIDE CONTROL VEHICLE EDIT FORM"
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuideControlVehicleEntranceThird(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(o => o.id == id);

            if (remissionGuide == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("08"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

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
                    RemissionGuideControlVehicle = new RemissionGuideControlVehicle
                    {

                    }

                };
            }
            else
            {
                if (remissionGuide.RemissionGuideControlVehicle == null)
                {
                    remissionGuide.RemissionGuideControlVehicle = remissionGuide.RemissionGuideControlVehicle ?? new Models.RemissionGuideControlVehicle();
                    remissionGuide.RemissionGuideControlVehicle.RemissionGuide = remissionGuide;
                }

            }

            string strPUP = db.Setting.FirstOrDefault(fod => fod.code == "EUPPPRIM")?.value ?? "";
            bool isOwn = remissionGuide?.RemissionGuideTransportation?.isOwn ?? false;

            remissionGuide.RemissionGuideControlVehicle.providerNameAux = remissionGuide?.Provider?.Person?.fullname_businessName ?? "SIN PROVEEDOR";
            remissionGuide.RemissionGuideControlVehicle.productionUnitProviderAux = remissionGuide?.ProductionUnitProvider?.name ?? (strPUP != "" ? "SIN " + strPUP : "SIN CAMARONERA");
            remissionGuide.RemissionGuideControlVehicle.poolNameAux = remissionGuide
                                                                        .RemissionGuideDetail != null ? (string.Join("", remissionGuide
                                                                                                                            .RemissionGuideDetail
                                                                                                                            .Select(s => s.productionUnitProviderPoolreference)
                                                                                                                            .DefaultIfEmpty("").ToArray())) : ("");
            remissionGuide.RemissionGuideControlVehicle.totalPoundsAux = remissionGuide
                                                                        .RemissionGuideDetail != null ? (remissionGuide
                                                                                                            .RemissionGuideDetail
                                                                                                            .Select(s => s.quantityProgrammed).DefaultIfEmpty(0).Sum()) : (0);
            remissionGuide.RemissionGuideControlVehicle.zoneNameAux = remissionGuide?.ProductionUnitProvider?.FishingSite?.FishingZone?.name ?? "";
            remissionGuide.RemissionGuideControlVehicle.siteNameAux = remissionGuide?.ProductionUnitProvider?.FishingSite?.name ?? "";
            remissionGuide.RemissionGuideControlVehicle.addressTargetAux = remissionGuide?.ProductionUnitProvider?.FishingSite?.address ?? "";
            remissionGuide.RemissionGuideControlVehicle.shippingTypeAux = remissionGuide?.PurchaseOrderShippingType?.name ?? "";
            remissionGuide.RemissionGuideControlVehicle.driverNameAux = (!isOwn) ? (remissionGuide?.RemissionGuideTransportation?.driverName ?? "") : (remissionGuide?.RemissionGuideTransportation?.driverNameThird ?? "");

            remissionGuide.RemissionGuideControlVehicle.productionUnitProviderAuxLab = (strPUP != "") ? strPUP + ":" : "Unidad de Producción:";
            remissionGuide.RemissionGuideTransportationCustomizedInformation = remissionGuide.RemissionGuideTransportationCustomizedInformation ?? new RemissionGuideTransportationCustomizedInformation();

            TempData["remissionGuideForControlVehicleEntranceThird"] = remissionGuide;
            TempData.Keep("remissionGuideForControlVehicleEntranceThird");

            return PartialView("_FormEditRemissionGuideControlVehicleEntranceThird", remissionGuide.RemissionGuideControlVehicle);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleCopy(int id)
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
                    isExport = remissionGuide.isExport,
                    route = remissionGuide.route,
                    startAdress = remissionGuide.startAdress
                };
                

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
                            dateUpdate = DateTime.Now
                        };

                        remissionGuideCopy.RemissionGuideDetail.Add(remissionGuideDetail);
                    }
                }

                #endregion
                
                
            }


            TempData["remissionGuide"] = remissionGuideCopy;
            TempData.Keep("remissionGuide");

            return PartialView("_FormEditRemissionGuideControlVehicle", remissionGuideCopy);
        }

        #endregion

        #region "ACTIONS"
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

            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id);
            //int state = remissionGuide.Document.DocumentState.id;
            string state = remissionGuide.Document.DocumentState.code;

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
                    btnAutorize = true,
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

        #region GUIA DE REMISION
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleDetailViewAssignedStaff()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideDetailViewAssignedStaffPartial", remissionGuide.RemissionGuideAssignedStaff.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleDetailViewSecuritySeals()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideDetailViewSecuritySealsPartial", remissionGuide.RemissionGuideSecuritySeal.ToList());
        }
        #endregion

        #region "PAGINATION"
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_remissionGuide)
        {
            TempData.Keep("remissionGuide");

            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }

            int index = db.RemissionGuide.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_remissionGuide);

            var result = new
            {
                maximunPages = db.RemissionGuide.Count(),
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
                TempData.Keep("remissionGuide");
                return PartialView("_RemissionGuideMainFormPartial", remissionGuide);
            }

            TempData.Keep("remissionGuide");

            return PartialView("_RemissionGuideMainFormPartial", new RemissionGuide());
        }
        #endregion

        #region SELECTED DOCUMENT STATE CHANGE 

        [HttpPost, ValidateInput(false)]
        public void ApproveDocuments(int[] ids)
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
        public void AutorizeDocuments(int[] ids)
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

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

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

                                //db.RemissionGuide.Attach(remissionGuide);
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

        #endregion

        #region ENVIO DE CORREO
        //Método para enviar correo en la salida de Vehículos
        private string Envio_Correos_Logistica_Salida_Vehiculos(int id_remissionGuide)
        {
            string mensajeRespuesta = string.Empty;
            DBContext db2 = new DBContext();
            try
            {
                string cuerpoMensaje = string.Empty;
                string asuntoMensaje = string.Empty;
                var remissionGuide = db2.RemissionGuide.FirstOrDefault(fod => fod.id == id_remissionGuide);
                var remissionGuideControlVehicle = db2.RemissionGuideControlVehicle.FirstOrDefault(fod => fod.id_remissionGuide == id_remissionGuide);

                string mailFrom = ConfigurationManager.AppSettings["correoDefaultDesde"];
                string passwordMailFrom = ConfigurationManager.AppSettings["contrasenaCorreoDefault"];
                string passwordMailFromUncrypted = clsEncriptacion1.LeadnjirSimple.Desencriptar(passwordMailFrom);
                string smtpHost = ConfigurationManager.AppSettings["smtpHost"];
                string puertoHost = ConfigurationManager.AppSettings["puertoHost"];
                string mensajePrueba = ConfigurationManager.AppSettings["Pruebas"];

                int puertoHostInt = int.Parse(puertoHost);


                if (remissionGuide != null)
                {

                    //string TextoPrueba = "PRUEBAS";
                    int id_buyer = (int)remissionGuide.id_buyer;
                    int id_provider = (int)remissionGuide.id_providerRemisionGuide;
                    int id_receiver = (int)remissionGuide.id_reciver;
                    int id_driver = (int)remissionGuide?.RemissionGuideTransportation?.id_driver;

                    string destanationMails = string.Empty;

                    string mailBuyer = db2.Person.FirstOrDefault(fod => fod.id == id_buyer)?.email;
                    string mailProvider = db2.Person.FirstOrDefault(fod => fod.id == id_provider)?.email;
                    string nameProvider = db2.Person.FirstOrDefault(fod => fod.id == id_provider)?.fullname_businessName;
                    string recieverName = db2.Person.FirstOrDefault(fod => fod.id == id_receiver)?.fullname_businessName ?? "SIN RECIBIDOR";
                    
                    //Inserto Proveedores
                    if (!(string.IsNullOrEmpty(mailProvider)))
                    {
                        destanationMails = destanationMails + ";" + mailBuyer;
                    }

                    //Lista de Opcionales
                    var mailListLogisticsExit = db.SettingDetail.Where(w => w.Setting.code == "CPESVGR").ToList();

                    if (mailListLogisticsExit != null && mailListLogisticsExit.Count > 0)
                    {
                        foreach (var i in mailListLogisticsExit)
                        {
                            if (i != null)
                            {
                                destanationMails = destanationMails + ";" + i.value;
                            }
                        }
                    }

                    if (!(string.IsNullOrEmpty(destanationMails) && destanationMails.Length > 0))
                    {
                        destanationMails = destanationMails.Substring(1, (destanationMails.Length - 1));
                    }

                    string carRegistration = remissionGuide?.RemissionGuideTransportation?.carRegistration ?? "SIN PLACA";
                    string carMark = remissionGuide?.RemissionGuideTransportation?.Vehicle?.mark ?? "SIN MARCA";
                    string carModel = remissionGuide?.RemissionGuideTransportation?.Vehicle?.model ?? "SIN MODELO";

                    string dateExit = remissionGuideControlVehicle.exitDateProductionBuilding.Value.ToString("dd/MM/yyyy");
                    string hoursTimeExit = remissionGuideControlVehicle.exitTimeProductionBuilding.Value.Hours.ToString();
                    string minutesTimeExit = remissionGuideControlVehicle.exitTimeProductionBuilding.Value.Minutes.ToString();

                    string codeProductionUnitProvider = remissionGuide?.ProductionUnitProvider?.code ?? "SIN CODIGO";
                    string nameProducctionUnitProvider = remissionGuide?.ProductionUnitProvider?.name ?? "SIN NOMBRE";
                    string addressProductionUnitProvider = remissionGuide?.ProductionUnitProvider?.address ?? "SIN DIRECCION";

                    string site = remissionGuide?.ProductionUnitProvider?.FishingSite?.name ?? "SIN SITIO";
                    string zone = remissionGuide?.ProductionUnitProvider?.FishingSite?.FishingZone?.name ?? "SIN ZONA";

                    string driverName = db2.Person.FirstOrDefault(fod => fod.id == id_driver)?.fullname_businessName ?? "SIN CONDUCTOR";
                    string cellPhoneDriver = db2.Person.FirstOrDefault(fod => fod.id == id_driver)?.cellPhoneNumberPerson ?? "";
					string description = remissionGuide?.Document?.description ?? "";

					cuerpoMensaje = string.Empty;
                    cuerpoMensaje = string.Concat("<b>Transporte: Placa: </b>", carRegistration, " <b>Marca: </b>", carMark, " <b>Modelo: </b>", carModel);
                    cuerpoMensaje = cuerpoMensaje + "<br />";
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Fecha Salida: </b>", dateExit, " <b> Hora Salida: </b>", hoursTimeExit , ":", minutesTimeExit);
                    cuerpoMensaje = cuerpoMensaje + "<br />";
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Nombre Proveedor: </b> ", nameProvider, "<b> Camaronera: </b>", nameProducctionUnitProvider);
                    cuerpoMensaje = cuerpoMensaje + "<br />";
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Zona: </b>", zone, "<b> Sitio: </b>", site);
                    cuerpoMensaje = cuerpoMensaje + "<br />";
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Recibe: </b>", recieverName);
                    cuerpoMensaje = cuerpoMensaje + "<br />";
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Chofer: </b>", driverName, "<b> Celular: </b>", cellPhoneDriver);
					cuerpoMensaje = cuerpoMensaje + "<br />";
					cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Descripción: </b>", description);

					if (mensajePrueba == "SI")
                    {
                        asuntoMensaje = "PRUEBAS ";
                    }
                    asuntoMensaje += string.Concat("Salida de Vehículo, Guia de Remisión: ", remissionGuide.Document.number);

                    mensajeRespuesta = clsCorreoElectronico.EnviarCorreoElectronico(destanationMails, mailFrom, asuntoMensaje, smtpHost, puertoHostInt, passwordMailFromUncrypted
                                        , cuerpoMensaje, ';');


                }
                
            }
            catch (Exception ex)
            {
                Console.Write("Error: " + ex.Message);
            }
            db2.Dispose();
            return mensajeRespuesta;
        }

        #endregion

        #region "BATCH EDIT"
        //Security Seals
        #region"BATCH EDIT SECURITY SEALS"

        public ActionResult RemissionGuideControlVehicleSecuritySealsBatchEditingUpdateModel(MVCxGridViewBatchUpdateValues<RemissionGuideSecuritySeal, int> updateValues)
        {
            TempData.Keep("remissionGuideForControlVehicle");
            List<RemissionGuideSecuritySeal> lstSecuritySeal = (List<RemissionGuideSecuritySeal>)TempData["securitySealsList"];
            foreach (var securitySeal in updateValues.Update)
            {
                if (updateValues.IsValid(securitySeal))
                    UpdateRGCVSecuritySealBatchDetail(securitySeal, updateValues);
            }
            TempData["securitySealsList"] = lstSecuritySeal;
            TempData.Keep("securitySealsList");
            return PartialView("_RemissionGuideControlVehicleSecuritySealsPartial", lstSecuritySeal);
        }

        public void UpdateRGCVSecuritySealBatchDetail(RemissionGuideSecuritySeal securitySeal, MVCxGridViewBatchUpdateValues<RemissionGuideSecuritySeal, int> updateValues)
        {
            List<RemissionGuideSecuritySeal> lstSecuritySeal = (List<RemissionGuideSecuritySeal>)TempData["securitySealsList"];

            if (securitySeal != null)
            {
                var securitySealTmp = lstSecuritySeal.FirstOrDefault(fod => fod.id == securitySeal.id);

                securitySealTmp.id_exitState = securitySeal.id_exitState;
                this.UpdateModel(securitySealTmp);
            }
            
        }
        #endregion

        #endregion
    }
}