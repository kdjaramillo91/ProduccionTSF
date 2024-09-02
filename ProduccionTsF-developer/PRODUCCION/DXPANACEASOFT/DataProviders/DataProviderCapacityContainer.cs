using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderCapacityContainer
    {

        private static DBContext db = null;

        public static IEnumerable CapacityContainerAll()
        {

            db = new DBContext();
            return db.CapacityContainer.Where(r => r.isActive).ToList();

        }
        public static IEnumerable AllCapacityContainer()
        {
            db = new DBContext();
            var model = db.CapacityContainer.ToList();

            return model;
        }

        public static CapacityContainer CapacityContainerById(int? id_capacityContainer)
        {
            db = new DBContext(); ;
            return db.CapacityContainer.FirstOrDefault(v => v.id == id_capacityContainer);
        }

        public static IEnumerable CapacityContainerWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.CapacityContainer.Where(g => (g.isActive) || g.id == id_current).ToList();
        }
    }
}