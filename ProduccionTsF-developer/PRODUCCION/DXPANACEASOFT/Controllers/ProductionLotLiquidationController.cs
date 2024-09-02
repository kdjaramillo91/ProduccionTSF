using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class ProductionLotLiquidationController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult GenerateLotLiquidation()
        {
            return PartialView("_ProductionLotLiquidationEditFormPartial");
        }

        [HttpPost]
        public ActionResult ProductionLotLiquidationResults()
        {
            var model = db.ProductionLot.OrderByDescending(l => l.id);
            return PartialView("_ProductionLotLiquidationResultPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult ProductionLotLiquidationsPartial()
        {
            var model = db.ProductionLot;
            return PartialView("_ProductionLotLiquidationsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotLiquidationsPartialAddNew(DXPANACEASOFT.Models.ProductionLot item)
        {
            var model = db.ProductionLot;
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
            return PartialView("_ProductionLotLiquidationsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotLiquidationsPartialUpdate(DXPANACEASOFT.Models.ProductionLot item)
        {
            var model = db.ProductionLot;
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
            return PartialView("_ProductionLotLiquidationsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotLiquidationsPartialDelete(System.Int32 id)
        {
            var model = db.ProductionLot;
            if (id >= 0)
            {
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
            }
            return PartialView("_ProductionLotLiquidationsPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult ProductionLotLiquidationeditFormLiquidationDetailsPartial()
        {
            var model = db.ProductionLotDetail;
            return PartialView("_ProductionLotLiquidationEditFormLiquidationDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotLiquidationEditFormLiquidationDetailsPartialAddNew(DXPANACEASOFT.Models.ProductionLotDetail item)
        {
            var model = db.ProductionLotDetail;
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
            return PartialView("_ProductionLotLiquidationEditFormLiquidationDetailsPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotLiquidationEditFormLiquidationDetailsPartialUpdate(DXPANACEASOFT.Models.ProductionLotDetail item)
        {
            var model = db.ProductionLotDetail;
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
            return PartialView("_ProductionLotLiquidationEditFormLiquidationDetailsPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotLiquidationEditFormLiquidationDetailsPartialDelete(System.Int32 id)
        {
            var model = db.ProductionLotDetail;
            if (id >= 0)
            {
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
            }
            return PartialView("_ProductionLotLiquidationEditFormLiquidationDetailsPartial", model.ToList());
        }
    }
}