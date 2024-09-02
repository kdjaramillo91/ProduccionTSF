using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;

namespace DXPANACEASOFT.Extensions.Querying
{
    public static class PriceListQueryExtensions
    {
        public static Expression<Func<PriceList, bool>> GetRequestByFilter(User ActiveUser, PriceListConsultDTO consultar, bool seeAllStates = false)
        {
            var fechaInicio = DateTime.Parse(consultar.fechaInicio);
            var fechaFin = DateTime.Parse(consultar.fechaFin);

            Expression<Func<PriceList, bool>> expression = s =>
                ((fechaInicio <= s.startDate && fechaFin >= s.startDate) || (fechaInicio <= s.endDate && fechaFin >= s.endDate)) &&
                ((consultar.id_estado == null || consultar.id_estado == 0) || (consultar.id_estado == s.Document.id_documentState)) && 
                ((consultar.id_grupo == null || consultar.id_grupo == 0) || (s.isQuotation && consultar.id_grupo == s.id_groupPersonByRol)) &&
                ((consultar.id_responsable == null || consultar.id_responsable == 0) || (consultar.id_responsable == s.id_userResponsable)) &&
                ((consultar.id_tipoListaCamaron == null || consultar.id_tipoListaCamaron == 0) || (consultar.id_tipoListaCamaron == s.id_processtype)) &&
                
                (((consultar.id_tipoLista == null || consultar.id_tipoLista == 0) && 
                  (seeAllStates == true || 
                  (!s.isQuotation || (s.isQuotation && (s.Document.id_userCreate == ActiveUser.id || s.id_userResponsable == ActiveUser.id))))) || 
                 (!s.isQuotation && consultar.id_tipoLista == 1 ) || 
                 (s.isQuotation && consultar.id_tipoLista == 2 && (seeAllStates == true || (s.Document.id_userCreate == ActiveUser.id || s.id_userResponsable == ActiveUser.id)))) &&
                
                 ((consultar.id_proveedor == 0 || consultar.id_proveedor == null) || (s.isQuotation && consultar.id_proveedor == s.PriceListPersonPersonRol.FirstOrDefault().id_Person)) &&

                 ((consultar.id_certification == 0 || consultar.id_certification == null) || (s.isQuotation && consultar.id_certification == s.id_certification));

            return expression;
        }

        public static Expression<Func<PriceList, bool>> GetRequestPendingAproval(User ActiveUser, int id_crateState, int id_reversedState, int id_aprovedState)
        {
            Expression<Func<PriceList, bool>> expression = s =>
                (s.Document.id_documentState == id_crateState)
                ? (id_crateState != id_reversedState) || 
                  (s.Document.id_userCreate == ActiveUser.id)
                : (s.Document.id_documentState == id_reversedState &&
                   s.Document.id_documentState != id_aprovedState);

            return expression;
        }
    }
}