
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.RemGuide
{
    public class RGRResultsQuery
    {
        public int id { get; set; }

        public string numberDoc { get; set; }

        public DateTime emissionDateDoc { get; set; }

        public string providerName { get; set; }

        public string productionUnitProviderName { get; set; }

        public DateTime despachureDateDoc { get; set; }

        public DateTime? exitTimePlanctDoc { get; set; }

        public bool isThird { get; set; }

        public string stateDoc {get;set;}

        
    }
}