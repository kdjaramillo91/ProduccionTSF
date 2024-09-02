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
using Newtonsoft.Json;
using System.Globalization;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ProductionSchedulePeriodController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region INVENTORY LINE GRIDVIEW

        public ActionResult ProductionSchedulePeriodsPartial(int? keyToCopy){
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ProductionSchedulePeriod.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.ProductionSchedulePeriod.Where(il => il.id_company == this.ActiveCompanyId).OrderByDescending(obd => obd.id);
            return PartialView("_ProductionSchedulePeriodsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionSchedulePeriodsPartialAddNew(ProductionSchedulePeriod item)
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

                        db.ProductionSchedulePeriod.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Período de Programación de Producción: " + item.name + " guardado exitosamente");
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

            var model = db.ProductionSchedulePeriod.Where(o => o.id_company == this.ActiveCompanyId).OrderByDescending(obd=> obd.id);
            return PartialView("_ProductionSchedulePeriodsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionSchedulePeriodsPartialUpdate(ProductionSchedulePeriod item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ProductionSchedulePeriod.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.name = item.name;


                            modelItem.dateStar = item.dateStar;
                            modelItem.dateEnd = item.dateEnd;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.ProductionSchedulePeriod.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Período de Programación de Producción: " + item.name + " guardado exitosamente");
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

            var model = db.ProductionSchedulePeriod.Where(o => o.id_company == this.ActiveCompanyId).OrderByDescending(obd => obd.id);
            return PartialView("_ProductionSchedulePeriodsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionSchedulePeriodsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ProductionSchedulePeriod.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.ProductionSchedulePeriod.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Período de Programación de Producción: " + (item?.name ?? "") + " desactivado exitosamente");
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

            var model = db.ProductionSchedulePeriod.Where(o => o.id_company == this.ActiveCompanyId).OrderByDescending(obd => obd.id);
            return PartialView("_ProductionSchedulePeriodsPartial", model.ToList());
        }


        [HttpPost]
        public ActionResult DeleteSelectedProductionSchedulePeriods(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ProductionSchedulePeriod.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ProductionSchedulePeriod.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Período(s) de Programación de Producción desactivado(s) exitosamente");
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

            var model = db.ProductionSchedulePeriod.Where(o => o.id_company == this.ActiveCompanyId).OrderByDescending(obd => obd.id);
            return PartialView("_ProductionSchedulePeriodsPartial", model.ToList());
        }

        #endregion

        #region MASTER DETAILS VIEW

        //public ActionResult ProductionSchedulePeriodDetailItemTypesPartial(int? id_ProductionSchedulePeriod)
        //{
        //    ProductionSchedulePeriod productionSchedulePeriod = db.ProductionSchedulePeriod.FirstOrDefault(il => il.id == id_productionSchedulePeriod);
        //    var model = ProductionSchedulePeriod?.ItemType?.ToList() ?? new List<ItemType>();

        //    return PartialView("_ProductionSchedulePeriodDetailItemTypesPartial", model.ToList());
        //}

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult OnDateIncludedInPeriodsValidation(string aDate, int? id_productionSchedulePeriod)
        {
            var result = new
            {
                itsIncluded = 0,
                Error = ""

            };

            DateTime _emissionDate = JsonConvert.DeserializeObject<DateTime>(aDate);

            var auxProductionSchedulePeriod = db.ProductionSchedulePeriod.AsEnumerable().FirstOrDefault(fod=> DateTime.Compare(_emissionDate.Date, fod.dateStar) >= 0 &&
                                                                                                              DateTime.Compare(fod.dateEnd, _emissionDate.Date) >= 0 && fod.id != id_productionSchedulePeriod && fod.id_company == this.ActiveCompanyId);

            if (auxProductionSchedulePeriod != null)
            {
                result = new
                {
                    itsIncluded = 1,
                    Error = "La Fecha esta incluida en otro período."
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateName(string dateStar, string dateEnd)
        {
           
            DateTime? _dateStar = dateStar == "" ? (DateTime?)null : JsonConvert.DeserializeObject<DateTime>(dateStar);
            DateTime? _dateEnd = dateEnd == "" ? (DateTime?)null : JsonConvert.DeserializeObject<DateTime>(dateEnd);

            var periodo = (_dateStar == null ? "" : (_dateStar.Value.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() +
                          _dateStar.Value.ToString("_dd") + "(" + _dateStar.Value.ToString("yyyy") + ")")) + " - " +
                          (_dateEnd == null ? "" : (_dateEnd.Value.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() +
                          _dateEnd.Value.ToString("_dd") + "(" + _dateEnd.Value.ToString("yyyy") + ")"));

            var result = new
            {
                name = periodo

            };


            return Json(result, JsonRequestBehavior.AllowGet);

        }

        //[HttpPost]
        //public JsonResult ValidateCode(int id_ProductionSchedulePeriod, string code)
        //{
        //    ProductionSchedulePeriod ProductionSchedulePeriod = db.ProductionSchedulePeriod.FirstOrDefault(b => b.id_company == this.ActiveCompanyId 
        //                                                                    && b.code == code
        //                                                                    && b.id != id_ProductionSchedulePeriod);

        //    if (ProductionSchedulePeriod == null)
        //    {
        //        return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { isValid = false, errorText = "Código en uso por otra línea de inventario" }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult ImportFileProductionSchedulePeriod()
        //{
        //    if(Request.Files.Count > 0)
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

        //            if(workbook.Sheets.Count > 0)
        //            {
        //                Excel.Worksheet worksheet = workbook.ActiveSheet;
        //                Excel.Range table = worksheet.UsedRange;

        //                string code = string.Empty;
        //                string name = string.Empty;
        //                string description = string.Empty;
        //                int sequential = 0;
        //                bool kardexControl = false;

        //                using (DbContextTransaction trans = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        for (int i = 2; i < table.Rows.Count; i++)
        //                        {
        //                            Excel.Range row = table.Rows[i]; // FILA i
        //                            try
        //                            {
        //                                code = row.Cells[1].Text;        // COLUMNA 1
        //                                name = row.Cells[2].Text;
        //                                sequential = int.Parse(row.Cells[3].Text);
        //                                description = row.Cells[4].Text;
        //                                kardexControl = row.Cells[5].Text.Equals("SI");
        //                            }
        //                            catch (Exception)
        //                            {
        //                                errorMessages.Add($"Error en formato de datos fila: {i}.");
        //                            }

        //                            ProductionSchedulePeriod ProductionSchedulePeriod = db.ProductionSchedulePeriod.FirstOrDefault(l => l.code.Equals(code));

        //                            if (ProductionSchedulePeriod == null)
        //                            {
        //                                ProductionSchedulePeriod = new ProductionSchedulePeriod
        //                                {
        //                                    code = code,
        //                                    name = name,
        //                                    description = description,
        //                                    sequential = sequential,
        //                                    kardexControl = kardexControl,
        //                                    isActive = true,

        //                                    id_company = this.ActiveCompanyId,
        //                                    id_userCreate = ActiveUser.id,
        //                                    dateCreate = DateTime.Now,
        //                                    id_userUpdate = ActiveUser.id,
        //                                    dateUpdate = DateTime.Now
        //                                };

        //                                db.ProductionSchedulePeriod.Add(ProductionSchedulePeriod);
        //                            }
        //                            else
        //                            {
        //                                ProductionSchedulePeriod.code = code;
        //                                ProductionSchedulePeriod.name = name;
        //                                ProductionSchedulePeriod.description = description;
        //                                ProductionSchedulePeriod.sequential = sequential;
        //                                ProductionSchedulePeriod.kardexControl = kardexControl;

        //                                ProductionSchedulePeriod.id_userUpdate = ActiveUser.id;
        //                                ProductionSchedulePeriod.dateUpdate = DateTime.Now;

        //                                db.ProductionSchedulePeriod.Attach(ProductionSchedulePeriod);
        //                                db.Entry(ProductionSchedulePeriod).State = EntityState.Modified;
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

        #endregion

        #region REPORT

        //[HttpPost, ValidateInput(false)]
        //public ActionResult ProductionSchedulePeriodReport()
        //{
        //    ProductionSchedulePeriodReport report = new ProductionSchedulePeriodReport();
        //    report.Parameters["id_company"].Value = this.ActiveCompanyId;
        //    return PartialView("_ProductionSchedulePeriodReport", report);
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult ProductionSchedulePeriodDetailReport(int id)
        //{
        //    ProductionSchedulePeriodDetailReport report = new ProductionSchedulePeriodDetailReport();
        //    report.Parameters["id_ProductionSchedulePeriod"].Value = id;
        //    return PartialView("_ProductionSchedulePeriodReport", report);
        //}

        #endregion
    }
}

