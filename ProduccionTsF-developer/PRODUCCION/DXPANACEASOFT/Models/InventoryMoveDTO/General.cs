
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.InventoryMoveDTO
{
	public partial class InventoryMoveDTO
	{
		public int id { get; set; }
		public string natureSequential { get; set; }
		public Nullable<int> idWarehouse { get; set; }
		public string nameWarehouse { get; set; }
		public System.DateTime emissionDate { get; set; }
		public Nullable<int> id_inventoryReason { get; set; }
		public string nameInventoryReason { get; set; }
		public int? id_receiver { get; set; }
		public string nameReceiver { get; set; }
		public int? id_dispatcher { get; set; }
		public string nameDispatcher { get; set; }
		public int id_documentState { get; set; }
		public string nameDocumentState { get; set; }
        public int id_documentType { get; set; }
        public string code_documentType { get; set; }
	}
	public class ParamForQueryInvMoveDetail
    {
        public string str_item { get; set; }
        public string emissiondate { get; set; }
        public string houremissiondate { get; set; }

    }

    public class ParamModelInveMvDetailBulk
    {
        public int id_item { get; set; }
        public int id_warehouse { get; set; }
        public int id_warehouselocation { get; set; }
    }

    public class ItemInvMoveDetail 
    {
        public int id_item { get; set; }
        public int Id_warehouse { get; set; }
        public int Id_warehouselocation { get; set; }         
        public int id_inventorymovedetail { get; set; }
    }

    public class ItemInventoryTransferP
    {
        public int idItemTransferP { get; set; }
        public int idWaresHouseTransferP { get; set; }
        public int idWarehouseLocationTransferP { get; set; }

        public int idMetricUnitInventoryTransferP { get; set; }

        public string codeMetricUnitInventoryTransferP { get; set; }

        public string nameMetricUnitInventoryTransferP { get; set; }
    }
    public class SequentialPar
    {
        public string sSequential { get; set; }
            public int iSequential { get; set; }
    }

    public class ItemInvMoveDetailLot
    {
        public int id_item { get; set; }
        public int? id_lot { get; set; }
        public decimal saldo { get; set; }
        public string number { get; set; }
    }
}