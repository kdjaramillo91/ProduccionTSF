//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DestinoListaProduccion
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductionCostCoefficientExecutionPlant
    {
        public int id { get; set; }
        public int id_coefficientExecution { get; set; }
        public int id_productionPlant { get; set; }
        public int id_inventoryLine { get; set; }
        public int id_itemType { get; set; }
        public decimal libras { get; set; }
        public decimal porcentaje { get; set; }
        public decimal valor { get; set; }
        public decimal coeficiente { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
    
        public virtual InventoryLine InventoryLine { get; set; }
        public virtual ItemType ItemType { get; set; }
        public virtual Person Person { get; set; }
        public virtual ProductionCostCoefficientExecution ProductionCostCoefficientExecution { get; set; }
    }
}
