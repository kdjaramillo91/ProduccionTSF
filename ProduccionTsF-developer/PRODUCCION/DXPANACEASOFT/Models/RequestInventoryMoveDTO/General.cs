
using DXPANACEASOFT.Models.DocumentP.DocumentDTO;
using DXPANACEASOFT.Models.RequestInventoryMoveModel;
using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.RequestInventoryMoveDTO
{
	public class ParamForQueryInvMoveDetail
	{
		public string str_item { get; set; }
		public string emissiondate { get; set; }
		public string houremissiondate { get; set; }

	}
	public class ItemInvMoveDetail
	{
		public int id_item { get; set; }
		public int id_inventorymovedetail { get; set; }

	}
	public class FilterQueryRequestInventoryMove
	{
		public int? idNatureMove { get; set; }
		public int? idDocumentState { get; set; }
		public int? idWarehouse { get; set; }
		public int? idPersonRequest { get; set; }

		public DateTime? startEmissionDate { get; set; }
		public DateTime? endEmissionDate { get; set; }
	}

	public class ResultRequestInventoryMoveAll
	{
		public int id { get; set; }

		public DateTime dateEmissionR { get; set; }

		public int sequentialR { get; set; }

		public string nameSequentialR { get; set; }
		public int? idWarehouseR { get; set; }
		public string descWarehouseR { get; set; }

		public int idPersonRequestR { get; set; }
		public string namePersonRequestR { get; set; }

		public int idDocumentStateR { get; set; }
		public string nameDocumentStateR { get; set; }

		public int idNatureMoveR { get; set; }

		public string nameNatureMoveR { get; set; }

		public string sequentialRemissionGuide { get; set; }

		public int? idRemGuiDispatchMaterial { get; set; }

		public string sequentialDispatchMaterial { get; set; }

	}

	#region REQUEST INVENTORY MOVE FILTER
	public class NatureMoveRIMFilter
	{
		public int idNatureF { get; set; }
		public string descNatureF { get; set; }
	}

	public class WarehouseRIMFilter
	{
		public int idWarehouseF { get; set; }
		public string descWarehouseF { get; set; }
	}

	public class PersonRequestRIMFilter
	{
		public int idPersonRequestF { get; set; }
		public string descPersonRequestF { get; set; }
	}

	#endregion

	#region REQUEST INVENTORY MOVE DTO
	public class RequestInventoryMoveTransferP
	{
		public int idRIMTransferP { get; set; }
		public int idPersonRTransferP { get; set; }

		public string namePersonRTransferp { get; set; }

		public int? idWarehouseTransferP { get; set; }

		public string nameWarehouseTransferP { get; set; }
		public int? idProviderTransferP { get; set; }

		public string nameProviderTransferP { get; set; }
		public int? idCustomerTransferP { get; set; }

		public string nameCustomerTransferP { get; set; }
		public int idNatureMoveTransferP { get; set; }
		public string nameNatureMoveTransferP { get; set; }

		public int? id_remissionguide_origin { get; set; }

		public string sequential_remissionGuide { get; set; }

		public bool ReadOnlyBeRemissionGuide { get; set; }

		public DocumentTansferP documentRequestTransferP { get; set; }
		public List<RequestInventoryMoveDetailTransferP> lstRequestInvDetail { get; set; }

		public List<RequestInventoryMoveDocsFromApprovedIM> lstReqAppDocsIM { get; set; }

		public string numberRequerimentTransferP { get; set; }

		public string nameDocumenTypeHeadP { get; set; }

		public int idDocumentTypeHeadP { get; set; }

		public DateTime? emissionDateHeadPOrigin { get; set; }

		public DateTime? emissionDateRemissionGuide { get; set; }

		public string sequentialInventoryMoveHeadP { get; set; }

		public string sequentialWarehouseRequisitionModelHeadP { get; set; }

		public string nameProviderRGTransferP { get; set; }
	}

	public class RequestInventoryMoveTransferPUpdateEntity
	{
		public int? id_Rim { get; set; }
		public int? yEmissionDate { get; set; }

		public int? mEmissionDate { get; set; }

		public int? dEmissionDate { get; set; }

		public int? hoursEmissionDate { get; set; }

		public int? minutesEmissionDate { get; set; }

		public int? id_PersonRequest { get; set; }

		public int? id_Warehouse { get; set; }

		public int? id_NatureMove { get; set; }

		public DateTime emissionDateDoc { get; set; }
	}

	public class RequestInventoryMoveDetailTransferP
	{
		public int id { get; set; }
		public int id_item { get; set; }
		public string master_code_item { get; set; }

		public string aux_code_item { get; set; }

		public string name_item { get; set; }

		public decimal quantityRequest { get; set; }

		public string codeMetricUnit { get; set; }

		public string nameMetricUnit { get; set; }

		public int? idWarehouseLocation { get; set; }

		public string nameWarehouseLocation { get; set; }

		public decimal? quantityUpdate { get; set; }

		public bool isActive { get; set; }
	}
	public class RequestInventoryMoveDocsFromApprovedIM
	{
		public int idDocumentDGModelP { get; set; }
		public string sequentialDGModelP { get; set; }
		public string nameNatureMove { get; set; }
		public string nameInventoryReasonDGModelP { get; set; }
	}

	public class RequestInventoryMoveEditForm
	{
		public RequestInventoryMoveTransferP rqTransfer { get; set; }

		public RequestInventoryMoveModelP rqModel { get; set; }
	}

	#endregion
}