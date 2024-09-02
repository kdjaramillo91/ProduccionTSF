using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class InventoryMovePlantTransferQueryExtensions
	{
		public static Expression<Func<InventoryMovePlantTransfer, bool>> GetRequestByFilter(InventoryMovePlantTransferConsultDTO consultar)
		{
			var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
			var fechaFinEmision = string.IsNullOrEmpty(consultar.endtDate) ? (DateTime?)null : DateTime.Parse(consultar.endtDate);

			Expression<Func<InventoryMovePlantTransfer, bool>> expression = s =>
			   ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.InventoryMove.Document.emissionDate)) &&
				(fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.InventoryMove.Document.emissionDate))) &&
			   ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.InventoryMove.Document.id_documentState)) &&
			   //((consultar.number == null || consultar.number == "") || (s.InventoryMove.Document.number.Contains(consultar.number))) &&
			   ((consultar.reference == null || consultar.reference == "") || (s.InventoryMove.Document.reference.Contains(consultar.reference))) &&
			   ((consultar.id_warehouseEntry == null || consultar.id_warehouseEntry == 0) || (s.InventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_warehouseEntry == consultar.id_warehouseEntry) != null)) &&
			   ((consultar.id_warehouseLocationEntry == null || consultar.id_warehouseLocationEntry == 0) || (s.InventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.id_warehouseLocationEntry == consultar.id_warehouseLocationEntry) != null)) &&
			   //((consultar.id_inventoryReason == null || consultar.id_inventoryReason == 0) || (consultar.id_inventoryReason == s.InventoryMove.id_inventoryReason)) &&
			   ((consultar.id_receiver == null || consultar.id_receiver == 0) || (consultar.id_receiver == s.InventoryMove.InventoryEntryMove.id_receiver)) &&
			   ((consultar.numberLot == null || consultar.numberLot == "") || s.InventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.Lot.internalNumber.Contains(consultar.numberLot) ||
																																		(fod.Lot.ProductionLot != null && fod.Lot.ProductionLot.internalNumber.Contains(consultar.numberLot))) != null) &&
			   ((consultar.secTransaction == null || consultar.secTransaction == "") || s.InventoryMove.InventoryMoveDetail.FirstOrDefault(fod => fod.Lot.number.Contains(consultar.secTransaction)) != null) &&
			   ((consultar.id_processType == null || consultar.id_processType == 0) || (s.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.LiquidationCartOnCartDetail.LiquidationCartOnCart.idProccesType == consultar.id_processType) != null)) &&
			   ((consultar.id_provider == null || consultar.id_provider == 0) || (s.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.LiquidationCartOnCartDetail.LiquidationCartOnCart.ProductionLot.id_provider == consultar.id_provider) != null)) &&
			   ((consultar.id_machineForProd == null || consultar.id_machineForProd == 0) /*|| (consultar.id_machineForProd == s.MachineProdOpeningDetail.id_MachineForProd)*/ || (s.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.LiquidationCartOnCartDetail.LiquidationCartOnCart.id_MachineForProd == consultar.id_machineForProd) != null)) &&
			   ((consultar.id_turn == null || consultar.id_turn == 0) || (consultar.id_turn == s.MachineProdOpeningDetail.MachineProdOpening.id_Turn)/*(s.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.LiquidationCartOnCartDetail.LiquidationCartOnCart.MachineProdOpening.id_Turn == consultar.id_turn) != null)*/) &&
			   ((consultar.id_productionCart == null || consultar.id_productionCart == 0) || (s.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.LiquidationCartOnCartDetail.id_ProductionCart == consultar.id_productionCart) != null));

			return expression;
		}
	}
}