using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderWarehouseUserPermit
	{
		private static DBContext db = null;

		public static User Users(int? id)
		{
			db = new DBContext();
			return db.User.FirstOrDefault(t => t.id == id);
		}

		public static IEnumerable UserFilter(int id_company)
		{
			db = new DBContext();
			var model = db.User.ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company).ToList();
			}

			return model;
		}

		public static UserEntity WarehouseUserPermitById(int? id)
		{
			db = new DBContext();
			var query = db.UserEntity.FirstOrDefault(t => t.id == id);
			return query;
		}

		public static IEnumerable userByCompanyAndCurrent(int? id_company, int? id_current)
		{
			db = new DBContext();

			int idUser;
			var userEntity = db.UserEntity.Select(e => e.id_user).ToList();
			var usuario = db.User.Where(g => (g.isActive && g.id_company == id_company) ||
												 g.id == (id_current == null ? 0 : id_current)).ToList();

			var auxUsuario = db.User.FirstOrDefault(e => e.id == id_current);

			if(id_current != null)
            {
				for (var i = 0; i < userEntity.Count(); i++)
				{
					idUser = userEntity[i];

					if (idUser != auxUsuario.id)
					{
						var listaAux2 = usuario.FirstOrDefault(e => e.id == idUser);

						if (listaAux2 != null)
						{
							usuario.Remove(listaAux2);
						}
					}
				}
			}
            else
            {
				for (var i = 0; i < userEntity.Count(); i++)
				{
					idUser = userEntity[i];

					var listaAux2 = usuario.FirstOrDefault(e => e.id == idUser);

					if (listaAux2 != null)
					{
						usuario.Remove(listaAux2);
					}
				}
			}

            return usuario.Select(s => new { id = s.id, username = s.username});
		}

		public static IEnumerable Employees(int? id_current)
        {
			db = new DBContext();

			if(id_current != null)
            {
				var codEmploye = db.User.FirstOrDefault(e => e.id == id_current);

				var personEmploye = db.Person.FirstOrDefault(q => q.id == codEmploye.id_employee);

				var objecta = new
				{
					id = id_current,
					fullname_businessName = personEmploye?.fullname_businessName,
				};

				return new[]
				{
					objecta
				};
			}
            else
            {
				return null;
            }
		}

		public static IEnumerable warehouseComboBox()
		{
			db = new DBContext();
			return db.Warehouse.Where(g => g.isActive).ToList();
		}

		public static Warehouse warehouseComboBoxCode(int? codigo)
		{
			var db = new DBContext();
			var code =  db.Warehouse.FirstOrDefault(g => g.id == codigo);
			if (code != null)
			{
				return code;
			}
			else
			{
				return null;
			}
		}
		
	}
}