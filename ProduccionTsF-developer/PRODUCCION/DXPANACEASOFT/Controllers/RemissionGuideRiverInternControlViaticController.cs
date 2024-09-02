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
using EntidadesAuxiliares.General;
using EntidadesAuxiliares.CrystalReport;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class RemissionGuideRiverInternControlViaticController : DefaultController
    {
        // POST: RemissionGuideRiverInternControlViaticVehicle
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region "REMISSION GUIDE RIVER CONTROL VEHICLE FILTER RESULTS"
        [HttpPost]
        public ActionResult RemissionGuideRiverInternControlViaticResults(RemissionGuideRiver remissionGuideRiver,
                                          Document document,
                                          DateTime? startEmissionDate, DateTime? endEmissionDate,
                                          DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
                                          DateTime? startDespachureDate, DateTime? endDespachureDate,
                                          DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
                                          int[] items, int[] businessOportunities)
        {
            var model = db.RemissionGuideRiver
                            .Where(w => 
                            //w.id_tbsysCatalogState != null
                            w.RemissionGuideRiverCustomizedAdvancedTransportist != null
                            // && w.RemissionGuideRiverCustomizedAdvancedTransportist.hasPayment != null
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

            return PartialView("_RemissionGuideRiverInternControlViaticResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverInternControlViaticPartial()
        {
            List<RemissionGuideRiver> model = (List<RemissionGuideRiver>)TempData["model"];
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_RemissionGuideRiverInternControlViaticPartial",model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverResults()
        {

            var model = db.RemissionGuideRiver.Where(w => (w.PurchaseOrderShippingType.code == "T" 
            || w.PurchaseOrderShippingType.code == "M"
            || w.PurchaseOrderShippingType.code == "TF") 
            && (w.Document.DocumentState.code == "03" || w.Document.DocumentState.code == "06")
            && w.RemissionGuideRiverCustomizedViaticPersonalAssigned.hasPayment != true
            //&& w.id_tbsysCatalogState == null 
            && (w.hasExitPlanctProduction != true && w.hasEntrancePlanctProduction != true)
            && ((w.RemissionGuideRiverAssignedStaff.Where(l => l.viaticPrice > 0).ToList().Count > 0) 
            || (w.RemissionGuideRiverTransportation != null && w.RemissionGuideRiverTransportation.advancePrice > 0)));
            return PartialView("_RemissionGuideRiverInternControlViaticHeaderResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverPartial()
        { 
            var model = db.RemissionGuideRiver.Where(w => (w.PurchaseOrderShippingType.code == "T" || w.PurchaseOrderShippingType.code == "M" || w.PurchaseOrderShippingType.code == "TF")
            && (w.Document.DocumentState.code == "03" || w.Document.DocumentState.code == "06")
            //&& w.id_tbsysCatalogState == null
            && (w.hasExitPlanctProduction != true && w.hasEntrancePlanctProduction != true)
            && w.RemissionGuideRiverCustomizedViaticPersonalAssigned.hasPayment != true
            && ((w.RemissionGuideRiverAssignedStaff.Where(l => l.viaticPrice > 0).ToList().Count > 0)
            || (w.RemissionGuideRiverTransportation != null && w.RemissionGuideRiverTransportation.advancePrice > 0)));
            //db.RemissionGuideRiver.Where(w => (w.PurchaseOrderShippingType.code == "T" || w.PurchaseOrderShippingType.code == "M") && w.Document.DocumentState.code == "03" && w.id_tbsysCatalogState == null && (w.RemissionGuideRiverTransportation.isOwn == false));
            return PartialView("_RemissionGuideRiverResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        #region "REMISSION GUIDE RIVER CONTROL VEHICLE HEADER"

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverIntermControlPartial()
        {
            var model = (TempData["model"] as List<RemissionGuideRiver>);
            model = model ?? new List<RemissionGuideRiver>();

            TempData.Keep("model");

            return PartialView("_RemissionGuideRiverInternControlViaticPartial", model.OrderByDescending(r => r.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverInternControlViaticPartialAddNew(bool approve, string exitTimeBuilding, RemissionGuideRiverControlVehicle itemModel)
        {
            bool registroInsertado = false;
            TempData.Keep("remissionGuideRiverForControlVehicle");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuideRiver remissionGuideRiverForControlVehicle = (RemissionGuideRiver)TempData["remissionGuideRiverForControlVehicle"];
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(fod => fod.id == remissionGuideRiverForControlVehicle.id);

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
                    

                    //exitTimeBuilding = exitTimeBuilding.Substring(11, (exitTimeBuilding.Length - 11));
                    if (!string.IsNullOrEmpty(exitTimeBuilding)) itemModel.exitTimeProductionBuilding = TimeSpan.Parse(exitTimeBuilding);

                    if (approve)
                    {

                    }

                    db.RemissionGuideRiverControlVehicle.Add(itemModel);
                    db.SaveChanges();
                    trans.Commit();

                    TempData["remissionGuideRiverForControlVehicle"] = remissionGuideRiver;
                    TempData.Keep("remissionGuideRiverForControlVehicle");

                    ViewData["EditMessage"] = SuccessMessage("Control de Pago Viatico: " + remissionGuideRiverForControlVehicle.Document.number + " se ha guardado correctamente");
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
            //TempData.Keep("remissionGuideRiver");
            itemModel.RemissionGuideRiver = remissionGuideRiverForControlVehicle;

            //string answer = Services.ServiceLogistics.AutorizeRemissionGuideRiver(remissionGuideRiver.id);
            //string viewDataEditMessage = "";
            //if (answer != "OK")
            //{
            //    viewDataEditMessage = (string)ViewData["EditMessage"];
            //    ViewData["EditMessage"] = viewDataEditMessage + " - " + answer;
            //}

            string respuestaCorreo = string.Empty;
            if (registroInsertado == true)
            {

                //respuestaCorreo = Envio_Correos_Logistica_Salida_Vehiculos(itemModel.id_remissionGuideRiver);

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

            return PartialView("_RemissionGuideRiverControlVehicleMainFormPartial", itemModel);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverInternControlViaticPartialUpdate(bool approve, string exitTimeBuilding, RemissionGuideRiver itemModel, RemissionGuideRiverCustomizedViaticPersonalAssigned rgt)
        {
            TempData.Keep("remissionGuideRiverForInternControl");
            if (TempData["securitySealsList"] != null)
            {
                TempData.Keep("securitySealsList");
            }
            RemissionGuideRiver remissionGuideRiverForInternControl = (RemissionGuideRiver)TempData["remissionGuideRiverForInternControl"];
            RemissionGuideRiver modelItem = db.RemissionGuideRiver.FirstOrDefault(r => r.id   == remissionGuideRiverForInternControl.id);
            RemissionGuideRiver rgTmp = db.RemissionGuideRiver.FirstOrDefault(fod => fod.id == modelItem.id);

            if (itemModel != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region "REMISSION GUIDE RIVER CONTROL VEHICLE"

                        RemissionGuideRiverCustomizedViaticPersonalAssigned rgcvpa = rgTmp.RemissionGuideRiverCustomizedViaticPersonalAssigned;
                        rgcvpa.id_PaymentState = rgt?.id_PaymentState; // cambia estado de pago
                        rgcvpa.hasPayment = true;
                        rgcvpa.id_UserApproved = ActiveUser.id;
                        rgcvpa.dateApproved = DateTime.Now;

                        db.RemissionGuideRiverCustomizedViaticPersonalAssigned.Attach(rgcvpa);
                        db.Entry(rgcvpa).State = EntityState.Modified;

                        rgTmp.id_tbsysCatalogState =  itemModel.id_tbsysCatalogState; // cambia estado pagado
                        
                        db.RemissionGuideRiver.Attach(rgTmp);
                        db.Entry(rgTmp).State = EntityState.Modified;

                        #endregion

                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["remissionGuideRiverInternControl"] = rgTmp;

                        TempData.Keep("remissionGuideRiverInternControl");

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión: " + rgTmp.Document.number + " Estado guardado exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("remissionGuideRiverInternControl");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("remissionGuideRiverInternControl");

            modelItem = remissionGuideRiverForInternControl;
            return PartialView("_RemissionGuideRiverInternControlViaticMainFormPartial", rgTmp);
        }
        #endregion

        #region "REMISSION GUIDE RIVER CONTROL VEHICLE EDIT FORM"
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditRemissionGuideRiverInternControlViatic(int id)
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
                   

                };
            }
            else
            {
                //remissionGuideRiver.id_tbsysCatalogState = 0;
                //if (remissionGuideRiver.RemissionGuideRiverControlVehicle == null)
                //{
                //    remissionGuideRiver.RemissionGuideRiverControlVehicle = remissionGuideRiver.RemissionGuideRiverControlVehicle ?? new Models.RemissionGuideRiverControlVehicle();
                //    remissionGuideRiver.RemissionGuideRiverControlVehicle.RemissionGuideRiver = remissionGuideRiver;
                //    //remissionGuideRiver.RemissionGuideRiverControlVehicle.id_remissionGuideRiver = remissionGuideRiver.id;
                //}

            }

            TempData["remissionGuideRiverForInternControl"] = remissionGuideRiver;
            TempData.Keep("remissionGuideRiverForInternControl");

            return PartialView("_FormEditRemissionGuideRiverInternControlViatic", remissionGuideRiver);
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

        #region "Guía de Remisión Fluvial"
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehicleDetailViewAssignedStaff()
        {
            int id_remissionGuideRiver = (Request.Params["id_remissionGuideRiver"] != null && Request.Params["id_remissionGuideRiver"] != "") ? int.Parse(Request.Params["id_remissionGuideRiver"]) : -1;
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id_remissionGuideRiver);
            return PartialView("_RemissionGuideRiverInternControlViaticDetailViewAssignedStaffPartial", remissionGuideRiver.RemissionGuideRiverAssignedStaff.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideRiverControlVehicleDetailViewSecuritySeals()
        {
            int id_remissionGuideRiver = (Request.Params["id_remissionGuideRiver"] != null && Request.Params["id_remissionGuideRiver"] != "") ? int.Parse(Request.Params["id_remissionGuideRiver"]) : -1;
            RemissionGuideRiver remissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(r => r.id == id_remissionGuideRiver);
            return PartialView("_RemissionGuideRiverInternControlViaticDetailViewSecuritySealsPartial", remissionGuideRiver.RemissionGuideRiverSecuritySeal.ToList());
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
                return PartialView("_RemissionGuideRiverInternControlViacticMainFormPartial", remissionGuideRiver);
            }

            TempData.Keep("remissionGuideRiver");

            return PartialView("_RemissionGuideRiverInternControlViaticMainFormPartial", new RemissionGuideRiver());
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

        #region PRINT REPORT
        [HttpPost, ValidateInput(false)]
        public JsonResult PRViaticFluvial(string codeReport, LiquidationFreightRiver liquidationFreightriver,
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

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }
        #endregion
    }
}