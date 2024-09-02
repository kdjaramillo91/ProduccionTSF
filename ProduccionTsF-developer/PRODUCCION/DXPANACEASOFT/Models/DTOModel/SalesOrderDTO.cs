using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class SalesOrderConsultDTO
	{
		public int? id_state { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public int? id_documentType { get; set; }
		public int? id_customer { get; set; }
		public int? id_seller { get; set; }
		public int[] items { get; set; }
		public int? id_Logistics { get; set; }

	}

	public class SalesOrderResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public string documentType { get; set; }
		public string customer { get; set; }
		public DateTime emissionDate { get; set; }
		public string numberProforma { get; set; }
		public DateTime? emissionDateProforma { get; set; }
		public string sellerProforma { get; set; }
        public bool logistics { get; set; }
        public string state { get; set; }

		public bool canEdit { get; set; }
		public bool canClosed { get; set; }
		public bool canAproved { get; set; }
        public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class SalesOrderDTO
	{
		public int id { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string code_documentType { get; set; }
		public string number { get; set; }
		public int idSate { get; set; }
		public string state { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public string reference { get; set; }
		public string employeeApplicant { get; set; }
		public int? id_employeeApplicant { get; set; }
		public string description { get; set; }
		public string customer { get; set; }
		public int? id_customer { get; set; }
		public string proformaOrder { get; set; }
		public string seller { get; set; }
		public int? id_seller { get; set; }
		public string paymentMethod { get; set; }
		public int? id_paymentMethod { get; set; }
		public DateTime dateShipment { get; set; }
		public string portDestination { get; set; }
		public int? id_portDestination { get; set; }
		public string portDischarge { get; set; }
		public int? id_portDischarge { get; set; }
		public bool logistics { get; set; }
		public string totalCM { get; set; }
		public string netoLbs { get; set; }
		public string netoKg { get; set; }
		public string dateHoy { get; set; }
		public string dateHoyMin { get; set; }
        public string numeroLote { get; set; }
        public int? id_orderReason { get; set; }
        public string orderReason { get; set; }

        public string Product { get; set; }
		public string ColourGrade { get; set; }
        public string PackingDetails { get; set; }
        public string ContainerDetails { get; set; }
        public int? id_provider { get; set; }
        public string Provider { get; set; }
        public string additionalInstructions { get; set; }
        public string shippingDocument { get; set; }

        public List<SalesOrderDetailDTO> SalesOrderDetails { get; set; }

        public List<SalesOrderMPMaterialDetailDTO> SalesOrderMPMaterialDetails { get; set; }

        public List<SalesOrderMPMaterialDetailSummaryDTO> SalesOrderMPMaterialDetailsSummary { get; set; }
    }
	public class SalesOrderDetailDTO
	{
		public int id { get; set; }
		public string noProgProduction { get; set; }
		public string noRequestProforma { get; set; }
		public int? id_salesRequestDetail { get; set; }
		public int? id_invoiceDetail { get; set; }
		public int id_item { get; set; }
		public string name_item { get; set; }
		public string description_item { get; set; }
		public string cod_item { get; set; }
		public string codAux_item { get; set; }
		public decimal cartons { get; set; }
		public decimal originQuantity { get; set; }
		public string originQuantityStr { get; set; }

		public decimal quantityProgrammed { get; set; }
		public decimal quantityApproved { get; set; }
		public decimal quantityProduced { get; set; }

        public List<SalesOrderDetailMPMaterialDetailDTO> SalesOrderDetailMPMaterialDetails { get; set; }

    }

    public class SalesOrderDetailMPMaterialDetailDTO
    {
        public int id { get; set; }
        public int id_salesOrderDetail { get; set; }
        public int id_salesOrderMPMaterialDetail { get; set; }
        public decimal quantity { get; set; }

    }

    public class SalesOrderMPMaterialDetailDTO
    {
        public int id { get; set; }
        //public string codProducts { get; set; }
        //public string nameProducts { get; set; }
        public string codProduct { get; set; }
        public int id_product { get; set; }
        public int id_inventoryLine { get; set; }
        public int id_itemType { get; set; }
        public int id_itemTypeCategory { get; set; }
        public string cod_item { get; set; }
        public string name_item { get; set; }
        public int id_item { get; set; }
        public decimal quantityRequiredForFormulation { get; set; }
        public decimal quantity { get; set; }
        public int? id_metricUnit { get; set; }
        public bool manual { get; set; }

    }

    public class SalesOrderMPMaterialDetailSummaryDTO
    {
        public int id { get; set; }
        public int id_inventoryLine { get; set; }
        public string cod_item { get; set; }
        public string name_item { get; set; }
        public int id_item { get; set; }
        public decimal quantityRequiredForFormulation { get; set; }
        public decimal quantity { get; set; }
        public int? id_metricUnit { get; set; }
    }

    public class SalesOrderPendingNewDTO
	{
		public string numberRequestProforma { get; set; }
		public int? id_salesRequestDetail { get; set; }
		public int? id_salesQuotationExterior { get; set; }
		public string customer { get; set; }
		public string emissionDateStr { get; set; }
		public DateTime? emissionDate { get; set; }
		public string name_item { get; set; }
		public decimal cartons { get; set; }
	}
}