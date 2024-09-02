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
    [Authorize]
    public class InventoryLineController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region INVENTORY LINE GRIDVIEW

        public ActionResult InventoryLinesPartial(int? keyToCopy){
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.InventoryLine.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.InventoryLine.Where(il => il.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryLinesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryLinesPartialAddNew(InventoryLine item)
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

                        db.InventoryLine.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Linea de Inventario: " + item.name + " guardada exitosamente");
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

            var model = db.InventoryLine.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryLinesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryLinesPartialUpdate(InventoryLine item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.InventoryLine.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.name = item.name;
                            modelItem.code = item.code;

                            modelItem.description = item.description;

                            modelItem.kardexControl = item.kardexControl;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.InventoryLine.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Linea de Inventario: " + item.name + " guardada exitosamente");
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

            var model = db.InventoryLine.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryLinesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryLinesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.InventoryLine.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.InventoryLine.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Linea de Inventario: " + (item?.name ?? "") + " desactivada exitosamente");
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

            var model = db.InventoryLine.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryLinesPartial", model.ToList());
        }


        [HttpPost]
        public ActionResult DeleteSelectedInventoryLines(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.InventoryLine.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.InventoryLine.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Lineas de Inventario desactivada exitosamente");
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

            var model = db.InventoryLine.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryLinesPartial", model.ToList());
        }

        #endregion

        #region MASTER DETAILS VIEW

        public ActionResult InventoryLineDetailItemTypesPartial(int? id_inventoryLine)
        {
            InventoryLine inventoryLine = db.InventoryLine.FirstOrDefault(il => il.id == id_inventoryLine);
            var model = inventoryLine?.ItemType?.ToList() ?? new List<ItemType>();

            return PartialView("_InventoryLineDetailItemTypesPartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ValidateCode(int id_inventoryLine, string code)
        {
            InventoryLine inventoryLine = db.InventoryLine.FirstOrDefault(b => b.id_company == this.ActiveCompanyId 
                                                                            && b.code == code
                                                                            && b.id != id_inventoryLine);

            if (inventoryLine == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra línea de inventario" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileInventoryLine()
        {
            if(Request.Files.Count > 0)
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
                     
                    if(workbook.Sheets.Count > 0)
                    {
                        Excel.Worksheet worksheet = workbook.ActiveSheet;
                        Excel.Range table = worksheet.UsedRange;

                        string code = string.Empty;
                        string name = string.Empty;
                        string description = string.Empty;
                        int sequential = 0;
                        bool kardexControl = false;

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
                                        sequential = int.Parse(row.Cells[3].Text);
                                        description = row.Cells[4].Text;
                                        kardexControl = row.Cells[5].Text.Equals("SI");
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    InventoryLine inventoryLine = db.InventoryLine.FirstOrDefault(l => l.code.Equals(code));

                                    if (inventoryLine == null)
                                    {
                                        inventoryLine = new InventoryLine
                                        {
                                            code = code,
                                            name = name,
                                            description = description,
                                            sequential = sequential,
                                            kardexControl = kardexControl,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.InventoryLine.Add(inventoryLine);
                                    }
                                    else
                                    {
                                        inventoryLine.code = code;
                                        inventoryLine.name = name;
                                        inventoryLine.description = description;
                                        inventoryLine.sequential = sequential;
                                        inventoryLine.kardexControl = kardexControl;

                                        inventoryLine.id_userUpdate = ActiveUser.id;
                                        inventoryLine.dateUpdate = DateTime.Now;

                                        db.InventoryLine.Attach(inventoryLine);
                                        db.Entry(inventoryLine).State = EntityState.Modified;
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

        #region REPORT

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryLineReport()
        {
            InventoryLineReport report = new InventoryLineReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_InventoryLineReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryLineDetailReport(int id)
        {
            InventoryLineDetailReport report = new InventoryLineDetailReport();
            report.Parameters["id_inventoryLine"].Value = id;
            return PartialView("_InventoryLineReport", report);
        }

        #endregion
    }
}

