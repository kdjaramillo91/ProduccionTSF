using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class ControlPanelController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }
    }
}