using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
	public class MachineForProdController : DefaultController
	{
		public ActionResult Index()
		{
			return View();
		}

		#region FILTERS RESULTS

		[HttpPost]
		public ActionResult MachineForProdResults(MachineForProd MachineForProd)
		{
			var model = db.MachineForProd.ToList();

			#region  FILTERS

			#endregion
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_MachineForProdResultsPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion
		#region  HEADER
		[HttpPost, ValidateInput(false)]
		public ActionResult MachineForProdPartial()
		{
			var model = (TempData["model"] as List<MachineForProd>);

			model = db.MachineForProd.ToList();

			model = model ?? new List<MachineForProd>();
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_MachineForProdPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion
		#region Edit MachineForProd
		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditMachineForProd(int id, int[] orderDetails)
		{
			MachineForProd MachineForProd = db.MachineForProd.Where(o => o.id == id).FirstOrDefault();

			//if (MachineForProd == null)
			//{
			//	MachineForProd = new MachineForProd
			//	{
			//		id_userUpdate = ActiveUser.id,
			//		dateUpdate = DateTime.Now

			//	};
			//}
			TempData["MachineForProd"] = MachineForProd;
			TempData.Keep("MachineForProd");

			return PartialView("_FormEditMachineForProd", MachineForProd);
		}
		#endregion
		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_MachineForProd)
		{
			TempData.Keep("MachineForProd");
			int index = db.MachineForProd.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_MachineForProd);
			var result = new
			{
				maximunPages = db.MachineForProd.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}



		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			MachineForProd MachineForProd = db.MachineForProd.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (MachineForProd != null)
			{
				TempData["MachineForProd"] = MachineForProd;
				TempData.Keep("MachineForProd");
				return PartialView("_MachineForProdMainFormPartial", MachineForProd);
			}

			TempData.Keep("MachineForProd");

			return PartialView("_MachineForProdMainFormPartial", new MachineForProd());
		}
		#endregion
		#region Validacion
		[HttpPost, ValidateInput(false)]
		public JsonResult ReptCodigo(int id_MachineForProd, string codigo)
		{
			TempData.Keep("MachineForProd");


			bool rept = false;

			var cantre = db.MachineForProd.Where(x => x.id != id_MachineForProd && x.code == codigo).ToList().Count();
			if (cantre > 0)
			{
				rept = true;
			}

			var result = new
			{
				rept = rept
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#region Save and Update
		[HttpPost, ValidateInput(false)]
		public ActionResult MachineForProdPartialAddNew(MachineForProd item)
		{
			MachineForProd conMachineForProd = (TempData["MachineForProd"] as MachineForProd);

			DBContext dbemp = new DBContext();

			using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
			{
				try
				{
                    #region MachineForProd
                    #endregion
                    item.dateCreate = DateTime.Now;
                    item.id_userCreate = ActiveUser.id;
                    item.dateUpdate = DateTime.Now;
                    item.id_userUpdate = ActiveUser.id;
                    item.dateUpdateAvailable = DateTime.Now;
                    item.id_userUpdateAvailable = ActiveUser.id;

                    dbemp.MachineForProd.Add(item);
					dbemp.SaveChanges();
					trans.Commit();

					TempData["MachineForProd"] = item;
					TempData.Keep("MachineForProd");
					ViewData["EditMessage"] = SuccessMessage("Máquina: " + item.id + " guardada exitosamente");
				}
				catch (Exception e)
				{
					TempData.Keep("MachineForProd");
					item = (TempData["MachineForProd"] as MachineForProd);
					ViewData["EditMessage"] = ErrorMessage(e.Message);

					trans.Rollback();
				}
			}

			//return PartialView("_MachineForProdMainFormPartial", item);

			return PartialView("_MachineForProdPartial", dbemp.MachineForProd.ToList());
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult MachineForProdPartialUpdate(MachineForProd item)
		{
			MachineForProd modelItem = db.MachineForProd.FirstOrDefault(r => r.id == item.id);
			if (modelItem != null)
			{
				MachineForProd conMachineForProd = (TempData["MachineForProd"] as MachineForProd);

				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						#region MachineForProd

						modelItem.id_userUpdate = ActiveUser.id;
						modelItem.dateUpdate = DateTime.Now;
						modelItem.code = item.code;
						modelItem.name = item.name;
						modelItem.isActive = item.isActive;
						modelItem.id_tbsysTypeMachineForProd = item.id_tbsysTypeMachineForProd;
						modelItem.QuantityCapacity = item.QuantityCapacity;
						modelItem.id_metricUnit = 1;
						modelItem.id_personProcessPlant = item.id_personProcessPlant;
						modelItem.id_warehouseType = item.id_warehouseType;
						modelItem.id_materialWarehouse = item.id_materialWarehouse;
						modelItem.id_materialWarehouseLocation = item.id_materialWarehouseLocation;
						modelItem.id_materialCostCenter = item.id_materialCostCenter;
						modelItem.id_materialSubCostCenter = item.id_materialSubCostCenter;
						modelItem.id_materialthirdWarehouse = item.id_materialthirdWarehouse;
						modelItem.id_materialthirdWarehouseLocation = item.id_materialthirdWarehouseLocation;
						modelItem.id_materialthirdCostCenter = item.id_materialthirdCostCenter;
						modelItem.id_materialthirdSubCostCenter = item.id_materialthirdSubCostCenter;
						#endregion

						db.MachineForProd.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;
						db.SaveChanges();
						trans.Commit();

						TempData["MachineForProd"] = modelItem;
						TempData.Keep("MachineForProd");
						ViewData["EditMessage"] = SuccessMessage("Máquina: " + modelItem.id + " guardado exitosamente");
					}
					catch (Exception e)
					{
						TempData.Keep("MachineForProd");
						ViewData["EditMessage"] = ErrorMessage(e.Message);

						trans.Rollback();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			TempData.Keep("MachineForProd");


			return PartialView("_MachineForProdPartial", db.MachineForProd.ToList());
		}
		#endregion
		#region MachineForProd Gridview

		[ValidateInput(false)]
		public ActionResult MachineForProdPartial(int? id)
		{
			if (id != null)
			{
				ViewData["MachineForProdToCopy"] = db.MachineForProd.Where(b => b.id == id).FirstOrDefault();
			}
			var model = db.MachineForProd.ToList();
			return PartialView("_MachineForProdPartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult MachineForProdPartialDelete(System.Int32 id)
		{
			if (id >= 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var item = db.MachineForProd.FirstOrDefault(it => it.id == id);
						//if (item != null)
						//{

						//	item.id_userUpdate = ActiveUser.id;
						//	item.dateUpdate = DateTime.Now;

						//}
						db.MachineForProd.Attach(item);
						db.Entry(item).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Máquina : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
					}
					catch (Exception)
					{
						ViewData["EditMessage"] = ErrorMessage();
						trans.Rollback();
					}
				}

			}

			var model = db.MachineForProd.ToList();
			return PartialView("_MachineForProdPartial", model.ToList());
		}

		public ActionResult DeleteSelectedMachineForProd(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var MachineForProds = db.MachineForProd.Where(i => ids.Contains(i.id));
						foreach (var vMachineForProd in MachineForProds)
						{
							//vMachineForProd.id_userUpdate = ActiveUser.id;
							//vMachineForProd.dateUpdate = DateTime.Now;

							db.MachineForProd.Attach(vMachineForProd);
							db.Entry(vMachineForProd).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Máquina desactivados exitosamente");
					}
					catch (Exception)
					{
						trans.Rollback();
						ViewData["EditMessage"] = ErrorMessage();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			var model = db.MachineForProd.ToList();
			return PartialView("_MachineForProdPartial", model.ToList());
		}

		#endregion

		#region Auxiliar
		[HttpPost]
		public JsonResult WarehouseChangeData(int id_warehouse)
		{
			if (TempData["machineForProd"] != null)
			{
				TempData.Keep("machineForProd");
			}
			var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse && w.isActive)
									   .Select(s => new
									   {
										   id = s.id,
										   name = s.name
									   });

			var result = new
			{
				warehouseLocations = warehouseLocations
									.Select(s => new
									{
										id = s.id,
										name = s.name
									})
			};

			TempData.Keep("machineForProd");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult WarehouseTypeChangeData(int id_warehouseType)
		{
			var model = db.Warehouse
									.Where(w => w.id_warehouseType == id_warehouseType && w.isActive)
										.Select(s => new {
											id = s.id,
											name = s.name
										}).ToList();


			return Json(model, JsonRequestBehavior.AllowGet);
		}

		public JsonResult CostCenterChangeData(int id_costCenter)
		{
			if (TempData["machineForProd"] != null)
			{
				TempData.Keep("machineForProd");
			}
			var subcenterCost = db.CostCenter.Where(w => w.id_higherCostCenter == id_costCenter && w.isActive && w.id_higherCostCenter != null)
									   .Select(s => new
									   {
										   id = s.id,
										   name = s.name
									   });

			return Json(subcenterCost, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
