using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class Vehicle
    {
        public int? id_providerT { get; set; }

		public string rucNameProvider { get; set; }
		public string nameProvider { get; set; }

        public int? id_providerTBilling { get; set; }

		public string rucNameProviderBilling { get; set; }
		public string nameProviderBilling { get; set; }

    }
}