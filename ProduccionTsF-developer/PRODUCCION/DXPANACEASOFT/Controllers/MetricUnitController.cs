using DevExpress.Web.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class MetricUnitController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region MetricUnit GridView

        [ValidateInput(false)]
        public ActionResult MetricUnitsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.MetricUnit.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.MetricUnit.Where(d => d.id_company == this.ActiveCompanyId);
            return PartialView("_MetricUnitsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricUnitsPartialAddNew(MetricUnit item)
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

                        db.MetricUnit.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Unidad de Medida: " + item.name + " guardada exitosamente");
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

            var model = db.MetricUnit.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_MetricUnitsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricUnitsPartialUpdate(MetricUnit item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.MetricUnit.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.id_metricType = item.id_metricType;
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.MetricUnit.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] =
                                SuccessMessage("Unidad de Medida: " + item.name + " guardada exitosamente");
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

                    var model = db.MetricUnit.Where(o => o.id_company == this.ActiveCompanyId);
                    return PartialView("_MetricUnitsPartial", model.ToList());

                }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricUnitsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.MetricUnit.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }
                        db.MetricUnit.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] =
                                     SuccessMessage("Unidad de Medida: " + (item?.name ?? "") + " desactivada exitosamente");
                                }
                                catch (Exception)
                            {
                                ViewData["EditMessage"] = ErrorMessage();
                                trans.Rollback();
                            }
                        }

                    }

            var model = db.MetricUnit.Where(o => o.id_company == this.ActiveCompanyId);
                return PartialView("_MetricUnitsPartial", model.ToList());
            }


        [HttpPost]
        public ActionResult DeleteSelectedMetricUnits(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.MetricUnit.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.MetricUnit.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Unidades de Medidas desactivadas exitosamente");
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

            var model = db.MetricUnit.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_MetricUnitsPartial", model.ToList());
        }

        #endregion

        #region REPORTS

        [HttpPost]
        public ActionResult CountryMetricUnit()
        {
            MetricUnitReport report = new MetricUnitReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_MetricUnitReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricUnitDetailReport(int id)
        {
            MetricUnitDetailReport report = new MetricUnitDetailReport();
            report.Parameters["id__metricUnit"].Value = id;
            return PartialView("_MetricUnitReport", report);
        }


        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ValidateCodeMetricUnit( int id_metricUnit, string code)
        {
            MetricUnit metricUnit = db.MetricUnit.FirstOrDefault(b => b.id_company == this.ActiveCompanyId 
                                                                    && b.code == code 
                                                                    && b.id != id_metricUnit);

            if (metricUnit == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra unidad de medida" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileMetricUnit()
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
                        int id_metricType = 0;
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
                                        id_metricType = int.Parse(row.Cells[3].Text);
                                        description = row.Cells[4].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    MetricUnit metricUnitImport = db.MetricUnit.FirstOrDefault(l => l.code.Equals(code));

                                    if (metricUnitImport == null)
                                    {
                                        metricUnitImport = new MetricUnit
                                        {
                                            code = code,
                                            name = name,
                                            id_metricType = id_metricType,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.MetricUnit.Add(metricUnitImport);
                                    }
                                    else
                                    {
                                        metricUnitImport.code = code;
                                        metricUnitImport.name = name;
                                        metricUnitImport.id_metricType = id_metricType;
                                        metricUnitImport.description = description;

                                        metricUnitImport.id_userUpdate = ActiveUser.id;
                                        metricUnitImport.dateUpdate = DateTime.Now;

                                        db.MetricUnit.Attach(metricUnitImport);
                                        db.Entry(metricUnitImport).State = EntityState.Modified;
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



