using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class QualityControlPendingNewDTO
    {
		public int id { get; set; }
		public string number { get; set; }
        public string internalNumber { get; set; }
		public DateTime receptionDate { get; set; }
		public string remissionGuideNumber { get; set; }
		public string remissionGuideNumberExterna { get; set; }
		public string remissionGuideProcess { get; set; }
		public string proveedor { get; set; }
		public string productionUnitProvider { get; set; }
		public string name_item { get; set; }
		public decimal quantityRecived { get; set; }
		public decimal? quantitydrained { get; set; }
		public string um { get; set; }
	}

    public class QualityControlResultConsultDTO
    {
        public int id { get; set; }
        public string number { get; set; }
        public string analysisType { get; set; }
        public DateTime dateAnalysis { get; set; }
        public string internalNumber { get; set; }
        public string remissionGuideNumber { get; set; }
        public string remissionGuideProcess { get; set; }
        public string proveedor { get; set; }
        public string processType { get; set; }
        public decimal? quantityPoundsReceived { get; set; }
        public string analyst { get; set; }
        public bool isConforms { get; set; }
        public string isConformsStr { get; set; }
        public string documentState { get; set; }

        //public bool canEdit { get; set; }
        //public bool canAproved { get; set; }
        //public bool canReverse { get; set; }
        //public bool canAnnul { get; set; }
    }
}