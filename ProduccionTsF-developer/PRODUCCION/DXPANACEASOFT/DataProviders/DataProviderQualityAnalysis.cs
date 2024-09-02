using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderQualityAnalysis
    {
        private static DBContext db = null;

        public static QualityAnalysis QualityAnalysis(int? id)
        {
            db = new DBContext();
            return db.QualityAnalysis.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable QualityAnalysiss(int? id_company)
        {
            db = new DBContext();
            return db.QualityAnalysis.Where(t=>t.isActive && t.id_company == id_company).ToList();
        }

        public static IEnumerable QualityControlAnalysisGroup(int ? id_company)
        {
            db = new DBContext();
            return db.QualityControlAnalysisGroup.Where(t => t.id_company == id_company).ToList();
        }
      
    }
}