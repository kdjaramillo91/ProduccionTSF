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
    
    public partial class LPListaPreciosProveedor
    {
        public int ListaPrecioId { get; set; }
        public int ProveedorId { get; set; }
        public bool Cerrada { get; set; }
        public Nullable<System.DateTime> FechaCierre { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
    }
}
