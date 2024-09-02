using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Auxiliares
{
    public static class GenericAuxMethod
    {
        private static DBContext db = new DBContext();
        public static decimal ToAdvanceDecimal(this decimal  value, string[] config, out string[] config2,  string tconfig= "FXGEN")
        {


            int numerosDecimales = 2;
            string manejoDecimales = "ROUND";
            decimal toReturn = 0;
            string[] configOut = new string[2];

            try
            {
                if(config == null ||  config.Length == 0 || config[0] ==null || config[1] == null)
                {

                    List<AdvanceParametersDetail> fexParametersConfig = db.AdvanceParameters
                                                                    .First(r => r.code == tconfig)?
                                                                    .AdvanceParametersDetail.ToList() ?? null;

                    if (fexParametersConfig != null)
                    {
                        string vnumerosDecimales = fexParametersConfig.First(r => r.valueCode.Trim() == "DEC")?.valueName ?? null;
                        if (vnumerosDecimales != null) numerosDecimales = int.Parse(vnumerosDecimales);


                        string vmanejoDecimales = fexParametersConfig.First(r => r.valueCode.Trim() == "TMD")?.valueName ?? null;
                        if (vmanejoDecimales != null) manejoDecimales = vmanejoDecimales;
                    }

                    configOut[0] = manejoDecimales;
                    configOut[1] = numerosDecimales.ToString();
                   
                    config2 = configOut;
                   
                   
                }
                else
                {
                    manejoDecimales = config[0];
                    numerosDecimales = int.Parse(config[1]);

                    config2 = config;
                }

                


                switch (manejoDecimales)
                {

                    case "ROUND":
                        toReturn = decimal.Round(value, numerosDecimales, MidpointRounding.AwayFromZero);
                        ///(decimal)(Math.Truncate(factura.subTotal * 100) / 100), //
                        break;

                    case "TRUNCATE":
                        int potence10 = 10 ^ numerosDecimales;
                        toReturn = (Math.Truncate(value * potence10) / potence10);
                        break;
                }

            }
            catch(Exception e)
            {
                throw e;
                // escribir en log

            }
            

            return toReturn;

        }
    }
}