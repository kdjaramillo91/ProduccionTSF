
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.DocumentP.DocumentDTO
{
    public class DocumentTansferP
    {
        public int idDocumentTransferP { get; set; }
        public string numberTransferP { get; set; }
        public int sequentialTransferP { get; set; }

        public DateTime emissionDateTransferP { get; set; }

        public DateTime? authorizationDateTransferP { get; set; }

        public string authorizationNumberTransferP { get; set; }
        public string accessKeyTransferP { get; set; }
        public string descriptionTransferP { get; set; }
        public string referenceTransferP { get; set; }
        public int? idEmissionPointTransferP { get; set; }
        public int? idDocumentTypeTransferP { get; set; }
        public string nameDocumentTypeTransferP { get; set; }
        public int? idDocumentStateTransferP { get; set; }

        public string codeDocumentStateTransferP { get; set; }

        public string nameDocumentStateTransferP { get; set; }

        public bool? isOpenTransferP { get; set; }

        public string nameWarehouseHeadP { get; set; }

        public int? idNatureMoveHeadP { get; set; }

        public bool ReadOnlyBeRemissionGuide { get; set; }

        public int idWarehouseHeadP { get; set; }
    }
}