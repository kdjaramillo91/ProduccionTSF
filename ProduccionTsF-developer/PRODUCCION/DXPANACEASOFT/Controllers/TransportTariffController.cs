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
	public class TransportTariffController : DefaultController
	{


		[HttpPost]
		public ActionResult Index()
		{
			return PartialView();
		}

		[ValidateInput(false)]
		public ActionResult TransportTariffPartial()
		{
			TransportTariff tempTransportTariff = (TempData["transportTariff"] as TransportTariff);
			if (tempTransportTariff != null) TempData.Remove("transportTariff");


			int? _id_transportTariffType = (int?)TempData["id_transportTariffType"];
			if (_id_transportTariffType != null || _id_transportTariffType != 0) TempData.Remove("id_transportTariffType");


			var model = db.TransportTariff.OrderByDescending(p => p.id);

			return PartialView("_TransportTariffPartial", model.ToList());
		}


		[HttpPost]
		public ActionResult DeleteSelectedTransportTariff(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var modelTransportTariff = db.TransportTariff.Where(i => ids.Contains(i.id));
						foreach (var transportTariff in modelTransportTariff)
						{
							transportTariff.isActive = false;

							transportTariff.id_userUpdate = ActiveUser.id;
							transportTariff.dateUpdate = DateTime.Now;

							db.TransportTariff.Attach(transportTariff);
							db.Entry(transportTariff).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();

						ViewData["EditMessage"] = SuccessMessage("Tarifario de Transporte");
					}
					catch (Exception)
					{
						trans.Rollback();
						TempData.Keep("transportTariff");
						TempData.Keep("id_transportTariffType");
						ViewData["EditMessage"] = ErrorMessage();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			var model = db.TransportTariff.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_TransportTariffPartial", model.ToList());
		}



		[HttpPost, ValidateInput(false)]
		public ActionResult TransportTariffPartialAddNew(DXPANACEASOFT.Models.TransportTariff item)
		{
			var model = db.TransportTariff;
			TransportTariff tempTransportTariff = (TempData["transportTariff"] as TransportTariff);
			Boolean commitOk = false;

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{





					if (tempTransportTariff?.TransportTariffDetail != null)
					{

						item.TransportTariffDetail = new List<TransportTariffDetail>();
						// tempTransportTariff.TransportTariffDetail = new List<TransportTariffDetail>();

						var transportTariffDetails = tempTransportTariff.TransportTariffDetail.ToList();

						foreach (var transportTariffDetail in transportTariffDetails)
						{
							var temptransportTariffDetail = new TransportTariffDetail
							{

								id_company = transportTariffDetail.id_company,
								id_FishingSite = transportTariffDetail.id_FishingSite,
								id_TransportSize = transportTariffDetail.id_TransportSize,
								id_IceBagRange = transportTariffDetail.id_IceBagRange,
								tariff = transportTariffDetail.tariff,
								orderTariff = transportTariffDetail.orderTariff,
								isActive = transportTariffDetail.isActive,
								id_userCreate = transportTariffDetail.id_userCreate,
								id_userUpdate = transportTariffDetail.id_userUpdate,
								dateCreate = transportTariffDetail.dateCreate,
								dateUpdate = transportTariffDetail.dateUpdate
							};

							item.TransportTariffDetail.Add(temptransportTariffDetail);
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
					commitOk = true;
					ViewData["EditMessage"] = SuccessMessage("Tarifario agregado exitosamente");
				}
				catch (Exception e)
				{
					TempData.Keep("transportTariff");
					TempData.Keep("id_transportTariffType");

					ViewData["EditError"] = e.Message;
					trans.Rollback();

				}
			}
			//}
			//else
			//    ViewData["EditError"] = "Please, correct all errors.";

			if (commitOk) setOrderTransportTariffDetail(tempTransportTariff.id);


			return PartialView("_TransportTariffPartial", model.OrderByDescending(p => p.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult TransportTariffPartialUpdate(DXPANACEASOFT.Models.TransportTariff item)
		{

			var modelItem = db.TransportTariff.FirstOrDefault(it => it.id == item.id);
			Boolean commitOk = false;
			TransportTariff tempTransportTariff = (TempData["transportTariff"] as TransportTariff);

			if (ModelState.IsValid)
			{

				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{

					try
					{
						if (modelItem != null)
						{


							/* No eliminar detalle, trazabilidad+ */
							if (tempTransportTariff?.TransportTariffDetail != null)
							{

								var dbtransportTariffDetails = modelItem.TransportTariffDetail.ToList();
								var upttransportTariffDetails = tempTransportTariff.TransportTariffDetail.ToList();

								foreach (var transportTariffDetail in upttransportTariffDetails)
								{

									var oriTariffDetail = dbtransportTariffDetails.FirstOrDefault(r => r.id == transportTariffDetail.id);
									if (oriTariffDetail != null)
									{
										oriTariffDetail.id_FishingSite = transportTariffDetail.id_FishingSite;
										oriTariffDetail.id_IceBagRange = transportTariffDetail.id_IceBagRange;
										oriTariffDetail.id_TransportSize = transportTariffDetail.id_TransportSize;
										oriTariffDetail.orderTariff = transportTariffDetail.orderTariff;
										oriTariffDetail.tariff = transportTariffDetail.tariff;
										oriTariffDetail.isActive = transportTariffDetail.isActive;
										oriTariffDetail.id_userUpdate = transportTariffDetail.id_userUpdate;
										oriTariffDetail.dateUpdate = transportTariffDetail.dateUpdate;

									}
									else
									{
										var _tempTransportTariffDetail = new TransportTariffDetail
										{
											id_company = transportTariffDetail.id_company,
											id_FishingSite = transportTariffDetail.id_FishingSite,
											id_TransportSize = transportTariffDetail.id_TransportSize,
											id_IceBagRange = transportTariffDetail.id_IceBagRange,
											tariff = transportTariffDetail.tariff,
											orderTariff = transportTariffDetail.orderTariff,
											isActive = transportTariffDetail.isActive,
											id_userCreate = transportTariffDetail.id_userCreate,
											id_userUpdate = transportTariffDetail.id_userUpdate,
											dateCreate = transportTariffDetail.dateCreate,
											dateUpdate = transportTariffDetail.dateUpdate

										};

										modelItem.TransportTariffDetail.Add(_tempTransportTariffDetail);
									}

								}

							}

							modelItem.isActive = item.isActive;
							//    modelItem.id_TransportTariffType = item.id_TransportTariffType;
							modelItem.code = item.code;
							modelItem.name = item.name;
							//modelItem.dateInit = item.dateInit;
							//modelItem.dateEnd = item.dateEnd;

							modelItem.id_userUpdate = ActiveUser.id;
							modelItem.dateUpdate = DateTime.Now;

							db.TransportTariff.Attach(modelItem);
							db.Entry(modelItem).State = EntityState.Modified;

							db.SaveChanges();
							trans.Commit();
							commitOk = true;
							ViewData["EditMessage"] = SuccessMessage("Tarifario actualizado exitosamente");

							//TempData["transportTariff"] = modelItem;
							//TempData.Keep("transportTariff");
						}
					}
					catch (Exception e)
					{
						TempData.Keep("transportTariff");
						TempData.Keep("id_transportTariffType");

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
				return TransportTariffPartialAddNew(item);
			}
			else
			{

				if (commitOk) setOrderTransportTariffDetail(item.id);
			}

			return PartialView("_TransportTariffPartial", db.TransportTariff.OrderByDescending(m => m.id).ToList());
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult TransportTariffPartialDelete(System.Int32 id)
		{
			var model = db.TransportTariff;
			if (id >= 0)
			{
				try
				{
					var item = model.FirstOrDefault(it => it.id == id);
					if (item != null)

						item.isActive = false;
					item.id_userUpdate = ActiveUser.id;
					item.dateUpdate = DateTime.Now;

					//model.Remove(item);
					db.SaveChanges();
				}
				catch (Exception e)
				{
					TempData.Keep("transportTariff");
					TempData.Keep("id_transportTariffType");

					ViewData["EditError"] = e.Message;
				}
			}
			return PartialView("_TransportTariffPartial", model.OrderByDescending(m => m.id).ToList());
		}


		/*  Detalle*/

		[ValidateInput(false)]
		public ActionResult TransportTariffDetail(int id_transportTariff, int id_transportTariffType)
		{


			TransportTariff transportTariff = ObtainTransportTariff(id_transportTariff);

			var model = transportTariff.TransportTariffDetail?.Where(r => r.isActive).OrderBy(o => o.orderTariff).ToList() ?? new List<TransportTariffDetail>();

			TempData["id_transportTariffType"] = id_transportTariffType;
			TempData.Keep("id_transportTariffType");

			TempData["transportTariff"] = TempData["transportTariff"] ?? transportTariff;
			TempData.Keep("transportTariff");

			string _viewPartial = returnViewPartial(id_transportTariffType);
			return PartialView(_viewPartial, model);
		}

		/* ADD */
		[HttpPost, ValidateInput(false)]
		public ActionResult TransportTariffDetailAddNew(int id_transportTariff, TransportTariffDetail transportTariffDetail)
		{

			TransportTariff transportTariff = ObtainTransportTariff(id_transportTariff);

			int _id_transportTariffType = (int)TempData["id_transportTariffType"];
			Boolean _IsMaritimeTariff = IsMaritimeTariff(_id_transportTariffType);
			Boolean _ExistsRecord = (transportTariff.TransportTariffDetail.Where(r => r.id_FishingSite == transportTariffDetail.id_FishingSite && r.isActive).Count() > 0);
			if (_IsMaritimeTariff && _ExistsRecord)
			{
				ViewData["EditError"] = "Se puede insertar una vez el sitio seleccionado";
				TempData.Keep("transportTariff");
				TempData.Keep("id_transportTariffType");
			}
			else
			{
				if (ModelState.IsValid)
				{
					try
					{
						transportTariffDetail.id = transportTariff.TransportTariffDetail.Count() > 0 ? transportTariff.TransportTariffDetail.Max(pld => pld.id) + 1 : 1;

						transportTariffDetail.dateCreate = DateTime.Now;
						transportTariffDetail.dateUpdate = DateTime.Now;
						transportTariffDetail.id_userCreate = ActiveUser.id;
						transportTariffDetail.id_userUpdate = ActiveUser.id;
						transportTariffDetail.id_company = (int)ViewData["id_company"];
						//transportTariffDetail.orderTariff = 0;

						transportTariff.TransportTariffDetail.Add(transportTariffDetail);
						TempData["transportTariff"] = transportTariff;
						TempData.Keep("transportTariff");
						TempData.Keep("id_transportTariffType");


					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
					}
				}
				else
				{
					ViewData["EditError"] = "Por favor, corrija todos los errores.";
				}
			}


			var model = transportTariff.TransportTariffDetail.Where(r => r.isActive);
			if (model != null)
			{
				model = model.OrderBy(o => (o.FishingSite?.name != null ? o.FishingSite.name : "")).ThenBy(o => o.orderTariff).ToList();

			}
			else
			{
				model = new List<TransportTariffDetail>();
			}

			int id_transportTariffType = (int)TempData["id_transportTariffType"];
			string _viewPartial = returnViewPartial(id_transportTariffType);
			return PartialView(_viewPartial, model);
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult TransportTariffDetailUpdate(TransportTariffDetail transportTariffDetail)
		{

			TransportTariff transportTariff = ObtainTransportTariff(transportTariffDetail.id_TransportTariff);

			int id_transportTariffType = (int)TempData["id_transportTariffType"];
			string _viewPartial = returnViewPartial(id_transportTariffType);


			List<TransportTariffDetail> model = TransportTariffDetailUpdateDelete(transportTariff, transportTariffDetail.id, false);
			TempData.Keep("transportTariff");
			TempData.Keep("id_transportTariffType");

			return PartialView(_viewPartial, model);

		}


		[HttpPost, ValidateInput(false)]
		public ActionResult TransportTariffDetailDelete(int id_transportTariff, int id)
		{

			TransportTariff transportTariff = ObtainTransportTariff(id_transportTariff);

			int id_transportTariffType = (int)TempData["id_transportTariffType"];

			string _viewPartial = returnViewPartial(id_transportTariffType);

			List<TransportTariffDetail> model = TransportTariffDetailUpdateDelete(transportTariff, id, true);

			return PartialView(_viewPartial, model);
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult TransportTariffDetailChangePartial(int id_transportTariffType, int id_transportTariff)
		{

			TransportTariff _transportTariff = ObtainTransportTariff(id_transportTariff);
			var model = _transportTariff.TransportTariffDetail?.Where(r => r.isActive).ToList() ?? new List<TransportTariffDetail>();



			string _viewPartial = returnViewPartial(id_transportTariffType);
			TempData["transportTariff"] = _transportTariff;
			TempData.Keep("transportTariff");
			TempData["id_transportTariffType"] = id_transportTariffType;
			TempData.Keep("id_transportTariffType");

			return PartialView(_viewPartial, model);
		}


		// Accion: Fechas inicio o fin no esten inlcuidas en otro rango
		// Accion: Fechas inicio y fin no contenga otro rango
		// Accion: Consistencia de fechas, fecha inicio tiene que ser menor que fecha final
		[HttpPost]
		public JsonResult TransportTarrifValidateConsistentDate(int id_TrasportTariff, DateTime FechaIni, DateTime FechaFin, int id_tipoTarifario)
		{

			string msgerr = "";
			Boolean isValidValue = true;

			int numTTDateInconsistent = db.TransportTariff.Where(d => ((d.dateInit <= FechaFin && d.dateEnd >= FechaIni) && d.isActive && d.id_company == this.ActiveCompanyId && d.id_TransportTariffType == id_tipoTarifario) && d.id != id_TrasportTariff).Count();
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

			TempData.Keep("transportTariff");
			TempData.Keep("id_transportTariffType");


			//return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
			return Json(new { isValid = isValidValue, errorText = msgerr }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult TransportTarrifTypeSingleton(int? id_transportTariff, int id_transportTariffType)
		{


			string msgerr = "";
			Boolean isValidValue = true;
			string nameTransportTariffType = db.TransportTariffType.FirstOrDefault(r => r.id == id_transportTariffType).name;
			int _ttSingletonCount = db.TransportTariff.Where(d => d.isActive && d.id_company == this.ActiveCompanyId && d.id_TransportTariffType == id_transportTariffType && (d.id != (id_transportTariff ?? 0))).Count();

			if (_ttSingletonCount > 0)
			{
				isValidValue = false;
				msgerr = "Solo puede haber un Tarifario con Tipo de Tarifario: " + nameTransportTariffType;
			}

			TempData.Keep("transportTariff");
			TempData.Keep("id_transportTariffType");

			return Json(new { isValid = isValidValue, errorText = msgerr }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult ObtainInfoTransportType(int id_transportTariffType)
		{

			var JsonReturn = new { isInternal = "false", isTerrestriel = "true" };

			TransportTariffType _TransportTariffType = db.TransportTariffType.FirstOrDefault(r => r.id == id_transportTariffType);
			if (_TransportTariffType != null)
			{
				bool _isInternal = _TransportTariffType.isInternal;
				bool _isTerrestriel = _TransportTariffType.PurchaseOrderShippingType.isTerrestriel;
				JsonReturn = new { isInternal = _isInternal ? "true" : "false", isTerrestriel = _isTerrestriel ? "true" : "false" };
			}

			TempData.Keep("transportTariff");
			TempData.Keep("id_transportTariffType");

			return Json(JsonReturn, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public JsonResult TransportTarrifFishingSiteSingleton(int? id_transportTariff, int? id_transportTariffDetail, int id_fishingSite)
		{

			string msgerr = "";
			Boolean isValidValue = true;


			TransportTariff _transportTariff = ObtainTransportTariff(id_transportTariff);
			string nameFishingSite = db.FishingSite.FirstOrDefault(r => r.id == id_fishingSite).name;

			int _ttSingletonCount = _transportTariff.TransportTariffDetail.Where(d => d.isActive && d.id_FishingSite == id_fishingSite && (d.id != (id_transportTariffDetail ?? 0))).Count();
			// dateInit <=  '30/11/2017' and dateEnd >= '01/11/2017'

			if (_ttSingletonCount > 0)
			{
				isValidValue = false;
				msgerr = "Solo puede haber un Sitio de Pesca : " + nameFishingSite + " por Tarifario.";
			}

			TempData.Keep("transportTariff");
			TempData.Keep("id_transportTariffType");

			return Json(new { isValid = isValidValue, errorText = msgerr }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult FishingSiteTransportSizeSingleton(int id_transportTariff, int id_fishingSite, int id_transportSize, int? id_transportTariffDetail)
		{

			var JsonReturn = new { isValid = true, errorText = "" };

			TransportTariff _transportTariff = ObtainTransportTariff(id_transportTariff);


			int countFishingSiteTransportSize = _transportTariff.TransportTariffDetail.Where(r => (r.isActive && r.id_FishingSite == id_fishingSite && r.id_TransportSize == id_transportSize) && r.id != (id_transportTariffDetail ?? 0)).Count();
			if (countFishingSiteTransportSize > 0) JsonReturn = new { isValid = false, errorText = "Tamaño de transporte ya definido para Sitio de Pesca" };

			TempData.Keep("transportTariff");
			TempData.Keep("id_transportTariffType");

			return Json(JsonReturn, JsonRequestBehavior.AllowGet);

		}

		[HttpPost]
		public JsonResult FishingSiteIceBagRangeSingleton(int id_transportTariff, int id_fishingSite, int id_iceBagRange, int? id_transportTariffDetail)
		{

			var JsonReturn = new { isValid = true, errorText = "" };

			TransportTariff _transportTariff = ObtainTransportTariff(id_transportTariff);

			int countFishingSiteIceBagRange = _transportTariff.TransportTariffDetail.Where(r => (r.isActive && r.id_FishingSite == id_fishingSite && r.id_IceBagRange == id_iceBagRange) && r.id != (id_transportTariffDetail ?? 0)).Count();
			if (countFishingSiteIceBagRange > 0) JsonReturn = new { isValid = false, errorText = "Rango de Hielo ya definido para Sitio de Pesca" };

			TempData.Keep("transportTariff");
			TempData.Keep("id_transportTariffType");

			return Json(JsonReturn, JsonRequestBehavior.AllowGet);

		}

		[HttpPost]
		public JsonResult GetDinamycComboData(int? id_transportTariff, int? id_transportTariffDetail, int id_transportTariffType, int id_fishingSite)
		{


			//var xresult = new { isValid = true, errorText = "" };

			TransportTariff _transportTariff = ObtainTransportTariff(id_transportTariff);
			Boolean isInternal = db.TransportTariffType.Where(r => r.id == id_transportTariffType).FirstOrDefault().isInternal;
			Boolean isTerrestriel = db.TransportTariffType.Where(r => r.id == id_transportTariffType).FirstOrDefault().PurchaseOrderShippingType.isTerrestriel;


			//  tariff detail : si no existe es un registro nuevo 
			//  todos los que no esten en el mismo detalle 

			// si el tarifario es nuevo no considerar :: 


			if (isInternal)
			{
				var _objIceBag = new List<IceBagRange>();
				// registro nuevo
				if (_transportTariff?.TransportTariffDetail == null || _transportTariff?.TransportTariffDetail.Count() == 0)
				{
					_objIceBag = db.IceBagRange.Where(r => r.isActive && r.id_company == this.ActiveCompanyId).ToList();
				}
				else
				{

					var _iceBagRangeInUse = _transportTariff.TransportTariffDetail.Where(r => r.isActive && r.id_FishingSite == id_fishingSite && r.id != ((id_transportTariffDetail == null) ? 0 : id_transportTariffDetail)).Select(s => s.id_IceBagRange).ToList();

					_objIceBag = db.IceBagRange.Where(r => r.isActive && r.id_company == this.ActiveCompanyId && !(_iceBagRangeInUse.Contains(r.id))).ToList();

				}

				var result = new
				{
					_dynamicdata = _objIceBag.Select
						(fs => new { id = fs.id, name = fs.name, range_ini = fs.range_ini, range_end = fs.range_end }).ToList(),

				};
				TempData.Keep("transportTariff");
				TempData.Keep("id_transportTariffType");

				return Json(result, JsonRequestBehavior.AllowGet);


			}
			else
			{
				var _objTransportSize = new List<TransportSize>();
				// registro nuevo
				if (_transportTariff?.TransportTariffDetail == null || _transportTariff?.TransportTariffDetail.Count() == 0)
				{
					_objTransportSize = db.TransportSize.Where(r => r.isActive && r.id_company == this.ActiveCompanyId).ToList();
				}
				else
				{

					var _transportSizeInUse = _transportTariff.TransportTariffDetail.Where(r => r.isActive && r.id_FishingSite == id_fishingSite && r.id != ((id_transportTariffDetail == null) ? 0 : id_transportTariffDetail)).Select(s => s.id_TransportSize).ToList();

					_objTransportSize = db.TransportSize.Where(r => r.isActive && r.id_company == this.ActiveCompanyId && !(_transportSizeInUse.Contains(r.id))).ToList();

				}

				var result = new
				{
					_dynamicdata = _objTransportSize.Select
						(fs => new { id = fs.id, code = fs.code, name = fs.name }).ToList(),

				};
				TempData.Keep("transportTariff");
				TempData.Keep("id_transportTariffType");

				return Json(result, JsonRequestBehavior.AllowGet);
			}

		}

		private Boolean IsMaritimeTariff(int? id_transportTariffType)
		{

			Boolean _IsMaritimeTariff = true;

			if (id_transportTariffType != 0)
			{

				TransportTariffType _transportTariffType = db.TransportTariffType.FirstOrDefault(t => t.id == id_transportTariffType);
				int _id_shippingType = _transportTariffType.id_shippingType;
				PurchaseOrderShippingType _purchaseOrderShippingType = db.PurchaseOrderShippingType.FirstOrDefault(t => t.id == _id_shippingType);

				Boolean _isTerrestriel = _purchaseOrderShippingType.isTerrestriel;
				Boolean _isInternal = _transportTariffType.isInternal;

				if (_isInternal)
				{
					_IsMaritimeTariff = false;
				}
				else if (_isTerrestriel)
				{
					_IsMaritimeTariff = false;
				}

			}

			return _IsMaritimeTariff;

		}

		private string returnViewPartial(int id_transportTariffType)
		{

			string _returnViewPartial = "";

			if (id_transportTariffType == 0)
			{

				_returnViewPartial = "_TransportTariffsDetailisMaritimePartial";
			}
			else
			{

				TransportTariffType _transportTariffType = db.TransportTariffType.FirstOrDefault(t => t.id == id_transportTariffType);
				int _id_shippingType = _transportTariffType.id_shippingType;
				PurchaseOrderShippingType _purchaseOrderShippingType = db.PurchaseOrderShippingType.FirstOrDefault(t => t.id == _id_shippingType);

				Boolean _isTerrestriel = _purchaseOrderShippingType.isTerrestriel;
				Boolean _isInternal = _transportTariffType.isInternal;

				if (_isInternal)
				{
					_returnViewPartial = "_TransportTariffsDetailisInternalPartial";
				}
				else if (_isTerrestriel)
				{
					_returnViewPartial = "_TransportTariffsDetailPartial";
				}
				else
				{
					_returnViewPartial = "_TransportTariffsDetailisMaritimePartial";
				}

			}

			return _returnViewPartial;
		}

		private List<TransportTariffDetail> TransportTariffDetailUpdateDelete(TransportTariff transportTariff, int id_TransportTariffDetail, Boolean isDelete)
		{

			if (ModelState.IsValid && transportTariff != null)
			{
				try
				{
					var modeltransportTariffDetail = transportTariff.TransportTariffDetail.FirstOrDefault(i => i.id == id_TransportTariffDetail);
					if (modeltransportTariffDetail != null)
					{
						modeltransportTariffDetail.id_userUpdate = ActiveUser.id;
						modeltransportTariffDetail.dateUpdate = DateTime.Now;
						if (isDelete) modeltransportTariffDetail.isActive = false;
						this.UpdateModel(modeltransportTariffDetail);
					}

					TempData["transportTariff"] = transportTariff;
					TempData.Keep("id_transportTariffType");

				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";


			TempData.Keep("transportTariff");
			TempData.Keep("id_transportTariffType");


			List<TransportTariffDetail> model = transportTariff.TransportTariffDetail?.Where(x => x.isActive).OrderBy(o => o.FishingSite.name).ThenBy(o => o.orderTariff).ToList() ?? new List<TransportTariffDetail>();
			model = (model.Count() != 0) ? model : new List<TransportTariffDetail>();


			return model;
		}

		private TransportTariff ObtainTransportTariff(int? id_transportTariff)
		{

			TransportTariff transportTariff = (TempData["transportTariff"] as TransportTariff);

			transportTariff = transportTariff ?? db.TransportTariff.FirstOrDefault(i => i.id == id_transportTariff);
			transportTariff = transportTariff ?? new TransportTariff();

			return transportTariff;

		}

		private JsonResult TransportTariffDetailOrderUpdate(int id_transportTariff, int id_transportTariffDetail, int UpDown)
		{

			TransportTariff _transportTariff = ObtainTransportTariff(id_transportTariff);

			if (ModelState.IsValid && _transportTariff != null)
			{
				try
				{
					var modeltransportTariffDetail = _transportTariff.TransportTariffDetail.Where(r => r.id_TransportTariff == id_transportTariff);
					if (modeltransportTariffDetail != null)
					{
						// TODO: VALIDACION NULL
						// obtener posicion
						TransportTariffDetail _currentTransportTariffDetail = modeltransportTariffDetail.FirstOrDefault(r => r.id == id_transportTariffDetail);
						int _maxRecords = modeltransportTariffDetail.Count();
						int _originOrder = _currentTransportTariffDetail.orderTariff;
						int _updateOrder = _originOrder + UpDown;
						TransportTariffDetail _DestinyTransportTariffDetail = modeltransportTariffDetail.FirstOrDefault(r => r.orderTariff == _updateOrder);

						if (_originOrder == 1) return Json(new { isValid = false, errorText = "El orden no puede ser menor a uno" }, JsonRequestBehavior.AllowGet);
						if (_originOrder == _maxRecords) return Json(new { isValid = false, errorText = "El orden no puede ser mayor a " + _maxRecords }, JsonRequestBehavior.AllowGet);
						if (_DestinyTransportTariffDetail != null)
						{
							_DestinyTransportTariffDetail.orderTariff = _originOrder;
							_DestinyTransportTariffDetail.id_userUpdate = ActiveUser.id;
							_DestinyTransportTariffDetail.dateUpdate = DateTime.Now;
						}

						if (_currentTransportTariffDetail != null)
						{
							_currentTransportTariffDetail.orderTariff = _updateOrder;
							_currentTransportTariffDetail.id_userUpdate = ActiveUser.id;
							_currentTransportTariffDetail.dateUpdate = DateTime.Now;
						}


						this.UpdateModel(modeltransportTariffDetail);
					}

					TempData["transportTariff"] = _transportTariff;
					TempData.Keep("id_transportTariffType");
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";


			TempData.Keep("transportTariff");
			TempData.Keep("id_transportTariffType");

			return Json(new { isValid = true, errorText = "Orden actualizado" }, JsonRequestBehavior.AllowGet);

		}

		private int setOrderUpdate(int id_transportTariff, TransportTariffDetail transportTariffDetail)
		{

			int _orderTariff = 1;

			// obtener el tipo de tarifario
			// buscar fishing Site
			// tamaño o rango hielo

			TransportTariff _transportTariff = ObtainTransportTariff(id_transportTariff);
			TransportTariffType _transportTariffType = _transportTariff.TransportTariffType;

			Boolean _isTerrestriel = _transportTariffType.PurchaseOrderShippingType.isTerrestriel;
			Boolean _isInternal = _transportTariffType.isInternal;

			if (_isTerrestriel)
			{

				int _id_fishingSite = transportTariffDetail.id_FishingSite;
				PoundsRange _poundsRange = transportTariffDetail.TransportSize.PoundsRange;
				int _poundsRangeIni = _poundsRange.range_ini;
				int _poundsRangeEnd = _poundsRange.range_end;

				var modeltransportTariffDetail = _transportTariff.TransportTariffDetail.Where(r => r.id_FishingSite == _id_fishingSite).OrderBy(o => o.orderTariff).ToList();

				int _lastIdTransportTariffDetail = 0;
				int _lastOrderTariff = 0;
				int _poundsRangeIniLoop = 0;
				int _poundsRangeEndLoop = 0;
				//Boolean _mode = false; // false buscar true reemplazar

				foreach (var _transportTariffDetail in modeltransportTariffDetail)
				{
					// obtener los rangos del actual
					PoundsRange _poundsRangeLoop = _transportTariffDetail.TransportSize.PoundsRange;
					_poundsRangeIniLoop = _poundsRangeLoop.range_ini;
					_poundsRangeEndLoop = _poundsRangeLoop.range_end;


					if (_poundsRangeEnd < _poundsRangeIniLoop)
					{
						// _transportTariffDetail 


					}

					this.UpdateModel(_transportTariffDetail);

					_lastIdTransportTariffDetail = _transportTariffDetail.id;
					_lastOrderTariff = _transportTariffDetail.orderTariff;

				}


				// loop detalles -> sitio = $sitio
				// obtener
				// 100 200  1
				// 300 400  2

			}


			return _orderTariff;

		}


		private Boolean setOrderTransportTariffDetail(int id_transportTariff)
		{

			// el modelo ya ha sido actualizado y esta incluido el detalle
			Boolean _result = false;
			DBContext dbtmp = new DBContext();
			TransportTariff _transportTariff = new TransportTariff();
			if (id_transportTariff == 0)
			{

				_transportTariff = dbtmp.TransportTariff.Where(r => r.isActive && r.id_company == this.ActiveCompanyId && r.id_userCreate == ActiveUser.id).OrderByDescending(o => o.dateCreate).FirstOrDefault();
			}
			else
			{

				_transportTariff = dbtmp.TransportTariff.FirstOrDefault(r => r.id == id_transportTariff);
			}

			id_transportTariff = id_transportTariff == 0 ? _transportTariff.id : id_transportTariff;

			if (_transportTariff.TransportTariffType == null) return _result;

			//ordernar por tamano o rango hielo
			Boolean _isInternal = _transportTariff.TransportTariffType.isInternal;
			Boolean _isTerrestriel = _transportTariff.TransportTariffType.PurchaseOrderShippingType.isTerrestriel;

			int orderDetail = 1;

			//Order 
			// ordernar por rango hielo
			if (_isInternal)
			{

				using (DbContextTransaction trans = dbtmp.Database.BeginTransaction())
				{

					// dbtmp.Entry(_transportTariff).Reload();
					// ctx.Refresh(RefreshMode.StoreWins, objects);
					try
					{
						foreach (var id_FishingSite in _transportTariff
														.TransportTariffDetail
														.Where(r => r.isActive)
														.GroupBy(gp => gp.id_FishingSite).ToList()
														)
						{
							orderDetail = 1;

							dbtmp.TransportTariff
								.FirstOrDefault(r => r.id == id_transportTariff)
								.TransportTariffDetail
								.Where(r => r.id_FishingSite == id_FishingSite.Key && r.isActive)
								.OrderBy(o => o.IceBagRange.range_ini)
								.ThenBy(o => o.IceBagRange.range_end).ToList()
								.ForEach(r => r.orderTariff = orderDetail++);
							dbtmp.SaveChanges();
						}
						trans.Commit();
						_result = true;

					}
					catch //(Exception e)
					{

						trans.Rollback();
						_result = false;

					}
				}

			}
			else
			{

				if (_isTerrestriel)
				{
					using (DbContextTransaction trans = dbtmp.Database.BeginTransaction())
					{

						//dbtmp.Entry(_transportTariff).Reload();

						try
						{

							foreach (var id_FishingSite in _transportTariff
														.TransportTariffDetail
														.Where(r => r.isActive)
														.GroupBy(gp => gp.id_FishingSite).ToList()

														)
							{
								orderDetail = 1;

								dbtmp.TransportTariff
								.FirstOrDefault(r => r.id == id_transportTariff)
								.TransportTariffDetail
								.Where(r => r.id_FishingSite == id_FishingSite.Key && r.isActive)
								.OrderBy(o => o.TransportSize.PoundsRange.range_ini)
								.ThenBy(o => o.TransportSize.PoundsRange.range_end).ToList()
								.ForEach(r => r.orderTariff = orderDetail++);
								dbtmp.SaveChanges();
							}
							trans.Commit();
							_result = true;

						}
						catch //(Exception e)
						{

							trans.Rollback();
							_result = false;

						}
					}

				}


			}

			return _result;
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult PrintTariffReport()
		{
			List<ParamCR> paramLst = new List<ParamCR>();

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = "RTRF";
			_repMod.conex = objConex;
			_repMod.paramCRList = paramLst;

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);
		}

	}
}
