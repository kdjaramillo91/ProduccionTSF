using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;
using DXPANACEASOFT.Models.InventoryBalance;
using DXPANACEASOFT.Models.InventoryMoveDTO;
using DXPANACEASOFT.Models.InventoryMoveP.InventoryMoveModel;
using DXPANACEASOFT.Services;
using static DXPANACEASOFT.Controllers.InventoryMoveController;
using static DXPANACEASOFT.Services.ServiceInventoryBalance;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderInventoryMove
	{
		private static DBContext db = null;

		public static IEnumerable InventoryMoves()
		{
			db = new DBContext();
			return db.InventoryMove.ToList();
		}

		public static InventoryMove InventoryMove(int? id_inventoryMove)
		{
			db = new DBContext();
			return db.InventoryMove.FirstOrDefault(i => i.id == id_inventoryMove);
		}

		public static InventoryMoveDetail InventoryMoveDetail(int? id_inventoryMoveDetail)
		{
			db = new DBContext();
			return db.InventoryMoveDetail.FirstOrDefault(i => i.id == id_inventoryMoveDetail);
		}

		public static IEnumerable InventoryReasonByCompany(int? id_company)
		{
			db = new DBContext();
			var inventoryReasons = db.InventoryReason.Where(i => i.id_company == id_company && i.isActive);

			return inventoryReasons.ToList();
		}

		public static InventoryMoveDetail InventoryMoveDetailById(int? id_inventoryMoveDetail)
		{
			db = new DBContext();
			return db.InventoryMoveDetail.FirstOrDefault(i => i.id == id_inventoryMoveDetail);
		}

		public static List<InventoryReason> InventoryReasonsByCompanyCodeDocumentTypeAndCurrent(int? id_company, string codeDocumentType, int? id_current, bool? isManual = null, bool? isSystem = null)
		{
			db = new DBContext();
			return db.InventoryReason.Where(g => (g.isActive && g.id_company == id_company && g.DocumentType.code.Equals(codeDocumentType) &&
												 (g.isAuthomatic ?? false) == !(isManual ?? !(g.isAuthomatic ?? false)) && //!(g.isForTransfer ?? false) &&
												  g.isSystem == (isSystem ?? g.isSystem)) ||
												  g.id == (id_current == null ? 0 : id_current)).ToList();
		}
        public static List<InventoryReason> InventoryReasonsByCompanyCodeDocumentAndCurrent(int? id_company, string naturaleza, int? id_current, bool? isManual = null, bool? isSystem = null)
        {
            db = new DBContext();
            return db.InventoryReason.Where(g => (g.isActive && g.id_company == id_company && g.AdvanceParametersDetail.valueCode == naturaleza &&
                                                 (g.isForTransfer == true) && g.requiereUserLotNubmber == true &&
                                                  g.isAuthomatic == false) ||
                                                  g.id == (id_current == null ? 0 : id_current)).ToList();
        }
        public static IEnumerable InventoryReasonsByCompanyNatureMove(int? id_company, string natureMoveType, bool? isManual = null, bool? isSystem = null)
		{
			db = new DBContext();
			var inventoryReasons = db.InventoryReason.Where(g => (g.isActive && g.id_company == id_company && g.AdvanceParametersDetail.valueCode.Trim().Equals(natureMoveType)
												 && (g.isAuthomatic ?? false) == !(isManual ?? !(g.isAuthomatic ?? false)) && g.isSystem == (isSystem ?? g.isSystem))).ToList();
			return inventoryReasons;

		}

		public static List<InventoryReason> InventoryReasonsByCompanyNatureMoveWithoutTransfer(int? id_company, string natureMoveType)
		{
			db = new DBContext();
			List<InventoryReason> inventoryReasons = new List<InventoryReason>();
			if (natureMoveType == "I")
			{
				inventoryReasons = db.InventoryReason.Where(g => (g.isActive && g.id_company == id_company && g.AdvanceParametersDetail.valueCode.Trim().Equals(natureMoveType)) && !(g.isForTransfer == null ? false : g.isForTransfer.Value) &&
																  //inventoryReasons = db.InventoryReason.Where(g => (g.isActive && g.id_company == id_company && g.AdvanceParametersDetail.valueCode.Trim().Equals(natureMoveType)) && !(g.isForTransfer) &&

																  g.DocumentType.code != "04" && g.DocumentType.code != "34" && g.DocumentType.code != "130" && g.DocumentType.code != "136" && g.DocumentType.code != "143").ToList();
			}
			else
			{
				if (natureMoveType == "E")
				{
					inventoryReasons = db.InventoryReason.Where(g => (g.isActive && g.id_company == id_company && g.AdvanceParametersDetail.valueCode.Trim().Equals(natureMoveType)) && !(g.isForTransfer == null ? false : g.isForTransfer.Value) &&
																	  //inventoryReasons = db.InventoryReason.Where(g => (g.isActive && g.id_company == id_company && g.AdvanceParametersDetail.valueCode.Trim().Equals(natureMoveType)) && !(g.isForTransfer) &&
																	  g.DocumentType.code != "32" && g.DocumentType.code != "129" && g.DocumentType.code != "135" && g.DocumentType.code != "142").ToList();
				}
			}

			return inventoryReasons;

		}

		public static List<InventoryReason> InventoryReasonsByCompanyNatureMoveOnlyTransfer(int? id_company, string natureMoveType)
		{
			db = new DBContext();
			List<InventoryReason> inventoryReasons = new List<InventoryReason>();
			if (natureMoveType == "I")
			{
				try
				{
					inventoryReasons = db.InventoryReason.Where(g => (g.isActive && g.id_company == id_company && g.AdvanceParametersDetail.valueCode.Trim().Equals(natureMoveType)) &&
																  ((g.isForTransfer == null ? false : g.isForTransfer.Value) || g.DocumentType.code == "34" || g.DocumentType.code == "130" || g.DocumentType.code == "136" || g.DocumentType.code == "143")).ToList();
				}
				catch (Exception e)
				{
					var s = e;

				}

				//((g.isForTransfer) || g.DocumentType.code == "34" || g.DocumentType.code == "130" || g.DocumentType.code == "136" || g.DocumentType.code == "143")).ToList();
			}
			else
			{
				if (natureMoveType == "E")
				{
					inventoryReasons = db.InventoryReason.Where(g => (g.isActive && g.id_company == id_company && g.AdvanceParametersDetail.valueCode.Trim().Equals(natureMoveType)) &&
																	  ((g.isForTransfer == null ? false : g.isForTransfer.Value) || g.DocumentType.code == "32" || g.DocumentType.code == "129" || g.DocumentType.code == "135" || g.DocumentType.code == "142")).ToList();
					//((g.isForTransfer) || g.DocumentType.code == "32" || g.DocumentType.code == "129" || g.DocumentType.code == "135" || g.DocumentType.code == "142")).ToList();
				}
			}

			return inventoryReasons;

		}

		public static IEnumerable InventoryReasonEntry(int? idInventoryReason = 0, bool isManual = false, bool isSystem = false)
		{
			db = new DBContext();
			var lstInvMove = db.InventoryReason.Where(g => g.AdvanceParametersDetail.valueCode.Trim().Equals("I") && (g.isAuthomatic ?? false) == !isManual && g.isSystem == isSystem).ToList();

			//if (isManual) lstInvMove = lstInvMove.Where(w => w.isAuthomatic == false).ToList();
			var invReasonTmp = db.InventoryReason.FirstOrDefault(fod => fod.id == idInventoryReason);

			if (invReasonTmp != null && !lstInvMove.Contains(invReasonTmp)) lstInvMove.Add(invReasonTmp);

			return lstInvMove;
		}

		public static IEnumerable InventoryReasonExit(int? idInventoryReason = 0, bool isManual = false, bool isSystem = false)
		{
			db = new DBContext();
			//var lstInvMove = db.InventoryReason.Where(g => g.AdvanceParametersDetail.valueCode.Trim().Equals("E")).ToList();
			var lstInvMove = db.InventoryReason.Where(g => g.AdvanceParametersDetail.valueCode.Trim().Equals("E") && (g.isAuthomatic ?? false) == !isManual && g.isSystem == isSystem).ToList();

			//if (isManual) lstInvMove = lstInvMove.Where(w => w.isAuthomatic == false).ToList(); 
			var invReasonTmp = db.InventoryReason.FirstOrDefault(fod => fod.id == idInventoryReason);

			if (invReasonTmp != null && !lstInvMove.Contains(invReasonTmp)) lstInvMove.Add(invReasonTmp);

			return lstInvMove;
		}



		public static IEnumerable InventoryReasonAutoExit(int? id_company, string codeDocumentType, int? id_current)
		{
			db = new DBContext();
			List<InventoryReason> inventoryReasons = new List<InventoryReason>();

			inventoryReasons = db.InventoryReason.Where(g => g.isActive && g.id_company == id_company && g.DocumentType.code.Equals("32")).ToList();


			//((g.isForTransfer) || g.DocumentType.code == "32" || g.DocumentType.code == "129" || g.DocumentType.code == "135" || g.DocumentType.code == "142")).ToList();

			return inventoryReasons;
		}



		public static IEnumerable InventoryReasonAutoEntry(int? id_company, string codeDocumentType, int? id_current)
		{
			db = new DBContext();
			List<InventoryReason> inventoryReasons = new List<InventoryReason>();

			inventoryReasons = db.InventoryReason.Where(g => g.isActive && g.id_company == id_company && g.DocumentType.code.Equals("34")).ToList();
			//((g.isForTransfer) || g.DocumentType.code == "32" || g.DocumentType.code == "129" || g.DocumentType.code == "135" || g.DocumentType.code == "142")).ToList();


			return inventoryReasons;
		}

		public static InventoryReason InventoryReasonByCode(string codeInventoryReason)
		{
			db = new DBContext();
			return db.InventoryReason.FirstOrDefault(g => g.code == codeInventoryReason && g.isActive);
		}


		public static decimal GetRemainingBalance(	int activeCompanyId,
													int? id_item, 
													int? id_warehouse, 
													int? id_warehouseLocation, 
													int? id_lot, 
													string lotMarked,
													DateTime? fechaEmision = null)
		{
			db = new DBContext();
            var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";
            lotMarked = lotMarked != null ? lotMarked : null;
			id_lot = id_lot.HasValue && id_lot > 0 ? id_lot : null;

			var requiresLot = db.Item
				.FirstOrDefault(e => e.id == id_item)?
				.ItemInventory?
				.requiresLot ?? false;

            var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral( new InvParameterBalanceGeneral
            {
                requiresLot = requiresLot,
                id_Warehouse = id_warehouse,
                id_WarehouseLocation = id_warehouseLocation,
                id_Item = id_item,
                id_ProductionLot = id_lot,
                lotMarket = lotMarked,
                cut_Date = fechaEmision,
                id_productionCart = null,
                id_company = activeCompanyId,
                consolidado = true,
                groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE
            }, modelSaldoProductlote: true);
            var saldos = resultItemsLotSaldo.Item2;

            //var saldos = new Services.ServiceInventoryMove()
			//	.GetSaldosProductoLote(requiresLot, id_warehouse, id_warehouseLocation, id_item, id_lot, lotMarked, fechaEmision);

			return saldos.Sum(e => e.saldo);
		}

		public static InventoryMove InventoryMoveByIdInventoryMoveDetail(int? id_inventoryMoveDetail)
		{
			db = new DBContext();
			return db.InventoryMoveDetail.FirstOrDefault(i => i.id == id_inventoryMoveDetail)?.InventoryMove;
		}

		public static string InventoryMoveDetailRemissionGuide(int id_inventoryMoveDetail)
		{
			string _answer = "";
			int _idInventoryMove = 0;

			_idInventoryMove = db.InventoryMoveDetail.FirstOrDefault(fod => fod.id == id_inventoryMoveDetail)?.InventoryMove?.id ?? 0;

			if (_idInventoryMove > 0)
			{
				var _lstDocSource = db.DocumentSource.Where(fod => fod.id_document == _idInventoryMove).Select(s => s.id_documentOrigin).ToList();

				if (_lstDocSource != null && _lstDocSource.Count > 0)
				{
					_answer = db.Document.FirstOrDefault(fod => _lstDocSource.Contains(fod.id) && (fod.DocumentType.code == "08" || fod.DocumentType.code == "69"))?.sequential.ToString() ?? "";
				}
			}

			return _answer;
		}
		public static InventoryMoveModelP FindIfInventoryMoveToReverse(int idInventoryMoveToReverse)
		{
			db = new DBContext();
			return db.InventoryMove
						.Where(w => w.id_inventoryMoveToReverse == idInventoryMoveToReverse)
						.Select(s => new InventoryMoveModelP
						{
							idInventoryMoveModelP = s.id,
							idInventoryReasonModelP = s.id_inventoryReason,
							idProductionLot = s.id_productionLot,
							idInventoryMoveToReverse = s.id_inventoryMoveToReverse

						})
						.FirstOrDefault();
		}

		public static IEnumerable GetInventoryMoveReason()
		{
			db = new DBContext();
			return db.InventoryReason
						.Select(s => new InventoryReasonModelP
						{
							idInventoryReasonModelP = s.id,
							codeInventoryReasonModelP = s.code,
							nameInventoryReasonModelP = s.name,
							descriptionInventoryReasonModelP = s.description,
							id_documentTypeInventoryReasonModelP = s.id_documentType,
							idNatureMoveInventoryReasonModelP = s.idNatureMove
						}).ToList();
		}

		public static IEnumerable GetInventoryMoves()
		{
			db = new DBContext();
			return db.InventoryMove
						.Select(s => new InventoryMoveModelP
						{
							idInventoryMoveModelP = s.id,
							idInventoryReasonModelP = s.id_inventoryReason,
							idProductionLot = s.id_productionLot,
							idInventoryMoveToReverse = s.id_inventoryMoveToReverse,
							natureSequential = s.natureSequential,
							iSequential = s.sequential ?? 0
						}).ToList();
		}

		public static IEnumerable GetInventoryMoveToReverse()
		{
			db = new DBContext();
			return db.InventoryMove
						.Where(w => w.id_inventoryMoveToReverse != null)
						.Select(s => (int)s.id_inventoryMoveToReverse
						).ToList();
		}
		public static IEnumerable GetInventoryMoveDetail()
		{
			db = new DBContext();
			return db.InventoryMoveDetail
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
							natureSequential = s.natureSequential,
							iSequential = s.InventoryMove.sequential ?? 0,
							idNatureMove = s.InventoryMove.idNatureMove
						}).ToList();
		}

		public static IEnumerable GetNatureMove()
		{
			db = new DBContext();
			return db.AdvanceParametersDetail
					.Where(w => w.AdvanceParameters.code.Equals("NMMGI"))
					.Select(s => new
					{
						id = s.id,
						name = s.description
					}).ToList();
		}

		public static IEnumerable GetInventoryReasons(int? idNatureMove)
		{
			db = new DBContext();
			return db.InventoryReason.Where(w => w.idNatureMove == idNatureMove).ToList();
		}

        public static IEnumerable LotsWithItemInventoryCodeDocumentTypeWarehouseWarehouseLocationAndCurrent(	int activeCompanyId, 
																												int? id_itemCurrent,
																												string codeDocumentType, 
																												int? id_warehouse, 
																												int? id_warehouseLocation, 
																												int? id_lot, 
																												bool? withLotSystem,
																												bool? withLotCustomer, 
																												DateTime? fechaEmision = null)
        {
            List<Item> items = new List<Item>();
            var lots = new List<CustomLot>();
            List<ItemInvMoveDetailLot> itemsLotSaldo = new List<ItemInvMoveDetailLot>();
            var lotMarkedPar = db.Setting.FirstOrDefault(fod => fod.code == "LMMASTER")?.value ?? "NO";
            if ((codeDocumentType == "05" || codeDocumentType == "32" || codeDocumentType == "129") && id_warehouse != null && id_warehouseLocation != null)
            {
                var requiresLot = ((withLotSystem ?? false) || (withLotCustomer ?? false));

                var resultItemsLotSaldo = ServiceInventoryBalance.ValidateBalanceGeneral(new InvParameterBalanceGeneral
                {
                    requiresLot = requiresLot,
                    id_Warehouse = id_warehouse,
                    id_WarehouseLocation = id_warehouseLocation,
                    id_Item = id_itemCurrent,
                    id_ProductionLot = id_lot,
                    lotMarket = null,
                    cut_Date = fechaEmision,
                    id_productionCart = null,
                    id_company = activeCompanyId,
                    consolidado = false,
                    groupby = ServiceInventoryGroupBy.GROUPBY_ITEM_LOTE
                }, modelSaldoProductlote: true);
                var saldos = resultItemsLotSaldo.Item2;

                //var saldos = new Services.ServiceInventoryMove()
                //    .GetSaldosProductoLote(requiresLot, id_warehouse, id_warehouseLocation, id_itemCurrent, id_lot, null, fechaEmision);

                var idsItem = saldos.Select(e => e.id_item).Distinct();
                items = db.Item.Where(w => idsItem.Contains(w.id)).ToList();

                var usarLoteMarcado = lotMarkedPar == "SI";
                lots = saldos
                    .Select(e => new CustomLot()
                    {
                        id = e.id_lote ?? 0,
                        number = e.number +
                            (!String.IsNullOrEmpty(e.internalNumber) ? $" / {e.internalNumber}" : string.Empty) +
                            (usarLoteMarcado && !String.IsNullOrEmpty(e.lot_market) ? $"" : string.Empty),
                        Saldo = e.saldo
                    })
                    .ToList();
            }

            return lots;
        }
    }
}