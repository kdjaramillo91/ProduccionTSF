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
    public class PersonTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult PersonTypesPartial()
        {
            var model = db.PersonType;
            return PartialView("_PersonTypesPartial", model.ToList());
        }

        public ActionResult PersonTypeDetailPartial(PersonType persontype)
        {
            var model = db.PersonType.FirstOrDefault(i => i.id == persontype.id);
            return PartialView("_PersonTypeDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PersonTypesPartialAddNew(DXPANACEASOFT.Models.PersonType item)
        {
            var model = db.PersonType;
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
            return PartialView("_PersonTypesPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PersonTypesPartialUpdate(DXPANACEASOFT.Models.PersonType item)
        {
            var model = db.PersonType;
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
            return PartialView("_PersonTypesPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PersonTypesPartialDelete(System.Int32 id)
        {
            var model = db.PersonType;
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
            return PartialView("_PersonTypesPartial", model.ToList());
        }

        [HttpPost]
        public void DeleteSelectedPersonTypes(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var persontypes = db.PersonType.Where(i => ids.Contains(i.id));
                        foreach (var persontype in persontypes)
                        {
                            persontype.isActive = false;
                            db.Entry(persontype).State = EntityState.Modified;
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