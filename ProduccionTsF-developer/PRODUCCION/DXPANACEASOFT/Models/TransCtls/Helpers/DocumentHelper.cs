using DXPANACEASOFT.Models.DTOModel;
using System;

namespace DXPANACEASOFT.Models.Helpers
{
    public static class DocumentHelper
    {
        public static DocumentDTO ToDto(this Document input)
        {
            DocumentDTO model = null;
            if (input == null) return model;

            model = new DocumentDTO
            {
                id = input.id,
                number = input.number,
                emissionDate = input.emissionDate,
                description = input.description,
                reference = input.reference,
                id_documentType = input.id_documentType,                  
                id_documentState = input.id_documentState                
            };
            return model;
        }

        public static Document ToModel(this DocumentDTO input)
        {
            Document model = null;
            if (input == null) return model;

            model = new Document
            {
                id = input.id,
                number = input.number,
                emissionDate = input.emissionDate,
                description = input.description,
                reference = input.reference,
                id_documentType = input.id_documentType,
                id_documentState = input.id_documentState
            };
            return model;
        }
    }
}