using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class PermissionController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PermissionsPartial()
        {
            var model = db.Permission;
            return PartialView("_PermissionsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PermissionsPartialAddNew(DXPANACEASOFT.Models.Permission item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_company = ActiveCompany.id;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.Permission.Add(item);
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
        public JsonResult PermissionsPartialUpdate(DXPANACEASOFT.Models.Permission item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Permission.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.Permission.Attach(modelItem);
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
        public ActionResult PermissionsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Permission.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            //model.Remove(item);
                            //db.SaveChanges();
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                            item.isActive = false;
                            //this.UpdateModel(item);

                        }

                        db.Permission.Attach(item);
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
            var model = db.Permission;
            return PartialView("_PermissionsPartial", model.ToList());
        }

        [HttpPost]
        public void DeleteSelectedPermissions(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Permission.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;
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
    }
}