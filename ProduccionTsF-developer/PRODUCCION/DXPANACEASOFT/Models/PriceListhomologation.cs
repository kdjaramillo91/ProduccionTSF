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
    
    public partial class PriceListhomologation
    {
        public int id { get; set; }
        public int Id_LPListaPrecios { get; set; }
        public int Id_PriceList { get; set; }
        public Nullable<int> Id_LPGrupo { get; set; }
        public string Id_TipoLPListaPrecios { get; set; }
        public Nullable<decimal> PrecioB { get; set; }
        public Nullable<int> id_LPProveedor { get; set; }
    }
}
