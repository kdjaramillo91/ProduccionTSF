using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTController
    {
        private static DBContext db = null;

        public static IEnumerable Tcontroller()
        {
            db = new DBContext(); ;
            return db.TController.Where(v => v.isActive).ToList();
        }

        public static TController TControllerById(int? id_tcontroller)
        {
            db = new DBContext(); ;
            return db.TController.FirstOrDefault(v => v.id == id_tcontroller);

        }

    
        //public static TController TControllerById(int? id_tcontroller)
        //{
        //    db = new DBContext(); ;
        //    return db.TController.FirstOrDefault(v => v.id == id_tcontroller && v.isActive);
        //}


    }
}