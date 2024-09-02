using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class RomaneoConsultDTO
	{
		public int? id_state { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public string number { get; set; }
		public string numberLote { get; set; }
		public string reference { get; set; }
		public string secTransaccional { get; set; }
		public int? id_typeRomaneo { get; set; }
	}

	public class RomaneoResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public string secTransaction { get; set; }
		public DateTime emissionDate { get; set; }
		public string numberLot { get; set; }
		public string provider { get; set; }
		public string nameProviderShrimp { get; set; }
		public string namePool { get; set; }
		public string nameItem { get; set; }
		public string typeRomaneo { get; set; }
		public string state { get; set; }

		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class RomaneoDTO
	{
		public int id { get; set; }
		public int idWeigher { get; set; }
		public string weigher { get; set; }
		public int idAnalist { get; set; }
		public string analist { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public string description { get; set; }
		public int idSate { get; set; }
		public string state { get; set; }
		public string dateTimeReception { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public List<RomaneoDetailDTO> RomaneoDetails { get; set; }
		//public ResultProdLotReceptionDetail ReceptionDetail { get; set; }
		public int idProductionLotReception { get; set; }
		public string numberLot { get; set; }
		public string numberLotSequential { get; set; }
		public decimal poundsRemitted { get; set; }
		public string nameProvider { get; set; }
		public string nameProviderShrimp { get; set; }
		public string INPnumber { get; set; }
		public string namePool { get; set; }
		public string nameWarehouse { get; set; }
		public string nameWarehouseLocation { get; set; }
		public string nameItem { get; set; }
		public string codeProcessType { get; set; }
		public int? idTypeRomaneo { get; set; }
		public string TypeRomaneo { get; set; }
		public int? idSize { get; set; }
		public string size { get; set; }
		public string codeTypeProcess { get; set; }
		public decimal poundsTrash { get; set; }
		public int drawersNumber { get; set; }
		public decimal totalPoundsGrossWeight { get; set; }
		public decimal percentTara { get; set; }
		public decimal totalPoundsNetWeight { get; set; }

	}
	//public class Romaneo
	//{

	//}
	public class RomaneoDetailDTO
	{
		public int id { get; set; }
		public int drawerNumber { get; set; }
		public int grossWeight { get; set; }
		public string um { get; set; }
		public int poundsTrash { get; set; }

	}
	public class RomaneoPendingNewDTO
	{
		public int idProductionLotReception { get; set; }
		public string number { get; set; }
		public string numberLot { get; set; }
		public string dateReception { get; set; }
		public string provider { get; set; }
		public string shrimper { get; set; }
		public string namePool { get; set; }
		public string item { get; set; }
		public decimal poundsRemitted { get; set; }
		public string metricUnit { get; set; }
	}
}