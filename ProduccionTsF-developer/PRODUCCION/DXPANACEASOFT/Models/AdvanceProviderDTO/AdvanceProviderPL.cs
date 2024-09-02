
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.AdvanceProviderDTO
{
	public class AdvanceProviderPL
	{
		public int id { get; set; }

		public string number { get; set; }

		public string internalNumber { get; set; }

		public string lotState { get; set; }

        public string personProcessPlant { get; set; }

		public DateTime ReceptionDate { get; set; }

		public int id_provider { get; set; }

		public string ProviderName { get; set; }

		public int id_buyer { get; set; }

		public string BuyerName { get; set; }

		public decimal QuantityPoundsReceived { get; set; }

		public string ZoneName { get; set; }

		public string SiteName { get; set; }

	}
}