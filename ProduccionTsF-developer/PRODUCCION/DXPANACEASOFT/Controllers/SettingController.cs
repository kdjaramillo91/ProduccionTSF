using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class SettingController : DefaultController
	{
		public ActionResult Index()
		{
			return View();
		}

		#region FILTERS RESULTS

		[HttpPost]
		public ActionResult SettingResults(Setting Setting)
		{
			var model = db.Setting.ToList();

			#region  FILTERS

			#endregion
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_SettingResultsPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion

		#region  HEADER
		[HttpPost, ValidateInput(false)]
		public ActionResult SettingPartial()
		{
			var model = (TempData["model"] as List<Setting>);

			model = db.Setting.ToList();

			model = model ?? new List<Setting>();
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_SettingPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion

		#region Edit Setting
		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditSetting(int id, int[] orderDetails)
		{
			Setting Setting = db.Setting.Where(o => o.id == id).FirstOrDefault();

			if (Setting == null)
			{
				Setting = new Setting
				{
					id_userUpdate = ActiveUser.id,
					dateUpdate = DateTime.Now

				};
			}
			TempData["Setting"] = Setting;
			TempData.Keep("Setting");

			return PartialView("_FormEditSetting", Setting);
		}
		#endregion

		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_Setting)
		{
			TempData.Keep("Setting");
			int index = db.Setting.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_Setting);
			var result = new
			{
				maximunPages = db.Setting.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}



		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			Setting Setting = db.Setting.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (Setting != null)
			{
				TempData["Setting"] = Setting;
				TempData.Keep("Setting");
				return PartialView("_SettingMainFormPartial", Setting);
			}

			TempData.Keep("Setting");

			return PartialView("_SettingMainFormPartial", new Setting());
		}
		#endregion

		#region Validacion
		[HttpPost, ValidateInput(false)]
		public JsonResult ReptCodigo(int id_Setting, string codigo)
		{
			TempData.Keep("Setting");


			bool rept = false;

			var cantre = db.Setting.Where(x => x.id != id_Setting && x.code == codigo).ToList().Count();
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
		public ActionResult SettingPartialAddNew(Setting item)
		{
			Setting conSetting = (TempData["Setting"] as Setting);

			DBContext dbemp = new DBContext();

			using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
			{
				try
				{
					#region Setting

					#endregion

					item.dateCreate = DateTime.Now;
					item.dateUpdate = DateTime.Now;
					item.id_userCreate = ActiveUser.id;
					item.id_userUpdate = ActiveUser.id;
					item.id_company = this.ActiveCompanyId;
					if(item.description == null)
					{
						item.description = "";
					}

					dbemp.Setting.Add(item);
					dbemp.SaveChanges();
					trans.Commit();

					TempData["Setting"] = item;
					TempData.Keep("Setting");
					ViewData["EditMessage"] = SuccessMessage("Parámetros: " + item.id + " guardada exitosamente");
				}
				catch (Exception e)
				{
					TempData.Keep("Setting");
					item = (TempData["Setting"] as Setting);
					ViewData["EditMessage"] = ErrorMessage(e.Message);

					trans.Rollback();
				}
			}

			//return PartialView("_SettingMainFormPartial", item);

			return PartialView("_SettingPartial", dbemp.Setting.ToList());
		}

		#region Update
		[HttpPost, ValidateInput(false)]
		public ActionResult SettingPartialUpdate(Setting item)
		{
			Setting modelItem = db.Setting.FirstOrDefault(r => r.id == item.id);
			if (modelItem != null)
			{
				Setting conSetting = (TempData["Setting"] as Setting);

				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						#region Setting

						modelItem.name = item.name;
						modelItem.code = item.code;
						modelItem.value = item.value;
						modelItem.id_settingDataType = item.id_settingDataType;
						modelItem.id_company = this.ActiveCompanyId;
						modelItem.id_module = item.id_module;
						modelItem.isActive = item.isActive;
						modelItem.dateUpdate = DateTime.Now;
						modelItem.id_userUpdate = ActiveUser.id;
						if(item.description == null){
							modelItem.description = "";
						}
						else
						{
							modelItem.description = item.description;
						}
							

						#endregion

						db.Setting.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;
						db.SaveChanges();
						trans.Commit();

						TempData["Setting"] = modelItem;
						TempData.Keep("Setting");
						ViewData["EditMessage"] = SuccessMessage("Parámetros: " + modelItem.id + " guardado exitosamente");
					}
					catch (Exception e)
					{
						TempData.Keep("Setting");
						ViewData["EditMessage"] = ErrorMessage(e.Message);

						trans.Rollback();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			TempData.Keep("Setting");


			return PartialView("_SettingPartial", db.Setting.ToList());
		}
		#endregion

		#endregion
		#region Setting Gridview

		[ValidateInput(false)]
		public ActionResult SettingPartial(int? id)
		{
			if (id != null)
			{
				ViewData["SettingToCopy"] = db.Setting.Where(b => b.id == id).FirstOrDefault();
			}
			var model = db.Setting.ToList();
			return PartialView("_SettingPartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult SettingPartialDelete(System.Int32 id)
		{
			if (id >= 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var item = db.Setting.FirstOrDefault(it => it.id == id);
						if (item != null)
						{

							item.id_userUpdate = ActiveUser.id;
							item.dateUpdate = DateTime.Now;

						}
						db.Setting.Attach(item);
						db.Entry(item).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Configuración : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
					}
					catch (Exception)
					{
						ViewData["EditMessage"] = ErrorMessage();
						trans.Rollback();
					}
				}

			}

			var model = db.Setting.ToList();
			return PartialView("_SettingPartial", model.ToList());
		}

		public ActionResult DeleteSelectedSetting(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var Settings = db.Setting.Where(i => ids.Contains(i.id));
						foreach (var vSetting in Settings)
						{
							vSetting.id_userUpdate = ActiveUser.id;
							vSetting.dateUpdate = DateTime.Now;

							db.Setting.Attach(vSetting);
							db.Entry(vSetting).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Parámetros desactivados exitosamente");
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

			var model = db.Setting.ToList();
			return PartialView("_SettingPartial", model.ToList());
		}

		#endregion
	}
}
