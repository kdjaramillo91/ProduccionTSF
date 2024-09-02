using DXPANACEASOFT.Extensions;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class CostAllocationController : DefaultController
	{
		[HttpPost]
		public ActionResult Index()
		{
			return View();
		}

		#region -- Filtro Consulta --
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult CostAllocationResultsPartial(int? id_documentState,
															string number,
															string reference,
															DateTime? startEmissionDate,
															DateTime? endEmissionDate,
															int? id_Warehouse,
															int? id_InventoryLine)
		{
			List<CostAllocation> model = null;

			if (id_Warehouse.HasValue)
			{
				model = db.CostAllocation
								.Include("Document")
								.Include("Document.DocumentState")
								.Include("CostAllocationResumido")
								.Include("CostAllocationWarehouse")
								.Where(r => r.CostAllocationWarehouse.Where(t => t.id_Warehouse == id_Warehouse.Value).Count() > 0)
								.ToList();
			}

			if (id_documentState.HasValue)
			{
				if (model == null)
				{
					model = db.CostAllocation
								.Include("Document")
								.Include("Document.DocumentState")
								.Include("CostAllocationResumido")
								.Include("CostAllocationWarehouse")
								.Where(r => r.Document.id_documentState == id_documentState.Value)
								.ToList();
				}
				else
				{
					model = model
							  .Where(r => r.Document.id_documentState == id_documentState.Value)
							  .ToList();
				}
			}

			if (!string.IsNullOrWhiteSpace(number))
			{
				if (model == null)
				{
					model = db.CostAllocation
								   .Include("Document")
								   .Include("Document.DocumentState")
								   .Include("CostAllocationResumido")
								   .Include("CostAllocationWarehouse")
								   .Where(r => r.Document.number.Trim().Contains(number.Trim()))
								   .ToList();
				}
				else
				{
					model = model
							.Where(r => r.Document.number.Trim().Contains(number.Trim()))
							.ToList();
				}
			}

			if (!string.IsNullOrWhiteSpace(reference))
			{
				if (model == null)
				{
					model = db.CostAllocation
									.Include("Document")
									.Include("Document.DocumentState")
									.Include("CostAllocationResumido")
									.Include("CostAllocationWarehouse")
									.Where(r => reference.Contains(r.Document.reference))
									.ToList();
				}
				else
				{
					model = model
							.Where(r => reference.Contains(r.Document.reference))
							.ToList();
				}
			}

			if (startEmissionDate.HasValue)
			{
				if (model == null)
				{
					model = db.CostAllocation
								.Include("Document")
								.Include("Document.DocumentState")
								.Include("CostAllocationResumido")
								.Include("CostAllocationWarehouse")
								.Where(o => DateTime.Compare(startEmissionDate.Value, o.Document.emissionDate) <= 0)
								 .ToList();
				}
				else
				{
					model = model.Where(o => DateTime.Compare(startEmissionDate.Value, o.Document.emissionDate) <= 0)
								 .ToList();
				}
			}

			if (endEmissionDate.HasValue)
			{
				if (model == null)
				{
					model = db.CostAllocation
								.Include("Document")
								.Include("Document.DocumentState")
								.Include("CostAllocationResumido")
								.Include("CostAllocationWarehouse")
								.Where(o => DateTime.Compare(o.Document.emissionDate, endEmissionDate.Value) <= 0)
								 .ToList();
				}
				else
				{
					model = model.Where(o => DateTime.Compare(o.Document.emissionDate, endEmissionDate.Value) <= 0)
								 .ToList();
				}
			}

			if (id_InventoryLine.HasValue)
			{
				if (model == null)
				{
					model = db.CostAllocation
									.Include("Document")
									.Include("Document.DocumentState")
									.Include("CostAllocationResumido")
									.Include("CostAllocationWarehouse")
									.Where(r => r.CostAllocationResumido.Where(t => t.id_InventoryLine == id_InventoryLine).Count() > 0)
									.ToList();
				}
				else
				{
					model = model
								.Where(r => r.CostAllocationResumido.Where(t => t.id_InventoryLine == id_InventoryLine).Count() > 0)
								.ToList();
				}
			}
			var _result = new List<CostAllocationDto>();
			if (model == null)
			{
				model = db.CostAllocation
								.Include("Document")
								.Include("Document.DocumentState")
								.Include("CostAllocationResumido")
								.Include("CostAllocationWarehouse")
								.ToList();
			}

			_result = model.Select(r => r.ToDto()).ToList();

			TempData["result"] = _result;
			TempData.Keep("result");
			return PartialView("_CostAllocationResultsPartial", _result.OrderByDescending(o => o.id).ToList());
		}

		[HttpPost]
		public ActionResult CostAllocationPartial()
		{
			//var model = db.ProductionLot.Where(p => p.ProductionProcess.code != "REC").OrderByDescending(p => p.id );
			var model = (TempData["result"] as List<CostAllocationDto>);
			model = model ?? new List<CostAllocationDto>();


			TempData.Keep("result");
			return PartialView("_CostAllocationPartial", model.OrderByDescending(o => o.id).ToList());
		}

		#endregion

		#region -- Edición --
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Edit(int? idCostAllocation)
		{
			CostAllocationDto model = new CostAllocationDto();
			CostAllocation costAllocation = db.CostAllocation.FirstOrDefault(i => i.id == idCostAllocation);
			if (costAllocation == null)
			{
				model = FillModelNew(idCostAllocation);
			}
			else
			{
				model = FillModelUpdate(idCostAllocation);
				model.IsCalculate = true;
			}

			SetTempData(model);
			ViewData["tabSelected"] = 0;
			ViewData["id_costAllocation"] = idCostAllocation;

			ViewEditResume(idCostAllocation);

			return PartialView(model);
		}

		private CostAllocationDto FillModelNew(int? idCostAllocation)
		{
			CostAllocationDto model = new CostAllocationDto();

			var _documentState = db.DocumentState
											.FirstOrDefault(r => r.code == "01");

			var _documentType = db.DocumentType
										.FirstOrDefault(r => r.code == "149");

			model.Document = new Document
			{
				emissionDate = DateTime.Now,
				id_documentState = _documentState.id,
				DocumentState = _documentState,
				id_documentType = _documentType.id,
				DocumentType = _documentType,
				dateCreate = DateTime.Now
			};

			model.CostAllocationResumido = new List<CostAllocationResumidoDto>();
			model.CostAllocationDetalle = new List<CostAllocationDetalleDto>();
			model.id_Warehousessex = new List<WarehouseModelDto>();
			model.warehouses = new string[] { };

			return model;
		}
		private CostAllocationDto FillModelUpdate(int? idCostAllocation)
		{
			CostAllocationDto model = new CostAllocationDto();

			// Obtener id naturaleza Ingreso
			var naturalezaList = db.AdvanceParametersDetail
											.Where(r => r.AdvanceParameters.code == "NMMGI")
											.Select(r => new
											{
												naturalezaId = r.id,
												naturalezaNombre = r.description
											})
											.ToArray();

			CostAllocation omodel = db.CostAllocation
										.Include("CostAllocationWarehouse")
										.Include("CostAllocationWarehouse.Warehouse")
										.FirstOrDefault(r => r.id == idCostAllocation);

			//var inventoryPeriodDetail = db.InventoryPeriodDetail
			//										.FirstOrDefault(r => r.id == omodel.id_InventoryPeriodDetail) ?? new InventoryPeriodDetail();
			//var warehouse = db.Warehouse
			//					  .FirstOrDefault(r => r.id == omodel.id_Warehouse) ?? new  Warehouse();

			var document = db.Document
							   .Include("DocumentState")
							   .Include("DocumentType")
							   .FirstOrDefault(r => r.id == omodel.id);
			var costAllocationResumido = db.CostAllocationResumido
													.Include("InventoryLine")
													.Include("ItemType")
													.Include("ItemTypeCategory")
													.Include("ItemSize")
													.Where(r => r.id_CostAllocation == idCostAllocation)
													.ToList()
													.Select(r => r.ToDto())
													.OrderBy(r => r.InventoryLineName)
													.ThenBy(r => r.ItemTypeName)
													.ThenBy(r => r.ItemTypeCategoryName)
													.ThenBy(r => r.ItemSizeName)
													.ToList();

			var costAllocationDetalle = db.CostAllocationDetail
												//.Include("InventoryLine")
												//.Include("ItemType")
												//.Include("ItemTypeCategory")
												.Where(r => r.id_CostAllocation == idCostAllocation)
												.ToList();

			// Construccion Detallado
			var _documentInventoryMoveIds = costAllocationDetalle
													.Select(r => r.id_InventoryMoveDetail)
													.ToArray();
			var _preInventoryMoveDetailList = db.InventoryMoveDetail
														.Where(r => _documentInventoryMoveIds.Contains(r.id))
														.Select(r => new
														{
															inventoryMoveDetailid = r.id,
															inventoryMoveId = r.id_inventoryMove,
															itemId = r.id_item,
															numeroCajas = (r.amountMove ?? 0),
															warehouseId = r.id_warehouse,
															warehouseName = (r.Warehouse != null) ? r.Warehouse.name : "",
															warehouseLocationId = (r.id_warehouseLocation ?? 0),
															warehouseLocationName = (r.WarehouseLocation != null) ? r.WarehouseLocation.name : "",
															loteId = (r.id_lot == null) ? 0 : r.id_lot,
															loteNumber = (r.Lot != null && r.Lot.internalNumber != null) ? r.Lot.internalNumber
																		: (r.Lot != null && r.Lot.ProductionLot != null && r.Lot.ProductionLot.internalNumber != null) ? r.Lot.ProductionLot.internalNumber : "",
															numeroMovimiento = r.InventoryMove.Document.number,
															fechaEmision = r.InventoryMove.Document.emissionDate,


														})
														.ToList();

			int[] _inventoryMoveIds = _preInventoryMoveDetailList
											.Select(r => r.inventoryMoveId)
											.ToArray();

			var inventoryMoveList = db.InventoryMove
											.Where(r => _inventoryMoveIds.Contains(r.id))
											.Select(r => new
											{
												inventoryMoveid = r.id,
												naturalesId = r.idNatureMove,
												motivoInventarioId = (r.id_inventoryReason ?? 0),
												motivoInventarioName = r.InventoryReason.description
											})
											.ToArray();


			var _inventoryMoveDetailList = (from mov in _preInventoryMoveDetailList
											join movhead in inventoryMoveList
											on mov.inventoryMoveId equals movhead.inventoryMoveid
											join natur in naturalezaList
											on movhead.naturalesId equals natur.naturalezaId
											select new
											{
												inventoryMoveDetailid = mov.inventoryMoveDetailid,
												inventoryMoveId = mov.inventoryMoveId,
												itemId = mov.itemId,
												numeroCajas = mov.numeroCajas,
												warehouseId = mov.warehouseId,
												warehouseName = mov.warehouseName,
												warehouseLocationId = mov.warehouseLocationId,
												warehouseLocationName = mov.warehouseLocationName,
												loteId = mov.loteId,
												loteNumber = mov.loteNumber,
												numeroMovimiento = mov.numeroMovimiento,
												fechaEmision = mov.fechaEmision,
												naturalezaName = natur.naturalezaNombre,
												motivoInventarioId = movhead.motivoInventarioId,
												motivoInventarioName = movhead.motivoInventarioName
											})
											.ToList();


			int[] _itemsIds = _preInventoryMoveDetailList
											.Select(r => r.itemId)
											.ToArray();
			var _itemList = db.Item
								.Where(r => _itemsIds.Contains(r.id))
								.Select(r => new
								{
									itemId = r.id,
									masterCode = r.masterCode,
									name = r.name,
									auxCode = r.auxCode,
									inventoryLineId = r.id_inventoryLine,
									inventoryLineName = r.InventoryLine.name,
									itemTypeId = r.id_itemType,
									itemTypeName = r.ItemType.name,
									itemTypeCategoryId = r.id_itemTypeCategory,
									itemTypeCategoryName = r.ItemTypeCategory.name,
									presentationId = (r.id_presentation ?? 0),
									itemSizeId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_size ?? 0),
									itemSizeName = (r.ItemGeneral != null && r.ItemGeneral.ItemSize != null) ? r.ItemGeneral.ItemSize.name : "",
									marcaId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_trademark ?? 0),
									marcaName = (r.ItemGeneral != null && r.ItemGeneral.ItemTrademark != null) ? r.ItemGeneral.ItemTrademark.name : "",
									grupoId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_group ?? 0),
									grupoName = (r.ItemGeneral != null && r.ItemGeneral.ItemGroup != null) ? r.ItemGeneral.ItemGroup.name : "",
									subGrupoId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_subgroup ?? 0),
									subGrupoName = (r.ItemGeneral != null && r.ItemGeneral.ItemGroup1 != null) ? r.ItemGeneral.ItemGroup1.name : "",
									modeloId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_trademarkModel ?? 0),
									modeloName = (r.ItemGeneral != null && r.ItemGeneral.ItemTrademarkModel != null) ? r.ItemGeneral.ItemTrademarkModel.name : "",
									// REFERENCES ActDatos.dbo.ItemGroup (id)
								})
								.ToList();
			int[] presentationIds = _itemList
										.Select(r => r.presentationId)
										.ToArray();

			var presentacionList = db.Presentation
											.Where(r => presentationIds.Contains(r.id))
											.ToList();
			var listaDetalle = (from cost in costAllocationDetalle
									//join doc in _inventoryMoveDetailList on
									//cost.id_InventoryMoveDetail equals doc.inventoryMoveId
								join mov in _inventoryMoveDetailList on
								cost.id_InventoryMoveDetail equals mov.inventoryMoveDetailid
								join it in _itemList on
								cost.id_Item equals it.itemId
								join pt in presentacionList on
								it.presentationId equals pt.id into p1
								from pres in p1.DefaultIfEmpty(new Presentation())
									//join conLibras in UMedidaConversionLibras on
									//pres.id_metricUnit equals conLibras.mOriginLibras into p2
									//from mlibra in p2.DefaultIfEmpty()
									//join conKilos in UMedidaConversionKilos on
									//pres.id_metricUnit equals conKilos.mOriginKilos into p3
									//from mkilos in p3.DefaultIfEmpty()
								select new CostAllocationDetalleDto
								{
									id_InventoryMoveDetail = mov.inventoryMoveDetailid,
									WarehouseId = mov.warehouseId,
									WarehouseName = mov.warehouseName,
									id_WarehouseLocation = mov.warehouseLocationId,
									WarehouseLocationName = mov.warehouseLocationName,
									InventoryLineId = it.inventoryLineId,
									InventoryLineName = it.inventoryLineName,
									ItemTypeId = it.itemTypeId,
									ItemTypeName = it.itemTypeName,
									ItemTypeCategoryId = it.itemTypeCategoryId,
									ItemTypeCategoryName = it.itemTypeCategoryName,
									CodigoProducto = it.masterCode,
									NombreProducto = it.name,
									PresentationId = it.presentationId,
									PresentationName = pres.name,
									ItemSizeId = it.itemSizeId,
									ItemSizeName = it.itemSizeName,
									ItemTrademarkId = it.marcaId,
									ItemTrademarkName = it.marcaName,
									ItemGroupId = it.grupoId,
									ItemGroupName = it.grupoName,
									ItemSubGroupId = it.subGrupoId,
									ItemSubGroupName = it.subGrupoName,
									ItemTrademarkModelId = it.modeloId,
									ItemTrademarkModelName = it.modeloName,
									LotNumber = mov.loteNumber,
									InventaryNumber = mov.numeroMovimiento,
									DateMovement = mov.fechaEmision,
									amountBox = cost.amountBox,
									amountPound = cost.amountPound,
									costPounds = cost.costPounds,
									costKg = cost.costKg,
									totalCost = cost.totalCost,
									totalCostKg = cost.totalCostKg,
									totalCostPounds = cost.totalCostPounds,
									amountKg = cost.amountKg,
									productionCost = cost.productionCost,
									id_Item = it.itemId,
									id_Lot = mov.loteId,
									id = cost.id,
									id_CostAllocation = cost.id_CostAllocation,
									NaturalezaMovimeinto = mov.naturalezaName,
									MotivoInventarioId = mov.motivoInventarioId,
									MotivoInventarioName = mov.motivoInventarioName,

								})
								.ToList();


			var _warehouseListIds = omodel
									.CostAllocationWarehouse
									.Select(r => r.id_Warehouse)
									.ToArray();

			model = omodel.ToDto();
			//model.EstadoPeriodoBodega = (inventoryPeriodDetail?.AdvanceParametersDetail?.description??"");
			//model.InventoryPeriodDetail = inventoryPeriodDetail.ToDto();
			model.Document = document;
			//model.Warehouse = warehouse.ToDto();
			model.CostAllocationResumido = costAllocationResumido;
			model.CostAllocationDetalle = listaDetalle;
			model.id_Warehousessex = GetWarehousexs(model.anio, model.mes, _warehouseListIds);
			model.warehouses = _warehouseListIds
									.Select(r => r.ToString())
									.ToArray();
			return model;
		}

		private void SetTempData(CostAllocationDto model)
		{
			TempData["costAllocation"] = model;
			TempData.Keep("costAllocation");
		}

		//[HttpPost, ValidateInput(false)]
		//public JsonResult GetEstateWareHousePeriod(CostAllocationDto input)
		//{
		//
		//	//CostAllocationDto model = (CostAllocationDto)TempData["costAllocation"];
		//	//int companyId = this.ActiveCompanyId;
		//	//string _estadoBodega = "";
		//	//
		//	//var peridoDetalle = db.InventoryPeriodDetail
		//	//							.FirstOrDefault(r => r.InventoryPeriod.year == input.anio
		//	//												&& r.InventoryPeriod.isActive == true
		//	//												&& r.periodNumber == input.mes
		//	//												&& r.InventoryPeriod.id_warehouse == input.id_Warehouse
		//	//												&& r.InventoryPeriod.id_Company == companyId);
		//	//model.id_Warehouse = input.id_Warehouse;
		//	//model.anio = input.anio;
		//	//model.mes = input.mes;
		//	//
		//	//if (peridoDetalle== null)
		//	//{
		//	//	// salir con error
		//	//	// retornar
		//	//	model.CostAllocationDetalle = model.CostAllocationDetalle?? new List<CostAllocationDetalleDto>();
		//	//	model.CostAllocationResumido = model.CostAllocationResumido ?? new List<CostAllocationResumidoDto>();
		//	//	SetTempData(model);
		//	//	ViewData["id_costAllocation"] = model.id;
		//	//
		//	//	var dataerr = new
		//	//	{
		//	//		estadoBodega = _estadoBodega,
		//	//		err = "No se puede determinar el estado de la bodega (No existe un Periodo Activo)"
		//	//	};
		//	//	return Json(dataerr, JsonRequestBehavior.AllowGet);
		//	//	//PartialView("_CostAllocationEditFormPartial", dataerr);
		//	//}
		//	//
		//	//_estadoBodega = (db.AdvanceParametersDetail
		//	//						.FirstOrDefault(r => r.id == peridoDetalle.id_PeriodState)?.description ?? "");
		//	//
		//	//var data = new
		//	//{
		//	//	estadoBodega = _estadoBodega,
		//	//	err = ""
		//	//};
		//	//
		//	//SetTempData(model);
		//	//ViewData["id_costAllocation"] = model.id;
		//	//
		//	//return Json(data, JsonRequestBehavior.AllowGet);
		//	////PartialView("_CostAllocationEditFormPartial", data);
		//}

		[HttpPost, ValidateInput(false)]
		public ActionResult Save(CostAllocationDto input,
									DateTime emissionDate,
									string description,
									bool isApproved)
		{
			CostAllocationDto model = (TempData["costAllocation"] as CostAllocationDto);
			CostAllocation modelSave = new CostAllocation();
			// CostAllocation 

			if (model.IsChangeResumen)
			{
				SetTempData(model);
				ViewData["tabSelected"] = 0;
				ViewData["id_costAllocation"] = model.id;
				ViewEditResume(model.id);
				ViewData["EditMessage"] = ErrorMessage("Valores Desactualizados, ejecute Distribuir Costos");
				return PartialView("_CostAllocationEditFormPartial", model);
			}
			var userId = this.ActiveUserId;

			var _documentType = db.DocumentType
											.FirstOrDefault(r => r.code == "149");
			if (input.id == 0)
			{
				var companyId = this.ActiveCompanyId;

				var _warehouse = input.warehouses.FirstOrDefault();
				int _warehouseId = int.Parse(_warehouse);
				// Obtener Periodo de Inventario
				var periodoInventario = db.InventoryPeriodDetail
													.FirstOrDefault(r => r.InventoryPeriod.year == input.anio
																		&& r.InventoryPeriod.isActive
																		&& r.InventoryPeriod.id_warehouse == _warehouseId
																		&& r.InventoryPeriod.id_Company == companyId
																		&& r.periodNumber == input.mes);
				modelSave.id_company = this.ActiveCompanyId;
				modelSave.fechaIncio = periodoInventario.dateInit;
				modelSave.fechaFin = periodoInventario.dateEnd;
				//modelSave.id_InventoryPeriodDetail = periodoInventario.id;
				//modelSave.id_Warehouse = input.id_Warehouse;
				modelSave.mes = input.mes;
				modelSave.anio = input.anio;
				//------------------------
				modelSave.dateCreate = DateTime.Now;
				modelSave.id_userCreate = userId;

				//------------------------
				// Document
				//------------------------

				var _documentState = db.DocumentState
											.FirstOrDefault(r => r.code == "01");

				var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
							 ? ActiveUser.EmissionPoint.First().id
							 : 0;
				if (id_emissionPoint == 0)
				{
					SetTempData(model);
					ViewData["tabSelected"] = 0;
					ViewData["id_costAllocation"] = model.id;
					ViewEditResume(model.id);
					ViewData["EditMessage"] = ErrorMessage("Su usuario no tiene asociado ningún punto de emisión.");
					return PartialView("_CostAllocationEditFormPartial", model);
				}

				modelSave.Document = new Document
				{
					number = GetDocumentNumber(_documentType?.id ?? 0),
					sequential = GetDocumentSequential(_documentType?.id ?? 0),
					id_emissionPoint = id_emissionPoint,
					id_documentState = _documentState.id,
					DocumentState = _documentState,
					id_documentType = _documentType.id,
					DocumentType = _documentType,
					dateCreate = DateTime.Now,
					id_userCreate = userId,
					emissionDate = emissionDate,
				};

			}
			else
			{
				modelSave = db.CostAllocation.FirstOrDefault(r => r.id == input.id);
			}
			modelSave.Document.description = description;
			modelSave.Document.dateUpdate = DateTime.Now;
			modelSave.Document.id_userUpdate = userId;
			modelSave.dateUpdate = DateTime.Now;
			modelSave.id_userUpdate = userId;

			bool saveOK = false;
			if (ModelState.IsValid)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						// Warehouse List
						if (model.id_Warehousessex != null && model.id_Warehousessex.Count > 0)
						{
							if (input.id != 0)
							{
								for (int j = modelSave.CostAllocationWarehouse.Count - 1; j >= 0; j--)
								{
									var bodega = modelSave.CostAllocationWarehouse.ElementAt(j);
									modelSave.CostAllocationWarehouse.Remove(bodega);
									db.Entry(bodega).State = EntityState.Deleted;
								}
							}

							var bodegaDtoList = model.id_Warehousessex
														.Select(r => new CostAllocationWarehouse
														{
															id_InventoryPeriodDetail = r.Id_InventoryPeriodDetail,
															id_Warehouse = r.Id
														}).ToList();
							foreach (var bodega in bodegaDtoList)
							{
								modelSave.CostAllocationWarehouse.Add(new CostAllocationWarehouse
								{
									id_InventoryPeriodDetail = bodega.id_InventoryPeriodDetail,
									id_Warehouse = bodega.id_Warehouse
								});
							}
						}

						// Detalle Resumido
						if (model.CostAllocationDetalle != null && model.CostAllocationDetalle.Count > 0)
						{

							if (input.id == 0)
							{
								if (_documentType != null)
								{
									_documentType.currentNumber = _documentType.currentNumber + 1;
									db.DocumentType.Attach(_documentType);
									db.Entry(_documentType).State = EntityState.Modified;
								}
							}

							// Base de Datas
							// modelSave.CostAllocationDetail
							var idsEdicion = model.CostAllocationDetalle.Select(r => r.id).ToArray();
							var idsBase = modelSave.CostAllocationDetail.Select(r => r.id).ToArray();
							var idsForDelete = idsBase
													.Where(r => !idsEdicion.Contains(r))
													.ToArray();
							var detailForDelete = modelSave
													.CostAllocationDetail
													.Where(r => idsForDelete.Contains(r.id))
													.ToList();

							for (int j = detailForDelete.Count - 1; j >= 0; j--)
							{
								var detail = detailForDelete.ElementAt(j);

								modelSave.CostAllocationDetail.Remove(detail);
								db.Entry(detail).State = EntityState.Deleted;
							}

							var resultCostAllocationDetail = model.CostAllocationDetalle
																		.Select(r => new CostAllocationDetail
																		{
																			id = r.id,
																			id_Item = r.id_Item,
																			id_metricUnitMove = r.id_metricUnitMove,
																			id_InventoryMoveDetail = r.id_InventoryMoveDetail,
																			id_Warehouse = r.WarehouseId,
																			id_WarehouseLocation = r.id_WarehouseLocation,
																			id_Lot = r.id_Lot,
																			amountBox = (int)r.amountBox,
																			productionCost = r.productionCost,
																			totalCost = r.totalCost,
																			amountPound = r.amountPound,
																			costPounds = r.costPounds,
																			amountKg = r.amountKg,
																			costKg = r.costKg,
																			id_userCreate = userId,
																			dateCreate = DateTime.Now,
																			totalCostPounds = r.totalCostPounds,
																			totalCostKg = r.totalCostKg,
																			dateUpdate = DateTime.Now,
																			id_userUpdate = userId
																		})
																		.ToList();

							foreach (var det in resultCostAllocationDetail)
							{
								if (det.id == 0)
								{
									modelSave.CostAllocationDetail.Add(new CostAllocationDetail
									{
										id_Item = det.id_Item,
										id_metricUnitMove = det.id_metricUnitMove,
										id_InventoryMoveDetail = det.id_InventoryMoveDetail,
										id_Warehouse = det.id_Warehouse,
										id_WarehouseLocation = det.id_WarehouseLocation,
										id_Lot = det.id_Lot,
										amountBox = (int)det.amountBox,
										productionCost = det.productionCost,
										totalCost = det.totalCost,
										amountPound = det.amountPound,
										costPounds = det.costPounds,
										amountKg = det.amountKg,
										costKg = det.costKg,
										id_userCreate = userId,
										dateCreate = DateTime.Now,
										totalCostPounds = det.totalCostPounds,
										totalCostKg = det.totalCostKg,
										dateUpdate = DateTime.Now,
										id_userUpdate = userId
									});
								}
								else
								{
									var detail = modelSave.CostAllocationDetail
																   .FirstOrDefault(r => r.id == det.id);
									detail.amountBox = det.amountBox;
									detail.amountKg = det.amountKg;
									detail.amountPound = det.amountPound;
									detail.costKg = det.costKg;
									detail.costPounds = det.costPounds;
									detail.dateUpdate = DateTime.Now;
									detail.id_metricUnitMove = det.id_metricUnitMove;
									detail.id_userUpdate = userId;
									detail.productionCost = det.productionCost;
									detail.totalCost = det.totalCost;
									detail.totalCostKg = det.totalCostKg;
									detail.totalCostPounds = det.totalCostPounds;
								}
							}
						}

						if (model.CostAllocationResumido != null && model.CostAllocationResumido.Count > 0)
						{
							// Update => si es actualizacion eliminar la version anterior
							if (input.id != 0)
							{
								for (int j = modelSave.CostAllocationResumido.Count - 1; j >= 0; j--)
								{
									var detail = modelSave.CostAllocationResumido.ElementAt(j);

									modelSave.CostAllocationResumido.Remove(detail);
									db.Entry(detail).State = EntityState.Deleted;
								}

							}
							//modelSave.CostAllocationResumido.Clear();
							var resultCostAllocationResumido = model.CostAllocationResumido
																		 .Select(r => new CostAllocationResumido
																		 {
																			 id_InventoryLine = r.id_InventoryLine,
																			 id_ItemType = r.id_ItemType,
																			 id_ItemTypeCategory = r.id_ItemTypeCategory,
																			 id_ItemSize = r.id_ItemSize,
																			 amountBox = (int)r.amountBox,
																			 amountPound = r.amountPound,
																			 amountKg = r.amountKg,
																			 unitCostPounds = r.unitCostPounds,
																			 unitCostKg = r.unitCostKg,
																			 averageCostUnit = r.averageCostUnit,
																			 totalCostPounds = r.totalCostPounds,
																			 totalCostKg = r.totalCostKg,
																			 totalCostUnit = r.totalCostUnit,
																			 id_userCreate = userId,
																			 dateCreate = DateTime.Now,
																			 id_userUpdate = userId,
																			 dateUpdate = DateTime.Now,
																		 })
																		 .ToList();

							foreach (var res in resultCostAllocationResumido)
							{
								modelSave.CostAllocationResumido.Add(new CostAllocationResumido
								{
									id_InventoryLine = res.id_InventoryLine,
									id_ItemType = res.id_ItemType,
									id_ItemTypeCategory = res.id_ItemTypeCategory,
									id_ItemSize = res.id_ItemSize,
									amountBox = res.amountBox,
									amountPound = res.amountPound,
									amountKg = res.amountKg,
									unitCostPounds = res.unitCostPounds,
									unitCostKg = res.unitCostKg,
									averageCostUnit = res.averageCostUnit,
									totalCostPounds = res.totalCostPounds,
									totalCostKg = res.totalCostKg,
									totalCostUnit = res.totalCostUnit,
									id_userCreate = userId,
									dateCreate = DateTime.Now,
									id_userUpdate = userId,
									dateUpdate = DateTime.Now,
								});
							}
						}

						if (input.id == 0)
						{
							db.CostAllocation.Add(modelSave);
						}
						else
						{
							if (isApproved)
							{
								var estadoAprobado = db.DocumentState
															.FirstOrDefault(r => r.code == "03");
								modelSave.Document.id_documentState = estadoAprobado.id;
								modelSave.Document.DocumentState = estadoAprobado;
								modelSave.dateUpdate = DateTime.Now;
							}

							db.CostAllocation.Attach(modelSave);
							db.Entry(modelSave).State = EntityState.Modified;
						}

						db.SaveChanges();
						trans.Commit();
						saveOK = true;
					}
					catch (Exception e)
					{
						trans.Rollback();

						SetTempData(model);
						ViewData["tabSelected"] = 0;
						ViewData["id_costAllocation"] = model.id;
						ViewData["EditMessage"] = ErrorMessage(e.Message);
						ViewEditResume(model.id);
						return PartialView("_CostAllocationEditFormPartial", model);
					}
				}
			}

			#region -- Set IDS / Actualizacion Modelo --
			if (input.id == 0)
			{
				model.id = modelSave.id;
				model.Document = modelSave.Document;
			}
			else
			{
				model.Document.emissionDate = modelSave.Document.emissionDate;
				model.Document.description = modelSave.Document.description;
			}

			string mensajeOK = "Asignación de Costo guardado exitosamente";
			if (saveOK)
			{
				model = FillModelUpdate(model.id);

				if (isApproved)
				{
					string err = AproveeCostAllocation(model);
					if (!string.IsNullOrWhiteSpace(err))
					{
						SetTempData(model);
						ViewData["tabSelected"] = 0;
						ViewData["id_costAllocation"] = model.id;
						ViewData["EditMessage"] = ErrorMessage(err);
						ViewEditResume(model.id);
						return PartialView("_CostAllocationEditFormPartial", model);
					}
					else
					{
						mensajeOK = "Asignación de Costo Aprobado";
					}

				}
			}

			//model.id_InventoryPeriodDetail = modelSave.id_InventoryPeriodDetail;
			model.IsCalculate = true;
			#endregion

			SetTempData(model);

			ViewData["tabSelected"] = 0;
			ViewData["id_costAllocation"] = model.id;
			ViewEditResume(model.id);
			ViewData["EditMessage"] = SuccessMessage(mensajeOK);
			return PartialView("_CostAllocationEditFormPartial", model);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Cancel(int id)
		{
			CostAllocationDto model = (TempData["costAllocation"] as CostAllocationDto);

			var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					var asignacion = db.CostAllocation.FirstOrDefault(r => r.id == id);
					if (asignacion != null)
					{
						asignacion.Document.id_documentState = documentState.id;
						asignacion.Document.DocumentState = documentState;
						asignacion.Document.dateUpdate = DateTime.Now;

						model.Document.id_documentState = documentState.id;
						model.Document.DocumentState = documentState;
						model.Document.dateUpdate = DateTime.Now;

						db.CostAllocation.Attach(asignacion);
						db.Entry(asignacion).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
					}
					else
						throw new Exception("No se ha anulado el documento");
				}
				catch (Exception)
				{
					trans.Rollback();
					// TODO LOG
					SetTempData(model);
					return Json(new { status = "error", message = "No se ha anulado el documento" });
				}
			}
			SetTempData(model);
			ViewData["tabSelected"] = 0;
			ViewData["id_costAllocation"] = model.id;
			ViewData["EditMessage"] = SuccessMessage("Asignación de Costo anulado");

			ViewEditResume(model.id);

			return PartialView("_CostAllocationEditFormPartial", model);
		}

		//[HttpPost, ValidateInput(false)]
		//public ActionResult Aprovee(int id)
		//{
		//	CostAllocationDto model = (TempData["costAllocation"] as CostAllocationDto);
		//
		//	if (model.id != id)
		//	{
		//		SetTempData(model);
		//		return Json(new { status = "error", message = $"Asignación de Costo no en Uso {Environment.NewLine} Cierre y vuelva a consultar" });
		//	}
		//
		//	using (DbContextTransaction trans = db.Database.BeginTransaction())
		//	{
		//		try
		//		{
		//			var estadoAprobado = db.DocumentState
		//										.FirstOrDefault(r => r.code == "03");
		//
		//			var document = db.Document
		//								.FirstOrDefault(r => r.id == model.id);
		//
		//			document.id_documentState = estadoAprobado.id;
		//			document.dateUpdate = DateTime.Now;
		//
		//			var costAllocation = db.CostAllocation
		//											.FirstOrDefault(r => r.id == model.id);
		//			
		//			bool ok = AproveeCostAllocation(model);
		//
		//			db.Document.Attach(document);
		//			db.Entry(document).State = EntityState.Modified;
		//
		//			db.SaveChanges();
		//			trans.Commit();
		//		}
		//		catch (Exception e)
		//		{
		//			SetTempData(model);
		//			trans.Rollback();
		//			return Json(new { status = "error", message = e.Message });
		//		}
		//	}
		//
		//	SetTempData(model);
		//	ViewData["tabSelected"] = 0;
		//	ViewData["id_costAllocation"] = model.id;
		//	ViewData["EditMessage"] = SuccessMessage("Asignación de Costo Aprobado");
		//
		//	ViewEditResume(model.id);
		//
		//	return PartialView("_CostAllocationEditFormPartial", model);
		//}

		[HttpPost, ValidateInput(false)]
		public ActionResult Revert(int id)
		{
			CostAllocationDto model = (TempData["costAllocation"] as CostAllocationDto);
			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					var estadoAprobado = db.DocumentState
												.FirstOrDefault(r => r.code == "01");

					var document = db.Document
										.FirstOrDefault(r => r.id == model.id);

					document.id_documentState = estadoAprobado.id;
					document.DocumentState = estadoAprobado;
					document.dateUpdate = DateTime.Now;

					model.Document.id_documentState = estadoAprobado.id;
					model.Document.DocumentState = estadoAprobado;
					model.Document.dateUpdate = DateTime.Now;

					bool ok = ReverseCostAllocation(model.id);

					db.Document.Attach(document);
					db.Entry(document).State = EntityState.Modified;

					db.SaveChanges();
					trans.Commit();
				}
				catch (Exception e)
				{
					trans.Rollback();
					SetTempData(model);
					return Json(new { status = "error", message = $"A ocurrido un error al reversar el Documento: {e.Message}" });
				}
			}

			SetTempData(model);
			ViewData["tabSelected"] = 0;
			ViewData["id_costAllocation"] = model.id;
			ViewData["EditMessage"] = SuccessMessage("Asignación de Costo Reversado");

			ViewEditResume(model.id);

			return PartialView("_CostAllocationEditFormPartial", model);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult CostAllocationWarehoseTokenView(int? anio, int? mes, int[] id_Warehousessex)
		{
			CostAllocationDto model = (TempData["costAllocation"] as CostAllocationDto);
			model = model ?? new CostAllocationDto();

			model.anio = (anio ?? 0);
			model.mes = (mes ?? 0);
			//model.id_Warehousessex = GetWarehousexs(anio, mes, id_Warehousessex);


			//List<WarehouseModelDto> bodegasModel = DataProviderWarehouse
			//												.FreezerWarehousebyCompanyParameterForCost(this.ActiveCompanyId, anio, mes);

			SetTempData(model);
			return PartialView("_CostAllocationWarehouseToken", model);
		}
		#endregion

		#region -- Vista Grid Resumen --
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult CostAllocationEditResumeDetailPartial(int? idCostAllocation)
		{
			CostAllocationDto model = (TempData["costAllocation"] as CostAllocationDto);
			model = model ?? new CostAllocationDto();

			var resultModel = model?.CostAllocationResumido.ToList() ?? new List<CostAllocationResumidoDto>();

			SetTempData(model);

			ViewData["id_costAllocation"] = idCostAllocation;

			ViewEditResume(model.id);


			return PartialView("_CostAllocationEditResumen", resultModel);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult CostAllocationEditResumeDetailUpdate(CostAllocationResumidoDto input)
		{
			CostAllocationDto model = (TempData["costAllocation"] as CostAllocationDto);
			//productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
			model = model ?? new CostAllocationDto();
			model.CostAllocationResumido = model.CostAllocationResumido ?? new List<CostAllocationResumidoDto>();

			if (ModelState.IsValid)
			{
				try
				{
					var modelItem = model.CostAllocationResumido.FirstOrDefault(it => it.id == input.id);//it.id_originLot == item.id_originLot && it.id_item == item.id_item);
					if (modelItem != null)
					{
						this.UpdateModel(modelItem);
						model.IsChangeResumen = true;
						SetTempData(model);
					}
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;

				}
			}
			else
				ViewData["EditError"] = "Corriga todos los errores.";


			var model0 = model?.CostAllocationResumido?.ToList() ?? new List<CostAllocationResumidoDto>();

			ViewEditResume(model.id);

			return PartialView("_CostAllocationEditResumen", model0);
		}
		#endregion

		#region -- Vista Grid Detalle--
		[HttpPost, ValidateInput(false)]
		public ActionResult CostAllocationEditDetallePartial(CostAllocationDetalleDto input)
		{
			CostAllocationDto model = (TempData["costAllocation"] as CostAllocationDto);
			model = model ?? new CostAllocationDto();
			model.CostAllocationDetalle = model.CostAllocationDetalle ?? new List<CostAllocationDetalleDto>();
			SetTempData(model);
			return PartialView("_CostAllocationEditDetalle", model.CostAllocationDetalle.ToList());
		}

		#endregion


		#region -- Procesamiento Informacion --
		private bool ExistsAllocation(CostAllocationDto input)
		{
			int idDocumentStateAnulado = (db.DocumentState.FirstOrDefault(r => r.code == "05")?.id ?? 0);

			int[] warehousexIds = input.id_Warehousessex.Select(r => r.Id).ToArray();

			bool model = db.CostAllocation
									.Any(r => r.anio == input.anio
											 && r.mes == input.mes
											 //&& r.id_Warehouse == input.id_Warehouse
											 && r.id_company == ActiveCompany.id
											 && r.Document.id_documentState != idDocumentStateAnulado
											 && r.CostAllocationWarehouse.Where(t => warehousexIds.Contains(t.id_Warehouse)).Count() > 0
											 );
			return model;
		}
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult CalculateInfo(CostAllocationDto input, Document document, bool isApproved)
		{

			CostAllocationDto model = (CostAllocationDto)TempData["costAllocation"];

			int[] warehousesIDs = input.warehouses.Select(r => int.Parse(r)).ToArray();

			input.id_Warehousessex = GetWarehousexs(input.anio, input.mes, warehousesIDs);
			input.warehouses = input.warehouses.Select(r => r).ToArray();

			if (ExistsAllocation(input))
			{
				SetTempData(model);
				return Json(new { status = "error", message = "Ya existe asignación para el Periodo y Bodega(s) ingresado(s)" });
			}

			model.anio = input.anio;
			model.mes = input.mes;
			//model.id_Warehouse = input.id_Warehouse;
			model.EstadoPeriodoBodega = input.EstadoPeriodoBodega;
			model.Document = model.Document ?? new Document();
			model.Document.emissionDate = document?.emissionDate ?? new DateTime();
			model.Document.description = document?.description ?? "";
			model.id_Warehousessex = GetWarehousexs(input.anio, input.mes, warehousesIDs);
			model.warehouses = input.warehouses.Select(r => r.ToString()).ToArray();

			int companyId = this.ActiveCompanyId;
			int documentStateAprobadoId = (db.DocumentState
												.FirstOrDefault(r => r.code == "03"
																	&& r.name == "APROBADA"
																	&& r.id_company == companyId
																	&& r.isActive == true)?.id ?? 0);

			var tipoBodegaFrio = db.SettingDetail
												.Where(r => r.Setting.code == "CONGW")
												.Select(r => r.value)
												.ToArray();
			var tiposBodegasFrio = db.WarehouseType
									.Where(r => tipoBodegaFrio.Contains(r.code))
									.Select(r => r.id)
									.ToArray();

			var bodegasFrio = db.Warehouse
									.Where(r => tiposBodegasFrio.Contains(r.id_warehouseType))
									.Select(r => r.id)
									.ToArray();

			// Obtener Bodegas Cerradas para el Periodo que estan cerradas
			List<InventoryPeriod> _inventoryPeriodList = db.InventoryPeriod
																//.Include("InventoryPeriod")
																.Where(r => bodegasFrio.Contains(r.id_warehouse)
																			&& r.id_Company == companyId
																			&& r.isActive == true
																			&& r.year == input.anio)
																.ToList();

			int[] _inventoryPeriodIds = _inventoryPeriodList
													.Select(r => r.id)
													.ToArray();

			List<InventoryPeriodDetail> _inventoryPeriodDetailList = db.InventoryPeriodDetail
																				//.Include("InventoryPeriodDetail")
																				.Where(r => _inventoryPeriodIds.Contains(r.id_InventoryPeriod)
																							&& r.periodNumber == input.mes
																							&& ((isApproved == true && r.isClosed == true) || isApproved == false))
																				.ToList();

			int[] bodegasCerradasPeriodo = (from ip in _inventoryPeriodList
											join ipd in _inventoryPeriodDetailList on
											ip.id equals ipd.id_InventoryPeriod
											select ip.id_warehouse)
											.Where(r => warehousesIDs.Contains(r))
											.ToArray();

			if (bodegasCerradasPeriodo.Count() == 0)
			{
				// salir con error
				// retornar
				//ViewData["EditError"] = "No existen movimientos para esta Bodega";
				model.CostAllocationDetalle = model.CostAllocationDetalle ?? new List<CostAllocationDetalleDto>();
				model.CostAllocationResumido = model.CostAllocationResumido ?? new List<CostAllocationResumidoDto>();
				SetTempData(model);
				return Json(new { status = "error", message = "No existen movimientos para estas Bodegas" });
				//ViewData["id_costAllocation"] = model.id;
				//return PartialView("_CostAllocationEditFormPartial", model);
			}


			// Obtener id naturaleza Ingreso
			var naturalezaList = db.AdvanceParametersDetail
											.Where(r => r.AdvanceParameters.code == "NMMGI")
											.Select(r => new
											{
												naturalezaId = r.id,
												naturalezaNombre = r.description
											})
											.ToArray();

			// Obtener los motivos de Inventario
			// Códigos Parametros
			//string[] _inventoryReasonCods = db.SettingDetail
			//									.Where(r => r.Setting.code == "MOVCON")
			//									.Select(r => r.value)
			//									.ToArray();

			// Ids 
			var _inventoryReasonList = db.InventoryReason
											.Where(r => r.isCostAllocation == true
														&& r.isActive
														&& r.id_company == companyId)
											.Select(r => new
											{
												r.id,
												inventoryReasonName = r.description
											})
											.ToArray();
			int[] _inventoryReasonIds = _inventoryReasonList.Select(r => r.id).ToArray();
			// Movimientos de Inventario
			var _preinventoryMoveIds = db.InventoryMove
											.Where(r => r.idWarehouse.HasValue
														&& bodegasCerradasPeriodo.Contains(r.idWarehouse.Value)
														&& r.id_inventoryReason.HasValue
														&& _inventoryReasonIds.Contains(r.id_inventoryReason.Value)
														/*&& r.idNatureMove == naturalezaIngresoId*/)
											.Select(r => new
											{
												r.id,
												naturalezaMovimientoId = r.idNatureMove,
												inventoryreasonId = r.id_inventoryReason
											})
											.ToArray();
			var _inventoryMoveIds = _preinventoryMoveIds.Select(r => r.id).ToArray();
			// Confirmación fecha de Movimientos
			// Estado Aprobado 
			var _documentInventoryMove0 = db.Document
													.Where(r => _inventoryMoveIds.Contains(r.id)
															   && r.id_documentState == documentStateAprobadoId)
													.Select(r => new
													{
														documentId = r.id,
														numeroMovimiento = r.number,
														fechaEmision = r.emissionDate
													})
													.ToList();

			var _documentInventoryMove = _documentInventoryMove0
													.Where(r => r.fechaEmision.Year == input.anio
															   && r.fechaEmision.Month == input.mes)
													.ToList();

			int[] _documentInventoryMoveIds = _documentInventoryMove
													.Select(r => r.documentId)
													.ToArray();
			// Detalle de Movimientos
			var _preInventoryMoveDetailList = db.InventoryMoveDetail
														.Where(r => _documentInventoryMoveIds.Contains(r.id_inventoryMove))
														.Select(r => new
														{
															inventoryMoveId = r.id_inventoryMove,
															inventoryMoveDetailId = r.id,
															metricUnitId = r.id_metricUnit,
															itemId = r.id_item,
															numeroCajas = (r.amountMove ?? 0),
															warehouseId = r.id_warehouse,
															warehouseName = (r.Warehouse != null) ? r.Warehouse.name : "",
															warehouseLocationId = (r.id_warehouseLocation ?? 0),
															warehouseLocationName = (r.WarehouseLocation != null) ? r.WarehouseLocation.name : "",
															loteId = (r.id_lot == null) ? 0 : r.id_lot,
															loteNumber = (r.Lot != null && r.Lot.internalNumber != null) ? r.Lot.internalNumber
																		: (r.Lot != null && r.Lot.ProductionLot != null && r.Lot.ProductionLot.internalNumber != null) ? r.Lot.ProductionLot.internalNumber : "",
															tipodocumentoId = r.Lot.Document.id_documentType,
															tidocumentoNombre = r.Lot.Document.DocumentType.name,

														})
														.ToList();

			var _inventoryMoveDetailList = (from modet in _preInventoryMoveDetailList
											join movHead in _preinventoryMoveIds
											on modet.inventoryMoveId equals movHead.id
											join natur in naturalezaList
											on movHead.naturalezaMovimientoId equals natur.naturalezaId
											join motivo in _inventoryReasonList
											on movHead.inventoryreasonId equals motivo.id
											select new
											{
												inventoryMoveId = modet.inventoryMoveId,
												inventoryMoveDetailId = modet.inventoryMoveDetailId,
												metricUnitId = modet.metricUnitId,
												itemId = modet.itemId,
												numeroCajas = modet.numeroCajas,
												warehouseId = modet.warehouseId,
												warehouseName = modet.warehouseName,
												warehouseLocationId = modet.warehouseLocationId,
												warehouseLocationName = modet.warehouseLocationName,
												loteId = modet.loteId,
												loteNumber = modet.loteNumber,
												tipodocumentoId = modet.tipodocumentoId,
												tidocumentoNombre = modet.tidocumentoNombre,
												naturalezaNombre = natur.naturalezaNombre,
												motivoInventarioId = motivo.id,
												motivoInventarioName = motivo.inventoryReasonName
											})
											.ToList();


			// Items Ids
			int[] _itemsIds = _preInventoryMoveDetailList
											.Select(r => r.itemId)
											.ToArray();
			// Items Listados
			// Seleccionar las columnas necesarias del resumido y del Detallado
			var _itemList = db.Item
								.Where(r => _itemsIds.Contains(r.id))
								.Select(r => new
								{
									itemId = r.id,
									masterCode = r.masterCode,
									name = r.name,
									auxCode = r.auxCode,
									inventoryLineId = r.id_inventoryLine,
									inventoryLineName = r.InventoryLine.name,
									itemTypeId = r.id_itemType,
									itemTypeName = r.ItemType.name,
									itemTypeCategoryId = r.id_itemTypeCategory,
									itemTypeCategoryName = r.ItemTypeCategory.name,
									presentationId = (r.id_presentation ?? 0),
									itemSizeId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_size ?? 0),
									itemSizeName = (r.ItemGeneral != null && r.ItemGeneral.ItemSize != null) ? r.ItemGeneral.ItemSize.name : "",
									marcaId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_trademark ?? 0),
									marcaName = (r.ItemGeneral != null && r.ItemGeneral.ItemTrademark != null) ? r.ItemGeneral.ItemTrademark.name : "",
									grupoId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_group ?? 0),
									grupoName = (r.ItemGeneral != null && r.ItemGeneral.ItemGroup != null) ? r.ItemGeneral.ItemGroup.name : "",
									subGrupoId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_subgroup ?? 0),
									subGrupoName = (r.ItemGeneral != null && r.ItemGeneral.ItemGroup1 != null) ? r.ItemGeneral.ItemGroup1.name : "",
									modeloId = (r.ItemGeneral == null) ? 0 : (r.ItemGeneral.id_trademarkModel ?? 0),
									modeloName = (r.ItemGeneral != null && r.ItemGeneral.ItemTrademarkModel != null) ? r.ItemGeneral.ItemTrademarkModel.name : "",
									// REFERENCES ActDatos.dbo.ItemGroup (id)
								})
								.ToList();

			// Presentaciones 
			int[] presentationIds = _itemList
										.Select(r => r.presentationId)
										.ToArray();

			var presentacionList = db.Presentation
											.Where(r => presentationIds.Contains(r.id))
											.ToList();

			// Obtener Conversiones Libras / Kilos
			var UMedidaConversionLibras = db.MetricUnitConversion
												.Where(r => r.MetricUnit1.code == "Lbs")
												.Select(r => new
												{
													mOriginLibras = r.id_metricOrigin,
													mFactorLibras = r.factor
												}).ToList();

			var UMedidaConversionKilos = db.MetricUnitConversion
												.Where(r => r.MetricUnit1.code == "Kg")
												.Select(r => new
												{
													mOriginKilos = r.id_metricOrigin,
													mFactorKilos = r.factor
												}).ToList();

			// Calcular Lista Detalle
			var listaDetalle = (from doc in _documentInventoryMove
								join mov in _inventoryMoveDetailList on
								doc.documentId equals mov.inventoryMoveId
								join it in _itemList on
								mov.itemId equals it.itemId
								join pt in presentacionList on
								it.presentationId equals pt.id into p1
								from pres in p1.DefaultIfEmpty(new Presentation())
								join conLibras in UMedidaConversionLibras on
								pres.id_metricUnit equals conLibras.mOriginLibras into p2
								from mlibra in p2.DefaultIfEmpty()
								join conKilos in UMedidaConversionKilos on
								pres.id_metricUnit equals conKilos.mOriginKilos into p3
								from mkilos in p3.DefaultIfEmpty()
								select new CostAllocationDetalleDto
								{
									WarehouseId = mov.warehouseId,
									WarehouseName = mov.warehouseName,
									id_WarehouseLocation = mov.warehouseLocationId,
									WarehouseLocationName = mov.warehouseLocationName,
									InventoryLineId = it.inventoryLineId,
									InventoryLineName = it.inventoryLineName,
									id_InventoryMoveDetail = mov.inventoryMoveDetailId,
									id_metricUnitMove = mov.metricUnitId,
									ItemTypeId = it.itemTypeId,
									ItemTypeName = it.itemTypeName,
									ItemTypeCategoryId = it.itemTypeCategoryId,
									ItemTypeCategoryName = it.itemTypeCategoryName,
									CodigoProducto = it.masterCode,
									NombreProducto = it.name,
									PresentationId = it.presentationId,
									PresentationName = pres.name,
									ItemSizeId = it.itemSizeId,
									ItemSizeName = it.itemSizeName,
									ItemTrademarkId = it.marcaId,
									ItemTrademarkName = it.marcaName,
									ItemGroupId = it.grupoId,
									ItemGroupName = it.grupoName,
									ItemSubGroupId = it.subGrupoId,
									ItemSubGroupName = it.subGrupoName,
									ItemTrademarkModelId = it.modeloId,
									ItemTrademarkModelName = it.modeloName,
									LotNumber = mov.loteNumber,
									InventaryNumber = doc.numeroMovimiento,
									DateMovement = doc.fechaEmision,
									amountBox = mov.numeroCajas,
									amountPound = ((pres.id != 0) ? (pres.maximum * pres.minimum) : 1) *
												   ((mlibra != null) ? mlibra.mFactorLibras : 1) * mov.numeroCajas,

									//((mlibra != null) ?
									//					( ( pres.id != 0) ? (pres.maximum * pres.minimum * mlibra.mFactorLibras ) : 1) : 1 ) * mov.numeroCajas,
									amountKg = ((pres.id != 0) ? (pres.maximum * pres.minimum) : 1) *
											   ((mkilos != null) ? mkilos.mFactorKilos : 1) * mov.numeroCajas,
									//((mkilos != null) ?
									//					((pres.id != 0) ? (pres.maximum * pres.minimum * mkilos.mFactorKilos) : 1) : 1) * mov.numeroCajas,
									id_Item = it.itemId,
									id_Lot = mov.loteId,
									NaturalezaMovimeinto = mov.naturalezaNombre,
									MotivoInventarioId = mov.motivoInventarioId,
									MotivoInventarioName = mov.motivoInventarioName

								})
								.ToList();

			model.CostAllocationDetalle = listaDetalle;

			Random rnd = new Random();
			var listaResumido = listaDetalle
									.GroupBy(r => new
									{
										r.InventoryLineId,
										r.ItemTypeId,
										r.ItemTypeCategoryId,
										r.ItemSizeId
									})
									.Select(r => new CostAllocationResumidoDto
									{
										id_InventoryLine = r.Key.InventoryLineId,
										id_ItemType = r.Key.ItemTypeId,
										id_ItemTypeCategory = r.Key.ItemTypeCategoryId,
										id_ItemSize = r.Key.ItemSizeId,
										InventoryLineName = r.Max(t => t.InventoryLineName),
										ItemTypeName = r.Max(t => t.ItemTypeName),
										ItemTypeCategoryName = r.Max(t => t.ItemTypeCategoryName),
										ItemSizeName = r.Max(t => t.ItemSizeName),
										amountBox = Math.Round(r.Sum(t => t.amountBox), 2),
										amountPound = r.Sum(t => t.amountPound),
										//amountKg = r.Sum(t => t.amountKg),
										id = rnd.Next(1, 999999)
									})
									.Select(r => new CostAllocationResumidoDto
									{
										id_InventoryLine = r.id_InventoryLine,
										id_ItemType = r.id_ItemType,
										id_ItemTypeCategory = r.id_ItemTypeCategory,
										id_ItemSize = r.id_ItemSize,
										InventoryLineName = r.InventoryLineName,
										ItemTypeName = r.ItemTypeName,
										ItemTypeCategoryName = r.ItemTypeCategoryName,
										ItemSizeName = r.ItemSizeName,
										amountBox = r.amountBox,
										amountPound = r.amountPound,
										amountKg = (r.amountPound / (decimal)2.2046),
										id = rnd.Next(1, 999999)

									})
									.ToList();
			model.CostAllocationResumido = listaResumido
												.OrderBy(r => r.InventoryLineName)
												.ThenBy(r => r.ItemTypeName)
												.ThenBy(r => r.ItemTypeCategoryName)
												.ThenBy(r => r.ItemSizeName)
												.ToList();
			model.IsCalculate = true;

			SetTempData(model);
			ViewData["id_costAllocation"] = model.id;

			ViewEditResume(model.id);

			return PartialView("_CostAllocationEditFormPartial", model);
		}

		[HttpPost]
		[ValidateInput(false)]
		public JsonResult CalculateDetalle()
		{
			CostAllocationDto model = (CostAllocationDto)TempData["costAllocation"];

			var result = new
			{
				estado = "OK",
				err = ""
			};

			if (model.CostAllocationDetalle == null || model.CostAllocationDetalle.Count() == 0
				 || model.CostAllocationResumido == null || model.CostAllocationResumido.Count() == 0
				)
			{
				result = new
				{
					estado = "ERR",
					err = "No se han obtenido los movimientos de Inventario"
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			var countOutValue = model.CostAllocationResumido
													.Where(r => r.unitCostPounds == 0)
													.Count();
			if (countOutValue > 0)
			{
				result = new
				{
					estado = "ERR",
					err = "Debe ingresar el Costo Unitario Lbs de cada grupo."
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}


			var _result = (from res in model.CostAllocationResumido
						   join det in model.CostAllocationDetalle on
						   new { linventario = res.id_InventoryLine, tproducto = res.id_ItemType, cat = res.id_ItemTypeCategory, talla = res.id_ItemSize } equals
						   new { linventario = det.InventoryLineId, tproducto = det.ItemTypeId, cat = det.ItemTypeCategoryId, talla = det.ItemSizeId }
						   select new CostAllocationDetalleDto
						   {
							   id = det.id,
							   id_CostAllocation = det.id_CostAllocation,
							   id_Item = det.id_Item,
							   id_metricUnitMove = det.id_metricUnitMove,
							   id_InventoryMoveDetail = det.id_InventoryMoveDetail,
							   id_WarehouseLocation = det.id_WarehouseLocation,
							   id_Lot = det.id_Lot,
							   amountBox = det.amountBox,
							   productionCost = (det.amountPound * res.unitCostPounds) / det.amountBox, // Costo Producto
							   totalCost = (det.amountPound * res.unitCostPounds), // Debe ser igual al Costo Total en Libras
							   amountPound = det.amountPound,
							   costPounds = res.unitCostPounds, // Costo Libra
							   totalCostPounds = (det.amountPound * res.unitCostPounds),  // (cantidad libras x Costo Libra)
							   amountKg = det.amountKg,
							   costKg = (res.unitCostPounds * (decimal)(2.2046)),// res.unitCostKg, // Costo Kilos
							   totalCostKg = (det.amountKg * (res.unitCostPounds * (decimal)(2.2046))), // Cantidad de kilos x Costo kilo
																										//-------------------------
							   WarehouseId = det.WarehouseId,
							   WarehouseName = det.WarehouseName,
							   WarehouseLocationName = det.WarehouseLocationName,
							   InventoryLineId = det.InventoryLineId,
							   InventoryLineName = det.InventoryLineName,
							   ItemTypeId = det.ItemTypeId,
							   ItemTypeName = det.ItemTypeName,
							   ItemTypeCategoryId = det.ItemTypeCategoryId,
							   ItemTypeCategoryName = det.ItemTypeCategoryName,
							   CodigoProducto = det.CodigoProducto,
							   NombreProducto = det.NombreProducto,
							   PresentationId = det.PresentationId,
							   PresentationName = det.PresentationName,
							   ItemSizeId = det.ItemSizeId,
							   ItemSizeName = det.ItemSizeName,
							   ItemTrademarkId = det.ItemTrademarkId,
							   ItemTrademarkName = det.ItemTrademarkName,
							   ItemGroupId = det.ItemGroupId,
							   ItemGroupName = det.ItemGroupName,
							   ItemSubGroupId = det.ItemSubGroupId,
							   ItemSubGroupName = det.ItemSubGroupName,
							   ItemTrademarkModelId = det.ItemTrademarkModelId,
							   ItemTrademarkModelName = det.ItemTrademarkModelName,
							   LotNumber = det.LotNumber,
							   InventaryNumber = det.InventaryNumber,
							   DateMovement = det.DateMovement,
							   NaturalezaMovimeinto = det.NaturalezaMovimeinto,
							   MotivoInventarioId = det.MotivoInventarioId,
							   MotivoInventarioName = det.MotivoInventarioName,
						   })
					   .ToList();

			model.CostAllocationDetalle = _result;
			model.IsChangeResumen = false;
			SetTempData(model);
			result = new
			{
				estado = "OK",
				err = ""
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateInput(false)]
		public JsonResult CalculateResumido(
			 string id_InventoryLine,
			 string id_ItemType,
			 string id_ItemTypeCategory,
			 string id_ItemSize,
			 int unitCostPoundsEnt,
			 int unitCostPoundsDec
			 )
		{
			CostAllocationDto model = (CostAllocationDto)TempData["costAllocation"];
			var result = new
			{
				status = "OK",
				err = "",
				unitCostKg = (decimal)0.00,
				averageCostUnit = (decimal)0.00000,
				totalCostPounds = (decimal)0.00,
				totalCostKg = (decimal)0.00,
				totalCostUnit = (decimal)0.00,
			};

			decimal unitCostPounds = (decimal)0.00000;


			char sep = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

			decimal.TryParse($"{unitCostPoundsEnt}{sep}{unitCostPoundsDec}", out unitCostPounds);

			var costResumido = model
								.CostAllocationResumido
								.FirstOrDefault(r => r.InventoryLineName.Trim() == id_InventoryLine.Trim()
														&& r.ItemTypeName.Trim() == id_ItemType.Trim()
														&& r.ItemTypeCategoryName.Trim() == id_ItemTypeCategory.Trim()
														&& r.ItemSizeName.Trim() == id_ItemSize.Trim());
			if (costResumido == null)
			{
				result = new
				{
					status = "ERR",
					err = "No existe información para el cálculo",
					unitCostKg = (decimal)0.00000,
					averageCostUnit = (decimal)0.00000,
					totalCostPounds = (decimal)0.00,
					totalCostKg = (decimal)0.00,
					totalCostUnit = (decimal)0.00,
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			costResumido.unitCostPounds = (unitCostPounds);

			var costoUnitarioKg = (costResumido.unitCostPounds * (decimal)2.2046);
			var costoTotalLb = (costResumido.unitCostPounds * costResumido.amountPound);
			var costoTotalKg = (costResumido.unitCostPounds * ((decimal)2.2046 * costResumido.amountKg));

			var costoPromedioUnidades = ((costResumido.unitCostPounds * costResumido.amountPound) / costResumido.amountBox);

			costResumido.unitCostKg = costoUnitarioKg;
			costResumido.totalCostPounds = costoTotalLb;
			costResumido.totalCostKg = costoTotalKg;
			costResumido.averageCostUnit = costoPromedioUnidades;
			costResumido.totalCostUnit = costoTotalLb;

			result = new
			{
				status = "OK",
				err = "",
				unitCostKg = costoUnitarioKg,
				averageCostUnit = costoPromedioUnidades,
				totalCostPounds = costoTotalLb,
				totalCostKg = costoTotalKg,
				totalCostUnit = costoTotalLb,
			};

			SetTempData(model);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateInput(false)]
		public JsonResult ValidateWarehouseOpen()
		{
			CostAllocationDto model = (CostAllocationDto)TempData["costAllocation"];

			var result = new
			{
				estado = "OK",
				err = ""
			};

			int[] warehouseIds = model.id_Warehousessex.Select(r => r.Id).ToArray();
			var warehouse = db.Warehouse
								.Where(r => warehouseIds.Contains(r.id))
								.ToList();
			if (warehouse.Count < warehouseIds.Length)
			{

				result = new
				{
					estado = "ERR",
					err = "Bodega de Distribución de Costo no esta configurada."
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			var inventoryPeriodDet = db.InventoryPeriodDetail
											  .Where(r => warehouseIds.Contains(r.InventoryPeriod.id_warehouse)
																					&& r.InventoryPeriod.year == model.anio
																					&& r.periodNumber == model.mes)
											  .ToList();
			if (inventoryPeriodDet.Count < warehouseIds.Length)
			{
				result = new
				{
					estado = "ERR",
					err = "Periodo de Inventario no esta configurado."
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
			if (vestado == null)
			{
				result = new
				{
					estado = "ERR",
					err = "No Existe Estados de Periodos de Bodega"
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			var estadoCerrado = (vestado.FirstOrDefault(r => r.valueCode.Trim() == "C")?.id ?? 0);

			var periodoNoCerrado = inventoryPeriodDet.FirstOrDefault(r => r.id_PeriodState != estadoCerrado);
			if (periodoNoCerrado != null)
			{
				result = new
				{
					estado = "ERR",
					err = $"{periodoNoCerrado.InventoryPeriod.Warehouse.name}  en Periodo {model.anio}/{model.mes}, no esta Cerrada."
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			var resumenSinAsignacion = model.CostAllocationResumido
													.Where(r => r.unitCostPounds == 0)
													.Select(r => new
													{
														msg = $"L. Inventario: {r.InventoryLineName}, Tipo: {r.ItemTypeName}, Categoría: {r.ItemTypeCategoryName}, Talla: {r.ItemSizeName}"
													}).ToList();

			if (resumenSinAsignacion.Count() > 0)
			{
				var sinAsignacion = resumenSinAsignacion.FirstOrDefault();
				result = new
				{
					estado = "ERR",
					err = $"Falta asignación de costo al grupo {sinAsignacion.msg}"
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			var NumdetalleCero = model.CostAllocationDetalle
										.Where(r => r.productionCost == 0)
										.Count();
			if (NumdetalleCero > 0)
			{
				result = new
				{
					estado = "ERR",
					err = $"Falta Distribuir Costos"
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			if (model.IsChangeResumen)
			{
				result = new
				{
					estado = "ERR",
					err = "Valores Desactualizados, ejecute Distribuir Costos"
				};
				SetTempData(model);
				return Json(result, JsonRequestBehavior.AllowGet);
			}


			SetTempData(model);
			result = new
			{
				estado = "OK",
				err = ""
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		private string AproveeCostAllocation(CostAllocationDto model)
		{
			string Err = "";
			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					db.AproveeCostAllocation(model.id);
					db.SaveChanges();
					trans.Commit();
				}
				catch (Exception e)
				{
					trans.Rollback();
					Err = e.Message;
				}
			}
			return Err;
		}
		private bool ReverseCostAllocation(int modelId)
		{
			bool OK = false;
			db.ReverseCostAllocation(modelId);
			return OK;
		}

		private void ViewEditResume(int? idcostAllocation)
		{
			bool viewEditResume = false;
			if (!idcostAllocation.HasValue)
			{
				viewEditResume = true;
			}
			else
			{
				string code_state = (db.Document.FirstOrDefault(r => r.id == idcostAllocation)?.DocumentState?.code ?? "00");

				switch (code_state)
				{
					case "00":
						viewEditResume = true;
						break;
					case "01":
						viewEditResume = true;
						break;
					case "03":
						viewEditResume = false;
						break;
					case "05":
						viewEditResume = false;
						break;
					case "09":
						viewEditResume = false;
						break;
				};
			}

			ViewBag.viewEditResume = viewEditResume;
		}

		private List<WarehouseModelDto> GetWarehousexs(int anio, int mes, int[] warehousexIds)
		{
			List<WarehouseModelDto> model = new List<WarehouseModelDto>();

			var bodegas = db.Warehouse
								.Where(r => warehousexIds.Contains(r.id))
								.ToList();
			var estadosPeriodos = db.AdvanceParametersDetail
													.Where(r => r.AdvanceParameters.code == "EPIV1")
													.ToList();
			var periodos = db.InventoryPeriodDetail
									.Where(r => warehousexIds.Contains(r.InventoryPeriod.id_warehouse)
												&& r.InventoryPeriod.year == anio
												&& r.periodNumber == mes)
									.Select(r => new
									{
										bodegaId = r.InventoryPeriod.id_warehouse,
										periodoId = r.id,
										estadoId = r.id_PeriodState
									})
									.ToList();

			model = (from bodega in bodegas
					 join periodo in periodos on bodega.id equals periodo.bodegaId
					 join estado in estadosPeriodos on periodo.estadoId equals estado.id
					 select new WarehouseModelDto
					 {
						 Id = bodega.id,
						 Id_InventoryPeriodDetail = periodo.periodoId,
						 Descripcion = $"{bodega.name} - {estado.valueName}"
					 })
					 .ToList();

			return model;
		}
		#endregion


		#region ACTIONS

		[HttpPost, ValidateInput(false)]
		public JsonResult Actions(int id)
		{
			var actions = new
			{
				btnApprove = true,
				btnSave = true,
				btnCancel = false,
				btnRevert = false,
				btnPrint = false,
			};

			if (id == 0)
			{
				return Json(actions, JsonRequestBehavior.AllowGet);
			}

			CostAllocation costAllocation = db.CostAllocation.FirstOrDefault(r => r.id == id);
			string code_state = costAllocation.Document.DocumentState.code;

			if (code_state == "01") // Pendiente
			{
				actions = new
				{
					btnApprove = true,
					btnSave = true,
					btnCancel = true,
					btnRevert = false,
					btnPrint = true
				};
			}
			else if (code_state == "03") // Aprobado
			{
				actions = new
				{
					btnApprove = false,
					btnSave = true,
					btnCancel = false,
					btnRevert = true,
					btnPrint = true
				};
			}
			else if (code_state == "05") // ANULADO
			{
				actions = new
				{
					btnApprove = false,
					btnSave = false,
					btnCancel = false,
					btnRevert = false,
					btnPrint = true,
				};
			}
			else if (code_state == "09") // ANULADO
			{
				actions = new
				{
					btnApprove = false,
					btnSave = true,
					btnCancel = false,
					btnRevert = false,
					btnPrint = false,
				};
			}

			return Json(actions, JsonRequestBehavior.AllowGet);
		}

		#endregion


		#region BULK 
		[HttpPost, ValidateInput(false)]
		public JsonResult CancelSelected(int[] idsCancel)
		{
			var actions = new
			{
				message = "",
				status = ""
			};

			List<CostAllocationDto> model = (TempData["result"] as List<CostAllocationDto>);
			model = model ?? new List<CostAllocationDto>();

			var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					var asignaciones = db.CostAllocation
												.Where(r => idsCancel.Contains(r.id))
												.ToList();
					foreach (var asignacion in asignaciones)
					{
						if (asignacion.Document.DocumentState.code == "01")
						{
							asignacion.Document.id_documentState = documentState.id;
							asignacion.Document.DocumentState = documentState;
							asignacion.Document.dateUpdate = DateTime.Now;

							var _asignacion = model.FirstOrDefault(r => r.id == asignacion.id);
							_asignacion.Document.id_documentState = documentState.id;
							_asignacion.Document.DocumentState = documentState;
							_asignacion.Document.dateUpdate = DateTime.Now;

							db.CostAllocation.Attach(asignacion);
							db.Entry(asignacion).State = EntityState.Modified;


						}
						else
							throw new Exception("No se ha(n) anulado el(los) documento(s)");
					}

					db.SaveChanges();
					trans.Commit();
				}
				catch (Exception)
				{
					trans.Rollback();
					TempData["result"] = model;
					TempData.Keep("result");
					return Json(new { status = "error", message = "No se ha(n) anulado el(los) documento(s)" });
				}
			}
			TempData["result"] = model;
			TempData.Keep("result");
			return Json(new { status = "ok", message = "Asignacion(es) de Costo anulada(s)" });
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult RevertSelected(int[] idsRevert)
		{
			var actions = new
			{
				message = "",
				status = ""
			};

			List<CostAllocationDto> model = (TempData["result"] as List<CostAllocationDto>);
			model = model ?? new List<CostAllocationDto>();

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					var documentState = db.DocumentState
												.FirstOrDefault(r => r.code == "01");

					var asignaciones = db.CostAllocation
												.Where(r => idsRevert.Contains(r.id))
												.ToList();

					foreach (var asignacion in asignaciones)
					{
						var document = db.Document
											.FirstOrDefault(r => r.id == asignacion.id);

						if (document.DocumentState.code == "03")
						{
							document.id_documentState = documentState.id;
							document.DocumentState = documentState;
							document.dateUpdate = DateTime.Now;

							var _asignacion = model.FirstOrDefault(r => r.id == asignacion.id);
							_asignacion.Document.id_documentState = documentState.id;
							_asignacion.Document.DocumentState = documentState;
							_asignacion.Document.dateUpdate = DateTime.Now;

							bool ok = ReverseCostAllocation(asignacion.id);

							db.Document.Attach(document);
							db.Entry(document).State = EntityState.Modified;
						}
						else
						{
							throw new Exception("No se ha(n) reversado el(los) documento(s)");
						}
					}

					db.SaveChanges();
					trans.Commit();
				}
				catch
				{
					trans.Rollback();
					TempData["result"] = model;
					TempData.Keep("result");
					return Json(new { status = "error", message = "No se ha(n) reversado el(los) documento(s)" });
				}
			}

			TempData["result"] = model;
			TempData.Keep("result");
			return Json(new { status = "ok", message = "Asignacion(es) de Costo reversada(s)" });
		}

		#endregion
	}
}