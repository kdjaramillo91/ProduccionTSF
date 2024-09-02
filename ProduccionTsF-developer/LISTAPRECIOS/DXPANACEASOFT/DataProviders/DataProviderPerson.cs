using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPerson
    {
        private static DBContext db = null;

        public static IEnumerable Persons(int id_company)
        {
            db = new DBContext();
            var model = db.Person.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable PersonsByCompany(int? id_company)
        {
            db = new DBContext();
            var model = db.Person.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }
        public static IEnumerable AllPersonsByCompany(int? id_company)
        {
            db = new DBContext();
            var model = db.Person.Where(p => p.id_company == id_company).ToList();

            return model;
        }
        public static IEnumerable ProvidersAndCustomers(int? id_company)
        {
            db = new DBContext();
            var model = db.Person.Where(t => (t.Provider != null ) && t.isActive).ToList();

            if (id_company != 0 && id_company != null)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static IEnumerable AllRequestingPersonsByCompany(int? id_company)
        {
            db = new DBContext();

            return db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Solicitante")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();

        }

        public static IEnumerable AllReceivingPersonsByCompany(int? id_company)
        {
            db = new DBContext();

            return db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Recibidor")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();

        }
        public static Person PersonById(int? id_partner)
        {
            db = new DBContext(); ;
            return db.Person.FirstOrDefault(v => v.id == id_partner);
        }

        public static Person PersonByIdFilter(int? id_partner)
        {
            db = new DBContext(); ;
            return db.Person.FirstOrDefault(v => v.id == id_partner);
        }

        public static IEnumerable PersonByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.Person.Where(g => ( g.isActive && g.id_company == id_company) || 
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        public static IEnumerable ProviderByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.Provider.Where(g => (g.Person.isActive && g.Person.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).Select(p=> new { p.id, name = p.Person.fullname_businessName}).ToList();
        }



        public static Person Person(int? id)
        {
            db = new DBContext();
            return db.Person.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable Providers()
        {
            db = new DBContext();
            return db.Person.Where(p => p.Provider != null && p.isActive).ToList();
        }

        public static IEnumerable AllProvidersByCompany(int? id_company)
        {
            db = new DBContext();

            return db.Provider.Where(s => s.Person.id_company == id_company).Select(s => new { id = s.id, name = s.Person.fullname_businessName }).ToList();

        }

        public static IEnumerable ProviderByCompany(int? id_company)
        {
            db = new DBContext();
            return db.Provider.Where(g => (g.Person.isActive && g.Person.id_company == id_company)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
        }

        public static IEnumerable AllProviderByCompany(int? id_company)
        {
            db = new DBContext();
            return db.Provider.Where(g => (g.Person.id_company == id_company)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
        }


        public static IEnumerable Employees()
        {
            db = new DBContext();
            return db.Person.Where(p => p.Employee != null && p.isActive).ToList();
        }

        public static IEnumerable EmployeesbyCompany(int? id_company)
        {
            db = new DBContext();
            var model = db.Person.Where(t => (t.Employee != null) && t.isActive).ToList();

            if (id_company != 0 && id_company != null)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static IEnumerable EmployesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.Person.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }
        public static IEnumerable Driver()
        {
            db = new DBContext();
            return db.Person.Where(p =>  p.isActive && p.Rol.Any(a => a.name == "Chofer")).ToList();
        }


        public static IEnumerable AllBuyersByCompany(int? id_company)
        {
            db = new DBContext();

            return db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Comprador")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();

        }


        public static IEnumerable RolsByCompanyAndCurrent(int? id_company, int? id_current, string rol)
        {
            db = new DBContext();
            return db.Person.Where(g => (g.isActive && g.id_company == id_company && g.Rol.Any(a => a.name == rol)) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        public static IEnumerable RolsByCompany(int? id_company, string rol)
        {
            db = new DBContext();
            return db.Person.Where(g => (g.isActive && g.id_company == id_company && g.Rol.Any(a => a.name == rol))).ToList();
        }

        public static IEnumerable RolsByCompanyAndCurrent(int? id_company, int? id_current, List<string> rols)
        {
            db = new DBContext();
            var personsAux = db.Person.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();

            if(rols !=null && rols.Count>0)
            {
                var lisid = new List<int>();


                
                var lisrol = db.Rol.Where(x => rols.Contains(x.name));

                var fr = (from e in lisrol.Select(x => x.Person).ToList()
                          select new { id = e.Select(x => x.id) }).ToList();

                foreach ( var e  in fr)
                {
                    var listrola = from d in e.id
                                   select d;
                   

                   lisid.AddRange(listrola);

                }



                // var lisrol = from e in db.Rol.Where(x => rols.Contains(x.name))



                id_current = id_current ?? 0;



                personsAux = personsAux.Where(g => lisid.Contains(g.id)  ||
                                             g.id == id_current).ToList();


                    
                       
               

            }

            //foreach (var rol in rols)
            // {



            //personsAux = personsAux.Where(g => (g.Rol.Any(a => a.name == rol)) ||
            //                                 g.id == (id_current == null ? 0 : id_current)).ToList();
            // }
            return personsAux;
        }



        public static Provider Provider(int? id)
        {
            db = new DBContext();
            return db.Provider.FirstOrDefault(p => p.id == id);
        }

        public static Employee Employee(int? id)
        {
            db = new DBContext();
            return db.Employee.FirstOrDefault(p => p.id == id);
        }

        public static IEnumerable ProviderTypeWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.ProviderType.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable EconomicGroupWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.EconomicGroup.Where(w => (w.isActive) || w.id == id_current).ToList();
        }
        public static IEnumerable OriginWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.Origin.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable PersonByCompanyDocumentTypeOportunityAndCurrent(int? id_company, string codeDocumentTypeOportunity , int? id_current)
        {
            db = new DBContext();
            var personAux =  db.Person.Where(g => ((g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)) &&
                                                 (codeDocumentTypeOportunity == "15" ?(
                                                 g.id == (id_current == null ? 0 : id_current)):
                                                 (codeDocumentTypeOportunity == "16"? ((g.Provider != null) ||
                                                 g.id == (id_current == null ? 0 : id_current)): false))).ToList();
            //if(codeDocumentTypeOportunity == "15")
            //{
            //    personAux = personAux.Where(w=> (w.Customer != null) ||
            //                                     w.id == (id_current == null ? 0 : id_current)).ToList();
            //}
            //else
            //{
            //    if (codeDocumentTypeOportunity == "16")
            //    {
            //        personAux = personAux.Where(w => (w.Provider != null) ||
            //                                         w.id == (id_current == null ? 0 : id_current)).ToList();
            //    }
            //}
            return personAux;
        }

       

        public static IEnumerable PersonsTransportistDriver(int? id_company)
        {
            db = new DBContext();

            var model = db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Transportista")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();

            //var model2= db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Transportista")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();

            return model;
        }

        public static IEnumerable PersonsTransportist(int? id_company)
        {
            db = new DBContext();

            var model = db.ProviderGeneralData.Where(w => w.ProviderType != null && w.ProviderType.isTransportist == true).Select(s => new { id = s.id_provider, name = s.Provider.Person.fullname_businessName }).ToList();

            //var model2 = db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Transportista")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();


            return model;
        }
    }
}