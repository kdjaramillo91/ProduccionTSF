using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models.AdvanceProviderDTO;
using Utilitarios.Logs;
using System.Configuration;



namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderJulianoNumber
    {
        //private static DBContext db = null;

        public static string GetJulianoNumber(DateTime dt)
        {
            string resultado = "";
            int year = dt.Date.Year;


            DateTime dtDateNow = dt;
            DateTime dtDateBeginYear = new DateTime(year, 01, 01, 0,0,0);

            TimeSpan days = dtDateNow.Subtract(dtDateBeginYear);

            int da = days.Days + 1;
            string daysStr = da.ToString().PadLeft(3,'0');
            string yearStr = year.ToString().Substring(2,2);
            resultado = yearStr + daysStr;

            return resultado;
        }
    }
}



