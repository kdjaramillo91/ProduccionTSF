using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class LiquidationTurnConsultDTO
	{
		public int? id_state { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public string number { get; set; }
		public int? id_turn { get; set; }
		public string numberLot { get; set; }
		public int? id_provider { get; set; }
		public int? id_productionUnitProvider { get; set; }
	}

	public class LiquidationTurnResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public DateTime emissionDate { get; set; }
		public string turn { get; set; }
		public string processPlant { get; set; }
		public string state { get; set; }

		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class LiquidationTurnDTO
	{
		public int id { get; set; }
		public int id_turn { get; set; }
		public int id_personProcessPlant { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public string description { get; set; }
		public int idSate { get; set; }
		public string state { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public DateTime liquidationDate { get; set; }
		public TimeSpan liquidationTime { get; set; }
		public List<LiquidationTurnDetailDTO> LiquidationTurnDetails { get; set; }
	}
	public class LiquidationTurnDetailDTO
	{
		public int id { get; set; }
		public string turn { get; set; }
		public string numberLot { get; set; }
		public string process { get; set; }
		public string provider { get; set; }
		public string numberLiquidationCarOnCar { get; set; }
		public string machineForProd { get; set; }
		public string cod_state { get; set; }
		public string state { get; set; }
		public string cod_stateLote { get; set; }
		public string stateLote { get; set; }
		public decimal tail { get; set; }
		public decimal whole { get; set; }
		public decimal total { get; set; }
	}
	public class LiquidationTurnPendingNewDTO
	{
		public string emissionDateStr { get; set; }
		public DateTime emissionDate { get; set; }
		public int id_turn { get; set; }
		public string turn { get; set; }
		public int? id_personProcessPlant { get; set; }
		public string processPlant { get; set; }
	}
}