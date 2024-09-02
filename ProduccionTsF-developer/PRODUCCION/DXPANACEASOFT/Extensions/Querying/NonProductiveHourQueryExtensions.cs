using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class NonProductiveHourQueryExtensions
    {
		public static Expression<Func<NonProductiveHour, bool>> GetRequestByFilter(NonProductiveHourConsultDTO consultar)
		{
			var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
			var fechaFinEmision = string.IsNullOrEmpty(consultar.endtDate) ? (DateTime?)null : DateTime.Parse(consultar.endtDate);

			Expression<Func<NonProductiveHour, bool>> expression = s =>
			   ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
				(fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
			   ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
			   ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
               ((consultar.id_turn == null || consultar.id_turn == 0) || (consultar.id_turn == s.id_turn)) &&
			   ((consultar.id_machineForProd == null || consultar.id_machineForProd == 0) || (consultar.id_machineForProd == s.id_machineForProd));


            return expression;
		}
	}
}