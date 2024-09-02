using DevExpress.Web.Mvc;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;using System.Data.Entity;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryControlTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult InventoryControlTypePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.InventoryControlType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.InventoryControlType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryControlTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryControlTypePartialAddNew(DXPANACEASOFT.Models.InventoryControlType item)
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
                        db.InventoryControlType.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Control de Inventario: " + item.name + " guardado exitosamente");
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

            var model = db.InventoryControlType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryControlTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryControlTypePartialUpdate(DXPANACEASOFT.Models.InventoryControlType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.InventoryControlType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.InventoryControlType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Control de Inventario: " + item.name + " guardado exitosamente");
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

            var model = db.InventoryControlType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryControlTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryControlTypePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.InventoryControlType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.InventoryControlType.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Control de Inventario: " + (item?.name ?? "") + " desactivadao exitosamente");
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

            var model = db.InventoryControlType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryControlTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedInventoryControlType(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var inventory_control_type = db.InventoryControlType.Where(i => ids.Contains(i.id));
                        foreach (var inventorycontroltype in inventory_control_type)
                        {
                            inventorycontroltype.isActive = false;

                            inventorycontroltype.id_userUpdate = ActiveUser.id;
                            inventorycontroltype.dateUpdate = DateTime.Now;

                            db.InventoryControlType.Attach(inventorycontroltype);
                            db.Entry(inventorycontroltype).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Controles de Inventarios desactivados exitosamente");
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

            var model = db.InventoryControlType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_InventoryControlTypePartial", model.ToList());
        }

        #region REPORT

        [HttpPost]
        public ActionResult InventoryControlTypeReport()
        {
            InventoryControlTypeReport report = new InventoryControlTypeReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_InventoryControlTypeReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryControlTypeDetailReport(int id)
        {
            InventoryControlTypeDetailReport report = new InventoryControlTypeDetailReport();
            report.Parameters["id_InventoryControlType"].Value = id;
            return PartialView("_InventoryControlTypeReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ValidateCodeInventoryControlType(int id_inventoryControlType, string code)
        {
            InventoryControlType inventorylControlType = db.InventoryControlType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_inventoryControlType);

            if (inventorylControlType == null)
            {return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Control de Inventario" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileInventoryControlType()
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

                                    InventoryControlType inventoryControlType = db.InventoryControlType.FirstOrDefault(l => l.code.Equals(code));

                                    if (inventoryControlType == null)
                                    {
                                        inventoryControlType = new InventoryControlType
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

                                        db.InventoryControlType.Add(inventoryControlType);
                                    }
                                    else
                                    {
                                        inventoryControlType.code = code;
                                        inventoryControlType.name = name;
                                        inventoryControlType.description = description;

                                        inventoryControlType.id_userUpdate = ActiveUser.id;
                                        inventoryControlType.dateUpdate = DateTime.Now;

                                        db.InventoryControlType.Attach(inventoryControlType);
                                        db.Entry(inventoryControlType).State = EntityState.Modified;
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