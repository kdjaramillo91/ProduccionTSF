using DevExpress.Web.Mvc;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

namespace DXPANACEASOFT.Controllers
{
    public class CalendarPriceListTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region CalendarPriceListType GRIDVIEW

        [ValidateInput(false)]
        public ActionResult CalendarPriceListTypePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.CalendarPriceListType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.CalendarPriceListType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CalendarPriceListTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CalendarPriceListTypePartialAddNew(DXPANACEASOFT.Models.CalendarPriceListType item)
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
                        db.CalendarPriceListType.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Lista de Precios: " + item.name + " guardada exitosamente");
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

            var model = db.CalendarPriceListType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CalendarPriceListTypePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CalendarPriceListTypePartialUpdate(DXPANACEASOFT.Models.CalendarPriceListType item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.CalendarPriceListType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            //modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            //modelItem.isQuotation = item.isQuotation;
                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.CalendarPriceListType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] =
                                SuccessMessage("Lista de Precios: " + item.name + " guardada exitosamente");
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

            var model = db.CalendarPriceListType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CalendarPriceListTypePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CalendarPriceListTypePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.CalendarPriceListType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.CalendarPriceListType.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Lista de Precios: " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.CalendarPriceListType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CalendarPriceListTypePartial", model.ToList());
        }

        public ActionResult DeleteSelectedCalendarPriceListType(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var price_list_type = db.CalendarPriceListType.Where(i => ids.Contains(i.id));
                        foreach (var CalendarPriceListType in price_list_type)
                        {
                            CalendarPriceListType.isActive = false;

                            CalendarPriceListType.id_userUpdate = ActiveUser.id;
                            CalendarPriceListType.dateUpdate = DateTime.Now;

                            db.CalendarPriceListType.Attach(CalendarPriceListType);
                            db.Entry(CalendarPriceListType).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Listas de Precios desactivadas exitosamente");
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

            var model = db.CalendarPriceListType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CalendarPriceListTypePartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileCalendarPriceListType()
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

                                    CalendarPriceListType calendarPriceListTypeImport = db.CalendarPriceListType.FirstOrDefault(/*l => l.code.Equals(code)*/);

                                    if (calendarPriceListTypeImport == null)
                                    {
                                        calendarPriceListTypeImport = new CalendarPriceListType
                                        {
                                            //code = code,
                                            name = name,
                                            description = description,
                                            isActive = true,
                                            //isQuotation = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.CalendarPriceListType.Add(calendarPriceListTypeImport);
                                    }
                                    else
                                    {
                                        //calendarPriceListTypeImport.code = code;
                                        calendarPriceListTypeImport.name = name;
                                        calendarPriceListTypeImport.description = description;

                                        calendarPriceListTypeImport.id_userUpdate = ActiveUser.id;
                                        calendarPriceListTypeImport.dateUpdate = DateTime.Now;

                                        db.CalendarPriceListType.Attach(calendarPriceListTypeImport);
                                        db.Entry(calendarPriceListTypeImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeCalendarPriceListType(int id_calendarPriceListType, string code)
        {
            CalendarPriceListType calendarPriceListType = db.CalendarPriceListType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            //&& b.code == code
                                                                            && b.id != id_calendarPriceListType);

            if (calendarPriceListType == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Lista de Precios" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult CalendarPriceListTypeReport()
        {
            PriceListTypeReport report = new PriceListTypeReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_CalendarPriceListTypeReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CalendarPriceListTypeDetailReport(int id)
        {
            PriceListTypeDetailReport report = new PriceListTypeDetailReport();
            report.Parameters["id_calendarPriceListType"].Value = id;
            return PartialView("_CalendarPriceListTypeReport", report);
        }

        #endregion

    }
}