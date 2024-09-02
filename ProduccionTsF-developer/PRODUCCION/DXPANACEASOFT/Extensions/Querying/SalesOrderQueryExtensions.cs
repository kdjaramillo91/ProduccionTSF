using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
	public static class SalesOrderQueryExtensions
    {
		public static Expression<Func<SalesOrder, bool>> GetRequestByFilter(SalesOrderConsultDTO consultar)
		{
            var db = new DBContext();
            
            if (string.IsNullOrEmpty(consultar.number))
            {
                var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
                var fechaFinEmision = string.IsNullOrEmpty(consultar.endtDate) ? (DateTime?)null : DateTime.Parse(consultar.endtDate);

                Expression<Func<SalesOrder, bool>> expression = s =>
               ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
                (fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
               ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
               //((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
               ((consultar.reference == null || consultar.reference == "") || (s.Document.reference.Contains(consultar.reference))) &&
               ((consultar.id_documentType == null || consultar.id_documentType == 0) || (consultar.id_documentType == s.Document.id_documentType)) &&
               ((consultar.id_customer == null || consultar.id_customer == 0) || (consultar.id_customer == s.id_customer)) &&
               ((consultar.id_seller == null || consultar.id_seller == 0) || (consultar.id_seller == s.id_employeeSeller)) &&
               ((consultar.id_Logistics == null) || (consultar.id_Logistics == 0 && !s.requiredLogistic) || (consultar.id_Logistics == 1 && s.requiredLogistic));
                return expression;
            }
            else {
                Expression<Func<SalesOrder, bool>> expression = s =>
               (consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number));
                return expression;
            }
            
		}
	}
}