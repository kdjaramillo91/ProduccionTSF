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
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using DevExpress.Web;
using DevExpress.Utils;
using System.Configuration;
using System.Windows.Media.Media3D;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models.ComboBoxes;

namespace DXPANACEASOFT.Controllers
{
	public class LiquidationFreightController : DefaultController
	{
		public string pathFiles = ConfigurationManager.AppSettings["RootDirectoryServerUploadFiles"];
		public string pathFilesCopy = ConfigurationManager.AppSettings["RootDirectoryServerUploadFilesCopy"];

		const string codigoParametroGuiaDistintosProveedores = "LIQANYPROV";

        public ActionResult Index()
		{
			return View();
		}

		#region LIQUITACION DE FLETE GUIDE FILTERS RESULTS

		[HttpPost]
		public ActionResult LiquidationFreightResults(string numberGuide, LiquidationFreight liquidationFreight,
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
			var model = db.LiquidationFreight.ToList();

			#region DOCUMENT FILTERS
			if (!string.IsNullOrEmpty(numberGuide))
			{
				model = model.Where(w => w.LiquidationFreightDetail.Any(a => a.RemissionGuide.Document.sequential.ToString().Contains(numberGuide))).ToList();
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
				model = model.Where(o => o.LiquidationFreightDetail.Any(x => x.RemissionGuide.RemissionGuideTransportation.Vehicle.carRegistration.Contains(carRegistration))).ToList();
			}

			if (liquidationFreight != null && liquidationFreight.id_providertransport > 0)
			{
				model = model.Where(o => o.id_providertransport == liquidationFreight.id_providertransport).ToList();
			}

			if (id_providerfilter !=null && id_providerfilter > 0)
			{
				model = model.Where(o => o.LiquidationFreightDetail.Any(p => p.RemissionGuide.id_providerRemisionGuide == id_providerfilter)).ToList();
			}
			if (id_owner != null && id_owner > 0)
			{
				model = model.Where(o => o.LiquidationFreightDetail.Any(p => p.RemissionGuide.RemissionGuideTransportation.Vehicle.id_personOwner == id_owner)).ToList();
			}

			#endregion


			TempData["model"] = model;
			TempData.Keep("model");

			return PartialView("_LiquidationFreightResultsPartial", model.OrderByDescending(r => r.id).ToList());
		}


		#endregion

		#region LIQUITACION DE FLETE REMISSION GUIDE  RESULTS

		[HttpPost, ValidateInput(false)]
		public ActionResult RemissionGuideResults()
		{
			var model = QueryResult();

			return PartialView("_RemissionGuideDetailsResultsPartial", model);
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult RemissionGuideDetailsPartial()
		{
			var model = QueryResult();
			return PartialView("_RemissionGuideDetailsPartial", model);
		}
		#endregion

		#region LiquidationFreight HEADER
		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationFreightPartial()
		{
			var model = (TempData["model"] as List<LiquidationFreight>);
			model = model ?? new List<LiquidationFreight>();
			TempData.Keep("model");
			return PartialView("_LiquidationFreightPartial", model.OrderByDescending(r => r.id).ToList());
		}
		#endregion

		#region EDIT LIQUIDATION FREIGHT

		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditLiquidationFreight(int id, int[] orderDetails)
		{
			LiquidationFreight liquidationfreight = db.LiquidationFreight.FirstOrDefault(o => o.id == id);

			decimal _priceadjustment = 0;
			decimal _pricedays = 0;
			decimal _priceextension = 0;
			decimal _default = 0;
			string _descriptionRG = "";

			if (liquidationfreight == null)
			{
				DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("67"));
				DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

				liquidationfreight = new LiquidationFreight
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
					LiquidationFreightDetail = new List<LiquidationFreightDetail>(),
				};
			}

			if (orderDetails != null)
			{
				List<LiquidationFreightDetail> LiquidationFreightDetail = new List<LiquidationFreightDetail>();

				decimal dTotal = 0, dAvance=0, dPriceAdjustment = 0, dPriceDays = 0, dPriceExtension = 0, dPriceCancelled = 0;
				foreach (var od in orderDetails)
				{
					_priceadjustment = 0;
					_pricedays = 0;
					_priceextension = 0;
					_descriptionRG = "";

					RemissionGuide tempRemissionGuide = db.RemissionGuide.Where(d => d.id == od).FirstOrDefault();

					RemGuideLiqTransportation rmp = db.RemGuideLiqTransportation
														.FirstOrDefault(fod => fod.id_remisionGuide == od);

					if (rmp != null)
					{
						_priceadjustment = (rmp.priceadjustment > 0) ? rmp.priceadjustment : _default;
						_pricedays = (rmp.pricedays > 0) ? rmp.pricedays : _default;
						_priceextension = (rmp.priceextension > 0) ? rmp.priceextension : _default;

						_descriptionRG = rmp.descriptionRG;
					}

					LiquidationFreightDetail liquidationFreightDetail = new LiquidationFreightDetail()
					{
						id = liquidationfreight.LiquidationFreightDetail.Count() > 0 ? liquidationfreight.LiquidationFreightDetail.Max(pld => pld.id) + 1 : 1,
						id_remisionGuide = tempRemissionGuide.id,
						isActive = true,
						id_userCreate = ActiveUser.id,
						dateCreate = DateTime.Now,
						id_userUpdate = ActiveUser.id,
						dateUpdate = DateTime.Now,
						id_LiquidationFreight = liquidationfreight.id,
						RemissionGuide = tempRemissionGuide,
						LiquidationFreight = liquidationfreight,
						price = tempRemissionGuide?.RemissionGuideTransportation?.valuePrice ?? 0,
						pricesavance = tempRemissionGuide?.RemissionGuideTransportation?.advancePrice ?? 0,
						priceadjustment = _priceadjustment,
						pricedays = _pricedays,
						priceextension = _priceextension,
						PriceCancelled = 0,
						descriptionRG = _descriptionRG,
						pricesubtotal = (tempRemissionGuide?.RemissionGuideTransportation?.valuePrice ?? 0) + _pricedays + _priceextension + _priceadjustment,
						pricetotal = (tempRemissionGuide?.RemissionGuideTransportation?.valuePrice ?? 0) - (tempRemissionGuide?.RemissionGuideTransportation?.advancePrice ?? 0) + (_pricedays + _priceextension + _priceadjustment)
					};


					if (tempRemissionGuide?.Document?.DocumentState?.code == "08")
					{
						liquidationFreightDetail.PriceCancelled = tempRemissionGuide?.RemissionGuideTransportation?.valuePrice ?? 0;
						liquidationFreightDetail.pricesubtotal = liquidationFreightDetail.pricesubtotal - (liquidationFreightDetail.PriceCancelled ?? 0);
						liquidationFreightDetail.pricetotal = liquidationFreightDetail.pricetotal - (tempRemissionGuide?.RemissionGuideTransportation?.valuePrice ?? 0);
					}
					//Precios Nuevos
					dPriceAdjustment += liquidationFreightDetail.priceadjustment;
					dPriceDays += liquidationFreightDetail.pricedays;
					dPriceExtension += liquidationFreightDetail.priceextension;
					dPriceCancelled += liquidationFreightDetail.PriceCancelled ?? 0;

					dTotal += (liquidationFreightDetail.price);
					dAvance += (liquidationFreightDetail.pricesavance);

                    var setting = db.Setting.FirstOrDefault(r => r.code == codigoParametroGuiaDistintosProveedores);
                    string parametroGuiasDistintosProveedores = (setting?.value ?? "N");
					//Eliminar esto
					if (parametroGuiasDistintosProveedores == "N")
                    {
                        var id_vehicleTmpl = tempRemissionGuide?.RemissionGuideTransportation?.id_vehicle ?? 0;

                        int id_proviTmpl = 0;
                        if (id_vehicleTmpl != 0)
                        {
                            var tmp = db.VehicleProviderTransportBilling.FirstOrDefault(w => w.id_vehicle == id_vehicleTmpl && w.state && w.datefin == null);
                            if (tmp != null)
                            {
                                id_proviTmpl = tmp.id_provider;
                            }
                        }

                        if (liquidationfreight.id_providertransport == 0)
                        {
                            liquidationfreight.id_providertransport = (tempRemissionGuide != null) && (tempRemissionGuide.RemissionGuideTransportation != null) && (tempRemissionGuide.RemissionGuideTransportation.VehicleProviderTransportBilling != null) && (tempRemissionGuide.RemissionGuideTransportation.VehicleProviderTransportBilling.id_provider != 0) ? (int)tempRemissionGuide?.RemissionGuideTransportation?.VehicleProviderTransportBilling?.id_provider : id_proviTmpl;
                            liquidationfreight.Person = db.Person.Where(x => x.id == liquidationfreight.id_providertransport).FirstOrDefault() ?? (db.Person.FirstOrDefault(fod => fod.id == id_proviTmpl) ?? new Person());
                        }
                    }
                    
					LiquidationFreightDetail.Add(liquidationFreightDetail);
				}

				liquidationfreight.priceadjustment = dPriceAdjustment;
				liquidationfreight.pricedays = dPriceDays;
				liquidationfreight.priceextension = dPriceExtension;

				liquidationfreight.PriceCancelledTotal = dPriceCancelled;

				liquidationfreight.priceavance = dAvance;

				liquidationfreight.price = dTotal;
				//Se colocan los Totales
				dTotal = dTotal + dPriceAdjustment + dPriceDays + dPriceExtension - dPriceCancelled;
				liquidationfreight.pricesubtotal = dTotal;
				liquidationfreight.pricetotal = dTotal - (dAvance);

				liquidationfreight.LiquidationFreightDetail = LiquidationFreightDetail;
			}

