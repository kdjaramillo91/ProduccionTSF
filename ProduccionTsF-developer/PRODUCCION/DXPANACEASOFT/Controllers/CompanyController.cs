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
    public class CompanyController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Company GridView
        //[ValidateInput(false)]
        public ActionResult CompaniesPartial()
        {
            var model = db.Company;
            return PartialView("_CompaniesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CompaniesPartialAddNew(Company item, CompanyElectronicFacturation electronicFacturation)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.CompanyElectronicFacturation = electronicFacturation;

                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.Company.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Compañía " + item.trademark + " guardada exitosamente");
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

            var model = db.Company;
            return PartialView("_CompaniesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CompaniesPartialUpdate(Company item, CompanyElectronicFacturation electronicFacturation)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Company.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.id_businessGroup = item.id_businessGroup;
                            modelItem.ruc = item.ruc;
                            modelItem.businessName = item.businessName;
                            modelItem.trademark = item.trademark;
                            modelItem.description = item.description;
                            modelItem.logo = item.logo;
                            modelItem.address = item.address;
                            modelItem.email = item.email;
                            modelItem.phoneNumber = item.phoneNumber;
                            modelItem.isActive = item.isActive;
							modelItem.plantCode = item.plantCode;
							modelItem.registryFDA = item.registryFDA;
							modelItem.websiteCompany = item.websiteCompany;
                            modelItem.requiredLogistic = item.requiredLogistic;

                            // UPDATE ELECTRONIC FACTURATION

                            if (modelItem.CompanyElectronicFacturation != null)
                            {
                                modelItem.CompanyElectronicFacturation.id_enviromentType = electronicFacturation.id_enviromentType;
                                modelItem.CompanyElectronicFacturation.id_emissionType = electronicFacturation.id_emissionType;

                                modelItem.CompanyElectronicFacturation.resolutionNumber = electronicFacturation.resolutionNumber;
                                modelItem.CompanyElectronicFacturation.rise = electronicFacturation.rise;
                                modelItem.CompanyElectronicFacturation.requireAccounting = electronicFacturation.requireAccounting;

                                if(!string.IsNullOrEmpty(electronicFacturation.password) &&
                                   !electronicFacturation.password.Equals(modelItem.CompanyElectronicFacturation.password))
                                {
                                    modelItem.CompanyElectronicFacturation.password = electronicFacturation.password;
                                }

                                modelItem.CompanyElectronicFacturation.certificate = electronicFacturation.certificate;
                            }
                            else
                            {
                                modelItem.CompanyElectronicFacturation = electronicFacturation;
                            }

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.Company.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Compañía " + item.trademark + " guardada exitosamente");
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

            var model = db.Company;
            return PartialView("_CompaniesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CompaniesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Company.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                            item.isActive = false;

                        }

                        db.Company.Attach(item);
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
            var model = db.Company;
            return PartialView("_CompaniesPartial", model.ToList());
        }

        [HttpPost]
        public void DeleteSelectedCompanies(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Company.Where(i => ids.Contains(i.id));
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

        #endregion

        #region MASTER DETAILS VIEW

        public ActionResult CompaniesDetailDivisionsPartial(int? id_company)
        {
            Company company = db.Company.FirstOrDefault(d => d.id == id_company);
            var model = company?.Division?.ToList() ?? new List<Division>();

            return PartialView("_CompanyDetailDivisionsPartial", model.ToList());
        }

        #endregion

        #region REPORTS

        public ActionResult CompanyReport(int[] ids)
        {
            // PurchaseRequestReport requestReport = new PurchaseRequestReport();
            CompanyReport Company = new CompanyReport();
            CompanyDetailReport CompanyDetail = new CompanyDetailReport();


            //PurchaseRequestsListReport requestsListReport = new PurchaseRequestsListReport();
            var partial = "";

            if (ids == null)
            {
                partial = "_CompanyReport";
                return PartialView(partial, Company);
            }
            if (ids.Length > 0)
            {
                if (ids.Length == 1)
                {
                    // Department.Parameters["numberDepartment"].Value = ids[0];
                    //Department.Parameters["numberDepartment"].Visible = false;
                    Company.Parameters["CompanyRuc"].Value = ids[0];
                    Company.Parameters["CompanyRuc"].Visible = false;
                    partial = "_CompanyReport";
                    return PartialView(partial, Company);
                }

            }
            return PartialView(partial, Company);
        }

        #endregion

        #region Auxiliar Functions

        [HttpPost, ValidateInput(false)]
        public JsonResult UploadCertificate()
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                List<string> errorMessages = new List<string>();

                if (file != null)
                {
                    string filename = Server.MapPath("~/App_Data/Temp/" + file.FileName);

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                    file.SaveAs(filename);

                    TempData["certificate"] = file.FileName;
                    TempData.Keep("certificate");

                    return Json(file?.FileName, JsonRequestBehavior.AllowGet);
                }
            }

            TempData["certificate"] = null;
            TempData.Keep("certificate");

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BinaryImageColumnPhotoUpdate()
        {
            return BinaryImageEditExtension.GetCallbackResult();
        }

        #endregion
    }
}



