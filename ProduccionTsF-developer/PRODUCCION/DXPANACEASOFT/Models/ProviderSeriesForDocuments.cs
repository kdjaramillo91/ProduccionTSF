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
    
    public partial class ProviderSeriesForDocuments
    {
        public int id { get; set; }
        public int id_provider { get; set; }
        public int id_documentType { get; set; }
        public string serialNumber { get; set; }
        public string authorizationNumber { get; set; }
        public int initialNumber { get; set; }
        public int finalNumber { get; set; }
        public System.DateTime dateOfExpiry { get; set; }
        public int currentNumber { get; set; }
        public int id_retentionSeriesForDocumentsType { get; set; }
        public bool isActive { get; set; }
    
        public virtual DocumentType DocumentType { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual RetentionSeriesForDocumentsType RetentionSeriesForDocumentsType { get; set; }
    }
}
