using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderPoundType
    {
        private static DBContext db = null;

        public static IEnumerable PoundsTypes(int? id_company)
        {
            db = new DBContext();
            var model = db.SettingDetail.ToList();

            if (id_company != 0)
            {
                model = model.Where(w => w.Setting.code == "TYPOU").ToList();
            }

            return model;

        }

    }
}