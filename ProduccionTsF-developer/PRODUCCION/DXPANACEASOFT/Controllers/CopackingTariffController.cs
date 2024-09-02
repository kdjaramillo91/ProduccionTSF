using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using DevExpress.Data.Utils;
using DevExpress.Web;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class CopackingTariffController : DefaultController
	{


		[HttpPost]
		public ActionResult Index()
		{
			return PartialView();
		}

		[ValidateInput(false)]
		public ActionResult CopackingTariffPartial()
		{
			CopackingTariff tempCopackingTariff = (TempData["CopackingTariff"] as CopackingTariff);
			if (tempCopackingTariff != null) TempData.Remove("CopackingTariff");

			var model = db.CopackingTariff.Where(o => o.isActive).OrderByDescending(p => p.id);

			return PartialView("_CopackingTariffPartial", model.ToList());
		}


		[HttpPost]
		public ActionResult DeleteSelectedCopackingTariff(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var modelCopackingTariff = db.CopackingTariff.Where(i => ids.Contains(i.id));
						foreach (var CopackingTariff in modelCopackingTariff)
						{
							CopackingTariff.isActive = false;

							CopackingTariff.id_userUpdate = ActiveUser.id;
							CopackingTariff.dateUpdate = DateTime.Now;

							db.CopackingTariff.Attach(CopackingTariff);
							db.Entry(CopackingTariff).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();

						ViewData["EditMessage"] = SuccessMessage("Tarifario de Copacking");
					}
					catch (Exception)
					{
						trans.Rollback();
						TempData.Keep("CopackingTariff");
						TempData.Keep("id_provider");
						ViewData["EditMessage"] = ErrorMessage();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			var model = db.CopackingTariff.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_CopackingTariffPartial", model.ToList());
		}



		[HttpPost, ValidateInput(false)]
		public ActionResult CopackingTariffPartialAddNew(DXPANACEASOFT.Models.CopackingTariff item)
		{
			var model = db.CopackingTariff;
			CopackingTariff tempCopackingTariff = (TempData["CopackingTariff"] as CopackingTariff);
			//Boolean commitOk = false;

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					if (tempCopackingTariff?.CopackingTariffDetail != null)
					{

						item.CopackingTariffDetail = new List<CopackingTariffDetail>();

						var CopackingTariffDetails = tempCopackingTariff.CopackingTariffDetail.ToList();

						foreach (var CopackingTariffDetail in CopackingTariffDetails)
						{
							var tempCopackingTariffDetail = new CopackingTariffDetail
							{

								id_company = CopackingTariffDetail.id_company,
								id_copackingTariff = CopackingTariffDetail.id_copackingTariff,
								id_inventoryLine = CopackingTariffDetail.id_inventoryLine,
								id_productType = CopackingTariffDetail.id_productType,
								tariff = CopackingTariffDetail.tariff,
								orderTariff = CopackingTariffDetail.orderTariff,
								isActive = CopackingTariffDetail.isActive,
								id_userCreate = CopackingTariffDetail.id_userCreate,
								id_userUpdate = CopackingTariffDetail.id_userUpdate,
								dateCreate = CopackingTariffDetail.dateCreate,
								dateUpdate = CopackingTariffDetail.dateUpdate
							};

							item.CopackingTariffDetail.Add(tempCopackingTariffDetail);
						}
					}


					// CAMPOS DE AUDITORIA 
					item.id_userCreate = ActiveUser.id;
					item.dateCreate = DateTime.Now;
					item.id_userUpdate = ActiveUser.id;
					item.dateUpdate = DateTime.Now;

					// FORMA DE QUEMAR EL ID DEL LA COMPANIA
					item.id_company = this.ActiveCompanyId;

					model.Add(item);
					db.SaveChanges();
					trans.Commit();
					//commitOk = true;
					ViewData["EditMessage"] = SuccessMessage("Tarifario agregado exitosamente");
				}
				catch (Exception e)
				{
					TempData.Keep("CopackingTariff");
					TempData.Keep("id_provider");

					ViewData["EditError"] = e.Message;
					trans.Rollback();

				}
			}

			return PartialView("_CopackingTariffPartial", model.OrderByDescending(p => p.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult CopackingTariffPartialUpdate(DXPANACEASOFT.Models.CopackingTariff item)
		{

			var modelItem = db.CopackingTariff.FirstOrDefault(it => it.id == item.id);
			//Boolean commitOk = false;
			CopackingTariff tempCopackingTariff = (TempData["CopackingTariff"] as CopackingTariff);

			if (ModelState.IsValid)
			{

				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						if (modelItem != null)
						{
							if (tempCopackingTariff?.CopackingTariffDetail != null)
							{
								if (tempCopackingTariff.isActive)
								{

									var dbCopackingTariffDetails = modelItem.CopackingTariffDetail.ToList();
									var uptCopackingTariffDetails = tempCopackingTariff.CopackingTariffDetail.ToList();

									foreach (var CopackingTariffDetail in uptCopackingTariffDetails)
									{

										var oriTariffDetail = dbCopackingTariffDetails.FirstOrDefault(r => r.id == CopackingTariffDetail.id);
										if (oriTariffDetail != null)
										{
											oriTariffDetail.id_copackingTariff = CopackingTariffDetail.id_copackingTariff;
											oriTariffDetail.id_inventoryLine = CopackingTariffDetail.id_inventoryLine;
											oriTariffDetail.id_productType = CopackingTariffDetail.id_productType;
											oriTariffDetail.orderTariff = CopackingTariffDetail.orderTariff;
											oriTariffDetail.tariff = CopackingTariffDetail.tariff;
											oriTariffDetail.isActive = CopackingTariffDetail.isActive;
											oriTariffDetail.id_userUpdate = CopackingTariffDetail.id_userUpdate;
											oriTariffDetail.dateUpdate = CopackingTariffDetail.dateUpdate;

										}
										else
										{
											var _tempCopackingTariffDetail = new CopackingTariffDetail
											{
												id_company = CopackingTariffDetail.id_company,
												id_copackingTariff = CopackingTariffDetail.id_copackingTariff,
												id_inventoryLine = CopackingTariffDetail.id_inventoryLine,
												id_productType = CopackingTariffDetail.id_productType,
												tariff = CopackingTariffDetail.tariff,
												orderTariff = CopackingTariffDetail.orderTariff,
												isActive = CopackingTariffDetail.isActive,
												id_userCreate = CopackingTariffDetail.id_userCreate,
												id_userUpdate = CopackingTariffDetail.id_userUpdate,
												dateCreate = CopackingTariffDetail.dateCreate,
												dateUpdate = CopackingTariffDetail.dateUpdate

											};

											modelItem.CopackingTariffDetail.Add(_tempCopackingTariffDetail);
										}

									}
								}
							}
							else
							{
								ViewData["EditMessage"] = WarningMessage("No se Pueden editar los productos de un Tarifario inactivos.");
							}

							modelItem.isActive = item.isActive;
							//    modelItem.id_provider = item.id_provider;
							modelItem.code = item.code;
							modelItem.name = item.name;
							//modelItem.dateInit = item.dateInit;
							//modelItem.dateEnd = item.dateEnd;

							modelItem.id_userUpdate = ActiveUser.id;
							modelItem.dateUpdate = DateTime.Now;

							db.CopackingTariff.Attach(modelItem);
							db.Entry(modelItem).State = EntityState.Modified;

							db.SaveChanges();
							trans.Commit();
							//commitOk = true;
							ViewData["EditMessage"] = SuccessMessage("Tarifario actualizado exitosamente");
						}
					}
					catch (Exception e)
					{
						TempData.Keep("CopackingTariff");
						TempData.Keep("id_provider");

						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}
			else
			{

				ViewData["EditMessage"] = ErrorMessage();

			}

			if (modelItem == null)
			{
				return CopackingTariffPartialAddNew(item);
			}

			return PartialView("_CopackingTariffPartial", db.CopackingTariff.OrderByDescending(m => m.id).ToList());
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult CopackingTariffPartialDelete(System.Int32 id)
		{
			var model = db.CopackingTariff;
			if (id >= 0)
			{
				try
				{
					var item = model.FirstOrDefault(it => it.id == id);
					if (item != null)

						item.isActive = false;
					item.id_userUpdate = ActiveUser.id;
					item.dateUpdate = DateTime.Now;

					db.SaveChanges();
				}
				catch (Exception e)
				{
					TempData.Keep("CopackingTariff");
					TempData.Keep("id_provider");

					ViewData["EditError"] = e.Message;
				}
			}
			return PartialView("_CopackingTariffPartial", model.OrderByDescending(m => m.id).ToList());
		}


		/*  Detalle*/

		[ValidateInput(false)]
		public ActionResult CopackingTariffDetail(int id_copackingTariff, int id_provider)
		{
			CopackingTariff copackingTariff = ObtainCopackingTariff(id_copackingTariff);

			var model = copackingTariff.CopackingTariffDetail?.Where(r => r.isActive).OrderBy(o => o.orderTariff).ToList() ?? new List<CopackingTariffDetail>();

			TempData["id_provider"] = id_provider;
			TempData.Keep("id_provider");

			TempData["copackingTariff"] = TempData["copackingTariff"] ?? copackingTariff;
			TempData.Keep("copackingTariff");

			string _viewPartial = returnViewPartial(id_provider);
			return PartialView(_viewPartial, model);
		}

		/* ADD */
		[HttpPost, ValidateInput(false)]
		public ActionResult CopackingTariffDetailAddNew(int id_CopackingTariff, CopackingTariffDetail CopackingTariffDetail)
		{

			CopackingTariff CopackingTariff = ObtainCopackingTariff(id_CopackingTariff);

			int _id_provider = (int)TempData["id_provider"];

			Boolean _ExistsRecord = (CopackingTariff.CopackingTariffDetail.Where(r => r.id_inventoryLine == CopackingTariffDetail.id_inventoryLine && r.id_productType == CopackingTariffDetail.id_productType && r.isActive).Count() > 0);
			if (_ExistsRecord)
			{
				ViewData["EditError"] = "Se puede registrar ya existente en el tarifario.";
				TempData.Keep("CopackingTariff");
				TempData.Keep("id_provider");
			}
			else
			{
				if (ModelState.IsValid)
				{
					if (CopackingTariff.isActive)
					{
						try
						{
							CopackingTariffDetail.id = CopackingTariff.CopackingTariffDetail.Count() > 0 ? CopackingTariff.CopackingTariffDetail.Max(pld => pld.id) + 1 : 1;

							CopackingTariffDetail.dateCreate = DateTime.Now;
							CopackingTariffDetail.dateUpdate = DateTime.Now;
							CopackingTariffDetail.id_userCreate = ActiveUser.id;
							CopackingTariffDetail.id_userUpdate = ActiveUser.id;
							CopackingTariffDetail.id_company = (int)ViewData["id_company"];

							CopackingTariff.CopackingTariffDetail.Add(CopackingTariffDetail);
							TempData["CopackingTariff"] = CopackingTariff;
							TempData.Keep("CopackingTariff");
							TempData.Keep("id_provider");


						}
						catch (Exception e)
						{
							ViewData["EditError"] = e.Message;
						}
					}
					else
					{
						ViewData["EditError"] = "No puede guardar productos en un tarifario inactivo.";
					}
				}
				else
				{
					ViewData["EditError"] = "Por favor, corrija todos los errores.";
				}
			}


			var model = CopackingTariff.CopackingTariffDetail.Where(r => r.isActive);
			if (model != null)
			{
				model = model.OrderBy(o => (o.InventoryLine?.name != null ? o.InventoryLine.name : "")).ThenBy(o => o.orderTariff).ToList();

			}
			else
			{
				model = new List<CopackingTariffDetail>();
			}

			int id_provider = (int)TempData["id_provider"];
			string _viewPartial = returnViewPartial(id_provider);
			return PartialView(_viewPartial, model);
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult CopackingTariffDetailUpdate(CopackingTariffDetail CopackingTariffDetail)
		{
			CopackingTariff CopackingTariff = ObtainCopackingTariff(CopackingTariffDetail.id_copackingTariff);

			int id_provider = (int)TempData["id_provider"];
			string _viewPartial = returnViewPartial(id_provider);
			List<CopackingTariffDetail> model = CopackingTariffDetailUpdateAlter(CopackingTariff, CopackingTariffDetail, CopackingTariffDetail.id, false);
			TempData.Keep("CopackingTariff");
			TempData.Keep("id_provider");

			return PartialView(_viewPartial, model);

		}


		[HttpPost, ValidateInput(false)]
		public ActionResult CopackingTariffDetailDelete(int id_CopackingTariff, int id)
		{

			CopackingTariff CopackingTariff = ObtainCopackingTariff(id_CopackingTariff);

			int id_provider = (int)TempData["id_provider"];

			string _viewPartial = returnViewPartial(id_provider);

			List<CopackingTariffDetail> model = CopackingTariffDetailUpdateDelete(CopackingTariff, id, true);

			return PartialView(_viewPartial, model);
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult CopackingTariffDetailChangePartial(int id_provider, int id_CopackingTariff)
		{

			CopackingTariff _CopackingTariff = ObtainCopackingTariff(id_CopackingTariff);
			var model = _CopackingTariff.CopackingTariffDetail?.Where(r => r.isActive).ToList() ?? new List<CopackingTariffDetail>();

			string _viewPartial = returnViewPartial(id_provider);
			TempData["CopackingTariff"] = _CopackingTariff;
			TempData.Keep("CopackingTariff");
			TempData["id_provider"] = id_provider;
			TempData.Keep("id_transportTariffType");

			return PartialView(_viewPartial, model);
		}


		// Accion: Fechas inicio o fin no esten inlcuidas en otro rango
		// Accion: Fechas inicio y fin no contenga otro rango
		// Accion: Consistencia de fechas, fecha inicio tiene que ser menor que fecha final
		[HttpPost]
		public JsonResult CopackingTarrifValidateConsistentDate(int id_CopackingTariff, DateTime FechaIni, DateTime FechaFin, int id_proveedor)
		{

			string msgerr = "";
			Boolean isValidValue = true;

			int numTTDateInconsistent = db.CopackingTariff.Where(d => ((d.dateInit <= FechaFin && d.dateEnd >= FechaIni) && d.isActive && d.id_company == this.ActiveCompanyId) && (d.id_provider == id_proveedor) && d.id != id_CopackingTariff).Count();
			// dateInit <=  '30/11/2017' and dateEnd >= '01/11/2017'

			if (numTTDateInconsistent > 0)
			{
				isValidValue = false;
				msgerr = "Rango de fechas presente en otro Tarifario";
			}

			if (FechaIni > FechaFin)
			{

				isValidValue = false;
				msgerr = "Rango de fechas invalido Fecha Inicial es mayor que Fecha Final";
			}

			TempData.Keep("CopackingTariff");
			TempData.Keep("id_provider");


			//return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
			return Json(new { isValid = isValidValue, errorText = msgerr }, JsonRequestBehavior.AllowGet);
		}

		#region return view partial
		private string returnViewPartial(int id_provider)
		{

			string _returnViewPartial = "";

			if (id_provider == 0)
			{

				_returnViewPartial = "_CopackingTariffDetailPartial";
			}
			else
			{
				_returnViewPartial = "_CopackingTariffDetailPartial";
			}

			return _returnViewPartial;
		}
		#endregion

		private List<CopackingTariffDetail> CopackingTariffDetailUpdateAlter(CopackingTariff CopackingTariff, CopackingTariffDetail copackingTariffDetail, int id_CopackingTariffDetail, Boolean isDelete)
		{
			if (ModelState.IsValid && CopackingTariff != null && copackingTariffDetail != null)
			{
				if (CopackingTariff.isActive)
				{
					try
					{
						var modelCopackingTariffDetail = CopackingTariff.CopackingTariffDetail.FirstOrDefault(i => i.id == id_CopackingTariffDetail);
						if (modelCopackingTariffDetail != null)
						{
							modelCopackingTariffDetail.orderTariff = copackingTariffDetail.orderTariff;
							modelCopackingTariffDetail.id_userUpdate = ActiveUser.id;
							modelCopackingTariffDetail.dateUpdate = DateTime.Now;
							if (isDelete) modelCopackingTariffDetail.isActive = false;
							this.UpdateModel(modelCopackingTariffDetail);
						}

						TempData["CopackingTariff"] = CopackingTariff;
						TempData.Keep("id_provider");

					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
					}
				}
				else
				{
					ViewData["EditError"] = "No puede editar productos de un tarifario Inactivo.";
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";

			TempData.Keep("CopackingTariff");
			TempData.Keep("id_provider");

			List<CopackingTariffDetail> model = CopackingTariff.CopackingTariffDetail?.Where(x => x.isActive).OrderBy(o => o.InventoryLine.name).ThenBy(o => o.orderTariff).ToList() ?? new List<CopackingTariffDetail>();
			model = (model.Count() != 0) ? model : new List<CopackingTariffDetail>();

			return model;
		}

		private List<CopackingTariffDetail> CopackingTariffDetailUpdateDelete(CopackingTariff CopackingTariff, int id_CopackingTariffDetail, Boolean isDelete)
		{
			if (ModelState.IsValid && CopackingTariff != null)
			{
				if (CopackingTariff.isActive)
				{
					try
					{
						var modelCopackingTariffDetail = CopackingTariff.CopackingTariffDetail.FirstOrDefault(i => i.id == id_CopackingTariffDetail);
						if (modelCopackingTariffDetail != null)
						{
							modelCopackingTariffDetail.id_userUpdate = ActiveUser.id;
							modelCopackingTariffDetail.dateUpdate = DateTime.Now;
							if (isDelete) modelCopackingTariffDetail.isActive = false;
							this.UpdateModel(modelCopackingTariffDetail);
						}

						TempData["CopackingTariff"] = CopackingTariff;
						TempData.Keep("id_provider");

					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
					}
				}
				else
				{
					ViewData["EditError"] = "No puede editar productos de un tarifario Inactivo.";
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";

			TempData.Keep("CopackingTariff");
			TempData.Keep("id_provider");

			List<CopackingTariffDetail> model = CopackingTariff.CopackingTariffDetail?.Where(x => x.isActive).OrderBy(o => o.InventoryLine.name).ThenBy(o => o.orderTariff).ToList() ?? new List<CopackingTariffDetail>();
			model = (model.Count() != 0) ? model : new List<CopackingTariffDetail>();

			return model;
		}

		private CopackingTariff ObtainCopackingTariff(int? id_CopackingTariff)
		{

			CopackingTariff CopackingTariff = (TempData["CopackingTariff"] as CopackingTariff);

			CopackingTariff = CopackingTariff ?? db.CopackingTariff.FirstOrDefault(i => i.id == id_CopackingTariff);

			CopackingTariff = CopackingTariff ?? new CopackingTariff();

			if (CopackingTariff.id == 0)
				CopackingTariff.isActive = true;

			return CopackingTariff;

		}

		private JsonResult CopackingTariffDetailOrderUpdate(int id_CopackingTariff, int id_CopackingTariffDetail, int UpDown)
		{

			CopackingTariff _CopackingTariff = ObtainCopackingTariff(id_CopackingTariff);

			if (ModelState.IsValid && _CopackingTariff != null)
			{
				try
				{
					var modelCopackingTariffDetail = _CopackingTariff.CopackingTariffDetail.Where(r => r.id_copackingTariff == id_CopackingTariff);
					if (modelCopackingTariffDetail != null)
					{
						CopackingTariffDetail _currentCopackingTariffDetail = modelCopackingTariffDetail.FirstOrDefault(r => r.id == id_CopackingTariffDetail);
						int _maxRecords = modelCopackingTariffDetail.Count();
						int _originOrder = _currentCopackingTariffDetail.orderTariff;
						int _updateOrder = _originOrder + UpDown;
						CopackingTariffDetail _DestinyCopackingTariffDetail = modelCopackingTariffDetail.FirstOrDefault(r => r.orderTariff == _updateOrder);

						if (_originOrder == 1) return Json(new { isValid = false, errorText = "El orden no puede ser menor a uno" }, JsonRequestBehavior.AllowGet);
						if (_originOrder == _maxRecords) return Json(new { isValid = false, errorText = "El orden no puede ser mayor a " + _maxRecords }, JsonRequestBehavior.AllowGet);
						if (_DestinyCopackingTariffDetail != null)
						{
							_DestinyCopackingTariffDetail.orderTariff = _originOrder;
							_DestinyCopackingTariffDetail.id_userUpdate = ActiveUser.id;
							_DestinyCopackingTariffDetail.dateUpdate = DateTime.Now;
						}

						if (_currentCopackingTariffDetail != null)
						{
							_currentCopackingTariffDetail.orderTariff = _updateOrder;
							_currentCopackingTariffDetail.id_userUpdate = ActiveUser.id;
							_currentCopackingTariffDetail.dateUpdate = DateTime.Now;
						}


						this.UpdateModel(modelCopackingTariffDetail);
					}

					TempData["CopackingTariff"] = _CopackingTariff;
					TempData.Keep("id_provider");
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";


			TempData.Keep("CopackingTariff");
			TempData.Keep("id_provider");

			return Json(new { isValid = true, errorText = "Orden actualizado" }, JsonRequestBehavior.AllowGet);

		}

		private int setOrderUpdate(int id_CopackingTariff, CopackingTariffDetail CopackingTariffDetail)
		{

			int _orderTariff = 1;

			// obtener el tipo de tarifario
			// buscar fishing Site
			// tamaño o rango hielo

			CopackingTariff _CopackingTariff = ObtainCopackingTariff(id_CopackingTariff);

			Boolean _isTerrestriel = false;
			//Boolean _isInternal = false;

			if (_isTerrestriel)
			{
				_orderTariff = 1;
			}

			return _orderTariff;
		}

		public JsonResult ObtainInfoProvider(int id_provider)
		{

			var JsonReturn = new { isInternal = "false", isTerrestriel = "false" };

			Person _Person = db.Person.FirstOrDefault(r => r.id == id_provider);


			TempData.Keep("CopackingTariff");
			TempData.Keep("id_provider");

			return Json(JsonReturn, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult ItemTypesByInventoryLine(int? id_inventoryLine)
		{
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			var model = db.ItemType.Where(t => t.id_inventoryLine == id_inventoryLine).Select(t => new { t.id, t.name }).ToList();
			TempData.Keep("item");
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		public JsonResult InventoryLineIngredientItemInit(int? id_inventoryLine, int? id_itemType)
		{
			UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
			//inventoryLines
			var inventoryLines = db.InventoryLine.Where(t => t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_inventoryLine)).Select(t => new { t.id, t.name }).ToList();

			//itemTypes
			var itemTypes = db.ItemType.Where(t => t.id_inventoryLine == id_inventoryLine && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_itemType)).Select(t => new { t.id, t.name }).ToList();

			var result = new
			{
				inventoryLines = inventoryLines,
				itemTypes = itemTypes,
			};

			TempData.Keep("item");
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult ReptCodigo(int id_CopackingTariff, string codigo)
		{
			TempData.Keep("CopackingTariff");

			bool rept = false;

			var cantre = db.CopackingTariff.Where(x => x.id != id_CopackingTariff && x.code == codigo).ToList().Count();
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

		[HttpPost, ValidateInput(false)]
		public JsonResult PrintCopackingReport(int id_copacking, string codeReport)
		{
			List<ParamCR> paramLst = new List<ParamCR>();

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			if (id_copacking != 0 && codeReport == "TRCKI")
			{
				ParamCR _param = new ParamCR
				{
					Nombre = "@id",
					Valor = id_copacking
				};

				paramLst.Add(_param);
			}

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = codeReport;
			_repMod.conex = objConex;
			_repMod.paramCRList = paramLst;

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public ActionResult CopackingTariffCopy(int id)
		{
			CopackingTariff copackingTariff = db.CopackingTariff.FirstOrDefault(o => o.id == id);
			if (copackingTariff == null)
				copackingTariff = new CopackingTariff();

			copackingTariff.CopackingTariffDetail = db.CopackingTariffDetail.Where(o => o.id_copackingTariff == id).ToList();
			if (copackingTariff.CopackingTariffDetail == null)
				copackingTariff.CopackingTariffDetail = new List<CopackingTariffDetail>();

			//copackingTariff.id = 0;
			copackingTariff.code = "";
			copackingTariff.name = "";
			TempData["copackingTariff"] = copackingTariff;
			TempData.Keep("copackingTariff");

			return PartialView("_CopackingTariffFormEditPartial", copackingTariff);
		}

	}
}
