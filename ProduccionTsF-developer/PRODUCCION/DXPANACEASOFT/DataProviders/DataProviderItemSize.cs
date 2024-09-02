using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderItemSize
    {
        private static DBContext db = null;

        public static IEnumerable ItemSizes()
        {
            db = new DBContext();
            return db.ItemSize
                .Where(t=>t.isActive)
                .OrderBy(o => o.orderSize)
                .Select(e => new {
                    e.id,
                    e.name
                })
                .ToList()
                .Select(e => new ItemSize() 
                {
                    id = e.id,
                    name = e.name,
                })
                .ToList();
        }

        public static IEnumerable ItemSizebyCompany(int? id_company)
        {
            db = new DBContext();
            return db.ItemSize.Where(t => t.isActive && t.id_company == id_company).ToList();
        }

        public static ItemSize ItemSize(int id)
        {
            db = new DBContext();
            return db.ItemSize.FirstOrDefault(i => i.id == id);
        }

        public static ItemSize ItemSizeById(int? id)
        {
            db = new DBContext();
            var query = db.ItemSize.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static Class ClassItemById(int? id)
        {
            db = new DBContext();
            var query = db.Class.FirstOrDefault(fod => fod.id == id);
            return query;
        }

        public static IEnumerable ItemSizebyProcessTypeAndCurrent(int? id_processType, int? id_size)
        {
            db = new DBContext();
            return db.ItemSize.Where(t => t.id == id_size || (t.isActive && t.ProcessType.id == id_processType))
                .Select(s => new { s.id, s.name })
                .ToList()
                .Select(e => new ItemSize()
                {
                    id = e.id,
                    name = e.name,
                })
                .ToList();
        }

        public static IEnumerable ItemSizebyProcessTypeAndCurrentCOL(int? id_processType, int? id_size)
        {
            db = new DBContext();
            var aCodProcessType = db.ProcessType.FirstOrDefault(fod=> fod.id == id_processType)?.code;
            var aIdProcessTypeCOL = db.ProcessType.FirstOrDefault(fod=> fod.code == "COL")?.id;
            return db.ItemSize.Where(t => t.id == id_size || (t.isActive && t.ProcessType.id == id_processType) || (t.isActive && aCodProcessType == "ENC" && t.ProcessType.id == aIdProcessTypeCOL))
                .Select(s => new { s.id, s.name })
                .ToList()
                .Select(e => new ItemSize()
                {
                    id = e.id,
                    name = e.name,
                })
                .ToList();
        }
    }
}