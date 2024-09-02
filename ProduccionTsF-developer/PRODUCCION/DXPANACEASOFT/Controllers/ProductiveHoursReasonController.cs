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
 
    public class ProductiveHoursReasonController : DefaultController
    {
        
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ProductiveHoursReason GRIDVIEW

        [ValidateInput(false)]
        public ActionResult ProductiveHoursReasonPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.productiveHoursReason.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.productiveHoursReason.ToList();
            return PartialView("_ProductiveHoursReasonPartial", model.ToList());
        }
        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductiveHoursReasonPartialAddNew(DXPANACEASOFT.Models.productiveHoursReason item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;
                        db.productiveHoursReason.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Motivo de parada de Máquina: " + item.name + " guardado exitosamente");
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

            var model = db.productiveHoursReason.Where(o => o.isActive);
            return PartialView("_ProductiveHoursReasonPartial", model.ToList());

        }

        public ActionResult ProductiveHoursReasonPartialUpdate(DXPANACEASOFT.Models.productiveHoursReason item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.productiveHoursReason.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;

                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.productiveHoursReason.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Motivo de parada de Máquina: " + item.name + " guardado exitosamente");
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

            var model = db.productiveHoursReason.Where(o => o.isActive);
            return PartialView("_ProductiveHoursReasonPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductiveHoursReasonPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.productiveHoursReason.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.productiveHoursReason.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Motivo de parada de Máquina: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.productiveHoursReason.Where(o => o.isActive);
            return PartialView("_ProductiveHoursReasonPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedProductiveHoursReason(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var car = db.productiveHoursReason.Where(i => ids.Contains(i.id));
                        foreach (var country in car)
                        {
                            country.isActive = false;

                            country.id_userUpdate = ActiveUser.id;
                            country.dateUpdate = DateTime.Now;

                            db.productiveHoursReason.Attach(country);
                            db.Entry(country).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Motivos de Parada de Máquina desactivados exitosamente");
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

            var model = db.productiveHoursReason.Where(o => o.isActive);
            return PartialView("_ProductiveHoursReasonPartial", model.ToList());
        }

        [HttpPost]
        public JsonResult ImportFileProductiveHoursReason()
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

                                    productiveHoursReason countryImport = db.productiveHoursReason.FirstOrDefault(l => l.code.Equals(code));

                                    if (countryImport == null)
                                    {
                                        countryImport = new productiveHoursReason
                                        {
                                            code = code,
                                            name = name,
                                            description = description,
                                            isActive = true,

                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.productiveHoursReason.Add(countryImport);
                                    }
                                    else
                                    {
                                        countryImport.code = code;
                                        countryImport.name = name;
                                        countryImport.description = description;

                                        countryImport.id_userUpdate = ActiveUser.id;
                                        countryImport.dateUpdate = DateTime.Now;

                                        db.productiveHoursReason.Attach(countryImport);
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
        public JsonResult ValidateCodeProductiveHoursReason(int id_productiveHoursReason, string code)
        {
            productiveHoursReason country = db.productiveHoursReason.FirstOrDefault(b => b.code == code
                                                                            && b.id != id_productiveHoursReason);

            if (country == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Motivo de Parada de Máquina" }, JsonRequestBehavior.AllowGet);
        }
    }
}