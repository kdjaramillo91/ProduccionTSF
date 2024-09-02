using DocumentFormat.OpenXml.Drawing.Charts;
using DXPANACEASOFT.Models;
using System.Linq;
using Z.Dapper.Plus;

namespace DXPANACEASOFT.Dapper
{
    public static class DapperConnectionConfig
    {
        public static void ConfigPaths()
        {

            DapperPlusManager.Entity<EmailNotifyDocumentType>().Table("EmailNotifyDocumentType")
                            .Identity(x => x.id)
                            .Ignore(x => x.EmailNotifyDocumentTypePersons);

            DapperPlusManager.Entity<EmailNotifyDocumentTypePerson>().Table("EmailNotifyDocumentTypePerson");
            //DapperPlusManager.Entity<EmailNotifyDocumentType>().Table("EmailNotifyDocumentType")
            //                .Identity(x => x.id)
            //                .Ignore(x => x.EmailNotifyDocumentTypePersons)
            //                .AfterAction((kind, x) =>
            //                {
            //                    if (kind == DapperPlusActionKind.Insert || kind == DapperPlusActionKind.Merge)
            //                    {
            //                        x. .ForEach(r=>  r , r => r.id_EmailNotifyDocumentType = x.id);
            //                    }
            //                });
            //


            DapperPlusManager.Entity<EmailNotifyDocumentTypePerson>().ForeignKey(r => r.id_EmailNotifyDocumentType);
            

        }

    }
}