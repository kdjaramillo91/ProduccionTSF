using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class PoundsRangeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region PoundsRange GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult PoundsRangePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.PoundsRange.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.PoundsRange.Where(whl => whl.id_company == this.ActiveCompanyId);
            return PartialView("_PoundsRangePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PoundsRangePartialAddNew(PoundsRange item)
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

                        db.PoundsRange.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tarifario: " + item.name + " guardada exitosamente");
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

            var model = db.PoundsRange.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PoundsRangePartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult PoundsRangePartialUpdate(PoundsRange item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.PoundsRange.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;

                            modelItem.id_metricUnit = item.id_metricUnit;
                            modelItem.range_end = item.range_end;
                            modelItem.range_ini= item.range_ini;
                            modelItem.id_suggestedIceBagRange = item.id_suggestedIceBagRange;

                            modelItem.isActive = item.isActive;
                            modelItem.id_company = ActiveCompany.id;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.PoundsRange.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Rango de Libras: " + item.name + " guardada exitosamente");
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

            var model = db.PoundsRange.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PoundsRangePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PoundsRangePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.PoundsRange.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.PoundsRange.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();


                            ViewData["EditMessage"] = SuccessMessage("Rango de Libras: " + (item?.name ?? "") + " desactivada exitosamente");
                        }
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

            var model = db.PoundsRange.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PoundsRangePartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedPoundsRange(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.PoundsRange.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.PoundsRange.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Rango de libras desactivadas exitosamente");
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

            var model = db.PoundsRange.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PoundsRangePartial", model.ToList());
        }
        #endregion

        #region REPORTS

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFilePoundsRange()
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
                        int id_metricUnit = 0;
                        int range_ini = 0;
                        int range_end = 0;
                        int? id_suggestedIceBagRange = null;


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
                                        id_metricUnit = int.Parse(row.Cells[3].Text);
                                        range_ini = int.Parse(row.Cells[4].Text);
                                        range_end = int.Parse(row.Cells[5].Text);
                                        id_suggestedIceBagRange = (row.Cells[6].Text==null)?null: int.Parse(row.Cells[6].Text);

                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    PoundsRange poundsRange = db.PoundsRange.FirstOrDefault(l => l.code.Equals(code));

                                    if (poundsRange == null)
                                    {
                                        poundsRange = new PoundsRange
                                        {
                                            code = code,
                                            name = name,
                                            
                                            id_metricUnit = id_metricUnit,
                                            id_suggestedIceBagRange = id_suggestedIceBagRange,
                                            range_ini = range_ini,
                                            range_end = range_end,

                                            isActive = true,
                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.PoundsRange.Add(poundsRange);
                                    }
                                    else
                                    {
                                        poundsRange.code = code;
                                        poundsRange.name = name;
                                        poundsRange.id_metricUnit= id_metricUnit;
                                        poundsRange.id_suggestedIceBagRange = id_suggestedIceBagRange;
                                        poundsRange.range_ini = range_ini;
                                        poundsRange.range_end = range_end;

                                        poundsRange.id_userUpdate = ActiveUser.id;
                                        poundsRange.dateUpdate = DateTime.Now;

                                        db.PoundsRange.Attach(poundsRange);
                                        db.Entry(poundsRange).State = EntityState.Modified;
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
        public JsonResult ValidateCodePoundsRange(int id_PoundsRange, string code)
        {
            PoundsRange poundsRange = db.PoundsRange.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_PoundsRange);

            if (poundsRange == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Rango de Libras" }, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}



