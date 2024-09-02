using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;



namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderCustomerType
    {

        private static DBContext db = null;

        public static IEnumerable CustomerTypes(int id_company)
        {
            db = new DBContext();
            var model = db.CustomerType.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;

        }

        public static IEnumerable CustomerTypesFilter(int id_company)
        {
            db = new DBContext();
            var model = db.CustomerType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;

        }

        public static IEnumerable CustomerTypebyCompany(int? id_company)
        {
            db = new DBContext();
            return db.CustomerType.Where(t => t.isActive && t.id_company == id_company).Select
                ( se => new
                        {
                           se.id,
                           se.code,
                           se.name,
                           se.def_id_paymentMethod,
                           se.def_id_paymentTerm
                           // los campos extendidos
                        } 
                
                ).ToList();
        }

        

        public static CustomerType CustomerTypeById(int? id)
        {
            db = new DBContext();
            return db.CustomerType.FirstOrDefault(i => i.id == id);
        }

        //public static IEnumerable CategoryByCompanyAndCurrent(int? id_company, int? id_current)
        //{
        //    db = new DBContext();
        //    return db.Category.Where(g => (g.isActive && g.id_company == id_company) ||
        //                                         g.id == (id_current == null ? 0 : id_current)).ToList();
        //}

        public static IEnumerable CustomerTypeAll(int? id_current)
        {
            db = new DBContext();
            var CustomerTypeaux = db.CustomerType.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

            if (id_current != null && id_current > 0)
            {
                var cant = (from de in CustomerTypeaux
                            where de.id == id_current
                            select de).ToList().Count;
                if (cant == 0)
                {
                    var CustomerTypecuuretaux = db.CustomerType.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

                    CustomerTypeaux.AddRange(CustomerTypecuuretaux);
                }
            }

            return CustomerTypeaux.OrderBy(x => x.name);

        }
        public static IEnumerable CustomerTypebyCompanyClientCategoryAndCurrent(int? id_company, int? id_clientCategory, int? id_customerType)
        {
            db = new DBContext();
            return db.ClientCategoryTypeBusiness.Where(t => (t.ClientCategory.isActive && t.ClientCategory.id_company == id_company && t.id_clientCategory == id_clientCategory) ||
                                                             t.id_customerType == id_customerType).Select
                (se => new
                {
                    se.CustomerType.id,
                    se.CustomerType.code,
                    se.CustomerType.name,
                    se.CustomerType.def_id_paymentMethod,
                    se.CustomerType.def_id_paymentTerm
                    // los campos extendidos
                }

                ).Distinct().ToList();
        }

        public static int? CustomerTypebyCompanyDefault(int? id_company)
        {
            db = new DBContext();
            return db.CustomerType.FirstOrDefault(i => i.id_company == id_company && i.byDefault)?.id;
        }
    }
}