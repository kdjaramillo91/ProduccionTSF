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
    
    public partial class SaldoInicial
    {
        public int id { get; set; }
        public int año { get; set; }
        public int mes { get; set; }
        public int idBodega { get; set; }
        public int idItem { get; set; }
        public int cantidad { get; set; }
        public decimal costo_unitario { get; set; }
        public decimal costo_total { get; set; }
        public bool activo { get; set; }
        public int idUsuarioCreacion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int idUsuarioModificacion { get; set; }
        public System.DateTime fechaModificacion { get; set; }
    }
}
