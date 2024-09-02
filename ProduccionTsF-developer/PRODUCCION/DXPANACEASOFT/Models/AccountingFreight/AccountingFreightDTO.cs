using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Dto
{
    public class AccountingFreightDTO
    {

        public int id { get; set; }
        public int id_processPlant { get; set; }
        public string processPlantName { get; set; }
        public int liquidation_type { get; set; }
        public string liquidationName { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }

        public virtual AccountingFreightDetailsDTO AccountingFreightDetail {  get; set; }

    }
}