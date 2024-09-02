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
    public class PurchaseReasonController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region COUNTRY GRIDVIEW

        [ValidateInput(false)]
        public ActionResult PurchaseReasonPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.PurchaseReason.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.PurchaseReason.Where(p => p.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseReasonPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseReasonPartialAddNew(DXPANACEASOFT.Models.PurchaseReason item)
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

                        db.PurchaseReason.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Motivo Orden de Compra: " + item.name + " guardado exitosamente");
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

            var model = db.PurchaseReason.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseReasonPartial", model.ToList());

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseReasonPartialUpdate(DXPANACEASOFT.Models.PurchaseReason item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.PurchaseReason.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.PurchaseReason.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Motivo Orden de Compra: " + item.name + " guardado exitosamente");
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

            var model = db.PurchaseReason.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseReasonPartial", model.ToList());

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseReasonPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.PurchaseReason.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.PurchaseReason.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Motivo Orden de Compra: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.PurchaseReason.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseReasonPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedPurchaseReason(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var purchase_reason = db.PurchaseReason.Where(i => ids.Contains(i.id));
                        foreach (var purchasereason in purchase_reason)
                        {
                            purchasereason.isActive = false;

                            purchasereason.id_userUpdate = ActiveUser.id;
                            purchasereason.dateUpdate = DateTime.Now;

                            db.PurchaseReason.Attach(purchasereason);
                            db.Entry(purchasereason).State = EntityState.Modified;
                        }
                        db.SaveChanges();

                        ViewData["EditMessage"] = SuccessMessage("Motivos Orden de Compra desactivados exitosamente");
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

            var model = db.PurchaseReason.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseReasonPartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFilePurchaseReason()
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

                                    PurchaseReason purchaseReasonImport = db.PurchaseReason.FirstOrDefault(l => l.code.Equals(code));

                                    if (purchaseReasonImport == null)
                                    {
                                        purchaseReasonImport = new PurchaseReason
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

                                        db.PurchaseReason.Add(purchaseReasonImport);
                                    }
                                    else
                                    {
                                        purchaseReasonImport.code = code;
                                        purchaseReasonImport.name = name;
                                        purchaseReasonImport.description = description;

                                        purchaseReasonImport.id_userUpdate = ActiveUser.id;
                                        purchaseReasonImport.dateUpdate = DateTime.Now;

                                        db.PurchaseReason.Attach(purchaseReasonImport);
                                        db.Entry(purchaseReasonImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodePurchaseReason(int id_purchaseReason, string code)
        {
            PurchaseReason purchaseReason = db.PurchaseReason.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_purchaseReason);

            if (purchaseReason == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Motivo Orden de Compra" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult PurchaseReasonReport()
        {
            PurchaseReasonReport report = new PurchaseReasonReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_PurchaseReasonReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseReasonDetailReport(int id)
        {
            PurchaseReasonDetailReport report = new PurchaseReasonDetailReport();
            report.Parameters["id_purchaseReason"].Value = id;
            return PartialView("_PurchaseReasonReport", report);
        }

        #endregion
    }
}

    
