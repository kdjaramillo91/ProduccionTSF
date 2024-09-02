using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
	public class TurnController : DefaultController
	{
		public ActionResult Index()
		{
			return View();
		}

		#region FILTERS RESULTS

		[HttpPost]
		public ActionResult TurnResults(Turn Turn)
		{
			var model = db.Turn.ToList();

			#region  FILTERS

			#endregion
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_TurnResultsPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion
		#region  HEADER
		[HttpPost, ValidateInput(false)]
		public ActionResult TurnPartial()
		{
			var model = (TempData["model"] as List<Turn>);

			model = db.Turn.ToList();

			model = model ?? new List<Turn>();
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_TurnPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion
		#region Edit Turn
		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditTurn(int id, int[] orderDetails)
		{
			Turn Turn = db.Turn.Where(o => o.id == id).FirstOrDefault();

			TempData["Turn"] = Turn;
			TempData.Keep("Turn");

			return PartialView("_FormEditTurn", Turn);
		}
		#endregion
		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_Turn)
		{
			TempData.Keep("Turn");
			int index = db.Turn.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_Turn);
			var result = new
			{
				maximunPages = db.Turn.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}



		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			Turn Turn = db.Turn.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (Turn != null)
			{
				TempData["Turn"] = Turn;
				TempData.Keep("Turn");
				return PartialView("_TurnMainFormPartial", Turn);
			}

			TempData.Keep("Turn");

			return PartialView("_TurnMainFormPartial", new Turn());
		}
		#endregion

		#region Validacion
		[HttpPost, ValidateInput(false)]
		public JsonResult ReptCodigo(int id_Turn, string codigo)
		{
			TempData.Keep("Turn");

			bool rept = false;

			var cantre = db.Turn.Where(x => x.id != id_Turn && x.code == codigo).ToList().Count();
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
		public ActionResult TurnPartialAddNew(Turn item)
		{
			Turn conTurn = (TempData["Turn"] as Turn);

			DBContext dbemp = new DBContext();

			using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
			{
				try
				{
                    #region Turn

                    item.timeInit = TimeSpan.Parse(Request.Params["timeInit1"]);
                    item.timeEnd = TimeSpan.Parse(Request.Params["timeEnd1"]);

                    //TempData["Turn"] = item;
                    //TempData.Keep("Turn");

                    dbemp.Turn.Add(item);
					dbemp.SaveChanges();
					trans.Commit();
             
					ViewData["EditMessage"] = SuccessMessage("Turno: " + item.code + " guardado exitosamente");
					#endregion
				}
				catch (Exception e)
				{
					TempData.Keep("Turn");
					item = (TempData["Turn"] as Turn);
					
					trans.Rollback();
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                }
			}

			return PartialView("_TurnPartial", dbemp.Turn.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult TurnPartialUpdate(Turn item)
		{
			Turn modelItem = db.Turn.FirstOrDefault(r => r.id == item.id);
		    Turn conTurn = (TempData["Turn"] as Turn);

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						#region Turn

						modelItem.code = item.code;
						modelItem.name = item.name;
                        modelItem.timeInit = TimeSpan.Parse(Request.Params["timeInit1"]);
                        modelItem.timeEnd = TimeSpan.Parse(Request.Params["timeEnd1"]);

                        modelItem.isActive = item.isActive;

						#endregion

						db.Turn.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;
						db.SaveChanges();
						trans.Commit();
                        //TempData["Turn"] = modelItem;
						//TempData.Keep("Turn");
						ViewData["EditMessage"] = SuccessMessage("Turno: " + modelItem.code + " guardado exitosamente");
					}
					catch (Exception e)
					{
						//TempData.Keep("Turn");
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
					}
				}

            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }


            return PartialView("_TurnPartial", db.Turn.ToList());
		}
		#endregion
		#region Turn Gridview

		[ValidateInput(false)]
		public ActionResult TurnPartial(int? id)
		{
			if (id != null)
			{
				ViewData["TurnToCopy"] = db.Turn.Where(b => b.id == id).FirstOrDefault();
			}
			var model = db.Turn.ToList();
			return PartialView("_TurnPartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult TurnPartialDelete(System.Int32 id)
		{
			if (id >= 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var item = db.Turn.FirstOrDefault(it => it.id == id);

						db.Turn.Attach(item);
						db.Entry(item).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Turno : " + (item?.id.ToString() ?? "") + " desactivado exitosamente");
					}
					catch (Exception)
					{
						ViewData["EditMessage"] = ErrorMessage();
						trans.Rollback();
					}
				}

			}

			var model = db.Turn.ToList();
			return PartialView("_TurnPartial", model.ToList());
		}

		public ActionResult DeleteSelectedTurn(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var Turns = db.Turn.Where(i => ids.Contains(i.id));
						foreach (var vTurn in Turns)
						{
							db.Turn.Attach(vTurn);
							db.Entry(vTurn).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Turnos desactivados exitosamente");
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

			var model = db.Turn.ToList();
			return PartialView("_TurnPartial", model.ToList());
		}

		#endregion
	}
}
