using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using System.Globalization;
using DevExpress.Web.Mvc;
using EntidadesAuxiliares.General;
using EntidadesAuxiliares.CrystalReport;
using System.Configuration;
using DevExpress.Web;
using DevExpress.Utils;

namespace DXPANACEASOFT.Controllers
{
	public class LiquidationFreightRiverController : DefaultController
	{
		public string pathFiles = ConfigurationManager.AppSettings["RootDirectoryServerUploadFiles"];
		public string pathFilesCopy = ConfigurationManager.AppSettings["RootDirectoryServerUploadFilesCopy"];

		const string codigoParametroGuiaDistintosProveedores = "LIQANYPROV";
		public ActionResult Index()
		{
			return View();
		}

		#region LIQUITACION DE FLETE FLUVIAL GUIDE FILTERS RESULTS

		[HttpPost]
		public ActionResult LiquidationFreightRiverResults(string numberGuide, LiquidationFreightRiver liquidationFreightriver,
												  Document document,
												  DateTime? startEmissionDate, DateTime? endEmissionDate,
												  DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
												  string carRegistration,
												  int? id_providerfilter,
												  int? id_owner,
												  int[] items
												  )
		{
			//int id_providerremision
			var model = db.LiquidationFreightRiver.ToList();

			#region DOCUMENT FILTERS
			if (!string.IsNullOrEmpty(numberGuide))
			{
				model = model.Where(w => w.LiquidationFreightRiverDetail.Any(a => a.RemissionGuideRiver.Document.sequential.ToString().Contains(numberGuide))).ToList();
			}
			if (document.id_documentState != 0)
			{
				model = model.Where(o => o.Document.id_documentState == document.id_documentState).ToList();
			}

			if (!string.IsNullOrEmpty(document.number))
			{
				model = model.Where(o => o.Document.number.Contains(document.number)).ToList();
			}

			if (startEmissionDate != null && endEmissionDate != null)
			{
				model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0 && DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
			}

			if (startAuthorizationDate != null && endAuthorizationDate != null)
			{
				model = model.Where(o => DateTime.Compare(startAuthorizationDate.Value.Date, o.Document.authorizationDate.Value.Date) <= 0 && DateTime.Compare(o.Document.authorizationDate.Value.Date, endAuthorizationDate.Value.Date) <= 0).ToList();
			}

			if (!string.IsNullOrEmpty(carRegistration))
			{
				model = model.Where(o => o.LiquidationFreightRiverDetail.Where(x => x.RemissionGuideRiver.RemissionGuideRiverTransportation.DriverVeicleProviderTransport.VeicleProviderTransport.Vehicle.carRegistration.Contains(carRegistration)) != null).ToList();
			}

			if (liquidationFreightriver != null && liquidationFreightriver.id_providertransport > 0)
			{
				model = model.Where(o => o.id_providertransport == liquidationFreightriver.id_providertransport).ToList();
			}

			if (id_providerfilter != null && id_providerfilter > 0)
			{
				model = model.Where(o => o.LiquidationFreightRiverDetail.Any(p => p.RemissionGuideRiver.id_providerRemisionGuideRiver == id_providerfilter)).ToList();
			}
			if (id_owner != null && id_owner > 0)
			{
				model = model.Where(o => o.LiquidationFreightRiverDetail.Any(p => p.RemissionGuideRiver.RemissionGuideRiverTransportation.Vehicle.id_personOwner == id_owner)).ToList();
			}

			#endregion

			TempData["model"] = model;
			TempData.Keep("model");

			return PartialView("_LiquidationFreightRiverResultsPartial", model.OrderByDescending(r => r.id).ToList());
		}


		#endregion

		#region LIQUITACION DE FLETE FLUVIAL REMISSION GUIDE  RESULTS

