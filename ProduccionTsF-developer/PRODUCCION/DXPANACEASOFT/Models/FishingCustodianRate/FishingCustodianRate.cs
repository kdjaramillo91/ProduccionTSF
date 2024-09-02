using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class FishingCustodianRate
    {
        public int id { get; set; }
		public decimal patrol { get; set; }
		public decimal semiComplete { get; set; }
		public decimal truckDriver { get; set; }
		public decimal changeHG { get; set; }
		public decimal cabinHR { get; set; }
		public bool isActive { get; set; }
		public int id_userCreate { get; set; }
		public DateTime dateCreate { get; set; }
		public int? id_userUpdate { get; set; }
		public DateTime? dateUpdate { get; set; }
		public int id_fishingSite { get; set; }
	}
}