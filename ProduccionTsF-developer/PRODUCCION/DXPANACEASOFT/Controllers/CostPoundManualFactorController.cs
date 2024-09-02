using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class CostPoundManualFactorController : DefaultController
	{
		[HttpPost]
		public ActionResult Index()
		{
			return PartialView();
		}


        #region CostPoundManualFactor GridView
        //[ValidateInput(false)]
        public ActionResult CostsPoundManualFactorPartial()
        {
            var model = db.CostPoundManualFactor;
            return PartialView("_CostsPoundManualFactorPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostsPoundManualFactorPartialAddNew(CostPoundManualFactor item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.idUserCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.idUserUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.CostPoundManualFactor.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Factor " + item.name + " guardado exitosamente");
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

            var model = db.CostPoundManualFactor;
            return PartialView("_CostsPoundManualFactorPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostsPoundManualFactorPartialUpdate(CostPoundManualFactor item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.CostPoundManualFactor.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.año = item.año;
                            modelItem.mes = item.mes;
                            modelItem.category = item.category;
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.valor = item.valor;
                            modelItem.codTiposItem = item.codTiposItem;
                            modelItem.isActive = item.isActive;

                            modelItem.idUserUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.CostPoundManualFactor.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Factor " + item.name + " guardado exitosamente");
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

            var model = db.CostPoundManualFactor;
            return PartialView("_CostsPoundManualFactorPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostsPoundManualFactorPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.CostPoundManualFactor.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.idUserUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                            item.isActive = false;

                        }

                        db.CostPoundManualFactor.Attach(item);
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
            var model = db.CostPoundManualFactor;
            return PartialView("_CostsPoundManualFactorPartial", model.ToList());
        }

        [HttpPost]
        public void DeleteSelectedCostsPoundManualFactor(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.CostPoundManualFactor.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;
                            db.Entry(item).State = EntityState.Modified;
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
        }

        [HttpPost]
        public JsonResult ValidateCodeCostPoundManualFactor(int id, int? año, int? mes, string code)
        {
            CostPoundManualFactor data = db.CostPoundManualFactor
                .FirstOrDefault(b => b.año == año && b.mes == mes && b.code == code && b.id != id);

            if (data == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Factor" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}



