
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.RemGuide
{
	public class RGResultsQuery
	{
		public int id { get; set; }

		public string numberDoc { get; set; }

		public string numberDocPurchaseOrder { get; set; }

		public DateTime emissionDateDoc { get; set; }

		public string personProcesPlant { get; set; }

		public string providerName { get; set; }

		public string productionUnitProviderName { get; set; }

		public string certificadoName { get; set; }

        public DateTime despachureDateDoc { get; set; }

		public DateTime? exitTimePlanctDoc { get; set; }

		public DateTime? entranceTimePlanctDoc { get; set; }

		public bool isThird { get; set; }

		public string stateDoc { get; set; }

		public string stateDocElectronic { get; set; }
		public string guia_externa { get; set; }


	}

	public class RGRFilterWindow
	{
		public string codeReport { get; set; }

		public string str_emissionDateStart { get; set; }

		public string str_emissionDateEnd { get; set; }
	}
}