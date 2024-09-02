using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;


namespace DXPANACEASOFT.Controllers
{
    public class TActionController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        //DXPANACEASOFT.Models.DBContext db = new DXPANACEASOFT.Models.DBContext();

        //[ValidateInput(false)]
        public ActionResult TActionPartial()
        {
            var model = db.TAction;
            return PartialView("_TActionPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult TActionPartialAddNew(DXPANACEASOFT.Models.TAction item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.TAction.Add(item);
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
                message = item.name
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult TActionPartialUpdate(DXPANACEASOFT.Models.TAction item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.TAction.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            
                            modelItem.id_controller = item.id_controller;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            db.TAction.Attach(modelItem);
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
                    message = "Por favor corrigir los errores!!!",
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                code = item.id,
                message = item.name
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult TActionPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.TAction.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                        }

                        db.TAction.Attach(item);
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

            var model = db.TAction;
            return PartialView("_TActionPartial", model.ToList());
        }

        public void DeleteSelectedTAction(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.TAction.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            db.TAction.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
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

        //public JsonResult TControllerByTAction(int id_controller)
        //{
        //    var model = db.TController.Where(d => d.id == id_controller && d.isActive).ToList();

        //    var result = model.Select(d => new { d.id, d.name });

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
    }
}