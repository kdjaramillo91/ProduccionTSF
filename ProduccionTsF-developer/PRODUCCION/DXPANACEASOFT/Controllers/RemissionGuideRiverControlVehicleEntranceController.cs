using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using System.IO;
using System.Configuration;
using DevExpress.Web.Mvc;
using static DXPANACEASOFT.Controllers.RemissionGuideRiverControlVehicleController;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class RemissionGuideRiverControlVehicleEntranceController : DefaultController
    {
        // POST: RemissionGuideRiverControlVehicleEntrance
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region "REMISSION GUIDE RIVER CONTROL VEHICLE ENTRANCE FILTER RESULTS"
        [HttpPost]
        public ActionResult RemissionGuideRiverControlVehicleEntranceResults(RemissionGuideRiver remissionGuideRiver,
                                          Document document,
                                          DateTime? startEmissionDate, DateTime? endEmissionDate,
                                          DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                          DateTime? startDespachureDate, DateTime? endDespachureDate,
                                          DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                          DateTime? startentranceDateProductionBuilding, DateTime? endentranceDateProductionBuilding,
                                          int[] items, int[] businessOportunities)
        {
            //var model = db.RemissionGuideRiver.Where(w => (w.RemissionGuideRiverControlVehicle.hasExitPlanctProduction == true &&
            //                                                w.RemissionGuideRiverControlVehicle.hasEntrancePlanctProduction == true &&
                                                          
            //                                                w.Document.DocumentState.code == "06")).ToList();

            db.Database.CommandTimeout = 1200;
            List<RemissionGuideRiverEntradaDTO> model = db.Database.SqlQuery<RemissionGuideRiverEntradaDTO>("exec spc_Consultar_GuiasRemisionFluvialEntradaDTO_StoredProcedure").ToList();

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

            if (startEmissionDate != null && endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.FechaEmision.Date) <= 0 && DateTime.Compare(o.FechaEmision.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }
            if (startexitDateProductionBuilding != null && endexitDateProductionBuilding != null)
            {
                model = model.Where(o => (o.FechaSalida != null) && 
                (DateTime.Compare(startexitDateProductionBuilding.Value.Date, o.FechaSalida.Value.Date) <= 0 && 
                DateTime.Compare(o.FechaSalida.Value.Date, endexitDateProductionBuilding.Value.Date) <= 0)).ToList();
            }
            if (startentranceDateProductionBuilding != null && endentranceDateProductionBuilding != null)
            {
                model = model.Where(o => (o.FechaEntrada != null) &&
                (DateTime.Compare(startentranceDateProductionBuilding.Value.Date, o.FechaEntrada.Value.Date) <= 0 &&
                DateTime.Compare(o.FechaEntrada.Value.Date, endentranceDateProductionBuilding.Value.Date) <= 0)).ToList();
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

            return PartialView("_RemissionGuideRiverControlVehicleEntranceResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }

        public class RemissionGuideRiverEntradaDTO
        {
            public int id { get; set; }
            public string Numero { get; set; }
            public string Referencia { get; set; }
            public System.DateTime FechaEmision { get; set; }
            public System.DateTime? FechaAutorizacion { get; set; }
            public string NumeroAutorizacion { get; set; }
            public string AccessKey { get; set; }
            public int iddocumentState { get; set; }
            public string Proceso { get; set; }
            public string Proveedor { get; set; }
            public string UnidadProduccion { get; set; }
            public string Zona { get; set; }
            public string Sitio { get; set; }
            public string Chofer { get; set; }
            public string Placa { get; set; }
            public Nullable<System.DateTime> FechaSalida { get; set; }
            public Nullable<System.TimeSpan> HoraSalida { get; set; }
            public Nullable<System.DateTime> FechaEntrada { get; set; }
            public Nullable<System.TimeSpan> HoraEntrada { get; set; }
            public string UsuarioCreacion { get; set; }
            public DateTime? FechaCreacion { get; set; }
            public string UsuarioModificacion { get; set; }
            public DateTime? FechaModificacion { get; set; }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverResults()
        {
            var model = db.RemissionGuideRiver.Where(w => (w.RemissionGuideRiverControlVehicle.hasExitPlanctProduction == true && 
                                                w.RemissionGuideRiverControlVehicle.hasEntrancePlanctProduction != true) && 
                                                ( w.Document.DocumentState.code == "06" || w.Document.DocumentState.code == "09"));
            return PartialView("_RemissionGuideRiverCVEHeaderResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverPartial()
        {
            var model = db.RemissionGuideRiver.Where(w => (w.RemissionGuideRiverControlVehicle.hasExitPlanctProduction == true && 
                                                    w.RemissionGuideRiverControlVehicle.hasEntrancePlanctProduction != true) &&
                                                    (w.Document.DocumentState.code == "06" || w.Document.DocumentState.code == "09"));
            return PartialView("_RemissionGuideRiverCVEResultsPartial",model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverPartialEntrance()
        {
            //var model = db.RemissionGuideRiver.Where(w => (w.RemissionGuideRiverControlVehicle.hasExitPlanctProduction == true && 
            //                                                w.RemissionGuideRiverControlVehicle.hasEntrancePlanctProduction == true && 
            //                                                w.Document.DocumentState.code == "06" || w.Document.DocumentState.code == "03"));

            var model = (TempData["model"] as List<RemissionGuideRiverEntradaDTO>);
            model = model ?? new List<RemissionGuideRiverEntradaDTO>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideRiverControlVehicleEntrancePartial", model.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        #region "REMISSION GUIDE RIVER CONTROL VEHICLE ENTRANCE HEADER"

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehicleEntrancePartial()
        {
            var model = (TempData["model"] as List<RemissionGuideRiver>);
            model = model ?? new List<RemissionGuideRiver>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideRiverControlVehicleEntrancePartial", model.OrderByDescending(r => r.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehicleEntrancePartialAddNew(bool approve, string entranceTimePUP, string exitTimePUP, string entranceTimeBuilding, RemissionGuideRiverControlVehicle itemModel)
        {
            TempData.Keep("remissionGuideRiverForControlVehicleEntrance");
            if (TempData["securitySealsEntranceList"] != null)
            {
                TempData.Keep("securitySealsEntranceList");
            }
            RemissionGuideRiver remissionGuideRiverForControlVehicle = (RemissionGuideRiver)TempData["remissionGuideRiverForControlVehicleEntrance"];
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(fod => fod.id == remissionGuideRiverForControlVehicle.id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    itemModel.id_remissionGuideRiver = remissionGuideRiverForControlVehicle.id;

                    remissionGuideRiver.hasEntrancePlanctProduction = true;

                    db.RemissionGuideRiver.Attach(remissionGuideRiver);
                    db.Entry(remissionGuideRiver).State = EntityState.Modified;

                    #region Document
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "163");
                    EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);

                    var document = db.Document.FirstOrDefault(d => d.id_documentOrigen == itemModel.id_remissionGuideRiver && d.id_documentType == documentType.id);

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
                            id_documentOrigen = itemModel.id_remissionGuideRiver,
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

                    //Update SecuritySeals
                    if (TempData["securitySealsEntranceList"] != null)
                    {
                        List<RemissionGuideRiverSecuritySeal> lstSecuritySeal = (List<RemissionGuideRiverSecuritySeal>)TempData["securitySealsEntranceList"];
                        var lstSecuritySealDB = db.RemissionGuideRiverSecuritySeal.Where(w => w.id_remissionGuideRiver == remissionGuideRiverForControlVehicle.id).ToList();
                        foreach (var securitySeal in lstSecuritySeal)
                        {
                            var securitySealTmp = lstSecuritySealDB.FirstOrDefault(fod => fod.id == securitySeal.id);
                            securitySealTmp.id_arrivalState = securitySeal.id_arrivalState;
                            db.RemissionGuideRiverSecuritySeal.Attach(securitySealTmp);
                            db.Entry(securitySealTmp).State = EntityState.Modified;
                        }
                    }

                    RemissionGuideRiverControlVehicle rgcvTmp = db.RemissionGuideRiverControlVehicle.FirstOrDefault(fod => fod.id_remissionGuideRiver == remissionGuideRiverForControlVehicle.id);

                    
                    if (!string.IsNullOrEmpty(entranceTimePUP)) rgcvTmp.entranceTimeProductionUnitProviderBuilding = TimeSpan.Parse(entranceTimePUP);
                    if (!string.IsNullOrEmpty(exitTimePUP)) rgcvTmp.exitTimeProductionUnitProviderBuilding = TimeSpan.Parse(exitTimePUP);
                    if (!string.IsNullOrEmpty(entranceTimeBuilding)) rgcvTmp.entranceTimeProductionBuilding = TimeSpan.Parse(entranceTimeBuilding);

                    rgcvTmp.hasEntrancePlanctProduction = true;
                    rgcvTmp.ObservationEntrance = itemModel.ObservationEntrance;

                    if (approve){ }

                    db.RemissionGuideRiverControlVehicle.Attach(rgcvTmp);
                    db.SaveChanges();
                    trans.Commit();

                    TempData["remissionGuideRiverForControlVehicleEntrance"] = remissionGuideRiver;
                    TempData.Keep("remissionGuideRiverForControlVehicleEntrance");

                    ViewData["EditMessage"] = SuccessMessage("Control de Entrada para Guía de Remisión: " + remissionGuideRiverForControlVehicle.Document.number + " se ha guardado correctamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuideRiverForControlVehicle");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            itemModel.RemissionGuideRiver = remissionGuideRiverForControlVehicle;
            return PartialView("_RemissionGuideRiverControlVehicleEntranceMainFormPartial", itemModel);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehicleEntrancePartialUpdate(bool approve, string entranceTimePUP, string exitTimePUP, string entranceTimeBuilding, RemissionGuideRiverControlVehicle itemModel)
        {

            TempData.Keep("remissionGuideRiverForControlVehicleEntrance");
            if (TempData["securitySealsEntranceList"] != null)
            {
                TempData.Keep("securitySealsEntranceList");
            }
            RemissionGuideRiver remissionGuideRiverForControlVehicle = (RemissionGuideRiver)TempData["remissionGuideRiverForControlVehicleEntrance"];
            RemissionGuideRiverControlVehicle modelItem = db.RemissionGuideRiverControlVehicle.FirstOrDefault(r => r.id_remissionGuideRiver == remissionGuideRiverForControlVehicle.id);

            if (itemModel != null && modelItem !=null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region "REMISSION GUIDE RIVER CONTROL VEHICLE"

                        modelItem.entranceDateProductionBuilding = itemModel.entranceDateProductionBuilding;
                        modelItem.hasEntrancePlanctProduction = true;

                        RemissionGuideRiver rgTmp = db.RemissionGuideRiver.FirstOrDefault(fod => fod.id == modelItem.id_remissionGuideRiver);

                        rgTmp.hasEntrancePlanctProduction = true;
                        db.RemissionGuideRiver.Attach(rgTmp);
                        db.Entry(rgTmp).State = EntityState.Modified;

                        if (!string.IsNullOrEmpty(entranceTimePUP)) modelItem.entranceTimeProductionUnitProviderBuilding = TimeSpan.Parse(entranceTimePUP);
                        if (!string.IsNullOrEmpty(exitTimePUP)) modelItem.exitTimeProductionUnitProviderBuilding = TimeSpan.Parse(exitTimePUP);
                        if (!string.IsNullOrEmpty(entranceTimeBuilding)) modelItem.entranceTimeProductionBuilding = TimeSpan.Parse(entranceTimeBuilding);


                        modelItem.entranceDateProductionUnitProviderBuilding = itemModel.entranceDateProductionUnitProviderBuilding;
                        modelItem.exitDateProductionUnitProviderBuilding = itemModel.exitDateProductionUnitProviderBuilding;
                        modelItem.entranceDateProductionBuilding = itemModel.entranceDateProductionBuilding;

                        modelItem.ObservationEntrance = itemModel.ObservationEntrance;

                        #endregion

                        #region Document
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "163");
                        EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);

                        var document = db.Document.FirstOrDefault(d => d.id_documentOrigen == itemModel.id_remissionGuideRiver && d.id_documentType == documentType.id);

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
                                id_documentOrigen = itemModel.id_remissionGuideRiver,
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

                        //Update SecuritySeals
                        if (TempData["securitySealsEntranceList"] != null)
                        {
                            List<RemissionGuideRiverSecuritySeal> lstSecuritySeal = (List<RemissionGuideRiverSecuritySeal>)TempData["securitySealsEntranceList"];
                            var lstSecuritySealDB = db.RemissionGuideRiverSecuritySeal.Where(w => w.id_remissionGuideRiver == remissionGuideRiverForControlVehicle.id).ToList();
                            foreach (var securitySeal in lstSecuritySeal)
                            {
                                var securitySealTmp = lstSecuritySealDB.FirstOrDefault(fod => fod.id == securitySeal.id);
                                securitySealTmp.id_arrivalState = securitySeal.id_arrivalState;
                                db.RemissionGuideRiverSecuritySeal.Attach(securitySealTmp);
                                db.Entry(securitySealTmp).State = EntityState.Modified;
                            }
                        }

                        db.Entry(modelItem).State = EntityState.Modified;
                        //NO ELIMINAR
                        modelItem.providerNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.providerNameAux;
                        modelItem.productionUnitProviderAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.productionUnitProviderAux;

                        modelItem.totalPoundsAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.totalPoundsAux;
                        modelItem.zoneNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.zoneNameAux;
                        modelItem.siteNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.siteNameAux;
                        modelItem.addressTargetAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.addressTargetAux;
                        modelItem.shippingTypeAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.shippingTypeAux;
                        modelItem.driverNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.driverNameAux;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["remissionGuideRiverControlVehicleEntrance"] = modelItem;

                        TempData.Keep("remissionGuideRiverControlVehicleEntrance");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + modelItem.RemissionGuideRiver.Document.number + " guardada exitosamente");
                    }


                    catch (Exception e)
                    {
                        TempData.Keep("remissionGuideRiverControlVehicleEntrance");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("remissionGuideRiverControlVehicleEntrance");

            modelItem.RemissionGuideRiver = remissionGuideRiverForControlVehicle;
            return PartialView("_RemissionGuideRiverControlVehicleEntranceMainFormPartial", modelItem);
        }


        #endregion

        #region "REMISSION GUIDE CONTROL VEHICLE EDIT FORM"
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuideRiverControlVehicleEntrance(int id)
        {
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(o => o.id == id);

            if (remissionGuideRiver == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("08"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

                remissionGuideRiver = new RemissionGuideRiver
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
                    //arrivalDate = DateTime.Now,
                    //returnDate = DateTime.Now,
                    startAdress = ActiveSucursal.address,
                    RemissionGuideRiverDetail = new List<RemissionGuideRiverDetail>(),
                    RemissionGuideRiverDispatchMaterial = new List<RemissionGuideRiverDispatchMaterial>(),
                    //isInternal = false,
                    RemissionGuideRiverControlVehicle = new RemissionGuideRiverControlVehicle
                    {

                    }

                };
            }
            else
            {
                if (remissionGuideRiver.RemissionGuideRiverControlVehicle == null)
                {
                    remissionGuideRiver.RemissionGuideRiverControlVehicle = remissionGuideRiver.RemissionGuideRiverControlVehicle ?? new Models.RemissionGuideRiverControlVehicle();
                    remissionGuideRiver.RemissionGuideRiverControlVehicle.RemissionGuideRiver = remissionGuideRiver;
                    //remissionGuideRiver.RemissionGuideRiverControlVehicle.id_remissionGuideRiver = remissionGuideRiver.id;
                }

            }
            string strPUP = db.Setting.FirstOrDefault(fod => fod.code == "EUPPPRIM")?.value ?? "";

            //bool isOwn = remissionGuideRiver?.RemissionGuideRiverTransportation?.isOwn ?? false;

            remissionGuideRiver.RemissionGuideRiverControlVehicle.providerNameAux = remissionGuideRiver?.Provider?.Person?.fullname_businessName ?? "SIN PROVEEDOR";
            remissionGuideRiver.RemissionGuideRiverControlVehicle.productionUnitProviderAux = remissionGuideRiver?.ProductionUnitProvider?.name ?? (strPUP != "" ? "SIN " + strPUP : "SIN CAMARONERA");

            remissionGuideRiver.RemissionGuideRiverControlVehicle.totalPoundsAux = remissionGuideRiver
                                                                        .RemissionGuideRiverDetail != null ? (remissionGuideRiver
                                                                                                            .RemissionGuideRiverDetail
                                                                                                            .Select(s => s.quantityProgrammed).DefaultIfEmpty(0).Sum()) : (0);
            remissionGuideRiver.RemissionGuideRiverControlVehicle.zoneNameAux = remissionGuideRiver?.RemissionGuideRiverTransportation?.FishingSite?.name ?? "";
            remissionGuideRiver.RemissionGuideRiverControlVehicle.siteNameAux = remissionGuideRiver?.RemissionGuideRiverTransportation?.FishingSite?.FishingZone?.name ?? "";
            remissionGuideRiver.RemissionGuideRiverControlVehicle.addressTargetAux = remissionGuideRiver?.ProductionUnitProvider?.FishingSite?.address ?? "";
            remissionGuideRiver.RemissionGuideRiverControlVehicle.shippingTypeAux = remissionGuideRiver?.PurchaseOrderShippingType?.name ?? "";
            remissionGuideRiver.RemissionGuideRiverControlVehicle.driverNameAux = remissionGuideRiver?.RemissionGuideRiverTransportation?.driverName ?? "";

            remissionGuideRiver.RemissionGuideRiverControlVehicle.productionUnitProviderAuxLab = (strPUP != "") ? strPUP + ":" : "Unidad de Producción:";

            TempData["remissionGuideRiverForControlVehicleEntrance"] = remissionGuideRiver;
            TempData.Keep("remissionGuideRiverForControlVehicleEntrance");

            return PartialView("_FormEditRemissionGuideRiverControlVehicleEntrance", remissionGuideRiver.RemissionGuideRiverControlVehicle);
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

            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);
            //int state = remissionGuideRiver.Document.DocumentState.id;
            string state = remissionGuideRiver.Document.DocumentState.code;

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

        #region "PAGINATION"
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_remissionGuideRiver)
        {
            TempData.Keep("remissionGuideRiver");

            int index = db.RemissionGuideRiver.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_remissionGuideRiver);

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
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (remissionGuideRiver != null)
            {
                TempData["remissionGuideRiver"] = remissionGuideRiver;
                TempData.Keep("remissionGuideRiver");
                return PartialView("_RemissionGuideRiverMainFormPartial", remissionGuideRiver);
            }

            TempData.Keep("remissionGuideRiver");

            return PartialView("_RemissionGuideRiverMainFormPartial", new RemissionGuideRiver());
        }
        #endregion

        #region SELECTED DOCUMENT STATE CHANGE 

        [HttpPost, ValidateInput(false)]
        public void ApproveDocuments(int[] ids)
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
                            RemissionGuideRiver remissionGuideRiver = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

                            if (remissionGuideRiver != null && documentState != null)
                            {
                                remissionGuideRiver.Document.id_documentState = documentState.id;
                                remissionGuideRiver.Document.DocumentState = documentState;

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
                            RemissionGuideRiver remissionGuideRiver = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

                            if (remissionGuideRiver != null && documentState != null)
                            {
                                remissionGuideRiver.Document.id_documentState = documentState.id;
                                remissionGuideRiver.Document.DocumentState = documentState;

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
                            RemissionGuideRiver remissionGuideRiver = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

                            if (remissionGuideRiver != null && documentState != null)
                            {
                                remissionGuideRiver.Document.id_documentState = documentState.id;
                                remissionGuideRiver.Document.DocumentState = documentState;

                                //db.RemissionGuideRiver.Attach(remissionGuideRiver);
                                //db.Entry(remissionGuideRiver).State = EntityState.Modified;
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
                            RemissionGuideRiver remissionGuideRiver = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

                            if (remissionGuideRiver != null && documentState != null)
                            {
                                remissionGuideRiver.Document.id_documentState = documentState.id;
                                remissionGuideRiver.Document.DocumentState = documentState;

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
                            RemissionGuideRiver remissionGuideRiver = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                            if (remissionGuideRiver != null && documentState != null)
                            {
                                remissionGuideRiver.Document.id_documentState = documentState.id;
                                remissionGuideRiver.Document.DocumentState = documentState;

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

        #region"Auxiliar"
        #region"Security Seals"
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehicleEntranceDetailViewSecuritySeals()
        {
            int id_remissionGuideRiver = (Request.Params["id_remissionGuideRiver"] != null && Request.Params["id_remissionGuideRiver"] != "") ? int.Parse(Request.Params["id_remissionGuideRiver"]) : -1;
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id_remissionGuideRiver);
            return PartialView("_RemissionGuideRiverDetailViewSecuritySealsPartial", remissionGuideRiver.RemissionGuideRiverSecuritySeal.ToList());
        }
        #endregion
        #endregion

        #region"BATCH EDIT"
        //Security Seals
        #region"BATCH EDIT SECURITY SEALS"

        public ActionResult RemissionGuideRiverControlVehicleEntranceSecuritySealsBatchEditingUpdateModel(MVCxGridViewBatchUpdateValues<RemissionGuideRiverSecuritySeal, int> updateValues)
        {
            TempData.Keep("remissionGuideRiverForControlVehicleEntrance");
            List<RemissionGuideRiverSecuritySeal> lstSecuritySeal = (List<RemissionGuideRiverSecuritySeal>)TempData["securitySealsEntranceList"];
            foreach (var securitySeal in updateValues.Update)
            {
                if (updateValues.IsValid(securitySeal))
                    UpdateRGCVSecuritySealBatchDetail(securitySeal, updateValues);
            }
            TempData["securitySealsEntranceList"] = lstSecuritySeal;
            TempData.Keep("securitySealsEntranceList");
            return PartialView("_RemissionGuideRiverControlVehicleEntranceSecuritySealsPartial", lstSecuritySeal);
        }

        public void UpdateRGCVSecuritySealBatchDetail(RemissionGuideRiverSecuritySeal securitySeal, MVCxGridViewBatchUpdateValues<RemissionGuideRiverSecuritySeal, int> updateValues)
        {
            List<RemissionGuideRiverSecuritySeal> lstSecuritySeal = (List<RemissionGuideRiverSecuritySeal>)TempData["securitySealsEntranceList"];

            if (securitySeal != null)
            {
                var securitySealTmp = lstSecuritySeal.FirstOrDefault(fod => fod.id == securitySeal.id);

                securitySealTmp.id_arrivalState = securitySeal.id_arrivalState;
                this.UpdateModel(securitySealTmp);
            }

        }
        #endregion

        #endregion
    }
}