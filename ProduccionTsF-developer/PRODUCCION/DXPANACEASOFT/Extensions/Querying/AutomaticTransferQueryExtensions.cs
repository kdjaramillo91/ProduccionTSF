using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class AutomaticTransferQueryExtensions
	{
		
		public static Expression<Func<AutomaticTransfer, bool>> GetRequestByFilter(AutomaticTransferFilterDTO consultar)
		{
			var fechaInicioEmision = string.IsNullOrEmpty(consultar.dateEmissionFrom) ? (DateTime?)null : DateTime.Parse(consultar.dateEmissionFrom);
			var fechaFinEmision = string.IsNullOrEmpty(consultar.dateEmissionTo) ? (DateTime?)null : DateTime.Parse(consultar.dateEmissionTo);

			Expression<Func<AutomaticTransfer, bool>> expression = s =>
			   ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
				(fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&

			   ((consultar.id_StateDocument == null || consultar.id_StateDocument == 0) || (consultar.id_StateDocument == s.Document.id_documentState)) &&
			   ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
			   ((consultar.reference == null || consultar.reference == "") || s.Document.reference.Contains(consultar.reference)) &&
			   ((consultar.id_InventoryReasonExit == null || consultar.id_InventoryReasonExit == 0) || s.id_InventoryReasonExit == consultar.id_InventoryReasonExit) &&
			   ((consultar.id_WarehouseExit == null || consultar.id_WarehouseExit == 0) || s.id_WarehouseExit == consultar.id_WarehouseExit) &&
			   ((consultar.id_WarehouseLocationExit == null || consultar.id_WarehouseLocationExit == 0) || s.id_WarehouseLocationExit == consultar.id_WarehouseLocationExit) &&
			   ((consultar.id_InventoryReasonEntry == null || consultar.id_InventoryReasonEntry == 0) || s.id_InventoryReasonEntry == consultar.id_InventoryReasonEntry) &&
			   ((consultar.id_WarehouseEntry == null || consultar.id_WarehouseEntry == 0) || s.id_WarehouseEntry == consultar.id_WarehouseEntry) &&
			   ((consultar.id_WarehouseLocationEntry == null || consultar.id_WarehouseLocationEntry == 0) || s.id_WarehouseLocationEntry == consultar.id_WarehouseLocationEntry) &&
			   ((consultar.id_Dispatcher == null || consultar.id_Dispatcher == 0) || s.id_Despachador == consultar.id_Dispatcher) &&

			   ((consultar.id_Item == null || consultar.id_Item == 0) || s.AutomaticTransferDetail.FirstOrDefault(fod => fod.id_Item == consultar.id_Item) != null) ;

			return expression;
		}
	}
}