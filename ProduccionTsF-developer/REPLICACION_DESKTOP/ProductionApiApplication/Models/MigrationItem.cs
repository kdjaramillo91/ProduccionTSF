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
    
    public partial class MigrationItem
    {
        public int id { get; set; }
        public int id_item { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
    
        public virtual Item Item { get; set; }
    }
}