		[HttpPost, ValidateInput(false)]
		public ActionResult RemissionGuideRiverResults()
		{

			string[] documentStates = new string[] { "01", "05" };

			var remissionGuideRiverTransportationWithValuePrice = db.RemissionGuideRiverTransportation
																				.Where(r => (r.valuePrice ?? 0) > 0)
																				.Select(r => r.id_remissionGuideRiver)
																				.ToArray();

		   var model = db.RemissionGuideRiver.Where(d => !documentStates.Contains(d.Document.DocumentState.code)
														  && remissionGuideRiverTransportationWithValuePrice.Contains(d.id)
														  && (d.hasEntrancePlanctProduction??false) == true)
							.ToList();
				

			var LiquidationFreightRiverDetailActive = db.LiquidationFreightRiverDetail
																	.Where(r => r.LiquidationFreightRiver.Document.DocumentState.code != "05")
																	.ToList();

			var filter = ( from d in LiquidationFreightRiverDetailActive
						   join e in model on d.id_remisionGuideRiver equals e.id
						   select d.id_remisionGuideRiver).ToArray();

			if (filter != null && model != null)
			{
				model = model.Where(X => !filter.Contains(X.id)).ToList();
			}
            foreach (var item in model)
            {
                item.requiredLogistic = item.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;
            }
			if (model != null)
			{
				model = model.OrderByDescending(d => d.id).ToList();
			}

			return PartialView("_RemissionGuideRiverDetailsResultsPartial", model.ToList());
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult RemissionGuideRiverDetailsPartial()
		{
			var model = db.RemissionGuideRiver.Where(d => !(d.Document.DocumentState.code == "01"
														|| d.Document.DocumentState.code == "05")
														&& d.RemissionGuideRiverTransportation.valuePrice > 0
														&& d.RemissionGuideRiverDetail.Any(a => a.RemissionGuideRiver.hasEntrancePlanctProduction == true));

			var filter = (from d in db.LiquidationFreightRiverDetail
						  join e in model on d.id_remisionGuideRiver equals e.id
						  where d.LiquidationFreightRiver.Document.DocumentState.code != "05"
						  select d.id_remisionGuideRiver).ToList();
			if (filter != null)
			{
				model = model.Where(X => !filter.Contains(X.id));
			}
            foreach (var item in model)
            {
                item.requiredLogistic = item.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;
            }
            if (model != null)
			{
				model = model.OrderByDescending(d => d.id);
			}

			return PartialView("_RemissionGuideRiverDetailsPartial", model.ToList());
		}
		#endregion

		#region LiquidationFreightRiver HEADER
		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationFreightRiverPartial()
		{
			var model = (TempData["model"] as List<LiquidationFreightRiver>);
			model = model ?? new List<LiquidationFreightRiver>();
			TempData.Keep("model");
			return PartialView("_LiquidationFreightRiverPartial", model.OrderByDescending(r => r.id).ToList());
		}
		#endregion

		#region Edit LiquidationFreightRiver
		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditLiquidationFreightRiver(int id, int[] orderDetails)
		{
			LiquidationFreightRiver liquidationfreightRiver = db.LiquidationFreightRiver
																.FirstOrDefault(o => o.id == id);
			decimal _priceadjustment = 0;
			decimal _pricedays = 0;
			decimal _priceextension = 0;
			decimal _default = 0;
			string _descriptionRG = "";
			decimal _quantityPoundsReceived = 0;

			if (liquidationfreightRiver == null)
			{
				DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("67"));
				DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

				liquidationfreightRiver = new LiquidationFreightRiver
				{
					Document = new Document
					{
						id = 0,
						id_documentType = documentType?.id ?? 0,
						DocumentType = documentType,
						id_documentState = documentState?.id ?? 0,
						DocumentState = documentState,
						emissionDate = DateTime.Now.Date
					},
					id_providertransport = 0,
					priceadjustment = 0,
					pricedays = 0,
					priceextension = 0,
					priceavance = 0,
					PriceCancelledTotal = 0,
					LiquidationFreightRiverDetail = new List<LiquidationFreightRiverDetail>(),
				};
			}

			if (orderDetails != null)
			{
				List<LiquidationFreightRiverDetail> LiquidationFreightRiverDetail = new List<LiquidationFreightRiverDetail>();

				decimal dTotal = 0, dAvance = 0, dPriceAdjustment = 0, dPriceDays = 0, dPriceExtension = 0, dPriceCancelled = 0;
				foreach (var od in orderDetails)
				{
					RemissionGuideRiver tempRemissionGuideRiver = db.RemissionGuideRiver
																	.Where(d => d.id == od)
																	.FirstOrDefault();
                    _priceadjustment = 0;
					_pricedays = 0;
					_priceextension = 0;
					_descriptionRG = "";
					_quantityPoundsReceived = 0;

					RemGuideRiverLiqTransportation rmp = db.RemGuideRiverLiqTransportation
														.FirstOrDefault(fod => fod.id_remisionGuideRiver == od);
                    if (rmp != null)
                    {
                        _priceadjustment = rmp.priceadjustment;
                        _pricedays = (rmp.pricedays > 0) ? rmp.pricedays : _default;
                        _priceextension = (rmp.priceextension > 0) ? rmp.priceextension : _default;
                        _descriptionRG = rmp.descriptionRGR;
                        //_quantityPoundsReceived = rmp.quantityPoundsTransported ?? 0;
                    }

                    RemissionGuideRiverDetail rmpDetail = db.RemissionGuideRiverDetail
                                                        .FirstOrDefault(fod => fod.id_remissionGuideRiver == od);
                    if (rmpDetail != null)
					{
                        _quantityPoundsReceived = rmpDetail.quantityProgrammed;
                    }

					LiquidationFreightRiverDetail liquidationFreightRiverDetail = new LiquidationFreightRiverDetail()
					{
						id = liquidationfreightRiver.LiquidationFreightRiverDetail.Count() > 0 ? liquidationfreightRiver.LiquidationFreightRiverDetail.Max(pld => pld.id) + 1 : 1,
						id_remisionGuideRiver = tempRemissionGuideRiver.id,
						isActive = true,
						id_userCreate = ActiveUser.id,
						dateCreate = DateTime.Now,
						id_userUpdate = ActiveUser.id,
						dateUpdate = DateTime.Now,
						id_LiquidationFreightRiver = liquidationfreightRiver.id,
						RemissionGuideRiver = tempRemissionGuideRiver,
						LiquidationFreightRiver = liquidationfreightRiver,
						price = tempRemissionGuideRiver?.RemissionGuideRiverTransportation?.valuePrice ?? 0,
						pricesavance = tempRemissionGuideRiver?.RemissionGuideRiverTransportation?.advancePrice ?? 0,
						priceadjustment = _priceadjustment,
						pricedays = _pricedays,
						PriceCancelled = 0,
						priceextension = _priceextension,
						descriptionRGR = _descriptionRG,
						quantityPoundsTransported = _quantityPoundsReceived,
						pricesubtotal = (tempRemissionGuideRiver?.RemissionGuideRiverTransportation?.valuePrice ?? 0) + (+_pricedays + _priceextension + _priceadjustment),
						pricetotal = (tempRemissionGuideRiver?.RemissionGuideRiverTransportation?.valuePrice ?? 0) - (tempRemissionGuideRiver?.RemissionGuideRiverTransportation?.advancePrice ?? 0) + (+_pricedays + _priceextension + _priceadjustment)

					};


					if (tempRemissionGuideRiver?.Document?.DocumentState?.code == "08")
					{
						liquidationFreightRiverDetail.PriceCancelled = tempRemissionGuideRiver?.RemissionGuideRiverTransportation?.valuePrice ?? 0;
						liquidationFreightRiverDetail.pricesubtotal = liquidationFreightRiverDetail.pricesubtotal - (liquidationFreightRiverDetail.PriceCancelled ?? 0);
						liquidationFreightRiverDetail.pricetotal = liquidationFreightRiverDetail.pricetotal - (tempRemissionGuideRiver?.RemissionGuideRiverTransportation?.valuePrice ?? 0);
					}

					//Precios Nuevos
					dPriceAdjustment += liquidationFreightRiverDetail.priceadjustment;
					dPriceDays += liquidationFreightRiverDetail.pricedays;
					dPriceExtension += liquidationFreightRiverDetail.priceextension;
					dPriceCancelled += liquidationFreightRiverDetail.PriceCancelled ?? 0;

					dTotal += (liquidationFreightRiverDetail.price);
					dAvance += (liquidationFreightRiverDetail.pricesavance);

					//dTotal = dTotal + ((tempRemissionGuide?.RemissionGuideTransportation?.valuePrice ?? 0) < 0 ? 0 : (decimal)(tempRemissionGuide?.RemissionGuideTransportation?.valuePrice ?? 0));
					//dAvance = dAvance + ((tempRemissionGuide?.RemissionGuideTransportation?.advancePrice ?? 0)< 0 ? 0 : (decimal)(tempRemissionGuide?.RemissionGuideTransportation?.advancePrice ?? 0));



					//Eliminar esto
					var id_vehicleTmpl = tempRemissionGuideRiver?
											.RemissionGuideRiverTransportation?.id_vehicle ?? 0;

					int id_proviTmpl = 0;
					if (id_vehicleTmpl != 0)
					{
						var tmp = db.VehicleProviderTransportBilling.FirstOrDefault(w => w.id_vehicle == id_vehicleTmpl && w.state && w.datefin == null);
						if (tmp != null)
						{
							id_proviTmpl = tmp.id_provider;
						}
					}

					if (liquidationfreightRiver.id_providertransport == 0)
					{
						liquidationfreightRiver.id_providertransport = (tempRemissionGuideRiver != null)
																		&& (tempRemissionGuideRiver.RemissionGuideRiverTransportation != null)
																		&& (tempRemissionGuideRiver.RemissionGuideRiverTransportation.VehicleProviderTransportBilling != null)
																		&& (tempRemissionGuideRiver.RemissionGuideRiverTransportation.VehicleProviderTransportBilling.id_provider != 0) ? (int)tempRemissionGuideRiver?.RemissionGuideRiverTransportation?.VehicleProviderTransportBilling?.id_provider : id_proviTmpl;
						liquidationfreightRiver.Person = db.Person.Where(x => x.id == liquidationfreightRiver.id_providertransport).FirstOrDefault() ?? (db.Person.FirstOrDefault(fod => fod.id == id_proviTmpl) ?? new Person());
					}
					LiquidationFreightRiverDetail.Add(liquidationFreightRiverDetail);
				}

				liquidationfreightRiver.priceadjustment = dPriceAdjustment;
				liquidationfreightRiver.pricedays = dPriceDays;
				liquidationfreightRiver.priceextension = dPriceExtension;

				liquidationfreightRiver.PriceCancelledTotal = dPriceCancelled;
				liquidationfreightRiver.priceavance = dAvance;

				liquidationfreightRiver.price = dTotal;
				//Se colocan los Totales
				dTotal = dTotal + dPriceAdjustment + dPriceDays + dPriceExtension - dPriceCancelled;
				liquidationfreightRiver.pricesubtotal = dTotal;
				liquidationfreightRiver.pricetotal = dTotal - (dAvance);

				liquidationfreightRiver.LiquidationFreightRiverDetail = LiquidationFreightRiverDetail;
			}
            foreach (var item in liquidationfreightRiver.LiquidationFreightRiverDetail)
            {
                item.RemissionGuideRiver.requiredLogistic = item.RemissionGuideRiver.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

            }

            TempData["LiquidationFreightRiver"] = liquidationfreightRiver;
			TempData.Keep("LiquidationFreightRiver");

			return PartialView("_FormEditLiquidationFreightRiver", liquidationfreightRiver);
		}
		#endregion

