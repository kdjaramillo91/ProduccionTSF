using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.InventoryBalance;
using DXPANACEASOFT.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static DXPANACEASOFT.Services.ServiceInventoryBalance;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderAutomaticTransfer
	{
		public static IEnumerable ItemsWithLotInventoryCodeDocumentTypeWarehouseWarehouseLocationAndCurrent(
            int activeCompanyId, 
			int? id_itemCurrent,
			bool? withLotSystem, 
			bool? withLotCustomer, 
			int? idWarehouse, 
			int? idWarehouseLocation,
            int? id_itemType,
            int? id_size,
            int? id_trademark,
            int? id_presentation,
            string codigoProducto,
            int? categoriaProducto,
            int? modeloProducto,
            DateTime? fechaEmision = null)
		{
			var db = new DBContext();
			List<Item> items = new List<Item>();

            bool requiresLot = ((withLotSystem ?? false) || (withLotCustomer ?? false));
            //var idsItemsSaldo = new Services.ServiceInventoryMove()
            //    .GetSaldosProductoLote(requiresLot, idWarehouse, idWarehouseLocation, id_itemCurrent, null, null, fechaEmision)
            //    .Select(e => e.id_item)
            //    .Distinct()
            //    .ToArray();

            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(	 new InvParameterBalanceGeneral
            {
                requiresLot = requiresLot,
                id_Warehouse = idWarehouse,
                id_WarehouseLocation = idWarehouseLocation,
                id_Item = id_itemCurrent,
                id_ProductionLot = null,
                lotMarket = null,
                id_productionCart = null,
                cut_Date = fechaEmision,
                id_company = activeCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;
            var idsItemsSaldo = saldos
									.Select(e => e.id_item)
									.Distinct()
									.ToArray();

            items = db.Item
                .Where(e => idsItemsSaldo.Contains(e.id) && e.isActive)
                .ToList();

            if (id_size != null && id_size > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_size == id_size).ToList();
            }
            if (id_itemType != null && id_itemType > 0)
            {
                items = items
                    .Where(w => w.id_itemType == id_itemType).ToList();
            }

            if (id_trademark != null && id_trademark > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_trademark == id_trademark).ToList();
            }

            if (modeloProducto != null && modeloProducto > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_trademarkModel == modeloProducto).ToList();
            }

            if (categoriaProducto != null && categoriaProducto > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_groupCategory == categoriaProducto).ToList();
            }

            if (id_presentation != null && id_presentation > 0)
            {
                items = items
                    .Where(w => w.id_presentation == id_presentation).ToList();
            }

            if (!String.IsNullOrEmpty(codigoProducto))
            {
                items = items
                    .Where(w => w.masterCode.Contains(codigoProducto)).ToList();
            }


            var model = items.Select(s => new 
			{
				id = s.id,
				masterCode = s.masterCode,
				name = s.name,
                id_metricUnitInventory = s.ItemInventory.id_metricUnitInventory
            }).ToList();
			return model;

		}
		public static IEnumerable ItemsWithLotInventory(int? id_itemCurrent
			, bool? withLotSystem, bool? withLotCustomer, int? id_warehouse, int? id_warehouse_location)
		{
			var db = new DBContext();
			List<Item> items = new List<Item>();
			
			var inventoryMoveDetails = db.InventoryMoveDetail
												.Where(fod => fod.InventoryMove.Document.DocumentState.code == "03" 
												&& fod.id_warehouse == id_warehouse 
												&& fod.id_warehouseLocation == id_warehouse_location
                                                //&& (
                                                //(
                                                //fod.Item.ItemInventory.requiresLot == ((withLotSystem ?? false) || (withLotCustomer ?? false))
                                                //)
                                                //|| (fod.id == id_itemCurrent)
                                                //)
                                                );

			if (inventoryMoveDetails.Count() > 0)
			{
				var itemsLotSaldo = inventoryMoveDetails
									.GroupBy(r => new { r.id_item, r.id_lot, r.Lot })
									.Select(s => new
									{
										id_item = s.Key.id_item,
										id_lot = s.Key.id_lot,
										saldo = s.Sum(g => g.entryAmount - g.exitAmount),
										number = ((s.Key.Lot.number ?? "") + ((s.Key.Lot.number != "" && s.Key.Lot.number != null && s.Key.Lot.internalNumber != "" && s.Key.Lot.internalNumber != "") ? "-" : "") + (s.Key.Lot.internalNumber ?? ""))//s.Lot.number
									}).ToList();
				itemsLotSaldo = itemsLotSaldo.Where(w => w.saldo > 0).Distinct()
					.ToList();
				items = db.Item.AsEnumerable().Where(w => w.isActive && itemsLotSaldo.FirstOrDefault(fod => fod.id_item == w.id) != null).ToList();
			}

			if (id_itemCurrent != null && id_itemCurrent > 0)
			{
				items = items.Where(w => w.id == id_itemCurrent).ToList();
			}
			
			var model = items.Select(s => new 
			{
				id = s.id,
				masterCode = s.masterCode,
				name = s.name,
				id_metricUnitInventory = s.ItemInventory.id_metricUnitInventory
			}).ToList();
			return model;

		}
		public static IEnumerable ItemDetail(int? id_warehouse, int? id_warehouse_location)
		{
			using (var db = new DBContext())
			{
				return db.Item.Where(w => w.isActive).Select(s => new
				{
					id = s.id,
					mastercode = s.masterCode,
					name = s.name
				}).ToList();
			}
		}
		public static IEnumerable MetricUnit()
		{
			using (var db = new DBContext())
			{
				return db.MetricUnit.Where(w => w.isActive).Select(s => new
				{
					id = s.id,
					name = s.code
				}).ToList();
			}
		}
		public static IEnumerable LotDetail2(int? id_warehouseAux, int? id_warehouseLocationAux, int? id_item, int? id_lot)
		{
			/*
			 
			 */
			using (var db = new DBContext())
			{
				var lots = db.InventoryMoveDetail.Where(w => w.id_warehouse == id_warehouseAux &&
														  w.id_warehouseLocation == id_warehouseLocationAux &&
														  w.id_item == id_item &&
														  w.id_inventoryMoveDetailNext == null &&
														  w.InventoryMove.Document.DocumentState.code.Equals("03") &&//"03": APROBADA
														  w.id_lot != null)
									   .Select(s => new
									   {
										   id = s.id_lot,
										   number = s.Lot.number,
										   internalNumber = s.Lot.ProductionLot.internalNumber
									   }).ToList();

				if (id_lot != null) 
				{
					lots = lots.Where(w => w.id == id_lot).ToList();
				}

				return lots;
			}
		}

		public static IEnumerable LotDetail(	int activeCompanyId, 
												int? id_inventoryMoveDetail, 
												int? id_itemCurrent, 
												int? id_metricUnitMove,
												int? id_warehouse, 
												int? id_warehouseLocation, 
												int? id_warehouseEntry, 
												int? id_warehouseLocationEntry, 
												int? id_lot, 
												bool? withLotSystem, 
												bool? withLotCustomer, 
												DateTime? fechaEmision = null)
		{
            var db = new DBContext();
            List<Item> items = new List<Item>();
            List<Lot> lots = new List<Lot>();
			var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";

			bool requiresLot = ((withLotSystem ?? false) || (withLotCustomer ?? false));
            //lots = new Services.ServiceInventoryMove()
            //    .GetSaldosProductoLote(requiresLot, id_warehouse, id_warehouseLocation, id_itemCurrent, id_lot, null, fechaEmision)
            //    .Select(e => new Lot() {
            //        id = e.id_lote ?? 0,
            //        number = e.number + 
            //            (!String.IsNullOrEmpty(e.internalNumber) ? $" / {e.internalNumber}" : string.Empty)
			//			+ (lotMarkedPar == "SI" ? $" / {e.lot_market}" : string.Empty),
			//		internalNumber = e.internalNumber,
            //    })
            //    .ToList();


            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
            {
                requiresLot = requiresLot,
                id_Warehouse = id_warehouse,
                id_WarehouseLocation = id_warehouseLocation,
                id_Item = id_itemCurrent,
                id_ProductionLot = id_lot,
                lotMarket = null,
                id_productionCart = null,
                cut_Date = fechaEmision,
                id_company = activeCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;

            lots = saldos
						.Select(e => new Lot()
						{
							id = e.id_lote ?? 0,
							number = e.number +
									(!String.IsNullOrEmpty(e.internalNumber) ? $" / {e.internalNumber}" : string.Empty)
									+ (lotMarkedPar == "SI" ? $" / {e.lot_market}" : string.Empty),
							internalNumber = e.internalNumber,
						})
						.ToList();

            return lots;
        }
    }
}