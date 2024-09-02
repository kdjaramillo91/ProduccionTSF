
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.RemGuide
{
	public class RemissionGuideRiverTransportLiq
	{
		public int id_remissionGuideRiver { get; set; }

		public string number { get; set; }

		public DateTime emissionDate { get; set; }

		public int? id_personaProcessPlant { get; set; }

		public string personaProcessPlant { get; set; }

		public string nameProvider { get; set; }

		public string nameSite { get; set; }

		public string nameZone { get; set; }

		public string nameDriver { get; set; }

		public string namePlac { get; set; }

		public string nameCiaFact { get; set; }

		public string nameCiaTrans { get; set; }

		public decimal? priceCancelled { get; set; }
		public decimal? price { get; set; }

		public decimal? priceAdvance { get; set; }

		public decimal? priceAdjustment { get; set; }

		public decimal? priceDays { get; set; }

		public decimal? priceExtension { get; set; }

		public decimal? priceSubTotal { get; set; }

		public decimal? priceTotal { get; set; }

		public decimal? quantityPoundsTransported { get; set; }

		public string descriptionRG { get; set; }
	}
}