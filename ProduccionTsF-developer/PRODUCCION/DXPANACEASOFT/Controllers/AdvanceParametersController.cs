using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class AdvanceParametersController : DefaultController
	{
		public ActionResult Index()
		{
			return View();
		}

		#region FILTERS RESULTS

		[HttpPost]
		public ActionResult AdvanceParametersResults(AdvanceParameters AdvanceParameters)
		{
			var model = db.AdvanceParameters.ToList();

			#region  FILTERS

			#endregion
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_AdvanceParametersResultsPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion

		#region  HEADER
		[HttpPost, ValidateInput(false)]
		public ActionResult AdvanceParametersPartial()
		{
			var model = (TempData["model"] as List<AdvanceParameters>);

			model = db.AdvanceParameters.ToList();

			model = model ?? new List<AdvanceParameters>();
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_AdvanceParametersPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion

		#region Edit AdvanceParameters
		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditAdvanceParameters(int id, int[] orderDetails)
		{
			AdvanceParameters AdvanceParameters = db.AdvanceParameters.Where(o => o.id == id).FirstOrDefault();

			TempData["AdvanceParameters"] = AdvanceParameters;
			TempData.Keep("AdvanceParameters");

			return PartialView("_FormEditAdvanceParameters", AdvanceParameters);
		}
		#endregion

		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_AdvanceParameters)
		{
			TempData.Keep("AdvanceParameters");
			int index = db.AdvanceParameters.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_AdvanceParameters);
			var result = new
			{
				maximunPages = db.AdvanceParameters.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}



		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			AdvanceParameters AdvanceParameters = db.AdvanceParameters.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (AdvanceParameters != null)
			{
				TempData["AdvanceParameters"] = AdvanceParameters;
				TempData.Keep("AdvanceParameters");
				return PartialView("_AdvanceParametersMainFormPartial", AdvanceParameters);
			}

			TempData.Keep("AdvanceParameters");

			return PartialView("_AdvanceParametersMainFormPartial", new AdvanceParameters());
		}
		#endregion

		#region Validacion
		[HttpPost, ValidateInput(false)]
		public JsonResult ReptCodigo(int id_AdvanceParameters, string codigo)
		{
			TempData.Keep("AdvanceParameters");


			bool rept = false;

			var cantre = db.AdvanceParameters.Where(x => x.id != id_AdvanceParameters && x.code == codigo).ToList().Count();
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
		public ActionResult AdvanceParametersPartialAddNew(AdvanceParameters item)
		{
			AdvanceParameters conAdvanceParameters = (TempData["AdvanceParameters"] as AdvanceParameters);

			DBContext dbemp = new DBContext();

			using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
			{
				try
				{
                    item.valueTime = TimeSpan.Parse(Request.Params["valueTime"]);
                    dbemp.AdvanceParameters.Add(item);
					dbemp.SaveChanges();
					trans.Commit();

					TempData["AdvanceParameters"] = item;
					TempData.Keep("AdvanceParameters");
					ViewData["EditMessage"] = SuccessMessage("Parámetros: " + item.id + " guardada exitosamente");
				}
				catch (Exception e)
				{
					TempData.Keep("AdvanceParameters");
					item = (TempData["AdvanceParameters"] as AdvanceParameters);
					ViewData["EditMessage"] = ErrorMessage(e.Message);

					trans.Rollback();
				}
			}

			//return PartialView("_AdvanceParametersMainFormPartial", item);

			return PartialView("_AdvanceParametersPartial", dbemp.AdvanceParameters.ToList());
		}

		#region Update
		[HttpPost, ValidateInput(false)]
		public ActionResult AdvanceParametersPartialUpdate(AdvanceParameters item)
		{
			AdvanceParameters modelItem = db.AdvanceParameters.FirstOrDefault(r => r.id == item.id);
			if (modelItem != null)
			{
				AdvanceParameters conAdvanceParameters = (TempData["AdvanceParameters"] as AdvanceParameters);

				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						#region AdvanceParameters

						modelItem.code = item.code;
						modelItem.description = item.description;
						modelItem.hasDetail = item.hasDetail;
						modelItem.valueInteger = item.valueInteger;
						modelItem.valueDecimal = item.valueDecimal;
						modelItem.valueVarchar = item.valueVarchar;
                        modelItem.valueDate = item.valueDate;
                        modelItem.valueTime = TimeSpan.Parse(Request.Params["valueTime"]);

                        #endregion

                        db.AdvanceParameters.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;
						db.SaveChanges();
						trans.Commit();

						TempData["AdvanceParameters"] = modelItem;
						TempData.Keep("AdvanceParameters");
						ViewData["EditMessage"] = SuccessMessage("Parámetros: " + modelItem.id + " guardado exitosamente");
					}
					catch (Exception e)
					{
						TempData.Keep("AdvanceParameters");
						ViewData["EditMessage"] = ErrorMessage(e.Message);

						trans.Rollback();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			TempData.Keep("AdvanceParameters");


			return PartialView("_AdvanceParametersPartial", db.AdvanceParameters.ToList());
		}
		#endregion

		#endregion
		#region AdvanceParameters Gridview

		[ValidateInput(false)]
		public ActionResult AdvanceParametersPartial(int? id)
		{
			if (id != null)
			{
				ViewData["AdvanceParametersToCopy"] = db.AdvanceParameters.Where(b => b.id == id).FirstOrDefault();
			}
			var model = db.AdvanceParameters.ToList();
			return PartialView("_AdvanceParametersPartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult AdvanceParametersPartialDelete(System.Int32 id)
		{
			if (id >= 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var item = db.AdvanceParameters.FirstOrDefault(it => it.id == id);
						//if (item != null)
						//{

						//	item.id_userUpdate = ActiveUser.id;
						//	item.dateUpdate = DateTime.Now;

						//}
						db.AdvanceParameters.Attach(item);
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

			var model = db.AdvanceParameters.ToList();
			return PartialView("_AdvanceParametersPartial", model.ToList());
		}

		public ActionResult DeleteSelectedAdvanceParameters(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var AdvanceParameterss = db.AdvanceParameters.Where(i => ids.Contains(i.id));
						foreach (var vAdvanceParameters in AdvanceParameterss)
						{
							db.AdvanceParameters.Attach(vAdvanceParameters);
							db.Entry(vAdvanceParameters).State = EntityState.Modified;
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

			var model = db.AdvanceParameters.ToList();
			return PartialView("_AdvanceParametersPartial", model.ToList());
		}

		#endregion
	}
}
