using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class OpeningClosingPlateLyingQueryExtensions
	{
		public static Expression<Func<OpeningClosingPlateLying, bool>> GetRequestByFilter(OpeningClosingPlateLyingConsultDTO consultar)
		{
			var db = new DBContext();
			var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
			var fechaFinEmision = string.IsNullOrEmpty(consultar.endtDate) ? (DateTime?)null : DateTime.Parse(consultar.endtDate);

			Expression<Func<OpeningClosingPlateLying, bool>> expression = s =>
			   ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
				(fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
			   ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
			   ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
			   ((consultar.reference == null || consultar.reference == "") || (s.Document.description.Contains(consultar.reference))) &&
			   ((consultar.id_responsable == null || consultar.id_responsable == 0) || (consultar.id_responsable == s.id_responsable)) &&
			   ((consultar.id_freezerWarehouse == null || consultar.id_freezerWarehouse == 0) || (consultar.id_freezerWarehouse == s.id_freezerWarehouse));
			return expression;
		}
	}
}