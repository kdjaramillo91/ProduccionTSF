using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
   

    public static class DataProviderLiquidationFreightRiver
    {
        private static DBContext db = null;

        public static LiquidationFreightRiver LiquidationFreightRiver(int? id_LiquidationFreightRiver)
        {
            db = new DBContext();
            return db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id_LiquidationFreightRiver);
        }

        public static LiquidationFreightRiverDetail LiquidationFreightRiverDetail(int? id_LiquidationFreightRiverDet)
        {
            db = new DBContext();
            return db.LiquidationFreightRiverDetail.FirstOrDefault(r => r.id == id_LiquidationFreightRiverDet);
        }

     
    }
}