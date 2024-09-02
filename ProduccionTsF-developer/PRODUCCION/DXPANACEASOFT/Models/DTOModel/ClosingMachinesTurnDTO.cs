using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class ClosingMachinesTurnConsultDTO
	{
		public int? id_state { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public string number { get; set; }
		public int? id_turn { get; set; }
		public int? id_machineForProd { get; set; }
		public int? id_person { get; set; }
		public string numberLot { get; set; }
		public int? id_provider { get; set; }
		public int? id_productionUnitProvider { get; set; }
	}

	public class ClosingMachinesTurnResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public DateTime emissionDate { get; set; }
		public int id_machineForProd { get; set; }
		public string machineForProd { get; set; }
		public string plantProcess { get; set; }
		public string turn { get; set; }
		public string state { get; set; }

		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class ClosingMachinesTurnDTO
	{
		public int id { get; set; }
		public int id_machineProdOpeningDetail { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public string description { get; set; }
		public int idSate { get; set; }
		public string state { get; set; }
		public int id_machineForProd { get; set; }
		public string machineForProd { get; set; }
		public int id_turn { get; set; }
		public string turn { get; set; }
		public int noOfPerson { get; set; }
		public decimal poundsRemitted { get; set; }
		public int noOfBox { get; set; }
		public int idPerson { get; set; }
		public string person { get; set; }
		public decimal poundsProcessed { get; set; }
		public decimal poundsTailProcessed { get; set; }
		public decimal poundsWholeProcessed { get; set; }
		public decimal poundsCooling { get; set; }
		public decimal poundsTailCooling { get; set; }
		public decimal poundsWholeCooling { get; set; }

		public List<ClosingMachinesTurnDetailDTO> ClosingMachinesTurnDetails { get; set; }
	}
	public class ClosingMachinesTurnDetailDTO
	{
		public int id_liquidationCartOnCart { get; set; }
		public string numberLiquidationCartOnCart { get; set; }
		public string provider { get; set; }
		public string nameProviderShrimp { get; set; }
		public string productionUnitProviderPool { get; set; }
		public decimal weight { get; set; }
		public string proccesType { get; set; }
		public string numberLot { get; set; }
		public string plantProcess { get; set; }
		public decimal poundsRemitted { get; set; }
		public decimal poundsProcessed { get; set; }
		public decimal poundsCooling { get; set; }
		public int noOfBox { get; set; }
		public string cod_state { get; set; }
		public string state { get; set; }
		public string nameliquidator { get; set; }

	}
	public class ClosingMachinesTurnPendingNewDTO
	{
		public int id_MachineProdOpeningDetail { get; set; }
		public string numberMachineProdOpening { get; set; }
		public string plantProcess { get; set; }
		public string machineForProd { get; set; }
		public string emissionDateStr { get; set; }
		public DateTime emissionDate { get; set; }
		public int id_turn { get; set; }
		public string turn { get; set; }
		public TimeSpan timeInit { get; set; }
		public TimeSpan? timeEnd { get; set; }
		public string state { get; set; }
	}
}