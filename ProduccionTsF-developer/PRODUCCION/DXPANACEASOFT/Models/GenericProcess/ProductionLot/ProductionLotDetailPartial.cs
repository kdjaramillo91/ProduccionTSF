using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
	public partial class ProductionLotDetail
	{

		public Boolean quantityRecivedEditing { get; set; }
		public int? id_purchaseOrder { get; set; }
		public int? id_remissionGuide { get; set; }
		public string process { get; set; }
		public string metricUnitPO { get; set; }


	}
}