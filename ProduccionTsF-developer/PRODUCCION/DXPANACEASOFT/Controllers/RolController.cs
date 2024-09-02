using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class RolController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RolDetailPartial(Rol rol)
        {
            var model = db.Rol.FirstOrDefault(i => i.id == rol.id);
            return PartialView("_RolDetailPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult RolesPartial()
        {
            var model = db.Rol;
            return PartialView("_RolesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RolesPartialAddNew(DXPANACEASOFT.Models.Rol item)
        {
            var model = db.Rol;
            if (ModelState.IsValid)
            {
                try
                {
                    // CAMPOS DE AUDITORIA
                    item.id_company = this.ActiveCompanyId;
                    item.id_userCreate = ActiveUser.id;
                    item.dateCreate = DateTime.Now;
                    item.id_userUpdate = ActiveUser.id;
                    item.dateUpdate = DateTime.Now;

                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_RolesPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RolesPartialUpdate(DXPANACEASOFT.Models.Rol item)
        {
            var model = db.Rol;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_RolesPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RolesPartialDelete(System.Int32 id)
        {
            var model = db.Rol;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_RolesPartial", model.ToList());
        }

        [HttpPost]
        public void DeleteSelectedRoles(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var roles = db.Rol.Where(i => ids.Contains(i.id));
                        foreach (var rol in roles)
                        {
                            rol.isActive = false;
                            db.Entry(rol).State = EntityState.Modified;
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