using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;


namespace DXPANACEASOFT.Controllers
{
    public class ProductionUnitController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult ProductionUnitPartial()
        {
            var model = db.ProductionUnit.Where(p => p.id_company == this.ActiveCompanyId);
            ViewBag.idCompany = this.ActiveCompanyId;
            return PartialView("_ProductionUnitPartial", model.ToList());
        }

        #region PRODUCTION UNIT GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitPartialAddNew(DXPANACEASOFT.Models.ProductionUnit item)
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

                        db.ProductionUnit.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Unidad de Producción: " + item.name + " guardada exitosamente");
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

            var model = db.ProductionUnit.Where(p => p.id_company == this.ActiveCompanyId);
            return PartialView("_ProductionUnitPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitPartialUpdate(DXPANACEASOFT.Models.ProductionUnit item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ProductionUnit.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.id_branchOffice = item.id_branchOffice;
                            modelItem.name = item.name;
                            modelItem.code = item.code;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ProductionUnit.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Unidad de Producción: " + item.name + " guardada exitosamente");
                        }
                        else
                        {
                            ViewData["EditMessage"] = ErrorMessage();
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

            var model = db.ProductionUnit.Where(p => p.id_company == this.ActiveCompanyId);
            return PartialView("_ProductionUnitPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ProductionUnit.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.ProductionUnit.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Unidad de Producción: " + item.name + " deshabilitada exitosamente");
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

            var model = db.ProductionUnit.Where(p => p.id_company == this.ActiveCompanyId);
            return PartialView("_ProductionUnitPartial", model.ToList());
        }

        public ActionResult DeleteSelectedProductionUnit(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var production_unit = db.ProductionUnit.Where(i => ids.Contains(i.id));
                        foreach (var productionunit in production_unit)
                        {
                            productionunit.isActive = false;

                            productionunit.id_userUpdate = ActiveUser.id;
                            productionunit.dateUpdate = DateTime.Now;

                            db.ProductionUnit.Attach(productionunit);
                            db.Entry(productionunit).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Unidades de Producción deshabilitadas exitosamente");
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

            var model = db.ProductionUnit.Where(p => p.id_company == this.ActiveCompanyId);
            return PartialView("_ProductionUnitPartial", model.ToList());
        }



            #endregion

            #region REPORTS

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitsReport()
        {
            ProductionUnitReport report = new ProductionUnitReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ProductionUnitsReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileProductionUnit()
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
                        string branchOfficeCode = string.Empty;

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
                                        branchOfficeCode = int.Parse(row.Cells[4].Text);
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    ProductionUnit productionUnit = db.ProductionUnit.FirstOrDefault(l => l.code.Equals(code));

                                    BranchOffice branchOffice = db.BranchOffice.FirstOrDefault(b => b.code.Equals(branchOfficeCode));

                                    if (productionUnit == null)
                                    {
                                        productionUnit = new ProductionUnit
                                        {
                                            code = code,
                                            name = name,
                                            description = description,

                                            id_branchOffice = branchOffice?.id ?? 0,

                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.ProductionUnit.Add(productionUnit);
                                    }
                                    else
                                    {
                                        productionUnit.code = code;
                                        productionUnit.name = name;
                                        productionUnit.description = description;

                                        productionUnit.id_branchOffice = branchOffice?.id ?? 0; 

                                        productionUnit.id_userUpdate = ActiveUser.id;
                                        productionUnit.dateUpdate = DateTime.Now;

                                        db.ProductionUnit.Attach(productionUnit);
                                        db.Entry(productionUnit).State = EntityState.Modified;
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

        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateCode(int id_productionUnit, int id_branchOffice, string code)
        {
            ProductionUnit productionUnit = db.ProductionUnit.FirstOrDefault(b => b.id != id_branchOffice && b.id_branchOffice == id_branchOffice && b.code == code && b.isActive);

            if (productionUnit == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra unidad de producción" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}