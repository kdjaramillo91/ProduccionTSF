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
using DXPANACEASOFT.Services;
using System.Threading.Tasks;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using static DXPANACEASOFT.Controllers.RemissionGuideControlVehicleEntranceController;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class RemissionGuideControlVehicleController : DefaultController
    {
        // POST: RemissionGuideControlVehicle
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region REMISSION GUIDE CONTROL VEHICLE FILTER RESULTS
        [HttpPost]
        public ActionResult RemissionGuideControlVehicleResults(RemissionGuide remissionGuide,
                                          Document document,
                                          DateTime? startEmissionDate, DateTime? endEmissionDate,
                                          DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                          DateTime? startDespachureDate, DateTime? endDespachureDate,
                                          DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                          int[] items, int[] businessOportunities)
        {

            db.Database.CommandTimeout = 1200;
            List<RemissionGuideSalidaDTO> model = db.Database.SqlQuery<RemissionGuideSalidaDTO>("exec spc_Consultar_GuiasRemisionSalidaDTO_StoredProcedure").ToList();



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
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.FechaEmision) <= 0 && DateTime.Compare(o.FechaEmision, endEmissionDate.Value.Date) <= 0).ToList();
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

            return PartialView("_RemissionGuideControlVehicleResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        public class RemissionGuideSalidaDTO
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
            public string Proveedor { get; set; }
            public string UnidadProduccion { get; set; }
            public string Zona { get; set; }
            public string Sitio { get; set; }
            public string Chofer { get; set; }
            public string Placa { get; set; }
            public Nullable<System.DateTime> FechaSalida { get; set; }
            public Nullable<System.TimeSpan> HoraSalida { get; set; }
            public bool RequiereSeguridad { get; set; }
            public string Observacion { get; set; }
            public string Sello1 { get; set; }
            public string Sello2 { get; set; }
            public string Sello3 { get; set; }
            public string UsuarioCreacion { get; set; }
            public DateTime? FechaCreacion { get; set; }
            public string UsuarioModificacion { get; set; }
            public DateTime? FechaModificacion { get; set; }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideResults()
        {
            string[] codes = new string[] { "T", "M", "TF" };

            var model = db.RemissionGuide.Where(w => codes.Contains(w.PurchaseOrderShippingType.code)
                                                && w.Document.DocumentState.code == "03"
                                                && w.RemissionGuideControlVehicle == null
                                                && (w.RemissionGuideTransportation.isOwn == false)).ToList();

            //Listo todos los que tienen staff con valores
            var lstStaffWithPrice = db.RemissionGuideAssignedStaff.Where(w => w.viaticPrice > 0).Select(s => s.id_remissionGuide).Distinct().ToList();

            // Listo todos los que no tienen ni staff ni anticipo
            var lstRGOK = model.Where(w => !lstStaffWithPrice.Contains(w.id) 
            && (w.RemissionGuideTransportation.advancePrice ?? 0) <= 0);

            var id_paymentState = db.tbsysCatalogState
                        .FirstOrDefault(w => w.TController.name == "RemissionGuideInternControl"
                        && w.codeClasification == "01" && w.codeState == "02").id; //codeState = 02 Pagado


            var lstFinal = model.Where(w => ((w.RemissionGuideTransportation.advancePrice ?? 0) > 0
                                || w.RemissionGuideAssignedStaff.Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() > 0)
                                && w.id_tbsysCatalogState == id_paymentState).ToList();

            List<RemissionGuide> RgFinalLst = new List<RemissionGuide>();
            RgFinalLst.AddRange(lstRGOK.Concat(lstFinal));

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
                RgFinalLst = (TempData["modelBegin"] as List<RemissionGuide>);

            }
            else
            {
                RgFinalLst = db.RemissionGuide.Where(w => (w.PurchaseOrderShippingType.code == "T"
                                        || w.PurchaseOrderShippingType.code == "M"
                                        || w.PurchaseOrderShippingType.code == "TF")
                                        && w.Document.DocumentState.code == "03"
                                        && w.RemissionGuideControlVehicle == null
                                        && (w.RemissionGuideTransportation.isOwn == false)).ToList();

                ////Listo todos los que tienen staff con valores
                //var lstStaffWithPrice = db.RemissionGuideAssignedStaff.Where(w => w.viaticPrice > 0).Select(s => s.id_remissionGuide).Distinct().ToList();

                //// Listo todos los que no tienen ni staff ni anticipo
                //var lstRGOK = model.Where(w => !lstStaffWithPrice.Contains(w.id)
                //&& (w.RemissionGuideTransportation.advancePrice ?? 0) <= 0);

                //var id_paymentState = db.tbsysCatalogState
                //            .FirstOrDefault(w => w.TController.name == "RemissionGuideInternControl"
                //            && w.codeClasification == "01" && w.codeState == "02").id; //codeState = 02 Pagado


                //var lstFinal = model.Where(w => ((w.RemissionGuideTransportation.advancePrice ?? 0) > 0
                //                    || w.RemissionGuideAssignedStaff.Select(s => s.viaticPrice).DefaultIfEmpty(0).Sum() > 0)
                //                    && w.id_tbsysCatalogState == id_paymentState);
                
                //RgFinalLst.AddRange(lstRGOK.Concat(lstFinal));
            }

            TempData["modelBegin"] = RgFinalLst;
            TempData.Keep("modelBegin");

            return PartialView("_RemissionGuideResultsPartial", RgFinalLst.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        #region "REMISSION GUIDE CONTROL VEHICLE HEADER"

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehiclePartial()
        {
            var model = (TempData["model"] as List<RemissionGuideSalidaDTO>);
            model = model ?? new List<RemissionGuideSalidaDTO>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideControlVehiclePartial", model.OrderByDescending(r => r.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehiclePartialAddNew(bool approve, string exitTimeBuilding, RemissionGuideControlVehicle itemModel)
        {
            bool registroInsertado = false;
            TempData.Keep("remissionGuideForControlVehicle");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuide remissionGuideForControlVehicle = (RemissionGuide)TempData["remissionGuideForControlVehicle"];
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == remissionGuideForControlVehicle.id);

            string ruta = ConfigurationManager.AppSettings["rutaXmlFEX"];
            string rutaA1Firmar = ConfigurationManager.AppSettings["rutaXmlA1Firmar"];

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    itemModel.id_remissionGuide = remissionGuideForControlVehicle.id;

                    remissionGuide.hasExitPlanctProduction = true;
                    itemModel.hasExitPlanctProduction = true;


                    #region Document
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "160");
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


                    db.RemissionGuide.Attach(remissionGuide);
                    db.Entry(remissionGuide).State = EntityState.Modified;

                    //Update SecuritySeals
                    if (TempData["securitySealsList"] != null)
                    {
                        List<RemissionGuideSecuritySeal> lstSecuritySeal = (List<RemissionGuideSecuritySeal>)TempData["securitySealsList"];
                        var lstSecuritySealDB = db.RemissionGuideSecuritySeal.Where(w => w.id_remissionGuide == remissionGuideForControlVehicle.id).ToList();
                        foreach (var securitySeal in lstSecuritySeal)
                        {
                            var securitySealTmp = lstSecuritySealDB.FirstOrDefault(fod => fod.id == securitySeal.id);
                            securitySealTmp.id_exitState = securitySeal.id_exitState;
                            db.RemissionGuideSecuritySeal.Attach(securitySealTmp);
                            db.Entry(securitySealTmp).State = EntityState.Modified;
                        }
                    }
                    if (!string.IsNullOrEmpty(exitTimeBuilding)) itemModel.exitTimeProductionBuilding = TimeSpan.Parse(exitTimeBuilding);

                    if (approve)
                    {

                    }
                    db.RemissionGuideControlVehicle.Add(itemModel);
                    db.SaveChanges();
                    trans.Commit();

                    TempData["remissionGuideForControlVehicle"] = remissionGuide;
                    TempData.Keep("remissionGuideForControlVehicle");
                    

                    ViewData["EditMessage"] = SuccessMessage("Control de Salida para Guía de Remisión: " + remissionGuideForControlVehicle.Document.number + " se ha guardado correctamente");
                    registroInsertado = true;
                    
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuideForControlVehicle");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    //ViewData["EditError"] = ErrorMessage();
                    trans.Rollback();
                    registroInsertado = false;
                }
            }
            //TempData.Keep("remissionGuide");
            itemModel.RemissionGuide = remissionGuideForControlVehicle;

            if (registroInsertado == true)
            {
                string answer = Services.ServiceLogistics.AutorizeRemissionGuide(remissionGuide.id, this.ActiveCompanyId, ruta, rutaA1Firmar);
                string respuestaCorreo = string.Empty;

                _= Task.Run(() => ServiceRemissionGuideControlVehicle
                                        .Envio_Correos_Logistica_Salida( db,
                                                                         itemModel.id_remissionGuide,
                                                                         remissionGuideForControlVehicle.id
                                                                         ) ).ConfigureAwait(false);
                
            }
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

            return PartialView("_RemissionGuideControlVehicleMainFormPartial", itemModel);

        }

        

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehiclePartialUpdate(bool approve, string exitTimeBuilding, RemissionGuideControlVehicle itemModel)
        {
            TempData.Keep("remissionGuideForControlVehicle");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuide remissionGuideForControlVehicle = (RemissionGuide)TempData["remissionGuideForControlVehicle"];

            RemissionGuideControlVehicle modelItem = db.RemissionGuideControlVehicle.FirstOrDefault(r => r.id_remissionGuide == remissionGuideForControlVehicle.id);

            if (itemModel != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region "REMISSION GUIDE CONTROL VEHICLE"

                        modelItem.exitDateProductionBuilding = itemModel.exitDateProductionBuilding;
                        modelItem.hasExitPlanctProduction = true;

                        if (!string.IsNullOrEmpty(exitTimeBuilding)) modelItem.exitTimeProductionBuilding = TimeSpan.Parse(exitTimeBuilding);

                        modelItem.ObservationExit = itemModel.ObservationExit;

                        RemissionGuide rgTmp = db.RemissionGuide.FirstOrDefault(fod => fod.id == modelItem.id_remissionGuide);

                        rgTmp.hasExitPlanctProduction = true;
                        db.RemissionGuide.Attach(rgTmp);
                        db.Entry(rgTmp).State = EntityState.Modified;

                        #endregion

                        #region Document
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "160");
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

                        #region"UPDATE REMISSION GUIDE SECURITY SEALS"
                        //Update SecuritySeals
                        if (TempData["securitySealsList"] != null)
                        {
                            List<RemissionGuideSecuritySeal> lstSecuritySeal = (List<RemissionGuideSecuritySeal>)TempData["securitySealsList"];
                            var lstSecuritySealDB = db.RemissionGuideSecuritySeal.Where(w => w.id_remissionGuide == remissionGuideForControlVehicle.id).ToList();
                            foreach (var securitySeal in lstSecuritySeal)
                            {
                                var securitySealTmp = lstSecuritySealDB.FirstOrDefault(fod => fod.id == securitySeal.id);
                                securitySealTmp.id_exitState = securitySeal.id_exitState;
                                db.RemissionGuideSecuritySeal.Attach(securitySealTmp);
                                db.Entry(securitySealTmp).State = EntityState.Modified;
                            }
                        }
                        #endregion


                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["remissionGuideControlVehicle"] = modelItem;

                        TempData.Keep("remissionGuideControlVehicle");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + modelItem.RemissionGuide.Document.number + " guardada exitosamente");
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

            TempData.Keep("remissionGuideControlVehicle");


            modelItem.RemissionGuide = remissionGuideForControlVehicle;
            return PartialView("_RemissionGuideControlVehicleMainFormPartial", modelItem);
        }
        #endregion

        #region "REMISSION GUIDE CONTROL VEHICLE EDIT FORM"
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuideControlVehicle(int id)
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
                                                                        .RemissionGuideDetail != null?  (string.Join("", remissionGuide
                                                                                                                            .RemissionGuideDetail
                                                                                                                            .Select(s => s.productionUnitProviderPoolreference)
                                                                                                                            .DefaultIfEmpty("").ToArray())) :("");
            remissionGuide.RemissionGuideControlVehicle.totalPoundsAux = remissionGuide
                                                                        .RemissionGuideDetail != null ? (remissionGuide
                                                                                                            .RemissionGuideDetail
                                                                                                            .Select(s => s.quantityProgrammed).DefaultIfEmpty(0).Sum()):(0);
            remissionGuide.RemissionGuideControlVehicle.zoneNameAux = remissionGuide?.RemissionGuideTransportation?.FishingSite?.name ?? "";
            remissionGuide.RemissionGuideControlVehicle.siteNameAux = remissionGuide?.RemissionGuideTransportation?.FishingSite?.FishingZone?.name ?? "";
            remissionGuide.RemissionGuideControlVehicle.addressTargetAux = remissionGuide?.ProductionUnitProvider?.FishingSite?.address ?? "";
            remissionGuide.RemissionGuideControlVehicle.shippingTypeAux = remissionGuide?.PurchaseOrderShippingType?.name ?? "";
            remissionGuide.RemissionGuideControlVehicle.driverNameAux = (!isOwn) ? (remissionGuide?.RemissionGuideTransportation?.driverName ?? "") : (remissionGuide?.RemissionGuideTransportation?.driverNameThird ?? "");

            remissionGuide.RemissionGuideControlVehicle.productionUnitProviderAuxLab = (strPUP != "") ? strPUP + ":" : "Unidad de Producción:";


            TempData["remissionGuideForControlVehicle"] = remissionGuide;
            TempData.Keep("remissionGuideForControlVehicle");

            return PartialView("_FormEditRemissionGuideControlVehicle", remissionGuide.RemissionGuideControlVehicle);
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
            var ids = db.RemissionGuide.Select(r => r.id).ToArray();
            int index = ids.OrderByDescending(r => r)
                           .ToList()
                           .FindIndex(r => r == id_remissionGuide);

            var result = new
            {
                maximunPages = ids.Count(),
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

        #region BATCH EDIT
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