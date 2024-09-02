using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class RateController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Rate GRIDVIEW

        [ValidateInput(false)]
        public ActionResult RatesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.Rate.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.Rate.Where(r => r.id_company == this.ActiveCompanyId);
            return PartialView("_RatesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RatesPartialAddNew(Rate item)
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

                        db.Rate.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tarifa: " + item.name + " guardado exitosamente");
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

            var model = db.Rate.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_RatesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RatesPartialUpdate(Rate item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Rate.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.id_taxType = item.id_taxType;
                            modelItem.name = item.name;
                            modelItem.code = item.code;
                            modelItem.percentage = item.percentage;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.Rate.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Tarifa: " + item.name + " guardado exitosamente");
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

            var model = db.Rate.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_RatesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RatesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Rate.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.Rate.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tarifa: " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.Rate.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_RatesPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedRates(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Rate.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.Rate.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
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

                var model = db.Rate.Where(o => o.id_company == this.ActiveCompanyId);
                return PartialView("_RatesPartial", model.ToList());
            }
        #endregion

        #region REPORTS

        [HttpPost]
        public ActionResult RateReport()
        {
            RateReport report = new RateReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_RateReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RateDetailReport(int id)
        {
            RateDetailReport report = new RateDetailReport();
            report.Parameters["id_rate"].Value = id;
            return PartialView("_RateReport", report);
        }


        #endregion

        #region AUXILIAR FUNCTIONS


        [HttpPost]
        public JsonResult ValidateCodeRate(int id_rate, string code)
        {
            Rate rate = db.Rate.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_rate);

            if (rate == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Tarifa" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileRate()
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
                        int id_taxType = 0;
                        int percentage = 0;
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
                                        id_taxType = int.Parse(row.Cells[3].Text);
                                        percentage = int.Parse(row.Cells[4].Text);
                                        description = row.Cells[5].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    Rate rateImport = db.Rate.FirstOrDefault(l => l.code.Equals(code));

                                    if (rateImport == null)
                                    {
                                        rateImport = new Rate
                                        {
                                            code = code,
                                            name = name,
                                            id_taxType = id_taxType,
                                            percentage = percentage,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.Rate.Add(rateImport);
                                    }
                                    else
                                    {
                                        rateImport.code = code;
                                        rateImport.name = name;
                                        rateImport.id_taxType = id_taxType;
                                        rateImport.percentage = percentage;
                                        rateImport.description = description;

                                        rateImport.id_userUpdate = ActiveUser.id;
                                        rateImport.dateUpdate = DateTime.Now;

                                        db.Rate.Attach(rateImport);
                                        db.Entry(rateImport).State = EntityState.Modified;
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

        #endregion

    }
}

