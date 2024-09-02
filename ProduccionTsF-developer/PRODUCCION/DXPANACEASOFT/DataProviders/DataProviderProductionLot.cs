using System;
using System.Collections;
using System.Linq;
using System.Data.Entity;

using DXPANACEASOFT.Models;
using System.Collections.Generic;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using System.Web.Mvc;
using DXPANACEASOFT.Models.InventoryMoveDTO;
using DXPANACEASOFT.Models.InventoryBalance;
using DXPANACEASOFT.Services;
using static DXPANACEASOFT.Services.ServiceInventoryBalance;
//using DocumentFormat.OpenXml.Spreadsheet;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderProductionLot
    {
        private static DBContext db = null;

        public static IEnumerable ProductionLots()
        {
            db = new DBContext();
            return db.ProductionLot.ToList();
        }

        public static IEnumerable productProcessProductionLotRecepctionDistibuted()
        {
            db = new DBContext();

            return db.Item.Where(e => e.id_inventoryLine == 12 && e.isActive == true && e.Presentation.maximum == 1).Select(e => new { id = e.id, masterCode = e.masterCode, description = e.description, category = e.ItemTypeCategory.name, size = e.ItemGeneral.ItemSize.name }).ToList();
        }

        public static Item ProductionLotPaymentNameItem(int? codigo)
        {
            db = new DBContext();
            var codeItem = db.Item.FirstOrDefault(e => e.id == codigo);
            if (codeItem != null)
            {
                return codeItem;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable ProductionLotByCompany(int? id_company)
        {
            db = new DBContext();
            return db.ProductionLot.Where(w => w.id_company == id_company).ToList();
            //&& (w.InventoryMove != null) && (w.InventoryMove.FirstOrDefault(fod=> fod.Document.DocumentState.code.Equals("03") && fod.InventoryMoveDetail.Any(a=> a.id_inventoryMoveDetailNext == null && a.balance > 0)) != null)).ToList();
        }
        public static IEnumerable LotByCompany(int? id_company)
        {
            db = new DBContext();
            var lots = db.Lot.Where(w => w.id_company == id_company).Select(s => new { id = s.id, number = ((s.number ?? "") + ((s.number != "" && s.number != null && s.internalNumber != "" && s.internalNumber != "") ? "-" : "") + (s.internalNumber ?? "")) }).ToList();
            return lots;
        }

        public static Lot LotManualInventoryMoveById(int? id)
        {
            db = new DBContext();
            return db.Lot.FirstOrDefault(t => t.id == id);
        }

        public static IEnumerable LotItemLotMarked(int? id_lot, int? id_item)
        {
            db = new DBContext();
            var lotMarked = db.MasteredDetail.FirstOrDefault(fod => fod.id_productPT == id_item && fod.id_lotMP == id_lot)?.lotMarked;
            return lotMarked;
        }

        public static ProductionLot ProductionLotById(int? id)
        {
            db = new DBContext();
            return db.ProductionLot.FirstOrDefault(t => t.id == id);
        }

        public static Warehouse WarehouseByProductionLotAndItem(int? id_productionLot, int? id_item)
        {
            db = new DBContext();
            return db.ProductionLotLiquidation.FirstOrDefault(t => t.id_productionLot == id_productionLot && t.id_item == id_item)?.Warehouse;
        }

        public static WarehouseLocation WarehouseLocationByProductionLotAndItem(int? id_productionLot, int? id_item)
        {
            db = new DBContext();
            return db.ProductionLotLiquidation.FirstOrDefault(t => t.id_productionLot == id_productionLot && t.id_item == id_item)?.WarehouseLocation;
        }

        public static decimal? QuantityRecivedByProductionLotAndItem(int? id_productionLot, int? id_item)
        {
            db = new DBContext();
            return db.ProductionLotLiquidation.FirstOrDefault(t => t.id_productionLot == id_productionLot && t.id_item == id_item)?.quantity;
        }

        public static decimal ExistingAmount(int? id_productionLot, int? id_item, int? id_warehouse, int? id_warehouseLocation)
        {
            db = new DBContext();
            //ProductionLotLiquidation productionLotLiquidation = db.ProductionLotLiquidation.FirstOrDefault(p => p.id_productionLot == id_productionLot && p.id_item == id_item &&
            //                                                                                                    p.id_warehouse == id_warehouse && p.id_warehouseLocation == id_warehouseLocation);
            //var productionLotDetail = db.ProductionLotDetail.Where(p => p.id_originLot == id_productionLot && p.id_item == id_item &&
            //                                                            p.id_warehouse == id_warehouse && p.id_warehouseLocation == id_warehouseLocation && p.ProductionLot.ProductionLotState.code != "09");
            //var usedProductionLotLiquidation = (productionLotDetail?.Count() ?? 0) > 0 ? productionLotDetail.Sum(s => s.quantityRecived) : 0;
            //var inventoryDetailProductionLot = db.InventoryMoveDetail.FirstOrDefault(w => (w.InventoryMove.Document.DocumentState.code.Equals("03") && w.id_inventoryMoveDetailNext == null &&
            //																			  w.id_lot == id_productionLot && w.id_item == id_item &&
            //																			  w.id_warehouse == id_warehouse && w.id_warehouseLocation == id_warehouseLocation));
            //return ((productionLotLiquidation?.quantity ?? 0) - usedProductionLotLiquidation);
            //return (inventoryDetailProductionLot?.balance ?? 0);
            var inventoryMoveDetailAux = db.InventoryMoveDetail.Where(w => w.id_warehouse == id_warehouse &&
                                                                         w.id_warehouseLocation == id_warehouseLocation &&
                                                                         w.id_item == id_item &&
                                                                         //w.InventoryMove.Document.DocumentState.code != "05" &&
                                                                         w.InventoryMove.Document.DocumentState.code != "05" &&
                                                                         //w.InventoryMove.Document.DocumentState.code.Equals("03") &&//"03": APROBADA
                                                                         w.id_lot == (id_productionLot == 0 ? null : id_productionLot)).ToList();
            decimal remainingBalance = inventoryMoveDetailAux.Count() > 0 ? inventoryMoveDetailAux.Sum(s => s.entryAmount - s.exitAmount) : 0;
            return remainingBalance;
        }


        public static decimal ExistingAmountProcess(    int activeCompanyId, 
                                                        int? id_productionLot,
                                                        int? id_item, 
                                                        int? id_warehouse, 
                                                        int? id_warehouseLocation, 
                                                        DateTime? fechaProceso = null)
        {
            db = new DBContext();

            var requiresLot = db.ItemInventory
                .FirstOrDefault(e => e.id_item == id_item)?
                .requiresLot ?? false;

            //var saldos = new Services.ServiceInventoryMove()
            //    .GetSaldosProductoLote(requiresLot, id_warehouse, id_warehouseLocation, id_item, id_productionLot, null, fechaProceso);


            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
            {
                requiresLot = requiresLot,
                id_Warehouse = id_warehouse,
                id_WarehouseLocation = id_warehouseLocation,
                id_Item = id_item,
                id_ProductionLot = id_productionLot,
                lotMarket = null,
                id_productionCart = null,
                cut_Date = fechaProceso,
                id_company = activeCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE

            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;

            if (id_productionLot.HasValue)
            {
                var saldo = saldos
                    .FirstOrDefault(e => e.id_lote == id_productionLot.Value)?
                    .saldo ?? 0;

                return saldo > 0 ? saldo : 0m;
            }
            else
            {
                var saldo = saldos.Sum(e => e.saldo);

                return saldo > 0 ? saldo : 0m;
            }
        }

        public static Lot LotById(int? id)
        {
            db = new DBContext();
            return db.Lot.FirstOrDefault(t => t.id == id);
        }

        public static IEnumerable LotsByWarehousesWarehouseLocationItemAndCurrent(int? id_warehouse, int? id_warehouseLocation, int? id_item, int? id_current)
        {
            db = new DBContext();
            var inventoryMoveDetails = db.InventoryMoveDetail.Where(i => i.id_warehouse == id_warehouse &&
                                                                         i.id_warehouseLocation == id_warehouseLocation &&
                                                                         i.id_item == id_item &&
                                                                         i.id_inventoryMoveDetailNext == null &&
                                                                         i.balance != 0);
            var lotZero = new Lot { id = 0, number = "(Sin Lote)" };
            var lotCurrent = db.Lot.FirstOrDefault(fod => fod.id == id_current);

            var lots = new List<Lot>();
            if (id_current == 0)
            {
                lots.Add(lotZero);
            }
            else
            {
                if (lotCurrent != null) lots.Add(lotCurrent);
            }

            foreach (var detail in inventoryMoveDetails)
            {
                if (!lots.Contains(detail.Lot ?? lotZero))
                {
                    lots.Add(detail.Lot ?? lotZero);
                }
            }
            return lots;
        }

        public static IEnumerable GetProductionLotSingle()
        {
            db = new DBContext();
            return db.ProductionLot
                        .Select(s => new ProductionLotSingleModelP
                        {
                            idProductionLotSingleModelP = s.id,
                            numberProductionLotSingleModelP = s.number,
                            internalNumberProductionLotSingleModelP = s.internalNumber,
                            id_providerProductionLotSingleModelP = s.id_provider
                        }).ToList();
        }

        public static IEnumerable GetProductionLotSingleWithDays(int daysAfter = 0)
        {

            db = new DBContext();

            var resultProductionLot = db.ProductionLot
                                            //.AsParallel()
                                            .Where(r => r.ProductionLotState.code == "02" &&
                                                        r.receptionDate >= DbFunctions.AddDays(DateTime.Now, -daysAfter)


                                            )
                                            .ToList();




            if (resultProductionLot == null) return new List<ProductionLotSingleModelP>();

            return resultProductionLot
                    //.AsParallel()
                    .Select(s => new ProductionLotSingleModelP
                    {
                        idProductionLotSingleModelP = s.id,
                        numberProductionLotSingleModelP = s.number,
                        internalNumberProductionLotSingleModelP = s.internalNumber,
                        id_providerProductionLotSingleModelP = s.id_provider,
                        nameProviderProductionLotSingleModelP = (s.Provider == null) ? "" : (s.Provider.Person.fullname_businessName),
                        nameUnitProviderProductionLotSingleModelP = (s.ProductionUnitProvider == null) ? "" : (s.ProductionUnitProvider.name)
                    }).ToList();

        }
        public static IEnumerable LotsWithItemInventoryCodeDocumentTypeWarehouseWarehouseLocation(string codeDocumentType, int? id_warehouse, int? id_warehouseLocation, int? id_lot, bool? withLotSystem, bool? withLotCustomer)
        {
            db = new DBContext();
            List<Item> items = new List<Item>();
            List<Lot> lots = new List<Lot>();
            List<ItemInvMoveDetailLot> itemsLotSaldo = new List<ItemInvMoveDetailLot>();
            var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";
            if ((codeDocumentType == "05" || codeDocumentType == "32" || codeDocumentType == "129") && id_warehouse != null && id_warehouseLocation != null)//05: Egreso, 32: Egreso por transferencia y 129: Egreso Por Transferencia Automática
            {
                var inventoryMoveDetails = db.InventoryMoveDetail.Where(fod => fod.InventoryMove.Document.DocumentState.code == "03" && //"03":APROBADA
                                                                               fod.id_warehouse == id_warehouse &&
                                                                               fod.id_warehouseLocation == id_warehouseLocation &&
                                                                               ((fod.Item.ItemInventory.requiresLot == ((withLotSystem ?? false) || (withLotCustomer ?? false)))));
                if (inventoryMoveDetails.Count() > 0)
                {
                    var _itemsGroupsLotSaldo = inventoryMoveDetails
                        .GroupBy(r => new { r.id_item, r.id_lot, r.Lot })
                        .Select(e => new
                        {
                            e.Key.id_item,
                            e.Key.id_lot,
                            e.Key.Lot,
                            LotDetails = e.Select(x => x).ToList(),
                        })
                        .ToList();

                    foreach (var _itemsGroupLotSaldo in _itemsGroupsLotSaldo)
                    {
                        var saldo = _itemsGroupLotSaldo.LotDetails.Sum(e => e.entryAmount - e.exitAmountCost);
                        var numberP1 = _itemsGroupLotSaldo.Lot?.number ?? String.Empty;

                        var lot = _itemsGroupLotSaldo.Lot;
                        var productionLot = _itemsGroupLotSaldo.Lot.ProductionLot;
                        var internalNumber = lot.internalNumber ?? productionLot?.internalNumber ?? String.Empty;
                        var lotMarket = _itemsGroupLotSaldo.LotDetails.OrderByDescending(e => e.id_item).FirstOrDefault()?.lotMarked;

                        if (!String.IsNullOrEmpty(internalNumber))
                        {
                            numberP1 += " / " + internalNumber + ((lotMarkedPar == "SI") ? (" / " + lotMarket) : "");
                        }

                        itemsLotSaldo.Add(new ItemInvMoveDetailLot()
                        {
                            id_item = _itemsGroupLotSaldo.id_item,
                            id_lot = _itemsGroupLotSaldo.id_lot,
                            saldo = saldo,
                            number = numberP1
                        });

                    }
                    //var itemsLotSaldo = inventoryMoveDetails.GroupBy(r => new { r.id_item, r.id_lot, r.Lot })
                    //                                    .Select(s => new
                    //                                    {
                    //                                        id_item = s.Key.id_item,
                    //                                        id_lot = s.Key.id_lot,
                    //                                        saldo = s.Sum(g => g.entryAmount - g.exitAmount),
                    //                                        number = ((s.Key.Lot.number ?? "") + ((s.Key.Lot.number != "" &&
                    //                                                                               s.Key.Lot.number != null &&
                    //                                                                               (s.Key.Lot.internalNumber != "" ||
                    //                                                                               (s.Key.Lot.ProductionLot != null ? s.Key.Lot.ProductionLot.internalNumber : "") != "") &&
                    //                                                                               (s.Key.Lot.internalNumber != null ||
                    //                                                                               (s.Key.Lot.ProductionLot != null ? s.Key.Lot.ProductionLot.internalNumber : null) != null)) ? " / " : "")
                    //                                                                           + (s.Key.Lot.internalNumber ?? (s.Key.Lot.ProductionLot != null ? (s.Key.Lot.ProductionLot.internalNumber ?? "") : "")))//s.Lot.number
                    //                                    }).ToList();

                    itemsLotSaldo = itemsLotSaldo.Where(w => w.saldo > 0)/*.Distinct().Select(s => db.Item.FirstOrDefault(fod=> fod.id == s.id_item))*/.ToList();
                    items = db.Item.AsEnumerable().Where(w => itemsLotSaldo.FirstOrDefault(fod => fod.id_item == w.id) != null).ToList();
                    //items = itemsLotSaldo.Where(w => w.saldo > 0).Select(s => db.Item.FirstOrDefault(fod => fod.id == s.id_item)).ToList();

                    lots = itemsLotSaldo.Where(w => w.saldo > 0 /*&& w.id_lot != null*/)
                                       .Select(s => new Lot
                                       {
                                           id = s.id_lot ?? 0,
                                           number = s.number
                                       }).ToList();

                }
            }

            return lots;
        }


        public static IEnumerable LotsWithItemInventoryCodeDocumentTypeWarehouseWarehouseLocationNew(string codeDocumentType, int? id_warehouse, int? id_warehouseLocation, int? id_lot, bool? withLotSystem, bool? withLotCustomer)
        {
            db = new DBContext();
            List<Item> items = new List<Item>();
            List<Lot> lots = new List<Lot>();
            List<ItemInvMoveDetailLot> itemsLotSaldo = new List<ItemInvMoveDetailLot>();
            var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";
            if ((codeDocumentType == "05" || codeDocumentType == "32" || codeDocumentType == "129") && id_warehouse != null && id_warehouseLocation != null)//05: Egreso, 32: Egreso por transferencia y 129: Egreso Por Transferencia Automática
            {
                var inventoryMoveDetails = db.InventoryMoveDetail.Where(fod => fod.InventoryMove.Document.DocumentState.code == "03" && //"03":APROBADA
                                                                               fod.id_warehouse == id_warehouse &&
                                                                               fod.id_warehouseLocation == id_warehouseLocation &&
                                                                               ((fod.Item.ItemInventory.requiresLot == ((withLotSystem ?? false) || (withLotCustomer ?? false)))));
                var itemsLotSald2o = inventoryMoveDetails.ToList();
                if (inventoryMoveDetails.Count() > 0)
                {
                    var _itemsGroupsLotSaldo = inventoryMoveDetails
                        .GroupBy(r => new { r.id_item, r.id_lot, r.Lot })
                        .Select(e => new
                        {
                            e.Key.id_item,
                            e.Key.id_lot,
                            e.Key.Lot,
                            LotDetails = e.Select(x => x).ToList(),
                        })
                        .ToList();

                    foreach (var _itemsGroupLotSaldo in _itemsGroupsLotSaldo)
                    {
                        var saldo = _itemsGroupLotSaldo.LotDetails.Sum(e => e.entryAmount - e.exitAmountCost);
                        var numberP1 = _itemsGroupLotSaldo.Lot?.number ?? String.Empty;

                        var lot = _itemsGroupLotSaldo.Lot;
                        var productionLot = _itemsGroupLotSaldo.Lot.ProductionLot;
                        var internalNumber = lot.internalNumber ?? productionLot?.internalNumber ?? String.Empty;
                        var lotMarket = _itemsGroupLotSaldo.LotDetails.OrderByDescending(e => e.id_item).FirstOrDefault()?.lotMarked;

                        if (!String.IsNullOrEmpty(internalNumber))
                        {
                            numberP1 += " / " + internalNumber + ((lotMarkedPar == "SI") ? (" / " + lotMarket) : "");
                        }

                        itemsLotSaldo.Add(new ItemInvMoveDetailLot()
                        {
                            id_item = _itemsGroupLotSaldo.id_item,
                            id_lot = _itemsGroupLotSaldo.id_lot,
                            saldo = saldo,
                            number = numberP1
                        });

                    }
                    //var itemsLotSaldo = inventoryMoveDetails.GroupBy(r => new { r.id_item, r.id_lot, r.Lot })
                    //                                    .Select(s => new
                    //                                    {
                    //                                        id_item = s.Key.id_item,
                    //                                        id_lot = s.Key.id_lot,
                    //                                        saldo = s.Sum(g => g.entryAmount - g.exitAmount),
                    //                                        number = ((s.Key.Lot.number ?? "") + ((s.Key.Lot.number != "" &&
                    //                                                                               s.Key.Lot.number != null &&
                    //                                                                               (s.Key.Lot.internalNumber != "" ||
                    //                                                                               (s.Key.Lot.ProductionLot != null ? s.Key.Lot.ProductionLot.internalNumber : "") != "") &&
                    //                                                                               (s.Key.Lot.internalNumber != null ||
                    //                                                                               (s.Key.Lot.ProductionLot != null ? s.Key.Lot.ProductionLot.internalNumber : null) != null)) ? " / " : "")
                    //                                                                           + (s.Key.Lot.internalNumber ?? (s.Key.Lot.ProductionLot != null ? (s.Key.Lot.ProductionLot.internalNumber ?? "") : "")))//s.Lot.number
                    //                                    }).ToList();
                    var _item = itemsLotSaldo.Where(a => a.id_item == 11028);

                    itemsLotSaldo = itemsLotSaldo.Where(w => w.saldo > 0)/*.Distinct().Select(s => db.Item.FirstOrDefault(fod=> fod.id == s.id_item))*/.ToList();
                    items = db.Item.AsEnumerable().Where(w => itemsLotSaldo.FirstOrDefault(fod => fod.id_item == w.id) != null).ToList();
                    //items = itemsLotSaldo.Where(w => w.saldo > 0).Select(s => db.Item.FirstOrDefault(fod => fod.id == s.id_item)).ToList();

                    lots = itemsLotSaldo.Where(w => w.saldo > 0 /*&& w.id_lot != null*/)
                                       .Select(s => new Lot
                                       {
                                           id = s.id_lot ?? 0,
                                           number = s.number
                                       }).ToList();

                }
            }

            return lots;
        }

        public static string ReferenceLotCost(int? id_lot)
        {
            string ReferenceLotCost = "";
            db = new DBContext();
            var id_inventaryMoveAutomaticRawMaterialEntry = db.DocumentSource
                                                                    .OrderByDescending(r=> r.Document.dateCreate)
                                                                    .FirstOrDefault(fod =>  fod.id_documentOrigin == id_lot &&
                                                                                            fod.Document.DocumentType.code.Equals("137"))?.id_document;//137: Ingreso Materia Prima Automático

            var inventoryMove = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveAutomaticRawMaterialEntry);
            ReferenceLotCost = inventoryMove != null ? ("Bodega: " + inventoryMove.InventoryMoveDetail.FirstOrDefault().Warehouse.name +
                                                        " - Ubicación: " + inventoryMove.InventoryMoveDetail.FirstOrDefault().WarehouseLocation.name +
                                                        " - " + inventoryMove.natureSequential +
                                                        " - Fecha Registro: " + inventoryMove.Document.emissionDate.ToString("yyyy-MM-dd")) : "";
            return ReferenceLotCost;
        }

        public static IEnumerable ProductionLotManual(int? id)
        {
            db = new DBContext();

            var retorno = db.ProductionLot.Where(g => (g.ProductionProcess.code == "RMM" && g.ProductionLotState.code == "08"))
                .Select(e => new {
                    id = e.id,
                    internalNumber = e.number + " - " + e.internalNumber,
                })
                .ToList();


            if (id.HasValue)
            {
                var productionLot = db.ProductionLot.FirstOrDefault(t => t.id == id);

                if (productionLot != null)
                {
                    retorno.Add(new
                    {
                        id = productionLot.id,
                        internalNumber = productionLot.number + " - " + productionLot.internalNumber
                    });
                }
            }

            return retorno;
        }
    }
}