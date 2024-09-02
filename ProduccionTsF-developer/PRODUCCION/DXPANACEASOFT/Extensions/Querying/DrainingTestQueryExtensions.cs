using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;

namespace DXPANACEASOFT.Extensions.Querying
{
    public static class DrainingTestQueryExtensions
    {

        public static Expression<Func<DrainingTest, bool>> GetRequestByFilter(DrainingTestConsultDTO consultar)
        {
            var fechaInicio = DateTime.Parse(consultar.initDate) ;
            var fechaFin = DateTime.Parse(consultar.endtDate);

            Expression<Func<DrainingTest, bool>> expression = s =>
                ((fechaInicio <= DbFunctions.TruncateTime(s.Document.emissionDate) && fechaFin >= DbFunctions.TruncateTime(s.Document.emissionDate) )) &&
                ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
                ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
                ((consultar.reference == null || consultar.reference == "") || (s.Document.reference.Contains(consultar.reference))) &&
                ((consultar.numberLote == null || consultar.numberLote == "") || (s.ReceptionDetailDrainingTest.FirstOrDefault().ResultProdLotReceptionDetail != null 
                                                                                      ? s.ReceptionDetailDrainingTest.FirstOrDefault().ResultProdLotReceptionDetail.numberLot.Contains(consultar.numberLote)
                                                                                      :false));
                
            return expression;
        }

    }
}