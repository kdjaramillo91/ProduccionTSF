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
    
    public partial class TblBcCtaCteServContratado
    {
        public string CCiBanco { get; set; }
        public string CCiCtaCte { get; set; }
        public string CCiServicioPago { get; set; }
        public string CCiTipoMvtoBco { get; set; }
        public string CCiMotivoBco { get; set; }
    
        public virtual TblCiCtaCte TblCiCtaCte { get; set; }
    }
}
