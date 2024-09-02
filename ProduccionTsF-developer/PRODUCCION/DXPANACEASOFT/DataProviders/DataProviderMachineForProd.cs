using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderMachineForProd
	{
		private static DBContext db = null;

		public static IEnumerable MachineForProds(EntityObjectPermissions entityObjectPermissions = null)
		{
			db = new DBContext();
			var model = db.MachineForProd.Where(t => t.isActive).ToList();

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
				if (entityPermissions != null)
				{
					var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
					model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
				}
			}

			return model;
		}
		public static IEnumerable AllMachineForProds()
		{
			db = new DBContext();
			var model = db.MachineForProd.ToList();

			return model;
		}

		//GetMachineForProdOpening
		public static IEnumerable GetMachineForProdOpening(int? id_MachineForProdCurrent,
														   int? id_MachineProdOpeningCurrent,
														   DateTime? fechaTurno,
														   int? id_PersonProcessPlant,
														   int? id_Turn,
														   EntityObjectPermissions entityObjectPermissions = null)
		{
			db = new DBContext();

			// Preparamos los detalles de aperturas de máquina...
			var machineProdOpeningDetailAux = db.MachineProdOpeningDetail
				.Where(w => w.MachineProdOpening.Document.DocumentState.code == "03" && w.MachineForProd.available);

			// Aplicamos el filtro de fecha, si hubiera
			if (fechaTurno.HasValue && id_PersonProcessPlant != null)
			{
				var fechaTurnoFiltro = fechaTurno.Value.Date;
				machineProdOpeningDetailAux = machineProdOpeningDetailAux
					.Where(w => DbFunctions.TruncateTime(w.MachineProdOpening.Document.emissionDate) == fechaTurnoFiltro &&
								w.MachineForProd.id_personProcessPlant == id_PersonProcessPlant && w.MachineForProd.tbsysTypeMachineForProd.code.Equals("CLA"));
			}else
            {
                var fechaTurnoFiltro = fechaTurno.Value.Date;
                machineProdOpeningDetailAux = machineProdOpeningDetailAux
                    .Where(w => DbFunctions.TruncateTime(w.MachineProdOpening.Document.emissionDate) == fechaTurnoFiltro &&
                                w.MachineForProd.tbsysTypeMachineForProd.code.Equals("CLA"));
            }
            // Se aplican filtros de permisos sobre objetos...
            if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions
					.listEntityPermissions
					.FirstOrDefault(fod => fod.codeEntity == "MAC");

				if (entityPermissions != null)
				{
					var entityValuePermissions = entityPermissions
						.listValue
						.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);

					machineProdOpeningDetailAux = machineProdOpeningDetailAux
						.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id_MachineForProd) != null);
				}
			}
            
            var model = machineProdOpeningDetailAux
				.Select(s => new { s.id, s.MachineForProd, s.MachineProdOpening })
				.ToList();

			var modelAux = model
				.FirstOrDefault(fod => fod.MachineForProd.id == id_MachineForProdCurrent && fod.MachineProdOpening.id == id_MachineProdOpeningCurrent);

			if (id_MachineForProdCurrent != 0 && id_MachineProdOpeningCurrent != 0 && id_MachineForProdCurrent != null && id_MachineProdOpeningCurrent != null && modelAux == null)
			{
				model.Add(new
				{
					id = db.MachineProdOpeningDetail
						.FirstOrDefault(fod => fod.id_MachineForProd == id_MachineForProdCurrent && fod.id_MachineProdOpening == id_MachineProdOpeningCurrent).id,
					MachineForProd = db.MachineForProd
						.FirstOrDefault(fod => fod.id == id_MachineForProdCurrent),
					MachineProdOpening = db.MachineProdOpening.FirstOrDefault(fod => fod.id == id_MachineProdOpeningCurrent)
				});
			}

			if (id_Turn != null)
				model = model.Where(a => a.MachineProdOpening.Turn.id == id_Turn).ToList();

			return model;
		}

		public static MachineForProd MachineForProdById(int? id_machineForProd)
		{
			db = new DBContext(); ;
			return db.MachineForProd.FirstOrDefault(v => v.id == id_machineForProd);
		}

		public static IEnumerable MachineForProdWithCurrent(int? id_current)
		{
			db = new DBContext();
			return db.MachineForProd.Where(g => (g.isActive) || g.id == id_current).ToList();
		}
		public static IEnumerable TbsysTypeMachineForProdAll(int? id_current)
		{
			db = new DBContext();
			var model = db.tbsysTypeMachineForProd.Where(g => g.isActive || id_current == g.id).Select(p => new { p.id, name = p.name });

			return model.ToList();
		}

		public static IEnumerable MachineByUserRol(EntityObjectPermissions entityObjectPermissions, int? id_user, bool active = false)
		{
			db = new DBContext();
			var retorno = new List<MachineForProd>();
			List<MachineForProd> model = active ? db.MachineForProd.Where(w => w.isActive).ToList() : db.MachineForProd.ToList();
			var idPerson = db.User.FirstOrDefault(u => u.id == id_user).id_employee;

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
				if (entityPermissions != null)
				{
					var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
					model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
				}
			}

			if (idPerson.HasValue)
			{
				var idsRol = db.Person.FirstOrDefault(p => p.id == idPerson).Rol.Select(r => r.id).Distinct();
				foreach (var maquina in model)
				{
					foreach (var idRol in idsRol)
					{
						if (maquina.tbsysTypeMachineForProd.Rol.id == idRol && (!retorno.Contains(maquina)))
						{
							retorno.Add(maquina);
						}
					}
				}
			}
			return retorno.ToList();
		}
	}
}