
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.OpeningCloseDocumentsDTO
{
    public class OpeningCloseDocumentsFilter
    {
        public int? id_DocumentType { get; set; }
        
        public DateTime? emissionDateStart { get; set; }

        public DateTime? emissionDateEnd { get; set; }

    }
    public class OpeningCloseDocumentResults
    {
        public int id { get; set; }

        public string numberDoc { get; set; }

        public DateTime emissionDate { get; set; }

		public string codeDocumentType { get; set; }

		public string nameDocumentType { get; set; }

        public string nameProvider { get; set; }

        public string nameDocumentState { get; set; }

        public bool isOpen { get; set; }
    }
}