using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;



namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderBusinessLine
    {

        private static DBContext db = null;

        public static IEnumerable BusinessLines(int id_company)
        {
            db = new DBContext();
            var model = db.BusinessLine.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;

        }

        public static IEnumerable BusinessLineFilter(int id_company)
        {
            db = new DBContext();
            var model = db.BusinessLine.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;

        }

        public static IEnumerable BusinessLinebyCompany(int? id_company)
        {
            db = new DBContext();
            return db.BusinessLine.Where(t => t.isActive && t.id_company == id_company).Select
                ( se => new
                        {
                           se.id,
                           se.code,
                           se.name,
                           // los campos extendidos
                        } 
                
                ).ToList();
        }

        

        public static BusinessLine BusinessLinepeById(int? id)
        {
            db = new DBContext();
            return db.BusinessLine.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable BusinessLineByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.BusinessLine.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }
        public static IEnumerable BusinessLinebyCompanyClientCategoryCustomerTypeAndCurrent(int? id_company, int? id_clientCategory, int? id_customerType, int? id_businessLine)
        {
            db = new DBContext();
            return db.ClientCategoryTypeBusiness.Where(t => (t.ClientCategory.isActive && t.ClientCategory.id_company == id_company && t.id_clientCategory == id_clientCategory &&
                                                             t.CustomerType.isActive && t.CustomerType.id_company == id_company && t.id_customerType == id_customerType) ||
                                                             t.id_businessLine == id_businessLine).Select
                (se => new
                {
                    se.BusinessLine.id,
                    se.BusinessLine.code,
                    se.BusinessLine.description,
                }

                ).Distinct().ToList();
        }
        public static IEnumerable BusinessLineAll(int? id_current)
        {
            db = new DBContext();
            var CustomerTypeaux = db.BusinessLine.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

            if (id_current != null && id_current > 0)
            {
                var cant = (from de in CustomerTypeaux
                            where de.id == id_current
                            select de).ToList().Count;
                if (cant == 0)
                {
                    var CustomerTypecuuretaux = db.BusinessLine.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

                    CustomerTypeaux.AddRange(CustomerTypecuuretaux);
                }
            }

            return CustomerTypeaux.OrderBy(x => x.name);

        }
        public static int? BusinessLinebyCompanyDefault(int? id_company)
        {
            db = new DBContext();
            return db.BusinessLine.FirstOrDefault(i => i.id_company == id_company && i.byDefault)?.id;
        }

    }
}