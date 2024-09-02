using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.General;
using DXPANACEASOFT.Models.ItemP.ItemModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Utilitarios.Logs;
using Newtonsoft.Json;
using DXPANACEASOFT.Services;
using DevExpress.XtraCharts;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderItem
	{
		public static Item Item(int? id)
		{
			var db = new DBContext();
			return db.Item.FirstOrDefault(i => i.id == id);
		}

		public static IEnumerable Items()
		{
			var db = new DBContext();
			return db.Item.Where(t => t.isActive).ToList();
		}

		public static IEnumerable ItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.isActive);

			return items.ToList();
		}

		public static IEnumerable AllItems()
		{
			var db = new DBContext();
			return db.Item.ToList();
		}

		public static IEnumerable AllItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.InventoryLine.code != "PT");

			return items.ToList();
		}

		public static List<Item> ItemsByProvider(int id_provider)
		{
			var db = new DBContext();
			ItemProvider provider = db.ItemProvider.FirstOrDefault(p => p.id_provider == id_provider);
			return db.Item.Where(i => i.ItemProvider.Contains(provider)).OrderBy(i => i.id).ToList();
		}

		public static List<Item> ItemsByItemType(int id_itemType)
		{
			var db = new DBContext();
			return db.Item.Where(i => i.ItemType.id == id_itemType).ToList();
		}

		public static IEnumerable PurchaseItems(/*int? id_item*/)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.isPurchased && i.isActive);

			//if (id_item != -1)
			//    items = from i in items where i.id == id_item select i;

			return items.ToList();
		}

		public static IEnumerable PurchaseItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.isPurchased && i.isActive);

			return items.ToList();
		}

		public static IEnumerable AllPurchaseItemsMPByCompany(int? id_company)
		{
			var db = new DBContext();

			return db.Item.Where(s => s.id_company == id_company && s.InventoryLine.code == "MP").Select(s => new { id = s.id, name = s.name }).ToList();

		}
		public static IEnumerable AllPurchaseItemsPPPTByCompany(int? id_company)
		{
			var db = new DBContext();

			return db.Item.Where(s => s.id_company == id_company && (s.InventoryLine.code == "PP" || s.InventoryLine.code == "PT")).Select(s => new { id = s.id, name = s.name }).ToList();

		}
		public static IEnumerable AllPurchaseItemsMDEByCompany(int? id_company)
		{
			var db = new DBContext();

			return db.Item.Where(s => s.id_company == id_company && (s.ItemTypeCategory.code == "MDE")).Select(s => new { id = s.id, name = s.name }).ToList();

		}
		public static IEnumerable AllPurchaseItemsDEByCompany(int? id_company)
		{
			var db = new DBContext();

			return db.Item.Where(s => s.id_company == id_company && (s.InventoryLine.code == "DE")).Select(s => new { id = s.id, name = s.name }).ToList();

		}
		public static IEnumerable AllPurchaseItemsMDDByCompany(int? id_company)
		{
			var db = new DBContext();

			return db.Item.Where(s => s.id_company == id_company && s.InventoryLine.code == "MI" && s.ItemType.code == "MDD").Select(s => new { id = s.id, name = s.name }).ToList();

		}

		public static IEnumerable PurchaseItemsByCompanyAndCurrent(int? id_company, int? id_current)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => (i.id_company == id_company && i.isPurchased && i.isActive) || id_company == id_current);

			return items.ToList();
		}

		public static IEnumerable AllPurchaseItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item
				.Include(i => i.ItemPurchaseInformation.MetricUnit)
				.Where(i => i.id_company == id_company && i.isPurchased);

			return items.ToList();
		}

		public static IEnumerable AllPurchaseItemsByCompanyLine(int? id_company, string InventoryLine)
		{
			var db = new DBContext();
			List<Item> items = new List<Models.Item>();
			if (!string.IsNullOrEmpty(InventoryLine))
			{
				int id_inventoryLine = db.InventoryLine.Where(x => x.code == InventoryLine).FirstOrDefault().id;

				items = db.Item.Where(i => i.id_company == id_company && i.isPurchased && i.id_inventoryLine == id_inventoryLine).ToList();

			}
			else
			{


				items = db.Item.Where(i => i.id_company == id_company && i.isPurchased).ToList();
			}

			return items;
		}

		public static IEnumerable AllSaleItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.isSold);

			return items.ToList();
		}

		public static IEnumerable AllInventoryItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.inventoryControl);

			return items.ToList();
		}
		public static IEnumerable AllInventoryWarehouseItemsByCompany(int? id_company, int? idWarehouse)
		{
			var db = new DBContext();
			//var items = db.Item.Where(i => i.id_company == id_company && i.inventoryControl);
			//if (idWarehouse != null) items = items.Where(w => w.ItemInventory != null && w.ItemInventory.id_warehouse == idWarehouse);

			var lstItemWarehouse = db.ItemWarehouse.Where(w => w.id_warehouse == idWarehouse).ToList();

			var items = db.Item.Where(i => i.id_company == id_company && i.isActive).ToList();

			items = (from a in lstItemWarehouse
					 join b in items on a.id_item equals b.id
					 select b).ToList();
			//&& i.ItemInventory != null 
			//&& i.ItemInventory.id_warehouse == idWarehouse);
			//if (idWarehouse != null) items = items.Where(w => w.ItemInventory != null && w.ItemInventory.id_warehouse == idWarehouse);

			return items.ToList();
		}

		public static IEnumerable PurchaseItemsByCompanyAndInventoryLine(int? id_company, string code_InventoryLine)
		{
			var db = new DBContext();
			var idInventoryLine = db.InventoryLine.FirstOrDefault(e => e.code == code_InventoryLine)?.id;
            if (idInventoryLine.HasValue && id_company.HasValue)
            {
				return db.Item.Where(i => i.id_company == id_company && i.id_inventoryLine == idInventoryLine && i.isPurchased && i.isActive)
					.Select(e => new { id = e.id, name = e.name })
					.ToList()
					.Select(e => new Item()
					{
						id = e.id,
						name = e.name,
					})
					.ToList();
			}
            else
            {
				return null;
            }
		}

		public static IEnumerable ItemsByCompanyAndInventoryLine(int? id_company, string code_InventoryLine)
		{
			var db = new DBContext();

			var idInventoryLine = db.InventoryLine.FirstOrDefault(e => e.code == code_InventoryLine)?.id;
			if (idInventoryLine.HasValue && id_company.HasValue)
			{
				return db.Item
					.Where(i => i.id_company == id_company && i.id_inventoryLine == idInventoryLine && i.isActive)
					.Select(e => new { id = e.id, name = e.name })
					.ToList()
					.Select(e => new Item() 
					{
						id = e.id,
						name = e.name,
					})
					.ToList();
			}
			else
			{
				return null;
			}
		}

        public static IEnumerable ItemsByCompanyInventoryLineItemTypeAndCurrent(int? id_company, string code_inventoryLine, string code_itemType, int? id_current)
        {
            var db = new DBContext();
            var items = db.Item.Where(i => i.id == id_current || (i.id_company == id_company && i.isActive && i.InventoryLine.code == code_inventoryLine && i.ItemType.code == code_itemType))
                                .Select(s => new
                                {
                                    s.id,
                                    name = s.masterCode + " - " + s.name
                                });

            return items.ToList();
        }

        public static IEnumerable SalesItems()
		{
			var db = new DBContext();

			return db.Item.Where(i => i.isSold && i.isActive).ToList();
		}

		public static IEnumerable SalesItemsPTByCompanyAndCurrent(int? id_company, int? id_current)
		{
			var db = new DBContext();
			var items = db.Item.Where(w => (w.isActive && w.id_company == id_company && w.isSold && w.InventoryLine.code == "PT") || w.id == id_current).ToList();
			//var items = db.Item.Where(i => (i.id_company == id_company && i.isPurchased && i.isActive) || id_company == id_current);

			return items.ToList();
		}

		public static IEnumerable CodeItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.isActive).Select(s => new
			{
				id = s.id,
				name = s.masterCode + " - " + s.name
			});

			return items.ToList();
		}

		public static IEnumerable ItemsByItemTypeItemTypeCategoryAndCurrent(int? id_itemType, int? id_itemTypeCategory, int? id_current)
		{
			var db = new DBContext();
			// var query = db.ItemTypeCategory.Where(t => t.id_itemType == id_itemType && t.isActive);
			var query = db.Item.Where(g => g.id_itemType == id_itemType &&
										   g.id_itemTypeCategory == id_itemTypeCategory &&
										   (g.id == id_current || g.isActive)).ToList();
			return query.ToList();
		}

		public static IEnumerable TrashItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.isActive && i.InventoryLine.code.Equals("DE"));

			return items.ToList();
		}
		public static IEnumerable LossItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.isActive && i.InventoryLine.code.Equals("DE") &&
										   i.ItemInventory.requiresLot == true);

			return items.ToList();
		}

		public static IEnumerable AllMDDItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.InventoryLine.code.Equals("MI") && i.ItemType.code.Equals("MDD"));//MI Materiales e Insumos//MDD Materiales de Despacho

			return items.ToList();
		}

		public static IEnumerable AllMDEItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.InventoryLine.code.Equals("MI") && i.ItemType.code.Equals("INS") && i.ItemTypeCategory.code.Equals("MDE"));//MI Materiales e Insumos//INS Insumos//Materiales de Empaque

			return items.ToList();
		}

		public static IEnumerable AllMIItemsByCompany(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.InventoryLine.code.Equals("MI"));//MI Materiales e Insumos

			return items.ToList();
		}

		public static IEnumerable ItemsByWarehousesWarehouseLocationAndCurrent(int? id_warehouse, int? id_warehouseLocation, int? id_current)
		{
			var db = new DBContext();
			var inventoryMoveDetails = db.InventoryMoveDetail.Where(i => i.id_warehouse == id_warehouse &&
																		 i.id_warehouseLocation == id_warehouseLocation &&
																		 i.id_inventoryMoveDetailNext == null &&
																		 i.balance != 0);

			var itemCurrent = db.Item.FirstOrDefault(fod => fod.id == id_current);
			var items = new List<Item>();
			if (itemCurrent != null) items.Add(itemCurrent);

			foreach (var detail in inventoryMoveDetails)
			{
				if (!items.Contains(detail.Item))
				{
					items.Add(detail.Item);
				}
			}
			return items;
		}

		public static IEnumerable ItemsIsCurrent(int? id_current)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id == id_current);

			return items.ToList();
		}

		public static IEnumerable ItemByCompanyDocumentTypeOportunityAndCurrent(int? id_company, string codeDocumentTypeOportunity, int? id_current)
		{
			var db = new DBContext();
			var items = db.Item.Where(w => (w.isActive && w.id_company == id_company &&
										   ((codeDocumentTypeOportunity == "15" && w.isSold) || (codeDocumentTypeOportunity == "16" && w.isPurchased))) ||
										   w.id == id_current).Select(s => new
										   {
											   id = s.id,
											   masterCode = s.masterCode,
											   MetricUnitCode = (codeDocumentTypeOportunity) == "15" ? (s.ItemSaleInformation != null ? s.ItemSaleInformation.MetricUnit.code : "") :
																((codeDocumentTypeOportunity) == "16" ? (s.ItemPurchaseInformation != null ? s.ItemPurchaseInformation.MetricUnit.code : "") : ("")),
											   name = s.name
										   }).ToList();

			return items;
		}


		public static IEnumerable ItemInvoiceCommercial(int? id_company)
		{
			var db = new DBContext();
			var codeFEXP = db.Setting.FirstOrDefault(fod => fod.code == "FEXP")?.value ?? "";
			var codeMaster = db.Setting.FirstOrDefault(fod => fod.code == "PMASTER")?.value ?? "";

			var _items = db.Item
					   .Where(w => w.isSold
								  && w.isActive
								  && w.id_company == id_company
								  && w.InventoryLine.code == codeFEXP
								  && w.Presentation.code.Substring(0, 1) == codeMaster
							  ).Select(s => new { s.id, s.auxCode, s.name, s.masterCode }).ToList();

			//var iii = _items.FirstOrDefault(r => r.id == 2553);

			//var itemsReturn = _items.Select(r => new
			//                {
			//                    auxCode = r.auxCode,
			//                    id = r.id,
			//                    masterCode = r.masterCode,
			//                    name = r.name
			//                });




			return _items;

		}

		///<summary>
		/// Autor: {RA}
		/// Descripcion: Origen de Datos Items facturables al exterior
		///</summary>
		public static IEnumerable SalesItemsByCompany(int? id_company, List<int> id_items)
		//public static IEnumerable SalesItemsByCompany(int? id_company)
		{

			var db = new DBContext();
			String rutaLog = ConfigurationManager.AppSettings["rutaLog"];
			List<Item> itemsFacturaExportacion = new List<Item>();


			Boolean existsItemList = (id_items == null) ? false : (id_items.Count() > 0);

			try
			{


				String prefijoMaster = db.Setting.First(r => r.code == "PMASTER").value;
				String lineaInventario = db.Setting.First(r => r.code == "FEXP").value;

				if (prefijoMaster == null && lineaInventario == null)
				{
					MetodosEscrituraLogs.EscribeMensajeLog("No se han definido Setting: PMASTER o  FEXP", rutaLog, "DP-Item:SalesItemsByCompany", "APP PRODUCCION");
				}
				else
				{
					itemsFacturaExportacion = db.Item.Where(i =>
														i.isSold &&
														i.isActive &&
														i.id_company == id_company &&
														((i.InventoryLine != null) ? i.InventoryLine.code == lineaInventario : 1 == 2) &&
														((i.Presentation != null) ? i.Presentation.code.Substring(0, 1) == prefijoMaster : 1 == 2)
														).OrderBy(o=> o.masterCode).ToList();

					if (existsItemList)
					{
						itemsFacturaExportacion = itemsFacturaExportacion.Where(i => !id_items.Contains(i.id)).ToList();
					}

				}


			}
			catch (Exception e)
			{
				MetodosEscrituraLogs.EscribeExcepcionLog(e, rutaLog, "DP-Item:SalesItemsByCompany", "APP PRODUCCION");
			}

			return itemsFacturaExportacion.ToList();
		}

		public static IEnumerable SalesItemsBuyerByCompany(int? id_company, int? id_buyer, List<int> id_items,
															string nameItem, int? sizeBegin, int? sizeEnd,
															int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory,
															int? id_group, int? id_subgroup, int? id_size,
															int? id_trademark, int? id_trademarkModel, int? id_color,
															string nameCodigoItem)
		//public static IEnumerable SalesItemsByCompany(int? id_company)
		{

			var db = new DBContext();
			String rutaLog = ConfigurationManager.AppSettings["rutaLog"];
			List<Item> itemsFacturaExportacion = new List<Item>();
			List<Item> itemsNew = new List<Item>();

			Boolean existsItemList = (id_items == null) ? false : (id_items.Count() > 0);

			try
			{

				String prefijoMaster = db.Setting.First(r => r.code == "PMASTER").value;
				String lineaInventario = db.Setting.First(r => r.code == "FEXP").value;
				string busqItem = db.Setting.FirstOrDefault(fod => fod.code == "PROITEM")?.value ?? "0";
				if (prefijoMaster == null && lineaInventario == null)
				{
					MetodosEscrituraLogs.EscribeMensajeLog("No se han definido Setting: PMASTER o  FEXP", rutaLog, "DP-Item:SalesItemsByCompany", "APP PRODUCCION");
				}
				else
				{
					if(busqItem != "0")
					{
						var int_DFEAFA = int.Parse(busqItem);

						DateTime nowDate = DateTime.Now;
						DateTime emissionMinDate = nowDate.AddDays(-int_DFEAFA);
						DateTime emissionMinDateNew = nowDate.AddDays(-30); 

						var itIds = db.InvoiceDetail.Where(i => i.Invoice.id_buyer == id_buyer &&
														i.Invoice.Document.emissionDate > emissionMinDate).Distinct().Select(r => r.id_item).ToList();

						var itemsNew1 = db.Item.Where(i => i.dateCreate > emissionMinDateNew || i.dateUpdate > emissionMinDateNew).ToList();

						itemsFacturaExportacion = db.Item.Where(i =>
														i.isSold &&
														i.isActive &&
														i.id_company == id_company &&
														((i.InventoryLine != null) ? i.InventoryLine.code == lineaInventario : 1 == 2) &&
														((i.Presentation != null) ? i.Presentation.code.Substring(0, 1) == prefijoMaster : 1 == 2) &&
														(itIds.Contains(i.id) || i.dateCreate > emissionMinDateNew || i.dateUpdate > emissionMinDateNew)
														).OrderBy(o => o.foreignName).ThenBy(o => o.masterCode).ToList();

					}
					else
					{
						itemsFacturaExportacion = db.Item.Where(i =>
														i.isSold &&
														i.isActive &&
														i.id_company == id_company &&
														((i.InventoryLine != null) ? i.InventoryLine.code == lineaInventario : 1 == 2) &&
														((i.Presentation != null) ? i.Presentation.code.Substring(0, 1) == prefijoMaster : 1 == 2)
														).OrderBy(o => o.masterCode).ToList();
					}

					if (existsItemList)
					{
						itemsFacturaExportacion = itemsFacturaExportacion.Where(i => !id_items.Contains(i.id)).ToList();
					}

					if (!String.IsNullOrEmpty(nameItem))
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.name.Contains(nameItem)).ToList();
					}
					if (!String.IsNullOrEmpty(nameCodigoItem))
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.masterCode.Contains(nameCodigoItem)).ToList();
					}

					if (sizeBegin > 0)
					{
						var orderSizeBegin = db.ItemSize
							.FirstOrDefault(fod => fod.id == sizeBegin)?
							.orderSize;

						if (orderSizeBegin.HasValue)
						{
							itemsFacturaExportacion = (from v in itemsFacturaExportacion
													   join c in db.ItemGeneral on v.id equals c.id_item
										  join d in db.ItemSize on c.id_size equals d.id
										  where d.orderSize >= orderSizeBegin.Value && d.isActive
										  select v).ToList();
						}
					}

					if (sizeEnd > 0)
					{
						var orderSizeEnd = db.ItemSize
							.FirstOrDefault(fod => fod.id == sizeEnd)?
							.orderSize;

						if (orderSizeEnd.HasValue)
						{
							itemsFacturaExportacion = (from v in itemsFacturaExportacion
													   join c in db.ItemGeneral on v.id equals c.id_item
										  join d in db.ItemSize on c.id_size equals d.id
										  where d.orderSize <= orderSizeEnd.Value && d.isActive
										  select v).ToList();
						}
					}

					if (id_inventoryLine > 0)
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.id_inventoryLine == id_inventoryLine).ToList();
					}

					if (id_itemType > 0)
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.id_itemType == id_itemType).ToList();
					}

					if (id_itemTypeCategory > 0)
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.id_itemTypeCategory == id_itemTypeCategory).ToList();
					}

					if (id_group > 0)
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.ItemGeneral.id_group == id_group).ToList();
					}

					if (id_subgroup > 0)
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.ItemGeneral.id_subgroup == id_subgroup).ToList();
					}

					if (id_size > 0)
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.ItemGeneral.id_size == id_size).ToList();
					}

					if (id_trademark > 0)
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.ItemGeneral.id_trademark == id_trademark).ToList();
					}

					if (id_trademarkModel > 0)
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.ItemGeneral.id_trademarkModel == id_trademarkModel).ToList();
					}

					if (id_color > 0)
					{
						itemsFacturaExportacion = itemsFacturaExportacion
							.Where(w => w.ItemGeneral.id_color == id_color).ToList();
					}

				}


			}
			catch (Exception e)
			{
				MetodosEscrituraLogs.EscribeExcepcionLog(e, rutaLog, "DP-Item:SalesItemsByCompany", "APP PRODUCCION");
			}

			return itemsFacturaExportacion.ToList();
		}
		public static List<ItemModelP> GetListItemModelP()
		{
			var db = new DBContext();
			return db.Item
						.Where(w => w.isActive)
						.Select(s => new ItemModelP
						{
							idModelP = s.id,
							masterCodeModelP = s.masterCode,
							auxCodeModelP = s.auxCode,
							nameModelP = s.name,
							descriptionModelP = s.description
						}).ToList();
		}
		public static List<ItemModelP> GetListItemModelP(string codeInventoryLine)
		{
			var db = new DBContext();
			return db.Item
						.Where(w => w.isActive
						&& w.InventoryLine.code.Equals(codeInventoryLine))
						.Select(s => new ItemModelP
						{
							idModelP = s.id,
							masterCodeModelP = s.masterCode,
							auxCodeModelP = s.auxCode,
							nameModelP = s.name,
							descriptionModelP = s.description
						}).ToList();
		}
		public static List<ItemInventoryModelP> GetListItemInventory()
		{
			var db = new DBContext();
			return db.ItemInventory
						.Select(s => new ItemInventoryModelP
						{
							idItemModelP = s.id_item,
							idWaresHouseModelP = s.id_warehouse,
							idWarehouseLocationModelP = s.id_warehouseLocation,
							idMetricUnitInventoryModelP = s.id_metricUnitInventory
						}).ToList();
		}

		public static List<ItemModelP> GetListItemModelPByCustomInventoryLine()
		{
			var db = new DBContext();



			return db.Item
						.Where(r => (r.InventoryLine.code == "PT" || r.InventoryLine.code == "PP") && r.isActive)
						.Select(s => new ItemModelP
						{
							idModelP = s.id,
							masterCodeModelP = s.masterCode,
							auxCodeModelP = s.auxCode,
							nameModelP = s.name,
							descriptionModelP = s.description,
							quantity = 0
						})
						.OrderBy(o => o.masterCodeModelP)
						.ToList();


		}


		public static List<ItemModelP> GetListItemModelPByProductionLot(int? idProductionLot)
		{
			var db = new DBContext();

			if (idProductionLot == null) return new List<ItemModelP>();


			var resultProductionLotDetail = db.ProductionLot
													   .FirstOrDefault(r => r.id == idProductionLot)
													   .ProductionLotDetail;

			if (resultProductionLotDetail == null) return new List<ItemModelP>();

			return resultProductionLotDetail
						.GroupBy(r => r.id_item)

						.Select(s => new ItemModelP
						{
							idModelP = s.Key,
							masterCodeModelP = s.Max(g => g.Item.masterCode),
							auxCodeModelP = s.Max(g => g.Item.auxCode),
							nameModelP = s.Max(g => g.Item.name),
							descriptionModelP = s.Max(g => g.Item.description),
							quantity = decimal.Round((int)s.Sum(g => g.quantitydrained), 2)
						}).ToList();


		}

		public static IEnumerable GetMetricUnit()
		{
			var db = new DBContext();
			return db.MetricUnit
						.Select(s => new MetricUnitModelP
						{
							idMetricUnitModelP = s.id,
							codeMetricUnitModelP = s.code,
							nameMetricUnitModelP = s.name
						}).ToList();
		}

		public static IEnumerable ItemsWithLotInventoryCodeDocumentTypeWarehouseWarehouseLocationAndCurrent(
			int companyId,
			int? id_itemCurrent, 
			bool? withLotSystem, 
			bool? withLotCustomer, 
			string code, 
			int? idWarehouse, 
			int? idWarehouseLocation,
			DateTime? fechaEmision,
            int? id_itemType, 
			int? id_size, 
			int? id_trademark, 
			int? id_presentation,
			string codigoProducto,
			int? categoriaProducto,
            int? modeloProducto
            )
		{
			var db = new DBContext();
			Item[] items = Array.Empty<Item>(); // new List<Item>();
			if (code == "03")
			{//"03" Ingreso
				items = db.Item.Where(w => w.isActive && (w.ItemInventory.requiresLot == ((withLotSystem ?? false) || (withLotCustomer ?? false))) ||
												(w.id == id_itemCurrent)).ToArray();
            }
			else
			{
				if ((code == "05" || code == "32" || code == "129") && idWarehouse != null && idWarehouseLocation != null && fechaEmision.HasValue)
				{//"05" Egreso, 32: Egreso por Transferencia y 129: Egreso Por Transferencia Automática

					bool requierLot = ((withLotSystem ?? false) || (withLotCustomer ?? false));
                    items = ServiceInventoryMove.GetItemsWithBalance(	db,
																		companyId, 
																		idWarehouse.Value, 
																		idWarehouseLocation.Value, 
																		requierLot, 
																		fechaEmision.Value,
                                                                        id_itemCurrent
                                                                        );                   
                }
				else
				{
					items = db.Item.Where(w => (w.id == id_itemCurrent)).ToArray();
				}
			}

            if (id_size != null && id_size > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_size == id_size).ToArray();
            }
            if (id_itemType != null && id_itemType > 0)
            {
                items = items
                    .Where(w => w.id_itemType == id_itemType).ToArray();
            }

            if (id_trademark != null && id_trademark > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_trademark == id_trademark).ToArray();
            }

            if (modeloProducto != null && modeloProducto > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_trademarkModel == modeloProducto).ToArray();
            }

            if (categoriaProducto != null && categoriaProducto > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_groupCategory == categoriaProducto).ToArray();
            }

            if (id_presentation != null && id_presentation > 0)
            {
                items = items
                    .Where(w => w.id_presentation == id_presentation).ToArray();
            }

            if (!String.IsNullOrEmpty(codigoProducto))
            {
                items = items
                    .Where(w => w.masterCode.Contains(codigoProducto)).ToArray();
            }

            items = items.Select(s => new Item
			{
				id = s.id,
				masterCode = s.masterCode,
				name = s.name
			}).ToArray();
			return items;

		}
        public static Item ItemEquivalence(int? id)
        {
            var db = new DBContext();
			var item = new Item();
			item = db.Item.FirstOrDefault(i => i.id == id);
            var itemProcessTypeAux = (item != null) && (item.ItemType != null) ? item.ItemType : null;
            var codeAux = itemProcessTypeAux != null && itemProcessTypeAux.ProcessType != null ? itemProcessTypeAux.ProcessType.code : "";
			//if (codeAux == "ENT")
			//{
            var itemEquivalence = db.ItemEquivalence.FirstOrDefault(i => i.id_item == id);
            item = db.Item.FirstOrDefault(i => i.id == itemEquivalence.id_itemEquivalence);
            //}

			return item;
        }
        public static decimal GetMinimoProductionProcessWMasterCalc(int? idItem)
		{

			decimal model = 1;

			var db = new DBContext();
			//string[] ivLines = new string[] { "PT", "PP" };
			if (idItem == null) return model;

			var item  = db.Item.FirstOrDefault(i => i.id == idItem);
			if (item == null) return model;

			if (item.Presentation?.code==null   ) return model;

			if (item.Presentation.code.Substring(0, 1) != "M")
			{
				return item.Presentation.minimum;
			}
			return (item.Presentation.minimum * item.Presentation.maximum);
		}

		public static IEnumerable AllItemsWithFormulation(int? id_company)
		{
			var db = new DBContext();
			var items = db.Item.Where(i => i.id_company == id_company && i.hasFormulation);

			items = items.Where(e => e.ItemIngredient.Any());

			return items.ToList();
		}
		public static IEnumerable AllItemsWithPPPT(int? id_company)
		{
			var db = new DBContext();
			var codigosInventoryLine = new string[]
			{
				"PT", "PP"
			};
			var idsInventoryLine = db.InventoryLine
				.Where(e => codigosInventoryLine.Contains(e.code))
				.Select(e => e.id)
				.ToArray();

			var items = db.Item
				.Where(i => i.id_company == id_company && idsInventoryLine.Contains(i.id_inventoryLine) && i.isActive)
				.OrderBy(e => e.id_inventoryLine)
				.ThenBy(e => e.masterCode)
				.Select(e => new {
					e.id,
					e.masterCode,
					e.name
				});

			return items.ToList();
		}
	}
}

