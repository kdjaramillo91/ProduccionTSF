using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class CoefficientProductionQueryExtensions
	{
		public static Expression<Func<ProductionCoefficient, bool>> GetRequestByFilter(CoefficientProductionDTO consultar)
		{
			var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
			var fechaFinEmision = string.IsNullOrEmpty(consultar.endDate) ? (DateTime?)null : DateTime.Parse(consultar.endDate);

			Expression<Func<ProductionCoefficient, bool>> expression = s =>
			   ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
				(fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
			   ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
			   ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
			   ((consultar.reference == null || consultar.reference == "") || s.Document.reference.Contains(consultar.reference)) &&
			   ((consultar.year == null || consultar.year == 0) || s.year == consultar.year) &&
			   ((consultar.month == null || consultar.month == 0) || s.year == consultar.month) &&
			   ((consultar.id_assignmentType == null || consultar.id_assignmentType == "") || s.assignmentType == consultar.id_assignmentType);
			
			return expression;
		}
	}
}