using DevExpress.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class MetricTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region MetricType GridView

        [ValidateInput(false)]
        public ActionResult MetricTypesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.MetricType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.MetricType.Where(d => d.id_company == this.ActiveCompanyId);
            return PartialView("_MetricTypesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricTypesPartialAddNew(MetricType item)
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

                        db.MetricType.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tipo de Métrica: " + item.name + " guardada exitosamente");
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

            var model = db.MetricType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_MetricTypesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricTypesPartialUpdate(MetricType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.MetricType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.id_dataType = item.id_dataType;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;
                        
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.MetricType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] =  SuccessMessage("Tipo de Métrica: " + item.name + " guardada exitosamente");
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

            var model = db.MetricType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_MetricTypesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricTypesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.MetricType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }
                        db.MetricType.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tipo de Métrica: " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.MetricType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_MetricTypesPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedMetricTypes(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.MetricType.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.MetricType.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tipos de Métrica desactivados exitosamente");
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

            var model = db.MetricType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_MetricTypesPartial", model.ToList());
        }

        #endregion

        #region MASTER DETAILS VIEW

        public ActionResult MetricTypesDetailMetricUnitsPartial(int? id_metricTypes)
        {
            MetricType metricType = db.MetricType.FirstOrDefault(o => o.id == id_metricTypes);
            var model = metricType?.MetricUnit?.ToList() ?? new List<MetricUnit>();

            return PartialView("_MetricTypeDetailMetricUnitsPartial", model.ToList());
        }

        #endregion

        #region REPORTS


        [HttpPost]
        public ActionResult MetricTypeReport()
        {
            MetricTypeReport report = new MetricTypeReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_MetricTypeReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricTypeDetailReport(int id)
        {
            MetricTypeDetailReport report = new MetricTypeDetailReport();
            report.Parameters["id_metricType"].Value = id;
            return PartialView("_MetricTypeReport", report);
        }


        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ValidateCodeMetricType(int id_metricType, string code)
        {
            MetricType metricType = db.MetricType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_metricType);

            if (metricType == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tipo de Métrica" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileMetricType()
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
                        int id_dataType = 0;
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
                                        id_dataType = int.Parse(row.Cells[3].Text);
                                        description = row.Cells[4].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    MetricType metricTypeImport = db.MetricType.FirstOrDefault(l => l.code.Equals(code));

                                    if (metricTypeImport == null)
                                    {
                                        metricTypeImport = new MetricType
                                        {
                                            code = code,
                                            name = name,
                                            id_dataType = id_dataType,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.MetricType.Add(metricTypeImport);
                                    }
                                    else
                                    {
                                        metricTypeImport.code = code;
                                        metricTypeImport.name = name;
                                        metricTypeImport.id_dataType = id_dataType;
                                        metricTypeImport.description = description;

                                        metricTypeImport.id_userUpdate = ActiveUser.id;
                                        metricTypeImport.dateUpdate = DateTime.Now;

                                        db.MetricType.Attach(metricTypeImport);
                                        db.Entry(metricTypeImport).State = EntityState.Modified;
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