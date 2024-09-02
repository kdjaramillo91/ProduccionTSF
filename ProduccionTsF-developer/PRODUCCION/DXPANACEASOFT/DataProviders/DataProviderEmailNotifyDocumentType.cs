using DXPANACEASOFT.Models;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderEmailNotifyDocumentType
    {
        private static DBContext db = null;

        public static EmailNotifyDocumentType EmailNotifyDocumentTypeById(int? id_emailNotifyDocumentType)
        {
            //db = new DBContext();
            if(id_emailNotifyDocumentType.HasValue) return EmailNotifyDocumentType.GetOneById(id_emailNotifyDocumentType.Value);
            //db.EmailNotifyDocumentType.FirstOrDefault(t =>  t.id == id_emailNotifyDocumentType);

            return null;
        }
    }
    


}