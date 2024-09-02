using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    public class InstallWizardController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            InstallWizard wizard = new InstallWizard
            {
                acceptTermsAndConditions = true,
                Company = new Company
                {
                },
                Division = new Division
                {
                },
                BranchOffice = new BranchOffice
                {
                    code = "01",
                },
                EmissionPoint = new EmissionPoint
                {
                    code = 1
                },
                User = new User
                {
                    username = "Administrador",
                } 
            };

            return PartialView(wizard);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Install(InstallWizard wizard)
        {
            return PartialView("_Confirmation");
        }

        public ActionResult BinaryImageColumnPhotoUpdate()
        {
            return BinaryImageEditExtension.GetCallbackResult();
        }
    }
}