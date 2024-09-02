using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class HeadlessConsultDTO
	{
		public int? id_state { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public string number { get; set; }
		public int? id_turn { get; set; }
		public string numberLot { get; set; }
		public string secTransaction { get; set; }
		public string numberLotOrigin { get; set; }
    }

    public class HeadlessResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public DateTime emissionDate { get; set; }
		public string turn { get; set; }
        public string secTransaction { get; set; }
		public string numberLot { get; set; }
		public string process { get; set; }
		public string provider { get; set; }
		public string productionUnitProvider { get; set; }
		public string productionUnitProviderPool { get; set; }
        public string state { get; set; }

		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class HeadlessDTO
	{
		public int id { get; set; }
		public int id_productionLot { get; set; }
		public int? id_documentType { get; set; }
        public string documentType { get; set; }
		public string number { get; set; }
		public string description { get; set; }
		public int idSate { get; set; }
		public string state { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public DateTime dateTimeEmision { get; set; }
        public string provider { get; set; }
        public string productionUnitProvider { get; set; }
        public string productionUnitProviderPool { get; set; }
        public string numberLot { get; set; }
        public string secTransaction { get; set; }
		public string process { get; set; }
		public string receptionDateStr { get; set; }
		public string receptionTimeStr { get; set; }
		public DateTime receptionDate { get; set; }
		public decimal poundsRemitted { get; set; }
        public int? id_turn { get; set; }
        public string timeInitTurn { get; set; }
        public string timeEndTurn { get; set; }
        public int? id_programmer { get; set; }
        public string programmer { get; set; }
        public int? id_supervisor { get; set; }
        public string supervisor { get; set; }
        public DateTime? dateStartTime { get; set; }
        public DateTime? endDateTime { get; set; }
        public int? noOfPeople { get; set; }
        public decimal? grammage { get; set; }
        public int? id_color { get; set; }
        public decimal? manualPerformance { get; set; }
        public int? noOfDrawers { get; set; }
		public decimal? lbsWholeSurplus { get; set; }
		public decimal? lbsDirect { get; set; }
		public bool isWholeShrimp { get; set; }
        
    }
    public class HeadlessPendingNewDTO
	{
		public int id_productionLot { get; set; }
        public string numberLot { get; set; }
        public string secTransaction { get; set; }
        public string receptionDateStr { get; set; }
        public DateTime receptionDate { get; set; }
		public decimal poundsRemitted { get; set; }
        public string process { get; set; }
        public string provider { get; set; }
        public string productionUnitProvider { get; set; }
        public string productionUnitProviderPool { get; set; }
        public string state { get; set; }
    }
}