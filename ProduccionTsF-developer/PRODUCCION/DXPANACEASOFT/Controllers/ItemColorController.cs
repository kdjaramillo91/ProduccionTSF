using DevExpress.Web.Mvc;
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
    public class ItemColorController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ItemColor GRIDVIEW

        [ValidateInput(false)]
        public ActionResult ItemColorsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ItemColor.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.ItemColor.Where(it => it.id_company == this.ActiveCompanyId);
            return PartialView("_ItemColorsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemColorsPartialAddNew(ItemColor item)
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

                        db.ItemColor.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Color: " + item.name + " guardado exitosamente");
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

            var model = db.ItemColor.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemColorsPartial", model.ToList());

        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ItemColorsPartialUpdate(ItemColor item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemColor.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.color = item.color;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.ItemColor.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Color: " + item.name + " guardado exitosamente");
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

            var model = db.ItemColor.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemColorsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemColorsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ItemColor.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemColor.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Color: " + (item?.name ?? "") + " desactivada exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.ItemColor.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemColorsPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedItemColors(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemColor.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemColor.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Colores desactivados exitosamente");
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

            var model = db.ItemColor.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemColorsPartial", model.ToList());
        }


        #endregion

        #region REPORTS    

        [HttpPost]
        public ActionResult ItemColorReport()
        {
            ItemColorReport report = new ItemColorReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ItemColorReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemColorDetailReport(int id)
        {
            ItemColorDetailReport report = new ItemColorDetailReport();
            report.Parameters["id_itemColor"].Value = id;
            return PartialView("_ItemColorReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ValidateColor(int id_color, string color)
        {
            ItemColor itemColor = db.ItemColor.FirstOrDefault(b => b.id_company == this.ActiveCompanyId &&
                                                                        b.color == color &&
                                                                        b.id != id_color);

            if (itemColor == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Color en uso" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateCodeItemColor(int id_itemColor, string code)
        {
            ItemColor itemColorCode = db.ItemColor.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_itemColor);
            if (itemColorCode == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Color" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileItemColor(){
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
                        string color = string.Empty;
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
                                        color = row.Cells[3].Text;
                                        description = row.Cells[4].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    ItemColor itemColorImport = db.ItemColor.FirstOrDefault(l => l.code.Equals(code));

                                    if (itemColorImport == null)
                                    {
                                        itemColorImport = new ItemColor
                                        {
                                            code = code,
                                            name = name,
                                            color =color,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.ItemColor.Add(itemColorImport);
                                    }
                                    else
                                    {itemColorImport.code = code;
                                        itemColorImport.name = name;
                                        itemColorImport.color = color;
                                        itemColorImport.description = description;

                                        itemColorImport.id_userUpdate = ActiveUser.id;
                                        itemColorImport.dateUpdate = DateTime.Now;

                                        db.ItemColor.Attach(itemColorImport);
                                        db.Entry(itemColorImport).State = EntityState.Modified;
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



