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
//using DXPANACEASOFT.Models.RemGuide.ControlVehicle;
using DXPANACEASOFT.Extensions;
using DXPANACEASOFT.Models.DTOModel;
using static DXPANACEASOFT.Controllers.RemissionGuideControlVehicleEntranceThirdController;
using static DXPANACEASOFT.Controllers.RemissionGuideControlVehicleEntranceController;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class RemissionGuideControlVehicleExitThirdController : DefaultController
    {
        // POST: RemissionGuideControlVehicleExitThird
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region "REMISSION GUIDE CONTROL VEHICLE EXIT THIRD FILTER RESULTS"
        [HttpPost]
        public ActionResult RemissionGuideControlVehicleExitThirdResults(RemissionGuide remissionGuide,
                                          Document document,
                                          DateTime? startEmissionDate, DateTime? endEmissionDate,
                                          DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                          DateTime? startDespachureDate, DateTime? endDespachureDate,
                                          DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                          DateTime? startentranceDateProductionBuilding, DateTime? endentranceDateProductionBuilding,
                                          int[] items, int[] businessOportunities)
        {
            //var model = db.RemissionGuide.Where(w => (w.RemissionGuideControlVehicle.hasExitPlanctProduction == true &&
            //                                                w.RemissionGuideControlVehicle.hasEntrancePlanctProduction == true &&
            //                                                w.RemissionGuideTransportation.isOwn == true &&
            //                                                (w.Document.DocumentState.code == "06" || w.Document.DocumentState.code == "09"))).ToList();

            db.Database.CommandTimeout = 1200;
            List<RemissionGuideSalidaTerceroDTO> model = db.Database.SqlQuery<RemissionGuideSalidaTerceroDTO>("exec spc_Consultar_GuiasRemisionSalidaTerceroDTO_StoredProcedure").ToList();

            #region DOCUMENT FILTERS

            if (document.id_documentState != 0)
            {
                model = model.Where(o => o.iddocumentState == document.id_documentState).ToList();
            }

            if (!string.IsNullOrEmpty(document.number))
            {
                model = model.Where(o => o.Numero.Contains(document.number)).ToList();
            }
            if (!string.IsNullOrEmpty(remissionGuide.Guia_Externa))
            {
                model = model.Where(o => o.GuiaExterna.Contains(remissionGuide.Guia_Externa)).ToList();
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

            return PartialView("_RemissionGuideControlVehicleExitThirdResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }

        public class RemissionGuideSalidaTerceroDTO
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
            public string Proceso { get; set; }
            public string GuiaExterna { get; set; }
            public string Proveedor { get; set; }
            public string UnidadProduccion { get; set; }
            public string Zona { get; set; }
            public string Sitio { get; set; }
            public string Chofer { get; set; }
            public string Placa { get; set; }
            public Nullable<System.DateTime> FechaSalida { get; set; }
            public Nullable<System.TimeSpan> HoraSalida{ get; set; }
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
            var model = db.RemissionGuide.Where(w => w.RemissionGuideTransportation.isOwn == true &&
                                                     w.RemissionGuideControlVehicle.hasExitPlanctProduction != true &&
                                                     w.RemissionGuideControlVehicle.hasEntrancePlanctProduction == true &&
                                                     (w.Document.DocumentState.code == "06" || w.Document.DocumentState.code == "09")
                                                    ).ToList();
            return PartialView("_RemissionGuideCVExitThirdHeaderResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuidePartial()
        {
            var model = db.RemissionGuide.Where(w => w.RemissionGuideTransportation.isOwn == true &&
                                                     w.RemissionGuideControlVehicle.hasExitPlanctProduction != true &&
                                                    w.RemissionGuideControlVehicle.hasEntrancePlanctProduction == true &&
                                                     (w.Document.DocumentState.code == "06" || w.Document.DocumentState.code == "09")
                                                    );
            return PartialView("_RemissionGuideCVExitThirdResultsPartial",model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuidePartialExitThird()
        {
            //var model = db.RemissionGuide.Where(w => (w.RemissionGuideControlVehicle.hasExitPlanctProduction == true &&
            //                                                w.RemissionGuideControlVehicle.hasEntrancePlanctProduction == true &&
            //                                                w.RemissionGuideTransportation.isOwn == true &&
            //                                                (w.Document.DocumentState.code == "06" || w.Document.DocumentState.code == "09"))).ToList();

            var model = (TempData["model"] as List<RemissionGuideSalidaTerceroDTO>);
            model = model ?? new List<RemissionGuideSalidaTerceroDTO>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideControlVehicleExitThirdPartial", model.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        #region "REMISSION GUIDE CONTROL VEHICLE EXIT THIRD HEADER"

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleExitThirdPartial()
        {
            var model = (TempData["model"] as List<RemissionGuide>);
            model = model ?? new List<RemissionGuide>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideControlVehicleExitThirdPartial", model.OrderByDescending(r => r.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleEntrancePartialAddNew(bool approve, string exitTimeBuilding, RemissionGuideControlVehicle itemModel)
        {
            TempData.Keep("remissionGuideForControlVehicleExitThird");
            //if (TempData["securitySealsEntranceList"] != null)
            //{
            //    TempData.Keep("securitySealsEntranceList");
            //}
            RemissionGuide remissionGuideForControlVehicle = (RemissionGuide)TempData["remissionGuideForControlVehicleExitThird"];
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == remissionGuideForControlVehicle.id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    itemModel.id_remissionGuide = remissionGuideForControlVehicle.id;

                    remissionGuide.hasEntrancePlanctProduction = true;

                    db.RemissionGuide.Attach(remissionGuide);
                    db.Entry(remissionGuide).State = EntityState.Modified;

                    ////Update SecuritySeals
                    //if (TempData["securitySealsEntranceList"] != null)
                    //{
                    //    List<RemissionGuideSecuritySeal> lstSecuritySeal = (List<RemissionGuideSecuritySeal>)TempData["securitySealsEntranceList"];
                    //    var lstSecuritySealDB = db.RemissionGuideSecuritySeal.Where(w => w.id_remissionGuide == remissionGuideForControlVehicle.id).ToList();
                    //    foreach (var securitySeal in lstSecuritySeal)
                    //    {
                    //        var securitySealTmp = lstSecuritySealDB.FirstOrDefault(fod => fod.id == securitySeal.id);
                    //        securitySealTmp.id_arrivalState = securitySeal.id_arrivalState;
                    //        db.RemissionGuideSecuritySeal.Attach(securitySealTmp);
                    //        db.Entry(securitySealTmp).State = EntityState.Modified;
                    //    }
                    //}
                    #region Document
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                    DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "162");
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

                    RemissionGuideControlVehicle rgcvTmp = db.RemissionGuideControlVehicle.FirstOrDefault(fod => fod.id_remissionGuide == remissionGuideForControlVehicle.id);

                    if (!string.IsNullOrEmpty(exitTimeBuilding)) rgcvTmp.exitTimeProductionBuilding = TimeSpan.Parse(exitTimeBuilding);

                    rgcvTmp.exitDateProductionBuilding = itemModel.exitDateProductionBuilding;

                    rgcvTmp.hasExitPlanctProduction = true;
                    rgcvTmp.ObservationExit = itemModel.ObservationExit;

                    if (approve){ }

                    db.RemissionGuideControlVehicle.Attach(rgcvTmp);
                    db.SaveChanges();
                    trans.Commit();

                    TempData["remissionGuideForControlVehicleExitThird"] = remissionGuide;
                    TempData.Keep("remissionGuideForControlVehicleExitThird");

                    ViewData["EditMessage"] = SuccessMessage("Control de Entrada para Guía de Remisión: " + remissionGuideForControlVehicle.Document.number + " se ha guardado correctamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("remissionGuideForControlVehicleExitThird");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            itemModel.RemissionGuide = remissionGuideForControlVehicle;
            return PartialView("_RemissionGuideControlVehicleExitThirdMainFormPartial", itemModel);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleExitThirdPartialUpdate(bool approve, string exitTimeBuilding, ControlVehicleDto itemModel)
        {

            TempData.Keep("remissionGuideForControlVehicleExitThird");

            RemissionGuideControlVehicleDto model = (RemissionGuideControlVehicleDto)TempData["remissionGuideForControlVehicleExitThird"];
            RemissionGuideControlVehicle modelItem = db.RemissionGuideControlVehicle.FirstOrDefault(r => r.id_remissionGuide == model.RemissionGuideControlVehicle.id_remissionGuide);

            if (itemModel != null && modelItem != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region "REMISSION GUIDE CONTROL VEHICLE EXIT THIRD"

                        modelItem.exitDateProductionBuilding = itemModel.exitDateProductionBuilding;
                        modelItem.hasExitPlanctProduction = true;

                        RemissionGuide rgTmp = db.RemissionGuide.FirstOrDefault(fod => fod.id == modelItem.id_remissionGuide);
                        rgTmp.hasExitPlanctProduction = true;
                        db.RemissionGuide.Attach(rgTmp);
                        db.Entry(rgTmp).State = EntityState.Modified;

                        if (!string.IsNullOrEmpty(exitTimeBuilding)) modelItem.exitTimeProductionBuilding = TimeSpan.Parse(exitTimeBuilding);

                        modelItem.exitDateProductionBuilding = itemModel.exitDateProductionBuilding;
                        modelItem.ObservationExit = itemModel.ObservationExit;
                        modelItem.RemissionGuide = rgTmp;



                        #endregion

                        #region Document
                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code == "162");
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
                       // db.Entry(rgTmp).State= EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        
                        TempData.Keep("remissionGuideForControlVehicleExitThird");
                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + modelItem.RemissionGuide.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("remissionGuideForControlVehicleExitThird");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }
            modelItem.providerNameAux = model.RemissionGuideControlVehicle.providerNameAux;
            modelItem.productionUnitProviderAux = model.RemissionGuideControlVehicle.productionUnitProviderAux;
            modelItem.poolNameAux = model.RemissionGuideControlVehicle.poolNameAux;
            modelItem.totalPoundsAux = model.RemissionGuideControlVehicle.totalPoundsAux;
            modelItem.zoneNameAux = model.RemissionGuideControlVehicle.zoneNameAux;
            modelItem.siteNameAux = model.RemissionGuideControlVehicle.siteNameAux;
            modelItem.addressTargetAux = model.RemissionGuideControlVehicle.addressTargetAux;
            modelItem.shippingTypeAux = model.RemissionGuideControlVehicle.shippingTypeAux;
            modelItem.driverNameAux = model.RemissionGuideControlVehicle.driverNameAux;
            modelItem.productionUnitProviderAuxLab = model.RemissionGuideControlVehicle.productionUnitProviderAuxLab;
            string personProcessPlant = model.RemissionGuideControlVehicle.PersonProcessPlant;
            string statusDocumento = model.RemissionGuideControlVehicle.DocumentStateCode;
            model.RemissionGuideControlVehicle = modelItem.ToDto();
            model.RemissionGuideControlVehicle.PersonProcessPlant = personProcessPlant;
            model.RemissionGuideControlVehicle.DocumentStateCode = statusDocumento;


            TempData["remissionGuideForControlVehicleExitThird"] = model;
            TempData.Keep("remissionGuideForControlVehicleExitThird");
            //modelItem.RemissionGuide = remissionGuideForControlVehicle;
            return PartialView("_RemissionGuideControlVehicleExitThirdMainFormPartial", model);
        }


        #endregion

        #region "REMISSION GUIDE CONTROL EXIT THIRD VEHICLE EDIT FORM"
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuideControlVehicleExitThird(int id)
        {
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(o => o.id == id);
            RemissionGuideControlVehicleDto model = new RemissionGuideControlVehicleDto();

            if (remissionGuide == null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("08"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

                model = new RemissionGuideControlVehicleDto
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
                    RemissionGuideControlVehicle = new ControlVehicleDto()

                };
            }
            else
            {
                if (remissionGuide?.Document == null)
                    model.Document = new Document();
                else
                    model.Document = remissionGuide.Document.ToSimpleModel();

                if (remissionGuide.RemissionGuideControlVehicle == null)
                {
                   
                    model.RemissionGuideControlVehicle = new ControlVehicleDto();
                }
                else
                {
                    model.RemissionGuideControlVehicle = remissionGuide.RemissionGuideControlVehicle.ToDto();
                }
                model.RemissionGuideControlVehicle.DocumentStateCode = remissionGuide?.Document?.DocumentState?.code ?? string.Empty;
                model.RemissionGuideControlVehicle.PersonProcessPlant = remissionGuide?.Person2?.processPlant?? string.Empty;
            }
            string strPUP = db.Setting.FirstOrDefault(fod => fod.code == "EUPPPRIM")?.value ?? "";
            bool isOwn = remissionGuide?.RemissionGuideTransportation?.isOwn ?? false;
            
            model.RemissionGuideControlVehicle.providerNameAux = remissionGuide?.Provider?.Person?.fullname_businessName ?? "SIN PROVEEDOR";
            model.RemissionGuideControlVehicle.productionUnitProviderAux = remissionGuide?.ProductionUnitProvider?.name ?? (strPUP != "" ? "SIN " + strPUP : "SIN CAMARONERA");
            model.RemissionGuideControlVehicle.poolNameAux = remissionGuide
                                                                        .RemissionGuideDetail != null ? (string.Join("", remissionGuide
                                                                                                                            .RemissionGuideDetail
                                                                                                                            .Select(s => s.productionUnitProviderPoolreference)
                                                                                                                            .DefaultIfEmpty("").ToArray())) : ("");
            model.RemissionGuideControlVehicle.totalPoundsAux = remissionGuide
                                                                        .RemissionGuideDetail != null ? (remissionGuide
                                                                                                            .RemissionGuideDetail
                                                                                                            .Select(s => s.quantityProgrammed).DefaultIfEmpty(0).Sum()) : (0);
            model.RemissionGuideControlVehicle.zoneNameAux = remissionGuide?.RemissionGuideTransportation?.FishingSite?.name ?? "";
            model.RemissionGuideControlVehicle.RemissionGuide = remissionGuide;
            model.RemissionGuideControlVehicle.siteNameAux = remissionGuide?.RemissionGuideTransportation?.FishingSite?.FishingZone?.name ?? "";
            model.RemissionGuideControlVehicle.addressTargetAux = remissionGuide?.ProductionUnitProvider?.FishingSite?.address ?? "";
            model.RemissionGuideControlVehicle.shippingTypeAux = remissionGuide?.PurchaseOrderShippingType?.name ?? "";
            model.RemissionGuideControlVehicle.driverNameAux = (!isOwn) ? (remissionGuide?.RemissionGuideTransportation?.driverName ?? "") : (remissionGuide?.RemissionGuideTransportation?.driverNameThird ?? "");

            model.RemissionGuideControlVehicle.productionUnitProviderAuxLab = (strPUP != "") ? strPUP + ":" : "Unidad de Producción:";
                      
            TempData["remissionGuideForControlVehicleExitThird"] = model;
            TempData.Keep("remissionGuideForControlVehicleExitThird");
                           
            return PartialView("_FormEditRemissionGuideControlVehicleExitThird", model);
        }
        //public ActionResult FormEditRemissionGuideControlVehicleExitThird(int id)
        //{
        //    RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(o => o.id == id);
        //
        //    if (remissionGuide == null)
        //    {
        //        DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("08"));
        //        DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");
        //
        //        remissionGuide = new RemissionGuide
        //        {
        //            Document = new Document
        //            {
        //                id = 0,
        //                id_documentType = documentType?.id ?? 0,
        //                DocumentType = documentType,
        //                id_documentState = documentState?.id ?? 0,
        //                DocumentState = documentState,
        //                emissionDate = DateTime.Now
        //            },
        //            despachureDate = DateTime.Now,
        //            arrivalDate = DateTime.Now,
        //            returnDate = DateTime.Now,
        //            startAdress = ActiveSucursal.address,
        //            RemissionGuideDetail = new List<RemissionGuideDetail>(),
        //            RemissionGuideDispatchMaterial = new List<RemissionGuideDispatchMaterial>(),
        //            isInternal = false,
        //            RemissionGuideControlVehicle = new RemissionGuideControlVehicle
        //            {
        //
        //            }
        //
        //        };
        //    }
        //    else
        //    {
        //        if (remissionGuide.RemissionGuideControlVehicle == null)
        //        {
        //            remissionGuide.RemissionGuideControlVehicle = remissionGuide.RemissionGuideControlVehicle ?? new Models.RemissionGuideControlVehicle();
        //            remissionGuide.RemissionGuideControlVehicle.RemissionGuide = remissionGuide;
        //            //remissionGuide.RemissionGuideControlVehicle.id_remissionGuide = remissionGuide.id;
        //        }
        //
        //    }
        //    string strPUP = db.Setting.FirstOrDefault(fod => fod.code == "EUPPPRIM")?.value ?? "";
        //    bool isOwn = remissionGuide?.RemissionGuideTransportation?.isOwn ?? false;
        //
        //    remissionGuide.RemissionGuideControlVehicle.providerNameAux = remissionGuide?.Provider?.Person?.fullname_businessName ?? "SIN PROVEEDOR";
        //    remissionGuide.RemissionGuideControlVehicle.productionUnitProviderAux = remissionGuide?.ProductionUnitProvider?.name ?? (strPUP != "" ? "SIN " + strPUP : "SIN CAMARONERA");
        //    remissionGuide.RemissionGuideControlVehicle.poolNameAux = remissionGuide
        //                                                                .RemissionGuideDetail != null ? (string.Join("", remissionGuide
        //                                                                                                                    .RemissionGuideDetail
        //                                                                                                                    .Select(s => s.productionUnitProviderPoolreference)
        //                                                                                                                    .DefaultIfEmpty("").ToArray())) : ("");
        //    remissionGuide.RemissionGuideControlVehicle.totalPoundsAux = remissionGuide
        //                                                                .RemissionGuideDetail != null ? (remissionGuide
        //                                                                                                    .RemissionGuideDetail
        //                                                                                                    .Select(s => s.quantityProgrammed).DefaultIfEmpty(0).Sum()) : (0);
        //    remissionGuide.RemissionGuideControlVehicle.zoneNameAux = remissionGuide?.RemissionGuideTransportation?.FishingSite?.name ?? "";
        //    remissionGuide.RemissionGuideControlVehicle.siteNameAux = remissionGuide?.RemissionGuideTransportation?.FishingSite?.FishingZone?.name ?? "";
        //    remissionGuide.RemissionGuideControlVehicle.addressTargetAux = remissionGuide?.ProductionUnitProvider?.FishingSite?.address ?? "";
        //    remissionGuide.RemissionGuideControlVehicle.shippingTypeAux = remissionGuide?.PurchaseOrderShippingType?.name ?? "";
        //    remissionGuide.RemissionGuideControlVehicle.driverNameAux = (!isOwn) ? (remissionGuide?.RemissionGuideTransportation?.driverName ?? "") : (remissionGuide?.RemissionGuideTransportation?.driverNameThird ?? "");
        //
        //    remissionGuide.RemissionGuideControlVehicle.productionUnitProviderAuxLab = (strPUP != "") ? strPUP + ":" : "Unidad de Producción:";
        //
        //    TempData["remissionGuideForControlVehicleExitThird"] = remissionGuide;
        //    TempData.Keep("remissionGuideForControlVehicleExitThird");
        //
        //    return PartialView("_FormEditRemissionGuideControlVehicleExitThird", remissionGuide.RemissionGuideControlVehicle);
        //}

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleEntranceCopy(int id)
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
                    startAdress = remissionGuide.startAdress,
                    Guia_Externa = remissionGuide.Guia_Externa
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

        #region "PAGINATION"
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_remissionGuide)
        {
            TempData.Keep("remissionGuideForControlVehicleExitThird");
            var preRemissionGuide = db.RemissionGuide.ToList();
            int index = preRemissionGuide.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_remissionGuide);

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
                TempData["remissionGuideForControlVehicleExitThird"] = remissionGuide;
                TempData.Keep("remissionGuideForControlVehicleExitThird");
                return PartialView("_RemissionGuideControlVehicleExitThirdMainFormPartial", remissionGuide);
            }

            TempData.Keep("remissionGuideForControlVehicleExitThird");

            return PartialView("_RemissionGuideControlVehicleExitThirdMainFormPartial", new RemissionGuide());
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

        #region"Auxiliar"
        #region"Security Seals"
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleEntranceDetailViewSecuritySeals()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideDetailViewSecuritySealsPartial", remissionGuide.RemissionGuideSecuritySeal.ToList());
        }
        #endregion
        #endregion

        #region"BATCH EDIT"
        //Security Seals
        #region"BATCH EDIT SECURITY SEALS"

        public ActionResult RemissionGuideControlVehicleEntranceSecuritySealsBatchEditingUpdateModel(MVCxGridViewBatchUpdateValues<RemissionGuideSecuritySeal, int> updateValues)
        {
            TempData.Keep("remissionGuideForControlVehicleEntrance");
            List<RemissionGuideSecuritySeal> lstSecuritySeal = (List<RemissionGuideSecuritySeal>)TempData["securitySealsEntranceList"];
            foreach (var securitySeal in updateValues.Update)
            {
                if (updateValues.IsValid(securitySeal))
                    UpdateRGCVSecuritySealBatchDetail(securitySeal, updateValues);
            }
            TempData["securitySealsEntranceList"] = lstSecuritySeal;
            TempData.Keep("securitySealsEntranceList");
            return PartialView("_RemissionGuideControlVehicleEntranceSecuritySealsPartial", lstSecuritySeal);
        }

        public void UpdateRGCVSecuritySealBatchDetail(RemissionGuideSecuritySeal securitySeal, MVCxGridViewBatchUpdateValues<RemissionGuideSecuritySeal, int> updateValues)
        {
            List<RemissionGuideSecuritySeal> lstSecuritySeal = (List<RemissionGuideSecuritySeal>)TempData["securitySealsEntranceList"];

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