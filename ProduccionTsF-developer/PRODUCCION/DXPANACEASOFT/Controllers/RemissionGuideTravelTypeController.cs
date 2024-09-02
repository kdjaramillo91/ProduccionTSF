using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Services;
using DevExpress.Web.Mvc;


namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class RemissionGuideTravelTypeController : DefaultController
	{
		// private DBContext db = new DBContext();

		[HttpPost]
		public ActionResult Index()
		{
			return PartialView();
		}

		public ActionResult RemissionGuideTravelTypePartial(int? keyToCopy)
		{
			if (keyToCopy != null)
			{
				ViewData["rowToCopy"] = db.RemissionGuideTravelType.FirstOrDefault(b => b.id == keyToCopy);
			}

			var model = db.RemissionGuideTravelType.Where(fz => fz.id_company == this.ActiveCompanyId);
			return PartialView("_RemissionGuideTravelTypePartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult RemissionGuideTravelTypePartialAddNew(RemissionGuideTravelType RemissionGuideTravelType)
		{
			if (ModelState.IsValid)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						RemissionGuideTravelType.id_company = this.ActiveCompanyId;
						RemissionGuideTravelType.id_userCreate = ActiveUser.id;
						RemissionGuideTravelType.dateCreate = DateTime.Now;
						RemissionGuideTravelType.id_userUpdate = ActiveUser.id;
						RemissionGuideTravelType.dateUpdate = DateTime.Now;

						db.RemissionGuideTravelType.Add(RemissionGuideTravelType);
						db.SaveChanges();
						trans.Commit();

						ViewData["EditMessage"] = SuccessMessage("Tipo de Viaje: " + RemissionGuideTravelType.name + " guardado exitosamente");
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

			var model = db.RemissionGuideTravelType.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_RemissionGuideTravelTypePartial", model.ToList());
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult RemissionGuideTravelTypePartialUpdate(RemissionGuideTravelType RemissionGuideTravelType)
		{
			if (ModelState.IsValid)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var modelRemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(fz => fz.id == RemissionGuideTravelType.id);
						if (modelRemissionGuideTravelType != null)
						{

							modelRemissionGuideTravelType.code = RemissionGuideTravelType.code;
							modelRemissionGuideTravelType.name = RemissionGuideTravelType.name;
							modelRemissionGuideTravelType.description = RemissionGuideTravelType.description;
							modelRemissionGuideTravelType.isActive = RemissionGuideTravelType.isActive;

							modelRemissionGuideTravelType.id_userUpdate = ActiveUser.id;
							modelRemissionGuideTravelType.dateUpdate = DateTime.Now;

							db.RemissionGuideTravelType.Attach(modelRemissionGuideTravelType);
							db.Entry(modelRemissionGuideTravelType).State = EntityState.Modified;

							db.SaveChanges();
							trans.Commit();

							ViewData["EditMessage"] = SuccessMessage("Tipo de Viaje: " + RemissionGuideTravelType.name + " guardado exitosamente");
						}
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

			var model = db.RemissionGuideTravelType.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_RemissionGuideTravelTypePartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult RemissionGuideTravelTypePartialDelete(System.Int32 id)
		{
			if (id >= 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(fz => fz.id == id);
						if (RemissionGuideTravelType != null)
						{
							RemissionGuideTravelType.isActive = false;
							RemissionGuideTravelType.id_userUpdate = ActiveUser.id;
							RemissionGuideTravelType.dateUpdate = DateTime.Now;
						}

						db.RemissionGuideTravelType.Attach(RemissionGuideTravelType);
						db.Entry(RemissionGuideTravelType).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						ViewData["EditMessage"] = SuccessMessage("Tipo de Viaje: " + (RemissionGuideTravelType?.name ?? "") + " desactivado exitosamente");
					}
					catch (Exception)
					{
						ViewData["EditMessage"] = ErrorMessage();
						trans.Rollback();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			var model = db.RemissionGuideTravelType.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_RemissionGuideTravelTypePartial", model.ToList());
		}


		[HttpPost]
		public ActionResult DeleteSelectedRemissionGuideTravelType(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var modelRemissionGuideTravelType = db.RemissionGuideTravelType.Where(i => ids.Contains(i.id));
						foreach (var RemissionGuideTravelType in modelRemissionGuideTravelType)
						{
							RemissionGuideTravelType.isActive = false;

							RemissionGuideTravelType.id_userUpdate = ActiveUser.id;
							RemissionGuideTravelType.dateUpdate = DateTime.Now;

							db.RemissionGuideTravelType.Attach(RemissionGuideTravelType);
							db.Entry(RemissionGuideTravelType).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();

						ViewData["EditMessage"] = SuccessMessage("Tipos de Viaje desactivados exitosamente");
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

			var model = db.RemissionGuideTravelType.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_RemissionGuideTravelTypePartial", model.ToList());
		}



		// GET: RemissionGuideTravelTypes/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RemissionGuideTravelType RemissionGuideTravelType = await db.RemissionGuideTravelType.FindAsync(id);
			if (RemissionGuideTravelType == null)
			{
				return HttpNotFound();
			}
			return View(RemissionGuideTravelType);
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult RemissionGuideTravelTypeGetSites(int id_RemissionGuideTravelType, string ValueField)
		{
			return GridViewExtension.GetComboBoxCallbackResult(p => {
				p.ValueField = ValueField;
				p.BindList(DataProviderFishingSite.FishingSites(this.ActiveCompanyId, id_RemissionGuideTravelType));
			});
		}

		[HttpPost]
		public JsonResult ValidateCodeRemissionGuideTravelType(int id_RemissionGuideTravelType, string code)
		{
			RemissionGuideTravelType RemissionGuideTravelType = db.RemissionGuideTravelType.FirstOrDefault(il => il.code == code);

			if (RemissionGuideTravelType == null || RemissionGuideTravelType.id == id_RemissionGuideTravelType)
			{
				return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
			}

			return Json(new { isValid = false, errorText = "Código en uso por otro Tipo de Viaje" }, JsonRequestBehavior.AllowGet);
		}

	}
}
