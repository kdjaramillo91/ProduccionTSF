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
    
    public partial class BusinessOportunityPartner
    {
        public int id { get; set; }
        public int id_businessOportunity { get; set; }
        public int id_partner { get; set; }
        public string referencePartner { get; set; }
        public string descriptionPartner { get; set; }
        public bool manual { get; set; }
    
        public virtual BusinessOportunity BusinessOportunity { get; set; }
        public virtual Person Person { get; set; }
    }
}
