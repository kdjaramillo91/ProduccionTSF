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
    public static class LiquidationMaterialQueryExtensions
    {
        public static Expression<Func<LiquidationMaterialSupplies, bool>> GetRequestByFilter(LiquidationMaterialConsultDTO consultar)
        {
            var fechaInicioEmision = string.IsNullOrEmpty(consultar.fechaInicioEmision) ? (DateTime?)null : DateTime.Parse(consultar.fechaInicioEmision);
            var fechaFinEmision = string.IsNullOrEmpty(consultar.fechaFinEmision) ? (DateTime?)null : DateTime.Parse(consultar.fechaFinEmision);

            var fechaInicioGuia = string.IsNullOrEmpty(consultar.fechaInicioGuia) ? (DateTime?)null : DateTime.Parse(consultar.fechaInicioGuia);
            var fechaFinGuia = string.IsNullOrEmpty(consultar.fechaFinGuia) ? (DateTime?)null : DateTime.Parse(consultar.fechaFinGuia);

            Expression<Func<LiquidationMaterialSupplies, bool>> expression = s =>
               ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
                (fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
               ((consultar.id_estado == null || consultar.id_estado == 0) || (consultar.id_estado == s.Document.id_documentState)) &&
               ((consultar.numeroLiquidacion == null || consultar.numeroLiquidacion == "") || (s.Document.number.Contains(consultar.numeroLiquidacion))) &&
               ((/*consultar.id_proveedor == null ||*/ consultar.id_proveedor == 0) || (consultar.id_proveedor == s.idProvider)) &&
               ((/*consultar.id_producto == null ||*/ consultar.id_producto == 0) || (s.LiquidationMaterialSuppliesDetail.FirstOrDefault() != null
                                                                                     ? s.LiquidationMaterialSuppliesDetail.FirstOrDefault().idItem == consultar.id_producto
                                                                                     : false)) &&
               (((s.ResultReceptionDispatchMaterial.FirstOrDefault().dateRemissionGuide != null
                                                                                     ? fechaInicioGuia == null || fechaInicioGuia <= DbFunctions.TruncateTime(s.ResultReceptionDispatchMaterial.FirstOrDefault().dateRemissionGuide)
                                                                                     : true) &&
                                                                                     (s.ResultReceptionDispatchMaterial.FirstOrDefault().dateRemissionGuide != null
                                                                                     ? fechaFinGuia == null || fechaFinGuia >= DbFunctions.TruncateTime(s.ResultReceptionDispatchMaterial.FirstOrDefault().dateRemissionGuide)
                                                                                     : true))) &&
               ((consultar.numeroGuia == null || consultar.numeroGuia == "") || (s.ResultReceptionDispatchMaterial.FirstOrDefault().dateRemissionGuide != null
                                                                                     ? (s.ResultReceptionDispatchMaterial.FirstOrDefault().numberRemissionGuide.Contains(consultar.numeroGuia))
                                                                                     : true));


            return expression;
        }
    }
}