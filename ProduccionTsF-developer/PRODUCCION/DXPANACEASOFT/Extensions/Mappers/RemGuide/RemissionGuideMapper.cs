using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
//using DXPANACEASOFT.Models.RemGuide.ControlVehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Extensions
{
    public static class RemissionGuideMapper
    {
        public static ControlVehicleDto ToDto(this RemissionGuideControlVehicle input)
        {
            return new ControlVehicleDto
            {
                addressTargetAux = input.addressTargetAux,
                driverNameAux = input.driverNameAux,
                entranceDateProductionBuilding = input.entranceDateProductionBuilding,
                entranceDateProductionUnitProviderBuilding = input.entranceDateProductionUnitProviderBuilding,
                entranceTimeProductionBuilding = input.entranceTimeProductionBuilding,
                entranceTimeProductionUnitProviderBuilding = input.entranceTimeProductionUnitProviderBuilding,
                exitDateProductionBuilding = input.exitDateProductionBuilding,
                exitDateProductionUnitProviderBuilding = input.exitDateProductionUnitProviderBuilding,
                exitTimeProductionBuilding = input.exitTimeProductionBuilding,
                exitTimeProductionUnitProviderBuilding = input.exitTimeProductionUnitProviderBuilding,
                hasEntrancePlanctProduction = input.hasEntrancePlanctProduction,
                hasExitPlanctProduction = input.hasExitPlanctProduction,
                hasSentEmail = input.hasSentEmail,
                id_remissionGuide = input.id_remissionGuide,
                ObservationEntrance = input.ObservationEntrance,
                ObservationExit = input.ObservationExit,
                poolNameAux = input.poolNameAux,
                productionUnitProviderAux = input.productionUnitProviderAux,
                productionUnitProviderAuxLab = input.productionUnitProviderAuxLab,
                providerNameAux = input.providerNameAux,
                shippingTypeAux = input.shippingTypeAux,
                siteNameAux = input.siteNameAux,
                totalPoundsAux = input.totalPoundsAux,
                zoneNameAux = input.zoneNameAux,
                RemissionGuide = input?.RemissionGuide,

            };
        }
    }
}