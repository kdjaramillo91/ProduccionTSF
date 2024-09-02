using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class AccountingFreightDetails
    {
        public int id { get; set; }
        public int id_accountingFreight { get; set; }
        public string accountingAccountCode { get; set; }
        public bool isAuxiliar { get; set; }
        public string code_Auxiliar { get; set; }
        public string idAuxContable { get; set; }
        public int? id_costCenter { get; set; }
        public int? id_subCostCenter { get; set; }
        public string accountType { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public Nullable<int> id_userUpdate { get; set; }
        public Nullable<System.DateTime> dateUpdate { get; set; }
        public bool isActive { get; set; }
    }
}