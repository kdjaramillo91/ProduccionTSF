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
    
    public partial class TblFaLineaTelefonica
    {
        public string CCiNumeroTelefono { get; set; }
        public string CCiTipoLinea { get; set; }
        public string CCiCentral { get; set; }
        public Nullable<short> NNuIdPuerto { get; set; }
        public Nullable<int> NNuIdBoxPair { get; set; }
        public string CCiDirIP { get; set; }
        public Nullable<short> NNuiIdPuerto { get; set; }
        public string CCeActiva { get; set; }
        public string CCiEstadoLinea { get; set; }
        public Nullable<short> NNuIdGrupoTelef { get; set; }
        public Nullable<short> NNuNumeroZona { get; set; }
        public Nullable<short> NNuNumeroSubZona { get; set; }
        public string CDsLugar { get; set; }
        public System.DateTime DFxIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public System.DateTime DFxModifica { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
    
        public virtual TblFaCentral TblFaCentral { get; set; }
        public virtual TblFaEstadoLinea TblFaEstadoLinea { get; set; }
        public virtual TblFaGrupoTelefono TblFaGrupoTelefono { get; set; }
        public virtual TblFaLineaNoFactura TblFaLineaNoFactura { get; set; }
        public virtual TblFaSubZona TblFaSubZona { get; set; }
        public virtual TblFaTdmPuerto TblFaTdmPuerto { get; set; }
        public virtual TblFaTdmBoxPair TblFaTdmBoxPair { get; set; }
    }
}
