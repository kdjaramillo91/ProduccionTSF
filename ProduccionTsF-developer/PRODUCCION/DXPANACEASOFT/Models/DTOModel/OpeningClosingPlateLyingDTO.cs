using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class OpeningClosingPlateLyingConsultDTO
	{
		public int? id_state { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public int? id_responsable { get; set; }
		public int? id_freezerWarehouse { get; set; }
		public int? id_freezerWarehouseLocation { get; set; }
		public int? id_boxedWarehouse { get; set; }
		public int? id_boxedWarehouseLocation { get; set; }
		public string numberLot { get; set; }
		public string secTransLot { get; set; }
		public int[] items { get; set; }
	}

	public class OpeningClosingPlateLyingResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public DateTime emissionDate { get; set; }
		public string responsable { get; set; }
        public string turn { get; set; }
        public string warehouse { get; set; }
        public bool tunnelTransferPlate { get; set; }
        public string freezerMachineForProd { get; set; }
		public string freezerWarehouse { get; set; }
		public decimal selectedQuantity { get; set; }
		public string state { get; set; }

		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class OpeningClosingPlateLyingDTO
	{
		public int id { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public int idSate { get; set; }
		public string state { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public string description { get; set; }
		public string reference { get; set; }
		public string responsable { get; set; }
		public int? id_responsable { get; set; }
		public string department { get; set; }
		public string freezerWarehouse { get; set; }
		public int? id_freezerWarehouse { get; set; }
		public string ids_freezerWarehouseLocation { get; set; }
		public string ids_productionCart { get; set; }
		public string ids_item { get; set; }
		public string ids_lot { get; set; }
        public int? id_company { get; set; }
		public decimal selectedQuantity { get; set; }
		public string selectedQuantityStr { get; set; }
		public string dateHoy { get; set; }
		public string dateHoyMin { get; set; }
		public DateTime? dateTimeStartLying { get; set; }
		public DateTime? dateTimeEndLying { get; set; }
		public decimal? temperature { get; set; }
		public string freezerMachineForProd { get; set; }
		public int? id_freezerMachineForProd { get; set; }
        public string freezerMachineForProdDestination { get; set; }
        public int? id_freezerMachineForProdDestination { get; set; }
        public string turn { get; set; }
        public int? id_turn { get; set; }
        public string timeInitTurn { get; set; }
        public string timeEndTurn { get; set; }
        public bool tunnelTransferPlate { get; set; }

		public int? id_warehouseDestiny { get; set; }
		public string name_warehouseDestiny { get; set; }
		public int? id_warehouseLocationDestiny { get; set; }
		public string name_warehouseLocationDestiny { get; set; }

		public List<OpeningClosingPlateLyingDetailDTO> OpeningClosingPlateLyingDetails { get; set; }
	}
	public class OpeningClosingPlateLyingDetailDTO
	{
		public int id { get; set; }
		public int? id_lot { get; set; }
		public string noSecTransLote { get; set; }
		public string noLote { get; set; }
		public int id_item { get; set; }
		public string name_item { get; set; }
		public int id_warehouse { get; set; }
		public string warehouse { get; set; }
		public int? id_warehouseLocation { get; set; }
		public string warehouseLocation { get; set; }
		public int? id_costCenterExit { get; set; }
		public int? id_subCostCenterExit { get; set; }
		public int? id_productionCart { get; set; }
		public string productionCart { get; set; }
		public decimal amount { get; set; }
		public int id_metricUnit { get; set; }
		public string metricUnit { get; set; }
		public int? id_boxedWarehouse { get; set; }
		public string boxedWarehouse { get; set; }
		public int? id_boxedWarehouseLocation { get; set; }
		public string boxedWarehouseLocation { get; set; }
		public int? id_costCenter { get; set; }
		public int? id_subCostCenter { get; set; }
		public string numberInventoryExit { get; set; }
		public string numberInventoryEntry { get; set; }
        public string cliente { get; set; }
    }

}