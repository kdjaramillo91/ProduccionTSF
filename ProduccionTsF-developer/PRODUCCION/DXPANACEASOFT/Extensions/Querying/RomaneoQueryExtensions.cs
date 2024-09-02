using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class RomaneoQueryExtensions
	{
		public static Expression<Func<Romaneo, bool>> GetRequestByFilter(RomaneoConsultDTO consultar)
		{
			var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
			var fechaFinEmision = string.IsNullOrEmpty(consultar.endtDate) ? (DateTime?)null : DateTime.Parse(consultar.endtDate);

			Expression<Func<Romaneo, bool>> expression = s =>
			   ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
				(fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
			   ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
			   ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
			   ((consultar.reference == null || consultar.reference == "") || (s.Document.reference.Contains(consultar.reference))) &&
			   ((consultar.id_typeRomaneo == null || consultar.id_typeRomaneo == 0) || (consultar.id_typeRomaneo == s.idRomaneoType)) &&
			   ((consultar.numberLote == null || consultar.numberLote == "") || (s.ResultProdLotRomaneo != null
																					 ? (s.ResultProdLotRomaneo.numberLot.Contains(consultar.numberLote))
																					 : (consultar.numberLote == null || consultar.numberLote == ""))) &&
			   ((consultar.secTransaccional == null || consultar.secTransaccional == "") || (s.ResultProdLotRomaneo != null
																					 ? (s.ResultProdLotRomaneo.numberLotSequential.Contains(consultar.secTransaccional))
																					 : (consultar.secTransaccional == null || consultar.secTransaccional == "")));


			return expression;
		}
	}
}