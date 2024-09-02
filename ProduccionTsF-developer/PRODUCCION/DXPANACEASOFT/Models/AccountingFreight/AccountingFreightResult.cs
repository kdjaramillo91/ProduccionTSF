using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class AccountingFreightResult
    {
        public int id { get; set; }
        public int id_processPlant { get; set; }
        public string processPlantName { get; set; }
        public string liquidation_type { get; set; }
        public string liquidationName { get; set; }
        public bool isActive{ get; set; }

    }
}