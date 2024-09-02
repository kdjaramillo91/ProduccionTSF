using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Web;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.FE.Xmls.Common;
using System.Web.UI.WebControls;
using DXPANACEASOFT.DataProviders;
using DevExpress.Web.Mvc;
using DevExpress.Utils;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class SalesRequestController : DefaultController
	{
		[HttpPost]
		public ActionResult Index()
		{
			return View();
		}

		#region PURCHESE ORDER FILTERS RESULTS

		[HttpPost]
		public ActionResult SalesRequestsResults(SalesRequest salesRequest,
												 Document document,
												 DateTime? startEmissionDate, DateTime? endEmissionDate,
												 DateTime? startAuthorizationDate, DateTime? endAuthorizationDate,
												 int[] items)
		{
			List<SalesRequest> model = db.SalesRequest.AsEnumerable().ToList();

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

			if (!string.IsNullOrEmpty(document.accessKey))
			{
				model = model.Where(o => o.Document.accessKey.Contains(document.accessKey)).ToList();
			}

			if (!string.IsNullOrEmpty(document.authorizationNumber))
			{
				model = model.Where(o => o.Document.authorizationNumber.Contains(document.authorizationNumber)).ToList();
			}

			#endregion

			#region SALES REQUESTS FILTERS

			if (salesRequest.id_customer != null && salesRequest.id_customer != 0)
			{
				model = model.Where(o => o.id_customer == salesRequest.id_customer).ToList();
			}

			if (salesRequest.id_priceList != null && salesRequest.id_priceList != 0)
			{
				model = model.Where(o => o.id_priceList == salesRequest.id_priceList).ToList();
			}

			if (items != null && items.Length > 0)
			{
				var tempModel = new List<SalesRequest>();
				foreach (var request in model)
				{
					var details = request.SalesRequestDetail.Where(d => items.Contains(d.id_item));
					if (details.Any())
					{
						tempModel.Add(request);
					}
				}

				model = tempModel;
			}

			#endregion

			TempData["model"] = model;
			TempData.Keep("model");

			return PartialView("_SalesRequestsResultsPartial", model.OrderByDescending(o => o.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult SalesQuotationsDetailsResults()
		{
			var model = db.SalesQuotationDetail.Where(d =>
						(d.SalesQuotation.Document.DocumentState.code == "06")).ToList(); //"06" AUTORIZADA
			var tempSalesQuotationDetail = new List<SalesQuotationDetail>();
			foreach (var item in model)
			{
				var auxSalesRequestDetailSalesQuotation = (item.SalesRequestDetailSalesQuotation.Where(w => w.SalesRequestDetail.SalesRequest.Document.DocumentState.code == "03" ||//Aprobada
																											w.SalesRequestDetail.SalesRequest.Document.DocumentState.code != "06"));//Autorizada
				var quantityAux = auxSalesRequestDetailSalesQuotation != null && auxSalesRequestDetailSalesQuotation.Count() > 0 ? auxSalesRequestDetailSalesQuotation.Sum(s => s.quantity) : 0;
				if (item.quantityTypeUMPresentation > quantityAux)
				{
					tempSalesQuotationDetail.Add(item);
				}
			}
			model = tempSalesQuotationDetail;
			return PartialView("_SalesQuotationsDetailsResultsPartial", model.OrderByDescending(d => d.id_salesQuotation).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult SalesQuotationsDetailsPartial()
		{
			var model = db.SalesQuotationDetail.Where(d =>
						(d.SalesQuotation.Document.DocumentState.code == "06")).ToList(); //"06" AUTORIZADA
			var tempSalesQuotationDetail = new List<SalesQuotationDetail>();
			foreach (var item in model)
			{
				var auxSalesRequestDetailSalesQuotation = (item.SalesRequestDetailSalesQuotation.Where(w => w.SalesRequestDetail.SalesRequest.Document.DocumentState.code == "03" ||//Aprobada
																											w.SalesRequestDetail.SalesRequest.Document.DocumentState.code != "06"));//Autorizada
				var quantityAux = auxSalesRequestDetailSalesQuotation != null && auxSalesRequestDetailSalesQuotation.Count() > 0 ? auxSalesRequestDetailSalesQuotation.Sum(s => s.quantity) : 0;
				if (item.quantityTypeUMPresentation > quantityAux)
				{
					tempSalesQuotationDetail.Add(item);
				}
			}
			model = tempSalesQuotationDetail;
			return PartialView("_SalesQuotationsDetailsPartial", model.OrderByDescending(d => d.id_salesQuotation).ToList());
		}

		#endregion

		#region SALES REQUEST EDITFORM

		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditSalesRequest(int id, int[] quotationsDetails)//SalesQuotationDetail [] quotationsDetails)
		{

			SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(o => o.id == id);

			if (salesRequest == null)
			{
				DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("10"));//10: Requerimiento de Pedido

				DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");//01: Estado Pendiente
																								   //DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.id == 1);

				Employee employee = ActiveUser.Employee;
				Department department = employee?.Department;


				salesRequest = new SalesRequest
				{
					Document = new Document
					{
						id = 0,
						id_documentType = documentType?.id ?? 0,
						DocumentType = documentType,
						id_documentState = documentState?.id ?? 0,
						DocumentState = documentState,
						emissionDate = DateTime.Now

					},
					id_employeeSeller = employee?.id ?? 0,
					Employee = employee,
					Customer = null,
					id_customer = null,
					PriceList = null,
					id_priceList = null,
					SalesRequestTotal = new SalesRequestTotal()
				};

			}

			if (quotationsDetails != null)
			{
				foreach (var id_salesQuotationDetail in quotationsDetails)
				{
					SalesQuotationDetail
					quotationDetail =
						db.SalesQuotationDetail.FirstOrDefault(
							d =>
								d.id == id_salesQuotationDetail);/*&&*/
																 //d.id_item == salesQuotationDetail.id_item);

					//if (quotationDetail != null)
					//{
					//SalesQuotationDetail detail =
					//    db.SalesQuotationDetail.FirstOrDefault(
					//        d => d.id_salesQuotation == quotationDetail.id_salesQuotation && d.id_item == quotationDetail.id_item);
					if (quotationDetail != null)
					{
						if (salesRequest.id_customer == null)
						{
							salesRequest.id_customer = quotationDetail.SalesQuotation.id_customer;
							salesRequest.Customer = quotationDetail.SalesQuotation.Customer;
							salesRequest.id_priceList = quotationDetail.SalesQuotation.id_priceList;
							salesRequest.PriceList = quotationDetail.SalesQuotation.PriceList;
						}
						//SalesRequestDetail tempDetail = salesRequest.SalesRequestDetail.FirstOrDefault(d => d.id_item == detail.id_item);

						//if (tempDetail != null)
						//{
						//    tempDetail.quantityRequested += detail.quantity;

						//    tempDetail.iva = SalesRequestDetailIVA(detail.id_item, detail.quantity, tempDetail.price);
						//    tempDetail.subtotal = tempDetail.price * tempDetail.quantityRequested;
						//    tempDetail.total = tempDetail.price * tempDetail.quantityRequested + tempDetail.iva;

						//    SalesRequestDetailSalesQuotation newSalesRequestDetailSalesQuotation = new SalesRequestDetailSalesQuotation
						//    {
						//        id_salesQuotation = detail.id_salesQuotation,
						//        quantity = detail.quantity
						//    };
						//    tempDetail.SalesRequestDetailSalesQuotation.Add(newSalesRequestDetailSalesQuotation);
						//}
						//else
						//{
						decimal price = quotationDetail.price;


						//TruncateDecimal(1);

						var metricUnitPresentation = quotationDetail.Item.Presentation?.MetricUnit;

						var metricUnitAux = quotationDetail.MetricUnit;

						var factorConversionAux = GetFactorConversion(metricUnitPresentation, metricUnitAux, "Falta de Factor de Conversión entre : " + metricUnitPresentation.code + " y " + (metricUnitAux.code) + ".Necesario para la cantidad de Venta del detalle del Requerimiento de Pedido Configúrelo, e intente de nuevo", db);

						var auxSalesRequestDetailSalesQuotation = (quotationDetail.SalesRequestDetailSalesQuotation.Where(w => w.SalesRequestDetail.SalesRequest.Document.DocumentState.code == "03" ||//Aprobada
																											w.SalesRequestDetail.SalesRequest.Document.DocumentState.code != "06"));//Autorizada
						var quantityAux = auxSalesRequestDetailSalesQuotation != null && auxSalesRequestDetailSalesQuotation.Count() > 0 ? auxSalesRequestDetailSalesQuotation.Sum(s => s.quantity) : 0;

						//if (item.quantityTypeUMPresentation > quantityAux)
						//{
						//    tempSalesQuotationDetail.Add(item);
						//}
						quantityAux = (quotationDetail.quantityTypeUMPresentation - quantityAux);
						var quantitySalesAux = (quantityAux) / ((quotationDetail.Item.Presentation?.minimum * factorConversionAux) ?? 1);




						//var quantitySalesAux = (quotationDetail.quantityTypeUMPresentation) / ((quotationDetail.Item.Presentation?.minimum * factorConversionAux) ?? 1);


						quantitySalesAux = TruncateDecimal(quantitySalesAux);
						var quantityTypeUMPresentation = quantitySalesAux * ((quotationDetail.Item.Presentation?.minimum * factorConversionAux) ?? 1);

						decimal iva = SalesRequestDetailIVA(quotationDetail.id_item, quantityTypeUMPresentation, price);

						SalesRequestDetail newSalesRequestDetail = new SalesRequestDetail
						{
							id_item = quotationDetail.id_item,
							Item = quotationDetail.Item,
							quantityTypeUMSale = quantitySalesAux,
							quantityRequested = quantityTypeUMPresentation,
							quantityApproved = quantityTypeUMPresentation,
							quantityOutstandingOrder = quantityTypeUMPresentation,
							id_metricUnitTypeUMPresentation = quotationDetail.id_metricUnitTypeUMPresentation,
							MetricUnit = quotationDetail.MetricUnit,

							price = price,
							iva = iva,
							subtotal = price * quantityTypeUMPresentation,
							total = (price * quantityTypeUMPresentation) + iva,

							isActive = true,
							id_userCreate = ActiveUser.id,
							dateCreate = DateTime.Now,
							id_userUpdate = ActiveUser.id,
							dateUpdate = DateTime.Now,

							SalesRequestDetailSalesQuotation = new List<SalesRequestDetailSalesQuotation>()

						};

						SalesRequestDetailSalesQuotation newSalesRequestDetailSalesQuotation = new SalesRequestDetailSalesQuotation
						{

							id_salesQuotation = quotationDetail.id_salesQuotation,
							SalesQuotation = quotationDetail.SalesQuotation,
							id_salesQuotationDetail = quotationDetail.id,
							SalesQuotationDetail = quotationDetail,
							quantity = quantityTypeUMPresentation
						};
						newSalesRequestDetail.SalesRequestDetailSalesQuotation.Add(newSalesRequestDetailSalesQuotation);

						salesRequest.SalesRequestDetail.Add(newSalesRequestDetail);
						//}
					}
					//}
				}
			}

			salesRequest.SalesRequestTotal = SalesRequestTotals(salesRequest.id, salesRequest.SalesRequestDetail.ToList());

			TempData["request"] = salesRequest;
			TempData.Keep("request");

			return PartialView("_FormEditSalesRequest", salesRequest);
		}

		#endregion

		#region SALES REQUEST HEADER

		[HttpPost, ValidateInput(false)]
		public ActionResult SalesRequestPartial()
		{
			var model = (TempData["model"] as List<SalesRequest>);
			model = model ?? new List<SalesRequest>();

			TempData.Keep("model"); return PartialView("_SalesRequestsPartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult SalesRequestPartialAddNew(bool approve, SalesRequest itemSR, Document itemDoc)
		{
			var model = db.SalesRequest;
			SalesRequest tempRequest = (TempData["request"] as SalesRequest);
			tempRequest = tempRequest ?? new SalesRequest();

			tempRequest.id_customer = itemSR.id_customer;
			tempRequest.id_employeeSeller = itemSR.id_employeeSeller;
			tempRequest.id_priceList = itemSR.id_priceList;
			tempRequest.id_PaymentMethod = itemSR.id_PaymentMethod;
			tempRequest.id_PaymentTerm = itemSR.id_PaymentTerm;
			tempRequest.id_portDestination = itemSR.id_portDestination;
			tempRequest.Document = itemDoc;

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					#region Document

					itemDoc.id_userCreate = ActiveUser.id;
					itemDoc.dateCreate = DateTime.Now;
					itemDoc.id_userUpdate = ActiveUser.id;
					itemDoc.dateUpdate = DateTime.Now;
					itemDoc.sequential = GetDocumentSequential(itemDoc.id_documentType);
					itemDoc.number = GetDocumentNumber(itemDoc.id_documentType);

					DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == itemDoc.id_documentType);
					itemDoc.DocumentType = documentType;

					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == itemDoc.id_documentState);
					itemDoc.DocumentState = documentState;

					Employee employee = db.Employee.FirstOrDefault(e => e.id == ActiveUser.id);
					itemSR.Employee = employee;
					tempRequest.Employee = employee;

					itemDoc.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
					itemDoc.id_emissionPoint = ActiveEmissionPoint.id;

					string emissionDate = itemDoc.emissionDate.ToString("dd/MM/yyyy").Replace("/", "");

					itemDoc.accessKey = AccessKey.GenerateAccessKey(emissionDate,
																	itemDoc.DocumentType.code,
																	itemDoc.EmissionPoint.BranchOffice.Division.Company.ruc,
																	"1",
																	itemDoc.EmissionPoint.BranchOffice.code.ToString() + itemDoc.EmissionPoint.code.ToString("D3"),
																	itemDoc.sequential.ToString("D9"),
																	itemDoc.sequential.ToString("D8"),
																	"1");

					//Actualiza Secuencial
					if (documentType != null)
					{
						documentType.currentNumber = documentType.currentNumber + 1;
						db.DocumentType.Attach(documentType);
						db.Entry(documentType).State = EntityState.Modified;
					}
					//itemPR.Employee.id = itemPR.id_personRequesting;


					#endregion

					#region SaleRequest

					itemSR.Document = itemDoc;

					#endregion

					#region SalesRequestDetails

					if (tempRequest?.SalesRequestDetail != null)
					{
						itemSR.SalesRequestDetail = new List<SalesRequestDetail>();
						var itemSRDetails = tempRequest.SalesRequestDetail.ToList();

						foreach (var detail in itemSRDetails)
						{
							var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
							if (approve)
							{
								if (detail.price <= 0)
								{
									throw new Exception("No se puede aprobar el requerimiento de Pedido, Ítem: " + itemAux.name + " debe tener el precio mayor que cero.");
								}
								if (detail.quantityApproved <= 0)
								{
									throw new Exception("No se puede aprobar el requerimiento de Pedido, Ítem: " + itemAux.name + " debe tener la cantidad aprobada mayor que cero.");
								}
							}

							var tempItemSRDetail = new SalesRequestDetail
							{
								id_item = detail.id_item,
								Item = itemAux,

								quantityRequested = detail.quantityRequested,
								quantityApproved = detail.quantityApproved,
								quantityOutstandingOrder = detail.quantityOutstandingOrder,
								quantityTypeUMSale = detail.quantityTypeUMSale,
								id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation,
								MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == detail.id_metricUnitTypeUMPresentation),

								price = detail.price,
								discount = detail.discount,
								iva = detail.iva,

								subtotal = detail.subtotal,
								total = detail.total,

								isActive = detail.isActive,
								id_userCreate = detail.id_userCreate,
								dateCreate = detail.dateCreate,
								id_userUpdate = detail.id_userUpdate,
								dateUpdate = detail.dateUpdate,

								SalesRequestDetailSalesQuotation = new List<SalesRequestDetailSalesQuotation>(),
								SalesRequestDetailBusinessOportunity = new List<SalesRequestDetailBusinessOportunity>()
							};

							foreach (var requestDetail in detail.SalesRequestDetailSalesQuotation)
							{
								tempItemSRDetail.SalesRequestDetailSalesQuotation.Add(new SalesRequestDetailSalesQuotation
								{
									id_salesRequestDetail = detail.id,
									SalesRequestDetail = tempItemSRDetail,
									id_salesQuotationDetail = requestDetail.id_salesQuotationDetail,
									SalesQuotationDetail = db.SalesQuotationDetail.FirstOrDefault(i => i.id == requestDetail.id_salesQuotationDetail),
									id_salesQuotation = requestDetail.id_salesQuotation,
									SalesQuotation = db.SalesQuotation.FirstOrDefault(i => i.id == requestDetail.id_salesQuotation),
									quantity = detail.quantityApproved
								});
							}

							foreach (var requestDetail in detail.SalesRequestDetailBusinessOportunity)
							{
								tempItemSRDetail.SalesRequestDetailBusinessOportunity.Add(new SalesRequestDetailBusinessOportunity
								{
									id_salesRequestDetail = detail.id,
									SalesRequestDetail = tempItemSRDetail,
									id_businessOportunityPlanningDetail = requestDetail.id_businessOportunityPlanningDetail,
									BusinessOportunityPlanningDetail = db.BusinessOportunityPlanningDetail.FirstOrDefault(i => i.id == requestDetail.id_businessOportunityPlanningDetail),
									id_businessOportunity = requestDetail.id_businessOportunity,
									BusinessOportunity = db.BusinessOportunity.FirstOrDefault(i => i.id == requestDetail.id_businessOportunity),
									quantity = detail.quantityTypeUMSale
								});
							}

							if (tempItemSRDetail.isActive)
								itemSR.SalesRequestDetail.Add(tempItemSRDetail);
						}
					}

					#endregion

					#region TOTALS

					itemSR.SalesRequestTotal = SalesRequestTotals(itemSR.id, itemSR.SalesRequestDetail.ToList());

					#endregion

					if (itemSR.SalesRequestDetail.Count == 0)
					{
						throw new Exception("No se puede guardar un requerimiento de pedido sin detalles");
					}

					if (approve)
					{
						itemSR.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
					}

					//Guarda Requerimiento de Pedido
					db.SalesRequest.Add(itemSR);
					db.SaveChanges();
					trans.Commit();

					TempData["request"] = itemSR;
					TempData.Keep("request");

					ViewData["EditMessage"] = SuccessMessage("Requerimiento de Pedido " + itemSR.Document.number + " guardada exitosamente");
				}
				catch (Exception e)
				{
					TempData["request"] = tempRequest;
					TempData.Keep("request");
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
					return PartialView("_SalesRequestMainFormPartial", tempRequest);
				}

				return PartialView("_SalesRequestMainFormPartial", itemSR);
			}
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult SalesRequestPartialUpdate(bool approve, SalesRequest itemSR, Document itemDoc)
		{
			var model = db.SalesRequest;

			SalesRequest tempRequest = (TempData["request"] as SalesRequest);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					var modelItem = model.FirstOrDefault(it => it.id == itemSR.id);
					if (modelItem != null)
					{
						#region UPDATE TEMPREQUEST

						tempRequest.Document.description = itemDoc.description;
						tempRequest.Document.reference = itemDoc.reference;
						tempRequest.Document.emissionDate = itemDoc.emissionDate;

						tempRequest.id_customer = itemSR.id_customer;
						tempRequest.id_priceList = itemSR.id_priceList;
						tempRequest.id_PaymentMethod = itemSR.id_PaymentMethod;
						tempRequest.id_PaymentTerm = itemSR.id_PaymentTerm;
						tempRequest.id_portDestination = itemSR.id_portDestination;

						#endregion

						#region DOCUMENT

						modelItem.Document.description = itemDoc.description;
						modelItem.Document.reference = itemDoc.reference;
						modelItem.Document.emissionDate = itemDoc.emissionDate;

						modelItem.Document.id_userUpdate = ActiveUser.id;
						modelItem.Document.dateUpdate = DateTime.Now;

						#endregion

						#region SALE REQUEST

						modelItem.id_customer = itemSR.id_customer;
						modelItem.Customer = db.Customer.FirstOrDefault(fod => fod.id == itemSR.id_customer);
						tempRequest.Customer = modelItem.Customer;

						//modelItem.id_employeeSeller = itemSR.id_employeeSeller;
						modelItem.id_priceList = itemSR.id_priceList;
						modelItem.PriceList = db.PriceList.FirstOrDefault(fod => fod.id == itemSR.id_priceList);
						tempRequest.PriceList = modelItem.PriceList;

						modelItem.id_PaymentMethod = itemSR.id_PaymentMethod;
						modelItem.PaymentMethod = db.PaymentMethod.FirstOrDefault(fod => fod.id == itemSR.id_PaymentMethod);
						tempRequest.PaymentMethod = modelItem.PaymentMethod;

						modelItem.id_PaymentTerm = itemSR.id_PaymentTerm;
						modelItem.PaymentTerm = db.PaymentTerm.FirstOrDefault(fod => fod.id == itemSR.id_PaymentTerm);
						tempRequest.PaymentTerm = modelItem.PaymentTerm;

						modelItem.id_portDestination = itemSR.id_portDestination;
						modelItem.Port = db.Port.FirstOrDefault(fod => fod.id == itemSR.id_portDestination);
						tempRequest.Port = modelItem.Port;

						#endregion

						#region SALE REQUEST DETAILS

						int count = 0;
						if (tempRequest?.SalesRequestDetail != null)
						{
							var details = tempRequest.SalesRequestDetail.ToList();

							foreach (var detail in details)
							{
								SalesRequestDetail requestDetail = modelItem.SalesRequestDetail.FirstOrDefault(d => d.id == detail.id);

								var itemAux = db.Item.FirstOrDefault(i => i.id == detail.id_item);
								if (approve)
								{
									if (detail.price <= 0)
									{
										throw new Exception("No se puede aprobar el requerimiento de Pedido, Ítem: " + itemAux.name + " debe tener el precio mayor que cero.");
									}
									if (detail.quantityApproved <= 0)
									{
										throw new Exception("No se puede aprobar el requerimiento de Pedido, Ítem: " + itemAux.name + " debe tener la cantidad aprobada mayor que cero.");
									}
								}

								if (requestDetail == null)
								{
									requestDetail = new SalesRequestDetail()
									{
										id_item = detail.id_item,
										Item = itemAux,

										quantityRequested = detail.quantityRequested,
										quantityApproved = detail.quantityApproved,
										quantityOutstandingOrder = detail.quantityOutstandingOrder,
										quantityTypeUMSale = detail.quantityTypeUMSale,
										id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation,
										MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == detail.id_metricUnitTypeUMPresentation),

										price = detail.price,
										discount = detail.discount,
										iva = detail.iva,

										subtotal = detail.subtotal,
										total = detail.total,

										isActive = detail.isActive,
										id_userCreate = detail.id_userCreate,
										dateCreate = detail.dateCreate,
										id_userUpdate = detail.id_userUpdate,
										dateUpdate = detail.dateUpdate,

										SalesRequestDetailSalesQuotation = new List<SalesRequestDetailSalesQuotation>(),
										SalesRequestDetailBusinessOportunity = new List<SalesRequestDetailBusinessOportunity>()
									};

									foreach (var salesRequestDetailSalesQuotationDetail in detail.SalesRequestDetailSalesQuotation)
									{
										requestDetail.SalesRequestDetailSalesQuotation.Add(new SalesRequestDetailSalesQuotation
										{
											id_salesRequestDetail = detail.id,
											SalesRequestDetail = requestDetail,
											id_salesQuotationDetail = salesRequestDetailSalesQuotationDetail.id_salesQuotationDetail,
											SalesQuotationDetail = db.SalesQuotationDetail.FirstOrDefault(i => i.id == salesRequestDetailSalesQuotationDetail.id_salesQuotationDetail),
											id_salesQuotation = salesRequestDetailSalesQuotationDetail.id_salesQuotation,
											SalesQuotation = db.SalesQuotation.FirstOrDefault(i => i.id == salesRequestDetailSalesQuotationDetail.id_salesQuotation),
											quantity = detail.quantityApproved
										});
									}

									foreach (var salesRequestDetailBusinessOportunityDetail in detail.SalesRequestDetailBusinessOportunity)
									{
										requestDetail.SalesRequestDetailBusinessOportunity.Add(new SalesRequestDetailBusinessOportunity
										{
											id_salesRequestDetail = detail.id,
											SalesRequestDetail = requestDetail,
											id_businessOportunityPlanningDetail = salesRequestDetailBusinessOportunityDetail.id_businessOportunityPlanningDetail,
											BusinessOportunityPlanningDetail = db.BusinessOportunityPlanningDetail.FirstOrDefault(i => i.id == salesRequestDetailBusinessOportunityDetail.id_businessOportunityPlanningDetail),
											id_businessOportunity = salesRequestDetailBusinessOportunityDetail.id_businessOportunity,
											BusinessOportunity = db.BusinessOportunity.FirstOrDefault(i => i.id == salesRequestDetailBusinessOportunityDetail.id_businessOportunity),
											quantity = detail.quantityTypeUMSale
										});
									}

									if (requestDetail.isActive)
									{
										modelItem.SalesRequestDetail.Add(requestDetail);
										count++;
									}
								}
								else
								{
									requestDetail.id_item = detail.id_item;
									requestDetail.Item = itemAux;

									requestDetail.quantityRequested = detail.quantityRequested;
									requestDetail.quantityApproved = detail.quantityApproved;
									requestDetail.quantityOutstandingOrder = detail.quantityOutstandingOrder;
									requestDetail.quantityTypeUMSale = detail.quantityTypeUMSale;
									requestDetail.id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation;
									requestDetail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == detail.id_metricUnitTypeUMPresentation);

									requestDetail.price = detail.price;
									requestDetail.discount = detail.discount;
									requestDetail.iva = detail.iva;

									requestDetail.subtotal = detail.subtotal;
									requestDetail.total = detail.total;

									requestDetail.isActive = detail.isActive;
									//requestDetail.id_userCreate = detail.id_userCreate;
									//requestDetail.dateCreate = detail.dateCreate;
									requestDetail.id_userUpdate = detail.id_userUpdate;
									requestDetail.dateUpdate = detail.dateUpdate;


									foreach (var salesRequestDetailSalesQuotationDetail in detail.SalesRequestDetailSalesQuotation)
									{
										SalesRequestDetailSalesQuotation tempSalesRequestDetailSalesQuotation = requestDetail.SalesRequestDetailSalesQuotation.FirstOrDefault(d => d.id == salesRequestDetailSalesQuotationDetail.id);
										tempSalesRequestDetailSalesQuotation.quantity = detail.quantityApproved;
										//tempItemSRDetail.SalesRequestDetailSalesQuotation.Add(new SalesRequestDetailSalesQuotation
										//{
										//    id_salesRequestDetail = detail.id,
										//    SalesRequestDetail = tempItemSRDetail,
										//    id_salesQuotationDetail = salesRequestDetailSalesQuotationDetail.id_salesQuotationDetail,
										//    SalesQuotationDetail = db.SalesQuotationDetail.FirstOrDefault(i => i.id == salesRequestDetailSalesQuotationDetail.id_salesQuotationDetail),
										//    id_salesQuotation = salesRequestDetailSalesQuotationDetail.id_salesQuotation,
										//    SalesQuotation = db.SalesQuotation.FirstOrDefault(i => i.id == salesRequestDetailSalesQuotationDetail.id_salesQuotation),
										//    quantity = detail.quantityApproved
										//});
									}

									foreach (var salesRequestDetailBusinessOportunityDetail in detail.SalesRequestDetailBusinessOportunity)
									{
										SalesRequestDetailBusinessOportunity tempSalesRequestDetailBusinessOportunity = requestDetail.SalesRequestDetailBusinessOportunity.FirstOrDefault(d => d.id == salesRequestDetailBusinessOportunityDetail.id);
										tempSalesRequestDetailBusinessOportunity.quantity = detail.quantityTypeUMSale;
										//tempItemSRDetail.SalesRequestDetailBusinessOportunity.Add(new SalesRequestDetailBusinessOportunity
										//{
										//    id_salesRequestDetail = detail.id,
										//    SalesRequestDetail = tempItemSRDetail,
										//    id_businessOportunityPlanningDetail = salesRequestDetailBusinessOportunityDetail.id_businessOportunityPlanningDetail,
										//    BusinessOportunityPlanningDetail = db.BusinessOportunityPlanningDetail.FirstOrDefault(i => i.id == salesRequestDetailBusinessOportunityDetail.id_businessOportunityPlanningDetail),
										//    id_businessOportunity = salesRequestDetailBusinessOportunityDetail.id_businessOportunity,
										//    BusinessOportunity = db.BusinessOportunity.FirstOrDefault(i => i.id == salesRequestDetailBusinessOportunityDetail.id_businessOportunity),
										//    quantity = detail.quantityTypeUMSale
										//});
									}

									if (requestDetail.isActive)
										count++;
								}
							}
							//}

							// UPDATE TOTALS
							modelItem.SalesRequestTotal = SalesRequestTotals(modelItem.id, details);
						}

						if (count == 0)
						{
							throw new Exception("No se puede guardar un requerimiento de pedido sin detalles");
							//TempData.Keep("request");
							//ViewData["EditMessage"] = ErrorMessage("No se puede guardar un requerimiento sin detalles");
							//return PartialView("_SalesRequestsPartial", tempRequest);
						}

						#endregion

						if (approve)
						{
							modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
						}

						//modelItem.Document.description = itemDoc.description;
						//modelItem.Document.id_userUpdate = ActiveUser.id;
						//modelItem.Document.dateUpdate = DateTime.Now;

						db.SalesRequest.Attach(modelItem);
						db.Entry(modelItem).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						itemSR = modelItem;
						TempData["request"] = itemSR;
						TempData.Keep("request");

						ViewData["EditMessage"] = SuccessMessage("Requerimiento de Pedido: " + itemSR.Document.number + " guardado exitosamente");

					}
				}
				catch (Exception e)
				{
					TempData["request"] = tempRequest;
					TempData.Keep("request");
					ViewData["EditMessage"] = ErrorMessage(e.Message);
					trans.Rollback();
					return PartialView("_SalesRequestMainFormPartial", tempRequest);
					//TempData.Keep("request");
					//ViewData["EditMessage"] = ErrorMessage();
					//trans.Rollback();
				}
			}

			return PartialView("_SalesRequestMainFormPartial", itemSR);
		}

		#endregion

		#region SALES REQUEST DETAIL

		[HttpPost, ValidateInput(false)]
		public ActionResult SalesRequestDetailsPartial()
		{
			SalesRequest salesRequest = (TempData["request"] as SalesRequest);
			salesRequest = salesRequest ?? new SalesRequest();

			var model = salesRequest.SalesRequestDetail.Where(d => d.isActive).ToList();

			TempData["request"] = salesRequest;
			TempData.Keep("request");

			return PartialView("_SalesRequestEditFormDetailPartial", model.OrderByDescending(obd => obd.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult SalesRequestDetailsPartialAddNew(SalesRequestDetail salesDetail)
		{
			SalesRequest salesRequest = (TempData["request"] as SalesRequest);
			salesRequest = salesRequest ?? new SalesRequest();

			if (ModelState.IsValid)
			{
				try
				{
					salesDetail.id = salesRequest.SalesRequestDetail.Count() > 0 ? salesRequest.SalesRequestDetail.Max(pld => pld.id) + 1 : 1;
					salesDetail.id_salesRequest = salesRequest.id;
					salesDetail.SalesRequest = salesRequest;
					salesDetail.Item = db.Item.FirstOrDefault(i => i.id == salesDetail.id_item);
					salesDetail.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == salesDetail.id_metricUnitTypeUMPresentation);

					salesDetail.isActive = true;
					salesDetail.id_userCreate = ActiveUser.id;
					salesDetail.dateCreate = DateTime.Now;
					salesDetail.id_userUpdate = ActiveUser.id;
					salesDetail.dateUpdate = DateTime.Now;

					salesRequest.SalesRequestDetail.Add(salesDetail);

					salesRequest.SalesRequestTotal = SalesRequestTotals(salesRequest.id, salesRequest.SalesRequestDetail.ToList());
					TempData["request"] = salesRequest;
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por Favor, corrija todos los errores.";

			TempData.Keep("request");

			var model = salesRequest?.SalesRequestDetail.Where(d => d.isActive).ToList() ?? new List<SalesRequestDetail>();

			return PartialView("_SalesRequestEditFormDetailPartial", model.OrderByDescending(obd => obd.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult SalesRequestDetailsPartialUpdate(SalesRequestDetail requestDetail)
		{
			SalesRequest salesRequest = (TempData["request"] as SalesRequest);
			salesRequest = salesRequest ?? new SalesRequest();

			if (ModelState.IsValid)
			{
				try
				{
					//var modelItem = salesRequest.SalesRequestDetail.FirstOrDefault(it => it.id_item == requestDetail.id_item);
					SalesRequestDetail modelItem = salesRequest.SalesRequestDetail.FirstOrDefault(it => it.id == requestDetail.id);
					// var modelItem = model.FirstOrDefault(it => it.id_item == item.id_item);
					if (modelItem != null)
					{
						modelItem.id_item = requestDetail.id_item;
						modelItem.Item = db.Item.FirstOrDefault(i => i.id == requestDetail.id_item);

						modelItem.quantityRequested = requestDetail.quantityRequested;
						modelItem.quantityApproved = requestDetail.quantityApproved;
						modelItem.quantityOutstandingOrder = requestDetail.quantityOutstandingOrder;
						modelItem.quantityTypeUMSale = requestDetail.quantityTypeUMSale;
						modelItem.id_metricUnitTypeUMPresentation = requestDetail.id_metricUnitTypeUMPresentation;
						modelItem.MetricUnit = db.MetricUnit.FirstOrDefault(i => i.id == requestDetail.id_metricUnitTypeUMPresentation);

						modelItem.price = requestDetail.price;
						modelItem.discount = requestDetail.discount;
						modelItem.iva = requestDetail.iva;
						modelItem.subtotal = requestDetail.subtotal;
						modelItem.total = requestDetail.total;

						modelItem.id_userUpdate = ActiveUser.id;
						modelItem.dateUpdate = DateTime.Now;

						this.UpdateModel(modelItem);
						salesRequest.SalesRequestTotal = SalesRequestTotals(salesRequest.id, salesRequest.SalesRequestDetail.ToList());
						TempData["request"] = salesRequest;
					}
					//db.SaveChanges();
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Por Favor, corrija todos los errores.";

			TempData.Keep("request");

			var model = salesRequest?.SalesRequestDetail.Where(d => d.isActive).ToList() ?? new List<SalesRequestDetail>();

			return PartialView("_SalesRequestEditFormDetailPartial", model.OrderByDescending(obd => obd.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult SalesRequestDetailsPartialDelete(System.Int32 id)
		{
			SalesRequest salesRequest = (TempData["request"] as SalesRequest);
			salesRequest = salesRequest ?? new SalesRequest();

			//if (id_item >= 0)
			//{
			try
			{
				var salesDetail = salesRequest.SalesRequestDetail.FirstOrDefault(p => p.id == id);
				if (salesDetail != null)
				{
					salesDetail.isActive = false;
					salesDetail.id_userUpdate = ActiveUser.id;
					salesDetail.dateCreate = DateTime.Now;
				}
				salesRequest.SalesRequestTotal = SalesRequestTotals(salesRequest.id, salesRequest.SalesRequestDetail.ToList());

				TempData["request"] = salesRequest;
			}
			catch (Exception e)
			{
				ViewData["EditError"] = e.Message;
			}
			//}

			TempData.Keep("request");

			var model = salesRequest?.SalesRequestDetail.Where(d => d.isActive).ToList() ?? new List<SalesRequestDetail>();

			return PartialView("_SalesRequestEditFormDetailPartial", model.OrderByDescending(obd => obd.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public void SalesRequestDetailsDeleteSelectedItems(int[] ids)
		{
			SalesRequest salesRequest = (TempData["request"] as SalesRequest);
			salesRequest = salesRequest ?? new SalesRequest();

			if (ids != null)
			{
				try
				{
					var requestDetails = salesRequest.SalesRequestDetail.Where(i => ids.Contains(i.id_item));

					foreach (var detail in requestDetails)
					{
						detail.isActive = false;
						detail.id_userUpdate = ActiveUser.id;
						detail.dateUpdate = DateTime.Now;
					}

					TempData["request"] = salesRequest;
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}

			TempData.Keep("request");
		}

		#endregion

		#region SINGLE CHANGE DOCUMENT STATE

		[HttpPost]
		public ActionResult Approve(int id)
		{
			SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

					if (salesRequest != null && documentState != null)
					{
						salesRequest.Document.id_documentState = documentState.id;
						salesRequest.Document.DocumentState = documentState;

						foreach (var details in salesRequest.SalesRequestDetail)
						{
							details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

							db.SalesRequestDetail.Attach(details);
							db.Entry(details).State = EntityState.Modified;
						}

						db.SalesRequest.Attach(salesRequest);
						db.Entry(salesRequest).State = EntityState.Modified;

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

			TempData["request"] = salesRequest;
			TempData.Keep("request");

			return PartialView("_SalesRequestMainFormPartial", salesRequest);
		}

		[HttpPost]
		public ActionResult Autorize(int id)
		{
			SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06"); //Autorizada

					if (salesRequest != null && documentState != null)
					{
						salesRequest.Document.id_documentState = documentState.id;
						salesRequest.Document.DocumentState = documentState;

						//foreach (var details in salesRequest.SalesRequestDetail)
						//{
						//    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

						//    db.SalesRequestDetail.Attach(details);
						//    db.Entry(details).State = EntityState.Modified;
						//}

						db.SalesRequest.Attach(salesRequest);
						db.Entry(salesRequest).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						TempData["request"] = salesRequest;
						TempData.Keep("request");

						ViewData["EditMessage"] = SuccessMessage("Requerimiento de Pedido: " + salesRequest.Document.number + " autorizada exitosamente");
					}
				}
				catch (Exception e)
				{
					TempData.Keep("request");
					ViewData["EditError"] = ErrorMessage(e.Message);
					trans.Rollback();
					//ViewData["EditError"] = e.Message;
					//trans.Rollback();
				}
			}



			return PartialView("_SalesRequestMainFormPartial", salesRequest);
		}

		[HttpPost]
		public ActionResult Protect(int id)
		{
			SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

					if (salesRequest != null && documentState != null)
					{
						salesRequest.Document.id_documentState = documentState.id;
						salesRequest.Document.DocumentState = documentState;

						db.SalesRequest.Attach(salesRequest);
						db.Entry(salesRequest).State = EntityState.Modified;

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

			TempData["request"] = salesRequest;
			TempData.Keep("request");

			return PartialView("_SalesRequestMainFormPartial", salesRequest);
		}

		[HttpPost]
		public ActionResult Cancel(int id)
		{
			SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					var existInSalesRequestOrQuotationDetailProductionScheduleRequestDetail = salesRequest.SalesRequestDetail.FirstOrDefault(fod => fod.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.FirstOrDefault(fod2 => fod2.ProductionScheduleRequestDetail.ProductionSchedule.Document.DocumentState.code != "05" &&
																																																				   fod2.ProductionScheduleRequestDetail.ProductionSchedule.Document.DocumentState.code != "01") != null);//Diferente de Anulado y Pendiente

					if (existInSalesRequestOrQuotationDetailProductionScheduleRequestDetail != null)
					{
						throw new Exception("No se puede cancelar el requerimiento de pedido, debido a tener detalles que pertenecen a alguna programación de producción.");
					}

					var existInSalesOrderDetailSalesRequest = salesRequest.SalesRequestDetail.FirstOrDefault(fod => fod.SalesOrderDetailSalesRequest.FirstOrDefault(fod2 => fod2.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "05" &&
																																																			 fod2.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "01") != null);//Diferente de Anulado y Pendiente

					if (existInSalesOrderDetailSalesRequest != null)
					{
						throw new Exception("No se puede cancelar el requerimiento de pedido, debido a tener detalles que pertenecen a alguna orden de producción.");
					}

					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

					if (salesRequest != null && documentState != null)
					{
						salesRequest.Document.id_documentState = documentState.id;
						salesRequest.Document.DocumentState = documentState;

						db.SalesRequest.Attach(salesRequest);
						db.Entry(salesRequest).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						TempData["request"] = salesRequest;
						TempData.Keep("request");

						ViewData["EditMessage"] = SuccessMessage("Requerimiento de Pedido: " + salesRequest.Document.number + " cancelado exitosamente");

					}
				}
				catch (Exception e)
				{
					TempData.Keep("request");
					ViewData["EditError"] = ErrorMessage(e.Message);
					trans.Rollback();
					//ViewData["EditError"] = e.Message;
					//trans.Rollback();
				}
			}

			//TempData["request"] = salesRequest;
			//TempData.Keep("request");

			return PartialView("_SalesRequestMainFormPartial", salesRequest);
		}

		[HttpPost]
		public ActionResult Revert(int id)
		{
			SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					var existInSalesRequestOrQuotationDetailProductionScheduleRequestDetail = salesRequest.SalesRequestDetail.FirstOrDefault(fod => fod.SalesRequestOrQuotationDetailProductionScheduleRequestDetail.FirstOrDefault(fod2 => fod2.ProductionScheduleRequestDetail.ProductionSchedule.Document.DocumentState.code != "05" &&
																																																				   fod2.ProductionScheduleRequestDetail.ProductionSchedule.Document.DocumentState.code != "01") != null);//Diferente de Anulado y Pendiente

					if (existInSalesRequestOrQuotationDetailProductionScheduleRequestDetail != null)
					{
						throw new Exception("No se puede reversar el requerimiento de pedido, debido a tener detalles que pertenecen a alguna programación de producción.");
					}

					var existInSalesOrderDetailSalesRequest = salesRequest.SalesRequestDetail.FirstOrDefault(fod => fod.SalesOrderDetailSalesRequest.FirstOrDefault(fod2 => fod2.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "05" &&
																																																			 fod2.SalesOrderDetail.SalesOrder.Document.DocumentState.code != "01") != null);//Diferente de Anulado y Pendiente

					if (existInSalesOrderDetailSalesRequest != null)
					{
						throw new Exception("No se puede reversar el requerimiento de pedido, debido a tener detalles que pertenecen a alguna orden de producción.");
					}

					DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

					if (salesRequest != null && documentState != null)
					{
						salesRequest.Document.id_documentState = documentState.id;
						salesRequest.Document.DocumentState = documentState;

						//foreach (var details in salesRequest.SalesRequestDetail)
						//{
						//    details.quantityApproved = 0.0M;

						//    db.SalesRequestDetail.Attach(details);
						//    db.Entry(details).State = EntityState.Modified;
						//}

						db.SalesRequest.Attach(salesRequest);
						db.Entry(salesRequest).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();

						TempData["request"] = salesRequest;
						TempData.Keep("request");

						ViewData["EditMessage"] = SuccessMessage("Requerimiento de Pedido: " + salesRequest.Document.number + " reversado exitosamente");

					}
				}
				catch (Exception e)
				{
					TempData.Keep("request");
					ViewData["EditError"] = ErrorMessage(e.Message);
					trans.Rollback();
					//ViewData["EditError"] = e.Message;
					//trans.Rollback();
				}
			}

			//TempData["request"] = salesRequest;
			//TempData.Keep("request");

			return PartialView("_SalesRequestMainFormPartial", salesRequest);
		}

		#endregion

		#region SELECTED DOCUMENT STATE CHANGE

		[HttpPost, ValidateInput(false)]
		public void ApproveDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						foreach (var id in ids)
						{
							SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 3);

							if (salesRequest != null && documentState != null)
							{
								salesRequest.Document.id_documentState = documentState.id;
								salesRequest.Document.DocumentState = documentState;

								foreach (var details in salesRequest.SalesRequestDetail)
								{
									details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;
									db.SalesRequestDetail.Attach(details);
									db.Entry(details).State = EntityState.Modified;
								}

								db.SalesRequest.Attach(salesRequest);
								db.Entry(salesRequest).State = EntityState.Modified;
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			var model = (TempData["model"] as List<SalesRequest>);
			model = model ?? new List<SalesRequest>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.SalesRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["model"] = model;
			TempData.Keep("model");
		}

		[HttpPost, ValidateInput(false)]
		public void AutorizeDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						foreach (var id in ids)
						{
							SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 6);

							if (salesRequest != null && documentState != null)
							{
								salesRequest.Document.id_documentState = documentState.id;
								salesRequest.Document.DocumentState = documentState;

								foreach (var details in salesRequest.SalesRequestDetail)
								{
									details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

									db.SalesRequestDetail.Attach(details);
									db.Entry(details).State = EntityState.Modified;
								}

								db.SalesRequest.Attach(salesRequest);
								db.Entry(salesRequest).State = EntityState.Modified;
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			var model = (TempData["model"] as List<SalesRequest>);
			model = model ?? new List<SalesRequest>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.SalesRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["model"] = model;
			TempData.Keep("model");
		}

		[HttpPost, ValidateInput(false)]
		public void ProtectDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						foreach (var id in ids)
						{
							SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 4);

							if (salesRequest != null && documentState != null)
							{
								salesRequest.Document.id_documentState = documentState.id;
								salesRequest.Document.DocumentState = documentState;

								db.SalesRequest.Attach(salesRequest);
								db.Entry(salesRequest).State = EntityState.Modified;
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			var model = (TempData["model"] as List<SalesRequest>);
			model = model ?? new List<SalesRequest>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.SalesRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["model"] = model;
			TempData.Keep("model");
		}

		[HttpPost, ValidateInput(false)]
		public void CancelDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						foreach (var id in ids)
						{
							SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 5);

							if (salesRequest != null && documentState != null)
							{
								salesRequest.Document.id_documentState = documentState.id;
								salesRequest.Document.DocumentState = documentState;

								db.SalesRequest.Attach(salesRequest);
								db.Entry(salesRequest).State = EntityState.Modified;
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			var model = (TempData["model"] as List<SalesRequest>);
			model = model ?? new List<SalesRequest>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.SalesRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["model"] = model;
			TempData.Keep("model");
		}

		[HttpPost, ValidateInput(false)]
		public void RevertDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						foreach (var id in ids)
						{
							SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);

							DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.id == 1);

							if (salesRequest != null && documentState != null)
							{
								salesRequest.Document.id_documentState = documentState.id;
								salesRequest.Document.DocumentState = documentState;

								foreach (var details in salesRequest.SalesRequestDetail)
								{
									details.quantityApproved = 0.0M;

									db.SalesRequestDetail.Attach(details);
									db.Entry(details).State = EntityState.Modified;
								}

								db.SalesRequest.Attach(salesRequest);
								db.Entry(salesRequest).State = EntityState.Modified;
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			var model = (TempData["model"] as List<SalesRequest>);
			model = model ?? new List<SalesRequest>();
			int[] filters = model.Select(i => i.id).ToArray();
			model = db.SalesRequest.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			TempData["model"] = model;
			TempData.Keep("model");
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
				btnRevert = false
			};

			if (id == 0)
			{
				return Json(actions, JsonRequestBehavior.AllowGet);
			}

			SalesRequest salesRequest = db.SalesRequest.FirstOrDefault(r => r.id == id);
			string state = salesRequest.Document.DocumentState.code;

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
					btnProtect = false,
					btnCancel = true,
					btnRevert = true,
				};
			}

			return Json(actions, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region PAGINATION

		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_salesRequest)
		{
			TempData.Keep("request");

			int index = db.SalesRequest.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_salesRequest);

			var result = new
			{
				maximunPages = db.SalesRequest.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			SalesRequest salesRequest = db.SalesRequest.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (salesRequest != null)
			{
				TempData["request"] = salesRequest;
				TempData.Keep("request");
				return PartialView("_SalesRequestMainFormPartial", salesRequest);
			}

			TempData.Keep("request");

			return PartialView("_SalesRequestMainFormPartial", new SalesRequest());
		}

		#endregion

		#region AUXILIAR FUNCTIONS

		private SalesRequestTotal SalesRequestTotals(int id_salesRequest, List<SalesRequestDetail> requestDetails)
		{
			SalesRequestTotal salesRequestTotal = db.SalesRequestTotal.FirstOrDefault(t => t.id_salesRequest == id_salesRequest);

			salesRequestTotal = salesRequestTotal ?? new SalesRequestTotal
			{
				id_salesRequest = id_salesRequest
			};

			decimal subtotalIVA12Percent = 0.0M;
			decimal subtotalIVA14Percent = 0.0M;
			decimal subtotalIVA0Percent = 0.0M;
			decimal subtotalIVANoObjectIVA = 0.0M;
			decimal subtotalExentedIVA = 0.0M;

			decimal subtotal = 0.0M;

			decimal discount = 0.0M;
			decimal valueICE = 0.0M;
			decimal valueIRBPNR = 0.0M;

			decimal totalIVA12 = 0.0M;
			decimal totalIVA14 = 0.0M;

			decimal total = 0.0M;

			foreach (var detail in requestDetails)
			{
				if (detail.Item != null && detail.isActive)
				{
					ItemTaxation rateIVA12 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("2"));//Taxtype: "2": Impuesto IVA con Rate: "2": tarifa 12% 
					ItemTaxation rateIVA14 = detail.Item.ItemTaxation.FirstOrDefault(t => t.TaxType.code.Equals("2") && t.Rate.code.Equals("5"));//Taxtype: "2": Impuesto IVA con Rate: "5": tarifa 14% 

					if (rateIVA12 != null)
					{
						subtotalIVA12Percent += detail.quantityApproved * detail.price;
					}
					if (rateIVA14 != null)
					{
						subtotalIVA14Percent += detail.quantityApproved * detail.price;
					}

					subtotal += detail.quantityApproved * detail.price;
					discount += detail.discount;
				}

			}

			//totalIVA12 = subtotalIVA12Percent * 0.12M;
			//totalIVA14 = subtotalIVA14Percent * 0.14M;
			var percent12 = db.Rate.FirstOrDefault(fod => fod.code.Equals("2"))?.percentage / 100 ?? 0.12M; //"2" Tarifa 12%
			var percent14 = db.Rate.FirstOrDefault(fod => fod.code.Equals("5"))?.percentage / 100 ?? 0.14M; //"5" Tarifa 14%
			totalIVA12 = subtotalIVA12Percent * percent12;// 0.12M;
			totalIVA14 = subtotalIVA14Percent * percent14;//0.14M;

			total = subtotal + totalIVA12 + totalIVA14 + valueICE + valueIRBPNR - discount;

			salesRequestTotal.subtotalIVA12Percent = subtotalIVA12Percent;
			salesRequestTotal.subtotalIVA14Percent = subtotalIVA14Percent;
			salesRequestTotal.subtotalIVA0Percent = subtotalIVA0Percent;
			salesRequestTotal.subtotalIVANoObjectIVA = subtotalIVANoObjectIVA;
			salesRequestTotal.subtotalExentedIVA = subtotalExentedIVA;

			salesRequestTotal.subtotal = subtotal;
			salesRequestTotal.discount = discount;
			salesRequestTotal.valueICE = valueICE;
			salesRequestTotal.valueIRBPNR = valueIRBPNR;

			salesRequestTotal.totalIVA12 = totalIVA12;
			salesRequestTotal.totalIVA14 = totalIVA14;

			salesRequestTotal.total = total;

			return salesRequestTotal;
		}

		private decimal SalesRequestDetailPrice(int id_item, SalesRequest salesRequest)
		{
			Item item = db.Item.FirstOrDefault(i => i.id == id_item);

			if (item == null)
			{
				return 0.0M;
			}

			PriceList list = salesRequest.PriceList;

			decimal price = item.ItemSaleInformation.salePrice ?? 0.0M;

			if (list != null)
			{
				PriceListDetail listDetail = list.PriceListDetail.FirstOrDefault(d => d.id_item == item.id);
				price = listDetail?.salePrice ?? price;
			}

			return price;
		}

		private decimal SalesRequestDetailIVA(int id_item, decimal quantity, decimal price)
		{
			decimal iva = 0.0M;

			Item item = db.Item.FirstOrDefault(i => i.id == id_item);

			if (item == null)
			{
				return 0.0M;
			}

			List<ItemTaxation> taxations = item.ItemTaxation.ToList();

			foreach (var taxation in taxations)
			{
				iva += quantity * price * taxation.Rate.percentage / 100.0M;
			}

			return iva;
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult ItemDetailData(int? id, int? id_item, string quantityApproved, string discount, int? id_metricUnitTypeUMPresentation)
		{
			//decimal _quantityRequested = decimal.Parse(quantityRequested, NumberStyles.Any, new CultureInfo(Request.UserLanguages.First()));
			//decimal _price = decimal.Parse(price, NumberStyles.Any, new CultureInfo(Request.UserLanguages.First()));
			decimal _quantityApproved = Convert.ToDecimal(quantityApproved);
			decimal _discount = Convert.ToDecimal(discount);

			SalesRequest request = (TempData["request"] as SalesRequest);

			SalesRequest request2 = (TempData["request2"] as SalesRequest);

			request2 = request2 ?? Copy(request, request2);

			Item item = db.Item.FirstOrDefault(i => i.id == id_item);

			MetricUnit metricUnitTypeUMPresentation = db.MetricUnit.FirstOrDefault(i => i.id == id_metricUnitTypeUMPresentation);


			//Item item = db.Item.FirstOrDefault(i => i.id == id_itemRequest);
			decimal quantityTypeUMSale = 0;
			string metricUnitSale = "";
			string msgErrorConversion = "";
			string msgErrorDiscount = "";
			//decimal _quantitySchedule = Convert.ToDecimal(quantitySchedule ?? "0");

			if (item != null)
			{
				metricUnitSale = item.ItemSaleInformation?.MetricUnit?.code ?? "";

				//metricUnitTypeUMPresentation var metricUnitRequest = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitRequest);

				if (metricUnitTypeUMPresentation != null)
				{
					var id_metricUnitPresentation = item.Presentation?.id_metricUnit;
					var factorConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
																						 muc.id_metricOrigin == metricUnitTypeUMPresentation.id &&
																						 muc.id_metricDestiny == id_metricUnitPresentation);
					if (id_metricUnitPresentation != null && id_metricUnitPresentation == metricUnitTypeUMPresentation.id)
					{
						factorConversion = new MetricUnitConversion() { factor = 1 };
					}
					if (factorConversion == null)
					{
						//var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
						msgErrorConversion = ("Falta el Factor de Conversión entre : " + (metricUnitTypeUMPresentation.code) + " y " + (item.Presentation?.MetricUnit?.code ?? "(UM de Presentación No Existe)") + ". Necesario para calcular las cantidades Configúrelo, e intente de nuevo");
					}
					else
					{
						var quantityAux = _quantityApproved * factorConversion.factor;
						quantityAux /= (item.Presentation?.minimum ?? 1);
						quantityTypeUMSale = TruncateDecimal(quantityAux);
						//var truncateQuantityAux = decimal.Truncate(quantityAux);
						//if ((quantityAux - truncateQuantityAux) > 0)
						//{
						//    quantityAux = truncateQuantityAux + 1;
						//};
						//quantitySale = quantityAux;

						//var id_metricUnitPresentation = item.Presentation?.id_metricUnit;
						factorConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
																							 muc.id_metricOrigin == id_metricUnitPresentation &&
																							 muc.id_metricDestiny == metricUnitTypeUMPresentation.id);
						if (id_metricUnitPresentation != null && id_metricUnitPresentation == metricUnitTypeUMPresentation.id)
						{
							factorConversion = new MetricUnitConversion() { factor = 1 };
						}
						if (factorConversion == null)
						{
							//var metricUnitProductionScheduleProductionOrderDetail = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitProductionScheduleProductionOrderDetail);
							msgErrorConversion = ("Falta el Factor de Conversión entre : " + (item.Presentation?.MetricUnit?.code ?? "(UM de Presentación No Existe)") + " y " + (metricUnitTypeUMPresentation.code) + ". Necesario para calcular las cantidades Configúrelo, e intente de nuevo");
						}
						else
						{
							quantityAux = (quantityTypeUMSale * (item.Presentation?.minimum ?? 1)) * factorConversion.factor;

							_quantityApproved = quantityAux;
						}
					}
				}
			}

			decimal _price;
			try
			{
				_price = SaleDetailPrice(item.id, request2, id_metricUnitTypeUMPresentation);
			}
			catch (Exception e)
			{
				_price = 0;
				msgErrorConversion = e.Message;
			}


			decimal iva = SaleDetailIVA(item.id, _quantityApproved, _price);

			SalesRequestDetail detail = request2.SalesRequestDetail.FirstOrDefault(d => d.id_item == id_item && d.id == id);
			if ((_price * _quantityApproved + iva) <= _discount && _discount != 0)
			{
				msgErrorDiscount = "El descuento no puede ser mayor o igual a la suma de Subtotal mas el Iva";
				_discount = 0;
			}
			if (detail != null)
			{
				detail.price = _price;
				detail.quantityApproved = _quantityApproved;
				detail.discount = _discount;
				request2.SalesRequestTotal = SalesRequestTotals(request2.id, request2.SalesRequestDetail.ToList());
			}
			else
			{
				detail = new SalesRequestDetail()
				{
					id = request2.SalesRequestDetail.Count() > 0 ? request2.SalesRequestDetail.Max(ppd => ppd.id) + 1 : 1,
					//id_item = id_item.Value,
					Item = db.Item.FirstOrDefault(i => i.id == id_item),
					isActive = true,
					price = _price,
					iva = iva,
					subtotal = _price * _quantityApproved,
					discount = _discount,
					total = (_price * _quantityApproved + iva) - _discount,
					SalesRequest = request2,
					id_salesRequest = request2.id,
					quantityApproved = _quantityApproved,
					//id_metricUnitTypeUMPresentation = id_metricUnitTypeUMPresentation.Value,
					MetricUnit = metricUnitTypeUMPresentation
				};
				if (id_item != null) detail.id_item = id_item.Value;
				if (id_metricUnitTypeUMPresentation != null) detail.id_metricUnitTypeUMPresentation = id_metricUnitTypeUMPresentation.Value;

				request2.SalesRequestDetail.Add(detail);
				request2.SalesRequestTotal = SalesRequestTotals(request2.id, request2.SalesRequestDetail.ToList());

			}

			//var metricUnits = item?.Presentation.MetricUnit.MetricType.MetricUnit.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId) || w.id == id_metricUnitTypeUMPresentation).ToList() ?? new List<MetricUnit>();


			var result = new
			{
				ItemDetailData = new
				{
					masterCode = item.masterCode,
					um = item.ItemSaleInformation.MetricUnit.code,
					currentStock = item.ItemInventory.currentStock,
					price = _price,
					iva = iva,
					subtotal = _price * _quantityApproved,
					total = (_price * _quantityApproved + iva) - _discount
				},
				RequestTotal = new
				{
					subtotal = request2.SalesRequestTotal.subtotal,
					subtotalIVA12Percent = request2.SalesRequestTotal.subtotalIVA12Percent,
					totalIVA12 = request2.SalesRequestTotal.totalIVA12,
					discount = request2.SalesRequestTotal.discount,
					subtotalIVA14Percent = request2.SalesRequestTotal.subtotalIVA14Percent,
					totalIVA14 = request2.SalesRequestTotal.totalIVA14,
					total = request2.SalesRequestTotal.total
				},
				quantityTypeUMSale = quantityTypeUMSale,
				metricUnitSale = metricUnitSale,
				quantityApproved = _quantityApproved,
				msgErrorConversion = msgErrorConversion,
				msgErrorDiscount = msgErrorDiscount,
				//id_metricUnitTypeUMPresentationDefault = item?.Presentation.id_metricUnit
			};

			TempData["request2"] = request2;
			TempData.Keep("request2");

			TempData["request"] = request;
			TempData.Keep("request");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		private SalesRequest Copy(SalesRequest request, SalesRequest request2)
		{
			request2 = new SalesRequest()
			{
				id = 0,
				id_customer = request.id_customer,
				Customer = request.Customer,
				id_employeeSeller = request.id_employeeSeller,
				Employee = request.Employee,
				id_priceList = request.id_priceList,
				PriceList = request.PriceList,
			};
			foreach (var detail in request.SalesRequestDetail)
			{
				if (detail.Item != null && detail.isActive)
				{
					var tempItemSRDetail = new SalesRequestDetail
					{
						id = detail.id,
						id_item = detail.id_item,
						Item = db.Item.FirstOrDefault(i => i.id == detail.id_item),

						quantityRequested = detail.quantityRequested,
						quantityApproved = detail.quantityApproved,
						quantityOutstandingOrder = detail.quantityOutstandingOrder,
						quantityTypeUMSale = detail.quantityTypeUMSale,
						id_metricUnitTypeUMPresentation = detail.id_metricUnitTypeUMPresentation,
						MetricUnit = detail.MetricUnit,

						price = detail.price,
						discount = detail.discount,
						iva = detail.iva,

						subtotal = detail.subtotal,
						total = detail.total,

						isActive = detail.isActive,
						id_userCreate = detail.id_userCreate,
						dateCreate = detail.dateCreate,
						id_userUpdate = detail.id_userUpdate,
						dateUpdate = detail.dateUpdate,

						SalesRequestDetailSalesQuotation = detail.SalesRequestDetailSalesQuotation,
						SalesRequestDetailBusinessOportunity = detail.SalesRequestDetailBusinessOportunity

					};
					request2.SalesRequestDetail.Add(tempItemSRDetail);
				}

			}
			return request2;
		}

		private decimal SaleDetailPrice(int id_item, SalesRequest request, int? id_metricUnitTypeUMPresentation)
		{
			Item item = db.Item.FirstOrDefault(i => i.id == id_item);
			MetricUnit metricUnitTypeUMPresentation = db.MetricUnit.FirstOrDefault(i => i.id == id_metricUnitTypeUMPresentation);

			if (item == null || metricUnitTypeUMPresentation == null)
			{
				return 0.0M;
			}

			PriceList list = request.PriceList;

			decimal priceUMPresentation = item.ItemSaleInformation.salePrice / ((item.Presentation?.maximum ?? 1) * (item.Presentation?.minimum ?? 1)) ?? 0.0M;

			decimal priceAux = 0;
			var metricUnitPresentation = item.Presentation?.MetricUnit;

			if (metricUnitPresentation?.id != null)
			{
				var factorConversion = GetFactorConversion(metricUnitTypeUMPresentation, metricUnitPresentation, "Falta el Factor de Conversión entre : " + metricUnitTypeUMPresentation.code + " y " + (metricUnitPresentation.code) + ".Necesario para obtener el precio del detalle del Requerimiento de Pedido Configúrelo, e intente de nuevo", db);
				//var factorConversion = (metricUnitTypeUMPresentation.id != metricUnitPresentation.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitTypeUMPresentation.id &&
				//                                                                                                                             fod.id_metricDestiny == metricUnitPresentation.id)?.factor ?? 0 : 1;
				//priceAux = 0;
				//if (factorConversion == 0)
				//{
				//    throw new Exception("Falta el Factor de Conversión entre : " + metricUnitTypeUMPresentation.code + " y " + (metricUnitPresentation.code) + ".Necesario para obtener el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

				//}
				//else
				//{
				priceAux = priceUMPresentation * factorConversion;
				//}
			}


			if (list != null)
			{
				PriceListDetail listDetail = list.PriceListDetail.FirstOrDefault(d => d.id_item == item.id);
				var priceListDetail = listDetail?.salePrice;

				if (priceListDetail != null)
				{
					var detailtPriceList = list?.PriceListDetail.FirstOrDefault(fod => fod.id_item == item.id);
					var metricUnitPriceList = detailtPriceList?.MetricUnit;

					if (metricUnitPriceList != null)
					{
						var metricUnitSale = item.ItemSaleInformation.MetricUnit;

						var metricUnitAux = metricUnitPriceList;
						decimal priceMetricUnitPresentation = detailtPriceList.salePrice;

						if (metricUnitPriceList.id_metricType == metricUnitSale.id_metricType)
						{
							metricUnitAux = item.Presentation?.MetricUnit;

							//metricUnitDetail = metricUnitTypeUMPresentation;
							var factorConversionAux = GetFactorConversion(metricUnitSale, metricUnitPriceList, "Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle del Requerimiento de Pedido Configúrelo, e intente de nuevo", db);
							//var factorConversionAux = (metricUnitSale.id != metricUnitPriceList.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitSale.id &&
							//                                                                                                                 fod.id_metricDestiny == metricUnitPriceList.id)?.factor ?? 0 : 1;
							//priceAux = 0;
							//if (factorConversionAux == 0)
							//{
							//    throw new Exception("Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

							//}
							//else
							//{
							priceMetricUnitPresentation = (priceMetricUnitPresentation * factorConversionAux) / (item.Presentation?.minimum ?? 1);
							//}

							//priceMetricUnitPresentation = detailtPriceList.salePrice / (item.Presentation?.minimum ?? 1);
						}

						var metricUnitDetail = metricUnitTypeUMPresentation;

						var factorConversion = GetFactorConversion(metricUnitDetail, metricUnitAux, "Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para el precio del detalle del Requerimiento de Pedidio Configúrelo, e intente de nuevo", db);
						//var factorConversion = (metricUnitDetail.id != metricUnitAux.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitDetail.id &&
						//                                                                                                                 fod.id_metricDestiny == metricUnitAux.id)?.factor ?? 0 : 1;
						//priceAux = 0;
						//if (factorConversion == 0)
						//{
						//    throw new Exception("Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

						//}
						//else
						//{
						priceAux = priceMetricUnitPresentation * factorConversion;
						//}
					}
					else
					{
						priceAux = 0;
					}

				}
				else
				{
					priceAux = 0;
				}
			}

			return priceAux;
		}

		private decimal SaleDetailIVA(int id_item, decimal quantity, decimal price)
		{
			decimal iva = 0.0M;

			Item item = db.Item.FirstOrDefault(i => i.id == id_item);

			if (item == null)
			{
				return 0.0M;
			}

			List<ItemTaxation> taxations = item.ItemTaxation.Where(w => w.TaxType.code.Equals("2")).ToList();//"2" Es el Impuesto de IVA

			foreach (var taxation in taxations)
			{
				iva += quantity * price * taxation.Rate.percentage / 100.0M;
			}

			return iva;
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult RequestTotals()
		{
			SalesRequest salesRequest = (TempData["request"] as SalesRequest);

			salesRequest = salesRequest ?? new SalesRequest();
			salesRequest.SalesRequestDetail = salesRequest.SalesRequestDetail ?? new List<SalesRequestDetail>();

			salesRequest.SalesRequestTotal = SalesRequestTotals(salesRequest.id, salesRequest.SalesRequestDetail.ToList());

			TempData["request"] = salesRequest;
			TempData.Keep("request");

			var result = new
			{
				requestSubtotal = salesRequest.SalesRequestTotal.subtotal,
				requestSubtotalIVA12Percent = salesRequest.SalesRequestTotal.subtotalIVA12Percent,
				requestTotalIVA12 = salesRequest.SalesRequestTotal.totalIVA12,
				requestDiscount = salesRequest.SalesRequestTotal.discount,
				requestSubtotalIVA14Percent = salesRequest.SalesRequestTotal.subtotalIVA14Percent,
				requestTotalIVA14 = salesRequest.SalesRequestTotal.totalIVA14,
				requestTotal = salesRequest.SalesRequestTotal.total
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult ValidateSelectedRowsSalesQuotation(int[] ids)
		{
			var result = new
			{
				Message = "OK"
			};

			PriceList priceListFirst = null;
			PriceList priceListCurrent = null;
			Customer customerFirst = null;
			Customer customerCurrent = null;
			int count = 0;
			foreach (var i in ids)
			{
				priceListCurrent = db.SalesQuotationDetail.FirstOrDefault(fod => fod.id == i)?.SalesQuotation.PriceList;
				customerCurrent = db.SalesQuotationDetail.FirstOrDefault(fod => fod.id == i)?.SalesQuotation.Customer;

				if (count == 0)
				{
					priceListFirst = priceListCurrent;
					customerFirst = customerCurrent;
				}
				//else
				//{
				//    if (providerFirst == null)
				//    {
				//        providerFirst = providerCurrent;
				//    }
				//}



				if (priceListFirst != null && priceListCurrent != null && priceListCurrent != priceListFirst)
				{
					result = new
					{
						Message = ErrorMessage("No se pueden mezclar detalles con Lista de Precio diferente")
					};
					TempData.Keep("request");
					return Json(result, JsonRequestBehavior.AllowGet);
				}

				if (customerFirst != null && customerCurrent != null && customerCurrent != customerFirst)
				{
					result = new
					{
						Message = ErrorMessage("No se pueden mezclar detalles con clientes diferentes")
					};
					TempData.Keep("request");
					return Json(result, JsonRequestBehavior.AllowGet);
				}
				count++;
			}

			TempData.Keep("request");
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult OnPriceList_SelectedIndexChanged(int? id_priceList)
		{
			SalesRequest request = (TempData["request"] as SalesRequest);
			request = request ?? new SalesRequest();

			var priceList = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
			request.PriceList = priceList;
			var listSalesRequestDetail = request.SalesRequestDetail.ToList();
			decimal priceAux = 0;

			foreach (var salesRequestDetail in listSalesRequestDetail)
			{

				if (priceList != null)
				{
					//var metricUnitPresentation = salesQuotationDetail.Item.Presentation?.MetricUnit;
					var detailtPriceList = priceList?.PriceListDetail.FirstOrDefault(fod => fod.id_item == salesRequestDetail.id_item);
					var metricUnitPriceList = detailtPriceList?.MetricUnit;

					if (metricUnitPriceList != null)
					{
						var metricUnitSale = salesRequestDetail.Item.ItemSaleInformation.MetricUnit;

						var metricUnitAux = metricUnitPriceList;
						decimal priceMetricUnitPresentation = detailtPriceList.salePrice;

						if (metricUnitPriceList.id_metricType == metricUnitSale.id_metricType)
						{
							metricUnitAux = salesRequestDetail.Item.Presentation?.MetricUnit;
							//priceMetricUnitPresentation = detailtPriceList.salePrice / (salesQuotationDetail.Item.Presentation?.minimum ?? 1);
							var factorConversionAux = GetFactorConversion(metricUnitSale, metricUnitPriceList, "Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle del Requerimiento de Pedido Configúrelo, e intente de nuevo", db);
							//var factorConversionAux = (metricUnitSale.id != metricUnitPriceList.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitSale.id &&
							//                                                                                                                 fod.id_metricDestiny == metricUnitPriceList.id)?.factor ?? 0 : 1;
							////priceAux = 0;
							//if (factorConversionAux == 0)
							//{
							//    throw new Exception("Falta de Factor de Conversión entre : " + metricUnitSale.code + " y " + (metricUnitPriceList.code) + ".Necesario para el precio del detalle de la Cotización Configúrelo, e intente de nuevo");

							//}
							//else
							//{
							priceMetricUnitPresentation = (priceMetricUnitPresentation * factorConversionAux) / (salesRequestDetail.Item.Presentation?.minimum ?? 1);
							//}

							//priceMetricUnitPresentation = detailtPriceList.salePrice / (item.Presentation?.minimum ?? 1);

						}

						var metricUnitDetail = salesRequestDetail.MetricUnit;

						var factorConversion = GetFactorConversion(metricUnitDetail, metricUnitAux, "Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para recoste el detalle del Requerimiento de Pedido Configúrelo, e intente de nuevo", db);
						//var factorConversion = (metricUnitDetail.id != metricUnitAux.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitDetail.id &&
						//                                                                                                                 fod.id_metricDestiny == metricUnitAux.id)?.factor ?? 0 : 1;
						//priceAux = 0;
						//if (factorConversion == 0)
						//{
						//    throw new Exception("Falta de Factor de Conversión entre : " + metricUnitDetail.code + " y " + (metricUnitAux.code) + ".Necesario para recoste el detalle de la Cotización Configúrelo, e intente de nuevo");

						//}
						//else
						//{
						priceAux = priceMetricUnitPresentation * factorConversion;
						//}
					}
					else
					{
						priceAux = 0;
					}

				}
				else
				{
					priceAux = 0;
				}

				salesRequestDetail.price = priceAux;
			}

			//quotation.SalesQuotationTotal = SaleQuotationTotals(quotation.id, quotation.SalesQuotationDetail.ToList());

			var result = new
			{
				Message = "Ok"
			};

			TempData["request"] = request;
			TempData.Keep("request");
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult GetPriceList(int? id_customer)
		{
			TempData.Keep("request");
			var salesRequestAux = id_customer == null ? new SalesRequest() : new SalesRequest { id_customer = id_customer.Value };
			return PartialView("Component/_ComboBoxPriceList", salesRequestAux);
		}

		[HttpPost]
		public ActionResult GetItem(int? id_item)
		{
			SalesRequest request = (TempData["request"] as SalesRequest);
			request = request ?? new SalesRequest();
			TempData.Keep("request");

			return GridViewExtension.GetComboBoxCallbackResult(p => {
				//settings.Name = "id_person";
				p.ClientInstanceName = "id_item";
				p.Width = Unit.Percentage(100);

				p.DropDownStyle = DropDownStyle.DropDownList;
				p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
				p.EnableSynchronization = DefaultBoolean.False;
				p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

				p.CallbackRouteValues = new { Controller = "SalesRequest", Action = "GetItem" };
				p.CallbackPageSize = 5;
				//settings.Properties.EnableCallbackMode = true;
				//settings.Properties.TextField = "CityName";
				p.ClientSideEvents.BeginCallback = "SalesRequestItem_BeginCallback";
				p.ClientSideEvents.EndCallback = "SalesRequestItem_EndCallback";

				p.DataSource = DataProviderItem.SalesItemsPTByCompanyAndCurrent(this.ActiveCompanyId, id_item);
				p.ValueField = "id";
				//p.TextField = "name";
				p.TextFormatString = "{1}";
				p.ValueType = typeof(int);

				p.Columns.Add("masterCode", "Código", 70);
				p.Columns.Add("name", "Nombre del Producto", 300);
				p.Columns.Add("ItemSaleInformation.MetricUnit.code", "UM", 50);
				//p.ClientSideEvents.Init = "ItemCombo_OnInit";
				p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
				p.ClientSideEvents.SelectedIndexChanged = "ItemCombo_SelectedIndexChanged";
				p.ClientSideEvents.Validation = "OnItemValidation";

				//p.TextField = textField;
				//p.BindList(DataProviderPerson.RolByCompanyAndCurrentDistinctInBusinessOportunityCompetition(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_competitor, "Competidor", businessOportunity.BusinessOportunityCompetition.ToList()));//.Bind(id_person);

			});

			//return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
		}

		[HttpPost]
		public ActionResult GetMetricUnitTypeUMPresentation(int? id_item, int? id_metricUnitTypeUMPresentation)
		{
			SalesRequest request = (TempData["request"] as SalesRequest);
			request = request ?? new SalesRequest();
			TempData.Keep("request");

			return GridViewExtension.GetComboBoxCallbackResult(p => {
				//settings.Name = "id_person";
				p.ClientInstanceName = "id_metricUnitTypeUMPresentation";
				p.Width = Unit.Percentage(100);

				p.DropDownStyle = DropDownStyle.DropDownList;
				p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
				p.EnableSynchronization = DefaultBoolean.False;
				p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

				p.CallbackRouteValues = new { Controller = "SalesRequest", Action = "GetMetricUnitTypeUMPresentation" };
				p.CallbackPageSize = 5;
				//settings.Properties.EnableCallbackMode = true;
				//settings.Properties.TextField = "CityName";
				p.ClientSideEvents.BeginCallback = "SalesRequestMetricUnitTypeUMPresentation_BeginCallback";
				p.ClientSideEvents.EndCallback = "SalesRequestMetricUnitTypeUMPresentation_EndCallback";

				p.DataSource = DataProviderMetricUnit.MetricUnitTypeUMPresentation(this.ActiveCompanyId, id_item, id_metricUnitTypeUMPresentation);
				p.ValueField = "id";
				p.TextField = "code";
				p.Width = Unit.Percentage(100);
				p.ValueType = typeof(int);
				p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
				p.DropDownStyle = DropDownStyle.DropDownList;
				p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
				p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
				p.ClientSideEvents.SelectedIndexChanged = "MetricUnitTypeUMPresentationCombo_SelectedIndexChanged";
				p.ClientSideEvents.Validation = "OnMetricUnitTypeUMPresentationValidation";

				//p.BindList(DataProviderPerson.RolByCompanyAndCurrentDistinctInBusinessOportunityCompetition(this.ActiveCompanyId, businessOportunity.Document?.DocumentType?.code ?? "", id_competitor, "Competidor", businessOportunity.BusinessOportunityCompetition.ToList()));//.Bind(id_person);

			});

		}

		[HttpPost, ValidateInput(false)]
		public JsonResult ItsRepeatedItemDetail(int? id_itemNew)
		{
			SalesRequest request = (TempData["request"] as SalesRequest);

			request = request ?? new SalesRequest();

			var result = new
			{
				itsRepeated = 0,
				Error = ""

			};


			var requestDetailAux = request.SalesRequestDetail.FirstOrDefault(w => w.id_item == id_itemNew && w.isActive &&
																				 (w.SalesRequestDetailSalesQuotation == null || w.SalesRequestDetailSalesQuotation.Count() <= 0) &&
																				 (w.SalesRequestDetailBusinessOportunity == null || w.SalesRequestDetailBusinessOportunity.Count() <= 0));
			if (requestDetailAux != null)
			{
				var itemNewAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
				result = new
				{
					itsRepeated = 1,
					Error = "No se puede repetir el Ítem: " + itemNewAux.name + " en los detalles."

				};

			}

			TempData["request"] = request;
			TempData.Keep("request");

			return Json(result, JsonRequestBehavior.AllowGet);

		}

		[HttpPost, ValidateInput(false)]
		public JsonResult UpdateSalesRequested2()
		{
			SalesRequest request = (TempData["request"] as SalesRequest);

			SalesRequest request2 = (TempData["request2"] as SalesRequest);

			request2 = Copy(request, request2);

			var result = new
			{
				Message = "OK"
			};

			TempData["request2"] = request2;
			TempData.Keep("request2");

			TempData["request"] = request;
			TempData.Keep("request");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}

}
