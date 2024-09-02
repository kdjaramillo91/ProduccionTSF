using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class AccountingFreight
    {
        public int id { get; set; }
        public int id_processPlant { get; set; }
        public string liquidation_type { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public Nullable<int> id_userUpdate { get; set; }
        public Nullable<System.DateTime> dateUpdate { get; set; }

        public virtual ICollection<AccountingFreightDetails> AccountingFreightDetails { get; set; }

    }
}