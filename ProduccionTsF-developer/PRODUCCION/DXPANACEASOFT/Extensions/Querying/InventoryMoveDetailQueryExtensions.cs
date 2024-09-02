using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class InventoryMoveDetailQueryExtensions
    {
		public static Expression<Func<InventoryMoveDetail, bool>> GetRequestByFilter(int? id_boxedWarehouse, int? id_boxedWarehouseLocation)
		{
			var db = new DBContext();

            Expression<Func<InventoryMoveDetail, bool>> expression = s =>
               (id_boxedWarehouse == s.id_warehouse) &&
               (id_boxedWarehouseLocation == s.id_warehouseLocation) &&
               (s.InventoryMove.Document.DocumentState.code == "03") &&
               (s.Item.InventoryLine.code == "PP" || s.Item.InventoryLine.code == "PT");
			return expression;
		}
	}
}