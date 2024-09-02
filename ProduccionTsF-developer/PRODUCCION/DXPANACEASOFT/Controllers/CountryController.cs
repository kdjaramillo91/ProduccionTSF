using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

namespace DXPANACEASOFT.Controllers{
    public class CountryController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region COUNTRY GRIDVIEW

        [ValidateInput(false)]
        public ActionResult CountryPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.Country.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.Country.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CountryPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CountryPartialAddNew(DXPANACEASOFT.Models.Country item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;
                        item.id_origin = 2;
                        db.Country.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("País: " + item.name + " guardado exitosamente");
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

            var model = db.Country.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CountryPartial", model.ToList());

        }

        public ActionResult CountryPartialUpdate(DXPANACEASOFT.Models.Country item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Country.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                        
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.Leyenda = item.Leyenda;

                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.Country.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("País: " + item.name + " guardado exitosamente");
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

            var model = db.Country.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CountryPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CountryPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Country.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.Country.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("País: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.Country.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CountryPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedCountry(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var car = db.Country.Where(i => ids.Contains(i.id));
                        foreach (var country in car)
                        {
                            country.isActive = false;

                            country.id_userUpdate = ActiveUser.id;
                            country.dateUpdate = DateTime.Now;

                            db.Country.Attach(country);
                            db.Entry(country).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Paises desactivados exitosamente");
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

            var model = db.Country.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CountryPartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileCountry()
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

                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(filename);

                    if (workbook.Sheets.Count > 0)
                    {
                        Excel.Worksheet worksheet = workbook.ActiveSheet;
                        Excel.Range table = worksheet.UsedRange;

                        string code = string.Empty;
                        string name = string.Empty;
                        string description = string.Empty;

                        using (DbContextTransaction trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 2; i < table.Rows.Count; i++)
                                {
                                    Excel.Range row = table.Rows[i]; // FILA i
                                    try
                                    {
                                        code = row.Cells[1].Text;        // COLUMNA 1
                                        name = row.Cells[2].Text;
                                        description = row.Cells[3].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    Country countryImport = db.Country.FirstOrDefault(l => l.code.Equals(code));

                                    if (countryImport == null)
                                    {
                                        countryImport = new Country
                                        {
                                            code = code,
                                            name = name,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.Country.Add(countryImport);
                                    }
                                    else
                                    {
                                        countryImport.code = code;
                                        countryImport.name = name;
                                        countryImport.description = description;

                                        countryImport.id_userUpdate = ActiveUser.id;
                                        countryImport.dateUpdate = DateTime.Now;

                                        db.Country.Attach(countryImport);
                                        db.Entry(countryImport).State = EntityState.Modified;
                                    }
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

                    application.Workbooks.Close();

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }

                    return Json(file?.FileName, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateCodeCountry(int id_country, string code)
        {
            Country country = db.Country.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_country);

            if (country == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra País" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult CountryReport()
        {
            CountryReport report = new CountryReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_CountryReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CountryDetailReport(int id)
        {
            CountryDetailReport report = new CountryDetailReport();
            report.Parameters["id_country"].Value = id;
            return PartialView("_CountryReport", report);
        }

        #endregion
    }
}

