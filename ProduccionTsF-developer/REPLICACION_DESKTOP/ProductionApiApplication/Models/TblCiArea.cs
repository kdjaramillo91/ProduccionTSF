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
    
    public partial class TblCiArea
    {
        public string CCiTipoPres { get; set; }
        public string CCiProyecto { get; set; }
        public string CCiSubProyecto { get; set; }
        public string CCiArea { get; set; }
        public string CDsArea { get; set; }
        public string CCeArea { get; set; }
    
        public virtual TblCiSubProyecto TblCiSubProyecto { get; set; }
    }
}
