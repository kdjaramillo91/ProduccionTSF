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
    
    public partial class PurchaseOrderImportationInformation
    {
        public int id { get; set; }
        public string customsDocumentNumber { get; set; }
        public string referendumNumber { get; set; }
        public string shipmentDate { get; set; }
        public string departureCustomsDate { get; set; }
        public string arrivalDate { get; set; }
    
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
