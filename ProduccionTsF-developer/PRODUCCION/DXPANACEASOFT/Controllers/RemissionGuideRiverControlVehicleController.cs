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
using static DXPANACEASOFT.Controllers.RemissionGuideControlVehicleExitThirdController;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class RemissionGuideRiverControlVehicleController : DefaultController
    {
        // POST: RemissionGuideControlVehicle
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region "REMISSION GUIDE RIVER CONTROL VEHICLE FILTER RESULTS"
        [HttpPost]
        public ActionResult RemissionGuideRiverControlVehicleResults(RemissionGuideRiver remissionGuideRiver,
                                          Document document,
                                          DateTime? startEmissionDate, DateTime? endEmissionDate,
                                          DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                          DateTime? startDespachureDate, DateTime? endDespachureDate,
                                          DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                          int[] items, int[] businessOportunities)
        {
            //var model = db.RemissionGuideRiver
            //                .Where(w => w.RemissionGuideRiverControlVehicle != null 
            //                            && w.RemissionGuideRiverControlVehicle.hasExitPlanctProduction == true).ToList();

            db.Database.CommandTimeout = 1200;
            List<RemissionGuideRiverSalidaDTO> model = db.Database.SqlQuery<RemissionGuideRiverSalidaDTO>("exec spc_Consultar_GuiasRemisionFluvialSalidaDTO_StoredProcedure").ToList();


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
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.FechaEmision.Date) <= 0 
                && DateTime.Compare(o.FechaEmision.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            if (startAuthorizationDate != null && endAuthorizationDate != null)
            {
                model = model.Where(o => o.FechaAutorizacion != null 
                && DateTime.Compare(startAuthorizationDate.Value.Date, o.FechaAutorizacion.Value.Date) <= 0 
                && DateTime.Compare(o.FechaAutorizacion.Value.Date, endAuthorizationDate.Value.Date) <= 0).ToList();
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

            return PartialView("_RemissionGuideRiverControlVehicleResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }

        public class RemissionGuideRiverSalidaDTO
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
            public string MedioTransporte { get; set; }
            public string Proveedor { get; set; }
            public string UnidadProduccion { get; set; }
            public string Zona { get; set; }
            public string Sitio { get; set; }
            public string Chofer { get; set; }
            public string Placa { get; set; }
            public Nullable<System.DateTime> FechaSalida { get; set; }
            public Nullable<System.TimeSpan> HoraSalida { get; set; }
            public string UsuarioCreacion { get; set; }
            public DateTime? FechaCreacion { get; set; }
            public string UsuarioModificacion { get; set; }
            public DateTime? FechaModificacion { get; set; }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverResults()
        {
            var model = db.RemissionGuideRiver.Where(w => (w.PurchaseOrderShippingType.code == "T" 
                                        || w.PurchaseOrderShippingType.code == "M"
                                        || w.PurchaseOrderShippingType.code == "TF") 
                                        && w.Document.DocumentState.code == "03" 
                                        && w.RemissionGuideRiverControlVehicle == null).ToList();

            //Listo todos los que tienen staff con valores
            var lstStaffWithPrice = db.RemissionGuideRiverAssignedStaff
                                        .Where(w => w.viaticPrice > 0)
                                        .Select(s => s.id_remissionGuideRiver).Distinct().ToList();

            // Listo todos los que no tienen ni staff ni anticipo
            var lstRGOK = model.Where(w => !lstStaffWithPrice.Contains(w.id) 
            && (w.RemissionGuideRiverTransportation.advancePrice ?? 0) <= 0);

            var id_paymentState = db.tbsysCatalogState
                        .FirstOrDefault(w => w.TController.name == "RemissionGuideInternControl"
                        && w.codeClasification == "01" && w.codeState == "02").id; //codeState = 02 Pagado


            var lstFinal = model.Where(w => ((w.RemissionGuideRiverTransportation.advancePrice ?? 0) > 0
                                || w.RemissionGuideRiverAssignedStaff.Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() > 0)
                                && w.id_tbsysCatalogState == id_paymentState).ToList();

            List<RemissionGuideRiver> RgFinalLst = new List<RemissionGuideRiver>();
            RgFinalLst.AddRange(lstRGOK.Concat(lstFinal));

            TempData["modelBegin"] = model;
            TempData.Keep("modelBegin");
            
            return PartialView("_RemissionGuideRiverHeaderResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverPartial()
        {
            List<RemissionGuideRiver> RgFinalLst = new List<RemissionGuideRiver>();

            if (TempData["modelBegin"] != null)
            {
                RgFinalLst = (TempData["modelBegin"] as List<RemissionGuideRiver>);

            }
            else
            {
                RgFinalLst = db.RemissionGuideRiver.Where(w => (w.PurchaseOrderShippingType.code == "T"
                                        || w.PurchaseOrderShippingType.code == "M"
                                        || w.PurchaseOrderShippingType.code == "TF")
                                        && w.Document.DocumentState.code == "03"
                                        && w.RemissionGuideRiverControlVehicle == null).ToList();

                ////Listo todos los que tienen staff con valores
                //var lstStaffWithPrice = db.RemissionGuideAssignedStaff.Where(w => w.viaticPrice > 0).Select(s => s.id_remissionGuide).Distinct().ToList();

                //// Listo todos los que no tienen ni staff ni anticipo
                //var lstRGOK = model.Where(w => !lstStaffWithPrice.Contains(w.id)
                //&& (w.RemissionGuideRiverTransportation.advancePrice ?? 0) <= 0);

                //var id_paymentState = db.tbsysCatalogState
                //            .FirstOrDefault(w => w.TController.name == "RemissionGuideInternControl"
                //            && w.codeClasification == "01" && w.codeState == "02").id; //codeState = 02 Pagado


                //var lstFinal = model.Where(w => ((w.RemissionGuideRiverTransportation.advancePrice ?? 0) > 0
                //                    || w.RemissionGuideRiverAssignedStaff.Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() > 0)
                //                    && w.id_tbsysCatalogState == id_paymentState);
                
                //RgFinalLst.AddRange(lstRGOK.Concat(lstFinal));
            }

            TempData["modelBegin"] = RgFinalLst;
            TempData.Keep("modelBegin");

            return PartialView("_RemissionGuideRiverResultsPartial", RgFinalLst.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        #region "REMISSION GUIDE RIVER CONTROL VEHICLE HEADER"

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehiclePartial()
        {
            var model = (TempData["model"] as List<RemissionGuideRiverSalidaDTO>);
            model = model ?? new List<RemissionGuideRiverSalidaDTO>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideRiverControlVehiclePartial", model.OrderByDescending(r => r.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehiclePartialAddNew(bool approve, string exitTimeBuilding, RemissionGuideRiverControlVehicle itemModel)
        {
            bool registroInsertado = false;
            TempData.Keep("remissionGuideRiverForControlVehicle");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuideRiver remissionGuideRiverForControlVehicle = (RemissionGuideRiver)TempData["remissionGuideRiverForControlVehicle"];
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(fod => fod.id == remissionGuideRiverForControlVehicle.id);

            string ruta = ConfigurationManager.AppSettings["rutaXmlFEX"];
            string rutaA1Firmar = ConfigurationManager.AppSettings["rutaXmlA1Firmar"];

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    itemModel.id_remissionGuideRiver = remissionGuideRiverForControlVehicle.id;

                    remissionGuideRiver.hasExitPlanctProduction = true;
                    itemModel.hasExitPlanctProduction = true;

                    db.RemissionGuideRiver.Attach(remissionGuideRiver);
                    db.Entry(remissionGuideRiver).State = EntityState.Modified;

                    //Update SecuritySeals
                    if (TempData["securitySealsList"] != null)
                    {
                        List<RemissionGuideRiverSecuritySeal> lstSecuritySeal = (List<RemissionGuideRiverSecuritySeal>)TempData["securitySealsList"];
                        var lstSecuritySealDB = db.RemissionGuideRiverSecuritySeal.Where(w => w.id_remissionGuideRiver == remissionGuideRiverForControlVehicle.id).ToList();
                        foreach (var securitySeal in lstSecuritySeal)
                        {
                            var securitySealTmp = lstSecuritySealDB.FirstOrDefault(fod => fod.id == securitySeal.id);
                            securitySealTmp.id_exitState = securitySeal.id_exitState;
                            db.RemissionGuideRiverSecuritySeal.Attach(securitySealTmp);
                            db.Entry(securitySealTmp).State = EntityState.Modified;
                        }
                    }

                    #region Document
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "164");
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

                    if (!string.IsNullOrEmpty(exitTimeBuilding)) itemModel.exitTimeProductionBuilding = TimeSpan.Parse(exitTimeBuilding);

                    if (approve)
                    {

                    }
                    //OJO CON ESTO
                    string answer = Services.ServiceLogistics.AutorizeRemissionGuideRiver(db, remissionGuideRiver.id, this.ActiveCompanyId, ruta, rutaA1Firmar);

                    db.RemissionGuideRiverControlVehicle.Add(itemModel);
                    db.SaveChanges();
                    trans.Commit();

                    TempData["remissionGuideRiverForControlVehicle"] = remissionGuideRiver;
                    TempData.Keep("remissionGuideRiverForControlVehicle");
                    

                    ViewData["EditMessage"] = SuccessMessage("Control de Salida para Guía de Remisión: " + remissionGuideRiverForControlVehicle.Document.number + " se ha guardado correctamente");
                    registroInsertado = true;
                    
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuideRiverForControlVehicle");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    //ViewData["EditError"] = ErrorMessage();
                    trans.Rollback();
                    registroInsertado = false;
                }
            }
            //TempData.Keep("remissionGuide");
            itemModel.RemissionGuideRiver = remissionGuideRiverForControlVehicle;

            if (registroInsertado == true)
            {
                string respuestaCorreo = string.Empty;
            
                respuestaCorreo = Envio_Correos_Logistica_Salida_Vehiculos(itemModel.id_remissionGuideRiver);

                if (respuestaCorreo == "OK")
                {
                    DBContext db3 = new DBContext();
                    RemissionGuideRiverControlVehicle rgcvSETmp = null;
                    using (DbContextTransaction trans = db3.Database.BeginTransaction())
                    {
                        try
                        {
                            rgcvSETmp = db3.RemissionGuideRiverControlVehicle.FirstOrDefault(fod => fod.id_remissionGuideRiver == remissionGuideRiverForControlVehicle.id);
                            if (rgcvSETmp != null)
                            {
                                rgcvSETmp.hasSentEmail = true;
                                db3.RemissionGuideRiverControlVehicle.Attach(rgcvSETmp);
                                db3.Entry(rgcvSETmp).State = EntityState.Modified;
                                db3.SaveChanges();
                                trans.Commit();

                            }
                        }
                        catch //(Exception ex)
                        {

                        }
                    }
                    db3.Dispose();

                }
            }

            //NO ELIMINAR


            itemModel.providerNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.providerNameAux;
            itemModel.productionUnitProviderAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.productionUnitProviderAux;

            itemModel.totalPoundsAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.totalPoundsAux;
            itemModel.zoneNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.zoneNameAux;
            itemModel.siteNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.siteNameAux;
            itemModel.addressTargetAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.addressTargetAux;
            itemModel.shippingTypeAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.shippingTypeAux;
            itemModel.driverNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.driverNameAux;

            //itemModel.productionUnitProviderAuxLab = remissionGuideForControlVehicle.RemissionGuideControlVehicle.productionUnitProviderAuxLab;

            return PartialView("_RemissionGuideRiverControlVehicleMainFormPartial", itemModel);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehiclePartialUpdate(bool approve, string exitTimeBuilding, RemissionGuideRiverControlVehicle itemModel)
        {
            TempData.Keep("remissionGuideRiverForControlVehicle");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuideRiver remissionGuideRiverForControlVehicle = (RemissionGuideRiver)TempData["remissionGuideRiverForControlVehicle"];

            RemissionGuideRiverControlVehicle modelItem = db.RemissionGuideRiverControlVehicle
                                                            .FirstOrDefault(r => r.id_remissionGuideRiver == remissionGuideRiverForControlVehicle.id);

            if (itemModel != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region "REMISSION GUIDE RIVER CONTROL VEHICLE"

                        modelItem.exitDateProductionBuilding = itemModel.exitDateProductionBuilding;
                        modelItem.hasExitPlanctProduction = true;

                        if (!string.IsNullOrEmpty(exitTimeBuilding)) modelItem.exitTimeProductionBuilding = TimeSpan.Parse(exitTimeBuilding);

                        modelItem.ObservationExit = itemModel.ObservationExit;

                        RemissionGuideRiver rgTmp = db.RemissionGuideRiver.FirstOrDefault(fod => fod.id == modelItem.id_remissionGuideRiver);

                        rgTmp.hasExitPlanctProduction = true;
                        db.RemissionGuideRiver.Attach(rgTmp);
                        db.Entry(rgTmp).State = EntityState.Modified;

                        #endregion

                        #region"UPDATE REMISSION GUIDE RIVER SECURITY SEALS"
                        //Update SecuritySeals
                        if (TempData["securitySealsList"] != null)
                        {
                            List<RemissionGuideRiverSecuritySeal> lstSecuritySeal = (List<RemissionGuideRiverSecuritySeal>)TempData["securitySealsList"];
                            var lstSecuritySealDB = db.RemissionGuideRiverSecuritySeal.Where(w => w.id_remissionGuideRiver == remissionGuideRiverForControlVehicle.id).ToList();
                            foreach (var securitySeal in lstSecuritySeal)
                            {
                                var securitySealTmp = lstSecuritySealDB.FirstOrDefault(fod => fod.id == securitySeal.id);
                                securitySealTmp.id_exitState = securitySeal.id_exitState;
                                db.RemissionGuideRiverSecuritySeal.Attach(securitySealTmp);
                                db.Entry(securitySealTmp).State = EntityState.Modified;
                            }
                        }
                        #endregion

                        #region Document
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "164");
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


                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["remissionGuideRiverControlVehicle"] = modelItem;

                        TempData.Keep("remissionGuideRiverControlVehicle");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + modelItem.RemissionGuideRiver.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("remissionGuideControlVehicle");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            //NO ELIMINAR

            modelItem.providerNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.providerNameAux;
            modelItem.productionUnitProviderAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.productionUnitProviderAux;
            
            modelItem.totalPoundsAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.totalPoundsAux;
            modelItem.zoneNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.zoneNameAux;
            modelItem.siteNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.siteNameAux;
            modelItem.addressTargetAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.addressTargetAux;
            modelItem.shippingTypeAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.shippingTypeAux;
            modelItem.driverNameAux = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.driverNameAux;

            modelItem.productionUnitProviderAuxLab = remissionGuideRiverForControlVehicle.RemissionGuideRiverControlVehicle.productionUnitProviderAuxLab;

            TempData.Keep("remissionGuideRiverControlVehicle");


            modelItem.RemissionGuideRiver = remissionGuideRiverForControlVehicle;
            return PartialView("_RemissionGuideRiverControlVehicleMainFormPartial", modelItem);
        }
        #endregion

        #region "REMISSION GUIDE RIVER CONTROL VEHICLE EDIT FORM"
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuideRiverControlVehicle(int id)
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
                    RemissionGuideRiverDetail = new List<RemissionGuideRiverDetail>(),
                    RemissionGuideRiverDispatchMaterial = new List<RemissionGuideRiverDispatchMaterial>(),
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
                }
            }

            string strPUP = db.Setting.FirstOrDefault(fod => fod.code == "EUPPPRIM")?.value ?? "";

            //bool isOwn = remissionGuideRiver?.RemissionGuideTransportation?.isOwn ?? false;

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
            remissionGuideRiver.RemissionGuideRiverControlVehicle.driverNameAux = (remissionGuideRiver?.RemissionGuideRiverTransportation?.driverName ?? "");

            remissionGuideRiver.RemissionGuideRiverControlVehicle.productionUnitProviderAuxLab = (strPUP != "") ? strPUP + ":" : "Unidad de Producción:";


            TempData["remissionGuideRiverForControlVehicle"] = remissionGuideRiver;
            TempData.Keep("remissionGuideRiverForControlVehicle");

            return PartialView("_FormEditRemissionGuideRiverControlVehicle", remissionGuideRiver.RemissionGuideRiverControlVehicle);
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

            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id);

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

        #region "Guía de Remisión Fluvial"
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehicleDetailViewAssignedStaff()
        {
            int id_remissionGuideRiver = (Request.Params["id_remissionGuideRiver"] != null && Request.Params["id_remissionGuideRiver"] != "") ? int.Parse(Request.Params["id_remissionGuideRiver"]) : -1;
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id_remissionGuideRiver);
            return PartialView("_RemissionGuideRiverDetailViewAssignedStaffPartial", remissionGuideRiver.RemissionGuideRiverAssignedStaff.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehicleDetailViewSecuritySeals()
        {
            int id_remissionGuideRiver = (Request.Params["id_remissionGuideRiver"] != null && Request.Params["id_remissionGuideRiver"] != "") ? int.Parse(Request.Params["id_remissionGuideRiver"]) : -1;
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id_remissionGuideRiver);
            return PartialView("_RemissionGuideRiverDetailViewSecuritySealsPartial", remissionGuideRiver.RemissionGuideRiverSecuritySeal.ToList());
        }
        #endregion

        #region "PAGINATION"
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_remissionGuideRiver)
        {
            TempData.Keep("remissionGuideRiver");

            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }

            int index = db.RemissionGuideRiver.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_remissionGuideRiver);

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
                            RemissionGuideRiver RemissionGuideRiver = model.FirstOrDefault(r => r.id == id);

                            DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

                            if (RemissionGuideRiver != null && documentState != null)
                            {
                                RemissionGuideRiver.Document.id_documentState = documentState.id;
                                RemissionGuideRiver.Document.DocumentState = documentState;
                                
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

        #region "Envio de Correo"
        //Método para enviar correo en la salida de Vehículos
        private string Envio_Correos_Logistica_Salida_Vehiculos(int id_remissionGuideRiver)
        {
            string mensajeRespuesta = string.Empty;
            DBContext db2 = new DBContext();
            try
            {
                string cuerpoMensaje = string.Empty;
                string asuntoMensaje = string.Empty;
                var remissionGuideRiver = db2.RemissionGuideRiver.FirstOrDefault(fod => fod.id == id_remissionGuideRiver);
                var remissionGuideRiverControlVehicle = db2.RemissionGuideRiverControlVehicle.FirstOrDefault(fod => fod.id_remissionGuideRiver == id_remissionGuideRiver);

                string mailFrom = ConfigurationManager.AppSettings["correoDefaultDesde"];
                string passwordMailFrom = ConfigurationManager.AppSettings["contrasenaCorreoDefault"];
                string passwordMailFromUncrypted = clsEncriptacion1.LeadnjirSimple.Desencriptar(passwordMailFrom);
                string smtpHost = ConfigurationManager.AppSettings["smtpHost"];
                string puertoHost = ConfigurationManager.AppSettings["puertoHost"];
                string mensajePrueba = ConfigurationManager.AppSettings["Pruebas"];

                int puertoHostInt = int.Parse(puertoHost);


                if (remissionGuideRiver != null)
                {

                    //string TextoPrueba = "PRUEBAS";
                    //int id_buyer = (int)remissionGuideRiver.id_buyer;
                    int id_provider = (int)remissionGuideRiver.id_providerRemisionGuideRiver;
                    int id_receiver = (int)remissionGuideRiver.id_reciver;
                    int id_driver = (int)remissionGuideRiver?.RemissionGuideRiverTransportation?.id_driver;

                    int id_buyer = remissionGuideRiver?.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide?.id_buyer ?? 0;

                    string destanationMails = string.Empty;

                    string mailBuyer = db2.Person.FirstOrDefault(fod => fod.id == id_buyer)?.email;
                    string mailProvider = db2.Person.FirstOrDefault(fod => fod.id == id_provider)?.email;
                    string nameProvider = db2.Person.FirstOrDefault(fod => fod.id == id_provider)?.fullname_businessName;
                    string recieverName = db2.Person.FirstOrDefault(fod => fod.id == id_receiver)?.fullname_businessName ?? "SIN RECIBIDOR";

                    //Inserto mail Compradores
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

                    string carRegistration = remissionGuideRiver?.RemissionGuideRiverTransportation?.Vehicle?.carRegistration ?? "SIN PLACA";
                    string carMark = remissionGuideRiver?.RemissionGuideRiverTransportation?.Vehicle?.mark ?? "SIN MARCA";
                    string carModel = remissionGuideRiver?.RemissionGuideRiverTransportation?.Vehicle?.model ?? "SIN MODELO";

                    string dateExit = remissionGuideRiverControlVehicle.exitDateProductionBuilding.Value.ToString("dd/MM/yyyy");
                    string hoursTimeExit = remissionGuideRiverControlVehicle.exitTimeProductionBuilding.Value.Hours.ToString();
                    string minutesTimeExit = remissionGuideRiverControlVehicle.exitTimeProductionBuilding.Value.Minutes.ToString();

                    string codeProductionUnitProvider = remissionGuideRiver?.ProductionUnitProvider?.code ?? "SIN CODIGO";
                    string nameProducctionUnitProvider = remissionGuideRiver?.ProductionUnitProvider?.name ?? "SIN NOMBRE";
                    string addressProductionUnitProvider = remissionGuideRiver?.ProductionUnitProvider?.address ?? "SIN DIRECCION";

                    string site = remissionGuideRiver?.ProductionUnitProvider?.FishingSite?.name ?? "SIN SITIO";
                    string zone = remissionGuideRiver?.ProductionUnitProvider?.FishingSite?.FishingZone?.name ?? "SIN ZONA";

                    string driverName = db2.Person.FirstOrDefault(fod => fod.id == id_driver)?.fullname_businessName ?? "SIN CONDUCTOR";
                    string cellPhoneNumber = db2.Person.FirstOrDefault(fod => fod.id == id_driver)?.cellPhoneNumberPerson ?? "SIN CELULAR";
					string description = remissionGuideRiver?.Document?.description ?? "";

					cuerpoMensaje = string.Empty;
                    cuerpoMensaje = string.Concat("<b>Transporte: Placa: </b>", carRegistration, " <b>Marca: </b>", carMark, " <b>Modelo: </b>", carModel);
                    cuerpoMensaje = cuerpoMensaje + "<br />";
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Fecha Salida: </b>", dateExit, " <b> Hora Salida: </b>", hoursTimeExit , ":", minutesTimeExit);
                    cuerpoMensaje = cuerpoMensaje + "<br />";
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Nombre Proveedor: </b> ", nameProvider, "<b> Camaronera: </b>", nameProducctionUnitProvider);
                    cuerpoMensaje = cuerpoMensaje + "<br />";
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Zona: </b>", zone, "<b> Sitio: </b>", site);
                    cuerpoMensaje = cuerpoMensaje + "<br />";
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Chofer: </b>", driverName, "<b> Celular: </b>", cellPhoneNumber);
					cuerpoMensaje = cuerpoMensaje + "<br />";
					cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Descripción: </b>", description);


					if (mensajePrueba == "SI")
                    {
                        asuntoMensaje = "PRUEBAS ";
                    }
                    asuntoMensaje += string.Concat("Salida de Vehículo, Guia de Remisión: ", remissionGuideRiver.Document.number);

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

        #region"BATCH EDIT"
        //Security Seals
        #region"BATCH EDIT SECURITY SEALS"

        public ActionResult RemissionGuideRiverControlVehicleSecuritySealsBatchEditingUpdateModel(MVCxGridViewBatchUpdateValues<RemissionGuideRiverSecuritySeal, int> updateValues)
        {
            TempData.Keep("remissionGuideRiverForControlVehicle");
            List<RemissionGuideRiverSecuritySeal> lstSecuritySeal = (List<RemissionGuideRiverSecuritySeal>)TempData["securitySealsList"];
            foreach (var securitySeal in updateValues.Update)
            {
                if (updateValues.IsValid(securitySeal))
                    UpdateRGCVSecuritySealBatchDetail(securitySeal, updateValues);
            }
            TempData["securitySealsList"] = lstSecuritySeal;
            TempData.Keep("securitySealsList");
            return PartialView("_RemissionGuideRiverControlVehicleSecuritySealsPartial", lstSecuritySeal);
        }

        public void UpdateRGCVSecuritySealBatchDetail(RemissionGuideRiverSecuritySeal securitySeal, MVCxGridViewBatchUpdateValues<RemissionGuideRiverSecuritySeal, int> updateValues)
        {
            List<RemissionGuideRiverSecuritySeal> lstSecuritySeal = (List<RemissionGuideRiverSecuritySeal>)TempData["securitySealsList"];

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