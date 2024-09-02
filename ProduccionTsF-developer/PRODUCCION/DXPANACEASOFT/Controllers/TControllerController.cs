using DevExpress.Web.Mvc;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class TControllerController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }


        [ValidateInput(false)]
        public ActionResult TControllerPartial()
        {
            var model = db.TController;
            return PartialView("_TControllerPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult TControllerPartialAddNew(DXPANACEASOFT.Models.TController item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.TController.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return Json(new
                        {
                            code = (e.HResult < 0) ? e.HResult : -e.HResult,
                            message = e.Message
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            else
            {

                return Json(new
                {
                    code = -1,
                    message = "Por favor corregir los errores!!!",
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                code = item.id,
                message = item.name,
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult TControllerPartialUpdate(DXPANACEASOFT.Models.TController item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.TController.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            db.TController.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return Json(new
                        {
                            code = (e.HResult < 0) ? e.HResult : -e.HResult,
                            message = e.Message
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            else
            {
                return Json(new
                {
                    code = -1,
                    message = "Por favor corregir los errores!!!",
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                code = item.id,
                message = item.name,
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult TControllerPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.TController.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            //item.id_userUpdate = ActiveUser.id;
                            //item.dateUpdate = DateTime.Now;

                        }
                        db.TController.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

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
            var model = db.TController;
            return PartialView("_TControllerPartial", model.ToList());
        }

        public void DeleteSelectedTController(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var tc = db.TController.Where(i => ids.Contains(i.id));
                        foreach (var tcontroller in tc)
                        {
                            tcontroller.isActive = false;

                            db.TController.Attach(tcontroller);
                            db.Entry(tcontroller).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                    }
                }
            }
        }
    }
}