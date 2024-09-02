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
    public class BusinessTurnController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region businessTurn GRIDVIEW

        [HttpPost, ValidateInput(false)]

        public ActionResult BusinessTurnPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.BusinessLine.FirstOrDefault(b => b.id == keyToCopy);
            }

            var model = db.BusinessLine.Where(wht => wht.id_company == this.ActiveCompanyId);
            return PartialView("_BusinessTurnPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessTurnPartialAddNew(BusinessLine item)
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

                        db.BusinessLine.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Giro de Negocio: " + item.name + " guardada exitosamente");
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

            var model = db.BusinessLine.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_BusinessTurnPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]

        public ActionResult BusinessTurnPartialUpdate(BusinessLine item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.BusinessLine.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.BusinessLine.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Giro de Negocio: " + item.name + " guardada exitosamente");
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

            var model = db.BusinessLine.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_BusinessTurnPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessTurnPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.BusinessLine.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.BusinessLine.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Giro de Negocio: " + (item?.name ?? "") + " desactivada exitosamente");
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

            var model = db.BusinessLine.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_BusinessTurnPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedBusinessTurn(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.BusinessLine.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.BusinessLine.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Giros de Negocios desactivados exitosamente");
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

            var model = db.BusinessLine.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_BusinessTurnPartial", model.ToList());
        }

        #endregion

        #region REPORTS 


        #endregion

        #region AUXILIAR FUNCTIONS

        

        [HttpPost]
        public JsonResult ValidateCodeBusinessTurn(int id_businessTurn, string code)
        {
            BusinessLine businessTurn = db.BusinessLine.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_businessTurn);

            if (businessTurn == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Giro de Negocio" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}




