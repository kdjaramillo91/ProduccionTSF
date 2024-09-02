using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class MetricUnitConversionController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region MetricUnitConversion GridView

        [ValidateInput(false)]
        public ActionResult MetricUnitConversionsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.Country.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.MetricUnitConversion.Where(d => d.id_company == this.ActiveCompanyId);
            return PartialView("_MetricUnitConversionsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricUnitConversionsPartialAddNew(MetricUnitConversion item)
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

                        db.MetricUnitConversion.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Conversión de Medida guardada exitosamente");
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

            var model = db.MetricUnitConversion.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_MetricUnitConversionsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricUnitConversionsPartialUpdate(MetricUnitConversion item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.MetricUnitConversion.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.id_metricOrigin = item.id_metricOrigin;
                            modelItem.id_metricDestiny = item.id_metricDestiny;
                            modelItem.factor = item.factor;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.MetricUnitConversion.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Conversión de Medida guardada exitosamente");
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

            var model = db.MetricUnitConversion.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_MetricUnitConversionsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricUnitConversionsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.MetricUnitConversion.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }
                        db.MetricUnitConversion.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            var model = db.MetricUnitConversion.Where(d => d.id_company == this.ActiveCompanyId);
            return PartialView("_MetricUnitConversionsPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedMetricUnits(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.MetricUnitConversion.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.MetricUnitConversion.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Paises desactivados exitosamente");
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

            var model = db.MetricUnitConversion.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_MetricUnitConversionsPartial", model.ToList());
        }


        #endregion

        //public ActionResult ComboBoxMetricsUnitsPartial(int? id_metricOrigin)
        //{
        //    //var id_metricOrigin = Request.Params["id_metricOrigin"];

        //    var destinyMetricsUnits = (id_metricOrigin == null)
        //                                        ? DataProviderMetricUnit.MetricUnits()
        //                                        : DataProviderMetricUnit.MetricsUnitsByMectricUnitConversion((int)id_metricOrigin);


        //    return PartialView("_ComboBoxMetricsUnitsPartial", destinyMetricsUnits);
        //}

        //[HttpPost]
        //public void DeleteSelectedMetricUnitConversions(int[] ids)
        //{
        //    if (ids != null && ids.Length > 0)
        //    {
        //        using (DbContextTransaction trans = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var metricunitconversions = db.MetricUnitConversion.Where(i => ids.Contains(i.id_metricOrigin));
        //                foreach (var metricunitconversion in metricunitconversions)
        //                {
        //                    metricunitconversion.isActive = false;
        //                    db.Entry(metricunitconversion).State = EntityState.Modified;
        //                }
        //                db.SaveChanges();
        //                trans.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                trans.Rollback();
        //            }
        //        }
        //    }
        //}

        #region REPORTS

        [HttpPost]
        public ActionResult MetricUnitConversionReport()
        {
            MetricUnitConversionReport report = new MetricUnitConversionReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_MetricUnitConversionReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MetricUnitConversionDetailReport(int id)
        {
            MetricUnitConversionDetailReport report = new MetricUnitConversionDetailReport();
            report.Parameters["id_metricUnitConversion"].Value = id;
            return PartialView("_MetricUnitConversionReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult MetricDestinyByMetricOrigin(int? id_metricOrigin, int? id_metricDestinyIni/*, bool esNuevo*/)
        {
            MetricUnit metricUnitOrigin = db.MetricUnit.FirstOrDefault(mu => mu.id == id_metricOrigin);
            int? id_metricUnitOriginAux = metricUnitOrigin?.id;
            int? id_metricTypeOriginAux = metricUnitOrigin?.MetricType.id;
            MetricUnit metricUnitDestinyIni = db.MetricUnit.FirstOrDefault(mu => mu.id == id_metricDestinyIni);
            int? id_metricUnitDestinyIni = metricUnitDestinyIni?.id;
            int? id_metricTypeDestinyIni = metricUnitDestinyIni?.MetricType.id;
            //if (esNuevo)
            //{
            var model = db.MetricUnit.Where(d => d.id_company == this.ActiveCompanyId &&
                                                     d.isActive &&
                                                     d.id != id_metricUnitOriginAux &&
                                                     d.MetricType.id == id_metricTypeOriginAux &&
                                                     (db.MetricUnitConversion.FirstOrDefault(muc => muc.id_metricOrigin == id_metricOrigin &&
                                                                                                   muc.id_metricDestiny == d.id &&
                                                                                                   muc.id_company == this.ActiveCompanyId) == null ||
                                                      (id_metricTypeOriginAux == id_metricTypeDestinyIni && d.id == id_metricUnitDestinyIni))).ToList();

                var result = new {
                    metricDestinys = model.Select(d => new { d.id, d.name })
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    var model = db.MetricUnit.Where(d => d.id_company == id_company &&
            //                                         d.is_Active &&
            //                                         metricUnitOrigin.MetricType.id == d.MetricType.id &&
            //                                         db.MetricUnitConversion.FirstOrDefault(muc => muc.id_metricOrigin == id_metricOrigin &&
            //                                                                                       muc.id_metricDestiny == d.id &&
            //                                                                                       muc.id_company == id_company) != null).ToList();

            //    var result = model.Select(d => new { d.id, d.name });

            //    return Json(result, JsonRequestBehavior.AllowGet);

            //}
        }

        #endregion
    }
}



