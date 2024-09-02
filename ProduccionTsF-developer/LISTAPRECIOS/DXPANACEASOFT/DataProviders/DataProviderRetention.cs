using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderRetention
    {
        private static DBContext db = null;

        public static Retention Retention(int id)
        {
            db = new DBContext();
            return db.Retention.FirstOrDefault(i => i.id == id);
        }

        public static Retention RetentionById(int? id)
        {
            db = new DBContext();
            return db.Retention.FirstOrDefault(i => i.id == id);
        }
        public static IEnumerable Retentions()
        {
            db = new DBContext();
            return db.Retention.ToList();
        }

        public static IEnumerable RtInternationalWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.RtInternational.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable RetentionSeriesForDocumentsTypes()
        {
            db = new DBContext();
            return db.RetentionSeriesForDocumentsType.Where(g => (g.isActive)).ToList();
        }

        public static IEnumerable AllRetentionSeriesForDocumentsTypes()
        {
            db = new DBContext();
            return db.RetentionSeriesForDocumentsType.ToList();
        }

        public static RetentionSeriesForDocumentsType RetentionSeriesForDocumentsTypeById(int? id)
        {
            db = new DBContext();
            return db.RetentionSeriesForDocumentsType.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable RetentionTypes()
        {
            db = new DBContext();
            return db.RetentionType.Where(g => (g.isActive)).ToList();
        }

        public static IEnumerable AllRetentionTypes()
        {
            db = new DBContext();
            return db.RetentionType.ToList();
        }

        public static RetentionType RetentionTypeById(int? id)
        {
            db = new DBContext();
            return db.RetentionType.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable RetentionGroups()
        {
            db = new DBContext();
            return db.RetentionGroup.Where(g => (g.isActive)).ToList();
        }

        public static IEnumerable AllRetentionGroups()
        {
            db = new DBContext();
            return db.RetentionGroup.ToList();
        }

        public static RetentionGroup RetentionGroupById(int? id)
        {
            db = new DBContext();
            return db.RetentionGroup.FirstOrDefault(i => i.id == id);
        }
    }
}