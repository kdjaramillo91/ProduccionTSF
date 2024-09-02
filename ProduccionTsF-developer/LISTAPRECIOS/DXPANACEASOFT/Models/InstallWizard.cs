using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class InstallWizard
    {
        public bool acceptTermsAndConditions { get; set; }

        public Company Company { get; set; }

        public Division Division { get; set; }

        public BranchOffice BranchOffice { get; set; }

        public  EmissionPoint EmissionPoint { get; set; }

        public User User { get; set; }
    }
}