			TempData["LiquidationFreight"] = liquidationfreight;
			TempData.Keep("LiquidationFreight");

			return PartialView("_FormEditLiquidationFreight", liquidationfreight);
		}

		#endregion

		#region LIQUIDACION DE FLETE DETAILS
		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationFreightResultsDetailViewPartial()
		{
			LiquidationFreight _lfTmp = (TempData["LiquidationFreight"] as LiquidationFreight);
			if (TempData["LiquidationFreight"] != null)
			{

				_lfTmp = _lfTmp ?? new LiquidationFreight();
				TempData["LiquidationFreight"] = _lfTmp;
				TempData.Keep("LiquidationFreight");
			}
			//int id_LiquidationFreight = (Request.Params["id_LiquidationFreight"] != null && Request.Params["id_LiquidationFreight"] != "") ? int.Parse(Request.Params["id_LiquidationFreight"]) : -1;
			//LiquidationFreight LiquidationFreight = db.LiquidationFreight.FirstOrDefault(r => r.id == id_LiquidationFreight);
			return PartialView("_LiquidationFreightResultsDetailViewPartial", _lfTmp.LiquidationFreightDetail.Where(w => w.isActive).ToList());
		}
		public ActionResult LiquidationFreightDetailViewDetailsPartial()
		{
			int id_LiquidationFreight = (Request.Params["id_LiquidationFreight"] != null && Request.Params["id_LiquidationFreight"] != "") ? int.Parse(Request.Params["id_LiquidationFreight"]) : -1;
			LiquidationFreight LiquidationFreight = db.LiquidationFreight.FirstOrDefault(r => r.id == id_LiquidationFreight);
			return PartialView("_LiquidationFreightDetailViewDetailsPartial", LiquidationFreight.LiquidationFreightDetail.Where(w => w.isActive).ToList());
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

			LiquidationFreight LiquidationFreight = db.LiquidationFreight.FirstOrDefault(r => r.id == id);
			//int state = remissionGuide.Document.DocumentState.id;
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

		#region VALIDACION lIQUIDACION
		[HttpPost]
		public JsonResult ValidateSelectedRowsRemissionGuide(int[] ids)
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
				TempData.Keep("remissionGuide");
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			
			var setting = db.Setting.FirstOrDefault(r => r.code == codigoParametroGuiaDistintosProveedores);

            string parametroGuiasDistintosProveedores = (setting?.value??"N");


			if (parametroGuiasDistintosProveedores == "N")
			{
                var idsProvider = (from e in db.RemissionGuide
                                   where ids.Contains(e.id)
                                   select e.RemissionGuideTransportation.VehicleProviderTransportBilling.id_provider).ToArray();

                int distinctProveedor = (from e in db.RemissionGuide
                                         where ids.Contains(e.id)
                                         select e.RemissionGuideTransportation.VehicleProviderTransportBilling.id_provider).Distinct().Count();

                if (distinctProveedor > 1)
                {
                    result = new
                    {
                        Message = ErrorMessage("Las compañías que facturan no pueden ser diferentes")
                    };
                    TempData.Keep("remissionGuide");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }


            

			//int distinctProcess = (from e in db.RemissionGuide
			//					   where ids.Contains(e.id)
			//					   select e.id_personProcessPlant).Distinct().Count();

			//if (distinctProcess > 1)
			//{
			//	result = new
			//	{
			//		Message = ErrorMessage("Los procesos no pueden ser diferentes")
			//	};
			//	TempData.Keep("remissionGuide");
			//	return Json(result, JsonRequestBehavior.AllowGet);
			//}

			TempData.Keep("remissionGuide");
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_LiquidationFreight)
		{
			TempData.Keep("LiquidationFreight");
			int index = db.LiquidationFreight.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_LiquidationFreight);
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
			LiquidationFreight LiquidationFreight = db.LiquidationFreight.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (LiquidationFreight != null)
			{
				TempData["LiquidationFreight"] = LiquidationFreight;
				TempData.Keep("LiquidationFreight");
				return PartialView("_LiquidationFreightMainFormPartial", LiquidationFreight);
			}

			TempData.Keep("LiquidationFreight");

			return PartialView("_LiquidationFreightMainFormPartial", new LiquidationFreight());
		}
		#endregion

