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
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using System.Data.SqlClient;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class RemissionGuideInternControlViaticController : DefaultController
    {
        // POST: RemissionGuideInternControlViatic
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region "REMISSION GUIDE INTERN CONTROL VIATIC VEHICLE FILTER RESULTS"
        [HttpPost]
        public ActionResult RemissionGuideInternControlViaticResults(RemissionGuide remissionGuide,
                                          Document document,
                                          DateTime? startEmissionDate, DateTime? endEmissionDate,
                                          DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                          DateTime? startDespachureDate, DateTime? endDespachureDate,
                                          DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                          int[] items, int[] businessOportunities)
        {
            var model = db.RemissionGuide
                .Where(w => 
                //w.id_tbsysCatalogState != null
                w.RemissionGuideCustomizedViaticPersonalAssigned != null
                && w.RemissionGuideCustomizedViaticPersonalAssigned.hasPayment == true
                ).ToList();

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
            
            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_RemissionGuideInternControlViaticResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideInternControlViaticPartial()
        {
            List<RemissionGuide> model = (List<RemissionGuide>)TempData["model"];
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_RemissionGuideInternControlViaticPartial",model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideResults()
        {

            var model = db.RemissionGuide.Where(w => 
            (w.PurchaseOrderShippingType.code == "T" 
            || w.PurchaseOrderShippingType.code == "M"
            || w.PurchaseOrderShippingType.code == "TF") 
            && (w.Document.DocumentState.code == "03" || w.Document.DocumentState.code == "06")
            && w.RemissionGuideCustomizedViaticPersonalAssigned.hasPayment != true
            && w.RemissionGuideCustomizedViaticPersonalAssigned != null
            && w.RemissionGuideAssignedStaff.Where(wh => wh.isActive && wh.viaticPrice > 0).Count() > 0 
            && (w.RemissionGuideTransportation.isOwn == false) 
            && ((w.RemissionGuideAssignedStaff.Where(l => l.viaticPrice > 0).ToList().Count > 0) 
            || (w.RemissionGuideTransportation != null && w.RemissionGuideTransportation.advancePrice > 0)));
            return PartialView("_RemissionGuideInternControlViaticHeaderResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuidePartial()
        { 
            var model = db.RemissionGuide.Where(w => 
            (w.PurchaseOrderShippingType.code == "T" 
            || w.PurchaseOrderShippingType.code == "M" 
            || w.PurchaseOrderShippingType.code == "TF")
            && (w.Document.DocumentState.code == "03" || w.Document.DocumentState.code == "06")
            && w.RemissionGuideCustomizedViaticPersonalAssigned != null
            && w.RemissionGuideCustomizedViaticPersonalAssigned.hasPayment != true
            && w.RemissionGuideAssignedStaff.Where(wh => wh.isActive && wh.viaticPrice > 0).Count() > 0
            && (w.RemissionGuideTransportation.isOwn == false)
            && ((w.RemissionGuideAssignedStaff.Where(l => l.viaticPrice > 0).ToList().Count > 0)
            || (w.RemissionGuideTransportation != null && w.RemissionGuideTransportation.advancePrice > 0)));
            return PartialView("_RemissionGuideResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        #region "REMISSION GUIDE INTERN CONTROL VIATIC VEHICLE HEADER"

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideIntermControlPartial()
        {
            var model = (TempData["model"] as List<RemissionGuide>);
            model = model ?? new List<RemissionGuide>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideInternControlViaticPartial", model.OrderByDescending(r => r.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideInternControlViaticPartialAddNew(bool approve, string exitTimeBuilding, RemissionGuideControlVehicle itemModel)
        {
            bool registroInsertado = false;
            TempData.Keep("remissionGuideForControlVehicle");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuide remissionGuideForControlVehicle = (RemissionGuide)TempData["remissionGuideForControlVehicle"];
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == remissionGuideForControlVehicle.id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    itemModel.id_remissionGuide = remissionGuideForControlVehicle.id;

                    remissionGuide.hasExitPlanctProduction = true;
                    itemModel.hasExitPlanctProduction = true;

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
                    

                    //exitTimeBuilding = exitTimeBuilding.Substring(11, (exitTimeBuilding.Length - 11));
                    if (!string.IsNullOrEmpty(exitTimeBuilding)) itemModel.exitTimeProductionBuilding = TimeSpan.Parse(exitTimeBuilding);

                    if (approve)
                    {

                    }

                    db.RemissionGuideControlVehicle.Add(itemModel);
                    db.SaveChanges();
                    trans.Commit();

                    TempData["remissionGuideForControlVehicle"] = remissionGuide;
                    TempData.Keep("remissionGuideForControlVehicle");

                    ViewData["EditMessage"] = SuccessMessage("Control de Pago Viatico: " + remissionGuideForControlVehicle.Document.number + " se ha guardado correctamente");
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

            //string answer = Services.ServiceLogistics.AutorizeRemissionGuide(remissionGuide.id);
            //string viewDataEditMessage = "";
            //if (answer != "OK")
            //{
            //    viewDataEditMessage = (string)ViewData["EditMessage"];
            //    ViewData["EditMessage"] = viewDataEditMessage + " - " + answer;
            //}

            string respuestaCorreo = string.Empty;
            if (registroInsertado == true)
            {

                respuestaCorreo = Envio_Correos_Logistica_Salida_Vehiculos(itemModel.id_remissionGuide);

                if (respuestaCorreo == "OK")
                {
                    DBContext db3 = new DBContext();
                    RemissionGuideControlVehicle rgcvSETmp = null;
                    using (DbContextTransaction trans = db3.Database.BeginTransaction())
                    {
                        try
                        {
                            rgcvSETmp = db3.RemissionGuideControlVehicle.FirstOrDefault(fod => fod.id_remissionGuide == remissionGuideForControlVehicle.id);
                            if (rgcvSETmp != null)
                            {
                                rgcvSETmp.hasSentEmail = true;
                                db3.RemissionGuideControlVehicle.Attach(rgcvSETmp);
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

            return PartialView("_RemissionGuideControlVehicleMainFormPartial", itemModel);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideInternControlViaticPartialUpdate(bool approve, string exitTimeBuilding, RemissionGuide itemModel, RemissionGuideCustomizedViaticPersonalAssigned rgt)
        {
            TempData.Keep("remissionGuideForInternControl");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuide remissionGuideForInternControl = (RemissionGuide)TempData["remissionGuideForInternControl"];
            RemissionGuide modelItem = db.RemissionGuide.FirstOrDefault(r => r.id   == remissionGuideForInternControl.id);
            RemissionGuide rgTmp = db.RemissionGuide.FirstOrDefault(fod => fod.id == modelItem.id);

            if (itemModel != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region "REMISSION GUIDE INTERN CONTROL VIATIC"

                        RemissionGuideCustomizedViaticPersonalAssigned rgcvpa = rgTmp.RemissionGuideCustomizedViaticPersonalAssigned;
                        rgcvpa.id_PaymentState = rgt?.id_PaymentState;
                        rgcvpa.hasPayment = true;
                        rgcvpa.id_UserApproved = ActiveUser.id;
                        rgcvpa.dateApproved = DateTime.Now;

                        db.RemissionGuideCustomizedViaticPersonalAssigned.Attach(rgcvpa);
                        db.Entry(rgcvpa).State = EntityState.Modified;

                        rgTmp.id_tbsysCatalogState =  itemModel.id_tbsysCatalogState; // cambia estado pagado
                        
                        db.RemissionGuide.Attach(rgTmp);
                        db.Entry(rgTmp).State = EntityState.Modified;

                        #endregion

                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["RemissionGuideInternControlViatic"] = rgTmp;

                        TempData.Keep("RemissionGuideInternControlViatic");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + rgTmp.Document.number + " Estado guardado exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("RemissionGuideInternControlViatic");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("RemissionGuideInternControlViatic");

            modelItem = remissionGuideForInternControl;
            return PartialView("_RemissionGuideInternControlViaticMainFormPartial", rgTmp);
        }
        #endregion

        #region "REMISSION GUIDE INTERN CONTROL VIATIC VEHICLE EDIT FORM"
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuideInternControlViatic(int id)
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
                   

                };
            }
            else
            {
                //remissionGuide.id_tbsysCatalogState = 0;
                //if (remissionGuide.RemissionGuideControlVehicle == null)
                //{
                //    remissionGuide.RemissionGuideControlVehicle = remissionGuide.RemissionGuideControlVehicle ?? new Models.RemissionGuideControlVehicle();
                //    remissionGuide.RemissionGuideControlVehicle.RemissionGuide = remissionGuide;
                //    //remissionGuide.RemissionGuideControlVehicle.id_remissionGuide = remissionGuide.id;
                //}

            }

            TempData["remissionGuideForInternControl"] = remissionGuide;
            TempData.Keep("remissionGuideForInternControl");

            return PartialView("_FormEditRemissionGuideInternControlViatic", remissionGuide);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideInternControlViaticCopy(int id)
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

            return PartialView("_FormEditRemissionGuideInternControlViatic", remissionGuideCopy);
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

        #region "Guía de Remisión"
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleDetailViewAssignedStaff()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideInternControlViaticDetailViewAssignedStaffPartial", remissionGuide.RemissionGuideAssignedStaff.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideControlVehicleDetailViewSecuritySeals()
        {
            int id_remissionGuide = (Request.Params["id_remissionGuide"] != null && Request.Params["id_remissionGuide"] != "") ? int.Parse(Request.Params["id_remissionGuide"]) : -1;
            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
            return PartialView("_RemissionGuideInternControlViaticDetailViewSecuritySealsPartial", remissionGuide.RemissionGuideSecuritySeal.ToList());
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
                return PartialView("_RemissionGuideInternControlViaticMainFormPartial", remissionGuide);
            }

            TempData.Keep("remissionGuide");

            return PartialView("_RemissionGuideInternControlViaticMainFormPartial", new RemissionGuide());
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

        #region "Envio de Correo"
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


                    if (!(string.IsNullOrEmpty(mailBuyer)))
                    {
                        destanationMails = destanationMails + ";" + mailBuyer;
                    }
                    if (!(string.IsNullOrEmpty(mailProvider)))
                    {
                        destanationMails = destanationMails + ";" + mailProvider;
                    }

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
                    cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Chofer: </b>", driverName);
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

        #region"BATCH EDIT"
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

        #region PRINT
        [HttpPost, ValidateInput(false)]
        public JsonResult PRViaticTerrestrel(string codeReport, LiquidationFreight liquidationFreight,
                                          Document document,
                                          DateTime? startEmissionDate, DateTime? endEmissionDate,
                                          DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                          string carRegistration,
                                          int? id_providerfilter,
                                          int? id_owner,
                                          int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();

            string str_starEmissionDate = "";
            if (startEmissionDate != null) { str_starEmissionDate = startEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@str_FEmisionDateStart";
            _param.Valor = str_starEmissionDate;
            paramLst.Add(_param);

            string str_endEmissionDate = "";
            if (endEmissionDate != null) { str_endEmissionDate = endEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@str_FEmisionDateEnd";
            _param.Valor = str_endEmissionDate;
            paramLst.Add(_param);

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;
            _repMod.nameReport = "Pagos Valores";

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            db.Database.CommandTimeout = 2200;

            List<ResultRemisionGuideViaticTerrestre> modelAux = new List<ResultRemisionGuideViaticTerrestre>();
            modelAux = db.Database.SqlQuery<ResultRemisionGuideViaticTerrestre>
                    ("exec par_GuideRemisionViaticoGeneral @str_FEmisionDateStart, @str_FEmisionDateEnd",
                    new SqlParameter("str_FEmisionDateStart", paramLst[0].Valor),
                    new SqlParameter("str_FEmisionDateEnd", paramLst[1].Valor)
                    ).ToList();

            TempData["modelRemisionGuieViaticTerrestre"] = modelAux;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        #endregion
    }
}