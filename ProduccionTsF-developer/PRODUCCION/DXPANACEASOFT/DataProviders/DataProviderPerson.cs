using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.PersonP.PersonModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderPerson
	{
		protected static DBContext dba = new DBContext();
		public static IEnumerable Persons(int id_company)
		{
			var db = new DBContext();
			var model = db.Person.Where(t => t.isActive).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
			}

			return model;
		}

		public static IEnumerable PersonsByCompany(int? id_company)
		{
			var db = new DBContext();
			var model = db.Person.Where(t => t.isActive).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company).ToList();
			}

			return model;
		}

		public static IEnumerable AllPersonsByCompany(int? id_company)
		{
			var db = new DBContext();
			return db.Person.Where(p => p.id_company == id_company).ToList();
		}

		public static IEnumerable ProvidersAndCustomers(int? id_company)
		{
			var db = new DBContext();
			var model = db.Person.Where(t => (t.Provider != null || t.Customer != null) && t.isActive).ToList();

			if (id_company != 0 && id_company != null)
			{
				model = model.Where(p => p.id_company == id_company).ToList();
			}

			return model;
		}

		public static IEnumerable AllRequestingPersonsByCompany(int? id_company)
		{
			var db = new DBContext();
			return db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Solicitante")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();
		}

		public static IEnumerable AllReceivingPersonsByCompany(int? id_company)
		{
			var db = new DBContext();
			return db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Recibidor")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();
		}

		public static IEnumerable AllBuyerPersonsByCompany(int? id_company, int? id_buyer)
		{
			var db = new DBContext();
			return db.Person.Where(s => (s.isActive && s.id_company == id_company && s.Rol.Any(a => a.name == "Comprador")) ||
										 s.id == (id_buyer == null ? 0 : id_buyer)).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();
		}

		public static Person PersonById(int? id_partner)
		{
			var db = new DBContext(); ;
			return db.Person.FirstOrDefault(v => v.id == id_partner);
		}

		public static Person PersonByIdFilter(int? id_partner)
		{
			var db = new DBContext();
			return db.Person.FirstOrDefault(v => v.id == id_partner);
		}

		public static IEnumerable PersonByCompanyAndCurrent(int? id_company, int? id_current)
		{
			var db = new DBContext();
			return db.Person.Where(g => (g.isActive && g.id_company == id_company) ||
									g.id == (id_current == null ? 0 : id_current)).ToList();
		}

		public static IEnumerable ProviderByCompanyAndCurrent(int? id_company, int? id_current)
		{
			var db = new DBContext();
			return db.Provider.Where(g => (g.Person.isActive && g.Person.id_company == id_company && g.ProviderGeneralData.ProviderType.isShrimpPerson) ||
										 g.id == (id_current == null ? 0 : id_current)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
		}

		public static IEnumerable CustomerByCompanyAndCurrent(int? id_company, int? id_current)
		{
			var db = new DBContext();
			return db.Customer.Where(g => (g.Person.isActive && g.Person.id_company == id_company) ||
										 g.id == (id_current == null ? 0 : id_current)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
		}

		public static Person Person(int? id)
		{
			var db = new DBContext();
			return db.Person.FirstOrDefault(i => i.id == id);
		}

		public static IEnumerable Providers()
		{
			var db = new DBContext();
			return db.Person.Where(p => p.Provider != null && p.isActive).ToList();
		}

		public static IEnumerable ProvidersShrimpPerson()
		{
			var db = new DBContext();
			return db.Provider.Where(w => w.ProviderGeneralData.ProviderType.isShrimpPerson && w.Person.isActive).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
		}

		public static IEnumerable AllProvidersByCompany(int? id_company)
		{
			var db = new DBContext();
			return db.Provider.Where(s => s.Person.id_company == id_company).Select(s => new { id = s.id, name = s.Person.fullname_businessName }).ToList();
		}

		public static IEnumerable ProviderByCompany(int? id_company)
		{
			var db = new DBContext();
			return db.Provider.Where(g => (g.Person.isActive && g.Person.id_company == id_company)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
		}

        public static IEnumerable ProviderByCompanyRolProviderAndProviderType(int? id_company, string rol, string nameProviderType)
        {
            var db = new DBContext();
            return db.Provider.Where(g => (g.Person.Rol.FirstOrDefault(fod => fod.name == rol) != null &&
                                           g.ProviderGeneralData.ProviderType.name == nameProviderType &&
                                           g.Person.id_company == id_company)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
        }

        public static IEnumerable AllProviderByCompany(int? id_company)
		{
			var db = new DBContext();
			return db.Provider.Where(g => (g.Person.id_company == id_company)).Select(p => new { p.id, name = p.Person.fullname_businessName }).ToList();
		}

		public static IEnumerable ProvidersByItem(int id_item)
		{
			var db = new DBContext();
			return (from prov in db.Provider
					join itemProv in db.ItemProvider
						on prov.id equals itemProv.id_provider into providers
					from prov2 in providers
					where prov2.id_item == id_item
					select new { id = prov.Person.id, fullname_businessName = prov.Person.fullname_businessName }).ToList();
		}

		public static IEnumerable Employees()
		{
			var db = new DBContext();
			return db.Person
				.Where(p => p.Employee != null && p.isActive)
				.Select(e => new {
					e.id,
					e.fullname_businessName,
				})
				.ToList()
				.Select(e => new Person()
				{
					id = e.id,
					fullname_businessName = e.fullname_businessName,
				})
				.ToList();
		}

		public static IEnumerable EmployeesbyCompany(int? id_company)
		{
			var db = new DBContext();
			var model = db.Person.Where(t => (t.Employee != null) && t.isActive).ToList();

			if (id_company != 0 && id_company != null)
			{
				model = model.Where(p => p.id_company == id_company).ToList();
			}

			return model;
		}

		public static IEnumerable EmployesByCompanyAndCurrent(int? id_company, int? id_current)
		{
			var db = new DBContext();
			return db.Person.Where(g => (g.isActive && g.id_company == id_company) ||
										 g.id == (id_current == null ? 0 : id_current)).ToList();
		}

		public static IEnumerable CustomersExteriorByCompany(int? id_company)
		{
			var db = new DBContext();
			var personas = db.Person
				.Where(p => p.ForeignCustomer != null && p.isActive)
				.Select(e => new 
				{ 
					e.id_company,
					e.id,
					e.fullname_businessName,
					e.identification_number,
				})
				.ToList()
				.Select(e => new Person()
				{
					id = e.id,
					fullname_businessName = e.fullname_businessName,
					identification_number = e.identification_number,
					id_company = e.id_company,
				})
				.ToList();

            if (id_company.HasValue)
            {
				personas = personas.Where(e => e.id_company == id_company.Value).ToList();
			}

			return personas;
		}

		public static IEnumerable AllPersonsLocalByCompany(int? id_company)
		{
			var db = new DBContext();
			return db.Person.Where(s => (s.isActive && s.id_company == id_company && s.Rol.Any(a => a.name == "Cliente Local")))
							.Select(s => new { s.id, name = s.fullname_businessName }).ToList();
		}

        public static IEnumerable CustomersExteriorAndAllPersonsLocalByCompany(int? id_company)
        {
            var db = new DBContext();
            return db.Person.Where(p => (p.ForeignCustomer != null && p.isActive && p.id_company == id_company) ||
                                        (p.isActive && p.id_company == id_company && p.Rol.Any(a => a.name == "Cliente Local")))
                                        .Select(s => new { s.id, name = s.fullname_businessName }).ToList();
        }

        public static IEnumerable BuyersByCompanyAndCurrent(int? id_company, int? id_current, int? id_user = 0)
		{
			var db = new DBContext();
			tbsysUserRecordSecurity userRecordSecurity = null;

			if (id_user != 0 && id_user != null)
			{
				userRecordSecurity = db.tbsysUserRecordSecurity.FirstOrDefault(r => r.id_user == id_user && r.tbsysObjSecurityRecord.obj == "PurchaseOrder");
			}

			var lstBuyers = db.Person.Where(g => (g.isActive && g.id_company == id_company && g.Rol.Any(a => a.name == "Comprador")) ||
												 g.id == (id_current == null ? 0 : id_current)).ToList();
			if (userRecordSecurity != null)
			{
				var id_employee = db.User.FirstOrDefault(fod => fod.id == id_user)?.id_employee ?? 0;
				if (db.Person.FirstOrDefault(fod => fod.Rol.Any(a => a.name == "Comprador") && fod.id == id_employee) != null)
				{
					lstBuyers = lstBuyers.Where(w => w.id == id_employee).ToList();
				}

			}

			return lstBuyers;
		}

		public static IEnumerable liquidatorsByCompanyAndCurrent(int? id_company, int? id_current, int? id_user = 0)
		{
			var db = new DBContext();
			tbsysUserRecordSecurity userRecordSecurity = null;

			if (id_user != 0 && id_user != null)
			{
				userRecordSecurity = db.tbsysUserRecordSecurity.FirstOrDefault(r => r.id_user == id_user && r.tbsysObjSecurityRecord.obj == "LiquidationCartOnCart");
			}

			var lstBuyers = db.Person.Where(g => (g.isActive && g.id_company == id_company && g.Rol.Any(a => a.name == "Liquidador")) ||
												 g.id == (id_current == null ? 0 : id_current)).ToList();
			if (userRecordSecurity != null)
			{
				var id_employee = db.User.FirstOrDefault(fod => fod.id == id_user)?.id_employee ?? 0;
				if (db.Person.FirstOrDefault(fod => fod.Rol.Any(a => a.name == "Liquidador") && fod.id == id_employee) != null)
				{
					lstBuyers = lstBuyers.Where(w => w.id == id_employee).ToList();
				}

			}

			return lstBuyers;
		}

		public static IEnumerable Driver()
		{
			var db = new DBContext();
			return db.Person.Where(p => p.isActive && p.Rol.Any(a => a.name == "Chofer")).ToList();
		}

		public static IEnumerable IceProvider()
		{
			var db = new DBContext();
			return db.Person.Where(p => p.isActive && p.Rol.Any(a => a.name == "Proveedor")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();
		}

		public static IEnumerable AllBuyersByCompany(int? id_company)
		{
			var db = new DBContext();
			return db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Comprador")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();
		}

		public static IEnumerable RolsByCompanyAndCurrent(int? id_company, int? id_current, string rol)
		{
			var db = new DBContext();
			return db.Person.Where(g => (g.isActive && g.id_company == id_company && g.Rol.Any(a => a.name == rol)) ||
												 g.id == (id_current == null ? 0 : id_current)).ToList();
		}

		public static IEnumerable RolsByCompany(int? id_company, string rol)
		{
			var db = new DBContext();
			return db.Person.Where(g => (g.isActive && g.id_company == id_company && g.Rol.Any(a => a.name == rol))).ToList();
		}

		public static IEnumerable RolsByCompanyAndCurrent(int? id_company, int? id_current, List<string> rols)
		{
			var db = new DBContext();
			var personsAux = db.Person.Where(g => (g.isActive && g.id_company == id_company) ||
												 g.id == (id_current == null ? 0 : id_current)).AsEnumerable();

			if (rols != null && rols.Count > 0)
			{
				var lisid = new List<int>();



				var lisrol = db.Rol.Where(x => rols.Contains(x.name));

				var fr = (from e in lisrol.Select(x => x.Person).ToList()
						  select new { id = e.Select(x => x.id) }).ToList();

				foreach (var e in fr)
				{
					var listrola = from d in e.id
								   select d;


					lisid.AddRange(listrola);

				}

				id_current = id_current ?? 0;

				personsAux = personsAux.Where(g => lisid.Contains(g.id) ||
											 g.id == id_current);
			}

			return personsAux.ToList();
		}

		public static IEnumerable Customers()
		{
			var db = new DBContext();
            return db.Person
				.Where(p => p.Customer != null && p.isActive)
				.Select(e => new {
					e.id,
					e.fullname_businessName,
				})
				.ToList()
				.Select(e => new Person() 
				{ 
					id = e.id,
					fullname_businessName = e.fullname_businessName,
					isActive = true,
				})
				.ToList();
		}

		public static Provider Provider(int? id)
		{
			var db = new DBContext();
			return db.Provider.FirstOrDefault(p => p.id == id);
		}

		public static Provider GetProviderFromRemissionGuide(int? id)
		{
			var db = new DBContext();
			return db.RemissionGuide.FirstOrDefault(p => p.id == id)?.RemissionGuideDetail?.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail?.PurchaseOrder?.Provider;
		}
		public static Person GetProviderTransportBilling(int? id)
		{
			var db = new DBContext();
			return db.VehicleProviderTransportBilling.FirstOrDefault(p => p.id == id)?.Person;
		}

		public static Employee Employee(int? id)
		{
			var db = new DBContext();
			return db.Employee.FirstOrDefault(p => p.id == id);
		}

		public static IEnumerable ProviderTypeWithCurrent(int? id_current)
		{
			var db = new DBContext();
			return db.ProviderType.Where(g => (g.isActive) || g.id == id_current).ToList();
		}

		public static IEnumerable EconomicGroupWithCurrent(int? id_current)
		{
			var db = new DBContext();
			return db.EconomicGroup.Where(w => (w.isActive) || w.id == id_current).ToList();
		}
		public static IEnumerable OriginWithCurrent(int? id_current)
		{
			var db = new DBContext();
			return db.Origin.Where(g => (g.isActive) || g.id == id_current).ToList();
		}

		public static IEnumerable PersonByCompanyDocumentTypeOportunityAndCurrent(int? id_company, string codeDocumentTypeOportunity, int? id_current)
		{
			var db = new DBContext();
			return db.Person.Where(g => ((g.isActive && g.id_company == id_company) ||
									g.id == (id_current == null ? 0 : id_current)) &&
									(codeDocumentTypeOportunity == "15" ? ((g.Customer != null) ||
									g.id == (id_current == null ? 0 : id_current)) :
									(codeDocumentTypeOportunity == "16" ? ((g.Provider != null) ||
									g.id == (id_current == null ? 0 : id_current)) : false))).ToList();
		}

		public static IEnumerable PersonByCompanyDocumentTypeOportunityAndCurrentDistinctInBusinessOportunityPartner(int? id_company, string codeDocumentTypeOportunity, int? id_current, List<BusinessOportunityPartner> businessOportunityPartner)
		{
			var db = new DBContext();
			var personAux = db.Person.Where(g => ((g.isActive && g.id_company == id_company) ||
												g.id == (id_current == null ? 0 : id_current)) &&
												(codeDocumentTypeOportunity == "15" ? ((g.Customer != null) ||
												g.id == (id_current == null ? 0 : id_current)) :
												(codeDocumentTypeOportunity == "16" ? ((g.Provider != null) ||
												g.id == (id_current == null ? 0 : id_current)) : false))).ToList();

			var tempPersons = new List<Person>();
			foreach (var p in personAux)
			{
				if (!(businessOportunityPartner.Any(a => a.id_partner == p.id)) || p.id == id_current)
				{
					tempPersons.Add(p);
				}

			}
			personAux = tempPersons;
			return personAux;
		}

		public static IEnumerable RolByCompanyAndCurrentDistinctInBusinessOportunityCompetition(int? id_company, string codeDocumentTypeOportunity, int? id_current, string rol, List<BusinessOportunityCompetition> businessOportunityCompetition)
		{
			var db = new DBContext();
			var personAux = db.Person.Where(g => (g.isActive && g.id_company == id_company && g.Rol.Any(a => a.name == rol)) ||
												 g.id == (id_current == null ? 0 : id_current)).ToList();
			var tempPersons = new List<Person>();
			foreach (var p in personAux)
			{
				if (!(businessOportunityCompetition.Any(a => a.id_competitor == p.id)) || p.id == id_current)
				{
					tempPersons.Add(p);
				}

			}
			personAux = tempPersons;
			return personAux;
		}

		public static IEnumerable PersonsTransportistDriver(int? id_company)
		{
			var db = new DBContext();
			return db.Person.Where(s => s.id_company == id_company && s.Rol.Any(a => a.name == "Transportista")).Select(s => new { id = s.id, name = s.fullname_businessName }).ToList();
		}

        public static IEnumerable PersonsExportPlant()
        {
            var db = new DBContext();
            return db.Person.Where(s => s.Rol.Any(a => a.name == "Planta Exportadora")).ToList();
        }

        public static IEnumerable PersonsTransportist(int? id_company)
		{
			var db = new DBContext();
			return db.Person.Where(s => s.isActive && s.id_company == id_company && s.Rol.Any(a => a.name == "Transportista")).Select(s => new { id = s.id, name = s.identification_number.ToString() + " - " + s.fullname_businessName }).ToList();
		}

		public static IEnumerable CustomerByCompanyWithForeignCustomer(int? id_company)
		{
			var db = new DBContext();
			return db.ForeignCustomerIdentification
							.Where(g => (g.Person.isActive && g.Person.id_company == id_company &&
										   g.Person.Rol.Any(a => a.name == "Cliente Exterior")
								  ) /*&& g.Person.ForeignCustomer != null)*/)
							 .Select(p => new
							 {
								 id = p.Person.id,
								 name = p.Person.fullname_businessName,
								 phone = p.phone1FC,
								 fax = p.fax1FC,
							 }).Distinct().ToList();
		}
		public static IEnumerable CustomerByCompanyWithForeignCustomerIdentification(int? id_customer, int? id_adddressCustomer)
		{
			var db = new DBContext();
			var model = db.ForeignCustomerIdentification
							.Where(g => (g.Person.isActive && g.Person.id == id_customer &&
										   g.Person.Rol.Any(a => a.name == "Cliente Exterior")) ||
										g.id == (id_adddressCustomer == null ? 0 : id_adddressCustomer)
										   )
							 .Select(p => new
							 {
								 p.id,
								 tipoDireccion = db.tbsysCatalogueDetail.FirstOrDefault(a => a.id == p.AddressType).name,
								 name = p.addressForeign,
								 emailInterno = p.emailInterno,
								 emailInternoCC = p.emailInternoCC,
								 phone1FC = p.phone1FC,
								 fax1FC = p.fax1FC
							 }).ToList();

			return model;
		}

		public static IEnumerable PersonsByCompanyByWarehouseTypeAndCurrent(int? id_company, int? id_warehouseType, int? id_current)
		{
			var db = new DBContext();
			var warehouseType = db.WarehouseType.FirstOrDefault(fod => fod.id == id_warehouseType) ?? new WarehouseType();
			var persons = db.Person.Where(g => (g.isActive && g.id_company == id_company) ||
												 g.id == id_current).ToList();
			if (warehouseType.code == "VIR01")
			{
				persons = persons.Where(w => (w.Provider != null && (w.WarehouseLocation.FirstOrDefault(fod => fod.Warehouse.WarehouseType.code == "VIR01") == null)) ||
											  w.id == id_current).ToList();
			}
            else if (warehouseType.code == "BE01")
            {
                persons = persons.Where(w => w.Rol.Any(a => a.name == "Cliente Exterior") ||
                                             w.id == id_current).ToList();
            }
            else
			{
				if (warehouseType.code == "RES01")
				{
					persons = persons.Where(w => (w.Customer != null && (w.WarehouseLocation.FirstOrDefault(fod => fod.Warehouse.WarehouseType.code == "RES01") == null)) ||
												  w.id == id_current).ToList();
				}
				else
				{
					persons = new List<Person>();
				}
			}
			return persons;
		}

		public static IQueryable<PersonBasicModelP> QueryPersonBasicModelInformation(DBContext db)
		{
			return db.Person.Select(s => new PersonBasicModelP
			{
				idPersonP = s.id,
				fullNamePersonP = s.fullname_businessName,
				addressP = s.address,
				emailP = s.email,
				identificationNumberP = s.identification_number,
				idIdentificationTypeP = s.id_identificationType,
				idPersonTypeP = s.id_personType,
				isActive = s.isActive
			});
		}

		public static PersonBasicModelP GetOnePersonBasicModelInformation(int idPerson)
		{
			var db = new DBContext();
			return db.Person.Where(w => w.id == idPerson).Select(s => new PersonBasicModelP
			{
				idPersonP = s.id,
				idPersonTypeP = s.id_personType,
				idIdentificationTypeP = s.id_identificationType,
				identificationNumberP = s.identification_number,
				addressP = s.address,
				emailP = s.email,
				fullNamePersonP = s.fullname_businessName,
				isActive = s.isActive
			}).FirstOrDefault();
		}

		public static IEnumerable GetCommisionAgents()
		{
			var db = new DBContext();
			var list = db.Person.Where(p => p.Rol.FirstOrDefault(r => r.name.Equals("Comisionista")) != null).Select(s => new
			{
				s.id,
				s.fullname_businessName
			}).ToList();
			return list;
		}
		public static IEnumerable GetSellerAssigned()
		{
			var db = new DBContext();
			var list = db.Person.Where(p => p.Rol.FirstOrDefault(r => r.name.Equals("Vendedor")) != null).Select(s => new
			{
				id = s.id,
				name = s.fullname_businessName
			}).ToList();
			return list;
		}
		public static IEnumerable GetPersonProcesPlant()
		{
			var db = new DBContext();
			var list = db.Person.Where(c => c.isActive && c.Rol.FirstOrDefault(r => r.name.Equals("Planta Proceso")) != null).Select(s => new
			{
				id = s.id,
				planta = s.identification_number,
				name = s.fullname_businessName,
				processPlant = s.processPlant
			}).ToList();
			return list;
		}

		public static IEnumerable ProviderShrimpByCompany(int? id_company)
		{
			var db = new DBContext();
			return db.Provider.Where(g => (g.Person.isActive && g.Person.id_company == id_company) && g.ProviderGeneralData.ProviderType.code.Equals("40")).Select(p => new
			{
				p.id,
				name = p.Person.fullname_businessName
			}).ToList();
		}

		public static bool validarRolExistente(string nombreRol, int[] ids)
		{
			var db = new DBContext();
			var rol = db.Rol.Where(p => p.name.Equals(nombreRol)).FirstOrDefault().id;
			if (ids.Contains(rol))
			{
				return true;
			}
			return false;
		}

		public static IEnumerable PersonsByMachineRolAndInputRol(int? id_company, string rol, int? id_user)
		{
			var idPerson = dba.User.FirstOrDefault(u => u.id == id_user && u.id_company == id_company).id_employee;
			var idRols = new List<int>();
			var personas = new List<Person>();
			if (idPerson.HasValue)
			{
				var idsRol = dba.Person.FirstOrDefault(p => p.id == idPerson && p.id_company == id_company).Rol.Select(r => r.id).Distinct();
				foreach (var maquina in dba.MachineForProd)
				{
					foreach (var idRol in idsRol)
					{
						var rolId = maquina.tbsysTypeMachineForProd.Rol.id;
						if (rolId == idRol && (!idRols.Contains(rolId)))
						{
							idRols.Add(rolId);
						}
					}
				}
			}

			foreach (var idRol in idRols)
			{
				var range = dba.Person.Where(e => e.id_company == id_company && (e.Rol.Select(z => z.id).Distinct().Contains(idRol) ||
											 e.Rol.Select(z => z.name).Distinct().Contains(rol))).ToList();

				personas.AddRange(range);
			}

			var retorno = personas.Select(e => new {
				e.id,
				e.fullname_businessName
			}).Distinct();

			return retorno;
		}

        public static Person PersonByIdentificationNumberByIdCompany(int? id_company)
        {
            var db = new DBContext();
            var aCompany = db.Company.FirstOrDefault(fod=> fod.id == id_company);
            var aCompanyRUC = aCompany?.ruc;
            return db.Person.FirstOrDefault(p => p.identification_number == aCompanyRUC);
        }

		#region Métodos de consulta
		public static Person[] CustomersByCompany(int idCompany)
		{
			var db = new DBContext();
			return db.Person
				.Where(p => p.id_company == idCompany
					&& p.Customer != null && p.isActive)
				.Select(e => new {
					e.id,
					e.identification_number,
					e.fullname_businessName,
				})
				.ToList()
				.Select(e => new Person()
				{
					id = e.id,
					identification_number = e.identification_number,
					fullname_businessName = e.fullname_businessName,
				})
				.ToArray();
		}

		public static Person[] CustomersExteriorByCompany(int id_company)
		{
			var db = new DBContext();
			var personas = db.Person
				.Where(p => p.id_company == id_company
					&& p.ForeignCustomer != null && p.isActive)
				.Select(e => new
				{
					e.id_company,
					e.id,
					e.fullname_businessName,
					e.identification_number,
				})
				.ToList()
				.Select(e => new Person()
				{
					id = e.id,
					fullname_businessName = e.fullname_businessName,
					identification_number = e.identification_number,
					id_company = e.id_company,
				})
				.ToArray();

			return personas;
		}

		public static Person[] EmployeesByCompany(int id_company)
		{
			var db = new DBContext();

			return db.Person
				.Where(t => t.id_company == id_company 
					&& (t.Employee != null) && t.isActive)
				.ToArray();
		}
        
        public static Person[] PersonsByCompanyRolName(int? id_company, string rolName)
        {
            var db = new DBContext();
            var personas = db.Person
                .Where(g => g.id_company == id_company && g.Rol.Any(a => a.name == rolName) && g.isActive)
                .Select(e => new
                {
                    e.id_company,
                    e.id,
                    e.fullname_businessName,
                    e.identification_number,
                })
                .Distinct()
                .ToList()
                .Select(e => new Person()
                {
                    id = e.id,
                    fullname_businessName = e.fullname_businessName,
                    identification_number = e.identification_number,
                    id_company = e.id_company,
                    isActive = true,
                })
                .OrderBy(e => e.fullname_businessName)
                .ToArray();

            return personas;
        }
        public static Person[] PersonsByCompanyRols(int? id_company, string[] rols)
		{
			var db = new DBContext();
			var personas = db.Person
				.Where(g => g.id_company == id_company && g.Rol.Any(a => rols.Contains(a.name))	&& g.isActive)
				.Select(e => new
				{
					e.id_company,
					e.id,
					e.fullname_businessName,
					e.identification_number,
				})
				.Distinct()
				.ToList()
				.Select(e => new Person()
				{
					id = e.id,
					fullname_businessName = e.fullname_businessName,
					identification_number = e.identification_number,
					id_company = e.id_company,
					isActive = true,
				})
				.OrderBy(e => e.fullname_businessName)
				.ToArray();

			return personas;
		}
		
		public static Person[] PersonsByCompanyRolsForWarehouseLocations(
			int? id_company, int[] idsRol, int? idCurrentPerson)
		{
			var db = new DBContext();

			var idsPersonasUsadas = db.WarehouseLocation
				.Select(e => e.id_person)
				.Distinct();

			var personas = db.Person
				.Where(g => g.id_company == id_company && !idsPersonasUsadas.Contains(g.id)
					&& g.Rol.Any(a => idsRol.Contains(a.id)) && g.isActive)
				.ToList();

			if (idCurrentPerson.HasValue && idCurrentPerson > 0)
			{
				var personaAgregar = db.Person.FirstOrDefault(e => e.id == idCurrentPerson.Value);
				personas.Add(personaAgregar);
            }

			return personas
                .Select(e => new Person()
                {
                    id = e.id,
                    fullname_businessName = e.fullname_businessName,
                    identification_number = e.identification_number,
                    id_company = e.id_company,
                    isActive = true,
                })
                .OrderBy(e => e.fullname_businessName)
                .ToArray();
		}

		public static DireccionResult GetForeignCustomerIdentificationById(int id)
		{
			var db = new DBContext();
			var foreignCustomer = db.ForeignCustomerIdentification
				.FirstOrDefault(e => e.id == id);

			if(foreignCustomer != null)
            {
				return new DireccionResult
				{
					Id = foreignCustomer.id,
					TipoDireccion = db.tbsysCatalogueDetail.FirstOrDefault(a => a.id == foreignCustomer.AddressType).name,
					Direccion = foreignCustomer.addressForeign,
					EmailInterno = foreignCustomer.emailInterno,
					EmailInternoCC = foreignCustomer.emailInternoCC,
					Telefono = foreignCustomer.phone1FC,
					Fax = foreignCustomer.fax1FC
				};
            }

			return null;
		}

		public static DireccionResult[] GetForeignCustomerIdentificationByCustomer(int idCompany, int? id_customer)
		{
			var db = new DBContext();
			var model = db.ForeignCustomerIdentification
				.Where(g => g.Person.id_company == idCompany &&
							g.Person.isActive && g.Person.id == id_customer &&
							g.Person.Rol.Any(a => a.name == DataProviderRol.m_RolClienteExterior))
				.Select(p => new
				{
					p.id,
					tipoDireccion = db.tbsysCatalogueDetail.FirstOrDefault(a => a.id == p.AddressType).name,
					name = p.addressForeign,
					emailInterno = p.emailInterno,
					emailInternoCC = p.emailInternoCC,
					phone1FC = p.phone1FC,
					fax1FC = p.fax1FC
				})
				.ToList()
				.Select(e => new DireccionResult 
				{
					Id = e.id,
					TipoDireccion = e.tipoDireccion,
					Direccion = e.name,
					EmailInterno = e.emailInterno,
					EmailInternoCC = e.emailInternoCC,
					Telefono = e.phone1FC,
					Fax = e.fax1FC
				})
				.ToArray();

			return model;
		}

		public class DireccionResult
        {
			public int Id { get; set; }
			public string TipoDireccion { get; set; }
			public string Direccion { get; set; }
			public string EmailInterno { get; set; }
			public string EmailInternoCC { get; set; }
			public string Telefono { get; set; }
			public string Fax { get; set; }
        }
		#endregion
	}
}