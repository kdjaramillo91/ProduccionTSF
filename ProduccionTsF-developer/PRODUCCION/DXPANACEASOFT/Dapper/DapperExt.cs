using Dapper;
using DXPANACEASOFT.Models;
using System.Data.SqlClient;

namespace DXPANACEASOFT.Dapper
{
    internal static class DapperExt
    {
        internal static int InsertDocument(Document document, SqlConnection connection, SqlTransaction transaction)
        {
            const string INSERT_DOCUMENT = "INSERT DOCUMENT " +
                                       "(number,sequential,emissionDate,description,id_emissionPoint,id_documentType,id_documentState, " +
                                       "id_userCreate,dateCreate,id_userUpdate,dateUpdate) " +
                                       "VALUES (@number, @sequential, @emissionDate, @description, @id_emissionPoint, @id_documentType, @id_documentState,  " +
                                       "@id_userCreate, @dateCreate, @id_userUpdate, @dateUpdate); SELECT SCOPE_IDENTITY()";

            var documentId = connection.ExecuteScalar<int>(INSERT_DOCUMENT, new
            {
                number = document.number,
                sequential = document.sequential,
                emissionDate = document.emissionDate,
                description = document.description,
                id_emissionPoint = document.id_emissionPoint,
                id_documentType = document.id_documentType,
                id_documentState = document.id_documentState,
                id_userCreate = document.id_userCreate,
                dateCreate = document.dateCreate,
                id_userUpdate = document.id_userUpdate,
                dateUpdate = document.dateUpdate
            }, transaction);
            return documentId;
        }

        internal static void InsertNotification(Notification notification, SqlConnection connection, SqlTransaction transaction)
        {
            const string INSERT_NOTIFICATION = "INSERT INTO Notification " +
                                            "(id_user, id_document, noDocument, id_documentType, documentType, " +
                                            "id_documentState, documentState, title, description, " +
                                            "dateTime, reading) " +
                                            "VALUES (@id_user, @id_document, @noDocument, @id_documentType, @documentType, " +
                                            "@id_documentState, @documentState, @title, @description, " +
                                            "@dateTime, @reading);";

            connection.Execute(INSERT_NOTIFICATION, new
            {
                id_user = notification.id_user,
                id_document = notification.id_document,
                noDocument = notification.noDocument,
                id_documentType = notification.id_documentType,
                documentType = notification.documentType,
                id_documentState = notification.id_documentState,
                documentState = notification.documentState,
                title = notification.title,
                description = notification.description,
                dateTime = notification.dateTime,
                reading = notification.reading,

            }, transaction);
        }
    }

}