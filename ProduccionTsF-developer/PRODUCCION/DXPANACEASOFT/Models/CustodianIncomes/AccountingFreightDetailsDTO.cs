using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Dto
{
    public class AccountingFreightDetailsDTO
    {
        public int Id { get; set; }
        public int id_accountingFreight { get; set; }
        public string accountingAccountCode { get; set; }
        public bool isAuxiliar { get; set; }
        public string code_Auxiliar { get; set; }
        public int id_costCenter { get; set; }
        public string costCenterName { get; set; }
        public int id_subCostCent { get; set; }
        public string SubcostCenterName { get; set; }
        public string accountType { get; set; }


    }
}