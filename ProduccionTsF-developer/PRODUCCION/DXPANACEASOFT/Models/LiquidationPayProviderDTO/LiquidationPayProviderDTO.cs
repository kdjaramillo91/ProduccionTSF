
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.LiquidationPayProviderDTO
{
	public class LiquidationPayProviderFilter
	{
		public string codeReport { get; set; }
		public int? id_provider { get; set; }

		public DateTime? liquidationDateStart { get; set; }

		public DateTime? liquidationDateEnd { get; set; }
	}

	public class LiquidationPayProviderResults
	{
		public int id { get; set; }
		public int id_productionLot { get; set; }

		public int? id_personProcessPlant { get; set; }

		public string personProcessPlant { get; set; }

		public string nameProvider { get; set; }

		public DateTime liquidationDate { get; set; }

		public string numberOoc { get; set; }

		public string internalNumberDoc { get; set; }

		public decimal poundsQuantityReceived { get; set; }

		public decimal valueToPay { get; set; }

		public string processType { get; set; }

		public string nameState { get; set; }

	}
}