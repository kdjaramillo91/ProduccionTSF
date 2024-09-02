using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class MasteredConsultDTO
	{
		public int? id_state { get; set; }
		public string number { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public int? id_responsable { get; set; }
		public int? id_turn { get; set; }
        public int? id_boxedWarehouse { get; set; }
        public int? id_boxedWarehouseLocation { get; set; }
		public int[] boxedItems { get; set; }
		public string boxedNumberLot { get; set; }
        public int? id_masteredWarehouse { get; set; }
        public int? id_masteredWarehouseLocation { get; set; }
		public int[] masteredItems { get; set; }
		public string masteredNumberLot { get; set; }
	}

	public class MasteredResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public DateTime emissionDate { get; set; }
		public string responsable { get; set; }
        public string turn { get; set; }
        public string boxedWarehouse { get; set; }
		public string masteredWarehouse { get; set; }
		public string state { get; set; }

		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class MasteredDTO
	{
        //Documento
		public int id { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public DateTime dateTimeEmision { get; set; }
        public int idSate { get; set; }
        public string state { get; set; }
        public string turn { get; set; }
        public int? id_turn { get; set; }
        public string responsable { get; set; }
        public int? id_responsable { get; set; }
        public DateTime? dateTimeStartMastered { get; set; }
        public DateTime? dateTimeEndMastered { get; set; }
        public string description { get; set; }

        //Cabecera
        public string boxedWarehouse { get; set; }
        public int? id_boxedWarehouse { get; set; }
        public string boxedWarehouseLocation { get; set; }
        public int? id_boxedWarehouseLocation { get; set; }
        public string masteredWarehouse { get; set; }
        public int? id_masteredWarehouse { get; set; }
        public string masteredWarehouseLocation { get; set; }
        public int? id_masteredWarehouseLocation { get; set; }
        //public string ids_masteredWarehouseLocation { get; set; }
        public string warehouseBoxes { get; set; }
        public int? id_warehouseBoxes { get; set; }
        public string warehouseLocationBoxes { get; set; }
        public int? id_warehouseLocationBoxes { get; set; }
        //public string ids_warehouseLocationBoxes { get; set; }
        public int? id_company { get; set; }
        public string numberExitBoxed { get; set; }
        public string numberEntryMastered { get; set; }
        public string numberEntryBoxes { get; set; }

        //Summary
        public decimal amountMP { get; set; }
        public string amountMPStr { get; set; }
        public decimal amountPT { get; set; }
        public string amountPTStr { get; set; }
        public decimal amountBoxes { get; set; }
        public string amountBoxesStr { get; set; }
        public decimal lbsMP { get; set; }
        public string lbsMPStr { get; set; }
        public decimal lbsPT { get; set; }
        public string lbsPTStr { get; set; }
        public decimal lbsBoxes { get; set; }
        public string lbsBoxesStr { get; set; }
        public decimal kgMP { get; set; }
        public string kgMPStr { get; set; }
        public decimal kgPT { get; set; }
        public string kgPTStr { get; set; }
        public decimal kgBoxes { get; set; }
        public string kgBoxesStr { get; set; }
        public string dateHoy { get; set; }
        public string dateHoyMin { get; set; }
        public string timeInitTurn { get; set; }
        public string timeEndTurn { get; set; }

        //      public string department { get; set; }

        //public string ids_productionCart { get; set; }
        //public string ids_item { get; set; }
        //public decimal selectedQuantity { get; set; }
        //public string selectedQuantityStr { get; set; }


        //public decimal? temperature { get; set; }
        //public string freezerMachineForProd { get; set; }
        //public int? id_freezerMachineForProd { get; set; }
        //      public string freezerMachineForProdDestination { get; set; }
        //      public int? id_freezerMachineForProdDestination { get; set; }


        //      public bool tunnelTransferPlate { get; set; }

        public List<MasteredDetailDTO> MasteredDetails { get; set; }
	}
	public class MasteredDetailDTO
	{
		public int id { get; set; }
		public int? id_sales { get; set; }
        //MP-Boxed
        public string codProductMP { get; set; }
        public string id_productLotMP { get; set; }
        public int id_productMP { get; set; }
        public int? id_lotMP { get; set; }
        public string loteMP { get; set; }
        public decimal saldoMP { get; set; }
        public decimal quantityMP { get; set; }
        public int? id_boxedWarehouse { get; set; }
        public string boxedWarehouse { get; set; }
        public int? id_boxedWarehouseLocation { get; set; }
        public string boxedWarehouseLocation { get; set; }
        public int? id_costCenterExitBoxed { get; set; }
        public int? id_subCostCenterExitBoxed { get; set; }
        public int id_metricUnitBoxed { get; set; }
        public string metricUnitBoxed { get; set; }
        //PT-Mastered
        public string codProductPT { get; set; }
        public int id_productPT { get; set; }
        public int? id_customer { get; set; }
        public decimal quantityPT { get; set; }
        public int? id_masteredWarehouse { get; set; }
        public string masteredWarehouse { get; set; }
        public int? id_masteredWarehouseLocation { get; set; }
        public string masteredWarehouseLocation { get; set; }
        public int? id_costCenterEntryMastered { get; set; }
        public int? id_subCostCenterEntryMastered { get; set; }
        public int id_metricUnitMastered { get; set; }
        public string metricUnitMastered { get; set; }
        //Cajas-Boxes
        public int? id_lotBoxes { get; set; }
        public string loteBoxes { get; set; }
        public decimal quantityBoxes { get; set; }
        public int? id_warehouseBoxes { get; set; }
        public string warehouseBoxes { get; set; }
        public int? id_warehouseLocationBoxes { get; set; }
        public string warehouseLocationBoxes { get; set; }
        public int? id_costCenterEntryBoxes { get; set; }
        public int? id_subCostCenterEntryBoxes { get; set; }
        public string lotMarked { get; set; }


        //      public string noSecTransLote { get; set; }
        //public int id_item { get; set; }
        //public string name_item { get; set; }

        //public string numberInventoryExit { get; set; }
        //public string numberInventoryEntry { get; set; }

    }

}