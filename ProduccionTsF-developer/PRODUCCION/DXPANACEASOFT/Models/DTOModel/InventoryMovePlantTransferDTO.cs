using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class InventoryMovePlantTransferConsultDTO
	{
		public int? id_state { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public int? id_warehouseEntry { get; set; }
		public int? id_warehouseLocationEntry { get; set; }
		public int? id_inventoryReason { get; set; }
		public int? id_receiver { get; set; }
		public string numberLot { get; set; }
		public string secTransaction { get; set; }
		public int? id_processType { get; set; }
		public int? id_provider { get; set; }
		public int? id_machineForProd { get; set; }
		public int? id_turn { get; set; }
		public int? id_productionCart { get; set; }
	}

	public class InventoryMovePlantTransferResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public int? id_warehouse { get; set; }
		public string warehouse { get; set; }
		public int? id_warehouseLocation { get; set; }
		public string warehouseLocation { get; set; }
		public DateTime emissionDate { get; set; }
		public int? id_inventoryReason { get; set; }
		public string inventoryReason { get; set; }
		public int? id_receiver { get; set; }
		public string receiver { get; set; }
		public string state { get; set; }
		public int id_machineForProdEntry { get; set; }
		public int id_machineForProdExit { get; set; }

		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class InventoryMovePlantTransferDTO
	{
		public int id { get; set; }
		public int? id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public string description { get; set; }
		public int idSate { get; set; }
		public string state { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public int id_machineForProdCartOnCart { get; set; }
		public string machineForProdCartOnCart { get; set; }
		public string numberLiquidationCartOnCart { get; set; }
		//public int? id_productionCart { get; set; }
		//public string productionCart { get; set; }
		public string processType { get; set; }
		public string processPlant { get; set; }
		public string liquidator { get; set; }
		public int? id_machineProdOpeningDetail { get; set; }
		public int? id_machineForProdCogellingFresh { get; set; }
		public string machineForProdCogellingFresh { get; set; }
		public int? id_turnCogellingFresh { get; set; }
		public string turnCogellingFresh { get; set; }
		public string timeInitTurn { get; set; }
		public string timeEndTurn { get; set; }
		public string dateTimeEntryStr { get; set; }
		public DateTime dateTimeEntry { get; set; }
		public int? id_natureMove { get; set; }
		public string natureMove { get; set; }
		public int? id_inventoryReason { get; set; }
		public string inventoryReason { get; set; }
		public int? id_receiver { get; set; }
		public string receiver { get; set; }
		public int? id_warehouseEntry { get; set; }
		public string warehouseEntry { get; set; }
		public bool isCopackingLot { get; set; }

		public List<InventoryMovePlantTransferDetailDTO> InventoryMovePlantTransferDetails { get; set; }
	}
	public class InventoryMovePlantTransferDetailDTO
	{
		public int id { get; set; }
		public int? id_liquidationCartOnCartDetail { get; set; }
		public int id_liquidationCartOnCart { get; set; }
		public int? id_inventoryMoveExit { get; set; }
		public string noInventoryMoveExit { get; set; }
		public int? id_warehouseExit { get; set; }
		public string warehouseExit { get; set; }
		public int? id_warehouseLocationExit { get; set; }
		public string warehouseLocationExit { get; set; }
		public int? id_warehouseEntry { get; set; }
		public string warehouseEntry { get; set; }
		public int? id_warehouseLocationEntry { get; set; }
		public string warehouseLocationEntry { get; set; }
        public int? id_productionCart { get; set; }
        public string productionCart { get; set; }
        public int id_item { get; set; }
		public string codItem { get; set; }
		public string nameItem { get; set; }
        public int? id_customer { get; set; }
        public string customer { get; set; }
        public int id_umMovExit { get; set; }
		public string umMovExit { get; set; }
		public int? id_costCenter { get; set; }
		public string costCenter { get; set; }
		public int? id_subCostCenter { get; set; }
		public string subCostCenter { get; set; }
		public decimal amountToEnter { get; set; }
		public int id_umMov { get; set; }
		public string umMov { get; set; }
		public decimal cost { get; set; }
		public decimal total { get; set; }
		public int? id_lot { get; set; }
		public string lot { get; set; }
		public decimal pending { get; set; }
		public int? id_costCenterEntry { get; set; }
		public string costCenterEntry { get; set; }
		public int? id_subCostCenterEntry { get; set; }
		public string subCostCenterEntry { get; set; }
		//public decimal quantityKgs { get; set; }
		//public decimal quantityPounds { get; set; }
		public string machineForProdCartOnCartDetail { get; set; }
		
	}
	public class InventoryMovePlantTransferPendingNewDTO
	{
		public int id_liquidationCartOnCartDetail { get; set; }
		public string numberLiquidationCartOnCart { get; set; }
		public string emissionDateStr { get; set; }
		public DateTime emissionDate { get; set; }
		public int id_machineForProd { get; set; }
		public string machineForProd { get; set; }
		public int id_productionCart { get; set; }
		public string productionCart { get; set; }
		public string turn { get; set; }
        public int id_processType { get; set; }
		public string processType { get; set; }
		public string numberLot { get; set; }
		public string processPlant { get; set; }
		public string provider { get; set; }
        public int? id_customer { get; set; }
        public string customer { get; set; }
        public string itemWarehouse { get; set; }
		public decimal box { get; set; }    
    }
}