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
    public class DivisionController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Division GRIDVIEW

        [ValidateInput(false)]
        public ActionResult DivisionsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.Division.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.Division.Where(d => d.id_company == this.ActiveCompanyId);
            return PartialView("_DivisionsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionsPartialAddNew(Division item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.Division.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("División: " + item.name + " guardado exitosamente");
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

            var model = db.Division.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DivisionsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionsPartialUpdate(Division item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Division.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.id_company = item.id_company;
                            modelItem.ruc = item.ruc;
                            modelItem.name = item.name;
                            modelItem.description = item.description;

                            modelItem.address = item.address;
                            modelItem.email = item.email;
                            modelItem.phoneNumber = item.phoneNumber;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.Division.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("División: " + item.name + " guardado exitosamente");
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

            var model = db.Division.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DivisionsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Division.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }
                        db.Division.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("División: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.Division.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DivisionsPartial", model.ToList());
        }
        [HttpPost]
        public ActionResult DeleteSelectedDivisions(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Division.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.Division.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Divisiones desactivadas exitosamente");
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

            var model = db.Division.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DivisionsPartial", model.ToList());
        }


        #endregion

        #region MASTER DETAILS VIEW
        [HttpPost]
        public ActionResult DivisionsDetailBranchOfficesPartial(int? id_division)
        {
            Division division = db.Division.FirstOrDefault(d => d.id == id_division);
            var model = division?.BranchOffice?.ToList() ?? new List<BranchOffice>();

            return PartialView("_DivisionDetailBranchOfficesPartial", model.ToList());
        }

        #endregion

        #region REPORTS


        [HttpPost]
        public ActionResult DivisionReport()
        {
            DivisionReport report = new DivisionReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_DivisionReport", report);
        }

        [HttpPost, ValidateInput(false)] 
        public ActionResult DivisionDetailReport(int id)
        {
            DivisionDetailReport report = new DivisionDetailReport();
            report.Parameters["id_division"].Value = id;
            return PartialView("_DivisionReport", report);
        }


        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult CompanyData(int id_company)
        {
            var model = db.Company.FirstOrDefault(d => d.id == id_company);

            if (model == null)
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

        

        #endregion
    }
}



