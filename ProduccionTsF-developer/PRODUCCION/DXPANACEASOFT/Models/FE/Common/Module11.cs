using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.FE.Common
{
    public class Module11
    {
        public static int Get(string str)
        {
            int factor = 2;
            int cantidadTotal = 0;

            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (factor >= 8)
                    factor = 2;

                cantidadTotal += int.Parse(str[i].ToString()) * factor;
                factor++;
            }

            cantidadTotal = 11 - (cantidadTotal % 11);

            if (cantidadTotal == 11)
            {
                cantidadTotal = 0;
            }
            else if (cantidadTotal == 10)
            {
                cantidadTotal = 1;
            }

            return cantidadTotal;
        }
    }
}