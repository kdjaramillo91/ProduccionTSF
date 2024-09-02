using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    public class AccountPlanController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            var model = db.AccountPlan;
            AccountPlan plan = model.FirstOrDefault(m => m.id == this.ActiveCompanyId);

            return View(plan);
        }

        public ActionResult AccountPlan()
        {
            var model = db.AccountPlan;
            AccountPlan plan = model.FirstOrDefault(m => m.id == this.ActiveCompanyId);

            return PartialView("_AccountPlan", plan);
        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult Guardar_Plan(AccountPlan plan)
        {
            var model = db.AccountPlan;
            AccountPlan plan_model = model.FirstOrDefault(m => m.id == this.ActiveCompanyId);
            bool nuevo = false;
            if (plan_model == null)
            {
                nuevo = true;
                plan_model = new AccountPlan();
                plan_model.id = this.ActiveCompanyId;
                plan_model.id_userCreate = ActiveUser.id;
                plan_model.dateCreate = DateTime.Now;
            }
            plan_model.isActive = plan.isActive;
            plan_model.name = plan.name;
            plan_model.description = plan.description;
            plan_model.separator = plan.separator;
            plan_model.id_userUpdate = ActiveUser.id;
            plan_model.dateUpdate = DateTime.Now;
            if (nuevo)
            {
                model.Add(plan_model);
            }
            else
            {
                this.UpdateModel(plan_model);
            }
            db.SaveChanges();
            return Index();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AccountPlanDelete()
        {
            AccountPlan item = (TempData["plan"] as AccountPlan);
            int id = item.id;
            var model = db.Account;
            try
            {
                var plan = model.FirstOrDefault(it => it.id == id);
                if (plan != null)
                    model.Remove(plan);
                db.SaveChanges();
                TempData.Remove("plan");
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }           
            return Index();
        }

        #region ACCOUNTS

        [ValidateInput(false)]
        public ActionResult AccountsTreeListPartial()
        {
            var model = db.Account;
            return PartialView("_AccountsTreeListPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AccountsTreeListPartialAddNew(DXPANACEASOFT.Models.Account item)
        {
            var model = db.Account;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_AccountsTreeListPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AccountsTreeListPartialUpdate(DXPANACEASOFT.Models.Account item)
        {
            var model = db.Account;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_AccountsTreeListPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AccountsTreeListPartialDelete(System.Int32 id)
        {
            var model = db.Account;
            try
            {
                var item = model.FirstOrDefault(it => it.id == id);
                if (item != null)
                    model.Remove(item);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            return PartialView("_AccountsTreeListPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AccountsTreeListPartialMove(System.Int32 id, System.Int32? id_parentAccount)
        {
            var model = db.Account;
            try
            {
                var item = model.FirstOrDefault(it => it.id == id);
                if (item != null)
                    item.id_parentAccount = id_parentAccount;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            return PartialView("_AccountsTreeListPartial", model.ToList());
        }

        #endregion
    }
}