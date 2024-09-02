using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryPeriodCloseController : DefaultController
    {

        public ActionResult Index()
        {
            return View();
        }
  

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult InventoryPeriodCloseResults(InventoryPeriod InventoryPeriod,

                                                  int? anio,


                                                  int? id_InventoryPeriodType

                                                  )
        {
            var model = db.InventoryPeriod.ToList();

            #region  FILTERS



            if (id_InventoryPeriodType != null && id_InventoryPeriodType > 0)
            {
                model = model.Where(o => o.id_PeriodType == id_InventoryPeriodType).ToList();
            }



            if (/*InventoryPeriod.id_warehouse != null &&*/ InventoryPeriod.id_warehouse > 0)
            {
                model = model.Where(o => o.id_warehouse == InventoryPeriod.id_warehouse).ToList();
            }

            if (anio != null && anio > 0)
            {
                model = model.Where(o => o.year == anio).ToList();
            }

            model = model.Where(o => o.id_Company == this.ActiveCompanyId && o.id_Division == ActiveDivision.id && o.id_BranchOffice == ActiveSucursal.id).ToList();
            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_InventoryPeriodCloseResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }


        #endregion

        #region InventoryPeriod HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodClosePartial()
        {
            var model = (TempData["model"] as List<InventoryPeriod>);
            model = model ?? new List<InventoryPeriod>();
            TempData.Keep("model");
            return PartialView("_InventoryPeriodClosePartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion

        #region Edit InventoryPeriod
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditInventoryPeriodClose(int id, int[] orderDetails)
        {
            InventoryPeriod InventoryPeriod = db.InventoryPeriod.Where(o => o.id == id).FirstOrDefault();

            if (InventoryPeriod == null)
            {

                InventoryPeriod = new InventoryPeriod
                {
                    id_Company = ActiveUser.id_company,
                    id_Division = ActiveDivision.id,
                    id_BranchOffice = ActiveSucursal.id,
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                    year = DateTime.Now.Year,
                    isActive = true,
                    id_userCreate = ActiveUser.id,
                    dateCreate = DateTime.Now,

                };
            }
            TempData["InventoryPeriod"] = InventoryPeriod;
            TempData.Keep("InventoryPeriod");

            return PartialView("_FormEditInventoryPeriodClose", InventoryPeriod);
        }
        #endregion

        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_InventoryPeriod)
        {
            TempData.Keep("InventoryPeriod");
            int index = db.InventoryPeriod.Where(o => o.id_Company == this.ActiveCompanyId && o.id_Division == ActiveDivision.id && o.id_BranchOffice == ActiveSucursal.id).OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_InventoryPeriod);
            var result = new
            {
                maximunPages = db.InventoryPeriod.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            InventoryPeriod InventoryPeriod = db.InventoryPeriod.Where(o => o.id_Company == this.ActiveCompanyId && o.id_Division == ActiveDivision.id && o.id_BranchOffice == ActiveSucursal.id).OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (InventoryPeriod != null)
            {
                TempData["InventoryPeriod"] = InventoryPeriod;
                TempData.Keep("InventoryPeriod");
                return PartialView("_InventoryPeriodCloseMainFormPartial", InventoryPeriod);
            }

            TempData.Keep("InventoryPeriod");

            return PartialView("_InventoryPeriodCloseMainFormPartial", new InventoryPeriod());
        }
        #endregion
 

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPerioClosedPartialUpdate(bool approve, InventoryPeriod item)
        {


            var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado == null)
            {
                TempData.Keep("InventoryPeriod");
                ViewData["EditMessage"] = ErrorMessage("No esta definido los Estados...");
                return PartialView("_InventoryPeriodCloseMainFormPartial", item);
            }


            var idestado = (from es in vestado
                            where es.valueCode.ToUpper().Trim() == "C"
                            select es).FirstOrDefault();

            DBContext dbemp = new DBContext();

            InventoryPeriod modelItem = dbemp.InventoryPeriod.Where(r => r.id == item.id).FirstOrDefault();
            if (modelItem != null)
            {

                InventoryPeriod conInventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);





                using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
                {
                    try
                    {
                        #region  DETAILS
                        if (conInventoryPeriod?.InventoryPeriodDetail != null)
                        {
                            var details = conInventoryPeriod.InventoryPeriodDetail.ToList();
                            foreach (var detail in details)
                            {
                                InventoryPeriodDetail inventoryPeriodDetail = modelItem.InventoryPeriodDetail.Where(d => d.id == detail.id).FirstOrDefault();

                                if (inventoryPeriodDetail != null)
                                {
                                    inventoryPeriodDetail.id_PeriodState = detail.id_PeriodState;
                                    inventoryPeriodDetail.dateInit = detail.dateInit;
                                    inventoryPeriodDetail.dateEnd = detail.dateEnd;
                                    //inventoryPeriodDetail.id = detail.id;
                                    //inventoryPeriodDetail.id_InventoryPeriod = detail.id_InventoryPeriod;

                                    if (idestado.id == detail.id_PeriodState)
                                    {
                                        inventoryPeriodDetail.dateClose = DateTime.Now;
                                        inventoryPeriodDetail.isClosed = true;
                                    }
                                    dbemp.InventoryPeriodDetail.Attach(inventoryPeriodDetail);
                                    dbemp.Entry(inventoryPeriodDetail).State = EntityState.Modified;
                                }
                            }
                        }
                        #endregion
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.id_Company = ActiveUser.id_company;
                        modelItem.id_Division = ActiveDivision.id;
                        modelItem.id_BranchOffice = ActiveSucursal.id;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.id_userCreate = ActiveUser.id;

                        modelItem.isActive = item.isActive;
                        modelItem.id_warehouse = item.id_warehouse;
                        dbemp.InventoryPeriod.Attach(modelItem);
                        dbemp.Entry(modelItem).State = EntityState.Modified;
                        dbemp.SaveChanges();
                        trans.Commit();
                        TempData["InventoryPeriod"] = modelItem;
                        TempData.Keep("InventoryPeriod");
                        ViewData["EditMessage"] = SuccessMessage("Periodo: " + item.year + " guardada exitosamente");

                    }
                    catch (Exception e)
                    {
                        TempData.Keep("InventoryPeriod");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("InventoryPeriod");
            return PartialView("_InventoryPeriodCloseMainFormPartial", modelItem);
        }


        #region InventoryPeriod Gridview

        [ValidateInput(false)]
        public ActionResult InventoryPeriodPartial(int? id)
        {
            if (id != null)
            {
                ViewData["InventoryPeriodToCopy"] = db.InventoryPeriod.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.InventoryPeriod.Where(o => o.id_Company == this.ActiveCompanyId);
            return PartialView("_InventoryPeriodClosePartial", model.ToList());
        }





        #endregion


        #region  Inventory details

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodCloseDetailPartial()
        {
            InventoryPeriod inventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);
            inventoryPeriod = inventoryPeriod ?? new InventoryPeriod();
            inventoryPeriod.InventoryPeriodDetail = inventoryPeriod.InventoryPeriodDetail ?? new List<InventoryPeriodDetail>();

            var model = inventoryPeriod.InventoryPeriodDetail;

            TempData.Keep("InventoryPeriod");

            return PartialView("_Details", model.ToList());
        }

        private bool validarDetalle(InventoryPeriodDetail inventoryPeriodDetail, ref string mensaje, ref int IdSiguiente, ref int IdEStadoA)
        {
            bool result = true;
            IdSiguiente = 0;
            int idEstadActivre = 0;
            TempData["EditMessage"] = null;
           InventoryPeriod inventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);

            inventoryPeriod = inventoryPeriod ?? db.InventoryPeriod.Where(i => i.id == inventoryPeriod.id).FirstOrDefault();
            inventoryPeriod = inventoryPeriod ?? new InventoryPeriod();

            var cantre = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && inventoryPeriodDetail.dateInit >= x.dateInit && inventoryPeriodDetail.dateInit <= x.dateEnd).ToList().Count();
            if (cantre > 0)
            {
                mensaje = ErrorMessage("Ya existe un Periodo con el mismo rango de fecha");
                TempData.Keep("InventoryPeriod");
                ViewData["EditMessage"] = mensaje;

                TempData["EditMessage"] = mensaje;

                TempData.Keep("EditMessage");
                result = false;
            }


            var cantrefin = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && inventoryPeriodDetail.dateEnd >= x.dateInit && inventoryPeriodDetail.dateEnd <= x.dateEnd).ToList().Count();
            if (cantrefin > 0)
            {
                mensaje = ErrorMessage("Y existe un Periodo con el mismo rango de fecha");
                TempData.Keep("InventoryPeriod");
                ViewData["EditMessage"] = mensaje;
                TempData["EditMessage"] = mensaje;

                TempData.Keep("EditMessage");
                result = false;
            }

            var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado != null)
            {
                var idestado = (from es in vestado
                                where es.valueCode.ToUpper().Trim() == "A"
                                select es).FirstOrDefault();

                if (idestado != null)
                {

                    idEstadActivre = idestado.id;
                    if (inventoryPeriodDetail.id_PeriodState == idestado.id)
                    {
                        var cantestado = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && inventoryPeriodDetail.id_PeriodState == x.id_PeriodState).ToList().Count();

                        if (cantestado > 0)
                        {
                            TempData.Keep("InventoryPeriod");
                            ViewData["EditMessage"] = ErrorMessage("Y existe un Periodo Activo");
                            mensaje = ErrorMessage("Y existe un Perido Activo");
                            TempData["EditMessage"] = mensaje;

                            TempData.Keep("EditMessage");
                            result = false;
                        }
                    }
                    else
                    {
                        IdEStadoA = idestado.id;
                    }

                }


                idestado = (from es in vestado
                            where es.valueCode.ToUpper().Trim() == "C"
                            select es).FirstOrDefault();

                if (idestado != null)
                {
                    if (inventoryPeriodDetail.id_PeriodState == idestado.id)
                    {
                        var idsi = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idestado.id != x.id_PeriodState && x.periodNumber > inventoryPeriodDetail.periodNumber).FirstOrDefault();

                        if (idsi == null)
                        {
                            var idsig = db.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idestado.id != x.id_PeriodState && x.InventoryPeriod.year > inventoryPeriodDetail.InventoryPeriod.year
                            && x.InventoryPeriod.id_Company  == inventoryPeriodDetail.InventoryPeriod.id_Company && x.InventoryPeriod.id_Division == inventoryPeriodDetail.InventoryPeriod.id_Division
                            && x.InventoryPeriod.id_BranchOffice == inventoryPeriodDetail.InventoryPeriod.id_BranchOffice && x.InventoryPeriod.id_warehouse == inventoryPeriodDetail.InventoryPeriod.id_warehouse

                            ).FirstOrDefault();
                            if (idsig == null)
                            {
                                TempData.Keep("InventoryPeriod");
                                ViewData["EditMessage"] = ErrorMessage("No existe otro periodo Pendiente");
                                mensaje = ErrorMessage("No existe otro periodo Pendiente");
                                TempData["EditMessage"] = mensaje;

                                TempData.Keep("EditMessage");

                                ViewData["EditMessage"] = mensaje;

                                result = false;
                            }
                            else
                            {

                                var registroactivoa = db.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idEstadActivre == x.id_PeriodState && x.InventoryPeriod.year > inventoryPeriodDetail.InventoryPeriod.year
                           && x.InventoryPeriod.id_Company == inventoryPeriodDetail.InventoryPeriod.id_Company && x.InventoryPeriod.id_Division == inventoryPeriodDetail.InventoryPeriod.id_Division
                           && x.InventoryPeriod.id_BranchOffice == inventoryPeriodDetail.InventoryPeriod.id_BranchOffice && x.InventoryPeriod.id_warehouse == inventoryPeriodDetail.InventoryPeriod.id_warehouse).ToList().Count();

                                if (registroactivoa <= 0)
                                {
                                    IdSiguiente = idsig.id;
                                }
                            }
                        }
                        else
                        {

                            var registroactivo = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idEstadActivre == x.id_PeriodState).ToList().Count();

                            if (registroactivo <= 0)
                            {
                                IdSiguiente = idsi.id;
                            }
                        }
                    }

                }

            }

            return result;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodDetailPartialUpdate(int id)
        {

            
            InventoryPeriod inventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);

            InventoryPeriodDetail inventoryPeriodDetail = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == id).FirstOrDefault();


            var id_PeriodState = inventoryPeriodDetail.id_PeriodState;
            var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado != null)
            {
                var idestado = (from es in vestado
                                where es.valueCode.ToUpper().Trim() == "C"
                                select es).FirstOrDefault();



                inventoryPeriodDetail.id_PeriodState = idestado.id;
                   
                }


                


                inventoryPeriod = inventoryPeriod ?? db.InventoryPeriod.Where(i => i.id == inventoryPeriod.id).FirstOrDefault();
            inventoryPeriod = inventoryPeriod ?? new InventoryPeriod();

            if (ModelState.IsValid)
            {
                try
                {

                    var modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == inventoryPeriodDetail.id).FirstOrDefault();


                    if (modelItem != null)
                    {
                        string mensaje = "";
                        int IdSiguiente = 0;
                        int IdSEstadoA = 0;

                        if (!validarDetalle(inventoryPeriodDetail, ref mensaje, ref IdSiguiente, ref IdSEstadoA))
                        {
                            inventoryPeriodDetail.id_PeriodState = id_PeriodState;
                            ViewData["EditError"] = mensaje;

                            TempData.Keep("InventoryPeriod");
                            var modelre = inventoryPeriod?.InventoryPeriodDetail.ToList() ?? new List<InventoryPeriodDetail>();

                            //return PartialView("_Details", modelre.ToList());
                            TempData.Keep("InventoryPeriod");

                            var resultd = new
                            {
                                id = 1,
                                mensaje= mensaje
                            };
                            return Json(resultd, JsonRequestBehavior.AllowGet);

                        }

                        modelItem.dateInit = inventoryPeriodDetail.dateInit;
                        modelItem.dateEnd = inventoryPeriodDetail.dateEnd;
                        modelItem.id_PeriodState = inventoryPeriodDetail.id_PeriodState;
                        this.UpdateModel(modelItem);

                        if (IdSiguiente > 0 && IdSEstadoA > 0)
                        {
                            modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == IdSiguiente).FirstOrDefault();
                            if(modelItem !=null)
                            { 
                            modelItem.id_PeriodState = IdSEstadoA;
                            }


                        }
                        TempData["InventoryPeriod"] = inventoryPeriod;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("InventoryPeriod");

            var model = inventoryPeriod?.InventoryPeriodDetail.ToList() ?? new List<InventoryPeriodDetail>();

            //return PartialView("_Details", model.ToList());
            var result = new
            {
                id = 0
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        #endregion
    }
}
