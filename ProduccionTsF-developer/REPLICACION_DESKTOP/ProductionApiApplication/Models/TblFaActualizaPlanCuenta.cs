//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProductionApiApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblFaActualizaPlanCuenta
    {
        public int NNuIdSecuencia { get; set; }
        public int NNuIdPlanCuenta { get; set; }
        public int NNuIdCuenta { get; set; }
        public short NNuIdPlan { get; set; }
        public System.DateTime DFxRegistro { get; set; }
        public System.DateTime DFxProceso { get; set; }
        public string CSNProcesado { get; set; }
    
        public virtual TblFaCuenta TblFaCuenta { get; set; }
        public virtual TblFaPlan TblFaPlan { get; set; }
    }
}
