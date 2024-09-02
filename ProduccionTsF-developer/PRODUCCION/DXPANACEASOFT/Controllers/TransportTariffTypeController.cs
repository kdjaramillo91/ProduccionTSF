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
    public class TransportTariffTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region TransportTariffType GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportTariffTypePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.TransportTariffType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.TransportTariffType.Where(whl => whl.id_company == this.ActiveCompanyId);
            return PartialView("_TransportTarifftypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportTariffTypePartialAddNew(TransportTariffType item)
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

                        db.TransportTariffType.Add(item);
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

            var model = db.TransportTariffType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_TransportTariffTypePartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult TransportTariffTypePartialUpdate(TransportTariffType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.TransportTariffType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.isInternal = item.isInternal;
                            modelItem.id_shippingType = item.id_shippingType;
                            modelItem.isActive = item.isActive;
                            modelItem.id_company = ActiveCompany.id;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.TransportTariffType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Tarifario: " + item.name + " guardada exitosamente");
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

            var model = db.TransportTariffType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_TransportTariffTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportTariffTypePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.TransportTariffType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.TransportTariffType.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();


                            ViewData["EditMessage"] = SuccessMessage("Tarifario: " + (item?.name ?? "") + " desactivada exitosamente");
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

            var model = db.TransportTariffType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_TransportTariffTypePartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedTransportTariffType(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.TransportTariffType.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.TransportTariffType.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tarifarios desactivadas exitosamente");
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

            var model = db.TransportTariffType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_TransportTariffTypePartial", model.ToList());
        }
        #endregion

        #region REPORTS


        //[HttpPost]
        //public ActionResult TransportTariffTypeReport()
        //{
        //    WarehouseReport report = new WarehouseReport();
        //    report.Parameters["id_company"].Value = this.ActiveCompanyId;
        //    return PartialView("_TransportTariffTypeReport", report);
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult WarehouseDetailReport(int id)
        //{
        //    WarehouseDetailReport report = new WarehouseDetailReport();
        //    report.Parameters["id_warehouse"].Value = id;
        //    return PartialView("_WarehouseReport", report);
        //}

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileTransportTariffType()
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
                        bool isInternal = true;
                        int id_shippingType = 0;


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
                                        isInternal = int.Parse(row.Cells[3].Text);
                                        id_shippingType = int.Parse(row.Cells[4].Text);

                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    TransportTariffType warehouseImport = db.TransportTariffType.FirstOrDefault(l => l.code.Equals(code));

                                    if (warehouseImport == null)
                                    {
                                        warehouseImport = new TransportTariffType
                                        {
                                            code = code,
                                            name = name,
                                            isInternal = isInternal,
                                            id_shippingType = id_shippingType,
                                            


                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.TransportTariffType.Add(warehouseImport);
                                    }
                                    else
                                    {
                                        warehouseImport.code = code;
                                        warehouseImport.name = name;
                                        warehouseImport.isInternal = isInternal;
                                        warehouseImport.id_shippingType = id_shippingType;
                                       
                                        warehouseImport.id_userUpdate = ActiveUser.id;
                                        warehouseImport.dateUpdate = DateTime.Now;

                                        db.TransportTariffType.Attach(warehouseImport);
                                        db.Entry(warehouseImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeTransportTariffType(int id_TransportTariffType, string code)
        {
            TransportTariffType warehouse = db.TransportTariffType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_TransportTariffType);

            if (warehouse == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tarifario" }, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}



