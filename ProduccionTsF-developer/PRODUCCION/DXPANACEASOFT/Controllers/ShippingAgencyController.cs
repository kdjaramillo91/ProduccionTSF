using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class ShippingAgencyController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult ShippingAgencyResults(ShippingAgency ShippingAgency,
                                                  string scode
                                                  )
        {
            var model = db.ShippingAgency.ToList();

            #region  FILTERS
            if (!string.IsNullOrEmpty(scode))
            {
                model = model.Where(o => o.code == scode).ToList();
            }


            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_ShippingAgencyResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingAgencyPartial()
        {
            var model = (TempData["model"] as List<ShippingAgency>);

             model = db.ShippingAgency.ToList();

            model = model ?? new List<ShippingAgency>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_ShippingAgencyPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit ShippingAgency
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditShippingAgency(int id, int[] orderDetails)
        {
            ShippingAgency ShippingAgency = db.ShippingAgency.Where(o => o.id == id).FirstOrDefault();

            if (ShippingAgency == null)
            {

                ShippingAgency = new ShippingAgency
                {

                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                    isActive = true,
                };
            }
            TempData["ShippingAgency"] = ShippingAgency;
            TempData.Keep("ShippingAgency");

            return PartialView("_FormEditShippingAgency", ShippingAgency);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_ShippingAgency)
        {
            TempData.Keep("ShippingAgency");
            int index = db.ShippingAgency.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_ShippingAgency);
            var result = new
            {
                maximunPages = db.ShippingAgency.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id_ShippingAgency,string codio)
        {
            TempData.Keep("ShippingAgency");


            bool rept = false;

            var cantre = db.ShippingAgency.Where(x => x.id != id_ShippingAgency && x.code == codio).ToList().Count();
            if (cantre > 0)
            {
               rept = true;
            }
       
            var result = new
            {

                rept = rept
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            ShippingAgency ShippingAgency = db.ShippingAgency.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (ShippingAgency != null)
            {
                TempData["ShippingAgency"] = ShippingAgency;
                TempData.Keep("ShippingAgency");
                return PartialView("_ShippingAgencyMainFormPartial", ShippingAgency);
            }

            TempData.Keep("ShippingAgency");

            return PartialView("_ShippingAgencyMainFormPartial", new ShippingAgency());
        }
        #endregion
        #region Validacion
        public Boolean validate(ShippingAgency item)
        {
            Boolean wsreturn = true;
            try
            {
                ///  ShippingAgency conShippingAgency = (TempData["ShippingAgency"] as ShippingAgency);
                if (String.IsNullOrEmpty(item.name))
                {
                    TempData.Keep("ShippingAgency");
                    ViewData["EditMessage"] = ErrorMessage("Ingrse el Nombre del Agencia");
   
                    wsreturn = false;
                }

                if (String.IsNullOrEmpty(item.code))
                {
                    TempData.Keep("ShippingAgency");
                    ViewData["EditMessage"] = ErrorMessage("Ingrse el Codigo del Agencia");
               
                    wsreturn = false;
                }
                else
                {


                    if (item.code.ToString().ToString().Contains(" ") && item.code.ToString().ToString().Length > 0)
                    {
                        TempData.Keep("ShippingAgency");
                        ViewData["EditMessage"] = ErrorMessage("El Codigo del Agencia no puede tener espacio");
                   
                        wsreturn = false;
                    }
                }


              

            }
            catch (Exception)
            {


            }

            return wsreturn;

        }
        #endregion
        #region Save and Update
        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingAgencyPartialAddNew( ShippingAgency item)
        {
            ShippingAgency conShippingAgency = (TempData["ShippingAgency"] as ShippingAgency);

            if (!validate(item))
            {
                //return PartialView("_ShippingAgencyMainFormPartial", item);
   
                return PartialView("_ShippingAgencyPartial", db.ShippingAgency.ToList());
            }
            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region ShippingAgency

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;

           

                    dbemp.ShippingAgency.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["ShippingAgency"] = item;
                    TempData.Keep("ShippingAgency");
                    ViewData["EditMessage"] = SuccessMessage("Agencia: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("ShippingAgency");
                    item = (TempData["ShippingAgency"] as ShippingAgency);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
             
                    trans.Rollback();
                }
            }

         

            //return PartialView("_ShippingAgencyMainFormPartial", item);

            return PartialView("_ShippingAgencyPartial", dbemp.ShippingAgency.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingAgencyPartialUpdate(ShippingAgency item)
        {
            ShippingAgency modelItem = db.ShippingAgency.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                ShippingAgency conShippingAgency = (TempData["ShippingAgency"] as ShippingAgency);

              
                if (!validate(item))
                {
                
                    //return PartialView("_ShippingAgencyMainFormPartial", item);
                    return PartialView("_ShippingAgencyPartial", db.ShippingAgency.ToList());
                }



                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region ShippingAgency
                        modelItem.code = item.code;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.name = item.name;
                        modelItem.isActive = item.isActive;



                        #endregion


                      

                        db.ShippingAgency.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["ShippingAgency"] = modelItem;
                        TempData.Keep("ShippingAgency");
                        ViewData["EditMessage"] = SuccessMessage("Agencia: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("ShippingAgency");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                      
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("ShippingAgency");
            //return PartialView("_ShippingAgencyMainFormPartial", modelItem);
       
            return PartialView("_ShippingAgencyPartial", db.ShippingAgency.ToList());
        }
        #endregion
        #region ShippingAgency Gridview

        [ValidateInput(false)]
        public ActionResult ShippingAgencyPartial(int? id)
        {
            if (id != null)
            {
                ViewData["ShippingAgencyToCopy"] = db.ShippingAgency.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.ShippingAgency.ToList();
            return PartialView("_ShippingAgencyPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingAgencyPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ShippingAgency.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.ShippingAgency.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Agencia : " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.ShippingAgency.ToList();
            return PartialView("_ShippingAgencyPartial", model.ToList());
        }

        public ActionResult DeleteSelectedShippingAgency(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var ShippingAgencys = db.ShippingAgency.Where(i => ids.Contains(i.id));
                        foreach (var vShippingAgency in ShippingAgencys)
                        {
                            vShippingAgency.isActive = false;

                            vShippingAgency.id_userUpdate = ActiveUser.id;
                            vShippingAgency.dateUpdate = DateTime.Now;

                            db.ShippingAgency.Attach(vShippingAgency);
                            db.Entry(vShippingAgency).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Agencia desactivadas exitosamente");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.ShippingAgency.ToList();
            return PartialView("_ShippingAgencyPartial", model.ToList());
        }

        #endregion
    }
}
