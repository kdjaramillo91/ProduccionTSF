using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using DevExpress.Data.Utils;
using DevExpress.Web;
using DXPANACEASOFT.DataProviders;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class IdentificationTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }


        [ValidateInput(false)]
        public ActionResult IdentificationTypesPartial()
        {
            var model = db.IdentificationType;
            return PartialView("_IdentificationTypesPartial", model.ToList());
        }

        public ActionResult IdentificationTypeDetailPartial(IdentificationType identificationtype)
        {
            var model = db.IdentificationType.FirstOrDefault(i => i.id == identificationtype.id);
            return PartialView("_IdentificationTypeDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult IdentificationTypesPartialAddNew(DXPANACEASOFT.Models.IdentificationType item)
        {
            var model = db.IdentificationType;
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // CAMPOS DE AUDITORIA
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        // FORMA DE QUEMAR EL ID DEL LA COMPANIA
                        item.id_company = this.ActiveCompanyId;

                        model.Add(item);
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
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_IdentificationTypesPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult IdentificationTypesPartialUpdate(DXPANACEASOFT.Models.IdentificationType item)
        {
            var model = db.IdentificationType;
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
            return PartialView("_IdentificationTypesPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult IdentificationTypesPartialDelete(System.Int32 id)
        {
            var model = db.IdentificationType;
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
            return PartialView("_IdentificationTypesPartial", model.ToList());
        }

        [HttpPost]
        public void DeleteSelectedIdentificationTypes(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var identificationtypes = db.IdentificationType.Where(i => ids.Contains(i.id));
                        foreach (var identificationtype in identificationtypes)
                        {
                            identificationtype.is_Active = false;
                            db.Entry(identificationtype).State = EntityState.Modified;
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