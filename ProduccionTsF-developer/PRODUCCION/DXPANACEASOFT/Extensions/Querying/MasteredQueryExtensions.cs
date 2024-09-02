using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class MasteredQueryExtensions
	{
		public static Expression<Func<Mastered, bool>> GetRequestByFilter(MasteredConsultDTO consultar)
		{
			var db = new DBContext();
			var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
			var fechaFinEmision = string.IsNullOrEmpty(consultar.endtDate) ? (DateTime?)null : DateTime.Parse(consultar.endtDate);

            Expression<Func<Mastered, bool>> expression = s =>
               ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
                (fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
               ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
               ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
               ((consultar.id_responsable == null || consultar.id_responsable == 0) || (consultar.id_responsable == s.id_responsable)) &&
               ((consultar.id_turn == null || consultar.id_turn == 0) || (consultar.id_turn == s.id_turn)) &&
               ((consultar.id_boxedWarehouse == null || consultar.id_boxedWarehouse == 0) || (consultar.id_boxedWarehouse == s.id_boxedWarehouse)) &&
               ((consultar.id_boxedWarehouseLocation == null || consultar.id_boxedWarehouseLocation == 0) || (consultar.id_boxedWarehouseLocation == s.id_boxedWarehouseLocation)) &&
               ((consultar.id_masteredWarehouse == null || consultar.id_masteredWarehouse == 0) || (consultar.id_masteredWarehouse == s.id_masteredWarehouse)) &&
               ((consultar.id_masteredWarehouseLocation == null || consultar.id_masteredWarehouseLocation == 0) || (consultar.id_masteredWarehouseLocation == s.id_masteredWarehouseLocation));
               //((consultar.id_warehouseBoxes == null || consultar.id_warehouseBoxes == 0) || (consultar.id_warehouseBoxes == s.id_warehouseBoxes)) &&
               //((consultar.id_warehouseLocationBoxes == null || consultar.id_warehouseLocationBoxes == 0) || (consultar.id_warehouseLocationBoxes == s.id_warehouseLocationBoxes));
			return expression;
		}
	}
}