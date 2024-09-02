using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DXPANACEASOFT.Extensions.Querying
{
    public static class ClosePlateTunnelCoolQueryExtensions
    {
        public static Expression<Func<ClosePlateTunnelCool, bool>> GetRequestByFilter(ClosePlateTunnelCoolConsultDTO consultar)
        {
            var fechaInicioEmision = string.IsNullOrEmpty(consultar.initDate) ? (DateTime?)null : DateTime.Parse(consultar.initDate);
            var fechaFinEmision = string.IsNullOrEmpty(consultar.endtDate) ? (DateTime?)null : DateTime.Parse(consultar.endtDate);

            Expression<Func<ClosePlateTunnelCool, bool>> expression = s =>
               ((fechaInicioEmision == null || fechaInicioEmision <= DbFunctions.TruncateTime(s.Document.emissionDate)) &&
                (fechaFinEmision == null || fechaFinEmision >= DbFunctions.TruncateTime(s.Document.emissionDate))) &&
               ((consultar.id_state == null || consultar.id_state == 0) || (consultar.id_state == s.Document.id_documentState)) &&
               ((consultar.number == null || consultar.number == "") || (s.Document.number.Contains(consultar.number))) &&
               ((consultar.id_turn == null || consultar.id_turn == 0) || (consultar.id_turn == s.MachineProdOpeningDetail.MachineProdOpening.id_Turn)) &&
               ((consultar.id_machineForProd == null || consultar.id_machineForProd == 0) || (consultar.id_machineForProd == s.MachineProdOpeningDetail.id_MachineForProd)) &&
               ((consultar.id_person == null || consultar.id_person == 0) || (consultar.id_person == s.MachineProdOpeningDetail.id_Person));// &&
               //((consultar.numberLot == null || consultar.numberLot == "") || (s.MachineProdOpeningDetail.InventoryMovePlantTransfer.MachineProdOpening.LiquidationCartOnCart.FirstOrDefault(fod => fod.id_MachineForProd == s.MachineProdOpeningDetail.id_MachineForProd &&
               //                                                                                                                                                         fod.Document.DocumentState.code != "05" &&
               //                                                                                                                                                         fod.ProductionLot.internalNumber.Contains(consultar.numberLot))) != null) &&
               //((consultar.id_provider == null || consultar.id_provider == 0) || (s.MachineProdOpeningDetail.MachineProdOpening.LiquidationCartOnCart.FirstOrDefault(fod => fod.id_MachineForProd == s.MachineProdOpeningDetail.id_MachineForProd &&
               //                                                                                                                                                         fod.Document.DocumentState.code != "05" &&
               //                                                                                                                                                         fod.ProductionLot.id_provider == consultar.id_provider)) != null)&&
               //((consultar.id_productionUnitProvider == null || consultar.id_productionUnitProvider == 0) || (s.MachineProdOpeningDetail.MachineProdOpening.LiquidationCartOnCart.FirstOrDefault(fod => fod.id_MachineForProd == s.MachineProdOpeningDetail.id_MachineForProd &&
               //                                                                                                                                                         fod.Document.DocumentState.code != "05" &&
               //                                                                                                                                                         fod.ProductionLot.id_productionUnitProvider == consultar.id_productionUnitProvider)) != null);

            return expression;
        }
    }
}