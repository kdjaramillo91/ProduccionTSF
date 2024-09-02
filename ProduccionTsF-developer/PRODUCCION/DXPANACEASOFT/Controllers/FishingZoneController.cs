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
using Newtonsoft.Json;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Services;
using DevExpress.Web.Mvc;


namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class FishingZoneController : DefaultController
    {
       // private DBContext db = new DBContext();

        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }      

        public ActionResult FishingZonePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.FishingZone.FirstOrDefault(b => b.id == keyToCopy);
            }

            var model = db.FishingZone.Where(fz => fz.id_company == this.ActiveCompanyId);
            return PartialView("_FishingZonePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FishingZonePartialAddNew(FishingZone fishingZone)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        fishingZone.id_company = this.ActiveCompanyId;
                        fishingZone.id_userCreate = ActiveUser.id;
                        fishingZone.dateCrete = DateTime.Now;
                        fishingZone.id_userUpdate = ActiveUser.id;
                        fishingZone.dateUpdate = DateTime.Now;

                        db.FishingZone.Add(fishingZone);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Zona de Pesca: " + fishingZone.name + " guardado exitosamente");
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

            var model = db.FishingZone.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_FishingZonePartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult FishingZonePartialUpdate(FishingZone fishingZone)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelFishingZone = db.FishingZone.FirstOrDefault(fz => fz.id == fishingZone.id);
                        if (modelFishingZone != null)
                        {

                            modelFishingZone.code = fishingZone.code;
                            modelFishingZone.name = fishingZone.name;
                            modelFishingZone.description = fishingZone.description;
                            modelFishingZone.isActive = fishingZone.isActive;

                            modelFishingZone.id_userUpdate = ActiveUser.id;
                            modelFishingZone.dateUpdate = DateTime.Now;

                            db.FishingZone.Attach(modelFishingZone);
                            db.Entry(modelFishingZone).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Zone de Pesca: " + fishingZone.name + " guardado exitosamente");
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

            var model = db.FishingZone.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_FishingZonePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FishingZonePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var fishingZone = db.FishingZone.FirstOrDefault(fz => fz.id == id);
                        if (fishingZone != null)
                        {
                            fishingZone.isActive = false;
                            fishingZone.id_userUpdate = ActiveUser.id;
                            fishingZone.dateUpdate = DateTime.Now;
                        }

                        db.FishingZone.Attach(fishingZone);
                        db.Entry(fishingZone).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Zona de Pesca: " + (fishingZone?.name ?? "") + " desactivado exitosamente");
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

            var model = db.FishingZone.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_FishingZonePartial", model.ToList());
        }


        [HttpPost]
        public ActionResult DeleteSelectedFishingZone(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelFishingZone = db.FishingZone.Where(i => ids.Contains(i.id));
                        foreach (var fishingZone in modelFishingZone)
                        {
                            fishingZone.isActive = false;

                            fishingZone.id_userUpdate = ActiveUser.id;
                            fishingZone.dateUpdate = DateTime.Now;

                            db.FishingZone.Attach(fishingZone);
                            db.Entry(fishingZone).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Zonas de Pesca desactivados exitosamente");
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

            var model = db.FishingZone.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_FishingZonePartial", model.ToList());
        }



        // GET: FishingZones/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FishingZone fishingZone = await db.FishingZone.FindAsync(id);
            if (fishingZone == null)
            {
                return HttpNotFound();
            }
            return View(fishingZone);
        }

       
        public ActionResult FishingZoneDetailFishingSitePartial(int? id_fishingZone)
        {
            FishingZone fishingZone = db.FishingZone.FirstOrDefault(o => o.id == id_fishingZone);
            //var model = itemType?.ItemTypeCategory?.ToList() ?? new List<ItemTypeCategory>();
            var model = db.FishingSite.Where(w => w.id_FishingZone == id_fishingZone).ToList() ?? new List<FishingSite>();

            return PartialView("_FishingZoneDetailFishingSitePartial", model);//.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult FishingZoneSiteData(int? id_fishingZone)
        {
 
                
            var _fishingZone = DataProviderFishingZone.FishingZoneById(id_fishingZone) ?? new FishingZone();
            
            var result = new
            {
                _fishingSite = _fishingZone.FishingSite.Where(a => a.isActive).Select(
                fs => new
                {
                    id = fs.id,
                    name = fs.name
                }

                ).ToList(),

            };

        

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FishingZoneGetSites(int id_FishingZone, string ValueField)
        {
            return GridViewExtension.GetComboBoxCallbackResult(p => {
                p.ValueField = ValueField;
                p.BindList( DataProviderFishingSite.FishingSites(this.ActiveCompanyId, id_FishingZone));
            });
        }

        [HttpPost]
        public JsonResult ValidateCodeFishingZone(int id_fishingZone, string code)
        {
            FishingZone fishingZone = db.FishingZone.FirstOrDefault(il => il.code == code);

            if (fishingZone == null || fishingZone.id == id_fishingZone)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Zona de Pesca" }, JsonRequestBehavior.AllowGet);
        }

    }
}
