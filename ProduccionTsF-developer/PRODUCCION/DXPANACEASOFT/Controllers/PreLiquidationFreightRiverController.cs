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
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models.RemGuide;
using Utilitarios.Logs;
using System.Configuration;
using System.Data;

namespace DXPANACEASOFT.Controllers
{
	public class PreLiquidationFreightRiverController : DefaultController
	{

		public ActionResult Index()
		{
			return View();
		}

		#region "REMISSION GUIDE RIVER TRANSPORTATION RESULTS LISTS"

		[HttpPost]
		public ActionResult RemissionGuideRiverTransportationResults(RemissionGuide remissionGuide,
														  Document document,
														  DateTime? startEmissionDate, DateTime? endEmissionDate,
														  DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
														  DateTime? startDespachureDate, DateTime? endDespachureDate,
														  DateTime? startexitDateProductionBuilding, DateTime? endexitDateProductionBuilding,
														  DateTime? startentranceDateProductionBuilding, DateTime? endentranceDateProductionBuilding,
														  int[] items, int[] businessOportunities)
		{
			string ruta = ConfigurationManager.AppSettings["rutaLog"];

			var lstLFD = db.LiquidationFreightRiverDetail.Select(s => s.id_remisionGuideRiver).ToList();

			var model = db.RemissionGuideRiver
							.Where(w => (w.Document.DocumentState.code == "06" || w.Document.DocumentState.code == "08") &&
							w.hasEntrancePlanctProduction == true &&
										w.hasExitPlanctProduction == true && w.RemissionGuideRiverTransportation.valuePrice > 0 &&
										!lstLFD.Contains(w.id)).ToList();


			#region DOCUMENT FILTERS

			if (document.id_documentState != 0)
			{
				model = model.Where(o => o.Document.id_documentState == document.id_documentState).ToList();
			}

			if (!string.IsNullOrEmpty(document.number))
			{
				model = model.Where(o => o.Document.number.Contains(document.number)).ToList();
			}

			if (!string.IsNullOrEmpty(document.reference))
			{
				model = model.Where(o => o.Document.reference.Contains(document.reference)).ToList();
			}

			if (startEmissionDate != null && endEmissionDate != null)
			{
				model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0 && DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
			}

			if (startAuthorizationDate != null && endAuthorizationDate != null)
			{
				model = model.Where(o => o.Document.authorizationDate != null && DateTime.Compare(startAuthorizationDate.Value.Date, o.Document.authorizationDate.Value.Date) <= 0 && DateTime.Compare(o.Document.authorizationDate.Value.Date, endAuthorizationDate.Value.Date) <= 0).ToList();
			}
			//if (startexitDateProductionBuilding != null && endexitDateProductionBuilding != null)
			//{
			//    model = model.Where(o => (o.RemissionGuideControlVehicle.exitDateProductionBuilding != null) &&
			//    (DateTime.Compare(startexitDateProductionBuilding.Value.Date, o.RemissionGuideControlVehicle.exitDateProductionBuilding.Value.Date) <= 0 &&
			//    DateTime.Compare(o.RemissionGuideControlVehicle.exitDateProductionBuilding.Value.Date, endexitDateProductionBuilding.Value.Date) <= 0)).ToList();
			//}
			//if (startentranceDateProductionBuilding != null && endentranceDateProductionBuilding != null)
			//{
			//    model = model.Where(o => (o.RemissionGuideControlVehicle.entranceDateProductionBuilding != null) &&
			//    (DateTime.Compare(startentranceDateProductionBuilding.Value.Date, o.RemissionGuideControlVehicle.entranceDateProductionBuilding.Value.Date) <= 0 &&
			//    DateTime.Compare(o.RemissionGuideControlVehicle.entranceDateProductionBuilding.Value.Date, endentranceDateProductionBuilding.Value.Date) <= 0)).ToList();
			//}

			if (!string.IsNullOrEmpty(document.accessKey))
			{
				model = model.Where(o => o.Document.accessKey.Contains(document.accessKey)).ToList();
			}

			if (!string.IsNullOrEmpty(document.authorizationNumber))
			{
				model = model.Where(o => o.Document.authorizationNumber.Contains(document.authorizationNumber)).ToList();
			}

			#endregion

			#region PURCHASE ORDER FILTERS

			if (startDespachureDate != null && endDespachureDate != null)
			{
				model = model.Where(o => DateTime.Compare(startDespachureDate.Value.Date, o.despachureDate.Date) <= 0 && DateTime.Compare(o.despachureDate.Date, endDespachureDate.Value.Date) <= 0).ToList();
			}

			//QUITAR ITEMS Y BUSINESS OPORTUNITY

			//if (items != null && items.Length > 0)
			//{
			//    var tempRemissionGuide = new List<RemissionGuideRI>();
			//    foreach (var qp in model)
			//    {
			//        var details = qp.remi.Where(d => items.Contains(d.id_item));
			//        if (details.Any())
			//        {
			//            tempRemissionGuide.Add(qp);
			//        }

			//    }
			//    model = tempRemissionGuide;
			//}

			//if (businessOportunities != null && businessOportunities.Length > 0)
			//{
			//    var tempRemissionGuide = new List<RemissionGuide>();
			//    foreach (var qp in model)
			//    {
			//        var details = qp.RemissionGuideDetail.Where(d => businessOportunities.Contains(d.BusinessOportunityPlanningDetail?.BusinessOportunityPlaninng?.BusinessOportunity.id ?? 0));
			//        if (details.Any())
			//        {
			//            tempRemissionGuide.Add(qp);
			//        }

			//    }
			//    model = tempRemissionGuide;
			//}

			//model = model.Where(o => o.isExport == remissionGuide.isExport).ToList();
			List<RemissionGuideRiver> lstModel = new List<RemissionGuideRiver>();
			lstModel = model
						.Select(s => new RemissionGuideRiver
						{
							id = s.id,
							Document = DataProviderDocument.Document(s.id),
							Provider = DataProviderPerson.Provider(s.ProductionUnitProvider.id_provider),
							//id_providerRemisionGuide = s.ProductionUnitProvider.id_provider,
							//id_buyer = s.,
							ProductionUnitProvider = DataProviderProductionUnitProvider.ProductionUnitProviderById(s.id_productionUnitProvider),
							id_productionUnitProvider = s.id_productionUnitProvider,
							//emissionDateRG = new DateTime(DataProviderDocument.Document(s.id).emissionDate.Year, DataProviderDocument.Document(s.id).emissionDate.Month, DataProviderDocument.Document(s.id).emissionDate.Day, 0, 0, 0),
							//RemissionGuideControlVehicle = DataProviderRemissionGuide.RemissionGuideControlVehicle(s.id),
							id_reciver = s.id_reciver,
							id_reason = s.id_reason,
							route = s.route,
							startAdress = s.startAdress,
							despachureDate = s.despachureDate,
							//arrivalDate = s.arrivalDate,
							//returnDate = s.returnDate,
							//isExport = s.isExport,
							//id_priceList = s.id_priceList,
							//id_protectiveProvider = s.id_protectiveProvider,
							//id_productionUnitProviderPool = s.id_productionUnitProviderPool,
							//descriptionpurchaseorder = s.descriptionpurchaseorder,
							//isInternal = s.isInternal,
							//id_RemisionGuideReassignment = s.id_RemisionGuideReassignment,
							id_shippingType = s.id_shippingType,
							//hasEntrancePlanctProduction = s.hasEntrancePlanctProduction,
							//hasExitPlanctProduction = s.hasExitPlanctProduction,
							despachurehour = s.despachurehour,
							id_TransportTariffType = s.id_TransportTariffType
							//RemissionGuideTransportation = s.RemissionGuideTransportation

						}).ToList();
			#endregion


			List<RemissionGuideRiverTransportLiq> lstLeftJoinRGT = new List<RemissionGuideRiverTransportLiq>();
			var lstRGLT = db.RemGuideRiverLiqTransportation.ToList();

			lstLeftJoinRGT = model.Select(s => new RemissionGuideRiverTransportLiq
			{
				id_remissionGuideRiver = s.id,
				number = s.Document.number,
				emissionDate = s.Document.emissionDate,
				id_personaProcessPlant = s.id_personProcessPlant,
				personaProcessPlant = s.Person1.processPlant,
				nameProvider = s.Provider?.Person?.fullname_businessName ?? "",
				nameDriver = s.RemissionGuideRiverTransportation?.DriverVeicleProviderTransport?.Person?.fullname_businessName ?? "",
				namePlac = s.RemissionGuideRiverTransportation?.Vehicle?.carRegistration ?? "",
				nameSite = s.ProductionUnitProvider?.FishingSite?.name ?? "",
				nameZone = s.ProductionUnitProvider?.FishingSite?.FishingZone?.name ?? "",
				price = s.RemissionGuideRiverTransportation?.valuePrice ?? 0,
				priceAdvance = s.RemissionGuideRiverTransportation?.advancePrice ?? 0,
				priceCancelled = s.Document.DocumentState.code.Equals("08") ? (s.RemissionGuideRiverTransportation?.valuePrice ?? 0) : 0,
				priceAdjustment = 0,
				priceDays = 0,
				priceExtension = 0,
				priceTotal = (s.RemissionGuideRiverTransportation?.valuePrice ?? 0) - (s.RemissionGuideRiverTransportation?.advancePrice ?? 0) - (s.Document.DocumentState.code.Equals("08") ? (s.RemissionGuideRiverTransportation?.valuePrice ?? 0) : 0),
				quantityPoundsTransported = 0
			}).ToList();

			if (lstRGLT != null && lstRGLT.Count > 0)
			{
				foreach (var el in lstRGLT)
				{
					var eTmp = lstLeftJoinRGT.FirstOrDefault(fod => fod.id_remissionGuideRiver == el.id_remisionGuideRiver);
					if (eTmp != null)
					{
						//eTmp.price = el.price;
						//eTmp.priceAdvance = el.priceadvance;
						eTmp.priceCancelled = el.PriceCancelled;
						eTmp.priceAdjustment = el.priceadjustment;
						eTmp.priceDays = el.pricedays;
						eTmp.priceExtension = el.priceextension;
						eTmp.priceTotal = el.pricetotal;
						eTmp.quantityPoundsTransported = el.quantityPoundsTransported;
						eTmp.descriptionRG = el.descriptionRGR;
					}
				}
			}

			lstLeftJoinRGT = lstLeftJoinRGT.OrderByDescending(r => r.id_remissionGuideRiver).ToList();

			TempData["model"] = lstLeftJoinRGT;
			TempData.Keep("model");

			return PartialView("_PreLiqRiverMainFormPartial", lstLeftJoinRGT);
		}
		public ActionResult PrelimLiquidationRiverPartial()
		{
			var model = (TempData["model"] as List<RemissionGuideTransportLiq>);
			model = model ?? new List<RemissionGuideTransportLiq>();
			TempData.Keep("model");
			return PartialView("_RemGuideRiverTransportationDetail", model.ToList());
		}

