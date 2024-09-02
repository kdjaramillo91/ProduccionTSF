using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderBusinessOportunityPartner
    {
        private static DBContext db = null;

        public static BusinessOportunityPartner BusinessOportunityPartnerById(int? id_businessOportunityPartner)
        {
            db = new DBContext(); 
            return db.BusinessOportunityPartner.FirstOrDefault(v => v.id == id_businessOportunityPartner);
        }

        public static BusinessOportunityPartner BusinessOportunityPartnerByIdFilter(int? id_businessOportunityPartner)
        {
            db = new DBContext(); 
            return db.BusinessOportunityPartner.FirstOrDefault(v => v.id == id_businessOportunityPartner);
        }

    }
}