		[HttpPost, ValidateInput(false)]
		public JsonResult ItemDetailData(string pricedays, string priceextension, string priceadjustment, string priceCanc)
		{
			decimal _pricedays = Convert.ToDecimal(pricedays);
			decimal _priceextension = Convert.ToDecimal(priceextension);
			decimal _priceadjustment = Convert.ToDecimal(priceadjustment);
			decimal _priceCanc = Convert.ToDecimal(priceCanc);

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
			liquidationFreight.PriceCancelledTotal = _priceCanc;
			liquidationFreight.pricetotal = (liquidationFreight.pricesubtotal + _pricedays + _priceextension + _priceadjustment) - liquidationFreight.priceavance - _priceCanc;

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
					price = liquidationFreight.priceavance,
					priceCancel = liquidationFreight.PriceCancelledTotal
				},
			};

			TempData["LiquidationFreight"] = liquidationFreight;
			TempData.Keep("LiquidationFreight");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		#region SAVE AND UPDATE

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationFreightPartialAddNew(bool approve, LiquidationFreight item, Document document)
		{
			LiquidationFreight conLiquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);

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

					#region LiquidationFreight

					item.Document = document;
					item.Person = null;

					#endregion

					LiquidationFreight tempLiquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);
					tempLiquidationFreight = tempLiquidationFreight ?? new LiquidationFreight();

					#region LiquidationFreight DETAILS
					if (tempLiquidationFreight?.LiquidationFreightDetail != null)
					{
						item.LiquidationFreightDetail = new List<LiquidationFreightDetail>();

						var details = tempLiquidationFreight.LiquidationFreightDetail.ToList();

						foreach (var detail in details)
						{
							id_Vehicle = db.RemissionGuideTransportation.FirstOrDefault(fod => fod.id_remionGuide == detail.id_remisionGuide)?.id_vehicle ?? 0;

							var liquidationFreightDetail = new LiquidationFreightDetail
							{
								id_remisionGuide = detail.id_remisionGuide,
								RemissionGuide = db.RemissionGuide.FirstOrDefault(i => i.id == detail.id_remisionGuide),
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
								descriptionRG = detail.descriptionRG,
								idOwnerVehicle = db.Vehicle.FirstOrDefault(fod => fod.id == id_Vehicle)?.id_personOwner
							};

							item.LiquidationFreightDetail.Add(liquidationFreightDetail);
						}
					}

					if (item.LiquidationFreightDetail.Count == 0)
					{
						TempData.Keep("LiquidationFreight");
						ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Liquidacion de Flete sin detalles");
						return PartialView("_LiquidationFreightMainFormPartial", tempLiquidationFreight);
					}
					#endregion

					#region Documents Attached

					if (conLiquidationFreight?.LiquidationFreightDocument != null)
					{
						item.LiquidationFreightDocument = new List<LiquidationFreightDocument>();
						var liquidationDocAttach = conLiquidationFreight.LiquidationFreightDocument.ToList();

						foreach (var detail in liquidationDocAttach)
						{
							var tempLiquidationDocAttach = new LiquidationFreightDocument
							{
								guid = detail.guid,
								url = detail.url,
								attachment = detail.attachment,
								referenceDocument = detail.referenceDocument,
								descriptionDocument = detail.descriptionDocument
							};

							item.LiquidationFreightDocument.Add(tempLiquidationDocAttach);
						}
					}

					#endregion

					if (approve)
					{
						item.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
					}

					db.LiquidationFreight.Add(item);
					db.SaveChanges();
					trans.Commit();

					item.Person = db.Person.Where(x => x.id == item.id_providertransport).FirstOrDefault();
					TempData["LiquidationFreight"] = item;
					TempData.Keep("LiquidationFreight");
					ViewData["EditMessage"] = SuccessMessage("Liquidación de Flete: " + item.Document.number + " guardada exitosamente");
				}
				catch (Exception e)
				{
					TempData.Keep("LiquidationFreight");
					item = (TempData["LiquidationFreight"] as LiquidationFreight);
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}



			return PartialView("_LiquidationFreightMainFormPartial", item);


		}
		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationFreightPartialUpdate(bool approve, LiquidationFreight item
			, Document document)
		{
			LiquidationFreight modelItem = db.LiquidationFreight.FirstOrDefault(r => r.id == item.id);
			int id_Vehicle = 0;
			if (modelItem != null)
			{

				LiquidationFreight conLiquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);

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
						modelItem.PriceCancelledTotal = item.PriceCancelledTotal;
						modelItem.InvoiceNumber = item.InvoiceNumber;
						modelItem.priceextension = item.priceextension;
						modelItem.id_providertransport = item.id_providertransport;
						modelItem.Person = db.Person.FirstOrDefault(r => r.id == item.id_providertransport);

						#endregion

						LiquidationFreight tempLiquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);
						tempLiquidationFreight = tempLiquidationFreight ?? new LiquidationFreight();

						#region LiquidationFreigth DETAILS

						int count = 0;

						if (tempLiquidationFreight?.LiquidationFreightDetail != null)
						{
							var details = tempLiquidationFreight.LiquidationFreightDetail.ToList();

							//List<LiquidationFreightDetail> lfd = modelItem.LiquidationFreightDetail.ToList();

							foreach (var detail in details)
							{
								LiquidationFreightDetail liquidationFreightDetail = modelItem.LiquidationFreightDetail.FirstOrDefault(d => d.id == detail.id);
								id_Vehicle = db.RemissionGuideTransportation.FirstOrDefault(fod => fod.id_remionGuide == detail.id_remisionGuide)?.id_vehicle ?? 0;

								if (liquidationFreightDetail == null)
								{
									liquidationFreightDetail = new LiquidationFreightDetail
									{
										id_LiquidationFreight = detail.id_LiquidationFreight,
										id_remisionGuide = detail.id_remisionGuide,
										//RemissionGuide = db.RemissionGuide.FirstOrDefault(i => i.id == detail.id_remisionGuide),
										//LiquidationFreight = detail.LiquidationFreight,

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
										pricesavance = detail.pricesavance,
										PriceCancelled = detail.PriceCancelled,
										descriptionRG = detail.descriptionRG,
										idOwnerVehicle = db.Vehicle.FirstOrDefault(fod => fod.id == id_Vehicle)?.id_personOwner
									};

									modelItem.LiquidationFreightDetail.Add(liquidationFreightDetail);

									db.LiquidationFreightDetail.Add(liquidationFreightDetail);
									db.Entry(liquidationFreightDetail).State = EntityState.Added;
									count++;
								}
								else
								{
									liquidationFreightDetail.isActive = detail.isActive;
									liquidationFreightDetail.id_userCreate = detail.id_userCreate;
									liquidationFreightDetail.dateCreate = detail.dateCreate;
									liquidationFreightDetail.id_userUpdate = detail.id_userUpdate;
									liquidationFreightDetail.dateUpdate = detail.dateUpdate;
									liquidationFreightDetail.price = detail.price;
									liquidationFreightDetail.pricedays = detail.pricedays;
									liquidationFreightDetail.pricesavance = detail.pricesavance;
									liquidationFreightDetail.priceadjustment = detail.priceadjustment;
									liquidationFreightDetail.PriceCancelled = detail.PriceCancelled;
									liquidationFreightDetail.priceextension = detail.priceextension;
									liquidationFreightDetail.pricesubtotal = detail.pricesubtotal;
									liquidationFreightDetail.pricetotal = detail.pricetotal;
									liquidationFreightDetail.descriptionRG = detail.descriptionRG;
									count++;
								}
							}
						}

						if (count == 0)
						{
							TempData.Keep("LiquidationFreight");
							ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Liquidacion de Flete sin detalles");
							return PartialView("_LiquidationFreightMainFormPartial", tempLiquidationFreight);
						}

						#endregion

						#region DELETE LIQUIDATIONFREIGHT DETAILS

						List<int> lstNew = tempLiquidationFreight.LiquidationFreightDetail.Select(s => s.id_remisionGuide).ToList();
						List<int> lstToDelete = modelItem.LiquidationFreightDetail.Select(s => s.id_remisionGuide).ToList();

						List<int> lstToInactivate = lstToDelete.Where(w => !lstNew.Contains(w)).ToList();

						foreach (int i in lstToInactivate)
						{
							LiquidationFreightDetail _lfdTmp = modelItem.LiquidationFreightDetail.FirstOrDefault(fod => fod.id_remisionGuide == i);
							db.LiquidationFreightDetail.Remove(_lfdTmp);
							db.Entry(_lfdTmp).State = EntityState.Deleted;
						}

						#endregion

						#region FILL DATA
						List<LiquidationFreightDetail> lfd = modelItem.LiquidationFreightDetail.Where(w => w.isActive).ToList();
						foreach (var detail in lfd)
						{
							LiquidationFreightDetail _lfdTmp = lfd.FirstOrDefault(fod => fod.id_remisionGuide == detail.id_remisionGuide);
							_lfdTmp.RemissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == detail.id_remisionGuide);
						}
						#endregion

						#region UPDATE LIQUIDATION DOCUMENTS
						List<LiquidationFreightDocument> lstDocAttac = conLiquidationFreight.LiquidationFreightDocument.ToList();
						if (lstDocAttac != null)
						{
							var lstDocAttach = lstDocAttac.ToList();

							for (int i = modelItem.LiquidationFreightDocument.Count - 1; i >= 0; i--)
							{
								var detail = modelItem.LiquidationFreightDocument.ElementAt(i);

								if (lstDocAttach.FirstOrDefault(fod => fod.id == detail.id) == null)
								{
									LiquidationDocumentsDeleteAttachment(detail);
									modelItem.LiquidationFreightDocument.Remove(detail);
									db.Entry(detail).State = EntityState.Deleted;
								}

							}

							foreach (var detail in lstDocAttach)
							{
								var tempLiqDocument = modelItem
															.LiquidationFreightDocument
															.FirstOrDefault(fod => fod.id == detail.id);
								if (tempLiqDocument == null)
								{
									tempLiqDocument = new LiquidationFreightDocument
									{
										guid = detail.guid,
										url = detail.url,
										attachment = detail.attachment,
										referenceDocument = detail.referenceDocument,
										descriptionDocument = detail.descriptionDocument
									};
									modelItem.LiquidationFreightDocument.Add(tempLiqDocument);
								}
								else
								{
									if (tempLiqDocument.url != detail.url)
									{
										LiquidationDocumentsDeleteAttachment(tempLiqDocument);
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

						db.LiquidationFreight.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;
						db.SaveChanges();
						trans.Commit();

						TempData["LiquidationFreight"] = modelItem;
						TempData.Keep("LiquidationFreight");
						ViewData["EditMessage"] = SuccessMessage("Liquidación de Flete: " + modelItem.Document.number + " guardada exitosamente");
					}


					catch (Exception e)
					{
						TempData.Keep("LiquidationFreight");
						ViewData["EditMessage"] = ErrorMessage(e.Message);
						trans.Rollback();
					}
				}
			}
			else
			{
				ViewData["EditMessage"] = ErrorMessage();
			}

			TempData.Keep("LiquidationFreight");

			return PartialView("_LiquidationFreightMainFormPartial", modelItem);
		}

		public ActionResult UpdateDetailDocumentLiquidation(LiquidationFreight item
			, Document document)
		{
			LiquidationFreight modelItem = db.LiquidationFreight.FirstOrDefault(r => r.id == item.id);

			if (modelItem != null)
			{

				LiquidationFreight conLiquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);

				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						#region FILL DATA
						List<LiquidationFreightDetail> lfd = modelItem.LiquidationFreightDetail.Where(w => w.isActive).ToList();
						foreach (var detail in lfd)
						{
							LiquidationFreightDetail _lfdTmp = lfd.FirstOrDefault(fod => fod.id_remisionGuide == detail.id_remisionGuide);
							_lfdTmp.RemissionGuide = db.RemissionGuide.FirstOrDefault(fod => fod.id == detail.id_remisionGuide);
						}
						#endregion

						#region UPDATE LIQUIDATION DOCUMENTS
						List<LiquidationFreightDocument> lstDocAttac = conLiquidationFreight.LiquidationFreightDocument.ToList();
						if (lstDocAttac != null)
						{
							var lstDocAttach = lstDocAttac.ToList();

							for (int i = modelItem.LiquidationFreightDocument.Count - 1; i >= 0; i--)
							{
								var detail = modelItem.LiquidationFreightDocument.ElementAt(i);

								if (lstDocAttach.FirstOrDefault(fod => fod.id == detail.id) == null)
								{
									LiquidationDocumentsDeleteAttachment(detail);
									modelItem.LiquidationFreightDocument.Remove(detail);
									db.Entry(detail).State = EntityState.Deleted;
								}

							}

							foreach (var detail in lstDocAttach)
							{
								var tempLiqDocument = modelItem
															.LiquidationFreightDocument
															.FirstOrDefault(fod => fod.id == detail.id);
								if (tempLiqDocument == null)
								{
									tempLiqDocument = new LiquidationFreightDocument
									{
										guid = detail.guid,
										url = detail.url,
										attachment = detail.attachment,
										referenceDocument = detail.referenceDocument,
										descriptionDocument = detail.descriptionDocument
									};
									modelItem.LiquidationFreightDocument.Add(tempLiqDocument);
								}
								else
								{
									if (tempLiqDocument.url != detail.url)
									{
										LiquidationDocumentsDeleteAttachment(tempLiqDocument);
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

						db.LiquidationFreight.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;
						db.SaveChanges();
						trans.Commit();

						TempData["LiquidationFreight"] = modelItem;
						TempData.Keep("LiquidationFreight");
						ViewData["EditMessage"] = SuccessMessage("Se actualizaron los detalles de los Documentos Liquidación de Flete: " + modelItem.Document.number + " guardada exitosamente");
					}
					catch (Exception ex)
					{
						TempData.Keep("LiquidationFreight");
						ViewData["EditMessage"] = ErrorMessage(ex.Message);
						trans.Rollback();
					}
				}
			}
			return PartialView("_LiquidationFreightMainFormPartial", modelItem);
		}
		#endregion

		#region CHANGE DOCUMENT STATE

		[HttpPost]
		public ActionResult Approve(int id)
		{
			LiquidationFreight liquidationFreight = db.LiquidationFreight.FirstOrDefault(r => r.id == id);



			if (liquidationFreight.pricetotal <= 0)
			{
				TempData.Keep("LiquidationFreight");
				ViewData["EditMessage"] = ErrorMessage("No se puede Aprobar una Liquidacion de Flete con  Totales menores o igual a 0");
				return PartialView("_LiquidationFreightMainFormPartial", liquidationFreight);
			}

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

					if (liquidationFreight != null && documentState != null)
					{
						liquidationFreight.Document.id_documentState = documentState.id;
						liquidationFreight.Document.DocumentState = documentState;



						db.LiquidationFreight.Attach(liquidationFreight);
						db.Entry(liquidationFreight).State = EntityState.Modified;

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

			TempData["LiquidationFreight"] = liquidationFreight;
			TempData.Keep("LiquidationFreight");

			return PartialView("_LiquidationFreightMainFormPartial", liquidationFreight);
		}

		[HttpPost]
		public ActionResult Autorize(int id)
		{
			LiquidationFreight liquidationFreight = db.LiquidationFreight.FirstOrDefault(r => r.id == id);

			if (liquidationFreight.pricetotal <= 0)
			{
				TempData.Keep("LiquidationFreight");
				ViewData["EditMessage"] = ErrorMessage("No se puede Autorizar una Liquidacion de Flete con  Totales menores o igual a 0");
				return PartialView("_LiquidationFreightMainFormPartial", liquidationFreight);
			}

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					//DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada

					if (liquidationFreight != null && documentState != null)
					{
						liquidationFreight.Document.id_documentState = documentState.id;
						liquidationFreight.Document.DocumentState = documentState;




						db.LiquidationFreight.Attach(liquidationFreight);
						db.Entry(liquidationFreight).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						TempData["LiquidationFreight"] = liquidationFreight;
						TempData.Keep("LiquidationFreight");

						ViewData["EditMessage"] = SuccessMessage("La Liquidación de Flete: " + liquidationFreight.Document.number + " autorizada exitosamente");

					}
				}
				catch (Exception e)
				{
					TempData.Keep("LiquidationFreight");
					ViewData["EditError"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}


			return PartialView("_LiquidationFreightMainFormPartial", liquidationFreight);
		}

		[HttpPost]
		public ActionResult Protect(int id)
		{
			LiquidationFreight liquidationFreight = db.LiquidationFreight.FirstOrDefault(r => r.id == id);

			if (liquidationFreight.pricetotal <= 0)
			{
				TempData.Keep("LiquidationFreight");
				ViewData["EditMessage"] = ErrorMessage("No se puede Proteger una Liquidacion de Flete con  Totales menores o igual a 0");
				return PartialView("_LiquidationFreightMainFormPartial", liquidationFreight);
			}


			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

					if (liquidationFreight != null && documentState != null)
					{
						liquidationFreight.Document.id_documentState = documentState.id;
						liquidationFreight.Document.DocumentState = documentState;

						db.LiquidationFreight.Attach(liquidationFreight);
						db.Entry(liquidationFreight).State = EntityState.Modified;

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

			TempData["LiquidationFreight"] = liquidationFreight;
			TempData.Keep("LiquidationFreight");

			return PartialView("_LiquidationFreightMainFormPartial", liquidationFreight);
		}

		[HttpPost]
		public ActionResult Cancel(int id)
		{
			LiquidationFreight liquidationFreight = db.LiquidationFreight.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{


					//DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

					if (liquidationFreight != null && documentState != null)
					{


						liquidationFreight.Document.id_documentState = documentState.id;
						liquidationFreight.Document.DocumentState = documentState;

						db.LiquidationFreight.Attach(liquidationFreight);
						db.Entry(liquidationFreight).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						TempData["LiquidationFreight"] = liquidationFreight;
						TempData.Keep("LiquidationFreight");

						ViewData["EditMessage"] = SuccessMessage("La Liquidación de Flete: " + liquidationFreight.Document.number + " anulada exitosamente");

					}
				}
				catch (Exception e)
				{
					TempData.Keep("LiquidationFreight");
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}



			return PartialView("_LiquidationFreightMainFormPartial", liquidationFreight);
		}

		[HttpPost]
		public ActionResult Revert(int id)
		{
			LiquidationFreight liquidationFreight = db.LiquidationFreight.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{




					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

					//DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

					if (liquidationFreight != null && documentState != null)
					{




						liquidationFreight.Document.id_documentState = documentState.id;
						liquidationFreight.Document.DocumentState = documentState;



						db.LiquidationFreight.Attach(liquidationFreight);
						db.Entry(liquidationFreight).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						TempData["LiquidationFreight"] = liquidationFreight;
						TempData.Keep("LiquidationFreight");

						ViewData["EditMessage"] = SuccessMessage("La Liquidación de Flete: " + liquidationFreight.Document.number + " reversada exitosamente");

					}
				}
				catch (Exception e)
				{
					TempData.Keep("LiquidationFreight");
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
				}
			}



			return PartialView("_LiquidationFreightMainFormPartial", liquidationFreight);
		}

		#endregion

		#region  BATCH EDIT 
		[ValidateInput(false)]
		public ActionResult LiquidationFreightResultsDetailBatchEditingUpdateModel(MVCxGridViewBatchUpdateValues<LiquidationFreightDetail, int> updateValues)
		{

			LiquidationFreight tempLiquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);
			tempLiquidationFreight = tempLiquidationFreight ?? new LiquidationFreight();

			LiquidationFreight LiquidationFreightAUX = tempLiquidationFreight;

			bool bcontinue = true;
			//decimal Total = 0, Subtotal = 0, extencion = 0;
			try
			{
				foreach (var idToDelete in updateValues.DeleteKeys)
				{
					LiquidationFreightDetail liquidationFreightDetail = LiquidationFreightAUX.LiquidationFreightDetail.Where(x => x.id_remisionGuide == idToDelete).FirstOrDefault();
					LiquidationFreightAUX.LiquidationFreightDetail.Remove(liquidationFreightDetail);
				}
				foreach (var detail in updateValues.Update)
				{
					if (updateValues.IsValid(detail))
					{
						LiquidationFreightDetail liquidationFreightDetail = LiquidationFreightAUX.LiquidationFreightDetail.Where(x => x.id_remisionGuide == detail.id_remisionGuide).FirstOrDefault();
						liquidationFreightDetail.priceadjustment = detail.priceadjustment;
						liquidationFreightDetail.pricedays = detail.pricedays;
						liquidationFreightDetail.priceextension = detail.priceextension;
						liquidationFreightDetail.pricesubtotal = liquidationFreightDetail.price + liquidationFreightDetail.priceadjustment + liquidationFreightDetail.pricedays + liquidationFreightDetail.priceextension - (liquidationFreightDetail.PriceCancelled ?? 0);
						liquidationFreightDetail.pricetotal = liquidationFreightDetail.pricesubtotal - liquidationFreightDetail.pricesavance;
						liquidationFreightDetail.descriptionRG = detail.descriptionRG;
					}
					else
					{
						bcontinue = false;
						updateValues.SetErrorText(detail, "Error");
					}
				}


				if (bcontinue)
				{

					var price = (from e in LiquidationFreightAUX.LiquidationFreightDetail
								 select e.price).Sum();

					var priceadjustment = (from e in LiquidationFreightAUX.LiquidationFreightDetail
										   select e.priceadjustment).Sum();

					var pricedays = (from e in LiquidationFreightAUX.LiquidationFreightDetail
									 select e.pricedays).Sum();

					var priceextension = (from e in LiquidationFreightAUX.LiquidationFreightDetail
										  select e.priceextension).Sum();

					var pricesubTotal = (from e in LiquidationFreightAUX.LiquidationFreightDetail
										 select e.pricesubtotal).Sum();


					var pricetotal = (from e in LiquidationFreightAUX.LiquidationFreightDetail
									  select e.pricetotal).Sum();

					var priceCancelled = (from e in LiquidationFreightAUX.LiquidationFreightDetail
										  select e.PriceCancelled).Sum();

					LiquidationFreightAUX.priceadjustment = priceadjustment;
					LiquidationFreightAUX.pricedays = pricedays;
					LiquidationFreightAUX.priceextension = priceextension;
					LiquidationFreightAUX.pricesubtotal = pricesubTotal;
					LiquidationFreightAUX.pricetotal = pricetotal;
					LiquidationFreightAUX.price = price;

					tempLiquidationFreight = LiquidationFreightAUX;
					TempData["LiquidationFreight"] = tempLiquidationFreight;
				}

				TempData.Keep("LiquidationFreight");

			}
			catch (Exception e)
			{
				updateValues.SetErrorText(0, e.Message);
			}
			return PartialView("_LiquidationFreightResultsDetailViewPartial", tempLiquidationFreight.LiquidationFreightDetail.ToList());
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
			var result = new
			{
				ItemData = new
				{
					pricedays = liquidationFreight.pricedays,
					priceextension = liquidationFreight.priceextension,
					priceadjustment = liquidationFreight.priceadjustment,
					pricesubtotal = liquidationFreight.pricesubtotal,
					pricetotal = liquidationFreight.pricetotal,
					priceCancelled = liquidationFreight.PriceCancelledTotal,
					priceavance = liquidationFreight.priceavance,
					price = liquidationFreight.price
				},
			};

			TempData["LiquidationFreight"] = liquidationFreight;
			TempData.Keep("LiquidationFreight");

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region REPORT
		[HttpPost, ValidateInput(false)]
		public JsonResult PrintLiquidationFreightReport(int id_lf)
		{
			LiquidationFreight liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);
			liquidationFreight = liquidationFreight ?? new LiquidationFreight();
			TempData["LiquidationFreight"] = liquidationFreight;
			TempData.Keep("LiquidationFreight");

			#region "Armo Parametros"
			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param = new ParamCR();
			_param.Nombre = "@id";
			_param.Valor = id_lf;

			paramLst.Add(_param);

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = "LRFT1";
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
		public JsonResult PRLiquidationRotation(string codeReport, LiquidationFreight liquidationFreight,
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

		#region POPUP REMISSION GUIDE LIST

		[HttpPost]
		public JsonResult AddSelectedRemissionGuides(int[] ids)
		{
			LiquidationFreight _lfTmp = (TempData["LiquidationFreight"] as LiquidationFreight);
			_lfTmp = _lfTmp ?? new LiquidationFreight();

			List<int> _lstTmp = ids.Select(s => s).Distinct().ToList();

			List<RemissionGuide> _lstRg = TempData["remissionGuidePopUpPartial"] as List<RemissionGuide>;

			foreach (int i in _lstTmp)
			{
				RemissionGuide _rgTmp = _lstRg.Where(x => x.id == i).FirstOrDefault();

				var modelItem = _lfTmp.LiquidationFreightDetail.FirstOrDefault(it => (!it.isActive) &&
																						   it.id_remisionGuide == i);
				if (modelItem == null)
				{
					LiquidationFreightDetail liquidationFreightDetail = new LiquidationFreightDetail()
					{
						id = _lfTmp.LiquidationFreightDetail.Count() > 0 ? _lfTmp.LiquidationFreightDetail.Max(pld => pld.id) + 1 : 1,
						id_remisionGuide = _rgTmp.id,
						isActive = true,
						id_userCreate = ActiveUser.id,
						dateCreate = DateTime.Now,
						id_userUpdate = ActiveUser.id,
						dateUpdate = DateTime.Now,
						id_LiquidationFreight = _lfTmp.id,
						RemissionGuide = _rgTmp,
						LiquidationFreight = _lfTmp,
						price = _rgTmp?.RemissionGuideTransportation?.valuePrice ?? 0,
						pricesavance = _rgTmp?.RemissionGuideTransportation?.advancePrice ?? 0,
						priceadjustment = 0,
						pricedays = 0,
						priceextension = 0,
						PriceCancelled = 0,
						pricesubtotal = _rgTmp?.RemissionGuideTransportation?.valuePrice ?? 0,
						pricetotal = (_rgTmp?.RemissionGuideTransportation?.valuePrice ?? 0) - (_rgTmp?.RemissionGuideTransportation?.advancePrice ?? 0)
					};

					RemGuideLiqTransportation rmp = db.RemGuideLiqTransportation
														.FirstOrDefault(fod => fod.id_remisionGuide == i);

					if (rmp != null)
					{
						liquidationFreightDetail.price = rmp.price;
						liquidationFreightDetail.pricesavance = rmp.priceadvance;
						liquidationFreightDetail.priceadjustment = rmp.priceadjustment;
						liquidationFreightDetail.pricedays = rmp.pricedays;
						liquidationFreightDetail.priceextension = rmp.priceextension;
						liquidationFreightDetail.pricetotal = rmp.pricetotal;
						liquidationFreightDetail.pricesubtotal = rmp.pricesubtotal;
					}

					if (_rgTmp?.Document?.DocumentState?.code == "08")
					{
						liquidationFreightDetail.PriceCancelled = _rgTmp?.RemissionGuideTransportation?.valuePrice ?? 0;
						liquidationFreightDetail.pricesubtotal = liquidationFreightDetail.pricesubtotal - (liquidationFreightDetail.PriceCancelled ?? 0);
						liquidationFreightDetail.pricetotal = liquidationFreightDetail.pricetotal - (_rgTmp?.RemissionGuideTransportation?.valuePrice ?? 0);
					}

					_lfTmp.LiquidationFreightDetail.Add(liquidationFreightDetail);
				}
			}

			#region TOTALS
			var model = _lfTmp?.LiquidationFreightDetail.Where(d => d.isActive).ToList() ?? new List<LiquidationFreightDetail>();

			_lfTmp.price = model.Where(w => w.isActive).Select(s => s.price).DefaultIfEmpty(0).Sum();
			_lfTmp.priceadjustment = model.Where(w => w.isActive).Select(s => s.priceadjustment).DefaultIfEmpty(0).Sum();
			_lfTmp.pricedays = model.Where(w => w.isActive).Select(s => s.pricedays).DefaultIfEmpty(0).Sum();
			_lfTmp.priceextension = model.Where(w => w.isActive).Select(s => s.priceextension).DefaultIfEmpty(0).Sum();
			_lfTmp.pricesubtotal = model.Where(w => w.isActive).Select(s => s.pricesubtotal).DefaultIfEmpty(0).Sum();
			_lfTmp.pricetotal = model.Where(w => w.isActive).Select(s => s.pricetotal).DefaultIfEmpty(0).Sum();
			_lfTmp.PriceCancelledTotal = model.Where(w => w.isActive).Select(s => s.PriceCancelled).DefaultIfEmpty(0).Sum();

			#endregion

			TempData["LiquidationFreight"] = _lfTmp;
			TempData.Keep("LiquidationFreight");


			return null;
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult GetRemissionGuidesPopUpPartial(int id_pt)
		{
			LiquidationFreight _lfTmp = (TempData["LiquidationFreight"] as LiquidationFreight);
			_lfTmp = _lfTmp ?? new LiquidationFreight();

			List<LiquidationFreightDetail> _lfdTmp = _lfTmp.LiquidationFreightDetail.ToList();
			int[] _iLfdTmp = _lfdTmp.Select(s => s.id_remisionGuide).ToArray();

			int[]  _lrgTmp = db.LiquidationFreightDetail
												.Where(w => w.LiquidationFreight.Document.DocumentState.code != "05")
												.Select(s => s.id_remisionGuide).Distinct()
												.ToArray();

			int[] idsExllude = _lrgTmp.Concat(_iLfdTmp).ToArray();

			var state06Id = (db.DocumentState.FirstOrDefault(r => r.code == "06")?.id??0);
			var state08Id = (db.DocumentState.FirstOrDefault(r => r.code == "08")?.id ?? 0);
			var state09Id = (db.DocumentState.FirstOrDefault(r => r.code == "09")?.id ?? 0);

			int[] stateIds = new int[] { state06Id, state08Id, state09Id };


			int[] ids_Preselect = db.RemissionGuideTransportation
													.Where(w =>  ( w.id_VehicleProviderTranportistBilling == id_pt || w.id_provider == id_pt)
																  && stateIds.Contains(w.RemissionGuide.Document.id_documentState) 
																  && !w.isOwn
																  && w.valuePrice > 0)
													.Select(s => s.id_remionGuide)
													.ToArray();

			int[] ids_Select = ids_Preselect.Where(r => !idsExllude.Contains(r)).ToArray();

			List<RemissionGuide> _rgList = db.RemissionGuide
												.Where(w => ids_Select.Contains(w.id))
												.ToList();

			TempData["remissionGuidePopUpPartial"] = _rgList;
			TempData.Keep("remissionGuidePopUpPartial");
			TempData.Keep("LiquidationFreight");

			var result = new
			{
				register = _rgList.Count(),
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult RemissionGuideForLiquidationPopupPartial()
		{
			List<RemissionGuide> _rgList = (TempData["remissionGuidePopUpPartial"] as List<RemissionGuide>);
			_rgList = _rgList ?? new List<RemissionGuide>();

			TempData["remissionGuidePopUpPartial"] = _rgList;
			TempData.Keep("remissionGuidePopUpPartial");

			if (TempData["LiquidationFreight"] != null)
			{
				LiquidationFreight _lfTmp = (TempData["LiquidationFreight"] as LiquidationFreight);
				_lfTmp = _lfTmp ?? new LiquidationFreight();
				TempData["LiquidationFreight"] = _lfTmp;
				TempData.Keep("LiquidationFreight");
			}

			return PartialView("_RemissionGuidesPartial", _rgList);

		}

		#endregion

		#region LIQUIDATION UPLOAD FILE
		[ValidateInput(false)]
		public ActionResult LiquidationDocumentsAttachedDocumentsPartial()
		{
			LiquidationFreight _liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);
			_liquidationFreight = _liquidationFreight ?? new LiquidationFreight();

			var model = _liquidationFreight.LiquidationFreightDocument;
			TempData.Keep("LiquidationFreight");

			return PartialView("_LiquidationDocumentsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationDocumentsAttachedDocumentsPartialAddNew(DXPANACEASOFT.Models.LiquidationFreightDocument liquidationFreightDocument)
		{
			LiquidationFreight _liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);
			_liquidationFreight = _liquidationFreight ?? new LiquidationFreight();

			if (ModelState.IsValid)
			{
				try
				{
					if (string.IsNullOrEmpty(liquidationFreightDocument.attachment))
					{
						throw new Exception("El Documento adjunto no puede ser vacio");
					}
					else
					{
						if (string.IsNullOrEmpty(liquidationFreightDocument.guid) || string.IsNullOrEmpty(liquidationFreightDocument.url))
						{
							throw new Exception("El fichero no se cargo completo, intente de nuevo");
						}
						else
						{
							var itemTechnicalSpecificationsDocumentDetailAux = _liquidationFreight
								.LiquidationFreightDocument
								.FirstOrDefault(fod => fod.attachment == liquidationFreightDocument.attachment);
							if (itemTechnicalSpecificationsDocumentDetailAux != null)
							{
								throw new Exception("No se puede repetir el Documento Adjunto: " + liquidationFreightDocument.attachment + ", en el detalle de los Documentos Adjunto.");
							}

						}
					}
					liquidationFreightDocument.id = _liquidationFreight.LiquidationFreightDocument.Count() > 0 ? _liquidationFreight.LiquidationFreightDocument.Max(pld => pld.id) + 1 : 1;
					_liquidationFreight.LiquidationFreightDocument.Add(liquidationFreightDocument);
					TempData["LiquidationFreight"] = _liquidationFreight;
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";

			TempData.Keep("LiquidationFreight");
			var model = _liquidationFreight.LiquidationFreightDocument;

			return PartialView("_LiquidationDocumentsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());

		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationDocumentsAttachedDocumentsPartialUpdate(DXPANACEASOFT.Models.LiquidationFreightDocument liquidationFreightDocument)
		{
			LiquidationFreight _liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);
			_liquidationFreight = _liquidationFreight ?? new LiquidationFreight();

			if (ModelState.IsValid)
			{
				try
				{
					var modelItem = _liquidationFreight.LiquidationFreightDocument.FirstOrDefault(i => i.id == _liquidationFreight.id);
					if (string.IsNullOrEmpty(liquidationFreightDocument.attachment))
					{
						throw new Exception("El Documento adjunto no puede ser vacio");
					}
					else
					{
						if (string.IsNullOrEmpty(liquidationFreightDocument.guid) || string.IsNullOrEmpty(liquidationFreightDocument.url))
						{
							throw new Exception("El fichero no se cargo completo, intente de nuevo");
						}
						else
						{
							if (modelItem.attachment != liquidationFreightDocument.attachment)
							{
								var itemTechnicalSpecificationsDocumentDetailAux =
									_liquidationFreight
									.LiquidationFreightDocument
									.FirstOrDefault(fod => fod.attachment == liquidationFreightDocument.attachment);
								if (itemTechnicalSpecificationsDocumentDetailAux != null)
								{
									throw new Exception("No se puede repetir el Documento Adjunto: " + liquidationFreightDocument.attachment + ", en el detalle de los Documentos Adjunto.");
								}
							}
						}
					}
					if (modelItem != null)
					{
						modelItem.guid = liquidationFreightDocument.guid;
						modelItem.url = liquidationFreightDocument.url;
						modelItem.attachment = liquidationFreightDocument.attachment;
						modelItem.referenceDocument = liquidationFreightDocument.referenceDocument;
						modelItem.descriptionDocument = liquidationFreightDocument.descriptionDocument;

						this.UpdateModel(modelItem);
					}
					TempData["LiquidationFreight"] = _liquidationFreight;
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";

			TempData.Keep("LiquidationFreight");
			var model = _liquidationFreight.LiquidationFreightDocument;


			return PartialView("_LiquidationDocumentsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationDocumentsAttachedDocumentsPartialDelete(System.Int32 id)
		{
			LiquidationFreight _liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);
			_liquidationFreight = _liquidationFreight ?? new LiquidationFreight();

			try
			{
				var liquidationFreightDocument = _liquidationFreight.LiquidationFreightDocument.FirstOrDefault(it => it.id == id);
				if (liquidationFreightDocument != null)
					_liquidationFreight.LiquidationFreightDocument.Remove(liquidationFreightDocument);

				TempData["LiquidationFreight"] = _liquidationFreight;
			}
			catch (Exception e)
			{
				ViewData["EditError"] = e.Message;
			}

			TempData.Keep("item");
			var model = _liquidationFreight.LiquidationFreightDocument;

			return PartialView("_LiquidationDocumentsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
		}

		#endregion

		#region LIQUIDATION DOCUMENT ATTACHED DOCUMENTS
		private void LiquidationDocumentsUpdateAttachment(LiquidationFreight liqFreight)
		{
			List<LiquidationFreightDocument> liqDocument = liqFreight.LiquidationFreightDocument.ToList() ?? new List<LiquidationFreightDocument>();
			foreach (var itemTmp in liqDocument)
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
							ref guidAux, out nameAttachment, out typeContentAttachment, pathFiles + "\\LiquidacionFlete\\");

						// Guardamos en el directorio final el fichero que este aun en su ruta temporal
						itemTmp.guid = FileUploadHelper.FileUploadProcessAttachment(pathFiles + "\\LiquidacionFleteTmp\\" + liqFreight.id.ToString(), nameAttachment, typeContentAttachment, contentAttachment, out urlAux);
						itemTmp.url = urlAux;

					}
					catch (Exception exception)
					{
						throw new Exception("Error al guardar el adjunto. Error: " + exception.Message);
					}

				}
			}
		}

		private void LiquidationDocumentsDeleteAttachment(LiquidationFreightDocument liqDocument)
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
		public ActionResult LiquidationDocumentsDownloadAttachment(int id)
		{
			//TempData.Keep("item");

			try
			{
				LiquidationFreight _liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);
				TempData.Keep("LiquidationFreight");
				List<LiquidationFreightDocument> liqDocument = _liquidationFreight?.LiquidationFreightDocument?.ToList() ?? new List<LiquidationFreightDocument>();


				var liqDocumentAux = liqDocument.FirstOrDefault(fod => fod.id == id);
				if (liqDocumentAux != null)
				{
					// Carga el contenido guardado en el temp
					string nameAttachment;
					string typeContentAttachment;
					string guidAux = liqDocumentAux.guid;
					string urlAux = liqDocumentAux.url;
					var contentAttachment = FileUploadHelper.ReadFileUpload(
						ref guidAux, ref urlAux, out nameAttachment, out typeContentAttachment, pathFiles + "\\LiquidacionFlete\\");

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
			TempData.Keep("LiquidationFreight");
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
					e.CallbackData = FileUploadHelper.FileUploadProcess(e, pathFiles + "\\LiquidacionFlete\\");
				}
			}
		}

		public JsonResult ItsRepeatedAttachmentDetail(string attachmentNameNew)
		{
			LiquidationFreight _liquidationFreight = (TempData["LiquidationFreight"] as LiquidationFreight);

			_liquidationFreight = _liquidationFreight ?? new LiquidationFreight();
			var result = new
			{
				itsRepeated = 0,
				Error = ""
			};

			var liquidationDocumentDetailAux = _liquidationFreight.
													LiquidationFreightDocument.
													FirstOrDefault(fod => fod.attachment == attachmentNameNew);

			if (liquidationDocumentDetailAux != null)
			{
				result = new
				{
					itsRepeated = 1,
					Error = "No se puede repetir el Documento Adjunto: " + attachmentNameNew + ", en el detalle de los Documentos Adjunto."

				};
			}
			TempData.Keep("LiquidationFreight");
			return Json(result, JsonRequestBehavior.AllowGet);
		}
        #endregion

        #region << Performance Load >>
		private IList<RemissionGuide> QueryResult()
		{
			string[] documenStateDisabledCodes = new string[] { "01", "05" };
			var documentStatesIds = db.DocumentState
											.Where(r => !documenStateDisabledCodes.Contains(r.code))
											.Select(r => r.id)
											.ToArray();

			var documentTypeId = db.DocumentType
										.FirstOrDefault(r => r.code == "08")?.id;

			var documentGuiaRemisionList = (from documento in db.Document
											join state in documentStatesIds on
											documento.id_documentState equals state
											where documento.id_documentType == documentTypeId
											select documento.id).ToArray();

            //var guiaRemisionHasPlantProduction = (from guia in db.RemissionGuide
            //									  join documento in documentGuiaRemisionList on
            //									  guia.id equals documento
            //									  where guia.hasEntrancePlanctProduction == true
            //									  select guia.id
            //									 )
            //									 .ToArray();
            //

            /////////var guiaRemisionHasPlantProduction = db.RemissionGuide
			/////////											.Where(r => documentGuiaRemisionList.Contains(r.id)
			/////////													   && r.hasEntrancePlanctProduction == true)
			/////////											.Select(r => r.id)
			/////////											.ToArray();

            //var modelIds = db.RemissionGuide
			//					.Where(r => guiaRemisionHasPlantProduction.Contains(r.id)
			//								&& r.RemissionGuideTransportation.valuePrice > 0
			//								&& r.RemissionGuideTransportation.isOwn == false)
			//					.Select(r => r.id)
			//					.ToArray();

            var preModelIds = db.RemissionGuide
								   .Where(r => /*guiaRemisionHasPlantProduction.Contains(r.id)*/
											   r.RemissionGuideTransportation.valuePrice > 0
											   && r.RemissionGuideTransportation.isOwn == false
											   && r.hasEntrancePlanctProduction == true)
								   .Select(r => r.id)
								   .ToArray();

			var modelIds = (from pre in preModelIds
							join dc in documentGuiaRemisionList
							on pre equals dc
							select pre
							).ToArray();
				

			var _filter = db.LiquidationFreightDetail
								.Where(d => d.LiquidationFreight.Document.DocumentState.code != "05")
								.Select(r => r.id_remisionGuide)
								.ToArray();

			var filter = modelIds.Intersect(_filter).ToArray();

			if (filter != null)
			{
				modelIds = modelIds.Where(r => !filter.Contains(r)).ToArray();
			}
			 
			IList<RemissionGuide> model = new List<RemissionGuide>();
			if (modelIds != null && modelIds.Length > 0)
			{
				model = db.RemissionGuide
									.Where(r => modelIds.Contains(r.id))
									.OrderByDescending(r => r.id)
									.ToList();
			}

			return model;

		}
        #endregion

        #region Métodos de consultas adicionales
        [HttpPost]
        [ActionName("query-person-rol")]
        public PartialViewResult QueryPersonRol(int? idCompany, int? idPerson, string tipo)
        {
            return this.PartialView(
                "ComboBoxes/_PersonRolComboBoxPartial",
                GetPersonRolComboBoxModel(idCompany, idPerson, tipo));
        }

        public static PersonRolComboBoxModel GetPersonRolComboBoxModel(int? idCompany, int? idPerson, string tipo)
        {
            if (tipo == "Proveedor")
            {
                return new PersonRolComboBoxModel()
                {
                    Name = "fullname_businessName",
                    CallbackRouteValues = new { Controller = "liquidationFreight", Action = "query-person-rol" },
                    ClientSideEvents = new DevExpress.Web.ComboBoxClientSideEvents()
                    {
                        BeginCallback = "OnTransportProviderBeginCallback",
                        Validation = "OnTransportProviderValidation",
						ValueChanged = "OnTransportProviderValueChanged"
                    },
                    IdCompany = idCompany,
                    IdPerson = idPerson,
                    Rols = new[] {
                        DataProviderRol.m_RolProveedor,
                        
                    },
                    CustomProperties = new Dictionary<string, object>
                    {
                        { "IdCompany", idCompany },
                    },
                };
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
