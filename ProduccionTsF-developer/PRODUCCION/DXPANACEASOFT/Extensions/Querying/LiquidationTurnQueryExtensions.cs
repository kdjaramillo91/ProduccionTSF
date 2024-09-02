using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class LiquidationTurnQueryExtensions
	{
		public static Expression<Func<LiquidationTurn, bool>> GetRequestByFilter(LiquidationTurnConsultDTO consultar)
		{
			var db = new DBContext();
			var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
			var fechaFinEmision = string.IsNullOrEmpty(consultar.endtDate) ? (DateTime?)null : DateTime.Parse(consultar.endtDate);

			Expression<Func<LiquidationTurn, bool>> expression = s =>
			   ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
				(fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
			   ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
			   ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
			   ((consultar.id_turn == null || consultar.id_turn == 0) || (consultar.id_turn == s.id_turn));
			//((consultar.numberLot == null || consultar.numberLot == "") || (db.LiquidationCartOnCart.FirstOrDefault(fod => fod.MachineForProd.id_personProcessPlant == s.id_personProcessPlant &&
			//                                                                                                               fod.MachineProdOpening.id_Turn == s.id_turn &&
			//                                                                                                               DbFunctions.TruncateTime(s.emissionDate) == DbFunctions.TruncateTime(fod.MachineProdOpening.Document.emissionDate) &&
			//                                                                                                               fod.Document.DocumentState.code != "05" &&
			//                                                                                                               fod.ProductionLot.internalNumber.Contains(consultar.numberLot))) != null) &&
			//((consultar.id_provider == null || consultar.id_provider == 0) || (db.LiquidationCartOnCart.FirstOrDefault(fod => fod.MachineForProd.id_personProcessPlant == s.id_personProcessPlant &&
			//                                                                                                                  fod.MachineProdOpening.id_Turn == s.id_turn &&
			//                                                                                                                  DbFunctions.TruncateTime(s.emissionDate) == DbFunctions.TruncateTime(fod.MachineProdOpening.Document.emissionDate) &&
			//                                                                                                                  fod.Document.DocumentState.code != "05" &&
			//                                                                                                                  fod.ProductionLot.id_provider == consultar.id_provider)) != null) &&
			//((consultar.id_productionUnitProvider == null || consultar.id_productionUnitProvider == 0) || (db.LiquidationCartOnCart.FirstOrDefault(fod => fod.MachineForProd.id_personProcessPlant == s.id_personProcessPlant &&
			//                                                                                                               fod.MachineProdOpening.id_Turn == s.id_turn &&
			//                                                                                                               DbFunctions.TruncateTime(s.emissionDate) == DbFunctions.TruncateTime(fod.MachineProdOpening.Document.emissionDate) &&
			//                                                                                                               fod.Document.DocumentState.code != "05" &&
			//                                                                                                               fod.ProductionLot.id_productionUnitProvider == consultar.id_productionUnitProvider)) != null);


			return expression;
		}
	}
}