using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ModelExtension
{
	public class ProductionLotReceptionFilter
	{

		public int idProductionLot { get; set; }
		public string numberProductionLot { get; set; }

		public string internalNumberProductionLot { get; set; }

		public DateTime receptionDateProductionLot { get; set; }

		public string nameProductionUnit { get; set; }

		public string nameCertification { get; set; }
        
        public string nameProductionUnitProvider { get; set; }
		public int? id_personProcessPlant { get; set; }
		public string personProcessPlant { get; set; }
		public string fullnameBusinessNameProvider { get; set; }

		public string nameProductionLotState { get; set; }

		public string stateQuality { get; set; }

		public string liquidationNumber { get; set; }

		public int idProductionUnit { get; set; }
		public int idProductionUnitProvider { get; set; }
		public int idProvider { get; set; }
		public int idProcessType { get; set; }
		public string Process { get; set; }


	}
}