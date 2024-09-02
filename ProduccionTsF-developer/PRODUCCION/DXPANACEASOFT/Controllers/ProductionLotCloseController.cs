using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class ProductionLotCloseController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            List<ProductionLot> model = db.ProductionLot.Where(p => p.ProductionLotState.code == "06" ||
                                                                    p.ProductionLotState.code == "07" ||
                                                                    p.ProductionLotState.code == "08").ToList();

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_ProductionLotReceptionResultsPartial", model.OrderByDescending(o => o.id).ToList());
            //return PartialView();
        }

        #region ResultGridView

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionResultsPartial()
        {
            List<ProductionLot> model = db.ProductionLot.Where(p => p.ProductionLotState.code == "06" ||
                                                                   p.ProductionLotState.code == "07" ||
                                                                   p.ProductionLotState.code == "08").ToList();

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_ProductionLotReceptionResultsPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionResultsOrdersPartial()
        {
            var model = db.PurchaseOrder.OrderByDescending(o => o.id); ;

            return PartialView("_ProductionLotReceptionDetailPartial");
        }

        #endregion

        #region PRODUCTION LOT CLOSE REPORTS

        [HttpPost]
        public ActionResult ProductionLotCloseReport(int id)
        {
            //RemissionGuideReport remissionGuideReport = new RemissionGuideReport();
            //remissionGuideReport.Parameters["id_company"].Value = this.ActiveCompanyId;
            //remissionGuideReport.Parameters["id_remissionGuide"].Value = id;
            //return PartialView("_RemissionGuideReport", remissionGuideReport);
            try
            {
                Session["URRESUMENLOTE"] = ConfigurationManager.AppSettings["URRESUMENLOTE"];
            }
            catch (Exception ex)
            {
                ViewBag.IframeUrl = ex.Message;


            }

            ViewBag.IframeUrl = Session["URRESUMENLOTE"] + "?id=" + id;



            //return Redirect("../Views/AditionalReport/WReportGuia.aspx"); //  View("WReportGuia"); //Aspx file Views/Products/WebForm1.aspx
            return PartialView("IndexReportLote");
        }

        #endregion
    }
}