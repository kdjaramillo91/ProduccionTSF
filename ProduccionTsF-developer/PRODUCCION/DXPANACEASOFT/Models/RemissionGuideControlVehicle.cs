//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DXPANACEASOFT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RemissionGuideControlVehicle
    {
        public int id_remissionGuide { get; set; }
        public Nullable<System.DateTime> exitDateProductionBuilding { get; set; }
        public Nullable<System.TimeSpan> exitTimeProductionBuilding { get; set; }
        public Nullable<System.DateTime> entranceDateProductionUnitProviderBuilding { get; set; }
        public Nullable<System.TimeSpan> entranceTimeProductionUnitProviderBuilding { get; set; }
        public Nullable<System.DateTime> exitDateProductionUnitProviderBuilding { get; set; }
        public Nullable<System.TimeSpan> exitTimeProductionUnitProviderBuilding { get; set; }
        public Nullable<System.DateTime> entranceDateProductionBuilding { get; set; }
        public Nullable<System.TimeSpan> entranceTimeProductionBuilding { get; set; }
        public Nullable<bool> hasEntrancePlanctProduction { get; set; }
        public Nullable<bool> hasExitPlanctProduction { get; set; }
        public string ObservationEntrance { get; set; }
        public string ObservationExit { get; set; }
        public Nullable<bool> hasSentEmail { get; set; }
        public Nullable<decimal> entrancePoundsRemitted { get; set; }
        public string Piscinas { get; set; }
        public Nullable<int> NoGabetas { get; set; }
        public Nullable<int> NoBines { get; set; }
    
        public virtual RemissionGuide RemissionGuide { get; set; }
    }
}
