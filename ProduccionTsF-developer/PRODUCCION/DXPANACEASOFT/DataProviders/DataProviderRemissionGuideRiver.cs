using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderRemissionGuideRiver
    {
        private static DBContext db = null;

        public static RemissionGuideRiver RemissionGuideRiver(int? id_remissionGuideRiver)
        {
            db = new DBContext();
            return db.RemissionGuideRiver.FirstOrDefault(r => r.id == id_remissionGuideRiver);
        }

        public static RemissionGuideRiverDetail RemissionGuideRiverDetail(int? id_remissionGuideRiverDet)
        {
            db = new DBContext();
            return db.RemissionGuideRiverDetail.FirstOrDefault(r => r.id == id_remissionGuideRiverDet);
        }


   
    }
}