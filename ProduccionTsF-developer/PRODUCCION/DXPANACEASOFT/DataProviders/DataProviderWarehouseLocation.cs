using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderWarehouseLocation
	{
		private static DBContext db = new DBContext();

		public static IEnumerable WarehouseLocations(int? id_company, EntityObjectPermissions entityObjectPermissions = null)
		{
			db = new DBContext();
			var model = db.WarehouseLocation.Where(t => t.isActive).ToList();

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null).ToList();
					var entityValuesWarehouse = entityValuePermissions.Select(s => s.id_entityValue).Distinct().ToList();
					model = model.Where(w => entityValuesWarehouse.Contains(w.id_warehouse)).ToList();
				}
			}

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company && p.isActive).ToList();


			}

			return model;
		}

		public static IEnumerable WarehouseLocationsByWarehouse(int? id_company, EntityObjectPermissions entityObjectPermissions = null, int? idWarehouse = null, int? id_current = null)
		{
			db = new DBContext();
			//var model = db.WarehouseLocation.Where(t => t.isActive).ToList();

			//model = model.Where(w => w.id_warehouse == idWarehouse).ToList();
			var model = db.WarehouseLocation.Where(t => t.id_warehouse == idWarehouse && t.isActive || (t.id == id_current)).ToList();

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null).ToList();
					var entityValuesWarehouse = entityValuePermissions.Select(s => s.id_entityValue).Distinct().ToList();
					model = model.Where(w => entityValuesWarehouse.Contains(w.id_warehouse) || (w.id == id_current)).ToList();
				}
			}

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company || (p.id == id_current)).ToList();
			}

			return model;
		}

		public static IEnumerable FreezerWarehouseLocationbyCompany(int? id_company)
		{
			db = new DBContext();
			var model = db.WarehouseLocation.Where(t => t.id_company == id_company &&
														t.Warehouse.WarehouseType.code.Equals("BCO01")).ToList();//BCO01 Codigo de bodegas de Congelación

			return model;
		}

		public static IEnumerable MaintenanceWarehouseLocationbyCompany(int? id_company)
		{
			db = new DBContext();
			var model = db.WarehouseLocation.Where(t => t.id_company == id_company &&
														t.Warehouse.WarehouseType.code.Equals("BCC01")).ToList();//BCC01 Codigo de Bodega Camara de Congelación

			return model;
		}

		public static IEnumerable WarehouseLocationsByWarehouseCode(string codeWarehouse)
		{
			db = new DBContext();
			var query = db.WarehouseLocation.Where(t => t.Warehouse.code == codeWarehouse && t.isActive);
			return query.ToList();
		}

		public static IEnumerable WarehouseLocationsByWarehouse(int? id_warehouse)
		{
			db = new DBContext();
			var query = db.WarehouseLocation.Where(t => t.id_warehouse == id_warehouse && t.isActive);
			return query.ToList();
		}

		public static IEnumerable WarehouseLocationsByWarehouseAndCurrent(int? id_warehouse, int? id_current)
		{
			db = new DBContext();
			var query = db.WarehouseLocation.Where(t => t.id_warehouse == id_warehouse && t.isActive || (t.id == id_current));
			return query.ToList();
		}

		public static WarehouseLocation WarehouseLocationById(int? id)
		{
			db = new DBContext();
			var query = db.WarehouseLocation.FirstOrDefault(t => t.id == id);
			return query;
		}

		public static IEnumerable AllWarehouseLocationsMPByCompany(int? id_company)
		{
			db = new DBContext();

			return db.WarehouseLocation.Where(s => s.id_company == id_company && s.Warehouse.InventoryLine.code == "MP").Select(s => new { id = s.id, name = s.name + "(" + s.Warehouse.name + ")" }).ToList();

		}

		public static IEnumerable AllWarehouseLocationsMIByCompany(int? id_company)
		{
			db = new DBContext();

			return db.WarehouseLocation.Where(s => s.id_company == id_company && s.Warehouse.InventoryLine.code == "MI").Select(s => new { id = s.id, name = s.name + "(" + s.Warehouse.name + ")" }).ToList();

		}

		public static IEnumerable AllWarehouseLocationsDEByCompany(int? id_company)
		{
			db = new DBContext();

			return db.WarehouseLocation.Where(s => s.id_company == id_company && s.Warehouse.InventoryLine.code == "DE").Select(s => new { id = s.id, name = s.name + "(" + s.Warehouse.name + ")" }).ToList();

		}
		public static WarehouseLocation WarehouseLocationDispatchMaterialByCompanyAndInventoryLine(int? id_company, int? id_inventoryLine)
		{
			db = new DBContext();

			Setting settingUDLI = db.Setting.FirstOrDefault(t => t.code == "UDLI");
			WarehouseLocation warehouseLocationAux = null;


			var id_warehouseLocationAux = settingUDLI?.SettingDetail.FirstOrDefault(fod => fod.value == id_inventoryLine.ToString())?.valueAux;
			var id_warehouseLocationAuxInt = (id_warehouseLocationAux != null) ? int.Parse(id_warehouseLocationAux) : (int?)null;
			warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationAuxInt);


			return warehouseLocationAux;
		}

		public static WarehouseLocation WarehouseLocationByProcess(int? idProduccionProcess)
		{
			db = new DBContext();

			WarehouseLocation ubicacion = new WarehouseLocation();

			var processo = db.ProductionProcess.FirstOrDefault(r => r.id == idProduccionProcess);
			if (processo != null)
			{
				ubicacion = db.WarehouseLocation.FirstOrDefault(t => t.id == processo.id_WarehouseLocation);
			}
			return ubicacion;
		}
	}
}