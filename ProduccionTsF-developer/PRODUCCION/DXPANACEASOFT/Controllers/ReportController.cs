using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class ReportController : DefaultController
    {


        public string getnamedata() {

            string reportadat = "reportModel";
            try
            {
                if (Request.QueryString.Count > 0 && Request.QueryString["reportModel"] != null)
                {
                    reportadat = Request.QueryString["reportModel"];
                }
            }
            catch (Exception)
            {

                
            }

            reportadat =  reportadat ?? "reportModel";

            return reportadat;
        }

        public ActionResult Index()
        {
            string temdata = getnamedata();


            ReportModel model = (TempData[temdata] as ReportModel);
      
            model = model ?? new ReportModel();
            
            TempData[temdata] = model;
            TempData.Keep(temdata);

            return View(model);
        }



        public ActionResult ReportCallbackPanelPartial(ReportModel reportModel)
        {
            string temdata = getnamedata();
            if (reportModel == null)
            {
                 reportModel = (TempData[temdata] as ReportModel);
            }

            TempData.Keep(temdata);



            return PartialView("_ReportCallbackPanelPartial", reportModel);
        }

        public ActionResult DocumentViewerPartial( ReportModel reportModel)
        {
            string temdata = getnamedata();
            if (reportModel == null)
            {
                reportModel = (TempData[temdata] as ReportModel);
            }

         


            return PartialView("_DocumentViewerPartial", reportModel);
        }

        public ActionResult DocumentViewerExport(ReportModel reportModel)
        {
            return DocumentViewerExtension.ExportTo(reportModel.CreateReport());
        }




        }






    }

