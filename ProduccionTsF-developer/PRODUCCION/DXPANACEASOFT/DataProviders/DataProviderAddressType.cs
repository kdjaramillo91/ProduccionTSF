using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderAddressType
    {
        private static DBContext db = null;

        public static IEnumerable AddressTypeByCompany(int? id_company)
        {
            db = new DBContext();
            var model = db.AddressType.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static AddressType AddressTypeById(int? id_addressType)
        {
            db = new DBContext();
            AddressType model = db.AddressType.FirstOrDefault(t => t.id == id_addressType);

            
            return model;
        }


    }
}