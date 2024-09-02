using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
	public partial class QualityControl
	{
		public string remissionGuideNumber { get; set; }

		public string remissionGuideProcess { get; set; }

		public string poolName { get; set; }

		public bool? hasUpd { get; set; }
		public string remissionGuideExternalGuide { get; set; }
	}
}