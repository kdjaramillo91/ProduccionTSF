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
    [Authorize]
    public class BranchOfficeController : DefaultController
    {
        [HttpPost, ValidateInput(false)]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region BRANCHOFFICE GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult BranchOfficesPartial(int? keyToCopy)
        {
            if(keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.BranchOffice.FirstOrDefault(b => b.id == keyToCopy);
            }

            var model = db.BranchOffice.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_BranchOfficesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BranchOfficesPartialAddNew(DXPANACEASOFT.Models.BranchOffice item)
        {
            if (ModelState.IsValid)
            {
                using(DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;
                        
                        db.BranchOffice.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Sucursal: " + item.name + " guardada exitosamente");
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

            var model = db.BranchOffice.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_BranchOfficesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BranchOfficesPartialUpdate(DXPANACEASOFT.Models.BranchOffice item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.BranchOffice.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.id_division = item.id_division;

                            modelItem.name = item.name;
                            modelItem.code = item.code;

                            modelItem.email = item.email;
                            modelItem.address = item.address;
                            modelItem.phoneNumber = item.phoneNumber;

                            modelItem.description = item.description;

                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.BranchOffice.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Sucursal: " + item.name + " guardada exitosamente");
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

            var model = db.BranchOffice.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_BranchOfficesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BranchOfficesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.BranchOffice.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {   
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.BranchOffice.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Sucursal: " + item?.name ?? "" + " desactivada exitosamente");
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

            var model = db.BranchOffice.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_BranchOfficesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedBranchOffices(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.BranchOffice.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.BranchOffice.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Sucursales desactivadas exitosamente");
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

            var model = db.BranchOffice.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_BranchOfficesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BranchOfficesCopy(int copyId)
        {
            return View("Index");
        }

        #endregion

        #region MASTER DETAILS VIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult BranchOfficesDetailEmissionPointsPartial(int? id_branchOffice)
        {
            BranchOffice branchOffice = db.BranchOffice.FirstOrDefault(o => o.id == id_branchOffice);
            var model = branchOffice?.EmissionPoint?.ToList() ?? new List<EmissionPoint>();

            return PartialView("_BranchOfficeDetailEmissionPointsPartial", model.ToList());
        }

        #endregion
        #region REPORTS

        [HttpPost, ValidateInput(false)]
        public ActionResult BranchOfficesReport()
        {
            BranchOfficesReport report = new BranchOfficesReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_BranchOfficesReport", report);            
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BranchOfficeDetailReport(int id)
        {
            BranchOfficeDetailReport report = new BranchOfficeDetailReport();
            report.Parameters["id_branchOffice"].Value = id;
            return PartialView("_BranchOfficesReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult DivisionByCompany(int id_company)
        {
            var model = db.Division.Where(d => d.id_company == id_company && d.isActive).ToList();

            var result = model.Select(d => new { d.id, d.name });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult DivisionData(int id_division)
        {
            var model = db.Division.FirstOrDefault(d => d.id == id_division);

            if(model == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                model.ruc,
                model.address,
                model.phoneNumber,
                model.email
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateCode(int id_division, int id_branchOffice, string code)
        {
            BranchOffice branchOffice = db.BranchOffice.FirstOrDefault(b => b.id != id_branchOffice && b.id_division == id_division && b.code.Equals(code) && b.isActive);

            if(branchOffice == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra sucursal" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}



