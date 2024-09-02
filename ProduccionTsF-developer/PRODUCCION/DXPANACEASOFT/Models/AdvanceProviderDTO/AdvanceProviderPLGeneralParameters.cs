
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.AdvanceProviderDTO
{
    public class AdvanceProviderPLGeneralParameters
    {
        public decimal percentageTailPerformanceUsed { get; set; }

        public int id_processType { get; set; }

        public string codeProcessType { get; set; }

        public int id_grammage { get; set; }

        public int? id_priceList { get; set; }

        public int? idProvider { get; set; }
        public decimal totalPoundsLot { get; set; }

        public decimal performanceLot { get; set; }

        

    }
}