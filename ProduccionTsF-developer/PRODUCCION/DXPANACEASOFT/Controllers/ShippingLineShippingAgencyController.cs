using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class ShippingLineShippingAgencyController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult ShippingLineShippingAgencyResults(ShippingLineShippingAgency ShippingLineShippingAgency
                                                  )
        {
            
            var model = db.ShippingLineShippingAgency.ToList();

            #region  FILTERS
          


            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_ShippingLineShippingAgencyResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingLineShippingAgencyPartial()
        {
            var model = (TempData["model"] as List<ShippingLineShippingAgency>);

            model = db.ShippingLineShippingAgency.ToList();

            model = model ?? new List<ShippingLineShippingAgency>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_ShippingLineShippingAgencyPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit ShippingLineShippingAgency
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditShippingLineShippingAgency(int id, int[] orderDetails)
        {
            ShippingLineShippingAgency ShippingLineShippingAgency = db.ShippingLineShippingAgency.Where(o => o.id == id).FirstOrDefault();

            if (ShippingLineShippingAgency == null)
            {

                ShippingLineShippingAgency = new ShippingLineShippingAgency
                {
                    fecha_inicio = DateTime.Now,
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now
         
                };
            }
            TempData["ShippingLineShippingAgency"] = ShippingLineShippingAgency;
            TempData.Keep("ShippingLineShippingAgency");

            return PartialView("_FormEditShippingLineShippingAgency", ShippingLineShippingAgency);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_ShippingLineShippingAgency)
        {
            TempData.Keep("ShippingLineShippingAgency");
            int index = db.ShippingLineShippingAgency.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_ShippingLineShippingAgency);
            var result = new
            {
                maximunPages = db.ShippingLineShippingAgency.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

 
     
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            ShippingLineShippingAgency ShippingLineShippingAgency = db.ShippingLineShippingAgency.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (ShippingLineShippingAgency != null)
            {
                TempData["ShippingLineShippingAgency"] = ShippingLineShippingAgency;
                TempData.Keep("ShippingLineShippingAgency");
                return PartialView("_ShippingLineShippingAgencyMainFormPartial", ShippingLineShippingAgency);
            }

            TempData.Keep("ShippingLineShippingAgency");

            return PartialView("_ShippingLineShippingAgencyMainFormPartial", new ShippingLineShippingAgency());
        }
        #endregion
        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id,int id_ShippingLine, int id_ShippingAgency)
        {
            TempData.Keep("ShippingLineShippingAgency");


            bool rept = false;

            var cantre = db.ShippingLineShippingAgency.Where(x => x.id != id && x.id_ShippingLine == id_ShippingLine && x.id_ShippingAgency == id_ShippingAgency).ToList().Count();
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
        #endregion
        #region Save and Update
        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingLineShippingAgencyPartialAddNew(ShippingLineShippingAgency item)
        {
            ShippingLineShippingAgency conShippingLineShippingAgency = (TempData["ShippingLineShippingAgency"] as ShippingLineShippingAgency);


            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region ShippingLineShippingAgency

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;



                    dbemp.ShippingLineShippingAgency.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["ShippingLineShippingAgency"] = item;
                    TempData.Keep("ShippingLineShippingAgency");
                    ViewData["EditMessage"] = SuccessMessage("Linea: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("ShippingLineShippingAgency");
                    item = (TempData["ShippingLineShippingAgency"] as ShippingLineShippingAgency);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }



            //return PartialView("_ShippingLineShippingAgencyMainFormPartial", item);

            return PartialView("_ShippingLineShippingAgencyPartial", dbemp.ShippingLineShippingAgency.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingLineShippingAgencyPartialUpdate(ShippingLineShippingAgency item)
        {
            ShippingLineShippingAgency modelItem = db.ShippingLineShippingAgency.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                ShippingLineShippingAgency conShippingLineShippingAgency = (TempData["ShippingLineShippingAgency"] as ShippingLineShippingAgency);


        


                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region ShippingLineShippingAgency
                        modelItem.id_ShippingAgency = item.id_ShippingAgency;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.id_ShippingLine = item.id_ShippingLine;
                        modelItem.fecha_inicio = item.fecha_inicio;
                        modelItem.fecha_fin = item.fecha_fin;




                        #endregion




                        db.ShippingLineShippingAgency.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["ShippingLineShippingAgency"] = modelItem;
                        TempData.Keep("ShippingLineShippingAgency");
                        ViewData["EditMessage"] = SuccessMessage("Linea: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("ShippingLineShippingAgency");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("ShippingLineShippingAgency");


            return PartialView("_ShippingLineShippingAgencyPartial", db.ShippingLineShippingAgency.ToList());
        }
        #endregion
        #region ShippingLineShippingAgency Gridview

        [ValidateInput(false)]
        public ActionResult ShippingLineShippingAgencyPartial(int? id)
        {
            if (id != null)
            {
                ViewData["ShippingLineShippingAgencyToCopy"] = db.ShippingLineShippingAgency.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.ShippingLineShippingAgency.ToList();
            return PartialView("_ShippingLineShippingAgencyPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingLineShippingAgencyPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ShippingLineShippingAgency.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                    
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.ShippingLineShippingAgency.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Linea : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.ShippingLineShippingAgency.ToList();
            return PartialView("_ShippingLineShippingAgencyPartial", model.ToList());
        }

        public ActionResult DeleteSelectedShippingLineShippingAgency(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var ShippingLineShippingAgencys = db.ShippingLineShippingAgency.Where(i => ids.Contains(i.id));
                        foreach (var vShippingLineShippingAgency in ShippingLineShippingAgencys)
                        {
                            

                            vShippingLineShippingAgency.id_userUpdate = ActiveUser.id;
                            vShippingLineShippingAgency.dateUpdate = DateTime.Now;

                            db.ShippingLineShippingAgency.Attach(vShippingLineShippingAgency);
                            db.Entry(vShippingLineShippingAgency).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Linea desactivadas exitosamente");
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

            var model = db.ShippingLineShippingAgency.ToList();
            return PartialView("_ShippingLineShippingAgencyPartial", model.ToList());
        }

        #endregion
    }
}
