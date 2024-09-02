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
    public class ProductionExpenseController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Production Expense GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionExpensePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ProductionExpense.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.ProductionExpense.Where(whl => whl.id_company == this.ActiveCompanyId).OrderBy(e => e.orden);
            return PartialView("_ProductionExpensePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionExpensePartialAddNew(ProductionExpense item)
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

                        db.ProductionExpense.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Gasto de Producción: " + item.name + " guardado exitosamente");
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

            var model = db.ProductionExpense.Where(o => o.id_company == this.ActiveCompanyId).OrderBy(e => e.orden);
            return PartialView("_ProductionExpensePartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionExpensePartialUpdate(ProductionExpense item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ProductionExpense.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.id_productionCostType = item.id_productionCostType;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.orden = item.orden;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.ProductionExpense.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Gasto de Producción: " + item.name + " guardado exitosamente");
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

            var model = db.ProductionExpense.Where(o => o.id_company == this.ActiveCompanyId).OrderBy(e => e.orden);
            return PartialView("_ProductionExpensePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionExpensePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ProductionExpense.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ProductionExpense.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();


                            ViewData["EditMessage"] = SuccessMessage("Gasto de Producción: " + (item?.name ?? "") + " desactivado exitosamente");
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

            var model = db.ProductionExpense.Where(o => o.id_company == this.ActiveCompanyId).OrderBy(e => e.orden);
            return PartialView("_ProductionExpensePartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedProductionExpense(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ProductionExpense.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ProductionExpense.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Gastos desactivados exitosamente");
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

            var model = db.ProductionExpense.Where(o => o.id_company == this.ActiveCompanyId).OrderBy(e => e.orden);
            return PartialView("_ProductionExpensePartial", model.ToList());
        }
        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ValidateCodeProductionExpense(int id_productionExpense, string code)
        {
            ProductionExpense productionExpense = db.ProductionExpense.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_productionExpense);

            if (productionExpense == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Gasto" }, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}



