using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentP.DocumentModels;
using DXPANACEASOFT.Models.DocumentStateP.DocumentStateModel;
using DXPANACEASOFT.Models.DocumentTypeP.DocumentTypeModels;
using DXPANACEASOFT.Models.Estructure.EstructureModels;
using DXPANACEASOFT.Models.General;
using DXPANACEASOFT.Models.InventoryMoveP.InventoryMoveModel;
using DXPANACEASOFT.Models.ItemP.ItemModel;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using DXPANACEASOFT.Models.WarehouseP.WarehouseModel;
using DXPANACEASOFT.Reports.Kardex;
using DXPANACEASOFT.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DXPANACEASOFT.Models.BackgroundProcessManagement;
using OfficeOpenXml;
using System.IO;
using LicenseContext = OfficeOpenXml.LicenseContext;
using DevExpress.CodeParser;
using DevExpress.Web.Internal;
using Utilitarios.Logs;
using System.Configuration;
using OfficeOpenXml.Table;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class KardexController : DefaultController
	{
		public static GridViewSettings gridViewSettings { get; set; }
		public static void SetSettingGridViewKardex(GridViewSettings settings, GridViewSettings settingLast)
		{
			if (settingLast != null)
			{
				gridViewSettings = settingLast;
			}
			else
			{
				CreateSettingGridViewKardexTempData();
			}
			//gridViewSettings = new GridViewSettings();
			settings.Name = gridViewSettings.Name;
			settings.CallbackRouteValues = gridViewSettings.CallbackRouteValues;

			settings.Width = gridViewSettings.Width;

			settings.SettingsBehavior.AllowFixedGroups = gridViewSettings.SettingsBehavior.AllowFixedGroups;

			settings.KeyFieldName = gridViewSettings.KeyFieldName;

			settings.SettingsPager.Visible = gridViewSettings.SettingsPager.Visible;
			settings.Settings.ShowGroupPanel = gridViewSettings.Settings.ShowGroupPanel;
			settings.Settings.ShowFilterRow = gridViewSettings.Settings.ShowFilterRow;
			settings.SettingsBehavior.AllowSelectByRowClick = gridViewSettings.SettingsBehavior.AllowSelectByRowClick;

			settings.SettingsAdaptivity.AdaptivityMode = gridViewSettings.SettingsAdaptivity.AdaptivityMode;
			settings.SettingsAdaptivity.AdaptiveColumnPosition = gridViewSettings.SettingsAdaptivity.AdaptiveColumnPosition;
			settings.SettingsAdaptivity.AdaptiveDetailColumnCount = gridViewSettings.SettingsAdaptivity.AdaptiveDetailColumnCount;
			settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = gridViewSettings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded;
			settings.SettingsAdaptivity.HideDataCellsAtWindowInnerWidth = gridViewSettings.SettingsAdaptivity.HideDataCellsAtWindowInnerWidth;

			settings.Styles.Header.BackColor = gridViewSettings.Styles.Header.BackColor;
			settings.Styles.Header.Font.Bold = gridViewSettings.Styles.Header.Font.Bold;


			settings.ClientSideEvents.BeginCallback = gridViewSettings.ClientSideEvents.BeginCallback;

			#region SearchPanel

			//Panel de Busqueda
			settings.SettingsSearchPanel.Visible = gridViewSettings.SettingsSearchPanel.Visible;
			settings.Styles.SearchPanel.CssClass = gridViewSettings.Styles.SearchPanel.CssClass;

			#endregion

			settings.Settings.ShowFooter = gridViewSettings.Settings.ShowFooter;

			settings.CustomJSProperties = gridViewSettings.CustomJSProperties;


			settings.GroupSummary.Add(DevExpress.Data.SummaryItemType.Sum, "balanceCutting").DisplayFormat = gridViewSettings.GroupSummary.Add(DevExpress.Data.SummaryItemType.Sum, "balanceCutting").DisplayFormat;
			settings.GroupSummary.Add(DevExpress.Data.SummaryItemType.Sum, "balanceCuttingCost").DisplayFormat = gridViewSettings.GroupSummary.Add(DevExpress.Data.SummaryItemType.Sum, "balanceCuttingCost").DisplayFormat;
			//}

			//SetSettingGridViewKardexTempData(settings);

		}
		public static GridViewSettings CreateSettingGridViewKardexTempData()
		{
			gridViewSettings = new GridViewSettings();
			gridViewSettings.Name = "gvKardexDetails";
			gridViewSettings.CallbackRouteValues = new { Controller = "Kardex", Action = "KardexPartial" };

			gridViewSettings.Width = Unit.Percentage(100);

			gridViewSettings.SettingsBehavior.AllowFixedGroups = true;

			gridViewSettings.KeyFieldName = "id";

			gridViewSettings.SettingsPager.Visible = true;
			gridViewSettings.Settings.ShowGroupPanel = true;
			gridViewSettings.Settings.ShowFilterRow = true;
			gridViewSettings.SettingsBehavior.AllowSelectByRowClick = true;

			gridViewSettings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.Off;
			gridViewSettings.SettingsAdaptivity.AdaptiveColumnPosition = GridViewAdaptiveColumnPosition.Right;
			gridViewSettings.SettingsAdaptivity.AdaptiveDetailColumnCount = 1;
			gridViewSettings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = false;
			gridViewSettings.SettingsAdaptivity.HideDataCellsAtWindowInnerWidth = 0;

			gridViewSettings.Styles.Header.BackColor = System.Drawing.Color.FromArgb(255, 255, 191, 102);
			gridViewSettings.Styles.Header.Font.Bold = true;


			gridViewSettings.ClientSideEvents.BeginCallback = "OnGridViewKardex_BeginCallback";

			#region SearchPanel

			//Panel de Busqueda
			gridViewSettings.SettingsSearchPanel.Visible = true;
			gridViewSettings.Styles.SearchPanel.CssClass = "searchPanel";

			#endregion

			gridViewSettings.Settings.ShowFooter = true;

			gridViewSettings.CustomJSProperties = (s, e) =>
			{
				MVCxGridView detailsGrid = s as MVCxGridView;
				if (detailsGrid == null) return;

				e.Properties["cpSettingKardex"] = KardexController.gridViewSettings;// detailsGrid.Settings;//ViewData["settingGridViewKardex"];
																					//e.Properties["cpFilteredRowCountWithoutPage"] = GetFilteredRowCountWithoutPage(detailsGrid);
			};


			gridViewSettings.GroupSummary.Add(DevExpress.Data.SummaryItemType.Sum, "balanceCutting").DisplayFormat = "<b>Saldo: {0:#,##0.00}</b>"; //Color.FromArgb(255, 255, 191, 102) <font color='red'><b>Saldo: {0:#,##0.00}</b></font>
			gridViewSettings.GroupSummary.Add(DevExpress.Data.SummaryItemType.Sum, "balanceCuttingCost").DisplayFormat = "Costo de Saldo: {0:c2}";

			return gridViewSettings;
		}

		[HttpPost]
		public ActionResult Index(bool? toReturn)
		{
			if ((bool?)toReturn ?? false)
			{
				ViewData["KardexFilter"] = TempData["KardexFilter"] as KardexFilter;
				TempData.Keep("KardexFilter");

				var model = TempData["model"] as List<ResultKardex>;
				model = model ?? new List<ResultKardex>();

				TempData["model"] = model;
				TempData.Keep("model");
			}
			else
			{
				TempData.Remove("KardexFilter");
				TempData.Remove("model");
			}

			return PartialView("Index");
		}

		#region KARDEX RESULTS

		private InventoryMoveDetail GetInventoryMoveDetailLessEqualOf(DateTime cutoffDate, InventoryMoveDetail inventoryMoveDetailCurrent)
		{
			if (inventoryMoveDetailCurrent != null)
			{
				if (DateTime.Compare(cutoffDate.Date, inventoryMoveDetailCurrent.InventoryMove.Document.emissionDate.Date) >= 0)
				{
					return inventoryMoveDetailCurrent;
				}
				else
				{
					if (inventoryMoveDetailCurrent.id_inventoryMoveDetailPrevious != null)
					{
						return GetInventoryMoveDetailLessEqualOf(cutoffDate, inventoryMoveDetailCurrent.InventoryMoveDetail3);
					}
					else
					{
						return null;
					}
				}
			}
			return null;
		}
		private InventoryMoveDetailCabModelP GetInventoryMoveDetailLessEqualOfFixed(DateTime cutoffDate, InventoryMoveDetailCabModelP inventoryMoveDetailCurrent, List<InventoryMoveDetailCabModelP> lstInv)
		{
			if (inventoryMoveDetailCurrent != null)
			{
				if (DateTime.Compare(cutoffDate.Date, inventoryMoveDetailCurrent.emissionDate) >= 0)
				{
					return inventoryMoveDetailCurrent;
				}
				else
				{
					if (inventoryMoveDetailCurrent.id_inventoryMoveDetailPrevious != null)
					{
						return GetInventoryMoveDetailLessEqualOfFixed(cutoffDate, lstInv.FirstOrDefault(fod => fod.id == inventoryMoveDetailCurrent.id_inventoryMoveDetailPrevious), lstInv);
					}
					else
					{
						return null;
					}
				}
			}
			return null;
		}

		[HttpPost]
		public ActionResult KardexResults(InventoryMove inventoryMove,
										  InventoryEntryMove entryMove,
										  InventoryExitMove exitMove,
										  Document document,
										  DateTime? startEmissionDate, DateTime? endEmissionDate,
										  string numberLot, string internalNumberLot, int[] items)
		{
			var tempKardexFilter = TempData["KardexFilter"] as KardexFilter;
			//tempKardexFilter = tempKardexFilter ?? new KardexFilter();
			//if(tempKardexFilter != null)
			//{
			//    inventoryMove = tempKardexFilter.inventoryMoveDetail.InventoryMove;
			//    entryMove = tempKardexFilter.inventoryMoveDetail.InventoryMove.InventoryEntryMove;
			//    exitMove = tempKardexFilter.inventoryMoveDetail.InventoryMove.InventoryExitMove;
			//    document = tempKardexFilter.inventoryMoveDetail.InventoryMove.Document;
			//    startEmissionDate = tempKardexFilter.startEmissionDate;
			//    endEmissionDate = tempKardexFilter.endEmissionDate;
			//    numberLot = tempKardexFilter.inventoryMoveDetail.Lot.number;
			//    internalNumberLot = tempKardexFilter.inventoryMoveDetail.Lot.ProductionLot.internalNumber;
			//    items = tempKardexFilter.items;
			//}
			//else
			//{
			inventoryMove.InventoryEntryMove = entryMove;
			inventoryMove.InventoryExitMove = exitMove;
			inventoryMove.Document = document;
			tempKardexFilter = new KardexFilter
			{
				inventoryMoveDetail = new InventoryMoveDetail
				{
					InventoryMove = inventoryMove,
					Lot = new Lot
					{
						number = numberLot,
						ProductionLot = new ProductionLot
						{
							internalNumber = internalNumberLot
						}
					},
				},
				startEmissionDate = startEmissionDate,
				endEmissionDate = endEmissionDate,
				items = items
			};
			//}


			TempData["KardexFilter"] = tempKardexFilter;
			TempData.Keep("KardexFilter");

			var model = db.InventoryMoveDetail.Where(w => w.InventoryMove.Document.DocumentState.code.Equals("03") ||//APROBADA
														 w.InventoryMove.Document.DocumentState.code.Equals("07")).ToList();//REVERSADA


			#region DOCUMENT FILTERS

			if (document.id_documentType != 0)
			{
				model = model.Where(o => o.InventoryMove.Document.id_documentType == document.id_documentType).ToList();
			}

			//if (document.id_documentState != 0)
			//{
			//    model = model.Where(o => o.InventoryMove.Document.id_documentState == document.id_documentState).ToList();
			//}

			if (!string.IsNullOrEmpty(document.number) && document.number != "Todos")
			{
				model = model.Where(o => o.InventoryMove.Document.number.Contains(document.number)).ToList();
			}

			if (!string.IsNullOrEmpty(document.reference) && document.reference != "Todas")
			{
				model = model.Where(o => o.InventoryMove.Document.reference.Contains(document.reference)).ToList();
			}

			if (startEmissionDate != null && endEmissionDate != null)
			{
				model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.InventoryMove.Document.emissionDate.Date) <= 0 && DateTime.Compare(o.InventoryMove.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
			}

			#endregion

			#region INVENTORY ENTRY MOVE

			if (inventoryMove.id_inventoryReason != 0 && inventoryMove.id_inventoryReason != null)
			{
				model = model.Where(o => o.InventoryMove.id_inventoryReason == inventoryMove.id_inventoryReason).ToList();
			}

			#region InventoryExitMove
			if (exitMove.id_warehouseExit != 0 && exitMove.id_warehouseExit != null)
			{
				model = model.Where(o => o.InventoryMove.InventoryExitMove?.id_warehouseExit == exitMove.id_warehouseExit).ToList();
			}

			if (exitMove.id_warehouseLocationExit != 0 && exitMove.id_warehouseLocationExit != null)
			{
				model = model.Where(o => o.InventoryMove.InventoryExitMove?.id_warehouseLocationExit == exitMove.id_warehouseLocationExit).ToList();
			}

			if (exitMove.id_dispatcher != 0)
			{
				model = model.Where(o => o.InventoryMove.InventoryExitMove?.id_dispatcher == exitMove.id_dispatcher).ToList();
			}
			#endregion

			#region InventoryEntryMove
			if (entryMove.id_warehouseEntry != 0 && entryMove.id_warehouseEntry != null)
			{
				model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_warehouseEntry == entryMove.id_warehouseEntry).ToList();
			}

			if (entryMove.id_warehouseLocationEntry != 0 && entryMove.id_warehouseLocationEntry != null)
			{
				model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_warehouseLocationEntry == entryMove.id_warehouseLocationEntry).ToList();
			}

			if (entryMove.id_receiver != 0)
			{
				model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_receiver == entryMove.id_receiver).ToList();
			}
			#endregion

			#endregion

			#region INVENTORY MOVE DETAIL

			if (!string.IsNullOrEmpty(numberLot) && numberLot != "Todos")
			{
				model = model.Where(o => o.Lot?.number.Contains(numberLot) ?? false).ToList();
			}

			if (!string.IsNullOrEmpty(internalNumberLot) && internalNumberLot != "Todos")
			{
				model = model.Where(o => o.Lot?.ProductionLot?.internalNumber.Contains(internalNumberLot) ?? false).ToList();
			}

			if (items != null && items.Length > 0)
			{
				var tempModel = new List<InventoryMoveDetail>();
				foreach (var inventoryMoveDetail in model)
				{
					if (items.Contains(inventoryMoveDetail.id_item))
					{
						tempModel.Add(inventoryMoveDetail);
					}
				}

				model = tempModel;
			}

			#endregion
				
			List<InventoryMoveDetailModelP> lstInvDetail = DataProviderInventoryMove.GetInventoryMoveDetail() as List<InventoryMoveDetailModelP>;
			List<DocumentModelP> lstDocumentLv = DataProviderDocument.GetDocumentModelP();
			List<DocumentModelP> lstDocument = lstDocumentLv
												.Select(s => new DocumentModelP
												{
													idDocumentModelP = s.idDocumentModelP,
													numberModelP = s.numberModelP,
													sequentialModelP = s.sequentialModelP,
													emissionDateModelP = s.emissionDateModelP,
													authorizationDateModelP = s.authorizationDateModelP,
													authorizationNumberModelP = s.authorizationNumberModelP,
													accessKeyModelP = s.accessKeyModelP,
													descriptionModelP = s.descriptionModelP,
													referenceModelP = s.referenceModelP,
													idEmissionPointModelP = s.idEmissionPointModelP,
													idDocumentTypeModelP = s.idDocumentTypeModelP,
													idDocumentStateModelP = s.idDocumentStateModelP,
													isOpenModelP = s.isOpenModelP
												}).ToList();

			List<InventoryMoveModelP> lstInvMov = DataProviderInventoryMove.GetInventoryMoves() as List<InventoryMoveModelP>;

			List<InventoryMoveDetailCabModelP> _lstInvDetailLv = (from _det in lstInvDetail
																  join _cab in lstDocumentLv on _det.id_inventoryMove equals _cab.idDocumentModelP
																  where _cab.idDocumentStateModelP == 3 && _det.id_inventoryMoveDetailNext == null
																  select new InventoryMoveDetailCabModelP
																  {
																	  id = _det.id,
																	  id_lot = _det.id_lot,
																	  id_item = _det.id_item,
																	  id_inventoryMove = _det.id_inventoryMove,
																	  emissionDate = _cab.emissionDateModelP,
																	  entryAmount = _det.entryAmount,
																	  entryAmountCost = _det.entryAmountCost,
																	  exitAmount = _det.exitAmount,
																	  exitAmountCost = _det.exitAmountCost,
																	  id_metricUnit = _det.id_metricUnit,
																	  id_warehouse = _det.id_warehouse,
																	  id_warehouseLocation = _det.id_warehouseLocation,
																	  id_warehouseEntry = _det.id_warehouseEntry,
																	  id_inventoryMoveDetailExit = _det.id_inventoryMoveDetailExit,
																	  inMaximumUnit = _det.inMaximumUnit,
																	  id_userCreate = _det.id_userCreate,
																	  dateCreate = _det.dateCreate,
																	  id_userUpdate = _det.id_userUpdate,
																	  dateUpdate = _det.dateUpdate,
																	  id_inventoryMoveDetailPrevious = _det.id_inventoryMoveDetailPrevious,
																	  id_inventoryMoveDetailNext = _det.id_inventoryMoveDetailNext,
																	  unitPrice = _det.unitPrice,
																	  balance = _det.balance,
																	  averagePrice = _det.averagePrice,
																	  balanceCost = _det.balanceCost,
																	  id_metricUnitMove = _det.id_metricUnitMove,
																	  unitPriceMove = _det.unitPriceMove,
																	  amountMove = _det.amountMove,
																	  id_costCenter = _det.id_costCenter,
																	  id_subCostCenter = _det.id_subCostCenter,
																	  natureSequential = _det.natureSequential
																  }).ToList();

			List<InventoryMoveDetailCabModelP> _lstInvDetailLvCopy = _lstInvDetailLv
																			.Select(s => new InventoryMoveDetailCabModelP
																			{
																				id = s.id,
																				id_lot = s.id_lot,
																				id_item = s.id_item,
																				id_inventoryMove = s.id_inventoryMove,
																				emissionDate = s.emissionDate,
																				entryAmount = s.entryAmount,
																				entryAmountCost = s.entryAmountCost,
																				exitAmount = s.exitAmount,
																				exitAmountCost = s.exitAmountCost,
																				id_metricUnit = s.id_metricUnit,
																				id_warehouse = s.id_warehouse,
																				id_warehouseLocation = s.id_warehouseLocation,
																				id_warehouseEntry = s.id_warehouseEntry,
																				id_inventoryMoveDetailExit = s.id_inventoryMoveDetailExit,
																				inMaximumUnit = s.inMaximumUnit,
																				id_userCreate = s.id_userCreate,
																				dateCreate = s.dateCreate,
																				id_userUpdate = s.id_userUpdate,
																				dateUpdate = s.dateUpdate,
																				id_inventoryMoveDetailPrevious = s.id_inventoryMoveDetailPrevious,
																				id_inventoryMoveDetailNext = s.id_inventoryMoveDetailNext,
																				unitPrice = s.unitPrice,
																				balance = s.balance,
																				averagePrice = s.averagePrice,
																				balanceCost = s.balanceCost,
																				id_metricUnitMove = s.id_metricUnitMove,
																				unitPriceMove = s.unitPriceMove,
																				amountMove = s.amountMove,
																				id_costCenter = s.id_costCenter,
																				id_subCostCenter = s.id_subCostCenter,
																				natureSequential = s.natureSequential
																			}).ToList();

			var tempModel1Aux = new List<InventoryMoveDetailCabModelP>();

			foreach (var inventoryMoveDetail in _lstInvDetailLv)
			{
				var inventoryMoveDetailAux = GetInventoryMoveDetailLessEqualOfFixed(endEmissionDate ?? DateTime.Now, inventoryMoveDetail, _lstInvDetailLvCopy);
				if (inventoryMoveDetailAux != null)
				{
					tempModel1Aux.Add(inventoryMoveDetailAux);
				}
			}

			var tempModel1AuxFixed = tempModel1Aux.Select(s => s.id).ToList();
			var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					var tempModel = new List<InventoryMoveDetail>();
					foreach (var item2 in model)
					{
						var inventoryMoveDetail = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item2.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null);
						if (inventoryMoveDetail != null)
						{
							tempModel.Add(item2);
						}
					}
					model = tempModel;
				}
			}

			#region FINAL COLLECTION

			List<WarehouseModelP> lstWarehouse = DataProviderWarehouse.QueryWarehouseModelP(db).ToList();
			List<WarehouseLocationModelP> lstWarehouseLocation = DataProviderWarehouse.GetWarehouseLocationModelP();
			List<InventoryReasonModelP> lstInvReason = DataProviderInventoryMove.GetInventoryMoveReason() as List<InventoryReasonModelP>;
			List<ItemModelP> lstItem = DataProviderItem.GetListItemModelP();
			List<DocumentTypeModelP> lstDocType = DataProviderDocumentType.GetDocumentTypes() as List<DocumentTypeModelP>;
			List<InventoryMoveModelP> lstInvMove = DataProviderInventoryMove.GetInventoryMoves() as List<InventoryMoveModelP>;
			List<MetricUnitModelP> lstMetUn = DataProviderItem.GetMetricUnit() as List<MetricUnitModelP>;
			List<ProductionLotSingleModelP> lstPlSm = DataProviderProductionLot.GetProductionLotSingle() as List<ProductionLotSingleModelP>;
			List<EmissionPointModelP> lstEmis = DataProviderEmissionPoint.GetEmissionPointsModelP() as List<EmissionPointModelP>;

			var queryDocSour = DataProviderDocumentSource.QueryDocumentsSource(db);
			var dtmp = DataProviderDocumentType.GetOneDocumentTypeByCode("08");
			var queryDocumentsFromRg = DataProviderDocument.QueryDocumentModelP(db, dtmp.idDocumentTypeModelP);
			var lstDocSourceFromRG = (from _lstDs in queryDocSour
									  join _lstDo in queryDocumentsFromRg on _lstDs.id_Document_Origin_P equals _lstDo.idDocumentModelP
									  select new
									  {
										  idDocument = _lstDs.id_Document_P,
										  seqRemissionGuide = _lstDo.sequentialModelP
									  }).ToList();

			List<InventoryMoveDetailModelP> lstInvDetail1 = model
				.Select(s => new InventoryMoveDetailModelP
				{
					id = s.id,
					id_lot = s.id_lot,
					id_item = s.id_item,
					id_inventoryMove = s.id_inventoryMove,
					entryAmount = s.entryAmount,
					entryAmountCost = s.entryAmountCost,
					exitAmount = s.exitAmount,
					exitAmountCost = s.exitAmountCost,
					id_metricUnit = s.id_metricUnit,
					id_warehouse = s.id_warehouse,
					id_warehouseLocation = s.id_warehouseLocation,
					id_warehouseEntry = s.id_warehouseEntry,
					id_inventoryMoveDetailExit = s.id_inventoryMoveDetailExit,
					inMaximumUnit = s.inMaximumUnit,
					id_userCreate = s.id_userCreate,
					dateCreate = s.dateCreate,
					id_userUpdate = s.id_userUpdate,
					dateUpdate = s.dateUpdate,
					id_inventoryMoveDetailPrevious = s.id_inventoryMoveDetailPrevious,
					id_inventoryMoveDetailNext = s.id_inventoryMoveDetailNext,
					unitPrice = s.unitPrice,
					balance = s.balance,
					averagePrice = s.averagePrice,
					balanceCost = s.balanceCost,
					id_metricUnitMove = s.id_metricUnitMove,
					unitPriceMove = s.unitPriceMove,
					amountMove = s.amountMove,
					id_costCenter = s.id_costCenter,
					id_subCostCenter = s.id_subCostCenter,
					natureSequential = s.natureSequential
				}).ToList();

			List<InventoryMoveDetailModelP> lstInvDetail2 = model
				.Select(s => new InventoryMoveDetailModelP
				{
					id = s.id,
					id_lot = s.id_lot,
					id_item = s.id_item,
					id_inventoryMove = s.id_inventoryMove,
					entryAmount = s.entryAmount,
					entryAmountCost = s.entryAmountCost,
					exitAmount = s.exitAmount,
					exitAmountCost = s.exitAmountCost,
					id_metricUnit = s.id_metricUnit,
					id_warehouse = s.id_warehouse,
					id_warehouseLocation = s.id_warehouseLocation,
					id_warehouseEntry = s.id_warehouseEntry,
					id_inventoryMoveDetailExit = s.id_inventoryMoveDetailExit,
					inMaximumUnit = s.inMaximumUnit,
					id_userCreate = s.id_userCreate,
					dateCreate = s.dateCreate,
					id_userUpdate = s.id_userUpdate,
					dateUpdate = s.dateUpdate,
					id_inventoryMoveDetailPrevious = s.id_inventoryMoveDetailPrevious,
					id_inventoryMoveDetailNext = s.id_inventoryMoveDetailNext,
					unitPrice = s.unitPrice,
					balance = s.balance,
					averagePrice = s.averagePrice,
					balanceCost = s.balanceCost,
					id_metricUnitMove = s.id_metricUnitMove,
					unitPriceMove = s.unitPriceMove,
					amountMove = s.amountMove,
					id_costCenter = s.id_costCenter,
					id_subCostCenter = s.id_subCostCenter,
					natureSequential = s.natureSequential
				}).ToList();

			List<InventoryMoveDetailModelP> lstInvDetail3 = model
					.Select(s => new InventoryMoveDetailModelP
					{
						id = s.id,
						id_lot = s.id_lot,
						id_item = s.id_item,
						id_inventoryMove = s.id_inventoryMove,
						entryAmount = s.entryAmount,
						entryAmountCost = s.entryAmountCost,
						exitAmount = s.exitAmount,
						exitAmountCost = s.exitAmountCost,
						id_metricUnit = s.id_metricUnit,
						id_warehouse = s.id_warehouse,
						id_warehouseLocation = s.id_warehouseLocation,
						id_warehouseEntry = s.id_warehouseEntry,
						id_inventoryMoveDetailExit = s.id_inventoryMoveDetailExit,
						inMaximumUnit = s.inMaximumUnit,
						id_userCreate = s.id_userCreate,
						dateCreate = s.dateCreate,
						id_userUpdate = s.id_userUpdate,
						dateUpdate = s.dateUpdate,
						id_inventoryMoveDetailPrevious = s.id_inventoryMoveDetailPrevious,
						id_inventoryMoveDetailNext = s.id_inventoryMoveDetailNext,
						unitPrice = s.unitPrice,
						balance = s.balance,
						averagePrice = s.averagePrice,
						balanceCost = s.balanceCost,
						id_metricUnitMove = s.id_metricUnitMove,
						unitPriceMove = s.unitPriceMove,
						amountMove = s.amountMove,
						id_costCenter = s.id_costCenter,
						id_subCostCenter = s.id_subCostCenter,
						natureSequential = s.natureSequential
					}).ToList();


			#endregion


			var modelAux = (from _lstDet1 in lstInvDetail1
							join _lstInM in lstInvMov on _lstDet1.id_inventoryMove equals _lstInM.idInventoryMoveModelP
							join _lstCab1 in lstDocument on _lstDet1.id_inventoryMove equals _lstCab1.idDocumentModelP
							join _lstEmp in lstEmis on _lstCab1.idEmissionPointModelP equals _lstEmp.idEmissionPointModelP
							join _lstIt in lstItem on _lstDet1.id_item equals _lstIt.idModelP
							join _lstMu in lstMetUn on _lstDet1.id_metricUnit equals _lstMu.idMetricUnitModelP
							join _lstInMoCab in lstInvMove on _lstCab1.idDocumentModelP equals _lstInMoCab.idInventoryMoveModelP
							join _lstPl in lstPlSm on _lstInMoCab.idProductionLot equals _lstPl.idProductionLotSingleModelP into _lstPlLOJ
							from _lstPl in _lstPlLOJ.DefaultIfEmpty()
							join _lstDet1W in lstWarehouse on _lstDet1.id_warehouse equals _lstDet1W.idWarehouseModelP into _bode
							from _lstDet1W in _bode.DefaultIfEmpty()
							join _lstDet1WE in lstWarehouse on _lstDet1.id_warehouseEntry equals _lstDet1WE.idWarehouseModelP into _bodeE
							from _lstDet1WE in _bode.DefaultIfEmpty()
							join _lstDet1WL in lstWarehouseLocation on _lstDet1.id_warehouseLocation equals _lstDet1WL.idWarehouseLocationModelP into _ubi
							from _lstDet1WL in _ubi.DefaultIfEmpty()
							join _lstInRe in lstInvReason on _lstInMoCab.idInventoryReasonModelP equals _lstInRe.idInventoryReasonModelP
							join _lstDt1 in lstDocType on _lstCab1.idDocumentTypeModelP equals _lstDt1.idDocumentTypeModelP
							join _lstRG in lstDocSourceFromRG on _lstInMoCab.idInventoryMoveModelP equals _lstRG.idDocument into _gui
							from _lstRG in _gui.DefaultIfEmpty()
							select new ResultKardex
							{
								id = _lstDet1.id,
								id_document = _lstDet1.id_inventoryMove,
								document = _lstInM.natureSequential,
								id_documentType = _lstCab1.idDocumentTypeModelP,
								documentType = _lstDt1.nameModelP,
								id_inventoryReason = _lstInMoCab.idInventoryReasonModelP,
								inventoryReason = _lstInRe.nameInventoryReasonModelP,
								emissionDate = _lstCab1.emissionDateModelP,
								id_item = _lstDet1.id_item,
								code_item = _lstIt.masterCodeModelP + " - " + _lstIt.nameModelP,
								id_metricUnit = _lstDet1.id_metricUnit,
								metricUnit = _lstMu.codeMetricUnitModelP,
								id_lot = _lstDet1.id_lot,
								number = (_lstPl != null) ? _lstPl.numberProductionLotSingleModelP + "-" + _lstPl.internalNumberProductionLotSingleModelP : "",
								internalNumber = (_lstPl != null) ? _lstPl.internalNumberProductionLotSingleModelP : "",
								id_warehouse = _lstDet1.id_warehouse,
								warehouse = (_lstDet1W != null) ? _lstDet1W.nameWarehouseModelP : "",
								id_warehouseLocation = _lstDet1.id_warehouseLocation,
								warehouseLocation = (_lstDet1WL != null) ? _lstDet1WL.nameWarehouseLocationModelP : "",
								id_warehouseExit = 1,
								warehouseExit = "",
								id_warehouseLocationExit = 2,
								warehouseLocationExit = "",
								id_warehouseEntry = 2,
								warehouseEntry = "",
								id_warehouseLocationEntry = (_lstDet1.entryAmount > 0) ? _lstDet1.id_warehouseLocation : null,
								warehouseLocationEntry = (_lstDet1WL != null) ? ((_lstDet1.entryAmount > 0) ? _lstDet1WL.nameWarehouseLocationModelP : "") : "",
								priceCost = _lstDet1.unitPrice,
								previousBalance = _lstDet1.balance - _lstDet1.entryAmount + _lstDet1.exitAmount,
								previousBalanceCost = Convert.ToDecimal(0),
								entry = _lstDet1.entryAmount,
								entryCost = _lstDet1.entryAmountCost,
								exit = _lstDet1.exitAmount,
								exitCost = _lstDet1.exitAmountCost,
								balance = _lstDet1.balance,
								balanceCost = _lstDet1.balanceCost,
								balanceCutting = tempModel1AuxFixed.Contains(_lstDet1.id) ? _lstDet1.balance : 0,
								balanceCuttingCost = tempModel1AuxFixed.Contains(_lstDet1.id) ? _lstDet1.balanceCost : 0,
								numberRemissionGuide = (_lstRG != null) ? _lstRG.seqRemissionGuide.ToString() : "",
								idCompany = _lstEmp.idCompanyModelP
							}).ToList();


			TempData["modelKardex"] = modelAux;
			TempData.Keep("modelKardex");

			ViewData["settingGridViewKardex"] = tempKardexFilter.settingGridViewKardex;

			//return PartialView("_KardexResults", modelAux.ToList());
			return PartialView("_KardexResults", modelAux.OrderByDescending(o => o.emissionDate).ThenByDescending(i => i.dateCreate).ToList());
		}

		[HttpPost]
		public ActionResult KardexResultsFixedVersion(InventoryMove inventoryMove,
								  InventoryEntryMove entryMove,
								  InventoryExitMove exitMove,
								  Document document,
								  DateTime? startEmissionDate, DateTime? endEmissionDate,
								  string numberLot, string internalNumberLot, int[] items)
		{
			var tempKardexFilter = TempData["KardexFilter"] as KardexFilter;

			inventoryMove.InventoryEntryMove = entryMove;
			inventoryMove.InventoryExitMove = exitMove;
			inventoryMove.Document = document;
			tempKardexFilter = new KardexFilter
			{
				inventoryMoveDetail = new InventoryMoveDetail
				{
					InventoryMove = inventoryMove,
					Lot = new Lot
					{
						number = numberLot,
						ProductionLot = new ProductionLot
						{
							internalNumber = internalNumberLot
						}
					},
				},
				startEmissionDate = startEmissionDate,
				endEmissionDate = endEmissionDate,
				items = items
			};


			TempData["KardexFilter"] = tempKardexFilter;
			TempData.Keep("KardexFilter");

			var model = db.InventoryMoveDetail.Where(w => w.InventoryMove.Document.DocumentState.code.Equals("03") ||//APROBADA
														 w.InventoryMove.Document.DocumentState.code.Equals("07")).ToList();//REVERSADA


			#region DOCUMENT FILTERS

			if (document.id_documentType != 0)
			{
				model = model.Where(o => o.InventoryMove.Document.id_documentType == document.id_documentType).ToList();
			}

			if (!string.IsNullOrEmpty(document.number) && document.number != "Todos")
			{
				model = model.Where(o => o.InventoryMove.Document.number.Contains(document.number)).ToList();
			}

			if (!string.IsNullOrEmpty(document.reference) && document.reference != "Todas")
			{
				model = model.Where(o => o.InventoryMove.Document.reference.Contains(document.reference)).ToList();
			}

			if (startEmissionDate != null && endEmissionDate != null)
			{
				model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.InventoryMove.Document.emissionDate.Date) <= 0 && DateTime.Compare(o.InventoryMove.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
			}

			#endregion

			#region INVENTORY ENTRY MOVE

			if (inventoryMove.id_inventoryReason != 0 && inventoryMove.id_inventoryReason != null)
			{
				model = model.Where(o => o.InventoryMove.id_inventoryReason == inventoryMove.id_inventoryReason).ToList();
			}

			#region InventoryExitMove
			if (exitMove.id_warehouseExit != 0 && exitMove.id_warehouseExit != null)
			{
				model = model.Where(o => o.InventoryMove.InventoryExitMove?.id_warehouseExit == exitMove.id_warehouseExit).ToList();
			}

			if (exitMove.id_warehouseLocationExit != 0 && exitMove.id_warehouseLocationExit != null)
			{
				model = model.Where(o => o.InventoryMove.InventoryExitMove?.id_warehouseLocationExit == exitMove.id_warehouseLocationExit).ToList();
			}

			if (exitMove.id_dispatcher != 0)
			{
				model = model.Where(o => o.InventoryMove.InventoryExitMove?.id_dispatcher == exitMove.id_dispatcher).ToList();
			}
			#endregion

			#region InventoryEntryMove
			if (entryMove.id_warehouseEntry != 0 && entryMove.id_warehouseEntry != null)
			{
				model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_warehouseEntry == entryMove.id_warehouseEntry).ToList();
			}

			if (entryMove.id_warehouseLocationEntry != 0 && entryMove.id_warehouseLocationEntry != null)
			{
				model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_warehouseLocationEntry == entryMove.id_warehouseLocationEntry).ToList();
			}

			if (entryMove.id_receiver != 0)
			{
				model = model.Where(o => o.InventoryMove.InventoryEntryMove?.id_receiver == entryMove.id_receiver).ToList();
			}
			#endregion

			#endregion

			#region INVENTORY MOVE DETAIL

			if (!string.IsNullOrEmpty(numberLot) && numberLot != "Todos")
			{
				model = model.Where(o => o.Lot?.number.Contains(numberLot) ?? false).ToList();
			}

			if (!string.IsNullOrEmpty(internalNumberLot) && internalNumberLot != "Todos")
			{
				model = model.Where(o => o.Lot?.ProductionLot?.internalNumber.Contains(internalNumberLot) ?? false).ToList();
			}

			if (items != null && items.Length > 0)
			{
				var tempModel = new List<InventoryMoveDetail>();
				foreach (var inventoryMoveDetail in model)
				{
					if (items.Contains(inventoryMoveDetail.id_item))
					{
						tempModel.Add(inventoryMoveDetail);
					}
				}

				model = tempModel;
			}

			#endregion

			List<InventoryMoveDetailModelP> lstInvDetail = DataProviderInventoryMove.GetInventoryMoveDetail() as List<InventoryMoveDetailModelP>;
			List<DocumentModelP> lstDocumentLv = DataProviderDocument.GetDocumentModelP();
			List<DocumentModelP> lstDocument = lstDocumentLv
												.Select(s => new DocumentModelP
												{
													idDocumentModelP = s.idDocumentModelP,
													numberModelP = s.numberModelP,
													sequentialModelP = s.sequentialModelP,
													emissionDateModelP = s.emissionDateModelP,
													authorizationDateModelP = s.authorizationDateModelP,
													authorizationNumberModelP = s.authorizationNumberModelP,
													accessKeyModelP = s.accessKeyModelP,
													descriptionModelP = s.descriptionModelP,
													referenceModelP = s.referenceModelP,
													idEmissionPointModelP = s.idEmissionPointModelP,
													idDocumentTypeModelP = s.idDocumentTypeModelP,
													idDocumentStateModelP = s.idDocumentStateModelP,
													isOpenModelP = s.isOpenModelP
												}).ToList();

			List<InventoryMoveModelP> lstInvMov = DataProviderInventoryMove.GetInventoryMoves() as List<InventoryMoveModelP>;

			List<InventoryMoveDetailCabModelP> _lstInvDetailLv = (from _det in lstInvDetail
																  join _cab in lstDocumentLv on _det.id_inventoryMove equals _cab.idDocumentModelP
																  where _cab.idDocumentStateModelP == 3 && _det.id_inventoryMoveDetailNext == null
																  select new InventoryMoveDetailCabModelP
																  {
																	  id = _det.id,
																	  id_lot = _det.id_lot,
																	  id_item = _det.id_item,
																	  id_inventoryMove = _det.id_inventoryMove,
																	  emissionDate = _cab.emissionDateModelP,
																	  entryAmount = _det.entryAmount,
																	  entryAmountCost = _det.entryAmountCost,
																	  exitAmount = _det.exitAmount,
																	  exitAmountCost = _det.exitAmountCost,
																	  id_metricUnit = _det.id_metricUnit,
																	  id_warehouse = _det.id_warehouse,
																	  id_warehouseLocation = _det.id_warehouseLocation,
																	  id_warehouseEntry = _det.id_warehouseEntry,
																	  id_inventoryMoveDetailExit = _det.id_inventoryMoveDetailExit,
																	  inMaximumUnit = _det.inMaximumUnit,
																	  id_userCreate = _det.id_userCreate,
																	  dateCreate = _det.dateCreate,
																	  id_userUpdate = _det.id_userUpdate,
																	  dateUpdate = _det.dateUpdate,
																	  id_inventoryMoveDetailPrevious = _det.id_inventoryMoveDetailPrevious,
																	  id_inventoryMoveDetailNext = _det.id_inventoryMoveDetailNext,
																	  unitPrice = _det.unitPrice,
																	  balance = _det.balance,
																	  averagePrice = _det.averagePrice,
																	  balanceCost = _det.balanceCost,
																	  id_metricUnitMove = _det.id_metricUnitMove,
																	  unitPriceMove = _det.unitPriceMove,
																	  amountMove = _det.amountMove,
																	  id_costCenter = _det.id_costCenter,
																	  id_subCostCenter = _det.id_subCostCenter,
																	  natureSequential = _det.natureSequential
																  }).ToList();

			List<InventoryMoveDetailCabModelP> _lstInvDetailLvCopy = _lstInvDetailLv
																			.Select(s => new InventoryMoveDetailCabModelP
																			{
																				id = s.id,
																				id_lot = s.id_lot,
																				id_item = s.id_item,
																				id_inventoryMove = s.id_inventoryMove,
																				emissionDate = s.emissionDate,
																				entryAmount = s.entryAmount,
																				entryAmountCost = s.entryAmountCost,
																				exitAmount = s.exitAmount,
																				exitAmountCost = s.exitAmountCost,
																				id_metricUnit = s.id_metricUnit,
																				id_warehouse = s.id_warehouse,
																				id_warehouseLocation = s.id_warehouseLocation,
																				id_warehouseEntry = s.id_warehouseEntry,
																				id_inventoryMoveDetailExit = s.id_inventoryMoveDetailExit,
																				inMaximumUnit = s.inMaximumUnit,
																				id_userCreate = s.id_userCreate,
																				dateCreate = s.dateCreate,
																				id_userUpdate = s.id_userUpdate,
																				dateUpdate = s.dateUpdate,
																				id_inventoryMoveDetailPrevious = s.id_inventoryMoveDetailPrevious,
																				id_inventoryMoveDetailNext = s.id_inventoryMoveDetailNext,
																				unitPrice = s.unitPrice,
																				balance = s.balance,
																				averagePrice = s.averagePrice,
																				balanceCost = s.balanceCost,
																				id_metricUnitMove = s.id_metricUnitMove,
																				unitPriceMove = s.unitPriceMove,
																				amountMove = s.amountMove,
																				id_costCenter = s.id_costCenter,
																				id_subCostCenter = s.id_subCostCenter,
																				natureSequential = s.natureSequential
																			}).ToList();

			var tempModel1Aux = new List<InventoryMoveDetailCabModelP>();

			foreach (var inventoryMoveDetail in _lstInvDetailLv)
			{
				var inventoryMoveDetailAux = GetInventoryMoveDetailLessEqualOfFixed(endEmissionDate ?? DateTime.Now, inventoryMoveDetail, _lstInvDetailLvCopy);
				if (inventoryMoveDetailAux != null)
				{
					tempModel1Aux.Add(inventoryMoveDetailAux);
				}
			}

			var tempModel1AuxFixed = tempModel1Aux.Select(s => s.id).ToList();
			var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					var tempModel = new List<InventoryMoveDetail>();
					foreach (var item2 in model)
					{
						var inventoryMoveDetail = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item2.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null);
						if (inventoryMoveDetail != null)
						{
							tempModel.Add(item2);
						}
					}
					model = tempModel;
				}
			}

			#region FINAL COLLECTION

			List<WarehouseModelP> lstWarehouse = DataProviderWarehouse.QueryWarehouseModelP(db).ToList();
			List<WarehouseLocationModelP> lstWarehouseLocation = DataProviderWarehouse.GetWarehouseLocationModelP();
			List<InventoryReasonModelP> lstInvReason = DataProviderInventoryMove.GetInventoryMoveReason() as List<InventoryReasonModelP>;
			List<ItemModelP> lstItem = DataProviderItem.GetListItemModelP();
			List<DocumentTypeModelP> lstDocType = DataProviderDocumentType.GetDocumentTypes() as List<DocumentTypeModelP>;
			List<InventoryMoveModelP> lstInvMove = DataProviderInventoryMove.GetInventoryMoves() as List<InventoryMoveModelP>;
			List<MetricUnitModelP> lstMetUn = DataProviderItem.GetMetricUnit() as List<MetricUnitModelP>;
			List<ProductionLotSingleModelP> lstPlSm = DataProviderProductionLot.GetProductionLotSingle() as List<ProductionLotSingleModelP>;
			List<EmissionPointModelP> lstEmis = DataProviderEmissionPoint.GetEmissionPointsModelP() as List<EmissionPointModelP>;

			var queryDocSour = DataProviderDocumentSource.QueryDocumentsSource(db);
			var dtmp = DataProviderDocumentType.GetOneDocumentTypeByCode("08");
			var queryDocumentsFromRg = DataProviderDocument.QueryDocumentModelP(db, dtmp.idDocumentTypeModelP);
			var lstDocSourceFromRG = (from _lstDs in queryDocSour
									  join _lstDo in queryDocumentsFromRg on _lstDs.id_Document_Origin_P equals _lstDo.idDocumentModelP
									  select new
									  {
										  idDocument = _lstDs.id_Document_P,
										  seqRemissionGuide = _lstDo.sequentialModelP
									  }).ToList();

			List<InventoryMoveDetailModelP> lstInvDetail1 = model
				.Select(s => new InventoryMoveDetailModelP
				{
					id = s.id,
					id_lot = s.id_lot,
					id_item = s.id_item,
					id_inventoryMove = s.id_inventoryMove,
					entryAmount = s.entryAmount,
					entryAmountCost = s.entryAmountCost,
					exitAmount = s.exitAmount,
					exitAmountCost = s.exitAmountCost,
					id_metricUnit = s.id_metricUnit,
					id_warehouse = s.id_warehouse,
					id_warehouseLocation = s.id_warehouseLocation,
					id_warehouseEntry = s.id_warehouseEntry,
					id_inventoryMoveDetailExit = s.id_inventoryMoveDetailExit,
					inMaximumUnit = s.inMaximumUnit,
					id_userCreate = s.id_userCreate,
					dateCreate = s.dateCreate,
					id_userUpdate = s.id_userUpdate,
					dateUpdate = s.dateUpdate,
					id_inventoryMoveDetailPrevious = s.id_inventoryMoveDetailPrevious,
					id_inventoryMoveDetailNext = s.id_inventoryMoveDetailNext,
					unitPrice = s.unitPrice,
					balance = s.balance,
					averagePrice = s.averagePrice,
					balanceCost = s.balanceCost,
					id_metricUnitMove = s.id_metricUnitMove,
					unitPriceMove = s.unitPriceMove,
					amountMove = s.amountMove,
					id_costCenter = s.id_costCenter,
					id_subCostCenter = s.id_subCostCenter,
					natureSequential = s.natureSequential
				}).ToList();

			List<InventoryMoveDetailModelP> lstInvDetail2 = model
				.Select(s => new InventoryMoveDetailModelP
				{
					id = s.id,
					id_lot = s.id_lot,
					id_item = s.id_item,
					id_inventoryMove = s.id_inventoryMove,
					entryAmount = s.entryAmount,
					entryAmountCost = s.entryAmountCost,
					exitAmount = s.exitAmount,
					exitAmountCost = s.exitAmountCost,
					id_metricUnit = s.id_metricUnit,
					id_warehouse = s.id_warehouse,
					id_warehouseLocation = s.id_warehouseLocation,
					id_warehouseEntry = s.id_warehouseEntry,
					id_inventoryMoveDetailExit = s.id_inventoryMoveDetailExit,
					inMaximumUnit = s.inMaximumUnit,
					id_userCreate = s.id_userCreate,
					dateCreate = s.dateCreate,
					id_userUpdate = s.id_userUpdate,
					dateUpdate = s.dateUpdate,
					id_inventoryMoveDetailPrevious = s.id_inventoryMoveDetailPrevious,
					id_inventoryMoveDetailNext = s.id_inventoryMoveDetailNext,
					unitPrice = s.unitPrice,
					balance = s.balance,
					averagePrice = s.averagePrice,
					balanceCost = s.balanceCost,
					id_metricUnitMove = s.id_metricUnitMove,
					unitPriceMove = s.unitPriceMove,
					amountMove = s.amountMove,
					id_costCenter = s.id_costCenter,
					id_subCostCenter = s.id_subCostCenter,
					natureSequential = s.natureSequential
				}).ToList();

			List<InventoryMoveDetailModelP> lstInvDetail3 = model
					.Select(s => new InventoryMoveDetailModelP
					{
						id = s.id,
						id_lot = s.id_lot,
						id_item = s.id_item,
						id_inventoryMove = s.id_inventoryMove,
						entryAmount = s.entryAmount,
						entryAmountCost = s.entryAmountCost,
						exitAmount = s.exitAmount,
						exitAmountCost = s.exitAmountCost,
						id_metricUnit = s.id_metricUnit,
						id_warehouse = s.id_warehouse,
						id_warehouseLocation = s.id_warehouseLocation,
						id_warehouseEntry = s.id_warehouseEntry,
						id_inventoryMoveDetailExit = s.id_inventoryMoveDetailExit,
						inMaximumUnit = s.inMaximumUnit,
						id_userCreate = s.id_userCreate,
						dateCreate = s.dateCreate,
						id_userUpdate = s.id_userUpdate,
						dateUpdate = s.dateUpdate,
						id_inventoryMoveDetailPrevious = s.id_inventoryMoveDetailPrevious,
						id_inventoryMoveDetailNext = s.id_inventoryMoveDetailNext,
						unitPrice = s.unitPrice,
						balance = s.balance,
						averagePrice = s.averagePrice,
						balanceCost = s.balanceCost,
						id_metricUnitMove = s.id_metricUnitMove,
						unitPriceMove = s.unitPriceMove,
						amountMove = s.amountMove,
						id_costCenter = s.id_costCenter,
						id_subCostCenter = s.id_subCostCenter,
						natureSequential = s.natureSequential
					}).ToList();


			#endregion

			var modelAux = (from _lstDet1 in lstInvDetail1
							join _lstInM in lstInvMov on _lstDet1.id_inventoryMove equals _lstInM.idInventoryMoveModelP
							join _lstCab1 in lstDocument on _lstDet1.id_inventoryMove equals _lstCab1.idDocumentModelP
							join _lstEmp in lstEmis on _lstCab1.idEmissionPointModelP equals _lstEmp.idEmissionPointModelP
							join _lstIt in lstItem on _lstDet1.id_item equals _lstIt.idModelP
							join _lstMu in lstMetUn on _lstDet1.id_metricUnit equals _lstMu.idMetricUnitModelP
							join _lstInMoCab in lstInvMove on _lstCab1.idDocumentModelP equals _lstInMoCab.idInventoryMoveModelP
							join _lstPl in lstPlSm on _lstInMoCab.idProductionLot equals _lstPl.idProductionLotSingleModelP into _lstPlLOJ
							from _lstPl in _lstPlLOJ.DefaultIfEmpty()
							join _lstDet1W in lstWarehouse on _lstDet1.id_warehouse equals _lstDet1W.idWarehouseModelP into _bode
							from _lstDet1W in _bode.DefaultIfEmpty()
							join _lstDet1WE in lstWarehouse on _lstDet1.id_warehouseEntry equals _lstDet1WE.idWarehouseModelP into _bodeE
							from _lstDet1WE in _bode.DefaultIfEmpty()
							join _lstDet1WL in lstWarehouseLocation on _lstDet1.id_warehouseLocation equals _lstDet1WL.idWarehouseLocationModelP into _ubi
							from _lstDet1WL in _ubi.DefaultIfEmpty()
							join _lstInRe in lstInvReason on _lstInMoCab.idInventoryReasonModelP equals _lstInRe.idInventoryReasonModelP
							join _lstDt1 in lstDocType on _lstCab1.idDocumentTypeModelP equals _lstDt1.idDocumentTypeModelP
							join _lstRG in lstDocSourceFromRG on _lstInMoCab.idInventoryMoveModelP equals _lstRG.idDocument into _gui
							from _lstRG in _gui.DefaultIfEmpty()
							select new ResultKardex
							{
								id = _lstDet1.id,
								id_document = _lstDet1.id_inventoryMove,
								document = _lstInM.natureSequential,
								id_documentType = _lstCab1.idDocumentTypeModelP,
								documentType = _lstDt1.nameModelP,
								id_inventoryReason = _lstInMoCab.idInventoryReasonModelP,
								inventoryReason = _lstInRe.nameInventoryReasonModelP,
								emissionDate = _lstCab1.emissionDateModelP,
								id_item = _lstDet1.id_item,
								code_item = _lstIt.masterCodeModelP + " - " + _lstIt.nameModelP,
								id_metricUnit = _lstDet1.id_metricUnit,
								metricUnit = _lstMu.codeMetricUnitModelP,
								id_lot = _lstDet1.id_lot,
								number = (_lstPl != null) ? _lstPl.numberProductionLotSingleModelP + "-" + _lstPl.internalNumberProductionLotSingleModelP : "",
								internalNumber = (_lstPl != null) ? _lstPl.internalNumberProductionLotSingleModelP : "",
								id_warehouse = _lstDet1.id_warehouse,
								warehouse = (_lstDet1W != null) ? _lstDet1W.nameWarehouseModelP : "",
								id_warehouseLocation = _lstDet1.id_warehouseLocation,
								warehouseLocation = (_lstDet1WL != null) ? _lstDet1WL.nameWarehouseLocationModelP : "",
								id_warehouseExit = 1,
								warehouseExit = "",
								id_warehouseLocationExit = 2,
								warehouseLocationExit = "",
								id_warehouseEntry = 2,
								warehouseEntry = "",
								id_warehouseLocationEntry = (_lstDet1.entryAmount > 0) ? _lstDet1.id_warehouseLocation : null,
								warehouseLocationEntry = (_lstDet1WL != null) ? ((_lstDet1.entryAmount > 0) ? _lstDet1WL.nameWarehouseLocationModelP : "") : "",
								priceCost = _lstDet1.unitPrice,
								previousBalance = _lstDet1.balance - _lstDet1.entryAmount + _lstDet1.exitAmount,
								previousBalanceCost = Convert.ToDecimal(0),
								entry = _lstDet1.entryAmount,
								entryCost = _lstDet1.entryAmountCost,
								exit = _lstDet1.exitAmount,
								exitCost = _lstDet1.exitAmountCost,
								balance = _lstDet1.balance,
								balanceCost = _lstDet1.balanceCost,
								balanceCutting = tempModel1AuxFixed.Contains(_lstDet1.id) ? _lstDet1.balance : 0,
								balanceCuttingCost = tempModel1AuxFixed.Contains(_lstDet1.id) ? _lstDet1.balanceCost : 0,
								numberRemissionGuide = (_lstRG != null) ? _lstRG.seqRemissionGuide.ToString() : "",
								idCompany = _lstEmp.idCompanyModelP
							}).ToList();

			TempData["modelKardex"] = modelAux;
			TempData.Keep("modelKardex");

			ViewData["settingGridViewKardex"] = tempKardexFilter.settingGridViewKardex;

			return PartialView("_KardexResults", modelAux.OrderByDescending(o => o.emissionDate).ThenByDescending(i => i.dateCreate).ToList());
		}

		[HttpPost]
		public ActionResult KardexResultsFixedVersionOptimized(InventoryMove inventoryMove,
						  InventoryEntryMove entryMove,
						  InventoryExitMove exitMove,
						  Document document,
						  DateTime? startEmissionDateFinal, DateTime? endEmissionDateFinal,
						  string numberLot, string internalNumberLot, string lotMarked, int[] items)
		{
			var tempKardexFilter = TempData["KardexFilter"] as KardexFilter;

			inventoryMove.InventoryEntryMove = entryMove;
			inventoryMove.InventoryExitMove = exitMove;
			inventoryMove.Document = document;
			tempKardexFilter = new KardexFilter
			{
				inventoryMoveDetail = new InventoryMoveDetail
				{
					InventoryMove = inventoryMove,
					Lot = new Lot
					{
						number = numberLot,
						ProductionLot = new ProductionLot
						{
							internalNumber = internalNumberLot
						}
					},
				},
				startEmissionDate = startEmissionDateFinal,
				endEmissionDate = endEmissionDateFinal,
				items = items
			};


			TempData["KardexFilter"] = tempKardexFilter;
			TempData.Keep("KardexFilter");

			Parametros.ParametrosBusquedaKardexSaldo parametrosBusquedaKardexSaldo = new Parametros.ParametrosBusquedaKardexSaldo();
			parametrosBusquedaKardexSaldo.id_documentType = document.id_documentType;
			if (!string.IsNullOrEmpty(document.number)) parametrosBusquedaKardexSaldo.number = document.number;
			if (!string.IsNullOrEmpty(document.reference)) parametrosBusquedaKardexSaldo.reference = document.reference;
			parametrosBusquedaKardexSaldo.startEmissionDate = startEmissionDateFinal;
			parametrosBusquedaKardexSaldo.endEmissionDate = endEmissionDateFinal;
			if (inventoryMove.idNatureMove != null) parametrosBusquedaKardexSaldo.idNatureMove = inventoryMove.idNatureMove;
			if (inventoryMove.id_inventoryReason != null) parametrosBusquedaKardexSaldo.id_inventoryReason = inventoryMove.id_inventoryReason;
			if (exitMove.id_warehouseExit != null) parametrosBusquedaKardexSaldo.id_warehouseExit = exitMove.id_warehouseExit;
			if (exitMove.id_warehouseLocationExit != null) parametrosBusquedaKardexSaldo.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
			parametrosBusquedaKardexSaldo.id_dispatcher = exitMove.id_dispatcher;
			if (entryMove.id_warehouseEntry != null) parametrosBusquedaKardexSaldo.id_warehouseEntry = entryMove.id_warehouseEntry;
			if (entryMove.id_warehouseLocationEntry != null) parametrosBusquedaKardexSaldo.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
			parametrosBusquedaKardexSaldo.id_receiver = entryMove.id_receiver;
			if (!string.IsNullOrEmpty(numberLot)) parametrosBusquedaKardexSaldo.numberLot = numberLot;
			if (!string.IsNullOrEmpty(internalNumberLot)) parametrosBusquedaKardexSaldo.internalNumberLot = internalNumberLot;
            if (!string.IsNullOrEmpty(lotMarked)) parametrosBusquedaKardexSaldo.lotMarked = lotMarked;
            if (items != null && items.Length > 0)
			{
				parametrosBusquedaKardexSaldo.items = items[0].ToString();
				for (int i = 1; i < items.Length; i++)
				{
					parametrosBusquedaKardexSaldo.items += "," + items[i].ToString();
				}
			}
			parametrosBusquedaKardexSaldo.id_user = ActiveUser.id;

			var parametrosBusquedaKardexSaldoAux = new SqlParameter();
			parametrosBusquedaKardexSaldoAux.ParameterName = "@ParametrosBusquedaKardexSaldo";
			parametrosBusquedaKardexSaldoAux.Direction = ParameterDirection.Input;
			parametrosBusquedaKardexSaldoAux.SqlDbType = SqlDbType.NVarChar;
			var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaKardexSaldo);
			parametrosBusquedaKardexSaldoAux.Value = jsonAux;
			db.Database.CommandTimeout = 3600;
			List<ResultKardex> modelAux = db.Database.SqlQuery<ResultKardex>("exec inv_Consultar_Kardex_Saldo_StoredProcedure @ParametrosBusquedaKardexSaldo ", parametrosBusquedaKardexSaldoAux).ToList();

			TempData["modelKardex"] = modelAux;
			TempData.Keep("modelKardex");

			ViewData["settingGridViewKardex"] = tempKardexFilter.settingGridViewKardex;

			return PartialView("_KardexResults", modelAux.ToList());
		}

        [HttpPost]
        public ActionResult KardexResultsFixedVersionOptimizedExcel(InventoryMove inventoryMove,
                          InventoryEntryMove entryMove,
                          InventoryExitMove exitMove,
                          Document document,
                          DateTime? startEmissionDateFinal, DateTime? endEmissionDateFinal,
                          string numberLot, string internalNumberLot, string lotMarked, int[] items)
        {
            var tempKardexFilter = TempData["KardexFilter"] as KardexFilter;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            inventoryMove.InventoryEntryMove = entryMove;
            inventoryMove.InventoryExitMove = exitMove;
            inventoryMove.Document = document;
            tempKardexFilter = new KardexFilter
            {
                inventoryMoveDetail = new InventoryMoveDetail
                {
                    InventoryMove = inventoryMove,
                    Lot = new Lot
                    {
                        number = numberLot,
                        ProductionLot = new ProductionLot
                        {
                            internalNumber = internalNumberLot
                        }
                    },
                },
                startEmissionDate = startEmissionDateFinal,
                endEmissionDate = endEmissionDateFinal,
                items = items
            };


            TempData["KardexFilter"] = tempKardexFilter;
            TempData.Keep("KardexFilter");

            Parametros.ParametrosBusquedaKardexSaldo parametrosBusquedaKardexSaldo = new Parametros.ParametrosBusquedaKardexSaldo();
            parametrosBusquedaKardexSaldo.id_documentType = document.id_documentType;
            if (!string.IsNullOrEmpty(document.number)) parametrosBusquedaKardexSaldo.number = document.number;
            if (!string.IsNullOrEmpty(document.reference)) parametrosBusquedaKardexSaldo.reference = document.reference;
            parametrosBusquedaKardexSaldo.startEmissionDate = startEmissionDateFinal;
            parametrosBusquedaKardexSaldo.endEmissionDate = endEmissionDateFinal;
            if (inventoryMove.idNatureMove != null) parametrosBusquedaKardexSaldo.idNatureMove = inventoryMove.idNatureMove;
            if (inventoryMove.id_inventoryReason != null) parametrosBusquedaKardexSaldo.id_inventoryReason = inventoryMove.id_inventoryReason;
            if (exitMove.id_warehouseExit != null) parametrosBusquedaKardexSaldo.id_warehouseExit = exitMove.id_warehouseExit;
            if (exitMove.id_warehouseLocationExit != null) parametrosBusquedaKardexSaldo.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
            parametrosBusquedaKardexSaldo.id_dispatcher = exitMove.id_dispatcher;
            if (entryMove.id_warehouseEntry != null) parametrosBusquedaKardexSaldo.id_warehouseEntry = entryMove.id_warehouseEntry;
            if (entryMove.id_warehouseLocationEntry != null) parametrosBusquedaKardexSaldo.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
            parametrosBusquedaKardexSaldo.id_receiver = entryMove.id_receiver;
            if (!string.IsNullOrEmpty(numberLot)) parametrosBusquedaKardexSaldo.numberLot = numberLot;
            if (!string.IsNullOrEmpty(internalNumberLot)) parametrosBusquedaKardexSaldo.internalNumberLot = internalNumberLot;
            if (!string.IsNullOrEmpty(lotMarked)) parametrosBusquedaKardexSaldo.lotMarked = lotMarked;
            if (items != null && items.Length > 0)
            {
                parametrosBusquedaKardexSaldo.items = items[0].ToString();
                for (int i = 1; i < items.Length; i++)
                {
                    parametrosBusquedaKardexSaldo.items += "," + items[i].ToString();
                }
            }
			parametrosBusquedaKardexSaldo.movAnulados = "S";
            parametrosBusquedaKardexSaldo.id_user = ActiveUser.id;

            var parametrosBusquedaKardexSaldoAux = new SqlParameter();
            parametrosBusquedaKardexSaldoAux.ParameterName = "@ParametrosBusquedaKardexSaldo";
            parametrosBusquedaKardexSaldoAux.Direction = ParameterDirection.Input;
            parametrosBusquedaKardexSaldoAux.SqlDbType = SqlDbType.NVarChar;
            var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaKardexSaldo);
            parametrosBusquedaKardexSaldoAux.Value = jsonAux;
            db.Database.CommandTimeout = 3600;
			List<ResultKardex> modelAux = null;

            try
			{
                modelAux = db.Database.SqlQuery<ResultKardex>("exec inv_Consultar_Kardex_Saldo_StoredProcedure_Excel @ParametrosBusquedaKardexSaldo ", parametrosBusquedaKardexSaldoAux).ToList();
            }
            catch (Exception ex)
			{
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "Kardex", "PROD");

            }
			modelAux = modelAux ?? new List<ResultKardex>();
            TempData["modelKardexExcel"] = modelAux;
            TempData.Keep("modelKardexExcel");

            ViewData["settingGridViewKardex"] = tempKardexFilter.settingGridViewKardex;

            return PartialView("_KardexResultsExcel", modelAux.ToList());
        }

        [HttpPost]
        public JsonResult ExportKardexExcel()
        {
            
            var model = TempData["modelKardexExcel"] as List<ResultKardex>;
            model = model ?? new List<ResultKardex>();

            TempData["modelKardexExcel"] = model;
            TempData.Keep("modelKardexExcel");

            var rows = model.Count();


            bool _isOk = rows > 0;
            string _message = rows > 0 ? "Se generará el archivo para descarga" : "No existen datos para descargar";
            string _fileName = rows > 0 ? "Kardex_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx" : "";

            if (rows > 0)
            {
                TempData[_fileName] = _fileName;
                TempData.Keep(_fileName);
            }

            return Json(new { isOk = _isOk, message = _message, fileName = _fileName });
        }
        [HttpGet]
        public ActionResult DownloadExcel(string fileName)
        {
            string _fileName = (string)TempData[fileName];
            TempData.Remove(fileName);

            var model = TempData["modelKardexExcel"] as List<ResultKardex>;
            model = model ?? new List<ResultKardex>();

			var modelExcel = model.Select(s => new ResultKardexExcel2
            {
				//id = s.id,
				emissionDate = s.emissionDate != null? s.emissionDate.Value.ToString("dd/MM/yyyy") :"",
				name_productionProcess = s.name_productionProcess ?? "",
				internalNumber = s.internalNumber ?? "",
				Provider_name = s.Provider_name ?? "",
				inventoryReason = s.inventoryReason ?? "",
				warehouse = s.warehouse ?? "",
				warehouseLocation = s.warehouseLocation ?? "",
				natureSequential = s.document ?? "",
				codigo_producto = s.codigo_producto ?? "",
				descripcion_producto = s.descripcion_producto ?? "",
				number = s.number,
				costo_promedio = s.costo_promedio ?? "",
				itemSize= s.itemSize ?? "",
				ItemMetricUnit = s.ItemMetricUnit ?? "",
				entry = s.entry??0,
				exit= s.exit??0,
				LB = s.LB ?? 0,
				KG = s.KG??0,
				usuario = s.usuario ?? "",
				nameDocumentState = s.nameDocumentState ?? ""
            }).ToList();
            /*
			 
 
			 */
            TempData["modelKardexExcel"] = model;
            TempData.Keep("modelKardexExcel");

            Stream stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Kardex");

			
                worksheet.Cells["A1"].LoadFromCollection(modelExcel, true);
                worksheet.Cells.AutoFitColumns();
                stream = new MemoryStream(package.GetAsByteArray());
                stream.Position = 0;
            }

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", _fileName);
        }


		[HttpGet]
		public ActionResult DownloadExcelToLot(string fileName)
        {

            fileName = "Reporte de Movimientos de Inventario por Lote.xlsx";
            Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelKardexExcelToLot"] as List<ResultKardexExcel>;
			var initialDate =  TempData["modelKardexFechaInicio"];
			var endDate = TempData["modelKardexFechaFin"];

			var companyName = db.Company.Where(c => c.isActive).Select(x => new {x.id, x.businessName }).FirstOrDefault();
			var divisionName = db.Division.Where(d => d.isActive && d.id_company == companyName.id).Select(dv => new { dv.id, dv.name}).FirstOrDefault();
			var branchOfficeName = db.BranchOffice.Where(b => b.isActive && b.id_company == companyName.id && b.id_division == divisionName.id )
															.Select(bo => new { bo.name}).FirstOrDefault();



			var spanishColumnNames = new Dictionary<string, string>
			{
				{"warehouse","Bodega"},
				{"warehouseLocation","Ubicación"},
				{"documentType","Tipo"},
				{"inventoryReason","Motivo"},
				{"document ","No. Mvto."},
				{"emissionDate","Fecha"},
				{"periodo","Periodo"},
				{"Sec_Interna","Sec. Interna"},
				{"internalNumber","Lote"},
				{"NumLote","Num Lote"},
				{"Codigo_Producto","Código Producto"},
				{"Nombre_Producto","Nombre Producto"},
				{"Presentacion","Presentación"},
				{"talla","Talla"},
				{"marca","Marca"},
				{"UM","UM"},
				{"Cantidad","Cantidad"},
				{"Costo_unitario","Costo U."},
				{"Costo_Total","Costo Total"},
				{"Cantidad_libras","Cantidad Lbs"},
				{"CostoULibra","Costo U. Lbs"},
				{"CostoTotalLB","Costo Total Lbs"},
				{"Estado","Estado"},
				{"Linea_Inventario","Línea Inventario"},
				{"Tipo_Producto","Tipo Producto"},
				{"Categoria","Categoría"},
				{"Grupo","Grupo"},
				{"Sub_Grupo","Sub-Grupo"},
				{"Modelo","Modelo"},
				{"EsTransferencia","Es Transferencia"},
				{"EsAutomatico","Es Automático"},
				{"Descripcion","Descripción"},
				{"CentroCosto","Centro Costo"},
				{"SubCentroCosto","Sub Centro Costo"},
				{"guiaRemission","N° Guía"},
				{"id","Id"},
				{"Usuario","Usuario"},
				{"factura","Factura"},
				{"contenedor","Contenedor"},
				{"guiaFactura","Guia-Factura"},
				{"cliente","Cliente"},
				{"ordenProduccion","Orden Producción"}
			};
			var data = modelExcel.Select(x => new
			{
				warehouse = x.warehouse,
				warehouseLocation = x.warehouseLocation,
				documentType = x.documentType,
				inventoryReason = x.inventoryReason,
				document = x.document,
				emissionDate = x.emissionDate.ToString("dd/MM/yyyy"),
				periodo = x.periodo,
				Sec_Interna = x.Sec_Interna,
				internalNumber = x.internalNumber,
				NumLote = x.NumLote,
				Codigo_Producto = x.Codigo_Producto,
				Nombre_Producto = x.Nombre_Producto,
				Presentacion = x.Presentacion,
				Talla = x.Talla,
				Marca = x.Marca,
				UM = x.UM,
				Cantidad = x.Cantidad,
				Costo_unitario = x.Costo_unitario,
				Costo_Total = x.Costo_Total,
				Cantidad_libras = x.Cantidad_libras,
				CostoULibra = x.CostoULibra,
				CostoTotalLB = x.CostoTotalLB,
				Estado = x.Estado,
				Linea_Inventario = x.Linea_Inventario,
				Tipo_Producto = x.Tipo_Producto,
				Categoria = x.Categoria,
				Grupo = x.Grupo,
				Sub_Grupo = x.Sub_Grupo,
				Modelo = x.Modelo,
				EsTransferencia = x.EsTransferencia,
				EsAutomatico = x.EsAutomatico,
				Descripcion = x.Descripcion,
				CentroCosto = x.CentroCosto,
				SubCentroCosto = x.SubCentroCosto,
				guiaRemission = x.guiaRemission,
				id = x.id,
				Usuario = x.Usuario,
				factura = x.factura,
				contenedor = x.contenedor,
				guiaFactura = x.guiaFactura,
				cliente = x.cliente,
				ordenProduccion = x.ordenProduccion

			});

			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Movimiento Por Lotes");

				// Establecer el nombre del reporte
				// Unir celdas para crear un encabezado
				worksheet.View.TabSelected = true; // Marcar la hoja como seleccionada
				worksheet.Cells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
				worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

				worksheet.Cells["A1:F1"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["A1:F1"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["A1"].Value = "Reporte de Movimientos de Inventario por Lote"; // Contenido en A1
				worksheet.Cells["A1:F1"].Style.Font.Size = 14;
				// Establecer el nombre de la compañía en A1

				worksheet.Cells["A3"].Style.Font.Bold = true;
				worksheet.Cells["A3"].Value = "Compañía:";
				worksheet.Cells["B3"].Value = data.Count() == 0 ? companyName.businessName : modelExcel[0]?.nameCompany;

				worksheet.Cells["A4"].Style.Font.Bold = true;
				worksheet.Cells["A4"].Value = "División:";
				worksheet.Cells["B4"].Value = data.Count() == 0 ? divisionName.name: modelExcel[0].nameDivision;

				worksheet.Cells["A5"].Style.Font.Bold = true;
				worksheet.Cells["A5"].Value = "Sucursal:";
				worksheet.Cells["B5"].Value = data.Count() == 0 ? branchOfficeName.name : modelExcel[0].nameBranchOffice;

				worksheet.Cells["A6"].Style.Font.Bold = true;
				worksheet.Cells["A6"].Value = "Desde:";
				worksheet.Cells["B6"].Value = initialDate;

				worksheet.Cells["D6"].Style.Font.Bold = true;
				worksheet.Cells["D6"].Value = "Hasta:";
				worksheet.Cells["E6"].Value = endDate;

				// Asignar nombres en español a las columnas utilizando el diccionario
				var columnIndex = 1;
				foreach (var kvp in spanishColumnNames)
				{
					worksheet.Cells[8, columnIndex].Style.Font.Bold = true;
					worksheet.Cells[8, columnIndex].Value = kvp.Value;
					columnIndex++;
				}
				if (data.Count() > 0)
                {
					worksheet.Cells["A9"].LoadFromCollection(data, false);
				}
				
				worksheet.Cells.AutoFitColumns();
				stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}


			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}


		[ValidateInput(false)]
		public ActionResult KardexPartial(GridViewSettings settingKardex)
		{
			var model = TempData["modelKardex"] as List<ResultKardex>;
			model = model ?? new List<ResultKardex>();

			TempData["modelKardex"] = model;
			TempData.Keep("modelKardex");

			var tempKardexFilter = TempData["KardexFilter"] as KardexFilter;
			tempKardexFilter = tempKardexFilter ?? new KardexFilter();
			tempKardexFilter.settingGridViewKardex = settingKardex;
			TempData["KardexFilter"] = tempKardexFilter;

			TempData.Keep("KardexFilter");

			return PartialView("_KardexPartial", model.OrderByDescending(o => o.emissionDate).ThenByDescending(i => i.dateCreate).ToList());
		}

        [ValidateInput(false)]
        public ActionResult KardexPartialExcel(GridViewSettings settingKardex)
        {
            var model = TempData["modelKardexExcel"] as List<ResultKardex>;
            model = model ?? new List<ResultKardex>();

            TempData["modelKardexExcel"] = model;
            TempData.Keep("modelKardexExcel");

            var tempKardexFilter = TempData["KardexFilter"] as KardexFilter;
            tempKardexFilter = tempKardexFilter ?? new KardexFilter();
            tempKardexFilter.settingGridViewKardex = settingKardex;
            TempData["KardexFilter"] = tempKardexFilter;

            TempData.Keep("KardexFilter");

            return PartialView("_KardexPartialExcel", model.OrderByDescending(o => o.emissionDate).ThenByDescending(i => i.dateCreate).ToList());
        }
        public ActionResult GetWarehouseEntry()
		{
			return PartialView("Components/_ComboBoxWarehouseEntry");
		}

		public ActionResult GetWarehouseExit()
		{
			return PartialView("Components/_ComboBoxWarehouseExit");
		}
		public ActionResult GetNatureMove()
		{
			return PartialView("Components/_ComboBoxNatureMove");
		}

		public ActionResult GetWarehouseLocationEntry(int? id_warehouseEntry)
		{
			int idWarehouseEntry = 0;
			if (id_warehouseEntry == null || id_warehouseEntry < 0)
			{
				if (Request.Params["id_warehouseEntry"] != null && Request.Params["id_warehouseEntry"] != "")
					idWarehouseEntry = int.Parse(Request.Params["id_warehouseEntry"]);
				else idWarehouseEntry = -1;
			}

			ViewBag.IdWarehouseEntry = id_warehouseEntry;

			return PartialView("Components/_ComboBoxWarehouseEntryLocation");
		}

		public ActionResult GetWarehouseLocationExit(int? id_warehouseExit)
		{
			int idWarehouseExit = 0;
			if (id_warehouseExit == null || id_warehouseExit < 0)
			{
				if (Request.Params["id_warehouseExit"] != null && Request.Params["id_warehouseExit"] != "")
					idWarehouseExit = int.Parse(Request.Params["id_warehouseExit"]);
				else idWarehouseExit = -1;
			}

			ViewBag.IdWarehouseExit = id_warehouseExit;

			return PartialView("Components/_ComboBoxWarehouseExitLocation");
		}

		public ActionResult GetInventoryReason(int? id_NatureMove)
		{
			int idNatureMove = 0;
			if (id_NatureMove == null || id_NatureMove < 0)
			{
				if (Request.Params["id_warehouseEntry"] != null && Request.Params["id_warehouseEntry"] != "")
					idNatureMove = int.Parse(Request.Params["id_warehouseEntry"]);
				else idNatureMove = -1;
			}

			ViewBag.idNatureMove = id_NatureMove;

			return PartialView("Components/_ComboBoxInventoryReason");
		}

		#endregion

		#region REPORTS
		/// <summary>
		/// Odd Version
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public ActionResult KardexReport()
		{
			var model = TempData["modelKardex"] as List<ResultKardex>;
			model = model ?? new List<ResultKardex>();

			KardexReport kardexReport = new KardexReport();

			Company company = db.Company.FirstOrDefault(c => c.id == this.ActiveCompanyId);
			kardexReport.DataSource = new KardexCompany
			{
				list_id_kardex = new List<int>(1),
				//listResultKardex = model.ToList(),
				listResultKardex = model.OrderByDescending(o => o.emissionDate).ThenByDescending(i => i.dateCreate).ToList(),//.OrderByDescending(i => i.id).ToList(),
				company = company
			};
			TempData.Keep("modelKardex");
			return PartialView("_KardexReport", kardexReport);
		}

		[HttpPost]
		public JsonResult InventoryKardexReport(string codeReport, InventoryMove inventoryMove,
						  InventoryEntryMove entryMove,
						  InventoryExitMove exitMove,
						  Document document,
						  DateTime? startEmissionDate, DateTime? endEmissionDate,
						  string numberLot, string internalNumberLot, int[] items)
		{
			var model = TempData["modelKardex"] as List<ResultKardex>;
			model = model ?? new List<ResultKardex>();
			TempData.Keep("modelKardex");

			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdFromSystemModel _repMod = new ReportProdFromSystemModel();

			_repMod.codeReport = codeReport;
			_repMod.dsSc = GetInventoryKardexReport2(codeReport, model, inventoryMove,
						  entryMove, exitMove, document, startEmissionDate, endEmissionDate,
						  numberLot, internalNumberLot, items);
			_repMod.hasSubReport = true;

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region AUXILIAR FUNCTIONS
		[HttpPost, ValidateInput(false)]
		public JsonResult InventoryMoveDetails()
		{
			InventoryMove inventoryMove = (TempData["inventoryMoveKardex"] as InventoryMove);

			inventoryMove = inventoryMove ?? new InventoryMove();
			inventoryMove.InventoryMoveDetail = inventoryMove.InventoryMoveDetail ?? new List<InventoryMoveDetail>();

			TempData.Keep("inventoryMoveKardex");

			return Json(inventoryMove.InventoryMoveDetail.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult WarehouseChangeData(int id_warehouse)
		{
			Warehouse warehouse = db.Warehouse.FirstOrDefault(i => i.id == id_warehouse);

			if (warehouse == null)
			{
				return Json(null, JsonRequestBehavior.AllowGet);
			}

			InventoryMove inventoryMove = (TempData["inventoryMoveKardex"] as InventoryMove);


			if (inventoryMove?.InventoryMoveDetail != null)
			{
				var itemDetail = inventoryMove.InventoryMoveDetail.ToList();

				foreach (var i in itemDetail)
				{
					i.id_warehouse = id_warehouse;
					UpdateModel(i);
				}
			}

			var result = new
			{
				id_warehouse,
				warehouse.name
			};

			TempData["inventoryMoveKardex"] = inventoryMove;
			TempData.Keep("inventoryMoveKardex");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult WarehouseLocationChangeData(int id_warehouseLocation)
		{
			WarehouseLocation warehouseLocation = db.WarehouseLocation.FirstOrDefault(i => i.id == id_warehouseLocation);

			if (warehouseLocation == null)
			{
				return Json(null, JsonRequestBehavior.AllowGet);
			}

			InventoryMove inventoryMove = (TempData["inventoryMoveKardex"] as InventoryMove);


			if (inventoryMove?.InventoryMoveDetail != null)
			{
				var itemDetail = inventoryMove.InventoryMoveDetail.ToList();

				foreach (var i in itemDetail)
				{
					i.id_warehouseLocation = id_warehouseLocation;
					UpdateModel(i);
				}
			}

			var result = new
			{
				id_warehouseLocation,
				warehouseLocation.name
			};

			TempData["inventoryMoveKardex"] = inventoryMove;
			TempData.Keep("inventoryMoveKardex");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		private DataSet GetInventoryKardexReportCosto(string codeReport
			, List<RepoKardexCosto> lstRK
			, InventoryMove inventoryMove
			, InventoryEntryMove entryMove,
			InventoryExitMove exitMove,
			Document document,
			DateTime? startEmissionDate, DateTime? endEmissionDate,
			string numberLot, string internalNumberLot, int[] items)
		{
			DataSet ds = new DataSet();
			List<RepoKardexCosto> lstRks = null;
			List<RepoKardexCostoResumenModel> lstResumen = null;
			List<RepoKardexCostoAgrupadoModel> lstResumenAgrupado = null;

			string usuario = this.ActiveUser.username;

			if (codeReport.Equals("KEDST"))
			{
				lstRks = new List<RepoKardexCosto>();
				var saldosIniciales = lstRK
										.Where(r => r.idMotivoInventario == 0)
										.GroupBy(r => new { r.idBodega, r.idProducto })
										.Select(s => new RepoKardexCosto
										{
											idDetalleInventario = s.Max(r => r.idDetalleInventario),
											idCabeceraInventario = s.Max(r => r.idCabeceraInventario),
											fechaInicio = startEmissionDate,
											fechaFin = endEmissionDate,
											numeroDocumentoInventario = s.Max(r => r.numeroDocumentoInventario),
											numberRemissionGuide = s.Max(r => r.numberRemissionGuide),
											nombreEstado = s.Max(r => r.nombreEstado),
											idBodega = s.Key.idBodega,
											nombreBodega = s.Max(r => r.nombreBodega),
											idUbicacion = s.Max(r => r.idUbicacion),
											nombreUbicacion = s.Max(r => r.nombreUbicacion),
											idProducto = s.Key.idProducto,
											nombreProducto = s.Max(r => r.nombreProducto),
											fechaEmison = s.Max(r => r.fechaEmison),
											idMotivoInventario = s.Max(r => r.idMotivoInventario),
											nombreMotivoInventario = s.Max(r => r.nombreMotivoInventario),
											idUnidadMedida = s.Max(r => r.idUnidadMedida),
											nombreUnidadMedida = s.Max(r => r.nombreUnidadMedida),
											montoEntrada = s.Sum(r => r.montoEntrada),
											montoSalida = s.Sum(r => r.montoSalida),
											balance = s.Sum(r => r.balance),
											previousBalance = s.Sum(r => r.previousBalance),
											idCompania = s.Max(r => r.idCompania),
											nameCompania = s.Max(r => r.nameCompania),
											nameDivision = s.Max(r => r.nameDivision),
											nameBranchOffice = s.Max(r => r.nameBranchOffice),
											//numberLot = s.Max(r => r.numberLot),
											numberLot = "",
											isCopacking = s.Max(r => r.isCopacking),
											nameProviderShrimp = s.Max(r => r.nameProviderShrimp),
											productionUnitProviderPool = s.Max(r => r.productionUnitProviderPool),
											itemSize = s.Max(r => r.itemSize),
											itemType = s.Max(r => r.itemType),
											ItemMetricUnit = s.Max(r => r.ItemMetricUnit),
											ItemPresentationValue = s.Max(r => r.ItemPresentationValue),
											amount = s.Sum(r => r.amount),
											amountCostUnit = s.Max(r => r.amountCostUnit),
											amountCostTotal = s.Sum(r => r.amountCostTotal),
											previousPound = s.Sum(r => r.previousPound),
											previousCostPound = s.Max(r => r.previousCostPound),
											previousTotalCostPound = s.Sum(r => r.previousTotalCostPound),
											entradaPound = 0,
											entradaCostPound = 0,
											entradaTotalCostPound = 0,
											salidaPound = 0,
											salidaCostPound = 0,
											salidaTotalCostPound = 0,
											finalPound = s.Sum(r => r.finalPound),
											finalCostPound = s.Max(r => r.finalCostPound),
											finalTotalCostPound = s.Sum(r => r.finalTotalCostPound),
											itemPresentationDescrip = s.Max(r => r.itemPresentationDescrip),
											oneItemPound = s.Max(r => r.oneItemPound),
											// Reemplazo Nombre de Usuario
											Provider_name = usuario,

										}).ToList();

				var movimientosMes = lstRK
										.Where(r => r.idMotivoInventario != 0)
										.Select(s => new RepoKardexCosto
										{
											idDetalleInventario = s.idDetalleInventario,
											idCabeceraInventario = s.idCabeceraInventario,
											fechaInicio = startEmissionDate,
											fechaFin = endEmissionDate,
											numeroDocumentoInventario = s.numeroDocumentoInventario,
											numberRemissionGuide = s.numberRemissionGuide,
											nombreEstado = s.nombreEstado,
											idBodega = s.idBodega,
											nombreBodega = s.nombreBodega,
											idUbicacion = s.idUbicacion,
											nombreUbicacion = s.nombreUbicacion,
											idProducto = s.idProducto,
											nombreProducto = s.nombreProducto,
											fechaEmison = s.fechaEmison,
											idMotivoInventario = s.idMotivoInventario,
											nombreMotivoInventario = s.nombreMotivoInventario,
											idUnidadMedida = s.idUnidadMedida,
											nombreUnidadMedida = s.nombreUnidadMedida,
											montoEntrada = s.montoEntrada,
											montoSalida = s.montoSalida,
											balance = s.balance,
											previousBalance = s.previousBalance,
											idCompania = s.idCompania,
											nameCompania = s.nameCompania,
											nameDivision = s.nameDivision,
											nameBranchOffice = s.nameBranchOffice,
											numberLot = s.numberLot,
											isCopacking = s.isCopacking,
											nameProviderShrimp = s.nameProviderShrimp,
											productionUnitProviderPool = s.productionUnitProviderPool,
											itemSize = s.itemSize,
											itemType = s.itemType,
											ItemMetricUnit = s.ItemMetricUnit,
											ItemPresentationValue = s.ItemPresentationValue,
											amount = s.amount,
											amountCostUnit = s.amountCostUnit,
											amountCostTotal = s.amountCostTotal,
											previousPound = s.previousPound,
											previousCostPound = s.previousCostPound,
											previousTotalCostPound = s.previousTotalCostPound,
											entradaPound = s.entradaPound,
											entradaCostPound = s.entradaCostPound,
											entradaTotalCostPound = s.entradaTotalCostPound,
											salidaPound = s.salidaPound,
											salidaCostPound = s.salidaCostPound,
											salidaTotalCostPound = s.salidaTotalCostPound,
											finalPound = s.finalPound,
											finalCostPound = s.finalCostPound,
											finalTotalCostPound = s.finalTotalCostPound,
											itemPresentationDescrip = s.itemPresentationDescrip,
											oneItemPound = s.oneItemPound,
											// Reemplazo Nombre de Usuario
											Provider_name = usuario
										})
										.ToList();
				lstRks.AddRange(saldosIniciales);
				lstRks.AddRange(movimientosMes);
				lstRks = lstRks
							.OrderBy(o1 => o1.idBodega)
							.ThenBy(o2 => o2.idProducto)
							.ThenBy(o => o.fechaEmison)
							.ThenByDescending(o => o.nombreMotivoInventario)
							.ThenBy(i => i.idCabeceraInventario)
							.ThenBy(i => i.idDetalleInventario)
							.ToList();
				//.ThenBy(i => i.idDetalleInventario).ToList();

				int idProducto = 0;
				decimal totalCosto = 0;
				foreach (var i in lstRks)
				{
					if (idProducto != i.idProducto)
					{
						idProducto = i.idProducto;
						totalCosto = (i.finalTotalCostPound ?? 0);
						continue;
					}

					totalCosto = ((totalCosto) + ((i.previousCostPound ?? 0) + (i.entradaTotalCostPound ?? 0))) - (i.salidaTotalCostPound ?? 0);
					i.finalTotalCostPound = totalCosto;
					if (i.finalPound == 0)
					{
						i.finalCostPound = 0;
					}
					else
					{
						i.finalCostPound = (i.finalTotalCostPound / i.finalPound);
					}


				}

				// saldo Inicial Costo
				lstResumen = new List<RepoKardexCostoResumenModel>();
				RepoKardexCostoResumenModel saldoInicial = saldosIniciales
																.GroupBy(r => r.idMotivoInventario)
																.Select(r => new RepoKardexCostoResumenModel
																{
																	id = r.Key,
																	descripcion = "Saldo Inicial",
																	cantidadesUnidades = r.Sum(t => Math.Round((t.amount ?? 0), 2)),
																	costoTotal = r.Sum(t => Math.Round((t.amount ?? 0), 2) * t.amountCostUnit),
																	cantidadesLibras = r.Sum(t => Math.Round((t.previousPound ?? 0), 2)),
																})
																.Select(r => new RepoKardexCostoResumenModel
																{
																	id = r.id,
																	descripcion = r.descripcion,
																	cantidadesUnidades = r.cantidadesUnidades,
																	costoTotal = Math.Round(r.costoTotal.Value, 5),
																	cantidadesLibras = Math.Round(r.cantidadesLibras.Value, 2),
																	promedioUnidades = Math.Round((r.costoTotal.Value / r.cantidadesUnidades.Value), 5),
																	promedioLibras = Math.Round((r.costoTotal.Value / r.cantidadesLibras.Value), 5),
																})
																.FirstOrDefault() ??
																new RepoKardexCostoResumenModel
																{
																	id = 0,
																	descripcion = "Saldo Inicial",
																	cantidadesUnidades = 0,
																	costoTotal = 0,
																	cantidadesLibras = 0,
																	promedioLibras = 0,
																	promedioUnidades = 0
																};
				lstResumen.Add(saldoInicial);

				RepoKardexCostoResumenModel ingresos = movimientosMes
																.Where(r => (r.entradaPound ?? 0) != 0)
																.GroupBy(r => r.idCompania)
																.Select(r => new RepoKardexCostoResumenModel
																{
																	id = 1,
																	descripcion = "Ingresos",
																	cantidadesUnidades = r.Sum(t => Math.Round((t.amount ?? 0), 2)),
																	costoTotal = r.Sum(t => Math.Round((t.amount ?? 0), 2) * t.amountCostUnit),
																	cantidadesLibras = r.Sum(t => Math.Round((t.entradaPound ?? 0), 2)),
																})
																.Select(r => new RepoKardexCostoResumenModel
																{
																	id = r.id,
																	descripcion = r.descripcion,
																	cantidadesUnidades = r.cantidadesUnidades,
																	costoTotal = Math.Round(r.costoTotal.Value, 5),
																	cantidadesLibras = Math.Round(r.cantidadesLibras.Value, 2),
																	promedioUnidades = Math.Round((r.costoTotal.Value / r.cantidadesUnidades.Value), 5),
																	promedioLibras = Math.Round((r.costoTotal.Value / r.cantidadesLibras.Value), 5),
																})
																.FirstOrDefault() ?? new RepoKardexCostoResumenModel
																{
																	id = 1,
																	descripcion = "Ingresos",
																	cantidadesUnidades = 0,
																	costoTotal = 0,
																	cantidadesLibras = 0,
																	promedioLibras = 0,
																	promedioUnidades = 0
																};
				lstResumen.Add(ingresos);

				RepoKardexCostoResumenModel egresos = movimientosMes
																.Where(r => (r.salidaPound ?? 0) != 0)
																.GroupBy(r => r.idCompania)
																.Select(r => new RepoKardexCostoResumenModel
																{
																	id = 2,
																	descripcion = "Egresos",
																	cantidadesUnidades = r.Sum(t => Math.Round((t.amount ?? 0), 2)),
																	costoTotal = r.Sum(t => Math.Round((t.amount ?? 0), 2) * t.amountCostUnit),
																	cantidadesLibras = r.Sum(t => Math.Round((t.salidaPound ?? 0), 2) * -1),
																})
																.Select(r => new RepoKardexCostoResumenModel
																{
																	id = r.id,
																	descripcion = r.descripcion,
																	cantidadesUnidades = r.cantidadesUnidades,
																	costoTotal = Math.Round(r.costoTotal.Value, 5),
																	cantidadesLibras = Math.Round(r.cantidadesLibras.Value, 2),
																	promedioUnidades = Math.Round((r.costoTotal.Value / r.cantidadesUnidades.Value), 5),
																	promedioLibras = Math.Round((r.costoTotal.Value / r.cantidadesLibras.Value), 5),
																})
																.FirstOrDefault() ?? new RepoKardexCostoResumenModel
																{
																	id = 2,
																	descripcion = "Egresos",
																	cantidadesUnidades = 0,
																	costoTotal = 0,
																	cantidadesLibras = 0,
																	promedioLibras = 0,
																	promedioUnidades = 0
																};
				lstResumen.Add(egresos);

				lstResumen = lstResumen.OrderBy(r => r.id).ToList();

				// Agrupado Linea , Tipo
				var mainAgrupacion = lstRK
										.GroupBy(r => new { r.itemSize, r.itemType })
										.Select(r => new RepoKardexCostoAgrupadoModel
										{
											id = r.Max(t => t.idDetalleInventario),
											descripcionLinea = r.Key.itemSize,
											descripcionTipo = r.Key.itemType
										})
										.ToList();
				var agrupadoSaldoInicial = lstRK
												.Where(r => r.idMotivoInventario == 0)
												.GroupBy(r => new { r.itemSize, r.itemType })
												.Select(r => new RepoKardexCostoAgrupadoModel
												{
													id = r.Max(t => t.idDetalleInventario),
													descripcionLinea = r.Key.itemSize,
													descripcionTipo = r.Key.itemType,
													saldoInicialCostTotLbs = r.Sum(t => Math.Round((t.amount ?? 0), 2) * (t.amountCostUnit ?? 0)),
													saldoInicialCantLbs = r.Sum(t => Math.Round((t.previousPound ?? 0), 2)),
												}).ToList();

				var agrupadoIngresoEgreso = lstRK
												.Where(r => r.idMotivoInventario != 0)
												.GroupBy(r => new { r.itemSize, r.itemType })
												.Select(r => new RepoKardexCostoAgrupadoModel
												{
													id = r.Max(t => t.idDetalleInventario),
													descripcionLinea = r.Key.itemSize,
													descripcionTipo = r.Key.itemType,
													ingresoCantLbs = r.Where(t => (t.entradaPound ?? 0) != 0)
																	  .Sum(t => Math.Round((t.entradaPound ?? 0), 2)),
													ingresoCostTotLbs = r.Where(t => (t.entradaPound ?? 0) != 0)
																		.Sum(t => Math.Round((t.amount ?? 0), 2) * (t.amountCostUnit ?? 0)),
													egresoCantLbs = r.Where(t => (t.salidaPound ?? 0) != 0)
																	  .Sum(t => Math.Round((t.salidaPound ?? 0), 2) * -1),
													egresoCostTotLbs = r.Where(t => (t.salidaPound ?? 0) != 0)
																		.Sum(t => (Math.Round((t.amount ?? 0), 2) * (t.amountCostUnit ?? 0)))
												}).ToList();

				lstResumenAgrupado = (from main in mainAgrupacion
									  join saldo in agrupadoSaldoInicial on
										new { linea = main.descripcionLinea, tipo = main.descripcionTipo } equals
										new { linea = saldo.descripcionLinea, tipo = saldo.descripcionTipo }
										into s1
									  from leftsaldo in s1.DefaultIfEmpty()
									  join ingresoegreso in agrupadoIngresoEgreso on
										new { linea = main.descripcionLinea, tipo = main.descripcionTipo } equals
										new { linea = ingresoegreso.descripcionLinea, tipo = ingresoegreso.descripcionTipo }
										into s2
									  from leftingegr in s2.DefaultIfEmpty()
									  select new RepoKardexCostoAgrupadoModel
									  {
										  id = main.id,
										  descripcionLinea = main.descripcionLinea,
										  descripcionTipo = main.descripcionTipo,
										  saldoInicialCantLbs = (leftsaldo == null) ? 0 : leftsaldo.saldoInicialCantLbs,
										  saldoInicialCostTotLbs = (leftsaldo == null) ? 0 : leftsaldo.saldoInicialCostTotLbs,
										  ingresoCantLbs = (leftingegr == null) ? 0 : leftingegr.ingresoCantLbs,
										  ingresoCostTotLbs = (leftingegr == null) ? 0 : leftingegr.ingresoCostTotLbs,
										  egresoCantLbs = (leftingegr == null) ? 0 : leftingegr.egresoCantLbs,
										  egresoCostTotLbs = (leftingegr == null) ? 0 : leftingegr.egresoCostTotLbs,
									  })
									  .Select(r => new RepoKardexCostoAgrupadoModel
									  {
										  id = r.id,
										  descripcionLinea = r.descripcionLinea,
										  descripcionTipo = r.descripcionTipo,
										  saldoInicialCantLbs = r.saldoInicialCantLbs,
										  saldoInicialCostTotLbs = r.saldoInicialCostTotLbs,
										  ingresoCantLbs = r.ingresoCantLbs,
										  ingresoCostTotLbs = r.ingresoCostTotLbs,
										  egresoCantLbs = r.egresoCantLbs,
										  egresoCostTotLbs = r.egresoCostTotLbs,
										  saldoFinalCantLbs = (r.saldoInicialCantLbs + r.ingresoCantLbs) + r.egresoCantLbs,
										  saldoFinalCostTotLbs = (r.saldoInicialCostTotLbs + r.ingresoCostTotLbs) + r.egresoCostTotLbs
									  })
									  .OrderBy(r => r.descripcionLinea)
									  .ThenBy(r => r.descripcionTipo)
									  .ToList();
			}


			ds.Tables.Add(ConvertToDataTable(lstRks, "RepoKardexCosto"));


			int idCompany = lstRks.Select(s => s.idCompania).FirstOrDefault() ?? 0;
			Company _co = db.Company.FirstOrDefault(fod => fod.id == idCompany);

			_co = _co ?? db.Company.FirstOrDefault();
			List<RepoCompany> lstCom = new List<RepoCompany>();
			RepoCompany _re = new RepoCompany();
			_re.idCompany = _co.id;
			_re.logo = _co?.logo;

			lstCom.Add(_re);
			ds.Tables.Add(ConvertToDataTable(lstCom, "RepoCompany"));
			ds.Tables.Add(ConvertToDataTable(lstResumen, "RepoKardexCostoResumenModel"));
			ds.Tables.Add(ConvertToDataTable(lstResumenAgrupado, "RepoKardexCostoAgrupadoModel"));
			return ds;
		}

		private DataSet GetInventoryKardexReport2(string codeReport
			, List<ResultKardex> lstRK
			, InventoryMove inventoryMove
			, InventoryEntryMove entryMove,
			InventoryExitMove exitMove,
			Document document,
			DateTime? startEmissionDate, DateTime? endEmissionDate,
			string numberLot, string internalNumberLot, int[] items)
		{
			DataSet ds = new DataSet();
			List<RepoKardexSaldo> lstRks = null;

			if (codeReport.Equals("IMIPV1"))
			{
				lstRks = lstRK
							.Select(s => new RepoKardexSaldo
							{
								idDetalleInventario = s.id,
								idCabeceraInventario = s.id_document,
								fechaInicio = startEmissionDate,
								fechaFin = endEmissionDate,
								numeroDocumentoInventario = s.document,
								numberRemissionGuide = s.numberRemissionGuide,
								nombreEstado = s.nameDocumentState,
								idBodega = s.id_warehouse ?? 0,
								nombreBodega = s.warehouse,
								idUbicacion = s.id_warehouseLocation ?? 0,
								nombreUbicacion = s.warehouseLocation,
								idProducto = s.id_item ?? 0,
								nombreProducto = s.code_item,
								fechaEmison = s.emissionDate ?? DateTime.Now,
								idMotivoInventario = s.id_inventoryReason ?? 0,
								nombreMotivoInventario = s.inventoryReason,
								idUnidadMedida = s.id_metricUnit ??0,
								nombreUnidadMedida = s.metricUnit,
								montoEntrada = s.entry,
								montoSalida = s.exit,
								balance = s.balanceCutting,
								balanceCutting = s.balanceCutting,
								previousBalance = s.previousBalance,
								idCompania = s.idCompany,
								nameCompania = s.nameCompany,
								nameDivision = s.nameDivision,
								nameBranchOffice = s.nameBranchOffice,
								numberLot = s.internalNumber,
								Provider_name = s.Provider_name,
								isCopacking = s.isCopacking,
								nameProviderShrimp = s.nameProviderShrimp,
								productionUnitProviderPool = s.productionUnitProviderPool,
								itemSize = s.itemSize,
								itemType = s.itemType,
								ItemMetricUnit = s.ItemMetricUnit,
								ItemPresentationValue = s.ItemPresentationValue,
							})
							.OrderBy(o1 => o1.idBodega)
							.ThenBy(o => o.fechaEmison)
							.ThenBy(i => i.idDetalleInventario).ToList();
			}
			else
			{
				lstRks = lstRK
							.Select(s => new RepoKardexSaldo
							{
								idDetalleInventario = s.id,
								idCabeceraInventario = s.id_document,
								fechaInicio = startEmissionDate,
								fechaFin = endEmissionDate,
								numeroDocumentoInventario = s.document,
								numberRemissionGuide = s.numberRemissionGuide,
								idBodega = s.id_warehouse ?? 0,
								nombreBodega = s.warehouse,
								idUbicacion = s.id_warehouseLocation ?? 0,
								nombreUbicacion = s.warehouseLocation,
								idProducto = s.id_item??0,
								nombreProducto = s.code_item,
								fechaEmison = s.emissionDate?? DateTime.Now,
								idMotivoInventario = s.id_inventoryReason ?? 0,
								nombreMotivoInventario = s.inventoryReason,
								idUnidadMedida = s.id_metricUnit??0,
								nombreUnidadMedida = s.metricUnit,
								montoEntrada = s.entry,
								montoSalida = s.exit,
								balance = s.balance,
								balanceCutting = s.balanceCutting,
								previousBalance = s.previousBalance,
								idCompania = s.idCompany,
								nameCompania = s.nameCompany,
								nameDivision = s.nameDivision,
								nameBranchOffice = s.nameBranchOffice,
								numberLot = s.internalNumber,
								Provider_name = s.Provider_name,
								isCopacking = s.isCopacking,
								nameProviderShrimp = s.nameProviderShrimp,
								productionUnitProviderPool = s.productionUnitProviderPool,
								itemSize = s.itemSize,
								itemType = s.itemType,
								ItemMetricUnit = s.ItemMetricUnit,
								ItemPresentationValue = s.ItemPresentationValue,
								nombreEstado = s.nameDocumentState
							})
							.OrderBy(o1 => o1.idBodega)
							.ThenBy(o2 => o2.idProducto)
							.ThenBy(o => o.fechaEmison)
							.ThenBy(i => i.idDetalleInventario).ToList();
			}

			ds.Tables.Add(ConvertToDataTable(lstRks, "RepoKardexSaldo"));

			int idCompany = lstRks.Select(s => s.idCompania).FirstOrDefault() ?? 0;
			Company _co = db.Company.FirstOrDefault(fod => fod.id == idCompany);

			_co = _co ?? db.Company.FirstOrDefault();
			List<RepoCompany> lstCom = new List<RepoCompany>();
			RepoCompany _re = new RepoCompany();
			_re.idCompany = _co.id;
			_re.logo = _co?.logo;

			lstCom.Add(_re);
			ds.Tables.Add(ConvertToDataTable(lstCom, "RepoCompany"));
			return ds;
		}

		private DataSet GetInventoryKardexExcelReport(string codeReport
	, List<ResultKardexExcel> lstRK
	, InventoryMove inventoryMove
	, InventoryEntryMove entryMove,
	InventoryExitMove exitMove,
	Document document,
	DateTime? startEmissionDate, DateTime? endEmissionDate,
	string numberLot, string internalNumberLot, int[] items)
		{
			DataSet ds = new DataSet();
			List<ResultKardexExcel> lstRks = null;


			lstRks = lstRK
						.Select(s => new ResultKardexExcel
						{
							id = s.id,
							idCompania = s.idCompania,
							nameCompany = s.nameCompany,
							nameDivision = s.nameDivision,
							nameBranchOffice = s.nameBranchOffice,
							warehouse = s.warehouse,
							warehouseLocation = s.warehouseLocation,
							documentType = s.documentType,
							document = s.document,
							emissionDate = s.emissionDate,
							Sec_Interna = s.Sec_Interna,
							internalNumber = s.internalNumber,
							Codigo_Producto = s.Codigo_Producto,
							Nombre_Producto = s.Nombre_Producto,
							Presentacion = s.Presentacion,
							Talla = s.Talla,
							Marca = s.Marca,
							UM = s.UM,
							Cantidad = s.Cantidad,
							Costo_unitario = s.Costo_unitario,
							Costo_Total = s.Costo_Total,
							Cantidad_libras = s.Cantidad_libras,
							CostoULibra = s.CostoULibra,
							CostoTotalLB = s.CostoTotalLB,
							nameDocumentState = s.nameDocumentState,
							Linea_Inventario = s.Linea_Inventario,
							Tipo_Producto = s.Tipo_Producto,
							Categoria = s.Categoria,
							Grupo = s.Grupo,
							Sub_Grupo = s.Sub_Grupo,
							Modelo = s.Modelo,
							Descripcion = s.Descripcion,
							CentroCosto = s.CentroCosto,
							SubCentroCosto = s.SubCentroCosto,
							//Usuario = s.Usuario
						})
						.OrderBy(o1 => o1.emissionDate)
						.ThenBy(i => i.Nombre_Producto).ToList();


			ds.Tables.Add(ConvertToDataTable(lstRks, "RepoKardexSaldoExcel"));

			int idCompany = lstRks.Select(s => s.idCompania).FirstOrDefault() ?? 0;
			Company _co = db.Company.FirstOrDefault(fod => fod.id == idCompany);

			_co = _co ?? db.Company.FirstOrDefault();
			List<RepoCompany> lstCom = new List<RepoCompany>();
			RepoCompany _re = new RepoCompany();
			_re.idCompany = _co.id;
			_re.logo = _co?.logo;

			lstCom.Add(_re);
			ds.Tables.Add(ConvertToDataTable(lstCom, "RepoCompany"));
			return ds;
		}

		public DataTable ConvertToDataTable<T>(IList<T> data, string name)
		{
			PropertyDescriptorCollection properties =
			   TypeDescriptor.GetProperties(typeof(T));
			DataTable table = new DataTable(name);
			foreach (PropertyDescriptor prop in properties)
				table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
			foreach (T item in data)
			{
				DataRow row = table.NewRow();
				foreach (PropertyDescriptor prop in properties)
					row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
				table.Rows.Add(row);
			}
			return table;

		}
		//Funcion Original
		//
		[HttpPost]
		public JsonResult InventoryKardexReportIndex(string codeReport, InventoryMove inventoryMove,
				  InventoryEntryMove entryMove,
				  InventoryExitMove exitMove,
				  Document document,
				  DateTime? startEmissionDateFinal, DateTime? endEmissionDateFinal,
				  string numberLot, string internalNumberLot, int[] items)
        {
			var reporte = db.tbsysPathReportProduction.FirstOrDefault(e => e.code == codeReport);
			if(reporte?.isCustomized ?? false)
            {
				return InventoryKardexReportIndex1(codeReport, inventoryMove, entryMove, exitMove,
					 document, startEmissionDateFinal, endEmissionDateFinal, numberLot, internalNumberLot, items);
			}
            else
            {
				return InventoryKardexReportIndex2(codeReport, inventoryMove, entryMove, exitMove,
					 document, startEmissionDateFinal, endEmissionDateFinal, numberLot, internalNumberLot, items);
			}
        }
		private JsonResult InventoryKardexReportIndex2(string codeReport, InventoryMove inventoryMove,
				  InventoryEntryMove entryMove,
				  InventoryExitMove exitMove,
				  Document document,
				  DateTime? startEmissionDateFinal, DateTime? endEmissionDateFinal,
				  string numberLot, string internalNumberLot, int[] items)
		{
			Parametros.ParametrosBusquedaKardexSaldo parametrosBusquedaKardexSaldo = new Parametros.ParametrosBusquedaKardexSaldo();
			parametrosBusquedaKardexSaldo.id_documentType = document.id_documentType;
			if (!string.IsNullOrEmpty(document.number)) parametrosBusquedaKardexSaldo.number = document.number;
			if (!string.IsNullOrEmpty(document.reference)) parametrosBusquedaKardexSaldo.reference = document.reference;
			parametrosBusquedaKardexSaldo.startEmissionDate = startEmissionDateFinal;
			parametrosBusquedaKardexSaldo.endEmissionDate = endEmissionDateFinal;
			if (inventoryMove.idNatureMove != null) parametrosBusquedaKardexSaldo.idNatureMove = inventoryMove.idNatureMove;
			if (inventoryMove.id_inventoryReason != null) parametrosBusquedaKardexSaldo.id_inventoryReason = inventoryMove.id_inventoryReason;
			if (codeReport.Equals("IKRPV") || codeReport.Equals("IKRPS"))
			{
				parametrosBusquedaKardexSaldo.id_warehouseExit = db.Warehouse
																	.Where(e => e.code.Equals("VIRPRO"))?.FirstOrDefault()?.id ?? 0;
			}
			else
			{
				if (exitMove.id_warehouseExit != null) parametrosBusquedaKardexSaldo.id_warehouseExit = exitMove.id_warehouseExit;
			}
			if (exitMove.id_warehouseLocationExit != null) parametrosBusquedaKardexSaldo.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
			parametrosBusquedaKardexSaldo.id_dispatcher = exitMove.id_dispatcher;
			if (entryMove.id_warehouseEntry != null) parametrosBusquedaKardexSaldo.id_warehouseEntry = entryMove.id_warehouseEntry;
			if (entryMove.id_warehouseLocationEntry != null) parametrosBusquedaKardexSaldo.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
			parametrosBusquedaKardexSaldo.id_receiver = entryMove.id_receiver;
			if (!string.IsNullOrEmpty(numberLot)) parametrosBusquedaKardexSaldo.numberLot = numberLot;
			if (!string.IsNullOrEmpty(internalNumberLot)) parametrosBusquedaKardexSaldo.internalNumberLot = internalNumberLot;
			if (items != null && items.Length > 0)
			{
				parametrosBusquedaKardexSaldo.items = items[0].ToString();
				for (int i = 1; i < items.Length; i++)
				{
					parametrosBusquedaKardexSaldo.items += "," + items[i].ToString();
				}
			}
			parametrosBusquedaKardexSaldo.codeReport = codeReport;

			var parametrosBusquedaKardexSaldoAux = new SqlParameter();
			parametrosBusquedaKardexSaldoAux.ParameterName = "@ParametrosBusquedaKardexSaldo";
			parametrosBusquedaKardexSaldoAux.Direction = ParameterDirection.Input;
			parametrosBusquedaKardexSaldoAux.SqlDbType = SqlDbType.NVarChar;
			var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaKardexSaldo);
			parametrosBusquedaKardexSaldoAux.Value = jsonAux;
			db.Database.CommandTimeout = 2200;
			List<ResultKardex> modelAux = new List<ResultKardex>();

			List<RepoKardexCosto> modelAuxCost = new List<RepoKardexCosto>();
			if (codeReport.Equals("IKRPS"))
			{
				modelAux = db.Database.SqlQuery<ResultKardex>("exec KardexProveedorSaldoJSON @ParametrosBusquedaKardexSaldo", parametrosBusquedaKardexSaldoAux).ToList();
			}
			else
			{
				if (codeReport.Equals("KEDST"))
				{
					modelAuxCost = db.Database.SqlQuery<RepoKardexCosto>("exec inv_Consultar_Kardex_Saldo_Costo_StoredProcedure @ParametrosBusquedaKardexSaldo ", parametrosBusquedaKardexSaldoAux).OrderBy(obd => obd.fechaEmison).ThenBy(tbd => tbd.idDetalleInventario).ToList();
				}
				else
				{
					if (codeReport.Equals("IKSPV1") || codeReport.Equals("ISPV1") || codeReport.Equals("IMIPV1"))
					{
						modelAux = db.Database.SqlQuery<ResultKardex>("exec inv_Consultar_Kardex_Saldo_SinLote_StoredProcedure @ParametrosBusquedaKardexSaldo ", parametrosBusquedaKardexSaldoAux).OrderBy(obd => obd.emissionDate).ThenBy(tbd => tbd.dateCreate).ToList();
					}
					else
					{
						modelAux = db.Database.SqlQuery<ResultKardex>("exec inv_Consultar_Kardex_Saldo_StoredProcedure @ParametrosBusquedaKardexSaldo ", parametrosBusquedaKardexSaldoAux).OrderBy(obd => obd.emissionDate).ThenBy(tbd => tbd.dateCreate).ToList();
					}
					if (codeReport.Equals("IMIPV1"))
					{
						modelAux = modelAux.Where(w => w.entry != 0 || w.exit != 0).ToList();
					}
				}

			}

			ReportParanNameModel rep = new ReportParanNameModel();
			ReportProdFromSystemModel _repMod = new ReportProdFromSystemModel();

			_repMod.codeReport = codeReport;

			if (codeReport.Equals("KEDST"))
			{
				_repMod.dsSc = GetInventoryKardexReportCosto(codeReport, modelAuxCost, inventoryMove,
																entryMove, exitMove, document, startEmissionDateFinal,
																endEmissionDateFinal, numberLot, internalNumberLot, items);

				_repMod.subReportNames = new string[] { "", "repokardexcostoresumen", "repokardexcostoagrupado" };
			}
			else
			{
				_repMod.dsSc = GetInventoryKardexReport2(codeReport, modelAux, inventoryMove,
							  entryMove, exitMove, document, startEmissionDateFinal, endEmissionDateFinal,
							  numberLot, internalNumberLot, items);
			}

			_repMod.hasSubReport = true;
			_repMod.toSend = true;

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = new
			{
				rep,
				ServerScriptTimeout = Server.ScriptTimeout
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		//
		[HttpPost]
		private JsonResult InventoryKardexReportIndex1(string codeReport, InventoryMove inventoryMove,
				  InventoryEntryMove entryMove,
				  InventoryExitMove exitMove,
				  Document document,
				  DateTime? startEmissionDateFinal, DateTime? endEmissionDateFinal,
				  string numberLot, string internalNumberLot, int[] items)
		{
			Parametros.ParametrosBusquedaKardexSaldo parametrosBusquedaKardexSaldo = new Parametros.ParametrosBusquedaKardexSaldo();
			parametrosBusquedaKardexSaldo.id_documentType = document.id_documentType;
			if (!string.IsNullOrEmpty(document.number)) parametrosBusquedaKardexSaldo.number = document.number;
			if (!string.IsNullOrEmpty(document.reference)) parametrosBusquedaKardexSaldo.reference = document.reference;
			parametrosBusquedaKardexSaldo.startEmissionDate = startEmissionDateFinal;
			parametrosBusquedaKardexSaldo.endEmissionDate = endEmissionDateFinal;
			if (inventoryMove.idNatureMove != null) parametrosBusquedaKardexSaldo.idNatureMove = inventoryMove.idNatureMove;
			if (inventoryMove.id_inventoryReason != null) parametrosBusquedaKardexSaldo.id_inventoryReason = inventoryMove.id_inventoryReason;
			if (codeReport.Equals("IKRPV") || codeReport.Equals("IKRPS"))
			{
				parametrosBusquedaKardexSaldo.id_warehouseExit = db.Warehouse
																	.Where(e => e.code.Equals("VIRPRO"))?.FirstOrDefault()?.id ?? 0;
			}
			else
			{
				if (exitMove.id_warehouseExit != null) parametrosBusquedaKardexSaldo.id_warehouseExit = exitMove.id_warehouseExit;
			}
			if (exitMove.id_warehouseLocationExit != null) parametrosBusquedaKardexSaldo.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
			parametrosBusquedaKardexSaldo.id_dispatcher = exitMove.id_dispatcher;
			if (entryMove.id_warehouseEntry != null) parametrosBusquedaKardexSaldo.id_warehouseEntry = entryMove.id_warehouseEntry;
			if (entryMove.id_warehouseLocationEntry != null) parametrosBusquedaKardexSaldo.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
			parametrosBusquedaKardexSaldo.id_receiver = entryMove.id_receiver;
			if (!string.IsNullOrEmpty(numberLot)) parametrosBusquedaKardexSaldo.numberLot = numberLot;
			if (!string.IsNullOrEmpty(internalNumberLot)) parametrosBusquedaKardexSaldo.internalNumberLot = internalNumberLot;
			if (items != null && items.Length > 0)
			{
				parametrosBusquedaKardexSaldo.items = items[0].ToString();
				for (int i = 1; i < items.Length; i++)
				{
					parametrosBusquedaKardexSaldo.items += "," + items[i].ToString();
				}
			}
			parametrosBusquedaKardexSaldo.codeReport = codeReport;

			var parametrosBusquedaKardexSaldoAux = new SqlParameter();
			parametrosBusquedaKardexSaldoAux.ParameterName = "@ParametrosBusquedaKardexSaldo";
			parametrosBusquedaKardexSaldoAux.Direction = ParameterDirection.Input;
			parametrosBusquedaKardexSaldoAux.SqlDbType = SqlDbType.NVarChar;
			var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaKardexSaldo);
			parametrosBusquedaKardexSaldoAux.Value = jsonAux;
			db.Database.CommandTimeout = 2200;
			List<ResultKardex> modelAux = new List<ResultKardex>();

			List<RepoKardexCosto> modelAuxCost = new List<RepoKardexCosto>();
			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();
			ReportProdModel _repMod = new ReportProdModel();
			_repMod.paramCRList = new[]
			{
				new ParamCR()
                {
					Nombre = "@ParametrosBusquedaKardexSaldo",
					Valor = jsonAux
                }
			}
			.ToList();
			_repMod.conex = objConex;
			_repMod.codeReport = codeReport;			
			rep = GetTmpDataName(20);
			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);
			var result = rep;
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		public JsonResult InventoryKardexReportExcelIndex(string codeReport, InventoryMove inventoryMove,
				  InventoryEntryMove entryMove,
				  InventoryExitMove exitMove,
				  Document document,
				  DateTime? startEmissionDateFinal, DateTime? endEmissionDateFinal,
				  string numberLot, string internalNumberLot, int[] items)
		{

			Parametros.ParametrosBusquedaKardexSaldo parametrosBusquedaKardexSaldo = new Parametros.ParametrosBusquedaKardexSaldo();
			parametrosBusquedaKardexSaldo.id_documentType = document.id_documentType;
			if (!string.IsNullOrEmpty(document.number)) parametrosBusquedaKardexSaldo.number = document.number;
			if (!string.IsNullOrEmpty(document.reference)) parametrosBusquedaKardexSaldo.reference = document.reference;
			parametrosBusquedaKardexSaldo.startEmissionDate = startEmissionDateFinal;
			parametrosBusquedaKardexSaldo.endEmissionDate = endEmissionDateFinal;
			if (inventoryMove.idNatureMove != null) parametrosBusquedaKardexSaldo.idNatureMove = inventoryMove.idNatureMove;
			if (inventoryMove.id_inventoryReason != null) parametrosBusquedaKardexSaldo.id_inventoryReason = inventoryMove.id_inventoryReason;
			if (codeReport.Equals("IKRPV") || codeReport.Equals("IKRPS"))
			{
				parametrosBusquedaKardexSaldo.id_warehouseExit = db.Warehouse
																	.Where(e => e.code.Equals("VIRPRO"))?.FirstOrDefault()?.id ?? 0;
			}
			else
			{
				if (exitMove.id_warehouseExit != null) parametrosBusquedaKardexSaldo.id_warehouseExit = exitMove.id_warehouseExit;
			}
			if (exitMove.id_warehouseLocationExit != null) parametrosBusquedaKardexSaldo.id_warehouseLocationExit = exitMove.id_warehouseLocationExit;
			parametrosBusquedaKardexSaldo.id_dispatcher = exitMove.id_dispatcher;
			if (entryMove.id_warehouseEntry != null) parametrosBusquedaKardexSaldo.id_warehouseEntry = entryMove.id_warehouseEntry;
			if (entryMove.id_warehouseLocationEntry != null) parametrosBusquedaKardexSaldo.id_warehouseLocationEntry = entryMove.id_warehouseLocationEntry;
			parametrosBusquedaKardexSaldo.id_receiver = entryMove.id_receiver;
			if (!string.IsNullOrEmpty(numberLot)) parametrosBusquedaKardexSaldo.numberLot = numberLot;
			if (!string.IsNullOrEmpty(internalNumberLot)) parametrosBusquedaKardexSaldo.internalNumberLot = internalNumberLot;
			if (items != null && items.Length > 0)
			{
				parametrosBusquedaKardexSaldo.items = items[0].ToString();
				for (int i = 1; i < items.Length; i++)
				{
					parametrosBusquedaKardexSaldo.items += "," + items[i].ToString();
				}
			}
			parametrosBusquedaKardexSaldo.codeReport = codeReport;

			var parametrosBusquedaKardexSaldoAux = new SqlParameter();
			parametrosBusquedaKardexSaldoAux.ParameterName = "@ParametrosBusquedaKardexSaldo";
			parametrosBusquedaKardexSaldoAux.Direction = ParameterDirection.Input;
			parametrosBusquedaKardexSaldoAux.SqlDbType = SqlDbType.NVarChar;
			var jsonAux = JsonConvert.SerializeObject(parametrosBusquedaKardexSaldo);
            parametrosBusquedaKardexSaldoAux.Value = jsonAux;
			db.Database.CommandTimeout = 2200;
			List<ResultKardexExcel> modelAux = new List<ResultKardexExcel>();

			modelAux = db.Database.SqlQuery<ResultKardexExcel>("exec inv_Consultar_Kardex_Saldo_Excel_StoredProcedure @ParametrosBusquedaKardexSaldo ", parametrosBusquedaKardexSaldoAux).OrderBy(obd => obd.emissionDate).ThenBy(tbd => tbd.emissionDate).ToList();
			List<ParamCR> paramLst = new List<ParamCR>();
			TempData["modelKardexExcelToLot"] = modelAux;
			
			ParamCR _param = new ParamCR();

			string id_documentState = "";
			if (startEmissionDateFinal != null) { id_documentState = startEmissionDateFinal.Value.Date.ToString("yyyy/MM/dd"); }
			_param = new ParamCR
			{
				Nombre = "@fechaInicio",
				Valor = id_documentState
			};
			paramLst.Add(_param);

			string str_endEmissionDate = "";
			if (endEmissionDateFinal != null) { str_endEmissionDate = endEmissionDateFinal.Value.Date.ToString("yyyy/MM/dd"); }
			_param = new ParamCR
			{
				Nombre = "@fechaFin",
				Valor = str_endEmissionDate
			};
			paramLst.Add(_param);

			TempData["modelKardexFechaInicio"] = id_documentState;
			TempData["modelKardexFechaFin"] = str_endEmissionDate;
			//_param = new ParamCR
			//{
			//	Nombre = "@ParametrosBusquedaKardexSaldo",
			//	Valor = JsonConvert.SerializeObject(parametrosBusquedaKardexSaldo)
			//};
			//paramLst.Add(_param);


			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = codeReport;
			_repMod.conex = objConex;
			_repMod.paramCRList = paramLst;
			_repMod.nameReport = "Reporte de Movimiento de Inventario por Lote";

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);

		}
		#endregion
	}
}