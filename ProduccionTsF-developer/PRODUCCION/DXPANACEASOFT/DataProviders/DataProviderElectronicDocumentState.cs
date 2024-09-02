using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderElectronicDocumentState
    {
        private static DBContext db = null;

        public static IEnumerable ElectronicDocumentStates()
        {
            db = new DBContext();
            return db.ElectronicDocumentState.Where(s => s.isActive).OrderBy(s => s.id).ToList();
        }

        public static ElectronicDocumentState ElectronicDocumentState(int? id)
        {
            db = new DBContext();
            return db.ElectronicDocumentState.FirstOrDefault(e => e.id == id);
        }
    }
}