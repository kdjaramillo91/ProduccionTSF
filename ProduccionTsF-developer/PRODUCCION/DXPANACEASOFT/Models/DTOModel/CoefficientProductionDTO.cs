using System;
using System.Collections.Generic;

// TODO: Eliminar una vez que se haya removido todas las pantallas que los usan

namespace DXPANACEASOFT.Models.DTOModel
{
	public class ModifiedValueCostAllocationDetail
	{
		public int idAllocationDetail { get; set; }
		public decimal value { get; set; }
		public string strvalue { get; set; }
	}
	public class PoundsDistributionGroupedByPlant
	{
		public string nameProcessPlant { get; set; }
		public decimal pounds { get; set; }
		public decimal percentageP { get; set; }
		public decimal value { get; set; }
	}
	public class CoefficientProductionDTO
	{
		public int? id_state { get; set; }
		public string initDate { get; set; }
		public string endDate { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public int? year { get; set; }
		public int? month { get; set; }
		public string id_assignmentType { get; set; }

	}
	public class CoefficientProductionResultsDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public DateTime emissionDate { get; set; }
		public int? year { get; set; }
		public int? month { get; set; }
		public DateTime? dateFrom { get; set; }
		public DateTime? dateTo { get; set; }
		public string assignmentType { get; set; }
		public string state { get; set; }
		public string Valorizado { get; set; }
		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class ProductionCoefficientDTO
	{
		//Documento
		public int id { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public int? idSate { get; set; }
		public string state { get; set; }
		public string description { get; set; }
		public int? id_company { get; set; }
		public string id_assignmentType { get; set; }
		public string assignmentType { get; set; }
		public bool poundsRemitted { get; set; }
		public string year { get; set; }
		public string month { get; set; }
		public string dateTimeInvPeriodStartStr { get; set; }
		public DateTime dateTimeInvPeriodStart { get; set; }
		public string dateTimeInvPeriodEndStr { get; set; }
		public DateTime dateTimeInvPeriodEnd { get; set; }
		public int? id_warehousetype { get; set; }
		public string warehousetype { get; set; }
		public string dateHoy { get; set; }
		public string dateHoyMin { get; set; }
		public string nameProductionCost { get; set; }
		public string typeCoefficientCalculus { get; set; }
		public List<ProductionCoefficientDetailDTO> ProductionCoefficientDetails { get; set; }
		public List<ProductionCostBuysDTO> ProductionCostBuysDTODetail { get; set; }
		public List<ProductionCostBuysDistributionDTO> ProductionCostBuysDistributionDTODetail { get; set; }
		public List<ProductionCostsBuysDetailDTO> ProductionCostsBuysDetailDTODetail { get; set; }
		public List<PoundsDistributionGroupedByPlant> lsCostBuyDistributionGrouped { get; set; }
		public List<ProductionCostProcessDetailDto> lsCostProcessDetailDto { get; set; }
		public List<ProductionCostProcessDetailDtoDetail> lsCostProcessDetailDtoDetail { get; set; }
		public ProductionCoefficientDTO()
		{
			ProductionCoefficientDetails = new List<ProductionCoefficientDetailDTO>();
			ProductionCostBuysDTODetail = new List<ProductionCostBuysDTO>();
			ProductionCostBuysDistributionDTODetail = new List<ProductionCostBuysDistributionDTO>();
			ProductionCostsBuysDetailDTODetail = new List<ProductionCostsBuysDetailDTO>();
			lsCostBuyDistributionGrouped = new List<PoundsDistributionGroupedByPlant>();
			lsCostProcessDetailDto = new List<ProductionCostProcessDetailDto>();
			lsCostProcessDetailDtoDetail = new List<ProductionCostProcessDetailDtoDetail>();
		}
	}
	public class ProductionCoefficientDetailDTO
	{
		public bool selected { get; set; }
		public int id { get; set; }
		public int? idCostType { get; set; }
		public string costType { get; set; }
		public int idCostingPeriod { get; set; }
		public string numberCostingPeriod { get; set; }
		public DateTime date { get; set; }
		public string strDateDocument { get; set; }
		public string accountingValues { get; set; }
	}
	public class ProductionCostBuysDTO
	{
		public int id { get; set; }
		public int idProductionCoefficient { get; set; }
		public int idAllocationCostPeriodDetail { get; set; }
		public int idProductionExpense { get; set; }
		public int idProcessPlant { get; set; }
		public bool isCoefficient { get; set; }
		public decimal valueCost { get; set; }
		public bool canEdit { get; set; }
		public string nameProductionExpense { get; set; }
		public string nameProcessPlant { get; set; }
	}
	public class ProductionCostBuysDistributionDTO
	{
		public int id { get; set; }
		public int idProductionCoefficient { get; set; }
		public int? idProcessPlant { get; set; }
		public string nameProcessPlant { get; set; }
		public int? idInventoryLine { get; set; }
		public string nameInventoryLine { get; set; }
		public int? idProcessType { get; set; }
		public string nameProcessType { get; set; }
		public decimal pounds { get; set; }
		public decimal percentageP { get; set; }
		public decimal valueCurrency { get; set; }
		public decimal coefficientP { get; set; }
	}
	public class ProductionCostsBuysDetailDTO
	{
		public int id { get; set; }
		public int idProductionCoefficient { get; set; }
		public int? idProductionLotDetail { get; set; }
		public decimal quantityPounds { get; set; }
		public int? idPlantProcess { get; set; }
		public string PlantProcess { get; set; }
		public int idInventoryLine { get; set; }
		public string nameInventoryLine { get; set; }
		public int? idProcessType { get; set; }
		public string ProcessType { get; set; }
		public int id_lot { get; set; }
		public string NoLoteInterno { get; set; }
		public string NoLote { get; set; }
		public DateTime fechaRecepcion { get; set; }
		public string itemType { get; set; }
		public int id_item { get; set; }
		public int id_presentation { get; set; }
		public int id_warehouse { get; set; }
		public string warehouse { get; set; }
		public int id_warehousetype { get; set; }
		public string warehouseType { get; set; }
		public string itemCode { get; set; }
		public string itemName { get; set; }
		public string secuencia { get; set; }
		public int idInventoryMoveDetail { get; set; }
	}
	public class ProductionCostProcessDetailDto
	{
		public int id { get; set; }
		public int? idPlantProcess { get; set; }
		public string namePlantProcess { get; set; }
		public int idTipoBodega { get; set; }
		public string nameTipoBodega { get; set; }
		public int idBodega { get; set; }
		public string nameBodega { get; set; } 
		public int? idInventoryLine { get; set; }
		public string nameInventoryLine { get; set; }
		public int? idProcessType { get; set; }
		public string nameProcessType { get; set; }
		public decimal libras { get; set; }
		public bool tipo { get; set; }
		public decimal percentage { get; set; }
		public decimal valor { get; set; }
		public decimal valorOriginal { get; set; }
		public decimal coeficiente { get; set; }
	}
	public class ProductionCostProcessDetailDtoDetail
	{
		public int id { get; set; }
		public int? idPlantaProcess { get; set; }
		public string namePlantProcess { get; set; }
		public int idTipoBodega { get; set; }
		public string nameTipoBodega { get; set; }
		public int idBodega { get; set; }
		public string nameBodega { get; set; }
		public int idUbicacion { get; set; }
		public string nameUbicacion { get; set; }
		public int idLot { get; set; }
		public string loteInterno { get; set; }
		public string secuenciaTransaccional { get; set; }

		public int idInventoryLine { get; set; }
		public string nameInventoryLine { get; set; }
		public int? idProcessType { get; set; }
		public string nameProcessType { get; set; }
		public string categoria { get; set; }
		public int idProducto { get; set; }
		public string codigoProducto { get; set; }
		public string nameProducto { get; set; }
		public string presentacion { get; set; }
		public string talla { get; set; }
		public string marca { get; set; }
		public string grupo { get; set; }
		public string subgrupo { get; set; }
		public string modelo { get; set; }
		public string numeroLote { get; set; }
		public string numeroMovimiento { get; set; }
		public string tipoMovimiento { get; set; }
		public string naturaleza { get; set; }
		public DateTime fecha { get; set; }
		public decimal numeroCajas { get; set; }
		public decimal cantidadLibras { get; set; }
		public decimal cantidadKilos { get; set; }
	}
}