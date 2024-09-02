using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderCertification
    {
        private static DBContext db = null;

        public static IEnumerable Certifications(/*int id_company*/)
        {
            db = new DBContext();
            var model = db.Certification.Where(t => t.isActive).ToList();

            //if (id_company != 0)
            //{
            //    model = model.Where(p => /*p.id_company == id_company && */p.isActive).ToList();
            //}

            return model;

        }

        public static IEnumerable CertificationsFilter(/*int id_company*/)
        {
            db = new DBContext();
            var model = db.Certification.ToList();

            //if (id_company != 0)
            //{
            //    model = model.Where(p => p.id_company == id_company).ToList();
            //}

            return model;

        }

        public static IEnumerable CertificationsByCompany(/*int? id_company*/)
        {
            db = new DBContext();
            var query = db.Certification.Where(t =>/* t.id_company == id_company &&*/ t.isActive);
            return query.ToList();
        }

        public static Certification CertificationById(int? id)
        {
            db = new DBContext();
            var query = db.Certification.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable CertificationsByCompanyAndCurrent(/*int? id_company,*/ int? id_current)
        {
            db = new DBContext();
            return db.Certification.Where(g => (g.isActive /*&& g.id_company == id_company*/) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }
    }
}