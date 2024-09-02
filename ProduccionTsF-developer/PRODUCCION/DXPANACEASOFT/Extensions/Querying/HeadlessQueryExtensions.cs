using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class HeadlessQueryExtensions
	{
		public static Expression<Func<Headless, bool>> GetRequestByFilter(HeadlessConsultDTO consultar)
		{
			var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
			var fechaFinEmision = string.IsNullOrEmpty(consultar.endtDate) ? (DateTime?)null : DateTime.Parse(consultar.endtDate);

            Expression<Func<Headless, bool>> expression = s =>
               ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
                (fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
               ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
               ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
               ((consultar.id_turn == null || consultar.id_turn == 0) || (consultar.id_turn == s.id_turn)) &&
               ((consultar.numberLot == null || consultar.numberLot == "") || s.ProductionLot.internalNumber.Contains(consultar.numberLot)) &&
               ((consultar.secTransaction == null || consultar.secTransaction == "") || s.ProductionLot.number.Contains(consultar.secTransaction)) &&
               ((consultar.numberLotOrigin == null || consultar.numberLotOrigin == "") || s.ProductionLot.ProductionLotDetail.FirstOrDefault(fod=> fod.ProductionLot1 != null && fod.ProductionLot1.internalNumber.Contains(consultar.numberLotOrigin)) != null);

            return expression;
		}
	}
}