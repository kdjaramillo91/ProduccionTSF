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
    public class CustomerTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region customerType GRIDVIEW

        [HttpPost, ValidateInput(false)]

        public ActionResult CustomerTypePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.CustomerType.FirstOrDefault(b => b.id == keyToCopy);
            }

            var model = db.CustomerType.Where(wht => wht.id_company == this.ActiveCompanyId);
            return PartialView("_CustomerTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerTypePartialAddNew(CustomerType item)
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

                        db.CustomerType.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipo de Cliente: " + item.name + " guardado exitosamente");
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

            var model = db.CustomerType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CustomerTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]

        public ActionResult CustomerTypePartialUpdate(CustomerType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.CustomerType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            //modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.CustomerType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Tipo de Cliente: " + item.name + " guardado exitosamente");
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

            var model = db.CustomerType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CustomerTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomerTypePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.CustomerType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.CustomerType.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipo de Cliente: " + (item?.name ?? "") + " desactivado exitosamente");
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

            var model = db.CustomerType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CustomerTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedCustomerType(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.CustomerType.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.CustomerType.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipos de Clientes desactivados exitosamente");
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

            var model = db.CustomerType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CustomerTypePartial", model.ToList());
        }

        #endregion

        #region REPORTS 


        //[HttpPost]
        //public ActionResult CategoryReport(int[] ids)
        //{
        //    WarehouseTypeReport report = new WarehouseTypeReport();
        //    report.Parameters["id_company"].Value = this.ActiveCompanyId;
        //    return PartialView("_CategoryReport", report);
        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult WarehouseTypeDetailReport(int id)
        //{
        //    WarehouseTypeDetailReport report = new WarehouseTypeDetailReport();
        //    report.Parameters["id__warehouseType"].Value = id;
        //    return PartialView("_WarehouseTypeReport", report);}

        #endregion

        #region AUXILIAR FUNCTIONS

        //public JsonResult ImportFileWarehouseType()
        //{
        //    if (Request.Files.Count > 0)
        //    {
        //        HttpPostedFileBase file = Request.Files[0];

        //        List<string> errorMessages = new List<string>();

        //        if (file != null)
        //        {
        //            string filename = Server.MapPath("~/App_Data/Temp/" + file.FileName);

        //            if (System.IO.File.Exists(filename))
        //            {
        //                System.IO.File.Delete(filename);
        //            }
        //            file.SaveAs(filename);

        //            Excel.Application application = new Excel.Application();
        //            Excel.Workbook workbook = application.Workbooks.Open(filename);

        //            if (workbook.Sheets.Count > 0)
        //            {
        //                Excel.Worksheet worksheet = workbook.ActiveSheet;
        //                Excel.Range table = worksheet.UsedRange;

        //                string code = string.Empty;
        //                string name = string.Empty;
        //                string description = string.Empty;

        //                using (DbContextTransaction trans = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        for (int i = 2; i < table.Rows.Count; i++)
        //                        {
        //                            Excel.Range row = table.Rows[i];
        //                            try
        //                            {
        //                                code = row.Cells[1].Text;
        //                                name = row.Cells[2].Text;
        //                                description = row.Cells[4].Text;
        //                            }
        //                            catch (Exception)
        //                            {
        //                                errorMessages.Add($"Error en formato de datos fila: {i}.");
        //                            }

        //                            WarehouseType wareHouseType = db.WarehouseType.FirstOrDefault(l => l.code.Equals(code));

        //                            if (wareHouseType == null)
        //                            {
        //                                wareHouseType = new WarehouseType
        //                                {
        //                                    code = code,
        //                                    name = name,
        //                                    description = description,
        //                                    isActive = true,

        //                                    id_company = this.ActiveCompanyId,
        //                                    id_userCreate = ActiveUser.id,
        //                                    dateCreate = DateTime.Now,
        //                                    id_userUpdate = ActiveUser.id,
        //                                    dateUpdate = DateTime.Now
        //                                };

        //                                db.WarehouseType.Add(wareHouseType);
        //                            }
        //                            else
        //                            {
        //                                wareHouseType.code = code;
        //                                wareHouseType.name = name;
        //                                wareHouseType.description = description;

        //                                wareHouseType.id_userUpdate = ActiveUser.id;
        //                                wareHouseType.dateUpdate = DateTime.Now;

        //                                db.WarehouseType.Attach(wareHouseType);
        //                                db.Entry(wareHouseType).State = EntityState.Modified;
        //                            }
        //                        }

        //                        db.SaveChanges();
        //                        trans.Commit();
        //                    }
        //                    catch (Exception)
        //                    {
        //                        trans.Rollback();
        //                    }
        //                }
        //            }

        //            application.Workbooks.Close();

        //            if (System.IO.File.Exists(filename))
        //            {
        //                System.IO.File.Delete(filename);
        //            }

        //            return Json(file?.FileName, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult ValidateCodeCustomerType(int id_customerType, string code)
        {
            CustomerType customerType = db.CustomerType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_customerType);

            if (customerType == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tipo de Cliente" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}




