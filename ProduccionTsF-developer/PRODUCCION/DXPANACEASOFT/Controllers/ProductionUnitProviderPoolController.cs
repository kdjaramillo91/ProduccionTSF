using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using DevExpress.Web;
using System.Web.UI.WebControls;
using DXPANACEASOFT.DataProviders;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ProductionUnitProviderPoolController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ProductionUnitProviderPool GRIDVIEW
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderPoolsPartial(int? idProvider, 
            int? id_productionUnitProvider, bool isCallback = false)
        {
            var model = idProvider.HasValue && id_productionUnitProvider.HasValue
                ? db.ProductionUnitProviderPool
                    .Where(e => e.id_productionUnitProvider == id_productionUnitProvider
                        && e.ProductionUnitProvider.id_provider == idProvider)
                    .OrderByDescending(ob => ob.id).ToList()
                : new List<ProductionUnitProviderPool>();

			if (isCallback)
			{
                return PartialView("_ProductionUnitProviderPoolPartial", model);
            }
			else
			{
                return PartialView("_ProductionUnitProviderPoolViewPartial", model);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderPoolsPartialAddNew(ProductionUnitProviderPool item,
            int idProvider, int id_productionUnitProvider)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_productionUnitProvider = id_productionUnitProvider;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.ProductionUnitProviderPool.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Camaronera: " + item.name + " guardada exitosamente");
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

            var model = db.ProductionUnitProviderPool
                .Where(e => e.id_productionUnitProvider == id_productionUnitProvider && e.ProductionUnitProvider.id_provider == idProvider)
                .OrderByDescending(ob => ob.id)
                .ToList();

            return PartialView("_ProductionUnitProviderPoolPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderPoolsPartialUpdate(ProductionUnitProviderPool item,
            int idProvider, int id_productionUnitProvider)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ProductionUnitProviderPool.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.name = item.name;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;
                            modelItem.id_certification = item.id_certification;
                            db.ProductionUnitProviderPool.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Camaronera: " + item.name + " guardada exitosamente");
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

            var model = db.ProductionUnitProviderPool
                .Where(e => e.id_productionUnitProvider == id_productionUnitProvider && e.ProductionUnitProvider.id_provider == idProvider)
                .OrderByDescending(ob => ob.id)
                .ToList();

            return PartialView("_ProductionUnitProviderPoolPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderPoolsPartialDelete(System.Int32 id,
            int idProvider, int id_productionUnitProvider)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ProductionUnitProviderPool.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.ProductionUnitProviderPool.Remove(item);
                        db.Entry(item).State = EntityState.Deleted;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Camaronera: " + (item?.name ?? "") + " eliminada exitosamente");
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

            var model = db.ProductionUnitProviderPool
                .Where(e => e.id_productionUnitProvider == id_productionUnitProvider && e.ProductionUnitProvider.id_provider == idProvider)
                .OrderByDescending(ob => ob.id)
                .ToList();

            return PartialView("_ProductionUnitProviderPoolPartial", model);
        }

        [HttpPost]
        public ActionResult DeleteSelectedProductionUnitProviderPools(int[] ids,
            int idProvider, int id_productionUnitProvider)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ProductionUnitProviderPool.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ProductionUnitProviderPool.Remove(item);
                            db.Entry(item).State = EntityState.Deleted;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Camaroneras desactivadas exitosamente");
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

            var model = db.ProductionUnitProviderPool
                .Where(e => e.id_productionUnitProvider == id_productionUnitProvider && e.ProductionUnitProvider.id_provider == idProvider)
                .OrderByDescending(ob => ob.id)
                .ToList();

            return PartialView("_ProductionUnitProviderPoolPartial", model);
        }

        #endregion

        #region AUXILIAR FUNCTIONS
        [HttpPost]
        public JsonResult ValidateCodeProductionUnitProviderPool(int id_productionUnitProviderPool,
            int id_productionUnitProvider, string code)
        {
            var productionUnitProviderPool = db.ProductionUnitProviderPool
                .FirstOrDefault(b => b.code == code  && b.id_productionUnitProvider == id_productionUnitProvider
                    && b.id != id_productionUnitProviderPool);

            if (productionUnitProviderPool == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra camaronera." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetProductionUnitProviderByProvider(int? idProvider)
        {
            var result = DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, idProvider);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        public ActionResult GetCertificationsByPriceList(int? id_priceList, int? id_certificationCurrent)
        {
            TempData.Keep("productionUnitProviderPool");

            var aPriceList = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
            var id_certificationPriceList = aPriceList?.id_certification;
            var items = db.Certification.Where(w => w.id == id_certificationPriceList || w.id == id_certificationCurrent || w.isActive).ToList();
            Session["id_certificationUpdate"] = id_certificationPriceList ?? id_certificationCurrent;
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_certification";
                p.Width = Unit.Percentage(100);
                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);
                p.CallbackRouteValues = new { Controller = "ProductionUnitProviderPool", Action = "GetCertificationsByPriceList" };
                p.ClientSideEvents.BeginCallback = "function(s, e) { e.customArgs['id_priceList'] = id_priceList.GetValue(); e.customArgs['id_certificationCurrent'] = id_certification.GetValue();}";
                p.ClientSideEvents.EndCallback = "OnCertification_EndCallback";
                p.CallbackPageSize = 5;
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                p.BindList(items);// .Bind(id_person).ge;
            });
        }

        public JsonResult UpdateCertification()
        {
            TempData.Keep("productionUnitProviderPool");

            var result = new
            {
                id_certificationUpdate = (int?)Session["id_certificationUpdate"]
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}



