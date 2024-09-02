using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.DTOModel
{
    public class RemissionGuideControlVehicleDto
    {
        public Document Document { get; set; }

        public string startAdress { get; set; }
        public System.DateTime despachureDate { get; set; }
        public System.DateTime arrivalDate { get; set; }
        public System.DateTime returnDate { get; set; }

        public IList<RemissionGuideDetail> RemissionGuideDetail { get; set; }
        public IList<RemissionGuideDispatchMaterial> RemissionGuideDispatchMaterial { get; set; }
        public bool? isInternal { get; set; }
        public RemissionGuideTransportation RemissionGuideTransportation { get; set; }

        public ControlVehicleDto RemissionGuideControlVehicle { get; set; }
    }

    public class ControlVehicleDto
    {
        public string DocumentStateCode { get; set; }
        public int id_remissionGuide { get; set; }
        public DateTime? exitDateProductionBuilding { get; set; }
        public System.TimeSpan? exitTimeProductionBuilding { get; set; }
        public System.DateTime? entranceDateProductionUnitProviderBuilding { get; set; }
        public System.TimeSpan? entranceTimeProductionUnitProviderBuilding { get; set; }
        public System.DateTime? exitDateProductionUnitProviderBuilding { get; set; }
        public System.TimeSpan? exitTimeProductionUnitProviderBuilding { get; set; }
        public System.DateTime? entranceDateProductionBuilding { get; set; }
        public System.TimeSpan? entranceTimeProductionBuilding { get; set; }
        public bool? hasEntrancePlanctProduction { get; set; }
        public bool? hasExitPlanctProduction { get; set; }
        public string ObservationEntrance { get; set; }
        public string ObservationExit { get; set; }
        public bool? hasSentEmail { get; set; }
        public string providerNameAux { get; set; }
        public string productionUnitProviderAux { get; set; }
        public string poolNameAux { get; set; }
        public decimal totalPoundsAux { get; set; }
        public string zoneNameAux { get; set; }
        public string siteNameAux { get; set; }
        public string addressTargetAux { get; set; }
        public string shippingTypeAux { get; set; }
        public string driverNameAux { get; set; }
        public string productionUnitProviderAuxLab { get; set; }

        public string PersonProcessPlant { get; set; }
        public virtual RemissionGuide RemissionGuide { get; set; }
        //public string Guia_externa { get; set; }
    }
}