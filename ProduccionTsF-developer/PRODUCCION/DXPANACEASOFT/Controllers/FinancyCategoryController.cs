using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
	public class FinancyCategoryController : DefaultController
	{
		public ActionResult Index()
		{
			return View();
		}

		#region FILTERS RESULTS

		[HttpPost]
		public ActionResult FinancyCategoryResults(FinancyCategory FinancyCategory)
		{
			var model = db.FinancyCategory.ToList();

			#region  FILTERS

			#endregion
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_FinancyCategoryResultsPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion
		#region  HEADER
		[HttpPost, ValidateInput(false)]
		public ActionResult FinancyCategoryPartial()
		{
			var model = (TempData["model"] as List<FinancyCategory>);

			model = db.FinancyCategory.ToList();

			model = model ?? new List<FinancyCategory>();
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_FinancyCategoryPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion
		#region Edit FinancyCategory
		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditFinancyCategory(int id, int[] orderDetails)
		{
			FinancyCategory FinancyCategory = db.FinancyCategory.Where(o => o.id == id).FirstOrDefault();

			//if (FinancyCategory == null)
			//{
			//	FinancyCategory = new FinancyCategory
			//	{
			//		id_userUpdate = ActiveUser.id,
			//		dateUpdate = DateTime.Now

			//	};
			//}
			TempData["FinancyCategory"] = FinancyCategory;
			TempData.Keep("FinancyCategory");

			return PartialView("_FormEditFinancyCategory", FinancyCategory);
		}
		#endregion
		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_FinancyCategory)
		{
			TempData.Keep("FinancyCategory");
			int index = db.FinancyCategory.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_FinancyCategory);
			var result = new
			{
				maximunPages = db.FinancyCategory.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}



		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			FinancyCategory FinancyCategory = db.FinancyCategory.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (FinancyCategory != null)
			{
				TempData["FinancyCategory"] = FinancyCategory;
				TempData.Keep("FinancyCategory");
				return PartialView("_FinancyCategoryMainFormPartial", FinancyCategory);
			}

			TempData.Keep("FinancyCategory");

			return PartialView("_FinancyCategoryMainFormPartial", new FinancyCategory());
		}
		#endregion
		#region Validacion
		[HttpPost, ValidateInput(false)]
		public JsonResult ReptCodigo(int id_FinancyCategory, string codigo)
		{
			TempData.Keep("FinancyCategory");


			bool rept = false;

			var cantre = db.FinancyCategory.Where(x => x.id != id_FinancyCategory && x.code == codigo).ToList().Count();
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
		public ActionResult FinancyCategoryPartialAddNew(FinancyCategory item)
		{
			FinancyCategory conFinancyCategory = (TempData["FinancyCategory"] as FinancyCategory);

			DBContext dbemp = new DBContext();

			using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
			{
				try
				{
					#region FinancyCategory

					#endregion

					//item.dateCreate = DateTime.Now;
					//item.dateUpdate = DateTime.Now;

					dbemp.FinancyCategory.Add(item);
					dbemp.SaveChanges();
					trans.Commit();

					TempData["FinancyCategory"] = item;
					TempData.Keep("FinancyCategory");
					ViewData["EditMessage"] = SuccessMessage("Categoría Financiera: " + item.id + " guardada exitosamente");
				}
				catch (Exception e)
				{
					TempData.Keep("FinancyCategory");
					item = (TempData["FinancyCategory"] as FinancyCategory);
					ViewData["EditMessage"] = ErrorMessage(e.Message);

					trans.Rollback();
				}
			}

			//return PartialView("_FinancyCategoryMainFormPartial", item);

			return PartialView("_FinancyCategoryPartial", dbemp.FinancyCategory.ToList());
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult FinancyCategoryPartialUpdate(FinancyCategory item)
		{
			FinancyCategory modelItem = db.FinancyCategory.FirstOrDefault(r => r.id == item.id);
			if (modelItem != null)
			{
				FinancyCategory conFinancyCategory = (TempData["FinancyCategory"] as FinancyCategory);

				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						#region FinancyCategory

						//modelItem.id_userUpdate = ActiveUser.id;
						//modelItem.dateUpdate = DateTime.Now;
						modelItem.code = item.code;
						modelItem.isActive = item.isActive;
						modelItem.description = item.description;
						modelItem.name = item.name;

						#endregion

						db.FinancyCategory.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;
						db.SaveChanges();
						trans.Commit();

						TempData["FinancyCategory"] = modelItem;
						TempData.Keep("FinancyCategory");
						ViewData["EditMessage"] = SuccessMessage("Categoría Financiera: " + modelItem.id + " guardado exitosamente");
					}
					catch (Exception e)
					{
						TempData.Keep("FinancyCategory");
						ViewData["EditMessage"] = ErrorMessage(e.Message);

						trans.Rollback();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			TempData.Keep("FinancyCategory");


			return PartialView("_FinancyCategoryPartial", db.FinancyCategory.ToList());
		}
		#endregion
		#region FinancyCategory Gridview

		[ValidateInput(false)]
		public ActionResult FinancyCategoryPartial(int? id)
		{
			if (id != null)
			{
				ViewData["FinancyCategoryToCopy"] = db.FinancyCategory.Where(b => b.id == id).FirstOrDefault();
			}
			var model = db.FinancyCategory.ToList();
			return PartialView("_FinancyCategoryPartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult FinancyCategoryPartialDelete(System.Int32 id)
		{
			if (id >= 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var item = db.FinancyCategory.FirstOrDefault(it => it.id == id);
						//if (item != null)
						//{

						//	item.id_userUpdate = ActiveUser.id;
						//	item.dateUpdate = DateTime.Now;

						//}
						db.FinancyCategory.Attach(item);
						db.Entry(item).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Categoría Financiera : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
					}
					catch (Exception)
					{
						ViewData["EditMessage"] = ErrorMessage();
						trans.Rollback();
					}
				}

			}

			var model = db.FinancyCategory.ToList();
			return PartialView("_FinancyCategoryPartial", model.ToList());
		}

		public ActionResult DeleteSelectedFinancyCategory(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var FinancyCategorys = db.FinancyCategory.Where(i => ids.Contains(i.id));
						foreach (var vFinancyCategory in FinancyCategorys)
						{
							//vFinancyCategory.id_userUpdate = ActiveUser.id;
							//vFinancyCategory.dateUpdate = DateTime.Now;

							db.FinancyCategory.Attach(vFinancyCategory);
							db.Entry(vFinancyCategory).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Categoría Financiera desactivados exitosamente");
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

			var model = db.FinancyCategory.ToList();
			return PartialView("_FinancyCategoryPartial", model.ToList());
		}

		#endregion
	}
}
