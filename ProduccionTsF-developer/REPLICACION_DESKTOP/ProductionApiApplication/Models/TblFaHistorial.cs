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
    
    public partial class TblFaHistorial
    {
        public int NNuIdSecuencia { get; set; }
        public int NNuIdCuenta { get; set; }
        public string CCiNumeroTelefono { get; set; }
        public short NNuIdPlan { get; set; }
        public string CCiTipoIdentificacion { get; set; }
        public string CCiDocumento { get; set; }
        public System.DateTime DFxRegistro { get; set; }
        public short NNuIdDireccionEnv { get; set; }
        public short NNuIdDireccionInst { get; set; }
        public string CCiTipoCambio { get; set; }
        public string CCiDocumentoAnterior { get; set; }
        public string CdsNombreAnterior { get; set; }
        public System.DateTime DFxIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
    }
}