		#region LIQUIDACION DE FLETE DETAILS
		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationFreightRiverResultsDetailViewPartial()
		{
			int id_LiquidationFreightRiver = (Request.Params["id_LiquidationFreightRiver"] != null && Request.Params["id_LiquidationFreightRiver"] != "") ? int.Parse(Request.Params["id_LiquidationFreightRiver"]) : -1;
			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id_LiquidationFreightRiver);
			return PartialView("_LiquidationFreightRiverResultsDetailViewPartial", liquidationFreightRiver.LiquidationFreightRiverDetail.ToList());
		}
		public ActionResult LiquidationFreightRiverDetailViewDetailsPartial()
		{
			int id_LiquidationFreightRiver = (Request.Params["id_LiquidationFreightRiver"] != null && Request.Params["id_LiquidationFreightRiver"] != "") ? int.Parse(Request.Params["id_LiquidationFreightRiver"]) : -1;
			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id_LiquidationFreightRiver);
			return PartialView("_LiquidationFreightRiverDetailViewDetailsPartial", liquidationFreightRiver.LiquidationFreightRiverDetail.ToList());
		}
		#endregion

		#region ACTIONS
		[HttpPost, ValidateInput(false)]
		public JsonResult Actions(int id)
		{
			var actions = new
			{
				btnApprove = true,
				btnAutorize = false,
				btnProtect = false,
				btnCancel = false,
				btnRevert = false,
			};

			if (id == 0)
			{
				return Json(actions, JsonRequestBehavior.AllowGet);
			}

			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id);
			//int state = remissionGuide.Document.DocumentState.id;
			string state = liquidationFreightRiver.Document.DocumentState.code;

			if (state == "01") // PENDIENTE
			{
				actions = new
				{
					btnApprove = true,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = true,
					btnRevert = false,
				};
			}
			else if (state == "03")//|| state == 3) // APROBADA
			{
				actions = new
				{
					btnApprove = false,
					btnAutorize = true,
					btnProtect = false,
					btnCancel = true,
					btnRevert = true,
				};
			}
			else if (state == "04" || state == "05") // CERRADA O ANULADA
			{
				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = false,
				};
			}
			else if (state == "06") // AUTORIZADA
			{
				//var purchaseOrderDetailAux = purchaseOrder.PurchaseOrderDetail.Where(w => w.isActive = true).FirstOrDefault(fod => fod.quantityReceived < fod.quantityApproved);

				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,//purchaseOrderDetailAux != null,// true,
									   //btnProtect = true,
					btnCancel = true,
					btnRevert = true,
				};
			}

			return Json(actions, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region VALIDACION lIQUIDACION
		[HttpPost]
		public JsonResult ValidateSelectedRowsRemissionGuideRiver(int[] ids)
		{
			var result = new
			{
				Message = "OK"
			};

			if (ids == null || ids.Count() == 0)
			{
				result = new
				{
					Message = ErrorMessage("No Ha seleccionado una Guia")
				};
				TempData.Keep("remissionGuideRiver");
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			int noneProviderBilling = (from e in db.RemissionGuideRiver
									   where ids.Contains(e.id) && e.RemissionGuideRiverTransportation.id_VehicleProviderTranportistBilling == null
									   select e.RemissionGuideRiverTransportation.VehicleProviderTransportBilling.id_provider).Distinct().Count();

			if (noneProviderBilling >= 1)
			{
				result = new
				{
					Message = ErrorMessage("Hay Vehículos que no tienen compañía que factura")
				};
				TempData.Keep("remissionGuideRiver");
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			int distinctProveedor = (from e in db.RemissionGuideRiver
									 where ids.Contains(e.id)
									 select e.RemissionGuideRiverTransportation.VehicleProviderTransportBilling.id_provider).Distinct().Count();

			if (distinctProveedor > 1)
			{
				result = new
				{
					Message = ErrorMessage("Las compañías que facturan no pueden ser diferentes")
				};
				TempData.Keep("remissionGuideRiver");
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			//int distinctProcess = (from e in db.RemissionGuideRiver
			//					   where ids.Contains(e.id)
			//					   select e.id_personProcessPlant).Distinct().Count();

			//if (distinctProcess > 1)
			//{
			//	result = new
			//	{
			//		Message = ErrorMessage("Los procesos no pueden ser diferentes")
			//	};
			//	TempData.Keep("remissionGuideRiver");
			//	return Json(result, JsonRequestBehavior.AllowGet);
			//}

			TempData.Keep("remissionGuideRiver");
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_LiquidationFreightRiver)
		{
			TempData.Keep("LiquidationFreightRiver");
			int[] idx = db.LiquidationFreightRiver.Select(r => r.id).ToArray();
			int index = idx.OrderByDescending(r => r).ToList().FindIndex(r => r == id_LiquidationFreightRiver);
			var result = new
			{
				maximunPages = idx.Length,
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (liquidationFreightRiver != null)
			{
				TempData["LiquidationFreightRiver"] = liquidationFreightRiver;
				TempData.Keep("LiquidationFreightRiver");
				return PartialView("_LiquidationFreightRiverMainFormPartial", liquidationFreightRiver);
			}

			TempData.Keep("LiquidationFreight");

			return PartialView("_LiquidationFreightRiverMainFormPartial", new LiquidationFreightRiver());
		}
		#endregion

		[HttpPost, ValidateInput(false)]
		public JsonResult ItemDetailData(string pricedays, string priceextension, string priceadjustment, string priceCanc)
		{
			decimal _pricedays = Convert.ToDecimal(pricedays);
			decimal _priceextension = Convert.ToDecimal(priceextension);
			decimal _priceadjustment = Convert.ToDecimal(priceadjustment);
			decimal _priceCanc = Convert.ToDecimal(priceCanc);

			LiquidationFreightRiver liquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);

			if (_pricedays <= 0.0M)
			{
				_pricedays = 0;
			}

			if (_priceextension <= 0.0M)
			{
				_priceextension = 0;
			}

			if (_priceadjustment <= 0.0M)
			{
				_priceadjustment = 0;
			}

			liquidationFreightRiver.pricedays = _pricedays;
			liquidationFreightRiver.priceextension = _priceextension;
			liquidationFreightRiver.priceadjustment = _priceadjustment;
			liquidationFreightRiver.PriceCancelledTotal = _priceCanc;
			liquidationFreightRiver.pricetotal = (liquidationFreightRiver.pricesubtotal + _pricedays + _priceextension + _priceadjustment) - liquidationFreightRiver.priceavance - _priceCanc;

			var result = new
			{
				ItemData = new
				{
					pricedays = _pricedays,
					priceextension = _priceextension,
					priceadjustment = _priceadjustment,
					pricetotal = liquidationFreightRiver.pricetotal,
					pricesubtotal = liquidationFreightRiver.pricesubtotal,
					priceavance = liquidationFreightRiver.priceavance,
					price = liquidationFreightRiver.priceavance,
					priceCancel = liquidationFreightRiver.PriceCancelledTotal
				},

			};

			TempData["LiquidationFreightRiver"] = liquidationFreightRiver;
			TempData.Keep("LiquidationFreightRiver");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		#region Save and Update
		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationFreightRiverPartialAddNew(bool approve, LiquidationFreightRiver item, Document document)
		{
			LiquidationFreightRiver conLiquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
			//if (conLiquidationFreightRiver.pricetotal <= 0)
			//{
			//    TempData.Keep("LiquidationFreightRiver");
			//    ViewData["EditMessage"] = ErrorMessage("No se puede generar una Liquidacion de Flete con  Totales menores o igual a 0");
			//    return PartialView("_LiquidationFreightRiverMainFormPartial", conLiquidationFreightRiver);
			//}
			int id_Vehicle = 0;
			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					#region Document
					document.id_userCreate = ActiveUser.id;
					document.dateCreate = DateTime.Now;
					document.id_userUpdate = ActiveUser.id;
					document.dateUpdate = DateTime.Now;
					document.authorizationDate = DateTime.Now;

					DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == document.id_documentType);

					document.sequential = documentType?.currentNumber ?? 0;
					document.number = GetDocumentNumber(document.id_documentType);
					document.DocumentType = documentType;

					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == document.id_documentState);
					document.DocumentState = documentState;

					EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
					document.id_emissionPoint = emissionPoint?.id ?? 0;
					document.EmissionPoint = emissionPoint;

					string emissionDate = document.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");

					document.accessKey = AccessKey.GenerateAccessKey(emissionDate,
																	document.DocumentType.code,
																	document.EmissionPoint.BranchOffice.Division.Company.ruc,
																	"1",
																	document.EmissionPoint.BranchOffice.code.ToString() + document.EmissionPoint.code.ToString("D3"),
																	document.sequential.ToString("D9"),
																	document.sequential.ToString("D8"),
																	"1");
					//Actualiza Secuencial
					if (documentType != null)
					{
						documentType.currentNumber = documentType.currentNumber + 1;
						db.DocumentType.Attach(documentType);
						db.Entry(documentType).State = EntityState.Modified;

					}
					#endregion

					#region LiquidationFreight River

					item.Document = document;
					item.Person = null;

					#endregion

					LiquidationFreightRiver tempLiquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
					tempLiquidationFreightRiver = tempLiquidationFreightRiver ?? new LiquidationFreightRiver();

					#region LiquidationFreight DETAILS



					if (tempLiquidationFreightRiver?.LiquidationFreightRiverDetail != null)
					{
						item.LiquidationFreightRiverDetail = new List<LiquidationFreightRiverDetail>();

						var details = tempLiquidationFreightRiver.LiquidationFreightRiverDetail.ToList();

						foreach (var detail in details)
						{
							id_Vehicle = db.RemissionGuideRiverTransportation.FirstOrDefault(fod => fod.id_remissionGuideRiver == detail.id_remisionGuideRiver)?.id_vehicle ?? 0;
							var liquidationFreightDetail = new LiquidationFreightRiverDetail
							{
								id_remisionGuideRiver = detail.id_remisionGuideRiver,
								RemissionGuideRiver = db.RemissionGuideRiver.FirstOrDefault(i => i.id == detail.id_remisionGuideRiver),
								isActive = detail.isActive,
								id_userCreate = detail.id_userCreate,
								dateCreate = detail.dateCreate,
								id_userUpdate = detail.id_userUpdate,
								dateUpdate = detail.dateUpdate,
								price = detail.price,
								pricedays = detail.pricedays,
								pricesavance = detail.pricesavance,
								priceadjustment = detail.priceadjustment,
								priceextension = detail.priceextension,
								pricesubtotal = detail.pricesubtotal,
								pricetotal = detail.pricetotal,
								PriceCancelled = detail.PriceCancelled,
								quantityPoundsTransported = detail.quantityPoundsTransported ?? 0,
								descriptionRGR = detail.descriptionRGR,
								idOwnerVehicle = db.Vehicle.FirstOrDefault(fod => fod.id == id_Vehicle)?.id_personOwner
							};

							item.LiquidationFreightRiverDetail.Add(liquidationFreightDetail);
						}
					}

					if (item.LiquidationFreightRiverDetail.Count == 0)
					{
						TempData.Keep("LiquidationFreight");
						ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Liquidacion de Flete sin detalles");
						return PartialView("_LiquidationFreightRiverMainFormPartial", tempLiquidationFreightRiver);
					}
					#endregion

					#region Documents Attached

					if (conLiquidationFreightRiver?.LiquidationFreightRiverDocument != null)
					{
						item.LiquidationFreightRiverDocument = new List<LiquidationFreightRiverDocument>();
						var liquidationDocAttach = conLiquidationFreightRiver.LiquidationFreightRiverDocument.ToList();

						foreach (var detail in liquidationDocAttach)
						{
							var tempLiquidationDocAttach = new LiquidationFreightRiverDocument
							{
								guid = detail.guid,
								url = detail.url,
								attachment = detail.attachment,
								referenceDocument = detail.referenceDocument,
								descriptionDocument = detail.descriptionDocument
							};

							item.LiquidationFreightRiverDocument.Add(tempLiquidationDocAttach);
						}
					}

					#endregion


					if (approve)
					{
						item.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
					}

					db.LiquidationFreightRiver.Add(item);
					db.SaveChanges();
					trans.Commit();

					item.Person = db.Person.Where(x => x.id == item.id_providertransport).FirstOrDefault();
					TempData["LiquidationFreightRiver"] = item;
					TempData.Keep("LiquidationFreightRiver");
					ViewData["EditMessage"] = SuccessMessage("Liquidación de Flete: " + item.Document.number + " guardada exitosamente");
				}
				catch (Exception e)
				{
					TempData.Keep("LiquidationFreightRiver");
					item = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}


            foreach (var detail in item.LiquidationFreightRiverDetail)
            {
                detail.RemissionGuideRiver.requiredLogistic = detail.RemissionGuideRiver.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

            }
            return PartialView("_LiquidationFreightRiverMainFormPartial", item);


		}
		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationFreightRiverPartialUpdate(bool approve, LiquidationFreightRiver item, Document document)
		{
			LiquidationFreightRiver modelItem = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == item.id);

			int id_Vehicle = 0;
			if (modelItem != null)
			{
				LiquidationFreightRiver conLiquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);

				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						#region Document

						modelItem.Document.description = document.description;
						modelItem.Document.emissionDate = document.emissionDate;
						modelItem.Document.id_userUpdate = ActiveUser.id;
						modelItem.Document.dateUpdate = DateTime.Now;
						#endregion

						#region LiquidationFreigth
						modelItem.price = item.price;
						modelItem.priceadjustment = item.priceadjustment;
						modelItem.pricesubtotal = item.pricesubtotal;
						modelItem.pricetotal = item.pricetotal;
						modelItem.pricedays = item.pricedays;
						modelItem.priceavance = item.priceavance;
						modelItem.InvoiceNumber = item.InvoiceNumber;
						modelItem.PriceCancelledTotal = item.PriceCancelledTotal;
						modelItem.priceextension = item.priceextension;
						modelItem.id_providertransport = item.id_providertransport;
						modelItem.Person = db.Person.FirstOrDefault(r => r.id == item.id_providertransport);

						#endregion

						LiquidationFreightRiver tempLiquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
						tempLiquidationFreightRiver = tempLiquidationFreightRiver ?? new LiquidationFreightRiver();

						#region LiquidationFreigth River DETAILS

						int count = 0;

						if (tempLiquidationFreightRiver?.LiquidationFreightRiverDetail != null)
						{
							var details = tempLiquidationFreightRiver.LiquidationFreightRiverDetail.ToList();



							foreach (var detail in details)
							{
								LiquidationFreightRiverDetail liquidationFreightRiverDetail = modelItem.LiquidationFreightRiverDetail.FirstOrDefault(d => d.id == detail.id);
								id_Vehicle = db.RemissionGuideRiverTransportation.FirstOrDefault(fod => fod.id_remissionGuideRiver == detail.id_remisionGuideRiver)?.id_vehicle ?? 0;

								if (liquidationFreightRiverDetail == null)
								{
									liquidationFreightRiverDetail = new LiquidationFreightRiverDetail
									{
										id_LiquidationFreightRiver = detail.id_LiquidationFreightRiver,
										id_remisionGuideRiver = detail.id_remisionGuideRiver,
										RemissionGuideRiver = db.RemissionGuideRiver
																.FirstOrDefault(i => i.id == detail.id_remisionGuideRiver),
										LiquidationFreightRiver = detail.LiquidationFreightRiver,

										isActive = detail.isActive,
										id_userCreate = detail.id_userCreate,
										dateCreate = detail.dateCreate,
										id_userUpdate = detail.id_userUpdate,
										dateUpdate = detail.dateUpdate,
										price = detail.price,
										pricedays = detail.pricedays,
										priceadjustment = detail.priceadjustment,
										priceextension = detail.priceextension,
										pricesubtotal = detail.pricesubtotal,
										pricetotal = detail.pricetotal,
										PriceCancelled = detail.PriceCancelled,
										pricesavance = detail.pricesavance,
										quantityPoundsTransported = detail.quantityPoundsTransported ?? 0,
										descriptionRGR = detail.descriptionRGR,
										idOwnerVehicle = db.Vehicle.FirstOrDefault(fod => fod.id == id_Vehicle)?.id_personOwner
									};

									modelItem.LiquidationFreightRiverDetail.Add(liquidationFreightRiverDetail);
									count++;
								}
								else
								{
									liquidationFreightRiverDetail.isActive = detail.isActive;
									liquidationFreightRiverDetail.id_userCreate = detail.id_userCreate;
									liquidationFreightRiverDetail.dateCreate = detail.dateCreate;
									liquidationFreightRiverDetail.id_userUpdate = detail.id_userUpdate;
									liquidationFreightRiverDetail.dateUpdate = detail.dateUpdate;
									liquidationFreightRiverDetail.price = detail.price;
									liquidationFreightRiverDetail.pricedays = detail.pricedays;
									liquidationFreightRiverDetail.pricesavance = detail.pricesavance;
									liquidationFreightRiverDetail.priceadjustment = detail.priceadjustment;
									liquidationFreightRiverDetail.priceextension = detail.priceextension;
									liquidationFreightRiverDetail.pricesubtotal = detail.pricesubtotal;
									liquidationFreightRiverDetail.PriceCancelled = detail.PriceCancelled;
									liquidationFreightRiverDetail.pricetotal = detail.pricetotal;
									liquidationFreightRiverDetail.quantityPoundsTransported = detail.quantityPoundsTransported;
									count++;
								}

							}
						}

						if (count == 0)
						{
							TempData.Keep("LiquidationFreightRiver");
							ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Liquidacion de Flete sin detalles");
							return PartialView("_LiquidationFreightRiverMainFormPartial", tempLiquidationFreightRiver);
						}

						#endregion

						#region UPDATE LIQUIDATION DOCUMENTS
						List<LiquidationFreightRiverDocument> lstDocAttac = conLiquidationFreightRiver.LiquidationFreightRiverDocument.ToList();
						if (lstDocAttac != null)
						{
							var lstDocAttach = lstDocAttac.ToList();

							for (int i = modelItem.LiquidationFreightRiverDocument.Count - 1; i >= 0; i--)
							{
								var detail = modelItem.LiquidationFreightRiverDocument.ElementAt(i);

								if (lstDocAttach.FirstOrDefault(fod => fod.id == detail.id) == null)
								{
									LiquidationRiverDocumentsDeleteAttachment(detail);
									modelItem.LiquidationFreightRiverDocument.Remove(detail);
									db.Entry(detail).State = EntityState.Deleted;
								}

							}

							foreach (var detail in lstDocAttach)
							{
								var tempLiqDocument = modelItem
															.LiquidationFreightRiverDocument
															.FirstOrDefault(fod => fod.id == detail.id);
								if (tempLiqDocument == null)
								{
									tempLiqDocument = new LiquidationFreightRiverDocument
									{
										guid = detail.guid,
										url = detail.url,
										attachment = detail.attachment,
										referenceDocument = detail.referenceDocument,
										descriptionDocument = detail.descriptionDocument
									};
									modelItem.LiquidationFreightRiverDocument.Add(tempLiqDocument);
								}
								else
								{
									if (tempLiqDocument.url != detail.url)
									{
										LiquidationRiverDocumentsDeleteAttachment(tempLiqDocument);
										tempLiqDocument.guid = detail.guid;
										tempLiqDocument.url = detail.url;
										tempLiqDocument.attachment = detail.attachment;
									}
									tempLiqDocument.referenceDocument = detail.referenceDocument;
									tempLiqDocument.descriptionDocument = detail.descriptionDocument;
									db.Entry(tempLiqDocument).State = EntityState.Modified;
								}

							}
						}
						#endregion

						if (approve)
						{
							modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
						}

						db.LiquidationFreightRiver.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;
						db.SaveChanges();
						trans.Commit();

						TempData["LiquidationFreightRiver"] = modelItem;
						TempData.Keep("LiquidationFreightRiver");
						ViewData["EditMessage"] = SuccessMessage("Liquidación de Flete: " + modelItem.Document.number + " guardada exitosamente");
					}


					catch (Exception e)
					{
						TempData.Keep("LiquidationFreightRiver");
						ViewData["EditMessage"] = ErrorMessage(e.Message);
						trans.Rollback();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			TempData.Keep("LiquidationFreightRiver");

            foreach (var detail in modelItem.LiquidationFreightRiverDetail)
            {
                detail.RemissionGuideRiver.requiredLogistic = detail.RemissionGuideRiver.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

            }

            return PartialView("_LiquidationFreightRiverMainFormPartial", modelItem);
		}
		#endregion

		#region CHANGE DOCUMENT STATE

		[HttpPost]
		public ActionResult Approve(int id)
		{
			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id);



			//if (liquidationFreightRiver.pricetotal <= 0)
			//{
			//    TempData.Keep("LiquidationFreightRiver");
			//    ViewData["EditMessage"] = ErrorMessage("No se puede Aprobar una Liquidacion de Flete con  Totales menores o igual a 0");
			//    return PartialView("_LiquidationFreightRiverMainFormPartial", liquidationFreightRiver);
			//}

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

					if (liquidationFreightRiver != null && documentState != null)
					{
						liquidationFreightRiver.Document.id_documentState = documentState.id;
						liquidationFreightRiver.Document.DocumentState = documentState;



						db.LiquidationFreightRiver.Attach(liquidationFreightRiver);
						db.Entry(liquidationFreightRiver).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
					}
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
					trans.Rollback();
				}
			}

			TempData["LiquidationFreightRiver"] = liquidationFreightRiver;
			TempData.Keep("LiquidationFreightRiver");

            foreach (var detail in liquidationFreightRiver.LiquidationFreightRiverDetail)
            {
                detail.RemissionGuideRiver.requiredLogistic = detail.RemissionGuideRiver.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

            }

            return PartialView("_LiquidationFreightRiverMainFormPartial", liquidationFreightRiver);
		}

		[HttpPost]
		public ActionResult Autorize(int id)
		{
			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id);

			//if (liquidationFreightRiver.pricetotal <= 0)
			//{
			//    TempData.Keep("LiquidationFreightRiver");
			//    ViewData["EditMessage"] = ErrorMessage("No se puede Autorizar una Liquidacion de Flete con  Totales menores o igual a 0");
			//    return PartialView("_LiquidationFreightRiverMainFormPartial", liquidationFreightRiver);
			//}

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					//DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada

					if (liquidationFreightRiver != null && documentState != null)
					{
						liquidationFreightRiver.Document.id_documentState = documentState.id;
						liquidationFreightRiver.Document.DocumentState = documentState;




						db.LiquidationFreightRiver.Attach(liquidationFreightRiver);
						db.Entry(liquidationFreightRiver).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						TempData["LiquidationFreightRiver"] = liquidationFreightRiver;
						TempData.Keep("LiquidationFreightRiver");

						ViewData["EditMessage"] = SuccessMessage("La Liquidación de Flete: " + liquidationFreightRiver.Document.number + " autorizada exitosamente");

					}
				}
				catch (Exception e)
				{
					TempData.Keep("LiquidationFreightRiver");
					ViewData["EditError"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}

            foreach (var detail in liquidationFreightRiver.LiquidationFreightRiverDetail)
            {
                detail.RemissionGuideRiver.requiredLogistic = detail.RemissionGuideRiver.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

            }

            return PartialView("_LiquidationFreightRiverMainFormPartial", liquidationFreightRiver);
		}

		[HttpPost]
		public ActionResult Protect(int id)
		{
			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id);

			//if (liquidationFreightRiver.pricetotal <= 0)
			//{
			//    TempData.Keep("LiquidationFreightRiver");
			//    ViewData["EditMessage"] = ErrorMessage("No se puede Proteger una Liquidacion de Flete con  Totales menores o igual a 0");
			//    return PartialView("_LiquidationFreightRiverMainFormPartial", liquidationFreightRiver);
			//}


			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

					if (liquidationFreightRiver != null && documentState != null)
					{
						liquidationFreightRiver.Document.id_documentState = documentState.id;
						liquidationFreightRiver.Document.DocumentState = documentState;

						db.LiquidationFreightRiver.Attach(liquidationFreightRiver);
						db.Entry(liquidationFreightRiver).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
					}
				}
				catch (Exception e)
				{
					ViewData["EditMessage"] = e.Message;
					trans.Rollback();
				}
			}

			TempData["LiquidationFreightRiver"] = liquidationFreightRiver;
			TempData.Keep("LiquidationFreightRiver");

            foreach (var detail in liquidationFreightRiver.LiquidationFreightRiverDetail)
            {
                detail.RemissionGuideRiver.requiredLogistic = detail.RemissionGuideRiver.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

            }

            return PartialView("_LiquidationFreightRiverMainFormPartial", liquidationFreightRiver);
		}

		[HttpPost]
		public ActionResult Cancel(int id)
		{
			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					//DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

					if (liquidationFreightRiver != null && documentState != null)
					{


						liquidationFreightRiver.Document.id_documentState = documentState.id;
						liquidationFreightRiver.Document.DocumentState = documentState;

						db.LiquidationFreightRiver.Attach(liquidationFreightRiver);
						db.Entry(liquidationFreightRiver).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						TempData["LiquidationFreightRiver"] = liquidationFreightRiver;
						TempData.Keep("LiquidationFreightRiver");

						ViewData["EditMessage"] = SuccessMessage("La Liquidación de Flete: " + liquidationFreightRiver.Document.number + " anulada exitosamente");

					}
				}
				catch (Exception e)
				{
					TempData.Keep("LiquidationFreightRiver");
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}


            foreach (var detail in liquidationFreightRiver.LiquidationFreightRiverDetail)
            {
                detail.RemissionGuideRiver.requiredLogistic = detail.RemissionGuideRiver.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

            }

            return PartialView("_LiquidationFreightRiverMainFormPartial", liquidationFreightRiver);
		}

		[HttpPost]
		public ActionResult Revert(int id)
		{
			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

					//DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

					if (liquidationFreightRiver != null && documentState != null)
					{
						liquidationFreightRiver.Document.id_documentState = documentState.id;
						liquidationFreightRiver.Document.DocumentState = documentState;



						db.LiquidationFreightRiver.Attach(liquidationFreightRiver);
						db.Entry(liquidationFreightRiver).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						TempData["LiquidationFreightRiver"] = liquidationFreightRiver;
						TempData.Keep("LiquidationFreightRiver");

						ViewData["EditMessage"] = SuccessMessage("La Liquidación de Flete: " + liquidationFreightRiver.Document.number + " reversada exitosamente");

					}
				}
				catch (Exception e)
				{
					TempData.Keep("LiquidationFreightRiver");
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}


            foreach (var detail in liquidationFreightRiver.LiquidationFreightRiverDetail)
            {
                detail.RemissionGuideRiver.requiredLogistic = detail.RemissionGuideRiver.RemissionGuideRiverDetail?.FirstOrDefault()?.RemissionGuide.RemissionGuideDetail.FirstOrDefault()?.RemissionGuideDetailPurchaseOrderDetail?.FirstOrDefault()?.PurchaseOrderDetail.PurchaseOrder.requiredLogistic ?? false;

            }

            return PartialView("_LiquidationFreightRiverMainFormPartial", liquidationFreightRiver);
		}

		#endregion

		#region  bach edit grid
		[ValidateInput(false)]
		public ActionResult LiquidationFreightRiverResultsDetailBatchEditingUpdateModel(MVCxGridViewBatchUpdateValues<LiquidationFreightRiverDetail, int> updateValues)
		{

			LiquidationFreightRiver tempLiquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
			tempLiquidationFreightRiver = tempLiquidationFreightRiver ?? new LiquidationFreightRiver();

			LiquidationFreightRiver LiquidationFreightRiverAUX = tempLiquidationFreightRiver;

			bool bcontinue = true;
			//decimal Total = 0, Subtotal = 0, extencion = 0;
			try
			{
				foreach (var detail in updateValues.Update)
				{
					if (updateValues.IsValid(detail))
					{
						LiquidationFreightRiverDetail liquidationFreightRiverDetail = LiquidationFreightRiverAUX.LiquidationFreightRiverDetail.Where(x => x.id_remisionGuideRiver == detail.id_remisionGuideRiver).FirstOrDefault();
						liquidationFreightRiverDetail.priceadjustment = detail.priceadjustment;
						liquidationFreightRiverDetail.pricedays = detail.pricedays;
						liquidationFreightRiverDetail.priceextension = detail.priceextension;
						liquidationFreightRiverDetail.pricesubtotal = liquidationFreightRiverDetail.price + liquidationFreightRiverDetail.priceadjustment + liquidationFreightRiverDetail.pricedays + liquidationFreightRiverDetail.priceextension - (liquidationFreightRiverDetail.PriceCancelled ?? 0);
						liquidationFreightRiverDetail.pricetotal = liquidationFreightRiverDetail.pricesubtotal - liquidationFreightRiverDetail.pricesavance;
						liquidationFreightRiverDetail.quantityPoundsTransported = detail.quantityPoundsTransported;
						liquidationFreightRiverDetail.descriptionRGR = detail.descriptionRGR;
					}
					else
					{
						bcontinue = false;
						updateValues.SetErrorText(detail, "Error");
					}
				}

				if (bcontinue)
				{

					var price = (from e in LiquidationFreightRiverAUX.LiquidationFreightRiverDetail
								 select e.price).Sum();

					var priceadjustment = (from e in LiquidationFreightRiverAUX.LiquidationFreightRiverDetail
										   select e.priceadjustment).Sum();

					var pricedays = (from e in LiquidationFreightRiverAUX.LiquidationFreightRiverDetail
									 select e.pricedays).Sum();

					var priceextension = (from e in LiquidationFreightRiverAUX.LiquidationFreightRiverDetail
										  select e.priceextension).Sum();

					var pricesubTotal = (from e in LiquidationFreightRiverAUX.LiquidationFreightRiverDetail
										 select e.pricesubtotal).Sum();


					var pricetotal = (from e in LiquidationFreightRiverAUX.LiquidationFreightRiverDetail
									  select e.pricetotal).Sum();

					LiquidationFreightRiverAUX.priceadjustment = priceadjustment;
					LiquidationFreightRiverAUX.pricedays = pricedays;
					LiquidationFreightRiverAUX.priceextension = priceextension;
					LiquidationFreightRiverAUX.pricesubtotal = pricesubTotal;
					LiquidationFreightRiverAUX.pricetotal = pricetotal;
					LiquidationFreightRiverAUX.price = price;

					tempLiquidationFreightRiver = LiquidationFreightRiverAUX;
					TempData["LiquidationFreightRiver"] = tempLiquidationFreightRiver;
				}

				TempData.Keep("LiquidationFreightRiver");

			}
			catch (Exception e)
			{
				updateValues.SetErrorText(0, e.Message);
			}

			// return PartialView("_LiquidationFreightPartial", tempLiquidationFreight);

			return PartialView("_LiquidationFreightRiverResultsDetailViewPartial", tempLiquidationFreightRiver.LiquidationFreightRiverDetail.ToList());
		}
		[HttpPost, ValidateInput(false)]
		public JsonResult BachEdiTItemDetailData()
		{

			LiquidationFreightRiver liquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);

			if (liquidationFreightRiver.pricedays <= 0.0M)
			{
				liquidationFreightRiver.pricedays = 0;
			}

			if (liquidationFreightRiver.priceextension <= 0.0M)
			{
				liquidationFreightRiver.priceextension = 0;
			}

			//if (liquidationFreight.priceadjustment <= 0.0M)
			//{
			//    liquidationFreight.priceadjustment = 0;
			//}
			var result = new
			{
				ItemData = new
				{
					pricedays = liquidationFreightRiver.pricedays,
					priceextension = liquidationFreightRiver.priceextension,
					priceadjustment = liquidationFreightRiver.priceadjustment,
					pricesubtotal = liquidationFreightRiver.pricesubtotal,
					pricetotal = liquidationFreightRiver.pricetotal,
					priceavance = liquidationFreightRiver.priceavance,
					priceCancelled = liquidationFreightRiver.PriceCancelledTotal,
					price = liquidationFreightRiver.price,

				},
			};

			TempData["LiquidationFreightRiver"] = liquidationFreightRiver;
			TempData.Keep("LiquidationFreightRiver");

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Report
		[HttpPost, ValidateInput(false)]
		public JsonResult PrintLiquidationFreightRiverReport(int id_lf)
		{
			LiquidationFreightRiver liquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
			liquidationFreightRiver = liquidationFreightRiver ?? new LiquidationFreightRiver();
			TempData["LiquidationFreightRiver"] = liquidationFreightRiver;
			TempData.Keep("LiquidationFreightRiver");

			#region "Armo Parametros"
			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param = new ParamCR();
			_param.Nombre = "@id";
			_param.Valor = id_lf;

			paramLst.Add(_param);

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = "LRFF1";
			_repMod.conex = objConex;
			_repMod.paramCRList = paramLst;

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);

			#endregion
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult PRLiquidationRiverRotation(string codeReport, LiquidationFreightRiver liquidationFreightriver,
												  Document document,
												  DateTime? startEmissionDate, DateTime? endEmissionDate,
												  DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
												  string carRegistration,
												  int? id_providerfilter,
												  int? id_owner,
												  int[] items)
		{
			#region "Armo Parametros"

			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param = new ParamCR();

			string str_starEmissionDate = "";
			if (startEmissionDate != null) { str_starEmissionDate = startEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
			_param = new ParamCR();
			_param.Nombre = "@dt_start";
			_param.Valor = str_starEmissionDate;
			paramLst.Add(_param);

			string str_endEmissionDate = "";
			if (endEmissionDate != null) { str_endEmissionDate = endEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
			_param = new ParamCR();
			_param.Nombre = "@dt_end";
			_param.Valor = str_endEmissionDate;
			paramLst.Add(_param);

			paramLst.Add(_param);

			_param = new ParamCR();
			_param.Nombre = "@idCompany";
			_param.Valor = ViewData["id_company"];
			paramLst.Add(_param);

			paramLst.Add(_param);

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = codeReport;
			_repMod.conex = objConex;
			_repMod.paramCRList = paramLst;

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);

			#endregion
		}
		#endregion

		#region LIQUIDATION UPLOAD FILE
		[ValidateInput(false)]
		public ActionResult LiquidationRiverDocumentsAttachedDocumentsPartial()
		{
			LiquidationFreightRiver _liquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
			_liquidationFreightRiver = _liquidationFreightRiver ?? new LiquidationFreightRiver();

			var model = _liquidationFreightRiver.LiquidationFreightRiverDocument;
			TempData.Keep("LiquidationFreightRiver");

			return PartialView("_LiquidationRiverDocumentsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationRiverDocumentsAttachedDocumentsPartialAddNew(DXPANACEASOFT.Models.LiquidationFreightRiverDocument liquidationFreightRiverDocument)
		{
			LiquidationFreightRiver _liquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
			_liquidationFreightRiver = _liquidationFreightRiver ?? new LiquidationFreightRiver();

			if (ModelState.IsValid)
			{
				try
				{
					if (string.IsNullOrEmpty(liquidationFreightRiverDocument.attachment))
					{
						throw new Exception("El Documento adjunto no puede ser vacio");
					}
					else
					{
						if (string.IsNullOrEmpty(liquidationFreightRiverDocument.guid) || string.IsNullOrEmpty(liquidationFreightRiverDocument.url))
						{
							throw new Exception("El fichero no se cargo completo, intente de nuevo");
						}
						else
						{
							var liquidationRiverDocumentDetailAux = _liquidationFreightRiver
								.LiquidationFreightRiverDocument
								.FirstOrDefault(fod => fod.attachment == liquidationFreightRiverDocument.attachment);
							if (liquidationRiverDocumentDetailAux != null)
							{
								throw new Exception("No se puede repetir el Documento Adjunto: " + liquidationFreightRiverDocument.attachment + ", en el detalle de los Documentos Adjunto.");
							}

						}
					}
					liquidationFreightRiverDocument.id = _liquidationFreightRiver.LiquidationFreightRiverDocument.Count() > 0 ? _liquidationFreightRiver.LiquidationFreightRiverDocument.Max(pld => pld.id) + 1 : 1;
					_liquidationFreightRiver.LiquidationFreightRiverDocument.Add(liquidationFreightRiverDocument);
					TempData["LiquidationFreightRiver"] = _liquidationFreightRiver;
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";

			TempData.Keep("LiquidationFreightRiver");
			var model = _liquidationFreightRiver.LiquidationFreightRiverDocument;

			return PartialView("_LiquidationRiverDocumentsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());

		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationRiverDocumentsAttachedDocumentsPartialUpdate(DXPANACEASOFT.Models.LiquidationFreightDocument liquidationFreightRiverDocument)
		{
			LiquidationFreightRiver _liquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
			_liquidationFreightRiver = _liquidationFreightRiver ?? new LiquidationFreightRiver();

			if (ModelState.IsValid)
			{
				try
				{
					var modelItem = _liquidationFreightRiver.LiquidationFreightRiverDocument.FirstOrDefault(i => i.id == _liquidationFreightRiver.id);
					if (string.IsNullOrEmpty(liquidationFreightRiverDocument.attachment))
					{
						throw new Exception("El Documento adjunto no puede ser vacio");
					}
					else
					{
						if (string.IsNullOrEmpty(liquidationFreightRiverDocument.guid) || string.IsNullOrEmpty(liquidationFreightRiverDocument.url))
						{
							throw new Exception("El fichero no se cargo completo, intente de nuevo");
						}
						else
						{
							if (modelItem.attachment != liquidationFreightRiverDocument.attachment)
							{
								var liquidationRiverDocumentDetailAux =
									_liquidationFreightRiver
									.LiquidationFreightRiverDocument
									.FirstOrDefault(fod => fod.attachment == liquidationFreightRiverDocument.attachment);
								if (liquidationRiverDocumentDetailAux != null)
								{
									throw new Exception("No se puede repetir el Documento Adjunto: " + liquidationFreightRiverDocument.attachment + ", en el detalle de los Documentos Adjunto.");
								}
							}
						}
					}
					if (modelItem != null)
					{
						modelItem.guid = liquidationFreightRiverDocument.guid;
						modelItem.url = liquidationFreightRiverDocument.url;
						modelItem.attachment = liquidationFreightRiverDocument.attachment;
						modelItem.referenceDocument = liquidationFreightRiverDocument.referenceDocument;
						modelItem.descriptionDocument = liquidationFreightRiverDocument.descriptionDocument;

						this.UpdateModel(modelItem);
					}
					TempData["LiquidationFreightRiver"] = _liquidationFreightRiver;
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";

			TempData.Keep("LiquidationFreight");
			var model = _liquidationFreightRiver.LiquidationFreightRiverDocument;


			return PartialView("_LiquidationRiverDocumentsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationRiverDocumentsAttachedDocumentsPartialDelete(System.Int32 id)
		{
			LiquidationFreightRiver _liquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
			_liquidationFreightRiver = _liquidationFreightRiver ?? new LiquidationFreightRiver();

			try
			{
				var liquidationFreightRiverDocument = _liquidationFreightRiver.LiquidationFreightRiverDocument.FirstOrDefault(it => it.id == id);
				if (liquidationFreightRiverDocument != null)
					_liquidationFreightRiver.LiquidationFreightRiverDocument.Remove(liquidationFreightRiverDocument);

				TempData["LiquidationFreightRiver"] = _liquidationFreightRiver;
			}
			catch (Exception e)
			{
				ViewData["EditError"] = e.Message;
			}

			TempData.Keep("item");
			var model = _liquidationFreightRiver.LiquidationFreightRiverDocument;

			return PartialView("_LiquidationRiverDocumentsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
		}

		#endregion

		#region LIQUIDATION DOCUMENT ATTACHED DOCUMENTS
		private void LiquidationRiverDocumentsUpdateAttachment(LiquidationFreightRiver liquidationRiver)
		{
			List<LiquidationFreightRiverDocument> lst = liquidationRiver.LiquidationFreightRiverDocument.ToList() ?? new List<LiquidationFreightRiverDocument>();
			foreach (var itemTmp in lst)
			{
				if (itemTmp.url == FileUploadHelper.UploadDirectoryDefaultTemp)
				{
					try
					{
						// Carga el contenido guardado en el temp
						string nameAttachment;
						string typeContentAttachment;
						string guidAux = itemTmp.guid;
						string urlAux = itemTmp.url;
						var contentAttachment = FileUploadHelper.ReadFileUpload(
							ref guidAux, out nameAttachment, out typeContentAttachment, pathFiles + "\\LiquidacionFleteFluvial\\");

						// Guardamos en el directorio final el fichero que este aun en su ruta temporal
						itemTmp.guid = FileUploadHelper.FileUploadProcessAttachment(pathFiles + "\\LiquidacionFleteFluvialTmp\\" + liquidationRiver.id.ToString(), nameAttachment, typeContentAttachment, contentAttachment, out urlAux);
						itemTmp.url = urlAux;

					}
					catch (Exception exception)
					{
						throw new Exception("Error al guardar el adjunto. Error: " + exception.Message);
					}

				}
			}
		}

		private void LiquidationRiverDocumentsDeleteAttachment(LiquidationFreightRiverDocument liqDocument)
		{
			if (liqDocument.url != FileUploadHelper.UploadDirectoryDefaultTemp)
			{
				try
				{
					// Carga el contenido guardado en el temp
					FileUploadHelper.CleanUpUploadedFiles(liqDocument.url, liqDocument.guid);

				}
				catch (Exception exception)
				{
					throw new Exception("Error al borrar el adjunto. Error: " + exception.Message);
				}
			}
		}

		[HttpGet]
		[ActionName("TSdownload-attachment")]
		public ActionResult LiquidationRiverDocumentsDownloadAttachment(int id)
		{
			//TempData.Keep("item");

			try
			{
				LiquidationFreightRiver _liquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);
				TempData.Keep("LiquidationFreightRiver");
				List<LiquidationFreightRiverDocument> liqDocument = _liquidationFreightRiver?.LiquidationFreightRiverDocument?.ToList() ?? new List<LiquidationFreightRiverDocument>();


				var liqDocumentAux = liqDocument.FirstOrDefault(fod => fod.id == id);
				if (liqDocumentAux != null)
				{
					// Carga el contenido guardado en el temp
					string nameAttachment;
					string typeContentAttachment;
					string guidAux = liqDocumentAux.guid;
					string urlAux = liqDocumentAux.url;
					var contentAttachment = FileUploadHelper.ReadFileUpload(
						ref guidAux, ref urlAux, out nameAttachment, out typeContentAttachment, pathFiles + "\\LiquidacionFleteFluvial\\");

					return this.File(contentAttachment, typeContentAttachment, nameAttachment);
				}
				else
				{
					//return this.File(new byte[] { }, "", "");
					return null;
				}

			}
			catch (Exception exception)
			{
				throw new Exception("Error al bajar el adjunto. Error: " + exception.Message);
			}
		}

		#endregion

		#region UPLOAD FILE
		[HttpPost]
		[ActionName("TSupload-attachment")]
		public ActionResult UploadControlUpload()
		{
			TempData.Keep("LiquidationFreightRiver");
			UploadControlExtension.GetUploadedFiles(
				"attachmentUploadControl", UploadControlSettings.UploadValidationSettings, UploadControlSettings.FileUploadComplete);

			return null;
		}

		public class UploadControlSettings
		{
			public static string pathFiles = ConfigurationManager.AppSettings["RootDirectoryServerUploadFiles"];
			public static string pathFilesCopy = ConfigurationManager.AppSettings["RootDirectoryServerUploadFilesCopy"];

			public readonly static UploadControlValidationSettings UploadValidationSettings;

			static UploadControlSettings()
			{
				UploadValidationSettings = new UploadControlValidationSettings()
				{
					MaxFileSize = FileUploadHelper.MaxFileUploadSize,
					MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
				};
			}

			public static void FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
			{
				if (e.UploadedFile.IsValid)
				{
					e.CallbackData = FileUploadHelper.FileUploadProcess(e, pathFiles + "\\LiquidacionFleteFluvial\\");
				}
			}
		}

		public JsonResult ItsRepeatedAttachmentDetail(string attachmentNameNew)
		{
			LiquidationFreightRiver _liquidationFreightRiver = (TempData["LiquidationFreightRiver"] as LiquidationFreightRiver);

			_liquidationFreightRiver = _liquidationFreightRiver ?? new LiquidationFreightRiver();
			var result = new
			{
				itsRepeated = 0,
				Error = ""
			};

			var liquidationRiverDocumentDetailAux = _liquidationFreightRiver
														.LiquidationFreightRiverDocument
														.FirstOrDefault(fod => fod.attachment == attachmentNameNew);

			if (liquidationRiverDocumentDetailAux != null)
			{
				result = new
				{
					itsRepeated = 1,
					Error = "No se puede repetir el Documento Adjunto: " + attachmentNameNew + ", en el detalle de los Documentos Adjunto."

				};
			}
			TempData.Keep("LiquidationFreightRiver");
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
