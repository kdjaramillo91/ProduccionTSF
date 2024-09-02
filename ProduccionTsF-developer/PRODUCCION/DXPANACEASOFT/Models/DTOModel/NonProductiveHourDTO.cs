using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class NonProductiveHourConsultDTO
	{
		public int? id_state { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public string number { get; set; }
		public int? id_turn { get; set; }
		public int? id_machineForProd { get; set; }
	}

	public class NonProductiveHourResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public DateTime emissionDate { get; set; }
		public string turn { get; set; }
		public int id_machineForProd { get; set; }
		public string machineForProd { get; set; }
		public string processPlant { get; set; }
		public string state { get; set; }

		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class NonProductiveHourDTO
	{
		public int id { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public string description { get; set; }
		public int idSate { get; set; }
		public string state { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public int id_machineForProd { get; set; }
		public string machineForProd { get; set; }
		public int? id_personProcessPlant { get; set; }
		public string personProcessPlant { get; set; }
		public int id_turn { get; set; }
		public string turn { get; set; }
		public string timeInitTurn { get; set; }
		public string timeEndTurn { get; set; }
		public string hoursStop { get; set; }
		public string hoursProduction { get; set; }
		public string totalHours { get; set; }
        public int? numPerson { get; set; }

        public List<NonProductiveHourDetailDTO> NonProductiveHourDetails { get; set; }
	}
	public class NonProductiveHourDetailDTO
	{
		public int id { get; set; }
		public bool stop { get; set; }
		public int id_motiveLotProcessType { get; set; }
		public int id_motiveLot { get; set; }
		public string motiveLot { get; set; }
		public int? id_processType { get; set; }
		public DateTime startDate { get; set; }
		public TimeSpan startTime { get; set; }
		public DateTime endDate { get; set; }
		public TimeSpan endTime { get; set; }
		public string totalHours { get; set; }
		public string observation { get; set; }
		public int? id_RemisionGuide { get; set; }
		public string id_motiveLotProcessTypeGeneral { get; set; }
		public int? numPerson { get; set; }
    }
    public class NonProductiveHourPendingNewDTO
	{
		public string emissionDateStr { get; set; }
		public DateTime emissionDate { get; set; }
		public string numberMachineProdOpening { get; set; }
		public int id_turn { get; set; }
		public string turn { get; set; }
		public int id_machineForProd { get; set; }
		public string machineForProd { get; set; }
		public string processPlant { get; set; }
		public string personRequire { get; set; }
		public string state { get; set; }
	}
}