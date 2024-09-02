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
    public class OrderReasonController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region orderReason GRIDVIEW

        [HttpPost, ValidateInput(false)]

        public ActionResult OrderReasonPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.OrderReason.FirstOrDefault(b => b.id == keyToCopy);
            }

            var model = db.OrderReason.Where(wht => wht.id_company == this.ActiveCompanyId);
            return PartialView("_OrderReasonPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult OrderReasonPartialAddNew(OrderReason item)
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

                        db.OrderReason.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Motivo de Orden de Producción: " + item.name + " guardada exitosamente");
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

            var model = db.OrderReason.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_OrderReasonPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]

        public ActionResult OrderReasonPartialUpdate(OrderReason item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.OrderReason.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.OrderReason.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Motivo de Orden de Producción: " + item.name + " guardada exitosamente");
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

            var model = db.OrderReason.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_OrderReasonPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult OrderReasonPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.OrderReason.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.OrderReason.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Motivo de Orden de Producción: " + (item?.name ?? "") + " desactivada exitosamente");
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

            var model = db.OrderReason.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_OrderReasonPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedOrderReason(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.OrderReason.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.OrderReason.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Motivos de Orden de Producción desactivados exitosamente");
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

            var model = db.OrderReason.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_OrderReasonPartial", model.ToList());
        }

        #endregion

        #region REPORTS 


        #endregion

        #region AUXILIAR FUNCTIONS

        

        [HttpPost]
        public JsonResult ValidateCodeOrderReason(int id_orderReason, string code)
        {
            OrderReason orderReason = db.OrderReason.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_orderReason);

            if (orderReason == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Motivo de Orden de Producción" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}




