using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace DXPANACEASOFT.Models.GenericProcess
{
    internal static class DocumentOperationExtension
    {


        internal static IList<Document> FindCodeType(this DbSet<Document> thisObj,string codeDocumentType)
        {

            IList<Document>  resultDocument = thisObj
                                                //.AsParallel()
                                                .Where(r => r.DocumentType.code == codeDocumentType).ToList();

            return resultDocument;
        }


    }
}