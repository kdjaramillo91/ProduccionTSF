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
    public class PurchaseOrderShippingTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region COUNTRY PurchaseOrderShippingType

        [ValidateInput(false)]
        public ActionResult PurchaseOrderShippingTypePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.PurchaseOrderShippingType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.PurchaseOrderShippingType.Where(p => p.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseOrderShippingTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderShippingTypePartialAddNew(DXPANACEASOFT.Models.PurchaseOrderShippingType item)
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
                        db.PurchaseOrderShippingType.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Medio de Envio: " + item.name + " guardado exitosamente");
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

            var model = db.PurchaseOrderShippingType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseOrderShippingTypePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderShippingTypePartialUpdate(DXPANACEASOFT.Models.PurchaseOrderShippingType item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.PurchaseOrderShippingType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.PurchaseOrderShippingType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Medio de Envio: " + item.name + " guardado exitosamente");
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

            var model = db.PurchaseOrderShippingType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseOrderShippingTypePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderShippingTypePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.PurchaseOrderShippingType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.PurchaseOrderShippingType.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Medio de Envio: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.PurchaseOrderShippingType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseOrderShippingTypePartial", model.ToList());
        }

        public ActionResult DeleteSelectedPurchaseOrderShippingType(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var purchase_order_shipping_type = db.PurchaseOrderShippingType.Where(i => ids.Contains(i.id));
                        foreach (var purchaseordershippingtype in purchase_order_shipping_type)
                        {
                            purchaseordershippingtype.isActive = false;

                            purchaseordershippingtype.id_userUpdate = ActiveUser.id;
                            purchaseordershippingtype.dateUpdate = DateTime.Now;

                            db.PurchaseOrderShippingType.Attach(purchaseordershippingtype);
                            db.Entry(purchaseordershippingtype).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Medios de Envios desactivados exitosamente");
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

            var model = db.PurchaseOrderShippingType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PurchaseOrderShippingTypePartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFilePurchaseOrderShippingType()
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

                                    PurchaseOrderShippingType purchaseOrderShippingTypeImport = db.PurchaseOrderShippingType.FirstOrDefault(l => l.code.Equals(code));

                                    if (purchaseOrderShippingTypeImport == null)
                                    {
                                        purchaseOrderShippingTypeImport = new PurchaseOrderShippingType
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

                                        db.PurchaseOrderShippingType.Add(purchaseOrderShippingTypeImport);
                                    }
                                    else
                                    {
                                        purchaseOrderShippingTypeImport.code = code;
                                        purchaseOrderShippingTypeImport.name = name;
                                        purchaseOrderShippingTypeImport.description = description;

                                        purchaseOrderShippingTypeImport.id_userUpdate = ActiveUser.id;
                                        purchaseOrderShippingTypeImport.dateUpdate = DateTime.Now;

                                        db.PurchaseOrderShippingType.Attach(purchaseOrderShippingTypeImport);
                                        db.Entry(purchaseOrderShippingTypeImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodePurchaseOrderShippingType(int id_purchaseOrderShippingType, string code)
        {
            PurchaseOrderShippingType purchaseOrderShippingType = db.PurchaseOrderShippingType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_purchaseOrderShippingType);

            if (purchaseOrderShippingType == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Medio de Envio" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult PurchaseOrderShippingTypeReport()
        {
            PurchaseOrderShippingTypeReport report = new PurchaseOrderShippingTypeReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_PurchaseOrderShippingTypeReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderShippingTypeDetailReport(int id)
        {
            PurchaseOrderShippingTypeDetailReport report = new PurchaseOrderShippingTypeDetailReport();
            report.Parameters["id_purchaseOrderShippingType"].Value = id;
            return PartialView("_PurchaseOrderShippingTypeReport", report);
        }

        #endregion
    }
}



 