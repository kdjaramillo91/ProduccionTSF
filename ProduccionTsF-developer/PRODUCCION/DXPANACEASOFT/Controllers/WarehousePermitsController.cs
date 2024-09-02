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
    public class WarehousePermitsController : DefaultController
    {
        // GET: WarehousePermits
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: WarehousePermits/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: WarehousePermits/Create
        public ActionResult Create()
        {
            return View();
        }


        [ValidateInput(false)]
        public ActionResult WarehousePermitsPartial()
        {
            var model = db.User.Where(e=> e.isActive == true && e.Company.id == this.ActiveCompanyId);
            return PartialView("_WarehousePermitsPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult FormEditWarehousePermtis()
        {
            var model = db.User.Where(e=> e.isActive == true && e.Company.id == this.ActiveCompanyId);
            return PartialView("_FormEditWarehousePermtis", model.ToList());
        }

        // POST: WarehousePermits/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: WarehousePermits/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WarehousePermits/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: WarehousePermits/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WarehousePermits/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
