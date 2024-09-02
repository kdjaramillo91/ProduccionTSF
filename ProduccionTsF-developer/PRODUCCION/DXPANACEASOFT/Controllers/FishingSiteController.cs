using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    public class FishingSiteController : DefaultController
    {
        
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult FishingSitePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.FishingSite.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.FishingSite.Where(fs => fs.id_company == this.ActiveCompanyId);
            return PartialView("_FishingSitePartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult FishingSitePartialAddNew(FishingSite fishingSite)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        fishingSite.id_company = this.ActiveCompanyId;
                        fishingSite.id_userCreate = ActiveUser.id;
                        fishingSite.dateCreate = DateTime.Now;
                        fishingSite.id_userUpdate = ActiveUser.id;
                        fishingSite.dateUpdate = DateTime.Now;

                        db.FishingSite.Add(fishingSite);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Sitio de Cosecha: " + fishingSite.name + " guardado exitosamente");
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

            var model = db.FishingSite.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_FishingSitePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FishingSitePartialUpdate(FishingSite fishingSite)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.FishingSite.FirstOrDefault(fs => fs.id == fishingSite.id);
                        if (modelItem != null)
                        {
                            modelItem.id_FishingZone = fishingSite.id_FishingZone;

                            modelItem.name = fishingSite.name;
                            modelItem.code = fishingSite.code;
                            modelItem.description = fishingSite.description;
                            modelItem.isActive = fishingSite.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.FishingSite.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Sitio de Cosecha: " + fishingSite.name + " guardado exitosamente");
                        }
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

            var model = db.FishingSite.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_FishingSitePartial", model.ToList());
        }
        

        [HttpPost, ValidateInput(false)]
        public ActionResult FishingSitePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var fishingSite = db.FishingSite.FirstOrDefault(fs => fs.id == id);
                        if (fishingSite != null)
                        {
                            fishingSite.isActive = false;
                            fishingSite.id_userUpdate = ActiveUser.id;
                            fishingSite.dateUpdate = DateTime.Now;
                        }

                        db.FishingSite.Attach(fishingSite);
                        db.Entry(fishingSite).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Sitio de Cosecha: " + (fishingSite?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.FishingSite.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_FishingSitePartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedFishingSite(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelFishingSite = db.FishingSite.Where(i => ids.Contains(i.id));
                        foreach (var fishingSite in modelFishingSite)
                        {
                            fishingSite.isActive = false;

                            fishingSite.id_userUpdate = ActiveUser.id;
                            fishingSite.dateUpdate = DateTime.Now;

                            db.FishingSite.Attach(fishingSite);
                            db.Entry(fishingSite).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Sitios de cosecha desactivados exitosamente");
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

            var model = db.FishingSite.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_FishingSitePartial", model.ToList());
        }

        public ActionResult FishingSiteDetailFishingZonePartial(int? id_fishingSite)
        {
            FishingSite fishingSite = db.FishingSite.FirstOrDefault(o => o.id == id_fishingSite);
            var model = new List<FishingZone>();
            model.Add(fishingSite.FishingZone);

            return PartialView("_ItemTypeDetailItemTypeCategoriesPartial", model);//.ToList());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

       
        [HttpPost]
        public JsonResult ValidateCodeFishingSite(int id_fishingSite, string code)
        {
            FishingSite fishingSite = db.FishingSite.FirstOrDefault(il => il.code == code);

            if (fishingSite == null || fishingSite.id == id_fishingSite)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro sitio de cosecha" }, JsonRequestBehavior.AllowGet);
        }

    }
}
