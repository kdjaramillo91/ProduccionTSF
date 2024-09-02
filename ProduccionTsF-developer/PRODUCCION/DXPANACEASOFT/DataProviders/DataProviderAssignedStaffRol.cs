using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderAssignedStaffRol
    {
        private static DBContext db = null;

        public static RemissionGuideAssignedStaffRol AssignedStaffRol(int? id_assignedRol)
        {
            db = new DBContext();
            return db.RemissionGuideAssignedStaffRol.FirstOrDefault(w => w.id == id_assignedRol);
        }

        public static IEnumerable AssignedStaffRoles()
        {
            db = new DBContext();
            return db.RemissionGuideAssignedStaffRol.Where(w => w.isActive).ToList();
        }
    }
}