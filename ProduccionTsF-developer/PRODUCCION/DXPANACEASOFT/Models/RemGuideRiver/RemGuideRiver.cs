
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.RemGuideRiver
{
	public class RGRParamPriceFreight
	{
		public int? id_FishingSite { get; set; }

		public int? id_TransportTariff { get; set; }

	}
	public class RemGuideResultsForRiver
	{
		public int id { get; set; }

		public string numberRG { get; set; }

		public int? id_personProcessPlant { get; set; }
		public string personProcessPlant { get; set; }

		public string nameProvider { get; set; }

		public string namerProductionUnitProvider { get; set; }

		public bool requiredLogistic { get; set; }
        
        public DateTime despachureDate { get; set; }

		public DateTime emissionDate { get; set; }

		public string nameZone { get; set; }

		public string nameSite { get; set; }

		public string nameItem { get; set; }

		public int daysDiff { get; set; }
	}

}