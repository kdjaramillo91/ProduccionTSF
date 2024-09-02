using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class DashboardDesignerController : Controller
    {
        // GET: DashboardDesigner
        public ActionResult Index()
        {
            return PartialView();
        }
    }
}