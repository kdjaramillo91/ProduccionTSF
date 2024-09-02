namespace DXPANACEASOFT.Models.FE.Xmls.Common
{
    public static class AccessKey
    {
        public static string GenerateAccessKey(string emissionDate, 
                                               string documentTypeCode, 
                                               string ruc, 
                                               string enviromentCode, 
                                               string serie,
                                               string documentNumber,
                                               string numericCode,
                                               string emissionType)
        {
            object [] args =
            {
                emissionDate,
                documentTypeCode,
                ruc,
                enviromentCode,
                serie,
                documentNumber,
                numericCode,
                emissionType
            };
            string accessKey = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", args);
            accessKey += Module11.Get(accessKey);

            return accessKey;
        }

        public static string GenerateAccessKey(string emissionDate, 
                                               int documentTypeCode, 
                                               string ruc, 
                                               int enviromentCode, 
                                               int divisionCode, 
                                               int emissionPointCode, 
                                               int documentNumber, 
                                               int numericCode, 
                                               int emissionTypeCode)
        {
            return AccessKey.GenerateAccessKey(emissionDate,
                                               documentTypeCode.ToString("D2"),
                                               ruc,
                                               enviromentCode.ToString(),
                                               divisionCode.ToString("D3") + emissionPointCode.ToString("D3"),
                                               documentNumber.ToString("D9"),
                                               numericCode.ToString("D8"),
                                               emissionTypeCode.ToString()
                                              );
        }
    }
}