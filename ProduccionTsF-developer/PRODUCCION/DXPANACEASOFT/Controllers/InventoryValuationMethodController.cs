using DevExpress.Web.Mvc;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryValuationMethodController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult InventoryValuationMethodPartial(int? keyToCopy)
        {

            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.InventoryValuationMethod.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.InventoryValuationMethod.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryValuationMethodPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryValuationMethodPartialAddNew(DXPANACEASOFT.Models.InventoryValuationMethod item)
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
                        db.InventoryValuationMethod.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Método de valoración de stocks: " + item.name + " guardada exitosamente");
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

            var model = db.InventoryValuationMethod.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryValuationMethodPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryValuationMethodPartialUpdate(DXPANACEASOFT.Models.InventoryValuationMethod item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.InventoryValuationMethod.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.InventoryValuationMethod.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Método de valoración de stocks: " + item.name + " guardada exitosamente");
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

            var model = db.InventoryValuationMethod.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryValuationMethodPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryValuationMethodPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.InventoryValuationMethod.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.InventoryValuationMethod.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Método de valoración de stocks: " + (item?.name ?? "") + " desactivada exitosamente");
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

            var model = db.InventoryValuationMethod.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryValuationMethodPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedInventoryValuationMethod(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var inventory_valuation_method = db.InventoryValuationMethod.Where(i => ids.Contains(i.id));
                        foreach (var inventoryvaluationmethod in inventory_valuation_method)
                        {
                            inventoryvaluationmethod.isActive = false;

                            inventoryvaluationmethod.id_userUpdate = ActiveUser.id;
                            inventoryvaluationmethod.dateUpdate = DateTime.Now;

                            db.InventoryValuationMethod.Attach(inventoryvaluationmethod);
                            db.Entry(inventoryvaluationmethod).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Métodos de valoración de stocks desactivados exitosamente");
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

            var model = db.InventoryValuationMethod.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryValuationMethodPartial", model.ToList());
        }

        #region REPORT

        [HttpPost]
        public ActionResult InventoryValuationMethodReport()
        {
            InventoryValuationMethodReport report = new InventoryValuationMethodReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_InventoryValuationMethodReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryValuationMethodDetailReport(int id)
        {
            InventoryValuationMethodDetailReport report = new InventoryValuationMethodDetailReport();
            report.Parameters["id_inventoryValuationMethod"].Value = id;
            return PartialView("_InventoryValuationMethodReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileInventoryValuationMethod()
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

                                    InventoryValuationMethod inventoryValuationMethodImport = db.InventoryValuationMethod.FirstOrDefault(l => l.code.Equals(code));

                                    if (inventoryValuationMethodImport == null)
                                    {
                                        inventoryValuationMethodImport = new InventoryValuationMethod
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

                                        db.InventoryValuationMethod.Add(inventoryValuationMethodImport);
                                    }
                                    else
                                    {
                                        inventoryValuationMethodImport.code = code;
                                        inventoryValuationMethodImport.name = name;
                                        inventoryValuationMethodImport.description = description;

                                        inventoryValuationMethodImport.id_userUpdate = ActiveUser.id;
                                        inventoryValuationMethodImport.dateUpdate = DateTime.Now;

                                        db.InventoryValuationMethod.Attach(inventoryValuationMethodImport);
                                        db.Entry(inventoryValuationMethodImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeInventoryValuationMethod(int id_inventoryValuationMethod, string code)
        {
            InventoryValuationMethod inventoryValuationMethod = db.InventoryValuationMethod.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_inventoryValuationMethod);

            if (inventoryValuationMethod == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Método de valoración de stocks" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}