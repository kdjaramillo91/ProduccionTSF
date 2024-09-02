using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;
using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Models.WarehouseP.WarehouseModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderWarehouse
	{
		private static DBContext _db = null;

		public static IEnumerable Warehouses(int? id_company,
												EntityObjectPermissions entityObjectPermissions = null,
												string customParamOP = null,
												int idWarehouseDefault = 0
												)
		{
			_db = new DBContext();
			var model = _db.Warehouse.Where(t => t.isActive).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
			}
			if (idWarehouseDefault != 0)
			{
				model = model.Where(p => p.id == idWarehouseDefault).ToList();
			}

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
					model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
				}
			}
			if (customParamOP != null && customParamOP == "IPXM")
			{
				var paramPackagingMaterials = DataProviderAdvanceParametersDetail.GetAdvanceParameterDetailByCode("IPXM") as List<AdvanceParametersDetailModelP>;
				if (paramPackagingMaterials != null)
				{

					var datWarehouseLocation = paramPackagingMaterials.FirstOrDefault(r => r.codeAdvanceDetailModelP.Trim() == "CWAR");
					if (datWarehouseLocation != null)
					{

						model = model.Where(f => f.InventoryLine.code == datWarehouseLocation.nameAdvanceDetailModelP).ToList();


					}


				}





			}
			return model;
		}

		public static IEnumerable Warehouses(int? id_company, string codeDocumentTypeInventory, EntityObjectPermissions entityObjectPermissions = null)
		{
			_db = new DBContext();
			var model = _db.Warehouse.Where(t => t.isActive).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
			}

			if (codeDocumentTypeInventory == "04")//Ingreso x Orden de Compra
			{
				model = model.Where(p => p.WarehouseType.code.Contains("BMP") || p.WarehouseType.code.Contains("BRE") || p.WarehouseType.code.Contains("BPD")).ToList();
				//Tipos de Bodegas: BMP## Bodega de Materias Primas, BRE## Bodega de Recepcion y BPD## Bodega Planta de Despacho
			}

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
					model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
				}
			}

			return model;
		}

		public static IEnumerable ProductionSchedulePeriod(int? id_yearPeriod)
        {
			_db = new DBContext();

			var detail = _db.InventoryValuationPeriodDetail.Where(e => e.id_InventoryValuationPeriod == id_yearPeriod).ToList();

			return detail;
        }

		public static IEnumerable ProductionSchedulePeriodsByCompany(int? id_company, int? id_current)
		{
			_db = new DBContext();

            var productionSchedulePeriods = _db.InventoryValuationPeriodDetail.Where(t => (t.id_InventoryValuationPeriod == (id_current == null ? 0 : id_current))).ToList();

            var productionSchedulePeriods2 = _db.InventoryValuationPeriod.Where(t => (t.isActive == true) || (t.id == (id_current == null ? 0 : id_current))).ToList();

            //join
            var lstInventoryMoveEPTAMDL = (from a in productionSchedulePeriods
                                           join b in productionSchedulePeriods2 on a.id_InventoryValuationPeriod equals b.id
                                           select new
                                           {
                                               id = a.id,
                                               year = b.year,
                                               period = a.periodNumber,
                                               id_InventoryValuationPeriod = a.id_InventoryValuationPeriod,
                                           }).ToList();

            return lstInventoryMoveEPTAMDL;

		}

		public static IEnumerable WarehouseYearPeriod(int? yearPeriod)
		{
			_db = new DBContext();
			var model = _db.InventoryValuationPeriod.Where(t => t.year == (yearPeriod == null ? 0 : yearPeriod) || t.isActive == true).OrderByDescending(e => e.year).ToList();

			return model;
		}

		public static IEnumerable FreezerWarehousebyCompany(int? id_company)
		{
			_db = new DBContext();
			var model = _db.Warehouse.Where(t => t.id_company == id_company && t.WarehouseType.code.Equals("BCO01")).ToList();//BCO01 Codigo de bodegas de Congelación

			return model;
		}

		public static IEnumerable FreezerWarehousebyCompanyAndCurrent(int? id_company, int? id_current)
		{
			_db = new DBContext();
			var model = _db.Warehouse.Where(t => (t.isActive && t.id_company == id_company && t.WarehouseType.code.Equals("BCO01")) || t.id == id_current).ToList();//BCO01 Codigo de bodegas de Congelación

			return model;
		}

		public static IEnumerable MaintenanceWarehousebyCompany(int? id_company)
		{
			_db = new DBContext();
			var model = _db.Warehouse.Where(t => t.id_company == id_company && t.WarehouseType.code.Equals("BCC01")).ToList();//BCC01 Codigo de Bodega Camara de Congelación

			return model;
		}

		public static IEnumerable WarehouseFilter(int id_company)
		{
			_db = new DBContext();
			var model = _db.Warehouse.ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company).ToList();
			}

			return model;
		}

		public static IEnumerable WarehousesByWarehouseType(int? id_warehouseType)
		{
			_db = new DBContext();
			var query = _db.Warehouse.Where(t => t.id_warehouseType == id_warehouseType && t.isActive);
			return query.ToList();
		}

		public static Warehouse WarehouseById(int? id)
		{
			_db = new DBContext();
			var query = _db.Warehouse.FirstOrDefault(t => t.id == id);
			return query;
		}

		public static Warehouse WarehouseByProcess(int? idProduccionProcess)
		{
			_db = new DBContext();

			Warehouse bodega = new Warehouse();

			var processo = _db.ProductionProcess.FirstOrDefault(r => r.id == idProduccionProcess);
			if (processo != null)
			{
				bodega = _db.Warehouse.FirstOrDefault(t => t.id == processo.id_warehouse);
			}
			return bodega;
		}

		public static IEnumerable WarehouseByCompany(int? id_company)
		{
			_db = new DBContext();
			return _db.Warehouse.Where(g => (g.isActive && g.id_company == id_company))
							   .Select(p => new { p.id, name = p.name }).ToList();
		}

		public static IEnumerable WarehousesByCompanyAndCurrent(int? id_company, int? id_current)
		{
			_db = new DBContext();
			return _db.Warehouse.Where(g => (g.isActive && g.id_company == id_company) ||
												 g.id == (id_current == null ? 0 : id_current)).ToList();
		}

		public static IEnumerable AllWarehousesMPByCompany(int? id_company)
		{
			_db = new DBContext();

			return _db.Warehouse.Where(s => s.id_company == id_company && s.InventoryLine.code == "MP").Select(s => new { id = s.id, name = s.name }).ToList();

		}
		public static Warehouse WarehousesDispatchMaterialByCompanyAndInventoryLine(int? id_company, int? id_inventoryLine)
		{
			_db = new DBContext();

			Setting settingUDLI = _db.Setting.FirstOrDefault(t => t.code == "UDLI");
			WarehouseLocation warehouseLocationAux = null;


			var id_warehouseLocationAux = settingUDLI?.SettingDetail.FirstOrDefault(fod => fod.value == id_inventoryLine.ToString())?.valueAux;
			var id_warehouseLocationAuxInt = (id_warehouseLocationAux != null) ? int.Parse(id_warehouseLocationAux) : (int?)null;
			warehouseLocationAux = _db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationAuxInt);


			return warehouseLocationAux?.Warehouse;
		}

		public static List<Warehouse> WarehousesDispatchMaterialByCompany(int? id_company)
		{

			_db = new DBContext();

			string codeWarehouseTypeDispatchMaterial = "BMI01";
			var warehousesDispatchMaterial = _db.Warehouse
												  .Where(r => r.id_company == id_company && r.WarehouseType.code == codeWarehouseTypeDispatchMaterial)
												  .ToList();


			return warehousesDispatchMaterial;
		}

		public static IEnumerable AllWarehousesMIByCompany(int? id_company)
		{
			_db = new DBContext();

			return _db.Warehouse.Where(s => s.id_company == id_company && s.InventoryLine.code == "MI").Select(s => new { id = s.id, name = s.name }).ToList();

		}

		public static IEnumerable AllWarehousesDEByCompany(int? id_company)
		{
			_db = new DBContext();

			return _db.Warehouse.Where(s => s.id_company == id_company && s.InventoryLine.code == "DE").Select(s => new { id = s.id, name = s.name }).ToList();

		}
		public static IEnumerable AllWarehousesVIRPROByCompany(int? id_company)
		{
			_db = new DBContext();
			var model = _db.Warehouse.Where(t => t.id_company == id_company && t.code.Equals("VIRPRO")).ToList();//VIRPRO Codigo de bodegas Virtual de Proveedores

			return model;
		}

		public static IEnumerable AllWarehousesWithVIRPROByCompany(int? id_company, EntityObjectPermissions entityObjectPermissions = null)
		{
			_db = new DBContext();
			var model = _db.Warehouse.Where(t => t.id_company == id_company && (t.isActive || t.code.Equals("VIRPRO"))).ToList();//VIRPRO Codigo de bodegas Virtual de Proveedores

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
					model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.id) != null).ToList();
				}
			}

			return model;
		}

		public static IQueryable<WarehouseModelP> QueryWarehouseModelP(DBContext db)
		{
			return db.Warehouse.Select(s => new WarehouseModelP
			{
				idWarehouseModelP = s.id,
				nameWarehouseModelP = s.name,
				descWarehouseModelP = s.description,
				idInventoryLineModelP = s.id_inventoryLine,
				idWarehouseTypeModelP = s.id_warehouseType,
				isActive = s.isActive
			});
		}
		public static List<WarehouseLocationModelP> GetWarehouseLocationModelP()
		{
			_db = new DBContext();
			return _db.WarehouseLocation.Select(s => new WarehouseLocationModelP
			{
				idWarehouseLocationModelP = s.id,
				idWarehouseModelP = s.id_warehouse,
				codeWarehouseLocationModelP = s.code,
				nameWarehouseLocationModelP = s.name
			}).ToList();
		}

		public static WarehouseModelP GetOneWarehouseModelP(int idW)
		{
			_db = new DBContext();
			return _db.Warehouse.Where(w => w.id == idW).Select(s => new WarehouseModelP
			{
				idWarehouseModelP = s.id,
				nameWarehouseModelP = s.name,
				descWarehouseModelP = s.description,
				idInventoryLineModelP = s.id_inventoryLine,
				idWarehouseTypeModelP = s.id_warehouseType,
				isActive = s.isActive
			}).FirstOrDefault();
		}

		public static WarehouseLocationModelP GetOneWarehouseLocationModelP(int idWL)
		{
			_db = new DBContext();
			return _db.WarehouseLocation
				.Where(w => w.id_warehouse == idWL)
				.Select(s => new WarehouseLocationModelP
				{
					idWarehouseLocationModelP = s.id,
					idWarehouseModelP = s.id_warehouse,
					codeWarehouseLocationModelP = s.code,
					nameWarehouseLocationModelP = s.name
				}).FirstOrDefault();
		}

		public static IEnumerable WarehousesByCodeType(int? id_company, string code)
		{
			_db = new DBContext();
			var model = _db.Warehouse.Where(t => t.isActive && t.WarehouseType.code.Equals(code)).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company && p.isActive && p.WarehouseType.code.Equals(code)).ToList();
			}

			return model;
		}

		public static IEnumerable FreezerWarehousebyCompanyParameter(int? id_company)
		{
			_db = new DBContext();
			List<Warehouse> warehouseList = new List<Warehouse>();
			var tipoBodegasCods = _db.SettingDetail
								.Where(r => r.Setting.code == "CONGW")
								.Select(r => r.value)
								.ToArray();
			if (tipoBodegasCods.Count() > 0)
			{
				warehouseList = _db.Warehouse.Where(t => t.id_company == id_company
														 && tipoBodegasCods.Contains(t.WarehouseType.code)
														 && t.isActive == true
														 ).ToList();
			}

			return warehouseList;
		}

		public static List<WarehouseModelDto> FreezerWarehousebyCompanyParameterForCost(int id_company, int anio, int mes)
		{
			_db = new DBContext();
			List<WarehouseModelDto> warehouseList = new List<WarehouseModelDto>();
			var tipoBodegasCods = _db.SettingDetail
								.Where(r => r.Setting.code == "CONGW")
								.Select(r => r.value)
								.ToArray();
			var estadosPeriodos = _db.AdvanceParametersDetail
													.Where(r => r.AdvanceParameters.code == "EPIV1")
													.ToList();

			if (tipoBodegasCods.Count() > 0)
			{
				var resultWarehouse = _db.Warehouse.Where(t => t.id_company == id_company
														 && tipoBodegasCods.Contains(t.WarehouseType.code)
														 && t.isActive == true
														 ).ToList();
				int[] warehouseIds = resultWarehouse
											.Select(r => r.id)
											.ToArray();

				if (warehouseIds.Length > 0)
				{

					var periodos = _db.InventoryPeriodDetail
											.Where(r => warehouseIds.Contains(r.InventoryPeriod.id_warehouse)
														&& r.InventoryPeriod.year == anio
														&& r.periodNumber == mes)
											.Select(r => new
											{
												bodegaId = r.InventoryPeriod.id_warehouse,
												estadoId = r.id_PeriodState
											})
											.ToList();


					warehouseList = (from bodega in resultWarehouse
									 join periodo in periodos on bodega.id equals periodo.bodegaId
									 join estado in estadosPeriodos on periodo.estadoId equals estado.id
									 select new WarehouseModelDto
									 {
										 Id = bodega.id,
										 Descripcion = $"{bodega.name} - {estado.valueName}"
									 })
									 .ToList();

				}
			}

			return warehouseList;
		}

	}
}
