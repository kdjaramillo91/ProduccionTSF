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
    
    public partial class ProductionCostProductValuationWarehouse
    {
        public int id { get; set; }
        public int id_productValuation { get; set; }
        public int id_warehouse { get; set; }
        public int id_periodState { get; set; }
        public bool process { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
    
        public virtual AdvanceParametersDetail AdvanceParametersDetail { get; set; }
        public virtual ProductionCostProductValuation ProductionCostProductValuation { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
