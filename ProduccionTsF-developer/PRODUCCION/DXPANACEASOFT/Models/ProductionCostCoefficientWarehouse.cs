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
    
    public partial class ProductionCostCoefficientWarehouse
    {
        public int id { get; set; }
        public int id_coefficient { get; set; }
        public int id_warehouse { get; set; }
    
        public virtual Warehouse Warehouse { get; set; }
        public virtual ProductionCostCoefficient ProductionCostCoefficient { get; set; }
    }
}
