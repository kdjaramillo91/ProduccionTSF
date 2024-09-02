using DXPANACEASOFT.Dapper;
using System.Linq;

namespace DXPANACEASOFT.Models
{

    public partial class EmailNotifyDocumentType
    {

        public static string SELECT_ONE_BY_ID = "SELECT id,id_DocumentType,description,id_company,isActive,id_userCreate,dateCreate,id_userUpdate,dateUpdate " +
                                                "FROM EmailNotifyDocumentType WHERE ID = @ID";

        public static string SELECT_ONE_BY_ID_DOCUMENTTYPE = "SELECT id,id_DocumentType,description,id_company,isActive,id_userCreate,dateCreate,id_userUpdate,dateUpdate " +
                                                             "FROM EmailNotifyDocumentType WHERE id_DocumentType = @id_DocumentType";

        public static string SELECT_ALL_BY_COMPANYID = "SELECT id,id_DocumentType,description,id_company,isActive,id_userCreate,dateCreate,id_userUpdate,dateUpdate " +
                                                        "FROM EmailNotifyDocumentType WHERE ID_COMPANY = @ID_COMPANY";

        public static string SELECT_ALL_BY_CONTAIN_IDS = "SELECT id,id_DocumentType,description,id_company,isActive,id_userCreate,dateCreate,id_userUpdate,dateUpdate " +
                                                         "FROM EmailNotifyDocumentType WHERE ID IN @IDS";

        

        public static EmailNotifyDocumentType GetOneById(int id)
        {
            return DapperConnection.Execute<EmailNotifyDocumentType>(SELECT_ONE_BY_ID, new 
            {
                ID =id
            })?.FirstOrDefault();
        }

        public static EmailNotifyDocumentType GetOneByIdDocumentType(int id_DocumentType)
        {
            return DapperConnection.Execute<EmailNotifyDocumentType>(SELECT_ONE_BY_ID_DOCUMENTTYPE, new
            {
                id_DocumentType = id_DocumentType
            })?.FirstOrDefault();
        }
        

        public static EmailNotifyDocumentType[] GetAllByCompanyId(int companyId)
        {
            return DapperConnection.Execute<EmailNotifyDocumentType>(SELECT_ALL_BY_COMPANYID, new
            {
                ID_COMPANY = companyId
            });
        }
    
        public static EmailNotifyDocumentType[] GetAllByContaisIds(int[] ids)
        {
            return DapperConnection.Execute<EmailNotifyDocumentType>(SELECT_ALL_BY_CONTAIN_IDS, new
            {
                IDS = ids
            });
        }
    

    }
}