
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.DocumentP.DocumentModels
{
    public class DocumentModelP
    {
        public int idDocumentModelP { get; set; }
        public string numberModelP { get; set; }
        public int sequentialModelP { get; set; }

        public DateTime emissionDateModelP { get; set; }

        public DateTime? authorizationDateModelP { get; set; }

        public string authorizationNumberModelP { get; set; }
        public string accessKeyModelP { get; set; }
        public string descriptionModelP { get; set; }
        public string referenceModelP { get; set; }
        public int idEmissionPointModelP { get; set; }
        public int idDocumentTypeModelP { get; set; }
        public int idDocumentStateModelP { get; set; }

        public bool? isOpenModelP { get; set; }
    }
    public class DocumentDocumentOriginInformationModelP
    {
        public int idDocumentModelP { get; set; }

        public int idDocumentOriginModelP { get; set; }

        public string sequentialDocumentOriginModelP { get; set; }
            
        public DateTime emissionDate { get; set; }
    }
    public class DocumentLogModelP
    {
        public int idDocumentModelP { get; set; }

        public int idActionOnDocumentModelP { get; set; }

        public string descriptionModelP { get; set; }

        public int idUserModelP { get; set; }

        public DateTime dtDateModelP { get; set; }
    }
    public class DocumentTrackStateModelP
    {
        public int idDocumentModelP { get; set; }

        public int idActionOnDocumentModelP { get; set; }

        public string descriptionModelP { get; set; }

        public int idUserModelP { get; set; }

        public DateTime dtDateModelP { get; set; }
    }
    public class ActionOnDocumentModelP
    {
        public int idActionOnDocumentModelP { get; set; }
        public string codeActionOnDocumentModelP { get; set; }
        public string nameActionOnDocumentModelP { get; set; }
        public string descriptionActionOnDocumentModelP { get; set; }
        public bool isActiveActionOnDocumentModelP { get; set; }
    }
}