		#endregion

		#region Edit LiquidationFreight
		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditLiquidationFreightRiver(int id, int[] orderDetails)
		{
			LiquidationFreightRiver liquidationfreight = db.LiquidationFreightRiver.FirstOrDefault(o => o.id == id);

			if (liquidationfreight == null)
			{
				DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("67"));
				DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

				liquidationfreight = new LiquidationFreightRiver
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
					LiquidationFreightRiverDetail = new List<LiquidationFreightRiverDetail>(),
				};
			}

			TempData["LiquidationFreightRiver"] = liquidationfreight;
			TempData.Keep("LiquidationFreightRiver");

			return PartialView("_FormEditLiquidationFreightRiver", liquidationfreight);
		}
		#endregion

		#region LIQUIDACION DE FLETE DETAILS
		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationFreightRiverResultsDetailViewPartial()
		{
			int id_LiquidationFreight = (Request.Params["id_LiquidationFreight"] != null && Request.Params["id_LiquidationFreight"] != "") ? int.Parse(Request.Params["id_LiquidationFreight"]) : -1;
			LiquidationFreight LiquidationFreight = db.LiquidationFreight.FirstOrDefault(r => r.id == id_LiquidationFreight);
			return PartialView("_LiquidationFreightRiverResultsDetailViewPartial", LiquidationFreight.LiquidationFreightDetail.ToList());
		}
		public ActionResult LiquidationFreightRiverDetailViewDetailsPartial()
		{
			int id_LiquidationFreight = (Request.Params["id_LiquidationFreight"] != null && Request.Params["id_LiquidationFreight"] != "") ? int.Parse(Request.Params["id_LiquidationFreight"]) : -1;
			LiquidationFreightRiver liquidationFreightRiver = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id_LiquidationFreight);
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

			LiquidationFreightRiver LiquidationFreight = db.LiquidationFreightRiver.FirstOrDefault(r => r.id == id);

			string state = LiquidationFreight.Document.DocumentState.code;

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

		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_LiquidationFreight)
		{
			TempData.Keep("LiquidationFreight");
			int index = db.LiquidationFreightRiver.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_LiquidationFreight);
			var result = new
			{
				maximunPages = db.LiquidationFreight.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			LiquidationFreightRiver LiquidationFreight = db.LiquidationFreightRiver.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (LiquidationFreight != null)
			{
				TempData["LiquidationFreightRiver"] = LiquidationFreight;
				TempData.Keep("LiquidationFreightRiver");
				return PartialView("_LiquidationFreightRiverMainFormPartial", LiquidationFreight);
			}

			TempData.Keep("LiquidationFreightRiver");

			return PartialView("_LiquidationFreighRivertMainFormPartial", new LiquidationFreight());
		}
		#endregion
		[HttpPost, ValidateInput(false)]
		public JsonResult ItemDetailData(string pricedays, string priceextension, string priceadjustment)
		{
			decimal _pricedays = Convert.ToDecimal(pricedays);
			decimal _priceextension = Convert.ToDecimal(priceextension);
			decimal _priceadjustment = Convert.ToDecimal(priceadjustment);

			LiquidationFreight liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);

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

			liquidationFreight.pricedays = _pricedays;
			liquidationFreight.priceextension = _priceextension;
			liquidationFreight.priceadjustment = _priceadjustment;
			liquidationFreight.pricetotal = (liquidationFreight.pricesubtotal + _pricedays + _priceextension + _priceadjustment) - liquidationFreight.priceavance;

			var result = new
			{
				ItemData = new
				{
					pricedays = _pricedays,
					priceextension = _priceextension,
					priceadjustment = _priceadjustment,
					pricetotal = liquidationFreight.pricetotal,
					pricesubtotal = liquidationFreight.pricesubtotal,
					priceavance = liquidationFreight.priceavance,
					price = liquidationFreight.priceavance
				},

			};

			TempData["LiquidationFreight"] = liquidationFreight;
			TempData.Keep("LiquidationFreight");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		#region "BATCH EDIT "
		[ValidateInput(false)]
		public ActionResult PrelimLiquidationRiverBatchEdit(MVCxGridViewBatchUpdateValues<RemissionGuideRiverTransportLiq, int> updateValues)
		{
			string ruta = ConfigurationManager.AppSettings["rutaLog"];

			List<RemissionGuideRiverTransportLiq> tempPreLiqFri = (TempData["model"] as List<RemissionGuideRiverTransportLiq>);
			tempPreLiqFri = tempPreLiqFri ?? new List<RemissionGuideRiverTransportLiq>();

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					foreach (var detail in updateValues.Update)
					{
						if (updateValues.IsValid(detail))
						{
							RemGuideRiverLiqTransportation rglt = db.RemGuideRiverLiqTransportation.FirstOrDefault(fod => fod.id_remisionGuideRiver == detail.id_remissionGuideRiver);
							RemissionGuideRiverTransportLiq rgtl = tempPreLiqFri.FirstOrDefault(fod => fod.id_remissionGuideRiver == detail.id_remissionGuideRiver);

							if (rgtl != null)
							{
								rgtl.priceAdjustment = detail.priceAdjustment;
								rgtl.priceDays = detail.priceDays;
								rgtl.priceExtension = detail.priceExtension;
								rgtl.priceSubTotal = rgtl.price + rgtl.priceAdjustment + rgtl.priceDays + rgtl.priceExtension;
								rgtl.priceTotal = rgtl.priceSubTotal - (rgtl.priceAdvance ?? 0) - (rgtl.priceCancelled ?? 0);
								rgtl.descriptionRG = detail.descriptionRG;
								rgtl.quantityPoundsTransported = detail.quantityPoundsTransported;
							}

							if (rglt != null)
							{
								rglt.PriceCancelled = rgtl.priceCancelled ?? 0;
								rglt.price = rgtl.price ?? 0;
								rglt.priceadvance = rgtl.priceAdvance ?? 0;
								rglt.priceadjustment = rgtl.priceAdjustment ?? 0;
								rglt.pricedays = rgtl.priceDays ?? 0;
								rglt.priceextension = rgtl.priceExtension ?? 0;
								rglt.pricesubtotal = (rgtl.price ?? 0) + (rgtl.priceAdjustment ?? 0) + (rgtl.priceDays ?? 0) + (rgtl.priceExtension ?? 0);
								rglt.pricetotal = rglt.pricesubtotal - (rgtl.priceAdvance ?? 0);
								rglt.quantityPoundsTransported = rgtl.quantityPoundsTransported ?? 0;
								rglt.descriptionRGR = rgtl.descriptionRG;
								db.RemGuideRiverLiqTransportation.Attach(rglt);
								db.Entry(rglt).State = EntityState.Modified;
							}
							else
							{
								rglt = new RemGuideRiverLiqTransportation();
								rglt.PriceCancelled = rgtl.priceCancelled;
								rglt.id_remisionGuideRiver = rgtl.id_remissionGuideRiver;
								rglt.price = rgtl.price ?? 0;
								rglt.priceadvance = rgtl.priceAdvance ?? 0;
								rglt.priceadjustment = rgtl.priceAdjustment ?? 0;
								rglt.pricedays = rgtl.priceDays ?? 0;
								rglt.priceextension = rgtl.priceExtension ?? 0;
								rglt.pricesubtotal = (rgtl.price ?? 0) + (rgtl.priceAdjustment ?? 0) + (rgtl.priceDays ?? 0) + (rgtl.priceExtension ?? 0);
								rglt.pricetotal = rglt.pricesubtotal - (rgtl.priceAdvance ?? 0);
								rglt.quantityPoundsTransported = rgtl.quantityPoundsTransported ?? 0;
								rglt.descriptionRGR = rgtl.descriptionRG;
								db.RemGuideRiverLiqTransportation.Attach(rglt);
								db.Entry(rglt).State = EntityState.Added;
							}
						}
						else
						{
							updateValues.SetErrorText(detail, "Error");
						}
					}
					db.SaveChanges();
					trans.Commit();
				}
				catch (Exception e)
				{

					MetodosEscrituraLogs.EscribeExcepcionLog(e, ruta, "PreLiquidacion", "PROD");
					trans.Rollback();
					updateValues.SetErrorText(0, e.Message);
				}
			}
			TempData["model"] = tempPreLiqFri;
			TempData.Keep("model");
			return PartialView("_RemGuideRiverTransportationDetail", tempPreLiqFri.ToList());
		}
		[HttpPost, ValidateInput(false)]
		public JsonResult BachEdiTItemDetailData()
		{

			LiquidationFreight liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);

			if (liquidationFreight.pricedays <= 0.0M)
			{
				liquidationFreight.pricedays = 0;
			}

			if (liquidationFreight.priceextension <= 0.0M)
			{
				liquidationFreight.priceextension = 0;
			}

			if (liquidationFreight.priceadjustment <= 0.0M)
			{
				liquidationFreight.priceadjustment = 0;
			}
			var result = new
			{
				ItemData = new
				{
					pricedays = liquidationFreight.pricedays,
					priceextension = liquidationFreight.priceextension,
					priceadjustment = liquidationFreight.priceadjustment,
					pricesubtotal = liquidationFreight.pricesubtotal,
					pricetotal = liquidationFreight.pricetotal,
					priceavance = liquidationFreight.priceavance,
					price = liquidationFreight.price
				},
			};

			TempData["LiquidationFreight"] = liquidationFreight;
			TempData.Keep("LiquidationFreight");

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Report
		public JsonResult LiquidationFreightRiverReportFilter(ReportModel reportModel)
		{
			LiquidationFreight liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);

			bool isvalid = false;
			string message = "";
			string strnamedata = "reportModel" + DateTime.Now.ToString("yyyyMMddmmssfff");
			int cantidad = 1;

			try
			{

				int id_company = int.Parse(ViewData["id_company"].ToString());
				Setting settingCREI = db.Setting.FirstOrDefault(t => t.code == "CREI" && t.id_company == id_company);//CREI-Parametro de Cantidad antes de la Reimpresion
				if (settingCREI != null)
				{
					try
					{
						cantidad = settingCREI.value != null ? int.Parse(settingCREI.value.ToString()) : 1;
					}
					catch (Exception)
					{


					}
				}


				if (reportModel == null)
				{
					reportModel = new ReportModel();
					reportModel.ReportName = "LiquidationFreightReport";

				}
				else
				{
					if (reportModel.ListReportParameter == null)
					{

						reportModel.ListReportParameter = new List<ReportParameter>();

					}

					printcontrol printcontrol = new printcontrol()
					{
						id_referencia = liquidationFreight.id,
						namereport = reportModel.ReportName,
						optiondescrip = "LiquidationFreight",
						dateCreate = DateTime.Now,
						id_userCreate = ActiveUser.id,
						isActive = true,
						dateUpdate = DateTime.Now,
						id_userUpdate = ActiveUser.id,
						printnumber = 1
					};


					printcontrol = DataProviders.DataProviderPrintControl.SaveControlPrint(printcontrol);



					//ReportParameter reportParameter = new ReportParameter();
					//reportParameter.Name = "id_company";
					//reportParameter.Value = ViewData["id_company"].ToString();
					//reportModel.ListReportParameter.Add(reportParameter);


					//reportParameter = new ReportParameter();
					//reportParameter.Name = "id_LiquidationFreight";
					//reportParameter.Value = liquidationFreight.id.ToString();
					//reportModel.ListReportParameter.Add(reportParameter);

					//reportParameter = new ReportParameter();
					//reportParameter.Name = "Reimpresion";
					//reportParameter.Value = printcontrol.printnumber - 1 >= cantidad ? "SI" : "NO";
					//reportModel.ListReportParameter.Add(reportParameter);





				}

				isvalid = true;
			}
			catch //(Exception ex)
			{


			}


			TempData[strnamedata] = reportModel;
			TempData.Keep(strnamedata);
			TempData.Keep("LiquidationFreight");




			var result = new
			{
				isvalid,
				message,
				reportModel = strnamedata,
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
