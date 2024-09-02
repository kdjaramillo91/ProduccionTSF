using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.FE.Common
{
    public static class AccessKey
    {
        public static string GenerateAccessKey(string emissionDate, 
                                               string documentType, 
                                               string ruc, 
                                               string ambient, 
                                               string serie,
                                               string documentNumber,
                                               string numericCode,
                                               string emissionType)
        {
            object [] args =
            {
                emissionDate,
                documentType,
                ruc,
                ambient,
                serie,
                documentNumber,
                numericCode,
                emissionType
            };
            string accessKey = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", args);
            accessKey += Module11.Get(accessKey);

            return accessKey;
        }
    }
}