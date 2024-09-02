
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels
{
    public class AdvanceParametersDetailModelP
    {
        public int idAdvanceDetailModelP { get; set; }

        public string codeAdvanceDetailModelP { get; set; }

        public string nameAdvanceDetailModelP { get; set; }

        public int idAdvanceModelP { get; set; }
    }
    public class AdvanceParametersModelP
    {
        public int idAdvanceModelP { get; set; }

        public string codeAdvanceModelP { get; set; }

        public bool hasDetailModelP { get; set; }

        public int? valueIntegerModelP { get; set; }
        
    }
}