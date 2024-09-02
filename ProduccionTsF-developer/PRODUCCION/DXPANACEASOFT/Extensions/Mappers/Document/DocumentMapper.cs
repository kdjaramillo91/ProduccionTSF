using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DXPANACEASOFT.Extensions
{
    public static class DocumentMapper
    {
        public static Document ToSimpleModel(this Document input)
        {
            return new Document
            {
                id = input.id,
                number = input.number,
                sequential = input.sequential,
                emissionDate = input.emissionDate,
                authorizationDate = input.authorizationDate,
                authorizationNumber = input.authorizationNumber,
                accessKey = input.accessKey,
                description = input.description,
                reference = input.reference,
                id_emissionPoint = input.id_emissionPoint,
                id_documentType = input.id_documentType,
                id_documentState = input.id_documentState,
                id_userCreate = input.id_userCreate,
                dateCreate = input.dateCreate,
                id_userUpdate = input.id_userUpdate,
                dateUpdate = input.dateUpdate,
                isOpen = input.isOpen,
                id_documentTransactionState = input.id_documentTransactionState,
                id_documentOrigen = input.id_documentOrigen,
                DocumentType = input.DocumentType.ToSimpleModel(),
                DocumentState = input.DocumentState.ToSimpleModel()
            };
        }

        public static DocumentType ToSimpleModel(this DocumentType input)
        {
            var model = new DocumentType();
            if (input != null)
            {
                model = new DocumentType
                {
                    id = input.id,
                    code = input.code,
                    name = input.name,
                    description = input.description,
                    currentNumber = input.currentNumber,
                    daysToExpiration = input.daysToExpiration,
                    isElectronic = input.isElectronic,
                    codeSRI = input.codeSRI,
                    id_company = input.id_company,
                    isActive = input.isActive,
                    id_userCreate = input.id_userCreate,
                    dateCreate = input.dateCreate,
                    id_userUpdate = input.id_userUpdate,
                    dateUpdate = input.dateUpdate
                };
            }
            return model;
        }

        public static DocumentState ToSimpleModel(this DocumentState input)
        {
            var model = new DocumentState();
            if (input != null)
            {
                model = new DocumentState
                {
                    id = input.id,
                    code = input.code,
                    name = input.name,
                    description = input.description,
                    id_company = input.id_company,
                    isActive = input.isActive,
                    id_userCreate = input.id_userCreate,
                    dateCreate = input.dateCreate,
                    id_userUpdate = input.id_userCreate,
                    dateUpdate = input.dateUpdate
                };
            }
            return model;
        }
    }
}