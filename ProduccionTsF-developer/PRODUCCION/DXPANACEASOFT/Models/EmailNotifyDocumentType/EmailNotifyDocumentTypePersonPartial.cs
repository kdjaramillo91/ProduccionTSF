using DXPANACEASOFT.Dapper;

namespace DXPANACEASOFT.Models
{
    public partial class EmailNotifyDocumentTypePerson
    {
        public static string SELECT_ALL_BY_EMAILNOTIFYID = "SELECT id, id_EmailNotifyDocumentType,id_PersonReceiver FROM " +
                                                           "EmailNotifyDocumentTypePerson WHERE id_EmailNotifyDocumentType = @id_EmailNotifyDocumentType ";



        public static EmailNotifyDocumentTypePerson[] GetAllByEmailNotifyId(int id_EmailNotifyDocumentType)
        {
            return DapperConnection.Execute<EmailNotifyDocumentTypePerson>(SELECT_ALL_BY_EMAILNOTIFYID, new
            {
                id_EmailNotifyDocumentType = id_EmailNotifyDocumentType
            });
        }
    }
}