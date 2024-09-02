
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Estructure.EstructureModels
{
    public class CompanyModelP
    {
        public int idCompanyModelP { get; set; }
        public string nameCompanyModelP { get; set; }

    }
    public class DivisionModelP
    {
        public int idDivisionModelP { get; set; }
        public int idCompanyModelP { get; set; }
        public string nameDivisionModelP { get; set; }

    }

    public class BranchOfficeModelP
    {
        public int idBranchOfficeModelP { get; set; }
        public int idDivisionModelP { get; set; }
        public string nameBranchOfficeModelP { get; set; }

    }
}