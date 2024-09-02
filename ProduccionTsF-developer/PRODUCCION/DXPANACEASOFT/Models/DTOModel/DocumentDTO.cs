using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.DTOModel
{
    public class DocumentDTO
    {
        public int id { get; set; }
        public string number { get; set; }
        public DateTime emissionDate { get; set; }
        public string description { get; set; }
        public string reference { get; set; }
        public int id_documentType { get; set; }
        public string documentType { get; set; }
        public int id_documentState { get; set; }
        public string documentState { get; set; }

    }
}