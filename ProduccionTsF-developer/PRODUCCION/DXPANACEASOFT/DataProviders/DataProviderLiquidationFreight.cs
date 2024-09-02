using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
   

    public static class DataProviderLiquidationFreight
    {
        private static DBContext db = null;

        public static LiquidationFreight LiquidationFreight(int? id_LiquidationFreight)
        {
            db = new DBContext();
            return db.LiquidationFreight.FirstOrDefault(r => r.id == id_LiquidationFreight);
        }

        public static LiquidationFreightDetail LiquidationFreightDetail(int? id_LiquidationFreightDet)
        {
            db = new DBContext();
            return db.LiquidationFreightDetail.FirstOrDefault(r => r.id == id_LiquidationFreightDet);
        }

     
    }
}