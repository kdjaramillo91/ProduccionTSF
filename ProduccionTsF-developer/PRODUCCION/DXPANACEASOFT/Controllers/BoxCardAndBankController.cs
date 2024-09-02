using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace DXPANACEASOFT.Controllers
{
	public class BoxCardAndBankController : DefaultController
	{
		public ActionResult Index()
		{
			return View();
		}

		#region FILTERS RESULTS

		[HttpPost]
		public ActionResult BoxCardAndBankResults(BoxCardAndBank BoxCardAndBank)
		{
			var model = db.BoxCardAndBank.ToList();

			#region  FILTERS

			#endregion
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_BoxCardAndBankResultsPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion
		#region  HEADER
		[HttpPost, ValidateInput(false)]
		public ActionResult BoxCardAndBankPartial()
		{
			var model = (TempData["model"] as List<BoxCardAndBank>);

			model = db.BoxCardAndBank.ToList();

			model = model ?? new List<BoxCardAndBank>();
			TempData["model"] = model;
			TempData.Keep("model");
			return PartialView("_BoxCardAndBankPartial", model.OrderBy(r => r.id).ToList());
		}
		#endregion
		#region Edit BoxCardAndBank
		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditBoxCardAndBank(int id, int[] orderDetails)
		{
			BoxCardAndBank BoxCardAndBank = db.BoxCardAndBank.Where(o => o.id == id).FirstOrDefault();

			if (BoxCardAndBank == null)
			{
				BoxCardAndBank = new BoxCardAndBank
				{
					id_userUpdate = ActiveUser.id,
					dateUpdate = DateTime.Now

				};
			}
			TempData["BoxCardAndBank"] = BoxCardAndBank;
			TempData.Keep("BoxCardAndBank");

			return PartialView("_FormEditBoxCardAndBank", BoxCardAndBank);
		}
		#endregion
		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_BoxCardAndBank)
		{
			TempData.Keep("BoxCardAndBank");
			int index = db.BoxCardAndBank.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_BoxCardAndBank);
			var result = new
			{
				maximunPages = db.BoxCardAndBank.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}



		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			BoxCardAndBank BoxCardAndBank = db.BoxCardAndBank.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (BoxCardAndBank != null)
			{
				TempData["BoxCardAndBank"] = BoxCardAndBank;
				TempData.Keep("BoxCardAndBank");
				return PartialView("_BoxCardAndBankMainFormPartial", BoxCardAndBank);
			}

			TempData.Keep("BoxCardAndBank");

			return PartialView("_BoxCardAndBankMainFormPartial", new BoxCardAndBank());
		}
		#endregion
		#region Validacion
		[HttpPost, ValidateInput(false)]
		public JsonResult ReptCodigo(int id_BoxCardAndBank, string codigo)
		{
			TempData.Keep("BoxCardAndBank");


			bool rept = false;

			var cantre = db.BoxCardAndBank.Where(x => x.id != id_BoxCardAndBank && x.code == codigo).ToList().Count();
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
		public ActionResult BoxCardAndBankPartialAddNew(BoxCardAndBank item)
		{
			BoxCardAndBank conBoxCardAndBank = (TempData["BoxCardAndBank"] as BoxCardAndBank);

			DBContext dbemp = new DBContext();

			using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
			{
				try
				{
					#region BoxCardAndBank

					#endregion

					item.dateCreate = DateTime.Now;
					item.dateUpdate = DateTime.Now;


					dbemp.BoxCardAndBank.Add(item);
					dbemp.SaveChanges();
					trans.Commit();

					TempData["BoxCardAndBank"] = item;
					TempData.Keep("BoxCardAndBank");
					ViewData["EditMessage"] = SuccessMessage("Banco: " + item.id + " guardada exitosamente");
				}
				catch (DbEntityValidationException dbEx)
				{
					foreach (var validationErrors in dbEx.EntityValidationErrors)
					{
						foreach (var validationError in validationErrors.ValidationErrors)
						{
							Trace.TraceInformation("Property: {0} Error: {1}",
								validationError.PropertyName,
								validationError.ErrorMessage);
						}
					}
					/*TempData.Keep("BoxCardAndBank");
					item = (TempData["BoxCardAndBank"] as BoxCardAndBank);
					ViewData["EditMessage"] = ErrorMessage(e.Message);

					trans.Rollback();*/
				}
			}

			//return PartialView("_BoxCardAndBankMainFormPartial", item);

			return PartialView("_BoxCardAndBankPartial", dbemp.BoxCardAndBank.ToList());
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult BoxCardAndBankPartialUpdate(BoxCardAndBank item)
		{
			BoxCardAndBank modelItem = db.BoxCardAndBank.FirstOrDefault(r => r.id == item.id);
			if (modelItem != null)
			{
				BoxCardAndBank conBoxCardAndBank = (TempData["BoxCardAndBank"] as BoxCardAndBank);

				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						#region BoxCardAndBank

						modelItem.id_userUpdate = ActiveUser.id;
						modelItem.dateUpdate = DateTime.Now;
						modelItem.code = item.code;
						modelItem.isActive = item.isActive;
						modelItem.id_typeBoxCardAndBank = item.id_typeBoxCardAndBank;
						modelItem.name = item.name;
						modelItem.country = item.country;
						modelItem.adress = item.adress;
						modelItem.currency = item.currency;
						modelItem.routing = item.routing;
						modelItem.account = item.account;
                        modelItem.companyName = item.companyName;
                        modelItem.codeIntermediary = item.codeIntermediary;
						modelItem.bankNameIntermediary = item.bankNameIntermediary;
						modelItem.accountIntermediary = item.accountIntermediary;
						modelItem.currencyIntermediary= item.currencyIntermediary;
						modelItem.countryIntermediary = item.countryIntermediary;



						#endregion

						db.BoxCardAndBank.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;
						db.SaveChanges();
						trans.Commit();

						TempData["BoxCardAndBank"] = modelItem;
						TempData.Keep("BoxCardAndBank");
						ViewData["EditMessage"] = SuccessMessage("Banco: " + modelItem.id + " guardado exitosamente");
					}
					catch (Exception e)
					{
						TempData.Keep("BoxCardAndBank");
						ViewData["EditMessage"] = ErrorMessage(e.Message);

						trans.Rollback();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			TempData.Keep("BoxCardAndBank");


			return PartialView("_BoxCardAndBankPartial", db.BoxCardAndBank.ToList());
		}
		#endregion
		#region BoxCardAndBank Gridview

		[ValidateInput(false)]
		public ActionResult BoxCardAndBankPartial(int? id)
		{
			if (id != null)
			{
				ViewData["BoxCardAndBankToCopy"] = db.BoxCardAndBank.Where(b => b.id == id).FirstOrDefault();
			}
			var model = db.BoxCardAndBank.ToList();
			return PartialView("_BoxCardAndBankPartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult BoxCardAndBankPartialDelete(System.Int32 id)
		{
			if (id >= 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var item = db.BoxCardAndBank.FirstOrDefault(it => it.id == id);
						if (item != null)
						{

							item.id_userUpdate = ActiveUser.id;
							item.dateUpdate = DateTime.Now;

						}
						db.BoxCardAndBank.Attach(item);
						db.Entry(item).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Banco : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
					}
					catch (Exception)
					{
						ViewData["EditMessage"] = ErrorMessage();
						trans.Rollback();
					}
				}

			}

			var model = db.BoxCardAndBank.ToList();
			return PartialView("_BoxCardAndBankPartial", model.ToList());
		}

		public ActionResult DeleteSelectedBoxCardAndBank(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var BoxCardAndBanks = db.BoxCardAndBank.Where(i => ids.Contains(i.id));
						foreach (var vBoxCardAndBank in BoxCardAndBanks)
						{
							vBoxCardAndBank.id_userUpdate = ActiveUser.id;
							vBoxCardAndBank.dateUpdate = DateTime.Now;

							db.BoxCardAndBank.Attach(vBoxCardAndBank);
							db.Entry(vBoxCardAndBank).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Banco desactivados exitosamente");
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

			var model = db.BoxCardAndBank.ToList();
			return PartialView("_BoxCardAndBankPartial", model.ToList());
		}

		#endregion
	}
}
