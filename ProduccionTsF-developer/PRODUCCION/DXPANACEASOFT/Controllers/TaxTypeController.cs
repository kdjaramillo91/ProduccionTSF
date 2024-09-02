using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using Excel = Microsoft.Office.Interop.Excel;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class TaxTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region TaxType GRIDVIEW

        [ValidateInput(false)]
        public ActionResult TaxesTypesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.TaxType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.TaxType.Where(tt => tt.id_company == this.ActiveCompanyId);
            return PartialView("_TaxesTypesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaxesTypesPartialAddNew(TaxType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_company = this.ActiveCompanyId;item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.TaxType.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tipo de Impuesto: " + item.name + " guardado exitosamente");
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

            var model = db.TaxType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_TaxesTypesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaxesTypesPartialUpdate(TaxType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.TaxType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.name = item.name;
                            modelItem.code = item.code;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.TaxType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Tipo de Impuesto: " + item.name + " guardado exitosamente");
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

                    var model = db.TaxType.Where(o => o.id_company == this.ActiveCompanyId);
                    return PartialView("_TaxesTypesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaxesTypesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.TaxType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.TaxType.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipo de Impuesto: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.TaxType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_TaxesTypesPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedTaxesTypes(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.TaxType.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.TaxType.Attach(item);
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

                    var model = db.TaxType.Where(o => o.id_company == this.ActiveCompanyId);
                    return PartialView("_TaxesTypesPartial", model.ToList());
            }

        #endregion

        #region MASTER DETAILS VIEW

        public ActionResult TaxTypeDetailRatesPartial(int? id_taxType)
        {
            TaxType taxType = db.TaxType.FirstOrDefault(tt => tt.id == id_taxType);
            var model = taxType?.Rate?.ToList() ?? new List<Rate>();

            return PartialView("_TaxTypeDetailRatesPartial", model.ToList());
        }

        #endregion

        #region REPORTS

        [HttpPost]public ActionResult TaxTypeReport()
        {
            TaxTypeReport report = new TaxTypeReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_TaxTypeReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaxTypeDetailReport(int id)
        {
            TaxTypeDetailReport report = new TaxTypeDetailReport();
            report.Parameters["id_taxType"].Value = id;
            return PartialView("_TaxTypeReport", report);
        }


        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ValidateCodeTaxType(int id_taxType, string code)
        {
            TaxType taxType = db.TaxType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_taxType);

            if (taxType == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tipo de Impuesto" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileTaxType()
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

                                    TaxType taxTypeImport = db.TaxType.FirstOrDefault(l => l.code.Equals(code));

                                    if (taxTypeImport == null)
                                    {
                                        taxTypeImport = new TaxType
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

                                        db.TaxType.Add(taxTypeImport);
                                    }
                                    else
                                    {
                                        taxTypeImport.code = code;
                                        taxTypeImport.name = name;
                                        taxTypeImport.description = description;

                                        taxTypeImport.id_userUpdate = ActiveUser.id;
                                        taxTypeImport.dateUpdate = DateTime.Now;

                                        db.TaxType.Attach(taxTypeImport);
                                        db.Entry(taxTypeImport).State = EntityState.Modified;
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



