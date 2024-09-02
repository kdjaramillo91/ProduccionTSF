using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class ProductBalanceQueryResult 
	{
		public string masterCode { get; set; }
		public string Message { get; set; }
		public decimal remainingBalance { get; set; }
		public decimal unitPriceMove { get; set; }
		public decimal averagePrice { get; set; }
	}

	public class AutomaticTransferResultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public DateTime dateEmission { get; set; }
		public string warehouseExit { get; set; }
		public string warehouseLocationExit { get; set; }
		public string numberExit { get; set; }
		public string warehouseEntry { get; set; }
		public string warehouseLocationEntry { get; set; }
		public string numberEntry { get; set; }
		public string dispatcher { get; set; }
		public string stateDocument { get; set; }
		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}
	public class AutomaticTransferFilterDTO 
	{
		public int? id_StateDocument { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public string dateEmissionFrom { get; set; }
		public string dateEmissionTo { get; set; }
		public int? id_InventoryReasonExit { get; set; }
		public int? id_WarehouseExit { get; set; }
		public int? id_WarehouseLocationExit { get; set; }
		public int? id_InventoryReasonEntry { get; set; }
		public int? id_WarehouseEntry { get; set; }
		public int? id_WarehouseLocationEntry { get; set; }
		public int? id_Dispatcher { get; set; }
		public int? id_Item { get; set; }
	}
    public class AutomaticTransferDTO
    {
		public int id { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public string dateHoy { get; set; }
		public string dateHoyMin { get; set; }
		public int idState { get; set; }
		public string state { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public string description { get; set; }
		public int? id_InventoryMoveExit { get; set; }
		public int? id_InventoryMoveEntry { get; set; }
		public int? id_WarehouseExit { get; set; }
		public int? id_WarehouseLocationExit { get; set; }
		public int? id_InventoryReasonExit { get; set; }
		public int? id_CostCenterExit { get; set; }
		public int? id_SubCostCenterExit { get; set; }
		public string code_SubCostCenterExit { get; set; }
		public string RequerimentNumber { get; set; }
		public int? id_Despachador { get; set; }
		public int? id_WarehouseEntry { get; set; }
		public int? id_WarehouseLocationEntry { get; set; }
		public int? id_InventoryReasonEntry { get; set; }
		public int? id_CostCenterEntry { get; set; }
		public int? id_SubCostCenterEntry { get; set; }
		public string code_SubCostCenterEntry { get; set; }
		public string ExitNumberMove { get; set; }
		public string EntryNumberMove { get; set; }
		public int? idProcessPlantExit { get; set; }
		public int? idProcessPlantEntry { get; set; }
		public List<AutomaticTransferDetailDTO> lsDetail { get; set; }
		public AutomaticTransferDTO()
		{
			lsDetail = new List<AutomaticTransferDetailDTO>();
		}
	}
	public class AutomaticTransferDetailDTO
	{
		public int id { get; set; }
		public int? id_Item { get; set; }
		public string str_ItemName { get; set; }
		public int? id_MetricUnitInv { get; set; }
		public string str_MetricUnitInv { get; set; }
		public int? id_MetricUnitMov { get; set; }
		public string str_MetricUnitMov { get; set; }
		public decimal quantity { get; set; }
		public string strQuantity { get; set; }
		public decimal cost { get; set; }
		public string strCost { get; set; }
		public int? id_lot { get; set; }
		public string numero_lote { get; set; }		
		public decimal? saldo { get; set; }
		public string strSaldo { get; set; }
		public decimal? total { get; set; }
		public string strTotal { get; set; }
		public int? id_warehouse_exit { get; set; }
		public int? id_warehouselocation_exit { get; set; }
		public int? id_warehouse_entry { get; set; }
		public int? id_warehouselocation_entry { get; set; }
		public string requiereUsuarioLote { get; set; }
		public string requiereSistemaLote { get; set; }
        public AutomaticTransferDetailDTO()
		{
			this.id = 0;
		}
	}
}